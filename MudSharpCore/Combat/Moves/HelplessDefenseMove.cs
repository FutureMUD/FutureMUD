using MudSharp.RPG.Checks;
using System;

namespace MudSharp.Combat.Moves;

public class HelplessDefenseMove : CombatMoveBase, IDefenseMove
{
	#region Overrides of CombatMoveBase

	public override string Description { get; } = "Helpless to defend themself";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		throw new NotImplementedException();
	}

	public int DifficultStageUps => 0;

	public void ResolveDefenseUsed(OpposedOutcome outcome)
	{
		// Do nothing
	}

	#endregion
}