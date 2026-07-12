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
	private const int ExpectedFieldProfileCount = 81;
	private const int ExpectedCropDefinitionCount = 215;
	private const int ExpectedHerdDefinitionCount = 27;
	private const int ExpectedWoodlandDefinitionCount = 51;
	private const int ExpectedOperationCount = 68;

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

	private static XElement CropDefinition(FuturemudDatabaseContext context, string cropName)
	{
		return XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == cropName).Definition);
	}

	private static XElement ProfileDefinition(FuturemudDatabaseContext context, string profileName)
	{
		return XElement.Parse(context.AgricultureFieldProfiles.Single(x => x.Name == profileName).Definition);
	}

	private static XElement HerdDefinition(FuturemudDatabaseContext context, string herdName)
	{
		return XElement.Parse(context.AgricultureHerdDefinitions.Single(x => x.Name == herdName).Definition);
	}

	private static XElement WoodlandDefinition(FuturemudDatabaseContext context, string woodlandName)
	{
		return XElement.Parse(context.AgricultureWoodlandDefinitions.Single(x => x.Name == woodlandName).Definition);
	}

	private static XElement OperationDefinition(FuturemudDatabaseContext context, string operationName)
	{
		return XElement.Parse(context.AgricultureOperations.Single(x => x.Name == operationName).Definition);
	}

	private static int ProfileScore(FuturemudDatabaseContext context, string profileName, AgricultureScoreType score)
	{
		return int.Parse(ProfileDefinition(context, profileName)
		                 .Elements("Score")
		                 .Single(x => x.Attribute("type")!.Value == score.ToString())
		                 .Attribute("value")!.Value);
	}

	private static void AssertCropHasSeededYieldAndSeedOutput(FuturemudDatabaseContext context, string cropName)
	{
		var definition = CropDefinition(context, cropName);
		Assert.IsTrue(definition.Element("PlantingWindows")!.Elements("Window").Any(), $"{cropName} should have planting windows.");
		Assert.IsTrue(definition.Element("Seeds")!.Elements("Commodity").Any(x => x.Attribute("tag")!.Value == "Seeds"),
			$"{cropName} should have a seed requirement.");
		Assert.IsTrue(definition.Element("Outputs")!.Elements("Commodity").Any(x => x.Attribute("tag")?.Value == "Seeded Yield"),
			$"{cropName} should have a primary seeded yield.");
		Assert.IsTrue(definition.Element("Outputs")!.Elements("Commodity").Any(x => x.Attribute("tag")?.Value == "Seeds"),
			$"{cropName} should recover seed output.");
	}

	private static void AssertHerdOutput(FuturemudDatabaseContext context, string herdName, string materialName, string tagName)
	{
		var outputs = HerdDefinition(context, herdName).Element("SecondaryOutputs")!.Elements("Commodity").ToArray();
		Assert.IsTrue(outputs.Any(x =>
				x.Attribute("material")!.Value == materialName &&
				x.Attribute("tag")!.Value == tagName),
			$"{herdName} should output {materialName} tagged {tagName}.");
	}

	private static void AssertWoodlandOutput(FuturemudDatabaseContext context, string woodlandName, string materialName)
	{
		Assert.IsTrue(WoodlandDefinition(context, woodlandName)
		              .Element("Outputs")!
		              .Elements("Commodity")
		              .Any(x => x.Attribute("material")!.Value == materialName),
			$"{woodlandName} should output {materialName}.");
	}

	private static void AssertOperationDelta(FuturemudDatabaseContext context, string operationName, AgricultureScoreType score, int value)
	{
		Assert.IsTrue(OperationDefinition(context, operationName)
		              .Elements("Score")
		              .Any(x =>
			              x.Attribute("type")!.Value == score.ToString() &&
			              x.Attribute("value")!.Value == value.ToString()),
			$"{operationName} should include {score} {value}.");
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

		Assert.AreEqual(ExpectedFieldProfileCount, context.AgricultureFieldProfiles.Count());
		Assert.AreEqual(ExpectedCropDefinitionCount, context.AgricultureCropDefinitions.Count());
		Assert.AreEqual(ExpectedHerdDefinitionCount, context.AgricultureHerdDefinitions.Count());
		Assert.AreEqual(ExpectedWoodlandDefinitionCount, context.AgricultureWoodlandDefinitions.Count());
		Assert.AreEqual(ExpectedOperationCount, context.AgricultureOperations.Count());

		foreach (var cropName in context.AgricultureCropDefinitions.Select(x => x.Name).ToArray())
		{
			AssertCropHasSeededYieldAndSeedOutput(context, cropName);
		}
		foreach (var cropName in new[]
		         {
			         "Wheat", "Barley", "Rye", "Oats", "Quinoa", "Field Beans", "Peas", "Lentils", "Potatoes",
			         "Carrots", "Beetroot", "Turnips", "Onions", "Cabbage", "Lettuce", "Sugar Beet", "Canola",
			         "Flax"
		         })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Spring");
		}

		foreach (var cropName in new[]
		         {
			         "Emmer Wheat", "Einkorn Wheat", "Spelt Wheat", "Naked Barley", "New Glume Wheat", "Bitter Vetch",
			         "Grass Peas", "Lupins", "Canihua", "Pitseed Goosefoot", "Maygrass", "Little Barley", "Erect Knotweed",
			         "Oca", "Ulluco", "Mashua", "Jerusalem Artichokes", "Mustard", "Madder", "Weld", "Alkanet",
			         "Woad", "Coriander", "Saffron Crocus", "Chamomile", "Lavender", "Yarrow", "Foxglove", "Henbane",
			         "Mandrake"
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

		foreach (var cropName in new[]
		         {
			         "Cowpeas", "Mung Beans", "Bambara Groundnuts", "Adzuki Beans", "Teff", "Fonio", "Finger Millet",
			         "Pearl Millet", "Foxtail Millet", "Proso Millet", "Amaranth", "Chia", "Marshelder", "African Rice",
			         "Eggplants", "Okra", "Bottle Gourds", "Safflower", "Niger Seed", "Cumin"
		         })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer");
		}

		foreach (var cropName in new[] { "Cassava", "Taro", "Yams", "Sugarcane", "Sesame", "Cotton", "Jute", "Ramie", "Sisal" })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer", "Autumn");
		}

		foreach (var cropName in new[]
		         {
			         "Pigeon Peas", "Arrowroot", "Lotus Root", "Water Chestnuts", "Kenaf", "Indigo", "Tobacco",
			         "Cardamom"
		         })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer", "Autumn");
		}

		foreach (var cropName in new[] { "Grapes", "Apples", "Pears", "Peaches", "Plums", "Cherries", "Almonds", "Hazelnuts" })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Winter", "Spring");
		}

		foreach (var cropName in new[] { "Quinces", "Apricots", "Walnuts", "Persimmons", "Mulberries", "Chestnuts", "Pecans", "Kiwifruit" })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Winter", "Spring");
		}

		foreach (var cropName in new[] { "Olives", "Figs", "Oranges", "Lemons" })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Spring");
		}

		foreach (var cropName in new[] { "Pomegranates", "Pistachios", "Limes", "Grapefruits", "Mandarins", "Carob" })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Spring");
		}

		foreach (var cropName in new[] { "Dates", "Bananas" })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer", "Autumn");
		}

		foreach (var cropName in new[]
		         {
			         "Mangoes", "Coconuts", "Plantains", "Breadfruit", "Avocados", "Cacao", "Coffee", "Tea", "Cashews",
			         "Macadamias", "Guavas", "Lychees", "Jackfruit", "Papayas", "Passionfruit", "Cinnamon", "Black Pepper", "Cloves",
			         "Nutmeg", "Henna", "Kola Nuts", "Tamarinds"
		         })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer", "Autumn");
		}

		foreach (var cropName in new[]
		         {
			         "Camas", "Maca", "Yacon", "Tarwi", "Ethiopian Oats", "Spinach", "Kale", "Chard", "Celery",
			         "Parsnips", "Radishes", "Leeks", "Rutabagas", "Chicory", "Sainfoin", "Clover", "Mangelwurzel",
			         "Poppy Seed"
		         })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Spring");
		}

		foreach (var cropName in new[]
		         {
			         "Tepary Beans", "Lima Beans", "Runner Beans", "Wild Rice", "Groundnut", "Prairie Turnip",
			         "Tomatillos", "Chayote", "Ahipa", "Guinea Millet", "Egusi Melons", "African Yam Beans",
			         "Kersting's Groundnuts", "Lablab Beans", "Fluted Pumpkins", "African Eggplants", "Melons",
			         "Watermelons"
		         })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer");
		}

		foreach (var cropName in new[] { "Jicama", "Arracacha", "Enset", "Roselle", "Jute Mallow" })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer", "Autumn");
		}

		foreach (var cropName in new[]
		         {
			         "Cranberries", "Blueberries", "Raspberries", "Blackberries", "Strawberries", "Hops", "Asparagus",
			         "Rhubarb", "Currants"
		         })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Winter", "Spring");
		}

		foreach (var cropName in new[] { "Nopal Cactus", "Agave", "Artichokes" })
		{
			AssertPlantingGroups(context, cropName, "Autumn", "Spring");
		}

		foreach (var cropName in new[]
		         {
			         "Henequen", "Pineapples", "Vanilla", "Yerba Mate", "Oil Palms", "Shea Trees", "Baobabs",
			         "Raffia Palms", "Allspice", "Logwood"
		         })
		{
			AssertPlantingGroups(context, cropName, "Spring", "Summer", "Autumn");
		}

		var pastureDefinition = XElement.Parse(context.AgricultureFieldProfiles.Single(x => x.Name == "Pasture").Definition);
		Assert.AreEqual("Fallow,Pasture", pastureDefinition.Attribute("uses")!.Value);
		Assert.AreEqual("75", pastureDefinition.Elements("Score").Single(x => x.Attribute("type")!.Value == "Pasture").Attribute("value")!.Value);

		var orchardProfile = XElement.Parse(context.AgricultureFieldProfiles.Single(x => x.Name == "Orchard Grove").Definition);
		Assert.AreEqual("Fallow,Orchard", orchardProfile.Attribute("uses")!.Value);

		var paddyDefinition = XElement.Parse(context.AgricultureFieldProfiles.Single(x => x.Name == "Paddy Field").Definition);
		Assert.AreEqual("Fallow,Crop", paddyDefinition.Attribute("uses")!.Value);
		Assert.AreEqual("90", paddyDefinition.Elements("Score").Single(x => x.Attribute("type")!.Value == "Moisture").Attribute("value")!.Value);

		var apiaryYardDefinition = XElement.Parse(context.AgricultureFieldProfiles.Single(x => x.Name == "Apiary Yard").Definition);
		Assert.AreEqual("Fallow,Crop,Orchard,Pasture,Woodland", apiaryYardDefinition.Attribute("uses")!.Value);

		foreach (var ladderProfile in new[]
		         {
			         "Exhausted Cropland", "Borderline Cropping Patch", "Poor Tenant Field", "Average Worked Field",
			         "Well-Worked Open Field", "Manured Market Garden", "Irrigated Goodfield", "Deep Loam Showpiece",
			         "Estate Model Farm", "Paradise Garden"
		         })
		{
			Assert.IsTrue(context.AgricultureFieldProfiles.Any(x => x.Name == ladderProfile),
				$"{ladderProfile} should be seeded as part of the cultivated-field quality ladder.");
		}

		var internationalProfiles = new[]
		{
			"Chinampa Garden", "Andean Waru-Waru Raised Field", "Sahel Millet Field", "Monsoon Rice Terrace",
			"Mangrove Rice Polder", "Volcanic Ash Garden"
		};
		var internationalSignatures = internationalProfiles
			.Select(profile => string.Join("|", ProfileDefinition(context, profile)
			                                .Elements("Score")
			                                .Select(x => $"{x.Attribute("type")!.Value}={x.Attribute("value")!.Value}")))
			.ToArray();
		Assert.AreEqual(internationalProfiles.Length, internationalSignatures.Distinct().Count(),
			"International profiles should have distinct score signatures.");

		Assert.IsTrue(ProfileScore(context, "Exhausted Cropland", AgricultureScoreType.Nutrients) <= 15);
		Assert.IsTrue(ProfileScore(context, "Exhausted Cropland", AgricultureScoreType.Condition) <= 15);
		Assert.IsTrue(ProfileScore(context, "Exhausted Cropland", AgricultureScoreType.Weeds) >= 75);
		Assert.IsTrue(ProfileScore(context, "Mangrove Rice Polder", AgricultureScoreType.Salinity) >= 70);
		Assert.IsTrue(ProfileScore(context, "Mangrove Rice Polder", AgricultureScoreType.Drainage) <= 20);
		Assert.IsTrue(ProfileScore(context, "Windblown Sand Field", AgricultureScoreType.Nutrients) <= 20);
		Assert.IsTrue(ProfileScore(context, "Windblown Sand Field", AgricultureScoreType.Topsoil) <= 20);
		Assert.IsTrue(ProfileScore(context, "Reclaimed Peat Field", AgricultureScoreType.Drainage) <= 30);
		Assert.IsTrue(ProfileScore(context, "Reclaimed Peat Field", AgricultureScoreType.Condition) <= 25);
		Assert.IsTrue(ProfileScore(context, "Laterite Upland Garden", AgricultureScoreType.Nutrients) <= 25);
		Assert.IsTrue(ProfileScore(context, "Laterite Upland Garden", AgricultureScoreType.Weeds) >= 70);

		foreach (var profile in context.AgricultureFieldProfiles)
		{
			var definition = XElement.Parse(profile.Definition);
			foreach (var score in definition.Elements("Score"))
			{
				var value = int.Parse(score.Attribute("value")!.Value);
				Assert.IsTrue(value is >= 0 and <= 100, $"{profile.Name} has a score outside 0-100.");
			}
		}

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
		Assert.AreEqual("None", wheatDefinition.Element("Pollination")!.Attribute("dependency")!.Value);

		var applesDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Apples").Definition);
		Assert.AreEqual("true", applesDefinition.Attribute("perennial")!.Value);
		Assert.AreEqual("1095", applesDefinition.Attribute("growthDays")!.Value);
		Assert.AreEqual("220", applesDefinition.Attribute("harvestCycleDays")!.Value);
		Assert.IsTrue(applesDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "apple" &&
			x.Attribute("tag")!.Value == "Seeded Yield"));
		Assert.AreEqual("Strong", applesDefinition.Element("Pollination")!.Attribute("dependency")!.Value);
		Assert.AreEqual("1", applesDefinition.Element("Pollination")!.Attribute("healthBonus")!.Value);
		Assert.AreEqual("2", applesDefinition.Element("Pollination")!.Attribute("yieldBonus")!.Value);

		var cucumberDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Cucumbers").Definition);
		Assert.AreEqual("Required", cucumberDefinition.Element("Pollination")!.Attribute("dependency")!.Value);

		var walnutsDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Walnuts").Definition);
		Assert.IsTrue(walnutsDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "walnut nut" &&
			x.Attribute("tag")!.Value == "Seeded Yield"));

		var corianderDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Coriander").Definition);
		Assert.IsTrue(corianderDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "coriander" &&
			x.Attribute("tag")!.Value == "Seeded Yield"));
		var saffronDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Saffron Crocus").Definition);
		Assert.IsTrue(saffronDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "saffron crocus" &&
			x.Attribute("tag")!.Value == "Seeded Yield"));
		var blackPepperDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == "Black Pepper").Definition);
		Assert.IsTrue(blackPepperDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "black pepper" &&
			x.Attribute("tag")!.Value == "Seeded Yield"));
		foreach (var (cropName, materialName) in new[]
		         {
			         ("Madder", "madder root"),
			         ("Weld", "weld"),
			         ("Alkanet", "alkanet root"),
			         ("Henna", "henna leaf")
		         })
		{
			var dyeCropDefinition = XElement.Parse(context.AgricultureCropDefinitions.Single(x => x.Name == cropName).Definition);
			Assert.IsTrue(dyeCropDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
				x.Attribute("material")!.Value == materialName &&
				x.Attribute("tag")!.Value == "Seeded Yield"));
		}

		foreach (var (cropName, materialName) in new[]
		         {
			         ("Ramie", "ramie cloth"),
			         ("Raffia Palms", "raffia cloth"),
			         ("Breadfruit", "barkcloth")
		         })
		{
			Assert.IsTrue(CropDefinition(context, cropName)
			              .Element("Outputs")!
			              .Elements("Commodity")
			              .Any(x => x.Attribute("material")!.Value == materialName),
				$"{cropName} should provide production support for {materialName}.");
		}

		foreach (var (cropName, materialName) in new[]
		         {
			         ("Tepary Beans", "tepary bean"),
			         ("Enset", "enset starch"),
			         ("Egusi Melons", "egusi seed"),
			         ("Hops", "hop"),
			         ("Artichokes", "artichoke"),
			         ("Vanilla", "vanilla"),
			         ("Oil Palms", "palm fruit"),
			         ("Tobacco", "tobacco leaf"),
			         ("Cardamom", "cardamom"),
			         ("Allspice", "allspice"),
			         ("Logwood", "logwood"),
			         ("Chamomile", "chamomile"),
			         ("Mandrake", "mandrake")
		         })
		{
			var expandedCropDefinition = CropDefinition(context, cropName);
			Assert.IsTrue(expandedCropDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
				x.Attribute("material")!.Value == materialName &&
				x.Attribute("tag")!.Value == "Seeded Yield"));
		}

		foreach (var (cropName, materialName) in new[]
		         {
			         ("Cacao", "cacao bean"),
			         ("Nutmeg", "mace"),
			         ("Nopal Cactus", "cochineal")
		         })
		{
			Assert.IsTrue(CropDefinition(context, cropName)
			              .Element("Outputs")!
			              .Elements("Commodity")
			              .Any(x => x.Attribute("material")!.Value == materialName),
				$"{cropName} should include the {materialName} secondary output.");
		}

		Assert.AreEqual("Required", CropDefinition(context, "Melons").Element("Pollination")!.Attribute("dependency")!.Value);
		Assert.AreEqual("Required", CropDefinition(context, "Watermelons").Element("Pollination")!.Attribute("dependency")!.Value);
		Assert.AreEqual("Required", CropDefinition(context, "Vanilla").Element("Pollination")!.Attribute("dependency")!.Value);
		Assert.AreEqual("Strong", CropDefinition(context, "Cranberries").Element("Pollination")!.Attribute("dependency")!.Value);
		Assert.AreEqual("Beneficial", CropDefinition(context, "Tomatillos").Element("Pollination")!.Attribute("dependency")!.Value);

		Assert.IsFalse(context.AgricultureHerdDefinitions.Any(x => x.Name.Contains(" or ", StringComparison.InvariantCultureIgnoreCase)));
		Assert.IsFalse(context.AgricultureHerdDefinitions.Any(x => x.Name == "Sheep or Goat Flock"));
		Assert.IsFalse(context.AgricultureHerdDefinitions.Any(x => x.Name == "Poultry Flock"));
		AssertHerdOutput(context, "Cattle Herd", "milk", "Raw Milk");
		AssertHerdOutput(context, "Cattle Herd", "feces", "Manure Commodity");
		AssertHerdOutput(context, "Sheep Flock", "wool", "Raw Textile Fibre");
		AssertHerdOutput(context, "Sheep Flock", "milk", "Raw Milk");
		AssertHerdOutput(context, "Goat Herd", "wool", "Raw Textile Fibre");
		AssertHerdOutput(context, "Goat Herd", "milk", "Raw Milk");
		AssertHerdOutput(context, "Horse Herd", "milk", "Raw Milk");
		AssertHerdOutput(context, "Camel Herd", "wool", "Raw Textile Fibre");
		AssertHerdOutput(context, "Llama Herd", "camelid wool", "Raw Textile Fibre");
		AssertHerdOutput(context, "Alpaca Herd", "camelid wool", "Raw Textile Fibre");
		AssertHerdOutput(context, "Reindeer Herd", "milk", "Raw Milk");
		AssertHerdOutput(context, "Chicken Flock", "egg", "Egg Product");
		AssertHerdOutput(context, "Duck Flock", "egg", "Egg Product");
		AssertHerdOutput(context, "Goose Flock", "egg", "Egg Product");
		AssertHerdOutput(context, "Turkey Flock", "egg", "Egg Product");
		AssertHerdOutput(context, "Quail Covey", "egg", "Egg Product");
		AssertHerdOutput(context, "Ostrich Flock", "egg", "Egg Product");

		var hazelDefinition = XElement.Parse(context.AgricultureWoodlandDefinitions.Single(x => x.Name == "Hazel Coppice").Definition);
		Assert.AreEqual("180", hazelDefinition.Attribute("establishmentDays")!.Value);
		Assert.IsTrue(hazelDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
			x.Attribute("material")!.Value == "hazel"));
		foreach (var (woodlandName, materialName) in new[]
		         {
			         ("Kermes Oak Scrub", "kermes grain"),
			         ("Dye Lichen Grove", "orchil lichen"),
			         ("Lac Host Grove", "lac dye cake")
		         })
		{
			var dyeWoodlandDefinition = XElement.Parse(context.AgricultureWoodlandDefinitions.Single(x => x.Name == woodlandName).Definition);
			Assert.IsTrue(dyeWoodlandDefinition.Element("Outputs")!.Elements("Commodity").Any(x =>
				x.Attribute("material")!.Value == materialName));
		}

		foreach (var (woodlandName, materialName) in new[]
		         {
			         ("Acacia Gum Grove", "gum arabic"),
			         ("Aleppo Pine Resin Stand", "pine resin"),
			         ("Reedbed Thatch Stand", "reed"),
			         ("Rattan Cane Brake", "rattan"),
			         ("Teak Plantation", "teak"),
			         ("Shea Parkland", "shea nut"),
			         ("Baobab Parkland", "baobab fruit"),
			         ("Brazil Nut Forest Grove", "brazil nut"),
			         ("Ash Coppice", "ash"),
			         ("Maple Sugarbush", "maple sap"),
			         ("Mangrove Coppice", "mangrove wood"),
			         ("Charcoal Coppice", "charcoal")
		         })
		{
			AssertWoodlandOutput(context, woodlandName, materialName);
		}

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

		var collectHerdProducts = context.AgricultureOperations.Single(x => x.Name == "Collect Herd Products");
		Assert.AreEqual((int)AgricultureOperationType.HarvestHerdProducts, collectHerdProducts.OperationType);
		Assert.AreEqual((int)AgricultureTargetType.Herd, collectHerdProducts.TargetType);
		var collectHerdDefinition = XElement.Parse(collectHerdProducts.Definition);
		Assert.AreEqual("1", collectHerdDefinition.Attribute("herdYieldMultiplier")!.Value);
		Assert.AreEqual("55", collectHerdDefinition.Attribute("herdYieldCost")!.Value);

		var coppice = context.AgricultureOperations.Single(x => x.Name == "Coppice Woodland");
		var coppiceDefinition = XElement.Parse(coppice.Definition);
		Assert.AreEqual("0.45", coppiceDefinition.Attribute("woodlandYieldMultiplier")!.Value);
		Assert.AreEqual("45", coppiceDefinition.Attribute("woodlandYieldCost")!.Value);

		var installApiary = context.AgricultureOperations.Single(x => x.Name == "Install Apiary");
		Assert.AreEqual((int)AgricultureOperationType.InstallApiary, installApiary.OperationType);
		var installApiaryDefinition = XElement.Parse(installApiary.Definition);
		Assert.AreEqual("Fallow,Crop,Orchard,Pasture,Woodland", installApiaryDefinition.Element("AllowedUses")!.Attribute("uses")!.Value);
		Assert.AreEqual("2", installApiaryDefinition.Element("Apiary")!.Attribute("installHives")!.Value);
		Assert.AreEqual("2", installApiaryDefinition.Element("Apiary")!.Attribute("pollinationRadius")!.Value);

		var harvestApiary = context.AgricultureOperations.Single(x => x.Name == "Harvest Apiary");
		Assert.AreEqual((int)AgricultureOperationType.HarvestApiary, harvestApiary.OperationType);
		var apiaryOutputs = XElement.Parse(harvestApiary.Definition).Element("Apiary")!.Element("Outputs")!.Elements("Commodity").ToArray();
		Assert.IsTrue(apiaryOutputs.Any(x => x.Attribute("material")!.Value == "honeycomb" && x.Attribute("tag")!.Value == "Raw Honeycomb"));
		Assert.IsTrue(apiaryOutputs.Any(x => x.Attribute("material")!.Value == "honey" && x.Attribute("tag")!.Value == "Pressed Honey"));
		Assert.IsTrue(apiaryOutputs.Any(x => x.Attribute("material")!.Value == "beeswax" && x.Attribute("tag")!.Value == "Rendered Beeswax"));

		AssertOperationDelta(context, "Harrow Field", AgricultureScoreType.Tilth, 4);
		AssertOperationDelta(context, "Hoe Rows", AgricultureScoreType.Weeds, -5);
		AssertOperationDelta(context, "Ridge and Furrow", AgricultureScoreType.Drainage, 4);
		AssertOperationDelta(context, "Spread Manure", AgricultureScoreType.Nutrients, 7);
		AssertOperationDelta(context, "Flush Salts", AgricultureScoreType.Salinity, -8);
		AssertOperationDelta(context, "Install Tile Drainage", AgricultureScoreType.Drainage, 10);
		AssertOperationDelta(context, "Net Orchard", AgricultureScoreType.Pests, -6);
		AssertOperationDelta(context, "Improve Pasture", AgricultureScoreType.Pasture, 6);
		AssertOperationDelta(context, "Reseed Pasture", AgricultureScoreType.Pasture, 8);
		AssertOperationDelta(context, "Pollard Trees", AgricultureScoreType.Pests, -1);
		AssertOperationDelta(context, "Tap Resin", AgricultureScoreType.Condition, -1);
		AssertOperationDelta(context, "Build Raised Beds", AgricultureScoreType.Drainage, 5);
		AssertOperationDelta(context, "Kraal Night Penning", AgricultureScoreType.Nutrients, 6);

		foreach (var (operationName, profileName) in new[]
		         {
			         ("Harrow Field", "Average Worked Field"),
			         ("Hoe Rows", "Average Worked Field"),
			         ("Ridge and Furrow", "Average Worked Field"),
			         ("Spread Manure", "Average Worked Field"),
			         ("Flush Salts", "Salt-Affected Irrigated Field"),
			         ("Install Tile Drainage", "Reclaimed Peat Field"),
			         ("Net Orchard", "Neglected Orchard"),
			         ("Improve Pasture", "Pasture"),
			         ("Reseed Pasture", "Steppe Pasture"),
			         ("Pollard Trees", "Managed Woodland"),
			         ("Tap Resin", "Managed Woodland"),
			         ("Build Raised Beds", "Waterlogged Former Paddy"),
			         ("Kraal Night Penning", "Kraal-Manured Garden")
		         })
		{
			foreach (var delta in OperationDefinition(context, operationName).Elements("Score"))
			{
				var score = Enum.Parse<AgricultureScoreType>(delta.Attribute("type")!.Value);
				var after = ProfileScore(context, profileName, score) + int.Parse(delta.Attribute("value")!.Value);
				Assert.IsTrue(after is >= 0 and <= 100,
					$"{operationName} pushes {profileName} {score} outside 0-100.");
			}
		}

		var projectIds = context.Projects
			.Where(x => x.Name.StartsWith("Stock Agriculture:"))
			.Select(x => x.Id)
			.ToHashSet();
		Assert.AreEqual(context.AgricultureOperations.Count(), projectIds.Count);
		Assert.IsTrue(projectIds.Contains(sow.ProjectId));

		var phaseIds = context.ProjectPhases
			.Where(x => projectIds.Contains(x.ProjectId))
			.Select(x => x.Id)
			.ToHashSet();
		Assert.AreEqual(projectIds.Count, phaseIds.Count);
		Assert.AreEqual(projectIds.Count, context.ProjectActions.Count(x => phaseIds.Contains(x.ProjectPhaseId) && x.Type == "agriculture"));
		Assert.AreEqual(projectIds.Count, context.ProjectLabourRequirements.Count(x => phaseIds.Contains(x.ProjectPhaseId) && x.Type == "simple"));
		Assert.AreEqual(projectIds.Count, context.ProjectLabourRequirements.Count(x => phaseIds.Contains(x.ProjectPhaseId) && x.Type == "supervision"));

		foreach (var operation in context.AgricultureOperations)
		{
			var project = context.Projects.Single(x => x.Id == operation.ProjectId && x.RevisionNumber == operation.ProjectRevisionNumber);
			var phase = context.ProjectPhases.Single(x =>
				x.ProjectId == project.Id &&
				x.ProjectRevisionNumber == project.RevisionNumber &&
				x.PhaseNumber == 1);
			Assert.IsTrue(context.ProjectActions.Any(x => x.ProjectPhaseId == phase.Id && x.Type == "agriculture"));
			Assert.IsTrue(context.ProjectLabourRequirements.Any(x => x.ProjectPhaseId == phase.Id && x.Name == "Field Labour"));
			Assert.IsTrue(context.ProjectLabourRequirements.Any(x => x.ProjectPhaseId == phase.Id && x.Name == "Agricultural Supervision"));
		}

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

		var apiaryPhase = context.ProjectPhases.Single(x =>
			x.ProjectId == installApiary.ProjectId &&
			x.ProjectRevisionNumber == installApiary.ProjectRevisionNumber);
		var beeHives = context.ProjectMaterialRequirements.Single(x => x.ProjectPhaseId == apiaryPhase.Id && x.Name == "Bee Hives");
		Assert.AreEqual("simple", beeHives.Type);
		Assert.AreEqual("4", XElement.Parse(beeHives.Definition).Element("Tag")!.Value);
		Assert.AreEqual("2", XElement.Parse(beeHives.Definition).Element("Amount")!.Value);
		var hiveStand = context.ProjectMaterialRequirements.Single(x => x.ProjectPhaseId == apiaryPhase.Id && x.Name == "Hive Stand");
		Assert.AreEqual("5", XElement.Parse(hiveStand.Definition).Element("Tag")!.Value);
		Assert.AreEqual("1", XElement.Parse(hiveStand.Definition).Element("Amount")!.Value);

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
