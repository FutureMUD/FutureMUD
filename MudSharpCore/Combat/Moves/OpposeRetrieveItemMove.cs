
namespace MudSharp.Combat.Moves;

public class OpposeRetrieveItemMove : CombatMoveBase
{
    public override string Description => "Opposing the retrieval of a lost item";

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
		using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterMove(this);
		if (!CanContinueCommand()) return CombatMoveResult.Irrelevant;
        throw new NotImplementedException();
    }
}