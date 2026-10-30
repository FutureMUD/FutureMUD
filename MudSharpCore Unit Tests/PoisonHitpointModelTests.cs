using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class PoisonHitpointModelTests
{
	[TestMethod]
	[DataRow(false, 80.0)]
	[DataRow(true, 90.0)]
	public void CapacityPenalty_OptionalModeDoesNotCountDamageTwiceOrHealOnExpiry(bool separate, double current)
	{
		var (actor, body, world, strategy, wounds, modifiers) = Fixture(separate);
		var wound = LoadWound(actor.Object, body.Object, world.Object, 10, 0); wounds.Add(wound);
		modifiers.Add(new TimedMaximumHitPointsEffect(body.Object, -10, 1, capacityOnly: true));
		Assert.AreEqual(90, strategy.MaxHP(actor.Object));
		Assert.AreEqual(current, strategy.CurrentHealthPercentage(actor.Object) * strategy.MaxHP(actor.Object), 1e-9);
		Assert.AreEqual(current, ((IHealthStrategy)strategy).CurrentHitPoints(actor.Object), 1e-9);
		var query = new MudSharp.FutureProg.Functions.Characters.HealthCapacityFunction([
			new MudSharp.FutureProg.Functions.ConstantFunction(actor.Object),
			new MudSharp.FutureProg.Functions.ConstantFunction(new MudSharp.FutureProg.Variables.TextVariable("health"))], true);
		Assert.AreEqual(MudSharp.FutureProg.StatementResult.Normal, query.Execute(Mock.Of<MudSharp.FutureProg.IVariableSpace>()));
		Assert.AreEqual((decimal)current, query.Result.GetObject);
		wound.CurrentDamage = 0;
		Assert.AreEqual(separate ? 10 : 0, wound.CurrentDamage);
		modifiers.Clear();
		Assert.AreEqual(separate ? 90 : 100, strategy.CurrentHealthPercentage(actor.Object) * strategy.MaxHP(actor.Object), 1e-9);
	}

	[TestMethod]
	public void CapacityPenalty_HealingFloorIsSharedAcrossWoundsAndDoesNotPreventLethality()
	{
		var (actor, body, world, strategy, wounds, modifiers) = Fixture(true);
		var first = LoadWound(actor.Object, body.Object, world.Object, 10, 0);
		var second = LoadWound(actor.Object, body.Object, world.Object, 20, 0);
		wounds.Add(first); wounds.Add(second);
		modifiers.Add(new TimedMaximumHitPointsEffect(body.Object, -20, 1, capacityOnly: true));
		first.CurrentDamage = 0; second.CurrentDamage = 0;
		Assert.AreEqual(20, wounds.Sum(x => x.CurrentDamage));
		actor.SetupGet(x => x.State).Returns(CharacterState.Unconscious);
		Assert.IsFalse(strategy.IsCriticallyInjured(actor.Object));
		first.CurrentDamage = 100;
		Assert.AreEqual(HealthTickResult.Dead, strategy.EvaluateStatus(actor.Object));
	}

	[TestMethod]
	public void OptionalStun_PersistsInHealingWoundsAndDrivesNativeUnconsciousness()
	{
		var (actor, body, world, strategy, wounds, _) = Fixture(true);
		typeof(BrainHitpointsStrategy).GetProperty("UseStun")!.SetValue(strategy, true);
		strategy.MaximumStunExpression = Constant(actor.Object, 100);
		var wound = LoadWound(actor.Object, body.Object, world.Object, 0, 120); wounds.Add(wound);
		Assert.AreEqual(120, wound.CurrentStun);
		Assert.IsFalse(wound.ShouldWoundBeRemoved());
		Assert.AreEqual(HealthTickResult.Unconscious, strategy.EvaluateStatus(actor.Object));
		wound.CurrentStun = 0;
		Assert.IsTrue(wound.ShouldWoundBeRemoved());
		Assert.AreEqual(HealthTickResult.None, strategy.EvaluateStatus(actor.Object));
	}

	[TestMethod]
	public void RetainedOrganicWounds_RespectTheSameHealingCeiling()
	{
		var (actor, body, world, _, wounds, modifiers) = Fixture(true);
		var wound = new SimpleOrganicWound(actor.Object, new MudSharp.Models.Wound { Id = 2, CurrentDamage = 10,
			OriginalDamage = 10, DamageType = (int)DamageType.Cellular, ExtraInformation = "<Definition/>" }, world.Object, body.Object);
		wounds.Add(wound);
		modifiers.Add(new TimedMaximumHitPointsEffect(body.Object, -10, 1, capacityOnly: true));
		wound.CurrentDamage = 0;
		Assert.AreEqual(10, wound.CurrentDamage);
	}

	[TestMethod]
	[DataRow(CharacterState.Awake, false)]
	[DataRow(CharacterState.Unconscious, false)]
	[DataRow(CharacterState.Dead, true)]
	public void NativeDeathProgProperty_DistinguishesDeathFromIncapacitation(CharacterState state, bool expected)
	{
		var actor = TestObjectFactory.CreateUninitialized<MudSharp.Character.Character>();
		typeof(MudSharp.Character.Character).GetField("_state", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(actor, state);
		Assert.AreEqual(expected, ((MudSharp.FutureProg.IProgVariable)actor).GetProperty("isdead").GetObject);
	}

	private static void Set(BrainHitpointsStrategy strategy, string property, object value) =>
		typeof(BrainHitpointsStrategy).GetProperty(property)!.SetValue(strategy, value);

	private static void NegativeHealth(BrainHitpointsStrategy strategy)
	{
		Set(strategy, nameof(strategy.UseNegativeHitPoints), true);
		Set(strategy, nameof(strategy.DeathHitPoints), -11.0);
		Set(strategy, nameof(strategy.UnconsciousHitPoints), 0.0);
		Set(strategy, nameof(strategy.MortallyWoundedHitPoints), -3.0);
	}

	[TestMethod]
	[DataRow(1.0, HealthTickResult.None)]
	[DataRow(0.0, HealthTickResult.Unconscious)]
	[DataRow(-2.0, HealthTickResult.Unconscious)]
	[DataRow(-3.0, HealthTickResult.Unconscious)]
	[DataRow(-6.0, HealthTickResult.Unconscious)]
	[DataRow(-10.0, HealthTickResult.Unconscious)]
	[DataRow(-11.0, HealthTickResult.Dead)]
	[DataRow(-12.0, HealthTickResult.Dead)]
	public void NegativeHitPoints_UsesConfiguredDeathAndConsciousnessBoundaries(double hp, HealthTickResult expected)
	{
		var (actor, body, world, strategy, wounds, _) = Fixture(true);
		NegativeHealth(strategy);
		wounds.Add(LoadWound(actor.Object, body.Object, world.Object, 100 - hp, 0));
		Assert.AreEqual(hp, strategy.CurrentHitPoints(actor.Object));
		Assert.AreEqual(expected, strategy.EvaluateStatus(actor.Object));
		if (hp <= -3) Assert.IsTrue(strategy.IsCriticallyInjured(actor.Object));
	}

	[TestMethod]
	[DataRow(false, -2.0, HealthTickResult.Unconscious)]
	[DataRow(true, -2.0, HealthTickResult.None)]
	[DataRow(true, -10.0, HealthTickResult.None)]
	[DataRow(true, -11.0, HealthTickResult.Dead)]
	public void ApplicableMerit_BypassesOnlyHitPointUnconsciousness(bool applies, double hp, HealthTickResult expected)
	{
		var (actor, body, world, strategy, wounds, _) = Fixture(true);
		NegativeHealth(strategy);
		var merit = new Mock<IIgnoreHitPointUnconsciousnessMerit>();
		merit.Setup(x => x.Applies(actor.Object)).Returns(applies);
		actor.SetupGet(x => x.Merits).Returns([merit.Object]);
		wounds.Add(LoadWound(actor.Object, body.Object, world.Object, 100 - hp, 0));
		Assert.AreEqual(expected, strategy.EvaluateStatus(actor.Object));
		if (hp <= -11) return;
		Set(strategy, nameof(strategy.UseStun), true);
		strategy.MaximumStunExpression = Constant(actor.Object, 100);
		wounds[0].CurrentStun = 100;
		Assert.AreEqual(HealthTickResult.Unconscious, strategy.EvaluateStatus(actor.Object));
		body.Setup(x => x.OrganFunction<BrainProto>()).Returns(0);
		Assert.AreEqual(HealthTickResult.Dead, strategy.EvaluateStatus(actor.Object));
	}

	[TestMethod]
	public void CriticalRecovery_SuspendsOnlyNaturalDamageHealingAndExplicitHealingCanRescue()
	{
		var (actor, body, world, strategy, wounds, _) = Fixture(true);
		NegativeHealth(strategy);
		Set(strategy, nameof(strategy.UseCriticalRecovery), true);
		Set(strategy, nameof(strategy.CriticalRecoveryTrait), Mock.Of<ITraitDefinition>());
		var wound = LoadWound(actor.Object, body.Object, world.Object, 102, 12); wounds.Add(wound);
		Assert.IsFalse(NaturalRecoveryRules.CanRecoverDamage(actor.Object));
		Assert.IsTrue(NaturalRecoveryRules.CanRecoverStun(actor.Object));
		Assert.AreEqual(0, strategy.GetHealingTickAmount(wound, Outcome.Pass, HealthDamageType.Damage));
		Assert.IsTrue(HitPointChanges.Apply(actor.Object, 5));
		Assert.AreEqual(3, strategy.CurrentHitPoints(actor.Object));
		Assert.AreEqual(12, wound.CurrentStun);
		Assert.IsTrue(NaturalRecoveryRules.CanRecoverDamage(actor.Object));
		actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		Assert.IsFalse(HitPointChanges.Apply(actor.Object, 100));
		Assert.AreEqual(3, strategy.CurrentHitPoints(actor.Object));
	}

	[TestMethod]
	public void IncompleteRecoveryConfiguration_DoesNotSuspendNaturalHealing()
	{
		var (actor, body, world, strategy, wounds, _) = Fixture(true);
		NegativeHealth(strategy);
		Set(strategy, nameof(strategy.UseCriticalRecovery), true);
		wounds.Add(LoadWound(actor.Object, body.Object, world.Object, 101, 0));
		Assert.IsTrue(NaturalRecoveryRules.CanRecoverDamage(actor.Object));
		strategy.PerformCriticalRecovery(actor.Object);
		Assert.AreEqual(-1, strategy.CurrentHitPoints(actor.Object));
	}

	[TestMethod]
	[DataRow(-2.0, Difficulty.Normal)]
	[DataRow(-3.0, Difficulty.Hard)]
	public void CriticalRecovery_UsesConfiguredAttributeAndSeverityOncePerInvocation(double hp, Difficulty expected)
	{
		var (actor, body, world, strategy, wounds, _) = Fixture(true);
		NegativeHealth(strategy);
		Set(strategy, nameof(strategy.UseCriticalRecovery), true);
		Set(strategy, nameof(strategy.CriticalRecoveryHealing), 1.0);
		Set(strategy, nameof(strategy.CriticalRecoveryDifficulty), Difficulty.Normal);
		Set(strategy, nameof(strategy.MortalRecoveryDifficulty), Difficulty.Hard);
		var trait = Mock.Of<ITraitDefinition>();
		Set(strategy, nameof(strategy.CriticalRecoveryTrait), trait);
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(actor.Object, expected, trait, null, 0.0, TraitUseType.Practical,
			It.IsAny<(string, object)[]>())).Returns(new CheckOutcome { Outcome = Outcome.Pass });
		world.Setup(x => x.GetCheck(CheckType.GenericAttributeCheck)).Returns(check.Object);
		wounds.Add(LoadWound(actor.Object, body.Object, world.Object, 100 - hp, 0));
		strategy.PerformCriticalRecovery(actor.Object);
		Assert.AreEqual(hp + 1, strategy.CurrentHitPoints(actor.Object));
		check.Verify(x => x.Check(actor.Object, expected, trait, null, 0.0, TraitUseType.Practical,
			It.IsAny<(string, object)[]>()), Times.Once);
	}

	private static HealingSimpleWound LoadWound(ICharacter actor, IBody body, IFuturemud world, double damage, double stun) =>
		new(actor, new MudSharp.Models.Wound { Id = 1, CurrentDamage = damage, OriginalDamage = damage, CurrentStun = stun,
			DamageType = (int)DamageType.Cellular, ExtraInformation = "<Definition/>" }, world, body);

	private static (Mock<ICharacter>, Mock<IBody>, Mock<IFuturemud>, BrainHitpointsStrategy, List<IWound>, List<IMaximumHitPointsBonusEffect>) Fixture(bool separate)
	{
		var actor = new Mock<ICharacter>(); var body = new Mock<IBody>(); var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		world.SetupGet(x => x.BodypartPrototypes).Returns(new All<IBodypart>());
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Body).Returns(body.Object); actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		body.SetupGet(x => x.Actor).Returns(actor.Object); body.SetupGet(x => x.Gameworld).Returns(world.Object);
		var brain = TestObjectFactory.CreateUninitialized<BrainProto>(); body.SetupGet(x => x.Organs).Returns(new[] { brain });
		body.Setup(x => x.OrganFunction<BrainProto>()).Returns(1.0);
		var wounds = new List<IWound>(); actor.SetupGet(x => x.Wounds).Returns(() => wounds); body.SetupGet(x => x.Wounds).Returns(() => wounds);
		var modifiers = new List<IMaximumHitPointsBonusEffect>();
		body.Setup(x => x.EffectsOfType<IMaximumHitPointsBonusEffect>(It.IsAny<Predicate<IMaximumHitPointsBonusEffect>>())).Returns(() => modifiers);
		var strategy = (BrainHitpointsStrategy)RuntimeHelpers.GetUninitializedObject(typeof(BrainHitpointsStrategy));
		strategy.MaximumHitPointsExpression = Constant(actor.Object, 100);
		typeof(BrainHitpointsStrategy).GetProperty("SeparateCapacityPenalties")!.SetValue(strategy, separate);
		strategy.CriticalInjuryThreshold = 0.9;
		actor.SetupGet(x => x.HealthStrategy).Returns(strategy);
		return (actor, body, world, strategy, wounds, modifiers);
	}
	private static ITraitExpression Constant(ICharacter actor, double value)
	{
		var expression = new Mock<ITraitExpression>(); expression.Setup(x => x.Evaluate(actor, null, TraitBonusContext.None)).Returns(value);
		return expression.Object;
	}
}
