#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using RuntimeNpc = MudSharp.NPC.NPC;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedNpcService
{
	private const string RemovalPending = "Native remains removal pending: ";
	// A failed transaction retains its pre-transfer roots for a same-process retry. On a
	// restart the uncommitted inventory joins load those roots through the ordinary body loader.
	private readonly Dictionary<long, ForeignCustodySnapshot> _evacuationRetries = new();
	private long _lastInspectedRetirementNpcId;
	public bool HasPendingRetirement(ICharacter character, long creatorId)
	{
		var life = FindNpc(character.Id);
		return life is { State: SpellLifecycleState.Retiring or SpellLifecycleState.RemainsPending } &&
			life.Origin.CreatorId == creatorId && life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter) == 1 &&
			life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Body) == 1 &&
			life.Entities.Any(x => x.Kind == SpellOwnedEntityKind.Body && x.Id == character.Body.Id);
	}

	public bool SuppressNativeRemains(ICharacter character)
	{
		var life = FindNpc(character.Id);
		return life is { Origin.Mode: SpellLifecycleMode.TemporaryCleanup } &&
			life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Body && x.Id == character.Body.Id) == 1;
	}

	public int ReconcileRetirements(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000)
			throw new ArgumentException("A bounded UTC retirement pass is required.");
		// Even an already-durable retirement intent must not enter native callbacks in a
		// read-only caller. Establish writable authority before loading or changing runtime state.
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		ReconcilePersistedDeaths(nowUtc, limit);
		(SpellOwnedLifecycle Life, long ActorId)[] ReadPending()
		{
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			return (from claim in FMDB.Context.MagicSpellOwnedEntities.AsNoTracking()
				join life in FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities) on claim.LifecycleId equals life.Id
				where claim.Kind == (int)SpellOwnedEntityKind.AutonomousCharacter && claim.EntityId > _lastInspectedRetirementNpcId &&
					(life.State == (int)SpellLifecycleState.Retiring || life.State == (int)SpellLifecycleState.RemainsPending ||
					 life.State == (int)SpellLifecycleState.Active && life.DeadlineUtc <= nowUtc)
				orderby claim.EntityId
				select new { Life = life, ActorId = claim.EntityId }).Take(limit).ToArray()
				.Select(x => (SpellOwnedLifecycleStore.Read(x.Life), x.ActorId)).ToArray();
		}
		var pending = ReadPending();
		if (pending.Length == 0 && _lastInspectedRetirementNpcId != 0) { _lastInspectedRetirementNpcId = 0; pending = ReadPending(); }
		foreach (var entry in pending)
		{
			_lastInspectedRetirementNpcId = entry.ActorId;
			var initial = entry.Life;
			var life = initial;
			try
			{
				if (life.Diagnostic.StartsWith(ActivationPendingDiagnostic, StringComparison.Ordinal)) continue;
				if (!HasSimpleRetirementClaims(life)) throw new InvalidOperationException("Retirement holds unsupported or ambiguous native ownership claims.");
				var actorId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter).Id;
				var bodyId = life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Body).Id;
				if (world.CharacterArchives?.Find(actorId) is { } archive)
				{
					if (archive.OriginalBodyId != bodyId || archive.LifecycleId != life.Origin.Id)
						throw new InvalidOperationException("The archive does not match this owned physical body.");
					if (world.Actors.Concat(world.CachedActors).Concat(world.NPCs).FirstOrDefault(x => x.Id == actorId) is RuntimeNpc cached &&
						!world.CharacterArchives.TryArchiveNpc(life.Origin.Id, life.Version, cached, TransitionTime(life, nowUtc), out var retryError))
						throw new InvalidOperationException(retryError);
					_store.Complete(life.Origin.Id, life.Version, TransitionTime(life, nowUtc));
					_evacuationRetries.Remove(bodyId);
					continue;
				}
				if (world.TryGetCharacter(actorId, true) is not RuntimeNpc npc || !ReferenceEquals(npc.Gameworld, world) ||
					npc.IsPlayerCharacter || npc.Body.Id != bodyId)
					throw new InvalidOperationException("The exact native NPC/body cannot be loaded safely; retain its graph.");
				if (life.State == SpellLifecycleState.Active)
					life = _store.BeginRetirement(life.Origin.Id, life.Version, SpellRetirementReason.Expiry, TransitionTime(life, nowUtc));
				// This exact creation's outgoing bond must also release on persisted-dead reloads.
				// External effects and relationships retain their ordinary custody guards.
				foreach (var bond in npc.EffectsOfType<SpellNpcGuardian>().ToArray()) bond.PrepareRetirement(life.Origin.CreatorId);
				if (!npc.State.HasFlag(CharacterState.Dead))
				{
					if (CharacterArchiveService.HasRuntimeDependants(npc) || world.NPCs.OfType<RuntimeNpc>().Any(x => x.BodyguardingCharacterID == npc.Id))
						throw new InvalidOperationException("Connected controllers, runtime relationships or dependent instances must release before native death or evacuation.");
					if (life.Origin.Mode == SpellLifecycleMode.TemporaryCleanup)
						EvacuateBody(npc.Body, life, RouteSpatialService.Instance.GetEffectiveLocation(npc));
					// Intent is durable before native death. Native Die itself refuses a second death and
					// the post-persistence observer records its one optional remains item.
					npc.Die();
					life = _store.Find(life.Origin.Id)!;
				}
				if (!npc.State.HasFlag(CharacterState.Dead) || life.DeathObservedUtc is null)
					throw new InvalidOperationException("Native death is not durably correlated; terminal work must wait.");
				if ((life.RemainsRemovalRequestedUtc is not null || life.Diagnostic.StartsWith(RemovalPending, StringComparison.Ordinal)) && life.RemainsItemId is { } retryRemains)
					world.TryGetItem(retryRemains, true)?.Delete();
				life = _store.Find(life.Origin.Id)!;
				using (var isolated = FMDB.BeginIndependentScope())
				using (var db = new FMDB())
					if (life.RemainsItemId is { } remains && FMDB.Context.GameItems.Any(x => x.Id == remains))
						throw new InvalidOperationException(life.Diagnostic.StartsWith(RemovalPending, StringComparison.Ordinal)
							? life.Diagnostic : "Native remains retain the exact body until ordinary decay or removal releases them.");
				EvacuateBody(npc.Body, life, RouteSpatialService.Instance.GetEffectiveLocation(npc));
				if (CharacterArchiveService.HasRuntimeDependants(npc) || npc.Effects.Any() || !RetirementBodyEffects.TryCapture(npc.Body, out _) ||
					world.NPCs.OfType<RuntimeNpc>().Any(x => x.BodyguardingCharacterID == npc.Id))
					throw new InvalidOperationException("Runtime relationships or unadapted effects still retain this native NPC.");
				npc.Quit(silent: true);
				life = _store.Find(life.Origin.Id)!;
				var archives = world.CharacterArchives ?? throw new InvalidOperationException("Native archival is unavailable.");
				if (!archives.TryArchiveNpc(life.Origin.Id, life.Version, npc, TransitionTime(life, nowUtc), out var diagnostic))
					throw new InvalidOperationException(diagnostic);
				life = _store.Find(life.Origin.Id)!;
				_store.Complete(life.Origin.Id, life.Version, TransitionTime(life, nowUtc));
				_evacuationRetries.Remove(bodyId);
			}
			catch (Exception ex)
			{
				life = _store.Find(initial.Origin.Id) ?? life;
				RecordRetirementHold(life, ex.Message.StartsWith(RemovalPending, StringComparison.Ordinal)
					? ex.Message : "Native retirement held: " + ex.Message, nowUtc);
			}
		}
		return pending.Length;
	}

	public bool TryPrepareRemainsRemoval(IGameItem remains, out string diagnostic, bool morphing = false)
	{
		diagnostic = string.Empty;
		if (remains.GetItemType<ICorpse>() is not { OriginalBodyId: > 0 } corpse) return true;
		SpellOwnedLifecycle? life;
		using (var isolated = FMDB.BeginIndependentScope())
		using (var db = new FMDB())
			life = FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
				.SingleOrDefault(x => x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.Body && e.EntityId == corpse.OriginalBodyId))
				is { } row ? SpellOwnedLifecycleStore.Read(row) : null;
		if (life is null || !life.MayRemoveOwnedEntities) return true;
		try
		{
			if (morphing && remains.Prototype.MorphTargetId is not null)
				throw new InvalidOperationException("Native remains morph held: replacement items require a verified transfer adapter before morph side effects.");
			if (remains is ILateInitialisingItem { IdHasBeenRegistered: false } ||
				!HasSimpleRetirementClaims(life) || life.DeathObservedUtc is null || life.RemainsItemId != remains.Id ||
				corpse.OriginalBody is not { } body || body.Id != corpse.OriginalBodyId ||
				!body.Actor.State.HasFlag(CharacterState.Dead))
				throw new InvalidOperationException("Owned remains require exact persisted native death/body correlation before removal.");
			EvacuateBody(body, life, RouteSpatialService.Instance.GetEffectiveLocation(remains.LocationLevelPerceivable));
			_store.RequestRemainsRemoval(life.Origin.Id, life.Version, remains.Id, TransitionTime(life, RuntimeClock.UtcNow));
			return true;
		}
		catch (Exception ex)
		{
			diagnostic = ex.Message.StartsWith("Native remains morph held:", StringComparison.Ordinal) ? ex.Message : RemovalPending + ex.Message;
			RecordRetirementHold(_store.Find(life.Origin.Id) ?? life, diagnostic, RuntimeClock.UtcNow);
			return false;
		}
	}

	public bool TryNotifyRemainsDeletion(IGameItem remains, Action notify)
	{
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		SpellOwnedLifecycle? life;
		using (var db = new FMDB())
			life = FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
				.SingleOrDefault(x => x.RemainsItemId == remains.Id) is { } row ? SpellOwnedLifecycleStore.Read(row) : null;
		if (life is null || !life.MayRemoveOwnedEntities) { notify(); return true; }
		if (life.RemainsNotificationCompletedUtc is not null) return true;
		if (life.RemainsRemovalRequestedUtc is null || life.RemainsNotificationAttemptedUtc is not null)
		{
			RecordRetirementHold(life, RemovalPending + "Deletion observers have an incomplete durable attempt; retain remains for review.", RuntimeClock.UtcNow);
			return false;
		}
		life = _store.AttemptRemainsNotification(life.Origin.Id, life.Version, TransitionTime(life, RuntimeClock.UtcNow));
		try
		{
			notify();
			_store.CompleteRemainsNotification(life.Origin.Id, life.Version, TransitionTime(life, RuntimeClock.UtcNow));
			return true;
		}
		catch
		{
			RecordRetirementHold(_store.Find(life.Origin.Id)!, RemovalPending + "Deletion observer attempt did not complete; retain remains without replay.", RuntimeClock.UtcNow);
			throw;
		}
	}

	private static bool HasSimpleRetirementClaims(SpellOwnedLifecycle life) => life.MayRemoveOwnedEntities &&
		life.Entities.Count == 2 && life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.AutonomousCharacter) == 1 &&
		life.Entities.Count(x => x.Kind == SpellOwnedEntityKind.Body) == 1;

	private void RecordRetirementHold(SpellOwnedLifecycle life, string diagnostic, DateTime nowUtc)
	{
		var current = _store.Find(life.Origin.Id)!;
		if (current.State == SpellLifecycleState.Completed || current.Diagnostic == diagnostic) return;
		_store.Hold(current.Origin.Id, current.Version, diagnostic[..Math.Min(diagnostic.Length, 2048)], TransitionTime(current, nowUtc));
	}

	internal void EvacuateBody(IBody body, SpellOwnedLifecycle life, SpatialLocation destination,
		IReadOnlyCollection<MudSharp.Effects.IEffect>? ownedEffects = null)
	{
		_evacuationRetries.TryGetValue(body.Id, out var retrySnapshot);
		var retryRoots = retrySnapshot?.Roots;
		if (retryRoots?.Any(x => x.Deleted || x.InInventoryOf is { } custodian && !ReferenceEquals(custodian, body) ||
			x.ContainedIn is not null || x.Location is { } room && !ReferenceEquals(room, destination.Room)) == true)
			throw new InvalidOperationException("Previously transferred foreign goods changed custody; preserve the owned body for review.");
		if (!TryCaptureForeignCustody(body, out var roots, out var graph, out var error, retryRoots)) throw new InvalidOperationException(error);
		if (retrySnapshot is not null && !retrySnapshot.Matches(roots, graph))
			throw new InvalidOperationException("Previously transferred foreign custody topology changed; retain its exact retry graph for review.");
		var snapshot = retrySnapshot ?? new ForeignCustodySnapshot(roots, graph);
		var permittedEffects = ownedEffects?.ToArray() ?? [];
		bool ActorEffectsUnchanged() => body.Actor.Effects.All(permittedEffects.Contains) && permittedEffects.All(body.Actor.Effects.Contains);
		var structuralComponents = graph.SelectMany(x => x.Components).Where(c =>
			c is IContainer or ILockable or IBelt or IFirearmAttachmentHost or ISeveredBodypart).ToArray();
		if (body.Implants.Any() || body.Prosthetics.Any() || body.Wounds.Any(x => x.Lodged is not null))
			throw new InvalidOperationException("Installed prosthetics, implants or lodged goods require a separately verified native detachment adapter.");
		if (roots.Any(x => x.ContainedIn is not null) || structuralComponents.Any(c => c.Changed))
			throw new InvalidOperationException("Foreign subtree structural edits or external containment must settle before evacuation.");
		if (body is not MudSharp.Body.Implementations.Body nativeBody || !RetirementBodyEffects.TryCapture(body, out var bodyEffectsUnchanged) || body.Actor.Effects.Any(x => ownedEffects?.Contains(x) != true) ||
			body.Actor.PositionTarget is not null || graph.Any(x => x is not GameItem || x.Effects.Any() || x.Wounds.Any() ||
				x.PositionTarget is not null || x.PositionEmote is not null || x.TargetedBy.Any()))
			throw new InvalidOperationException($"Foreign custody requires a verified callback-free rollback adapter for its body, effects, wounds and position graph. " +
				$"Body effects [{string.Join(", ", body.Effects.Select(x => x.GetType().Name))}]; " +
				$"actor effects [{string.Join(", ", body.Actor.Effects.Select(x => x.GetType().Name))}]; " +
				$"actor position target {body.Actor.PositionTarget is not null}; " +
				$"item effects [{string.Join(", ", graph.SelectMany(x => x.Effects).Select(x => x.GetType().Name))}]; " +
				$"items with wounds {graph.Count(x => x.Wounds.Any())}, position targets {graph.Count(x => x.PositionTarget is not null)}, " +
				$"position emotes {graph.Count(x => x.PositionEmote is not null)}, targeting references {graph.Count(x => x.TargetedBy.Any())}, " +
				$"non-native items {graph.Count(x => x is not GameItem)}.");
		var componentRestores = graph.SelectMany(x => x.Components).Select(x =>
			(Component: x, Restore: (x as GameItemComponent)?.CaptureCustodyRollback())).ToArray();
		if (componentRestores.Any(x => x.Restore is null))
			throw new InvalidOperationException("Foreign structural custody has no verified native rollback adapter; retain the original graph.");
		var itemRestores = graph.Cast<GameItem>().Select(x => (Item: x, Restore: x.CaptureCustodyRollback())).ToArray();
		var restoreInventory = nativeBody.CaptureCustodyInventoryRollback();
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		var context = FMDB.Context;
		var current = context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities).Single(x => x.Id == life.Origin.Id);
		var authority = SpellOwnedLifecycleStore.Read(current);
		if (current.Version != life.Version || !(HasSimpleRetirementClaims(authority) || SpellOwnedProjectionService.HasClaims(authority)) ||
			!authority.Entities.Any(x => x.Kind == SpellOwnedEntityKind.Body && x.Id == body.Id))
			throw new InvalidOperationException("The exact evacuation authority changed; reload before transferring custody.");
		var persisted = context.BodiesGameItems.Where(x => x.BodyId == body.Id).Select(x => x.GameItemId)
			.Concat(context.BodiesImplants.Where(x => x.BodyId == body.Id).Select(x => x.ImplantId))
			.Concat(context.BodiesProsthetics.Where(x => x.BodyId == body.Id).Select(x => x.ProstheticId))
			.Concat(context.Wounds.Where(x => x.BodyId == body.Id && x.LodgedItemId != null).Select(x => x.LodgedItemId!.Value)).ToArray();
		var ids = graph.Select(x => x.Id).ToArray();
		if (persisted.Any(x => !ids.Contains(x)) || context.GameItems.Count(x => ids.Contains(x.Id)) != ids.Length)
			throw new InvalidOperationException("Persisted or missing foreign custody is not fully loaded; preserve the native body.");
		if (roots.Length == 0) { transaction.Commit(); return; }
		if (!ReferenceEquals(destination.Room?.Gameworld, body.Gameworld) ||
			!RouteSpatialService.Instance.TryValidateLocation(destination, out error) ||
			!context.Rooms.Any(x => x.Id == destination.Room.Id) || destination.Room is not ICustodyRollbackLocation rollbackLocation)
			throw new InvalidOperationException("Foreign custody has no validated persisted destination; retain a recoverable holding graph.");
		var restoreLocation = rollbackLocation.CaptureCustodyMembershipRollback(graph)
			?? throw new InvalidOperationException("Foreign custody has no verified location rollback adapter.");
		_evacuationRetries[body.Id] = snapshot;
		var resumeItems = body.Actor.State.HasFlag(CharacterState.Dead);
		IGameItemComponent[] savedComponents = [];
		using var transfer = ForeignCustodyTransferContext.Enter(body, graph, destination.Room);
		try
		{
		foreach (var item in roots)
		{
			body.Take(item);
			item.Get(null);
			item.InsertAtSpatialLocation(destination, newStack: true);
		}
		body.RecalculateItemHelpers();
		var topologyChanged = !snapshot.Matches(roots, graph);
		if (!bodyEffectsUnchanged() || !ActorEffectsUnchanged() || body.AllItems.Any() || roots.Any(x => x.Deleted || !ReferenceEquals(x.Location, destination.Room)) ||
			graph.Any(x => x.Deleted) || !TryCaptureForeignCustody(body, out _, out var after, out _, roots) ||
			!new HashSet<IGameItem>(graph, ReferenceEqualityComparer.Instance).SetEquals(after) ||
			topologyChanged || structuralComponents.Any(c => c.Changed))
		{
			// A callback can save a component and clear its dirty flag inside this transaction.
			// Keep the original typed topology across retries and restore structural flags before
			// rollback rather than treating the same item IDs as proof of unchanged custody.
			if (topologyChanged) foreach (var component in structuralComponents) component.Changed = true;
			throw new InvalidOperationException("A native transfer callback changed custody; retain the owned graph and retry safely.");
		}
		body.Save();
		savedComponents = roots.SelectMany(x => x.Components).Where(x => x.Changed).ToArray();
		foreach (var item in roots)
		{
			item.Save();
			// A previous failed native transfer may have cleared the position dirty flag.
			// This exact cut always releases its physical position target without changing legal ownership.
			var row = context.GameItems.Find(item.Id)!;
			row.PositionTargetId = null; row.PositionTargetType = string.Empty;
		}
		foreach (var component in savedComponents) component.Save();
		var rootIds = roots.Select(x => x.Id).ToArray();
		context.RoomsGameItems.RemoveRange(context.RoomsGameItems.Where(x => rootIds.Contains(x.GameItemId)));
		foreach (var item in roots) context.RoomsGameItems.Add(new() { RoomId = destination.Room.Id, GameItemId = item.Id });
		context.SaveChanges(); transaction.Commit();
		}
		catch (Exception original)
		{
			// Provider rollback can itself fail. Every callback-free native restore still
			// runs independently; a secondary error must never skip the remaining graph.
			var failures = new List<Exception> { original };
			void Recover(Action action) { try { action(); } catch (Exception ex) { failures.Add(ex); } }
			Recover(transaction.Rollback);
			foreach (var entry in componentRestores) Recover(entry.Restore!);
			foreach (var entry in itemRestores) Recover(entry.Restore);
			Recover(restoreInventory);
			Recover(restoreLocation);
			transfer.RestorePendingSaves(Recover);
			foreach (var entry in componentRestores) Recover(() => entry.Component.Changed = true);
			foreach (var item in graph) Recover(() => RouteSpatialService.Instance.TrackPerceivable(item));
			Recover(body.RecalculateItemHelpers);
			if (!snapshot.Matches(roots, graph) || graph.Any(x => x.Deleted) ||
				!roots.All(x => ReferenceEquals(x.InInventoryOf, body)) || destination.Room.GameItems.Any(graph.Contains))
				failures.Add(new InvalidOperationException("Native custody rollback could not restore its exact captured graph."));
			if (failures.Count > 1) throw new AggregateException("Native custody transfer and compensation failed; retain the retirement hold.", failures);
			throw;
		}
		// Committed custody must never enter the compensation catch. Runtime activation
		// failures leave the durable cut intact for reconciliation instead of rolling it back.
		transfer.Dispose();
		_evacuationRetries.Remove(body.Id);
		body.Gameworld.SaveManager.Abort(body);
		foreach (var item in roots)
		{
			if (!body.Gameworld.Items.Has(item.Id)) body.Gameworld.Add(item);
			if (resumeItems) item.Login();
		}
	}

	internal static bool TryCaptureForeignCustody(IBody body, out IGameItem[] roots, out IGameItem[] graph, out string error,
		IEnumerable<IGameItem>? additionalRoots = null)
	{
		roots = []; graph = []; error = string.Empty;
		var candidates = body.AllItems.Concat(additionalRoots ?? []).Distinct<IGameItem>(ReferenceEqualityComparer.Instance).ToArray();
		var items = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var children = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var pending = new Queue<IGameItem>(candidates);
		var edges = new Dictionary<IGameItem, IGameItem[]>(ReferenceEqualityComparer.Instance);
		while (pending.TryDequeue(out var item))
		{
			if (!items.Add(item)) continue;
			if (items.Count > 256 || item is ILateInitialisingItem { IdHasBeenRegistered: false } || item.Id <= 0 || item.Deleted || !ReferenceEquals(item.Gameworld, body.Gameworld) ||
				item.ConnectedItems.Any() || item.Components.OfType<IProvideItemTargetProjections>().Any(x => x.TargetProjections.Any()))
			{ error = "Unpersisted, connected, projected or oversized foreign custody requires an explicit adapter."; return false; }
			var nested = ForeignRelations(item).SelectMany(x => x.Items).Distinct<IGameItem>(ReferenceEqualityComparer.Instance).ToArray();
			edges.Add(item, nested);
			foreach (var child in nested) { children.Add(child); pending.Enqueue(child); }
		}
		roots = candidates.Where(x => !children.Contains(x)).ToArray(); graph = items.ToArray();
		var active = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		var visited = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		bool IsAcyclic(IGameItem item)
		{
			if (active.Contains(item)) return false;
			if (!visited.Add(item)) return true;
			active.Add(item);
			foreach (var child in edges[item]) if (!IsAcyclic(child)) return false;
			active.Remove(item); return true;
		}
		if (roots.Length == 0 && graph.Length != 0 || graph.Select(x => x.Id).Distinct().Count() != graph.Length || graph.Any(x => !IsAcyclic(x)))
		{ error = "Cyclic or duplicate-identity foreign custody cannot be evacuated safely."; return false; }
		return true;
	}

	private sealed record ForeignRelation(object Source, string Kind, HashSet<IGameItem> Items);

	private static IEnumerable<ForeignRelation> ForeignRelations(IGameItem item)
	{
		HashSet<IGameItem> Items(IEnumerable<IGameItem> items) => new(items, ReferenceEqualityComparer.Instance);
		foreach (var container in item.Components.OfType<IContainer>())
			yield return new(container, "contents", Items(container.Contents));
		foreach (var lockable in item.Components.OfType<ILockable>())
			yield return new(lockable, "locks", Items(lockable.Locks.Select(x => x.Parent)));
		if (item.GetItemType<IBelt>() is { } belt)
			yield return new(belt, "belt", Items(belt.ConnectedItems.Select(x => x.Parent)));
		if (item.GetItemType<IFirearmAttachmentHost>() is { } firearm)
			foreach (var slot in firearm.InstalledAttachments)
				yield return new(firearm, "attachment:" + slot.Key, Items([slot.Value.Parent]));
		if (item.GetItemType<ISeveredBodypart>() is { } severed)
			yield return new(severed, "implants", Items(severed.Implants));
		foreach (var wound in item.Wounds.Where(x => x is not null))
			yield return new(wound, "lodged", Items(wound.Lodged is { } lodged ? [lodged] : []));
	}

	private sealed class ForeignCustodySnapshot
	{
		public IGameItem[] Roots { get; }
		private readonly HashSet<IGameItem> _graph;
		private readonly Dictionary<IGameItem, ForeignRelation[]> _relations = new(ReferenceEqualityComparer.Instance);
		private readonly Dictionary<IGameItem, IGameItem?> _nestedParents = new(ReferenceEqualityComparer.Instance);

		public ForeignCustodySnapshot(IGameItem[] roots, IGameItem[] graph)
		{
			Roots = roots;
			_graph = new(graph, ReferenceEqualityComparer.Instance);
			foreach (var item in graph)
			{
				_relations.Add(item, ForeignRelations(item).ToArray());
				if (!roots.Contains(item, ReferenceEqualityComparer.Instance)) _nestedParents.Add(item, item.ContainedIn);
			}
		}

		public bool Matches(IGameItem[] roots, IGameItem[] graph)
		{
			if (!new HashSet<IGameItem>(Roots, ReferenceEqualityComparer.Instance).SetEquals(roots) || !_graph.SetEquals(graph) ||
				_nestedParents.Any(x => !ReferenceEquals(x.Key.ContainedIn, x.Value))) return false;
			foreach (var item in graph)
			{
				var current = ForeignRelations(item).ToArray(); var previous = _relations[item];
				if (current.Length != previous.Length || previous.Any(x => !current.Any(y =>
					ReferenceEquals(x.Source, y.Source) && x.Kind == y.Kind && x.Items.SetEquals(y.Items)))) return false;
			}
			return true;
		}
	}
}
