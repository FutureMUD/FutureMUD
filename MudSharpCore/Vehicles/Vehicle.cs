using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using System;
using System.Collections.Generic;
using System.Linq;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public class Vehicle : SaveableItem, IVehicle
{
	private readonly List<IVehicleOccupancy> _occupancies = new();
	private readonly List<IVehicleAccessState> _accessStates = new();
	private readonly List<IVehicleAccessPoint> _accessPoints = new();
	private readonly List<IVehicleCargoSpace> _cargoSpaces = new();
	private readonly List<IVehicleInstallation> _installations = new();
	private readonly List<IVehicleTowLink> _towLinks = new();
	private readonly List<IVehicleDamageZone> _damageZones = new();
	private readonly CellExitVehicleMovementStrategy _cellExitMovementStrategy = new();
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
			_towLinks.Add(new VehicleTowLink(gameworld, towLink));
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
	public IVehicleMovementState MovementState => new VehicleMovementState(_locationType, Location, _roomLayer, _movementStatus, _currentExitId, _destinationCellId);
	public VehicleLocationType LocationType => _locationType;
	public ICell Location => _currentCellId is null ? null : Gameworld.Cells.Get(_currentCellId.Value);
	public RoomLayer RoomLayer => _roomLayer;
	public IEnumerable<IVehicleOccupancy> Occupancies => _occupancies;
	public IEnumerable<ICharacter> Occupants => _occupancies.Select(x => x.Occupant).Where(x => x is not null);
	public IEnumerable<IVehicleAccessState> AccessStates => _accessStates;
	public IEnumerable<IVehicleAccessPoint> AccessPoints => _accessPoints;
	public IEnumerable<IVehicleCargoSpace> CargoSpaces => _cargoSpaces;
	public IEnumerable<IVehicleInstallation> Installations => _installations;
	public IEnumerable<IVehicleTowLink> TowLinks => _towLinks;
	public IEnumerable<IVehicleDamageZone> DamageZones => _damageZones;
	public IEnumerable<IGameItem> ProjectedTargetItems => _accessPoints.Select(x => x.ProjectionItem)
	                                                                   .Concat(_cargoSpaces.Select(x => x.ProjectionItem))
	                                                                   .Concat(_installations.Select(x => x.InstalledItem))
	                                                                   .Where(x => x is not null);
	public bool Destroyed => _movementStatus == VehicleMovementStatus.Destroyed ||
	                         _damageZones.Any(x => x.Status == VehicleSystemStatus.Destroyed && x.Prototype.DisablesMovement);
	public bool Disabled => Destroyed ||
	                        _damageZones.Any(x => x.Status != VehicleSystemStatus.Functional && x.Prototype.DisablesMovement) ||
	                        IsDisabledByDamage(VehicleDamageEffectTargetType.WholeVehicleMovement, null);
	public ICharacter Controller => _occupancies.FirstOrDefault(x => x.IsController)?.Occupant;

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
		return _occupancies.Any(x => x.Occupant == actor);
	}

	public bool CanBoard(ICharacter actor, IVehicleOccupantSlotPrototype slot, out string reason)
	{
		slot ??= FirstAvailableSlot();
		if (!CanBoardCore(actor, slot, out reason))
		{
			return false;
		}

		if (!_accessPoints.Any())
		{
			return true;
		}

		var access = FirstViableAccessPoint(actor, slot);
		if (access is not null)
		{
			return true;
		}

		reason = "There is no open and usable access point for that vehicle slot.";
		return false;
	}

	private bool CanBoardCore(ICharacter actor, IVehicleOccupantSlotPrototype slot, out string reason)
	{
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

		if (Location is not null && actor.Location != Location)
		{
			reason = "You must be in the same location as the vehicle to board it.";
			return false;
		}

		if (Location is not null && actor.RoomLayer != _roomLayer)
		{
			reason = "You must be on the same room layer as the vehicle to board it.";
			return false;
		}

		if (_accessStates.Any() && !_accessStates.Any(x => x.Character == actor))
		{
			reason = "You do not have access to board that vehicle.";
			return false;
		}

		if (slot is null)
		{
			reason = "There are no available occupant slots on that vehicle.";
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
		if (!CanBoardCore(actor, slot, out reason))
		{
			return false;
		}

		if (!_accessPoints.Any())
		{
			return true;
		}

		if (accessPoint is null)
		{
			accessPoint = FirstViableAccessPoint(actor, slot);
			if (accessPoint is null)
			{
				reason = "There is no open and usable access point for that vehicle slot.";
				return false;
			}
		}

		if (!_accessPoints.Contains(accessPoint))
		{
			reason = "That access point does not belong to this vehicle.";
			return false;
		}

		if (!AccessPointCanReachSlot(accessPoint, slot))
		{
			reason = "That access point does not lead to that vehicle slot.";
			return false;
		}

		return accessPoint.CanUse(actor, out reason);
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
		if (!CanBoard(actor, slot, accessPoint, out _))
		{
			return false;
		}

		var isController = slot.SlotType == VehicleOccupantSlotType.Driver && Controller is null;
		using (new FMDB())
		{
			var dbitem = new DB.VehicleOccupancy
			{
				VehicleId = Id,
				CharacterId = actor.Id,
				VehicleOccupantSlotProtoId = slot.Id,
				IsController = isController
			};
			FMDB.Context.VehicleOccupancies.Add(dbitem);
			FMDB.Context.SaveChanges();
			_occupancies.Add(new VehicleOccupancy(this, dbitem));
		}

		return true;
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
			var occupancy = _occupancies.FirstOrDefault(x => x.Occupant == actor);
			var access = FirstViableAccessPoint(actor, occupancy?.Slot);
			if (access is null)
			{
				reason = "There is no open and usable access point you can use to leave that vehicle.";
				return false;
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

		var occupancy = _occupancies.First(x => x.Occupant == actor);
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
		return true;
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
		Changed = true;
	}

	public void SynchroniseExteriorItemToLocation()
	{
		if (ExteriorItem is null || Location is null)
		{
			return;
		}

		if (ExteriorItem.Location == Location && ExteriorItem.RoomLayer == _roomLayer)
		{
			return;
		}

		ExteriorItem.Location?.Extract(ExteriorItem);
		ExteriorItem.RoomLayer = _roomLayer;
		Location.Insert(ExteriorItem, true);
		Changed = true;
	}

	public void MoveToCell(ICell destination, RoomLayer layer, ICellExit exit)
	{
		var origin = Location;
		_locationType = VehicleLocationType.CellExitTransit;
		_movementStatus = VehicleMovementStatus.Moving;
		_currentExitId = exit?.Exit.Id;
		_destinationCellId = destination?.Id;
		Changed = true;
		Gameworld.SaveManager.Flush();

		if (ExteriorItem is not null)
		{
			origin?.Extract(ExteriorItem);
			ExteriorItem.RoomLayer = layer;
			destination.Insert(ExteriorItem, true);
		}

		foreach (var occupant in Occupants.ToList())
		{
			occupant.Teleport(destination, layer, false, false);
		}

		_currentCellId = destination.Id;
		_roomLayer = layer;
		_locationType = VehicleLocationType.Cell;
		_movementStatus = VehicleMovementStatus.Stationary;
		_currentExitId = null;
		_destinationCellId = null;
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
		       .Where(x => AccessPointCanReachSlot(x, slot))
		       .FirstOrDefault(x => x.CanUse(actor, out _));
	}

	private static bool AccessPointCanReachSlot(IVehicleAccessPoint accessPoint, IVehicleOccupantSlotPrototype slot)
	{
		return accessPoint.Prototype.Compartment is null ||
		       slot?.Compartment is null ||
		       accessPoint.Prototype.Compartment.Id == slot.Compartment.Id;
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
		Occupant = vehicle.Gameworld.TryGetCharacter(dbitem.CharacterId);
		Slot = vehicle.Prototype.OccupantSlots.FirstOrDefault(x => x.Id == dbitem.VehicleOccupantSlotProtoId);
		IsController = dbitem.IsController;
	}

	public override string FrameworkItemType => "VehicleOccupancy";
	public IVehicle Vehicle { get; }
	public ICharacter Occupant { get; }
	public IVehicleOccupantSlotPrototype Slot { get; }
	public bool IsController { get; }
}

public class VehicleMovementState : IVehicleMovementState
{
	public VehicleMovementState(VehicleLocationType locationType, ICell location, RoomLayer roomLayer, VehicleMovementStatus movementStatus, long? currentExitId, long? destinationCellId)
	{
		LocationType = locationType;
		Location = location;
		RoomLayer = roomLayer;
		MovementStatus = movementStatus;
		CurrentExitId = currentExitId;
		DestinationCellId = destinationCellId;
	}

	public VehicleLocationType LocationType { get; }
	public ICell Location { get; }
	public RoomLayer RoomLayer { get; }
	public VehicleMovementStatus MovementStatus { get; }
	public long? CurrentExitId { get; }
	public long? DestinationCellId { get; }
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
