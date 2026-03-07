using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Climate.Analysis;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using SeasonModel = MudSharp.Models.Season;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeatherSeederClimateTests
{
	[TestMethod]
	public void WeatherSeeder_TemperateOceanicProfile_RemainsWithinBroadLondonBounds()
	{
		var result = AnalyzeSeededNorthernHemisphereClimate(
			"Oceanic Temperate Northern Hemisphere",
			"Seeded Oceanic Temperate");

		var annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
		var winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
		var summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
		var annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
		var annualGaleOrWorse = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"Gale Wind",
				"Hurricane Wind",
				"Maelstrom Wind"
			});
		var annualHurricaneOrWorse = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"Hurricane Wind",
				"Maelstrom Wind"
			});
		var annualBreezeOrCalmer = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"None",
				"Still",
				"Occasional Breeze",
				"Breeze"
			});
		var midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
		var midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
		var lateAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_late_autumn", "Shelter");

		Assert.IsTrue(annualWet is >= 20.0 and <= 40.0, $"Expected annual wet occupancy to be broadly oceanic, but got {annualWet:F2}%.");
		Assert.IsTrue(winterWet is >= 22.0 and <= 45.0, $"Expected mid-winter wet occupancy to be believable for London, but got {winterWet:F2}%.");
		Assert.IsTrue(summerWet is >= 15.0 and <= 35.0, $"Expected mid-summer wet occupancy to avoid both drought and constant rain, but got {summerWet:F2}%.");
		Assert.IsTrue(winterWet > summerWet, $"Expected winter to be wetter than summer, but winter={winterWet:F2}% and summer={summerWet:F2}%.");
		Assert.IsTrue(annualSnow is > 0.0 and <= 3.0, $"Expected snow to remain uncommon in London-like conditions, but got {annualSnow:F2}%.");
		Assert.IsTrue(annualGaleOrWorse <= 6.0, $"Expected gale-force or worse wind to remain uncommon, but got {annualGaleOrWorse:F2}%.");
		Assert.IsTrue(annualHurricaneOrWorse <= 0.5, $"Expected hurricane-force or worse wind to be vanishingly rare, but got {annualHurricaneOrWorse:F2}%.");
		Assert.IsTrue(annualBreezeOrCalmer >= 70.0, $"Expected most weather to stay at breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
		Assert.IsTrue(midWinterShelterMean is >= 3.0 and <= 7.0, $"Expected mid-winter mean sheltered temperature to be mild, but got {midWinterShelterMean:F2}C.");
		Assert.IsTrue(midSummerShelterMean is >= 16.0 and <= 20.5, $"Expected mid-summer mean sheltered temperature to be warm without heat, but got {midSummerShelterMean:F2}C.");
		Assert.IsTrue(lateAutumnShelterMean is >= 7.0 and <= 11.5, $"Expected late autumn mean sheltered temperature to stay cool but not harsh, but got {lateAutumnShelterMean:F2}C.");
	}

	[TestMethod]
	public void WeatherSeeder_HumidSubtropicalProfile_RemainsWithinBroadAtlantaBounds()
	{
		var result = AnalyzeSeededNorthernHemisphereClimate(
			"Humid Subtropical Northern Hemisphere",
			"Seeded Humid Subtropical");

		var annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
		var winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
		var summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
		var autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
		var annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
		var annualGaleOrWorse = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"Gale Wind",
				"Hurricane Wind",
				"Maelstrom Wind"
			});
		var annualHurricaneOrWorse = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"Hurricane Wind",
				"Maelstrom Wind"
			});
		var annualBreezeOrCalmer = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"None",
				"Still",
				"Occasional Breeze",
				"Breeze"
			});
		var midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
		var midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
		var lateAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_late_autumn", "Shelter");

		Assert.IsTrue(annualWet is >= 25.0 and <= 45.0, $"Expected annual wet occupancy to stay in a humid subtropical band, but got {annualWet:F2}%.");
		Assert.IsTrue(winterWet is >= 18.0 and <= 40.0, $"Expected mid-winter wet occupancy to remain present without a dry season, but got {winterWet:F2}%.");
		Assert.IsTrue(summerWet is >= 28.0 and <= 50.0, $"Expected mid-summer wet occupancy to reflect a convective summer maximum, but got {summerWet:F2}%.");
		Assert.IsTrue(autumnWet is >= 15.0 and <= 32.0, $"Expected mid-autumn wet occupancy to be the comparatively drier season, but got {autumnWet:F2}%.");
		Assert.IsTrue(summerWet > autumnWet, $"Expected summer to be wetter than autumn, but summer={summerWet:F2}% and autumn={autumnWet:F2}%.");
		Assert.IsTrue(annualSnow <= 0.8, $"Expected snow to remain very rare in Atlanta-like conditions, but got {annualSnow:F2}%.");
		Assert.IsTrue(annualGaleOrWorse <= 3.0, $"Expected gale-force or worse wind to remain rare, but got {annualGaleOrWorse:F2}%.");
		Assert.IsTrue(annualHurricaneOrWorse <= 0.05, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
		Assert.IsTrue(annualBreezeOrCalmer >= 78.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
		Assert.IsTrue(midWinterShelterMean is >= 5.5 and <= 9.5, $"Expected mid-winter mean sheltered temperature to stay mild, but got {midWinterShelterMean:F2}C.");
		Assert.IsTrue(midSummerShelterMean is >= 24.0 and <= 28.8, $"Expected mid-summer mean sheltered temperature to be hot and humid, but got {midSummerShelterMean:F2}C.");
		Assert.IsTrue(lateAutumnShelterMean is >= 10.0 and <= 14.5, $"Expected late autumn mean sheltered temperature to stay mild, but got {lateAutumnShelterMean:F2}C.");
	}

	[TestMethod]
	public void WeatherSeeder_MediterraneanProfile_RemainsWithinBroadSacramentoBounds()
	{
		var result = AnalyzeSeededNorthernHemisphereClimate(
			"Mediterranean Northern Hemisphere",
			"Seeded Mediterranean");

		var annualWet = WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet");
		var winterWet = GetPercent(result.Rows, "temperate_mid_winter", "RainfallProxy", "Wet");
		var summerWet = GetPercent(result.Rows, "temperate_mid_summer", "RainfallProxy", "Wet");
		var autumnWet = GetPercent(result.Rows, "temperate_mid_autumn", "RainfallProxy", "Wet");
		var annualSnow = WeightedAveragePercent(result.Rows, "RainfallProxy", "Snow");
		var annualGaleOrWorse = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"Gale Wind",
				"Hurricane Wind",
				"Maelstrom Wind"
			});
		var annualHurricaneOrWorse = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"Hurricane Wind",
				"Maelstrom Wind"
			});
		var annualBreezeOrCalmer = WeightedAveragePercent(
			result.Rows,
			"Wind",
			new[]
			{
				"None",
				"Still",
				"Occasional Breeze",
				"Breeze"
			});
		var midWinterShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_winter", "Shelter");
		var midSummerShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_summer", "Shelter");
		var midAutumnShelterMean = AverageHourlyMean(result.Rows, "temperate_mid_autumn", "Shelter");

		Assert.IsTrue(annualWet is >= 8.0 and <= 21.0, $"Expected annual wet occupancy to stay low for a dry-summer climate, but got {annualWet:F2}%.");
		Assert.IsTrue(winterWet is >= 20.0 and <= 45.0, $"Expected mid-winter wet occupancy to carry the mediterranean rainy season, but got {winterWet:F2}%.");
		Assert.IsTrue(summerWet is >= 0.0 and <= 4.0, $"Expected mid-summer wet occupancy to be near-zero, but got {summerWet:F2}%.");
		Assert.IsTrue(autumnWet is >= 3.0 and <= 18.0, $"Expected mid-autumn wet occupancy to stay below winter while beginning the rainy season, but got {autumnWet:F2}%.");
		Assert.IsTrue(winterWet > autumnWet, $"Expected winter to be wetter than autumn, but winter={winterWet:F2}% and autumn={autumnWet:F2}%.");
		Assert.IsTrue(autumnWet > summerWet, $"Expected autumn to be wetter than summer, but autumn={autumnWet:F2}% and summer={summerWet:F2}%.");
		Assert.IsTrue(annualSnow <= 0.1, $"Expected snow to be essentially absent in Sacramento-like conditions, but got {annualSnow:F2}%.");
		Assert.IsTrue(annualGaleOrWorse <= 2.0, $"Expected gale-force or worse wind to remain rare, but got {annualGaleOrWorse:F2}%.");
		Assert.IsTrue(annualHurricaneOrWorse <= 0.05, $"Expected hurricane-force or worse wind to be effectively absent, but got {annualHurricaneOrWorse:F2}%.");
		Assert.IsTrue(annualBreezeOrCalmer >= 80.0, $"Expected most weather to remain breeze strength or calmer, but got {annualBreezeOrCalmer:F2}%.");
		Assert.IsTrue(midWinterShelterMean is >= 7.0 and <= 10.5, $"Expected mid-winter mean sheltered temperature to stay cool and mild, but got {midWinterShelterMean:F2}C.");
		Assert.IsTrue(midSummerShelterMean is >= 22.5 and <= 26.5, $"Expected mid-summer mean sheltered temperature to be hot and dry, but got {midSummerShelterMean:F2}C.");
		Assert.IsTrue(midAutumnShelterMean is >= 16.5 and <= 20.5, $"Expected mid-autumn mean sheltered temperature to stay warm, but got {midAutumnShelterMean:F2}C.");
	}

	private static WeatherStatisticsResult AnalyzeSeededNorthernHemisphereClimate(string regionalClimateName, string controllerName)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
			.Options;
		using var context = new FuturemudDatabaseContext(options);
		context.Celestials.Add(new Celestial
		{
			Definition = "<Celestial />",
			Minutes = 0,
			FeedClockId = 1,
			CelestialYear = 0,
			LastYearBump = 0,
			CelestialType = "test"
		});
		context.SaveChanges();

		var seeder = new WeatherSeeder();
		var error = seeder.SeedData(context, new Dictionary<string, string>
		{
			["rain"] = "none"
		});
		Assert.AreEqual(string.Empty, error);

		var regionalClimate = context.RegionalClimates
			.Include(x => x.RegionalClimatesSeasons)
			.ThenInclude(x => x.Season)
			.Single(x => x.Name == regionalClimateName);
		var climateModel = context.ClimateModels
			.Include(x => x.ClimateModelSeasons)
			.ThenInclude(x => x.Season)
			.Include(x => x.ClimateModelSeasons)
			.ThenInclude(x => x.SeasonEvents)
			.Single(x => x.Id == regionalClimate.ClimateModelId);
		var weatherEvents = context.WeatherEvents.ToList();
		var eventMap = weatherEvents.ToDictionary(x => x.Id);

		var seasons = regionalClimate.RegionalClimatesSeasons
			.Select(x => x.Season)
			.OrderBy(x => x.CelestialDayOnset)
			.ThenBy(x => x.Name)
				.ToList();
		var seasonMocks = seasons.ToDictionary(
			x => x.Id,
			CreateSeasonMock);
		var seasonRotation = new CircularRange<ISeason>(
			365.0,
			seasons.Select(x => (seasonMocks[x.Id], (double)x.CelestialDayOnset)));
		var baseTemperatures = regionalClimate.RegionalClimatesSeasons
			.SelectMany(x => ParseTemperatures(x.SeasonId, x.TemperatureInfo))
			.ToDictionary(x => (x.SeasonId, x.Hour), x => x.Temp);
		var snapshot = BuildSnapshot(climateModel, eventMap);
		var clock = new SeederSimulationClock();
		var analyzer = new WeatherStatisticsAnalyzer();
		return analyzer.AnalyzeSimulation(new WeatherStatisticsSimulationRequest
		{
			Years = 12,
			BurnInYears = 2,
			Seed = 123456,
			SeasonInfo = seasonMocks.ToDictionary(x => x.Key, x => x.Value),
			TransitionSnapshot = snapshot,
			SimulationContext = new WeatherSimulationContext
			{
				Geography = new GeographicCoordinate(51.5074, -0.1278, 11.0, 6_371_000.0),
				SeasonRotation = seasonRotation,
				FeedHoursPerDay = 24,
				FeedMinutesPerHour = 60,
				YearLengthDays = 365.0,
				GetCelestialDay = () => clock.CelestialDay,
				GetCurrentTimeOfDay = () => clock.GetTimeOfDay(),
				GetFeedLocalHour = () => clock.Hour,
				AdvanceOneFeedMinute = clock.AdvanceMinute,
				ConsumeYearBoundary = clock.ConsumeYearBoundary,
				GetBaseTemperature = (seasonId, hour) => baseTemperatures[(seasonId, hour)]
			},
			InitialSeasonId = seasonMocks.Values.First().Id,
			InitialWeatherEventId = null,
			InitialConsecutiveUnchangedPeriods = 0,
			InitialMinuteCounter = 0
		}, controllerName: controllerName);
	}

	private static WeatherTransitionSnapshot BuildSnapshot(ClimateModel climateModel, IReadOnlyDictionary<long, WeatherEvent> eventMap)
	{
		var eventInfo = eventMap.Values.ToDictionary(
			x => x.Id,
			x => new WeatherEventInfo
			{
				Id = x.Id,
				Name = x.Name,
				Precipitation = (PrecipitationLevel)x.Precipitation,
				Wind = (WindLevel)x.Wind,
				TemperatureEffect = x.TemperatureEffect,
				PrecipitationTemperatureEffect = x.PrecipitationTemperatureEffect,
				WindTemperatureEffect = x.WindTemperatureEffect,
				PermittedTimesOfDay = GetPermittedTimes(x)
			});
		var maxAdditional = new Dictionary<long, double>();
		var incremental = new Dictionary<long, double>();
		var baseChangeChance = new Dictionary<(long SeasonId, long EventId), double>();
		var transitions = new Dictionary<(long SeasonId, long EventId), IReadOnlyList<(long EventId, double Chance)>>();

		foreach (var season in climateModel.ClimateModelSeasons)
		{
			maxAdditional[season.SeasonId] = season.MaximumAdditionalChangeChanceFromStableWeather;
			incremental[season.SeasonId] = season.IncrementalAdditionalChangeChanceFromStableWeather;

			foreach (var seasonEvent in season.SeasonEvents)
			{
				baseChangeChance[(season.SeasonId, seasonEvent.WeatherEventId)] = seasonEvent.ChangeChance;
				transitions[(season.SeasonId, seasonEvent.WeatherEventId)] = WeatherStatisticsAnalyzer.ParseTransitions(seasonEvent.Transitions)
					.Where(x => x.EventId != seasonEvent.WeatherEventId)
					.ToList();
			}
		}

		return new WeatherTransitionSnapshot(
			climateModel.MinuteProcessingInterval,
			eventInfo,
			maxAdditional,
			incremental,
			baseChangeChance,
			transitions);
	}

	private static HashSet<TimeOfDay> GetPermittedTimes(WeatherEvent weatherEvent)
	{
		var permitted = new HashSet<TimeOfDay>();
		if (weatherEvent.PermittedAtNight)
		{
			permitted.Add(TimeOfDay.Night);
		}

		if (weatherEvent.PermittedAtDawn)
		{
			permitted.Add(TimeOfDay.Dawn);
		}

		if (weatherEvent.PermittedAtMorning)
		{
			permitted.Add(TimeOfDay.Morning);
		}

		if (weatherEvent.PermittedAtAfternoon)
		{
			permitted.Add(TimeOfDay.Afternoon);
		}

		if (weatherEvent.PermittedAtDusk)
		{
			permitted.Add(TimeOfDay.Dusk);
		}

		return permitted;
	}

	private static IEnumerable<(long SeasonId, int Hour, double Temp)> ParseTemperatures(long seasonId, string xml)
	{
		var root = XElement.Parse(xml);
		return root.Elements("Value")
			.Select(x => (
				SeasonId: seasonId,
				Hour: int.Parse(x.Attribute("hour")!.Value),
				Temp: double.Parse(x.Value)));
	}

	private static ISeason CreateSeasonMock(SeasonModel season)
	{
		var mock = new Mock<ISeason>();
		mock.SetupGet(x => x.Id).Returns(season.Id);
		mock.SetupGet(x => x.Name).Returns(season.Name);
		mock.SetupGet(x => x.DisplayName).Returns(season.DisplayName);
		mock.SetupGet(x => x.SeasonGroup).Returns(season.SeasonGroup);
		mock.SetupGet(x => x.CelestialDayOnset).Returns(season.CelestialDayOnset);
		return mock.Object;
	}

	private static double GetPercent(IEnumerable<WeatherStatisticsCsvRow> rows, string season, string metricType, string key)
	{
		return rows.Single(x =>
			x.Season == season &&
			x.MetricType == metricType &&
			x.Key == key &&
			x.Statistic == "Percent").Value;
	}

	private static double AverageHourlyMean(IEnumerable<WeatherStatisticsCsvRow> rows, string season, string key)
	{
		return rows
			.Where(x =>
				x.Season == season &&
				x.MetricType == "TemperatureHourly" &&
				x.Key == key &&
				x.Statistic == "Mean")
			.Average(x => x.Value);
	}

	private static double WeightedAveragePercent(IEnumerable<WeatherStatisticsCsvRow> rows, string metricType, string key)
	{
		return WeightedAveragePercent(rows, metricType, new[] { key });
	}

	private static double WeightedAveragePercent(IEnumerable<WeatherStatisticsCsvRow> rows, string metricType, IEnumerable<string> keys)
	{
		var keySet = keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var seasonRows = rows
			.Where(x => x.MetricType == metricType && x.Statistic == "Percent" && keySet.Contains(x.Key))
			.GroupBy(x => x.Season)
			.Select(x => new
			{
				Percent = x.Sum(y => y.Value),
				SeasonMinutes = x.First().SeasonMinutes
			})
			.ToList();
		var totalMinutes = seasonRows.Sum(x => x.SeasonMinutes);
		return totalMinutes == 0
			? 0.0
			: seasonRows.Sum(x => x.Percent * x.SeasonMinutes) / totalMinutes;
	}

	private sealed class SeederSimulationClock
	{
		private int _minute;
		private int _day;
		private bool _yearBoundary;

		public int Hour { get; private set; }

		public double CelestialDay => _day + ((Hour * 60 + _minute) / 1_440.0);

		public void AdvanceMinute()
		{
			_yearBoundary = false;
			if (++_minute < 60)
			{
				return;
			}

			_minute = 0;
			if (++Hour < 24)
			{
				return;
			}

			Hour = 0;
			if (++_day < 365)
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

		public TimeOfDay GetTimeOfDay()
		{
			return Hour switch
			{
				< 5 => TimeOfDay.Night,
				5 => TimeOfDay.Dawn,
				< 12 => TimeOfDay.Morning,
				< 18 => TimeOfDay.Afternoon,
				18 => TimeOfDay.Dusk,
				_ => TimeOfDay.Night
			};
		}
	}
}
