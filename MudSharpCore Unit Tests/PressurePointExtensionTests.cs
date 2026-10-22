#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.AuxiliaryEffects;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PressurePointExtensionTests
{
	[DataTestMethod]
	[DataRow("stomach", "stomach")]
	[DataRow("leftarm", "leftarm")]
	[DataRow("left arm", "leftarm")]
	[DataRow("\"right arm\"", "rightarm")]
	[DataRow("RIGHTLEG", "rightleg")]
	[DataRow("right leg", "rightleg")]
	[DataRow("\"left leg\"", "leftleg")]
	[DataRow("neck", "neck")]
	[DataRow("spine", "spine")]
	[DataRow("throat", "throat")]
	public void ParseSelection_AcceptedSyntax_CanonicalLocation(string input, string expected)
	{
		Assert.IsTrue(PressurePointAnatomy.TryParseSelection(new StringStack(input), out var selection));
		Assert.AreEqual(expected, selection);
	}

	[DataTestMethod]
	[DataRow("")]
	[DataRow("left")]
	[DataRow("arm")]
	[DataRow("head")]
	[DataRow("stomach throat")]
	[DataRow("left arm extra")]
	public void ParseSelection_InvalidOrExcessArguments_Rejects(string input) =>
		Assert.IsFalse(PressurePointAnatomy.TryParseSelection(new StringStack(input), out _));

	[DataTestMethod]
	[DataRow("leftarm", "lupperarm", LimbType.Arm, Alignment.Left)]
	[DataRow("rightarm", "rupperarm", LimbType.Arm, Alignment.Right)]
	[DataRow("leftleg", "lthigh", LimbType.Leg, Alignment.Left)]
	[DataRow("rightleg", "rthigh", LimbType.Leg, Alignment.Right)]
	public void ResolveLimb_ConfiguredPartAndSide_SelectsExactLimb(string location, string alias, LimbType type, Alignment side)
	{
		var body = new Mock<IBody>();
		var part = Part(alias, side);
		var limb = Limb(17, part, type);
		body.SetupGet(x => x.Bodyparts).Returns([part]);
		body.SetupGet(x => x.Limbs).Returns([limb]);
		body.Setup(x => x.GetLimbFor(part)).Returns(limb);
		Assert.AreSame(limb, PressurePointAnatomy.ResolveLimb(body.Object, location, PressurePointAnatomy.StockAliases[location]));
		Assert.IsNull(PressurePointAnatomy.ResolveLimb(body.Object,
			location.StartsWith("left") ? location.Replace("left", "right") : location.Replace("right", "left"), [alias]));
		Assert.IsNull(PressurePointAnatomy.ResolveLimb(body.Object, location, ["different"]));
	}

	[TestMethod]
	public void ResolveLimb_MultipleCompatibleLimbs_RejectsRatherThanChoosingFirst()
	{
		var body = new Mock<IBody>();
		var parts = new[] { Part("tentacle", Alignment.Left), Part("tentacle", Alignment.Left) };
		var limbs = new[] { Limb(1, parts[0], LimbType.Arm), Limb(2, parts[1], LimbType.Arm) };
		body.SetupGet(x => x.Bodyparts).Returns(parts);
		body.SetupGet(x => x.Limbs).Returns(limbs);
		for (var i = 0; i < parts.Length; i++) body.Setup(x => x.GetLimbFor(parts[i])).Returns(limbs[i]);
		Assert.IsNull(PressurePointAnatomy.ResolveLimb(body.Object, "leftarm", ["tentacle"]));
	}

	[TestMethod]
	public void LegacyConfiguration_UsesNewDefaultsAndPreservesCustomizedXml()
	{
		var fixture = new Fixture();
		var effect = new PressurePoint(fixture.Xml, fixture.World.Object);
		Assert.AreEqual(45.0, effect.StomachDurationSeconds);
		Assert.AreEqual(4.0, effect.NauseaIntensity);
		Assert.AreEqual(8, effect.Save().Element("Anatomy")!.Elements().Count());
		Assert.AreEqual(5.0, (double)effect.Save().Attribute("leftarmseconds")!);
		var configured = effect.Save();
		configured.SetAttributeValue("stomachseconds", 63);
		configured.SetAttributeValue("nauseaintensity", 2.5);
		configured.SetAttributeValue("leftarmseconds", 8);
		configured.Element("Anatomy")!.Elements().Single(x => (string?)x.Attribute("name") == "leftarm").SetAttributeValue("parts", "tentacle");
		var reloaded = new PressurePoint(configured, fixture.World.Object).Save();
		Assert.AreEqual(63.0, (double)reloaded.Attribute("stomachseconds")!);
		Assert.AreEqual(2.5, (double)reloaded.Attribute("nauseaintensity")!);
		Assert.AreEqual(8.0, (double)reloaded.Attribute("leftarmseconds")!);
		Assert.AreEqual("tentacle", (string?)reloaded.Element("Anatomy")!.Elements().Single(x => (string?)x.Attribute("name") == "leftarm").Attribute("parts"));
	}

	[TestMethod]
	public void QueuedLimbContext_BodyOrLimbChanges_RejectsWithoutRetargeting()
	{
		var fixture = new Fixture();
		var part = Part("lupperarm", Alignment.Left);
		var limb = Limb(17, part, LimbType.Arm);
		fixture.Body.SetupGet(x => x.Bodyparts).Returns([part]);
		fixture.Body.SetupGet(x => x.Limbs).Returns([limb]);
		fixture.Body.Setup(x => x.GetLimbFor(part)).Returns(limb);
		var effect = new PressurePoint(fixture.Xml, fixture.World.Object);
		Assert.IsTrue(effect.TryParseContext(fixture.Actor.Object, fixture.Target.Object, new StringStack("left arm"), out var context, out var error), error);
		Assert.AreEqual(17, context!.TargetLimbId);
		fixture.Body.SetupGet(x => x.Id).Returns(99);
		Assert.IsFalse(effect.IsValidContext(fixture.Actor.Object, fixture.Target.Object, context, out _));
		fixture.Body.SetupGet(x => x.Id).Returns(12);
		var replacement = Limb(18, part, LimbType.Arm);
		fixture.Body.SetupGet(x => x.Limbs).Returns([replacement]);
		fixture.Body.Setup(x => x.GetLimbFor(part)).Returns(replacement);
		Assert.IsFalse(effect.IsValidContext(fixture.Actor.Object, fixture.Target.Object, context, out _));
	}

	[TestMethod]
	public void StomachContext_AbdomenWithoutStomachOrgan_Rejects()
	{
		var fixture = new Fixture();
		fixture.Body.SetupGet(x => x.Bodyparts).Returns([Part("abdomen", Alignment.Front)]);
		var effect = new PressurePoint(fixture.Xml, fixture.World.Object);
		Assert.IsFalse(effect.TryParseContext(fixture.Actor.Object, fixture.Target.Object, new StringStack("stomach"), out _, out _));
	}

	[DataTestMethod]
	[DataRow("LEFTARM")]
	[DataRow("STOMACH")]
	public void UppercaseContext_UsesBodyStorageForPhysicalEffects(string location)
	{
		var fixture = new Fixture();
		var part = Part("lupperarm", Alignment.Left);
		var limb = Limb(17, part, LimbType.Arm);
		fixture.Body.SetupGet(x => x.Bodyparts).Returns([part, Part("abdomen", Alignment.Front)]);
		fixture.Body.SetupGet(x => x.Organs).Returns([Mock.Of<IOrganProto>(x => x.BodypartType == BodypartTypeEnum.Stomach)]);
		fixture.Body.SetupGet(x => x.Limbs).Returns([limb]);
		fixture.Body.Setup(x => x.GetLimbFor(part)).Returns(limb);
		var effect = new PressurePoint(fixture.Xml, fixture.World.Object);
		var context = new ManualCombatActionContext(location, 12, location == "LEFTARM" ? 17 : null);
		Assert.IsTrue(effect.IsValidContext(fixture.Actor.Object, fixture.Target.Object, context, out var error), error);
		effect.ApplyEffect(fixture.Actor.Object, fixture.Target.Object, null!, context, opposedSuccess: true);
		fixture.Body.Verify(x => x.AddEffect(It.Is<IEffect>(e => e.Owner == fixture.Body.Object), It.IsAny<TimeSpan>()), Times.Once);
		fixture.Target.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void LimbNumbness_GripReleaseDistinctItemsAndPosture_UsesNativeConsequences()
	{
		var body = new Mock<IBody> { DefaultValue = DefaultValue.Mock };
		var part = Part("lthigh", Alignment.Left);
		var hand = Part("lfoot", Alignment.Left);
		var limb = Mock.Get(Limb(17, part, LimbType.Leg));
		limb.SetupGet(x => x.Parts).Returns([part, hand]);
		body.SetupGet(x => x.Limbs).Returns([limb.Object]);
		var item = Mock.Of<IGameItem>();
		body.Setup(x => x.HeldOrWieldedItemsFor(It.IsAny<IBodypart>())).Returns([item]);
		var effect = new LimbNumbnessEffect(body.Object, limb.Object);
		effect.InitialEffect();
		body.Verify(x => x.Drop(item, 0, false, null, true), Times.Once);
		body.Verify(x => x.CheckPositionStillValid(), Times.Once);
		Assert.IsTrue(effect.AppliesToLimb(limb.Object));
		body.SetupGet(x => x.Limbs).Returns(Array.Empty<ILimb>());
		Assert.IsFalse(effect.AppliesToLimb(limb.Object));
		Assert.IsFalse(effect.Applies());
	}

	[TestMethod]
	public void TimedSources_PersistTheirIdentityIntensityAndRemainingDuration()
	{
		var body = new Mock<IBody> { DefaultValue = DefaultValue.Mock };
		var limb = Limb(17, Part("lupperarm", Alignment.Left), LimbType.Arm);
		var numbness = new LimbNumbnessEffect(body.Object, limb);
		var nausea = new MundaneNauseaEffect(body.Object, 4.0);
		foreach (var effect in new IEffect[] { numbness, nausea })
		{
			var xml = effect.SaveToXml(new Dictionary<IEffect, TimeSpan> { [effect] = TimeSpan.FromSeconds(3) });
			Assert.AreEqual("3000", xml.Element("Remaining")!.Value);
			Assert.IsTrue(effect.SavingEffect);
		}
		Assert.AreEqual(17, (int)numbness.SaveToXml([]).Element("Effect")!.Element("Limb")!);
		Assert.AreEqual(4.0, (double)nausea.SaveToXml([]).Element("Effect")!.Element("Intensity")!);
	}

	[TestMethod]
	public void NauseaSourceChanges_RefreshAfterMutationWithoutDrugTick()
	{
		var body = new Mock<IBody> { DefaultValue = DefaultValue.Mock };
		var handler = new EffectHandler(body.Object);
		var source = new MundaneNauseaEffect(body.Object, 4.0);
		var states = new List<bool>();
		body.Setup(x => x.RefreshNausea()).Callback(() => states.Add(handler.Effects.Contains(source)));
		handler.AddEffect(source);
		handler.RemoveEffect(source, true);
		CollectionAssert.AreEqual(new[] { true, false }, states);
		body.Verify(x => x.CheckDrugTick(), Times.Never);
	}

	[TestMethod]
	public void SevereNausea_NoAlcohol_ShowsSymptomsAndCanVomit()
	{
		var body = new Mock<IBody> { DefaultValue = DefaultValue.Mock };
		body.Setup(x => x.Gameworld.GetStaticDouble("NauseaIntensityToBonusConversionRate")).Returns(1.66667);
		body.Setup(x => x.NeedsModel.Status).Returns(NeedsResult.Peckish | NeedsResult.Sated);
		var nausea = new Nausea(body.Object, 4.0, 0.0);
		Assert.AreEqual(-6.66668, nausea.CheckBonus, 0.00001);
		StringAssert.Contains(nausea.ScoreAddendum, "vomit");
		nausea.ExpireEffect();
		body.Verify(x => x.SpendStamina(50), Times.Once);
		body.Verify(x => x.NeedsModel.FulfilNeeds(It.Is<INeedFulfiller>(n => n.WaterLitres < 0.0 && n.SatiationPoints < 0.0), true), Times.Once);
	}

	[TestMethod]
	public void NauseaCallback_DeadOwner_RemovesSymptomInsteadOfLeavingUnscheduledPenalty()
	{
		var body = new Mock<IBody> { DefaultValue = DefaultValue.Mock };
		body.SetupGet(x => x.Actor.State).Returns(CharacterState.Dead);
		var nausea = new Nausea(body.Object, 4.0, 0.0);
		nausea.ExpireEffect();
		body.Verify(x => x.RemoveEffect(nausea, true), Times.Once);
		body.Verify(x => x.Reschedule(nausea, It.IsAny<TimeSpan>()), Times.Never);
	}

	private static IBodypart Part(string name, Alignment side) => Mock.Of<IBodypart>(x =>
		x.Name == name && x.Keywords == new[] { name } && x.Alignment == side);
	private static ILimb Limb(long id, IBodypart root, LimbType type) => Mock.Of<ILimb>(x =>
		x.Id == id && x.Name == "limb" && x.RootBodypart == root && x.LimbType == type && x.Parts == new[] { root });

	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Target { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IBody> Body { get; } = new() { DefaultValue = DefaultValue.Mock };
		public XElement Xml { get; } = new("Effect", new XAttribute("agilitytrait", 1),
			new XAttribute("defensetrait", 2), new XAttribute("defensedifficulty", (int)Difficulty.Normal));
		public Fixture()
		{
			var traits = new All<ITraitDefinition>();
			traits.Add(Mock.Of<ITraitDefinition>(x => x.Id == 1));
			traits.Add(Mock.Of<ITraitDefinition>(x => x.Id == 2));
			World.SetupGet(x => x.Traits).Returns(traits);
			Target.SetupGet(x => x.Body).Returns(Body.Object);
			Target.SetupGet(x => x.Race.BloodLiquid).Returns(Mock.Of<ILiquid>());
			Body.SetupGet(x => x.Id).Returns(12);
			var hand = Mock.Of<IGrab>();
			Actor.SetupGet(x => x.Body.HoldLocs).Returns([hand]);
			Actor.Setup(x => x.Body.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		}
	}
}
