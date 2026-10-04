#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;

namespace MudSharp_Unit_Tests.Community;

[TestClass]
[DoNotParallelize]
public class ClanBudgetLengthTests
{
	[DataTestMethod]
	[DataRow(200, true)]
	[DataRow(201, false)]
	public void BudgetCreate_NameBoundary_ValidatesBeforePersistence(int length, bool accepted)
	{
		using var fixture = new BudgetFixture();
		var name = new string('n', length);
		fixture.Clan.BudgetCommand(fixture.Actor.Object, new StringStack($"create office 10 \"every day\" {name}"));
		Assert.AreEqual(accepted ? 1 : 0, fixture.Clan.Budgets.Count);
		Assert.AreEqual(accepted ? 1 : 0, fixture.Context.ClanBudgets.Count());
		if (accepted) Assert.AreEqual(name, fixture.Clan.Budgets.Single().Name);
		else Assert.IsTrue(fixture.Messages.Single().Contains("200"));
	}

	[TestMethod]
	public void BudgetConstructor_OversizedName_RejectsBeforePersistence()
	{
		using var fixture = new BudgetFixture();
		Assert.ThrowsException<ArgumentException>(() => new ClanBudget(fixture.Clan, fixture.Appointment.Object,
			null, fixture.Currency.Object, new string('n', 201), 10M,
			new RecurringInterval { Type = IntervalType.Daily, IntervalAmount = 1 }));
		Assert.AreEqual(0, fixture.Context.ClanBudgets.Count());
	}

	[DataTestMethod]
	[DataRow(false, 1000, true)]
	[DataRow(false, 1001, false)]
	[DataRow(true, 1000, true)]
	[DataRow(true, 1001, false)]
	public void BudgetDraw_FullyVirtualReasonBoundary_PreservesFullAuditReason(bool hasBank, int length, bool accepted)
	{
		using var fixture = new BudgetFixture();
		fixture.AddBudget(new string('n', 200), hasBank, 10M);
		var reason = new string('r', length);
		fixture.Draw(reason);
		if (!accepted) { fixture.AssertRejectedWithoutMutation(); return; }
		Assert.AreEqual(reason, fixture.Context.ClanBudgetTransactions.Single().Reason);
		Assert.AreEqual(0M, VirtualCashLedger.Balance(fixture.Clan, fixture.Currency.Object));
		Assert.AreEqual(0, fixture.BankTransactions.Count);
		fixture.Budget.Verify(x => x.AddDrawdown(It.IsAny<IClanBudgetTransaction>()), Times.Once);
		Assert.AreEqual(1, fixture.Context.ClanBudgetTransactions.Count());
	}

	[DataTestMethod]
	[DataRow(0, 255, false)]
	[DataRow(0, 256, false)]
	[DataRow(5, 255, false)]
	[DataRow(5, 256, false)]
	[DataRow(0, 255, true)]
	[DataRow(0, 256, true)]
	[DataRow(5, 255, true)]
	[DataRow(5, 256, true)]
	public void BudgetDraw_CompleteBankDescriptionBoundary_TruncatesAndPreservesFullAuditReason(
		int virtualBalance, int descriptionLength, bool databaseLedger)
	{
		using var fixture = new BudgetFixture();
		fixture.UseDatabaseLedger(databaseLedger);
		fixture.AddBudget("office budget", true, virtualBalance);
		var bankAmount = 10M - virtualBalance;
		var prefix = $"Withdraw {fixture.AmountDescription(bankAmount)} from transaction - Clan budget office budget: ";
		var reason = new string('r', descriptionLength - prefix.Length);
		fixture.Draw(reason);
		var expected = (prefix + reason)[..Math.Min(descriptionLength, 255)];
		Assert.AreEqual(expected, fixture.BankTransactions.Single().TransactionDescription);
		Assert.AreEqual(expected, fixture.PersistBankTransaction().TransactionDescription);
		Assert.AreEqual(1000M - bankAmount, fixture.BankAccount.CurrentBalance);
		Assert.AreEqual(0M, VirtualCashLedger.Balance(fixture.Clan, fixture.Currency.Object));
		Assert.AreEqual(reason, fixture.Context.ClanBudgetTransactions.Single().Reason);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void BudgetDraw_ChangingCurrencyFormatting_FormatsOnceAndPersistsTheExactDescription(bool databaseLedger)
	{
		using var fixture = new BudgetFixture();
		fixture.UseDatabaseLedger(databaseLedger);
		var format = fixture.AmountDescription;
		var bankFormatCount = 0;
		fixture.AmountDescription = amount => amount == 5M && ++bankFormatCount > 1 ? new string('c', 300) : format(amount);
		fixture.AddBudget("office budget", true, 5M);
		fixture.Draw("supplies");
		Assert.AreEqual("Withdraw 5.00 coins from transaction - Clan budget office budget: supplies",
			fixture.BankTransactions.Single().TransactionDescription);
		Assert.AreEqual(1, bankFormatCount);
		Assert.AreEqual(995M, fixture.BankAccount.CurrentBalance);
		Assert.AreEqual(0M, VirtualCashLedger.Balance(fixture.Clan, fixture.Currency.Object));
		Assert.AreEqual("supplies", fixture.Context.ClanBudgetTransactions.Single().Reason);
		Assert.AreEqual(fixture.BankTransactions.Single().TransactionDescription,
			fixture.PersistBankTransaction().TransactionDescription);
	}

	[DataTestMethod]
	[DataRow(5)]
	[DataRow(10)]
	public void BudgetDraw_OriginalBankContract_SupportsBankAndVirtualFunding(int virtualBalance)
	{
		using var fixture = new BudgetFixture();
		fixture.AddBudget("office budget", true, virtualBalance);
		var account = new Mock<IBankAccount>();
		account.SetupGet(x => x.Id).Returns(12L);
		account.SetupGet(x => x.Currency).Returns(fixture.Currency.Object);
		account.SetupGet(x => x.Bank).Returns(fixture.BankAccount.Bank);
		account.Setup(x => x.CanWithdraw(It.IsAny<decimal>(), false)).Returns((true, string.Empty));
		fixture.Budget.SetupGet(x => x.BankAccount).Returns(account.Object);
		var reason = new string('r', 1000);
		fixture.Draw(reason);
		Assert.AreEqual(reason, fixture.Context.ClanBudgetTransactions.Single().Reason);
		Assert.AreEqual(0M, VirtualCashLedger.Balance(fixture.Clan, fixture.Currency.Object));
		account.Verify(x => x.WithdrawFromTransaction(10M - virtualBalance, "Clan budget office budget: " + reason),
			virtualBalance < 10 ? Times.Once() : Times.Never());
	}

	[TestMethod]
	public void BudgetDraw_LongCurrencyDescriptionAndBudgetName_TruncatesOnlyTheBankDescription()
	{
		using var fixture = new BudgetFixture();
		fixture.AmountDescription = _ => new string('c', 230);
		fixture.AddBudget(new string('n', 200), true, 5M);
		fixture.Draw("x");
		Assert.AreEqual(255, fixture.BankTransactions.Single().TransactionDescription.Length);
		Assert.AreEqual("Withdraw " + new string('c', 230) + " from transactio",
			fixture.PersistBankTransaction().TransactionDescription);
		Assert.AreEqual("x", fixture.Context.ClanBudgetTransactions.Single().Reason);
		Assert.AreEqual(995M, fixture.BankAccount.CurrentBalance);
		Assert.AreEqual(0M, VirtualCashLedger.Balance(fixture.Clan, fixture.Currency.Object));
	}

	[DataTestMethod]
	[DataRow(0, 1000, true, false)]
	[DataRow(0, 1001, false, false)]
	[DataRow(5, 1000, true, false)]
	[DataRow(5, 1001, false, false)]
	[DataRow(0, 1000, true, true)]
	[DataRow(0, 1001, false, true)]
	[DataRow(5, 1000, true, true)]
	[DataRow(5, 1001, false, true)]
	public void BudgetDraw_BankFundedReasonBoundary_PreservesFullReasonOrRejectsBeforeMutation(
		int virtualBalance, int reasonLength, bool accepted, bool databaseLedger)
	{
		using var fixture = new BudgetFixture();
		fixture.UseDatabaseLedger(databaseLedger);
		fixture.AddBudget(new string('n', 200), true, virtualBalance);
		var reason = new string('r', reasonLength);
		fixture.Draw(reason);
		if (!accepted) { fixture.AssertRejectedWithoutMutation(); return; }
		Assert.AreEqual(reason, fixture.Context.ClanBudgetTransactions.Single().Reason);
		Assert.AreEqual("Clan budget " + new string('n', 200) + ": " + reason,
			VirtualCashLedger.LedgerEntries(fixture.Clan).Single(x => x.Amount < 0M).Reason);
		Assert.AreEqual(255, fixture.PersistBankTransaction().TransactionDescription.Length);
		Assert.AreEqual(1000M - (10M - virtualBalance), fixture.BankAccount.CurrentBalance);
		Assert.AreEqual(0M, VirtualCashLedger.Balance(fixture.Clan, fixture.Currency.Object));
	}

	[DataTestMethod]
	[DataRow("\U0001F600")]
	[DataRow("e\u0301")]
	[DataRow("\U0001F469\u200D\U0001F4BB")]
	public void BudgetDraw_UnicodeAtDescriptionBoundary_KeepsWholeTextElements(string textElement)
	{
		using var fixture = new BudgetFixture();
		fixture.AddBudget("office budget", true, 5M);
		var prefix = $"Withdraw {fixture.AmountDescription(5M)} from transaction - Clan budget office budget: ";
		var padding = new string('r', 254 - prefix.Length);
		var reason = padding + textElement + "tail";
		fixture.Draw(reason);
		Assert.AreEqual(prefix + padding, fixture.BankTransactions.Single().TransactionDescription);
		Assert.AreEqual(prefix + padding, fixture.PersistBankTransaction().TransactionDescription);
		Assert.AreEqual(reason, fixture.Context.ClanBudgetTransactions.Single().Reason);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void BudgetDraw_FormattingFailure_LeavesFundsLedgersAndPayoutUntouched(bool databaseLedger)
	{
		using var fixture = new BudgetFixture();
		fixture.UseDatabaseLedger(databaseLedger);
		fixture.AddBudget("office budget", true, 5M);
		fixture.AmountDescription = _ => throw new InvalidOperationException("format failed");
		Assert.ThrowsException<InvalidOperationException>(() => fixture.Draw("supplies"));
		Assert.AreEqual(5M, VirtualCashLedger.Balance(fixture.Clan, fixture.Currency.Object));
		Assert.AreEqual(1000M, fixture.BankAccount.CurrentBalance);
		Assert.AreEqual(0, fixture.BankTransactions.Count);
		Assert.AreEqual(0, fixture.Context.ClanBudgetTransactions.Count());
		Assert.IsFalse(VirtualCashLedger.LedgerEntries(fixture.Clan).Any(x => x.Amount < 0M));
		fixture.Budget.Verify(x => x.AddDrawdown(It.IsAny<IClanBudgetTransaction>()), Times.Never);
		fixture.AssertNoPayout();
		Assert.IsFalse(fixture.BankAccount.Changed);
	}

	[DataTestMethod]
	[DataRow(255)]
	[DataRow(256)]
	public void BankWithdrawal_OriginalTwoArgumentMethod_PersistsTheBoundedDescription(int descriptionLength)
	{
		using var fixture = new BudgetFixture();
		var prefix = $"Withdraw {fixture.AmountDescription(5M)} from transaction - ";
		var reference = new string('r', descriptionLength - prefix.Length);
		fixture.BankAccount.WithdrawFromTransaction(5M, reference);
		var expected = (prefix + reference)[..Math.Min(descriptionLength, 255)];
		Assert.AreEqual(expected, fixture.BankTransactions.Single().TransactionDescription);
		Assert.AreEqual(expected, fixture.PersistBankTransaction().TransactionDescription);
		Assert.AreEqual(995M, fixture.BankAccount.CurrentBalance);
		fixture.Currency.Verify(x => x.Describe(5M, CurrencyDescriptionPatternType.ShortDecimal), Times.Once);
	}

	[TestMethod]
	public void BudgetTransactionConstructor_OversizedReason_RejectsBeforePersistence()
	{
		using var fixture = new BudgetFixture();
		fixture.AddBudget("office budget", false, 10M);
		Assert.ThrowsException<ArgumentException>(() => new ClanBudgetTransaction(
			fixture.Budget.Object, fixture.Actor.Object, 10M, new string('r', 1001)));
		Assert.AreEqual(0, fixture.Context.ClanBudgetTransactions.Count());
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void BudgetDraw_ExistingCreatorAppointmentAndAdministratorPermissions_StillAllowOrdinaryDraw(int permission)
	{
		using var fixture = new BudgetFixture(permission);
		fixture.AddBudget("office budget", true, 5M);
		fixture.Draw("ordinary supplies");
		Assert.AreEqual(1, fixture.Context.ClanBudgetTransactions.Count());
		Assert.AreEqual(1, fixture.BankTransactions.Count);
		Assert.IsTrue(fixture.Messages.Single().StartsWith("You draw "));
	}

	[TestMethod]
	public void BudgetDraw_UnauthorisedActor_RemainsRejectedWithoutMutation()
	{
		using var fixture = new BudgetFixture(3);
		fixture.AddBudget("office budget", true, 5M);
		fixture.Draw("ordinary supplies");
		fixture.AssertRejectedWithoutMutation();
		Assert.IsTrue(fixture.Messages.Single().Contains("not allowed"));
	}

	[TestMethod]
	public void CurrentModel_ConfirmsAllInputAndComposedDescriptionLimits()
	{
		using var fixture = new BudgetFixture();
		Assert.AreEqual("varchar(200)", fixture.Context.Model.FindEntityType(typeof(MudSharp.Models.ClanBudget))!
			.FindProperty("Name")!.FindAnnotation("Relational:ColumnType")!.Value);
		Assert.AreEqual("varchar(1000)", fixture.Context.Model.FindEntityType(typeof(MudSharp.Models.ClanBudgetTransaction))!
			.FindProperty("Reason")!.FindAnnotation("Relational:ColumnType")!.Value);
		Assert.AreEqual("varchar(255)", fixture.Context.Model.FindEntityType(typeof(MudSharp.Models.BankAccountTransaction))!
			.FindProperty("TransactionDescription")!.FindAnnotation("Relational:ColumnType")!.Value);
		Assert.AreEqual("mediumtext", fixture.Context.Model.FindEntityType(typeof(MudSharp.Models.VirtualCashLedgerEntry))!
			.FindProperty("Reason")!.FindAnnotation("Relational:ColumnType")!.Value);
	}

	private sealed class BudgetFixture : IDisposable
	{
		private readonly object _ambient;
		private readonly PropertyInfo _ambientValue;
		private readonly object? _previousSession;
		private readonly string _previousConnectionString;
		private readonly IGameItemProto? _oldCurrencyPrototype;
		private decimal _openingVirtualBalance;
		public FuturemudDatabaseContext Context { get; }
		public Clan Clan { get; } = TestObjectFactory.CreateUninitialized<Clan>();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IAppointment> Appointment { get; } = new();
		public Mock<ICurrency> Currency { get; } = new();
		public Mock<IClanBudget> Budget { get; } = new();
		public BankAccount BankAccount { get; } = TestObjectFactory.CreateUninitialized<BankAccount>();
		public List<IBankAccountTransaction> BankTransactions { get; } = [];
		public List<string> Messages { get; } = [];
		public Func<decimal, string> AmountDescription { get; set; } = amount => amount.ToString("0.00", CultureInfo.InvariantCulture) + " coins";
		private readonly Mock<IGameItem> _payout = new();
		private readonly Mock<IFuturemud> _gameworld = new();

		public BudgetFixture(int permission = 0)
		{
			Assert.IsTrue(string.IsNullOrWhiteSpace(FMDB.ConnectionString), "This fixture requires an explicitly in-memory run.");
			_previousConnectionString = FMDB.ConnectionString;
			Context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase("percival-clan-budget-" + Guid.NewGuid()).Options);
			_ambient = typeof(FMDB).GetField("_ambientSession", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
			_ambientValue = _ambient.GetType().GetProperty("Value")!;
			_previousSession = _ambientValue.GetValue(_ambient);
			var sessionType = typeof(FMDB).GetNestedType("DatabaseSession", BindingFlags.NonPublic)!;
			var session = Activator.CreateInstance(sessionType, true)!;
			sessionType.GetProperty("Context")!.SetValue(session, Context);
			_ambientValue.SetValue(_ambient, session);
			Assert.IsTrue(FMDB.IsIsolated);
			Assert.AreSame(Context, FMDB.Context);
			Assert.IsNull(FMDB.Connection, "This fixture owns an in-memory session without a connection.");
			VirtualCashLedger.ClearInMemoryForTests();
			_gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			SetGameworld(Clan, _gameworld.Object);
			SetGameworld(BankAccount, _gameworld.Object);
			Clan.Id = 42L;
			Clan.FullName = "Test Clan";
			typeof(Clan).GetField("_budgets", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(Clan, new List<IClanBudget>());
			typeof(Clan).GetField("<Appointments>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(Clan, new List<IAppointment> { Appointment.Object });
			var clocks = new All<IClock>();
			_gameworld.SetupGet(x => x.Clocks).Returns(clocks);
			var clock = new Clock(XElement.Parse(@"<Clock><Alias>UTC</Alias><Description>UTC</Description><ShortDisplayString>$j:$m:$s $i</ShortDisplayString><SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString><LongDisplayString>$c $i</LongDisplayString><SecondsPerMinute>60</SecondsPerMinute><MinutesPerHour>60</MinutesPerHour><HoursPerDay>24</HoursPerDay><InGameSecondsPerRealSecond>1</InGameSecondsPerRealSecond><SecondFixedDigits>2</SecondFixedDigits><MinuteFixedDigits>2</MinuteFixedDigits><HourFixedDigits>0</HourFixedDigits><NoZeroHour>true</NoZeroHour><NumberOfHourIntervals>2</NumberOfHourIntervals><HourIntervalNames><HourIntervalName>a.m</HourIntervalName><HourIntervalName>p.m</HourIntervalName></HourIntervalNames><HourIntervalLongNames><HourIntervalLongName>morning</HourIntervalLongName><HourIntervalLongName>afternoon</HourIntervalLongName></HourIntervalLongNames><CrudeTimeIntervals><CrudeTimeInterval text=""day"" Lower=""0"" Upper=""24"" /></CrudeTimeIntervals></Clock>"), _gameworld.Object) { Id = 1L };
			var zone = new MudTimeZone(1, 0, 0, "UTC", "UTC");
			clock.AddTimezone(zone);
			clock.SetTime(MudTime.CreatePrimaryTime(0, 0, 0, zone, clock));
			clocks.Add(clock);
			var calendar = new MudSharp.TimeAndDate.Date.Calendar(1, _gameworld.Object);
			calendar.SetNoSave(true);
			calendar.SetupTestData();
			calendar.SetDate("1/jan/2026");
			typeof(Clan).GetField("_calendar", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(Clan, calendar);
			var accountSet = new Mock<IUneditableAll<IBankAccount>>();
			_gameworld.SetupGet(x => x.BankAccounts).Returns(accountSet.Object);
			Appointment.SetupGet(x => x.Id).Returns(7L);
			Appointment.SetupGet(x => x.Name).Returns("office");
			Appointment.SetupGet(x => x.Abbreviations).Returns(Array.Empty<string>());
			Appointment.SetupGet(x => x.Titles).Returns(Array.Empty<string>());
			var membership = new Mock<IClanMembership>();
			membership.SetupGet(x => x.Clan).Returns(Clan);
			membership.SetupGet(x => x.NetPrivileges).Returns(permission == 0 ? ClanPrivilegeType.CanCreateBudgets : ClanPrivilegeType.None);
			membership.SetupGet(x => x.Appointments).Returns(permission == 1 ? [Appointment.Object] : []);
			Actor.SetupGet(x => x.ClanMemberships).Returns([membership.Object]);
			Actor.Setup(x => x.IsAdministrator(PermissionLevel.Admin)).Returns(permission == 2);
			Actor.SetupGet(x => x.Id).Returns(3L);
			Actor.SetupGet(x => x.Gameworld).Returns(_gameworld.Object);
			var personalName = new Mock<IPersonalName>();
			personalName.Setup(x => x.GetName(NameStyle.FullName)).Returns("Budget Holder");
			Actor.SetupGet(x => x.PersonalName).Returns(personalName.Object);
			var output = new Mock<IOutputHandler>();
			output.Setup(x => x.Send(It.IsAny<string>(), true, false)).Callback<string, bool, bool>((message, _, _) => Messages.Add(message));
			Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
			Currency.SetupGet(x => x.Id).Returns(9L);
			Currency.SetupGet(x => x.Name).Returns("coins");
			Currency.Setup(x => x.Describe(It.IsAny<decimal>(), CurrencyDescriptionPatternType.ShortDecimal))
				.Returns<decimal, CurrencyDescriptionPatternType>((amount, _) => AmountDescription(amount));
			decimal amount = 10M;
			Currency.Setup(x => x.TryGetBaseCurrency("10", out amount)).Returns(true);
			bool exact = true;
			Currency.Setup(x => x.FindCoinsForAmount(10M, out exact)).Returns(new Dictionary<ICoin, int>());
			Actor.SetupGet(x => x.Currency).Returns(Currency.Object);
			var bank = new Mock<IBank>();
			bank.SetupGet(x => x.PrimaryCurrency).Returns(Currency.Object);
			bank.SetupGet(x => x.CurrencyReserves).Returns(new DecimalCounter<ICurrency>());
			bank.Setup(x => x.EconomicZone.ZoneForTimePurposes.DateTime(It.IsAny<ICalendar>())).Returns(MudDateTime.Never);
			BankAccount.Bank = bank.Object;
			BankAccount.Id = 12L;
			BankAccount.CurrentBalance = 1000M;
			BankAccount.BankAccountType = Mock.Of<IBankAccountType>();
			typeof(BankAccount).GetField("_recentTransactions", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(BankAccount, BankTransactions);
			_oldCurrencyPrototype = CurrencyGameItemComponentProto.ItemPrototype;
			var prototype = new Mock<IGameItemProto>();
			prototype.Setup(x => x.CreateNew(null)).Returns(_payout.Object);
			CurrencyGameItemComponentProto.ItemPrototype = prototype.Object;
			_payout.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(Mock.Of<ICurrencyPile>());
			_payout.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("coins");
			var body = new Mock<IBody>();
			body.Setup(x => x.CanGet(_payout.Object, 0, ItemCanGetIgnore.None)).Returns(true);
			Actor.SetupGet(x => x.Body).Returns(body.Object);
		}

		public void AddBudget(string name, bool hasBank, decimal openingVirtualBalance)
		{
			Budget.SetupGet(x => x.Id).Returns(1L);
			Budget.SetupGet(x => x.Name).Returns(name);
			Budget.SetupGet(x => x.Clan).Returns(Clan);
			Budget.SetupGet(x => x.Appointment).Returns(Appointment.Object);
			Budget.SetupGet(x => x.Currency).Returns(Currency.Object);
			Budget.SetupGet(x => x.BankAccount).Returns(hasBank ? BankAccount : null);
			Budget.SetupGet(x => x.IsActive).Returns(true);
			Budget.SetupGet(x => x.RemainingBudget).Returns(100M);
			Budget.SetupGet(x => x.CurrentPeriodStart).Returns(MudDateTime.Never);
			Budget.SetupGet(x => x.CurrentPeriodEnd).Returns(MudDateTime.Never);
			Clan.Budgets.Add(Budget.Object);
			_openingVirtualBalance = openingVirtualBalance;
			if (openingVirtualBalance > 0M) VirtualCashLedger.Credit(Clan, Currency.Object, openingVirtualBalance, null, null, "Cash", "owned test fixture");
		}

		public void Draw(string reason) => Clan.BudgetCommand(Actor.Object, new StringStack($"draw 1 10 {reason}"));

		public void UseDatabaseLedger(bool enabled)
		{
			if (!enabled) return;
			Assert.IsTrue(FMDB.IsIsolated);
			Assert.AreSame(Context, FMDB.Context);
			Assert.IsNull(FMDB.Connection);
			// Select the EF ledger branch using this already-owned InMemory session; no connection is opened.
			FMDB.ConnectionString = "percival-owned-inmemory-ledger";
		}

		public void AssertNoPayout() => _payout.Verify(x => x.Login(), Times.Never);

		public MudSharp.Models.BankAccountTransaction PersistBankTransaction()
		{
			using (new FMDB())
			{
				var transaction = (BankAccountTransaction)BankTransactions.Single();
				var row = (MudSharp.Models.BankAccountTransaction)transaction.DatabaseInsert();
				Context.SaveChanges();
				return row;
			}
		}

		public void AssertRejectedWithoutMutation()
		{
			Assert.AreEqual(_openingVirtualBalance, VirtualCashLedger.Balance(Clan, Currency.Object));
			Assert.AreEqual(1000M, BankAccount.CurrentBalance);
			Assert.AreEqual(0, BankTransactions.Count);
			Assert.AreEqual(0, VirtualCashLedger.LedgerEntries(Clan).Count(x => x.Amount < 0M));
			Assert.AreEqual(0, Context.ClanBudgetTransactions.Count());
			Budget.Verify(x => x.RollToCurrentPeriod(), Times.Never);
			Budget.Verify(x => x.AddDrawdown(It.IsAny<IClanBudgetTransaction>()), Times.Never);
			_payout.Verify(x => x.Login(), Times.Never);
			Assert.IsFalse(Clan.Changed);
			Assert.IsFalse(BankAccount.Changed);
			Assert.AreEqual(1, Messages.Count);
		}

		public void Dispose()
		{
			CurrencyGameItemComponentProto.ItemPrototype = _oldCurrencyPrototype;
			FMDB.ConnectionString = _previousConnectionString;
			_ambientValue.SetValue(_ambient, _previousSession);
			Context.Dispose();
			VirtualCashLedger.ClearInMemoryForTests();
		}

		private static void SetGameworld(SaveableItem item, IFuturemud gameworld) =>
			typeof(SaveableItem).GetField("_gameworld", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(item, gameworld);
	}
}
