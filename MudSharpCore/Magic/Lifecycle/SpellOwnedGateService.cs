#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;

namespace MudSharp.Magic.Lifecycle;

/// <summary>Distinct occupied Nilaz topology. No shelter evacuation path is called.</summary>
public sealed partial class SpellOwnedGateService : ISpellOwnedGateService
{
	private readonly IFuturemud _world;
	private readonly SpellOwnedLifecycleStore _store = new();
	private Dictionary<long, Guid>? _rooms;
	private HashSet<long>? _exits;
	private long _cursor;

	public SpellOwnedGateService(IFuturemud world) => _world = world;

	private Dictionary<long, Guid> Rooms
	{
		get
		{
			if (_rooms is not null) return _rooms;
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			_rooms = FMDB.Context.MagicSpellOwnedEntities.AsNoTracking()
				.Where(x => x.Kind == (int)SpellOwnedEntityKind.Room && x.Lifecycle.Family == SpellGateAnchor.Family &&
					x.Lifecycle.State != (int)SpellLifecycleState.Completed)
				.ToDictionary(x => x.EntityId, x => x.LifecycleId);
			return _rooms;
		}
	}

	public bool OwnsRoom(long roomId) => roomId > 0 && Rooms.ContainsKey(roomId);
	public bool OwnsExit(long exitId)
	{
		if (exitId <= 0) return false;
		if (_exits is null)
		{
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			_exits = FMDB.Context.MagicSpellOwnedEntities.AsNoTracking()
				.Where(x => x.Kind == (int)SpellOwnedEntityKind.Exit && x.Lifecycle.Family == SpellGateAnchor.Family &&
					x.Lifecycle.State != (int)SpellLifecycleState.Completed)
				.Select(x => x.EntityId).ToHashSet();
		}
		return _exits.Contains(exitId);
	}

	private bool TryMarkedDestination(ICharacter caster, IGameItem item,
		out ResolvedSpellTravelAnchor? anchor, out SpellMagicTagEffect? mark, out MagicSpellParent? parent)
	{
		mark = null; parent = null;
		if (!SpellTravelAnchorResolver.TryResolve(caster, item, SpellTravelAnchorMode.ArcaneMarkedRoom,
			out anchor, out _) || anchor!.Effect is not SpellMagicTagEffect typed ||
			typed.ParentEffect is not MagicSpellParent head || head.Identity == Guid.Empty ||
			!caster.Body.DirectItems.Any(x => ReferenceEquals(x, item))) return false;
		mark = typed; parent = head;
		return true;
	}

	private string? PresentationError(IRoom source, SpellGateConfiguration configuration,
		out Models.RoomOverlay? overlay, out long? forageProfileId)
	{
		var presentationOnly = new SpellShelterConfiguration(SpellShelterKind.SandShelter, 0, [], 0,
			configuration.SecondsPerGrade, 1, AutobuilderTemplateId: configuration.AutobuilderTemplateId);
		return new SpellOwnedShelterService(_world).ResolveTemplateOverlay(source, presentationOnly,
			out overlay, out forageProfileId);
	}

	public string? AdmissionError(ICharacter caster, IRoom source, IGameItem markedItem,
		IMagicSpell spell, SpellGateConfiguration configuration, int grade)
	{
		if (grade is < 1 or > 7 || configuration.AutobuilderTemplateId <= 0 || configuration.UndeadMeritId <= 0 ||
			_world.Merits.Get(configuration.UndeadMeritId) is null ||
			!double.IsFinite(configuration.SecondsPerGrade) || configuration.SecondsPerGrade <= 0 ||
			configuration.SecondsPerGrade * grade > (DateTime.MaxValue - RuntimeClock.UtcNow).TotalSeconds ||
			configuration.SuffocationPulseSeconds is < 10 or > 3600)
			return "Bind a valid Nilaz presentation, positive lifetime and pulse interval between 10 and 3600 seconds.";
		if (!ReferenceEquals(caster.Gameworld, _world) || !ReferenceEquals(source.Gameworld, _world) ||
			!ReferenceEquals(spell.Gameworld, _world) || !ReferenceEquals(caster.Location, source) ||
			!_world.Actors.Any(x => ReferenceEquals(x, caster)) ||
			!CharacterState.Able.HasFlag(caster.State) || caster.State.HasFlag(CharacterState.Stasis) ||
			caster.RoomLayer != RoomLayer.GroundLevel || source.Temporary || source is Room { HostedVehicleId: not null } ||
			source.RouteDefinition is not null || source.CurrentOverlay.Package.Status != RevisionStatus.Current ||
			!source.Terrain(null).TerrainLayers.Contains(RoomLayer.GroundLevel))
			return "Create a Nilaz passage from a stable permanent ground-level room.";
		if (!TryMarkedDestination(caster, markedItem, out var anchor, out _, out _) ||
			anchor!.Destination is not { } destination || ReferenceEquals(source, destination) ||
			destination.Temporary || destination is Room { HostedVehicleId: not null } ||
			destination.RouteDefinition is not null || destination.CurrentOverlay.Package.Status != RevisionStatus.Current ||
			!destination.Terrain(null).TerrainLayers.Contains(RoomLayer.GroundLevel))
			return "Exactly one active owned Arcane Mark on a directly carried item must name a distinct permanent ground-level room.";
		if (!caster.CanInteractPlanar(source, PlanarInteractionKind.MovementBarrier) ||
			!caster.CanInteractPlanar(destination, PlanarInteractionKind.MovementBarrier) ||
			MagicInterdictionHelper.GetInterdiction(caster, source, spell.School, false) is not null ||
			MagicInterdictionHelper.GetInterdiction(caster, destination, spell.School, false) is not null)
			return "A planar or magical ward prevents this Nilaz connection.";
		if (_world.DefaultHooks.Any(x => x.PerceivableType.EqualTo("Room") || x.PerceivableType.EqualTo("Cell")))
			return "Nilaz creation needs an adapter for configured default room hooks.";
		return PresentationError(source, configuration, out _, out _) ??
			PresentationError(destination, configuration, out _, out _);
	}

	public (IRoom South, IRoom North) CreatePrepared(ICharacter caster, IRoom source,
		IGameItem markedItem, IMagicSpell spell, SpellGateConfiguration configuration,
		SpellLifecycleOrigin origin, Func<bool> inputsCurrent)
	{
		ArgumentNullException.ThrowIfNull(inputsCurrent);
		void Confirm()
		{
			if (!inputsCurrent()) throw new InvalidOperationException("The prepared Nilaz source, mark or template changed.");
		}
		Confirm();
		if (AdmissionError(caster, source, markedItem, spell, configuration, origin.Grade) is { } error)
			throw new InvalidOperationException(error);
		if (!TryMarkedDestination(caster, markedItem, out var anchor, out var mark, out var parent))
			throw new InvalidOperationException("The prepared Arcane Mark is no longer current.");
		var destination = anchor!.Destination;
		var metadata = new SpellGateAnchor(source.Id, source.CurrentOverlay.Id, destination.Id,
			destination.CurrentOverlay.Id, markedItem.Id, parent!.Identity,
			configuration.AutobuilderTemplateId, configuration.UndeadMeritId,
			configuration.SuffocationPulseSeconds);
		if (origin.Family != SpellGateAnchor.Family || origin.Mode != SpellLifecycleMode.TemporaryCleanup ||
			origin.CreatorId != CharacterInstanceIdentityComparer.IdentityId(caster) ||
			origin.SpellId != spell.Id || origin.Provenance != metadata.Save() ||
			origin.DeadlineUtc != origin.CreatedUtc.AddSeconds(configuration.SecondsPerGrade * origin.Grade))
			throw new ArgumentException("Nilaz creation requires its exact typed origin and absolute deadline.");
		if (PresentationError(source, configuration, out var southOverlay, out var southForage) is { } southError)
			throw new InvalidOperationException(southError);
		if (PresentationError(destination, configuration, out var northOverlay, out var northForage) is { } northError)
			throw new InvalidOperationException(northError);
		var southRow = new Models.Room { ZoneId = source.OwningZone.Id, Temporary = true,
			EffectData = "<Effects />", ForagableProfileId = southForage,
			X = source.StoredCoordinates.X, Y = source.StoredCoordinates.Y, Z = source.StoredCoordinates.Z };
		var northRow = new Models.Room { ZoneId = destination.OwningZone.Id, Temporary = true,
			EffectData = "<Effects />", ForagableProfileId = northForage,
			X = destination.StoredCoordinates.X, Y = destination.StoredCoordinates.Y, Z = destination.StoredCoordinates.Z };
		Confirm();
		// An already paid mark is flushed once; the exact saved use debit and seven topology claims
		// then commit together or roll back together in SpellOwnedLifecycleStore.Create.
		_world.SaveManager.Flush();
		Confirm();
		var life = _store.Create(origin, creation =>
		{
			Confirm();
			creation.StageGateTopology(southRow, southOverlay!, northRow, northOverlay!, source.Id,
				source.CurrentOverlay.Id, destination.Id, destination.CurrentOverlay.Id);
			creation.DebitMarkedRoomUse(markedItem.Id, caster.Body.Id, parent.Identity,
				parent.Spell.Id, mark!.SavedRoomId, mark.RemainingUses);
		});
		Rooms[southRow.Id] = origin.Id; Rooms[northRow.Id] = origin.Id;
		foreach (var exit in life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.Exit)) _exits?.Add(exit.Id);
		try
		{
			mark!.AcceptCommittedMarkedRoomUse(mark.RemainingUses);
			var south = new Room(southRow, source.OwningZone);
			var north = new Room(northRow, destination.OwningZone);
			_world.Add(south); _world.Add(north);
			_world.ExitManager.InitialiseRoom(south, south.CurrentOverlay);
			_world.ExitManager.InitialiseRoom(north, north.CurrentOverlay);
			ReloadExternal(source, life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.Exit)
				.Single(x => _world.ExitManager.GetExitByID(x.Id).Rooms.Any(r => r.Id == source.Id)).Id);
			ReloadExternal(destination, life.Entities.Where(x => x.Kind == SpellOwnedEntityKind.Exit)
				.Single(x => _world.ExitManager.GetExitByID(x.Id).Rooms.Any(r => r.Id == destination.Id)).Id);
			return (south, north);
		}
		catch (Exception ex)
		{
			_store.Hold(origin.Id, life.Version, "Committed Nilaz gate activation needs recovery: " + ex.Message,
				RuntimeClock.UtcNow < life.UpdatedUtc ? life.UpdatedUtc : RuntimeClock.UtcNow);
			throw;
		}
	}

	private void ReloadExternal(IRoom endpoint, long exitId)
	{
		var exit = _world.ExitManager.GetExitByID(exitId);
		if (!endpoint.CurrentOverlay.ExitIDs.Contains(exitId)) ((IEditableRoomOverlay)endpoint.CurrentOverlay).AddExit(exit);
		_world.ExitManager.UpdateRoomOverlayExits(endpoint, endpoint.CurrentOverlay);
	}
}
