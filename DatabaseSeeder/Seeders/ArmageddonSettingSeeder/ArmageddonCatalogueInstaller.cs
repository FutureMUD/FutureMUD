#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

/// <summary>Resumable S3 batches; each native batch owns its own transaction and retained identities.</summary>
public static class ArmageddonCatalogueInstaller
{
	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<string, ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		var batches = new (string Name, Func<Func<FuturemudDatabaseContext>, Action<ArmageddonInstallCheckpoint>?, ArmageddonInstallResult> Install)[]
		{
			("supplies", ArmageddonCatalogueSupplies.Install),
			("weapons", ArmageddonCatalogueWeapons.Install),
			("provisions", ArmageddonCatalogueProvisions.Install),
			("anatomy", ArmageddonCatalogueAnatomy.Install),
			("creature-foundations", ArmageddonCatalogueCreatureFoundations.Install),
			("controllers", ArmageddonCatalogueControllers.Install),
			("creature-templates", ArmageddonCatalogueCreatures.Install),
			("specialised-stock", ArmageddonCatalogueSpecials.Install)
		};
		var messages = new List<string>(); var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		foreach (var batch in batches)
		{
			var result = batch.Install(factory, stage => checkpoint?.Invoke(batch.Name, stage));
			messages.AddRange(result.Messages.Select(x => $"{batch.Name}: {x}"));
			foreach (var identity in result.Identities) ids.Add(identity.Key, identity.Value);
			if (result.Status == ArmageddonInstallStatus.Completed) continue;
			messages.Add($"Stopped at {batch.Name}. Earlier committed batches are retained; rerun with the same fixed settings to reconcile and resume. Full spell activation remains disabled.");
			return new(result.Status, messages.AsReadOnly(), ids);
		}
		using var db = factory();
		var catalogue = new ArmageddonStockCatalogue();
		var missing = catalogue.Blueprints.Where(x => !x.DeferredToS4).Where(x => !db.SeederManagedRecords.Any(record =>
			record.Seeder == ArmageddonMagicInstaller.Package && record.StableKey == x.Key && !record.Retired)).Select(x => x.Key).ToArray();
		if (missing.Length != 0)
			return new(ArmageddonInstallStatus.Blocked, messages.Append("S3 native catalogue identities are missing: " + string.Join(", ", missing)).ToArray(), ids);
		messages.Add("S3 native catalogue content reconciled: all 198 nondeferred blueprint identities are present. The twelve foci/devices and their recipes remain S4 obligations. This content receipt does not certify S4 spell hydration, S5 exposure calibration or complete-gameplay acceptance.");
		return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
	}
}
