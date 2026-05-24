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
	public void InferReworkFunctionalTags_MirrorsMarketTagsWithoutRemovingThem()
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
		StringAssert.Contains(item.BuilderNotes, "Stock unique reference: antiquity_short_wool_chiton.");
		StringAssert.Contains(item.BuilderNotes, "Cultures: Hellenic.");
	}

	[TestMethod]
	public void CreateReworkItem_BackfillsExistingItemMetadataWithoutReplacingBuilderNotes()
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

		var item = new ItemSeeder().CreateReworkItemForTesting(
			context,
			"antiquity_short_wool_chiton",
			"chiton",
			"a reused wool chiton",
			"wool");

		Assert.IsNotNull(item);
		Assert.AreEqual(10, item!.Id);
		Assert.AreEqual("antiquity_short_wool_chiton", item.UniqueName);
		StringAssert.Contains(item.BuilderNotes, "Existing builder-maintained note.");
		StringAssert.Contains(item.BuilderNotes, "Stock unique reference: antiquity_short_wool_chiton.");
		StringAssert.Contains(item.BuilderNotes, "Cultures: Hellenic.");
	}

	[TestMethod]
	public void CreateReworkItem_BackfillsInferredFunctionalTagsOntoExistingItems()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		context.GameItemProtos.Add(Item(
			11,
			"hammer",
			"a reused workshop hammer",
			"wool",
			null,
			null));
		context.SaveChanges();

		var item = new ItemSeeder().CreateReworkItemForTesting(
			context,
			"antiquity_reused_workshop_hammer",
			"hammer",
			"a reused workshop hammer",
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
		StringAssert.Contains(roman.BuilderNotes, "Cultures: Roman.");
		StringAssert.Contains(hellenic.BuilderNotes, "Cultures: Hellenic.");
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
