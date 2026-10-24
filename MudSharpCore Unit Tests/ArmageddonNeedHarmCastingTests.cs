#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonNeedHarmCastingTests
{
	[TestMethod]
	[DataRow(ArmageddonNeedHarmSpellContent.DesiccateKey, 1)]
	[DataRow(ArmageddonNeedHarmSpellContent.DesiccateKey, 7)]
	[DataRow(ArmageddonNeedHarmSpellContent.HeadlongRevelKey, 1)]
	[DataRow(ArmageddonNeedHarmSpellContent.HeadlongRevelKey, 7)]
	public void NeedHarm_SelectedGradeBindsNativeNeedAmount(string key, int grade)
	{
		var fixture = new MagicCastingFixture();
		var spell = Load(fixture, key);
		Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, string.Join("\n", spell.GradeConfigurationErrors()));
		var copy = spell.CastingCopy(fixture.Actor.Object, fixture.Traits[0], grade,
			grade == 1 ? SpellPower.ExtremelyStrong : SpellPower.ExtremelyWeak, Difficulty.Normal);
		var effect = (NeedDeltaEffect)copy.SpellEffects.First();
		if (key == ArmageddonNeedHarmSpellContent.DesiccateKey)
			Assert.AreEqual(-5.0 * grade, effect.ThirstDelta, 1e-12);
		else Assert.AreEqual(0.01 * grade, effect.DrunkDelta, 1e-12);
		Assert.AreEqual(0.0, ((NeedDeltaEffect)spell.SpellEffects.First()).ThirstDelta);
		Assert.AreEqual(0.0, ((NeedDeltaEffect)spell.SpellEffects.First()).DrunkDelta);
	}

	[TestMethod]
	[DataRow(ArmageddonNeedHarmSpellContent.DesiccateKey, "-1001")]
	[DataRow(ArmageddonNeedHarmSpellContent.HeadlongRevelKey, "26")]
	public void NeedHarm_OutOfRangeNativeNeedAmountRefusesDetachedCastingCopy(string key, string formula)
	{
		var fixture = new MagicCastingFixture();
		var row = Load(fixture, key).SnapshotModel();
		var definition = XElement.Parse(row.Definition);
		definition.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().Single().Value = formula;
		row.Definition = definition.ToString();
		var edited = new MagicSpell(row, fixture.World.Object);
		Assert.ThrowsExactly<InvalidOperationException>(() => edited.CastingCopy(
			fixture.Actor.Object, fixture.Traits[0], 7, SpellPower.ExtremelyWeak, Difficulty.Normal));
	}

	[TestMethod]
	public void NativeNeedOperation_ReportsOnlyAnObservedChangeOnCapturedBody()
	{
		var fixture = new MagicCastingFixture();
		var spell = Load(fixture, ArmageddonNeedHarmSpellContent.HeadlongRevelKey);
		var effect = (NeedDeltaEffect)spell.CastingCopy(fixture.Actor.Object, fixture.Traits[0], 1,
			SpellPower.ExtremelyWeak, Difficulty.Normal).SpellEffects.Single();
		var needs = new ActiveNeedsModel(new MudSharp.Models.Character
		{
			AlcoholLitres = 0, FoodSatiatedHours = 12, DrinkSatiatedHours = 8, WaterLitres = 0.5
		}, fixture.Actor.Object);
		fixture.Body.SetupGet(x => x.Actor).Returns(fixture.Actor.Object);
		fixture.Body.SetupGet(x => x.NeedsModel).Returns(needs);
		fixture.Body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5);
		fixture.Actor.SetupGet(x => x.NeedsModel).Returns(needs);
		fixture.Actor.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>()).Returns([]);
		fixture.Body.Setup(x => x.FulfilNeedsOnSelectedBody(It.IsAny<INeedFulfiller>(), needs, It.IsAny<Func<bool>>()))
			.Callback<INeedFulfiller, INeedsModel, Func<bool>>((delta, _, guard) =>
				needs.FulfilNeedsOnSelectedBody(delta, fixture.Body.Object, guard));
		Assert.IsNotNull(effect.CapturePreparedSelection(fixture.Actor.Object, fixture.Actor.Object));
		var first = effect.Apply(fixture.Actor.Object, fixture.Actor.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, first.Status);
		Assert.AreEqual(0.0025, needs.AlcoholLitres, 1e-12);
		fixture.Body.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Exactly(3));
		fixture.Body.Setup(x => x.FulfilNeedsOnSelectedBody(It.IsAny<INeedFulfiller>(), needs, It.IsAny<Func<bool>>()));
		var second = effect.Apply(fixture.Actor.Object, fixture.Actor.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.NoChange, second.Status);
	}

	[TestMethod]
	public void SelectedThirstChange_AmplifyingNativeRateCannotExceedConfiguredDecrease()
	{
		var fixture = new MagicCastingFixture();
		var needs = new ActiveNeedsModel(new MudSharp.Models.Character
		{
			FoodSatiatedHours = 12, DrinkSatiatedHours = 8, WaterLitres = 0.5
		}, fixture.Actor.Object);
		fixture.Body.SetupGet(x => x.NeedsModel).Returns(needs);
		fixture.Body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5);
		fixture.Actor.SetupGet(x => x.NeedsModel).Returns(needs);
		var rate = new Mock<INeedRateEffect>();
		rate.SetupGet(x => x.AppliesToActive).Returns(true);
		rate.SetupGet(x => x.ThirstMultiplier).Returns(2);
		fixture.Actor.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>()).Returns([rate.Object]);

		needs.FulfilNeedsOnSelectedBody(new NeedFulfiller { ThirstPoints = -5 }, fixture.Body.Object,
			() => ReferenceEquals(fixture.Actor.Object.Body, fixture.Body.Object) &&
			      ReferenceEquals(fixture.Body.Object.NeedsModel, needs));

		Assert.AreEqual(3.0, needs.DrinkSatiatedHours, 1e-12);
	}

	[TestMethod]
	public void SelectedAlcoholBodySwap_StopsBeforeAnyDelayedChildReachesReplacementBody()
	{
		var fixture = new MagicCastingFixture();
		var original = fixture.Body.Object;
		var replacement = new Mock<IBody>();
		IBody currentBody = original;
		fixture.Actor.SetupGet(x => x.Body).Returns(() => currentBody);
		var needs = new ActiveNeedsModel(new MudSharp.Models.Character
		{
			FoodSatiatedHours = 12, DrinkSatiatedHours = 8, WaterLitres = 0.5
		}, fixture.Actor.Object);
		fixture.Body.SetupGet(x => x.NeedsModel).Returns(needs);
		fixture.Body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5);
		fixture.Actor.SetupGet(x => x.NeedsModel).Returns(needs);
		fixture.Actor.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>()).Returns([]);
		fixture.Body.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((_, _) => currentBody = replacement.Object);

		Assert.ThrowsExactly<InvalidOperationException>(() => needs.FulfilNeedsOnSelectedBody(
			new NeedFulfiller { AlcoholLitres = 0.01 }, original,
			() => ReferenceEquals(fixture.Actor.Object.Body, original) && ReferenceEquals(original.NeedsModel, needs)));
		fixture.Body.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Once);
		replacement.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	private static MagicSpell Load(MagicCastingFixture fixture, string key)
	{
		var content = ArmageddonNeedHarmSpellContent.Create().Single(x => x.Content.Key == key).Content;
		fixture.Expressions.Clear();
		var cost = content.CostRow(1); cost.Id = 1;
		var duration = content.DurationRow(1); duration.Id = 2;
		fixture.Expressions.Add(new TraitExpression(cost, fixture.World.Object));
		fixture.Expressions.Add(new TraitExpression(duration, fixture.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		return new MagicSpell(row, fixture.World.Object);
	}
}
