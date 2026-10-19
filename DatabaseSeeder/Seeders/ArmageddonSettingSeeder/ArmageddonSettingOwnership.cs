#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Field baselines shared by early and late Setting phases, independent of public installer names.</summary>
internal static class ArmageddonSettingOwnership
{
	internal static SeederManagedRecord RecordNew(FuturemudDatabaseContext db, string module, string key,
		object entity, params string[] fields)
	{
		var entry = db.Entry(entity);
		var id = (long)entry.Property("Id").CurrentValue!;
		if (id <= 0 || db.SeederManagedRecords.Any(x => x.EntityType == entry.Metadata.ClrType.Name && x.LogicalId == id))
			throw new InvalidOperationException($"{key}: permanent unclaimed identity required before ownership is recorded.");
		var record = new SeederManagedRecord
		{
			Seeder = ArmageddonMagicInstaller.Package, Module = module, StableKey = key,
			EntityType = entry.Metadata.ClrType.Name, LogicalId = id,
			ManifestVersion = ArmageddonSkillPackage.Version, AppliedAt = DateTime.UtcNow
		};
		var values = fields.ToDictionary(x => x, x => JsonSerializer.Serialize(entry.Property(x).CurrentValue, entry.Property(x).Metadata.ClrType));
		SeederManagedRecordReconciler.Reconcile(record, values, values, true, new List<string>());
		db.SeederManagedRecords.Add(record);
		return record;
	}

	internal static void Reconcile(FuturemudDatabaseContext db, SeederManagedRecord record, object entity,
		IReadOnlyDictionary<string, string> desired, ICollection<string> messages)
	{
		var entry = db.Entry(entity);
		if (record.Seeder != ArmageddonMagicInstaller.Package || record.EntityType != entry.Metadata.ClrType.Name ||
			record.LogicalId != (long)entry.Property("Id").CurrentValue! || record.Retired || record.SeedBaseline is null)
			throw new InvalidOperationException($"{record.StableKey}: invalid owned field baseline.");
		var previous = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline)
			?? throw new InvalidOperationException($"{record.StableKey}: missing field baseline.");
		var current = previous.Keys.Union(desired.Keys).ToDictionary(x => x,
			x => JsonSerializer.Serialize(entry.Property(x).CurrentValue, entry.Property(x).Metadata.ClrType));
		var merged = SeederManagedRecordReconciler.Reconcile(record, current, desired, false, messages);
		foreach (var field in merged)
			entry.Property(field.Key).CurrentValue = JsonSerializer.Deserialize(field.Value, entry.Property(field.Key).Metadata.ClrType);
		record.ManifestVersion = ArmageddonSkillPackage.Version;
		record.AppliedAt = DateTime.UtcNow;
	}
}
