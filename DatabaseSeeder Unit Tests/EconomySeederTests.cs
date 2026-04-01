#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using Calendar = MudSharp.Models.Calendar;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EconomySeederTests
{
	private const string ClassicalEra = "Classical Age";
	private const string DefaultCurrency = "Bits";
	private const string StandardScale = "Standard";
	private const string HelperProgPrefix = "EconomySeeder";
	private const string ExternalTemplatePrefix = "EconomySeeder External ";

	private static readonly IReadOnlyDictionary<string, string[]> FamilyTags =
		new Dictionary<string, string[]>
		{
			["Nourishment"] = ["Staple Food", "Standard Food", "Luxury Food", "Seasonings", "Salt", "Spices"],
			["Domestic Heating"] = ["Combustion Heating"],
			["Lighting"] = [],
			["Medicine"] = ["Simple Medicine", "Standard Medicine", "High-Quality Medicine"],
			["Writing Materials"] = ["Wax Tablets", "Parchment", "Paper", "Ink"],
			["Clothing"] = ["Simple Clothing", "Standard Clothing", "Luxury Clothing"],
			["Intoxicants"] = ["Beer", "Wine"],
			["Luxury Drinks"] = ["Tea"],
			["Household Goods"] =
			[
				"Simple Wares",
				"Standard Wares",
				"Simple Furniture",
				"Standard Furniture",
				"Luxury Furniture",
				"Standard Decorations",
				"Luxury Decorations"
			],
			["Hospitality"] = ["Standard Lodging", "Luxury Lodging"],
			["Entertainment"] = ["Cheap Entertainment", "Standard Entertainment", "Luxury Entertainment"],
			["Personal Services"] = ["Bathing Services", "Domestic Services", "Barbering", "Laundry Services"],
			["Communications"] = ["Messenger Services", "Courier Services", "Postal Services", "Printed News"],
			["Religious Goods"] = [],
			["Household Consumables"] = [],
			["Military Goods"] = ["Weapons", "Armour", "Ammunition", "Military Uniforms"],
			["Transportation"] = ["Mule Haulage"],
			["Warehousing"] = [],
			["Professional Tools"] = ["Primitive Tools", "Simple Tools", "Standard Tools", "High-Quality Tools"],
			["Raw Materials"] = []
		};

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

	private static void SeedEconomyPrerequisites(FuturemudDatabaseContext context, params string[] currencyNames)
	{
		SeedAccount(context);

		var seededCurrencies = currencyNames.Length > 0 ? currencyNames : [DefaultCurrency];
		var nextCurrencyId = 1L;
		foreach (var currencyName in seededCurrencies.Distinct(StringComparer.OrdinalIgnoreCase))
		{
			context.Currencies.Add(new Currency
			{
				Id = nextCurrencyId++,
				Name = currencyName,
				BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
			});
		}

		context.Shards.Add(new Shard
		{
			Id = 1,
			Name = "Test Shard",
			MinimumTerrestrialLux = 0.0,
			SkyDescriptionTemplateId = 1,
			SphericalRadiusMetres = 6371000.0
		});

		context.Zones.Add(new Zone
		{
			Id = 1,
			Name = "Test Zone",
			ShardId = 1,
			Latitude = 0.0,
			Longitude = 0.0,
			Elevation = 0.0,
			AmbientLightPollution = 0.0
		});

		context.Clocks.Add(new Clock
		{
			Id = 1,
			Definition = "<Clock />",
			Seconds = 0,
			Minutes = 0,
			Hours = 8,
			PrimaryTimezoneId = 1
		});

		context.Calendars.Add(new Calendar
		{
			Id = 1,
			Definition = "<Calendar />",
			Date = "1-1-1",
			FeedClockId = 1
		});

		context.Timezones.Add(new Timezone
		{
			Id = 1,
			Name = "UTC",
			Description = "Test timezone",
			OffsetHours = 0,
			OffsetMinutes = 0,
			ClockId = 1
		});

		context.ShardsClocks.Add(new ShardsClocks
		{
			ShardId = 1,
			ClockId = 1
		});

		context.ShardsCalendars.Add(new ShardsCalendars
		{
			ShardId = 1,
			CalendarId = 1
		});

		context.ZonesTimezones.Add(new ZonesTimezones
		{
			ZoneId = 1,
			ClockId = 1,
			TimezoneId = 1
		});

		SeedMarketTags(context);
		context.SaveChanges();
	}

	private static void SeedMarketTags(FuturemudDatabaseContext context)
	{
		long nextId = 100;
		var root = new Tag
		{
			Id = nextId++,
			Name = "Market"
		};

		context.Tags.Add(root);
		foreach (var (familyName, leafNames) in FamilyTags)
		{
			var family = new Tag
			{
				Id = nextId++,
				Name = familyName,
				Parent = root,
				ParentId = root.Id
			};
			context.Tags.Add(family);

			foreach (var leafName in leafNames)
			{
				context.Tags.Add(new Tag
				{
					Id = nextId++,
					Name = leafName,
					Parent = family,
					ParentId = family.Id
				});
			}
		}
	}

	private static Dictionary<string, string> BuildAnswers(
		string shopperScale = StandardScale,
		string era = ClassicalEra,
		string? currency = null)
	{
		var answers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["era"] = era,
			["shopper-scale"] = shopperScale
		};

		if (!string.IsNullOrWhiteSpace(currency))
		{
			answers["currency"] = currency;
		}

		return answers;
	}

	private static decimal GetBudgetForShopper(FuturemudDatabaseContext context, string shopperName)
	{
		var shopper = context.Shoppers.Single(x => x.Name == shopperName);
		return decimal.Parse(XElement.Parse(shopper.Definition).Element("BudgetPerShop")!.Value, CultureInfo.InvariantCulture);
	}

	private static List<string> GetPopulationNeedCategoryNames(FuturemudDatabaseContext context, string populationName)
	{
		var population = context.MarketPopulations.Single(x => x.Name == populationName);
		var categoryIds = XElement.Parse(population.MarketPopulationNeeds)
			.Elements("Need")
			.Select(x => long.Parse(x.Attribute("category")!.Value, CultureInfo.InvariantCulture))
			.ToList();

		return context.MarketCategories
			.Where(x => categoryIds.Contains(x.Id))
			.Select(x => x.Name)
			.ToList();
	}

	private static decimal GetPopulationNeedExpenditure(
		FuturemudDatabaseContext context,
		string populationName,
		string categoryName)
	{
		var population = context.MarketPopulations.Single(x => x.Name == populationName);
		var needs = XElement.Parse(population.MarketPopulationNeeds)
			.Elements("Need")
			.Select(x => new
			{
				CategoryId = long.Parse(x.Attribute("category")!.Value, CultureInfo.InvariantCulture),
				Expenditure = decimal.Parse(x.Attribute("expenditure")!.Value, CultureInfo.InvariantCulture)
			})
			.ToList();
		var category = context.MarketCategories.Single(x => x.Name == categoryName);
		return needs.Single(x => x.CategoryId == category.Id).Expenditure;
	}

	private static Dictionary<long, (int Upward, int Downward)> GetExternalCoverageByCategory(FuturemudDatabaseContext context)
	{
		var coverage = context.MarketCategories.ToDictionary(x => x.Id, _ => (Upward: 0, Downward: 0));
		foreach (var template in context.MarketInfluenceTemplates
			         .AsEnumerable()
			         .Where(x => x.Name.StartsWith(ExternalTemplatePrefix, StringComparison.OrdinalIgnoreCase)))
		{
			var impacts = XElement.Parse(template.Impacts).Elements("Impact");
			foreach (var impact in impacts)
			{
				var categoryId = long.Parse(impact.Attribute("category")!.Value, CultureInfo.InvariantCulture);
				var demand = double.Parse(impact.Attribute("demand")!.Value, CultureInfo.InvariantCulture);
				var supply = double.Parse(impact.Attribute("supply")!.Value, CultureInfo.InvariantCulture);
				var pressure = demand - supply;
				if (pressure > 0)
				{
					coverage[categoryId] = (coverage[categoryId].Upward + 1, coverage[categoryId].Downward);
					continue;
				}

				if (pressure < 0)
				{
					coverage[categoryId] = (coverage[categoryId].Upward, coverage[categoryId].Downward + 1);
				}
			}
		}

		return coverage;
	}

	[TestMethod]
	public void ShouldSeedData_MissingPrerequisites_ReturnsBlocked()
	{
		using var emptyContext = BuildContext();
		var seeder = new EconomySeeder();

		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(emptyContext));

		using var missingTagsContext = BuildContext();
		SeedAccount(missingTagsContext);
		missingTagsContext.Currencies.Add(new Currency { Id = 1, Name = "Test Crown", BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m });
		missingTagsContext.Zones.Add(new Zone { Id = 1, Name = "Test Zone", ShardId = 1 });
		missingTagsContext.Clocks.Add(new Clock { Id = 1, Definition = "<Clock />", Hours = 0, Minutes = 0, Seconds = 0, PrimaryTimezoneId = 1 });
		missingTagsContext.Calendars.Add(new Calendar { Id = 1, Definition = "<Calendar />", Date = "1-1-1", FeedClockId = 1 });
		missingTagsContext.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(missingTagsContext));
	}

	[TestMethod]
	public void SeedData_FirstRun_CreatesExpectedEconomyPackage()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();
		var expectedCategoryCount = FamilyTags.Count + FamilyTags.Sum(x => x.Value.Length);

		var result = seeder.SeedData(context, BuildAnswers());

		Assert.AreEqual("The operation completed successfully.", result);
		Assert.AreEqual(1, context.EconomicZones.Count(x => x.Name == "Classical Age Economy Template Zone"));
		Assert.AreEqual(1, context.Markets.Count(x => x.Name == "Classical Age Economy Template Market"));
		Assert.AreEqual(expectedCategoryCount, context.MarketCategories.Count());
		Assert.AreEqual(60, context.MarketInfluenceTemplates.AsEnumerable().Count(x =>
			x.Name.StartsWith(ExternalTemplatePrefix, StringComparison.OrdinalIgnoreCase)));
		Assert.AreEqual(21, context.MarketInfluenceTemplates.AsEnumerable().Count(x =>
			x.Name.StartsWith($"{HelperProgPrefix} Stress ", StringComparison.OrdinalIgnoreCase)));
		Assert.AreEqual(7, context.MarketPopulations.Count());
		Assert.AreEqual(7, context.Shoppers.Count());
		Assert.AreEqual(59, context.FutureProgs.Count(x => x.FunctionName.StartsWith(HelperProgPrefix, StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void SeedData_Rerun_DoesNotDuplicateSeededAssets()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();
		var answers = BuildAnswers();

		seeder.SeedData(context, answers);
		var initialZoneCount = context.EconomicZones.Count();
		var initialMarketCount = context.Markets.Count();
		var initialCategoryCount = context.MarketCategories.Count();
		var initialTemplateCount = context.MarketInfluenceTemplates.Count();
		var initialPopulationCount = context.MarketPopulations.Count();
		var initialShopperCount = context.Shoppers.Count();
		var initialProgCount = context.FutureProgs.Count();

		seeder.SeedData(context, answers);

		Assert.AreEqual(initialZoneCount, context.EconomicZones.Count());
		Assert.AreEqual(initialMarketCount, context.Markets.Count());
		Assert.AreEqual(initialCategoryCount, context.MarketCategories.Count());
		Assert.AreEqual(initialTemplateCount, context.MarketInfluenceTemplates.Count());
		Assert.AreEqual(initialPopulationCount, context.MarketPopulations.Count());
		Assert.AreEqual(initialShopperCount, context.Shoppers.Count());
		Assert.AreEqual(initialProgCount, context.FutureProgs.Count());
	}

	[TestMethod]
	public void SeedData_Rerun_RestoresDeletedSeededAsset()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();
		var answers = BuildAnswers();

		seeder.SeedData(context, answers);
		var deletedTemplate = context.MarketInfluenceTemplates.AsEnumerable().Single(x => x.Name == "EconomySeeder External Classical Age Harvest Failure");
		context.MarketInfluenceTemplates.Remove(deletedTemplate);
		context.SaveChanges();

		Assert.AreEqual(0, context.MarketInfluenceTemplates.AsEnumerable().Count(x => x.Name == deletedTemplate.Name));

		seeder.SeedData(context, answers);

		Assert.AreEqual(1, context.MarketInfluenceTemplates.AsEnumerable().Count(x => x.Name == deletedTemplate.Name));
	}

	[TestMethod]
	public void SeedData_EveryCategory_HasAtLeastThreeUpwardAndDownwardExternalTemplates()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();

		seeder.SeedData(context, BuildAnswers());
		var coverage = GetExternalCoverageByCategory(context);

		foreach (var category in context.MarketCategories.OrderBy(x => x.Name))
		{
			Assert.IsTrue(coverage[category.Id].Upward >= 3, $"{category.Name} should have at least three upward-pressure external templates.");
			Assert.IsTrue(coverage[category.Id].Downward >= 3, $"{category.Name} should have at least three downward-pressure external templates.");
		}
	}

	[TestMethod]
	public void SeedData_EachPopulation_HasThreeStressThresholdsAndValidProgs()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();

		seeder.SeedData(context, BuildAnswers());

		foreach (var population in context.MarketPopulations.OrderBy(x => x.Name))
		{
			var stresses = XElement.Parse(population.MarketStressPoints).Elements("Stress").ToList();
			Assert.AreEqual(3, stresses.Count, $"{population.Name} should have three seeded stress thresholds.");

			foreach (var stress in stresses)
			{
				var startProgId = long.Parse(stress.Attribute("onstart")!.Value, CultureInfo.InvariantCulture);
				var endProgId = long.Parse(stress.Attribute("onend")!.Value, CultureInfo.InvariantCulture);
				Assert.IsNotNull(context.FutureProgs.SingleOrDefault(x => x.Id == startProgId), $"{population.Name} should reference a valid start prog.");
				Assert.IsNotNull(context.FutureProgs.SingleOrDefault(x => x.Id == endProgId), $"{population.Name} should reference a valid end prog.");
			}
		}
	}

	[TestMethod]
	public void SeedData_EveryStressTemplate_IncludesSupplyContraction()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);

		new EconomySeeder().SeedData(context, BuildAnswers());

		foreach (var template in context.MarketInfluenceTemplates
			         .AsEnumerable()
			         .Where(x => x.Name.StartsWith($"{HelperProgPrefix} Stress ", StringComparison.OrdinalIgnoreCase)))
		{
			var impacts = XElement.Parse(template.Impacts).Elements("Impact").ToList();
			Assert.IsTrue(impacts.Count > 0, $"{template.Name} should contain impacted categories.");
			Assert.IsTrue(
				impacts.Any(x => double.Parse(x.Attribute("supply")!.Value, CultureInfo.InvariantCulture) < 0.0),
				$"{template.Name} should include at least one negative supply impact.");
		}
	}

	[TestMethod]
	public void SeedData_AllPopulations_IncludeMedicineNeedAndChurchClassesExist()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();

		seeder.SeedData(context, BuildAnswers());

		CollectionAssert.IsSubsetOf(
			new[]
			{
				"Classical Age Temple Priesthood",
				"Classical Age Monastic Orders"
			},
			context.MarketPopulations.Select(x => x.Name).ToList());

		foreach (var population in context.MarketPopulations.OrderBy(x => x.Name))
		{
			var categoryNames = GetPopulationNeedCategoryNames(context, population.Name);

			Assert.IsTrue(
				categoryNames.Any(x => x.Contains("Medicine", StringComparison.OrdinalIgnoreCase)),
				$"{population.Name} should include at least one medicine category in its needs.");
		}
	}

	[TestMethod]
	public void SeedData_LiterateHouseholds_IncludeWritingMaterialsNeeds()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();
		foreach (var era in new[] { "Classical Age", "Feudal Age", "Medieval Age", "Early Modern Age" })
		{
			seeder.SeedData(context, BuildAnswers(era: era));
		}

		var literatePopulations = new[]
		{
			"Classical Age Temple Priesthood",
			"Classical Age Patrician Elite",
			"Feudal Age Parish Priesthood",
			"Feudal Age Noble Elite",
			"Medieval Age Guild-Merchant Households",
			"Medieval Age Parish Clergy",
			"Medieval Age Noble Elite",
			"Early Modern Age Middling Households",
			"Early Modern Age Merchant And Professional Class",
			"Early Modern Age Parish Clergy",
			"Early Modern Age Gentry Elite"
		};

		foreach (var populationName in literatePopulations)
		{
			var categoryNames = GetPopulationNeedCategoryNames(context, populationName);
			Assert.IsTrue(
				categoryNames.Any(x => x is "Wax Tablets" or "Parchment" or "Paper" or "Ink"),
				$"{populationName} should include an era-appropriate writing-material need.");
		}
	}

	[TestMethod]
	public void SeedData_LaterEras_UseBroaderServiceAndHospitalityNeeds()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();
		foreach (var era in new[] { "Feudal Age", "Medieval Age", "Early Modern Age" })
		{
			seeder.SeedData(context, BuildAnswers(era: era));
		}

		CollectionAssert.IsSubsetOf(
			new[] { "Standard Lodging", "Cheap Entertainment", "Messenger Services" },
			GetPopulationNeedCategoryNames(context, "Feudal Age Itinerant Tradesfolk"));
		CollectionAssert.IsSubsetOf(
			new[] { "Standard Lodging", "Standard Entertainment", "Messenger Services" },
			GetPopulationNeedCategoryNames(context, "Medieval Age Guild-Merchant Households"));
		CollectionAssert.IsSubsetOf(
			new[] { "Postal Services", "Standard Lodging", "Standard Entertainment", "Laundry Services" },
			GetPopulationNeedCategoryNames(context, "Early Modern Age Merchant And Professional Class"));
		CollectionAssert.IsSubsetOf(
			new[] { "Luxury Lodging", "Luxury Entertainment", "Domestic Services" },
			GetPopulationNeedCategoryNames(context, "Early Modern Age Gentry Elite"));
	}

	[TestMethod]
	public void SeedData_EliteHouseholds_SpendAtLeastFiveTimesPoorHouseholds()
	{
		using var context = BuildContext();
		SeedEconomyPrerequisites(context);
		new EconomySeeder().SeedData(context, BuildAnswers());

		var poorBudget = GetBudgetForShopper(context, "Classical Age Urban Poor Shopper");
		var eliteBudget = GetBudgetForShopper(context, "Classical Age Patrician Elite Shopper");

		Assert.IsTrue(eliteBudget >= poorBudget * 5.0m,
			$"Elite shopper budget {eliteBudget} should be at least five times the poor shopper budget {poorBudget}.");
	}

	[TestMethod]
	public void SeedData_ShopperScale_MapsBudgetsToExpectedMultipliers()
	{
		const string shopperName = "Classical Age Urban Poor Shopper";

		using var lowContext = BuildContext();
		SeedEconomyPrerequisites(lowContext);
		new EconomySeeder().SeedData(lowContext, BuildAnswers("Low"));
		var lowBudget = GetBudgetForShopper(lowContext, shopperName);

		using var standardContext = BuildContext();
		SeedEconomyPrerequisites(standardContext);
		new EconomySeeder().SeedData(standardContext, BuildAnswers("Standard"));
		var standardBudget = GetBudgetForShopper(standardContext, shopperName);

		using var highContext = BuildContext();
		SeedEconomyPrerequisites(highContext);
		new EconomySeeder().SeedData(highContext, BuildAnswers("High"));
		var highBudget = GetBudgetForShopper(highContext, shopperName);

		Assert.IsTrue(standardBudget > 0.0m);
		Assert.AreEqual(decimal.Round(standardBudget * 0.75m, 2, MidpointRounding.AwayFromZero), lowBudget);
		Assert.AreEqual(decimal.Round(standardBudget * 1.50m, 2, MidpointRounding.AwayFromZero), highBudget);
	}

	[TestMethod]
	public void SeedData_CurrencyScaling_KeepsComparableSterlingValueAcrossCurrencies()
	{
		using var bitContext = BuildContext();
		SeedEconomyPrerequisites(bitContext, "Bits");
		new EconomySeeder().SeedData(bitContext, BuildAnswers(currency: "Bits"));
		var bitNeed = GetPopulationNeedExpenditure(bitContext, "Classical Age Urban Poor", "Staple Food");

		using var poundContext = BuildContext();
		SeedEconomyPrerequisites(poundContext, "Pounds");
		new EconomySeeder().SeedData(poundContext, BuildAnswers(currency: "Pounds"));
		var poundNeed = GetPopulationNeedExpenditure(poundContext, "Classical Age Urban Poor", "Staple Food");

		using var dollarContext = BuildContext();
		SeedEconomyPrerequisites(dollarContext, "Dollars");
		new EconomySeeder().SeedData(dollarContext, BuildAnswers(currency: "Dollars"));
		var dollarNeed = GetPopulationNeedExpenditure(dollarContext, "Classical Age Urban Poor", "Staple Food");

		var bitSterling = bitNeed / 100.0m;
		var poundSterling = poundNeed / 960.0m;
		var dollarSterling = dollarNeed / 125.0m;

		Assert.IsTrue(Math.Abs(bitSterling - poundSterling) < 0.05m, "Bits and pounds should seed to near-equivalent sterling value.");
		Assert.IsTrue(Math.Abs(bitSterling - dollarSterling) < 0.05m, "Bits and dollars should seed to near-equivalent sterling value.");
	}

	[TestMethod]
	public void SeedData_EraScaling_RaisesLaterEraCommonerBudgets()
	{
		using var feudalContext = BuildContext();
		SeedEconomyPrerequisites(feudalContext, "Bits");
		new EconomySeeder().SeedData(feudalContext, BuildAnswers(era: "Feudal Age", currency: "Bits"));
		var feudalBudget = GetBudgetForShopper(feudalContext, "Feudal Age Peasantry Shopper");

		using var earlyModernContext = BuildContext();
		SeedEconomyPrerequisites(earlyModernContext, "Bits");
		new EconomySeeder().SeedData(earlyModernContext, BuildAnswers(era: "Early Modern Age", currency: "Bits"));
		var earlyModernBudget = GetBudgetForShopper(earlyModernContext, "Early Modern Age Labourers Shopper");

		Assert.IsTrue(earlyModernBudget > feudalBudget,
			$"Early modern commoner budget {earlyModernBudget} should exceed feudal commoner budget {feudalBudget} when both use the same currency.");
	}
}
