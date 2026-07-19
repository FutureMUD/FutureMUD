using MudSharp.GameItems;
using MudSharp.GameItems.Components;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentWorkerItemLocator
{
	public static IEnumerable<IGameItem> HeldOrWieldedItems(ICharacter actor)
	{
		return actor.Body?.HeldOrWieldedItems ?? Enumerable.Empty<IGameItem>();
	}

	public static IEnumerable<IGameItem> TaskHeldItems(IEmploymentTaskContext context, ICharacter actor)
	{
		return context.CarriedTaskItems(actor)
		              .Concat(HeldOrWieldedItems(actor).SelectMany(SelfAndDeepItems))
		              .DistinctBy(x => x.Id);
	}

	public static bool IsHeldOrWielded(ICharacter actor, IGameItem item)
	{
		return HeldOrWieldedItems(actor)
		       .SelectMany(SelfAndDeepItems)
		       .Any(x => x.Id == item.Id);
	}

	private static IEnumerable<IGameItem> SelfAndDeepItems(IGameItem item)
	{
		yield return item;
		if (item.GetItemType<PileGameItemComponent>() is not { } pile)
		{
			yield break;
		}

		foreach (var child in pile.Contents)
		{
			if (child.Id != item.Id)
			{
				yield return child;
			}
		}
	}
}
