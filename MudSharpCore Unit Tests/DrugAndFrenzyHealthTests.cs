#nullable enable

using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Health.Strategies;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DrugAndFrenzyHealthTests
{
	[TestMethod]
	[DataRow(typeof(SimpleLivingHealthStrategy))]
	[DataRow(typeof(ComplexLivingHealthStrategy))]
	[DataRow(typeof(BrainConstructHealthStrategy))]
	[DataRow(typeof(BrainHitpointsStrategy))]
	[DataRow(typeof(RobotHealthStrategy))]
	public void MaximumHitPoints_AppliesDependenceLossBeforeWithdrawalMultiplier(Type strategyType)
	{
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		var modifier = Mock.Of<IMaximumHitPointsBonusEffect>(x =>
			x.MaximumHitPointsBonus == -50.0 && x.MaximumHitPointsMultiplier == 0.5);
		body.Setup(x => x.EffectsOfType<IMaximumHitPointsBonusEffect>(It.IsAny<Predicate<IMaximumHitPointsBonusEffect>>()))
			.Returns(new[] { modifier });
		var strategy = (IHealthStrategy)RuntimeHelpers.GetUninitializedObject(strategyType);
		strategyType.GetProperty("MaximumHitPointsExpression")!.SetValue(strategy, Constant(actor.Object, 100.0));
		Assert.AreEqual(25.0, strategy.MaxHP(actor.Object));
	}

	[TestMethod]
	[DataRow(false, 100.0, 0.0, 0.0, HealthTickResult.Unconscious)]
	[DataRow(true, 100.0, 0.0, 0.0, HealthTickResult.None)]
	[DataRow(true, 0.0, 100.0, 0.0, HealthTickResult.PassOut)]
	[DataRow(true, 0.0, 0.0, 120.0, HealthTickResult.Dead)]
	public void Frenzy_PreventsStunUnconsciousnessButPreservesPainAndHpConsequences(bool frenzy,
		double stun, double pain, double damage, HealthTickResult expected)
	{
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		body.SetupGet(x => x.Id).Returns(123L);
		body.SetupGet(x => x.TotalBloodVolumeLitres).Returns(5.0);
		body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5.0);
		body.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
		body.Setup(x => x.HitpointsForBodypart(It.IsAny<IBodypart>())).Returns(100.0);
		var brain = TestObjectFactory.CreateUninitialized<BrainProto>();
		var heart = TestObjectFactory.CreateUninitialized<HeartProto>();
		body.SetupGet(x => x.Organs).Returns(new IOrganProto[] { brain, heart });
		var wound = Mock.Of<IWound>(x => x.CurrentDamage == damage && x.CurrentPain == pain && x.CurrentStun == stun);
		actor.SetupGet(x => x.Wounds).Returns(new[] { wound });
		body.SetupGet(x => x.Wounds).Returns(new[] { wound });
		Assert.AreEqual(1.0, brain.OrganFunctionFactor(body.Object));
		Assert.AreEqual(1.0, heart.OrganFunctionFactor(body.Object));
		var effect = new CombatFrenzyEffect(actor.Object, 15, 15, 5, 4);
		actor.Setup(x => x.EffectsOfType<IPreventStunUnconsciousnessEffect>(It.IsAny<Predicate<IPreventStunUnconsciousnessEffect>>()))
			.Returns(frenzy ? new IPreventStunUnconsciousnessEffect[] { effect } : Array.Empty<IPreventStunUnconsciousnessEffect>());
		var strategy = TestObjectFactory.CreateUninitialized<SimpleLivingHealthStrategy>();
		strategy.MaximumHitPointsExpression = Constant(actor.Object, 100);
		strategy.MaximumStunExpression = Constant(actor.Object, 100);
		strategy.MaximumPainExpression = Constant(actor.Object, 100);
		strategy.HPGrace = 1.1;
		strategy.BloodLossDeadThreshold = 0.5;
		strategy.BrainActivityUnconsciousThreshold = 0.9;
		strategy.BloodLossUnconsciousThreshold = 0.6;
		strategy.AnesthesiaDeathThreshold = 10;
		strategy.AnesthesiaUnconsciousThreshold = 1;
		Assert.AreEqual(expected, strategy.EvaluateStatus(actor.Object));
	}

	[TestMethod]
	[DataRow(BuiltInCombatMoveType.UseWeaponAttack, DamageType.Crushing, 5.0)]
	[DataRow(BuiltInCombatMoveType.NaturalWeaponAttack, DamageType.Bite, 5.0)]
	[DataRow(BuiltInCombatMoveType.RangedWeaponAttack, DamageType.Crushing, 0.0)]
	[DataRow(BuiltInCombatMoveType.MagicPowerAttack, DamageType.Crushing, 0.0)]
	[DataRow(BuiltInCombatMoveType.UseWeaponAttack, DamageType.Burning, 0.0)]
	public void FrenzyDamageBonus_UsesOnlyPhysicalArmedAndNaturalMelee(BuiltInCombatMoveType moveType,
		DamageType damageType, double expected)
	{
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		body.SetupGet(x => x.Id).Returns(123L);
		var effect = new CombatFrenzyEffect(actor.Object, 15, 15, 5, 4);
		actor.Setup(x => x.EffectsOfType<IFixedMeleeDamageBonusEffect>(It.IsAny<Predicate<IFixedMeleeDamageBonusEffect>>()))
			.Returns(new IFixedMeleeDamageBonusEffect[] { effect });
		var move = TestObjectFactory.CreateUninitialized<MeleeWeaponAttack>();
		typeof(CombatMoveBase).GetProperty(nameof(move.Assailant))!.SetValue(move, actor.Object);
		Assert.AreEqual(expected, move.CombatFrenzyMeleeDamageBonus(moveType, damageType));
	}

	private static ITraitExpression Constant(ICharacter actor, double value)
	{
		var expression = new Mock<ITraitExpression>();
		expression.Setup(x => x.Evaluate(actor, null, TraitBonusContext.None)).Returns(value);
		return expression.Object;
	}
}
