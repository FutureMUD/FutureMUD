#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAntiquityFoodCraftingTests
{
	private static readonly string[] Cultures =
	[
		"Hellenic",
		"Egyptian",
		"Roman",
		"Celtic",
		"Germanic",
		"Kushite",
		"Punic",
		"Persian",
		"Etruscan",
		"Anatolian",
		"Scythian-Sarmatian"
	];

	private static readonly string[] CommodityTags =
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

	[TestMethod]
	public void AntiquityApiaryCrafting_BackfillsApicultureItemsAndProcessing()
	{
		var reworkSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityApiary.cs");
		var craftRootSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityApiary.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		AssertContains(reworkSource, "SeedAntiquityApiaryItems();");
		AssertContains(craftRootSource, "SeedAntiquityApiaryCrafts();");
		foreach (var expected in new[]
		         {
			         "antiquity_wicker_beehive",
			         "antiquity_clay_tube_hive",
			         "antiquity_wooden_hive_stand",
			         "antiquity_bee_smoke_pot",
			         "antiquity_honey_knife",
			         "antiquity_honey_press",
			         "antiquity_honey_strainer"
		         })
		{
			AssertContains(itemSource, expected);
			AssertContains(craftSource, expected);
		}

		foreach (var expected in new[]
		         {
			         "Beekeeping Tools",
			         "Bee Hive",
			         "Hive Stand",
			         "Bee Smoke Pot",
			         "Honey Knife",
			         "Honey Press",
			         "Honey Strainer",
			         "Raw Honeycomb",
			         "Pressed Honey",
			         "Rendered Beeswax"
		         })
		{
			AssertContains(tagSource, expected);
		}

		AssertContains(materialSource, "AddMaterial(\"honeycomb\"");
		AssertContains(materialSource, "\"Animal Product\", \"Apiary Product\"");
		AssertContains(craftSource, "press raw honeycomb");
		AssertContains(craftSource, "Commodity - 2 kilograms of honeycomb; piletag Raw Honeycomb");
		AssertContains(craftSource, "CommodityProduct - 1 kilogram 200 grams of honey commodity; tag Pressed Honey");
		AssertContains(craftSource, "CommodityProduct - 350 grams of beeswax commodity; tag Rendered Beeswax");
	}

	[TestMethod]
	public void AntiquityAgricultureCrafting_BackfillsSeedPastoralAndDerivativeProcessing()
	{
		var craftRootSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityAgriculture.cs");
		var agricultureSource = ReadSource("DatabaseSeeder", "Seeders", "AgricultureSeeder.cs");
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		AssertContains(craftRootSource, "SeedAntiquityAgriculturalProcessingCrafts();");
		AssertContains(craftSource, "select antiquity seed stock");
		AssertContains(craftSource, "CommodityTag - 5 kilograms of a material tagged as Agriculture Seedable; tag Seeded Yield");
		AssertContains(craftSource, "ScrapInput - 25.00% by weight of 5 kilograms of a material tagged as Agriculture Seedable ($i1); tag Seeds");
		AssertContains(craftSource, "strain fresh milk into amphora");
		AssertContains(craftSource, "Commodity - 3 kilograms of milk; piletag Raw Milk");
		AssertContains(craftSource, "filled with 3 litres of milk");
		AssertContains(craftSource, "heap herd manure compost");
		AssertContains(craftSource, "Commodity - 3 kilograms of feces; piletag Manure Commodity");
		AssertContains(craftSource, "CommodityProduct - 3 kilograms 500 grams of compost commodity; tag Manure Commodity");

		foreach (var expected in new[]
		         {
			         "ferment indigo dye cakes",
			         "strip pomegranate rind dye stock",
			         "separate walnut hull dye stock",
			         "dry saffron threads",
			         "CommodityProduct - 500 grams of indigo dye cake commodity; tag Textile Dye Stock",
			         "CommodityProduct - 350 grams of pomegranate rind commodity; tag Textile Dye Stock",
			         "CommodityProduct - 300 grams of walnut hull commodity; tag Textile Dye Stock",
			         "CommodityProduct - 45 grams of saffron commodity; tag Textile Dye Stock"
		         })
		{
			AssertContains(craftSource, expected);
		}

		foreach (var expected in new[]
		         {
			         "Raw Milk",
			         "Raw Wool",
			         "Egg Product",
			         "Manure Commodity"
		         })
		{
			AssertContains(tagSource, expected);
		}

		foreach (var expected in new[]
		         {
			         "Yield(\"milk\", 3500, RawMilkTagName)",
			         "Yield(\"wool\", 450, RawTextileFibreTagName)",
			         "new(\"Horse Herd\"",
			         "Yield(\"egg\", 60, EggProductTagName)",
			         "Yield(\"feces\", 2500, ManureCommodityTagName)",
			         "AgricultureOperationType.HarvestHerdProducts",
			         "new(\"Madder\"",
			         "new(\"Weld\"",
			         "new(\"Alkanet\"",
			         "new(\"Henna\"",
			         "new(\"Coriander\"",
			         "new(\"Cumin\"",
			         "new(\"Saffron Crocus\"",
			         "new(\"Black Pepper\"",
			         "new(\"Kermes Oak Scrub\"",
			         "new(\"Dye Lichen Grove\"",
			         "new(\"Lac Host Grove\""
		         })
		{
			AssertContains(agricultureSource, expected);
		}

		AssertContains(materialSource, "AddMaterial(\"milk\"");
		AssertContains(materialSource, "AddMaterial(\"egg\"");
		AssertContains(materialSource, "AddMaterial(\"saffron crocus\"");
	}

	[TestMethod]
	public void AntiquityFoodCrafting_RunsThroughItemSeederReworkPath()
	{
		var shimPath = SourcePath("DatabaseSeeder", "Seeders", "AntiquityFoodBeverageSeeder.cs");
		var reworkSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityFood.cs");
		var craftRootSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityFood.cs");
		var metadataSource = ReadSource("DatabaseSeeder", "SeederMetadataRegistry.cs");
		var cookingSource = ReadSource("DatabaseSeeder", "Seeders", "CookingSeeder.cs");

		Assert.IsFalse(File.Exists(shimPath), "The food pass should run through ItemSeeder, not a post-butchery shim seeder.");
		Assert.IsFalse(metadataSource.Contains("AntiquityFoodBeverageSeeder", StringComparison.Ordinal),
			"The removed shim should not be registered in seeder metadata.");
		AssertContains(reworkSource, "SeedAntiquityFoodAndBeverageItems();");
		AssertContains(craftRootSource, "SeedAntiquityFoodCrafts();");
		AssertContains(itemSource, "SeedAntiquityFoodAndBeverageItems");
		AssertContains(craftSource, "SeedAntiquityFoodCrafts");
		Assert.IsFalse(cookingSource.Contains("Antiquity Foodways", StringComparison.Ordinal),
			"The antiquity food pass should remain in ItemSeeder partials, not CookingSeeder.");
	}

	[TestMethod]
	public void AntiquityFoodCrafting_UsesSpecialisedAgriculturalAndBeverageSkills()
	{
		var skillSource = ReadSource("DatabaseSeeder", "Seeders", "SkillPackageSeeder.cs");
		var progSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityFood.cs");

		AssertContains(skillSource, "new SkillDetails(\"Threshing\"");
		AssertContains(skillSource, "new SkillDetails(\"Milling\"");
		AssertContains(skillSource, "new SkillDetails(\"Brewing\"");
		AssertContains(progSource, "HasThreshing");
		AssertContains(progSource, "HasMilling");
		AssertContains(progSource, "HasBrewing");
		AssertRegexContains(craftSource, @"AddCraft\(""thresh grain into heads""[\s\S]*?""HasThreshing""");
		AssertRegexContains(craftSource, @"AddCraft\(""winnow threshed grain""[\s\S]*?""HasThreshing""");
		AssertRegexContains(craftSource, @"AddCraft\(""mill cleaned grain into flour""[\s\S]*?""HasMilling""");
		AssertRegexContains(craftSource, @"AddCraft\(""grind cleaned grain into meal""[\s\S]*?""HasMilling""");
		AssertRegexContains(craftSource, @"AddCraft\(""split and grind pulses""[\s\S]*?""HasMilling""");
		AssertRegexContains(craftSource, @"AddCraft\(""crush oilseeds for pressing""[\s\S]*?""HasMilling""");
		AssertRegexContains(craftSource, @"AddCraft\(""press vegetable oil from oilseed mash""[\s\S]*?""HasMilling""");
		AssertRegexContains(craftSource, @"AddCraft\(""mash grain wort""[\s\S]*?""HasBrewing""");
		AssertContains(craftSource, "trait.Name");
	}

	[TestMethod]
	public void AntiquityFoodCrafting_DeclaresCommodityTagsLiquidsPreparedFoodsAndSpoilageRules()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityFood.cs");
		var tagHierarchy = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv");

		foreach (var tag in CommodityTags)
		{
			AssertContains(itemSource, tag);
			AssertContains(tagHierarchy, $"Antiquity Food Commodities / {tag}");
		}

		foreach (var expected in new[]
		         {
			         "barley beer",
			         "date beer",
			         "mare's milk kumis",
			         "meat broth",
			         "garum sauce",
			         "spiced wine",
			         "spiced beer",
			         "spiced kumis",
			         "PreparedFood_Antiquity_Bread",
			         "PreparedFood_Antiquity_Porridge",
			         "PreparedFood_Antiquity_Stew",
			         "PreparedFood_Antiquity_Preserved",
			         "PreparedFood_Antiquity_Sweet",
			         "PreparedFood_Antiquity_Fruit",
			         "PreparedFood_Antiquity_BrinedFruit",
			         "PreparedFood_Antiquity_LuxuryStew",
			         "PreparedFood_Antiquity_LuxuryBread",
			         "PreparedFood_Antiquity_LuxurySweet",
			         "PreparedFood_Antiquity_Condiment",
			         "prepared_fruit",
			         "brined_fruit",
			         "Ready Fruit Crop",
			         "Fruit Brining Crop",
			         "Raw meat commodity spoilage",
			         "Prepared meat commodity spoilage",
			         "Salted meat commodity spoilage",
			         "Dried meat commodity spoilage",
			         "Smoked meat commodity spoilage",
			         "Broth base commodity spoilage"
		         })
		{
			AssertContains(itemSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityFoodCrafting_MakesFoodToolsAndEmptyVesselsCraftable()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityFood.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityFood.cs");

		AssertContains(itemSource, "[toolTag, \"Market / Professional Tools / Standard Tools\"]");
		foreach (var expected in new[]
		         {
			         "antiquity_food_butchers_knife",
			         "antiquity_food_cooking_knife",
			         "antiquity_food_threshing_flail",
			         "antiquity_food_winnowing_basket",
			         "antiquity_food_pitchfork",
			         "antiquity_food_quern",
			         "antiquity_food_mortar",
			         "antiquity_food_grain_sieve",
			         "antiquity_food_fruit_press",
			         "antiquity_food_oil_press",
			         "antiquity_food_mash_tun",
			         "antiquity_food_drying_rack",
			         "antiquity_food_smoking_rack",
			         "antiquity_food_salting_trough"
		         })
		{
			AssertContains(itemSource, expected);
		}

		foreach (var expected in new[]
		         {
			         "Functions / Tools / Butcher Tools / Meat Cutting Tools / Butcher's Knife",
			         "Functions / Tools / Cooking / Cooking Utensils / Cooking Knife",
			         "Functions / Tools / Foodmaking Tools / Threshing Flail",
			         "Functions / Tools / Agricultural Tools / Winnowing Basket",
			         "Functions / Tools / Agricultural Tools / Pitchfork",
			         "Functions / Tools / Foodmaking Tools / Hand Quern",
			         "Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle",
			         "Functions / Tools / Milling Tools / Grain Sieve",
			         "Functions / Tools / Foodmaking Tools / Fruit Press",
			         "Functions / Tools / Foodmaking Tools / Oil Press",
			         "Functions / Tools / Brewing Tools / Mash Tun",
			         "Functions / Tools / Cooking / Cooking Utensils / Drying Rack",
			         "Functions / Tools / Foodmaking Tools / Smoking Rack",
			         "Functions / Tools / Foodmaking Tools / Salting Trough"
		         })
		{
			AssertContains(itemSource, expected);
		}

		foreach (var expected in new[]
		         {
			         "SeedAntiquityFoodVesselCrafts();",
			         "private void SeedAntiquityFoodVesselCrafts()",
			         "finish clay serving amphora",
			         "line pitch fermenting amphora",
			         "AncientCeramicVesselmakingKnowledge",
			         "CommodityInput(900.0, \"fired clay\", \"Bisque Vessel Blank\")",
			         "CommodityInput(80.0, \"pitch\", \"Prepared Pitch\")",
			         "CommodityInput(240.0, \"pitch\", \"Prepared Pitch\")",
			         "TagTool - InRoom - an item with the Potter's Wheel tag",
			         "TagTool - Held - an item with the Potter's Rib tag",
			         "TagTool - InRoom - an item with the Lit Kiln tag",
			         "StableSimpleProduct(\"antiquity_food_serving_amphora\")",
			         "StableSimpleProduct(\"antiquity_food_fermenting_amphora\")"
		         })
		{
			AssertContains(craftSource, expected);
		}

		AssertContains(itemSource, "beerFinished is null ? null : \"antiquity_food_finished_beer_amphora\"");
		Assert.IsFalse(craftSource.Contains("StableSimpleProduct(\"antiquity_food_finished_beer_amphora\")", StringComparison.Ordinal),
			"The finished beer amphora should remain a morph target, not a direct craft output.");
	}

	[TestMethod]
	public void AntiquityFoodCrafting_UsesMilkForKumisAndKeepsCoreWineLiquidsInCoreData()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityFood.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityFood.cs");
		var materialSource = ReadSource("DatabaseSeeder", "Seeders", "CoreDataSeeder.Materials.cs");

		var beverageStockBody = ExtractMethodBody(craftSource, "CultureBeverageStockInput");
		AssertContains(beverageStockBody, "culture.BeverageLiquid.Contains(\"kumis\", StringComparison.OrdinalIgnoreCase)");
		AssertContains(beverageStockBody, "return \"LiquidUse - 3 litres of milk\";");
		Assert.IsTrue(beverageStockBody.IndexOf("\"kumis\"", StringComparison.Ordinal) <
		              beverageStockBody.IndexOf("Wort Commodity", StringComparison.Ordinal),
			"Kumis should be handled before the fallback grain-wort branch.");

		var foodLiquidsBody = ExtractMethodBody(itemSource, "EnsureAntiquityFoodLiquids");
		AssertContains(materialSource, "AddLiquid(\"red wine\"");
		AssertContains(materialSource, "AddLiquid(\"white wine\"");
		Assert.IsFalse(foodLiquidsBody.Contains("EnsureAntiquityLiquid(\"red wine\"", StringComparison.Ordinal),
			"Red wine is a core liquid and should not be duplicated by the food seeder.");
		Assert.IsFalse(foodLiquidsBody.Contains("EnsureAntiquityLiquid(\"white wine\"", StringComparison.Ordinal),
			"White wine is a core liquid and should not be duplicated by the food seeder.");
	}

	[TestMethod]
	public void AntiquityFoodCrafting_AddsOilAndFruitCommodityPaths()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityFood.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityFood.cs");

		AssertContains(itemSource, "EnsureMaterialHasTag(material, \"Oilseed Crop\")");
		AssertContains(itemSource, "EnsureMaterialHasTag(material, \"Ready Fruit Crop\")");
		AssertContains(itemSource, "EnsureMaterialHasTag(material, \"Fruit Brining Crop\")");
		AssertContains(itemSource, "\"pomegranate\"");
		AssertContains(itemSource, "\"olive crop\", \"olive\"");

		AssertContains(craftSource, "crush oilseeds for pressing");
		AssertContains(craftSource, "press vegetable oil from oilseed mash");
		AssertContains(craftSource, "press olive oil from olive mash");
		AssertContains(craftSource, "filled with 1 litre of vegetable oil");
		AssertContains(craftSource, "filled with 1 litre of olive oil");
		AssertContains(craftSource, "Oilseed Mash Commodity");
		AssertContains(craftSource, "Oilseed Cake Commodity");
		Assert.IsFalse(craftSource.Contains("press oilseed paste", StringComparison.Ordinal),
			"Oilseed processing should use the mash/press chain rather than the old rendered-fat shortcut.");

		AssertContains(craftSource, "prepare fruit for eating");
		AssertContains(craftSource, "CommodityTag - 500 grams of a material tagged as Ready Fruit Crop");
		AssertContains(craftSource, "CookedFoodProduct - 1x {_items[\"antiquity_food_prepared_fruit\"].ShortDescription}");
		AssertContains(craftSource, "brine bitter fruit stock");
		AssertContains(craftSource, "CommodityTag - 500 grams of a material tagged as Fruit Brining Crop");
		AssertContains(craftSource, "Brined Fruit Commodity");
		AssertContains(craftSource, "serve brined fruit");
		AssertContains(craftSource, "CookedFoodProduct - 1x {_items[\"antiquity_food_brined_fruit\"].ShortDescription}");
	}

	[TestMethod]
	public void AntiquityFoodCrafting_DoublesCultureSuiteWithLuxuryFoodsAndBeverages()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityFood.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityFood.cs");
		var tagHierarchy = ReadSource("Design Documents", "Data", "SeededTagHierarchy.csv");

		Assert.AreEqual(14, Regex.Matches(craftSource, @"AddCultureFoodCraft\(culture,").Count,
			"Each culture should now get the original seven foodway crafts plus seven expanded dishes or beverages.");
		AssertContains(itemSource, "Luxury Prepared Foods");
		AssertContains(tagHierarchy,
			"Luxury Prepared Foods\tPrepared Foods\tFood and Drink / Antiquity Food / Prepared Foods / Luxury Prepared Foods");

		foreach (var suffix in new[]
		         {
			         "fruit_platter",
			         "oilseed_cake",
			         "spiced_meat_stew",
			         "honeyed_pastry",
			         "fish_sauce_relish",
			         "stuffed_flatbread"
		         })
		{
			AssertContains(itemSource, $"{{culture.Key}}_{suffix}");
			AssertContains(craftSource, $"{{key}}_{suffix}");
		}

		foreach (var luxuryCraft in new[]
		         {
			         "cook {culture.Display.ToLowerInvariant()} spiced meat stew",
			         "bake {culture.Display.ToLowerInvariant()} honeyed pastry",
			         "prepare {culture.Display.ToLowerInvariant()} fish sauce relish",
			         "bake {culture.Display.ToLowerInvariant()} stuffed flatbread",
			         "fill {culture.Display.ToLowerInvariant()} spiced beverage amphora"
		         })
		{
			AssertContains(craftSource, luxuryCraft);
		}

		AssertContains(craftSource, "Commodity - 25 grams of coriander");
		AssertContains(craftSource, "Commodity - 10 grams of cumin");
		AssertContains(craftSource, "Commodity - 5 grams of saffron");
		AssertContains(craftSource, "LiquidUse - 250 millilitres of garum sauce");
		AssertContains(craftSource, "Commodity - 15 grams of black pepper");
		AssertContains(craftSource, "LiquidUse - 100 millilitres of olive oil");
		AssertContains(craftSource, "CultureLuxuryBeverageLiquid(culture)");
		AssertContains(craftSource, "Difficulty.Hard");
		AssertContains(craftSource, "LuxuryFoodPhases");
	}

	[TestMethod]
	public void AntiquityFoodCrafting_CoversAllCulturesWithVariableMeatAndPreparedFoodProducts()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityFood.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityFood.cs");

		foreach (var culture in Cultures)
		{
			AssertContains(itemSource, $"{culture} Foodways");
		}

		AssertContains(craftSource, "foreach (var culture in AntiquityFoodCultures)");
		AssertContains(craftSource, "bake {culture.Display.ToLowerInvariant()} flatbread");
		AssertContains(craftSource, "cook {culture.Display.ToLowerInvariant()} meat grain dish");
		AssertContains(craftSource, "cook {culture.Display.ToLowerInvariant()} spiced meat stew");
		AssertContains(craftSource, "prepare {culture.Display.ToLowerInvariant()} fish sauce relish");
		AssertContains(craftSource, "fill {culture.Display.ToLowerInvariant()} beverage amphora");
		AssertContains(craftSource, "fill {culture.Display.ToLowerInvariant()} spiced beverage amphora");
		AssertContains(craftSource, "CommodityTag - 500 grams of a material tagged as Meat; piletag Prepared Meat Commodity");
		AssertContains(craftSource, "CookedFoodProduct - 1x");
		AssertContains(craftSource, "LiquidProduct - 1x");
		Assert.AreEqual(0, Regex.Matches(craftSource, @"\b(beef|lamb|mutton|goat|pork)\b", RegexOptions.IgnoreCase).Count,
			"Food crafts should use tagged meat commodities rather than species-specific recipe names.");
	}

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal), $"Expected source to contain: {expected}");
	}

	private static void AssertRegexContains(string source, string expected)
	{
		Assert.IsTrue(Regex.IsMatch(source, expected), $"Expected source to match: {expected}");
	}

	private static string ExtractMethodBody(string source, string methodName)
	{
		var start = FindMethodDeclarationStart(source, methodName);
		var openBrace = source.IndexOf('{', start);
		Assert.IsTrue(openBrace >= 0, $"Could not find body for method {methodName}.");

		var depth = 0;
		for (var i = openBrace; i < source.Length; i++)
		{
			switch (source[i])
			{
				case '{':
					depth++;
					break;
				case '}':
					depth--;
					if (depth == 0)
					{
						return source[(openBrace + 1)..i];
					}
					break;
			}
		}

		Assert.Fail($"Could not extract body for method {methodName}.");
		return string.Empty;
	}

	private static int FindMethodDeclarationStart(string source, string methodName)
	{
		var search = $"{methodName}(";
		var index = source.IndexOf(search, StringComparison.Ordinal);
		while (index >= 0)
		{
			var lineStart = source.LastIndexOf('\n', index);
			lineStart = lineStart < 0 ? 0 : lineStart + 1;
			var declarationPrefix = source[lineStart..index];
			if (declarationPrefix.Contains("private ", StringComparison.Ordinal))
			{
				return lineStart;
			}

			index = source.IndexOf(search, index + search.Length, StringComparison.Ordinal);
		}

		Assert.Fail($"Could not find declaration for method {methodName}.");
		return 0;
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(SourcePath(parts));
	}

	private static string SourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
