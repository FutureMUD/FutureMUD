#nullable enable

using MudSharp.Accounts;
using MudSharp.GameItems;

namespace MudSharp.Commands.Modules;

internal partial class StaffModule
{
	[PlayerCommand("Resize", "resize")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("resize", @"The #3resize#0 staff command sets the fit of a wearable item.

Syntax:
	#3resize <item> <fit>#0

Use #6unfitted#0, #6universal#0, #6for <character>#0, #6height <length> weight <mass>#0, or a scalar fit value.", AutoHelp.HelpArgOrNoArg)]
	protected static void Resize(ICharacter actor, string input)
	{
		StringStack ss = new(input.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which wearable item do you want to resize?");
			return;
		}

		var item = actor.TargetItem(ss.PopSpeech());
		if (item is null)
		{
			actor.OutputHandler.Send("You cannot see that item to resize it.");
			return;
		}

		var wearable = item.GetItemType<IWearable>();
		if (wearable is null)
		{
			actor.OutputHandler.Send($"{item.HowSeen(actor, true)} is not wearable.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What fit should that wearable have?");
			return;
		}

		var fitText = ss.SafeRemainingArgument;
		if (!WearableSizing.TryParse(fitText, actor, actor.Gameworld, out var fit, out var error))
		{
			actor.OutputHandler.Send(error.ColourError());
			return;
		}

		if (fitText.StartsWith("for ", StringComparison.OrdinalIgnoreCase))
		{
			var targetName = fitText[4..].Trim();
			var body = targetName.EqualTo("self") ? actor.Body : actor.TargetBody(targetName);
			if (body is null)
			{
				actor.OutputHandler.Send("There is no such individual to fit this for.".ColourError());
				return;
			}

			fit = wearable.FitFor(body);
		}

		wearable.SetFit(fit);
		actor.OutputHandler.Send($"You set the fit of {item.HowSeen(actor)}.");
	}
}
