#nullable enable

using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Movement;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterTeleportLayerTests
{
	[DataTestMethod]
	[DataRow(RoomLayer.InTrees)]
	[DataRow(RoomLayer.HighInAir)]
	[DataRow(RoomLayer.Underwater)]
	public void Teleport_PassesDestinationLayerToRoomForCharacterAndRider(RoomLayer layer)
	{
		var origin = new Mock<IRoom>();
		var destination = new Mock<IRoom>();
		var rider = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		rider.SetupGet(x => x.Location).Returns(origin.Object);
		var actor = TeleportCharacter.Create(origin.Object, rider.Object);
		actor.Teleport(destination.Object, layer, false, false);
		destination.Verify(x => x.Enter(actor, null, false, layer), Times.Once);
		destination.Verify(x => x.Enter(rider.Object, null, false, layer), Times.Once);
		Assert.AreEqual(layer, actor.RoomLayer);
	}

	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void Teleport_CancelsCompanionsOwnMovementBeforeRelocation(bool primaryMoving, bool dragging)
	{
		var origin = new Mock<IRoom>();
		var destination = new Mock<IRoom>();
		var companion = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		companion.SetupGet(x => x.Location).Returns(origin.Object);
		var actor = TeleportCharacter.Create(origin.Object, companion.Object);
		var pending = new Mock<IMovement>();
		var cancelled = false;
		companion.SetupGet(x => x.Movement).Returns(() => cancelled ? null : pending.Object);
		pending.Setup(x => x.CancelForMoverOnly(companion.Object, false))
			.Callback(() => cancelled = true).Returns(true);
		origin.Setup(x => x.Leave(companion.Object)).Callback(() => Assert.IsTrue(cancelled));
		destination.Setup(x => x.Enter(companion.Object, null, false, RoomLayer.GroundLevel))
			.Callback(() => Assert.IsTrue(cancelled));
		if (primaryMoving)
		{
			var primary = new Mock<IMovement>();
			typeof(MudSharp.Character.Character).GetField("_movement", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(actor, primary.Object);
			primary.Setup(x => x.CancelForMoverOnly(actor, false)).Callback(() =>
				typeof(MudSharp.Character.Character).GetField("_movement", BindingFlags.Instance | BindingFlags.NonPublic)!
					.SetValue(actor, null)).Returns(true);
		}

		if (dragging)
		{
			typeof(MudSharp.Character.Character).GetField("_riders", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(actor, new List<ICharacter>());
			var drag = new Mock<IDragging>();
			drag.SetupGet(x => x.Target).Returns(companion.Object);
			drag.SetupGet(x => x.CharacterDraggers).Returns(new[] { actor });
			actor.EffectHandler.AddEffect(drag.Object);
		}

		actor.Teleport(destination.Object, RoomLayer.GroundLevel, dragging, false);
		pending.Verify(x => x.CancelForMoverOnly(companion.Object, false), Times.Once);
		Assert.IsNull(companion.Object.Movement);
		origin.Verify(x => x.Leave(companion.Object), Times.Once);
		destination.Verify(x => x.Enter(companion.Object, null, false, RoomLayer.GroundLevel), Times.Once);
	}

	private sealed class TeleportCharacter : MudSharp.Character.Character
	{
		private TeleportCharacter() : base(null!, null!, true) { }

		public static TeleportCharacter Create(IRoom origin, ICharacter rider)
		{
			var actor = TestObjectFactory.CreateUninitialized<TeleportCharacter>();
			var body = new Mock<IBody>();
			body.SetupProperty(x => x.PositionState, PositionStanding.Instance);
			actor.Body = body.Object;
			actor.Location = origin;
			actor.SetNoSave(true);
			typeof(PerceivedItem).GetProperty(nameof(EffectHandler))!.SetValue(actor, new EffectHandler(actor));
			typeof(MudSharp.Character.Character).GetField("_riders", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(actor, new List<ICharacter> { rider });
			return actor;
		}

		public override RoomLayer RoomLayer { get; set; }
		public override bool HandleEvent(EventType type, params dynamic[] arguments) => false;
	}
}
