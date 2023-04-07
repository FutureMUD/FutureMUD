using System;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class StandMove : CombatMoveBase
{
	public StandMove() : base()
	{
	}

	public override string Description { get; } = "Attempting to stand up.";
	public override double BaseDelay => 0.1;
	public override double StaminaCost => StandStaminaCost(Gameworld);

	public static double StandStaminaCost(IFuturemud gameworld)
	{
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
			// Unopposed
			Assailant.MovePosition(PositionStanding.Instance, PositionModifier.None, null, null, null, true);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = Difficulty.Hard
			};
		}

		throw new NotImplementedException();
	}
}