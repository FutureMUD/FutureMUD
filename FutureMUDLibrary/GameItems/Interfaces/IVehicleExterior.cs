using MudSharp.Vehicles;

namespace MudSharp.GameItems.Interfaces;

public interface IVehicleExterior : IGameItemComponent
{
	long? VehicleId { get; }
	IVehicle Vehicle { get; }
	void LinkVehicle(IVehicle vehicle);
	void ClearVehicleLink(string reason);
	/// <summary>
	/// Updates the exterior's RouteCell coordinate as a projection of its canonical vehicle.
	/// The exterior component ignores its own forced-move callback for this update, while every
	/// other item component still receives the coordinate-change notification.
	/// </summary>
	bool TrySynchroniseRoutePosition(IVehicle vehicle, double? positionMetres);
}
