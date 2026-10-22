#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

// Test-only, persisted scalar snapshots. Never retain tracked entities or navigation graphs.
// This is intentionally restricted to the plain InMemory fixture, not relational/default tests.
internal sealed class InMemorySeederSnapshot
{
	private sealed record Table(string Name, Type Type, bool Shared, IProperty[] Properties, object?[][] Rows);
	private readonly Table[] _tables;
	private static readonly MethodInfo ReadMethod = typeof(InMemorySeederSnapshot)
		.GetMethod(nameof(Read), BindingFlags.NonPublic | BindingFlags.Static)!;

	private InMemorySeederSnapshot(Table[] tables) => _tables = tables;

	internal static InMemorySeederSnapshot Capture(FuturemudDatabaseContext db)
	{
		if (!db.Database.IsInMemory()) throw new InvalidOperationException("Snapshots require the InMemory test provider.");
		if (db.ChangeTracker.HasChanges()) throw new InvalidOperationException("Persist fixture changes before taking a snapshot.");
		db.ChangeTracker.Clear();
		var tables = new List<Table>();
		var detectChanges = db.ChangeTracker.AutoDetectChangesEnabled;
		db.ChangeTracker.AutoDetectChangesEnabled = false;
		try
		{
			foreach (var entityType in db.Model.GetEntityTypes().OrderBy(x => x.Name, StringComparer.Ordinal))
			{
				// Keyless mappings are read-only views; EF cannot persist fixture rows in them.
				if (entityType.FindPrimaryKey() is null) continue;
				if (entityType.IsOwned() || entityType.BaseType is not null ||
					entityType.HasSharedClrType && entityType.ClrType != typeof(Dictionary<string, object>))
					throw new InvalidOperationException("Unsupported snapshot mapping: " + entityType.Name);
				var properties = entityType.GetProperties().OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();
				var entities = (IEnumerable<object>)ReadMethod.MakeGenericMethod(entityType.ClrType)
					.Invoke(null, [db, entityType.HasSharedClrType ? entityType.Name : null])!;
				// Tracking is essential: detached CLR objects do not retain shadow property values.
				_ = entities.ToArray();
				var rows = db.ChangeTracker.Entries().Where(entry => entry.Metadata.Name == entityType.Name).Select(entry =>
				{
					var values = entry.CurrentValues;
					return properties.Select(property => Copy(property, values[property.Name])).ToArray();
				}).ToArray();
				if (rows.Length > 0) tables.Add(new(entityType.Name, entityType.ClrType, entityType.HasSharedClrType, properties, rows));
				db.ChangeTracker.Clear();
			}
		}
		finally
		{
			db.ChangeTracker.Clear();
			db.ChangeTracker.AutoDetectChangesEnabled = detectChanges;
		}
		return new(tables.ToArray());
	}

	internal FuturemudDatabaseContext Create()
	{
		var db = new OwnedInMemorySeederContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		try
		{
			db.ChangeTracker.AutoDetectChangesEnabled = false;
			foreach (var table in _tables)
			foreach (var row in table.Rows)
			{
				if (table.Shared)
				{
					var values = table.Properties.Select((property, i) => (property.Name, Value: Copy(property, row[i])))
						.ToDictionary(x => x.Name, x => x.Value!);
					db.Set<Dictionary<string, object>>(table.Name).Add(values);
					continue;
				}
				var entry = db.Entry(Activator.CreateInstance(table.Type)!);
				for (var i = 0; i < table.Properties.Length; i++)
					entry.Property(table.Properties[i].Name).CurrentValue = Copy(table.Properties[i], row[i]);
				entry.State = EntityState.Added;
			}
			db.SaveChanges();
			db.ChangeTracker.Clear();
			db.ChangeTracker.AutoDetectChangesEnabled = true;
			return db;
		}
		catch
		{
			db.Dispose();
			throw;
		}
	}

	// Include all persisted properties (including FKs and shadow properties), independent of row order.
	internal string Fingerprint => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
		string.Join("\n", _tables.Select(table => table.Name + "\n" +
			string.Join(",", table.Properties.Select(x => x.Name)) + "\n" +
			string.Join("\n", table.Rows.Select(row => JsonSerializer.Serialize(row)).OrderBy(x => x, StringComparer.Ordinal)))))));

	private static object? Copy(IProperty property, object? value) =>
		(property.GetValueComparer() ?? property.GetTypeMapping().Comparer).Snapshot(value);

	private static IEnumerable<object> Read<TEntity>(FuturemudDatabaseContext db, string? name) where TEntity : class =>
		(name is null ? db.Set<TEntity>() : db.Set<TEntity>(name)).AsTracking().IgnoreQueryFilters().AsEnumerable();
}
