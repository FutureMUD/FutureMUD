#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

/// <summary>
/// Writes already resolved named identities. This class deliberately has no display-name lookup:
/// the caller must supply a preflighted source binding or request a genuinely new identity.
/// </summary>
internal sealed class CultureToolkitEntityWriter(FuturemudDatabaseContext context, string era, ICollection<string> conflicts)
{
	public static T CopyScalars<T>(FuturemudDatabaseContext context, T source) where T : class, new()
	{
		var result = new T();
		var entity = context.Model.FindEntityType(typeof(T))!;
		var primary = entity.FindPrimaryKey()!.Properties;
		foreach (var property in entity.GetProperties().Where(x => x.PropertyInfo is not null && !primary.Contains(x)))
			property.PropertyInfo!.SetValue(result, property.PropertyInfo.GetValue(source));
		return result;
	}

	public T Upsert<T>(string key, T desired, T? bound = null, T? verifiedSourceBaseline = null,
		IReadOnlySet<string>? independentlyManagedFields = null) where T : class, new()
	{
		var entityType = context.Model.FindEntityType(typeof(T))!;
		var primary = entityType.FindPrimaryKey()!.Properties;
		if (primary.Count != 1 || primary[0].ClrType != typeof(long))
			throw new InvalidOperationException($"{typeof(T).Name} requires collection-member reconciliation, not a named identity write.");
		var idProperty = primary[0].PropertyInfo!;
		var fields = entityType.GetProperties().Where(x => x.PropertyInfo is not null && !primary.Contains(x) &&
			independentlyManagedFields?.Contains(x.Name) != true)
			.Select(x => x.PropertyInfo!).ToArray();
		Dictionary<string, string> Values(T model) => fields.ToDictionary(x => x.Name, x => JsonSerializer.Serialize(x.GetValue(model), x.PropertyType));
		var type = typeof(T).Name;
		if (bound is not null && context.Entry(bound).State == EntityState.Detached)
			throw new InvalidOperationException($"Source binding for {type} {key} must refer to a tracked installed entity.");
		var record = CultureToolkitManagedEntities.Find(context, type, key);
		var actual = record is not null ? context.Set<T>().Find(record.LogicalId)
			?? throw new InvalidOperationException($"Managed {type} {key} is missing; preserving possible builder deletion.") : bound;
		if (bound is not null && actual is not null && !Equals(idProperty.GetValue(bound), idProperty.GetValue(actual)))
			throw new InvalidOperationException($"Conflicting explicit source binding for {type} {key}.");
		var fresh = actual is null;
		if (actual is null)
		{
			actual = new T();
			foreach (var property in entityType.GetProperties().Where(x => x.PropertyInfo is not null && !primary.Contains(x)))
				property.PropertyInfo!.SetValue(actual, property.PropertyInfo.GetValue(desired));
			context.Set<T>().Add(actual);
			context.SaveChanges();
		}
		var id = (long)idProperty.GetValue(actual)!;
		if (record is null && !fresh && verifiedSourceBaseline is not null)
		{
			context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = "CultureSeeder", Module = era, EntityType = type, StableKey = key, LogicalId = id,
				ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow, SeedBaseline = JsonSerializer.Serialize(Values(verifiedSourceBaseline))
			});
			context.SaveChanges();
		}
		var merged = CultureToolkitManagedEntities.Reconcile(context, era, type, key, id, fresh, Values(actual), Values(desired), conflicts);
		foreach (var field in fields)
			if (merged.TryGetValue(field.Name, out var value)) field.SetValue(actual, JsonSerializer.Deserialize(value, field.PropertyType));
		context.SaveChanges();
		return actual;
	}
}
