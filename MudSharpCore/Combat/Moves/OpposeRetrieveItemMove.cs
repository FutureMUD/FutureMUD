using System;

namespace MudSharp.Combat.Moves;

public class OpposeRetrieveItemMove : CombatMoveBase
{
	public override string Description => "Opposing the retrieval of a lost item";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		throw new NotImplementedException();
	}
}