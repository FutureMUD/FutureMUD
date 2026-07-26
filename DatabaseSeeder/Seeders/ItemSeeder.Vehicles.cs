#nullable enable

using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;


public partial class ItemSeeder
{
	private const string VehicleSeederMarkerPrefix = "Item seeder vehicle key";
	private const string VehicleDomainTerrestrial = "Terrestrial";
	private const string VehicleDomainAquatic = "Aquatic";
	private const string VehicleOutboardMountType = "outboard_motor";
	private const string VehiclePropulsionRole = "propulsion";

	private static readonly IReadOnlyDictionary<string, string> VehicleEraTags =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["antiquity"] = "Era / Antiquity Era",
			["medieval"] = "Era / Medieval Era",
			["renaissance"] = "Era / Renaissance Era",
			["earlymodern"] = "Era / Early Modern Era",
			["revolution"] = "Era / Industrial Era",
			["modern"] = "Era / Modern Era",
			["atomic"] = "Era / Nuclear Era",
			["computer"] = "Era / Information Age Era"
		};

	private sealed record VehicleItemSeedSpec(
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

	private sealed record VehicleCompartmentSeedSpec(
		string Key,
		string Name,
		string Description,
		int DisplayOrder,
		long? InteriorTerrainId = null,
		int InteriorOutdoorsType = 0);

	private sealed record VehicleCompartmentLinkSeedSpec(
		string Key,
		string SourceCompartmentKey,
		string DestinationCompartmentKey,
		string OutboundDirection,
		string InboundDirection,
		string OutboundDescription,
		string InboundDescription);

	private sealed record VehicleOccupantSlotSeedSpec(
		string Key,
		string Name,
		string CompartmentKey,
		VehicleOccupantSlotType SlotType,
		int Capacity,
		bool RequiredForMovement,
		bool ContributesToPropulsion,
		Difficulty BoatStabilityDifficulty = Difficulty.Normal);

	private sealed record VehicleControlStationSeedSpec(
		string Key,
		string Name,
		string OccupantSlotKey,
		bool IsPrimary);

	private sealed record VehiclePropulsionSeedSpec(
		VehiclePropulsionType PropulsionType,
		bool IsDefault,
		double BaseMoveTimeMilliseconds,
		IReadOnlyCollection<string> TraitCandidates,
		Difficulty CheckDifficulty,
		string SpeedMultiplierExpression,
		string StaminaCostExpression);

	private sealed record VehicleMovementProfileSeedSpec(
		string Key,
		string Name,
		VehicleMovementProfileType MovementType,
		VehicleMovementEnvironment Environment,
		bool ExposesOccupantsToWater,
		bool IsDefault,
		double RequiredPowerSpikeInWatts,
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

	private sealed record VehicleAccessPointSeedSpec(
		string Key,
		string Name,
		string Description,
		string? CompartmentKey,
		VehicleAccessPointType AccessPointType,
		bool StartsOpen,
		bool MustBeClosedForMovement,
		int DisplayOrder,
		VehicleItemSeedSpec ProjectionItem);

	private sealed record VehicleCargoSpaceSeedSpec(
		string Key,
		string Name,
		string Description,
		string? CompartmentKey,
		string? RequiredAccessPointKey,
		int DisplayOrder,
		string ContainerComponent,
		VehicleItemSeedSpec ProjectionItem);

	private sealed record VehicleInstallationPointSeedSpec(
		string Key,
		string Name,
		string Description,
		string? RequiredAccessPointKey,
		string MountType,
		string RequiredRole,
		bool RequiredForMovement,
		int DisplayOrder);

	private sealed record VehicleTowPointSeedSpec(
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

	private sealed record VehicleDamageEffectSeedSpec(
		VehicleDamageEffectTargetType TargetType,
		string? TargetKey,
		VehicleSystemStatus MinimumStatus);

	private sealed record VehicleDamageZoneSeedSpec(
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

	private sealed record VehicleSeedSpec(
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
		string? BuilderNotes = null);

	internal sealed record VehicleExampleSummaryForTesting(
		string StableReference,
		string EraKey,
		string Domain,
		string Archetype,
		VehicleScale Scale,
		int CompartmentCount,
		int OccupantSlotCount,
		int PrimaryControlStationCount,
		int MovementProfileCount,
		bool HasDriverSlot,
		bool HasSurfaceWaterMovement,
		bool HasExplicitPropulsion,
		bool HasMotorInstallation,
		bool HasCargoProjection,
		bool HasAccessProjection);

	internal static IReadOnlyList<VehicleExampleSummaryForTesting> VehicleExamplesForTesting =>
		VehicleExampleSpecs.Select(x => new VehicleExampleSummaryForTesting(
			x.StableReference,
			x.EraKey,
			x.Domain,
			x.Archetype,
			x.Scale,
			x.Compartments.Count,
			x.OccupantSlots.Count,
			x.ControlStations.Count(station => station.IsPrimary),
			x.MovementProfiles.Count,
			x.OccupantSlots.Any(slot => slot.SlotType == VehicleOccupantSlotType.Driver),
			x.MovementProfiles.Any(profile => profile.Environment == VehicleMovementEnvironment.SurfaceWater),
			x.MovementProfiles.SelectMany(profile => profile.PropulsionProfiles).Any(),
			x.InstallationPoints.Any(point =>
				point.MountType.Equals(VehicleOutboardMountType, StringComparison.OrdinalIgnoreCase)),
			x.CargoSpaces.Any(),
			x.AccessPoints.Any())).ToList();

	internal static IReadOnlyCollection<string> ParseVehicleEraTokensForTesting(string? eras)
	{
		return ParseVehicleEraTokens(eras);
	}

	internal static void ValidateVehicleExamplesForTesting()
	{
		foreach (var spec in VehicleExampleSpecs)
		{
			ValidateVehicleSeedSpec(spec);
		}
	}

	private static IReadOnlyCollection<string> ParseVehicleEraTokens(string? eras)
	{
		return string.IsNullOrWhiteSpace(eras)
			? []
			: eras.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Select(x => x.ToLowerInvariant())
				.Where(VehicleEraTags.ContainsKey)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private void SeedVehicleItemsAndPrototypes(string eras)
	{
		var selectedEras = ParseVehicleEraTokens(eras);
		if (selectedEras.Count == 0)
		{
			return;
		}

		EnsureVehicleSeederTags();
		SeedVehicleSupportItems(selectedEras);
		foreach (var spec in VehicleExampleSpecs.Where(x => selectedEras.Contains(x.EraKey)))
		{
			ValidateVehicleSeedSpec(spec);
			UpsertVehiclePrototype(spec);
		}
	}

	private void EnsureVehicleSeederTags()
	{
		string[] tags =
		[
			"Functions / Vehicles",
			"Functions / Vehicles / Terrestrial Vehicles",
			"Functions / Vehicles / Aquatic Vehicles",
			"Functions / Vehicles / Projection Items",
			"Functions / Vehicles / Vehicle Equipment",
			"Functions / Vehicles / Vehicle Equipment / Propulsion Equipment",
			"Functions / Vehicles / Vehicle Equipment / Hitching Equipment",
			"Market / Transportation / Vehicles",
			"Market / Transportation / Vehicles / Terrestrial Vehicles",
			"Market / Transportation / Vehicles / Aquatic Vehicles",
			"Market / Transportation / Vehicle Equipment",
			"Market / Transportation / Cargo Transportation",
			"Market / Transportation / Passenger Transportation"
		];

		foreach (var tag in tags.Concat(VehicleEraTags.Values))
		{
			EnsureAntiquityTagPath(tag);
		}
	}

}
