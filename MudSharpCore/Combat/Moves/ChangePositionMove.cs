using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;

namespace MudSharp.Combat.Moves;

public class ChangePositionMove : CombatMoveBase
{
	public ChangePositionMove() : base()
	{
	}

	public IPositionState DesiredState { get; set; }
	public override string Description { get; } = "Attempting to change position.";
	public override double BaseDelay => 0.1;
	public override double StaminaCost => StandStaminaCost(Gameworld);

	public static double StandStaminaCost(IFuturemud gameworld)
	{
		// TODO - position specific costs?
		return gameworld.GetStaticDouble("StandMoveStaminaCost");
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		// Determine if anybody in the combat opposes them standing up
		var opponent =
			Assailant.Combat.Combatants.Where(x => x.CombatTarget == Assailant)
			         .SelectNotNull(x => x.ResponseToMove(this, Assailant))
			         .Shuffle()
			         .FirstOrDefault();
		if (opponent == null || opponent is HelplessDefenseMove)
		{
			var oldPosition = Assailant.PositionState;
			// Unopposed
			Assailant.MovePosition(DesiredState, PositionModifier.None, null, null, null, true);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = oldPosition.Upright || oldPosition.MoveRestrictions == MovementAbility.Free
					? Difficulty.Easy
					: Difficulty.Hard
			};
		}

		throw new NotImplementedException();
	}
}