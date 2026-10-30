#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NativeItemExchangeTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Exchange_BothRealCustodyLegsCommitBeforeFirstObserverAndRefusalChangesNeither(bool refuse)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var room = new Mock<IRoom>(); var actors = new All<ICharacter>();
		world.SetupGet(x => x.Actors).Returns(actors);
		world.SetupGet(x => x.Characters).Returns(new All<ICharacter>());
		world.SetupGet(x => x.NPCs).Returns(new All<ICharacter>());
		world.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
		world.SetupGet(x => x.ProximityEventService).Returns(() => null!);
		room.SetupGet(x => x.Characters).Returns(() => actors.ToArray());
		room.Setup(x => x.CanGet(It.IsAny<IGameItem>(), It.IsAny<ICharacter>())).Returns(true);
		var first = Participant(1); var second = Participant(2);
		var offered = Item("offered"); var requested = Item("requested");
		Hold(first.Body, first.Hand, offered); Hold(second.Body, second.Hand, requested);
		var observer = new Mock<IGameItemComponent>();
		var called = 0; var expected = new InvalidOperationException("fixture postcommit callback");
		observer.Setup(x => x.Taken()).Callback(() =>
		{
			called++;
			Assert.AreSame(second.Body, offered.InInventoryOf);
			Assert.AreSame(first.Body, requested.InInventoryOf);
			CollectionAssert.AreEqual(new IGameItem[] { requested }, first.Body.HeldItems.ToArray());
			CollectionAssert.AreEqual(new IGameItem[] { offered }, second.Body.HeldItems.ToArray());
			throw expected;
		});
		Components(offered).Add(observer.Object);
		if (refuse)
		{
			second.HandMock.Setup(x => x.CanGrab(It.IsAny<IGameItem>(), second.Body)).Returns(WearlocGrabResult.FailFull);
			Assert.IsFalse(first.Body.TryExchangeItems(offered, second.Body, requested));
			Assert.AreSame(first.Body, offered.InInventoryOf); Assert.AreSame(second.Body, requested.InInventoryOf);
			CollectionAssert.AreEqual(new IGameItem[] { offered }, first.Body.HeldItems.ToArray());
			CollectionAssert.AreEqual(new IGameItem[] { requested }, second.Body.HeldItems.ToArray());
			Assert.AreEqual(0, called);
		}
		else
		{
			Assert.AreSame(expected, Assert.ThrowsExactly<InvalidOperationException>(() => first.Body.TryExchangeItems(offered, second.Body, requested)));
			Assert.AreEqual(1, called);
			Assert.AreSame(second.Body, offered.InInventoryOf); Assert.AreSame(first.Body, requested.InInventoryOf);
		}

		(Body Body, IGrab Hand, Mock<IGrab> HandMock) Participant(long id)
		{
			var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock }; var instance = actor.As<ICharacterInstance>();
			var identity = new Mock<ICharacterIdentity>(); identity.SetupGet(x => x.Id).Returns(id);
			identity.SetupGet(x => x.Instances).Returns([instance.Object]);
			actor.SetupGet(x => x.Id).Returns(id); actor.SetupGet(x => x.Identity).Returns(identity.Object);
			actor.SetupGet(x => x.Gameworld).Returns(world.Object); actor.SetupGet(x => x.Location).Returns(room.Object);
			actor.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(room.Object, RoomLayer.GroundLevel, null));
			actor.SetupGet(x => x.PermissionLevel).Returns(PermissionLevel.Founder);
			actor.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
			var hand = new Mock<IGrab>(); hand.SetupGet(x => x.Unary).Returns(false);
			var prototype = new Mock<IBodyPrototype> { DefaultValue = DefaultValue.Mock };
			prototype.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			prototype.Setup(x => x.DescribeBodypartGroup(It.IsAny<IEnumerable<IBodypart>>())).Returns("inventory");
			var race = new Mock<IRace> { DefaultValue = DefaultValue.Mock }; race.SetupGet(x => x.BaseBody).Returns(prototype.Object);
			var template = new Mock<ICharacterTemplate> { DefaultValue = DefaultValue.Mock }; template.SetupGet(x => x.SelectedRace).Returns(race.Object);
			var body = new Body(world.Object, actor.Object, template.Object, deferInitialisation: true);
			actor.SetupGet(x => x.Body).Returns(body);
			Set(body, "_bodyparts", new HashSet<IBodypart> { hand.Object }); Set(body, "_holdlocs", new List<IGrab> { hand.Object });
			hand.Setup(x => x.CanGrab(It.IsAny<IGameItem>(), body)).Returns(WearlocGrabResult.Success);
			actors.Add(actor.Object);
			return (body, hand.Object, hand);
		}
		GameItem Item(string name)
		{
			var prototype = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
			prototype.SetupGet(x => x.Gameworld).Returns(world.Object); prototype.SetupGet(x => x.Name).Returns(name);
			prototype.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			prototype.SetupGet(x => x.Components).Returns([]);
			var item = new GameItem(prototype.Object);
			Components(item).Add(new HoldableGameItemComponent((HoldableGameItemComponentProto)null!, item, temporary: true));
			return item;
		}
	}
	private static List<IGameItemComponent> Components(GameItem item) =>
		(List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(item)!;
	private static void Hold(Body body, IGrab hand, GameItem item)
	{
		item.GetItemType<IHoldable>().HeldBy = body;
		Set(body, "_heldItems", new List<Tuple<IGameItem, IGrab>> { Tuple.Create((IGameItem)item, hand) });
	}
	private static void Set(object target, string name, object value) =>
		target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, value);
}
