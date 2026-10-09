#nullable enable

using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Movement;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CarriedFlightSecurityTests
{
	[DataTestMethod]
	[DataRow("rider")]
	[DataRow("dragged")]
	[DataRow("voluntary")]
	public void StartMove_FlyingExit_OnlyIndependentMoverReceivesFlyingPosture(string role)
	{
		IPositionState initial = role switch
		{
			"rider" => PositionRiding.Instance,
			"dragged" => PositionProne.Instance,
			_ => PositionStanding.Instance
		};
		var body = new Mock<IBody>();
		body.SetupProperty(x => x.PositionState, initial);
		var actor = TestObjectFactory.CreateUninitialized<MudSharp.Character.Character>();
		actor.SetNoSave(true);
		typeof(MudSharp.Character.Character).GetProperty(nameof(MudSharp.Character.Character.Body))!.SetValue(actor, body.Object);
		typeof(PerceivedItem).GetProperty(nameof(PerceivedItem.EffectHandler))!.SetValue(actor, new EffectHandler(actor));
		var room = new Mock<IRoom>();
		typeof(PerceivedItem).GetProperty(nameof(PerceivedItem.Location))!.SetValue(actor, room.Object);
		var exit = new Mock<IRoomExit>();
		exit.SetupGet(x => x.IsFlyExit).Returns(true);
		var movement = new Mock<IMovement>();
		movement.SetupGet(x => x.Exit).Returns(exit.Object);
		movement.Setup(x => x.IsConsensualMover(actor)).Returns(role == "voluntary");

		actor.StartMove(movement.Object);

		Assert.AreSame(role == "voluntary" ? PositionFlying.Instance : initial, actor.PositionState);
		if (role == "rider")
		{
			var mount = TestObjectFactory.CreateUninitialized<MudSharp.Character.Character>();
			mount.SetNoSave(true);
			typeof(MudSharp.Character.Character).GetField("_riders", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(mount, new List<ICharacter> { actor });
			actor.RidingMount = mount;
			typeof(MudSharp.Character.Character).GetMethod("ClearRidingState", BindingFlags.Instance | BindingFlags.NonPublic)!
				.Invoke(mount, [actor]);
			Assert.IsNull(actor.RidingMount);
			Assert.AreSame(PositionStanding.Instance, actor.PositionState);
		}
	}
}
