#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureAccentReceipt(long AccentId, string StableKey, string LanguageKey, string Rule,
	IReadOnlyList<string> AllowedPacks, string Role, long? OriginalPredicate, long? EffectivePredicate,
	bool IsDefault, bool BuilderOverride, bool EraEligible);
public sealed record CultureAccentResolution(long LanguageId, IReadOnlyList<long> NativeAccentIds,
	IReadOnlyList<long> RestrictedAccentIds, long EligibilityProgId)
{
	public IReadOnlyList<CultureAccentReceipt> Details { get; init; } = [];
}

/// <summary>Single ownership of combined source, era and ethnic-native role availability.</summary>
public static class CultureToolkitAccents
{
	public static IReadOnlyList<CultureAccentResolution> Upsert(FuturemudDatabaseContext context, string era,
		IReadOnlyDictionary<long, IReadOnlyList<long>> nativeEthnicities, ICollection<string> conflicts,
		CultureToolkitCatalogue? catalogue = null, IReadOnlyDictionary<string, FuturemudDatabaseContext>? stages = null,
		IReadOnlyDictionary<string, Language>? installedLanguages = null)
	{
		catalogue ??= new CultureToolkitCatalogue();
		var pack = catalogue.Compose(era);
		var sourceInfo = (stages ?? new Dictionary<string, FuturemudDatabaseContext>()).SelectMany(stage =>
			stage.Value.Languages.Include(x => x.Accents).AsEnumerable().SelectMany(language => language.Accents.Select(accent =>
				(Key: $"{CultureToolkitLanguageBindings.Key(stage.Key, language.Name)}.accent.{stage.Key}.{accent.Name}",
				 Policy: CultureToolkitAccentPolicy.Resolve(catalogue, stage.Key, language.Name, accent.Name, accent.Group, language.DefaultLearnerAccentId == accent.Id),
				 Predicate: accent.ChargenAvailabilityProgId, SourceDefault: language.DefaultLearnerAccentId == accent.Id)))).ToDictionary(x => x.Key);
		var result = new List<CultureAccentResolution>();
		var generated = new HashSet<long>();
		var languageIds = installedLanguages?.Values.Select(x => x.Id).Distinct().ToArray() ?? nativeEthnicities.Keys.ToArray();
		foreach (var languageId in languageIds)
		{
			var language = context.Languages.Find(languageId)!;
			var languageRecord = context.SeederManagedRecords.SingleOrDefault(x => x.Seeder == "CultureSeeder" && x.EntityType == "Language" && x.LogicalId == languageId);
			var languageKey = languageRecord?.StableKey ?? $"fixture.{languageId}";
			var nativeKey = $"accent.native-bindings.{languageId}";
			var savedNative = CultureToolkitManagedEntities.Find(context, "AccentNativeBindings", nativeKey);
			var nativeIds = nativeEthnicities.TryGetValue(languageId, out var supplied) ? supplied : savedNative?.SeedBaseline is not null
				? JsonSerializer.Deserialize<long[]>(savedNative.SeedBaseline)! : [];
			if (savedNative is null)
			{
				savedNative = new SeederManagedRecord { Seeder = "CultureSeeder", EntityType = "AccentNativeBindings", StableKey = nativeKey,
					Module = era, LogicalId = languageId, ManifestVersion = "2026-09-09", AppliedAt = DateTime.UtcNow };
				context.SeederManagedRecords.Add(savedNative);
			}
			savedNative.SeedBaseline = JsonSerializer.Serialize(nativeIds.Order());
			var body = string.Concat(nativeIds.Distinct().Order().Chunk(8).Select(ids =>
				$"if ({string.Join(" or ", ids.Select(id => $"@ch.ethnicity.id == {id}"))})\n  return false\nend if\n")) + "return true";
			var roleProg = CultureToolkitProgSeeder.Upsert(context, era, $"accent.non-native.{languageId}", $"CultureNonNativeAccent{languageId}",
				body, ProgVariableTypes.Boolean, [(ProgVariableTypes.Chargen, "ch")], conflicts);
			generated.Add(roleProg.Id);
			var specification = pack.Languages.SingleOrDefault(x => CultureToolkitCatalogue.Text(x, "key") == languageKey);
			CultureAccentPolicyResult Policy(SeederManagedRecord record, Accent accent) => sourceInfo.TryGetValue(record.StableKey, out var source)
				? source.Policy : new("canonical-installed-era", specification.ValueKind != JsonValueKind.Undefined
					? specification.GetProperty("labels").EnumerateObject().Select(x => x.Name).ToArray() : [era], CultureToolkitAccentPolicy.IsLearner(accent.Group) ||
					CultureToolkitAccentPolicy.IsLearner(accent.Name) || language.DefaultLearnerAccentId == accent.Id ? "learner_or_foreign" : "native-tradition");
			var owned = context.SeederManagedRecords.Where(x => x.Seeder == "CultureSeeder" && x.EntityType == "Accent").ToArray()
				.Select(x => (Record: x, Accent: context.Accents.Find(x.LogicalId)!)).Where(x => x.Accent?.LanguageId == languageId).ToList();
			var writer = new CultureToolkitEntityWriter(context, era, conflicts);
			if (specification.ValueKind != JsonValueKind.Undefined && !owned.Any(x => Policy(x.Record, x.Accent).Role == "native-tradition" && Policy(x.Record, x.Accent).AllowedPacks.Contains(era)))
			{
				var fallback = specification.GetProperty("fallback_accent_if_no_legacy_accent");
				var local = writer.Upsert(languageKey + ".accent.local", new Accent { LanguageId = languageId,
					Name = CultureToolkitCatalogue.Text(fallback, "name"), Group = CultureToolkitCatalogue.Text(fallback, "group"),
					Suffix = CultureToolkitCatalogue.Text(fallback, "suffix"), VagueSuffix = CultureToolkitCatalogue.Text(fallback, "vague_suffix"),
					Description = CultureToolkitCatalogue.Text(fallback, "description"),
					Difficulty = (int)Enum.Parse<MudSharp.RPG.Checks.Difficulty>(CultureToolkitCatalogue.Text(fallback, "difficulty")) },
					independentlyManagedFields: new HashSet<string> { nameof(Accent.ChargenAvailabilityProgId) });
				owned.Add((CultureToolkitManagedEntities.Find(context, "Accent", languageKey + ".accent.local")!, local));
			}
			if (specification.ValueKind != JsonValueKind.Undefined && !owned.Any(x => Policy(x.Record, x.Accent).Role == "learner_or_foreign" && Policy(x.Record, x.Accent).AllowedPacks.Contains(era)))
			{
				var learner = writer.Upsert(languageKey + ".accent.learner", new Accent { LanguageId = languageId, Name = "Learner", Group = "learner",
					Suffix = "with a learner's accent", VagueSuffix = "with an unfamiliar accent",
					Description = "The hesitant pronunciation of someone learning the language.", Difficulty = 6 },
					independentlyManagedFields: new HashSet<string> { nameof(Accent.ChargenAvailabilityProgId) });
				owned.Add((CultureToolkitManagedEntities.Find(context, "Accent", languageKey + ".accent.learner")!, learner));
			}
			var details = new List<CultureAccentReceipt>();
			foreach (var (record, accent) in owned.DistinctBy(x => x.Accent.Id))
			{
				var policy = Policy(record, accent);
				var allowed = policy.AllowedPacks.Contains(era);
				var key = $"accent.availability.{accent.Id}";
				var owner = CultureToolkitManagedEntities.Find(context, "AccentAvailability", key);
				var sourceBaseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
				var old = owner?.SeedBaseline is null ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(owner.SeedBaseline)!;
				var original = old.TryGetValue("source", out var savedSource) ? JsonSerializer.Deserialize<long?>(savedSource) :
					sourceInfo.TryGetValue(record.StableKey, out var info) ? info.Predicate :
					JsonSerializer.Deserialize<long?>(sourceBaseline.GetValueOrDefault(nameof(Accent.ChargenAvailabilityProgId)) ?? "null");
				var baselinePointer = sourceBaseline.GetValueOrDefault(nameof(Accent.ChargenAvailabilityProgId)) ?? "null";
				if (record.StableKey.EndsWith(".accent.learner", StringComparison.Ordinal) && original is long falseId &&
					context.FutureProgs.Find(falseId) is { FunctionName: "AlwaysFalse", FunctionText: var falseBody } && falseBody.Trim().Equals("return false", StringComparison.OrdinalIgnoreCase)) original = null;
				if (owner is null)
				{
					owner = new SeederManagedRecord { Seeder = "CultureSeeder", EntityType = "AccentAvailability", StableKey = key,
						Module = era, LogicalId = accent.Id, ManifestVersion = "2026-09-09", AppliedAt = DateTime.UtcNow,
						SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { ["prog"] = baselinePointer, ["source"] = JsonSerializer.Serialize(original) }) };
					context.SeederManagedRecords.Add(owner);
					context.SaveChanges();
				}
				var predicate = allowed ? "" : "return false\n";
				if (original is long originalId)
				{
					var sourceProg = context.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == originalId);
					var arguments = !sourceProg.AcceptsAnyParameters && sourceProg.FutureProgsParameters.Count == 0 ? "" : "@ch";
					predicate += $"if (@{sourceProg.FunctionName}({arguments}) == false)\n  return false\nend if\n";
					generated.Add(originalId);
				}
				predicate += policy.Role == "learner_or_foreign" ? $"return @{roleProg.FunctionName}(@ch)" : "return true";
				// An unconditional early return would leave unreachable statements in the prog compiler.
				if (!allowed) predicate = "return false";
				var combined = CultureToolkitProgSeeder.Upsert(context, era, key + ".combined", $"CultureAccentAvailable{accent.Id}", predicate,
					ProgVariableTypes.Boolean, [(ProgVariableTypes.Chargen, "ch")], conflicts);
				generated.Add(combined.Id);
				var merged = SeederManagedRecordReconciler.Reconcile(owner,
					new Dictionary<string, string> { ["prog"] = JsonSerializer.Serialize(accent.ChargenAvailabilityProgId), ["source"] = JsonSerializer.Serialize(original) },
					new Dictionary<string, string> { ["prog"] = JsonSerializer.Serialize(combined.Id), ["source"] = JsonSerializer.Serialize(original) }, false, conflicts);
				accent.ChargenAvailabilityProgId = JsonSerializer.Deserialize<long?>(merged["prog"]);
				sourceBaseline.Remove(nameof(Accent.ChargenAvailabilityProgId));
				record.SeedBaseline = JsonSerializer.Serialize(sourceBaseline);
				details.Add(new(accent.Id, record.StableKey, languageKey, policy.Rule, policy.AllowedPacks, policy.Role, original,
					accent.ChargenAvailabilityProgId, language.DefaultLearnerAccentId == accent.Id,
					accent.ChargenAvailabilityProgId != combined.Id || combined.FunctionText != predicate, allowed));
			}
			var defaultKey = languageKey + ".default-learner";
			var defaultRecord = CultureToolkitManagedEntities.Find(context, "LanguageLearnerDefault", defaultKey);
			var preferred = details.FirstOrDefault(x => x.IsDefault && x.EraEligible && x.Role == "learner_or_foreign") ??
				details.Where(x => x.EraEligible && x.Role == "learner_or_foreign").OrderBy(x => x.AccentId).FirstOrDefault();
			if (preferred is not null)
			{
				if (defaultRecord is null)
				{
					// The retained source pointer, never the current edited pointer, is the
					// initial baseline when upgrading an already-installed current toolkit.
					var expected = owned.FirstOrDefault(x => sourceInfo.TryGetValue(x.Record.StableKey, out var info) && info.SourceDefault).Accent
						?? owned.FirstOrDefault(x => x.Record.StableKey == languageKey + ".accent.learner").Accent;
					defaultRecord = new SeederManagedRecord { Seeder = "CultureSeeder", EntityType = "LanguageLearnerDefault", StableKey = defaultKey,
						Module = era, LogicalId = languageId, ManifestVersion = "2026-09-09", AppliedAt = DateTime.UtcNow,
						SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { ["accent"] = JsonSerializer.Serialize((long?)expected?.Id ??
							(installedLanguages is null ? language.DefaultLearnerAccentId : null)) }) };
					context.SeederManagedRecords.Add(defaultRecord);
					context.SaveChanges();
				}
				var merged = CultureToolkitManagedEntities.Reconcile(context, era, "LanguageLearnerDefault", defaultKey, languageId,
					false,
					new Dictionary<string, string> { ["accent"] = JsonSerializer.Serialize(language.DefaultLearnerAccentId) },
					new Dictionary<string, string> { ["accent"] = JsonSerializer.Serialize(preferred.AccentId) }, conflicts);
				language.DefaultLearnerAccentId = JsonSerializer.Deserialize<long?>(merged["accent"]);
			}
			else conflicts.Add($"Language {languageKey} ({languageId}) has no era-eligible learner path; default {language.DefaultLearnerAccentId}.");
			context.SaveChanges();
			result.Add(new(languageId, details.Where(x => x.EraEligible && x.Role == "native-tradition").Select(x => x.AccentId).ToArray(),
				details.Where(x => x.Role == "learner_or_foreign").Select(x => x.AccentId).ToArray(), roleProg.Id)
				{ Details = details.Select(x => x with { IsDefault = x.AccentId == language.DefaultLearnerAccentId }).ToArray() });
		}
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var id in generated) compiler.Compile(id);
		return result;
	}
}
