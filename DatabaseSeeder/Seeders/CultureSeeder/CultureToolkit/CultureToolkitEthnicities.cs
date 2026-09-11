#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureEthnicityDefinition(string Key, Ethnicity Source, Ethnicity Desired,
	IReadOnlyDictionary<short, long> SourceNameCultures, CultureNativeBinding NativeBinding);
public sealed record CultureEthnicityResolution(IReadOnlyDictionary<string, Ethnicity> Ethnicities,
	IReadOnlyDictionary<(long EthnicityId, short Gender), long> OriginalNameCultures,
	IReadOnlyList<CultureNativeBinding> NativeBindings);

/// <summary>Source phenotype/name memberships and supplied overlay fields reconcile independently.</summary>
public static class CultureToolkitEthnicities
{
	public static CultureEthnicityResolution Upsert(FuturemudDatabaseContext context, string era,
		IReadOnlyList<CultureEthnicityDefinition> definitions, IReadOnlyDictionary<string, Ethnicity> sourceBindings,
		ICollection<string> conflicts)
	{
		foreach (var definition in definitions)
		{
			if (CultureToolkitManagedEntities.Find(context, "Ethnicity", definition.Key) is null && !sourceBindings.ContainsKey(definition.Key) &&
				context.Ethnicities.Any(x => x.Name == definition.Desired.Name))
				throw new InvalidOperationException($"Unresolved ethnicity source identity {definition.Key}: {definition.Desired.Name}.");
			foreach (var id in definition.SourceNameCultures.Values)
				if (context.NameCultures.Find(id) is null) throw new InvalidOperationException($"{definition.Key}: missing resolved naming structure {id}.");
			foreach (var characteristic in definition.Source.EthnicitiesCharacteristics)
				if (context.CharacteristicProfiles.Find(characteristic.CharacteristicProfileId) is null)
					throw new InvalidOperationException($"{definition.Key}: missing source characteristic profile {characteristic.CharacteristicProfileId}.");
		}
		var writer = new CultureToolkitEntityWriter(context, era, conflicts);
		var result = new Dictionary<string, Ethnicity>();
		var originalNames = new Dictionary<(long, short), long>();
		foreach (var definition in definitions)
		{
			var key = definition.Key;
			var wasInstalled = CultureToolkitManagedEntities.Find(context, "Ethnicity", key) is not null || sourceBindings.ContainsKey(key);
			var ethnicity = writer.Upsert(key, definition.Desired, sourceBindings.GetValueOrDefault(key), definition.Source,
				definition.NativeBinding.IsResolved || definition.NativeBinding.Rule == "reviewed-missing-language"
					? null : new HashSet<string> { nameof(Ethnicity.NativeLanguageId) });
			result[key] = ethnicity;
			context.Entry(ethnicity).Collection(x => x.EthnicitiesNameCultures).Load();
			context.Entry(ethnicity).Collection(x => x.EthnicitiesCharacteristics).Load();
			var desiredMembers = definition.Source.EthnicitiesCharacteristics.ToDictionary(x => $"characteristic:{x.CharacteristicDefinitionId}", x => x.CharacteristicProfileId.ToString(CultureInfo.InvariantCulture));
			foreach (var name in definition.SourceNameCultures)
			{
				desiredMembers[$"name:{name.Key}"] = name.Value.ToString(CultureInfo.InvariantCulture);
				originalNames[(ethnicity.Id, name.Key)] = name.Value;
			}
			var actualMembers = ethnicity.EthnicitiesCharacteristics.ToDictionary(x => $"characteristic:{x.CharacteristicDefinitionId}", x => x.CharacteristicProfileId.ToString(CultureInfo.InvariantCulture));
			foreach (var name in ethnicity.EthnicitiesNameCultures) actualMembers[$"name:{name.Gender}"] = name.NameCultureId.ToString(CultureInfo.InvariantCulture);
			var record = CultureToolkitManagedEntities.Find(context, "EthnicitySourceMembers", key);
			var delegatedNames = context.SeederManagedRecords.Where(x => x.Seeder == "CultureSeeder" && x.EntityType == "EthnicityNameCulture" &&
				x.LogicalId == ethnicity.Id && x.StableKey.StartsWith("names.target.")).AsEnumerable()
				.Select(x => "name:" + x.StableKey.Split('.').Last()).ToHashSet();
			foreach (var name in delegatedNames) { desiredMembers.Remove(name); actualMembers.Remove(name); }
			if (record?.SeedBaseline is not null && delegatedNames.Count > 0)
			{
				var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline)!;
				foreach (var name in delegatedNames) baseline.Remove(name);
				record.SeedBaseline = JsonSerializer.Serialize(baseline);
			}
			if (record is null && wasInstalled)
			{
				// The source binding supplies the prior source fields, not the current builder-edited values.
				context.SeederManagedRecords.Add(new SeederManagedRecord
				{
					Seeder = "CultureSeeder", EntityType = "EthnicitySourceMembers", StableKey = key, Module = era, LogicalId = ethnicity.Id,
					ManifestVersion = "2026-09-07", AppliedAt = DateTime.UtcNow, SeedBaseline = JsonSerializer.Serialize(desiredMembers)
				});
				context.SaveChanges();
			}
			var merged = CultureToolkitManagedEntities.Reconcile(context, era, "EthnicitySourceMembers", key, ethnicity.Id,
				!wasInstalled, actualMembers, desiredMembers, conflicts);
			foreach (var item in merged)
			{
				var id = long.Parse(item.Value, CultureInfo.InvariantCulture);
				if (item.Key.StartsWith("characteristic:", StringComparison.Ordinal))
				{
					var characteristicId = long.Parse(item.Key[15..], CultureInfo.InvariantCulture);
					var link = ethnicity.EthnicitiesCharacteristics.SingleOrDefault(x => x.CharacteristicDefinitionId == characteristicId);
					if (link?.CharacteristicProfileId == id) continue;
					if (link is not null) { ethnicity.EthnicitiesCharacteristics.Remove(link); context.EthnicitiesCharacteristics.Remove(link); }
					ethnicity.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics { EthnicityId = ethnicity.Id, CharacteristicDefinitionId = characteristicId, CharacteristicProfileId = id });
					continue;
				}
				var gender = short.Parse(item.Key[5..], CultureInfo.InvariantCulture);
				var nameLink = ethnicity.EthnicitiesNameCultures.SingleOrDefault(x => x.Gender == gender);
				if (nameLink?.NameCultureId == id) continue;
				if (nameLink is not null) { ethnicity.EthnicitiesNameCultures.Remove(nameLink); context.EthnicitiesNameCultures.Remove(nameLink); }
				ethnicity.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures { EthnicityId = ethnicity.Id, Gender = gender, NameCultureId = id });
			}
			context.SaveChanges();
		}
		return new(result, originalNames, definitions.Select(x => x.NativeBinding).ToArray());
	}
}
