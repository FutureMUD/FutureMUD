#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class WeatherSeeder
{
	/// <summary>The canonical row writer, including collapsed self-transition probability.</summary>
	internal static ClimateModelSeasonEvent CreateSeasonEvent(ClimateModel model, Season season, WeatherEvent weather,
		IEnumerable<(long EventId, double Chance)> transitions, double baseChangeChance)
	{
		var definition = BuildTransitionDefinition(weather.Id, transitions, baseChangeChance);
		return new ClimateModelSeasonEvent
		{
			ClimateModel = model, Season = season, WeatherEvent = weather,
			ClimateModelId = model.Id, SeasonId = season.Id, WeatherEventId = weather.Id,
			ChangeChance = definition.EffectiveChangeChance, Transitions = definition.TransitionXml
		};
	}

	/// <summary>Read-only verification of the canonical dry catalogue used by setting packages.</summary>
	internal static IReadOnlyDictionary<string, WeatherEvent> RequireDryWeatherFoundations(FuturemudDatabaseContext db)
	{
		using var reference = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase($"weather-catalogue-{Guid.NewGuid():N}", x => x.EnableNullChecks(false)).Options);
		var catalog = new WeatherSeeder().CreateWeatherEvents(reference);
		var expected = catalog.EventsByName.Values.ToDictionary(x => x.Id);
		var actual = db.WeatherEvents.ToDictionary(x => x.Id);
		var result = new Dictionary<string, WeatherEvent>(StringComparer.Ordinal);
		foreach (var wind in new[] { WindLevel.Still, WindLevel.Breeze, WindLevel.Wind, WindLevel.StrongWind, WindLevel.GaleWind })
		{
			var name = $"Dry{wind}";
			var source = catalog.EventsByName[name];
			var candidates = actual.Values.Where(x => x.Name == name).ToArray();
			if (candidates.Length != 1 || EventSignature(candidates[0], actual, db) != EventSignature(source, expected, reference))
				throw new InvalidOperationException($"Armageddon weather requires verified canonical weather event {name}. Install Weather Seeder first.");
			result[name] = candidates[0];
		}
		foreach (var (kind, gasName) in new[] { ("Dust", "dusty air"), ("ChokingDust", "choking dust") })
		foreach (var wind in new[] { WindLevel.StrongWind, WindLevel.GaleWind })
		{
			var name = $"WeatherHazard_{kind}_Dry{wind}";
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == "Weather Seeder" && x.Module == HazardModule &&
				x.EntityType == "WeatherEvent" && x.StableKey == name && !x.Retired);
			var weather = record?.LogicalId is { } id ? actual.GetValueOrDefault(id) : null;
			var gasRecord = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == "Weather Seeder" && x.Module == HazardModule &&
				x.EntityType == "Gas" && x.StableKey == gasName && !x.Retired);
			var gas = gasRecord?.LogicalId is { } gasId ? db.Gases.Find(gasId) : null;
			var hazards = weather is null ? null : WeatherHazardSettings.FromXml(XElement.Parse(weather.AdditionalInfo).Element("Hazards"));
			if (weather is null || gas is null || weather.CountsAsId != result[$"Dry{wind}"].Id ||
				weather.Precipitation != (int)PrecipitationLevel.Dry || weather.Wind != (int)wind ||
				weather.WeatherEventType != "simple" ||
				hazards?.AtmosphereGasId != gas.Id || hazards.LightningChance != 0 || hazards.AtmosphericLightningChance != 0 ||
				weather.TemperatureEffect != 0 || !weather.PermittedAtNight || !weather.PermittedAtDawn ||
				!weather.PermittedAtMorning || !weather.PermittedAtAfternoon || !weather.PermittedAtDusk)
				throw new InvalidOperationException($"Armageddon weather requires verified shared dust hazard {name}. Run the Weather Seeder hazards update first.");
			result[name] = weather;
		}
		if (result.Values.Select(x => x.Id).Distinct().Count() != result.Count)
			throw new InvalidOperationException("Shared weather and dust records must retain distinct event identities.");
		return result;
	}
}
