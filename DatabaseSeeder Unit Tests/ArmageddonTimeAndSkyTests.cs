#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using RuntimeCalendar = MudSharp.TimeAndDate.Date.Calendar;
using RuntimeClock = MudSharp.TimeAndDate.Time.Clock;

namespace DatabaseSeederUnitTests;

[TestClass]
public class ArmageddonTimeAndSkyTests
{
	private const long Day = 540;
	private const long Year = 693 * Day;
	private static string[] _definitions = [];
	private static CompiledAuthoredCelestial[] _sky = [];
	private static readonly Dictionary<string, string> TimeAnswers = new()
	{
		["mode"] = "Armageddon", ["secondsmultiplier"] = "6", ["startyear"] = "1"
	};
	private static readonly Dictionary<string, string> SkyAnswers = new() { ["packages"] = "Armageddon" };

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString())
		.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);

	[ClassInitialize]
	public static void Initialise(TestContext _)
	{
		using var context = Context();
		new TimeSeeder().SeedData(context, TimeAnswers);
		new CelestialSeeder().SeedData(context, SkyAnswers);
		_definitions = context.Celestials.OrderBy(x => x.Id).Select(x => x.Definition).ToArray();
		_sky = _definitions.Select(x => new CompiledAuthoredCelestial(AuthoredCelestialFormat.Parse(x), 60, 60, 9)).ToArray();
	}

	[TestMethod]
	public void Time_InstallsNineHourClockAndCalendarWithAll77LiteralNamesAndCorrectBoundaries()
	{
		using var context = Context();
		var seeder = new TimeSeeder();
		Assert.IsTrue(seeder.SeederQuestions.Single(x => x.Id == "mode").Validator("Armageddon", context).Success);
		seeder.SeedData(context, TimeAnswers);
		var clocks = new All<IClock>();
		var calendars = new All<ICalendar>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Clocks).Returns(clocks);
		world.SetupGet(x => x.Calendars).Returns(calendars);
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var clock = new RuntimeClock(context.Clocks.Include(x => x.Timezones).Single(), world.Object);
		clocks.Add(clock);
		var calendar = new RuntimeCalendar(context.Calendars.Single(), world.Object) { FeedClock = clock };
		Assert.AreEqual(9, clock.HoursPerDay);
		Assert.AreEqual(600.0, clock.SecondsPerMinute * clock.MinutesPerHour / (double)clock.InGameSecondsPerRealSecond);
		for (var hour = 0; hour < 9; hour++)
		{
			Assert.AreEqual(clock.HourIntervalNames[hour], clock.DisplayTime(
				MudTime.CreatePrimaryTime(0, 0, hour, clock.PrimaryTimezone, clock), "$i"));
		}
		Assert.AreEqual(693, calendar.CountDaysInYear(1));
		Assert.AreEqual(11, calendar.Weekdays.Count);
		Assert.AreEqual("Ocandra", calendar.CurrentDate.Weekday);
		Assert.AreEqual("Detal", calendar.GetDate("11/descending/1").Weekday);
		Assert.AreEqual("Ocandra", calendar.GetDate("1/low/1").Weekday);
		Assert.AreEqual("Descending Sun", calendar.CurrentDate.Month.FullName);
		var next = calendar.GetDate("231/ascending/77");
		next.AdvanceDays(1);
		Assert.AreEqual("1/descending/78", next.GetDateString());
		Assert.AreEqual("Jihae's Anger|1|2", calendar.DisplayDate(next, "$yn|$yi|$ya"));
		Assert.AreEqual(0L, MudInstant.FromMudDateTime(calendar.CurrentDateTime).Ticks);

		string[] possessors = ["Jihae", "Drov", "Desert", "Ruk", "Whira", "Dragon", "Vivadu", "King", "Silt", "Suk-Krath", "Lirathu"];
		string[] qualities = ["Anger", "Peace", "Vengeance", "Slumber", "Defiance", "Reverence", "Agitation"];
		Assert.AreEqual(77, calendar.YearCycle!.YearNames.Count);
		for (var year = 1; year <= 77; year++)
		{
			Assert.AreEqual($"{possessors[(year - 1) % 11]}'s {qualities[(year - 1) % 7]}", calendar.YearCycle.GetYear(year).Name);
		}
		StringAssert.Contains(calendar.DisplayDate(CalendarDisplayMode.Long), "Year of Jihae's Anger, Year 1 of King's Age 1");
	}

	[TestMethod]
	[DataRow("startyear", "0")]
	[DataRow("startyear", "-1")]
	[DataRow("startyear", "2147483648")]
	[DataRow("startyear", "4294967295")]
	[DataRow("secondsmultiplier", "0")]
	[DataRow("secondsmultiplier", "-1")]
	[DataRow("secondsmultiplier", "2147483648")]
	[DataRow("secondsmultiplier", "4294967295")]
	public void Time_Questions_RejectValuesThatCannotBeRepresentedByRuntimeIntegers(string question, string value)
	{
		using var context = Context();
		Assert.IsFalse(new TimeSeeder().SeederQuestions.Single(x => x.Id == question).Validator(value, context).Success);
	}

	[TestMethod]
	public void Time_RerunPreservesClockCalendarNamesAndLiveDate()
	{
		using var context = Context();
		var seeder = new TimeSeeder();
		seeder.SeedData(context, TimeAnswers);
		var calendar = context.Calendars.Single();
		var root = XElement.Parse(calendar.Definition);
		root.Element("namedyearcycle")!.Elements("year").First().Value = "Builder's First Year";
		root.Element("alias")!.Value = "Armageddon";
		calendar.Definition = root.ToString();
		calendar.Date = "17/low/80";
		var clock = context.Clocks.Single();
		var clockRoot = XElement.Parse(clock.Definition);
		clockRoot.Element("InGameSecondsPerRealSecond")!.Value = "3";
		clock.Definition = clockRoot.ToString();
		clock.Hours = 4;
		var timezone = context.Timezones.Single();
		timezone.Name = "Builder Time";
		timezone.OffsetHours = 2;
		context.SaveChanges();
		var originalDefinition = calendar.Definition;
		seeder.SeedData(context, TimeAnswers);
		Assert.AreEqual(1, context.Calendars.Count());
		Assert.AreEqual(1, context.Clocks.Count());
		Assert.AreEqual(originalDefinition, calendar.Definition);
		Assert.AreEqual("17/low/80", calendar.Date);
		Assert.AreEqual("3", XElement.Parse(clock.Definition).Element("InGameSecondsPerRealSecond")!.Value);
		Assert.AreEqual(4, clock.Hours);
		Assert.AreEqual("Builder Time", timezone.Name);
		Assert.AreEqual(2, timezone.OffsetHours);
		Assert.AreEqual(timezone.Id, clock.PrimaryTimezoneId);
		Assert.AreEqual(2, context.Timezones.Count());
	}

	[TestMethod]
	public void Sky_InstallsExactlyThreeUnattachedBodiesAndPreservesPartialPackageEdits()
	{
		using var context = Context();
		new TimeSeeder().SeedData(context, TimeAnswers);
		foreach (var source in _definitions)
		{
			var definition = AuthoredCelestialFormat.Parse(source);
			context.Celestials.Add(new Celestial { Definition = source, CelestialType = definition.Kind.ToString(), FeedClockId = context.Clocks.Single().Id });
		}
		context.SaveChanges();
		Assert.AreEqual(3, context.Celestials.Count());
		CollectionAssert.AreEquivalent(new[] { "Suk-Krath", "Jihae", "Lirathu" }, _definitions.Select(x => AuthoredCelestialFormat.Parse(x).Name).ToArray());
		Assert.IsFalse(context.ShardsCelestials.Any());
		var original = context.Celestials.OrderBy(x => x.Id).ToArray();
		var edited = AuthoredCelestialFormat.Parse(original[0].Definition);
		edited.Name = "Builder's Sun";
		original[0].Definition = AuthoredCelestialFormat.Serialize(edited);
		context.Celestials.Remove(original[2]);
		context.SaveChanges();
		var saved = original[0].Definition;
		var seeder = new CelestialSeeder();
		seeder.SeedData(context, SkyAnswers);
		Assert.AreEqual(3, context.Celestials.Count());
		Assert.AreEqual(saved, context.Celestials.Find(original[0].Id)!.Definition);
		Assert.IsNotNull(context.Celestials.Find(original[1].Id));
		seeder.SeedData(context, SkyAnswers);
		Assert.AreEqual(3, context.Celestials.Count());
		Assert.AreEqual("none", CelestialSeeder.ResolvePackagesDefault(context, SkyAnswers));
		CollectionAssert.AreEqual(new[] { "packages" }, seeder.SeederQuestions.Where(x => x.Filter(context, SkyAnswers)).Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void Sky_PrerequisiteValidationAndDirectExecutionRejectIncompatibleCalendarsBeforeMutation()
	{
		using var context = Context();
		var seeder = new CelestialSeeder();
		var question = seeder.SeederQuestions.Single(x => x.Id == "packages");
		Assert.IsFalse(question.Validator("Armageddon", context).Success);
		Assert.IsTrue(question.Validator("EarthSun", context).Success);
		Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedData(context, SkyAnswers));
		Assert.AreEqual(0, context.Celestials.Count());
		new TimeSeeder().SeedData(context, TimeAnswers);
		Assert.IsTrue(question.Validator("Armageddon", context).Success);
		Assert.AreEqual("Armageddon", CelestialSeeder.ResolvePackagesDefault(context, SkyAnswers));
		CollectionAssert.AreEqual(new[] { "packages" }, seeder.SeederQuestions.Where(x => x.Filter(context, SkyAnswers)).Select(x => x.Id).ToArray());
		Assert.IsTrue(question.Validator("EarthSun Armageddon", context).Success);
		var clock = context.Clocks.Single();
		var definition = XElement.Parse(clock.Definition);
		definition.Element("HoursPerDay")!.Value = "24";
		clock.Definition = definition.ToString();
		context.SaveChanges();
		Assert.IsFalse(question.Validator("Armageddon", context).Success);
		Assert.IsTrue(question.Validator("EarthSun", context).Success);
		Assert.ThrowsExactly<InvalidOperationException>(() => seeder.SeedData(context, SkyAnswers));
		Assert.AreEqual(0, context.Celestials.Count());
	}

	[TestMethod]
	public void Sky_CompiledFullYear_HasOnlyNineMidnightConjunctionsAndClosesAcrossNegativeAndPositiveSeams()
	{
		Assert.AreEqual(3, _sky.Length);
		var conjunctions = new List<long>();
		var cursors = new AuthoredCelestialCursor[3];
		for (long minute = 1; minute <= Year; minute++)
		{
			var sun = _sky[0].Evaluate(minute * 60, ref cursors[0]).DirectionVector;
			var jihae = _sky[1].Evaluate(minute * 60, ref cursors[1]).DirectionVector;
			var lirathu = _sky[2].Evaluate(minute * 60, ref cursors[2]).DirectionVector;
			if (Distance(sun, jihae) < 1e-10 && Distance(sun, lirathu) < 1e-10)
			{
				conjunctions.Add(minute / Day);
				Assert.AreEqual(0L, minute % Day);
				Assert.IsTrue(sun.Up < 0);
			}
		}
		CollectionAssert.AreEqual(new long[] { 77, 154, 231, 308, 385, 462, 539, 616, 693 }, conjunctions);
		foreach (var body in _sky)
		{
			Assert.IsTrue(body.Path.StoredEntries < 100000);
			foreach (var minute in new long[] { -Year - 1, -1, 0, 1, Year - 1, Year, Year + 1 })
			{
				Assert.IsTrue(Distance(body.Evaluate(minute * 60).DirectionVector, body.Evaluate((minute + Year) * 60).DirectionVector) < 1e-12);
			}
			var next = body.FindNext(new MudInstant(0), new CelestialEventRequest(null, EventKey: "custom:conjunction"));
			Assert.IsTrue(next.Found);
			Assert.AreEqual(77 * Day * 60, next.Instant.Ticks);
		}
	}

	[TestMethod]
	public void Sky_SolarArcs_All693DaysHaveSevenHourHorizonIntervalAndLowArc()
	{
		var sun = _sky[0];
		Assert.AreEqual(693, sun.Path.RiseOffsets.Count);
		Assert.AreEqual(693, sun.Path.SetOffsets.Count);
		for (var day = 0; day < 693; day++)
		{
			var start = day * Day;
			Assert.AreEqual(start + 60, sun.Path.RiseOffsets[day]);
			Assert.AreEqual(start + 480, sun.Path.SetOffsets[day]);
			Assert.AreEqual(420L, sun.Path.SetOffsets[day] - sun.Path.RiseOffsets[day]);
			Assert.IsTrue(sun.Evaluate((start + 59) * 60).Elevation < 0);
			Assert.IsTrue(sun.Evaluate((start + 61) * 60).Elevation > 0);
			Assert.IsTrue(sun.Evaluate((start + 479) * 60).Elevation > 0);
			Assert.IsTrue(sun.Evaluate((start + 481) * 60).Elevation < 0);
		}
		var high = sun.Evaluate(270 * 60).Elevation;
		var low = sun.Evaluate((231 * Day + 270) * 60).Elevation;
		Assert.IsTrue(high > low);
		Assert.AreEqual(30.0, low * 180 / Math.PI, 1e-8);
		Assert.AreEqual(low, sun.Evaluate((400 * Day + 270) * 60).Elevation, 1e-10);
		Assert.IsTrue(sun.Evaluate((650 * Day + 270) * 60).Elevation > low);
	}

	[TestMethod]
	public void Sky_MoonPhaseBands_MatchHistoricalNewFullAndQuarterDaysThroughoutYear()
	{
		for (var day = 1; day <= 693; day++)
		{
			var sourceDay = (day - 1) % 231 + 1;
			var jihae = _sky[1].Evaluate((day - 1) * Day * 60).Phase!.Value.Name;
			var lirathu = _sky[2].Evaluate((day - 1) * Day * 60).Phase!.Value.Name;
			Assert.AreEqual(sourceDay is >= 106 and <= 109 or >= 222 and <= 225, jihae == MoonPhase.Full, $"Jihae full day {day}");
			Assert.AreEqual(sourceDay is >= 49 and <= 51 or >= 164 and <= 167, jihae == MoonPhase.New, $"Jihae new day {day}");
			Assert.AreEqual(sourceDay is >= 78 and <= 91 or >= 194 and <= 207, jihae == MoonPhase.FirstQuarter, $"Jihae waxing half day {day}");
			Assert.AreEqual(sourceDay is >= 9 and <= 22 or >= 124 and <= 137, jihae == MoonPhase.LastQuarter, $"Jihae waning half day {day}");
			Assert.AreEqual(sourceDay is <= 8 or >= 110 and <= 123 or >= 226, jihae == MoonPhase.WaningGibbous, $"Jihae waning gibbous day {day}");
			Assert.AreEqual(sourceDay is >= 92 and <= 105 or >= 208 and <= 221, jihae == MoonPhase.WaxingGibbous, $"Jihae waxing gibbous day {day}");
			Assert.AreEqual(sourceDay is >= 23 and <= 48 or >= 138 and <= 163, jihae == MoonPhase.WaningCrescent, $"Jihae waning crescent day {day}");
			Assert.AreEqual(sourceDay is >= 52 and <= 77 or >= 168 and <= 193, jihae == MoonPhase.WaxingCrescent, $"Jihae waxing crescent day {day}");
			Assert.AreEqual(sourceDay is >= 115 and <= 118, lirathu == MoonPhase.Full, $"Lirathu full day {day}");
			Assert.AreEqual(sourceDay <= 2 || sourceDay >= 230, lirathu == MoonPhase.New, $"Lirathu new day {day}");
			Assert.AreEqual(sourceDay is >= 60 and <= 86, lirathu == MoonPhase.FirstQuarter, $"Lirathu waxing half day {day}");
			Assert.AreEqual(sourceDay is >= 147 and <= 173, lirathu == MoonPhase.LastQuarter, $"Lirathu waning half day {day}");
			Assert.AreEqual(sourceDay is >= 87 and <= 114, lirathu == MoonPhase.WaxingGibbous, $"Lirathu waxing gibbous day {day}");
			Assert.AreEqual(sourceDay is >= 119 and <= 146, lirathu == MoonPhase.WaningGibbous, $"Lirathu waning gibbous day {day}");
			Assert.AreEqual(sourceDay is >= 3 and <= 59, lirathu == MoonPhase.WaxingCrescent, $"Lirathu waxing crescent day {day}");
			Assert.AreEqual(sourceDay is >= 174 and <= 229, lirathu == MoonPhase.WaningCrescent, $"Lirathu waning crescent day {day}");
			foreach (var body in _sky.Skip(1))
			{
				var phaseAtStart = body.Evaluate((day - 1) * Day * 60).Phase;
				Assert.AreEqual(phaseAtStart, body.Evaluate(((day - 1) * Day + 270) * 60).Phase);
				Assert.AreEqual(phaseAtStart, body.Evaluate((day * Day - 1) * 60).Phase);
			}
		}
	}

	private static double Distance(CelestialVector a, CelestialVector b) =>
		Math.Sqrt(Math.Pow(a.East - b.East, 2) + Math.Pow(a.North - b.North, 2) + Math.Pow(a.Up - b.Up, 2));
}
