#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class IndustrialisedClothingReuseTests
{
	[DataTestMethod]
	[DataRow("builder-name", "already uses")]
	[DataRow("wrong-owner-id", "ownership conflict")]
	[DataRow("duplicate-marker", "Multiple outfit templates")]
	[DataRow("duplicate-name", "already uses")]
	[DataRow("unmanaged-drift", "Unmanaged outfit conflict")]
	[DataRow("other-owner", "is claimed by")]
	[DataRow("other-marker", "another stock key")]
	public void OutfitOwnership_RejectsLaterConflictBeforeAnyClothingMutation(string fault, string diagnostic)
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var catalogue = CatalogueWithTwoOutfits();
		InstallSkins(context, catalogue);
		var second = context.OutfitTemplates.Single(x => x.Name == "Fixture second ensemble");
		var managed = context.SeederManagedRecords.Single(x => x.EntityType == "outfit" && x.StableKey == "fixture_second");
		switch (fault)
		{
			case "builder-name": context.SeederManagedRecords.Remove(managed); second.Description = "A builder's unrelated outfit."; break;
			case "wrong-owner-id": managed.LogicalId = 987654; break;
			case "duplicate-marker": context.OutfitTemplates.Add(new() { Name = "Duplicate claim", Description = second.Description }); break;
			case "duplicate-name": context.OutfitTemplates.Add(new() { Name = second.Name, Description = "A separate builder-owned template." }); break;
			case "unmanaged-drift": context.SeederManagedRecords.Remove(managed); second.Description += "\nBuilder changes"; break;
			case "other-marker": managed.LogicalId = second.Id; second.Description = "[[ItemSeederOutfitManifest:another_key]]"; break;
			case "other-owner":
				context.SeederManagedRecords.Add(new()
				{
					Seeder = "Items", EntityType = "outfit", StableKey = "another_owner", Module = "outfits", LogicalId = second.Id
				});
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = OutfitOwnershipState(context);
		var seeder = new ItemSeeder(catalogue);
		var error = Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/outfits.tsv:3");
		StringAssert.Contains(error.Message, diagnostic);
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, OutfitOwnershipState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
		Assert.ThrowsException<InvalidOperationException>(() => seeder.ApplyClothingReuseForTesting("industrial"));
	}

	[DataTestMethod]
	[DataRow("exact-unmanaged")]
	[DataRow("custom-name")]
	[DataRow("custom-description")]
	[DataRow("custom-colour")]
	[DataRow("removed-marker")]
	[DataRow("stock-update")]
	public void OutfitOwnership_PreflightIsReadOnlyAndRerunsPreserveCompatibleCustomizations(string change)
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var catalogue = CatalogueWithTwoOutfits();
		InstallSkins(context, catalogue);
		context.ChangeTracker.Clear(); // Customisation immediately after the first installation must remain owned.
		var target = context.OutfitTemplates.Include(x => x.OutfitTemplateItems).Single(x => x.Name == "Fixture second ensemble");
		var id = target.Id;
		var record = context.SeederManagedRecords.Single(x => x.EntityType == "outfit" && x.StableKey == "fixture_second");
		Assert.AreEqual(id, record.LogicalId);
		switch (change)
		{
			case "exact-unmanaged": context.SeederManagedRecords.Remove(record); break;
			case "custom-name": target.Name = "Builder's ensemble"; break;
			case "custom-description": target.Description += "\nBuilder's description."; break;
			case "custom-colour": target.OutfitTemplateItems.Single().LoadArguments = "colour=22"; break;
			case "removed-marker": target.Description = "An entirely builder-authored description."; break;
			case "stock-update":
				catalogue = catalogue with { Clothing = catalogue.Clothing with
				{
					Outfits = catalogue.Clothing.Outfits.Select(x => x.StableReference == "fixture_second"
						? x with { Name = "Updated stock ensemble", Description = "An updated stock ensemble description." } : x).ToArray()
				} };
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = OutfitOwnershipState(context);
		new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, OutfitOwnershipState(context));
		InstallSkins(context, catalogue);
		var after = OutfitOwnershipState(context);
		if (change is not ("exact-unmanaged" or "stock-update")) Assert.AreEqual(before, after);
		Assert.AreEqual(2, context.OutfitTemplates.Count());
		Assert.AreEqual(id, context.SeederManagedRecords.Single(x => x.EntityType == "outfit" && x.StableKey == "fixture_second").LogicalId);
		context.ChangeTracker.Clear();
		InstallSkins(context, catalogue);
		Assert.AreEqual(after, OutfitOwnershipState(context));
	}

	[TestMethod]
	public void OutfitOwnership_NewBatchDuplicateNamesRejectBeforeCreatingAnyBase()
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var original = CatalogueWithTwoOutfits();
		var catalogue = original with { Clothing = original.Clothing with
		{
			Outfits = original.Clothing.Outfits.Select(x => x with { Name = "Same prospective name" }).ToArray()
		} };
		var before = OutfitOwnershipState(context);
		var error = Assert.ThrowsException<InvalidDataException>(() => new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "already uses");
		Assert.AreEqual(0, context.GameItemProtos.Count());
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, OutfitOwnershipState(context));
	}

	[TestMethod]
	public void OutfitOwnership_DirectWriterRejectsWholeBatchBeforeUpdatingEarlierTemplate()
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var catalogue = CatalogueWithTwoOutfits();
		InstallSkins(context, catalogue);
		var later = context.OutfitTemplates.Single(x => x.Name == "Fixture second ensemble");
		later.Description += "\nBuilder-owned conflicting prose.";
		context.SeederManagedRecords.Remove(context.SeederManagedRecords.Single(x => x.EntityType == "outfit" && x.StableKey == "fixture_second"));
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var seeder = new ItemSeeder(Catalogue());
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		var before = OutfitOwnershipState(context);
		var specType = typeof(ItemSeeder).GetNestedType("OutfitManifestSpec", BindingFlags.NonPublic)!;
		var specs = Array.CreateInstance(specType, 2);
		for (var i = 0; i < 2; i++)
		{
			var outfit = catalogue.Clothing.Outfits[i];
			var entry = catalogue.Clothing.OutfitEntries.Single(x => x.OutfitReference == outfit.StableReference);
			ItemSeeder.OutfitManifestItemSpec[] entries =
			[
				new(Garters, null) { EntryKey = entry.EntryKey, WearProfile = entry.WearProfile, LoadArguments = "colour=blue" }
			];
			specs.SetValue(Activator.CreateInstance(specType, outfit.StableReference, outfit.Name,
				"Changed source prose that must not be applied to the earlier template.", entries), i);
		}
		var method = typeof(ItemSeeder).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
			.Single(x => x.Name == "UpsertOutfitManifests" && x.GetParameters().Length == 1);
		var error = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(seeder, [specs]));
		StringAssert.Contains(error.InnerException!.Message, "Unmanaged outfit conflict");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, OutfitOwnershipState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
	}

	[TestMethod]
	public void OutfitOwnership_RenamedManagedSkinRetainsIdentityDuringOutfitSourceUpdate()
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var original = CatalogueWithTwoOutfits();
		var catalogue = original with { Clothing = original.Clothing with
		{
			Skins = CatalogueWithSkins().Clothing.Skins,
			OutfitEntries = original.Clothing.OutfitEntries.Select(x => x with { SkinReference = "fixture_skin_first" }).ToArray()
		} };
		InstallSkins(context, catalogue);
		context.ChangeTracker.Clear();
		var skin = context.GameItemSkins.Single(x => x.Name == "fixture_skin_first");
		var skinId = skin.Id;
		skin.Name = "Builder's renamed presentation";
		context.SaveChanges();
		context.ChangeTracker.Clear();
		catalogue = catalogue with { Clothing = catalogue.Clothing with
		{
			Outfits = catalogue.Clothing.Outfits.Select(x => x with { Description = "An updated stock ensemble description." }).ToArray()
		} };
		var before = OutfitOwnershipState(context);
		new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, OutfitOwnershipState(context));
		InstallSkins(context, catalogue);
		Assert.AreEqual("Builder's renamed presentation", context.GameItemSkins.Single(x => x.Id == skinId).Name);
		foreach (var outfit in context.OutfitTemplates.Include(x => x.OutfitTemplateItems))
		{
			StringAssert.Contains(outfit.Description, "An updated stock ensemble description.");
			Assert.AreEqual(skinId, outfit.OutfitTemplateItems.Single().SkinId);
		}
		var after = OutfitOwnershipState(context);
		context.ChangeTracker.Clear();
		InstallSkins(context, catalogue);
		Assert.AreEqual(after, OutfitOwnershipState(context));
	}

	private static IndustrialisedItemCatalogueDocument CatalogueWithTwoOutfits()
	{
		var original = CatalogueWithOutfit(1);
		var document = original.Clothing;
		var keys = new[] { "fixture_first", "fixture_second" };
		return original with { Clothing = document with
		{
			Outfits = keys.Select((key, index) => document.Outfits.Single() with
			{
				Source = new("Clothing/outfits.tsv", index + 2), StableReference = key,
				Name = index == 0 ? "Fixture first ensemble" : "Fixture second ensemble"
			}).ToArray(),
			OutfitEntries = keys.Select(key => document.OutfitEntries.Single() with { OutfitReference = key }).ToArray(),
			OutfitColours = keys.Select(key => document.OutfitColours.Single() with { OutfitReference = key }).ToArray()
		} };
	}

	private static string OutfitOwnershipState(FuturemudDatabaseContext context) => System.Text.Json.JsonSerializer.Serialize(new
	{
		ItemsSkinsOwnership = SkinState(context),
		Outfits = context.OutfitTemplates.Include(x => x.OutfitTemplateItems).AsNoTracking().OrderBy(x => x.Id).AsEnumerable()
			.Select(x => new
			{
				x.Id, x.Name, x.Description, x.Exclusivity,
				Entries = x.OutfitTemplateItems.OrderBy(y => y.Id).Select(y => new
				{
					y.Id, y.TemplateKey, y.GameItemProtoId, y.SkinId, y.WearProfileId, y.WearOrder, y.Placement, y.ContainerKey, y.LoadArguments
				}).ToArray()
			}).ToArray()
	});
}
