#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederReworkMetadataTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void BuildReworkItemBuilderNotes_RecordsStableReferenceCultureAndCategory()
	{
		var notes = ItemSeeder.BuildReworkItemBuilderNotesForTesting(
			"antiquity_food_hellenic_flatbread",
			["Food and Drink / Antiquity Food / Prepared Foods"],
			"Crafted through Hellenic Foodways.");

		StringAssert.Contains(notes, "Stock unique reference: antiquity_food_hellenic_flatbread.");
		StringAssert.Contains(notes, "Cultures: Hellenic.");
		StringAssert.Contains(notes, "Seeder category: antiquity food and beverage stock.");
		StringAssert.Contains(notes, "Crafted through Hellenic Foodways.");
	}

	[TestMethod]
	public void BuildReworkItemBuilderNotes_RecordsSharedCultureMembership()
	{
		var notes = ItemSeeder.BuildReworkItemBuilderNotesForTesting(
			"adjacent_antiquity_narrow_linen_kilt",
			[]);

		StringAssert.Contains(notes, "Cultures: Egyptian, Kushite.");
	}

	[TestMethod]
	public void BuildReworkItemBuilderNotes_RecordsMedievalCultureStatusAndCategory()
	{
		var notes = ItemSeeder.BuildReworkItemBuilderNotesForTesting(
			"medieval_clothing_norman_noble_silk_surcoat",
			["Market / Clothing / Luxury Clothing"]);

		StringAssert.Contains(notes, "Stock unique reference: medieval_clothing_norman_noble_silk_surcoat.");
		StringAssert.Contains(notes, "Cultures: Norman/Angevin.");
		StringAssert.Contains(notes, "Status/role: Noble/Court.");
		StringAssert.Contains(notes, "Seeder category: medieval clothing stock.");
	}

	[TestMethod]
	public void BuildReworkItemBuilderNotes_RecordsHistoricFoundationScope()
	{
		var notes = ItemSeeder.BuildReworkItemBuilderNotesForTesting(
			"historic_sewing_needle",
			["Market / Professional Tools / Standard Tools"]);

		StringAssert.Contains(notes, "Stock unique reference: historic_sewing_needle.");
		StringAssert.Contains(notes, "Shared scope: cross-era historic foundation.");
		StringAssert.Contains(notes, "Seeder category: shared historic foundation stock.");
	}

	[TestMethod]
	public void BuildReworkItemBuilderNotes_RecordsPreIndustrialCultureAndCategory()
	{
		var notes = ItemSeeder.BuildReworkItemBuilderNotesForTesting(
			"preindustrial_printing_hand_press",
			["Market / Professional Tools / Standard Tools"]);

		StringAssert.Contains(notes, "Cultures: Shared Pre-Industrial.");
		StringAssert.Contains(notes, "Seeder category: shared pre-industrial foundation stock.");
	}

	[TestMethod]
	public void BuildReworkItemBuilderNotes_RecordsMedievalRepairAndGapCategories()
	{
		var repairNotes = ItemSeeder.BuildReworkItemBuilderNotesForTesting(
			"medieval_textile_repair_kit",
			["Market / Professional Tools / Standard Tools"]);
		var surveyNotes = ItemSeeder.BuildReworkItemBuilderNotesForTesting(
			"medieval_surveyor_measuring_rope",
			["Market / Professional Tools / Standard Tools"]);
		var gapNotes = ItemSeeder.BuildReworkItemBuilderNotesForTesting(
			"medieval_music_psaltery",
			["Market / Household Goods / Luxury Decorations"]);

		StringAssert.Contains(repairNotes, "Seeder category: medieval repair-kit stock.");
		StringAssert.Contains(surveyNotes, "Seeder category: medieval writing and administration stock.");
		StringAssert.Contains(gapNotes, "Seeder category: medieval component-gap prop stock.");
	}

	[TestMethod]
	public void InferReworkFunctionalTags_MirrorsMarketTags()
	{
		var inferredTags = ItemSeeder.InferReworkFunctionalTagsForTesting(
			[
				"Market / Professional Tools / Standard Tools",
				"Market / Military Goods / Armour / Shields",
				"Market / Household Goods / Luxury Furniture",
				"Market / Writing Materials / Scrolls",
				"Materials / Writing Product"
			]).ToList();

		CollectionAssert.Contains(inferredTags, "Functions / Tools");
		CollectionAssert.Contains(inferredTags, "Functions / Military Equipment");
		CollectionAssert.Contains(inferredTags, "Functions / Military Equipment / Military Armour");
		CollectionAssert.Contains(inferredTags, "Functions / Military Equipment / Military Armour / Military Shields");
		CollectionAssert.Contains(inferredTags, "Functions / Household Items");
		CollectionAssert.Contains(inferredTags, "Functions / Household Items / Household Furniture");
		CollectionAssert.Contains(inferredTags, "Functions / Writing Goods");
	}

	[TestMethod]
	public void RemoveRedundantParentTags_KeepsOnlyMostSpecificTagPerHierarchyBranch()
	{
		var tags = ItemSeeder.RemoveRedundantParentTagsForTesting(
		[
			"Era / Early Modern Era",
			"Functions / Military Equipment",
			"Functions / Military Equipment / Military Weapons",
			"Market / Military Goods",
			"Market / Military Goods / Weapons",
			"Market / Military Goods / Weapons / Spears"
		]);

		CollectionAssert.AreEquivalent(
		new[]
		{
			"Era / Early Modern Era",
			"Functions / Military Equipment / Military Weapons",
			"Market / Military Goods / Weapons / Spears"
		}, tags.ToArray());
	}

	[TestMethod]
	public void CreateReworkItem_AssignsStableReferenceAsUniqueName()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);

		var item = new ItemSeeder().CreateReworkItemForTesting(
			context,
			"antiquity_short_wool_chiton",
			"chiton",
			"a test wool chiton",
			"wool");

		Assert.IsNotNull(item);
		Assert.AreEqual("antiquity_short_wool_chiton", item!.UniqueName);
		Assert.IsNull(item.BuilderNotes);
	}

	[TestMethod]
	public void CreateReworkItem_DriftedLegacyShortDescriptionBlocksWithoutOverwritingBuilderNotes()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		context.GameItemProtos.Add(Item(
			10,
			"chiton",
			"a reused wool chiton",
			"wool",
			null,
			"Existing builder-maintained note."));
		context.SaveChanges();

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			new ItemSeeder().CreateReworkItemForTesting(
				context,
				"antiquity_short_wool_chiton",
				"chiton",
				"a reused wool chiton",
				"wool"));

		StringAssert.Contains(exception.Message, "Unmanaged legacy item conflict");
		var legacyItem = context.GameItemProtos.Single(x => x.Id == 10);
		Assert.IsNull(legacyItem.UniqueName);
		Assert.AreEqual("Existing builder-maintained note.", legacyItem.BuilderNotes);
		Assert.AreEqual(2, context.GameItemProtos.Count());
	}

	[TestMethod]
	public void CreateReworkItem_AddsInferredFunctionalTagsOntoNewStockItems()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);

		var item = new ItemSeeder().CreateReworkItemForTesting(
			context,
			"antiquity_workshop_hammer",
			"hammer",
			"a workshop hammer",
			"wool",
			tags: ["Market / Professional Tools / Standard Tools"]);

		Assert.IsNotNull(item);
		var tagNames = item!.GameItemProtosTags
			.Select(x => context.Tags.Single(tag => tag.Id == x.TagId).Name)
			.ToList();
		CollectionAssert.Contains(tagNames, "Standard Tools");
		CollectionAssert.Contains(tagNames, "Tools");
	}

	[TestMethod]
	public void CreateReworkItem_AdoptsUniqueExactLegacySignature()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var original = new ItemSeeder().CreateReworkItemForTesting(
			context,
			"antiquity_exact_legacy_hammer",
			"hammer",
			"an exact legacy hammer",
			"wool");
		Assert.IsNotNull(original);
		context.SaveChanges();

		context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords);
		original!.UniqueName = null;
		context.SaveChanges();

		var adopted = new ItemSeeder().CreateReworkItemForTesting(
			context,
			"antiquity_exact_legacy_hammer",
			"hammer",
			"an exact legacy hammer",
			"wool");
		context.SaveChanges();

		Assert.IsNotNull(adopted);
		Assert.AreEqual(original.Id, adopted!.Id);
		Assert.AreEqual("antiquity_exact_legacy_hammer", adopted.UniqueName);
		Assert.AreEqual(1, context.GameItemProtos.Count(x => x.ShortDescription == "an exact legacy hammer"));
		Assert.AreEqual(1, context.SeederManagedRecords.Count());
	}

	[TestMethod]
	public void CreateReworkItem_AmbiguousLegacyShortDescriptionBlocksEvenWithOneExactSignature()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var exactCandidate = new ItemSeeder().CreateReworkItemForTesting(
			context,
			"antiquity_ambiguous_legacy_hammer",
			"hammer",
			"an ambiguous legacy hammer",
			"wool");
		Assert.IsNotNull(exactCandidate);
		context.SaveChanges();

		context.SeederManagedRecords.RemoveRange(context.SeederManagedRecords);
		exactCandidate!.UniqueName = null;
		context.GameItemProtos.Add(Item(
			20,
			"other hammer",
			"an ambiguous legacy hammer",
			"wool",
			null,
			"Builder-owned candidate."));
		context.SaveChanges();

		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			new ItemSeeder().CreateReworkItemForTesting(
				context,
				"antiquity_ambiguous_legacy_hammer",
				"hammer",
				"an ambiguous legacy hammer",
				"wool"));

		StringAssert.Contains(exception.Message, "Legacy item adoption");
		StringAssert.Contains(exception.Message, "ambiguous");
		Assert.AreEqual(2, context.GameItemProtos.Count(x => x.ShortDescription == "an ambiguous legacy hammer"));
		Assert.AreEqual(0, context.SeederManagedRecords.Count());
	}

	[TestMethod]
	public void CreateReworkItem_DoesNotCollapseDistinctStableReferencesWithSameShortDescription()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();

		var roman = seeder.CreateReworkItemForTesting(
			context,
			"antiquity_roman_bronze_greaves",
			"greaves",
			"a pair of bronze greaves",
			"wool");
		var hellenic = seeder.CreateReworkItemForTesting(
			context,
			"antiquity_hellenic_bronze_greaves",
			"greaves",
			"a pair of bronze greaves",
			"wool");

		Assert.IsNotNull(roman);
		Assert.IsNotNull(hellenic);
		Assert.AreNotEqual(roman!.Id, hellenic!.Id);
		Assert.AreEqual("antiquity_roman_bronze_greaves", roman.UniqueName);
		Assert.AreEqual("antiquity_hellenic_bronze_greaves", hellenic.UniqueName);
		Assert.IsNull(roman.BuilderNotes);
		Assert.IsNull(hellenic.BuilderNotes);
	}

	[TestMethod]
	public void CreateReworkItem_RerunRemovesSeederNotesButPreservesBuilderNotes()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();
		var item = seeder.CreateReworkItemForTesting(
			context,
			"preindustrial_test_noted_item",
			"item",
			"a shared noted item",
			"wool",
			"Seeder-only source note.");
		Assert.IsNotNull(item);
		item!.BuilderNotes = "Stock unique reference: preindustrial_test_noted_item.\n" +
		                     "Cultures: Shared Pre-Industrial.\n" +
		                     "Seeder-only source note.\n" +
		                     "Builder-authored note.";
		context.SaveChanges();

		var rerun = seeder.CreateReworkItemForTesting(
			context,
			"preindustrial_test_noted_item",
			"item",
			"a shared noted item",
			"wool",
			"Seeder-only source note.");

		Assert.AreSame(item, rerun);
		Assert.AreEqual("Builder-authored note.", rerun!.BuilderNotes);
	}

	[TestMethod]
	public void CreateReworkItem_ReusingStableReferenceIsIdempotent()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();

		var first = seeder.CreateReworkItemForTesting(
			context,
			"preindustrial_test_shared_item",
			"item",
			"a shared test item",
			"wool");
		var second = seeder.CreateReworkItemForTesting(
			context,
			"preindustrial_test_shared_item",
			"item",
			"a shared test item",
			"wool");

		Assert.IsNotNull(first);
		Assert.AreSame(first, second);
		Assert.AreEqual(1, context.GameItemProtos.Local.Count(x => x.UniqueName == "preindustrial_test_shared_item"));
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "SeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
			UseMxp = false,
			UseMsp = false,
			UseMccp = false,
			ActiveCharactersAllowed = 1,
			UseUnicode = true,
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = string.Empty,
			IsRegistered = true,
			RecoveryCode = string.Empty,
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow,
			PageLength = 22,
			PromptType = 0,
			TabRoomDescriptions = false,
			CodedRoomDescriptionAdditionsOnNewLine = false,
			CharacterNameOverlaySetting = 0,
			AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
			ActLawfully = false,
			HasBeenActiveInWeek = true,
			HintsEnabled = true,
			AutoReacquireTargets = false
		});
		context.Materials.Add(Material(1, "wool"));
		var functions = Tag(1, "Functions");
		var tools = Tag(2, "Tools", functions);
		var market = Tag(3, "Market");
		var professionalTools = Tag(4, "Professional Tools", market);
		var standardTools = Tag(5, "Standard Tools", professionalTools);
		context.Tags.AddRange(functions, tools, market, professionalTools, standardTools);
		context.GameItemProtos.Add(Item(1, "dummy", "a dummy item", "wool", null, null));
		context.SaveChanges();
	}

	private static Material Material(long id, string name)
	{
		return new Material
		{
			Id = id,
			Name = name,
			MaterialDescription = name,
			Type = 0,
			BehaviourType = 0,
			Density = 1.0,
			Organic = true,
			ResidueSdesc = string.Empty,
			ResidueDesc = string.Empty,
			ResidueColour = "grey"
		};
	}

	private static Tag Tag(long id, string name, Tag? parent = null)
	{
		return new Tag
		{
			Id = id,
			Name = name,
			Parent = parent
		};
	}

	private static GameItemProto Item(
		long id,
		string name,
		string shortDescription,
		string material,
		string? uniqueName,
		string? builderNotes)
	{
		return new GameItemProto
		{
			Id = id,
			Name = name,
			UniqueName = uniqueName,
			BuilderNotes = builderNotes,
			Keywords = name,
			MaterialId = material == "wool" ? 1 : 0,
			EditableItem = Editable(),
			RevisionNumber = 0,
			Size = 0,
			Weight = 1.0,
			ReadOnly = false,
			LongDescription = string.Empty,
			BaseItemQuality = 0,
			CustomColour = string.Empty,
			MorphTimeSeconds = 0,
			MorphEmote = string.Empty,
			ShortDescription = shortDescription,
			FullDescription = string.Empty,
			PermitPlayerSkins = false,
			CostInBaseCurrency = 0.0M,
			IsHiddenFromPlayers = false,
			PlanarData = string.Empty
		};
	}

	private static EditableItem Editable()
	{
		return new EditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = 4,
			BuilderAccountId = 1,
			BuilderDate = DateTime.UtcNow,
			BuilderComment = "test",
			ReviewerAccountId = 1,
			ReviewerComment = "test",
			ReviewerDate = DateTime.UtcNow
		};
	}
}
