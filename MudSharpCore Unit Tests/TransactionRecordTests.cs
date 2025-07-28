using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Shops;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.Framework.Save;
using MudSharp.Economy;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TransactionRecordTests
{
    private Mock<ICurrency> _currency = null!;
    private Mock<IShop> _shop = null!;
    private Mock<IFuturemud> _gameworld = null!;
    private Mock<ISaveManager> _save = null!;
    private Mock<IEconomicZone> _zone = null!;

    [TestInitialize]
    public void Setup()
    {
        _currency = new Mock<ICurrency>();
        _shop = new Mock<IShop>();
        _gameworld = new Mock<IFuturemud>();
        _save = new Mock<ISaveManager>();
        _zone = new Mock<IEconomicZone>();
        _gameworld.SetupGet(x => x.SaveManager).Returns(_save.Object);
        _save.Setup(x => x.Add(It.IsAny<ISaveable>()));
        _save.Setup(x => x.AddInitialisation(It.IsAny<ILateInitialisingItem>()));
        _shop.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
        _shop.SetupGet(x => x.EconomicZone).Returns(_zone.Object);
    }

    [TestMethod]
    public void NetValueReflectsTransactionType()
    {
        var md = MudDateTime.Never;
        var sale = new TransactionRecord(ShopTransactionType.Sale, _currency.Object, _shop.Object, md, null, 10m, 2m, null);
        var restock = new TransactionRecord(ShopTransactionType.Restock, _currency.Object, _shop.Object, md, null, 5m, 0m, null);
        var deposit = new TransactionRecord(ShopTransactionType.Deposit, _currency.Object, _shop.Object, md, null, 5m, 0m, null);
        var withdrawal = new TransactionRecord(ShopTransactionType.Withdrawal, _currency.Object, _shop.Object, md, null, 7m, 0m, null);
        Assert.AreEqual(10m, sale.NetValue);
        Assert.AreEqual(-5m, restock.NetValue);
        Assert.AreEqual(5m, deposit.NetValue);
        Assert.AreEqual(-7m, withdrawal.NetValue);
    }
}
