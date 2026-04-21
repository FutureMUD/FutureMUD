#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederAutobuilderTests
{
	private static readonly string[] LegacyAutobuilderTemplateNames =
	[
		"Seeded Terrain Baseline",
		"Seeded Terrain Random Description"
	];

	private static readonly string[] BaseRoadTags =
	[
		"Animal Trail",
		"Trail",
		"Dirt Road",
		"Compacted Dirt Road",
		"Gravel Road",
		"Cobblestone Road",
		"Asphalt Road"
	];

	private static readonly string[] SupportedRoadBaseFeatures =
	[
		"Animal Trail",
		"Trail",
		"Dirt Road",
		"Compacted Dirt Road",
		"Gravel Road",
		"Cobblestone Road",
		"Asphalt Road"
	];

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
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
		context.SaveChanges();
	}

	private static void SeedMaterials(FuturemudDatabaseContext context)
	{
		CoreDataSeeder seeder = new();
		typeof(CoreDataSeeder)
			.GetMethod("SeedMaterials", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(seeder, [context]);
	}

	private static void SeedVoidTerrain(FuturemudDatabaseContext context)
	{
		context.Terrains.Add(new Terrain
		{
			Id = 1,
			Name = "Void",
			HideDifficulty = 0,
			SpotDifficulty = 0,
			InfectionType = 0,
			InfectionVirulence = 0,
			InfectionMultiplier = 0,
			StaminaCost = 0,
			TerrainANSIColour = "7",
			TerrainEditorColour = "#FFFFFFFF",
			TerrainBehaviourMode = "outdoors",
			DefaultTerrain = true,
			MovementRate = 0,
			ForagableProfileId = 0,
			AtmosphereType = "Gas",
			DefaultCellOutdoorsType = 0,
			TerrainEditorText = "Vo",
			CanHaveTracks = false,
			TrackIntensityMultiplierVisual = 1.0,
			TrackIntensityMultiplierOlfactory = 1.0,
			TagInformation = string.Empty
		});
		context.SaveChanges();
	}

	private static void SeedTerrainFoundations(FuturemudDatabaseContext context)
	{
		SeedMaterials(context);
		SeedVoidTerrain(context);
		CoreDataSeeder.SeedTerrainFoundationsForTesting(context);
	}

	private static Dictionary<string, string> BuildAutobuilderOnlyAnswers()
	{
		return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["ai"] = "no",
			["covers"] = "no",
			["items"] = "no",
			["modernitems"] = "no",
			["tags"] = "no",
			["autobuilder"] = "yes",
			["hints"] = "no",
			["dreams"] = "no"
		};
	}

	private static XElement GetTemplateDefinition(FuturemudDatabaseContext context, string templateName)
	{
		return XElement.Parse(context.AutobuilderRoomTemplates.Single(x => x.Name == templateName).Definition);
	}

	private static XElement GetAreaTemplateDefinition(FuturemudDatabaseContext context, string templateName)
	{
		return XElement.Parse(context.AutobuilderAreaTemplates.Single(x => x.Name == templateName).Definition);
	}

	private static IEnumerable<string> SplitTags(string? tags)
	{
		return (tags ?? string.Empty)
			.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	}

	private static IEnumerable<string> GetFeatureNames(XElement group)
	{
		return group.Element("Features")?
			.Elements("Feature")
			.Select(x => x.Element("Name")?.Value ?? string.Empty)
			.Where(x => !string.IsNullOrWhiteSpace(x)) ?? Enumerable.Empty<string>();
	}

	[TestMethod]
	public void ClassifyAutobuilderPackagePresence_NonePartialAndFull_ReturnExpectedStates()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedTerrainFoundations(context);
		UsefulSeeder seeder = new();

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));

		context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
		{
			Id = 100000,
			Name = UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single(),
			TemplateType = "room random description",
			Definition = "<Template />"
		});
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));

		seeder.SeedTerrainAutobuilderForTesting(context);

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));
	}

	[TestMethod]
	public void SeederQuestions_AutobuilderQuestionAppearsOnlyWhenTerrainCatalogueExistsAndPackageIncomplete()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedAccount(context);
		UsefulSeeder seeder = new();
		var question = seeder.SeederQuestions.Single(x => x.Id == "autobuilder");

		Assert.IsFalse(question.Filter(context, new Dictionary<string, string>()));

		SeedTerrainFoundations(context);

		Assert.IsTrue(question.Filter(context, new Dictionary<string, string>()));

		context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
		{
			Id = 100001,
			Name = UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single(),
			TemplateType = "room random description",
			Definition = "<Template />"
		});
		context.SaveChanges();

		Assert.IsTrue(question.Filter(context, new Dictionary<string, string>()),
			"Partial autobuilder installs should still offer the question so reruns can repair the package.");

		seeder.SeedTerrainAutobuilderForTesting(context);

		Assert.IsFalse(question.Filter(context, new Dictionary<string, string>()));
	}

	[TestMethod]
	public void SeedData_AutobuilderOnlyAnswers_InstallsWildernessGroupedAutobuilderPackage()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedAccount(context);
		SeedTerrainFoundations(context);
		UsefulSeeder seeder = new();

		string result = seeder.SeedData(context, BuildAutobuilderOnlyAnswers());
		string roomTemplateName = UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single();
		string areaTemplateName = UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting.Single();

		Assert.AreEqual("The operation completed successfully.", result);
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));

		foreach (string name in UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting)
		{
			Assert.AreEqual(1, context.AutobuilderRoomTemplates.Count(x => x.Name == name),
				$"Expected a single autobuilder template named {name}.");
		}

		foreach (string name in UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting)
		{
			Assert.AreEqual(1, context.AutobuilderAreaTemplates.Count(x => x.Name == name),
				$"Expected a single autobuilder area template named {name}.");
		}

		foreach (string name in UsefulSeeder.StockAutobuilderTagNamesForTesting)
		{
			Assert.AreEqual(1, context.Tags.Count(x => x.Name == name),
				$"Expected the wilderness autobuilder tag {name} to be seeded exactly once.");
		}

		foreach (string legacyName in LegacyAutobuilderTemplateNames)
		{
			Assert.AreEqual(0, context.AutobuilderRoomTemplates.Count(x => x.Name == legacyName),
				$"Legacy starter template {legacyName} should not be newly installed by the replacement package.");
		}

		XElement roomTemplate = GetTemplateDefinition(context, roomTemplateName);
		XElement areaTemplate = GetAreaTemplateDefinition(context, areaTemplateName);
		List<Terrain> nonVoidTerrains = context.Terrains
			.Where(x => !string.Equals(x.Name, "Void", StringComparison.OrdinalIgnoreCase))
			.ToList();
		List<XElement> descriptionGroups = roomTemplate.Element("Descriptions")!.Elements("Description").ToList();
		List<XElement> roadDescriptions = roomTemplate
			.Descendants("Description")
			.Where(x => string.Equals((string?)x.Attribute("type"), "road", StringComparison.OrdinalIgnoreCase))
			.ToList();
		List<XElement> areaGroups = areaTemplate.Element("Groups")!.Elements("Group").ToList();
		List<XElement> uniformGroups = areaGroups
			.Where(x => string.Equals((string?)x.Attribute("type"), "uniform", StringComparison.OrdinalIgnoreCase))
			.ToList();
		List<XElement> simpleGroups = areaGroups
			.Where(x => string.Equals((string?)x.Attribute("type"), "simple", StringComparison.OrdinalIgnoreCase))
			.ToList();
		List<XElement> roadGroups = areaGroups
			.Where(x => string.Equals((string?)x.Attribute("type"), "road", StringComparison.OrdinalIgnoreCase))
			.ToList();

		Assert.AreEqual("room random description", context.AutobuilderRoomTemplates.Single(x => x.Name == roomTemplateName).TemplateType);
		Assert.AreEqual("room by terrain random features", context.AutobuilderAreaTemplates.Single(x => x.Name == areaTemplateName).TemplateType);
		Assert.AreEqual(nonVoidTerrains.Count - 1, roomTemplate.Element("Terrains")!.Elements("Terrain").Count());
		Assert.AreEqual("2+1d2", roomTemplate.Element("Default")!.Element("NumberOfRandomElements")!.Value);
		Assert.IsTrue(roomTemplate.Element("Terrains")!.Elements("Terrain").All(x => x.Element("NumberOfRandomElements") != null));
		Assert.IsTrue(descriptionGroups.Any(), "The wilderness room template should seed description groups.");
		Assert.IsTrue(descriptionGroups.Any(x => string.Equals((string?)x.Attribute("mandatory"), "true", StringComparison.OrdinalIgnoreCase) &&
		                                       string.Equals((string?)x.Attribute("fixedposition"), "1", StringComparison.OrdinalIgnoreCase)),
			"Primary physical description groups should be mandatory and fixed in the first sentence position.");
		Assert.IsTrue(descriptionGroups.Any(x => string.Equals((string?)x.Attribute("mandatory"), "true", StringComparison.OrdinalIgnoreCase) &&
		                                       string.Equals((string?)x.Attribute("fixedposition"), "2", StringComparison.OrdinalIgnoreCase)),
			"Secondary physical description groups should be mandatory and fixed in the second sentence position.");
		Assert.IsTrue(descriptionGroups
			.Where(x => string.Equals((string?)x.Attribute("mandatory"), "true", StringComparison.OrdinalIgnoreCase) &&
			            string.Equals((string?)x.Attribute("fixedposition"), "1", StringComparison.OrdinalIgnoreCase))
			.All(x => x.Elements("Description")
				.All(y => SplitTags(y.Element("Tags")?.Value).Contains("Physical Primary"))));
		Assert.IsTrue(descriptionGroups
			.Where(x => string.Equals((string?)x.Attribute("mandatory"), "true", StringComparison.OrdinalIgnoreCase) &&
			            string.Equals((string?)x.Attribute("fixedposition"), "2", StringComparison.OrdinalIgnoreCase))
			.All(x => x.Elements("Description")
				.All(y => SplitTags(y.Element("Tags")?.Value).Contains("Physical Secondary"))));
		Assert.IsTrue(roadDescriptions.Any(), "The wilderness room template should contain road-aware descriptions.");
		Assert.IsFalse(roadDescriptions.Any(x =>
			BaseRoadTags.Contains(SplitTags(x.Element("Tags")?.Value).FirstOrDefault() ?? string.Empty,
				StringComparer.OrdinalIgnoreCase)),
			"Road descriptions should only be keyed to topology tags that supply direction substitutions.");

		Assert.IsTrue(uniformGroups.Any(x => GetFeatureNames(x).Contains("Physical Primary")),
			"The area template should add a uniform primary-layer marker tag.");
		Assert.IsTrue(uniformGroups.Any(x => GetFeatureNames(x).Contains("Physical Secondary")),
			"The area template should add a uniform secondary-layer marker tag.");
		Assert.IsTrue(uniformGroups.Any(x => GetFeatureNames(x).Contains("Worn Furnishings")),
			"The area template should include the primary physical feature pool.");
		Assert.IsTrue(uniformGroups.Any(x => GetFeatureNames(x).Contains("Recent Cleaning")),
			"The area template should include the secondary physical feature pool.");
		Assert.IsTrue(simpleGroups.Any(x =>
			double.TryParse(x.Element("MinimumFeatureDensity")?.Value, out double min) &&
			double.TryParse(x.Element("MaximumFeatureDensity")?.Value, out double max) &&
			Math.Abs(min - 0.35) < 0.0001 &&
			Math.Abs(max - 0.65) < 0.0001),
			"The area template should include an optional sound feature density group.");
		Assert.IsTrue(simpleGroups.Any(x =>
			double.TryParse(x.Element("MinimumFeatureDensity")?.Value, out double min) &&
			double.TryParse(x.Element("MaximumFeatureDensity")?.Value, out double max) &&
			Math.Abs(min - 0.25) < 0.0001 &&
			Math.Abs(max - 0.55) < 0.0001),
			"The area template should include an optional smell feature density group.");
		Assert.IsTrue(simpleGroups.Any(x =>
			double.TryParse(x.Element("MinimumFeatureDensity")?.Value, out double min) &&
			double.TryParse(x.Element("MaximumFeatureDensity")?.Value, out double max) &&
			Math.Abs(min - 0.08) < 0.0001 &&
			Math.Abs(max - 0.18) < 0.0001),
			"The area template should include an optional resource feature density group.");
		CollectionAssert.AreEquivalent(
			SupportedRoadBaseFeatures,
			roadGroups.Select(x => x.Element("BaseFeature")!.Value).ToArray(),
			"The area template should seed all supported stock road topology groups.");
		Assert.IsFalse(roomTemplate
			.Descendants("DefaultTerrain")
			.Any(x => x.Value == "1"),
			"Void terrain should not be part of the seeded autobuilder package.");
	}

	[TestMethod]
	public void SeedTerrainAutobuilderForTesting_RerunRepairsTemplatesTagsAndPreservesLegacyTemplates()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedAccount(context);
		SeedTerrainFoundations(context);
		UsefulSeeder seeder = new();
		string roomTemplateName = UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Single();
		string areaTemplateName = UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting.Single();

		context.AutobuilderRoomTemplates.AddRange(
			LegacyAutobuilderTemplateNames.Select((name, index) => new AutobuilderRoomTemplate
			{
				Id = 200000 + index,
				Name = name,
				TemplateType = name == "Seeded Terrain Baseline" ? "room by terrain" : "room random description",
				Definition = "<Template />"
			}));
		context.SaveChanges();

		seeder.SeedTerrainAutobuilderForTesting(context);

		context.AutobuilderRoomTemplates.Single(x => x.Name == roomTemplateName).Definition =
			"<Template><Descriptions /></Template>";
		context.AutobuilderAreaTemplates.Remove(
			context.AutobuilderAreaTemplates.Single(x => x.Name == areaTemplateName));
		context.Tags.Remove(context.Tags.Single(x => x.Name == UsefulSeeder.StockAutobuilderTagNamesForTesting.Last()));
		context.SaveChanges();

		seeder.SeedTerrainAutobuilderForTesting(context);

		foreach (string name in UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting)
		{
			Assert.AreEqual(1, context.AutobuilderRoomTemplates.Count(x => x.Name == name),
				$"Expected rerun to preserve exactly one autobuilder template named {name}.");
		}

		foreach (string name in UsefulSeeder.StockAutobuilderAreaTemplateNamesForTesting)
		{
			Assert.AreEqual(1, context.AutobuilderAreaTemplates.Count(x => x.Name == name),
				$"Expected rerun to preserve exactly one autobuilder area template named {name}.");
		}

		foreach (string name in UsefulSeeder.StockAutobuilderTagNamesForTesting)
		{
			Assert.AreEqual(1, context.Tags.Count(x => x.Name == name),
				$"Expected rerun to restore missing autobuilder tag {name} without duplication.");
		}

		foreach (string legacyName in LegacyAutobuilderTemplateNames)
		{
			Assert.AreEqual(1, context.AutobuilderRoomTemplates.Count(x => x.Name == legacyName),
				$"Legacy template {legacyName} should remain untouched by wilderness autobuilder reruns.");
		}

		XElement repairedRoom = GetTemplateDefinition(context, roomTemplateName);
		XElement repairedArea = GetAreaTemplateDefinition(context, areaTemplateName);
		Assert.IsTrue(repairedRoom.Element("Descriptions")!.Elements("Description").Any());
		Assert.IsTrue(repairedRoom.Element("Terrains")!.Elements("Terrain").All(x => x.Element("NumberOfRandomElements") != null));
		Assert.IsTrue(repairedArea.Element("Groups")!.Elements("Group").Any(),
			"Rerun should rebuild the wilderness area template definition if it was deleted.");
	}
}
