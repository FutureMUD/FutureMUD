#nullable enable

using MudSharp.Construction;

namespace MudSharp.Framework;

/// <summary>
/// A physical location in the world. Ordinary rooms leave <see cref="RoutePositionMetres"/> null;
/// linear route rooms supply a coordinate measured from the route room's negative endpoint.
/// </summary>
public readonly record struct SpatialLocation(
	IRoom Room,
	RoomLayer Layer,
	double? RoutePositionMetres = null)
{
	/// <summary>
	/// The legacy cell-and-layer projection of this location.
	/// </summary>
	public InRoomLocation InRoomLocation => new()
	{
		Location = Room,
		RoomLayer = Layer
	};

	/// <summary>
	/// True when this location carries a coordinate within a linear route room.
	/// </summary>
	public bool HasRoutePosition => RoutePositionMetres.HasValue;

	/// <summary>
	/// Tests raw room-and-layer membership without applying spatial proximity rules.
	/// </summary>
	public bool SharesRoomLayerWith(SpatialLocation other)
	{
		return ReferenceEquals(Room, other.Room) && Layer == other.Layer;
	}
}
