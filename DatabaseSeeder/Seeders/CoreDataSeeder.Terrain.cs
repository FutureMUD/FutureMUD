#nullable enable

using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RevisionStatus = MudSharp.Framework.Revision.RevisionStatus;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
	private sealed record StockForageYield(string YieldType, double MaximumYield, double HourlyRecovery);

	private sealed record StockTerrainForageProfileDefinition(
		string TerrainName,
		IReadOnlyCollection<StockForageYield> Yields)
	{
		public string ProfileName => $"{TerrainName} Stock Forage";
	}

    private static readonly (string Name, string Parent)[] StockTerrainTagDefinitions =
    [
        ("Terrain", ""),
        ("Wild", "Terrain"),
        ("Human Influenced", "Terrain"),
        ("Urban", "Human Influenced"),
        ("Rural", "Human Influenced"),
        ("Public", "Urban"),
        ("Private", "Urban"),
        ("Commercial", "Urban"),
        ("Residential", "Urban"),
        ("Administrative", "Urban"),
        ("Industrial", "Urban"),
        ("Natural", "Urban"),
        ("Diggable Soil", "Terrain"),
        ("Foragable Clay", "Terrain"),
        ("Foragable Sand", "Terrain"),
        ("Terrestrial", "Wild"),
        ("Riparian", "Wild"),
        ("Littoral", "Wild"),
        ("Aquatic", "Wild"),
        ("Extraterrestrial", "Wild"),
        ("Lunar", "Extraterrestrial"),
        ("Space", "Extraterrestrial"),
        ("Vacuum", "Terrain"),
        ("Arid", "Terrain"),
        ("Glacial", "Terrain"),
        ("Volcanic", "Terrain"),
        ("Wetland", "Terrain")
    ];

    internal static IReadOnlyCollection<string> StockTerrainTagNamesForTesting =>
        StockTerrainTagDefinitions.Select(x => x.Name).ToArray();

	private static readonly IReadOnlyDictionary<string, StockTerrainForageProfileDefinition>
		StockTerrainForageProfileDefinitions = BuildStockTerrainForageProfileDefinitions();

	internal static IReadOnlyCollection<string> StockTerrainForageProfileTerrainNamesForTesting =>
		StockTerrainForageProfileDefinitions.Keys.OrderBy(x => x).ToArray();

	internal static IReadOnlyCollection<string> StockTerrainForageYieldTypesForTesting =>
		StockTerrainForageProfileDefinitions.Values
			.SelectMany(x => x.Yields)
			.Select(x => x.YieldType)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x)
			.ToArray();

	internal static IReadOnlyDictionary<string, IReadOnlyCollection<string>>
		StockTerrainForageYieldTypesByTerrainForTesting =>
		StockTerrainForageProfileDefinitions.ToDictionary(
			x => x.Key,
			x => (IReadOnlyCollection<string>)x.Value.Yields.Select(y => y.YieldType).ToArray(),
			StringComparer.OrdinalIgnoreCase);

	private static IReadOnlyDictionary<string, StockTerrainForageProfileDefinition>
		BuildStockTerrainForageProfileDefinitions()
	{
		static StockForageYield Yield(string type, double maximum, double hourly) => new(type, maximum, hourly);

		static StockForageYield[] Combine(params StockForageYield[][] groups)
		{
			return groups
				.SelectMany(x => x)
				.GroupBy(x => x.YieldType, StringComparer.OrdinalIgnoreCase)
				.Select(x => x.Last())
				.ToArray();
		}

		StockForageYield[] urbanScavenge =
		[
			Yield("trash", 25.0, 1.0),
			Yield("discarded-food", 10.0, 0.5),
			Yield("insects", 20.0, 2.0),
			Yield("pebbles", 20.0, 0.05)
		];

		StockForageYield[] urbanNatural =
		[
			Yield("grass", 100.0, 7.0),
			Yield("shrubs", 45.0, 3.0),
			Yield("low-trees", 35.0, 1.5),
			Yield("high-trees", 20.0, 0.5),
			Yield("roots", 25.0, 1.0),
			Yield("seeds", 35.0, 2.0),
			Yield("fruit", 20.0, 1.0),
			Yield("herbs", 25.0, 2.0),
			Yield("flowers", 35.0, 3.0),
			Yield("insects", 50.0, 5.0),
			Yield("grubs-worms", 25.0, 2.0),
			Yield("leaves-detritus", 55.0, 4.0),
			Yield("branches-brushwood", 25.0, 0.5),
			Yield("pebbles", 25.0, 0.05)
		];

		StockForageYield[] lawnLike =
		[
			Yield("grass", 130.0, 9.0),
			Yield("herbs", 20.0, 2.0),
			Yield("flowers", 25.0, 2.0),
			Yield("seeds", 30.0, 2.5),
			Yield("insects", 45.0, 4.0),
			Yield("grubs-worms", 20.0, 1.5),
			Yield("pebbles", 20.0, 0.05)
		];

		StockForageYield[] garbage =
		[
			Yield("trash", 220.0, 5.0),
			Yield("discarded-food", 120.0, 4.0),
			Yield("insects", 140.0, 8.0),
			Yield("grubs-worms", 90.0, 5.0),
			Yield("leaves-detritus", 80.0, 4.0),
			Yield("pebbles", 45.0, 0.05),
			Yield("rocks", 30.0, 0.02)
		];

		StockForageYield[] ruralRoadEdge =
		[
			Yield("grass", 55.0, 4.0),
			Yield("shrubs", 25.0, 1.5),
			Yield("herbs", 15.0, 1.0),
			Yield("seeds", 20.0, 1.5),
			Yield("insects", 35.0, 3.0),
			Yield("grubs-worms", 15.0, 1.0),
			Yield("leaves-detritus", 25.0, 1.5),
			Yield("pebbles", 45.0, 0.05),
			Yield("rocks", 20.0, 0.02),
			Yield("trash", 8.0, 0.3),
			Yield("discarded-food", 4.0, 0.2)
		];

		StockForageYield[] gravelRoadEdge =
		[
			Yield("grass", 30.0, 2.0),
			Yield("shrubs", 15.0, 0.8),
			Yield("insects", 25.0, 2.0),
			Yield("pebbles", 85.0, 0.05),
			Yield("rocks", 45.0, 0.02),
			Yield("trash", 6.0, 0.2)
		];

		StockForageYield[] openGrassland =
		[
			Yield("grass", 150.0, 8.0),
			Yield("shrubs", 30.0, 2.0),
			Yield("tubers", 25.0, 1.0),
			Yield("roots", 30.0, 1.5),
			Yield("seeds", 45.0, 3.0),
			Yield("herbs", 40.0, 3.0),
			Yield("flowers", 25.0, 2.0),
			Yield("insects", 60.0, 5.0),
			Yield("grubs-worms", 30.0, 2.0),
			Yield("leaves-detritus", 35.0, 2.0),
			Yield("pebbles", 30.0, 0.05)
		];

		StockForageYield[] richGrassland =
		[
			Yield("grass", 210.0, 10.0),
			Yield("shrubs", 45.0, 2.5),
			Yield("tubers", 35.0, 1.5),
			Yield("roots", 45.0, 2.0),
			Yield("seeds", 60.0, 4.0),
			Yield("herbs", 55.0, 4.0),
			Yield("flowers", 45.0, 3.5),
			Yield("insects", 90.0, 7.0),
			Yield("grubs-worms", 55.0, 3.5),
			Yield("leaves-detritus", 55.0, 3.0),
			Yield("pebbles", 25.0, 0.05)
		];

		StockForageYield[] savannah =
		[
			Yield("grass", 135.0, 7.0),
			Yield("shrubs", 45.0, 2.0),
			Yield("low-trees", 45.0, 1.5),
			Yield("high-trees", 15.0, 0.4),
			Yield("roots", 30.0, 1.5),
			Yield("seeds", 40.0, 2.5),
			Yield("fruit", 20.0, 0.8),
			Yield("insects", 70.0, 5.0),
			Yield("grubs-worms", 30.0, 2.0),
			Yield("leaves-detritus", 40.0, 2.0),
			Yield("branches-brushwood", 20.0, 0.3),
			Yield("pebbles", 35.0, 0.05)
		];

		StockForageYield[] scrub =
		[
			Yield("grass", 65.0, 4.0),
			Yield("shrubs", 115.0, 4.0),
			Yield("low-trees", 30.0, 1.0),
			Yield("tubers", 35.0, 1.2),
			Yield("roots", 45.0, 1.5),
			Yield("seeds", 40.0, 2.0),
			Yield("herbs", 35.0, 2.0),
			Yield("flowers", 25.0, 1.8),
			Yield("insects", 65.0, 4.0),
			Yield("grubs-worms", 25.0, 1.5),
			Yield("leaves-detritus", 45.0, 2.0),
			Yield("branches-brushwood", 35.0, 0.5),
			Yield("pebbles", 35.0, 0.05)
		];

		StockForageYield[] tundra =
		[
			Yield("grass", 35.0, 2.0),
			Yield("shrubs", 30.0, 1.2),
			Yield("roots", 20.0, 0.8),
			Yield("moss", 90.0, 4.0),
			Yield("lichen", 100.0, 4.0),
			Yield("insects", 35.0, 2.5),
			Yield("leaves-detritus", 20.0, 1.0),
			Yield("pebbles", 45.0, 0.05),
			Yield("rocks", 35.0, 0.02)
		];

		StockForageYield[] sparseArid =
		[
			Yield("grass", 20.0, 1.0),
			Yield("shrubs", 35.0, 1.2),
			Yield("tubers", 15.0, 0.5),
			Yield("roots", 25.0, 0.8),
			Yield("seeds", 20.0, 0.8),
			Yield("lichen", 20.0, 0.8),
			Yield("insects", 35.0, 2.0),
			Yield("pebbles", 60.0, 0.05),
			Yield("rocks", 55.0, 0.02),
			Yield("sand", 85.0, 0.05)
		];

		StockForageYield[] upland =
		[
			Yield("grass", 75.0, 4.0),
			Yield("shrubs", 60.0, 2.5),
			Yield("low-trees", 30.0, 1.0),
			Yield("tubers", 25.0, 0.8),
			Yield("roots", 35.0, 1.2),
			Yield("herbs", 35.0, 2.0),
			Yield("flowers", 25.0, 1.5),
			Yield("insects", 55.0, 4.0),
			Yield("moss", 35.0, 1.5),
			Yield("lichen", 35.0, 1.5),
			Yield("branches-brushwood", 20.0, 0.4),
			Yield("pebbles", 75.0, 0.05),
			Yield("rocks", 75.0, 0.02),
			Yield("boulders", 35.0, 0.01)
		];

		StockForageYield[] rocky =
		[
			Yield("grass", 25.0, 1.0),
			Yield("shrubs", 30.0, 1.0),
			Yield("moss", 25.0, 1.0),
			Yield("lichen", 40.0, 1.5),
			Yield("insects", 35.0, 2.0),
			Yield("pebbles", 120.0, 0.05),
			Yield("rocks", 140.0, 0.02),
			Yield("boulders", 80.0, 0.01)
		];

		StockForageYield[] valley =
		[
			Yield("grass", 140.0, 8.0),
			Yield("shrubs", 80.0, 4.0),
			Yield("low-trees", 65.0, 2.0),
			Yield("high-trees", 55.0, 1.0),
			Yield("tubers", 45.0, 1.5),
			Yield("roots", 55.0, 2.0),
			Yield("seeds", 55.0, 3.0),
			Yield("fruit", 35.0, 1.5),
			Yield("nuts", 30.0, 1.0),
			Yield("herbs", 55.0, 4.0),
			Yield("flowers", 40.0, 3.0),
			Yield("mushrooms", 35.0, 2.0),
			Yield("insects", 85.0, 6.0),
			Yield("grubs-worms", 45.0, 3.0),
			Yield("leaves-detritus", 80.0, 5.0),
			Yield("branches-brushwood", 45.0, 0.8),
			Yield("deadwood", 35.0, 0.3),
			Yield("pebbles", 45.0, 0.05)
		];

		StockForageYield[] forest =
		[
			Yield("grass", 45.0, 3.0),
			Yield("shrubs", 95.0, 4.0),
			Yield("low-trees", 100.0, 2.5),
			Yield("high-trees", 180.0, 1.5),
			Yield("mature-trees", 70.0, 0.05),
			Yield("roots", 45.0, 1.5),
			Yield("seeds", 65.0, 3.0),
			Yield("fruit", 45.0, 2.0),
			Yield("nuts", 55.0, 1.5),
			Yield("herbs", 35.0, 2.5),
			Yield("flowers", 25.0, 2.0),
			Yield("moss", 55.0, 3.0),
			Yield("lichen", 45.0, 2.0),
			Yield("mushrooms", 75.0, 4.0),
			Yield("insects", 105.0, 7.0),
			Yield("grubs-worms", 65.0, 4.0),
			Yield("leaves-detritus", 160.0, 8.0),
			Yield("vines", 35.0, 1.0),
			Yield("branches-brushwood", 85.0, 1.0),
			Yield("deadwood", 65.0, 0.4),
			Yield("pebbles", 35.0, 0.05)
		];

		StockForageYield[] rainforest =
		[
			Yield("grass", 35.0, 3.0),
			Yield("shrubs", 120.0, 6.0),
			Yield("low-trees", 140.0, 4.0),
			Yield("high-trees", 240.0, 2.5),
			Yield("mature-trees", 90.0, 0.06),
			Yield("roots", 65.0, 2.5),
			Yield("seeds", 85.0, 5.0),
			Yield("fruit", 95.0, 5.0),
			Yield("nuts", 45.0, 2.0),
			Yield("herbs", 60.0, 4.0),
			Yield("flowers", 55.0, 4.0),
			Yield("moss", 90.0, 5.0),
			Yield("lichen", 55.0, 3.0),
			Yield("mushrooms", 115.0, 6.0),
			Yield("insects", 160.0, 10.0),
			Yield("grubs-worms", 90.0, 6.0),
			Yield("leaves-detritus", 220.0, 10.0),
			Yield("vines", 120.0, 4.0),
			Yield("branches-brushwood", 95.0, 1.2),
			Yield("deadwood", 80.0, 0.5),
			Yield("pebbles", 35.0, 0.05)
		];

		StockForageYield[] orchard =
		[
			Yield("grass", 75.0, 5.0),
			Yield("shrubs", 30.0, 1.5),
			Yield("low-trees", 90.0, 2.5),
			Yield("high-trees", 45.0, 1.0),
			Yield("mature-trees", 30.0, 0.05),
			Yield("roots", 30.0, 1.0),
			Yield("seeds", 45.0, 3.0),
			Yield("fruit", 160.0, 6.0),
			Yield("flowers", 60.0, 4.0),
			Yield("insects", 90.0, 6.0),
			Yield("leaves-detritus", 60.0, 4.0),
			Yield("branches-brushwood", 40.0, 0.7),
			Yield("deadwood", 25.0, 0.2),
			Yield("pebbles", 25.0, 0.05)
		];

		StockForageYield[] wetland =
		[
			Yield("grass", 70.0, 5.0),
			Yield("shrubs", 55.0, 3.0),
			Yield("roots", 60.0, 2.5),
			Yield("tubers", 45.0, 1.8),
			Yield("reeds-rushes", 150.0, 8.0),
			Yield("aquatic-plants", 120.0, 7.0),
			Yield("algae", 80.0, 6.0),
			Yield("insects", 150.0, 10.0),
			Yield("grubs-worms", 75.0, 5.0),
			Yield("crustaceans", 35.0, 2.0),
			Yield("tiny-fish", 50.0, 3.0),
			Yield("leaves-detritus", 120.0, 7.0),
			Yield("branches-brushwood", 35.0, 0.6),
			Yield("clay", 80.0, 0.05),
			Yield("pebbles", 25.0, 0.05)
		];

		StockForageYield[] wetlandForest =
		[
			Yield("grass", 50.0, 4.0),
			Yield("shrubs", 85.0, 4.0),
			Yield("low-trees", 90.0, 3.0),
			Yield("high-trees", 120.0, 1.5),
			Yield("mature-trees", 55.0, 0.05),
			Yield("roots", 80.0, 3.0),
			Yield("reeds-rushes", 110.0, 7.0),
			Yield("aquatic-plants", 100.0, 7.0),
			Yield("algae", 80.0, 6.0),
			Yield("mushrooms", 70.0, 4.0),
			Yield("insects", 160.0, 10.0),
			Yield("grubs-worms", 90.0, 6.0),
			Yield("crustaceans", 45.0, 2.5),
			Yield("tiny-fish", 50.0, 3.0),
			Yield("leaves-detritus", 170.0, 8.0),
			Yield("vines", 65.0, 2.0),
			Yield("branches-brushwood", 70.0, 0.8),
			Yield("deadwood", 60.0, 0.4),
			Yield("clay", 80.0, 0.05)
		];

		StockForageYield[] oasis =
		[
			Yield("grass", 130.0, 8.0),
			Yield("shrubs", 75.0, 4.0),
			Yield("low-trees", 60.0, 2.0),
			Yield("high-trees", 35.0, 1.0),
			Yield("roots", 55.0, 2.0),
			Yield("seeds", 45.0, 3.0),
			Yield("fruit", 50.0, 2.5),
			Yield("herbs", 45.0, 3.0),
			Yield("flowers", 35.0, 2.5),
			Yield("reeds-rushes", 70.0, 5.0),
			Yield("aquatic-plants", 55.0, 4.0),
			Yield("algae", 35.0, 3.0),
			Yield("insects", 100.0, 7.0),
			Yield("tiny-fish", 25.0, 1.5),
			Yield("leaves-detritus", 65.0, 4.0),
			Yield("branches-brushwood", 35.0, 0.5),
			Yield("sand", 55.0, 0.05),
			Yield("pebbles", 35.0, 0.05)
		];

		StockForageYield[] volcanic =
		[
			Yield("grass", 20.0, 1.0),
			Yield("shrubs", 25.0, 1.0),
			Yield("moss", 25.0, 1.2),
			Yield("lichen", 45.0, 1.5),
			Yield("insects", 25.0, 1.5),
			Yield("pebbles", 100.0, 0.05),
			Yield("rocks", 140.0, 0.02),
			Yield("boulders", 90.0, 0.01)
		];

		StockForageYield[] ice =
		[
			Yield("moss", 15.0, 0.8),
			Yield("lichen", 35.0, 1.2),
			Yield("ice", 180.0, 0.5),
			Yield("pebbles", 35.0, 0.05),
			Yield("rocks", 45.0, 0.02),
			Yield("boulders", 30.0, 0.01)
		];

		StockForageYield[] cave =
		[
			Yield("mushrooms", 80.0, 4.0),
			Yield("moss", 45.0, 2.0),
			Yield("lichen", 35.0, 1.5),
			Yield("insects", 75.0, 5.0),
			Yield("grubs-worms", 65.0, 4.0),
			Yield("leaves-detritus", 25.0, 1.5),
			Yield("pebbles", 70.0, 0.05),
			Yield("rocks", 95.0, 0.02),
			Yield("boulders", 45.0, 0.01),
			Yield("clay", 45.0, 0.05)
		];

		StockForageYield[] caveWater =
		[
			Yield("mushrooms", 50.0, 3.0),
			Yield("algae", 80.0, 5.0),
			Yield("aquatic-plants", 45.0, 3.0),
			Yield("tiny-fish", 35.0, 2.0),
			Yield("crustaceans", 40.0, 2.0),
			Yield("insects", 45.0, 3.0),
			Yield("grubs-worms", 35.0, 2.0),
			Yield("pebbles", 60.0, 0.05),
			Yield("rocks", 75.0, 0.02),
			Yield("clay", 55.0, 0.05)
		];

		StockForageYield[] shore =
		[
			Yield("grass", 35.0, 2.0),
			Yield("shrubs", 25.0, 1.0),
			Yield("sea-grass", 45.0, 3.0),
			Yield("algae", 70.0, 5.0),
			Yield("insects", 55.0, 4.0),
			Yield("crustaceans", 60.0, 3.0),
			Yield("shellfish", 45.0, 1.5),
			Yield("molluscs", 40.0, 1.5),
			Yield("tiny-fish", 45.0, 3.0),
			Yield("shells", 60.0, 0.05),
			Yield("sand", 120.0, 0.05),
			Yield("pebbles", 60.0, 0.05),
			Yield("trash", 10.0, 0.3)
		];

		StockForageYield[] rockyShore =
		[
			Yield("algae", 90.0, 5.0),
			Yield("sea-grass", 30.0, 2.0),
			Yield("crustaceans", 75.0, 3.5),
			Yield("shellfish", 75.0, 2.0),
			Yield("molluscs", 70.0, 2.0),
			Yield("tiny-fish", 40.0, 2.5),
			Yield("shells", 50.0, 0.05),
			Yield("pebbles", 80.0, 0.05),
			Yield("rocks", 120.0, 0.02),
			Yield("boulders", 60.0, 0.01),
			Yield("trash", 8.0, 0.3)
		];

		StockForageYield[] freshwaterEdge =
		[
			Yield("grass", 80.0, 5.0),
			Yield("shrubs", 55.0, 3.0),
			Yield("roots", 45.0, 2.0),
			Yield("reeds-rushes", 95.0, 6.0),
			Yield("aquatic-plants", 85.0, 6.0),
			Yield("algae", 55.0, 4.0),
			Yield("insects", 100.0, 7.0),
			Yield("grubs-worms", 45.0, 3.0),
			Yield("crustaceans", 35.0, 2.0),
			Yield("tiny-fish", 60.0, 3.5),
			Yield("leaves-detritus", 65.0, 4.0),
			Yield("clay", 65.0, 0.05),
			Yield("sand", 55.0, 0.05),
			Yield("pebbles", 45.0, 0.05)
		];

		StockForageYield[] saltwater =
		[
			Yield("sea-grass", 75.0, 5.0),
			Yield("algae", 120.0, 8.0),
			Yield("plankton", 150.0, 10.0),
			Yield("crustaceans", 70.0, 3.5),
			Yield("shellfish", 55.0, 2.0),
			Yield("molluscs", 50.0, 2.0),
			Yield("tiny-fish", 120.0, 6.0),
			Yield("shells", 45.0, 0.05),
			Yield("sand", 60.0, 0.05)
		];

		StockForageYield[] openOcean =
		[
			Yield("algae", 80.0, 6.0),
			Yield("plankton", 220.0, 10.0),
			Yield("crustaceans", 65.0, 3.0),
			Yield("tiny-fish", 135.0, 6.0)
		];

		StockForageYield[] reef =
		[
			Yield("sea-grass", 55.0, 4.0),
			Yield("algae", 145.0, 8.0),
			Yield("plankton", 170.0, 10.0),
			Yield("crustaceans", 100.0, 4.0),
			Yield("shellfish", 100.0, 3.0),
			Yield("molluscs", 90.0, 3.0),
			Yield("tiny-fish", 160.0, 7.0),
			Yield("coral", 100.0, 0.2),
			Yield("shells", 60.0, 0.05),
			Yield("sand", 40.0, 0.05)
		];

		StockForageYield[] freshwater =
		[
			Yield("reeds-rushes", 45.0, 3.0),
			Yield("aquatic-plants", 105.0, 7.0),
			Yield("algae", 95.0, 7.0),
			Yield("plankton", 115.0, 8.0),
			Yield("insects", 65.0, 5.0),
			Yield("crustaceans", 45.0, 2.5),
			Yield("tiny-fish", 100.0, 5.0),
			Yield("clay", 50.0, 0.05),
			Yield("sand", 45.0, 0.05)
		];

		var definitions =
			new Dictionary<string, StockTerrainForageProfileDefinition>(StringComparer.OrdinalIgnoreCase);

		void Add(IEnumerable<string> terrains, params StockForageYield[][] yieldGroups)
		{
			foreach (var terrain in terrains)
			{
				if (definitions.ContainsKey(terrain))
				{
					throw new InvalidOperationException($"The stock terrain forage profile for {terrain} is defined more than once.");
				}

				definitions[terrain] = new StockTerrainForageProfileDefinition(terrain, Combine(yieldGroups));
			}
		}

		Add(["Rooftop", "Ghetto Street", "Slum Street", "Poor Street", "Urban Street", "Suburban Street",
			"Wealthy Street", "Marketplace", "Courtyard", "Forum", "Public Square", "Outdoor Mall", "Alleyway",
			"Battlement", "Asphalt Road"], urbanScavenge);
		Add(["Park", "Garden"], urbanNatural, urbanScavenge);
		Add(["Lawn", "Showground"], lawnLike, urbanScavenge);
		Add(["Garbage Dump", "Midden Heap"], garbage);
		Add(["Village Street", "Rural Street", "Animal Trail", "Trail", "Dirt Road"], ruralRoadEdge);
		Add(["Compacted Dirt Road", "Gravel Road", "Cobblestone Road"], gravelRoadEdge);

		Add(["Grasslands", "Steppe", "Shortgrass Prairie", "Pasture"], openGrassland);
		Add(["Tallgrass Prairie", "Meadow", "Field"], richGrassland);
		Add(["Savannah"], savannah);
		Add(["Shrublands", "Heath", "Chaparral"], scrub);
		Add(["Tundra"], tundra);
		Add(["Flood Plain"], richGrassland, freshwaterEdge);
		Add(["Badlands", "Salt Flat"], sparseArid);

		Add(["Hills", "Foothills", "Mound", "Drumlin", "Butte", "Kuppe", "Mesa", "Canyon", "Knoll", "Moor",
			"Tell", "Plateau", "Mountainside", "Mountain Pass"], upland);
		Add(["Dunes"], sparseArid, [Yield("sand", 140.0, 0.05)]);
		Add(["Escarpment", "Scree Slope", "Talus Field", "Mountain Ridge", "Cliff Face", "Cliff Edge"], rocky);
		Add(["Valley", "Vale", "Dell", "Glen", "Strath", "Combe", "Ravine", "Gorge", "Gully"], valley);

		Add(["Boreal Forest", "Broadleaf Forest", "Temperate Coniferous Forest"], forest);
		Add(["Temperate Rainforest", "Tropical Rainforest"], rainforest);
		Add(["Bramble"], scrub, [Yield("vines", 90.0, 3.0), Yield("fruit", 55.0, 3.0)]);
		Add(["Plantation Forest", "Grove", "Woodland"], forest);
		Add(["Orchard"], orchard);

		Add(["Bog", "Fen", "Marsh", "Salt Marsh", "Wetland"], wetland);
		Add(["Mangrove Swamp", "Swamp Forest", "Tropical Freshwater Swamp", "Temperate Freshwater Swamp"], wetlandForest);

		Add(["Sandy Desert", "Rocky Desert"], sparseArid);
		Add(["Coastal Desert"], sparseArid, shore);
		Add(["Oasis"], oasis);
		Add(["Volcanic Plain", "Caldera", "Crater"], volcanic);
		Add(["Lava Field"], rocky, [Yield("lichen", 20.0, 0.8)]);
		Add(["Glacier", "Ice Field"], ice);
		Add(["Snowfield"], tundra, [Yield("ice", 90.0, 0.3)]);

		Add(["Cave Entrance"], cave, [Yield("shrubs", 20.0, 1.0), Yield("roots", 25.0, 1.0)]);
		Add(["Grotto", "Cave", "Cavern"], cave);
		Add(["Cave Pool", "Underground Water"], caveWater);

		Add(["Sandy Beach"], shore);
		Add(["Rocky Beach", "Beachrock"], rockyShore);
		Add(["Riverbank", "Lake Shore", "Mudflat"], freshwaterEdge);
		Add(["Ocean Shallows", "Ocean Surf", "Cove", "Tide Pool", "Shoal"], saltwater);
		Add(["Ocean", "Bay", "Sound", "Deep Ocean"], openOcean);
		Add(["Lagoon", "Estuary"], saltwater, freshwater);
		Add(["Coral Reef", "Reef"], reef);
		Add(["Shallow River", "River", "Deep River", "Shallow Lake", "Lake", "Deep Lake"], freshwater);

		return definitions;
	}

    internal static void SeedTerrainFoundationsForTesting(FuturemudDatabaseContext context)
    {
        SeedTerrainFoundationsCore(context);
    }

    private void SeedTerrainFoundations(FuturemudDatabaseContext context)
    {
        SeedTerrainFoundationsCore(context);
    }

    private static void SeedTerrainFoundationsCore(FuturemudDatabaseContext context)
    {
        DictionaryWithDefault<string, Tag> tags = context.Tags.ToDictionaryWithDefault(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        foreach ((string? name, string? parent) in StockTerrainTagDefinitions)
        {
            AddTerrainTag(context, tags, name, parent);
        }

        context.SaveChanges();
        SeedStockTerrainCatalogue(context, tags);
    }

    private static void AddTerrainTag(FuturemudDatabaseContext context, DictionaryWithDefault<string, Tag> tags,
        string name, string parent)
    {
        if (tags.Any(x => x.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
        {
            return;
        }

        Tag tag = new()
        {
            Name = name,
            Parent = string.IsNullOrEmpty(parent) ? null : tags[parent]
        };
        tags[name] = tag;
        context.Tags.Add(tag);
    }

	private static void SeedStockTerrainForageProfiles(FuturemudDatabaseContext context)
	{
		var installedTerrainNames = context.Terrains
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (!installedTerrainNames.Any())
		{
			return;
		}

		var builderAccountId = context.Accounts
			.OrderBy(x => x.Id)
			.Select(x => x.Id)
			.FirstOrDefault();
		var now = DateTime.UtcNow;
		var nextForageProfileId = context.ForagableProfiles
			.Select(x => x.Id)
			.AsEnumerable()
			.DefaultIfEmpty(0L)
			.Max() + 1L;
		var stockProfiles = new Dictionary<string, ForagableProfile>(StringComparer.OrdinalIgnoreCase);

		foreach (var definition in StockTerrainForageProfileDefinitions.Values
			         .Where(x => installedTerrainNames.Contains(x.TerrainName)))
		{
			var profile = EnsureStockTerrainForageProfile(
				context,
				definition,
				builderAccountId,
				now,
				ref nextForageProfileId);
			stockProfiles[definition.TerrainName] = profile;
		}

		context.SaveChanges();

		var validForageProfileIds =
			(from profile in context.ForagableProfiles
			 join editable in context.EditableItems on profile.EditableItemId equals editable.Id
			 where editable.RevisionStatus == (int)RevisionStatus.Current
			 select profile.Id)
			.ToHashSet();

		foreach (var definition in StockTerrainForageProfileDefinitions.Values)
		{
			if (!stockProfiles.TryGetValue(definition.TerrainName, out var profile))
			{
				continue;
			}

			var terrain = context.Terrains.FirstOrDefault(x =>
				x.Name.Equals(definition.TerrainName, StringComparison.OrdinalIgnoreCase));
			if (terrain is null ||
			    (terrain.ForagableProfileId != 0L && validForageProfileIds.Contains(terrain.ForagableProfileId)))
			{
				continue;
			}

			terrain.ForagableProfileId = profile.Id;
		}

		context.SaveChanges();
	}

	private static ForagableProfile EnsureStockTerrainForageProfile(
		FuturemudDatabaseContext context,
		StockTerrainForageProfileDefinition definition,
		long builderAccountId,
		DateTime now,
		ref long nextForageProfileId)
	{
		var profile = context.ForagableProfiles
			.OrderBy(x => x.Id)
			.ThenBy(x => x.RevisionNumber)
			.FirstOrDefault(x => x.Name == definition.ProfileName);

		if (profile is null)
		{
			profile = new ForagableProfile
			{
				Id = nextForageProfileId++,
				RevisionNumber = 0,
				Name = definition.ProfileName,
				EditableItem = new EditableItem
				{
					RevisionNumber = 0,
					RevisionStatus = (int)RevisionStatus.Current,
					BuilderAccountId = builderAccountId,
					ReviewerAccountId = builderAccountId == 0L ? null : builderAccountId,
					BuilderDate = now,
					ReviewerDate = now,
					BuilderComment = "Auto-generated by the system",
					ReviewerComment = "Auto-generated by the system"
				}
			};
			context.ForagableProfiles.Add(profile);
		}
		else
		{
			profile.Name = definition.ProfileName;
			var editableItem = profile.EditableItem ?? context.EditableItems.Find(profile.EditableItemId);
			if (editableItem is null)
			{
				editableItem = new EditableItem
				{
					RevisionNumber = profile.RevisionNumber,
					BuilderAccountId = builderAccountId,
					BuilderDate = now,
					BuilderComment = "Auto-generated by the system"
				};
				context.EditableItems.Add(editableItem);
				profile.EditableItem = editableItem;
			}

			editableItem.RevisionNumber = profile.RevisionNumber;
			editableItem.RevisionStatus = (int)RevisionStatus.Current;
			editableItem.ReviewerAccountId = builderAccountId == 0L ? null : builderAccountId;
			editableItem.ReviewerDate ??= now;
			editableItem.BuilderComment ??= "Auto-generated by the system";
			editableItem.ReviewerComment ??= "Auto-generated by the system";
		}

		var existingForagables = context.ForagableProfilesForagables
			.Where(x => x.ForagableProfileId == profile.Id &&
			            x.ForagableProfileRevisionNumber == profile.RevisionNumber)
			.ToList();
		context.ForagableProfilesForagables.RemoveRange(existingForagables);

		var existingMaximums = context.ForagableProfilesMaximumYields
			.Where(x => x.ForagableProfileId == profile.Id &&
			            x.ForagableProfileRevisionNumber == profile.RevisionNumber)
			.ToList();
		context.ForagableProfilesMaximumYields.RemoveRange(existingMaximums);

		var existingHourly = context.ForagableProfilesHourlyYieldGains
			.Where(x => x.ForagableProfileId == profile.Id &&
			            x.ForagableProfileRevisionNumber == profile.RevisionNumber)
			.ToList();
		context.ForagableProfilesHourlyYieldGains.RemoveRange(existingHourly);

		foreach (var forageYield in definition.Yields)
		{
			context.ForagableProfilesMaximumYields.Add(new ForagableProfilesMaximumYields
			{
				ForagableProfile = profile,
				ForagableProfileId = profile.Id,
				ForagableProfileRevisionNumber = profile.RevisionNumber,
				ForageType = forageYield.YieldType,
				Yield = forageYield.MaximumYield
			});
			context.ForagableProfilesHourlyYieldGains.Add(new ForagableProfilesHourlyYieldGains
			{
				ForagableProfile = profile,
				ForagableProfileId = profile.Id,
				ForagableProfileRevisionNumber = profile.RevisionNumber,
				ForageType = forageYield.YieldType,
				Yield = forageYield.HourlyRecovery
			});
		}

		return profile;
	}

	private static void BackfillStockTerrainGravity(FuturemudDatabaseContext context)
	{
		var zeroGravityTerrains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Orbital Space",
			"Interplanetary Space",
			"Interstellar Space",
			"Intergalactic Space",
			"Zero-G Spaceship Compartment"
		};

		foreach (var terrain in context.Terrains.Where(x => zeroGravityTerrains.Contains(x.Name)))
		{
			terrain.GravityModel = (int)GravityModel.ZeroGravity;
		}

		foreach (var terrain in context.Terrains.Where(x => x.Name.StartsWith("Lunar") || x.Name == "Moon Surface" || x.Name == "Asteroid Surface"))
		{
			terrain.GravityModel = (int)GravityModel.Normal;
		}

		context.SaveChanges();
	}

    internal static void SeedStockTerrainCatalogue(FuturemudDatabaseContext context, DictionaryWithDefault<string, Tag> tagLookup,
        ICollection<string>? errors = null)
    {
        if (context.Terrains.Count() > 1)
        {
			SeedStockTerrainForageProfiles(context);
			BackfillStockTerrainGravity(context);
			errors?.Add("Terrains were already installed, so did not add any new terrain data. Missing stock forage profiles and stock gravity models were repaired or backfilled where safe.");
            return;
        }

        context.Terrains.Find(1L)!.DefaultTerrain = false;

        void AddTerrain(string name, string behaviour, double movementRate, double staminaCost,
            Difficulty hideDifficulty, Difficulty spotDifficulty, string? atmosphere, CellOutdoorsType outdoorsType,
            Color editorColour, string? editorText = null, bool isdefault = false, IEnumerable<string>? tags = null,
            GravityModel gravityModel = GravityModel.Normal)
        {
            context.Terrains.Add(new Terrain
            {
                Name = name,
                TerrainBehaviourMode = behaviour,
                MovementRate = movementRate,
                StaminaCost = staminaCost,
                HideDifficulty = (int)hideDifficulty,
                SpotDifficulty = (int)spotDifficulty,
                AtmosphereId = context.Gases.FirstOrDefault(x => x.Name == atmosphere)?.Id,
                AtmosphereType = "Gas",
                InfectionMultiplier = 1.0,
                InfectionType = (int)InfectionType.Simple,
                InfectionVirulence = (int)Difficulty.Normal,
                ForagableProfileId = 0,
                DefaultTerrain = isdefault,
                TerrainANSIColour = "7",
                TerrainEditorColour = $"#{editorColour.R:X2}{editorColour.G:X2}{editorColour.B:X2}",
                TerrainEditorText = editorText,
                DefaultCellOutdoorsType = (int)outdoorsType,
                GravityModel = (int)gravityModel,
                TagInformation = tags is not null ?
                    tags.SelectNotNull(x => x is null ? null : tagLookup[x]?.Id.ToString("F0")).ListToCommaSeparatedValues() :
                    ""
            });
            context.SaveChanges();
        }

        Liquid poolwater = context.Liquids.First(x => x.Name == "pool water");
        Liquid springwater = context.Liquids.First(x => x.Name == "spring water");
        Liquid saltwater = context.Liquids.First(x => x.Name == "salt water");
        Liquid brackishwater = context.Liquids.First(x => x.Name == "brackish water");
        Liquid riverwater = context.Liquids.First(x => x.Name == "river water");
        Liquid lakewater = context.Liquids.First(x => x.Name == "lake water");
        Liquid swampwater = context.Liquids.First(x => x.Name == "swamp water");

        #region Urban

        AddTerrain("Residence", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.CornflowerBlue, "Re", true,
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Bedroom", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.MediumPurple, "Br",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Kitchen", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.Orange, "Ki",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Bathroom", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.SkyBlue, "To",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Living Room", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.SeaGreen, "LR",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Hallway", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.CadetBlue, "Hw",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Hall", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.Teal, "Ha", tags: ["Urban", "Administrative", "Public"]);
        AddTerrain("Barracks", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.OliveDrab, "Bk", tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Gymnasium", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.Goldenrod, "Gy", tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Shopfront", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.SandyBrown, "Sf",
                tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Workshop", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.SaddleBrown, "Ws",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Office", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.LightSteelBlue, "Of",
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Factory", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.Silver, "Fa",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Warehouse", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.DarkGray, "Wh",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Indoor Market", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
                "Breathable Atmosphere", CellOutdoorsType.IndoorsWithWindows, Color.Plum, "Im",
                tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Underground Market", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
                "Breathable Atmosphere", CellOutdoorsType.IndoorsWithWindows, Color.DarkOrchid, "Um",
                tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Garage", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.DimGray, "Ga",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Underground Garage", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
                "Breathable Atmosphere", CellOutdoorsType.IndoorsNoLight, Color.DarkSlateGray, "Ug",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Barn", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.Brown, "Bn", tags: ["Rural"]);
        AddTerrain("Cell", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsNoLight, Color.LightSlateGray, "Ce",
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Dank Cell", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsNoLight, Color.Gray, "Dc",
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Dungeon", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsNoLight, Color.Indigo, "Du",
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Grotto", "cave", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsNoLight, Color.DarkSlateBlue, "Gr", tags: ["Rural"]);
        AddTerrain("Cellar", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                 CellOutdoorsType.IndoorsNoLight, Color.BurlyWood, "Cl",
                 tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Baths", "indoors", 0.5, 3.0, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
                 "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.LightBlue, "Bt",
                 tags: ["Urban", "Aquatic", "Commercial", "Public"]);
        AddTerrain("Indoor Pool", $"shallowwater {poolwater.Id}", 0.5, 5.0, Difficulty.ExtremelyHard,
                 Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.DeepSkyBlue, "IP",
                 tags: ["Urban", "Aquatic", "Private"]);
        AddTerrain("Indoor Spring", $"shallowwater {springwater.Id}", 0.5, 5.0, Difficulty.ExtremelyHard,
                 Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.MediumAquamarine, "IS", tags: ["Rural", "Aquatic"]);

        AddTerrain("Rooftop", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.DarkSlateGray, tags: ["Urban", "Private"]);
        AddTerrain("Ghetto Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Urban", "Public"]);
        AddTerrain("Slum Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.Gray, tags: ["Urban", "Public"]);
        AddTerrain("Poor Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Public"]);
        AddTerrain("Urban Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGray, tags: ["Urban", "Public"]);
        AddTerrain("Suburban Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightSlateGray, tags: ["Urban", "Public"]);
        AddTerrain("Wealthy Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Gainsboro, tags: ["Urban", "Public"]);
        AddTerrain("Village Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGray, tags: ["Rural"]);
        AddTerrain("Rural Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.WhiteSmoke, tags: ["Rural"]);

        AddTerrain("Marketplace", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.VeryEasy, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Courtyard", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Private"]);
        AddTerrain("Park", "trees", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Public", "Diggable Soil"]);
        AddTerrain("Garden", "trees", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Private", "Diggable Soil"]);
        AddTerrain("Lawn", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Private", "Diggable Soil"]);
        AddTerrain("Showground", "outdoors", 1.0, 7.0, Difficulty.VeryHard, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen,
                tags: ["Urban", "Commercial", "Public", "Diggable Soil"]);
        AddTerrain("Forum", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Administrative", "Public"]);
        AddTerrain("Public Square", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
                tags: ["Urban", "Administrative", "Public"]);
        AddTerrain("Outdoor Mall", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
                tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Alleyway", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Public"]);
        AddTerrain("Garbage Dump", "outdoors", 1.5, 10.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
                tags: ["Urban", "Industrial", "Private", "Diggable Soil"]);
        AddTerrain("Midden Heap", "outdoors", 1.5, 10.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
                tags: ["Urban", "Industrial", "Private", "Diggable Soil"]);
        AddTerrain("Gatehouse", "indoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Trivial, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsClimateExposed, Color.SlateGray,
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Battlement", "outdoors", 1.0, 7.0, Difficulty.VeryHard, Difficulty.Trivial, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Administrative", "Private"]);

        #endregion

        #region Roads

        AddTerrain("Animal Trail", "outdoors", 1.75, 10.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Trail", "outdoors", 1.6, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Dirt Road", "outdoors", 1.5, 10.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Compacted Dirt Road", "outdoors", 1.4, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
        AddTerrain("Gravel Road", "outdoors", 1.3, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
        AddTerrain("Cobblestone Road", "outdoors", 1.2, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
        AddTerrain("Asphalt Road", "outdoors", 1.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Urban"]);

        #endregion

        #region Terrestrial

        AddTerrain("Grasslands", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Savannah", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Shrublands", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Steppe", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Shortgrass Prairie", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Tallgrass Prairie", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Heath", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Pasture", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Meadow", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Field", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Tundra", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Flood Plain", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Riparian", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Chaparral", "outdoors", 2.5, 18.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.OliveDrab, tags: ["Terrestrial", "Arid", "Diggable Soil"]);
        AddTerrain("Badlands", "outdoors", 3.5, 18.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SandyBrown, tags: ["Terrestrial", "Arid"]);
        AddTerrain("Salt Flat", "outdoors", 2.5, 18.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Linen, tags: ["Terrestrial", "Arid", "Foragable Sand"]);

        AddTerrain("Hills", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Foothills", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Mound", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Drumlin", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Butte", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Kuppe", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Mesa", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Canyon", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Knoll", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Moor", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Tell", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Dunes", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Plateau", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Peru, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Escarpment", "cliff", 4.5, 22.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.IndianRed, tags: ["Terrestrial"]);
        AddTerrain("Scree Slope", "outdoors", 4.0, 25.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkKhaki, tags: ["Terrestrial"]);
        AddTerrain("Talus Field", "outdoors", 4.0, 25.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGoldenrod, tags: ["Terrestrial"]);

        AddTerrain("Mountainside", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Mountain Pass", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Mountain Ridge", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);
        AddTerrain("Cliff Face", "cliff", 5.0, 20.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);
        AddTerrain("Cliff Edge", "outdoors", 5.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);

        AddTerrain("Valley", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Vale", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Dell", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Glen", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Strath", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Combe", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Ravine", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Gorge", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Gully", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);

        AddTerrain("Boreal Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Broadleaf Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Temperate Coniferous Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Temperate Rainforest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Tropical Rainforest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Bramble", "talltrees", 3.0, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Plantation Forest", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Orchard", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Grove", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Woodland", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);

        AddTerrain("Bog", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Fen", $"shallowwater {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.MediumPurple, tags: ["Terrestrial", "Wetland", "Riparian", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Marsh", $"shallowwater {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkMagenta, tags: ["Terrestrial", "Wetland", "Riparian", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Salt Marsh", $"shallowwater {brackishwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Littoral", "Wetland", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Mangrove Swamp", $"shallowwatertrees {brackishwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Littoral", "Wetland", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Wetland", $"shallowwater {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Swamp Forest", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Tropical Freshwater Swamp", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Temperate Freshwater Swamp", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);

        AddTerrain("Sandy Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Arid", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Rocky Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Arid", "Diggable Soil"]);
        AddTerrain("Coastal Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Arid", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Oasis", "trees", 2.0, 12.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.MediumSeaGreen, tags: ["Terrestrial", "Arid", "Diggable Soil"]);
        AddTerrain("Volcanic Plain", "outdoors", 3.5, 20.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Firebrick, tags: ["Terrestrial", "Volcanic"]);
        AddTerrain("Lava Field", "outdoors", 4.0, 25.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkRed, tags: ["Terrestrial", "Volcanic"]);
        AddTerrain("Caldera", "outdoors", 3.5, 20.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.IndianRed, tags: ["Terrestrial", "Volcanic"]);
        AddTerrain("Crater", "outdoors", 3.5, 20.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.BurlyWood, tags: ["Terrestrial"]);
        AddTerrain("Glacier", "outdoors", 4.0, 22.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightCyan, tags: ["Terrestrial", "Glacial"]);
        AddTerrain("Ice Field", "outdoors", 3.0, 18.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.AliceBlue, tags: ["Terrestrial", "Glacial"]);
        AddTerrain("Snowfield", "outdoors", 3.0, 18.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.WhiteSmoke, tags: ["Terrestrial", "Glacial"]);

        AddTerrain("Cave Entrance", "indoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.IndoorsClimateExposed, Color.LightGreen, tags: ["Terrestrial"]);
        AddTerrain("Cave", "cave", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.IndoorsNoLight, Color.LightGreen, tags: ["Terrestrial"]);
        AddTerrain("Cavern", "cave", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.IndoorsNoLight, Color.DarkSeaGreen, tags: ["Terrestrial"]);
        AddTerrain("Cave Pool", $"shallowwatercave {springwater.Id}", 3.0, 10.0, Difficulty.Normal,
            Difficulty.Automatic, "Breathable Atmosphere", CellOutdoorsType.IndoorsNoLight, Color.LightGreen, tags: ["Terrestrial", "Aquatic"]);
        AddTerrain("Underground Water", $"deepwatercave {springwater.Id}", 3.0, 10.0, Difficulty.Normal,
            Difficulty.Automatic, "Breathable Atmosphere", CellOutdoorsType.IndoorsNoLight, Color.LightGreen, tags: ["Terrestrial", "Aquatic"]);

        #endregion

        #region Water

        AddTerrain("Sandy Beach", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Rocky Beach", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral"]);
        AddTerrain("Beachrock", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral"]);
        AddTerrain("Riverbank", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Riparian", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Lake Shore", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);

        AddTerrain("Ocean Shallows", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Littoral", "Foragable Sand"]);
        AddTerrain("Ocean Surf", $"water {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Littoral", "Foragable Sand"]);
        AddTerrain("Ocean", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Mudflat", "outdoors", 4.0, 30.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SaddleBrown, tags: ["Littoral", "Wetland", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Bay", $"water {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Lagoon", $"water {brackishwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Cove", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Tide Pool", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Shoal", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Coral Reef", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Reef", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Sound", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Estuary", $"shallowwater {brackishwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Shallow River", $"shallowwater {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("River", $"water {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Deep River", $"deepwater {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Shallow Lake", $"shallowwater {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Lake", $"water {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Deep Lake", $"deepwater {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Deep Ocean", $"verydeepunderwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane,
            Difficulty.Automatic, null, CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);

        #endregion

        #region Extraterrestrial

        AddTerrain("Moon Surface", "outdoors", 2.5, 18.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.LightSlateGray, tags: ["Extraterrestrial", "Lunar", "Vacuum"]);
        AddTerrain("Lunar Mare", "outdoors", 2.5, 18.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Extraterrestrial", "Lunar", "Vacuum", "Volcanic"]);
        AddTerrain("Lunar Highlands", "outdoors", 3.0, 20.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Gainsboro, tags: ["Extraterrestrial", "Lunar", "Vacuum"]);
        AddTerrain("Lunar Crater", "outdoors", 3.5, 22.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Gray, tags: ["Extraterrestrial", "Lunar", "Vacuum"]);
        AddTerrain("Asteroid Surface", "outdoors", 4.0, 25.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.DarkSlateGray, tags: ["Extraterrestrial", "Vacuum"]);
        AddTerrain("Orbital Space", "outdoors", 1.0, 5.0, Difficulty.Insane, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Black, tags: ["Extraterrestrial", "Space", "Vacuum"],
            gravityModel: GravityModel.ZeroGravity);
        AddTerrain("Interplanetary Space", "outdoors", 1.0, 5.0, Difficulty.Insane, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Black, tags: ["Extraterrestrial", "Space", "Vacuum"],
            gravityModel: GravityModel.ZeroGravity);
        AddTerrain("Interstellar Space", "outdoors", 1.0, 5.0, Difficulty.Insane, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Black, tags: ["Extraterrestrial", "Space", "Vacuum"],
            gravityModel: GravityModel.ZeroGravity);
        AddTerrain("Intergalactic Space", "outdoors", 1.0, 5.0, Difficulty.Insane, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Black, tags: ["Extraterrestrial", "Space", "Vacuum"],
            gravityModel: GravityModel.ZeroGravity);
        AddTerrain("Zero-G Spaceship Compartment", "indoors", 0.5, 2.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.MidnightBlue, "ZG",
            tags: ["Human Influenced", "Industrial", "Space"],
            gravityModel: GravityModel.ZeroGravity);

        #endregion

		SeedStockTerrainForageProfiles(context);

        #region Autobuilders

        context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
        {
            Name = "Blank",
            TemplateType = "simple",
            Definition = @"<Template>
	<RoomName>An Unnamed Room</RoomName>
	<CellDescription>This room has not been given a description.</CellDescription>
	<ShowCommandByline>Create a blank, undescribed room</ShowCommandByline>
</Template>"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Rectangle",
            Definition = "<Definition/>",
            TemplateType = "rectangle"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Rectangle Diagonals",
            Definition = "<Definition/>",
            TemplateType = "rectangle diagonals"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Terrain Rectangle",
            Definition = "<Definition connect_diagonals=\"false\"/>",
            TemplateType = "terrain rectangle"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Terrain Rectangle Diagonals",
            Definition = "<Definition connect_diagonals=\"true\"/>",
            TemplateType = "terrain rectangle"
        });

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Feature Rectangle",
            Definition = "<Definition connect_diagonals=\"false\"/>",
            TemplateType = "terrain feature rectangle"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Feature Rectangle Diagonals",
            Definition = "<Definition connect_diagonals=\"true\"/>",
            TemplateType = "terrain feature rectangle"
        });
        context.SaveChanges();

        //context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
        //{
        //    Name = "Variable",
        //    TemplateType = "room random description",
        //    Definition = @""
        //});
        //context.SaveChanges();

        #endregion
    }
}
