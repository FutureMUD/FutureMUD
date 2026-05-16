using MudSharp.Vehicles;

namespace MudSharp.GameItems.Interfaces;

public interface IVehicleCargoSpaceItem : IGameItemComponent
{
	long? VehicleId { get; }
	long? CargoSpaceId { get; }
	IVehicleCargoSpace CargoSpace { get; }
	void LinkCargoSpace(IVehicleCargoSpace cargoSpace);
	void ClearCargoSpace(string reason);
}
