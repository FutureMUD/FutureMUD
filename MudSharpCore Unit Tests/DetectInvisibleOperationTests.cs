#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DetectInvisibleOperationTests
{
	[TestMethod]
	public void LegacyFactory_StillReturnsUnattachedDetectionChild()
	{
		var f = Fixture();
		var parent = new MagicSpellParent(f.Actor.Object, f.Spells.Single(), f.Actor.Object);
		var child = f.Spells.Single().SpellEffects.Single().GetOrApplyEffect(f.Actor.Object, f.Actor.Object,
			default, SpellPower.Weak, parent, []);
		Assert.IsInstanceOfType(child, typeof(SpellDetectInvisibleEffect));
		Assert.IsFalse(f.Actor.Object.Effects.Any());
		Assert.IsFalse(parent.SpellEffects.Any());
	}

	[DataTestMethod]
	[DataRow(true, false, MagicEffectOperationStatus.Applied)]
	[DataRow(false, false, MagicEffectOperationStatus.NoChange)]
	[DataRow(false, true, MagicEffectOperationStatus.Unknown)]
	public void Operation_ReportsRetainedMutationAndDoesNotReturnASecondAttachment(bool retain, bool unrelatedMutation,
		MagicEffectOperationStatus expected)
	{
		var f = Fixture();
		var effects = Attachments(f, retain, unrelatedMutation);
		var parent = new MagicSpellParent(f.Actor.Object, f.Spells.Single(), f.Actor.Object);
		var report = ((IMagicSpellEffectOperation)f.Spells.Single().SpellEffects.Single())
			.Apply(f.Actor.Object, f.Actor.Object, default, SpellPower.Weak, parent, []);
		Assert.AreEqual(expected, report.Status);
		Assert.IsNull(report.Effect);
		Assert.AreEqual(retain ? 1 : 0, effects.OfType<SpellDetectInvisibleEffect>().Count());
		Assert.AreEqual(retain ? 1 : 0, parent.SpellEffects.Count());
		if (retain)
		{
			Assert.AreEqual(MagicEffectOperationStatus.NoChange,
				((IMagicSpellEffectOperation)f.Spells.Single().SpellEffects.Single())
				.Apply(f.Actor.Object, f.Actor.Object, default, SpellPower.Weak, parent, []).Status);
			Assert.AreEqual(1, effects.OfType<SpellDetectInvisibleEffect>().Count());
		}
	}

	[DataTestMethod]
	[DataRow(true, 3)]
	[DataRow(false, 2)]
	public void OrdinaryPaidCast_OnlyRetainedChildEarnsMastery(bool retain, int expectedGrade)
	{
		var f = Fixture();
		var effects = Attachments(f, retain);
		var before = f.Balances[f.Resources[1]];
		var quote = f.Service.Quote(f.Intent());
		Assert.IsTrue(quote.Allowed, quote.Reason);
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(before - quote.Invocation!.Costs.Single().Amount, f.Balances[f.Resources[1]]);
		Assert.AreEqual(expectedGrade, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(retain, (bool)XElement.Parse(f.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied")!);
		Assert.AreEqual(retain ? 1 : 0, effects.OfType<SpellDetectInvisibleEffect>().Count());
		Assert.AreEqual(retain ? 1 : 0, effects.OfType<MagicSpellParent>().Count());
		Assert.AreEqual(1, f.Rolls);
	}

	[TestMethod]
	public void Refresh_ReplacesOneParentAndChildWithoutDoubleAttachment()
	{
		var f = Fixture();
		var effects = Attachments(f, true);
		Assert.IsTrue(f.Spells.Single().BuildingCommand(f.Actor.Object, new StringStack("exclusiveeffect")));
		var first = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, first.Status, first.Message);
		var original = effects.OfType<SpellDetectInvisibleEffect>().Single();
		f.Now = f.Now.AddMinutes(20);
		f.Balances[f.Resources[1]] = 100;
		f.Acquire();
		var refreshed = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, refreshed.Status, refreshed.Message);
		Assert.AreNotSame(original, effects.OfType<SpellDetectInvisibleEffect>().Single());
		Assert.AreEqual(1, effects.OfType<MagicSpellParent>().Count());
		Assert.AreEqual(1, effects.OfType<MagicSpellParent>().Single().SpellEffects.Count());
		Assert.IsTrue((bool)XElement.Parse(f.Store.Operations[refreshed.OperationId!.Value].Definition).Attribute("applied")!);
	}

	[TestMethod]
	public void AttachmentFailure_AfterPaymentKeepsTimedOwnershipWithoutMasteryOrReplay()
	{
		var f = Fixture();
		var effects = Attachments(f, true);
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect =>
		{
			effects.Add(effect);
			throw new InvalidOperationException("Injected failure after retaining detection");
		});
		var before = f.Balances[f.Resources[1]];
		var result = f.Service.Cast(f.Intent());
		Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.IsTrue(f.Balances[f.Resources[1]] < before);
		Assert.AreEqual(2, f.Service.Acquisition(f.Actor.Object, 1)!.ControlledGrade);
		Assert.AreEqual(1, effects.OfType<SpellDetectInvisibleEffect>().Count());
		Assert.AreEqual(1, effects.OfType<MagicSpellParent>().Count());
		var paid = f.Balances[f.Resources[1]];
		f.Restart();
		Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(f.Intent()).Status);
		Assert.AreEqual(paid, f.Balances[f.Resources[1]]);
		Assert.AreEqual(0, f.Samples);
	}

	private static MagicCastingFixture Fixture()
	{
		var f = new MagicCastingFixture();
		f.Spells.Remove(f.Spell);
		var spell = f.NewSpell(1, "Detection operation fixture", "<Effect type='detectinvisible'/>");
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		f.Acquire();
		return f;
	}

	private static List<IEffect> Attachments(MagicCastingFixture f, bool retain, bool unrelatedMutation = false)
	{
		var effects = new List<IEffect>();
		f.Actor.SetupGet(x => x.Effects).Returns(effects);
		f.Actor.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
			.Returns<Predicate<MagicSpellParent>>(filter => effects.OfType<MagicSpellParent>().Where(x => filter is null || filter(x)).ToArray());
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect =>
		{
			if (retain) effects.Add(effect);
			if (unrelatedMutation) effects.Add(Mock.Of<IEffect>());
		});
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => effects.Add(effect));
		f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, _) => effects.Remove(effect));
		f.Actor.Setup(x => x.RemoveAllEffects<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>(), It.IsAny<bool>()))
			.Callback<Predicate<MagicSpellParent>, bool>((filter, _) =>
			{
				foreach (var parent in effects.OfType<MagicSpellParent>().Where(filter.Invoke).ToArray())
				{
					parent.RemovalEffect();
					effects.Remove(parent);
				}
			});
		return effects;
	}
}
