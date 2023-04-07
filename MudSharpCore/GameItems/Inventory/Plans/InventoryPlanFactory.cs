using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Inventory.Plans;

public static class InventoryPlanFactory
{
	public static IInventoryPlanTemplate CreatePhaseForEachItem(IFuturemud gameworld, IEnumerable<IGameItem> items,
		DesiredItemState state, object reference)
	{
		var phase = 1;
		return new InventoryPlanTemplate(gameworld,
			from item in items
			select new InventoryPlanPhaseTemplate(phase++,
				new[]
				{
					InventoryPlanAction.LoadAction(gameworld, state, 0, 0, x => x == item, null,
						originalReference: reference)
				})
		);
	}

	public static IInventoryPlan CreatePhaseForEachItem(ICharacter actor, IEnumerable<IGameItem> items,
		DesiredItemState state, object reference)
	{
		var phase = 1;
		return new InventoryPlanTemplate(actor.Gameworld,
				from item in items
				select new InventoryPlanPhaseTemplate(phase++,
					new[]
					{
						InventoryPlanAction.LoadAction(actor.Gameworld, state, 0, 0, x => x == item, null,
							originalReference: reference)
					})
			)
			.CreatePlan(actor);
	}
}