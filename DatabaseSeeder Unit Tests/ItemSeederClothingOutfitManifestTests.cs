#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ItemQuality = MudSharp.GameItems.ItemQuality;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederClothingOutfitManifestTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options =
			new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
				.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void DocumentedOutfitManifests_HaveExpectedCompleteCoverage()
	{
		var antiquity = ItemSeeder.AntiquityOutfitManifestSpecsForTesting;
		var medieval = ItemSeeder.MedievalOutfitManifestSpecsForTesting;
		var renaissance = ItemSeeder.RenaissanceOutfitManifestSpecsForTesting;
		var earlyModern = ItemSeeder.EarlyModernOutfitManifestSpecsForTesting;

		Assert.AreEqual(34, antiquity.Count);
		Assert.AreEqual(167, medieval.Count);
		Assert.AreEqual(65, renaissance.Count);
		Assert.AreEqual(885, earlyModern.Count);
		Assert.AreEqual(11, renaissance.Count(x => x.StableKey.StartsWith(
			"renaissance_outfit_overlay_", StringComparison.Ordinal)));

		var all = antiquity.Concat(medieval).Concat(renaissance).Concat(earlyModern).ToArray();
		Assert.AreEqual(all.Length, all.Select(x => x.StableKey).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.AreEqual(all.Length, all.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(all.All(x => x.Name.Length <= 200));
		Assert.IsTrue(all.All(x => !x.Name.Contains(":", StringComparison.Ordinal)));
		Assert.IsTrue(all.All(x => !x.Description.Contains("Source:", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(all.All(x => !x.Description.Contains(".md", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(all.All(x => x.ItemStableReferences.Count > 0));
		Assert.IsTrue(all.All(x =>
			x.ItemStableReferences.Count == x.ItemStableReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count()));
		Assert.IsTrue(all.SelectMany(x => x.ItemStableReferences).All(x => x.Length <= 100));
		Assert.IsTrue(all.SelectMany(x => x.Items)
			.Where(x => x.SkinStableReference is not null)
			.All(x => x.SkinStableReference!.Length <= 100));

		var skins = ItemSeeder.DocumentedClothingSkinsForTesting;
		Assert.AreEqual(5, skins.Count);
		Assert.AreEqual(skins.Count, skins.Select(x => x.StableReference)
			.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		var skinsByStableReference = skins.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);
		var skinnedItems = all.SelectMany(x => x.Items)
			.Where(x => x.SkinStableReference is not null)
			.ToArray();
		Assert.AreEqual(5, skinnedItems.Length);
		Assert.IsTrue(skinnedItems.All(x =>
			skinsByStableReference.TryGetValue(x.SkinStableReference!, out var skin) &&
			skin.BaseItemStableReference.Equals(x.ItemStableReference, StringComparison.OrdinalIgnoreCase)));

		var renaissanceWearComponents = ItemSeeder.RenaissanceOutfitWearComponentsForTesting;
		Assert.IsTrue(renaissance.All(outfit =>
		{
			var components = outfit.ItemStableReferences
				.Where(renaissanceWearComponents.ContainsKey)
				.Select(x => renaissanceWearComponents[x])
				.ToArray();
			return outfit.StableKey == "renaissance_outfit_latin_priest_mass" ||
			       components.Length == components.Distinct(StringComparer.OrdinalIgnoreCase).Count();
		}), "Renaissance outfits must not require two items through the same default wearable component, except the established Latin Mass vestment layering.");
		CollectionAssert.AreEqual(
			new[]
			{
				"medieval_latin_amice", "medieval_latin_white_alb", "medieval_latin_linen_cincture",
				"medieval_latin_stole", "medieval_latin_maniple", "medieval_latin_chasuble", "medieval_soft_leather_shoes"
			},
			renaissance.Single(x => x.StableKey == "renaissance_outfit_latin_priest_mass").ItemStableReferences.ToArray());
		Assert.AreEqual(
			"Early Modern Maritime South-east Asian port artisan male",
			earlyModern.Single(x => x.StableKey == "earlymodern_outfit_0213").Name);
	}

	[TestMethod]
	public void DocumentedOutfitManifests_AllItemReferencesHaveSeedSources()
	{
		var itemSeederSource = SeederSourceTestHelper.ReadPartialFamily("ItemSeeder");
		var generatedReferences = ItemSeeder.DocumentedClothingItemStableReferencesForTesting;
		var unresolved = ItemSeeder.AntiquityOutfitManifestSpecsForTesting
			.Concat(ItemSeeder.MedievalOutfitManifestSpecsForTesting)
			.Concat(ItemSeeder.RenaissanceOutfitManifestSpecsForTesting)
			.Concat(ItemSeeder.EarlyModernOutfitManifestSpecsForTesting)
			.SelectMany(x => x.ItemStableReferences)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Where(x => !generatedReferences.Contains(x) &&
			            !itemSeederSource.Contains($"\"{x}\"", StringComparison.Ordinal))
			.ToArray();

		Assert.AreEqual(0, unresolved.Length, string.Join(Environment.NewLine, unresolved));
		Assert.AreEqual(1324, generatedReferences.Count);
		Assert.AreEqual(215, ItemSeeder.RenaissanceOutfitItemStableReferencesForTesting.Count);
		Assert.AreEqual(472, ItemSeeder.RenaissanceClothingItemStableReferencesForTesting.Count);
	}

	[TestMethod]
	public void DocumentedClothingItems_HaveSubstantiveAppearanceDescriptions()
	{
		var items = ItemSeeder.DocumentedClothingItemDescriptionsForTesting;

		Assert.IsTrue(items.Count > 1300);
		Assert.IsTrue(items.All(x => x.FullDescription.Length >= 300));
		Assert.IsTrue(items.All(x => x.FullDescription.Count(character => character == '.') >= 4));
		Assert.IsTrue(items.All(x => !x.FullDescription.Contains(
			"recognisable form and drape", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(items.All(x => !x.FullDescription.Contains(
			"documented form", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void UpsertOutfitManifest_RerunUpdatesOwnedTemplateWithoutDuplicating()
	{
		using var context = BuildContext();
		var shirt = new GameItemProto { Id = 101, Name = "shirt", ShortDescription = "a shirt" };
		var trousers = new GameItemProto { Id = 102, Name = "trousers", ShortDescription = "some trousers" };

		var first = ItemSeeder.UpsertOutfitManifestForTesting(
			context,
			"test_manifest",
			"Test Manifest",
			"First description.",
			new[] { ("shirt", shirt), ("trousers", trousers) });
		context.SaveChanges();
		var id = first.Id;

		var second = ItemSeeder.UpsertOutfitManifestForTesting(
			context,
			"test_manifest",
			"Renamed Test Manifest",
			"Second description.",
			new[] { ("trousers", trousers) });
		context.SaveChanges();

		Assert.AreEqual(id, second.Id);
		Assert.AreEqual(1, context.OutfitTemplates.Count());
		Assert.AreEqual("Renamed Test Manifest", second.Name);
		Assert.IsTrue(second.Description.Contains("[[ItemSeederOutfitManifest:test_manifest]]", StringComparison.Ordinal));
		var item = second.OutfitTemplateItems.Single();
		Assert.AreEqual("trousers", item.TemplateKey);
		Assert.AreEqual(102, item.GameItemProtoId);
		Assert.AreEqual(0, item.WearOrder);
		Assert.AreEqual(0, item.Placement);
	}

	[TestMethod]
	public void UpsertOutfitManifest_DoesNotOverwriteBuilderAuthoredNameCollision()
	{
		using var context = BuildContext();
		context.OutfitTemplates.Add(new OutfitTemplate
		{
			Name = "Test Manifest",
			Description = "Builder-authored content.",
			Exclusivity = 0
		});
		context.SaveChanges();
		var shirt = new GameItemProto { Id = 101, Name = "shirt", ShortDescription = "a shirt" };

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			ItemSeeder.UpsertOutfitManifestForTesting(
				context,
				"test_manifest",
				"Test Manifest",
				"Stock description.",
				new[] { ("shirt", shirt) }));

		StringAssert.Contains(exception.Message, "builder-authored template");
		Assert.AreEqual("Builder-authored content.", context.OutfitTemplates.Single().Description);
	}

	[TestMethod]
	public void DocumentedClothingSkins_HaveExpectedBasesAndPresentationFields()
	{
		var skins = ItemSeeder.DocumentedClothingSkinsForTesting
			.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);
		var expected = new Dictionary<string, (string Base, string ItemName, string ShortDescription, ItemQuality Quality)>
		{
			["antiquity_skin_senatorial_broad_striped_tunica"] =
				("antiquity_fine_linen_tunica", "tunica", "a broad-striped white linen tunica", ItemQuality.Good),
			["antiquity_skin_vestal_white_priestly_veil"] =
				("adjacent_antiquity_priestly_linen_veil", "veil", "a long white linen priestly veil", ItemQuality.Good),
			["renaissance_skin_venetian_senatorial_red_gown"] =
				("renaissance_western_furred_gown", "gown", "a scarlet fur-edged senatorial gown", ItemQuality.VeryGood),
			["earlymodern_skin_judicial_full_sleeved_robe"] =
				("renaissance_institution_academic_robe", "robe", "a long dark full-sleeved judicial robe", ItemQuality.VeryGood),
			["earlymodern_skin_formal_mourning_mantua"] =
				("earlymodern_western_clothing_plain_mantua_gown", "mantua", "a full black formal mourning mantua", ItemQuality.VeryGood)
		};

		CollectionAssert.AreEquivalent(expected.Keys.ToArray(), skins.Keys.ToArray());
		foreach (var (stableReference, expectedSkin) in expected)
		{
			var actual = skins[stableReference];
			Assert.AreEqual(expectedSkin.Base, actual.BaseItemStableReference);
			Assert.AreEqual(expectedSkin.ItemName, actual.ItemName);
			Assert.AreEqual(expectedSkin.ShortDescription, actual.ShortDescription);
			Assert.AreEqual(expectedSkin.Quality, actual.Quality);
			Assert.IsTrue(actual.FullDescription.Length >= 300, stableReference);
		}
	}

	[TestMethod]
	public void UpsertOutfitManifest_PersistsSkinBindingsAndRejectsMismatches()
	{
		using var context = BuildContext();
		var gown = new GameItemProto { Id = 101, Name = "gown", ShortDescription = "a gown" };
		var compatibleSkin = new GameItemSkin
		{
			Id = 201,
			Name = "test_senatorial_gown_skin",
			ItemProtoId = gown.Id
		};

		var outfit = ItemSeeder.UpsertOutfitManifestWithSkinsForTesting(
			context,
			"test_skinned_manifest",
			"Test Skinned Manifest",
			"Stock description.",
			new (string StableReference, GameItemProto Prototype, string? SkinStableReference, GameItemSkin? Skin)[]
			{
				("gown", gown, "test_senatorial_gown_skin", compatibleSkin)
			});
		context.SaveChanges();

		Assert.AreEqual(201, outfit.OutfitTemplateItems.Single().SkinId);

		var incompatibleSkin = new GameItemSkin
		{
			Id = 202,
			Name = "test_other_skin",
			ItemProtoId = 999
		};
		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			ItemSeeder.UpsertOutfitManifestWithSkinsForTesting(
				context,
				"test_incompatible_skin_manifest",
				"Test Incompatible Skin Manifest",
				"Stock description.",
				new (string StableReference, GameItemProto Prototype, string? SkinStableReference, GameItemSkin? Skin)[]
				{
					("gown", gown, "test_other_skin", incompatibleSkin)
				}));

		StringAssert.Contains(exception.Message, "incompatible item prototypes");
	}
}
