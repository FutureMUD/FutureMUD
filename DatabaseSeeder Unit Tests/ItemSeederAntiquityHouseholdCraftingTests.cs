#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAntiquityHouseholdCraftingTests
{
	[TestMethod]
	public void AntiquityHouseholdSeeder_CurrentCatalogueHasFurnitureAndContainerTargets()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");

		Assert.AreEqual(243, CountCreateItemsInMethod(itemSource, "SeedAntiquityContainers"));
		Assert.AreEqual(89, CountCreateItemsInMethod(itemSource, "SeedAntiquityHouseholdFurniture"));
	}

	[TestMethod]
	public void AntiquityHouseholdCrafts_DiscoverAllHouseholdGoodsByMarketTags()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");

		AssertContains(craftRoot, "SeedAntiquityFurnitureAndContainerCrafts();");
		AssertContains(craftSource, "Market / Household Goods /");
		AssertContains(craftSource, "GameItemProtosTags.Any");
		AssertContains(craftSource, "SeedAntiquityHouseholdIntermediateCommodityCrafts();");
		AssertContains(craftSource, "targetItems");
	}

	[TestMethod]
	public void AntiquityHouseholdCrafts_UseCommodityIntermediatePathways()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");

		foreach (var expected in new[]
		{
			"CommodityProduct -",
			"Furniture Timber Stock",
			"Furniture Panel Stock",
			"Carved Wood Stock",
			"Coopered Staves",
			"Basketry Splint",
			"Reed Matting",
			"Prepared Leather Panel",
			"Lamp Wick",
			"Prepared Pitch",
			"Bisque Vessel Blank",
			"Glass Vessel Blank",
			"Cast Vessel Blank",
			"Stone Vessel Blank",
			"Inlay Stock",
			"Paint Pigment",
			"Lacquer Finish",
			"SimpleVariableProduct - 1x"
		})
		{
			AssertContains(craftSource, expected);
		}

		Assert.IsFalse(craftSource.Contains("\"Tag - 1x", StringComparison.Ordinal),
			"Household craft suite should prefer commodities over full-item inputs.");
	}

	[TestMethod]
	public void AntiquityHouseholdCrafts_CopySingleColourVariablesFromColourBearingInputs()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");

		foreach (var expected in new[]
		         {
			         "GetHouseholdVariableSource(",
			         "AntiquityHouseholdVariableSource.PaintPigment",
			         "AntiquityHouseholdVariableSource.LacquerFinish",
			         "CommodityPileInput(180.0, \"Paint Pigment\", colour: true, fineColour: true)",
			         "variable Colour=$i{variableInputIndex}",
			         "variable Colour=$i{variableInputIndex}, Fine Colour=$i{variableInputIndex}"
		         })
		{
			AssertContains(craftSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityHouseholdCrafts_DefineCultureKnowledgeAndNewSkills()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.AntiquityHousehold.cs");
		var skillSource = ReadSource("DatabaseSeeder", "Seeders", "SkillPackageSeeder.cs");

		foreach (var expected in new[]
		{
			"Ancient Household Crafting",
			"Ancient Woodworking and Joinery",
			"Ancient Basketry",
			"Ancient Coopering",
			"Ancient Leather Containers",
			"Ancient Ceramic Vesselmaking",
			"Ancient Glassworking",
			"Ancient Metal Vesselmaking",
			"Ancient Stone Bone and Horn Carving",
			"Ancient Lighting and Heating",
			"Hellenic Household Crafting",
			"Roman Household Crafting",
			"Egyptian Household Crafting",
			"Kushite Household Crafting",
			"Punic Household Crafting",
			"Persian Household Crafting",
			"Etruscan Household Crafting",
			"Anatolian Household Crafting",
			"Scythian-Sarmatian Household Crafting",
			"Celtic Household Crafting",
			"Germanic Household Crafting"
		})
		{
			AssertContains(craftSource, expected);
		}

		foreach (var expected in new[]
		{
			"new SkillDetails(\"Basketry\"",
			"new SkillDetails(\"Coopering\"",
			"new SkillDetails(\"Ropemaking\"",
			"new SkillDetails(\"Lacquerwork\""
		})
		{
			AssertContains(skillSource, expected);
		}
	}

	[TestMethod]
	public void AntiquityHouseholdCrafts_AddToolAndCommodityTagsAndPeriodTools()
	{
		var tagSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.Tags.cs");
		var toolSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.AntiquityHouseholdTools.cs");
		var reworkRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");

		foreach (var expected in new[]
		{
			"AddTag(context, \"Household Craft Stock\", \"Material Functions\")",
			"AddTag(context, \"Furniture Timber Stock\", \"Household Craft Stock\")",
			"AddTag(context, \"Bisque Vessel Blank\", \"Household Craft Stock\")",
			"AddTag(context, \"Glass Vessel Blank\", \"Household Craft Stock\")",
			"AddTag(context, \"Cast Vessel Blank\", \"Household Craft Stock\")",
			"AddTag(context, \"Stone Vessel Blank\", \"Household Craft Stock\")",
			"AddTag(context, \"Bow Drill\", \"Woodcrafting Tools\")",
			"AddTag(context, \"Vessel Casting Mould\", \"Casting Mould\")",
			"AddTag(context, \"Candle Mould\", \"Candlemaking Tools\")",
			"AddTag(context, \"Lamp Mould\", \"Candlemaking Tools\")"
		})
		{
			AssertContains(tagSource, expected);
		}

		foreach (var expected in new[]
		{
			"SeedAntiquityHouseholdCraftTools();",
			"antiquity_bronze_hand_saw",
			"antiquity_bronze_adze",
			"antiquity_basket_knife",
			"antiquity_slow_potters_wheel",
			"antiquity_updraft_kiln",
			"antiquity_glass_blowpipe",
			"antiquity_vessel_casting_mould",
			"antiquity_candle_mould",
			"antiquity_lacquer_brush"
		})
		{
			Assert.IsTrue(reworkRoot.Contains(expected, StringComparison.Ordinal) ||
			              toolSource.Contains(expected, StringComparison.Ordinal),
				$"Expected source to contain: {expected}");
		}
	}

	private static int CountCreateItemsInMethod(string source, string methodName)
	{
		var body = ExtractMethodBody(source, methodName);
		return Regex.Matches(body, @"\bCreateItem\s*\(").Count;
	}

	private static string ExtractMethodBody(string source, string methodName)
	{
		var marker = $"private void {methodName}()";
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find method {methodName}.");
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

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal), $"Expected source to contain: {expected}");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}
}
