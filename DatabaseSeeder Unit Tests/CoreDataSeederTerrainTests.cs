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

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederTerrainTests
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

    private static HashSet<string> GetTerrainTagNames(FuturemudDatabaseContext context, Terrain terrain)
    {
        if (string.IsNullOrWhiteSpace(terrain.TagInformation))
        {
            return [];
        }

        Dictionary<long, string> tags = context.Tags.ToDictionary(x => x.Id, x => x.Name);
        return terrain.TagInformation
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .Select(x => tags[x])
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    [TestMethod]
    public void SeedTerrainFoundationsForTesting_SeedsTerrainTagsAndCatalogue()
    {
        using FuturemudDatabaseContext context = BuildContext();

        SeedTerrainFoundations(context);

        foreach (string tagName in CoreDataSeeder.StockTerrainTagNamesForTesting)
        {
            Assert.AreEqual(1, context.Tags.Count(x => x.Name == tagName), $"Expected a single terrain tag named {tagName}.");
        }

        Assert.IsTrue(context.Terrains.Any(x => x.Name == "Residence"));
        Assert.IsTrue(context.Terrains.Any(x => x.Name == "Urban Street"));
        Assert.IsTrue(context.Terrains.Any(x => x.Name == "Ocean"));
        Assert.IsTrue(context.Terrains.Any(x => x.Name == "River"));
    }

    [TestMethod]
    public void SeedTerrainFoundationsForTesting_RerunDoesNotDuplicateTagsOrTerrains()
    {
        using FuturemudDatabaseContext context = BuildContext();

        SeedTerrainFoundations(context);
        CoreDataSeeder.SeedTerrainFoundationsForTesting(context);

        foreach (string tagName in CoreDataSeeder.StockTerrainTagNamesForTesting)
        {
            Assert.AreEqual(1, context.Tags.Count(x => x.Name == tagName), $"Expected rerun to preserve a single terrain tag named {tagName}.");
        }

        foreach (string? terrainName in new[] { "Residence", "Urban Street", "Ocean", "River" })
        {
            Assert.AreEqual(1, context.Terrains.Count(x => x.Name == terrainName), $"Expected rerun to preserve a single terrain named {terrainName}.");
        }
    }

    [TestMethod]
    public void UsefulSeeder_NoLongerOwnsTerrainQuestionOrTerrainInstalledState()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedAccount(context);
        SeedTerrainFoundations(context);
        UsefulSeeder seeder = new();

        List<string> ids = seeder.SeederQuestions.Select(x => x.Id).ToList();

        Assert.IsFalse(ids.Contains("terrain"));
        Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));
    }

    [TestMethod]
    public void SeedTerrainFoundationsForTesting_CorrectsTerrainBehavioursAndAddsExtraterrestrialTerrains()
    {
        using FuturemudDatabaseContext context = BuildContext();

        SeedTerrainFoundations(context);

        Terrain cave = context.Terrains.Single(x => x.Name == "Cave");
        Terrain cavePool = context.Terrains.Single(x => x.Name == "Cave Pool");
        Terrain undergroundWater = context.Terrains.Single(x => x.Name == "Underground Water");
        Terrain villageStreet = context.Terrains.Single(x => x.Name == "Village Street");
        Terrain ruralStreet = context.Terrains.Single(x => x.Name == "Rural Street");
        Terrain mudflat = context.Terrains.Single(x => x.Name == "Mudflat");
        long springWaterId = context.Liquids.Single(x => x.Name == "spring water").Id;

        Assert.AreEqual("cave", cave.TerrainBehaviourMode);
        Assert.AreEqual($"shallowwatercave {springWaterId}", cavePool.TerrainBehaviourMode);
        Assert.AreEqual($"deepwatercave {springWaterId}", undergroundWater.TerrainBehaviourMode);

        HashSet<string> villageStreetTags = GetTerrainTagNames(context, villageStreet);
        HashSet<string> ruralStreetTags = GetTerrainTagNames(context, ruralStreet);
        HashSet<string> mudflatTags = GetTerrainTagNames(context, mudflat);

        Assert.IsTrue(villageStreetTags.Contains("Rural"));
        Assert.IsFalse(villageStreetTags.Contains("Urban"));
        Assert.IsTrue(ruralStreetTags.Contains("Rural"));
        Assert.IsFalse(ruralStreetTags.Contains("Urban"));
        Assert.IsTrue(mudflatTags.Contains("Littoral"));
        Assert.IsTrue(mudflatTags.Contains("Wetland"));
        Assert.IsFalse(mudflatTags.Contains("Aquatic"));

        foreach (string tagName in new[]
                 { "Extraterrestrial", "Lunar", "Space", "Vacuum", "Arid", "Glacial", "Volcanic", "Wetland" })
        {
            Assert.IsTrue(context.Tags.Any(x => x.Name == tagName), $"Expected terrain tag {tagName} to be seeded.");
        }

        foreach (string terrainName in new[]
                 {
                     "Chaparral", "Badlands", "Salt Flat", "Fen", "Marsh", "Oasis", "Volcanic Plain", "Glacier",
                     "Moon Surface", "Lunar Mare", "Lunar Crater", "Interstellar Space", "Intergalactic Space"
                 })
        {
            Assert.IsTrue(context.Terrains.Any(x => x.Name == terrainName),
                $"Expected terrain {terrainName} to be present in the stock catalogue.");
        }
    }
}
