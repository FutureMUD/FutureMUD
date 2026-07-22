using MudSharp.Body;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.Movement;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public class Vehicle : SaveableItem, IVehicle
{
	private readonly List<IVehicleOccupancy> _occupancies = new();
	private readonly List<IVehicleCompartment> _compartments = new();
	private readonly List<IVehicleCompartmentLink> _compartmentLinks = new();
	private readonly List<VehicleDocking> _dockings = new();
	private readonly List<IVehicleAccessState> _accessStates = new();
	private readonly List<IVehicleAccessPoint> _accessPoints = new();
	private readonly List<IVehicleCargoSpace> _cargoSpaces = new();
	private readonly List<IVehicleInstallation> _installations = new();
	private readonly List<IVehicleTowLink> _towLinks = new();
	private readonly List<IVehicleDamageZone> _damageZones = new();
	private readonly CellExitVehicleMovementStrategy _cellExitMovementStrategy = new();
	private readonly IVehicleOperationalReadinessService _operationalReadinessService = new VehicleOperationalReadinessService();
	private readonly VehicleDockingService _dockingService = new();
	private bool _forceDisembarking;
	private long _prototypeId;
	private int _prototypeRevision;
	private long? _exteriorItemId;
	private IGameItem _exteriorItem;
	private VehicleLocationType _locationType;
	private long? _currentCellId;
	private RoomLayer _roomLayer;
	private VehicleMovementStatus _movementStatus;
	private long? _currentExitId;
	private long? _destinationCellId;
	private long? _movementProfileId;
	private long? _activePropulsionProfileId;
	private double? _routePositionMetres;
	private double? _destinationRoutePositionMetres;

	public Vehicle(DB.Vehicle dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_prototypeId = dbitem.VehicleProtoId;
		_prototypeRevision = dbitem.VehicleProtoRevision;
		_exteriorItemId = dbitem.ExteriorItemId;
		_locationType = (VehicleLocationType)dbitem.LocationType;
		_currentCellId = dbitem.CurrentCellId;
		_roomLayer = (RoomLayer)dbitem.CurrentRoomLayer;
		_movementStatus = (VehicleMovementStatus)dbitem.MovementStatus;
		_currentExitId = dbitem.CurrentExitId;
		_destinationCellId = dbitem.DestinationCellId;
		_movementProfileId = dbitem.MovementProfileProtoId;
		_activePropulsionProfileId = dbitem.ActivePropulsionProfileProtoId;
		if (dbitem.CurrentRoutePosition is { } persistedRoutePosition)
		{
			var route = Location?.RouteDefinition;
			var position = (double)persistedRoutePosition;
			if (route is null)
			{
				throw new System.IO.InvalidDataException(
					$"Vehicle #{dbitem.Id:N0} has a persisted RouteCell coordinate but is not located in a RouteCell.");
			}

			if (!double.IsFinite(position) || position < 0.0 || position > route.LengthMetres)
			{
				throw new System.IO.InvalidDataException(
					$"Vehicle #{dbitem.Id:N0} has invalid RouteCell coordinate {position:N3}m in Cell #{route.Cell.Id:N0}; valid coordinates are 0-{route.LengthMetres:N3}m.");
			}

			_routePositionMetres = position;
		}
		else
		{
			if (Location?.RouteDefinition is { } route)
			{
				throw new System.IO.InvalidDataException(
					$"Vehicle #{dbitem.Id:N0} is located in RouteCell #{route.Cell.Id:N0} but has no persisted route coordinate; use vehicle recovery to assign an explicit position.");
			}

			// Null is the legacy value for an ordinary-cell vehicle.
			_routePositionMetres = null;
		}

		foreach (var compartment in dbitem.Compartments.OrderBy(x => x.Id))
		{
			var runtimeCompartment = new VehicleCompartment(this, compartment);
			if (runtimeCompartment.Prototype is not null)
			{
				_compartments.Add(runtimeCompartment);
			}
		}

		foreach (var occupancy in dbitem.Occupancies)
		{
			var runtimeOccupancy = new VehicleOccupancy(this, occupancy);
			if (runtimeOccupancy.Occupant is not null && runtimeOccupancy.Slot is not null)
			{
				_occupancies.Add(runtimeOccupancy);
			}
		}

		foreach (var access in dbitem.AccessStates)
		{
			_accessStates.Add(new VehicleAccessState(this, access));
		}

		foreach (var accessPoint in dbitem.AccessPoints.OrderBy(x => x.Id))
		{
			var runtime = new VehicleAccessPoint(this, accessPoint);
			if (runtime.Prototype is not null)
			{
				_accessPoints.Add(runtime);
			}
		}

		RebuildCompartmentLinks();

		foreach (var docking in dbitem.Dockings.OrderBy(x => x.Id))
		{
			var runtime = new VehicleDocking(this, docking);
			if (runtime.AccessPoint is not null && runtime.Compartment is not null &&
			    runtime.ExteriorCell is not null)
			{
				_dockings.Add(runtime);
				runtime.BuildAndRegisterIfOpen();
			}
		}

		foreach (var cargoSpace in dbitem.CargoSpaces.OrderBy(x => x.Id))
		{
			var runtime = new VehicleCargoSpace(this, cargoSpace);
			if (runtime.Prototype is not null)
			{
				_cargoSpaces.Add(runtime);
			}
		}

		foreach (var installation in dbitem.Installations.OrderBy(x => x.Id))
		{
			var runtime = new VehicleInstallation(this, installation);
			if (runtime.Prototype is not null)
			{
				_installations.Add(runtime);
			}
		}

		foreach (var towLink in dbitem.SourceTowLinks.Concat(dbitem.TargetTowLinks).OrderBy(x => x.Id))
		{
			var runtime = new VehicleTowLink(gameworld, towLink);
			_towLinks.Add(runtime);
			HitchGearRules.Reserve(runtime.HitchItem, vehicleTowLinkId: runtime.Id);
		}

		foreach (var damageZone in dbitem.DamageZones.OrderBy(x => x.Id))
		{
			var runtime = new VehicleDamageZone(this, damageZone);
			if (runtime.Prototype is not null)
			{
				_damageZones.Add(runtime);
			}
		}
	}

	public override string FrameworkItemType => "Vehicle";
	public IVehiclePrototype Prototype => Gameworld.VehiclePrototypes.Get(_prototypeId, _prototypeRevision);
	public int PrototypeRevisionNumber => _prototypeRevision;

	public IGameItem ExteriorItem
	{
		get
		{
			if (_exteriorItem is not null)
			{
				return _exteriorItem;
			}

			_exteriorItem = _exteriorItemId is null ? null : Gameworld.TryGetItem(_exteriorItemId.Value, true);
			return _exteriorItem;
		}
	}

	public long? ExteriorItemId => _exteriorItemId;
	public IVehicleMovementProfilePrototype MovementProfile =>
		Prototype?.MovementProfiles.FirstOrDefault(x => x.Id == _movementProfileId) ??
		Prototype?.MovementProfiles
		          .Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
		          .OrderByDescending(x => x.IsDefault)
		          .FirstOrDefault();
	public IVehiclePropulsionProfilePrototype ActivePropulsionProfile =>
		MovementProfile?.PropulsionProfiles.FirstOrDefault(x => x.Id == _activePropulsionProfileId) ??
		MovementProfile?.PropulsionProfiles
		               .OrderByDescending(x => x.IsDefault)
		               .FirstOrDefault();
	public IVehicleMovementState MovementState => new VehicleMovementState(_locationType, Location, _roomLayer,
		_movementStatus, _currentExitId, _destinationCellId, _routePositionMetres,
		_destinationRoutePositionMetres);
	public VehicleLocationType LocationType => _locationType;
	public ICell Location => _currentCellId is null ? null : Gameworld.Cells.Get(_currentCellId.Value);
	public RoomLayer RoomLayer => _roomLayer;
	public double? RoutePositionMetres => _routePositionMetres;
	public IEnumerable<IVehicleOccupancy> Occupancies => _occupancies;
	public IEnumerable<ICharacter> Occupants => _occupancies.Select(x => x.Occupant).Where(x => x is not null);
	public IEnumerable<IVehicleAccessState> AccessStates => _accessStates;
	public IEnumerable<IVehicleAccessPoint> AccessPoints => _accessPoints;
	public IEnumerable<IVehicleCargoSpace> CargoSpaces => _cargoSpaces;
	public IEnumerable<IVehicleInstallation> Installations => _installations;
	public IEnumerable<IVehicleTowLink> TowLinks => _towLinks;
	public IEnumerable<IVehicleDamageZone> DamageZones => _damageZones;
	public IEnumerable<IVehicleCompartment> Compartments => _compartments;
	public IEnumerable<IVehicleDocking> Dockings => _dockings;
	internal IEnumerable<VehicleDocking> DockingsInternal => _dockings;
	public IEnumerable<IGameItem> ProjectedTargetItems => _accessPoints.Select(x => x.ProjectionItem)
	                                                                   .Concat(_cargoSpaces.Select(x => x.ProjectionItem))
	                                                                   .Concat(_installations.Select(x => x.InstalledItem))
	                                                                   .Where(x => x is not null && !x.Deleted && !x.Destroyed);
	public bool Destroyed => _movementStatus == VehicleMovementStatus.Destroyed ||
	                         _damageZones.Any(x => x.Status == VehicleSystemStatus.Destroyed && x.Prototype.DisablesMovement);
	public bool Disabled => Destroyed ||
	                        _damageZones.Any(x => x.Status != VehicleSystemStatus.Functional && x.Prototype.DisablesMovement) ||
	                        IsDisabledByDamage(VehicleDamageEffectTargetType.WholeVehicleMovement, null);
	public ICharacter Controller => _occupancies.FirstOrDefault(x => x.IsController)?.Occupant;

	internal VehicleCompartment CompartmentFor(IVehicleCompartmentPrototype prototype)
	{
		return prototype is null
			? null
			: _compartments
				.OfType<VehicleCompartment>()
				.FirstOrDefault(x => x.Prototype.Id == prototype.Id);
	}

	internal void AddDocking(VehicleDocking docking)
	{
		if (docking is not null && _dockings.All(x => x.Id != docking.Id))
		{
			_dockings.Add(docking);
		}
	}

	internal void RemoveDocking(long id)
	{
		_dockings.RemoveAll(x => x.Id == id);
	}

	public void SuspendDockings()
	{
		foreach (var docking in _dockings)
		{
			docking.Suspend();
		}
	}

	public void RebuildDockings()
	{
		_dockingService.RebuildDockings(this);
	}

	public void RebuildCompartmentLinks()
	{
		foreach (var link in _compartmentLinks.OfType<VehicleCompartmentLink>())
		{
			link.Remove();
		}

		_compartmentLinks.Clear();
		foreach (var compartment in _compartments.OfType<VehicleCompartment>())
		{
			compartment.ClearLinks();
		}

		var prototype = Gameworld.VehiclePrototypes?.Get(_prototypeId, _prototypeRevision);
		if (prototype?.Scale != VehicleScale.RoomScale)
		{
			return;
		}

		foreach (var linkPrototype in prototype.CompartmentLinks)
		{
			var source = CompartmentFor(linkPrototype.SourceCompartment);
			var destination = CompartmentFor(linkPrototype.DestinationCompartment);
			if (source is null || destination is null)
			{
				continue;
			}

			var link = new VehicleCompartmentLink(this, linkPrototype, source, destination);
			_compartmentLinks.Add(link);
			source.AddLink(link);
			destination.AddLink(link);
			link.Rebuild();
		}
	}

	public bool EnsureRoomScaleInteriors(out string reason)
	{
		if (Prototype.Scale != VehicleScale.RoomScale)
		{
			reason = string.Empty;
			return true;
		}

		foreach (var compartment in _compartments.OfType<VehicleCompartment>())
		{
			if (!RoomScaleVehicleInteriorService.TryCreateInterior(this, compartment, out reason))
			{
				return false;
			}
		}

		RebuildCompartmentLinks();
		RebuildDockings();
		reason = string.Empty;
		return true;
	}

	public bool IsInteriorOccupied(IVehicleCompartment compartment)
	{
		if (compartment is null)
		{
			return false;
		}

		return _occupancies.Any(x => x.Slot?.Compartment?.Id == compartment.Prototype.Id) ||
		       compartment.InteriorCell?.Characters.Any() == true ||
		       compartment.InteriorCell?.GameItems.Any() == true;
	}

	public bool IsHostedInterior(ICell cell)
	{
		return cell is not null && _compartments.Any(x => x.InteriorCell?.Id == cell.Id);
	}

	public void EchoHostedInteriors(string message)
	{
		if (Prototype.Scale != VehicleScale.RoomScale || string.IsNullOrWhiteSpace(message))
		{
			return;
		}

		foreach (var cell in _compartments.Select(x => x.InteriorCell).Where(x => x is not null).Distinct())
		{
			cell.HandleRoomEcho(message, RoomLayer.GroundLevel);
		}
	}

	public bool CanRetire(out string reason)
	{
		var occupied = _compartments.FirstOrDefault(IsInteriorOccupied);
		if (occupied is not null)
		{
			reason = $"The {occupied.Name} hosted interior still contains occupants or loose contents.";
			return false;
		}

		if (((IVehicle)this).ActiveJourney is not null)
		{
			reason = "The vehicle has an active journey.";
			return false;
		}

		using (new FMDB())
		{
			if (FMDB.Context.VehicleServices.Any(x => x.VehicleId == Id))
			{
				reason = "The vehicle is still assigned to a vehicle service. Reassign or remove that service first.";
				return false;
			}
		}

		if (_movementStatus == VehicleMovementStatus.Moving)
		{
			reason = "The vehicle is currently moving.";
			return false;
		}

		if (_towLinks.Any() || (Gameworld.VehicleHitchLinks ?? Enumerable.Empty<IVehicleHitchLink>())
			.Any(x => x.SourceVehicleId == Id || x.TargetVehicleId == Id))
		{
			reason = "The vehicle still has a tow or hitch connection.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool Retire(out string reason)
	{
		if (!CanRetire(out reason))
		{
			return false;
		}

		var interiorRooms = _compartments
			.Select(x => x.InteriorCell?.Room)
			.Where(x => x is not null)
			.Distinct()
			.Cast<Room>()
			.ToList();
		using (new FMDB())
		{
			using var transaction = FMDB.Context.Database.BeginTransaction();
			try
			{
				var dockings = FMDB.Context.VehicleDockings.Where(x => x.VehicleId == Id).ToList();
				FMDB.Context.VehicleDockings.RemoveRange(dockings);
				foreach (var compartment in FMDB.Context.VehicleCompartments.Where(x => x.VehicleId == Id))
				{
					compartment.InteriorCellId = null;
				}

				foreach (var cell in FMDB.Context.Cells.Where(x => x.HostedVehicleId == Id))
				{
					cell.HostedVehicleId = null;
					cell.HostedVehicleCompartmentId = null;
				}

				var dbitem = FMDB.Context.Vehicles.Find(Id);
				if (dbitem is not null)
				{
					FMDB.Context.Vehicles.Remove(dbitem);
				}
				FMDB.Context.SaveChanges();
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}

		foreach (var docking in _dockings)
		{
			docking.Suspend(false);
		}
		foreach (var link in _compartmentLinks.OfType<VehicleCompartmentLink>())
		{
			link.Remove();
		}

		foreach (var room in interiorRooms)
		{
			room.DestroyRoom(Location);
		}

		ExteriorItem?.GetItemType<IVehicleExterior>()?.ClearVehicleLink("The vehicle instance was retired.");
		Gameworld.SaveManager.Abort(this);
		Gameworld.Destroy(this);
		reason = string.Empty;
		return true;
	}

	public bool SetActivePropulsionProfile(IVehiclePropulsionProfilePrototype profile, out string reason)
	{
		if (_movementStatus != VehicleMovementStatus.Stationary)
		{
			reason = "The vehicle must be stationary before changing its propulsion mode.";
			return false;
		}

		if (profile is null || MovementProfile?.PropulsionProfiles.All(x => x.Id != profile.Id) != false)
		{
			reason = "That propulsion mode is not supported by this vehicle's current movement profile.";
			return false;
		}

		_activePropulsionProfileId = profile.Id;
		Changed = true;
		reason = string.Empty;
		return true;
	}

	public IEnumerable<IVehicleDamageZone> DamageZonesDisabling(VehicleDamageEffectTargetType targetType,
		long? targetPrototypeId)
	{
		return _damageZones.Where(zone =>
			zone.Prototype.Effects.Any(effect =>
				effect.TargetType == targetType &&
				(effect.TargetPrototypeId is null || targetPrototypeId is null ||
				 effect.TargetPrototypeId.Value == targetPrototypeId.Value) &&
				zone.Status >= effect.MinimumStatus));
	}

	public bool IsDisabledByDamage(VehicleDamageEffectTargetType targetType, long? targetPrototypeId)
	{
		return DamageZonesDisabling(targetType, targetPrototypeId).Any();
	}

	public string DamageDisabledReason(VehicleDamageEffectTargetType targetType, long? targetPrototypeId)
	{
		var zone = DamageZonesDisabling(targetType, targetPrototypeId).FirstOrDefault();
		return zone is null
			? string.Empty
			: $"{zone.Name} is {zone.Status.DescribeEnum().ToLowerInvariant()}";
	}

	public bool IsOccupant(ICharacter actor)
	{
		return _occupancies.Any(x => x.Occupant?.SamePhysicalInstance(actor) == true);
	}

	public bool CanBoard(ICharacter actor, IVehicleOccupantSlotPrototype slot, out string reason)
	{
		slot ??= FirstAvailableSlot();
		var access = ResolveBoardingAccess(actor, slot, null);
		if (_accessPoints.Any() && access is null)
		{
			reason = Prototype.Scale == VehicleScale.RoomScale
				? "There is no open docking access point for that vehicle slot at your location."
				: "There is no open and usable access point for that vehicle slot.";
			return false;
		}

		return CanBoardCore(actor, slot, access, out reason);
	}

	private bool CanBoardCore(ICharacter actor, IVehicleOccupantSlotPrototype slot,
		IVehicleAccessPoint accessPoint, out string reason)
	{
		if (_movementStatus != VehicleMovementStatus.Stationary)
		{
			reason = "You cannot board a vehicle while it is moving.";
			return false;
		}

		if (actor is null)
		{
			reason = "There is no such character.";
			return false;
		}

		if (IsOccupant(actor))
		{
			reason = "You are already aboard that vehicle.";
			return false;
		}

		if (actor.Gameworld.Vehicles.Any(x => !ReferenceEquals(x, this) && x.IsOccupant(actor)))
		{
			reason = "You are already aboard another vehicle.";
			return false;
		}

		if (Prototype.Scale == VehicleScale.RoomScale)
		{
			if (accessPoint is null || ActiveDockingForBoarding(actor, accessPoint) is null)
			{
				reason = "You must be at an open docking access point to board that room-scale vehicle.";
				return false;
			}
		}
		else if (Location is not null && (actor.Location != Location || actor.RoomLayer != _roomLayer))
		{
			reason = "You must be in the same location and room layer as the vehicle to board it.";
			return false;
		}

		if (!_operationalReadinessService.CanPerformAction(this, actor, VehicleOperationalAction.Board, out var accessResult))
		{
			reason = accessResult.Reason;
			return false;
		}

		if (slot is null)
		{
			reason = "There are no available occupant slots on that vehicle.";
			return false;
		}

		if (Prototype.Scale == VehicleScale.RoomScale &&
		    CompartmentFor(slot.Compartment)?.InteriorCell is null)
		{
			reason = $"The {slot.Compartment.Name} hosted interior is unavailable. An administrator must recover it before boarding.";
			return false;
		}

		if (_occupancies.Count(x => x.Slot.Id == slot.Id) >= slot.Capacity)
		{
			reason = "That vehicle slot is already full.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool CanBoard(ICharacter actor, IVehicleOccupantSlotPrototype slot, IVehicleAccessPoint accessPoint, out string reason)
	{
		slot ??= FirstAvailableSlot();
		if (_accessPoints.Any() && accessPoint is null)
		{
			accessPoint = ResolveBoardingAccess(actor, slot, null);
			if (accessPoint is null)
			{
				reason = Prototype.Scale == VehicleScale.RoomScale
					? "There is no open docking access point for that vehicle slot at your location."
					: "There is no open and usable access point for that vehicle slot.";
				return false;
			}
		}

		if (accessPoint is not null && !_accessPoints.Contains(accessPoint))
		{
			reason = "That access point does not belong to this vehicle.";
			return false;
		}

		if (accessPoint is not null && !this.AccessPointCanReachSlot(accessPoint, slot))
		{
			reason = "That access point does not lead to that vehicle slot.";
			return false;
		}

		if (accessPoint is not null && !accessPoint.CanUse(actor, out reason))
		{
			return false;
		}

		return CanBoardCore(actor, slot, accessPoint, out reason);
	}

	public bool Board(ICharacter actor, IVehicleOccupantSlotPrototype slot = null)
	{
		slot ??= FirstAvailableSlot();
		var access = _accessPoints.Any() ? FirstViableAccessPoint(actor, slot) : null;
		return Board(actor, slot, access);
	}

	public bool Board(ICharacter actor, IVehicleOccupantSlotPrototype slot, IVehicleAccessPoint accessPoint)
	{
		slot ??= FirstAvailableSlot();
		accessPoint = ResolveBoardingAccess(actor, slot, accessPoint);
		if (!CanBoard(actor, slot, accessPoint, out _))
		{
			return false;
		}

		ICell boardingDestination = null;
		if (Prototype.Scale == VehicleScale.RoomScale)
		{
			var docking = ActiveDockingForBoarding(actor, accessPoint);
			boardingDestination = DockingExitFrom(docking, actor.Location)?.Destination;
			if (boardingDestination is null || docking?.Compartment.InteriorCell?.Id != boardingDestination.Id)
			{
				return false;
			}

			actor.Teleport(boardingDestination, RoomLayer.GroundLevel, false, false);
		}

		var isController = slot.SlotType == VehicleOccupantSlotType.Driver &&
		                   Prototype.ControlStations.Any(x => x.OccupantSlot.Id == slot.Id) &&
		                   Controller is null &&
		                   this.IsAtOccupantSlotLocation(actor, slot);
		using (new FMDB())
		{
			var dbitem = new DB.VehicleOccupancy
			{
				VehicleId = Id,
				CharacterId = CharacterInstanceIdentityComparer.IdentityId(actor),
				CharacterInstanceId = CharacterInstanceIdentityComparer.InstanceId(actor),
				VehicleOccupantSlotProtoId = slot.Id,
				IsController = isController
			};
			FMDB.Context.VehicleOccupancies.Add(dbitem);
			FMDB.Context.SaveChanges();
			_occupancies.Add(new VehicleOccupancy(this, dbitem));
		}

		return true;
	}

	public bool CanTakeControl(ICharacter actor, out string reason)
	{
		var occupancy = _occupancies.FirstOrDefault(x => x.Occupant?.SamePhysicalInstance(actor) == true);
		if (occupancy is null)
		{
			reason = "You must be aboard that vehicle to take control of it.";
			return false;
		}

		if (occupancy.Slot.SlotType != VehicleOccupantSlotType.Driver)
		{
			reason = "You must occupy a driver slot to take control of that vehicle.";
			return false;
		}

		if (Prototype.ControlStations.All(x => x.OccupantSlot.Id != occupancy.Slot.Id))
		{
			reason = "Your driver slot does not have a configured control station.";
			return false;
		}

		if (!this.IsAtOccupantSlotLocation(actor, occupancy.Slot))
		{
			reason = $"You must be in the {occupancy.Slot.Compartment.Name} compartment at its control station to take control of that vehicle.";
			return false;
		}

		if (!_operationalReadinessService.CanPerformAction(this, actor, VehicleOperationalAction.Control,
			    out var accessResult))
		{
			reason = accessResult.Reason;
			return false;
		}

		if (Controller?.SamePhysicalInstance(actor) == true)
		{
			reason = "You are already controlling that vehicle.";
			return false;
		}

		if (Controller is not null)
		{
			reason = $"{Controller.HowSeen(actor, true)} is already controlling that vehicle.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool TakeControl(ICharacter actor)
	{
		if (!CanTakeControl(actor, out _))
		{
			return false;
		}

		var occupancy = _occupancies.First(x => x.Occupant?.SamePhysicalInstance(actor) == true);
		SetControllerOccupancy(occupancy.Id);
		return true;
	}

	public bool ReleaseControl(ICharacter actor)
	{
		var occupancy = _occupancies.FirstOrDefault(x => x.IsController &&
		                                                     x.Occupant?.SamePhysicalInstance(actor) == true);
		if (occupancy is null)
		{
			return false;
		}

		SetControllerOccupancy(null);
		return true;
	}

	private void SetControllerOccupancy(long? occupancyId)
	{
		using (new FMDB())
		{
			foreach (var dbitem in FMDB.Context.VehicleOccupancies.Where(x => x.VehicleId == Id))
			{
				dbitem.IsController = dbitem.Id == occupancyId;
			}

			FMDB.Context.SaveChanges();
		}

		for (var i = 0; i < _occupancies.Count; i++)
		{
			var occupancy = _occupancies[i];
			_occupancies[i] = new VehicleOccupancy(this, occupancy.Id, occupancy.Occupant,
				occupancy.CharacterInstanceId, occupancy.Slot, occupancy.Id == occupancyId);
		}
	}

	public bool CanLeave(ICharacter actor, out string reason)
	{
		if (!IsOccupant(actor))
		{
			reason = "You are not aboard that vehicle.";
			return false;
		}

		if (_movementStatus == VehicleMovementStatus.Moving)
		{
			reason = "You cannot leave that vehicle while it is moving.";
			return false;
		}

		if (_accessPoints.Any())
		{
			if (Prototype.Scale == VehicleScale.RoomScale)
			{
				if (ActiveDockingForDisembark(actor) is null)
				{
					reason = "There is no open docking exit from your current compartment.";
					return false;
				}
			}
			else
			{
				var occupancy = _occupancies.FirstOrDefault(x => x.Occupant?.SamePhysicalInstance(actor) == true);
				var access = FirstViableAccessPoint(actor, occupancy?.Slot);
				if (access is null)
				{
					reason = "There is no open and usable access point you can use to leave that vehicle.";
					return false;
				}
			}
		}

		reason = string.Empty;
		return true;
	}

	public bool Leave(ICharacter actor)
	{
		if (!CanLeave(actor, out _))
		{
			return false;
		}

		var occupancy = _occupancies.First(x => x.Occupant?.SamePhysicalInstance(actor) == true);
		if (Prototype.Scale == VehicleScale.RoomScale)
		{
			var docking = ActiveDockingForDisembark(actor);
			var dockingExit = DockingExitFrom(docking, actor.Location);
			if (docking is null || dockingExit?.Destination.Id != docking.ExteriorCell.Id)
			{
				return false;
			}

			actor.Teleport(dockingExit.Destination, docking.ExteriorLayer, false, false);
			actor.Body.Look(true);
		}
		else
		{
			SynchroniseOccupantWithVehicle(actor);
		}
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleOccupancies.Find(occupancy.Id);
			if (dbitem is not null)
			{
				FMDB.Context.VehicleOccupancies.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		_occupancies.Remove(occupancy);
		NormaliseDisembarkedOccupant(actor);
		return true;
	}

	public void ForceDisembarkAll()
	{
		if (_forceDisembarking)
		{
			return;
		}

		_forceDisembarking = true;
		try
		{
			SetStationaryAfterForcedExteriorChange();
			foreach (var occupancy in _occupancies.ToList())
			{
				SynchroniseOccupantWithVehicle(occupancy.Occupant);
				ForceDisembark(occupancy.Occupant);
			}

			Changed = true;
		}
		finally
		{
			_forceDisembarking = false;
		}
	}

	public void ForceDisembark(ICharacter actor, bool cancelMovement = true)
	{
		var occupancy = _occupancies.FirstOrDefault(x => x.Occupant?.SamePhysicalInstance(actor) == true);
		if (occupancy is null)
		{
			return;
		}

		if (cancelMovement)
		{
			ClearForcedOccupantMovement(occupancy.Occupant);
		}

		SynchroniseOccupantWithVehicle(occupancy.Occupant);

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleOccupancies.Find(occupancy.Id);
			if (dbitem is not null)
			{
				FMDB.Context.VehicleOccupancies.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		_occupancies.Remove(occupancy);
		NormaliseDisembarkedOccupant(occupancy.Occupant);
		Changed = true;
	}

	private void SynchroniseOccupantWithVehicle(ICharacter occupant)
	{
		if (occupant is null || Location is null)
		{
			return;
		}

		if (Prototype.Scale == VehicleScale.RoomScale)
		{
			if (occupant.Location != Location || occupant.RoomLayer != _roomLayer)
			{
				occupant.Teleport(Location, _roomLayer, false, false);
			}

			return;
		}

		if (!this.IsSurfaceWaterVehicle() ||
		    occupant.Location == Location && occupant.RoomLayer == _roomLayer)
		{
			return;
		}

		occupant.Teleport(Location, _roomLayer, false, false);
	}

	private void NormaliseDisembarkedOccupant(ICharacter occupant)
	{
		if (!this.IsSurfaceWaterVehicle() || occupant?.Location?.IsSwimmingLayer(occupant.RoomLayer) != true)
		{
			return;
		}

		occupant.SetPosition(PositionSwimming.Instance, PositionModifier.None, null, null);
	}

	public IVehicleAccessState GrantAccess(ICharacter character, string accessTag, int accessLevel)
	{
		if (character is null)
		{
			throw new ArgumentNullException(nameof(character));
		}

		accessTag = NormaliseAccessTag(accessTag);
		accessLevel = Math.Clamp(accessLevel, 1, 3);
		var characterId = CharacterInstanceIdentityComparer.IdentityId(character);
		DB.VehicleAccessState dbitem;
		using (new FMDB())
		{
			dbitem = FMDB.Context.VehicleAccessStates.FirstOrDefault(x =>
				x.VehicleId == Id && x.CharacterId == characterId && x.AccessTag == accessTag);
			if (dbitem is null)
			{
				dbitem = new DB.VehicleAccessState
				{
					VehicleId = Id,
					CharacterId = characterId,
					AccessTag = accessTag,
					AccessLevel = accessLevel
				};
				FMDB.Context.VehicleAccessStates.Add(dbitem);
			}
			else
			{
				dbitem.AccessLevel = accessLevel;
			}

			FMDB.Context.SaveChanges();
		}

		_accessStates.RemoveAll(x => x.Id == dbitem.Id);
		var runtime = new VehicleAccessState(this, dbitem);
		_accessStates.Add(runtime);
		return runtime;
	}

	public bool RevokeAccess(long accessStateId)
	{
		var removed = false;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleAccessStates.Find(accessStateId);
			if (dbitem is not null && dbitem.VehicleId == Id)
			{
				FMDB.Context.VehicleAccessStates.Remove(dbitem);
				FMDB.Context.SaveChanges();
				removed = true;
			}
		}

		if (removed)
		{
			_accessStates.RemoveAll(x => x.Id == accessStateId);
		}

		return removed;
	}

	public int RevokeAccess(ICharacter character, string accessTag = null)
	{
		if (character is null)
		{
			return 0;
		}

		accessTag = string.IsNullOrWhiteSpace(accessTag) ? null : NormaliseAccessTag(accessTag);
		var characterId = CharacterInstanceIdentityComparer.IdentityId(character);
		List<long> ids;
		using (new FMDB())
		{
			var rows = FMDB.Context.VehicleAccessStates
			               .Where(x => x.VehicleId == Id && x.CharacterId == characterId)
			               .ToList();
			if (!string.IsNullOrWhiteSpace(accessTag))
			{
				rows = rows.Where(x => x.AccessTag == accessTag).ToList();
			}

			ids = rows.Select(x => x.Id).ToList();
			if (rows.Any())
			{
				FMDB.Context.VehicleAccessStates.RemoveRange(rows);
				FMDB.Context.SaveChanges();
			}
		}

		_accessStates.RemoveAll(x => ids.Contains(x.Id));
		return ids.Count;
	}

	private static string NormaliseAccessTag(string accessTag)
	{
		return string.IsNullOrWhiteSpace(accessTag) ? "all" : accessTag.Trim().ToLowerInvariant();
	}
	private void SetStationaryAfterForcedExteriorChange()
	{
		_locationType = VehicleLocationType.Cell;
		if (_movementStatus != VehicleMovementStatus.Destroyed)
		{
			_movementStatus = VehicleMovementStatus.Stationary;
		}
		_currentExitId = null;
		_destinationCellId = null;
	}

	private static void ClearForcedOccupantMovement(ICharacter occupant)
	{
		occupant?.Movement?.CancelForMoverOnly(occupant);
	}

	public bool CanMove(ICharacter actor, ICellExit exit, out string reason)
	{
		return _cellExitMovementStrategy.CanMove(this, actor, exit, out reason);
	}

	public bool Move(ICharacter actor, ICellExit exit)
	{
		return _cellExitMovementStrategy.Move(this, actor, exit);
	}

	public void LinkExteriorItem(IGameItem item)
	{
		_exteriorItem = item;
		_exteriorItemId = item?.Id;
		if (Prototype.Scale == VehicleScale.RoomScale)
		{
			if (item is null)
			{
				SuspendDockings();
			}
			else
			{
				RebuildDockings();
			}
		}
		Changed = true;
	}

	public void BeginMoveAlongRoute(double destinationPositionMetres)
	{
		if (Location?.RouteDefinition is not { } route)
		{
			throw new InvalidOperationException("The vehicle is not in a RouteCell.");
		}

		_destinationRoutePositionMetres = Math.Clamp(destinationPositionMetres, 0.0, route.LengthMetres);
		_locationType = VehicleLocationType.Route;
		_movementStatus = VehicleMovementStatus.Moving;
		SuspendDockings();
		Changed = true;
	}

	public void MaterialiseRoutePosition(double positionMetres, bool stationary = false)
	{
		if (Location?.RouteDefinition is not { } route)
		{
			throw new InvalidOperationException("The vehicle is not in a RouteCell.");
		}

		_routePositionMetres = Math.Clamp(positionMetres, 0.0, route.LengthMetres);
		_locationType = VehicleLocationType.Route;
		SynchroniseExteriorRoutePosition(_routePositionMetres);
		if (stationary)
		{
			_movementStatus = VehicleMovementStatus.Stationary;
			_destinationRoutePositionMetres = null;
			RebuildDockings();
		}

		Changed = true;
	}

	public void SynchroniseExteriorItemToLocation()
	{
		var isRoomScale = Gameworld.VehiclePrototypes?.Get(_prototypeId, _prototypeRevision)?.Scale ==
		                  VehicleScale.RoomScale;
		EnsureExteriorProjectionLink(out _, out _);
		if (ExteriorItem is null || Location is null)
		{
			if (isRoomScale)
			{
				SuspendDockings();
			}
			return;
		}

		if ((Destroyed || ExteriorItem.Deleted || ExteriorItem.Destroyed) &&
		    this.IsSurfaceWaterVehicle() &&
		    _occupancies.Any())
		{
			ForceDisembarkAll();
		}

		if (ExteriorItem.Location == Location && ExteriorItem.RoomLayer == _roomLayer)
		{
			SynchroniseExteriorRoutePosition(_routePositionMetres);
			EnsureExteriorWaterPosition();
			if (isRoomScale)
			{
				RebuildDockings();
			}
			return;
		}

		ExteriorItem.Location?.Extract(ExteriorItem);
		ExteriorItem.RoomLayer = _roomLayer;
		Location.Insert(ExteriorItem, true);
		SynchroniseExteriorRoutePosition(_routePositionMetres);
		EnsureExteriorWaterPosition();
		if (isRoomScale)
		{
			RebuildDockings();
		}
		Changed = true;
	}

	/// <summary>
	/// Restores a missing item-side link from this vehicle's canonical exterior-item ID.
	/// A conflicting non-null link is deliberately left alone so that fleet recovery can
	/// report it instead of silently stealing another vehicle's projection.
	/// </summary>
	internal bool EnsureExteriorProjectionLink(out bool repaired, out string reason)
	{
		repaired = false;
		if (ExteriorItem is null)
		{
			reason = "The canonical exterior item is missing.";
			return false;
		}

		var exterior = ExteriorItem.GetItemType<IVehicleExterior>();
		if (exterior is null)
		{
			reason = "The canonical exterior item does not have a vehicle exterior component.";
			return false;
		}

		if (exterior.VehicleId == Id)
		{
			reason = string.Empty;
			return true;
		}

		if (exterior.VehicleId is not null)
		{
			reason = $"The canonical exterior item is linked to vehicle #{exterior.VehicleId:N0} instead.";
			return false;
		}

		exterior.LinkVehicle(this);
		repaired = true;
		reason = string.Empty;
		return true;
	}

	private void EnsureExteriorWaterPosition()
	{
		if (this.KeepsExteriorAfloat() && ExteriorItem.PositionState != PositionFloatingInWater.Instance)
		{
			ExteriorItem.PositionState = PositionFloatingInWater.Instance;
		}
	}

	public void BeginMoveToCell(ICell destination, RoomLayer layer, ICellExit exit)
	{
		if (Gameworld.VehiclePrototypes?.Get(_prototypeId, _prototypeRevision)?.Scale == VehicleScale.RoomScale)
		{
			SuspendDockings();
		}
		_locationType = VehicleLocationType.CellExitTransit;
		_movementStatus = VehicleMovementStatus.Moving;
		_currentExitId = exit?.Exit.Id;
		_destinationCellId = destination?.Id;
		Changed = true;
		Gameworld.SaveManager.Flush();
	}

	public void MoveToCell(ICell destination, RoomLayer layer, ICellExit exit, IMovement movement = null)
	{
		var origin = Location;
		BeginMoveToCell(destination, layer, exit);

		if (ExteriorItem is not null)
		{
			origin?.Extract(ExteriorItem);
			ExteriorItem.RoomLayer = layer;
			destination.Insert(ExteriorItem, true);
			ForceExteriorConnectablesMoved();
		}

		foreach (var occupant in Occupants.ToList())
		{
			if (Prototype.Scale == VehicleScale.RoomScale)
			{
				continue;
			}

			if (origin is not null && (occupant.Location != origin || occupant.RoomLayer != _roomLayer))
			{
				ForceDisembark(occupant);
				continue;
			}

			origin?.Leave(occupant);
			occupant.RoomLayer = layer;
			occupant.Moved(movement);
			destination.Enter(occupant, exit, roomLayer: layer);
			if (movement is null)
			{
				occupant.Body.Look(true);
			}
		}

		_currentCellId = destination.Id;
		_roomLayer = layer;
		_routePositionMetres = destination.RouteDefinition is null
			? null
			: destination.RouteDefinition.ExitAnchors
				.FirstOrDefault(x => x.Exit.Exit.Id == exit?.Exit.Id)?.ArrivalPositionMetres ??
			  destination.RouteDefinition.DefaultPositionMetres;
		SynchroniseExteriorRoutePosition(_routePositionMetres);
		_locationType = destination.RouteDefinition is null
			? VehicleLocationType.Cell
			: VehicleLocationType.Route;
		_movementStatus = VehicleMovementStatus.Stationary;
		_currentExitId = null;
		_destinationCellId = null;
		EnsureExteriorWaterPosition();
		if (Prototype.Scale == VehicleScale.RoomScale)
		{
			RebuildDockings();
		}
		Changed = true;
	}

	private void SynchroniseExteriorRoutePosition(double? positionMetres)
	{
		if (ExteriorItem is null)
		{
			return;
		}

		var exterior = ExteriorItem.GetItemType<IVehicleExterior>();
		if (exterior?.TrySynchroniseRoutePosition(this, positionMetres) == true)
		{
			return;
		}

		// A missing or conflicting projection link must not grant the canonical-sync exemption.
		// The ordinary setter deliberately invokes forced-move handling so fleet recovery can
		// diagnose the displaced or incorrectly linked projection.
		ExteriorItem.SetRoutePosition(positionMetres);
	}

	private void ForceExteriorConnectablesMoved()
	{
		if (ExteriorItem is null)
		{
			return;
		}

		foreach (var connectable in ExteriorItem.GetItemTypes<IConnectable>())
		{
			connectable.ForceMove();
		}
	}

	public void HandleExteriorItemForceMoved()
	{
		if (ExteriorItem is null || ExteriorItem.Deleted || ExteriorItem.Destroyed ||
		    ExteriorItem.InInventoryOf is not null || ExteriorItem.ContainedIn is not null ||
		    ExteriorItem.Location is null)
		{
			if (Prototype.Scale == VehicleScale.RoomScale)
			{
				SetStationaryAfterForcedExteriorChange();
				SuspendDockings();
				Changed = true;
				return;
			}

			ForceDisembarkAll();
			return;
		}

		var origin = Location;
		var destination = ExteriorItem.Location;
		var layer = ExteriorItem.RoomLayer;
		SetStationaryAfterForcedExteriorChange();
		foreach (var occupant in Occupants.ToList())
		{
			if (Prototype.Scale == VehicleScale.RoomScale)
			{
				ClearForcedOccupantMovement(occupant);
				continue;
			}

			ClearForcedOccupantMovement(occupant);
			origin?.Leave(occupant);
			occupant.RoomLayer = layer;
			destination.Enter(occupant, null, roomLayer: layer);
		}

		_currentCellId = destination.Id;
		_roomLayer = layer;
		EnsureExteriorWaterPosition();
		if (Prototype.Scale == VehicleScale.RoomScale)
		{
			RebuildDockings();
		}
		Changed = true;
	}

	public void RecoverInterruptedMovement()
	{
		if (_movementStatus != VehicleMovementStatus.Moving && _locationType != VehicleLocationType.CellExitTransit)
		{
			return;
		}

		_locationType = VehicleLocationType.Cell;
		_movementStatus = VehicleMovementStatus.Stationary;
		_currentExitId = null;
		_destinationCellId = null;
		Changed = true;
		SynchroniseExteriorItemToLocation();
		if (Gameworld.VehiclePrototypes?.Get(_prototypeId, _prototypeRevision)?.Scale == VehicleScale.RoomScale)
		{
			RebuildCompartmentLinks();
			RebuildDockings();
			return;
		}

		if (Location is null)
		{
			return;
		}

		foreach (var occupant in Occupants.ToList())
		{
			if (occupant.Location != Location || occupant.RoomLayer != _roomLayer)
			{
				occupant.Teleport(Location, _roomLayer, false, false);
			}
		}
	}

	private IVehicleOccupantSlotPrototype FirstAvailableSlot()
	{
		return Prototype.OccupantSlots.FirstOrDefault(slot => _occupancies.Count(x => x.Slot.Id == slot.Id) < slot.Capacity);
	}

	private IVehicleAccessPoint FirstViableAccessPoint(ICharacter actor, IVehicleOccupantSlotPrototype slot)
	{
		return _accessPoints
		       .Where(x => this.AccessPointCanReachSlot(x, slot))
		       .Where(x => x.CanUse(actor, out _))
		       .FirstOrDefault(x => Prototype.Scale != VehicleScale.RoomScale ||
		                            ActiveDockingForBoarding(actor, x) is not null);
	}

	internal IVehicleAccessPoint ResolveBoardingAccess(ICharacter actor, IVehicleOccupantSlotPrototype slot,
		IVehicleAccessPoint requestedAccess)
	{
		return requestedAccess ?? (_accessPoints.Any() ? FirstViableAccessPoint(actor, slot) : null);
	}

	private VehicleDocking ActiveDockingForBoarding(ICharacter actor, IVehicleAccessPoint accessPoint)
	{
		return actor is null || accessPoint is null
			? null
			: _dockings.FirstOrDefault(x =>
				x.IsRegistered &&
				x.State == VehicleDockingState.BoardingOpen &&
				x.AccessPoint.Id == accessPoint.Id &&
				x.ExteriorCell.Id == actor.Location?.Id &&
				x.ExteriorLayer == actor.RoomLayer);
	}

	private VehicleDocking ActiveDockingForDisembark(ICharacter actor)
	{
		return actor is null
			? null
			: _dockings.FirstOrDefault(x =>
				x.IsRegistered &&
				x.State == VehicleDockingState.BoardingOpen &&
				x.Compartment.InteriorCell?.Id == actor.Location?.Id &&
				x.AccessPoint.CanUse(actor, out _));
	}

	internal static ICellExit DockingExitFrom(IVehicleDocking docking, ICell origin)
	{
		return docking?.State == VehicleDockingState.BoardingOpen
			? docking.TransientExit?.CellExitFor(origin)
			: null;
	}

	public IEnumerable<IWound> SufferDamage(IDamage damage)
	{
		var wounds = PassiveSufferDamage(damage).ToList();
		wounds.ProcessPassiveWounds();
		return wounds;
	}

	public IEnumerable<IWound> PassiveSufferDamage(IDamage damage)
	{
		if (damage is null || ExteriorItem is null || !_damageZones.Any())
		{
			return Enumerable.Empty<IWound>();
		}

		var zone = ChooseDamageZone();
		if (zone is null)
		{
			return Enumerable.Empty<IWound>();
		}

		var wound = new SimpleWound(Gameworld, ExteriorItem, damage.DamageAmount, damage.DamageType, null,
			damage.LodgableItem, damage.ToolOrigin, damage.ActorOrigin, Id, zone.Id);
		((VehicleDamageZone)zone).AddWound(wound);
		zone.AddDamage(damage.DamageAmount);
		return [wound];
	}

	public IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing)
	{
		return damage?.ReferenceDamages.SelectMany(PassiveSufferDamage).ToList() ?? Enumerable.Empty<IWound>();
	}

	public void AddVehicleWound(IWound wound)
	{
		if (wound is null)
		{
			return;
		}

		if (_damageZones.Any(x => x.Wounds.Contains(wound)))
		{
			return;
		}

		var zone = ChooseDamageZone();
		if (zone is null)
		{
			return;
		}

		((VehicleDamageZone)zone).AddWound(wound);
		zone.AddDamage(wound.CurrentDamage);
	}

	public void CureAllVehicleWounds()
	{
		foreach (var zone in _damageZones.OfType<VehicleDamageZone>())
		{
			zone.ClearWoundsAndDamage();
		}
	}

	public void AddTowLink(IVehicleTowLink link)
	{
		if (link is not null && _towLinks.All(x => x.Id != link.Id))
		{
			_towLinks.Add(link);
		}
	}

	public void RemoveTowLink(long id)
	{
		_towLinks.RemoveAll(x => x.Id == id);
	}

	private IVehicleDamageZone ChooseDamageZone()
	{
		return _damageZones
		       .Where(x => x.Prototype.HitWeight > 0.0)
		       .DefaultIfEmpty(_damageZones.FirstOrDefault())
		       .GetWeightedRandom(x => x?.Prototype.HitWeight ?? 0.0);
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Vehicles.Find(Id);
			if (dbitem is null)
			{
				Changed = false;
				return;
			}

			dbitem.Name = Name;
			dbitem.VehicleProtoId = _prototypeId;
			dbitem.VehicleProtoRevision = _prototypeRevision;
			dbitem.ExteriorItemId = _exteriorItemId;
			dbitem.LocationType = (int)_locationType;
			dbitem.CurrentCellId = _currentCellId;
			dbitem.CurrentRoomLayer = (int)_roomLayer;
			dbitem.MovementStatus = (int)_movementStatus;
			dbitem.CurrentExitId = _currentExitId;
			dbitem.DestinationCellId = _destinationCellId;
			dbitem.MovementProfileProtoId = _movementProfileId;
			dbitem.ActivePropulsionProfileProtoId = _activePropulsionProfileId;
			dbitem.CurrentRoutePosition = _routePositionMetres is null
				? null
				: (decimal)_routePositionMetres.Value;
			dbitem.LastMovementDateTime = DateTime.UtcNow;
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}
}

public class VehicleOccupancy : FrameworkItem, IVehicleOccupancy
{
	public VehicleOccupancy(IVehicle vehicle, DB.VehicleOccupancy dbitem)
	{
		Vehicle = vehicle;
		_id = dbitem.Id;
		_name = $"Vehicle Occupancy #{dbitem.Id:N0}";
		CharacterInstanceId = dbitem.CharacterInstanceId;
		Occupant = vehicle.Gameworld.ResolveActorReference(new CharacterActorReference(
			dbitem.CharacterId,
			dbitem.CharacterInstanceId,
			ReferenceKind: dbitem.CharacterInstanceId is > 0
				? CharacterActorReferenceKind.SpecificInstance
				: CharacterActorReferenceKind.IdentityOnly)).Actor;
		Slot = vehicle.Prototype.OccupantSlots.FirstOrDefault(x => x.Id == dbitem.VehicleOccupantSlotProtoId);
		IsController = dbitem.IsController;
	}

	internal VehicleOccupancy(IVehicle vehicle, long id, ICharacter occupant, long? characterInstanceId,
		IVehicleOccupantSlotPrototype slot, bool isController)
	{
		Vehicle = vehicle;
		_id = id;
		_name = $"Vehicle Occupancy #{id:N0}";
		Occupant = occupant;
		CharacterInstanceId = characterInstanceId;
		Slot = slot;
		IsController = isController;
	}

	public override string FrameworkItemType => "VehicleOccupancy";
	public IVehicle Vehicle { get; }
	public ICharacter Occupant { get; }
	public long? CharacterInstanceId { get; }
	public IVehicleOccupantSlotPrototype Slot { get; }
	public bool IsController { get; }
}

public class VehicleMovementState : IVehicleMovementState
{
	public VehicleMovementState(VehicleLocationType locationType, ICell location, RoomLayer roomLayer,
		VehicleMovementStatus movementStatus, long? currentExitId, long? destinationCellId,
		double? routePositionMetres = null, double? destinationRoutePositionMetres = null)
	{
		LocationType = locationType;
		Location = location;
		RoomLayer = roomLayer;
		MovementStatus = movementStatus;
		CurrentExitId = currentExitId;
		DestinationCellId = destinationCellId;
		RoutePositionMetres = routePositionMetres;
		DestinationRoutePositionMetres = destinationRoutePositionMetres;
	}

	public VehicleLocationType LocationType { get; }
	public ICell Location { get; }
	public RoomLayer RoomLayer { get; }
	public VehicleMovementStatus MovementStatus { get; }
	public long? CurrentExitId { get; }
	public long? DestinationCellId { get; }
	public double? RoutePositionMetres { get; }
	public double? DestinationRoutePositionMetres { get; }
}

public class VehicleAccessState : FrameworkItem, IVehicleAccessState
{
	public VehicleAccessState(IVehicle vehicle, DB.VehicleAccessState dbitem)
	{
		Vehicle = vehicle;
		_id = dbitem.Id;
		_name = $"Vehicle Access #{dbitem.Id:N0}";
		Character = dbitem.CharacterId is null ? null : vehicle.Gameworld.TryGetCharacter(dbitem.CharacterId.Value);
		AccessTag = dbitem.AccessTag;
		AccessLevel = dbitem.AccessLevel;
	}

	public override string FrameworkItemType => "VehicleAccessState";
	public IVehicle Vehicle { get; }
	public ICharacter Character { get; }
	public string AccessTag { get; }
	public int AccessLevel { get; }
}
