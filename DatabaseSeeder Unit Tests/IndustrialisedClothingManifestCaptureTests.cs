#nullable enable

using System;
using System.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingManifestCaptureTests
{
	[TestMethod]
	public void NonemptyAuthoredGraph_CapturesItemsSkinsOutfitsAndCraftsWithoutInstalledPrerequisites()
	{
		var sources = IndustrialisedClothingCatalogueTests.Fixture();
		IndustrialisedClothingCatalogueTests.ReplaceCell(sources, "bases.tsv", 6, "Reviewed");
		IndustrialisedClothingCatalogueTests.ReplaceCell(sources, "skins.tsv", 10, "Reviewed");
		IndustrialisedClothingCatalogueTests.ReplaceCell(sources, "outfits.tsv", 4, "Reviewed");
		IndustrialisedClothingCatalogueTests.ReplaceCell(sources, "crafts.tsv", 15, "Reviewed");
		var clothing = IndustrialisedClothingCatalogueTests.Load(sources);
		clothing = clothing with
		{
			CraftProducts = clothing.CraftProducts.Where(x => !x.FailureProduct).ToArray(),
			CraftPhases = clothing.CraftPhases.Select(x => x.Order == 2
				? x with { FailEcho = "$0 stop|stops the failed work." }
				: x).ToArray()
		};
		var original = ItemSeeder.IndustrialisedCatalogueForTesting;
		var physical = original.Items.First() with
		{
			StableReference = "coat",
			Layer = "industrial",
			Domain = "Clothing, footwear and uniforms",
			EraAdmissions = ["industrial", "modern"],
			Noun = "coat",
			ShortDescription = "a plain $colour coat",
			FullDescription = "This complete unskinned coat has a practical cut in $colour cloth.",
			Material = "cotton",
			FixedComponents = [],
			ProfileBindings = [],
			SupportedClaims = [],
			MorphTo = null,
			MorphSeconds = 0,
			MorphEmote = null,
			DestroyedItem = null,
			LifecycleKind = null
		};
		var catalogue = original with
		{
			Items = [physical],
			Crafts = [],
			Outfits = [],
			Clothing = clothing
		};

		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		context.Accounts.Add(new Account { Id = 1, Name = "Manifest test", CultureName = "en-AU", TimeZoneId = "UTC", UnitPreference = "Metric" });
		context.TraitDefinitions.Add(new TraitDefinition
		{
			Id = 1, Name = "Crafting", Type = 0, OwnerScope = 0, TraitGroup = "Crafting",
			ChargenBlurb = string.Empty, ValueExpression = string.Empty
		});
		context.SaveChanges();
		var manifest = new ItemSeeder(catalogue).CaptureManifest(context, ItemSeederManifestCatalogue.FindRepositoryRoot());

		Assert.AreEqual(1, manifest.Entries.Count(x => x.EntityType == "item" && x.StableKey == "coat"));
		Assert.AreEqual(1, manifest.Entries.Count(x => x.EntityType == "item-skin" && x.StableKey == "trimmed_coat"));
		Assert.AreEqual(1, manifest.Entries.Count(x => x.EntityType == "outfit" && x.StableKey == "test_outfit"));
		Assert.AreEqual(1, manifest.Entries.Count(x => x.EntityType == "craft" && x.StableKey == "sew_coat"));
		CollectionAssert.AreEquivalent(new[] { "item:coat", "item-skin:trimmed_coat" }, manifest.Entries
			.Single(x => x.EntityType == "outfit" && x.StableKey == "test_outfit").Dependencies.ToArray());
	}

	[TestMethod]
	public void ManifestOutfitBinding_UsesStableSymbolicColoursAndKeepsUnskinnedBasesValid()
	{
		var sources = IndustrialisedClothingCatalogueTests.Fixture();
		IndustrialisedClothingCatalogueTests.ReplaceCell(sources, "outfit-entries.tsv", 4, string.Empty);
		var clothing = IndustrialisedClothingCatalogueTests.Load(sources);
		var result = ItemSeeder.BindClothingOutfitEntryForManifestCapture(clothing, clothing.OutfitEntries.Single());

		Assert.IsNull(result.SkinStableReference);
		Assert.AreEqual("colour=[Garment Colour/cream]", result.LoadArguments);
		Assert.AreEqual("Coat", result.WearProfile);
	}
}
