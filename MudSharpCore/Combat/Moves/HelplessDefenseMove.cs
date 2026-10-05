using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class HelplessDefenseMove : CombatMoveBase, IDefenseMove
{
    #region Overrides of CombatMoveBase

    public override string Description { get; } = "Helpless to defend themself";

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
        throw new NotImplementedException();
    }

    public int DifficultStageUps => 0;

    public void ResolveDefenseUsed(OpposedOutcome outcome)
    {
        // Do nothing
    }

    #endregion
}