#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreIndustrialFoodCatalogueCraftSeedingTests
{
	[TestMethod]
	public void CatalogueCrafts_SeedAllSupportedScopesWithResolvableProductsAndSelectorIds()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();
		seeder.SeedPreIndustrialFoodCatalogueCraftsForTesting(context, "medieval renaissance earlymodern");

		var specs = ItemSeeder.PreIndustrialFoodCatalogueCraftSpecsForTesting.ToArray();
		Assert.AreEqual(547, specs.Length);
		Assert.AreEqual(547, context.Crafts.Count());
		Assert.AreEqual(275, context.CraftProducts.AsEnumerable().Count(x => x.ProductType == "ProgCookedFoodProduct"));
		Assert.AreEqual(47, context.CraftProducts.AsEnumerable().Count(x => x.ProductType == "Prog"));
		Assert.AreEqual(225, context.CraftProducts.AsEnumerable().Count(x => x.ProductType == "LiquidProduct"));

		var selectorProducts = context.CraftProducts.AsEnumerable()
			.Where(x => x.ProductType is "ProgCookedFoodProduct" or "Prog")
			.ToArray();
		Assert.IsTrue(selectorProducts.All(x => long.Parse(XDocument.Parse(x.Definition).Root!.Element("ItemProg")!.Value) > 0));
		var selectorProgs = context.FutureProgs.AsEnumerable()
			.Where(x => x.FunctionName.StartsWith("ItemSeederPreIndustrialFood_", StringComparison.OrdinalIgnoreCase))
			.ToArray();
		Assert.AreEqual(322, selectorProgs.Length);
		Assert.IsTrue(selectorProgs
			.Where(x => x.FunctionName.StartsWith("ItemSeederPreIndustrialFood_", StringComparison.OrdinalIgnoreCase))
			.All(x => x.Id > 0 &&
			          x.ReturnType == (long)ProgVariableTypes.Item &&
			          x.FunctionText.Contains("loaditem(\"", StringComparison.Ordinal) &&
			          x.FunctionText.Contains("collectionfirst(collectionshuffle(@products))", StringComparison.Ordinal)));
		var catalogueReferences = ItemSeeder.PreIndustrialFoodItemsForTesting
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var selector in selectorProgs)
		{
			var selectedReferences = Regex.Matches(selector.FunctionText, "loaditem\\(\\\"(?<reference>[^\\\"]+)\\\"\\)")
				.Cast<System.Text.RegularExpressions.Match>()
				.Select(x => x.Groups["reference"].Value)
				.ToArray();
			Assert.IsTrue(selectedReferences.Length > 0, $"{selector.FunctionName} selects no catalogue item.");
			Assert.IsTrue(selectedReferences.All(x => catalogueReferences.Contains(x)),
				$"{selector.FunctionName} selects an item outside the maintained food catalogue.");
		}
		MudSharp.FutureProg.FutureProg.Initialise();
		var gameworld = new Mock<MudSharp.Framework.IFuturemud>().Object;
		foreach (var selector in selectorProgs)
		{
			var compiled = new MudSharp.FutureProg.FutureProg(
				gameworld,
				selector.FunctionName,
				(ProgVariableTypes)selector.ReturnType,
				[],
				selector.FunctionText);
			Assert.IsTrue(compiled.Compile(), $"{selector.FunctionName}: {compiled.CompileError}");
		}
		Assert.IsTrue(context.CraftProducts.AsEnumerable()
			.Where(x => x.ProductType == "LiquidProduct")
			.All(x => XDocument.Parse(x.Definition).Root!.Element("ProductProducedId")?.Value ==
				(ItemSeeder.PreIndustrialFoodItemsForTesting.Count(item =>
					item.Scope == FoodCatalogueScope.Shared && item.Kind == FoodCatalogueKind.Intermediate) + 1).ToString()));
	}

	[TestMethod]
	public void CatalogueCrafts_SeedTwiceWithoutDuplicateCraftsOrSelectorPrograms()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();
		seeder.SeedPreIndustrialFoodCatalogueCraftsForTesting(context, "medieval renaissance earlymodern");
		var counts = (context.Crafts.Count(), context.CraftInputs.Count(), context.CraftTools.Count(),
			context.CraftProducts.Count(), context.FutureProgs.Count(), context.Knowledges.Count());

		seeder.SeedPreIndustrialFoodCatalogueCraftsForTesting(context, "medieval renaissance earlymodern");

		Assert.AreEqual(counts.Item1, context.Crafts.Count());
		Assert.AreEqual(counts.Item2, context.CraftInputs.Count());
		Assert.AreEqual(counts.Item3, context.CraftTools.Count());
		Assert.AreEqual(counts.Item4, context.CraftProducts.Count());
		Assert.AreEqual(counts.Item5, context.FutureProgs.Count());
		Assert.AreEqual(counts.Item6, context.Knowledges.Count());
		Assert.AreEqual(1, context.Knowledges.Count(x => x.Name == "Pre-Industrial Food Catalogue Production"));
	}

	[TestMethod]
	public void CatalogueCrafts_RerunRepairsManagedSelectorPrograms()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ItemSeeder();
		seeder.SeedPreIndustrialFoodCatalogueCraftsForTesting(context, "medieval");

		var selector = context.FutureProgs.First(x =>
			x.FunctionName.StartsWith("ItemSeederPreIndustrialFood_", StringComparison.OrdinalIgnoreCase));
		selector.FunctionText = "return false";
		selector.ReturnType = (long)ProgVariableTypes.Boolean;
		context.SaveChanges();

		seeder.SeedPreIndustrialFoodCatalogueCraftsForTesting(context, "medieval");

		Assert.AreEqual((long)ProgVariableTypes.Item, selector.ReturnType);
		StringAssert.Contains(selector.FunctionText, "collectionfirst(collectionshuffle(@products))");
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

		context.TraitDefinitions.AddRange(
			new TraitDefinition
			{
				Id = 1,
				Name = "Cooking",
				Type = 0,
				OwnerScope = 0,
				TraitGroup = "Crafting",
				ChargenBlurb = string.Empty,
				ValueExpression = string.Empty
			},
			new TraitDefinition
			{
				Id = 2,
				Name = "Brewing",
				Type = 0,
				OwnerScope = 0,
				TraitGroup = "Crafting",
				ChargenBlurb = string.Empty,
				ValueExpression = string.Empty
			},
			new TraitDefinition
			{
				Id = 3,
				Name = "Milling",
				Type = 0,
				OwnerScope = 0,
				TraitGroup = "Crafting",
				ChargenBlurb = string.Empty,
				ValueExpression = string.Empty
			},
			new TraitDefinition
			{
				Id = 4,
				Name = "Baking",
				Type = 0,
				OwnerScope = 0,
				TraitGroup = "Crafting",
				ChargenBlurb = string.Empty,
				ValueExpression = string.Empty
			},
			new TraitDefinition
			{
				Id = 5,
				Name = "Butchering",
				Type = 0,
				OwnerScope = 0,
				TraitGroup = "Crafting",
				ChargenBlurb = string.Empty,
				ValueExpression = string.Empty
			});

		var tagNames = new[]
		{
			"Cooking", "Raw Non-Fish Meat Cut", "Raw Fish Cut", "Raw Milk", "Egg Product", "Food Crop", "Vegetable",
			"Fruit", "Oil Crop", "Offal", "Seeded Yield", "Brew Copper", "Pressed Honey", "Food"
		};
		context.Tags.AddRange(tagNames.Select((name, index) => new Tag { Id = index + 1, Name = name }));

		var stockItems = ItemSeeder.PreIndustrialFoodItemsForTesting
			.Where(x => x.Scope == FoodCatalogueScope.Shared && x.Kind == FoodCatalogueKind.Intermediate)
			.OrderBy(x => x.StableReference)
			.ToArray();
		var stockProtos = stockItems.Select((stock, index) => new GameItemProto
			{
				Id = index + 1,
				Name = stock.StableReference,
				UniqueName = stock.StableReference,
				Keywords = stock.StableReference.Replace('_', ' '),
				EditableItem = Editable(),
				RevisionNumber = 0,
				ShortDescription = stock.ShortDescription,
				LongDescription = string.Empty,
				FullDescription = string.Empty,
				CustomColour = string.Empty,
				PlanarData = string.Empty,
				MorphEmote = string.Empty
			}).ToArray();
		context.GameItemProtos.AddRange(stockProtos);
		context.GameItemProtos.Add(new GameItemProto
			{
				Id = stockItems.Length + 1,
				Name = "preindustrial_food_catalogue_liquid_amphora",
				UniqueName = "preindustrial_food_catalogue_liquid_amphora",
				Keywords = "shared catalogue liquid amphora",
				EditableItem = Editable(),
				RevisionNumber = 0,
				ShortDescription = "a shared catalogue liquid amphora",
				LongDescription = string.Empty,
				FullDescription = string.Empty,
				CustomColour = string.Empty,
				PlanarData = string.Empty,
				MorphEmote = string.Empty
			});

		context.Liquids.AddRange(ItemSeeder.PreIndustrialFoodLiquidsForTesting.Select((liquid, index) => new Liquid
		{
			Id = index + 1,
			Name = liquid.Name,
			Description = liquid.Name,
			LongDescription = liquid.Name,
			TasteText = liquid.Name,
			VagueTasteText = liquid.Name,
			SmellText = liquid.Name,
			VagueSmellText = liquid.Name,
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
