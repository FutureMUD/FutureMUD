#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonMagicInstaller
{
	private sealed record Contribution(Type Type, string Key, string Name, string[] Dependencies, IReadOnlyDictionary<string, long>? Bindings = null);
	private static string EntityType(Type type) => type.Name;
	private static readonly Lazy<JsonDocument> ReviewedManifest = new(() =>
	{
		using var stream = typeof(ArmageddonMagicInstaller).Assembly.GetManifestResourceStream("ArmageddonReviewedContentManifest")
			?? throw new InvalidOperationException("Reviewed Armageddon ownership manifest is unavailable.");
		return JsonDocument.Parse(stream);
	});
	private static object? Find(FuturemudDatabaseContext db, Contribution contribution, SeederManagedRecord record) =>
		record.LogicalId is not { } id ? null : db.Find(contribution.Type,
			contribution.Type == typeof(GameItemProto) || contribution.Type == typeof(GameItemComponentProto)
				? [id, record.RevisionNumber ?? -1] : [id]);

	private static Dictionary<string, string> Fields(FuturemudDatabaseContext db, object row)
	{
		var entry = db.Entry(row);
		var owned = ReviewedManifest.Value.RootElement.GetProperty("ownedFields").GetProperty(entry.Metadata.ClrType.Name)
			.EnumerateArray().Select(x => x.GetString()!).ToHashSet(StringComparer.Ordinal);
		return entry.Properties.Where(x => owned.Contains(x.Metadata.Name))
			.ToDictionary(x => x.Metadata.Name, x => JsonSerializer.Serialize(x.CurrentValue, x.Metadata.ClrType), StringComparer.Ordinal);
	}
	private static void ValidateManifest(List<Contribution> contributions)
	{
		var root = ReviewedManifest.Value.RootElement;
		if (root.GetProperty("package").GetString() != Package || root.GetProperty("module").GetString() != Module ||
			root.GetProperty("version").GetString() != ManifestVersion || !root.GetProperty("partial").GetBoolean())
			throw new InvalidOperationException("Reviewed partial ownership manifest metadata does not match this installer.");
		var declared = root.GetProperty("contributions").EnumerateArray().ToDictionary(x => x.GetProperty("key").GetString()!);
		if (declared.Count != contributions.Count) throw new InvalidOperationException("Reviewed contribution inventory drift.");
		foreach (var contribution in contributions)
			if (!declared.TryGetValue(contribution.Key, out var row) || row.GetProperty("entityType").GetString() != contribution.Type.Name ||
				!row.GetProperty("dependencies").EnumerateArray().Select(x => x.GetString()).SequenceEqual(contribution.Dependencies))
				throw new InvalidOperationException($"Reviewed manifest contribution drift: {contribution.Key}");
	}

	private static void Reconcile(FuturemudDatabaseContext db, Contribution contribution, object row, object desired,
		SeederManagedRecord record, bool created, List<string> messages)
	{
		var current = Fields(db, row);
		var next = Fields(db, desired);
		// Dependencies are package metadata, not mutable entity fields. Preserve their previous
		// baseline separately from the builder's values; never adopt a clone or a display name.
		const string dependencies = "$dependencies";
		next[dependencies] = JsonSerializer.Serialize(contribution.Dependencies);
		// Explicit external selections are reproducible bindings, not adopted owned rows.
		next["$bindings"] = JsonSerializer.Serialize(contribution.Bindings);
		var oldBaseline = record.SeedBaseline is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline);
		if (oldBaseline?.TryGetValue(dependencies, out var oldDependencies) == true) current[dependencies] = oldDependencies;
		if (oldBaseline?.TryGetValue("$bindings", out var oldBindings) == true) current["$bindings"] = oldBindings;
		if (row is FutureProg prog && desired is FutureProg desiredProg)
		{
			foreach (var parameter in prog.FutureProgsParameters) current[$"parameter:{parameter.ParameterIndex}"] = Parameter(parameter);
			foreach (var parameter in desiredProg.FutureProgsParameters) next[$"parameter:{parameter.ParameterIndex}"] = Parameter(parameter);
		}
		if (row is GameItemProto item && desired is GameItemProto desiredItem)
		{
			foreach (var link in item.GameItemProtosGameItemComponentProtos) current[LinkKey(link)] = "true";
			foreach (var link in desiredItem.GameItemProtosGameItemComponentProtos) next[LinkKey(link)] = "true";
		}
		var merged = SeederManagedRecordReconciler.Reconcile(record, current, next, created, messages);
		foreach (var property in db.Entry(row).Properties.Where(x => !x.Metadata.IsPrimaryKey() && x.Metadata.Name != "EditableItemId"))
			if (merged.TryGetValue(property.Metadata.Name, out var value)) property.CurrentValue = JsonSerializer.Deserialize(value, property.Metadata.ClrType);
		if (row is FutureProg actualProg && desired is FutureProg parameters)
		{
			foreach (var obsolete in actualProg.FutureProgsParameters.Where(x => !merged.ContainsKey($"parameter:{x.ParameterIndex}")).ToArray())
			{ actualProg.FutureProgsParameters.Remove(obsolete); db.Remove(obsolete); }
			foreach (var parameter in parameters.FutureProgsParameters)
			{
				var key = $"parameter:{parameter.ParameterIndex}";
				if (!merged.TryGetValue(key, out var value)) continue;
				var fields = JsonSerializer.Deserialize<string[]>(value)!;
				var actual = actualProg.FutureProgsParameters.SingleOrDefault(x => x.ParameterIndex == parameter.ParameterIndex);
				if (actual is null) { actual = new FutureProgsParameter { ParameterIndex = parameter.ParameterIndex }; actualProg.FutureProgsParameters.Add(actual); }
				actual.ParameterName = fields[0]; actual.ParameterTypeDefinition = fields[1];
			}
		}
		if (row is GameItemProto actualItem && desired is GameItemProto links)
		{
			foreach (var obsolete in actualItem.GameItemProtosGameItemComponentProtos.Where(x => !merged.ContainsKey(LinkKey(x))).ToArray())
			{ actualItem.GameItemProtosGameItemComponentProtos.Remove(obsolete); db.Remove(obsolete); }
			foreach (var link in links.GameItemProtosGameItemComponentProtos)
				if (merged.ContainsKey(LinkKey(link)) && !actualItem.GameItemProtosGameItemComponentProtos.Any(x => LinkKey(x) == LinkKey(link)))
					actualItem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{ GameItemComponentProtoId = link.GameItemComponentProtoId, GameItemComponentRevision = link.GameItemComponentRevision });
		}
		record.ManifestVersion = ManifestVersion;
		if (created || current.Count != merged.Count || current.Any(x => !merged.TryGetValue(x.Key, out var value) || value != x.Value))
			record.AppliedAt = DateTime.UtcNow;
	}
	private static string Parameter(FutureProgsParameter parameter) => JsonSerializer.Serialize(new[] { parameter.ParameterName, parameter.ParameterTypeDefinition });
	private static string LinkKey(GameItemProtosGameItemComponentProtos link) => $"component:{link.GameItemComponentProtoId}:{link.GameItemComponentRevision}";

	private static T Apply<T>(FuturemudDatabaseContext db, Contribution contribution, T desired,
		Dictionary<string, SeederManagedRecord> records, List<string> messages, bool newlyAllocated = false) where T : class
	{
		var created = !records.TryGetValue(contribution.Key, out var record);
		T row;
		if (created)
		{
			var nameProperty = typeof(T).GetProperty(typeof(T) == typeof(FutureProg) ? "FunctionName" : "Name")!;
			var desiredName = (string)nameProperty.GetValue(desired)!;
			if (db.Set<T>().Any(x => EF.Property<string>(x, nameProperty.Name) == desiredName))
				throw new InvalidOperationException($"{contribution.Key}: unowned {nameProperty.Name} collision; no automatic adoption.");
			row = desired; db.Add(row); db.SaveChanges();
			record = new SeederManagedRecord { Seeder = Package, Module = Module, EntityType = EntityType(typeof(T)),
				StableKey = contribution.Key, LogicalId = (long)typeof(T).GetProperty("Id")!.GetValue(row)!,
				RevisionNumber = typeof(T).GetProperty("RevisionNumber")?.GetValue(row) as int? };
			db.SeederManagedRecords.Add(record); records.Add(contribution.Key, record);
		}
		else row = (T)Find(db, contribution, record!)!;
		if (row is FutureProg prog) db.Entry(prog).Collection(x => x.FutureProgsParameters).Load();
		if (row is GameItemProto item) db.Entry(item).Collection(x => x.GameItemProtosGameItemComponentProtos).Load();
		Reconcile(db, contribution, row, desired, record!, created || newlyAllocated, messages);
		db.SaveChanges();
		return row;
	}
}
