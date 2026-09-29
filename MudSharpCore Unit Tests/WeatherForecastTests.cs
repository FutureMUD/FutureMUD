#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class WeatherForecastTests
{
	[TestMethod]
	public void Forecast_OversizedClock_ContinuesLiveWeatherAndRejectsOversizedHorizon()
	{
		using var fixture = new Fixture(minutes: 10_000, interval: 1);
		Assert.ThrowsException<InvalidOperationException>(() => fixture.Controller.GetForecast());
		fixture.Tick(fixture.Controller);
		Assert.AreEqual(1L, fixture.Controller.ForecastMinute);
		Assert.AreEqual(0, fixture.Controller.MinuteCounter);
		fixture.Controller.Save();
		fixture.Context.SaveChanges();
		using (var saved = System.Text.Json.JsonDocument.Parse(fixture.Model.ForecastState!))
			Assert.AreEqual(0, saved.RootElement.GetProperty("Points").GetArrayLength());
		var actor = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<MudSharp.PerceptionEngine.IOutputHandler>());
		Assert.IsFalse(fixture.Controller.BuildingCommand(actor, new StringStack("forecast 30")));
		Assert.AreEqual(7, fixture.Controller.ForecastHorizonDays);
		Assert.IsTrue(fixture.Controller.BuildingCommand(actor, new StringStack("forecast 1")));
		Assert.AreEqual(1, fixture.Controller.ForecastHorizonDays);
	}

	[TestMethod]
	public void Forecast_CalendarDateEdit_ReanchorsDailyKeyAndDiscardsOldFuture()
	{
		using var fixture = new Fixture(hours: 24, minutes: 60);
		var context = CelestialTestFactory.CreateEarthSystem();
		context.SetDateTime("1/jan/2010", 9, 0, 0);
		fixture.World.SetupGet(x => x.Clocks).Returns(new All<IClock> { context.Clock });
		fixture.World.SetupGet(x => x.Calendars).Returns(new All<ICalendar> { context.Calendar });
		fixture.Model.FeedClockId = context.Clock.Id;
		fixture.Model.FeedClockTimeZoneId = context.Clock.PrimaryTimezone.Id;
		var controller = new WeatherController(fixture.Model, fixture.World.Object);
		Mock.Get(context.Gameworld).SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController> { controller });
		var day = controller.ForecastDay;
		var oldFuture = controller.GetForecast().ToArray();
		context.Calendar.SetDate("2/jan/2010");
		Assert.AreEqual(day + 1, controller.ForecastDay);
		Assert.AreEqual(context.Calendar.CurrentInstant.Ticks / context.Clock.SecondsPerMinute, controller.ForecastMinute);
		controller.Save();
		fixture.Context.SaveChanges();
		using (var saved = System.Text.Json.JsonDocument.Parse(fixture.Model.ForecastState!))
			Assert.AreEqual(0, saved.RootElement.GetProperty("Points").GetArrayLength(), "The old future must be discarded before the next observation.");
		var nextFuture = controller.GetForecast().ToArray();
		Assert.AreEqual(oldFuture[0].Minute + 1440, nextFuture[0].Minute);
		controller.Save();
		fixture.Context.SaveChanges();
		var resumed = new WeatherController(fixture.Model, fixture.World.Object);
		CollectionAssert.AreEqual(nextFuture, resumed.GetForecast().ToArray());
	}

	[TestMethod]
	public void Forecast_MadeBetweenMinutes_MatchesAstronomyAtActualTickBoundaries()
	{
		using var fixture = new Fixture(hours: 24, minutes: 60, interval: 1);
		var context = CelestialTestFactory.CreateEarthSystem();
		context.SetDateTime("1/jan/2010", 5, 30, 59);
		fixture.World.SetupGet(x => x.Clocks).Returns(new All<IClock> { context.Clock });
		fixture.World.SetupGet(x => x.Calendars).Returns(new All<ICalendar> { context.Calendar });
		// The XML-only sun fixture has not received a database ID yet.
		var celestials = new Mock<IUneditableAll<ICelestialObject>>();
		celestials.Setup(x => x.Get(1L)).Returns(context.Sun);
		fixture.World.SetupGet(x => x.CelestialObjects).Returns(celestials.Object);
		fixture.Model.FeedClockId = context.Clock.Id;
		fixture.Model.FeedClockTimeZoneId = context.Clock.PrimaryTimezone.Id;
		fixture.Model.CelestialId = 1;
		var controller = new WeatherController(fixture.Model, fixture.World.Object);
		Assert.AreSame(context.Sun, controller.Celestial);
		Assert.AreSame(context.Clock, controller.FeedClock);
		var points = controller.GetForecast().Skip(1).Take(1440).ToArray();
		Assert.AreEqual(WeatherAstronomy.Sample(context.Sun, 0.5, controller.GeographyForTimeOfDay).Time, points[0].TimeOfDay,
			$"Forecast origin {controller.ForecastMinute}, first {points[0].Minute}, clock {context.Clock.CurrentTime.GetTimeString()}");
		context.Clock.CurrentTime.AddSeconds(1);
		foreach (var point in points)
		{
			while (controller.ForecastMinute < point.Minute) context.Clock.CurrentTime.AddSeconds(60);
			Assert.AreEqual(context.Sun.CurrentTimeOfDay(controller.GeographyForTimeOfDay), point.TimeOfDay, $"Minute {point.Minute}");
		}
	}

	[TestMethod]
	public void Forecast_ReducedIntervalBelowAccumulatedCounter_SchedulesNextMinuteWithoutDuplicate()
	{
		using var fixture = new Fixture();
		fixture.Tick(fixture.Controller);
		fixture.Tick(fixture.Controller);
		Mock.Get(fixture.Controller.RegionalClimate.ClimateModel).SetupGet(x => x.MinuteProcessingInterval).Returns(1);
		var expected = fixture.Controller.GetForecast().ToArray();
		Assert.AreEqual(fixture.Controller.ForecastMinute + 1, expected[1].Minute);
		fixture.Controller.Save();
		fixture.Context.SaveChanges();
		var resumed = new WeatherController(fixture.Model, fixture.World.Object);
		CollectionAssert.AreEqual(expected, resumed.GetForecast().ToArray());
		fixture.Tick(resumed);
		Assert.AreEqual(expected[1].EventId, resumed.CurrentWeatherEvent.Id);
		Assert.AreEqual(expected[1].TemperatureFluctuation, resumed.CurrentTemperatureFluctuation);
	}

	[TestMethod]
	public void ForecastAstronomy_PhysicalAndAuthoredFrames_MatchOrdinaryFutureMinutes()
	{
		var context = CelestialTestFactory.CreateEarthSystem();
		context.SetDateTime("1/jan/2010", 0, 0, 0);
		using var authored = AuthoredCelestial.Create(50, AuthoredCelestialPresets.Create("RailSun", 1, 60, 24), context.Clock, context.Gameworld);
		ICelestialObject[] bodies = [context.Sun, context.Moon, context.SunFromMoon, context.Planet, authored];
		int[] offsets = [1, 359, 719, 1079, 1439, 2880, 10080];
		var instant = context.Calendar.CurrentInstant;
		var samples = offsets.ToDictionary(x => x, x => bodies.Select(body => WeatherAstronomy.Sample(body,
			x * 60.0 / context.Clock.InGameSecondsPerRealSecond, context.ZeroGeography)).ToArray());
		Assert.AreEqual(instant, context.Calendar.CurrentInstant);
		for (var minute = 1; minute <= offsets.Last(); minute++)
		{
			context.Clock.CurrentTime.AddMinutes(1);
			if (!samples.TryGetValue(minute, out var expected)) continue;
			for (var i = 0; i < bodies.Length; i++)
			{
				Assert.AreEqual(bodies[i].CurrentTimeOfDay(context.ZeroGeography), expected[i].Time, $"{bodies[i].GetType().Name} minute {minute}");
				Assert.AreEqual(bodies[i].CurrentCelestialDay, expected[i].Day, 0.00001, $"{bodies[i].GetType().Name} minute {minute}");
			}
		}
	}

	[TestMethod]
	public void SeasonOnsetBuilder_RebuildsRegionalRotationAndInvalidatesScheduledFuture()
	{
		using var fixture = new Fixture();
		var sun = Mock.Of<ICelestialObject>(x => x.Id == 1 && x.CelestialDaysPerYear == 365.0);
		fixture.World.SetupGet(x => x.CelestialObjects).Returns(new All<ICelestialObject> { sun });
		var first = new Season(new MudSharp.Models.Season { Id = 1, Name = "First", CelestialId = 1, CelestialDayOnset = 0 }, fixture.World.Object);
		var second = new Season(new MudSharp.Models.Season { Id = 2, Name = "Second", CelestialId = 1, CelestialDayOnset = 100 }, fixture.World.Object);
		fixture.World.SetupGet(x => x.Seasons).Returns(new All<ISeason> { first, second });
		var model = Mock.Of<IClimateModel>(x => x.Id == 1);
		fixture.World.SetupGet(x => x.ClimateModels).Returns(new All<IClimateModel> { model });
		var regional = new RegionalClimate(new MudSharp.Models.RegionalClimate
		{
			Id = 1, ClimateModelId = 1,
			RegionalClimatesSeasons = new[] { 1L, 2L }.Select(id => new MudSharp.Models.RegionalClimatesSeason { SeasonId = id, TemperatureInfo = "<Temperatures/>" }).ToList()
		}, fixture.World.Object);
		fixture.World.SetupGet(x => x.RegionalClimates).Returns(new All<IRegionalClimate> { regional });
		var controller = new Mock<IWeatherController>();
		var source = controller.As<IWeatherForecastSource>();
		controller.SetupGet(x => x.Id).Returns(1);
		controller.SetupGet(x => x.RegionalClimate).Returns(regional);
		fixture.World.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController> { controller.Object });
		Assert.AreSame(second, regional.SeasonRotation.Get(125));
		var actor = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<MudSharp.PerceptionEngine.IOutputHandler>());
		Assert.IsTrue(second.BuildingCommand(actor, new StringStack("onset 150")));
		Assert.AreSame(first, regional.SeasonRotation.Get(125));
		source.Verify(x => x.InvalidateForecast(), Times.Once);
	}

	[TestMethod]
	public void Forecast_SharedClockCalendarOrdering_DoesNotChangeDailyKeyOrSavedFuture()
	{
		using var fixture = new Fixture();
		var first = Mock.Of<ICalendar>(x => x.Id == 7 && x.FeedClock == fixture.Clock.Object &&
			x.CurrentInstant == new MudSharp.TimeAndDate.MudInstant(3_600_000));
		var second = Mock.Of<ICalendar>(x => x.Id == 8 && x.FeedClock == fixture.Clock.Object &&
			x.CurrentInstant == new MudSharp.TimeAndDate.MudInstant(7200));
		fixture.World.SetupGet(x => x.Calendars).Returns(new All<ICalendar> { first, second });
		var controller = new WeatherController(fixture.Model, fixture.World.Object);
		var expected = controller.GetForecast().ToArray();
		controller.Save();
		fixture.Context.SaveChanges();
		fixture.World.SetupGet(x => x.Calendars).Returns(new All<ICalendar> { second, first });
		var resumed = new WeatherController(fixture.Model, fixture.World.Object);
		Assert.AreEqual(controller.ForecastDay, resumed.ForecastDay);
		CollectionAssert.AreEqual(expected, resumed.GetForecast().ToArray());
	}

	[TestMethod]
	public void Forecast_RestartBetweenSeasonBoundaryAndCheckpoint_PreservesDurableSeasonAndQueue()
	{
		using var fixture = new Fixture();
		var oldSeason = fixture.Controller.CurrentSeason;
		var nextSeason = Mock.Of<ISeason>(x => x.Id == 2 && x.Name == "Autumn");
		((All<ISeason>)fixture.World.Object.Seasons).Add(nextSeason);
		var climate = Mock.Get(fixture.Controller.RegionalClimate);
		climate.SetupGet(x => x.Seasons).Returns([oldSeason, nextSeason]);
		climate.SetupGet(x => x.SeasonRotation).Returns(new CircularRange<ISeason>(365, new[] { (nextSeason, 0.0) }));
		climate.SetupGet(x => x.HourlyBaseTemperaturesBySeason).Returns(new[] { oldSeason, nextSeason }
			.SelectMany(s => Enumerable.Range(0, 4).Select(h => (s, h))).ToDictionary(x => x, _ => 15.0));
		fixture.Controller.GetForecast();
		fixture.Tick(fixture.Controller);
		Assert.AreSame(oldSeason, fixture.Controller.CurrentSeason);
		fixture.Controller.Save();
		fixture.Context.SaveChanges();
		var expected = fixture.Controller.GetForecast().ToArray();
		var resumed = new WeatherController(fixture.Model, fixture.World.Object);
		Assert.AreSame(oldSeason, resumed.CurrentSeason);
		CollectionAssert.AreEqual(expected, resumed.GetForecast().ToArray());
		fixture.Tick(resumed);
		fixture.Tick(resumed);
		Assert.AreSame(nextSeason, resumed.CurrentSeason);
	}

	[TestMethod]
	public void Forecast_ConsumedByLiveTicks_MatchesEventsDriftAndPersistence()
	{
		using var fixture = new Fixture();
		var controller = fixture.Controller;
		var forecast = controller.GetForecast().Skip(1).Take(30).ToArray();
		Assert.AreEqual(7, controller.ForecastHorizonDays);
		fixture.Weather[0].Verify(x => x.OnMinuteEvent(It.IsAny<MudSharp.Construction.ICell>()), Times.Never);
		Assert.AreEqual(0, fixture.Time.Minutes, "Prediction must not advance the live clock.");
		foreach (var point in forecast)
		{
			while (controller.ForecastMinute < point.Minute) fixture.Tick(controller);
			Assert.AreEqual(point.EventId, controller.CurrentWeatherEvent.Id);
			Assert.AreEqual(point.TemperatureFluctuation, controller.CurrentTemperatureFluctuation);
			Assert.AreEqual(point.UnchangedPeriods, controller.ConsecutiveUnchangedPeriods);
		}
		fixture.Tick(controller); // Save between climate processing checkpoints.
		controller.Save();
		fixture.Context.SaveChanges();
		var expected = controller.GetForecast().ToArray();
		var resumed = new WeatherController(fixture.Model, fixture.World.Object);
		CollectionAssert.AreEqual(expected, resumed.GetForecast().ToArray());
		foreach (var point in expected.Skip(1).Take(15))
		{
			while (resumed.ForecastMinute < point.Minute) fixture.Tick(resumed);
			Assert.AreEqual(point.EventId, resumed.CurrentWeatherEvent.Id);
			Assert.AreEqual(point.TemperatureFluctuation, resumed.CurrentTemperatureFluctuation);
		}
	}

	[TestMethod]
	public void Forecast_RepeatedQueriesAndUnrelatedRandomness_PreserveScheduledPrefix()
	{
		using var fixture = new Fixture();
		var expected = fixture.Controller.GetForecast().ToArray();
		for (var i = 0; i < 500; i++) Constants.Random.NextDouble();
		CollectionAssert.AreEqual(expected, fixture.Controller.GetForecast().ToArray());
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<MudSharp.PerceptionEngine.IOutputHandler>());
		fixture.Controller.BuildingCommand(actor.Object, new StringStack("forecast 1"));
		fixture.Controller.GetForecast();
		fixture.Controller.BuildingCommand(actor.Object, new StringStack("forecast 10"));
		CollectionAssert.AreEqual(expected, fixture.Controller.GetForecast().Take(expected.Length).ToArray());
	}

	[TestMethod]
	public void Forecast_ForcedWeatherAndFreeze_RebuildFutureWithoutSideEffects()
	{
		using var fixture = new Fixture();
		fixture.Controller.GetForecast();
		fixture.Controller.SetWeather(fixture.Weather[1].Object);
		fixture.Controller.FreezeWeather();
		Assert.IsTrue(fixture.Controller.GetForecast().All(x => x.EventId == 2));
		for (var i = 0; i < 10; i++) fixture.Tick(fixture.Controller);
		Assert.AreEqual(2L, fixture.Controller.CurrentWeatherEvent.Id);
		fixture.Controller.UnfreezeWeather();
		Assert.IsTrue(fixture.Controller.GetForecast().Any(x => x.EventId == 1));
	}

	[TestMethod]
	public void ForecastDay_PositiveTimezone_ChangesAtLocalMidnight()
	{
		using var fixture = new Fixture(hours: 24, minutes: 60);
		fixture.Zone.SetupGet(x => x.OffsetHours).Returns(2);
		for (var i = 0; i < 22 * 60 - 1; i++) fixture.Tick(fixture.Controller);
		Assert.AreEqual(0L, fixture.Controller.ForecastDay);
		fixture.Tick(fixture.Controller);
		Assert.AreEqual(1L, fixture.Controller.ForecastDay);
		for (var i = 0; i < 120; i++) fixture.Tick(fixture.Controller);
		Assert.AreEqual(1L, fixture.Controller.ForecastDay);
	}

	[TestMethod]
	public void Forecast_LongProcessingInterval_CoversHorizonAndProcessesDueCheckpoint()
	{
		using var fixture = new Fixture(interval: 100);
		var actor = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<MudSharp.PerceptionEngine.IOutputHandler>());
		fixture.Controller.BuildingCommand(actor, new StringStack("forecast 1"));
		Assert.AreEqual(40L, fixture.Controller.GetForecast().Last().Minute);
		Assert.IsTrue(fixture.Controller.GetForecast().All(x => x.EventId == 1));
		for (var i = 0; i < 100; i++) fixture.Tick(fixture.Controller);
		Assert.AreEqual(0, fixture.Controller.MinuteCounter);
	}

	[TestMethod]
	public void RecalledPeriodLabels_AgeWithoutChangingSavedContents()
	{
		var reading = new WeatherOutlookPeriod("Tomorrow", "rain", "breeze", "", 12, 20, 1);
		Assert.AreEqual("Today", WeatherForecastService.PeriodLabel(reading, 1, System.Globalization.CultureInfo.InvariantCulture));
		Assert.AreEqual("Yesterday", WeatherForecastService.PeriodLabel(reading, 2, System.Globalization.CultureInfo.InvariantCulture));
		Assert.AreEqual("Tomorrow", reading.Period);
	}

	[TestMethod]
	public void ForecastReading_RepeatedViewsForcedWeatherAndIndoorRecall_DoNotRepeatCheck()
	{
		using var fixture = new Fixture();
		var actor = new Mock<ICharacter>();
		var cell = new Mock<ICell>();
		var output = new Mock<MudSharp.PerceptionEngine.IOutputHandler>();
		actor.SetupGet(x => x.Gameworld).Returns(fixture.World.Object);
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.InnerLineFormatLength == 100 && x.LineFormatLength == 100 && x.UnitPreference == "SI"));
		actor.Setup(x => x.CanSee(cell.Object, PerceiveIgnoreFlags.None)).Returns(true);
		cell.SetupGet(x => x.WeatherController).Returns(fixture.Controller);
		cell.Setup(x => x.OutdoorsType(actor.Object)).Returns(CellOutdoorsType.Outdoors);
		fixture.World.SetupGet(x => x.UnitManager).Returns(Mock.Of<IUnitManager>());
		var capability = new Mock<ICheck>();
		capability.Setup(x => x.Check(actor.Object, Difficulty.Automatic, It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(),
			It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(new CheckOutcome { Outcome = Outcome.MajorPass });
		var check = new Mock<ICheck>();
		check.Setup(x => x.CheckAgainstAllDifficulties(actor.Object, Difficulty.Normal, It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(),
			It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => new CheckOutcome { Outcome = Outcome.MajorFail }));
		fixture.World.Setup(x => x.GetCheck(CheckType.WeatherForecastCapability)).Returns(capability.Object);
		fixture.World.Setup(x => x.GetCheck(CheckType.WeatherForecast)).Returns(check.Object);
		var saved = new List<WeatherForecastReading>();
		actor.Setup(x => x.EffectsOfType<WeatherForecastReading>(It.IsAny<Predicate<WeatherForecastReading>>())).Returns(() => saved);
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(e => saved.Add((WeatherForecastReading)e));
		actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), false)).Callback<IEffect, bool>((e, _) => saved.Remove((WeatherForecastReading)e));
		WeatherForecastService.Show(actor.Object, false);
		Assert.AreEqual(1, saved.Count);
		var first = saved.Single();
		WeatherForecastService.Show(actor.Object, true);
		fixture.Controller.SetWeather(fixture.Weather[1].Object);
		WeatherForecastService.Show(actor.Object, false);
		cell.Setup(x => x.OutdoorsType(actor.Object)).Returns(CellOutdoorsType.Indoors);
		for (var i = 0; i < 40; i++) fixture.Tick(fixture.Controller);
		WeatherForecastService.Show(actor.Object, true);
		Assert.AreSame(first, saved.Single());
		check.Verify(x => x.CheckAgainstAllDifficulties(actor.Object, Difficulty.Normal, It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(),
			It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Once);
		cell.Setup(x => x.OutdoorsType(actor.Object)).Returns(CellOutdoorsType.Outdoors);
		WeatherForecastService.Show(actor.Object, false);
		Assert.AreNotSame(first, saved.Single());
		check.Verify(x => x.CheckAgainstAllDifficulties(actor.Object, Difficulty.Normal, It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(),
			It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Exactly(2));
	}

	[TestMethod]
	public void ForecastReading_SaveAndLoad_PreservesDailyOutcomeAndBothViewsData()
	{
		using var fixture = new Fixture();
		var actor = Mock.Of<ICharacter>(x => x.Gameworld == fixture.World.Object);
		WeatherForecastReading.InitialiseEffectType();
		var reading = new WeatherForecastReading(actor, 4, 120, 7, Outcome.MajorFail,
			[new("Tomorrow", "rain", "Breeze", "lightning", 15, 24)]);
		var xml = reading.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = (WeatherForecastReading)Effect.LoadEffect(xml, actor);
		Assert.IsTrue(loaded.SavingEffect);
		Assert.AreEqual(120L, loaded.Day);
		Assert.AreEqual(4L, loaded.ControllerId);
		Assert.AreEqual(Outcome.MajorFail, loaded.Outcome);
		CollectionAssert.AreEqual(reading.Periods.ToArray(), loaded.Periods.ToArray());
	}

	private sealed class Fixture : IDisposable
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IClock> Clock { get; } = new();
		public Mock<IMudTimeZone> Zone { get; } = new();
		public Mock<IWeatherEvent>[] Weather { get; } = [new(), new()];
		public MudTime Time { get; }
		public WeatherController Controller { get; }
		public MudSharp.Models.WeatherController Model { get; }
		public FuturemudDatabaseContext Context { get; }
		private readonly object? _previousContext;
		private static readonly PropertyInfo ContextProperty = typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!;

		public Fixture(int hours = 4, int minutes = 10, int interval = 3)
		{
			Clock.SetupGet(x => x.Id).Returns(1);
			Clock.SetupGet(x => x.SecondsPerMinute).Returns(60);
			Clock.SetupGet(x => x.MinutesPerHour).Returns(minutes);
			Clock.SetupGet(x => x.HoursPerDay).Returns(hours);
			Clock.SetupGet(x => x.InGameSecondsPerRealSecond).Returns(1);
			var primary = Mock.Of<IMudTimeZone>(x => x.Id == 1 && x.Clock == Clock.Object);
			Zone.SetupGet(x => x.Id).Returns(2);
			Zone.SetupGet(x => x.Clock).Returns(Clock.Object);
			Clock.SetupGet(x => x.PrimaryTimezone).Returns(primary);
			Clock.SetupGet(x => x.Timezones).Returns([primary, Zone.Object]);
			Time = MudTime.CreatePrimaryTime(0, 0, 0, primary, Clock.Object);
			Clock.SetupGet(x => x.CurrentTime).Returns(Time);
			var season = Mock.Of<ISeason>(x => x.Id == 1 && x.Name == "Summer");
			for (var i = 0; i < Weather.Length; i++)
			{
				Weather[i].SetupGet(x => x.Id).Returns(i + 1);
				Weather[i].SetupGet(x => x.Hazards).Returns(new WeatherHazardSettings());
				Weather[i].SetupGet(x => x.PermittedTimesOfDay).Returns(Enum.GetValues<TimeOfDay>());
			}
			var model = new Mock<IClimateModel>();
			model.SetupGet(x => x.MinuteProcessingInterval).Returns(interval);
			model.Setup(x => x.HandleWeatherTick(It.IsAny<IWeatherEvent>(), It.IsAny<ISeason>(), It.IsAny<TimeOfDay>(), It.IsAny<int>(), It.IsAny<Func<double>>()))
				.Returns((IWeatherEvent _, ISeason _, TimeOfDay _, int _, Func<double> next) => Weather[next() < 0.5 ? 0 : 1].Object);
			var climate = new Mock<IRegionalClimate>();
			climate.SetupGet(x => x.Id).Returns(1);
			climate.SetupGet(x => x.ClimateModel).Returns(model.Object);
			climate.SetupGet(x => x.SeasonRotation).Returns(new CircularRange<ISeason>(365, new[] { (season, 0.0) }));
			climate.SetupGet(x => x.Seasons).Returns([season]);
			climate.SetupGet(x => x.TemperatureFluctuationStandardDeviation).Returns(3);
			climate.SetupGet(x => x.TemperatureFluctuationPeriod).Returns(TimeSpan.FromHours(1));
			climate.SetupGet(x => x.HourlyBaseTemperaturesBySeason).Returns(Enumerable.Range(0, hours).ToDictionary(x => (season, x), x => 15.0 + x));
			World.SetupGet(x => x.Clocks).Returns(new All<IClock> { Clock.Object });
			World.SetupGet(x => x.Calendars).Returns(new All<ICalendar>());
			World.SetupGet(x => x.RegionalClimates).Returns(new All<IRegionalClimate> { climate.Object });
			World.SetupGet(x => x.Seasons).Returns(new All<ISeason> { season });
			World.SetupGet(x => x.CelestialObjects).Returns(new All<ICelestialObject>());
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
			World.SetupGet(x => x.WeatherEvents).Returns(new All<IWeatherEvent> { Weather[0].Object, Weather[1].Object });
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			World.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
			Model = new() { Id = 1, Name = "Test", FeedClockId = 1, FeedClockTimeZoneId = 2, RegionalClimateId = 1,
				CurrentSeasonId = 1, CurrentWeatherEventId = 1, Radius = 6371000 };
			Context = new(new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
			Context.WeatherControllers.Add(Model);
			Context.SaveChanges();
			_previousContext = ContextProperty.GetValue(null);
			ContextProperty.SetValue(null, Context);
			Controller = new(Model, World.Object);
			World.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController> { Controller });
		}
		public void Tick(WeatherController controller) { Time.AddMinutes(1); controller.HandleWeatherTick(); }
		public void Dispose() { ContextProperty.SetValue(null, _previousContext); Context.Dispose(); }
	}
}
