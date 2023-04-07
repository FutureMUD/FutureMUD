using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Effects.Concrete;

namespace MudSharp.Combat.Moves;

public class InterposeMove : CombatMoveBase
{
	public override string Description => "Attempting to interpose between ranged attacks.";

	public override double BaseDelay => 0.1;

	public IPerceivable Target { get; }

	public InterposeMove(ICharacter assailant, IPerceivable target)
	{
		Assailant = assailant;
		Target = target;
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		Assailant.AddEffect(new PassiveInterdiction.PassivelyInterceding(Assailant, Target));
		Assailant.OutputHandler.Handle(new EmoteOutput(
			new Emote(Gameworld.GetStaticString("PassiveInterdictionEmote"), Assailant, Assailant, Target),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		return CombatMoveResult.Irrelevant;
	}
}