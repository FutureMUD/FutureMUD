#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonWaterSeeInstaller
{
	private static readonly IReadOnlyDictionary<Type, string[]> OwnedFields = new Dictionary<Type, string[]>
	{
		[typeof(MagicSpell)] = ["SpellLevel", "ScrollInscriptionAllowed", "Name", "Blurb", "Description", "SpellKnownProgId", "MagicSchoolId", "ExclusiveDelay", "NonExclusiveDelay", "Definition",
			"CastingDifficulty", "ResistingDifficulty", "CastingTraitDefinitionId", "ResistingTraitDefinitionId", "EffectDurationExpressionId", "MinimumSuccessThreshold", "AppliedEffectsAreExclusive",
			"CastingEmote", "FailCastingEmote", "TargetEmote", "TargetResistedEmote", "TargetNullEmote", "CastingEmoteFlags", "TargetEmoteFlags"],
		[typeof(TraitExpression)] = ["Name", "Expression"],
		[typeof(FutureProg)] = ["FunctionName", "FunctionComment", "FunctionText", "ReturnTypeDefinition", "Category", "Subcategory", "Public", "AcceptsAnyParameters", "StaticType"]
	};
	private static object? Find(FuturemudDatabaseContext db, Contribution contribution, SeederManagedRecord record) =>
		record.LogicalId is { } id ? db.Find(contribution.Type, id) : null;
	private static void Allocate<T>(FuturemudDatabaseContext db, Contribution contribution, T row, Dictionary<string, SeederManagedRecord> records) where T : class
	{
		db.Add(row); db.SaveChanges();
		var id = (long)typeof(T).GetProperty("Id")!.GetValue(row)!;
		if (db.SeederManagedRecords.Any(x => x.EntityType == typeof(T).Name && x.LogicalId == id))
			throw new InvalidOperationException($"{contribution.Key}: retained logical identity #{id}; no takeover.");
		var record = new SeederManagedRecord { Seeder = Package, Module = Module, StableKey = contribution.Key, EntityType = typeof(T).Name, LogicalId = id };
		db.SeederManagedRecords.Add(record); records.Add(contribution.Key, record);
	}
	private static Dictionary<string, string> Fields(FuturemudDatabaseContext db, object row)
	{
		var fields = db.Entry(row).Properties.Where(x => OwnedFields[row.GetType()].Contains(x.Metadata.Name))
			.ToDictionary(x => x.Metadata.Name, x => JsonSerializer.Serialize(x.CurrentValue, x.Metadata.ClrType));
		if (row is FutureProg prog) fields["$parameters"] = JsonSerializer.Serialize(prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
			.Select(x => new { x.ParameterIndex, x.ParameterName, x.ParameterTypeDefinition }).ToArray());
		return fields;
	}
	private static T Apply<T>(FuturemudDatabaseContext db, Contribution contribution, T desired, Dictionary<string, SeederManagedRecord> records,
		ArmageddonWaterSeeInstallPlan plan, List<string> messages, bool allocated = false) where T : class
	{
		var created = !records.TryGetValue(contribution.Key, out var record);
		if (created)
		{
			var name = typeof(T) == typeof(FutureProg) ? "FunctionName" : "Name";
			var text = (string)typeof(T).GetProperty(name)!.GetValue(desired)!;
			if (db.Set<T>().Any(x => EF.Property<string>(x, name).ToLower() == text.ToLower()))
				throw new InvalidOperationException($"{contribution.Key}: unowned name collision; no adoption.");
			Allocate(db, contribution, desired, records); record = records[contribution.Key];
		}
		var row = created ? desired : (T)Find(db, contribution, record!)!;
		if (row is FutureProg currentProg) db.Entry(currentProg).Collection(x => x.FutureProgsParameters).Load();
		var actual = Fields(db, row); var next = Fields(db, desired);
		next["$bindings"] = JsonSerializer.Serialize(plan);
		var baseline = record!.SeedBaseline is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline);
		if (baseline?.TryGetValue("$bindings", out var old) == true) actual["$bindings"] = old;
		var merged = SeederManagedRecordReconciler.Reconcile(record, actual, next, created || allocated, messages);
		foreach (var property in db.Entry(row).Properties.Where(x => OwnedFields[typeof(T)].Contains(x.Metadata.Name)))
			if (merged.TryGetValue(property.Metadata.Name, out var value)) property.CurrentValue = JsonSerializer.Deserialize(value, property.Metadata.ClrType);
		if (row is FutureProg parameters && merged.TryGetValue("$parameters", out var mergedParameters) && actual["$parameters"] != mergedParameters)
		{
			foreach (var parameter in parameters.FutureProgsParameters.ToArray()) { parameters.FutureProgsParameters.Remove(parameter); db.Remove(parameter); }
			using var json = JsonDocument.Parse(mergedParameters);
			foreach (var parameter in json.RootElement.EnumerateArray()) parameters.FutureProgsParameters.Add(new()
			{
				ParameterIndex = parameter.GetProperty("ParameterIndex").GetInt32(), ParameterName = parameter.GetProperty("ParameterName").GetString()!,
				ParameterTypeDefinition = parameter.GetProperty("ParameterTypeDefinition").GetString()!
			});
		}
		if (created || allocated || actual.Count != merged.Count || actual.Any(x => !merged.TryGetValue(x.Key, out var value) || value != x.Value)) record.AppliedAt = DateTime.UtcNow;
		record.ManifestVersion = Version;
		db.SaveChanges();
		return row;
	}
}
