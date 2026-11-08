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

/// <summary>Unadvertised seven-record module; content/ownership are atomic and no player records are refreshed.</summary>
public static partial class ArmageddonProvisionInstaller
{
	public const string Package = ArmageddonMagicInstaller.Package;
	public const string Module = "reviewed-provisions";
	public const string Version = "partial-1";
	private sealed record Contribution(Type Type, string Key, string Name);
	private static IReadOnlyList<ArmageddonUtilitySpellContent> Content(ArmageddonProvisionInstallPlan plan) =>
	[
		ArmageddonReviewedProvisionContent.SustainMeal(plan.FoodProfiles.OrderBy(x => x.Order).Last().Foods.Select(x => x.Id).ToArray()),
		ArmageddonReviewedProvisionContent.DrawWine(plan.Wine, plan.WineBonusPlane)
	];
	private static List<Contribution> Contributions() => new[]
	{
		new Contribution(typeof(MagicSpell), ArmageddonReviewedProvisionContent.SustainMealKey, ArmageddonReviewedProvisionContent.SustainMealName),
		new Contribution(typeof(MagicSpell), ArmageddonReviewedProvisionContent.DrawWineKey, ArmageddonReviewedProvisionContent.DrawWineName)
	}.SelectMany(x => new[] { x, new Contribution(typeof(TraitExpression), x.Key + ".duration", ""), new Contribution(typeof(TraitExpression), x.Key + ".cost", "") })
	.Concat([new Contribution(typeof(FutureProg), ArmageddonReviewedProvisionContent.DrawWineKey + ".eligibility", "")]).ToList();

	public static ArmageddonInstallResult Install(FuturemudDatabaseContext db, ArmageddonProvisionInstallPlan plan,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		var messages = new List<string>(); var ids = new Dictionary<string, long>();
		ArmageddonInstallResult Result(ArmageddonInstallStatus status) => new(status, messages.AsReadOnly(), new Dictionary<string, long>(ids));
		if (!plan.Install) return Result(ArmageddonInstallStatus.Declined);
		if (db.Database.CurrentTransaction is not null || db.ChangeTracker.Entries().Any())
		{ messages.Add("Use a fresh dedicated context without tracked objects or a transaction."); return Result(ArmageddonInstallStatus.Blocked); }
		bool committed = false, committing = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : db.Database.BeginTransaction();
			var contributions = Contributions();
			var records = db.SeederManagedRecords.Where(x => x.Seeder == Package && x.Module == Module).ToDictionary(x => x.StableKey);
			Preflight(db, plan, contributions, records, messages);
			if (messages.Count != 0) return Result(ArmageddonInstallStatus.Blocked);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			foreach (var content in Content(plan))
			{
				Contribution Spec(string suffix = "") => contributions.Single(x => x.Key == content.Key + suffix);
				var skill = content.Key == ArmageddonReviewedProvisionContent.SustainMealKey ? plan.MealSkill : plan.WineSkill;
				var isNew = !records.TryGetValue(content.Key, out var retained);
				var spell = isNew ? content.SpellRow(plan.School, skill, plan.AlwaysFalseProg) : (MagicSpell)Find(db, Spec(), retained!)!;
				if (isNew) Allocate(db, Spec(), spell, records);
				var duration = Apply(db, Spec(".duration"), content.DurationRow(spell.Id), records, plan, messages);
				var cost = Apply(db, Spec(".cost"), content.CostRow(spell.Id), records, plan, messages);
				var eligibility = content.EligibilityRow(spell.Id);
				var filter = eligibility is null ? 0 : Apply(db, Spec(".eligibility"), eligibility, records, plan, messages).Id;
				var desired = content.SpellRow(plan.School, skill, plan.AlwaysFalseProg);
				desired.EffectDurationExpressionId = duration.Id;
				var definition = content.BuildDefinition(plan.Resource, cost.Id, filter);
				if (content.Key == ArmageddonReviewedProvisionContent.SustainMealKey)
					definition.Descendants("FoodProfiles").Single().ReplaceWith(new XElement("FoodProfiles", new XAttribute("version", 1),
						plan.FoodProfiles.OrderBy(x => x.Order).Select(x => new XElement("Profile", new XAttribute("order", x.Order),
							new XAttribute("predicate", x.Predicate), x.Foods.Select(y => new XElement("Prototype", y.Id))))));
				else definition.Descendants("Recipes").Single().ReplaceWith(new XElement("Recipes", new XAttribute("version", 1),
					plan.WineRecipes.OrderBy(x => x.Order).Select(x => new XElement("Recipe", new XAttribute("order", x.Order),
						new XAttribute("predicate", x.Predicate), new XAttribute("liquid", x.Liquid)))));
				desired.Definition = definition.ToString();
				Apply(db, Spec(), desired, records, plan, messages, isNew);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges(); foreach (var record in records) ids.Add(record.Key, record.Value.LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit); committing = true; transaction.Commit(); committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("Committed seven owned Sustain Meal/Draw Wine records. Native dependencies remain builder-owned; no player refresh or admissions.");
			return Result(ArmageddonInstallStatus.Completed);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Fresh-context rerun resolves the same seven identities. " : committing ?
				"Commit outcome unknown. Reconnect/rerun without replaying player actions. " : "Failed before commit; batch rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return Result(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed);
		}
		finally { if (!committed) db.ChangeTracker.Clear(); }
	}
}
