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
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
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
    private static readonly IReadOnlySet<string> StockCombinationFamilies =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Medicine",
            "Writing Materials",
            "Clothing",
            "Intoxicants",
            "Household Goods",
            "Hospitality",
            "Entertainment",
            "Personal Services",
            "Communications",
            "Military Goods",
            "Professional Tools"
        };

    private static readonly IReadOnlyDictionary<string, string[]> FamilyTags =
        new Dictionary<string, string[]>
        {
            ["Nourishment"] = ["Staple Food", "Standard Food", "Luxury Food", "Salt", "Spices"],
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
    }

    private static void SeedEconomyPrerequisites(FuturemudDatabaseContext context, params string[] currencyNames)
    {
        SeedAccount(context);

        string[] seededCurrencies = currencyNames.Length > 0 ? currencyNames : [DefaultCurrency];
        long nextCurrencyId = 1L;
        foreach (string? currencyName in seededCurrencies.Distinct(StringComparer.OrdinalIgnoreCase))
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
        Tag root = new()
        {
            Id = nextId++,
            Name = "Market"
        };

        context.Tags.Add(root);
        foreach ((string? familyName, string[]? leafNames) in FamilyTags)
        {
            Tag family = new()
            {
                Id = nextId++,
                Name = familyName,
                Parent = root,
                ParentId = root.Id
            };
            context.Tags.Add(family);

            foreach (string leafName in leafNames)
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
        Dictionary<string, string> answers = new(StringComparer.OrdinalIgnoreCase)
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
        Shopper shopper = context.Shoppers.Single(x => x.Name == shopperName);
        return decimal.Parse(XElement.Parse(shopper.Definition).Element("BudgetPerShop")!.Value, CultureInfo.InvariantCulture);
    }

    private static List<string> GetPopulationNeedCategoryNames(FuturemudDatabaseContext context, string populationName)
    {
        MarketPopulation population = context.MarketPopulations.Single(x => x.Name == populationName);
        List<long> categoryIds = XElement.Parse(population.MarketPopulationNeeds)
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
        MarketPopulation population = context.MarketPopulations.Single(x => x.Name == populationName);
        var needs = XElement.Parse(population.MarketPopulationNeeds)
            .Elements("Need")
            .Select(x => new
            {
                CategoryId = long.Parse(x.Attribute("category")!.Value, CultureInfo.InvariantCulture),
                Expenditure = decimal.Parse(x.Attribute("expenditure")!.Value, CultureInfo.InvariantCulture)
            })
            .ToList();
        MarketCategory category = context.MarketCategories.Single(x => x.Name == categoryName);
        return needs.Single(x => x.CategoryId == category.Id).Expenditure;
    }

    private static Dictionary<long, (int Upward, int Downward)> GetExternalCoverageByCategory(FuturemudDatabaseContext context)
    {
        Dictionary<long, MarketCategory> categoriesById = context.MarketCategories
            .AsEnumerable()
            .ToDictionary(x => x.Id);
        Dictionary<long, (int Upward, int Downward)> coverage = context.MarketCategories.ToDictionary(x => x.Id, _ => (Upward: 0, Downward: 0));
        foreach (MarketInfluenceTemplate? template in context.MarketInfluenceTemplates
                     .AsEnumerable()
                     .Where(x => x.Name.StartsWith(ExternalTemplatePrefix, StringComparison.OrdinalIgnoreCase)))
        {
            IEnumerable<XElement> impacts = XElement.Parse(template.Impacts).Elements("Impact");
            foreach (XElement impact in impacts)
            {
                long categoryId = long.Parse(impact.Attribute("category")!.Value, CultureInfo.InvariantCulture);
                double demand = double.Parse(impact.Attribute("demand")!.Value, CultureInfo.InvariantCulture);
                double supply = double.Parse(impact.Attribute("supply")!.Value, CultureInfo.InvariantCulture);
                double pressure = demand - supply;
                foreach (long effectiveCategoryId in ExpandEffectiveCategoryIds(categoryId, categoriesById))
                {
                    if (pressure > 0)
                    {
                        coverage[effectiveCategoryId] = (coverage[effectiveCategoryId].Upward + 1, coverage[effectiveCategoryId].Downward);
                        continue;
                    }

                    if (pressure < 0)
                    {
                        coverage[effectiveCategoryId] = (coverage[effectiveCategoryId].Upward, coverage[effectiveCategoryId].Downward + 1);
                    }
                }
            }
        }

        return coverage;
    }

    private static HashSet<long> ExpandEffectiveCategoryIds(
        long categoryId,
        IReadOnlyDictionary<long, MarketCategory> categoriesById)
    {
        HashSet<long> impactedCategoryIds = [categoryId];
        ExpandEffectiveCategoryIds(categoryId, categoriesById, impactedCategoryIds, new HashSet<long>());
        return impactedCategoryIds;
    }

    private static void ExpandEffectiveCategoryIds(
        long categoryId,
        IReadOnlyDictionary<long, MarketCategory> categoriesById,
        ISet<long> impactedCategoryIds,
        ISet<long> visitedCategoryIds)
    {
        if (!visitedCategoryIds.Add(categoryId)
            || !categoriesById.TryGetValue(categoryId, out MarketCategory? category)
            || category.MarketCategoryType != 1)
        {
            return;
        }

        foreach (long componentId in GetCombinationComponentIds(category))
        {
            impactedCategoryIds.Add(componentId);
            ExpandEffectiveCategoryIds(componentId, categoriesById, impactedCategoryIds, visitedCategoryIds);
        }
    }

    private static List<long> GetCombinationComponentIds(MarketCategory category)
    {
        return XElement.Parse(category.CombinationCategories ?? "<Components />")
            .Elements("Component")
            .Select(x => long.Parse(x.Attribute("category")!.Value, CultureInfo.InvariantCulture))
            .ToList();
    }

    private static List<XElement> GetPopulationIncomeImpacts(MarketInfluenceTemplate template)
    {
        return XElement.Parse(template.PopulationImpacts ?? "<PopulationImpacts />")
            .Elements("PopulationImpact")
            .ToList();
    }

    [TestMethod]
    public void ShouldSeedData_MissingPrerequisites_ReturnsBlocked()
    {
        using FuturemudDatabaseContext emptyContext = BuildContext();
        EconomySeeder seeder = new();

        Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(emptyContext));

        using FuturemudDatabaseContext missingTagsContext = BuildContext();
        SeedAccount(missingTagsContext);
        missingTagsContext.Currencies.Add(new Currency { Id = 1, Name = "Test Crown", BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m });
        missingTagsContext.Zones.Add(new Zone { Id = 1, Name = "Test Zone", ShardId = 1 });
        missingTagsContext.Clocks.Add(new Clock { Id = 1, Definition = "<Clock />", Hours = 0, Minutes = 0, Seconds = 0, PrimaryTimezoneId = 1 });
        missingTagsContext.Calendars.Add(new Calendar { Id = 1, Definition = "<Calendar />", Date = "1-1-1", FeedClockId = 1 });
        missingTagsContext.SaveChanges();

        Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, seeder.ShouldSeedData(missingTagsContext));
    }

    [TestMethod]
    public void ShouldSeedData_IncompleteMarketTagVocabulary_ReturnsBlocked()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedAccount(context);
        context.Currencies.Add(new Currency { Id = 1, Name = "Test Crown", BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m });
        context.Shards.Add(new Shard { Id = 1, Name = "Test Shard", MinimumTerrestrialLux = 0.0, SkyDescriptionTemplateId = 1, SphericalRadiusMetres = 6371000.0 });
        context.Zones.Add(new Zone { Id = 1, Name = "Test Zone", ShardId = 1, Latitude = 0.0, Longitude = 0.0, Elevation = 0.0, AmbientLightPollution = 0.0 });
        context.Clocks.Add(new Clock { Id = 1, Definition = "<Clock />", Seconds = 0, Minutes = 0, Hours = 8, PrimaryTimezoneId = 1 });
        context.Calendars.Add(new Calendar { Id = 1, Definition = "<Calendar />", Date = "1-1-1", FeedClockId = 1 });
        context.Timezones.Add(new Timezone { Id = 1, Name = "UTC", Description = "Test timezone", OffsetHours = 0, OffsetMinutes = 0, ClockId = 1 });
        Tag root = new() { Id = 100, Name = "Market" };
        Tag nourishment = new() { Id = 101, Name = "Nourishment", Parent = root, ParentId = root.Id };
        context.Tags.AddRange(
            root,
            nourishment,
            new Tag { Id = 102, Name = "Staple Food", Parent = nourishment, ParentId = nourishment.Id });
        context.SaveChanges();

        Assert.AreEqual(ShouldSeedResult.PrerequisitesNotMet, new EconomySeeder().ShouldSeedData(context));
    }

    [TestMethod]
    public void SeedData_FirstRun_CreatesExpectedEconomyPackage()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        EconomySeeder seeder = new();
        int expectedCategoryCount = FamilyTags.Count + FamilyTags.Sum(x => x.Value.Length);

        string result = seeder.SeedData(context, BuildAnswers());

        Assert.AreEqual("The operation completed successfully.", result);
        Assert.AreEqual(1, context.EconomicZones.Count(x => x.Name == "Classical Age Economy Template Zone"));
        Assert.AreEqual(1, context.Markets.Count(x => x.Name == "Classical Age Economy Template Market"));
        Assert.AreEqual(expectedCategoryCount, context.MarketCategories.Count());
        Assert.AreEqual(60, context.MarketInfluenceTemplates.AsEnumerable().Count(x =>
            x.Name.StartsWith(ExternalTemplatePrefix, StringComparison.OrdinalIgnoreCase)));
        Assert.AreEqual(expectedCategoryCount * 4, context.MarketInfluenceTemplates.AsEnumerable().Count(x =>
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.EndsWith("Minor Tariff", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.EndsWith("Major Tariff", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.EndsWith("Minor Subsidy", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.EndsWith("Major Subsidy", StringComparison.OrdinalIgnoreCase)));
        Assert.AreEqual(10, context.MarketInfluenceTemplates.AsEnumerable().Count(x =>
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Wage Squeeze", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Hiring Season", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Credit Crunch", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Trade Windfall", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Garrison Muster", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Demobilisation Glut", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Tithe Boom", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Alms Shortfall", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Noble Rent Increase", StringComparison.OrdinalIgnoreCase) ||
            x.Name.StartsWith($"{ClassicalEra} ", StringComparison.OrdinalIgnoreCase) &&
            x.Name.Contains("Patronage Windfall", StringComparison.OrdinalIgnoreCase)));
        Assert.AreEqual(21, context.MarketInfluenceTemplates.AsEnumerable().Count(x =>
            x.Name.StartsWith($"{HelperProgPrefix} Stress ", StringComparison.OrdinalIgnoreCase)));
        Assert.AreEqual(7, context.MarketPopulations.Count());
        Assert.AreEqual(7, context.Shoppers.Count());
        Assert.AreEqual(59, context.FutureProgs.Count(x => x.FunctionName.StartsWith(HelperProgPrefix, StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void SeedData_Rerun_DoesNotDuplicateSeededAssets()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        EconomySeeder seeder = new();
        Dictionary<string, string> answers = BuildAnswers();

        seeder.SeedData(context, answers);
        int initialZoneCount = context.EconomicZones.Count();
        int initialMarketCount = context.Markets.Count();
        int initialCategoryCount = context.MarketCategories.Count();
        int initialTemplateCount = context.MarketInfluenceTemplates.Count();
        int initialPopulationCount = context.MarketPopulations.Count();
        int initialShopperCount = context.Shoppers.Count();
        int initialProgCount = context.FutureProgs.Count();

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
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        EconomySeeder seeder = new();
        Dictionary<string, string> answers = BuildAnswers();

        seeder.SeedData(context, answers);
        MarketInfluenceTemplate deletedTemplate = context.MarketInfluenceTemplates.AsEnumerable().Single(x => x.Name == "EconomySeeder External Classical Age Harvest Failure");
        context.MarketInfluenceTemplates.Remove(deletedTemplate);
        context.SaveChanges();

        Assert.AreEqual(0, context.MarketInfluenceTemplates.AsEnumerable().Count(x => x.Name == deletedTemplate.Name));

        seeder.SeedData(context, answers);

        Assert.AreEqual(1, context.MarketInfluenceTemplates.AsEnumerable().Count(x => x.Name == deletedTemplate.Name));
    }

    [TestMethod]
    public void SeedData_EveryCategory_HasAtLeastThreeUpwardAndDownwardExternalTemplates()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        EconomySeeder seeder = new();

        seeder.SeedData(context, BuildAnswers());
        Dictionary<long, (int Upward, int Downward)> coverage = GetExternalCoverageByCategory(context);

        foreach (MarketCategory? category in context.MarketCategories.OrderBy(x => x.Name))
        {
            Assert.IsTrue(coverage[category.Id].Upward >= 3, $"{category.Name} should have at least three upward-pressure external templates.");
            Assert.IsTrue(coverage[category.Id].Downward >= 3, $"{category.Name} should have at least three downward-pressure external templates.");
        }
    }

    [TestMethod]
    public void SeedData_SeedsSettingAgnosticCombinationCategoryExamples()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);

        new EconomySeeder().SeedData(context, BuildAnswers());

        Dictionary<long, MarketCategory> categoriesById = context.MarketCategories
            .AsEnumerable()
            .ToDictionary(x => x.Id);

        foreach ((string familyName, string[] leafNames) in FamilyTags
                     .Where(x => StockCombinationFamilies.Contains(x.Key)))
        {
            MarketCategory familyCategory = context.MarketCategories.Single(x => x.Name == familyName);
            Assert.AreEqual(1, familyCategory.MarketCategoryType, $"{familyName} should seed as a combination category example.");

            List<string> componentNames = GetCombinationComponentIds(familyCategory)
                .Select(id => categoriesById[id].Name)
                .OrderBy(x => x)
                .ToList();
            CollectionAssert.AreEquivalent(leafNames, componentNames, $"{familyName} should be composed of its direct seeded child categories.");

            foreach (XElement component in XElement.Parse(familyCategory.CombinationCategories).Elements("Component"))
            {
                Assert.AreEqual(1.0m, decimal.Parse(component.Attribute("weight")!.Value, CultureInfo.InvariantCulture), $"{familyName} should seed equal-weight component examples.");
            }
        }

        foreach (string familyName in new[] { "Nourishment", "Domestic Heating", "Luxury Drinks", "Transportation" })
        {
            Assert.AreEqual(
                0,
                context.MarketCategories.Single(x => x.Name == familyName).MarketCategoryType,
                $"{familyName} should remain standalone in the stock seeder.");
        }
    }

    [TestMethod]
    public void SeedData_EachPopulation_HasThreeStressThresholdsAndValidProgs()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        EconomySeeder seeder = new();

        seeder.SeedData(context, BuildAnswers());

        foreach (MarketPopulation? population in context.MarketPopulations.OrderBy(x => x.Name))
        {
            List<XElement> stresses = XElement.Parse(population.MarketStressPoints).Elements("Stress").ToList();
            Assert.AreEqual(3, stresses.Count, $"{population.Name} should have three seeded stress thresholds.");

            foreach (XElement? stress in stresses)
            {
                long startProgId = long.Parse(stress.Attribute("onstart")!.Value, CultureInfo.InvariantCulture);
                long endProgId = long.Parse(stress.Attribute("onend")!.Value, CultureInfo.InvariantCulture);
                Assert.IsNotNull(context.FutureProgs.SingleOrDefault(x => x.Id == startProgId), $"{population.Name} should reference a valid start prog.");
                Assert.IsNotNull(context.FutureProgs.SingleOrDefault(x => x.Id == endProgId), $"{population.Name} should reference a valid end prog.");
            }
        }
    }

    [TestMethod]
    public void SeedData_EveryStressTemplate_IncludesSupplyContraction()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);

        new EconomySeeder().SeedData(context, BuildAnswers());

        foreach (MarketInfluenceTemplate? template in context.MarketInfluenceTemplates
                     .AsEnumerable()
                     .Where(x => x.Name.StartsWith($"{HelperProgPrefix} Stress ", StringComparison.OrdinalIgnoreCase)))
        {
            List<XElement> impacts = XElement.Parse(template.Impacts).Elements("Impact").ToList();
            Assert.IsTrue(impacts.Count > 0, $"{template.Name} should contain impacted categories.");
            Assert.IsTrue(
                impacts.Any(x => double.Parse(x.Attribute("supply")!.Value, CultureInfo.InvariantCulture) < 0.0),
                $"{template.Name} should include at least one negative supply impact.");
        }
    }

    [TestMethod]
    public void SeedData_PopulationsPersistIncomeFactorSavingsAndSavingsCaps()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);

        new EconomySeeder().SeedData(context, BuildAnswers());

        foreach (MarketPopulation? population in context.MarketPopulations.OrderBy(x => x.Name))
        {
            Assert.AreEqual(1.0m, population.IncomeFactor, $"{population.Name} should seed with a neutral base income factor.");
            Assert.AreEqual(0.0m, population.Savings, $"{population.Name} should start with no accumulated savings.");
            Assert.IsTrue(population.SavingsCap > 0.0m, $"{population.Name} should seed with a visible savings cap.");
        }
    }

    [TestMethod]
    public void SeedData_EveryCategory_GetsTariffAndSubsidyTemplatesWithFlatPriceOnly()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);

        new EconomySeeder().SeedData(context, BuildAnswers());

        foreach (MarketCategory? category in context.MarketCategories.OrderBy(x => x.Name))
        {
            foreach ((string Suffix, double ExpectedPrice) adjustment in new[]
                     {
                         ("Minor Tariff", 0.05),
                         ("Major Tariff", 0.12),
                         ("Minor Subsidy", -0.05),
                         ("Major Subsidy", -0.12)
                     })
            {
                MarketInfluenceTemplate template = context.MarketInfluenceTemplates.AsEnumerable()
                    .Single(x => x.Name == $"{ClassicalEra} {category.Name} {adjustment.Suffix}");
                XElement impact = XElement.Parse(template.Impacts).Elements("Impact").Single();

                Assert.AreEqual(0.0, double.Parse(impact.Attribute("supply")!.Value, CultureInfo.InvariantCulture), 0.0001, $"{template.Name} should not alter supply.");
                Assert.AreEqual(0.0, double.Parse(impact.Attribute("demand")!.Value, CultureInfo.InvariantCulture), 0.0001, $"{template.Name} should not alter demand.");
                Assert.AreEqual(adjustment.ExpectedPrice, double.Parse(impact.Attribute("price")!.Value, CultureInfo.InvariantCulture), 0.0001, $"{template.Name} should use the expected flat price adjustment.");
                Assert.AreEqual(0, GetPopulationIncomeImpacts(template).Count, $"{template.Name} should not include population income impacts.");
            }
        }
    }

    [TestMethod]
    public void SeedData_CreatesDedicatedIncomeTemplatesAndUpdatesSelectedExistingTemplates()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);

        new EconomySeeder().SeedData(context, BuildAnswers());

        string[] incomeTemplateNames =
        [
            "Rural Wage Squeeze",
            "Bountiful Hiring Season",
            "Merchant Credit Crunch",
            "Trade Windfall",
            "Garrison Muster",
            "Demobilisation Glut",
            "Tithe Boom",
            "Alms Shortfall",
            "Noble Rent Increase",
            "Patronage Windfall"
        ];

        foreach (string templateName in incomeTemplateNames)
        {
            MarketInfluenceTemplate template = context.MarketInfluenceTemplates.AsEnumerable()
                .Single(x => x.Name == $"{ClassicalEra} {templateName}");
            Assert.AreEqual(0, XElement.Parse(template.Impacts).Elements("Impact").Count(), $"{template.Name} should be income-only.");
            Assert.IsTrue(GetPopulationIncomeImpacts(template).Count > 0, $"{template.Name} should include income impacts.");
        }

        foreach (string templateName in new[]
                 {
                     $"{ExternalTemplatePrefix}{ClassicalEra} Harvest Failure",
                     $"{ExternalTemplatePrefix}{ClassicalEra} River Trade Disruption",
                     $"{ExternalTemplatePrefix}{ClassicalEra} Bumper Harvest",
                     $"{ExternalTemplatePrefix}{ClassicalEra} Caravan Surplus",
                     $"{ExternalTemplatePrefix}{ClassicalEra} Local War",
                     $"{ExternalTemplatePrefix}{ClassicalEra} Disbanded Levies",
                     $"{ExternalTemplatePrefix}{ClassicalEra} Smithy Subsidy"
                 })
        {
            MarketInfluenceTemplate template = context.MarketInfluenceTemplates.AsEnumerable().Single(x => x.Name == templateName);
            Assert.IsTrue(GetPopulationIncomeImpacts(template).Count > 0, $"{template.Name} should now include population income impacts.");
        }
    }

    [TestMethod]
    public void SeedData_AllPopulations_IncludeMedicineNeedAndChurchClassesExist()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        EconomySeeder seeder = new();

        seeder.SeedData(context, BuildAnswers());

        CollectionAssert.IsSubsetOf(
            new[]
            {
                "Classical Age Temple Priesthood",
                "Classical Age Monastic Orders"
            },
            context.MarketPopulations.Select(x => x.Name).ToList());

        foreach (MarketPopulation? population in context.MarketPopulations.OrderBy(x => x.Name))
        {
            List<string> categoryNames = GetPopulationNeedCategoryNames(context, population.Name);

            Assert.IsTrue(
                categoryNames.Any(x => x.Contains("Medicine", StringComparison.OrdinalIgnoreCase)),
                $"{population.Name} should include at least one medicine category in its needs.");
        }
    }

    [TestMethod]
    public void SeedData_LiterateHouseholds_IncludeWritingMaterialsNeeds()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        EconomySeeder seeder = new();
        foreach (string? era in new[] { "Classical Age", "Feudal Age", "Medieval Age", "Early Modern Age" })
        {
            seeder.SeedData(context, BuildAnswers(era: era));
        }

        string[] literatePopulations = new[]
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

        foreach (string? populationName in literatePopulations)
        {
            List<string> categoryNames = GetPopulationNeedCategoryNames(context, populationName);
            Assert.IsTrue(
                categoryNames.Any(x => x is "Wax Tablets" or "Parchment" or "Paper" or "Ink"),
                $"{populationName} should include an era-appropriate writing-material need.");
        }
    }

    [TestMethod]
    public void SeedData_LaterEras_UseBroaderServiceAndHospitalityNeeds()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        EconomySeeder seeder = new();
        foreach (string? era in new[] { "Feudal Age", "Medieval Age", "Early Modern Age" })
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
        using FuturemudDatabaseContext context = BuildContext();
        SeedEconomyPrerequisites(context);
        new EconomySeeder().SeedData(context, BuildAnswers());

        decimal poorBudget = GetBudgetForShopper(context, "Classical Age Urban Poor Shopper");
        decimal eliteBudget = GetBudgetForShopper(context, "Classical Age Patrician Elite Shopper");

        Assert.IsTrue(eliteBudget >= poorBudget * 5.0m,
            $"Elite shopper budget {eliteBudget} should be at least five times the poor shopper budget {poorBudget}.");
    }

    [TestMethod]
    public void SeedData_ShopperScale_MapsBudgetsToExpectedMultipliers()
    {
        const string shopperName = "Classical Age Urban Poor Shopper";

        using FuturemudDatabaseContext lowContext = BuildContext();
        SeedEconomyPrerequisites(lowContext);
        new EconomySeeder().SeedData(lowContext, BuildAnswers("Low"));
        decimal lowBudget = GetBudgetForShopper(lowContext, shopperName);

        using FuturemudDatabaseContext standardContext = BuildContext();
        SeedEconomyPrerequisites(standardContext);
        new EconomySeeder().SeedData(standardContext, BuildAnswers("Standard"));
        decimal standardBudget = GetBudgetForShopper(standardContext, shopperName);

        using FuturemudDatabaseContext highContext = BuildContext();
        SeedEconomyPrerequisites(highContext);
        new EconomySeeder().SeedData(highContext, BuildAnswers("High"));
        decimal highBudget = GetBudgetForShopper(highContext, shopperName);

        Assert.IsTrue(standardBudget > 0.0m);
        Assert.AreEqual(decimal.Round(standardBudget * 0.75m, 2, MidpointRounding.AwayFromZero), lowBudget);
        Assert.AreEqual(decimal.Round(standardBudget * 1.50m, 2, MidpointRounding.AwayFromZero), highBudget);
    }

    [TestMethod]
    public void SeedData_CurrencyScaling_KeepsComparableSterlingValueAcrossCurrencies()
    {
        using FuturemudDatabaseContext bitContext = BuildContext();
        SeedEconomyPrerequisites(bitContext, "Bits");
        new EconomySeeder().SeedData(bitContext, BuildAnswers(currency: "Bits"));
        decimal bitNeed = GetPopulationNeedExpenditure(bitContext, "Classical Age Urban Poor", "Staple Food");

        using FuturemudDatabaseContext poundContext = BuildContext();
        SeedEconomyPrerequisites(poundContext, "Pounds");
        new EconomySeeder().SeedData(poundContext, BuildAnswers(currency: "Pounds"));
        decimal poundNeed = GetPopulationNeedExpenditure(poundContext, "Classical Age Urban Poor", "Staple Food");

        using FuturemudDatabaseContext dollarContext = BuildContext();
        SeedEconomyPrerequisites(dollarContext, "Dollars");
        new EconomySeeder().SeedData(dollarContext, BuildAnswers(currency: "Dollars"));
        decimal dollarNeed = GetPopulationNeedExpenditure(dollarContext, "Classical Age Urban Poor", "Staple Food");

        decimal bitSterling = bitNeed / 100.0m;
        decimal poundSterling = poundNeed / 960.0m;
        decimal dollarSterling = dollarNeed / 125.0m;

        Assert.IsTrue(Math.Abs(bitSterling - poundSterling) < 0.05m, "Bits and pounds should seed to near-equivalent sterling value.");
        Assert.IsTrue(Math.Abs(bitSterling - dollarSterling) < 0.05m, "Bits and dollars should seed to near-equivalent sterling value.");
    }

    [TestMethod]
    public void SeedData_EraScaling_RaisesLaterEraCommonerBudgets()
    {
        using FuturemudDatabaseContext feudalContext = BuildContext();
        SeedEconomyPrerequisites(feudalContext, "Bits");
        new EconomySeeder().SeedData(feudalContext, BuildAnswers(era: "Feudal Age", currency: "Bits"));
        decimal feudalBudget = GetBudgetForShopper(feudalContext, "Feudal Age Peasantry Shopper");

        using FuturemudDatabaseContext earlyModernContext = BuildContext();
        SeedEconomyPrerequisites(earlyModernContext, "Bits");
        new EconomySeeder().SeedData(earlyModernContext, BuildAnswers(era: "Early Modern Age", currency: "Bits"));
        decimal earlyModernBudget = GetBudgetForShopper(earlyModernContext, "Early Modern Age Labourers Shopper");

        Assert.IsTrue(earlyModernBudget > feudalBudget,
            $"Early modern commoner budget {earlyModernBudget} should exceed feudal commoner budget {feudalBudget} when both use the same currency.");
    }
}
