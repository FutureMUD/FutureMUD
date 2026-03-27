using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Shops;
using MudSharp.Economy.Payment;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Character;
using MudSharp.Body;
using MudSharp.TimeAndDate;
using MudSharp.Construction;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine;
using MudSharp.Events;
using MudSharp.TimeAndDate.Date;
using MudSharp.RPG.Law;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ShopTests
{
	private Mock<IFuturemud> _gameworld = null!;
	private Mock<IUneditableAll<IEconomicZone>> _zones = null!;
	private Mock<IUneditableAll<ICurrency>> _currencies = null!;
	private Mock<IUneditableAll<IMarket>> _markets = null!;
	private Mock<IUneditableAll<IFutureProg>> _progs = null!;
	private Mock<IUneditableRevisableAll<IGameItemProto>> _itemProtos = null!;
	private All<ITag> _tags = null!;
	private Mock<ISaveManager> _save = null!;
	private Mock<IEconomicZone> _zone = null!;
	private Mock<ICurrency> _currency = null!;
	private TestShop _shop = null!;
	private Dictionary<long, IGameItem> _items = null!;
	private Dictionary<long, IGameItemProto> _registeredProtos = null!;
	private readonly List<ISalesTax> _salesTaxes = new();
	private long _nextDealId;
	private Mock<ICell> _currentCell = null!;

	[TestInitialize]
	public void Setup()
	{
		_gameworld = new Mock<IFuturemud>();
		_save = new Mock<ISaveManager>();
		_gameworld.SetupGet(x => x.SaveManager).Returns(_save.Object);
		_items = new Dictionary<long, IGameItem>();
		_registeredProtos = new Dictionary<long, IGameItemProto>();
		_nextDealId = 1;
		_salesTaxes.Clear();
		_gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
		          .Returns<long, bool>((id, add) => _items.TryGetValue(id, out var item) ? item : null);

		_currency = new Mock<ICurrency>();
		_currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
		         .Returns<decimal, CurrencyDescriptionPatternType>((value, type) => value.ToString("F0"));

		_zone = new Mock<IEconomicZone>();
		var zoneLoc = new Mock<IZone>();
		zoneLoc.SetupGet(x => x.Calendars).Returns(new List<ICalendar>());
		zoneLoc.Setup(x => x.DateTime(It.IsAny<ICalendar>())).Returns(MudDateTime.Never);
		_zone.SetupGet(x => x.ZoneForTimePurposes).Returns(zoneLoc.Object);
		_zone.SetupGet(x => x.SalesTaxes).Returns(_salesTaxes);
		_zone.SetupGet(x => x.Currency).Returns(_currency.Object);

		_zones = new Mock<IUneditableAll<IEconomicZone>>();
		_zones.Setup(x => x.Get(It.IsAny<long>())).Returns(_zone.Object);
		_gameworld.SetupGet(x => x.EconomicZones).Returns(_zones.Object);

		_currencies = new Mock<IUneditableAll<ICurrency>>();
		_currencies.Setup(x => x.Get(It.IsAny<long>())).Returns(_currency.Object);
		_gameworld.SetupGet(x => x.Currencies).Returns(_currencies.Object);

		_markets = new Mock<IUneditableAll<IMarket>>();
		_markets.Setup(x => x.Get(It.IsAny<long>())).Returns((IMarket)null);
		_gameworld.SetupGet(x => x.Markets).Returns(_markets.Object);

		_progs = new Mock<IUneditableAll<IFutureProg>>();
		_progs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null);
		var cells = new Mock<IUneditableAll<ICell>>();
		cells.Setup(x => x.Get(It.IsAny<long>())).Returns((ICell)null);
		_gameworld.SetupGet(x => x.Cells).Returns(cells.Object);
		_gameworld.SetupGet(x => x.FutureProgs).Returns(_progs.Object);

		_itemProtos = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		_itemProtos.Setup(x => x.Get(It.IsAny<long>()))
		           .Returns<long>(id => _registeredProtos.TryGetValue(id, out var proto) ? proto : null);
		_gameworld.SetupGet(x => x.ItemProtos).Returns(_itemProtos.Object);
		_tags = new All<ITag>();
		_gameworld.SetupGet(x => x.Tags).Returns(_tags);

		var actors = new MudSharp.Framework.All<ICharacter>();
		_gameworld.SetupGet(x => x.Actors).Returns(actors);
		_gameworld.SetupGet(x => x.LegalAuthorities).Returns(new All<ILegalAuthority>());
		_currentCell = new Mock<ICell>();
		_currentCell.Setup(x => x.DateTime()).Returns(MudDateTime.Never);
		_currentCell.Setup(x => x.DateTime(It.IsAny<ICalendar>())).Returns(MudDateTime.Never);
		_currentCell.SetupGet(x => x.EventHandlers).Returns(new List<IHandleEvents>());

		var model = new MudSharp.Models.Shop
		{
			Id = 1,
			Name = "Test Shop",
			CashBalance = 0m,
			ExpectedCashBalance = 0m,
			CurrencyId = 1,
			EconomicZoneId = 1,
			EmployeeRecords = "<Employees/>",
			AutopayTaxes = false,
			ShopType = "Permanent",
			Merchandises = new List<MudSharp.Models.Merchandise>(),
			ShopDeals = new List<MudSharp.Models.ShopDeal>(),
			ShopTransactionRecords = new List<MudSharp.Models.ShopTransactionRecord>(),
			LineOfCreditAccounts = new List<MudSharp.Models.LineOfCreditAccount>(),
			ShopsStoreroomCells = new List<MudSharp.Models.ShopsStoreroomCell>(),
			ShopsTills = new List<MudSharp.Models.ShopsTill>()
		};

		_shop = new TestShop(model, _gameworld.Object);
		_shop.CurrentLocationsOverride = new[] { _currentCell.Object };
		_shop.EconomicZone = _zone.Object;
		_shop.Currency = _currency.Object;
	}

	[TestMethod]
	public void DisposeFromStock_StackedItem_RecordsStockRemovalCredit()
	{
		var proto = RegisterPrototype(20);
		var fallbackMerch = new Merchandise(_shop, "fallback-removal", proto.Object, 99m, false, null, null);
		var merch = new Merchandise(_shop, "stock-removal", proto.Object, 10m, false, null, null);
		_shop.AddMerchandise(fallbackMerch);
		_shop.AddMerchandise(merch);

		var item = CreateStackedItem(200L, 3, proto.Object);

		_shop.AddToStock(null, item.Object, merch);
		_shop.DisposeFromStock(null, item.Object);

		var transaction = _shop.TransactionRecords.Last();
		Assert.AreEqual(ShopTransactionType.StockRemoval, transaction.TransactionType);
		Assert.AreEqual(30m, transaction.PretaxValue);
		Assert.AreEqual(30m, transaction.NetValue);
		Assert.AreEqual(0, _shop.StockedItems(merch).Count());
		Assert.AreEqual(0, _shop.StockedItems(fallbackMerch).Count());
	}

	[TestMethod]
	public void LoseFromStock_StackedItem_RecordsStockLoss()
	{
		var proto = RegisterPrototype(21);
		var fallbackMerch = new Merchandise(_shop, "fallback-loss", proto.Object, 99m, false, null, null);
		var merch = new Merchandise(_shop, "stock-loss", proto.Object, 10m, false, null, null);
		_shop.AddMerchandise(fallbackMerch);
		_shop.AddMerchandise(merch);

		var item = CreateStackedItem(201L, 4, proto.Object);

		_shop.AddToStock(null, item.Object, merch);
		_shop.LoseFromStock(null, item.Object);

		var transaction = _shop.TransactionRecords.Last();
		Assert.AreEqual(ShopTransactionType.StockLoss, transaction.TransactionType);
		Assert.AreEqual(40m, transaction.PretaxValue);
		Assert.AreEqual(-40m, transaction.NetValue);
		Assert.AreEqual(0, _shop.StockedItems(merch).Count());
		Assert.AreEqual(0, _shop.StockedItems(fallbackMerch).Count());
	}

	[TestMethod]
	public void TransactionRecord_NonOperationalTransactions_HaveZeroNetValue()
	{
		var now = MudDateTime.Never;
		var deposit = new TransactionRecord(ShopTransactionType.Deposit, _currency.Object, _shop, now, null, 10m, 0m, null);
		var withdrawal = new TransactionRecord(ShopTransactionType.Withdrawal, _currency.Object, _shop, now, null, 10m, 0m, null);
		var taxPayment = new TransactionRecord(ShopTransactionType.TaxPayment, _currency.Object, _shop, now, null, 10m, 0m, null);

		Assert.AreEqual(0m, deposit.NetValue);
		Assert.AreEqual(0m, withdrawal.NetValue);
		Assert.AreEqual(0m, taxPayment.NetValue);
	}

	[TestMethod]
	public void BuyingItemUpdatesStockAndTransactions()
	{
		var proto = RegisterPrototype(10);
		proto.SetupGet(x => x.Morphs).Returns(false);
		proto.SetupGet(x => x.MorphTimeSpan).Returns(TimeSpan.Zero);
		var merch = new Merchandise(_shop, "item", proto.Object, 10m, false, null, null);
		_shop.AddMerchandise(merch);

		var item = CreateStackedItem(100L, 1, proto.Object);
		item.Setup(x => x.DropsWhole(It.IsAny<int>())).Returns(true);
		item.Setup(x => x.Get(null, It.IsAny<int>())).Returns(item.Object);
		item.SetupGet(x => x.InInventoryOf).Returns((IBody)null);
		item.SetupGet(x => x.ContainedIn).Returns((IGameItem)null);
		item.SetupGet(x => x.Location).Returns((ICell)null);

		_shop.AddToStock(null, item.Object, merch);
		Assert.AreEqual(1, _shop.StockedItems(merch).Count());

		var shopAccount = new Mock<IBankAccount>();
		shopAccount.SetupGet(x => x.Currency).Returns(_currency.Object);
		_shop.BankAccount = shopAccount.Object;
		var playerAccount = new Mock<IBankAccount>();
		playerAccount.SetupGet(x => x.Currency).Returns(_currency.Object);
		playerAccount.Setup(x => x.WithdrawFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()));
		shopAccount.Setup(x => x.DepositFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()));

		var paymentItem = new Mock<IBankPaymentItem>();
		paymentItem.SetupProperty(x => x.CurrentUsesRemaining, 1);
		paymentItem.SetupProperty(x => x.BankAccount, playerAccount.Object);

		var body = new Mock<IBody>();
		body.Setup(x => x.CanGet(It.IsAny<IGameItem>(), 0, ItemCanGetIgnore.None)).Returns(true);
		body.Setup(x => x.Get(It.IsAny<IGameItem>(), 0, It.IsAny<IEmote>(), true, ItemCanGetIgnore.None));
		var location = new Mock<ICell>();
		location.SetupGet(x => x.EventHandlers).Returns(new List<IHandleEvents>());
		location.SetupGet(x => x.Calendars).Returns(new List<ICalendar>());
		location.Setup(x => x.DateTime(It.IsAny<ICalendar>())).Returns(MudDateTime.Never);
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Location).Returns(location.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()));
		output.SetupGet(x => x.Perceiver).Returns(actor.Object);

		var payment = new BankPayment(actor.Object, paymentItem.Object, _shop);
		var bought = _shop.Buy(actor.Object, merch, 1, payment).ToList();

		Assert.AreEqual(1, bought.Count);
		Assert.AreEqual(0, _shop.StockedItems(merch).Count());
		Assert.AreEqual(2, _shop.TransactionRecords.Count());
		var sale = _shop.TransactionRecords.Last();
		Assert.AreEqual(ShopTransactionType.Sale, sale.TransactionType);
		Assert.AreEqual(10m, sale.NetValue);
		playerAccount.Verify(x => x.WithdrawFromTransaction(10m, It.IsAny<string>()), Times.Once);
		shopAccount.Verify(x => x.DepositFromTransaction(10m, It.IsAny<string>()), Times.Once);
	}

	[TestMethod]
	public void SaleDeal_ChangesDisplayedBilledAndTransactionPretax()
	{
		var proto = RegisterPrototype(30);
		var merch = new Merchandise(_shop, "sale-item", proto.Object, 10m, false, null, null);
		_shop.AddMerchandise(merch);
		CreateDeal("sale", ShopDealType.Sale, ShopDealTargetType.Merchandise, -0.1m, merchandiseId: merch.Id);

		Assert.AreEqual(9m, _shop.GetDetailedPriceInfo(null, merch).TotalPrice);
		Assert.AreEqual(9m, _shop.PriceForMerchandise(null, merch, 1));

		var item = CreateStackedItem(300L, 1, proto.Object);
		var transaction = ExecuteBankPurchase(merch, item);

		Assert.AreEqual(ShopTransactionType.Sale, transaction.TransactionType);
		Assert.AreEqual(9m, transaction.PretaxValue);
		Assert.AreEqual(9m, transaction.NetValue);
	}

	[TestMethod]
	public void VolumeDeal_AppliesOnlyAtThreshold()
	{
		var proto = RegisterPrototype(31);
		var merch = new Merchandise(_shop, "bulk-item", proto.Object, 10m, false, null, null);
		_shop.AddMerchandise(merch);
		CreateDeal("bulk", ShopDealType.Volume, ShopDealTargetType.Merchandise, -0.2m, minimumQuantity: 10,
			merchandiseId: merch.Id);

		Assert.AreEqual(90m, _shop.PriceForMerchandise(null, merch, 9));
		Assert.AreEqual(80m, _shop.PriceForMerchandise(null, merch, 10));
		Assert.IsTrue(_shop.GetDetailedPriceInfo(null, merch).VolumeDealsExist);
	}

	[TestMethod]
	public void TagTargetedDeal_OnlyAppliesToMatchingMerchandise()
	{
		var tag = RegisterTag(1, "Discounted");
		var matchingProto = RegisterPrototype(32, tag.Object);
		var otherProto = RegisterPrototype(33);
		var matchingMerch = new Merchandise(_shop, "tagged", matchingProto.Object, 10m, false, null, null);
		var otherMerch = new Merchandise(_shop, "plain", otherProto.Object, 10m, false, null, null);
		_shop.AddMerchandise(matchingMerch);
		_shop.AddMerchandise(otherMerch);
		CreateDeal("tagged sale", ShopDealType.Sale, ShopDealTargetType.ItemTag, -0.5m, tagId: tag.Object.Id);

		Assert.AreEqual(5m, _shop.PriceForMerchandise(null, matchingMerch, 1));
		Assert.AreEqual(10m, _shop.PriceForMerchandise(null, otherMerch, 1));
	}

	[TestMethod]
	public void CumulativeAndNonCumulativeDeals_SelectBestCombination()
	{
		var proto = RegisterPrototype(34);
		var merch = new Merchandise(_shop, "combo", proto.Object, 100m, false, null, null);
		_shop.AddMerchandise(merch);
		CreateDeal("cum down", ShopDealType.Sale, ShopDealTargetType.Merchandise, -0.1m, isCumulative: true,
			merchandiseId: merch.Id);
		CreateDeal("cum up", ShopDealType.Sale, ShopDealTargetType.Merchandise, 0.05m, isCumulative: true,
			merchandiseId: merch.Id);
		CreateDeal("best", ShopDealType.Sale, ShopDealTargetType.Merchandise, -0.3m, isCumulative: false,
			merchandiseId: merch.Id);
		CreateDeal("worse", ShopDealType.Sale, ShopDealTargetType.Merchandise, -0.2m, isCumulative: false,
			merchandiseId: merch.Id);

		var calculation = _shop.GetPriceCalculation(null, merch, 1);
		Assert.AreEqual(-0.35m, calculation.TotalPriceAdjustmentPercentage);
		Assert.AreEqual(65m, calculation.TotalPrice);
	}

	[TestMethod]
	public void ExpiredDeal_IsIgnored()
	{
		var proto = RegisterPrototype(35);
		var merch = new Merchandise(_shop, "expired", proto.Object, 10m, false, null, null);
		_shop.AddMerchandise(merch);

		var expiredDeal = new Mock<IShopDeal>();
		expiredDeal.SetupGet(x => x.Shop).Returns(_shop);
		expiredDeal.SetupGet(x => x.Name).Returns("expired");
		expiredDeal.SetupGet(x => x.Id).Returns(999L);
		expiredDeal.SetupGet(x => x.FrameworkItemType).Returns("ShopDeal");
		expiredDeal.SetupGet(x => x.DealType).Returns(ShopDealType.Sale);
		expiredDeal.SetupGet(x => x.TargetType).Returns(ShopDealTargetType.Merchandise);
		expiredDeal.SetupGet(x => x.PriceAdjustmentPercentage).Returns(-0.5m);
		expiredDeal.SetupGet(x => x.Applicability).Returns(ShopDealApplicability.Sell);
		expiredDeal.SetupGet(x => x.IsCumulative).Returns(true);
		expiredDeal.SetupGet(x => x.IsExpired).Returns(true);
		expiredDeal.Setup(x => x.AppliesToMerchandise(merch)).Returns(true);
		expiredDeal.Setup(x => x.Applies(merch, It.IsAny<ICharacter>(), It.IsAny<int>(), ShopDealApplicability.Sell,
			It.IsAny<MudDateTime>())).Returns(false);
		_shop.AddDeal(expiredDeal.Object);

		Assert.AreEqual(10m, _shop.PriceForMerchandise(null, merch, 1));
	}

	[TestMethod]
	public void BuySideDeal_AffectsSellToShopWithoutChangingSellPrice()
	{
		var proto = RegisterPrototype(36);
		var merch = new Merchandise(_shop, "buy-side", proto.Object, 20m, false, null, null);
		_shop.AddMerchandise(merch);
		CreateDeal("preferred seller", ShopDealType.Sale, ShopDealTargetType.Merchandise, 0.5m,
			applicability: ShopDealApplicability.Buy, merchandiseId: merch.Id);

		Assert.AreEqual(20m, _shop.PriceForMerchandise(null, merch, 1));
		Assert.AreEqual(9m, _shop.GetPriceCalculation(null, merch, 1, ShopDealApplicability.Buy).TotalPrice);

		var item = CreateStackedItem(301L, 1, proto.Object);
		var actor = new Mock<ICharacter>();
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		output.SetupGet(x => x.Perceiver).Returns(actor.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
		actor.SetupGet(x => x.Location).Returns(_currentCell.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var payment = new Mock<IPaymentMethod>();
		payment.Setup(x => x.GivePayment(It.IsAny<decimal>()));

		_shop.Sell(actor.Object, merch, payment.Object, item.Object);

		var transaction = _shop.TransactionRecords.Last();
		Assert.AreEqual(ShopTransactionType.Purchase, transaction.TransactionType);
		Assert.AreEqual(-9m, transaction.PretaxValue);
		payment.Verify(x => x.GivePayment(9m), Times.Once);
	}

	[TestMethod]
	public void AdjustedTax_UsesDiscountedPretax()
	{
		var tax = new Mock<ISalesTax>();
		tax.Setup(x => x.Applies(It.IsAny<IMerchandise>(), It.IsAny<ICharacter>())).Returns(true);
		tax.Setup(x => x.TaxValue(It.IsAny<IMerchandise>(), It.IsAny<ICharacter>(), It.IsAny<decimal>()))
		   .Returns<IMerchandise, ICharacter, decimal>((m, c, saleValue) => saleValue * 0.2m);
		tax.Setup(x => x.TaxValue(It.IsAny<IMerchandise>(), It.IsAny<ICharacter>())).Returns(4m);
		_salesTaxes.Add(tax.Object);

		var proto = RegisterPrototype(37);
		var merch = new Merchandise(_shop, "taxed", proto.Object, 20m, false, null, null);
		_shop.AddMerchandise(merch);
		CreateDeal("half off", ShopDealType.Sale, ShopDealTargetType.Merchandise, -0.5m, merchandiseId: merch.Id);

		var calculation = _shop.GetPriceCalculation(null, merch, 1);
		Assert.AreEqual(10m, calculation.TotalPretaxPrice);
		Assert.AreEqual(2m, calculation.IncludedTax);
		Assert.AreEqual(12m, calculation.TotalPrice);

		var item = CreateStackedItem(302L, 1, proto.Object);
		var transaction = ExecuteBankPurchase(merch, item);
		Assert.AreEqual(10m, transaction.PretaxValue);
		Assert.AreEqual(2m, transaction.Tax);
	}

	private Mock<IGameItemProto> RegisterPrototype(long id, params ITag[] tags)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id);
		proto.SetupGet(x => x.Tags).Returns(tags.ToList());
		_registeredProtos[id] = proto.Object;
		return proto;
	}

	private Mock<ITag> RegisterTag(long id, string name)
	{
		var tag = new Mock<ITag>();
		tag.SetupGet(x => x.Id).Returns(id);
		tag.SetupGet(x => x.Name).Returns(name);
		tag.SetupGet(x => x.FullName).Returns(name);
		tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(other => other == tag.Object);
		_tags.Add(tag.Object);
		return tag;
	}

	private IShopDeal CreateDeal(string name, ShopDealType dealType, ShopDealTargetType targetType,
		decimal adjustment, int minimumQuantity = 0, ShopDealApplicability applicability = ShopDealApplicability.Sell,
		bool isCumulative = true, long? merchandiseId = null, long? tagId = null)
	{
		var deal = new ShopDeal(new MudSharp.Models.ShopDeal
		{
			Id = _nextDealId++,
			ShopId = _shop.Id,
			Name = name,
			DealType = (int)dealType,
			TargetType = (int)targetType,
			MerchandiseId = merchandiseId,
			TagId = tagId,
			PriceAdjustmentPercentage = adjustment,
			MinimumQuantity = dealType == ShopDealType.Volume ? minimumQuantity : null,
			Applicability = (int)applicability,
			IsCumulative = isCumulative
		}, _shop, _gameworld.Object);
		_shop.AddDeal(deal);
		return deal;
	}

	private Mock<IGameItem> CreateStackedItem(long id, int quantity, IGameItemProto prototype)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Quantity).Returns(quantity);
		item.SetupGet(x => x.Prototype).Returns(prototype);
		item.Setup(x => x.DropsWhole(It.IsAny<int>())).Returns<int>(q => q >= quantity);
		item.Setup(x => x.Delete());
		item.Setup(x => x.AddEffect(It.IsAny<IEffect>()));
		item.Setup(x => x.RemoveAllEffects<ItemOnDisplayInShop>(It.IsAny<Predicate<ItemOnDisplayInShop>>(), true));
		_items[id] = item.Object;
		return item;
	}

	private ITransactionRecord ExecuteBankPurchase(IMerchandise merchandise, Mock<IGameItem> item, int quantity = 1)
	{
		item.Setup(x => x.Get(null, It.IsAny<int>())).Returns(item.Object);
		item.SetupGet(x => x.InInventoryOf).Returns((IBody)null);
		item.SetupGet(x => x.ContainedIn).Returns((IGameItem)null);
		item.SetupGet(x => x.Location).Returns((ICell)null);
		_shop.AddToStock(null, item.Object, merchandise);

		var shopAccount = new Mock<IBankAccount>();
		shopAccount.SetupGet(x => x.Currency).Returns(_currency.Object);
		_shop.BankAccount = shopAccount.Object;
		var playerAccount = new Mock<IBankAccount>();
		playerAccount.SetupGet(x => x.Currency).Returns(_currency.Object);
		playerAccount.Setup(x => x.WithdrawFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()));
		shopAccount.Setup(x => x.DepositFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()));

		var paymentItem = new Mock<IBankPaymentItem>();
		paymentItem.SetupProperty(x => x.CurrentUsesRemaining, 1);
		paymentItem.SetupProperty(x => x.BankAccount, playerAccount.Object);

		var body = new Mock<IBody>();
		body.Setup(x => x.CanGet(It.IsAny<IGameItem>(), 0, ItemCanGetIgnore.None)).Returns(true);
		body.Setup(x => x.Get(It.IsAny<IGameItem>(), 0, It.IsAny<IEmote>(), true, ItemCanGetIgnore.None));
		var location = new Mock<ICell>();
		location.SetupGet(x => x.EventHandlers).Returns(new List<IHandleEvents>());
		location.SetupGet(x => x.Calendars).Returns(new List<ICalendar>());
		location.Setup(x => x.DateTime(It.IsAny<ICalendar>())).Returns(MudDateTime.Never);
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Location).Returns(location.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()));
		output.SetupGet(x => x.Perceiver).Returns(actor.Object);

		var payment = new BankPayment(actor.Object, paymentItem.Object, _shop);
		_shop.Buy(actor.Object, merchandise, quantity, payment).ToList();
		return _shop.TransactionRecords.Last();
	}

	private class TestShop : PermanentShop
	{
		public IEnumerable<ICell> CurrentLocationsOverride { get; set; } = Enumerable.Empty<ICell>();
		public TestShop(MudSharp.Models.Shop model, IFuturemud gameworld) : base(model, gameworld) { }
		protected override void Save(MudSharp.Models.Shop dbitem) { }
		public override bool IsReadyToDoBusiness => true;
		public override IEnumerable<ICell> CurrentLocations => CurrentLocationsOverride;
		public override IEnumerable<IGameItem> DoAutoRestockForMerchandise(IMerchandise merchandise, List<(IGameItem Item, IGameItem Container)> purchasedItems = null) => Enumerable.Empty<IGameItem>();
		public override IEnumerable<IGameItem> DoAutostockForMerchandise(IMerchandise merchandise) => Enumerable.Empty<IGameItem>();
		public override void SortItemToStorePhysicalLocation(IGameItem item, IMerchandise merchandise, IGameItem container) { }
		public override IEnumerable<IGameItem> StockedItems(IMerchandise merchandise)
		{
			return _stockedMerchandise[merchandise].Select(id => Gameworld.TryGetItem(id, true)).Where(x => x != null)!;
		}
		public override IEnumerable<IGameItem> AllStockedItems => _stockedMerchandise.SelectMany(x => x.Value).Select(id => Gameworld.TryGetItem(id, true)).Where(x => x != null)!;
	}
}
