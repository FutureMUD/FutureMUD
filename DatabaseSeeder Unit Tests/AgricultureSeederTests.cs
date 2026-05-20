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
		context.SaveChanges();
	}

	[TestMethod]
	public void AgricultureSeeder_InstallsStockDefinitionsOperationsAndProjectsIdempotently()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		AgricultureSeeder seeder = new();

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));
		var metadata = ((IDatabaseSeeder)seeder).Metadata;
		Assert.AreEqual(SeederRepeatabilityMode.Idempotent, metadata.RepeatabilityMode);
		Assert.AreEqual(SeederUpdateCapability.RepairExisting, metadata.UpdateCapability);

		seeder.SeedData(context, new Dictionary<string, string>());
		seeder.SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual(8, context.AgricultureFieldProfiles.Count());
		Assert.AreEqual(6, context.AgricultureCropDefinitions.Count());
		Assert.AreEqual(4, context.AgricultureHerdDefinitions.Count());
		Assert.AreEqual(3, context.AgricultureWoodlandDefinitions.Count());
		Assert.AreEqual(15, context.AgricultureOperations.Count());

		var pastureDefinition = XElement.Parse(context.AgricultureFieldProfiles.Single(x => x.Name == "Pasture").Definition);
		Assert.AreEqual("Fallow,Pasture", pastureDefinition.Attribute("uses")!.Value);
		Assert.AreEqual("75", pastureDefinition.Elements("Score").Single(x => x.Attribute("type")!.Value == "Pasture").Attribute("value")!.Value);

		var cerealDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Cereal Grain").Definition);
		Assert.AreEqual("90", cerealDefinition.Attribute("growthDays")!.Value);
		Assert.AreEqual("14", cerealDefinition.Attribute("harvestWindowDays")!.Value);

		var sow = context.AgricultureOperations.Single(x => x.Name == "Sow Crop");
		Assert.AreEqual((int)AgricultureOperationType.Sow, sow.OperationType);
		Assert.AreEqual((int)AgricultureTargetType.Crop, sow.TargetType);
		Assert.AreEqual((int)AgricultureFieldUse.Fallow, sow.RequiredUse);
		Assert.AreEqual((int)AgricultureFieldUse.Crop, sow.ResultUse);
		Assert.IsTrue(XElement.Parse(sow.Definition).Elements("Score").Any(x =>
			x.Attribute("type")!.Value == "Tilth" &&
			x.Attribute("value")!.Value == "-5"));

		var projectIds = context.Projects
			.Where(x => x.Name.StartsWith("Stock Agriculture:"))
			.Select(x => x.Id)
			.ToHashSet();
		Assert.AreEqual(15, projectIds.Count);
		Assert.IsTrue(projectIds.Contains(sow.ProjectId));

		var phaseIds = context.ProjectPhases
			.Where(x => projectIds.Contains(x.ProjectId))
			.Select(x => x.Id)
			.ToHashSet();
		Assert.AreEqual(projectIds.Count, phaseIds.Count);
		Assert.AreEqual(projectIds.Count, context.ProjectActions.Count(x => phaseIds.Contains(x.ProjectPhaseId) && x.Type == "agriculture"));
		Assert.AreEqual(projectIds.Count, context.ProjectLabourRequirements.Count(x => phaseIds.Contains(x.ProjectPhaseId) && x.Type == "simple"));

		var sowProject = context.Projects.Single(x => x.Id == sow.ProjectId && x.RevisionNumber == sow.ProjectRevisionNumber);
		var projectDefinition = XElement.Parse(sowProject.Definition);
		Assert.AreEqual("1", projectDefinition.Element("AppearInProjectListProg")!.Value);
		Assert.AreEqual("1", projectDefinition.Element("CanInitiateProg")!.Value);
		Assert.IsFalse(sowProject.AppearInJobsList);

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
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
		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));
	}
}
