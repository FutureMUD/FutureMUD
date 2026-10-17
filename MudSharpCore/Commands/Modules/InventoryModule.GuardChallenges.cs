using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class InventoryModule
{
	private static void GetWithGuardChallenge(ICharacter actor, string input)
	{
		if (!HostileGuardChallenge.Enabled(actor)) return;
		var arguments = input.RemoveFirstWord();
		var match = GetCommandRegex.Match(arguments);
		if (!match.Success || GetWeightRegex.IsMatch(arguments) || actor.Combat is not null ||
		    char.IsDigit(match.Groups[2].Value[0]) || match.Groups[2].Value.EqualTo("exactly"))
		{
			actor.Send("Guard challenges support only immediate get <item> [<container>] actions, including an optional quantity.");
			return;
		}
		var quantity = 0;
		if (match.Groups[1].Length > 0 && !int.TryParse(match.Groups[1].Value, out quantity))
		{
			actor.Send("That is not a valid quantity to get.");
			return;
		}
		PlayerEmote? emote = null;
		if (match.Groups[4].Length > 0)
		{
			emote = new PlayerEmote(new StringStack(match.Groups[4].Value).PopParentheses(), actor.Body);
			if (!emote.Valid) { actor.Send(emote.ErrorMessage); return; }
		}
		IGameItem? containerItem = null;
		IGameItem? item;
		if (match.Groups[3].Length > 0)
		{
			containerItem = actor.TargetItem(match.Groups[3].Value);
			var container = containerItem?.GetItemType<IContainer>();
			if (container is null || containerItem!.IsItemType<ICorpse>() ||
			    containerItem.GetItemType<IOpenable>()?.IsOpen == false)
			{
				actor.Send("You must choose an accessible open container for that guard challenge.");
				return;
			}
			item = container.Contents.GetFromItemListByKeyword(match.Groups[2].Value, actor);
		}
		else item = actor.TargetLocalItem(match.Groups[2].Value);
		if (item is null) { actor.Send("You do not see anything like that to get."); return; }
		using var attempt = HostileGuardChallenge.ForItem(actor, item, containerItem, GuardChallengeAction.Get);
		if (attempt is null) return;
		using var authority = attempt.Enter();
		if (containerItem is not null)
		{
			var manipulation = actor.CanManipulateItem(containerItem);
			if (!manipulation.Truth) { actor.Send(manipulation.Message); return; }
		}
		var canGet = containerItem is null ? actor.Body.CanGet(item, quantity) : actor.Body.CanGet(item, containerItem, quantity);
		if (!canGet)
		{
			actor.Send(containerItem is null ? actor.Body.WhyCannotGet(item, quantity) : actor.Body.WhyCannotGet(item, containerItem, quantity));
			return;
		}
		if (!attempt.Resolve()) return;
		if (containerItem is null) actor.Body.Get(item, quantity, emote);
		else actor.Body.Get(item, containerItem, quantity, emote);
	}
}
