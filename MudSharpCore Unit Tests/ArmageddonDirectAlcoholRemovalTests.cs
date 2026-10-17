#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Effects.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ArmageddonDirectAlcoholRemovalTests
{
	[DataTestMethod]
	[DataRow(1, 0.1, 0.09)]
	[DataRow(7, 0.1, 0.03)]
	[DataRow(7, 0.005, 0.0)]
	[DataRow(1, 0.0, 0.0)]
	public void ExactAlcohol_RealActiveModelBypassesMultiplierAndRestoresSavedNeedsSnapshot(int grade, double before, double expected)
	{
		var (f, needs) = Fixture(before);
		var effect = Effect(f, -0.01 * grade);
		var savedBefore = Snapshot(needs);
		var result = effect.Apply(f.Actor.Object, f.Actor.Object, OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []);
		Assert.AreEqual(expected, needs.AlcoholLitres, 1e-12);
		Assert.AreEqual(before == expected ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Applied, result.Status);
		Assert.AreEqual(before != expected, f.Actor.Object.Changed);
		Assert.AreEqual(before != expected, f.Body.Object.Changed);
		Assert.AreEqual(savedBefore.FoodSatiatedHours, needs.FoodSatiatedHours);
		Assert.AreEqual(savedBefore.DrinkSatiatedHours, needs.DrinkSatiatedHours);
		Assert.AreEqual(savedBefore.WaterLitres, needs.WaterLitres);
		Assert.AreEqual(savedBefore.SatiationReserve, needs.SatiationReserve);
		f.Body.Verify(x => x.FulfilNeeds(It.IsAny<INeedFulfiller>(), It.IsAny<bool>()), Times.Never);
		var serialized = JsonSerializer.Serialize(Snapshot(needs));
		var restored = new ActiveNeedsModel(JsonSerializer.Deserialize<MudSharp.Models.Character>(serialized)!, f.Actor.Object);
		Assert.AreEqual(expected, restored.AlcoholLitres, 1e-12);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ExactAlcohol_NonPersistingModelsDoNotPretendMutation(bool passive)
	{
		var (f, _) = Fixture(0.1);
		INeedsModel needs = passive ? new PassiveNeedsModel(f.Actor.Object) : new NoNeedsModel();
		needs.AlcoholLitres = 0.1;
		f.Actor.SetupGet(x => x.NeedsModel).Returns(needs);
		f.Body.SetupGet(x => x.NeedsModel).Returns(needs);
		var before = needs.AlcoholLitres;
		var result = Effect(f, -0.07).Apply(f.Actor.Object, f.Actor.Object, OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.NoChange, result.Status);
		Assert.AreEqual(before, needs.AlcoholLitres);
		Assert.IsFalse(f.Actor.Object.Changed);
		Assert.IsFalse(f.Body.Object.Changed);
	}

	[TestMethod]
	public void ExactAlcohol_PreparedBodyDriftRejectsBeforeMutation()
	{
		var (f, needs) = Fixture(0.1);
		var effect = Effect(f, -0.07);
		var token = effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object)!;
		f.Actor.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object);
		Assert.IsFalse(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object, out _));
		Assert.IsFalse(effect.TryReusePreparedSelection(token, f.Actor.Object, f.Actor.Object, out _));
		Assert.AreEqual(MagicEffectOperationStatus.Rejected,
			effect.Apply(f.Actor.Object, f.Actor.Object, OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []).Status);
		Assert.AreEqual(0.1, needs.AlcoholLitres);
		Assert.IsFalse(f.Actor.Object.Changed);
	}

	[TestMethod]
	public void ExactAlcohol_RealSetterStatusCallbackBodyChangePreservesWriteAndSignalsUncertainty()
	{
		var (f, needs) = Fixture(0.1);
		var effect = Effect(f, -0.07);
		effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object);
		var replacement = new Mock<IBody>(); replacement.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5);
		var output = Mock.Get(f.Actor.Object.OutputHandler);
		output.Setup(x => x.Send("native status callback", It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback(() => f.Actor.SetupGet(x => x.Body).Returns(replacement.Object));
		f.Body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(() =>
		{
			if (needs.AlcoholLitres < 0.1) f.Actor.Object.OutputHandler.Send("native status callback");
			return 5.0;
		});
		Assert.ThrowsException<InvalidOperationException>(() => effect.Apply(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []));
		Assert.AreEqual(0.03, needs.AlcoholLitres, 1e-12);
		Assert.IsTrue(f.Actor.Object.Changed);
		Assert.IsTrue(f.Body.Object.Changed);
		Assert.IsFalse(replacement.Object.Changed);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, effect.Apply(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []).Status);
		Assert.AreEqual(0.03, needs.AlcoholLitres, 1e-12, "An uncertain old-body write must not be replayed.");
	}

	[DataTestMethod]
	[DataRow("NaN", "0", "0")]
	[DataRow("0.01", "0", "0")]
	[DataRow("-0.01", "1", "0")]
	[DataRow("-0.01", "0", "1")]
	public void ExactAlcohol_InvalidXmlRetainsDiagnosticThroughNativeClone(string drunk, string hunger, string thirst)
	{
		var (f, _) = Fixture(0.1);
		var xml = XElement.Parse($"<Effect type='needdelta'><Hunger>{hunger}</Hunger><Thirst>{thirst}</Thirst><Drunk>{drunk}</Drunk><DirectAlcoholRemoval>true</DirectAlcoholRemoval></Effect>");
		var effect = (NeedDeltaEffect)SpellEffectFactory.LoadEffect(xml, f.Spell);
		Assert.IsNotNull(effect.DefinitionError);
		Assert.IsNotNull(((NeedDeltaEffect)effect.Clone()).DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
	}

	[TestMethod]
	public void ExactAlcohol_DirectModeRequiresOneSelectedCharacterWithoutNarrowingLegacyTriggers()
	{
		var (f, _) = Fixture(0.1);
		var effect = Effect(f, -0.01);
		var character = Mock.Of<IMagicTrigger>(x => x.TargetTypes == "character");
		var characters = Mock.Of<IMagicTrigger>(x => x.TargetTypes == "characters");
		Assert.IsTrue(effect.IsCompatibleWithTrigger(character));
		Assert.IsFalse(effect.IsCompatibleWithTrigger(characters));
		var xml = effect.SaveToXml(); xml.Element("DirectAlcoholRemoval")!.Remove();
		Assert.IsTrue(SpellEffectFactory.LoadEffect(xml, f.Spell).IsCompatibleWithTrigger(characters));
	}

	[TestMethod]
	public void ExactAlcohol_ModeRoundTripsAndAbsentModeRetainsLegacyFulfiller()
	{
		var (f, _) = Fixture(0.1);
		var effect = Effect(f, -0.01);
		Assert.IsTrue(((NeedDeltaEffect)effect.Clone()).DirectAlcoholRemoval);
		var xml = effect.SaveToXml(); xml.Element("DirectAlcoholRemoval")!.Remove();
		var legacy = (NeedDeltaEffect)SpellEffectFactory.LoadEffect(xml, f.Spell);
		Assert.AreEqual(MagicEffectOperationStatus.Unknown, legacy.Apply(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []).Status);
		f.Body.Verify(x => x.FulfilNeeds(It.Is<INeedFulfiller>(n => n.AlcoholLitres == -0.01), false), Times.Once);
	}

	private static NeedDeltaEffect Effect(MagicCastingFixture f, double delta) => (NeedDeltaEffect)SpellEffectFactory.LoadEffect(
		new XElement("Effect", new XAttribute("type", "needdelta"), new XElement("Hunger", 0), new XElement("Thirst", 0),
			new XElement("Drunk", delta), new XElement("DirectAlcoholRemoval", true)), f.Spell);

	private static (MagicCastingFixture, ActiveNeedsModel) Fixture(double alcohol)
	{
		var f = new MagicCastingFixture();
		f.Actor.SetupProperty(x => x.Changed, false); f.Body.SetupProperty(x => x.Changed, false);
		f.Body.SetupGet(x => x.Actor).Returns(f.Actor.Object);
		f.Body.SetupGet(x => x.CurrentBloodVolumeLitres).Returns(5);
		var multiplier = new Mock<INeedRateEffect>();
		multiplier.SetupGet(x => x.AppliesToActive).Returns(true);
		multiplier.SetupGet(x => x.DrunkennessMultiplier).Returns(10);
		f.Actor.Setup(x => x.CombinedEffectsOfType<INeedRateEffect>()).Returns([multiplier.Object]);
		var needs = new ActiveNeedsModel(new MudSharp.Models.Character
		{
			AlcoholLitres = alcohol, FoodSatiatedHours = 12, DrinkSatiatedHours = 12, WaterLitres = 0.5, SatiationReserve = 1
		}, f.Actor.Object);
		f.Actor.SetupGet(x => x.NeedsModel).Returns(needs);
		f.Body.SetupGet(x => x.NeedsModel).Returns(needs);
		return (f, needs);
	}

	private static MudSharp.Models.Character Snapshot(INeedsModel needs) => new()
	{
		NeedsModel = needs.ModelName, AlcoholLitres = needs.AlcoholLitres, FoodSatiatedHours = needs.FoodSatiatedHours,
		DrinkSatiatedHours = needs.DrinkSatiatedHours, WaterLitres = needs.WaterLitres, SatiationReserve = needs.SatiationReserve
	};
}
