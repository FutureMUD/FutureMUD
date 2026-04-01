#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using ProgVariableTypes = MudSharp.FutureProg.ProgVariableTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederAiPackageTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static MudSharp.Models.FutureProg CreateProg(long id, string name, ProgVariableTypes returnType, string text)
	{
		return new MudSharp.Models.FutureProg
		{
			Id = id,
			FunctionName = name,
			FunctionComment = $"{name} test prog",
			FunctionText = text,
			ReturnType = (long)returnType,
			Category = "Tests",
			Subcategory = "UsefulSeeder",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
	}

	private static void SeedAiPrerequisites(FuturemudDatabaseContext context)
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
		context.FutureProgs.AddRange(
			CreateProg(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"),
			CreateProg(2, "AlwaysFalse", ProgVariableTypes.Boolean, "return false"));
		context.SaveChanges();
	}

	private static Dictionary<string, string> BuildAiOnlyAnswers()
	{
		return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["ai"] = "yes",
			["terrain"] = "no",
			["covers"] = "no",
			["items"] = "no",
			["modernitems"] = "no",
			["tags"] = "no",
			["autobuilder"] = "no",
			["hints"] = "no"
		};
	}

	[TestMethod]
	public void SeederQuestions_ExposeSingleRepeatableAiQuestion()
	{
		var seeder = new UsefulSeeder();
		var ids = seeder.SeederQuestions.Select(x => x.Id).ToList();

		Assert.AreEqual(1, ids.Count(x => x == "ai"));
		Assert.IsFalse(ids.Contains("ai2"));
	}

	[TestMethod]
	public void ClassifyAiPackagePresence_NonePartialAndFull_ReturnExpectedStates()
	{
		using var context = BuildContext();
		var stockNames = UsefulSeeder.StockAiExampleNamesForTesting;

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyAiPackagePresence(context));

		context.ArtificialIntelligences.Add(new ArtificialIntelligence
		{
			Name = stockNames.First(),
			Type = "Test",
			Definition = "<Definition />"
		});
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyAiPackagePresence(context));

		context.ArtificialIntelligences.RemoveRange(context.ArtificialIntelligences.ToList());
		context.ArtificialIntelligences.AddRange(stockNames.Select((name, index) => new ArtificialIntelligence
		{
			Id = index + 1,
			Name = name,
			Type = "Test",
			Definition = "<Definition />"
		}));
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyAiPackagePresence(context));
	}

	[TestMethod]
	public void SeedData_AiOnlyAnswers_InstallsAllStockExamples()
	{
		using var context = BuildContext();
		SeedAiPrerequisites(context);
		var seeder = new UsefulSeeder();

		var result = seeder.SeedData(context, BuildAiOnlyAnswers());

		Assert.AreEqual("The operation completed successfully.", result);
		foreach (var name in UsefulSeeder.StockAiExampleNamesForTesting)
		{
			Assert.AreEqual(1, context.ArtificialIntelligences.Count(x => x.Name == name), $"Expected a single stock AI named {name}.");
		}
	}

	[TestMethod]
	public void SeedAIExamplesForTesting_RerunRestoresMissingExamplesWithoutDuplicates()
	{
		using var context = BuildContext();
		SeedAiPrerequisites(context);
		var seeder = new UsefulSeeder();

		seeder.SeedAIExamplesForTesting(context);
		context.ArtificialIntelligences.Remove(context.ArtificialIntelligences.First(x => x.Name == "ExampleArenaParticipant"));
		context.FutureProgs.Remove(context.FutureProgs.First(x => x.FunctionName == "LairScavengerFallbackHome"));
		context.SaveChanges();

		seeder.SeedAIExamplesForTesting(context);

		foreach (var name in UsefulSeeder.StockAiExampleNamesForTesting)
		{
			Assert.AreEqual(1, context.ArtificialIntelligences.Count(x => x.Name == name), $"Expected rerun to restore exactly one stock AI named {name}.");
		}

		Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == "LairScavengerFallbackHome"));
		Assert.AreEqual(1, context.VariableDefinitions.Count(x => x.Property == "npcownerid"));
		Assert.AreEqual(1, context.VariableDefaults.Count(x => x.Property == "npcownerid"));
	}
}
