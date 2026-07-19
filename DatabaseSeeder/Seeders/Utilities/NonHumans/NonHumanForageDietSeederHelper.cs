#nullable enable

using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal enum StockNonHumanDietProfile
{
	None,
	Grazer,
	Browser,
	GeneralHerbivore,
	Omnivore,
	ScavengerOmnivore,
	Carnivore,
	Insectivore,
	Piscivore,
	AquaticOmnivore,
	FilterFeeder,
	Detritivore,
	Nectarivore,
	Fungivore,
	PlantMatter
}

internal sealed record StockForageYieldEdibility(
	string YieldType,
	double HungerMultiplier,
	double HydrationMultiplier,
	string EatEmote);

internal static class NonHumanForageDietSeederHelper
{
	internal const double MaximumFoodSatiationHours = 16.0;
	internal const double MaximumDrinkSatiationHours = 8.0;

	private sealed record StockCorpseMaterialEdibility(
		string MaterialName,
		double HungerPerKilogram,
		double WaterPerKilogram,
		double ThirstPerKilogram);

	private static readonly IReadOnlyDictionary<string, StockForageYieldEdibility> YieldEdibilities =
		new ReadOnlyDictionary<string, StockForageYieldEdibility>(
			new Dictionary<string, StockForageYieldEdibility>(StringComparer.OrdinalIgnoreCase)
			{
				["grass"] = new("grass", 0.65, 0.25, "@ graze|grazes on {0}the grass here."),
				["shrubs"] = new("shrubs", 0.55, 0.20, "@ browse|browses {0}the shrubs here."),
				["low-trees"] = new("low-trees", 0.35, 0.15, "@ browse|browses {0}the low tree growth here."),
				["high-trees"] = new("high-trees", 0.30, 0.12, "@ browse|browses {0}the higher tree growth here."),
				["mature-trees"] = new("mature-trees", 0.20, 0.08, "@ gnaw|gnaws {0}at the mature tree growth here."),
				["roots"] = new("roots", 0.75, 0.25, "@ dig|digs up and eat|eats {0}the roots here."),
				["tubers"] = new("tubers", 1.00, 0.35, "@ dig|digs up and eat|eats {0}the tubers here."),
				["seeds"] = new("seeds", 0.90, 0.05, "@ peck|pecks through and eat|eats {0}the seeds here."),
				["fruit"] = new("fruit", 0.90, 0.80, "@ eat|eats {0}the fruit here."),
				["nuts"] = new("nuts", 1.20, 0.05, "@ crack|cracks and eat|eats {0}the nuts here."),
				["herbs"] = new("herbs", 0.65, 0.20, "@ crop|crops {0}the herbs here."),
				["flowers"] = new("flowers", 0.45, 0.20, "@ nibble|nibbles {0}the flowers here."),
				["moss"] = new("moss", 0.35, 0.25, "@ scrape|scrapes up and eat|eats {0}the moss here."),
				["lichen"] = new("lichen", 0.35, 0.15, "@ scrape|scrapes up and eat|eats {0}the lichen here."),
				["mushrooms"] = new("mushrooms", 0.60, 0.50, "@ eat|eats {0}the mushrooms here."),
				["reeds-rushes"] = new("reeds-rushes", 0.65, 0.35, "@ crop|crops {0}the reeds and rushes here."),
				["aquatic-plants"] = new("aquatic-plants", 0.60, 0.55, "@ graze|grazes on {0}the aquatic plants here."),
				["algae"] = new("algae", 0.45, 0.65, "@ skim|skims up and eat|eats {0}the algae here."),
				["sea-grass"] = new("sea-grass", 0.60, 0.50, "@ graze|grazes on {0}the sea grass here."),
				["insects"] = new("insects", 1.10, 0.10, "@ snap|snaps up {0}the insects here."),
				["grubs-worms"] = new("grubs-worms", 1.15, 0.20, "@ root|roots out and eat|eats {0}the grubs and worms here."),
				["crustaceans"] = new("crustaceans", 1.05, 0.30, "@ crunch|crunches through {0}the crustaceans here."),
				["shellfish"] = new("shellfish", 1.05, 0.30, "@ crack|cracks open and eat|eats {0}the shellfish here."),
				["molluscs"] = new("molluscs", 0.95, 0.35, "@ eat|eats {0}the molluscs here."),
				["tiny-fish"] = new("tiny-fish", 1.15, 0.25, "@ snap|snaps up {0}the tiny fish here."),
				["plankton"] = new("plankton", 0.55, 0.70, "@ filter|filters {0}the plankton here."),
				["leaves-detritus"] = new("leaves-detritus", 0.25, 0.10, "@ nose|noses through and eat|eats {0}the leaf litter here."),
				["vines"] = new("vines", 0.40, 0.20, "@ tear|tears down and eat|eats {0}the vines here."),
				["deadwood"] = new("deadwood", 0.08, 0.02, "@ gnaw|gnaws {0}the deadwood here."),
				["discarded-food"] = new("discarded-food", 0.75, 0.15, "@ scavenge|scavenges {0}the discarded food here."),
				["trash"] = new("trash", 0.08, 0.00, "@ scavenge|scavenges through {0}the trash here.")
			});

	private static readonly IReadOnlyDictionary<StockNonHumanDietProfile, string[]> DietYieldTypes =
		new ReadOnlyDictionary<StockNonHumanDietProfile, string[]>(
			new Dictionary<StockNonHumanDietProfile, string[]>
			{
				[StockNonHumanDietProfile.None] = [],
				[StockNonHumanDietProfile.Grazer] =
				[
					"grass", "herbs", "flowers", "seeds", "roots", "tubers", "shrubs", "reeds-rushes",
					"aquatic-plants"
				],
				[StockNonHumanDietProfile.Browser] =
				[
					"shrubs", "low-trees", "high-trees", "mature-trees", "leaves-detritus", "vines", "roots",
					"fruit", "nuts", "flowers", "moss", "lichen"
				],
				[StockNonHumanDietProfile.GeneralHerbivore] =
				[
					"grass", "shrubs", "low-trees", "roots", "tubers", "seeds", "fruit", "nuts", "herbs",
					"flowers", "moss", "lichen", "mushrooms", "reeds-rushes", "aquatic-plants", "algae",
					"sea-grass", "vines"
				],
				[StockNonHumanDietProfile.Omnivore] =
				[
					"grass", "shrubs", "roots", "tubers", "seeds", "fruit", "nuts", "herbs", "flowers",
					"mushrooms", "insects", "grubs-worms", "discarded-food"
				],
				[StockNonHumanDietProfile.ScavengerOmnivore] =
				[
					"grass", "shrubs", "roots", "tubers", "seeds", "fruit", "nuts", "herbs", "flowers",
					"mushrooms", "insects", "grubs-worms", "crustaceans", "shellfish", "molluscs",
					"tiny-fish", "leaves-detritus", "discarded-food", "trash"
				],
				[StockNonHumanDietProfile.Carnivore] =
				[
					"insects", "grubs-worms", "crustaceans", "shellfish", "molluscs", "tiny-fish",
					"discarded-food"
				],
				[StockNonHumanDietProfile.Insectivore] = ["insects", "grubs-worms"],
				[StockNonHumanDietProfile.Piscivore] = ["tiny-fish", "crustaceans", "shellfish", "molluscs"],
				[StockNonHumanDietProfile.AquaticOmnivore] =
				[
					"reeds-rushes", "aquatic-plants", "algae", "sea-grass", "plankton", "crustaceans",
					"shellfish", "molluscs", "tiny-fish"
				],
				[StockNonHumanDietProfile.FilterFeeder] = ["plankton", "algae", "tiny-fish", "crustaceans"],
				[StockNonHumanDietProfile.Detritivore] =
				[
					"leaves-detritus", "deadwood", "mushrooms", "algae", "discarded-food", "trash",
					"insects", "grubs-worms"
				],
				[StockNonHumanDietProfile.Nectarivore] = ["flowers", "fruit"],
				[StockNonHumanDietProfile.Fungivore] = ["mushrooms", "leaves-detritus"],
				[StockNonHumanDietProfile.PlantMatter] =
				[
					"grass", "shrubs", "low-trees", "roots", "tubers", "fruit", "nuts", "flowers", "moss",
					"lichen", "mushrooms", "aquatic-plants", "algae"
				]
			});

	private static readonly IReadOnlyList<StockCorpseMaterialEdibility> CorpseMaterialEdibilities =
	[
		new("flesh", 6.0, 0.35, 0.50),
		new("fatty flesh", 7.5, 0.20, 0.25),
		new("muscly flesh", 6.5, 0.30, 0.45),
		new("bony flesh", 4.0, 0.20, 0.25),
		new("dense bony flesh", 3.0, 0.15, 0.15),
		new("viscera", 5.5, 0.50, 0.75)
	];

	internal static IReadOnlyCollection<string> KnownEdibleYieldTypesForTesting =>
		YieldEdibilities.Keys
			.OrderBy(x => x)
			.ToArray();

	internal static IReadOnlyCollection<string> StockCorpseMaterialNamesForTesting =>
		CorpseMaterialEdibilities
			.Select(x => x.MaterialName)
			.OrderBy(x => x)
			.ToArray();

	internal static IReadOnlyList<StockForageYieldEdibility> GetYieldEdibilitiesForTesting(
		IEnumerable<StockNonHumanDietProfile> profiles,
		SizeCategory size)
	{
		return BuildYieldRows(null, size, profiles)
			.Select(x => new StockForageYieldEdibility(
				x.YieldType,
				x.HungerPerYield,
				x.ThirstPerYield,
				x.EatEmote))
			.ToArray();
	}

	internal static IReadOnlyCollection<string> GetYieldTypes(IEnumerable<StockNonHumanDietProfile> profiles)
	{
		return ExpandYieldTypes(profiles.Where(x => x != StockNonHumanDietProfile.None))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x)
			.ToArray();
	}

	internal static bool ProfilesEatCorpses(IEnumerable<StockNonHumanDietProfile> profiles)
	{
		return profiles.Any(ProfileEatsCorpses);
	}

	internal static void ApplyDiet(
		FuturemudDatabaseContext context,
		Race race,
		SizeCategory size,
		IEnumerable<StockNonHumanDietProfile> profiles)
	{
		var profileList = profiles
			.Where(x => x != StockNonHumanDietProfile.None)
			.Distinct()
			.ToArray();
		if (profileList.Length == 0)
		{
			return;
		}

		var stockYieldTypes = new HashSet<string>(KnownEdibleYieldTypesForTesting, StringComparer.OrdinalIgnoreCase);
		var desiredRows = BuildYieldRows(race, size, profileList)
			.ToDictionary(x => x.YieldType, StringComparer.OrdinalIgnoreCase);
		var existingStockRows = context.RaceEdibleForagableYields
			.Where(x => x.RaceId == race.Id)
			.AsEnumerable()
			.Where(x => stockYieldTypes.Contains(x.YieldType))
			.ToDictionary(x => x.YieldType, StringComparer.OrdinalIgnoreCase);

		foreach (var staleRow in existingStockRows
			         .Where(x => !desiredRows.ContainsKey(x.Key))
			         .Select(x => x.Value)
			         .ToList())
		{
			context.RaceEdibleForagableYields.Remove(staleRow);
		}

		foreach (var desiredRow in desiredRows.Values)
		{
			if (!existingStockRows.TryGetValue(desiredRow.YieldType, out var existingRow))
			{
				context.RaceEdibleForagableYields.Add(desiredRow);
				continue;
			}

			existingRow.BiteYield = desiredRow.BiteYield;
			existingRow.HungerPerYield = desiredRow.HungerPerYield;
			existingRow.WaterPerYield = desiredRow.WaterPerYield;
			existingRow.ThirstPerYield = desiredRow.ThirstPerYield;
			existingRow.AlcoholPerYield = desiredRow.AlcoholPerYield;
			existingRow.EatEmote = desiredRow.EatEmote;
		}

		if (!profileList.Any(ProfileEatsCorpses))
		{
			return;
		}

		race.CanEatCorpses = true;
		race.BiteWeight = CorpseBiteWeight(size);
		race.EatCorpseEmoteText = "@ tear|tears off and eat|eats {0}$1.";
		EnsureCorpseMaterials(context, race);
	}

	internal static double ForageYieldPerBiteForTesting(SizeCategory size)
	{
		return ForageYieldPerBite(size);
	}

	internal static double FullYieldForFoodSaturationForTesting(SizeCategory size)
	{
		return FullYieldForFoodSaturation(size);
	}

	internal static double CorpseBiteWeightForTesting(SizeCategory size)
	{
		return CorpseBiteWeight(size);
	}

	private static IEnumerable<RaceEdibleForagableYields> BuildYieldRows(
		Race? race,
		SizeCategory size,
		IEnumerable<StockNonHumanDietProfile> profiles)
	{
		var fullYieldForFood = FullYieldForFoodSaturation(size);
		var yieldPerBite = ForageYieldPerBite(size);
		foreach (var yield in ExpandYieldTypes(profiles)
			         .Select(x => YieldEdibilities[x])
			         .GroupBy(x => x.YieldType, StringComparer.OrdinalIgnoreCase)
			         .Select(CombineYieldEdibilities)
			         .OrderBy(x => x.YieldType))
		{
			var thirstPerYield = MaximumDrinkSatiationHours * yield.HydrationMultiplier / fullYieldForFood;
			yield return new RaceEdibleForagableYields
			{
				Race = race!,
				RaceId = race?.Id ?? 0,
				YieldType = yield.YieldType,
				BiteYield = yieldPerBite,
				HungerPerYield = MaximumFoodSatiationHours * yield.HungerMultiplier / fullYieldForFood,
				ThirstPerYield = thirstPerYield,
				WaterPerYield = thirstPerYield * 0.1,
				AlcoholPerYield = 0.0,
				EatEmote = yield.EatEmote
			};
		}
	}

	private static IEnumerable<string> ExpandYieldTypes(IEnumerable<StockNonHumanDietProfile> profiles)
	{
		foreach (var profile in profiles)
		{
			foreach (var yieldType in DietYieldTypes[profile])
			{
				yield return yieldType;
			}
		}
	}

	private static StockForageYieldEdibility CombineYieldEdibilities(IEnumerable<StockForageYieldEdibility> yields)
	{
		var ordered = yields
			.OrderByDescending(x => x.HungerMultiplier)
			.ThenByDescending(x => x.HydrationMultiplier)
			.ToArray();
		var bestNutrition = ordered[0];
		return bestNutrition with
		{
			HydrationMultiplier = ordered.Max(x => x.HydrationMultiplier)
		};
	}

	private static bool ProfileEatsCorpses(StockNonHumanDietProfile profile)
	{
		return profile is StockNonHumanDietProfile.Carnivore or
			StockNonHumanDietProfile.Piscivore or
			StockNonHumanDietProfile.ScavengerOmnivore;
	}

	private static void EnsureCorpseMaterials(FuturemudDatabaseContext context, Race race)
	{
		var desiredMaterials = CorpseMaterialEdibilities
			.ToDictionary(x => x.MaterialName, StringComparer.OrdinalIgnoreCase);
		var materialLookup = context.Materials
			.AsEnumerable()
			.Where(x => desiredMaterials.ContainsKey(x.Name))
			.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var existingRows = context.RacesEdibleMaterials
			.Where(x => x.RaceId == race.Id)
			.AsEnumerable()
			.ToDictionary(x => x.MaterialId);

		foreach (var definition in CorpseMaterialEdibilities)
		{
			if (!materialLookup.TryGetValue(definition.MaterialName, out var material))
			{
				continue;
			}

			if (!existingRows.TryGetValue(material.Id, out var row))
			{
				context.RacesEdibleMaterials.Add(new RacesEdibleMaterials
				{
					Race = race,
					RaceId = race.Id,
					Material = material,
					MaterialId = material.Id,
					HungerPerKilogram = definition.HungerPerKilogram,
					WaterPerKilogram = definition.WaterPerKilogram,
					ThirstPerKilogram = definition.ThirstPerKilogram,
					AlcoholPerKilogram = 0.0
				});
				continue;
			}

			row.HungerPerKilogram = definition.HungerPerKilogram;
			row.WaterPerKilogram = definition.WaterPerKilogram;
			row.ThirstPerKilogram = definition.ThirstPerKilogram;
			row.AlcoholPerKilogram = 0.0;
		}
	}

	private static double FullYieldForFoodSaturation(SizeCategory size)
	{
		return size switch
		{
			SizeCategory.Nanoscopic or SizeCategory.Microscopic or SizeCategory.Miniscule => 1.0,
			SizeCategory.Tiny => 4.0,
			SizeCategory.VerySmall => 8.0,
			SizeCategory.Small => 20.0,
			SizeCategory.Normal => 60.0,
			SizeCategory.Large => 100.0,
			SizeCategory.VeryLarge => 180.0,
			SizeCategory.Huge => 300.0,
			SizeCategory.Enormous => 500.0,
			SizeCategory.Gigantic => 800.0,
			SizeCategory.Titanic => 1200.0,
			_ => 60.0
		};
	}

	private static double ForageYieldPerBite(SizeCategory size)
	{
		return size switch
		{
			SizeCategory.Nanoscopic or SizeCategory.Microscopic or SizeCategory.Miniscule => 0.10,
			SizeCategory.Tiny => 0.25,
			SizeCategory.VerySmall => 0.50,
			SizeCategory.Small => 1.50,
			SizeCategory.Normal => 5.00,
			SizeCategory.Large => 10.00,
			SizeCategory.VeryLarge => 20.00,
			SizeCategory.Huge => 35.00,
			SizeCategory.Enormous => 60.00,
			SizeCategory.Gigantic => 90.00,
			SizeCategory.Titanic => 125.00,
			_ => 5.00
		};
	}

	private static double CorpseBiteWeight(SizeCategory size)
	{
		return size switch
		{
			SizeCategory.Nanoscopic or SizeCategory.Microscopic or SizeCategory.Miniscule => 1.0,
			SizeCategory.Tiny => 20.0,
			SizeCategory.VerySmall => 75.0,
			SizeCategory.Small => 250.0,
			SizeCategory.Normal => 750.0,
			SizeCategory.Large => 1500.0,
			SizeCategory.VeryLarge => 3000.0,
			SizeCategory.Huge => 5000.0,
			SizeCategory.Enormous => 9000.0,
			SizeCategory.Gigantic => 14000.0,
			SizeCategory.Titanic => 20000.0,
			_ => 750.0
		};
	}
}
