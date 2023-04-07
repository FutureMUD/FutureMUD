using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Inventory.Plans;

public static class InventoryPlanExtensions
{
	public static bool IsInInventoryState(this DesiredItemState state)
	{
		switch (state)
		{
			case DesiredItemState.Held:
			case DesiredItemState.Wielded:
			case DesiredItemState.Worn:
			case DesiredItemState.WieldedOneHandedOnly:
			case DesiredItemState.WieldedTwoHandedOnly:
				return true;
		}

		return false;
	}

	public static string Describe(this DesiredItemState state)
	{
		switch (state)
		{
			case DesiredItemState.InRoom:
				return "InRoom";
			case DesiredItemState.Held:
				return "Held";
			case DesiredItemState.Wielded:
				return "Wielded";
			case DesiredItemState.Worn:
				return "Worn";
			case DesiredItemState.Consumed:
				return "Consumed";
			case DesiredItemState.InContainer:
				return "InContainer";
			case DesiredItemState.Sheathed:
				return "Sheathed";
			case DesiredItemState.Attached:
				return "Attached";
			case DesiredItemState.ConsumeLiquid:
				return "ConsumeLiquid";
			case DesiredItemState.WieldedOneHandedOnly:
				return "Wielded 1-Hand";
			case DesiredItemState.WieldedTwoHandedOnly:
				return "Wielded 2-Hand";
			case DesiredItemState.Unknown:
				return "Unknown";
			default:
				throw new ApplicationException("Unknown DesiredItemState in Describe function.");
		}
	}
}