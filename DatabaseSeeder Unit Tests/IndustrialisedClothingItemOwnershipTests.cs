#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
	[DataRow("unmanaged", "Unmanaged clothing item conflict")]
	[DataRow("wrong-owner", "ownership conflict")]
	[DataRow("other-owner", "claimed by another aggregate")]
	[DataRow("not-current", "exactly one current revision")]
	[DataRow("duplicate-current", "exactly one current revision")]
	[DataRow("custom-components", "Variable")]
	[DataRow("custom-material", "resolvable solid material")]
	public void ClothingItemOwnership_RejectsLaterConflictBeforeAnyEarlierMutation(string fault, string diagnostic)
	{
		using var context = Context();
		SeedPrerequisites(context);
		var catalogue = CatalogueWithNewClothing();
		InstallNewClothing(context, catalogue);
		var second = context.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos)
			.Single(x => x.UniqueName == "industrial_clothing_fixture_second");
		var record = context.SeederManagedRecords.Single(x => x.EntityType == "item" && x.StableKey == second.UniqueName);
		switch (fault)
		{
			case "unmanaged": context.SeederManagedRecords.Remove(record); second.FullDescription += " Builder alteration."; break;
			case "wrong-owner": record.LogicalId = 987654; break;
			case "other-owner": context.SeederManagedRecords.Add(new() { Seeder = "Items", EntityType = "item", StableKey = "other_item", LogicalId = second.Id }); break;
			case "not-current": second.EditableItem.RevisionStatus = (int)RevisionStatus.Obsolete; break;
			case "duplicate-current":
				var duplicate = (GameItemProto)context.Entry(second).CurrentValues.ToObject();
				duplicate.RevisionNumber++;
				duplicate.EditableItem = Component(900, duplicate.RevisionNumber, "fixture").EditableItem;
				context.GameItemProtos.Add(duplicate);
				break;
			case "custom-material": second.MaterialId = 987654; break;
			case "custom-components":
				second.FullDescription += " Deliberate builder changes.";
				var variableId = context.GameItemComponentProtos.Single(x => x.Type == "Variable").Id;
				context.GameItemProtosGameItemComponentProtos.Remove(second.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemComponentProtoId == variableId));
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = NewClothingState(context);
		catalogue = catalogue with { Items = catalogue.Items.Select(x => x with { FullDescription = x.FullDescription + " New source prose." }).ToArray() };
		var seeder = new ItemSeeder(catalogue);
		var error = Assert.ThrowsException<InvalidDataException>(() => seeder.ValidateClothingPrerequisitesForTesting(context, "industrial"));
		StringAssert.Contains(error.Message, "Clothing/bases.tsv:3");
		StringAssert.Contains(error.Message, diagnostic);
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, NewClothingState(context));
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
		Assert.ThrowsException<InvalidOperationException>(() => seeder.ApplyClothingReuseForTesting("industrial"));
	}

	[DataTestMethod]
	[DataRow("renamed")]
	[DataRow("custom-prose")]
	[DataRow("exact-unmanaged")]
	[DataRow("source-update")]
	[DataRow("repair-variable")]
	public void ClothingItemOwnership_UsesSameTargetAndProjectionOnFreshInstallAndRerun(string change)
	{
		using var context = Context();
		SeedPrerequisites(context);
		var catalogue = CatalogueWithNewClothing();
		InstallNewClothing(context, catalogue);
		var target = context.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos)
			.Single(x => x.UniqueName == "industrial_clothing_fixture_second");
		var id = target.Id;
		var stockDescription = target.FullDescription;
		switch (change)
		{
			case "renamed": target.UniqueName = "builder_renamed_garment"; break;
			case "custom-prose": target.FullDescription += " Builder-authored finishing."; break;
			case "exact-unmanaged": context.SeederManagedRecords.Remove(context.SeederManagedRecords.Single(x => x.EntityType == "item" && x.LogicalId == id)); break;
			case "source-update": catalogue = catalogue with { Items = catalogue.Items.Select(x => x with { FullDescription = x.FullDescription + " Reviewed source finishing." }).ToArray() }; break;
			case "repair-variable":
				var variableId = context.GameItemComponentProtos.Single(x => x.Type == "Variable").Id;
				context.GameItemProtosGameItemComponentProtos.Remove(target.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemComponentProtoId == variableId));
				break;
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();
		var before = NewClothingState(context);
		new ItemSeeder(catalogue).ValidateClothingPrerequisitesForTesting(context, "industrial");
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(before, NewClothingState(context));
		InstallNewClothing(context, catalogue);
		Assert.AreEqual(2, context.GameItemProtos.Count());
		Assert.AreEqual(id, context.SeederManagedRecords.Single(x => x.EntityType == "item" && x.StableKey == "industrial_clothing_fixture_second").LogicalId);
		var updated = context.GameItemProtos.Single(x => x.Id == id);
		if (change == "renamed") Assert.AreEqual("builder_renamed_garment", updated.UniqueName);
		if (change == "custom-prose") Assert.AreEqual(stockDescription + " Builder-authored finishing.", updated.FullDescription);
		if (change == "source-update") Assert.AreEqual(stockDescription + " Reviewed source finishing.", updated.FullDescription);
		Assert.AreEqual(ItemSeeder.FindHistoricalClothingSource(Garters)!.Components.Count,
			context.GameItemProtosGameItemComponentProtos.Count(x => x.GameItemProtoId == id));
		var after = NewClothingState(context);
		context.ChangeTracker.Clear();
		InstallNewClothing(context, catalogue);
		Assert.AreEqual(after, NewClothingState(context));
	}

	[TestMethod]
	public void ClothingItemOwnership_WriterRechecksLaterTargetBeforeEarlierSourceUpdate()
	{
		using var context = Context();
		SeedPrerequisites(context);
		var catalogue = CatalogueWithNewClothing();
		InstallNewClothing(context, catalogue);
		catalogue = catalogue with { Items = catalogue.Items.Select(x => x with { FullDescription = x.FullDescription + " Reviewed source update." }).ToArray() };
		var seeder = new ItemSeeder(catalogue);
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		context.SeederManagedRecords.Single(x => x.StableKey == "industrial_clothing_fixture_second").LogicalId = 987654;
		context.SaveChanges();
		var before = NewClothingState(context);
		var error = Assert.ThrowsException<TargetInvocationException>(() => typeof(ItemSeeder)
			.GetMethod("SeedIndustrialisedCatalogueItems", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, ["industrial"]));
		StringAssert.Contains(error.InnerException!.Message, "ownership conflict");
		Assert.AreEqual(before, NewClothingState(context));
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		Assert.AreEqual(0, seeder.GetCapturedManifestEntriesForTesting().Count);
	}

	private static IndustrialisedItemCatalogueDocument CatalogueWithNewClothing()
	{
		var original = Catalogue();
		var source = ItemSeeder.FindHistoricalClothingSource(Garters)!;
		var keys = new[] { "industrial_clothing_fixture_first", "industrial_clothing_fixture_second" };
		return original with
		{
			Items = keys.Select((key, index) => ItemSeeder.IndustrialisedCatalogueForTesting.Items.First() with
			{
				Source = "Industrial/clothing.tsv", Line = index + 2, StableReference = key, Layer = "industrial", Domain = "clothing",
				EraAdmissions = ["industrial"], Noun = source.Noun, ShortDescription = source.ShortDescription, FullDescription = source.FullDescription,
				Material = source.Material, Tags = source.Tags.ToArray(), FixedComponents = source.Components.ToArray(), ProfileBindings = [], SupportedClaims = [],
				Size = source.Size, Quality = source.Quality, WeightGrams = source.WeightInGrams, CostIndex = source.Cost,
				MorphTo = null, MorphEmote = null, MorphSeconds = 0, DestroyedItem = null, Craftable = false, LifecycleKind = null
			}).ToArray(),
			Clothing = original.Clothing with
			{
				Bases = keys.Select((key, index) => original.Clothing.Bases.Single() with { ItemReference = key, Source = new("Clothing/bases.tsv", index + 2) }).ToArray(),
				Colours = keys.Select(key => original.Clothing.Colours.Single() with { PresentationReference = key }).ToArray()
			}
		};
	}

	private static void InstallNewClothing(FuturemudDatabaseContext context, IndustrialisedItemCatalogueDocument catalogue)
	{
		var seeder = new ItemSeeder(catalogue);
		seeder.ValidateClothingPrerequisitesForTesting(context, "industrial");
		typeof(ItemSeeder).GetMethod("SeedIndustrialisedCatalogueItems", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, ["industrial"]);
		typeof(ItemSeeder).GetMethod("SaveManifestChanges", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(seeder, null);
	}

	private static string NewClothingState(FuturemudDatabaseContext context) => System.Text.Json.JsonSerializer.Serialize(new
	{
		Items = context.GameItemProtos.AsNoTracking().OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber)
			.Select(x => new { x.Id, x.RevisionNumber, x.UniqueName, x.MaterialId, x.FullDescription }).ToArray(),
		Links = context.GameItemProtosGameItemComponentProtos.AsNoTracking().OrderBy(x => x.GameItemProtoId).ThenBy(x => x.GameItemComponentProtoId)
			.Select(x => new { x.GameItemProtoId, x.GameItemProtoRevision, x.GameItemComponentProtoId, x.GameItemComponentRevision }).ToArray(),
		Ownership = context.SeederManagedRecords.AsNoTracking().OrderBy(x => x.Id)
			.Select(x => new { x.Id, x.StableKey, x.LogicalId, x.AppliedFingerprint }).ToArray()
	});
}
