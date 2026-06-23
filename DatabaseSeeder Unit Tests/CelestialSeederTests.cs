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
    private static readonly string[] EpochSuggestionCalendarModes =
    [
        "gregorian-us",
        "gregorian-uk",
        "gregorian-us-ce",
        "gregorian-uk-ce",
        "julian",
        "latin-7day",
        "latin-8day",
        "latin-ancient",
        "middle-earth",
        "tranquility",
        "republicain",
        "mission",
        "seasonal-360",
        "islamic-hijri",
        "hebrew",
        "old-persian",
        "babylonian",
        "chinese-minguo",
        "chinese-lunisolar",
        "korean-dangi",
        "korean-modern",
        "korean-lunisolar",
        "japanese-koki",
        "japanese-modern",
        "japanese-lunisolar"
    ];

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
    public void SeederQuestions_MoonAndGasGiantPromptsGiveSupportingContext()
    {
        Dictionary<string, string> questions = new CelestialSeeder().SeederQuestions
            .ToDictionary(x => x.Id, x => x.Question, StringComparer.OrdinalIgnoreCase);

        StringAssert.Contains(questions["installmoon"], "lunar phases");
        StringAssert.Contains(questions["installmoon"], "moon-surface sky views");
        StringAssert.Contains(questions["mooncalendar"], "same calendar used by your Earth-facing sun");
        StringAssert.Contains(questions["moonname"], "The Moon");
        StringAssert.Contains(questions["installgasgiantmoon"], "Jupiter dominating the sky");
        StringAssert.Contains(questions["gasgiantcalendar"], "All four linked objects");
        StringAssert.Contains(questions["gasgiantsunepoch"], "distant solar orbit");
        StringAssert.Contains(questions["gasgiantmoonepoch"], "keeps the linked Jupiter and Sol views coherent");
    }

    [TestMethod]
    public void ResolveEpochDisplays_AllSupportedTimeSeederCalendarModes_ProvideSuggestedDefaults()
    {
        foreach (string mode in EpochSuggestionCalendarModes)
        {
            using FuturemudDatabaseContext context = BuildContext();
            long calendarId = SeedCalendar(context, mode, "1200", mode == "middle-earth" ? "3" : null);
            string calendarAnswer = calendarId.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Dictionary<string, string> answers = new(StringComparer.OrdinalIgnoreCase)
            {
                ["suncalendar"] = calendarAnswer,
                ["mooncalendar"] = calendarAnswer,
                ["gasgiantcalendar"] = calendarAnswer
            };

            ConsoleQuestionDisplay sun = CelestialSeeder.ResolveSunEpochDisplay(context, answers);
            ConsoleQuestionDisplay moon = CelestialSeeder.ResolveMoonEpochDisplay(context, answers);
            ConsoleQuestionDisplay gasGiantSun = CelestialSeeder.ResolveGasGiantSunEpochDisplay(context, answers);
            ConsoleQuestionDisplay gasGiantMoon = CelestialSeeder.ResolveGasGiantMoonEpochDisplay(context, answers);

            Assert.IsFalse(string.IsNullOrWhiteSpace(sun.DefaultAnswer), $"{mode}: sun default missing.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(moon.DefaultAnswer), $"{mode}: moon default missing.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(gasGiantSun.DefaultAnswer), $"{mode}: gas giant sun default missing.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(gasGiantMoon.DefaultAnswer), $"{mode}: gas giant moon default missing.");
            StringAssert.Contains(sun.Prompt, "Selected calendar");
            StringAssert.Contains(moon.Prompt, "21st day of the year");
            StringAssert.Contains(gasGiantMoon.Prompt, "epoch-aligned");
        }
    }

    [TestMethod]
    public void Questions_InitialInstallExposeGuidedDefaultsAndCalendarList()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        Dictionary<string, SeederQuestion> questions = ((IDatabaseSeeder)new CelestialSeeder()).Questions
            .ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

        ConsoleQuestionDisplay installSun = questions["installsun"].ResolveDisplay(context, new Dictionary<string, string>());
        StringAssert.Contains(installSun.Prompt, "#DCelestial Package: Earth-facing Sun#F");
        StringAssert.Contains(installSun.Prompt, "#ARecommended:#F choose #3yes#F");
        Assert.AreEqual("yes", installSun.DefaultAnswer);
        Assert.AreEqual("yes", questions["installsun"].DefaultAnswerResolver!(context, new Dictionary<string, string>()));

        ConsoleQuestionDisplay sunCalendar = questions["suncalendar"].ResolveDisplay(context, new Dictionary<string, string>
        {
            ["installsun"] = "yes"
        });
        StringAssert.Contains(sunCalendar.Prompt, "#BAvailable calendars:#F");
        StringAssert.Contains(sunCalendar.Prompt, "#61#F - #ATest Calendar#F");
        StringAssert.Contains(sunCalendar.Prompt, "current date #21-1-1#F");
        Assert.AreEqual("1", sunCalendar.DefaultAnswer);

        ConsoleQuestionDisplay sunName = questions["sunname"].ResolveDisplay(context, new Dictionary<string, string>
        {
            ["installsun"] = "yes"
        });
        StringAssert.Contains(sunName.Prompt, "#DName: Earth-facing Sun#F");
        Assert.AreEqual("The Sun", sunName.DefaultAnswer);

        ConsoleQuestionDisplay gasGiant = questions["installgasgiantmoon"].ResolveDisplay(context, new Dictionary<string, string>());
        StringAssert.Contains(gasGiant.Prompt, "#DOptional Package: Gas Giant Moon#F");
        StringAssert.Contains(gasGiant.Prompt, "#ARecommended:#F choose #3no#F");
        Assert.AreEqual("no", gasGiant.DefaultAnswer);
    }

    [TestMethod]
    public void Questions_InferCalendarDefaultsFromPreviousCelestialChoices()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        Dictionary<string, SeederQuestion> questions = ((IDatabaseSeeder)new CelestialSeeder()).Questions
            .ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> moonAnswers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["installsun"] = "yes",
            ["suncalendar"] = "test",
            ["installmoon"] = "yes"
        };
        ConsoleQuestionDisplay moonCalendar = questions["mooncalendar"].ResolveDisplay(context, moonAnswers);
        StringAssert.Contains(moonCalendar.Prompt, "Use the same calendar as the Earth-facing Sun");
        Assert.AreEqual("test", moonCalendar.DefaultAnswer);
        Assert.AreEqual("test", questions["mooncalendar"].DefaultAnswerResolver!(context, moonAnswers));

        ConsoleQuestionDisplay moonName = questions["moonname"].ResolveDisplay(context, moonAnswers);
        StringAssert.Contains(moonName.Prompt, "#DName: Earth's Moon#F");
        Assert.AreEqual("The Moon", moonName.DefaultAnswer);

        Dictionary<string, string> gasGiantAnswers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["suncalendar"] = "test",
            ["mooncalendar"] = "1",
            ["installgasgiantmoon"] = "yes"
        };
        ConsoleQuestionDisplay gasGiantCalendar = questions["gasgiantcalendar"].ResolveDisplay(context, gasGiantAnswers);
        StringAssert.Contains(gasGiantCalendar.Prompt, "Reusing your previous celestial calendar");
        Assert.AreEqual("1", gasGiantCalendar.DefaultAnswer);
        Assert.AreEqual("1", questions["gasgiantcalendar"].DefaultAnswerResolver!(context, gasGiantAnswers));
    }

    [TestMethod]
    public void QuestionFilters_SkipInstalledPackagesAndRedundantDetailPrompts()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        CelestialSeeder seeder = new();
        Dictionary<string, SeederQuestion> questions = ((IDatabaseSeeder)seeder).Questions
            .ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

        Assert.IsTrue(questions["installsun"].Filter(context, new Dictionary<string, string>()));
        Assert.IsFalse(questions["installmoon"].Filter(context, new Dictionary<string, string>()));
        Assert.IsTrue(questions["installgasgiantmoon"].Filter(context, new Dictionary<string, string>()));
        Assert.IsTrue(questions["installmoon"].Filter(context, new Dictionary<string, string>
        {
            ["installsun"] = "yes"
        }));

        seeder.SeedData(context, BuildAnswers(installSun: true));

        Assert.IsFalse(questions["installsun"].Filter(context, new Dictionary<string, string>()));
        Assert.IsFalse(questions["suncalendar"].Filter(context, new Dictionary<string, string>
        {
            ["installsun"] = "yes"
        }));
        Assert.IsTrue(questions["installmoon"].Filter(context, new Dictionary<string, string>()));
        Assert.IsTrue(questions["mooncalendar"].Filter(context, new Dictionary<string, string>
        {
            ["installmoon"] = "yes"
        }));

        seeder.SeedData(context, BuildAnswers(installMoon: true));

        Assert.IsFalse(questions["installmoon"].Filter(context, new Dictionary<string, string>()));
        Assert.IsFalse(questions["mooncalendar"].Filter(context, new Dictionary<string, string>
        {
            ["installmoon"] = "yes"
        }));

        seeder.SeedData(context, BuildAnswers(installGasGiant: true));

        Assert.IsFalse(questions["installgasgiantmoon"].Filter(context, new Dictionary<string, string>()));
        Assert.IsFalse(questions["gasgiantcalendar"].Filter(context, new Dictionary<string, string>
        {
            ["installgasgiantmoon"] = "yes"
        }));
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
    public void ResolveMoonEpochDisplay_MiddleEarthCalendar_UsesTwentyFirstDayOfYearAndRichGuidance()
    {
        using FuturemudDatabaseContext context = BuildContext();
        long calendarId = SeedCalendar(context, "middle-earth", "3019", "3");

        ConsoleQuestionDisplay display = CelestialSeeder.ResolveMoonEpochDisplay(context, new Dictionary<string, string>
        {
            ["mooncalendar"] = calendarId.ToString(System.Globalization.CultureInfo.InvariantCulture)
        });

        StringAssert.Contains(display.Prompt, "21st day of the year");
        StringAssert.Contains(display.Prompt, "20/tuile/year");
        Assert.AreEqual("20/tuile/3019", display.DefaultAnswer);
    }

    [TestMethod]
    public void ResolveGasGiantMoonEpochDisplay_MissionCalendar_ShowsCalendarSpecificExampleAndDefault()
    {
        using FuturemudDatabaseContext context = BuildContext();
        long calendarId = SeedCalendar(context, "mission", "77");

        ConsoleQuestionDisplay display = CelestialSeeder.ResolveGasGiantMoonEpochDisplay(context, new Dictionary<string, string>
        {
            ["gasgiantcalendar"] = calendarId.ToString(System.Globalization.CultureInfo.InvariantCulture)
        });

        StringAssert.Contains(display.Prompt, "epoch-aligned");
        StringAssert.Contains(display.Prompt, "01/ignis/year");
        Assert.AreEqual("01/ignis/77", display.DefaultAnswer);
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

        using FuturemudDatabaseContext middleEarthContext = BuildContext();
        long middleEarthCalendarId = SeedCalendar(middleEarthContext, "middle-earth", "3019", "3");
        Assert.AreEqual(
            "20/tuile/3019",
            CelestialSeeder.ResolveMoonEpochDefault(middleEarthContext, new Dictionary<string, string>
            {
                ["mooncalendar"] = middleEarthCalendarId.ToString(System.Globalization.CultureInfo.InvariantCulture)
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
