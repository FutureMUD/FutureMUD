#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Lending;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp_Unit_Tests;

public partial class MudDateTimeTests
{
	[TestMethod]
	[DoNotParallelize]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void LendingAutopay_DueOffline_PreservesFailedAttemptThenRetriesOnce(bool authorised, bool automaticRetry)
	{
		var now = LendingStart();
		var current = now;
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), o => o.EnableNullChecks(false)).Options);
		var context = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var previous = context.GetValue(null);
		context.SetValue(null, db);
		try
		{
			using var handle = new FMDB();
			var world = new Mock<IFuturemud>();
			world.SetupGet(x => x.Calendars).Returns(_gameworld.Calendars);
			world.SetupGet(x => x.Clocks).Returns(_gameworld.Clocks);
			world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			world.SetupGet(x => x.Actors).Returns(new All<ICharacter>());
			var currency = Mock.Of<ICurrency>(x => x.Id == 17);
			world.SetupGet(x => x.Currencies).Returns(new All<ICurrency> { currency });
			var calendar = new Mock<ICalendar>();
			calendar.SetupGet(x => x.CurrentDateTime).Returns(() => current);
			var zone = Mock.Of<IEconomicZone>(x => x.Id == 19 && x.Currency == currency &&
				x.FinancialPeriodReferenceCalendar == calendar.Object);
			world.SetupGet(x => x.EconomicZones).Returns(new All<IEconomicZone> { zone });
			var lenderReserves = new DecimalCounter<ICurrency> { [currency] = 50M };
			var payerReserves = new DecimalCounter<ICurrency> { [currency] = 100M };
			var lender = Mock.Of<IBank>(x => x.Id == 11 && x.EconomicZone == zone && x.CurrencyReserves == lenderReserves);
			var payer = Mock.Of<IBank>(x => x.Id == 12 && x.EconomicZone == zone && x.CurrencyReserves == payerReserves);
			world.SetupGet(x => x.Banks).Returns(new All<IBank> { lender, payer });
			var actor = Mock.Of<ICharacter>(x => x.Id == 7 && x.Gameworld == world.Object);
			world.Setup(x => x.TryGetCharacter(7, true)).Returns(actor);
			var account = new Mock<IBankAccount>();
			account.SetupGet(x => x.Id).Returns(23);
			account.SetupGet(x => x.Currency).Returns(currency);
			account.SetupGet(x => x.Bank).Returns(payer);
			account.SetupGet(x => x.AccountStatus).Returns(BankAccountStatus.Active);
			account.SetupGet(x => x.AccountReference).Returns("payer:1");
			account.Setup(x => x.IsAuthorisedAccountUser(actor)).Returns(() => authorised);
			account.Setup(x => x.CanWithdraw(10M, false)).Returns((true, ""));
			decimal balance = 10M;
			account.Setup(x => x.MaximumWithdrawal()).Returns(() => balance);
			var type = new Mock<IBankAccountType>();
			type.Setup(x => x.QuoteTransactionWithdrawalFees(account.Object, 10M)).Returns(2M);
			account.SetupGet(x => x.BankAccountType).Returns(type.Object);
			account.Setup(x => x.WithdrawFromTransaction(10M, It.IsAny<string>())).Callback(() => balance -= 12M);
			world.SetupGet(x => x.BankAccounts).Returns(new All<IBankAccount> { account.Object });
			var loan = new LendingContract { Id = 5, BorrowerId = 7, LenderId = 11, LenderType = "Bank", EconomicZoneId = 19,
				CurrencyId = 17, Status = "Active", Principal = 20M };
			var terms = new LoanTerms { AutopayAccountId = 23, Bills = new List<LoanInstalment>
				{ new() { Due = now.GetDateTimeString(), Principal = 10M } } };
			LoanService.Store(loan, terms);
			db.LendingContracts.Add(loan);
			db.SaveChanges();

			LoanService.LoadPortfolio(world.Object);
			LoanService.ProcessAutopay(world.Object);
			Assert.IsNotNull(LoanService.Terms(db.LendingContracts.Single()).AutopayFailure);
			Assert.AreEqual((10M, 50M, 100M, 20M), (balance, lenderReserves[currency], payerReserves[currency], loan.Principal));
			Assert.IsFalse(db.LendingPostings.Any());
			var output = new Mock<IOutputHandler>();
			Mock.Get(actor).SetupGet(x => x.OutputHandler).Returns(output.Object);
			LoanService.LoadPortfolio(world.Object);
			LoanService.NotifyBorrower(actor);
			Assert.IsTrue(output.Invocations.Any(x => x.Arguments.OfType<string>().Any(s => s.Contains("Failed automatic payment"))));
			balance = 100M;
			authorised = true;
			var changedTerms = LoanService.Terms(db.LendingContracts.Single());
			changedTerms.Bills[0].Interest = 1M;
			loan.Interest = 1M;
			LoanService.Store(loan, changedTerms);
			db.SaveChanges();
			Assert.AreEqual("Automatic payment already attempted today.", LoanService.AttemptAutopay(world.Object, 5));
			changedTerms.Bills[0].Interest = 0M;
			changedTerms.BorrowerDiedOutsideProbate = true;
			loan.Interest = 0M;
			LoanService.Store(loan, changedTerms);
			db.SaveChanges();
			LoanService.RetryAutopay(actor, 5);
			Assert.AreEqual(100M, balance);
			changedTerms.BorrowerDiedOutsideProbate = false;
			LoanService.Store(loan, changedTerms);
			db.SaveChanges();
			if (automaticRetry)
			{
				current = now + MudSharp.TimeAndDate.MudTimeSpan.FromDays(1);
				LoanService.LoadPortfolio(world.Object);
				LoanService.ProcessAutopay(world.Object);
			}
			else LoanService.RetryAutopay(actor, 5);
			LoanService.AttemptAutopay(world.Object, 5);
			Assert.AreEqual((88M, 60M, 90M), (balance, lenderReserves[currency], payerReserves[currency]));
			Assert.AreEqual(10M, db.LendingContracts.Single().Principal);
			Assert.IsNull(LoanService.Terms(db.LendingContracts.Single()).AutopayFailure);
			Assert.AreEqual(10M, db.LendingPostings.Single().Amount);
			LoanService.ConfigureAutopay(actor, 5, "off");
			Assert.IsNull(LoanService.Terms(db.LendingContracts.AsNoTracking().Single()).AutopayAccountId);
			LoanService.ProcessAutopay(world.Object);
			account.Verify(x => x.WithdrawFromTransaction(10M, It.IsAny<string>()), Times.Once);
		}
		finally { context.SetValue(null, previous); }
	}
}
