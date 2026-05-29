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
	private static readonly string[] ExplicitMedievalCultureCatalogueGroups =
	[
		"Clothing",
		"Military",
		"Food and Beverage",
		"Writing and Administration",
		"Household and Devotional"
	];

	private static readonly string[] MedievalOutfitRequiredSlots =
	[
		"underlayer",
		"lower_body",
		"leg_or_sock_layer",
		"footwear",
		"bodywear",
		"outerwear",
		"headwear",
		"belt_or_sash",
		"worn_container",
		"fastener_or_jewellery"
	];

	private static readonly string[] MedievalOutfitRoleItemRequiredRoles =
	[
		"merchant",
		"religious",
		"military"
	];

	private static readonly string[] MedievalOutfitCriticalWearSlots =
	[
		"footwear",
		"headwear",
		"bodywear",
		"belt_or_sash"
	];

	[TestMethod]
	public void MedievalDispatcher_WiresItemAndCraftSuites()
	{
		var reworkRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var medievalItemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Medieval.cs");
		var medievalCraftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");

		foreach (var expected in new[]
		{
			"SeedHistoricCommonWorkshopItems();",
			"SeedMedievalClothing();",
			"SeedMedievalHouseholdCraftTools();",
			"SeedMedievalWritingAdministrationAndDocuments();",
			"SeedMedievalMedicalAndApothecaryItems();",
			"SeedMedievalJewelleryAndDevotionalGoods();",
			"SeedMedievalArmour();",
			"SeedMedievalContainers();",
			"SeedMedievalDoorsLocksAndStrongboxes();",
			"SeedMedievalRepairKits();",
			"SeedMedievalHouseholdFurniture();",
			"SeedMedievalWeaponsShieldsAccessories();",
			"SeedMedievalFoodAndBeverageItems();",
			"SeedMedievalComponentGapItems();"
		})
		{
			AssertContains(reworkRoot, expected);
		}

		foreach (var expected in new[]
		{
			"SeedHistoricFoundationCrafts();",
			"SeedMedievalProductionChainCrafts();",
			"SeedMedievalClothingCrafts();",
			"SeedMedievalEquipmentCrafts();",
			"SeedMedievalWritingAdministrationCrafts();",
			"SeedMedievalMedicalApothecaryCrafts();",
			"SeedMedievalJewelleryDevotionalCrafts();",
			"SeedMedievalFurnitureAndContainerCrafts();",
			"SeedMedievalFoodBeverageCrafts();",
			"SeedMedievalRepairKitCrafts();",
			"SeedMedievalComponentGapCrafts();"
		})
		{
			AssertContains(craftRoot, expected);
		}

		AssertContains(medievalItemSource, "private static readonly MedievalCultureProfile[] MedievalCultureProfiles");
		AssertContains(medievalItemSource, "private static readonly MedievalStatusRoleProfile[] MedievalStatusRoleProfiles");
		AssertContains(medievalCraftSource, "private bool ShouldSeedMedievalCrafts()");
		AssertContains(medievalCraftSource, "private Craft? AddMedievalCraft(");
	}

	[TestMethod]
	public void MedievalExplicitCultureCatalogue_CoversEveryCultureAndSurface()
	{
		var groupedReferences = ItemSeeder.MedievalExplicitCultureStableReferenceGroupsForTesting;

		foreach (var culture in ItemSeeder.MedievalCultureKeysForTesting)
		{
			Assert.IsTrue(groupedReferences.TryGetValue(culture, out var cultureGroups),
				$"Expected explicit medieval culture catalogue entries for {culture}.");
			Assert.IsTrue(cultureGroups.Values.SelectMany(x => x).Any(),
				$"Expected {culture} to have explicit non-generic catalogue entries.");

			foreach (var group in ExplicitMedievalCultureCatalogueGroups)
			{
				Assert.IsTrue(cultureGroups.TryGetValue(group, out var stableReferences),
					$"Expected {culture} to include the explicit catalogue group {group}.");
				Assert.IsTrue(stableReferences.Count > 0,
					$"Expected {culture} / {group} to include at least one explicit stable reference.");
			}
		}

		Assert.AreEqual(ItemSeeder.MedievalCultureKeysForTesting.Count, groupedReferences.Count,
			"Expected the explicit medieval catalogue to have exactly one entry set per culture key.");
	}

	[TestMethod]
	public void MedievalExplicitCultureCatalogue_DoesNotUseRegionalPlaceholderDescriptions()
	{
		var placeholders = ItemSeeder.MedievalExplicitCultureCatalogueEntriesForTesting
			.Where(x => x.ShortDescription.StartsWith("a regional", StringComparison.OrdinalIgnoreCase))
			.Select(x => $"{x.CultureKey}/{x.Group}/{x.StableReference}: {x.ShortDescription}")
			.ToList();

		Assert.AreEqual(0, placeholders.Count,
			$"Explicit medieval culture catalogue entries must not use regional placeholder short descriptions: {string.Join(", ", placeholders)}");
	}

	[TestMethod]
	public void MedievalExplicitCultureCatalogue_ExactStableReferencesMatchAuthoritativeDocument()
	{
		var documentEntries = ReadExplicitMedievalCultureCatalogueEntriesFromDocs();
		var codeEntries = ItemSeeder.MedievalExplicitCultureCatalogueEntriesForTesting
			.Select(x => (x.CultureKey, x.Group, x.StableReference))
			.ToList();
		var codeKeys = codeEntries
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}")
			.ToHashSet(StringComparer.Ordinal);
		var documentKeys = documentEntries
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}")
			.ToHashSet(StringComparer.Ordinal);

		var missingFromCode = documentKeys
			.Where(x => !codeKeys.Contains(x))
			.ToList();
		var missingFromDocument = codeKeys
			.Where(x => !documentKeys.Contains(x))
			.ToList();

		Assert.AreEqual(0, missingFromCode.Count,
			$"Expected exact medieval culture catalogue references from Medieval_Culture_Catalogue.md to be present in code. Missing: {string.Join(", ", missingFromCode)}");
		Assert.AreEqual(0, missingFromDocument.Count,
			$"Expected explicit code catalogue references to be documented exactly. Missing: {string.Join(", ", missingFromDocument)}");
	}

	[TestMethod]
	public void MedievalOutfits_CoverEveryCultureSexAndSocialRole()
	{
		var outfits = ItemSeeder.MedievalOutfitsForTesting;

		Assert.AreEqual(18, ItemSeeder.MedievalCultureKeysForTesting.Count);
		Assert.AreEqual(2, ItemSeeder.MedievalOutfitSexGenderPresentationKeysForTesting.Count);
		Assert.AreEqual(6, ItemSeeder.MedievalOutfitSocialClassRoleKeysForTesting.Count);
		Assert.AreEqual(216, outfits.Count);

		foreach (var culture in ItemSeeder.MedievalCultureKeysForTesting)
		{
			var cultureOutfits = outfits
				.Where(x => x.CultureKey.Equals(culture, StringComparison.OrdinalIgnoreCase))
				.ToList();
			Assert.AreEqual(12, cultureOutfits.Count, $"Expected 12 outfits for {culture}.");

			foreach (var sex in ItemSeeder.MedievalOutfitSexGenderPresentationKeysForTesting)
			{
				foreach (var role in ItemSeeder.MedievalOutfitSocialClassRoleKeysForTesting)
				{
					var expected = $"medieval_outfit_{culture}_{sex}_{role}";
					Assert.IsTrue(cultureOutfits.Any(x =>
							x.OutfitReference.Equals(expected, StringComparison.Ordinal) &&
							x.SexGenderPresentation.Equals(sex, StringComparison.Ordinal) &&
							x.SocialClassRole.Equals(role, StringComparison.Ordinal)),
						$"Expected outfit {expected}.");
				}
			}
		}
	}

	[TestMethod]
	public void MedievalOutfits_HaveRequiredSlotsAndResolvableStableReferences()
	{
		var itemReferences = ItemSeeder.MedievalItemStableReferencesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var slotDefinitions = ItemSeeder.MedievalOutfitSlotsForTesting
			.ToDictionary(x => x.Key, x => x, StringComparer.OrdinalIgnoreCase);

		foreach (var slot in MedievalOutfitRequiredSlots)
		{
			Assert.IsTrue(slotDefinitions.TryGetValue(slot, out var definition),
				$"Expected medieval outfit slot definition for {slot}.");
			Assert.IsTrue(definition.RequiredForAllOutfits, $"Expected {slot} to be required for all outfits.");
		}

		foreach (var role in MedievalOutfitRoleItemRequiredRoles)
		{
			Assert.IsTrue(slotDefinitions["role_item"].RequiredForRoles.Contains(role),
				$"Expected role_item to be required for {role} outfits.");
		}

		foreach (var outfit in ItemSeeder.MedievalOutfitsForTesting)
		{
			foreach (var slot in MedievalOutfitRequiredSlots)
			{
				Assert.IsTrue(outfit.SlotItemStableReferences.TryGetValue(slot, out var stableReference),
					$"Expected {outfit.OutfitReference} to include {slot}.");
				Assert.IsTrue(itemReferences.Contains(stableReference),
					$"Expected {outfit.OutfitReference} / {slot} reference {stableReference} to resolve to a medieval item spec.");
			}

			if (MedievalOutfitRoleItemRequiredRoles.Contains(outfit.SocialClassRole))
			{
				Assert.IsTrue(outfit.SlotItemStableReferences.TryGetValue("role_item", out var roleItem),
					$"Expected {outfit.OutfitReference} to include a role item.");
				Assert.IsTrue(itemReferences.Contains(roleItem),
					$"Expected {outfit.OutfitReference} role item {roleItem} to resolve to a medieval item spec.");
			}

			foreach (var sharedSlot in outfit.IntentionallySharedOrGenericSlots)
			{
				Assert.IsTrue(outfit.SlotItemStableReferences.ContainsKey(sharedSlot),
					$"Expected {outfit.OutfitReference} shared slot {sharedSlot} to be present in the slot map.");
			}
		}
	}

	[TestMethod]
	public void MedievalOutfits_AreDocumentedByExactOutfitReference()
	{
		var documentedOutfits = ReadMedievalOutfitReferencesFromDocs()
			.ToHashSet(StringComparer.Ordinal);
		var codeOutfits = ItemSeeder.MedievalOutfitsForTesting
			.Select(x => x.OutfitReference)
			.ToHashSet(StringComparer.Ordinal);
		var missingFromCode = documentedOutfits
			.Where(x => !codeOutfits.Contains(x))
			.ToList();
		var missingFromDocs = codeOutfits
			.Where(x => !documentedOutfits.Contains(x))
			.ToList();

		Assert.AreEqual(216, documentedOutfits.Count);
		Assert.AreEqual(0, missingFromCode.Count,
			$"Expected documented medieval outfit references to exist in code: {string.Join(", ", missingFromCode)}");
		Assert.AreEqual(0, missingFromDocs.Count,
			$"Expected code medieval outfit references to be documented exactly: {string.Join(", ", missingFromDocs)}");
	}

	[TestMethod]
	public void MedievalExplicitOutfitPieceCraftNames_DoNotUseRegionalPattern()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");
		var helperBlock = ExtractMethodBlockContaining(craftSource, "MedievalExplicitOutfitPieceCraftName");

		AssertContains(helperBlock, "spec.ShortDescription");
		Assert.IsFalse(helperBlock.Contains("BuildRegionalCraftName", StringComparison.Ordinal),
			"Explicit outfit-piece craft names should use exact object names, not regional patterns.");
		Assert.IsFalse(helperBlock.Contains("regional pattern", StringComparison.OrdinalIgnoreCase),
			"Explicit outfit-piece craft names should not contain regional pattern.");
	}

	[TestMethod]
	public void MedievalNorthAtlanticOutfits_AreExplicitCompleteAndVariantDistinct()
	{
		AssertExplicitOutfitClusterComplete(ItemSeeder.MedievalNorthAtlanticOutfitCultureKeysForTesting, 6, 72);
	}

	[TestMethod]
	public void MedievalContinentalWesternOutfits_AreExplicitCompleteAndVariantDistinct()
	{
		AssertExplicitOutfitClusterComplete(ItemSeeder.MedievalContinentalWesternOutfitCultureKeysForTesting, 4, 48);
	}

	[TestMethod]
	public void MedievalNorthAtlanticOutfits_ContainRequiredCultureVocabulary()
	{
		var piecesByCulture = ItemSeeder.MedievalExplicitOutfitPiecesForTesting
			.GroupBy(x => x.CultureKey, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => string.Join(" ", x.Select(piece => piece.PieceName)).ToLowerInvariant(),
				StringComparer.OrdinalIgnoreCase);
		var required = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["early_anglo_saxon"] = ["tablet-banded", "cloak brooch", "linen head veil", "seax belt"],
			["anglo_danish"] = ["long seax", "shield-wall", "panelled", "reeve"],
			["norse"] = ["hangerok", "oval brooch", "sea cloak", "runic", "leg wraps"],
			["norman"] = ["split riding tunic", "bliaut", "mail surcoat", "nasal"],
			["high_british"] = ["cote", "surcoat", "coif", "wimple", "arming"],
			["gaelic"] = ["brat", "ring pin", "long shirt", "bardic", "pastoral"]
		};

		foreach (var culture in ItemSeeder.MedievalNorthAtlanticOutfitCultureKeysForTesting)
		{
			Assert.IsTrue(piecesByCulture.TryGetValue(culture, out var sourceText),
				$"Expected explicit outfit vocabulary source for {culture}.");
			foreach (var expected in required[culture])
			{
				Assert.IsTrue(sourceText.Contains(expected, StringComparison.OrdinalIgnoreCase),
					$"Expected {culture} outfit vocabulary to include {expected}.");
			}
		}
	}

	[TestMethod]
	public void MedievalContinentalWesternOutfits_ContainRequiredCultureVocabulary()
	{
		var piecesByCulture = ItemSeeder.MedievalExplicitOutfitPiecesForTesting
			.GroupBy(x => x.CultureKey, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => string.Join(" ", x.Select(piece => piece.PieceName)).ToLowerInvariant(),
				StringComparer.OrdinalIgnoreCase);
		var required = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["carolingian"] = ["high-belted tunic", "broad-banded mantle", "spatha belt", "capitulary", "monastic"],
			["capetian"] = ["cote", "bliaut", "burgher gown", "wimple", "guild apron"],
			["german_hre"] = ["guild apron", "civic gown", "alpine felt cap", "fur-lined mantle", "town crossbow", "militia"],
			["iberian_christian"] = ["saya", "pellote", "manto", "toca", "frontier riding cloak"]
		};

		foreach (var culture in ItemSeeder.MedievalContinentalWesternOutfitCultureKeysForTesting)
		{
			Assert.IsTrue(piecesByCulture.TryGetValue(culture, out var sourceText),
				$"Expected explicit outfit vocabulary source for {culture}.");
			foreach (var expected in required[culture])
			{
				Assert.IsTrue(sourceText.Contains(expected, StringComparison.OrdinalIgnoreCase),
					$"Expected {culture} outfit vocabulary to include {expected}.");
			}
		}
	}

	[TestMethod]
	public void MedievalExplicitOutfitPieceCraftNames_UseNamedObjectsAndTextileStocks()
	{
		var craftNames = ItemSeeder.MedievalExplicitOutfitPieceCraftsForTesting;
		var craftByReference = craftNames
			.ToDictionary(x => x.StableReference, x => x, StringComparer.OrdinalIgnoreCase);

		foreach (var piece in ItemSeeder.MedievalExplicitOutfitPiecesForTesting
			         .DistinctBy(x => x.StableReference))
		{
			Assert.IsTrue(craftByReference.TryGetValue(piece.StableReference, out var craft),
				$"Expected explicit outfit craft for {piece.StableReference}.");
			Assert.IsFalse(craft.CraftName.Contains("regional pattern", StringComparison.OrdinalIgnoreCase),
				$"Explicit outfit piece craft {craft.CraftName} should not use regional pattern naming.");
			Assert.IsTrue(craft.CraftName.Contains(piece.PieceName, StringComparison.OrdinalIgnoreCase),
				$"Explicit outfit piece craft {craft.CraftName} should include named object {piece.PieceName}.");
		}

		Assert.IsTrue(craftNames.Any(x => x.CraftName.Contains("sew a Norse hangerok apron dress", StringComparison.Ordinal)),
			"Expected the Norse hangerok craft name to include the named object.");

		var allInputs = craftNames
			.SelectMany(x => x.Inputs)
			.ToList();
		foreach (var expected in new[]
		         {
			         "Garment Cloth",
			         "Broadcloth Stock",
			         "Embroidered Trim Stock",
			         "Tablet-Woven Band Stock",
			         "Quilted Armour Padding",
			         "Turnshoe Upper Stock",
			         "Spun Yarn"
		         })
		{
			Assert.IsTrue(allInputs.Any(x => x.Contains(expected, StringComparison.OrdinalIgnoreCase)),
				$"Expected explicit outfit piece crafts to use {expected} where appropriate.");
		}

		foreach (var stableReference in new[]
		         {
			         "medieval_outfit_piece_carolingian_male_military_riding_spurs",
			         "medieval_outfit_piece_german_hre_male_noble_belt_mounts",
			         "medieval_outfit_piece_german_hre_male_military_town_crossbow_militia_hook"
		         })
		{
			Assert.IsTrue(craftByReference.TryGetValue(stableReference, out var craft),
				$"Expected explicit outfit craft for reviewed hardware piece {stableReference}.");
			Assert.IsTrue(craft.Inputs.Any(x => x.Contains("Tool Blank Stock", StringComparison.OrdinalIgnoreCase)),
				$"Expected reviewed hardware piece {stableReference} to use metal tool stock rather than textile stock.");
			Assert.IsFalse(craft.Inputs.Any(x =>
					x.Contains("Garment Cloth", StringComparison.OrdinalIgnoreCase) ||
					x.Contains("Broadcloth Stock", StringComparison.OrdinalIgnoreCase) ||
					x.Contains("Spun Yarn", StringComparison.OrdinalIgnoreCase)),
				$"Reviewed hardware piece {stableReference} should not be generated as a sewn textile craft.");
		}
	}

	[TestMethod]
	public void MedievalCultureStatusAndStableReferenceFamilies_AreDocumented()
	{
		var docs = ReadMedievalDocs();
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Medieval.cs");

		Assert.AreEqual(18, ItemSeeder.MedievalCultureKeysForTesting.Count);
		Assert.AreEqual(6, ItemSeeder.MedievalStatusRoleKeysForTesting.Count);
		Assert.AreEqual(10, ItemSeeder.MedievalWardrobeSlotKeysForTesting.Count);

		foreach (var culture in ItemSeeder.MedievalCultureKeysForTesting)
		{
			AssertContains(docs, culture);
			AssertContains(itemSource, culture);
			AssertContains(docs, $"medieval_clothing_{{culture}}_");
			AssertContains(docs, $"medieval_food_{{culture}}_{{foodway_item}}");
			AssertContains(docs, $"medieval_writing_{{culture}}_{{administration_item}}");
			AssertContains(docs, $"medieval_military_{{culture}}_{{equipment_piece}}");
		}

		foreach (var status in ItemSeeder.MedievalStatusRoleKeysForTesting)
		{
			AssertContains(docs, status);
		}

		foreach (var slot in ItemSeeder.MedievalWardrobeSlotKeysForTesting)
		{
			AssertContains(itemSource, slot);
		}

		foreach (var expected in new[]
		{
			"medieval_clothing_norman_noble_fur_lined_mantle",
			"medieval_clothing_norse_peasant_wool_leggings",
			"medieval_clothing_song_china_merchant_lined_hat",
			"medieval_clothing_abbasid_clergy_plain_sandals",
			"medieval_clothing_steppe_turkic_military_riding_boots",
			"medieval_clothing_high_british_artisan_tool_belt",
			"medieval_clothing_fatimid_noble_alms_purse"
		})
		{
			Assert.IsTrue(ItemSeeder.MedievalItemStableReferencesForTesting.Contains(expected, StringComparer.OrdinalIgnoreCase),
				$"Expected generated medieval clothing reference {expected}.");
		}

		Assert.IsTrue(ItemSeeder.MedievalItemStableReferencesForTesting.Count(x => x.StartsWith("medieval_military_", StringComparison.OrdinalIgnoreCase)) >= 18 * 7,
			"Expected every medieval culture to have armour plus equipment accessories.");
		Assert.IsTrue(ItemSeeder.MedievalItemStableReferencesForTesting.Count(x => x.StartsWith("medieval_food_", StringComparison.OrdinalIgnoreCase)) >= 130,
			"Expected food stock to include culture foodways plus common production stock.");
		Assert.IsTrue(ItemSeeder.MedievalItemStableReferencesForTesting.Count(x => x.StartsWith("medieval_household_", StringComparison.OrdinalIgnoreCase)) >= 25,
			"Expected household stock to cover furniture, storage, lighting, and security props.");
		Assert.IsTrue(ItemSeeder.MedievalItemStableReferencesForTesting.Count(x => x.StartsWith("medieval_writing_", StringComparison.OrdinalIgnoreCase)) >= 90,
			"Expected writing stock to include culture administration plus common office stock.");

		foreach (var expected in new[]
		{
			"medieval_military_norman_field_pack",
			"medieval_military_song_china_war_banner",
			"medieval_military_abbasid_padded_coif",
			"medieval_food_song_china_drinking_vessel",
			"medieval_food_norse_preserved_provision",
			"medieval_food_ale_cask",
			"medieval_household_writing_desk",
			"medieval_household_market_stall",
			"medieval_household_iron_lantern",
			"medieval_devotional_byzantine_pilgrim_token",
			"medieval_jewellery_court_circlet",
			"medieval_medical_monastic_infirmary_kit",
			"medieval_medical_bone_saw",
			"medieval_writing_song_china_record_tablet",
			"medieval_writing_notary_kit",
			"medieval_writing_bound_codex",
			"medieval_writing_ledger_chest"
		})
		{
			Assert.IsTrue(ItemSeeder.MedievalItemStableReferencesForTesting.Contains(expected, StringComparer.OrdinalIgnoreCase),
				$"Expected expanded medieval coverage reference {expected}.");
			Assert.IsTrue(IsDocumentedStableReference(expected, docs),
				$"Expected expanded medieval coverage reference {expected} to be documented.");
		}

		foreach (var expected in new[]
		{
			"Wear_Hat",
			"Wear_Cloak_(Closed)",
			"Wear_Mantle",
			"Wear_Chausses",
			"Wear_Mittens",
			"Wear_Fingerless_Gloves",
			"Wear_Socks",
			"Wear_Shoes",
			"Wear_Boots",
			"Wear_Sandals",
			"Wear_Waist",
			"Beltable"
		})
		{
			AssertContains(itemSource, expected);
		}

		var undocumented = ItemSeeder.MedievalItemStableReferencesForTesting
			.Where(x => !IsDocumentedStableReference(x, docs))
			.ToList();
		Assert.AreEqual(0, undocumented.Count,
			$"Expected every medieval stable reference to be documented exactly or by an explicit family pattern. Missing: {string.Join(", ", undocumented)}");

		foreach (var stableReference in ItemSeeder.HistoricFoundationStableReferencesForTesting)
		{
			AssertContains(docs, stableReference);
		}
	}

	[TestMethod]
	public void MedievalImplementedItems_UseLiveComponentsAndDocumentDeferredGaps()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Medieval.cs");
		var componentSource = ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.ItemComponents.cs");
		var docs = ReadMedievalDocs();

		foreach (var path in new[]
		{
			Path.Combine("FutureMUDLibrary", "GameItems", "Interfaces", "ISealStamp.cs"),
			Path.Combine("FutureMUDLibrary", "GameItems", "Interfaces", "ISealable.cs"),
			Path.Combine("FutureMUDLibrary", "GameItems", "Interfaces", "IMeasuringInstrument.cs"),
			Path.Combine("FutureMUDLibrary", "GameItems", "Interfaces", "IOfferingReceiver.cs")
		})
		{
			Assert.IsTrue(File.Exists(SourcePath(path)), $"Expected live interface file {path}");
		}

		foreach (var component in new[]
		{
			"SealStamp_Antiquity_BronzeSignet",
			"Sealable_Document_Wax",
			"Sealable_Envelope",
			"Sealable_Container_Wax",
			"MeasuringInstrument_Antiquity_BalanceScale",
			"MeasuringInstrument_Antiquity_StandardWeights",
			"MeasuringInstrument_Antiquity_FalseWeights",
			"MeasuringInstrument_Antiquity_GrainMeasure",
			"MeasuringInstrument_Antiquity_OilCup",
			"MeasuringInstrument_Antiquity_WineCup",
			"MeasuringInstrument_Antiquity_TaxAssessorKit",
			"OfferingReceiver_Antiquity_VotiveBasin"
		})
		{
			AssertContains(componentSource, component);
			AssertContains(itemSource, component);
		}

		foreach (var stableReference in ItemSeeder.MedievalLiveComponentStableReferencesForTesting)
		{
			Assert.IsTrue(IsDocumentedStableReference(stableReference, docs),
				$"Expected live component item {stableReference} to be documented.");
		}

		foreach (var stableReference in ItemSeeder.MedievalDeferredComponentGapStableReferencesForTesting)
		{
			AssertContains(docs, stableReference);
			var block = ExtractCallBlockContaining(itemSource, stableReference);
			Assert.IsFalse(block.Contains("SealStamp_", StringComparison.Ordinal), $"{stableReference} should not use seal components.");
			Assert.IsFalse(block.Contains("Sealable_", StringComparison.Ordinal), $"{stableReference} should not use sealable components.");
			Assert.IsFalse(block.Contains("MeasuringInstrument_", StringComparison.Ordinal), $"{stableReference} should not use measurement components.");
		}
	}

	[TestMethod]
	public void MedievalExpandedNonClothingSuites_HaveCraftCoverage()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");
		var docs = ReadMedievalDocs();

		foreach (var expected in new[]
		{
			"medieval_military_{culture.Key}_padded_coif",
			"medieval_military_{culture.Key}_sidearm_harness",
			"medieval_military_{culture.Key}_arrow_quiver",
			"medieval_military_{culture.Key}_field_pack",
			"medieval_military_{culture.Key}_war_banner",
			"medieval_food_{culture.Key}_staple_bread",
			"medieval_food_{culture.Key}_pottage_bowl",
			"medieval_food_{culture.Key}_preserved_provision",
			"medieval_food_{culture.Key}_drinking_vessel",
			"medieval_food_{culture.Key}_feast_dish",
			"medieval_food_{culture.Key}_market_ration",
			"medieval_writing_{culture.Key}_record_tablet",
			"medieval_writing_{culture.Key}_tally_bundle",
			"medieval_writing_{culture.Key}_seal_tag_packet"
		})
		{
			AssertContains(craftSource, expected);
		}

		foreach (var expected in new[]
		{
			"MedievalFoodAndBeverageItemSpecs()",
			"MedievalFurnitureContainerItemSpecs()",
			"MedievalJewelleryDevotionalItemSpecs()",
			"MedievalMedicalApothecaryItemSpecs()",
			"MedievalWritingAdministrationItemSpecs()",
			"AddMedievalGeneralSpecCraft(spec",
			"MedievalSpecCraftName(\"make\", spec)",
			"MedievalSpecCraftName(\"prepare\", spec)"
		})
		{
			AssertContains(craftSource, expected);
		}

		foreach (var expected in new[]
		{
			"Builder Workflows",
			"Food and beverage",
			"Furniture and containers",
			"Jewellery/devotional",
			"Medical/apothecary",
			"Writing/administration"
		})
		{
			AssertContains(docs, expected);
		}
	}

	[TestMethod]
	public void MedievalCrafting_TagToolsHaveSeededHistoricCoverage()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Medieval.cs");

		var toolTags = Regex.Matches(craftSource,
				@"TagTool\s*-\s*(?:Held|InRoom|In Room|Wielded)\s*-\s*an item with the (?<tag>.+?) tag",
				RegexOptions.IgnoreCase)
			.Select(x => x.Groups["tag"].Value.Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		var seededToolTagLeafs = Regex.Matches(itemSource, @"""(?<tag>Functions / [^""]+)""")
			.Select(x => x.Groups["tag"].Value.Split(" / ").Last().Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var missing = toolTags
			.Where(x => !seededToolTagLeafs.Contains(x))
			.ToList();

		Assert.AreEqual(0, missing.Count,
			$"Every medieval TagTool leaf should have at least one seeded historic prototype. Missing: {string.Join(", ", missing)}");
	}

	[TestMethod]
	public void MedievalProductionChains_RepresentOriginalProcessChanges()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Medieval.cs");
		var docs = ReadMedievalDocs();

		foreach (var expected in new[]
		{
			"SeedMedievalProductionChainCrafts()",
			"spin linen yarn stock",
			"weave linen garment cloth stock",
			"prepare leather panel stock",
			"forge iron tool blank stock",
			"shape weapon shaft stock",
			"forge weapon blade stock",
			"forge weapon head stock",
			"cut fletching stock",
			"mix pottery clay body stock",
			"prepare glass batch stock",
			"full broadcloth stock",
			"prepare embroidered trim stock",
			"weave tablet band stock",
			"quilt armour padding stock",
			"cut turnshoe upper stock",
			"draw mail wire stock",
			"assemble mail panel stock",
			"forge crossbow prod stock",
			"carve crossbow tiller stock",
			"beat rag paper pulp stock",
			"compound sealing wax stock",
			"prepare brewing mash stock",
			"mix glaze slurry stock",
			"lead stained glass panel stock",
			"cast standard weight blank stock"
		})
		{
			AssertContains(craftSource, expected);
		}

		foreach (var expected in new[]
		{
			"medieval_household_fulling_stocks",
			"medieval_household_embroidery_frame",
			"medieval_household_turnshoe_last",
			"medieval_household_drawplate",
			"medieval_household_mail_riveting_tongs",
			"medieval_household_crossbow_tiller_jig",
			"medieval_household_papermakers_mould",
			"medieval_household_cheese_press",
			"medieval_household_lauter_tun",
			"medieval_household_glaziers_grozing_iron",
			"medieval_household_lantern_pane_mould",
			"medieval_weapon_common_crossbow",
			"medieval_weapon_common_crossbow_bolts",
			"medieval_household_stained_glass_panel",
			"medieval_household_roof_tile_stack"
		})
		{
			AssertContains(itemSource, expected);
			Assert.IsTrue(ItemSeeder.MedievalItemStableReferencesForTesting.Contains(expected, StringComparer.OrdinalIgnoreCase),
				$"Expected production-chain item {expected} to be in the medieval item catalogue.");
			Assert.IsTrue(IsDocumentedStableReference(expected, docs),
				$"Expected production-chain item {expected} to be documented.");
		}

		var boltBlock = ExtractCallBlockContaining(itemSource, "medieval_weapon_common_crossbow_bolts");
		AssertContains(boltBlock, "Ammo_BroadheadBolt");
		AssertContains(boltBlock, "Stack_Number");
		Assert.IsFalse(boltBlock.Contains("Container_Quiver", StringComparison.Ordinal),
			"Crossbow bolts should be single stackable ammo, not a quiver/container bundle.");

		foreach (var tag in new[]
		{
			"Spun Yarn",
			"Garment Cloth",
			"Fulled Cloth",
			"Prepared Leather Panel",
			"Leather Strap",
			"Furniture Timber Stock",
			"Furniture Panel Stock",
			"Pottery Clay Body",
			"Glass Batch",
			"Glass Vessel Blank",
			"Tool Blank Stock",
			"Weapon Shaft Stock",
			"Weapon Blade Stock",
			"Weapon Head Stock",
			"Fletching Stock",
			"Military Cord Stock",
			"Shield Board Stock",
			"Shield Facing Stock",
			"Armour Lamella Stock",
			"Flour Commodity"
		})
		{
			AssertCommodityOutputTag(craftSource, tag);
		}

		foreach (var tag in new[]
		{
			"Broadcloth Stock",
			"Embroidered Trim Stock",
			"Tablet-Woven Band Stock",
			"Quilted Armour Padding",
			"Silk Brocade Panel",
			"Turnshoe Upper Stock",
			"Scabbard Leather Stock",
			"Bookbinding Leather Stock",
			"Coopered Staves",
			"Hoop Stock",
			"Mail Wire Stock",
			"Armour Ring Stock",
			"Mail Panel Stock",
			"Crossbow Prod Stock",
			"Crossbow Tiller Stock",
			"Crossbow Lockwork Stock",
			"Paper Pulp Stock",
			"Paper Sheet Stock",
			"Parchment Sheet Stock",
			"Seal Cord Stock",
			"Sealing Wax Stock",
			"Cheese Curd Stock",
			"Brewing Mash Stock",
			"Ale Stock",
			"Cider Stock",
			"Mead Stock",
			"Glaze Slurry Stock",
			"Tile Blank Stock",
			"Stained Glass Quarry Stock",
			"Lead Came Stock",
			"Stained Glass Panel Stock",
			"Lantern Pane Stock",
			"Standard Weight Blank",
			"Sealable Bale Wrapper Stock",
			"Tally Stick Stock",
			"Lockwork Stock"
		})
		{
			AssertContains(craftSource, tag);
		}

		foreach (var expected in new[]
		{
			"Cheese Wheel Stock",
			"crossbow manufacture",
			"paper and parchment",
			"stained glass",
			"guild weights and measures",
			"luxury textile finishes"
		})
		{
			AssertContains(docs, expected);
		}
	}

	[TestMethod]
	public void HistoricLitFoundationItems_HaveMorphTargetsTimersAndCrafts()
	{
		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Medieval.cs");
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");

		foreach (var spec in new[]
		{
			(Lit: "historic_lit_workshop_hearth", Unlit: "historic_workshop_hearth", Timer: "TimeSpan.FromHours(2)"),
			(Lit: "historic_lit_updraft_kiln", Unlit: "historic_updraft_kiln", Timer: "TimeSpan.FromHours(4)"),
			(Lit: "historic_lit_oil_lamp", Unlit: "historic_oil_lamp", Timer: "TimeSpan.FromHours(3)")
		})
		{
			var block = ExtractCallBlockContaining(itemSource, spec.Lit);
			AssertContains(block, spec.Unlit);
			AssertContains(block, spec.Timer);
		}

		AssertContains(craftSource, "specs.Where(x => !string.IsNullOrWhiteSpace(x.MorphToUniqueReference))");
		AssertContains(craftSource, "StableSimpleItemInput(spec.MorphToUniqueReference!)");
		AssertContains(craftSource, "StableSimpleProduct(spec.StableReference)");
	}

	private static bool IsDocumentedStableReference(string stableReference, string docs)
	{
		if (docs.Contains(stableReference, StringComparison.Ordinal))
		{
			return true;
		}

		return stableReference switch
		{
			_ when stableReference.StartsWith("medieval_outfit_piece_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_outfit_piece_{culture}_{sex}_{class}_{piece}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_clothing_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_clothing_{culture}_{status}_{piece}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_food_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_food_{culture}_{foodway_item}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_writing_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_writing_{culture}_{administration_item}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_military_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_military_{culture}_{equipment_piece}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_weapon_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_weapon_{culture}_{weapon}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_shield_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_shield_{culture}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_devotional_", StringComparison.OrdinalIgnoreCase) &&
			       stableReference.EndsWith("_pilgrim_token", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_devotional_{culture}_pilgrim_token", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_household_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_household_{furniture_or_container}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_medical_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_medical_{medical_item}", StringComparison.Ordinal),
			_ when stableReference.StartsWith("medieval_jewellery_", StringComparison.OrdinalIgnoreCase) =>
				docs.Contains("medieval_jewellery_{jewellery_item}", StringComparison.Ordinal),
			_ => false
		};
	}

	private static string ReadMedievalDocs()
	{
		return string.Concat(new[]
		{
			"Crafting_System_Builder_Workflows.md",
			"Medieval_Culture_Catalogue.md",
			"Medieval_Outfit_Catalogue.md",
			"Medieval_Crafting_Audit.md",
			"Medieval_Item_Component_Gap_Report.md",
			"Medieval_Clothing_Crafting_Suite.md",
			"Medieval_Equipment_Crafting_Suite.md",
			"Medieval_Food_Beverage_Crafting_Suite.md",
			"Medieval_Furniture_Container_Crafting_Suite.md",
			"Medieval_Jewellery_Devotional_Crafting_Suite.md",
			"Medieval_Medical_Apothecary_Crafting_Suite.md",
			"Medieval_Writing_Administration_Crafting_Suite.md"
		}.Select(x => ReadSource("Design Documents", "Crafting", x)));
	}

	private static IReadOnlyCollection<(string CultureKey, string Group, string StableReference)> ReadExplicitMedievalCultureCatalogueEntriesFromDocs()
	{
		var docs = ReadSource("Design Documents", "Crafting", "Medieval_Culture_Catalogue.md");
		var entries = new List<(string CultureKey, string Group, string StableReference)>();
		string? cultureKey = null;
		string? group = null;
		var inExactCatalogue = false;

		foreach (var line in Regex.Split(docs, "\r?\n"))
		{
			if (line.Equals("## Exact Culture Catalogue", StringComparison.Ordinal))
			{
				inExactCatalogue = true;
				cultureKey = null;
				group = null;
				continue;
			}

			if (!inExactCatalogue)
			{
				continue;
			}

			var cultureMatch = Regex.Match(line, @"^### .+ \(`(?<culture>[^`]+)`\)");
			if (cultureMatch.Success)
			{
				cultureKey = cultureMatch.Groups["culture"].Value;
				group = null;
				continue;
			}

			var groupMatch = Regex.Match(line, @"^#### (?<group>.+)$");
			if (groupMatch.Success)
			{
				group = groupMatch.Groups["group"].Value;
				continue;
			}

			var stableReferenceMatch = Regex.Match(line, @"^- `(?<stableReference>[^`]+)`");
			if (!stableReferenceMatch.Success)
			{
				continue;
			}

			Assert.IsFalse(string.IsNullOrWhiteSpace(cultureKey),
				$"Stable reference {stableReferenceMatch.Groups["stableReference"].Value} is outside a culture section.");
			Assert.IsTrue(ExplicitMedievalCultureCatalogueGroups.Contains(group ?? string.Empty),
				$"Stable reference {stableReferenceMatch.Groups["stableReference"].Value} is under unexpected group {group}.");
			entries.Add((cultureKey!, group!, stableReferenceMatch.Groups["stableReference"].Value));
		}

		return entries;
	}

	private static IReadOnlyCollection<string> ReadMedievalOutfitReferencesFromDocs()
	{
		var docs = ReadSource("Design Documents", "Crafting", "Medieval_Outfit_Catalogue.md");
		return Regex.Matches(docs, @"`(?<outfit>medieval_outfit_[^`]+)`")
			.Select(x => x.Groups["outfit"].Value)
			.Distinct(StringComparer.Ordinal)
			.ToArray();
	}

	private static string ExtractCallBlockContaining(string source, string marker)
	{
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find source block for {marker}.");
		var end = source.IndexOf(");", start, StringComparison.Ordinal);
		Assert.IsTrue(end >= 0, $"Could not find end of source block for {marker}.");
		return source[start..end];
	}

	private static string ExtractMethodBlockContaining(string source, string marker)
	{
		var start = source.IndexOf(marker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find source block for {marker}.");
		var openBrace = source.IndexOf('{', start);
		Assert.IsTrue(openBrace >= 0, $"Could not find opening brace for source block {marker}.");
		var depth = 0;
		for (var i = openBrace; i < source.Length; i++)
		{
			if (source[i] == '{')
			{
				depth++;
				continue;
			}

			if (source[i] != '}')
			{
				continue;
			}

			depth--;
			if (depth == 0)
			{
				return source[start..(i + 1)];
			}
		}

		Assert.Fail($"Could not find end of source block for {marker}.");
		return string.Empty;
	}

	private static void AssertExplicitOutfitClusterComplete(IReadOnlyCollection<string> cultures,
		int expectedCultureCount, int expectedOutfitCount)
	{
		var outfits = ItemSeeder.MedievalOutfitsForTesting
			.Where(x => cultures.Contains(x.CultureKey, StringComparer.OrdinalIgnoreCase))
			.ToList();
		var piecesByOutfit = ItemSeeder.MedievalExplicitOutfitPiecesForTesting
			.GroupBy(x => x.OutfitReference, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);

		Assert.AreEqual(expectedCultureCount, cultures.Count);
		Assert.AreEqual(expectedOutfitCount, outfits.Count);

		foreach (var culture in cultures)
		{
			var cultureOutfits = outfits
				.Where(x => x.CultureKey.Equals(culture, StringComparison.OrdinalIgnoreCase))
				.ToList();
			Assert.AreEqual(12, cultureOutfits.Count, $"Expected 12 explicit outfits for {culture}.");

			foreach (var outfit in cultureOutfits)
			{
				Assert.AreEqual(0, outfit.IntentionallySharedOrGenericSlots.Count,
					$"{outfit.OutfitReference} should be implemented with explicit outfit pieces, not generic slot markers.");
				Assert.IsTrue(piecesByOutfit.TryGetValue(outfit.OutfitReference, out var outfitPieces),
					$"Expected explicit outfit pieces for {outfit.OutfitReference}.");

				foreach (var slot in MedievalOutfitCriticalWearSlots)
				{
					Assert.IsTrue(outfit.SlotItemStableReferences.TryGetValue(slot, out var stableReference),
						$"{outfit.OutfitReference} should include {slot}.");
					Assert.IsTrue(stableReference.StartsWith($"medieval_outfit_piece_{culture}_", StringComparison.OrdinalIgnoreCase),
						$"{outfit.OutfitReference} / {slot} should use an explicit {culture} outfit piece.");
					if (slot.Equals("footwear", StringComparison.OrdinalIgnoreCase))
					{
						var footwearPiece = outfitPieces.Single(x =>
							x.StableReference.Equals(stableReference, StringComparison.OrdinalIgnoreCase));
						Assert.IsTrue(IsMedievalOutfitFootwearPieceName(footwearPiece.PieceName),
							$"{outfit.OutfitReference} footwear slot should resolve to footwear, not {footwearPiece.PieceName}.");
					}
				}

				Assert.IsTrue(outfit.SlotItemStableReferences.Values.Distinct(StringComparer.OrdinalIgnoreCase).Count() >= 9,
					$"{outfit.OutfitReference} should have at least nine wearable pieces or documented slot equivalents.");
				Assert.IsTrue(outfit.SlotItemStableReferences.Values
						.Distinct(StringComparer.OrdinalIgnoreCase)
						.Count(x => x.StartsWith($"medieval_outfit_piece_{culture}_", StringComparison.OrdinalIgnoreCase)) >= 4,
					$"{outfit.OutfitReference} should include at least four culture-specific or culture-cluster-specific items.");
				Assert.IsTrue(outfitPieces.All(x => outfit.SlotItemStableReferences.Values.Contains(x.StableReference, StringComparer.OrdinalIgnoreCase)),
					$"Every explicit piece for {outfit.OutfitReference} should be referenced by its slot map.");
			}

			foreach (var role in ItemSeeder.MedievalOutfitSocialClassRoleKeysForTesting)
			{
				var male = cultureOutfits.Single(x =>
					x.SexGenderPresentation.Equals("male", StringComparison.OrdinalIgnoreCase) &&
					x.SocialClassRole.Equals(role, StringComparison.OrdinalIgnoreCase));
				var female = cultureOutfits.Single(x =>
					x.SexGenderPresentation.Equals("female", StringComparison.OrdinalIgnoreCase) &&
					x.SocialClassRole.Equals(role, StringComparison.OrdinalIgnoreCase));
				Assert.IsTrue(CountDifferentOutfitSlots(male.SlotItemStableReferences, female.SlotItemStableReferences) >= 2,
					$"{culture} {role} male/female variants should differ in at least two wearable slots.");
			}

			foreach (var sex in ItemSeeder.MedievalOutfitSexGenderPresentationKeysForTesting)
			{
				var sexOutfits = cultureOutfits
					.Where(x => x.SexGenderPresentation.Equals(sex, StringComparison.OrdinalIgnoreCase))
					.ToList();
				foreach (var left in sexOutfits)
				{
					foreach (var right in sexOutfits.Where(x => string.CompareOrdinal(x.SocialClassRole, left.SocialClassRole) > 0))
					{
						Assert.IsTrue(CountDifferentOutfitSlots(left.SlotItemStableReferences, right.SlotItemStableReferences) >= 2,
							$"{culture} {sex} {left.SocialClassRole}/{right.SocialClassRole} class variants should differ in at least two wearable slots.");
					}
				}
			}
		}
	}

	private static int CountDifferentOutfitSlots(IReadOnlyDictionary<string, string> left,
		IReadOnlyDictionary<string, string> right)
	{
		return left.Keys
			.Intersect(right.Keys, StringComparer.OrdinalIgnoreCase)
			.Count(slot => !left[slot].Equals(right[slot], StringComparison.OrdinalIgnoreCase));
	}

	private static bool IsMedievalOutfitFootwearPieceName(string pieceName)
	{
		return pieceName.Contains("shoe", StringComparison.OrdinalIgnoreCase) ||
		       pieceName.Contains("boot", StringComparison.OrdinalIgnoreCase) ||
		       pieceName.Contains("sandal", StringComparison.OrdinalIgnoreCase) ||
		       pieceName.Contains("slipper", StringComparison.OrdinalIgnoreCase);
	}

	private static void AssertContains(string source, string expected)
	{
		Assert.IsTrue(source.Contains(expected, StringComparison.Ordinal), $"Expected source to contain: {expected}");
	}

	private static void AssertCommodityOutputTag(string source, string tag)
	{
		Assert.IsTrue(Regex.IsMatch(source,
				$@"CommodityOutput\([^)]*,\s*""[^""]+"",\s*""{Regex.Escape(tag)}""",
				RegexOptions.None),
			$"Expected medieval production chains to produce commodity tag: {tag}");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(SourcePath(Path.Combine(parts)));
	}

	private static string SourcePath(string path)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			path));
	}
}
