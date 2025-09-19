using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Combat.ScatterStrategies;

internal readonly record struct CellScatterInfo(ICell Cell, int Distance, CardinalDirection DirectionFromOrigin);

internal static class ScatterStrategyUtilities
{
    public static IReadOnlyList<CellScatterInfo> GetCellInfos(IPerceiver originalTarget, uint range, bool respectDoors)
    {
        if (originalTarget.Location is null)
        {
            return Array.Empty<CellScatterInfo>();
        }

        var cells = originalTarget.CellsAndDistancesInVicinity(range, respectDoors, false).ToList();
        if (cells.Count == 0)
        {
            return Array.Empty<CellScatterInfo>();
        }

        var result = new List<CellScatterInfo>(cells.Count);
        foreach (var (cell, distance) in cells)
        {
            var direction = CardinalDirection.Unknown;
            if (distance > 0)
            {
                var dummy = new DummyPerceiver(location: cell)
                {
                    RoomLayer = originalTarget.RoomLayer
                };
                var pathToCell = originalTarget
                        .PathBetween(dummy, (uint)(distance + 1), false, false, true)
                        .ToList();
                if (pathToCell.Count > 0)
                {
                    direction = pathToCell[0].OutboundDirection;
                }
            }

            result.Add(new CellScatterInfo(cell, distance, direction));
        }

        return result;
    }

    public static string DescribeFromDirection(CardinalDirection direction)
    {
        return direction switch
        {
            CardinalDirection.Unknown => string.Empty,
            CardinalDirection.Up => " from above",
            CardinalDirection.Down => " from below",
            _ => $" from the {new[] { direction }.DescribeDirection()}"
        };
    }
}
