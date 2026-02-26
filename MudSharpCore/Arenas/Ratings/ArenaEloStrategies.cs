#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Arenas;

internal interface IArenaEloStrategy
{
	ArenaEloStyle Style { get; }
	IReadOnlyDictionary<long, decimal> CalculateDeltas(
		IReadOnlyList<ArenaRatingParticipant> participants,
		ArenaOutcome outcome,
		IReadOnlySet<int> winningSides,
		decimal kFactor,
		Func<long, ICombatantClass, decimal> ratingLookup);
}

internal sealed record ArenaRatingParticipant(long CharacterId, int SideIndex, ICombatantClass CombatantClass,
	decimal? StartingRating);

internal sealed class TeamAverageEloStrategy : IArenaEloStrategy
{
	public ArenaEloStyle Style => ArenaEloStyle.TeamAverage;

	public IReadOnlyDictionary<long, decimal> CalculateDeltas(
		IReadOnlyList<ArenaRatingParticipant> participants,
		ArenaOutcome outcome,
		IReadOnlySet<int> winningSides,
		decimal kFactor,
		Func<long, ICombatantClass, decimal> ratingLookup)
	{
		var sideGroups = participants.GroupBy(x => x.SideIndex).ToList();
		if (sideGroups.Count < 2)
		{
			return new Dictionary<long, decimal>();
		}

		var sideRatings = sideGroups.ToDictionary(
			group => group.Key,
			group => group.Select(p => p.StartingRating ?? ratingLookup(p.CharacterId, p.CombatantClass))
				.DefaultIfEmpty(ArenaRatingsService.DefaultRating)
				.Average());

		var deltas = new Dictionary<long, decimal>();
		foreach (var group in sideGroups)
		{
			var actual = ArenaEloMath.ComputeActualScore(outcome, group.Key, winningSides, sideGroups.Count);
			var expected = ComputeExpected(group.Key, sideRatings);
			var sideDelta = kFactor * (actual - expected);

			var members = group.ToList();
			if (members.Count == 0)
			{
				continue;
			}

			var perParticipantDelta = sideDelta / members.Count;
			foreach (var participant in members)
			{
				deltas[participant.CharacterId] = deltas.TryGetValue(participant.CharacterId, out var existing)
					? existing + perParticipantDelta
					: perParticipantDelta;
			}
		}

		return deltas;
	}

	private static decimal ComputeExpected(int sideIndex, IReadOnlyDictionary<int, decimal> sideRatings)
	{
		var rating = sideRatings.TryGetValue(sideIndex, out var value) ? value : ArenaRatingsService.DefaultRating;
		var total = 0.0m;
		var comparisons = 0;

		foreach (var (otherSide, otherRating) in sideRatings)
		{
			if (otherSide == sideIndex)
			{
				continue;
			}

			comparisons++;
			total += ArenaEloMath.ComputeExpectedScore(rating, otherRating);
		}

		if (comparisons == 0)
		{
			return 0.5m;
		}

		return total / comparisons;
	}
}

internal sealed class PairwiseIndividualEloStrategy : IArenaEloStrategy
{
	public ArenaEloStyle Style => ArenaEloStyle.PairwiseIndividual;

	public IReadOnlyDictionary<long, decimal> CalculateDeltas(
		IReadOnlyList<ArenaRatingParticipant> participants,
		ArenaOutcome outcome,
		IReadOnlySet<int> winningSides,
		decimal kFactor,
		Func<long, ICombatantClass, decimal> ratingLookup)
	{
		var sideCount = participants
			.Select(x => x.SideIndex)
			.Distinct()
			.Count();
		if (sideCount < 2)
		{
			return new Dictionary<long, decimal>();
		}

		var ratings = participants
			.ToDictionary(x => x.CharacterId,
				x => x.StartingRating ?? ratingLookup(x.CharacterId, x.CombatantClass));

		var deltas = new Dictionary<long, decimal>();
		foreach (var participant in participants)
		{
			var opponents = participants
				.Where(x => x.SideIndex != participant.SideIndex)
				.ToList();
			if (opponents.Count == 0)
			{
				continue;
			}

			var actual = ArenaEloMath.ComputeActualScore(outcome, participant.SideIndex, winningSides, sideCount);
			var playerRating = ratings[participant.CharacterId];
			var rawDelta = opponents.Sum(opponent =>
			{
				var opponentRating = ratings[opponent.CharacterId];
				var expected = ArenaEloMath.ComputeExpectedScore(playerRating, opponentRating);
				return kFactor * (actual - expected);
			});
			var normalizedDelta = rawDelta / opponents.Count;

			deltas[participant.CharacterId] = deltas.TryGetValue(participant.CharacterId, out var existing)
				? existing + normalizedDelta
				: normalizedDelta;
		}

		return deltas;
	}
}

internal sealed class PairwiseSideEloStrategy : IArenaEloStrategy
{
	public ArenaEloStyle Style => ArenaEloStyle.PairwiseSide;

	public IReadOnlyDictionary<long, decimal> CalculateDeltas(
		IReadOnlyList<ArenaRatingParticipant> participants,
		ArenaOutcome outcome,
		IReadOnlySet<int> winningSides,
		decimal kFactor,
		Func<long, ICombatantClass, decimal> ratingLookup)
	{
		var sideGroups = participants
			.GroupBy(x => x.SideIndex)
			.ToDictionary(x => x.Key, x => x.ToList());
		if (sideGroups.Count < 2)
		{
			return new Dictionary<long, decimal>();
		}

		var sideRatings = sideGroups.ToDictionary(
			x => x.Key,
			x => x.Value
				.Select(y => y.StartingRating ?? ratingLookup(y.CharacterId, y.CombatantClass))
				.DefaultIfEmpty(ArenaRatingsService.DefaultRating)
				.Average());

		var sideIndices = sideGroups.Keys.OrderBy(x => x).ToList();
		var scale = Math.Max(1, sideIndices.Count - 1);
		var deltas = new Dictionary<long, decimal>();

		for (var i = 0; i < sideIndices.Count; i++)
		{
			for (var j = i + 1; j < sideIndices.Count; j++)
			{
				var sideA = sideIndices[i];
				var sideB = sideIndices[j];
				var ratingA = sideRatings[sideA];
				var ratingB = sideRatings[sideB];
				var expectedA = ArenaEloMath.ComputeExpectedScore(ratingA, ratingB);
				var expectedB = ArenaEloMath.ComputeExpectedScore(ratingB, ratingA);
				var actualA = ArenaEloMath.ComputeActualScore(outcome, sideA, winningSides, sideIndices.Count);
				var actualB = ArenaEloMath.ComputeActualScore(outcome, sideB, winningSides, sideIndices.Count);
				var deltaA = (kFactor * (actualA - expectedA)) / scale;
				var deltaB = (kFactor * (actualB - expectedB)) / scale;

				DistributeDelta(sideGroups[sideA], deltaA, deltas);
				DistributeDelta(sideGroups[sideB], deltaB, deltas);
			}
		}

		return deltas;
	}

	private static void DistributeDelta(
		IReadOnlyCollection<ArenaRatingParticipant> participants,
		decimal sideDelta,
		Dictionary<long, decimal> accumulator)
	{
		if (participants.Count == 0)
		{
			return;
		}

		var perParticipant = sideDelta / participants.Count;
		foreach (var participant in participants)
		{
			accumulator[participant.CharacterId] = accumulator.TryGetValue(participant.CharacterId, out var existing)
				? existing + perParticipant
				: perParticipant;
		}
	}
}

internal static class ArenaEloMath
{
	public static decimal ComputeActualScore(ArenaOutcome outcome, int sideIndex, IReadOnlySet<int> winningSides, int totalSides)
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

	public static decimal ComputeExpectedScore(decimal rating, decimal otherRating)
	{
		var exponent = (double)((otherRating - rating) / 400.0m);
		var denominator = 1.0 + Math.Pow(10.0, exponent);
		return (decimal)(1.0 / denominator);
	}
}
