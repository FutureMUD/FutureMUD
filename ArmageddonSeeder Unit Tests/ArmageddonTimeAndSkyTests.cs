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
	[TestMethod]
	public void Sky_MoonsIlluminateNightButNotBelowTheHorizon()
	{
		foreach (var moon in _sky.Skip(1))
		{
			var brightestNight = 0.0;
			for (long minute = 0; minute < Year; minute += 30)
			{
				var state = moon.Evaluate(minute * 60);
				if (state.Elevation <= 0) Assert.AreEqual(0.0, state.SourceLux);
				if (_sky[0].Evaluate(minute * 60).Elevation < 0)
					brightestNight = Math.Max(brightestNight, state.SourceLux);
			}
			Assert.IsTrue(brightestNight > 5.0, "Each moon should provide meaningful illumination during some nights.");
		}
	}
	[TestMethod]
	[DataRow(50, 0, 0, 75.0)]
	[DataRow(20, 1, 0, 75.0)]
	[DataRow(64, 0, 0, 75.0)]
	[DataRow(50, 0, 1, 0.0)]
	[DataRow(50, 14, 0, 0.0)]
	[DataRow(48, 3, 0, 0.0)]
	[DataRow(1, 3, 134217728, 25.0)]
	public void WorldfileLighting_PreservesDarknessAndWildTerrain(int zone, int sector, int flags, double expected)
	{
		Assert.AreEqual(expected, ArmageddonWorldfileLighting.AddedLight(zone, sector, flags));
	}

	[TestMethod]
	public void WorldfileLighting_AttachesNativeSkyWithoutDuplicateAssignments()
	{
		using var context = Context();
		new TimeSeeder().SeedData(context, TimeAnswers);
		new CelestialSeeder().SeedData(context, SkyAnswers);
		Assert.IsTrue(ArmageddonWorldfileLighting.AttachSky(context, 1));
		Assert.IsTrue(ArmageddonWorldfileLighting.AttachSky(context, 1));
		Assert.AreEqual(3, context.ShardsCelestials.Count(x => x.ShardId == 1));
		context.ShardsCelestials.Add(new ShardsCelestials { ShardId = 2, CelestialId = 999 });
		context.SaveChanges();
		Assert.Throws<InvalidOperationException>(() => ArmageddonWorldfileLighting.AttachSky(context, 2));
	}
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
	public void Time_InstalledClockAndCalendarLoadAndRenderRuntimeHours()
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
		for (var hour = 0; hour < clock.HoursPerDay; hour++)
		{
			Assert.AreEqual(clock.HourIntervalNames[hour], clock.DisplayTime(
				MudTime.CreatePrimaryTime(0, 0, hour, clock.PrimaryTimezone, clock), "$i"));
		}
		Assert.IsTrue(calendar.CountDaysInYear(1) > 0);
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
	public void Sky_CompiledPathsCloseAcrossSignedCycleSeamsAndSupportEventLookup()
	{
		foreach (var body in _sky)
		{
			Assert.IsTrue(body.Path.StoredEntries < 100000);
			foreach (var minute in new long[] { -Year - 1, -1, 0, 1, Year - 1, Year, Year + 1 })
			{
				Assert.IsTrue(Distance(body.Evaluate(minute * 60).DirectionVector, body.Evaluate((minute + Year) * 60).DirectionVector) < 1e-12);
			}
			var next = body.FindNext(new MudInstant(0), new CelestialEventRequest(null, EventKey: "custom:conjunction"));
			Assert.IsTrue(next.Found);
		}
	}

	private static double Distance(CelestialVector a, CelestialVector b) =>
		Math.Sqrt(Math.Pow(a.East - b.East, 2) + Math.Pow(a.North - b.North, 2) + Math.Pow(a.Up - b.Up, 2));
}
