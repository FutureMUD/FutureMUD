
namespace MudSharp.Combat.Moves;

public class OpposeWithdrawFromMeleeMove : CombatMoveBase
{
    public override string Description => "Opposing someone withdrawing from melee";

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
        throw new NotImplementedException();
    }
}