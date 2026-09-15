#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
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
	public void DirectHealthContract_DispatchesSimpleLivingImplementationThroughIHealthStrategy()
	{
		using NativeHealthFixture fixture = new();
		IHealthStrategy strategy = fixture.Strategy;

		Assert.AreEqual(DirectHealthCostChannels.Damage | DirectHealthCostChannels.Pain |
			DirectHealthCostChannels.Stun, strategy.SupportedDirectHealthCostChannels);
		Assert.IsTrue(strategy.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart.Object, 6.0, 9.0, 12.0,
			WoundSeverity.Horrifying, out DirectHealthCostPlan plan, out string error), error);
		Assert.IsInstanceOfType(strategy.ApplyDirectHealthCost(fixture.Actor.Object, plan).Single(),
			typeof(SimpleOrganicWound));
	}

	[TestMethod]
	public void DirectHealthPlan_UsesNativeModifiersAndUpdatesAnExistingCellularWound()
	{
		using NativeHealthFixture fixture = new();
		SimpleLivingHealthStrategy strategy = fixture.Strategy;

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
	public void DirectHealthPlan_RefusesANearCapPartBeforeMutatingAnyNativeWound()
	{
		using NativeHealthFixture fixture = new(capacity: 10.0);
		SimpleLivingHealthStrategy strategy = fixture.Strategy;

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

	private sealed class NativeHealthFixture : IDisposable
	{
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IBody> Body { get; } = new();
		public Mock<IBodypart> Bodypart { get; } = new();
		public Mock<ISaveManager> Saves { get; } = new();
		public List<IWound> Wounds { get; } = [];
		public SimpleLivingHealthStrategy Strategy { get; }

		public NativeHealthFixture(double capacity = 100.0)
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
			race.SetupGet(x => x.BloodLiquid).Returns((MudSharp.Form.Material.ILiquid)null!);
			Actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.SetupGet(x => x.Race).Returns(race.Object);
			Actor.SetupGet(x => x.Wounds).Returns(() => Wounds);
			Actor.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());

			Strategy = LoadStrategy(gameworld.Object);
			Actor.SetupGet(x => x.HealthStrategy).Returns(Strategy);
		}

		private static SimpleLivingHealthStrategy LoadStrategy(IFuturemud gameworld)
		{
			SimpleLivingHealthStrategy.RegisterHealthStrategyLoader();
			var definition = new XElement("Definition",
				new XElement("MaximumHitPointsExpression", 1),
				new XElement("MaximumStunExpression", 1),
				new XElement("MaximumPainExpression", 1),
				new XElement("HealingTickDamageExpression", 1),
				new XElement("HealingTickStunExpression", 1),
				new XElement("HealingTickPainExpression", 1));
			return (SimpleLivingHealthStrategy)BaseHealthStrategy.LoadStrategy(new Db.HealthStrategy
			{
				Id = 1,
				Name = "Native direct-health test strategy",
				Type = "SimpleLiving",
				Definition = definition.ToString()
			}, gameworld);
		}

		public void Dispose()
		{
		}
	}
}
