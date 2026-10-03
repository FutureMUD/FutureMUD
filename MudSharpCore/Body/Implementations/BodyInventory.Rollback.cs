#nullable enable

using System;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	internal Action CaptureCustodyInventoryRollback()
	{
		var held = _heldItems.ToArray(); var wielded = _wieldedItems.ToArray(); var worn = _wornItems.ToArray();
		var restorePosition = ((PerceivedItem)Actor).CaptureCustodyPositionRollback();
		var restoreBodyPosition = CaptureCustodyPositionRollback();
		return () =>
		{
			_heldItems.Clear(); _heldItems.AddRange(held);
			_wieldedItems.Clear(); _wieldedItems.AddRange(wielded);
			_wornItems.Clear(); _wornItems.AddRange(worn);
			restorePosition();
			restoreBodyPosition();
			_inventoryChanged = true; Changed = true;
		};
	}
}
