using MudSharp.Vehicles;

namespace MudSharp.GameItems.Interfaces;

public interface IVehicleInstallable : IGameItemComponent
{
	string MountType { get; }
	string Role { get; }
	bool IsInstalled { get; }
	IVehicleInstallation Installation { get; }
	void LinkInstallation(IVehicleInstallation installation);
	void ClearInstallation();
}
