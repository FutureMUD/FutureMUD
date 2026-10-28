#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AIStorytellerSeeder
{
	private readonly List<string> _reconciliationMessages = [];
	private bool _unownedProgFound;
	private const string Module = "starter-pack";
	private sealed record ParameterDefinition(int Index, string Name, string Type);

	// Use mapped scalar fields only: navigation properties and computed compatibility aliases
	// are not ownership evidence. Each value is JSON encoded to distinguish null from empty.
	private static Dictionary<string, string> ScalarValues<T>(FuturemudDatabaseContext context, T entity) where T : class
	{
		var properties = context.Model.FindEntityType(typeof(T))!.GetProperties().Where(x => x.Name != "Id");
		var values = properties.ToDictionary(x => x.Name,
			x => JsonSerializer.Serialize(x.PropertyInfo!.GetValue(entity), x.ClrType), StringComparer.Ordinal);
		if (entity is FutureProg prog)
			values["Parameters"] = JsonSerializer.Serialize(prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
				.Select(x => new ParameterDefinition(x.ParameterIndex, x.ParameterName, x.ParameterTypeDefinition)));
		return values;
	}

	private T ReconcileEntity<T>(FuturemudDatabaseContext context, string stableKey, T desired,
		Dictionary<string, string> legacy, List<T> named, bool allowLegacy = true) where T : class
	{
		var type = typeof(T).Name;
		var records = context.SeederManagedRecords.Where(x => x.Seeder == nameof(AIStorytellerSeeder) &&
			x.Module == Module && x.EntityType == type && x.StableKey == stableKey).ToList();
		if (records.Count > 1) throw new InvalidOperationException($"Duplicate storyteller stock claim: {stableKey}.");
		var record = records.SingleOrDefault();
		var row = record is null ? named.SingleOrDefault() : context.Set<T>().Find(record.LogicalId);
		if (record is not null && row is null) throw new InvalidOperationException($"Missing managed {type}: {stableKey}; repair its identity before rerunning.");
		if (named.Count > 1 || (row is not null && named.Any(x => !ReferenceEquals(x, row))))
			throw new InvalidOperationException($"Conflicting stock identity: {stableKey}.");
		var newlyCreated = row is null;
		if (row is FutureProg existingProg) context.Entry(existingProg).Collection(x => x.FutureProgsParameters).Load();
		var desiredValues = ScalarValues(context, desired);
		var actual = row is null ? desiredValues : ScalarValues(context, row);
		if (row is not null)
		{
			var id = (long)typeof(T).GetProperty("Id")!.GetValue(row)!;
			var competing = context.SeederManagedRecords.Where(x => x.EntityType == type && x.LogicalId == id).ToList();
			if (competing.Any(x => record is null || x.Id != record.Id))
				throw new InvalidOperationException($"Competing ownership of {type} #{id}.");
		}
		if (record is null && !newlyCreated && (!allowLegacy || !Equivalent(actual, legacy)))
		{
			_reconciliationMessages.Add($"Preserved unrecognized/customized {type}: {stableKey}; no ownership inferred from its name.");
			return row!;
		}
		if (newlyCreated)
		{
			row = desired;
			context.Set<T>().Add(row);
			context.SaveChanges();
		}
		if (record is null)
		{
			record = new SeederManagedRecord
			{
				Seeder = nameof(AIStorytellerSeeder), Module = Module, EntityType = type, StableKey = stableKey,
				LogicalId = (long)typeof(T).GetProperty("Id")!.GetValue(row)!,
				SeedBaseline = newlyCreated ? null : JsonSerializer.Serialize(actual)
			};
			context.SeederManagedRecords.Add(record);
		}
		var merged = SeederManagedRecordReconciler.Reconcile(record, actual, desiredValues, newlyCreated, _reconciliationMessages);
		foreach (var property in context.Model.FindEntityType(typeof(T))!.GetProperties().Where(x => x.Name != "Id"))
			property.PropertyInfo!.SetValue(row, JsonSerializer.Deserialize(merged[property.Name], property.ClrType));
		if (row is FutureProg prog && actual["Parameters"] != merged["Parameters"])
		{
			context.FutureProgsParameters.RemoveRange(prog.FutureProgsParameters);
			prog.FutureProgsParameters.Clear();
			foreach (var parameter in JsonSerializer.Deserialize<ParameterDefinition[]>(merged["Parameters"])!)
				prog.FutureProgsParameters.Add(new FutureProgsParameter
				{
					FutureProg = prog, ParameterIndex = parameter.Index, ParameterName = parameter.Name,
					ParameterTypeDefinition = parameter.Type
				});
		}
		record.ManifestVersion = "2";
		record.AppliedAt = DateTime.UtcNow;
		context.SaveChanges();
		return row!;
	}

	private static bool HasManagedIdentity<T>(FuturemudDatabaseContext context, string key) where T : class
	{
		var record = context.SeederManagedRecords.FirstOrDefault(x => x.Seeder == nameof(AIStorytellerSeeder) &&
			x.Module == Module && x.EntityType == typeof(T).Name && x.StableKey == key);
		return record?.LogicalId is long id && context.Set<T>().Find(id) is not null;
	}

	private static bool Equivalent(IReadOnlyDictionary<string, string> actual, IReadOnlyDictionary<string, string> expected) =>
		actual.Count == expected.Count && expected.All(x => actual.TryGetValue(x.Key, out var value) &&
			value.Replace("\\r\\n", "\\n", StringComparison.Ordinal) == x.Value.Replace("\\r\\n", "\\n", StringComparison.Ordinal));

	private static string ModernizeDocument(string text)
	{
		// The original source strings remain above as a historical adoption fingerprint.
		// Unsupported focus tools must never be advertised by fresh or upgraded stock.
		var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal);
		var start = normalized.IndexOf("Attention bypass lifecycle:", StringComparison.Ordinal);
		if (start >= 0)
		{
			var end = normalized.IndexOf("Operational principle:", start, StringComparison.Ordinal);
			if (end >= 0) normalized = normalized.Remove(start, end - start);
		}
		start = normalized.IndexOf("Attention focus guidance:", StringComparison.Ordinal);
		if (start >= 0)
		{
			var end = normalized.IndexOf("Output discipline:", start, StringComparison.Ordinal);
			if (end >= 0) normalized = normalized.Remove(start, end - start);
		}
		return string.Join("\n", normalized.Split('\n')
			.Where(x => !x.Contains("BypassAttention", StringComparison.Ordinal) && !x.Contains("EndBypassAttention", StringComparison.Ordinal))) +
			"\n\nRuntime guidance: routine room activity can be grouped into fixed five-second batches. " +
			"Use memory lists for titles and IDs, then ShowMemory for details. List tools accept Offset and Limit; " +
			"follow NextOffset to continue. ShowMemory and ShowReferenceDocument use character offsets. " +
			"Do not repeat a world-changing tool solely because its result exceeded the output limit.";
	}
}
