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

/// <summary>One atomic eight-record contribution; admissions belong to the final tradition composition.</summary>
public static partial class ArmageddonWaterSeeInstaller
{
	public const string Package = ArmageddonMagicInstaller.Package;
	public const string Module = "reviewed-water-see";
	public const string Version = "partial-1";
	public const string WaterBreathingKey = "arm.spell.water_breathing";
	public const string SeeTheUnbodiedKey = "arm.spell.see_the_unbodied";
	public static IReadOnlyList<string> SpellKeys => [WaterBreathingKey, SeeTheUnbodiedKey];
	private sealed record Contribution(Type Type, string Key, string Name);
	private static List<Contribution> Contributions() => new[]
	{
		(WaterBreathingKey, "Water Breathing"), (SeeTheUnbodiedKey, "See the Unbodied")
	}.SelectMany(x => new[]
	{
		new Contribution(typeof(MagicSpell), x.Item1, x.Item2),
		new Contribution(typeof(TraitExpression), x.Item1 + ".duration", ""),
		new Contribution(typeof(TraitExpression), x.Item1 + ".cost", ""),
		new Contribution(typeof(FutureProg), x.Item1 + ".eligibility", "")
	}).ToList();

	private static IReadOnlyList<ArmageddonUtilitySpellContent> Content(ArmageddonWaterSeeInstallPlan plan) =>
	[
		ArmageddonReviewedWaterSeeContent.WaterBreathing(plan.Bindings.WaterLiquids),
		ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(plan.Bindings.SiltTerrain,
			plan.Bindings.ShadowTerrain, plan.Bindings.DivinationRankTags)
	];

	public static ArmageddonInstallResult Install(FuturemudDatabaseContext db, ArmageddonWaterSeeInstallPlan plan,
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
		if (plan.Bindings?.WaterLiquids is null || plan.Bindings.DivinationRankTags is null)
		{
			messages.Add("Select explicit water liquids, terrains and ordered Divination rank tags.");
			return Result(ArmageddonInstallStatus.Blocked);
		}
		// Caller collections cannot change the selected content after preflight/checkpoint callbacks.
		plan = plan with { Bindings = plan.Bindings with
			{ WaterLiquids = plan.Bindings.WaterLiquids.ToArray(), DivinationRankTags = plan.Bindings.DivinationRankTags.ToArray() } };
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
			foreach (var content in Content(plan))
			{
				Contribution Spec(string suffix = "") => contributions.Single(x => x.Key == content.Key + suffix);
				var skill = content.Key == WaterBreathingKey ? plan.WaterBreathingSkill : plan.SeeTheUnbodiedSkill;
				var isNew = !records.TryGetValue(content.Key, out var retained);
				var spell = isNew ? content.SpellRow(plan.School, skill, plan.AlwaysFalseProg)
					: (MagicSpell)Find(db, Spec(), retained!)!;
				if (isNew) Allocate(db, Spec(), spell, records);
				var duration = Apply(db, Spec(".duration"), content.DurationRow(spell.Id), records, plan, messages);
				var cost = Apply(db, Spec(".cost"), content.CostRow(spell.Id), records, plan, messages);
				var eligibility = Apply(db, Spec(".eligibility"), content.EligibilityRow(spell.Id)!, records, plan, messages);
				var desired = content.SpellRow(plan.School, skill, plan.AlwaysFalseProg);
				desired.EffectDurationExpressionId = duration.Id;
				desired.Definition = content.BuildDefinition(plan.Resource, cost.Id, eligibility.Id).ToString();
				Apply(db, Spec(), desired, records, plan, messages, isNew);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			foreach (var record in records) ids.Add(record.Key, record.Value.LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("Committed eight owned Water/See records. External mappings remain builder-owned; no player refresh or admissions.");
			return Result(ArmageddonInstallStatus.Completed);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Inspect/rerun from a fresh context to resolve the same eight identities. " : committing
				? "Commit outcome unknown. Inspect/rerun from a fresh context without replaying player actions. " : "Failed before commit; batch rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return Result(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed
				: committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed);
		}
		finally { if (!committed) db.ChangeTracker.Clear(); }
	}
}
