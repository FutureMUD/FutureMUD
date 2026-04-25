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
using GravityModel = MudSharp.Construction.GravityModel;
using RevisionStatus = MudSharp.Framework.Revision.RevisionStatus;

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
            GravityModel = (int)GravityModel.Normal,
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

    private static ForagableProfile AddCustomForageProfile(FuturemudDatabaseContext context, string name)
    {
        long profileId = context.ForagableProfiles
            .Select(x => x.Id)
            .AsEnumerable()
            .DefaultIfEmpty(0L)
            .Max() + 1L;
        ForagableProfile profile = new()
        {
            Id = profileId,
            RevisionNumber = 0,
            Name = name,
            EditableItem = new EditableItem
            {
                RevisionNumber = 0,
                RevisionStatus = (int)RevisionStatus.Current,
                BuilderAccountId = 0,
                BuilderDate = DateTime.UtcNow,
                BuilderComment = "Test custom forage profile"
            }
        };
        context.ForagableProfiles.Add(profile);
        context.ForagableProfilesMaximumYields.Add(new ForagableProfilesMaximumYields
        {
            ForagableProfile = profile,
            ForagableProfileId = profile.Id,
            ForagableProfileRevisionNumber = profile.RevisionNumber,
            ForageType = "custom",
            Yield = 10.0
        });
        context.ForagableProfilesHourlyYieldGains.Add(new ForagableProfilesHourlyYieldGains
        {
            ForagableProfile = profile,
            ForagableProfileId = profile.Id,
            ForagableProfileRevisionNumber = profile.RevisionNumber,
            ForageType = "custom",
            Yield = 1.0
        });
        context.SaveChanges();
        return profile;
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
        int initialForageProfileCount = context.ForagableProfiles.Count();
        int initialMaximumYieldCount = context.ForagableProfilesMaximumYields.Count();
        int initialHourlyYieldCount = context.ForagableProfilesHourlyYieldGains.Count();
        CoreDataSeeder.SeedTerrainFoundationsForTesting(context);

        foreach (string tagName in CoreDataSeeder.StockTerrainTagNamesForTesting)
        {
            Assert.AreEqual(1, context.Tags.Count(x => x.Name == tagName), $"Expected rerun to preserve a single terrain tag named {tagName}.");
        }

        foreach (string? terrainName in new[] { "Residence", "Urban Street", "Ocean", "River" })
        {
            Assert.AreEqual(1, context.Terrains.Count(x => x.Name == terrainName), $"Expected rerun to preserve a single terrain named {terrainName}.");
        }

        Assert.AreEqual(initialForageProfileCount, context.ForagableProfiles.Count());
        Assert.AreEqual(initialMaximumYieldCount, context.ForagableProfilesMaximumYields.Count());
        Assert.AreEqual(initialHourlyYieldCount, context.ForagableProfilesHourlyYieldGains.Count());

        foreach (string terrainName in CoreDataSeeder.StockTerrainForageProfileTerrainNamesForTesting)
        {
            Assert.AreEqual(1, context.ForagableProfiles.Count(x => x.Name == $"{terrainName} Stock Forage"),
                $"Expected rerun to preserve a single stock forage profile for {terrainName}.");
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
                     "Moon Surface", "Lunar Mare", "Lunar Crater", "Orbital Space", "Interstellar Space",
                     "Intergalactic Space", "Zero-G Spaceship Compartment"
                 })
        {
            Assert.IsTrue(context.Terrains.Any(x => x.Name == terrainName),
                $"Expected terrain {terrainName} to be present in the stock catalogue.");
        }

        foreach (string terrainName in new[] { "Moon Surface", "Lunar Mare", "Lunar Highlands", "Lunar Crater", "Asteroid Surface" })
        {
            Assert.AreEqual((int)GravityModel.Normal, context.Terrains.Single(x => x.Name == terrainName).GravityModel,
                $"Expected terrain {terrainName} to remain normal gravity.");
        }

        foreach (string terrainName in new[]
                 { "Orbital Space", "Interplanetary Space", "Interstellar Space", "Intergalactic Space", "Zero-G Spaceship Compartment" })
        {
            Assert.AreEqual((int)GravityModel.ZeroGravity, context.Terrains.Single(x => x.Name == terrainName).GravityModel,
                $"Expected terrain {terrainName} to be zero gravity.");
        }
    }

    [TestMethod]
    public void SeedTerrainFoundationsForTesting_SeedsStockForageProfilesForAppropriateTerrains()
    {
        using FuturemudDatabaseContext context = BuildContext();

        SeedTerrainFoundations(context);

        foreach (string yieldType in new[]
                 {
                     "grass", "shrubs", "low-trees", "high-trees", "tubers", "roots", "seeds", "fruit", "nuts",
                     "herbs", "flowers", "moss", "lichen", "mushrooms", "reeds-rushes", "aquatic-plants",
                     "sea-grass", "algae", "plankton", "insects", "grubs-worms", "crustaceans", "shellfish",
                     "molluscs", "tiny-fish", "leaves-detritus", "vines", "branches-brushwood", "deadwood",
                     "mature-trees", "pebbles", "rocks", "boulders", "sand", "clay", "shells", "coral", "ice",
                     "trash", "discarded-food"
                 })
        {
            Assert.IsTrue(CoreDataSeeder.StockTerrainForageYieldTypesForTesting.Contains(yieldType),
                $"Expected stock forage yield type {yieldType} to be used by at least one terrain.");
        }

        foreach (string terrainName in CoreDataSeeder.StockTerrainForageProfileTerrainNamesForTesting)
        {
            Terrain terrain = context.Terrains.Single(x => x.Name == terrainName);
            Assert.AreNotEqual(0, terrain.ForagableProfileId,
                $"Expected terrain {terrainName} to have a default forage profile.");

            ForagableProfile profile = context.ForagableProfiles.Single(x => x.Id == terrain.ForagableProfileId);
            EditableItem editableItem = context.EditableItems.Single(x => x.Id == profile.EditableItemId);
            List<ForagableProfilesMaximumYields> maximums = context.ForagableProfilesMaximumYields
                .Where(x => x.ForagableProfileId == profile.Id &&
                            x.ForagableProfileRevisionNumber == profile.RevisionNumber)
                .ToList();
            List<ForagableProfilesHourlyYieldGains> hourly = context.ForagableProfilesHourlyYieldGains
                .Where(x => x.ForagableProfileId == profile.Id &&
                            x.ForagableProfileRevisionNumber == profile.RevisionNumber)
                .ToList();

            Assert.AreEqual($"{terrainName} Stock Forage", profile.Name);
            Assert.AreEqual((int)RevisionStatus.Current, editableItem.RevisionStatus);
            Assert.AreEqual(0, context.ForagableProfilesForagables.Count(x =>
                x.ForagableProfileId == profile.Id &&
                x.ForagableProfileRevisionNumber == profile.RevisionNumber));
            Assert.IsTrue(maximums.Any(), $"Expected terrain {terrainName} to have stock forage yields.");
            Assert.AreEqual(maximums.Count, hourly.Count);
            Assert.IsTrue(maximums.All(x => x.Yield > 0.0));
            Assert.IsTrue(hourly.All(x => x.Yield > 0.0));
            CollectionAssert.AreEquivalent(
                maximums.Select(x => x.ForageType).ToArray(),
                hourly.Select(x => x.ForageType).ToArray());
            CollectionAssert.IsSubsetOf(
                CoreDataSeeder.StockTerrainForageYieldTypesByTerrainForTesting[terrainName].ToArray(),
                maximums.Select(x => x.ForageType).ToArray(),
                $"Expected terrain {terrainName} to contain all of its declared forage yield types.");
        }
    }

    [TestMethod]
    public void SeedTerrainFoundationsForTesting_SeedsRepresentativeTerrainForageYieldsAndLeavesBarrenTerrainEmpty()
    {
        using FuturemudDatabaseContext context = BuildContext();

        SeedTerrainFoundations(context);

        void AssertTerrainHasYields(string terrainName, params string[] yields)
        {
            Terrain terrain = context.Terrains.Single(x => x.Name == terrainName);
            ForagableProfile profile = context.ForagableProfiles.Single(x => x.Id == terrain.ForagableProfileId);
            HashSet<string> profileYields = context.ForagableProfilesMaximumYields
                .Where(x => x.ForagableProfileId == profile.Id &&
                            x.ForagableProfileRevisionNumber == profile.RevisionNumber)
                .Select(x => x.ForageType)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (string yield in yields)
            {
                Assert.IsTrue(profileYields.Contains(yield),
                    $"Expected terrain {terrainName} to contain the {yield} forage yield.");
            }
        }

        AssertTerrainHasYields("Grasslands", "grass", "insects", "seeds");
        AssertTerrainHasYields("Boreal Forest", "high-trees", "mature-trees", "branches-brushwood", "mushrooms");
        AssertTerrainHasYields("Ocean", "plankton", "algae", "tiny-fish");
        AssertTerrainHasYields("Coral Reef", "coral", "shellfish", "tiny-fish");
        AssertTerrainHasYields("Garbage Dump", "trash", "discarded-food", "grubs-worms");
        AssertTerrainHasYields("Cave", "mushrooms", "rocks", "insects");

        foreach (string terrainName in new[]
                 {
                     "Residence", "Bedroom", "Kitchen", "Indoor Pool", "Moon Surface", "Lunar Mare",
                     "Asteroid Surface", "Orbital Space", "Interstellar Space", "Intergalactic Space"
                 })
        {
            Assert.AreEqual(0, context.Terrains.Single(x => x.Name == terrainName).ForagableProfileId,
                $"Expected terrain {terrainName} to remain without a stock forage profile.");
        }
    }

    [TestMethod]
    public void SeedTerrainFoundationsForTesting_RerunPreservesCustomProfileAssignmentsAndRepairsInvalidAssignments()
    {
        using FuturemudDatabaseContext context = BuildContext();

        SeedTerrainFoundations(context);
        ForagableProfile customProfile = AddCustomForageProfile(context, "Builder Custom Forage");
        Terrain grasslands = context.Terrains.Single(x => x.Name == "Grasslands");
        Terrain ocean = context.Terrains.Single(x => x.Name == "Ocean");
        Terrain orbitalSpace = context.Terrains.Single(x => x.Name == "Orbital Space");
        Terrain moonSurface = context.Terrains.Single(x => x.Name == "Moon Surface");

        grasslands.ForagableProfileId = customProfile.Id;
        ocean.ForagableProfileId = 999999L;
        orbitalSpace.GravityModel = (int)GravityModel.Normal;
        moonSurface.GravityModel = (int)GravityModel.ZeroGravity;
        context.SaveChanges();

        CoreDataSeeder.SeedTerrainFoundationsForTesting(context);

        Assert.AreEqual(customProfile.Id, context.Terrains.Single(x => x.Name == "Grasslands").ForagableProfileId,
            "Expected rerun to preserve terrain assignments to a valid custom forage profile.");

        Terrain repairedOcean = context.Terrains.Single(x => x.Name == "Ocean");
        ForagableProfile oceanStockProfile = context.ForagableProfiles.Single(x => x.Name == "Ocean Stock Forage");
        Assert.AreEqual(oceanStockProfile.Id, repairedOcean.ForagableProfileId,
            "Expected rerun to repair terrain assignments that point at no current forage profile.");
        Assert.AreEqual((int)GravityModel.ZeroGravity, context.Terrains.Single(x => x.Name == "Orbital Space").GravityModel,
            "Expected rerun to repair orbital space to zero gravity.");
        Assert.AreEqual((int)GravityModel.Normal, context.Terrains.Single(x => x.Name == "Moon Surface").GravityModel,
            "Expected rerun to preserve lunar terrain as normal gravity.");
    }
}
