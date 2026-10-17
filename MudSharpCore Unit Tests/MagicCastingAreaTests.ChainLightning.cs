using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;

#nullable enable
namespace MudSharp_Unit_Tests;

public partial class MagicCastingAreaTests
{
	private static void BindChain(AreaFixture fixture, bool taggedTerrain = false, bool waterPlane = false)
	{
		var tags = new[] { Mock.Of<ITag>(x => x.Id == 51), Mock.Of<ITag>(x => x.Id == 52) };
		var planes = new[] { Mock.Of<IPlane>(x => x.Id == 61), Mock.Of<IPlane>(x => x.Id == 62) };
		fixture.F.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => tags));
		fixture.F.World.SetupGet(x => x.Planes).Returns(MagicCastingFixture.Collection(() => planes));
		var terrain = new Mock<ITerrain>();
		terrain.Setup(x => x.IsA(tags[0])).Returns(taggedTerrain);
		var overlay = Mock.Of<IRoomOverlay>(x => x.Terrain == terrain.Object);
		Mock.Get(fixture.F.Actor.Object.Location).SetupGet(x => x.CurrentOverlay).Returns(overlay);
		Mock.Get(fixture.F.Actor.Object.Body).SetupGet(x => x.BasePlanarPresence)
			.Returns(PlanarPresenceDefinition.DefaultMaterial(waterPlane ? 61 : 1));
		fixture.Build("grades area applications 15");
		fixture.Build("grades area applicationsgrade 1 1");
		fixture.Build("grades area chain 51 52 61 62");
	}

	[TestMethod]
	public void ChainLightning_SharedInclusiveRoll_NormalAndAmplifiedCounts_OnePayment()
	{
		var normal = new AreaFixture("chainlightning"); var other = normal.Target(); BindChain(normal);
		for (var i = 0; i < 3; i++) Assert.AreEqual(4, normal.Service.Quote(normal.Intent()).Invocation!.Area!.MaximumApplications);
		Assert.AreEqual(0, normal.AreaDraws);
		normal.Picks.Enqueue(0); // The shared roll is 3*grade = 9, before target draws.
		foreach (var pick in new[] { 0, 1, 0, 1 }) normal.Picks.Enqueue(pick);
		var cast = normal.Service.Cast(normal.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, cast.Status, cast.Message);
		CollectionAssert.AreEqual(new[] { 2.25, 9.0, 2.25, 9.0 }, normal.Damage.Select(x => x.Amount).ToArray());
		Assert.AreEqual(5, normal.AreaDraws); Assert.AreEqual(1, normal.F.Rolls);
		var receipt = XElement.Parse(normal.F.Store.Operations[cast.OperationId!.Value].Definition).Element("Area")!;
		Assert.AreEqual(9.0, (double)receipt.Attribute("sharedDamage")!);
		Assert.AreEqual(false, (bool)receipt.Attribute("amplified")!);
		CollectionAssert.AreEqual(new[] { normal.F.Actor.Object, other.Object, normal.F.Actor.Object, other.Object },
			normal.Damage.Select(x => x.Target).ToArray());

		foreach (var (tagged, water) in new[] { (true, false), (false, true) })
		{
			var amplified = new AreaFixture("chainlightning"); var second = amplified.Target(); BindChain(amplified, tagged, water);
			if (water) Mock.Get(second.Object.Body).SetupGet(x => x.BasePlanarPresence)
				.Returns(PlanarPresenceDefinition.DefaultMaterial(61));
			Assert.AreEqual(7, amplified.Service.Quote(amplified.Intent()).Invocation!.Area!.MaximumApplications);
			amplified.Picks.Enqueue(6); // Inclusive high end, 5*grade = 15.
			foreach (var pick in new[] { 0, 1, 0, 1, 0, 1, 0 }) amplified.Picks.Enqueue(pick);
			var result = amplified.Service.Cast(amplified.Intent());
			Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
			CollectionAssert.AreEqual(new[] { 3.75, 15.0, 3.75, 15.0, 3.75, 15.0, 3.75 },
				amplified.Damage.Select(x => x.Amount).ToArray());
			Assert.AreEqual(8, amplified.AreaDraws); Assert.AreEqual(1, amplified.F.Rolls);
			var area = XElement.Parse(amplified.F.Store.Operations[result.OperationId!.Value].Definition).Element("Area")!;
			Assert.AreEqual(true, (bool)area.Attribute("amplified")!);
			Assert.AreEqual(15.0, (double)area.Attribute("sharedDamage")!);
		}
	}

	[TestMethod]
	public void ChainLightning_ReloadsTypedPolicy_RejectsManifestAndSourceDriftBeforePayment()
	{
		var a = new AreaFixture("chainlightning"); a.Target(); BindChain(a);
		var cold = new MagicSpell(a.F.Spell.SnapshotModel(), a.F.World.Object);
		Assert.AreEqual(4, cold.GradeProfile!.Area!.ApplicationsAtGrade(3));
		Assert.AreEqual(7, cold.GradeProfile.Area.ApplicationsAtGrade(3, true));
		Assert.IsFalse(a.Service.Quote(a.Intent() with { Mode = MagicCastingMode.Manifest, Targets = "self" }).Allowed);
		var room = Mock.Get(a.F.Actor.Object.Location);
		a.F.Checkpoint = stage =>
		{
			if (stage != "BeforePayment") return;
			room.SetupGet(x => x.CurrentOverlay).Returns(Mock.Of<IRoomOverlay>());
		};
		var result = a.Service.Cast(a.Intent());
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(0, a.AreaDraws); Assert.AreEqual(0, a.F.Rolls);
	}

	[TestMethod]
	public void ChainLightning_DamageTagAllowsMatchingIncomingWardToAbsorbSelectedBolt()
	{
		var a = new AreaFixture("chainlightning"); var target = a.Target(); BindChain(a);
		var effects = (System.Collections.Generic.List<IMagicSpellEffectTemplate>)a.F.Spell.SpellEffects;
		effects.Clear();
		effects.Add(SpellEffectFactory.LoadEffect(XElement.Parse(
			"<Effect type='damage'><DamageType>11</DamageType><DamageExpression>0</DamageExpression><InterdictionTag>arm.source.chain_lightning</InterdictionTag><Bodypart>0</Bodypart><Limb>-1</Limb></Effect>"), a.F.Spell));
		var ward = new Mock<IMagicInterdictionEffect>();
		var contextual = ward.As<IMagicContextualInterdictionEffect>();
		ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Fail);
		contextual.Setup(x => x.ShouldInterdict(It.IsAny<MagicInterdictionContext>()))
			.Returns<MagicInterdictionContext>(context => context.Tags.Any(x => x.Tag == ArmageddonLeapingLightningSpellContent.ProtectionTag));
		target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
			.Returns([ward.Object]);
		a.Picks.Enqueue(0);
		foreach (var pick in new[] { 1, 0, 1, 0 }) a.Picks.Enqueue(pick);
		var result = a.Service.Cast(a.Intent());
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(2, a.Damage.Count);
		Assert.IsTrue(a.Damage.All(x => ReferenceEquals(x.Target, a.F.Actor.Object)));
		Assert.AreEqual(1, a.F.Rolls);
	}
}
