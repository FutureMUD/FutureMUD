using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

#nullable enable
namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	[PlayerCommand("ItemPower", "itempower")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[HelpInfo("itempower", @"Invoke a configured magical item. Authority, costs, location, and available actions depend on its component settings.

	#3itempower <item> list#0 - lists a shrine's spell choices
	#3itempower <item> select <spell ID or quoted name> ...#0 - replaces your shrine selections
	#3itempower <item> use|recharge#0 - draws from or recharges a resource-restoring item
	#3itempower <item> status#0 - shows a resource-restoring item's remaining charges
	#3itempower <item> issue <prototype ID or quoted name>#0 - issues a configured item
	#3itempower <item> shock <online character>#0 - activates a matching worn binding, if authorised

Resource restorers refuse to spend a charge when the reserve is already full. Recharge territory, authority and binding activation range are builder settings.", AutoHelp.HelpArg)]
	protected static void ItemPower(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var item = actor.TargetItem(ss.PopSpeech());
		var action = ss.PopSpeech();
		if (item is null || string.IsNullOrWhiteSpace(action)) { actor.Send("Specify an accessible configured item and an action. See HELP ITEMPOWER."); return; }
		var components = item.Components.OfType<ConditionalMagicItemGameItemComponent>().ToArray();
		if (action.EqualTo("shock"))
		{
			var authority = components.FirstOrDefault(x => x.Accessible(actor));
			var text = ss.SafeRemainingArgument;
			var target = actor.TargetActor(text) ?? actor.Gameworld.Characters.GetByPersonalName(text);
			if (authority is null || target is null) { actor.Send("No eligible binding can be reached."); return; }
			foreach (var binding in target.Body.DirectWornItems.SelectMany(x => x.Components).OfType<IConditionalBindingItem>().Where(x => x.BindingGroup == authority.BindingGroup))
			{
				if (binding.Activate(actor, target, out var reason)) { actor.Send(reason); return; }
			}
			actor.Send("No eligible binding can be reached.");
			return;
		}
		foreach (var component in components)
		{
			if (component.Use(actor, action, new StringStack(ss.SafeRemainingArgument), out var reason)) { actor.Send(reason); return; }
		}
		actor.Send("That action is unavailable here. Check the item's authority, charges and configured choices.");
	}
}
