#nullable enable

using System.Collections.Generic;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// A path that may combine ordinary exit traversal with longitudinal movement inside route cells.
/// </summary>
public interface ISpatialPath
{
	SpatialLocation Origin { get; }
	SpatialLocation Destination { get; }
	IReadOnlyList<ISpatialPathStep> Steps { get; }
	double RouteDistanceMetres { get; }
	double RoomEquivalentCost { get; }
	IReadOnlyList<IRoomExit> TraversedExits { get; }
}

public interface ISpatialPathStep
{
	SpatialLocation Origin { get; }
	SpatialLocation Destination { get; }
	double RoomEquivalentCost { get; }
}

public interface ILinearRoutePathStep : ISpatialPathStep
{
	IRouteRoomDefinition RouteRoom { get; }
	RouteRoomDirection Direction { get; }
	double DistanceMetres { get; }
}

public interface IExitTraversalPathStep : ISpatialPathStep
{
	IRoomExit Exit { get; }
}
