#nullable enable

using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Magic;

namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	[PlayerCommand("Compel", "compel")]
	[HelpInfo("compel", @"Order a nearby character affected by your Compelling Regard to perform one permitted in-character action. The target keeps ordinary command permissions, state and delay restrictions.

	#3compel <target> <command>#0

Permitted exact commands: look, drop, give, stand, sit, kneel, prostrate, lounge, rest, sprawl and prone. Abbreviations, aliases, indirect commands, queued actions, account and staff commands refuse.", AutoHelp.HelpArgOrNoArg)]
	protected static void Compel(ICharacter actor, string input)
	{
		var command = new StringStack(input.RemoveFirstWord()); var keyword = command.PopSpeech();
		var target = actor.TargetActor(keyword); var text = command.RemainingArgument;
		var effect = target?.Effects.OfType<SpellCompellingRegardEffect>().SingleOrDefault(x => x.CanControl(actor));
		if (target is null || effect is null || !SpellCompulsionExecution.Execute(actor, target, effect, text))
		{ actor.OutputHandler.Send("That character cannot obey that order through your Compelling Regard."); return; }
		actor.OutputHandler.Send("You compel the permitted action.");
	}
}
