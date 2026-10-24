#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonBurningDraughtTests
{
	[TestMethod]
	public void PaidGradeTwoCastAddsTimedCurseToExactLivingBody()
	{
		var f = new MagicCastingFixture();
		f.Actor.DefaultValue = DefaultValue.Empty;
		f.Body.DefaultValue = DefaultValue.Empty;
		f.World.DefaultValue = DefaultValue.Empty;
		Mock.Get(f.Actor.Object.Location).DefaultValue = DefaultValue.Empty;
		f.Actor.Setup(x => x.TargetActorOrCorpse("self")).Returns(f.Actor.Object);
		f.Actor.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		f.Actor.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<Predicate<IPlanarOverlayEffect>>())).Returns([]);
		f.Body.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(It.IsAny<Predicate<IPlanarOverlayEffect>>())).Returns([]);
		f.Actor.SetupGet(x => x.Merits).Returns(Array.Empty<IMerit>());
		var spell = Load(f);
		f.Spells.Remove(f.Spell); f.Spells.Add(spell);
		f.Acquire(2);
		var effects = new List<IEffect>();
		TimeSpan duration = TimeSpan.Zero;
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, time) => { effects.Add(effect); duration = time; });
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		var result = f.Service.Cast(f.Intent(2, false));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(100 - new ControlledSpellEfficiency(20, 1).Cost(2, 2),
			f.Balances[f.Resources[1]], 0.000001);
		Assert.AreEqual(TimeSpan.FromSeconds(120), duration);
		Assert.AreEqual(5.0, effects.OfType<SpellBurningDraughtEffect>().Single().DamagePerSip);
	}

	[TestMethod]
	public void CurseAdmitsOnlyLivingExactBodyAndPositiveDuration()
	{
		var f = new MagicCastingFixture();
		var effect = (BurningDraughtEffect)Load(f).SpellEffects.Single();
		f.Actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		Assert.IsFalse(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, TimeSpan.FromSeconds(60), out _, out _));
		f.Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		Assert.IsFalse(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, TimeSpan.Zero, out _, out _));
		Assert.IsTrue(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, TimeSpan.FromSeconds(60), out var application, out _));
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Owner).Returns(f.Actor.Object);
		f.Actor.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object);
		Assert.ThrowsExactly<InvalidOperationException>(() => application!.Create(parent.Object));
	}

	[TestMethod]
	public void AttachedCurseAppliesOneBurningPacketPerAcceptedSipAndSkipsDetachedCurse()
	{
		var f = new MagicCastingFixture();
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Owner).Returns(f.Actor.Object);
		var child = new SpellBurningDraughtEffect(f.Actor.Object, parent.Object, 5);
		var effects = new List<IEffect> { parent.Object, child };
		parent.SetupGet(x => x.SpellEffects).Returns([child]);
		f.Actor.SetupGet(x => x.Effects).Returns(() => effects);
		f.Actor.Setup(x => x.EffectsOfType<SpellBurningDraughtEffect>(
			It.IsAny<Predicate<SpellBurningDraughtEffect>>())).Returns(() => effects.OfType<SpellBurningDraughtEffect>());
		f.Body.SetupGet(x => x.Actor).Returns(f.Actor.Object);
		var part = new Mock<IBodypart>();
		f.Body.SetupGet(x => x.RandomBodypart).Returns(part.Object);
		f.Body.SetupGet(x => x.Bodyparts).Returns([part.Object]);
		f.Body.SetupGet(x => x.WornItems).Returns([]);
		f.Body.SetupGet(x => x.Prosthetics).Returns([]);
		var packets = new List<IDamage>();
		f.Body.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(packets.Add)
			.Returns(Array.Empty<IWound>());
		Assert.IsTrue(SpellBurningDraughtEffect.TryBurnSip(f.Actor.Object, f.Body.Object));
		Assert.AreEqual(1, packets.Count);
		Assert.AreEqual(DamageType.Burning, packets[0].DamageType);
		Assert.AreEqual(5.0, packets[0].DamageAmount);
		Assert.AreSame(f.Body.Object, packets[0].TargetBody);
		packets[0].MutationGuard!.Validate(f.Body.Object, packets[0]);
		effects.Remove(parent.Object);
		try
		{
			packets[0].MutationGuard!.Validate(f.Body.Object, packets[0]);
			Assert.Fail("A detached curse must reject wound mutation.");
		}
		catch (InvalidOperationException)
		{
			// Revoked spell ownership is rejected before the native wound write.
		}
		Assert.IsFalse(SpellBurningDraughtEffect.TryBurnSip(f.Actor.Object, f.Body.Object));
		Assert.AreEqual(1, packets.Count);
	}

	[TestMethod]
	public void ColdParentReloadRetainsChildUntilExplicitExpiry()
	{
		var f = new MagicCastingFixture();
		var spell = Load(f);
		f.Spells.Remove(f.Spell); f.Spells.Add(spell);
		MagicSpellParent.InitialiseEffectType();
		SpellBurningDraughtEffect.InitialiseEffectType();
		var effects = new List<IEffect>();
		f.Actor.SetupGet(x => x.Effects).Returns(() => effects);
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((effect, fire) =>
			{
				if (fire) effect.RemovalEffect();
				effects.Remove(effect);
			});
		var parent = new MagicSpellParent(f.Actor.Object, spell, f.Actor.Object,
			SpellPower.ExtremelyWeak);
		parent.AddSpellEffect(new SpellBurningDraughtEffect(f.Actor.Object, parent, 5));
		var saved = parent.SaveToXml(new Dictionary<IEffect, TimeSpan>
		{
			[parent] = TimeSpan.FromSeconds(45)
		});
		Assert.AreEqual(45000L, (long)saved.Element("Remaining")!);
		var loaded = (MagicSpellParent)Effect.LoadEffect(saved, f.Actor.Object);
		effects.Add(loaded);
		Assert.AreEqual(5.0, ((SpellBurningDraughtEffect)loaded.SpellEffects.Single()).DamagePerSip);
		loaded.ExpireEffect();
		Assert.IsFalse(effects.Any());
	}

	[TestMethod]
	public void ColdChildReloadKeepsBoundedSipDamage()
	{
		var f = new MagicCastingFixture();
		SpellBurningDraughtEffect.InitialiseEffectType();
		var child = new SpellBurningDraughtEffect(f.Actor.Object, new Mock<IMagicSpellEffectParent>().Object, 5);
		var xml = child.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = (SpellBurningDraughtEffect)Effect.LoadEffect(xml, f.Actor.Object);
		Assert.AreEqual(5.0, loaded.DamagePerSip);
		Assert.AreEqual(CharacterState.Awake, f.Actor.Object.State);
	}

	private static MagicSpell Load(MagicCastingFixture f)
	{
		var content = ArmageddonBurningDraughtSpellContent.Create().Content;
		var cost = content.CostRow(1); cost.Id = 61;
		var duration = content.DurationRow(1); duration.Id = 62;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object));
		f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 62;
		row.Definition = content.BuildDefinition(10, 61, 0).ToString();
		return new MagicSpell(row, f.World.Object);
	}
}
