#nullable enable

using System.Collections.Generic;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// The authored one-dimensional spatial definition attached to a linear route cell.
/// Coordinates are expressed in metres from the negative endpoint and are inclusive in
/// the range zero through <see cref="LengthMetres"/>.
/// </summary>
public interface IRouteCellDefinition
{
	ICell Cell { get; }
	double LengthMetres { get; }
	double DefaultPositionMetres { get; }
	string PositiveDirectionName { get; }
	string NegativeDirectionName { get; }
	double MetresPerRoomEquivalent { get; }
	long TopologyVersion { get; }
	IReadOnlyList<IRouteCellLandmark> Landmarks { get; }
	IReadOnlyCollection<IRouteExitAnchor> ExitAnchors { get; }
}

/// <summary>
/// A named point used for navigation and local presentation within a route cell.
/// </summary>
public interface IRouteCellLandmark : IFrameworkItem, IKeyworded
{
	IRouteCellDefinition RouteCell { get; }
	double PositionMetres { get; }
	string Description { get; }
	int DisplayOrder { get; }
}

/// <summary>
/// The coordinate band in which one side of an exit can be used, together with the
/// coordinate assigned on arrival through that side of the exit.
/// </summary>
public interface IRouteExitAnchor
{
	ICellExit Exit { get; }
	ICell Cell { get; }
	double MinimumPositionMetres { get; }
	double MaximumPositionMetres { get; }
	double ArrivalPositionMetres { get; }

	bool Contains(double positionMetres)
	{
		return positionMetres >= MinimumPositionMetres && positionMetres <= MaximumPositionMetres;
	}
}
