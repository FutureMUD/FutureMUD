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

namespace MudSharp_Unit_Tests;

[TestClass]
public class CelestialSeederTests
{
    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
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

        context.Clocks.Add(new Clock
        {
            Id = 1,
            Definition = "<Clock />",
            Seconds = 0,
            Minutes = 0,
            Hours = 12,
            PrimaryTimezoneId = 1
        });

        context.Calendars.Add(new Calendar
        {
            Id = 1,
            Definition = "<calendar><alias>test</alias><shortname>Test Calendar</shortname><fullname>Test Calendar</fullname></calendar>",
            Date = "1-1-1",
            FeedClockId = 1
        });

        context.SaveChanges();
    }

    private static Dictionary<string, string> BuildAnswers(
        bool installSun = false,
        bool installMoon = false,
        bool installGasGiant = false)
    {
        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["installsun"] = installSun ? "yes" : "no",
            ["suncalendar"] = "1",
            ["sunname"] = "The Sun",
            ["sunepoch"] = "1-jan-2000",
            ["installmoon"] = installMoon ? "yes" : "no",
            ["mooncalendar"] = "1",
            ["moonname"] = "The Moon",
            ["moonepoch"] = "21-jan-2000",
            ["installgasgiantmoon"] = installGasGiant ? "yes" : "no",
            ["gasgiantcalendar"] = "1",
            ["gasgiantsunepoch"] = "1-jan-2000",
            ["gasgiantmoonepoch"] = "1-jan-2000"
        };
    }

    private static long SeedCalendar(FuturemudDatabaseContext context, string mode, string startYear, string? ardaAge = null)
    {
        Dictionary<string, string> answers = new()
        {
            ["secondsmultiplier"] = "2",
            ["mode"] = mode,
            ["startyear"] = startYear
        };
        if (!string.IsNullOrWhiteSpace(ardaAge))
        {
            answers["ardaage"] = ardaAge;
        }

        new TimeSeeder().SeedData(context, answers);

        return mode switch
        {
            "middle-earth" => context.Calendars.AsEnumerable().First(x => x.Date.Contains("yestare")).Id,
            _ => context.Calendars.Single().Id
        };
    }

    private static XElement GetDefinition(Celestial celestial)
    {
        return XElement.Parse(celestial.Definition);
    }

    private static string GetPackage(Celestial celestial)
    {
        return GetDefinition(celestial).Element("SeederPackage")!.Value;
    }

    private static string? GetPathValue(XElement root, string path)
    {
        XElement? current = root;
        foreach (string part in path.Split('/'))
        {
            current = current.Element(part)!;
            if (current is null)
            {
                return null;
            }
        }

        return current.Value;
    }

    private static double GetRequiredDouble(XElement root, string path)
    {
        return double.Parse(
            GetPathValue(root, path) ?? throw new AssertFailedException($"Missing path {path}"),
            System.Globalization.CultureInfo.InvariantCulture);
    }

    [TestMethod]
    public void SeedData_EarthMoonPackage_CreatesLinkedCelestials()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        CelestialSeeder seeder = new();

        seeder.SeedData(context, BuildAnswers(installSun: true, installMoon: true));

        List<Celestial> celestials = context.Celestials.ToList();
        Assert.AreEqual(4, celestials.Count);

        Celestial sun = celestials.Single(x => x.CelestialType == "Sun");
        Celestial moon = celestials.Single(x => x.CelestialType == "PlanetaryMoon" && GetPackage(x) == "EarthMoonView");
        Celestial planet = celestials.Single(x => x.CelestialType == "PlanetFromMoon" && GetPackage(x) == "EarthMoonView");
        Celestial sunFromMoon = celestials.Single(x => x.CelestialType == "SunFromPlanetaryMoon" && GetPackage(x) == "EarthMoonView");

        Assert.AreEqual("EarthSun", GetPackage(sun));

        XElement planetDefinition = GetDefinition(planet);
        XElement moonDefinition = GetDefinition(moon);
        XElement sunDefinition = GetDefinition(sun);
        XElement sunFromMoonDefinition = GetDefinition(sunFromMoon);

        Assert.AreEqual(moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), planetDefinition.Element("Moon")!.Value);
        Assert.AreEqual(sun.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), planetDefinition.Element("Sun")!.Value);
        Assert.AreEqual("Earth", planetDefinition.Element("Name")!.Value);
        Assert.AreEqual(moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), sunFromMoonDefinition.Element("Moon")!.Value);
        Assert.AreEqual(sun.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), sunFromMoonDefinition.Element("Sun")!.Value);
        Assert.AreEqual("The Sun", sunFromMoonDefinition.Element("Name")!.Value);
        Assert.IsNotNull(sunFromMoonDefinition.Element("Illumination"));

        Assert.AreEqual(0.016713, GetRequiredDouble(sunDefinition, "Orbital/OrbitalEccentricity"), 0.0000001);
        Assert.AreEqual(149597870.7, GetRequiredDouble(sunDefinition, "Orbital/OrbitalSemiMajorAxis"), 0.0001);
        Assert.AreEqual(0.004654793, GetRequiredDouble(sunDefinition, "Orbital/ApparentAngularRadius"), 0.0000001);
        Assert.AreEqual(384400.0, GetRequiredDouble(moonDefinition, "Orbital/OrbitalSemiMajorAxis"), 0.0001);
        Assert.AreEqual(4.889488, GetRequiredDouble(moonDefinition, "Orbital/SiderealTimeAtEpoch"), 0.0000001);
        Assert.AreEqual(6.300388, GetRequiredDouble(moonDefinition, "Orbital/SiderealTimePerDay"), 0.0000001);
        Assert.AreEqual(0.004654793, GetRequiredDouble(planetDefinition, "SunAngularRadius"), 0.0000001);
    }

    [TestMethod]
    public void SeedData_EarthMoonPackageWithoutEarthSun_ThrowsClearError()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        CelestialSeeder seeder = new();

        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(() =>
            seeder.SeedData(context, BuildAnswers(installMoon: true)));

        StringAssert.Contains(exception.Message, "requires a matching Earth-facing Sun");
    }

    [TestMethod]
    public void SeedData_GasGiantPackage_CreatesSelfContainedBundle()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        CelestialSeeder seeder = new();

        seeder.SeedData(context, BuildAnswers(installGasGiant: true));

        List<Celestial> celestials = context.Celestials.ToList();
        Assert.AreEqual(4, celestials.Count);

        Celestial sun = celestials.Single(x => x.CelestialType == "Sun");
        Celestial moon = celestials.Single(x => x.CelestialType == "PlanetaryMoon");
        Celestial planet = celestials.Single(x => x.CelestialType == "PlanetFromMoon");
        Celestial sunFromMoon = celestials.Single(x => x.CelestialType == "SunFromPlanetaryMoon");

        Assert.AreEqual("GasGiantMoonView", GetPackage(sun));
        Assert.AreEqual("GasGiantMoonView", GetPackage(moon));
        Assert.AreEqual("GasGiantMoonView", GetPackage(planet));
        Assert.AreEqual("GasGiantMoonView", GetPackage(sunFromMoon));

        XElement sunDefinition = GetDefinition(sun);
        XElement moonDefinition = GetDefinition(moon);
        XElement planetDefinition = GetDefinition(planet);
        XElement sunFromMoonDefinition = GetDefinition(sunFromMoon);

        Assert.AreEqual("Sol", sunDefinition.Element("Name")!.Value);
        Assert.AreEqual("Ganymede", moonDefinition.Element("Name")!.Value);
        Assert.AreEqual("Jupiter", planetDefinition.Element("Name")!.Value);
        Assert.AreEqual("Sol", sunFromMoonDefinition.Element("Name")!.Value);
        Assert.AreEqual(moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), planetDefinition.Element("Moon")!.Value);
        Assert.AreEqual(sun.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), planetDefinition.Element("Sun")!.Value);
        Assert.AreEqual(moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), sunFromMoonDefinition.Element("Moon")!.Value);
        Assert.AreEqual(sun.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), sunFromMoonDefinition.Element("Sun")!.Value);
        Assert.AreEqual(
            sunDefinition.Element("Illumination")!.ToString(SaveOptions.DisableFormatting),
            sunFromMoonDefinition.Element("Illumination")!.ToString(SaveOptions.DisableFormatting));

        Assert.AreEqual(0.048775, GetRequiredDouble(sunDefinition, "Orbital/OrbitalEccentricity"), 0.0000001);
        Assert.AreEqual(778547200.0, GetRequiredDouble(sunDefinition, "Orbital/OrbitalSemiMajorAxis"), 0.0001);
        Assert.AreEqual(0.000894416, GetRequiredDouble(sunDefinition, "Orbital/ApparentAngularRadius"), 0.0000001);
        Assert.AreEqual(1070400.0, GetRequiredDouble(moonDefinition, "Orbital/OrbitalSemiMajorAxis"), 0.0001);
        Assert.AreEqual(4.97331570355784, GetRequiredDouble(moonDefinition, "Orbital/SiderealTimeAtEpoch"), 0.0000001);
        Assert.AreEqual(6.28378508344426, GetRequiredDouble(moonDefinition, "Orbital/SiderealTimePerDay"), 0.0000001);
        Assert.AreEqual(0.000894416, GetRequiredDouble(planetDefinition, "SunAngularRadius"), 0.0000001);
    }

    [TestMethod]
    public void ShouldSeedData_PartialPackagesRemain_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        CelestialSeeder seeder = new();

        seeder.SeedData(context, BuildAnswers(installSun: true));

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));
    }

    [TestMethod]
    public void ResolveSunEpochDisplay_GregorianCalendar_ShowsCalendarSpecificExampleAndDefault()
    {
        using FuturemudDatabaseContext context = BuildContext();
        long calendarId = SeedCalendar(context, "gregorian-uk", "2000");

        ConsoleQuestionDisplay display = CelestialSeeder.ResolveSunEpochDisplay(context, new Dictionary<string, string>
        {
            ["suncalendar"] = calendarId.ToString(System.Globalization.CultureInfo.InvariantCulture)
        });

        StringAssert.Contains(display.Prompt, "01/january/year");
        Assert.AreEqual("01/january/2000", display.DefaultAnswer);
    }

    [TestMethod]
    public void ResolveSunEpochDisplay_MiddleEarthCalendar_ShowsCalendarSpecificExampleAndDefault()
    {
        using FuturemudDatabaseContext context = BuildContext();
        long calendarId = SeedCalendar(context, "middle-earth", "3019", "3");

        ConsoleQuestionDisplay display = CelestialSeeder.ResolveSunEpochDisplay(context, new Dictionary<string, string>
        {
            ["suncalendar"] = calendarId.ToString(System.Globalization.CultureInfo.InvariantCulture)
        });

        StringAssert.Contains(display.Prompt, "01/yestare/year");
        Assert.AreEqual("01/yestare/3019", display.DefaultAnswer);
    }

    [TestMethod]
    public void ResolveMoonEpochDefaults_UseStockReferenceDatesForSeededCalendars()
    {
        using FuturemudDatabaseContext gregorianContext = BuildContext();
        long gregorianCalendarId = SeedCalendar(gregorianContext, "gregorian-uk", "2000");
        Assert.AreEqual(
            "21/january/2000",
            CelestialSeeder.ResolveMoonEpochDefault(gregorianContext, new Dictionary<string, string>
            {
                ["mooncalendar"] = gregorianCalendarId.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }));

        using FuturemudDatabaseContext missionContext = BuildContext();
        long missionCalendarId = SeedCalendar(missionContext, "mission", "77");
        Assert.AreEqual(
            "01/ignis/77",
            CelestialSeeder.ResolveGasGiantMoonEpochDefault(missionContext, new Dictionary<string, string>
            {
                ["gasgiantcalendar"] = missionCalendarId.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }));
    }
}
