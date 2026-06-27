using MudSharp.Vehicles;

namespace MudSharp.GameItems.Interfaces;

public interface IVehicleInstallable : IGameItemComponent
{
	string MountType { get; }
	string Role { get; }
	double MinimumFunctionalCondition { get; }
	double MinimumMovementCondition { get; }
	bool IsInstalled { get; }
	IVehicleInstallation Installation { get; }
	bool IsFunctional(out string reason);
	bool IsFunctionalForMovement(out string reason);
	void LinkInstallation(IVehicleInstallation installation);
	void ClearInstallation();
}
