#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Commands.Helpers;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed class RecordingSelectedMeleeCheck(Db.Check model, IFuturemud world) : StandardCheck(model, world)
	{
		internal int Calls { get; private set; }
		internal Difficulty ReferenceDifficulty { get; private set; }
		internal Dictionary<Difficulty, CheckOutcome>? Latest { get; private set; }
		internal void Reset() { Calls = 0; Latest = null; }
		public override Dictionary<Difficulty, CheckOutcome> CheckAgainstAllDifficulties(IPerceivableHaveTraits checkee,
			Difficulty referenceDifficulty, ITraitDefinition trait, IPerceivable? target = null, double externalBonus = 0.0,
			TraitUseType traitUseType = TraitUseType.Practical, params (string Parameter, object value)[] customParameters)
		{
			++Calls; ReferenceDifficulty = referenceDifficulty;
			return Latest = base.CheckAgainstAllDifficulties(checkee, referenceDifficulty, trait, target, externalBonus, traitUseType, customParameters);
		}
	}

	private sealed record SelectedMeleeCheckReader(string Database, long Owner, long Trait, double Raw,
		long Body, double Stamina, long VictimBody, double Wounds, string Case);
	private static int RunSelectedMeleeCheckReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<SelectedMeleeCheckReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		using var db = NewIndependentContext(database.ConnectionString);
		var trait = db.CharacterTraits.AsNoTracking().Single(x => x.CharacterId == input.Owner && x.TraitDefinitionId == input.Trait);
		Require(Same(trait.Value, input.Raw) && Same(db.Bodies.AsNoTracking().Single(x => x.Id == input.Body).CurrentStamina, input.Stamina) &&
			Same(db.Wounds.AsNoTracking().Where(x => x.BodyId == input.VictimBody).Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun), input.Wounds),
			"Fresh selected-melee reader must observe exact canonical trait, stamina and wound rows.");
		Console.WriteLine($"ARMSelectedCheck-reader={input.Case} passed raw:{trait.Value} stamina:{input.Stamina} wounds:{input.Wounds} fresh-process native-rows no-order-replay");
		return 0;
	}

	private static int RunSelectedMeleeCheck(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order)
	{
		using var learningGlobals = new CheckLearningGlobals();
		var native = host.Native; var world = native.World; var service = world.SpellOwnedCorpseAnimations!;
		typeof(MudSharp.Body.Implementations.Body).GetField("_encumbranceLimitExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1000", world));
		foreach (var property in new[] { "PowerMoveStaminaCost", "GraceMoveStaminaCost", "RecoveryTimeExpression" })
			typeof(CombatBase).GetProperty(property, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1", world));
		native.WorldMock.Setup(x => x.GetStaticDouble("EncumbranceLimitRatioHeavy")).Returns(0.8);
		native.WorldMock.Setup(x => x.GetStaticDouble("EncumbranceLimitRatioModerate")).Returns(0.5);
		native.WorldMock.Setup(x => x.GetStaticDouble("EncumbranceLimitRatioLight")).Returns(0.25);
		ConfigureStormHands(native);
		// Penetration/recovery remain fixture checks; the selected MeleeWeaponCheck is native.
		Mock.Get(world.GetCheck(CheckType.MeleeWeaponPenetrateCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponPenetrateCheck, Outcome.Pass));
		var expressions = (All<ITraitExpression>)world.TraitExpressions;
		var attacks = new All<IWeaponAttack>(); native.WorldMock.SetupGet(x => x.WeaponAttacks).Returns(attacks);
		native.WorldMock.SetupGet(x => x.BodypartShapes).Returns(new All<IBodypartShape>());
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var model in db.TraitExpressions.Where(x => x.Name.StartsWith("Flame declared")))
				if (!expressions.Has(model.Id)) expressions.Add(new TraitExpression(model, world));
			attacks.Add(WeaponAttack.LoadWeaponAttack(db.WeaponAttacks.Single(x => x.Name == "Flame declared burning thrust"), world));
		}
		var attack = attacks.Single();
		var weaponType = (WeaponType)world.WeaponTypes.Single(x => x.Name.StartsWith("ARM03C1")); weaponType.AddAttack(attack);
		var definition = (SkillDefinition)world.Traits.GetByName("ARM02 Earth Proficiency")!;
		Require(ReferenceEquals(weaponType.AttackTrait, definition), "Selected weapon must use the exact canonical learning trait.");
		var previousImprover = definition.Improver;
		var improver = new ClassicImprovement(world, new Db.Improver { Id = 99101, Name = "Selected melee native learning",
			Definition = "<Definition Chance='1' Expression='5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='60'/>" });
		var gain = new FutureProg(world, "armSelectedNativeGain", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.Trait, "trait")], "return 5");
		var applies = new FutureProg(world, "armSelectedNativeApplies", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "actor")], "return true");
		var bonus = new FutureProg(world, "armSelectedNativeBonus", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.Perceivable, "target")], "return 0");
		Require(gain.Compile() && applies.Compile() && bonus.Compile(), "Native selected-melee policy progs must compile."); improver.ImprovementProg = gain;
		var expression = new TraitExpression("1000+variable*0", world) { Id = 99101 }; expressions.Add(expression);
		var template = new Db.CheckTemplate { Name = "Native selected melee Check", ImproveTraits = true, CanBranchIfTraitMissing = false };
		foreach (var difficulty in Enum.GetValues<Difficulty>()) template.CheckTemplateDifficulties.Add(new() { Difficulty = (int)difficulty });
		var check = new RecordingSelectedMeleeCheck(new Db.Check { Type = (int)CheckType.MeleeWeaponCheck, TraitExpressionId = expression.Id,
			CheckTemplate = template, MaximumDifficultyForImprovement = (int)Difficulty.Impossible }, world);
		native.WorldMock.Setup(x => x.GetCheck(CheckType.MeleeWeaponCheck)).Returns(check);
		SpecificCheckBonusMerit.RegisterMeritInitialiser(); ScaledCheckCategoryBonusMerit.RegisterMeritInitialiser();
		IMerit Merit(string type, string xml)
		{
			using var db = NewIndependentContext(database.ConnectionString);
			var model = new Db.Merit { Id = (db.Merits.Max(x => (long?)x.Id) ?? 0) + 1, Name = "ARMSelected " + type,
				Type = type, MeritType = (int)MeritType.Merit, MeritScope = (int)MeritScope.Character, Definition = xml };
			db.Merits.Add(model); db.SaveChanges(); var merit = MeritFactory.LoadMerit(model, world); ((All<IMerit>)world.Merits).Add(merit); return merit;
		}
		var specific = (SpecificCheckBonusMerit)Merit("Specific Check Bonus", $"<Definition type='{(int)CheckType.MeleeWeaponCheck}' bonus='0'/>");
		var scaled = (ScaledCheckCategoryBonusMerit)Merit("Scaled Check Category Bonus", "<Definition hostile='true'/>");
		var messages = new CombatMessageManager(world); native.WorldMock.SetupGet(x => x.CombatMessageManager).Returns(messages);
		var bindings = new All<IManualCombatCommand>(); native.WorldMock.SetupGet(x => x.ManualCombatCommands).Returns(bindings);
		native.WorldMock.Setup(x => x.Add(It.IsAny<IManualCombatCommand>())).Callback<IManualCombatCommand>(x => bindings.Add(x));
		EditableItemHelper.ManualCombatCommandHelper.EditableNewAction(caster, new StringStack("learningstrike Learning Strike"));
		var binding = bindings.Single();
		Require(binding.NpcUsable && binding.BuildingCommand(caster, new StringStack($"action weapon {attack.Id}")), "Native NPC manual binding must be builder-authored.");
		var settings = (CharacterCombatSettings)caster.CombatSettings;
		settings.WeaponUsePercentage = 1; settings.NaturalWeaponPercentage = settings.AuxiliaryPercentage = settings.MagicUsePercentage = settings.PsychicUsePercentage = 0;
		settings.PreferredMeleeMode = CombatStrategyMode.StandardMelee; settings.ForbiddenIntentions = CombatMoveIntentions.None;
		foreach (var race in world.Races)
		{
			Mock.Get(race).SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true, CanUseWeapons = true, CanDefend = false, DefaultCombatSetting = settings });
			Mock.Get(race).SetupGet(x => x.RaceUsesStamina).Returns(true);
		}
		var configured = new HashSet<CommandableAI>();
		try
		{
			SetPrivateMember(definition, "Improver", improver);
			foreach (var scenario in new[] { "ordered-valid", "ordered-revoked", "applicable-independent", "applicable-revoke", "scoring-independent", "scoring-revoke", "final-setter-independent", "final-setter-aba", "final-setter-revoke", "direct-valid", "direct-scoring-independent" })
			{
				var actor = scenario == "ordered-valid" ? animated : cast(); actor.CombatSettings = settings;
				var body = actor.Body; var ai = actor.AIs.OfType<CommandableAI>().Single();
				if (configured.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included learningstrike")), "Native AI must allowlist the authored selected attack.");
				var previousMerits = actor.CharacterMerits.ToArray(); SetPrivateField(actor, "_merits", previousMerits.Concat(new IMerit[] { specific, scaled }).ToList());
				Require(actor.SetTraitValue(definition, 40) && actor.GetTrait(definition) is Skill, "Native skill baseline setup failed.");
				actor.RemoveAllEffects<NoTraitGain>(fireRemovalAction: true);
				var owner = (ICharacter)actor.GetTrait(definition).Owner;
				Require(!ReferenceEquals(owner, actor), "Animated learner must retain its separate canonical trait owner.");
				var weapon = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B flame knife").CreateNew(caster);
				world.Add(weapon); caster.Location.Insert(weapon, true); weapon.Login(); body.Get(weapon, silent: true); body.Wield(weapon, silent: true);
				Require(body.WieldedItems.Any(x => ReferenceEquals(x, weapon)), "Selected attack must wield the exact native item.");
				actor.TargettedBodypart = foe.Body.Bodyparts.OfType<IExternalBodypart>().First(); order(actor, caster, "hit opponent"); actor.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
				void Expire()
				{
					var origin = service.CommandGrant(actor.InstanceId, caster.Id)!;
					var source = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
					clock.Advance(DateTime.Parse(source.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) - RuntimeClock.UtcNow);
					Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Revocation must expire only original command control.");
				}
				var inResolution = false; var applicabilityCalls = 0; var scoringCalls = 0; var setterCalls = 0;
				bool SelectedScoring() => new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(StandardCheck) && x.GetMethod()?.Name == nameof(StandardCheck.CheckAgainstAllDifficulties)) &&
					new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(MeleeWeaponAttack));
				void Write(double value) { using var independent = CommandExecutionScope.EnterIndependent(); Require(actor.SetTraitValue(definition, value), "Independent same-trait write must succeed."); }
				var applicableFault = new Mock<IFutureProg>();
				applicableFault.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns<object[]>(arguments =>
				{
					if (inResolution && ReferenceEquals(arguments[0], actor) && SelectedScoring())
					{
						++applicabilityCalls;
						if (scenario is "applicable-independent" or "applicable-revoke") { Write(60); if (scenario == "applicable-revoke") Expire(); }
					}
					return applies.ExecuteBool(arguments);
				});
				SetPrivateMember(specific, "ApplicabilityProg", applicableFault.Object); SetPrivateMember(scaled, "ApplicabilityProg", applies);
				var scoringFault = new Mock<IFutureProg>();
				scoringFault.Setup(x => x.ExecuteDouble(It.IsAny<double>(), It.IsAny<object[]>())).Returns<double, object[]>((fallback, arguments) =>
				{
					if (inResolution && ReferenceEquals(arguments[0], actor) && SelectedScoring())
					{
						++scoringCalls;
						if (scenario is "scoring-independent" or "scoring-revoke" or "direct-scoring-independent") { Write(60); if (scenario == "scoring-revoke") Expire(); }
					}
					return bonus.ExecuteDouble(fallback, arguments);
				});
				scaled.GetBonusAmountProg = scoringFault.Object;
				var policyField = typeof(CommandableAI).GetField("_canCommandProg", BindingFlags.Instance | BindingFlags.NonPublic)!;
				var policy = (IFutureProg)policyField.GetValue(ai)!;
				var setterFault = new Mock<IFutureProg>();
				setterFault.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns<object[]>(arguments =>
				{
					if (inResolution && setterCalls == 0 && scenario.StartsWith("final-setter", StringComparison.Ordinal) &&
						new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(Trait) && x.GetMethod()?.Name == "set_Value") &&
						new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(Skill) && x.GetMethod()?.Name == nameof(Skill.TraitUsed)))
					{
						++setterCalls; Write(60);
						if (scenario == "final-setter-aba") Write(40);
						if (scenario == "final-setter-revoke") Expire();
					}
					return policy.ExecuteBool(arguments);
				});
				policyField.SetValue(ai, setterFault.Object);
				try
				{
					var direct = scenario.StartsWith("direct", StringComparison.Ordinal);
					if (direct) { Expire(); actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectManualCombatCommand(actor, binding, foe)); }
					else order(actor, caster, "learningstrike opponent");
					var move = actor.ChooseMove();
					Require(move is MeleeWeaponAttack && CommandExecutionAuthority.IsOrdered(move) == !direct, "Actual ChooseMove must select native melee with exact order provenance.");
					if (scenario == "ordered-revoked") Expire();
					((MudSharp.Body.Implementations.Body)body).CurrentStamina = 100; Require(Same(actor.CurrentStamina, 100), "Native stamina baseline must be exact.");
					world.SaveManager.Flush();
					var before = foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun); check.Reset(); inResolution = true;
					try { actor.Combat!.CombatAction(actor, move); } finally { inResolution = false; }
					var permitted = !scenario.Contains("revoke", StringComparison.Ordinal);
					var early = scenario is "applicable-independent" or "scoring-independent" or "direct-scoring-independent";
					var improved = permitted && !scenario.StartsWith("final-setter", StringComparison.Ordinal);
					var expected = early ? 65.0 : scenario is "applicable-revoke" or "scoring-revoke" or "final-setter-independent" or "final-setter-revoke" ? 60.0 : improved ? 45.0 : 40.0;
					var after = foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
					var rolled = check.Latest?[check.ReferenceDifficulty];
					Require(check.Calls == (scenario == "ordered-revoked" ? 0 : 1) && applicabilityCalls == check.Calls && scoringCalls == check.Calls,
						$"Native selected scoring seams must occur exactly once: {scenario}, check:{check.Calls}, applicability:{applicabilityCalls}, scoring:{scoringCalls}.");
					Require(rolled is null ? scenario == "ordered-revoked" : rolled.Outcome.IsPass() && rolled.Rolls.Count() == 3 && rolled.Rolls.All(x => x >= 0 && x < 100) && rolled.ImprovedTraits.Count() == (improved ? 1 : 0), "Native rolls and improvement reports must remain truthful after callbacks.");
					Require(Same(actor.TraitRawValue(definition), expected) && setterCalls == (scenario.StartsWith("final-setter", StringComparison.Ordinal) ? 1 : 0),
						$"Independent native write must survive final learning admission: {scenario}, raw:{actor.TraitRawValue(definition)}, expected:{expected}, setter:{setterCalls}.");
					Require(permitted ? after > before : Same(after, before), "Revoked learning/attack must not add wounds; admitted attacks must commit native wounds.");
					var stamina = permitted ? 100 - move.StaminaCost : 100;
					Require(Same(actor.CurrentStamina, stamina) && (!permitted || Same(move.StaminaCost, 1)), "Actual CombatAction must charge exact native cost only for committed attacks.");
					var cooldown = scenario is "ordered-revoked" or "applicable-revoke" or "scoring-revoke" ? 0 : 1;
					Require(actor.EffectsOfType<NoTraitGain>().Count() == cooldown, "Already committed learning cooldown must be retained without duplication.");
					world.SaveManager.Flush();
					using (var db = NewIndependentContext(database.ConnectionString))
						if (permitted) Require(db.Wounds.AsNoTracking().Any(x => x.BodyId == foe.Body.Id && x.ActorOriginId == actor.Identity.Id), "Committed wound attribution must use the canonical identity.");
					RunItemReaderProcess(new SelectedMeleeCheckReader(database.Name, owner.Id, definition.Id, expected, body.Id, stamina, foe.Body.Id, after, scenario), "--selected-melee-check-reader");
					Console.WriteLine($"ARMSelectedCheck={scenario} passed physical:{actor.InstanceId} canonical:{owner.Id} raw:{expected} improved:{rolled?.ImprovedTraits.Count() ?? 0} cooldown:{cooldown} cost:{100-stamina} wound-delta:{after-before} actual-ChooseMove-MeleeWeaponAttack-StandardCheck-native-merits");
				}
				finally { inResolution = false; policyField.SetValue(ai, policy); SetPrivateField(actor, "_merits", previousMerits.ToList()); }
				Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why); restored(actor); body.Take(weapon); weapon.Delete(); world.SaveManager.Flush();
			}
		}
		finally { SetPrivateMember(definition, "Improver", previousImprover); foreach (var item in bindings.ToArray()) bindings.Remove(item); ManualCombatCommandRegistry.Rebuild(world); }
		return 0;
	}
}
