using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.GameItems;

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
		              .Concat(HeldOrWieldedItems(actor))
		              .DistinctBy(x => x.Id);
	}

	public static bool IsHeldOrWielded(ICharacter actor, IGameItem item)
	{
		return HeldOrWieldedItems(actor).Any(x => x.Id == item.Id);
	}
}