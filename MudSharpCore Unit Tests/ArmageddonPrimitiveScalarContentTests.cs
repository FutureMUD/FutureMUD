#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPrimitiveScalarContentTests
{
	[TestMethod]
	public void NativeScalarBinding_AbsentBindingRetainsOrdinaryBuilderFields()
	{
		foreach (var contribution in ArmageddonPrimitiveSpellContent.CreateScalarContributions())
		{
			var f = new MagicCastingFixture();
			var row = Load(f, contribution.Content).SnapshotModel();
			var definition = XElement.Parse(row.Definition);
			definition.Element("ControlledPower")!.Element("ScalarBindings")!.RemoveNodes();
			row.Definition = definition.ToString();
			var spell = new MagicSpell(row, f.World.Object);
			var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], 7, SpellPower.ExtremelyStrong, Difficulty.Normal);
			if (copy.SpellEffects.Single() is RoomLightEffect roomLight)
				Assert.AreEqual(contribution.Content.Key.EndsWith("deep_darkness") ? -10.0 : 10.0, roomLight.AddedLuxPerPower);
			else
				Assert.AreEqual(1.1, ((HealingRateSpellEffect)copy.SpellEffects.Single()).Multiplier);
		}
	}

	[TestMethod]
	public void NativeScalarFactory_AllDefinitionsAndGradesRoundTripWithoutChangingBuilderDefaults()
	{
		foreach (var contribution in ArmageddonPrimitiveSpellContent.CreateScalarContributions())
		{
			var f = new MagicCastingFixture();
			var spell = Load(f, contribution.Content);
			var original = spell.SpellEffects.Single().SaveToXml();
			Assert.IsTrue(XNode.DeepEquals(contribution.Content.BuildDefinition(10, 1, 0).Element("Effects")!.Elements().Single(), original));
			Assert.IsTrue(XNode.DeepEquals(original, spell.SpellEffects.Single().Clone().SaveToXml()));
			Assert.IsTrue(spell.SpellEffects.Single().IsCompatibleWithTrigger(spell.Trigger));
			Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, string.Join("\n", spell.GradeConfigurationErrors()));
			for (var grade = 1; grade <= 7; grade++)
			{
				var power = (SpellPower)((int)SpellPower.ExtremelyWeak + grade - 1);
				AssertOutcome(f, spell, contribution.Content.Key, grade, power);
			}
			Assert.IsTrue(XNode.DeepEquals(original, spell.SpellEffects.Single().SaveToXml()));
			var reload = new MagicSpell(spell.SnapshotModel(), f.World.Object);
			Assert.IsTrue(XNode.DeepEquals(original, reload.SpellEffects.Single().SaveToXml()));
		}
	}

	[TestMethod]
	[DataRow("daylight", 1, SpellPower.ExtremelyStrong)]
	[DataRow("daylight", 7, SpellPower.ExtremelyWeak)]
	[DataRow("deep_darkness", 3, SpellPower.VeryStrong)]
	[DataRow("quickened_healing", 1, SpellPower.ExtremelyStrong)]
	[DataRow("quickened_healing", 7, SpellPower.ExtremelyWeak)]
	public void NativeScalarBinding_GradeAndNativePowerRemainIndependent(string suffix, int grade, SpellPower power)
	{
		var f = new MagicCastingFixture();
		var content = ArmageddonPrimitiveSpellContent.CreateScalarContributions().Single(x => x.Content.Key == "arm.spell." + suffix).Content;
		AssertOutcome(f, Load(f, content), content.Key, grade, power);
	}

	[TestMethod]
	public void QuickenedHealing_PaidNativeChild_ReportsApplied()
	{
		var fixture = new MagicCastingFixture();
		var content = ArmageddonPrimitiveSpellContent.CreateScalarContributions()
			.Single(x => x.Content.Key == "arm.spell.quickened_healing").Content;
		fixture.Spells[0] = Load(fixture, content);
		fixture.Acquire(1);
		TrackEffects(fixture, false);
		var result = fixture.Service.Cast(fixture.Intent(1, false));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var operation = fixture.Store.Operations[result.OperationId!.Value];
		Assert.AreEqual("true", (string?)XElement.Parse(operation.Definition).Attribute("applied"));
		fixture.Actor.Verify(x => x.AddEffect(It.IsAny<SpellHealingRateEffect>()), Times.Once);
	}

	[TestMethod]
	public void QuickenedHealing_ChildRemovedDuringParentAttachment_DoesNotReportApplied()
	{
		var fixture = new MagicCastingFixture();
		var content = ArmageddonPrimitiveSpellContent.CreateScalarContributions()
			.Single(x => x.Content.Key == "arm.spell.quickened_healing").Content;
		fixture.Spells[0] = Load(fixture, content);
		fixture.Acquire(1);
		var effects = TrackEffects(fixture, true);
		var result = fixture.Service.Cast(fixture.Intent(1, false));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var operation = fixture.Store.Operations[result.OperationId!.Value];
		Assert.AreEqual("false", (string?)XElement.Parse(operation.Definition).Attribute("applied"));
		Assert.IsFalse(effects.OfType<SpellHealingRateEffect>().Any());
		Assert.IsFalse(effects.OfType<MagicSpellParent>().Any());
	}

	private static List<IEffect> TrackEffects(MagicCastingFixture fixture, bool removeChildOnParent)
	{
		var effects = new List<IEffect>();
		fixture.Actor.SetupGet(x => x.Effects).Returns(() => effects);
		fixture.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		fixture.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) =>
			{
				effects.Add(effect);
				if (!removeChildOnParent) return;
				var child = effects.OfType<SpellHealingRateEffect>().Single();
				effects.Remove(child);
				child.RemovalEffect();
			});
		fixture.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((effect, fire) =>
			{
				effects.Remove(effect);
				if (fire) effect.RemovalEffect();
			});
		return effects;
	}

	[TestMethod]
	[DataRow("daylight", "20*grade/0")]
	[DataRow("deep_darkness", "sqrt(-1)")]
	[DataRow("quickened_healing", "1/0")]
	public void NativeScalarBinding_InvalidResultRefusesBeforePayment(string suffix, string formula)
	{
		var f = new MagicCastingFixture(); f.Acquire();
		var content = ArmageddonPrimitiveSpellContent.CreateScalarContributions().Single(x => x.Content.Key == "arm.spell." + suffix).Content;
		var spell = Load(f, content);
		var row = spell.SnapshotModel();
		var definition = XElement.Parse(row.Definition);
		definition.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().Single().Value = formula;
		row.Definition = definition.ToString();
		f.Spells[0] = new MagicSpell(row, f.World.Object);
		var intent = f.Intent(1, false) with { Targets = suffix == "quickened_healing" ? "self" : "here" };
		var result = f.Service.Cast(intent);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		StringAssert.Contains(result.Message, "target[0]");
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(0, f.Rolls);
		Assert.AreEqual(0, f.Store.Operations.Count);
	}

	private static void AssertOutcome(MagicCastingFixture f, MagicSpell spell, string key, int grade, SpellPower power)
	{
		var copy = spell.CastingCopy(f.Actor.Object, f.Traits[0], grade, power, Difficulty.Normal);
		Assert.AreEqual((key.EndsWith("quickened_healing") ? 120.0 : 60.0) * grade,
			copy.EffectDurationExpression.Evaluate(f.Actor.Object));
		var template = copy.SpellEffects.Single();
		if (template is RoomLightEffect)
		{
			Mock.Get(f.World.Object.LightModel).Setup(x => x.GetIlluminationDescription(It.IsAny<double>())).Returns("bright");
			Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Gameworld).Returns(f.World.Object);
			var child = template.GetOrApplyEffect(f.Actor.Object, f.Actor.Object.Location,
				OpposedOutcomeDegree.None, power, null!, []);
			Assert.IsInstanceOfType(child, typeof(SpellRoomLightEffect));
			Assert.AreEqual((key.EndsWith("deep_darkness") ? -20.0 : 20.0) * grade,
				((SpellRoomLightEffect)child!).GlowLux, 1e-10);
		}
		else
		{
			var child = template.GetOrApplyEffect(f.Actor.Object, f.Actor.Object,
				OpposedOutcomeDegree.None, power, null!, []);
			Assert.IsInstanceOfType(child, typeof(SpellHealingRateEffect));
			Assert.AreEqual(1.0 + 0.1 * grade, ((SpellHealingRateEffect)child!).HealingRateMultiplier, 1e-10);
			Assert.AreEqual(0, ((SpellHealingRateEffect)child).HealingDifficultyStages);
		}
	}

	private static MagicSpell Load(MagicCastingFixture f, ArmageddonUtilitySpellContent content)
	{
		f.Actor.Setup(x => x.TargetActorOrCorpse("self", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.Gameworld).Returns(f.World.Object);
		f.Expressions.Clear();
		var cost = content.CostRow(1); cost.Id = 1;
		var duration = content.DurationRow(1); duration.Id = 2;
		f.Expressions.Add(new TraitExpression(cost, f.World.Object));
		f.Expressions.Add(new TraitExpression(duration, f.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		return new MagicSpell(row, f.World.Object);
	}
}
