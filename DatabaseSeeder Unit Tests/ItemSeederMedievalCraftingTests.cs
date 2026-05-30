#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Form.Material;
using System;
using System.Collections.Generic;
using System.Globalization;
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

	private static readonly string[] MedievalOutfitForbiddenCultureNames =
	[
		"Early Anglo-Saxon",
		"Anglo-Danish",
		"Norse",
		"Norman",
		"High British",
		"Gaelic",
		"Carolingian",
		"Capetian",
		"German-HRE",
		"Iberian Christian",
		"Andalusi",
		"Byzantine",
		"Abbasid",
		"Fatimid",
		"Seljuk-Ayyubid",
		"Rus-Novgorod",
		"Steppe Turkic",
		"Song Chinese"
	];

	[TestMethod]
	public void MedievalDispatcher_WiresItemAndCraftSuites()
	{
		var reworkRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.cs");
		var craftRoot = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.cs");
		var medievalItemSource = ReadMedievalItemSources();
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
	public void MedievalItemDefinitions_UseSharedEraSpecSeederForClothingCatalogue()
	{
		var clothingSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.MedievalClothing.cs");
		var eraSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.EraDefinitions.cs");
		var itemSource = ReadMedievalItemSources();
		var stableReferences = ItemSeeder.HistoricFoundationItemSpecsForTesting
			.Concat(ItemSeeder.MedievalItemSpecsForTesting)
			.Select(x => x.StableReference)
			.ToArray();

		AssertContains(clothingSource, "SeedEraItemSpecs(MedievalClothingItemSpecs())");
		AssertContains(eraSource, "private void SeedEraItemSpecs(IEnumerable<EraItemSpec> specs)");
		Assert.IsFalse(itemSource.Contains("SeedMedievalItemSpecs(", StringComparison.Ordinal),
			"Medieval clothing should use the shared era spec seeder rather than a separate generated item-spec helper.");

		var duplicates = stableReferences
			.GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.ToArray();
		Assert.AreEqual(0, duplicates.Length,
			$"Expected every historic/medieval item definition to have one stable reference. Duplicates: {string.Join(", ", duplicates)}");
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
	public void MedievalExplicitCultureCatalogue_EveryExactReferenceIsClassified()
	{
		var entries = ItemSeeder.MedievalExplicitCultureCatalogueEntriesForTesting;
		var unclassified = entries
			.Where(x => !Enum.IsDefined(typeof(ItemSeeder.MedievalCultureCatalogueReferenceStatus), x.Status))
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}")
			.ToList();
		var aliasesWithoutImplementation = entries
			.Where(x => x.Status == ItemSeeder.MedievalCultureCatalogueReferenceStatus.AliasOfExistingStableReference &&
			            string.IsNullOrWhiteSpace(x.ImplementationStableReference))
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}")
			.ToList();
		var outfitCoveredWithoutImplementation = entries
			.Where(x => x.Status == ItemSeeder.MedievalCultureCatalogueReferenceStatus.CoveredByOutfitPiece &&
			            string.IsNullOrWhiteSpace(x.ImplementationStableReference))
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}")
			.ToList();

		Assert.AreEqual(0, unclassified.Count,
			$"Expected every exact medieval culture catalogue reference to have a known status. Missing: {string.Join(", ", unclassified)}");
		Assert.AreEqual(0, aliasesWithoutImplementation.Count,
			$"AliasOfExistingStableReference entries must record the actual implementation reference. Missing: {string.Join(", ", aliasesWithoutImplementation)}");
		Assert.AreEqual(0, outfitCoveredWithoutImplementation.Count,
			$"CoveredByOutfitPiece entries must record at least one outfit-piece implementation reference. Missing: {string.Join(", ", outfitCoveredWithoutImplementation)}");
	}

	[TestMethod]
	public void MedievalExplicitCultureCatalogue_ImplementedItemsResolveToItemSpecs()
	{
		var itemReferences = ItemSeeder.MedievalItemStableReferencesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var unresolved = ItemSeeder.MedievalExplicitCultureCatalogueEntriesForTesting
			.Where(x => x.Status == ItemSeeder.MedievalCultureCatalogueReferenceStatus.ImplementedItem)
			.Where(x => !itemReferences.Contains(x.StableReference))
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}")
			.ToList();

		Assert.AreEqual(0, unresolved.Count,
			$"ImplementedItem catalogue references must resolve to item specs. Missing: {string.Join(", ", unresolved)}");
	}

	[TestMethod]
	public void MedievalExplicitCultureCatalogue_ImplementedItemsHaveCraftCoverageOrExemption()
	{
		var craftedReferences = ItemSeeder.MedievalCraftedItemStableReferencesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var allowedExemptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Stock-only",
			"Morph target",
			"Deferred"
		};
		var invalidExemptions = ItemSeeder.MedievalExplicitCultureCatalogueEntriesForTesting
			.Where(x => x.Status == ItemSeeder.MedievalCultureCatalogueReferenceStatus.ImplementedItem)
			.Where(x => !string.IsNullOrWhiteSpace(x.CraftCoverageExemption) &&
			            !allowedExemptions.Contains(x.CraftCoverageExemption!))
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}: {x.CraftCoverageExemption}")
			.ToList();
		var missingCrafts = ItemSeeder.MedievalExplicitCultureCatalogueEntriesForTesting
			.Where(x => x.Status == ItemSeeder.MedievalCultureCatalogueReferenceStatus.ImplementedItem)
			.Where(x => string.IsNullOrWhiteSpace(x.CraftCoverageExemption))
			.Where(x => !craftedReferences.Contains(x.StableReference))
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}")
			.ToList();

		Assert.AreEqual(0, invalidExemptions.Count,
			$"Craft coverage exemptions must be explicit stock-only, morph target, or deferred markers. Invalid: {string.Join(", ", invalidExemptions)}");
		Assert.AreEqual(0, missingCrafts.Count,
			$"ImplementedItem catalogue references must have craft products or documented exemptions. Missing craft coverage: {string.Join(", ", missingCrafts)}");
	}

	[TestMethod]
	public void MedievalExplicitCultureCatalogue_DeferredEntriesHaveReasonStrings()
	{
		var deferredWithoutReasons = ItemSeeder.MedievalExplicitCultureCatalogueEntriesForTesting
			.Where(x => x.Status == ItemSeeder.MedievalCultureCatalogueReferenceStatus.Deferred)
			.Where(x => string.IsNullOrWhiteSpace(x.Reason))
			.Select(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}")
			.ToList();

		Assert.AreEqual(0, deferredWithoutReasons.Count,
			$"Deferred medieval culture catalogue references must document a reason. Missing: {string.Join(", ", deferredWithoutReasons)}");
	}

	[TestMethod]
	public void MedievalExplicitCultureCatalogue_DocumentShowsImplementationStatusForEveryEntry()
	{
		var documentEntries = ReadExplicitMedievalCultureCatalogueEntryStatusesFromDocs();
		var documentStatuses = documentEntries
			.ToDictionary(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}", x => x.Status, StringComparer.Ordinal);
		var codeStatuses = ItemSeeder.MedievalExplicitCultureCatalogueEntriesForTesting
			.ToDictionary(x => $"{x.CultureKey}|{x.Group}|{x.StableReference}", x => x.Status.ToString(), StringComparer.Ordinal);
		var missingStatus = codeStatuses
			.Keys
			.Where(x => !documentStatuses.ContainsKey(x))
			.ToList();
		var mismatchedStatus = codeStatuses
			.Where(x => documentStatuses.TryGetValue(x.Key, out var status) && !status.Equals(x.Value, StringComparison.Ordinal))
			.Select(x => $"{x.Key}: doc={documentStatuses[x.Key]}, code={x.Value}")
			.ToList();

		Assert.AreEqual(0, missingStatus.Count,
			$"Expected every exact medieval catalogue document entry to show a status. Missing: {string.Join(", ", missingStatus)}");
		Assert.AreEqual(0, mismatchedStatus.Count,
			$"Documented medieval catalogue statuses must match code classifications. Mismatched: {string.Join(", ", mismatchedStatus)}");
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
	public void MedievalAuthoredOutfitPieces_ResolveAndReplaceGeneratedDescriptions()
	{
		var authoredPieces = ItemSeeder.MedievalAuthoredOutfitPiecesForTesting;
		var specsByReference = ItemSeeder.MedievalExplicitOutfitPieceItemSpecsForTesting
			.ToDictionary(x => x.StableReference, x => x, StringComparer.OrdinalIgnoreCase);
		var itemReferences = ItemSeeder.MedievalItemStableReferencesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var expectedCount = ItemSeeder.MedievalExplicitOutfitPiecesForTesting
			.Select(x => x.StableReference)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Count();

		Assert.AreEqual(expectedCount, authoredPieces.Count,
			"Expected every explicit outfit-piece item to have one final authored catalogue entry.");

		foreach (var authoredRow in authoredPieces)
		{
			Assert.IsTrue(itemReferences.Contains(authoredRow.StableReference),
				$"Expected authored stable reference {authoredRow.StableReference} to resolve to a medieval item spec.");
			Assert.IsTrue(specsByReference.TryGetValue(authoredRow.StableReference, out var spec),
				$"Expected authored stable reference {authoredRow.StableReference} to resolve to an explicit outfit-piece item spec.");

			Assert.AreEqual(authoredRow.Noun, spec.Noun);
			Assert.AreEqual(authoredRow.ShortDescription, spec.ShortDescription);
			Assert.AreEqual(authoredRow.FullDescription, spec.FullDescription);
			Assert.AreEqual(authoredRow.Material, spec.Material);
			Assert.AreEqual(authoredRow.MaterialType, spec.MaterialType);
			Assert.AreEqual(authoredRow.Quality, spec.Quality);
			Assert.AreEqual(authoredRow.Size, spec.Size);
			Assert.AreEqual(authoredRow.WeightInGrams, spec.WeightInGrams);
			Assert.AreEqual(authoredRow.Cost, spec.Cost);
			AssertContains(spec.BuilderNotes ?? string.Empty, "authored catalogue entry");
			AssertContains(spec.BuilderNotes ?? string.Empty, $"Outfit reference: {authoredRow.OutfitReference}.");
			AssertContains(spec.BuilderNotes ?? string.Empty, $"Culture key: {authoredRow.CultureKey}.");
			Assert.IsTrue(authoredRow.SlotKeys.Any(),
				$"{authoredRow.StableReference} should expose outfit slot usage through the authored catalogue test seam.");

			var oldOwnershipPhrase = "belongs" + " to the";
			var oldSlotPhrase = "fills" + " the";
			var oldCataloguePhrase = "outfit slot" + " for the explicit medieval outfit catalogue";
			Assert.IsFalse(spec.FullDescription.Contains(oldOwnershipPhrase, StringComparison.OrdinalIgnoreCase),
				$"{authoredRow.StableReference} should not retain generated outfit ownership prose.");
			Assert.IsFalse(spec.FullDescription.Contains(oldSlotPhrase, StringComparison.OrdinalIgnoreCase),
				$"{authoredRow.StableReference} should not retain generated slot-filling prose.");
			Assert.IsFalse(spec.FullDescription.Contains(oldCataloguePhrase,
					StringComparison.OrdinalIgnoreCase),
				$"{authoredRow.StableReference} should not retain generated explicit-catalogue prose.");
		}
	}

	[TestMethod]
	public void MedievalAuthoredOutfitPieces_AreSingleSourceSeederData()
	{
		var projectSource = ReadSource("DatabaseSeeder", "DatabaseSeeder.csproj");
		var itemSource = ReadMedievalItemSources();
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");
		var oldDescriptionCsvResource = "MED_OUTFIT_006_description_" + "overrides_REVISED_colour_style.csv";
		var oldGeneratedSeederMethod = "SeedMedieval" + "ExplicitOutfitPieces";

		Assert.IsFalse(projectSource.Contains(oldDescriptionCsvResource, StringComparison.OrdinalIgnoreCase),
			"Authored outfit-piece rows should live in seeder code, not as an embedded CSV resource.");
		Assert.IsFalse(itemSource.Contains("GetManifestResourceStream", StringComparison.Ordinal),
			"Authored outfit-piece rows should not be read from an embedded resource.");
		var oldRecordName = "MedievalExplicitOutfitPiece" + "Override";
		var oldApplyName = "Apply" + oldRecordName;
		Assert.IsFalse(itemSource.Contains("Parse" + oldRecordName + "Csv", StringComparison.Ordinal),
			"Authored outfit-piece rows should not depend on a runtime CSV parser.");
		Assert.IsFalse(itemSource.Contains(oldRecordName, StringComparison.Ordinal),
			"The old generated-then-overridden outfit-piece record should not exist.");
		Assert.IsFalse(itemSource.Contains(oldApplyName, StringComparison.Ordinal),
			"The old outfit-piece patch application helpers should not exist.");
		Assert.IsFalse(itemSource.Contains(oldGeneratedSeederMethod, StringComparison.Ordinal),
			"Explicit outfit pieces should seed from the same authored clothing spec catalogue as common clothing.");
		AssertContains(itemSource, "SeedEraItemSpecs(MedievalClothingItemSpecs())");
		AssertContains(itemSource, "MedievalExplicitOutfitClothingPieces");
		AssertContains(itemSource, "new EraClothingPieceSpec");
		AssertContains(itemSource, "BuildMedievalExplicitOutfitPieceItemSpec");
		AssertContains(craftSource, "foreach (var piece in MedievalExplicitOutfitClothingPieces())");
		AssertContains(craftSource, "var craftSpec = piece.Craft");
		AssertContains(craftSource, "BuildMedievalExplicitOutfitPieceProductSpec");
		AssertContains(craftSource, "craftSpec.Products.Select(x => x.BuildDefinition(item))");
	}

	[TestMethod]
	public void MedievalAuthoredOutfitPieces_CoverEveryCultureAndUseNoLazyCultureNames()
	{
		var authoredPieces = ItemSeeder.MedievalAuthoredOutfitPiecesForTesting;
		var rowsByCulture = authoredPieces
			.GroupBy(x => x.CultureKey, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);

		Assert.AreEqual(18, rowsByCulture.Count);
		foreach (var culture in ItemSeeder.MedievalExplicitOutfitCultureKeysForTesting)
		{
			Assert.IsTrue(rowsByCulture.TryGetValue(culture, out var cultureRows),
				$"Expected authored outfit-piece rows for {culture}.");
			Assert.IsTrue(cultureRows.Count >= 100, $"Expected complete authored outfit-piece coverage for {culture}.");
		}

		foreach (var authoredRow in authoredPieces)
		{
			foreach (var forbiddenCultureName in MedievalOutfitForbiddenCultureNames)
			{
				Assert.IsFalse(authoredRow.ShortDescription.Contains(forbiddenCultureName,
						StringComparison.OrdinalIgnoreCase),
					$"{authoredRow.StableReference} short description should not contain visible culture name {forbiddenCultureName}.");
				Assert.IsFalse(authoredRow.FullDescription.Contains(forbiddenCultureName,
						StringComparison.OrdinalIgnoreCase),
					$"{authoredRow.StableReference} full description should not contain visible culture name {forbiddenCultureName}.");
			}
		}
	}

	[TestMethod]
	public void MedievalAuthoredOutfitPieces_PreserveVariableColourComponentsAndTokens()
	{
		var specsByReference = ItemSeeder.MedievalExplicitOutfitPieceItemSpecsForTesting
			.ToDictionary(x => x.StableReference, x => x, StringComparer.OrdinalIgnoreCase);

		foreach (var authoredRow in ItemSeeder.MedievalAuthoredOutfitPiecesForTesting)
		{
			var spec = specsByReference[authoredRow.StableReference];
			if (!string.IsNullOrWhiteSpace(authoredRow.VariableColourComponent))
			{
				Assert.IsTrue(spec.Components.Contains(authoredRow.VariableColourComponent, StringComparer.OrdinalIgnoreCase),
					$"{authoredRow.StableReference} should include {authoredRow.VariableColourComponent}.");
				Assert.IsTrue(authoredRow.ColourVariablesUsed.Any(),
					$"{authoredRow.StableReference} should document colour variables used.");
				Assert.IsTrue(authoredRow.ColourVariablesUsed.Any(variable =>
						spec.ShortDescription.Contains(variable, StringComparison.Ordinal)),
					$"{authoredRow.StableReference} short description should preserve the authored colour variable.");
				Assert.IsTrue(authoredRow.ColourVariablesUsed.Any(variable =>
						spec.FullDescription.Contains(variable, StringComparison.Ordinal)),
					$"{authoredRow.StableReference} full description should preserve the authored colour variable.");

				if (authoredRow.VariableColourComponent.Contains("_2", StringComparison.OrdinalIgnoreCase))
				{
					Assert.IsTrue(spec.FullDescription.Contains("$colour1", StringComparison.Ordinal),
						$"{authoredRow.StableReference} two-colour full description should include $colour1.");
					Assert.IsTrue(spec.FullDescription.Contains("$colour2", StringComparison.Ordinal),
						$"{authoredRow.StableReference} two-colour full description should include $colour2.");
				}

				continue;
			}

			Assert.IsTrue(IsDocumentedNonColourItem(authoredRow),
				$"{authoredRow.StableReference} has no variable colour component and should be a non-colourable rigid, writing, liquid, or metal item.");
		}
	}

	[TestMethod]
	public void MedievalExplicitOutfitPieceCrafts_MapVariableColourProducts()
	{
		var craftSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeederCrafting.Medieval.cs");
		var variableRows = ItemSeeder.MedievalAuthoredOutfitPiecesForTesting
			.Where(x => !string.IsNullOrWhiteSpace(x.VariableColourComponent))
			.ToDictionary(x => x.StableReference, x => x, StringComparer.OrdinalIgnoreCase);
		var mappingsByReference = ItemSeeder.MedievalExplicitOutfitPieceProductVariableMappingsForTesting
			.ToDictionary(x => x.StableReference, x => x, StringComparer.OrdinalIgnoreCase);

		Assert.AreEqual(variableRows.Count, mappingsByReference.Count,
			"Every colourable authored outfit-piece row should have a variable-product mapping for its craft product.");
		AssertContains(craftSource, "SimpleVariableProduct - 1x");
		AssertContains(craftSource, "BuildMedievalExplicitOutfitPieceProductSpec");
		AssertContains(craftSource, "piece.Craft.Products");
		Assert.IsFalse(craftSource.Contains("productsFactory", StringComparison.Ordinal),
			"Explicit outfit-piece products should live on EraCraftSpec.Products, not a separate seed-time product factory hook.");
		Assert.IsFalse(craftSource.Contains("MedievalExplicitOutfitPieceProduct(", StringComparison.Ordinal),
			"Explicit outfit-piece products should be built by EraCraftProductSpec entries on the craft spec.");

		foreach (var (stableReference, authoredRow) in variableRows)
		{
			Assert.IsTrue(mappingsByReference.TryGetValue(stableReference, out var mappingRow),
				$"{stableReference} should have product variable mappings.");
			Assert.IsTrue(mappingRow.ProductVariableMappings.Any(),
				$"{stableReference} should not craft as a StableSimpleProduct when it has {authoredRow.VariableColourComponent}.");

			if (authoredRow.VariableColourComponent!.Contains("_2", StringComparison.OrdinalIgnoreCase))
			{
				Assert.IsTrue(mappingRow.ProductVariableMappings.Any(x => x.StartsWith("Colour1=$i", StringComparison.Ordinal)),
					$"{stableReference} should map $colour1 from a craft input.");
				Assert.IsTrue(mappingRow.ProductVariableMappings.Any(x => x.StartsWith("Colour2=$i", StringComparison.Ordinal)),
					$"{stableReference} should map $colour2 from a craft input.");
				continue;
			}

			Assert.IsTrue(mappingRow.ProductVariableMappings.Any(x => x.StartsWith("Colour=$i", StringComparison.Ordinal)),
				$"{stableReference} should map $colour from a craft input.");
		}

		AssertContains(string.Join("|",
				mappingsByReference["medieval_outfit_piece_early_anglo_saxon_male_peasant_tablet_banded_wool_tunic"]
					.ProductVariableMappings),
			"Colour2=$i3");
	}

	[TestMethod]
	public void MedievalClothingDescriptions_DoNotContainBuilderAdminOrMetaWording()
	{
		var forbiddenTerms = new[]
		{
			"builder",
			"builders",
			"metadata",
			"visible craft text",
			"culture-neutral",
			"culture is visible",
			"belongs" + " to the",
			"fills" + " the",
			"outfit slot"
		};
		var clothingSpecs = ItemSeeder.MedievalItemSpecsForTesting
			.Where(x => x.StableReference.StartsWith("medieval_clothing_", StringComparison.OrdinalIgnoreCase) ||
			            x.StableReference.StartsWith("medieval_outfit_piece_", StringComparison.OrdinalIgnoreCase))
			.ToList();

		Assert.IsTrue(clothingSpecs.Count > 0, "Expected medieval clothing specs to test.");
		foreach (var spec in clothingSpecs)
		{
			foreach (var term in forbiddenTerms)
			{
				Assert.IsFalse(spec.ShortDescription.Contains(term, StringComparison.OrdinalIgnoreCase),
					$"{spec.StableReference} short description should not contain meta wording '{term}'.");
				Assert.IsFalse(spec.FullDescription.Contains(term, StringComparison.OrdinalIgnoreCase),
					$"{spec.StableReference} full description should not contain meta wording '{term}'.");
			}
		}
	}

	[TestMethod]
	public void MedievalOutfitSlots_PointToAuthoredOrIntentionallySharedItems()
	{
		var authoredReferences = ItemSeeder.MedievalAuthoredOutfitPiecesForTesting
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var itemReferences = ItemSeeder.MedievalItemStableReferencesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (var outfit in ItemSeeder.MedievalOutfitsForTesting)
		{
			foreach (var (slot, stableReference) in outfit.SlotItemStableReferences)
			{
				Assert.IsTrue(itemReferences.Contains(stableReference),
					$"{outfit.OutfitReference} / {slot} references missing item {stableReference}.");
				if (authoredReferences.Contains(stableReference))
				{
					continue;
				}

				Assert.IsTrue(outfit.IntentionallySharedOrGenericSlots.Contains(slot, StringComparer.OrdinalIgnoreCase),
					$"{outfit.OutfitReference} / {slot} must point to an authored outfit item or an intentionally shared/common item.");
			}
		}
	}

	[TestMethod]
	public void SharedEraRecords_AreConfiguredForAntiquityAndMedieval()
	{
		var eraSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.EraDefinitions.cs");
		var medievalConfig = ItemSeeder.MedievalEraConfigurationForTesting;
		var antiquityConfig = ItemSeeder.AntiquityEraConfigurationForTesting;

		foreach (var expected in new[]
		         {
			         "EraItemSpec",
			         "EraClothingPieceSpec",
			         "EraOutfitSpec",
			         "EraOutfitSlotSpec",
			         "EraCraftSpec",
			         "EraCraftInputSpec",
			         "EraCraftToolSpec",
			         "EraCraftProductSpec",
			         "EraCultureSpec",
			         "EraSeederConfiguration",
			         "EraVariableColourPolicy"
		         })
		{
			AssertContains(eraSource, $"record {expected}");
		}

		Assert.AreEqual("medieval", medievalConfig.EraKey);
		Assert.AreEqual("antiquity", antiquityConfig.EraKey);
		Assert.IsTrue(medievalConfig.CompleteOutfitCataloguesRequired);
		Assert.IsFalse(antiquityConfig.CompleteOutfitCataloguesRequired);
		Assert.IsTrue(medievalConfig.GenericBaselineWardrobeGenerationAllowed);
		Assert.IsFalse(antiquityConfig.GenericBaselineWardrobeGenerationAllowed);
		Assert.IsFalse(medievalConfig.PlayerFacingDescriptionsMayIncludeCultureNames);
		Assert.IsFalse(antiquityConfig.PlayerFacingDescriptionsMayIncludeCultureNames);
		Assert.IsTrue(medievalConfig.SlotKeys.Contains("bodywear", StringComparer.OrdinalIgnoreCase));
		Assert.IsTrue(antiquityConfig.CultureKeys.Contains("hellenic", StringComparer.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void MedievalAuthoredOutfitPieces_UseValidComponentsAndRoleItemShapes()
	{
		var knownComponentSource =
			ReadSource("DatabaseSeeder", "Seeders", "UsefulSeeder.ItemComponents.cs") +
			ReadMedievalItemSources() +
			ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Rework.Antiquity.cs");
		var authoredComponents = ItemSeeder.MedievalAuthoredOutfitPiecesForTesting
			.SelectMany(x => x.Components)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		foreach (var component in authoredComponents)
		{
			Assert.IsTrue(knownComponentSource.Contains($"\"{component}\"", StringComparison.OrdinalIgnoreCase),
				$"Expected authored component {component} to exist in seeded components or existing accepted item seeder usage.");
		}

		var specsByReference = ItemSeeder.MedievalExplicitOutfitPieceItemSpecsForTesting
			.ToDictionary(x => x.StableReference, x => x, StringComparer.OrdinalIgnoreCase);

		AssertSpecComponents(specsByReference, "medieval_outfit_piece_high_british_male_religious_small_prayer_book",
			"Book_Small_40_Page", "Destroyable_Paper");
		AssertSpecComponents(specsByReference, "medieval_outfit_piece_fatimid_male_religious_endowment_slip",
			"PaperSheet_Scroll", "Destroyable_Paper");
		AssertSpecComponents(specsByReference, "medieval_outfit_piece_anglo_danish_male_merchant_reeve_tally_pouch",
			"Wear_Waist", "Container_Pouch", "Beltable");
		AssertSpecComponents(specsByReference, "medieval_outfit_piece_fatimid_male_military_archer_quiver",
			"Wear_Shoulder", "Container_Quiver");
		AssertSpecComponents(specsByReference, "medieval_outfit_piece_seljuk_ayyubid_male_military_bowcase_belt",
			"Wear_Waist", "Belt_6", "Beltable", "Armour_BoiledLeather");
		AssertSpecComponents(specsByReference, "medieval_outfit_piece_high_british_male_military_archer_bracer",
			"Wear_Fingerless_Gloves", "Armour_BoiledLeather");
		AssertSpecComponents(specsByReference, "medieval_outfit_piece_early_anglo_saxon_female_merchant_silver_brooch",
			"Wear_Shoulder", "Destroyable_HeavyMetal");
		AssertSpecComponents(specsByReference, "medieval_outfit_piece_byzantine_male_religious_icon_tablet",
			"Holdable", "Destroyable_Misc");

		foreach (var stableReference in new[]
		         {
			         "medieval_outfit_piece_byzantine_male_religious_icon_tablet",
			         "medieval_outfit_piece_gaelic_male_religious_note_board",
			         "medieval_outfit_piece_norse_male_merchant_runic_trade_tag",
			         "medieval_outfit_piece_steppe_turkic_male_merchant_seal_tag"
		         })
		{
			var components = specsByReference[stableReference].Components;
			Assert.IsFalse(components.Contains("Wear_Necklace", StringComparer.OrdinalIgnoreCase),
				$"{stableReference} should not be forced into jewellery components.");
			Assert.IsFalse(components.Contains("Wear_Shoulder", StringComparer.OrdinalIgnoreCase),
				$"{stableReference} should not be forced into shoulder jewellery components.");
		}
	}

	[TestMethod]
	public void MedievalExplicitOutfitPieceCrafts_UseExpectedStockFamilies()
	{
		var craftByReference = ItemSeeder.MedievalExplicitOutfitPieceCraftsForTesting
			.ToDictionary(x => x.StableReference, x => x, StringComparer.OrdinalIgnoreCase);

		foreach (var stableReference in new[]
		         {
			         "medieval_outfit_piece_early_anglo_saxon_female_merchant_silver_brooch",
			         "medieval_outfit_piece_carolingian_male_military_riding_spurs",
			         "medieval_outfit_piece_german_hre_male_noble_belt_mounts",
			         "medieval_outfit_piece_german_hre_male_military_town_crossbow_militia_hook"
		         })
		{
			AssertCraftInputs(craftByReference, stableReference, "Tool Blank Stock");
			AssertCraftInputsDoNotContain(craftByReference, stableReference, "Garment Cloth", "Broadcloth Stock", "Spun Yarn");
		}

		AssertCraftInputs(craftByReference, "medieval_outfit_piece_high_british_male_religious_small_prayer_book",
			"Paper Sheet Stock", "Bookbinding Leather Stock");
		AssertCraftInputs(craftByReference, "medieval_outfit_piece_anglo_danish_female_noble_fur_edged_cloak",
			"Fur Panel Stock");
		AssertCraftInputs(craftByReference, "medieval_outfit_piece_early_anglo_saxon_female_military_brooch_fastened_war_cloak",
			"Fulled Cloth");
		AssertCraftInputs(craftByReference, "medieval_outfit_piece_seljuk_ayyubid_male_noble_high_boots",
			"Turnshoe Upper Stock");
		AssertCraftInputs(craftByReference, "medieval_outfit_piece_early_anglo_saxon_male_military_padded_shield_wall_tunic",
			"Quilted Armour Padding");
		AssertCraftInputs(craftByReference, "medieval_outfit_piece_early_anglo_saxon_male_peasant_tablet_banded_wool_tunic",
			"Tablet-Woven Band Stock");
		AssertCraftInputs(craftByReference, "medieval_outfit_piece_early_anglo_saxon_male_merchant_bordered_tunic",
			"Embroidered Trim Stock");
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
	public void MedievalEasternOutfits_AreExplicitCompleteAndVariantDistinct()
	{
		AssertExplicitOutfitClusterComplete(ItemSeeder.MedievalEasternOutfitCultureKeysForTesting, 8, 96);
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
	public void MedievalEasternOutfits_ContainRequiredCultureVocabulary()
	{
		var piecesByCulture = ItemSeeder.MedievalExplicitOutfitPiecesForTesting
			.GroupBy(x => x.CultureKey, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => string.Join(" ", x.Select(piece => piece.PieceName)).ToLowerInvariant(),
				StringComparer.OrdinalIgnoreCase);
		var required = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["andalusi"] = ["qamis", "sirwal", "burnous", "turban", "tiraz"],
			["byzantine"] = ["silk dalmatic", "sagion", "court belt", "icon pouch", "skaramangion"],
			["abbasid"] = ["qamis", "qaba", "caftan", "scholar robe", "sash"],
			["fatimid"] = ["linen robe", "tiraz-banded", "cotton wrap", "court kaftan"],
			["seljuk_ayyubid"] = ["riding caftan", "quilted coat", "high riding boots", "bowcase belt"],
			["rus_novgorod"] = ["rubakha", "fur-edged kaftan", "onuchi", "fur hat", "birchbark"],
			["steppe_turkic"] = ["felt riding caftan", "tied riding coat", "high boots", "bowcase-and-quiver"],
			["song_china"] = ["cross-collar robe", "scholar robe", "official cap", "padded winter robe", "cloth shoes"]
		};

		foreach (var culture in ItemSeeder.MedievalEasternOutfitCultureKeysForTesting)
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
		var authoredByReference = ItemSeeder.MedievalAuthoredOutfitPiecesForTesting
			.ToDictionary(x => x.StableReference, x => x, StringComparer.OrdinalIgnoreCase);

		foreach (var piece in ItemSeeder.MedievalExplicitOutfitPiecesForTesting
			         .DistinctBy(x => x.StableReference))
		{
			Assert.IsTrue(craftByReference.TryGetValue(piece.StableReference, out var craft),
				$"Expected explicit outfit craft for {piece.StableReference}.");
			Assert.IsFalse(craft.CraftName.Contains("regional pattern", StringComparison.OrdinalIgnoreCase),
				$"Explicit outfit piece craft {craft.CraftName} should not use regional pattern naming.");
			if (authoredByReference.TryGetValue(piece.StableReference, out var authoredRow))
			{
				Assert.IsTrue(craft.CraftName.Contains(authoredRow.ShortDescription, StringComparison.OrdinalIgnoreCase),
					$"Explicit outfit piece craft {craft.CraftName} should include authored object {authoredRow.ShortDescription}.");
			}
			else
			{
				Assert.IsTrue(craft.CraftName.Contains(piece.PieceName, StringComparison.OrdinalIgnoreCase),
					$"Explicit outfit piece craft {craft.CraftName} should include named object {piece.PieceName}.");
			}
		}

		Assert.IsTrue(craftNames.Any(x => x.CraftName.Contains("sew a $colour hangerok apron dress", StringComparison.Ordinal)),
			"Expected the hangerok craft name to include the authored object without a direct culture adjective.");

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
			         "Silk Brocade Panel",
			         "Paper Sheet Stock",
			         "Armour Lamella Stock",
			         "Fulled Cloth",
			         "Fur Panel Stock",
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

		var expectedInputsByStableReference = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["medieval_outfit_piece_fatimid_male_peasant_cotton_lower_wrap"] = ["cotton", "Garment Cloth"],
			["medieval_outfit_piece_byzantine_male_noble_silk_dalmatic"] = ["silk", "Silk Brocade Panel"],
			["medieval_outfit_piece_byzantine_male_military_lamellar_coat_cover"] = ["Armour Lamella Stock"],
			["medieval_outfit_piece_steppe_turkic_male_merchant_felt_riding_caftan"] = ["felt", "Fulled Cloth"],
			["medieval_outfit_piece_rus_novgorod_male_merchant_fur_edged_kaftan"] = ["fur", "Fur Panel Stock"],
			["medieval_outfit_piece_abbasid_male_religious_notebook"] = ["paper", "Paper Sheet Stock"],
			["medieval_outfit_piece_song_china_male_peasant_cloth_shoes"] = ["cotton", "Garment Cloth"],
			["medieval_outfit_piece_song_china_male_noble_padded_winter_robe"] = ["Quilted Armour Padding"]
		};

		foreach (var (stableReference, expectedInputs) in expectedInputsByStableReference)
		{
			Assert.IsTrue(craftByReference.TryGetValue(stableReference, out var craft),
				$"Expected explicit outfit craft for eastern material-stock piece {stableReference}.");
			foreach (var expectedInput in expectedInputs)
			{
				Assert.IsTrue(craft.Inputs.Any(x => x.Contains(expectedInput, StringComparison.OrdinalIgnoreCase)),
					$"Expected {stableReference} craft inputs to include {expectedInput}.");
			}
		}

		foreach (var stableReference in new[]
		         {
			         "medieval_outfit_piece_high_british_male_religious_small_prayer_book",
			         "medieval_outfit_piece_capetian_male_religious_chapel_book"
		         })
		{
			Assert.IsTrue(craftByReference.TryGetValue(stableReference, out var boundBookCraft),
				$"Expected explicit outfit craft for bound paper item {stableReference}.");
			Assert.IsTrue(boundBookCraft.Inputs.Any(x => x.Contains("Bookbinding Leather Stock", StringComparison.OrdinalIgnoreCase)),
				$"Expected {stableReference} craft inputs to include bookbinding leather stock.");
			Assert.IsTrue(boundBookCraft.Tools.Any(x => x.Contains("Book Press", StringComparison.OrdinalIgnoreCase)),
				$"Expected {stableReference} craft tools to include a book press.");
		}
	}

	[TestMethod]
	public void MedievalCultureStatusAndStableReferenceFamilies_AreDocumented()
	{
		var docs = ReadMedievalDocs();
		var itemSource = ReadMedievalItemSources();

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
		var itemSource = ReadMedievalItemSources();
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
		var itemSource = ReadMedievalItemSources();

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
		var itemSource = ReadMedievalItemSources();
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
		var itemSource = ReadMedievalItemSources();
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

	private static string ReadMedievalItemSources()
	{
		var seederPath = SourcePath(Path.Combine("DatabaseSeeder", "Seeders"));
		return string.Concat(Directory
			.GetFiles(seederPath, "ItemSeeder.Rework.Medieval*.cs")
			.OrderBy(x => x, StringComparer.Ordinal)
			.Select(File.ReadAllText));
	}

	private static string ReadMedievalDirectItemSources()
	{
		var seederPath = SourcePath(Path.Combine("DatabaseSeeder", "Seeders"));
		return string.Concat(Directory
			.GetFiles(seederPath, "ItemSeeder.Rework.Medieval*.cs")
			.Where(x =>
			{
				var name = Path.GetFileName(x);
				return !name.Equals("ItemSeeder.Rework.Medieval.cs", StringComparison.OrdinalIgnoreCase) &&
				       !name.Equals("ItemSeeder.Rework.MedievalSupport.cs", StringComparison.OrdinalIgnoreCase);
			})
			.OrderBy(x => x, StringComparer.Ordinal)
			.Select(File.ReadAllText));
	}

	private static string ReadMedievalDocs()
	{
		return string.Concat(new[]
		{
			"Crafting_System_Builder_Workflows.md",
			"Era_Seeder_Shared_Architecture.md",
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

	private static IReadOnlyCollection<(string CultureKey, string Group, string StableReference, string Status)> ReadExplicitMedievalCultureCatalogueEntryStatusesFromDocs()
	{
		var docs = ReadSource("Design Documents", "Crafting", "Medieval_Culture_Catalogue.md");
		var entries = new List<(string CultureKey, string Group, string StableReference, string Status)>();
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

			var statusMatch = Regex.Match(line,
				@"^- `(?<stableReference>[^`]+)` - Status: `(?<status>ImplementedItem|CoveredByOutfitPiece|AliasOfExistingStableReference|Deferred)`");
			if (!statusMatch.Success)
			{
				continue;
			}

			Assert.IsFalse(string.IsNullOrWhiteSpace(cultureKey),
				$"Status row {statusMatch.Groups["stableReference"].Value} is outside a culture section.");
			Assert.IsTrue(ExplicitMedievalCultureCatalogueGroups.Contains(group ?? string.Empty),
				$"Status row {statusMatch.Groups["stableReference"].Value} is under unexpected group {group}.");
			entries.Add((cultureKey!, group!, statusMatch.Groups["stableReference"].Value,
				statusMatch.Groups["status"].Value));
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

	private static bool IsDocumentedNonColourItem(ItemSeeder.MedievalAuthoredOutfitPieceTestData authoredRow)
	{
		var hasRigidMaterial = authoredRow.MaterialType is MaterialBehaviourType.Metal or MaterialBehaviourType.Wood
			or MaterialBehaviourType.Ceramic or MaterialBehaviourType.Wax;
		var hasWritingOrLiquidComponent = authoredRow.Components.Any(component =>
			component.Contains("Book_", StringComparison.OrdinalIgnoreCase) ||
			component.Contains("PaperSheet", StringComparison.OrdinalIgnoreCase) ||
			component.Contains("LContainer", StringComparison.OrdinalIgnoreCase));
		var hasRigidNoun = new[]
			{
				"book", "leaf", "slip", "tag", "token", "board", "tablet", "flask", "brooch", "pin",
				"mount", "mounts", "spurs", "fibula", "hook"
			}
			.Contains(authoredRow.Noun, StringComparer.OrdinalIgnoreCase);

		return hasRigidMaterial || hasWritingOrLiquidComponent || hasRigidNoun ||
		       authoredRow.Material.Equals("paper", StringComparison.OrdinalIgnoreCase);
	}

	private static void AssertSpecComponents(
		IReadOnlyDictionary<string, ItemSeeder.EraItemSpecTestData> specsByReference,
		string stableReference,
		params string[] expectedComponents)
	{
		Assert.IsTrue(specsByReference.TryGetValue(stableReference, out var spec),
			$"Expected item spec {stableReference}.");
		foreach (var expectedComponent in expectedComponents)
		{
			Assert.IsTrue(spec.Components.Contains(expectedComponent, StringComparer.OrdinalIgnoreCase),
				$"Expected {stableReference} to include component {expectedComponent}.");
		}
	}

	private static void AssertCraftInputs(
		IReadOnlyDictionary<string, (string StableReference, string CraftName, IReadOnlyCollection<string> Inputs, IReadOnlyCollection<string> Tools)> craftByReference,
		string stableReference,
		params string[] expectedInputs)
	{
		Assert.IsTrue(craftByReference.TryGetValue(stableReference, out var craft),
			$"Expected explicit outfit craft for {stableReference}.");
		foreach (var expectedInput in expectedInputs)
		{
			Assert.IsTrue(craft.Inputs.Any(input => input.Contains(expectedInput, StringComparison.OrdinalIgnoreCase)),
				$"Expected {stableReference} craft inputs to include {expectedInput}.");
		}
	}

	private static void AssertCraftInputsDoNotContain(
		IReadOnlyDictionary<string, (string StableReference, string CraftName, IReadOnlyCollection<string> Inputs, IReadOnlyCollection<string> Tools)> craftByReference,
		string stableReference,
		params string[] forbiddenInputs)
	{
		Assert.IsTrue(craftByReference.TryGetValue(stableReference, out var craft),
			$"Expected explicit outfit craft for {stableReference}.");
		foreach (var forbiddenInput in forbiddenInputs)
		{
			Assert.IsFalse(craft.Inputs.Any(input => input.Contains(forbiddenInput, StringComparison.OrdinalIgnoreCase)),
				$"Expected {stableReference} craft inputs not to include {forbiddenInput}.");
		}
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

	private static string SourceStringLiteral(string value)
	{
		return $"\"{value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal).Replace("\r", "\\r", StringComparison.Ordinal).Replace("\n", "\\n", StringComparison.Ordinal)}\"";
	}

	private static string SourceDoubleLiteral(double value)
	{
		var text = value.ToString("0.0#########", CultureInfo.InvariantCulture);
		return text.Contains('.', StringComparison.Ordinal) ? text : $"{text}.0";
	}

	private static string SourceDecimalLiteral(decimal value)
	{
		return $"{value.ToString("0.0#########", CultureInfo.InvariantCulture)}m";
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
