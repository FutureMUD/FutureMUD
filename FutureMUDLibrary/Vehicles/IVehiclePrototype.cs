using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.RPG.Checks;
using MudSharp.Combat;
using System;
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
	IEnumerable<IVehicleCompartmentLinkPrototype> CompartmentLinks =>
		Array.Empty<IVehicleCompartmentLinkPrototype>();
	bool CanCreateVehicle(out string reason);
}

public interface IVehicleCompartmentPrototype : IFrameworkItem
{
	string Description { get; }
	int DisplayOrder { get; }
	long? InteriorTerrainId => null;
#nullable enable annotations
	ITerrain? InteriorTerrain => null;
#nullable restore annotations
	CellOutdoorsType InteriorOutdoorsType => CellOutdoorsType.Indoors;
}

public interface IVehicleCompartmentLinkPrototype : IFrameworkItem
{
	IVehicleCompartmentPrototype SourceCompartment { get; }
	IVehicleCompartmentPrototype DestinationCompartment { get; }
	string OutboundDirection { get; }
	string InboundDirection { get; }
	string OutboundDescription { get; }
	string InboundDescription { get; }
}

public interface IVehicleOccupantSlotPrototype : IFrameworkItem
{
	IVehicleCompartmentPrototype Compartment { get; }
	VehicleOccupantSlotType SlotType { get; }
	int Capacity { get; }
	bool RequiredForMovement { get; }
	bool ContributesToPropulsion { get; }
	IRangedCover SameLevelRangedCover { get; }
	IRangedCover AboveRangedCover { get; }
	IRangedCover BelowRangedCover { get; }
	Difficulty BoatStabilityDifficulty { get; }
}

public interface IVehicleControlStationPrototype : IFrameworkItem
{
	IVehicleOccupantSlotPrototype OccupantSlot { get; }
	bool IsPrimary { get; }
}

public interface IVehicleMovementProfilePrototype : IFrameworkItem
{
	VehicleMovementProfileType MovementType { get; }
	VehicleMovementEnvironment MovementEnvironment { get; }
	bool ExposesOccupantsToWater { get; }
	bool IsDefault { get; }
	double RequiredPowerSpikeInWatts { get; }
	long? FuelLiquidId { get; }
	double FuelVolumePerMove { get; }
	string RequiredInstalledRole { get; }
	bool RequiresTowLinksClosed { get; }
	bool RequiresAccessPointsClosed { get; }
	IEnumerable<IVehiclePropulsionProfilePrototype> PropulsionProfiles { get; }

	/// <summary>
	/// Authored longitudinal speed for a Route movement profile. Non-route profiles retain zero.
	/// </summary>
	double RouteSpeedMetresPerSecond => 0.0;
	RouteVehiclePropulsionMode RoutePropulsionMode => RouteVehiclePropulsionMode.Powered;
	double RouteFuelVolumePerMetre => 0.0;
	double RoutePowerDrawWatts => 0.0;
	bool AutomaticOperationCapable => false;
}

public interface IVehiclePropulsionProfilePrototype : IFrameworkItem
{
	VehiclePropulsionType PropulsionType { get; }
	bool IsDefault { get; }
	double BaseMoveTimeMilliseconds { get; }
	ITraitDefinition PropulsionTrait { get; }
	Difficulty CheckDifficulty { get; }
	string SpeedMultiplierExpression { get; }
	string StaminaCostExpression { get; }
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
	double CharacterPullMultiplier { get; }
	double? TowStressWarningRatio { get; }
	double? TowStressFailureStartRatio { get; }
	double? TowStressMaximumFailureChance { get; }
	double? TowStressDamageMultiplier { get; }
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
