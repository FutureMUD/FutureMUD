#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Network;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DraggingTimeoutSecurityTests
{
	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void DetachConnection_OrdinaryDraggerOrTarget_SchedulesNormalLogout(bool targetDisconnects, bool guest)
	{
		var owner = CreateCharacter(false);
		var target = CreateCharacter(false);
		var targetEffects = new List<IEffect>();
		target.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(targetEffects.Add);
		var drag = new Dragging(owner.Object, null!, target.Object);
		var actor = targetDisconnects ? target : owner;
		actor.SetupGet(x => x.IsGuest).Returns(guest);
		var effects = targetDisconnects ? targetEffects : new List<IEffect> { drag };
		actor.Setup(x => x.CombinedEffectsOfType<INoTimeOutEffect>()).Returns(() => effects.OfType<INoTimeOutEffect>());

		Detach(actor);

		actor.Verify(x => x.AddEffect(It.IsAny<LinkdeadLogout>(), guest ? TimeSpan.FromSeconds(1) : TimeSpan.FromMinutes(10)), Times.Once);
		actor.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()), Times.Never);
		Assert.IsTrue(drag is INoQuitEffect);
		Assert.IsTrue(targetEffects.Single() is INoQuitEffect);
	}

	[TestMethod]
	public void DetachConnection_CharacterHitchPuller_PreservesTimeoutProtection()
	{
		var actor = CreateCharacter(true);
		var hitch = new CharacterHitch(actor.Object, CreateCharacter(false).Object, 1.0);
		actor.Setup(x => x.CombinedEffectsOfType<INoTimeOutEffect>()).Returns([hitch]);

		Detach(actor);

		actor.Verify(x => x.AddEffect(It.IsAny<LinkdeadLogout>(), It.IsAny<TimeSpan>()), Times.Never);
		actor.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), false), Times.Once);
	}

	private static Mock<ICharacter> CreateCharacter(bool guest)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>());
		actor.SetupGet(x => x.IsGuest).Returns(guest);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		return actor;
	}

	private static void Detach(Mock<ICharacter> actor)
	{
		var context = TestObjectFactory.CreateUninitialized<FuturemudControlContext>();
		var connection = new Mock<IPlayerConnection>();
		connection.As<IAsyncPlayerConnection>();
		typeof(FuturemudControlContext).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(context, actor.Object);
		typeof(FuturemudControlContext).GetField("_connection", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(context, connection.Object);
		((IAccountController)context).DetachConnection();
	}
}
