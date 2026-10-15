extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Combat;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

public sealed record ArmageddonStockSupportProgBindings(long SiltTag, long ShallowsTag, long WaterPlane,
	long ShadowPlane, long UndeadMerit, long VoidAffinityMerit, long DefilerMerit, long FirePlane,
	long MagicalExceptionMerit, IReadOnlyList<long> AffinityMerits)
{
	internal void Validate()
	{
		ArgumentNullException.ThrowIfNull(AffinityMerits);
		if (new[] { SiltTag, ShallowsTag, WaterPlane, ShadowPlane, UndeadMerit, VoidAffinityMerit, DefilerMerit, FirePlane, MagicalExceptionMerit }.Any(x => x <= 0) ||
			AffinityMerits.Count != 7 || AffinityMerits.Any(x => x <= 0) || AffinityMerits.Distinct().Count() != 7 || !AffinityMerits.Contains(VoidAffinityMerit))
			throw new ArgumentOutOfRangeException(nameof(ArmageddonStockSupportProgBindings), "Every support binding must be an explicit positive native identity.");
	}
}

/// <summary>Supplemental typed supports, outside the approved 210-blueprint inventory. No live entities or relocation.</summary>
public static partial class ArmageddonStockSupportInstaller
{
	public const string Module = ArmageddonCatalogueOwnership.Module;
	public const string Version = ArmageddonCatalogueOwnership.Version;
	public const string WaterPlaneKey = "arm.support.plane.water";
	public const string ShadowPlaneKey = "arm.support.plane.shadow";
	public const string FirePlaneKey = "arm.support.plane.fire";
	public const string LightningPlaneKey = "arm.support.plane.lightning";
	public const string NilazPassageTemplateKey = "arm.support.roomtemplate.nilaz_passage";
	public const string SiltTagKey = "arm.support.tag.silt";
	public const string ShallowsTagKey = "arm.support.tag.shallows";
	public const string TravelRestrictedTagKey = "arm.support.tag.travel_restricted";
	public const string TravelPopulatedTagKey = "arm.support.tag.travel_populated";
	public const string UndeadMeritKey = "arm.support.merit.undead";
	public const string MagicalExceptionMeritKey = "arm.support.merit.magical_exception";
	public const string CorpseEligibilityKey = "arm.support.prog.corpse_eligibility";
	public const string WeaponEligibilityKey = "arm.support.prog.weapon_eligibility";
	public const string FlameMultiplierKey = "arm.support.prog.flame_multiplier";
	public const string ShadowMultiplierKey = "arm.support.prog.shadow_multiplier";
	public const string TargetEligibilityKey = "arm.support.prog.target_eligibility";
	public const string PerceptionEligibilityKey = "arm.support.prog.perception_eligibility";
	public const string LiquidEligibilityKey = "arm.support.prog.liquid_eligibility";
	public const string MagicalEligibilityKey = "arm.support.prog.magical_eligibility";
	public const string IdentifyTextKey = "arm.support.prog.identify_text";
	public const string ReciteLinkKey = "arm.support.prog.recite_link";
	public const string DeadSpeakLinkKey = "arm.support.prog.deadspeak_link";
	public const string DefilerMeritKey = "arm.merit.defiler";
	public const string VoidAffinityMeritKey = "arm.race.outsider.affinity";
	public static IReadOnlyList<string> AffinityMeritKeys { get; } = Array.AsReadOnly(new[]
	{
		"arm.race.earth_mount.affinity", "arm.race.clay_sentinel.affinity", "arm.race.air_guardian.affinity",
		"arm.race.seeking_wind.affinity", "arm.race.wind_courier.affinity", "arm.race.haunting_shape.affinity", VoidAffinityMeritKey
	});
	public static string FlameStaffKey(int variant) => Variant("arm.item.flame_knife.permanent_choice_", variant);
	public static string ShadowStaffKey(int variant) => Variant("arm.item.shadow_staff.g", variant);
	public static string SparkStaffKey(int variant) => Variant("arm.item.spark_staff.g", variant);
	private static string Variant(string prefix, int variant) => variant is >= 1 and <= 8
		? prefix + variant : throw new ArgumentOutOfRangeException(nameof(variant));

	public static IReadOnlyDictionary<string, FutureProg> ProgDefinitions(ArmageddonStockSupportProgBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings); bindings.Validate();
		var locationAllowed = $"not (istagged(@caster.location, tag({bindings.SiltTag})) or istagged(@caster.location.terrain, tag({bindings.SiltTag})) or istagged(@caster.location, tag({bindings.ShallowsTag})) or istagged(@caster.location.terrain, tag({bindings.ShallowsTag})))";
		var siltAllowed = $"not (istagged(@caster.location, tag({bindings.SiltTag})) or istagged(@caster.location.terrain, tag({bindings.SiltTag})))";
		return new Dictionary<string, FutureProg>(StringComparer.Ordinal)
		{
			[CorpseEligibilityKey] = Prog("arm_support_corpse_eligibility", ProgVariableTypes.Boolean,
				"return " + locationAllowed, ("caster", ProgVariableTypes.Character), ("corpse", ProgVariableTypes.Item)),
			[WeaponEligibilityKey] = Prog("arm_support_weapon_eligibility", ProgVariableTypes.Boolean,
				$"return currentplaneid(@caster) != {bindings.WaterPlane}", ("caster", ProgVariableTypes.Character)),
			[FlameMultiplierKey] = Prog("arm_support_flame_multiplier", ProgVariableTypes.Number,
				$"if (currentplaneid(@caster) == {bindings.ShadowPlane})\nreturn 0.5\nend if\nreturn 1", ("caster", ProgVariableTypes.Character)),
			[ShadowMultiplierKey] = Prog("arm_support_shadow_multiplier", ProgVariableTypes.Number,
				$"if (currentplaneid(@caster) == {bindings.ShadowPlane})\nreturn 2\nend if\nreturn 1", ("caster", ProgVariableTypes.Character)),
			[TargetEligibilityKey] = Prog("arm_support_target_eligibility", ProgVariableTypes.Boolean,
				$"return contains(@caster.applicablemerits, tomerit({bindings.DefilerMerit})) or not (contains(@target.applicablemerits, tomerit({bindings.UndeadMerit})) or contains(@target.applicablemerits, tomerit({bindings.VoidAffinityMerit})))",
				("target", ProgVariableTypes.Character), ("caster", ProgVariableTypes.Character)),
			[PerceptionEligibilityKey] = Prog("arm_support_perception_eligibility", ProgVariableTypes.Boolean,
				"return " + siltAllowed, ("target", ProgVariableTypes.Character), ("caster", ProgVariableTypes.Character)),
			[LiquidEligibilityKey] = Prog("arm_support_liquid_eligibility", ProgVariableTypes.Boolean,
				$"return currentplaneid(@caster) != {bindings.FirePlane} and {siltAllowed}", ("target", ProgVariableTypes.Item), ("caster", ProgVariableTypes.Character)),
			[IdentifyTextKey] = Prog("arm_support_identify_text", ProgVariableTypes.Text,
				string.Join("\n", bindings.AffinityMerits.Select((id, index) =>
					$"if (contains(@target.applicablemerits, tomerit({id})))\nreturn \"One explicitly recorded elemental affinity is {new[] { "Earth", "Earth", "Wind", "Wind", "Wind", "Shadow", "Void" }[index]}.\"\nend if")) +
					"\nreturn \"No package-recorded elemental affinity is revealed.\"", ("target", ProgVariableTypes.Character), ("caster", ProgVariableTypes.Character)),
			[ReciteLinkKey] = Prog("arm_support_recite_link", ProgVariableTypes.Character,
				"return @caster", ("caster", ProgVariableTypes.Character), ("proxy", ProgVariableTypes.Character)),
			[DeadSpeakLinkKey] = Prog("arm_support_deadspeak_link", ProgVariableTypes.Character,
				"return @caster", ("caster", ProgVariableTypes.Character), ("proxy", ProgVariableTypes.Character)),
			[MagicalEligibilityKey] = Prog("arm_support_magical_eligibility", ProgVariableTypes.Boolean,
				$"return hasmagictag(@target, \"armageddon.magical_creature\") or contains(@target.applicablemerits, tomerit({bindings.MagicalExceptionMerit})) or " +
				string.Join(" or ", bindings.AffinityMerits.Select(id => $"contains(@target.applicablemerits, tomerit({id}))")),
				("target", ProgVariableTypes.Character), ("caster", ProgVariableTypes.Character))
		};
	}

	private static FutureProg Prog(string name, ProgVariableTypes type, string text, params (string Name, ProgVariableTypes Type)[] parameters)
	{
		var prog = new FutureProg { FunctionName = name, FunctionText = text, ReturnTypeDefinition = type.ToStorageString(),
			FunctionComment = "Editable supplemental Armageddon stock policy; typed native identities, no entity mutation.",
			Category = "Magic", Subcategory = "Armageddon Supports", Public = false };
		foreach (var (parameter, index) in parameters.Select((value, index) => (value, index)))
			prog.FutureProgsParameters.Add(new() { ParameterIndex = index, ParameterName = parameter.Name, ParameterTypeDefinition = parameter.Type.ToStorageString() });
		return prog;
	}

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory(); var messages = new List<string>(); var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var settings = ArmageddonInstallation.Read(db);
		if (settings is null) return new(ArmageddonInstallStatus.Blocked, ["Prepare explicit Armageddon installation settings first."], ids);
		if (db.Database.CurrentTransaction is not null || db.ChangeTracker.HasChanges())
			return new(ArmageddonInstallStatus.Blocked, ["Use a fresh dedicated context without pending writes or an existing transaction."], ids);
		var committing = false; var committed = false; var preflightComplete = false;
		try
		{
			messages.AddRange(ArmageddonInstallation.ValidateFoundations(db, settings));
			if (messages.Count != 0) return new(ArmageddonInstallStatus.Blocked, messages.AsReadOnly(), ids);
			var holdable = Require<GameItemComponentProto>(db, "arm.catalogue.component.holdable", x => x.Type == "Holdable");
			var defiler = Require<Merit>(db, DefilerMeritKey, x => x.MeritScope == 2 && x.Type == "Magic Capability");
			var affinities = AffinityMeritKeys.ToDictionary(key => key, key => Require<Merit>(db, key,
				x => x.MeritScope == 2 && x.Type == "Elemental Affinity", ArmageddonCatalogueOwnership.Module), StringComparer.Ordinal);
			var aliasRecord = db.SeederManagedRecords.Single(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == "arm.binding.weapon.staff");
			if (!ArmageddonCatalogueLinks.Valid(db, aliasRecord) || aliasRecord.EntityType != ArmageddonCatalogueLinks.AliasType)
				throw new InvalidOperationException("The typed native Club staff foundation is missing or invalid.");
			var alias = JsonSerializer.Deserialize<ArmageddonCatalogueAlias>(aliasRecord.SeedBaseline!)!;
			if (alias.NativeType != nameof(WeaponType)) throw new InvalidOperationException("Staff foundation has an incompatible native alias type.");
			var foundation = db.WeaponTypes.Single(x => x.Id == alias.NativeId);
			if (foundation.AttackTraitId is null || foundation.ParryTraitId is null ||
				!db.TraitDefinitions.Any(x => x.Id == foundation.AttackTraitId && x.Type == 0 && x.OwnerScope == 0) ||
				!db.TraitDefinitions.Any(x => x.Id == foundation.ParryTraitId && x.Type == 0 && x.OwnerScope == 0))
				throw new InvalidOperationException("The retained Club foundation must have valid native attack and parry skills.");
			var catalogue = new ArmageddonStockCatalogue();
			var protection = ProtectionPreflight(db, settings);
			for (var variant = 1; variant <= 8; variant++)
			{
				var key = FlameStaffKey(variant);
				if (!catalogue.ByKey.TryGetValue(key, out var blueprint) || blueprint.Kind != ArmageddonBlueprintKind.MeleeItem || blueprint.Configuration.GetProperty("shape").GetString() != "staff")
					throw new InvalidOperationException($"{key}: approved staff blueprint is required; blade/whip substitution is refused.");
				var item = Require<GameItemProto>(db, key, _ => true);
				var component = Require<GameItemComponentProto>(db, key + ".component", x => x.Type == "MeleeWeapon");
				var weapon = Require<WeaponType>(db, key + ".weapon", x => x.AttackTraitId == foundation.AttackTraitId && x.ParryTraitId == foundation.ParryTraitId);
				if ((long?)XElement.Parse(component.Definition).Element("WeaponType") != weapon.Id ||
					!db.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevision == item.RevisionNumber && x.GameItemComponentProtoId == component.Id && x.GameItemComponentRevision == component.RevisionNumber) ||
					!db.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemProtoId == item.Id && x.GameItemProtoRevision == item.RevisionNumber && x.GameItemComponentProtoId == holdable.Id && x.GameItemComponentRevision == holdable.RevisionNumber))
					throw new InvalidOperationException($"{key}: approved Holdable/MeleeWeapon staff graph is incomplete.");
				ids.Add(key, item.Id);
			}
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			preflightComplete = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			InstallProtection(owned, protection, ids, messages);
			InstallTravelSupports(db, owned, protection, ids, messages);
			foreach (var (key, name, order) in new[] { (WaterPlaneKey, "Water", 10), (ShadowPlaneKey, "Shadow", 20),
				(FirePlaneKey, "Fire", 30), (LightningPlaneKey, "Lightning", 40) })
			{
				var plane = owned.Put(key, new Plane { Name = "Armageddon " + name, Alias = "armageddon" + name.ToLowerInvariant(),
					Description = "An inert editable stock " + name.ToLowerInvariant() + " plane marker. Installation assigns no entities or rooms.",
					RoomDescriptionAddendum = "", RoomNameFormat = "{0}", RemoteObservationTag = "", DisplayOrder = order, IsDefault = false }, []);
				if (plane.IsDefault) throw new InvalidOperationException($"{key}: a builder made the support plane default; resolve explicitly before reuse.");
				ids.Add(key, plane.Id);
			}
			var passageTerrain = ArmageddonCatalogueLinks.Bind(db, "arm.binding.terrain.sheltered", nameof(Terrain), () =>
			{
				var candidates = db.Terrains.Where(x => x.Name == "Hallway").ToArray();
				if (candidates.Length != 1) throw new InvalidOperationException("Exactly one CoreDataSeeder Hallway terrain is required for the Nilaz passage.");
				return candidates[0].Id;
			});
			var passageDefinition = new XElement("Template", new XElement("ApplyAutobuilderTagsAsFrameworkTags", false),
				new XElement("ShowCommandByline", new XCData("An Armageddon Nilaz passage template")),
				new XElement("Default", new XElement("Terrain", new XElement("DefaultTerrain", passageTerrain),
					new XElement("RoomName", new XCData("A Grey, Bleak Hallway")),
					new XElement("RoomDescription", new XCData("Grey, rough stone encloses a lifeless passage. The air is stale, and the hall continues north and south.")),
					new XElement("OutdoorsType", (int)RoomOutdoorsType.IndoorsNoLight),
					new XElement("CellLightMultiplier", 1), new XElement("ForagableProfile", 0))), new XElement("Terrains"));
			ids.Add(NilazPassageTemplateKey, owned.Put(NilazPassageTemplateKey,
				new AutobuilderRoomTemplate { Name = "Armageddon Nilaz Passage", TemplateType = "room by terrain",
					Definition = passageDefinition.ToString() }, ["arm.binding.terrain.sheltered"]).Id);
			foreach (var (key, name) in new[] { (SiltTagKey, "Silt"), (ShallowsTagKey, "Shallows") })
				ids.Add(key, owned.Put(key, new Tag { Name = "Armageddon " + name, ShouldSeeProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg] }, []).Id);
			foreach (var (key, name) in new[] { (UndeadMeritKey, "undead marker"), (MagicalExceptionMeritKey, "magical exception marker") })
				ids.Add(key, owned.Put(key, new Merit { Name = "Armageddon " + name, Type = "Simple Character", MeritScope = 2, MeritType = 0,
					Definition = new XElement("Merit", new XElement("ChargenAvailableProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
						new XElement("ApplicabilityProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]), new XElement("ChargenBlurb", "Explicit builder attachment only."),
						new XElement("DescriptionText", "@ have|has an explicitly declared " + name + ".")).ToString() }, []).Id);
			var bindings = new ArmageddonStockSupportProgBindings(ids[SiltTagKey], ids[ShallowsTagKey], ids[WaterPlaneKey], ids[ShadowPlaneKey], ids[UndeadMeritKey],
				affinities[VoidAffinityMeritKey].Id, defiler.Id, ids[FirePlaneKey], ids[MagicalExceptionMeritKey], AffinityMeritKeys.Select(key => affinities[key].Id).ToArray());
			foreach (var (key, desired) in ProgDefinitions(bindings))
				ids.Add(key, owned.Put(key, desired, key switch
				{
					IdentifyTextKey => AffinityMeritKeys.ToArray(), ReciteLinkKey or DeadSpeakLinkKey => [],
					TargetEligibilityKey => [UndeadMeritKey, VoidAffinityMeritKey, DefilerMeritKey],
					CorpseEligibilityKey => [SiltTagKey, ShallowsTagKey], WeaponEligibilityKey => [WaterPlaneKey],
					PerceptionEligibilityKey => [SiltTagKey], LiquidEligibilityKey => [FirePlaneKey, SiltTagKey],
					MagicalEligibilityKey => AffinityMeritKeys.Prepend(MagicalExceptionMeritKey).ToArray(), _ => [ShadowPlaneKey]
				}).Id);
			var installedProgs = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray();
			using (var compiler = new OfflineProgCompilation(installedProgs))
				foreach (var (key, desired) in ProgDefinitions(bindings))
				{
					var installed = installedProgs.Single(x => x.Id == ids[key]);
					var expected = desired.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => (x.ParameterName, x.ParameterTypeDefinition)).ToArray();
					if (installed.AcceptsAnyParameters || !installed.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
						.Select(x => (x.ParameterName, x.ParameterTypeDefinition)).SequenceEqual(expected))
						throw new InvalidOperationException($"{key}: retained policy prog requires its exact typed parameter contract.");
					var compiled = compiler.Compile(ids[key]);
					if (compiled.ReturnType != ProgVariableTypes.FromStorageString(desired.ReturnTypeDefinition) || !compiled.MatchesParameters(desired.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition)).ToArray()))
						throw new InvalidOperationException($"{key}: retained policy prog has incompatible native return/parameter types.");
				}
			var config = catalogue.ByKey[FlameStaffKey(8)].Configuration;
			for (var variant = 1; variant <= 8; variant++)
			{
				ids.Add(ShadowStaffKey(variant), Staff(db, owned, settings, foundation, holdable, config, ShadowStaffKey(variant), "Shadow Staff " + variant, DamageType.Crushing));
				ids.Add(SparkStaffKey(variant), Staff(db, owned, settings, foundation, holdable, config, SparkStaffKey(variant), "Spark Staff " + variant, DamageType.Electrical));
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated); db.SaveChanges(); checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true; transaction?.Commit(); committed = true; checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("Supplemental owned supports committed: 16 native staff graphs, eight exact S3 staff references, inert markers, eight typed editable policy progs and one deterministic Nilaz passage template. New staff numerics reuse authored grade-seven S3 staff defaults; Crushing/Electrical kinds are editable authored choices. No live entities, temporary rooms or plane assignments changed; installed-world qualification remains separate.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown; reconnect before rerunning. " :
				!preflightComplete ? "Support preflight blocked before writes. " : "Support batch failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown :
				!preflightComplete ? ArmageddonInstallStatus.Blocked : ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}

	private static T Require<T>(FuturemudDatabaseContext db, string key, Func<T, bool> compatible, string? requiredModule = null) where T : class
	{
		var record = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		var revisioned = db.Model.FindEntityType(typeof(T))!.FindPrimaryKey()!.Properties.Count == 2;
		var module = requiredModule ?? (typeof(T) == typeof(Merit) ? ArmageddonProgressionInstaller.Module : ArmageddonCatalogueOwnership.Module);
		var version = module == ArmageddonProgressionInstaller.Module ? ArmageddonProgressionInstaller.Version : ArmageddonCatalogueOwnership.Version;
		if (record is null || record.Retired || record.EntityType != typeof(T).Name || record.LogicalId is not > 0 ||
			record.Module != module || record.ManifestVersion != version || revisioned != (record.RevisionNumber is not null) || record.SeedBaseline is null ||
			db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
			throw new InvalidOperationException($"{key}: exact typed stock ownership is missing or incompatible.");
		var row = (T?)db.Find(typeof(T), revisioned ? [record.LogicalId.Value, record.RevisionNumber!.Value] : [record.LogicalId.Value]);
		if (row is null || !compatible(row)) throw new InvalidOperationException($"{key}: retained native support is missing or incompatible.");
		if (revisioned)
		{
			var editable = EFPropertyEditable(db, row);
			if (!db.EditableItems.Any(x => x.Id == editable && x.RevisionStatus == (int)RevisionStatus.Current))
				throw new InvalidOperationException($"{key}: retained native revision is not current/approved.");
		}
		return row;
	}
	private static long EFPropertyEditable(FuturemudDatabaseContext db, object row) => (long)db.Entry(row).Property("EditableItemId").CurrentValue!;

	private static long Staff(FuturemudDatabaseContext db, ArmageddonCatalogueOwnership owned, ArmageddonInstallationSettings settings,
		WeaponType foundation, GameItemComponentProto holdable, JsonElement config, string key, string name, DamageType damageType)
	{
		var weapon = owned.Put(key + ".weapon", new WeaponType { Name = "Armageddon " + name, Classification = (int)WeaponClassification.Lethal,
			AttackTraitId = foundation.AttackTraitId, ParryTraitId = foundation.ParryTraitId, ParryBonus = foundation.ParryBonus, Reach = 2, StaminaPerParry = foundation.StaminaPerParry }, ["arm.binding.weapon.staff"]);
		TraitExpression Expression(string suffix, string field) => owned.Put(key + ".attack." + suffix, new TraitExpression
		{ Name = "Armageddon " + name + " " + suffix, Expression = config.GetProperty(field).GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture) }, []);
		var damage = Expression("damage", "native_attack_damage"); var pain = Expression("pain", "native_attack_pain"); var stun = Expression("stun", "native_attack_stun");
		var attack = owned.Put(key + ".attack", new WeaponAttack { Name = "Armageddon " + name + " strike", WeaponTypeId = weapon.Id,
			Verb = (int)MeleeWeaponVerb.Swing, MoveType = (int)BuiltInCombatMoveType.UseWeaponAttack, BaseAttackerDifficulty = (int)Difficulty.Normal,
			BaseBlockDifficulty = (int)Difficulty.Normal, BaseDodgeDifficulty = (int)Difficulty.Normal, BaseParryDifficulty = (int)Difficulty.Normal,
			BaseAngleOfIncidence = Math.PI * 0.5, RecoveryDifficultySuccess = (int)Difficulty.Easy, RecoveryDifficultyFailure = (int)Difficulty.Hard,
			Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill), ExertionLevel = (int)ExertionLevel.Heavy,
			DamageType = (int)damageType, DamageExpressionId = damage.Id, PainExpressionId = pain.Id, StunExpressionId = stun.Id,
			Weighting = 100, MaximumTargets = 1, StaminaCost = config.GetProperty("attack_stamina").GetDouble(), BaseDelay = 1,
			Orientation = (int)Orientation.Centre, Alignment = (int)Alignment.Front, HandednessOptions = (int)AttackHandednessOptions.Any, RequiredPositionStateIds = "1 16 17 18" },
			[key + ".weapon", key + ".attack.damage", key + ".attack.pain", key + ".attack.stun"]);
		var message = new CombatMessage { Type = (int)BuiltInCombatMoveType.UseWeaponAttack, Verb = (int)MeleeWeaponVerb.Swing, Priority = 50,
			Chance = 1, Message = "@ strike|strikes at $1 with $2", FailureMessage = "@ strike|strikes at $1 with $2" };
		message.CombatMessagesWeaponAttacks.Add(new() { WeaponAttackId = attack.Id }); owned.Put(key + ".attack.message", message, [key + ".attack"]);
		var component = owned.Put(key + ".component", new GameItemComponentProto { Id = owned.Id<GameItemComponentProto>(key + ".component"),
			Name = "Armageddon " + name + " melee", Type = "MeleeWeapon", Description = "Native supplemental staff component.", EditableItem = owned.Approval(),
			Definition = new XElement("Definition", new XElement("WeaponType", weapon.Id), new XElement("CanWieldProg", 0), new XElement("WhyCannotWieldProg", 0)).ToString() }, [key + ".weapon"]);
		var item = new GameItemProto { Id = owned.Id<GameItemProto>(key), Name = "Armageddon " + name, Keywords = "armageddon " + name.ToLowerInvariant(),
			ShortDescription = "a conjured " + name.Split(' ')[0].ToLowerInvariant() + " staff", FullDescription = "This magical staff is fashioned for native staff strikes and parries.",
			MaterialId = settings.NativeBindings[ArmageddonNativeBinding.Wood], Size = (int)SizeCategory.Normal,
			Weight = config.GetProperty("weight_kilograms").GetDouble() * db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier,
			BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = owned.Approval() };
		item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = holdable.Id, GameItemComponentRevision = holdable.RevisionNumber });
		item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
		return owned.Put(key, item, ["arm.catalogue.component.holdable", key + ".component"]).Id;
	}
}
