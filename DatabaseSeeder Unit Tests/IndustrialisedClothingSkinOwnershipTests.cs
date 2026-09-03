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
using MudSharp.Framework.Revision;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class IndustrialisedClothingReuseTests
{
	[DataTestMethod]
	[DataRow("unmanaged-drift", "Unmanaged item-skin conflict")]
	[DataRow("wrong-owner-id", "ownership conflict")]
	[DataRow("duplicate-current", "ambiguous current skin")]
	[DataRow("custom-base", "different base")]
	[DataRow("no-current", "no current revision")]
	[DataRow("unmanaged-no-current", "no current revision")]
	[DataRow("other-owner", "is claimed by")]
	public void SkinOwnership_RejectsLaterBatchConflictBeforeAnyMutation(string fault, string diagnostic)
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var catalogue = CatalogueWithSkins();
		InstallSkins(context, catalogue);
		var skin = context.GameItemSkins.Include(x => x.EditableItem).Single(x => x.Name == "fixture_skin_second");
		var managed = context.SeederManagedRecords.Single(x => x.EntityType == "item-skin" && x.StableKey == skin.Name);
		switch (fault)
		{
			case "unmanaged-drift": context.SeederManagedRecords.Remove(managed); skin.FullDescription = "Builder-authored different text."; break;
			case "wrong-owner-id": managed.LogicalId = 987654; break;
			case "duplicate-current":
				context.GameItemSkins.Add(new GameItemSkin
				{
					Id = 654321, RevisionNumber = 0, Name = skin.Name, ItemProtoId = skin.ItemProtoId,
					EditableItem = Component(654321, 0, "fixture").EditableItem
				});
				break;
			case "custom-base": skin.ItemProtoId = 456789; break;
			case "no-current": skin.EditableItem.RevisionStatus = (int)RevisionStatus.Obsolete; break;
			case "unmanaged-no-current":
				context.SeederManagedRecords.Remove(managed);
				skin.EditableItem.RevisionStatus = (int)RevisionStatus.Obsolete;
				break;
			case "other-owner":
				context.SeederManagedRecords.Add(new SeederManagedRecord
				{
					Seeder = "Items", EntityType = "item-skin", StableKey = "another_claim", Module = "outfits", LogicalId = skin.Id
				});
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = SkinState(context);
		var seeder = new ItemSeeder(catalogue);
		var error = Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/skins.tsv:3");
		StringAssert.Contains(error.Message, diagnostic);
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, SkinState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
		Assert.ThrowsException<InvalidOperationException>(() => seeder.ApplyClothingReuseForTesting("industrial"));
	}

	[DataTestMethod]
	[DataRow("exact-unmanaged")]
	[DataRow("custom-name")]
	[DataRow("custom-description")]
	[DataRow("stock-update")]
	[DataRow("later-revision")]
	[DataRow("renamed-later-revision")]
	public void SkinOwnership_ReadOnlyPreflightAndRerunPreserveIdentityAndCompatibleCustomization(string change)
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var catalogue = CatalogueWithSkins();
		InstallSkins(context, catalogue);
		var skin = context.GameItemSkins.Include(x => x.EditableItem).Single(x => x.Name == "fixture_skin_second");
		var id = skin.Id;
		var managed = context.SeederManagedRecords.Single(x => x.EntityType == "item-skin" && x.StableKey == skin.Name);
		switch (change)
		{
			case "exact-unmanaged": context.SeederManagedRecords.Remove(managed); break;
			case "custom-name": skin.Name = "Builder's renamed skin"; break;
			case "custom-description": skin.FullDescription = "The $colour woven bands bear a builder's embroidered motif."; break;
			case "stock-update":
				catalogue = catalogue with { Clothing = catalogue.Clothing with
				{
					Skins = catalogue.Clothing.Skins.Select(x => x.StableReference == skin.Name
						? x with { FullDescription = "A line of closely worked embroidery runs along each $colour band." } : x).ToArray()
				} };
				break;
			case "later-revision":
			case "renamed-later-revision":
				skin.EditableItem.RevisionStatus = (int)RevisionStatus.Obsolete;
				context.GameItemSkins.Add(new GameItemSkin
				{
					Id = id, RevisionNumber = 1, Name = change == "renamed-later-revision" ? "Builder's renamed skin" : skin.Name, ItemProtoId = skin.ItemProtoId,
					ItemName = skin.ItemName, ShortDescription = skin.ShortDescription, FullDescription = skin.FullDescription,
					CanUseSkinProgId = skin.CanUseSkinProgId, Quality = skin.Quality, IsPublic = skin.IsPublic,
					EditableItem = Component(id, 1, "fixture").EditableItem
				});
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = SkinState(context);
		new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, SkinState(context), "Preflight must not adopt an exact unmanaged skin or mark a customization.");
		InstallSkins(context, catalogue);
		var current = context.GameItemSkins.Include(x => x.EditableItem).Single(x => x.Id == id && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		Assert.AreEqual(change is "custom-name" or "renamed-later-revision" ? "Builder's renamed skin" : "fixture_skin_second", current.Name);
		Assert.AreEqual(change == "custom-description" ? "The $colour woven bands bear a builder's embroidered motif."
			: catalogue.Clothing.Skins.Last().FullDescription, current.FullDescription);
		Assert.IsNull(current.Quality);
		Assert.AreEqual(2, context.GameItemSkins.Select(x => x.Id).Distinct().Count());
		Assert.AreEqual(id, context.SeederManagedRecords.Single(x => x.EntityType == "item-skin" && x.StableKey == "fixture_skin_second").LogicalId);
		var applied = SkinState(context);
		context.ChangeTracker.Clear();
		InstallSkins(context, catalogue);
		Assert.AreEqual(applied, SkinState(context), "An identical rerun retains skin revisions and complete provenance signatures.");
	}

	[TestMethod]
	public void SkinWriter_ValidatesWholeDirectBatchBeforeUpdatingItsFirstSkin()
	{
		using var context = Context();
		SeedSkinPrerequisites(context);
		var catalogue = CatalogueWithSkins();
		InstallSkins(context, catalogue);
		var later = context.GameItemSkins.Single(x => x.Name == "fixture_skin_second");
		later.FullDescription = "Builder-owned conflicting prose.";
		context.SeederManagedRecords.Remove(context.SeederManagedRecords.Single(x => x.EntityType == "item-skin" && x.StableKey == later.Name));
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var seeder = new ItemSeeder(Catalogue());
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		var before = SkinState(context);
		var specType = typeof(ItemSeeder).GetNestedType("DocumentedClothingSkinSpec", BindingFlags.NonPublic)!;
		var specs = Array.CreateInstance(specType, 2);
		for (var i = 0; i < 2; i++)
		{
			var skin = catalogue.Clothing.Skins[i];
			specs.SetValue(Activator.CreateInstance(specType, skin.StableReference, Garters, skin.Noun,
				skin.ShortDescription, "Changed source prose that must not be applied to the earlier skin.", null), i);
		}
		var method = typeof(ItemSeeder).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
			.Single(x => x.Name == "SeedDocumentedClothingSkins" && x.GetParameters()[0].ParameterType.GenericTypeArguments.Single() == specType);
		var error = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(seeder, [specs]));
		StringAssert.Contains(error.InnerException!.Message, "Unmanaged item-skin conflict");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, SkinState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
	}

	private static IndustrialisedItemCatalogueDocument CatalogueWithSkins()
	{
		var catalogue = Catalogue();
		var skin = IndustrialisedClothingCatalogueTests.Load(IndustrialisedClothingCatalogueTests.Fixture()).Skins.Single();
		return catalogue with { Clothing = catalogue.Clothing with
		{
			Skins = new[] { "fixture_skin_first", "fixture_skin_second" }.Select((key, index) => skin with
			{
				Source = new("Clothing/skins.tsv", index + 2), StableReference = key, BaseItemReference = Garters,
				EraAdmissions = ["industrial"], ReviewStatus = ClothingReviewStatus.Reviewed
			}).ToArray()
		} };
	}

	private static void SeedSkinPrerequisites(FuturemudDatabaseContext context)
	{
		SeedPrerequisites(context);
		context.FutureProgs.Add(new FutureProg
		{
			Id = 1, FunctionName = "AlwaysTrue", FunctionComment = "Fixture", FunctionText = "return true",
			ReturnType = 4, Category = "Fixture", Subcategory = "Fixture"
		});
		context.SaveChanges();
	}

	private static void InstallSkins(FuturemudDatabaseContext context, IndustrialisedItemCatalogueDocument catalogue)
	{
		var seeder = new ItemSeeder(catalogue);
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		seeder.ApplyClothingReuseForTesting("industrial");
		context.SaveChanges();
		typeof(ItemSeeder).GetMethod("SeedIndustrialisedClothingPresentations", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(seeder, ["industrial"]);
		typeof(ItemSeeder).GetMethod("SaveManifestChanges", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(seeder, null);
	}

	private static string SkinState(FuturemudDatabaseContext context) => System.Text.Json.JsonSerializer.Serialize(new
	{
		Skins = context.GameItemSkins.AsNoTracking().OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber)
			.Select(x => new { x.Id, x.RevisionNumber, x.Name, x.ItemProtoId, x.FullDescription, x.Quality }).ToArray(),
		Ownership = context.SeederManagedRecords.AsNoTracking().OrderBy(x => x.Id)
			.Select(x => new { x.Id, x.EntityType, x.StableKey, x.LogicalId, x.AppliedFingerprint }).ToArray(),
		Items = context.GameItemProtos.AsNoTracking().OrderBy(x => x.Id).Select(x => new { x.Id, x.UniqueName, x.FullDescription }).ToArray()
	});
}
