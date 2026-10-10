#nullable enable

using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.GameItems;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedShelterService
{
	private void Retire(SpellOwnedLifecycle life)
	{
		var metadata = SpellShelterAnchor.Load(life.Origin.Provenance);
		var roomId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Room).Id;
		var overlayId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.RoomOverlay).Id;
		var exitId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Exit).Id;
		if (life.Origin.Family != SpellShelterAnchor.Family || !life.MayRemoveOwnedEntities ||
			life.State != SpellLifecycleState.Retiring || life.Entities.Any(x =>
				x.Kind is not (SpellOwnedEntityKind.Room or SpellOwnedEntityKind.RoomOverlay or SpellOwnedEntityKind.Exit or SpellOwnedEntityKind.GameItem)) ||
			life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.GameItem) != (metadata.Kind == SpellShelterKind.SpringHaven ? 1 : 0))
			throw new InvalidOperationException("Shelter retirement needs its exact owned native topology and optional finite supply.");
		var room = _world.Rooms.Get(roomId) as Room;
		bool exists;
		using (var isolated = FMDB.BeginIndependentScope())
		using (var db = new FMDB()) exists = FMDB.Context.Rooms.Any(x => x.Id == roomId);
		if (exists)
		{
			if (!TryDestination(metadata.AnchorRoomId, metadata.AnchorRoutePosition, out var destination) &&
				!TryDestination(metadata.FallbackRoomId, null, out destination))
				throw new InvalidOperationException("Neither recorded anchor nor configured fallback is safe; retain the occupied shelter for recovery.");
			if (room is null || !room.Temporary) throw new InvalidOperationException("The claimed shelter room is missing from runtime or no longer temporary.");
			if (room.Hooks.Any() || room.Effects.Any())
				throw new InvalidOperationException("Room hooks or effects need cleanup before topology removal.");
			// Check before any movement. A builder/vehicle/economy link cannot silently become cast ownership.
			using (var isolated = FMDB.BeginIndependentScope())
			using (var db = new FMDB())
			{
				RequireTopologyAuthority(FMDB.Context, roomId, overlayId, exitId, metadata);
				RequireNoForeignRoomEffectReferences(FMDB.Context, room);
				foreach (var supply in life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.GameItem))
					RequireSupplyPersistence(FMDB.Context, supply.Id, roomId);
			}
			RemoveRoomWorkPermits(room);
			var itemId = life.Entities.SingleOrDefault(x => x.Kind == SpellOwnedEntityKind.GameItem)?.Id;
			var actors = _world.Actors.Concat(_world.CachedActors)
				.SelectMany(x => x.Identity.Instances.OfType<ICharacter>())
				.Concat(room.Characters).Distinct(ReferenceEqualityComparer.Instance)
				.OfType<ICharacter>().Where(x => x.Location?.Id == roomId).ToArray();
			foreach (var actor in actors)
			{
				actor.RidingMount?.RemoveRider(actor);
				foreach (var rider in actor.Riders.ToArray()) actor.RemoveRider(rider);
				Evacuate(actor, room, destination);
			}
			foreach (var item in room.GameItems.Where(x => x.Id != itemId).ToArray())
			{
				EvacuateForeignItem(item, room, destination);
			}
			// Native saves finish custody and primary/secondary actor state before deletion is permitted.
			// A crash here leaves the durable retirement intent and a replayable remaining room, never an absent destination.
			_world.SaveManager.Flush(); _world.LogManager.FlushLog();
			using var writes = FMDB.BeginIndependentScope(requireWrites: true); using var database = new FMDB();
			using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
			var context = FMDB.Context;
			var current = context.MagicSpellLifecycles.Single(x => x.Id == life.Origin.Id);
			if (current.Version != life.Version || current.State != (int)SpellLifecycleState.Retiring)
				throw new InvalidOperationException("Shelter retirement authority changed before commit.");
			RequireTopologyAuthority(context, roomId, overlayId, exitId, metadata);
			RequireNoForeignRoomEffectReferences(context, room);
			var point = destination.RoutePositionMetres is { } position ? (decimal?)position : null;
			foreach (var actor in context.Characters.Where(x => x.Location == roomId))
			{
				actor.Location = destination.Room.Id; actor.RoomLayer = (int)destination.Layer; actor.RoutePosition = point;
			}
			foreach (var instance in context.CharacterInstances.Where(x => x.LocationId == roomId))
			{
				instance.LocationId = destination.Room.Id; instance.RoomLayer = (int)destination.Layer; instance.RoutePosition = point;
			}
			RemovePersistedRoomWorkPermits(context, roomId);
			// This non-null native FK cascades. Preserve the original audit rows and their payloads.
			foreach (var log in context.CharacterLogs.Where(x => x.RoomId == roomId)) log.RoomId = destination.Room.Id;
			// These are discriminated native position links, including unloaded visitors. Legacy "Room" is a different identity space.
			foreach (var actor in context.Characters.Where(x => x.PositionTargetId == roomId &&
				(x.PositionTargetType == PersistedFrameworkItemReference.RoomType || x.PositionTargetType == PersistedFrameworkItemReference.LegacyCellType) ||
				itemId != null && x.PositionTargetId == itemId && x.PositionTargetType == "GameItem"))
			{ actor.PositionTargetId = null; actor.PositionTargetType = string.Empty; }
			foreach (var instance in context.CharacterInstances.Where(x => x.PositionTargetId == roomId &&
				(x.PositionTargetType == PersistedFrameworkItemReference.RoomType || x.PositionTargetType == PersistedFrameworkItemReference.LegacyCellType) ||
				itemId != null && x.PositionTargetId == itemId && x.PositionTargetType == "GameItem"))
			{ instance.PositionTargetId = null; instance.PositionTargetType = string.Empty; }
			foreach (var item in context.GameItems.Where(x => x.PositionTargetId == roomId &&
				(x.PositionTargetType == PersistedFrameworkItemReference.RoomType || x.PositionTargetType == PersistedFrameworkItemReference.LegacyCellType) ||
				itemId != null && x.PositionTargetId == itemId && x.PositionTargetType == "GameItem"))
			{ item.PositionTargetId = null; item.PositionTargetType = string.Empty; }
			if (context.RoomsGameItems.Any(x => x.RoomId == roomId && x.GameItemId != itemId))
				throw new InvalidOperationException("Unloaded foreign goods remain in the room; preserve it for a custody-aware retry.");
			var water = RetireWater(context, life, destination);
			var surfaceSupply = itemId is { } supplyId ? (_world.Items.Get(supplyId) as GameItem)?.SurfaceLiquidState : null;
			var applySurfaceTransfer = room.StageShelterSurfaceTransfer(context, destination,
				surfaceSupply is null ? [] : [surfaceSupply]);
			context.RoomOverlaysExits.RemoveRange(context.RoomOverlaysExits.Where(x => x.ExitId == exitId));
			var exit = context.Exits.Find(exitId); if (exit is not null) context.Exits.Remove(exit);
			var row = context.Rooms.Find(roomId)!;
			row.CurrentOverlayId = null;
			context.SaveChanges();
			var overlay = context.RoomOverlays.Find(overlayId); if (overlay is not null) context.RoomOverlays.Remove(overlay);
			context.Rooms.Remove(row);
			context.SaveChanges(); transaction.Commit();
			applySurfaceTransfer();
			if (water is not null) _pendingWater[life.Origin.Id] = water;
			water?.Activate(_world, destination);
			_pendingWater.Remove(life.Origin.Id);
		}
		// A previous attempt may have committed and then failed runtime release. These operations are idempotent.
		if (_pendingWater.TryGetValue(life.Origin.Id, out var pending))
		{
			var target = _world.Rooms.Get(pending.Row.RoomsGameItems.Single().RoomId) ??
				throw new InvalidOperationException("The committed conserved-water destination disappeared; retain its exact pending activation.");
			pending.Activate(_world, new SpatialLocation(target, (RoomLayer)pending.Row.RoomLayer,
				pending.Row.RoutePosition is { } coordinate ? (double)coordinate : null));
			_pendingWater.Remove(life.Origin.Id);
		}
		foreach (var claim in life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.GameItem))
		{
			var cached = _world.Items.Get(claim.Id) as GameItem;
			cached?.FinishCommittedSpellItemRemoval();
		}
		_world.ExitManager.ForgetCommittedTopology(roomId, [exitId]);
		room?.FinishCommittedShelterRemoval();
		_store.Complete(life.Origin.Id, life.Version, TransitionTime(life));
		Rooms.Remove(roomId);
		_exits?.Remove(exitId);
	}

	internal static void EvacuateForeignItem(IGameItem item, IRoom source, SpatialLocation destination)
	{
		var restore = ((ICustodyRollbackLocation)source).CaptureCustodyMembershipRollback([item]);
		try
		{
			source.Extract(item);
			if (item.Deleted || item.InInventoryOf is not null || item.ContainedIn is not null || !ReferenceEquals(item.Location, source))
				throw new InvalidOperationException("An extraction callback changed foreign custody; retain topology without overwriting that custody.");
			item.MoveTo(destination);
			if (!ReferenceEquals(item.Location, destination.Room) || item.Deleted || item.InInventoryOf is not null || item.ContainedIn is not null)
				throw new InvalidOperationException("A movement callback changed foreign custody; preserve it.");
			destination.Room.Insert(item, true);
			if (!ReferenceEquals(item.Location, destination.Room) || !destination.Room.GameItems.Any(x => ReferenceEquals(x, item)) ||
				item.InInventoryOf is not null || item.ContainedIn is not null)
				throw new InvalidOperationException("A custody callback failed to preserve a visitor's item; retain the topology.");
		}
		catch
		{
			if (!item.Deleted && item.InInventoryOf is null && item.ContainedIn is null && ReferenceEquals(item.Location, source)) restore();
			throw;
		}
		finally
		{
			if (source is Room origin) origin.ReconcileShelterItemMembership(item);
			if (destination.Room is Room target) target.ReconcileShelterItemMembership(item);
			if (item.Location is Room current && !ReferenceEquals(current, source) && !ReferenceEquals(current, destination.Room))
				current.ReconcileShelterItemMembership(item);
		}
	}

	private static void RequireTopologyAuthority(FuturemudDatabaseContext context, long roomId, long overlayId, long exitId, SpellShelterAnchor metadata)
	{
		if (metadata.AnchorOverlayId <= 0 ||
			context.Exits.Any(x => x.Id == exitId && (x.RoomId1 != metadata.AnchorRoomId || x.RoomId2 != roomId ||
				x.DoorId != null || x.FallRoom != null || x.IsClimbExit)) ||
			context.RoomOverlaysExits.Any(x => x.ExitId == exitId && x.RoomOverlayId != overlayId && x.RoomOverlayId != metadata.AnchorOverlayId) ||
			context.RoomOverlaysExits.Any(x => x.RoomOverlayId == overlayId && x.ExitId != exitId))
			throw new InvalidOperationException("Claimed entrance endpoints or overlay associations changed; retain foreign topology.");
		if (!context.Rooms.Any(x => x.Id == roomId && x.Temporary) ||
			context.RoomOverlays.Any(x => x.RoomId == roomId && x.Id != overlayId) ||
			!context.RoomOverlays.Any(x => x.Id == overlayId && x.RoomId == roomId) ||
			context.Rooms.Any(x => x.Id == roomId && x.CurrentOverlayId != overlayId) ||
			context.Exits.Any(x => (x.RoomId1 == roomId || x.RoomId2 == roomId) && x.Id != exitId))
			throw new InvalidOperationException("A foreign overlay/exit or changed claimed topology requires explicit recovery.");
		// Inspect declared EF foreign keys, rather than field-name guesses or arbitrary definition text.
		foreach (var entity in context.Model.GetEntityTypes())
		foreach (var foreignKey in entity.GetForeignKeys())
		{
			var principal = foreignKey.PrincipalEntityType.ClrType;
			long target;
			if (principal == typeof(Models.Room)) target = roomId;
			else if (principal == typeof(Models.Exit)) target = exitId;
			else if (principal == typeof(Models.RoomOverlay)) target = overlayId;
			else continue;
			var dependent = entity.ClrType;
			if (dependent == typeof(Models.Character) || dependent == typeof(Models.CharacterInstance) ||
				dependent == typeof(Models.RoomsGameItems) || dependent == typeof(Models.RoomOverlayExit) ||
				dependent == typeof(Models.Track) || dependent == typeof(Models.CharacterLog)) continue;
			if (dependent == typeof(Models.RoomOverlay) && principal == typeof(Models.Room)) continue;
			if (dependent == typeof(Models.Room) && principal == typeof(Models.RoomOverlay))
			{
				if (context.Rooms.Any(x => x.CurrentOverlayId == overlayId && x.Id != roomId))
					throw new InvalidOperationException("Another room references the claimed overlay.");
				continue;
			}
			if (foreignKey.Properties.Count != 1 || foreignKey.PrincipalKey.Properties.Single().Name != "Id")
				throw new InvalidOperationException($"Unsupported declared topology foreign key: {dependent.Name}.");
			if (HasDeclaredReference(context, dependent, foreignKey.Properties[0].Name, foreignKey.Properties[0].ClrType, target))
				throw new InvalidOperationException($"Foreign {dependent.Name}.{foreignKey.Properties[0].Name} references the shelter topology.");
		}
	}

	internal void Evacuate(ICharacter actor, IRoom source, SpatialLocation destination)
	{
		var present = source.Characters.Any(x => ReferenceEquals(x, actor));
		var active = present || _world.Actors.Any(x => ReferenceEquals(x, actor));
		try
		{
			if (active) { actor.Combat?.EndCombat(true); actor.Movement?.Cancel(); actor.Movement = null!; }
			if (!ReferenceEquals(actor.Location, source)) throw new InvalidOperationException("A cancellation callback changed the occupant's room.");
			if (present) source.Leave(actor);
			if (!ReferenceEquals(actor.Location, source)) throw new InvalidOperationException("A leave callback changed the occupant's room; preserve that location.");
			actor.MoveTo(destination);
			if (!ReferenceEquals(actor.Location, destination.Room)) throw new InvalidOperationException("An occupant movement callback changed the destination.");
			actor.Changed = true;
			if (active) destination.Room.Enter(actor, roomLayer: destination.Layer);
			if (!ReferenceEquals(actor.Location, destination.Room) || active && !destination.Room.Characters.Any(x => ReferenceEquals(x, actor)))
				throw new InvalidOperationException("An entry callback changed the occupant's room or membership.");
		}
		finally
		{
			var current = actor.Location as Room;
			var live = _world.Actors.Any(x => ReferenceEquals(x, actor));
			if (source is Room nativeSource) nativeSource.ReconcileNativeCharacterMembership(actor, live && ReferenceEquals(current, source));
			if (destination.Room is Room target) target.ReconcileNativeCharacterMembership(actor, live && ReferenceEquals(current, target));
			if (current is not null && !ReferenceEquals(current, source) && !ReferenceEquals(current, destination.Room))
				current.ReconcileNativeCharacterMembership(actor, live);
		}
	}

	internal static bool HasDeclaredReference(DbContext context, Type entityType, string propertyName, Type propertyType, long target)
	{
		var set = typeof(DbContext).GetMethods().Single(x => x.Name == nameof(DbContext.Set) && x.IsGenericMethod && x.GetParameters().Length == 0)
			.MakeGenericMethod(entityType).Invoke(context, null) as IQueryable;
		var parameter = Expression.Parameter(entityType, "row");
		var property = Expression.Call(typeof(EF), nameof(EF.Property), [propertyType], parameter, Expression.Constant(propertyName));
		var value = Expression.Convert(Expression.Constant(target), propertyType);
		var predicate = Expression.Lambda(Expression.Equal(property, value), parameter);
		var any = Expression.Call(typeof(Queryable), nameof(Queryable.Any), [entityType], set!.Expression, Expression.Quote(predicate));
		return set.Provider.Execute<bool>(any);
	}
}
