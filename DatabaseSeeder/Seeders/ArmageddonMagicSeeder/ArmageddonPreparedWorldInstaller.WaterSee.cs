#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonPreparedWorldInstaller
{
	public const string WaterSeeReadiness = "Optional Water/See requires explicit water-liquid IDs, Silt/Shadow terrains, ordered Divination rank0-4 tags and an owned Draw Wine prerequisite path. Null preserves owned Water/See content without reconciling it.";

	private static ArmageddonPreparedWorldBindings FreezeWaterSeeBindings(ArmageddonPreparedWorldBindings bindings) =>
		bindings.WaterSee is { WaterLiquids: not null, DivinationRankTags: not null } selected
			? bindings with { WaterSee = selected with { WaterLiquids = selected.WaterLiquids.ToArray(), DivinationRankTags = selected.DivinationRankTags.ToArray() } }
			: bindings;

	private static void ValidateWaterSee(FuturemudDatabaseContext db, ArmageddonPreparedWorldBindings bindings, List<string> errors)
	{
		try { _ = PreservedWaterSeeSpells(db); }
		catch (Exception error) when (error is InvalidOperationException or System.Xml.XmlException or FormatException)
		{ errors.Add(error.Message); }
		if (bindings.WaterSee is null) return;
		errors.AddRange(ArmageddonWaterSeeInstaller.ValidateMappings(db, bindings.WaterSee));
		if (bindings.Provisions is not null) return; // Its own module must commit successfully before Water/See can run.
		try
		{
			if (!PreservedProvisionSpells(db).ContainsKey(MudSharp.Magic.ArmageddonReviewedProvisionContent.DrawWineKey))
				errors.Add("Water/See selection requires valid existing owned Draw Wine content or explicit Provisions bindings. No provisions or food/wine mappings are inferred.");
		}
		catch (Exception error) when (error is InvalidOperationException or System.Xml.XmlException or FormatException)
		{ errors.Add(error.Message); }
	}

	private static IReadOnlyDictionary<string, long> PreservedWaterSeeSpells(FuturemudDatabaseContext db)
	{
		var records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonWaterSeeInstaller.Package && x.Module == ArmageddonWaterSeeInstaller.Module).ToArray();
		if (records.Length == 0) return new Dictionary<string, long>();
		var keys = ArmageddonWaterSeeInstaller.SpellKeys;
		var expected = keys.SelectMany(x => new[] { x, x + ".duration", x + ".cost", x + ".eligibility" }).ToArray();
		if (!records.Select(x => x.StableKey).Order().SequenceEqual(expected.Order()) || records.Any(x => x.Retired || x.LogicalId is null ||
			x.RevisionNumber is not null || x.ManifestVersion != ArmageddonWaterSeeInstaller.Version))
			throw new InvalidOperationException("Existing owned Water/See is incomplete, retired or has unknown keys/version. Preserved without repair; resolve ownership explicitly before dependent admissions.");
		foreach (var record in records)
		{
			var type = keys.Contains(record.StableKey) ? nameof(MagicSpell) : record.StableKey.EndsWith(".eligibility", StringComparison.Ordinal) ? nameof(FutureProg) : nameof(TraitExpression);
			var exists = record.EntityType switch
			{
				nameof(MagicSpell) => db.MagicSpells.AsNoTracking().Any(x => x.Id == record.LogicalId),
				nameof(FutureProg) => db.FutureProgs.AsNoTracking().Any(x => x.Id == record.LogicalId),
				nameof(TraitExpression) => db.TraitExpressions.AsNoTracking().Any(x => x.Id == record.LogicalId),
				_ => false
			};
			if (record.EntityType != type || !exists || db.SeederManagedRecords.AsNoTracking().Any(x => x.Id != record.Id &&
				x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
				throw new InvalidOperationException($"Existing Water/See {record.StableKey} is missing, has invalid type or competing ownership; no resurrection/adoption.");
		}
		var result = new Dictionary<string, long>();
		foreach (var key in keys)
		{
			var record = records.Single(x => x.StableKey == key);
			var spell = db.MagicSpells.AsNoTracking().Single(x => x.Id == record.LogicalId);
			if ((string?)XElement.Parse(spell.Definition).Element("StockIdentity") != key)
				throw new InvalidOperationException($"Existing Water/See {key} has no matching reviewed stock identity; preserved, not adopted or admitted automatically.");
			result.Add(key, spell.Id);
		}
		return result;
	}
}
