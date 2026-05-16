using MudSharp.Character;
using MudSharp.Construction.Boundary;

namespace MudSharp.Vehicles;

public interface IVehicleMovementStrategy
{
	VehicleMovementProfileType MovementType { get; }
	bool CanMove(IVehicle vehicle, ICharacter actor, ICellExit exit, out string reason);
	bool Move(IVehicle vehicle, ICharacter actor, ICellExit exit);
}
