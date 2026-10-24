#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class DualWieldDoubleAttackTests
{
	private sealed class LearningProbe(MudSharp.Models.Check model, IFuturemud world) : StandardCheck(model, world)
	{
		public CheckOutcome Finish(ICharacter actor) => HandleStandardCheck(actor, null!, Outcome.MajorPass, Difficulty.Normal, null!);
	}

	[TestMethod]
	public void Coordination_PersistedIdentityAndClassification_RemainsPhysicalOffensiveAndNotVisionLimited()
	{
		Assert.AreEqual(230, (int)CheckType.DualWieldCoordinationCheck);
		var identities = Enum.GetValues<CheckType>().Select(x => (int)x).ToArray();
		Assert.AreEqual(identities.Length, identities.Distinct().Count(), "Persisted check IDs must remain unique across integrated features.");
		Assert.IsTrue(CheckType.DualWieldCoordinationCheck.IsPhysicalActivityCheck());
		Assert.IsTrue(CheckType.DualWieldCoordinationCheck.IsOffensiveCombatAction());
		Assert.IsTrue(CheckType.DualWieldCoordinationCheck.IsTargettedHostileCheck());
		Assert.IsFalse(CheckType.DualWieldCoordinationCheck.IsVisionInfluencedCheck());
	}

	[TestMethod]
	[DataRow("missing-supplement")][DataRow("duplicate-learning")][DataRow("branching")][DataRow("syntax")]
	public void Coordination_UnsafeCustomBinding_DisablesPairWithoutEvaluation(string cause)
	{
		using var f = new Fixture(); var parameters = f.Formula.Object.Parameters;
		if (cause == "missing-supplement") parameters["missing"] = new() { Trait = null!, CanImprove = false, CanBranch = false };
		if (cause == "duplicate-learning") parameters["duplicate"] = new() { Trait = f.Skill.Object, CanImprove = true };
		if (cause == "branching") f.Coordination.SetupGet(x => x.CanTraitBranchIfMissing).Returns(true);
		if (cause == "syntax") f.Formula.Setup(x => x.HasErrors()).Returns(true);
		Assert.IsNull(DualWieldCombat.Candidate(f.Actor.Object, f.Candidates));
		Assert.AreEqual(0, f.CoordinationCalls);
	}

	[TestMethod]
	[DataRow(false)][DataRow(true)]
	public void Coordination_NativeLearningPolicy_MissingSkillMajorFailsWithoutAcquisitionAndKnownSkillImprovesOnce(bool trained)
	{
		using var f = new Fixture(); var expressions = new All<ITraitExpression>();
		f.Formula.SetupGet(x => x.Id).Returns(55); expressions.Add(f.Formula.Object);
		f.World.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var trait = new Mock<ITrait>(); trait.SetupGet(x => x.Definition).Returns(f.Skill.Object);
		f.Actor.Setup(x => x.HasTrait(f.Skill.Object)).Returns(trained);
		f.Actor.Setup(x => x.GetTrait(f.Skill.Object)).Returns(trained ? trait.Object : null!);
		var template = new MudSharp.Models.CheckTemplate { ImproveTraits = true,
			FailIfTraitMissingMode = (short)FailIfTraitMissingType.FailIfAnyMissing, CanBranchIfTraitMissing = false };
		foreach (var difficulty in Enum.GetValues<Difficulty>()) template.CheckTemplateDifficulties.Add(new() { Difficulty = (int)difficulty });
		var check = new LearningProbe(new MudSharp.Models.Check { Type = (int)CheckType.DualWieldCoordinationCheck,
			TraitExpressionId = 55, CheckTemplate = template, MaximumDifficultyForImprovement = 10 }, f.World.Object);
		var result = check.Finish(f.Actor.Object);
		Assert.AreEqual(trained ? Outcome.MajorPass : Outcome.MajorFail, result.Outcome);
		Assert.AreEqual(0, result.AcquiredTraits.Count());
		trait.Verify(x => x.TraitUsed(f.Actor.Object, It.IsAny<Outcome>(), Difficulty.Normal, TraitUseType.Practical,
			It.IsAny<IEnumerable<Tuple<string, double>>>()), trained ? Times.Once : Times.Never);
		Assert.IsFalse(f.Actor.Invocations.Any(x => x.Method.Name == "AddTrait"));
	}
	private sealed class Host : CombatBase
	{
		public int Uses, Scores, Recoveries;
		public Action? OnUse;
		public (Difficulty Difficulty, double Delay) Recovery;
		public override bool Friendly => true;
		public override string CombatHeaderDescription => "test";
		public override string LDescAddendumFor(ICombatant combatant, IPerceiver voyeur) => "test";
		public override void JoinCombat(IPerceiver character, Difficulty initialDelayDifficulty = Difficulty.Automatic, bool preserveHide = false) => _combatants.Add(character);
		public override bool LeaveCombat(IPerceiver character) => _combatants.Remove(character);
		public override void EndCombat(bool echo) { }
		public override void TruceRequested(IPerceiver combatant) { }
		protected override void FireOnUseProg(ICombatMove move) { Uses++; OnUse?.Invoke(); }
		protected override void HandleCombatResult(IPerceiver actor, ICombatMove move, ICombatMove response, CombatMoveResult result) => Scores++;
		protected override TimeSpan GetCombatDelay(IPerceiver actor, Difficulty difficulty, double baseTime = 1)
		{ Recoveries++; Recovery = (difficulty, baseTime); return TimeSpan.FromSeconds(baseTime); }
	}

	private sealed class Fixture : IDisposable
	{
		public readonly Mock<IFuturemud> World = new();
		public readonly Mock<ICharacter> Actor = new(), Target = new();
		public readonly Mock<IBody> Body = new(), TargetBody = new();
		public readonly Mock<ICharacterCombatSettings> Settings = new();
		public readonly Mock<ICheck> Coordination = new();
		public readonly Mock<ITraitExpression> Formula = new();
		public readonly Mock<ITraitDefinition> Skill = new();
		public readonly Mock<IRace> Race = new();
		public readonly Mock<IScheduler> Scheduler = new();
		public readonly Host Combat;
		public readonly List<IGameItem> Equipment = [];
		public readonly List<(IMeleeWeapon Weapon, IWeaponAttack Attack, double Weight)> Candidates = [];
		public readonly Dictionary<string, string> Config = new(DefaultStaticSettings.DefaultStaticConfigurations);
		public Outcome Outcome = Outcome.Pass;
		public int CoordinationCalls;
		public double Stamina = 50, DefenceStamina = 50;
		private readonly object? _oldPower;

		public Fixture()
		{
			Combat = new Host { Gameworld = World.Object };
			World.Setup(x => x.GetStaticConfiguration(It.IsAny<string>())).Returns((string key) => Config.GetValueOrDefault(key)!);
			World.Setup(x => x.GetCheck(CheckType.DualWieldCoordinationCheck)).Returns(Coordination.Object);
			World.SetupGet(x => x.Scheduler).Returns(Scheduler.Object);
			Skill.SetupGet(x => x.Id).Returns(42);
			Skill.SetupGet(x => x.TraitType).Returns(TraitType.Skill);
			Skill.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character);
			Formula.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>
			{ ["coordination"] = new() { Trait = Skill.Object, CanImprove = true, CanBranch = false } });
			Coordination.SetupGet(x => x.Type).Returns(CheckType.DualWieldCoordinationCheck);
			Coordination.SetupGet(x => x.FailIfTraitMissing).Returns(FailIfTraitMissingType.FailIfAnyMissing);
			Coordination.SetupGet(x => x.TargetNumberExpression).Returns(Formula.Object);
			Coordination.Setup(x => x.Check(Actor.Object, It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), Target.Object,
				It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(() => { CoordinationCalls++; return CheckOutcome.SimpleOutcome(CheckType.DualWieldCoordinationCheck, Outcome); });
			Config["DualWieldDoubleAttacksEnabled"] = "true";
			foreach (var actor in new[] { Actor, Target })
			{
				actor.SetupProperty(x => x.Combat, Combat);
				actor.SetupGet(x => x.Gameworld).Returns(World.Object);
				actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
				actor.SetupGet(x => x.Race).Returns(Race.Object);
				actor.Setup(x => x.CheckCombatStatus()).Returns(true);
				Combat.JoinCombat(actor.Object);
			}
			Race.SetupGet(x => x.RaceUsesStamina).Returns(true);
			Actor.SetupGet(x => x.CombatTarget).Returns(Target.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Target.SetupGet(x => x.Body).Returns(TargetBody.Object);
			Actor.SetupGet(x => x.MeleeRange).Returns(true);
			Actor.Setup(x => x.ColocatedWith(Target.Object)).Returns(true);
			Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns((double amount) => Stamina >= amount);
			Actor.Setup(x => x.SpendStamina(It.IsAny<double>())).Callback((double amount) => Stamina -= amount);
			Target.Setup(x => x.SpendStamina(It.IsAny<double>())).Callback((double amount) => DefenceStamina -= amount);
			Body.Setup(x => x.SpendStamina(It.IsAny<double>())).Callback((double amount) => Stamina -= amount);
			TargetBody.Setup(x => x.SpendStamina(It.IsAny<double>())).Callback((double amount) => DefenceStamina -= amount);
			Actor.SetupGet(x => x.CombatSettings).Returns(Settings.Object);
			Settings.SetupGet(x => x.AttackHelpless).Returns(true);
			Settings.SetupGet(x => x.AttackCriticallyInjured).Returns(true);
			Settings.SetupGet(x => x.AttackDisarmed).Returns(true);
			Settings.SetupGet(x => x.ClassificationsAllowed).Returns([WeaponClassification.Lethal]);
			Body.SetupGet(x => x.WieldedItems).Returns(Equipment);
			Actor.Setup(x => x.GetDifficultyForTool(It.IsAny<IGameItem>(), It.IsAny<Difficulty>())).Returns(Difficulty.Normal);
			var property = typeof(CombatBase).GetProperty("PowerMoveStaminaCost", BindingFlags.Static | BindingFlags.NonPublic)!;
			_oldPower = property.GetValue(null);
			var power = new Mock<ITraitExpression>();
			power.Setup(x => x.EvaluateWith(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>(), It.IsAny<(string, object)[]>())).Returns(1);
			property.SetValue(null, power.Object);
			AddWeapon(1, 3, 1); AddWeapon(2, 5, 2);
		}

		public void AddWeapon(long id, double cost, double delay)
		{
			var weapon = new Mock<IMeleeWeapon>(); var item = new Mock<IGameItem>(); var attack = new Mock<IWeaponAttack>();
			var hand = new Mock<IWield>();
			item.SetupGet(x => x.Id).Returns(id); item.Setup(x => x.GetItemType<IMeleeWeapon>()).Returns(weapon.Object);
			weapon.SetupGet(x => x.Parent).Returns(item.Object); weapon.SetupGet(x => x.Classification).Returns(WeaponClassification.Lethal);
			weapon.SetupGet(x => x.WeaponType).Returns(Mock.Of<IWeaponType>(x => x.AttackTrait == Mock.Of<ITraitDefinition>(s => s.Id == id)));
			Body.Setup(x => x.WieldedHand(item.Object)).Returns(hand.Object);
			Body.Setup(x => x.WieldedHandCount(item.Object)).Returns(1);
			attack.SetupGet(x => x.Id).Returns(id); attack.SetupGet(x => x.MaximumTargets).Returns(1);
			attack.SetupGet(x => x.Profile).Returns(Mock.Of<IDamageProfile>(x => x.BaseAttackerDifficulty == Difficulty.Normal));
			attack.SetupGet(x => x.StaminaCost).Returns(cost); attack.SetupGet(x => x.BaseDelay).Returns(delay);
			attack.Setup(x => x.UsableAttack(Actor.Object, item.Object, Target.Object, It.IsAny<AttackHandednessOptions>(), false,
				It.IsAny<BuiltInCombatMoveType[]>())).Returns(true);
			Equipment.Add(item.Object); Candidates.Add((weapon.Object, attack.Object, 100));
		}

		public DualWieldDoubleAttackMove Pair() => new(Actor.Object, Target.Object,
			Candidates[0].Weapon, Candidates[0].Attack, Candidates[1].Weapon, Candidates[1].Attack);
		public void Dispose() => typeof(CombatBase).GetProperty("PowerMoveStaminaCost", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, _oldPower);
	}

	[TestMethod]
	[DataRow(Outcome.MajorPass, Difficulty.Easy)]
	[DataRow(Outcome.Pass, Difficulty.Normal)]
	[DataRow(Outcome.MinorPass, Difficulty.Normal)]
	[DataRow(Outcome.MinorFail, Difficulty.Hard)]
	[DataRow(Outcome.Fail, Difficulty.Hard)]
	[DataRow(Outcome.MajorFail, Difficulty.VeryHard)]
	public void Prepare_CoordinationOutcome_StagesBothIndependentWeaponChecksOnce(Outcome outcome, Difficulty difficulty)
	{
		using var f = new Fixture(); f.Outcome = outcome; var pair = f.Pair();
		Assert.IsTrue(pair.TryPrepare()); Assert.IsFalse(pair.TryPrepare());
		Assert.AreEqual(1, f.CoordinationCalls); Assert.AreEqual(8, pair.StaminaBudget);
		Assert.AreEqual(2, pair.Children.Count);
		foreach (var child in pair.Children) Assert.AreEqual(difficulty, child.CheckDifficulty);
		Assert.AreNotSame(((IWeaponAttackMove)pair.Children[0]).Weapon.WeaponType.AttackTrait,
			((IWeaponAttackMove)pair.Children[1]).Weapon.WeaponType.AttackTrait);
		Assert.AreEqual(50, f.Stamina, "Planning/coordination must not precharge unexecuted strikes.");
	}

	[TestMethod]
	[DataRow("disabled")][DataRow("unbound")][DataRow("stamina")][DataRow("twohands")]
	public void Prepare_FailedPreflight_PerformsNoCheckOrCharge(string cause)
	{
		using var f = new Fixture();
		if (cause == "disabled") f.Config["DualWieldDoubleAttacksEnabled"] = "false";
		if (cause == "unbound") f.Formula.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>());
		if (cause == "stamina") f.Stamina = 7;
		if (cause == "twohands") f.Body.Setup(x => x.WieldedHandCount(f.Equipment[1])).Returns(2);
		Assert.IsFalse(f.Pair().TryPrepare()); Assert.AreEqual(0, f.CoordinationCalls);
	}

	[TestMethod]
	[DataRow("target")][DataRow("counter")][DataRow("disarm")][DataRow("destroy")]
	[DataRow("leave")][DataRow("retarget")][DataRow("move")][DataRow("arm")][DataRow("drain")]
	public void Continue_FirstExchangeInvalidatesParticipant_SkipsSecond(string cause)
	{
		using var f = new Fixture(); var pair = f.Pair(); Assert.IsTrue(pair.TryPrepare());
		switch (cause)
		{
			case "target": f.Target.SetupGet(x => x.State).Returns(CharacterState.Unconscious); break;
			case "counter": f.Actor.SetupGet(x => x.State).Returns(CharacterState.Unconscious); break;
			case "disarm": f.Equipment.RemoveAt(1); break;
			case "destroy": Mock.Get(f.Equipment[1]).SetupGet(x => x.Destroyed).Returns(true); break;
			case "leave": f.Target.Object.Combat = null!; break;
			case "retarget": f.Actor.SetupGet(x => x.CombatTarget).Returns(Mock.Of<ICharacter>()); break;
			case "move": f.Actor.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(false); break;
			case "arm": f.Body.Setup(x => x.CanUseBodypart(f.Body.Object.WieldedHand(f.Equipment[1]))).Returns(CanUseBodypartResult.CantUsePartDamage); break;
			case "drain": f.Stamina = 0; break;
		}
		Assert.IsFalse(pair.CanExecute(pair.Children[1])); Assert.AreEqual(1, f.CoordinationCalls);
	}

	[TestMethod]
	public void Continue_UsabilityCallbackRemovesEquipment_FinalPhysicalCheckRejectsStrike()
	{
		using var f = new Fixture(); var pair = f.Pair(); Assert.IsTrue(pair.TryPrepare());
		Mock.Get(f.Candidates[1].Attack).Setup(x => x.UsableAttack(f.Actor.Object, f.Equipment[1], f.Target.Object,
			It.IsAny<AttackHandednessOptions>(), false, It.IsAny<BuiltInCombatMoveType[]>()))
			.Returns(() => { f.Equipment.RemoveAt(1); return true; });
		Assert.IsFalse(pair.CanExecute(pair.Children[1]));
	}

	[TestMethod]
	public void Candidate_ProfileCountAndExtraArms_NormalizeMassAndFreezeAffordableLead()
	{
		using var f = new Fixture();
		var pair = DualWieldCombat.Candidate(f.Actor.Object, f.Candidates, () => 0.9)!.Value;
		Assert.AreEqual(50, pair.Weight); Assert.AreEqual(0, f.CoordinationCalls);
		f.AddWeapon(3, 4, 3);
		Assert.AreEqual(75, DualWieldCombat.Candidate(f.Actor.Object, f.Candidates, () => 0.1)!.Value.Weight);
		f.Stamina = 7;
		var affordable = DualWieldCombat.Candidate(f.Actor.Object, f.Candidates, () => 0.9);
		Assert.IsNotNull(affordable); Assert.IsTrue(affordable.Value.Move.TryPrepare());
		Assert.IsTrue(affordable.Value.Move.StaminaBudget <= 7);
		f.Stamina = 2;
		Assert.IsNull(DualWieldCombat.Candidate(f.Actor.Object, f.Candidates, () => 0.5));
	}

	[TestMethod]
	public void Candidate_DualFeintOrMultiTargetOrDisabled_LeavesOrdinaryProfileAlone()
	{
		using var f = new Fixture(); var profile = Mock.Get(f.Candidates[1].Attack);
		profile.SetupGet(x => x.HandednessOptions).Returns(AttackHandednessOptions.DualWieldOnly);
		Assert.IsNull(DualWieldCombat.Candidate(f.Actor.Object, f.Candidates));
		profile.SetupGet(x => x.HandednessOptions).Returns(AttackHandednessOptions.Any);
		profile.SetupGet(x => x.MaximumTargets).Returns(2);
		Assert.IsNull(DualWieldCombat.Candidate(f.Actor.Object, f.Candidates));
		f.Config["DualWieldDoubleAttacksEnabled"] = "false";
		Assert.IsNull(DualWieldCombat.Candidate(f.Actor.Object, f.Candidates));
		Assert.AreEqual(0, f.CoordinationCalls);
	}

	[TestMethod]
	public void Recovery_CompletedChildLedger_UsesSlowerDelayAndHarderActualDifficulty()
	{
		using var f = new Fixture(); var pair = f.Pair(); Assert.IsTrue(pair.TryPrepare());
		var miss = new CombatMoveResult { RecoveryDifficulty = Difficulty.Hard };
		pair.RecordResolution(new(pair.Children[0], null, miss, true, true, 3, 0));
		Assert.AreEqual(Difficulty.Hard, pair.CompleteResolution().RecoveryDifficulty); Assert.AreEqual(1, pair.BaseDelay);
		// A completed sequence computes its final delay from completed children, independently of partial inspection.
		var both = f.Pair(); Assert.IsTrue(both.TryPrepare());
		both.RecordResolution(new(both.Children[0], null, miss, true, true, 3, 0));
		both.RecordResolution(new(both.Children[1], null, new CombatMoveResult { MoveWasSuccessful = true, RecoveryDifficulty = Difficulty.Easy }, true, true, 5, 0));
		Assert.IsTrue(both.CompleteResolution().MoveWasSuccessful); Assert.AreEqual(3, both.BaseDelay);
	}

	[TestMethod]
	[DataRow("normal")][DataRow("scheduled")][DataRow("interrupt")][DataRow("body-replaced")][DataRow("onuse-leaves")][DataRow("lethal")]
	public void SequenceHost_ChildCompletionBeforeNextDefence_ChargesAndFiresOnce(string mode)
	{
		using var f = new Fixture(); var sequence = new Mock<ICombatMoveSequence>();
		var records = new List<CombatMoveResolution>(); var children = new[] { new Mock<ICombatMove>(), new Mock<ICombatMove>() };
		var result = new CombatMoveResult { MoveWasSuccessful = true, RecoveryDifficulty = Difficulty.Normal };
		sequence.SetupGet(x => x.Assailant).Returns(f.Actor.Object); sequence.SetupGet(x => x.Target).Returns(f.Target.Object);
		sequence.SetupGet(x => x.OriginalCombat).Returns(f.Combat); sequence.SetupGet(x => x.Children).Returns(children.Select(x => x.Object).ToArray());
		sequence.SetupGet(x => x.Resolutions).Returns(records);
		sequence.SetupGet(x => x.BaseDelay).Returns(3);
		sequence.Setup(x => x.TryPrepare()).Returns(true);
		sequence.Setup(x => x.CanExecute(It.IsAny<ICombatMove>())).Returns(() => mode is "normal" or "scheduled" || records.Count == 0);
		sequence.Setup(x => x.RecordResolution(It.IsAny<CombatMoveResolution>())).Callback((CombatMoveResolution r) => records.Add(r));
		sequence.Setup(x => x.CompleteResolution()).Returns(result);
		for (var i = 0; i < children.Length; i++)
		{
			var index = i; var defence = new Mock<ICombatMove>();
			defence.SetupGet(x => x.Assailant).Returns(f.Target.Object); defence.SetupGet(x => x.StaminaCost).Returns(2);
			defence.Setup(x => x.UsesStaminaWithResult(result)).Returns(true);
			children[i].SetupGet(x => x.Assailant).Returns(f.Actor.Object); children[i].SetupGet(x => x.StaminaCost).Returns(3);
			children[i].Setup(x => x.UsesStaminaWithResult(result)).Returns(true);
			children[i].Setup(x => x.ResolveMove(defence.Object)).Returns(() =>
			{
				if (mode == "body-replaced") f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
				if (mode == "lethal") { f.Combat.LeaveCombat(f.Target.Object); f.Target.Object.Combat = null!; }
				return result;
			});
			f.Target.Setup(x => x.ResponseToMove(children[index].Object, f.Actor.Object)).Returns(() =>
			{
				Assert.AreEqual(50 - index * 3, f.Stamina); Assert.AreEqual(50 - index * 2, f.DefenceStamina);
				Assert.AreEqual(index, f.Combat.Uses); return defence.Object;
			});
		}
		if (mode == "onuse-leaves") f.Combat.OnUse = () => { f.Combat.LeaveCombat(f.Actor.Object); f.Actor.Object.Combat = null!; };
		if (mode == "scheduled") f.Combat.CombatAction(f.Actor.Object, sequence.Object);
		else Assert.AreSame(result, f.Combat.ResolveSequence(sequence.Object));
		var count = mode is "normal" or "scheduled" ? 2 : 1;
		Assert.AreEqual(count, records.Count);
		Assert.AreEqual(mode == "body-replaced" ? 0 : count, f.Combat.Uses);
		Assert.AreEqual(mode is "body-replaced" or "onuse-leaves" ? 0 : count, f.Combat.Scores);
		Assert.AreEqual(50 - count * 3, f.Stamina); Assert.AreEqual(50 - count * 2, f.DefenceStamina);
		Assert.AreEqual(mode == "scheduled" ? 1 : 0, f.Combat.Recoveries, "Only the outer action may schedule recovery, once.");
		if (mode == "scheduled")
		{
			Assert.AreEqual((Difficulty.Normal, 3.0), f.Combat.Recovery);
			f.Scheduler.Verify(x => x.AddOrDelaySchedule(It.IsAny<ISchedule>(), f.Actor.Object), Times.Once);
		}
	}
}
