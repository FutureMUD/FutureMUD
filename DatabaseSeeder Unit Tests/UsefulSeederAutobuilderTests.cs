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

	[TestMethod]
	public void ClassifyAutobuilderPackagePresence_NonePartialAndFull_ReturnExpectedStates()
	{
		using FuturemudDatabaseContext context = BuildContext();

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));

		context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
		{
			Id = 1,
			Name = UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.First(),
			TemplateType = "room by terrain",
			Definition = "<Template />"
		});
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));

		context.AutobuilderRoomTemplates.RemoveRange(context.AutobuilderRoomTemplates.ToList());
		context.AutobuilderRoomTemplates.AddRange(
			UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting.Select((name, index) => new AutobuilderRoomTemplate
			{
				Id = index + 1,
				Name = name,
				TemplateType = index == 0 ? "room by terrain" : "room random description",
				Definition = "<Template />"
			}));
		context.SaveChanges();

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

		seeder.SeedTerrainAutobuilderForTesting(context);

		Assert.IsFalse(question.Filter(context, new Dictionary<string, string>()));
	}

	[TestMethod]
	public void SeedData_AutobuilderOnlyAnswers_InstallsStockTerrainAwareRoomTemplates()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedAccount(context);
		SeedTerrainFoundations(context);
		UsefulSeeder seeder = new();

		string result = seeder.SeedData(context, BuildAutobuilderOnlyAnswers());

		Assert.AreEqual("The operation completed successfully.", result);
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, UsefulSeeder.ClassifyAutobuilderPackagePresence(context));

		foreach (string name in UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting)
		{
			Assert.AreEqual(1, context.AutobuilderRoomTemplates.Count(x => x.Name == name),
				$"Expected a single autobuilder template named {name}.");
		}

		XElement baseline = GetTemplateDefinition(context, "Seeded Terrain Baseline");
		XElement random = GetTemplateDefinition(context, "Seeded Terrain Random Description");
		List<Terrain> nonVoidTerrains = context.Terrains
			.Where(x => !string.Equals(x.Name, "Void", StringComparison.OrdinalIgnoreCase))
			.ToList();

		Assert.AreEqual("room by terrain", context.AutobuilderRoomTemplates.Single(x => x.Name == "Seeded Terrain Baseline").TemplateType);
		Assert.AreEqual("room random description", context.AutobuilderRoomTemplates.Single(x => x.Name == "Seeded Terrain Random Description").TemplateType);
		Assert.AreEqual(nonVoidTerrains.Count - 1, baseline.Element("Terrains")!.Elements("Terrain").Count());
		Assert.AreEqual(nonVoidTerrains.Count - 1, random.Element("Terrains")!.Elements("Terrain").Count());
		Assert.IsTrue(random.Element("Descriptions")!.Elements("Description").Any());
		Assert.AreEqual("2+1d2", random.Element("Default")!.Element("NumberOfRandomElements")!.Value);
		Assert.IsTrue(random.Element("Terrains")!.Elements("Terrain").All(x => x.Element("NumberOfRandomElements") != null));
		Assert.IsFalse(random
			.Descendants("DefaultTerrain")
			.Any(x => x.Value == "1"),
			"Void terrain should not be part of the seeded autobuilder package.");
	}

	[TestMethod]
	public void SeedTerrainAutobuilderForTesting_RerunRepairsTemplatesWithoutDuplicates()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedAccount(context);
		SeedTerrainFoundations(context);
		UsefulSeeder seeder = new();

		seeder.SeedTerrainAutobuilderForTesting(context);

		AutobuilderRoomTemplate randomTemplate =
			context.AutobuilderRoomTemplates.Single(x => x.Name == "Seeded Terrain Random Description");
		randomTemplate.Definition = "<Template><Descriptions /></Template>";
		context.AutobuilderRoomTemplates.Remove(
			context.AutobuilderRoomTemplates.Single(x => x.Name == "Seeded Terrain Baseline"));
		context.SaveChanges();

		seeder.SeedTerrainAutobuilderForTesting(context);

		foreach (string name in UsefulSeeder.StockAutobuilderRoomTemplateNamesForTesting)
		{
			Assert.AreEqual(1, context.AutobuilderRoomTemplates.Count(x => x.Name == name),
				$"Expected rerun to preserve exactly one autobuilder template named {name}.");
		}

		XElement repairedRandom = GetTemplateDefinition(context, "Seeded Terrain Random Description");
		Assert.IsTrue(repairedRandom.Element("Descriptions")!.Elements("Description").Any());
		Assert.IsTrue(repairedRandom.Element("Terrains")!.Elements("Terrain").All(x => x.Element("NumberOfRandomElements") != null));
	}
}
