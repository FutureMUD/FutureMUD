using Dapper;
using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Community.Boards;
using MudSharp.Construction.Boundary;
using MudSharp.Construction.Grids;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Agriculture;
using MudSharp.Vehicles;
using MudSharp.Work.Foraging;
using MudSharp.Work.Projects;
using Parlot;
using System.Text.RegularExpressions;
using Track = MudSharp.Models.Track;

namespace MudSharp.Construction;

public partial class Room : Location, IDisposable, IRoom, IRecoverableSaveFailure, ICustodyRollbackLocation
{
	private readonly bool _isCombatSimulationRoom;
	private readonly long _combatSimulationDatabaseLocationId;

	internal long DatabaseLocationId => _isCombatSimulationRoom
		? _combatSimulationDatabaseLocationId
		: Id;
    private readonly List<IRangedCover> _localCover = new();
	private RouteRoomDefinition _routeDefinition;
	private long? _hostedVehicleId;
	private long? _hostedVehicleCompartmentId;

    private bool _contentsChanged;

    protected List<IEditableRoomOverlay> _overlays = new();

    private bool _yieldsChanged;
	private RoomSaveAttemptState? _recoverableSaveAttempt;

	private readonly record struct RoomSaveAttemptState(
		bool ContentsChanged,
		bool ResourcesChanged,
		bool YieldsChanged,
		bool TagsChanged,
		bool EffectsChanged,
		bool SurfaceLiquidChanged,
		bool HooksChanged,
		bool EnvironmentStateChanged,
		long? ExpectedEnvironmentDatabaseRevision);

    public Room(IRoomOverlayPackage package, IZone zone, bool temporary = false) : base(zone.Gameworld)
    {
        _owningZone = zone ?? throw new ArgumentNullException(nameof(zone));
        using (new FMDB())
        {
            Models.Room dbRoom = new()
            {
                ZoneId = zone.Id,
                Temporary = temporary,
                EffectData = SaveEffects().ToString()
            };
            FMDB.Context.Rooms.Add(dbRoom);
            FMDB.Context.SaveChanges();
            _id = dbRoom.Id;
            RoomOverlay newOverlay = new(this, package);
            dbRoom.CurrentOverlayId = newOverlay.Id;
            FMDB.Context.SaveChanges();
            SetupRoom(dbRoom);

            List<IHook> hooks = Gameworld.DefaultHooks.Where(x => x.Applies(this, "Room")).Select(x => x.Hook).ToList();
            if (hooks.Any())
            {
                FMDB.Context.HooksPerceivables.AddRange(hooks.Select(hook =>
                    new Models.HooksPerceivable { RoomId = Id, HookId = hook.Id }));
                FMDB.Context.SaveChanges();

                foreach (IHook hook in hooks)
                {
                    InstallHook(hook);
                }
            }
        }

        Gameworld.Add(this);
    }

    public Room(IRoomOverlayPackage package, IZone zone, IRoom templateRoom, bool temporary = false) : base(
        zone.Gameworld)
    {
        _owningZone = zone ?? throw new ArgumentNullException(nameof(zone));
        using (new FMDB())
        {
            Models.Room dbRoom = new()
            {
                ZoneId = zone.Id,
                Temporary = temporary,
                EffectData = SaveEffects().ToString()
            };
            FMDB.Context.Rooms.Add(dbRoom);
            FMDB.Context.SaveChanges();
            _id = dbRoom.Id;
            RoomOverlay newOverlay = new(this, package, templateRoom);
            dbRoom.CurrentOverlayId = newOverlay.Id;
            FMDB.Context.SaveChanges();
            SetupRoom(dbRoom);

            List<IHook> hooks = Gameworld.DefaultHooks.Where(x => x.Applies(this, "Room")).Select(x => x.Hook).ToList();
            if (hooks.Any())
            {
                FMDB.Context.HooksPerceivables.AddRange(hooks.Select(hook =>
                    new Models.HooksPerceivable { RoomId = Id, HookId = hook.Id }));
                FMDB.Context.SaveChanges();

                foreach (IHook hook in hooks)
                {
                    InstallHook(hook);
                }
            }
        }

        Gameworld.Add(this);
    }

    public Room(MudSharp.Models.Room room, IZone zone) : base(zone.Gameworld)
    {
        _owningZone = zone ?? throw new ArgumentNullException(nameof(zone));
        SetupRoom(room);
    }

	/// <summary>
	/// Creates a transient room which borrows environmental data from a real room but owns its
	/// contents independently. It is never registered with its owning zone or written to the database.
	/// </summary>
	internal Room(IRoom combatSimulationTemplate, long temporaryId) : base(combatSimulationTemplate.Gameworld)
	{
		_isCombatSimulationRoom = true;
		_combatSimulationDatabaseLocationId = combatSimulationTemplate.Id;
		_noSave = true;
		_id = temporaryId;
		_owningZone = combatSimulationTemplate.OwningZone;
		_storedCoordinates = combatSimulationTemplate.StoredCoordinates;
		_areas.AddRange(combatSimulationTemplate.OwningAreas);
		CurrentOverlay = combatSimulationTemplate.CurrentOverlay;
		_overlays = combatSimulationTemplate.Overlays
			.OfType<IEditableRoomOverlay>()
			.ToList();
		Movements = [];
		Temporary = true;
		_routeDefinition = combatSimulationTemplate.RouteDefinition is null
			? null
			: new RouteRoomDefinition(this, combatSimulationTemplate.RouteDefinition);
		_localCover.AddRange(combatSimulationTemplate.LocalCover);
	}

    public bool ContentsChanged
    {
        get => _contentsChanged;
        set
        {
            if (_noSave && value)
            {
                return;
            }

            if (value)
            {
                Changed = true;
            }

            _contentsChanged = value;
        }
    }

    public bool YieldsChanged
    {
        get => _yieldsChanged;
        set
        {
            if (_noSave && value)
            {
                return;
            }

            if (value)
            {
                Changed = true;
            }

            _yieldsChanged = value;
        }
    }

	private IVehicle HostedVehicle => _hostedVehicleId is null
		? null
		: Gameworld.Vehicles.Get(_hostedVehicleId.Value);

	private IRoom HostedExteriorContext => HostedVehicle?.Location is { } exterior && exterior.Id != Id
		? exterior
		: null;

	public int? X => HostedExteriorContext?.X ?? _storedCoordinates.X;

	public int? Y => HostedExteriorContext?.Y ?? _storedCoordinates.Y;

	public int? Z => HostedExteriorContext?.Z ?? _storedCoordinates.Z;

    /// <summary>
    /// If a room is temporary, it may disappear at any time.
    /// </summary>
    public bool Temporary { get; set; }

	public RoomSpatialType SpatialType => _routeDefinition is null
		? RoomSpatialType.Ordinary
		: RoomSpatialType.LinearRoute;

	public IRouteRoomDefinition RouteDefinition => _routeDefinition;

	/// <summary>
	/// The persistent room-scale vehicle that owns this hosted interior room, if any.
	/// Hosted rooms remain ordinary rooms; this metadata prevents their identity from
	/// being inferred from the vehicle's current exterior location.
	/// </summary>
	public long? HostedVehicleId => _hostedVehicleId;

	/// <summary>
	/// The live vehicle compartment row that owns this hosted interior room, if any.
	/// </summary>
	public long? HostedVehicleCompartmentId => _hostedVehicleCompartmentId;

	public void SetHostedVehicle(long? vehicleId, long? compartmentId)
	{
		_hostedVehicleId = vehicleId;
		_hostedVehicleCompartmentId = compartmentId;
	}

	/// <summary>
	/// Replaces the immutable runtime RouteRoom snapshot after a persisted builder mutation.
	/// </summary>
	public void ReloadRouteDefinition(Models.RouteRoom routeRoom)
	{
		_routeDefinition = routeRoom is null ? null : new RouteRoomDefinition(this, routeRoom);
		if (_routeDefinition is not null)
		{
			RouteSpatialConfiguration.FromGameworld(Gameworld).Validate();
		}

		foreach (var perceivable in Perceivables.ToList())
		{
			RouteSpatialService.Instance.TrackPerceivable(perceivable);
		}
	}

    #region Overrides of Location

    /// <inheritdoc />
    public override IEnumerable<IRoom> Rooms => [this];

    #endregion

    protected List<IMovement> Movements { get; set; }
    public override string FrameworkItemType => "Room";

    public override IRoom Location
    {
        get => this;

        protected set { }
    }

    public bool IsSwimmingLayer(RoomLayer referenceLayer = RoomLayer.GroundLevel)
    {
        // TODO - wading pools
        // TODO - room flooding effects
        return !referenceLayer.IsHigherThan(RoomLayer.GroundLevel) &&
               Terrain(null).TerrainLayers.Any(x => x.IsUnderwater());
    }

    public bool IsUnderwaterLayer(RoomLayer referenceLayer)
    {
        // TODO - room flooding effects
        return referenceLayer.IsLowerThan(RoomLayer.GroundLevel) &&
               Terrain(null).TerrainLayers.Any(x => x.IsUnderwater());
    }

	internal RoomLayer PrepareCurrencyInsertionLayer(IGameItem item) => HandleEnterLayers(item);

	internal void SetPreparedCurrencyRoomMembership(IGameItem item, bool present)
	{
		SetPreparedCurrencyMembership(item, present);
		if (!_isCombatSimulationRoom)
			foreach (var location in new[] { OwningZone as Location, OwningZone?.Shard as Location }.OfType<Location>().Distinct())
				location.SetPreparedCurrencyMembership(item, present);
	}

	internal void FinishPreparedCurrencyInsertion(IGameItem item, SpatialLocation point)
	{
		bool StillPresent() => !item.Deleted && !item.Destroyed && item.ContainedIn is null && item.InInventoryOf is null &&
			ReferenceEquals(ComponentItemTransfer.DirectLocationOf(item), this) && item.RoomLayer == point.Layer &&
			(RouteDefinition is null || item.RoutePositionMetres == point.RoutePositionMetres) && _gameItems.Any(x => ReferenceEquals(x, item));
		if (!StillPresent()) return;
		_gameItems = _gameItems.OrderBy(x => !x.HighPriority).ToList();
		if (!StillPresent()) return;
		var swimming = IsSwimmingLayer(point.Layer);
		if (!StillPresent()) return;
		if (swimming) item.PositionState = PositionFloatingInWater.Instance;
		else if (ZeroGravityMovementHelper.IsZeroGravity(this, point.Layer) && StillPresent()) ZeroGravityMovementHelper.EnsureFloating(item);
		if (!StillPresent()) return;
		CheckFallExitStatus();
		if (!StillPresent()) return;
		new MagicPortalTopologyService().RebuildNetworksForItem(Gameworld, item);
	}

    public override void Insert(IGameItem thing, bool newStack)
    {
		if (thing is not null) ForeignCustodyTransferContext.EnsureRoom(this, thing);
        if (thing == null || _gameItems.Contains(thing))
        {
#if DEBUG
            if (_gameItems.Contains(thing))
            {
                throw new ApplicationException("Item duplication in Room.");
            }
#endif
            return;
        }

        var originalBody = thing.InInventoryOf;
        var originalContainer = thing.ContainedIn;
        var originalRoom = ComponentItemTransfer.DirectLocationOf(thing);
        var originalBelt = thing.GetItemType<MudSharp.GameItems.Interfaces.IBeltable>()?.ConnectedTo;
        var originalLayer = thing.RoomLayer;
        var originalRoutePosition = thing.RoutePositionMetres;
        bool SameCustody(IGameItem expectedContainer, IBody expectedBody, IRoom expectedRoom) =>
            !thing.Deleted && !thing.Destroyed &&
            ReferenceEquals(thing.ContainedIn, expectedContainer) &&
            ReferenceEquals(thing.InInventoryOf, expectedBody) &&
            ReferenceEquals(ComponentItemTransfer.DirectLocationOf(thing), expectedRoom) &&
            ReferenceEquals(thing.GetItemType<MudSharp.GameItems.Interfaces.IBeltable>()?.ConnectedTo, originalBelt);
        bool BeforeMove() => SameCustody(originalContainer, originalBody, originalRoom) &&
                             thing.RoomLayer == originalLayer && thing.RoutePositionMetres == originalRoutePosition;

        RoomLayer newLayer = HandleEnterLayers(thing);
        if (!BeforeMove()) return;
		var explicitlyAssignedPosition = _routeDefinition is not null && ReferenceEquals(thing.Location, this)
			? thing.RoutePositionMetres
			: null;
		var inheritedPosition = _routeDefinition is null
			? null
			: RouteSpatialService.Instance.GetInheritedRoutePosition(
				thing,
				thing.InInventoryOf ?? (ILocateable)thing.ContainedIn);
		double? insertionPosition = _routeDefinition is null
			? null
			: inheritedPosition ?? explicitlyAssignedPosition ?? _routeDefinition.DefaultPositionMetres;
        if (!BeforeMove()) return;
        var intendedPoint = new SpatialLocation(this, newLayer, insertionPosition);
        bool StillAtIntendedPoint(IGameItem expectedContainer, IBody expectedBody) =>
            SameCustody(expectedContainer, expectedBody, this) &&
            thing.RoomLayer == intendedPoint.Layer &&
            (_routeDefinition is null || thing.RoutePositionMetres == intendedPoint.RoutePositionMetres);
        thing.MoveTo(intendedPoint);
        if (!StillAtIntendedPoint(originalContainer, originalBody)) return;

        if (!newStack)
        {
            IGameItem mergeTarget = null;
            foreach (var candidate in LayerGameItems(newLayer).ToArray())
            {
                if (!StillAtIntendedPoint(originalContainer, originalBody)) return;
                var nearby = _routeDefinition is null ||
                             RouteSpatialService.Instance.GetProximity(thing, candidate) <= Proximity.Immediate;
                if (!StillAtIntendedPoint(originalContainer, originalBody)) return;
                if (!nearby || candidate.Deleted || candidate.Destroyed) continue;
                var canMerge = thing.CanMerge(candidate);
                if (!StillAtIntendedPoint(originalContainer, originalBody)) return;
                if (canMerge && !candidate.Deleted && !candidate.Destroyed &&
                    _gameItems.Contains(candidate) && candidate.RoomLayer == newLayer &&
                    ReferenceEquals(ComponentItemTransfer.DirectLocationOf(candidate), this))
                {
                    mergeTarget = candidate;
                    break;
                }
            }
            if (mergeTarget != null)
            {
                if (!StillAtIntendedPoint(originalContainer, originalBody)) return;
                mergeTarget.Merge(thing);
                if (!StillAtIntendedPoint(originalContainer, originalBody)) return;
                new MagicPortalTopologyService().RebuildNetworksForItem(Gameworld, thing);
                if (!StillAtIntendedPoint(originalContainer, originalBody)) return;
				RouteSpatialService.Instance.UntrackPerceivable(thing);
                thing.Delete();
                return;
            }
        }


        if (!StillAtIntendedPoint(originalContainer, originalBody)) return;
        thing.ContainedIn = null;
        // Clearing containment may publish proximity/environment callbacks. A new holder or
        // container owns the item; do not overwrite that custody with enclosing room membership.
        var clearedBody = originalContainer is null ? originalBody : null;
        if (!StillAtIntendedPoint(null, clearedBody)) return;
        if (_gameItems.Contains(thing)) return; // Reentrant insertion already completed membership.
        base.Insert(thing, newStack);
		if (!_isCombatSimulationRoom)
		{
			OwningZone.Insert(thing, newStack);
		}
        _gameItems = _gameItems.OrderBy(x => !x.HighPriority).ToList();
        if (!StillAtIntendedPoint(null, clearedBody)) return;
        var swimming = IsSwimmingLayer(newLayer);
        if (!StillAtIntendedPoint(null, clearedBody)) return;
        if (swimming)
        {
            thing.PositionState = PositionFloatingInWater.Instance;
        }
        else
        {
            var zeroGravity = ZeroGravityMovementHelper.IsZeroGravity(this, newLayer);
            if (!StillAtIntendedPoint(null, clearedBody)) return;
            if (zeroGravity)
            {
                ZeroGravityMovementHelper.EnsureFloating(thing);
            }
        }

        if (!StillAtIntendedPoint(null, clearedBody)) return;
        ContentsChanged = true;
        CheckFallExitStatus();
        if (!StillAtIntendedPoint(null, clearedBody)) return;
        new MagicPortalTopologyService().RebuildNetworksForItem(Gameworld, thing);
    }

	Action ICustodyRollbackLocation.CaptureCustodyMembershipRollback(IReadOnlyCollection<IGameItem> items)
	{
		if (OwningZone is not Location zone || OwningZone.Shard is not Location shard)
			throw new InvalidOperationException("Foreign custody requires native enclosing location rollback adapters.");
		var restores = new[] { (Location)this, zone, shard }.Distinct()
			.Select(x => x.CaptureCustodyMembershipRollback(items)).ToArray();
		return () => { foreach (var restore in restores) restore(); };
	}

    private RoomLayer HandleEnterLayers(IGameItem thing)
    {
        List<RoomLayer> localLayers = Terrain(thing).TerrainLayers.ToList();
        if (localLayers.Contains(thing.RoomLayer))
        {
            return thing.RoomLayer;
        }

        RoomLayer highest = localLayers.HighestLayer();
        RoomLayer lowest = localLayers.LowestLayer();
        if (thing.RoomLayer.IsHigherThan(highest))
        {
            return highest;
        }

        return lowest;
    }

	public IEnumerable<IRoom> Surrounds => Gameworld.ExitManager.GetExitsFor(this).Select(x => x.Destination);

    public override string Name => CurrentOverlay.RoomName;

    public override void Extract(IGameItem thing)
    {
        if (thing == null || !_gameItems.Contains(thing))
        {
            return;
        }

		ForeignCustodyTransferContext.EnsureRoom(this, thing);
        base.Extract(thing);
		RouteSpatialService.Instance.UntrackPerceivable(thing);
		if (!_isCombatSimulationRoom)
		{
			OwningZone.Extract(thing);
		}
        ContentsChanged = true;
        CheckFallExitStatus();
        new MagicPortalTopologyService().RebuildNetworksForItem(Gameworld, thing);
    }


    public IZone Zone => HostedExteriorContext?.Zone ?? OwningZone;
    public IShard Shard => Zone.Shard;

    public IEnumerable<IArea> Areas => HostedExteriorContext?.Areas ?? _areas;

    public IHearingProfile HearingProfile(IPerceiver voyeur)
    {
        return GetOverlayFor(voyeur).HearingProfile;
    }

    public int LoadItems(IEnumerable<Models.GameItem> items)
    {
        List<Tuple<Models.GameItem, IGameItem>> stagingTable = new();
        foreach (Models.GameItem item in items)
        {
            IGameItem gitem = Gameworld.TryGetItem(item, true);
            if (gitem == null)
            {
                continue;
            }

            if (gitem.InInventoryOf != null || gitem.ContainedIn != null || gitem.Location != null)
            {
                Changed = true;
                Gameworld.SystemMessage(
                    $"Duplicated Item: {gitem.HowSeen(gitem, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLoadThings)} {gitem.Id:N0}",
                    true);
                continue;
            }

            gitem.Drop(this);
			gitem.SetRoutePosition(item.RoutePosition.HasValue ? (double)item.RoutePosition.Value : null);
            stagingTable.Add(new Tuple<Models.GameItem, IGameItem>(item, gitem));
            _gameItems.Add(gitem);
            OwningZone.Insert(gitem);
        }

        foreach (Tuple<Models.GameItem, IGameItem> item in stagingTable)
        {
            item.Item2.LoadPosition(item.Item1);
            item.Item2.FinaliseLoadTimeTasks();
        }

        _gameItems = _gameItems.OrderBy(x => !x.HighPriority).ToList();
        return _gameItems.Count;
    }

    public IEnumerable<IRoomExit> ExitsFor(IPerceiver voyeur, bool ignoreLayers = false)
    {
		if (_isCombatSimulationRoom)
		{
			return Enumerable.Empty<IRoomExit>();
		}

		return Gameworld.ExitManager
			.GetExitsFor(this, GetOverlayFor(voyeur), ignoreLayers ? default : voyeur?.RoomLayer)
			.Where(x => IsSpatiallyAccessibleExit(voyeur, x));
    }

    public IRoomExit GetExit(CardinalDirection direction, IPerceiver voyeur)
    {
		if (_isCombatSimulationRoom)
		{
			return null;
		}

		var exit = Gameworld.ExitManager.GetExit(this, direction, voyeur);
		return exit is not null && IsSpatiallyAccessibleExit(voyeur, exit) ? exit : null;
    }

    public IRoomExit GetExit(string direction, string target, IPerceiver voyeur)
    {
		if (_isCombatSimulationRoom)
		{
			return null;
		}

		var exit = Gameworld.ExitManager.GetExit(this, direction, target, voyeur, GetOverlayFor(voyeur));
		return exit is not null && IsSpatiallyAccessibleExit(voyeur, exit) ? exit : null;
    }

    public IRoomExit GetExitKeyword(string direction, IPerceiver voyeur)
    {
		if (_isCombatSimulationRoom)
		{
			return null;
		}

		var exit = Gameworld.ExitManager.GetExitKeyword(this, direction, voyeur, GetOverlayFor(voyeur));
		return exit is not null && IsSpatiallyAccessibleExit(voyeur, exit) ? exit : null;
    }

    public IRoomExit GetExitTo(IRoom otherRoom, IPerceiver voyeur, bool ignoreLayers = false)
    {
		if (_isCombatSimulationRoom)
		{
			return null;
		}

        return
            Gameworld.ExitManager.GetExitsFor(this, GetOverlayFor(voyeur), ignoreLayers ? default : voyeur?.RoomLayer)
					 .Where(x => IsSpatiallyAccessibleExit(voyeur, x))
                     .FirstOrDefault(x => x.Destination == otherRoom);
    }

	private bool IsSpatiallyAccessibleExit(IPerceiver voyeur, IRoomExit exit)
	{
		if (_routeDefinition is null || voyeur is null)
		{
			return true;
		}

		return ReferenceEquals(voyeur.Location, this) &&
		       RouteSpatialService.Instance.IsExitAccessible(voyeur, exit);
	}

    public ITerrain Terrain(IPerceiver voyeur)
    {
        return
            EffectsOfType<IOverrideTerrain>().FirstOrDefault(x => x.Applies())?.Terrain ??
            GetOverlayFor(voyeur).Terrain;
    }

    public RoomOutdoorsType OutdoorsType(IPerceiver voyeur)
    {
        return GetOverlayFor(voyeur).OutdoorsType;
    }

    public PrecipitationLevel HighestRecentPrecipitationLevel(IPerceiver voyeur)
    {
        IRoomOverlay overlay = GetOverlayFor(voyeur);
        if (overlay.Terrain.OverrideWeatherController != null)
        {
            return overlay.Terrain.OverrideWeatherController.HighestRecentPrecipitationLevel;
        }

        return Areas.FirstOrDefault(x => x.WeatherController != null)?.WeatherController.HighestRecentPrecipitationLevel ??
               Zone.WeatherController?.HighestRecentPrecipitationLevel ??
               PrecipitationLevel.Parched;
    }

    public override IWeatherController WeatherController => CurrentOverlay.Terrain.OverrideWeatherController ??
                   Areas.FirstOrDefault(x => x.WeatherController != null)?.WeatherController ??
                   Zone.WeatherController;

    public IWeatherEvent CurrentWeather(IPerceiver voyeur)
    {
        return WeatherController?.CurrentWeatherEvent;
    }

    public ISeason CurrentSeason(IPerceiver voyeur)
    {
        return WeatherController?.CurrentSeason;
    }

    public double CurrentTemperature(IPerceiver voyeur)
    {
        double baseTemperature = 0.0;
        IRoomOverlay overlay = GetOverlayFor(voyeur);
        if (overlay.Terrain.OverrideWeatherController != null)
        {
            baseTemperature = overlay.Terrain.OverrideWeatherController.CurrentTemperature;
        }
        else
        {
            IWeatherController weather = Areas.FirstOrDefault(x => x.WeatherController != null)?.WeatherController ??
                          Zone.WeatherController;

            if (weather == null)
            {
                baseTemperature = Gameworld.GetStaticDouble("DefaultCellTemperature");
            }
            else
            {
                switch (overlay.OutdoorsType)
                {
                    case RoomOutdoorsType.Indoors:
                    case RoomOutdoorsType.IndoorsWithWindows:
                    case RoomOutdoorsType.IndoorsNoLight:
                        // Indoors is sheltered from rain and wind unless there is an open exit to an outdoors room
                        if (Gameworld.ExitManager.GetExitsFor(this, overlay, voyeur?.RoomLayer).Any(x =>
                                x.Exit.Door?.IsOpen != false && x.Destination.OutdoorsType(voyeur)
                                                                 .In(RoomOutdoorsType.IndoorsClimateExposed,
                                                                     RoomOutdoorsType.Outdoors)))
                        {
                            baseTemperature = weather.CurrentTemperature +
                                              weather.CurrentWeatherEvent.PrecipitationTemperatureEffect;
                            break;
                        }

                        baseTemperature = weather.CurrentTemperature +
                                          weather.CurrentWeatherEvent.PrecipitationTemperatureEffect +
                                          weather.CurrentWeatherEvent.WindTemperatureEffect;
                        break;
                    case RoomOutdoorsType.IndoorsClimateExposed:
                        // This kind of location only protects from the rain, not the wind
                        baseTemperature = weather.CurrentTemperature +
                                          weather.CurrentWeatherEvent.PrecipitationTemperatureEffect;
                        break;
                    default:
                        baseTemperature = weather.CurrentTemperature;
                        break;
                }
            }
        }

        double effectTemperature = EffectsOfType<IAffectEnvironmentalTemperature>()
                                .Concat(Zone.EffectsOfType<IAffectEnvironmentalTemperature>())
                                .Where(x => x.Applies())
                                .Select(x => x.TemperatureDelta)
                                .DefaultIfEmpty(0)
                                .Sum();

        RoomOutdoorsType outdoorsType = OutdoorsType(voyeur);
        double ambientTemperature = ThermalSourceTemperatureModel.AmbientHeatForRoom(this, outdoorsType, voyeur);
        double proximityTemperature = ThermalSourceTemperatureModel.ProximityHeatForTarget(this, voyeur);

        return baseTemperature + effectTemperature + ambientTemperature + proximityTemperature;
    }

    public double CurrentIllumination(IPerceiver voyeur)
    {
        if (voyeur is null)
        {
            voyeur = new DummyPerceiver();
        }

        IRoomOverlay overlay = GetOverlayFor(voyeur);
        double environmentalLight = (OwningZone.CurrentLightLevel * overlay.AmbientLightFactor + overlay.AddedLight) *
                                 (CurrentWeather(voyeur)?.LightLevelMultiplier ?? 1.0);
        IEnumerable<IPerceivable> localLightSources;
        if (RouteDefinition is not null && ReferenceEquals(voyeur.Location, this) &&
            voyeur.RoutePositionMetres.HasValue)
        {
            var maximumDistance = Gameworld.GetStaticDouble("RouteCellVeryDistantDistanceMetres");
            if (!double.IsFinite(maximumDistance) || maximumDistance <= 0.0)
            {
                maximumDistance = RouteSpatialConfiguration.Default.VeryDistantDistanceMetres;
            }

            localLightSources = RouteSpatialService.Instance.GetPerceivablesWithin(
                voyeur.SpatialLocation,
                maximumDistance,
                x => x.RoomLayer == voyeur.RoomLayer);
        }
        else
        {
            localLightSources = LayerCharacters(voyeur.RoomLayer)
                .Cast<IPerceivable>()
                .Concat(LayerGameItems(voyeur.RoomLayer));
        }

        double ambientLight = environmentalLight +
                           localLightSources.Sum(x => x.IlluminationProvided) +
                           EffectsOfType<IAreaLightEffect>(x => x.Applies()).Sum(x => x.AddedLight);
        return ambientLight;
    }

    public Difficulty SpotDifficulty(IPerceiver spotter)
    {
        Difficulty spotDifficultyWeather =
            CurrentWeather(spotter)?.Precipitation.MinimumSightDifficulty() ?? Difficulty.Automatic;
        Difficulty spotDifficultyLight = spotter is ICharacter ch ? ch.IlluminationSightDifficulty(this) : Difficulty.Automatic;
        Difficulty spotDifficultyTerrain = Terrain(spotter).SpotDifficulty;
        return spotDifficultyLight.Highest(spotDifficultyWeather, spotDifficultyTerrain);
    }

    public IRoomOverlay CurrentOverlay { get; protected set; }

    public bool SetCurrentOverlay(IRoomOverlayPackage package)
    {
		using var exposureChange = EnvironmentalExposureService.ChangingEnvironment(this);
        IRoomOverlay overlay = Overlays.FirstOrDefault(x => x.Package == package);
        if (overlay == null)
        {
            return false;
        }

        CurrentOverlay = overlay;
		RefreshWeatherSubscriptions();
		SynchroniseForagableProfile();
		Gameworld.EnvironmentalMagic?.RoomTerrainChanged(this);
        Changed = true;
        return true;
    }

    public IEditableRoomOverlay GetOrCreateOverlay(IRoomOverlayPackage package)
    {
        IEditableRoomOverlay overlay = _overlays.FirstOrDefault(x => x.Package == package);
        if (overlay == null)
        {
            overlay = CurrentOverlay.CreateClone(package);
            _overlays.Add(overlay);
            Changed = true;
        }

        return overlay;
    }

    public void AddOverlay(IEditableRoomOverlay overlay)
    {
        _overlays.Add(overlay);
		RefreshWeatherSubscriptions();
    }

    public void RemoveOverlay(long id)
    {
        IEditableRoomOverlay overlay = _overlays.Find(x => x.Id == id);
        if (overlay is null)
        {
            return;
        }

        _overlays.Remove(overlay);
        if (CurrentOverlay == overlay)
        {
            CurrentOverlay = _overlays.FirstOrDefault(x => x.Package.Status == RevisionStatus.Current);
			SynchroniseForagableProfile();
			Gameworld.EnvironmentalMagic?.RoomTerrainChanged(this);
            Changed = true;
        }
    }

    public IRoomOverlay GetOverlay(IRoomOverlayPackage package)
    {
        return _overlays.FirstOrDefault(x => x.Package == package);
    }

    public IEnumerable<IRoomOverlay> Overlays => _overlays;

    public void Login(ICharacter loginCharacter)
    {
#if DEBUG
        if (loginCharacter.State.HasFlag(CharacterState.Dead))
        {
            Console.WriteLine("Dead NPC!");
        }
#endif
        DateTime oldLoginTime = loginCharacter.LoginDateTime;
        loginCharacter.State &= ~CharacterState.Stasis;
        loginCharacter.LastMinutesUpdate = System.DateTime.UtcNow;
        loginCharacter.LoginDateTime = System.DateTime.UtcNow;
        loginCharacter.OutputHandler?.Register(loginCharacter);
		if (CommandExecutionScope.TryContinue(loginCharacter))
			EnterCore(loginCharacter, null, true, loginCharacter.RoomLayer, null, nativeLogin: true);
        if (loginCharacter is NPC.NPC npc)
        {
            npc.SetupEventSubscriptions();
        }
        else
        {
            StringBuilder sb = new();
            List<KeyValuePair<IChargenResource, double>> displayResourceUpdates = loginCharacter.Account.AccountResources.Where(x => x.Key.DisplayChangesOnLogin && loginCharacter.Account.AccountResourcesLastAwarded[x.Key] >= oldLoginTime).ToList();
            if (displayResourceUpdates.Count > 0)
            {
                sb.AppendLine($"\n\nYou have received awards of {displayResourceUpdates.Select(x => x.Key.PluralName.TitleCase().ColourValue()).ListToString()} since your last login.");
            }

            if (loginCharacter.IsGuest)
            {
                Gameworld.SystemMessage($"Account {loginCharacter.Account.Name} has entered the guest lounge.", true);
            }
            else
            {
                if (loginCharacter.PreviousLoginDateTime != null)
                {
                    loginCharacter.Body.DoOfflineHealing(loginCharacter.LoginDateTime -
                                                         (loginCharacter.LastLogoutDateTime ??
                                                          loginCharacter.LoginDateTime));
                }

                if (loginCharacter.IsAdministrator())
                {

                    if (Gameworld.Boards.Any(x => x.DisplayOnLogin))
                    {
                        Dictionary<IBoard, int> counts = new();
                        foreach (IBoard board in Gameworld.Boards.Where(x => x.DisplayOnLogin))
                        {
                            counts[board] =
                                board.Posts.Count(x =>
                                    x.PostTime > (loginCharacter.PreviousLoginDateTime ?? System.DateTime.MinValue));
                        }

                        if (counts.Any(x => x.Value > 0))
                        {
                            sb.AppendLine("\n\nThe following boards have new posts since your last login:");
                            foreach (KeyValuePair<IBoard, int> count in counts.Where(x => x.Value > 0))
                            {
                                sb.AppendLine(
                                    $"\t{count.Key.Name.Colour(Telnet.Green)} - {count.Value:N0} new post{(count.Value == 1 ? "" : "s")}.");
                            }

                            sb.AppendLine();
                        }
                    }

                    using (new FMDB())
                    {
                        List<Chargen> applications =
                            FMDB.Context.Chargens
                                .Where(x =>
                                    x.Status == (int)CharacterStatus.Submitted &&
                                    (PermissionLevel)(x.MinimumApprovalAuthority ?? 0) <= loginCharacter.Account.Authority.Level
                                )
                                .ToList();
                        if (applications.Count > 0)
                        {
                            sb.AppendLine($"\n\nThere are {applications.Count.ToStringN0Colour(loginCharacter)} new character {"application".Pluralise(applications.Count != 1)} for you to review.");
                        }
                    }

                }

                if (sb.Length > 0)
                {
                    loginCharacter.Send(sb.ToString());
                }
                Gameworld.SystemMessage(
                    new EmoteOutput(
                        new Emote(
                            $"@ ({loginCharacter.PersonalName.GetName(NameStyle.FullName)}) has logged in.",
                            loginCharacter), flags: OutputFlags.SuppressSource), true);
            }
        }

        loginCharacter.LoginCharacter();
    }

    public override void Enter(ICharacter movingCharacter, IRoomExit exit = null, bool noSave = false,
        RoomLayer roomLayer = RoomLayer.GroundLevel)
    {
		if (!CommandExecutionScope.TryContinue(movingCharacter)) return;
		EnterCore(movingCharacter, exit, noSave, roomLayer, null);
	}

	internal bool EnterDisplaced(ICharacter actor, NativeDisplacementReceipt receipt)
	{
		if (!receipt.BeginEnter(actor, this)) return false;
		EnterCore(actor, null, false, receipt.Layer, receipt);
		return ReferenceEquals(actor.Location, this) && Characters.Any(x => ReferenceEquals(x, actor)) && receipt.Continue();
	}

	private void EnterCore(ICharacter movingCharacter, IRoomExit exit, bool noSave, RoomLayer roomLayer,
		NativeDisplacementReceipt? receipt, bool nativeLogin = false)
	{
		if (Gameworld.SpellOwnedShelters is { } shelters && shelters.OwnsRoom(Id) &&
			!(nativeLogin ? shelters.CanReconnect(this, movingCharacter) : shelters.CanEnter(this, movingCharacter)))
		{
			if (ReferenceEquals(movingCharacter.Location, this))
			{
				if (shelters.ReturnRejectedEntrant(this, movingCharacter) || !ReferenceEquals(movingCharacter.Location, this)) return;
			}
			else
			{
				if (movingCharacter.Location is Room original) original.ReconcileNativeCharacterMembership(movingCharacter, true);
				movingCharacter.OutputHandler.Send("That shelter is full or closing.".ColourError());
				return;
			}
		}
		var explicitlyAssignedPosition = receipt is not null ? receipt.RoutePosition : _routeDefinition is not null &&
		                                 ReferenceEquals(movingCharacter.Location, this)
			? movingCharacter.RoutePositionMetres
			: null;
        base.Enter(movingCharacter, exit);
		if (explicitlyAssignedPosition.HasValue)
		{
			movingCharacter.MoveTo(
				new SpatialLocation(this, roomLayer, explicitlyAssignedPosition),
				exit,
				noSave);
		}
		else
		{
			movingCharacter.MoveTo(this, roomLayer, exit, noSave);
		}
		if (receipt is not null && !receipt.AfterMoveTo(movingCharacter)) return;
		if (!_isCombatSimulationRoom)
		{
			OwningZone.Enter(movingCharacter, exit);
			if (receipt is not null && !receipt.Continue()) return;
		}
        DoEnterEvent(movingCharacter);
		if (receipt is not null && !receipt.Continue()) return;

        if (exit != null && exit.InboundDirection != CardinalDirection.Unknown)
        {
            movingCharacter.AddEffect(new AdjacentToExit(movingCharacter, exit.Exit.RoomExitFor(this)),
                AdjacentToExit.DefaultEffectTimeSpan);
        }

        movingCharacter.HandleEvent(EventType.CharacterEnterRoom, movingCharacter, this,
            movingCharacter.Movement?.Exit);
		if (receipt is not null && !receipt.Continue()) return;
        foreach (IHandleEvents witness in SpatialEventHandlersFor(movingCharacter, exit).Except(movingCharacter).ToArray())
        {
            witness.HandleEvent(EventType.CharacterEnterRoomWitness, movingCharacter, this,
                movingCharacter.Movement?.Exit, witness);
			if (receipt is not null && !receipt.Continue()) return;
        }

        foreach (IGameItem witness in movingCharacter.Body.ExternalItems.ToArray())
        {
            witness.HandleEvent(EventType.CharacterEnterRoomWitness, movingCharacter, this,
                movingCharacter.Movement?.Exit, witness);
			if (receipt is not null && !receipt.Continue()) return;
        }

        if (exit is not null && !noSave)
        {
            var isDraggedMovementTarget = movingCharacter.Movement?.Targets.Contains(movingCharacter) == true;
            AutomaticCrimeExtensions.CheckLocationEntryCrimes(movingCharacter, this, exit,
                !isDraggedMovementTarget);
        }

		if (receipt is not null && !receipt.Continue()) return;
        CheckFallExitStatus();
		if (receipt is not null && !receipt.Continue()) return;
        if (movingCharacter.CurrentProject.Project == null)
        {
            movingCharacter.TryJoinQueuedProjectLabour();
        }
    }

    private RoomLayer HandleEnterLayers(ICharacter movingCharacter)
    {
        List<RoomLayer> localLayers = Terrain(movingCharacter).TerrainLayers.ToList();
        if (localLayers.Contains(movingCharacter.RoomLayer))
        {
            return movingCharacter.RoomLayer;
        }

        RoomLayer highest = localLayers.HighestLayer();
        RoomLayer lowest = localLayers.LowestLayer();
        if (movingCharacter.RoomLayer.IsHigherThan(highest))
        {
            return highest;
        }

        return lowest;
    }


	internal void ReconcileNativeCharacterMembership(ICharacter actor, bool present)
	{
		SetNativeCharacterMembership(actor, present);
		if (!_isCombatSimulationRoom)
		{
			foreach (var location in new[] { OwningZone as Location, OwningZone?.Shard as Location }.OfType<Location>().Distinct())
				location.SetNativeCharacterMembership(actor, present || actor.Location?.Characters.Any(x => ReferenceEquals(x, actor)) == true &&
					(location.Rooms.Any(x => ReferenceEquals(x, actor.Location))));
		}
		if (present) RouteSpatialService.Instance.TrackPerceivable(actor);
		else if (ReferenceEquals(actor.Location, this) && !CommandExecutionAuthority.IsCurrentWithoutRoomMembership(actor))
			RouteSpatialService.Instance.UntrackPerceivable(actor);
	}

	internal bool LeaveDisplaced(ICharacter actor, NativeDisplacementReceipt receipt)
	{
		if (!receipt.BeginLeave(actor)) return false;
		LeaveCore(actor, receipt);
		return receipt.Continue();
	}

    public override void Leave(ICharacter movingCharacter) => LeaveCore(movingCharacter, null);

	private void LeaveCore(ICharacter movingCharacter, NativeDisplacementReceipt? receipt)
	{
        ForceDisembarkVehicleOccupantLeavingWithoutVehicle(movingCharacter);
		if (receipt is not null && !receipt.Continue()) return;
        base.Leave(movingCharacter);
		RouteSpatialService.Instance.UntrackPerceivable(movingCharacter);
		if (!_isCombatSimulationRoom)
		{
			OwningZone.Leave(movingCharacter);
			if (receipt is not null && !receipt.Continue()) return;
		}
        DoLeaveEvent(movingCharacter);
		if (receipt is not null && !receipt.Continue()) return;
        movingCharacter.HandleEvent(EventType.CharacterLeaveRoom, movingCharacter, this,
            movingCharacter.Movement?.Exit);
		if (receipt is not null && !receipt.Continue()) return;
        foreach (IHandleEvents witness in SpatialEventHandlersFor(movingCharacter, movingCharacter.Movement?.Exit)
		         .Except(movingCharacter).ToArray())
        {
            witness.HandleEvent(EventType.CharacterLeaveRoomWitness, movingCharacter, this,
                movingCharacter.Movement?.Exit, witness);
			if (receipt is not null && !receipt.Continue()) return;
        }

        foreach (IGameItem witness in movingCharacter.Body.ExternalItems.ToArray())
        {
            witness.HandleEvent(EventType.CharacterLeaveRoomWitness, movingCharacter, this,
                movingCharacter.Movement?.Exit, witness);
			if (receipt is not null && !receipt.Continue()) return;
        }

        CheckFallExitStatus();
    }

    private void ForceDisembarkVehicleOccupantLeavingWithoutVehicle(ICharacter movingCharacter)
    {
        if (movingCharacter is null)
        {
            return;
        }

        foreach (var vehicle in GameItems
                                     .Select(x => x.GetItemType<IVehicleExterior>()?.Vehicle)
                                     .Where(x => x is not null)
                                     .Distinct()
                                     .Where(x => x.IsOccupant(movingCharacter))
                                     .ToList())
        {
            if (movingCharacter.Movement is VehicleMovement vehicleMovement &&
                vehicleMovement.Targets.Contains(vehicle.ExteriorItem))
            {
                continue;
            }

            vehicle.ForceDisembark(movingCharacter, false);
        }
    }

    public IFluid Atmosphere => EffectsOfType<IAffectAtmosphere>().FirstOrDefault(x => x.Applies())?.Atmosphere ??
        MudSharp.Climate.WeatherHazardService.WeatherAtmosphere(this) ?? CurrentOverlay.Atmosphere;

    public override IEnumerable<ICalendar> Calendars => HostedExteriorContext?.Calendars ?? OwningZone.Calendars;

    public override IEnumerable<IClock> Clocks => HostedExteriorContext?.Clocks ?? OwningZone.Clocks;

    public IPermanentShop Shop { get; set; }

    public override IMudTimeZone TimeZone(IClock whichClock)
    {
		return HostedExteriorContext?.TimeZone(whichClock) ?? OwningZone.TimeZone(whichClock);
    }

	private IEnumerable<IHandleEvents> SpatialEventHandlersFor(ILocateable source, IRoomExit exit = null)
	{
		if (RouteDefinition is null)
		{
			return EventHandlers;
		}

		SpatialLocation origin;
		if (ReferenceEquals(source.Location, this) && source.RoutePositionMetres.HasValue)
		{
			origin = RouteSpatialService.Instance.GetEffectiveLocation(source);
		}
		else if (exit is not null &&
		         RouteSpatialService.Instance.TryGetExitAnchor(exit, this, out var anchor))
		{
			origin = new SpatialLocation(this, source.RoomLayer, anchor!.ArrivalPositionMetres);
		}
		else
		{
			origin = new SpatialLocation(this, source.RoomLayer, RouteDefinition.DefaultPositionMetres);
		}

		var maximumDistance = Gameworld.GetStaticDouble("RouteCellVeryDistantDistanceMetres");
		if (!double.IsFinite(maximumDistance) || maximumDistance <= 0.0)
		{
			maximumDistance = RouteSpatialConfiguration.Default.VeryDistantDistanceMetres;
		}

		return RouteSpatialService.Instance
			.GetPerceivablesWithinAcrossLayers(origin, maximumDistance)
			.OfType<IHandleEvents>()
			.Concat(new IHandleEvents[] { this })
			.Distinct();
	}

	public IEnumerable<IHandleEvents> EventHandlersFor(IPerceivable source)
	{
		return SpatialEventHandlersFor(source);
	}

    public override IEnumerable<ICelestialObject> Celestials => HostedExteriorContext?.Celestials ?? OwningZone.Celestials;
    public IEnumerable<IRangedCover> LocalCover => _localCover;

    public IEnumerable<IRangedCover> GetCoverFor(IPerceiver voyeur)
    {
        return Terrain(voyeur).TerrainCovers.Concat(LocalCover).Distinct().ToList();
    }

    public bool IsExitVisible(IPerceiver voyeur, IRoomExit exit, PerceptionTypes type,
        PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
    {
        return
            EffectHandler.EffectsOfType<IExitHiddenEffect>()
                         .Where(x => x.Exit == exit.Exit && x.Applies(voyeur))
                         .Select(x => x.HiddenTypes)
                         .DefaultIfEmpty(PerceptionTypes.None)
                         .Aggregate(type, (prev, effect) => prev & ~effect) != PerceptionTypes.None;
    }

    public override void Register(IOutputHandler handler)
    {
        throw new NotImplementedException();
    }

    public override CelestialInformation GetInfo(ICelestialObject celestial)
    {
        return OwningZone.GetInfo(celestial);
    }

    public double EstimatedDirectDistanceTo(IRoom otherRoom)
    {
        return Math.Sqrt(Math.Pow(StoredCoordinates.X - otherRoom.StoredCoordinates.X, 2) + Math.Pow(StoredCoordinates.Y - otherRoom.StoredCoordinates.Y, 2) +
                         Math.Pow(StoredCoordinates.Z - otherRoom.StoredCoordinates.Z, 2));
    }

    public TimeOfDay CurrentTimeOfDay => HostedExteriorContext?.CurrentTimeOfDay ?? Zone.CurrentTimeOfDay;

    public override MudDate Date(ICalendar whichCalendar)
    {
		return HostedExteriorContext?.Date(whichCalendar) ?? OwningZone.Date(whichCalendar);
    }

    public override MudTime Time(IClock whichClock)
    {
		return HostedExteriorContext?.Time(whichClock) ?? OwningZone.Time(whichClock);
    }

    public void RegisterMovement(IMovement move)
    {
        Movements.Add(move);
        if (Equals(move.Exit.Destination, this))
        {
            foreach (ICharacter member in move.CharacterMovers)
            {
				if (MovementEventUtilities.ShouldSuppressMovementEvents(member))
				{
					continue;
				}

                member.HandleEvent(EventType.CharacterEnterRoom, member, this, move.Exit);
                foreach (IHandleEvents witness in SpatialEventHandlersFor(member, move.Exit).Except(member))
                {
                    witness.HandleEvent(EventType.CharacterEnterRoomWitness, member, this, move.Exit, witness);
                }

                foreach (IGameItem witness in member.Body.ExternalItems)
                {
                    witness.HandleEvent(EventType.CharacterEnterRoomWitness, member, this, move.Exit, witness);
                }
            }
        }
        else if (Equals(move.Exit.Origin, this))
        {
            foreach (ICharacter member in move.CharacterMovers)
            {
				if (MovementEventUtilities.ShouldSuppressMovementEvents(member))
				{
					continue;
				}

                member.HandleEvent(EventType.CharacterBeginMovement, member, this, move.Exit);
                foreach (IHandleEvents witness in SpatialEventHandlersFor(member, move.Exit).Except(member))
                {
                    witness.HandleEvent(EventType.CharacterBeginMovementWitness, member, this, move.Exit, witness, move);
                }

                foreach (IGameItem witness in member.Body.ExternalItems)
                {
                    witness.HandleEvent(EventType.CharacterBeginMovementWitness, member, this, move.Exit, witness, move);
                }
            }
        }
    }

    public void ResolveMovement(IMovement move)
    {
        Movements.Remove(move);
        // Cancelled movements need to handle their own events
        if (!move.Cancelled)
        {
            if (Equals(move.Exit.Destination, this))
            {
                foreach (ICharacter member in move.CharacterMovers)
                {
					if (MovementEventUtilities.ShouldSuppressMovementEvents(member))
					{
						continue;
					}

                    member.HandleEvent(EventType.CharacterEnterRoomFinish, member, this, move.Exit);
                    foreach (IHandleEvents witness in SpatialEventHandlersFor(member, move.Exit).Except(member))
                    {
                        witness.HandleEvent(EventType.CharacterEnterRoomFinishWitness, member, this, move.Exit,
                            witness);
                    }

                    foreach (IGameItem witness in member.Body.ExternalItems)
                    {
                        witness.HandleEvent(EventType.CharacterEnterRoomFinishWitness, member, this, move.Exit,
                            witness);
                    }
                }
            }
            else if (Equals(move.Exit.Origin, this))
            {
                foreach (ICharacter member in move.CharacterMovers)
                {
					if (MovementEventUtilities.ShouldSuppressMovementEvents(member))
					{
						continue;
					}

                    member.HandleEvent(EventType.CharacterEnterRoom, member, this, move.Exit);
                    foreach (IHandleEvents witness in SpatialEventHandlersFor(member, move.Exit).Except(member))
                    {
                        witness.HandleEvent(EventType.CharacterEnterRoomWitness, member, this, move.Exit, witness);
                    }

                    foreach (IGameItem witness in member.Body.ExternalItems)
                    {
                        witness.HandleEvent(EventType.CharacterEnterRoomWitness, member, this, move.Exit, witness);
                    }
                }
            }
        }
    }

    public override Difficulty LocalAudioDifficulty(IPerceiver perceiver, AudioVolume volume, Proximity proximity)
    {
        IRoomOverlay overlay = GetOverlayFor(perceiver);
        return overlay.HearingProfile?.AudioDifficulty(this, volume, proximity) ??
               base.LocalAudioDifficulty(perceiver, volume, proximity);
    }

    public override void Save()
    {
		PrepareForSaveAttempt();
        Models.Room dbcell = FMDB.Context.Rooms.Find(Id);
        dbcell.CurrentOverlayId = CurrentOverlay.Id;
        dbcell.ZoneId = OwningZone.Id;
		dbcell.X = _storedCoordinates.X;
		dbcell.Y = _storedCoordinates.Y;
		dbcell.Z = _storedCoordinates.Z;
		dbcell.UniqueName = UniqueName;
        dbcell.ForagableProfileId = ExplicitForagableProfileId;
		SaveEnvironment(dbcell);
        SaveEffects();
        if (ContentsChanged)
        {
            FMDB.Context.RoomsGameItems.RemoveRange(dbcell.RoomsGameItems);
            foreach (IGameItem item in _gameItems)
            {
                if (item.Id == 0)
                {
                    continue;
                }
                dbcell.RoomsGameItems.Add(new RoomsGameItems { Room = dbcell, GameItemId = item.Id });
            }

            _contentsChanged = false;
        }

        if (ResourcesChanged)
        {
            SaveMagic(dbcell);
        }

        if (TagsChanged)
        {
            SaveTags(dbcell);
        }

        if (EffectsChanged)
        {
            dbcell.EffectData = SaveEffects().ToString();
            EffectsChanged = false;
        }

        if (_surfaceLiquidChanged)
        {
            dbcell.SurfaceLiquidData = SaveSurfaceLiquidState();
            _surfaceLiquidChanged = false;
        }

        if (HooksChanged)
        {
            FMDB.Context.HooksPerceivables.RemoveRange(dbcell.HooksPerceivables);
            foreach (IHook hook in _installedHooks)
            {
                dbcell.HooksPerceivables.Add(new HooksPerceivable
                {
                    Room = dbcell,
                    HookId = hook.Id
                });
            }

            HooksChanged = false;
        }

		// Keep the staged forage rows, their specialised dirty bit and the owner's final save state atomic
		// with respect to native consumption and recovery. A mutation either precedes this snapshot or waits
		// until Changed has been cleared, at which point it marks and queues the room again.
		lock (_foragableYields)
		{
			if (_yieldsChanged)
			{
				RecordForagableYieldSaveAttempt();
				FMDB.Context.RoomsForagableYields.RemoveRange(dbcell.RoomsForagableYields);
				foreach (var item in _foragableYields)
				{
					var dbYield = new RoomsForagableYield
					{
						Room = dbcell,
						ForagableType = item.Key,
						Yield = item.Value
					};
					dbcell.RoomsForagableYields.Add(dbYield);
				}

				_yieldsChanged = false;
			}

			Changed = false;
		}
    }

	internal void PrepareForSaveAttempt()
	{
		_recoverableSaveAttempt = new RoomSaveAttemptState(
			ContentsChanged,
			ResourcesChanged,
			YieldsChanged,
			TagsChanged,
			EffectsChanged,
			_surfaceLiquidChanged,
			HooksChanged,
			_environmentStateChanged,
			ExpectedEnvironmentDatabaseRevision);
	}

	private void RecordForagableYieldSaveAttempt()
	{
		if (_recoverableSaveAttempt is { } attempt)
		{
			_recoverableSaveAttempt = attempt with { YieldsChanged = true };
		}
	}

	public void RecoverFromSaveFailure()
	{
		if (_recoverableSaveAttempt is not { } attempt)
		{
			return;
		}

		_recoverableSaveAttempt = null;
		_contentsChanged |= attempt.ContentsChanged;
		_resourcesChanged |= attempt.ResourcesChanged;
		_yieldsChanged |= attempt.YieldsChanged;
		_tagsChanged |= attempt.TagsChanged;
		_surfaceLiquidChanged |= attempt.SurfaceLiquidChanged;
		_environmentStateChanged |= attempt.EnvironmentStateChanged;
		if (attempt.EnvironmentStateChanged)
		{
			ExpectedEnvironmentDatabaseRevision = attempt.ExpectedEnvironmentDatabaseRevision;
		}

		if (attempt.EffectsChanged)
		{
			EffectsChanged = true;
		}

		if (attempt.HooksChanged)
		{
			HooksChanged = true;
		}
	}

    public void Dispose()
    {
		foreach (var controller in _subscribedWeatherControllers) UnsubscribeWeather(controller);
		_subscribedWeatherControllers.Clear();
        Gameworld.Destroy(this);
        GC.SuppressFinalize(this);
    }

    public static void RegisterPerceivableType(IFuturemud gameworld)
    {
        gameworld.RegisterPerceivableType("Cell", id => gameworld.Rooms.Get(id));
		gameworld.RegisterPerceivableType(PersistedFrameworkItemReference.RoomType, id => gameworld.Rooms.Get(id));
		gameworld.RegisterPerceivableType("Room", id =>
			throw new InvalidOperationException($"Unsupported legacy Room entity reference #{id}; review its provenance before loading it. It cannot be treated as a current Room ID."));
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return (obj as IRoom)?.Id == Id || this == obj;
    }

    public string GetFriendlyReference(IPerceiver voyeur)
    {
        return
            $"{HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreLayers)} [#{Id.ToString("N0", voyeur)}]";
    }

    public void SetupRoom(MudSharp.Models.Room room)
    {
        _noSave = true;
        _id = room.Id;
		ValidatePersistedUniqueNames([room]);
		UniqueName = RoomLookupExtensions.NormaliseUniqueName(room.UniqueName);
		_hostedVehicleId = room.HostedVehicleId;
		_hostedVehicleCompartmentId = room.HostedVehicleCompartmentId;
		ReloadRouteDefinition(room.RouteRoom);
        _storedCoordinates = (room.X, room.Y, room.Z);
		OwningZone.Register(this);
        foreach (Models.RoomOverlay overlay in room.RoomOverlays)
        {
            _overlays.Add(new RoomOverlay(overlay, this, Gameworld));
        }

        CurrentOverlay = _overlays.First(x => x.Id == room.CurrentOverlayId);
        _foragableProfile = null;
        _foragableProfileId = room.ForagableProfileId ?? 0;
        Movements = new List<IMovement>();
        LoadHooks(room.HooksPerceivables, "Room");
        LoadEffects(XElement.Parse(room.EffectData.IfNullOrWhiteSpace("<Effects/>")));
        LoadSurfaceLiquidState(room.SurfaceLiquidData);
        foreach (RoomsRangedCovers cover in room.RoomsRangedCovers)
        {
            _localCover.Add(Gameworld.RangedCovers.Get(cover.RangedCoverId));
        }

        Temporary = room.Temporary;
		RefreshWeatherSubscriptions();

        LoadTags(room);
		LoadEnvironment(room);
        ScheduleCachedEffects();
        LoadMagic(room);
        _noSave = false;
    }

    private void ControllerOnWeatherControllerRoomTick(IWeatherController sender, Action<IRoom> visitor)
    {
        if (ReferenceEquals(WeatherController, sender)) visitor(this);
    }

    private void WeatherControllerChangedController(IWeatherController sender, IWeatherEvent oldWeather, IWeatherEvent newWeather)
    {
        foreach (IHandleEvents handler in EventHandlers)
        {
            if ((handler is IPerceiver p ? WeatherForObserver(p) : WeatherController) != sender)
            {
                continue;
            }

            handler.HandleEvent(EventType.WeatherChanged, handler, oldWeather, newWeather);
        }
    }

    private void WeatherControllerEchoController(IWeatherController sender, string echo)
    {
        foreach (ICharacter actor in Characters.Where(x =>
                     WeatherForObserver(x) == sender))
        {
            if (actor.RoomLayer.IsUnderwater())
            {
                continue;
            }

            switch (actor.Location.OutdoorsType(actor))
            {
                case RoomOutdoorsType.Outdoors:
                    actor.OutputHandler.Send(echo);
                    break;
                case RoomOutdoorsType.IndoorsClimateExposed:
                case RoomOutdoorsType.IndoorsWithWindows:
                    actor.OutputHandler.Send($"{"[Outside]".ColourValue()} {echo}");
                    break;
            }
        }
    }

    private readonly List<IWeatherController> _subscribedWeatherControllers = new();

	internal void RefreshWeatherSubscriptions()
	{
		var controllers = _overlays.Select(x => x.Terrain?.OverrideWeatherController)
			.Concat(Areas.Select(x => x.WeatherController))
			.Append(Zone?.WeatherController)
			.Where(x => x is not null)
			.Distinct()
			.ToArray();
		foreach (var controller in _subscribedWeatherControllers.Except(controllers).ToArray())
		{
			UnsubscribeWeather(controller);
			_subscribedWeatherControllers.Remove(controller);
		}
		foreach (var controller in controllers.Except(_subscribedWeatherControllers))
		{
			controller.WeatherEcho += WeatherControllerEchoController;
			controller.WeatherChanged += WeatherControllerChangedController;
			controller.WeatherRoomTick += ControllerOnWeatherControllerRoomTick;
			_subscribedWeatherControllers.Add(controller);
		}
	}

	private void UnsubscribeWeather(IWeatherController controller)
	{
		controller.WeatherEcho -= WeatherControllerEchoController;
		controller.WeatherChanged -= WeatherControllerChangedController;
		controller.WeatherRoomTick -= ControllerOnWeatherControllerRoomTick;
	}

	private IWeatherController WeatherForObserver(IPerceiver observer) =>
		GetOverlayFor(observer).Terrain.OverrideWeatherController ??
		Areas.FirstOrDefault(x => x.WeatherController is not null)?.WeatherController ?? Zone.WeatherController;

	public void AreaAdded(IArea area) => RefreshWeatherSubscriptions();
	public void AreaRemoved(IArea area) => RefreshWeatherSubscriptions();

    /// <summary>
    ///     If the voyeur is specifying an overlay package they wish to see, and this room has an overlay from that package,
    ///     display that, otherwise display the current one
    /// </summary>
    /// <param name="voyeur">The person for whom the overlay is being displayed</param>
    /// <returns>The appropriate IRoomOverlay</returns>
    public IRoomOverlay GetOverlayFor(IPerceiver voyeur)
    {
        return voyeur?.CurrentOverlayPackage != null
            ? Overlays.FirstOrDefault(x => x.Package == voyeur.CurrentOverlayPackage) ?? CurrentOverlay
            : CurrentOverlay;
    }

    public bool CanSee(ILocateable target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
    {
        return true;
    }

    public override string ToString()
    {
        return $"Room ID {Id} Name {CurrentOverlay.RoomName}";
    }

    public void OnExitsInitialised()
    {
        CheckFallExitStatus();
    }

    private bool _roomFallActive;
    private bool _treeFallActive;
    private bool _sinkUnderwaterActive;
    private bool _zeroGravityActive;

    public void CheckFallExitStatus()
    {
        if (_roomFallActive)
        {
            Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= RoomFallTick;
            _roomFallActive = false;
        }

        if (_treeFallActive)
        {
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= TreeFallTick;
            _treeFallActive = false;
        }

        if (_sinkUnderwaterActive)
        {
            Gameworld.HeartbeatManager.FuzzyThirtySecondHeartbeat -= SinkUnderwaterTick;
            _sinkUnderwaterActive = false;
        }

        if (_zeroGravityActive)
        {
            Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= ZeroGravityTick;
            _zeroGravityActive = false;
        }

        if (!_characters.Any() && !_gameItems.Any())
        {
            return;
        }

        _fallExit = ExitsFor(null).FirstOrDefault(x => x.IsFallExit && x.OutboundDirection == CardinalDirection.Down);

        if (_fallExit != null || Terrain(null).TerrainLayers.Any(x => x.In(RoomLayer.InAir, RoomLayer.HighInAir)))
        {
            Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += RoomFallTick;
            _roomFallActive = true;
        }

        if (ZeroGravityMovementHelper.GravityFor(this) == GravityModel.ZeroGravity &&
            !Terrain(null).TerrainLayers.Any(x => x.IsUnderwater()))
        {
            Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += ZeroGravityTick;
            _zeroGravityActive = true;
        }

        if (Terrain(null).TerrainLayers.Any(x => x.In(RoomLayer.HighInTrees, RoomLayer.InTrees, RoomLayer.OnRooftops)))
        {
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += TreeFallTick;
            _treeFallActive = true;
        }

        if (Terrain(null).TerrainLayers.Any(x => x.IsUnderwater()))
        {
            Gameworld.HeartbeatManager.FuzzyThirtySecondHeartbeat += SinkUnderwaterTick;
            _sinkUnderwaterActive = true;
        }

        List<RoomLayer> layers = Terrain(null).TerrainLayers.ToList();
        foreach (IGameItem item in GameItems)
        {
            if (layers.Contains(item.RoomLayer))
            {
                continue;
            }

            item.RoomLayer = layers.All(x => x.IsHigherThan(item.RoomLayer)) ? layers.LowestLayer() : layers.Where(x => x.IsLowerThan(item.RoomLayer)).LowestLayer();
            item.Changed = true;
        }
    }

    private IRoomExit _fallExit;

    private void ZeroGravityTick()
    {
        foreach (var item in GameItems.ToList())
        {
            ZeroGravityMovementHelper.EnsureFloating(item);
        }

        foreach (var character in Characters.ToList())
        {
            ZeroGravityMovementHelper.EnsureFloating(character);
        }
    }

    private static Expression _itemWeightPerWindLevelExpression;

    public static Expression ItemWeightPerWindLevelExpression
    {
        get
        {
            if (_itemWeightPerWindLevelExpression == null)
            {
                _itemWeightPerWindLevelExpression = new Expression(Futuremud.Games.First()
                                                                            .GetStaticConfiguration(
                                                                                "ItemWeightPerWindLevelTreeFall"));
            }

            return _itemWeightPerWindLevelExpression;
        }
    }

    private static Dictionary<WindLevel, double> WindLevels = new();

    private double WeightForWind(WindLevel level)
    {
        if (!WindLevels.ContainsKey(level))
        {
            WindLevels[level] = ItemWeightPerWindLevelExpression.EvaluateDoubleWith(("wind", (int)level));
        }

        return WindLevels[level];
    }

    private static Dictionary<WindLevel, Difficulty> _treeFallDifficultyPerWind;

    private static Dictionary<WindLevel, Difficulty> TreeFallDifficultyPerWind
    {
        get
        {
            if (_treeFallDifficultyPerWind == null)
            {
                _treeFallDifficultyPerWind = new Dictionary<WindLevel, Difficulty>();
                foreach (WindLevel wind in Enum.GetValues(typeof(WindLevel)).OfType<WindLevel>())
                {
                    _treeFallDifficultyPerWind[wind] = (Difficulty)Futuremud.Games.First()
                                                                            .GetStaticInt(
                                                                                $"TreeFallDifficulty{wind.DescribeEnum()}");
                }
            }

            return _treeFallDifficultyPerWind;
        }
    }

    private void SinkUnderwaterTick()
    {
        CollectionDictionary<RoomLayer, IGameItem> items = GameItems.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
        CollectionDictionary<RoomLayer, ICharacter> characters = Characters.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
        RoomLayer lowestLayer = Terrain(null).TerrainLayers.FirstMin(x => x.LayerHeight());
        ILiquid liquid = (ILiquid)Terrain(null).WaterFluid;

        List<RoomLayer> underwaterLayers = Terrain(null).TerrainLayers.Where(x => IsSwimmingLayer(x)).ToList();
        foreach (RoomLayer layer in underwaterLayers.OrderBy(x => x.LayerHeight()))
        {
            foreach (IGameItem item in items[layer])
            {
                EnvironmentalExposureService.For(Gameworld).RefreshImmersion(item, liquid);
                var exteriorVehicle = item.GetItemType<IVehicleExterior>()?.Vehicle;
                if (exteriorVehicle.KeepsExteriorAfloat())
                {
                    item.PositionState = PositionFloatingInWater.Instance;
                    continue;
                }

                if (!item.IsItemType<IHoldable>() || !item.GetItemType<IHoldable>().IsHoldable)
                {
                    continue;
                }

                if (layer == lowestLayer && item.PositionState != PositionFloatingInWater.Instance)
                {
                    continue;
                }

                if (item.Buoyancy(liquid.Density) >= 0.0)
                {
                    continue;
                }

                if (lowestLayer == layer)
                {
                    item.OutputHandler.Handle(new EmoteOutput(
                        new Emote(Gameworld.GetStaticString("ItemSinkHitBottomEmote"), item),
                        style: OutputStyle.NoNewLine));
                    item.PositionState = PositionUndefined.Instance;
                    continue;
                }

                if (IsUnderwaterLayer(layer))
                {
                    item.OutputHandler.Handle(new EmoteOutput(
                        new Emote(Gameworld.GetStaticString("ItemSinkEmote"), item), style: OutputStyle.NoNewLine));
                }
                else
                {
                    item.OutputHandler.Handle(new EmoteOutput(
                        new Emote(Gameworld.GetStaticString("ItemSinkBelowSurfaceEmote"), item),
                        style: OutputStyle.NoNewLine));
                }

                item.RoomLayer = underwaterLayers.Where(x => x.IsLowerThan(layer)).FirstMax(x => x.LayerHeight());
                item.OutputHandler.Handle(new EmoteOutput(
                    new Emote(Gameworld.GetStaticString("ItemSinkTargetEmote"), item), style: OutputStyle.NoNewLine,
                    flags: OutputFlags.SuppressSource));
            }

            foreach (ICharacter character in characters[layer])
            {
                if (character.PositionState == PositionFlying.Instance)
                {
                    continue;
                }

                if (character.IsProtectedFromSurfaceWater(out _))
                {
                    continue;
                }

                EnvironmentalExposureService.For(Gameworld).RefreshImmersion(character.Body);
            }
        }
    }

    private void TreeFallTick()
    {
        IWeatherEvent weather = CurrentWeather(null);
        if (weather == null)
        {
            return;
        }

        double itemWeightLimit = WeightForWind(weather.Wind);
        CollectionDictionary<RoomLayer, IGameItem> items = GameItems.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
        foreach (IGameItem item in items[RoomLayer.HighInTrees])
        {
            if (!item.IsItemType<IHoldable>() || !item.GetItemType<IHoldable>().IsHoldable)
            {
                continue;
            }

            if (item.Weight > itemWeightLimit)
            {
                continue;
            }

            item.FallToGround();
        }

        foreach (IGameItem item in items[RoomLayer.InTrees])
        {
            if (!item.IsItemType<IHoldable>() || !item.GetItemType<IHoldable>().IsHoldable)
            {
                continue;
            }

            if (item.Weight > itemWeightLimit)
            {
                continue;
            }

            item.FallToGround();
        }

        foreach (IGameItem item in items[RoomLayer.OnRooftops])
        {
            if (!item.IsItemType<IHoldable>() || !item.GetItemType<IHoldable>().IsHoldable)
            {
                continue;
            }

            if (item.Weight > itemWeightLimit)
            {
                continue;
            }

            item.FallToGround();
        }


        CollectionDictionary<RoomLayer, ICharacter> characters = Characters.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
        ICheck check = Gameworld.GetCheck(CheckType.AvoidFallDueToWind);
        Difficulty difficulty = TreeFallDifficultyPerWind[weather.Wind];
        foreach (ICharacter ch in characters[RoomLayer.HighInTrees])
        {
            if (MudSharp.Combat.Moves.CombatForcedMovementUtilities.IsSupportedByGrapple(ch)) continue;
            if (check.Check(ch, difficulty).FailureDegrees() <= 1)
            {
                continue;
            }

            ch.FallToGround();
        }

        foreach (ICharacter ch in characters[RoomLayer.InTrees])
        {
            if (MudSharp.Combat.Moves.CombatForcedMovementUtilities.IsSupportedByGrapple(ch)) continue;
            if (check.Check(ch, difficulty).FailureDegrees() <= 1)
            {
                continue;
            }

            ch.FallToGround();
        }

        foreach (ICharacter ch in characters[RoomLayer.OnRooftops])
        {
            if (MudSharp.Combat.Moves.CombatForcedMovementUtilities.IsSupportedByGrapple(ch)) continue;
            if (check.Check(ch, difficulty).FailureDegrees() <= 1)
            {
                continue;
            }

            ch.FallToGround();
        }
    }

    private void RoomFallTick()
    {
        CollectionDictionary<RoomLayer, IGameItem> items = GameItems.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
        foreach (IGameItem item in items[RoomLayer.HighInAir])
        {
            if (item.ShouldFall())
            {
                item.FallToGround();
            }
        }

        CollectionDictionary<RoomLayer, ICharacter> characters = Characters.Select(x => (x.RoomLayer, x)).ToCollectionDictionary();
        foreach (ICharacter ch in characters[RoomLayer.HighInAir])
        {
            if (ch.ShouldFall())
            {
                ch.FallToGround();
            }
        }

        foreach (IGameItem item in items[RoomLayer.InAir])
        {
            if (item.ShouldFall())
            {
                item.FallToGround();
            }
        }

        foreach (ICharacter ch in characters[RoomLayer.InAir])
        {
            if (ch.ShouldFall())
            {
                ch.FallToGround();
            }
        }

        if (_fallExit?.Exit.Door?.IsOpen == false)
        {
            return;
        }

        RoomLayer lowest = Terrain(null).TerrainLayers.LowestLayer();
        if (lowest.IsUnderwater())
        {
            return;
        }

        foreach (IGameItem item in items[lowest])
        {
            if (item.ShouldFall())
            {
                item.FallToGround();
            }
        }

        foreach (ICharacter ch in characters[lowest])
        {
            if (ch.ShouldFall())
            {
                ch.FallToGround();
            }
        }
    }

    #region IRoom Members

    public (bool Truth, IEnumerable<string> Errors) ProposeDelete()
    {
        ProposalRejectionResponse response = new();
        RoomProposedForDeletion?.Invoke(this, response);
        return (!response.IsRejected, response.Reasons);
    }

    public event RoomProposedForDeletionDelegate RoomProposedForDeletion;
    public event EventHandler RoomRequestsDeletion;

    public void Destroy(IRoom fallbackRoom)
    {
		if (Gameworld.SpellOwnedShelters?.RequestRetirement(Id, SpellRetirementReason.Dismissal) == true) return;
        Action action = DestroyWithDatabaseAction(fallbackRoom);
        Gameworld.SaveManager.Flush();
        Gameworld.LogManager.FlushLog();
        using (new FMDB())
        {
            action?.Invoke();
            Gameworld.SaveManager.Abort(this);
            if (_id != 0)
            {
                Gameworld.SaveManager.Flush();
                Models.Room dbitem = FMDB.Context.Rooms.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.Rooms.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }


            }
        }
    }

    public Action DestroyWithDatabaseAction(IRoom fallbackRoom)
    {
		if (Gameworld.SpellOwnedShelters?.OwnsRoom(Id) == true)
			throw new InvalidOperationException("Spell-owned rooms retire through their exact shelter manifest.");
        _noSave = true;
        RoomRequestsDeletion?.Invoke(this, EventArgs.Empty);
        foreach (IGameItem item in _gameItems.ToList())
        {
            Extract(item);
            fallbackRoom.Insert(item, true);
        }

        foreach (ICharacter ch in _characters.ToList())
        {
            ch.Combat?.EndCombat(true);
            ch.Movement?.Cancel();
            ch.Movement = null;
            Leave(ch);
            fallbackRoom.Enter(ch);
        }

        Gameworld.SaveManager.Abort(this);
        Gameworld.Destroy(this);
        Gameworld.EffectScheduler.Destroy(this);
        Gameworld.ExitManager.DeleteRoom(this);
        return () =>
        {
            EffectHandler.RemoveAllEffects();
            FMDB.Connection.Query($"UPDATE characters SET location = {fallbackRoom.Id} WHERE location = {Id}");
        };
    }

    public bool CanGet(IGameItem item, ICharacter getter)
    {
        if (!CanGetAccess(item, getter))
        {
            return false;
        }

        if (Characters.Any(x => x.Body.EffectsOfType<RestraintEffect>().Any(y => y.TargetItem == item)))
        {
            return false;
        }

        return true;
    }

    public string WhyCannotGet(IGameItem item, ICharacter getter)
    {
        if (!CanGetAccess(item, getter))
        {
            return WhyCannotGetAccess(item, getter);
        }

        if (Characters.Any(x => x.Body.EffectsOfType<RestraintEffect>().Any(y => y.TargetItem == item)))
        {
            return
                $"Unfortunately {Characters.Where(x => x.Body.EffectsOfType<RestraintEffect>().Any(y => y.TargetItem == item)).Select(x => x.HowSeen(getter)).ListToString()} are secured to {item.HowSeen(getter)} and so you must first release them from their restraints before you can get that.";
        }

        throw new ApplicationException("");
    }

    public bool CanGetAccess(IGameItem item, ICharacter getter)
    {
		if (RouteDefinition is not null && getter.GetProximity(item) > Proximity.Immediate)
		{
			return false;
		}

		List<IGameItem> vicinity = LayerGameItems(getter.RoomLayer)
			.Except(item)
			.Where(x => x.InVicinity(item))
			.Where(x => RouteDefinition is null || x.GetProximity(item) <= Proximity.Immediate)
			.ToList();
		return !LayerCharacters(getter.RoomLayer)
			.Where(x => !x.SamePhysicalInstance(getter))
			.Where(x => RouteDefinition is null || x.GetProximity(item) <= Proximity.Immediate)
                                                 .Any(
                                                     x =>
                                                         x.EffectsOfType<IGuardItemEffect>()
                                                          .Any(y => (y.Applies() && y.TargetItem == item) ||
                                                                    vicinity.Contains(y.TargetItem)) &&
                                                         !x.TrustedAllyIDs.Contains(getter.Id) &&
                                                         !x.AllyIDs.Contains(getter.Id));
    }

    public string WhyCannotGetAccess(IGameItem item, ICharacter getter)
    {
		if (RouteDefinition is not null && getter.GetProximity(item) > Proximity.Immediate)
		{
			return $"{item.HowSeen(getter, true)} is too far away for you to reach.";
		}

		List<IGameItem> vicinity = LayerGameItems(getter.RoomLayer)
			.Except(item)
			.Where(x => x.InVicinity(item))
			.Where(x => RouteDefinition is null || x.GetProximity(item) <= Proximity.Immediate)
			.ToList();
        List<ICharacter> guarders =
			LayerCharacters(getter.RoomLayer)
			                                 .Where(x => !x.SamePhysicalInstance(getter))
			                                 .Where(x => RouteDefinition is null ||
			                                             x.GetProximity(item) <= Proximity.Immediate)
                                             .Where(
                                                 x =>
                                                     x.EffectsOfType<IGuardItemEffect>()
                                                      .Any(y => (y.Applies() && y.TargetItem == item) ||
                                                                vicinity.Contains(y.TargetItem)) &&
                                                     !x.TrustedAllyIDs.Contains(getter.Id) &&
                                                     !x.AllyIDs.Contains(getter.Id))
                                             .ToList();
        if (guarders.Any())
        {
            return
                $"{guarders.Select(x => x.HowSeen(getter)).ListToString()} {(guarders.Count == 1 ? "is" : "are")} guarding {item.HowSeen(getter)} and blocking all access to it.";
        }

        throw new NotImplementedException();
    }

    public event RoomEchoEvent OnRoomEcho;
    public event RoomEmoteEchoEvent OnRoomEmoteEcho;

    public override void HandleRoomEcho(string echo, RoomLayer? layer = null)
    {
        OnRoomEcho?.Invoke(this, layer, echo);
    }

    public override void HandleRoomEcho(IEmoteOutput emote, RoomLayer? layer = null)
    {
        if (emote is null)
        {
            return;
        }

        OnRoomEmoteEcho?.Invoke(this, layer, emote);
        if (OnRoomEcho != null)
        {
            OnRoomEcho(this, layer, emote.ParseFor(null));
        }
    }

    public void HandleAudioEcho(string audioText, AudioVolume volume, IPerceiver source, RoomLayer originalLayer,
        bool ignoreOriginLayer = true)
    {
        HandleAudioEcho(audioText, volume, source, originalLayer, ignoreOriginLayer, "sound");
    }

    public void HandleAudioEcho(string audioText, AudioVolume volume, IPerceiver source, RoomLayer originalLayer,
        bool ignoreOriginLayer, string noiseType)
    {
        if (volume == AudioVolume.Silent)
        {
            return;
        }

        NoiseEmission.RaiseEvent(this, source, volume, noiseType, audioText);

		if (RouteRoomAudioPropagation.Instance.RequiresSpatialPropagation(this, volume))
		{
			// Any bounded audio graph that touches RouteRoom topology uses the spatial audio
			// service. It resolves an exact source coordinate, uses indexed longitudinal
			// candidates and follows only reachable anchored portals. Missing RouteRoom source
			// coordinates fail closed.
			RouteRoomAudioPropagation.Instance.Propagate(
				this,
				audioText,
				volume,
				source,
				originalLayer,
				ignoreOriginLayer);
			return;
		}

        List<IRoom> vicinity = this.RoomsInVicinity((uint)volume, false, false)
                                       .Except(this)
                                       .ToList();

        foreach (IRoom location in vicinity)
        {
            if (!location.Characters.Any() && !location.GameItems.Any())
            {
                continue;
            }

            List<IRoomExit> directions = location.ExitsBetween(this, 10).ToList();
            AudioVolume adjustedVolume = volume.StageDown((uint)Math.Max(0, directions.Count - 1));
            location.Handle(new AudioOutput(
                new Emote(string.Format(audioText, directions.DescribeDirectionsToFrom(), adjustedVolume.DescribeEnum(true)), source),
                adjustedVolume,
                flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
        }

        foreach (RoomLayer layer in Terrain(null).TerrainLayers)
        {
            if (layer == originalLayer)
            {
                if (ignoreOriginLayer)
                {
                    continue;
                }

                this.Handle(layer,
                    new AudioOutput(new Emote(string.Format(audioText, "here", volume.DescribeEnum(true)), source),
                        volume,
                        flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
                continue;
            }

            if (layer.IsLowerThan(originalLayer))
            {
                this.Handle(layer,
                        new AudioOutput(new Emote(string.Format(audioText, "from above", volume.DescribeEnum(true)), source),
                            volume,
                            flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers))
                    ;
                continue;
            }

            this.Handle(layer,
                new AudioOutput(new Emote(string.Format(audioText, "from below", volume.DescribeEnum(true)), source),
                    volume,
                    flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
        }
    }

    public void HandleAudioEcho(string audioText, AudioVolume volume, double propagationBudget,
        AudioPropagationMode propagationMode, IPerceiver source, RoomLayer originalLayer,
        bool ignoreOriginLayer, string noiseType)
    {
		if (!Enum.IsDefined(propagationMode))
		{
			throw new ArgumentOutOfRangeException(nameof(propagationMode));
		}

        if (volume == AudioVolume.Silent)
        {
            return;
        }

        NoiseEmission.RaiseEvent(this, source, volume, noiseType, audioText);
        StructuredNoisePropagation.Instance.Propagate(
            this,
            audioText,
            volume,
            propagationBudget,
            propagationMode,
            source,
            originalLayer,
            ignoreOriginLayer,
            noiseType);
    }


    private long _foragableProfileId;
    private IForagableProfile _foragableProfile;
    private long _foragableYieldProfileId;
    private int _foragableYieldProfileRevision;
    private long _foragableYieldDefinitionRevision;
	private long _foragableYieldSourceRevision;
	private object _foragableYieldSubscriptionSync = new();
	private object ForagableYieldSubscriptionSync => _foragableYieldSubscriptionSync ??= new object();

    public IForagableProfile ForagableProfile
    {
        get => ResolveForagableProfile();
        set
        {
            _foragableProfile = value;
            _foragableProfileId = 0;
            SynchroniseForagableYields(ResolveForagableProfile());
            Changed = true;
        }
    }

    public IAgricultureField AgricultureField => Gameworld.AgricultureFields.FirstOrDefault(x => x.Room?.Id == Id);

    private long? ExplicitForagableProfileId => _foragableProfile?.Id ?? (_foragableProfileId == 0 ? null : _foragableProfileId);

    private IForagableProfile ResolveForagableProfile()
    {
        if (_foragableProfileId == 0 && _foragableProfile is { Status: not RevisionStatus.Current })
        {
            _foragableProfileId = _foragableProfile.Id;
        }

        if (_foragableProfileId != 0)
        {
            var profile = Gameworld.ForagableProfiles.Get(_foragableProfileId);
            if (profile == null)
            {
                return null;
            }

            _foragableProfile = profile;
            _foragableProfileId = 0;
        }

        return _foragableProfile ?? OwningZone?.ForagableProfile ?? CurrentOverlay?.Terrain?.ForagableProfile;
    }

	public bool HasForagableProfile => PeekForagableProfile() != null;

	private IForagableProfile PeekForagableProfile()
	{
		if (_foragableProfileId != 0)
		{
			return Gameworld.ForagableProfiles.Get(_foragableProfileId);
		}

		if (_foragableProfile is { Status: not RevisionStatus.Current })
		{
			return Gameworld.ForagableProfiles.Get(_foragableProfile.Id);
		}

		return _foragableProfile ?? OwningZone?.ForagableProfile ?? CurrentOverlay?.Terrain?.ForagableProfile;
	}

	public void SynchroniseForagableProfile()
	{
		SynchroniseForagableYields(ResolveForagableProfile());
	}

    private readonly Dictionary<string, double> _foragableYields = new(StringComparer.InvariantCultureIgnoreCase);
    private const double YieldComparisonTolerance = 1.0e-9;

    public double GetForagableYield(string foragableType)
    {
        SynchroniseForagableYields(ResolveForagableProfile());
		lock (_foragableYields)
		{
			var yield = _foragableYields.GetValueOrDefault(foragableType);
			return double.IsFinite(yield) && yield > 0.0 ? yield : 0.0;
		}
    }

	public bool TryPeekForagableYield(string foragableType, out double yield)
	{
		yield = 0.0;
		if (string.IsNullOrWhiteSpace(foragableType))
		{
			return false;
		}

		var profile = PeekForagableProfile();
		if (profile == null || !profile.MaximumYieldPoints.TryGetValue(foragableType, out var maximum) ||
		    !double.IsFinite(maximum) || maximum <= 0.0)
		{
			return false;
		}

		lock (_foragableYields)
		{
			// Project the same clamping/new-key rules as synchronisation without writing its result.
			yield = _foragableYields.TryGetValue(foragableType, out var existing)
				? double.IsFinite(existing) ? Math.Clamp(existing, 0.0, maximum) : 0.0
				: maximum;
		}

		return true;
	}

	public bool TryPeekForagableYield(string foragableType, out NativeForageYieldSnapshot snapshot)
	{
		snapshot = null;
		if (!NativeOrganicSourceSelectors.IsValidForageKey(foragableType))
		{
			return false;
		}

		var key = NativeOrganicSourceSelectors.NormaliseForageKey(foragableType);
		var profile = PeekForagableProfile();
		lock (_foragableYields)
		{
			return TryCreateNativeForageSnapshotLocked(profile, key, out snapshot);
		}
	}

	private bool TryCreateNativeForageSnapshotLocked(IForagableProfile profile, string key,
		out NativeForageYieldSnapshot snapshot)
	{
		snapshot = null;
		if (profile == null || !profile.MaximumYieldPoints.TryGetValue(key, out var maximum) ||
		    !double.IsFinite(maximum) || maximum <= 0.0)
		{
			return false;
		}

		// Project the same clamping/new-key rules as synchronisation without writing its result.
		var stock = _foragableYields.TryGetValue(key, out var existing)
			? double.IsFinite(existing) ? Math.Clamp(existing, 0.0, maximum) : 0.0
			: maximum;
		snapshot = new NativeForageYieldSnapshot(
			key,
			profile.Id,
			profile.RevisionNumber,
			profile.YieldDefinitionRevision,
			maximum,
			stock,
			_foragableYieldSourceRevision);
		return true;
	}

    public bool CanConsumeYield(string foragableType, double yield)
    {
        return !string.IsNullOrWhiteSpace(foragableType) &&
               double.IsFinite(yield) &&
               yield > 0.0 &&
               GetForagableYield(foragableType) + YieldComparisonTolerance >= yield;
    }

    public bool TryConsumeYield(string foragableType, double yield)
    {
        if (string.IsNullOrWhiteSpace(foragableType) || !double.IsFinite(yield) || yield <= 0.0)
        {
            return false;
        }

		SynchroniseForagableYields(ResolveForagableProfile());
		var changed = false;
        lock (_foragableYields)
        {
            var available = _foragableYields.GetValueOrDefault(foragableType);
            if (!double.IsFinite(available) || available + YieldComparisonTolerance < yield)
            {
                return false;
            }

            var current = Math.Max(0.0, available - yield);
            if (current == available)
            {
                return true;
            }

            _foragableYields[foragableType] = current;
			IncrementForagableYieldSourceRevision();
			changed = true;
        }

		if (changed)
		{
			NotifyForagableYieldChanged();
		}

		return true;
    }

	public bool TryConsumeYield(NativeForageYieldSnapshot expected, double yield, out string reason)
	{
		if (expected == null)
		{
			reason = "A forage yield snapshot is required.";
			return false;
		}

		if (!NativeOrganicAccountingMath.TryAmount(yield, out var exactAmount, out var amountError))
		{
			reason = amountError ?? "The forage debit amount is invalid.";
			return false;
		}

		var exactYield = (double)exactAmount;
		if (exactYield != yield)
		{
			reason = "The forage debit cannot be represented exactly by native accounting.";
			return false;
		}

		var profile = PeekForagableProfile();
		lock (_foragableYields)
		{
			if (!TryCreateNativeForageSnapshotLocked(profile, expected.Key, out var current))
			{
				reason = "The forage source is no longer available.";
				return false;
			}

			if (current != expected)
			{
				reason = "The forage source changed after it was inspected; inspect it again before retrying.";
				return false;
			}

			decimal currentAmount;
			try
			{
				currentAmount = (decimal)current.Stock;
			}
			catch (OverflowException)
			{
				reason = "The forage source stock cannot be represented by native accounting.";
				return false;
			}

			if (exactAmount > currentAmount)
			{
				reason = "The forage source does not contain enough stock for that exact debit.";
				return false;
			}

			var remainingAmount = currentAmount - exactAmount;
			var remaining = (double)remainingAmount;
			decimal roundTrippedRemaining;
			try
			{
				roundTrippedRemaining = (decimal)remaining;
			}
			catch (OverflowException)
			{
				reason = "The remaining forage stock cannot be represented by native accounting.";
				return false;
			}

			if (!double.IsFinite(remaining) || remaining < 0.0 || roundTrippedRemaining != remainingAmount ||
			    remaining == current.Stock)
			{
				reason = "The forage debit is too small to produce an exact native stock change.";
				return false;
			}

			_foragableYields[current.Key] = remaining;
			IncrementForagableYieldSourceRevision();
		}

		NotifyForagableYieldChanged();
		reason = string.Empty;
		return true;
	}

	public bool TryConsumeYieldBatch(IReadOnlyList<NativeForageDebitRequest> requests, out string reason)
	{
		if (requests.Count is < 1 or > 16 ||
		    requests.Select(x => x.Expected.Key).Distinct(StringComparer.Ordinal).Count() != requests.Count)
		{
			reason = "A forage debit group requires one to sixteen distinct keys.";
			return false;
		}
		var profile = PeekForagableProfile();
		var closing = new Dictionary<string, double>(StringComparer.Ordinal);
		lock (_foragableYields)
		{
			foreach (NativeForageDebitRequest request in requests)
			{
				if (request.Expected is null ||
				    !TryCreateNativeForageSnapshotLocked(profile, request.Expected.Key, out var current) ||
				    current != request.Expected)
				{
					reason = "A forage source changed before its complete owner group was consumed.";
					return false;
				}
				if (!NativeOrganicAccountingMath.TryAmount(request.Amount, out decimal debit, out string amountError) ||
				    (double)debit != request.Amount)
				{
					reason = amountError ?? "A forage group debit cannot be represented exactly.";
					return false;
				}
				decimal remaining;
				try
				{
					remaining = (decimal)current.Stock - debit;
				}
				catch (OverflowException)
				{
					reason = "A forage group stock cannot be represented by native accounting.";
					return false;
				}
				double output = (double)remaining;
				if (remaining < 0m || !double.IsFinite(output) || (decimal)output != remaining ||
				    output == current.Stock)
				{
					reason = "A forage group debit exceeds or cannot change its exact native stock.";
					return false;
				}
				closing.Add(current.Key, output);
			}
			foreach (var value in closing) _foragableYields[value.Key] = value.Value;
			IncrementForagableYieldSourceRevision();
		}
		NotifyForagableYieldChanged();
		reason = string.Empty;
		return true;
	}

    public void ConsumeYieldFor(IForagable foragable)
    {
        SynchroniseForagableProfile();
		var forageTypes = foragable.ForagableTypes.ToList();
        var changed = false;
		lock (_foragableYields)
		{
			foreach (var type in forageTypes.Where(type => _foragableYields.ContainsKey(type)))
			{
				var previous = _foragableYields[type];
				var current = Math.Max(0.0, previous - 1.0);
				_foragableYields[type] = current;
				changed |= current != previous;
			}

			if (changed)
			{
				IncrementForagableYieldSourceRevision();
			}
		}

        if (!changed)
        {
            return;
        }

		NotifyForagableYieldChanged();
    }

    public void ConsumeYield(string foragableType, double yield)
    {
        if (string.IsNullOrWhiteSpace(foragableType) || !double.IsFinite(yield) || yield <= 0.0)
        {
            return;
        }

		SynchroniseForagableYields(ResolveForagableProfile());
		var changed = false;
		lock (_foragableYields)
		{
			var previous = _foragableYields.GetValueOrDefault(foragableType);
			var current = Math.Max(0.0, previous - yield);
			if (current != previous)
			{
				_foragableYields[foragableType] = current;
				IncrementForagableYieldSourceRevision();
				changed = true;
			}
		}

		if (changed)
		{
			NotifyForagableYieldChanged();
		}
    }

	private static double GetMaxYield(IForagableProfile profile, string type)
    {
        if (!(profile?.MaximumYieldPoints.ContainsKey(type) ?? false))
        {
            return 0.0;
        }

        var yield = profile.MaximumYieldPoints[type];
        return double.IsFinite(yield) && yield > 0.0 ? yield : 0.0;
    }

	private static double GetHourlyYield(IForagableProfile profile, string type)
    {
        if (!(profile?.HourlyYieldPoints.ContainsKey(type) ?? false))
        {
            return 0.0;
        }

        var yield = profile.HourlyYieldPoints[type];
        return double.IsFinite(yield) && yield >= 0.0 ? yield : 0.0;
    }

	internal double PeekNativeForageHourlyProduction(string type) =>
		GetHourlyYield(PeekForagableProfile(), type);

	internal static NativeOrganicPenaltyContext BuildNativeForagePenaltyContext(string type,
		double stock, double maximum, double hourly) => new(
		NativeOrganicSourceKind.Forage,
		NativeOrganicSourceSelectors.Canonical(NativeOrganicSourceKind.Forage, type),
		stock, 0.0, stock, maximum, 0.0, hourly);

	private double GetForagableRecoveryIncrement(string type, double stock, double maximum, double hourly)
	{
		var headroom = Math.Max(0.0, maximum - stock);
		if (!double.IsFinite(headroom) || headroom <= 0.0 || !double.IsFinite(hourly) || hourly <= 0.0)
		{
			return 0.0;
		}

		var context = BuildNativeForagePenaltyContext(type, stock, maximum, hourly);
		var evaluation = Gameworld.EnvironmentalMagic?.EvaluateOrganicPenalty(
			this,
			NativeOrganicPenaltyChannel.ForageReplenishment,
			context) ?? NativeOrganicPenaltyEvaluation.Neutral;
		var factor = !evaluation.IsConfigured
			? 1.0
			: evaluation.IsValid && double.IsFinite(evaluation.Factor) && evaluation.Factor is >= 0.0 and <= 1.0
				? evaluation.Factor
				: 0.0;
		var production = hourly * factor;
		return double.IsFinite(production) && production >= 0.0
			? Math.Min(headroom, production)
			: 0.0;
	}

    private void YieldTick()
    {
		const int maximumAttempts = 4;
		for (var attempt = 0; attempt < maximumAttempts; attempt++)
		{
			var profile = ResolveForagableProfile();
			SynchroniseForagableYields(profile);
			if (profile == null)
			{
				RefreshForagableYieldSubscription();
				return;
			}

			long sourceRevision;
			List<(string Key, double Stock, double Maximum, double Hourly)> candidates;
			lock (_foragableYields)
			{
				sourceRevision = _foragableYieldSourceRevision;
				candidates = _foragableYields
					.Select(item => (item.Key, item.Value, GetMaxYield(profile, item.Key),
						GetHourlyYield(profile, item.Key)))
					.ToList();
			}

			var recoveries = candidates
				.Select(item => (item.Key, item.Stock, item.Maximum,
					Increment: GetForagableRecoveryIncrement(item.Key, item.Stock, item.Maximum, item.Hourly)))
				.ToList();
			var changed = false;
			lock (_foragableYields)
			{
				if (_foragableYieldSourceRevision != sourceRevision)
				{
					continue;
				}

				foreach (var recovery in recoveries)
				{
					var current = recovery.Increment >= recovery.Maximum - recovery.Stock
						? recovery.Maximum
						: Math.Min(recovery.Stock + recovery.Increment, recovery.Maximum);
					if (current == recovery.Stock)
					{
						continue;
					}

					_foragableYields[recovery.Key] = current;
					changed = true;
				}

				if (changed)
				{
					IncrementForagableYieldSourceRevision();
				}
			}

			if (changed)
			{
				NotifyForagableYieldChanged(refreshSubscription: false);
			}

			RefreshForagableYieldSubscription();
			return;
		}

		RefreshForagableYieldSubscription();
    }

    public IEnumerable<string> ForagableTypes
    {
        get
        {
            SynchroniseForagableYields(ResolveForagableProfile());
			lock (_foragableYields)
			{
				return _foragableYields.Keys.ToList();
			}
        }
    }

    public void PostLoadTasks(MudSharp.Models.Room room)
    {
        var profile = ResolveForagableProfile();
        if (profile == null)
        {
            return;
        }

		lock (_foragableYields)
		{
			foreach (var foragable in room.RoomsForagableYields)
			{
				_foragableYields[foragable.ForagableType] = foragable.Yield;
			}
		}

		SynchroniseForagableYields(profile, markChanged: false);
    }

    private void SynchroniseForagableYields(IForagableProfile profile, bool markChanged = true)
    {
        var profileId = profile?.Id ?? 0;
        var profileRevision = profile?.RevisionNumber ?? 0;
        var definitionRevision = profile?.YieldDefinitionRevision ?? 0;
		var validYields = profile?.MaximumYieldPoints
			.Where(x => double.IsFinite(x.Value) && x.Value > 0.0)
			.ToList() ?? [];
		var changed = false;
		lock (_foragableYields)
		{
			if (_foragableYieldProfileId == profileId && _foragableYieldProfileRevision == profileRevision &&
			    _foragableYieldDefinitionRevision == definitionRevision)
			{
				return;
			}

			if (profile == null)
			{
				if (_foragableYields.Any())
				{
					_foragableYields.Clear();
					changed = true;
				}
			}
			else
			{
				var validTypes = validYields.Select(x => x.Key)
					.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
				foreach (var type in _foragableYields.Keys.Where(x => !validTypes.Contains(x)).ToList())
				{
					_foragableYields.Remove(type);
					changed = true;
				}

				foreach (var yield in validYields)
				{
					if (!_foragableYields.TryGetValue(yield.Key, out var currentYield))
					{
						_foragableYields[yield.Key] = yield.Value;
						changed = true;
						continue;
					}

					var normalisedYield = double.IsFinite(currentYield)
						? Math.Clamp(currentYield, 0.0, yield.Value)
						: 0.0;
					if (normalisedYield != currentYield)
					{
						_foragableYields[yield.Key] = normalisedYield;
						changed = true;
					}
				}
			}

			_foragableYieldProfileId = profileId;
			_foragableYieldProfileRevision = profileRevision;
			_foragableYieldDefinitionRevision = definitionRevision;
			IncrementForagableYieldSourceRevision();
		}

		if (changed && markChanged)
		{
			YieldsChanged = true;
        }

        if (markChanged)
		{
			Gameworld.EnvironmentalMagic?.MarkDirty(this, EnvironmentalMagicDirtyReason.Forage);
		}

		RefreshForagableYieldSubscription();
    }

	private void NotifyForagableYieldChanged(bool refreshSubscription = true)
	{
		YieldsChanged = true;
		Gameworld.EnvironmentalMagic?.MarkDirty(this, EnvironmentalMagicDirtyReason.Forage);
		if (refreshSubscription)
		{
			RefreshForagableYieldSubscription();
		}
	}

	private void RefreshForagableYieldSubscription()
	{
		lock (ForagableYieldSubscriptionSync)
		{
			var profile = ResolveForagableProfile();
			bool requiresTick;
			lock (_foragableYields)
			{
				requiresTick = profile != null &&
				               _foragableYields.Any(item => GetMaxYield(profile, item.Key) > item.Value);
			}

			Gameworld.HeartbeatManager.HourHeartbeat -= YieldTick;
			if (requiresTick)
			{
				Gameworld.HeartbeatManager.HourHeartbeat += YieldTick;
			}
		}
	}

	private void IncrementForagableYieldSourceRevision()
	{
		_foragableYieldSourceRevision = _foragableYieldSourceRevision == long.MaxValue
			? 1L
			: _foragableYieldSourceRevision + 1L;
	}

    private readonly List<ILocalProject> _localProjects = new();
    public IEnumerable<ILocalProject> LocalProjects => _localProjects;

    public void AddProject(ILocalProject project)
    {
        _localProjects.Add(project);
    }

    public void RemoveProject(ILocalProject project)
    {
        _localProjects.Remove(project);
    }

    private readonly List<ITrack> _tracks = new();
    public IEnumerable<ITrack> Tracks => _tracks;

    public void AddTrack(ITrack track)
    {
        if (_tracks.Count == 0)
        {
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= TrackHeartbeat;
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += TrackHeartbeat;
        }
        _tracks.Add(track);
    }

    public void RemoveTrack(ITrack track)
    {
        _tracks.Remove(track);
        if (_tracks.Count == 0)
        {
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= TrackHeartbeat;
        }
    }

    private void TrackHeartbeat()
    {
        IWeatherEvent weather = CurrentWeather(null);
        double visualReduction = 0.0;
        double olfactoryReduction = 0.0;
        if (weather is null)
        {
            olfactoryReduction = Gameworld.GetStaticDouble("OlfactoryTrackReductionPerTickNone") +
                                  Gameworld.GetStaticDouble("OlfactoryTrackReductionPerTickParched");
            visualReduction = Gameworld.GetStaticDouble("VisualTrackReductionPerTickParched");
        }
        else
        {
            olfactoryReduction = Gameworld.GetStaticDouble($"OlfactoryTrackReductionPerTick{weather.Precipitation.DescribeEnum()}") +
                                 Gameworld.GetStaticDouble($"OlfactoryTrackReductionPerTick{weather.Wind.DescribeEnum()}");
            visualReduction = Gameworld.GetStaticDouble($"VisualTrackReductionPerTick{weather.Precipitation.DescribeEnum()}");
        }

        List<ITrack> toDeleteTracks = null;
        foreach (ITrack track in _tracks)
        {
            track.TrackIntensityOlfactory -= olfactoryReduction;
            track.TrackIntensityVisual -= visualReduction;
            if (track.TrackIntensityOlfactory <= 0.0 && track.TrackIntensityVisual <= 0.0)
            {
                (toDeleteTracks ??= []).Add(track);
            }
        }

		if (toDeleteTracks is null)
		{
			return;
		}

        HashSet<long> toDelete = toDeleteTracks.Select(x => x.Id).ToHashSet();
        using (new FMDB())
        {
            FMDB.Context.Tracks.Where(x => toDelete.Contains(x.Id)).ExecuteDelete();
        }

        _tracks.RemoveAll(x => toDelete.Contains(x.Id));
        foreach (ITrack track in toDeleteTracks)
        {
            Gameworld.Destroy(track);
            Gameworld.SaveManager.Abort(track);
            track.Deleted = true;
        }

        if (_tracks.Count <= 0)
        {
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= TrackHeartbeat;
        }
    }

    public void InitialiseTracks(IReadOnlyCollectionDictionary<IRoom, ITrack> tracks)
    {
        _tracks.AddRange(tracks[this]);
        if (_tracks.Count > 0)
        {
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= TrackHeartbeat;
            Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += TrackHeartbeat;
        }
    }

    #endregion

    #region Tags

    private readonly List<ITag> _tags = new();
    public IEnumerable<ITag> Tags => _tags;

    private bool _tagsChanged;

    public bool TagsChanged
    {
        get => _tagsChanged;
        set
        {
            if (value && !_tagsChanged)
            {
                Changed = true;
            }

            _tagsChanged = value;
        }
    }

    public bool AddTag(ITag tag)
    {
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
            TagsChanged = true;
            return true;
        }

        return false;
    }

    public bool RemoveTag(ITag tag)
    {
        if (_tags.Contains(tag))
        {
            _tags.Remove(tag);
            TagsChanged = true;
            return true;
        }

        return false;
    }

    public bool IsA(ITag tag)
    {
        return _tags.Any(x => x.IsA(tag));
    }

    private void SaveTags(MudSharp.Models.Room room)
    {
        FMDB.Context.RoomsTags.RemoveRange(room.RoomsTags);
        foreach (ITag tag in Tags)
        {
            room.RoomsTags.Add(new RoomsTags { Room = room, TagId = tag.Id });
        }

        _tagsChanged = false;
    }

    private void LoadTags(MudSharp.Models.Room room)
    {
        foreach (RoomsTags tag in room.RoomsTags)
        {
            ITag gtag = Gameworld.Tags.Get(tag.TagId);
            if (gtag != null)
            {
                _tags.Add(gtag);
            }
        }
    }

    #endregion
}
