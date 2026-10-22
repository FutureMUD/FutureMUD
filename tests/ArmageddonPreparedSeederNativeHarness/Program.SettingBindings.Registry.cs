// Reviewed concrete binding dispositions for Program.SettingBindings.cs.
// Source-reviewed registry, not a claim that native execution has passed. No DB writes.
#nullable enable
using System.Globalization;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using DatabaseSeeder.Seeders;
using RuntimeExpression = MudSharp.Body.Traits.TraitExpression;
namespace FutureMUD.GatheringNativePersistenceHarness;
internal static partial class GNHProgram
{
	private static IReadOnlyDictionary<string, BindingProof> ReviewedStaticProofs() =>
		new Dictionary<string, BindingProof>(StringComparer.Ordinal)
		{
			["AppraiseCommandSkill"] = FixedTraitStatic("AppraiseCommandSkill", "arm.skill.value", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:2"),
			["ArmageddonHostileGuardAttemptsEnabled"] = ExactStatic("ArmageddonHostileGuardAttemptsEnabled", "true", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:3; fresh-world opt-in after binding"),
			["BlindFightingEnabled"] = ExactStatic("BlindFightingEnabled", "true", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:4; fresh-world opt-in after binding"),
			["BlindFightingMinimumSightDifficulty"] = ExactStatic("BlindFightingMinimumSightDifficulty", "6", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:5; retained native tuning"),
			["BlindFightingSightExtremelyHardDifficulty"] = ExactStatic("BlindFightingSightExtremelyHardDifficulty", "6", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:6; retained native tuning"),
			["BlindFightingSightHardDifficulty"] = ExactStatic("BlindFightingSightHardDifficulty", "4", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:7; retained native tuning"),
			["BlindFightingSightInsaneDifficulty"] = ExactStatic("BlindFightingSightInsaneDifficulty", "7", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:8; retained native tuning"),
			["BlindFightingSightVeryHardDifficulty"] = ExactStatic("BlindFightingSightVeryHardDifficulty", "5", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:9; retained native tuning"),
			["BlindFightingUnseenDifficulty"] = ExactStatic("BlindFightingUnseenDifficulty", "8", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:10; retained native tuning"),
			["DefaultAuxiliaryMoveTraitId"] = ScaffoldStatic("DefaultAuxiliaryMoveTraitId", "arm.scaffold.auxiliary", null, "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:11"),
			["DefaultDragWeightExpression"] = StrengthFormulaStatic("DefaultDragWeightExpression", 2500000, "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:12"),
			["DefaultLiftWeightExpression"] = StrengthFormulaStatic("DefaultLiftWeightExpression", 1000000, "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:13"),
			["DefaultSkillDecorator"] = TypedDefaultStatic("DefaultSkillDecorator", true, "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:14"),
			["DefaultSkillImprover"] = TypedDefaultStatic("DefaultSkillImprover", false, "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:15"),
			["DefaultWeaponAttackDamageExpressionId"] = ExpressionIdStatic("DefaultWeaponAttackDamageExpressionId", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:16"),
			["DefaultWeaponAttackPainExpressionId"] = ExpressionIdStatic("DefaultWeaponAttackPainExpressionId", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:17"),
			["DefaultWeaponAttackStunExpressionId"] = ExpressionIdStatic("DefaultWeaponAttackStunExpressionId", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:18"),
			["DirectionSenseDifficulty"] = ExactStatic("DirectionSenseDifficulty", "Normal", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:19; retained native tuning"),
			["DirectionSenseEnabled"] = ExactStatic("DirectionSenseEnabled", "true", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:20; fresh-world opt-in after binding"),
			["DirectionSenseMaximumObscuringWeatherLightMultiplier"] = ExactStatic("DirectionSenseMaximumObscuringWeatherLightMultiplier", "0.3", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:21; retained native tuning"),
			["DirectionSenseMinimumSightDifficulty"] = ExactStatic("DirectionSenseMinimumSightDifficulty", "VeryHard", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:22; retained native tuning"),
			["DirectionSenseNoProgressDelaySeconds"] = ExactStatic("DirectionSenseNoProgressDelaySeconds", "10", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:23; retained native tuning"),
			["DirectionSenseObscurationProg"] = ExactStatic("DirectionSenseObscurationProg", "0", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:24; native light/weather fallback is supported for zero"),
			["DirectionSenseRecoveryDifficulty"] = ExactStatic("DirectionSenseRecoveryDifficulty", "Normal", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:25; retained native tuning"),
			["DrawingTraitId"] = ScaffoldStatic("DrawingTraitId", "arm.scaffold.drawing", null, "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:26"),
			["DualWieldCoordinationDifficulty"] = ExactStatic("DualWieldCoordinationDifficulty", "5", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:27; retained native tuning"),
			["DualWieldCoordinationFailStages"] = ExactStatic("DualWieldCoordinationFailStages", "1", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:28; retained native tuning"),
			["DualWieldCoordinationMajorFailStages"] = ExactStatic("DualWieldCoordinationMajorFailStages", "2", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:29; retained native tuning"),
			["DualWieldCoordinationMajorPassStages"] = ExactStatic("DualWieldCoordinationMajorPassStages", "-1", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:30; retained native tuning"),
			["DualWieldCoordinationMinorFailStages"] = ExactStatic("DualWieldCoordinationMinorFailStages", "1", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:31; retained native tuning"),
			["DualWieldCoordinationMinorPassStages"] = ExactStatic("DualWieldCoordinationMinorPassStages", "0", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:32; retained native tuning"),
			["DualWieldCoordinationPassStages"] = ExactStatic("DualWieldCoordinationPassStages", "0", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:33; retained native tuning"),
			["DualWieldDoubleAttackRecoveryMultiplier"] = ExactStatic("DualWieldDoubleAttackRecoveryMultiplier", "1.5", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:34; retained native tuning"),
			["DualWieldDoubleAttackWeightMultiplier"] = ExactStatic("DualWieldDoubleAttackWeightMultiplier", "0.25", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:35; retained native tuning"),
			["DualWieldDoubleAttacksEnabled"] = ExactStatic("DualWieldDoubleAttacksEnabled", "true", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:36; fresh-world opt-in after binding"),
			["ElectricalConfigureTraitName"] = ScaffoldStatic("ElectricalConfigureTraitName", "arm.scaffold.electronics", "ElectricalCommandEnabled", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:37"),
			["ElectricalInstallTraitName"] = ScaffoldStatic("ElectricalInstallTraitName", "arm.scaffold.electrician", "ElectricalCommandEnabled", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:38"),
			["ForgerySkillId"] = FixedTraitStatic("ForgerySkillId", "arm.skill.forgery", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:39"),
			["HaggleActionDurationSeconds"] = ExactStatic("HaggleActionDurationSeconds", "20", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:40; retained native tuning"),
			["HaggleDefaultDifficulty"] = ExactStatic("HaggleDefaultDifficulty", "Normal", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:41; retained native tuning"),
			["HaggleMinimumSpread"] = ExactStatic("HaggleMinimumSpread", "0.05", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:42; retained native tuning"),
			["HaggleNegotiationScope"] = ExactStatic("HaggleNegotiationScope", "Shared", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:43; retained native tuning"),
			["HaggleTraitId"] = FixedTraitStatic("HaggleTraitId", "arm.skill.haggle", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:44"),
			["HaggleWindowDurationHours"] = ExactStatic("HaggleWindowDurationHours", "6", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:45; retained native tuning"),
			["HagglingEnabled"] = ExactStatic("HagglingEnabled", "true", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:46; fresh-world opt-in after binding"),
			["HandwritingSkillId"] = FixedTraitStatic("HandwritingSkillId", "arm.skill.handwriting", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:47"),
			["LiteracySkillId"] = FixedTraitStatic("LiteracySkillId", "arm.skill.literacy", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:48"),
			["PeekCarriedItemDifficulty"] = ExactStatic("PeekCarriedItemDifficulty", "5", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:49; retained native tuning"),
			["PeekContainerItemDifficulty"] = ExactStatic("PeekContainerItemDifficulty", "5", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:50; retained native tuning"),
			["PeekCoveredItemDifficulty"] = ExactStatic("PeekCoveredItemDifficulty", "6", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:51; retained native tuning"),
			["ProgrammingTraitName"] = ScaffoldStatic("ProgrammingTraitName", "arm.scaffold.programming", "ProgrammingCommandEnabled", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:52"),
			["RecoveryTimeExpressionId"] = ExactStatic("RecoveryTimeExpressionId", "0", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:53; CombatBase.SetupCombat explicit max(1,25-recovery) fallback"),
			["SkinningTraitId"] = FixedTraitStatic("SkinningTraitId", "arm.skill.skinning", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:54"),
			["StreetwiseSkillExpression"] = ExactStatic("StreetwiseSkillExpression", "0", "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:55; constant formula, not a trait-ID pointer"),
			["TattooistTrait"] = ScaffoldStatic("TattooistTrait", "arm.scaffold.tattoo", null, "MUNDANE_STATIC_DEPENDENCIES_20261018.tsv:56"),
		};

	private static IReadOnlyDictionary<string, BindingProof> ReviewedDynamicProofs() =>
		new Dictionary<string, BindingProof>(StringComparer.Ordinal)
		{
			["arm.skill.bludgeoning_weapons|weapon attack traits"] = WeaponFamily("arm.skill.bludgeoning_weapons", "Club", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:2"),
			["arm.skill.chopping_weapons|weapon attack traits"] = WeaponFamily("arm.skill.chopping_weapons", "Axe", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:3"),
			["arm.skill.knife_weapons|weapon attack traits"] = WeaponFamily("arm.skill.knife_weapons", "Knife", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:4"),
			["arm.skill.piercing_weapons|weapon attack traits"] = WeaponFamily("arm.skill.piercing_weapons", "Short Spear", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:5"),
			["arm.skill.pike_weapons|weapon attack traits"] = WeaponFamily("arm.skill.pike_weapons", "Pike", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:6"),
			["arm.skill.polearm_weapons|weapon attack traits"] = WeaponFamily("arm.skill.polearm_weapons", "Halberd", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:7"),
			["arm.skill.razor_weapons|weapon attack traits"] = WeaponFamily("arm.skill.razor_weapons", "Razor Weapon", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:8"),
			["arm.skill.slashing_weapons|weapon attack traits"] = WeaponFamily("arm.skill.slashing_weapons", "Longsword", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:9"),
			["arm.skill.trident_weapons|weapon attack traits"] = WeaponFamily("arm.skill.trident_weapons", "Trident", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:10"),
			["arm.skill.two_handed|weapon attack traits"] = WeaponFamily("arm.skill.two_handed", "Two Handed Sword", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:11"),
			["arm.skill.archery|ranged weapon trait"] = RangedFamily("arm.skill.archery", MudSharp.Combat.RangedWeaponType.Bow, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:12"),
			["arm.skill.throw|throwing trait"] = RangedFamily("arm.skill.throw", MudSharp.Combat.RangedWeaponType.Thrown, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:13"),
			["arm.skill.armor_making|craft recipe trait"] = AbsentSourceRecipes("arm.skill.armor_making", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:14"),
			["arm.skill.armor_repair|craft recipe trait"] = AbsentSourceRecipes("arm.skill.armor_repair", false, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:15"),
			["arm.skill.axe_making|craft recipe trait"] = AbsentSourceRecipes("arm.skill.axe_making", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:16"),
			["arm.skill.bandage_making|craft recipe trait"] = AbsentSourceRecipes("arm.skill.bandage_making", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:17"),
			["arm.skill.basketweaving|craft recipe trait"] = AbsentSourceRecipes("arm.skill.basketweaving", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:18"),
			["arm.skill.bow_making|craft recipe trait"] = AbsentSourceRecipes("arm.skill.bow_making", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:19"),
			["arm.skill.brew|craft recipe trait"] = AbsentSourceRecipes("arm.skill.brew", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:20"),
			["arm.skill.cloth_working|craft recipe trait"] = AbsentSourceRecipes("arm.skill.cloth_working", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:21"),
			["arm.skill.club_making|craft recipe trait"] = AbsentSourceRecipes("arm.skill.club_making", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:22"),
			["arm.skill.cooking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.cooking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:23"),
			["arm.skill.dyeing|craft recipe trait"] = AbsentSourceRecipes("arm.skill.dyeing", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:24"),
			["arm.skill.feather_working|craft recipe trait"] = AbsentSourceRecipes("arm.skill.feather_working", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:25"),
			["arm.skill.fletchery|craft recipe trait"] = AbsentSourceRecipes("arm.skill.fletchery", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:26"),
			["arm.skill.floristry|craft recipe trait"] = AbsentSourceRecipes("arm.skill.floristry", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:27"),
			["arm.skill.instrument_making|craft recipe trait"] = AbsentSourceRecipes("arm.skill.instrument_making", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:28"),
			["arm.skill.jewelrymaking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.jewelrymaking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:29"),
			["arm.skill.knife_making|craft recipe trait"] = AbsentSourceRecipes("arm.skill.knife_making", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:30"),
			["arm.skill.leather_working|craft recipe trait"] = AbsentSourceRecipes("arm.skill.leather_working", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:31"),
			["arm.skill.lumberjacking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.lumberjacking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:32"),
			["arm.skill.pick_making|craft recipe trait"] = AbsentSourceRecipes("arm.skill.pick_making", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:33"),
			["arm.skill.ropemaking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.ropemaking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:34"),
			["arm.skill.spearmaking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.spearmaking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:35"),
			["arm.skill.stone_working|craft recipe trait"] = AbsentSourceRecipes("arm.skill.stone_working", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:36"),
			["arm.skill.stonecrafting|craft recipe trait"] = AbsentSourceRecipes("arm.skill.stonecrafting", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:37"),
			["arm.skill.swordmaking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.swordmaking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:38"),
			["arm.skill.tanning|craft recipe trait"] = AbsentSourceRecipes("arm.skill.tanning", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:39"),
			["arm.skill.tentmaking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.tentmaking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:40"),
			["arm.skill.toolmaking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.toolmaking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:41"),
			["arm.skill.wagonmaking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.wagonmaking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:42"),
			["arm.skill.woodworking|craft recipe trait"] = AbsentSourceRecipes("arm.skill.woodworking", true, "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:43"),
			["arm.skill.armor_repair|repair trait"] = BoundArmorRepair("MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:44"),
			["arm.skill.trap|trap trait"] = AbsentOptionalTraps("MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:45"),
			["arm.skill.allspeak|seeded power verb allspeak / model allspeak"] = AbsentSourcePower("arm.skill.allspeak", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:46"),
			["arm.skill.babble|seeded power verb babble / model babble"] = AbsentSourcePower("arm.skill.babble", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:47"),
			["arm.skill.barrier|seeded power verb barrier / model mindbarrier"] = AbsentSourcePower("arm.skill.barrier", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:48"),
			["arm.skill.clairaudience|seeded power verb clairaudience / model clairaudience"] = AbsentSourcePower("arm.skill.clairaudience", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:49"),
			["arm.skill.compel|unresolved: no meaningful stock power route verified"] = AbsentSourcePower("arm.skill.compel", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:50"),
			["arm.skill.conceal|adjacent effect MindConcealPower (not stock seeded)"] = AbsentSourcePower("arm.skill.conceal", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:51"),
			["arm.skill.contact|seeded power verb contact / model connectmind"] = AbsentSourcePower("arm.skill.contact", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:52"),
			["arm.skill.control|adjacent effect possess (spellbacked)"] = AbsentSourcePower("arm.skill.control", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:53"),
			["arm.skill.disorient|adjacent effect hex"] = AbsentSourcePower("arm.skill.disorient", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:54"),
			["arm.skill.dome|unresolved: no meaningful stock power route verified"] = AbsentSourcePower("arm.skill.dome", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:55"),
			["arm.skill.empathy|seeded power verb empathy / model empathy"] = AbsentSourcePower("arm.skill.empathy", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:56"),
			["arm.skill.expel|seeded power verb expel / model mindexpel"] = AbsentSourcePower("arm.skill.expel", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:57"),
			["arm.skill.hear|seeded power verb hear / model hear"] = AbsentSourcePower("arm.skill.hear", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:58"),
			["arm.skill.illusion|seeded power verb illusion / model spellbacked"] = AbsentSourcePower("arm.skill.illusion", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:59"),
			["arm.skill.locate|unresolved: no meaningful stock power route verified"] = AbsentSourcePower("arm.skill.locate", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:60"),
			["arm.skill.masquerade|adjacent effect illusion/MindConcealPower"] = AbsentSourcePower("arm.skill.masquerade", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:61"),
			["arm.skill.mindblast|adjacent effect psychicbolt"] = AbsentSourcePower("arm.skill.mindblast", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:62"),
			["arm.skill.mindwipe|adjacent effect forgetting"] = AbsentSourcePower("arm.skill.mindwipe", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:63"),
			["arm.skill.probe|adjacent effect MindLookPower (not stock seeded)"] = AbsentSourcePower("arm.skill.probe", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:64"),
			["arm.skill.project|seeded power verb project / model spellbacked"] = AbsentSourcePower("arm.skill.project", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:65"),
			["arm.skill.rejuvenate|unresolved: no meaningful stock power route verified"] = AbsentSourcePower("arm.skill.rejuvenate", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:66"),
			["arm.skill.sense_presence|adjacent effect audit"] = AbsentSourcePower("arm.skill.sense_presence", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:67"),
			["arm.skill.shadowwalk|unresolved: no meaningful stock power route verified"] = AbsentSourcePower("arm.skill.shadowwalk", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:68"),
			["arm.skill.suggest|seeded power verb suggest / model suggest"] = AbsentSourcePower("arm.skill.suggest", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:69"),
			["arm.skill.thoughtsense|adjacent effect hear"] = AbsentSourcePower("arm.skill.thoughtsense", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:70"),
			["arm.skill.trace|seeded power verb trace / model trace"] = AbsentSourcePower("arm.skill.trace", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:71"),
			["arm.skill.vanish|adjacent effect attentionsuppression"] = AbsentSourcePower("arm.skill.vanish", "MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv:72"),
		};

	private static BindingProof ExactStatic(string key, string expected, string evidence) =>
		new("bound", evidence, db =>
		{
			var row = db.StaticConfigurations.AsNoTracking().Single(x => x.SettingName == key);
			Require(row.Definition == expected, $"Fresh static drift: {key}; expected {expected}; observed {row.Definition}.");
			return new[] { $"StaticConfigurations[{key}]={row.Definition}" };
		});

	private static BindingProof TypedDefaultStatic(string key, bool decorator, string evidence) =>
		new("bound", evidence, db =>
		{
			var text = db.StaticConfigurations.AsNoTracking().Single(x => x.SettingName == key).Definition;
			Require(long.TryParse(text, out var id) && id > 0 &&
				(decorator ? db.TraitDecorators.Any(x => x.Id == id) : db.Improvers.Any(x => x.Id == id)), "Dangling typed default: " + key);
			return new[] { $"{key}={id}; table={(decorator ? "TraitDecorators" : "Improvers")}" };
		});

	private static BindingProof ExpressionIdStatic(string key, string evidence) =>
		new("bound", evidence, db =>
		{
			var text = db.StaticConfigurations.AsNoTracking().Single(x => x.SettingName == key).Definition;
			Require(long.TryParse(text, out var id) && id > 0 && db.TraitExpressions.Any(x => x.Id == id), "Dangling expression static: " + key);
			var expression = db.TraitExpressions.AsNoTracking().Single(x => x.Id == id);
			AuditFormulaReferences(db, expression.Expression);
			foreach (var parameter in db.TraitExpressionParameters.AsNoTracking().Where(x => x.TraitExpressionId == id).ToArray())
				Require(db.TraitDefinitions.Any(x => x.Id == parameter.TraitDefinitionId), "Dangling expression parameter: " + key);
			return new[] { $"{key}={id}; formula={expression.Expression}" };
		});

	private static void AuditFormulaReferences(FuturemudDatabaseContext db, string formula)
	{
		foreach (System.Text.RegularExpressions.Match match in RuntimeExpression.TraitFormulaRegex.Matches(formula))
		{
			var id = long.Parse(match.Groups["reference"].Value, CultureInfo.InvariantCulture);
			Require(db.TraitDefinitions.Any(x => x.Id == id), "Dangling static formula trait: " + id);
		}
	}

	private static BindingProof StrengthFormulaStatic(string key, int multiplier, string evidence) =>
		new("bound", evidence, db =>
		{
			var strength = db.TraitDefinitions.AsNoTracking().Where(x => x.Type == 1).AsEnumerable()
				.Single(x => x.Name is "Strength" or "Physique" or "Body" or "Upper Body Strength");
			var text = db.StaticConfigurations.AsNoTracking().Single(x => x.SettingName == key).Definition;
			Require(text == $"str:{strength.Id} * {multiplier}", "Wrong native strength formula: " + key);
			AuditFormulaReferences(db, text);
			return new[] { $"{key}={text}" };
		});

	private static BindingProof ScaffoldStatic(string key, string canonical, string? disabledGate, string evidence) =>
		new("engine-scaffold-ungranted", evidence, db =>
		{
			var skill = SettingAuditSkills(db)[canonical];
			Require(ArmageddonSkillPackage.Skills.Single(x => x.Key == canonical).Scaffold, "Expected explicit scaffold: " + canonical);
			var text = db.StaticConfigurations.AsNoTracking().Single(x => x.SettingName == key).Definition;
			Require(disabledGate is null ? text == skill.Id.ToString(CultureInfo.InvariantCulture) : text == skill.Name,
				"Wrong scaffold static: " + key);
			Require(!db.ChargenRolesTraits.Any(x => x.TraitId == skill.Id), "Scaffold was directly role-granted: " + canonical);
			foreach (var merit in db.Merits.AsNoTracking().Where(x => x.Type == "Role Skill Progression"))
				Require(!XElement.Parse(merit.Definition).Descendants("Rule").Any(x => (long?)x.Attribute("trait") == skill.Id),
					"Scaffold entered source progression: " + canonical);
			if (disabledGate is not null)
				Require(db.StaticConfigurations.AsNoTracking().Single(x => x.SettingName == disabledGate).Definition == "false",
					"Absent optional scaffold feature became active: " + disabledGate);
			return new[] { $"{key}={text}; scaffold={canonical}; no source-role grants; optional gate={disabledGate ?? "none"}" };
		});

	private static BindingProof RangedFamily(string canonical, MudSharp.Combat.RangedWeaponType family, string evidence) =>
		new("bound", evidence, db =>
		{
			var id = SettingAuditSkills(db)[canonical].Id;
			var types = db.RangedWeaponTypes.AsNoTracking().Where(x => x.RangedWeaponType == (int)family).ToArray();
			Require(types.Length > 0 && types.All(x => x.FireTraitId == id && x.OperateTraitId == id), "Wrong ranged family: " + canonical);
			var componentType = family == MudSharp.Combat.RangedWeaponType.Bow ? "Bow" : "ThrownWeapon";
			var components = db.GameItemComponentProtos.AsNoTracking().Where(x => x.Type == componentType).ToArray();
			foreach (var type in types)
			{
				Require(components.Any(x => (long?)XElement.Parse(x.Definition).Element("RangedWeaponType") == type.Id), "Missing ranged component: " + type.Name);
				AuditFormulaReferences(db, type.AccuracyBonusExpression);
				AuditFormulaReferences(db, type.DamageBonusExpression);
			}
			return types.Select(x => $"RangedWeaponTypes[{x.Id}]={x.Name};Fire/Operate={id}").ToArray();
		});

	private static BindingProof AbsentSourceRecipes(string canonical, bool approvedDeferred, string evidence) =>
		new(approvedDeferred ? "approved-content-deferred" : "disabled-no-consumer", evidence, db =>
		{
			var id = SettingAuditSkills(db)[canonical].Id;
			Require(!db.Crafts.Any(x => x.CheckTraitId == id), "A supposedly absent source recipe requires a concrete enabled binding audit: " + canonical);
			return new[] { $"Owned source trait={id}; Crafts[CheckTraitId={id}]=0; recipe gameplay not qualified" };
		});

	private static BindingProof AbsentSourcePower(string canonical, string evidence) =>
		new("approved-content-deferred", evidence + "; powers last; source trait/tree retained", db =>
		{
			var id = SettingAuditSkills(db)[canonical].Id;
			// Templar-only powers do not establish a psychic consumer for the mundane class trees.
			var templarPowers = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
				x.Module == ArmageddonTemplarInstaller.Module && x.EntityType == "MagicPower" && !x.Retired).Select(x => x.LogicalId).ToArray();
			Require(db.MagicPowers.All(x => Enumerable.Contains(templarPowers, x.Id)), "Unqualified non-Templar power appeared: " + canonical);
			return new[] { $"Owned source trait={id}; non-Templar MagicPowers=0; mundane source psychic functionality remains unqualified" };
		});

	private static readonly CheckType[] OptionalTrapChecks =
	[
		CheckType.SetTrapCheck, CheckType.SpotTrapCheck, CheckType.SearchForTrapCheck, CheckType.AvoidTrapCheck,
		CheckType.DisarmTrapCheck, CheckType.DispelTrapCheck, CheckType.EscapeTrapCheck
	];
	private static bool TryAuditInactiveTrapCheck(FuturemudDatabaseContext db, CheckType type)
	{
		if (!OptionalTrapChecks.Contains(type)) return false;
		Require(!db.TrapTemplates.Any(), "Optional Trap consumers appeared; audit their explicit canonical bindings.");
		var types=OptionalTrapChecks.Select(x=>(int)x).ToArray();
		var checks=db.Checks.AsNoTracking().Where(x=>types.Contains(x.Type)).ToArray();
		Require(checks.Length is 0 or 7, "Optional Trap check inventory is incomplete.");
		// Futuremud.LoadChecks persists generic50 backfills for missing enum values.
		// Verify that precise bootstrap shape, report it UNBOUND, and never count it
		// as canonical Trap qualification or adopt its identity into the adapter.
		foreach (var check in checks)
		{
			var expression=db.TraitExpressions.AsNoTracking().Single(x=>x.Id==check.TraitExpressionId);
			var template=db.CheckTemplates.AsNoTracking().Single(x=>x.Id==check.CheckTemplateId);
			Require(expression.Expression=="50" && expression.Name==$"{((CheckType)check.Type).DescribeEnum(true)} Check" &&
				!db.TraitExpressionParameters.Any(x=>x.TraitExpressionId==expression.Id) &&
				template.Name is "SkillCheck" or "Skill Check" && template.CheckMethod=="Standard" &&
				check.MaximumDifficultyForImprovement==(int)Difficulty.Impossible &&
				!db.SeederManagedRecords.Any(x=>x.EntityType==nameof(MudSharp.Models.Check) && x.LogicalId==check.Type),
				"Optional Trap row is not the unowned native bootstrap fallback; add explicit adapter qualification.");
		}
		return true;
	}
	private static BindingProof AbsentOptionalTraps(string evidence) =>
		new("disabled-no-consumer", evidence + "; TrapSeeder is explicitly optional, not baseline", db =>
		{
			var id = SettingAuditSkills(db)["arm.skill.trap"].Id;
			Require(TryAuditInactiveTrapCheck(db, CheckType.SetTrapCheck), "Inactive trap qualification failed.");
			return new[] { $"Owned Trap trait={id}; TrapTemplates=0; adapter uninstalled; any native generic50 backfills remain explicitly unbound and excluded from qualification" };
		});

	private static BindingProof BoundArmorRepair(string evidence) =>
		new("bound", evidence + "; ArmageddonRepairBinding native component adaptation", db =>
		{
			var id = SettingAuditSkills(db)["arm.skill.armor_repair"].Id;
			var check = db.Checks.AsNoTracking().Single(x => x.Type == (int)CheckType.RepairItemCheck);
			Require(db.TraitExpressions.AsNoTracking().Single(x => x.Id == check.TraitExpressionId).Expression == "variable",
				"RepairItemCheck stopped being a supplied-trait consumer.");
			var component = db.GameItemComponentProtos.AsNoTracking().Single(x => x.Name == "Armageddon Armour Repair Kit" && x.Type == "RepairKit");
			var xml = XElement.Parse(component.Definition);
			Require((long?)xml.Element("CheckTrait") == id && (int?)xml.Element("RepairPoints") == 500 &&
				(int?)xml.Element("MaximumSeverity") == (int)MudSharp.Health.WoundSeverity.Grievous && (double?)xml.Element("CheckBonus") == 0.0,
				"Authored native armour repair component binding/tuning drifted.");
			var armour = db.Tags.AsNoTracking().Single(x => x.Name == "Military Armour");
			Require(db.SeederManagedRecords.Any(x => x.Seeder == "CoreDataSeeder" && x.Module == "TagCatalogue" && x.EntityType == "Tag" && x.LogicalId == armour.Id),
				"Armour repair tag is not owned Core bootstrap data.");
			Require(xml.Element("Tags")!.Elements("Tag").Select(x => (long)x).SequenceEqual(new[] { armour.Id }), "Wrong native armour repair tag scope.");
			var claim = db.SeederManagedRecords.AsNoTracking().Single(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == "arm.repair.armour.component");
			Require(claim.EntityType == "GameItemComponentProto" && claim.LogicalId == component.Id && claim.RevisionNumber == component.RevisionNumber && !claim.Retired,
				"Armour repair component has no exact revision ownership.");
			return new[] { $"RepairItemCheck={check.Type}; formula=variable; CheckTrait={id}",
				$"RepairKit[{component.Id}:{component.RevisionNumber}]; MilitaryArmourTag={armour.Id}; authored points=500/maxGrievous; XML={component.Definition}" };
		});

	// Run in addition to the historical 55-key registry. This checks all ten current fixed-ID bindings,
	// including VehiclePilotTraitId, and all six fresh-world feature flags. Do not silently roll these into 55.
	private static object AuditCurrentStaticExtras(FuturemudDatabaseContext db)
	{
		Require(ArmageddonSkillPackage.StaticTraitBindings.Count == 10, "Current static canonical inventory drift.");
		var ids = ArmageddonSkillPackage.StaticTraitBindings.Select(x =>
			FixedTraitStatic(x.Key, x.Value, "ArmageddonSkillPackage.StaticTraitBindings").Inspect(db)).ToArray();
		var flags = new[] { "VehiclePilotEnabled", "DirectionSenseEnabled", "ArmageddonHostileGuardAttemptsEnabled",
			"HagglingEnabled", "BlindFightingEnabled", "DualWieldDoubleAttacksEnabled" }.Select(key =>
			ExactStatic(key, "true", "fresh Armageddon world opt-in after references bind").Inspect(db)).ToArray();
		var ride = SettingAuditSkills(db)["arm.skill.ride"].Id;
		var mount = db.Checks.AsNoTracking().Single(x => x.Type == (int)CheckType.MountControlCheck);
		var mountExpression = db.TraitExpressions.AsNoTracking().Single(x => x.Id == mount.TraitExpressionId);
		Require(mountExpression.Expression == $"ride:{ride}", "Dedicated MountControlCheck is missing its canonical Ride formula.");
		AuditFormulaReferences(db, mountExpression.Expression);
		var owned=SettingAuditSkills(db);
		var engine=(IReadOnlyDictionary<string,MudSharp.Models.TraitDefinition>)typeof(ArmageddonSkillPackage).GetMethod("EngineBindings",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!.Invoke(null,new object[]{db})!;
		var semanticChecks=new List<object>();
		foreach(var type in Enum.GetValues<CheckType>())
		{
			var expected=ArmageddonMundaneBindings.Formula(type,engine); if(expected is null) continue;
			var row=db.Checks.AsNoTracking().Single(x=>x.Type==(int)type);
			var formula=db.TraitExpressions.AsNoTracking().Single(x=>x.Id==row.TraitExpressionId).Expression;
			Require(formula==expected,"Canonical support check drift: "+type);
			semanticChecks.Add(new {Type=type.ToString(),Formula=formula});
		}
		foreach(var verb in new[]{"bash","kick"})
		{
			var command=db.ManualCombatCommands.AsNoTracking().Single(x=>x.PrimaryVerb==verb);
			var action=db.CombatActions.AsNoTracking().Single(x=>x.Id==command.CombatActionId);
			var profile=XElement.Parse(action.AdditionalInfo).Element("PhysicalImpact")!;
			Require(command.PlayerUsable && command.NpcUsable && action.TraitDefinitionId==owned["arm.skill."+verb].Id &&
				(long?)profile.Attribute("countertrait")==owned["arm.skill."+verb].Id && (long?)profile.Attribute("guardtrait")==owned["arm.skill.guarding"].Id,
				"Wrong coupled native impact binding: "+verb);
			Require(db.RacesCombatActions.Any(x=>x.CombatActionId==action.Id),"Unreachable impact action: "+verb);
		}
		foreach(var name in new[]{"Knife Backstab","Dagger Backstab","Blackjack Sapping Blow"})
		{
			var attack=db.WeaponAttacks.AsNoTracking().Single(x=>x.Name==name);var xml=XElement.Parse(attack.AdditionalInfo);
			var verb=name=="Blackjack Sapping Blow" ? "sap" : "backstab";
			Require((long?)xml.Element("Proficiency")==owned["arm.skill."+verb].Id && (string?)xml.Element("Command")==verb,"Wrong native opening binding: "+name);
		}
		foreach(var action in db.CombatActions.AsNoTracking().ToArray())
		{
			Require(db.TraitDefinitions.Any(x=>x.Id==action.TraitDefinitionId),"Auxiliary fallback would conceal missing primary trait: "+action.Name);
			foreach(var effect in XElement.Parse(action.AdditionalInfo).Elements("Effect"))
				if((long?)effect.Attribute("defensetrait") is {} trait) Require(db.TraitDefinitions.Any(x=>x.Id==trait),"Dangling auxiliary defense trait: "+action.Name);
		}
		return new { CanonicalStaticIds = ids, FreshFlags = flags, NewMountControl = new { mount.Type, mount.TraitExpressionId, mountExpression.Expression }, SemanticSupportChecks=semanticChecks };
	}
}
