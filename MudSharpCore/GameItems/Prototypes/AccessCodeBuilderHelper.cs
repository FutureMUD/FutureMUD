#nullable enable

using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

internal static class AccessCodeBuilderHelper
{
	public static bool Handle(ICharacter actor, StringStack command, List<string> codes, Action changed,
		string owner)
	{
		var action = command.PopForSwitch();
		if (action.EqualTo("clear"))
		{
			if (!codes.Any())
			{
				actor.Send($"That {owner} has no initial codes to clear.");
				return false;
			}

			codes.Clear();
			changed();
			actor.Send($"That {owner} will now begin with no codes.");
			return true;
		}

		if (!action.EqualToAny("add", "remove") || command.IsFinished)
		{
			actor.Send($"Do you want to add, remove, or clear an initial code on that {owner}?");
			return false;
		}

		if (!AccessCredentialUtilities.TryNormaliseCode(command.SafeRemainingArgument, out var code, out var error))
		{
			actor.Send(error);
			return false;
		}

		var existing = codes.FirstOrDefault(x => x.Equals(code, StringComparison.Ordinal));
		if (action.EqualTo("add"))
		{
			if (existing is not null || codes.Count >= AccessCredentialUtilities.MaximumCodes)
			{
				actor.Send(existing is not null ? "That code is already present." : "That component cannot store any more codes.");
				return false;
			}

			codes.Add(code);
		}
		else
		{
			if (existing is null)
			{
				actor.Send("That code is not present.");
				return false;
			}

			codes.Remove(existing);
		}

		changed();
		actor.Send($"Code {code.ColourCommand()} was {(action.EqualTo("add") ? "added to" : "removed from")} that {owner}.");
		return true;
	}
}
