using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves;

public class DesperateBlock : BlockMove
{
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.DesperateBlock;

	public override int DifficultStageUps => 3;
}