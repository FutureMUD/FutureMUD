#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using GameItemProto = MudSharp.Models.GameItemProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingOutfitPersistenceTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
	private static Dictionary<string, GameItemProto> Items() => new()
	{
		["coat"] = new() { Id = 101, Name = "coat", UniqueName = "coat", ShortDescription = "a coat" },
		["pouch"] = new() { Id = 102, Name = "pouch", UniqueName = "pouch", ShortDescription = "a pouch" }
	};
	private static ItemSeeder.OutfitManifestItemSpec[] Entries() =>
	[
		new("coat", null) { EntryKey = "outer", LoadArguments = "colour=blue" },
		new("pouch", null) { EntryKey = "pocket", Placement = OutfitTemplateItemPlacement.Container, ContainerKey = "outer" }
	];
	private static void Apply(FuturemudDatabaseContext context, ItemSeeder.OutfitManifestItemSpec[]? entries = null,
		string description = "A coordinated ensemble.")
	{
		ItemSeeder.ReconcileOutfitForTesting(context, "test_outfit", description, entries ?? Entries(), Items());
		context.SaveChanges();
	}

	[TestMethod]
	public void RichEntries_PersistReloadRerunAndUpdateWithoutDuplicates()
	{
		using var context = Context();
		Apply(context);
		var originalId = context.OutfitTemplates.Single().Id;
		context.ChangeTracker.Clear();
		Apply(context);
		var unchanged = context.OutfitTemplates.Include(x => x.OutfitTemplateItems).Single();
		Assert.AreEqual(originalId, unchanged.Id);
		Assert.AreEqual(2, unchanged.OutfitTemplateItems.Count);
		var ordered = unchanged.OutfitTemplateItems.OrderBy(x => x.WearOrder).ToArray();
		Assert.AreEqual("outer", ordered[0].TemplateKey);
		Assert.AreEqual(101, ordered[0].GameItemProtoId);
		Assert.IsNull(ordered[0].SkinId);
		Assert.AreEqual("colour=blue", ordered[0].LoadArguments);
		Assert.AreEqual("outer", ordered[1].ContainerKey);
		Assert.AreEqual((int)OutfitTemplateItemPlacement.Container, ordered[1].Placement);
		Assert.AreEqual(1, ordered[1].WearOrder);
		var updated = Entries();
		updated[0] = updated[0] with { LoadArguments = "colour=cream" };
		Apply(context, updated, "Updated stock description.");
		Assert.AreEqual(1, context.OutfitTemplates.Count());
		Assert.AreEqual(2, context.OutfitTemplateItems.Count());
		Assert.AreEqual("colour=cream", unchanged.OutfitTemplateItems.Single(x => x.TemplateKey == "outer").LoadArguments);
		StringAssert.Contains(unchanged.Description, "Updated stock description.");
	}

	[DataTestMethod]
	[DataRow("colour")]
	[DataRow("wear-profile")]
	[DataRow("placement")]
	[DataRow("container")]
	[DataRow("key")]
	[DataRow("order")]
	[DataRow("item")]
	[DataRow("skin")]
	public void CustomizedOwnedFields_PreserveWholeAggregate(string field)
	{
		using var context = Context();
		Apply(context);
		var outfit = context.OutfitTemplates.Include(x => x.OutfitTemplateItems).Single();
		var entry = outfit.OutfitTemplateItems.Single(x => x.TemplateKey == "outer");
		switch (field)
		{
			case "colour": entry.LoadArguments = "colour=red"; break;
			case "wear-profile": entry.WearProfileId = 99; break;
			case "placement": entry.Placement = (int)OutfitTemplateItemPlacement.Room; break;
			case "container": entry.ContainerKey = "custom"; break;
			case "key": entry.TemplateKey = "custom"; break;
			case "order": entry.WearOrder = 5; break;
			case "item": entry.GameItemProtoId = 102; break;
			case "skin": entry.SkinId = 99; break;
		}
		context.SaveChanges();
		Apply(context, description: "Source changed after customization.");
		Assert.IsFalse(outfit.Description.Contains("Source changed", StringComparison.Ordinal));
		Assert.AreSame(entry, outfit.OutfitTemplateItems.Single(x => x.Id == entry.Id));
	}

	[DataTestMethod]
	[DataRow("missing-profile")]
	[DataRow("missing-container")]
	[DataRow("cycle")]
	[DataRow("unexpected-container")]
	[DataRow("duplicate-key")]
	[DataRow("invalid-placement")]
	public void InvalidEntryGraph_DoesNotMutateExistingOutfit(string defect)
	{
		using var context = Context();
		Apply(context);
		var before = context.OutfitTemplateItems.Select(x => x.Id).OrderBy(x => x).ToArray();
		var entries = Entries();
		entries[0] = defect switch
		{
			"missing-profile" => entries[0] with { WearProfile = "absent" },
			"cycle" => entries[0] with { Placement = OutfitTemplateItemPlacement.Container, ContainerKey = "pocket" },
			"unexpected-container" => entries[0] with { ContainerKey = "pocket" },
			"duplicate-key" => entries[0] with { EntryKey = "pocket" },
			"invalid-placement" => entries[0] with { Placement = (OutfitTemplateItemPlacement)99 },
			_ => entries[0]
		};
		if (defect == "missing-container") entries[1] = entries[1] with { ContainerKey = null };
		Assert.ThrowsException<InvalidOperationException>(() => Apply(context, entries));
		Assert.IsFalse(context.ChangeTracker.HasChanges());
		CollectionAssert.AreEqual(before, context.OutfitTemplateItems.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void RepeatedPrototypeWithDistinctEntryKeys_IsDeliberateNotDuplicateStock()
	{
		using var context = Context();
		Apply(context, [new("coat", null) { EntryKey = "first" }, new("coat", null) { EntryKey = "second" }]);
		Assert.AreEqual(2, context.OutfitTemplateItems.Count());
		Assert.AreEqual(1, context.OutfitTemplateItems.Select(x => x.GameItemProtoId).Distinct().Count());
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void LegacyOutfitFingerprint_UpgradesOnlyUnchangedDefaultComposition(bool customized)
	{
		using var context = Context();
		Apply(context, [new("coat", null)]);
		var outfit = context.OutfitTemplates.Include(x => x.OutfitTemplateItems).Single();
		var managed = context.SeederManagedRecords.Single();
		managed.LogicalId = null; // Original insertion records can predate the database-generated template ID.
		managed.AppliedFingerprint = ItemSeederManifestCatalogue.Fingerprint(new
		{
			StableKey = "test_outfit", outfit.Name, outfit.Description, outfit.Exclusivity,
			Items = new[] { new { ItemStableReference = "coat", SkinStableReference = (string?)null } }
		});
		if (customized) outfit.OutfitTemplateItems.Single().LoadArguments = "colour=red";
		context.SaveChanges();
		Apply(context, [new("coat", null)], "Updated stock.");
		Assert.AreEqual(!customized, outfit.Description.Contains("Updated stock.", StringComparison.Ordinal));
	}
}
