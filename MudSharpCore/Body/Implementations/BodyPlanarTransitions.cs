using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	public void EjectInventoryForPlanarTransition() => EjectInventoryForPlanarTransition([]);

	public void EjectInventoryForPlanarTransition(System.Collections.Generic.IReadOnlyCollection<long> destinationPlanes)
	{
		var executor = Actor;
		var originalActiveBody = executor.Body;
		foreach (var item in DirectItems.Where(x => !WornItems.Any(y => ReferenceEquals(y, x)) && !WieldedItems.Any(y => ReferenceEquals(y, x))).ToArray())
		{
			if (!ReferenceEquals(Actor, executor) || !ReferenceEquals(executor.Body, originalActiveBody)) return;
			if (item.Deleted || !ReferenceEquals(item.InInventoryOf, this) || item.ContainedIn is not null ||
			    !DirectItems.Any(x => ReferenceEquals(x, item)) || WornItems.Any(x => ReferenceEquals(x, item)) || WieldedItems.Any(x => ReferenceEquals(x, item))) continue;
			if (item.GetItemTypes<ITransientItemPolicy>().Any(x => x.Triggers.HasFlag(ItemRetirementTrigger.EtherealTransition) &&
			    (x.RetirementPlaneId == 0 || destinationPlanes.Contains(x.RetirementPlaneId))))
				ItemRetirement.Retire(item, ItemRetirementTrigger.EtherealTransition);
		}
		if (!ReferenceEquals(Actor, executor) || !ReferenceEquals(executor.Body, originalActiveBody)) return;
		var items = _heldItems.Select(x => x.Item1)
		                      .Concat(_wieldedItems.Select(x => x.Item1))
		                      .Concat(_wornItems.Select(x => x.Item))
		                      .Concat(_implants.Select(x => x.Parent))
		                      .Concat(_prosthetics.Select(x => x.Parent))
		                      .Distinct()
		                      .ToList();
		if (!items.Any())
		{
			return;
		}

		OutputHandler.Handle(new EmoteOutput(
			new Emote("@ shimmer|shimmers faintly as &0's physical belongings slip free and fall away.", Actor, Actor)));
		ClearDirectInventoryState();
		ClearImplantsAndProsthetics();
		foreach (IGameItem item in items)
		{
			DropTransferredItem(item);
		}
	}
}
