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

		Assert.AreEqual(29, antiquity.Count);
		Assert.AreEqual(164, medieval.Count);
		Assert.AreEqual(59, renaissance.Count);
		Assert.AreEqual(883, earlyModern.Count);
		Assert.AreEqual(11, renaissance.Count(x => x.StableKey.StartsWith(
			"renaissance_outfit_overlay_", StringComparison.Ordinal)));

		var all = antiquity.Concat(medieval).Concat(renaissance).Concat(earlyModern).ToArray();
		Assert.AreEqual(all.Length, all.Select(x => x.StableKey).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.AreEqual(all.Length, all.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(all.All(x => x.Name.Length <= 200));
		Assert.IsTrue(all.All(x => x.ItemStableReferences.Count > 0));
		Assert.IsTrue(all.All(x =>
			x.ItemStableReferences.Count == x.ItemStableReferences.Distinct(StringComparer.OrdinalIgnoreCase).Count()));
		Assert.IsTrue(all.SelectMany(x => x.ItemStableReferences).All(x => x.Length <= 100));

		var renaissanceWearComponents = ItemSeeder.RenaissanceOutfitWearComponentsForTesting;
		Assert.IsTrue(renaissance.All(outfit =>
		{
			var components = outfit.ItemStableReferences
				.Select(x => renaissanceWearComponents[x])
				.ToArray();
			return components.Length == components.Distinct(StringComparer.OrdinalIgnoreCase).Count();
		}), "Renaissance outfits must not require two items through the same default wearable component.");
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
		Assert.AreEqual(1044, generatedReferences.Count);
		Assert.AreEqual(202, ItemSeeder.RenaissanceOutfitItemStableReferencesForTesting.Count);
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
}
