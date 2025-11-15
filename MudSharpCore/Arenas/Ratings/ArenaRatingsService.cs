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
	private const decimal DefaultRating = 1500.0m;
	private const decimal DefaultKFactor = 32.0m;

	private readonly IFuturemud _gameworld;
	private readonly Func<FuturemudDatabaseContext>? _contextFactory;

	public ArenaRatingsService(IFuturemud gameworld, Func<FuturemudDatabaseContext>? contextFactory = null)
	{
		_gameworld = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
		_contextFactory = contextFactory;
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

		using var scope = BeginContext(out var context);
		var rating = context.ArenaRatings.FirstOrDefault(x => x.CharacterId == character.Id &&
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

		var participantsByCharacterId = arenaEvent.Participants
		                                     .Where(x => x.Character is not null)
		                                     .GroupBy(x => x.Character.Id)
		                                     .ToDictionary(x => x.Key, x => x.First());

		if (participantsByCharacterId.Count == 0)
		{
			return;
		}

		using var scope = BeginContext(out var context);
		var now = DateTime.UtcNow;
		foreach (var (character, delta) in deltas)
		{
			if (character is null)
			{
				continue;
			}

			if (!participantsByCharacterId.TryGetValue(character.Id, out var participant))
			{
				continue;
			}

			var combatantClass = participant.CombatantClass;
			if (combatantClass is null)
			{
				continue;
			}

			var rating = context.ArenaRatings.FirstOrDefault(x => x.CharacterId == character.Id &&
			                                               x.CombatantClassId == combatantClass.Id);
			if (rating is null)
			{
				rating = new ArenaRating
				{
					ArenaId = arenaEvent.Arena.Id,
					CharacterId = character.Id,
					CombatantClassId = combatantClass.Id,
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

		var participants = arenaEvent.Participants
		                            .Where(x => x.Character is not null && !x.IsNpc)
		                            .ToList();
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

		var sideRatings = sideGroups.ToDictionary(
		group => group.Key,
		group => group.Select(p => p.StartingRating ?? GetRating(p.Character, p.CombatantClass))
		      .DefaultIfEmpty(DefaultRating)
		      .Average());

		var deltaByCharacter = new Dictionary<ICharacter, decimal>();
		var winningSet = winningSides?.ToHashSet() ?? new HashSet<int>();

		foreach (var group in sideGroups)
		{
			var actual = ComputeActualScore(outcome.Value, group.Key, winningSet, sideGroups.Count);
			var expected = ComputeExpectedScore(group.Key, sideRatings);
			var sideDelta = DefaultKFactor * (actual - expected);

			var members = group.ToList();
			if (members.Count == 0)
			{
				continue;
			}

			var perParticipantDelta = sideDelta / members.Count;
			foreach (var participant in members)
			{
				var character = participant.Character;
				if (character is null)
				{
					continue;
				}

				deltaByCharacter[character] = deltaByCharacter.TryGetValue(character, out var existing)
					? existing + perParticipantDelta
					: perParticipantDelta;
			}
		}

		if (deltaByCharacter.Count == 0)
		{
			return;
		}

		UpdateRatings(arenaEvent, deltaByCharacter);
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

	private static decimal ComputeActualScore(ArenaOutcome outcome, int sideIndex, IReadOnlySet<int> winningSides, int totalSides)
	{
		return outcome switch
		{
			ArenaOutcome.Win => winningSides.Count == 0
				? (totalSides == 2 ? 1.0m : 0.0m)
				: (winningSides.Contains(sideIndex) ? 1.0m : 0.0m),
			ArenaOutcome.Draw => 0.5m,
			_ => 0.0m
		};
	}

	private static decimal ComputeExpectedScore(int sideIndex, IReadOnlyDictionary<int, decimal> sideRatings)
	{
		var rating = sideRatings.TryGetValue(sideIndex, out var value) ? value : DefaultRating;
		var total = 0.0m;
		var comparisons = 0;

		foreach (var (otherSide, otherRating) in sideRatings)
		{
			if (otherSide == sideIndex)
			{
				continue;
			}

			comparisons++;
			var exponent = (double)((otherRating - rating) / 400.0m);
			var denominator = 1.0 + Math.Pow(10.0, exponent);
			total += (decimal)(1.0 / denominator);
		}

		if (comparisons == 0)
		{
			return 0.5m;
		}

		return total / comparisons;
	}
}
