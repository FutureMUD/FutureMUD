#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private sealed record PendingGeneratedManifestIdentity(object Entity, SeederManagedRecord Record,
		string EntityType, string StableKey, string AppliedFingerprint);

	private readonly Dictionary<string, PendingGeneratedManifestIdentity> _pendingGeneratedManifestIdentities =
		new(StringComparer.OrdinalIgnoreCase);

	private void CompleteGeneratedManifestAggregate(ItemSeederManifestEntry entry, object entity, object liveDefinition)
	{
		if (_manifestCaptureOnly) return;
		var identity = ManagedRecordIdentity(entry.EntityType, entry.StableKey);
		if (_pendingGeneratedManifestIdentities.TryGetValue(identity, out var pending) && !ReferenceEquals(pending.Entity, entity))
			throw new InvalidOperationException($"Two generated targets claim ItemSeeder aggregate {entry.EntityType}:{entry.StableKey}.");
		CompleteManifestAggregate(entry, null, liveDefinition, ManifestAggregateDisposition.Insert);
		var record = _managedRecordsByIdentity[identity];
		_pendingGeneratedManifestIdentities[identity] = new(entity, record, entry.EntityType, entry.StableKey, record.AppliedFingerprint);
	}

	/// <summary>Flush actual entities, bind their database IDs, then persist provenance in the same seeder transaction.</summary>
	private void SaveManifestChanges()
	{
		if (_manifestCaptureOnly) return;
		SaveTrackedChanges();
		if (_pendingGeneratedManifestIdentities.Count == 0) return;
		if (ReconcileGeneratedManifestIdentities() > 0) SaveTrackedChanges();
		_pendingGeneratedManifestIdentities.Clear();

		void SaveTrackedChanges()
		{
			if (!_context!.ChangeTracker.AutoDetectChangesEnabled) _context.ChangeTracker.DetectChanges();
			_context.SaveChanges();
		}
	}

	private int ReconcileGeneratedManifestIdentities()
	{
		// Validate the complete pending batch before assigning even its first provenance ID.
		var resolved = new List<(PendingGeneratedManifestIdentity Pending, long Id)>();
		var claimed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var establishedClaims = _managedRecordsByIdentity.Values.Where(x => x.LogicalId.HasValue)
			.ToLookup(x => $"{x.EntityType}\u001f{x.LogicalId}", StringComparer.OrdinalIgnoreCase);
		foreach (var (identity, pending) in _pendingGeneratedManifestIdentities.OrderBy(x => x.Key, StringComparer.Ordinal))
		{
			var targetEntry = _context!.Entry(pending.Entity);
			var idProperty = targetEntry.Property("Id");
			if (targetEntry.State is EntityState.Added or EntityState.Deleted or EntityState.Detached ||
				idProperty.IsTemporary || idProperty.CurrentValue is not long id || id <= 0)
				throw new InvalidOperationException($"Generated ID for {pending.EntityType}:{pending.StableKey} is not a persisted, positive, non-temporary identity.");
			var record = pending.Record;
			if (!_managedRecordsByIdentity.TryGetValue(identity, out var indexed) || !ReferenceEquals(indexed, record) ||
				_context.Entry(record).State is EntityState.Deleted or EntityState.Detached ||
				record.Seeder != Name || record.EntityType != pending.EntityType || record.StableKey != pending.StableKey ||
				record.Retired || record.AppliedFingerprint != pending.AppliedFingerprint ||
				record.LogicalId is { } existingId && existingId != id)
				throw new InvalidOperationException($"Generated provenance for {pending.EntityType}:{pending.StableKey} changed before ID reconciliation.");
			var idKey = $"{pending.EntityType}\u001f{id}";
			if (!claimed.Add(idKey) || establishedClaims[idKey].Any(other =>
				!other.StableKey.Equals(pending.StableKey, StringComparison.OrdinalIgnoreCase)))
				throw new InvalidOperationException($"Generated ID {id} for {pending.EntityType}:{pending.StableKey} is claimed by another aggregate.");
			resolved.Add((pending, id));
		}
		var changes = 0;
		foreach (var (pending, id) in resolved.Where(x => x.Pending.Record.LogicalId != x.Id))
		{
			pending.Record.LogicalId = id;
			changes++;
		}
		return changes;
	}
}
