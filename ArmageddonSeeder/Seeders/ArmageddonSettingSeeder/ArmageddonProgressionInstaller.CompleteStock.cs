#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonProgressionInstaller
{
	private const string CompleteStockMarker = "$completeStockPolicy";
	private const string CompleteStockVersion = "inactive-stock-policy-1";

	private static bool HasCompleteStockPolicy(SeederManagedRecord record)
	{
		if (record.SeedBaseline is null) return false;
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline)
			?? throw new InvalidOperationException($"{record.StableKey}: invalid progression baseline.");
		if (!baseline.TryGetValue(CompleteStockMarker, out var version)) return false;
		if (version != JsonSerializer.Serialize(CompleteStockVersion))
			throw new InvalidOperationException($"{record.StableKey}: incompatible complete-stock policy version.");
		return true;
	}

	private static IReadOnlyDictionary<string, long> CompleteStockSpells(FuturemudDatabaseContext db,
		IReadOnlyDictionary<string, SeederManagedRecord> records, List<string> errors)
	{
		var spells = db.MagicSpells.ToDictionary(x => x.Id);
		var result = new Dictionary<string, long>(StringComparer.Ordinal);
		foreach (var key in ArmageddonStockSpellInstaller.CanonicalKeys)
		{
			if (!records.TryGetValue(key, out var claim) || claim.Module != ArmageddonStockSpellInstaller.Module ||
				claim.EntityType != nameof(MagicSpell) || claim.Retired || claim.LogicalId is not > 0 || claim.RevisionNumber is not null ||
				!spells.TryGetValue(claim.LogicalId.Value, out var spell))
			{
				errors.Add($"{key}: complete inactive progression requires the real retained canonical spell; existing policies were not downgraded.");
				continue;
			}
			var root = XElement.Parse(spell.Definition);
			if (root.Element("StockIdentity")?.Value != key || root.Element("ControlledPower") is null || spell.CastingTraitDefinitionId is not > 0 ||
				db.SeederManagedRecords.Any(x => x.Id != claim.Id && x.EntityType == claim.EntityType && x.LogicalId == claim.LogicalId))
			{
				errors.Add($"{key}: incompatible or competing canonical spell identity; complete-stock policy held.");
				continue;
			}
			result.Add(key, spell.Id);
		}
		return result;
	}
}
