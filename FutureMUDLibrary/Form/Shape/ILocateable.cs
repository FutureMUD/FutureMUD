using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Form.Shape;

public delegate void LocatableEvent(ILocateable locatable, ICellExit exit);
public delegate void SpatialLocationEvent(
	ILocateable locatable,
	SpatialLocation previousLocation,
	SpatialLocation currentLocation);

public interface ILocateable : IFrameworkItem, IKeyworded
{
	ICell Location { get; }

	RoomLayer RoomLayer { get; set; }

	/// <summary>
	/// The persisted coordinate within a linear route cell, in metres from its negative
	/// endpoint. Ordinary-cell implementations remain compatible by using the default null value.
	/// </summary>
	double? RoutePositionMetres => null;

	SpatialLocation SpatialLocation => new(Location, RoomLayer, RoutePositionMetres);

	/// <summary>
	/// Materialises a coordinate within a linear route cell. Ordinary locateables retain the
	/// default no-op implementation for source and binary compatibility.
	/// </summary>
	void SetRoutePosition(double? metres)
	{
	}

	InRoomLocation InRoomLocation => SpatialLocation.InRoomLocation;

	/// <summary>
	/// Tests raw cell-and-layer membership. Unlike <see cref="ColocatedWith"/>, this deliberately
	/// ignores longitudinal distance inside a route cell.
	/// </summary>
	bool SharesCellLayerWith(ILocateable? otherThing)
	{
		return otherThing is not null &&
		       ReferenceEquals(Location, otherThing.Location) &&
		       RoomLayer == otherThing.RoomLayer;
	}

	bool ColocatedWith(IPerceivable otherThing);
	event LocatableEvent OnLocationChanged;
	event LocatableEvent OnLocationChangedIntentionally;

	/// <summary>
	/// Fires when a locateable's coordinate changes without changing its cell or layer.
	/// The default no-op accessors preserve compatibility for ordinary-cell implementations.
	/// </summary>
	event SpatialLocationEvent OnSpatialPositionChanged
	{
		add { }
		remove { }
	}
}
