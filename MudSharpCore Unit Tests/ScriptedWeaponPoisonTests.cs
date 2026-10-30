using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ScriptedWeaponPoisonTests
{
	[TestMethod]
	[DataRow(0.0, DamageType.Piercing, false)]
	[DataRow(1.0, DamageType.Crushing, false)]
	[DataRow(1.0, DamageType.Piercing, true)]
	public void Delivery_SpendsOnlyOneChargeOnAnEligibleWoundBeforeTheProg(double damage, DamageType type, bool delivers)
	{
		var (world, item, effects, prog) = Fixture();
		Assert.IsTrue(WeaponPoisonDeliveryHelper.AddScriptedPoisonCoating(item.Object, prog.Object));
		Assert.IsFalse(WeaponPoisonDeliveryHelper.AddScriptedPoisonCoating(item.Object, prog.Object));
		var actor = Mock.Of<ICharacter>();
		var victim = new Mock<ICharacter>(); var body = new Mock<IBody>();
		victim.SetupGet(x => x.Body).Returns(body.Object); body.SetupGet(x => x.Actor).Returns(victim.Object);
		var wound = Mock.Of<IWound>(x => x.CurrentDamage == damage && x.DamageType == type &&
			x.Severity == WoundSeverity.Minor && x.ToolOrigin == item.Object && x.Parent == body.Object);
		body.SetupGet(x => x.Wounds).Returns(new[] { wound });
		prog.Setup(x => x.Execute(It.IsAny<object[]>())).Callback(() => Assert.AreEqual(0, effects.Count)).Returns(false);
		WeaponPoisonDeliveryHelper.DeliverFromWeapon(actor, item.Object, [wound, wound]);
		WeaponPoisonDeliveryHelper.DeliverFromWeapon(actor, item.Object, [wound]);
		prog.Verify(x => x.Execute(It.IsAny<object[]>()), delivers ? Times.Once() : Times.Never());
		Assert.AreEqual(delivers ? 0 : 1, effects.Count);
	}

	[TestMethod]
	public void Delivery_DoesNotPoisonAReplacementBodyAfterWoundCallbacks()
	{
		var (_, item, effects, prog) = Fixture();
		Assert.IsTrue(WeaponPoisonDeliveryHelper.AddScriptedPoisonCoating(item.Object, prog.Object));
		var victim = new Mock<ICharacter>(); var replacement = new Mock<IBody>();
		victim.SetupGet(x => x.Body).Returns(replacement.Object);
		replacement.SetupGet(x => x.Actor).Returns(victim.Object);
		replacement.SetupGet(x => x.Wounds).Returns(Array.Empty<IWound>());
		var oldWound = Mock.Of<IWound>(x => x.CurrentDamage == 1.0 && x.DamageType == DamageType.Piercing &&
			x.Severity == WoundSeverity.Minor && x.ToolOrigin == item.Object && x.Parent == victim.Object);
		WeaponPoisonDeliveryHelper.DeliverFromWeapon(Mock.Of<ICharacter>(), item.Object, [oldWound]);
		prog.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never());
		Assert.AreEqual(1, effects.Count);
	}

	[TestMethod]
	public void SingleCharge_TransfersOnceWhenAmmunitionCreatesMultipleProjectiles()
	{
		var (_, source, sourceEffects, prog) = Fixture();
		var (_, first, firstEffects, _) = Fixture();
		var (_, second, secondEffects, _) = Fixture();
		Assert.IsTrue(WeaponPoisonDeliveryHelper.AddScriptedPoisonCoating(source.Object, prog.Object));
		WeaponPoisonDeliveryHelper.CopyPoisonCoating(source.Object, first.Object);
		WeaponPoisonDeliveryHelper.CopyPoisonCoating(source.Object, second.Object);
		Assert.AreEqual(0, sourceEffects.Count);
		Assert.AreEqual(1, firstEffects.Count);
		Assert.AreEqual(0, secondEffects.Count);
		Assert.AreSame(prog.Object, firstEffects.Single().DeliveryProg);
		first.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never());
	}

	[TestMethod]
	public void ScriptedCoating_RoundTripsAndSurvivesMorphWithoutManufacturingLiquid()
	{
		var (_, owner, effects, prog) = Fixture();
		Assert.IsTrue(WeaponPoisonDeliveryHelper.AddScriptedPoisonCoating(owner.Object, prog.Object));
		var effect = (WeaponPoisonCoating)effects.Single();
		var xml = effect.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = (WeaponPoisonCoating)Activator.CreateInstance(typeof(WeaponPoisonCoating),
			BindingFlags.Instance | BindingFlags.NonPublic, null, [xml, owner.Object], null)!;
		Assert.AreSame(prog.Object, loaded.DeliveryProg);
		Assert.IsTrue(loaded.SingleUseOnWound);
		Assert.IsTrue(loaded.ContaminatingLiquid.IsEmpty);
		var (_, target, _, _) = Fixture();
		var morphed = (IWeaponPoisonCoatingEffect)loaded.NewEffectOnItemMorph(owner.Object, target.Object);
		Assert.AreSame(prog.Object, morphed.DeliveryProg);
		Assert.IsTrue(morphed.SingleUseOnWound);
	}

	private static (Mock<IFuturemud> World, Mock<IGameItem> Item, List<IWeaponPoisonCoatingEffect> Effects, Mock<IFutureProg> Prog) Fixture()
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns((string key) =>
			double.Parse(DefaultStaticSettings.DefaultStaticConfigurations[key], System.Globalization.CultureInfo.InvariantCulture));
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(42);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		var progs = new All<IFutureProg>(); progs.Add(prog.Object); world.SetupGet(x => x.FutureProgs).Returns(progs);
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(1);
		item.SetupGet(x => x.Gameworld).Returns(world.Object); item.SetupGet(x => x.Quantity).Returns(1);
		item.Setup(x => x.IsItemType<IMeleeWeapon>()).Returns(true);
		var effects = new List<IWeaponPoisonCoatingEffect>();
		item.Setup(x => x.EffectsOfType<IWeaponPoisonCoatingEffect>(It.IsAny<Predicate<IWeaponPoisonCoatingEffect>>())).Returns(() => effects);
		item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect => effects.Add((IWeaponPoisonCoatingEffect)effect));
		item.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((effect, _) => effects.Remove((IWeaponPoisonCoatingEffect)effect));
		return (world, item, effects, prog);
	}
}
