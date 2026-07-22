#nullable enable

using MudSharp.Construction;

namespace MudSharp.Framework;

/// <summary>
/// A physical location in the world. Ordinary cells leave <see cref="RoutePositionMetres"/> null;
/// linear route cells supply a coordinate measured from the route cell's negative endpoint.
/// </summary>
public readonly record struct SpatialLocation(
	ICell Cell,
	RoomLayer Layer,
	double? RoutePositionMetres = null)
{
	/// <summary>
	/// The legacy cell-and-layer projection of this location.
	/// </summary>
	public InRoomLocation InRoomLocation => new()
	{
		Location = Cell,
		RoomLayer = Layer
	};

	/// <summary>
	/// True when this location carries a coordinate within a linear route cell.
	/// </summary>
	public bool HasRoutePosition => RoutePositionMetres.HasValue;

	/// <summary>
	/// Tests raw cell-and-layer membership without applying spatial proximity rules.
	/// </summary>
	public bool SharesCellLayerWith(SpatialLocation other)
	{
		return ReferenceEquals(Cell, other.Cell) && Layer == other.Layer;
	}
}
