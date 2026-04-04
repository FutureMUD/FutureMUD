#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.CharacterCreation;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Merits;
using ProgVariableTypes = MudSharp.FutureProg.ProgVariableTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StockMeritsSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedAccount(FuturemudDatabaseContext context)
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
	}

	private static FutureProg CreateProg(long id, string name, ProgVariableTypes returnType, string text)
	{
		return new FutureProg
		{
			Id = id,
			FunctionName = name,
			FunctionComment = $"{name} test prog",
			FunctionText = text,
			ReturnType = (long)returnType,
			Category = "Tests",
			Subcategory = "StockMeritsSeeder",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
	}

	private static TraitDecorator CreateTraitDecorator(long id)
	{
		return new TraitDecorator
		{
			Id = id,
			Name = "Test Decorator",
			Type = "Basic",
			Contents = "<Decorator />"
		};
	}

	private static TraitDefinition CreateTraitDefinition(long id, string name, string traitGroup, int type)
	{
		return new TraitDefinition
		{
			Id = id,
			Name = name,
			Type = type,
			DecoratorId = 1,
			TraitGroup = traitGroup,
			DerivedType = 0,
			ChargenBlurb = $"{name} blurb",
			BranchMultiplier = 1.0,
			Alias = name.ToLowerInvariant(),
			TeachDifficulty = 0,
			LearnDifficulty = 0,
			ValueExpression = "0",
			DisplayOrder = (int)id,
			DisplayAsSubAttribute = false,
			ShowInScoreCommand = true,
			ShowInAttributeCommand = true
		};
	}

	private static BodyProto CreateBodyProto(long id, string name)
	{
		return new BodyProto
		{
			Id = id,
			Name = name,
			ConsiderString = string.Empty,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WielderDescriptionPlural = "hands",
			WielderDescriptionSingle = "hand",
			NameForTracking = name
		};
	}

	private static bool HasNaturalAttackExample(FuturemudDatabaseContext context, MeleeWeaponVerb verb, MeritType meritType)
	{
		return context.Merits
			.AsEnumerable()
			.Any(x =>
			x.Type == "Natural Attack Quality" &&
			x.MeritType == (int)meritType &&
			int.Parse(XElement.Parse(x.Definition).Attribute("verb")?.Value ?? "-1") == (int)verb);
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context)
	{
		SeedAccount(context);
		context.FutureProgs.Add(CreateProg(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"));
		context.Races.Add(new Race
		{
			Id = 1,
			Name = "Human",
			Description = "Human test race",
			BaseBodyId = 1,
			AllowedGenders = "Male Female Neuter NonBinary",
			AttributeBonusProgId = 1,
			DiceExpression = "1d100",
			CorpseModelId = 0,
			DefaultHealthStrategyId = 0,
			BreathingModel = "Simple",
			CommunicationStrategyType = "HumanoidCommunicationStrategy",
			HandednessOptions = "Left,Right",
			MaximumDragWeightExpression = "100",
			MaximumLiftWeightExpression = "100",
			EatCorpseEmoteText = "@ eat|eats $0.",
			BreathingVolumeExpression = "1",
			HoldBreathLengthExpression = "1"
		});
		context.TraitDecorators.Add(CreateTraitDecorator(1));
		context.TraitDefinitions.AddRange(
			CreateTraitDefinition(1, "Strength", "attribute", 0),
			CreateTraitDefinition(2, "Constitution", "attribute", 0),
			CreateTraitDefinition(3, "Perception", "attribute", 0),
			CreateTraitDefinition(4, "Willpower", "attribute", 0));

		var body = CreateBodyProto(1, "Humanoid");
		context.BodyProtos.Add(body);
		context.MoveSpeeds.AddRange(
			new MoveSpeed
			{
				Id = 1,
				BodyProto = body,
				BodyProtoId = body.Id,
				Alias = "run",
				FirstPersonVerb = "run",
				ThirdPersonVerb = "runs",
				PresentParticiple = "running",
				PositionId = 1,
				Multiplier = 1.0,
				StaminaMultiplier = 1.0
			},
			new MoveSpeed
			{
				Id = 2,
				BodyProto = body,
				BodyProtoId = body.Id,
				Alias = "sprint",
				FirstPersonVerb = "sprint",
				ThirdPersonVerb = "sprints",
				PresentParticiple = "sprinting",
				PositionId = 1,
				Multiplier = 1.0,
				StaminaMultiplier = 1.0
			});

		context.ChargenScreenStoryboards.Add(new MudSharp.Models.ChargenScreenStoryboard
		{
			Id = 1,
			ChargenStage = (int)ChargenStage.SelectMerits,
			ChargenType = "MeritPicker",
			Order = 1,
			StageDefinition = "<Definition stage=\"select-merits\" />",
			NextStage = 0
		});
		context.SaveChanges();
	}

	[TestMethod]
	public void ShouldSeedData_WithPrerequisitesAndNoStockPackage_ReturnsReadyToInstall()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new StockMeritsSeeder();

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));
	}

	[TestMethod]
	public void SeedData_RerunDoesNotDuplicateStockMeritsOrHelperProgs()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new StockMeritsSeeder();

		seeder.SeedData(context, new Dictionary<string, string>());
		seeder.SeedData(context, new Dictionary<string, string>());

		foreach (var meritName in StockMeritsSeeder.StockMeritNamesForTesting)
		{
			Assert.AreEqual(1, context.Merits.Count(x => x.Name == meritName), $"Expected a single stock merit named {meritName}.");
		}

		foreach (var progName in StockMeritsSeeder.HelperProgNamesForTesting)
		{
			Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == progName), $"Expected a single helper prog named {progName}.");
		}
	}

	[TestMethod]
	public void SeedData_HelperProgsUseTerrainTagsRatherThanTerrainNames()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new StockMeritsSeeder();

		seeder.SeedData(context, new Dictionary<string, string>());

		var terrainProgTexts = context.FutureProgs
			.Where(x => StockMeritsSeeder.HelperProgNamesForTesting.Contains(x.FunctionName) &&
			            x.FunctionName.StartsWith("StockMeritsIs", StringComparison.Ordinal) &&
			            !x.FunctionName.Equals("StockMeritsIsDark", StringComparison.Ordinal))
			.Select(x => x.FunctionText)
			.ToList();

		Assert.IsTrue(terrainProgTexts.Count > 0);
		Assert.IsTrue(terrainProgTexts.All(x => x.Contains("istagged(@ch.Location.Terrain,")));
		Assert.IsTrue(terrainProgTexts.All(x => !x.Contains("\"Beach\"")));
		Assert.IsTrue(terrainProgTexts.All(x => !x.Contains("\"City\"")));
		Assert.IsTrue(terrainProgTexts.All(x => !x.Contains("switch")));
	}

	[TestMethod]
	public void SeedData_NaturalAttackQualityExamplesExistForPunchKickAndBiteOnBothSides()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new StockMeritsSeeder();

		seeder.SeedData(context, new Dictionary<string, string>());

		foreach (var verb in new[] { MeleeWeaponVerb.Punch, MeleeWeaponVerb.Kick, MeleeWeaponVerb.Bite })
		{
			Assert.IsTrue(HasNaturalAttackExample(context, verb, MeritType.Merit), $"Expected a merit-side natural attack example for {verb}.");
			Assert.IsTrue(HasNaturalAttackExample(context, verb, MeritType.Flaw), $"Expected a flaw-side natural attack example for {verb}.");
		}
	}

	[TestMethod]
	public void SeedData_DoesNotAlterExistingChargenMeritStoryboard()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new StockMeritsSeeder();
		var storyboard = context.ChargenScreenStoryboards.Single();
		var originalStageDefinition = storyboard.StageDefinition;
		var originalChargenType = storyboard.ChargenType;
		var originalCount = context.ChargenScreenStoryboards.Count();

		seeder.SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual(originalCount, context.ChargenScreenStoryboards.Count());
		Assert.AreEqual(originalChargenType, context.ChargenScreenStoryboards.Single().ChargenType);
		Assert.AreEqual(originalStageDefinition, context.ChargenScreenStoryboards.Single().StageDefinition);
	}
}
