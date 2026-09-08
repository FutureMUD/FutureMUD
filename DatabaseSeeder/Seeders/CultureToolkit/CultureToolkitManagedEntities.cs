#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

internal static class CultureToolkitManagedEntities
{
	public static SeederManagedRecord? Find(FuturemudDatabaseContext context, string type, string key) =>
		context.SeederManagedRecords.SingleOrDefault(x => x.Seeder == "CultureSeeder" && x.EntityType == type && x.StableKey == key);

	public static IReadOnlyDictionary<string, string> Reconcile(FuturemudDatabaseContext context,
		string era, string type, string key, long id, bool newlyCreated,
		IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> desired, ICollection<string> conflicts)
	{
		var record = Find(context, type, key);
		if (record is null)
		{
			record = new SeederManagedRecord
			{
				Seeder = "CultureSeeder", Module = era, EntityType = type, StableKey = key, LogicalId = id,
				ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow
			};
			context.SeederManagedRecords.Add(record);
		}
		if (record.LogicalId != id) throw new InvalidOperationException($"Identity changed for {type}:{key}.");
		return SeederManagedRecordReconciler.Reconcile(record, current, desired, newlyCreated, conflicts);
	}
}
