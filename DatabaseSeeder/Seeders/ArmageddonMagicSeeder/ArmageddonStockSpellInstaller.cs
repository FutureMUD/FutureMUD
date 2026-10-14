extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Framework.Revision;
using MudSharp.Framework;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Canonical S4 content batches. Never activates a partial roster or changes player state.</summary>
public static partial class ArmageddonStockSpellInstaller
{
	public const string Module = "stock-spells";
	public const string Version = "stock-spells-1";

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<string, ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		var messages = new List<string>();
		var identities = new Dictionary<string, long>(StringComparer.Ordinal);
		var contributions = Contributions();
		foreach (var contribution in contributions)
		{
			if (contribution.Key == ArmageddonExistingFactorySpellContent.Keys[0])
			{
				var supports = ArmageddonStockSupportInstaller.Install(factory);
				messages.AddRange(supports.Messages);
				if (supports.Status != ArmageddonInstallStatus.Completed)
					return new(supports.Status, messages.AsReadOnly(), identities);
			}
			var result = InstallContribution(factory, contribution.Resolve, stage => checkpoint?.Invoke(contribution.Key, stage));
			messages.AddRange(result.Messages);
			foreach (var pair in result.Identities) identities.Add(pair.Key, pair.Value);
			if (result.Status == ArmageddonInstallStatus.Completed) continue;
			messages.Add("Earlier committed S4 contributions remain retained. Reconcile and resume with the same fixed settings; admissions remain disabled.");
			return new(result.Status, messages.AsReadOnly(), identities);
		}
		messages.Add("Canonical S4 definitions reconciled. Content identities alone do not qualify gameplay or enable package admissions, classes or regeneration.");
		return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), identities);
	}

	public static IReadOnlyList<string> CanonicalKeys => Contributions().Select(x => x.Key).ToArray();

	private static IEnumerable<(string Key, Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution> Resolve)> Contributions() =>
		ArmageddonStockSpellContent.Contributions
		.Select(x => (Key: x.Content.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => x)))
		.Concat(ArmageddonCreatureSpellContent.Contributions.Select(x =>
			(Key: x.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveCreature(db, x)))))
		.Concat(ArmageddonShelterSpellContent.Contributions.Select(x =>
			(Key: x.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveShelter(db, x)))))
		.Append((Key: "arm.spell.folded_pocket", Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)ResolvePocket))
		.Concat(PrimitiveKeys.Select(key => (Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolvePrimitive(db, key)))))
		.Concat(ArmageddonPrimitiveSpellContent.CreateScalarContributions().Append(ArmageddonPrimitiveSpellContent.CreateAlcoholRemovalContribution())
			.Select(x => (Key: x.Content.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => x))))
		.Append((Key: ArmageddonNativeReserveSpellContent.StealBreathKey, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => ArmageddonNativeReserveSpellContent.StealBreath())))
		.Concat(new[] { "arm.spell.sow_sickness", "arm.spell.purge_disease" }.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveCondition(db, key)))))
		.Concat(ArmageddonExistingFactorySpellContent.Keys.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveExistingFactory(db, key)))))
		.Concat(new[] { "arm.spell.vital_siphon", "arm.spell.devouring_touch", "arm.spell.leeching_edge" }.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveVital(db, key)))))
		.Concat(ArmageddonGradedWeaponSpellContent.Keys.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveGradedWeapon(db, key)))))
		.Concat(new[] { ("fire", ArmageddonBarrierSpellContent.FlameBarrier()), ("water", ArmageddonBarrierSpellContent.ThornBarrier()),
			("void", ArmageddonBarrierSpellContent.BladeBarrier()) }.Select(x =>
			(Key: x.Item2.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => new("arm.capability." + x.Item1, x.Item2)))))
		.Concat(SimpleKeys.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveSimple(db, key)))))
		.Concat(UtilityKeys.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveUtility(db, key)))))
		.Concat(RetainedUtilityKeys.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveRetainedUtility(db, key)))))
		.Append((Key: "arm.spell.turn_the_elements", Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db =>
			ArmageddonTurnElementsContent.Create(ProgressionIdentity<MagicSchool>(db, "arm.school.fire")))))
		.Concat(ArmourKeys.Select(x =>
			(Key: "arm.spell." + x.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveArmour(db, "arm.spell." + x.Key, x.School)))))
		.Concat(ArmageddonScalarUtilityContent.Create().Select(x =>
			(Key: x.Content.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => x))))
		.Concat(new[] { "arm.spell.ember_lance", "arm.spell.pillar_of_flame", "arm.spell.lightning_lance", "arm.spell.mind_scour" }.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveDirectDamage(db, key)))))
		.Append((Key: "arm.spell.consuming_flame", Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)ResolveConsumingFlame))
		.Append((Key: "arm.spell.false_presence", Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)ResolveFalsePresence))
		.Append((Key: "arm.spell.water_breathing", Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)ResolveWaterBreathing))
		.Append((Key: "arm.spell.see_the_unbodied", Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)ResolveSeeTheUnbodied))
		.Concat(new[] { "arm.spell.fleet_step", "arm.spell.lightning_pace", "arm.spell.leadfoot" }.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db => ResolveMovementTempo(db, key)))))
		.Append((Key: ArmageddonImmolateSpellContent.Key,
			Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => ArmageddonImmolateSpellContent.Create())))
		.Append((Key: ArmageddonSenseToxinSpellContent.Key,
			Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => ArmageddonSenseToxinSpellContent.Create())))
		.Concat(new[] { ArmageddonToxinSpellContent.VenomTouchKey, ArmageddonToxinSpellContent.PurgeToxinKey }.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db =>
				ArmageddonToxinSpellContent.Create(CatalogueIdentity<Drug>(db, ArmageddonToxinSpellContent.OwnedFamily))
					.Single(x => x.Content.Key == key)))))
		.Concat(ArmageddonWindForceSpellContent.Create().Select(x =>
			(Key: x.Content.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => x))))
		.Concat(ArmageddonNeedHarmSpellContent.Create().Select(x =>
			(Key: x.Content.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => x))))
		.Concat(new[] { ArmageddonAreaHarmSpellContent.CinderRainKey, ArmageddonAreaHarmSpellContent.ThunderclapKey,
			ArmageddonAreaHarmSpellContent.ScouringGustKey, ArmageddonAreaHarmSpellContent.CausticSprayKey }.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db =>
				ArmageddonAreaHarmSpellContent.Create(CharacterResistance(db, "water", ArmageddonStockSpellContent.ReadAttunementKey))
					.Single(x => x.Content.Key == key)))))
		.Concat(ArmageddonItemConditionSpellContent.Create().Select(x =>
			(Key: x.Content.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => x))))
		.Concat(ArmageddonOwnedItemMarkSpellContent.Create().Select(x =>
			(Key: x.Content.Key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => x))))
		.Append((Key: ArmageddonThresholdAlarmSpellContent.Key,
			Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => ArmageddonThresholdAlarmSpellContent.Create())))
		.Concat(new[] { ArmageddonPlanarSpellContent.ShadowPassageKey, ArmageddonPlanarSpellContent.CrossTheVeilKey }.Select(key =>
			(Key: key, Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db =>
				ArmageddonPlanarSpellContent.Create(CatalogueIdentity<Plane>(db, ArmageddonStockSupportInstaller.ShadowPlaneKey))
					.Single(x => x.Content.Key == key)))))
		.Append((Key: "arm.spell.unbodied_journey", Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db =>
			new("arm.capability.void", ArmageddonExistingFactorySpellContent.UnbodiedJourney(
				CatalogueIdentity<Plane>(db, ArmageddonStockSupportInstaller.ShadowPlaneKey))))))
		.Append((Key: ArmageddonEtherealRemovalContent.Key,
			Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(_ => ArmageddonEtherealRemovalContent.Create())))
		.Append((Key: ArmageddonDrowningGripSpellContent.Key,
			Resolve: (Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution>)(db =>
				ArmageddonDrowningGripSpellContent.Create(CharacterResistance(db, "water", ArmageddonStockSpellContent.ReadAttunementKey)))));

	private static ArmageddonInstallResult InstallContribution(Func<FuturemudDatabaseContext> factory,
		Func<FuturemudDatabaseContext, ArmageddonCanonicalSpellContribution> resolve, Action<ArmageddonInstallCheckpoint>? checkpoint)
	{
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
			var contribution = resolve(db);
			var content = contribution.Content;
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
			}.ToList();
			if (content.EligibilitySource is not null) specs.Add(new(typeof(FutureProg), content.Key + ".eligibility", ""));
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
			if (records.Count != 0 && records.Count != specs.Count)
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
			long filter = 0;
			if (content.EligibilitySource is not null)
			{
				var spec = specs.Single(x => x.Type == typeof(FutureProg));
				var eligibility = reconciler.Apply(spec, content.EligibilityRow(spell.Id)!, records, bindings, messages);
				using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray());
				var compiled = compiler.Compile(eligibility.Id);
				if (!compiled.ReturnType.CompatibleWith(MudSharp.FutureProg.ProgVariableTypes.Boolean) ||
					!compiled.MatchesParameters([content.TargetType ?? MudSharp.FutureProg.ProgVariableTypes.Character, MudSharp.FutureProg.ProgVariableTypes.Character]))
					throw new StockDependencyException(content.Key + ": restore the owned Boolean(target,caster) eligibility signature before casting.");
				filter = eligibility.Id;
			}
			var desired = content.SpellRow(school, trait, known);
			desired.EffectDurationExpressionId = duration.Id;
			desired.Definition = content.BuildDefinition(resource, cost.Id, filter).ToString();
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
		catch (StockDependencyException error)
		{
			messages.Add(error.Message);
			return Result(ArmageddonInstallStatus.Blocked);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Inspect and rerun from a fresh context. " : committing ?
				"Commit outcome unknown. Inspect and reconcile owned identities before retrying. " : "S4 contribution failed before commit. ") + error.Message);
			return Result(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed);
		}
	}

	private sealed class StockDependencyException(string message) : InvalidOperationException(message) { }

	private static ArmageddonCanonicalSpellContribution ResolveCreature(FuturemudDatabaseContext db, ArmageddonCreatureSpellContent creature)
	{
		var aiClaim = CatalogueClaim<ArtificialIntelligence>(db, "arm.ai." + creature.NpcFamily, false);
		var ai = db.ArtificialIntelligences.AsNoTracking().SingleOrDefault(x => x.Id == aiClaim.LogicalId);
		XElement controller;
		try { controller = XElement.Parse(ai?.Definition ?? ""); }
		catch (System.Xml.XmlException) { throw new StockDependencyException($"{creature.Key}: the exact owned controller definition is unreadable."); }
		if (ai?.Type != "SpellCompanion" || (string?)controller.Element("Purpose") != creature.ControllerPurpose ||
			(string?)controller.Element("Mode") != (creature.NpcFamily == "outsider" ? "outsider" : "companion"))
			throw new StockDependencyException($"{creature.Key}: restore the exact compatible owned native companion controller.");
		long? riding = null;
		if (creature.NpcFamily == "earth_mount")
		{
			var mount = CatalogueClaim<ArtificialIntelligence>(db, "arm.ai.earth_mount.riding", false);
			if (!db.ArtificialIntelligences.Any(x => x.Id == mount.LogicalId && x.Type == "Mount"))
				throw new StockDependencyException("Earth Mount requires its exact owned native riding AI.");
			riding = mount.LogicalId;
		}
		var templates = new Dictionary<int, long>();
		for (var grade = 1; grade <= 7; grade++)
		{
			var key = $"arm.npc.{creature.NpcFamily}.g{grade}";
			var record = CatalogueClaim<NpcTemplate>(db, key, true);
			var template = db.NpcTemplates.AsNoTracking().Include(x => x.EditableItem)
				.Include(x => x.NpctemplatesArtificalIntelligences).ThenInclude(x => x.Ai)
				.SingleOrDefault(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber);
			var controllers = template?.NpctemplatesArtificalIntelligences.Where(x => x.Ai?.Type == "SpellCompanion").ToArray();
			if (template is null || template.Type != "Simple" || template.UniqueName != key ||
				template.EditableItem?.RevisionStatus != (int)RevisionStatus.Current || controllers is not { Length: 1 } ||
				controllers[0].AiId != ai!.Id || riding is { } mountId && !template.NpctemplatesArtificalIntelligences.Any(x => x.AiId == mountId))
				throw new StockDependencyException($"{key}: its exact approved Simple revision and owned native controller links are required; preserve and resolve explicitly.");
			templates.Add(grade, template.Id);
		}
		return new(creature.NativeCapabilityKey, creature.Bind(templates));
	}

	private static string? StockIdentity(string definition)
	{
		try { return (string?)XElement.Parse(definition).Element("StockIdentity"); }
		catch (System.Xml.XmlException) { return null; }
	}
}
