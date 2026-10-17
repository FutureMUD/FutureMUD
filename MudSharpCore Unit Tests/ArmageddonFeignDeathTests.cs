#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonFeignDeathTests
{
	[TestMethod]
	public void PaidGradeTwoCastAppliesTimedAppearanceWithoutChangingLivingState()
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
		Assert.IsTrue(effects.OfType<SpellFeignedDeathEffect>().Any());
		Assert.AreEqual(CharacterState.Awake, f.Actor.Object.State);
	}

	[TestMethod]
	public void LivingBodyAdmission_RefusesDeadTargetAndChangedBody()
	{
		var f = new MagicCastingFixture();
		var spell = Load(f);
		var effect = (FeignDeathEffect)spell.SpellEffects.Single();
		f.Actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		Assert.IsFalse(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, TimeSpan.FromSeconds(60), out _, out _));
		f.Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		Assert.IsTrue(effect.TryPrepareApplication(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyWeak, TimeSpan.FromSeconds(60), out var application, out _));
		var parent = new Mock<IMagicSpellEffectParent>();
		parent.SetupGet(x => x.Owner).Returns(f.Actor.Object);
		f.Actor.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object);
		Assert.ThrowsException<InvalidOperationException>(() => application!.Create(parent.Object));
	}

	[TestMethod]
	public void OrdinaryVisualAndVitalObserversSeeDeathWhileSelfAndStaffSeeLivingSigns()
	{
		var f = new MagicCastingFixture();
		var parent = new Mock<IMagicSpellEffectParent>();
		var child = new SpellFeignedDeathEffect(f.Actor.Object, parent.Object);
		var effects = new List<IEffect> { child };
		f.Actor.Setup(x => x.EffectsOfType<SpellFeignedDeathEffect>(
			It.IsAny<Predicate<SpellFeignedDeathEffect>>())).Returns(() => effects.OfType<SpellFeignedDeathEffect>());
		var observer = new Mock<ICharacter>(); observer.SetupGet(x => x.Id).Returns(400);
		Assert.IsTrue(SpellFeignedDeathEffect.AppearsDeadTo(observer.Object, f.Actor.Object));
		Assert.IsTrue(child.DescriptionAdditionApplies(observer.Object));
		Assert.AreEqual("(apparently dead)", child.GetAddendumText(false));
		Assert.IsTrue(child.GetAdditionalText(observer.Object, false).Contains("no visible breath"));
		Assert.AreEqual("A traveller is lying motionless, apparently dead",
			SpellFeignedDeathEffect.ApparentPositionDescription(observer.Object, f.Actor.Object,
				"A traveller", false));
		Assert.IsNull(SpellFeignedDeathEffect.ApparentPositionDescription(f.Staff.Object, f.Actor.Object,
			"A traveller", false));
		Assert.AreEqual(("no pulse", "not breathing"), SpellFeignedDeathEffect.ObservedVitalSigns(
			observer.Object, f.Actor.Object, "an ordinary pulse", "breathing"));
		Assert.AreEqual(("an ordinary pulse", "breathing"), SpellFeignedDeathEffect.ObservedVitalSigns(
			f.Actor.Object, f.Actor.Object, "an ordinary pulse", "breathing"));
		Assert.AreEqual(("an ordinary pulse", "breathing"), SpellFeignedDeathEffect.ObservedVitalSigns(
			f.Staff.Object, f.Actor.Object, "an ordinary pulse", "breathing"));
		Assert.AreEqual(CharacterState.Awake, f.Actor.Object.State);
	}

	[TestMethod]
	public void VoluntaryActionRemovesTimedParentWhilePassiveObservationKeepsIt()
	{
		var f = new MagicCastingFixture();
		f.Actor.DefaultValue = DefaultValue.Empty;
		var parent = new Mock<IMagicSpellEffectParent>();
		var child = new SpellFeignedDeathEffect(f.Actor.Object, parent.Object);
		var effects = new List<IEffect> { parent.Object, child };
		f.Actor.SetupGet(x => x.Effects).Returns(() => effects);
		f.Actor.Setup(x => x.EffectsOfType<SpellFeignedDeathEffect>(
			It.IsAny<Predicate<SpellFeignedDeathEffect>>())).Returns(() => effects.OfType<SpellFeignedDeathEffect>());
		f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((removed, _) =>
		{
			effects.Remove(removed);
			effects.Remove(child);
		});
		SpellFeignedDeathEffect.BreakOnVoluntaryAction(f.Actor.Object, "look");
		Assert.AreEqual(2, effects.Count);
		var command = new Mock<IExecutable<ICharacter>>();
		command.SetupGet(x => x.Name).Returns("Move");
		command.SetupGet(x => x.PermissionRequired).Returns(PermissionLevel.Any);
		command.SetupGet(x => x.AllowedStates).Returns(CharacterState.Any);
		var removedBeforeExecute = false;
		command.Setup(x => x.Execute(f.Actor.Object, "north", CharacterState.Awake,
			PermissionLevel.Any, f.Actor.Object.OutputHandler)).Returns(() =>
		{
			removedBeforeExecute = effects.Count == 0;
			return true;
		});
		Assert.IsTrue(new CharacterCommandManager().Execute(f.Actor.Object, command.Object,
			"north", CharacterState.Awake, PermissionLevel.Any, f.Actor.Object.OutputHandler));
		Assert.IsTrue(removedBeforeExecute);
		Assert.AreEqual(0, effects.Count);
		f.Actor.Verify(x => x.RemoveEffect(parent.Object, true), Times.Once);
	}

	[DataTestMethod]
	[DataRow("lo")]
	[DataRow("loo")]
	[DataRow("sc")]
	[DataRow("sco")]
	[DataRow("scor")]
	[DataRow("i")]
	[DataRow("equipment")]
	public void PassiveCommandAliasesKeepFeignDeath(string command)
	{
		var f = new MagicCastingFixture();
		var parent = new Mock<IMagicSpellEffectParent>();
		var child = new SpellFeignedDeathEffect(f.Actor.Object, parent.Object);
		var effects = new List<IEffect> { parent.Object, child };
		f.Actor.SetupGet(x => x.Effects).Returns(effects);
		f.Actor.Setup(x => x.EffectsOfType<SpellFeignedDeathEffect>(
			It.IsAny<Predicate<SpellFeignedDeathEffect>>())).Returns(() => effects.OfType<SpellFeignedDeathEffect>());
		SpellFeignedDeathEffect.BreakOnVoluntaryAction(f.Actor.Object, command);
		Assert.AreEqual(2, effects.Count);
		f.Actor.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void ColdChildReloadRetainsObserverBehaviorWithoutChangingLivingState()
	{
		var f = new MagicCastingFixture();
		SpellFeignedDeathEffect.InitialiseEffectType();
		var child = new SpellFeignedDeathEffect(f.Actor.Object, new Mock<IMagicSpellEffectParent>().Object);
		var xml = child.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = (SpellFeignedDeathEffect)Effect.LoadEffect(xml, f.Actor.Object);
		var observer = new Mock<ICharacter>(); observer.SetupGet(x => x.Id).Returns(400);
		Assert.IsTrue(loaded.DescriptionAdditionApplies(observer.Object));
		Assert.AreEqual("(apparently dead)", loaded.GetAddendumText(false));
		Assert.AreEqual(CharacterState.Awake, f.Actor.Object.State);
	}

	[TestMethod]
	public void ColdParentReloadPreservesRemainingLifetimeAndExpiryRemovesAppearance()
	{
		var f = new MagicCastingFixture();
		var spell = Load(f);
		f.Spells.Remove(f.Spell); f.Spells.Add(spell);
		MagicSpellParent.InitialiseEffectType();
		SpellFeignedDeathEffect.InitialiseEffectType();
		var effects = new List<IEffect>();
		f.Actor.SetupGet(x => x.Effects).Returns(() => effects);
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		f.Actor.Setup(x => x.EffectsOfType<SpellFeignedDeathEffect>(
			It.IsAny<Predicate<SpellFeignedDeathEffect>>())).Returns(() => effects.OfType<SpellFeignedDeathEffect>());
		f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((effect, fire) =>
			{
				if (fire) effect.RemovalEffect();
				effects.Remove(effect);
			});
		var parent = new MagicSpellParent(f.Actor.Object, spell, f.Actor.Object,
			SpellPower.ExtremelyWeak);
		parent.AddSpellEffect(new SpellFeignedDeathEffect(f.Actor.Object, parent));
		var saved = parent.SaveToXml(new Dictionary<IEffect, TimeSpan>
		{
			[parent] = TimeSpan.FromSeconds(45)
		});
		Assert.AreEqual(45000L, (long)saved.Element("Remaining")!);
		var loaded = (MagicSpellParent)Effect.LoadEffect(saved, f.Actor.Object);
		effects.Add(loaded);
		Assert.IsInstanceOfType(loaded.SpellEffects.Single(), typeof(SpellFeignedDeathEffect));
		Assert.AreSame(loaded, loaded.SpellEffects.Single().ParentEffect);
		var observer = new Mock<ICharacter>(); observer.SetupGet(x => x.Id).Returns(400);
		Assert.IsTrue(SpellFeignedDeathEffect.AppearsDeadTo(observer.Object, f.Actor.Object));
		loaded.ExpireEffect();
		Assert.IsFalse(SpellFeignedDeathEffect.AppearsDeadTo(observer.Object, f.Actor.Object));
		Assert.IsFalse(effects.Any());
	}

	private static MagicSpell Load(MagicCastingFixture f)
	{
		var content = ArmageddonFeignDeathSpellContent.Create().Content;
		var cost = content.CostRow(1); cost.Id = 61;
		var duration = content.DurationRow(1); duration.Id = 62;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object));
		f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 62;
		row.Definition = content.BuildDefinition(10, 61, 0).ToString();
		return new MagicSpell(row, f.World.Object);
	}
}
