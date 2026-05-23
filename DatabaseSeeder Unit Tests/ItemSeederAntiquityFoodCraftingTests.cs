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
			         "PreparedFood_Antiquity_Bread",
			         "PreparedFood_Antiquity_Porridge",
			         "PreparedFood_Antiquity_Stew",
			         "PreparedFood_Antiquity_Preserved",
			         "PreparedFood_Antiquity_Sweet",
			         "PreparedFood_Antiquity_Fruit",
			         "PreparedFood_Antiquity_BrinedFruit",
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
		AssertContains(craftSource, "fill {culture.Display.ToLowerInvariant()} beverage amphora");
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
