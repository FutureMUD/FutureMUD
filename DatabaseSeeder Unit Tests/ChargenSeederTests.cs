#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ProgVariableTypes = MudSharp.FutureProg.ProgVariableTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ChargenSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
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
			Subcategory = "ChargenSeeder",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
	}

	private static void SeedAccount(FuturemudDatabaseContext context, long id)
	{
		context.Accounts.Add(new Account
		{
			Id = id,
			Name = $"SeederTest{id}",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = $"seeder{id}@example.com",
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

	private static void SeedHumanRace(FuturemudDatabaseContext context)
	{
		context.FutureProgs.Add(CreateProg(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"));
		context.BodyProtos.Add(new BodyProto
		{
			Id = 1,
			Name = "Humanoid",
			ConsiderString = string.Empty,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WielderDescriptionPlural = "hands",
			WielderDescriptionSingle = "hand",
			NameForTracking = "Humanoid"
		});

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
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context, params long[] accountIds)
	{
		foreach (long accountId in accountIds.DefaultIfEmpty(1L))
		{
			SeedAccount(context, accountId);
		}

		SeedHumanRace(context);
		context.SaveChanges();
	}

	private static Dictionary<string, string> BuildAnswers(
		string attributeMode = "order",
		string skillMode = "picker",
		string meritsMode = "merit",
		string roleFirst = "race",
		string rpp = "no",
		string bp = "no",
		string classMode = "no",
		string customDescriptions = "no")
	{
		Dictionary<string, string> answers = new()
		{
			["rpp"] = rpp,
			["bp"] = bp,
			["class"] = classMode,
			["role-first"] = roleFirst,
			["attributemode"] = attributeMode,
			["skillmode"] = skillMode,
			["merits"] = meritsMode,
			["customdescs"] = customDescriptions
		};

		if (string.Equals(rpp, "yes", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(rpp, "y", StringComparison.OrdinalIgnoreCase))
		{
			answers["rppname"] = "Roleplay Point/RPP";
		}

		if (string.Equals(classMode, "yes", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(classMode, "y", StringComparison.OrdinalIgnoreCase))
		{
			answers["subclass"] = "no";
		}

		return answers;
	}

	private static MudSharp.Models.ChargenScreenStoryboard GetStage(FuturemudDatabaseContext context, ChargenStage stage)
	{
		return context.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)stage);
	}

	[TestMethod]
	public void ShouldSeedData_WithPrerequisitesAndNoChargenPackage_ReturnsReadyToInstall()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context, 7);
		ChargenSeeder seeder = new();

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));
	}

	[TestMethod]
	public void SeedData_RerunDoesNotDuplicateChargenStagesOrDefaultStartingLocationRole()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context, 7);
		ChargenSeeder seeder = new();
		ChargenStage[] requiredStages =
		[
			ChargenStage.Welcome,
			ChargenStage.SpecialApplication,
			ChargenStage.SelectRole,
			ChargenStage.SelectRace,
			ChargenStage.SelectEthnicity,
			ChargenStage.SelectGender,
			ChargenStage.SelectCulture,
			ChargenStage.SelectHandedness,
			ChargenStage.SelectBirthday,
			ChargenStage.SelectHeight,
			ChargenStage.SelectWeight,
			ChargenStage.SelectName,
			ChargenStage.SelectDisfigurements,
			ChargenStage.SelectMerits,
			ChargenStage.SelectAttributes,
			ChargenStage.SelectSkills,
			ChargenStage.SelectAccents,
			ChargenStage.SelectKnowledges,
			ChargenStage.SelectCharacteristics,
			ChargenStage.SelectDescription,
			ChargenStage.SelectStartingLocation,
			ChargenStage.SelectNotes,
			ChargenStage.Submit,
			ChargenStage.Menu
		];

		seeder.SeedData(context, BuildAnswers(attributeMode: "order", skillMode: "picker", meritsMode: "merit", roleFirst: "race"));
		seeder.SeedData(context, BuildAnswers(attributeMode: "points", skillMode: "boosts", meritsMode: "quirk", roleFirst: "role", rpp: "yes"));

		foreach (ChargenStage stage in requiredStages)
		{
			Assert.AreEqual(1, context.ChargenScreenStoryboards.Count(x => x.ChargenStage == (int)stage),
				$"Expected a single storyboard for {stage}.");
		}

		Assert.AreEqual(1, context.ChargenRoles.Count(x =>
			x.Type == (int)MudSharp.CharacterCreation.Roles.ChargenRoleType.StartingLocation &&
			x.Name == "Default Starting Location"));
		Assert.AreEqual(7L, context.ChargenRoles.Single(x => x.Name == "Default Starting Location").PosterId);
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}

	[TestMethod]
	public void ShouldSeedData_MissingSingleChargenStage_ReturnsExtraPackagesAvailable_AndRerunRepairsIt()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ChargenSeeder seeder = new();

		seeder.SeedData(context, BuildAnswers());
		MudSharp.Models.ChargenScreenStoryboard submitStage = GetStage(context, ChargenStage.Submit);
		context.ChargenScreenStoryboards.Remove(submitStage);
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));

		seeder.SeedData(context, BuildAnswers());

		Assert.AreEqual(1, context.ChargenScreenStoryboards.Count(x => x.ChargenStage == (int)ChargenStage.Submit));
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}

	[TestMethod]
	public void ShouldSeedData_AttributePointBuyWithoutHelperProg_ReturnsExtraPackagesAvailable_AndRerunRepairsIt()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ChargenSeeder seeder = new();

		seeder.SeedData(context, BuildAnswers(attributeMode: "points", skillMode: "picker", bp: "yes"));
		FutureProg maximumBoostsProg = context.FutureProgs.Single(x => x.FunctionName == "MaximumAttributeBoosts");
		context.FutureProgs.Remove(maximumBoostsProg);
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));

		seeder.SeedData(context, BuildAnswers(attributeMode: "points", skillMode: "picker", bp: "yes"));

		Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "MaximumAttributeBoosts"));
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}

	[TestMethod]
	public void SeedData_SetsSpecialApplicationStaticConfiguration_AndFallsBackFromBoostsWithoutResources()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ChargenSeeder seeder = new();

		string result = seeder.SeedData(context, BuildAnswers(attributeMode: "points", skillMode: "boosts", rpp: "no", bp: "no"));

		Assert.IsTrue(result.Contains("free-only mode", StringComparison.OrdinalIgnoreCase));
		Assert.IsTrue(result.Contains("simpler skill picker", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual("0", context.StaticConfigurations.Single(x => x.SettingName == "SpecialApplicationCost").Definition);
		Assert.AreEqual("0", context.StaticConfigurations.Single(x => x.SettingName == "SpecialApplicationResource").Definition);
		Assert.AreEqual("SkillPicker", GetStage(context, ChargenStage.SelectSkills).ChargenType);

		XElement attributeDefinition = XElement.Parse(GetStage(context, ChargenStage.SelectAttributes).StageDefinition);
		Assert.AreEqual("0", attributeDefinition.Element("MaximumExtraBoosts")?.Value);
		Assert.AreEqual("0", attributeDefinition.Element("BoostResource")?.Value);
	}

	[TestMethod]
	public void SeedData_UsesConfiguredChargenResourceForSpecialApplicationAndSkillBoostScreens()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ChargenSeeder seeder = new();

		seeder.SeedData(context, BuildAnswers(attributeMode: "points", skillMode: "boosts", rpp: "yes", bp: "no"));

		ChargenResource rppResource = context.ChargenResources.Single(x => x.Alias == "rpp");
		Assert.AreEqual("2", context.StaticConfigurations.Single(x => x.SettingName == "SpecialApplicationCost").Definition);
		Assert.AreEqual(rppResource.Id.ToString(), context.StaticConfigurations.Single(x => x.SettingName == "SpecialApplicationResource").Definition);
		Assert.AreEqual("SkillCostPicker", GetStage(context, ChargenStage.SelectSkills).ChargenType);

		XElement skillDefinition = XElement.Parse(GetStage(context, ChargenStage.SelectSkills).StageDefinition);
		Assert.AreEqual(rppResource.Id.ToString(), skillDefinition.Element("BoostResource")?.Value);
		Assert.AreEqual("1", skillDefinition.Element("FreeBoostResource")?.Value);
	}
}
