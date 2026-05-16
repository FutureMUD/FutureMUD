using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using System.Collections.Generic;

namespace MudSharp.Vehicles;

public interface IVehicle : IFrameworkItem, IHaveFuturemud, ISaveable
{
	IVehiclePrototype Prototype { get; }
	int PrototypeRevisionNumber { get; }
	IGameItem ExteriorItem { get; }
	long? ExteriorItemId { get; }
	IVehicleMovementState MovementState { get; }
	VehicleLocationType LocationType { get; }
	ICell Location { get; }
	RoomLayer RoomLayer { get; }
	IEnumerable<IVehicleOccupancy> Occupancies { get; }
	IEnumerable<ICharacter> Occupants { get; }
	IEnumerable<IVehicleAccessState> AccessStates { get; }
	IEnumerable<IVehicleAccessPoint> AccessPoints { get; }
	IEnumerable<IVehicleCargoSpace> CargoSpaces { get; }
	IEnumerable<IVehicleInstallation> Installations { get; }
	IEnumerable<IVehicleTowLink> TowLinks { get; }
	IEnumerable<IVehicleDamageZone> DamageZones { get; }
	IEnumerable<IGameItem> ProjectedTargetItems { get; }
	bool Disabled { get; }
	bool Destroyed { get; }
	ICharacter Controller { get; }
	IEnumerable<IVehicleDamageZone> DamageZonesDisabling(VehicleDamageEffectTargetType targetType, long? targetPrototypeId);
	bool IsDisabledByDamage(VehicleDamageEffectTargetType targetType, long? targetPrototypeId);
	string DamageDisabledReason(VehicleDamageEffectTargetType targetType, long? targetPrototypeId);
	bool IsOccupant(ICharacter actor);
	bool CanBoard(ICharacter actor, IVehicleOccupantSlotPrototype slot, out string reason);
	bool CanBoard(ICharacter actor, IVehicleOccupantSlotPrototype slot, IVehicleAccessPoint accessPoint, out string reason);
	bool Board(ICharacter actor, IVehicleOccupantSlotPrototype slot = null);
	bool Board(ICharacter actor, IVehicleOccupantSlotPrototype slot, IVehicleAccessPoint accessPoint);
	bool CanLeave(ICharacter actor, out string reason);
	bool Leave(ICharacter actor);
	bool CanMove(ICharacter actor, ICellExit exit, out string reason);
	bool Move(ICharacter actor, ICellExit exit);
	void MoveToCell(ICell destination, RoomLayer layer, ICellExit exit);
	void LinkExteriorItem(IGameItem item);
	void SynchroniseExteriorItemToLocation();
	IEnumerable<IWound> SufferDamage(IDamage damage);
	IEnumerable<IWound> PassiveSufferDamage(IDamage damage);
	IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing);
}

public interface IVehicleOccupancy : IFrameworkItem
{
	IVehicle Vehicle { get; }
	ICharacter Occupant { get; }
	IVehicleOccupantSlotPrototype Slot { get; }
	bool IsController { get; }
}

public interface IVehicleMovementState
{
	VehicleLocationType LocationType { get; }
	ICell Location { get; }
	RoomLayer RoomLayer { get; }
	VehicleMovementStatus MovementStatus { get; }
	long? CurrentExitId { get; }
	long? DestinationCellId { get; }
}

public interface IVehicleAccessState : IFrameworkItem
{
	IVehicle Vehicle { get; }
	ICharacter Character { get; }
	string AccessTag { get; }
	int AccessLevel { get; }
}

public interface IVehicleAccessPoint : IFrameworkItem
{
	IVehicle Vehicle { get; }
	IVehicleAccessPointPrototype Prototype { get; }
	IGameItem ProjectionItem { get; }
	long? ProjectionItemId { get; }
	bool IsOpen { get; }
	bool IsDisabled { get; }
	bool IsManuallyDisabled { get; }
	bool IsLocked { get; }
	IEnumerable<ILock> Locks { get; }
	bool CanUse(ICharacter actor, out string reason);
	void SetOpen(bool open);
	bool InstallLock(ILock theLock, ICharacter actor = null);
	bool RemoveLock(ILock theLock);
	void LinkProjectionItem(IGameItem item);
	void SetDisabled(bool disabled);
}

public interface IVehicleCargoSpace : IFrameworkItem
{
	IVehicle Vehicle { get; }
	IVehicleCargoSpacePrototype Prototype { get; }
	IGameItem ProjectionItem { get; }
	long? ProjectionItemId { get; }
	bool IsDisabled { get; }
	bool IsManuallyDisabled { get; }
	bool CanAccess(ICharacter actor, out string reason);
	void LinkProjectionItem(IGameItem item);
	void SetDisabled(bool disabled);
}

public interface IVehicleInstallation : IFrameworkItem
{
	IVehicle Vehicle { get; }
	IVehicleInstallationPointPrototype Prototype { get; }
	IGameItem InstalledItem { get; }
	long? InstalledItemId { get; }
	bool IsDisabled { get; }
	bool IsManuallyDisabled { get; }
	bool CanInstall(ICharacter actor, IGameItem item, out string reason);
	bool Install(ICharacter actor, IGameItem item);
	bool CanRemove(ICharacter actor, out string reason);
	IGameItem Remove(ICharacter actor);
	void SetDisabled(bool disabled);
}

public interface IVehicleTowLink : IFrameworkItem
{
	long SourceVehicleId { get; }
	long TargetVehicleId { get; }
	long SourceTowPointPrototypeId { get; }
	long TargetTowPointPrototypeId { get; }
	IVehicle SourceVehicle { get; }
	IVehicle TargetVehicle { get; }
	IVehicleTowPointPrototype SourceTowPoint { get; }
	IVehicleTowPointPrototype TargetTowPoint { get; }
	IGameItem HitchItem { get; }
	long? HitchItemId { get; }
	bool IsDisabled { get; }
	bool IsManuallyDisabled { get; }
	bool IsBroken { get; }
	string WhyInvalid { get; }
}

public interface IVehicleDamageZone : IFrameworkItem
{
	IVehicle Vehicle { get; }
	IVehicleDamageZonePrototype Prototype { get; }
	double CurrentDamage { get; }
	VehicleSystemStatus Status { get; }
	IEnumerable<IWound> Wounds { get; }
	void AddDamage(double amount);
	void SetStatus(VehicleSystemStatus status);
	void ClearWoundsAndDamage();
}
