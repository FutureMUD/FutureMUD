extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using FuryStock = EngineCompiler::MudSharp.Magic.ArmageddonRousedFuryStock;
using CalmStock = EngineCompiler::MudSharp.Magic.ArmageddonStillAngerStock;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

/// <summary>Six owned definition rows; external mappings and character state remain builder-owned.</summary>
public static class ArmageddonEmotionalInstaller
{
	public const string Package = ArmageddonMagicInstaller.Package;
	public const string Module = "reviewed-fury-calm";
	public const string Version = "partial-1";
	public const string FuryKey = FuryStock.Key;
	public const string CalmKey = CalmStock.Key;
	public static IReadOnlyList<string> SpellKeys => [FuryKey, CalmKey];
	private static ArmageddonStockContribution[] Contributions() => new[] { (FuryKey, FuryStock.Name), (CalmKey, CalmStock.Name) }
		.SelectMany(x => new[] { new ArmageddonStockContribution(typeof(MagicSpell), x.Item1, x.Item2),
			new(typeof(TraitExpression), x.Item1 + ".duration", ""), new(typeof(TraitExpression), x.Item1 + ".cost", "") }).ToArray();

	internal static long ResolveAttribute(FuturemudDatabaseContext db, long? selected)
	{
		if (selected is { } id)
		{
			if (id <= 0 || !db.TraitDefinitions.AsNoTracking().Any(x => x.Id == id && x.Type == 1 && x.OwnerScope == 0))
				throw new ArgumentException("FuryAttribute must select an existing ordinary body-owned attribute; derived attributes are unsupported.");
			return id;
		}
		// Reuse SkillPackageSeeder's Constitution aliases, with no precedence guess when several exist.
		var aliases = new[] { "constitution", "physique", "endurance", "body" };
		var candidates = db.TraitDefinitions.AsNoTracking().Where(x => x.Type == 1 && x.OwnerScope == 0)
			.AsEnumerable().Where(x => aliases.Contains(x.Name.ToLowerInvariant())).ToArray();
		if (candidates.Length != 1)
			throw new ArgumentException($"Fury attribute inference found {candidates.Length} ordinary body attributes matching Constitution/Physique/Endurance/Body. Supply FuryAttribute explicitly; no first-match or derived-attribute inference.");
		return candidates[0].Id;
	}

	internal static IReadOnlyList<string> ValidateMappings(FuturemudDatabaseContext db, ArmageddonEmotionalBindings? bindings)
	{
		var errors = new List<string>();
		if (bindings?.Terrains is null || bindings.CalmSaves is null)
			return ["Emotions requires explicit terrain, intensity, attribute-unit, save and eligibility mappings."];
		long attribute = 0;
		try { attribute = ResolveAttribute(db, bindings.FuryAttribute); }
		catch (ArgumentException error) { errors.Add(error.Message); }
		var t = bindings.Terrains;
		var terrains = new[] { t.Air, t.City, t.Inside, t.Hills, t.Mountain, t.Thornlands, t.Earth };
		if (terrains.Any(id => id <= 0 || !db.Terrains.AsNoTracking().Any(x => x.Id == id)) || terrains.Distinct().Count() != 7)
			errors.Add("Fury requires seven distinct existing native terrain IDs in Air/City/Inside/Hills/Mountain/Thornlands/Earth order.");
		if (bindings.CalmSaveTrait <= 0 || !db.TraitDefinitions.AsNoTracking().Any(x => x.Id == bindings.CalmSaveTrait &&
			((x.Type == 0 || x.Type == 2) && x.OwnerScope == 1 || (x.Type == 1 || x.Type == 3) && x.OwnerScope == 0)))
			errors.Add("CalmSaveTrait must select an existing native character skill or body attribute.");
		if (bindings.CalmSaves.Count != 7 || bindings.CalmSaves.Any(x => !Enum.IsDefined(x)))
			errors.Add("CalmSaves must contain exactly seven named native difficulties, in grade 1 through 7 order.");
		try
		{
			var progs = db.FutureProgs.AsNoTracking().Include(x => x.FutureProgsParameters).ToArray();
			using var compiler = new OfflineProgCompilation(progs);
			foreach (var id in new[] { bindings.FuryEligibilityProg, bindings.CalmEligibilityProg }.Distinct())
			{
				if (id <= 0 || progs.All(x => x.Id != id)) { errors.Add("Emotional eligibility must select existing native progs."); continue; }
				var compiled = compiler.Compile(id);
				if (compiled.ReturnType != ProgVariableTypes.Boolean || !compiled.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]))
					errors.Add($"Eligibility #{id} must compile as boolean(target character, caster character).");
			}
			if (attribute > 0 && bindings.CalmSaves.Count == 7)
				_ = Content(bindings with { FuryAttribute = attribute }); // Canonical profile validates all numerical fields.
		}
		catch (Exception error) when (error is ArgumentException or InvalidOperationException or FormatException)
		{ errors.Add("Invalid emotional mapping: " + error.Message); }
		return errors;
	}

	private static IReadOnlyList<ArmageddonUtilitySpellContent> Content(ArmageddonEmotionalBindings b)
	{
		var t = b.Terrains;
		var fury = FuryStock.Profile(b.FuryAttribute!.Value, b.UnitsPerSourcePoint, b.FuryIntensity, b.FuryEligibilityProg,
			new(t.Air, t.City, t.Inside, t.Hills, t.Mountain, t.Thornlands, t.Earth));
		var calm = CalmStock.Profile(b.CalmSaveTrait, b.CalmIntensity, b.CalmEligibilityProg,
			b.CalmSaves.Select((difficulty, index) => new KeyValuePair<int, Difficulty>(index + 1, difficulty)));
		return [new(FuryKey, FuryStock.Name, "Source Fury grants the selected attribute bonus without replenishing stamina.",
			FuryStock.DurationFormula, FuryStock.MinimumEnergy, "$0 rouse|rouses a fierce enchantment.", (resource, cost, _) => FuryStock.Definition(resource, cost, fury)),
			new(CalmKey, CalmStock.Name, "Source Calm counters Fury and uses prepared selective combat cessation; an admitted incoming hostile attack breaks it.",
				CalmStock.DurationFormula, CalmStock.MinimumEnergy, "$0 shape|shapes a calming enchantment.", (resource, cost, _) => CalmStock.Definition(resource, cost, calm))];
	}

	internal static IReadOnlyDictionary<string, long> PreservedSpells(FuturemudDatabaseContext db, long? school = null)
	{
		var records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == Package && x.Module == Module).ToDictionary(x => x.StableKey);
		var errors = new List<string>(); ValidateOwnership(db, records, school, errors);
		if (errors.Count != 0) throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
		return SpellKeys.Where(records.ContainsKey).ToDictionary(x => x, x => records[x].LogicalId!.Value);
	}

	private static void ValidateOwnership(FuturemudDatabaseContext db, Dictionary<string, SeederManagedRecord> records, long? school, List<string> errors)
	{
		var specs = Contributions(); var reconcile = new ArmageddonStockContributionReconciler(db, Package, Module, Version);
		if (records.Count > 0 && !records.Keys.Order().SequenceEqual(specs.Select(x => x.Key).Order()))
			errors.Add("Existing owned Fury/Calm keys are incomplete or unknown; reconcile explicitly, no resurrection.");
		foreach (var spec in specs)
		{
			if (records.TryGetValue(spec.Key, out var record))
			{
				var row = reconcile.Find(spec, record);
				if (record.Retired || record.EntityType != spec.Type.Name || record.RevisionNumber is not null || record.ManifestVersion != Version || row is null)
					errors.Add($"{spec.Key}: owned row missing, retired or invalid; preserved without repair.");
				if (db.SeederManagedRecords.AsNoTracking().Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
					errors.Add($"{spec.Key}: competing retained ownership claim.");
				if (row is MagicSpell spell && StockIdentity(spell.Definition) != spec.Key)
					errors.Add($"{spec.Key}: owned spell has no matching reviewed stock identity.");
			}
			else if (db.SeederManagedRecords.AsNoTracking().Any(x => x.Seeder == Package && x.StableKey == spec.Key))
				errors.Add($"{spec.Key}: cross-module ownership; no adoption.");
			else if (school is { } schoolId && spec.Type == typeof(MagicSpell) &&
				(db.MagicSpells.AsNoTracking().Any(x => x.MagicSchoolId == schoolId && x.Name.ToLower() == spec.Name.ToLower()) ||
				 db.MagicSpells.AsNoTracking().Select(x => x.Definition).AsEnumerable().Any(x => StockIdentity(x) == spec.Key)))
				errors.Add($"{spec.Key}: unowned name/stock-identity collision; no adoption.");
		}
	}
	private static string? StockIdentity(string xml) { try { return (string?)XElement.Parse(xml).Element("StockIdentity"); } catch (System.Xml.XmlException) { return null; } }

	public static ArmageddonInstallResult Install(FuturemudDatabaseContext db, ArmageddonEmotionalInstallPlan plan, Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		var messages = new List<string>(); var ids = new Dictionary<string, long>();
		ArmageddonInstallResult Result(ArmageddonInstallStatus status) => new(status, messages.ToArray(), new Dictionary<string, long>(ids));
		if (!plan.Install) return Result(ArmageddonInstallStatus.Declined);
		if (db.Database.CurrentTransaction is not null || db.ChangeTracker.Entries().Any())
		{ messages.Add("Use a fresh dedicated context without tracked objects or a transaction."); return Result(ArmageddonInstallStatus.Blocked); }
		bool committed = false, committing = false;
		try
		{
			messages.AddRange(ValidateMappings(db, plan.Bindings));
			if (messages.Count != 0) return Result(ArmageddonInstallStatus.Blocked);
			plan = plan with { Bindings = plan.Bindings with { FuryAttribute = ResolveAttribute(db, plan.Bindings.FuryAttribute), CalmSaves = plan.Bindings.CalmSaves.ToArray() } };
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : db.Database.BeginTransaction();
			var records = db.SeederManagedRecords.Where(x => x.Seeder == Package && x.Module == Module).ToDictionary(x => x.StableKey);
			ValidateOwnership(db, records, plan.School, messages);
			if (plan.School <= 0 || !db.MagicSchools.Any(x => x.Id == plan.School) || plan.Resource <= 0 ||
				!db.MagicResources.Any(x => x.Id == plan.Resource && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0))
				messages.Add("Select an existing school and player-capable source resource.");
			foreach (var (key, skill) in new[] { (FuryKey, plan.FurySkill), (CalmKey, plan.CalmSkill) })
			{
				var owner = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == Package && x.Module == ArmageddonTraditionInstaller.Module && x.StableKey == key + ".skill");
				if (owner is null || owner.Retired || owner.EntityType != nameof(TraitDefinition) || owner.LogicalId != skill || owner.RevisionNumber is not null ||
					owner.ManifestVersion != ArmageddonTraditionInstaller.Version || !db.TraitDefinitions.Any(x => x.Id == skill && x.Type == 0 && x.OwnerScope == 1) ||
					db.SeederManagedRecords.Any(x => x.Id != owner.Id && x.EntityType == nameof(TraitDefinition) && x.LogicalId == skill))
					messages.Add($"{key}: use the owned character-source skill from tradition bootstrap.");
			}
			var progs = db.FutureProgs.AsNoTracking().Include(x => x.FutureProgsParameters).ToArray();
			using var compiler = new OfflineProgCompilation(progs);
			if (progs.SingleOrDefault(x => x.Id == plan.AlwaysFalseProg) is not { } no || no.FunctionText.Trim() != "return false" ||
				compiler.Compile(no.Id) is var compiled && (compiled.ReturnType != ProgVariableTypes.Boolean || !compiled.MatchesParameters([])))
				messages.Add("Select the compiled ordinary no-argument always-false support prog.");
			if (messages.Count != 0) return Result(ArmageddonInstallStatus.Blocked);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var specs = Contributions(); var reconcile = new ArmageddonStockContributionReconciler(db, Package, Module, Version);
			foreach (var content in Content(plan.Bindings))
			{
				ArmageddonStockContribution Spec(string suffix = "") => specs.Single(x => x.Key == content.Key + suffix);
				var skill = content.Key == FuryKey ? plan.FurySkill : plan.CalmSkill;
				var isNew = !records.TryGetValue(content.Key, out var retained);
				var spell = isNew ? content.SpellRow(plan.School, skill, plan.AlwaysFalseProg) : (MagicSpell)reconcile.Find(Spec(), retained!)!;
				if (isNew) reconcile.Allocate(Spec(), spell, records);
				var duration = reconcile.Apply(Spec(".duration"), content.DurationRow(spell.Id), records, plan, messages);
				var cost = reconcile.Apply(Spec(".cost"), content.CostRow(spell.Id), records, plan, messages);
				var desired = content.SpellRow(plan.School, skill, plan.AlwaysFalseProg);
				desired.EffectDurationExpressionId = duration.Id; desired.Definition = content.BuildDefinition(plan.Resource, cost.Id, 0).ToString();
				reconcile.Apply(Spec(), desired, records, plan, messages, isNew);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated); db.SaveChanges();
			foreach (var record in records) ids.Add(record.Key, record.Value.LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit); committing = true; transaction.Commit(); committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("Committed six owned Fury/Calm records. External eligibility progs, mappings and player state remain untouched; admissions belong to final tradition composition.");
			return Result(ArmageddonInstallStatus.Completed);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown. " : "Failed before commit; batch rolled back on a relational provider. ") + error.Message);
			if (!committed) ids.Clear();
			return Result(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed);
		}
		finally { if (!committed) db.ChangeTracker.Clear(); }
	}
}
