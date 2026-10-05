#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Unadvertised source-definition slice. Does not install spells, classes or any player state.</summary>
public static partial class ArmageddonTraditionInstaller
{
	private sealed record Definition(Type Type, string Key, string Name, string[] Dependencies);
	private static readonly IReadOnlyDictionary<Type, string[]> OwnedFields = new Dictionary<Type, string[]>
	{
		[typeof(TraitExpression)] = ["Name", "Expression"],
		[typeof(Improver)] = ["Name", "Type", "Definition"],
		[typeof(TraitDefinition)] = ["Name", "Type", "OwnerScope", "DecoratorId", "TraitGroup", "DerivedType", "ExpressionId", "ImproverId", "Hidden",
			"ChargenBlurb", "BranchMultiplier", "Alias", "AvailabilityProgId", "TeachableProgId", "LearnableProgId", "TeachDifficulty", "LearnDifficulty", "ValueExpression"],
		[typeof(MagicCapability)] = ["Name", "CapabilityModel", "PowerLevel", "Definition", "MagicSchoolId"],
		[typeof(Merit)] = ["Name", "Type", "MeritType", "MeritScope", "Definition"]
	};
	private static List<Definition> Definitions() =>
		new[] { new Definition(typeof(Improver), "arm.improver.spell_practice", "Armageddon spell practice", []) }
		.Concat(SourceRows.Where(x => x.Kind == "spell").SelectMany(x => new[]
		{
			new Definition(typeof(TraitExpression), x.Key + ".skill.cap", "Armageddon " + x.HistoricalName + " cap", []),
			new Definition(typeof(TraitDefinition), x.Key + ".skill", "Armageddon " + x.HistoricalName,
				[x.Key + ".skill.cap", "arm.improver.spell_practice", "external.decorator", "external.progs"])
		}))
		.Concat(Variants.SelectMany(x => new[]
		{
			new Definition(typeof(MagicCapability), "arm.capability." + x, "Armageddon partial " + x,
				["external.school", "external.mana", "external.gathering", "external.implemented_spells", "external.support_skills"]),
			new Definition(typeof(Merit), "arm.merit." + x, "Armageddon partial " + x + " caster", ["arm.capability." + x, "external.progs"])
		})).ToList();

	public static ArmageddonTraditionInstallResult Install(FuturemudDatabaseContext db, ArmageddonTraditionInstallPlan plan,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
		=> Install(db, plan, false, checkpoint);

	private static ArmageddonTraditionInstallResult Install(FuturemudDatabaseContext db, ArmageddonTraditionInstallPlan plan,
		bool bootstrapDefinitionsOnly, Action<ArmageddonInstallCheckpoint>? checkpoint)
	{
		var messages = new List<string>(); var ids = new Dictionary<string, long>(); var available = new HashSet<string>();
		ArmageddonTraditionInstallResult Result(ArmageddonInstallStatus status) => new(status, messages.AsReadOnly(), ids,
			SourceRows.Where(x => x.Kind == "spell" && available.Contains(x.Key)).Select(x => x.Key).ToArray(),
			SourceRows.Where(x => x.Kind == "spell" && !available.Contains(x.Key)).Select(x => x.Key).ToArray());
		if (!plan.Install) return new(ArmageddonInstallStatus.Declined, [], ids, [], []);
		if (db.Database.CurrentTransaction is not null || db.ChangeTracker.HasChanges())
		{ messages.Add("Use a fresh dedicated context without changes or an existing transaction."); return Result(ArmageddonInstallStatus.Blocked); }
		var original = db.ChangeTracker.Entries().Select(x => x.Entity).ToHashSet(); bool committed = false, committing = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : db.Database.BeginTransaction();
			var definitions = Definitions();
			var records = db.SeederManagedRecords.Where(x => x.Seeder == Package && x.Module == Module).ToDictionary(x => x.StableKey);
			Preflight(db, plan, definitions, records, messages);
			if (messages.Count != 0) return Result(ArmageddonInstallStatus.Blocked);
			available = Reachable(plan);
			if (!available.Any(x => x.StartsWith("arm.spell.", StringComparison.Ordinal)))
			{ messages.Add("No implemented source root is available. Supply a reviewed source-root spell; no fabricated roots are installed."); return Result(ArmageddonInstallStatus.Blocked); }
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var skills = new Dictionary<string, long>(plan.SupportSkills);
			var improver = Apply(db, definitions[0], new Improver { Name = definitions[0].Name, Type = "classic",
				Definition = "<Definition Chance='0.1' Expression='0.5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='60' ImprovementProg='0'/>" }, records, plan, messages);
			foreach (var row in SourceRows.Where(x => x.Kind == "spell"))
			{
				var capSpec = definitions.Single(x => x.Key == row.Key + ".skill.cap");
				var cap = Apply(db, capSpec, new TraitExpression { Name = capSpec.Name, Expression = row.RawCap.ToString(CultureInfo.InvariantCulture) }, records, plan, messages);
				var spec = definitions.Single(x => x.Key == row.Key + ".skill");
				var trait = Apply(db, spec, new TraitDefinition { Name = spec.Name, Type = 0, OwnerScope = 1, DecoratorId = plan.Decorator,
					TraitGroup = "Armageddon Spell", ExpressionId = cap.Id, ImproverId = improver.Id, Hidden = false,
					ChargenBlurb = "Acquisition and controlled grade require a legitimately enrolled capability. A skill alone grants no spell knowledge.",
					Alias = "", ValueExpression = "", BranchMultiplier = 0, AvailabilityProgId = plan.AlwaysFalseProg,
					TeachableProgId = plan.AlwaysFalseProg, LearnableProgId = plan.AlwaysTrueProg, TeachDifficulty = 7, LearnDifficulty = 7 }, records, plan, messages);
				skills.Add(row.Key, trait.Id);
			}
			var template = XElement.Parse(db.MagicCapabilities.Single(x => x.Id == plan.GatheringTemplate).Definition);
			foreach (var variant in bootstrapDefinitionsOnly ? Array.Empty<string>() : Variants)
			{
				var spec = definitions.Single(x => x.Key == "arm.capability." + variant);
				var capability = Apply(db, spec, new MagicCapability { Name = spec.Name, CapabilityModel = "skilllevel", PowerLevel = 1,
					MagicSchoolId = plan.School, Definition = Capability(plan, variant, template, skills, available).ToString() }, records, plan, messages);
				spec = definitions.Single(x => x.Key == "arm.merit." + variant);
				Apply(db, spec, new Merit { Name = spec.Name, Type = "Magic Capability", MeritScope = 2, MeritType = 0,
					Definition = new XElement("Definition", new XElement("ChargenAvailableProg", plan.AlwaysFalseProg),
						new XElement("ApplicabilityProg", plan.AlwaysTrueProg), new XElement("ChargenBlurb", "Partial source tradition; explicit builder attachment and enrolment required."),
						new XElement("DescriptionText", "@ have|has an Armageddon magic capability."),
						new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() }, records, plan, messages);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges(); foreach (var record in records) ids.Add(record.Key, record.Value.LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit); committing = true; transaction.Commit(); committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add(bootstrapDefinitionsOnly
				? "Committed 82 distinct spell skill definitions. Capability and merit definitions and ownership baselines were not reconciled; admission policy awaits the complete implemented-spell plan. No player or class refresh."
				: $"Committed 82 distinct spell skill definitions and three partial capability/merit pairs; {available.Count(x => x.StartsWith("arm.spell.", StringComparison.Ordinal))}/82 prerequisite-closed admissions. No player or class refresh.");
			foreach (var row in SourceRows.Where(x => x.Kind == "spell" && !available.Contains(x.Key)))
				messages.Add($"Unavailable {row.Key}: {(plan.ImplementedSpells.ContainsKey(row.Key) ? "source prerequisite path is incomplete" : "implemented spell binding not supplied")}; skill definition is not spell entitlement.");
			foreach (var row in SourceRows.Where(x => x.Kind == "support" && !available.Contains(x.Key)))
				messages.Add($"Support {row.Key}: {(plan.SupportSkills.ContainsKey(row.Key) ? "mapping retained as source data; no unrelated automatic grant" : "native mapping not supplied")}. Source opening {row.Opening}, cap {row.RawCap}, parent {row.ParentKey ?? "support root"}.");
			return Result(ArmageddonInstallStatus.Completed);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Fresh-context rerun resolves the same owned IDs. " : committing ?
				"Commit outcome unknown; reconnect and rerun without replaying player actions. " : "Failed before commit; batch rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return Result(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed);
		}
		finally
		{
			if (!committed) foreach (var entry in db.ChangeTracker.Entries().Where(x => !original.Contains(x.Entity)).ToArray()) entry.State = EntityState.Detached;
		}
	}
}
