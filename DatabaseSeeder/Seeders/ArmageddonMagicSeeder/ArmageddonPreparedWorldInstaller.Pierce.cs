#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonPreparedWorldInstaller
{
	private static long? PreservedPierceSpell(FuturemudDatabaseContext db)
	{
		var records = db.SeederManagedRecords.AsNoTracking()
			.Where(x => x.Seeder == ArmageddonPierceInstaller.Package && x.Module == ArmageddonPierceInstaller.Module)
			.ToArray();
		if (records.Length == 0) return null;
		var key = ArmageddonReviewedPierceContent.Key;
		var expected = new[] { key, key + ".duration", key + ".cost", key + ".eligibility" };
		if (!records.Select(x => x.StableKey).Order().SequenceEqual(expected.Order()) ||
			records.Any(x => x.Retired || x.LogicalId is null || x.RevisionNumber is not null || x.ManifestVersion != ArmageddonPierceInstaller.Version))
			throw new InvalidOperationException("Existing owned Pierce is incomplete, retired or has unknown keys/version; resolve ownership before bootstrap.");
		foreach (var record in records)
		{
			var type = record.StableKey == key ? nameof(MagicSpell) :
				record.StableKey.EndsWith(".eligibility", StringComparison.Ordinal) ? nameof(FutureProg) : nameof(TraitExpression);
			var exists = type switch
			{
				nameof(MagicSpell) => db.MagicSpells.AsNoTracking().Any(x => x.Id == record.LogicalId),
				nameof(FutureProg) => db.FutureProgs.AsNoTracking().Any(x => x.Id == record.LogicalId),
				_ => db.TraitExpressions.AsNoTracking().Any(x => x.Id == record.LogicalId)
			};
			if (record.EntityType != type || !exists || db.SeederManagedRecords.AsNoTracking().Any(x => x.Id != record.Id &&
				x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
				throw new InvalidOperationException($"Existing Pierce {record.StableKey} is missing, has invalid type or competing ownership; no resurrection/adoption.");
		}
		var id = records.Single(x => x.StableKey == key).LogicalId!.Value;
		var spell = db.MagicSpells.AsNoTracking().Single(x => x.Id == id);
		if ((string?)XElement.Parse(spell.Definition).Element("StockIdentity") != key)
			throw new InvalidOperationException("Existing Pierce has no matching reviewed stock identity; preserved without admission or adoption.");
		return id;
	}
}
