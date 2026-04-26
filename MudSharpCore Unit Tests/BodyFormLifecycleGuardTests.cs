#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using ConcreteBody = MudSharp.Body.Implementations.Body;
using ConcreteCharacter = MudSharp.Character.Character;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyFormLifecycleGuardTests
{
	[TestMethod]
	public void BodyProcessStarters_DormantForm_DoNotTouchRuntimeSchedulers()
	{
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		var activeBody = new Mock<IBody>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.CurrentBody).Returns(activeBody.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		body.Actor = actor.Object;

		body.StartHealthTick();
		body.StartStaminaTick();
		body.CheckDrugTick();
		body.CheckHealthStatus();
		body.DoBreathing();
	}

	[TestMethod]
	public void BodyProcessStarters_DeadCurrentBody_DoNotTouchRuntimeSchedulers()
	{
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.CurrentBody).Returns(body);
		actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		body.Actor = actor.Object;

		body.StartHealthTick();
		body.StartStaminaTick();
		body.CheckDrugTick();
		body.CheckHealthStatus();
		body.DoBreathing();
	}

	[TestMethod]
	public void CharacterNeedsHeartbeat_StasisCharacter_DoesNotTouchNeedsModel()
	{
		var character = TestObjectFactory.CreateUninitialized<ConcreteCharacter>();
		typeof(ConcreteCharacter)
			.GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(character, CharacterState.Stasis);

		character.StartNeedsHeartbeat();
		character.NeedsHeartbeat();
	}
}
