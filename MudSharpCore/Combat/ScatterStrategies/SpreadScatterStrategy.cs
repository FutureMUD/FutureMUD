using ExpressionEngine;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;

#nullable enable

namespace MudSharp.Combat.ScatterStrategies;

public class SpreadScatterStrategy : IRangedScatterStrategy
{
    public static SpreadScatterStrategy Instance { get; } = new();
    private SpreadScatterStrategy() { }

    private static IExpression? _weightExpression;
    private static IExpression WeightExpression => _weightExpression ??= new Expression(Futuremud.Games.First().GetStaticConfiguration("SpreadScatterWeightExpression"));

    private static double Weight(IPerceiver candidate, IPerceiver target)
    {
        IExpression expr = WeightExpression;
        return expr.EvaluateDoubleWith(("size", (int)candidate.Size),
            ("proximity", (int)candidate.GetProximity(target)));
    }

    public RangedScatterResult? GetScatterTarget(ICharacter shooter, IPerceiver originalTarget,
        IEnumerable<IRoomExit> path)
    {
        if (originalTarget.Location == null)
        {
            return null;
        }

        List<(RoomScatterInfo Info, double Weight)> rooms = ScatterStrategyUtilities.GetRoomInfos(originalTarget, 0, true)
            .Select(info => (Info: info, Weight: info.Distance == 0 ? 1.0 : 0.0))
            .Where(x => x.Weight > 0)
            .ToList();

        if (!rooms.Any())
        {
            return null;
        }

        (RoomScatterInfo Info, double Weight) chosen = rooms.GetWeightedRandom(x => x.Weight);
		List<IPerceiver> candidates = ScatterStrategyUtilities
			.GetCandidatesAtImpact(chosen.Info, originalTarget, false)
            .Where(x => !x.Equals(shooter) && !x.Equals(originalTarget))
            .ToList();

        IPerceiver? target = candidates.GetWeightedRandom(x => Weight(x, originalTarget));
		return ScatterStrategyUtilities.CreateResult(chosen.Info, originalTarget, target);
    }
}
