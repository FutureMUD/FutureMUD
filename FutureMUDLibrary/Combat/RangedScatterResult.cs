using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Combat;

/// <summary>
///     Represents the outcome of a ranged scatter calculation, including the room that receives the scattered shot,
///     the intended room layer and exact RouteRoom coordinate for any physical aftermath, and an optional target
///     that was struck. Ordinary rooms retain a null route coordinate.
/// </summary>
public sealed record RangedScatterResult(IRoom Room, RoomLayer RoomLayer, CardinalDirection DirectionFromTarget,
	int DistanceFromTarget, IPerceiver? Target, double? RoutePositionMetres = null)
{
	public SpatialLocation ImpactLocation => new(Room, RoomLayer, RoutePositionMetres);
}
