#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Wounds;
using MudSharp.Testing.EnvironmentalMagic;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SimpleLivingDirectHealthCostTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DirectHealthContract_DispatchesOrganicImplementationThroughIHealthStrategy(bool complex)
	{
		using NativeHealthFixture fixture = new(complex: complex);
		IHealthStrategy strategy = fixture.Strategy;

		Assert.AreEqual(DirectHealthCostChannels.Damage | DirectHealthCostChannels.Pain |
			DirectHealthCostChannels.Stun, strategy.SupportedDirectHealthCostChannels);
		Assert.IsTrue(strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object, 6.0, 9.0, 12.0,
			WoundSeverity.Horrifying, out DirectHealthCostPlan plan, out string error), error);
		Assert.IsInstanceOfType(strategy.ApplyDirectHealthCost(fixture.Actor.Object, plan).Single(),
			typeof(SimpleOrganicWound));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DirectHealthPlan_UsesNativeModifiersAndUpdatesAnExistingCellularWound(bool complex)
	{
		using NativeHealthFixture fixture = new(complex: complex);
		IHealthStrategy strategy = fixture.Strategy;

		Assert.IsTrue(strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object, 6.0, 9.0, 12.0,
			WoundSeverity.Horrifying, out DirectHealthCostPlan firstPlan, out string firstError), firstError);
		Assert.AreEqual(3.0, firstPlan.DamageInput, 0.000001);
		Assert.AreEqual(3.0, firstPlan.PainInput, 0.000001);
		Assert.AreEqual(3.0, firstPlan.StunInput, 0.000001);

		IWound first = strategy.ApplyDirectHealthCost(fixture.Actor.Object, firstPlan).Single();
		fixture.Wounds.Add(first);
		Assert.IsInstanceOfType(first, typeof(ILateInitialisingItem));
		fixture.Saves.Verify(x => x.AddInitialisation((ILateInitialisingItem)first), Times.Once,
			"A new native wound must enter the established late-initialisation lifecycle.");
		Assert.AreEqual(6.0, first.CurrentDamage, 0.000001);
		Assert.AreEqual(9.0, first.CurrentPain, 0.000001);
		Assert.AreEqual(12.0, first.CurrentStun, 0.000001);

		Assert.IsTrue(strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object, 6.0, 9.0, 12.0,
			WoundSeverity.Horrifying, out DirectHealthCostPlan existingPlan, out string existingError), existingError);
		Assert.AreSame(first, existingPlan.ExistingWound);
		Assert.AreEqual(6.0, existingPlan.DamageInput, 0.000001,
			"Existing native wounds use their direct channel units rather than applying body-part modifiers again.");

		IWound existing = strategy.ApplyDirectHealthCost(fixture.Actor.Object, existingPlan).Single();
		Assert.AreSame(first, existing);
		Assert.AreEqual(12.0, first.CurrentDamage, 0.000001);
		Assert.AreEqual(18.0, first.CurrentPain, 0.000001);
		Assert.AreEqual(24.0, first.CurrentStun, 0.000001);
		fixture.Saves.Verify(x => x.AddInitialisation(It.IsAny<ILateInitialisingItem>()), Times.Once,
			"Increasing an existing cellular wound must not queue a duplicate wound record.");
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DirectHealthPlan_RefusesANearCapPartBeforeMutatingAnyNativeWound(bool complex)
	{
		using NativeHealthFixture fixture = new(capacity: 10.0, complex: complex);
		IHealthStrategy strategy = fixture.Strategy;

		Assert.IsTrue(strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object, 8.0, 0.0, 0.0,
			WoundSeverity.Horrifying, out DirectHealthCostPlan firstPlan, out string firstError), firstError);
		IWound wound = strategy.ApplyDirectHealthCost(fixture.Actor.Object, firstPlan).Single();
		fixture.Wounds.Add(wound);

		Assert.IsFalse(strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object, 3.0, 0.0, 0.0,
			WoundSeverity.Horrifying, out _, out _));
		Assert.AreEqual(8.0, wound.CurrentDamage, 0.000001);
		Assert.AreEqual(0.0, wound.CurrentPain, 0.000001);
		Assert.AreEqual(0.0, wound.CurrentStun, 0.000001);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ConfiguredConsequence_OverwhelmsExistingStunWhilePreservingOtherChannelsAndHealthFloor(bool complex)
	{
		using NativeHealthFixture fixture = new(complex: complex);
		fixture.Actor.SetupGet(x => x.HealthStrategy).Returns(fixture.Strategy);
		fixture.Body.SetupGet(x => x.MaximumStamina).Returns(100);
		fixture.Body.SetupGet(x => x.CurrentStamina).Returns(100);
		Assert.IsTrue(fixture.Strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object, 0, 0, 80,
			WoundSeverity.Horrifying, out var initial, out var error), error);
		var wound = fixture.Strategy.ApplyDirectHealthCost(fixture.Actor.Object, initial).Single();
		fixture.Wounds.Add(wound);
		Assert.IsFalse(MudSharp.Magic.ConfiguredItemConsequence.CanApply(fixture.Actor.Object, new MudSharp.Magic.ItemConsequence(Stun: 40)), "Voluntary payment still refuses incapacitating prices.");
		Assert.IsTrue(MudSharp.Magic.ConfiguredItemConsequence.Apply(fixture.Actor.Object, new MudSharp.Magic.ItemConsequence(20, 40, 40, 10, true)));
		Assert.AreEqual(20, wound.CurrentDamage, 0.000001);
		Assert.AreEqual(120, wound.CurrentStun, 0.000001);
		fixture.Body.Verify(x => x.SpendStamina(40), Times.Once);
		Assert.IsTrue(MudSharp.Magic.ConfiguredItemConsequence.Apply(fixture.Actor.Object, new MudSharp.Magic.ItemConsequence(Health: 200, HealthFloor: 10)));
		Assert.AreEqual(90, wound.CurrentDamage, 0.000001);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ConfiguredConsequence_FullHealthDamageExceedsInitialPartCapWithoutDuplicatingStun(bool complex)
	{
		using NativeHealthFixture fixture = new(capacity: 10, complex: complex);
		fixture.Body.SetupGet(x => x.CurrentStamina).Returns(100);
		fixture.Actor.Setup(x => x.AddWounds(It.IsAny<IEnumerable<IWound>>()))
			.Callback<IEnumerable<IWound>>(wounds => fixture.Wounds.AddRange(wounds));
		Assert.IsFalse(fixture.Strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object, 100, 0, 40,
			WoundSeverity.Horrifying, out _, out _), "Voluntary admission must still respect part capacity.");
		Assert.IsTrue(MudSharp.Magic.ConfiguredItemConsequence.Apply(fixture.Actor.Object,
			new MudSharp.Magic.ItemConsequence(HealthFraction: 1, Stun: 40)));
		var wound = fixture.Wounds.Single();
		Assert.AreEqual(100, wound.CurrentDamage, 0.000001);
		Assert.AreEqual(40, wound.CurrentStun, 0.000001);
		Assert.AreEqual(0, wound.CurrentPain, 0.000001);
		fixture.Saves.Verify(x => x.AddInitialisation((ILateInitialisingItem)wound), Times.Once);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ConfiguredConsequence_AttachesNewWoundsAndReusesThemOnRepeatedConsequences(bool complex)
	{
		using NativeHealthFixture fixture = new(complex: complex);
		fixture.Body.SetupGet(x => x.CurrentStamina).Returns(100);
		fixture.Actor.Setup(x => x.AddWounds(It.IsAny<IEnumerable<IWound>>()))
			.Callback<IEnumerable<IWound>>(wounds => fixture.Wounds.AddRange(wounds));
		var consequence = new MudSharp.Magic.ItemConsequence(20, 40, 10, 10);
		Assert.IsTrue(MudSharp.Magic.ConfiguredItemConsequence.Apply(fixture.Actor.Object, consequence));
		var wound = fixture.Wounds.Single();
		Assert.AreEqual(20, wound.CurrentDamage, 0.000001);
		Assert.AreEqual(40, wound.CurrentStun, 0.000001);
		Assert.IsTrue(MudSharp.Magic.ConfiguredItemConsequence.Apply(fixture.Actor.Object, consequence));
		Assert.AreSame(wound, fixture.Wounds.Single());
		Assert.AreEqual(40, wound.CurrentDamage, 0.000001);
		Assert.AreEqual(80, wound.CurrentStun, 0.000001);
		fixture.Actor.Verify(x => x.ProcessPassiveWound(wound), Times.Exactly(2));
	}

	[TestMethod]
	public void HitpointConsequence_UsesExactNativeChannelsAndPreservesHealthFloor()
	{
		using NativeHealthFixture fixture = new(capacity: 10, hitpoints: true);
		fixture.Actor.Setup(x => x.AddWounds(It.IsAny<IEnumerable<IWound>>()))
			.Callback<IEnumerable<IWound>>(wounds => fixture.Wounds.AddRange(wounds));
		fixture.Body.SetupGet(x => x.CurrentStamina).Returns(100);
		Assert.IsTrue(MudSharp.Magic.ConfiguredItemConsequence.Apply(fixture.Actor.Object,
			new MudSharp.Magic.ItemConsequence(15, 20, 30, 10, true)));
		var wound = fixture.Wounds.Single();
		Assert.IsInstanceOfType<MudSharp.Health.Wounds.HealingSimpleWound>(wound);
		Assert.AreEqual(15, wound.CurrentDamage, 1e-6);
		Assert.AreEqual(20, wound.CurrentStun, 1e-6);
		fixture.Body.Verify(x => x.SpendStamina(30), Times.Once);
		Assert.IsTrue(MudSharp.Magic.ConfiguredItemConsequence.Apply(fixture.Actor.Object,
			new MudSharp.Magic.ItemConsequence(200, 100, HealthFloor: 10)));
		Assert.AreSame(wound, fixture.Wounds.Single());
		Assert.AreEqual(90, wound.CurrentDamage, 1e-6);
		Assert.AreEqual(120, wound.CurrentStun, 1e-6);
		Assert.IsTrue(MudSharp.Magic.ConfiguredItemConsequence.Apply(fixture.Actor.Object,
			new MudSharp.Magic.ItemConsequence(200, 20, HealthFloor: 10)));
		Assert.AreEqual(90, wound.CurrentDamage, 1e-6);
		Assert.AreEqual(140, wound.CurrentStun, 1e-6);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void HitpointCosts_RespectOptionalStunAndVoluntaryCapacity(bool stun)
	{
		using NativeHealthFixture fixture = new(hitpoints: true, stun: stun);
		Assert.IsFalse(fixture.Strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object,
			0, 1, 0, WoundSeverity.Horrifying, out _, out _));
		Assert.AreEqual(stun, fixture.Strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object,
			0, 0, 20, WoundSeverity.Horrifying, out _, out _));
		Assert.IsFalse(fixture.Strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object,
			101, 0, 0, WoundSeverity.Horrifying, out _, out _));
		Assert.IsTrue(fixture.Strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object,
			15, 0, 0, WoundSeverity.Horrifying, out var plan, out var error), error);
		var wound = fixture.Strategy.ApplyDirectHealthCost(fixture.Actor.Object, plan).Single();
		Assert.AreEqual(15, wound.CurrentDamage, 1e-6);
		Assert.AreEqual(0, wound.CurrentStun);
	}

	[TestMethod]
	public void HitpointCosts_CapacityOnlyPenaltyChargesVisibleHealthExactly()
	{
		using NativeHealthFixture fixture = new(hitpoints: true);
		typeof(BrainHitpointsStrategy).GetProperty("SeparateCapacityPenalties")!.SetValue(fixture.Strategy, true);
		var penalty = new MudSharp.Effects.Concrete.TimedMaximumHitPointsEffect(fixture.Body.Object, -20, 1, capacityOnly: true);
		fixture.Body.Setup(x => x.EffectsOfType<IMaximumHitPointsBonusEffect>(It.IsAny<Predicate<IMaximumHitPointsBonusEffect>>()))
			.Returns(new[] { penalty });
		Assert.AreEqual(80, fixture.Strategy.CurrentHitPoints(fixture.Actor.Object));
		Assert.IsTrue(fixture.Strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object,
			15, 0, 0, WoundSeverity.Horrifying, out var plan, out var error), error);
		Assert.AreEqual(15, plan.ExpectedDamage);
		fixture.Wounds.AddRange(fixture.Strategy.ApplyDirectHealthCost(fixture.Actor.Object, plan));
		Assert.AreEqual(65, fixture.Strategy.CurrentHitPoints(fixture.Actor.Object));
		Assert.IsTrue(fixture.Strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object,
			10, 0, 0, WoundSeverity.Horrifying, out plan, out error), error);
		fixture.Strategy.ApplyDirectHealthCost(fixture.Actor.Object, plan);
		Assert.AreEqual(55, fixture.Strategy.CurrentHitPoints(fixture.Actor.Object));
	}

	[TestMethod]
	public void HitPointAdjustment_UnsupportedStunDoesNotPartiallyDamage()
	{
		using NativeHealthFixture fixture = new(hitpoints: true, stun: false);
		Assert.IsFalse(HitPointChanges.Apply(fixture.Actor.Object, -20, 30));
		Assert.AreEqual(100, fixture.Strategy.CurrentHitPoints(fixture.Actor.Object));
		Assert.AreEqual(0, fixture.Wounds.Count);
	}

	[TestMethod]
	public void HitPointAdjustment_AppliesDamageAndStunTogether_RejectsMixedHealing()
	{
		using NativeHealthFixture fixture = new(hitpoints: true, stun: true);
		fixture.Actor.Setup(x => x.AddWounds(It.IsAny<IEnumerable<IWound>>()))
			.Callback<IEnumerable<IWound>>(wounds => fixture.Wounds.AddRange(wounds));
		Assert.IsTrue(HitPointChanges.Apply(fixture.Actor.Object, -20, 30));
		Assert.AreEqual(80, fixture.Strategy.CurrentHitPoints(fixture.Actor.Object));
		Assert.AreEqual(30, fixture.Wounds.Sum(x => x.CurrentStun));
		Assert.IsFalse(HitPointChanges.Apply(fixture.Actor.Object, 5, 10));
		Assert.AreEqual(80, fixture.Strategy.CurrentHitPoints(fixture.Actor.Object));
	}

	[TestMethod]
	public void HitPointAdjustment_RepairsAndHarmsThroughNegativeRangeWithoutChangingStun()
	{
		using NativeHealthFixture fixture = new(capacity: 10, hitpoints: true);
		var strategy = (BrainHitpointsStrategy)fixture.Strategy;
		typeof(BrainHitpointsStrategy).GetProperty(nameof(strategy.UseNegativeHitPoints))!.SetValue(strategy, true);
		fixture.Body.Setup(x => x.OrganFunction<BrainProto>()).Returns(1);
		fixture.Actor.Setup(x => x.AddWounds(It.IsAny<IEnumerable<IWound>>()))
			.Callback<IEnumerable<IWound>>(wounds => fixture.Wounds.AddRange(wounds));
		var state = CharacterState.Awake;
		fixture.Actor.SetupGet(x => x.State).Returns(() => state);
		fixture.Actor.Setup(x => x.CheckHealthStatus()).Callback(() => state = strategy.EvaluateStatus(fixture.Actor.Object) switch
		{
			HealthTickResult.Dead => CharacterState.Dead,
			HealthTickResult.Unconscious => CharacterState.Unconscious,
			_ => CharacterState.Awake
		});
		Assert.IsTrue(HitPointChanges.Apply(fixture.Actor.Object, -106));
		Assert.AreEqual(-6, strategy.CurrentHitPoints(fixture.Actor.Object));
		Assert.AreEqual(HealthTickResult.Unconscious, strategy.EvaluateStatus(fixture.Actor.Object));
		Assert.AreEqual(0, fixture.Wounds.Sum(x => x.CurrentStun));
		Assert.IsTrue(HitPointChanges.Apply(fixture.Actor.Object, 9));
		Assert.AreEqual(3, strategy.CurrentHitPoints(fixture.Actor.Object));
		Assert.AreEqual(HealthTickResult.None, strategy.EvaluateStatus(fixture.Actor.Object));
		Assert.IsTrue(HitPointChanges.Apply(fixture.Actor.Object, -9));
		Assert.IsTrue(HitPointChanges.Apply(fixture.Actor.Object, -5));
		Assert.AreEqual(-11, strategy.CurrentHitPoints(fixture.Actor.Object));
		Assert.AreEqual(HealthTickResult.Dead, strategy.EvaluateStatus(fixture.Actor.Object));
		Assert.AreEqual(CharacterState.Dead, fixture.Actor.Object.State);
		Assert.IsFalse(HitPointChanges.Apply(fixture.Actor.Object, 100));
		Assert.AreEqual(-11, strategy.CurrentHitPoints(fixture.Actor.Object));
		fixture.Actor.Verify(x => x.CheckHealthStatus(), Times.Exactly(4));
		Assert.AreEqual(1, fixture.Wounds.Count);
	}

	[TestMethod]
	public void CriticalRecovery_FailedCheckCanKillAtBoundaryAndDoesNotRunForDeadCharacters()
	{
		using NativeHealthFixture fixture = new(hitpoints: true);
		var strategy = (BrainHitpointsStrategy)fixture.Strategy;
		typeof(BrainHitpointsStrategy).GetProperty(nameof(strategy.UseNegativeHitPoints))!.SetValue(strategy, true);
		typeof(BrainHitpointsStrategy).GetProperty(nameof(strategy.UseCriticalRecovery))!.SetValue(strategy, true);
		var trait = Mock.Of<ITraitDefinition>();
		typeof(BrainHitpointsStrategy).GetProperty(nameof(strategy.CriticalRecoveryTrait))!.SetValue(strategy, trait);
		fixture.Body.Setup(x => x.OrganFunction<BrainProto>()).Returns(1);
		fixture.Actor.Setup(x => x.AddWounds(It.IsAny<IEnumerable<IWound>>()))
			.Callback<IEnumerable<IWound>>(wounds => fixture.Wounds.AddRange(wounds));
		Assert.IsTrue(HitPointChanges.Apply(fixture.Actor.Object, -110));
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(fixture.Actor.Object, Difficulty.Hard, trait, null, 0.0, TraitUseType.Practical,
			It.IsAny<(string, object)[]>())).Returns(new CheckOutcome { Outcome = Outcome.Fail });
		Mock.Get(fixture.Actor.Object.Gameworld).Setup(x => x.GetCheck(CheckType.GenericAttributeCheck)).Returns(check.Object);
		strategy.PerformCriticalRecovery(fixture.Actor.Object);
		Assert.AreEqual(-11, strategy.CurrentHitPoints(fixture.Actor.Object));
		Assert.AreEqual(HealthTickResult.Dead, strategy.EvaluateStatus(fixture.Actor.Object));
		fixture.Actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		strategy.PerformCriticalRecovery(fixture.Actor.Object);
		check.Verify(x => x.Check(fixture.Actor.Object, Difficulty.Hard, trait, null, 0.0, TraitUseType.Practical,
			It.IsAny<(string, object)[]>()), Times.Once);
	}

	[TestMethod]
	public void HitPointPolicy_SaveLoadPreservesOptionalThresholdsAndDefaultRemainsDisabled()
	{
		using NativeHealthFixture fixture = new(hitpoints: true);
		var strategy = (BrainHitpointsStrategy)fixture.Strategy;
		Assert.IsFalse(strategy.UseNegativeHitPoints);
		Assert.IsFalse(strategy.UseCriticalRecovery);
		typeof(BrainHitpointsStrategy).GetProperty(nameof(strategy.UseNegativeHitPoints))!.SetValue(strategy, true);
		typeof(BrainHitpointsStrategy).GetProperty(nameof(strategy.DeathHitPoints))!.SetValue(strategy, -15.0);
		var definition = (XElement)typeof(BaseHealthStrategy).GetMethod("SaveDefinition", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(strategy, null)!;
		var loaded = (BrainHitpointsStrategy)BaseHealthStrategy.LoadStrategy(new Db.HealthStrategy
		{
			Id = 2, Name = "Reloaded HP", Type = "BrainHitpoints", Definition = definition.ToString()
		}, fixture.Actor.Object.Gameworld);
		Assert.IsTrue(loaded.UseNegativeHitPoints);
		Assert.AreEqual(-15, loaded.DeathHitPoints);
		Assert.AreEqual(-3, loaded.MortallyWoundedHitPoints);
		Assert.AreEqual(0, loaded.UnconsciousHitPoints);
		Assert.IsFalse(loaded.UseCriticalRecovery);
		Assert.AreEqual(Difficulty.Normal, loaded.CriticalRecoveryDifficulty);
		Assert.AreEqual(Difficulty.Hard, loaded.MortalRecoveryDifficulty);
		Assert.AreEqual(1, loaded.CriticalRecoveryHealing);
		Assert.AreEqual(1, loaded.CriticalRecoveryDamage);
	}

	private sealed class NativeHealthFixture : IDisposable
	{
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IBody> Body { get; } = new();
		public Mock<IBodypart> Bodypart { get; } = new();
		public Mock<ISaveManager> Saves { get; } = new();
		public List<IWound> Wounds { get; } = [];
		public IHealthStrategy Strategy { get; }

		public NativeHealthFixture(double capacity = 100.0, bool complex = false, bool hitpoints = false, bool stun = true)
		{
			Mock<IFuturemud> gameworld = new() { DefaultValue = DefaultValue.Mock };
			gameworld.SetupGet(x => x.SaveManager).Returns(Saves.Object);

			Mock<ITraitExpression> expression = new();
			expression.SetupGet(x => x.Id).Returns(1L);
			expression.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), It.IsAny<ITraitDefinition>(),
				It.IsAny<TraitBonusContext>())).Returns(100.0);
			var expressions = new EnvironmentalMagicTestRegistry<ITraitExpression>();
			expressions.Add(expression.Object);
			gameworld.SetupGet(x => x.TraitExpressions).Returns(expressions);

			Bodypart.SetupGet(x => x.Id).Returns(10L);
			Bodypart.SetupGet(x => x.DamageModifier).Returns(2.0);
			Bodypart.SetupGet(x => x.PainModifier).Returns(3.0);
			Bodypart.SetupGet(x => x.StunModifier).Returns(4.0);
			Body.SetupGet(x => x.Bodyparts).Returns(() => [Bodypart.Object]);
			Body.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
			Body.Setup(x => x.HitpointsForBodypart(Bodypart.Object)).Returns(capacity);

			Mock<IRace> race = new();
			race.SetupGet(x => x.PainToleranceMultiplier).Returns(1.0);
			race.SetupGet(x => x.BloodLiquid).Returns((MudSharp.Form.Material.ILiquid)null!);
			Actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.SetupGet(x => x.Race).Returns(race.Object);
			Actor.SetupGet(x => x.Wounds).Returns(() => Wounds);
			Actor.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());

			Strategy = LoadStrategy(gameworld.Object, complex, hitpoints, stun);
			Actor.SetupGet(x => x.HealthStrategy).Returns(Strategy);
		}

		private static IHealthStrategy LoadStrategy(IFuturemud gameworld, bool complex, bool hitpoints, bool stun)
		{
			BrainHitpointsStrategy.RegisterHealthStrategyLoader();
			SimpleLivingHealthStrategy.RegisterHealthStrategyLoader();
			ComplexLivingHealthStrategy.RegisterHealthStrategyLoader();
			var definition = new XElement("Definition",
				new XElement("UseStun", stun),
				new XElement("UseCellularDamage", false),
				new XElement("MaximumHitPointsExpression", 1),
				new XElement("MaximumStunExpression", 1),
				new XElement("MaximumPainExpression", 1),
				new XElement("HealingTickDamageExpression", 1),
				new XElement("HealingTickStunExpression", 1),
				new XElement("HealingTickPainExpression", 1));
			return BaseHealthStrategy.LoadStrategy(new Db.HealthStrategy
			{
				Id = 1,
				Name = "Native direct-health test strategy",
				Type = hitpoints ? "BrainHitpoints" : complex ? "ComplexLiving" : "SimpleLiving",
				Definition = definition.ToString()
			}, gameworld);
		}

		public void Dispose()
		{
		}
	}
}
