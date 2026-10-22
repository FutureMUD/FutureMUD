#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PsionicTemplarPowerPolicyTests
{
	[TestMethod]
	public void MindwipeDuration_MatchesTenMinuteSourceTicks()
	{
		Assert.AreEqual(TimeSpan.FromMinutes(10), PsionicMindwipePolicy.Duration(0));
		Assert.AreEqual(TimeSpan.FromMinutes(10), PsionicMindwipePolicy.Duration(29.999));
		Assert.AreEqual(TimeSpan.FromMinutes(20), PsionicMindwipePolicy.Duration(30));
		Assert.AreEqual(TimeSpan.FromMinutes(80), PsionicMindwipePolicy.Duration(90));
		Assert.AreEqual(TimeSpan.FromMinutes(80), PsionicMindwipePolicy.Duration(100));
		Assert.AreEqual(1, PsionicMindwipePolicy.DurationTicks(double.NaN));
	}

	[TestMethod]
	public void MindwipeAndMindblastCosts_UseConfiguredFractionsOfMaximums()
	{
		Assert.AreEqual(25.0, PsionicMindwipePolicy.FractionOfMaximum(100.0, 0.25), 0.000001);
		Assert.AreEqual(0.0, PsionicMindwipePolicy.FractionOfMaximum(double.PositiveInfinity, 0.25));
		Assert.AreEqual(0.0, PsionicMindwipePolicy.FractionOfMaximum(100.0, double.NaN));
		Assert.AreEqual(25.0, PsionicMindwipePolicy.QuarterMaximum(100.0), 0.000001);
	}

	[TestMethod]
	public void ControlCommandPolicy_RejectsSensitiveRootsButAllowsOrdinaryCommands()
	{
		Assert.IsTrue(PsionicTrafficHelper.IsBlockedCommandRoot("quit"));
		Assert.IsTrue(PsionicTrafficHelper.IsBlockedCommandRoot("admin password reset"));
		Assert.IsFalse(PsionicTrafficHelper.IsBlockedCommandRoot("look around"));
	}

	[TestMethod]
	public void InvoluntaryMentalTraffic_RespectsIgnoreForce()
	{
		var target = new Mock<ICharacter>();
		target.Setup(x => x.AffectedBy<IIgnoreForceEffect>()).Returns(true);
		Assert.IsFalse(PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target.Object));
		target.Setup(x => x.AffectedBy<IIgnoreForceEffect>()).Returns(false);
		Assert.IsTrue(PsionicTrafficHelper.CanReceiveInvoluntaryMentalTraffic(target.Object));
	}

	[TestMethod]
	public void PsionicDome_BlocksRemoteTrafficAcrossWardBoundary()
	{
		var source = new Mock<ICharacter>();
		var target = new Mock<ICharacter>();
		var sourceRoom = new Mock<IRoom>();
		var wardRoom = new Mock<IRoom>();
		source.Setup(x => x.Location).Returns(sourceRoom.Object);
		target.Setup(x => x.Location).Returns(wardRoom.Object);
		sourceRoom.Setup(x => x.Characters).Returns([source.Object]);
		wardRoom.Setup(x => x.Characters).Returns([target.Object]);
		var dome = new Mock<IPsionicDomeEffect>();
		dome.Setup(x => x.AppliesTo(target.Object)).Returns(true);
		target.Setup(x => x.EffectsOfType<IPsionicDomeEffect>(It.IsAny<Predicate<IPsionicDomeEffect>>()))
			.Returns((Predicate<IPsionicDomeEffect>? predicate) =>
				new[] { dome.Object }.Where(x => predicate is null || predicate(x)));
		Assert.IsTrue(PsionicTrafficHelper.CrossesDome(source.Object, target.Object));
	}
}
