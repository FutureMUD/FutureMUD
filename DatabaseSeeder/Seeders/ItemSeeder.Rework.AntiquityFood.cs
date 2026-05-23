#nullable enable

using MudSharp.Framework;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AntiquityFoodRootTagPath = "Food and Drink / Antiquity Food";
	private const string AntiquityFoodToolsTagPath = "Functions / Tools / Foodmaking Tools";
	private const string AntiquityFoodCommodityTagPath = "Materials / Food Products / Antiquity Food Commodities";
	private const string AntiquityFoodVesselTagPath = "Food and Drink / Vessels";

	private static readonly string[] AntiquityFoodCommodityTagNames =
	[
		"Grain Cleaning Stock",
		"Cleaned Grain Commodity",
		"Flour Commodity",
		"Meal Commodity",
		"Bran Commodity",
		"Pulse Meal Commodity",
		"Vegetable Prep Commodity",
		"Fruit Must Commodity",
		"Oilseed Mash Commodity",
		"Oilseed Cake Commodity",
		"Brined Fruit Commodity",
		"Wort Commodity",
		"Raw Meat Commodity",
		"Prepared Meat Commodity",
		"Salted Meat Commodity",
		"Dried Meat Commodity",
		"Smoked Meat Commodity",
		"Rendered Fat Commodity",
		"Bone Stock Commodity",
		"Broth Base Commodity",
		"Rotten Food Commodity",
		"Fermenting Food Commodity",
		"Finished Beverage Stock"
	];

	private static readonly string[] AntiquityFoodMaterialTagNames =
	[
		"Grain Crop",
		"Pulse Crop",
		"Fruit Must Crop",
		"Ready Fruit Crop",
		"Fruit Brining Crop",
		"Vegetable Prep Crop",
		"Oilseed Crop"
	];

	private static readonly AntiquityFoodCultureSpec[] AntiquityFoodCultures =
	[
		new("Hellenic", "Hellenic Foodways", "barley", "chickpea", "fig", "red wine"),
		new("Egyptian", "Egyptian Foodways", "emmer wheat", "lentil", "date", "barley beer"),
		new("Roman", "Roman Foodways", "spelt wheat", "bean", "grape", "red wine"),
		new("Celtic", "Celtic Foodways", "oat", "pea", "apple", "barley beer"),
		new("Germanic", "Germanic Foodways", "rye", "pea", "apple", "barley beer"),
		new("Kushite", "Kushite Foodways", "pearl millet", "cowpea", "date", "date beer"),
		new("Punic", "Punic Foodways", "wheat", "chickpea", "fig", "white wine"),
		new("Persian", "Persian Foodways", "barley", "lentil", "grape", "white wine"),
		new("Etruscan", "Etruscan Foodways", "spelt wheat", "bean", "grape", "red wine"),
		new("Anatolian", "Anatolian Foodways", "einkorn wheat", "lentil", "fig", "white wine"),
		new("Scythian-Sarmatian", "Scythian-Sarmatian Foodways", "millet", "pea", "date", "mare's milk kumis")
	];

	internal static IReadOnlyList<string> AntiquityFoodCommodityTagsForTesting => AntiquityFoodCommodityTagNames;
	internal static IReadOnlyList<string> AntiquityFoodCultureKnowledgeForTesting =>
		AntiquityFoodCultures.Select(x => x.Knowledge).ToList();

	private void SeedAntiquityFoodAndBeverageItems()
	{
		EnsureAntiquityFoodTags();
		EnsureAntiquityFoodMaterialTags();
		EnsureAntiquityFoodLiquids();
		EnsureAntiquityFoodPreparedComponents();
		SeedAntiquityFoodToolsAndItems();
		EnsureAntiquityFoodSpoilageRules();
	}

	private void EnsureAntiquityFoodTags()
	{
		EnsureAntiquityTagPath(AntiquityFoodRootTagPath);
		EnsureAntiquityTagPath($"{AntiquityFoodRootTagPath} / Prepared Foods");
		EnsureAntiquityTagPath($"{AntiquityFoodRootTagPath} / Prepared Foods / Luxury Prepared Foods");
		EnsureAntiquityTagPath($"{AntiquityFoodRootTagPath} / Fermenting Foods");
		EnsureAntiquityTagPath(AntiquityFoodToolsTagPath);
		EnsureAntiquityTagPath($"{AntiquityFoodToolsTagPath} / Grain Processing Tools");
		EnsureAntiquityTagPath($"{AntiquityFoodToolsTagPath} / Pressing Tools");
		EnsureAntiquityTagPath($"{AntiquityFoodToolsTagPath} / Fermentation Tools");
		EnsureAntiquityTagPath(AntiquityFoodVesselTagPath);
		EnsureAntiquityTagPath($"{AntiquityFoodVesselTagPath} / Fermenting Vessel");
		EnsureAntiquityTagPath($"{AntiquityFoodVesselTagPath} / Beverage Serving Vessel");
		EnsureAntiquityTagPath(AntiquityFoodCommodityTagPath);

		foreach (var tag in AntiquityFoodCommodityTagNames)
		{
			EnsureAntiquityTagPath($"{AntiquityFoodCommodityTagPath} / {tag}");
		}

		foreach (var tag in new[]
		         {
			         "Threshing Flail", "Hand Quern", "Fruit Press", "Oil Press", "Fermentation Amphora",
			         "Smoking Rack", "Salting Trough"
		         })
		{
			EnsureAntiquityTagPath($"{AntiquityFoodToolsTagPath} / {tag}");
		}
	}

	private void EnsureAntiquityFoodMaterialTags()
	{
		foreach (var tag in AntiquityFoodMaterialTagNames)
		{
			EnsureAntiquityTagPath($"Materials / Natural Materials / Food / {tag}");
		}

		foreach (var material in new[]
		         {
			         "wheat", "barley", "emmer wheat", "einkorn wheat", "spelt wheat", "naked barley", "rye", "oat",
			         "millet", "finger millet", "pearl millet", "foxtail millet", "proso millet"
		         })
		{
			EnsureMaterialHasTag(material, "Grain Crop");
		}

		foreach (var material in new[] { "bean", "pea", "lentil", "chickpea", "grass pea", "cowpea", "mung bean", "pigeon pea" })
		{
			EnsureMaterialHasTag(material, "Pulse Crop");
		}

		foreach (var material in new[] { "grape", "fig", "date", "apple", "pear", "peach", "pomegranate", "grapefruit" })
		{
			EnsureMaterialHasTag(material, "Fruit Must Crop");
		}

		foreach (var material in new[] { "grape", "fig", "date", "apple", "pear", "peach", "pomegranate", "grapefruit" })
		{
			EnsureMaterialHasTag(material, "Ready Fruit Crop");
		}

		foreach (var material in new[] { "olive crop", "olive" })
		{
			EnsureMaterialHasTag(material, "Fruit Brining Crop");
		}

		foreach (var material in new[] { "onion", "garlic", "cucumber", "pumpkin", "bottle gourd" })
		{
			EnsureMaterialHasTag(material, "Vegetable Prep Crop");
		}

		foreach (var material in new[]
		         {
			         "olive crop", "sesame", "flax", "peanut crop", "marshelder seed", "sunflower", "canola", "chia",
			         "safflower", "niger seed", "mustard seed"
		         })
		{
			EnsureMaterialHasTag(material, "Oilseed Crop");
		}
	}

	private void EnsureAntiquityFoodLiquids()
	{
		EnsureAntiquityLiquid("barley beer", "a cloudy golden beer", "a cloudy golden beer with a grainy head",
			"It tastes mildly sour and grainy, with a small alcoholic bite.", "It smells of malted barley and yeast",
			"yellow", 0.035, 0.8, "Beverage", "Alcoholic");
		EnsureAntiquityLiquid("date beer", "a cloudy amber date beer", "a cloudy amber beer sweetened with dates",
			"It tastes sweet, yeasty and faintly sour.", "It smells of dates, malt and yeast", "yellow", 0.03, 0.75,
			"Beverage", "Alcoholic");
		EnsureAntiquityLiquid("mare's milk kumis", "a cloudy white fermented milk", "a cloudy white fermented milk drink",
			"It tastes tart, creamy and lightly alcoholic.", "It smells sour and milky", "white", 0.025, 0.65,
			"Beverage", "Alcoholic");
		EnsureAntiquityLiquid("meat broth", "a cloudy brown broth", "a cloudy brown broth with fat beads on the surface",
			"It tastes savoury, salty and meaty.", "It smells of boiled bones and herbs", "yellow", 0.0, 0.55, "Beverage");
		EnsureAntiquityLiquid("garum sauce", "a dark salty fish sauce", "a dark, pungent and salty fish sauce",
			"It tastes intensely salty and savoury.", "It smells pungently of fermented fish", "yellow", 0.0, 0.05, "Beverage");
		EnsureAntiquityLiquid("spiced wine", "a dark spiced wine", "a dark wine steeped with honey and costly spices",
			"It tastes sweet, warming and strongly spiced.", "It smells of wine, honey and imported spice", "red",
			0.09, 0.8, "Beverage", "Alcoholic", "Luxury Food");
		EnsureAntiquityLiquid("spiced beer", "a cloudy spiced beer", "a cloudy beer sweetened with honey and spice",
			"It tastes grainy, sweet and warmly spiced.", "It smells of malt, honey and coriander", "yellow", 0.04,
			0.75, "Beverage", "Alcoholic", "Luxury Food");
		EnsureAntiquityLiquid("spiced kumis", "a cloudy spiced kumis", "a cloudy fermented milk drink scented with spice",
			"It tastes tart, creamy, sweet and faintly spiced.", "It smells sour, milky and lightly spiced", "white",
			0.025, 0.65, "Beverage", "Alcoholic", "Luxury Food");
	}

	private void EnsureAntiquityFoodPreparedComponents()
	{
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_Bread", "Antiquity bread and flatbread",
			FoodDefinition("Item", 3.0, 0.05, -0.1, 0.0, 6, 1.0, 0.75, 0.2, 0.15, 3, 7,
				"It tastes of toasted grain and a little smoke.",
				"$sdesc",
				"$sdesc\nIt has been prepared from $ingredients.",
				("grain", "ground grain", "ground grain")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_Porridge", "Antiquity porridges and pottages",
			FoodDefinition("Item", 4.0, 0.25, 0.1, 0.0, 8, 1.0, 0.7, 0.2, 0.4, 2, 5,
				"It tastes soft, warm and filling.",
				"$sdesc",
				"$sdesc\nIt is a soft prepared dish made from $ingredients.",
				("grain", "boiled grain", "boiled grain")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_Stew", "Antiquity stews and braises",
			FoodDefinition("Item", 5.0, 0.35, 0.15, 0.0, 8, 1.0, 0.65, 0.2, 0.6, 2, 4,
				"It tastes savoury and well-cooked.",
				"$sdesc",
				"$sdesc\nIt combines $ingredients in a moist cooked dish.",
				("ingredient", "cooked ingredients", "cooked ingredients")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_Preserved", "Antiquity preserved foods",
			FoodDefinition("Item", 3.5, 0.05, -0.2, 0.0, 6, 1.0, 0.9, 0.45, 0.1, 14, 60,
				"It tastes salty, dry and concentrated.",
				"$sdesc",
				"$sdesc\nIt is preserved food made from $ingredients.",
				("ingredient", "preserved ingredients", "preserved ingredients")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_Sweet", "Antiquity fruit and nut sweets",
			FoodDefinition("Item", 3.5, 0.1, -0.05, 0.0, 6, 1.0, 0.8, 0.35, 0.2, 5, 14,
				"It tastes sweet, dense and fragrant.",
				"$sdesc",
				"$sdesc\nIt is a sweet prepared food made from $ingredients.",
				("fruit", "fruit and sweetening", "fruit and sweetening")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_Fruit", "Antiquity prepared fresh fruit",
			FoodDefinition("Item", 2.0, 0.35, 0.15, 0.0, 5, 1.0, 0.75, 0.25, 0.3, 2, 5,
				"It tastes fresh, sweet and tart.",
				"$sdesc",
				"$sdesc\nIt is prepared fresh fruit made from $ingredients.",
				("fruit", "prepared fruit", "fresh prepared fruit")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_BrinedFruit", "Antiquity brined fruit",
			FoodDefinition("Item", 1.5, 0.1, -0.25, 0.0, 5, 1.0, 0.9, 0.45, 0.1, 10, 45,
				"It tastes salty, sharp and fruity.",
				"$sdesc",
				"$sdesc\nIt is brined fruit prepared from $ingredients.",
				("fruit", "brined fruit", "salty brined fruit")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_LuxuryStew", "Antiquity luxury stews",
			FoodDefinition("Item", 6.0, 0.35, 0.05, 0.0, 8, 1.1, 0.7, 0.25, 0.6, 4, 8,
				"It tastes rich, savoury and warmly spiced.",
				"$sdesc",
				"$sdesc\nIt is an elaborate cooked dish made from $ingredients.",
				("ingredient", "spiced luxury ingredients", "rich spiced ingredients")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_LuxuryBread", "Antiquity filled luxury breads",
			FoodDefinition("Item", 4.5, 0.1, -0.05, 0.0, 7, 1.1, 0.8, 0.25, 0.25, 5, 12,
				"It tastes of toasted grain, oil, meat and spice.",
				"$sdesc",
				"$sdesc\nIt is a filled bread made from $ingredients.",
				("ingredient", "filled bread ingredients", "filled bread ingredients")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_LuxurySweet", "Antiquity luxury sweets",
			FoodDefinition("Item", 4.0, 0.1, -0.05, 0.0, 6, 1.1, 0.85, 0.35, 0.2, 7, 18,
				"It tastes sweet, oily and fragrant with expensive spice.",
				"$sdesc",
				"$sdesc\nIt is a luxury sweet prepared from $ingredients.",
				("ingredient", "honeyed luxury ingredients", "honeyed luxury ingredients")));
		EnsureAntiquityPreparedFoodComponent("PreparedFood_Antiquity_Condiment", "Antiquity condiments and relishes",
			FoodDefinition("Item", 1.0, 0.1, -0.2, 0.0, 5, 1.0, 0.9, 0.5, 0.1, 12, 45,
				"It tastes sharp, salty, savoury and spiced.",
				"$sdesc",
				"$sdesc\nIt is a condiment prepared from $ingredients.",
				("ingredient", "condiment ingredients", "sharp condiment ingredients")));
	}

	private void SeedAntiquityFoodToolsAndItems()
	{
		CreateAntiquityFoodTool("antiquity_food_butchers_knife", "knife", "a broad bronze butcher's knife",
			"A broad bronze knife with a thick spine for breaking down raw cuts of meat.", SizeCategory.Small, 650,
			"bronze", "Functions / Tools / Butcher Tools / Meat Cutting Tools / Butcher's Knife");
		CreateAntiquityFoodTool("antiquity_food_cooking_knife", "knife", "a bronze kitchen knife",
			"A practical bronze kitchen knife for chopping vegetables and prepared food.", SizeCategory.Small, 420,
			"bronze", "Functions / Tools / Cooking / Cooking Utensils / Cooking Knife");
		CreateAntiquityFoodTool("antiquity_food_threshing_flail", "flail", "a jointed wooden threshing flail",
			"A jointed wooden flail used to beat grain heads loose from straw.", SizeCategory.Normal, 1600, "oak",
			"Functions / Tools / Foodmaking Tools / Threshing Flail");
		CreateAntiquityFoodTool("antiquity_food_winnowing_basket", "basket", "a broad woven winnowing basket",
			"A broad, shallow basket for tossing grain and chaff into a breeze.", SizeCategory.Normal, 900, "willow",
			"Functions / Tools / Agricultural Tools / Winnowing Basket");
		CreateAntiquityFoodTool("antiquity_food_pitchfork", "fork", "a long wooden pitchfork",
			"A long-handled wooden fork used to turn straw, fodder and compost heaps.", SizeCategory.Normal, 2200,
			"oak", "Functions / Tools / Agricultural Tools / Pitchfork");
		CreateAntiquityFoodTool("antiquity_food_quern", "quern", "a heavy rotary hand quern",
			"A paired stone quern for grinding cleaned grain into flour or meal.", SizeCategory.Large, 18000, "basalt",
			"Functions / Tools / Foodmaking Tools / Hand Quern");
		CreateAntiquityFoodTool("antiquity_food_mortar", "mortar", "a stone mortar with a worn pestle",
			"A sturdy stone mortar and pestle for crushing pulses, herbs and spices.", SizeCategory.Normal, 8000, "stone",
			"Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle");
		CreateAntiquityFoodTool("antiquity_food_grain_sieve", "sieve", "a fine woven grain sieve",
			"A fine woven sieve for separating flour from bran and coarse meal.", SizeCategory.Normal, 550, "willow",
			"Functions / Tools / Milling Tools / Grain Sieve");
		CreateAntiquityFoodTool("antiquity_food_fruit_press", "press", "a wooden screw fruit press",
			"A wooden press that squeezes must or oil from fruit, seed or mash.", SizeCategory.VeryLarge, 45000, "oak",
			"Functions / Tools / Foodmaking Tools / Fruit Press");
		CreateAntiquityFoodTool("antiquity_food_oil_press", "press", "a weighted beam oil press",
			"A stout beam press for squeezing oil from crushed olives or seeds.", SizeCategory.VeryLarge, 65000, "oak",
			"Functions / Tools / Foodmaking Tools / Oil Press");
		CreateAntiquityFoodTool("antiquity_food_mash_tun", "tun", "a wooden mash tun with a slotted false bottom",
			"A large wooden tun used for steeping malted grain and drawing off wort.", SizeCategory.VeryLarge, 42000, "oak",
			"Functions / Tools / Brewing Tools / Mash Tun");
		CreateAntiquityFoodTool("antiquity_food_drying_rack", "rack", "a slatted food drying rack",
			"A slatted rack for drying meat, fruit, fish or herbs in the air.", SizeCategory.Large, 12000, "oak",
			"Functions / Tools / Cooking / Cooking Utensils / Drying Rack");
		CreateAntiquityFoodTool("antiquity_food_smoking_rack", "rack", "a soot-darkened smoking rack",
			"A darkened rack for hanging food over smoke.", SizeCategory.Large, 15000, "oak",
			"Functions / Tools / Foodmaking Tools / Smoking Rack");
		CreateAntiquityFoodTool("antiquity_food_salting_trough", "trough", "a shallow wooden salting trough",
			"A shallow trough for packing meat or fish in salt.", SizeCategory.Large, 11000, "oak",
			"Functions / Tools / Foodmaking Tools / Salting Trough");

		CreateAntiquityFoodVessel("antiquity_food_serving_amphora", "amphora", "a sealed clay serving amphora",
			"A sealed clay amphora suitable for storing finished drinks and sauces.", "fired clay",
			"LContainer_Amphora_Congius", $"{AntiquityFoodVesselTagPath} / Beverage Serving Vessel");
		CreateAntiquityFoodVessel("antiquity_food_fermenting_amphora", "amphora", "a pitch-lined fermenting amphora",
			"A wide-bellied, pitch-lined amphora suitable for controlled fermentation.", "fired clay",
			"LContainer_Amphora_Congius", $"{AntiquityFoodVesselTagPath} / Fermenting Vessel",
			"Functions / Tools / Foodmaking Tools / Fermentation Amphora");

		CreateAntiquityPreparedFoodItem("prepared_fruit", "fruit",
			"a bowl of prepared fruit",
			"Fresh fruit has been cut, seeded or otherwise made ready to eat.", "fruit",
			"PreparedFood_Antiquity_Fruit");
		CreateAntiquityPreparedFoodItem("brined_fruit", "fruit",
			"a bowl of brined fruit",
			"Bitter fruit has been cured in brine until it is ready to eat.", "olive",
			"PreparedFood_Antiquity_BrinedFruit");

		CreateAntiquityFermentingFoodVessel("antiquity_food_fermenting_beer_amphora",
			"antiquity_food_finished_beer_amphora", "a sealed beer-fermenting amphora",
			"A sealed amphora with a fermenting grain mash inside it.",
			"an amphora of finished barley beer", "A sealed amphora marked as containing finished barley beer.",
			"$0 finishes fermenting into $1.", TimeSpan.FromDays(3));
		CreateAntiquityFermentingFoodVessel("antiquity_food_fermenting_date_beer_amphora",
			"antiquity_food_finished_date_beer_amphora", "a sealed date-beer-fermenting amphora",
			"A sealed amphora with date-sweetened grain wort fermenting inside it.",
			"an amphora of finished date beer", "A sealed amphora marked as containing finished date beer.",
			"$0 finishes fermenting into $1.", TimeSpan.FromDays(3));
		CreateAntiquityFermentingFoodVessel("antiquity_food_fermenting_red_wine_amphora",
			"antiquity_food_finished_red_wine_amphora", "a sealed red-wine-fermenting amphora",
			"A sealed amphora with pressed red fruit must fermenting inside it.",
			"an amphora of finished red wine", "A sealed amphora marked as containing finished red wine.",
			"$0 finishes fermenting into $1.", TimeSpan.FromDays(14));
		CreateAntiquityFermentingFoodVessel("antiquity_food_fermenting_white_wine_amphora",
			"antiquity_food_finished_white_wine_amphora", "a sealed white-wine-fermenting amphora",
			"A sealed amphora with pale fruit must fermenting inside it.",
			"an amphora of finished white wine", "A sealed amphora marked as containing finished white wine.",
			"$0 finishes fermenting into $1.", TimeSpan.FromDays(14));
		CreateAntiquityFermentingFoodVessel("antiquity_food_fermenting_kumis_amphora",
			"antiquity_food_finished_kumis_amphora", "a sealed kumis-fermenting amphora",
			"A sealed amphora with milk fermenting into tart kumis inside it.",
			"an amphora of finished kumis", "A sealed amphora marked as containing finished kumis.",
			"$0 finishes fermenting into $1.", TimeSpan.FromDays(2));
		CreateAntiquityFermentingFoodVessel("antiquity_food_fermenting_garum_amphora",
			"antiquity_food_finished_garum_amphora", "a sealed fish-sauce-fermenting amphora",
			"A sealed amphora packed with salted fish fermenting into sauce.",
			"an amphora of finished garum sauce", "A sealed amphora marked as containing finished garum sauce.",
			"$0 finishes fermenting into $1.", TimeSpan.FromDays(10));
		CreateAntiquityFermentingFoodVessel("antiquity_food_aging_spiced_wine_amphora",
			"antiquity_food_finished_spiced_wine_amphora", "a sealed spiced-wine-aging amphora",
			"A sealed amphora where wine, honey and spice are settling together.",
			"an amphora of aged spiced wine", "A sealed amphora marked as containing aged spiced wine.",
			"$0 finishes steeping into $1.", TimeSpan.FromDays(2));
		CreateAntiquityFermentingFoodVessel("antiquity_food_aging_spiced_beer_amphora",
			"antiquity_food_finished_spiced_beer_amphora", "a sealed spiced-beer-aging amphora",
			"A sealed amphora where beer, honey and spice are settling together.",
			"an amphora of aged spiced beer", "A sealed amphora marked as containing aged spiced beer.",
			"$0 finishes steeping into $1.", TimeSpan.FromDays(2));
		CreateAntiquityFermentingFoodVessel("antiquity_food_aging_spiced_kumis_amphora",
			"antiquity_food_finished_spiced_kumis_amphora", "a sealed spiced-kumis-aging amphora",
			"A sealed amphora where kumis, honey and spice are settling together.",
			"an amphora of aged spiced kumis", "A sealed amphora marked as containing aged spiced kumis.",
			"$0 finishes steeping into $1.", TimeSpan.FromDays(2));

		foreach (var culture in AntiquityFoodCultures)
		{
			CreateAntiquityPreparedFoodItem($"{culture.Key}_flatbread", "bread",
				$"a {culture.Display.ToLowerInvariant()} grain flatbread",
				"A thin, lightly charred flatbread made for a common meal.", "bread",
				"PreparedFood_Antiquity_Bread");
			CreateAntiquityPreparedFoodItem($"{culture.Key}_porridge", "porridge",
				$"a bowl of {culture.Display.ToLowerInvariant()} grain porridge",
				"A warm bowl of grain porridge prepared with water or broth.", "barley",
				"PreparedFood_Antiquity_Porridge");
			CreateAntiquityPreparedFoodItem($"{culture.Key}_pulse_stew", "stew",
				$"a {culture.Display.ToLowerInvariant()} pulse and herb stew",
				"A hearty pulse stew thickened with grain and aromatics.", culture.PulseMaterial,
				"PreparedFood_Antiquity_Stew");
			CreateAntiquityPreparedFoodItem($"{culture.Key}_meat_dish", "dish",
				$"a {culture.Display.ToLowerInvariant()} meat and grain dish",
				"A filling dish of meat cooked with grain or meal.", "meat",
				"PreparedFood_Antiquity_Stew");
			CreateAntiquityPreparedFoodItem($"{culture.Key}_preserved_meat", "ration",
				$"a {culture.Display.ToLowerInvariant()} preserved meat ration",
				"A compact ration of preserved meat and grain for travel or storage.", "meat",
				"PreparedFood_Antiquity_Preserved");
			CreateAntiquityPreparedFoodItem($"{culture.Key}_sweet", "sweet",
				$"a {culture.Display.ToLowerInvariant()} fruit and nut sweet",
				"A dense sweet made from fruit, grain and nuts.", culture.SweetMaterial,
				"PreparedFood_Antiquity_Sweet");
			CreateAntiquityPreparedFoodItem($"{culture.Key}_fruit_platter", "fruit",
				$"a {culture.Display.ToLowerInvariant()} fresh fruit platter",
				"Fresh fruit has been cut, seeded and arranged for an immediate table serving.", culture.SweetMaterial,
				"PreparedFood_Antiquity_Fruit");
			CreateAntiquityPreparedFoodItem($"{culture.Key}_oilseed_cake", "cake",
				$"a {culture.Display.ToLowerInvariant()} oilseed cake",
				"A compact cake of grain flour and pressed oilseed meal, browned for a common table.", "bread",
				"PreparedFood_Antiquity_Bread");
			CreateAntiquityPreparedFoodItem($"{culture.Key}_spiced_meat_stew", "stew",
				$"a {culture.Display.ToLowerInvariant()} spiced meat stew",
				"A rich stew of prepared meat, pulses, vegetables, broth and costly spices.", "meat",
				"PreparedFood_Antiquity_LuxuryStew", true);
			CreateAntiquityPreparedFoodItem($"{culture.Key}_honeyed_pastry", "pastry",
				$"a {culture.Display.ToLowerInvariant()} honeyed pastry",
				"A delicate grain pastry enriched with honey, oil and precious spice.", "honey",
				"PreparedFood_Antiquity_LuxurySweet", true);
			CreateAntiquityPreparedFoodItem($"{culture.Key}_fish_sauce_relish", "relish",
				$"a {culture.Display.ToLowerInvariant()} fish sauce relish",
				"A sharp relish of fermented fish sauce, brined fruit, vegetables and spice.", "salt",
				"PreparedFood_Antiquity_Condiment", true);
			CreateAntiquityPreparedFoodItem($"{culture.Key}_stuffed_flatbread", "bread",
				$"a {culture.Display.ToLowerInvariant()} stuffed flatbread",
				"A filled flatbread stuffed with meat, fruit, oil and warm spice.", "bread",
				"PreparedFood_Antiquity_LuxuryBread", true);
		}
	}

	private GameItemProto? CreateAntiquityFoodTool(string stableReference, string noun, string sdesc, string fdesc,
		SizeCategory size, double weight, string material, string toolTag)
	{
		return CreateItem(stableReference, noun, sdesc, null, fdesc, size, ItemQuality.Standard, weight, 6M, false,
			false, material, [toolTag, "Market / Professional Tools / Standard Tools"],
			["Holdable", "Destroyable_Misc"], null, null, null, null);
	}

	private GameItemProto? CreateAntiquityFoodVessel(string stableReference, string noun, string sdesc, string fdesc,
		string material, string liquidComponent, params string[] vesselTags)
	{
		return CreateItem(stableReference, noun, sdesc, null, fdesc, SizeCategory.Large, ItemQuality.Standard, 5000, 10M,
			false, false, material, vesselTags, ["Holdable", "Destroyable_Misc", liquidComponent], null, null, null, null);
	}

	private GameItemProto? CreateAntiquityFermentingFoodVessel(string activeStableReference,
		string finishedStableReference, string activeSdesc, string activeFdesc, string finishedSdesc, string finishedFdesc,
		string morphEmote, TimeSpan morphTimer)
	{
		var finished = CreateAntiquityFoodVessel(finishedStableReference, "amphora", finishedSdesc, finishedFdesc,
			"fired clay", "LContainer_Amphora_Congius", $"{AntiquityFoodVesselTagPath} / Beverage Serving Vessel");
		return CreateItem(activeStableReference, "amphora", activeSdesc, null, activeFdesc, SizeCategory.Large,
			ItemQuality.Standard, 5200, 12M, false, false, "fired clay",
			[$"{AntiquityFoodRootTagPath} / Fermenting Foods", $"{AntiquityFoodVesselTagPath} / Fermenting Vessel"],
			["Holdable", "Destroyable_Misc", "LContainer_Amphora_Congius"],
			finished is null ? null : finishedStableReference, morphEmote, morphTimer, null);
	}

	private GameItemProto? CreateAntiquityPreparedFoodItem(string stableSuffix, string noun, string sdesc, string fdesc,
		string material, string preparedComponent, bool luxury = false)
	{
		var stableReference = $"antiquity_food_{stableSuffix}";
		string[] tags = luxury
			? [$"{AntiquityFoodRootTagPath} / Prepared Foods", $"{AntiquityFoodRootTagPath} / Prepared Foods / Luxury Prepared Foods"]
			: [$"{AntiquityFoodRootTagPath} / Prepared Foods"];
		return CreateItem(stableReference, noun, sdesc, null, fdesc, SizeCategory.Small, ItemQuality.Standard, 450, 4M,
			false, false, material,
			tags,
			["Holdable", "Destroyable_Misc", preparedComponent], null, null, null, null);
	}

	private void EnsureAntiquityFoodSpoilageRules()
	{
		var meatTag = LookupTag("Meat");
		var rawMeatTag = LookupTag("Raw Meat Commodity");
		var preparedMeatTag = LookupTag("Prepared Meat Commodity");
		var saltedTag = LookupTag("Salted Meat Commodity");
		var driedTag = LookupTag("Dried Meat Commodity");
		var smokedTag = LookupTag("Smoked Meat Commodity");
		var brothTag = LookupTag("Broth Base Commodity");
		var rottenTag = LookupTag("Rotten Food Commodity");
		var meat = LookupMaterial("meat");

		EnsureCommoditySpoilageRule("Raw meat commodity spoilage", "Raw meat commodities spoil quickly into rotten food.",
			null, meatTag, rawMeatTag, meat, rottenTag, TimeSpan.FromDays(2),
			"$0 has spoiled into rotten meat.");
		EnsureCommoditySpoilageRule("Prepared meat commodity spoilage", "Cooked or prepared meat commodities spoil after a few days.",
			null, meatTag, preparedMeatTag, meat, rottenTag, TimeSpan.FromDays(4),
			"$0 has spoiled into rotten meat.");
		EnsureCommoditySpoilageRule("Salted meat commodity spoilage", "Salted meats spoil more slowly than raw meat.",
			null, meatTag, saltedTag, meat, rottenTag, TimeSpan.FromDays(45),
			"$0 has gone rancid despite the salt.");
		EnsureCommoditySpoilageRule("Dried meat commodity spoilage", "Dried meats are durable but eventually spoil.",
			null, meatTag, driedTag, meat, rottenTag, TimeSpan.FromDays(60),
			"$0 has gone rancid and foul.");
		EnsureCommoditySpoilageRule("Smoked meat commodity spoilage", "Smoked meats spoil more slowly than ordinary prepared meat.",
			null, meatTag, smokedTag, meat, rottenTag, TimeSpan.FromDays(35),
			"$0 has gone sour and smoky-foul.");
		EnsureCommoditySpoilageRule("Broth base commodity spoilage", "Broth and stock bases sour quickly unless preserved.",
			null, meatTag, brothTag, meat, rottenTag, TimeSpan.FromDays(2),
			"$0 has soured and spoiled.");
	}

	private Tag EnsureAntiquityTagPath(string path)
	{
		if (_tagsByFullPath.TryGetValue(path, out var existing))
		{
			return existing;
		}

		var parts = path.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		Tag? parent = null;
		var fullPath = string.Empty;
		foreach (var part in parts)
		{
			fullPath = string.IsNullOrWhiteSpace(fullPath) ? part : $"{fullPath} / {part}";
			if (_tagsByFullPath.TryGetValue(fullPath, out existing))
			{
				parent = existing;
				continue;
			}

			var parentId = parent?.Id;
			existing = _context!.Tags.Local
			                    .FirstOrDefault(x => x.Name.Equals(part, StringComparison.OrdinalIgnoreCase) &&
			                                         x.ParentId == parentId) ??
			           _context.Tags
			                   .AsEnumerable()
			                   .FirstOrDefault(x => x.Name.Equals(part, StringComparison.OrdinalIgnoreCase) &&
			                                        x.ParentId == parentId);
			if (existing is null)
			{
				existing = new Tag
				{
					Id = NextTagId(),
					Name = part,
					Parent = parent,
					ParentId = parent?.Id
				};
				_context.Tags.Add(existing);
			}

			_tagsByFullPath[fullPath] = existing;
			_tags[existing.Name] = existing;
			parent = existing;
		}

		return parent!;
	}

	private long NextTagId()
	{
		var existing = _context!.Tags.Any() ? _context.Tags.Max(x => x.Id) : 0L;
		var local = _context.Tags.Local.Any() ? _context.Tags.Local.Max(x => x.Id) : 0L;
		return Math.Max(existing, local) + 1L;
	}

	private long NextComponentId()
	{
		var existing = _context!.GameItemComponentProtos.Any() ? _context.GameItemComponentProtos.Max(x => x.Id) : 0L;
		var local = _context.GameItemComponentProtos.Local.Any() ? _context.GameItemComponentProtos.Local.Max(x => x.Id) : 0L;
		return Math.Max(existing, local) + 1L;
	}

	private long NextLiquidId()
	{
		var existing = _context!.Liquids.Any() ? _context.Liquids.Max(x => x.Id) : 0L;
		var local = _context.Liquids.Local.Any() ? _context.Liquids.Local.Max(x => x.Id) : 0L;
		return Math.Max(existing, local) + 1L;
	}

	private void EnsureMaterialHasTag(string materialName, string tagName)
	{
		if (!_materials.TryGetValue(materialName, out var material) ||
		    !_tags.TryGetValue(tagName, out var tag))
		{
			return;
		}

		if (_context!.MaterialsTags.Any(x => x.MaterialId == material.Id && x.TagId == tag.Id) ||
		    _context.MaterialsTags.Local.Any(x => x.MaterialId == material.Id && x.TagId == tag.Id))
		{
			return;
		}

		_context.MaterialsTags.Add(new MaterialsTags
		{
			MaterialId = material.Id,
			Material = material,
			TagId = tag.Id,
			Tag = tag
		});
	}

	private GameItemComponentProto EnsureAntiquityPreparedFoodComponent(string name, string description, string definition)
	{
		if (_components.TryGetValue(name, out var component))
		{
			return component;
		}

		component = new GameItemComponentProto
		{
			Id = NextComponentId(),
			Name = name,
			Description = description,
			Type = "PreparedFood",
			RevisionNumber = 0,
			Definition = definition,
			EditableItem = NewAntiquityEditableItem()
		};
		_context!.GameItemComponentProtos.Add(component);
		_components[name] = component;
		return component;
	}

	private string FoodDefinition(string scope, double satiation, double water, double thirst, double alcohol,
		double bites, double qualityScale, double staleMultiplier, double spoiledMultiplier, double absorption,
		int staleDays, int spoilDays, string taste, string shortTemplate, string fullTemplate,
		(string Role, string Description, string Taste) ingredient)
	{
		return new XElement("Definition",
			new XAttribute("ServingScope", scope),
			new XAttribute("Satiation", satiation),
			new XAttribute("Water", water),
			new XAttribute("Thirst", thirst),
			new XAttribute("Alcohol", alcohol),
			new XAttribute("Bites", bites),
			new XAttribute("QualityScale", qualityScale),
			new XAttribute("StaleMultiplier", staleMultiplier),
			new XAttribute("SpoiledMultiplier", spoiledMultiplier),
			new XAttribute("LiquidAbsorption", absorption),
			new XAttribute("StaleAfterSeconds", TimeSpan.FromDays(staleDays).TotalSeconds),
			new XAttribute("SpoilAfterSeconds", TimeSpan.FromDays(spoilDays).TotalSeconds),
			new XAttribute("Decorator", 0),
			new XElement("Taste", new XCData(taste)),
			new XElement("Short", new XCData(shortTemplate)),
			new XElement("Full", new XCData(fullTemplate)),
			new XElement("OnEatProg", 0),
			new XElement("OnStaleProg", 0),
			new XElement("Ingredients",
				new XElement("Ingredient",
					new XAttribute("role", ingredient.Role),
					new XAttribute("source", 0),
					new XAttribute("material", 0),
					new XAttribute("liquid", 0),
					new XAttribute("weight", 0),
					new XAttribute("volume", 0),
					new XAttribute("quality", (int)ItemQuality.Standard),
					new XElement("Description", new XCData(ingredient.Description)),
					new XElement("Taste", new XCData(ingredient.Taste))
				)
			),
			new XElement("DrugDoses"),
			new XElement("StaleDrugDoses")
		).ToString();
	}

	private EditableItem NewAntiquityEditableItem()
	{
		return new EditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = 4,
			BuilderAccountId = _dbAccount.Id,
			BuilderDate = _now,
			BuilderComment = "Auto-generated by the system",
			ReviewerAccountId = _dbAccount.Id,
			ReviewerComment = "Auto-generated by the system",
			ReviewerDate = _now
		};
	}

	private void EnsureAntiquityLiquid(string name, string description, string longDescription, string taste,
		string smell, string colour, double alcohol, double water, params string[] tagNames)
	{
		if (_liquids.TryGetValue(name, out var existing))
		{
			foreach (var tagName in tagNames)
			{
				EnsureLiquidHasTag(existing, tagName);
			}

			return;
		}

		var liquid = new Liquid
		{
			Id = NextLiquidId(),
			Name = name,
			Description = description,
			LongDescription = longDescription,
			TasteText = taste,
			VagueTasteText = taste,
			SmellText = smell,
			VagueSmellText = smell,
			TasteIntensity = 500,
			SmellIntensity = 200,
			AlcoholLitresPerLitre = alcohol,
			WaterLitresPerLitre = water,
			FoodSatiatedHoursPerLitre = 0.2,
			DrinkSatiatedHoursPerLitre = water,
			Viscosity = 1.0,
			Density = 1.02,
			Organic = true,
			ThermalConductivity = 0.6,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 4184.0,
			DisplayColour = colour,
			DampDescription = $"It is damp with {name}",
			WetDescription = $"It is wet with {name}",
			DrenchedDescription = $"It is soaking wet with {name}",
			DampShortDescription = "(damp)",
			WetShortDescription = "(wet)",
			DrenchedShortDescription = "(soaked)",
			SolventVolumeRatio = 1.0,
			InjectionConsequence = 0,
			ResidueVolumePercentage = 0.01,
			RelativeEnthalpy = 1.0,
			LeaveResidueInRooms = false,
			SurfaceReactionInfo = string.Empty
		};
		_context!.Liquids.Add(liquid);
		_liquids[name] = liquid;
		foreach (var tagName in tagNames)
		{
			EnsureLiquidHasTag(liquid, tagName);
		}
	}

	private void EnsureLiquidHasTag(Liquid liquid, string tagName)
	{
		if (!_tags.TryGetValue(tagName, out var tag))
		{
			return;
		}

		if (_context!.LiquidsTags.Any(x => x.LiquidId == liquid.Id && x.TagId == tag.Id) ||
		    _context.LiquidsTags.Local.Any(x => x.LiquidId == liquid.Id && x.TagId == tag.Id))
		{
			return;
		}

		_context.LiquidsTags.Add(new LiquidsTags
		{
			LiquidId = liquid.Id,
			Liquid = liquid,
			TagId = tag.Id,
			Tag = tag
		});
	}

	private void EnsureCommoditySpoilageRule(string name, string description, Material? material, Tag? materialTag,
		Tag? commodityTag, Material resultMaterial, Tag? resultCommodityTag, TimeSpan spoilTime, string? echo)
	{
		var existing = _context!.CommoditySpoilageRules.FirstOrDefault(x => x.Name == name) ??
		               _context.CommoditySpoilageRules.Local.FirstOrDefault(x => x.Name == name);
		if (existing is null)
		{
			existing = new CommoditySpoilageRule
			{
				Name = name
			};
			_context.CommoditySpoilageRules.Add(existing);
		}

		existing.Description = description;
		existing.Enabled = true;
		existing.Priority = 0;
		existing.MaterialId = material?.Id;
		existing.MaterialTagId = materialTag?.Id;
		existing.CommodityTagId = commodityTag?.Id;
		existing.ResultMaterialId = resultMaterial.Id;
		existing.ResultCommodityTagId = resultCommodityTag?.Id;
		existing.SecondsUntilSpoiled = (long)spoilTime.TotalSeconds;
		existing.SpoilEcho = echo;
	}

	private sealed record AntiquityFoodCultureSpec(
		string Display,
		string Knowledge,
		string GrainMaterial,
		string PulseMaterial,
		string SweetMaterial,
		string BeverageLiquid)
	{
		public string Key => Display
		                     .ToLowerInvariant()
		                     .Replace("-", "_", StringComparison.Ordinal)
		                     .Replace(" ", "_", StringComparison.Ordinal);
	}
}
