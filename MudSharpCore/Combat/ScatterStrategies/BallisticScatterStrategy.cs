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

    public IPerceiver? GetScatterTarget(ICharacter shooter, IPerceiver originalTarget,
        IEnumerable<ICellExit> path)
    {
        var cell = originalTarget.Location;
        if (cell == null)
        {
            return null;
        }

        var counts = path.CountDirections();
        var directions = (counts.Northness, counts.Southness, counts.Westness, counts.Eastness,
                counts.Upness, counts.Downness).ContainedDirections();

        foreach (var dir in directions)
        {
            var current = cell;
            for (var i = 0; i < 3 && current != null; i++)
            {
                var exit = current.GetExit(dir, shooter);
                if (exit == null || (!(exit.Exit.Door?.IsOpen ?? true) && !exit.Exit.Door.CanFireThrough))
                {
                    break;
                }

                current = exit.Destination;
                var candidates = current.Characters.Cast<IPerceiver>()
                    .Concat(current.GameItems)
                    .Where(x => !x.Equals(shooter) && x.RoomLayer == originalTarget.RoomLayer)
                    .ToList();
                if (!candidates.Any())
                {
                    continue;
                }

                var result = candidates.GetWeightedRandom(x => Weight(x, originalTarget));
                if (result != null)
                {
                    return result;
                }
            }
        }

        return null;
    }
}
