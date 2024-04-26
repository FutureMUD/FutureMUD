using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class ReadyRangedWeaponMove : CombatMoveBase
{
	public override string Description => $"Readying {Weapon.Parent.HowSeen(Assailant)}";

	public override double StaminaCost => Weapon.WeaponType.StaminaPerLoadStage;

	#region Overrides of CombatMoveBase

	public override double BaseDelay
		=> Weapon.WeaponType.ReadyCombatDelay;

	#endregion

	public IRangedWeapon Weapon { get; init; }

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (!Weapon.CanReady(Assailant))
		{
			return CombatMoveResult.Irrelevant;
		}

		Weapon.Ready(Assailant);
		return new CombatMoveResult
		{
			RecoveryDifficulty = Difficulty.Easy,
			MoveWasSuccessful = true
		};
	}
}