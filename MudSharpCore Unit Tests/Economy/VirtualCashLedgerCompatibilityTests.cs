#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
[DoNotParallelize]
public class VirtualCashLedgerCompatibilityTests
{
	[TestInitialize]
	public void Setup()
	{
		Assert.IsTrue(string.IsNullOrWhiteSpace(FMDB.ConnectionString));
		Assert.IsNull(FMDB.Connection);
		VirtualCashLedger.ClearInMemoryForTests();
	}

	[TestCleanup]
	public void Cleanup() => VirtualCashLedger.ClearInMemoryForTests();

	[TestMethod]
	public void BankAccount_OriginalWithdrawalContract_RemainsUnchanged()
	{
		var methods = typeof(IBankAccount).GetMethods().Where(x => x.Name == "WithdrawFromTransaction").ToArray();
		Assert.AreEqual(1, methods.Length);
		CollectionAssert.AreEqual(new[] { typeof(decimal), typeof(string) },
			methods.Single().GetParameters().Select(x => x.ParameterType).ToArray());
	}

	[TestMethod]
	public void Debit_OriginalClrSignatureAndOptionalDefaults_AreAvailable()
	{
		var method = typeof(VirtualCashLedger).GetMethod("Debit", new[]
		{
			typeof(IFrameworkItem), typeof(ICurrency), typeof(decimal), typeof(ICharacter), typeof(IFrameworkItem),
			typeof(string), typeof(string), typeof(IBankAccount), typeof(MudDateTime), typeof(string).MakeByRefType(),
			typeof(IFrameworkItem), typeof(string)
		});
		Assert.IsNotNull(method, "The original twelve-parameter CLR method must remain callable.");
		Assert.AreEqual(typeof(bool), method.ReturnType);
		Assert.IsTrue(method.GetParameters()[9].IsOut);
		foreach (var parameter in method.GetParameters().Skip(10))
		{
			Assert.IsTrue(parameter.IsOptional);
			Assert.IsNull(parameter.DefaultValue);
		}
	}

	[TestMethod]
	public void Debit_OriginalExplicitCaller_PreservesLegacyBankAndReferenceFields()
	{
		var (owner, currency, bank, account) = CreateFixture();
		Assert.IsFalse(account.Object is IPreparedBankAccountWithdrawal);
		var reference = new Mock<IFrameworkItem>();
		reference.SetupGet(x => x.Id).Returns(8L);
		reference.SetupGet(x => x.FrameworkItemType).Returns("Order");
		VirtualCashLedger.Credit(owner.Object, currency.Object, 5M, null, null, "Cash", "opening");
		LegacyDebitCall debit = VirtualCashLedger.Debit;
		Assert.IsTrue(debit(owner.Object, currency.Object, 10M, null, null, "Cash", "legacy reason", account.Object,
			null, out var error, reference.Object, "legacy reference"), error);
		account.Verify(x => x.WithdrawFromTransaction(5M, "legacy reason"), Times.Once);
		Assert.AreEqual(0M, VirtualCashLedger.Balance(owner.Object, currency.Object));
		Assert.AreEqual(95M, bank.Object.CurrencyReserves[currency.Object]);
		var entry = VirtualCashLedger.LedgerEntries(owner.Object).Single(x => x.Amount < 0M);
		Assert.AreEqual("Order", entry.ReferenceType);
		Assert.AreEqual(8L, entry.ReferenceId);
		Assert.AreEqual("legacy reference", entry.Reference);
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(5)]
	public void Debit_PreparedDescriptionToLegacyBank_RejectsBeforeMutation(int virtualBalance)
	{
		var (owner, currency, bank, account) = CreateFixture();
		if (virtualBalance > 0) VirtualCashLedger.Credit(owner.Object, currency.Object, virtualBalance, null, null, "Cash", "opening");
		Assert.IsFalse(VirtualCashLedger.Debit(owner.Object, currency.Object, 10M, null, null, "Cash", "reason",
			account.Object, null, out var error, null, null, "validated description"));
		Assert.IsTrue(error.Contains("prepared transaction description"));
		Assert.AreEqual((decimal)virtualBalance, VirtualCashLedger.Balance(owner.Object, currency.Object));
		Assert.IsFalse(VirtualCashLedger.LedgerEntries(owner.Object).Any(x => x.Amount < 0M));
		Assert.AreEqual(100M, bank.Object.CurrencyReserves[currency.Object]);
		Assert.IsFalse(bank.Object.Changed);
		account.Verify(x => x.WithdrawFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void Debit_OptionalPreparedCapability_ForwardsExactDescription()
	{
		var (owner, currency, bank, account) = CreateFixture();
		var prepared = account.As<IPreparedBankAccountWithdrawal>();
		VirtualCashLedger.Credit(owner.Object, currency.Object, 5M, null, null, "Cash", "opening");
		Assert.IsTrue(VirtualCashLedger.Debit(owner.Object, currency.Object, 10M, null, null, "Cash", "reason",
			account.Object, null, out var error, null, null, "validated description"), error);
		prepared.Verify(x => x.WithdrawFromTransaction(5M, "reason", "validated description"), Times.Once);
		account.Verify(x => x.WithdrawFromTransaction(It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
		Assert.AreEqual(0M, VirtualCashLedger.Balance(owner.Object, currency.Object));
		Assert.AreEqual(95M, bank.Object.CurrencyReserves[currency.Object]);
	}

	private static (Mock<IFrameworkItem> Owner, Mock<ICurrency> Currency, Mock<IBank> Bank, Mock<IBankAccount> Account) CreateFixture()
	{
		var owner = new Mock<IFrameworkItem>();
		owner.SetupGet(x => x.Id).Returns(42L);
		owner.SetupGet(x => x.FrameworkItemType).Returns("Clan");
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(9L);
		var bank = new Mock<IBank>();
		var reserves = new DecimalCounter<ICurrency>();
		reserves[currency.Object] = 100M;
		bank.SetupGet(x => x.CurrencyReserves).Returns(reserves);
		bank.SetupProperty(x => x.Changed);
		var account = new Mock<IBankAccount>();
		account.SetupGet(x => x.Id).Returns(12L);
		account.SetupGet(x => x.Currency).Returns(currency.Object);
		account.SetupGet(x => x.Bank).Returns(bank.Object);
		account.Setup(x => x.CanWithdraw(It.IsAny<decimal>(), false)).Returns((true, string.Empty));
		return (owner, currency, bank, account);
	}

	private delegate bool LegacyDebitCall(IFrameworkItem owner, ICurrency currency, decimal amount,
		ICharacter? actor, IFrameworkItem? counterparty, string destinationKind, string reason,
		IBankAccount? bankAccount, MudDateTime? mudDateTime, out string error, IFrameworkItem? reference,
		string? referenceText);
}
