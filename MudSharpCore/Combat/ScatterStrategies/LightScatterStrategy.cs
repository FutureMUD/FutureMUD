using ExpressionEngine;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.ScatterStrategies;

public class LightScatterStrategy : IRangedScatterStrategy
{
    public static LightScatterStrategy Instance { get; } = new();
    private LightScatterStrategy() { }

    private static IExpression? _weightExpression;
    private static IExpression WeightExpression => _weightExpression ??= new Expression(Futuremud.Games.First().GetStaticConfiguration("LightScatterWeightExpression"));

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
        // Up to 5 cells away
        if (originalTarget.Location == null)
        {
            return null;
        }

        List<ICellExit> pathList = path?.ToList() ?? new List<ICellExit>();
        (int Northness, int Southness, int Westness, int Eastness, int Upness, int Downness) counts = pathList.CountDirections();
        HashSet<CardinalDirection> directionSet = new(
            (counts.Northness, counts.Southness, counts.Westness, counts.Eastness, counts.Upness, counts.Downness)
            .ContainedDirections()
            .Where(x => x != CardinalDirection.Unknown));
        CardinalDirection lastDirection = pathList.LastOrDefault()?.OutboundDirection ?? CardinalDirection.Unknown;
        if (lastDirection != CardinalDirection.Unknown)
        {
            directionSet.Add(lastDirection);
        }

        List<(CellScatterInfo Info, double Weight)> cells = ScatterStrategyUtilities.GetCellInfos(originalTarget, 5, true)
            .Select(info => (Info: info, Weight: CellWeight(info, directionSet, lastDirection)))
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
            .Where(x => !x.Equals(shooter) && !x.Equals(originalTarget) && x.RoomLayer == originalTarget.RoomLayer)
            .ToList();

        IPerceiver target = candidates.GetWeightedRandom(x => Weight(x, originalTarget));
        return target != null
            ? new RangedScatterResult(chosenCell, target.RoomLayer, chosen.Info.DirectionFromOrigin, chosen.Info.Distance,
                target)
            : new RangedScatterResult(chosenCell, originalTarget.RoomLayer, chosen.Info.DirectionFromOrigin,
                chosen.Info.Distance, null);
    }

    private static double CellWeight(CellScatterInfo info, HashSet<CardinalDirection> preferredDirections,
        CardinalDirection lastDirection)
    {
        double weight = 1.0 / (info.Distance + 0.5);
        if (info.Distance == 0)
        {
            weight *= 0.25;
            return weight;
        }

        CardinalDirection direction = info.DirectionFromOrigin;
        if (direction == CardinalDirection.Unknown)
        {
            return weight;
        }

        if (preferredDirections.Contains(direction) || direction == lastDirection)
        {
            weight *= 4.0 + (info.Distance * 0.75);
        }
        else if (preferredDirections.Contains(direction.Opposite()))
        {
            weight *= 0.6;
        }
        else
        {
            weight *= 0.85;
        }

        return weight;
    }
}
