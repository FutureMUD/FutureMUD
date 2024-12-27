using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class LoadRangedWeaponMove : CombatMoveBase
{
	public override string Description => $"Loading {Weapon.Parent.HowSeen(Assailant)}";

	public override double StaminaCost => Weapon.WeaponType.StaminaPerLoadStage;

	#region Overrides of CombatMoveBase

	public override double BaseDelay
		=> Weapon.WeaponType.LoadCombatDelay;

	#endregion

	public IRangedWeapon Weapon { get; init; }
	public LoadMode Mode { get; init; }

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (!Weapon.CanLoad(Assailant, true, Mode))
		{
			Assailant.OutputHandler.Send(Weapon.WhyCannotLoad(Assailant, true, Mode));
			return CombatMoveResult.Irrelevant;
		}

		Weapon.Load(Assailant, true, Mode);
		return new CombatMoveResult
		{
			RecoveryDifficulty = Difficulty.Easy,
			MoveWasSuccessful = true
		};
	}
}