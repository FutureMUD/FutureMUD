#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp_Unit_Tests.NPC.AI;

[TestClass]
public class ShopkeeperRestockingTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Sale_RestockingRetriesThePathAndDeliversExistingStock(bool initiallyBlocked)
	{
		var world = Mock.Of<IFuturemud>(x => x.FutureProgs == new All<IFutureProg>());
		var shopfront = new Mock<IRoom>();
		shopfront.SetupGet(x => x.Location).Returns(shopfront.Object);
		var stockroom = new Mock<IRoom>();
		stockroom.SetupGet(x => x.Location).Returns(stockroom.Object);
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		var shop = new Mock<IPermanentShop>();
		var merchandise = new Mock<IMerchandise>();
		var stock = new Mock<IGameItem>();
		var cabinet = new Mock<IGameItem>();
		var effects = new List<IEffect>();
		var held = new List<IGameItem>();
		var location = shopfront.Object;
		var there = MakeExit(shopfront.Object, stockroom.Object);
		var back = MakeExit(stockroom.Object, shopfront.Object);
		var blocked = initiallyBlocked;
		shopfront.Setup(x => x.ExitsFor(null, true)).Returns(() => blocked ? [] : [there]);
		stockroom.Setup(x => x.ExitsFor(null, true)).Returns([back]);
		cabinet.SetupGet(x => x.Location).Returns(shopfront.Object);
		cabinet.SetupGet(x => x.LocationLevelPerceivable).Returns(cabinet.Object);
		actor.SetupGet(x => x.Gameworld).Returns(world);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.SetupGet(x => x.Location).Returns(() => location);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Effects).Returns(effects);
		actor.Setup(x => x.CanCross(It.IsAny<IRoomExit>())).Returns((true, null!));
		actor.Setup(x => x.CanMoveForPathPlanning(It.IsAny<IRoomExit>(), CanMoveFlags.None))
			.Returns(CanMoveResponse.True);
		actor.Setup(x => x.EffectsOfType<RestockingMerchandise>(It.IsAny<Predicate<RestockingMerchandise>>()))
			.Returns((Predicate<RestockingMerchandise>? filter) => effects.OfType<RestockingMerchandise>()
				.Where(effect => filter?.Invoke(effect) ?? true));
		actor.Setup(x => x.EffectsOfType<FollowingPath>(It.IsAny<Predicate<FollowingPath>>()))
			.Returns((Predicate<FollowingPath>? filter) => effects.OfType<FollowingPath>()
				.Where(effect => filter?.Invoke(effect) ?? true));
		actor.Setup(x => x.AffectedBy<FollowingPath>()).Returns(() => effects.OfType<FollowingPath>().Any());
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => effects.Add(effect));
		actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((effect, _) => effects.Remove(effect));
		body.SetupGet(x => x.HeldItems).Returns(held);
		shop.SetupGet(x => x.StockroomRoom).Returns(stockroom.Object);
		shop.SetupGet(x => x.ShopfrontRooms).Returns([shopfront.Object]);
		shop.SetupGet(x => x.AllShopRooms).Returns([shopfront.Object, stockroom.Object]);
		shop.SetupGet(x => x.EmployeesOnDuty).Returns([actor.Object]);
		shop.Setup(x => x.IsClockedIn(actor.Object)).Returns(true);
		shop.SetupGet(x => x.DisplayContainers).Returns([cabinet.Object]);
		merchandise.SetupGet(x => x.Shop).Returns(shop.Object);
		merchandise.SetupGet(x => x.PreferredDisplayContainer).Returns(cabinet.Object);
		stockroom.SetupGet(x => x.GameItems).Returns([stock.Object]);
		stock.SetupGet(x => x.DeepItems).Returns([stock.Object]);
		stock.SetupGet(x => x.Quantity).Returns(2);
		stock.Setup(x => x.AffectedBy<ItemOnDisplayInShop>(merchandise.Object)).Returns(true);
		body.Setup(x => x.Get(stock.Object, 2, null, false, ItemCanGetIgnore.None, null!))
			.Callback(() => held.Add(stock.Object)).Returns(stock.Object);
		body.Setup(x => x.CanPut(stock.Object, cabinet.Object, null, 2, false)).Returns(true);
		body.Setup(x => x.Put(stock.Object, cabinet.Object, null, 2, null, false, false, null!))
			.Callback(() => held.RemoveAll(x => ReferenceEquals(x, stock.Object))).Returns(stock.Object);
		var ai = new FixtureShopkeeper(world);

		Assert.IsTrue(ai.HandleEvent(EventType.ItemRequiresRestocking, actor.Object, shop.Object, merchandise.Object, 2));
		Assert.IsEmpty(effects.OfType<FollowingPath>().ToArray());
		effects.OfType<DelayedAction>().Single().ExpireEffect();
		if (initiallyBlocked)
		{
			Assert.IsEmpty(effects.OfType<FollowingPath>().ToArray());
			blocked = false;
			ai.HandleEvent(EventType.MinuteTick, actor.Object);
		}

		Assert.AreSame(there, effects.OfType<FollowingPath>().Single().Exits.Single());
		location = stockroom.Object;
		effects.RemoveAll(x => x is FollowingPath);
		ai.HandleEvent(EventType.CharacterEnterRoomFinish, actor.Object, stockroom.Object);
		body.Verify(x => x.Get(stock.Object, 2, null, false, ItemCanGetIgnore.None, null!), Times.Once);
		Assert.HasCount(1, held);
		var request = effects.OfType<RestockingMerchandise>().Single();
		Assert.AreSame(stock.Object, request.CarriedItem);
		var route = ai.ActualPath(actor.Object);
		Assert.AreSame(shopfront.Object, route.Target, "The AI should choose a shopfront while carrying stock.");
		CollectionAssert.AreEqual(new[] { back }, route.Exits.ToArray(), "The computed return route should use the shop exit.");
		var activePaths = effects.OfType<FollowingPath>().ToArray();
		Assert.HasCount(1, activePaths, "Stock pickup should start the return path.");
		CollectionAssert.AreEqual(new[] { back }, activePaths[0].Exits.ToArray());

		location = shopfront.Object;
		effects.RemoveAll(x => x is FollowingPath);
		ai.HandleEvent(EventType.CharacterEnterRoomFinish, actor.Object, shopfront.Object);

		Assert.IsEmpty(held);
		Assert.IsEmpty(effects.OfType<RestockingMerchandise>().ToArray());
		body.Verify(x => x.Put(stock.Object, cabinet.Object, null, 2, null, false, false, null!), Times.Once);
	}

	private static IRoomExit MakeExit(IRoom from, IRoom to)
	{
		return Mock.Of<IRoomExit>(x => x.Origin == from && x.Destination == to);
	}

	private sealed class FixtureShopkeeper(IFuturemud world) : ShopkeeperAI(new MudSharp.Models.ArtificialIntelligence
	{
		Name = "Bakery shopkeeper",
		Definition = """
			<Definition>
			  <OpenDoors>false</OpenDoors><UseKeys>false</UseKeys><SmashLockedDoors>false</SmashLockedDoors>
			  <MoveEvenIfObstructionInWay>false</MoveEvenIfObstructionInWay><UseDoorguards>false</UseDoorguards>
			  <RestockStartDelay>1000</RestockStartDelay>
			</Definition>
			"""
	}, world)
	{
		public (IRoom? Target, IEnumerable<IRoomExit> Exits) ActualPath(ICharacter character) => GetPath(character);

		protected override FollowingPath CreatePathingEffect(ICharacter ch, IEnumerable<IRoomExit> path)
		{
			return new RecordedPath(ch, path);
		}

		private sealed class RecordedPath(ICharacter ch, IEnumerable<IRoomExit> path) : FollowingPath(ch, path)
		{
			public override void FollowPathAction() { }
		}
	}
}
