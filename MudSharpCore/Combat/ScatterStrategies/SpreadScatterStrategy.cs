using ExpressionEngine;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

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
        expr.Parameters["size"] = (int)candidate.Size;
        expr.Parameters["proximity"] = (int)candidate.GetProximity(target);
        return expr.EvaluateDouble();
    }

    public RangedScatterResult? GetScatterTarget(ICharacter shooter, IPerceiver originalTarget,
        IEnumerable<ICellExit> path)
    {
        if (originalTarget.Location == null)
        {
            return null;
        }

        List<(CellScatterInfo Info, double Weight)> cells = ScatterStrategyUtilities.GetCellInfos(originalTarget, 0, true)
            .Select(info => (Info: info, Weight: info.Distance == 0 ? 1.0 : 0.0))
            .Where(x => x.Weight > 0)
            .ToList();

        if (!cells.Any())
        {
            return null;
        }

        (CellScatterInfo Info, double Weight) chosen = cells.GetWeightedRandom(x => x.Weight);
        ICell chosenCell = chosen.Info.Cell;
        List<IPerceiver> candidates = chosenCell.Characters.Cast<IPerceiver>()
            .Concat(chosenCell.GameItems)
            .Where(x => !x.Equals(shooter) && !x.Equals(originalTarget))
            .ToList();

        IPerceiver target = candidates.GetWeightedRandom(x => Weight(x, originalTarget));
        return target != null
            ? new RangedScatterResult(chosenCell, target.RoomLayer, chosen.Info.DirectionFromOrigin, chosen.Info.Distance,
                target)
            : new RangedScatterResult(chosenCell, originalTarget.RoomLayer, chosen.Info.DirectionFromOrigin,
                chosen.Info.Distance, null);
    }
}
