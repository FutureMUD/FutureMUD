#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Effects;
using MudSharp.Health;
using System;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DrugModifierRuntimeTests
{
	[TestMethod]
	public void CharacterTraitModifier_IsIgnoredWhenItsSourceBodyIsNotCurrent()
	{
		var world = new Mock<IFuturemud>();
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Id).Returns(22L);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		var effect = new DrugTraitModifierEffect(actor.Object) { SourceBodyId = 11L };

		Assert.IsFalse(effect.Applies(actor.Object));
		effect.SourceBodyId = 22L;
		Assert.IsTrue(effect.Applies(actor.Object));
	}

	[TestMethod]
	public void OverdoseThreshold_UsesCurrentTraitAndTreatsZeroTraitAsLethalAtAnyPositiveDose()
	{
		Assert.IsFalse(ConcreteBody.IsDrugOverdoseThresholdReached(0.0, 10.0, 0.002));
		Assert.IsFalse(ConcreteBody.IsDrugOverdoseThresholdReached(0.019, 10.0, 0.002));
		Assert.IsTrue(ConcreteBody.IsDrugOverdoseThresholdReached(0.02, 10.0, 0.002));
		Assert.IsTrue(ConcreteBody.IsDrugOverdoseThresholdReached(0.001, 0.0, 0.002));
	}

	[TestMethod]
	public void SharedOverdoseGroup_CombinesEachConfiguredDrugDoseOnce()
	{
		var configurations = new[] { (DrugId: 1L, Group: "Armageddon Spice"), (DrugId: 2L, Group: "armageddon spice") };
		var activeDoses = new[] { (DrugId: 1L, Grams: 3.0), (DrugId: 2L, Grams: 3.0), (DrugId: 2L, Grams: 1.0) };

		Assert.AreEqual(3.0, ConcreteBody.ActiveGramsForOverdoseGroup(configurations.Take(1), activeDoses, "Armageddon Spice"));
		Assert.AreEqual(7.0, ConcreteBody.ActiveGramsForOverdoseGroup(configurations, activeDoses, "ARMAGEDDON SPICE"));
		Assert.AreEqual(0.0, ConcreteBody.ActiveGramsForOverdoseGroup(configurations, activeDoses, "Other"));
		Assert.IsFalse(ConcreteBody.IsDrugOverdoseThresholdReached(3.0, 5.0, 1.0));
		Assert.IsTrue(ConcreteBody.IsDrugOverdoseThresholdReached(
			ConcreteBody.ActiveGramsForOverdoseGroup(configurations, activeDoses, "Armageddon Spice"), 5.0, 1.0));
	}

	[TestMethod]
	public void ModifierNeutralisation_ScalesCombinedEffectiveIntensityAndNeverGoesNegative()
	{
		Assert.AreEqual(1.0, ConcreteBody.NetDrugModifierIntensityScale(20.0, 0.0), 0.000001);
		Assert.AreEqual(0.5, ConcreteBody.NetDrugModifierIntensityScale(20.0, 10.0), 0.000001);
		Assert.AreEqual(0.0, ConcreteBody.NetDrugModifierIntensityScale(20.0, 30.0), 0.000001);
		Assert.AreEqual(0.0, ConcreteBody.NetDrugModifierIntensityScale(0.0, 5.0), 0.000001);
	}

	[TestMethod]
	public void ResourceCapacityModifierUpdate_NotifiesMagicCapacityReconciliation()
	{
		var casting = new Mock<IMagicCastingService>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.MagicCasting).Returns(casting.Object);
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.Gameworld).Returns(world.Object);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		var handler = new EffectHandler(body.Object);
		var effect = new DrugResourceCapacityModifierEffect(body.Object, 42);

		handler.AddEffect(effect);
		casting.Invocations.Clear();
		effect.MagicResourceCapacityMultiplier = 0.5;
		handler.NotifyEffectUpdated(effect);

		casting.Verify(x => x.NotifyCapacityChange(actor.Object), Times.Once);
	}
}
