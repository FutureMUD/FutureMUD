#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Field baselines shared by early and late Setting phases, independent of public installer names.</summary>
internal static class ArmageddonSettingOwnership
{
	internal static T Apply<T>(FuturemudDatabaseContext db, string module, string key, T desired,
		ICollection<string> messages) where T : class
	{
		var metadata = db.Model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException($"Unmapped owned type {typeof(T).Name}.");
		var fields = metadata.GetProperties().Where(x => !x.IsPrimaryKey()).Select(x => x.Name).ToArray();
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (record is null)
		{
			var nameField = metadata.FindProperty("Name") ?? metadata.FindProperty("FunctionName");
			if (nameField is not null)
			{
				var name = (string?)db.Entry(desired).Property(nameField.Name).CurrentValue;
				if (db.Set<T>().AsEnumerable().Any(x => string.Equals((string?)db.Entry(x).Property(nameField.Name).CurrentValue, name, StringComparison.OrdinalIgnoreCase) &&
					(desired is not Accent accent || x is Accent other && other.LanguageId == accent.LanguageId)))
					throw new InvalidOperationException($"Unowned {typeof(T).Name} name collision: {name}.");
			}
			db.Add(desired); db.SaveChanges();
			RecordNew(db, module, key, desired, fields);
			return desired;
		}
		if (record.Module != module || record.EntityType != typeof(T).Name || record.Retired || record.LogicalId is not { } id ||
			db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == id))
			throw new InvalidOperationException($"Invalid or competing owned identity: {key}.");
		var actual = db.Find<T>(id) ?? throw new InvalidOperationException($"Missing owned entity: {key}.");
		var values = fields.ToDictionary(x => x, x => JsonSerializer.Serialize(db.Entry(desired).Property(x).CurrentValue, db.Entry(desired).Property(x).Metadata.ClrType));
		Reconcile(db, record, actual, values, messages);
		db.SaveChanges();
		return actual;
	}

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
		// This newly added nullable column did not exist in older heritage baselines. An unset
		// value is the migration default; a non-null builder choice must still be preserved.
		if (entity is Race && record.Module == ArmageddonHeritage.Module &&
			!previous.ContainsKey(nameof(Race.AgeDescriptionProgId)) && desired.ContainsKey(nameof(Race.AgeDescriptionProgId)) &&
			entry.Property(nameof(Race.AgeDescriptionProgId)).CurrentValue is null)
			current.Remove(nameof(Race.AgeDescriptionProgId));
		var merged = SeederManagedRecordReconciler.Reconcile(record, current, desired, false, messages);
		foreach (var field in merged)
			entry.Property(field.Key).CurrentValue = JsonSerializer.Deserialize(field.Value, entry.Property(field.Key).Metadata.ClrType);
		record.ManifestVersion = ArmageddonSkillPackage.Version;
		record.AppliedAt = DateTime.UtcNow;
	}
}
