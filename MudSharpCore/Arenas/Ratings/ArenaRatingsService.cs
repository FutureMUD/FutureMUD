#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Arenas;

/// <summary>
/// 	Persists combat arena ratings and provides a default Elo implementation.
/// </summary>
public class ArenaRatingsService : IArenaRatingsService
{
	internal const decimal DefaultRating = 1500.0m;
	private const decimal DefaultKFactor = 32.0m;

	private readonly Func<FuturemudDatabaseContext>? _contextFactory;
	private readonly IReadOnlyDictionary<ArenaEloStyle, IArenaEloStrategy> _eloStrategies;

	public ArenaRatingsService(IFuturemud gameworld, Func<FuturemudDatabaseContext>? contextFactory = null)
	{
		_ = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
		_contextFactory = contextFactory;
		_eloStrategies = new IArenaEloStrategy[]
		{
			new TeamAverageEloStrategy(),
			new PairwiseIndividualEloStrategy(),
			new PairwiseSideEloStrategy()
		}.ToDictionary(x => x.Style);
	}

	/// <inheritdoc />
	public decimal GetRating(ICharacter character, ICombatantClass combatantClass)
	{
		if (character is null)
		{
			throw new ArgumentNullException(nameof(character));
		}

		if (combatantClass is null)
		{
			throw new ArgumentNullException(nameof(combatantClass));
		}

		return GetRating(character.Id, combatantClass);
	}

	private decimal GetRating(long characterId, ICombatantClass combatantClass)
	{
		using var scope = BeginContext(out var context);
		var rating = context.ArenaRatings.FirstOrDefault(x => x.CharacterId == characterId &&
		                                               x.CombatantClassId == combatantClass.Id);
		return rating?.Rating ?? DefaultRating;
	}

	/// <inheritdoc />
	public void UpdateRatings(IArenaEvent arenaEvent, IReadOnlyDictionary<ICharacter, decimal> deltas)
	{
		if (arenaEvent is null)
		{
			throw new ArgumentNullException(nameof(arenaEvent));
		}

		if (deltas is null)
		{
			throw new ArgumentNullException(nameof(deltas));
		}

		if (deltas.Count == 0)
		{
			return;
		}

		var deltasByCharacterId = deltas
			.Where(x => x.Key is not null)
			.GroupBy(x => x.Key.Id)
			.ToDictionary(x => x.Key, x => x.Sum(y => y.Value));
		UpdateRatingsByCharacterId(arenaEvent, deltasByCharacterId);
	}

	private void UpdateRatingsByCharacterId(IArenaEvent arenaEvent, IReadOnlyDictionary<long, decimal> deltasByCharacterId)
	{
		if (deltasByCharacterId.Count == 0)
		{
			return;
		}

		var participantsByCharacterId = SelectRateableParticipants(arenaEvent)
			.GroupBy(x => x.CharacterId)
			.ToDictionary(x => x.Key, x => x.First());

		if (participantsByCharacterId.Count == 0)
		{
			return;
		}

		using var scope = BeginContext(out var context);
		var now = DateTime.UtcNow;
		foreach (var (characterId, delta) in deltasByCharacterId)
		{
			if (!participantsByCharacterId.TryGetValue(characterId, out var participant))
			{
				continue;
			}

			var rating = context.ArenaRatings.FirstOrDefault(x => x.CharacterId == characterId &&
			                                               x.CombatantClassId == participant.CombatantClass.Id);
			if (rating is null)
			{
				rating = new ArenaRating
				{
					ArenaId = arenaEvent.Arena.Id,
					CharacterId = characterId,
					CombatantClassId = participant.CombatantClass.Id,
					Rating = participant.StartingRating ?? DefaultRating,
					LastUpdatedAt = now
				};
				context.ArenaRatings.Add(rating);
			}

			rating.Rating = Math.Round(rating.Rating + delta, 2, MidpointRounding.AwayFromZero);
			rating.LastUpdatedAt = now;
		}

		context.SaveChanges();
	}

	/// <inheritdoc />
	public void ApplyDefaultElo(IArenaEvent arenaEvent)
	{
		if (arenaEvent is null)
		{
			throw new ArgumentNullException(nameof(arenaEvent));
		}

		var participants = SelectRateableParticipants(arenaEvent);
		if (participants.Count < 2)
		{
			return;
		}

		var (outcome, winningSides) = ResolveOutcome(arenaEvent);
		if (outcome is null || outcome == ArenaOutcome.Aborted)
		{
			return;
		}

		var sideGroups = participants.GroupBy(x => x.SideIndex).ToList();
		if (sideGroups.Count < 2)
		{
			return;
		}
		var winningSet = winningSides?.ToHashSet() ?? new HashSet<int>();
		var eventType = arenaEvent.EventType;
		var style = eventType?.EloStyle ?? ArenaEloStyle.TeamAverage;
		var kFactor = eventType is { EloKFactor: > 0.0m }
			? eventType.EloKFactor
			: DefaultKFactor;
		var strategy = _eloStrategies.TryGetValue(style, out var selectedStrategy)
			? selectedStrategy
			: _eloStrategies[ArenaEloStyle.TeamAverage];
		var deltaByCharacterId = strategy.CalculateDeltas(
			participants,
			outcome.Value,
			winningSet,
			kFactor,
			GetRating);
		if (deltaByCharacterId.Count == 0)
		{
			return;
		}

		UpdateRatingsByCharacterId(arenaEvent, deltaByCharacterId);
	}

	private static List<ArenaRatingParticipant> SelectRateableParticipants(IArenaEvent arenaEvent)
	{
		return arenaEvent.Participants
			.Select(x =>
			{
				var characterId = x.CharacterId;
				if (characterId <= 0)
				{
					characterId = x.Character?.Id ?? 0L;
				}

				return new ArenaRatingParticipant(characterId, x.SideIndex, x.CombatantClass, x.StartingRating);
			})
			.Where(x => x.CharacterId > 0)
			.ToList();
	}

	private IDisposable? BeginContext(out FuturemudDatabaseContext context)
	{
		if (_contextFactory is not null)
		{
			context = _contextFactory();
			return null;
		}

		var scope = new FMDB();
		context = FMDB.Context;
		return scope;
	}

	private static (ArenaOutcome?, IReadOnlyCollection<int>?) ResolveOutcome(IArenaEvent arenaEvent)
	{
		var outcome = TryExtractOutcome(arenaEvent);
		var winningSides = TryExtractWinningSides(arenaEvent);
		return (outcome, winningSides);
	}

	private static ArenaOutcome? TryExtractOutcome(IArenaEvent arenaEvent)
	{
		return arenaEvent.Outcome;
	}

	private static IReadOnlyCollection<int>? TryExtractWinningSides(IArenaEvent arenaEvent)
	{
		return arenaEvent.WinningSides;
	}

	private static bool TryConvertOutcome(object? value, out ArenaOutcome outcome)
	{
		switch (value)
		{
			case null:
				outcome = default;
				return false;
			case ArenaOutcome typed:
				outcome = typed;
				return true;
			case string text when Enum.TryParse<ArenaOutcome>(text, true, out var parsed):
				outcome = parsed;
				return true;
			case int numeric when Enum.IsDefined(typeof(ArenaOutcome), numeric):
				outcome = (ArenaOutcome)numeric;
				return true;
			default:
				outcome = default;
				return false;
		}
	}

	private static bool TryConvertWinningSides(object? value, out IReadOnlyCollection<int> sides)
	{
		switch (value)
		{
			case null:
				sides = Array.Empty<int>();
				return false;
			case IReadOnlyCollection<int> collection:
				sides = collection;
				return true;
			case IEnumerable<int> enumerable:
				sides = enumerable.ToList();
				return true;
			case IEnumerable<long> longs:
				sides = longs.Select(x => (int)x).ToList();
				return true;
			case IEnumerable<object> objects:
				sides = objects.Select(Convert.ToInt32).ToList();
				return true;
			default:
				sides = Array.Empty<int>();
				return false;
		}
	}

}
