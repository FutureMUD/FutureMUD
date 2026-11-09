#nullable enable

using DatabaseSeeder.Seeders;
using DatabaseSeeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ItemQuality = MudSharp.GameItems.ItemQuality;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederClothingOutfitManifestTests
{
	[TestMethod]
	[DataRow("rename")]
	[DataRow("blank")]
	[DataRow("status")]
	[DataRow("persisted-whitespace")]
	public void LiveItemReferenceBatch_PreservesCurrentRevisionsTrackedRenamesAndScope(string scenario)
	{
		using var context = BuildContext();
		var current = new GameItemProto { Id = 101, RevisionNumber = 0, UniqueName = "stock_shirt", EditableItem = new EditableItem { RevisionStatus = 4 } };
		context.GameItemProtos.AddRange(current,
			new GameItemProto { Id = 101, RevisionNumber = 1, UniqueName = "draft_shirt", EditableItem = new EditableItem { RevisionStatus = 0 } },
			new GameItemProto { Id = 102, RevisionNumber = 0, UniqueName = "stock_trousers", EditableItem = new EditableItem { RevisionStatus = 4 } });
		foreach (var item in context.GameItemProtos.Local)
		{
			item.Name = item.UniqueName;
			item.Keywords = item.UniqueName;
			item.MorphEmote = string.Empty;
		}
		context.SaveChanges();
		current.UniqueName = "builder_renamed_shirt";
		if (scenario == "blank") current.UniqueName = string.Empty;
		if (scenario == "status")
		{
			current.EditableItem.RevisionStatus = 0;
			context.GameItemProtos.Local.Single(x => x.Id == 101 && x.RevisionNumber == 1).EditableItem.RevisionStatus = 4;
		}
		if (scenario == "persisted-whitespace")
		{
			current.UniqueName = "   ";
			context.SaveChanges();
			context.ChangeTracker.Clear();
		}
		var seeder = new ItemSeeder();
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
		typeof(ItemSeeder).GetField("_context", flags)!.SetValue(seeder, context);
		var resolve = typeof(ItemSeeder).GetMethod("ResolveItemStableReference", flags)!;
		string? Reference(long id) => (string?)resolve.Invoke(seeder, new object[] { id });
		var expected = new[] { Reference(101), Reference(102), Reference(999) };
		if (scenario == "persisted-whitespace") context.ChangeTracker.Clear();
		using ((IDisposable)typeof(ItemSeeder).GetMethod("UseBatchLiveItemReferences", flags)!.Invoke(seeder, null)!)
		{
			CollectionAssert.AreEqual(expected, new[] { Reference(101), Reference(102), Reference(999) });
			Assert.AreEqual(scenario == "rename" ? "builder_renamed_shirt" : "draft_shirt", Reference(101));
		}
		Assert.IsNull(typeof(ItemSeeder).GetField("_batchLiveItemReferences", flags)!.GetValue(seeder), "The batch snapshot must not outlive reconciliation.");
	}

	[TestMethod]
	[DataRow("medieval")]
	[DataRow("renaissance")]
	[DataRow("earlymodern")]
	[DataRow("medieval renaissance")]
	[TestCategory("Integration")]
	public void SelectedEra_OutfitReferencesAreActuallySeeded(string eras)
	{
		using var context = BuildContext();
		context.Accounts.Add(new Account
		{
			Id = 1, Name = "Manifest test", CultureName = "en-AU", TimeZoneId = "UTC", UnitPreference = "Metric"
		});
		context.TraitDefinitions.Add(new TraitDefinition
		{
			Id = 1, Name = "Crafting", Type = 0, OwnerScope = 0, TraitGroup = "Crafting",
			ChargenBlurb = string.Empty, ValueExpression = string.Empty
		});
		context.SaveChanges();
		var document = new ItemSeeder().CaptureManifest(context,
			DatabaseSeeder.ItemSeederManifestCatalogue.FindRepositoryRoot(), eras);
		var items = document.Entries.Where(x => x.EntityType == "item")
			.Select(x => x.StableKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var outfits = new Dictionary<string, IReadOnlyList<ItemSeeder.ClothingOutfitManifestTestData>>
		{
			["antiquity"] = ItemSeeder.AntiquityOutfitManifestSpecsForTesting,
			["medieval"] = ItemSeeder.MedievalOutfitManifestSpecsForTesting,
			["renaissance"] = ItemSeeder.RenaissanceOutfitManifestSpecsForTesting,
			["earlymodern"] = ItemSeeder.EarlyModernOutfitManifestSpecsForTesting
		};
		var missing = eras.Split(' ').SelectMany(era => outfits[era])
			.SelectMany(outfit => outfit.ItemStableReferences.Where(item => !items.Contains(item))
				.Select(item => $"{outfit.StableKey}: {item}"))
			.ToArray();
		Assert.AreEqual(0, missing.Length, string.Join(Environment.NewLine, missing));
	}

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

		var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
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
		var exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
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
