using ExpressionEngine;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;

#nullable enable

namespace MudSharp.Combat.ScatterStrategies;

public class ArcingScatterStrategy : IRangedScatterStrategy
{
    public static ArcingScatterStrategy Instance { get; } = new();
    private ArcingScatterStrategy() { }

    private static IExpression? _weightExpression;
    private static IExpression WeightExpression => _weightExpression ??= new Expression(Futuremud.Games.First().GetStaticConfiguration("ArcingScatterWeightExpression"));

    private static double Weight(IPerceiver candidate, IPerceiver target)
    {
        IExpression expr = WeightExpression;
        double weight = expr.EvaluateDoubleWith(("size", (int)candidate.Size),
            ("proximity", (int)candidate.GetProximity(target)));
        if (candidate.RoomLayer.IsHigherThan(target.RoomLayer))
        {
            weight *= 1.5;
        }
        return weight;
    }

    public RangedScatterResult? GetScatterTarget(ICharacter shooter, IPerceiver originalTarget,
        IEnumerable<IRoomExit> path)
    {
        if (originalTarget.Location == null)
        {
            return null;
        }

        List<(RoomScatterInfo Info, double Weight)> rooms = ScatterStrategyUtilities.GetRoomInfos(originalTarget, 1, true)
            .Select(info => (Info: info, Weight: RoomWeight(info)))
            .Where(x => x.Weight > 0)
            .ToList();

        if (!rooms.Any())
        {
            return null;
        }

        (RoomScatterInfo Info, double Weight) chosen = rooms.GetWeightedRandom(x => x.Weight);
		List<IPerceiver> candidates = ScatterStrategyUtilities
			.GetCandidatesAtImpact(chosen.Info, originalTarget, false)
            .Where(x => !x.Equals(originalTarget) && !x.Equals(shooter))
            .ToList();

        IPerceiver? target = candidates.GetWeightedRandom(x => Weight(x, originalTarget));
		return ScatterStrategyUtilities.CreateResult(chosen.Info, originalTarget, target);
    }

    private static double RoomWeight(RoomScatterInfo info)
    {
        double weight = 1.0 / (info.Distance + 1.0);
        if (info.Distance == 0)
        {
            weight *= 5.0;
        }

        return weight;
    }
}
