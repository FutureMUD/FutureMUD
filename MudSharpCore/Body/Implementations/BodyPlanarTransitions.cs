using System.Linq;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	public void EjectInventoryForPlanarTransition()
	{
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
