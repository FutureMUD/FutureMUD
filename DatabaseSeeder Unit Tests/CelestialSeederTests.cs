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
            ["packages"] = string.Join(" ", new[] { installSun ? "EarthSun" : "", installMoon ? "EarthMoon" : "", installGasGiant ? "Jupiter" : "" }.Where(x => x.Length > 0)),
            ["suncalendar"] = "1",
            ["sunname"] = "The Sun",
            ["sunepoch"] = "1-jan-2000",
            ["mooncalendar"] = "1",
            ["moonname"] = "The Moon",
            ["moonepoch"] = "21-jan-2000",
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

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() =>
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

        StringAssert.Contains(questions["packages"], "lunar phases");
        StringAssert.Contains(questions["packages"], "moon-surface sky views");
        StringAssert.Contains(questions["mooncalendar"], "same calendar used by your Earth-facing sun");
        StringAssert.Contains(questions["packages"], "Jupiter dominating the sky");
        StringAssert.Contains(questions["gasgiantcalendar"], "All four linked objects");
        StringAssert.Contains(questions["gasgiantsunepoch"], "distant solar orbit");
        StringAssert.Contains(questions["gasgiantmoonepoch"], "keeps the linked Jupiter and Sol views coherent");
    }

    [TestMethod]
    public void ResolveEpochDisplays_AllSupportedTimeSeederCalendarModes_ProvideSuggestedDefaults()
    {
		Dictionary<string, SeederQuestion> questions = ((IDatabaseSeeder)new CelestialSeeder()).Questions
			.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);
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
			Assert.IsTrue(questions["sunepoch"].Validator(sun.DefaultAnswer!, context).Success,
				$"{mode}: sun default should pass its prompt validator.");
			Assert.IsTrue(questions["moonepoch"].Validator(moon.DefaultAnswer!, context).Success,
				$"{mode}: moon default should pass its prompt validator.");
			Assert.IsTrue(questions["gasgiantsunepoch"].Validator(gasGiantSun.DefaultAnswer!, context).Success,
				$"{mode}: gas giant sun default should pass its prompt validator.");
			Assert.IsTrue(questions["gasgiantmoonepoch"].Validator(gasGiantMoon.DefaultAnswer!, context).Success,
				$"{mode}: gas giant moon default should pass its prompt validator.");
            StringAssert.Contains(sun.Prompt, "Selected calendar");
            StringAssert.Contains(moon.Prompt, "21st day of the year");
            StringAssert.Contains(gasGiantMoon.Prompt, "epoch-aligned");
        }
    }

    [TestMethod]
    public void Questions_DefaultSelectionOnlyAsksForEarthEpochs()
    {
        using var context = BuildContext();
        SeedPrerequisites(context);
        var questions = ((IDatabaseSeeder)new CelestialSeeder()).Questions.ToList();
        var answers = new Dictionary<string, string> { ["packages"] = "EarthSun EarthMoon" };
        CollectionAssert.AreEqual(new[] { "packages", "sunepoch", "moonepoch" },
            questions.Where(x => x.Filter(context, answers)).Select(x => x.Id).ToArray());
        var display = questions[0].ResolveDisplay(context, answers);
        Assert.AreEqual("EarthSun EarthMoon", display.DefaultAnswer);
        StringAssert.Contains(display.Prompt, "Rails");
        new CelestialSeeder().SeedData(context, new Dictionary<string, string>(answers)
        {
            ["sunepoch"] = "01/jan/2000", ["moonepoch"] = "28/jan/2000"
        });
        Assert.AreEqual(4, context.Celestials.Count());
        CollectionAssert.AreEqual(new[] { "packages" },
            questions.Where(x => x.Filter(context, answers)).Select(x => x.Id).ToArray());
        Assert.AreEqual("none", questions[0].ResolveDisplay(context, answers).DefaultAnswer);
    }

    [TestMethod]
    public void Questions_SoleCalendarProvidesEpochDefaultsWithoutCalendarAnswers()
    {
        using var context = BuildContext();
        SeedCalendar(context, "gregorian-uk", "2000");
        var answers = new Dictionary<string, string> { ["packages"] = "EarthSun EarthMoon" };
        Assert.AreEqual("01/january/2000", CelestialSeeder.ResolveSunEpochDisplay(context, answers).DefaultAnswer);
        Assert.AreEqual("21/january/2000", CelestialSeeder.ResolveMoonEpochDisplay(context, answers).DefaultAnswer);
        Assert.AreEqual("01/january/2000", CelestialSeeder.ResolveSunEpochDefault(context, answers));
        Assert.AreEqual("21/january/2000", CelestialSeeder.ResolveMoonEpochDefault(context, answers));
    }

    [TestMethod]
    public void ShouldSeedData_OnlyRailsInstalledOffersAdditionalPackages()
    {
        using var context = BuildContext();
        SeedPrerequisites(context);
        context.Clocks.Single().Definition = "<Clock><SecondsPerMinute>60</SecondsPerMinute><MinutesPerHour>60</MinutesPerHour><HoursPerDay>24</HoursPerDay></Clock>";
        context.SaveChanges();
        var seeder = new CelestialSeeder();
        seeder.SeedData(context, new Dictionary<string, string> { ["packages"] = "Rails" });
        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));
    }

    [TestMethod]
    public void Questions_MultipleCalendarsAskOnlyForSelectedPackages()
    {
        using var context = BuildContext();
        SeedPrerequisites(context);
        context.Calendars.Add(new Calendar { Id = 2, Definition = "<calendar />", Date = "1-1-1", FeedClockId = 1 });
        context.SaveChanges();
        var questions = ((IDatabaseSeeder)new CelestialSeeder()).Questions.ToList();
        var answers = new Dictionary<string, string> { ["packages"] = "jUpItEr  Rails" };
        CollectionAssert.AreEqual(new[] { "packages", "gasgiantcalendar", "gasgiantsunepoch", "gasgiantmoonepoch", "authoredcalendar" },
            questions.Where(x => x.Filter(context, answers)).Select(x => x.Id).ToArray());
        Assert.IsTrue(questions[0].Validator(answers["packages"], context).Success);
        answers["packages"] = "EarthSun EarthMoon";
        CollectionAssert.AreEqual(new[] { "packages", "suncalendar", "sunepoch", "mooncalendar", "moonepoch" },
            questions.Where(x => x.Filter(context, answers)).Select(x => x.Id).ToArray());
    }

    [TestMethod]
    public void Questions_ValidateSelectionsAndMoonDependency()
    {
        using var context = BuildContext();
        SeedPrerequisites(context);
        var question = ((IDatabaseSeeder)new CelestialSeeder()).Questions.First();
        foreach (var invalid in new[] { "", "   ", "EarthSun typo", "none EarthSun", "EarthMoon" })
            Assert.IsFalse(question.Validator(invalid, context).Success, invalid);
        foreach (var valid in new[] { "none", "EarthSun EarthMoon", "EarthMoon EarthSun", " Jupiter\tRails  Scripts MorningStar MorningGlow ", "EarthSun EarthSun" })
            Assert.IsTrue(question.Validator(valid, context).Success, valid);
        new CelestialSeeder().SeedData(context, BuildAnswers(installSun: true));
        Assert.IsTrue(question.Validator("EarthMoon", context).Success);
        var answers = new Dictionary<string, string> { ["packages"] = "EarthMoon" };
        CollectionAssert.AreEqual(new[] { "packages", "moonepoch" },
            ((IDatabaseSeeder)new CelestialSeeder()).Questions.Where(x => x.Filter(context, answers)).Select(x => x.Id).ToArray());
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
