#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonMovementTempoSpellTests
{
	[TestMethod]
	[DataRow("arm.spell.fleet_step", 1, SpellPower.ExtremelyStrong, 1.0 / 1.05)]
	[DataRow("arm.spell.lightning_pace", 7, SpellPower.ExtremelyWeak, 1.0 / 1.35)]
	[DataRow("arm.spell.leadfoot", 1, SpellPower.ExtremelyStrong, 1.05)]
	[DataRow("arm.spell.leadfoot", 7, SpellPower.ExtremelyWeak, 1.35)]
	public void SelectedGradeSetsNativeMovementTimeAndSurvivesColdEffectLoad(string key, int grade,
		SpellPower power, double expected)
	{
		var fixture = new MagicCastingFixture();
		var content = ArmageddonMovementTempoSpellContent.Create(fixture.Traits[0].Id).Single(x => x.Content.Key == key).Content;
		var authored = Load(fixture, content);
		var definition = authored.SpellEffects.First().SaveToXml();
		var copy = authored.CastingCopy(fixture.Actor.Object, fixture.Traits[0], grade, power, Difficulty.Normal);
		Assert.AreEqual(0, copy.GradeConfigurationErrors().Count);
		Assert.AreEqual(60 * grade, copy.EffectDurationExpression.Evaluate(fixture.Actor.Object));
		var parent = new MagicSpellParent(fixture.Actor.Object, copy, fixture.Actor.Object, power);
		var child = (SpellMovementTempoEffect)copy.SpellEffects.First().GetOrApplyEffect(fixture.Actor.Object,
			fixture.Actor.Object, OpposedOutcomeDegree.None, power, parent, [])!;
		Assert.AreEqual(expected, child.DelayMultiplier(null!), 1e-10);
		MagicSpellParent.InitialiseEffectType();
		SpellMovementTempoEffect.InitialiseEffectType();
		parent.AddSpellEffect(child);
		var saved = parent.SaveToXml(new() { [parent] = TimeSpan.FromSeconds(60 * grade) });
		var loaded = (MagicSpellParent)Effect.LoadEffect(saved, fixture.Actor.Object);
		Assert.AreEqual(expected, ((SpellMovementTempoEffect)loaded.SpellEffects.Single()).DelayMultiplier(null!), 1e-10);
		Assert.IsTrue(XNode.DeepEquals(definition, authored.SpellEffects.First().SaveToXml()));
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(8)]
	public void SelectedGradeOutsideLockedRangeRefuses(int grade)
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			MovementTempoSpellEffect.MultiplierForGrade(grade, false, 0.05));
	}

	private static MagicSpell Load(MagicCastingFixture fixture, ArmageddonUtilitySpellContent content)
	{
		fixture.Expressions.Clear();
		var cost = content.CostRow(1); cost.Id = 1;
		var duration = content.DurationRow(1); duration.Id = 2;
		fixture.Expressions.Add(new TraitExpression(cost, fixture.World.Object));
		fixture.Expressions.Add(new TraitExpression(duration, fixture.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 900; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, content.EligibilitySource is null ? 0 : 1).ToString();
		return new MagicSpell(row, fixture.World.Object);
	}
}
