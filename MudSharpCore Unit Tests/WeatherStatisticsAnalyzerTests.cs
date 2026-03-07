using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Climate.Analysis;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeatherStatisticsAnalyzerTests
{
	[TestMethod]
	public void AnalyzeSimulation_SingleEventDeterministic_YieldsFullOccupancyAndFixedTemperatures()
	{
		var season = CreateSeason(1, "Season One", 0);
		var seasonInfo = new Dictionary<long, ISeason> { [season.Id] = season };
		var seasonRotation = new CircularRange<ISeason>(5.0, new[] { (season, 0.0) });
		var simulationClock = new TinySimulationClock(2, 3, 5);
		var context = CreateContext(simulationClock, seasonRotation, (_, _) => 20.0, () => TimeOfDay.Morning);

		var events = new Dictionary<long, WeatherEventInfo>
		{
			[10] = new()
			{
				Id = 10,
				Name = "Calm",
				Precipitation = PrecipitationLevel.Dry,
				Wind = WindLevel.Still,
				TemperatureEffect = 5.0,
				PermittedTimesOfDay = Enum.GetValues<TimeOfDay>().ToHashSet()
			}
		};

		var snapshot = new WeatherTransitionSnapshot(
			1,
			events,
			new Dictionary<long, double> { [season.Id] = 0.0 },
			new Dictionary<long, double> { [season.Id] = 0.0 },
			new Dictionary<(long SeasonId, long EventId), double> { [(season.Id, 10)] = 0.0 },
			new Dictionary<(long SeasonId, long EventId), IReadOnlyList<(long EventId, double Chance)>>
			{
				[(season.Id, 10)] = Array.Empty<(long EventId, double Chance)>()
			});

		var analyzer = new WeatherStatisticsAnalyzer();
		var result = analyzer.AnalyzeSimulation(new WeatherStatisticsSimulationRequest
		{
			Years = 2,
			BurnInYears = 0,
			Seed = 12345,
			SeasonInfo = seasonInfo,
			TransitionSnapshot = snapshot,
			SimulationContext = context,
			InitialSeasonId = season.Id,
			InitialWeatherEventId = 10,
			InitialConsecutiveUnchangedPeriods = 0,
			InitialMinuteCounter = 0
		});

		var eventPercent = result.Rows.Single(x =>
			x.Season == season.Name &&
			x.MetricType == "WeatherEvent" &&
			x.Key == "Calm" &&
			x.Statistic == "Percent");
		Assert.AreEqual(100.0, eventPercent.Value, 0.00001);

		var temperatureRows = result.Rows.Where(x => x.MetricType == "TemperatureHourly").ToList();
		Assert.IsTrue(temperatureRows.Count > 0);
		Assert.IsTrue(temperatureRows.All(x => Math.Abs(x.Value - 25.0) < 0.00001));
	}

	[TestMethod]
	public void AnalyzeSimulation_AlternatingTransitions_PreservesEventAndStateMinuteTotals()
	{
		var season = CreateSeason(1, "Season One", 0);
		var seasonInfo = new Dictionary<long, ISeason> { [season.Id] = season };
		var seasonRotation = new CircularRange<ISeason>(4.0, new[] { (season, 0.0) });
		var simulationClock = new TinySimulationClock(2, 2, 4);
		var context = CreateContext(simulationClock, seasonRotation, (_, _) => 10.0, () => TimeOfDay.Morning);

		var events = new Dictionary<long, WeatherEventInfo>
		{
			[1] = new()
			{
				Id = 1,
				Name = "Rain Event",
				Precipitation = PrecipitationLevel.Rain,
				Wind = WindLevel.Breeze,
				TemperatureEffect = 0.0,
				PermittedTimesOfDay = Enum.GetValues<TimeOfDay>().ToHashSet()
			},
			[2] = new()
			{
				Id = 2,
				Name = "Snow Event",
				Precipitation = PrecipitationLevel.Snow,
				Wind = WindLevel.StrongWind,
				TemperatureEffect = 0.0,
				PermittedTimesOfDay = Enum.GetValues<TimeOfDay>().ToHashSet()
			}
		};

		var snapshot = new WeatherTransitionSnapshot(
			1,
			events,
			new Dictionary<long, double> { [season.Id] = 0.0 },
			new Dictionary<long, double> { [season.Id] = 0.0 },
			new Dictionary<(long SeasonId, long EventId), double>
			{
				[(season.Id, 1)] = 1.0,
				[(season.Id, 2)] = 1.0
			},
			new Dictionary<(long SeasonId, long EventId), IReadOnlyList<(long EventId, double Chance)>>
			{
				[(season.Id, 1)] = new[] { (2L, 1.0) },
				[(season.Id, 2)] = new[] { (1L, 1.0) }
			});

		var analyzer = new WeatherStatisticsAnalyzer();
		var result = analyzer.AnalyzeSimulation(new WeatherStatisticsSimulationRequest
		{
			Years = 1,
			BurnInYears = 0,
			Seed = 1337,
			SeasonInfo = seasonInfo,
			TransitionSnapshot = snapshot,
			SimulationContext = context,
			InitialSeasonId = season.Id,
			InitialWeatherEventId = 1,
			InitialConsecutiveUnchangedPeriods = 0,
			InitialMinuteCounter = 0
		});

		var seasonMinutes = result.Rows
			.First(x => x.MetricType == "WeatherEvent" && x.Statistic == "Minutes")
			.SeasonMinutes;

		var totalEventMinutes = result.Rows
			.Where(x => x.MetricType == "WeatherEvent" && x.Statistic == "Minutes")
			.Sum(x => x.Value);
		Assert.AreEqual(seasonMinutes, totalEventMinutes, 0.00001);

		var totalPrecipMinutes = result.Rows
			.Where(x => x.MetricType == "Precipitation" && x.Statistic == "Minutes")
			.Sum(x => x.Value);
		Assert.AreEqual(seasonMinutes, totalPrecipMinutes, 0.00001);

		var totalWindMinutes = result.Rows
			.Where(x => x.MetricType == "Wind" && x.Statistic == "Minutes")
			.Sum(x => x.Value);
		Assert.AreEqual(seasonMinutes, totalWindMinutes, 0.00001);

		var rainMinutes = result.Rows.Single(x => x.MetricType == "WeatherEvent" && x.Key == "Rain Event" && x.Statistic == "Minutes").Value;
		var snowMinutes = result.Rows.Single(x => x.MetricType == "WeatherEvent" && x.Key == "Snow Event" && x.Statistic == "Minutes").Value;
		Assert.IsTrue(Math.Abs(rainMinutes - snowMinutes) <= 1.0);
	}

	[TestMethod]
	public void AnalyzeSimulation_TemperatureSummary_IncludesGeneralRainAndWindModifiers()
	{
		var season = CreateSeason(1, "Season One", 0);
		var seasonInfo = new Dictionary<long, ISeason> { [season.Id] = season };
		var seasonRotation = new CircularRange<ISeason>(4.0, new[] { (season, 0.0) });
		var simulationClock = new TinySimulationClock(1, 4, 1);
		var context = CreateContext(simulationClock, seasonRotation, (_, _) => 10.0, () => TimeOfDay.Morning);

		var events = new Dictionary<long, WeatherEventInfo>
		{
			[1] = new()
			{
				Id = 1,
				Name = "Warmer Wet Windy",
				Precipitation = PrecipitationLevel.Rain,
				Wind = WindLevel.StrongWind,
				TemperatureEffect = 1.0,
				PrecipitationTemperatureEffect = 2.0,
				WindTemperatureEffect = 3.0,
				PermittedTimesOfDay = Enum.GetValues<TimeOfDay>().ToHashSet()
			}
		};

		var snapshot = new WeatherTransitionSnapshot(
			1,
			events,
			new Dictionary<long, double> { [season.Id] = 0.0 },
			new Dictionary<long, double> { [season.Id] = 0.0 },
			new Dictionary<(long SeasonId, long EventId), double> { [(season.Id, 1)] = 0.0 },
			new Dictionary<(long SeasonId, long EventId), IReadOnlyList<(long EventId, double Chance)>>
			{
				[(season.Id, 1)] = Array.Empty<(long EventId, double Chance)>()
			});

		var analyzer = new WeatherStatisticsAnalyzer();
		var result = analyzer.AnalyzeSimulation(new WeatherStatisticsSimulationRequest
		{
			Years = 1,
			BurnInYears = 0,
			Seed = 2468,
			SeasonInfo = seasonInfo,
			TransitionSnapshot = snapshot,
			SimulationContext = context,
			InitialSeasonId = season.Id,
			InitialWeatherEventId = 1,
			InitialConsecutiveUnchangedPeriods = 0,
			InitialMinuteCounter = 0
		});

		var shelterMean = result.Rows.Single(x =>
			x.Season == season.Name &&
			x.MetricType == "TemperatureHourly" &&
			x.Key == "Shelter" &&
			x.Hour == 0 &&
			x.Statistic == "Mean");
		var shelterLikelyMin = result.Rows.Single(x =>
			x.Season == season.Name &&
			x.MetricType == "TemperatureHourly" &&
			x.Key == "Shelter" &&
			x.Hour == 0 &&
			x.Statistic == "LikelyMin");
		var shelterLikelyMax = result.Rows.Single(x =>
			x.Season == season.Name &&
			x.MetricType == "TemperatureHourly" &&
			x.Key == "Shelter" &&
			x.Hour == 0 &&
			x.Statistic == "LikelyMax");
		var outdoorsMean = result.Rows.Single(x =>
			x.Season == season.Name &&
			x.MetricType == "TemperatureHourly" &&
			x.Key == "Outdoors" &&
			x.Hour == 0 &&
			x.Statistic == "Mean");
		var outdoorsLikelyMin = result.Rows.Single(x =>
			x.Season == season.Name &&
			x.MetricType == "TemperatureHourly" &&
			x.Key == "Outdoors" &&
			x.Hour == 0 &&
			x.Statistic == "LikelyMin");
		var outdoorsLikelyMax = result.Rows.Single(x =>
			x.Season == season.Name &&
			x.MetricType == "TemperatureHourly" &&
			x.Key == "Outdoors" &&
			x.Hour == 0 &&
			x.Statistic == "LikelyMax");

		Assert.AreEqual(11.0, shelterMean.Value, 0.00001);
		Assert.AreEqual(11.0, shelterLikelyMin.Value, 0.00001);
		Assert.AreEqual(11.0, shelterLikelyMax.Value, 0.00001);
		Assert.AreEqual(16.0, outdoorsMean.Value, 0.00001);
		Assert.AreEqual(16.0, outdoorsLikelyMin.Value, 0.00001);
		Assert.AreEqual(16.0, outdoorsLikelyMax.Value, 0.00001);
	}

	[TestMethod]
	public void ParseTransitions_LegacySeederTransitionNodes_AreParsed()
	{
		const string xml = """
		                   <Transitions>
		                   	<Transition id="101" chance="2.5" />
		                   	<Transition id="202" chance="7.5" />
		                   </Transitions>
		                   """;

		var result = WeatherStatisticsAnalyzer.ParseTransitions(xml);

		Assert.AreEqual(2, result.Count);
		Assert.AreEqual((101L, 2.5), result[0]);
		Assert.AreEqual((202L, 7.5), result[1]);
	}

	[TestMethod]
	public void ParseTransitions_CurrentEventNodes_AreParsed()
	{
		const string xml = """
		                   <Transitions>
		                   	<Event id="303" chance="1.25" />
		                   	<Event id="404" chance="3.75" />
		                   </Transitions>
		                   """;

		var result = WeatherStatisticsAnalyzer.ParseTransitions(xml);

		Assert.AreEqual(2, result.Count);
		Assert.AreEqual((303L, 1.25), result[0]);
		Assert.AreEqual((404L, 3.75), result[1]);
	}

	[TestMethod]
	public void Percentile_Central95_ComputesExpectedInterpolatedValues()
	{
		var values = Enumerable.Range(1, 100).Select(x => (double)x).ToList();

		var low = WeatherStatisticsMath.Percentile(values, 0.025);
		var high = WeatherStatisticsMath.Percentile(values, 0.975);

		Assert.AreEqual(3.475, low, 0.0001);
		Assert.AreEqual(97.525, high, 0.0001);
	}

	[TestMethod]
	public void HourlyBlockTracker_BucketsBySeasonAndHour_RecordsMinMeanAndMaxPerBlock()
	{
		var accumulators = new Dictionary<long, SeasonAccumulator>
		{
			[1] = new SeasonAccumulator()
		};
		var tracker = new HourlyBlockTracker();

		tracker.RecordMinute(1, 0, 10.0, 15.0, accumulators);
		tracker.RecordMinute(1, 0, 20.0, 25.0, accumulators);
		tracker.RecordMinute(1, 0, 30.0, 35.0, accumulators);
		tracker.RecordMinute(1, 1, 5.0, 7.0, accumulators);
		tracker.RecordMinute(1, 1, 15.0, 17.0, accumulators);
		tracker.RecordMinute(1, 0, 40.0, 45.0, accumulators);
		tracker.RecordMinute(1, 0, 50.0, 55.0, accumulators);
		tracker.Finalize(accumulators);

		var hour0 = accumulators[1].HourlyStatistics[0];
		CollectionAssert.AreEqual(new[] { 10.0, 40.0 }, hour0.Shelter.HourlyMins.ToArray());
		CollectionAssert.AreEqual(new[] { 30.0, 50.0 }, hour0.Shelter.HourlyMaxs.ToArray());
		CollectionAssert.AreEqual(new[] { 20.0, 45.0 }, hour0.Shelter.HourlyMeans.ToArray());
		CollectionAssert.AreEqual(new[] { 15.0, 45.0 }, hour0.Outdoors.HourlyMins.ToArray());
		CollectionAssert.AreEqual(new[] { 35.0, 55.0 }, hour0.Outdoors.HourlyMaxs.ToArray());
		CollectionAssert.AreEqual(new[] { 25.0, 50.0 }, hour0.Outdoors.HourlyMeans.ToArray());

		var hour1 = accumulators[1].HourlyStatistics[1];
		CollectionAssert.AreEqual(new[] { 5.0 }, hour1.Shelter.HourlyMins.ToArray());
		CollectionAssert.AreEqual(new[] { 15.0 }, hour1.Shelter.HourlyMaxs.ToArray());
		CollectionAssert.AreEqual(new[] { 10.0 }, hour1.Shelter.HourlyMeans.ToArray());
		CollectionAssert.AreEqual(new[] { 7.0 }, hour1.Outdoors.HourlyMins.ToArray());
		CollectionAssert.AreEqual(new[] { 17.0 }, hour1.Outdoors.HourlyMaxs.ToArray());
		CollectionAssert.AreEqual(new[] { 12.0 }, hour1.Outdoors.HourlyMeans.ToArray());
	}

	private static WeatherSimulationContext CreateContext(
		TinySimulationClock clock,
		CircularRange<ISeason> seasonRotation,
		Func<long, int, double> temperatureSelector,
		Func<TimeOfDay> timeOfDaySelector)
	{
		return new WeatherSimulationContext
		{
			Geography = new GeographicCoordinate(0.0, 0.0, 0.0, 6_371_000.0),
			SeasonRotation = seasonRotation,
			FeedHoursPerDay = clock.HoursPerDay,
			FeedMinutesPerHour = clock.MinutesPerHour,
			YearLengthDays = clock.DaysPerYear,
			GetCelestialDay = () => clock.CelestialDay,
			GetCurrentTimeOfDay = timeOfDaySelector,
			GetFeedLocalHour = () => clock.Hour,
			AdvanceOneFeedMinute = clock.AdvanceMinute,
			ConsumeYearBoundary = clock.ConsumeYearBoundary,
			GetBaseTemperature = temperatureSelector
		};
	}

	private static ISeason CreateSeason(long id, string name, int onset)
	{
		var season = new Mock<ISeason>();
		season.SetupGet(x => x.Id).Returns(id);
		season.SetupGet(x => x.Name).Returns(name);
		season.SetupGet(x => x.DisplayName).Returns(name);
		season.SetupGet(x => x.CelestialDayOnset).Returns(onset);
		return season.Object;
	}

	private sealed class TinySimulationClock
	{
		private int _minute;
		private int _day;
		private bool _yearBoundary;

		public TinySimulationClock(int hoursPerDay, int minutesPerHour, int daysPerYear)
		{
			HoursPerDay = hoursPerDay;
			MinutesPerHour = minutesPerHour;
			DaysPerYear = daysPerYear;
			Hour = 0;
			_minute = 0;
			_day = 0;
		}

		public int HoursPerDay { get; }
		public int MinutesPerHour { get; }
		public int DaysPerYear { get; }
		public int Hour { get; private set; }

		public double CelestialDay => _day + ((Hour * MinutesPerHour + _minute) / (double)(HoursPerDay * MinutesPerHour));

		public void AdvanceMinute()
		{
			_yearBoundary = false;
			if (++_minute < MinutesPerHour)
			{
				return;
			}

			_minute = 0;
			if (++Hour < HoursPerDay)
			{
				return;
			}

			Hour = 0;
			if (++_day < DaysPerYear)
			{
				return;
			}

			_day = 0;
			_yearBoundary = true;
		}

		public bool ConsumeYearBoundary()
		{
			var value = _yearBoundary;
			_yearBoundary = false;
			return value;
		}
	}
}
