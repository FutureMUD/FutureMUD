#nullable enable

using MudSharp.Character;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems;

/// <summary>Explicit disposal boundaries; ordinary drops and deaths never imply retirement.</summary>
public static class ItemRetirement
{
	public static bool Retire(IGameItem item, ItemRetirementTrigger trigger)
	{
		if (item.Deleted || trigger == ItemRetirementTrigger.None || !item.GetItemTypes<ITransientItemPolicy>().Any(x => (x.Triggers & trigger) != 0)) return false;
		item.Delete();
		return item.Deleted;
	}

	public static void RetireCarriedItems(ICharacter actor, ItemRetirementTrigger trigger)
	{
		var body = actor.Body;
		foreach (var item in body.DirectItems.Where(x => !body.WornItems.Any(y => ReferenceEquals(y, x)) && !body.WieldedItems.Any(y => ReferenceEquals(y, x))).ToArray())
		{
			if (!ReferenceEquals(actor.Body, body)) return;
			if (!body.DirectItems.Any(x => ReferenceEquals(x, item)) || !ReferenceEquals(item.InInventoryOf, body) || item.ContainedIn is not null ||
			    body.WornItems.Any(x => ReferenceEquals(x, item)) || body.WieldedItems.Any(x => ReferenceEquals(x, item))) continue;
			Retire(item, trigger);
		}
	}
}
