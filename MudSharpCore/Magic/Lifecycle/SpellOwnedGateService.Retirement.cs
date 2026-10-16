#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedGateService
{
	private sealed record GateTopology(long South, long North, long SouthOverlay, long NorthOverlay,
		long SourcePortal, long Corridor, long DestinationPortal);

	public bool CanEnter(IRoom room, ICharacter entrant, bool nativeLogin)
	{
		if (!Rooms.TryGetValue(room.Id, out var id)) return true;
		try
		{
			var life = _store.Find(id);
			if (life is null) return false;
			if (life.State == SpellLifecycleState.Active && life.Origin.DeadlineUtc > RuntimeClock.UtcNow)
				return true;
			if (entrant.Location is { } previous && previous.Id != room.Id &&
				Rooms.TryGetValue(previous.Id, out var previousGate) && previousGate == id)
				return true;
			if (room.Characters.Any(x => ReferenceEquals(x, entrant))) return true;
			// A direct native MoveTo may have already changed physical custody before Enter runs.
			if (ReferenceEquals(entrant.Location, room) && !nativeLogin) return true;
			if (!nativeLogin || !ReferenceEquals(entrant.Location, room)) return false;
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			return entrant.InstanceId > 0 && FMDB.Context.CharacterInstances.AsNoTracking()
				.Any(x => x.Id == entrant.InstanceId && x.LocationId == room.Id) ||
				FMDB.Context.Characters.AsNoTracking().Any(x => x.Id == entrant.Id && x.Location == room.Id);
		}
		catch { return false; }
	}

	public bool RequestRetirement(long roomId, SpellRetirementReason reason)
	{
		if (!Rooms.TryGetValue(roomId, out var id)) return false;
		var life = _store.Find(id)!;
		if (life.State == SpellLifecycleState.Active)
			life = _store.BeginRetirement(id, life.Version, reason, TransitionTime(life));
		try { Retire(life); }
		catch (Exception ex) { Hold(_store.Find(id)!, "Nilaz retirement held: " + ex.Message); }
		return true;
	}

	public int ReconcileRetirements(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000)
			throw new ArgumentException("Invalid Nilaz reconciliation bound or UTC time.");
		var candidates = Rooms.Where(x => x.Key > _cursor).OrderBy(x => x.Key)
			.Select(x => x.Value).Distinct().Take(limit).ToArray();
		if (candidates.Length == 0) { _cursor = 0; return 0; }
		foreach (var id in candidates)
		{
			_cursor = Rooms.Where(x => x.Value == id).Max(x => x.Key);
			var life = _store.Find(id);
			if (life is null || life.State == SpellLifecycleState.Completed)
			{
				foreach (var room in Rooms.Where(x => x.Value == id).Select(x => x.Key).ToArray()) Rooms.Remove(room);
				continue;
			}
			try
			{
				if (life.State == SpellLifecycleState.Active && life.Origin.DeadlineUtc <= nowUtc)
					life = _store.BeginRetirement(id, life.Version, SpellRetirementReason.Expiry,
						nowUtc < life.UpdatedUtc ? life.UpdatedUtc : nowUtc);
				if (life.State == SpellLifecycleState.Retiring) Retire(life);
			}
			catch (Exception ex) { Hold(_store.Find(id)!, "Nilaz retirement held: " + ex.Message); }
		}
		return candidates.Length;
	}

	private void Retire(SpellOwnedLifecycle life)
	{
		var anchor = SpellGateAnchor.Load(life.Origin.Provenance);
		if (life.Origin.Family != SpellGateAnchor.Family || !life.MayRemoveOwnedEntities ||
			life.State != SpellLifecycleState.Retiring ||
			life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Room) != 2 ||
			life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.RoomOverlay) != 2 ||
			life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Exit) != 3 || life.Entities.Count != 7)
			throw new InvalidOperationException("Nilaz retirement requires its exact seven claimed topology rows.");
		if (AllOwnedRowsAbsent(life)) { FinishCommittedRemoval(life); return; }
		var topology = ReadTopology(life, anchor);
		if (CloseExternalLinks(life, topology)) life = _store.Find(life.Origin.Id)!;
		life = PulseClosedRooms(life, topology, anchor);
		// Once closed, the two internal rooms and their corridor remain habitable and owned.
		if (!CanRemove(topology)) return;
		_world.SaveManager.Flush(); _world.LogManager.FlushLog();
		using (var writes = FMDB.BeginIndependentScope(requireWrites: true))
		using (var db = new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		{
			var context = FMDB.Context;
			var current = context.MagicSpellLifecycles.Single(x => x.Id == life.Origin.Id);
			if (current.Version != life.Version || current.State != (int)SpellLifecycleState.Retiring ||
				current.GateLinksClosedUtc is null)
				throw new InvalidOperationException("Nilaz retirement authority changed before cleanup.");
			RequireTopology(context, topology, anchor, externalClosed: true);
			if (!CanRemove(topology, context)) return;
			RequireNoForeignTopologyReferences(context, topology);
			foreach (var roomId in new[] { topology.South, topology.North })
			{
				var row = context.Rooms.Single(x => x.Id == roomId);
				if (row.EffectData != "<Effects />" || !string.IsNullOrEmpty(row.SurfaceLiquidData))
					throw new InvalidOperationException("Nilaz room state needs a custody-aware recovery before deletion.");
				foreach (var log in context.CharacterLogs.Where(x => x.RoomId == roomId)) log.RoomId = anchor.SourceRoomId;
				row.CurrentOverlayId = null;
			}
			context.RoomOverlaysExits.RemoveRange(context.RoomOverlaysExits.Where(x => x.ExitId == topology.Corridor));
			context.Exits.Remove(context.Exits.Single(x => x.Id == topology.Corridor));
			context.SaveChanges();
			context.RoomOverlays.RemoveRange(context.RoomOverlays.Where(x => x.Id == topology.SouthOverlay || x.Id == topology.NorthOverlay));
			context.Rooms.RemoveRange(context.Rooms.Where(x => x.Id == topology.South || x.Id == topology.North));
			context.SaveChanges(); transaction.Commit();
		}
		FinishCommittedRemoval(life);
	}

	private bool AllOwnedRowsAbsent(SpellOwnedLifecycle life)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		var roomIds = life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.Room).Select(x => x.Id).ToArray();
		var overlayIds = life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.RoomOverlay).Select(x => x.Id).ToArray();
		var exitIds = life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.Exit).Select(x => x.Id).ToArray();
		return !FMDB.Context.Rooms.Any(x => roomIds.Contains(x.Id)) &&
			!FMDB.Context.RoomOverlays.Any(x => overlayIds.Contains(x.Id)) &&
			!FMDB.Context.Exits.Any(x => exitIds.Contains(x.Id));
	}

	private void FinishCommittedRemoval(SpellOwnedLifecycle life)
	{
		var roomIds = life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.Room).Select(x => x.Id).ToArray();
		var exitIds = life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.Exit).Select(x => x.Id).ToArray();
		foreach (var roomId in roomIds) _world.ExitManager.ForgetCommittedTopology(roomId, exitIds);
		foreach (var roomId in roomIds)
		{
			(_world.Rooms.Get(roomId) as Room)?.FinishCommittedGateRemoval();
			Rooms.Remove(roomId);
		}
		_store.Complete(life.Origin.Id, life.Version, TransitionTime(life));
		foreach (var exitId in exitIds) _exits?.Remove(exitId);
	}

	private GateTopology ReadTopology(SpellOwnedLifecycle life, SpellGateAnchor anchor)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		var sourceId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Exit &&
			x.Role == SpellOwnedEntityRole.GateSourcePortal).Id;
		var corridorId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Exit &&
			x.Role == SpellOwnedEntityRole.GateCorridor).Id;
		var destinationId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Exit &&
			x.Role == SpellOwnedEntityRole.GateDestinationPortal).Id;
		var ids = new[] { sourceId, corridorId, destinationId };
		var exits = FMDB.Context.Exits.AsNoTracking().Where(x => ids.Contains(x.Id)).ToArray();
		// Each portal has a durable typed claim role, independent of database ID allocation order.
		var source = exits.SingleOrDefault(x => x.Id == ids[0]);
		var corridor = exits.SingleOrDefault(x => x.Id == ids[1]);
		var destination = exits.SingleOrDefault(x => x.Id == ids[2]);
		if (corridor is null) throw new InvalidOperationException("The owned Nilaz corridor is absent.");
		var south = corridor.RoomId1; var north = corridor.RoomId2;
		var overlays = FMDB.Context.RoomOverlays.AsNoTracking().Where(x => x.RoomId == south || x.RoomId == north).ToArray();
		var southOverlay = overlays.Single(x => x.RoomId == south).Id;
		var northOverlay = overlays.Single(x => x.RoomId == north).Id;
		var topology = new GateTopology(south, north, southOverlay, northOverlay, ids[0], corridor.Id, ids[2]);
		RequireTopology(FMDB.Context, topology, anchor, source is null && destination is null);
		return topology;
	}

	private bool CloseExternalLinks(SpellOwnedLifecycle life, GateTopology topology)
	{
		using var writes = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		var context = FMDB.Context;
		var row = context.MagicSpellLifecycles.Single(x => x.Id == life.Origin.Id);
		if (row.Version != life.Version || row.State != (int)SpellLifecycleState.Retiring)
			throw new InvalidOperationException("Nilaz closure authority changed.");
		if (row.GateLinksClosedUtc is null)
		{
			RequireTopology(context, topology, SpellGateAnchor.Load(life.Origin.Provenance), externalClosed: false);
			var ids = new[] { topology.SourcePortal, topology.DestinationPortal };
			context.RoomOverlaysExits.RemoveRange(context.RoomOverlaysExits.Where(x => ids.Contains(x.ExitId)));
			context.Exits.RemoveRange(context.Exits.Where(x => ids.Contains(x.Id)));
			row.GateLinksClosedUtc = TransitionTime(life);
			row.GateLastPulseUtc = row.GateLinksClosedUtc;
			row.UpdatedUtc = row.GateLinksClosedUtc.Value;
			row.Version++;
			context.SaveChanges();
		}
		transaction.Commit();
		_world.ExitManager.ForgetCommittedTopology(topology.South, [topology.SourcePortal, topology.DestinationPortal]);
		_world.ExitManager.ForgetCommittedTopology(topology.North, [topology.SourcePortal, topology.DestinationPortal]);
		return row.Version != life.Version;
	}

	private bool CanRemove(GateTopology topology, FuturemudDatabaseContext? context = null)
	{
		var ids = new[] { topology.South, topology.North };
		if (ids.Any(id => _world.Rooms.Get(id) is not Room room || room.Characters.Any() || room.GameItems.Any(x => !x.Deleted)) ||
			_world.Actors.Concat(_world.CachedActors).Any(x => x.Location is { } place && ids.Contains(place.Id))) return false;
		if (context is not null) return PersistedEmpty(context, ids);
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		return PersistedEmpty(FMDB.Context, ids);
	}

	private static bool PersistedEmpty(FuturemudDatabaseContext context, long[] ids) =>
		!context.CharacterInstances.Any(x => x.LocationId.HasValue && ids.Contains(x.LocationId.Value)) &&
		!context.Characters.Any(x => ids.Contains(x.Location)) &&
		!context.RoomsGameItems.Any(x => ids.Contains(x.RoomId)) &&
		!context.Characters.Any(x => x.PositionTargetId.HasValue && ids.Contains(x.PositionTargetId.Value) &&
			(x.PositionTargetType == PersistedFrameworkItemReference.RoomType ||
			 x.PositionTargetType == PersistedFrameworkItemReference.LegacyCellType)) &&
		!context.CharacterInstances.Any(x => x.PositionTargetId.HasValue && ids.Contains(x.PositionTargetId.Value) &&
			(x.PositionTargetType == PersistedFrameworkItemReference.RoomType ||
			 x.PositionTargetType == PersistedFrameworkItemReference.LegacyCellType)) &&
		!context.GameItems.Any(x => x.PositionTargetId.HasValue && ids.Contains(x.PositionTargetId.Value) &&
			(x.PositionTargetType == PersistedFrameworkItemReference.RoomType ||
			 x.PositionTargetType == PersistedFrameworkItemReference.LegacyCellType));

	private void RequireNoForeignTopologyReferences(FuturemudDatabaseContext context, GateTopology t)
	{
		var effects = new SpellOwnedShelterService(_world);
		foreach (var id in new[] { t.South, t.North })
			effects.RequireNoForeignRoomEffectReferences(context,
				_world.Rooms.Get(id) ?? throw new InvalidOperationException("The retained Nilaz room is unavailable."));
		foreach (var entity in context.Model.GetEntityTypes())
		foreach (var foreignKey in entity.GetForeignKeys())
		{
			var principal = foreignKey.PrincipalEntityType.ClrType;
			var targets = principal == typeof(Models.Room) ? new[] { t.South, t.North } :
				principal == typeof(Models.RoomOverlay) ? new[] { t.SouthOverlay, t.NorthOverlay } :
				principal == typeof(Models.Exit) ? new[] { t.Corridor } : [];
			if (targets.Length == 0) continue;
			var dependent = entity.ClrType;
			if (dependent == typeof(Models.RoomOverlayExit) || dependent == typeof(Models.Exit) ||
				dependent == typeof(Models.RoomOverlay) || dependent == typeof(Models.Room) ||
				dependent == typeof(Models.Character) || dependent == typeof(Models.CharacterInstance) ||
				dependent == typeof(Models.RoomsGameItems) || dependent == typeof(Models.CharacterLog)) continue;
			if (foreignKey.Properties.Count != 1 || foreignKey.PrincipalKey.Properties.Single().Name != "Id")
				throw new InvalidOperationException($"Unsupported declared Nilaz topology reference: {dependent.Name}.");
			foreach (var target in targets)
				if (SpellOwnedShelterService.HasDeclaredReference(context, dependent,
					foreignKey.Properties[0].Name, foreignKey.Properties[0].ClrType, target))
					throw new InvalidOperationException($"Foreign {dependent.Name}.{foreignKey.Properties[0].Name} retains Nilaz topology.");
		}
	}

	private static void RequireTopology(FuturemudDatabaseContext context, GateTopology t, SpellGateAnchor a, bool externalClosed)
	{
		var rooms = new[] { t.South, t.North }; var overlays = new[] { t.SouthOverlay, t.NorthOverlay };
		if (rooms.Distinct().Count() != 2 || overlays.Distinct().Count() != 2 ||
			context.Rooms.Count(x => rooms.Contains(x.Id) && x.Temporary &&
				(x.Id == t.South ? x.CurrentOverlayId == t.SouthOverlay : x.CurrentOverlayId == t.NorthOverlay)) != 2 ||
			context.RoomOverlays.Count(x => overlays.Contains(x.Id) &&
				(x.Id == t.SouthOverlay ? x.RoomId == t.South : x.RoomId == t.North)) != 2 ||
			context.RoomOverlays.Any(x => rooms.Contains(x.RoomId) && !overlays.Contains(x.Id)) ||
			context.Rooms.Any(x => !rooms.Contains(x.Id) && x.CurrentOverlayId.HasValue && overlays.Contains(x.CurrentOverlayId.Value)) ||
			!context.Exits.Any(x => x.Id == t.Corridor && x.RoomId1 == t.South && x.RoomId2 == t.North &&
				x.DoorId == null && x.FallRoom == null && !x.IsClimbExit) ||
			context.Exits.Any(x => x.FallRoom.HasValue && rooms.Contains(x.FallRoom.Value)) ||
			context.Exits.Any(x => (rooms.Contains(x.RoomId1) || rooms.Contains(x.RoomId2)) &&
				x.Id != t.Corridor && x.Id != t.SourcePortal && x.Id != t.DestinationPortal))
			throw new InvalidOperationException("Nilaz topology has changed; preserve the owned rooms.");
		if (!externalClosed && (!context.Exits.Any(x => x.Id == t.SourcePortal && x.RoomId1 == a.SourceRoomId &&
			x.RoomId2 == t.South && x.DoorId == null && x.FallRoom == null && !x.IsClimbExit) ||
			!context.Exits.Any(x => x.Id == t.DestinationPortal && x.RoomId1 == t.North &&
				x.RoomId2 == a.DestinationRoomId && x.DoorId == null && x.FallRoom == null && !x.IsClimbExit)))
			throw new InvalidOperationException("Nilaz external portals have changed.");
		if (externalClosed && context.Exits.Any(x => x.Id == t.SourcePortal || x.Id == t.DestinationPortal))
			throw new InvalidOperationException("Closed Nilaz external portals are still present.");
		if (context.RoomOverlaysExits.Any(x => overlays.Contains(x.RoomOverlayId) &&
			x.ExitId != t.Corridor && (externalClosed || x.ExitId != t.SourcePortal && x.ExitId != t.DestinationPortal)))
			throw new InvalidOperationException("Foreign exit associations reference the Nilaz overlays.");
		if (context.RoomOverlaysExits.Any(x => x.ExitId == t.Corridor && !overlays.Contains(x.RoomOverlayId)) ||
			!context.RoomOverlaysExits.Any(x => x.ExitId == t.Corridor && x.RoomOverlayId == t.SouthOverlay) ||
			!context.RoomOverlaysExits.Any(x => x.ExitId == t.Corridor && x.RoomOverlayId == t.NorthOverlay))
			throw new InvalidOperationException("Nilaz corridor associations changed.");
		if (!externalClosed && (context.RoomOverlaysExits.Any(x => x.ExitId == t.SourcePortal &&
			x.RoomOverlayId != a.SourceOverlayId && x.RoomOverlayId != t.SouthOverlay) ||
			context.RoomOverlaysExits.Any(x => x.ExitId == t.DestinationPortal &&
				x.RoomOverlayId != a.DestinationOverlayId && x.RoomOverlayId != t.NorthOverlay) ||
			!context.RoomOverlaysExits.Any(x => x.ExitId == t.SourcePortal && x.RoomOverlayId == a.SourceOverlayId) ||
			!context.RoomOverlaysExits.Any(x => x.ExitId == t.SourcePortal && x.RoomOverlayId == t.SouthOverlay) ||
			!context.RoomOverlaysExits.Any(x => x.ExitId == t.DestinationPortal && x.RoomOverlayId == a.DestinationOverlayId) ||
			!context.RoomOverlaysExits.Any(x => x.ExitId == t.DestinationPortal && x.RoomOverlayId == t.NorthOverlay)))
			throw new InvalidOperationException("Nilaz external portal associations changed.");
	}

	private void Hold(SpellOwnedLifecycle life, string message)
	{
		if (life.State != SpellLifecycleState.Completed)
			_store.Hold(life.Origin.Id, life.Version, message[..Math.Min(message.Length, 2048)], TransitionTime(life));
	}

	private static DateTime TransitionTime(SpellOwnedLifecycle life) =>
		RuntimeClock.UtcNow < life.UpdatedUtc ? life.UpdatedUtc : RuntimeClock.UtcNow;
}
