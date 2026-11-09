#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederMedievalProductionCraftTests
{
	private static readonly string[] ExpectedOutputStableReferences =
	[
		"medieval_industry_stock_plank_bundle",
		"medieval_industry_stock_handle_blanks",
		"medieval_industry_stock_iron_bar",
		"medieval_industry_stock_bronze_bar",
		"medieval_industry_stock_wire_coil",
		"medieval_industry_stock_rivet_packet",
		"medieval_industry_stock_clay_body_lump",
		"medieval_industry_stock_fired_brick_stack",
		"medieval_industry_stock_leather_panel",
		"medieval_industry_stock_parchment_sheet",
		"medieval_industry_stock_yarn_skein",
		"medieval_industry_stock_sewing_thread",
		"medieval_industry_stock_plain_cloth_bolt",
		"medieval_industry_stock_glue_cake",
		"medieval_industry_stock_seal_wax_stick",
		"medieval_industry_stock_bandage_roll",
		"medieval_tool_felling_axe",
		"medieval_tool_hand_saw",
		"medieval_tool_wood_chisel",
		"medieval_tool_wood_auger",
		"medieval_tool_drawknife",
		"medieval_tool_hide_scraper",
		"medieval_tool_tanning_beam",
		"medieval_workshop_forge",
		"medieval_workshop_smelting_furnace",
		"medieval_tool_crucible",
		"medieval_tool_grindstone",
		"medieval_tool_parchment_stretching_frame",
		"medieval_tool_mould_and_deckle",
		"medieval_workshop_book_press",
		"medieval_tool_mortar_and_pestle",
		"medieval_tool_suture_needle",
		"medieval_tool_surgical_probe",
		"medieval_workshop_lit_forge",
		"medieval_workshop_lit_smelting_furnace"
	];

	private static readonly string[] MaterialNames =
	[
		"oak",
		"ash",
		"wrought iron",
		"bronze",
		"prepared clay",
		"fired brick",
		"leather",
		"parchment",
		"wool",
		"linen",
		"bone",
		"beeswax",
		"stone",
		"charcoal"
	];

	private static readonly string[] CommodityTagNames =
	[
		"Materials",
		"Furniture Timber Stock",
		"Tool Blank Stock",
		"Metal Bar Stock Commodity",
		"Metal Billet Commodity",
		"Prepared Clay Commodity",
		"Fired Brick Commodity",
		"Prepared Leather Panel",
		"Parchment Sheet Stock",
		"Raw Textile Fibre",
		"Prepared Textile Fibre",
		"Spun Yarn",
		"Woven Cloth",
		"Dressed Stone Block Commodity",
		"Charcoal Fuel Commodity"
	];

	[TestMethod]
	public void MedievalProductionCrafts_AreGatedToMedievalEra()
	{
		Assert.IsTrue(ItemSeeder.ShouldSeedMedievalCraftsForTesting("medieval"));
		Assert.IsTrue(ItemSeeder.ShouldSeedMedievalCraftsForTesting("antiquity medieval renaissance"));
		Assert.IsFalse(ItemSeeder.ShouldSeedMedievalCraftsForTesting("antiquity"));
		Assert.IsFalse(ItemSeeder.ShouldSeedMedievalCraftsForTesting("renaissance earlymodern"));
		Assert.IsFalse(ItemSeeder.ShouldSeedMedievalCraftsForTesting(null));
	}

	[TestMethod]
	public void MedievalProductionCrafts_SeedTwiceWithoutDuplicates()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();

		seeder.SeedMedievalProductionChainCraftsForTesting(context);
		var firstCraftCount = context.Crafts.Count();
		var firstInputCount = context.CraftInputs.Count();
		var firstToolCount = context.CraftTools.Count();
		var firstProductCount = context.CraftProducts.Count();
		seeder.SeedMedievalProductionChainCraftsForTesting(context);

		Assert.IsTrue(firstCraftCount > 0);
		Assert.AreEqual(firstCraftCount, context.Crafts.Count());
		Assert.AreEqual(firstInputCount, context.CraftInputs.Count());
		Assert.AreEqual(firstToolCount, context.CraftTools.Count());
		Assert.AreEqual(firstProductCount, context.CraftProducts.Count());

	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
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

		var traitNames = ItemSeeder.MedievalProductionCraftSpecsForTesting
			.Select(x => x.Trait)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		context.TraitDefinitions.AddRange(traitNames.Select((name, index) => new TraitDefinition
		{
			Id = index + 1,
			Name = name,
			Type = 0,
			OwnerScope = 0,
			TraitGroup = "Crafting",
			ChargenBlurb = string.Empty,
			ValueExpression = string.Empty
		}));

		var toolTagNames = ItemSeeder.MedievalProductionCraftSpecsForTesting
			.SelectMany(x => x.Tools)
			.Select(x => Regex.Match(x, @"with the (?<tag>.+) tag$", RegexOptions.CultureInvariant).Groups["tag"].Value);
		context.Tags.AddRange(CommodityTagNames
			.Concat(toolTagNames)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Select((name, index) => new Tag
			{
				Id = index + 1,
				Name = name
			}));

		context.Materials.AddRange(MaterialNames.Select((name, index) => new Material
		{
			Id = index + 1,
			Name = name,
			MaterialDescription = name,
			Type = 0,
			BehaviourType = 0,
			Density = 1.0,
			Organic = true,
			ResidueSdesc = string.Empty,
			ResidueDesc = string.Empty,
			ResidueColour = "grey"
		}));

		context.GameItemProtos.AddRange(ExpectedOutputStableReferences.Select((stableReference, index) =>
			new GameItemProto
			{
				Id = index + 100,
				Name = stableReference,
				UniqueName = stableReference,
				Keywords = stableReference.Replace('_', ' '),
				MaterialId = 1,
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
				ShortDescription = $"a test {stableReference.Replace('_', ' ')}",
				FullDescription = string.Empty,
				PermitPlayerSkins = false,
				CostInBaseCurrency = 0.0M,
				IsHiddenFromPlayers = false,
				PlanarData = string.Empty
			}));

		context.SaveChanges();
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
