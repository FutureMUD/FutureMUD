using MudSharp.Vehicles;

namespace MudSharp.GameItems.Interfaces;

public interface IVehicleAccessPointItem : IGameItemComponent
{
	long? VehicleId { get; }
	long? AccessPointId { get; }
	IVehicleAccessPoint AccessPoint { get; }
	void LinkAccessPoint(IVehicleAccessPoint accessPoint);
	void ClearAccessPoint(string reason);
}
