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
			new Tag { Id = 3, Name = "Agriculture Seedable" });
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

	private static void AssertPlantingGroups(FuturemudDatabaseContext context, string cropName, params string[] expected)
	{
		var definition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == cropName).Definition);
		var actual = definition.Element("PlantingWindows")!
		                       .Elements("Window")
		                       .Where(x => x.Attribute("type")!.Value == "group")
		                       .Select(x => x.Attribute("value")!.Value)
		                       .ToArray();
		CollectionAssert.AreEquivalent(expected, actual, cropName);
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
		Assert.IsTrue(MudSharp.Framework.DefaultStaticSettings.DefaultStaticConfigurations.ContainsKey(AgricultureScoreTypeExtensions.CustomScoreConfigurationStaticConfiguration));
		var customScoreDefaults = XElement.Parse(MudSharp.Framework.DefaultStaticSettings.DefaultStaticConfigurations[AgricultureScoreTypeExtensions.CustomScoreConfigurationStaticConfiguration]);
		Assert.AreEqual(12, customScoreDefaults.Elements("Score").Count());
		Assert.IsFalse(customScoreDefaults.Elements("Score").Any(x => (bool)x.Attribute("enabled")!));
		Assert.IsTrue(MudSharp.Framework.DefaultStaticSettings.DefaultStaticConfigurations.ContainsKey(AgriculturePlantingWindowExtensions.SeasonGroupWindowsStaticConfiguration));
		var seasonGroupDefaults = XElement.Parse(MudSharp.Framework.DefaultStaticSettings.DefaultStaticConfigurations[AgriculturePlantingWindowExtensions.SeasonGroupWindowsStaticConfiguration]);
		CollectionAssert.AreEquivalent(
			new[] { "Winter", "Spring", "Summer", "Autumn" },
			seasonGroupDefaults.Elements("Group").Select(x => x.Attribute("name")!.Value).ToArray());
		Assert.IsTrue(seasonGroupDefaults.Elements("Group")
		                                 .Single(x => x.Attribute("name")!.Value == "Winter")
		                                 .Elements("Range")
		                                 .Any(x =>
			                                 double.Parse(x.Attribute("start")!.Value, System.Globalization.CultureInfo.InvariantCulture) >
			                                 double.Parse(x.Attribute("end")!.Value, System.Globalization.CultureInfo.InvariantCulture)));

		seeder.SeedData(context, new Dictionary<string, string>());
		seeder.SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual(8, context.AgricultureFieldProfiles.Count());
		Assert.AreEqual(58, context.AgricultureCropDefinitions.Count());
		Assert.AreEqual(4, context.AgricultureHerdDefinitions.Count());
		Assert.AreEqual(8, context.AgricultureWoodlandDefinitions.Count());
		Assert.AreEqual(18, context.AgricultureOperations.Count());
		foreach (var cropName in new[]
		         {
			         "Wheat", "Barley", "Rye", "Oats", "Quinoa", "Field Beans", "Peas", "Lentils", "Potatoes",
			         "Carrots", "Beetroot", "Turnips", "Onions", "Cabbage", "Lettuce", "Sugar Beet", "Canola",
			         "Flax"
		         })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Spring");
		}

		AssertPlantingGroups(context, "Garlic", "Autumn", "Winter");
		foreach (var cropName in new[]
		         {
			         "Rice", "Maize", "Sorghum", "Millet", "Buckwheat", "Chickpeas", "Soybeans", "Peanuts",
			         "Sweet Potatoes", "Tomatoes", "Cucumbers", "Pumpkins", "Squash", "Peppers", "Sunflower",
			         "Hemp"
		         })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer");
		}

		foreach (var cropName in new[] { "Cassava", "Taro", "Yams", "Sugarcane", "Sesame", "Cotton", "Jute", "Ramie", "Sisal" })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer", "Autumn");
		}

		foreach (var cropName in new[] { "Grapes", "Apples", "Pears", "Peaches", "Plums", "Cherries", "Almonds", "Hazelnuts" })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Winter", "Spring");
		}

		foreach (var cropName in new[] { "Olives", "Figs", "Oranges", "Lemons" })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Spring");
		}

		foreach (var cropName in new[] { "Dates", "Bananas" })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer", "Autumn");
		}

		var pastureDefinition = XElement.Parse(context.AgricultureFieldProfiles.Single(x => x.Name == "Pasture").Definition);
		Assert.AreEqual("Fallow,Pasture", pastureDefinition.Attribute("uses")!.Value);
		Assert.AreEqual("75", pastureDefinition.Elements("Score").Single(x => x.Attribute("type")!.Value == "Pasture").Attribute("value")!.Value);

		var orchardProfile = XElement.Parse(context.AgricultureFieldProfiles.Single(x => x.Name == "Orchard Grove").Definition);
		Assert.AreEqual("Fallow,Orchard", orchardProfile.Attribute("uses")!.Value);

		var wheatDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Wheat").Definition);
		Assert.AreEqual("110", wheatDefinition.Attribute("growthDays")!.Value);
		Assert.AreEqual("18", wheatDefinition.Attribute("harvestWindowDays")!.Value);
		Assert.AreEqual("false", wheatDefinition.Attribute("perennial")!.Value);
		Assert.IsTrue(wheatDefinition.Element("Seeds")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "wheat" &&
			x.Attribute("tag")!.Value == "Seeds"));
		Assert.IsTrue(wheatDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "wheat" &&
			x.Attribute("weight")!.Value == "2500000" &&
			x.Attribute("tag")!.Value == "Seeded Yield"));

		var applesDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Apples").Definition);
		Assert.AreEqual("true", applesDefinition.Attribute("perennial")!.Value);
		Assert.AreEqual("1095", applesDefinition.Attribute("growthDays")!.Value);
		Assert.AreEqual("220", applesDefinition.Attribute("harvestCycleDays")!.Value);
		Assert.IsTrue(applesDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "apple" &&
			x.Attribute("tag")!.Value == "Seeded Yield"));

		var hazelDefinition = XElement.Parse(context.AgricultureWoodlandDefinitions.Single(x => x.Name == "Hazel Coppice").Definition);
		Assert.AreEqual("180", hazelDefinition.Attribute("establishmentDays")!.Value);
		Assert.IsTrue(hazelDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "hazel"));

		var sow = context.AgricultureOperations.Single(x => x.Name == "Sow Crop");
		Assert.AreEqual((int)AgricultureOperationType.Sow, sow.OperationType);
		Assert.AreEqual((int)AgricultureTargetType.Crop, sow.TargetType);
		Assert.AreEqual((int)AgricultureFieldUse.Fallow, sow.RequiredUse);
		Assert.AreEqual((int)AgricultureFieldUse.Crop, sow.ResultUse);
		Assert.IsTrue(XElement.Parse(sow.Definition).Elements("Score").Any(x =>
			x.Attribute("type")!.Value == "Tilth" &&
			x.Attribute("value")!.Value == "-4"));

		var plantOrchard = context.AgricultureOperations.Single(x => x.Name == "Plant Orchard");
		Assert.AreEqual((int)AgricultureOperationType.PlantOrchard, plantOrchard.OperationType);
		Assert.AreEqual((int)AgricultureFieldUse.Orchard, plantOrchard.ResultUse);

		var harvestOrchard = context.AgricultureOperations.Single(x => x.Name == "Harvest Orchard");
		Assert.AreEqual((int)AgricultureOperationType.Harvest, harvestOrchard.OperationType);
		Assert.AreEqual((int)AgricultureFieldUse.Orchard, harvestOrchard.RequiredUse);
		Assert.AreEqual((int)AgricultureFieldUse.Orchard, harvestOrchard.ResultUse);

		var coppice = context.AgricultureOperations.Single(x => x.Name == "Coppice Woodland");
		var coppiceDefinition = XElement.Parse(coppice.Definition);
		Assert.AreEqual("0.45", coppiceDefinition.Attribute("woodlandYieldMultiplier")!.Value);
		Assert.AreEqual("45", coppiceDefinition.Attribute("woodlandYieldCost")!.Value);

		var projectIds = context.Projects
			.Where(x => x.Name.StartsWith("Stock Agriculture:"))
			.Select(x => x.Id)
			.ToHashSet();
		Assert.AreEqual(18, projectIds.Count);
		Assert.IsTrue(projectIds.Contains(sow.ProjectId));

		var phaseIds = context.ProjectPhases
			.Where(x => projectIds.Contains(x.ProjectId))
			.Select(x => x.Id)
			.ToHashSet();
		Assert.AreEqual(projectIds.Count, phaseIds.Count);
		Assert.AreEqual(projectIds.Count, context.ProjectActions.Count(x => phaseIds.Contains(x.ProjectPhaseId) && x.Type == "agriculture"));
		Assert.AreEqual(projectIds.Count, context.ProjectLabourRequirements.Count(x => phaseIds.Contains(x.ProjectPhaseId) && x.Type == "simple"));
		Assert.AreEqual(projectIds.Count, context.ProjectLabourRequirements.Count(x => phaseIds.Contains(x.ProjectPhaseId) && x.Type == "supervision"));

		var sowProject = context.Projects.Single(x => x.Id == sow.ProjectId && x.RevisionNumber == sow.ProjectRevisionNumber);
		var projectDefinition = XElement.Parse(sowProject.Definition);
		Assert.AreEqual("1", projectDefinition.Element("AppearInProjectListProg")!.Value);
		Assert.AreEqual("1", projectDefinition.Element("CanInitiateProg")!.Value);
		Assert.IsFalse(sowProject.AppearInJobsList);
		var sowPhase = context.ProjectPhases.Single(x =>
			x.ProjectId == sow.ProjectId &&
			x.ProjectRevisionNumber == sow.ProjectRevisionNumber);
		Assert.AreEqual(32.0, context.ProjectLabourRequirements.Single(x => x.ProjectPhaseId == sowPhase.Id && x.Name == "Field Labour").TotalProgressRequired);
		var supervision = context.ProjectLabourRequirements.Single(x => x.ProjectPhaseId == sowPhase.Id && x.Name == "Agricultural Supervision");
		Assert.AreEqual("supervision", supervision.Type);
		var supervisionDefinition = XElement.Parse(supervision.Definition);
		Assert.AreEqual("false", supervisionDefinition.Element("Mandatory")!.Value);
		Assert.AreEqual("10", supervisionDefinition.Element("RequiredTrait")!.Value);
		Assert.AreEqual("15", supervisionDefinition.Element("MinimumTraitValue")!.Value);
		Assert.AreEqual("1.35", supervisionDefinition.Element("MultiplierForOtherLabours")!.Value);
		Assert.AreEqual("true", supervisionDefinition.Element("TraitScaledMultiplier")!.Value);
		var seedRequirement = context.ProjectMaterialRequirements.Single(x => x.ProjectPhaseId == sowPhase.Id && x.Name == "Seed Stock");
		Assert.AreEqual("commoditytag", seedRequirement.Type);
		var seedDefinition = XElement.Parse(seedRequirement.Definition);
		Assert.AreEqual("3", seedDefinition.Element("MaterialTag")!.Value);
		Assert.AreEqual("1", seedDefinition.Element("Tag")!.Value);
		Assert.AreEqual("100000", seedDefinition.Element("Amount")!.Value);

		var orchardProject = context.Projects.Single(x => x.Id == plantOrchard.ProjectId && x.RevisionNumber == plantOrchard.ProjectRevisionNumber);
		var orchardPhase = context.ProjectPhases.Single(x =>
			x.ProjectId == orchardProject.Id &&
			x.ProjectRevisionNumber == orchardProject.RevisionNumber);
		var orchardSeedRequirement = context.ProjectMaterialRequirements.Single(x => x.ProjectPhaseId == orchardPhase.Id && x.Name == "Seed Stock");
		Assert.AreEqual("250000", XElement.Parse(orchardSeedRequirement.Definition).Element("Amount")!.Value);

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
		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(context));

		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Seeds" },
			new Tag { Id = 2, Name = "Seeded Yield" },
			new Tag { Id = 3, Name = "Agriculture Seedable" });
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
