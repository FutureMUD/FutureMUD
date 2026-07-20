#nullable enable

using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Vehicles;

public sealed record VehicleRangedCoverResult(
	IRangedCover Cover,
	IGameItem? Provider,
	bool IsVehicleCover,
	IVehicle? Vehicle,
	IVehicleOccupantSlotPrototype? Slot,
	VehicleRangedCoverDirection Direction);

public sealed record VehicleOverboardResult(
	bool WasApplicable,
	bool FellOverboard,
	Outcome StabilityOutcome,
	Difficulty Difficulty,
	IVehicle? Vehicle,
	IVehicleOccupantSlotPrototype? Slot);

public interface IVehicleCombatService
{
	IVehicle? VehicleFor(ICharacter character);
	VehicleRangedCoverDirection ClassifyRangedCoverDirection(IPerceiver attacker, IVehicle vehicle);
	VehicleRangedCoverResult? ResolveVehicleRangedCover(IPerceiver attacker, ICharacter target);
	VehicleRangedCoverResult? ResolveEffectiveRangedCover(IPerceiver attacker, ICharacter target);
	bool CanCrossVehicleBoundary(ICharacter attacker, ICharacter target, bool rangedAttack,
		bool aquaticVehicleAttack, out string reason);
	VehicleOverboardResult ResolveDisplacement(ICharacter target, VehicleCombatDisplacementType displacementType,
		int successDegrees = 1, int additionalDifficultyStages = 0);
}
