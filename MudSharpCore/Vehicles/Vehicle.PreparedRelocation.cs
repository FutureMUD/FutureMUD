#nullable enable

using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Vehicles;

public partial class Vehicle
{
	// The relocation participant has validated an empty, stationary native vehicle
	// between ordinary rooms. Publish its exterior and canonical position together.
	internal void CommitPreparedRelocation(Room destination, RoomLayer layer)
	{
		var exterior = (GameItem)ExteriorItem;
		((Room)Location).SetPreparedCurrencyRoomMembership(exterior, false);
		exterior.SetPreparedVehicleLocation(new SpatialLocation(destination, layer, null));
		destination.SetPreparedCurrencyRoomMembership(exterior, true);
		_currentRoomId = destination.Id;
		_roomLayer = layer;
		_routePositionMetres = null;
		_destinationRoutePositionMetres = null;
		_locationType = VehicleLocationType.Room;
		_movementStatus = VehicleMovementStatus.Stationary;
		_currentExitId = null;
		_destinationRoomId = null;
	}

	internal void FinishPreparedRelocation(Room origin, Room destination)
	{
		Changed = true;
		ExteriorItem.Changed = true;
		origin.ContentsChanged = true;
		destination.ContentsChanged = true;
		SuspendDockings();
		RouteSpatialService.Instance.TrackPerceivable(ExteriorItem);
		EnvironmentalExposureService.TrackExisting(Gameworld, ExteriorItem);
		ForceExteriorConnectablesMoved();
		EnsureExteriorWaterPosition();
		if (Prototype.Scale == VehicleScale.RoomScale) RebuildDockings();
		origin.CheckFallExitStatus();
		destination.FinishPreparedCurrencyInsertion(ExteriorItem, new SpatialLocation(destination, _roomLayer, null));
	}
}
