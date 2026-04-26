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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
        Futuremud futuremud = new(null);
        futuremud.SetCharacterMaterialisationBootPhase(CharacterMaterialisationBootPhase.Disallowed);

        Assert.ThrowsException<InvalidOperationException>(() => futuremud.TryGetCharacter(123L, true));
    }

    [TestMethod]
    public void TryGetCharacter_PreNpcBootPhase_ReturnsAlreadyLoadedActor()
    {
        Futuremud futuremud = new(null);
        Mock<ICharacter> actor = new();
        actor.SetupGet(x => x.Id).Returns(123L);
        actor.SetupGet(x => x.Name).Returns("Guard");
        futuremud.Add(actor.Object, true);
        futuremud.SetCharacterMaterialisationBootPhase(CharacterMaterialisationBootPhase.Disallowed);

        ICharacter result = futuremud.TryGetCharacter(123L, false);

        Assert.AreSame(actor.Object, result);
    }

    [TestMethod]
    public void ShouldLoadNpcAtBoot_ActiveStableStay_ReturnsFalse()
    {
        HashSet<long> activeStableMountIds = new() { 123L };

        bool result = Futuremud.ShouldLoadNpcAtBoot(123L, CharacterState.Awake, activeStableMountIds);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ShouldLoadNpcAtBoot_NormalLivingNpc_ReturnsTrue()
    {
        HashSet<long> activeStableMountIds = new() { 456L };

        bool result = Futuremud.ShouldLoadNpcAtBoot(123L, CharacterState.Awake, activeStableMountIds);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void PropertySaleOrder_DbConstructor_MatchesConsentWithoutDereferencingOwner()
    {
        Mock<IFuturemud> gameworld = new();
        Mock<IPropertyOwner> owner = new();
        owner.SetupGet(x => x.OwnerId).Returns(44L);
        owner.SetupGet(x => x.OwnerFrameworkItemType).Returns("Character");
        owner.SetupGet(x => x.Owner).Throws(new AssertFailedException("Owner should not be dereferenced during load."));

        Mock<IProperty> property = new();
        property.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        property.SetupGet(x => x.PropertyOwners).Returns(new[] { owner.Object });

        DbPropertySaleOrder dbitem = new()
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

        PropertySaleOrder order = new(dbitem, gameworld.Object, property.Object);

        Assert.AreEqual(1, order.PropertyOwnerConsent.Count);
        Assert.IsTrue(order.PropertyOwnerConsent[owner.Object]);
    }

    [TestMethod]
    public void PropertyLeaseOrder_DbConstructor_MatchesConsentWithoutDereferencingOwner()
    {
        Mock<IUneditableAll<IFutureProg>> futureProgs = new();
        futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null);

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);

        Mock<IPropertyOwner> owner = new();
        owner.SetupGet(x => x.OwnerId).Returns(55L);
        owner.SetupGet(x => x.OwnerFrameworkItemType).Returns("Character");
        owner.SetupGet(x => x.Owner).Throws(new AssertFailedException("Owner should not be dereferenced during load."));

        Mock<IProperty> property = new();
        property.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        property.SetupGet(x => x.PropertyOwners).Returns(new[] { owner.Object });
        property.SetupGet(x => x.LeaseOrder).Returns((IPropertyLeaseOrder)null);

        DbPropertyLeaseOrder dbitem = new()
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

        PropertyLeaseOrder order = new(dbitem, property.Object);

        Assert.AreEqual(1, order.PropertyOwnerConsent.Count);
        Assert.IsFalse(order.PropertyOwnerConsent[owner.Object]);
    }

    [TestMethod]
    public void AuctionHouse_DbConstructor_DefersCharacterResolutionUntilFinaliseLoading()
    {
        Mock<IEconomicZone> zone = new();
        zone.SetupGet(x => x.Id).Returns(1L);
        zone.SetupGet(x => x.Name).Returns("Central");

        Mock<ICell> cell = new();
        cell.SetupGet(x => x.Id).Returns(2L);
        cell.SetupGet(x => x.Name).Returns("Auction Floor");

        Mock<IBankAccount> profitsAccount = new();
        profitsAccount.SetupGet(x => x.Id).Returns(33L);
        profitsAccount.SetupGet(x => x.Name).Returns("Profits");
        profitsAccount.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");

        Mock<IBankAccount> payoutAccount = new();
        payoutAccount.SetupGet(x => x.Id).Returns(44L);
        payoutAccount.SetupGet(x => x.Name).Returns("Seller Account");
        payoutAccount.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");

        Mock<ICharacter> seller = new();
        seller.SetupGet(x => x.Id).Returns(22L);
        seller.SetupGet(x => x.Name).Returns("Seller");
        seller.SetupGet(x => x.FrameworkItemType).Returns("Character");

        Mock<IGameItem> item = new();
        item.SetupGet(x => x.Id).Returns(11L);
        item.SetupGet(x => x.Name).Returns("Sabre");
        item.SetupGet(x => x.FrameworkItemType).Returns("GameItem");

        Mock<IHeartbeatManager> heartbeatManager = new();
        All<IEconomicZone> economicZones = new();
        economicZones.Add(zone.Object);
        All<ICell> cells = new();
        cells.Add(cell.Object);
        All<IBankAccount> bankAccounts = new();
        bankAccounts.Add(profitsAccount.Object);
        bankAccounts.Add(payoutAccount.Object);
        All<IProperty> properties = new();

        IPostCharacterLoadFinalisable registeredFinalisable = null;
        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager.Object);
        gameworld.SetupGet(x => x.EconomicZones).Returns(economicZones);
        gameworld.SetupGet(x => x.Cells).Returns(cells);
        gameworld.SetupGet(x => x.BankAccounts).Returns(bankAccounts);
        gameworld.SetupGet(x => x.Properties).Returns(properties);
        gameworld.Setup(x => x.TryGetItem(11L, true)).Returns(item.Object);
        gameworld.Setup(x => x.TryGetCharacter(22L, true)).Returns(seller.Object);
        gameworld.Setup(x => x.RegisterPostCharacterLoadFinalisable(It.IsAny<IPostCharacterLoadFinalisable>()))
            .Callback<IPostCharacterLoadFinalisable>(x => registeredFinalisable = x);

        DbAuctionHouse dbitem = new()
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

        AuctionHouse house = new(dbitem, gameworld.Object);

        Assert.AreSame(house, registeredFinalisable);
        Assert.AreEqual(0, house.ActiveAuctionItems.Count());
        gameworld.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);

        house.FinaliseLoading();

        AuctionItem loadedItem = house.ActiveAuctionItems.Single();
        Assert.AreSame(seller.Object, loadedItem.Seller);
        Assert.AreSame(payoutAccount.Object, loadedItem.PayoutTarget);
        gameworld.Verify(x => x.TryGetCharacter(22L, true), Times.Once);
    }
}
