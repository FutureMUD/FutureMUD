#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public sealed record VehicleItemSeedSpec(
	string Noun,
	string ShortDescription,
	string FullDescription,
	SizeCategory Size,
	ItemQuality Quality,
	double WeightInGrams,
	decimal Cost,
	string Material,
	string DestroyableComponent,
	bool Portable,
	bool Skinnable = true,
	bool HiddenFromPlayers = false,
	string? LongDescription = null);

public sealed record VehicleCompartmentSeedSpec(
	string Key,
	string Name,
	string Description,
	int DisplayOrder,
	long? InteriorTerrainId = null,
	int InteriorOutdoorsType = 0);

public sealed record VehicleCompartmentLinkSeedSpec(
	string Key,
	string SourceCompartmentKey,
	string DestinationCompartmentKey,
	string OutboundDirection,
	string InboundDirection,
	string OutboundDescription,
	string InboundDescription);

public sealed record VehicleOccupantSlotSeedSpec(
	string Key,
	string Name,
	string CompartmentKey,
	VehicleOccupantSlotType SlotType,
	int Capacity,
	bool RequiredForMovement,
	bool ContributesToPropulsion,
	Difficulty BoatStabilityDifficulty = Difficulty.Normal);

public sealed record VehicleControlStationSeedSpec(
	string Key,
	string Name,
	string OccupantSlotKey,
	bool IsPrimary);

public sealed record VehiclePropulsionSeedSpec(
	VehiclePropulsionType PropulsionType,
	bool IsDefault,
	double BaseMoveTimeMilliseconds,
	IReadOnlyCollection<string> TraitCandidates,
	Difficulty CheckDifficulty,
	string SpeedMultiplierExpression,
	string StaminaCostExpression,
	double RiderStaminaMultiplier = 1.0);

public sealed record VehicleMovementProfileSeedSpec(
	string Key,
	string Name,
	VehicleMovementProfileType MovementType,
	VehicleMovementEnvironment Environment,
	bool ExposesOccupantsToWater,
	bool IsDefault,
	double RequiredPowerSpikeInWatts,
	double MinimumEnginePowerInWatts,
	string? FuelLiquid,
	double FuelVolumePerMove,
	string RequiredInstalledRole,
	bool RequiresTowLinksClosed,
	bool RequiresAccessPointsClosed,
	double RouteSpeedMetresPerSecond,
	RouteVehiclePropulsionMode RoutePropulsionMode,
	double RouteFuelVolumePerMetre,
	double RoutePowerDrawWatts,
	bool AutomaticOperationCapable,
	IReadOnlyCollection<VehiclePropulsionSeedSpec> PropulsionProfiles);

public sealed record VehicleAccessPointSeedSpec(
	string Key,
	string Name,
	string Description,
	string? CompartmentKey,
	VehicleAccessPointType AccessPointType,
	bool StartsOpen,
	bool MustBeClosedForMovement,
	int DisplayOrder,
	VehicleItemSeedSpec ProjectionItem);

public sealed record VehicleCargoSpaceSeedSpec(
	string Key,
	string Name,
	string Description,
	string? CompartmentKey,
	string? RequiredAccessPointKey,
	int DisplayOrder,
	string ContainerComponent,
	VehicleItemSeedSpec ProjectionItem);

public sealed record VehicleInstallationPointSeedSpec(
	string Key,
	string Name,
	string Description,
	string? RequiredAccessPointKey,
	string MountType,
	string RequiredRole,
	bool RequiredForMovement,
	int DisplayOrder);

public sealed record VehicleTowPointSeedSpec(
	string Key,
	string Name,
	string Description,
	string? RequiredAccessPointKey,
	string TowType,
	bool CanTow,
	bool CanBeTowed,
	double MaximumTowedWeight,
	double CharacterPullMultiplier,
	double? TowStressWarningRatio,
	double? TowStressFailureStartRatio,
	double? TowStressMaximumFailureChance,
	double? TowStressDamageMultiplier,
	int DisplayOrder);

public sealed record VehicleDamageEffectSeedSpec(
	VehicleDamageEffectTargetType TargetType,
	string? TargetKey,
	VehicleSystemStatus MinimumStatus);

public sealed record VehicleDamageZoneSeedSpec(
	string Key,
	string Name,
	string Description,
	double MaximumDamage,
	double HitWeight,
	double DisabledThreshold,
	double DestroyedThreshold,
	bool DisablesMovement,
	int DisplayOrder,
	IReadOnlyCollection<VehicleDamageEffectSeedSpec> Effects);

public sealed record VehicleSeedSpec(
	string StableReference,
	string EraKey,
	string Domain,
	string Archetype,
	string Name,
	string Description,
	VehicleScale Scale,
	VehicleItemSeedSpec ExteriorItem,
	IReadOnlyCollection<VehicleCompartmentSeedSpec> Compartments,
	IReadOnlyCollection<VehicleCompartmentLinkSeedSpec> CompartmentLinks,
	IReadOnlyCollection<VehicleOccupantSlotSeedSpec> OccupantSlots,
	IReadOnlyCollection<VehicleControlStationSeedSpec> ControlStations,
	IReadOnlyCollection<VehicleMovementProfileSeedSpec> MovementProfiles,
	IReadOnlyCollection<VehicleAccessPointSeedSpec> AccessPoints,
	IReadOnlyCollection<VehicleCargoSpaceSeedSpec> CargoSpaces,
	IReadOnlyCollection<VehicleInstallationPointSeedSpec> InstallationPoints,
	IReadOnlyCollection<VehicleTowPointSeedSpec> TowPoints,
	IReadOnlyCollection<VehicleDamageZoneSeedSpec> DamageZones,
	bool ProvidesPassengerService,
	bool ProvidesCargoService,
	string? BuilderNotes = null,
	IReadOnlyCollection<string>? AdditionalEraKeys = null)
{
	public IReadOnlyCollection<string> SupportedEraKeys =>
		AdditionalEraKeys is null
			? [EraKey]
			: AdditionalEraKeys
				.Append(EraKey)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToArray();
}
