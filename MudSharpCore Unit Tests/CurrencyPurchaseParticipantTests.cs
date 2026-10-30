#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Events.Hooks;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.GameItems.Prototypes;
using MudSharp.Planes;
using PreparedStoredItemDelivery = MudSharp.FutureProg.Functions.GameItem.PreparedStoredItemDelivery;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class CurrencyPurchaseParticipantTests
{
	[TestMethod]
	public void TryGiveCurrency_ParticipantRefuses_DoesNotChangeCashAndDisposes()
	{
		using var fixture = new PurchaseFixture();
		var participant = new TestParticipant { PrepareResult = false };

		Assert.IsFalse(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 5m, true, participant, true));

		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body));
		Assert.AreEqual(0m, fixture.Cash(fixture.Seller.Body));
		Assert.AreEqual(0, participant.CommitCalls);
		Assert.AreEqual(0, participant.NotifyCalls);
		Assert.AreEqual(1, participant.DisposeCalls);
	}

	[TestMethod]
	public void TryGiveCurrency_ParticipantInvalidatedAfterPrepare_DoesNotChangeCash()
	{
		using var fixture = new PurchaseFixture();
		var participant = new TestParticipant { CurrentResult = false };

		Assert.IsFalse(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 5m, true, participant, true));

		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body));
		Assert.AreEqual(0m, fixture.Cash(fixture.Seller.Body));
		Assert.AreEqual(1, participant.PrepareCalls);
		Assert.AreEqual(0, participant.CommitCalls);
		Assert.AreEqual(0, participant.NotifyCalls);
		Assert.AreEqual(1, participant.DisposeCalls);
	}

	[TestMethod]
	public void TryGiveCurrency_ParticipantCommitsBeforeFirstNotification_SeesCoherentCashAndGoods()
	{
		using var fixture = new PurchaseFixture();
		var participantGoodsDelivered = false;
		var participant = new TestParticipant
		{
			OnCommit = () =>
			{
				participantGoodsDelivered = true;
				Assert.AreEqual(5m, fixture.Cash(fixture.Buyer.Body));
				Assert.AreEqual(5m, fixture.Cash(fixture.Seller.Body));
			},
			OnNotify = () =>
			{
				Assert.IsTrue(participantGoodsDelivered);
				Assert.AreEqual(5m, fixture.Cash(fixture.Buyer.Body));
				Assert.AreEqual(5m, fixture.Cash(fixture.Seller.Body));
			}
		};

		Assert.IsTrue(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 5m, true, participant, true));

		Assert.IsTrue(participantGoodsDelivered);
		Assert.AreEqual(1, participant.CommitCalls);
		Assert.AreEqual(1, participant.NotifyCalls);
		Assert.AreEqual(1, participant.DisposeCalls);
		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body) + fixture.Cash(fixture.Seller.Body));
	}

	[TestMethod]
	[DataRow(RoomLayer.GroundLevel)]
	[DataRow(RoomLayer.InTrees)]
	public void TryGiveCurrency_PurchaseStoredWearable_ChargesAndPublishesExactRoomStockOnce(RoomLayer buyerLayer)
	{
		using var fixture = new PurchaseFixture();
		var (source, destination, item, wearable) = fixture.CreateStockItem(buyerLayer);
		var requestedFit = WearableFit.ForScalar(3.5);
		var participant = new PreparedStoredItemDelivery(fixture.Buyer.Actor, fixture.Seller.Actor, item, wearable,
			source, destination, fixture.Buyer.Body, fixture.Seller.Body, item.GetItemType<HoldableGameItemComponent>()!,
			4m, requestedFit);

		Assert.IsTrue(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 4m, true, participant, true));

		Assert.AreEqual(6m, fixture.Cash(fixture.Buyer.Body));
		Assert.AreEqual(4m, fixture.Cash(fixture.Seller.Body));
		Assert.AreEqual(requestedFit, wearable.Fit);
		Assert.AreSame(destination, item.DirectLocation);
		Assert.AreEqual(buyerLayer, item.RoomLayer);
		Assert.IsFalse(source.GameItems.Any(x => ReferenceEquals(x, item)));
		Assert.AreEqual(1, destination.GameItems.Count(x => ReferenceEquals(x, item)));

		var sameStockAgain = new PreparedStoredItemDelivery(fixture.Buyer.Actor, fixture.Seller.Actor, item, wearable,
			source, destination, fixture.Buyer.Body, fixture.Seller.Body, item.GetItemType<HoldableGameItemComponent>()!,
			4m, WearableFit.ForScalar(4.5));
		Assert.IsFalse(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 4m, true, sameStockAgain, true));
		Assert.AreEqual(6m, fixture.Cash(fixture.Buyer.Body));
		Assert.AreEqual(4m, fixture.Cash(fixture.Seller.Body));
		Assert.AreEqual(requestedFit, wearable.Fit);
		Assert.AreSame(destination, item.DirectLocation);
	}

	[TestMethod]
	public void TryGiveCurrency_PurchaseStoredWearableFitInvalidatedAfterPrepare_PreservesCashAndStock()
	{
		using var fixture = new PurchaseFixture();
		var (source, destination, item, wearable) = fixture.CreateStockItem();
		var originalFit = WearableFit.ForScalar(2.5);
		wearable.SetFit(originalFit);
		var participant = new PreparedStoredItemDelivery(fixture.Buyer.Actor, fixture.Seller.Actor, item, wearable,
			source, destination, fixture.Buyer.Body, fixture.Seller.Body, item.GetItemType<HoldableGameItemComponent>()!,
			4m, WearableFit.ForScalar(3.5));
		Assert.IsTrue(participant.Prepare());
		wearable.SetFit(WearableFit.ForScalar(2.75));

		Assert.IsFalse(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 4m, true, participant, true));

		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body));
		Assert.AreEqual(0m, fixture.Cash(fixture.Seller.Body));
		Assert.AreEqual(WearableFit.ForScalar(2.75), wearable.Fit);
		Assert.AreSame(source, item.DirectLocation);
		Assert.AreEqual(1, source.GameItems.Count(x => ReferenceEquals(x, item)));
		Assert.IsFalse(destination.GameItems.Any(x => ReferenceEquals(x, item)));
	}

	[TestMethod]
	public void TryGiveCurrency_ParticipantNotifyThrows_CashDraftStillFinalizesAndValueIsConserved()
	{
		using var fixture = new PurchaseFixture();
		var participant = new TestParticipant { NotifyException = new InvalidOperationException("post-commit notification") };

		var thrown = Assert.ThrowsExactly<InvalidOperationException>(() =>
			fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 5m, true, participant, true));

		Assert.AreEqual("The prepared currency participant committed, but its post-commit notification failed.", thrown.Message);
		Assert.IsTrue(participant.GoodsDelivered);
		Assert.AreEqual(1, participant.CommitCalls);
		Assert.AreEqual(1, participant.NotifyCalls);
		fixture.World.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Once);
		Assert.AreEqual(5m, fixture.Cash(fixture.Buyer.Body));
		Assert.AreEqual(5m, fixture.Cash(fixture.Seller.Body));
		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body) + fixture.Cash(fixture.Seller.Body));
	}

	[TestMethod]
	public void TryGiveCurrency_HeldParcelCustody_PreservesActualItemAndChargesOnce()
	{
		using var fixture = new PurchaseFixture();
		var (storage, _, parcel, _) = fixture.CreateStockItem();
		fixture.MoveStockToBuyer(storage, parcel);
		using var participant = fixture.Buyer.Body.PrepareStoredItemCustody(fixture.Seller.Actor, parcel, storage);
		Assert.IsNotNull(participant);
		Assert.IsTrue(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 4m, true, participant, true));
		Assert.AreEqual(6m, fixture.Cash(fixture.Buyer.Body));
		Assert.AreEqual(4m, fixture.Cash(fixture.Seller.Body));
		Assert.AreSame(storage, parcel.DirectLocation);
		Assert.IsNull(parcel.InInventoryOf);
		Assert.IsFalse(fixture.Buyer.Body.HeldItems.Any(x => ReferenceEquals(x, parcel)));
		Assert.AreEqual(1, storage.GameItems.Count(x => ReferenceEquals(x, parcel)));
		Assert.IsNull(fixture.Buyer.Body.PrepareStoredItemCustody(fixture.Seller.Actor, parcel, storage));
	}

	[TestMethod]
	public void TryGiveCurrency_HeldParcelCannotAfford_PreservesCashAndCustody()
	{
		using var fixture = new PurchaseFixture();
		var (storage, _, parcel, _) = fixture.CreateStockItem();
		fixture.MoveStockToBuyer(storage, parcel);
		using var participant = fixture.Buyer.Body.PrepareStoredItemCustody(fixture.Seller.Actor, parcel, storage);
		Assert.IsNotNull(participant);
		Assert.IsFalse(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 11m, true, participant, true));
		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body));
		Assert.AreEqual(0m, fixture.Cash(fixture.Seller.Body));
		Assert.AreSame(fixture.Buyer.Body, parcel.InInventoryOf);
		Assert.IsFalse(storage.GameItems.Any());
	}

	[TestMethod]
	public void PrepareHeldParcel_LoosePaymentCurrencyRefused()
	{
		using var fixture = new PurchaseFixture();
		var (storage, _, _, _) = fixture.CreateStockItem();
		var money = (GameItem)fixture.Buyer.Body.HeldItems.Single();
		Assert.IsNull(fixture.Buyer.Body.PrepareStoredItemCustody(fixture.Seller.Actor, money, storage));
		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body));
	}

	[TestMethod]
	public void TryGiveCurrency_GenericStoredTicket_DeliversWithoutWearableComponent()
	{
		using var fixture = new PurchaseFixture();
		var (storage, destination, ticket, _) = fixture.CreateStockItem(RoomLayer.InTrees);
		var holdable = ticket.GetItemType<HoldableGameItemComponent>()!;
		Set(ticket, "_components", new List<IGameItemComponent> { holdable });
		using var participant = new PreparedStoredItemDelivery(fixture.Buyer.Actor, fixture.Seller.Actor, ticket, null,
			storage, destination, fixture.Buyer.Body, fixture.Seller.Body, holdable, 4m, null);
		Assert.IsTrue(fixture.Buyer.Body.TryGiveCurrency(fixture.Currency, fixture.Seller.Body, 4m, true, participant, true));
		Assert.AreSame(destination, ticket.DirectLocation);
		Assert.AreEqual(RoomLayer.InTrees, ticket.RoomLayer);
		Assert.AreEqual(6m, fixture.Cash(fixture.Buyer.Body));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void StoredParcel_RequiredInventoryReceipt_LeavesRefusedParcelsPrivate(bool noHands)
	{
		using var fixture = new PurchaseFixture();
		var (storage, destination, parcel, wearable) = fixture.CreateStockItem();
		if (noHands) Set(fixture.Buyer.Body, "_holdlocs", new List<IGrab>());
		if (!noHands)
		{
			Assert.AreEqual(CanUseBodypartResult.CanUse, fixture.Buyer.Body.CanUseBodypart(fixture.Buyer.Hand));
			Assert.IsTrue(storage.CanGet(parcel, fixture.Buyer.Actor), "Storage access");
			Assert.AreEqual(ItemGetResponse.CanGet, parcel.CanGet(0), "Parcel eligibility");
			Assert.IsNotNull(fixture.Buyer.Body.PrepareStoredItemReceipt(parcel), "Native receipt");
		}
		using var participant = new PreparedStoredItemDelivery(fixture.Buyer.Actor, fixture.Seller.Actor, parcel, wearable,
			storage, destination, fixture.Buyer.Body, fixture.Seller.Body, parcel.GetItemType<HoldableGameItemComponent>()!,
			0m, null, requireInventory: true);
		Assert.AreEqual(!noHands, participant.Prepare());
		if (!noHands)
		{
			Assert.IsTrue(participant.IsCurrent);
			participant.Commit();
			participant.Notify();
			Assert.AreSame(fixture.Buyer.Body, parcel.InInventoryOf);
			Assert.IsFalse(storage.GameItems.Any());
		}
		else
		{
			Assert.AreSame(storage, parcel.DirectLocation);
			Assert.AreEqual(1, storage.GameItems.Count(x => ReferenceEquals(x, parcel)));
		}
		Assert.IsFalse(destination.GameItems.Any(x => ReferenceEquals(x, parcel)));
		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body));
	}

	[TestMethod]
	public void StoredPrepaidGarment_SizingDisabled_StillDeliversSavedGarment()
	{
		using var fixture = new PurchaseFixture();
		var (storage, destination, parcel, wearable) = fixture.CreateStockItem();
		fixture.World.Setup(x => x.GetStaticBool("EnableWearableSizing")).Returns(false);
		var fit = wearable.Fit;
		using var participant = new PreparedStoredItemDelivery(fixture.Buyer.Actor, fixture.Seller.Actor, parcel, wearable,
			storage, destination, fixture.Buyer.Body, fixture.Seller.Body, parcel.GetItemType<HoldableGameItemComponent>()!, 0m, null);
		Assert.IsTrue(participant.Prepare());
		participant.Commit();
		participant.Notify();
		Assert.AreSame(destination, parcel.DirectLocation);
		Assert.AreEqual(fit, wearable.Fit);
		Assert.AreEqual(10m, fixture.Cash(fixture.Buyer.Body));
	}

	private sealed class TestParticipant : ICurrencyTransferParticipant
	{
		internal bool PrepareResult { get; init; } = true;
		internal bool CurrentResult { get; init; } = true;
		internal bool GoodsDelivered { get; private set; }
		internal int PrepareCalls { get; private set; }
		internal int CommitCalls { get; private set; }
		internal int NotifyCalls { get; private set; }
		internal int DisposeCalls { get; private set; }
		internal Exception? NotifyException { get; init; }
		internal Action? OnCommit { get; set; }
		internal Action? OnNotify { get; init; }
		public bool Prepare() { PrepareCalls++; return PrepareResult; }
		public bool IsCurrent => CurrentResult;
		public void Commit() { CommitCalls++; GoodsDelivered = true; OnCommit?.Invoke(); }
		public void Notify() { NotifyCalls++; OnNotify?.Invoke(); if (NotifyException is not null) throw NotifyException; }
		public void Dispose() => DisposeCalls++;
	}

	private sealed class PurchaseFixture : IDisposable
	{
		private readonly IGameItemProto _previousPrototype = CurrencyGameItemComponentProto.ItemPrototype;
		private readonly Mock<ISaveManager> _save = new();
		private readonly Mock<IRoom> _room = new();
		private readonly All<ICharacter> _actors = new();
		internal Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		internal ICurrency Currency { get; } = Mock.Of<ICurrency>();
		private ICoin Coin { get; } = Mock.Of<ICoin>(x => x.Value == 1m);
		private GameItemProto Prototype { get; }
		internal (ICharacter Actor, Body Body, IGrab Hand) Buyer { get; }
		internal (ICharacter Actor, Body Body, IGrab Hand) Seller { get; }

		internal PurchaseFixture()
		{
			Mock.Get(Currency).Setup(x => x.FindCurrency(It.IsAny<IEnumerable<ICurrencyPile>>(), It.IsAny<decimal>()))
				.Returns((IEnumerable<ICurrencyPile> piles, decimal amount) =>
				{
					var pile = piles.Single();
					return pile.TotalValue >= amount
						? new Dictionary<ICurrencyPile, Dictionary<ICoin, int>> { [pile] = new() { [Coin] = (int)amount } }
						: new Dictionary<ICurrencyPile, Dictionary<ICoin, int>>();
				});
			World.SetupGet(x => x.SaveManager).Returns(_save.Object);
			World.SetupGet(x => x.UnitManager.BaseWeightToKilograms).Returns(1.0);
			World.SetupGet(x => x.UnitManager.BaseFluidToLitres).Returns(1.0);
			World.SetupGet(x => x.Actors).Returns(_actors);
			World.SetupGet(x => x.Characters).Returns(new All<ICharacter>());
			World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>());
			World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
			World.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
			World.SetupGet(x => x.ProximityEventService).Returns(() => null!);
			World.Setup(x => x.GetStaticBool("EnableWearableSizing")).Returns(true);
			_room.SetupGet(x => x.Characters).Returns(() => _actors.ToArray());
			_room.Setup(x => x.CanGet(It.IsAny<IGameItem>(), It.IsAny<ICharacter>())).Returns(true);

			Prototype = TestObjectFactory.CreateUninitialized<GameItemProto>();
			typeof(GameItemProto).GetProperty("Gameworld")!.SetValue(Prototype, World.Object);
			Set(Prototype, "_name", "test currency");
			Set(Prototype, "_components", new List<IGameItemComponentProto>
			{
				TestObjectFactory.CreateUninitialized<CurrencyGameItemComponentProto>(),
				TestObjectFactory.CreateUninitialized<HoldableGameItemComponentProto>()
			});
			Prototype.OnLoadProgs = [];
			CurrencyGameItemComponentProto.ItemPrototype = Prototype;

			Buyer = MakeActor(1);
			Seller = MakeActor(2);
			var cash = CurrencyGameItemComponentProto.CreateNewCurrencyPile(Currency, new[] { (Coin, 10) }, true);
			Hold(Buyer.Body, Buyer.Hand, (GameItem)cash);
		}

		private (ICharacter Actor, Body Body, IGrab Hand) MakeActor(long id)
		{
			var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
			var instance = actor.As<ICharacterInstance>();
			var identity = new Mock<ICharacterIdentity>();
			identity.SetupGet(x => x.Id).Returns(id);
			identity.SetupGet(x => x.Instances).Returns([instance.Object]);
			actor.SetupGet(x => x.Id).Returns(id);
			actor.SetupGet(x => x.Identity).Returns(identity.Object);
			actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			actor.SetupGet(x => x.Location).Returns(_room.Object);
			actor.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(_room.Object, RoomLayer.GroundLevel, null));
			actor.SetupGet(x => x.PermissionLevel).Returns(PermissionLevel.Founder);
			actor.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			actor.SetupGet(x => x.Combat).Returns((MudSharp.Combat.ICombat)null!);
			actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
			var hand = new Mock<IGrab>(); hand.SetupGet(x => x.Unary).Returns(false);
			var prototype = new Mock<IBodyPrototype> { DefaultValue = DefaultValue.Mock };
			prototype.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			prototype.Setup(x => x.DescribeBodypartGroup(It.IsAny<IEnumerable<IBodypart>>())).Returns("inventory");
			var race = new Mock<IRace> { DefaultValue = DefaultValue.Mock }; race.SetupGet(x => x.BaseBody).Returns(prototype.Object);
			var template = new Mock<ICharacterTemplate> { DefaultValue = DefaultValue.Mock }; template.SetupGet(x => x.SelectedRace).Returns(race.Object);
			var body = new Body(World.Object, actor.Object, template.Object, deferInitialisation: true);
			actor.SetupGet(x => x.Body).Returns(body);
			Set(body, "_bodyparts", new HashSet<IBodypart> { hand.Object });
			Set(body, "_holdlocs", new List<IGrab> { hand.Object });
			hand.Setup(x => x.CanGrab(It.IsAny<IGameItem>(), body)).Returns(WearlocGrabResult.Success);
			_actors.Add(actor.Object);
			return (actor.Object, body, hand.Object);
		}

		internal decimal Cash(Body body) => body.HeldItems
			.Select(x => x.GetItemType<ICurrencyPile>())
			.Where(x => x is not null && ReferenceEquals(x.Currency, Currency))
			.Sum(x => x!.TotalValue);

		internal void MoveStockToBuyer(Room storage, GameItem item)
		{
			storage.SetPreparedCurrencyRoomMembership(item, false);
			item.SetPreparedHeldCustody(Buyer.Body);
			var held = Buyer.Body.HeldItems.Select(x => Tuple.Create(x, Buyer.Hand)).ToList();
			held.Add(Tuple.Create((IGameItem)item, Buyer.Hand));
			Set(Buyer.Body, "_heldItems", held);
		}

		internal (Room Source, Room Destination, GameItem Item, WearableGameItemComponent Wearable) CreateStockItem(RoomLayer buyerLayer = RoomLayer.GroundLevel)
		{
			var source = NativeRoom(101);
			var destination = NativeRoom(102);
			Set(destination, "_characters", new List<ICharacter> { Buyer.Actor, Seller.Actor });
			Mock.Get(Buyer.Actor).SetupGet(x => x.Location).Returns(destination);
			Mock.Get(Buyer.Actor).SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(destination, buyerLayer, null));
			Mock.Get(Buyer.Actor).SetupGet(x => x.RoomLayer).Returns(buyerLayer);
			Mock.Get(Seller.Actor).SetupGet(x => x.Location).Returns(destination);
			Mock.Get(Seller.Actor).SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(destination, buyerLayer, null));
			Mock.Get(Seller.Actor).SetupGet(x => x.RoomLayer).Returns(buyerLayer);

			var item = new GameItem(Prototype, null, ItemQuality.Standard);
			var holdable = item.GetItemType<HoldableGameItemComponent>()!;
			var wearablePrototype = TestObjectFactory.CreateUninitialized<WearableGameItemComponentProto>();
			Set(wearablePrototype, "SizingEnabled", true);
			var wearable = new TestWearableComponent(wearablePrototype, item);
			wearable.SetFit(WearableFit.ForScalar(2.5));
			Set(item, "_components", new List<IGameItemComponent> { wearable, holdable });
			item.SetPreparedVehicleLocation(new SpatialLocation(source, RoomLayer.GroundLevel, null));
			source.SetPreparedCurrencyRoomMembership(item, true);
			return (source, destination, item, wearable);
		}

		private Room NativeRoom(long id)
		{
			var terrain = new Mock<ITerrain>();
			terrain.SetupGet(x => x.TerrainLayers).Returns([RoomLayer.GroundLevel, RoomLayer.InTrees]);
			var overlay = new Mock<IRoomOverlay>();
			overlay.SetupGet(x => x.Terrain).Returns(terrain.Object);
			var template = new Mock<IRoom> { DefaultValue = DefaultValue.Mock };
			template.SetupGet(x => x.Gameworld).Returns(World.Object);
			template.SetupGet(x => x.Id).Returns(id);
			template.SetupGet(x => x.CurrentOverlay).Returns(overlay.Object);
			template.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition)null!);
			return new Room(template.Object, id);
		}

		private static void Hold(Body body, IGrab hand, GameItem item)
		{
			item.GetItemType<IHoldable>().HeldBy = body;
			Set(body, "_heldItems", new List<Tuple<IGameItem, IGrab>> { Tuple.Create((IGameItem)item, hand) });
		}

		public void Dispose() => CurrencyGameItemComponentProto.ItemPrototype = _previousPrototype;

		private sealed class TestWearableComponent(WearableGameItemComponentProto prototype, IGameItem parent)
			: WearableGameItemComponent(prototype, parent, null!, temporary: true)
		{
		}
	}

	private static void Set(object target, string name, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) is { } field)
			{
				field.SetValue(target, value);
				return;
			}
			else if (type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) is { } property)
			{
				property.SetValue(target, value);
				return;
			}
		Assert.Fail($"Missing native fixture field {name}.");
	}
}
