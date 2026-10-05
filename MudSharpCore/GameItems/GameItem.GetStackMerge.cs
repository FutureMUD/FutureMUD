#nullable enable

using System;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;

namespace MudSharp.GameItems;

public partial class GameItem
{
	internal void MergeCommittedStackForGet(GameItem absorbed, IBody holder)
	{
		var survivorStack = GetItemType<StackableGameItemComponent>();
		var sourceStack = absorbed.GetItemType<StackableGameItemComponent>();
		if (ReferenceEquals(this, absorbed) || survivorStack is null || sourceStack is null)
			throw new InvalidOperationException("Committed Get merge requires two distinct native stacks.");
		SpellOwnedItemValuePolicy.RequireOrdinaryValue(this, "merging");
		SpellOwnedItemValuePolicy.RequireOrdinaryValue(absorbed, "merging");
		ForeignCustodyTransferContext.EnsureItem(this, destructive: true);
		ForeignCustodyTransferContext.EnsureItem(absorbed, destructive: true);
		var quantity = checked(survivorStack.Quantity + sourceStack.Quantity);
		if (survivorStack.Quantity <= 0 || sourceStack.Quantity <= 0)
			throw new InvalidOperationException("Committed Get merge requires positive native quantities.");
		var owner = absorbed.OwnershipReference;
		var description = (absorbed.Prototype, absorbed.OverrideSdesc, absorbed.OverrideDesc);
		bool UnchangedAndEmpty() => !absorbed.Deleted && !absorbed.Destroyed && sourceStack.Quantity == 0 &&
			// Unknown/structural components can acquire foreign dependants in callbacks.
			// Only these exact native leaf components have qualified absorbed-source deletion.
			absorbed.Components.All(x => x.GetType() == typeof(StackableGameItemComponent) || x.GetType() == typeof(HoldableGameItemComponent)) &&
			absorbed.OwnershipReference == owner &&
			(absorbed.Prototype, absorbed.OverrideSdesc, absorbed.OverrideDesc) == description &&
			ReferenceEquals(absorbed.GetItemType<StackableGameItemComponent>(), sourceStack) &&
			ReferenceEquals(absorbed.GetItemType<IHoldable>()?.HeldBy, holder) &&
			absorbed.DirectLocation is null && absorbed.ContainedIn is null &&
			!holder.HeldOrWieldedItems.Any(x => ReferenceEquals(x, absorbed));

		// No description, shop or deletion observer can see duplicated source units.
		sourceStack.SetCommittedGetQuantity(0);
		survivorStack.SetCommittedGetQuantity(quantity);
		sourceStack.MarkCommittedGetQuantityChanged();
		survivorStack.MarkCommittedGetQuantityChanged();
		PsychometricRecorder.MergeHistory(this, absorbed);
		survivorStack.NotifyCommittedGetQuantityChanged();
		if (!absorbed.Deleted && !absorbed.Destroyed) sourceStack.NotifyCommittedGetQuantityChanged();
		if (!Deleted && !Destroyed && !absorbed.Deleted && !absorbed.Destroyed) NotifyStockItemMerge(absorbed);
		absorbed.DeleteCore(() =>
		{
			if (!UnchangedAndEmpty()) return false;
			if (absorbed._id != 0)
			{
				// Persist the conserved pair before native teardown aborts source saves. A
				// refused DELETE leaves a live zero source, rather than resurrecting its units.
				Gameworld.SaveManager.Flush();
				if (!UnchangedAndEmpty()) return false;
				try
				{
					using var caller = new FMDB();
					// A refused DELETE must not remain tracked as Deleted in a caller's
					// deferred context and destroy a subsequent independent refill.
					using (FMDB.BeginIndependentScope(requireWrites: true))
					using (new FMDB())
					{
						sourceStack.Save();
						survivorStack.Save();
						FMDB.Context.SaveChanges();
						var row = FMDB.Context.GameItems.Find(absorbed.Id);
						if (!UnchangedAndEmpty()) return false;
						if (row is not null)
						{
							FMDB.Context.GameItems.Remove(row);
							FMDB.Context.SaveChanges();
						}
					}
					// Successful independent deletion must also evict only this source's
					// cached graph, so the caller's native Find loader cannot resurrect it.
					foreach (var entry in FMDB.Context.ChangeTracker.Entries().Where(entry =>
						entry.Entity is MudSharp.Models.GameItem item && item.Id == absorbed.Id ||
						entry.Metadata.GetForeignKeys().Any(key => key.PrincipalEntityType.ClrType == typeof(MudSharp.Models.GameItem) &&
							key.Properties.Count == 1 && entry.Property(key.Properties[0].Name).CurrentValue is long id && id == absorbed.Id)).ToArray())
						entry.State = EntityState.Detached;
				}
				catch
				{
					sourceStack.MarkCommittedGetQuantityChanged();
					survivorStack.MarkCommittedGetQuantityChanged();
					throw;
				}
			}
			absorbed.GetItemType<IHoldable>()!.HeldBy = null;
			return true;
		}, UnchangedAndEmpty, persistedAlready: true);
	}
}
