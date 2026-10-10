#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;

namespace MudSharp.Magic.Lifecycle;

/// <summary>Creation and terminal removal of exact single leaf items, never their custodian or host.</summary>
public sealed class SpellOwnedItemService : ISpellOwnedItemService
{
	private readonly IFuturemud _world;
	private readonly Func<HashSet<long>> _readClaimedItemIds;
	private HashSet<long>? _claimedItemIds;
	private readonly SpellOwnedLifecycleStore _store = new();
	private long _lastInspectedItemId;
	internal const string ActivationPending = "Native item activation pending";

	public SpellOwnedItemService(IFuturemud world) : this(world, ReadClaimedItemIds)
	{
	}

	internal SpellOwnedItemService(IFuturemud world, Func<HashSet<long>> readClaimedItemIds)
	{
		_world = world;
		_readClaimedItemIds = readClaimedItemIds;
	}

	private static HashSet<long> ReadClaimedItemIds()
	{
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		return FMDB.Context.MagicSpellOwnedEntities.AsNoTracking()
			.Where(x => x.Kind == (int)SpellOwnedEntityKind.GameItem)
			.Select(x => x.EntityId)
			.ToHashSet();
	}

	private bool HasClaim(long itemId) => itemId > 0 &&
		(_claimedItemIds ??= _readClaimedItemIds()).Contains(itemId);

	internal void RegisterClaimedItem(long itemId) => _claimedItemIds?.Add(itemId);

	public IGameItem Create(IGameItemProto prototype, ICharacter caster, ItemQuality quality, SpellLifecycleOrigin origin)
	{
		origin.Validate();
		if (origin.Mode == SpellLifecycleMode.DeathOnExpiry || !Enum.IsDefined(quality) || !ReferenceEquals(caster.Gameworld, _world) ||
			origin.CreatorId != MudSharp.Character.CharacterInstanceIdentityComparer.IdentityId(caster))
			throw new ArgumentException("Native item creation requires a valid item mode, quality and caster world.");
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		if (NativeItemCreationEligibility.Error(prototype, _world) is { } error) throw new InvalidOperationException(error);
		if (_store.Find(origin.Id) is not null) throw new InvalidOperationException("This native item creation cannot be replayed.");
		var item = new GameItem(prototype, caster, quality, deferSpellInitialisation: true);
		// The light's usable state is part of its private birth, before rows or exposure.
		if (item.GetItemType<MudSharp.GameItems.Components.ProgLightGameItemComponent>() is { } light) light.Lit = true;
		MudSharp.Models.GameItem? inserted = null;
		var components = new List<(GameItemComponent Component, MudSharp.Models.GameItemComponent Row)>();
		var life = _store.Create(origin, creation =>
		{
			inserted = (MudSharp.Models.GameItem)item.DatabaseInsert();
			foreach (var component in item.Components.Cast<GameItemComponent>())
				components.Add((component, (MudSharp.Models.GameItemComponent)component.DatabaseInsert()));
			creation.Claim(SpellOwnedEntityKind.GameItem, inserted);
		}, ActivationPending);
		// Claims are immutable. Publish a newly committed ID before native activation can load it.
		_claimedItemIds?.Add(inserted!.Id);
		try
		{
			item.ActivateCommittedSpellItem(inserted!, origin, components);
			using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
			var row = FMDB.Context.MagicSpellLifecycles.Single(x => x.Id == origin.Id);
			if (row.Version != life.Version || row.Diagnostic != ActivationPending || row.State != (int)SpellLifecycleState.Active)
				throw new InvalidOperationException("Native item activation changed concurrently; retain the committed graph.");
			row.Diagnostic = ""; row.Version = checked(row.Version + 1); row.UpdatedUtc = TransitionTime(life, RuntimeClock.UtcNow);
			if (origin.Mode == SpellLifecycleMode.Permanent) row.State = (int)SpellLifecycleState.Completed;
			FMDB.Context.SaveChanges();
			return item;
		}
		catch (Exception ex)
		{
			item.SetNoSave(true); foreach (var component in components) component.Component.SetNoSave(true);
			Hold(_store.Find(origin.Id)!, ActivationPending + ": committed creation must not replay: " + ex.Message, RuntimeClock.UtcNow);
			throw;
		}
	}

	public SpellOwnedItemOrigin? FindOrigin(long itemId)
	{
		if (!HasClaim(itemId)) return null;
		var life = Find(itemId);
		return life is null ? null : new(life.Origin.Id, life.Origin.Mode, life.Origin.DeadlineUtc);
	}

	private SpellOwnedLifecycle? Find(long itemId)
	{
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		return FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
			.SingleOrDefault(x => x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.GameItem && e.EntityId == itemId))
			is { } row ? SpellOwnedLifecycleStore.Read(row) : null;
	}

	public bool IsActivationPending(long itemId)
	{
		if (!HasClaim(itemId)) return false;
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		return FMDB.Context.MagicSpellLifecycles.Any(x => x.Diagnostic.StartsWith(ActivationPending) &&
			x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.GameItem && e.EntityId == itemId));
	}

	public bool TryPrepareRemoval(IGameItem item, out string diagnostic)
	{
		diagnostic = "";
		if (item.SpellCreationOrigin?.IsTemporary != true) return true;
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		var life = Find(item.Id);
		if (life?.Origin.Family == SpellProjectionAnchor.Family)
		{
			_world.SpellOwnedProjections?.RequestAnchorRemoval(item.Id);
			diagnostic = "The effigy retires through its exact projection ownership adapter.";
			return false;
		}
		if (life?.Origin.Family == SpellShelterAnchor.Family)
		{
			_world.SpellOwnedShelters?.RequestRetirement(life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Room).Id,
				SpellRetirementReason.Dismissal);
			diagnostic = "The finite supply retires with its shelter through the exact topology adapter.";
			return false;
		}
		try
		{
			if (!ReferenceEquals(item.Gameworld, _world) || item is not GameItem || life is null || life.State == SpellLifecycleState.Completed ||
				life.Origin.Mode != SpellLifecycleMode.TemporaryCleanup || life.Origin.Id != item.SpellCreationOrigin.LifecycleId || !life.MayRemoveOwnedEntities ||
				life.Entities.Count != 1 || life.Entities[0] is not { Kind: SpellOwnedEntityKind.GameItem, Role: SpellOwnedEntityRole.CreatedEntity } ||
				life.Entities[0].Id != item.Id || life.Diagnostic.StartsWith(ActivationPending, StringComparison.Ordinal))
				throw new InvalidOperationException("Removal requires exact activated single-item creation authority.");
			if (item.DeepItems.Skip(1).Any() || item.AttachedAndConnectedItems.Any() || item.LodgedItems.Any() ||
				item.Wounds.Any(x => x.Lodged is not null) || item.Effects.Any() || item.Hooks.Any() ||
				item.PositionTarget is not null || item.TargetedBy.Any() || item.PositionEmote is not null)
				throw new InvalidOperationException("Foreign goods, hooks or effects need a detachment adapter; retain this recoverable item.");
			using (var db = new FMDB())
			{
				if (FMDB.Context.Wounds.Any(x => x.LodgedItemId == item.Id))
					throw new InvalidOperationException("An external wound retains this item; release it before removal.");
				var container = FMDB.Context.GameItems.AsNoTracking().Where(x => x.Id == item.Id).Select(x => x.ContainerId).SingleOrDefault();
				var bodies = FMDB.Context.BodiesGameItems.AsNoTracking().Where(x => x.GameItemId == item.Id).Select(x => x.BodyId).ToArray();
				var rooms = FMDB.Context.RoomsGameItems.AsNoTracking().Where(x => x.GameItemId == item.Id).Select(x => x.RoomId).ToArray();
				if (container is { } host && item.ContainedIn?.Id != host ||
					bodies.Any(x => item.InInventoryOf?.Id != x) || rooms.Any(x => item.Location?.Id != x))
					throw new InvalidOperationException("Persisted custody is not loaded or differs from live custody; load the custodian and save its current state before removal.");
				if (FMDB.Context.BodiesImplants.Any(x => x.ImplantId == item.Id) || FMDB.Context.BodiesProsthetics.Any(x => x.ProstheticId == item.Id))
					throw new InvalidOperationException("An installed body item needs its own detachment adapter; retain this item.");
			}
			if (life.State == SpellLifecycleState.Active)
				_store.BeginRetirement(life.Origin.Id, life.Version,
					RuntimeClock.UtcNow >= life.Origin.DeadlineUtc ? SpellRetirementReason.Expiry : SpellRetirementReason.EarlyItemRemoval,
					TransitionTime(life, RuntimeClock.UtcNow));
			return true;
		}
		catch (Exception ex)
		{
			diagnostic = "Native item removal held: " + ex.Message;
			if (life is not null) Hold(life, diagnostic, RuntimeClock.UtcNow);
			return false;
		}
	}

	public void ObserveRemoval(IGameItem item)
	{
		if (!item.Deleted || item.SpellCreationOrigin?.IsTemporary != true) return;
		var life = Find(item.Id);
		if (life is { State: SpellLifecycleState.Retiring } && life.Origin.Family != SpellProjectionAnchor.Family) _store.Complete(life.Origin.Id, life.Version, TransitionTime(life, RuntimeClock.UtcNow));
	}

	public int ReconcileRetirements(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000) throw new ArgumentException("A bounded UTC item reconciliation is required.");
		using var authorization = FMDB.BeginIndependentScope(requireWrites: true);
		(SpellOwnedLifecycle Life, long ItemId)[] ReadPending()
		{
			using var db = new FMDB();
			return (from claim in FMDB.Context.MagicSpellOwnedEntities.AsNoTracking()
				join life in FMDB.Context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities) on claim.LifecycleId equals life.Id
				where claim.Kind == (int)SpellOwnedEntityKind.GameItem && claim.EntityId > _lastInspectedItemId &&
					(life.State == (int)SpellLifecycleState.Retiring || life.State == (int)SpellLifecycleState.Active && life.DeadlineUtc <= nowUtc)
				orderby claim.EntityId select new { Life = life, ItemId = claim.EntityId }).Take(limit).ToArray()
				.Select(x => (SpellOwnedLifecycleStore.Read(x.Life), x.ItemId)).ToArray();
		}
		var pending = ReadPending();
		if (pending.Length == 0 && _lastInspectedItemId != 0) { _lastInspectedItemId = 0; pending = ReadPending(); }
		foreach (var (initial, id) in pending)
		{
			_lastInspectedItemId = id; var life = initial;
			try
			{
				if (life.Diagnostic.StartsWith(ActivationPending, StringComparison.Ordinal)) continue;
				if (life.Entities.Count != 1 || life.Entities[0].Kind != SpellOwnedEntityKind.GameItem || !life.MayRemoveOwnedEntities)
					continue; // NPC possessions, projections and future compound adapters own their own removal.
				if (life.State == SpellLifecycleState.Active)
					life = _store.BeginRetirement(life.Origin.Id, life.Version, SpellRetirementReason.Expiry, TransitionTime(life, nowUtc));
				using (var db = new FMDB())
					if (!FMDB.Context.GameItems.Any(x => x.Id == id))
					{
						if (_world.Items.FirstOrDefault(x => x.Id == id) is { } cached)
						{
							if (cached is not GameItem native || !ReferenceEquals(cached.Gameworld, _world) || cached.SpellCreationOrigin?.LifecycleId != life.Origin.Id)
								throw new InvalidOperationException("Cached item differs from its committed retirement authority.");
							native.FinishCommittedSpellItemRemoval();
							life = _store.Find(life.Origin.Id)!;
						}
						if (life.State != SpellLifecycleState.Completed) _store.Complete(life.Origin.Id, life.Version, TransitionTime(life, nowUtc));
						continue;
					}
				var item = _world.TryGetItem(id, true) ?? throw new InvalidOperationException("The exact persisted item is unavailable; retain its row.");
				if (!ReferenceEquals(item.Gameworld, _world) || item.SpellCreationOrigin?.LifecycleId != life.Origin.Id)
					throw new InvalidOperationException("Loaded item does not match this creation.");
				item.Delete();
				ObserveRemoval(item);
			}
			catch (Exception ex) { Hold(_store.Find(initial.Origin.Id)!, "Native item reconciliation held: " + ex.Message, nowUtc); }
		}
		return pending.Length;
	}

	private void Hold(SpellOwnedLifecycle life, string diagnostic, DateTime nowUtc)
	{
		if (life.State == SpellLifecycleState.Completed) return;
		_store.Hold(life.Origin.Id, life.Version, diagnostic[..Math.Min(diagnostic.Length, 2048)], TransitionTime(life, nowUtc));
	}
	private static DateTime TransitionTime(SpellOwnedLifecycle life, DateTime time) => time < life.UpdatedUtc ? life.UpdatedUtc : time;
}
