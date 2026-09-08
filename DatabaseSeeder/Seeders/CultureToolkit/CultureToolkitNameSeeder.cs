#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureNameResolution(string SourceKey, string Era, long NameCultureId,
	IReadOnlyList<long> ProfileIds, IReadOnlyDictionary<int, int> GivenCounts, IReadOnlyList<string> Exclusions);

/// <summary>Local name identities and each profile member use three-way reconciliation. Existing links need a baseline.</summary>
public static class CultureToolkitNameSeeder
{
	private static readonly IReadOnlyDictionary<string, string> EthnicityKeys = new Dictionary<string, string>
	{
		["names.target.finnish"] = "ethnicity.finnish", ["names.target.lithuanian"] = "ethnicity.lithuanian",
		["names.target.latvian"] = "ethnicity.latvian", ["names.target.estonian"] = "ethnicity.estonian",
		["names.target.old-prussian"] = "ethnicity.old-prussian", ["names.target.romanian"] = "ethnicity.vlach",
		["names.target.coptic-christian"] = "ethnicity.copt", ["names.target.syriac-christian"] = "ethnicity.syriac"
	};

	public static IReadOnlyList<CultureNameResolution> Upsert(FuturemudDatabaseContext context,
		CultureToolkitCatalogue catalogue, string era, IReadOnlyDictionary<string, Ethnicity> ethnicities,
		FutureProg suggestionsProg, ICollection<string> conflicts,
		IReadOnlyDictionary<(long EthnicityId, short Gender), long>? verifiedOriginalNameCultures = null)
	{
		var results = new List<CultureNameResolution>();
		foreach (var repertoire in CultureToolkitNameCatalogue.Build(catalogue, era))
		{
			var local = UpsertCulture(context, era, repertoire.StableKey, repertoire.Culture, suggestionsProg, conflicts);
			var neutral = UpsertCulture(context, era, repertoire.StableKey + ".shared",
				CultureToolkitNameCatalogue.NeutralRepertoire(repertoire), suggestionsProg, conflicts);
			var exclusions = repertoire.Exclusions.ToList();
			if (ethnicities.TryGetValue(EthnicityKeys[repertoire.StableKey], out var ethnicity))
			{
				foreach (var gender in repertoire.Culture.RandomNameProfiles.Select(x => (short)x.Gender))
					Bind(context, era, repertoire.StableKey, ethnicity, gender, local, conflicts, verifiedOriginalNameCultures);
				foreach (var gender in new[] { Gender.NonBinary, Gender.Indeterminate, Gender.Neuter })
					Bind(context, era, repertoire.StableKey, ethnicity, (short)gender, neutral, conflicts, verifiedOriginalNameCultures);
			}
			else exclusions.Add($"{EthnicityKeys[repertoire.StableKey]}: heritage not installed; no new ethnicity link.");
			foreach (var (key, culture) in new[] { (repertoire.StableKey, local), (repertoire.StableKey + ".shared", neutral) })
				results.Add(new CultureNameResolution(key, era, culture.Id, culture.RandomNameProfiles.Select(x => x.Id).ToArray(),
					culture.RandomNameProfiles.GroupBy(x => x.Gender).ToDictionary(x => x.Key,
						x => x.SelectMany(p => p.RandomNameProfilesElements).Count(e => e.NameUsage == 0)), exclusions));
		}
		context.SaveChanges();
		return results;
	}

	private static NameCulture UpsertCulture(FuturemudDatabaseContext context, string era, string key,
		NameCulture desired, FutureProg suggestionsProg, ICollection<string> conflicts)
	{
		var record = CultureToolkitManagedEntities.Find(context, "NameCulture", key);
		var fresh = record is null;
		var model = fresh ? new NameCulture() : context.NameCultures.Include(x => x.RandomNameProfiles)
			.ThenInclude(x => x.RandomNameProfilesElements).Include(x => x.RandomNameProfiles)
			.ThenInclude(x => x.RandomNameProfilesDiceExpressions).Single(x => x.Id == record!.LogicalId);
		if (fresh)
		{
			if (context.NameCultures.Any(x => x.Name == desired.Name))
				throw new InvalidOperationException($"Unowned local name-culture collision: {desired.Name}");
			model.Name = desired.Name;
			model.Definition = desired.Definition;
			context.NameCultures.Add(model);
			context.SaveChanges();
		}
		var merged = CultureToolkitManagedEntities.Reconcile(context, era, "NameCulture", key, model.Id, fresh,
			new Dictionary<string, string> { ["name"] = model.Name, ["definition"] = model.Definition },
			new Dictionary<string, string> { ["name"] = desired.Name, ["definition"] = desired.Definition }, conflicts);
		model.Name = merged["name"];
		model.Definition = merged["definition"];
		foreach (var profile in desired.RandomNameProfiles)
			UpsertProfile(context, era, key + $".gender.{profile.Gender}", model, profile, suggestionsProg, conflicts);
		return model;
	}

	internal static RandomNameProfile UpsertProfile(FuturemudDatabaseContext context, string era, string key, NameCulture culture,
		RandomNameProfile desired, FutureProg? suggestionsProg, ICollection<string> conflicts,
		RandomNameProfile? bound = null, RandomNameProfile? verifiedSourceBaseline = null)
	{
		var record = CultureToolkitManagedEntities.Find(context, "RandomNameProfile", key);
		var fresh = record is null && bound is null;
		var model = fresh ? new RandomNameProfile { NameCulture = culture, Name = desired.Name, Gender = desired.Gender } : record is null ? bound! :
			context.RandomNameProfiles.Include(x => x.RandomNameProfilesElements).Include(x => x.RandomNameProfilesDiceExpressions)
				.Single(x => x.Id == record!.LogicalId);
		if (!fresh && context.Entry(model).State == EntityState.Detached)
			throw new InvalidOperationException($"Profile binding {key} must be a tracked installed row.");
		if (bound is not null && bound.Id != model.Id) throw new InvalidOperationException($"Conflicting source profile binding: {key}");
		if (fresh)
		{
			context.RandomNameProfiles.Add(model);
			context.SaveChanges();
		}
		desired.NameCultureId = culture.Id;
		desired.UseForChargenSuggestionsProgId = suggestionsProg?.Id;
		if (record is null && !fresh && verifiedSourceBaseline is not null)
		{
			context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = "CultureSeeder", Module = era, EntityType = "RandomNameProfile", StableKey = key, LogicalId = model.Id,
				ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow, SeedBaseline = JsonSerializer.Serialize(Fields(verifiedSourceBaseline))
			});
			context.SaveChanges();
		}
		var merged = CultureToolkitManagedEntities.Reconcile(context, era, "RandomNameProfile", key, model.Id, fresh,
			Fields(model), Fields(desired), conflicts);
		model.Name = merged["name"];
		model.Gender = int.Parse(merged["gender"], CultureInfo.InvariantCulture);
		model.NameCultureId = long.Parse(merged["culture"], CultureInfo.InvariantCulture);
		model.UseForChargenSuggestionsProgId = long.TryParse(merged["suggestions"], out var progId) ? progId : null;
		foreach (var row in model.RandomNameProfilesElements.ToArray())
		{
			var field = ElementKey(row);
			if (merged.TryGetValue(field, out var weight)) row.Weighting = int.Parse(weight, CultureInfo.InvariantCulture);
			else { model.RandomNameProfilesElements.Remove(row); context.RandomNameProfilesElements.Remove(row); }
		}
		var existingElements = model.RandomNameProfilesElements.Select(ElementKey).ToHashSet(StringComparer.Ordinal);
		foreach (var field in merged.Where(x => x.Key.StartsWith("element:", StringComparison.Ordinal)))
		{
			if (!existingElements.Add(field.Key)) continue;
			var parts = JsonSerializer.Deserialize<string[]>(field.Key[8..])!;
			model.RandomNameProfilesElements.Add(new RandomNameProfilesElements
			{
				NameUsage = int.Parse(parts[0], CultureInfo.InvariantCulture), Name = parts[1], Weighting = int.Parse(field.Value, CultureInfo.InvariantCulture)
			});
		}
		foreach (var row in model.RandomNameProfilesDiceExpressions.ToArray())
		{
			if (merged.TryGetValue($"dice:{row.NameUsage}", out var dice)) row.DiceExpression = dice;
			else { model.RandomNameProfilesDiceExpressions.Remove(row); context.RandomNameProfilesDiceExpressions.Remove(row); }
		}
		foreach (var field in merged.Where(x => x.Key.StartsWith("dice:", StringComparison.Ordinal)))
		{
			var usage = int.Parse(field.Key[5..], CultureInfo.InvariantCulture);
			if (model.RandomNameProfilesDiceExpressions.All(x => x.NameUsage != usage))
				model.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions { NameUsage = usage, DiceExpression = field.Value });
		}
		context.SaveChanges();
		return model;
	}

	private static string ElementKey(RandomNameProfilesElements row) => "element:" + JsonSerializer.Serialize(new[]
	{
		row.NameUsage.ToString(CultureInfo.InvariantCulture), row.Name
	});

	private static IReadOnlyDictionary<string, string> Fields(RandomNameProfile model)
	{
		var result = new Dictionary<string, string>
		{
			["name"] = model.Name, ["gender"] = model.Gender.ToString(CultureInfo.InvariantCulture),
			["culture"] = model.NameCultureId.ToString(CultureInfo.InvariantCulture),
			["suggestions"] = model.UseForChargenSuggestionsProgId?.ToString(CultureInfo.InvariantCulture) ?? ""
		};
		foreach (var row in model.RandomNameProfilesElements) result.Add(ElementKey(row), row.Weighting.ToString(CultureInfo.InvariantCulture));
		foreach (var row in model.RandomNameProfilesDiceExpressions) result.Add($"dice:{row.NameUsage}", row.DiceExpression);
		return result;
	}

	private static void Bind(FuturemudDatabaseContext context, string era, string key, Ethnicity ethnicity,
		short gender, NameCulture culture, ICollection<string> conflicts,
		IReadOnlyDictionary<(long EthnicityId, short Gender), long>? verifiedOriginalNameCultures)
	{
		var link = ethnicity.EthnicitiesNameCultures.SingleOrDefault(x => x.Gender == gender);
		var linkKey = key + $".gender.{gender}";
		var record = CultureToolkitManagedEntities.Find(context, "EthnicityNameCulture", linkKey);
		var fresh = link is null && record is null;
		if (record is null && verifiedOriginalNameCultures?.TryGetValue((ethnicity.Id, gender), out var original) == true)
		{
			context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = "CultureSeeder", Module = era, EntityType = "EthnicityNameCulture", StableKey = linkKey, LogicalId = ethnicity.Id,
				ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow,
				SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { ["culture"] = original.ToString(CultureInfo.InvariantCulture) })
			});
			context.SaveChanges();
			fresh = false;
		}
		var merged = CultureToolkitManagedEntities.Reconcile(context, era, "EthnicityNameCulture",
			linkKey, ethnicity.Id, fresh,
			link is null ? new Dictionary<string, string>() : new Dictionary<string, string> { ["culture"] = link.NameCultureId.ToString(CultureInfo.InvariantCulture) },
			new Dictionary<string, string> { ["culture"] = culture.Id.ToString(CultureInfo.InvariantCulture) }, conflicts);
		if (!merged.TryGetValue("culture", out var value)) return;
		var target = long.Parse(value, CultureInfo.InvariantCulture);
		if (link?.NameCultureId == target) return;
		if (link is not null) { ethnicity.EthnicitiesNameCultures.Remove(link); context.EthnicitiesNameCultures.Remove(link); }
		ethnicity.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures { Ethnicity = ethnicity, Gender = gender, NameCultureId = target });
	}
}
