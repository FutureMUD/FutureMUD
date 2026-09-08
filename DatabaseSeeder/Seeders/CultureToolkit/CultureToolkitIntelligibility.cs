#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureSourceLanguageEdge(string ListenerKey, string TargetKey, int Difficulty, string SourceKey);
public sealed record CultureResolvedLanguageEdge(string ListenerKey, string TargetKey, long ListenerLanguageId,
	long TargetLanguageId, int Difficulty, string Policy, string? SourceKey);

public static class CultureToolkitIntelligibility
{
	public static IReadOnlyList<CultureResolvedLanguageEdge> Reconcile(FuturemudDatabaseContext context,
		CultureToolkitCatalogue catalogue, CultureToolkitPack pack, IReadOnlyDictionary<string, Language> languages,
		IReadOnlyList<CultureSourceLanguageEdge> originalSource, ICollection<string> conflicts,
		IReadOnlySet<long>? newlyCreatedLanguageIds = null)
	{
		var proposals = new Dictionary<(long Listener, long Target), (string ListenerKey, string TargetKey, int Value, string Policy)>();
		void Propose(string listenerKey, string targetKey, int difficulty, string policy)
		{
			if (!languages.TryGetValue(listenerKey, out var listener) || !languages.TryGetValue(targetKey, out var target)) return;
			if (listener.Id == target.Id) throw new InvalidOperationException($"Refusing alias/self intelligibility edge {listenerKey} -> {targetKey}.");
			var key = (listener.Id, target.Id);
			if (proposals.TryGetValue(key, out var prior) && prior.Value != difficulty)
				throw new InvalidOperationException($"Conflicting directed settings for {listenerKey} -> {targetKey}.");
			proposals[key] = (listenerKey, targetKey, difficulty, policy);
		}
		foreach (var edge in pack.DirectedEdges)
			Propose(CultureToolkitCatalogue.Text(edge, "listener_language"), CultureToolkitCatalogue.Text(edge, "target_language"),
				edge.GetProperty("difficulty_value").GetInt32(), "explicit-matrix");
		foreach (var edge in catalogue.Document("data.mutual_intelligibility.json").GetProperty("conditional_legacy_pairs").EnumerateArray())
		{
			var listener = CultureToolkitCatalogue.Text(edge, "first_language");
			var target = CultureToolkitCatalogue.Text(edge, "second_language");
			var difficulty = (int)Enum.Parse<Difficulty>(CultureToolkitCatalogue.Text(edge, "difficulty"));
			Propose(listener, target, difficulty, "conditional-retained");
			if (edge.GetProperty("two_way").GetBoolean()) Propose(target, listener, difficulty, "conditional-retained");
		}
		var sources = new Dictionary<(long Listener, long Target), List<CultureSourceLanguageEdge>>();
		foreach (var source in originalSource)
		{
			if (!languages.TryGetValue(source.ListenerKey, out var listener) || !languages.TryGetValue(source.TargetKey, out var target)) continue;
			if (listener.Id == target.Id) throw new InvalidOperationException($"Source edge became an alias: {source.SourceKey}");
			var key = (listener.Id, target.Id);
			if (!sources.TryGetValue(key, out var values)) sources[key] = values = [];
			values.Add(source);
		}
		foreach (var entry in sources.Where(x => !proposals.ContainsKey(x.Key)))
		{
			var values = entry.Value.Select(x => x.Difficulty <= (int)Difficulty.Insane ? Math.Max((int)Difficulty.VeryHard, x.Difficulty) : x.Difficulty).Distinct().ToArray();
			if (values.Length != 1)
				throw new InvalidOperationException($"Resolve disagreeing retained settings before activation: {string.Join(", ", entry.Value.Select(x => x.SourceKey))}");
			var source = entry.Value[0];
			Propose(source.ListenerKey, source.TargetKey, values[0], values[0] <= (int)Difficulty.Insane ? "retained-source-floor" : "retained-disabled");
		}
		foreach (var (endpoints, proposal) in proposals)
		{
			var key = $"mi.{proposal.ListenerKey}.hears.{proposal.TargetKey}";
			var row = context.MutualIntelligabilities.Find(endpoints.Listener, endpoints.Target);
			var managed = CultureToolkitManagedEntities.Find(context, "MutualIntelligability", key);
			var hasSource = sources.TryGetValue(endpoints, out var source);
			var freshEndpoints = newlyCreatedLanguageIds?.Contains(endpoints.Listener) == true || newlyCreatedLanguageIds?.Contains(endpoints.Target) == true;
			if (row is null && (managed is not null || hasSource && !freshEndpoints))
			{
				conflicts.Add($"{key}: existing/source edge is absent; preserved possible builder deletion.");
				continue;
			}
			var fresh = row is null;
			if (managed is null && hasSource && row is not null)
			{
				// The caller supplied a source-qualified baseline for these already resolved IDs.
				context.SeederManagedRecords.Add(new SeederManagedRecord
				{
					Seeder = "CultureSeeder", Module = pack.Era, EntityType = "MutualIntelligability", StableKey = key,
					LogicalId = endpoints.Listener, ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow,
					SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string> { ["difficulty"] =
						(source!.FirstOrDefault(x => x.Difficulty == row.IntelligabilityDifficulty) ?? source![0]).Difficulty.ToString(CultureInfo.InvariantCulture) })
				});
				context.SaveChanges();
			}
			var actual = row is null ? new Dictionary<string, string>() : new Dictionary<string, string> { ["difficulty"] = row.IntelligabilityDifficulty.ToString(CultureInfo.InvariantCulture) };
			var merged = CultureToolkitManagedEntities.Reconcile(context, pack.Era, "MutualIntelligability", key, endpoints.Listener, fresh,
				actual, new Dictionary<string, string> { ["difficulty"] = proposal.Value.ToString(CultureInfo.InvariantCulture) }, conflicts);
			if (!merged.TryGetValue("difficulty", out var value)) continue;
			if (row is null)
			{
				row = new MutualIntelligability { ListenerLanguageId = endpoints.Listener, TargetLanguageId = endpoints.Target };
				context.MutualIntelligabilities.Add(row);
			}
			row.IntelligabilityDifficulty = int.Parse(value, CultureInfo.InvariantCulture);
			context.SaveChanges();
		}
		var keysById = languages.GroupBy(x => x.Value.Id).ToDictionary(x => x.Key, x => x.First().Key);
		var ids = keysById.Keys.ToList();
		return context.MutualIntelligabilities.Where(x => ids.Contains(x.ListenerLanguageId) && ids.Contains(x.TargetLanguageId))
			.AsEnumerable().OrderBy(x => x.ListenerLanguageId).ThenBy(x => x.TargetLanguageId).Select(row =>
			{
				var endpoints = (row.ListenerLanguageId, row.TargetLanguageId);
				var hasProposal = proposals.TryGetValue(endpoints, out var proposed);
				var policy = !hasProposal ? "unlisted-existing" : row.IntelligabilityDifficulty == proposed.Value ? proposed.Policy : "preserved-override";
				return new CultureResolvedLanguageEdge(keysById[row.ListenerLanguageId], keysById[row.TargetLanguageId], row.ListenerLanguageId,
					row.TargetLanguageId, row.IntelligabilityDifficulty, policy, sources.TryGetValue(endpoints, out var provenance) ? string.Join("; ", provenance.Select(x => x.SourceKey)) : null);
			}).ToArray();
	}
}
