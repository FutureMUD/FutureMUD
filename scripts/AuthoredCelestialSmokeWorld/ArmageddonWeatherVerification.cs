#nullable enable

extern alias EngineCompiler;

using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using MySql.Data.MySqlClient;
using EngineCompiler::MudSharp.Celestial.Authored;
using EngineCompiler::MudSharp.Framework;
using FMDB = EngineCompiler::MudSharp.Database.FMDB;
using RuntimeClock = EngineCompiler::MudSharp.TimeAndDate.Time.Clock;
using RuntimeCalendar = EngineCompiler::MudSharp.TimeAndDate.Date.Calendar;
using RuntimeSeason = EngineCompiler::MudSharp.Climate.Season;
using RuntimeClimate = EngineCompiler::MudSharp.Climate.RegionalClimate;
using RuntimeModel = EngineCompiler::MudSharp.Climate.ClimateModels.TerrestrialClimateModel;
using RuntimeWeather = EngineCompiler::MudSharp.Climate.WeatherEvents.SimpleWeatherEvent;
using RuntimeController = EngineCompiler::MudSharp.Climate.WeatherController;

namespace AuthoredCelestialSmokeWorld;

/// <summary>Called only after the outer harness verifies the owned MySQL instance and data directory.</summary>
internal static class ArmageddonWeatherVerification
{
	private static void Require(bool condition, string message)
	{
		if (!condition) throw new InvalidOperationException(message);
	}

	public static void Run(string verifiedConnection, string receiptPath)
	{
		// A separate fresh schema proves native nine-hour WeatherSeeder installation without retiming
		// the existing terrestrial catalogue created by the maintained full replay.
		var builder = new MySqlConnectionStringBuilder(verifiedConnection) { Database = "authored_weather_native" };
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(verifiedConnection)).Options;
		FuturemudDatabaseContext Factory() => new(options);
		using (var db = Factory())
		{
			Require(!db.Database.CanConnect(), "Weather qualification requires a new owned schema.");
			var snapshot = Environment.GetEnvironmentVariable("AUTHORED_SMOKE_SNAPSHOT") ?? throw new InvalidOperationException("Missing repository snapshot path.");
			new MySqlDatabaseBackupService().RecreateEmptyDatabase(builder.ConnectionString);
			new DatabaseUpgradeCoordinator().ImportBlankDatabaseSnapshot(builder.ConnectionString, snapshot, "__FUTUREMUD_DATABASE__");
			db.Database.Migrate();
			new TimeSeeder().SeedData(db, new Dictionary<string, string> { ["mode"] = "Armageddon", ["secondsmultiplier"] = "6", ["startyear"] = "1" });
			new CelestialSeeder().SeedData(db, new Dictionary<string, string> { ["packages"] = "Armageddon" });
			using var source = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseMySql(verifiedConnection, ServerVersion.AutoDetect(verifiedConnection)).Options);
			var air = source.Gases.First(x => x.Name == "Breathable Atmosphere" || x.Name == "air");
			var clone = (Gas)source.Entry(air).CurrentValues.ToObject();
			clone.Id = 0; clone.CountAsId = null; clone.DrugId = null; clone.PrecipitateId = null;
			db.Gases.Add(clone); db.SaveChanges();
			Require(new WeatherSeeder().SeedData(db, new Dictionary<string, string> { ["rain"] = "none" }) == "", "Native nine-hour weather install failed.");
			Require(db.RegionalClimatesSeasons.AsEnumerable().All(x => XElement.Parse(x.TemperatureInfo).Elements().Count() == 9), "Native generic hourly tables did not use nine hours.");
			Require(db.Seasons.Single(x => x.Name == "temperate_early_spring").CelestialDayOnset == 116, "Native season dates did not scale to 693 days.");
		}
		ArmageddonWeatherInstaller.Install(Factory);
		long deletedId, modelId, regionId;
		long controllerId;
		string temperature;
		using (var db = Factory())
		{
			Require(db.RegionalClimates.Count(x => x.Name == "Southern Desert" || x.Name == "Salt Flats" || x.Name == "Northern Scrubland" || x.Name == "Northern Forest") == 4, "Missing native regional templates.");
			var model = db.ClimateModels.Single(x => x.Name == "Northern Forest"); modelId = model.Id;
			model.Description = "Native builder's retained description";
			var regional = db.RegionalClimates.Single(x => x.Name == "Northern Forest"); regionId = regional.Id;
			var table = db.RegionalClimatesSeasons.First(x => x.RegionalClimateId == regionId);
			var xml = XElement.Parse(table.TemperatureInfo); xml.Elements().First().Value = "17.125";
			table.TemperatureInfo = temperature = xml.ToString();
			var missing = db.Seasons.Single(x => x.Name == "armageddon_1_0"); deletedId = missing.Id;
			db.Seasons.Remove(missing); db.SaveChanges();
		}
		ArmageddonWeatherInstaller.Install(Factory);
		ArmageddonWeatherInstaller.Install(Factory);
		using (var db = Factory())
		{
			Require(db.Seasons.Find(deletedId) is not null, "Missing owned season was not restored at the same ID.");
			Require(db.ClimateModels.Find(modelId)!.Description == "Native builder's retained description", "Native rerun replaced model edit.");
			Require(db.RegionalClimatesSeasons.Any(x => x.RegionalClimateId == regionId && x.TemperatureInfo == temperature), "Native rerun replaced temperature edit.");
			Require(db.Set<ClimateModelSeasonEvent>().Count(x => x.Season.Name.StartsWith("armageddon_")) == 324, "Native partial graph recovery incomplete.");
			Require(db.WeatherControllers.Count() == 0, "Templates automatically created a controller.");
		}
		var (ticks, createdControllerId) = ExerciseController(Factory);
		controllerId = createdControllerId;
		ArmageddonWeatherInstaller.Install(Factory);
		using (var db = Factory())
		{
			var controller = db.WeatherControllers.Find(controllerId)!;
			Require(controller.RegionalClimateId == regionId, "Native rerun retargeted controller.");
			Require(controller.CurrentSeasonId == db.Seasons.Single(x => x.Name == "armageddon_0_0").Id && controller.ForecastState is not null, "Native controller boundary state did not persist.");
			var existingTemperatures = db.RegionalClimatesSeasons.OrderBy(x => x.RegionalClimateId).ThenBy(x => x.SeasonId).Select(x => x.TemperatureInfo).ToArray();
			var existingOnsets = db.Seasons.OrderBy(x => x.Id).Select(x => x.CelestialDayOnset).ToArray();
			Require(new WeatherSeeder().SeedData(db, new Dictionary<string, string> { ["operation"] = "install" }).Contains("Weather already exists"), "Native weather reinstall was not refused.");
			Require(new WeatherSeeder().SeedData(db, new Dictionary<string, string> { ["operation"] = "hazards" }) == "", "Native hazard update failed.");
			Require(existingTemperatures.SequenceEqual(db.RegionalClimatesSeasons.OrderBy(x => x.RegionalClimateId).ThenBy(x => x.SeasonId).Select(x => x.TemperatureInfo)), "Hazard update retimed native temperatures.");
			Require(existingOnsets.SequenceEqual(db.Seasons.OrderBy(x => x.Id).Select(x => x.CelestialDayOnset)), "Hazard update retimed native seasons.");
		}
		File.WriteAllText(receiptPath, JsonSerializer.Serialize(new
		{
			status = "PASS", scenario = "Native fresh nine-hour weather, four Armageddon templates, preserving partial recovery and loaded controller boundaries",
			freshSchema = builder.Database, sampledControllerMinutes = ticks, allNineHours = true, yearBoundary = true,
			modelId, regionId, deletedSeasonId = deletedId, controllerId, builderSelectedClimateClock = true,
			firstGlobalClockHours = 24, firstGlobalHour = 18, coldControllerReload = true, automaticAssignment = false, gameplaySmoke = "NOT_RUN"
		}, new JsonSerializerOptions { WriteIndented = true }));
		Console.WriteLine("PASS: native Armageddon weather installation, recovery and runtime controller boundaries.");
	}

	private static (int Ticks, long ControllerId) ExerciseController(Func<FuturemudDatabaseContext> factory)
	{
		using var db = factory();
		var world = new Mock<IFuturemud>();
		var clocks = new All<IClock>(); var calendars = new All<ICalendar>(); var bodies = new All<ICelestialObject>();
		world.SetupGet(x => x.Clocks).Returns(clocks); world.SetupGet(x => x.Calendars).Returns(calendars);
		world.SetupGet(x => x.CelestialObjects).Returns(bodies);
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		world.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController>());
		var clockRow = db.Clocks.Include(x => x.Timezones).Single();
		var calendarRow = db.Calendars.Single();
		var clock = new RuntimeClock(clockRow, world.Object);
		var foreignClock = new Mock<IClock>();
		foreignClock.SetupGet(x => x.Id).Returns(99999); foreignClock.SetupGet(x => x.HoursPerDay).Returns(24);
		foreignClock.SetupGet(x => x.MinutesPerHour).Returns(60); foreignClock.SetupGet(x => x.SecondsPerMinute).Returns(60);
		var foreignZone = Mock.Of<IMudTimeZone>(x => x.Id == 99999 && x.Clock == foreignClock.Object);
		foreignClock.SetupGet(x => x.PrimaryTimezone).Returns(foreignZone);
		foreignClock.SetupGet(x => x.CurrentTime).Returns(MudTime.CreatePrimaryTime(0, 0, 18, foreignZone, foreignClock.Object));
		clocks.Add(foreignClock.Object); clocks.Add(clock);
		bodies.Add(Mock.Of<ICelestialObject>(x => x.Id == 99999 && x.CelestialDaysPerYear == 365.24));
		var calendar = new RuntimeCalendar(calendarRow, world.Object); calendars.Add(calendar); calendar.FeedClock = clock;
		calendar.SetDate("231/ascending/1"); clock.CurrentTime.AddMinutes(8 * 60 + 55);
		using var sun = AuthoredCelestial.Load(db.Celestials.Single(x => x.CelestialType == "ScriptedSun"), world.Object); bodies.Add(sun);
		var seasons = new All<ISeason>();
		foreach (var row in db.Seasons.Where(x => x.Name.StartsWith("armageddon_"))) seasons.Add(new RuntimeSeason(row, world.Object));
		world.SetupGet(x => x.Seasons).Returns(seasons);
		var modelRow = db.ClimateModels.Include(x => x.ClimateModelSeasons).ThenInclude(x => x.SeasonEvents).Single(x => x.Name == "Northern Forest");
		var eventIds = modelRow.ClimateModelSeasons.SelectMany(x => x.SeasonEvents).Select(x => x.WeatherEventId).Distinct().ToArray();
		var weather = new All<IWeatherEvent>();
		foreach (var row in db.WeatherEvents.Where(x => eventIds.Contains(x.Id))) weather.Add(new RuntimeWeather(row, world.Object));
		world.SetupGet(x => x.WeatherEvents).Returns(weather);
		var model = new RuntimeModel(modelRow, world.Object);
		world.SetupGet(x => x.ClimateModels).Returns(new All<IClimateModel> { model });
		var climate = new RuntimeClimate(db.RegionalClimates.Include(x => x.RegionalClimatesSeasons).Single(x => x.Name == "Northern Forest"), world.Object);
		world.SetupGet(x => x.RegionalClimates).Returns(new All<IRegionalClimate> { climate });
		var priorConnection = FMDB.ConnectionString;
		try
		{
			FMDB.ConnectionString = db.Database.GetConnectionString()!;
			using var session = FMDB.BeginIsolatedScope();
			var zone = Mock.Of<IZone>(x => x.Geography == new GeographicCoordinate(0, 0, 0, 6_371_000));
			var controller = new RuntimeController(world.Object, "Native nine-hour controller", climate, zone);
			Require(ReferenceEquals(controller.FeedClock, clock) && ReferenceEquals(controller.Celestial, sun), "Builder creation selected the unrelated first global time context.");
			world.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController> { controller });
			var hours = new HashSet<int>();
			for (var minute = 0; minute < 1080; minute++)
			{
				clock.CurrentTime.AddSeconds(60);
				hours.Add(clock.CurrentTime.Hours);
				var expected = climate.HourlyBaseTemperaturesBySeason[(controller.CurrentSeason, clock.CurrentTime.Hours)] + controller.CurrentTemperatureFluctuation + controller.CurrentWeatherEvent.TemperatureEffect;
				Require(Math.Abs(controller.CurrentTemperature - expected) < 1e-9, "Native hourly temperature lookup failed.");
				Require(controller.CurrentWeatherEvent.Precipitation == PrecipitationLevel.Dry && controller.CurrentWeatherEvent.Wind <= WindLevel.GaleWind, "Native controller selected prohibited weather.");
			}
			Require(hours.Count == 9 && calendar.CurrentDate.Year == 2 && controller.CurrentSeason.Name == "armageddon_0_0", "Native controller did not cross all nine hours and the year boundary.");
			controller.Save(); FMDB.Context.SaveChanges();
			clockRow.Hours = clock.CurrentTime.Hours; clockRow.Minutes = clock.CurrentTime.Minutes; clockRow.Seconds = clock.CurrentTime.Seconds;
			calendarRow.Date = calendar.CurrentDate.GetDateString(); db.SaveChanges();
			using var cold = factory();
			var loaded = new RuntimeController(cold.WeatherControllers.AsNoTracking().Single(x => x.Id == controller.Id), world.Object);
			Require(loaded.CurrentSeason.Id == controller.CurrentSeason.Id && Math.Abs(loaded.CurrentTemperature - controller.CurrentTemperature) < 1e-9 && loaded.ForecastMinute == controller.ForecastMinute,
				"Native controller did not reload its season, temperature and forecast anchor.");
			return (1080, controller.Id);
		}
		finally { FMDB.ConnectionString = priorConnection; }
	}
}
