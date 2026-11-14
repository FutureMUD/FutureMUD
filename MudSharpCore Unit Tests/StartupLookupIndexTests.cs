#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Framework.Save;
using MudSharp.Framework.Revision;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StartupLookupIndexTests
{
	private static (Mock<ICharacter> Actor, EffectHandler Handler) Actor(Futuremud world, long id, long instance = 0)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(id);
		actor.SetupGet(x => x.InstanceId).Returns(instance == 0 ? id : instance);
		actor.SetupGet(x => x.Identity).Returns(Mock.Of<ICharacterIdentity>(x => x.Id == id));
		actor.SetupGet(x => x.Gameworld).Returns(world);
		actor.SetupGet(x => x.Name).Returns("actor");
		actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns((ICharacter other) => ReferenceEquals(other, actor.Object));
		var handler = new EffectHandler(actor.Object);
		actor.Setup(x => x.EffectsOfType<IPossessedBodyEffect>(It.IsAny<Predicate<IPossessedBodyEffect>>()))
			.Returns((Predicate<IPossessedBodyEffect>? p) => handler.EffectsOfType(p!));
		actor.Setup(x => x.EffectsOfType<ILiveBodyPossessionEffect>(It.IsAny<Predicate<ILiveBodyPossessionEffect>>()))
			.Returns((Predicate<ILiveBodyPossessionEffect>? p) => handler.EffectsOfType(p!));
		actor.Setup(x => x.EffectsOfType<ICorpsePossessionEffect>(It.IsAny<Predicate<ICorpsePossessionEffect>>()))
			.Returns((Predicate<ICorpsePossessionEffect>? p) => handler.EffectsOfType(p!));
		actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>(handler.RemoveEffect);
		world.Add(actor.Object, true);
		return (actor, handler);
	}

	private static Mock<ILiveBodyPossessionEffect> Possession(ICharacter owner, long identity, long instance)
	{
		var effect = new Mock<ILiveBodyPossessionEffect>();
		effect.SetupGet(x => x.Owner).Returns(owner);
		effect.SetupGet(x => x.AnchorCharacterId).Returns(identity);
		effect.SetupGet(x => x.AnchorInstanceId).Returns(instance);
		return effect;
	}

	[TestMethod]
	public void PossessionIndex_InitialisationSelfRemovalAndRemovalNotificationObserveNativeOrdering()
	{
		var world = new Futuremud(null);
		var (anchor, _) = Actor(world, 1, 100);
		var (owner, handler) = Actor(world, 2);
		var effect = Possession(owner.Object, 1, 100);
		effect.Setup(x => x.InitialEffect()).Callback(() => Assert.IsTrue(PossessionControlService.AnyPossessionEffectsForAnchor(anchor.Object)));
		effect.Setup(x => x.RemovalEffect()).Callback(() => Assert.IsTrue(PossessionControlService.AnyPossessionEffectsForAnchor(anchor.Object)));
		effect.As<IEffectRemovalNotification>().Setup(x => x.OnRemovedFromOwner())
			.Callback(() => Assert.IsFalse(PossessionControlService.AnyPossessionEffectsForAnchor(anchor.Object)));
		handler.AddEffect(effect.Object);
		Assert.IsTrue(PossessionControlService.RemovePossessionEffectsForAnchor(anchor.Object));
		Assert.AreEqual(0, handler.Effects.Count());
		effect.Verify(x => x.RemovalEffect(), Times.Once);
		effect.As<IEffectRemovalNotification>().Verify(x => x.OnRemovedFromOwner(), Times.Once);

		var selfRemoving = Possession(owner.Object, 1, 100);
		selfRemoving.Setup(x => x.InitialEffect()).Callback(() => handler.RemoveEffect(selfRemoving.Object));
		handler.AddEffect(selfRemoving.Object);
		Assert.IsFalse(PossessionControlService.AnyPossessionEffectsForAnchor(anchor.Object));
	}

	[TestMethod]
	public void PossessionIndex_WildcardsMatchIdentityWhileExactInstanceAndOtherOwnersRemainIsolated()
	{
		var world = new Futuremud(null);
		var (anchor, _) = Actor(world, 1, 100);
		var (owner, handler) = Actor(world, 2);
		var wildcard = Possession(owner.Object, 1, 0);
		var otherInstance = Possession(owner.Object, 1, 101);
		handler.AddEffect(wildcard.Object);
		handler.AddEffect(otherInstance.Object);
		Assert.IsTrue(PossessionControlService.RemovePossessionEffectsForAnchor(anchor.Object));
		CollectionAssert.AreEqual(new IEffect[] { otherInstance.Object }, handler.Effects.ToArray());
		Assert.IsFalse(PossessionControlService.AnyPossessionEffectsForAnchor(anchor.Object));
		handler.RemoveAllEffects();
		Assert.AreEqual(0, PossessionEffectIndex.For(world).All<ILiveBodyPossessionEffect>(world).Length);
	}

	[TestMethod]
	public void PossessionIndex_SimulationReleaseDropsOwnersWithAndWithoutEffects()
	{
		var world = new Futuremud(null);
		var (owner, handler) = Actor(world, 2);
		var index = PossessionEffectIndex.For(world);
		handler.AddEffect(Possession(owner.Object, 1, 0).Object);
		world.Destroy(owner.Object);
		Assert.AreEqual(1, index.RegisteredOwnerCount, "Ordinary logout keeps cached actors available.");
		world.ForgetCombatSimulationActor(owner.Object);
		Assert.IsFalse(world.Actors.Any());
		Assert.IsFalse(world.CachedActors.Any());
		Assert.AreEqual(0, index.RegisteredOwnerCount);
		Assert.AreEqual(0, index.All<ILiveBodyPossessionEffect>(world).Length);
		var (empty, _) = Actor(world, 3);
		world.Destroy(empty.Object); world.ForgetCombatSimulationActor(empty.Object);
		Assert.AreEqual(0, index.RegisteredOwnerCount);
	}

	[TestMethod]
	public void MediaRegistry_FollowsCurrentNestedLocationAndRefreshesComponentsWithoutScanningWorldItems()
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		world.SetupGet(x => x.Items).Returns(new All<IGameItem>());
		var service = new MediaChannelService(world.Object);
		var room = Mock.Of<IRoom>();
		var elsewhere = Mock.Of<IRoom>();
		IRoom current = room;
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.TrueLocations).Returns(() => new[] { current });
		var first = new Mock<IMediaSource>(); first.SetupGet(x => x.Parent).Returns(parent.Object);
		var second = new Mock<IMediaSource>(); second.SetupGet(x => x.Parent).Returns(parent.Object);
		IMediaSource[] components = [first.Object];
		parent.Setup(x => x.GetItemTypes<IMediaSource>()).Returns(() => components);
		service.RegisterItem(parent.Object);
		CollectionAssert.AreEqual(components, service.GetSources(room).ToArray());
		current = elsewhere;
		Assert.AreEqual(0, service.GetSources(room).Count());
		CollectionAssert.AreEqual(components, service.GetSources(elsewhere).ToArray());
		components = [second.Object];
		service.RefreshItem(parent.Object);
		CollectionAssert.AreEqual(components, service.GetSources().ToArray());
		service.UnregisterItem(parent.Object);
		Assert.AreEqual(0, service.GetSources().Count());
		world.VerifyGet(x => x.Items, Times.Once, "Only the initial compatibility bootstrap may enumerate world items.");
	}

	[TestMethod]
	public void MediaRegistry_NativePrototypeUpdatesAddAndRemoveSourceComponents()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Items).Returns(new All<IGameItem>());
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var service = new MediaChannelService(world.Object);
		world.SetupGet(x => x.MediaChannelService).Returns(service);
		var components = new List<IGameItemComponentProto>();
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Gameworld).Returns(world.Object); proto.SetupGet(x => x.Name).Returns("media fixture");
		proto.SetupGet(x => x.Status).Returns(RevisionStatus.Current); proto.SetupGet(x => x.Components).Returns(components);
		var item = new GameItem(proto.Object);
		service.RegisterItem(item);
		Assert.AreEqual(0, service.GetSources().Count());
		var sourceProto = new Mock<IGameItemComponentProto>(); sourceProto.SetupGet(x => x.Id).Returns(1L);
		var source = new Mock<IMediaSource>(); source.SetupGet(x => x.Parent).Returns(item);
		source.SetupGet(x => x.Prototype).Returns(sourceProto.Object);
		sourceProto.Setup(x => x.CreateNew(item, null, false)).Returns(source.Object);
		components.Add(sourceProto.Object);
		Assert.IsTrue(item.CheckPrototypeForUpdate());
		CollectionAssert.AreEqual(new[] { source.Object }, service.GetSources().ToArray());
		source.Setup(x => x.Delete()).Callback(() => Assert.AreEqual(0, service.GetSources().Count()));
		components.Clear();
		Assert.IsTrue(item.CheckPrototypeForUpdate());
		Assert.AreEqual(0, service.GetSources().Count());
		source.Verify(x => x.Delete(), Times.Once);
	}

	[TestMethod]
	public void RoomInstanceIndex_TracksMarkerRemovalAndRetainsWorldOrder()
	{
		var world = new Mock<IFuturemud>();
		var rooms = new All<IRoom>(); world.SetupGet(x => x.Rooms).Returns(rooms);
		Mock<IRoom> Room(long id)
		{
			var room = new Mock<IRoom>(); room.SetupGet(x => x.Id).Returns(id); room.SetupGet(x => x.Gameworld).Returns(world.Object);
			return room;
		}
		var first = Room(2); var second = Room(1);
		var a = new RoomTemplateInstance(first.Object, 10, false); var b = new RoomTemplateInstance(second.Object, 10, false);
		var firstMarkers = new List<RoomTemplateInstance> { a };
		first.Setup(x => x.EffectsOfType<RoomTemplateInstance>(It.IsAny<Predicate<RoomTemplateInstance>>())).Returns(firstMarkers);
		second.Setup(x => x.EffectsOfType<RoomTemplateInstance>(It.IsAny<Predicate<RoomTemplateInstance>>())).Returns([b]);
		rooms.Add(first.Object); rooms.Add(second.Object);
		var index = RoomTemplateInstanceIndex.For(world.Object);
		CollectionAssert.AreEqual(new[] { first.Object, second.Object }, index.Get(10).ToArray());
		firstMarkers.Clear(); index.Refresh(first.Object);
		CollectionAssert.AreEqual(new[] { second.Object }, index.Get(10).ToArray());
		firstMarkers.Add(a); index.Refresh(first.Object);
		CollectionAssert.AreEqual(new[] { first.Object, second.Object }, index.Get(10).ToArray());
		index.Unregister(second.Object);
		CollectionAssert.AreEqual(new[] { first.Object }, index.Get(10).ToArray());
	}
}
