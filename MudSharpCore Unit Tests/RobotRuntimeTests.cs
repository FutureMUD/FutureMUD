#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Body.Implementations;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.Health.Strategies;
using MudSharp.Health.Surgery;
using MudSharp.Health.Wounds;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RobotRuntimeTests
{
	[TestMethod]
	public void IsOrgan_SensorArray_IsTreatedAsAnOrgan()
	{
		Assert.IsTrue(BodypartTypeEnum.SensorArray.IsOrgan());
	}

	[TestMethod]
	public void SensorArrayAllowsPerception_WhenSensorArraysExist_RequiresFunction()
	{
		Assert.IsTrue(Body.SensorArrayAllowsPerception(false, 0.0),
			"Bodies without sensor arrays should fall back to the legacy eye and ear checks.");
		Assert.IsFalse(Body.SensorArrayAllowsPerception(true, 0.0),
			"Bodies with sensor arrays should not perceive when the array is non-functional.");
		Assert.IsTrue(Body.SensorArrayAllowsPerception(true, 0.25),
			"Bodies with functioning sensor arrays should be allowed to perceive.");
	}

	[TestMethod]
	public void HasSpeechSynthesizerFunctionForVolume_UsesExpectedThresholds()
	{
		Assert.IsFalse(RobotCommunicationStrategy.HasSpeechSynthesizerFunctionForVolume(0.49, AudioVolume.Decent));
		Assert.IsTrue(RobotCommunicationStrategy.HasSpeechSynthesizerFunctionForVolume(0.5, AudioVolume.Decent));
		Assert.IsFalse(RobotCommunicationStrategy.HasSpeechSynthesizerFunctionForVolume(0.89, AudioVolume.ExtremelyLoud));
		Assert.IsTrue(RobotCommunicationStrategy.HasSpeechSynthesizerFunctionForVolume(0.9, AudioVolume.ExtremelyLoud));
	}

	[TestMethod]
	public void FluidLossDescriptionForPrompt_UsesConfiguredLiquidNames()
	{
		Assert.AreEqual("Machine Oil", RobotHealthStrategy.CirculatoryFluidPromptLabel("machine oil"));
		StringAssert.Contains(RobotHealthStrategy.FluidLossDescriptionForPrompt(0.5, "machine oil"), "machine oil");
		StringAssert.Contains(RobotHealthStrategy.FluidLossDescriptionForPrompt(0.19, "hydraulic fluid"),
			"critical hydraulic fluid loss");
	}

	[TestMethod]
	public void EvaluateStatusFromVitals_PositronicBrainAndPowerCoreFailures_UseRobotSpecificOutcomes()
	{
		Assert.AreEqual(
			HealthTickResult.Dead,
			RobotHealthStrategy.EvaluateStatusFromVitals(false, 0.0, null, 1.0, 10.0, 0.0, 0.0, 100.0, false),
			"Losing the positronic brain should kill the robot.");
		Assert.AreEqual(
			HealthTickResult.Unconscious,
			RobotHealthStrategy.EvaluateStatusFromVitals(false, 1.0, null, 0.0, 10.0, 0.0, 0.0, 100.0, false),
			"Losing the power core should render the robot unconscious rather than dead.");
		Assert.AreEqual(
			HealthTickResult.None,
			RobotHealthStrategy.EvaluateStatusFromVitals(false, 1.0, null, 1.0, 10.0, 0.0, 120.0, 100.0, true),
			"Prevent-pass-out protections should bypass the stun-based unconsciousness branch.");
	}

	[TestMethod]
	public void MatchesPermissableBodypart_CountsAsTargetsAreHonoured()
	{
		var targetedPart = new Mock<IBodypart>();
		var actualPart = new Mock<IBodypart>();
		actualPart.Setup(x => x.CountsAs(targetedPart.Object)).Returns(true);

		Assert.IsTrue(
			BodypartSpecificSurgicalProcedure.MatchesPermissableBodypart(
				new[] { targetedPart.Object },
				false,
				actualPart.Object));
		Assert.IsFalse(
			BodypartSpecificSurgicalProcedure.MatchesPermissableBodypart(
				new[] { targetedPart.Object },
				true,
				actualPart.Object));
	}

	[TestMethod]
	public void MatchesPermissableOrganTarget_CountsAsTargetsAreHonoured()
	{
		var targetedPart = new Mock<IBodypart>();
		var actualOrgan = new Mock<IOrganProto>();
		actualOrgan.Setup(x => x.CountsAs(targetedPart.Object)).Returns(true);

		Assert.IsTrue(
			OrganViaBodypartProcedure.MatchesPermissableOrganTarget(
				new[] { targetedPart.Object },
				false,
				actualOrgan.Object));
		Assert.IsFalse(
			OrganViaBodypartProcedure.MatchesPermissableOrganTarget(
				new[] { targetedPart.Object },
				true,
				actualOrgan.Object));
	}

	[TestMethod]
	public void RobotWound_CurrentPain_IsAlwaysZero()
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);

		var bodypart = new Mock<IBodypart>();
		bodypart.SetupGet(x => x.DamageModifier).Returns(1.0);
		bodypart.SetupGet(x => x.StunModifier).Returns(1.0);

		var body = new Mock<IBody>();
		body.Setup(x => x.HitpointsForBodypart(bodypart.Object)).Returns(100.0);

		var owner = new Mock<ICharacter>();
		owner.SetupGet(x => x.Body).Returns(body.Object);
		owner.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns(WoundSeverity.Moderate);

		var wound = new RobotWound(gameworld.Object, owner.Object, 12.0, 6.0, DamageType.Slashing, bodypart.Object,
			null, null, null);

		Assert.AreEqual(0.0, wound.CurrentPain);
		wound.CurrentPain = 999.0;
		Assert.AreEqual(0.0, wound.CurrentPain);
		saveManager.Verify(x => x.AddInitialisation(wound), Times.Once);
	}
}
