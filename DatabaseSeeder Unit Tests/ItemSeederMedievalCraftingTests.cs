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
	public void MedievalItemLaunchers_AreNoOpsExceptImplementedDirectCatalogues()
	{
		foreach (var (fileName, methodName) in MedievalItemLaunchers
			         .Where(x => !x.Value.Equals("SeedMedievalClothing", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalHouseholdFurniture", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalArmour", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalWeaponsShieldsAccessories", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalWritingAdministrationAndDocuments", StringComparison.Ordinal)))
		{
			var source = ReadSource("DatabaseSeeder", "Seeders", fileName);
			AssertNoOpMethod(source, methodName);
		}

		var medievalItemSource = ReadMedievalItemSources(
			"ItemSeeder.Rework.MedievalClothing.cs",
			"ItemSeeder.Rework.MedievalFurniture.cs",
			"ItemSeeder.Rework.MedievalArmour.cs",
			"ItemSeeder.Rework.MedievalWeapons.cs",
			"ItemSeeder.Rework.MedievalWriting.cs");
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
	public void MedievalMilitarySeeder_ImplementsFullCatalogueWithDirectCreateItemCalls()
	{
		var armourSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalArmour.cs");
		var weaponsSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalWeapons.cs");
		var armourReferences = Regex.Matches(
				armourSource,
				@"CreateItem\s*\(\s*""(?<ref>medieval_military_[^""]+)""",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var weaponReferences = Regex.Matches(
				weaponsSource,
				@"CreateItem\s*\(\s*""(?<ref>medieval_military_[^""]+)""",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();

		Assert.AreEqual(142, weaponReferences.Length,
			"SeedMedievalWeaponsShieldsAccessories should contain exactly one direct CreateItem call for each melee, ranged, ammunition, and thrown catalogue row.");
		Assert.AreEqual(239, armourReferences.Length,
			"SeedMedievalArmour should contain exactly one direct CreateItem call for each armour, shield, and military carrying, storage, and support gear row.");
		Assert.AreEqual(weaponReferences.Length, weaponReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval military weapon or ammunition item should be created exactly once.");
		Assert.AreEqual(armourReferences.Length, armourReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval military armour, shield, or support gear item should be created exactly once.");

		CollectionAssert.AreEqual(
			new[]
			{
				"medieval_military_worn_fighting_knife",
				"medieval_military_training_axe_headed_polearm",
				"medieval_military_worn_ash_shortbow",
				"medieval_military_short_throwing_spear"
			},
			new[]
			{
				weaponReferences.First(),
				weaponReferences[76],
				weaponReferences[77],
				weaponReferences.Last()
			},
			"The weapons partial should contain the melee and ranged/ammunition/thrown catalogue slices in source order.");
		CollectionAssert.AreEqual(
			new[]
			{
				"medieval_military_padded_arming_cap",
				"medieval_military_lamellar_croupiere",
				"medieval_military_worn_round_shield",
				"medieval_military_door_board_shield",
				"medieval_military_rough_rawhide_knife_sheath",
				"medieval_military_spare_crossbow_string"
			},
			new[]
			{
				armourReferences.First(),
				armourReferences[104],
				armourReferences[105],
				armourReferences[169],
				armourReferences[170],
				armourReferences.Last()
			},
			"The armour partial should contain the armour/tack/barding, shield, and carrying/storage/support catalogue slices in source order.");

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
			Assert.IsFalse(armourSource.Contains(forbidden, StringComparison.Ordinal),
				$"SeedMedievalArmour should remain direct CreateItem calls without helper catalogue token {forbidden}.");
			Assert.IsFalse(weaponsSource.Contains(forbidden, StringComparison.Ordinal),
				$"SeedMedievalWeaponsShieldsAccessories should remain direct CreateItem calls without helper catalogue token {forbidden}.");
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
	public void MedievalFurnitureSeeder_ImplementsReferenceCatalogueWithDirectCreateItemCalls()
	{
		var furnitureSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalFurniture.cs");
		var sourceReferences = Regex.Matches(
				furnitureSource,
				@"CreateItem\s*\(\s*""(?<ref>medieval_[^""]+)""",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();

		Assert.AreEqual(1751, sourceReferences.Length,
			"SeedMedievalHouseholdFurniture should contain exactly one direct CreateItem call for each source catalogue row.");
		Assert.AreEqual(sourceReferences.Length, sourceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval household item should be created exactly once.");

		foreach (var forbidden in new[]
		{
			"foreach",
			"for (",
			"Dictionary<",
			"IReadOnly",
			"BuildMedieval",
			"SeedEraItemSpecs(",
			"EnsureMedieval"
		})
		{
			Assert.IsFalse(furnitureSource.Contains(forbidden, StringComparison.Ordinal),
				$"SeedMedievalHouseholdFurniture should remain direct CreateItem calls without helper catalogue token {forbidden}.");
		}
	}

	[TestMethod]
	public void MedievalWritingSeeder_ImplementsReferenceCatalogueWithDirectCreateItemCalls()
	{
		var writingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalWriting.cs");
		var designReference = ReadSource("Design Documents", "Seeding", "FutureMUD_Medieval_Writing_Books_Documents_Design_Reference.md");
		var fdescCatalogue = ReadSource("Design Documents", "Seeding", "FutureMUD_Medieval_Writing_Books_Documents_FDesc_Catalogue.csv");

		var designReferences = Regex.Matches(
				designReference,
				@"^\| `(?<ref>medieval_writing_[^`]+)` \|",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var csvReferences = Regex.Matches(
				fdescCatalogue,
				@"^(?<ref>medieval_writing_[^,\r\n]+),",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var sourceReferences = Regex.Matches(
				writingSource,
				@"CreateItem\s*\(\s*""(?<ref>medieval_writing_[^""]+)""",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();

		Assert.AreEqual(286, designReferences.Length,
			"The writing design reference should contain the full 286-item catalogue.");
		CollectionAssert.AreEqual(designReferences, csvReferences,
			"The writing fdesc catalogue should stay in the same order as the design reference.");
		CollectionAssert.AreEqual(designReferences, sourceReferences,
			"SeedMedievalWritingAdministrationAndDocuments should contain exactly one direct CreateItem call for each writing reference.");
		Assert.AreEqual(sourceReferences.Length, sourceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval writing, book, document, seal, container, scribal-tool, or writing-support item should be created exactly once.");

		foreach (var forbidden in new[]
		{
			"foreach",
			"for (",
			"Dictionary<",
			"IReadOnly",
			"BuildMedieval",
			"SeedEraItemSpecs(",
			"EnsureMedieval"
		})
		{
			Assert.IsFalse(writingSource.Contains(forbidden, StringComparison.Ordinal),
				$"SeedMedievalWritingAdministrationAndDocuments should remain direct CreateItem calls without helper catalogue token {forbidden}.");
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
			new[]
			{
				"Medieval_Crafting_Audit.md",
				"Medieval_Clothing_Seeder_Design_Reference.md",
				"Medieval_Household_Goods_Furniture_Seeder_Design_Reference.md",
				"Medieval_Military_Seeder_Design_Reference.md"
			},
			medievalDocs,
			"The surviving medieval design documents should be the current audit and live source references.");
		Assert.IsTrue(File.Exists(Path.Combine(seedingDocPath, "Medieval_Clothing_FDesc_Catalogue.csv")),
			"The live medieval clothing fdesc catalogue should remain beside its design reference.");
		Assert.IsTrue(File.Exists(Path.Combine(seedingDocPath, "FutureMUD_Medieval_Writing_Books_Documents_Design_Reference.md")),
			"The live medieval writing design reference should remain beside the other seeding references.");
		Assert.IsTrue(File.Exists(Path.Combine(seedingDocPath, "FutureMUD_Medieval_Writing_Books_Documents_FDesc_Catalogue.csv")),
			"The live medieval writing fdesc catalogue should remain beside its design reference.");

		var indexSource = ReadSource("Design Documents", "README.md");
		AssertContains(indexSource, "[Medieval ItemSeeder Rebuild Audit](./Seeding/Medieval_Crafting_Audit.md)");
		AssertContains(indexSource, "[Medieval Clothing Seeder Design Reference](./Seeding/Medieval_Clothing_Seeder_Design_Reference.md)");
		AssertContains(indexSource, "[Medieval Household Goods and Furniture Seeder Design Reference](./Seeding/Medieval_Household_Goods_Furniture_Seeder_Design_Reference.md)");
		AssertContains(indexSource, "[Medieval Military Seeder Design Reference](./Seeding/Medieval_Military_Seeder_Design_Reference.md)");
		AssertContains(indexSource, "[Medieval Writing, Books, and Documents Seeder Design Reference](./Seeding/FutureMUD_Medieval_Writing_Books_Documents_Design_Reference.md)");
		foreach (var removed in RetiredMedievalDesignDocuments)
		{
			Assert.IsFalse(indexSource.Contains(removed, StringComparison.Ordinal),
				$"Design document index should not link retired medieval document {removed}.");
		}

		var auditSource = ReadSource("Design Documents", "Seeding", "Medieval_Crafting_Audit.md");
		AssertContains(auditSource, "`SeedMedievalClothing` contains the direct clothing item `CreateItem(...)` calls.");
		AssertContains(auditSource, "`SeedMedievalHouseholdFurniture` contains the direct household goods");
		AssertContains(auditSource, "Medieval_Clothing_Seeder_Design_Reference.md");
		AssertContains(auditSource, "Medieval_Clothing_FDesc_Catalogue.csv");
		AssertContains(auditSource, "Medieval_Household_Goods_Furniture_Seeder_Design_Reference.md");
		AssertContains(auditSource, "ItemSeeder.Rework.MedievalFurniture.cs");
		AssertContains(auditSource, "`SeedMedievalWeaponsShieldsAccessories` contains the direct melee weapon, ranged weapon, ammunition, and thrown-weapon `CreateItem(...)` calls.");
		AssertContains(auditSource, "`SeedMedievalArmour` contains the direct armour, horse tack, barding, shield, and military support-gear `CreateItem(...)` calls.");
		AssertContains(auditSource, "`SeedMedievalWritingAdministrationAndDocuments` contains the direct writing-surface, book, document, seal, container, scribal-tool, and writing-support `CreateItem(...)` calls.");
		AssertContains(auditSource, "Medieval_Military_Seeder_Design_Reference.md");
		AssertContains(auditSource, "ItemSeeder.Rework.MedievalWeapons.cs");
		AssertContains(auditSource, "ItemSeeder.Rework.MedievalArmour.cs");
		AssertContains(auditSource, "FutureMUD_Medieval_Writing_Books_Documents_Design_Reference.md");
		AssertContains(auditSource, "FutureMUD_Medieval_Writing_Books_Documents_FDesc_Catalogue.csv");
		AssertContains(auditSource, "ItemSeeder.Rework.MedievalWriting.cs");
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
