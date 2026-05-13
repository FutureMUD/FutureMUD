#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederAddCraftTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
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

		context.TraitDefinitions.Add(new TraitDefinition
		{
			Id = 1,
			Name = "Crafting",
			Type = 0,
			OwnerScope = 0,
			TraitGroup = "Crafting",
			ChargenBlurb = string.Empty,
			ValueExpression = string.Empty
		});

		context.Tags.AddRange(
			Tag(1, "Ingredient"),
			Tag(2, "Variable Ingredient"),
			Tag(3, "Repairable"),
			Tag(4, "Flexible Material"),
			Tag(5, "Fabric"),
			Tag(6, "Metal Pile"),
			Tag(7, "Water Source"),
			Tag(8, "Tool Tag"),
			Tag(9, "Scrap Metal")
		);

		context.Materials.AddRange(
			Material(1, "iron"),
			Material(2, "clay"),
			Material(3, "oak")
		);

		context.Liquids.Add(Liquid(1, "Water"));
		context.Currencies.Add(new Currency { Id = 1, Name = "test coins", BaseCurrencyToGlobalBaseCurrencyConversion = 1.0M });
		context.NpcTemplates.Add(new NpcTemplate
		{
			Id = 1,
			Name = "test assistant",
			Type = "Test",
			Definition = "<Definition />",
			RevisionNumber = 0,
			EditableItem = Editable()
		});
		context.BloodModels.Add(new BloodModel { Id = 1, Name = "ABO" });

		context.CharacteristicDefinitions.AddRange(
			CharacteristicDefinition(1, "Colour"),
			CharacteristicDefinition(2, "Origin")
		);
		context.CharacteristicValues.AddRange(
			CharacteristicValue(1, 1, "red"),
			CharacteristicValue(2, 1, "blue"),
			CharacteristicValue(3, 2, "mine")
		);

		context.FutureProgs.AddRange(
			Prog(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"),
			Prog(2, "AlwaysText", ProgVariableTypes.Text, @"return ""No."""),
			Prog(3, "OnFinish", ProgVariableTypes.Void, string.Empty),
			Prog(4, "OnStart", ProgVariableTypes.Void, string.Empty),
			Prog(5, "OnCancel", ProgVariableTypes.Void, string.Empty),
			Prog(6, "SelectColour", ProgVariableTypes.Text, @"return ""red"""),
			Prog(7, "SetupNpc", ProgVariableTypes.Void, string.Empty),
			Prog(8, "LoadCraftProducts", ProgVariableTypes.Void, string.Empty)
		);

		context.GameItemProtos.AddRange(
			Item(100, "plain input item", "a plain input item", "iron"),
			Item(101, "simple product", "a simple test product", "iron"),
			Item(102, "cooked product", "a cooked test product", "iron"),
			Item(103, "variable product", "a variable test product", "iron"),
			Item(104, "input variable product", "an input-variable test product", "iron"),
			Item(105, "prog variable product", "a prog-variable test product", "iron"),
			Item(106, "failed product", "a failed test product", "clay"),
			Item(107, "test hammer", "a test hammer", "iron")
		);

		context.SaveChanges();
	}

	private static Tag Tag(long id, string name)
	{
		return new Tag { Id = id, Name = name };
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

	private static Liquid Liquid(long id, string name)
	{
		return new Liquid
		{
			Id = id,
			Name = name,
			Description = name,
			LongDescription = name,
			TasteText = string.Empty,
			VagueTasteText = string.Empty,
			SmellText = string.Empty,
			VagueSmellText = string.Empty,
			Density = 1.0,
			Organic = false,
			DisplayColour = "blue",
			DampDescription = string.Empty,
			WetDescription = string.Empty,
			DrenchedDescription = string.Empty,
			DampShortDescription = string.Empty,
			WetShortDescription = string.Empty,
			DrenchedShortDescription = string.Empty,
			SurfaceReactionInfo = string.Empty
		};
	}

	private static CharacteristicDefinition CharacteristicDefinition(long id, string name)
	{
		return new CharacteristicDefinition
		{
			Id = id,
			Name = name,
			Type = 0,
			Pattern = string.Empty,
			Description = name,
			Model = string.Empty,
			Definition = string.Empty
		};
	}

	private static CharacteristicValue CharacteristicValue(long id, long definitionId, string name)
	{
		return new CharacteristicValue
		{
			Id = id,
			Name = name,
			DefinitionId = definitionId,
			Value = name,
			AdditionalValue = string.Empty,
			Pluralisation = 0
		};
	}

	private static MudSharp.Models.FutureProg Prog(long id, string name, ProgVariableTypes returnType, string text)
	{
		return new MudSharp.Models.FutureProg
		{
			Id = id,
			FunctionName = name,
			Category = "Test",
			Subcategory = "Test",
			FunctionComment = string.Empty,
			FunctionText = text,
			ReturnType = (long)returnType,
			StaticType = (int)FutureProgStaticType.NotStatic,
			AcceptsAnyParameters = false,
			Public = true
		};
	}

	private static MudSharp.Models.GameItemProto Item(long id, string name, string shortDescription, string material)
	{
		return new GameItemProto
		{
			Id = id,
			Name = name,
			Keywords = name,
			MaterialId = material == "clay" ? 2 : 1,
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

	private static (int Seconds, string Echo, string FailEcho)[] BasicPhases()
	{
		return [(5, "$0 work|works.", "$0 fail|fails.")];
	}

	private static string[] BasicInputs()
	{
		return ["Tag - 1x an item with the Ingredient tag"];
	}

	private static string[] BasicTools()
	{
		return ["TagTool - Held - an item with the Tool Tag tag"];
	}

	private static string[] BasicProducts()
	{
		return ["SimpleProduct - 1x a simple test product (#101)"];
	}

	[TestMethod]
	public void ItemSeeder_AddCraft_ParsesInputToolAndProductImportGrammar()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);

		var craft = new ItemSeeder().AddCraftFromImportsForTesting(
			context,
			"test comprehensive craft",
			"Testing",
			BasicPhases(),
			[
				"Tag - 2x an item with the Ingredient tag; quality 2.5",
				"TagVariable - 1x an item with the Variable Ingredient tag with variable Colour",
				"ConditionRepair - 25% repair of an item with the Repairable tag",
				"SimpleItem - 1x a plain input item (#100)",
				"SimpleMaterial - 1x an item made of iron",
				"SimpleMaterial - 1x an item with material tagged as Flexible Material",
				"Commodity - 1 kilogram 250 grams of iron; piletag Metal Pile; characteristic Colour red; characteristic Origin any",
				"CommodityTag - 500 grams of a material tagged as Fabric; characteristic none",
				"LiquidUse - 1 litre 250 millilitres of Water",
				"LiquidTagUse - 500 millilitres of a liquid tagged Water Source"
			],
			[
				"TagTool - Held - an item with the Tool Tag tag; quality 3.25; usetool off",
				"SimpleTool - InRoom - a test hammer (#107); quality 0.75; usetool on"
			],
			[
				"SimpleProduct - 1x a simple test product (#101)",
				"CookedFoodProduct - 1x a cooked test product (#102); ingredient $i1=vegetable; purify off",
				"SimpleVariableProduct - 1x a variable test product (#103); variable Colour=$i2",
				"InputVariableProduct - 1x an input-variable test product (#104); variable Colour=$i2; specific Colour: a plain input item (#100)=red",
				"ProgVariableProduct - 1x a prog-variable test product (#105); variable Colour=SelectColour",
				"CommodityProduct - 250 grams of iron commodity; tag Scrap Metal; characteristic Colour=red; characteristic Origin from $i7",
				"MoneyProduct - 12.50 of test coins",
				"NPCProduct - 1x test assistant (#1); prog SetupNpc",
				"Prog - LoadCraftProducts",
				"DNATest - compare $i9 and $i10",
				"BloodTyping - test $i9 against ABO blood model",
				"ScrapInput - 35% by weight of 1x a plain input item (#100) ($i4); tag Scrap Metal",
				"UnusedInput - 45% of 2x an item with the Ingredient tag ($i1)"
			],
			["SimpleProduct - 1x a failed test product (#106)"],
			failProductMaterialInputIndexes: [(1, 5)]
		);

		Assert.AreEqual(10, craft.CraftInputs.Count);
		Assert.AreEqual(2, craft.CraftTools.Count);
		Assert.AreEqual(14, craft.CraftProducts.Count);
		Assert.AreEqual(2.5, craft.CraftInputs.OrderBy(x => x.OriginalAdditionTime).First().InputQualityWeight);

		var exactMaterial = XElement.Parse(craft.CraftInputs.Single(x =>
			x.InputType == "SimpleMaterial" && XElement.Parse(x.Definition).Element("TargetMaterial")!.Value == "1").Definition);
		Assert.AreEqual("0", exactMaterial.Element("TargetMaterialTag")!.Value);

		var taggedMaterial = XElement.Parse(craft.CraftInputs.Single(x =>
			x.InputType == "SimpleMaterial" && XElement.Parse(x.Definition).Element("TargetMaterialTag")!.Value == "4").Definition);
		Assert.AreEqual("0", taggedMaterial.Element("TargetMaterial")!.Value);

		var commodity = XElement.Parse(craft.CraftInputs.Single(x => x.InputType == "Commodity").Definition);
		Assert.AreEqual("6", commodity.Element("CommodityPileTag")!.Value);
		Assert.AreEqual("specific", commodity.Element("Characteristics")!.Attribute("mode")!.Value);
		Assert.AreEqual(2, commodity.Element("Characteristics")!.Elements("Characteristic").Count());

		var commodityTag = XElement.Parse(craft.CraftInputs.Single(x => x.InputType == "CommodityTag").Definition);
		Assert.AreEqual("none", commodityTag.Element("Characteristics")!.Attribute("mode")!.Value);

		var tagTool = craft.CraftTools.Single(x => x.ToolType == "TagTool");
		Assert.AreEqual(3.25, tagTool.ToolQualityWeight);
		Assert.IsFalse(tagTool.UseToolDuration);
		var simpleTool = craft.CraftTools.Single(x => x.ToolType == "SimpleTool");
		Assert.AreEqual(0.75, simpleTool.ToolQualityWeight);
		Assert.IsTrue(simpleTool.UseToolDuration);

		CollectionAssert.IsSubsetOf(
			new[]
			{
				"SimpleProduct", "CookedFoodProduct", "SimpleVariableProduct", "InputVariable", "ProgVariableProduct",
				"CommodityProduct", "MoneyProduct", "NPCProduct", "Prog", "DNATest", "BloodTyping", "ScrapInput",
				"UnusedInput"
			},
			craft.CraftProducts.Select(x => x.ProductType).Distinct().ToArray()
		);

		var commodityProduct = XElement.Parse(craft.CraftProducts.Single(x => x.ProductType == "CommodityProduct").Definition);
		Assert.AreEqual("9", commodityProduct.Element("Tag")!.Value);
		Assert.AreEqual("6", commodityProduct.Element("Characteristics")!.Elements("Characteristic").Single(x => x.Attribute("definition")!.Value == "2").Attribute("input")!.Value);
		Assert.AreEqual(4, craft.CraftProducts.Single(x => x.IsFailProduct).MaterialDefiningInputIndex);
	}

	[TestMethod]
	public void ItemSeeder_AddCraft_TypedSpecPersistsPhaseMetadataAndLifecycleProgs()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);

		var appear = context.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue");
		var start = context.FutureProgs.Single(x => x.FunctionName == "OnStart");
		var finish = context.FutureProgs.Single(x => x.FunctionName == "OnFinish");
		var cancel = context.FutureProgs.Single(x => x.FunctionName == "OnCancel");
		var trait = context.TraitDefinitions.Single(x => x.Name == "Crafting");

		var craft = new ItemSeeder().AddCraftForTesting(context, new ItemSeeder.CraftDefinitionSpec
		{
			Name = "test typed craft",
			Category = "Testing",
			Blurb = "typed test",
			Action = "testing typed",
			ActiveCraftItemSdesc = "a typed craft event",
			AppearProg = appear,
			OnStartProg = start,
			OnFinishProg = finish,
			OnCancelProg = cancel,
			Trait = trait,
			FailPhase = 1,
			Phases =
			[
				new ItemSeeder.CraftPhaseSpec
				{
					Seconds = 12,
					Echo = "$0 carefully work|works.",
					FailEcho = "$0 botch|botches it.",
					Exertion = ExertionLevel.Heavy,
					Stamina = 4.5
				}
			],
			Inputs = [new ItemSeeder.CraftInputSpec("Tag - 1x an item with the Ingredient tag", "Tag", "1x an item with the Ingredient tag", [], 1.0)],
			Tools = [new ItemSeeder.CraftToolSpec("TagTool - Held - an item with the Tool Tag tag", "TagTool", "Held - an item with the Tool Tag tag", [], 1.0, true)],
			Products = [new ItemSeeder.CraftProductSpec("SimpleProduct - 1x a simple test product (#101)", "SimpleProduct", "1x a simple test product (#101)", [], false, null)]
		});

		Assert.AreEqual(start.Id, craft.OnUseProgStartId);
		Assert.AreEqual(finish.Id, craft.OnUseProgCompleteId);
		Assert.AreEqual(cancel.Id, craft.OnUseProgCancelId);
		var phase = craft.CraftPhases.Single();
		Assert.AreEqual((int)ExertionLevel.Heavy, phase.ExertionLevel);
		Assert.AreEqual(4.5, phase.StaminaUsage);
	}

	[TestMethod]
	public void ItemSeeder_AddCraft_TraitGateCreatesDeterministicProgsAndUsesThem()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);

		var craft = new ItemSeeder().AddTraitGatedCraftFromImportsForTesting(
			context,
			"test trait gated craft",
			"Testing",
			"Crafting",
			40,
			BasicPhases(),
			BasicInputs(),
			BasicTools(),
			BasicProducts(),
			[]
		);

		var appear = context.FutureProgs.Single(x => x.FunctionName == "ItemSeederAppearCrafting40");
		var canUse = context.FutureProgs.Single(x => x.FunctionName == "ItemSeederCanUseCrafting40");
		var whyCannot = context.FutureProgs.Single(x => x.FunctionName == "ItemSeederWhyCannotUseCrafting40");
		Assert.AreEqual(appear.Id, craft.AppearInCraftsListProgId);
		Assert.AreEqual(canUse.Id, craft.CanUseProgId);
		Assert.AreEqual(whyCannot.Id, craft.WhyCannotUseProgId);
		Assert.IsTrue(appear.FunctionText.Contains(">= 40"));
		Assert.IsTrue(whyCannot.FunctionText.Contains("You need at least 40"));
	}

	[TestMethod]
	public void ItemSeeder_AddCraft_RerunSameNameAndCategorySkipsExistingCraft()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ItemSeeder seeder = new();

		var first = seeder.AddCraftFromImportsForTesting(
			context,
			"test idempotent craft",
			"Testing",
			BasicPhases(),
			BasicInputs(),
			BasicTools(),
			BasicProducts(),
			[]
		);
		var second = seeder.AddCraftFromImportsForTesting(
			context,
			"test idempotent craft",
			"Testing",
			BasicPhases(),
			["Tag - 2x an item with the Ingredient tag"],
			BasicTools(),
			["SimpleProduct - 1x a failed test product (#106)"],
			[]
		);

		Assert.AreEqual(first.Id, second.Id);
		Assert.AreEqual(1, context.Crafts.Count(x => x.Name == "test idempotent craft" && x.Category == "Testing"));
		Assert.AreEqual(1, first.CraftInputs.Count);
		Assert.AreEqual(1, first.CraftProducts.Count);
	}

	[TestMethod]
	public void ItemSeeder_AddCraft_InvalidImportsThrowAggregateValidationErrors()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);

		var exception = Assert.ThrowsException<ApplicationException>(() => new ItemSeeder().AddCraftFromImportsForTesting(
			context,
			"test invalid craft",
			"Testing",
			BasicPhases(),
			["Tag - 1x an item with the Missing tag"],
			["TagTool - Nowhere - an item with the Tool Tag tag"],
			["SimpleProduct - 1x a missing product (#999)"],
			[]
		));

		StringAssert.Contains(exception.Message, "Craft 'test invalid craft' has 3 validation error");
		StringAssert.Contains(exception.Message, "input 'Tag - 1x an item with the Missing tag'");
		StringAssert.Contains(exception.Message, "tool 'TagTool - Nowhere - an item with the Tool Tag tag'");
		StringAssert.Contains(exception.Message, "product 'SimpleProduct - 1x a missing product (#999)'");
	}
}
