#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Climate.Analysis;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class WeatherSeederClimateTests
{
	private static readonly object ArmageddonLock = new();
	private static DbContextOptions<FuturemudDatabaseContext>? _armageddonOptions;
	private static FuturemudDatabaseContext ArmageddonContext()
	{
		lock (ArmageddonLock)
		{
			if (_armageddonOptions is null)
			{
				var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase(
					$"armageddon-climate-{Guid.NewGuid():N}", x => x.EnableNullChecks(false)).ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;
				using var db = new FuturemudDatabaseContext(options);
				new TimeSeeder().SeedData(db, new Dictionary<string, string> { ["mode"] = "Armageddon", ["secondsmultiplier"] = "6", ["startyear"] = "1" });
				new CelestialSeeder().SeedData(db, new Dictionary<string, string> { ["installarmageddon"] = "yes" });
				db.Gases.Add(new Gas { Name = "air" }); db.SaveChanges();
				Assert.AreEqual("", new WeatherSeeder().SeedData(db, new Dictionary<string, string> { ["rain"] = "none" }));
				ArmageddonWeatherInstaller.Install(() => new FuturemudDatabaseContext(options));
				_armageddonOptions = options;
			}
			return new FuturemudDatabaseContext(_armageddonOptions);
		}
	}

	[TestMethod]
	[DataRow("Southern Desert", 3.0, 15.0, 2.0, 10.0, 123456)]
	[DataRow("Southern Desert", 3.0, 15.0, 2.0, 10.0, 8675309)]
	[DataRow("Southern Desert", 3.0, 15.0, 2.0, 10.0, 424242)]
	[DataRow("Salt Flats", 5.0, 20.0, 5.0, 15.0, 123456)]
	[DataRow("Salt Flats", 5.0, 20.0, 5.0, 15.0, 8675309)]
	[DataRow("Salt Flats", 5.0, 20.0, 5.0, 15.0, 424242)]
	[DataRow("Northern Scrubland", 1.0, 8.0, .5, 4.0, 123456)]
	[DataRow("Northern Scrubland", 1.0, 8.0, .5, 4.0, 8675309)]
	[DataRow("Northern Scrubland", 1.0, 8.0, .5, 4.0, 424242)]
	[DataRow("Northern Forest", 0.0, 3.0, 0.0, 1.0, 123456)]
	[DataRow("Northern Forest", 0.0, 3.0, 0.0, 1.0, 8675309)]
	[DataRow("Northern Forest", 0.0, 3.0, 0.0, 1.0, 424242)]
	public void ArmageddonProfiles_FiveYearsMeetOccupancyAndHeatingBands(string name, double windMin,
		double windMax, double dustMin, double dustMax, int seed)
	{
		using var db = ArmageddonContext();
		var result = AnalyzeSeededClimate(db, name, name, 5, 1, 9, 60, 693, seed, includeFluctuation: true);
		Assert.AreEqual(6L * 693 * 540, result.SimulatedMinutes);
		Assert.AreEqual(5L * 693 * 540, result.Rows.Where(x => x.MetricType == "WeatherEvent" && x.Statistic == "Minutes").Sum(x => (long)x.Value));
		var wind = WeightedAveragePercent(result.Rows, "Wind", new[] { "Strong Wind", "Gale Wind" });
		var dustNames = db.WeatherEvents.Where(x => x.Name.StartsWith("WeatherHazard_Dust_") || x.Name.StartsWith("WeatherHazard_ChokingDust_")).Select(x => x.Name).ToArray();
		var dust = WeightedAveragePercent(result.Rows, "WeatherEvent", dustNames);
		AssertBetween(wind, windMin, windMax, $"{name}/{seed} strong wind or gale");
		AssertBetween(dust, dustMin, dustMax, $"{name}/{seed} dust");
		Assert.AreEqual(0.0, WeightedAveragePercent(result.Rows, "RainfallProxy", "Wet"));
		Assert.AreEqual(0.0, WeightedAveragePercent(result.Rows, "Wind", new[] { "Hurricane Wind", "Maelstrom Wind" }));
		var prohibitedNames = db.WeatherEvents.Where(x => x.Name.StartsWith("WeatherHazard_Lightning_")).Select(x => x.Name).ToArray();
		Assert.AreEqual(0.0, WeightedAveragePercent(result.Rows, "WeatherEvent", prohibitedNames));
		var regional = db.RegionalClimates.Include(x => x.RegionalClimatesSeasons).ThenInclude(x => x.Season).Single(x => x.Name == name);
		var baselines = regional.RegionalClimatesSeasons.SelectMany(x => XElement.Parse(x.TemperatureInfo).Elements()
			.Select(e => (Key: (x.Season.Name, Hour: (int)e.Attribute("hour")!), Value: (double)e))).ToDictionary(x => x.Key, x => x.Value);
		var means = result.Rows.Where(x => x.MetricType == "TemperatureHourly" && x.Key == "Shelter" && x.Statistic == "Mean").ToArray();
		Assert.AreEqual(81, means.Length);
		foreach (var mean in means)
			Assert.AreEqual(baselines[(mean.Season, mean.Hour!.Value)], mean.Value, 2, $"{name}/{seed} {mean.Season} hour {mean.Hour}");
		Console.WriteLine($"{name}/{seed}: strong/gale {wind:F3}%; dust {dust:F3}%; largest hourly mean error {means.Max(x => Math.Abs(x.Value - baselines[(x.Season, x.Hour!.Value)])):F3}°C");
	}
}
