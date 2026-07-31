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
	public void MedievalProductionCrafts_DefineExpectedPhaseOrderedOutputSet()
	{
		var specs = ItemSeeder.MedievalProductionCraftSpecsForTesting
			.OrderBy(x => x.Phase)
			.ThenBy(x => x.Name, StringComparer.Ordinal)
			.ToArray();
		var outputs = specs
			.SelectMany(x => x.Products)
			.ToArray();

		Assert.AreEqual(35, specs.Length);
		Assert.AreEqual(35, outputs.Length);
		Assert.AreEqual(specs.Length,
			specs.Select(x => $"{x.Category}\0{x.Name}").Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Craft name/category identities must remain unique for rerun admission.");
		CollectionAssert.AreEquivalent(ExpectedOutputStableReferences, outputs);
		Assert.AreEqual(16, specs.Count(x => x.Phase == 1));
		Assert.AreEqual(11, specs.Count(x => x.Phase == 2));
		Assert.AreEqual(6, specs.Count(x => x.Phase == 3));
		Assert.AreEqual(2, specs.Count(x => x.Phase == 4));
	}

	[TestMethod]
	public void MedievalProductionCrafts_DependenciesAreOwnedAndAcyclic()
	{
		var specs = ItemSeeder.MedievalProductionCraftSpecsForTesting.ToArray();
		var outputPhases = specs
			.SelectMany(spec => spec.Products.Select(output => (Output: output, spec.Phase)))
			.ToDictionary(x => x.Output, x => x.Phase, StringComparer.OrdinalIgnoreCase);
		var allowedSourceStatuses = new[]
		{
			"historic_foundation",
			"primary_production",
			"medieval_crafted",
			"upstream_source_exempt"
		};

		foreach (var spec in specs)
		{
			Assert.IsTrue(spec.Dependencies.Count > 0, $"{spec.Name} should record its dependency ownership.");
			Assert.IsTrue(spec.SourceOwnership.Contains("medieval_crafted", StringComparer.Ordinal),
				$"{spec.Name} should identify its product as medieval crafted.");
			Assert.IsTrue(spec.Dependencies.All(x => allowedSourceStatuses.Contains(x.SourceStatus, StringComparer.Ordinal)),
				$"{spec.Name} has an unclassified dependency.");
			Assert.IsTrue(spec.Dependencies.All(x => !string.IsNullOrWhiteSpace(x.SourceOwner)),
				$"{spec.Name} has a dependency without an owning source.");
			Assert.IsTrue(spec.Dependencies.All(x => x.SourcePhase < spec.Phase),
				$"{spec.Name} depends on a same-phase or later-phase source.");

			foreach (var dependency in spec.Dependencies.Where(x => x.StableReference is not null))
			{
				Assert.IsTrue(outputPhases.TryGetValue(dependency.StableReference!, out var producerPhase),
					$"{spec.Name} references unproduced exact input {dependency.StableReference}.");
				Assert.AreEqual(dependency.SourcePhase, producerPhase,
					$"{spec.Name} records the wrong producer phase for {dependency.StableReference}.");
				Assert.IsTrue(producerPhase < spec.Phase,
					$"{spec.Name} must not depend on {dependency.StableReference} from its own or a later phase.");
			}
		}
	}

	[TestMethod]
	public void MedievalProductionCrafts_UseExpectedDifficultyPolicy()
	{
		var specs = ItemSeeder.MedievalProductionCraftSpecsForTesting.ToArray();

		Assert.IsTrue(specs.Where(x => x.Phase == 1).All(x =>
			x.MinimumTraitValue is 10 or 15 && x.Difficulty is Difficulty.Easy or Difficulty.Normal));
		Assert.IsTrue(specs.Where(x => x.Phase == 2 && x.KnowledgeSubtype == "Basic Tools").All(x =>
			x.MinimumTraitValue == 20 && x.Difficulty == Difficulty.Normal));
		Assert.IsTrue(specs.Where(x => x.Phase == 2 && x.KnowledgeSubtype == "Workshop Apparatus").All(x =>
			x.MinimumTraitValue == 25 && x.Difficulty == Difficulty.Hard));
		Assert.IsTrue(specs.Where(x => x.Phase == 3).All(x =>
			x.MinimumTraitValue == 25 && x.Difficulty == Difficulty.Hard));
		Assert.IsTrue(specs.Where(x => x.Phase == 4).All(x =>
			x.MinimumTraitValue == 10 && x.Difficulty == Difficulty.Easy));

		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.Medieval.cs");
		StringAssert.Contains(source, "Outcome.MinorFail");
		StringAssert.Contains(source, "5,");
		StringAssert.Contains(source, "3,");
		StringAssert.Contains(source, "false,");
	}

	[TestMethod]
	public void MedievalProductionCrafts_ExactReferencesAndFunctionalToolTagsResolveToSeededSources()
	{
		var specs = ItemSeeder.MedievalProductionCraftSpecsForTesting.ToArray();
		var medievalItems = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalComponentGaps.cs") +
		                    ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalHouseholdTools.cs");
		var toolSources = medievalItems +
		                  ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.HistoricFoundation.cs") +
		                  ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.PrimaryProductionTools.cs");

		foreach (var stableReference in specs
			         .SelectMany(x => x.Products)
			         .Concat(specs.SelectMany(x => x.Dependencies)
				         .Where(x => x.StableReference is not null)
				         .Select(x => x.StableReference!))
			         .Distinct(StringComparer.OrdinalIgnoreCase))
		{
			StringAssert.Contains(medievalItems, $"\"{stableReference}\"",
				$"Expected exact Medieval item reference {stableReference} to resolve.");
		}

		foreach (var tool in specs
			         .SelectMany(x => x.Tools)
			         .Distinct(StringComparer.OrdinalIgnoreCase))
		{
			var match = Regex.Match(
				tool,
				@"^TagTool - (?:Held|InRoom) - an item with the (?<tag>.+) tag$",
				RegexOptions.CultureInvariant);
			Assert.IsTrue(match.Success, $"Unexpected tool contract {tool}.");
			var tag = match.Groups["tag"].Value;
			Assert.IsTrue(
				toolSources.Contains(tag, StringComparison.Ordinal),
				$"Expected a seeded item source to provide the {tag} functional tool tag.");
		}
	}

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
	public void MedievalProductionCrafts_MaintainedMaterialAndTagCataloguesCoverAllContracts()
	{
		var tagRows = File.ReadLines(SourcePath("Design Documents", "Data", "SeededTagHierarchy.csv"))
			.Skip(1)
			.Select(x => x.Split('\t'))
			.Where(x => x.Length == 3)
			.ToArray();
		var tagNames = tagRows
			.Select(x => x[0])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var requiredToolTags = ItemSeeder.MedievalProductionCraftSpecsForTesting
			.SelectMany(x => x.Tools)
			.Select(x => Regex.Match(x, @"with the (?<tag>.+) tag$", RegexOptions.CultureInvariant).Groups["tag"].Value);
		var missingTags = CommodityTagNames
			.Concat(requiredToolTags)
			.Where(x => !tagNames.Contains(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

		Assert.AreEqual(0, missingTags.Length,
			$"SeededTagHierarchy.csv is missing Medieval production dependencies: {string.Join(", ", missingTags)}");
		Assert.AreEqual(tagRows.Length,
			tagRows.Select(x => x[2]).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"SeededTagHierarchy.csv should not contain duplicate hierarchy paths.");

		using var materialsDocument = JsonDocument.Parse(
			ReadSource("Design Documents", "Data", "Seeded_Materials.json"));
		var materialNames = materialsDocument.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Material Name").GetString()!)
			.ToArray();
		var missingMaterials = MaterialNames
			.Where(x => !materialNames.Contains(x, StringComparer.OrdinalIgnoreCase))
			.ToArray();

		Assert.AreEqual(0, missingMaterials.Length,
			$"Seeded_Materials.json is missing Medieval production materials: {string.Join(", ", missingMaterials)}");
		Assert.AreEqual(materialNames.Length,
			materialNames.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
			"Seeded_Materials.json should not contain duplicate material names.");
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

		Assert.AreEqual(35, firstCraftCount);
		Assert.AreEqual(firstCraftCount, context.Crafts.Count());
		Assert.AreEqual(firstInputCount, context.CraftInputs.Count());
		Assert.AreEqual(firstToolCount, context.CraftTools.Count());
		Assert.AreEqual(firstProductCount, context.CraftProducts.Count());
		Assert.AreEqual(1, context.Knowledges.Count(x => x.Name == "Medieval Industry Foundations"));
		Assert.AreEqual(35, context.Crafts
			.Select(x => new { x.Name, x.Category })
			.Distinct()
			.Count());
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

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(SourcePath(parts));
	}

	private static string SourcePath(params string[] parts)
	{
		return Path.Combine(new[] { SourceRoot() }.Concat(parts).ToArray());
	}

	private static string SourceRoot()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MudSharp.sln")))
		{
			directory = directory.Parent;
		}

		Assert.IsNotNull(directory, "Could not locate repository root from test output path.");
		return directory.FullName;
	}
}
