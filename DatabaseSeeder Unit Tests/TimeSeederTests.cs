#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RuntimeCalendar = MudSharp.TimeAndDate.Date.Calendar;
using RuntimeClock = MudSharp.TimeAndDate.Time.Clock;
using RuntimeICalendar = MudSharp.TimeAndDate.Date.ICalendar;
using RuntimeIClock = MudSharp.TimeAndDate.Time.IClock;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TimeSeederTests
{
    private static readonly string[] Modes =
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
        "republicain",
        "tranquility",
        "mission",
        "seasonal-360"
    ];

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    [TestMethod]
    public void SeedData_AllModes_CreateLoadableClockAndCalendarContent()
    {
        foreach (string mode in Modes)
        {
            using FuturemudDatabaseContext context = BuildContext();
            TimeSeeder seeder = new();

            seeder.SeedData(context, Answers(mode));

            Assert.IsTrue(context.Clocks.Any(), $"{mode}: no clocks were seeded.");
            Assert.IsTrue(context.Timezones.Any(), $"{mode}: no timezones were seeded.");
            Assert.IsTrue(context.Calendars.Any(), $"{mode}: no calendars were seeded.");

            foreach (Clock clock in context.Clocks)
            {
                XElement definition = XElement.Parse(clock.Definition);
                AssertHasValue(definition, "Alias", mode);
                AssertHasValue(definition, "ShortDisplayString", mode);
                AssertHasValue(definition, "LongDisplayString", mode);
                AssertHasValue(definition, "SuperDisplayString", mode);
                Assert.IsTrue(definition.Element("CrudeTimeIntervals")!.Elements("CrudeTimeInterval").Any(),
                    $"{mode}: clock {clock.Id} has no crude time intervals.");
            }

            foreach (Calendar calendar in context.Calendars)
            {
                XElement definition = XElement.Parse(calendar.Definition);
                AssertHasValue(definition, "alias", mode);
                AssertHasValue(definition, "shortstring", mode);
                AssertHasValue(definition, "longstring", mode);
                AssertHasValue(definition, "wordystring", mode);
                Assert.IsTrue(definition.Element("weekdays")!.Elements("weekday").Any(),
                    $"{mode}: calendar {calendar.Id} has no weekdays.");
                Assert.IsTrue(definition.Element("months")!.Elements("month").Any() ||
                              definition.Element("intercalarymonths")!.Elements("intercalarymonth").Any(),
                    $"{mode}: calendar {calendar.Id} has no months.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(calendar.Date), $"{mode}: calendar {calendar.Id} has no current date.");
            }

            AssertRuntimeLoadable(context, mode);
        }
    }

    [TestMethod]
    public void SeedData_AllModes_HaveExpectedCalendarShapes()
    {
        foreach (string mode in Modes)
        {
            using FuturemudDatabaseContext context = BuildContext();
            TimeSeeder seeder = new();

            seeder.SeedData(context, Answers(mode));

            List<XElement> calendars = context.Calendars
                .Select(x => XElement.Parse(x.Definition))
                .ToList();
            List<XElement> clocks = context.Clocks
                .Select(x => XElement.Parse(x.Definition))
                .ToList();

            switch (mode)
            {
                case "latin-8day":
                    Assert.IsTrue(calendars.Any(x => x.Element("weekdays")!.Elements("weekday").Count() == 8),
                        $"{mode}: expected an eight-day week.");
                    break;
                case "mission":
                case "seasonal-360":
                    Assert.IsTrue(calendars.Any(x => x.Element("weekdays")!.Elements("weekday").Count() == 6),
                        $"{mode}: expected a six-day week.");
                    break;
                case "middle-earth":
                    Assert.IsTrue(calendars.Count > 1, $"{mode}: expected multiple Tolkien calendars.");
                    break;
                case "republicain":
                    Assert.IsTrue(clocks.Any(x => x.Element("HoursPerDay")?.Value == "10" &&
                                                  x.Element("SecondsPerMinute")?.Value == "100"),
                        $"{mode}: expected the decimal clock.");
                    break;
            }
        }
    }

    [TestMethod]
    public void SeedData_MissionCalendar_RerunIsIdempotent()
    {
        using FuturemudDatabaseContext context = BuildContext();
        TimeSeeder seeder = new();
        Dictionary<string, string> answers = Answers("mission");

        seeder.SeedData(context, answers);
        int clockCount = context.Clocks.Count();
        int timezoneCount = context.Timezones.Count();
        int calendarCount = context.Calendars.Count();

        seeder.SeedData(context, answers);

        Assert.AreEqual(clockCount, context.Clocks.Count());
        Assert.AreEqual(timezoneCount, context.Timezones.Count());
        Assert.AreEqual(calendarCount, context.Calendars.Count());
    }

    [TestMethod]
    public void SeedData_Seasonal360Calendar_CreatesExpectedCalendar()
    {
        using FuturemudDatabaseContext context = BuildContext();
        TimeSeeder seeder = new();

        seeder.SeedData(context, new System.Collections.Generic.Dictionary<string, string>
        {
            ["secondsmultiplier"] = "2",
            ["mode"] = "seasonal-360",
            ["startyear"] = "1200"
        });

        Calendar calendar = context.Calendars.Single();
        Assert.AreEqual("01/early-winter/1200", calendar.Date);

        XElement definition = XElement.Parse(calendar.Definition);
        Assert.AreEqual("seasonal-360", definition.Element("alias")?.Value);
        Assert.AreEqual("1", definition.Element("weekdayatepoch")?.Value);

        List<string> weekdays = definition.Element("weekdays")!.Elements("weekday").Select(x => x.Value).ToList();
        CollectionAssert.AreEqual(
            new[]
            {
                "First Day",
                "Second Day",
                "Third Day",
                "Fourth Day",
                "Fifth Day",
                "Sixth Day"
            },
            weekdays);

        List<XElement> months = definition.Element("months")!.Elements("month").ToList();
        Assert.AreEqual(12, months.Count);
        Assert.AreEqual("early-winter", months.First().Element("alias")?.Value);
        Assert.AreEqual("late-autumn", months.Last().Element("alias")?.Value);
        Assert.IsTrue(months.All(x => x.Element("normaldays")?.Value == "30"));
        Assert.IsTrue(months.All(x => !x.Element("intercalarydays")!.Elements().Any()));
        Assert.IsFalse(definition.Element("intercalarymonths")!.Elements().Any());
    }

    private static Dictionary<string, string> Answers(string mode)
    {
        return new Dictionary<string, string>
        {
            ["secondsmultiplier"] = "2",
            ["mode"] = mode,
            ["startyear"] = "1200",
            ["ardaage"] = "3"
        };
    }

    private static void AssertHasValue(XElement definition, string elementName, string mode)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(definition.Element(elementName)?.Value),
            $"{mode}: missing or empty {elementName}.");
    }

    private static void AssertRuntimeLoadable(FuturemudDatabaseContext context, string mode)
    {
        All<RuntimeIClock> clocks = new();
        All<RuntimeICalendar> calendars = new();
        Mock<ISaveManager> saveManager = new();
        saveManager.Setup(x => x.Add(It.IsAny<ISaveable>()));
        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.Clocks).Returns(clocks);
        gameworld.SetupGet(x => x.Calendars).Returns(calendars);
        gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);

        foreach (Clock clockModel in context.Clocks.Include(x => x.Timezones).OrderBy(x => x.Id))
        {
            RuntimeClock clock = new(clockModel, gameworld.Object);
            Assert.IsNotNull(clock.PrimaryTimezone, $"{mode}: runtime clock {clockModel.Id} did not load a primary timezone.");
            Assert.IsNotNull(clock.CurrentTime, $"{mode}: runtime clock {clockModel.Id} did not load a current time.");
            clocks.Add(clock);
        }

        foreach (Calendar calendarModel in context.Calendars.OrderBy(x => x.Id))
        {
            RuntimeCalendar calendar = new(calendarModel, gameworld.Object);
            calendar.FeedClock = clocks.Get(calendar.ClockID);
            Assert.IsNotNull(calendar.FeedClock, $"{mode}: runtime calendar {calendarModel.Id} did not resolve its feed clock.");
            Assert.IsNotNull(calendar.CurrentDate, $"{mode}: runtime calendar {calendarModel.Id} did not load a current date.");
            Assert.IsTrue(calendar.Weekdays.Any(), $"{mode}: runtime calendar {calendarModel.Id} did not load weekdays.");
            Assert.IsTrue(calendar.CreateYear(calendar.CurrentDate.Year).Months.Any(),
                $"{mode}: runtime calendar {calendarModel.Id} could not generate the current year.");
            calendars.Add(calendar);
        }
    }
}
