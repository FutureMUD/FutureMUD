#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.Commands.Modules;

public partial class StealthModule
{
	[PlayerCommand("Peek", "peek")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can peek.")]
	[NoCombatCommand]
	[NoMovementCommand]
	[HelpInfo("peek", @"Peek covertly inspects someone's belongings without moving anything.

	#3peek <character>#0 - inspect carried and worn equipment, including a chance to identify covered equipment
	#3peek <character> <container>#0 - inspect eligible contents of one visible, open container

Each attempt takes five seconds. You must know the Peek skill, even when the target is helpless. Inspection and being noticed are separate: you may see little while unnoticed, or learn something while being noticed. Only people who notice receive an alert. Peek never opens containers, takes items or automatically reports a crime. Closed containers, invisible items and inaccessible belongings remain protected.", AutoHelp.HelpArg)]
	protected static void Peek(ICharacter actor, string command)
	{
		if (!PeekInspection.CanUse(actor))
		{
			actor.OutputHandler.Send("You do not know how to covertly inspect someone's belongings.");
			return;
		}

		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Whose belongings do you want to peek at?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target is null || !PeekInspection.CanInspectTarget(actor, target))
		{
			actor.OutputHandler.Send("You cannot covertly inspect anyone like that here.");
			return;
		}

		var containerText = ss.SafeRemainingArgument;
		var container = string.IsNullOrWhiteSpace(containerText) ? null :
			PeekInspection.EligibleContainers(actor, target).GetFromItemListByKeyword(containerText, actor);
		if (!string.IsNullOrWhiteSpace(containerText) && container is null)
		{
			// Do not distinguish absent, concealed, closed or inaccessible containers.
			actor.OutputHandler.Send("You cannot peek into any such open, accessible container.");
			return;
		}

		actor.AddEffect(new PeekInspectionAction(actor, target, _ =>
		{
			// Inventory, visibility, reach and skill bindings are revalidated at completion.
			var result = PeekInspection.Inspect(actor, target, container);
			if (result.Interrupted)
			{
				actor.OutputHandler.Send("You cannot finish your covert inspection.");
				return;
			}

			actor.OutputHandler.Send(result.Items.Count == 0
				? "You cannot make out any belongings from that attempt."
				: $"You make out the following belongings:\n{string.Join("\n", result.Items.Select(x => $"\t{x.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSpotting)}"))}");
		}), TimeSpan.FromSeconds(5));
		actor.OutputHandler.Send("You begin covertly inspecting their belongings.");
	}
}
