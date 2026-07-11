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
	private const string BeeHiveTagName = "Bee Hive";
	private const string HiveStandTagName = "Hive Stand";
	private const string RawHoneycombTagName = "Raw Honeycomb";
	private const string PressedHoneyTagName = "Pressed Honey";
	private const string RenderedBeeswaxTagName = "Rendered Beeswax";
	private const string RawMilkTagName = "Raw Milk";
	private const string RawTextileFibreTagName = "Raw Textile Fibre";
	private const string EggProductTagName = "Egg Product";
	private const string ManureCommodityTagName = "Manure Commodity";
	private const string FarmingTraitName = "Farming";

	private sealed record CropSeed(string Name, string Description, string Category, int Growth, int Window,
		int MinMoisture, int MaxMoisture, int MinTemp, int MaxTemp, AgricultureCommodityYield[] Outputs,
		bool Perennial = false, int HarvestCycle = 0, IReadOnlyCollection<string>? PlantingGroups = null);

	private sealed record WoodlandSeed(string Name, string Description, string Type, int Establishment, int Cycle,
		AgricultureCommodityYield[] Outputs);

	private sealed record HerdSeed(string Name, string Description, double AnimalUnits, double DailyGraze,
		int MaximumCondition, AgricultureCommodityYield[] SecondaryOutputs);

	private sealed record OperationSeed(string Name, string Description, AgricultureOperationType Type,
		AgricultureTargetType Target, AgricultureFieldUse Required, AgricultureFieldUse Result, double Hours,
		(AgricultureScoreType Score, int Delta)[] Deltas, double WoodlandYieldMultiplier = 0.0,
		int WoodlandYieldCost = 0, double HerdYieldMultiplier = 0.0, int HerdYieldCost = 0,
		AgricultureFieldUse[]? AllowedUses = null, int ApiaryInstallHiveCount = 0,
		int ApiaryPollinationRadius = 0, int ApiaryTendHealthDelta = 0, int ApiaryTendStoresDelta = 0,
		int ApiaryTendYieldDelta = 0, double ApiaryYieldMultiplier = 0.0, int ApiaryYieldCost = 0,
		AgricultureCommodityYield[]? ApiaryOutputs = null);

	private static AgricultureCommodityYield Yield(string materialName, double baseWeight, string tagName = "") => new(materialName, baseWeight, tagName);

	private static readonly string[] CoolSeasonPlanting = ["Autumn", "Spring"];
	private static readonly string[] OverwinteringPlanting = ["Autumn", "Winter"];
	private static readonly string[] WarmSeasonPlanting = ["Spring", "Summer"];
	private static readonly string[] HotLongSeasonPlanting = ["Spring", "Summer", "Autumn"];
	private static readonly string[] DormantPerennialPlanting = ["Autumn", "Winter", "Spring"];
	private static readonly string[] MediterraneanPerennialPlanting = ["Autumn", "Spring"];
	private static readonly string[] TropicalPerennialPlanting = ["Spring", "Summer", "Autumn"];

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

	private static IReadOnlyCollection<string> PlantingGroupsFor(CropSeed definition)
	{
		if (definition.PlantingGroups is not null)
		{
			return definition.PlantingGroups;
		}

		return definition.Name switch
		{
			"Wheat" or "Barley" or "Rye" or "Oats" or "Quinoa" or "Field Beans" or "Peas" or "Lentils" or
				"Emmer Wheat" or "Einkorn Wheat" or "Spelt Wheat" or "Naked Barley" or "New Glume Wheat" or
				"Bitter Vetch" or "Grass Peas" or "Lupins" or "Canihua" or "Pitseed Goosefoot" or "Maygrass" or
				"Little Barley" or "Erect Knotweed" or "Potatoes" or "Oca" or "Ulluco" or "Mashua" or
				"Jerusalem Artichokes" or "Carrots" or "Beetroot" or "Turnips" or "Onions" or "Cabbage" or
				"Lettuce" or "Sugar Beet" or "Canola" or "Mustard" or "Flax" or "Madder" or "Weld" or
				"Alkanet" or "Woad" or "Coriander" or "Saffron Crocus" => CoolSeasonPlanting,
			"Garlic" => OverwinteringPlanting,
			"Rice" or "Maize" or "Sorghum" or "Millet" or "Buckwheat" or "Chickpeas" or "Soybeans" or
				"Peanuts" or "Cowpeas" or "Mung Beans" or "Bambara Groundnuts" or "Adzuki Beans" or "Teff" or
				"Fonio" or "Finger Millet" or "Pearl Millet" or "Foxtail Millet" or "Proso Millet" or
				"Amaranth" or "Chia" or "Marshelder" or "African Rice" or "Sweet Potatoes" or "Tomatoes" or
				"Eggplants" or "Cucumbers" or "Pumpkins" or "Squash" or "Okra" or "Bottle Gourds" or
				"Peppers" or "Sunflower" or "Safflower" or "Niger Seed" or "Hemp" or "Cumin" => WarmSeasonPlanting,
			"Cassava" or "Taro" or "Yams" or "Sugarcane" or "Sesame" or "Cotton" or "Jute" or "Ramie" or
				"Sisal" or "Pigeon Peas" or "Arrowroot" or "Lotus Root" or "Water Chestnuts" or "Kenaf" or
				"Indigo" => HotLongSeasonPlanting,
			"Grapes" or "Apples" or "Pears" or "Peaches" or "Plums" or "Cherries" or "Almonds" or
				"Hazelnuts" or "Quinces" or "Apricots" or "Walnuts" or "Persimmons" or "Mulberries" or
				"Chestnuts" or "Pecans" or "Kiwifruit" => DormantPerennialPlanting,
			"Olives" or "Figs" or "Oranges" or "Lemons" or "Pomegranates" or "Pistachios" or "Limes" or
				"Grapefruits" or "Mandarins" or "Carob" => MediterraneanPerennialPlanting,
			"Dates" or "Bananas" or "Mangoes" or "Coconuts" or "Plantains" or "Breadfruit" or "Avocados" or
				"Cacao" or "Coffee" or "Tea" or "Cashews" or "Macadamias" or "Guavas" or "Lychees" or
				"Jackfruit" or "Papayas" or "Passionfruit" or "Cinnamon" or "Cloves" or "Nutmeg" or
				"Kola Nuts" or "Tamarinds" or "Black Pepper" or "Henna" => TropicalPerennialPlanting,
			_ => Array.Empty<string>()
		};
	}

	private static (AgriculturePollinationDependency Dependency, int HealthBonus, int YieldBonus) PollinationFor(CropSeed definition)
	{
		return definition.Name switch
		{
			"Cucumbers" or "Pumpkins" or "Squash" or "Bottle Gourds" or "Melons" or "Watermelons" or
				"Egusi Melons" or "Chayote" or "Fluted Pumpkins" or "Vanilla" or "Kiwifruit" or
				"Passionfruit" => (AgriculturePollinationDependency.Required, 1, 2),
			"Apples" or "Pears" or "Peaches" or "Plums" or "Cherries" or "Almonds" or "Apricots" or
				"Blueberries" or "Raspberries" or "Blackberries" or "Strawberries" or "Cranberries" or
				"Cacao" =>
				(AgriculturePollinationDependency.Strong, 1, 2),
			"Figs" or "Oranges" or "Lemons" or "Limes" or "Grapefruits" or "Mandarins" or "Mangoes" or
				"Avocados" or "Pomegranates" or "Quinces" or "Persimmons" or "Mulberries" or "Chestnuts" or
				"Pecans" or "Macadamias" or "Guavas" or "Lychees" or "Papayas" or "Coffee" or "Tea" or
				"Tomatillos" or "Nopal Cactus" or "Pineapples" or "Yerba Mate" or "Currants" or
				"Artichokes" or "Sunflower" or "Safflower" or "Sesame" or "Cotton" =>
				(AgriculturePollinationDependency.Beneficial, 0, 1),
			_ => (AgriculturePollinationDependency.None, 0, 0)
		};
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
		("Managed Woodland", "A cultivated woodland, coppice, or timber stand managed as a field.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 60), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 25), (AgricultureScoreType.Weeds, 40), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 65)]),
		("Paddy Field", "A bunded wet rice or aquatic-crop field with standing-water management.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 90), (AgricultureScoreType.Drainage, 20), (AgricultureScoreType.Nutrients, 60), (AgricultureScoreType.Salinity, 15), (AgricultureScoreType.Topsoil, 60), (AgricultureScoreType.Tilth, 45), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 60), (AgricultureScoreType.Pests, 45), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 50)]),
		("Irrigated Market Garden", "A productive irrigated vegetable plot with high fertility and intensive management.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 70), (AgricultureScoreType.Drainage, 65), (AgricultureScoreType.Nutrients, 80), (AgricultureScoreType.Salinity, 15), (AgricultureScoreType.Topsoil, 80), (AgricultureScoreType.Tilth, 75), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 35), (AgricultureScoreType.Fence, 50), (AgricultureScoreType.Pasture, 10), (AgricultureScoreType.Condition, 75)]),
		("Dryland Field", "A rainfed field for drought-tolerant cereals, pulses, oilseeds, or sparse grazing.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 30), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 40), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 45), (AgricultureScoreType.Tilth, 45), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 30), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 30), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 45)]),
		("Terraced Hillside Field", "A laboriously shaped hillside plot for mountain grains, vines, orchards, or tubers.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 50), (AgricultureScoreType.Drainage, 75), (AgricultureScoreType.Nutrients, 45), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 50), (AgricultureScoreType.Tilth, 50), (AgricultureScoreType.Rockiness, 35), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 40), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 55)]),
		("Floodplain Silt Field", "A rich but flood-prone alluvial plot with strong natural fertility and water risk.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 75), (AgricultureScoreType.Drainage, 40), (AgricultureScoreType.Nutrients, 75), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 80), (AgricultureScoreType.Tilth, 55), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 55), (AgricultureScoreType.Pests, 40), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 45), (AgricultureScoreType.Condition, 60)]),
		("Highland Short-Season Field", "A cool upland field for hardy grains, pulses, pasture, and mountain tubers.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 65), (AgricultureScoreType.Nutrients, 45), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 50), (AgricultureScoreType.Tilth, 45), (AgricultureScoreType.Rockiness, 30), (AgricultureScoreType.Weeds, 25), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 55)]),
		("Oasis Garden", "An irrigated arid-zone plot where salinity and water control shape the crop mix.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 65), (AgricultureScoreType.Drainage, 50), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 35), (AgricultureScoreType.Topsoil, 55), (AgricultureScoreType.Tilth, 55), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 30), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 55), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 55)]),
		("Tropical Plantation", "A humid warm-climate plot for perennial plantation crops and vigorous weed pressure.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 80), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 65), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 75), (AgricultureScoreType.Tilth, 45), (AgricultureScoreType.Rockiness, 10), (AgricultureScoreType.Weeds, 60), (AgricultureScoreType.Pests, 50), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 60)]),
		("Alluvial Orchard", "A fertile river-flat orchard profile for moisture-loving fruit or nut trees.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 65), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 70), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 80), (AgricultureScoreType.Tilth, 50), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 40), (AgricultureScoreType.Pests, 35), (AgricultureScoreType.Fence, 50), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 70)]),
		("Steppe Pasture", "A dry open grazing profile that can support hardy pasture but only limited cropping.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 30), (AgricultureScoreType.Drainage, 65), (AgricultureScoreType.Nutrients, 35), (AgricultureScoreType.Salinity, 15), (AgricultureScoreType.Topsoil, 45), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 25), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 45), (AgricultureScoreType.Pasture, 65), (AgricultureScoreType.Condition, 45)]),
		("Wetland Raised Bed", "A raised bed in marsh or lake-margin ground for wetland gardens and water-managed crops.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 85), (AgricultureScoreType.Drainage, 35), (AgricultureScoreType.Nutrients, 70), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 65), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 60), (AgricultureScoreType.Pests, 40), (AgricultureScoreType.Fence, 30), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 60)]),
		("Silvopasture Parkland", "An open tree-and-pasture profile for orchards, woodland, browsing, or mixed parkland use.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Orchard, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 65), (AgricultureScoreType.Tilth, 35), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 40), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 60), (AgricultureScoreType.Pasture, 60), (AgricultureScoreType.Condition, 65)]),
		("Old Field Fallow", "An abandoned former field reverting to coarse grass, weeds, and scattered shrubs.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 45), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 55), (AgricultureScoreType.Tilth, 35), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 60), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 20), (AgricultureScoreType.Pasture, 45), (AgricultureScoreType.Condition, 45)]),
		("Unimproved Hay Meadow", "A species-rich meadow that can graze or cut hay but has little tilth or fencing.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 60), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 45), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 65), (AgricultureScoreType.Tilth, 35), (AgricultureScoreType.Rockiness, 15), (AgricultureScoreType.Weeds, 45), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 70), (AgricultureScoreType.Condition, 55)]),
		("Rough Grazing Moor", "A coarse upland grazing profile with thin soil, wet hollows, and poor crop potential.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 50), (AgricultureScoreType.Nutrients, 30), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 40), (AgricultureScoreType.Tilth, 25), (AgricultureScoreType.Rockiness, 35), (AgricultureScoreType.Weeds, 50), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 20), (AgricultureScoreType.Pasture, 45), (AgricultureScoreType.Condition, 40)]),
		("Brushy Old Pasture", "A neglected pasture with woody encroachment, broken fencing, and uneven grazing value.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 50), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 40), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 50), (AgricultureScoreType.Tilth, 25), (AgricultureScoreType.Rockiness, 40), (AgricultureScoreType.Weeds, 65), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 20), (AgricultureScoreType.Pasture, 50), (AgricultureScoreType.Condition, 35)]),
		("Secondary Scrubland", "Young regrowth scrub or abandoned land with saplings, shrubs, roots, and heavy competition.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 45), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 60), (AgricultureScoreType.Tilth, 20), (AgricultureScoreType.Rockiness, 25), (AgricultureScoreType.Weeds, 70), (AgricultureScoreType.Pests, 35), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 35)]),
		("Woodland Edge Clearing", "A partial forest-edge clearing with decent soil but shade, roots, stumps, and browsing pests.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 60), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 65), (AgricultureScoreType.Tilth, 25), (AgricultureScoreType.Rockiness, 35), (AgricultureScoreType.Weeds, 65), (AgricultureScoreType.Pests, 40), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 40)]),
		("Light Deciduous Woodland", "Open broadleaf woodland with leaf mould and moisture, but roots and stumps block ordinary farming.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Orchard, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 60), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 60), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Rockiness, 30), (AgricultureScoreType.Weeds, 55), (AgricultureScoreType.Pests, 35), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 40)]),
		("Dense Coppice Scrub", "A dense thicket of poles, stools, saplings, and brush that must be cut back before other use.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 50), (AgricultureScoreType.Nutrients, 50), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 60), (AgricultureScoreType.Tilth, 10), (AgricultureScoreType.Rockiness, 35), (AgricultureScoreType.Weeds, 75), (AgricultureScoreType.Pests, 40), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 15), (AgricultureScoreType.Condition, 30)]),
		("Prairie Sod", "Deep grassland sod with good topsoil but dense roots that require heavy breaking before cropping.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 45), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 65), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 75), (AgricultureScoreType.Tilth, 20), (AgricultureScoreType.Rockiness, 10), (AgricultureScoreType.Weeds, 55), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 75), (AgricultureScoreType.Condition, 55)]),
		("Shallow Karst Pasture", "A limestone pavement or karst pasture with very shallow soil, fissures, and exposed stone.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 35), (AgricultureScoreType.Drainage, 85), (AgricultureScoreType.Nutrients, 30), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 25), (AgricultureScoreType.Tilth, 20), (AgricultureScoreType.Rockiness, 80), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 15), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 25)]),
		("Bracken Heath", "Acid heath or bracken ground with low fertility and persistent competitive vegetation.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 45), (AgricultureScoreType.Drainage, 65), (AgricultureScoreType.Nutrients, 25), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 35), (AgricultureScoreType.Tilth, 20), (AgricultureScoreType.Rockiness, 30), (AgricultureScoreType.Weeds, 75), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 25)]),
		("Gorse Scrub", "Thorny shrubland that can hide usable soil but demands serious clearing and fencing work.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 40), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 30), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 40), (AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Rockiness, 35), (AgricultureScoreType.Weeds, 80), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 25)]),
		("Mediterranean Garrigue", "Dry limestone scrub with aromatic brush, shallow soil, drought stress, and heavy stone.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 25), (AgricultureScoreType.Drainage, 75), (AgricultureScoreType.Nutrients, 30), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 30), (AgricultureScoreType.Tilth, 20), (AgricultureScoreType.Rockiness, 55), (AgricultureScoreType.Weeds, 60), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 30)]),
		("Chaparral Slope", "A dry shrub-covered slope with erosion risk, woody brush, and difficult access.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 25), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 35), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 35), (AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Rockiness, 45), (AgricultureScoreType.Weeds, 75), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 25)]),
		("Savanna Parkland", "Open tropical tree grassland with seasonal moisture, browsing pressure, and mixed pasture potential.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Orchard, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 45), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 45), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 50), (AgricultureScoreType.Tilth, 25), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 55), (AgricultureScoreType.Pests, 45), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 60), (AgricultureScoreType.Condition, 45)]),
		("Riparian Thicket", "A river-edge thicket with rich silt, high moisture, flood risk, weeds, and pests.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 85), (AgricultureScoreType.Drainage, 35), (AgricultureScoreType.Nutrients, 70), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 15), (AgricultureScoreType.Weeds, 80), (AgricultureScoreType.Pests, 50), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 50), (AgricultureScoreType.Condition, 45)]),
		("Reedbed Marsh", "A waterlogged reedbed or marsh margin that needs drainage or raised beds before most crops.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 95), (AgricultureScoreType.Drainage, 10), (AgricultureScoreType.Nutrients, 60), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 55), (AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 85), (AgricultureScoreType.Pests, 50), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 25)]),
		("Fen Meadow", "A wet organic meadow with useful grazing potential but poor drainage and high weed pressure.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 90), (AgricultureScoreType.Drainage, 20), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 60), (AgricultureScoreType.Tilth, 25), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 70), (AgricultureScoreType.Pests, 45), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 55), (AgricultureScoreType.Condition, 35)]),
		("Peat Bog Margin", "An acidic waterlogged peat edge where ordinary farming performs poorly without major reclamation.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 95), (AgricultureScoreType.Drainage, 10), (AgricultureScoreType.Nutrients, 20), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 45), (AgricultureScoreType.Tilth, 10), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 65), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 15)]),
		("Sandy Dune Swale", "A sandy coastal depression with droughty soil, wind exposure, and some salt influence.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 30), (AgricultureScoreType.Drainage, 90), (AgricultureScoreType.Nutrients, 15), (AgricultureScoreType.Salinity, 35), (AgricultureScoreType.Topsoil, 20), (AgricultureScoreType.Tilth, 25), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 15), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 20)]),
		("Salt Marsh Edge", "A saline wetland edge with poor ordinary crop prospects but possible rough grazing or reclamation.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 85), (AgricultureScoreType.Drainage, 25), (AgricultureScoreType.Nutrients, 35), (AgricultureScoreType.Salinity, 80), (AgricultureScoreType.Topsoil, 35), (AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Rockiness, 5), (AgricultureScoreType.Weeds, 50), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 15)]),
		("Heavy Clay Flat", "A compact heavy-clay flat with decent fertility but poor drainage and weak tilth.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 70), (AgricultureScoreType.Drainage, 20), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 55), (AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Rockiness, 10), (AgricultureScoreType.Weeds, 55), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 10), (AgricultureScoreType.Pasture, 40), (AgricultureScoreType.Condition, 30)]),
		("Stony Alluvial Fan", "A coarse alluvial fan or wash with sharp drainage, gravel, and low fine topsoil.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 25), (AgricultureScoreType.Drainage, 85), (AgricultureScoreType.Nutrients, 25), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 30), (AgricultureScoreType.Tilth, 20), (AgricultureScoreType.Rockiness, 70), (AgricultureScoreType.Weeds, 35), (AgricultureScoreType.Pests, 15), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 20)]),
		("Eroded Hillslope", "A depleted slope with lost topsoil, exposed stone, and continuing erosion risk.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 35), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 20), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 20), (AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Rockiness, 65), (AgricultureScoreType.Weeds, 45), (AgricultureScoreType.Pests, 15), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 15)]),
		("Lateritic Scrubland", "A tropical lateritic scrub profile with weathered low-fertility soil and hard seasonal limits.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture, AgricultureFieldUse.Orchard, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 60), (AgricultureScoreType.Nutrients, 20), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 35), (AgricultureScoreType.Tilth, 20), (AgricultureScoreType.Rockiness, 35), (AgricultureScoreType.Weeds, 65), (AgricultureScoreType.Pests, 45), (AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 25)]),
		("Apiary Yard", "A managed beeyard profile for placing hives alongside fallow ground, crops, orchards, pasture, or woodland.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard, AgricultureFieldUse.Pasture, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 50), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 50), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 55), (AgricultureScoreType.Tilth, 35), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 45), (AgricultureScoreType.Pests, 25), (AgricultureScoreType.Fence, 40), (AgricultureScoreType.Pasture, 45), (AgricultureScoreType.Condition, 60)]),
		("Exhausted Cropland", "A nearly ruined cultivated field with depleted soil, weak tilth, heavy weeds, and little resilience.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 45), (AgricultureScoreType.Drainage, 45), (AgricultureScoreType.Nutrients, 12), (AgricultureScoreType.Salinity, 20), (AgricultureScoreType.Topsoil, 18), (AgricultureScoreType.Tilth, 15), (AgricultureScoreType.Rockiness, 35), (AgricultureScoreType.Weeds, 78), (AgricultureScoreType.Pests, 48), (AgricultureScoreType.Fence, 18), (AgricultureScoreType.Pasture, 18), (AgricultureScoreType.Condition, 14)]),
		("Borderline Cropping Patch", "A poor but still usable small field that can carry hardy crops after careful work.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 42), (AgricultureScoreType.Drainage, 50), (AgricultureScoreType.Nutrients, 25), (AgricultureScoreType.Salinity, 15), (AgricultureScoreType.Topsoil, 30), (AgricultureScoreType.Tilth, 26), (AgricultureScoreType.Rockiness, 38), (AgricultureScoreType.Weeds, 62), (AgricultureScoreType.Pests, 35), (AgricultureScoreType.Fence, 20), (AgricultureScoreType.Pasture, 25), (AgricultureScoreType.Condition, 28)]),
		("Poor Tenant Field", "A low-quality worked field with thin soil, broken boundaries, and uneven preparation.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 48), (AgricultureScoreType.Drainage, 52), (AgricultureScoreType.Nutrients, 34), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 38), (AgricultureScoreType.Tilth, 32), (AgricultureScoreType.Rockiness, 30), (AgricultureScoreType.Weeds, 55), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 25), (AgricultureScoreType.Pasture, 32), (AgricultureScoreType.Condition, 36)]),
		("Average Worked Field", "A plain middle-quality cultivated field with fair soil, serviceable tilth, and ordinary weed pressure.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 55), (AgricultureScoreType.Drainage, 58), (AgricultureScoreType.Nutrients, 52), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 58), (AgricultureScoreType.Tilth, 52), (AgricultureScoreType.Rockiness, 22), (AgricultureScoreType.Weeds, 38), (AgricultureScoreType.Pests, 26), (AgricultureScoreType.Fence, 38), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 55)]),
		("Well-Worked Open Field", "A productive open-field strip with good tilth, adequate fertility, and maintained boundaries.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 58), (AgricultureScoreType.Drainage, 62), (AgricultureScoreType.Nutrients, 65), (AgricultureScoreType.Salinity, 8), (AgricultureScoreType.Topsoil, 68), (AgricultureScoreType.Tilth, 66), (AgricultureScoreType.Rockiness, 16), (AgricultureScoreType.Weeds, 30), (AgricultureScoreType.Pests, 24), (AgricultureScoreType.Fence, 48), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 68)]),
		("Manured Market Garden", "An intensively manured vegetable plot with rich soil, good tilth, fencing, and moderate pest pressure.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 65), (AgricultureScoreType.Drainage, 66), (AgricultureScoreType.Nutrients, 82), (AgricultureScoreType.Salinity, 12), (AgricultureScoreType.Topsoil, 78), (AgricultureScoreType.Tilth, 78), (AgricultureScoreType.Rockiness, 8), (AgricultureScoreType.Weeds, 28), (AgricultureScoreType.Pests, 38), (AgricultureScoreType.Fence, 62), (AgricultureScoreType.Pasture, 15), (AgricultureScoreType.Condition, 76)]),
		("Irrigated Goodfield", "A productive irrigated field with strong moisture, fertility, and condition but mild salt risk.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 78), (AgricultureScoreType.Drainage, 62), (AgricultureScoreType.Nutrients, 72), (AgricultureScoreType.Salinity, 25), (AgricultureScoreType.Topsoil, 75), (AgricultureScoreType.Tilth, 68), (AgricultureScoreType.Rockiness, 10), (AgricultureScoreType.Weeds, 32), (AgricultureScoreType.Pests, 32), (AgricultureScoreType.Fence, 55), (AgricultureScoreType.Pasture, 28), (AgricultureScoreType.Condition, 76)]),
		("Deep Loam Showpiece", "A near-ideal deep loam field with strong topsoil, fertility, tilth, and dependable condition.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 66), (AgricultureScoreType.Drainage, 72), (AgricultureScoreType.Nutrients, 84), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 90), (AgricultureScoreType.Tilth, 82), (AgricultureScoreType.Rockiness, 6), (AgricultureScoreType.Weeds, 20), (AgricultureScoreType.Pests, 18), (AgricultureScoreType.Fence, 60), (AgricultureScoreType.Pasture, 55), (AgricultureScoreType.Condition, 86)]),
		("Estate Model Farm", "An elite managed farm profile with strong fencing, tidy condition, and broad mixed-use potential.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 66), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 82), (AgricultureScoreType.Salinity, 6), (AgricultureScoreType.Topsoil, 84), (AgricultureScoreType.Tilth, 80), (AgricultureScoreType.Rockiness, 8), (AgricultureScoreType.Weeds, 18), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 82), (AgricultureScoreType.Pasture, 65), (AgricultureScoreType.Condition, 88)]),
		("Paradise Garden", "An exceptional irrigated garden-orchard profile with excellent positives and very few ordinary limits.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 82), (AgricultureScoreType.Drainage, 78), (AgricultureScoreType.Nutrients, 92), (AgricultureScoreType.Salinity, 4), (AgricultureScoreType.Topsoil, 92), (AgricultureScoreType.Tilth, 88), (AgricultureScoreType.Rockiness, 4), (AgricultureScoreType.Weeds, 12), (AgricultureScoreType.Pests, 16), (AgricultureScoreType.Fence, 78), (AgricultureScoreType.Pasture, 72), (AgricultureScoreType.Condition, 94)]),
		("Overcropped Plantation", "A worn plantation field with persistent pests, depleted topsoil, and tired perennial stock.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Orchard, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 62), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 28), (AgricultureScoreType.Salinity, 14), (AgricultureScoreType.Topsoil, 34), (AgricultureScoreType.Tilth, 35), (AgricultureScoreType.Rockiness, 18), (AgricultureScoreType.Weeds, 58), (AgricultureScoreType.Pests, 72), (AgricultureScoreType.Fence, 34), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 25)]),
		("Neglected Orchard", "An old orchard with weak fencing, high weeds and pests, and trees that need pruning or grafting.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Orchard, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 58), (AgricultureScoreType.Drainage, 55), (AgricultureScoreType.Nutrients, 42), (AgricultureScoreType.Salinity, 8), (AgricultureScoreType.Topsoil, 60), (AgricultureScoreType.Tilth, 25), (AgricultureScoreType.Rockiness, 24), (AgricultureScoreType.Weeds, 76), (AgricultureScoreType.Pests, 68), (AgricultureScoreType.Fence, 16), (AgricultureScoreType.Pasture, 28), (AgricultureScoreType.Condition, 24)]),
		("Waterlogged Former Paddy", "A former wet-rice field with too much standing water, poor drainage, and vigorous weeds.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 96), (AgricultureScoreType.Drainage, 12), (AgricultureScoreType.Nutrients, 55), (AgricultureScoreType.Salinity, 18), (AgricultureScoreType.Topsoil, 58), (AgricultureScoreType.Tilth, 28), (AgricultureScoreType.Rockiness, 4), (AgricultureScoreType.Weeds, 78), (AgricultureScoreType.Pests, 54), (AgricultureScoreType.Fence, 18), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 24)]),
		("Loess Plateau Plot", "A dry loess-field profile with deep silt topsoil, weak moisture, and erosion sensitivity.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 34), (AgricultureScoreType.Drainage, 72), (AgricultureScoreType.Nutrients, 58), (AgricultureScoreType.Salinity, 8), (AgricultureScoreType.Topsoil, 78), (AgricultureScoreType.Tilth, 54), (AgricultureScoreType.Rockiness, 12), (AgricultureScoreType.Weeds, 38), (AgricultureScoreType.Pests, 22), (AgricultureScoreType.Fence, 28), (AgricultureScoreType.Pasture, 35), (AgricultureScoreType.Condition, 50)]),
		("Black Earth Steppe Field", "A fertile steppe field with deep dark soil, moderate moisture risk, and low boundary investment.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 46), (AgricultureScoreType.Drainage, 64), (AgricultureScoreType.Nutrients, 82), (AgricultureScoreType.Salinity, 8), (AgricultureScoreType.Topsoil, 88), (AgricultureScoreType.Tilth, 60), (AgricultureScoreType.Rockiness, 8), (AgricultureScoreType.Weeds, 42), (AgricultureScoreType.Pests, 28), (AgricultureScoreType.Fence, 22), (AgricultureScoreType.Pasture, 62), (AgricultureScoreType.Condition, 66)]),
		("Volcanic Ash Garden", "A young volcanic-soil garden with rich minerals, sharp drainage, and some stony roughness.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 56), (AgricultureScoreType.Drainage, 76), (AgricultureScoreType.Nutrients, 78), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 62), (AgricultureScoreType.Tilth, 58), (AgricultureScoreType.Rockiness, 42), (AgricultureScoreType.Weeds, 34), (AgricultureScoreType.Pests, 24), (AgricultureScoreType.Fence, 35), (AgricultureScoreType.Pasture, 28), (AgricultureScoreType.Condition, 62)]),
		("Monsoon Rice Terrace", "A terraced wet-field profile for rice or aquatic crops under strong seasonal water management.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 92), (AgricultureScoreType.Drainage, 32), (AgricultureScoreType.Nutrients, 66), (AgricultureScoreType.Salinity, 12), (AgricultureScoreType.Topsoil, 68), (AgricultureScoreType.Tilth, 56), (AgricultureScoreType.Rockiness, 12), (AgricultureScoreType.Weeds, 54), (AgricultureScoreType.Pests, 46), (AgricultureScoreType.Fence, 28), (AgricultureScoreType.Pasture, 18), (AgricultureScoreType.Condition, 58)]),
		("Chinampa Garden", "A highly fertile wetland raised-field garden with abundant moisture, nutrients, weeds, and pests.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 88), (AgricultureScoreType.Drainage, 52), (AgricultureScoreType.Nutrients, 88), (AgricultureScoreType.Salinity, 8), (AgricultureScoreType.Topsoil, 82), (AgricultureScoreType.Tilth, 76), (AgricultureScoreType.Rockiness, 4), (AgricultureScoreType.Weeds, 62), (AgricultureScoreType.Pests, 54), (AgricultureScoreType.Fence, 38), (AgricultureScoreType.Pasture, 20), (AgricultureScoreType.Condition, 72)]),
		("Andean Waru-Waru Raised Field", "A highland raised-field profile with cold wet soils, good drainage islands, and resilient topsoil.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 78), (AgricultureScoreType.Drainage, 58), (AgricultureScoreType.Nutrients, 62), (AgricultureScoreType.Salinity, 6), (AgricultureScoreType.Topsoil, 70), (AgricultureScoreType.Tilth, 58), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 46), (AgricultureScoreType.Pests, 28), (AgricultureScoreType.Fence, 32), (AgricultureScoreType.Pasture, 45), (AgricultureScoreType.Condition, 62)]),
		("Sahel Millet Field", "A dry Sahelian millet field with low moisture, fair drainage, and modest fertility.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 24), (AgricultureScoreType.Drainage, 70), (AgricultureScoreType.Nutrients, 32), (AgricultureScoreType.Salinity, 12), (AgricultureScoreType.Topsoil, 38), (AgricultureScoreType.Tilth, 42), (AgricultureScoreType.Rockiness, 18), (AgricultureScoreType.Weeds, 36), (AgricultureScoreType.Pests, 28), (AgricultureScoreType.Fence, 20), (AgricultureScoreType.Pasture, 38), (AgricultureScoreType.Condition, 42)]),
		("Flood Recession Field", "An alluvial flood-recession field rich after floods but difficult to hold in stable condition.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 82), (AgricultureScoreType.Drainage, 38), (AgricultureScoreType.Nutrients, 78), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 76), (AgricultureScoreType.Tilth, 48), (AgricultureScoreType.Rockiness, 8), (AgricultureScoreType.Weeds, 58), (AgricultureScoreType.Pests, 44), (AgricultureScoreType.Fence, 20), (AgricultureScoreType.Pasture, 48), (AgricultureScoreType.Condition, 54)]),
		("Savannah Yam Mounds", "A West or Central African mound-field profile with good tilth pockets and heavy weed pressure.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 58), (AgricultureScoreType.Drainage, 62), (AgricultureScoreType.Nutrients, 56), (AgricultureScoreType.Salinity, 6), (AgricultureScoreType.Topsoil, 60), (AgricultureScoreType.Tilth, 64), (AgricultureScoreType.Rockiness, 20), (AgricultureScoreType.Weeds, 66), (AgricultureScoreType.Pests, 52), (AgricultureScoreType.Fence, 24), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 54)]),
		("Slash-and-Burn Swidden", "A fresh swidden clearing with short-term fertility, weak fencing, and fast weed return.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 68), (AgricultureScoreType.Drainage, 58), (AgricultureScoreType.Nutrients, 72), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 52), (AgricultureScoreType.Tilth, 42), (AgricultureScoreType.Rockiness, 26), (AgricultureScoreType.Weeds, 70), (AgricultureScoreType.Pests, 58), (AgricultureScoreType.Fence, 8), (AgricultureScoreType.Pasture, 22), (AgricultureScoreType.Condition, 42)]),
		("Forest Garden Clearing", "A humid agroforestry clearing with strong moisture, mixed perennial potential, and vigorous pests.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 78), (AgricultureScoreType.Drainage, 56), (AgricultureScoreType.Nutrients, 66), (AgricultureScoreType.Salinity, 5), (AgricultureScoreType.Topsoil, 72), (AgricultureScoreType.Tilth, 42), (AgricultureScoreType.Rockiness, 18), (AgricultureScoreType.Weeds, 72), (AgricultureScoreType.Pests, 62), (AgricultureScoreType.Fence, 12), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 56)]),
		("Kraal-Manured Garden", "A compound garden enriched by night-penned livestock, with rich nutrients and modest fence wear.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 54), (AgricultureScoreType.Drainage, 58), (AgricultureScoreType.Nutrients, 88), (AgricultureScoreType.Salinity, 10), (AgricultureScoreType.Topsoil, 64), (AgricultureScoreType.Tilth, 60), (AgricultureScoreType.Rockiness, 14), (AgricultureScoreType.Weeds, 42), (AgricultureScoreType.Pests, 42), (AgricultureScoreType.Fence, 48), (AgricultureScoreType.Pasture, 46), (AgricultureScoreType.Condition, 64)]),
		("Desert Wadi Field", "An arid floodwater field with low ordinary moisture but useful silt and oasis potential.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 28), (AgricultureScoreType.Drainage, 66), (AgricultureScoreType.Nutrients, 58), (AgricultureScoreType.Salinity, 22), (AgricultureScoreType.Topsoil, 54), (AgricultureScoreType.Tilth, 48), (AgricultureScoreType.Rockiness, 28), (AgricultureScoreType.Weeds, 26), (AgricultureScoreType.Pests, 22), (AgricultureScoreType.Fence, 28), (AgricultureScoreType.Pasture, 28), (AgricultureScoreType.Condition, 46)]),
		("Mangrove Rice Polder", "A difficult coastal rice polder with high water, high salinity, poor drainage, and reclamation needs.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 92), (AgricultureScoreType.Drainage, 18), (AgricultureScoreType.Nutrients, 56), (AgricultureScoreType.Salinity, 72), (AgricultureScoreType.Topsoil, 52), (AgricultureScoreType.Tilth, 34), (AgricultureScoreType.Rockiness, 6), (AgricultureScoreType.Weeds, 58), (AgricultureScoreType.Pests, 44), (AgricultureScoreType.Fence, 20), (AgricultureScoreType.Pasture, 12), (AgricultureScoreType.Condition, 28)]),
		("Reclaimed Peat Field", "A drained peat or marsh field with wet organic soil, low condition, and amendment needs.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 82), (AgricultureScoreType.Drainage, 28), (AgricultureScoreType.Nutrients, 34), (AgricultureScoreType.Salinity, 8), (AgricultureScoreType.Topsoil, 58), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 4), (AgricultureScoreType.Weeds, 66), (AgricultureScoreType.Pests, 38), (AgricultureScoreType.Fence, 24), (AgricultureScoreType.Pasture, 42), (AgricultureScoreType.Condition, 24)]),
		("Salt-Affected Irrigated Field", "An irrigation-degraded dryland field with useful moisture but severe salinity pressure.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 72), (AgricultureScoreType.Drainage, 50), (AgricultureScoreType.Nutrients, 48), (AgricultureScoreType.Salinity, 78), (AgricultureScoreType.Topsoil, 50), (AgricultureScoreType.Tilth, 46), (AgricultureScoreType.Rockiness, 12), (AgricultureScoreType.Weeds, 42), (AgricultureScoreType.Pests, 30), (AgricultureScoreType.Fence, 32), (AgricultureScoreType.Pasture, 32), (AgricultureScoreType.Condition, 30)]),
		("Laterite Upland Garden", "A tropical upland garden with lateritic low fertility, thin topsoil, and heavy weed competition.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard], [(AgricultureScoreType.Moisture, 58), (AgricultureScoreType.Drainage, 62), (AgricultureScoreType.Nutrients, 24), (AgricultureScoreType.Salinity, 6), (AgricultureScoreType.Topsoil, 32), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 34), (AgricultureScoreType.Weeds, 70), (AgricultureScoreType.Pests, 52), (AgricultureScoreType.Fence, 16), (AgricultureScoreType.Pasture, 24), (AgricultureScoreType.Condition, 30)]),
		("Cold Boreal Clearing", "A cold northern clearing with short-season moisture, low nutrients, and mixed crop-pasture potential.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture, AgricultureFieldUse.Woodland], [(AgricultureScoreType.Moisture, 62), (AgricultureScoreType.Drainage, 54), (AgricultureScoreType.Nutrients, 30), (AgricultureScoreType.Salinity, 4), (AgricultureScoreType.Topsoil, 44), (AgricultureScoreType.Tilth, 32), (AgricultureScoreType.Rockiness, 32), (AgricultureScoreType.Weeds, 42), (AgricultureScoreType.Pests, 18), (AgricultureScoreType.Fence, 22), (AgricultureScoreType.Pasture, 40), (AgricultureScoreType.Condition, 38)]),
		("Karst Terrace", "A dry limestone terrace with high drainage and rockiness but some orchard and crop potential.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Orchard, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 34), (AgricultureScoreType.Drainage, 82), (AgricultureScoreType.Nutrients, 38), (AgricultureScoreType.Salinity, 8), (AgricultureScoreType.Topsoil, 32), (AgricultureScoreType.Tilth, 38), (AgricultureScoreType.Rockiness, 70), (AgricultureScoreType.Weeds, 36), (AgricultureScoreType.Pests, 18), (AgricultureScoreType.Fence, 34), (AgricultureScoreType.Pasture, 30), (AgricultureScoreType.Condition, 42)]),
		("Windblown Sand Field", "A sandy coastal or desert field with sharp drainage, poor nutrients, thin topsoil, and salt risk.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 26), (AgricultureScoreType.Drainage, 92), (AgricultureScoreType.Nutrients, 18), (AgricultureScoreType.Salinity, 34), (AgricultureScoreType.Topsoil, 18), (AgricultureScoreType.Tilth, 32), (AgricultureScoreType.Rockiness, 4), (AgricultureScoreType.Weeds, 32), (AgricultureScoreType.Pests, 16), (AgricultureScoreType.Fence, 12), (AgricultureScoreType.Pasture, 22), (AgricultureScoreType.Condition, 22)]),
		("Hurricane-Damaged Plantation", "A storm-damaged plantation with broken fences, poor condition, pests, and regrowth pressure.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Orchard, AgricultureFieldUse.Crop], [(AgricultureScoreType.Moisture, 76), (AgricultureScoreType.Drainage, 44), (AgricultureScoreType.Nutrients, 50), (AgricultureScoreType.Salinity, 14), (AgricultureScoreType.Topsoil, 58), (AgricultureScoreType.Tilth, 30), (AgricultureScoreType.Rockiness, 18), (AgricultureScoreType.Weeds, 72), (AgricultureScoreType.Pests, 66), (AgricultureScoreType.Fence, 8), (AgricultureScoreType.Pasture, 18), (AgricultureScoreType.Condition, 18)]),
		("Frost Pocket Field", "A cool valley-bottom field with good soil and moisture but poor condition from recurring frost risk.", [AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureFieldUse.Pasture], [(AgricultureScoreType.Moisture, 70), (AgricultureScoreType.Drainage, 52), (AgricultureScoreType.Nutrients, 58), (AgricultureScoreType.Salinity, 6), (AgricultureScoreType.Topsoil, 68), (AgricultureScoreType.Tilth, 50), (AgricultureScoreType.Rockiness, 14), (AgricultureScoreType.Weeds, 40), (AgricultureScoreType.Pests, 20), (AgricultureScoreType.Fence, 32), (AgricultureScoreType.Pasture, 42), (AgricultureScoreType.Condition, 34)])
	];

	private static readonly CropSeed[] Crops =
	[
		new("Wheat", "A cool-season bread or flour wheat crop.", "grain", 110, 18, 35, 75, 2, 32, [Yield("wheat", 2500000), Yield("straw", 1200000)]),
		new("Barley", "A hardy cereal crop used for food, fodder, and brewing.", "grain", 95, 16, 30, 75, 0, 32, [Yield("barley", 2200000), Yield("straw", 1100000)]),
		new("Emmer Wheat", "An ancient hulled wheat crop once widely grown across the Near East and Mediterranean.", "grain", 120, 18, 30, 70, 0, 32, [Yield("emmer wheat", 1600000), Yield("straw", 900000)]),
		new("Einkorn Wheat", "A low-yielding ancient hulled wheat suited to poor dry soils and archaic farming settings.", "grain", 115, 18, 20, 65, -2, 30, [Yield("einkorn wheat", 1200000), Yield("straw", 700000)]),
		new("Spelt Wheat", "A hardy hulled wheat crop useful for heritage, upland, or lower-input agriculture.", "grain", 125, 20, 35, 75, -3, 30, [Yield("spelt wheat", 1800000), Yield("straw", 1000000)]),
		new("Naked Barley", "A free-threshing barley crop grown where grain is eaten directly rather than hulled.", "grain", 90, 14, 25, 70, 0, 32, [Yield("naked barley", 1900000), Yield("straw", 900000)]),
		new("New Glume Wheat", "An extinct archaeological hulled wheat type for deep-prehistoric or alternate-history agriculture.", "grain", 120, 18, 30, 70, 0, 32, [Yield("new glume wheat", 1300000), Yield("straw", 800000)]),
		new("Rye", "A cold-tolerant cereal crop for poorer or cooler soils.", "grain", 120, 18, 25, 70, -3, 28, [Yield("rye", 2000000), Yield("straw", 1200000)]),
		new("Oats", "A cool, moist cereal crop useful as food grain and fodder.", "grain", 100, 16, 40, 85, 0, 30, [Yield("oat", 1800000), Yield("straw", 1000000)]),
		new("Rice", "A water-loving paddy grain crop for warm growing seasons.", "grain", 120, 20, 70, 100, 16, 40, [Yield("rice", 3000000), Yield("straw", 1500000)]),
		new("African Rice", "A warm wetland rice crop domesticated in West Africa.", "grain", 120, 20, 65, 100, 16, 40, [Yield("african rice", 1800000), Yield("straw", 900000)]),
		new("Maize", "A warm-season maize or corn crop with heavy grain and stalk yields.", "grain", 115, 16, 40, 85, 12, 38, [Yield("corn", 3500000), Yield("straw", 1800000)]),
		new("Sorghum", "A heat-tolerant dryland cereal crop.", "grain", 110, 18, 25, 70, 15, 42, [Yield("sorghum", 2200000), Yield("straw", 1000000)]),
		new("Millet", "A short-season dryland millet crop.", "grain", 80, 14, 20, 65, 15, 42, [Yield("millet", 1600000), Yield("straw", 800000)]),
		new("Teff", "A tiny-seeded Ethiopian grain crop used for injera, porridge, fodder, and straw.", "grain", 95, 14, 35, 80, 10, 34, [Yield("teff", 1200000), Yield("straw", 600000)]),
		new("Fonio", "A fast-maturing West African grain crop suited to short rains and poor soils.", "grain", 70, 10, 20, 60, 18, 38, [Yield("fonio", 800000), Yield("straw", 300000)]),
		new("Finger Millet", "A warm-season millet crop valued in Africa and South Asia for durable small grain.", "grain", 110, 16, 25, 70, 12, 38, [Yield("finger millet", 1400000), Yield("straw", 600000)]),
		new("Pearl Millet", "A hot dryland millet crop for grain and fodder in semi-arid regions.", "grain", 90, 14, 15, 60, 18, 42, [Yield("pearl millet", 1500000), Yield("straw", 700000)]),
		new("Foxtail Millet", "A quick small millet crop historically important across northern China and Eurasian steppe margins.", "grain", 85, 12, 20, 65, 10, 36, [Yield("foxtail millet", 1200000), Yield("straw", 500000)]),
		new("Proso Millet", "A short-season millet crop suited to dry continental summers.", "grain", 75, 12, 15, 60, 10, 38, [Yield("proso millet", 1100000), Yield("straw", 450000)]),
		new("Buckwheat", "A fast-growing pseudocereal suited to short seasons.", "grain", 75, 12, 35, 80, 8, 30, [Yield("buckwheat", 1400000), Yield("straw", 500000)]),
		new("Quinoa", "A hardy pseudocereal crop tolerant of cool nights and dry air.", "grain", 100, 16, 20, 65, 2, 32, [Yield("quinoa", 1400000), Yield("straw", 500000)]),
		new("Amaranth", "A warm-season pseudocereal grown for protein-rich seed and edible leaves.", "grain", 100, 16, 25, 70, 12, 38, [Yield("amaranth", 1200000), Yield("vegetation", 400000)]),
		new("Canihua", "A high-Andean canihua pseudocereal related to quinoa and suited to cold highlands.", "grain", 115, 16, 20, 65, 0, 30, [Yield("canihua", 900000), Yield("straw", 300000)]),
		new("Pitseed Goosefoot", "A prehistoric eastern North American chenopod grain crop now represented mostly by wild relatives.", "grain", 90, 14, 25, 70, 4, 32, [Yield("pitseed goosefoot", 700000), Yield("vegetation", 300000)]),
		new("Marshelder", "A prehistoric North American oilseed crop also known as sumpweed.", "oilseed", 100, 16, 30, 75, 8, 35, [Yield("marshelder seed", 600000), Yield("vegetation", 300000)]),
		new("Maygrass", "A prehistoric North American starchy seed crop from wet meadows and bottomlands.", "grain", 80, 12, 35, 85, 4, 30, [Yield("maygrass", 600000), Yield("straw", 250000)]),
		new("Little Barley", "A native North American small-grain barley once cultivated in prehistoric fields.", "grain", 85, 12, 20, 65, 2, 32, [Yield("little barley", 650000), Yield("straw", 250000)]),
		new("Erect Knotweed", "A prehistoric North American starchy seed crop from disturbed riverine soils.", "grain", 90, 14, 30, 80, 4, 32, [Yield("erect knotweed", 650000), Yield("vegetation", 250000)]),
		new("Field Beans", "A broad bean or dry bean pulse crop.", "legume", 95, 14, 35, 80, 5, 32, [Yield("bean", 1800000), Yield("straw", 600000)]),
		new("Peas", "A cool-season pea crop for green or dry pulses.", "legume", 80, 12, 40, 80, 2, 28, [Yield("pea", 1500000), Yield("straw", 500000)]),
		new("Lentils", "A dryland pulse crop that prefers free-draining soil.", "legume", 100, 14, 25, 65, 5, 32, [Yield("lentil", 1200000), Yield("straw", 400000)]),
		new("Chickpeas", "A warm, dry pulse crop for well-drained fields.", "legume", 110, 16, 25, 65, 10, 35, [Yield("chickpea", 1400000), Yield("straw", 400000)]),
		new("Soybeans", "A warm-season oil and protein bean crop.", "legume", 115, 16, 40, 85, 12, 36, [Yield("soybean", 2200000), Yield("straw", 600000)]),
		new("Peanuts", "A warm-season groundnut crop harvested from loose soil.", "legume", 130, 18, 35, 75, 15, 38, [Yield("peanut crop", 1800000), Yield("hay", 500000)]),
		new("Bitter Vetch", "A founder-crop pulse grown in ancient Near Eastern farming systems and mostly replaced today.", "legume", 95, 14, 25, 65, 2, 32, [Yield("bitter vetch", 900000), Yield("straw", 300000)]),
		new("Grass Peas", "A hardy dryland pulse able to produce in poor seasons and marginal fields.", "legume", 100, 14, 20, 65, 4, 35, [Yield("grass pea", 1000000), Yield("straw", 300000)]),
		new("Cowpeas", "A warm-climate bean crop important in African and tropical dryland agriculture.", "legume", 85, 14, 25, 70, 14, 40, [Yield("cowpea", 1200000), Yield("hay", 400000)]),
		new("Mung Beans", "A short-season warm-climate pulse crop for small edible beans and sprouts.", "legume", 75, 12, 35, 80, 14, 38, [Yield("mung bean", 1000000), Yield("straw", 300000)]),
		new("Pigeon Peas", "A long-season tropical pulse shrub grown for peas, fodder, and resilience in dry heat.", "legume", 170, 24, 25, 70, 16, 42, [Yield("pigeon pea", 1200000), Yield("hay", 500000)]),
		new("Bambara Groundnuts", "A drought-tolerant African groundnut pulse that sets its pods underground.", "legume", 130, 18, 25, 65, 16, 40, [Yield("bambara groundnut", 1000000), Yield("hay", 350000)]),
		new("Lupins", "A cool-season pulse and fodder crop useful on poor or acidic soils.", "legume", 110, 16, 25, 70, 2, 30, [Yield("lupin", 1200000), Yield("straw", 400000)]),
		new("Adzuki Beans", "A warm-season East Asian pulse crop grown for small red beans.", "legume", 95, 14, 35, 75, 12, 36, [Yield("adzuki bean", 1100000), Yield("straw", 300000)]),
		new("Potatoes", "A cool-season tuber crop with high bulk yield.", "root", 100, 16, 45, 85, 4, 28, [Yield("potato", 12000000)]),
		new("Sweet Potatoes", "A warm-season sweet tuber crop.", "root", 120, 18, 40, 80, 15, 38, [Yield("sweet potato", 10000000)]),
		new("Cassava", "A tropical starch root crop tolerant of poor soils.", "root", 240, 35, 35, 75, 18, 42, [Yield("cassava", 16000000)]),
		new("Taro", "A wet-soil tuber crop for warm, damp fields.", "root", 210, 30, 65, 100, 18, 38, [Yield("taro", 10000000)]),
		new("Yams", "A long-season warm-climate tuber crop.", "root", 220, 30, 40, 85, 18, 40, [Yield("yam", 11000000)]),
		new("Oca", "An Andean tuber crop suited to cool mountain fields.", "root", 160, 24, 35, 80, 2, 28, [Yield("oca", 6000000)]),
		new("Ulluco", "A cool highland Andean tuber crop producing waxy edible tubers.", "root", 150, 22, 45, 85, 2, 28, [Yield("ulluco", 5500000)]),
		new("Mashua", "A hardy Andean tuber crop with pest-resistant vines and edible roots.", "root", 160, 24, 35, 80, 2, 28, [Yield("mashua", 5500000)]),
		new("Arrowroot", "A warm humid starch-root crop grown for rhizomes.", "root", 240, 35, 50, 95, 18, 38, [Yield("arrowroot", 8000000)]),
		new("Lotus Root", "A warm wetland crop grown in shallow water for edible rhizomes.", "root", 180, 28, 75, 100, 14, 36, [Yield("lotus root", 7000000)]),
		new("Water Chestnuts", "A hot wetland crop grown for crisp edible corms.", "root", 180, 28, 80, 100, 16, 38, [Yield("water chestnut", 6000000)]),
		new("Jerusalem Artichokes", "A hardy sunflower-relative tuber crop for food or fodder.", "root", 130, 20, 25, 70, -2, 32, [Yield("jerusalem artichoke", 9000000)]),
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
		new("Eggplants", "A warm-season aubergine crop for fleshy fruit.", "vegetable", 100, 18, 40, 80, 14, 38, [Yield("aubergine", 7000000)]),
		new("Okra", "A hot-season vegetable crop for edible pods.", "vegetable", 70, 14, 30, 75, 18, 42, [Yield("okra", 4000000)]),
		new("Bottle Gourds", "A warm-season gourd crop for edible young fruit or dried containers.", "vegetable", 100, 20, 35, 80, 14, 38, [Yield("bottle gourd", 8000000)]),
		new("Sugarcane", "A tall tropical cane crop for sugar and syrup.", "industrial", 300, 45, 45, 90, 18, 42, [Yield("sugarcane", 30000000)]),
		new("Tobacco", "A warm long-season leaf crop grown for curing and later tobacco processing.", "industrial", 140, 22, 30, 75, 14, 40, [Yield("tobacco leaf", 1800000)], PlantingGroups: HotLongSeasonPlanting),
		new("Sugar Beet", "A temperate beet crop grown for sugar-rich roots.", "industrial", 160, 24, 35, 80, 3, 30, [Yield("sugar beet", 18000000)]),
		new("Sunflower", "A warm oilseed crop with large flower heads.", "oilseed", 110, 18, 30, 75, 10, 36, [Yield("sunflower", 1800000), Yield("straw", 500000)]),
		new("Sesame", "A hot-climate oilseed crop.", "oilseed", 100, 14, 25, 65, 18, 42, [Yield("sesame", 900000), Yield("straw", 300000)]),
		new("Canola", "A cool-season brassica oilseed crop.", "oilseed", 120, 18, 35, 80, 2, 30, [Yield("canola", 1800000), Yield("straw", 700000)]),
		new("Chia", "A warm-climate seed crop grown for oil-rich edible seeds.", "oilseed", 110, 16, 25, 65, 12, 36, [Yield("chia", 700000), Yield("straw", 250000)]),
		new("Safflower", "A dryland oilseed and dye crop with thistle-like flower heads.", "oilseed", 120, 18, 15, 60, 8, 38, [Yield("safflower", 900000), Yield("straw", 300000)]),
		new("Niger Seed", "A warm oilseed crop important in parts of Ethiopia and India.", "oilseed", 100, 14, 25, 70, 12, 36, [Yield("niger seed", 700000), Yield("straw", 250000)]),
		new("Mustard", "A brassica seed crop for spice, greens, and oil.", "oilseed", 90, 14, 30, 80, 2, 30, [Yield("mustard seed", 900000), Yield("straw", 300000)]),
		new("Coriander", "A seed-spice and herb crop suited to irrigated gardens and cool-season fields.", "spice", 90, 14, 30, 75, 5, 32, [Yield("coriander", 450000), Yield("vegetation", 200000)]),
		new("Cumin", "A warm dryland seed-spice crop for aromatic cumin seed.", "spice", 110, 16, 20, 65, 10, 38, [Yield("cumin", 350000), Yield("straw", 150000)]),
		new("Cardamom", "A humid tropical spice crop grown for aromatic seed pods.", "spice", 240, 28, 60, 95, 18, 38, [Yield("cardamom", 300000)], PlantingGroups: HotLongSeasonPlanting),
		new("Saffron Crocus", "A cool-season crocus grown for labour-intensive saffron stigmas.", "spice", 150, 18, 20, 65, -2, 30, [Yield("saffron crocus", 80000), Yield("vegetation", 100000)]),
		new("Chamomile", "A cool-season medicinal flower crop for dried apothecary stock.", "herb", 90, 14, 25, 75, 0, 30, [Yield("chamomile", 300000)], PlantingGroups: CoolSeasonPlanting),
		new("Lavender", "A hardy aromatic medicinal herb crop for flowers and apothecary use.", "herb", 150, 18, 20, 65, -5, 34, [Yield("lavender", 450000)], PlantingGroups: CoolSeasonPlanting),
		new("Yarrow", "A hardy medicinal herb crop for leaves and flowering tops.", "herb", 100, 14, 20, 70, -5, 32, [Yield("yarrow", 350000)], PlantingGroups: CoolSeasonPlanting),
		new("Foxglove", "A cool-climate medicinal plant crop whose harvested leaves require careful handling.", "herb", 180, 20, 30, 80, -5, 28, [Yield("foxglove", 300000)], PlantingGroups: CoolSeasonPlanting),
		new("Henbane", "A cool-season medicinal and poisonous herb crop for controlled apothecary use.", "herb", 110, 16, 20, 65, 0, 32, [Yield("henbane", 250000)], PlantingGroups: CoolSeasonPlanting),
		new("Mandrake", "A long-season medicinal root crop for controlled apothecary use.", "herb", 210, 24, 25, 70, 0, 34, [Yield("mandrake", 350000)], PlantingGroups: CoolSeasonPlanting),
		new("Cotton", "A warm-season fibre crop producing cotton bolls.", "fibre", 160, 24, 35, 75, 15, 40, [Yield("cotton crop", 1800000), Yield("straw", 700000)]),
		new("Flax", "A cool-season fibre and seed crop.", "fibre", 110, 18, 35, 75, 2, 30, [Yield("flax", 1400000), Yield("straw", 800000)]),
		new("Hemp", "A tall bast-fibre crop for fibre, seed, or biomass.", "fibre", 120, 18, 35, 80, 8, 35, [Yield("hemp crop", 2500000), Yield("straw", 1200000)]),
		new("Jute", "A hot, humid bast-fibre crop.", "fibre", 140, 20, 60, 95, 18, 40, [Yield("jute crop", 2200000), Yield("straw", 1000000)]),
		new("Ramie", "A perennial nettle-family fibre crop.", "fibre", 130, 20, 45, 90, 12, 38,
			[Yield("ramie", 1800000), Yield("ramie cloth", 450000), Yield("straw", 800000)]),
		new("Sisal", "A dryland leaf-fibre crop.", "fibre", 240, 35, 15, 55, 12, 42, [Yield("sisal", 2000000)]),
		new("Kenaf", "A hot-season bast-fibre crop related to hibiscus.", "fibre", 140, 20, 45, 90, 16, 40, [Yield("kenaf", 1800000), Yield("straw", 800000)]),
		new("Madder", "A deep-rooted dye crop grown for red dye roots.", "industrial", 730, 45, 25, 70, 0, 35, [Yield("madder root", 900000), Yield("vegetation", 300000)]),
		new("Weld", "A cool-season yellow dye crop grown for flowering stems and leaves.", "industrial", 120, 18, 25, 70, 0, 32, [Yield("weld", 900000), Yield("vegetation", 250000)]),
		new("Alkanet", "A dryland dye herb grown for purple-red roots.", "industrial", 240, 28, 20, 60, 2, 36, [Yield("alkanet root", 650000), Yield("vegetation", 200000)]),
		new("Indigo", "A hot-climate dye crop grown for leaves used in blue dye production.", "industrial", 130, 20, 35, 85, 16, 40, [Yield("indigo crop", 1200000), Yield("vegetation", 400000)]),
		new("Woad", "A cool-season dye crop grown for blue-dye leaves in temperate fields.", "industrial", 110, 18, 35, 80, 0, 30, [Yield("woad leaves", 1000000), Yield("vegetation", 300000)]),
		new("Tepary Beans", "A drought-tolerant bean crop for arid and semi-arid warm-season agriculture.", "pulse", 85, 14, 15, 55, 12, 42, [Yield("tepary bean", 900000), Yield("straw", 350000)], PlantingGroups: WarmSeasonPlanting),
		new("Lima Beans", "A warm-season bean crop for tropical, subtropical, and garden-field settings.", "pulse", 95, 16, 35, 80, 14, 38, [Yield("lima bean", 1300000), Yield("straw", 450000)], PlantingGroups: WarmSeasonPlanting),
		new("Runner Beans", "A vigorous climbing bean crop for warm gardens and trellised field edges.", "pulse", 95, 18, 35, 80, 10, 34, [Yield("runner bean", 1200000), Yield("straw", 400000)], PlantingGroups: WarmSeasonPlanting),
		new("Wild Rice", "A wetland grain crop suited to marsh margins, lakes, and managed water plots.", "grain", 120, 18, 70, 100, 8, 32, [Yield("wild rice", 1300000), Yield("straw", 600000)], PlantingGroups: WarmSeasonPlanting),
		new("Groundnut", "A native American tuber legume grown for starchy groundnut tubers.", "root", 130, 20, 45, 85, 8, 34, [Yield("groundnut tuber", 1600000), Yield("vegetation", 500000)], PlantingGroups: WarmSeasonPlanting),
		new("Camas", "A cool-season bulb crop grown for edible camas bulbs in meadow and upland plots.", "root", 160, 18, 35, 75, -5, 28, [Yield("camas bulb", 900000), Yield("vegetation", 250000)], PlantingGroups: CoolSeasonPlanting),
		new("Prairie Turnip", "A hardy dryland root crop for plains and marginal warm-season fields.", "root", 110, 16, 15, 55, 5, 36, [Yield("prairie turnip", 950000), Yield("vegetation", 250000)], PlantingGroups: WarmSeasonPlanting),
		new("Tomatillos", "A warm garden crop grown for tart husked fruits.", "vegetable", 85, 18, 35, 80, 12, 35, [Yield("tomatillo", 1800000)], PlantingGroups: WarmSeasonPlanting),
		new("Chayote", "A warm humid cucurbit vine grown for crisp green fruits.", "vegetable", 115, 22, 50, 95, 14, 38, [Yield("chayote", 3000000), Yield("vegetation", 600000)], PlantingGroups: WarmSeasonPlanting),
		new("Jicama", "A long-season warm root crop grown for crunchy edible storage roots.", "root", 150, 20, 35, 80, 16, 40, [Yield("jicama", 1800000), Yield("vegetation", 350000)], PlantingGroups: HotLongSeasonPlanting),
		new("Maca", "A cool Andean root crop suited to highland short-season fields.", "root", 135, 18, 30, 75, -5, 24, [Yield("maca root", 800000), Yield("vegetation", 250000)], PlantingGroups: CoolSeasonPlanting),
		new("Yacon", "A cool to mild Andean root crop grown for sweet crisp tubers.", "root", 145, 20, 35, 80, 2, 30, [Yield("yacon root", 1500000), Yield("vegetation", 400000)], PlantingGroups: CoolSeasonPlanting),
		new("Arracacha", "A long-season Andean root crop for warm highland gardens.", "root", 180, 24, 40, 85, 10, 32, [Yield("arracacha root", 1600000), Yield("vegetation", 350000)], PlantingGroups: HotLongSeasonPlanting),
		new("Tarwi", "A cool Andean lupin crop grown for protein-rich beans.", "pulse", 130, 18, 30, 75, -2, 28, [Yield("tarwi bean", 1000000), Yield("straw", 350000)], PlantingGroups: CoolSeasonPlanting),
		new("Ahipa", "A warm Andean leguminous root crop for edible storage roots.", "root", 130, 18, 35, 80, 8, 34, [Yield("ahipa root", 1000000), Yield("vegetation", 300000)], PlantingGroups: WarmSeasonPlanting),
		new("Enset", "A highland African starch crop grown for bulky false-banana pseudostem starch.", "starch", 240, 30, 50, 95, 8, 32, [Yield("enset starch", 2500000), Yield("vegetation", 1400000)], PlantingGroups: HotLongSeasonPlanting),
		new("Ethiopian Oats", "A cool highland oat crop for grain and straw.", "grain", 100, 16, 35, 80, -2, 28, [Yield("ethiopian oat", 1500000), Yield("straw", 800000)], PlantingGroups: CoolSeasonPlanting),
		new("Guinea Millet", "A warm West African millet crop for dryland grain fields.", "grain", 95, 16, 20, 65, 12, 40, [Yield("guinea millet", 1200000), Yield("straw", 600000)], PlantingGroups: WarmSeasonPlanting),
		new("Egusi Melons", "A warm African cucurbit crop grown mainly for oil-rich seeds.", "oilseed", 105, 18, 35, 85, 14, 40, [Yield("egusi seed", 750000), Yield("vegetation", 500000)], PlantingGroups: WarmSeasonPlanting),
		new("Roselle", "A hot-season hibiscus crop grown for calyces, leaves, and useful fibre.", "industrial", 120, 20, 40, 90, 16, 42, [Yield("roselle", 1000000), Yield("vegetation", 450000)], PlantingGroups: HotLongSeasonPlanting),
		new("African Yam Beans", "A West African pulse and tuber crop for warm mixed fields.", "pulse", 130, 20, 35, 85, 12, 38, [Yield("african yam bean", 900000), Yield("straw", 350000)], PlantingGroups: WarmSeasonPlanting),
		new("Kersting's Groundnuts", "A drought-tolerant West African pulse crop with modest fodder value.", "pulse", 95, 16, 20, 65, 12, 40, [Yield("kersting groundnut", 750000), Yield("hay", 350000)], PlantingGroups: WarmSeasonPlanting),
		new("Lablab Beans", "A tropical pulse and fodder bean for warm fields and pasture edges.", "pulse", 110, 18, 30, 80, 12, 40, [Yield("lablab bean", 1000000), Yield("hay", 500000)], PlantingGroups: WarmSeasonPlanting),
		new("Fluted Pumpkins", "A warm African vegetable crop grown for leaves and edible seeds.", "vegetable", 100, 18, 45, 90, 15, 40, [Yield("fluted pumpkin leaf", 1200000), Yield("pumpkin seed", 350000)], PlantingGroups: WarmSeasonPlanting),
		new("Jute Mallow", "A hot-season leafy vegetable crop for humid gardens and market plots.", "vegetable", 60, 12, 45, 95, 18, 42, [Yield("jute mallow", 900000)], PlantingGroups: HotLongSeasonPlanting),
		new("African Eggplants", "A warm garden-egg crop for African market gardens and tropical plots.", "vegetable", 95, 18, 35, 85, 14, 38, [Yield("garden egg", 1800000)], PlantingGroups: WarmSeasonPlanting),
		new("Spinach", "A cool-season leafy vegetable for market gardens and household plots.", "vegetable", 45, 10, 35, 80, -2, 24, [Yield("spinach", 900000)], PlantingGroups: CoolSeasonPlanting),
		new("Kale", "A hardy cool-season brassica crop for leaves and winter greens.", "vegetable", 75, 16, 35, 85, -5, 28, [Yield("kale", 1400000)], PlantingGroups: CoolSeasonPlanting),
		new("Chard", "A cool-season beet-relative crop grown for broad edible leaves.", "vegetable", 65, 14, 35, 85, 0, 30, [Yield("chard", 1200000)], PlantingGroups: CoolSeasonPlanting),
		new("Celery", "A cool, moisture-loving market-garden crop for crisp stalks.", "vegetable", 110, 18, 55, 95, 2, 26, [Yield("celery", 1200000)], PlantingGroups: CoolSeasonPlanting),
		new("Parsnips", "A cool-season root crop that sweetens in cold ground.", "root", 125, 20, 35, 85, -5, 26, [Yield("parsnip", 1800000), Yield("vegetation", 300000)], PlantingGroups: CoolSeasonPlanting),
		new("Radishes", "A fast cool-season root crop useful for kitchen gardens and market plots.", "root", 35, 8, 35, 80, -2, 26, [Yield("radish", 900000), Yield("vegetation", 200000)], PlantingGroups: CoolSeasonPlanting),
		new("Leeks", "A hardy cool-season allium crop for winter and spring harvest.", "vegetable", 120, 20, 35, 85, -5, 28, [Yield("leek", 1200000), Yield("vegetation", 200000)], PlantingGroups: CoolSeasonPlanting),
		new("Rutabagas", "A cool-season food and fodder root crop for later medieval and early modern settings.", "root", 110, 18, 35, 85, -5, 28, [Yield("rutabaga", 2200000), Yield("vegetation", 350000)], PlantingGroups: CoolSeasonPlanting),
		new("Chicory", "A cool-season leaf and root crop for bitter greens or roasted-root use.", "vegetable", 100, 18, 30, 80, -2, 28, [Yield("chicory", 900000), Yield("vegetation", 250000)], PlantingGroups: CoolSeasonPlanting),
		new("Sainfoin", "A cool-season fodder legume grown for hay and soil-improving leys.", "fodder", 90, 16, 25, 75, -5, 30, [Yield("sainfoin hay", 1600000), Yield("hay", 600000)], PlantingGroups: CoolSeasonPlanting),
		new("Clover", "A cool-season ley and fodder crop for hay, rotation, and green manure.", "fodder", 80, 16, 35, 85, -5, 30, [Yield("clover hay", 1800000), Yield("hay", 700000)], PlantingGroups: CoolSeasonPlanting),
		new("Mangelwurzel", "A cool-season fodder beet crop for stock feed and early industrial agriculture.", "root", 130, 20, 35, 85, -2, 28, [Yield("mangelwurzel", 2600000), Yield("vegetation", 400000)], PlantingGroups: CoolSeasonPlanting),
		new("Poppy Seed", "A neutral oilseed and spice crop grown for culinary poppy seed.", "oilseed", 100, 16, 25, 75, -2, 30, [Yield("poppy seed", 450000), Yield("straw", 350000)], PlantingGroups: CoolSeasonPlanting),
		new("Melons", "A warm cucurbit crop for table melons such as cantaloupe and honeydew.", "vegetable", 95, 18, 35, 85, 12, 38, [Yield("cantaloupe", 2500000), Yield("honeydew", 2500000)], PlantingGroups: WarmSeasonPlanting),
		new("Watermelons", "A warm African-origin cucurbit crop for large sweet watermelons.", "vegetable", 100, 18, 35, 85, 14, 40, [Yield("watermelon", 6500000)], PlantingGroups: WarmSeasonPlanting)
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
		new("Hazelnuts", "A cool-temperate nut orchard crop.", "orchard", 1095, 22, 35, 80, -5, 32, [Yield("hazelnut", 2200000), Yield("hazel", 300000)], true, 230),
		new("Mangoes", "A tropical mango orchard crop for sweet stone fruit.", "orchard", 1095, 28, 40, 85, 10, 42, [Yield("mango", 8000000)], true, 240),
		new("Coconuts", "A tropical coconut palm crop for nuts, husk, and palm products.", "orchard", 1825, 35, 50, 95, 18, 42, [Yield("coconut", 7000000)], true, 365),
		new("Plantains", "A tropical plantain plantation crop for starchy cooking fruit.", "orchard", 480, 35, 60, 95, 18, 40, [Yield("plantain", 16000000)], true, 300),
		new("Breadfruit", "A humid tropical tree crop producing starchy fruit and workable inner bark.", "orchard",
			1095, 30, 60, 95, 18, 40, [Yield("breadfruit", 14000000), Yield("barkcloth", 350000)], true, 260),
		new("Avocados", "A subtropical avocado orchard crop for oily fruit.", "orchard", 1095, 28, 35, 80, 5, 38, [Yield("avocado", 7000000)], true, 260),
		new("Cacao", "A humid tropical cacao plantation crop for pods and beans.", "orchard", 1460, 35, 70, 100, 18, 38, [Yield("cacao", 2500000), Yield("cacao bean", 800000)], true, 300),
		new("Coffee", "A subtropical coffee shrub crop for beans.", "orchard", 1095, 30, 50, 90, 10, 32, [Yield("coffee bean", 2200000)], true, 280),
		new("Tea", "A humid subtropical tea shrub crop for harvested leaves.", "orchard", 1095, 24, 55, 95, 5, 32, [Yield("tea leaf", 3000000)], true, 180),
		new("Pomegranates", "A dry-climate pomegranate orchard crop.", "orchard", 730, 24, 20, 70, 0, 42, [Yield("pomegranate", 6000000)], true, 230),
		new("Quinces", "A cool-temperate quince orchard crop.", "orchard", 1095, 24, 30, 75, -8, 35, [Yield("quince", 6000000)], true, 220),
		new("Apricots", "A warm-temperate stone-fruit orchard crop.", "orchard", 730, 22, 25, 70, -5, 38, [Yield("apricot", 6500000)], true, 210),
		new("Walnuts", "A temperate walnut orchard crop for rich nuts and occasional wood.", "orchard", 1460, 26, 30, 75, -8, 36, [Yield("walnut nut", 2500000)], true, 260),
		new("Pistachios", "A dry-climate pistachio nut orchard crop.", "orchard", 1460, 26, 15, 60, -2, 42, [Yield("pistachio", 1800000)], true, 260),
		new("Cashews", "A tropical cashew orchard crop for nuts and fruit.", "orchard", 1095, 28, 25, 75, 15, 42, [Yield("cashew", 1800000)], true, 260),
		new("Limes", "A frost-sensitive citrus orchard crop for sour limes.", "orchard", 1095, 30, 35, 85, 5, 40, [Yield("lime", 6500000)], true, 240),
		new("Grapefruits", "A subtropical citrus orchard crop for large bitter-sweet fruit.", "orchard", 1095, 30, 35, 85, 0, 38, [Yield("grapefruit", 8000000)], true, 260),
		new("Mandarins", "A small-fruited citrus orchard crop for sweet mandarins and related easy-peeling fruit.", "orchard", 1095, 28, 35, 85, 0, 38, [Yield("mandarin", 7000000)], true, 240),
		new("Persimmons", "A temperate to subtropical persimmon orchard crop for late-season fruit.", "orchard", 1095, 24, 25, 75, -8, 38, [Yield("persimmon", 6000000)], true, 220),
		new("Mulberries", "A hardy mulberry orchard or silkworm-feed tree crop producing small aggregate fruit.", "orchard", 730, 24, 25, 80, -10, 38, [Yield("mulberry", 5000000)], true, 210),
		new("Chestnuts", "A temperate chestnut orchard crop for staple nuts.", "orchard", 1460, 26, 35, 80, -10, 35, [Yield("chestnut", 2500000)], true, 260),
		new("Pecans", "A warm-temperate pecan nut orchard crop for river flats and deep soils.", "orchard", 1460, 28, 35, 85, -5, 40, [Yield("pecan", 2200000)], true, 260),
		new("Macadamias", "A subtropical macadamia nut orchard crop.", "orchard", 1460, 28, 35, 85, 5, 38, [Yield("macadamia", 1800000)], true, 260),
		new("Carob", "A dry-climate carob tree crop for sweet pods.", "orchard", 1095, 28, 15, 60, -2, 42, [Yield("carob pod", 4000000)], true, 260),
		new("Guavas", "A tropical guava orchard crop for fragrant fruit.", "orchard", 730, 24, 45, 90, 12, 40, [Yield("guava", 8000000)], true, 220),
		new("Lychees", "A humid subtropical lychee orchard crop for delicate fruit.", "orchard", 1095, 24, 50, 95, 5, 36, [Yield("lychee", 5000000)], true, 220),
		new("Jackfruit", "A humid tropical jackfruit tree crop for very large starchy fruit.", "orchard", 1095, 30, 60, 95, 18, 40, [Yield("jackfruit", 16000000)], true, 260),
		new("Papayas", "A fast-establishing tropical papaya plantation crop.", "orchard", 365, 24, 55, 90, 15, 40, [Yield("papaya", 12000000)], true, 210),
		new("Kiwifruit", "A temperate climbing-vine orchard crop for fuzzy fruit.", "orchard", 730, 24, 45, 85, -5, 32, [Yield("kiwi fruit", 6000000)], true, 210),
		new("Passionfruit", "A warm-climate climbing-vine orchard crop for aromatic fruit.", "orchard", 450, 24, 45, 90, 8, 36, [Yield("passionfruit", 5000000)], true, 180),
		new("Cinnamon", "A tropical spice-tree plantation crop harvested for aromatic bark.", "orchard", 1095, 28, 60, 95, 18, 38, [Yield("cinnamon bark", 1200000)], true, 365),
		new("Black Pepper", "A tropical climbing pepper vine grown for dried peppercorns.", "orchard", 1095, 28, 60, 95, 18, 40, [Yield("black pepper", 700000)], true, 300),
		new("Cloves", "A humid tropical spice-tree crop for aromatic flower buds.", "orchard", 1460, 30, 65, 100, 18, 38, [Yield("cloves", 800000)], true, 300),
		new("Nutmeg", "A tropical spice-tree crop for nutmeg seed and mace.", "orchard", 1460, 30, 65, 100, 18, 38, [Yield("nutmeg", 900000), Yield("mace", 180000)], true, 300),
		new("Allspice", "A humid tropical spice-tree crop for dried aromatic berries.", "orchard", 1460, 30, 55, 95, 18, 40, [Yield("allspice", 650000)], Perennial: true, HarvestCycle: 300, PlantingGroups: TropicalPerennialPlanting),
		new("Logwood", "A tropical dye-wood plantation crop harvested for dense red heartwood.", "orchard", 1825, 35, 45, 90, 18, 42, [Yield("logwood", 6000000)], Perennial: true, HarvestCycle: 365, PlantingGroups: TropicalPerennialPlanting),
		new("Henna", "A hot dry-climate dye shrub grown for orange-red leaves.", "orchard", 730, 24, 15, 60, 14, 42, [Yield("henna leaf", 900000), Yield("vegetation", 200000)], true, 220),
		new("Kola Nuts", "A humid tropical kola tree crop for stimulant nuts.", "orchard", 1460, 30, 60, 95, 18, 38, [Yield("kola nut", 1200000)], true, 300),
		new("Tamarinds", "A drought-tolerant tropical tamarind tree crop for sour-sweet pods.", "orchard", 1095, 28, 20, 70, 12, 42, [Yield("tamarind", 5000000)], true, 260),
		new("Nopal Cactus", "A dryland cactus bed grown for edible pads, prickly pear fruit, and optional cochineal culture.", "orchard", 730, 28, 10, 55, 8, 45, [Yield("nopal pad", 1200000), Yield("prickly pear fruit", 2200000), Yield("cochineal", 120000)], Perennial: true, HarvestCycle: 180, PlantingGroups: MediterraneanPerennialPlanting),
		new("Agave", "An arid perennial agave plantation for leaves and coarse leaf fibre.", "orchard", 1095, 32, 10, 55, 8, 45, [Yield("agave leaf", 1800000), Yield("agave fibre", 350000)], Perennial: true, HarvestCycle: 365, PlantingGroups: MediterraneanPerennialPlanting),
		new("Henequen", "A tropical henequen plantation grown for strong leaf fibre.", "orchard", 1095, 30, 20, 70, 16, 42, [Yield("henequen fibre", 700000)], Perennial: true, HarvestCycle: 300, PlantingGroups: TropicalPerennialPlanting),
		new("Pineapples", "A tropical bromeliad plantation crop grown for sweet pineapples.", "orchard", 540, 28, 35, 85, 16, 40, [Yield("pineapple", 6500000)], Perennial: true, HarvestCycle: 300, PlantingGroups: TropicalPerennialPlanting),
		new("Vanilla", "A humid tropical orchid-vine plantation crop grown for aromatic vanilla pods.", "orchard", 1095, 30, 65, 100, 18, 38, [Yield("vanilla", 500000)], Perennial: true, HarvestCycle: 300, PlantingGroups: TropicalPerennialPlanting),
		new("Yerba Mate", "A subtropical beverage shrub crop for harvested yerba mate leaves.", "orchard", 1095, 28, 45, 90, 8, 34, [Yield("yerba mate leaf", 1500000)], Perennial: true, HarvestCycle: 220, PlantingGroups: TropicalPerennialPlanting),
		new("Cranberries", "A cool wetland perennial berry crop for bogs and wet raised beds.", "orchard", 730, 20, 65, 100, -8, 28, [Yield("cranberry", 2200000)], Perennial: true, HarvestCycle: 220, PlantingGroups: DormantPerennialPlanting),
		new("Blueberries", "An acid-soil temperate perennial berry crop for shrubs and wet uplands.", "orchard", 730, 20, 45, 90, -10, 30, [Yield("blueberry", 2500000)], Perennial: true, HarvestCycle: 220, PlantingGroups: DormantPerennialPlanting),
		new("Raspberries", "A cool-temperate cane fruit crop for bramble beds.", "orchard", 540, 20, 35, 85, -10, 32, [Yield("raspberry", 2200000)], Perennial: true, HarvestCycle: 180, PlantingGroups: DormantPerennialPlanting),
		new("Blackberries", "A hardy bramble crop for cane fruit hedges and managed berry beds.", "orchard", 540, 20, 30, 85, -10, 34, [Yield("blackberry", 2600000)], Perennial: true, HarvestCycle: 180, PlantingGroups: DormantPerennialPlanting),
		new("Strawberries", "A perennial bed crop grown for low sweet berries.", "orchard", 365, 18, 35, 85, -5, 32, [Yield("strawberry", 2000000)], Perennial: true, HarvestCycle: 150, PlantingGroups: DormantPerennialPlanting),
		new("Oil Palms", "A humid tropical palm plantation crop grown for palm fruit and oil material.", "orchard", 1460, 35, 60, 100, 18, 42, [Yield("palm fruit", 6500000), Yield("palm oil crop", 1200000)], Perennial: true, HarvestCycle: 300, PlantingGroups: TropicalPerennialPlanting),
		new("Shea Trees", "A savanna parkland tree crop grown for shea nuts.", "orchard", 1825, 35, 25, 75, 14, 42, [Yield("shea nut", 1800000)], Perennial: true, HarvestCycle: 300, PlantingGroups: TropicalPerennialPlanting),
		new("Baobabs", "A dry tropical tree crop grown for baobab fruit and edible leaves.", "orchard", 1825, 35, 15, 70, 14, 45, [Yield("baobab fruit", 3500000), Yield("baobab leaf", 700000)], Perennial: true, HarvestCycle: 300, PlantingGroups: TropicalPerennialPlanting),
		new("Raffia Palms", "A wet tropical palm crop grown for raffia fibre, woven cloth, and palm fronds.", "orchard",
			1460, 35, 65, 100, 18, 42,
			[Yield("raffia fibre", 900000), Yield("raffia cloth", 225000), Yield("palm frond", 1800000)],
			Perennial: true, HarvestCycle: 300, PlantingGroups: TropicalPerennialPlanting),
		new("Hops", "A hardy perennial bine crop grown for brewing hop cones.", "orchard", 730, 24, 35, 85, -10, 32, [Yield("hop", 650000)], Perennial: true, HarvestCycle: 180, PlantingGroups: DormantPerennialPlanting),
		new("Artichokes", "A Mediterranean perennial vegetable bed grown for edible flower buds.", "orchard", 365, 20, 25, 75, 0, 35, [Yield("artichoke", 1500000)], Perennial: true, HarvestCycle: 150, PlantingGroups: MediterraneanPerennialPlanting),
		new("Asparagus", "A long-lived cool-season asparagus bed for spring shoots.", "orchard", 730, 18, 30, 80, -10, 30, [Yield("asparagus", 1000000)], Perennial: true, HarvestCycle: 120, PlantingGroups: DormantPerennialPlanting),
		new("Rhubarb", "A hardy perennial kitchen-garden bed grown for tart stalks.", "orchard", 365, 18, 35, 85, -10, 28, [Yield("rhubarb", 1400000)], Perennial: true, HarvestCycle: 150, PlantingGroups: DormantPerennialPlanting),
		new("Currants", "A cool-temperate perennial shrub crop for clusters of tart currants.", "orchard", 730, 20, 35, 85, -10, 30, [Yield("currant", 1500000)], Perennial: true, HarvestCycle: 180, PlantingGroups: DormantPerennialPlanting)
	];

	private static readonly HerdSeed[] Herds =
	[
		new("Cattle Herd", "A cattle herd managed for milk, grazing pressure, and manure.", 1.0, 1.0, 100,
		[
			Yield("milk", 3500, RawMilkTagName),
			Yield("feces", 2500, ManureCommodityTagName)
		]),
		new("Oxen Team", "A team of draft oxen whose routine field value is work and manure.", 1.0, 0.95, 100,
		[
			Yield("feces", 2400, ManureCommodityTagName)
		]),
		new("Buffalo Herd", "A buffalo herd suited to wet pasture, milk collection, and manure recovery.", 1.1, 1.05, 100,
		[
			Yield("milk", 3200, RawMilkTagName),
			Yield("feces", 2700, ManureCommodityTagName)
		]),
		new("Bison Herd", "A hardy bison herd for frontier or steppe ranching pasture.", 1.1, 1.05, 100,
		[
			Yield("feces", 2600, ManureCommodityTagName)
		]),
		new("Sheep Flock", "A sheep flock managed for milk, fleece, and manure.", 0.2, 0.25, 100,
		[
			Yield("milk", 900, RawMilkTagName),
			Yield("wool", 450, RawTextileFibreTagName),
			Yield("feces", 700, ManureCommodityTagName)
		]),
		new("Goat Herd", "A goat herd managed as small browsing stock for milk, fibre, and manure.", 0.2, 0.25, 100,
		[
			Yield("milk", 800, RawMilkTagName),
			Yield("wool", 180, RawTextileFibreTagName),
			Yield("feces", 650, ManureCommodityTagName)
		]),
		new("Pig Herd", "A pig herd managed as rooting omnivorous livestock with manure recovery.", 0.4, 0.45, 100,
		[
			Yield("feces", 1800, ManureCommodityTagName)
		]),
		new("Horse Herd", "A horse herd for equine pasture, mare's milk, and manure.", 0.8, 0.8, 100,
		[
			Yield("milk", 1200, RawMilkTagName),
			Yield("feces", 1600, ManureCommodityTagName)
		]),
		new("Donkey Herd", "A donkey herd for hardy dryland equine pasture, milk, and manure.", 0.55, 0.55, 100,
		[
			Yield("milk", 650, RawMilkTagName),
			Yield("feces", 1100, ManureCommodityTagName)
		]),
		new("Mule Team", "A mule team whose routine pasture output is manure rather than milk or fibre.", 0.65, 0.65, 100,
		[
			Yield("feces", 1300, ManureCommodityTagName)
		]),
		new("Camel Herd", "A dryland camel herd managed for milk, fibre, and manure.", 0.9, 0.8, 100,
		[
			Yield("milk", 1800, RawMilkTagName),
			Yield("wool", 250, RawTextileFibreTagName),
			Yield("feces", 1600, ManureCommodityTagName)
		]),
		new("Llama Herd", "A llama herd for Andean pasture, pack-stock fibre, and manure.", 0.35, 0.35, 100,
		[
			Yield("camelid wool", 220, RawTextileFibreTagName),
			Yield("feces", 700, ManureCommodityTagName)
		]),
		new("Alpaca Herd", "An alpaca herd managed chiefly for fine fibre and manure.", 0.25, 0.25, 100,
		[
			Yield("camelid wool", 260, RawTextileFibreTagName),
			Yield("feces", 520, ManureCommodityTagName)
		]),
		new("Reindeer Herd", "A cold-climate reindeer herd for pastoral milk and manure.", 0.45, 0.45, 100,
		[
			Yield("milk", 700, RawMilkTagName),
			Yield("feces", 900, ManureCommodityTagName)
		]),
		new("Rabbit Warren", "A small livestock rabbit warren for manure and occasional fibre settings.", 0.05, 0.06, 100,
		[
			Yield("wool", 35, RawTextileFibreTagName),
			Yield("feces", 80, ManureCommodityTagName)
		]),
		new("Guinea Pig Run", "A small Andean guinea pig run with modest manure output.", 0.02, 0.03, 100,
		[
			Yield("feces", 35, ManureCommodityTagName)
		]),
		new("Chicken Flock", "A chicken flock managed for eggs and manure.", 0.03, 0.05, 100,
		[
			Yield("egg", 60, EggProductTagName),
			Yield("feces", 45, ManureCommodityTagName)
		]),
		new("Duck Flock", "A duck flock managed for eggs and wet-yard manure.", 0.04, 0.06, 100,
		[
			Yield("egg", 75, EggProductTagName),
			Yield("feces", 55, ManureCommodityTagName)
		]),
		new("Goose Flock", "A goose flock managed for larger eggs and grazing-yard manure.", 0.06, 0.08, 100,
		[
			Yield("egg", 90, EggProductTagName),
			Yield("feces", 70, ManureCommodityTagName)
		]),
		new("Turkey Flock", "A turkey flock managed for eggs and manure.", 0.08, 0.09, 100,
		[
			Yield("egg", 80, EggProductTagName),
			Yield("feces", 75, ManureCommodityTagName)
		]),
		new("Pigeon Loft", "A pigeon loft managed for small eggs and manure-rich droppings.", 0.01, 0.015, 100,
		[
			Yield("egg", 15, EggProductTagName),
			Yield("feces", 25, ManureCommodityTagName)
		]),
		new("Quail Covey", "A quail covey managed for small eggs and light manure.", 0.01, 0.015, 100,
		[
			Yield("egg", 18, EggProductTagName),
			Yield("feces", 18, ManureCommodityTagName)
		]),
		new("Pheasant Pen", "A pheasant pen managed for gamebird eggs and manure.", 0.03, 0.04, 100,
		[
			Yield("egg", 35, EggProductTagName),
			Yield("feces", 30, ManureCommodityTagName)
		]),
		new("Peafowl Flock", "A peafowl flock managed as ornamental fowl with modest eggs and manure.", 0.06, 0.07, 100,
		[
			Yield("egg", 45, EggProductTagName),
			Yield("feces", 45, ManureCommodityTagName)
		]),
		new("Ostrich Flock", "An ostrich flock managed for large eggs and manure on open pasture.", 0.25, 0.3, 100,
		[
			Yield("egg", 450, EggProductTagName),
			Yield("feces", 300, ManureCommodityTagName)
		]),
		new("Emu Flock", "An emu flock managed for large eggs and dryland manure.", 0.2, 0.25, 100,
		[
			Yield("egg", 350, EggProductTagName),
			Yield("feces", 250, ManureCommodityTagName)
		]),
		new("Rhea Flock", "A rhea flock managed for large eggs and South American pasture manure.", 0.18, 0.22, 100,
		[
			Yield("egg", 320, EggProductTagName),
			Yield("feces", 220, ManureCommodityTagName)
		])
	];

	private static readonly WoodlandSeed[] Woodlands =
	[
		new("Hazel Coppice", "A managed hazel coppice cut on a short cycle for rods, stakes, nuts, and firewood.", "coppice", 180, 365, [Yield("hazel", 2500000), Yield("hazelnut", 250000), Yield("firewood", 800000)]),
		new("Willow Coppice", "A damp-ground willow coppice for withies, rods, basketry material, and firewood.", "coppice", 150, 240, [Yield("willow", 2200000), Yield("firewood", 700000)]),
		new("Bamboo Grove", "A fast-growing bamboo grove for canes, poles, shoots, and light structural material.", "coppice", 120, 180, [Yield("bamboo", 3500000), Yield("vegetation", 500000)]),
		new("Oak Timber Stand", "A long-cycle hardwood timber stand managed for heavy oak logs and firewood.", "timber", 730, 3650, [Yield("oak", 9000000), Yield("firewood", 2500000), Yield("oak gall", 100000)]),
		new("Kermes Oak Scrub", "A Mediterranean scrub woodland managed for small oak wood, fuel, galls, and kermes dye insects.", "coppice", 420, 900, [Yield("oak", 2000000), Yield("firewood", 900000), Yield("oak gall", 80000), Yield("kermes grain", 45000)]),
		new("Dye Lichen Grove", "A damp stone-and-tree woodland managed for dye-bearing orchil lichens.", "coppice", 365, 730, [Yield("orchil lichen", 120000), Yield("moss", 200000), Yield("firewood", 400000)]),
		new("Lac Host Grove", "A warm host-tree grove managed for lac dye encrustations and light fuel wood.", "coppice", 365, 730, [Yield("lac dye cake", 70000), Yield("firewood", 450000)]),
		new("Pine Timber Stand", "A softwood timber stand grown for pine logs, poles, and fuel.", "timber", 540, 2555, [Yield("pine", 8500000), Yield("firewood", 2000000)]),
		new("Cedar Timber Stand", "A managed cedar stand for aromatic softwood timber.", "timber", 650, 3285, [Yield("cedar", 7000000), Yield("firewood", 1600000)]),
		new("Pollarded Willow Grove", "A pollarded willow grove cut above browsing height for poles and fuel.", "pollard", 240, 730, [Yield("willow", 3200000), Yield("firewood", 1000000)]),
		new("Fruitwood Pollard Grove", "A pollarded mixed fruitwood grove that yields prunings, light timber, and modest fruitwood.", "pollard", 300, 900, [Yield("applewood", 1500000), Yield("pearwood", 1000000), Yield("firewood", 1000000)]),
		new("Ash Coppice", "A temperate ash coppice cut for tool handles, poles, and fuelwood.", "coppice", 220, 730, [Yield("ash", 2600000), Yield("firewood", 900000)]),
		new("Sweet Chestnut Coppice", "A chestnut coppice for durable poles, nuts, and firewood in temperate or Mediterranean settings.", "coppice", 300, 900, [Yield("chestnut", 2500000), Yield("firewood", 900000)]),
		new("Beech Timber Stand", "A cool temperate beech timber stand for hardwood and firewood.", "timber", 650, 3285, [Yield("beech", 7000000), Yield("firewood", 1800000)]),
		new("Birch Coppice", "A boreal or cool temperate birch coppice for light wood and kindling.", "coppice", 180, 540, [Yield("birch", 2200000), Yield("firewood", 800000)]),
		new("Alder Carr", "A wet alder carr managed for water-tolerant poles and fuel.", "wetland", 240, 730, [Yield("alder", 2400000), Yield("firewood", 900000)]),
		new("Poplar Plantation", "A river-valley poplar plantation for fast light timber and fuel.", "plantation", 300, 1095, [Yield("poplar", 4200000), Yield("firewood", 1200000)]),
		new("Elm Pollard Grove", "A temperate elm pollard grove cut above browsing height for poles and fuel.", "pollard", 300, 900, [Yield("elm", 2600000), Yield("firewood", 900000)]),
		new("Linden Coppice", "A temperate linden coppice for light wood, bast, and fuel.", "coppice", 240, 730, [Yield("linden", 2200000), Yield("firewood", 700000)]),
		new("Maple Sugarbush", "A North American maple grove managed for sap, timber, and firewood.", "extraction", 730, 365, [Yield("maple", 1500000), Yield("maple sap", 450000), Yield("firewood", 600000)]),
		new("Osier Willow Bed", "A wet osier willow bed grown for basketry rods, wicker material, and fuel.", "fibre", 120, 240, [Yield("willow", 1800000), Yield("wicker", 450000), Yield("firewood", 400000)]),
		new("Reedbed Thatch Stand", "A wetland reedbed managed for thatch, matting reeds, and green vegetation.", "thatch", 90, 180, [Yield("reed", 1500000), Yield("vegetation", 350000)]),
		new("Cork Oak Woodland", "A Mediterranean cork oak woodland for cork bark, oak wood, and fuel.", "tannin", 730, 1095, [Yield("oak", 2500000), Yield("cork bark", 500000), Yield("firewood", 800000)]),
		new("Aleppo Pine Resin Stand", "A Mediterranean pine stand managed for resin, light timber, and fuel.", "resin", 540, 1095, [Yield("pine", 3000000), Yield("pine resin", 220000), Yield("firewood", 900000)]),
		new("Holm Oak Dehesa", "A Mediterranean holm oak parkland that yields oak wood, acorns, and fuel.", "silvopasture", 730, 900, [Yield("oak", 2200000), Yield("acorn", 600000), Yield("firewood", 800000)]),
		new("Mastic Scrub", "A Mediterranean mastic scrub managed for aromatic resin and small fuelwood.", "resin", 365, 730, [Yield("mastic resin", 90000), Yield("firewood", 350000)]),
		new("Acacia Gum Grove", "A Sahelian acacia grove tapped for gum arabic with light firewood output.", "gum", 540, 730, [Yield("acacia", 1600000), Yield("gum arabic", 180000), Yield("firewood", 500000)]),
		new("Shea Parkland", "A West African shea parkland managed as agroforestry for nuts and fuel.", "parkland", 730, 365, [Yield("shea nut", 1000000), Yield("firewood", 500000)]),
		new("Baobab Parkland", "An African savanna baobab parkland for fruit, leaves, and modest fuel.", "parkland", 730, 365, [Yield("baobab fruit", 1400000), Yield("baobab leaf", 350000), Yield("firewood", 350000)]),
		new("Raffia Palm Grove", "A wet tropical raffia palm grove for fibre, fronds, and light fuel.", "fibre", 540, 365, [Yield("raffia fibre", 550000), Yield("palm frond", 1300000), Yield("firewood", 350000)]),
		new("Mopane Coppice", "A southern African mopane coppice for durable fuelwood and poles.", "fuelwood", 300, 730, [Yield("mopane", 2200000), Yield("firewood", 1100000)]),
		new("Miombo Fuelwood Stand", "A miombo woodland stand managed mostly for fuelwood and light timber.", "fuelwood", 420, 900, [Yield("miombo wood", 2200000), Yield("firewood", 1200000)]),
		new("Mangrove Coppice", "A saline tropical mangrove coppice for wetland fuelwood and poles.", "wetland", 420, 730, [Yield("mangrove wood", 2200000), Yield("firewood", 900000)]),
		new("Frankincense Grove", "A Horn of Africa or Arabian grove tapped for frankincense resin.", "resin", 730, 365, [Yield("frankincense resin", 120000), Yield("firewood", 300000)]),
		new("Myrrh Scrub", "A dryland myrrh scrub tapped for resin and light brushwood.", "resin", 540, 365, [Yield("myrrh resin", 90000), Yield("firewood", 250000)]),
		new("Teak Plantation", "A tropical teak plantation for high-value timber and secondary fuel.", "timber", 730, 3650, [Yield("teak", 8500000), Yield("firewood", 1600000)]),
		new("Sandalwood Plantation", "A warm sandalwood plantation for aromatic timber and fuel.", "timber", 730, 3285, [Yield("sandalwood", 3500000), Yield("firewood", 800000)]),
		new("Eucalyptus Fuelwood Plantation", "A fast-growing eucalyptus plantation for fuelwood, poles, and rough timber.", "fuelwood", 300, 900, [Yield("eucalyptus", 4500000), Yield("firewood", 1800000)]),
		new("Rubber Tree Plantation", "A tropical rubber tree plantation tapped for latex with secondary timber.", "plantation", 1095, 365, [Yield("rubber", 400000), Yield("latex", 500000), Yield("firewood", 600000)]),
		new("Rattan Cane Brake", "A Southeast Asian rattan brake managed for cane, rattan, and light fuel.", "fibre", 240, 540, [Yield("rattan", 1400000), Yield("cane", 650000), Yield("firewood", 300000)]),
		new("Mulberry Coppice", "An East Asian mulberry coppice for silkworm fodder leaves, wood, and fuel.", "coppice", 240, 365, [Yield("mulberry leaf", 1200000), Yield("mulberry wood", 800000), Yield("firewood", 300000)]),
		new("Agarwood Plantation", "A humid tropical agarwood plantation for aromatic heartwood and light fuel.", "resin", 730, 2555, [Yield("agarwood", 1200000), Yield("firewood", 350000)]),
		new("Mesquite Coppice", "An arid American mesquite coppice for fuelwood and thorny poles.", "fuelwood", 240, 540, [Yield("mesquite", 1800000), Yield("firewood", 900000)]),
		new("Hickory Timber Stand", "A North American hickory timber stand for strong hardwood and fuel.", "timber", 650, 3285, [Yield("hickory", 7000000), Yield("firewood", 1800000)]),
		new("Black Walnut Stand", "A North American black walnut stand for timber, dye hulls, and fuel.", "timber", 730, 3285, [Yield("walnut", 6500000), Yield("walnut hull", 180000), Yield("firewood", 1400000)]),
		new("Brazil Nut Forest Grove", "An Amazonian forest grove managed for Brazil nuts and light fuelwood.", "parkland", 1095, 365, [Yield("brazil nut", 1200000), Yield("firewood", 300000)]),
		new("Moso Bamboo Grove", "An East Asian moso bamboo grove for large poles and edible green growth.", "coppice", 120, 180, [Yield("bamboo", 4500000), Yield("vegetation", 600000)]),
		new("Clumping Bamboo Brake", "A tropical clumping bamboo brake for canes, poles, and green biomass.", "coppice", 120, 180, [Yield("bamboo", 3200000), Yield("vegetation", 500000)]),
		new("Tannin Bark Oakwood", "An oakwood managed for bark, galls, tannin products, and fuel.", "tannin", 540, 900, [Yield("oak", 2200000), Yield("oak bark", 450000), Yield("oak gall", 140000), Yield("firewood", 700000)]),
		new("Charcoal Coppice", "A general coppice stand cut and burned for charcoal and firewood.", "charcoal", 300, 730, [Yield("firewood", 1500000), Yield("charcoal", 600000)]),
		new("Mixed Hedgerow", "A mixed field-boundary hedgerow for shelter, fencing rods, browse, and firewood.", "shelterbelt", 240, 365, [Yield("hazel", 700000), Yield("willow", 500000), Yield("firewood", 350000)])
	];

	private static readonly AgricultureFieldUse[] ApiaryAllowedUses =
	[
		AgricultureFieldUse.Fallow,
		AgricultureFieldUse.Crop,
		AgricultureFieldUse.Orchard,
		AgricultureFieldUse.Pasture,
		AgricultureFieldUse.Woodland
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
		new("Collect Herd Products", "Collect milk, fleece, eggs, manure, or other configured secondary products from an established herd.", AgricultureOperationType.HarvestHerdProducts, AgricultureTargetType.Herd, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 24.0, [(AgricultureScoreType.Pasture, -1)], HerdYieldMultiplier: 1.0, HerdYieldCost: 55),
		new("Plant Woodland", "Plant or establish a managed woodland, coppice, pollard, or timber stand.", AgricultureOperationType.Woodland, AgricultureTargetType.Woodland, AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland, 480.0, [(AgricultureScoreType.Topsoil, -2), (AgricultureScoreType.Weeds, -4), (AgricultureScoreType.Condition, 2)]),
		new("Coppice Woodland", "Cut a coppice stand back on cycle to produce poles, rods, or firewood while preserving the stand.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 160.0, [(AgricultureScoreType.Nutrients, -2), (AgricultureScoreType.Condition, 1)], 0.45, 45),
		new("Thin Woodland", "Thin a woodland stand to produce small timber and improve remaining growth.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 240.0, [(AgricultureScoreType.Pests, -4), (AgricultureScoreType.Condition, 2)], 0.25, 25),
		new("Fell Woodland", "Fell a managed woodland stand and return the field to fallow use.", AgricultureOperationType.Clear, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Fallow, 360.0, [(AgricultureScoreType.Topsoil, -4), (AgricultureScoreType.Weeds, 4), (AgricultureScoreType.Condition, -4)], 1.0, 100),
		new("Clear Land", "Clear brush, stumps, scrub, or unmanaged growth for future field use.", AgricultureOperationType.Clear, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Fallow, 480.0, [(AgricultureScoreType.Weeds, -6), (AgricultureScoreType.Topsoil, -3), (AgricultureScoreType.Condition, -3)], 0.15, 20),
		new("Install Apiary", "Place hives and a stand so bees can establish alongside the field's existing use.", AgricultureOperationType.InstallApiary, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 48.0, [(AgricultureScoreType.Condition, 1)], AllowedUses: ApiaryAllowedUses, ApiaryInstallHiveCount: 2, ApiaryPollinationRadius: 2),
		new("Tend Apiary", "Inspect and tend the hives, calming the colony and reducing pest pressure near the field.", AgricultureOperationType.TendApiary, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 16.0, [(AgricultureScoreType.Pests, -2), (AgricultureScoreType.Condition, 1)], AllowedUses: ApiaryAllowedUses, ApiaryTendHealthDelta: 8, ApiaryTendStoresDelta: 4, ApiaryTendYieldDelta: 8),
		new("Harvest Apiary", "Harvest honeycomb, honey, and wax from a healthy apiary without changing the field's use.", AgricultureOperationType.HarvestApiary, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 24.0, [(AgricultureScoreType.Pests, 1)], AllowedUses: ApiaryAllowedUses, ApiaryYieldMultiplier: 1.0, ApiaryYieldCost: 45, ApiaryOutputs: [Yield("honeycomb", 60000, RawHoneycombTagName), Yield("honey", 30000, PressedHoneyTagName), Yield("beeswax", 6000, RenderedBeeswaxTagName)]),
		new("Remove Apiary", "Remove the hives and stand from the field while leaving the primary field use alone.", AgricultureOperationType.RemoveApiary, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 16.0, [], AllowedUses: ApiaryAllowedUses),
		new("Harrow Field", "Break clods and level a prepared fallow field with lighter soil disturbance than ploughing.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 48.0, [(AgricultureScoreType.Tilth, 4), (AgricultureScoreType.Weeds, -2), (AgricultureScoreType.Moisture, -1)]),
		new("Hoe Rows", "Hand-hoe crop rows to suppress weeds and keep the growing field tidy.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 64.0, [(AgricultureScoreType.Weeds, -5), (AgricultureScoreType.Tilth, 1), (AgricultureScoreType.Condition, 1)]),
		new("Ridge and Furrow", "Shape fallow ground into ridges and furrows to improve drainage and workable tilth.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 120.0, [(AgricultureScoreType.Drainage, 4), (AgricultureScoreType.Tilth, 3), (AgricultureScoreType.Moisture, -1)]),
		new("Break Prairie Sod", "Cut dense pasture sod into rough fallow ground ready for further preparation.", AgricultureOperationType.Clear, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Fallow, 360.0, [(AgricultureScoreType.Tilth, 6), (AgricultureScoreType.Weeds, -4), (AgricultureScoreType.Topsoil, -1), (AgricultureScoreType.Condition, -1)]),
		new("Deep Dig Garden", "Deep-dig a small garden plot to improve tilth, mix soil layers, and tidy condition.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 160.0, [(AgricultureScoreType.Tilth, 6), (AgricultureScoreType.Topsoil, 1), (AgricultureScoreType.Condition, 1)]),
		new("Mulch Field", "Apply organic cover around a growing crop to hold moisture and suppress weeds.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 72.0, [(AgricultureScoreType.Moisture, 4), (AgricultureScoreType.Weeds, -2), (AgricultureScoreType.Topsoil, 1)]),
		new("Earth Up Root Crop", "Draw soil around root-crop rows to improve cover, tilth, and crop condition.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 56.0, [(AgricultureScoreType.Tilth, 2), (AgricultureScoreType.Weeds, -2), (AgricultureScoreType.Condition, 1)]),
		new("Dig Irrigation Channels", "Dig new irrigation channels across fallow land to bring controlled water to the field.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 280.0, [(AgricultureScoreType.Moisture, 6), (AgricultureScoreType.Drainage, 2), (AgricultureScoreType.Salinity, 1), (AgricultureScoreType.Condition, 1)]),
		new("Maintain Irrigation Channels", "Clear silt and weeds from irrigation channels serving an active crop.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 96.0, [(AgricultureScoreType.Moisture, 3), (AgricultureScoreType.Weeds, -1), (AgricultureScoreType.Condition, 1)]),
		new("Flood Paddy", "Flood and manage a paddy-style crop field for water-loving crops.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 72.0, [(AgricultureScoreType.Moisture, 8), (AgricultureScoreType.Weeds, -3), (AgricultureScoreType.Salinity, 1)]),
		new("Build Bunds", "Build low bunds on fallow ground to retain water and shape wet-field management.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 160.0, [(AgricultureScoreType.Moisture, 5), (AgricultureScoreType.Drainage, -2), (AgricultureScoreType.Condition, 1)]),
		new("Build Terraces", "Construct terraces on fallow sloping land to hold topsoil and improve drainage control.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 520.0, [(AgricultureScoreType.Topsoil, 3), (AgricultureScoreType.Drainage, 3), (AgricultureScoreType.Rockiness, -2), (AgricultureScoreType.Condition, 2)]),
		new("Maintain Terraces", "Repair terrace edges and drains around an established orchard or vineyard.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard, 160.0, [(AgricultureScoreType.Topsoil, 1), (AgricultureScoreType.Drainage, 2), (AgricultureScoreType.Condition, 2)]),
		new("Spread Compost", "Spread compost over fallow ground to build fertility and tilth.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 96.0, [(AgricultureScoreType.Nutrients, 6), (AgricultureScoreType.Tilth, 2), (AgricultureScoreType.Weeds, 1)]),
		new("Spread Manure", "Spread manure over fallow ground for a strong but messy fertility boost.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 96.0, [(AgricultureScoreType.Nutrients, 7), (AgricultureScoreType.Weeds, 2), (AgricultureScoreType.Pests, 1)]),
		new("Apply Lime", "Apply lime or similar amendment where a generic condition and fertility lift is appropriate.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 80.0, [(AgricultureScoreType.Condition, 2), (AgricultureScoreType.Nutrients, 1), (AgricultureScoreType.Tilth, 1)]),
		new("Apply Wood Ash", "Apply wood ash as a historical amendment that adds nutrients but slightly raises salt pressure.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 72.0, [(AgricultureScoreType.Nutrients, 3), (AgricultureScoreType.Condition, 1), (AgricultureScoreType.Salinity, 1)]),
		new("Sow Green Manure", "Sow a green-manure cover on fallow land to hold weeds and begin rebuilding soil.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 64.0, [(AgricultureScoreType.Nutrients, 2), (AgricultureScoreType.Weeds, -2), (AgricultureScoreType.Pasture, 2)]),
		new("Incorporate Green Manure", "Turn green manure into fallow soil for a stronger fertility and tilth gain.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 96.0, [(AgricultureScoreType.Nutrients, 5), (AgricultureScoreType.Tilth, 2), (AgricultureScoreType.Weeds, -1)]),
		new("Rest Field", "Leave fallow land resting under light management to recover condition and modest fertility.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 48.0, [(AgricultureScoreType.Condition, 3), (AgricultureScoreType.Weeds, 2), (AgricultureScoreType.Nutrients, 1)]),
		new("Flush Salts", "Move water through salt-affected fallow ground to reduce salinity at the cost of wetness.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 180.0, [(AgricultureScoreType.Salinity, -8), (AgricultureScoreType.Moisture, 3), (AgricultureScoreType.Drainage, -1)]),
		new("Install Tile Drainage", "Install early-industrial tile drains on fallow land for a major drainage improvement.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 640.0, [(AgricultureScoreType.Drainage, 10), (AgricultureScoreType.Moisture, -5), (AgricultureScoreType.Condition, 2)]),
		new("Build Surface Drains", "Cut surface drains and ditches on fallow land to remove excess water.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 240.0, [(AgricultureScoreType.Drainage, 6), (AgricultureScoreType.Moisture, -3), (AgricultureScoreType.Condition, 1)]),
		new("Add Topsoil", "Import and spread topsoil over damaged fallow ground.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 480.0, [(AgricultureScoreType.Topsoil, 6), (AgricultureScoreType.Tilth, 2), (AgricultureScoreType.Rockiness, -1)]),
		new("Stabilise Erosion", "Stabilise eroding pasture with small earthworks, plant cover, and repair labour.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 240.0, [(AgricultureScoreType.Topsoil, 3), (AgricultureScoreType.Condition, 2), (AgricultureScoreType.Weeds, 1)]),
		new("Clear Brush", "Cut back brush and woody regrowth from managed woodland to return it to fallow use.", AgricultureOperationType.Clear, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Fallow, 220.0, [(AgricultureScoreType.Weeds, -8), (AgricultureScoreType.Pasture, 1), (AgricultureScoreType.Condition, -1)], WoodlandYieldMultiplier: 0.1, WoodlandYieldCost: 10),
		new("Remove Stumps", "Dig out stumps and roots from fallow ground after clearing woodland or scrub.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 320.0, [(AgricultureScoreType.Rockiness, -3), (AgricultureScoreType.Tilth, 4), (AgricultureScoreType.Weeds, -3)]),
		new("Controlled Burn", "Burn rough pasture or brush under control to reduce weeds and pests while costing topsoil.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 160.0, [(AgricultureScoreType.Weeds, -8), (AgricultureScoreType.Pests, -2), (AgricultureScoreType.Nutrients, 2), (AgricultureScoreType.Topsoil, -2)]),
		new("Control Field Pests", "Perform general pest control on an active crop field.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Crop, AgricultureFieldUse.Crop, 72.0, [(AgricultureScoreType.Pests, -8), (AgricultureScoreType.Condition, 1)]),
		new("Net Orchard", "Net or protect an orchard against birds and small crop thieves.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard, 120.0, [(AgricultureScoreType.Pests, -6), (AgricultureScoreType.Condition, 1)]),
		new("Graft Orchard", "Graft or replace scion wood in an orchard to improve long-term condition.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard, 180.0, [(AgricultureScoreType.Condition, 3), (AgricultureScoreType.Pests, 1)]),
		new("Prune Orchard", "Prune and train an orchard as focused perennial maintenance.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard, 96.0, [(AgricultureScoreType.Condition, 2), (AgricultureScoreType.Weeds, -2), (AgricultureScoreType.Pests, -2)]),
		new("Improve Pasture", "Improve pasture sward and grazing value with general labour and light reseeding.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 120.0, [(AgricultureScoreType.Pasture, 6), (AgricultureScoreType.Weeds, -2), (AgricultureScoreType.Condition, 1)]),
		new("Reseed Pasture", "Reseed poor pasture to rebuild grazing cover at a modest nutrient cost.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 160.0, [(AgricultureScoreType.Pasture, 8), (AgricultureScoreType.Nutrients, -1), (AgricultureScoreType.Weeds, -3)]),
		new("Build Stockyard", "Build a stockyard or handling pen to improve pasture infrastructure.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 300.0, [(AgricultureScoreType.Fence, 8), (AgricultureScoreType.Condition, 1), (AgricultureScoreType.Pasture, -1)]),
		new("Repair Watering Point", "Repair a watering point or trough serving pasture stock.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 120.0, [(AgricultureScoreType.Moisture, 2), (AgricultureScoreType.Pasture, 2), (AgricultureScoreType.Condition, 1)]),
		new("Pollard Trees", "Pollard managed woodland trees to harvest poles while preserving the stand.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 140.0, [(AgricultureScoreType.Condition, 1), (AgricultureScoreType.Pests, -1)], WoodlandYieldMultiplier: 0.35, WoodlandYieldCost: 35),
		new("Tap Resin", "Tap resin or gum trees in managed woodland with only slight stand stress.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 80.0, [(AgricultureScoreType.Condition, -1)], WoodlandYieldMultiplier: 0.2, WoodlandYieldCost: 20),
		new("Strip Tannin Bark", "Strip bark from a managed tannin woodland for bark products at a condition cost.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 120.0, [(AgricultureScoreType.Condition, -2)], WoodlandYieldMultiplier: 0.3, WoodlandYieldCost: 30),
		new("Charcoal Burn", "Burn a managed coppice or fuelwood stand to release charcoal-like woodland products.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 180.0, [(AgricultureScoreType.Nutrients, -1), (AgricultureScoreType.Condition, -1)], WoodlandYieldMultiplier: 0.35, WoodlandYieldCost: 35),
		new("Gather Woodland Products", "Gather non-timber woodland products such as nuts, bark, reeds, leaves, gum, or resin.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, 64.0, [], WoodlandYieldMultiplier: 0.15, WoodlandYieldCost: 15),
		new("Plant Shelterbelt", "Plant a shelterbelt or windbreak as a managed woodland field use.", AgricultureOperationType.Woodland, AgricultureTargetType.Woodland, AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland, 360.0, [(AgricultureScoreType.Moisture, 1), (AgricultureScoreType.Weeds, -2), (AgricultureScoreType.Condition, 2)]),
		new("Lay Hedgerow", "Lay or repair a hedgerow along pasture boundaries for fencing and pest shelter balance.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 200.0, [(AgricultureScoreType.Fence, 5), (AgricultureScoreType.Pests, -1), (AgricultureScoreType.Weeds, 1)]),
		new("Build Raised Beds", "Build raised beds on fallow ground for wetland gardens, market plots, or waru-waru style fields.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Fallow, AgricultureFieldUse.Fallow, 220.0, [(AgricultureScoreType.Drainage, 5), (AgricultureScoreType.Tilth, 4), (AgricultureScoreType.Moisture, -2)]),
		new("Kraal Night Penning", "Use night-penned stock to manure pasture while wearing grazing and fencing slightly.", AgricultureOperationType.Improve, AgricultureTargetType.None, AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture, 96.0, [(AgricultureScoreType.Nutrients, 6), (AgricultureScoreType.Pasture, -2), (AgricultureScoreType.Fence, -1)])
	];

	public static IReadOnlyCollection<string> StockCommodityOutputMaterialsForTesting =>
		Crops.SelectMany(x => x.Outputs)
		     .Concat(Orchards.SelectMany(x => x.Outputs))
		     .Concat(Herds.SelectMany(x => x.SecondaryOutputs))
		     .Concat(Woodlands.SelectMany(x => x.Outputs))
		     .Concat(Operations.SelectMany(x => x.ApiaryOutputs ?? []))
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
		    !context.Tags.Any(x => x.Name == BeeHiveTagName) ||
		    !context.Tags.Any(x => x.Name == HiveStandTagName) ||
		    !context.Tags.Any(x => x.Name == RawHoneycombTagName) ||
		    !context.Tags.Any(x => x.Name == PressedHoneyTagName) ||
		    !context.Tags.Any(x => x.Name == RenderedBeeswaxTagName) ||
		    !context.Tags.Any(x => x.Name == RawMilkTagName) ||
		    !context.Tags.Any(x => x.Name == RawTextileFibreTagName) ||
		    !context.Tags.Any(x => x.Name == EggProductTagName) ||
		    !context.Tags.Any(x => x.Name == ManureCommodityTagName) ||
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
		var pollination = PollinationFor(definition);
		crop.Definition = new XElement("Crop",
			new XAttribute("growthDays", definition.Growth),
			new XAttribute("harvestWindowDays", definition.Window),
			new XAttribute("perennial", definition.Perennial),
			new XAttribute("harvestCycleDays", definition.HarvestCycle <= 0 ? definition.Growth : definition.HarvestCycle),
			new XAttribute("minMoisture", definition.MinMoisture),
			new XAttribute("maxMoisture", definition.MaxMoisture),
			new XAttribute("minTemperature", definition.MinTemp),
			new XAttribute("maxTemperature", definition.MaxTemp),
			new XElement("Pollination",
				new XAttribute("dependency", pollination.Dependency.ToString()),
				new XAttribute("healthBonus", pollination.HealthBonus),
				new XAttribute("yieldBonus", pollination.YieldBonus)),
			new XElement("PlantingWindows",
				PlantingGroupsFor(definition).Select(x => new XElement("Window",
					new XAttribute("type", "group"),
					new XAttribute("value", x)))),
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

	private static void EnsureHerd(FuturemudDatabaseContext context, HerdSeed definition)
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
			new XAttribute("maximumCondition", definition.MaximumCondition),
			new XElement("SecondaryOutputs",
				definition.SecondaryOutputs.Select(x => new XElement("Commodity",
					new XAttribute("material", x.MaterialName),
					new XAttribute("weight", x.BaseWeight),
					string.IsNullOrWhiteSpace(x.TagName) ? null : new XAttribute("tag", x.TagName))))).ToString();
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
		EnsureApiaryInstallRequirements(context, phase, operation);
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

	private static void EnsureApiaryInstallRequirements(FuturemudDatabaseContext context, ProjectPhase phase, OperationSeed operation)
	{
		if (operation.Type != AgricultureOperationType.InstallApiary)
		{
			RemoveMaterialRequirement(context, phase, "Bee Hives");
			RemoveMaterialRequirement(context, phase, "Hive Stand");
			return;
		}

		EnsureTaggedItemRequirement(context, phase, "Bee Hives", "Two reusable bee hives for the apiary installation.",
			BeeHiveTagName, 2);
		EnsureTaggedItemRequirement(context, phase, "Hive Stand", "A stand or rack to keep the installed hives off the ground.",
			HiveStandTagName, 1);
	}

	private static void RemoveMaterialRequirement(FuturemudDatabaseContext context, ProjectPhase phase, string name)
	{
		var existing = context.ProjectMaterialRequirements.FirstOrDefault(x => x.ProjectPhaseId == phase.Id && x.Name == name);
		if (existing != null)
		{
			context.ProjectMaterialRequirements.Remove(existing);
		}
	}

	private static void EnsureTaggedItemRequirement(FuturemudDatabaseContext context, ProjectPhase phase, string name,
		string description, string tagName, int amount)
	{
		var tag = context.Tags.First(x => x.Name == tagName);
		var existing = context.ProjectMaterialRequirements.FirstOrDefault(x => x.ProjectPhaseId == phase.Id && x.Name == name);
		if (existing == null)
		{
			existing = new ProjectMaterialRequirement
			{
				ProjectPhaseId = phase.Id,
				Name = name
			};
			context.ProjectMaterialRequirements.Add(existing);
		}

		existing.Type = "simple";
		existing.Description = description;
		existing.IsMandatoryForProjectCompletion = true;
		existing.Definition = new XElement("Material",
			new XElement("Tag", tag.Id),
			new XElement("Amount", amount),
			new XElement("Quality", (int)ItemQuality.Terrible)).ToString();
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
			new XAttribute("herdYieldMultiplier", definition.HerdYieldMultiplier),
			new XAttribute("herdYieldCost", definition.HerdYieldCost),
			new XElement("AllowedUses",
				new XAttribute("uses", string.Join(",", (definition.AllowedUses ?? [definition.Required]).Select(x => x.ToString())))),
			new XElement("Apiary",
				new XAttribute("installHives", definition.ApiaryInstallHiveCount),
				new XAttribute("pollinationRadius", definition.ApiaryPollinationRadius),
				new XAttribute("tendHealthDelta", definition.ApiaryTendHealthDelta),
				new XAttribute("tendStoresDelta", definition.ApiaryTendStoresDelta),
				new XAttribute("tendYieldDelta", definition.ApiaryTendYieldDelta),
				new XAttribute("yieldMultiplier", definition.ApiaryYieldMultiplier),
				new XAttribute("yieldCost", definition.ApiaryYieldCost),
				new XElement("Outputs",
					(definition.ApiaryOutputs ?? []).Select(x => new XElement("Commodity",
						new XAttribute("material", x.MaterialName),
						new XAttribute("weight", x.BaseWeight),
						string.IsNullOrWhiteSpace(x.TagName) ? null : new XAttribute("tag", x.TagName))))),
			definition.Deltas.Select(x => new XElement("Score",
				new XAttribute("type", x.Score.ToString()),
				new XAttribute("value", x.Delta)))).ToString();
	}
}
