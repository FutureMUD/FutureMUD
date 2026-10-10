#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Magic.Lifecycle;

/// <summary>Owns only a newly created pocket carrier. Stored items and the casting focus remain borrowed.</summary>
public sealed class SpellOwnedPocketService(IFuturemud world) : ISpellOwnedPocketService
{
	private readonly SpellOwnedLifecycleStore _store = new();
	internal static string? PrototypeError(IGameItemProto? prototype, IFuturemud world)
	{
		if (prototype is not GameItemProto native || !ReferenceEquals(prototype.Gameworld, world) || prototype.Status != RevisionStatus.Current ||
			prototype.PreventManualLoad || native.OnLoadProgs.Any() || prototype.Morphs ||
			world.DefaultHooks.Any(x => x.PerceivableType.EqualTo("GameItem"))) return "Select an approved loadable unscripted pocket prototype without morph/default hooks.";
		var types = prototype.Components.Select(x => x.GetType()).ToArray();
		return types.Length == 2 && types.Contains(typeof(HoldableGameItemComponentProto)) && types.Contains(typeof(FoldedPocketGameItemComponentProto))
			? null : "Pocket creation requires exactly native holdable and folded-pocket components.";
	}
	public string? AdmissionError(ICharacter caster, IGameItem source, SpellPocketConfiguration configuration, int grade)
	{
		try { configuration.Validate(); } catch (ArgumentException ex) { return ex.Message; }
		if (grade is < 1 or > 7 || !ReferenceEquals(caster.Gameworld, world) || !ReferenceEquals(source.Gameworld, world) ||
			source is not GameItem || source.Id <= 0 || source.Deleted || source.Destroyed || !caster.Body.HeldItems.Contains(source) ||
			source.IsItemType<ISpellPocket>() || source.Components.OfType<IProvideItemTargetProjections>().Any(x => x.TargetProjections.Any()))
			return "Select a persisted ordinary item held by the caster; pocket spaces and physical occupants are unsuitable targets.";
		if (world.SpellOwnedItems is not SpellOwnedItemService) return "Native pocket creation is unavailable.";
		if (PrototypeError(world.ItemProtos.Get(configuration.PrototypeId), world) is { } error) return error;
		if (!TryFallback(configuration.FallbackRoomId, out _)) return "Select a valid permanent ground-level fallback room.";
		var location = RouteSpatialService.Instance.GetEffectiveLocation(caster);
		return RouteSpatialService.Instance.TryValidateLocation(location, out error) && location.Room is ICustodyRollbackLocation
			? null : "The caster has no valid native placement destination.";
	}
	public IGameItem Create(ICharacter caster, IGameItem source, SpellPocketConfiguration configuration, SpellLifecycleOrigin origin)
	{
		if (AdmissionError(caster, source, configuration, origin.Grade) is { } error) throw new InvalidOperationException(error);
		var anchor = new SpellPocketAnchor(source.Id, origin.Grade, configuration);
		if (origin.Family != SpellPocketAnchor.Family || origin.Provenance != anchor.Save() || origin.CreatorId != CharacterInstanceIdentityComparer.IdentityId(caster) ||
			origin.Mode != SpellLifecycleMode.TemporaryCleanup || origin.CreatedUtc.Ticks % 10 != 0 ||
			origin.DeadlineUtc != origin.CreatedUtc + configuration.DurationForGrade(origin.Grade))
			throw new InvalidOperationException("Pocket creation policy differs from its frozen invocation.");
		var carrier = ((SpellOwnedItemService)world.SpellOwnedItems!).CreatePocket(world.ItemProtos.Get(configuration.PrototypeId), caster, origin, anchor);
		PlaceCommittedCarrier(carrier, caster);
		return carrier;
	}
	internal void PlaceCommittedCarrier(IGameItem carrier, ICharacter caster)
	{
		// Publish the committed instance before custody can expose it to retirement reconciliation.
		world.Add(carrier);
		if (caster.Body.CanGet(carrier, 0)) caster.Body.Get(carrier, silent: true);
		else carrier.InsertAtSource(caster, true);
	}
	private SpellOwnedLifecycle RequireAuthority(IGameItem carrier)
	{
		var pocket = carrier.GetItemType<FoldedPocketGameItemComponent>();
		var life = pocket is null || pocket.LifecycleId == Guid.Empty ? null : _store.Find(pocket.LifecycleId);
		if (carrier is not GameItem || !ReferenceEquals(carrier.Gameworld, world) || pocket?.Anchor is null || life is null ||
			life.Origin.Family != SpellPocketAnchor.Family || life.Origin.Mode != SpellLifecycleMode.TemporaryCleanup ||
			life.Origin.Provenance != pocket.Anchor.Save() || life.Origin.Grade != pocket.Anchor.Grade ||
			carrier.SpellCreationOrigin?.LifecycleId != life.Origin.Id || life.Entities.Count != 1 ||
			life.Entities[0] is not { Kind: SpellOwnedEntityKind.GameItem, Role: SpellOwnedEntityRole.CreatedEntity } || life.Entities[0].Id != carrier.Id ||
			carrier.Prototype.Id != pocket.Anchor.Configuration.PrototypeId || life.Diagnostic.StartsWith(SpellOwnedItemService.ActivationPending, StringComparison.Ordinal))
			throw new InvalidOperationException("Pocket removal requires its exact activated carrier, configuration and single-item creation claim.");
		if (carrier.Components.Count() != 2 || carrier.Components.Any(x => x.GetType() != typeof(HoldableGameItemComponent) && x.GetType() != typeof(FoldedPocketGameItemComponent)) ||
			pocket.Locks.Any() || life.Origin.DeadlineUtc != life.Origin.CreatedUtc + pocket.Anchor.Configuration.DurationForGrade(life.Origin.Grade))
			throw new InvalidOperationException("Pocket components or original deadline differ from their creation policy.");
		return life;
	}
	public bool IsActive(IGameItem carrier)
	{
		try { var life = RequireAuthority(carrier); return life.State == SpellLifecycleState.Active && life.Origin.DeadlineUtc > RuntimeClock.UtcNow && !carrier.Deleted && !carrier.Destroyed; }
		catch (Exception) { return false; }
	}
	public bool CanWithdraw(IGameItem carrier)
	{
		try { return RequireAuthority(carrier).State != SpellLifecycleState.Completed && !carrier.Deleted; }
		catch (Exception) { return false; }
	}
	public bool TryPrepareRemoval(IGameItem carrier, out string diagnostic)
	{
		diagnostic = ""; SpellOwnedLifecycle? life = null;
		try
		{
			life = RequireAuthority(carrier);
			if (life.State == SpellLifecycleState.Completed) throw new InvalidOperationException("The pocket is already completed.");
			if (life.State == SpellLifecycleState.Active) life = _store.BeginRetirement(life.Origin.Id, life.Version,
				RuntimeClock.UtcNow >= life.Origin.DeadlineUtc ? SpellRetirementReason.Expiry : SpellRetirementReason.EarlyItemRemoval, Time(life));
			if (((GameItem)carrier).SpellRemovalCustodianError() is { } custodianError) throw new InvalidOperationException(custodianError);
			Evacuate((GameItem)carrier, life);
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			var row = FMDB.Context.GameItems.Find(carrier.Id);
			if (row is not null && !CharacterArchiveService.ProjectionItemReferencesAreClear(FMDB.Context, row, out diagnostic)) throw new InvalidOperationException(diagnostic);
			if (!ReferencesAreClear(FMDB.Context, carrier, out diagnostic)) throw new InvalidOperationException(diagnostic);
			if (carrier.TargetedBy.Any() || carrier.PositionTarget is not null || carrier.Effects.Any() || carrier.Hooks.Any() || carrier.DeepItems.Skip(1).Any())
				throw new InvalidOperationException("The emptied pocket retains a native dependency; preserve its carrier.");
			return true;
		}
		catch (Exception ex)
		{
			diagnostic = "Pocket retirement held: " + ex.Message;
			if (life is not null && _store.Find(life.Origin.Id) is { State: not SpellLifecycleState.Completed } current)
				_store.Hold(current.Origin.Id, current.Version, diagnostic[..Math.Min(2048, diagnostic.Length)], Time(current));
			return false;
		}
	}
	private bool TryFallback(long id, out SpatialLocation location)
	{
		location = new(world.Rooms.Get(id)!, RoomLayer.GroundLevel);
		return location.Room is ICustodyRollbackLocation && world.SpellOwnedShelters?.OwnsRoom(id) != true &&
			RouteSpatialService.Instance.TryValidateLocation(location, out _);
	}
	private static DateTime Time(SpellOwnedLifecycle life) => RuntimeClock.UtcNow < life.UpdatedUtc ? life.UpdatedUtc : RuntimeClock.UtcNow;
	internal static bool ReferencesAreClear(FuturemudDatabaseContext context, IGameItem carrier, out string diagnostic)
	{
		var targets = new PhysicalReferenceTargets([new(PhysicalEntityKind.GameItem, carrier.Id, "PocketCarrier")]);
		if (!PhysicalReferenceGuard.PersistedReferencesAreClear(context, targets, [], [], [], carrier.Id, out diagnostic)) return false;
		if (PhysicalReferenceGuard.RuntimeReferencesAreClear(carrier.Gameworld, targets)) return true;
		diagnostic = "A live typed effect retains the pocket carrier."; return false;
	}

	private void Evacuate(GameItem carrier, SpellOwnedLifecycle life)
	{
		var pocket = carrier.GetItemType<FoldedPocketGameItemComponent>()!;
		var roots = pocket.Contents.ToArray();
		if (!SpellPocketContainment.TryGraph(roots, out var graph, out var error)) throw new InvalidOperationException(error);
		if (roots.Length == 0) return;
		if (roots.Any(x => !ReferenceEquals(x.ContainedIn, carrier)) || graph.Any(x => x is not GameItem || x.Id <= 0 ||
			!ReferenceEquals(x.Gameworld, world) || x.Effects.Any() || x.Hooks.Any() || x.Wounds.Any() || x.PositionTarget is not null ||
			x.PositionEmote is not null || x.TargetedBy.Any() || x.ConnectedItems.Any() || x.Components.OfType<IProvideItemTargetProjections>().Any(p => p.TargetProjections.Any())))
			throw new InvalidOperationException("Foreign pocket goods need a complete native callback-free custody adapter.");
		if (carrier.Effects.Any() || carrier.Hooks.Any() || carrier.AttachedAndConnectedItems.Any() || carrier.LodgedItems.Any())
			throw new InvalidOperationException("The carrier has foreign effects or structural dependants.");
		var destination = RouteSpatialService.Instance.GetEffectiveLocation(carrier.LocationLevelPerceivable);
		if (!RouteSpatialService.Instance.TryValidateLocation(destination, out _) || destination.Room is not ICustodyRollbackLocation)
			if (!TryFallback(pocket.Anchor!.Configuration.FallbackRoomId, out destination)) throw new InvalidOperationException("No safe persisted destination exists.");
		var carrierContainer = carrier.ContainedIn; var carrierBody = carrier.InInventoryOf;
		var carrierLocation = RouteSpatialService.Instance.GetEffectiveLocation(carrier.LocationLevelPerceivable);
		var captures = graph.Select(x => (Item: x, Parent: x.ContainedIn, Children: SpellPocketContainment.Children(x).ToArray())).ToArray();
		bool Unchanged() => captures.All(x => (roots.Contains(x.Item, ReferenceEqualityComparer.Instance) || ReferenceEquals(x.Item.ContainedIn, x.Parent)) &&
			new HashSet<IGameItem>(x.Children, ReferenceEqualityComparer.Instance).SetEquals(SpellPocketContainment.Children(x.Item))) &&
			ReferenceEquals(carrier.ContainedIn, carrierContainer) && ReferenceEquals(carrier.InInventoryOf, carrierBody) &&
			RouteSpatialService.Instance.GetEffectiveLocation(carrier.LocationLevelPerceivable) == carrierLocation;
		var all = graph.Prepend(carrier).Cast<GameItem>().ToArray();
		var components = all.SelectMany(x => x.Components).Cast<GameItemComponent>().ToArray();
		var componentRestores = components.Select(x => x.CaptureCustodyRollback()).ToArray();
		if (componentRestores.Any(x => x is null)) throw new InvalidOperationException("A foreign component has no verified custody rollback.");
		var itemRestores = all.Select(x => x.CaptureCustodyRollback()).ToArray();
		var saveRestores = all.Select(x => x.CaptureCustodySaveRollback()).ToArray();
		var locationRestore = ((ICustodyRollbackLocation)destination.Room).CaptureCustodyMembershipRollback(graph);
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		var context = FMDB.Context;
		var authority = context.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities).Single(x => x.Id == life.Origin.Id);
		if (authority.Version != life.Version || authority.State != (int)SpellLifecycleState.Retiring || authority.Provenance != life.Origin.Provenance ||
			!context.Rooms.Any(x => x.Id == destination.Room.Id)) throw new InvalidOperationException("Pocket evacuation authority or destination changed.");
		var rootIds = roots.Select(x => x.Id).ToArray(); var ids = graph.Select(x => x.Id).ToArray();
		var carrierRow = context.GameItems.AsNoTracking().Single(x => x.Id == carrier.Id);
		var bodyCustody = context.BodiesGameItems.Where(x => x.GameItemId == carrier.Id).Select(x => x.BodyId).ToArray();
		var floorCustody = context.RoomsGameItems.Where(x => x.GameItemId == carrier.Id).Select(x => x.RoomId).ToArray();
		if (carrierRow.ContainerId != carrierContainer?.Id || !new HashSet<long>(bodyCustody).SetEquals(carrierBody is null ? [] : new[] { carrierBody.Id }) ||
			!new HashSet<long>(floorCustody).SetEquals(carrier.DirectLocation is null ? [] : new[] { carrier.DirectLocation.Id }))
			throw new InvalidOperationException("Persisted carrier custody differs from its loaded native custodian.");
		if (!new HashSet<long>(context.GameItems.Where(x => x.ContainerId == carrier.Id).Select(x => x.Id)).SetEquals(rootIds) ||
			context.GameItems.Count(x => ids.Contains(x.Id)) != ids.Length) throw new InvalidOperationException("Persisted pocket custody is missing or incompletely loaded.");
		if (context.BodiesGameItems.Any(x => ids.Contains(x.GameItemId)) || context.RoomsGameItems.Any(x => ids.Contains(x.GameItemId)))
			throw new InvalidOperationException("A contained foreign item has conflicting persisted floor or body custody.");
		foreach (var entry in captures)
			if (context.GameItems.AsNoTracking().Single(x => x.Id == entry.Item.Id).ContainerId != entry.Parent?.Id)
				throw new InvalidOperationException("Persisted nested custody differs from its native parent.");
		using var transfer = ForeignCustodyTransferContext.EnterRemoval(null, all, destination.Room);
		try
		{
			foreach (var item in roots)
			{
				carrier.Take(item); item.Get(null!); item.InsertAtSpatialLocation(destination, newStack: true);
				if (!Unchanged() || item.Deleted || item.ContainedIn is not null || item.InInventoryOf is not null ||
					!ReferenceEquals(item.Location, destination.Room)) throw new InvalidOperationException("A native callback changed pocket evacuation custody.");
			}
			if (pocket.Contents.Any() || !Unchanged() || graph.Any(x => x.Deleted)) throw new InvalidOperationException("Pocket contents changed during evacuation.");
			foreach (var item in all) item.Save();
			foreach (var component in components.Where(x => x.Changed)) component.Save();
			context.RoomsGameItems.RemoveRange(context.RoomsGameItems.Where(x => rootIds.Contains(x.GameItemId)));
			foreach (var item in roots) context.RoomsGameItems.Add(new() { RoomId = destination.Room.Id, GameItemId = item.Id });
			context.SaveChanges(); transaction.Commit();
		}
		catch (Exception original)
		{
			var failures = new List<Exception> { original };
			void Recover(Action? action) { if (action is null) return; try { action(); } catch (Exception ex) { failures.Add(ex); } }
			Recover(transaction.Rollback);
			foreach (var restore in componentRestores) Recover(restore);
			foreach (var restore in itemRestores) Recover(restore);
			Recover(locationRestore);
			foreach (var restore in saveRestores) Recover(restore);
			transfer.RestorePendingSaves(action => Recover(action));
			foreach (var component in components) Recover(() => component.Changed = true);
			foreach (var item in all) Recover(() => RouteSpatialService.Instance.TrackPerceivable(item));
			if (failures.Count > 1) throw new AggregateException("Pocket evacuation and compensation failed; retain the exact holding graph.", failures);
			throw;
		}
		carrierBody?.RecalculateItemHelpers();
	}
}
