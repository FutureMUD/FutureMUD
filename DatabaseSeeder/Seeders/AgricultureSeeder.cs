using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Work.Agriculture;

namespace DatabaseSeeder.Seeders;

public sealed class AgricultureSeeder : IDatabaseSeeder
{
	private const string StockPrefix = "Stock Agriculture";
	private const string SeedTagName = "Seeds";
	private const string SeededYieldTagName = "Seeded Yield";
	private const string SeedableMaterialTagName = "Agriculture Seedable";
	private const string FarmingTraitName = "Farming";

	private sealed record CropSeed(string Name, string Description, string Category, int Growth, int Window,
		int MinMoisture, int MaxMoisture, int MinTemp, int MaxTemp, AgricultureCommodityYield[] Outputs,
		bool Perennial = false, int HarvestCycle = 0);

	private sealed record WoodlandSeed(string Name, string Description, string Type, int Establishment, int Cycle,
		AgricultureCommodityYield[] Outputs);

	private sealed record OperationSeed(string Name, string Description, AgricultureOperationType Type,
		AgricultureTargetType Target, AgricultureFieldUse Required, AgricultureFieldUse Result, double Hours,
		(AgricultureScoreType Score, int Delta)[] Deltas, double WoodlandYieldMultiplier = 0.0,
		int WoodlandYieldCost = 0);

	private static AgricultureCommodityYield Yield(string materialName, double baseWeight, string tagName = "") => new(materialName, baseWeight, tagName);
	private static AgricultureCommodityYield SeedRequirement(CropSeed definition)
	{
		var primary = definition.Outputs.First();
		var weight = Math.Max(definition.Perennial ? 5000.0 : 500.0, primary.BaseWeight * (definition.Perennial ? 0.02 : 0.04));
		return Yield(primary.MaterialName, weight, SeedTagName);
	}

	private static AgricultureCommodityYield SeedOutput(CropSeed definition)
	{
		var seed = SeedRequirement(definition);
		return Yield(seed.MaterialName, seed.BaseWeight * 1.25, SeedTagName);
	}

	private static IEnumerable<AgricultureCommodityYield> CropOutputs(CropSeed definition)
	{
		foreach (var output in definition.Outputs.Select((value, index) => (value, index)))
		{
			yield return output.index == 0 && string.IsNullOrWhiteSpace(output.value.TagName)
				? Yield(output.value.MaterialName, output.value.BaseWeight, SeededYieldTagName)
				: output.value;
		}

		yield return SeedOutput(definition);
	}

	private static readonly (string Name, string Description, AgricultureFieldUse[] Uses, (AgricultureScoreType Score, int Value)[] Scores)[] Profiles =
	[
		("Arable Field", "A general-purpose field suited to broadacre cropping and periodic improvement.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 65), (AgricultureScoreType.Tilth, 50), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 60)]),
		("Garden Bed", "A small intensively-worked plot suitable for vegetables, herbs, and specialist crops.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 60), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 70), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 75), (AgricultureScoreType.Tilth, 70), (AgricultureScoreType.Rockiness, 10), (AgricultureScoreType.Weeds, 25), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 45), (AgricultureScoreType.Pasture, 15), (AgricultureScoreType.Condition, 70)]),
		("Orchard Grove", "A cultivated grove suitable for long-lived fruit, nut, vine, or plantation crops.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 65), (AgricultureScoreType.Nutrients, 60), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 45), (AgricultureScoreType.Rockiness, 15), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 55), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 65)]),
		("Pasture", "A grazed field with enough grass, fencing, and soil cover to support abstract herds.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 50), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 60), (AgricultureScoreType.Tilth, 40), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 70), (AgricultureScoreType.Pasture, 75), (AgricultureScoreType.Condition, 65)]),
		("Wet Field", "Low, damp land that can be productive after drainage and careful crop choice.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 80), (AgricultureScoreType.Drainage, 25), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 55), (AgricultureScoreType.Tilth, 35), (AgricultureScoreType.Rockiness, 10), (AgricultureScoreType.Weeds, 55), (AgricultureScoreType.Pests, 35), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 45), (AgricultureScoreType.Condition, 45)]),
		("Rocky Field", "A stony plot that needs clearance before it can reliably crop well.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 45), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 35), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 35), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 75), (AgricultureScoreType.Weeds, 30), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 35)]),
		("Saline Coastal Field", "A salt-affected field where salinity management matters more than perfect drainage.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 45), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 40), (AgricultureScoreType.Salinity, 65), (AgricultureScoreType.Topsoil, 45), (AgricultureScoreType.Tilth, 40), (AgricultureScoreType.Rockiness, 15), (AgricultureScoreType.Weeds, 30), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 35)]),
		("Managed Woodland", "A cultivated woodland, coppice, or timber stand managed as a field.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 60), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 25), (AgricultureScoreType.Weeds, 40), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 65)])
	];

	private static readonly CropSeed[] Crops =
	[
		new("Wheat", "A cool-season bread or flour wheat crop.", "grain", 110, 18, 35, 75, 2, 32, [Yield("wheat", 2500000), Yield("straw", 1200000)]),
		new("Barley", "A hardy cereal crop used for food, fodder, and brewing.", "grain", 95, 16, 30, 75, 0, 32, [Yield("barley", 2200000), Yield("straw", 1100000)]),
		new("Rye", "A cold-tolerant cereal crop for poorer or cooler soils.", "grain", 120, 18, 25, 70, -3, 28, [Yield("rye", 2000000), Yield("straw", 1200000)]),
		new("Oats", "A cool, moist cereal crop useful as food grain and fodder.", "grain", 100, 16, 40, 85, 0, 30, [Yield("oat", 1800000), Yield("straw", 1000000)]),
		new("Rice", "A water-loving paddy grain crop for warm growing seasons.", "grain", 120, 20, 70, 100, 16, 40, [Yield("rice", 3000000), Yield("straw", 1500000)]),
		new("Maize", "A warm-season maize or corn crop with heavy grain and stalk yields.", "grain", 115, 16, 40, 85, 12, 38, [Yield("corn", 3500000), Yield("straw", 1800000)]),
		new("Sorghum", "A heat-tolerant dryland cereal crop.", "grain", 110, 18, 25, 70, 15, 42, [Yield("sorghum", 2200000), Yield("straw", 1000000)]),
		new("Millet", "A short-season dryland millet crop.", "grain", 80, 14, 20, 65, 15, 42, [Yield("millet", 1600000), Yield("straw", 800000)]),
		new("Buckwheat", "A fast-growing pseudocereal suited to short seasons.", "grain", 75, 12, 35, 80, 8, 30, [Yield("buckwheat", 1400000), Yield("straw", 500000)]),
		new("Quinoa", "A hardy pseudocereal crop tolerant of cool nights and dry air.", "grain", 100, 16, 20, 65, 2, 32, [Yield("quinoa", 1400000), Yield("straw", 500000)]),
		new("Field Beans", "A broad bean or dry bean pulse crop.", "legume", 95, 14, 35, 80, 5, 32, [Yield("bean", 1800000), Yield("straw", 600000)]),
		new("Peas", "A cool-season pea crop for green or dry pulses.", "legume", 80, 12, 40, 80, 2, 28, [Yield("pea", 1500000), Yield("straw", 500000)]),
		new("Lentils", "A dryland pulse crop that prefers free-draining soil.", "legume", 100, 14, 25, 65, 5, 32, [Yield("lentil", 1200000), Yield("straw", 400000)]),
		new("Chickpeas", "A warm, dry pulse crop for well-drained fields.", "legume", 110, 16, 25, 65, 10, 35, [Yield("chickpea", 1400000), Yield("straw", 400000)]),
		new("Soybeans", "A warm-season oil and protein bean crop.", "legume", 115, 16, 40, 85, 12, 36, [Yield("soybean", 2200000), Yield("straw", 600000)]),
		new("Peanuts", "A warm-season groundnut crop harvested from loose soil.", "legume", 130, 18, 35, 75, 15, 38, [Yield("peanut crop", 1800000), Yield("hay", 500000)]),
		new("Potatoes", "A cool-season tuber crop with high bulk yield.", "root", 100, 16, 45, 85, 4, 28, [Yield("potato", 12000000)]),
		new("Sweet Potatoes", "A warm-season sweet tuber crop.", "root", 120, 18, 40, 80, 15, 38, [Yield("sweet potato", 10000000)]),
		new("Cassava", "A tropical starch root crop tolerant of poor soils.", "root", 240, 35, 35, 75, 18, 42, [Yield("cassava", 16000000)]),
		new("Taro", "A wet-soil tuber crop for warm, damp fields.", "root", 210, 30, 65, 100, 18, 38, [Yield("taro", 10000000)]),
		new("Yams", "A long-season warm-climate tuber crop.", "root", 220, 30, 40, 85, 18, 40, [Yield("yam", 11000000)]),
		new("Carrots", "A temperate root vegetable crop.", "root", 80, 14, 35, 75, 2, 28, [Yield("carrot", 8000000)]),
		new("Beetroot", "A cool-season beetroot crop.", "root", 75, 12, 35, 80, 2, 30, [Yield("beetroot", 7000000)]),
		new("Turnips", "A hardy root crop for food or fodder.", "root", 65, 12, 30, 80, 0, 28, [Yield("turnip", 8000000)]),
		new("Onions", "A bulb onion crop for dry storage.", "root", 115, 18, 25, 70, 5, 32, [Yield("onion", 6000000)]),
		new("Garlic", "A long-season bulb crop harvested once foliage dies back.", "root", 180, 20, 25, 70, -2, 30, [Yield("garlic", 2500000)]),
		new("Cabbage", "A dense-headed brassica vegetable crop.", "vegetable", 90, 14, 40, 85, 2, 28, [Yield("cabbage", 10000000)]),
		new("Lettuce", "A quick leaf crop for cool garden beds.", "vegetable", 50, 8, 45, 85, 2, 26, [Yield("lettuce", 4500000)]),
		new("Tomatoes", "A warm-season fruiting vegetable crop.", "vegetable", 100, 20, 40, 80, 12, 36, [Yield("tomato", 9000000)]),
		new("Cucumbers", "A warm-season vine crop for fresh cucumbers.", "vegetable", 65, 16, 45, 85, 14, 36, [Yield("cucumber", 9000000)]),
		new("Pumpkins", "A sprawling cucurbit crop for large storage fruit.", "vegetable", 110, 22, 35, 80, 12, 36, [Yield("pumpkin", 12000000)]),
		new("Squash", "A warm-season squash crop for summer or winter varieties.", "vegetable", 80, 18, 35, 80, 12, 36, [Yield("squash", 9000000)]),
		new("Peppers", "A warm-season capsicum or chilli pepper crop.", "vegetable", 95, 18, 35, 75, 15, 38, [Yield("pepper", 4500000)]),
		new("Sugarcane", "A tall tropical cane crop for sugar and syrup.", "industrial", 300, 45, 45, 90, 18, 42, [Yield("sugarcane", 30000000)]),
		new("Sugar Beet", "A temperate beet crop grown for sugar-rich roots.", "industrial", 160, 24, 35, 80, 3, 30, [Yield("sugar beet", 18000000)]),
		new("Sunflower", "A warm oilseed crop with large flower heads.", "oilseed", 110, 18, 30, 75, 10, 36, [Yield("sunflower", 1800000), Yield("straw", 500000)]),
		new("Sesame", "A hot-climate oilseed crop.", "oilseed", 100, 14, 25, 65, 18, 42, [Yield("sesame", 900000), Yield("straw", 300000)]),
		new("Canola", "A cool-season brassica oilseed crop.", "oilseed", 120, 18, 35, 80, 2, 30, [Yield("canola", 1800000), Yield("straw", 700000)]),
		new("Cotton", "A warm-season fibre crop producing cotton bolls.", "fibre", 160, 24, 35, 75, 15, 40, [Yield("cotton crop", 1800000), Yield("straw", 700000)]),
		new("Flax", "A cool-season fibre and seed crop.", "fibre", 110, 18, 35, 75, 2, 30, [Yield("flax", 1400000), Yield("straw", 800000)]),
		new("Hemp", "A tall bast-fibre crop for fibre, seed, or biomass.", "fibre", 120, 18, 35, 80, 8, 35, [Yield("hemp crop", 2500000), Yield("straw", 1200000)]),
		new("Jute", "A hot, humid bast-fibre crop.", "fibre", 140, 20, 60, 95, 18, 40, [Yield("jute crop", 2200000), Yield("straw", 1000000)]),
		new("Ramie", "A perennial nettle-family fibre crop.", "fibre", 130, 20, 45, 90, 12, 38, [Yield("ramie", 1800000), Yield("straw", 800000)]),
		new("Sisal", "A dryland leaf-fibre crop.", "fibre", 240, 35, 15, 55, 12, 42, [Yield("sisal", 2000000)])
	];

	private static readonly CropSeed[] Orchards =
	[
		new("Olives", "A dry-climate olive orchard crop.", "orchard", 1095, 35, 20, 70, -3, 42, [Yield("olive crop", 3500000), Yield("olive wood", 250000)], true, 365),
		new("Grapes", "A vineyard crop for table grapes, raisins, or wine.", "orchard", 730, 28, 25, 70, 5, 38, [Yield("grape", 7000000)], true, 210),
		new("Apples", "A temperate apple orchard crop.", "orchard", 1095, 28, 30, 80, -10, 35, [Yield("apple", 10000000), Yield("applewood", 250000)], true, 220),
		new("Pears", "A temperate pear orchard crop.", "orchard", 1095, 28, 30, 80, -10, 35, [Yield("pear", 9000000), Yield("pearwood", 250000)], true, 220),
		new("Peaches", "A stone-fruit orchard crop for warm temperate seasons.", "orchard", 730, 24, 30, 75, -3, 38, [Yield("peach", 7000000)], true, 210),
		new("Plums", "A stone-fruit orchard crop with moderate cold tolerance.", "orchard", 730, 24, 30, 80, -8, 35, [Yield("plum", 6500000)], true, 210),
		new("Cherries", "A cool-winter cherry orchard crop.", "orchard", 1095, 18, 30, 75, -10, 32, [Yield("cherry", 4500000)], true, 210),
		new("Figs", "A dry-climate fig orchard crop.", "orchard", 730, 24, 20, 65, 5, 42, [Yield("fig", 5500000)], true, 210),
		new("Dates", "A hot, dry date palm crop.", "orchard", 1460, 35, 15, 55, 18, 48, [Yield("date", 6500000)], true, 300),
		new("Bananas", "A tropical banana plantation crop.", "orchard", 450, 35, 60, 95, 18, 40, [Yield("banana", 18000000)], true, 300),
		new("Oranges", "A citrus orchard crop for sweet oranges.", "orchard", 1095, 30, 30, 80, 0, 38, [Yield("orange", 9000000)], true, 260),
		new("Lemons", "A citrus orchard crop for lemons.", "orchard", 1095, 30, 30, 80, 0, 38, [Yield("lemon", 7000000)], true, 240),
		new("Almonds", "A dry temperate nut orchard crop.", "orchard", 1095, 22, 20, 65, -2, 38, [Yield("almond", 2500000)], true, 230),
		new("Hazelnuts", "A cool-temperate nut orchard crop.", "orchard", 1095, 22, 35, 80, -5, 32, [Yield("hazelnut", 2200000), Yield("hazel", 300000)], true, 230)
	];

	private static readonly (string Name, string Description, double AnimalUnits, double DailyGraze, int MaximumCondition)[] Herds =
	[
		("Cattle Herd", "A generic cattle, aurochs, buffalo, or large grazing herd.", 1.0, 1.0, 100),
		("Sheep or Goat Flock", "A generic flock of sheep, goats, or other small browsers.", 0.2, 0.3, 100),
		("Pig Herd", "A generic pig, boar, or omnivorous rooting herd.", 0.4, 0.45, 100),
		("Poultry Flock", "A generic flock of chickens, ducks, geese, or similar fowl.", 0.03, 0.05, 100)
	];

	private static readonly WoodlandSeed[] Woodlands =
	[
		new("Hazel Coppice", "A managed hazel coppice cut on a short cycle for rods, stakes, nuts, and firewood.", "coppice", 180, 365, [Yield("hazel", 2500000), Yield("hazelnut", 250000), Yield("firewood", 800000)]),
		new("Willow Coppice", "A damp-ground willow coppice for withies, rods, basketry material, and firewood.", "coppice", 150, 240, [Yield("willow", 2200000), Yield("firewood", 700000)]),
		new("Bamboo Grove", "A fast-growing bamboo grove for canes, poles, shoots, and light structural material.", "coppice", 120, 180, [Yield("bamboo", 3500000), Yield("vegetation", 500000)]),
		new("Oak Timber Stand", "A long-cycle hardwood timber stand managed for heavy oak logs and firewood.", "timber", 730, 3650, [Yield("oak", 9000000), Yield("firewood", 2500000), Yield("oak gall", 100000)]),
		new("Pine Timber Stand", "A softwood timber stand grown for pine logs, poles, and fuel.", "timber", 540, 2555, [Yield("pine", 8500000), Yield("firewood", 2000000)]),
		new("Cedar Timber Stand", "A managed cedar stand for aromatic softwood timber.", "timber", 650, 3285, [Yield("cedar", 7000000), Yield("firewood", 1600000)]),
		new("Pollarded Willow Grove", "A pollarded willow grove cut above browsing height for poles and fuel.", "pollard", 240, 730, [Yield("willow", 3200000), Yield("firewood", 1000000)]),
		new("Fruitwood Pollard Grove", "A pollarded mixed fruitwood grove that yields prunings, light timber, and modest fruitwood.", "pollard", 300, 900, [Yield("applewood", 1500000), Yield("pearwood", 1000000), Yield("firewood", 1000000)])
	];

	private static readonly OperationSeed[] Operations =
	[
		new("Plough Field", "Break and turn the soil to gradually improve tilth and suppress weeds.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 80.0, [(AgricultureScoreType.Tilth, 8), (AgricultureScoreType.Weeds, -4), (AgricultureScoreType.Moisture, -3), (AgricultureScoreType.Condition, 1)]),
		new("Sow Crop", "Sow a crop into prepared ground.", AgricultureOperationType.Sow, AgricultureTargetType.Crop, AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, 32.0, [(AgricultureScoreType.Tilth, -4), (AgricultureScoreType.Weeds, 4)]),
		new("Plant Orchard", "Plant a perennial orchard, vineyard, nut grove, or plantation crop.", AgricultureOperationType.PlantOrchard, AgricultureTargetType.Crop, AgricultureFieldUse.Fallow, AgricultureFieldUse.Orchard, 720.0, [(AgricultureScoreType.Topsoil, -2), (AgricultureScoreType.Tilth, -2), (AgricultureScoreType.Weeds, -4), (AgricultureScoreType.Condition, 1)]),
		new("Weed Field", "Remove competing weeds and tidy a growing field.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 48.0, [(AgricultureScoreType.Weeds, -8), (AgricultureScoreType.Condition, 1)]),
		new("Irrigate Field", "Move water onto the field or garden.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 40.0, [(AgricultureScoreType.Moisture, 8), (AgricultureScoreType.Salinity, 1)]),
		new("Tend Orchard", "Prune, weed, train, and inspect a perennial crop without resetting it.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard, 120.0, [(AgricultureScoreType.Weeds, -6), (AgricultureScoreType.Pests, -4), (AgricultureScoreType.Condition, 2)]),
		new("Drain Field", "Cut ditches, clear channels, or improve drainage.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 240.0, [(AgricultureScoreType.Drainage, 8), (AgricultureScoreType.Moisture, -4), (AgricultureScoreType.Condition, 1)]),
		new("Fertilise Field", "Add manure, compost, ash, or other soil amendments.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 80.0, [(AgricultureScoreType.Nutrients, 8), (AgricultureScoreType.Weeds, 2), (AgricultureScoreType.Condition, 1)]),
		new("Clear Stones", "Dig out rocks and boulders from the usable topsoil.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 320.0, [(AgricultureScoreType.Rockiness, -8), (AgricultureScoreType.Tilth, 2), (AgricultureScoreType.Condition, 1)]),
		new("Build or Repair Fences", "Build or repair fencing, hedges, walls, gates, or other stock barriers.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 240.0, [(AgricultureScoreType.Fence, 10), (AgricultureScoreType.Condition, 1)]),
		new("Harvest Crop", "Harvest a crop that is ready to be gathered.", AgricultureOperationType.Harvest, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Fallow, 96.0, [(AgricultureScoreType.Nutrients, -4), (AgricultureScoreType.Weeds, 4)]),
		new("Harvest Orchard", "Harvest a ready perennial crop while leaving the orchard or vineyard in place.", AgricultureOperationType.Harvest, AgricultureTargetType.None, AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard, 128.0, [(AgricultureScoreType.Nutrients, -3), (AgricultureScoreType.Weeds, 3)]),
		new("Rotate Grazing", "Move or settle an abstract herd onto pasture.", AgricultureOperationType.Herd, AgricultureTargetType.Herd, AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, 32.0, [(AgricultureScoreType.Fence, -2), (AgricultureScoreType.Pasture, -4)]),
		new("Plant Woodland", "Plant or establish a managed woodland, coppice, pollard, or timber stand.", AgricultureOperationType.Woodland, AgricultureTargetType.Woodland, AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland, 480.0, [(AgricultureScoreType.Topsoil, -2), (AgricultureScoreType.Weeds, -4), (AgricultureScoreType.Condition, 2)]),
		new("Coppice Woodland", "Cut a coppice stand back on cycle to produce poles, rods, or firewood while preserving the stand.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 160.0, [(AgricultureScoreType.Nutrients, -2), (AgricultureScoreType.Condition, 1)], 0.45, 45),
		new("Thin Woodland", "Thin a woodland stand to produce small timber and improve remaining growth.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 240.0, [(AgricultureScoreType.Pests, -4), (AgricultureScoreType.Condition, 2)], 0.25, 25),
		new("Fell Woodland", "Fell a managed woodland stand and return the field to fallow use.", AgricultureOperationType.Clear, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Fallow, 360.0, [(AgricultureScoreType.Topsoil, -4), (AgricultureScoreType.Weeds, 4), (AgricultureScoreType.Condition, -4)], 1.0, 100),
		new("Clear Land", "Clear brush, stumps, scrub, or unmanaged growth for future field use.", AgricultureOperationType.Clear, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Fallow, 480.0, [(AgricultureScoreType.Weeds, -6), (AgricultureScoreType.Topsoil, -3), (AgricultureScoreType.Condition, -3)], 0.15, 20)
	];

	public static IReadOnlyCollection<string> StockCommodityOutputMaterialsForTesting =>
		Crops.SelectMany(x => x.Outputs)
		     .Concat(Orchards.SelectMany(x => x.Outputs))
		     .Concat(Woodlands.SelectMany(x => x.Outputs))
		     .Select(x => x.MaterialName)
		     .Distinct(StringComparer.InvariantCultureIgnoreCase)
		     .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase)
		     .ToArray();

	public static IReadOnlyCollection<string> StockSeedMaterialNamesForTesting =>
		Crops.Concat(Orchards)
		     .Select(x => SeedRequirement(x).MaterialName)
		     .Distinct(StringComparer.InvariantCultureIgnoreCase)
		     .OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase)
		     .ToArray();

	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];
	public int SortOrder => 220;
	public string Name => "Agriculture";
	public string Tagline => "Adds stock field profiles, crop, herd, woodland, and agriculture project-operation definitions.";
	public string FullDescription => "Installs setting-neutral agriculture definitions used by the farming system, including specific crop and woodland examples with commodity outputs. These are builder-editable starting points.";
	public bool SafeToRunMoreThanOnce => true;

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() ||
		    !context.FutureProgs.Any(x => x.FunctionName == "AlwaysTrue") ||
		    !context.Tags.Any(x => x.Name == SeedTagName) ||
		    !context.Tags.Any(x => x.Name == SeededYieldTagName) ||
		    !context.Tags.Any(x => x.Name == SeedableMaterialTagName) ||
		    !context.TraitDefinitions.Any(x => x.Name == FarmingTraitName))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		var allStockDefinitionsPresent =
			Profiles.All(x => context.AgricultureFieldProfiles.Any(y => y.Name == x.Name)) &&
			Crops.All(x => context.AgricultureCropDefinitions.Any(y => y.Name == x.Name)) &&
			Orchards.All(x => context.AgricultureCropDefinitions.Any(y => y.Name == x.Name)) &&
			Herds.All(x => context.AgricultureHerdDefinitions.Any(y => y.Name == x.Name)) &&
			Woodlands.All(x => context.AgricultureWoodlandDefinitions.Any(y => y.Name == x.Name)) &&
			Operations.All(x => context.AgricultureOperations.Any(y => y.Name == x.Name)) &&
			Operations.All(x => StockProjectHasSupervision(context, x.Name));
		if (allStockDefinitionsPresent)
		{
			return ShouldSeedResult.MayAlreadyBeInstalled;
		}

		return context.AgricultureFieldProfiles.Any() ||
		       context.AgricultureCropDefinitions.Any() ||
		       context.AgricultureHerdDefinitions.Any() ||
		       context.AgricultureWoodlandDefinitions.Any()
			? ShouldSeedResult.ExtraPackagesAvailable
			: ShouldSeedResult.ReadyToInstall;
	}

	private static bool StockProjectHasSupervision(FuturemudDatabaseContext context, string operationName)
	{
		var project = context.Projects.FirstOrDefault(x =>
			x.Name == $"{StockPrefix}: {operationName}" &&
			x.RevisionNumber == 0);
		if (project == null)
		{
			return false;
		}

		var phase = context.ProjectPhases.FirstOrDefault(x =>
			x.ProjectId == project.Id &&
			x.ProjectRevisionNumber == project.RevisionNumber &&
			x.PhaseNumber == 1);
		return phase != null &&
		       context.ProjectLabourRequirements.Any(x =>
			       x.ProjectPhaseId == phase.Id &&
			       x.Name == "Agricultural Supervision" &&
			       x.Type == "supervision");
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		foreach (var profile in Profiles)
		{
			EnsureProfile(context, profile);
		}

		foreach (var crop in Crops)
		{
			EnsureCrop(context, crop);
		}

		foreach (var orchard in Orchards)
		{
			EnsureCrop(context, orchard);
		}

		foreach (var herd in Herds)
		{
			EnsureHerd(context, herd);
		}

		foreach (var woodland in Woodlands)
		{
			EnsureWoodland(context, woodland);
		}

		foreach (var operation in Operations)
		{
			var project = EnsureProject(context, operation);
			EnsureOperation(context, operation, project);
		}

		context.SaveChanges();
		return "Installed or refreshed stock agriculture definitions and project-backed operations.";
	}

	private static void EnsureProfile(FuturemudDatabaseContext context, (string Name, string Description, AgricultureFieldUse[] Uses, (AgricultureScoreType Score, int Value)[] Scores) definition)
	{
		var profile = context.AgricultureFieldProfiles.FirstOrDefault(x => x.Name == definition.Name);
		if (profile == null)
		{
			profile = new AgricultureFieldProfile { Name = definition.Name };
			context.AgricultureFieldProfiles.Add(profile);
		}

		profile.Description = definition.Description;
		profile.Definition = new XElement("Profile",
			new XAttribute("uses", string.Join(",", definition.Uses.Select(x => x.ToString()))),
			definition.Scores.Select(x => new XElement("Score",
				new XAttribute("type", x.Score.ToString()),
				new XAttribute("value", x.Value)))).ToString();
	}

	private static void EnsureCrop(FuturemudDatabaseContext context, CropSeed definition)
	{
		var crop = context.AgricultureCropDefinitions.FirstOrDefault(x => x.Name == definition.Name);
		if (crop == null)
		{
			crop = new AgricultureCropDefinition { Name = definition.Name };
			context.AgricultureCropDefinitions.Add(crop);
		}

		crop.Description = definition.Description;
		crop.Category = definition.Category;
		crop.Definition = new XElement("Crop",
			new XAttribute("growthDays", definition.Growth),
			new XAttribute("harvestWindowDays", definition.Window),
			new XAttribute("perennial", definition.Perennial),
			new XAttribute("harvestCycleDays", definition.HarvestCycle <= 0 ? definition.Growth : definition.HarvestCycle),
			new XAttribute("minMoisture", definition.MinMoisture),
			new XAttribute("maxMoisture", definition.MaxMoisture),
			new XAttribute("minTemperature", definition.MinTemp),
			new XAttribute("maxTemperature", definition.MaxTemp),
			new XElement("Seeds",
				new XElement("Commodity",
					new XAttribute("material", SeedRequirement(definition).MaterialName),
					new XAttribute("weight", SeedRequirement(definition).BaseWeight),
					new XAttribute("tag", SeedRequirement(definition).TagName))),
			new XElement("Outputs",
				CropOutputs(definition).Select(x => new XElement("Commodity",
					new XAttribute("material", x.MaterialName),
					new XAttribute("weight", x.BaseWeight),
					string.IsNullOrWhiteSpace(x.TagName) ? null : new XAttribute("tag", x.TagName))))).ToString();
	}

	private static void EnsureHerd(FuturemudDatabaseContext context, (string Name, string Description, double AnimalUnits, double DailyGraze, int MaximumCondition) definition)
	{
		var herd = context.AgricultureHerdDefinitions.FirstOrDefault(x => x.Name == definition.Name);
		if (herd == null)
		{
			herd = new AgricultureHerdDefinition { Name = definition.Name };
			context.AgricultureHerdDefinitions.Add(herd);
		}

		herd.Description = definition.Description;
		herd.NpcTemplateId = null;
		herd.NpcTemplateRevisionNumber = null;
		herd.Definition = new XElement("Herd",
			new XAttribute("animalUnits", definition.AnimalUnits),
			new XAttribute("dailyGraze", definition.DailyGraze),
			new XAttribute("maximumCondition", definition.MaximumCondition)).ToString();
	}

	private static void EnsureWoodland(FuturemudDatabaseContext context, WoodlandSeed definition)
	{
		var woodland = context.AgricultureWoodlandDefinitions.FirstOrDefault(x => x.Name == definition.Name);
		if (woodland == null)
		{
			woodland = new AgricultureWoodlandDefinition { Name = definition.Name };
			context.AgricultureWoodlandDefinitions.Add(woodland);
		}

		woodland.Description = definition.Description;
		woodland.WoodlandType = definition.Type;
		woodland.Definition = new XElement("Woodland",
			new XAttribute("establishmentDays", definition.Establishment),
			new XAttribute("harvestCycleDays", definition.Cycle),
			new XElement("Outputs",
				definition.Outputs.Select(x => new XElement("Commodity",
					new XAttribute("material", x.MaterialName),
					new XAttribute("weight", x.BaseWeight),
					string.IsNullOrWhiteSpace(x.TagName) ? null : new XAttribute("tag", x.TagName))))).ToString();
	}

	private static Project EnsureProject(FuturemudDatabaseContext context, OperationSeed operation)
	{
		var operationName = operation.Name;
		var projectName = $"{StockPrefix}: {operationName}";
		var project = context.Projects.FirstOrDefault(x => x.Name == projectName && x.RevisionNumber == 0);
		if (project == null)
		{
			var accountId = context.Accounts.OrderBy(x => x.Id).Select(x => x.Id).First();
			var editable = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.Current,
				BuilderAccountId = accountId,
				BuilderComment = "Seeded stock agriculture project template.",
				BuilderDate = DateTime.UtcNow
			};
			project = new Project
			{
				Id = context.Projects.Any() ? context.Projects.Max(x => x.Id) + 1 : 1,
				RevisionNumber = 0,
				EditableItem = editable,
				Type = "local",
				Name = projectName,
				AppearInJobsList = false
			};
			context.Projects.Add(project);
		}

		var alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		project.Definition = new XElement("Project",
			new XElement("Tagline", new XCData(operation.Description)),
			new XElement("AppearInProjectListProg", alwaysTrue.Id),
			new XElement("CanInitiateProg", alwaysTrue.Id),
			new XElement("WhyCannotInitiateProg", 0),
			new XElement("OnStartProg", 0),
			new XElement("OnFinishProg", 0),
			new XElement("OnCancelProg", 0),
			new XElement("CanCancelProg", 0),
			new XElement("WhyCannotCancelProg", 0)).ToString();
		context.SaveChanges();

		var phase = context.ProjectPhases.FirstOrDefault(x => x.ProjectId == project.Id && x.ProjectRevisionNumber == project.RevisionNumber && x.PhaseNumber == 1);
		if (phase == null)
		{
			phase = new ProjectPhase
			{
				ProjectId = project.Id,
				ProjectRevisionNumber = project.RevisionNumber,
				PhaseNumber = 1,
				Description = $"Complete the field work for {operationName}."
			};
			context.ProjectPhases.Add(phase);
			context.SaveChanges();
		}

		phase.Description = $"Complete the field work for {operationName}.";

		var labour = context.ProjectLabourRequirements.FirstOrDefault(x => x.ProjectPhaseId == phase.Id && x.Name == "Field Labour");
		if (labour == null)
		{
			labour = new ProjectLabourRequirement
			{
				ProjectPhaseId = phase.Id,
				Name = "Field Labour",
				Type = "simple"
			};
			context.ProjectLabourRequirements.Add(labour);
		}

		labour.Description = "General agricultural labour.";
		labour.TotalProgressRequired = operation.Hours;
		labour.MaximumSimultaneousWorkers = 6;
		labour.Type = "simple";
		labour.Definition = new XElement("Labour",
			new XElement("Mandatory", true),
			new XElement("IsQualifiedProg", 0),
			new XElement("RequiredTrait", 0),
			new XElement("MinimumTraitValue", 0.0),
			new XElement("TraitCheckDifficulty", 0)).ToString();

		var farmingTrait = context.TraitDefinitions.First(x => x.Name == FarmingTraitName);
		var supervision = context.ProjectLabourRequirements.FirstOrDefault(x => x.ProjectPhaseId == phase.Id && x.Name == "Agricultural Supervision");
		if (supervision == null)
		{
			supervision = new ProjectLabourRequirement
			{
				ProjectPhaseId = phase.Id,
				Name = "Agricultural Supervision",
				Type = "supervision"
			};
			context.ProjectLabourRequirements.Add(supervision);
		}

		supervision.Description = "Skilled farming oversight that improves the effectiveness and final result of field labour.";
		supervision.TotalProgressRequired = operation.Hours;
		supervision.MaximumSimultaneousWorkers = 1;
		supervision.Type = "supervision";
		supervision.Definition = new XElement("Labour",
			new XElement("Mandatory", false),
			new XElement("IsQualifiedProg", 0),
			new XElement("RequiredTrait", farmingTrait.Id),
			new XElement("MinimumTraitValue", 15.0),
			new XElement("TraitCheckDifficulty", (int)Difficulty.Normal),
			new XElement("MultiplierForOtherLabours", 1.35),
			new XElement("TraitScaledMultiplier", true)).ToString();

		var action = context.ProjectActions.FirstOrDefault(x => x.ProjectPhaseId == phase.Id && x.Type == "agriculture");
		if (action == null)
		{
			action = new ProjectAction
			{
				ProjectPhaseId = phase.Id,
				Name = "Agriculture Operation",
				Type = "agriculture",
				SortOrder = 0
			};
			context.ProjectActions.Add(action);
		}

		action.Description = "Apply the agriculture operation when the project completes.";
		action.Definition = "<Action />";
		EnsureSeedRequirement(context, phase, operation);
		context.SaveChanges();
		return project;
	}

	private static double ProjectSeedWeight(OperationSeed operation)
	{
		return operation.Type switch
		{
			AgricultureOperationType.Sow => 100000.0,
			AgricultureOperationType.PlantOrchard => 250000.0,
			_ => 0.0
		};
	}

	private static void EnsureSeedRequirement(FuturemudDatabaseContext context, ProjectPhase phase, OperationSeed operation)
	{
		var existing = context.ProjectMaterialRequirements.FirstOrDefault(x => x.ProjectPhaseId == phase.Id && x.Name == "Seed Stock");
		var amount = ProjectSeedWeight(operation);
		if (amount <= 0.0)
		{
			if (existing != null)
			{
				context.ProjectMaterialRequirements.Remove(existing);
			}

			return;
		}

		var materialTag = context.Tags.First(x => x.Name == SeedableMaterialTagName);
		var seedTag = context.Tags.First(x => x.Name == SeedTagName);
		if (existing == null)
		{
			existing = new ProjectMaterialRequirement
			{
				ProjectPhaseId = phase.Id,
				Name = "Seed Stock"
			};
			context.ProjectMaterialRequirements.Add(existing);
		}

		existing.Type = "commoditytag";
		existing.Description = "Seed or planting-stock commodity suitable for the chosen crop.";
		existing.IsMandatoryForProjectCompletion = true;
		existing.Definition = new XElement("Material",
			new XElement("MaterialTag", materialTag.Id),
			new XElement("Tag", seedTag.Id),
			new XElement("Amount", amount),
			new XElement("Quality", (int)ItemQuality.Terrible),
			new XElement("Characteristics")).ToString();
	}

	private static void EnsureOperation(FuturemudDatabaseContext context, OperationSeed definition, Project project)
	{
		var operation = context.AgricultureOperations.FirstOrDefault(x => x.Name == definition.Name);
		if (operation == null)
		{
			operation = new AgricultureOperation { Name = definition.Name };
			context.AgricultureOperations.Add(operation);
		}

		operation.Description = definition.Description;
		operation.OperationType = (int)definition.Type;
		operation.TargetType = (int)definition.Target;
		operation.RequiredUse = (int)definition.Required;
		operation.ResultUse = (int)definition.Result;
		operation.ProjectId = project.Id;
		operation.ProjectRevisionNumber = project.RevisionNumber;
		operation.CompletionProgId = null;
		operation.Definition = new XElement("Operation",
			new XAttribute("woodlandYieldMultiplier", definition.WoodlandYieldMultiplier),
			new XAttribute("woodlandYieldCost", definition.WoodlandYieldCost),
			definition.Deltas.Select(x => new XElement("Score",
				new XAttribute("type", x.Score.ToString()),
				new XAttribute("value", x.Delta)))).ToString();
	}
}
