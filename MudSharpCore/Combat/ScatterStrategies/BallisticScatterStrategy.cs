using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Construction;
using MudSharp.Framework;
using ExpressionEngine;

namespace MudSharp.Combat.ScatterStrategies;

public class BallisticScatterStrategy : IRangedScatterStrategy
{
    public static BallisticScatterStrategy Instance { get; } = new();
    private BallisticScatterStrategy() {}

    private static IExpression? _weightExpression;
    private static IExpression WeightExpression => _weightExpression ??= new Expression(Futuremud.Games.First().GetStaticConfiguration("BallisticScatterWeightExpression"));

    private static double Weight(IPerceiver candidate, IPerceiver target)
    {
        var expr = WeightExpression;
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

        var pathList = path?.ToList() ?? new List<ICellExit>();
        var counts = pathList.CountDirections();
        var directionSet = new HashSet<CardinalDirection>(
            (counts.Northness, counts.Southness, counts.Westness, counts.Eastness, counts.Upness, counts.Downness)
            .ContainedDirections()
            .Where(x => x != CardinalDirection.Unknown));
        var lastDirection = pathList.LastOrDefault()?.OutboundDirection ?? CardinalDirection.Unknown;
        if (lastDirection != CardinalDirection.Unknown)
        {
            directionSet.Add(lastDirection);
        }

        var cells = ScatterStrategyUtilities.GetCellInfos(originalTarget, 3, true)
            .Select(info => (Info: info, Weight: CellWeight(info, directionSet, lastDirection)))
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
            .Where(x => !x.Equals(shooter) && !x.Equals(originalTarget) && x.RoomLayer == originalTarget.RoomLayer)
            .ToList();

        var target = candidates.GetWeightedRandom(x => Weight(x, originalTarget));
        return target != null
            ? new RangedScatterResult(chosenCell, target.RoomLayer, chosen.Info.DirectionFromOrigin, chosen.Info.Distance,
                target)
            : new RangedScatterResult(chosenCell, originalTarget.RoomLayer, chosen.Info.DirectionFromOrigin,
                chosen.Info.Distance, null);
    }

    private static double CellWeight(CellScatterInfo info, HashSet<CardinalDirection> preferredDirections,
        CardinalDirection lastDirection)
    {
        var weight = 1.0 / (info.Distance + 1.0);
        if (info.Distance == 0)
        {
            weight *= 0.5;
            return weight;
        }

        var direction = info.DirectionFromOrigin;
        if (direction == CardinalDirection.Unknown)
        {
            return weight;
        }

        if (preferredDirections.Contains(direction) || direction == lastDirection)
        {
            weight *= 3.0 + info.Distance;
        }
        else if (preferredDirections.Contains(direction.Opposite()))
        {
            weight *= 0.75;
        }
        else
        {
            weight *= 0.5;
        }

        return weight;
    }
}
