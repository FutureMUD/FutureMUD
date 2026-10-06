#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTraditionInstaller
{
	private static object? Find(FuturemudDatabaseContext db, Definition definition, SeederManagedRecord record) =>
		record.LogicalId is { } id ? db.Find(definition.Type, id) : null;

	private static Dictionary<string, string> Fields(FuturemudDatabaseContext db, object row) => db.Entry(row).Properties
		.Where(x => OwnedFields[row.GetType()].Contains(x.Metadata.Name))
		.ToDictionary(x => x.Metadata.Name, x => JsonSerializer.Serialize(x.CurrentValue, x.Metadata.ClrType));

	private static T Apply<T>(FuturemudDatabaseContext db, Definition definition, T desired,
		Dictionary<string, SeederManagedRecord> records, ArmageddonTraditionInstallPlan plan, List<string> messages) where T : class
	{
		var created = !records.TryGetValue(definition.Key, out var record);
		T row;
		if (created)
		{
			row = desired; db.Add(row); db.SaveChanges();
			var id = (long)typeof(T).GetProperty("Id")!.GetValue(row)!;
			if (db.SeederManagedRecords.Any(x => x.EntityType == typeof(T).Name && x.LogicalId == id))
				throw new InvalidOperationException($"{definition.Key}: retained logical identity #{id}; no ownership takeover.");
			record = new SeederManagedRecord { Seeder = Package, Module = Module, StableKey = definition.Key,
				EntityType = typeof(T).Name, LogicalId = id };
			db.SeederManagedRecords.Add(record); records.Add(definition.Key, record);
		}
		else row = (T)Find(db, definition, record!)!;
		var actual = Fields(db, row); var next = Fields(db, desired);
		next["$dependencies"] = JsonSerializer.Serialize(definition.Dependencies);
		next["$bindings"] = JsonSerializer.Serialize(new
		{
			plan.School, plan.SourceResource, plan.ReserveResource, plan.Decorator, plan.AlwaysFalseProg, plan.AlwaysTrueProg, plan.GatheringTemplate,
			ImplementedSpells = plan.ImplementedSpells.OrderBy(x => x.Key, StringComparer.Ordinal).ToDictionary(x => x.Key, x => x.Value),
			SupportSkills = plan.SupportSkills.OrderBy(x => x.Key, StringComparer.Ordinal).ToDictionary(x => x.Key, x => x.Value),
			AllowedMethods = Variants.ToDictionary(x => x, x => Methods(plan, x).Order().ToArray())
		});
		next["$source"] = JsonSerializer.Serialize(SourceRows.Where(x => definition.Key == x.Key + ".skill" || definition.Key == x.Key + ".skill.cap").ToArray());
		var baseline = record!.SeedBaseline is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline);
		foreach (var key in new[] { "$dependencies", "$bindings", "$source" })
			if (baseline?.TryGetValue(key, out var old) == true) actual[key] = old;
		var merged = SeederManagedRecordReconciler.Reconcile(record, actual, next, created, messages);
		foreach (var property in db.Entry(row).Properties.Where(x => OwnedFields[typeof(T)].Contains(x.Metadata.Name)))
			if (merged.TryGetValue(property.Metadata.Name, out var value)) property.CurrentValue = JsonSerializer.Deserialize(value, property.Metadata.ClrType);
		if (created || actual.Count != merged.Count || actual.Any(x => !merged.TryGetValue(x.Key, out var value) || value != x.Value)) record.AppliedAt = DateTime.UtcNow;
		record.ManifestVersion = Version; db.SaveChanges(); return row;
	}

	private static bool NameCollision(FuturemudDatabaseContext db, Definition definition)
	{
		var name = definition.Name.ToLowerInvariant();
		return definition.Type == typeof(TraitExpression) ? db.TraitExpressions.Any(x => x.Name.ToLower() == name) :
			definition.Type == typeof(TraitDefinition) ? db.TraitDefinitions.Any(x => x.Name.ToLower() == name) :
			definition.Type == typeof(Improver) ? db.Improvers.Any(x => x.Name.ToLower() == name) :
			definition.Type == typeof(MagicCapability) ? db.MagicCapabilities.Any(x => x.Name.ToLower() == name) : db.Merits.Any(x => x.Name.ToLower() == name);
	}
}
