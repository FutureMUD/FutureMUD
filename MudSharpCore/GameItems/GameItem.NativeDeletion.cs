#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using NativeItemDeletionStage = MudSharp.Models.NativeItemDeletionStage;

namespace MudSharp.GameItems;

public partial class GameItem
{
	internal bool NativeDeletionInProgress => _nativeDeletionStarted || _nativeRowDeleted;
	private bool _nativeDeletionCheckpointed;
	private bool _nativeDeletionReleased;
	private Exception? _nativeDeletionTeardownFailure;
	internal sealed record NativeDeletionNode(GameItem Item, GameItem? Container, IWound? LodgedIn, IWound[] Wounds);

	/// <summary>
	/// Commits qualified native containment and wound custody before runtime teardown.
	/// Only components with understood deletion behavior can participate.
	/// </summary>
	private bool CommitOrdinaryContainerDeletion()
	{
		var plan = BuildOrdinaryContainerDeletionPlan();
		if (plan is null) return false;
		if (!NeedsTransaction(plan) || FMDB.WritesAreSuppressed) return true;

		using (new FMDB())
		{
			var caller = FMDB.Context;
			if (!caller.Database.IsRelational() || caller.Database.CurrentTransaction is not null) return true;
			if (Gameworld.SaveManager.Flushing)
				throw new InvalidOperationException($"Item #{_id} cannot commit graph deletion during a save flush.");
			// Publish ordinary transfers first so validation also accepts a child that
			// was legitimately moved into this container since the last save.
			Gameworld.SaveManager.Flush();
			plan = BuildOrdinaryContainerDeletionPlan();
			if (plan is null) return false;
			if (!NeedsTransaction(plan)) return true;

			using (FMDB.BeginIndependentScope(requireWrites: true))
			using (new FMDB())
			{
				var context = FMDB.Context;
				using var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable);
				var rows = ValidateNativeDeletionCustody(context, plan);
				foreach (var node in plan)
				{
					DisableHitchLinks(context, node.Item._id);
					context.GameItems.Remove(rows[node.Item._id]);
					// Each command runs in child-first order inside the same transaction.
					context.SaveChanges();
				}

				transaction.Commit();
				foreach (var node in plan) node.Item.MarkNativeItemRowDeleted();
			}

			DetachDeletedItemRows(caller, plan.Select(x => x.Item._id).ToHashSet());
		}
		return true;

		static bool NeedsTransaction(IReadOnlyList<NativeDeletionNode> graph) => graph.Count > 1 || graph[0].Wounds.Length > 0 ||
			graph[0].Item.Components.Any(component => component is IContainer or ILockable);
	}

	internal static IReadOnlyDictionary<long, Models.GameItem> ValidateNativeDeletionCustody(
		FuturemudDatabaseContext context, IReadOnlyList<NativeDeletionNode> plan)
	{
		var ids = plan.Select(x => x.Item._id).ToArray();
		var planById = plan.ToDictionary(x => x.Item._id);
		var rows = context.GameItems.Where(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);
		var contained = context.GameItems.AsNoTracking()
			.Where(x => x.ContainerId.HasValue && ids.Contains(x.ContainerId.Value))
			.Select(x => new { x.Id, x.ContainerId }).ToList();
		foreach (var child in contained)
		{
			if (!planById.TryGetValue(child.Id, out var node) || node.Container?._id != child.ContainerId)
				throw new InvalidOperationException($"Item #{child.ContainerId} has an unaccounted child #{child.Id} in the deletion graph.");
		}

		var wounds = context.Wounds.AsNoTracking()
			.Where(x => x.GameItemId.HasValue && ids.Contains(x.GameItemId.Value) ||
			            x.LodgedItemId.HasValue && ids.Contains(x.LodgedItemId.Value))
			.Select(x => new { x.Id, x.GameItemId, x.BodyId, x.VehicleId, x.VehicleDamageZoneId, x.LodgedItemId, Infected = x.Infections.Any() })
			.ToList();
		var woundsByOwner = wounds.ToLookup(x => x.GameItemId);
		var woundsByLodged = wounds.ToLookup(x => x.LodgedItemId);
		foreach (var (item, container, lodgedIn, itemWounds) in plan)
		{
			if (!rows.TryGetValue(item._id, out var row))
				throw new InvalidOperationException($"Item #{item._id} is missing from the native deletion graph.");
			if (row.NativeDeletionStage is not null)
				throw new InvalidOperationException($"Item #{item._id} already has a native deletion checkpoint.");

			var ownedWounds = woundsByOwner[item._id].ToDictionary(x => x.Id);
			if (ownedWounds.Count != itemWounds.Length || itemWounds.Any(wound =>
			    !ownedWounds.TryGetValue(wound.Id, out var stored) || stored.Infected || stored.BodyId is not null ||
			    stored.VehicleId is not null || stored.VehicleDamageZoneId is not null || stored.LodgedItemId != wound.Lodged?.Id))
				throw new InvalidOperationException($"Persisted wounds of item #{item._id} disagree with the deletion graph.");

			if (container is null && lodgedIn is null) continue;
			var lodging = woundsByLodged[item._id];
			if (row.ContainerId is { } current && current != container?._id ||
			    (lodgedIn is null ? lodging.Any() : lodging.Count() != 1 || lodging.Single().Id != lodgedIn.Id) ||
			    context.RoomsGameItems.Any(x => x.GameItemId == item._id) ||
			    context.BodiesGameItems.Any(x => x.GameItemId == item._id) ||
			    context.BodiesImplants.Any(x => x.ImplantId == item._id) ||
			    context.BodiesProsthetics.Any(x => x.ProstheticId == item._id))
				throw new InvalidOperationException($"Persisted custody of item #{item._id} disagrees with the deletion graph.");
		}

		return rows;
	}

	internal void DeleteNativeItemWound(IWound wound)
	{
		// These rows have already been removed by the qualified graph transaction.
		if (_nativeRowDeleted && ReferenceEquals(wound.Parent, this))
		{
			wound.Delete(ignoreDatabaseDeletion: true);
			return;
		}

		if (ReferenceEquals(wound.Parent, this) && wound.Lodged is { } lodged)
		{
			if (IsDetachedLodgedItem(lodged) && HasUncontestedNativeLodging(wound, lodged))
			{
				lodged.Delete();
				if (ReferenceEquals(wound.Lodged, lodged) && IsDetachedLodgedItem(lodged))
					throw new InvalidOperationException($"Wound #{wound.Id} still owns item #{lodged.Id} after deletion.");
			}
			else
			{
				Gameworld.DebugMessage($"Item #{_id} conserves lodged item #{lodged.Id} during wound deletion because its custody is disputed.");
			}

			if (ReferenceEquals(wound.Lodged, lodged)) wound.Lodged = null!;
			if (wound.Lodged is not null)
				throw new InvalidOperationException($"Wound #{wound.Id} acquired a different lodged item during deletion.");
		}

		wound.Delete();

		static bool IsDetachedLodgedItem(IGameItem item) =>
			ComponentItemTransfer.IsDetached(item) && !ComponentItemTransfer.HasDirectBodyCustody(item);
	}

	private bool HasUncontestedNativeLodging(IWound wound, IGameItem lodged)
	{
		if (Gameworld is not Futuremud || lodged is not ILateInitialisingItem { IdHasBeenRegistered: true }) return true;
		using (new FMDB())
		{
			var context = FMDB.Context;
			var lodgedId = lodged.Id;
			var woundId = wound is ILateInitialisingItem { IdHasBeenRegistered: true } ? wound.Id : (long?)null;
			return !context.GameItems.Any(x => x.Id == lodgedId && x.ContainerId != null) &&
			       !context.RoomsGameItems.Any(x => x.GameItemId == lodgedId) &&
			       !context.BodiesGameItems.Any(x => x.GameItemId == lodgedId) &&
			       !context.BodiesImplants.Any(x => x.ImplantId == lodgedId) &&
			       !context.BodiesProsthetics.Any(x => x.ProstheticId == lodgedId) &&
			       !context.Wounds.Any(x => x.LodgedItemId == lodgedId && x.Id != woundId);
		}
	}

	private void BeginNativeDeletionCheckpoint()
	{
		if (_nativeDeletionCheckpointed || _id == 0 || Gameworld is not Futuremud || FMDB.WritesAreSuppressed ||
		    SpellCreationOrigin is not null || InAuctionEscrow()) return;

		using (new FMDB())
		{
			if (!FMDB.Context.Database.IsRelational() || FMDB.Context.Database.CurrentTransaction is not null) return;
			if (GetItemType<ICorpse>() is not null && FMDB.Context.MagicSpellLifecycles.Any(x =>
				    x.RemainsItemId == _id && x.Mode != (int)SpellLifecycleMode.Permanent)) return;
			if (Gameworld.SaveManager.Flushing)
				throw new InvalidOperationException($"Complex item #{_id} cannot begin deletion during a save flush.");
			SetNativeDeletionStage(_id, null, NativeItemDeletionStage.TeardownInProgress);
			_nativeDeletionCheckpointed = true;
		}
	}

	private void CancelNativeDeletionCheckpoint()
	{
		if (!_nativeDeletionCheckpointed) return;
		SetNativeDeletionStage(_id, NativeItemDeletionStage.TeardownInProgress, null);
		_nativeDeletionCheckpointed = false;
	}

	private static void SetNativeDeletionStage(long itemId, NativeItemDeletionStage? from, NativeItemDeletionStage? to)
	{
		using (FMDB.BeginIndependentScope(requireWrites: true))
		using (new FMDB())
		{
			var items = FMDB.Context.GameItems.Where(x => x.Id == itemId);
			if (items.Where(x => x.NativeDeletionStage == from)
				    .ExecuteUpdate(row => row.SetProperty(x => x.NativeDeletionStage, to)) == 1) return;
			if (to == NativeItemDeletionStage.ReadyToDelete)
			{
				var current = items.Select(x => new { x.NativeDeletionStage }).SingleOrDefault();
				if (current is null || current.NativeDeletionStage == to) return;
			}
			throw new InvalidOperationException($"Could not move item #{itemId} deletion from {from} to {to}.");
		}
	}

	private void CommitCheckpointedNativeDeletion()
	{
		using (new FMDB())
		{
			var caller = FMDB.Context;
			if (caller.Database.CurrentTransaction is not null || Gameworld.SaveManager.Flushing)
				throw new InvalidOperationException($"Item #{_id} deletion requires its teardown writes to finish first.");
			Gameworld.SaveManager.Flush();
			SetNativeDeletionStage(_id, NativeItemDeletionStage.TeardownInProgress, NativeItemDeletionStage.ReadyToDelete);
			DeleteReadyNativeItemRow(_id);
			MarkNativeItemRowDeleted();
			DetachDeletedItemRows(caller, new HashSet<long> { _id });
		}
	}

	private void MarkNativeItemRowDeleted()
	{
		_nativeRowDeleted = true;
		if (Gameworld is Futuremud world) world.ForgetPrimedDeletedItem(_id);
	}

	internal static void RecoverCheckpointedNativeDeletions(FuturemudDatabaseContext bootContext)
	{
		var pending = bootContext.GameItems.AsNoTracking()
			.Where(x => x.NativeDeletionStage != null)
			.OrderBy(x => x.Id)
			.Select(x => new { x.Id, x.NativeDeletionStage })
			.ToList();
		var inDoubt = pending.Where(x => x.NativeDeletionStage != NativeItemDeletionStage.ReadyToDelete)
			.Select(x => x.Id).ToArray();
		if (inDoubt.Length > 0)
		{
			throw new InvalidOperationException($"Native item deletion requires reconciliation before world load. " +
			                                    $"Teardown outcome is unknown for item IDs {string.Join(", ", inDoubt)}.");
		}

		foreach (var item in pending)
		{
			try
			{
				DeleteReadyNativeItemRow(item.Id);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Could not finish native deletion of item #{item.Id} during world load.", ex);
			}
		}
	}

	private static void DeleteReadyNativeItemRow(long itemId)
	{
		using (FMDB.BeginIndependentScope(requireWrites: true))
		using (new FMDB())
		{
			var context = FMDB.Context;
			using var transaction = context.Database.BeginTransaction();
			var row = context.GameItems.Find(itemId);
			if (row is null) return;
			if (row.NativeDeletionStage != NativeItemDeletionStage.ReadyToDelete)
			{
				throw new InvalidOperationException($"Item #{itemId} has no completed native deletion checkpoint.");
			}

			DisableHitchLinks(context, itemId);
			context.GameItems.Remove(row);
			context.SaveChanges();
			transaction.Commit();
		}
	}

	private static void DetachDeletedItemRows(FuturemudDatabaseContext caller, HashSet<long> ids)
	{
		foreach (var entry in caller.ChangeTracker.Entries().Where(entry =>
			         entry.Entity is Models.GameItem item && ids.Contains(item.Id) ||
			         entry.Metadata.GetForeignKeys().Any(key => key.PrincipalEntityType.ClrType == typeof(Models.GameItem) &&
				         key.Properties.Count == 1 && entry.Property(key.Properties[0].Name).CurrentValue is long id &&
				         ids.Contains(id))).ToArray())
		{
			entry.State = EntityState.Detached;
		}
	}

	internal IReadOnlyList<NativeDeletionNode>? BuildOrdinaryContainerDeletionPlan()
	{
		var plan = new List<NativeDeletionNode>();
		var visited = new HashSet<long>();
		return Visit(this, null, null) ? plan : null;

		bool Visit(GameItem item, GameItem? container, IWound? lodgedIn)
		{
			if (item._id == 0 || item.Deleted || item._nativeDeletionStarted || item._nativeRowDeleted ||
			    item._deletionObserverFailure is not null || item.HasDeletionObservers ||
			    item.HasSupersedingInstance || item.SpellCreationOrigin is not null ||
			    item.InAuctionEscrow() || item._overridingWoundBehaviourComponent is not null || item.Effects.Any() || item.Hooks.Any() ||
			    !visited.Add(item._id))
			{
				return false;
			}

			foreach (var component in item.Components)
			{
				var type = component.GetType();
				IEnumerable<IGameItem> children;
				if (type == typeof(ContainerGameItemComponent))
				{
					var ordinary = (ContainerGameItemComponent)component;
					children = ordinary.Contents.Concat(ordinary.Locks.Select(x => x.Parent));
				}
				else if (type == typeof(LockingContainerGameItemComponent))
				{
					var locking = (LockingContainerGameItemComponent)component;
					children = locking.Contents.Concat(locking.Locks.Select(x => x.Parent));
				}
				else if (type == typeof(AmmoClipGameItemComponent))
				{
					children = ((AmmoClipGameItemComponent)component).Contents;
				}
				else if (type == typeof(PileGameItemComponent))
				{
					children = ((PileGameItemComponent)component).Contents;
				}
				else if (type == typeof(HoldableGameItemComponent) || type == typeof(StackableGameItemComponent) ||
				         type == typeof(AmmunitionGameItemComponent) || type == typeof(SimpleLockGameItemComponent))
				{
					continue;
				}
				else
				{
					return false;
				}

				foreach (var child in children)
				{
					if (!ReferenceEquals(child.ContainedIn, item)) continue;
					if (child is not GameItem native || native.DirectLocation is not null ||
					    ComponentItemTransfer.HasDirectBodyCustody(native) || native.GetItemType<IBeltable>()?.ConnectedTo is not null ||
					    !Visit(native, item, null))
					{
						return false;
					}
				}
			}

			var wounds = item.Wounds.ToArray();
			var woundIds = new HashSet<long>();
			foreach (var wound in wounds)
			{
				var type = wound.GetType();
				if (type != typeof(SimpleWound) && type != typeof(SimpleOrganicWound) && type != typeof(RobotWound) &&
				    type != typeof(HealingSimpleWound) && type != typeof(BoneFracture) ||
				    !ReferenceEquals(wound.Parent, item) || wound.Infection is not null ||
				    wound is not ILateInitialisingItem { IdHasBeenRegistered: true } || !woundIds.Add(wound.Id)) return false;
				if (wound.Lodged is not { } lodged) continue;
				if (lodged is not GameItem native || !ComponentItemTransfer.IsDetached(native) ||
				    ComponentItemTransfer.HasDirectBodyCustody(native) || !Visit(native, null, wound)) return false;
			}

			plan.Add(new NativeDeletionNode(item, container, lodgedIn, wounds));
			return true;
		}
	}

	private static void DisableHitchLinks(FuturemudDatabaseContext context, long itemId)
	{
		foreach (var link in context.VehicleTowLinks.Where(x => x.HitchItemId == itemId && !x.IsDisabled).ToList())
		{
			link.IsDisabled = true;
		}

		foreach (var link in context.VehicleHitchLinks.Where(x => x.HitchItemId == itemId && !x.IsDisabled).ToList())
		{
			link.IsDisabled = true;
		}
	}
}
