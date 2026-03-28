using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using DbAuctionHouse = MudSharp.Models.AuctionHouse;
using DbPropertyLeaseOrder = MudSharp.Models.PropertyLeaseOrder;
using DbPropertySaleOrder = MudSharp.Models.PropertySaleOrder;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BootLoadingRegressionTests
{
	[TestMethod]
	public void TryGetCharacter_PreNpcBootPhase_ThrowsBeforeMaterialisingNewCharacter()
	{
		var futuremud = new Futuremud(null);
		futuremud.SetCharacterMaterialisationBootPhase(CharacterMaterialisationBootPhase.Disallowed);

		Assert.ThrowsException<InvalidOperationException>(() => futuremud.TryGetCharacter(123L, true));
	}

	[TestMethod]
	public void TryGetCharacter_PreNpcBootPhase_ReturnsAlreadyLoadedActor()
	{
		var futuremud = new Futuremud(null);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(123L);
		actor.SetupGet(x => x.Name).Returns("Guard");
		futuremud.Add(actor.Object, true);
		futuremud.SetCharacterMaterialisationBootPhase(CharacterMaterialisationBootPhase.Disallowed);

		var result = futuremud.TryGetCharacter(123L, false);

		Assert.AreSame(actor.Object, result);
	}

	[TestMethod]
	public void PropertySaleOrder_DbConstructor_MatchesConsentWithoutDereferencingOwner()
	{
		var gameworld = new Mock<IFuturemud>();
		var owner = new Mock<IPropertyOwner>();
		owner.SetupGet(x => x.OwnerId).Returns(44L);
		owner.SetupGet(x => x.OwnerFrameworkItemType).Returns("Character");
		owner.SetupGet(x => x.Owner).Throws(new AssertFailedException("Owner should not be dereferenced during load."));

		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		property.SetupGet(x => x.PropertyOwners).Returns(new[] { owner.Object });

		var dbitem = new DbPropertySaleOrder
		{
			Id = 1L,
			ReservePrice = 1200.0M,
			StartOfListing = "Never",
			DurationOfListingDays = 14.0,
			OrderStatus = (int)PropertySaleOrderStatus.Approved,
			PropertyOwnerConsentInfo = new XElement("Owners",
				new XElement("Owner",
					new XAttribute("id", 44L),
					new XAttribute("type", "Character"),
					new XAttribute("consent", true))
			).ToString()
		};

		var order = new MudSharp.Economy.Property.PropertySaleOrder(dbitem, gameworld.Object, property.Object);

		Assert.AreEqual(1, order.PropertyOwnerConsent.Count);
		Assert.IsTrue(order.PropertyOwnerConsent[owner.Object]);
	}

	[TestMethod]
	public void PropertyLeaseOrder_DbConstructor_MatchesConsentWithoutDereferencingOwner()
	{
		var futureProgs = new Mock<IUneditableAll<IFutureProg>>();
		futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);

		var owner = new Mock<IPropertyOwner>();
		owner.SetupGet(x => x.OwnerId).Returns(55L);
		owner.SetupGet(x => x.OwnerFrameworkItemType).Returns("Character");
		owner.SetupGet(x => x.Owner).Throws(new AssertFailedException("Owner should not be dereferenced during load."));

		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		property.SetupGet(x => x.PropertyOwners).Returns(new[] { owner.Object });
		property.SetupGet(x => x.LeaseOrder).Returns((IPropertyLeaseOrder)null);

		var dbitem = new DbPropertyLeaseOrder
		{
			Id = 2L,
			PricePerInterval = 75.0M,
			BondRequired = 200.0M,
			Interval = "14 Daily",
			MinimumLeaseDurationDays = 14.0,
			MaximumLeaseDurationDays = 60.0,
			AllowAutoRenew = true,
			AutomaticallyRelistAfterLeaseTerm = true,
			AllowLeaseNovation = true,
			RekeyOnLeaseEnd = false,
			ListedForLease = false,
			FeeIncreasePercentageAfterLeaseTerm = 0.0M,
			PropertyOwnerConsentInfo = new XElement("Owners",
				new XElement("Owner",
					new XAttribute("id", 55L),
					new XAttribute("type", "Character"),
					new XAttribute("consent", false))
			).ToString()
		};

		var order = new MudSharp.Economy.Property.PropertyLeaseOrder(dbitem, property.Object);

		Assert.AreEqual(1, order.PropertyOwnerConsent.Count);
		Assert.IsFalse(order.PropertyOwnerConsent[owner.Object]);
	}

	[TestMethod]
	public void AuctionHouse_DbConstructor_DefersCharacterResolutionUntilFinaliseLoading()
	{
		var zone = new Mock<IEconomicZone>();
		zone.SetupGet(x => x.Id).Returns(1L);
		zone.SetupGet(x => x.Name).Returns("Central");

		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(2L);
		cell.SetupGet(x => x.Name).Returns("Auction Floor");

		var profitsAccount = new Mock<IBankAccount>();
		profitsAccount.SetupGet(x => x.Id).Returns(33L);
		profitsAccount.SetupGet(x => x.Name).Returns("Profits");
		profitsAccount.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");

		var payoutAccount = new Mock<IBankAccount>();
		payoutAccount.SetupGet(x => x.Id).Returns(44L);
		payoutAccount.SetupGet(x => x.Name).Returns("Seller Account");
		payoutAccount.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");

		var seller = new Mock<ICharacter>();
		seller.SetupGet(x => x.Id).Returns(22L);
		seller.SetupGet(x => x.Name).Returns("Seller");
		seller.SetupGet(x => x.FrameworkItemType).Returns("Character");

		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(11L);
		item.SetupGet(x => x.Name).Returns("Sabre");
		item.SetupGet(x => x.FrameworkItemType).Returns("GameItem");

		var heartbeatManager = new Mock<IHeartbeatManager>();
		var economicZones = new All<IEconomicZone>();
		economicZones.Add(zone.Object);
		var cells = new All<ICell>();
		cells.Add(cell.Object);
		var bankAccounts = new All<IBankAccount>();
		bankAccounts.Add(profitsAccount.Object);
		bankAccounts.Add(payoutAccount.Object);
		var properties = new All<IProperty>();

		IPostCharacterLoadFinalisable registeredFinalisable = null;
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager.Object);
		gameworld.SetupGet(x => x.EconomicZones).Returns(economicZones);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		gameworld.SetupGet(x => x.BankAccounts).Returns(bankAccounts);
		gameworld.SetupGet(x => x.Properties).Returns(properties);
		gameworld.Setup(x => x.TryGetItem(11L, true)).Returns(item.Object);
		gameworld.Setup(x => x.TryGetCharacter(22L, true)).Returns(seller.Object);
		gameworld.Setup(x => x.RegisterPostCharacterLoadFinalisable(It.IsAny<IPostCharacterLoadFinalisable>()))
			.Callback<IPostCharacterLoadFinalisable>(x => registeredFinalisable = x);

		var dbitem = new DbAuctionHouse
		{
			Id = 10L,
			Name = "Central Auction House",
			EconomicZoneId = zone.Object.Id,
			AuctionHouseCellId = cell.Object.Id,
			ProfitsBankAccountId = profitsAccount.Object.Id,
			AuctionListingFeeFlat = 1.0M,
			AuctionListingFeeRate = 0.05M,
			DefaultListingTime = 3600.0,
			Definition = new XElement("Definition",
				new XElement("ActiveItem",
					new XAttribute("kind", "Item"),
					new XAttribute("assetid", item.Object.Id),
					new XAttribute("assettype", "GameItem"),
					new XAttribute("sellerid", seller.Object.Id),
					new XAttribute("sellertype", "Character"),
					new XAttribute("payoutid", payoutAccount.Object.Id),
					new XAttribute("payouttype", "BankAccount"),
					new XAttribute("character", seller.Object.Id),
					new XAttribute("item", item.Object.Id),
					new XAttribute("price", 100.0M),
					new XAttribute("buyout", 150.0M),
					new XAttribute("list", "Never"),
					new XAttribute("finish", "Never"),
					new XAttribute("account", payoutAccount.Object.Id))
			).ToString()
		};

		var house = new AuctionHouse(dbitem, gameworld.Object);

		Assert.AreSame(house, registeredFinalisable);
		Assert.AreEqual(0, house.ActiveAuctionItems.Count());
		gameworld.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);

		house.FinaliseLoading();

		var loadedItem = house.ActiveAuctionItems.Single();
		Assert.AreSame(seller.Object, loadedItem.Seller);
		Assert.AreSame(payoutAccount.Object, loadedItem.PayoutTarget);
		gameworld.Verify(x => x.TryGetCharacter(22L, true), Times.Once);
	}
}
