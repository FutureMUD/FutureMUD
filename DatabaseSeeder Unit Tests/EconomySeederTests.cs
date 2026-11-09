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
[DoNotParallelize]
public class EconomySeederTests
{
    private const string ClassicalEra = "Classical Age";
    private const string DefaultCurrency = "Bits";
    private const string StandardScale = "Standard";

    private static readonly IReadOnlyDictionary<string, string[]> MarketTagsByParent =
        new Dictionary<string, string[]>
        {
            ["Market"] =
            [
                "Nourishment",
                "Clothing",
                "Domestic Heating",
                "Intoxicants",
                "Luxury Drinks",
                "Household Goods",
                "Military Goods",
                "Transportation",
                "Medicine",
                "Writing Materials",
                "Warehousing",
                "Professional Tools",
                "Raw Materials",
                "Lighting",
                "Hospitality",
                "Entertainment",
                "Personal Services",
                "Communications",
                "Religious Goods",
                "Construction Materials",
                "Household Consumables"
            ],
            ["Nourishment"] = ["Staple Food", "Standard Food", "Luxury Food", "Seasonings"],
            ["Seasonings"] = ["Salt", "Spices"],
			["Clothing"] =
			[
				"Simple Clothing", "Standard Clothing", "Luxury Clothing", "Winter Clothing", "Military Uniforms",
				"Work Clothing", "Ceremonial Clothing", "Religious Clothing", "Maritime Clothing"
			],
            ["Domestic Heating"] = ["Combustion Heating", "Oil Heating", "Electric Heating"],
            ["Intoxicants"] = ["Wine", "Beer", "Mead", "Spirits"],
            ["Luxury Drinks"] = ["Tea", "Coffee"],
            ["Household Goods"] =
            [
                "Simple Furniture",
                "Standard Furniture",
                "Luxury Furniture",
                "Simple Decorations",
                "Standard Decorations",
                "Luxury Decorations",
                "Simple Wares",
                "Standard Wares",
                "Luxury Wares"
            ],
            ["Military Goods"] = ["Weapons", "Ammunition", "Armour"],
            ["Weapons"] =
            [
                "Spears",
                "Swords",
                "Clubs",
                "Axes",
                "Maces",
                "Daggers",
                "Crossbows",
                "Bows",
                "Guns",
                "Hammers",
                "Polearms",
                "Other Weapons"
            ],
            ["Ammunition"] = ["Arrows", "Bolts", "Bullets", "Blackpowder"],
            ["Armour"] = ["Leather Armour", "Mail Armour", "Plate Armour", "Primitive Armour", "Shields"],
            ["Transportation"] = ["Cargo Transportation", "Passenger Transportation"],
            ["Cargo Transportation"] = ["Cart Haulage", "Manual Haulage", "Mule Haulage", "Ship Haulage"],
            ["Passenger Transportation"] = ["Cart Passage", "Horse Passage", "Wagon Passage", "Ship Passage"],
            ["Medicine"] = ["Simple Medicine", "Standard Medicine", "High-Quality Medicine", "Herbal Medicine", "Apothecary Goods", "Treatment Supplies", "Surgical Supplies", "Prosthetics and Mobility"],
            ["Writing Materials"] = ["Wax Tablets", "Papyrus", "Parchment", "Paper", "Ink", "Clay Tablets", "Writing Implements", "Document Containers", "Scrolls", "Codices"],
            ["Warehousing"] = [],
            ["Professional Tools"] = ["Primitive Tools", "Simple Tools", "Standard Tools", "High-Quality Tools"],
            ["Raw Materials"] =
            [
                "Lumber",
                "Straw",
                "Cloth",
                "Stone Blocks",
                "Sand",
                "Clay",
                "Aggregate",
                "Cement Mineral",
                "Steel",
                "Copper",
                "Gold",
                "Silver",
                "Bronze",
                "Brass",
                "Lead"
            ],
            ["Lighting"] = ["Candles", "Torches", "Lamps"],
            ["Hospitality"] = ["Simple Lodging", "Standard Lodging", "Luxury Lodging", "Prepared Meals", "Bathhouse Services", "Stabling Services"],
            ["Prepared Meals"] = ["Common Meals", "Fine Dining"],
            ["Entertainment"] = ["Cheap Entertainment", "Standard Entertainment", "Luxury Entertainment", "Music Performance", "Theatre Performance", "Festival Entertainment", "Sporting Entertainment"],
            ["Personal Services"] = ["Laundry Services", "Barbering", "Tailoring Services", "Domestic Services", "Bodyguard Services", "Bathing Services", "Grooming Supplies"],
            ["Communications"] = ["Messenger Services", "Courier Services", "Postal Services", "Printed News", "Telegraph Services", "Telephone Services"],
            ["Religious Goods"] = ["Ritual Supplies", "Temple Offerings", "Funerary Goods", "Devotional Goods"],
            ["Construction Materials"] = ["Brick", "Mortar", "Lime", "Worked Timber", "Worked Stone", "Glass Panes", "Roofing Materials"],
            ["Household Consumables"] = ["Lamp Oil", "Cleaning Supplies", "Candlemaking Wax", "Toiletries"]
        };

	private static readonly Lazy<FuturemudDatabaseContext> ClassicalContext =
		new(() => BuildSeededContext(BuildAnswers()));
	private static readonly Lazy<FuturemudDatabaseContext> AllErasContext =
		new(BuildAllErasContext);
	private static readonly Lazy<FuturemudDatabaseContext> LowScaleContext =
		new(() => BuildSeededContext(BuildAnswers(shopperScale: "Low")));
	private static readonly Lazy<FuturemudDatabaseContext> HighScaleContext =
		new(() => BuildSeededContext(BuildAnswers(shopperScale: "High")));
	private static readonly Lazy<FuturemudDatabaseContext> PoundsContext =
		new(() => BuildSeededContext(BuildAnswers(currency: "Pounds"), "Pounds"));
	private static readonly Lazy<FuturemudDatabaseContext> DollarsContext =
		new(() => BuildSeededContext(BuildAnswers(currency: "Dollars"), "Dollars"));

	[ClassCleanup]
	public static void ClassCleanup()
	{
		foreach (var context in new[]
		         {
			         ClassicalContext,
			         AllErasContext,
			         LowScaleContext,
			         HighScaleContext,
			         PoundsContext,
			         DollarsContext
		         }
		         .Where(x => x.IsValueCreated)
		         .Select(x => x.Value))
		{
			context.Dispose();
		}
	}

	private static FuturemudDatabaseContext BuildSeededContext(
		Dictionary<string, string> answers,
		params string[] currencyNames)
	{
		var context = BuildContext();
		SeedEconomyPrerequisites(context, currencyNames);
		var result = new EconomySeeder().SeedData(context, answers);
		if (!result.Equals("The operation completed successfully.", StringComparison.Ordinal))
		{
			context.Dispose();
			throw new InvalidOperationException($"Could not build shared economy test fixture: {result}");
		}

		return context;
	}

	private static FuturemudDatabaseContext BuildAllErasContext()
	{
		var context = BuildContext();
		SeedEconomyPrerequisites(context);
		var seeder = new EconomySeeder();
		foreach (var era in new[] { "Classical Age", "Feudal Age", "Medieval Age", "Early Modern Age" })
		{
			var result = seeder.SeedData(context, BuildAnswers(era: era));
			if (!result.Equals("The operation completed successfully.", StringComparison.Ordinal))
			{
				context.Dispose();
				throw new InvalidOperationException($"Could not build shared {era} economy test fixture: {result}");
			}
		}

		return context;
	}

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
        SeedEconomyPrerequisites(context, true, currencyNames);
    }

    private static void SeedEconomyPrerequisites(
        FuturemudDatabaseContext context,
        bool includeMarketTags,
        params string[] currencyNames)
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

        if (includeMarketTags)
        {
            SeedMarketTags(context);
        }

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
        AddMarketTagChildren(context, root, ref nextId);
    }

    private static void AddMarketTagChildren(FuturemudDatabaseContext context, Tag parent, ref long nextId)
    {
        if (!MarketTagsByParent.TryGetValue(parent.Name, out string[]? childNames))
        {
            return;
        }

        foreach (string childName in childNames)
        {
            Tag child = new()
            {
                Id = nextId++,
                Name = childName,
                Parent = parent,
                ParentId = parent.Id
            };
            context.Tags.Add(child);
            AddMarketTagChildren(context, child, ref nextId);
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
		return GetCombinationComponents(category)
			.Select(x => x.CategoryId)
			.ToList();
	}

	private static List<(long CategoryId, decimal Weight)> GetCombinationComponents(MarketCategory category)
	{
		return XElement.Parse(category.CombinationCategories ?? "<Components />")
			.Elements("Component")
			.Select(x => (
				CategoryId: long.Parse(x.Attribute("category")!.Value, CultureInfo.InvariantCulture),
				Weight: decimal.Parse(x.Attribute("weight")!.Value, CultureInfo.InvariantCulture)))
			.ToList();
	}

    private static void AssertPositiveExistingId(long id, ISet<long> validIds, string description)
    {
        Assert.IsTrue(id > 0, $"{description} should not use a placeholder ID.");
        Assert.IsTrue(validIds.Contains(id), $"{description} should reference an existing seeded row.");
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
    public void SeedData_DoesNotSerializePlaceholderIdsInStockXml()
    {
        FuturemudDatabaseContext context = ClassicalContext.Value;

        HashSet<long> categoryIds = context.MarketCategories.Select(x => x.Id).ToHashSet();
        HashSet<long> populationIds = context.MarketPopulations.Select(x => x.Id).ToHashSet();
        HashSet<long> futureProgIds = context.FutureProgs.Select(x => x.Id).ToHashSet();

        foreach (MarketCategory? category in context.MarketCategories.OrderBy(x => x.Name))
        {
            foreach ((long categoryId, _) in GetCombinationComponents(category))
            {
                AssertPositiveExistingId(categoryId, categoryIds, $"{category.Name} combination component {categoryId}");
            }
        }

        foreach (MarketInfluenceTemplate? template in context.MarketInfluenceTemplates.OrderBy(x => x.Name))
        {
            foreach (XElement impact in XElement.Parse(template.Impacts ?? "<Impacts />").Elements("Impact"))
            {
                long categoryId = long.Parse(impact.Attribute("category")!.Value, CultureInfo.InvariantCulture);
                AssertPositiveExistingId(categoryId, categoryIds, $"{template.Name} category impact {categoryId}");
            }

            foreach (XElement impact in GetPopulationIncomeImpacts(template))
            {
                long populationId = long.Parse(impact.Attribute("population")!.Value, CultureInfo.InvariantCulture);
                AssertPositiveExistingId(populationId, populationIds, $"{template.Name} population impact {populationId}");
            }
        }

        foreach (MarketPopulation? population in context.MarketPopulations.OrderBy(x => x.Name))
        {
            XElement needs = XElement.Parse(population.MarketPopulationNeeds ?? "<Needs />");
            foreach (XElement need in needs.Elements("Need"))
            {
                long categoryId = long.Parse(need.Attribute("category")!.Value, CultureInfo.InvariantCulture);
                AssertPositiveExistingId(categoryId, categoryIds, $"{population.Name} need category {categoryId}");
            }

            XElement stressPoints = XElement.Parse(population.MarketStressPoints ?? "<Stresses />");
            foreach (XElement stress in stressPoints.Elements("Stress"))
            {
                long onStartId = long.Parse(stress.Attribute("onstart")!.Value, CultureInfo.InvariantCulture);
                long onEndId = long.Parse(stress.Attribute("onend")!.Value, CultureInfo.InvariantCulture);
                AssertPositiveExistingId(onStartId, futureProgIds, $"{population.Name} stress start prog {onStartId}");
                AssertPositiveExistingId(onEndId, futureProgIds, $"{population.Name} stress end prog {onEndId}");
            }
        }

        foreach (Shopper? shopper in context.Shoppers.OrderBy(x => x.Name))
        {
            XElement definition = XElement.Parse(shopper.Definition);
            foreach (string progElementName in new[] { "WillShopAtShopProg", "ShopSelectionWeightProg", "WillBuyItemProg", "ItemBuyWeightProg" })
            {
                long progId = long.Parse(definition.Element(progElementName)!.Value, CultureInfo.InvariantCulture);
                AssertPositiveExistingId(progId, futureProgIds, $"{shopper.Name} {progElementName} {progId}");
            }
        }
    }

    [TestMethod]
    public void SeedData_PopulationStressThresholdsReferenceExistingProgs()
    {
        FuturemudDatabaseContext context = ClassicalContext.Value;

        foreach (MarketPopulation? population in context.MarketPopulations.OrderBy(x => x.Name))
        {
            List<XElement> stresses = XElement.Parse(population.MarketStressPoints).Elements("Stress").ToList();

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
    public void SeedData_ShopperScale_MapsBudgetsToExpectedMultipliers()
    {
        const string shopperName = "Classical Age Urban Poor Shopper";

        FuturemudDatabaseContext lowContext = LowScaleContext.Value;
        decimal lowBudget = GetBudgetForShopper(lowContext, shopperName);

        FuturemudDatabaseContext standardContext = ClassicalContext.Value;
        decimal standardBudget = GetBudgetForShopper(standardContext, shopperName);

        FuturemudDatabaseContext highContext = HighScaleContext.Value;
        decimal highBudget = GetBudgetForShopper(highContext, shopperName);

        Assert.IsTrue(standardBudget > 0.0m);
        Assert.AreEqual(decimal.Round(standardBudget * 0.75m, 2, MidpointRounding.AwayFromZero), lowBudget);
        Assert.AreEqual(decimal.Round(standardBudget * 1.50m, 2, MidpointRounding.AwayFromZero), highBudget);
    }

    [TestMethod]
    public void SeedData_CurrencyScaling_KeepsComparableSterlingValueAcrossCurrencies()
    {
        FuturemudDatabaseContext bitContext = ClassicalContext.Value;
        decimal bitNeed = GetPopulationNeedExpenditure(bitContext, "Classical Age Urban Poor", "Staple Food");

        FuturemudDatabaseContext poundContext = PoundsContext.Value;
        decimal poundNeed = GetPopulationNeedExpenditure(poundContext, "Classical Age Urban Poor", "Staple Food");

        FuturemudDatabaseContext dollarContext = DollarsContext.Value;
        decimal dollarNeed = GetPopulationNeedExpenditure(dollarContext, "Classical Age Urban Poor", "Staple Food");

        decimal bitSterling = bitNeed / 100.0m;
        decimal poundSterling = poundNeed / 960.0m;
        decimal dollarSterling = dollarNeed / 125.0m;

        Assert.IsTrue(Math.Abs(bitSterling - poundSterling) < 0.05m, "Bits and pounds should seed to near-equivalent sterling value.");
        Assert.IsTrue(Math.Abs(bitSterling - dollarSterling) < 0.05m, "Bits and dollars should seed to near-equivalent sterling value.");
    }
}
