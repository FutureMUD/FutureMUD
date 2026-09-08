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

public sealed record CultureAccentResolution(long LanguageId, IReadOnlyList<long> NativeAccentIds,
	IReadOnlyList<long> RestrictedAccentIds, long EligibilityProgId);

/// <summary>Native speakers choose retained native/regional accents; acquired-language learner defaults stay intact.</summary>
public static class CultureToolkitAccents
{
	public static IReadOnlyList<CultureAccentResolution> Upsert(FuturemudDatabaseContext context, string era,
		IReadOnlyDictionary<long, IReadOnlyList<long>> nativeEthnicities, ICollection<string> conflicts)
	{
		var result = new List<CultureAccentResolution>();
		foreach (var language in nativeEthnicities)
		{
			var accents = context.Accents.Where(x => x.LanguageId == language.Key).ToArray();
			var learner = context.Languages.Find(language.Key)!.DefaultLearnerAccentId;
			var native = accents.Where(x => x.Id != learner && !new[] { "foreign", "crude", "learner" }.Contains(x.Group.ToLowerInvariant())).ToArray();
			if (native.Length == 0)
				throw new InvalidOperationException($"Language {language.Key} ({context.Languages.Find(language.Key)!.Name}) has no retained native/regional accent; resolve it before activation.");
			var body = string.Concat(language.Value.Distinct().Chunk(8).Select(ids =>
				$"if ({string.Join(" or ", ids.Select(id => $"@ch.ethnicity.id == {id}"))})\n  return false\nend if\n")) + "return true";
			var prog = CultureToolkitProgSeeder.Upsert(context, era, $"accent.non-native.{language.Key}", $"CultureNonNativeAccent{language.Key}",
				body, ProgVariableTypes.Boolean, [(ProgVariableTypes.Chargen, "ch")], conflicts);
			var restricted = new List<long>();
			foreach (var accent in accents.Except(native))
			{
				var source = context.SeederManagedRecords.SingleOrDefault(x => x.Seeder == "CultureSeeder" && x.EntityType == "Accent" && x.LogicalId == accent.Id);
				if (source is null) continue; // Independent builder accents remain independently configured.
				var key = $"accent.availability.{accent.Id}";
				var stored = CultureToolkitManagedEntities.Find(context, "AccentAvailability", key);
				var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(source.SeedBaseline!)!;
				if (stored is null)
				{
					// Only unrestricted original accents can safely be narrowed. Existing custom conditions stay authoritative.
					if (accent.ChargenAvailabilityProgId is not null || baseline.GetValueOrDefault(nameof(Accent.ChargenAvailabilityProgId)) != "null") continue;
					stored = new SeederManagedRecord
					{
						Seeder = "CultureSeeder", EntityType = "AccentAvailability", StableKey = key, Module = era, LogicalId = accent.Id,
						ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow,
						SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { ["prog"] = "null" })
					};
					context.SeederManagedRecords.Add(stored);
				}
				var merged = SeederManagedRecordReconciler.Reconcile(stored,
					new Dictionary<string, string> { ["prog"] = JsonSerializer.Serialize(accent.ChargenAvailabilityProgId) },
					new Dictionary<string, string> { ["prog"] = JsonSerializer.Serialize(prog.Id) }, false, conflicts);
				accent.ChargenAvailabilityProgId = JsonSerializer.Deserialize<long?>(merged["prog"]);
				baseline.Remove(nameof(Accent.ChargenAvailabilityProgId));
				source.SeedBaseline = JsonSerializer.Serialize(baseline);
				restricted.Add(accent.Id);
			}
			context.SaveChanges();
			result.Add(new(language.Key, native.Select(x => x.Id).ToArray(), restricted, prog.Id));
		}
		using var compiler = new OfflineProgCompilation(context.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var prog in result) compiler.Compile(prog.EligibilityProgId);
		return result;
	}
}
