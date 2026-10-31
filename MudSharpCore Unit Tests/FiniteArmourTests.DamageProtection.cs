#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.Health;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class FiniteArmourTests
{
	[TestMethod]
	public void FlatProtection_UsesRealBodyDamageOnceAndPreservesHalfProtectionAfterTheFirstShield()
	{
		var f = ProtectionFixture();
		var shield = new DamageProtectionStatus(f.Victim.Object, 0, 1);
		var sanctuary = new DamageProtectionStatus(f.Victim.Object, 0.5, 0);
		f.Effects.Add(shield); f.Effects.Add(sanctuary);
		Assert.AreEqual(0, f.Body.PassiveSufferDamage(f.Packet(20)).Count());
		Assert.IsFalse(f.Effects.Contains(shield)); Assert.IsTrue(shield.IsSpent); Assert.IsTrue(f.Effects.Contains(sanctuary));
		var wound = f.Body.PassiveSufferDamage(f.Packet(20)).Single();
		Assert.AreEqual(10.0, wound.CurrentDamage, .000001);
		Assert.IsFalse(sanctuary.IsSpent);
	}

	[TestMethod]
	public void FlatProtection_FullArmourAbsorptionDoesNotConsumeTheShieldAndRepeatedReferencesApplyOnce()
	{
		var f = ProtectionFixture(); var shield = new DamageProtectionStatus(f.Victim.Object, 0, 1); f.Effects.Add(shield);
		var armour = new Mock<IMagicArmour>(); armour.Setup(x => x.Applies()).Returns(true);
		armour.Setup(x => x.AppliesToPart(f.Part.Object)).Returns(true);
		armour.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>(), ref It.Ref<List<IWound>>.IsAny)).Returns((IDamage)null!);
		f.Effects.Add(armour.Object);
		Assert.AreEqual(0, f.Body.PassiveSufferDamage(f.Packet(20)).Count()); Assert.IsFalse(shield.IsSpent);
		Assert.AreEqual(1, shield.RemainingHits);
		f.Effects.Clear(); var sanctuary = new DamageProtectionStatus(f.Victim.Object, 0.5, 0);
		f.Effects.Add(sanctuary); f.Effects.Add(sanctuary);
		IDamage? damage = f.Packet(20); IDamage? inward = null;
		Assert.IsTrue(f.Body.TryApplyDamageProtection(ref damage, ref inward)); Assert.AreEqual(10.0, damage!.DamageAmount);
	}

	[TestMethod]
	public void FlatProtection_ScalesTheExternalInwardPairAndPreservesPacketGuardsWithoutSpendingOnZero()
	{
		var f = ProtectionFixture(); var sanctuary = new DamageProtectionStatus(f.Victim.Object, 0.5, 0); f.Effects.Add(sanctuary);
		var guard = new Mock<IDamageMutationGuard>();
		IDamage? external = f.Packet(20, guard.Object); IDamage? inward = f.Packet(12, guard.Object);
		var oldExternal = external; var oldInward = inward;
		Assert.IsTrue(f.Body.TryApplyDamageProtection(ref external, ref inward));
		Assert.AreEqual(10.0, external!.DamageAmount); Assert.AreEqual(6.0, inward!.DamageAmount);
		Assert.AreSame(guard.Object, external.MutationGuard); Assert.AreSame(oldExternal.TargetBody, external.TargetBody);
		Assert.AreSame(oldInward.Bodypart, inward.Bodypart); Assert.AreEqual(oldExternal.PenetrationOutcome, external.PenetrationOutcome);
		f.Effects.Clear(); var shield = new DamageProtectionStatus(f.Victim.Object, 0, 1); f.Effects.Add(shield);
		external = f.Packet(0); inward = null;
		Assert.IsTrue(f.Body.TryApplyDamageProtection(ref external, ref inward)); Assert.IsFalse(shield.IsSpent);
	}

	[TestMethod]
	public void FlatProtection_ExposurePreviewNeverSpendsAnUnusedFiniteShield()
	{
		var f = ProtectionFixture(); var shield = new DamageProtectionStatus(f.Victim.Object, 0, 1); f.Effects.Add(shield);
		var patch = new ExposurePatch(f.Body, f.Part.Object, Mock.Of<ISolid>(), 1, 100);
		var reaction = new Mock<IEnvironmentalReaction>(); reaction.SetupGet(x => x.DamageType).Returns(DamageType.Burning);
		var resolution = new ExposureResolution(patch, reaction.Object, 20, 20, 20, 20, 0);
		var context = new ExposureDamageContext(default, default, "fixture", "ambient", "heat", Guid.NewGuid());
		Assert.IsNull(ExposureDiagnostic.PredictNormalModifiers(resolution, context, 1));
		Assert.IsNull(ExposureDiagnostic.PredictNormalModifiers(resolution, context, 1));
		Assert.AreEqual(1, shield.RemainingHits); Assert.IsFalse(shield.IsSpent);
		Assert.IsTrue(f.Effects.Contains(shield));
		f.Effects.Clear(); f.Effects.Add(new DamageProtectionStatus(f.Victim.Object, 0.5, 0));
		Assert.AreEqual(10.0, ExposureDiagnostic.PredictNormalModifiers(resolution, context, 1)!.DamageAmount);
	}

	[TestMethod]
	public void FlatProtection_DetachedByApplicabilityOrMutationGuardCannotSpendOrApplyStaleMultiplier()
	{
		foreach (var detachInGuard in new[] { false, true })
		{
			var f = ProtectionFixture(); var effect = new Mock<IDamageProtectionEffect>(); effect.SetupGet(x => x.DamageMultiplier).Returns(0);
			effect.Setup(x => x.Applies()).Returns(() => { if (!detachInGuard) f.Effects.Clear(); return true; });
			f.Effects.Add(effect.Object);
			var guard = new Mock<IDamageMutationGuard>(); guard.Setup(x => x.Validate(f.Body, It.IsAny<IDamage>())).Callback(f.Effects.Clear);
			IDamage? damage = f.Packet(20, guard.Object); IDamage? inward = null;
			Assert.IsTrue(f.Body.TryApplyDamageProtection(ref damage, ref inward)); Assert.AreEqual(20.0, damage!.DamageAmount);
			effect.Verify(x => x.TryCommitProtection(), Times.Never);
		}
	}

	[TestMethod]
	public void FlatProtection_MarksSpentBeforeRemovalCallbacksAndDoesNotResurrectThroughXml()
	{
		var f = ProtectionFixture(); var shield = new DamageProtectionStatus(f.Victim.Object, 0, 1); f.Effects.Add(shield);
		IDamage? reentrantDamage = null;
		f.Victim.Setup(x => x.RemoveEffect(shield, false)).Callback(() =>
		{
			Assert.IsTrue(shield.IsSpent); Assert.AreEqual(1.0, shield.DamageMultiplier);
			reentrantDamage = f.Packet(5); IDamage? inward = null;
			Assert.IsTrue(f.Body.TryApplyDamageProtection(ref reentrantDamage, ref inward));
			Assert.AreEqual(5.0, reentrantDamage!.DamageAmount);
			// Simulate interrupted removal: the exhausted object remains attached and must save safely.
		});
		IDamage? damage = f.Packet(20); IDamage? inner = null;
		Assert.IsTrue(f.Body.TryApplyDamageProtection(ref damage, ref inner)); Assert.IsNull(damage);
		DamageProtectionStatus.InitialiseEffectType();
		var saved = shield.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = (DamageProtectionStatus)Effect.LoadEffect(saved, f.Victim.Object);
		Assert.IsTrue(loaded.IsSpent); Assert.IsFalse(loaded.Applies()); Assert.IsFalse(loaded.TryCommitProtection());
		Assert.AreEqual(1.0, loaded.DamageMultiplier);
	}

	[TestMethod]
	[DataRow(DamageType.Cellular)]
	[DataRow(DamageType.Hypoxia)]
	public void FlatProtection_CellularShortcutUsesProtectionForNewAndExistingWounds(DamageType type)
	{
		var f = ProtectionFixture(); f.Effects.Add(new DamageProtectionStatus(f.Victim.Object, 0.5, 0));
		var strategy = new Mock<IHealthStrategy>(); var stored = new Mock<IWound>();
		stored.SetupGet(x => x.Bodypart).Returns(f.Part.Object); stored.SetupGet(x => x.DamageType).Returns(type);
		stored.SetupProperty(x => x.OriginalDamage); stored.SetupProperty(x => x.CurrentDamage);
		strategy.Setup(x => x.SufferDamage(f.Victim.Object, It.IsAny<IDamage>(), f.Part.Object))
			.Returns((IHaveWounds _, IDamage incoming, MudSharp.Body.IBodypart _) =>
			{
				stored.Object.OriginalDamage = stored.Object.CurrentDamage = incoming.DamageAmount;
				return new[] { stored.Object };
			});
		typeof(MudSharp.Body.Implementations.Body).GetField("_healthStrategy", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(f.Body, strategy.Object);
		Damage Packet() => new(f.Packet(20)) { DamageType = type };
		var first = f.Body.PassiveSufferDamage(Packet()).Single(); Assert.AreEqual(10.0, first.CurrentDamage, .000001);
		var second = f.Body.PassiveSufferDamage(Packet()).Single(); Assert.AreSame(first, second);
		Assert.AreEqual(20.0, second.CurrentDamage, .000001);
	}

	[TestMethod]
	public void EquippedAbsorption_AppliesOnActualBodyDamageBeforeMultiplicativeProtection()
	{
		var f = ProtectionFixture();
		var item = new Mock<IGameItem>(); var provider = new Mock<IEquippedItemBonusProvider>();
		item.SetupGet(x => x.InInventoryOf).Returns(f.Body);
		provider.Setup(x => x.BonusesFor(f.Victim.Object)).Returns([new(EquippedItemBonusKind.PhysicalAbsorption, 0, 2)]);
		item.Setup(x => x.GetItemTypes<IEquippedItemBonusProvider>()).Returns([provider.Object]);
		var external = (ICollection<IGameItem>)typeof(MudSharp.Body.Implementations.Body)
			.GetField("_externalItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(f.Body)!;
		external.Add(item.Object);
		Assert.AreEqual(18.0, f.Body.PassiveSufferDamage(f.Packet(20)).Single().CurrentDamage, 0.000001);
		f.Effects.Add(new DamageProtectionStatus(f.Victim.Object, 0.5, 0));
		IDamage? damage = f.Packet(20); IDamage? inward = f.Packet(12);
		Assert.IsTrue(f.Body.TryApplyDamageProtection(ref damage, ref inward));
		Assert.AreEqual(9.0, damage!.DamageAmount); Assert.AreEqual(5.0, inward!.DamageAmount);
	}

	private static Fixture ProtectionFixture()
	{
		var fixture = new Fixture();
		fixture.Victim.Setup(x => x.EffectsOfType<IDamageProtectionEffect>(It.IsAny<Predicate<IDamageProtectionEffect>>()))
			.Returns(() => fixture.Effects.OfType<IDamageProtectionEffect>());
		return fixture;
	}
}
