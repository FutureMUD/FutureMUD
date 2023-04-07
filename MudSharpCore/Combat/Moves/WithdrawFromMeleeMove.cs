using System;

namespace MudSharp.Combat.Moves;

public class WithdrawFromMeleeMove : CombatMoveBase
{
	public override string Description => "Withdrawing from Melee Combat";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		throw new NotImplementedException();
	}
}