using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class DesperateDodge : DodgeMove
{
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.DesperateDodge;

	public override int DifficultStageUps => 3;

	public override void ResolveDefenseUsed(OpposedOutcome outcome)
	{
		if (Assailant.PositionState == PositionSprawled.Instance && Assailant.CanMovePosition(PositionProne.Instance))
		{
			Assailant.MovePosition(PositionProne.Instance, PositionModifier.None, null, null, null);
		}
	}
}