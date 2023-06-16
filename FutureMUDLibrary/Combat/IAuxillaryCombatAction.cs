using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat
{
	public interface IAuxillaryCombatAction : IKeywordedItem
	{
		BuiltInCombatMoveType MoveType { get; set; }
		CombatMoveIntentions Intentions { get; set; }
		Difficulty RecoveryDifficultyFailure { get; set; }
		Difficulty RecoveryDifficultySuccess { get; set; }
		ExertionLevel ExertionLevel { get; set; }
		double StaminaCost { get; set; }
		double BaseDelay { get; set; }
		double Weighting { get; set; }
		IFutureProg UsabilityProg { get; set; }
		IEnumerable<IPositionState> RequiredPositionStates { get; }
		string ShowBuilder(ICharacter actor);
		bool BuildingCommand(ICharacter actor, StringStack command);
		IAuxillaryCombatAction Clone();
	}
}
