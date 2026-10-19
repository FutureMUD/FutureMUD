#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Owns explicit collection/reference fields, without claiming the shared parent entity.</summary>
internal static class ArmageddonSettingLinks
{
	internal const string Module = "setting-links";
	internal static bool Valid(FuturemudDatabaseContext db, SeederManagedRecord record)
	{
		if (record.Module != Module || record.Retired || record.LogicalId is not { } id || record.SeedBaseline is null ||
			record.AppliedFingerprint != Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(record.SeedBaseline))).ToLowerInvariant() ||
			db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == id)) return false;
		return record.EntityType switch
		{
			"Culture.SettingLinks" => db.Cultures.Any(x => x.Id == id),
			"Ethnicity.SettingLinks" => db.Ethnicities.Any(x => x.Id == id),
			"FutureProg.SettingLinks" => db.FutureProgs.Any(x => x.Id == id),
			"ChargenScreenStoryboard.SettingLinks" => db.ChargenScreenStoryboards.Any(x => x.Id == id),
			"Check.SettingLinks" => db.Checks.Any(x => x.Type == id),
			_ => false
		};
	}

	internal static IReadOnlyDictionary<string, string> Reconcile(FuturemudDatabaseContext db, string key, string parentType,
		long parentId, IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> desired, ICollection<string> messages)
	{
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		var created = record is null;
		if (created)
		{
			record = new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = Module, StableKey = key,
				EntityType = parentType + ".SettingLinks", LogicalId = parentId, ManifestVersion = ArmageddonSkillPackage.Version, AppliedAt = DateTime.UtcNow };
			if (db.SeederManagedRecords.Any(x => x.EntityType == record.EntityType && x.LogicalId == parentId))
				throw new InvalidOperationException($"Competing reference ownership: {key}.");
			db.SeederManagedRecords.Add(record);
		}
		else if (!Valid(db, record!) || record!.EntityType != parentType + ".SettingLinks" || record.LogicalId != parentId)
			throw new InvalidOperationException($"Invalid typed reference ownership: {key}.");
		return SeederManagedRecordReconciler.Reconcile(record!, current, desired, created, messages);
	}
}
