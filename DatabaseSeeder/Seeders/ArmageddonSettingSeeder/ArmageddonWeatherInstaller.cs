#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using EngineCompiler::MudSharp.Celestial.Authored;

namespace DatabaseSeeder.Seeders;

/// <summary>Editable fictional regional weather; no zone assignments or controller retargeting.</summary>
public static class ArmageddonWeatherInstaller
{
	internal const string Module = "setting-weather-v1";
	internal sealed record Profile(string Key, string Name, double WarmMin, double WarmMax, double CoolMin,
		double CoolMax, double StandardDeviation, double StrongWindWeight, double DustWeight);
	internal static readonly Profile[] Profiles =
	[
		new("southern_desert", "Southern Desert", 25, 50, 10, 35, 1.5, .09, .055),
		new("salt_flats", "Salt Flats", 28, 55, 12, 40, 2, .14, .10),
		new("northern_scrubland", "Northern Scrubland", 20, 42, 8, 28, 1, .045, .022),
		new("northern_forest", "Northern Forest", 16, 32, 5, 23, .8, .015, .004)
	];
	internal static readonly double[] Warmth = [1, .6, .2, 0, 0, 0, .2, .6, 1];
	internal static readonly double[] Heating = [.15, 0, .15, .45, .8, 1, .8, .45, .25];
	internal static readonly string[] Arcs = ["Descending Sun", "Low Sun", "Ascending Sun"];
	private static readonly string[] Parts = ["Early", "Mid", "Late"];
	internal static string SeasonKey(int index) => $"arm.weather.season.{index}";
	internal static string SeasonName(int index) => $"armageddon_{index / 3}_{index % 3}";
	internal static string SeasonDisplay(int index) => $"{Parts[index % 3]} {Arcs[index / 3]}";

	internal static string DefaultAnswer(FuturemudDatabaseContext db, IReadOnlyDictionary<string, string> answers)
	{
		try { _ = Require(db); return "yes"; }
		catch (Exception error) when (error is InvalidOperationException or ArgumentException or System.Xml.XmlException or JsonException or FormatException or OverflowException) { return "no"; }
	}

	internal static (bool Success, string error) ValidateAnswer(string answer, FuturemudDatabaseContext db)
	{
		if (answer.Trim().Equals("no", StringComparison.OrdinalIgnoreCase)) return (true, "");
		if (!answer.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase)) return (false, "Answer yes or no.");
		try { _ = Require(db); return (true, ""); }
		catch (Exception error) when (error is InvalidOperationException or ArgumentException or System.Xml.XmlException or JsonException or FormatException or OverflowException) { return (false, error.Message); }
	}

	internal static (SeederWeatherTiming Timing, IReadOnlyDictionary<string, WeatherEvent> Events) Require(FuturemudDatabaseContext db)
	{
		var calendar = CelestialSeeder.RequireArmageddonCalendar(db);
		var suns = db.Celestials.AsEnumerable().Where(x => x.CelestialType == "ScriptedSun")
			.Select(x => (Row: x, Source: AuthoredCelestialFormat.Parse(x.Definition)))
			.Where(x => x.Source.SeederPreset == "ArmageddonSun" && x.Source.CalendarId == calendar.Id &&
				x.Source.TimeOfDay).ToArray();
		if (suns.Length != 1) throw new InvalidOperationException("Armageddon weather requires the Suk-Krath celestial package.");
		var timing = SeederWeatherTiming.Resolve(db, new Dictionary<string, string> { ["weathercelestial"] = suns[0].Row.Id.ToString() });
		if (timing.HoursPerDay != 9 || timing.MinutesPerHour != 60 || timing.YearDays != 693 || suns[0].Row.FeedClockId != calendar.FeedClockId)
			throw new InvalidOperationException("Armageddon weather requires the compatible nine-hour clock and 693-day Suk-Krath year.");
		PreflightNames<Season>(db, Enumerable.Range(0, 9).Select(i => (SeasonKey(i), SeasonName(i))));
		PreflightNames<ClimateModel>(db, Profiles.Select(x => ($"arm.weather.model.{x.Key}", x.Name)));
		PreflightNames<RegionalClimate>(db, Profiles.Select(x => ($"arm.weather.region.{x.Key}", x.Name)));
		var events = WeatherSeeder.RequireDryWeatherFoundations(db);
		PreflightRows(db, events);
		return (timing, events);
	}

	private static void PreflightRows(FuturemudDatabaseContext db, IReadOnlyDictionary<string, WeatherEvent> events)
	{
		void Row<T>(string key, params object[] keys) where T : class
		{
			var record = Claim(db, key);
			if (record is null)
			{
				if (db.Find<T>(keys) is not null) throw new InvalidOperationException($"Unowned weather row collision: {key}.");
				return;
			}
			ValidateClaim<T>(db, record);
			if (record.LogicalId != (long)keys[0]) throw new InvalidOperationException($"Invalid weather row identity: {key}.");
		}
		foreach (var profile in Profiles)
		{
			var modelId = Claim(db, $"arm.weather.model.{profile.Key}")?.LogicalId;
			var regionId = Claim(db, $"arm.weather.region.{profile.Key}")?.LogicalId;
			var claims = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToArray();
			for (var i = 0; i < 9; i++)
			{
				var prefix = $"arm.weather.graph.{profile.Key}.{i}";
				var temperatureKey = $"arm.weather.temperatures.{profile.Key}.{i}";
				var graphClaims = claims.Any(x => x.StableKey == prefix || x.StableKey.StartsWith(prefix + ".", StringComparison.Ordinal));
				var temperatureClaim = claims.Any(x => x.StableKey == temperatureKey);
				var seasonIdentity = Claim(db, SeasonKey(i))?.LogicalId;
				if ((graphClaims && modelId is null) || (temperatureClaim && regionId is null) ||
					((graphClaims || temperatureClaim) && seasonIdentity is null))
					throw new InvalidOperationException($"Weather dependent claims require intact principal ownership: {prefix}.");
				if (seasonIdentity is not { } seasonId) continue;
				if (modelId is { } model)
				{
					Row<ClimateModelSeason>(prefix, model, seasonId);
					foreach (var (name, weather) in events) Row<ClimateModelSeasonEvent>($"{prefix}.{name}", model, seasonId, weather.Id);
				}
				if (regionId is { } region) Row<RegionalClimatesSeason>(temperatureKey, region, seasonId);
			}
		}
	}

	private static void ValidateClaim<T>(FuturemudDatabaseContext db, SeederManagedRecord record) where T : class
	{
		if (record.Module != Module || record.EntityType != typeof(T).Name || record.Retired || record.LogicalId is not > 0 || record.SeedBaseline is null)
			throw new InvalidOperationException($"Invalid Armageddon weather ownership: {record.StableKey}.");
		try
		{
			var fields = db.Model.FindEntityType(typeof(T))!.GetProperties().Where(x => !x.IsPrimaryKey()).ToDictionary(x => x.Name);
			var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline) ?? throw new JsonException("Missing baseline.");
			foreach (var (key, value) in baseline)
			{
				if (!fields.TryGetValue(key, out var field)) throw new JsonException("Unknown baseline field.");
				_ = JsonSerializer.Deserialize(value, field.ClrType);
			}
		}
		catch (JsonException error) { throw new InvalidOperationException($"Invalid weather field baseline: {record.StableKey}.", error); }
	}

	private static void PreflightNames<T>(FuturemudDatabaseContext db, IEnumerable<(string Key, string Name)> names) where T : class
	{
		foreach (var (key, name) in names)
		{
			var record = Claim(db, key);
			if (record is not null) ValidateClaim<T>(db, record);
			if (record is not null && db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
				throw new InvalidOperationException($"Competing weather principal identity: {key}.");
			foreach (var entity in db.Set<T>().AsEnumerable().Where(x => string.Equals((string?)db.Entry(x).Property("Name").CurrentValue, name, StringComparison.OrdinalIgnoreCase)))
				if (record?.LogicalId != (long)db.Entry(entity).Property("Id").CurrentValue!)
					throw new InvalidOperationException($"Unowned {typeof(T).Name} name collision: {name}.");
		}
	}

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory)
	{
		using var db = factory();
		if (db.ChangeTracker.HasChanges() || db.Database.CurrentTransaction is not null)
			throw new InvalidOperationException("Use a fresh weather installation context.");
		var (timing, events) = Require(db); // All prerequisites and name collisions before the first write.
		using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
		var messages = new List<string>();
		var identities = new Dictionary<string, long>();
		var seasons = Enumerable.Range(0, 9).Select(i => Put(db, SeasonKey(i), new Season
		{
			Name = SeasonName(i), DisplayName = SeasonDisplay(i), SeasonGroup = Arcs[i / 3],
			CelestialId = timing.CelestialId, CelestialDayOnset = i * 77
		}, messages)).ToArray();
		foreach (var profile in Profiles)
		{
			var modelKey = $"arm.weather.model.{profile.Key}";
			var model = Put(db, modelKey, new ClimateModel
			{
				Name = profile.Name, Type = "terrestrial", MinuteProcessingInterval = 10,
				MinimumMinutesBetweenFlavourEchoes = 60, MinuteFlavourEchoChance = .01,
				Description = $"{profile.Name}: editable fictional Zalanthan weather. Dry conditions with regional wind and dust; no routine precipitation or winds above Gale. All sun arcs share the same wind and dust weights."
			}, messages);
			identities[modelKey] = model.Id;
			var regional = Put(db, $"arm.weather.region.{profile.Key}", new RegionalClimate
			{
				Name = profile.Name, ClimateModelId = model.Id,
				TemperatureFluctuationStandardDeviation = profile.StandardDeviation,
				TemperatureFluctuationPeriodMinutes = timing.DaysToMinutes(4),
				Description = $"{profile.Name}: authored fictional defaults, not recovered historical measurements. Warm daily band {profile.WarmMin}–{profile.WarmMax}°C; cool band {profile.CoolMin}–{profile.CoolMax}°C. Dawn is coolest; early afternoon hottest. Nine 77-day seasonal blocks follow the shared sun arcs. Builders may explicitly select opposite-hemisphere timing."
			}, messages);
			var weights = Weights(profile, events);
			for (var i = 0; i < seasons.Length; i++)
			{
				var season = seasons[i];
				var prefix = $"arm.weather.graph.{profile.Key}.{i}";
				Put(db, prefix, new ClimateModelSeason
				{
					ClimateModelId = model.Id, SeasonId = season.Id,
					MaximumAdditionalChangeChanceFromStableWeather = 0,
					IncrementalAdditionalChangeChanceFromStableWeather = 0
				}, messages);
				// A lazy weighted resampling chain has the authored weights as its stationary occupancy.
				// The canonical writer folds each self weight into ChangeChance, preserving residence time.
				foreach (var weather in weights.Keys)
					Put(db, $"{prefix}.{weather.Key}", WeatherSeeder.CreateSeasonEvent(model, season, weather.Value,
						weights.Select(x => (x.Key.Value.Id, x.Value)), .08), messages);
				var minimum = profile.CoolMin + (profile.WarmMin - profile.CoolMin) * Warmth[i];
				var maximum = profile.CoolMax + (profile.WarmMax - profile.CoolMax) * Warmth[i];
				Put(db, $"arm.weather.temperatures.{profile.Key}.{i}", new RegionalClimatesSeason
				{
					RegionalClimateId = regional.Id, SeasonId = season.Id,
					TemperatureInfo = timing.Temperatures(minimum, maximum, Heating).ToString()
				}, messages);
			}
		}
		db.SaveChanges();
		transaction?.Commit();
		messages.Add("Four Armageddon regional weather templates installed. Assign them to weather controllers explicitly; existing controllers and zones were retained.");
		return new(ArmageddonInstallStatus.Completed, messages, identities);
	}

	private static Dictionary<KeyValuePair<string, WeatherEvent>, double> Weights(Profile profile, IReadOnlyDictionary<string, WeatherEvent> events)
	{
		var ordinaryStrong = profile.StrongWindWeight - profile.DustWeight;
		var weak = 1 - profile.StrongWindWeight;
		return new Dictionary<string, double>
		{
			["DryStill"] = weak * .40, ["DryBreeze"] = weak * .40, ["DryWind"] = weak * .20,
			["DryStrongWind"] = ordinaryStrong * .8, ["DryGaleWind"] = ordinaryStrong * .2,
			["WeatherHazard_Dust_DryStrongWind"] = profile.DustWeight * .64,
			["WeatherHazard_Dust_DryGaleWind"] = profile.DustWeight * .16,
			["WeatherHazard_ChokingDust_DryStrongWind"] = profile.DustWeight * .16,
			["WeatherHazard_ChokingDust_DryGaleWind"] = profile.DustWeight * .04
		}.ToDictionary(x => new KeyValuePair<string, WeatherEvent>(x.Key, events[x.Key]), x => x.Value);
	}

	private static SeederManagedRecord? Claim(FuturemudDatabaseContext db, string key) => db.SeederManagedRecords.Local
		.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key) ??
		db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);

	/// <summary>Scalar field reconciliation also supports the existing composite-key weather tables.</summary>
	private static T Put<T>(FuturemudDatabaseContext db, string key, T desired, ICollection<string> messages) where T : class
	{
		var entry = db.Entry(desired);
		var primaryKey = entry.Metadata.FindPrimaryKey()!.Properties;
		var record = Claim(db, key);
		var hasId = primaryKey.Count == 1 && primaryKey[0].Name == "Id";
		if (record is not null && (record.Module != Module || record.EntityType != typeof(T).Name || record.Retired || record.SeedBaseline is null))
			throw new InvalidOperationException($"Invalid Armageddon weather claim: {key}.");
		if (hasId && record?.LogicalId is { } id) entry.Property("Id").CurrentValue = id;
		var keys = primaryKey.Select(x => entry.Property(x.Name).CurrentValue).ToArray();
		if (record is not null && record.LogicalId != (long)keys[0]!)
			throw new InvalidOperationException($"Invalid weather row identity: {key}.");
		var actual = hasId && record is null ? null : db.Set<T>().Find(keys);
		if (actual is not null && record is null) throw new InvalidOperationException($"Unowned weather row collision: {key}.");
		var fields = entry.Metadata.GetProperties().Where(x => !x.IsPrimaryKey()).ToArray();
		var values = fields.ToDictionary(x => x.Name, x => JsonSerializer.Serialize(entry.Property(x.Name).CurrentValue, x.ClrType));
		var created = actual is null;
		if (actual is null)
		{
			actual = desired;
			// Navigations are supplied by the canonical writer; attach only this row's scalar values.
			db.Entry(actual).State = EntityState.Added;
			if (hasId) db.SaveChanges();
		}
		else entry.State = EntityState.Detached;
		if (record is null)
		{
			record = new SeederManagedRecord
			{
				Seeder = ArmageddonMagicInstaller.Package, Module = Module, StableKey = key, EntityType = typeof(T).Name,
				LogicalId = hasId ? (long)db.Entry(actual).Property("Id").CurrentValue! : (long)keys[0]!, ManifestVersion = "1", AppliedAt = DateTime.UtcNow
			};
			db.SeederManagedRecords.Add(record);
		}
		var current = fields.ToDictionary(x => x.Name, x => JsonSerializer.Serialize(db.Entry(actual).Property(x.Name).CurrentValue, x.ClrType));
		var merged = SeederManagedRecordReconciler.Reconcile(record, current, values, created, messages);
		foreach (var field in fields)
			db.Entry(actual).Property(field.Name).CurrentValue = JsonSerializer.Deserialize(merged[field.Name], field.ClrType);
		record.AppliedAt = DateTime.UtcNow;
		return actual;
	}
}
