using System;

namespace MudSharp.Combat.Moves;

public class OpposeWithdrawFromMeleeMove : CombatMoveBase
{
	public override string Description => "Opposing someone withdrawing from melee";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		throw new NotImplementedException();
	}
}