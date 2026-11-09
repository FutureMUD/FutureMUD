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
using System.Text.RegularExpressions;
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
            DefaultRoomOutdoorsType = 0,
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
	public void SeedTerrainFoundationsForTesting_RerunRepairsStockGasAndLiquidAtmospheres()
	{
		using FuturemudDatabaseContext context = BuildContext();

		SeedTerrainFoundations(context);

		Terrain grasslands = context.Terrains.Single(x => x.Name == "Grasslands");
		Terrain deepOcean = context.Terrains.Single(x => x.Name == "Deep Ocean");
		grasslands.AtmosphereId = null;
		deepOcean.AtmosphereId = null;
		context.SaveChanges();

		CoreDataSeeder.SeedTerrainFoundationsForTesting(context);

		Assert.AreEqual(
			context.Gases.Single(x => x.Name == "Breathable Atmosphere").Id,
			grasslands.AtmosphereId,
			"Expected rerun to repair a stock gas atmosphere.");
		Assert.AreEqual(
			context.Liquids.Single(x => x.Name == "salt water").Id,
			deepOcean.AtmosphereId,
			"Expected rerun to repair a stock liquid atmosphere.");
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
    public void SeedTerrainFoundationsForTesting_RerunPreservesCustomProfileAssignmentsAndRepairsInvalidAssignments()
    {
        using FuturemudDatabaseContext context = BuildContext();

        SeedTerrainFoundations(context);
        ForagableProfile customProfile = AddCustomForageProfile(context, "Builder Custom Forage");
        Terrain grasslands = context.Terrains.Single(x => x.Name == "Grasslands");
        Terrain ocean = context.Terrains.Single(x => x.Name == "Ocean");
        Terrain orbitalSpace = context.Terrains.Single(x => x.Name == "Orbital Space");
        Terrain moonSurface = context.Terrains.Single(x => x.Name == "Moon Surface");
        Gas breathableAtmosphere = context.Gases.Single(x => x.Name == "Breathable Atmosphere");
        long customAtmosphereId = context.Gases.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0L).Max() + 1L;
        Gas customAtmosphere = new()
        {
            Id = customAtmosphereId,
            Name = "Builder Custom Atmosphere",
            Description = "A builder-custom atmosphere",
            Density = 0.002,
            ThermalConductivity = 0.02,
            ElectricalConductivity = 0.000001,
            Organic = false,
            SpecificHeatCapacity = 1.0,
            BoilingPoint = -150,
            DisplayColour = "green",
            Viscosity = 10,
            SmellIntensity = 0,
            SmellText = "It has no smell",
            VagueSmellText = "It has no smell"
        };
        context.Gases.Add(customAtmosphere);

        grasslands.ForagableProfileId = customProfile.Id;
        grasslands.AtmosphereId = null;
        grasslands.AtmosphereType = "Gas";
        ocean.ForagableProfileId = 999999L;
        ocean.AtmosphereId = customAtmosphere.Id;
        ocean.AtmosphereType = "Gas";
        orbitalSpace.GravityModel = (int)GravityModel.Normal;
        moonSurface.GravityModel = (int)GravityModel.ZeroGravity;
        context.SaveChanges();

        CoreDataSeeder.SeedTerrainFoundationsForTesting(context);

        Assert.AreEqual(customProfile.Id, context.Terrains.Single(x => x.Name == "Grasslands").ForagableProfileId,
            "Expected rerun to preserve terrain assignments to a valid custom forage profile.");
        Assert.AreEqual(breathableAtmosphere.Id, context.Terrains.Single(x => x.Name == "Grasslands").AtmosphereId,
            "Expected rerun to repair stock terrains that were seeded before the breathable atmosphere existed.");
        Assert.IsNull(context.Terrains.Single(x => x.Name == "Moon Surface").AtmosphereId,
            "Expected rerun to leave vacuum stock terrains without a breathable atmosphere.");

        Terrain repairedOcean = context.Terrains.Single(x => x.Name == "Ocean");
        ForagableProfile oceanStockProfile = context.ForagableProfiles.Single(x => x.Name == "Ocean Stock Forage");
        Assert.AreEqual(oceanStockProfile.Id, repairedOcean.ForagableProfileId,
            "Expected rerun to repair terrain assignments that point at no current forage profile.");
        Assert.AreEqual(customAtmosphere.Id, repairedOcean.AtmosphereId,
            "Expected rerun to preserve builder-customized stock terrain atmospheres.");
        Assert.AreEqual((int)GravityModel.ZeroGravity, context.Terrains.Single(x => x.Name == "Orbital Space").GravityModel,
            "Expected rerun to repair orbital space to zero gravity.");
        Assert.AreEqual((int)GravityModel.Normal, context.Terrains.Single(x => x.Name == "Moon Surface").GravityModel,
            "Expected rerun to preserve lunar terrain as normal gravity.");
    }
}
