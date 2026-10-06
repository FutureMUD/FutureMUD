extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using OfflineWorld = EngineCompiler::MudSharp.Framework.Futuremud;
using CapabilityFactory = EngineCompiler::MudSharp.Magic.Capabilities.MagicCapabilityFactory;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonPreparedWorldInstaller
{
	private static IReadOnlyDictionary<string, long> PreservedProvisionSpells(FuturemudDatabaseContext db)
	{
		var records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == ArmageddonProvisionInstaller.Module).ToArray();
		if (records.Length == 0) return new Dictionary<string, long>();
		var keys = new[] { ArmageddonReviewedProvisionContent.SustainMealKey, ArmageddonReviewedProvisionContent.DrawWineKey };
		var expected = keys.SelectMany(x => new[] { x, x + ".duration", x + ".cost" }).Append(ArmageddonReviewedProvisionContent.DrawWineKey + ".eligibility").ToArray();
		if (!records.Select(x => x.StableKey).Order().SequenceEqual(expected.Order()) || records.Any(x => x.Retired || x.LogicalId is null || x.RevisionNumber is not null || x.ManifestVersion != ArmageddonProvisionInstaller.Version))
			throw new InvalidOperationException("Existing owned provisions are incomplete/retired or contain unknown keys. Preserved without repair; resolve their ownership explicitly before reconciling dependent admissions.");
		foreach (var record in records)
		{
			var expectedType = keys.Contains(record.StableKey) ? "MagicSpell" : record.StableKey.EndsWith(".eligibility", StringComparison.Ordinal) ? "FutureProg" : "TraitExpression";
			if (record.EntityType != expectedType) throw new InvalidOperationException($"Existing provision {record.StableKey} has an invalid identity type; preserved without adoption.");
			var exists = record.EntityType switch
			{
				"MagicSpell" => db.MagicSpells.AsNoTracking().Any(x => x.Id == record.LogicalId),
				"TraitExpression" => db.TraitExpressions.AsNoTracking().Any(x => x.Id == record.LogicalId),
				"FutureProg" => db.FutureProgs.AsNoTracking().Any(x => x.Id == record.LogicalId),
				_ => false
			};
			if (!exists || db.SeederManagedRecords.AsNoTracking().Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
				throw new InvalidOperationException($"Existing provision {record.StableKey} is missing or has competing ownership; no resurrection/adoption.");
		}
		var result = new Dictionary<string, long>();
		foreach (var key in keys)
		{
			var record = records.Single(x => x.StableKey == key);
			if (record.EntityType != "MagicSpell") throw new InvalidOperationException($"Existing provision {key} has an invalid identity type.");
			var spell = db.MagicSpells.AsNoTracking().Single(x => x.Id == record.LogicalId);
			if ((string?)XElement.Parse(spell.Definition).Element("StockIdentity") != key)
				throw new InvalidOperationException($"Existing provision {key} no longer has its reviewed stock identity; preserved, not admitted automatically.");
			result.Add(key, spell.Id);
		}
		return result;
	}

	public static IReadOnlyList<ArmageddonPersistedAvailability> ReadAvailability(FuturemudDatabaseContext db, IReadOnlyDictionary<string, long> identities)
	{
		var sourceKeys = ArmageddonTraditionInstaller.SourceRows.Where(x => x.Kind == "spell").Select(x => x.Key).ToArray();
		var spellKeys = db.MagicSpells.AsNoTracking().ToArray().ToDictionary(x => x.Id, x =>
		{
			try { return (string?)XElement.Parse(x.Definition).Element("StockIdentity"); }
			catch { return null; }
		});
		var result = new List<ArmageddonPersistedAvailability>();
		using var world = new OfflineWorld(null!);
		foreach (var variant in ArmageddonTraditionInstaller.Variants)
		{
			var id = identities["arm.capability." + variant];
			var row = db.MagicCapabilities.AsNoTracking().Single(x => x.Id == id);
			var capability = (IMagicCastingCapability)CapabilityFactory.LoadCapability(row, world);
			var policy = capability.CastingPolicy ?? throw new InvalidOperationException($"Persisted {variant} casting XML is unreadable or absent; preserved, no availability claim.");
			if (policy.Admissions.Select(x => x.SpellId).Distinct().Count() != policy.Admissions.Count || policy.Admissions.Any(x =>
				!spellKeys.TryGetValue(x.SpellId, out var key) || key is null || !sourceKeys.Contains(key)))
				throw new InvalidOperationException($"Persisted {variant} contains duplicate or non-source admissions; inspect the retained builder policy. No default-policy availability claim.");
			var stored = policy.Admissions.Select(x => spellKeys[x.SpellId]!).Order(StringComparer.Ordinal).ToArray();
			result.Add(new(variant, id, policy.Enabled, stored, sourceKeys.Except(stored).ToArray()));
		}
		return result.AsReadOnly();
	}
}
