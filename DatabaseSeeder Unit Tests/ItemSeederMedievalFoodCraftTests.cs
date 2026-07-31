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
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederMedievalFoodCraftTests
{
	private static readonly string[] ExpectedToolOutputs =
	[
		"medieval_tool_butchers_knife",
		"medieval_tool_cooking_knife",
		"medieval_tool_threshing_flail",
		"medieval_tool_winnowing_basket",
		"medieval_tool_cooking_pot",
		"medieval_workshop_lauter_tun",
		"medieval_workshop_bake_oven",
		"medieval_workshop_brew_copper",
		"medieval_workshop_mash_tun",
		"medieval_workshop_fermenting_gyle_tun",
		"medieval_tool_flour_sieve",
		"medieval_tool_kneading_trough",
		"medieval_tool_salting_trough",
		"medieval_tool_smoking_rack",
		"medieval_tool_oil_press",
		"medieval_tool_fruit_press",
		"medieval_tool_mashing_paddle"
	];

	private static readonly string[] ExpectedFilledVessels =
	[
		"medieval_tableware_oil_amphora",
		"medieval_tableware_table_beer_cask",
		"medieval_tableware_small_wine_cask"
	];

	private static readonly string[] MaterialNames =
	[
		"oak",
		"wrought iron",
		"bronze",
		"willow",
		"wheat",
		"salt",
		"olive crop",
		"grape",
		"meat",
		"fish"
	];

	[TestMethod]
	public void MedievalFoodCraftCatalogue_HasExactPhaseAndOutputCoverage()
	{
		var specs = ItemSeeder.MedievalFoodCraftSpecsForTesting.ToArray();

		Assert.AreEqual(48, specs.Length);
		Assert.AreEqual(48, specs.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.AreEqual(17, specs.Count(x => x.Phase == 1));
		Assert.AreEqual(18, specs.Count(x => x.Phase == 2));
		Assert.AreEqual(13, specs.Count(x => x.Phase == 3));

		var stableOutputs = specs
			.SelectMany(x => x.Products)
			.Select(ProductStableReference)
			.Where(x => x is not null)
			.Cast<string>()
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		CollectionAssert.AreEquivalent(
			ExpectedToolOutputs
				.Concat(ItemSeeder.MedievalPreparedFoodStableReferencesForTesting)
				.Concat(ExpectedFilledVessels)
				.ToArray(),
			stableOutputs);

		var commodityOutputs = specs
			.SelectMany(x => x.Products)
			.Where(x => x.StartsWith("commodity:", StringComparison.Ordinal))
			.Select(x => x["commodity:".Length..])
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		CollectionAssert.AreEquivalent(
			ItemSeeder.PreIndustrialFoodCommodityTagsForTesting.ToArray(),
			commodityOutputs);

		CollectionAssert.AreEquivalent(
			new[]
			{
				"Food Tools",
				"Grain Processing",
				"Baking and Pottage",
				"Oil and Fruit Pressing",
				"Meat Preservation",
				"Fish Preservation",
				"Brewing and Winemaking"
			},
			specs.Select(x => x.KnowledgeSubtype).Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
	}

	[TestMethod]
	public void MedievalFoodCraftCatalogue_HasMonotonicOwnedDependencies()
	{
		var specs = ItemSeeder.MedievalFoodCraftSpecsForTesting.ToArray();
		var commodityProducerIndex = specs
			.SelectMany((spec, index) => spec.Products
				.Where(x => x.StartsWith("commodity:", StringComparison.Ordinal))
				.Select(x => (Tag: x["commodity:".Length..], Index: index)))
			.GroupBy(x => x.Tag, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.Min(y => y.Index), StringComparer.OrdinalIgnoreCase);

		for (var index = 0; index < specs.Length; index++)
		{
			var spec = specs[index];
			Assert.IsTrue(spec.Dependencies.All(x => !string.IsNullOrWhiteSpace(x.SourceStatus)));
			Assert.IsTrue(spec.Dependencies.All(x => !string.IsNullOrWhiteSpace(x.SourceOwner)));
			Assert.IsTrue(spec.Dependencies
				.Where(x => x.StableReference is not null)
				.All(x => x.SourcePhase < spec.Phase),
				$"{spec.Name} has an exact item dependency from a non-earlier phase.");

			foreach (var input in spec.Inputs)
			{
				var pileTag = Regex.Match(input, @"; piletag (?<tag>.+)$", RegexOptions.CultureInvariant)
					.Groups["tag"].Value;
				if (string.IsNullOrWhiteSpace(pileTag) ||
				    !commodityProducerIndex.TryGetValue(pileTag, out var producerIndex))
				{
					continue;
				}

				Assert.IsTrue(producerIndex < index,
					$"{spec.Name} consumes {pileTag} before its producing craft.");
			}
		}
	}

	[TestMethod]
	public void MedievalFoodCraftCatalogue_UsesRequiredDifficultyAndExecutionPolicy()
	{
		var specs = ItemSeeder.MedievalFoodCraftSpecsForTesting;
		Assert.IsTrue(specs.All(x => x.MinimumTraitValue is 10 or 15 or 20 or 25));
		Assert.IsTrue(specs.All(x => x.Difficulty is Difficulty.Easy or Difficulty.Normal or Difficulty.Hard));
		Assert.IsTrue(specs.Where(x => x.MinimumTraitValue == 25).All(x => x.Difficulty == Difficulty.Hard));

		var source = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.Crafting.MedievalFood.cs");
		Assert.IsTrue(source.Contains("Outcome.MinorFail", StringComparison.Ordinal));
		Assert.IsTrue(Regex.IsMatch(
			source,
			@"Outcome\.MinorFail,\s*5,\s*3,\s*false,",
			RegexOptions.Multiline | RegexOptions.CultureInvariant));
		Assert.IsTrue(source.Contains("MedievalFoodCraftingPhases(spec.Phase)", StringComparison.Ordinal));
	}

	[TestMethod]
	public void MedievalFoodItemsAndSharedTags_ExposeExactFoundationSets()
	{
		Assert.AreEqual(6, ItemSeeder.MedievalFoodProductionToolStableReferencesForTesting.Count);
		Assert.AreEqual(11, ItemSeeder.MedievalPreparedFoodStableReferencesForTesting.Count);
		Assert.AreEqual(19, ItemSeeder.PreIndustrialFoodCommodityTagsForTesting.Count);
		Assert.AreEqual(
			19,
			ItemSeeder.PreIndustrialFoodCommodityTagsForTesting
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Count());

		var itemSource = ReadSource("DatabaseSeeder", "Seeders", "ItemSeeder.MedievalFoodProduction.cs");
		foreach (var component in new[]
		{
			"PreparedFood_Medieval_Bread",
			"PreparedFood_Medieval_HardBread",
			"PreparedFood_Medieval_Pottage",
			"PreparedFood_Medieval_PreservedProvision"
		})
		{
			Assert.IsTrue(itemSource.Contains(component, StringComparison.Ordinal));
		}

		Assert.IsTrue(itemSource.Contains("3, 8,", StringComparison.Ordinal));
		Assert.IsTrue(itemSource.Contains("30, 120,", StringComparison.Ordinal));
		Assert.IsTrue(itemSource.Contains("2, 5,", StringComparison.Ordinal));
		Assert.IsTrue(itemSource.Contains("14, 60,", StringComparison.Ordinal));
	}

	[TestMethod]
	public void MedievalFoodMaintainedExports_ContainLiveComponentsTagsLiquidsAndMaterials()
	{
		var componentCatalogue = ReadSource("Design Documents", "Data", "Seeded_Item_Components.json");
		foreach (var component in new[]
		         {
			         "PreparedFood_Medieval_Bread",
			         "PreparedFood_Medieval_HardBread",
			         "PreparedFood_Medieval_Pottage",
			         "PreparedFood_Medieval_PreservedProvision"
		         })
		{
			Assert.IsTrue(componentCatalogue.Contains($"\"Component Name\":  \"{component}\"",
				StringComparison.Ordinal));
		}

		var tagPaths = File.ReadLines(SourcePath("Design Documents", "Data", "SeededTagHierarchy.csv"))
			.Select(x => x.Split('\t'))
			.Where(x => x.Length >= 3)
			.Select(x => x[2])
			.ToArray();
		foreach (var tag in ItemSeeder.PreIndustrialFoodCommodityTagsForTesting)
		{
			Assert.AreEqual(
				1,
				tagPaths.Count(x => x.Equals(
					$"Materials / Food Products / Pre-Industrial Food Commodities / {tag}",
					StringComparison.Ordinal)));
		}

		foreach (var path in new[]
		         {
			         "Materials / Animal Product / Butchery Output / Raw Meat Cut / Raw Fish Cut",
			         "Materials / Animal Product / Butchery Output / Raw Meat Cut / Raw Non-Fish Meat Cut",
			         "Functions / Tools / Foodmaking Tools / Bake Oven",
			         "Functions / Tools / Foodmaking Tools / Kneading Trough"
		         })
		{
			Assert.AreEqual(1, tagPaths.Count(x => x.Equals(path, StringComparison.Ordinal)));
		}

		var liquidCatalogue = ReadSource("Design Documents", "Data", "Seeded_Liquids.json");
		foreach (var liquid in new[] { "amber ale", "red wine", "vegetable oil" })
		{
			Assert.IsTrue(liquidCatalogue.Contains($"\"Liquid Name\":  \"{liquid}\"",
				StringComparison.Ordinal));
		}

		var materialCatalogue = ReadSource("Design Documents", "Data", "Seeded_Materials.json");
		foreach (var material in new[] { "wheat", "olive crop", "meat", "fish" })
		{
			Assert.IsTrue(materialCatalogue.Contains($"\"Material Name\":  \"{material}\"",
				StringComparison.Ordinal));
		}
	}

	[TestMethod]
	public void AnimalButchery_ClassifiesFishAndNonFishCutsWithoutDroppingParentTag()
	{
		var specs = AnimalButcherySeeder.StockItemSpecsForTesting
			.Where(x => x.Tags.Contains("Raw Meat Cut", StringComparer.OrdinalIgnoreCase))
			.Where(x => !x.Key.StartsWith("global:", StringComparison.OrdinalIgnoreCase))
			.ToArray();
		var fishPrefixes = new[] { "fish:", "shark:", "crustacean:", "cephalopod:" };

		Assert.IsTrue(specs.Any());
		foreach (var spec in specs)
		{
			var expected = fishPrefixes.Any(prefix =>
				spec.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				? AnimalButcherySeeder.RawFishCutTagForTesting
				: AnimalButcherySeeder.RawNonFishMeatCutTagForTesting;
			Assert.AreEqual(expected, AnimalButcherySeeder.RawCutClassificationForTesting(spec));
			Assert.IsTrue(spec.Tags.Contains("Raw Meat Cut", StringComparer.OrdinalIgnoreCase));
		}
	}

	[TestMethod]
	public void MedievalFoodLiquidProducts_FitExistingVessels()
	{
		var specs = ItemSeeder.MedievalFoodCraftSpecsForTesting;
		var liquidProducts = specs
			.SelectMany(x => x.Products)
			.Where(x => x.StartsWith("liquid:", StringComparison.Ordinal))
			.ToArray();
		CollectionAssert.AreEquivalent(
			new[]
			{
				"liquid:medieval_tableware_oil_amphora:vegetable oil:1",
				"liquid:medieval_tableware_table_beer_cask:amber ale:3.5",
				"liquid:medieval_tableware_small_wine_cask:red wine:3.5"
			},
			liquidProducts);

		var componentSource = ReadSource(
			"DatabaseSeeder", "Seeders", "UsefulSeeder.ItemComponents.ContainersAndWriting.cs");
		Assert.IsTrue(componentSource.Contains(
			"CreateLiquidContainer(\"LContainer_GallonCask\", \"A liquid container for a non-see through gallon-sized cask\", 3.7",
			StringComparison.Ordinal));
		Assert.IsTrue(componentSource.Contains(
			"CreateLiquidContainer(\"LContainer_Amphora_Urna\", \"A liquid container for an amphora in the roman urna (~2.88 gallon)\", 13.1",
			StringComparison.Ordinal));
	}

	[TestMethod]
	public void MedievalFoodCrafts_SeedTwiceWithoutDuplicates()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();

		seeder.SeedMedievalFoodBeverageCraftsForTesting(context);
		var firstCraftCount = context.Crafts.Count();
		var firstInputCount = context.CraftInputs.Count();
		var firstToolCount = context.CraftTools.Count();
		var firstProductCount = context.CraftProducts.Count();
		seeder.SeedMedievalFoodBeverageCraftsForTesting(context);

		Assert.AreEqual(48, firstCraftCount);
		Assert.AreEqual(firstCraftCount, context.Crafts.Count());
		Assert.AreEqual(firstInputCount, context.CraftInputs.Count());
		Assert.AreEqual(firstToolCount, context.CraftTools.Count());
		Assert.AreEqual(firstProductCount, context.CraftProducts.Count());
		Assert.AreEqual(1, context.Knowledges.Count(x => x.Name == "Medieval Food Production"));
		Assert.AreEqual(48, context.Crafts
			.Select(x => new { x.Name, x.Category })
			.Distinct()
			.Count());
		Assert.IsTrue(context.Crafts.All(x =>
			x.FailThreshold == (int)Outcome.MinorFail &&
			x.FreeSkillChecks == 5 &&
			x.FailPhase == 3 &&
			!x.Interruptable &&
			x.CraftPhases.Count == 3));
		Assert.AreEqual(11, context.CraftProducts.Count(x => x.ProductType == "CookedFoodProduct"));
		Assert.AreEqual(3, context.CraftProducts.Count(x => x.ProductType == "LiquidProduct"));
		Assert.IsTrue(context.CraftProducts
			.Where(x => x.ProductType == "CookedFoodProduct")
			.All(x => XDocument.Parse(x.Definition).Descendants("Slot").Any()));
	}

	private static string? ProductStableReference(string product)
	{
		if (product.StartsWith("medieval_", StringComparison.Ordinal))
		{
			return product;
		}

		if (product.StartsWith("liquid:", StringComparison.Ordinal))
		{
			return product.Split(':')[1];
		}

		return null;
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

		var specs = ItemSeeder.MedievalFoodCraftSpecsForTesting;
		var traitNames = specs.Select(x => x.Trait).Distinct(StringComparer.OrdinalIgnoreCase);
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

		var toolTags = specs
			.SelectMany(x => x.Tools)
			.Select(x => Regex.Match(x, @"with the (?<tag>.+) tag$", RegexOptions.CultureInvariant).Groups["tag"].Value);
		var inputTags = specs
			.SelectMany(x => x.Inputs)
			.SelectMany(x => new[]
			{
				Regex.Match(x, @"with the (?<tag>.+) tag$", RegexOptions.CultureInvariant).Groups["tag"].Value,
				Regex.Match(x, @"tagged as (?<tag>.+?)(?:;|$)", RegexOptions.CultureInvariant).Groups["tag"].Value,
				Regex.Match(x, @"; piletag (?<tag>.+)$", RegexOptions.CultureInvariant).Groups["tag"].Value
			});
		var tags = toolTags
			.Concat(inputTags)
			.Concat(ItemSeeder.PreIndustrialFoodCommodityTagsForTesting)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		context.Tags.AddRange(tags.Select((name, index) => new Tag
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

		context.Liquids.AddRange(new[] { "Water", "vegetable oil", "amber ale", "red wine" }
			.Select((name, index) => new Liquid
			{
				Id = index + 1,
				Name = name,
				Description = name,
				LongDescription = name,
				TasteText = name,
				VagueTasteText = name,
				SmellText = name,
				VagueSmellText = name,
				DisplayColour = "yellow",
				DampDescription = "damp",
				WetDescription = "wet",
				DrenchedDescription = "drenched",
				DampShortDescription = "damp",
				WetShortDescription = "wet",
				DrenchedShortDescription = "drenched",
				SurfaceReactionInfo = string.Empty,
				Density = 1.0,
				SpecificHeatCapacity = 1.0
			}));

		var stableReferences = specs
			.SelectMany(x => x.Dependencies)
			.Where(x => x.StableReference is not null)
			.Select(x => x.StableReference!)
			.Concat(specs
				.SelectMany(x => x.Products)
				.Select(ProductStableReference)
				.Where(x => x is not null)
				.Cast<string>())
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
		context.GameItemProtos.AddRange(stableReferences.Select((stableReference, index) =>
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

		Assert.IsNotNull(directory);
		return directory.FullName;
	}
}
