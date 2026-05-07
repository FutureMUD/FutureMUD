using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Property;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using System.Linq;
using System.Xml.Linq;
using DbAuctionHouse = MudSharp.Models.AuctionHouse;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuctionHouseSettlementTests
{
	[TestInitialize]
	public void Setup()
	{
		VirtualCashLedger.ClearInMemoryForTests();
	}

	private static AuctionHouse CreateAuctionHouse(
		out Mock<IBankAccount> profitsAccount,
		out Mock<IBankAccount> payoutAccount,
		out Mock<ICurrency> currency,
		bool linkedProfitsAccount = true)
	{
		currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(1L);
		currency.SetupGet(x => x.Name).Returns("dollars");
		currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
		        .Returns<decimal, CurrencyDescriptionPatternType>((value, _) => value.ToString("N2"));

		Mock<IBank> profitsBank = new();
		profitsBank.SetupGet(x => x.Id).Returns(11L);
		profitsBank.SetupGet(x => x.Code).Returns("AHB");
		profitsBank.SetupGet(x => x.CurrencyReserves).Returns(new DecimalCounter<ICurrency>());

		Mock<IBank> payoutBank = new();
		payoutBank.SetupGet(x => x.Id).Returns(12L);
		payoutBank.SetupGet(x => x.Code).Returns("PAY");
		payoutBank.SetupGet(x => x.CurrencyReserves).Returns(new DecimalCounter<ICurrency>());

		profitsAccount = new Mock<IBankAccount>();
		profitsAccount.SetupGet(x => x.Id).Returns(21L);
		profitsAccount.SetupGet(x => x.Name).Returns("Auction Profits");
		profitsAccount.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");
		profitsAccount.SetupGet(x => x.Bank).Returns(profitsBank.Object);
		profitsAccount.SetupGet(x => x.Currency).Returns(currency.Object);
		profitsAccount.SetupGet(x => x.AccountNumber).Returns(1001);
		profitsAccount.Setup(x => x.CanWithdraw(It.IsAny<decimal>(), true)).Returns((true, string.Empty));

		payoutAccount = new Mock<IBankAccount>();
		payoutAccount.SetupGet(x => x.Id).Returns(22L);
		payoutAccount.SetupGet(x => x.Name).Returns("Seller Account");
		payoutAccount.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");
		payoutAccount.SetupGet(x => x.Bank).Returns(payoutBank.Object);
		payoutAccount.SetupGet(x => x.Currency).Returns(currency.Object);
		payoutAccount.SetupGet(x => x.AccountNumber).Returns(2002);

		Mock<IEconomicZone> zone = new();
		zone.SetupGet(x => x.Id).Returns(31L);
		zone.SetupGet(x => x.Name).Returns("Central");
		zone.SetupGet(x => x.Currency).Returns(currency.Object);

		Mock<ICalendar> calendar = new();
		calendar.SetupGet(x => x.CurrentDateTime).Returns(MudDateTime.Never);
		zone.SetupGet(x => x.FinancialPeriodReferenceCalendar).Returns(calendar.Object);

		Mock<ICell> cell = new();
		cell.SetupGet(x => x.Id).Returns(41L);
		cell.SetupGet(x => x.Name).Returns("Auction Floor");

		Mock<IHeartbeatManager> heartbeatManager = new();
		All<IEconomicZone> economicZones = new();
		economicZones.Add(zone.Object);
		All<ICell> cells = new();
		cells.Add(cell.Object);
		All<IBankAccount> bankAccounts = new();
		if (linkedProfitsAccount)
		{
			bankAccounts.Add(profitsAccount.Object);
		}
		bankAccounts.Add(payoutAccount.Object);
		All<IProperty> properties = new();

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		gameworld.SetupGet(x => x.EconomicZones).Returns(economicZones);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		gameworld.SetupGet(x => x.BankAccounts).Returns(bankAccounts);
		gameworld.SetupGet(x => x.Properties).Returns(properties);

		DbAuctionHouse dbitem = new()
		{
			Id = 51L,
			Name = "Central Auction House",
			EconomicZoneId = zone.Object.Id,
			AuctionHouseCellId = cell.Object.Id,
			ProfitsBankAccountId = linkedProfitsAccount ? profitsAccount.Object.Id : null,
			AuctionListingFeeFlat = 10.0M,
			AuctionListingFeeRate = 0.10M,
			DefaultListingTime = 3600.0,
			Definition = new XElement("Definition").ToString()
		};

		return new AuctionHouse(dbitem, gameworld.Object);
	}

	[TestMethod]
	public void BuyoutItem_ActiveItem_CompletesImmediatelyAndPaysNetSellerProceeds()
	{
		var house = CreateAuctionHouse(out var profitsAccount, out var payoutAccount, out _);
		Mock<ICharacter> seller = new();
		seller.SetupGet(x => x.Id).Returns(61L);
		seller.SetupGet(x => x.Name).Returns("Seller");
		seller.SetupGet(x => x.FrameworkItemType).Returns("Character");

		Mock<ICharacter> bidder = new();
		bidder.SetupGet(x => x.Id).Returns(62L);
		bidder.SetupGet(x => x.Name).Returns("Bidder");
		bidder.SetupGet(x => x.FrameworkItemType).Returns("Character");

		Mock<IGameItem> asset = new();
		asset.SetupGet(x => x.Id).Returns(71L);
		asset.SetupGet(x => x.Name).Returns("sabre");
		asset.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		asset.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		     .Returns("a sabre");

		AuctionItem lot = new()
		{
			Asset = asset.Object,
			Seller = seller.Object,
			PayoutTarget = payoutAccount.Object,
			MinimumPrice = 100.0M,
			BuyoutPrice = 200.0M,
			ListingDateTime = MudDateTime.Never,
			FinishingDateTime = MudDateTime.Never
		};

		house.AddAuctionItem(lot);

		house.BuyoutItem(lot, new AuctionBid
		{
			Bidder = bidder.Object,
			Bid = 200.0M,
			BidDateTime = MudDateTime.Never
		});

		Assert.AreEqual(0, house.ActiveAuctionItems.Count());
		Assert.AreSame(lot, house.UnclaimedItems.Single().AuctionItem);
		Assert.AreEqual(200.0M, house.UnclaimedItems.Single().WinningBid!.Bid);
		Assert.AreEqual(30.0M, house.CashBalance);
		profitsAccount.Verify(x => x.Deposit(It.IsAny<decimal>()), Times.Never);
		profitsAccount.Verify(x => x.DepositFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
		profitsAccount.Verify(x => x.WithdrawFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
		payoutAccount.Verify(x => x.DepositFromTransaction(170.0M, It.IsAny<string>()), Times.Once);
		var ledger = VirtualCashLedger.LedgerEntries(house, 10).ToList();
		Assert.AreEqual(2, ledger.Count);
		Assert.AreEqual(200.0M, ledger.Single(x => x.Amount > 0.0M).Amount);
		Assert.AreEqual(-170.0M, ledger.Single(x => x.Amount < 0.0M).Amount);
	}

	[TestMethod]
	public void BuyoutItem_BanklessAuctionHouse_PaysFromVirtualReserve()
	{
		var house = CreateAuctionHouse(out var profitsAccount, out var payoutAccount, out _, linkedProfitsAccount: false);
		Assert.IsNull(house.ProfitsBankAccount);

		Mock<ICharacter> seller = new();
		seller.SetupGet(x => x.Id).Returns(63L);
		seller.SetupGet(x => x.Name).Returns("Seller");
		seller.SetupGet(x => x.FrameworkItemType).Returns("Character");

		Mock<ICharacter> bidder = new();
		bidder.SetupGet(x => x.Id).Returns(64L);
		bidder.SetupGet(x => x.Name).Returns("Bidder");
		bidder.SetupGet(x => x.FrameworkItemType).Returns("Character");

		Mock<IGameItem> asset = new();
		asset.SetupGet(x => x.Id).Returns(72L);
		asset.SetupGet(x => x.Name).Returns("bracelet");
		asset.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		asset.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		     .Returns("a bracelet");

		AuctionItem lot = new()
		{
			Asset = asset.Object,
			Seller = seller.Object,
			PayoutTarget = payoutAccount.Object,
			MinimumPrice = 100.0M,
			BuyoutPrice = 200.0M,
			ListingDateTime = MudDateTime.Never,
			FinishingDateTime = MudDateTime.Never
		};

		house.AddAuctionItem(lot);
		house.BuyoutItem(lot, new AuctionBid
		{
			Bidder = bidder.Object,
			Bid = 200.0M,
			BidDateTime = MudDateTime.Never
		});

		Assert.AreEqual(30.0M, house.CashBalance);
		profitsAccount.Verify(x => x.Deposit(It.IsAny<decimal>()), Times.Never);
		profitsAccount.Verify(x => x.WithdrawFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
		payoutAccount.Verify(x => x.DepositFromTransaction(170.0M, It.IsAny<string>()), Times.Once);
	}

	[TestMethod]
	public void BuyoutItem_CashPayoutTargetWithOwnedBank_QueuesCashCollection()
	{
		var house = CreateAuctionHouse(out _, out var payoutAccount, out _);
		Mock<ICharacter> seller = new();
		seller.SetupGet(x => x.Id).Returns(65L);
		seller.SetupGet(x => x.Name).Returns("Seller");
		seller.SetupGet(x => x.FrameworkItemType).Returns("Character");
		payoutAccount.Setup(x => x.IsAccountOwner(seller.Object)).Returns(true);

		Mock<ICharacter> bidder = new();
		bidder.SetupGet(x => x.Id).Returns(66L);
		bidder.SetupGet(x => x.Name).Returns("Bidder");
		bidder.SetupGet(x => x.FrameworkItemType).Returns("Character");

		Mock<IGameItem> asset = new();
		asset.SetupGet(x => x.Id).Returns(73L);
		asset.SetupGet(x => x.Name).Returns("ring");
		asset.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		asset.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		     .Returns("a ring");

		AuctionItem lot = new()
		{
			Asset = asset.Object,
			Seller = seller.Object,
			PayoutTarget = seller.Object,
			MinimumPrice = 100.0M,
			BuyoutPrice = 200.0M,
			ListingDateTime = MudDateTime.Never,
			FinishingDateTime = MudDateTime.Never
		};

		house.AddAuctionItem(lot);
		house.BuyoutItem(lot, new AuctionBid
		{
			Bidder = bidder.Object,
			Bid = 200.0M,
			BidDateTime = MudDateTime.Never
		});

		Assert.AreEqual(200.0M, house.CashBalance);
		Assert.AreEqual(170.0M, house.BidderRefundsOwed[seller.Object.Id]);
		payoutAccount.Verify(x => x.DepositFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
	}
}
