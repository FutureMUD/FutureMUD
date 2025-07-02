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

    public IPerceiver? GetScatterTarget(ICharacter shooter, IPerceiver originalTarget,
        IEnumerable<ICellExit> path)
    {
        var cell = originalTarget.Location;
        if (cell == null)
        {
            return null;
        }

        var candidates = cell.Characters.Cast<IPerceiver>()
            .Concat(cell.GameItems)
            .Where(x => !x.Equals(originalTarget) && !x.Equals(shooter))
            .ToList();

        return candidates.GetWeightedRandom(x => Weight(x, originalTarget));
    }
}
