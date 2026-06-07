#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederMedievalCraftingTests
{
	private static readonly IReadOnlyDictionary<string, string> MedievalItemLaunchers = new Dictionary<string, string>
	{
		["ItemSeeder.Rework.MedievalArmour.cs"] = "SeedMedievalArmour",
		["ItemSeeder.Rework.MedievalClothing.cs"] = "SeedMedievalClothing",
		["ItemSeeder.Rework.MedievalComponentGaps.cs"] = "SeedMedievalComponentGapItems",
		["ItemSeeder.Rework.MedievalContainers.cs"] = "SeedMedievalContainers",
		["ItemSeeder.Rework.MedievalDoorsLocksStrongboxes.cs"] = "SeedMedievalDoorsLocksAndStrongboxes",
		["ItemSeeder.Rework.MedievalFood.cs"] = "SeedMedievalFoodAndBeverageItems",
		["ItemSeeder.Rework.MedievalFurniture.cs"] = "SeedMedievalHouseholdFurniture",
		["ItemSeeder.Rework.MedievalHouseholdTools.cs"] = "SeedMedievalHouseholdCraftTools",
		["ItemSeeder.Rework.MedievalJewellery.cs"] = "SeedMedievalJewelleryAndDevotionalGoods",
		["ItemSeeder.Rework.MedievalMedical.cs"] = "SeedMedievalMedicalAndApothecaryItems",
		["ItemSeeder.Rework.MedievalRepairKits.cs"] = "SeedMedievalRepairKits",
		["ItemSeeder.Rework.MedievalWeapons.cs"] = "SeedMedievalWeaponsShieldsAccessories",
		["ItemSeeder.Rework.MedievalWriting.cs"] = "SeedMedievalWritingAdministrationAndDocuments"
	};

	private static readonly string[] MedievalCraftLaunchers =
	[
		"SeedMedievalProductionChainCrafts",
		"SeedMedievalClothingCrafts",
		"SeedMedievalEquipmentCrafts",
		"SeedMedievalWritingAdministrationCrafts",
		"SeedMedievalMedicalApothecaryCrafts",
		"SeedMedievalJewelleryDevotionalCrafts",
		"SeedMedievalFurnitureAndContainerCrafts",
		"SeedMedievalFoodBeverageCrafts",
		"SeedMedievalRepairKitCrafts",
		"SeedMedievalComponentGapCrafts"
	];

	private static readonly string[] RetiredMedievalDesignDocuments =
	[
		"Medieval_Clothing_Crafting_Suite.md",
		"Medieval_Culture_Catalogue.md",
		"Medieval_Equipment_Crafting_Suite.md",
		"Medieval_Food_Beverage_Crafting_Suite.md",
		"Medieval_Furniture_Container_Crafting_Suite.md",
		"Medieval_Implementation_Roadmap.md",
		"Medieval_Item_Component_Gap_Report.md",
		"Medieval_Jewellery_Devotional_Crafting_Suite.md",
		"Medieval_Medical_Apothecary_Crafting_Suite.md",
		"Medieval_Outfit_Catalogue.md",
		"Medieval_Testing_Quality_Gates.md",
		"Medieval_Writing_Administration_Crafting_Suite.md"
	];

	[TestMethod]
	public void MedievalDispatcher_WiresHistoricFoundationAndLaunchStubs()
	{
		var reworkRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");

		AssertContains(reworkRoot, "SeedHistoricCommonWorkshopItems();");
		foreach (var method in MedievalItemLaunchers.Values)
		{
			AssertContains(reworkRoot, $"{method}();");
		}

		AssertContains(craftRoot, "SeedHistoricFoundationCrafts();");
		foreach (var method in MedievalCraftLaunchers)
		{
			AssertContains(craftRoot, $"{method}();");
		}
	}

	[TestMethod]
	public void HistoricFoundation_SourceLivesOutsideMedievalPartials()
	{
		var historicItemPath = SourcePath("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.HistoricFoundation.cs");
		var historicCraftPath = SourcePath("DatabaseSeeder", "Seeders", "ItemSeederCrafting.HistoricFoundation.cs");

		Assert.IsTrue(File.Exists(historicItemPath), "Expected historic foundation item source to have its own partial file.");
		Assert.IsTrue(File.Exists(historicCraftPath), "Expected historic foundation craft source to have its own partial file.");
		Assert.IsFalse(File.Exists(SourcePath("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalCommonWorkshop.cs")),
			"Historic workshop foundations should no longer live in a medieval-named item file.");

		var historicItemSource = File.ReadAllText(historicItemPath);
		var historicCraftSource = File.ReadAllText(historicCraftPath);
		AssertContains(historicItemSource, "SeedHistoricCommonWorkshopItems");
		AssertContains(historicItemSource, "HistoricFoundationItemSpecs");
		AssertContains(historicCraftSource, "SeedHistoricFoundationCrafts");
		AssertContains(historicCraftSource, "GetHistoricFoundationCraftPath");

		var medievalItemSource = ReadMedievalItemSources();
		var medievalCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");
		Assert.IsFalse(medievalItemSource.Contains("HistoricFoundationItemSpecs", StringComparison.Ordinal),
			"Historic foundation specs should not remain in medieval item source.");
		Assert.IsFalse(medievalCraftSource.Contains("SeedHistoricFoundationCrafts", StringComparison.Ordinal),
			"Historic foundation crafts should not remain in the medieval craft partial.");
	}

	[TestMethod]
	public void MedievalItemLaunchers_AreNoOpsExceptClothing()
	{
		foreach (var (fileName, methodName) in MedievalItemLaunchers
			         .Where(x => !x.Value.Equals("SeedMedievalClothing", StringComparison.Ordinal)))
		{
			var source = ReadSource("DatabaseSeeder", "Seeders", fileName);
			AssertNoOpMethod(source, methodName);
		}

		var medievalItemSource = ReadMedievalItemSources("ItemSeeder.Rework.MedievalClothing.cs");
		foreach (var forbidden in new[]
		{
			"CreateItem(",
			"SeedEraItemSpecs(",
			"EnsureMedievalItemMaterialAndTags",
			"BuildMedieval",
			"MedievalCultureProfile",
			"MedievalExplicitOutfit",
			"MedievalAuthoredOutfit"
		})
		{
			Assert.IsFalse(medievalItemSource.Contains(forbidden, StringComparison.Ordinal),
				$"Medieval item launch stubs should not retain retired source token {forbidden}.");
		}
	}

	[TestMethod]
	public void MedievalClothingSeeder_ImplementsReferenceCatalogueWithDirectCreateItemCalls()
	{
		var clothingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalClothing.cs");
		var designReference = ReadSource("Design Documents", "Seeding", "Medieval_Clothing_Seeder_Design_Reference.md");
		var fdescCatalogue = ReadSource("Design Documents", "Seeding", "Medieval_Clothing_FDesc_Catalogue.csv");

		var designReferences = Regex.Matches(
				designReference,
				@"^- `(?<ref>medieval_[^`]+)` - .*?; noun: `[^`]+`; material:",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var csvReferences = Regex.Matches(
				fdescCatalogue,
				@"^(?<ref>medieval_[^,\r\n]+),",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var sourceReferences = Regex.Matches(
				clothingSource,
				@"CreateItem\s*\(\s*""(?<ref>medieval_[^""]+)""",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();

		Assert.AreEqual(408, designReferences.Length, "The design reference should contain the full 408-garment catalogue.");
		CollectionAssert.AreEqual(designReferences, csvReferences,
			"The fdesc catalogue should stay in the same order as the design reference.");
		CollectionAssert.AreEqual(designReferences, sourceReferences,
			"SeedMedievalClothing should contain exactly one direct CreateItem call for each clothing reference.");
		Assert.AreEqual(sourceReferences.Length, sourceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval clothing item should be created exactly once.");

		foreach (var forbidden in new[]
		{
			"foreach",
			"for (",
			"Dictionary<",
			"IReadOnly",
			"record ",
			"BuildMedieval",
			"SeedEraItemSpecs(",
			"EnsureMedieval"
		})
		{
			Assert.IsFalse(clothingSource.Contains(forbidden, StringComparison.Ordinal),
				$"SeedMedievalClothing should remain direct CreateItem calls without helper catalogue token {forbidden}.");
		}
	}

	[TestMethod]
	public void MedievalCraftLaunchers_AreNoOps()
	{
		var medievalCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");
		foreach (var method in MedievalCraftLaunchers)
		{
			AssertNoOpMethod(medievalCraftSource, method);
		}

		foreach (var forbidden in new[]
		{
			"AddMedievalCraft",
			"BuildMedieval",
			"MedievalCultureProfile",
			"MedievalExplicitOutfit",
			"MedievalAuthoredOutfit",
			"MedievalCraftedItemStableReferencesForTesting"
		})
		{
			Assert.IsFalse(medievalCraftSource.Contains(forbidden, StringComparison.Ordinal),
				$"Medieval craft launch stubs should not retain retired source token {forbidden}.");
		}
	}

	[TestMethod]
	public void HistoricFoundation_TestDataRemainsAvailable()
	{
		var stableReferences = ItemSeeder.HistoricFoundationStableReferencesForTesting;
		var specs = ItemSeeder.HistoricFoundationItemSpecsForTesting;

		Assert.IsTrue(stableReferences.Contains("historic_workshop_hearth", StringComparer.OrdinalIgnoreCase));
		Assert.IsTrue(stableReferences.Contains("historic_lit_workshop_hearth", StringComparer.OrdinalIgnoreCase));
		Assert.AreEqual(stableReferences.Count, specs.Count);
		Assert.IsTrue(specs.All(x => x.Tags.Count > 0), "Historic foundation specs should retain seeded tags.");
		Assert.IsTrue(specs.All(x => x.Components.Count > 0), "Historic foundation specs should retain seeded components.");
	}

	[TestMethod]
	public void MedievalRetiredDataFiles_AreRemoved()
	{
		foreach (var removed in new[]
		{
			"ItemSeeder.Rework.Medieval.cs",
			"ItemSeeder.Rework.MedievalAuthoredOutfitPieces.cs",
			"ItemSeeder.Rework.MedievalSupport.cs"
		})
		{
			Assert.IsFalse(File.Exists(SourcePath("DatabaseSeeder", "Seeders", removed)),
				$"Expected obsolete medieval data/support file {removed} to be removed.");
		}
	}

	[TestMethod]
	public void MedievalRetiredDesignDocuments_AreRemovedAndIndexPointsAtCurrentDocs()
	{
		var seedingDocPath = SourcePath("Design Documents", "Seeding");
		foreach (var removed in RetiredMedievalDesignDocuments)
		{
			Assert.IsFalse(File.Exists(Path.Combine(seedingDocPath, removed)),
				$"Expected retired medieval design document {removed} to be removed with the old seeder content.");
		}

		var medievalDocs = Directory.GetFiles(seedingDocPath, "Medieval_*.md")
			.Select(Path.GetFileName)
			.ToArray();
		CollectionAssert.AreEquivalent(
			new[] { "Medieval_Crafting_Audit.md", "Medieval_Clothing_Seeder_Design_Reference.md" },
			medievalDocs,
			"The only surviving medieval design documents should be the current audit and live clothing reference.");
		Assert.IsTrue(File.Exists(Path.Combine(seedingDocPath, "Medieval_Clothing_FDesc_Catalogue.csv")),
			"The live medieval clothing fdesc catalogue should remain beside its design reference.");

		var indexSource = ReadSource("Design Documents", "README.md");
		AssertContains(indexSource, "[Medieval ItemSeeder Rebuild Audit](./Seeding/Medieval_Crafting_Audit.md)");
		AssertContains(indexSource, "[Medieval Clothing Seeder Design Reference](./Seeding/Medieval_Clothing_Seeder_Design_Reference.md)");
		foreach (var removed in RetiredMedievalDesignDocuments)
		{
			Assert.IsFalse(indexSource.Contains(removed, StringComparison.Ordinal),
				$"Design document index should not link retired medieval document {removed}.");
		}

		var auditSource = ReadSource("Design Documents", "Seeding", "Medieval_Crafting_Audit.md");
		AssertContains(auditSource, "`SeedMedievalClothing` is the first rebuilt medieval item slice");
		AssertContains(auditSource, "Medieval_Clothing_Seeder_Design_Reference.md");
		AssertContains(auditSource, "Medieval_Clothing_FDesc_Catalogue.csv");
		AssertContains(auditSource, "ItemSeeder.Rework.HistoricFoundation.cs");
		AssertContains(auditSource, "ItemSeederCrafting.HistoricFoundation.cs");
		Assert.IsFalse(auditSource.Contains("Medieval_Outfit_Catalogue.md", StringComparison.Ordinal),
			"The medieval audit should not point readers to retired outfit catalogue payloads.");
		Assert.IsFalse(auditSource.Contains("Medieval_Culture_Catalogue.md", StringComparison.Ordinal),
			"The medieval audit should not point readers to retired culture catalogue payloads.");
	}

	private static string ReadMedievalItemSources(params string[] excludedFileNames)
	{
		var excluded = excludedFileNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
		return string.Join(Environment.NewLine,
			Directory.GetFiles(SourcePath("DatabaseSeeder", "Seeders"), "ItemSeeder.Rework.Medieval*.cs")
				.Where(x => !excluded.Contains(Path.GetFileName(x)))
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.Select(File.ReadAllText));
	}

	private static void AssertNoOpMethod(string source, string methodName)
	{
		var pattern = $@"private\s+void\s+{Regex.Escape(methodName)}\s*\(\s*\)\s*\{{\s*\}}";
		Assert.IsTrue(Regex.IsMatch(source, pattern, RegexOptions.CultureInvariant),
			$"Expected {methodName} to be present as an empty no-op method.");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(SourcePath(parts));
	}

	private static string SourcePath(params string[] parts)
	{
		return Path.Combine(new[] { SourceRoot() }.Concat(parts).ToArray());
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

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal), $"Expected source to contain: {expected}");
	}
}
