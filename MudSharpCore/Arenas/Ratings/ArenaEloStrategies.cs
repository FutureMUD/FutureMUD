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
        List<IGrouping<int, ArenaRatingParticipant>> sideGroups = participants.GroupBy(x => x.SideIndex).ToList();
        if (sideGroups.Count < 2)
        {
            return new Dictionary<long, decimal>();
        }

        Dictionary<int, decimal> sideRatings = sideGroups.ToDictionary(
            group => group.Key,
            group => group.Select(p => p.StartingRating ?? ratingLookup(p.CharacterId, p.CombatantClass))
                .DefaultIfEmpty(ArenaRatingsService.DefaultRating)
                .Average());

        Dictionary<long, decimal> deltas = new();
        foreach (IGrouping<int, ArenaRatingParticipant> group in sideGroups)
        {
            decimal actual = ArenaEloMath.ComputeActualScore(outcome, group.Key, winningSides, sideGroups.Count);
            decimal expected = ComputeExpected(group.Key, sideRatings);
            decimal sideDelta = kFactor * (actual - expected);

            List<ArenaRatingParticipant> members = group.ToList();
            if (members.Count == 0)
            {
                continue;
            }

            decimal perParticipantDelta = sideDelta / members.Count;
            foreach (ArenaRatingParticipant participant in members)
            {
                deltas[participant.CharacterId] = deltas.TryGetValue(participant.CharacterId, out decimal existing)
                    ? existing + perParticipantDelta
                    : perParticipantDelta;
            }
        }

        return deltas;
    }

    private static decimal ComputeExpected(int sideIndex, IReadOnlyDictionary<int, decimal> sideRatings)
    {
        decimal rating = sideRatings.TryGetValue(sideIndex, out decimal value) ? value : ArenaRatingsService.DefaultRating;
        decimal total = 0.0m;
        int comparisons = 0;

        foreach ((int otherSide, decimal otherRating) in sideRatings)
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
        int sideCount = participants
            .Select(x => x.SideIndex)
            .Distinct()
            .Count();
        if (sideCount < 2)
        {
            return new Dictionary<long, decimal>();
        }

        Dictionary<long, decimal> ratings = participants
            .ToDictionary(x => x.CharacterId,
                x => x.StartingRating ?? ratingLookup(x.CharacterId, x.CombatantClass));

        Dictionary<long, decimal> deltas = new();
        foreach (ArenaRatingParticipant participant in participants)
        {
            List<ArenaRatingParticipant> opponents = participants
                .Where(x => x.SideIndex != participant.SideIndex)
                .ToList();
            if (opponents.Count == 0)
            {
                continue;
            }

            decimal actual = ArenaEloMath.ComputeActualScore(outcome, participant.SideIndex, winningSides, sideCount);
            decimal playerRating = ratings[participant.CharacterId];
            decimal rawDelta = opponents.Sum(opponent =>
            {
                decimal opponentRating = ratings[opponent.CharacterId];
                decimal expected = ArenaEloMath.ComputeExpectedScore(playerRating, opponentRating);
                return kFactor * (actual - expected);
            });
            decimal normalizedDelta = rawDelta / opponents.Count;

            deltas[participant.CharacterId] = deltas.TryGetValue(participant.CharacterId, out decimal existing)
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
        Dictionary<int, List<ArenaRatingParticipant>> sideGroups = participants
            .GroupBy(x => x.SideIndex)
            .ToDictionary(x => x.Key, x => x.ToList());
        if (sideGroups.Count < 2)
        {
            return new Dictionary<long, decimal>();
        }

        Dictionary<int, decimal> sideRatings = sideGroups.ToDictionary(
            x => x.Key,
            x => x.Value
                .Select(y => y.StartingRating ?? ratingLookup(y.CharacterId, y.CombatantClass))
                .DefaultIfEmpty(ArenaRatingsService.DefaultRating)
                .Average());

        List<int> sideIndices = sideGroups.Keys.OrderBy(x => x).ToList();
        int scale = Math.Max(1, sideIndices.Count - 1);
        Dictionary<long, decimal> deltas = new();

        for (int i = 0; i < sideIndices.Count; i++)
        {
            for (int j = i + 1; j < sideIndices.Count; j++)
            {
                int sideA = sideIndices[i];
                int sideB = sideIndices[j];
                decimal ratingA = sideRatings[sideA];
                decimal ratingB = sideRatings[sideB];
                decimal expectedA = ArenaEloMath.ComputeExpectedScore(ratingA, ratingB);
                decimal expectedB = ArenaEloMath.ComputeExpectedScore(ratingB, ratingA);
                decimal actualA = ArenaEloMath.ComputeActualScore(outcome, sideA, winningSides, sideIndices.Count);
                decimal actualB = ArenaEloMath.ComputeActualScore(outcome, sideB, winningSides, sideIndices.Count);
                decimal deltaA = (kFactor * (actualA - expectedA)) / scale;
                decimal deltaB = (kFactor * (actualB - expectedB)) / scale;

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

        decimal perParticipant = sideDelta / participants.Count;
        foreach (ArenaRatingParticipant participant in participants)
        {
            accumulator[participant.CharacterId] = accumulator.TryGetValue(participant.CharacterId, out decimal existing)
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
        double exponent = (double)((otherRating - rating) / 400.0m);
        double denominator = 1.0 + Math.Pow(10.0, exponent);
        return (decimal)(1.0 / denominator);
    }
}
