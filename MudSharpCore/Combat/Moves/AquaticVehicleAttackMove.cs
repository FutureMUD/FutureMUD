#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

public sealed class AquaticVehicleAttackMove : CombatMoveBase
{
	private readonly INaturalAttack _naturalAttack;
	private readonly IGameItem _targetItem;

	public AquaticVehicleAttackMove(ICharacter assailant, INaturalAttack naturalAttack, IGameItem targetItem)
	{
		Assailant = assailant;
		_naturalAttack = naturalAttack;
		_targetItem = targetItem;
		PrimaryTarget = targetItem;
	}

	public override string Description => "assaulting a vehicle from the water";
	public override double StaminaCost => NaturalAttackMove.MoveStaminaCost(Assailant, _naturalAttack.Attack);
	public override double BaseDelay => _naturalAttack.Attack.BaseDelay;
	public override ExertionLevel AssociatedExertion => _naturalAttack.Attack.ExertionLevel;
	public override Difficulty CheckDifficulty => _naturalAttack.Attack.Profile.BaseAttackerDifficulty;
	public override Difficulty RecoveryDifficultyFailure => _naturalAttack.Attack.RecoveryDifficultyFailure;
	public override Difficulty RecoveryDifficultySuccess => _naturalAttack.Attack.RecoveryDifficultySuccess;
	public override CheckType Check => CheckType.NaturalWeaponAttack;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var vehicle = _targetItem.GetItemType<IVehicleExterior>()?.Vehicle;
		if (vehicle is null || vehicle.Destroyed || !vehicle.IsSurfaceWaterVehicle() ||
		    !ReferenceEquals(vehicle.ExteriorItem, _targetItem) || !vehicle.Occupants.Any())
		{
			return CombatMoveResult.Irrelevant;
		}

		var unsupportedSwimmer = Assailant.Location == vehicle.Location &&
		                         Assailant.RoomLayer == vehicle.RoomLayer &&
		                         Assailant.Location?.IsSwimmingLayer(Assailant.RoomLayer) == true &&
		                         !Assailant.IsSupportedBySurfaceWaterVehicle(out _);
		if (!unsupportedSwimmer)
		{
			Assailant.OutputHandler.Send("You must be swimming alongside that craft to use an aquatic vehicle attack.");
			return CombatMoveResult.Irrelevant;
		}

		var attackOutcome = Gameworld.GetCheck(Check)
			.Check(Assailant, CheckDifficulty, _targetItem);
		if (!attackOutcome.Outcome.IsPass())
		{
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ surge|surges against $1, but fail|fails to unsettle the craft.", Assailant, Assailant,
				_targetItem), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return new CombatMoveResult
			{
				MoveWasSuccessful = false,
				AttackerOutcome = attackOutcome,
				DefenderOutcome = Outcome.NotTested,
				RecoveryDifficulty = RecoveryDifficultyFailure
			};
		}

		var additionalStages = (_naturalAttack.Attack as ISecondaryDifficultyAttack)?.SecondaryDifficulty.Difference(
			Difficulty.Normal) ?? 0;
		var occupants = vehicle.Occupants.ToList();
		var results = occupants
			.Select(x => (Occupant: x, Result: VehicleCombatService.Instance.ResolveDisplacement(x,
				VehicleCombatDisplacementType.AquaticVehicleAttack, attackOutcome.Outcome.SuccessDegrees(),
				additionalStages)))
			.ToList();
		var fallen = results.Where(x => x.Result.FellOverboard).Select(x => x.Occupant).ToList();

		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
			"@ slam|slams into $1 from the water, violently rocking the craft!", Assailant, Assailant, _targetItem),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		if (fallen.Any())
		{
			Assailant.OutputHandler.Send($"{fallen.Select(x => x.HowSeen(Assailant)).ListToString()} " +
				$"{(fallen.Count == 1 ? "is" : "are")} knocked into the water!");
		}

		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			AttackerOutcome = attackOutcome,
			DefenderOutcome = Outcome.NotTested,
			RecoveryDifficulty = RecoveryDifficultySuccess
		};
	}
}
