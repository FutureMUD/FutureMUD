#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureToolkitExistingBindings(
	IReadOnlyDictionary<string, NameCulture> NameCultures,
	IReadOnlyDictionary<string, RandomNameProfile> Profiles,
	IReadOnlyDictionary<string, Language> Languages,
	IReadOnlyDictionary<string, Script> Scripts,
	IReadOnlyDictionary<string, Ethnicity> Ethnicities);

/// <summary>Read-only binding before writes. A display-name collision without source corroboration is reported.</summary>
public static class CultureToolkitBindingPreflight
{
	public static CultureToolkitExistingBindings Resolve(FuturemudDatabaseContext context, CultureToolkitCatalogue catalogue,
		CultureSourceNamingPlan names, IReadOnlyDictionary<string, FuturemudDatabaseContext> stages,
		IReadOnlyList<CultureHeritageSource> heritage, ICollection<string> issues, CultureToolkitPack? pack = null)
	{
		var cultures = new Dictionary<string, NameCulture>();
		foreach (var source in names.Cultures)
		{
			var record = CultureToolkitManagedEntities.Find(context, "NameCulture", source.Key);
			var bound = record is null ? null : context.NameCultures.Find(record.LogicalId);
			if (record is not null && bound is null) { issues.Add($"{source.Key}: managed naming structure {record.LogicalId} was deleted."); continue; }
			if (bound is null)
			{
				var matches = context.NameCultures.Where(x => x.Name == source.Definition.Name).ToArray();
				if (matches.Length == 1 && matches[0].Definition == source.Definition.Definition) bound = matches[0];
				else if (matches.Length > 0) issues.Add($"{source.Key}: unbaselined naming structure candidates {string.Join(", ", matches.Select(x => x.Id))}; source definition does not identify one unchanged record.");
			}
			if (bound is not null) cultures[source.Key] = bound;
		}
		var profiles = new Dictionary<string, RandomNameProfile>();
		foreach (var source in names.Profiles)
		{
			var record = CultureToolkitManagedEntities.Find(context, "RandomNameProfile", source.Key);
			var query = context.RandomNameProfiles.Include(x => x.RandomNameProfilesElements).Include(x => x.RandomNameProfilesDiceExpressions);
			RandomNameProfile? bound = record is null ? null : query.SingleOrDefault(x => x.Id == record.LogicalId);
			if (record is not null && bound is null) { issues.Add($"{source.Key}: managed profile {record.LogicalId} was deleted."); continue; }
			if (bound is null && cultures.TryGetValue(source.CultureKey, out var culture))
			{
				var matches = query.Where(x => x.NameCultureId == culture.Id && x.Name == source.Definition.Name && x.Gender == source.Definition.Gender).ToArray();
				if (matches.Length == 1) bound = matches[0];
				else if (matches.Length > 1) issues.Add($"{source.Key}: ambiguous source profile IDs {string.Join(", ", matches.Select(x => x.Id))}.");
			}
			if (bound is not null) profiles[source.Key] = bound;
		}
		foreach (var duplicate in profiles.GroupBy(x => x.Value.Id).Where(x => x.Count() > 1))
			issues.Add($"Profile {duplicate.Key} matches divergent source identities: {string.Join(", ", duplicate.Select(x => x.Key))}.");
		var languages = new Dictionary<string, Language>();
		var canonicalKeys = pack?.Languages.Select(x => CultureToolkitCatalogue.Text(x, "key")).ToHashSet();
		var eraKeys = catalogue.Document("data.eras.json").EnumerateArray().Select(x => CultureToolkitCatalogue.Text(x, "key")).ToList();
		var sourceLanguages = stages.SelectMany(stage => stage.Value.Languages.Include(x => x.LinkedTrait).ThenInclude(x => x.Expression)
			.Include(x => x.Accents).AsEnumerable().Select(x => (Module: stage.Key, Key: CultureToolkitLanguageBindings.Key(stage.Key, x.Name), Model: x)))
			.Where(x => pack is null || canonicalKeys!.Contains(x.Key) || x.Key.StartsWith("source.", StringComparison.Ordinal) &&
				eraKeys.IndexOf(pack.Era) >= eraKeys.IndexOf(CultureToolkitLanguageBindings.SourceModuleFirstEra[x.Module]))
			.GroupBy(x => x.Key).ToArray();
		var plannedLanguageKeys = sourceLanguages.Select(x => x.Key).Concat(canonicalKeys ?? []).ToHashSet();
		foreach (var source in sourceLanguages)
		{
			var record = CultureToolkitManagedEntities.Find(context, "Language", source.Key);
			var query = context.Languages.Include(x => x.LinkedTrait).ThenInclude(x => x.Expression).Include(x => x.Accents);
			var bound = record is null ? null : query.SingleOrDefault(x => x.Id == record.LogicalId);
			if (record is not null && bound is null) { issues.Add($"{source.Key}: managed language {record.LogicalId} was deleted."); continue; }
			if (bound is null)
			{
				var sourceNames = source.Select(x => x.Model.Name).Distinct().ToList();
				var traitNames = source.Select(x => x.Model.LinkedTrait.Name).Distinct().ToList();
				var capNames = source.Select(x => x.Model.LinkedTrait.Expression.Name).Distinct().ToList();
				var candidates = query.Where(x => sourceNames.Contains(x.Name) || traitNames.Contains(x.LinkedTrait.Name) && capNames.Contains(x.LinkedTrait.Expression.Name)).ToArray();
				var corroborated = candidates.Where(candidate => source.Any(x =>
					candidate.LinkedTrait.Name == x.Model.LinkedTrait.Name && candidate.LinkedTrait.Expression.Name == x.Model.LinkedTrait.Expression.Name &&
					x.Model.Accents.Any(accent => candidate.Accents.Any(y => y.Name == accent.Name && y.Group == accent.Group &&
						y.Suffix == accent.Suffix && y.Description == accent.Description && y.Difficulty == accent.Difficulty &&
						!string.Equals(y.Group, "foreign", StringComparison.OrdinalIgnoreCase) && !string.Equals(y.Group, "crude", StringComparison.OrdinalIgnoreCase))))).ToArray();
				if (corroborated.Length == 1) bound = corroborated[0];
				else if (candidates.Length > 0) issues.Add($"{source.Key}: unresolved language candidates {string.Join(", ", candidates.Select(x => x.Id))}; source trait/cap/accent context is insufficient or ambiguous.");
			}
			if (bound is not null) languages[source.Key] = bound;
		}
		foreach (var duplicate in languages.GroupBy(x => x.Value.Id).Where(x => x.Count() > 1))
			issues.Add($"Language {duplicate.Key} matches distinct source stages: {string.Join(", ", duplicate.Select(x => x.Key))}.");
		var scripts = new Dictionary<string, Script>();
		var scriptKeys = catalogue.Document("data.script_policy.json").EnumerateArray().ToDictionary(x => CultureToolkitCatalogue.Text(x, "label"), x => CultureToolkitCatalogue.Text(x, "key"), StringComparer.OrdinalIgnoreCase);
		foreach (var source in stages.SelectMany(stage => stage.Value.Scripts.Include(x => x.Knowledge)
			.Include(x => x.ScriptsDesignedLanguages).ThenInclude(x => x.Language).AsEnumerable()
			.Where(x => pack is null || x.ScriptsDesignedLanguages.Any(y => plannedLanguageKeys.Contains(CultureToolkitLanguageBindings.Key(stage.Key, y.Language.Name))))
			.Select(x => (Key: scriptKeys.GetValueOrDefault(x.Name) ?? $"source.{stage.Key}.script.{x.Name}", Model: x))).GroupBy(x => x.Key))
		{
			var record = CultureToolkitManagedEntities.Find(context, "Script", source.Key);
			var query = context.Scripts.Include(x => x.Knowledge).ThenInclude(x => x.CanAcquireProg);
			var bound = record is null ? null : query.SingleOrDefault(x => x.Id == record.LogicalId);
			if (record is not null && bound is null) { issues.Add($"{source.Key}: managed script {record.LogicalId} was deleted."); continue; }
			if (bound is null)
			{
				var sourceNames = source.Select(x => x.Model.Name).Distinct().ToList();
				var candidates = query.Where(x => sourceNames.Contains(x.Name)).ToArray();
				var matches = candidates.Where(x => source.Any(y => y.Model.Knowledge.Name == x.Knowledge.Name && y.Model.Knowledge.Type == x.Knowledge.Type &&
					y.Model.KnownScriptDescription == x.KnownScriptDescription && y.Model.UnknownScriptDescription == x.UnknownScriptDescription)).ToArray();
				if (matches.Length == 1) bound = matches[0];
				else if (candidates.Length > 0) issues.Add($"{source.Key}: unresolved script candidates {string.Join(", ", candidates.Select(x => x.Id))}.");
			}
			if (bound is not null) scripts[source.Key] = bound;
		}
		var ethnicities = new Dictionary<string, Ethnicity>();
		foreach (var source in heritage)
		{
			var record = CultureToolkitManagedEntities.Find(context, "Ethnicity", source.Key);
			var query = context.Ethnicities.Include(x => x.EthnicitiesCharacteristics).Include(x => x.EthnicitiesNameCultures).ThenInclude(x => x.NameCulture);
			var bound = record is null ? null : query.SingleOrDefault(x => x.Id == record.LogicalId);
			if (record is not null && bound is null) { issues.Add($"{source.Key}: managed ethnicity {record.LogicalId} was deleted."); continue; }
			if (bound is null && source.Aliases.Count > 0)
			{
				var candidates = query.Where(x => x.Name == source.Template.Name && x.ParentRaceId == source.Template.ParentRaceId).ToArray();
				var matches = candidates.Where(x => x.EthnicGroup == source.Template.EthnicGroup && x.EthnicitiesNameCultures.Any(y =>
					source.Template.EthnicitiesNameCultures.Any(z => z.Gender == y.Gender && z.NameCulture.Name == y.NameCulture.Name))).ToArray();
				if (matches.Length == 1) bound = matches[0];
				else if (candidates.Length > 0) issues.Add($"{source.Key}: unresolved ethnicity candidates {string.Join(", ", candidates.Select(x => x.Id))}.");
			}
			if (bound is not null) ethnicities[source.Key] = bound;
		}
		return new(cultures, profiles, languages, scripts, ethnicities);
	}
}
