using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using System.Collections.Generic;

namespace MudSharp.Vehicles;

public interface IVehiclePrototype : IEditableRevisableItem
{
	VehicleScale Scale { get; }
	long? ExteriorItemPrototypeId { get; }
	IGameItemProto ExteriorItemPrototype { get; }
	IEnumerable<IVehicleCompartmentPrototype> Compartments { get; }
	IEnumerable<IVehicleOccupantSlotPrototype> OccupantSlots { get; }
	IEnumerable<IVehicleControlStationPrototype> ControlStations { get; }
	IEnumerable<IVehicleMovementProfilePrototype> MovementProfiles { get; }
	IEnumerable<IVehicleAccessPointPrototype> AccessPoints { get; }
	IEnumerable<IVehicleCargoSpacePrototype> CargoSpaces { get; }
	IEnumerable<IVehicleInstallationPointPrototype> InstallationPoints { get; }
	IEnumerable<IVehicleTowPointPrototype> TowPoints { get; }
	IEnumerable<IVehicleDamageZonePrototype> DamageZones { get; }
	bool CanCreateVehicle(out string reason);
}

public interface IVehicleCompartmentPrototype : IFrameworkItem
{
	string Description { get; }
	int DisplayOrder { get; }
}

public interface IVehicleOccupantSlotPrototype : IFrameworkItem
{
	IVehicleCompartmentPrototype Compartment { get; }
	VehicleOccupantSlotType SlotType { get; }
	int Capacity { get; }
	bool RequiredForMovement { get; }
}

public interface IVehicleControlStationPrototype : IFrameworkItem
{
	IVehicleOccupantSlotPrototype OccupantSlot { get; }
	bool IsPrimary { get; }
}

public interface IVehicleMovementProfilePrototype : IFrameworkItem
{
	VehicleMovementProfileType MovementType { get; }
	bool IsDefault { get; }
	double RequiredPowerSpikeInWatts { get; }
	long? FuelLiquidId { get; }
	double FuelVolumePerMove { get; }
	string RequiredInstalledRole { get; }
	bool RequiresTowLinksClosed { get; }
	bool RequiresAccessPointsClosed { get; }
}

public interface IVehicleAccessPointPrototype : IFrameworkItem
{
	IVehicleCompartmentPrototype Compartment { get; }
	VehicleAccessPointType AccessPointType { get; }
	string Description { get; }
	long? ProjectionItemPrototypeId { get; }
	IGameItemProto ProjectionItemPrototype { get; }
	bool StartsOpen { get; }
	bool MustBeClosedForMovement { get; }
	int DisplayOrder { get; }
}

public interface IVehicleCargoSpacePrototype : IFrameworkItem
{
	IVehicleCompartmentPrototype Compartment { get; }
	IVehicleAccessPointPrototype RequiredAccessPoint { get; }
	string Description { get; }
	long? ProjectionItemPrototypeId { get; }
	IGameItemProto ProjectionItemPrototype { get; }
	int DisplayOrder { get; }
}

public interface IVehicleInstallationPointPrototype : IFrameworkItem
{
	IVehicleAccessPointPrototype RequiredAccessPoint { get; }
	string Description { get; }
	string MountType { get; }
	string RequiredRole { get; }
	bool RequiredForMovement { get; }
	int DisplayOrder { get; }
}

public interface IVehicleTowPointPrototype : IFrameworkItem
{
	IVehicleAccessPointPrototype RequiredAccessPoint { get; }
	string Description { get; }
	string TowType { get; }
	bool CanTow { get; }
	bool CanBeTowed { get; }
	double MaximumTowedWeight { get; }
	int DisplayOrder { get; }
}

public interface IVehicleDamageZonePrototype : IFrameworkItem
{
	string Description { get; }
	double MaximumDamage { get; }
	double HitWeight { get; }
	double DisabledThreshold { get; }
	double DestroyedThreshold { get; }
	bool DisablesMovement { get; }
	IEnumerable<IVehicleDamageZoneEffectPrototype> Effects { get; }
	int DisplayOrder { get; }
}

public interface IVehicleDamageZoneEffectPrototype : IFrameworkItem
{
	VehicleDamageEffectTargetType TargetType { get; }
	long? TargetPrototypeId { get; }
	VehicleSystemStatus MinimumStatus { get; }
}
