#nullable enable

using Recipes = MudSharp_Unit_Tests.CompiledRecipeFixtures;

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
		["ItemSeeder.MedievalArmour.cs"] = "SeedMedievalArmour",
		["ItemSeeder.MedievalClothing.cs"] = "SeedMedievalClothing",
		["ItemSeeder.MedievalComponentGaps.cs"] = "SeedMedievalComponentGapItems",
		["ItemSeeder.MedievalContainers.cs"] = "SeedMedievalContainers",
		["ItemSeeder.MedievalDoorsLocksStrongboxes.cs"] = "SeedMedievalDoorsLocksAndStrongboxes",
		["ItemSeeder.MedievalFood.cs"] = "SeedMedievalFoodAndBeverageItems",
		["ItemSeeder.MedievalFoodProduction.cs"] = "SeedMedievalFoodProductionFoundationItems",
		["ItemSeeder.MedievalFurniture.cs"] = "SeedMedievalHouseholdFurniture",
		["ItemSeeder.MedievalHouseholdTools.cs"] = "SeedMedievalHouseholdCraftTools",
		["ItemSeeder.MedievalJewellery.cs"] = "SeedMedievalJewelleryAndDevotionalGoods",
		["ItemSeeder.MedievalMedical.cs"] = "SeedMedievalMedicalAndApothecaryItems",
		["ItemSeeder.MedievalRepairKits.cs"] = "SeedMedievalRepairKits",
		["ItemSeeder.MedievalWeapons.cs"] = "SeedMedievalWeaponsShieldsAccessories",
		["ItemSeeder.MedievalWriting.cs"] = "SeedMedievalWritingAdministrationAndDocuments"
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
		var selected = CompiledRecipeCatalogue.Select(new System.Collections.Generic.HashSet<string>(["medieval"], StringComparer.OrdinalIgnoreCase), false).ToArray();
		Assert.IsTrue(selected.Length > 1000);
		Assert.IsTrue(selected.Any(recipe => recipe.Gate is CatalogueKnowledgeRecipeGate gate && gate.Knowledge == "Historic Workshop Foundations"));
		Assert.IsTrue(selected.Any(recipe => recipe.Gate is CatalogueKnowledgeRecipeGate gate && gate.Knowledge == "Medieval Industry Foundations"));
	}

	[TestMethod]
	public void HistoricFoundation_SourceLivesOutsideMedievalPartials()
	{
		var historicItemPath = SourcePath("DatabaseSeeder", "Seeders", "ItemSeeder.HistoricFoundation.cs");
		var historicCraftPath = SourcePath("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.HistoricFoundation.cs");

		Assert.IsTrue(File.Exists(historicItemPath), "Expected historic foundation item source to have its own partial file.");
		Assert.IsTrue(File.Exists(historicCraftPath), "Expected historic foundation craft source to have its own partial file.");
		Assert.IsFalse(File.Exists(SourcePath("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalCommonWorkshop.cs")),
			"Historic workshop foundations should no longer live in a medieval-named item file.");

		var historicItemSource = File.ReadAllText(historicItemPath);
		var historicCraftSource = File.ReadAllText(historicCraftPath);
		AssertContains(historicItemSource, "SeedHistoricCommonWorkshopItems");
		AssertContains(historicItemSource, "HistoricFoundationItemSpecs");
		AssertContains(historicCraftSource, "SeedHistoricFoundationCrafts");
		AssertContains(historicCraftSource, "GetHistoricFoundationCraftPath");

		var medievalItemSource = ReadMedievalItemSources();
		var medievalCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.Medieval.cs");
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
			         .Where(x => !x.Value.Equals("SeedMedievalContainers", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalDoorsLocksAndStrongboxes", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalFoodAndBeverageItems", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalFoodProductionFoundationItems", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalArmour", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalWeaponsShieldsAccessories", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalWritingAdministrationAndDocuments", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalMedicalAndApothecaryItems", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalJewelleryAndDevotionalGoods", StringComparison.Ordinal))
			         .Where(x => !x.Value.Equals("SeedMedievalRepairKits", StringComparison.Ordinal)))
		{
			var source = ReadSource("DatabaseSeeder", "Seeders", fileName);
			AssertNoOpMethod(source, methodName);
		}

		var medievalItemSource = ReadMedievalItemSources(
			"ItemSeeder.MedievalClothing.cs",
			"ItemSeeder.MedievalFurniture.cs",
			"ItemSeeder.MedievalContainers.cs",
			"ItemSeeder.MedievalDoorsLocksStrongboxes.cs",
			"ItemSeeder.MedievalFood.cs",
			"ItemSeeder.MedievalFoodProduction.cs",
			"ItemSeeder.MedievalArmour.cs",
			"ItemSeeder.MedievalWeapons.cs",
			"ItemSeeder.MedievalWriting.cs",
			"ItemSeeder.MedievalMedical.cs",
			"ItemSeeder.MedievalJewellery.cs",
			"ItemSeeder.MedievalRepairKits.cs");
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
	public void MedievalJewellerySeeder_ImplementsReferenceCatalogueWithExplicitCatalogueDefinitions()
	{
		var jewellerySource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalJewellery.cs");
		var designReference = ReadSource("Design Documents", "Seeding", "Medieval_Jewellery_Seeder_Design_Reference.md");
		var itemCatalogue = ReadSource("Design Documents", "Seeding", "FutureMUD_Medieval_Jewellery_Item_Catalogue_Full.csv");
		var fdescCatalogue = ReadSource("Design Documents", "Seeding", "FutureMUD_Medieval_Jewellery_FDesc_Catalogue.csv");

		var designReferences = Regex.Matches(
				designReference,
				@"^\| `(?<ref>medieval_jewellery_[^`]+)` \|",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var itemCatalogueReferences = Regex.Matches(
				itemCatalogue,
				@"(?:^|,)(?<ref>medieval_jewellery_[^,\r\n]+),",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var fdescReferences = Regex.Matches(
				fdescCatalogue,
				@"^(?<ref>medieval_jewellery_[^,\r\n]+),",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var sourceReferences = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalJewellery.cs").Where(item => item.Metadata.StableKey.StartsWith("medieval_jewellery_", StringComparison.Ordinal)).Select(item => item.Metadata.StableKey).ToArray();

		Assert.AreEqual(400, designReferences.Length,
			"The medieval jewellery design reference should contain the full 400-item catalogue.");
		CollectionAssert.AreEqual(designReferences, itemCatalogueReferences,
			"The full item catalogue should stay in the same order as the design reference.");
		CollectionAssert.AreEqual(designReferences, fdescReferences,
			"The fdesc catalogue should stay in the same order as the design reference.");
		CollectionAssert.AreEqual(designReferences, sourceReferences,
			"SeedMedievalJewelleryAndDevotionalGoods should contain exactly one explicit catalogue definition for each jewellery reference.");
		Assert.AreEqual(sourceReferences.Length, sourceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval jewellery item should be created exactly once.");
		Assert.IsTrue(CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalJewellery.cs").Any(item => item.Components.Contains("SealStamp_Medieval_NobleSignetRing")));
		Assert.IsTrue(CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalJewellery.cs").Any(item => item.MorphSeconds == 21600));
		Assert.IsTrue(CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalJewellery.cs").Any(item => item.MorphSeconds == 86400));

	}

	[TestMethod]
	public void MedievalTreatmentAndRepairSeeder_ImplementsReferenceCatalogueWithExplicitCatalogueDefinitions()
	{
		var medicalSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalMedical.cs");
		var repairSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalRepairKits.cs");
		var designReference = ReadSource("Design Documents", "Seeding", "FutureMUD_Medieval_Treatment_Drugs_Repair_Kits_Design_Reference.md");

		var designReferences = Regex.Matches(
				designReference,
				@"^\| `(?<ref>medieval_(?:medical|drug|repair)_[^`]+)` \|",
				RegexOptions.Multiline | RegexOptions.CultureInvariant)
			.Cast<Match>()
			.Select(x => x.Groups["ref"].Value)
			.ToArray();
		var medicalReferences = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalMedical.cs").Select(item => item.Metadata.StableKey).ToArray();
		var repairReferences = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalRepairKits.cs").Select(item => item.Metadata.StableKey).ToArray();
		var sourceReferences = medicalReferences
			.Concat(repairReferences)
			.ToArray();

		Assert.AreEqual(183, designReferences.Length,
			"The treatment/drug/repair design reference should contain the full 183-item catalogue.");
		Assert.AreEqual(150, medicalReferences.Length,
			"SeedMedievalMedicalAndApothecaryItems should contain the 150 explicit treatment, drug, mobility, and prosthetic item calls.");
		Assert.AreEqual(33, repairReferences.Length,
			"SeedMedievalRepairKits should contain the 33 direct repair-kit and repair-supply item calls.");
		CollectionAssert.AreEqual(designReferences, sourceReferences,
			"The medical and repair partials should follow the merged design reference order with exactly one explicit catalogue definition per row.");
		Assert.AreEqual(sourceReferences.Length, sourceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval treatment, drug, mobility, prosthetic, or repair item should be created exactly once.");

	}

	[TestMethod]
	public void MedievalMilitarySeeder_ImplementsFullCatalogueWithExplicitCatalogueDefinitions()
	{
		var armourSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalArmour.cs");
		var weaponsSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalWeapons.cs");
		var armourReferences = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalArmour.cs").Select(item => item.Metadata.StableKey).ToArray();
		var weaponReferences = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalWeapons.cs").Select(item => item.Metadata.StableKey).ToArray();

		Assert.AreEqual(143, weaponReferences.Length,
			"SeedMedievalWeaponsShieldsAccessories should contain exactly one explicit catalogue definition for each melee, ranged, ammunition, and thrown catalogue row.");
		Assert.AreEqual(239, armourReferences.Length,
			"SeedMedievalArmour should contain exactly one explicit catalogue definition for each armour, shield, and military carrying, storage, and support gear row.");
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
				"medieval_military_short_throwing_spear",
				"medieval_military_blackjack_leather_wrapped"
			},
			new[]
			{
				weaponReferences.First(),
				weaponReferences[76],
				weaponReferences[77],
				weaponReferences[^2],
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

	}

	[TestMethod]
	public void MedievalClothingSeeder_HasExactReferenceCatalogueWithAuthoritativeReuse()
	{
		var clothingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder", "ItemSeeder.MedievalClothing.cs");
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
		var sourceReferences = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalClothing.cs").Select(item => item.Metadata.StableKey).Concat(Regex.Matches(clothingSource, @"CreateHistoricalClothingItem\(FindHistoricalClothingSource\(""(?<ref>[^""]+)""\)")
			.Cast<Match>().Select(match => match.Groups["ref"].Value)).ToArray();

		Assert.AreEqual(408, designReferences.Length, "The design reference should contain the full 408-garment catalogue.");
		CollectionAssert.AreEqual(designReferences, csvReferences,
			"The fdesc catalogue should stay in the same order as the design reference.");
		CollectionAssert.AreEquivalent(designReferences, sourceReferences,
			"SeedMedievalClothing must create each reference once, directly or through its authoritative shared source.");
		Assert.AreEqual(sourceReferences.Length, sourceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval clothing item should be created exactly once.");
		var reused = Regex.Matches(clothingSource, @"CreateHistoricalClothingItem\(FindHistoricalClothingSource\(""(?<ref>[^""]+)""\)")
			.Cast<Match>().Select(x => x.Groups["ref"].Value).ToArray();
		Assert.AreEqual(33, reused.Length);
		foreach (var reference in reused)
		{
			var source = ItemSeeder.FindHistoricalClothingSource(reference);
			Assert.IsNotNull(source, reference);
			Assert.AreEqual("medieval", source.OwningModule, reference);
			Assert.IsNull(source.LegacyAliasReference, reference);
		}

	}

	[TestMethod]
	public void MedievalFurnitureSeeder_ImplementsReferenceCatalogueWithExplicitCatalogueDefinitions()
	{
		var householdSource = string.Join(Environment.NewLine,
			new[]
			{
				"ItemSeeder.MedievalContainers.cs",
				"ItemSeeder.MedievalDoorsLocksStrongboxes.cs",
				"ItemSeeder.MedievalFood.cs",
				"ItemSeeder.MedievalFurniture.cs",
				"ItemSeeder.MedievalJewellery.cs"
			}
			.Select(x => ReadSource("DatabaseSeeder", "Seeders", x)));
		var sourceReferences = new[] { "ItemSeeder.MedievalContainers.cs", "ItemSeeder.MedievalDoorsLocksStrongboxes.cs", "ItemSeeder.MedievalFood.cs", "ItemSeeder.MedievalFurniture.cs", "ItemSeeder.MedievalJewellery.cs" }.SelectMany(CompiledCatalogueFixtures.FromSource).Select(item => item.Metadata.StableKey).Where(key => !key.StartsWith("medieval_jewellery_", StringComparison.Ordinal)).ToArray();

		Assert.AreEqual(1751, sourceReferences.Length,
			"The medieval household goods split should contain exactly one explicit catalogue definition for each source catalogue row.");
		Assert.AreEqual(sourceReferences.Length, sourceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval household item should be created exactly once.");

	}

	[TestMethod]
	public void MedievalWritingSeeder_ImplementsReferenceCatalogueWithExplicitCatalogueDefinitions()
	{
		var writingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalWriting.cs");
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
		var sourceReferences = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalWriting.cs").Select(item => item.Metadata.StableKey).ToArray();

		Assert.AreEqual(286, designReferences.Length,
			"The writing design reference should contain the full 286-item catalogue.");
		CollectionAssert.AreEqual(designReferences, csvReferences,
			"The writing fdesc catalogue should stay in the same order as the design reference.");
		CollectionAssert.AreEqual(designReferences, sourceReferences,
			"SeedMedievalWritingAdministrationAndDocuments should contain exactly one explicit catalogue definition for each writing reference.");
		Assert.AreEqual(sourceReferences.Length, sourceReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Each medieval writing, book, document, seal, container, scribal-tool, or writing-support item should be created exactly once.");

	}

	[TestMethod]
	public void MedievalCraftLaunchers_AllCurrentCatalogueFamiliesAreImplemented()
	{
		var medievalCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.Medieval.cs");
		var medievalFoodCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.MedievalFood.cs");
		Assert.IsTrue(
			Regex.IsMatch(
				medievalCraftSource,
				@"private\s+void\s+SeedMedievalProductionChainCrafts\s*\(\s*\)\s*\{(?<body>.*?)\n\t\}",
				RegexOptions.Singleline | RegexOptions.CultureInvariant) &&
			medievalCraftSource.Contains("MedievalProductionCraftSpecs().OrderBy(x => x.Phase)", StringComparison.Ordinal),
			"SeedMedievalProductionChainCrafts should admit the phase-ordered production catalogue.");
		Assert.IsTrue(
			medievalFoodCraftSource.Contains("MedievalFoodCraftSpecs().OrderBy(x => x.Phase)", StringComparison.Ordinal),
			"SeedMedievalFoodBeverageCrafts should admit the phase-ordered food catalogue.");

		foreach (var method in MedievalCraftLaunchers.Where(x =>
			         !x.Equals("SeedMedievalProductionChainCrafts", StringComparison.Ordinal) &&
			         !x.Equals("SeedMedievalFoodBeverageCrafts", StringComparison.Ordinal)))
		{
			Assert.IsTrue(
				Regex.IsMatch(
					medievalCraftSource,
					$@"private\s+void\s+{method}\s*\(\s*\)\s*\{{(?<body>.*?)\n\t\}}",
					RegexOptions.Singleline | RegexOptions.CultureInvariant) &&
				medievalCraftSource.Contains("SeedMedievalFinishedItemCrafts(", StringComparison.Ordinal),
				$"{method} should route its documented family through the medieval finished-item craft catalogue.");
		}

		foreach (var forbidden in new[]
		{
			"AddMedievalCraft",
			"BuildMedieval",
			"MedievalCultureProfile",
			"MedievalExplicitOutfit",
			"MedievalAuthoredOutfit"
		})
		{
			Assert.IsFalse(medievalCraftSource.Contains(forbidden, StringComparison.Ordinal),
				$"Medieval crafting should not retain retired source token {forbidden}.");
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
			"ItemSeeder.Medieval.cs",
			"ItemSeeder.MedievalAuthoredOutfitPieces.cs",
			"ItemSeeder.MedievalSupport.cs"
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
				"Medieval_Jewellery_Seeder_Dependency_Request.md",
				"Medieval_Jewellery_Seeder_Design_Reference.md",
				"Medieval_Military_Seeder_Design_Reference.md",
				"Medieval_Food_Beverage_Preservation_Foundation.md"
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
		AssertContains(indexSource, "[Medieval Jewellery Seeder Design Reference](./Seeding/Medieval_Jewellery_Seeder_Design_Reference.md)");
		AssertContains(indexSource, "[Medieval Military Seeder Design Reference](./Seeding/Medieval_Military_Seeder_Design_Reference.md)");
		AssertContains(indexSource, "[Medieval Writing, Books, and Documents Seeder Design Reference](./Seeding/FutureMUD_Medieval_Writing_Books_Documents_Design_Reference.md)");
		AssertContains(indexSource, "[Medieval Treatment Items, Drugs, and Repair Kits Design Reference](./Seeding/FutureMUD_Medieval_Treatment_Drugs_Repair_Kits_Design_Reference.md)");
		foreach (var removed in RetiredMedievalDesignDocuments)
		{
			Assert.IsFalse(indexSource.Contains(removed, StringComparison.Ordinal),
				$"Design document index should not link retired medieval document {removed}.");
		}

		var auditSource = ReadSource("Design Documents", "Seeding", "Medieval_Crafting_Audit.md");
		AssertContains(auditSource, "`SeedMedievalClothing` contains the direct clothing item `CreateItem(...)` calls.");
		AssertContains(auditSource, "`SeedMedievalHouseholdFurniture` contains the direct furniture");
		AssertContains(auditSource, "Medieval_Clothing_Seeder_Design_Reference.md");
		AssertContains(auditSource, "Medieval_Clothing_FDesc_Catalogue.csv");
		AssertContains(auditSource, "Medieval_Household_Goods_Furniture_Seeder_Design_Reference.md");
		AssertContains(auditSource, "ItemSeeder.MedievalFurniture.cs");
		AssertContains(auditSource, "`SeedMedievalWeaponsShieldsAccessories` contains the direct melee weapon, ranged weapon, ammunition, and thrown-weapon `CreateItem(...)` calls.");
		AssertContains(auditSource, "`SeedMedievalArmour` contains the direct armour, horse tack, barding, shield, and military support-gear `CreateItem(...)` calls.");
		AssertContains(auditSource, "`SeedMedievalWritingAdministrationAndDocuments` contains the direct writing-surface, book, document, seal, container, scribal-tool, and writing-support `CreateItem(...)` calls.");
		AssertContains(auditSource, "`SeedMedievalMedicalAndApothecaryItems` contains the direct treatment, apothecary, drug-delivery, mobility, casualty-transport, and prosthetic `CreateItem(...)` calls.");
		AssertContains(auditSource, "`SeedMedievalJewelleryAndDevotionalGoods` contains the direct decorative jewellery");
		AssertContains(auditSource, "`SeedMedievalRepairKits` contains the direct repair-kit and repair-supply `CreateItem(...)` calls.");
		AssertContains(auditSource, "Medieval_Military_Seeder_Design_Reference.md");
		AssertContains(auditSource, "ItemSeeder.MedievalWeapons.cs");
		AssertContains(auditSource, "ItemSeeder.MedievalArmour.cs");
		AssertContains(auditSource, "FutureMUD_Medieval_Writing_Books_Documents_Design_Reference.md");
		AssertContains(auditSource, "FutureMUD_Medieval_Writing_Books_Documents_FDesc_Catalogue.csv");
		AssertContains(auditSource, "ItemSeeder.MedievalWriting.cs");
		AssertContains(auditSource, "FutureMUD_Medieval_Treatment_Drugs_Repair_Kits_Design_Reference.md");
		AssertContains(auditSource, "ItemSeeder.MedievalMedical.cs");
		AssertContains(auditSource, "ItemSeeder.MedievalRepairKits.cs");
		AssertContains(auditSource, "Medieval_Jewellery_Seeder_Design_Reference.md");
		AssertContains(auditSource, "FutureMUD_Medieval_Jewellery_Item_Catalogue_Full.csv");
		AssertContains(auditSource, "FutureMUD_Medieval_Jewellery_FDesc_Catalogue.csv");
		AssertContains(auditSource, "ItemSeeder.HistoricFoundation.cs");
		AssertContains(auditSource, "ItemSeeder.Crafting.HistoricFoundation.cs");
		Assert.IsFalse(auditSource.Contains("Medieval_Outfit_Catalogue.md", StringComparison.Ordinal),
			"The medieval audit should not point readers to retired outfit catalogue payloads.");
		Assert.IsFalse(auditSource.Contains("Medieval_Culture_Catalogue.md", StringComparison.Ordinal),
			"The medieval audit should not point readers to retired culture catalogue payloads.");
	}

	[TestMethod]
	public void MedievalMilitaryItems_PreserveDocumentedPhysicalBindingsAndStableReferences()
	{
		var armour = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalArmour.cs");
		var weapons = CompiledCatalogueFixtures.FromSource("ItemSeeder.MedievalWeapons.cs");
		foreach (var forbidden in new[] { "Armour_HeavyClothing", "Armour_UltraHeavyClothing", "Armour_Platemail" })
			Assert.IsFalse(armour.Any(item => item.Components.Contains(forbidden)), forbidden);
		Assert.AreEqual(14, armour.Count(item => item.Components.Contains("Armour_Padded")));
		Assert.AreEqual(20, armour.Count(item => item.Components.Contains("Armour_RigidMetal")));
		Assert.AreEqual(1, armour.Count(item => item.Components.Contains("Armour_CoatOfPlates")));
		Assert.AreEqual(4, armour.Count(item => item.Components.Contains("Armour_Splinted")));
		foreach (var (key, component) in new[]
		{
			("medieval_military_light_riding_spear", "Melee_Lance"),
			("medieval_military_broad_poleblade", "Melee_Poleblade"),
			("medieval_military_hooked_polearm", "Melee_HookedPolearm"),
			("medieval_military_recurved_riders_shortbow", "CompositeBow_Light"),
			("medieval_military_fine_horn_recurve_bow", "CompositeBow_War"),
			("medieval_military_wootz_war_sword", "Melee_Sabre")
		}) Assert.IsTrue(weapons.Single(item => item.Metadata.StableKey == key).Components.Contains(component), key);
		Assert.AreEqual(MudSharp.GameItems.SizeCategory.Normal,
			weapons.Single(item => item.Metadata.StableKey == "medieval_military_plain_short_spear").Size);
		Assert.AreEqual(382, armour.Length + weapons.Length);
	}

	private static string ReadMedievalItemSources(params string[] excludedFileNames)
	{
		var excluded = excludedFileNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
		return string.Join(Environment.NewLine,
			Directory.GetFiles(SourcePath("DatabaseSeeder", "Seeders", "ItemSeeder"), "ItemSeeder.Medieval*.cs")
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

	private static void AssertItemUses(string source, string stableReference, string component)
	{
		Match match = Regex.Match(source,
			$"\"{Regex.Escape(stableReference)}\"(?<body>.{{0,1500}}?)\\);",
			RegexOptions.Singleline | RegexOptions.CultureInvariant);
		Assert.IsTrue(match.Success, $"Expected to find item {stableReference}.");
		StringAssert.Contains(match.Groups["body"].Value, $"\"{component}\"");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(SourcePath(parts));
	}

	private static string SourcePath(params string[] parts)
	{
		var direct = Path.Combine(new[] { SourceRoot() }.Concat(parts).ToArray());
		if (File.Exists(direct) || Directory.Exists(direct) ||
		    parts.Length < 3 || !parts[^2].Equals("Seeders", StringComparison.OrdinalIgnoreCase) ||
		    !parts[^1].StartsWith("ItemSeeder.", StringComparison.OrdinalIgnoreCase))
		{
			return direct;
		}

		return Path.Combine(new[] { SourceRoot() }.Concat(parts[..^1]).Append("ItemSeeder").Append(parts[^1]).ToArray());
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
