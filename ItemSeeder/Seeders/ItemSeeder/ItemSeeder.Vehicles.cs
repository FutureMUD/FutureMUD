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


	internal sealed record VehicleExampleSummaryForTesting(
		string StableReference,
		string EraKey,
		IReadOnlyCollection<string> SupportedEraKeys,
		string Domain,
		string Archetype,
		VehicleScale Scale,
		int CompartmentCount,
		int OccupantSlotCount,
		int PrimaryControlStationCount,
		int MovementProfileCount,
		bool HasDriverSlot,
		bool HasSurfaceWaterMovement,
		bool HasRouteMovement,
		bool HasExplicitPropulsion,
		IReadOnlyCollection<VehiclePropulsionType> PropulsionTypes,
		double MinimumEnginePowerInWatts,
		bool HasMotorInstallation,
		bool HasCargoProjection,
		bool HasAccessProjection);

	internal static IReadOnlyList<VehicleExampleSummaryForTesting> VehicleExamplesForTesting =>
		VehicleExampleSpecs.Select(x => new VehicleExampleSummaryForTesting(
			x.StableReference,
			x.EraKey,
			x.SupportedEraKeys,
			x.Domain,
			x.Archetype,
			x.Scale,
			x.Compartments.Count,
			x.OccupantSlots.Count,
			x.ControlStations.Count(station => station.IsPrimary),
			x.MovementProfiles.Count,
			x.OccupantSlots.Any(slot => slot.SlotType == VehicleOccupantSlotType.Driver),
			x.MovementProfiles.Any(profile => profile.Environment == VehicleMovementEnvironment.SurfaceWater),
			x.MovementProfiles.Any(profile => profile.MovementType == VehicleMovementProfileType.Route),
			x.MovementProfiles.SelectMany(profile => profile.PropulsionProfiles).Any(),
			x.MovementProfiles
				.SelectMany(profile => profile.PropulsionProfiles)
				.Select(profile => profile.PropulsionType)
				.Distinct()
				.ToArray(),
			x.MovementProfiles.Max(profile => profile.MinimumEnginePowerInWatts),
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
				.Select(x => GetVehicleEraToken(x.ToLowerInvariant()))
				.Where(x => x is not null && VehicleEraTags.ContainsKey(x))
				.Select(x => x!)
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
		foreach (var spec in VehicleStock.Select(selectedEras.ToHashSet(StringComparer.OrdinalIgnoreCase)))
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
