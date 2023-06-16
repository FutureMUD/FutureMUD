using MudSharp.Body.Position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves;

internal class AdvantageMove : CombatMoveBase
{
	public AdvantageMove() : base()
	{
	}

	public override string Description { get; } = "Using an advantage move.";

	public IPositionState RequiredState { get; set; }

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		throw new NotImplementedException();
	}
}
