#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Unadvertised four-record contribution; atomic ownership and no player/admission writes.</summary>
public static partial class ArmageddonPierceInstaller
{
	public const string Package = ArmageddonMagicInstaller.Package;
	public const string Module = "reviewed-pierce";
	public const string Version = "partial-1";
	private sealed record Contribution(Type Type, string Key, string Name);
	private static List<Contribution> Contributions() =>
	[
		new(typeof(MagicSpell), ArmageddonReviewedPierceContent.Key, ArmageddonReviewedPierceContent.Name),
		new(typeof(TraitExpression), ArmageddonReviewedPierceContent.Key + ".duration", ""),
		new(typeof(TraitExpression), ArmageddonReviewedPierceContent.Key + ".cost", ""),
		new(typeof(FutureProg), ArmageddonReviewedPierceContent.Key + ".eligibility", "")
	];

	public static ArmageddonInstallResult Install(FuturemudDatabaseContext db, ArmageddonPierceInstallPlan plan,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		var messages = new List<string>();
		var ids = new Dictionary<string, long>();
		ArmageddonInstallResult Result(ArmageddonInstallStatus status) => new(status, messages.AsReadOnly(), new Dictionary<string, long>(ids));
		if (!plan.Install) return Result(ArmageddonInstallStatus.Declined);
		if (db.Database.CurrentTransaction is not null || db.ChangeTracker.Entries().Any())
		{
			messages.Add("Use a fresh dedicated context without tracked objects or a transaction.");
			return Result(ArmageddonInstallStatus.Blocked);
		}
		bool committed = false, committing = false;
		try
		{
			using var transaction = db.Database.IsRelational()
				? db.Database.BeginTransaction(IsolationLevel.Serializable) : db.Database.BeginTransaction();
			var contributions = Contributions();
			var records = db.SeederManagedRecords.Where(x => x.Seeder == Package && x.Module == Module).ToDictionary(x => x.StableKey);
			Preflight(db, plan, contributions, records, messages);
			if (messages.Count != 0) return Result(ArmageddonInstallStatus.Blocked);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var content = ArmageddonReviewedPierceContent.PierceConcealment();
			Contribution Spec(string suffix = "") => contributions.Single(x => x.Key == content.Key + suffix);
			var isNew = !records.TryGetValue(content.Key, out var retained);
			var spell = isNew ? content.SpellRow(plan.School, plan.PierceSkill, plan.AlwaysFalseProg)
				: (MagicSpell)Find(db, Spec(), retained!)!;
			if (isNew) Allocate(db, Spec(), spell, records);
			var duration = Apply(db, Spec(".duration"), content.DurationRow(spell.Id), records, plan, messages);
			var cost = Apply(db, Spec(".cost"), content.CostRow(spell.Id), records, plan, messages);
			var eligibility = Apply(db, Spec(".eligibility"), content.EligibilityRow(spell.Id)!, records, plan, messages);
			var desired = content.SpellRow(plan.School, plan.PierceSkill, plan.AlwaysFalseProg);
			desired.EffectDurationExpressionId = duration.Id;
			desired.Definition = content.BuildDefinition(plan.Resource, cost.Id, eligibility.Id).ToString();
			Apply(db, Spec(), desired, records, plan, messages, isNew);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			foreach (var record in records) ids.Add(record.Key, record.Value.LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("Committed four owned Pierce records. Dependencies remain builder-owned; no player refresh or admissions.");
			return Result(ArmageddonInstallStatus.Completed);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Fresh-context rerun resolves the same four identities. " : committing
				? "Commit outcome unknown. Reconnect/rerun without replaying player actions. " : "Failed before commit; batch rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return Result(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed
				: committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed);
		}
		finally { if (!committed) db.ChangeTracker.Clear(); }
	}
}
