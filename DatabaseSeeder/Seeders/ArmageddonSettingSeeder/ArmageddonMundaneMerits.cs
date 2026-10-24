#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Owned merit definitions only. Role grants are composed by the mundane role phase.</summary>
public static class ArmageddonMundaneMerits
{
	public const string Module = "setting-mundane-merits";
	public const string BardKey = "arm.merit.heard_language_branching";
	// Historical source identity; this grants clearance, never direct poison protection.
	public const string MercenaryKey = "arm.merit.alcohol_poison_resistance";

	public static IReadOnlyDictionary<string, long> Install(FuturemudDatabaseContext db, ICollection<string> messages)
	{
		var yes = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id;
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		string Definition(string description, params object[] content) => new XElement("Definition", content,
			new XElement("ChargenAvailableProg", no), new XElement("ApplicabilityProg", yes),
			new XElement("DescriptionText", description), new XElement("ChargenBlurb", description)).ToString();
		var desired = new Dictionary<string, Merit>
		{
			[BardKey] = new Merit
			{
				Name = "Bard Languages", Type = "Skill Learning", MeritType = 0, MeritScope = 2,
				Definition = Definition("@ learn|learns languages readily", new XAttribute("branching", "50"),
					new XAttribute("improving", "5"), new XAttribute("min_difficulty", "0"), new XAttribute("max_difficulty", "10"),
					new XElement("Groups", new XElement("Group", "Language")))
			},
			[MercenaryKey] = new Merit
			{
				Name = "Mercenary Liver Clearance", Type = "Liver Clearance", MeritType = 0, MeritScope = 2,
				Definition = Definition("@ clear|clears alcohol and active drugs more quickly, including beneficial drugs", new XAttribute("multiplier", "2"))
			}
		};
		// Complete definition/identity preflight before either merit is added.
		foreach (var item in desired)
		{
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == item.Key);
			if (record is null)
			{
				if (db.Merits.Any(x => x.Name == item.Value.Name)) throw new InvalidOperationException($"Unowned merit name collision: {item.Value.Name}.");
			}
			else if (record.Module != Module || record.EntityType != nameof(Merit) || record.Retired || record.LogicalId is not { } id ||
				!db.Merits.Any(x => x.Id == id) || db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == nameof(Merit) && x.LogicalId == id))
				throw new InvalidOperationException($"Invalid or competing merit ownership: {item.Key}.");
		}
		using var transaction = db.Database.IsRelational() && db.Database.CurrentTransaction is null ? db.Database.BeginTransaction() : null;
		var ids = new Dictionary<string, long>();
		foreach (var item in desired)
		{
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == item.Key);
			Merit actual;
			if (record is null)
			{
				actual = item.Value;
				db.Merits.Add(actual); db.SaveChanges();
				ArmageddonSettingOwnership.RecordNew(db, Module, item.Key, actual, "Name", "Type", "MeritType", "MeritScope", "Definition");
			}
			else
			{
				actual = db.Merits.Single(x => x.Id == record.LogicalId);
				var entry = db.Entry(item.Value);
				var fields = new[] { "Name", "Type", "MeritType", "MeritScope", "Definition" }.ToDictionary(x => x,
					x => JsonSerializer.Serialize(entry.Property(x).CurrentValue, entry.Property(x).Metadata.ClrType));
				ArmageddonSettingOwnership.Reconcile(db, record, actual, fields, messages);
			}
			ids[item.Key] = actual.Id;
		}
		db.SaveChanges(); transaction?.Commit();
		return ids;
	}
}
