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
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
        if (!Weapon.CanReady(Assailant))
        {
            return CombatMoveResult.Irrelevant;
        }

        if (!Weapon.Ready(Assailant)) return CombatMoveResult.Irrelevant;
        return new CombatMoveResult
        {
            RecoveryDifficulty = Difficulty.Easy,
            MoveWasSuccessful = true
        };
    }

    public override bool UsesStaminaWithResult(CombatMoveResult result)
        => result.MoveWasSuccessful && base.UsesStaminaWithResult(result);
}
