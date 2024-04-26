using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class RepositionMove : CombatMoveBase
{
	public override string Description { get; } = "Attempting to change position.";
	public override double BaseDelay => 0.1;
	public override double StaminaCost => StandStaminaCost(Gameworld);
	public IPositionState TargetState { get; init; }
	public PositionModifier TargetModifier { get; init; }
	public IPerceivable TargetTarget { get; init; }
	public IEmote TargetEmote { get; init; }
	public IEmote Emote { get; init; }

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
			Assailant.MovePosition(TargetState, TargetModifier, TargetTarget, Emote, TargetEmote, true);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true,
				RecoveryDifficulty = Difficulty.Hard
			};
		}

		throw new NotImplementedException();
	}
}