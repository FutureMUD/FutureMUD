using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Construction;
using MudSharp.Framework;
using ExpressionEngine;

namespace MudSharp.Combat.ScatterStrategies;

public class SpreadScatterStrategy : IRangedScatterStrategy
{
    public static SpreadScatterStrategy Instance { get; } = new();
    private SpreadScatterStrategy() {}

    private static IExpression? _weightExpression;
    private static IExpression WeightExpression => _weightExpression ??= new Expression(Futuremud.Games.First().GetStaticConfiguration("SpreadScatterWeightExpression"));

    private static double Weight(IPerceiver candidate, IPerceiver target)
    {
        var expr = WeightExpression;
        expr.Parameters["size"] = (int)candidate.Size;
        expr.Parameters["proximity"] = (int)candidate.GetProximity(target);
        return expr.EvaluateDouble();
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
            .Where(x => !x.Equals(shooter) && !x.Equals(originalTarget))
            .ToList();

        return candidates.GetWeightedRandom(x => Weight(x, originalTarget));
    }
}
