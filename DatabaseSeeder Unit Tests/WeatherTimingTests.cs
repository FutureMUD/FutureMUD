#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.Json.Nodes;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeederUnitTests;

[TestClass]
public class WeatherTimingTests
{
	[TestMethod]
	[DataRow(24, 60, 365.0)]
	[DataRow(9, 60, 693.0)]
	[DataRow(10, 100, 360.0)]
	[DataRow(10, 37, 365.25)]
	public void NativeTiming_ResamplesPeriodicCurveAndSeasonBoundaries(int hours, int minutes, double year)
	{
		var timing = new SeederWeatherTiming(1, hours, minutes, year);
		timing.Validate();
		var values = timing.Temperatures(10, 30).Elements().ToArray();
		Assert.AreEqual(hours, values.Length);
		for (var hour = 0; hour < hours; hour++)
		{
			var x = 24.0 * hour / hours;
			var index = (int)x;
			var factor = SeederWeatherTiming.ConventionalHeating[index] * (1 - (x - index)) +
				SeederWeatherTiming.ConventionalHeating[(index + 1) % 24] * (x - index);
			Assert.AreEqual(hour, (int)values[hour].Attribute("hour")!);
			Assert.AreEqual(10 + 20 * factor, (double)values[hour], 1e-12);
			if (hours == 24) Assert.AreEqual(10 + 20 * SeederWeatherTiming.ConventionalHeating[hour], (double)values[hour]);
		}
		CollectionAssert.AreEqual(SeederWeatherTiming.ConventionalOnsets.Select(x => (int)Math.Round(x * year / 365, MidpointRounding.AwayFromZero)).ToArray(),
			SeederWeatherTiming.ConventionalOnsets.Select(timing.ScaleOnset).ToArray());
		Assert.AreEqual(4 * hours * minutes, timing.DaysToMinutes(4));
	}

	[TestMethod]
	[DataRow(0, 60, 365.0)]
	[DataRow(9, 0, 693.0)]
	[DataRow(24, 60, 0.0)]
	[DataRow(24, 60, 10.0)]
	[DataRow(24, 60, double.NaN)]
	[DataRow(24, 60, double.PositiveInfinity)]
	[DataRow(24, 60, 3000000000.0)]
	[DataRow(1000000, 1000000, 693.0)]
	public void InvalidOrUnrepresentableTiming_IsRejected(int hours, int minutes, double year) =>
		Assert.ThrowsExactly<InvalidOperationException>(() => new SeederWeatherTiming(1, hours, minutes, year).Validate());

	[TestMethod]
	public void SeasonOnsets_UseAwayFromZeroMidpoints() => Assert.AreEqual(31, new SeederWeatherTiming(1, 24, 60, 182.5).ScaleOnset(61));

	[TestMethod]
	public void PhysicalSunTiming_ReadsTheActualCelestialSeederDefinition()
	{
		using var db = Context();
		new TimeSeeder().SeedData(db, new Dictionary<string, string> { ["mode"] = "gregorian-uk", ["secondsmultiplier"] = "2", ["startyear"] = "2000" });
		new CelestialSeeder().SeedData(db, new Dictionary<string, string>
		{
			["installsun"] = "yes", ["suncalendar"] = "1", ["sunname"] = "The Sun", ["sunepoch"] = "1/jan/2000"
		});
		var timing = SeederWeatherTiming.Resolve(db, new Dictionary<string, string>());
		Assert.AreEqual(24, timing.HoursPerDay);
		Assert.AreEqual(60, timing.MinutesPerHour);
		Assert.AreEqual(365.24, timing.YearDays);
	}

	[TestMethod]
	public void AuthoredMoon_EligibleOnlyWhenExplicitlyGoverningTimeOfDay()
	{
		using var db = Context();
		new TimeSeeder().SeedData(db, new Dictionary<string, string> { ["mode"] = "Armageddon", ["secondsmultiplier"] = "6", ["startyear"] = "1" });
		new CelestialSeeder().SeedData(db, new Dictionary<string, string> { ["installarmageddon"] = "yes" });
		var moon = db.Celestials.First(x => x.CelestialType == "ScriptedMoon");
		db.Celestials.RemoveRange(db.Celestials.Where(x => x.Id != moon.Id)); db.SaveChanges();
		Assert.AreEqual(0, SeederWeatherTiming.Eligible(db).Count);
		var source = JsonNode.Parse(moon.Definition)!; source["TimeOfDay"] = true; moon.Definition = source.ToJsonString(); db.SaveChanges();
		var timing = SeederWeatherTiming.Resolve(db, new Dictionary<string, string>());
		Assert.AreEqual(moon.Id, timing.CelestialId);
		Assert.AreEqual(693.0, timing.YearDays);
		Assert.AreEqual(9, timing.HoursPerDay);
	}

	[TestMethod]
	public void Selection_RequiresExplicitChoiceWithMultipleSuns_AndRejectsInvalidBeforeWrites()
	{
		using var db = Context();
		var answers = new Dictionary<string, string> { ["operation"] = "install", ["rain"] = "none" };
		Assert.ThrowsExactly<InvalidOperationException>(() => new WeatherSeeder().SeedData(db, answers));
		Assert.AreEqual(0, db.Seasons.Count());
		Assert.AreEqual(0, db.WeatherEvents.Count());
		AddTime(db, 9, 60, 693);
		Assert.AreEqual(1L, SeederWeatherTiming.Resolve(db, answers).CelestialId);
		db.Celestials.Add(new Celestial { Id = 2, FeedClockId = 1, CelestialType = "Sun", Definition = "<SunV2><Calendar>1</Calendar><Orbital><CelestialDaysPerYear>365.25</CelestialDaysPerYear></Orbital></SunV2>" });
		db.SaveChanges();
		Assert.IsTrue(new WeatherSeeder().SeederQuestions.Single(x => x.Id == "weathercelestial").Filter(db, answers));
		Assert.ThrowsExactly<InvalidOperationException>(() => new WeatherSeeder().SeedData(db, answers));
		Assert.AreEqual(0, db.Seasons.Count());
		answers["weathercelestial"] = "2";
		Assert.AreEqual(365.25, SeederWeatherTiming.Resolve(db, answers).YearDays);
		answers["weathercelestial"] = "999";
		Assert.ThrowsExactly<InvalidOperationException>(() => new WeatherSeeder().SeedData(db, answers));
		Assert.AreEqual(0, db.WeatherEvents.Count());
	}

	[TestMethod]
	public void InvalidTiming_SharedExecutorFailsWithoutRecordingAnswers()
	{
		using var db = Context();
		var seeder = new WeatherSeeder();
		var result = SeederExecutionService.Execute(db, seeder, SeederQuestionRegistry.GetQuestions(seeder, seeder.SeederQuestions),
			new Dictionary<string, string> { ["operation"] = "install", ["rain"] = "none" }, new Version(1, 0));
		Assert.IsFalse(result.Success);
		Assert.IsInstanceOfType<InvalidOperationException>(result.Exception);
		Assert.AreEqual(0, db.SeederChoices.Count());
		Assert.AreEqual(0, db.WeatherEvents.Count());
		Assert.AreEqual(0, db.Seasons.Count());
		Assert.IsFalse(db.ChangeTracker.HasChanges());
	}

	[TestMethod]
	public void FreshInstall_NativeTablesAndPeriods_AreNotRetimedByReinstallOrHazards()
	{
		using var db = Context();
		AddTime(db, 9, 37, 693);
		db.Gases.Add(new Gas { Name = "air" });
		db.SaveChanges();
		var seeder = new WeatherSeeder();
		var answers = new Dictionary<string, string> { ["operation"] = "install", ["rain"] = "none" };
		Assert.AreEqual("", seeder.SeedData(db, answers));
		Assert.IsTrue(db.RegionalClimatesSeasons.AsEnumerable().All(x => XElement.Parse(x.TemperatureInfo).Elements().Count() == 9));
		Assert.AreEqual(116, db.Seasons.Single(x => x.Name == "temperate_early_spring").CelestialDayOnset, "61/365 of 693 rounds to 116.");
		Assert.IsTrue(db.RegionalClimates.All(x => x.TemperatureFluctuationPeriodMinutes < 8 * 9 * 37 + 1));
		var season = db.Seasons.First(); season.CelestialDayOnset = 13;
		var temperature = db.RegionalClimatesSeasons.First(); temperature.TemperatureInfo = "<Temperatures><Value hour=\"0\">123</Value></Temperatures>";
		var period = db.RegionalClimates.First(); period.TemperatureFluctuationPeriodMinutes = 123;
		db.Clocks.Single().Definition = "<Clock><HoursPerDay>24</HoursPerDay><MinutesPerHour>60</MinutesPerHour><SecondsPerMinute>60</SecondsPerMinute></Clock>";
		db.SaveChanges();
		Assert.IsTrue(seeder.SeedData(db, answers).Contains("Weather already exists"));
		Assert.AreEqual("", seeder.SeedData(db, new Dictionary<string, string> { ["operation"] = "hazards" }));
		Assert.AreEqual(13, season.CelestialDayOnset);
		Assert.AreEqual("<Temperatures><Value hour=\"0\">123</Value></Temperatures>", temperature.TemperatureInfo);
		Assert.AreEqual(123, period.TemperatureFluctuationPeriodMinutes);
	}

	internal static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
	internal static void AddTime(FuturemudDatabaseContext db, int hours, int minutes, double year)
	{
		db.Calendars.Add(new Calendar { Id = 1, FeedClockId = 1, Definition = "<Calendar />", Date = "1/test/1" });
		db.Clocks.Add(new Clock { Id = 1, Definition = $"<Clock><HoursPerDay>{hours}</HoursPerDay><MinutesPerHour>{minutes}</MinutesPerHour><SecondsPerMinute>60</SecondsPerMinute></Clock>" });
		db.Celestials.Add(new Celestial { Id = 1, FeedClockId = 1, CelestialType = "Sun", Definition = new XElement("SunV2", new XElement("Calendar", 1), new XElement("Orbital", new XElement("CelestialDaysPerYear", year))).ToString() });
		db.SaveChanges();
	}
}
