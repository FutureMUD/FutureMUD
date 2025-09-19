using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Construction;
using MudSharp.Framework;
using ExpressionEngine;

namespace MudSharp.Combat.ScatterStrategies;

public class ArcingScatterStrategy : IRangedScatterStrategy
{
    public static ArcingScatterStrategy Instance { get; } = new();
    private ArcingScatterStrategy() {}

    private static IExpression? _weightExpression;
    private static IExpression WeightExpression => _weightExpression ??= new Expression(Futuremud.Games.First().GetStaticConfiguration("ArcingScatterWeightExpression"));

    private static double Weight(IPerceiver candidate, IPerceiver target)
    {
        var expr = WeightExpression;
        expr.Parameters["size"] = (int)candidate.Size;
        expr.Parameters["proximity"] = (int)candidate.GetProximity(target);
        var weight = expr.EvaluateDouble();
        if (candidate.RoomLayer.IsHigherThan(target.RoomLayer))
        {
            weight *= 1.5;
        }
        return weight;
    }

    public RangedScatterResult? GetScatterTarget(ICharacter shooter, IPerceiver originalTarget,
        IEnumerable<ICellExit> path)
    {
        if (originalTarget.Location == null)
        {
            return null;
        }

        var cells = ScatterStrategyUtilities.GetCellInfos(originalTarget, 1, true)
            .Select(info => (Info: info, Weight: CellWeight(info)))
            .Where(x => x.Weight > 0)
            .ToList();

        if (!cells.Any())
        {
            return null;
        }

        var chosen = cells.GetWeightedRandom(x => x.Weight);
        var chosenCell = chosen.Info.Cell;
        var candidates = chosenCell.Characters.Cast<IPerceiver>()
            .Concat(chosenCell.GameItems)
            .Where(x => !x.Equals(originalTarget) && !x.Equals(shooter))
            .ToList();

        var target = candidates.GetWeightedRandom(x => Weight(x, originalTarget));
        return target != null
            ? new RangedScatterResult(chosenCell, target.RoomLayer, chosen.Info.DirectionFromOrigin, chosen.Info.Distance,
                target)
            : new RangedScatterResult(chosenCell, originalTarget.RoomLayer, chosen.Info.DirectionFromOrigin,
                chosen.Info.Distance, null);
    }

    private static double CellWeight(CellScatterInfo info)
    {
        var weight = 1.0 / (info.Distance + 1.0);
        if (info.Distance == 0)
        {
            weight *= 5.0;
        }

        return weight;
    }
}
