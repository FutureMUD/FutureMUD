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
using MudSharp.Events;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterTeleportLayerTests
{
	[DataTestMethod]
	[DataRow(RoomLayer.InTrees)]
	[DataRow(RoomLayer.HighInAir)]
	[DataRow(RoomLayer.Underwater)]
	public void Teleport_PassesDestinationLayerToCellForCharacterAndRider(RoomLayer layer)
	{
		var origin = new Mock<ICell>();
		var destination = new Mock<ICell>();
		var rider = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		rider.SetupGet(x => x.Location).Returns(origin.Object);
		var actor = TeleportCharacter.Create(origin.Object, rider.Object);
		actor.Teleport(destination.Object, layer, false, false);
		destination.Verify(x => x.Enter(actor, null, false, layer), Times.Once);
		destination.Verify(x => x.Enter(rider.Object, null, false, layer), Times.Once);
		Assert.AreEqual(layer, actor.RoomLayer);
	}

	private sealed class TeleportCharacter : MudSharp.Character.Character
	{
		private TeleportCharacter() : base(null!, null!, true) { }

		public static TeleportCharacter Create(ICell origin, ICharacter rider)
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
