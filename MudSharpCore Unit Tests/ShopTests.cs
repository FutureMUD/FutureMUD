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

namespace MudSharp_Unit_Tests;

[TestClass]
public class ShopTests
{
    private Mock<IFuturemud> _gameworld = null!;
    private Mock<IUneditableAll<IEconomicZone>> _zones = null!;
    private Mock<IUneditableAll<ICurrency>> _currencies = null!;
    private Mock<IUneditableAll<IMarket>> _markets = null!;
    private Mock<IUneditableAll<IFutureProg>> _progs = null!;
    private Mock<ISaveManager> _save = null!;
    private Mock<IEconomicZone> _zone = null!;
    private Mock<ICurrency> _currency = null!;
    private TestShop _shop = null!;
    private Dictionary<long, IGameItem> _items = null!;

    [TestInitialize]
    public void Setup()
    {
        _gameworld = new Mock<IFuturemud>();
        _save = new Mock<ISaveManager>();
        _gameworld.SetupGet(x => x.SaveManager).Returns(_save.Object);
        _items = new Dictionary<long, IGameItem>();
        _gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>())).Returns<long, bool>((id, add) => _items.TryGetValue(id, out var item) ? item : null);

        _currency = new Mock<ICurrency>();
        _currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
                .Returns<decimal, CurrencyDescriptionPatternType>((value, type) => value.ToString("F0"));

        _zone = new Mock<IEconomicZone>();
        var zoneLoc = new Mock<IZone>();
        zoneLoc.SetupGet(x => x.Calendars).Returns(new List<ICalendar>());
        zoneLoc.Setup(x => x.DateTime(It.IsAny<ICalendar>())).Returns(MudDateTime.Never);
        _zone.SetupGet(x => x.ZoneForTimePurposes).Returns(zoneLoc.Object);
        _zone.SetupGet(x => x.SalesTaxes).Returns(new List<ISalesTax>());
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
        var actors = new MudSharp.Framework.All<ICharacter>();
        _gameworld.SetupGet(x => x.Actors).Returns(actors);

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
            ShopTransactionRecords = new List<MudSharp.Models.ShopTransactionRecord>(),
            LineOfCreditAccounts = new List<MudSharp.Models.LineOfCreditAccount>(),
            ShopsStoreroomCells = new List<MudSharp.Models.ShopsStoreroomCell>(),
            ShopsTills = new List<MudSharp.Models.ShopsTill>()
        };

        _shop = new TestShop(model, _gameworld.Object);
        _shop.EconomicZone = _zone.Object;
        _shop.Currency = _currency.Object;
    }

    [TestMethod]
    public void BuyingItemUpdatesStockAndTransactions()
    {
        var proto = new Mock<IGameItemProto>();
        proto.SetupGet(x => x.Id).Returns(10);
        proto.SetupGet(x => x.Morphs).Returns(false);
        proto.SetupGet(x => x.MorphTimeSpan).Returns(TimeSpan.Zero);
        var merch = new Merchandise(_shop, "item", proto.Object, 10m, false, null, null);
        _shop.AddMerchandise(merch);

        var item = new Mock<IGameItem>();
        item.SetupGet(x => x.Id).Returns(100L);
        item.SetupGet(x => x.Quantity).Returns(1);
        item.Setup(x => x.DropsWhole(It.IsAny<int>())).Returns(true);
        item.Setup(x => x.Get(null, It.IsAny<int>())).Returns(item.Object);
        item.Setup(x => x.AddEffect(It.IsAny<IEffect>()));
        item.Setup(x => x.RemoveAllEffects<ItemOnDisplayInShop>(It.IsAny<Predicate<ItemOnDisplayInShop>>(), true));
        item.SetupGet(x => x.Prototype).Returns(proto.Object);
        item.SetupGet(x => x.InInventoryOf).Returns((IBody)null);
        item.SetupGet(x => x.ContainedIn).Returns((IGameItem)null);
        item.SetupGet(x => x.Location).Returns((ICell)null);
        _items[item.Object.Id] = item.Object;

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

    private class TestShop : PermanentShop
    {
        public TestShop(MudSharp.Models.Shop model, IFuturemud gameworld) : base(model, gameworld) { }
        protected override void Save(MudSharp.Models.Shop dbitem) { }
        public override bool IsReadyToDoBusiness => true;
        public override IEnumerable<ICell> CurrentLocations => Enumerable.Empty<ICell>();
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
