#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Canonical S4 content batches. Never activates a partial roster or changes player state.</summary>
public static class ArmageddonStockSpellInstaller
{
	public const string Module = "stock-spells";
	public const string Version = "stock-spells-1";

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<string, ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		var messages = new List<string>();
		var identities = new Dictionary<string, long>(StringComparer.Ordinal);
		foreach (var contribution in ArmageddonStockSpellContent.Contributions)
		{
			var content = contribution.Content;
			var result = InstallContribution(factory, contribution, stage => checkpoint?.Invoke(content.Key, stage));
			messages.AddRange(result.Messages);
			foreach (var pair in result.Identities) identities.Add(pair.Key, pair.Value);
			if (result.Status == ArmageddonInstallStatus.Completed) continue;
			messages.Add("Earlier committed S4 contributions remain retained. Reconcile and resume with the same fixed settings; admissions remain disabled.");
			return new(result.Status, messages.AsReadOnly(), identities);
		}
		messages.Add("Canonical S4 definitions reconciled. Content identities alone do not qualify gameplay or enable package admissions, classes or regeneration.");
		return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), identities);
	}

	private static ArmageddonInstallResult InstallContribution(Func<FuturemudDatabaseContext> factory,
		ArmageddonCanonicalSpellContribution contribution, Action<ArmageddonInstallCheckpoint>? checkpoint)
	{
		var content = contribution.Content;
		using var db = factory();
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		ArmageddonInstallResult Result(ArmageddonInstallStatus status) => new(status, messages.AsReadOnly(), ids);
		var committed = false;
		var committing = false;
		try
		{
			if (db.ChangeTracker.Entries().Any() || db.Database.CurrentTransaction is not null)
				throw new InvalidOperationException("S4 contributions require a fresh dedicated context.");
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix package settings and install progression before S4 content.");
			var stock = new ArmageddonStockProgression(settings.Mode);
			// The canonical school/trait are defaults. Route-local admissions select their own trait and reserve.
			var cap = stock.Capabilities.Single(x => x.Key == contribution.NativeCapabilityKey);
			var traitKey = settings.Mode == ArmageddonProgressionMode.Classic ? content.Key + ".skill" : cap.MainTraitKey;
			long Owned<T>(string key) where T : class
			{
				var claims = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key).ToArray();
				bool CompatibleOwner(SeederManagedRecord claim) =>
					claim.ManifestVersion == ArmageddonProgressionInstaller.Version &&
					(claim.Module == ArmageddonProgressionInstaller.Module || typeof(T) == typeof(TraitDefinition) &&
						settings.Mode == ArmageddonProgressionMode.Classic && claim.Module == ArmageddonTraditionInstaller.Module);
				if (claims.Length != 1 || claims[0] is not { Retired: false, LogicalId: { } id, RevisionNumber: null } record ||
					!CompatibleOwner(record) || record.EntityType != typeof(T).Name || record.SeedBaseline is null || db.Find<T>(id) is null ||
					db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == id))
					throw new InvalidOperationException($"{key}: complete unique typed progression ownership is required; no name adoption.");
				return id;
			}
			var school = Owned<MagicSchool>(cap.ResourceKey.Replace("arm.resource.", "arm.school.", StringComparison.Ordinal));
			var trait = Owned<TraitDefinition>(traitKey);
			var resource = Owned<MagicResource>(ArmageddonProgressionInstaller.SourceResourceKey);
			var known = settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
			if (!db.FutureProgs.Any(x => x.Id == known) || !db.TraitDefinitions.Any(x => x.Id == trait && x.Type == 0 && x.OwnerScope == 1))
				throw new InvalidOperationException("S4 requires the fixed always-false knowledge prog and a character-owned skill.");
			var specs = new[]
			{
				new ArmageddonStockContribution(typeof(MagicSpell), content.Key, content.Name),
				new ArmageddonStockContribution(typeof(TraitExpression), content.Key + ".duration", ""),
				new ArmageddonStockContribution(typeof(TraitExpression), content.Key + ".cost", "")
			};
			var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
				specs.Select(y => y.Key).Contains(x.StableKey)).ToDictionary(x => x.StableKey, StringComparer.Ordinal);
			var reconciler = new ArmageddonStockContributionReconciler(db, ArmageddonMagicInstaller.Package, Module, Version);
			foreach (var spec in specs)
			{
				if (!records.TryGetValue(spec.Key, out var record)) continue;
				if (record.Module != Module || record.ManifestVersion != Version || record.Retired || record.RevisionNumber is not null ||
					record.EntityType != spec.Type.Name || record.SeedBaseline is null || reconciler.Find(spec, record) is null ||
					db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
					messages.Add($"{spec.Key}: missing, retired, cross-module or competing owned content; restore explicitly.");
			}
			if (records.Count != 0 && records.Count != specs.Length)
				messages.Add($"{content.Key}: incomplete retained contribution; no child resurrection.");
			if (records.TryGetValue(content.Key, out var retained) && reconciler.Find(specs[0], retained) is MagicSpell retainedSpell &&
				StockIdentity(retainedSpell.Definition) != content.Key)
				messages.Add($"{content.Key}: owned definition has lost its declared stock identity; preserve and resolve explicitly.");
			if (!records.ContainsKey(content.Key) && (db.MagicSpells.Any(x => x.Name.ToLower() == content.Name.ToLower()) ||
				db.MagicSpells.AsNoTracking().Select(x => x.Definition).AsEnumerable().Any(x => StockIdentity(x) == content.Key)))
				messages.Add($"{content.Key}: unowned spell name or declared stock identity collision; no adoption.");
			if (messages.Count != 0) return Result(ArmageddonInstallStatus.Blocked);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var isNew = !records.ContainsKey(content.Key);
			var spell = isNew ? content.SpellRow(school, trait, known) : (MagicSpell)reconciler.Find(specs[0], retained!)!;
			if (isNew) reconciler.Allocate(specs[0], spell, records);
			var bindings = new { settings.Mode, School = school, Trait = trait, Resource = resource, KnownProg = known };
			var duration = reconciler.Apply(specs[1], content.DurationRow(spell.Id), records, bindings, messages);
			var cost = reconciler.Apply(specs[2], content.CostRow(spell.Id), records, bindings, messages);
			var desired = content.SpellRow(school, trait, known);
			desired.EffectDurationExpressionId = duration.Id;
			desired.Definition = content.BuildDefinition(resource, cost.Id, 0).ToString();
			reconciler.Apply(specs[0], desired, records, bindings, messages, isNew);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction?.Commit();
			committed = true;
			foreach (var record in records) ids.Add(record.Key, record.Value.LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add($"{content.Key}: canonical spell and stable duration/cost children committed; no player refresh or admissions.");
			return Result(ArmageddonInstallStatus.Completed);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Inspect and rerun from a fresh context. " : committing ?
				"Commit outcome unknown. Inspect and reconcile owned identities before retrying. " : "S4 contribution failed before commit. ") + error.Message);
			return Result(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed);
		}
	}

	private static string? StockIdentity(string definition)
	{
		try { return (string?)XElement.Parse(definition).Element("StockIdentity"); }
		catch (System.Xml.XmlException) { return null; }
	}
}
