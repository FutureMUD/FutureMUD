#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.Work.Agriculture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ProgVariableTypes = MudSharp.FutureProg.ProgVariableTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AgricultureSeederTests
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
			Subcategory = "AgricultureSeeder",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
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
		context.FutureProgs.Add(CreateProg(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"));
		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Seeds" },
			new Tag { Id = 2, Name = "Seeded Yield" },
			new Tag { Id = 3, Name = "Agriculture Seedable" },
			new Tag { Id = 4, Name = "Bee Hive" },
			new Tag { Id = 5, Name = "Hive Stand" },
			new Tag { Id = 6, Name = "Raw Honeycomb" },
			new Tag { Id = 7, Name = "Pressed Honey" },
			new Tag { Id = 8, Name = "Rendered Beeswax" },
			new Tag { Id = 9, Name = "Raw Milk" },
			new Tag { Id = 10, Name = "Raw Textile Fibre" },
			new Tag { Id = 11, Name = "Egg Product" },
			new Tag { Id = 12, Name = "Manure Commodity" });
		context.TraitDefinitions.Add(new TraitDefinition
		{
			Id = 10,
			Name = "Farming",
			Alias = "Farming",
			Type = 0,
			TraitGroup = "Professional",
			ValueExpression = string.Empty,
			BranchMultiplier = 1.0,
			ChargenBlurb = string.Empty
		});
		context.SaveChanges();
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void SeedData_InstallsAndRerunsWithoutDuplicatingDefinitionsOrProjects()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new AgricultureSeeder();
		var prior0 = context.AgricultureFieldProfiles.Select(x => x.Id).ToArray();
		var prior1 = context.AgricultureCropDefinitions.Select(x => x.Id).ToArray();
		var prior2 = context.AgricultureHerdDefinitions.Select(x => x.Id).ToArray();
		var prior3 = context.AgricultureWoodlandDefinitions.Select(x => x.Id).ToArray();
		var prior4 = context.Projects.Select(x => x.Id).ToArray();
		seeder.SeedData(context, new Dictionary<string, string>());
		var installed0 = context.AgricultureFieldProfiles.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		var installed1 = context.AgricultureCropDefinitions.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed1.Except(prior1).Any(), "The first installation must create records beyond its prerequisites.");
		var installed2 = context.AgricultureHerdDefinitions.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed2.Except(prior2).Any(), "The first installation must create records beyond its prerequisites.");
		var installed3 = context.AgricultureWoodlandDefinitions.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed3.Except(prior3).Any(), "The first installation must create records beyond its prerequisites.");
		var installed4 = context.Projects.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed4.Except(prior4).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedData(context, new Dictionary<string, string>());
		CollectionAssert.AreEqual(installed0, context.AgricultureFieldProfiles.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed1, context.AgricultureCropDefinitions.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed2, context.AgricultureHerdDefinitions.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed3, context.AgricultureWoodlandDefinitions.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed4, context.Projects.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void AgricultureSeeder_BlocksUntilCorePrerequisitesExist()
	{
		using FuturemudDatabaseContext context = BuildContext();
		AgricultureSeeder seeder = new();

		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(context));

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
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(context));

		context.FutureProgs.Add(CreateProg(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"));
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(context));

		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Seeds" },
			new Tag { Id = 2, Name = "Seeded Yield" },
			new Tag { Id = 3, Name = "Agriculture Seedable" });
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(context));

		context.Tags.AddRange(
			new Tag { Id = 4, Name = "Bee Hive" },
			new Tag { Id = 5, Name = "Hive Stand" },
			new Tag { Id = 6, Name = "Raw Honeycomb" },
			new Tag { Id = 7, Name = "Pressed Honey" },
			new Tag { Id = 8, Name = "Rendered Beeswax" });
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(context));

		context.Tags.AddRange(
			new Tag { Id = 9, Name = "Raw Milk" },
			new Tag { Id = 10, Name = "Raw Textile Fibre" },
			new Tag { Id = 11, Name = "Egg Product" },
			new Tag { Id = 12, Name = "Manure Commodity" });
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(context));

		context.TraitDefinitions.Add(new TraitDefinition
		{
			Id = 10,
			Name = "Farming",
			Alias = "Farming",
			Type = 0,
			TraitGroup = "Professional",
			ValueExpression = string.Empty,
			BranchMultiplier = 1.0,
			ChargenBlurb = string.Empty
		});
		context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));
	}
}
