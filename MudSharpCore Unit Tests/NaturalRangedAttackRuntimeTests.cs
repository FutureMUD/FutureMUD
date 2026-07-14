#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Health;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NaturalRangedAttackRuntimeTests
{
	[TestMethod]
	public void BreathVictimSelection_AlwaysIncludesPrimaryAndDeduplicatesNearbyTargets()
	{
		var primary = new Mock<ICharacter>();
		var assailant = new Mock<ICharacter>();
		var nearby = new Mock<ICharacter>();
		primary.Setup(x => x.LocalThingsAndProximities()).Returns([
			(nearby.Object, Proximity.Immediate),
			(nearby.Object, Proximity.Proximate),
			(assailant.Object, Proximity.Intimate)
		]);

		var victims = BreathWeaponAttackMove.SelectBreathVictims(primary.Object, assailant.Object, 1).ToList();

		CollectionAssert.AreEqual(new[] { primary.Object, nearby.Object }, victims);
	}

	[TestMethod]
	public void ExplosionVictimSelection_IncludesPrimaryAndUsesNearestDuplicateProximity()
	{
		var primary = new Mock<ICharacter>();
		var nearby = new Mock<ICharacter>();
		primary.Setup(x => x.LocalThingsAndProximities()).Returns([
			(nearby.Object, Proximity.Proximate),
			(nearby.Object, Proximity.Immediate),
			(primary.Object, Proximity.Distant)
		]);

		var victims = ExplosiveNaturalAttackMove
			.SelectExplosionVictims(primary.Object, Proximity.Proximate)
			.ToList();

		Assert.AreEqual(2, victims.Count);
		Assert.AreSame(primary.Object, victims[0].Target);
		Assert.AreEqual(Proximity.Intimate, victims[0].Proximity);
		Assert.AreSame(nearby.Object, victims[1].Target);
		Assert.AreEqual(Proximity.Immediate, victims[1].Proximity);
	}

	[TestMethod]
	public void TargetRange_ZeroRangeOnlyAllowsSameCell()
	{
		var firstCell = new Mock<ICell>();
		var secondCell = new Mock<ICell>();
		var assailant = new Mock<ICharacter>();
		var colocated = new Mock<IPerceivable>();
		var remote = new Mock<IPerceivable>();
		assailant.SetupGet(x => x.Location).Returns(firstCell.Object);
		colocated.SetupGet(x => x.Location).Returns(firstCell.Object);
		remote.SetupGet(x => x.Location).Returns(secondCell.Object);

		Assert.IsTrue(NaturalRangedAttackMoveBase.TargetIsInRange(assailant.Object, colocated.Object, 0));
		Assert.IsFalse(NaturalRangedAttackMoveBase.TargetIsInRange(assailant.Object, remote.Object, 0));
	}

	[TestMethod]
	public void FireProfile_RoundTripPreservesReleaseCriticalFields()
	{
		var gameworld = new Mock<IFuturemud>();
		var original = new FireProfile(gameworld.Object)
		{
			Name = "Dragonfire",
			DamageType = DamageType.Burning,
			DamagePerTick = 1.25,
			PainPerTick = 2.5,
			StunPerTick = 0.75,
			ThermalLoadPerTick = 4.5,
			SpreadChance = 0.35,
			MinimumOxidation = 0.05,
			SelfOxidising = true,
			TickFrequency = TimeSpan.FromSeconds(7.5)
		};

		var loaded = new FireProfile(original.SaveToXml(), gameworld.Object);

		Assert.AreEqual(original.Name, loaded.Name);
		Assert.AreEqual(original.DamageType, loaded.DamageType);
		Assert.AreEqual(original.DamagePerTick, loaded.DamagePerTick, 0.0001);
		Assert.AreEqual(original.PainPerTick, loaded.PainPerTick, 0.0001);
		Assert.AreEqual(original.StunPerTick, loaded.StunPerTick, 0.0001);
		Assert.AreEqual(original.ThermalLoadPerTick, loaded.ThermalLoadPerTick, 0.0001);
		Assert.AreEqual(original.SpreadChance, loaded.SpreadChance, 0.0001);
		Assert.AreEqual(original.MinimumOxidation, loaded.MinimumOxidation, 0.0001);
		Assert.AreEqual(original.SelfOxidising, loaded.SelfOxidising);
		Assert.AreEqual(original.TickFrequency, loaded.TickFrequency);
	}

	[TestMethod]
	public void FireProfile_LoadClampsInvalidTickInterval()
	{
		var gameworld = new Mock<IFuturemud>();
		var root = new XElement("FireProfile", new XElement("TickFrequencySeconds", 0));

		var profile = new FireProfile(root, gameworld.Object);

		Assert.AreEqual(TimeSpan.FromSeconds(0.1), profile.TickFrequency);
	}

	[TestMethod]
	public void FireExtinguishing_UsesConfiguredLiquidTags()
	{
		var tag = new Mock<ITag>();
		var matchingLiquid = new Mock<ILiquid>();
		var otherLiquid = new Mock<ILiquid>();
		var profile = new Mock<IFireProfile>();
		profile.SetupGet(x => x.ExtinguishTags).Returns([tag.Object]);
		matchingLiquid.Setup(x => x.IsA(tag.Object)).Returns(true);
		otherLiquid.Setup(x => x.IsA(tag.Object)).Returns(false);

		Assert.IsTrue(MudSharp.Effects.Concrete.OnFire.IsExtinguishing(profile.Object, [matchingLiquid.Object]));
		Assert.IsFalse(MudSharp.Effects.Concrete.OnFire.IsExtinguishing(profile.Object, [otherLiquid.Object]));
	}

	[TestMethod]
	public void LiquidSurfaceReaction_LoadsInvariantAmounts()
	{
		var gameworld = new Mock<IFuturemud>();
		var root = XElement.Parse(
			"<Reaction DamageType=\"12\" DamagePerTick=\"1.25\" PainPerTick=\"2.5\" StunPerTick=\"0.75\"><Tags /></Reaction>");

		var reaction = new LiquidSurfaceReaction(root, gameworld.Object);

		Assert.AreEqual(1.25, reaction.DamagePerTick, 0.0001);
		Assert.AreEqual(2.5, reaction.PainPerTick, 0.0001);
		Assert.AreEqual(0.75, reaction.StunPerTick, 0.0001);
	}
}
