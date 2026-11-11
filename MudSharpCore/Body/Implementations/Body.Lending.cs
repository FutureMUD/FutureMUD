#nullable enable
using MudSharp.Economy.Lending;
using MudSharp.GameItems;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	internal void TakePledgedItem(GameItem item)
	{
		var entries = _heldItems.Where(x => ReferenceEquals(x.Item1, item)).ToArray();
		var index = _heldItems.FindIndex(x => ReferenceEquals(x.Item1, item));
		var holdable = item.GetItemType<IHoldable>();
		if (entries.Length != 1 || holdable == null || !ReferenceEquals(holdable.HeldBy, this))
			throw new InvalidOperationException("The pledged item is no longer held.");
		LoanService.OnRollback(() =>
		{
			_heldItems.Insert(Math.Min(index, _heldItems.Count), entries[0]);
			holdable.HeldBy = this;
			InventoryChanged = true;
		});
		_heldItems.RemoveAll(x => ReferenceEquals(x.Item1, item));
		holdable.HeldBy = null;
		InventoryChanged = true;
	}
}
