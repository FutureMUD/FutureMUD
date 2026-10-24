#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CorpseRestorationSpatialTests
{
	[TestMethod]
	[DataRow(true, true)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(false, false)]
	public void Removal_RestoresCorpseAtCapturedPointDespiteCleanupRelocation(bool voice, bool route)
	{
		var world = new Mock<IFuturemud>();
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(47);
		var definition = new Mock<IRouteRoomDefinition>();
		definition.SetupGet(x => x.LengthMetres).Returns(10_000);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(0);
		room.SetupGet(x => x.RouteDefinition).Returns(route ? definition.Object : null);
		var rooms = new All<IRoom>(); rooms.Add(room.Object);
		world.SetupGet(x => x.Rooms).Returns(rooms);
		var corpse = new Mock<IGameItem>();
		corpse.SetupGet(x => x.Gameworld).Returns(world.Object);
		corpse.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		var parent = new Mock<IMagicSpellEffectParent>();
		var original = new Mock<ICharacter>();
		var identity = new Mock<ICharacterIdentity>();
		original.SetupGet(x => x.Identity).Returns(identity.Object);
		var animated = new Mock<ICharacterInstance>();
		animated.SetupGet(x => x.InstanceId).Returns(30);
		identity.SetupGet(x => x.Instances).Returns([animated.Object]);
		world.Setup(x => x.TryGetCharacter(10, true)).Returns(original.Object);
		IRoom currentRoom = room.Object;
		double? coordinate = route ? 3_333.25 : null;
		animated.SetupGet(x => x.Location).Returns(() => currentRoom);
		animated.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);
		animated.SetupGet(x => x.RoutePositionMetres).Returns(() => coordinate);
		// Retirement inspects state after the effect captures its spatial frame.
		// Simulate cleanup changing the actor before the corpse is reinserted.
		animated.SetupGet(x => x.State).Returns(() =>
		{
			currentRoom = Mock.Of<IRoom>(); coordinate = 9_000;
			return CharacterState.Awake;
		});
		IEffect effect = voice
			? new SpellDeadSpeakEffect(corpse.Object, parent.Object, 1, 1, 8, 10, 20, 30, 47,
				0, 9, 1, 1, 1, CharacterInstancePersistencePolicy.TemporaryEffectBound, "", "", "", "")
			: new SpellCorpsePossessionEffect(corpse.Object, parent.Object, 1, 1, 8, 10, 20, 30,
				47, 0, 9, CharacterInstancePersistencePolicy.TemporaryEffectBound, null!, "", "", "", "", "");

		effect.RemovalEffect();

		Assert.AreEqual(RoomLayer.InAir, corpse.Object.RoomLayer);
		room.Verify(x => x.Insert(corpse.Object, true), Times.Once);
		if (route)
			corpse.Verify(x => x.MoveTo(new SpatialLocation(room.Object, RoomLayer.InAir, 3_333.25), null, false), Times.Once);
		else
			corpse.Verify(x => x.MoveTo(It.IsAny<SpatialLocation>(), It.IsAny<MudSharp.Construction.Boundary.IRoomExit?>(), It.IsAny<bool>()), Times.Never);
	}
}
