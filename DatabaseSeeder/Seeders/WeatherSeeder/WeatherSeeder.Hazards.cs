#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class WeatherSeeder
{
	private const string HazardModule = "weather-hazards-v1";

	private string InstallWeatherHazards(FuturemudDatabaseContext context, bool fresh)
	{
		// The reference is disposable and uses the very same canonical base definitions as a fresh install.
		// It is never attached to, or saved over, the user's world.
		using var reference = fresh ? null : new FuturemudDatabaseContext(
			new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase($"weather-reference-{Guid.NewGuid():N}", x => x.EnableNullChecks(false)).Options);
		if (reference is not null)
		{
			reference.Celestials.Add(new Celestial { Definition = "<Celestial />", CelestialType = "reference", FeedClockId = 1 });
			reference.Liquids.Add(new Liquid { Name = "rain water" });
			reference.SaveChanges();
			new WeatherSeeder().SeedBaseData(reference, new Dictionary<string, string> { ["rain"] = "full" });
		}

		var source = reference ?? context;
		var sourceEvents = source.WeatherEvents.ToDictionary(x => x.Id);
		var actualEvents = context.WeatherEvents.ToDictionary(x => x.Id);
		var actualByName = actualEvents.Values.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var sourceSeasons = source.Seasons.ToDictionary(x => x.Id);
		var actualSeasons = context.Seasons.ToDictionary(x => x.Id);
		var actualSeasonsByName = actualSeasons.Values.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var records = context.SeederManagedRecords.Where(x => x.Seeder == Name && x.Module == HazardModule).ToList();
		var conflicts = new List<string>();
		var verified = new Dictionary<long, WeatherEvent>();
		foreach (var candidate in sourceEvents.Values.Where(x => !x.Name.StartsWith("WeatherHazard_", StringComparison.Ordinal)))
		{
			if (!actualByName.TryGetValue(candidate.Name, out var actual)) continue;
			if (fresh || EventSignature(actual, actualEvents, context) == EventSignature(candidate, sourceEvents, source, actual.WeatherEventType))
				verified[candidate.Id] = actual;
		}

		SeederManagedRecord Record(string type, string key, long id)
		{
			var record = records.SingleOrDefault(x => x.EntityType == type && x.StableKey == key);
			if (record is not null) { record.LogicalId = id; return record; }
			record = new() { Seeder = Name, Module = HazardModule, EntityType = type, StableKey = key, LogicalId = id,
				ManifestVersion = "1", AppliedAt = DateTime.UtcNow };
			context.SeederManagedRecords.Add(record);
			records.Add(record);
			return record;
		}

		var dust = InstallDustGases(context, records, Record, conflicts);
		var variants = new Dictionary<(long Id, string Kind), WeatherEvent>();
		foreach (var (sourceId, weather) in verified)
		{
			var kinds = new List<(string Kind, WeatherHazardSettings Settings)>();
			if ((PrecipitationLevel)weather.Precipitation is PrecipitationLevel.HeavyRain or PrecipitationLevel.TorrentialRain)
				kinds.Add(("Lightning", new() { ForecastDescription = "thunderstorms", LightningChance = 0.00002,
					AtmosphericLightningChance = 0.0005, Damage = 150, Pain = 150, Stun = 200, GroundDamageFactor = 0.2 }));
			if (dust is not null && (PrecipitationLevel)weather.Precipitation is PrecipitationLevel.Dry or PrecipitationLevel.Parched &&
				(WindLevel)weather.Wind is WindLevel.StrongWind or WindLevel.GaleWind)
			{
				kinds.Add(("Dust", new() { ForecastDescription = "blowing dust", AtmosphereGasId = dust.Value.Ordinary.Id }));
				kinds.Add(("ChokingDust", new() { ForecastDescription = "a choking dust storm", AtmosphereGasId = dust.Value.Severe.Id }));
			}
			foreach (var (kind, settings) in kinds)
			{
				var name = $"WeatherHazard_{kind}_{weather.Name}";
				var owned = records.SingleOrDefault(x => x.EntityType == "WeatherEvent" && x.StableKey == name);
				if (owned?.LogicalId is { } ownedId && actualEvents.TryGetValue(ownedId, out var ownedEvent))
				{
					variants[(sourceId, kind)] = ownedEvent; // Stable identity also preserves builder renames.
					continue;
				}
				if (actualByName.TryGetValue(name, out var existing))
				{
					if (records.Any(x => x.EntityType == "WeatherEvent" && x.StableKey == name && x.LogicalId == existing.Id))
						variants[(sourceId, kind)] = existing;
					else conflicts.Add($"Preserved unowned weather event {name}.");
					continue;
				}
				var clone = (WeatherEvent)context.Entry(weather).CurrentValues.ToObject();
				clone.Id = owned?.LogicalId ?? 0; // Restore a deleted owned row at its referenced identity.
				clone.Name = name;
				clone.CountsAsId = weather.Id;
				var xml = XElement.Parse(weather.AdditionalInfo);
				xml.Element("Hazards")?.Remove();
				xml.Add(settings.ToXml());
				if (kind != "Lightning")
				{
					clone.WeatherDescription = kind == "Dust" ? "Dust swirls thickly through the air." : "A choking wall of dust blots out the sky.";
					clone.WeatherRoomAddendum = clone.WeatherDescription;
					clone.ObscuresViewOfSky = true;
					clone.LightLevelMultiplier = kind == "Dust" ? 0.7 : 0.3;
					xml.Element("Echoes")?.ReplaceWith(new XElement("Echoes", new XElement("Echo", clone.WeatherDescription)));
					xml.Element("Transitions")?.ReplaceWith(new XElement("Transitions", new XElement("Default", clone.WeatherDescription)));
				}
				else
				{
					clone.WeatherDescription += " Lightning flashes among the storm clouds.";
					clone.WeatherRoomAddendum += " Lightning flashes among the storm clouds.";
				}
				clone.AdditionalInfo = xml.ToString();
				context.WeatherEvents.Add(clone);
				context.SaveChanges();
				Record("WeatherEvent", name, clone.Id);
				actualByName[name] = clone;
				actualEvents[clone.Id] = clone;
				variants[(sourceId, kind)] = clone;
			}
		}

		var updated = new HashSet<long>();
		foreach (var profile in GetClimateProfiles())
		{
			var basis = source.ClimateModels.Include(x => x.ClimateModelSeasons).ThenInclude(x => x.SeasonEvents)
				.SingleOrDefault(x => x.Name == profile.ClimateModelName);
			var actual = context.ClimateModels.Include(x => x.ClimateModelSeasons).ThenInclude(x => x.SeasonEvents)
				.SingleOrDefault(x => x.Name == profile.ClimateModelName);
			if (basis is null || actual is null) continue;
			var record = records.SingleOrDefault(x => x.EntityType == "ClimateModel" && x.StableKey == actual.Name && x.LogicalId == actual.Id);
			var current = ClimateSignature(actual, actualEvents, actualSeasons);
			// Existing managed climates are complete graph units. Any builder change preserves the whole graph.
			if (record is not null && record.SeedBaseline != JsonSerializer.Serialize(new Dictionary<string, string> { ["Graph"] = current }))
			{
				conflicts.Add($"Preserved customised climate {actual.Name}.");
				continue;
			}
			if (!fresh && (record is null && current != ClimateSignature(basis, sourceEvents, sourceSeasons) ||
				basis.ClimateModelSeasons.SelectMany(x => x.SeasonEvents).Any(x => !verified.ContainsKey(x.WeatherEventId))))
			{
				conflicts.Add($"Preserved unverified or customised climate {actual.Name}.");
				continue;
			}
			var arid = profile.KoppenClimateClassification.StartsWith("B", StringComparison.Ordinal);
			var polar = profile.KoppenClimateClassification.StartsWith("E", StringComparison.Ordinal);
			List<(WeatherEvent Event, double Weight)> Choices(long id)
			{
				var choices = new List<(WeatherEvent, double)>();
				if (!polar && variants.TryGetValue((id, "Lightning"), out var lightning)) choices.Add((lightning, 0.08));
				if (arid && variants.TryGetValue((id, "Dust"), out var ordinary)) choices.Add((ordinary, 0.08));
				if (arid && variants.TryGetValue((id, "ChokingDust"), out var severe)) choices.Add((severe, 0.01));
				choices.Add((verified[id], 1.0 - choices.Sum(x => x.Item2)));
				return choices;
			}
			// A prior install may have lacked stock air or met an unowned naming collision.
			// Fill newly available variants only while the complete managed graph is unchanged.
			if (record is not null && basis.ClimateModelSeasons.All(season =>
			{
				var destination = actual.ClimateModelSeasons.Single(x => actualSeasons[x.SeasonId].Name == sourceSeasons[season.SeasonId].Name);
				return season.SeasonEvents.All(row => Choices(row.WeatherEventId).All(choice => destination.SeasonEvents.Any(x => x.WeatherEventId == choice.Event.Id)));
			})) continue;
			foreach (var season in basis.ClimateModelSeasons.ToArray())
			{
				var actualSeason = actualSeasonsByName[sourceSeasons[season.SeasonId].Name];
				var destination = actual.ClimateModelSeasons.Single(x => x.SeasonId == actualSeason.Id);
				foreach (var entry in season.SeasonEvents.ToArray())
				{
					var transitions = XElement.Parse(entry.Transitions);
					foreach (var transition in transitions.Elements().ToArray())
					{
						var id = (long)transition.Attribute("id")!;
						var weight = (double)transition.Attribute("chance")!;
						transition.ReplaceWith(Choices(id).Select(x => new XElement("Transition", new XAttribute("id", x.Event.Id), new XAttribute("chance", weight * x.Weight))));
					}
					var xml = transitions.ToString();
					var baseEvent = verified[entry.WeatherEventId];
					var baseRow = destination.SeasonEvents.Single(x => x.WeatherEventId == baseEvent.Id);
					baseRow.Transitions = xml;
					foreach (var variant in Choices(entry.WeatherEventId).Where(x => x.Event.Id != baseEvent.Id))
					{
						var variantRow = destination.SeasonEvents.SingleOrDefault(x => x.WeatherEventId == variant.Event.Id);
						if (variantRow is null)
							destination.SeasonEvents.Add(new ClimateModelSeasonEvent { ClimateModel = actual, Season = actualSeason,
								WeatherEvent = variant.Event, ChangeChance = entry.ChangeChance, Transitions = xml });
						else variantRow.Transitions = xml;
					}
				}
			}
			context.SaveChanges();
			record = Record("ClimateModel", actual.Name, actual.Id);
			SeederManagedRecordReconciler.Reconcile(record, new Dictionary<string, string>(),
				new Dictionary<string, string> { ["Graph"] = ClimateSignature(actual, actualEvents, actualSeasons) }, true, conflicts);
			updated.Add(actual.Id);
		}
		// Stale, previously generated schedules must not mask the newly installed climate transitions.
		foreach (var controller in context.WeatherControllers.Include(x => x.RegionalClimate).Where(x => updated.Contains(x.RegionalClimate.ClimateModelId)))
			controller.ForecastState = null!;
		context.SaveChanges();
		Console.WriteLine($"[Weather Seeder] Hazard update: {updated.Count} stock climates updated; {conflicts.Count} preserved customisations/collisions.");
		foreach (var conflict in conflicts) Console.WriteLine($"[Weather Seeder] {conflict}");
		return string.Empty;
	}

	private static string EventSignature(WeatherEvent weather, IReadOnlyDictionary<long, WeatherEvent> events,
		FuturemudDatabaseContext context, string? targetType = null)
	{
		var fields = ScalarFields(weather, "Id", "CountsAsId", "AdditionalInfo");
		fields["CountsAs"] = weather.CountsAsId is { } id ? events.GetValueOrDefault(id)?.Name ?? $"missing:{id}" : "";
		var xml = XElement.Parse(weather.AdditionalInfo);
		if (targetType == "simple" && weather.WeatherEventType == "rain")
		{
			fields["WeatherEventType"] = "simple";
			xml.Element("Liquid")?.Remove();
		}
		if (xml.Element("Liquid") is { } liquid) liquid.Value = context.Liquids.Find((long)liquid)?.Name ?? "missing";
		if (xml.Element("Hazards") is null) xml.Add(new WeatherHazardSettings().ToXml());
		fields["AdditionalInfo"] = xml.ToString(SaveOptions.DisableFormatting);
		return JsonSerializer.Serialize(fields);
	}

	private static SortedDictionary<string, string> ScalarFields<T>(T value, params string[] exclude) where T : class => new(
		typeof(T).GetProperties().Where(x => !exclude.Contains(x.Name) &&
			(x.PropertyType.IsValueType || x.PropertyType == typeof(string)))
			.ToDictionary(x => x.Name, x => Convert.ToString(x.GetValue(value), CultureInfo.InvariantCulture) ?? ""), StringComparer.Ordinal);

	private static string ClimateSignature(ClimateModel climate, IReadOnlyDictionary<long, WeatherEvent> events, IReadOnlyDictionary<long, Season> seasons)
	{
		var fields = ScalarFields(climate, "Id");
		foreach (var season in climate.ClimateModelSeasons.OrderBy(x => seasons[x.SeasonId].Name, StringComparer.Ordinal))
		{
			var name = seasons[season.SeasonId].Name;
			fields[$"Season:{name}"] = JsonSerializer.Serialize(ScalarFields(season, "ClimateModelId", "SeasonId"));
			foreach (var entry in season.SeasonEvents)
			{
				var transitions = XElement.Parse(entry.Transitions);
				foreach (var element in transitions.Elements())
					element.SetAttributeValue("id", events.GetValueOrDefault((long)element.Attribute("id")!)?.Name ?? "missing");
				fields[$"Event:{name}:{events[entry.WeatherEventId].Name}"] = $"{entry.ChangeChance:R}|{transitions.ToString(SaveOptions.DisableFormatting)}";
			}
		}
		return JsonSerializer.Serialize(fields);
	}

	private (Gas Ordinary, Gas Severe)? InstallDustGases(FuturemudDatabaseContext context, List<SeederManagedRecord> records,
		Func<string, string, long, SeederManagedRecord> record, List<string> conflicts)
	{
		var air = context.Gases.FirstOrDefault(x => x.Name == "Breathable Atmosphere") ??
			context.Gases.FirstOrDefault(x => x.Name == "air");
		if (air is null) { conflicts.Add("Dust requires the stock Breathable Atmosphere gas (or legacy air); dust installation was skipped."); return null; }
		var ownedDrug = records.SingleOrDefault(x => x.EntityType == "Drug" && x.StableKey == "Weather Dust Irritant");
		var drug = ownedDrug?.LogicalId is { } drugId ? context.Drugs.Find(drugId) : null;
		drug ??= context.Drugs.FirstOrDefault(x => x.Name == "Weather Dust Irritant");
		if (drug is not null && !records.Any(x => x.EntityType == "Drug" && x.LogicalId == drug.Id))
		{ conflicts.Add("Preserved unowned Weather Dust Irritant drug; dust installation was skipped."); return null; }
		if (drug is null)
		{
			drug = new() { Id = ownedDrug?.LogicalId ?? 0, Name = "Weather Dust Irritant", DrugVectors = (int)DrugVector.Inhaled,
				IntensityPerGram = 1, RelativeMetabolisationRate = 0.2 };
			drug.DrugsIntensities.Add(new() { DrugType = (int)DrugType.Respiration, RelativeIntensity = 1,
				AdditionalEffects = new RespirationAdditionalInfo { BreathingDriveMultiplier = 0.75, HypoxiaDamageMultiplier = 1.2,
					AirwayToleranceMultiplier = 0.7 }.DatabaseString });
			context.Drugs.Add(drug);
			context.SaveChanges();
			record("Drug", drug.Name, drug.Id);
		}
		Gas? Gas(bool severe)
		{
			var name = severe ? "choking dust" : "dusty air";
			var owned = records.SingleOrDefault(x => x.EntityType == "Gas" && x.StableKey == name);
			var existing = owned?.LogicalId is { } id ? context.Gases.Find(id) : null;
			existing ??= context.Gases.FirstOrDefault(x => x.Name == name);
			if (existing is not null)
			{
				if (records.Any(x => x.EntityType == "Gas" && x.LogicalId == existing.Id)) return existing;
				conflicts.Add($"Preserved unowned gas {name}; dust installation was skipped.");
				return null;
			}
			var gas = (Gas)context.Entry(air).CurrentValues.ToObject();
			gas.Id = owned?.LogicalId ?? 0;
			gas.Name = name;
			gas.Description = severe ? "thick, choking dust" : "dust-laden air";
			gas.CountAsId = severe ? null : air.Id;
			gas.CountsAsQuality = severe ? null : (int)ItemQuality.Standard;
			gas.DrugId = drug.Id;
			gas.DrugGramsPerUnitVolume = 0.02;
			gas.SmellText = "a dry, dusty smell";
			gas.VagueSmellText = "a dusty smell";
			gas.SmellIntensity = 1;
			context.Gases.Add(gas);
			context.SaveChanges();
			record("Gas", name, gas.Id);
			return gas;
		}
		var ordinary = Gas(false);
		var severe = Gas(true);
		return ordinary is not null && severe is not null ? (ordinary, severe) : null;
	}
}
