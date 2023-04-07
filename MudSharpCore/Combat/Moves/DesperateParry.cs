using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves;

public class DesperateParry : ParryMove
{
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.DesperateParry;

	public override int DifficultStageUps => 3;
}