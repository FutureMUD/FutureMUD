#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Typed composite-key register definitions/defaults, never character/room value rows.</summary>
internal static class ArmageddonGatheringRegisters
{
	internal const string Module = "stock-gathering-registers";
	internal const string Version = "stock-gathering-registers-1";
	private const string EntityType = "ArmageddonGatheringRegister";
	internal const string Ecology = "armageddon_ecology";
	internal const string AlignmentUntil = "armageddon_gather_until";
	internal const string AshUntil = "armageddon_ash_until";
	private sealed record Spec(string Key, ProgVariableTypes Owner, string Property, ProgVariableTypes Type, string Default);
	private static readonly Spec[] Specs =
	[
		new("arm.register.gather.ecology", ProgVariableTypes.Character, Ecology, ProgVariableTypes.Number, "<var>0</var>"),
		new("arm.register.gather.alignment_until", ProgVariableTypes.Character, AlignmentUntil, ProgVariableTypes.DateTime, "<var>0001-01-01T00:00:00.0000000Z</var>"),
		new("arm.register.gather.ash_until", ProgVariableTypes.Location, AshUntil, ProgVariableTypes.DateTime, "<var>0001-01-01T00:00:00.0000000Z</var>")
	];

	internal static IReadOnlyList<VariableDefinition> Definitions => Specs.Select(spec => new VariableDefinition
	{
		OwnerTypeDefinition = spec.Owner.ToStorageString(), Property = spec.Property, ContainedTypeDefinition = spec.Type.ToStorageString()
	}).ToArray();

	internal static void Preflight(FuturemudDatabaseContext db)
	{
		foreach (var spec in Specs)
		{
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == spec.Key);
			if (record is not null)
			{
				if (!Valid(db, record)) throw new InvalidOperationException($"{spec.Key}: incompatible owned gathering register/default; preserve and resolve explicitly.");
				continue;
			}
			var owner = spec.Owner.ToStorageString();
			if (db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == spec.Property) ||
				db.VariableDefaults.Any(x => x.OwnerTypeDefinition == owner && x.Property == spec.Property))
				throw new InvalidOperationException($"{spec.Key}: unowned composite register collision; no adoption or default replacement.");
		}
	}

	internal static bool Valid(FuturemudDatabaseContext db, SeederManagedRecord record)
	{
		try
		{
			var spec = Specs.SingleOrDefault(x => x.Key == record.StableKey);
			if (spec is null || record.Seeder != ArmageddonMagicInstaller.Package || record.Module != Module ||
				record.ManifestVersion != Version || record.EntityType != EntityType || record.Retired ||
				record.LogicalId is not null || record.RevisionNumber is not null || record.SeedBaseline is null) return false;
			var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline)!;
			var desired = Fields(spec, spec.Default);
			if (baseline is null) return false;
			if (new[] { "OwnerType", "Property", "ContainedType" }.Any(key => !baseline.TryGetValue(key, out var value) || value != desired[key])) return false;
			var owner = spec.Owner.ToStorageString();
			var definition = db.VariableDefinitions.SingleOrDefault(x => x.OwnerTypeDefinition == owner && x.Property == spec.Property);
			var defaults = db.VariableDefaults.SingleOrDefault(x => x.OwnerTypeDefinition == owner && x.Property == spec.Property);
			if (definition?.ContainedTypeDefinition != spec.Type.ToStorageString() || defaults is null) return false;
			var valueText = XElement.Parse(defaults.DefaultValue).Value;
			return spec.Type == ProgVariableTypes.Number
				? decimal.TryParse(valueText, NumberStyles.Number, CultureInfo.InvariantCulture, out var numericDefault) && numericDefault is >= -100 and <= 100
				: DateTime.TryParse(valueText, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _);
		}
		catch (Exception error) when (error is JsonException or System.Xml.XmlException or InvalidOperationException or KeyNotFoundException or ArgumentException)
		{
			return false;
		}
	}

	internal static void Install(FuturemudDatabaseContext db, List<string> messages)
	{
		// All definitions/defaults were validated before any module writes. Existing instance values are never queried.
		foreach (var spec in Specs)
		{
			var owner = spec.Owner.ToStorageString();
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == spec.Key);
			var created = record is null;
			var definition = db.VariableDefinitions.SingleOrDefault(x => x.OwnerTypeDefinition == owner && x.Property == spec.Property);
			var defaults = db.VariableDefaults.SingleOrDefault(x => x.OwnerTypeDefinition == owner && x.Property == spec.Property);
			if (created)
			{
				definition = new VariableDefinition { OwnerTypeDefinition = owner, Property = spec.Property, ContainedTypeDefinition = spec.Type.ToStorageString() };
				defaults = new VariableDefault { OwnerTypeDefinition = owner, Property = spec.Property, DefaultValue = spec.Default };
				db.VariableDefinitions.Add(definition); db.VariableDefaults.Add(defaults);
				record = new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, StableKey = spec.Key,
					Module = Module, EntityType = EntityType, ManifestVersion = Version };
				db.SeederManagedRecords.Add(record);
			}
			var actual = Fields(spec, defaults!.DefaultValue);
			var merged = SeederManagedRecordReconciler.Reconcile(record!, actual, Fields(spec, spec.Default), created, messages);
			defaults.DefaultValue = JsonSerializer.Deserialize<string>(merged["DefaultValue"])!;
			if (created || actual["DefaultValue"] != merged["DefaultValue"]) record!.AppliedAt = DateTime.UtcNow;
			db.SaveChanges();
		}
	}

	private static Dictionary<string, string> Fields(Spec spec, string value) => new(StringComparer.Ordinal)
	{
		["OwnerType"] = JsonSerializer.Serialize(spec.Owner.ToStorageString()), ["Property"] = JsonSerializer.Serialize(spec.Property),
		["ContainedType"] = JsonSerializer.Serialize(spec.Type.ToStorageString()), ["DefaultValue"] = JsonSerializer.Serialize(value)
	};
}
