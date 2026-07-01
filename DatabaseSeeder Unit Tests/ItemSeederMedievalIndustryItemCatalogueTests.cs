#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederMedievalIndustryItemCatalogueTests
{
	[TestMethod]
	public void MedievalIndustryItemSeeder_ShouldContainToolAndStockCatalogueRows()
	{
		var toolSource = ReadDatabaseSeederSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalHouseholdTools.cs");
		var stockSource = ReadDatabaseSeederSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalComponentGaps.cs");
		var designReference = ReadDatabaseSeederSource("Design Documents", "Seeding", "FutureMUD_Medieval_Industry_Tools_And_Stock_Item_Catalogue.md");

		var toolReferences = Regex.Matches(toolSource, "\\\"medieval_(?:workshop|tool)_[^\\\"]+\\\"")
			.Select(x => x.Value.Trim('"'))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		var stockReferences = Regex.Matches(stockSource, "\\\"medieval_industry_stock_[^\\\"]+\\\"")
			.Select(x => x.Value.Trim('"'))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

		Assert.IsTrue(toolReferences.Length >= 150, $"Expected at least 150 tool/workshop rows but found {toolReferences.Length}.");
		Assert.IsTrue(stockReferences.Length >= 50, $"Expected at least 50 intermediate stock rows but found {stockReferences.Length}.");

		StringAssert.Contains(toolSource, "private void SeedMedievalHouseholdCraftTools()");
		StringAssert.Contains(stockSource, "private void SeedMedievalComponentGapItems()");
		StringAssert.Contains(toolSource, "Tool_Blacksmithing_General");
		StringAssert.Contains(toolSource, "Tool_Woodcrafting_General");
		StringAssert.Contains(toolSource, "Tool_Textilecraft_General");
		StringAssert.Contains(toolSource, "Tool_Leatherworking_General");
		StringAssert.Contains(toolSource, "Tool_Papermaking_General");
		StringAssert.Contains(toolSource, "Tool_Bookbinding_General");
		StringAssert.Contains(toolSource, "Tool_Jewellery_General");
		StringAssert.Contains(toolSource, "Tool_Medical_General");
		StringAssert.Contains(toolSource, "TimeSpan.FromHours");

		StringAssert.Contains(stockSource, "medieval_industry_stock_plank_bundle");
		StringAssert.Contains(stockSource, "medieval_industry_stock_yarn_skein");
		StringAssert.Contains(stockSource, "medieval_industry_stock_leather_panel");
		StringAssert.Contains(stockSource, "medieval_industry_stock_parchment_sheet");
		StringAssert.Contains(stockSource, "medieval_industry_stock_iron_bar");
		StringAssert.Contains(stockSource, "medieval_industry_stock_bandage_roll");

		StringAssert.Contains(designReference, "168 tool/workshop prototypes");
		StringAssert.Contains(designReference, "50 intermediate stock prototypes");
		StringAssert.Contains(designReference, "218 total item prototypes");
	}

	[TestMethod]
	public void MedievalIndustryItemSeeder_ShouldUseMergedPrerequisiteToolComponents()
	{
		var usefulSeederComponents = ReadDatabaseSeederSource("DatabaseSeeder", "Seeders", "UsefulSeeder.ItemComponents.cs");
		var toolSource = ReadDatabaseSeederSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalHouseholdTools.cs");

		var requiredComponents = new[]
		{
			"Tool_Blacksmithing_General",
			"Tool_Armouring_General",
			"Tool_Weaponsmithing_General",
			"Tool_Woodcrafting_General",
			"Tool_Coopering_General",
			"Tool_Textilecraft_General",
			"Tool_Dyeing_Fulling_General",
			"Tool_Leatherworking_General",
			"Tool_Parchmentmaking_General",
			"Tool_Papermaking_General",
			"Tool_Bookbinding_General",
			"Tool_Pottery_General",
			"Tool_Masonry_General",
			"Tool_Glassblowing_General",
			"Tool_Lapidary_General",
			"Tool_Jewellery_General",
			"Tool_Apothecary_General",
			"Tool_Medical_General",
			"Tool_Printing_Woodblock_General"
		};

		foreach (var component in requiredComponents)
		{
			StringAssert.Contains(usefulSeederComponents, component);
			StringAssert.Contains(toolSource, component);
		}
	}

	private static string ReadDatabaseSeederSource(params string[] parts)
	{
		return File.ReadAllText(Path.Combine(new[] { SourceRoot() }.Concat(parts).ToArray()));
	}

	private static string SourceRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate repository root from test output path.");
		return directory.FullName;
	}
}
