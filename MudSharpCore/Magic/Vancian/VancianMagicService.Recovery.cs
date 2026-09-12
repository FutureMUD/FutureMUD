using MudSharp.GameItems.Components;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed partial class VancianMagicService
{
	private readonly HashSet<(long Owner, long Capability)> _reconciling = [];
	private readonly HashSet<(long Owner, long Capability)> _reconciled = [];

	private VancianCapabilityState ReadAndReconcile(long owner, long capability)
	{
		var state = Store.Read(owner, capability);
		var key = (owner, capability);
		lock (_guard)
			if (state.DataError is not null || _reconciled.Contains(key) || !_reconciling.Add(key)) return state;
		try
		{
			foreach (var operation in Store.ReservedOperations(owner, capability))
				CancelAbandonedReservation(state, operation);
			_reconciled.Add(key);
			return state;
		}
		finally { lock (_guard) _reconciling.Remove(key); }
	}

	private void CancelAbandonedReservation(VancianCapabilityState state, VancianOperation operation)
	{
		lock (_guard)
		{
			if (_writing.ContainsKey(operation.Id) || _writingItems.ContainsValue(operation.Id)) return;
			if (Store.Operation(operation.Id)?.Status != "Reserved" || operation.Kind is not ("Inscription" or "Transcription")) return;
			var slots = state.Slots.Where(x => x.Reservation == operation.Id).ToArray();
			if (slots.Any(x => x.Status != VancianSlotStatus.Reserved ||
				x.PreviousStatus is not (VancianSlotStatus.Prepared or VancianSlotStatus.AvailableSpontaneous)))
			{
				Store.Record(operation with { Status = "NeedsReview", Diagnostic = "Abandoned reservation has inconsistent prior slot state; no refund applied." });
				return;
			}
			foreach (var slot in slots)
			{
				slot.Status = slot.PreviousStatus!.Value;
				slot.PreviousStatus = null;
				slot.Reservation = null;
			}
			// Cancellation and the exact slot restoration are atomic. Items loaded later can use this terminal record.
			Store.Commit(state, state.Version, operation with { Status = "Cancelled",
				Diagnostic = "Abandoned precommit writing automatically cancelled. No costs, formula or charge created." });
			foreach (var id in new[] { operation.SourceItem, operation.DestinationItem }.OfType<long>())
				if (_gameworld.TryGetItem(id, true)?.GetItemType<ISpellScroll>() is SpellScrollGameItemComponent scroll && scroll.Reservation == operation.Id)
				{
					scroll.CancelReservation(operation.Id);
					Persist(scroll);
				}
		}
	}

	private void ReconcileScrollReservation(SpellScrollGameItemComponent scroll)
	{
		if (scroll.Reservation is not { } token) return;
		lock (_guard) if (_writing.ContainsKey(token) || _writingItems.ContainsValue(token)) return;
		var operation = Store.Operation(token);
		if (operation is null || operation.SourceItem != scroll.Parent.Id && operation.DestinationItem != scroll.Parent.Id) return;
		if (operation.Status == "Reserved")
		{
			var state = Store.Read(operation.OwnerId, operation.CapabilityId);
			if (state.DataError is not null) return;
			CancelAbandonedReservation(state, operation);
		}
		if (Store.Operation(token)?.Status != "Cancelled" || scroll.Reservation != token) return;
		scroll.CancelReservation(token);
		Persist(scroll);
	}
}
