#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.GameItems;
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
	private const string VehicleSeederMarkerPrefix = "[[ItemSeederVehicle:";
	private const string VehicleExteriorComponentType = "Vehicle Exterior";
	private const string VehicleAccessComponentType = "Vehicle Access Point";
	private const string VehicleCargoComponentType = "Vehicle Cargo Space";
	private const string VehicleOarComponentType = "Vehicle Oar";

	private static readonly Regex StableVehicleReferenceRegex = new("^[a-z0-9]+(?:_[a-z0-9]+)*$", RegexOptions.Compiled);

	private sealed record VehicleProjectionItemSeedSpec(
		string StableReference,
		string Noun,
		string ShortDescription,
		string? LongDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		bool Skinnable,
		bool HiddenFromPlayers,
		string Material,
		string[] Tags,
		string[] Components,
		string BuilderNotes);

	private sealed record VehicleCompartmentSeedSpec(
		string Key,
		string Name,
		string Description,
		int DisplayOrder,
		string? InteriorTerrainName = null,
		int InteriorOutdoorsType = 0);

	private sealed record VehicleCompartmentLinkSeedSpec(
		string SourceCompartmentKey,
		string DestinationCompartmentKey,
		string OutboundDirection,
		string InboundDirection,
		string OutboundDescription,
		string InboundDescription);

	private sealed record VehicleOccupantSlotSeedSpec(
		string Key,
		string CompartmentKey,
		string Name,
		VehicleOccupantSlotType SlotType,
		int Capacity,
		bool RequiredForMovement,
		bool ContributesToPropulsion,
		Difficulty BoatStabilityDifficulty = Difficulty.Normal);

	private sealed record VehicleControlStationSeedSpec(
		string SlotKey,
		string Name,
		bool IsPrimary);

	private sealed record VehiclePropulsionSeedSpec(
		VehiclePropulsionType PropulsionType,
		bool IsDefault,
		double BaseMoveTimeMilliseconds,
		string? PropulsionTraitName,
		Difficulty CheckDifficulty,
		string SpeedMultiplierExpression,
		string StaminaCostExpression);

	private sealed record VehicleMovementProfileSeedSpec(
		string Key,
		string Name,
		VehicleMovementProfileType MovementType,
		VehicleMovementEnvironment MovementEnvironment,
		bool ExposesOccupantsToWater,
		bool IsDefault,
		double RequiredPowerSpikeInWatts,
		string? FuelLiquidName,
		double FuelVolumePerMove,
		string RequiredInstalledRole,
		bool RequiresTowLinksClosed,
		bool RequiresAccessPointsClosed,
		double RouteSpeedMetresPerSecond,
		RouteVehiclePropulsionMode RoutePropulsionMode,
		double RouteFuelVolumePerMetre,
		double RoutePowerDrawWatts,
		bool AutomaticOperationCapable,
		IReadOnlyList<VehiclePropulsionSeedSpec> PropulsionProfiles);

	private sealed record VehicleAccessPointSeedSpec(
		string Key,
		string? CompartmentKey,
		string Name,
		string Description,
		VehicleAccessPointType AccessPointType,
		bool StartsOpen,
		bool MustBeClosedForMovement,
		int DisplayOrder,
		VehicleProjectionItemSeedSpec ProjectionItem);

	private sealed record VehicleCargoSpaceSeedSpec(
		string Key,
		string? CompartmentKey,
		string? RequiredAccessPointKey,
		string Name,
		string Description,
		int DisplayOrder,
		VehicleProjectionItemSeedSpec ProjectionItem);

	private sealed record VehicleInstallationPointSeedSpec(
		string Key,
		string? RequiredAccessPointKey,
		string Name,
		string Description,
		string MountType,
		string RequiredRole,
		bool RequiredForMovement,
		int DisplayOrder);

	private sealed record VehicleTowPointSeedSpec(
		string Key,
		string? RequiredAccessPointKey,
		string Name,
		string Description,
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
		IReadOnlyList<VehicleDamageEffectSeedSpec> Effects);

	private sealed record VehiclePrototypeSeedSpec(
		string StableReference,
		string EraKey,
		string Name,
		string Description,
		VehicleScale Scale,
		VehicleProjectionItemSeedSpec ExteriorItem,
		IReadOnlyList<VehicleCompartmentSeedSpec> Compartments,
		IReadOnlyList<VehicleCompartmentLinkSeedSpec> CompartmentLinks,
		IReadOnlyList<VehicleOccupantSlotSeedSpec> OccupantSlots,
		IReadOnlyList<VehicleControlStationSeedSpec> ControlStations,
		IReadOnlyList<VehicleMovementProfileSeedSpec> MovementProfiles,
		IReadOnlyList<VehicleAccessPointSeedSpec> AccessPoints,
		IReadOnlyList<VehicleCargoSpaceSeedSpec> CargoSpaces,
		IReadOnlyList<VehicleInstallationPointSeedSpec> InstallationPoints,
		IReadOnlyList<VehicleTowPointSeedSpec> TowPoints,
		IReadOnlyList<VehicleDamageZoneSeedSpec> DamageZones);

	internal sealed record VehicleSeederSpecTestData(
		string StableReference,
		string EraKey,
		VehicleScale Scale,
		int CompartmentCount,
		int DriverCapacity,
		int PassengerCapacity,
		int ControlStationCount,
		int PrimaryControlStationCount,
		int MovementProfileCount,
		int DefaultMovementProfileCount,
		bool IsAquatic,
		IReadOnlyList<VehiclePropulsionType> PropulsionTypes,
		int DefaultPropulsionProfileCount,
		int CargoSpaceCount,
		int TowPointCount,
		int DamageZoneCount);

	internal static IReadOnlyList<VehicleSeederSpecTestData> VehicleSeederSpecsForTesting =>
		VehiclePrototypeSeedSpecs()
			.Select(spec => new VehicleSeederSpecTestData(
				spec.StableReference,
				spec.EraKey,
				spec.Scale,
				spec.Compartments.Count,
				spec.OccupantSlots.Where(x => x.SlotType == VehicleOccupantSlotType.Driver).Sum(x => x.Capacity),
				spec.OccupantSlots.Where(x => x.SlotType == VehicleOccupantSlotType.Passenger).Sum(x => x.Capacity),
				spec.ControlStations.Count,
				spec.ControlStations.Count(x => x.IsPrimary),
				spec.MovementProfiles.Count,
				spec.MovementProfiles.Count(x => x.IsDefault),
				spec.MovementProfiles.Any(x => x.MovementEnvironment == VehicleMovementEnvironment.SurfaceWater),
				spec.MovementProfiles.SelectMany(x => x.PropulsionProfiles).Select(x => x.PropulsionType).ToArray(),
				spec.MovementProfiles.SelectMany(x => x.PropulsionProfiles).Count(x => x.IsDefault),
				spec.CargoSpaces.Count,
				spec.TowPoints.Count,
				spec.DamageZones.Count))
			.ToArray();

	private void SeedSelectedEraVehicles(string eras)
	{
		if (_context is null)
		{
			throw new InvalidOperationException("The item seeder context must be initialised before vehicle prototypes are seeded.");
		}

		var selected = VehiclePrototypeSeedSpecs()
			.Where(x => HasAnyEra(eras, x.EraKey))
			.ToArray();
		if (selected.Length == 0)
		{
			return;
		}

		EnsureVehicleTagTaxonomy();
		EnsureVehicleOarComponentsAndItems();

		var validationIssues = ValidateVehiclePrototypeSeedSpecs(selected);
		if (validationIssues.Count > 0)
		{
			throw new InvalidOperationException(
				"Vehicle prototypes cannot be seeded because the declarative catalogue is invalid:" +
				Environment.NewLine + string.Join(Environment.NewLine, validationIssues.Select(x => $" - {x}")));
		}

		foreach (var spec in selected)
		{
			SeedVehiclePrototype(spec);
		}
	}

	private void EnsureVehicleTagTaxonomy()
	{
		foreach (var tag in new[]
		         {
			         "Functions / Vehicles",
			         "Functions / Vehicles / Terrestrial Vehicles",
			         "Functions / Vehicles / Aquatic Vehicles",
			         "Functions / Vehicles / Animal-Drawn Vehicles",
			         "Functions / Vehicles / Human-Powered Vehicles",
			         "Functions / Vehicles / Sailing Vehicles",
			         "Functions / Vehicles / Projection Items",
			         "Functions / Vehicles / Projection Items / Exterior Items",
			         "Functions / Vehicles / Projection Items / Access Points",
			         "Functions / Vehicles / Projection Items / Cargo Spaces",
			         "Functions / Vehicles / Propulsion Equipment",
			         "Functions / Vehicles / Propulsion Equipment / Oars and Paddles"
		         })
		{
			EnsureAntiquityTagPath(tag);
		}
	}

	private void EnsureVehicleOarComponentsAndItems()
	{
		var paddle = EnsureVehicleComponentPrototype(
			"VehicleOar_ShortPaddle",
			VehicleOarComponentType,
			"Makes an item a short paddle with slightly reduced rowing efficiency.",
			new XElement("Definition", new XElement("EfficiencyMultiplier", 0.85)).ToString());
		var oar = EnsureVehicleComponentPrototype(
			"VehicleOar_Standard",
			VehicleOarComponentType,
			"Makes an item a standard vehicle oar.",
			new XElement("Definition", new XElement("EfficiencyMultiplier", 1.0)).ToString());
		var sweep = EnsureVehicleComponentPrototype(
			"VehicleOar_LongSweep",
			VehicleOarComponentType,
			"Makes an item a long sweep oar with improved rowing efficiency.",
			new XElement("Definition", new XElement("EfficiencyMultiplier", 1.15)).ToString());

		SeedVehicleAssociatedItem(new VehicleProjectionItemSeedSpec(
			"preindustrial_vehicle_short_paddle",
			"paddle",
			"a short ash paddle",
			null,
			"This short ash paddle has a broad leaf-shaped blade and a rounded hand grip. The shaft is smooth where repeated strokes have worn away tool marks, while the blade remains thick enough to push against shallow water without flexing badly.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			18.0m,
			true,
			false,
			"ash",
			["Functions / Vehicles / Propulsion Equipment / Oars and Paddles", "Market / Transportation"],
			["Holdable", "Destroyable_WoodenHeavy", paddle.Name],
			"Shared pre-industrial paddle for self-propelled and rowed vehicle examples."));

		SeedVehicleAssociatedItem(new VehicleProjectionItemSeedSpec(
			"preindustrial_vehicle_standard_oar",
			"oar",
			"a long ash oar",
			null,
			"This long ash oar has a narrow, balanced shaft and a broad squared blade. A darkened leather sleeve protects the loom where it rests against a gunwale or thole, and the grip is worn smooth from many hours of rowing.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1800.0,
			32.0m,
			true,
			false,
			"ash",
			["Functions / Vehicles / Propulsion Equipment / Oars and Paddles", "Market / Transportation"],
			["Holdable", "Destroyable_WoodenHeavy", oar.Name],
			"Shared pre-industrial oar for rowed vehicle examples."));

		SeedVehicleAssociatedItem(new VehicleProjectionItemSeedSpec(
			"preindustrial_vehicle_long_sweep_oar",
			"oar",
			"a heavy pine sweep oar",
			null,
			"This long pine sweep is built around a thick shaft and a broad blade intended to move a heavily laden hull. The inboard end is counterbalanced and polished by handling, while the working blade bears shallow scars from quays, sand, and floating debris.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			3600.0,
			70.0m,
			true,
			false,
			"pine",
			["Functions / Vehicles / Propulsion Equipment / Oars and Paddles", "Market / Transportation"],
			["Holdable", "Destroyable_WoodenHeavy", sweep.Name],
			"Shared pre-industrial sweep oar for larger rowed vehicle examples."));
	}

	private void SeedVehicleAssociatedItem(VehicleProjectionItemSeedSpec spec)
	{
		var item = CreateItem(
			spec.StableReference,
			spec.Noun,
			spec.ShortDescription,
			spec.LongDescription,
			spec.FullDescription,
			spec.Size,
			spec.Quality,
			spec.WeightInGrams,
			spec.Cost,
			spec.Skinnable,
			spec.HiddenFromPlayers,
			spec.Material,
			spec.Tags,
			spec.Components,
			null,
			null,
			null,
			null,
			spec.BuilderNotes,
			allowLegacyShortDescriptionMatch: false);
		if (item is null)
		{
			throw new InvalidOperationException($"Unable to seed associated vehicle item {spec.StableReference}.");
		}

		ApplyVehicleItemDefinition(item, spec);
	}

}
