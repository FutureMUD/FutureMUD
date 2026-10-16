#nullable enable
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Shops;
using MudSharp.Effects.Concrete;
using MudSharp.Economy.Payment;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

public partial class ShopTests
{
	private void EnableHaggle() => _gameworld.Setup(x => x.GetStaticBool("HagglingEnabled")).Returns(true);
	private void Terms(Mock<ICharacter> actor, decimal sale = -.10M, decimal buy = .10M)
	{
		var effects = HaggleTestEffects.Bind(actor, _gameworld.Object);
		effects.Add(new HaggleShopPriceEffect(actor.Object, _shop.Id, ShopDealApplicability.Both, sale, buy, DateTime.UtcNow.AddHours(1)));
		_gameworld.Setup(x => x.GetStaticDecimal("HaggleMinimumSpread")).Returns(.05M);
	}

	[TestMethod]
	public void Haggle_BankPurchase_QuotePaymentTaxAndTransactionAgreeWithoutCatalogueMutation()
	{
		EnableHaggle();
		var tax = new Mock<ISalesTax>();
		tax.Setup(x => x.Applies(It.IsAny<IMerchandise>(), It.IsAny<ICharacter>())).Returns(true);
		tax.Setup(x => x.TaxValue(It.IsAny<IMerchandise>(), It.IsAny<ICharacter>(), It.IsAny<decimal>()))
			.Returns<IMerchandise, ICharacter, decimal>((_, _, value) => value * .2M);
		_salesTaxes.Add(tax.Object);
		var proto = RegisterPrototype(600);
		var merch = new Merchandise(_shop, "haggled goods", proto.Object, 100M, false, null, null);
		_shop.AddMerchandise(merch);
		CreateDeal("sale", ShopDealType.Sale, ShopDealTargetType.Merchandise, -.2M, merchandiseId: merch.Id);
		var bank = CreateBankPaymentActor(); Terms(bank.Actor);
		bank.PlayerAccount.Setup(x => x.MaximumWithdrawal()).Returns(1000M);
		var item = CreateStackedItem(601, 2, proto.Object);
		_shop.AddToStock(null, item.Object, merch);
		var calculation = _shop.GetPriceCalculation(bank.Actor.Object, merch, 2);
		Assert.AreEqual(144M, calculation.TotalPretaxPrice); Assert.AreEqual(28M, calculation.IncludedTax);
		Assert.AreEqual(172M, calculation.TotalPrice);
		Assert.AreEqual(172M, _shop.PreviewBuy(bank.Actor.Object, merch, 2, bank.Payment).Price);
		Assert.IsTrue(_shop.CanBuy(bank.Actor.Object, merch, 2, bank.Payment).Truth);
		_shop.Buy(bank.Actor.Object, merch, 2, bank.Payment).ToList();
		var transaction = _shop.TransactionRecords.Last();
		Assert.AreEqual(144M, transaction.PretaxValue); Assert.AreEqual(28M, transaction.Tax);
		bank.PlayerAccount.Verify(x => x.WithdrawFromTransaction(172M, It.IsAny<string>()), Times.Once);
		bank.ShopAccount.Verify(x => x.DepositFromTransaction(172M, It.IsAny<string>()), Times.Once);
		_zone.Verify(x => x.ReportSalesTaxCollected(_shop, 28M), Times.Once);
		Assert.AreEqual(100M, merch.EffectivePrice);
	}

	[TestMethod]
	public void Haggle_BuybackCeiling_AccountsForOverlappingVolumeDealsAndBaseBuyModifier()
	{
		EnableHaggle();
		var proto = RegisterPrototype(610);
		var merch = new Merchandise(_shop, "spread goods", proto.Object, 100M, false, null, null);
		_shop.AddMerchandise(merch);
		CreateDeal("cumulative", ShopDealType.Sale, ShopDealTargetType.Merchandise, -.2M, isCumulative: true, merchandiseId: merch.Id);
		CreateDeal("bulk cumulative", ShopDealType.Volume, ShopDealTargetType.Merchandise, -.1M, isCumulative: true, merchandiseId: merch.Id, minimumQuantity: 10);
		CreateDeal("bulk best", ShopDealType.Volume, ShopDealTargetType.Merchandise, -.4M, merchandiseId: merch.Id, minimumQuantity: 10);
		var bank = CreateBankPaymentActor(); Terms(bank.Actor);
		var buyback = _shop.GetPriceCalculation(bank.Actor.Object, merch, 1, ShopDealApplicability.Buy);
		// Lowest resale is 100 * (1 - .2 - .1 - .4) * .9 = 27, then 5% spread.
		Assert.AreEqual(25M, buyback.TotalPrice);
		Assert.IsTrue(buyback.AdjustedUnitPrice <= 25.65M);
		var item = CreateStackedItem(611, 1, proto.Object);
		var payment = new Mock<IPaymentMethod>();
		_shop.Sell(bank.Actor.Object, merch, payment.Object, item.Object);
		payment.Verify(x => x.GivePayment(25M), Times.Once);
		Assert.AreEqual(-25M, _shop.TransactionRecords.Last().PretaxValue);
	}

	[TestMethod]
	public void Haggle_ExclusionsAndOptOut_LeaveNormalPricingAndTermsIntact()
	{
		EnableHaggle();
		var proto = RegisterPrototype(620);
		var merch = new Merchandise(_shop, "excluded goods", proto.Object, 100M, false, null, null);
		_shop.AddMerchandise(merch);
		var bank = CreateBankPaymentActor(); Terms(bank.Actor);
		Assert.AreEqual(90M, _shop.PriceForMerchandise(bank.Actor.Object, merch, 1));
		_shop.Haggling.ExcludedMerchandise.Add(merch.Id);
		Assert.AreEqual(100M, _shop.PriceForMerchandise(bank.Actor.Object, merch, 1));
		_shop.Haggling.ExcludedMerchandise.Clear(); _shop.Haggling.Disabled = true;
		Assert.AreEqual(100M, _shop.PriceForMerchandise(bank.Actor.Object, merch, 1));
		_shop.Haggling.Disabled = false;
		Assert.AreEqual(90M, _shop.PriceForMerchandise(bank.Actor.Object, merch, 1));
	}

	[TestMethod]
	public void Haggle_CommodityBuyback_UsesActualFractionalWeightForPayoutAndRecord()
	{
		EnableHaggle();
		var material = RegisterMaterial(640, "test metal");
		var proto = RegisterPrototype(641);
		var merchandise = new Merchandise(_shop, "metal", proto.Object, material.Object, null, 100M, 1.0, false, null, null);
		_shop.AddMerchandise(merchandise);
		var item = CreateCommodityItem(642, proto.Object, material.Object, .01);
		var actor = CreateBankPaymentActor().Actor; Terms(actor);
		var payment = new Mock<IPaymentMethod>();
		var purchaseCost = _shop.PriceForMerchandiseWeight(actor.Object, merchandise, .01);
		_shop.Sell(actor.Object, merchandise, payment.Object, item.Object);
		var record = _shop.TransactionRecords.Last();
		Assert.IsTrue(-record.PretaxValue < purchaseCost);
		Assert.IsTrue(-record.PretaxValue < 1M);
		payment.Verify(x => x.GivePayment(-record.PretaxValue), Times.Once);
		Assert.AreEqual(0M, record.Tax);
	}

	[TestMethod]
	public void Haggle_CommodityCredit_QuoteUsesOwnerDealsAndActualCustomerTerms()
	{
		EnableHaggle();
		var material = RegisterMaterial(650, "credit metal");
		var proto = RegisterPrototype(651);
		var merchandise = new Merchandise(_shop, "credit metal", proto.Object, material.Object, null, 100M, 1.0, false, null, null);
		_shop.AddMerchandise(merchandise);
		var customer = CreateBankPaymentActor().Actor; Terms(customer, -.10M);
		var owner = new Mock<ICharacter>(); Terms(owner, .20M);
		_gameworld.Setup(x => x.TryGetCharacter(999, true)).Returns(owner.Object);
		var deal = new Mock<IShopDeal>();
		deal.SetupGet(x => x.IsCumulative).Returns(true);
		deal.SetupGet(x => x.PriceAdjustmentPercentage).Returns(-.50M);
		deal.Setup(x => x.Applies(merchandise, owner.Object, It.IsAny<int>(), ShopDealApplicability.Sell, It.IsAny<MudSharp.TimeAndDate.MudDateTime>())).Returns(true);
		_shop.AddDeal(deal.Object);
		var account = new Mock<ILineOfCreditAccount>();
		account.SetupGet(x => x.AccountOwnerId).Returns(999);
		var payment = new LineOfCreditPayment(customer.Object, account.Object);
		Assert.AreEqual(22.5M, _shop.PriceForMerchandiseWeight(customer.Object, merchandise, .5, payment));
		Assert.AreEqual(45M, _shop.PriceForMerchandiseWeight(customer.Object, merchandise, .5));
	}

	[TestMethod]
	public void Haggle_CreditDealOwner_UsesActualCustomersTerms()
	{
		EnableHaggle();
		var proto = RegisterPrototype(630);
		var merch = new Merchandise(_shop, "credit goods", proto.Object, 100M, false, null, null);
		_shop.AddMerchandise(merch);
		var customer = CreateBankPaymentActor().Actor; Terms(customer, -.10M);
		var owner = new Mock<ICharacter>(); Terms(owner, .20M);
		Assert.AreEqual(90M, _shop.GetPriceCalculation(owner.Object, merch, 1, negotiatingCustomer: customer.Object).TotalPrice);
		Assert.AreEqual(120M, _shop.GetPriceCalculation(owner.Object, merch, 1).TotalPrice);
	}
}
