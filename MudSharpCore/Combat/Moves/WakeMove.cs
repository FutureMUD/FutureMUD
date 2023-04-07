using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class WakeMove : CombatMoveBase
{
	public override string Description => "Attempting to awaken from sleep";
	public override double BaseDelay => 0.1;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		Assailant.Awaken(null);

		// TODO - could this be character specific? Some wake up easier than others
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = Difficulty.Easy
		};
	}
}