#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

internal static class CultureToolkitManagedEntities
{
	private static readonly ConditionalWeakTable<FuturemudDatabaseContext, LookupScope> Lookups = new();

	public static IDisposable BeginLookupScope(FuturemudDatabaseContext context) => new LookupScope(context);

	public static SeederManagedRecord? Find(FuturemudDatabaseContext context, string type, string key) =>
		Lookups.TryGetValue(context, out var lookup) ? lookup.Records.GetValueOrDefault((type, key)) :
			context.SeederManagedRecords.SingleOrDefault(x => x.Seeder == "CultureSeeder" && x.EntityType == type && x.StableKey == key);

	// Ownership keys are immutable during an import. Track additions/removals so newly
	// reconciled identities are immediately visible without one SQL query per field group.
	private sealed class LookupScope : IDisposable
	{
		private readonly FuturemudDatabaseContext _context;
		public Dictionary<(string Type, string Key), SeederManagedRecord> Records { get; }

		public LookupScope(FuturemudDatabaseContext context)
		{
			_context = context;
			Records = context.SeederManagedRecords.Where(x => x.Seeder == "CultureSeeder")
				.ToDictionary(x => (x.EntityType, x.StableKey));
			foreach (var record in context.SeederManagedRecords.Local.Where(x => x.Seeder == "CultureSeeder"))
				Records[(record.EntityType, record.StableKey)] = record;
			Lookups.Add(context, this);
			context.ChangeTracker.Tracked += Tracked;
			context.ChangeTracker.StateChanged += StateChanged;
		}

		private void Tracked(object? sender, EntityTrackedEventArgs args) => Update(args.Entry);
		private void StateChanged(object? sender, EntityStateChangedEventArgs args) => Update(args.Entry);
		private void Update(EntityEntry entry)
		{
			if (entry.Entity is not SeederManagedRecord { Seeder: "CultureSeeder" } record) return;
			var key = (record.EntityType, record.StableKey);
			if (entry.State is EntityState.Deleted or EntityState.Detached) Records.Remove(key);
			else Records[key] = record;
		}

		public void Dispose()
		{
			_context.ChangeTracker.Tracked -= Tracked;
			_context.ChangeTracker.StateChanged -= StateChanged;
			Lookups.Remove(_context);
		}
	}

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
