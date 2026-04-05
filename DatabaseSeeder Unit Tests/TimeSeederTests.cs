#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TimeSeederTests
{
    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
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
}
