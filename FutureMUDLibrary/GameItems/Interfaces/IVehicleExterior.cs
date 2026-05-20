using MudSharp.Vehicles;

namespace MudSharp.GameItems.Interfaces;

public interface IVehicleExterior : IGameItemComponent
{
	long? VehicleId { get; }
	IVehicle Vehicle { get; }
	void LinkVehicle(IVehicle vehicle);
	void ClearVehicleLink(string reason);
}
