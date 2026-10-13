#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Magic.Lifecycle;

/// <summary>One explicitly claimed native room, overlay and entrance per cast. Occupants are borrowed.</summary>
public sealed partial class SpellOwnedShelterService : ISpellOwnedShelterService
{
	private readonly IFuturemud _world;
	private readonly Func<ICharacter, IRoom, SpellShelterConfiguration, int, string?> _creationAdmission;
	private readonly SpellOwnedLifecycleStore _store = new();
	private Dictionary<long, Guid>? _rooms;
	private HashSet<long>? _exits;
	private long _cursor;

	public SpellOwnedShelterService(IFuturemud world) : this(world, null) { }

	internal SpellOwnedShelterService(IFuturemud world, Func<ICharacter, IRoom, SpellShelterConfiguration, int, string?>? creationAdmission)
	{
		_world = world;
		_creationAdmission = creationAdmission ?? AdmissionError;
	}

	private Dictionary<long, Guid> Rooms
	{
		get
		{
			if (_rooms is not null) return _rooms;
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			_rooms = FMDB.Context.MagicSpellOwnedEntities.AsNoTracking()
				.Where(x => x.Kind == (int)SpellOwnedEntityKind.Room && x.Lifecycle.Family == SpellShelterAnchor.Family &&
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
				.Where(x => x.Kind == (int)SpellOwnedEntityKind.Exit && x.Lifecycle.Family == SpellShelterAnchor.Family &&
					x.Lifecycle.State != (int)SpellLifecycleState.Completed)
				.Select(x => x.EntityId).ToHashSet();
		}
		return _exits.Contains(exitId);
	}

	public string? AdmissionError(ICharacter caster, IRoom anchor, SpellShelterConfiguration c, int grade)
	{
		if ((c.Kind == SpellShelterKind.SeveringRefuge) != (c.Ward is not null))
			return "Only Severing Refuge requires an explicitly authored ward.";
		if (c.Ward is { } ward)
		{
			try { ward.Validate(); } catch (ArgumentException ex) { return ex.Message; }
			if (ward.SchoolIds.Any(x => _world.MagicSchools.Get(x) is null)) return "A selected ward school is unavailable.";
		}
		if (!Enum.IsDefined(c.Kind) || grade is < 1 or > 7 || c.MaximumOccupants is < 1 or > 128 ||
			c.CapacityForGrade(grade) is < 1 or > 128 ||
			!double.IsFinite(c.SecondsPerGrade) || c.SecondsPerGrade <= 0 ||
			c.SecondsPerGrade * grade > (DateTime.MaxValue - RuntimeClock.UtcNow).TotalSeconds ||
			TimeSpan.FromSeconds(c.SecondsPerGrade * grade) <= TimeSpan.Zero)
			return "Configure a finite positive shelter lifetime and capacity from 1 to 128.";
		if (!ReferenceEquals(caster.Gameworld, _world) || !ReferenceEquals(anchor.Gameworld, _world) ||
			!ReferenceEquals(caster.Location, anchor) || anchor.Temporary || anchor is Room { HostedVehicleId: not null } ||
			caster.RoomLayer != RoomLayer.GroundLevel || !CharacterState.Able.HasFlag(caster.State) ||
			caster.State.HasFlag(CharacterState.Stasis) || !_world.Actors.Any(x => ReferenceEquals(x, caster)))
			return "Create a shelter from a stable, permanent ground-level room in this world.";
		if (c.UndergroundDepth is < 1 or > 128 || c.Kind == SpellShelterKind.BurrowRefuge && anchor.StoredCoordinates.Z < int.MinValue + c.UndergroundDepth)
			return "Configure an underground depth from 1 to 128 native grid levels within the coordinate range.";
		if (c.AllowedTerrainIds.Count == 0 || c.AllowedTerrainIds.Any(x => x <= 0 || _world.Terrains.Get(x) is null) ||
			!c.AllowedTerrainIds.Contains(anchor.Terrain(caster).Id))
			return "This terrain does not admit the configured shelter.";
		if (ResolveTemplateOverlay(anchor, c, out _, out _) is { } templateError) return templateError;
		if (_world.DefaultHooks.Any(x => x.PerceivableType.EqualTo("Room") || x.PerceivableType.EqualTo("Cell")))
			return "Shelter creation needs an adapter for configured default room hooks.";
		if (!TryDestination(c.FallbackRoomId, null, out _)) return "Bind a valid permanent ground-level fallback room.";
		if (c.Ward is not null && new SpellShelterAnchor(c.Kind, anchor.Id, c.FallbackRoomId, caster.RoutePositionMetres,
			c.CapacityForGrade(grade), anchor.CurrentOverlay.Id, c.Ward).Save().Length > 2048)
			return "The authored ward exceeds the native lifecycle provenance limit; reduce its selectors.";
		if (c.Kind != SpellShelterKind.SpringHaven)
			return c.WaterPrototypeId != 0 || c.LiquidId != 0 || c.LitresPerGrade != 0 ? "Only Spring Haven creates a finite water supply." : null;
		return WaterAdmissionError(c, grade);
	}

	public IRoom Create(ICharacter caster, IRoom anchor, SpellShelterConfiguration c, SpellLifecycleOrigin origin) =>
		CreateCore(caster, anchor, c, origin, null);

	public IRoom CreatePrepared(ICharacter caster, IRoom anchor, SpellShelterConfiguration c,
		SpellLifecycleOrigin origin, Func<bool> inputsCurrent)
	{
		ArgumentNullException.ThrowIfNull(inputsCurrent);
		return CreateCore(caster, anchor, c, origin, inputsCurrent);
	}

	private IRoom CreateCore(ICharacter caster, IRoom anchor, SpellShelterConfiguration c,
		SpellLifecycleOrigin origin, Func<bool>? inputsCurrent)
	{
		void ConfirmFrame()
		{
			if (inputsCurrent?.Invoke() == false)
				throw new InvalidOperationException("The prepared shelter physical frame changed during native creation.");
		}
		ConfirmFrame();
		if (_creationAdmission(caster, anchor, c, origin.Grade) is { } error) throw new InvalidOperationException(error);
		ConfirmFrame();
		var metadata = new SpellShelterAnchor(c.Kind, anchor.Id, c.FallbackRoomId, caster.RoutePositionMetres, c.CapacityForGrade(origin.Grade), anchor.CurrentOverlay.Id, c.Ward);
		if (origin.Family != SpellShelterAnchor.Family || origin.Mode != SpellLifecycleMode.TemporaryCleanup ||
			origin.CreatorId != CharacterInstanceIdentityComparer.IdentityId(caster) || origin.Provenance != metadata.Save())
			throw new ArgumentException("Shelter creation requires its exact typed origin and absolute deadline.");
		if (ResolveTemplateOverlay(anchor, c, out var resolvedOverlay, out var foragableProfileId) is { } templateError)
			throw new InvalidOperationException(templateError);
		var row = new Models.Room { ZoneId = anchor.OwningZone.Id, Temporary = true, EffectData = "<Effects />",
			ForagableProfileId = foragableProfileId, X = anchor.StoredCoordinates.X, Y = anchor.StoredCoordinates.Y,
			Z = c.Kind == SpellShelterKind.BurrowRefuge ? anchor.StoredCoordinates.Z - c.UndergroundDepth : anchor.StoredCoordinates.Z };
		// The durable row loads its native ward before world registration or entrance exposure.
		if (c.Ward is { } wardConfiguration)
			row.EffectData = new XElement("Effects", SpellShelterWard.Envelope(origin.Id, origin.SpellId, wardConfiguration)).ToString();
		var overlay = resolvedOverlay!;
		overlay.Room = row;
		GameItem? supply = c.Kind == SpellShelterKind.SpringHaven ? PrepareWater(c, caster, origin.Grade) : null;
		Models.GameItem? supplyRow = null;
		var components = new List<(GameItemComponent Component, Models.GameItemComponent Row)>();
		ConfirmFrame();
		var life = _store.Create(origin, creation =>
		{
			ConfirmFrame();
			creation.StageShelterTopology(row, overlay, anchor.Id, anchor.CurrentOverlay.Id, Keyword(c.Kind));
			if (supply is null) return;
			supplyRow = (Models.GameItem)supply.DatabaseInsert();
			foreach (var component in supply.Components.Cast<GameItemComponent>())
				components.Add((component, (Models.GameItemComponent)component.DatabaseInsert()));
			creation.Claim(SpellOwnedEntityKind.GameItem, supplyRow);
			creation.Context.RoomsGameItems.Add(new Models.RoomsGameItems { Room = row, GameItem = supplyRow });
		});
		Rooms[row.Id] = origin.Id;
		_exits?.Add(life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Exit).Id);
		try
		{
			var room = new Room(row, anchor.OwningZone);
			RequireWardAuthority(room, origin, c.Ward, true);
			_world.Add(room);
			_world.ExitManager.InitialiseRoom(room, room.CurrentOverlay);
			ReloadCommittedOverlayExit(anchor, life.Entities.Single(x => x.Kind == SpellOwnedEntityKind.Exit).Id);
			if (supply is not null)
			{
				(_world.SpellOwnedItems as SpellOwnedItemService)?.RegisterClaimedItem(supplyRow!.Id);
				supply.ActivateCommittedSpellItem(supplyRow!, origin, components);
				_world.Add(supply); room.Insert(supply, true); supply.Login();
			}
			return room;
		}
		catch (Exception ex)
		{
			Hold(life, "Committed shelter activation needs recovery; never replay creation: " + ex.Message);
			throw;
		}
	}

	/// <summary>Resolve a typed presentation into unsaved topology rows; allocate no ordinary room.</summary>
	internal string? ResolveTemplateOverlay(IRoom anchor, SpellShelterConfiguration c,
		out Models.RoomOverlay? overlay, out long? foragableProfileId)
	{
		overlay = null; foragableProfileId = null;
		if (c.TemplateRoomId < 0 || c.AutobuilderTemplateId < 0 ||
			(c.TemplateRoomId > 0) == (c.AutobuilderTemplateId > 0))
			return "Bind exactly one indoor Room template or deterministic autobuilder template.";
		if (c.AutobuilderTemplateId > 0)
		{
			if (_world.AutobuilderRooms.Get(c.AutobuilderTemplateId) is not IAutobuilderRoomPresentation source ||
				!ReferenceEquals(source.Gameworld, _world))
				return "Bind a deterministic native autobuilder presentation in this world.";
			AutobuilderRoomPresentation presentation;
			try { presentation = source.ResolvePresentation(); }
			catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or NullReferenceException)
			{ return "The deterministic autobuilder presentation has an unavailable native binding."; }
			var terrain = presentation.Terrain;
			var profile = presentation.ForagableProfile;
			if (terrain is null || terrain.Id <= 0 || !ReferenceEquals(_world.Terrains.Get(terrain.Id), terrain) ||
				!terrain.TerrainLayers.Contains(RoomLayer.GroundLevel) ||
				presentation.OutdoorsType is not (RoomOutdoorsType.Indoors or RoomOutdoorsType.IndoorsNoLight) ||
				!double.IsFinite(presentation.AmbientLightFactor) || presentation.AmbientLightFactor < 0 ||
				terrain.Atmosphere is not IGas gas || gas.Id <= 0 || !ReferenceEquals(_world.Gases.Get(gas.Id), gas) ||
				anchor.CurrentOverlay.Package.Status != RevisionStatus.Current ||
				profile is not null && (profile.Id <= 0 || profile.Status != RevisionStatus.Current ||
					!ReferenceEquals(_world.ForagableProfiles.Get(profile.Id), profile)))
				return "The autobuilder needs an indoor ground-level presentation, current native terrain/gas/forage and finite light.";
			foragableProfileId = profile?.Id;
			overlay = new Models.RoomOverlay
			{
				Name = presentation.RoomName, RoomName = presentation.RoomName, RoomDescription = presentation.RoomDescription,
				RoomOverlayPackageId = anchor.CurrentOverlay.Package.Id,
				RoomOverlayPackageRevisionNumber = anchor.CurrentOverlay.Package.RevisionNumber,
				TerrainId = terrain.Id, OutdoorsType = (int)presentation.OutdoorsType,
				AmbientLightFactor = presentation.AmbientLightFactor, AddedLight = 0,
				AtmosphereId = gas.Id, AtmosphereType = "gas", SafeQuit = true
			};
			return null;
		}
		var template = _world.Rooms.Get(c.TemplateRoomId);
		if (template is null || template.Temporary || template.RouteDefinition is not null || template is Room { HostedVehicleId: not null } ||
			template.CurrentOverlay.Package.Status != RevisionStatus.Current || template.Hooks.Any() || template.Effects.Any() ||
			template.CurrentOverlay.OutdoorsType is not (RoomOutdoorsType.Indoors or RoomOutdoorsType.IndoorsNoLight) ||
			!template.CurrentOverlay.Terrain.TerrainLayers.Contains(RoomLayer.GroundLevel))
			return "Bind an approved ordinary indoor room template without effects, hooks or route/vehicle topology.";
		var existing = template.CurrentOverlay;
		overlay = new Models.RoomOverlay
		{
			Name = existing.Name, RoomName = existing.RoomName, RoomDescription = existing.RoomDescription,
			RoomOverlayPackageId = existing.Package.Id, RoomOverlayPackageRevisionNumber = existing.Package.RevisionNumber,
			TerrainId = existing.Terrain.Id, OutdoorsType = (int)existing.OutdoorsType, HearingProfileId = existing.HearingProfile?.Id,
			AmbientLightFactor = existing.AmbientLightFactor, AddedLight = existing.AddedLight,
			AtmosphereId = existing.Atmosphere?.Id, AtmosphereType = existing.Atmosphere is ILiquid ? "liquid" : "gas", SafeQuit = true
		};
		return null;
	}

	internal static string Keyword(SpellShelterKind kind) => kind switch
	{
		SpellShelterKind.SpringHaven => "haven", SpellShelterKind.BurrowRefuge => "burrow", SpellShelterKind.SandShelter => "shelter",
		SpellShelterKind.SeveringRefuge => "refuge",
		_ => throw new ArgumentOutOfRangeException(nameof(kind))
	};

	private void ReloadCommittedOverlayExit(IRoom anchor, long exitId)
	{
		var exit = _world.ExitManager.GetExitByID(exitId);
		if (!anchor.CurrentOverlay.ExitIDs.Contains(exitId)) ((IEditableRoomOverlay)anchor.CurrentOverlay).AddExit(exit);
		_world.ExitManager.UpdateRoomOverlayExits(anchor, anchor.CurrentOverlay);
	}

	public bool CanEnter(IRoom room, IPerceiver entrant) => CanEnterCore(room, entrant, false);
	public bool CanReconnect(IRoom room, ICharacter resident) => CanEnterCore(room, resident, true);

	private bool CanEnterCore(IRoom room, IPerceiver entrant, bool nativeLogin)
	{
		if (!Rooms.TryGetValue(room.Id, out var id)) return true;
		try
		{
			var life = _store.Find(id);
			if (life is null) return false;
			var c = SpellShelterAnchor.Load(life.Origin.Provenance);
			RequireWardAuthority(room, life.Origin, c.Ward, true);
			using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
			var persisted = FMDB.Context.CharacterInstances.AsNoTracking().Where(x => x.LocationId == room.Id)
				.Select(x => new { x.Id, x.CharacterId }).ToArray();
			var legacyIds = FMDB.Context.Characters.AsNoTracking().Where(x => x.Location == room.Id &&
				!FMDB.Context.CharacterInstances.Any(i => i.CharacterId == x.Id && i.IsPrimary)).Select(x => x.Id).ToArray();
			ICharacter? Loaded(long id) => _world.Actors.Get(id) ?? _world.CachedActors.Get(id);
			var (instances, legacy) = ResidentOccupancy(room, persisted.Select(x => (x.Id, x.CharacterId)), legacyIds, Loaded);
			var resident = entrant is ICharacter character && IsResidentEntry(room, character, instances, nativeLogin);
			if (resident) return true;
			if (life.State != SpellLifecycleState.Active || life.Origin.DeadlineUtc <= RuntimeClock.UtcNow) return false;
			// Count other occupants: a prospective entrant's stale own row cannot consume a second place.
			if (entrant is ICharacter { InstanceId: > 0 } entering) instances.Remove(entering.InstanceId);
			return instances.Count + legacy < c.MaximumOccupants;
		}
		catch { return false; }
	}

	internal static bool IsResidentEntry(IRoom room, ICharacter actor, IReadOnlySet<long> instances, bool nativeLogin) =>
		ReferenceEquals(actor.Location, room) &&
		(room.Characters.Any(x => ReferenceEquals(x, actor)) ||
		 nativeLogin && actor.InstanceId > 0 && instances.Contains(actor.InstanceId));

	internal static (HashSet<long> Instances, int LegacyCount) ResidentOccupancy(IRoom room,
		IEnumerable<(long InstanceId, long CharacterId)> persisted, IEnumerable<long> legacyIds, Func<long, ICharacter?> loaded)
	{
		var instances = new HashSet<long>();
		foreach (var link in persisted)
		{
			var frame = loaded(link.CharacterId)?.Identity.Instances.OfType<ICharacter>().SingleOrDefault(x => x.InstanceId == link.InstanceId);
			// An exact loaded frame owns its current position even before the next native save.
			// Unknown offline instances retain their declared persisted location and are never materialised here.
			if (frame is null || ReferenceEquals(frame.Location, room)) instances.Add(link.InstanceId);
		}
		var anonymous = new HashSet<ICharacter>(ReferenceEqualityComparer.Instance);
		foreach (var actor in room.Characters.Where(x => ReferenceEquals(x.Location, room)))
		{
			if (actor.InstanceId > 0) instances.Add(actor.InstanceId);
			else anonymous.Add(actor);
		}
		var legacy = legacyIds.Count(id => loaded(id) is not { } actor ||
			ReferenceEquals(actor.Location, room) && !instances.Contains(actor.InstanceId) && !anonymous.Contains(actor));
		return (instances, legacy + anonymous.Count);
	}

	public bool ReturnRejectedEntrant(IRoom room, ICharacter entrant)
	{
		if (!Rooms.TryGetValue(room.Id, out var id)) return false;
		var life = _store.Find(id)!; var metadata = SpellShelterAnchor.Load(life.Origin.Provenance);
		if (TryDestination(metadata.AnchorRoomId, metadata.AnchorRoutePosition, out var destination) ||
			TryDestination(metadata.FallbackRoomId, null, out destination))
		{
			Evacuate(entrant, room, destination); return true;
		}
		if (life.State == SpellLifecycleState.Active) life = _store.BeginRetirement(id, life.Version, SpellRetirementReason.Dismissal, TransitionTime(life));
		Hold(life, "An entry already changed physical location and neither recorded return point is safe; retain the occupant for recovery.");
		return false;
	}

	public bool RequestRetirement(long roomId, SpellRetirementReason reason)
	{
		if (!Rooms.TryGetValue(roomId, out var id)) return false;
		var life = _store.Find(id)!;
		if (life.State == SpellLifecycleState.Active) life = _store.BeginRetirement(id, life.Version, reason, TransitionTime(life));
		try { Retire(life); }
		catch (Exception ex) { Hold(_store.Find(id)!, "Shelter retirement held: " + ex.Message); }
		return true;
	}

	public int ReconcileRetirements(DateTime nowUtc, int limit = 100)
	{
		if (nowUtc.Kind != DateTimeKind.Utc || limit is < 1 or > 1000) throw new ArgumentException("Invalid shelter reconciliation bound or clock.");
		var candidates = Rooms.Keys.Where(x => x > _cursor).Order().Take(limit).ToArray();
		if (candidates.Length == 0) { _cursor = 0; return 0; }
		foreach (var roomId in candidates)
		{
			_cursor = roomId;
			var life = _store.Find(Rooms[roomId]);
			if (life is null || life.State == SpellLifecycleState.Completed) { Rooms.Remove(roomId); continue; }
			try
			{
				if (life.State == SpellLifecycleState.Active && life.Origin.DeadlineUtc > nowUtc)
				{
					var metadata = SpellShelterAnchor.Load(life.Origin.Provenance);
					if (metadata.Ward is not null)
						RequireWardAuthority(_world.Rooms.Get(roomId) ?? throw new InvalidOperationException("The warded shelter is unavailable."),
							life.Origin, metadata.Ward, true);
					continue;
				}
				if (life.State == SpellLifecycleState.Active)
					life = _store.BeginRetirement(life.Origin.Id, life.Version, SpellRetirementReason.Expiry, nowUtc < life.UpdatedUtc ? life.UpdatedUtc : nowUtc);
				Retire(life);
			}
			catch (Exception ex) { Hold(_store.Find(life.Origin.Id)!, "Shelter retirement held: " + ex.Message); }
		}
		return candidates.Length;
	}

	private bool TryDestination(long roomId, double? position, out SpatialLocation destination)
	{
		var room = _world.Rooms.Get(roomId);
		destination = default;
		if (room is null || room.Temporary || room is Room { HostedVehicleId: not null } ||
			!room.Terrain(null).TerrainLayers.Contains(RoomLayer.GroundLevel) || room.IsSwimmingLayer()) return false;
		using var isolated = FMDB.BeginIndependentScope(); using var db = new FMDB();
		if (!FMDB.Context.Rooms.Any(x => x.Id == roomId && !x.Temporary)) return false;
		if (room.RouteDefinition is null) position = null;
		else position ??= room.RouteDefinition.DefaultPositionMetres;
		destination = new SpatialLocation(room, RoomLayer.GroundLevel, position);
		return RouteSpatialService.Instance.TryValidateLocation(destination, out _);
	}

	private void Hold(SpellOwnedLifecycle life, string message)
	{
		if (life.State == SpellLifecycleState.Completed) return;
		_store.Hold(life.Origin.Id, life.Version, message[..Math.Min(message.Length, 2048)], TransitionTime(life));
	}
	private static DateTime TransitionTime(SpellOwnedLifecycle life) => RuntimeClock.UtcNow < life.UpdatedUtc ? life.UpdatedUtc : RuntimeClock.UtcNow;
}
