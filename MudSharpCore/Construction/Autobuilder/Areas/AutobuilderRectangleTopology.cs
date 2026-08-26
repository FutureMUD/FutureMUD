using MudSharp.Construction.Boundary;

namespace MudSharp.Construction.Autobuilder.Areas;

internal readonly record struct AutobuilderRectangleConnection(
	int OriginX,
	int OriginY,
	int DestinationX,
	int DestinationY,
	CardinalDirection OutboundDirection,
	CardinalDirection InboundDirection);

internal static class AutobuilderRectangleTopology
{
	internal static IEnumerable<AutobuilderRectangleConnection> GetConnections(int width, int height,
		bool connectDiagonals)
	{
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				if (x + 1 < width)
				{
					yield return new AutobuilderRectangleConnection(x, y, x + 1, y,
						CardinalDirection.East, CardinalDirection.West);
				}

				if (y + 1 < height)
				{
					yield return new AutobuilderRectangleConnection(x, y, x, y + 1,
						CardinalDirection.North, CardinalDirection.South);
				}

				if (!connectDiagonals || x + 1 >= width)
				{
					continue;
				}

				if (y + 1 < height)
				{
					yield return new AutobuilderRectangleConnection(x, y, x + 1, y + 1,
						CardinalDirection.NorthEast, CardinalDirection.SouthWest);
				}

				if (y > 0)
				{
					yield return new AutobuilderRectangleConnection(x, y, x + 1, y - 1,
						CardinalDirection.SouthEast, CardinalDirection.NorthWest);
				}
			}
		}
	}

	internal static void ConnectCells(ICharacter builder, ICellOverlayPackage package, ICell[,] cells,
		bool connectDiagonals)
	{
		foreach (var connection in GetConnections(cells.GetLength(0), cells.GetLength(1), connectDiagonals))
		{
			var origin = cells[connection.OriginX, connection.OriginY];
			var destination = cells[connection.DestinationX, connection.DestinationY];
			if (origin == null || destination == null)
			{
				continue;
			}

			var exit = new Exit(builder.Gameworld, origin, destination, connection.OutboundDirection,
				connection.InboundDirection, 1.0);
			origin.GetOrCreateOverlay(package).AddExit(exit);
			destination.GetOrCreateOverlay(package).AddExit(exit);
		}
	}
}
