#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterInstanceFocusServiceTests
{
	[TestMethod]
	public void CanFocus_PlayerFocusableSecondary_ReturnsSuccess()
	{
		var fixture = CreateFixture();

		var result = CharacterInstanceFocusService.CanFocus(fixture.Actor.Object, fixture.Secondary.Object);

		Assert.IsTrue(result.Success);
		Assert.AreSame(fixture.Secondary.Object, result.Target);
	}

	[TestMethod]
	public void CanFocus_PassiveSecondary_ReturnsFailure()
	{
		var fixture = CreateFixture();
		fixture.Secondary.SetupGet(x => x.IsControllable).Returns(false);
		fixture.Secondary.SetupGet(x => x.ControlPolicy).Returns(CharacterInstanceControlPolicy.NotControllable);

		var result = CharacterInstanceFocusService.CanFocus(fixture.Actor.Object, fixture.Secondary.Object);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "not controllable");
	}

	[TestMethod]
	public void CanFocus_OtherIdentity_ReturnsFailure()
	{
		var fixture = CreateFixture();
		var otherIdentity = new Mock<ICharacterIdentity>();
		otherIdentity.SetupGet(x => x.Id).Returns(99);
		fixture.Secondary.SetupGet(x => x.Identity).Returns(otherIdentity.Object);

		var result = CharacterInstanceFocusService.CanFocus(fixture.Actor.Object, fixture.Secondary.Object);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "own identity");
	}

	[TestMethod]
	public void CanFocus_UnloadedInstance_ReturnsFailure()
	{
		var fixture = CreateFixture();
		fixture.Identity.SetupGet(x => x.Instances).Returns(new[] { fixture.Primary.Object });

		var result = CharacterInstanceFocusService.CanFocus(fixture.Actor.Object, fixture.Secondary.Object);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "not currently loaded");
	}

	[TestMethod]
	public void CanFocus_DeadOrStasisInstance_ReturnsFailure()
	{
		var deadFixture = CreateFixture();
		deadFixture.Secondary.SetupGet(x => x.State).Returns(CharacterState.Dead);

		var dead = CharacterInstanceFocusService.CanFocus(deadFixture.Actor.Object, deadFixture.Secondary.Object);

		Assert.IsFalse(dead.Success);
		StringAssert.Contains(dead.Message, "dead");

		var stasisFixture = CreateFixture();
		stasisFixture.Secondary.SetupGet(x => x.State).Returns(CharacterState.Stasis);

		var stasis = CharacterInstanceFocusService.CanFocus(stasisFixture.Actor.Object, stasisFixture.Secondary.Object);

		Assert.IsFalse(stasis.Success);
		StringAssert.Contains(stasis.Message, "stasis");
	}

	[TestMethod]
	public void Focus_ValidSecondary_SetsControllerContextToTarget()
	{
		var fixture = CreateFixture();

		var result = CharacterInstanceFocusService.Focus(fixture.Actor.Object, fixture.Secondary.Object);

		Assert.IsTrue(result.Success);
		fixture.Controller.Verify(x => x.SetContext(fixture.Secondary.Object), Times.Once);
		fixture.SecondaryOutput.Verify(x => x.Send(
				It.Is<string>(text => text.Contains("focus shifts") && !text.Contains("#2")),
				true,
				false),
			Times.Once);
	}

	[TestMethod]
	public void Focus_InvalidSecondary_DoesNotSetControllerContext()
	{
		var fixture = CreateFixture();
		fixture.Secondary.SetupGet(x => x.IsEmbodied).Returns(false);

		var result = CharacterInstanceFocusService.Focus(fixture.Actor.Object, fixture.Secondary.Object);

		Assert.IsFalse(result.Success);
		fixture.Controller.Verify(x => x.SetContext(It.IsAny<IControllable>()), Times.Never);
	}

	[TestMethod]
	public void QuitThroughPrimary_FromSecondary_ReturnsControllerToPrimaryAndQuitsPrimary()
	{
		var fixture = CreateFixture(actorIsSecondary: true);
		fixture.Primary.Setup(x => x.Quit(false)).Returns(true);

		var result = CharacterInstanceFocusService.QuitThroughPrimary(fixture.Actor.Object);

		Assert.IsTrue(result);
		fixture.Controller.Verify(x => x.SetContext(fixture.Primary.Object), Times.Once);
		fixture.Primary.Verify(x => x.Quit(false), Times.Once);
		fixture.Secondary.Verify(x => x.Quit(false), Times.Never);
	}

	private static FocusFixture CreateFixture(bool actorIsSecondary = false)
	{
		var identity = new Mock<ICharacterIdentity>();
		var primary = new Mock<ICharacterInstance>();
		var secondary = new Mock<ICharacterInstance>();
		var controller = new Mock<ICharacterController>();
		var primaryOutput = new Mock<IOutputHandler>();
		var secondaryOutput = new Mock<IOutputHandler>();
		var primaryBody = CreateBody("primary body");
		var secondaryBody = CreateBody("projection");

		identity.SetupGet(x => x.Id).Returns(10);
		identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
		identity.SetupGet(x => x.FocusedInstance).Returns(actorIsSecondary ? secondary.Object : primary.Object);
		identity.SetupGet(x => x.Instances).Returns(new[] { primary.Object, secondary.Object });
		identity.SetupGet(x => x.Forms).Returns(System.Array.Empty<ICharacterForm>());

		SetupInstance(primary, identity.Object, primaryBody.Object, 1, true, true, true,
			CharacterInstanceControlPolicy.PlayerFocusable, primaryOutput.Object);
		primary.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		primary.SetupGet(x => x.IsGuest).Returns(false);

		SetupInstance(secondary, identity.Object, secondaryBody.Object, 2, false, true, true,
			CharacterInstanceControlPolicy.PlayerFocusable, secondaryOutput.Object);
		secondary.SetupGet(x => x.CharacterController).Returns(controller.Object);

		var actor = actorIsSecondary ? secondary : primary;
		actor.SetupGet(x => x.CharacterController).Returns(controller.Object);

		return new FocusFixture(identity, primary, secondary, actor, controller, primaryOutput, secondaryOutput);
	}

	private static Mock<IBody> CreateBody(string prototypeName)
	{
		var prototype = new Mock<IBodyPrototype>();
		prototype.SetupGet(x => x.Name).Returns(prototypeName);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Prototype).Returns(prototype.Object);
		return body;
	}

	private static void SetupInstance(
		Mock<ICharacterInstance> instance,
		ICharacterIdentity identity,
		IBody body,
		long instanceId,
		bool primary,
		bool embodied,
		bool controllable,
		CharacterInstanceControlPolicy controlPolicy,
		IOutputHandler outputHandler)
	{
		instance.SetupGet(x => x.Identity).Returns(identity);
		instance.SetupGet(x => x.InstanceId).Returns(instanceId);
		instance.SetupGet(x => x.IsPrimaryInstance).Returns(primary);
		instance.SetupGet(x => x.IsEmbodied).Returns(embodied);
		instance.SetupGet(x => x.IsControllable).Returns(controllable);
		instance.SetupGet(x => x.ControlPolicy).Returns(controlPolicy);
		instance.SetupGet(x => x.State).Returns(CharacterState.Awake);
		instance.SetupGet(x => x.Status).Returns(CharacterStatus.Active);
		instance.SetupGet(x => x.Body).Returns(body);
		instance.SetupGet(x => x.OutputHandler).Returns(outputHandler);
	}

	private sealed record FocusFixture(
		Mock<ICharacterIdentity> Identity,
		Mock<ICharacterInstance> Primary,
		Mock<ICharacterInstance> Secondary,
		Mock<ICharacterInstance> Actor,
		Mock<ICharacterController> Controller,
		Mock<IOutputHandler> PrimaryOutput,
		Mock<IOutputHandler> SecondaryOutput
	);
}
