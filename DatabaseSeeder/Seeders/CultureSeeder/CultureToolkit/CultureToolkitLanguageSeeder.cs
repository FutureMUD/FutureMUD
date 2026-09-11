#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureLanguagePrerequisites(TraitDefinition Intelligence, TraitDecorator Decorator, Improver Improver,
	LanguageDifficultyModels DifficultyModel, FutureProg AlwaysTrue, FutureProg AlwaysFalse);
public sealed record CultureLanguageInstallResult(IReadOnlyDictionary<string, Language> Languages,
	IReadOnlyDictionary<string, TraitDefinition> Traits, IReadOnlyList<string> CapReports);

/// <summary>Resolved source definitions become shared language/trait identities; all writes retain field baselines.</summary>
public static class CultureToolkitLanguageSeeder
{
	public static CultureLanguageInstallResult Upsert(FuturemudDatabaseContext context, CultureToolkitCatalogue catalogue,
		CultureToolkitPack pack, IReadOnlyDictionary<string, FuturemudDatabaseContext> sourceStages,
		IReadOnlyDictionary<string, Language> preflightedBindings, CultureLanguagePrerequisites prerequisites, ICollection<string> conflicts,
		IReadOnlySet<string>? activeRetainedLanguageKeys = null, Action<string>? progress = null)
	{
		var writer = new CultureToolkitEntityWriter(context, pack.Era, conflicts);
		var eras = catalogue.Document("data.eras.json").EnumerateArray().Select(x => CultureToolkitCatalogue.Text(x, "key")).ToList();
		var firstLegacyEra = CultureToolkitLanguageBindings.SourceModuleFirstEra;
		var specifications = pack.Languages.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"));
		var sources = sourceStages.SelectMany(source => source.Value.Languages.Include(x => x.LinkedTrait).ThenInclude(x => x.Expression)
			.Include(x => x.Accents).AsEnumerable().Select(language => (Module: source.Key, Language: language,
				Key: CultureToolkitLanguageBindings.Key(source.Key, language.Name))))
			.Where(x => specifications.ContainsKey(x.Key) || x.Key.StartsWith("source.", StringComparison.Ordinal) &&
				eras.IndexOf(pack.Era) >= eras.IndexOf(firstLegacyEra[x.Module]))
			.GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.ToArray());
		var keys = specifications.Keys.Union(sources.Keys, StringComparer.Ordinal).ToArray();
		foreach (var key in specifications.Keys.Where(key => !sources.ContainsKey(key) && CultureToolkitLanguageBindings.All.Any(x => x.CanonicalKey == key)))
			throw new InvalidOperationException($"Missing retained source definition for {key}: " + string.Join(", ",
				CultureToolkitLanguageBindings.All.Where(x => x.CanonicalKey == key).Select(x => $"{x.SourcePack}:{x.SourceName}")));
		var installed = new Dictionary<string, Language>(StringComparer.Ordinal);
		var caps = new List<string>();
		foreach (var key in keys)
		{
			sources.TryGetValue(key, out var sourceRows);
			var source = sourceRows?.FirstOrDefault().Language;
			var hasSpecification = specifications.TryGetValue(key, out var specification);
			var label = hasSpecification ? CultureToolkitCatalogue.Text(specification.GetProperty("labels"), pack.Era) : source!.Name;
			var record = CultureToolkitManagedEntities.Find(context, "Language", key);
			var existing = record is null ? preflightedBindings.GetValueOrDefault(key) : context.Languages
				.Include(x => x.LinkedTrait).ThenInclude(x => x.Expression).Single(x => x.Id == record.LogicalId);
			if (existing is null && context.Languages.Any(x => x.Name == label))
				throw new InvalidOperationException($"Language label {label} has no source-qualified binding; resolve it before activation.");
			var sourceCap = source?.LinkedTrait.Expression ?? new TraitExpression
			{
				Name = $"{label} Skill Cap", Expression = $"10+(9.5 * {prerequisites.Intelligence.Alias}:{prerequisites.Intelligence.Id})"
			};
			var desiredCap = new TraitExpression { Name = $"{label} Skill Cap", Expression = $"max(200, ({sourceCap.Expression}))" };
			var existingCap = existing?.LinkedTrait.Expression;
			TraitExpression cap;
			if (existingCap is not null && context.TraitDefinitions.Count(x => x.ExpressionId == existingCap.Id) > 1)
			{
				cap = existingCap;
				caps.Add($"{key}: shared existing cap {cap.Id} preserved ({cap.Expression}); no floor was applied.");
			}
			else
			{
				// An unbaselined cap is editable only after exact stock-source comparison.
				var verifiedCap = existingCap is not null && source is not null && existingCap.Name == sourceCap.Name &&
					existingCap.Expression == sourceCap.Expression ? sourceCap : null;
				cap = writer.Upsert(key + ".cap", desiredCap, existingCap, verifiedCap);
				caps.Add($"{key}: cap {cap.Id} = {cap.Expression}" + (cap.Expression != desiredCap.Expression ? "; preserved override; effective native value requires runtime evaluation." : "; stock floor 200."));
			}
			var trait = source is null ? new TraitDefinition
			{
				Type = 0, OwnerScope = 1, DecoratorId = prerequisites.Decorator.Id, ImproverId = prerequisites.Improver.Id,
				TraitGroup = "Language", AvailabilityProgId = prerequisites.AlwaysTrue.Id,
				TeachableProgId = prerequisites.AlwaysFalse.Id, LearnableProgId = prerequisites.AlwaysTrue.Id,
				TeachDifficulty = 7, LearnDifficulty = 7, BranchMultiplier = .1
			} : CultureToolkitEntityWriter.CopyScalars(context, source.LinkedTrait);
			trait.Name = label;
			trait.ExpressionId = cap.Id;
			if (!hasSpecification && activeRetainedLanguageKeys?.Contains(key) != true)
				trait.AvailabilityProgId = prerequisites.AlwaysFalse.Id;
			if (hasSpecification) trait.ChargenBlurb = specification.TryGetProperty("descriptions_by_era", out var descriptions) &&
				descriptions.TryGetProperty(pack.Era, out var description) ? description.GetString()! : CultureToolkitCatalogue.Text(specification, "description");
			TraitDefinition? originalTrait = source is null ? null : CultureToolkitEntityWriter.CopyScalars(context, source.LinkedTrait);
			if (originalTrait is not null) originalTrait.ExpressionId = existing?.LinkedTrait.ExpressionId ?? cap.Id;
			var installedTrait = writer.Upsert(key + ".trait", trait, existing?.LinkedTrait, originalTrait);
			var desired = source is null ? new Language { LanguageObfuscationFactor = .1 } : CultureToolkitEntityWriter.CopyScalars(context, source);
			desired.Name = label;
			desired.DifficultyModel = prerequisites.DifficultyModel.Id;
			desired.LinkedTraitId = installedTrait.Id;
			if (hasSpecification) desired.UnknownLanguageDescription = CultureToolkitCatalogue.Text(specification, "unknown_description_template");
			Language? original = source is null ? null : CultureToolkitEntityWriter.CopyScalars(context, source);
			if (original is not null)
			{
				if (existing is not null && sourceRows!.Any(x => x.Language.UnknownLanguageDescription == existing.UnknownLanguageDescription))
					original.UnknownLanguageDescription = existing.UnknownLanguageDescription;
				original.LinkedTraitId = installedTrait.Id;
				original.DifficultyModel = prerequisites.DifficultyModel.Id;
			}
			var language = writer.Upsert(key, desired, existing, original);
			installed[key] = language;
			var newAccents = new Dictionary<string, Accent>();
			foreach (var row in sourceRows ?? [])
			foreach (var accent in row.Language.Accents)
			{
				var accentKey = $"{key}.accent.{row.Module}.{accent.Name}";
				var desiredAccent = CultureToolkitEntityWriter.CopyScalars(context, accent);
				desiredAccent.LanguageId = language.Id;
				var originalAccent = CultureToolkitEntityWriter.CopyScalars(context, desiredAccent);
				desiredAccent.Description = CultureToolkitProse.Display(CultureToolkitProse.Rewrite(row.Module, "Accent", row.Language.Name + ":" + accent.Name, "Description", accent.Description));
				desiredAccent.Suffix = CultureToolkitProse.Display(CultureToolkitProse.Rewrite(row.Module, "Accent", row.Language.Name + ":" + accent.Name, "Suffix", accent.Suffix));
				desiredAccent.VagueSuffix = CultureToolkitProse.Display(CultureToolkitProse.Rewrite(row.Module, "Accent", row.Language.Name + ":" + accent.Name, "VagueSuffix", accent.VagueSuffix));
				var accentRecord = CultureToolkitManagedEntities.Find(context, "Accent", accentKey);
				if (existing is null && accentRecord is null)
				{
					newAccents.Add(accentKey, desiredAccent);
					continue;
				}
				Accent? boundAccent = null;
				if (accentRecord is null && existing is not null)
				{
					// Only a full source fingerprint binds an unbaselined accent. A label alone is ambiguous.
					var matches = context.Accents.Where(x => x.LanguageId == language.Id && x.Name == accent.Name &&
						x.Suffix == accent.Suffix && x.VagueSuffix == accent.VagueSuffix && x.Description == accent.Description &&
						x.Group == accent.Group && x.Difficulty == accent.Difficulty).ToArray();
					var ownedIds = context.SeederManagedRecords.Where(x => x.Seeder == "CultureSeeder" && x.EntityType == "Accent")
						.Select(x => x.LogicalId).ToHashSet();
					var unownedMatches = matches.Where(x => !ownedIds.Contains(x.Id)).ToArray();
					if (unownedMatches.Length == 1) boundAccent = unownedMatches[0];
					else if (unownedMatches.Length > 1 || context.Accents.Where(x => x.LanguageId == language.Id && x.Name == accent.Name).AsEnumerable()
						.Any(x => !ownedIds.Contains(x.Id) && !(sourceRows ?? []).SelectMany(r => r.Language.Accents).Any(a =>
							x.Name == a.Name && x.Suffix == a.Suffix && x.VagueSuffix == a.VagueSuffix && x.Description == a.Description && x.Group == a.Group && x.Difficulty == a.Difficulty)))
						throw new InvalidOperationException($"Accent {accentKey} needs an explicit source binding; existing content was not replaced.");
				}
				var ownsAvailability = accentRecord is not null && CultureToolkitManagedEntities.Find(context, "AccentAvailability", $"accent.availability.{accentRecord.LogicalId}") is not null;
				writer.Upsert(accentKey, desiredAccent, boundAccent, originalAccent,
					ownsAvailability ? new HashSet<string> { nameof(Accent.ChargenAvailabilityProgId) } : null);
			}
			writer.InsertNewAccents(newAccents);
			if (hasSpecification && (CultureToolkitManagedEntities.Find(context, "Accent", key + ".accent.local") is not null ||
				!context.Accents.Where(x => x.LanguageId == language.Id).AsEnumerable().Any(x =>
					x.Role == 0)))
			{
				var fallback = specification.GetProperty("fallback_accent_if_no_legacy_accent");
				writer.Upsert(key + ".accent.local", new Accent
				{
					LanguageId = language.Id, Name = CultureToolkitCatalogue.Text(fallback, "name"),
					Group = CultureToolkitCatalogue.Text(fallback, "group"), Suffix = CultureToolkitCatalogue.Text(fallback, "suffix"),
					VagueSuffix = CultureToolkitCatalogue.Text(fallback, "vague_suffix"), Description = CultureToolkitCatalogue.Text(fallback, "description"),
					Difficulty = (int)Enum.Parse<MudSharp.RPG.Checks.Difficulty>(CultureToolkitCatalogue.Text(fallback, "difficulty"))
				}, independentlyManagedFields: new HashSet<string> { nameof(Accent.ChargenAvailabilityProgId) });
			}
			if (hasSpecification && sourceRows is not { Length: > 0 })
			{
				var learnerRecord = CultureToolkitManagedEntities.Find(context, "Accent", key + ".accent.learner");
				var learner = learnerRecord is not null ? context.Accents.Find(learnerRecord.LogicalId)! : writer.Upsert(key + ".accent.learner", new Accent
				{
					Role = 2, LanguageId = language.Id, Name = "Learner", Group = "learner", Suffix = "with a learner's accent",
					VagueSuffix = "with an unfamiliar accent", Description = "The hesitant pronunciation of someone learning the language.", Difficulty = 6
				}, independentlyManagedFields: new HashSet<string> { nameof(Accent.ChargenAvailabilityProgId) });
			}
			context.SaveChanges();
			if (installed.Count % 10 == 0 || installed.Count == keys.Length)
				progress?.Invoke($"Languages and source accents: {installed.Count}/{keys.Length}.");
		}
		// Legacy references resolve only to actual retained source rows, never newly invented labels.
		foreach (var sourceName in sources.Values.SelectMany(x => x).GroupBy(x => x.Language.Name))
		{
			var targets = sourceName.Select(x => installed[x.Key]).Distinct().ToArray();
			if (targets.Length == 1) installed[$"legacy:{sourceName.Key}"] = targets[0];
			else conflicts.Add($"Ambiguous retained language alias: legacy:{sourceName.Key}; no alias installed.");
		}
		foreach (var binding in catalogue.Document("data.mutual_intelligibility.json").GetProperty("conditional_legacy_bindings").EnumerateArray())
		{
			if (eras.IndexOf(pack.Era) < eras.IndexOf(CultureToolkitCatalogue.Text(binding, "earliest_pack")))
			{
				foreach (var alias in CultureToolkitCatalogue.Strings(binding.GetProperty("aliases"))) installed.Remove("legacy:" + alias);
				continue;
			}
			var matches = CultureToolkitCatalogue.Strings(binding.GetProperty("aliases"))
				.Where(x => installed.ContainsKey("legacy:" + x)).Select(x => installed["legacy:" + x]).Distinct().ToArray();
			if (matches.Length == 1) installed[CultureToolkitCatalogue.Text(binding, "key")] = matches[0];
		}
		var installedIds = installed.Values.Select(x => x.Id).Distinct().ToArray();
		context.Accents.Where(x => installedIds.Contains(x.LanguageId)).Include(x => x.AssociatedLanguages).Load();
		foreach (var row in sources.Values.SelectMany(x => x))
		foreach (var sourceAccent in row.Language.Accents)
		{
			var key = $"{row.Key}.accent.{row.Module}.{sourceAccent.Name}";
			var record = CultureToolkitManagedEntities.Find(context, "Accent", key);
			if (record is null) continue;
			var accent = context.Accents.Find(record.LogicalId)!;
			var desiredIds = new List<long>();
			foreach (var sourceLanguage in CultureStockAccentRoles.Associations(row.Language.Name, sourceAccent.Name, row.Module))
			{
				var binding = CultureToolkitLanguageBindings.Key(row.Module, sourceLanguage);
				// Some canonical-only skills (for example Sardinian) have no retained source row.
				if (!installed.ContainsKey(binding))
				{
					var canonical = pack.Languages.Where(x => CultureToolkitCatalogue.Strings(x.GetProperty("legacy_language_names"))
						.Contains(sourceLanguage, StringComparer.OrdinalIgnoreCase)).ToArray();
					if (canonical.Length == 1) binding = CultureToolkitCatalogue.Text(canonical[0], "key");
				}
				if (installed.TryGetValue(binding, out var associated) || installed.TryGetValue("legacy:" + sourceLanguage, out associated))
					desiredIds.Add(associated.Id);
				else conflicts.Add($"Accent {key}: associated source language {sourceLanguage} is not installed.");
			}
			var associationKey = key + ".associated-languages";
			var owner = CultureToolkitManagedEntities.Find(context, "AccentAssociations", associationKey);
			if (owner is null)
			{
				owner = new SeederManagedRecord { Seeder = "CultureSeeder", EntityType = "AccentAssociations",
					StableKey = associationKey, Module = pack.Era, LogicalId = accent.Id,
					ManifestVersion = "2026-09-10", AppliedAt = DateTime.UtcNow,
					SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { ["languages"] = "[]" }) };
				context.SeederManagedRecords.Add(owner);
			}
			var merged = SeederManagedRecordReconciler.Reconcile(owner,
				new Dictionary<string, string> { ["languages"] = JsonSerializer.Serialize(accent.AssociatedLanguages.Select(x => x.Id).Order().ToArray()) },
				new Dictionary<string, string> { ["languages"] = JsonSerializer.Serialize(desiredIds.Distinct().Order().ToArray()) }, false, conflicts);
			accent.AssociatedLanguages.Clear();
			foreach (var id in JsonSerializer.Deserialize<long[]>(merged["languages"])!)
				accent.AssociatedLanguages.Add(context.Languages.Find(id)!);
		}
		context.SaveChanges();
		return new CultureLanguageInstallResult(installed, installed.ToDictionary(x => x.Key,
			x => context.TraitDefinitions.Find(x.Value.LinkedTraitId)!), caps);
	}
}
