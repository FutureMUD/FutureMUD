#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureSourceNameCulture(string Key, NameCulture Definition, IReadOnlyList<(string Module, long Id)> Sources);
public sealed record CultureSourceNameProfile(string Key, string CultureKey, RandomNameProfile Definition,
	string? SuggestionProgName, IReadOnlyList<(string Module, long Id)> Sources);
public sealed record CultureSourceNamingPlan(IReadOnlyList<CultureSourceNameCulture> Cultures, IReadOnlyList<CultureSourceNameProfile> Profiles);
public sealed record CultureSourceNamingResolution(IReadOnlyDictionary<string, NameCulture> Cultures,
	IReadOnlyDictionary<string, RandomNameProfile> Profiles, IReadOnlyDictionary<(string Module, long Id), NameCulture> SourceCultures);

/// <summary>Aliases only byte-equivalent source definitions; divergent source profiles keep their module identity.</summary>
public static class CultureToolkitSourceNames
{
	public static CultureSourceNamingPlan Describe(IReadOnlyDictionary<string, FuturemudDatabaseContext> stages)
	{
		var all = stages.OrderBy(x => x.Key, StringComparer.Ordinal).SelectMany(stage => stage.Value.NameCultures.AsEnumerable()
			.Select(culture => (Module: stage.Key, Culture: culture))).ToArray();
		var cultures = new List<CultureSourceNameCulture>();
		foreach (var names in all.GroupBy(x => x.Culture.Name, StringComparer.Ordinal))
		{
			var shared = names.Select(x => x.Culture.Definition).Distinct(StringComparer.Ordinal).Count() == 1;
			foreach (var group in names.GroupBy(x => shared ? $"source.naming.shared.{names.Key}" : $"source.naming.{x.Module}.{names.Key}"))
				cultures.Add(new(group.Key, group.First().Culture, group.Select(x => (x.Module, x.Culture.Id)).ToArray()));
		}
		var cultureKeys = cultures.SelectMany(x => x.Sources.Select(y => (Source: y, x.Key))).ToDictionary(x => x.Source, x => x.Key);
		var profiles = new List<CultureSourceNameProfile>();
		var sourceProfiles = stages.OrderBy(x => x.Key, StringComparer.Ordinal).SelectMany(stage => stage.Value.RandomNameProfiles
			.Include(x => x.RandomNameProfilesElements).Include(x => x.RandomNameProfilesDiceExpressions).Include(x => x.UseForChargenSuggestionsProg)
			.AsEnumerable().Select(profile => (Module: stage.Key, Profile: profile, CultureKey: cultureKeys[(stage.Key, profile.NameCultureId)]))).ToArray();
		foreach (var named in sourceProfiles.GroupBy(x => (x.CultureKey, x.Profile.Name, x.Profile.Gender)))
		{
			var shared = named.Select(x => ProfileFingerprint(x.Profile)).Distinct(StringComparer.Ordinal).Count() == 1;
			foreach (var group in named.GroupBy(x => $"{x.CultureKey}.profile.{(shared ? "shared" : x.Module)}.{x.Profile.Name}.gender.{x.Profile.Gender}"))
				profiles.Add(new(group.Key, group.First().CultureKey, group.First().Profile,
					group.First().Profile.UseForChargenSuggestionsProg?.FunctionName, group.Select(x => (x.Module, x.Profile.Id)).ToArray()));
		}
		return new(cultures, profiles);
	}

	public static CultureSourceNamingResolution Upsert(FuturemudDatabaseContext context, string era, CultureSourceNamingPlan plan,
		IReadOnlyDictionary<string, NameCulture> cultureBindings, IReadOnlyDictionary<string, RandomNameProfile> profileBindings,
		IReadOnlyDictionary<string, FutureProg> suggestionProgs, bool includeProfiles, ICollection<string> conflicts)
	{
		foreach (var item in plan.Cultures)
			if (CultureToolkitManagedEntities.Find(context, "NameCulture", item.Key) is null && !cultureBindings.ContainsKey(item.Key) &&
				context.NameCultures.Any(x => x.Name == item.Definition.Name))
				throw new InvalidOperationException($"Retained naming structure {item.Key} needs its source binding.");
		if (includeProfiles)
		foreach (var item in plan.Profiles.Where(x => x.SuggestionProgName is not null))
			if (!suggestionProgs.ContainsKey(item.SuggestionProgName!))
				throw new InvalidOperationException($"Retained profile {item.Key} has unresolved suggestion prog {item.SuggestionProgName}.");
		var writer = new CultureToolkitEntityWriter(context, era, conflicts);
		var cultures = plan.Cultures.ToDictionary(x => x.Key, x =>
		{
			var desired = CultureToolkitEntityWriter.CopyScalars(context, x.Definition);
			desired.Definition = CultureToolkitProse.Rewrite(x.Sources[0].Module, "NameCulture", x.Definition.Name, "Definition", x.Definition.Definition);
			return writer.Upsert(x.Key, desired, cultureBindings.GetValueOrDefault(x.Key), x.Definition);
		});
		var profiles = new Dictionary<string, RandomNameProfile>();
		if (includeProfiles)
		foreach (var item in plan.Profiles)
		{
			var source = item.Definition;
			var desired = new RandomNameProfile { Name = source.Name, Gender = source.Gender, NameCultureId = cultures[item.CultureKey].Id };
			foreach (var element in source.RandomNameProfilesElements)
				desired.RandomNameProfilesElements.Add(new RandomNameProfilesElements { Name = element.Name, NameUsage = element.NameUsage, Weighting = element.Weighting });
			foreach (var dice in source.RandomNameProfilesDiceExpressions)
				desired.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions { NameUsage = dice.NameUsage, DiceExpression = dice.DiceExpression });
			var suggestions = item.SuggestionProgName is null ? null : suggestionProgs[item.SuggestionProgName];
			desired.UseForChargenSuggestionsProgId = suggestions?.Id;
			profiles[item.Key] = CultureToolkitNameSeeder.UpsertProfile(context, era, item.Key, cultures[item.CultureKey], desired,
				suggestions, conflicts, profileBindings.GetValueOrDefault(item.Key), desired);
		}
		context.SaveChanges();
		return new(cultures, profiles, plan.Cultures.SelectMany(x => x.Sources.Select(y => (Source: y, Culture: cultures[x.Key])))
			.ToDictionary(x => x.Source, x => x.Culture));
	}

	private static string ProfileFingerprint(RandomNameProfile profile) => JsonSerializer.Serialize(new
	{
		profile.Name, profile.Gender, Suggestions = profile.UseForChargenSuggestionsProg?.FunctionName,
		Elements = profile.RandomNameProfilesElements.OrderBy(x => x.NameUsage).ThenBy(x => x.Name, StringComparer.Ordinal)
			.Select(x => new { x.NameUsage, x.Name, x.Weighting }),
		Dice = profile.RandomNameProfilesDiceExpressions.OrderBy(x => x.NameUsage).Select(x => new { x.NameUsage, x.DiceExpression })
	});
}
