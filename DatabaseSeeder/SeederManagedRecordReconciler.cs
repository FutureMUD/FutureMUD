#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MudSharp.Models;

namespace DatabaseSeeder;

/// <summary>Field-level ownership for already resolved seeder identities.</summary>
public static class SeederManagedRecordReconciler
{
	public static IReadOnlyDictionary<string, string> Reconcile(SeederManagedRecord record,
		IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> desired,
		bool newlyCreated, ICollection<string> conflicts)
	{
		var previous = record.SeedBaseline is null ? null :
			JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline)
			?? throw new InvalidOperationException($"Invalid seed baseline: {record.StableKey}");
		var merged = current.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
		foreach (var key in desired.Keys.Union(previous?.Keys ?? Enumerable.Empty<string>(), StringComparer.Ordinal))
		{
			var hasActual = current.TryGetValue(key, out var actual);
			var hasNext = desired.TryGetValue(key, out var next);
			string? old = null;
			var hadOld = previous?.TryGetValue(key, out old) ?? false;
			var unchanged = previous is not null && hasActual == hadOld && actual == old;
			if (newlyCreated || unchanged)
			{
				if (hasNext) merged[key] = next!;
				else merged.Remove(key);
			}
			else if (hasActual != hasNext || actual != next)
			{
				conflicts.Add($"{record.EntityType} {record.StableKey}: preserved {(previous is null ? "unbaselined field" : "builder edit")} {key}");
			}
		}
		// Store the desired stock baseline, never the builder's value. A subsequent run
		// must not reinterpret a retained override as an unchanged seeded field.
		record.SeedBaseline = JsonSerializer.Serialize(desired.OrderBy(x => x.Key, StringComparer.Ordinal)
			.ToDictionary(x => x.Key, x => x.Value));
		record.AppliedFingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(record.SeedBaseline)))
			.ToLowerInvariant();
		return merged;
	}
}
