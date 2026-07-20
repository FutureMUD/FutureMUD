#nullable enable

using MudSharp.Body;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

public sealed class BoardVehicleCombatMove : CombatMoveBase
{
	public const double BoardingStaminaCost = 5.0;

	private readonly IVehicle _vehicle;
	private readonly IVehicleOccupantSlotPrototype _slot;
	private readonly IVehicleAccessPoint? _accessPoint;

	public BoardVehicleCombatMove(ICharacter assailant, IVehicle vehicle, IVehicleOccupantSlotPrototype slot,
		IVehicleAccessPoint? accessPoint = null)
	{
		Assailant = assailant;
		_vehicle = vehicle;
		_slot = slot;
		_accessPoint = accessPoint;
	}

	public override string Description => "boarding a vehicle in combat";
	public override double StaminaCost => 0.0;
	public override double BaseDelay => 2.0;
	public override ExertionLevel AssociatedExertion => ExertionLevel.Heavy;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (!Assailant.CanSpendStamina(BoardingStaminaCost))
		{
			Assailant.OutputHandler.Send("You are too exhausted to board that vehicle in combat.");
			return CombatMoveResult.Irrelevant;
		}

		if (!_vehicle.CanBoard(Assailant, _slot, _accessPoint, out var reason))
		{
			Assailant.OutputHandler.Send(reason);
			return CombatMoveResult.Irrelevant;
		}

		if (!_vehicle.Board(Assailant, _slot, _accessPoint))
		{
			Assailant.OutputHandler.Send("You are unable to complete your attempt to board the vehicle.");
			return CombatMoveResult.Irrelevant;
		}

		Assailant.SpendStamina(BoardingStaminaCost);
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote("@ haul|hauls $0 aboard $1 despite the fighting.",
			Assailant, Assailant, Assailant, _vehicle.ExteriorItem)));
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			AttackerOutcome = Outcome.NotTested,
			DefenderOutcome = Outcome.NotTested,
			RecoveryDifficulty = Difficulty.Normal
		};
	}
}
