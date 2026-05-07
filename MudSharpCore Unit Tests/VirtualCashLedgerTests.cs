using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VirtualCashLedgerTests
{
	[TestInitialize]
	public void Setup()
	{
		VirtualCashLedger.ClearInMemoryForTests();
	}

	[TestMethod]
	public void Credit_AddsBalanceAndLedgerEntry()
	{
		var owner = CreateOwner(1L, "AuctionHouse");
		var currency = CreateCurrency();

		var balance = VirtualCashLedger.Credit(owner.Object, currency.Object, 50.0M, null, null, "Cash", "cash deposit");

		Assert.AreEqual(50.0M, balance);
		Assert.AreEqual(50.0M, VirtualCashLedger.Balance(owner.Object, currency.Object));
		var entry = VirtualCashLedger.LedgerEntries(owner.Object).Single();
		Assert.AreEqual(50.0M, entry.Amount);
		Assert.AreEqual(50.0M, entry.BalanceAfter);
		Assert.AreEqual("Cash", entry.SourceKind);
		Assert.AreEqual("VirtualCash", entry.DestinationKind);
	}

	[TestMethod]
	public void Debit_UsesVirtualCashBeforeLinkedBank()
	{
		var owner = CreateOwner(2L, "Stable");
		var currency = CreateCurrency();
		var bank = CreateBank(currency.Object);
		var account = CreateBankAccount(currency.Object, bank.Object);
		account.Setup(x => x.CanWithdraw(25.0M, false)).Returns((true, string.Empty));

		VirtualCashLedger.Credit(owner.Object, currency.Object, 50.0M, null, null, "Cash", "opening cash");

		var result = VirtualCashLedger.Debit(owner.Object, currency.Object, 75.0M, null, null, "Payout",
			"mixed payout", account.Object, null, out var error);

		Assert.IsTrue(result, error);
		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(owner.Object, currency.Object));
		account.Verify(x => x.WithdrawFromTransaction(25.0M, "mixed payout"), Times.Once);
		Assert.AreEqual(-25.0M, bank.Object.CurrencyReserves[currency.Object]);
		var debit = VirtualCashLedger.LedgerEntries(owner.Object).Single(x => x.Amount < 0.0M);
		Assert.AreEqual(-75.0M, debit.Amount);
		Assert.AreEqual("VirtualCash+Bank", debit.SourceKind);
		Assert.AreEqual("Payout", debit.DestinationKind);
	}

	[TestMethod]
	public void Debit_FailsWhenVirtualCashAndBankAreInsufficient()
	{
		var owner = CreateOwner(3L, "Hotel");
		var currency = CreateCurrency();

		var result = VirtualCashLedger.Debit(owner.Object, currency.Object, 10.0M, null, null, "Refund",
			"refund", null, null, out var error);

		Assert.IsFalse(result);
		Assert.IsFalse(string.IsNullOrWhiteSpace(error));
		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(owner.Object, currency.Object));
		Assert.IsFalse(VirtualCashLedger.LedgerEntries(owner.Object).Any());
	}

	[TestMethod]
	public void CreditBankOrVirtual_UsesBankWhenValidAndRecordsDomainLedger()
	{
		var owner = CreateOwner(4L, "LegalAuthority");
		var currency = CreateCurrency();
		var bank = CreateBank(currency.Object);
		var account = CreateBankAccount(currency.Object, bank.Object);

		VirtualCashLedger.CreditBankOrVirtual(owner.Object, currency.Object, 20.0M, null, null, "Fine",
			"fine payment", account.Object, null);

		account.Verify(x => x.DepositFromTransaction(20.0M, "fine payment"), Times.Once);
		Assert.AreEqual(20.0M, bank.Object.CurrencyReserves[currency.Object]);
		Assert.AreEqual(0.0M, VirtualCashLedger.Balance(owner.Object, currency.Object));
		var entry = VirtualCashLedger.LedgerEntries(owner.Object).Single();
		Assert.AreEqual(20.0M, entry.Amount);
		Assert.AreEqual("Fine", entry.SourceKind);
		Assert.AreEqual("Bank", entry.DestinationKind);
		Assert.AreEqual(account.Object.Id, entry.LinkedBankAccountId);
	}

	private static Mock<IFrameworkItem> CreateOwner(long id, string type)
	{
		var owner = new Mock<IFrameworkItem>();
		owner.SetupGet(x => x.Id).Returns(id);
		owner.SetupGet(x => x.Name).Returns($"{type} {id}");
		owner.SetupGet(x => x.FrameworkItemType).Returns(type);
		return owner;
	}

	private static Mock<ICurrency> CreateCurrency()
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(10L);
		currency.SetupGet(x => x.Name).Returns("dollars");
		currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
		        .Returns<decimal, CurrencyDescriptionPatternType>((value, _) => value.ToString("N2"));
		return currency;
	}

	private static Mock<IBank> CreateBank(ICurrency currency)
	{
		var bank = new Mock<IBank>();
		bank.SetupGet(x => x.CurrencyReserves).Returns(new DecimalCounter<ICurrency>());
		bank.SetupProperty(x => x.Changed);
		return bank;
	}

	private static Mock<IBankAccount> CreateBankAccount(ICurrency currency, IBank bank)
	{
		var account = new Mock<IBankAccount>();
		account.SetupGet(x => x.Id).Returns(20L);
		account.SetupGet(x => x.Name).Returns("Settlement");
		account.SetupGet(x => x.FrameworkItemType).Returns("BankAccount");
		account.SetupGet(x => x.Currency).Returns(currency);
		account.SetupGet(x => x.Bank).Returns(bank);
		account.Setup(x => x.MaximumWithdrawal()).Returns(100.0M);
		return account;
	}
}
