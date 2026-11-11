#nullable enable
using System;
using System.Linq;
using System.Collections.Generic;
using MudSharp.Framework.Save;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Lending;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp_Unit_Tests;

public partial class MudDateTimeTests
{
	private static MudDateTime LendingStart()
	{
		// These calendars and clocks are already bound to the fixture world. Register their persisted IDs
		// just as a live world does so the new contract can be reloaded between each operation.
		var calendars = new All<ICalendar> { _testCalendar };
		var clocks = new All<IClock> { _testClock };
		Mock.Get(_gameworld).SetupGet(x => x.Calendars).Returns(calendars);
		Mock.Get(_gameworld).SetupGet(x => x.Clocks).Returns(clocks);
		return new MudDateTime("1/kep/22 UTC 12:00:00", _testCalendar, _testClock);
	}

	[TestMethod]
	public void LendingSchedule_ReplacementClearsFailureAndPreservesAutopayAccount()
	{
		var terms = new LoanTerms { AutopayAccountId = 23, AutopayFailure = "Insufficient funds", AutopayLastAttempt = "previous day" };
		LoanService.StartSchedule(terms, LendingStart());
		Assert.AreEqual(23L, terms.AutopayAccountId);
		Assert.IsNull(terms.AutopayFailure);
		Assert.IsNull(terms.AutopayLastAttempt);
	}

	[TestMethod]
	[DoNotParallelize]
	[DataRow(false)]
	[DataRow(true)]
	public void LendingPayment_FailedCashPostingPreservesCoinsDebtAndReserveAcrossSaveAndRetry(bool needsChange)
	{
		var now = LendingStart();
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), options => options.EnableNullChecks(false)).Options);
		var context = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var previous = context.GetValue(null);
		context.SetValue(null, db);
		try
		{
			using var handle = new FMDB();
			var world = new Mock<IFuturemud>();
			var saves = new Mock<ISaveManager>();
			world.SetupGet(x => x.SaveManager).Returns(saves.Object);
			var room = Mock.Of<IRoom>();
			var currency = new Mock<ICurrency>();
			currency.SetupGet(x => x.Id).Returns(17);
			var unit = Mock.Of<ICoin>(x => x.Id == 1 && x.Value == 1M);
			var four = Mock.Of<ICoin>(x => x.Id == 2 && x.Value == 4M);
			var coin = needsChange ? four : unit;
			var count = needsChange ? 3 : 12;
			currency.SetupGet(x => x.Coins).Returns(new[] { unit, four });
			world.SetupGet(x => x.Currencies).Returns(new All<ICurrency> { currency.Object });
			var zone = Mock.Of<IEconomicZone>(x => x.Id == 19 && x.Currency == currency.Object &&
				x.FinancialPeriodReferenceCalendar == Mock.Of<ICalendar>(c => c.CurrentDateTime == now));
			world.SetupGet(x => x.EconomicZones).Returns(new All<IEconomicZone> { zone });
			var reserves = new DecimalCounter<ICurrency> { [currency.Object] = 50M };
			var bank = Mock.Of<IBank>(x => x.Id == 11 && x.EconomicZone == zone && x.CurrencyReserves == reserves &&
				x.BranchLocations == new[] { room });
			world.SetupGet(x => x.Banks).Returns(new All<IBank> { bank });
			world.SetupGet(x => x.Shops).Returns(new All<IShop>());
			var item = new Mock<IGameItem>();
			item.SetupGet(x => x.Gameworld).Returns(world.Object);
			var component = new MudSharp.Models.GameItemComponent { Id = 31, Definition =
				$"""<Definition Currency="17"><Coins><Coin Id="{coin.Id}" Count="{count}"/></Coins></Definition>""" };
			var originalDefinition = component.Definition;
			db.GameItemComponents.Add(component);
			var pile = new CurrencyGameItemComponent(component, null!, item.Object);
			item.Setup(x => x.IsItemType<ICurrencyPile>()).Returns(true);
			item.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(pile);
			var exact = true;
			currency.Setup(x => x.FindCoinsForAmount(needsChange ? 2M : 0M, out exact))
				.Returns(needsChange ? new Dictionary<ICoin, int> { [unit] = 2 } : new Dictionary<ICoin, int>());
			currency.Setup(x => x.FindCurrency(It.IsAny<IEnumerable<ICurrencyPile>>(), 10))
				.Returns(new Dictionary<ICurrencyPile, Dictionary<ICoin, int>> { [pile] = new() { [coin] = needsChange ? 3 : 10 } });
			var actor = new Mock<ICharacter>();
			actor.SetupGet(x => x.Id).Returns(7);
			actor.SetupGet(x => x.Gameworld).Returns(world.Object);
			actor.SetupGet(x => x.Location).Returns(room);
			item.Setup(x => x.IsOwnedBy(actor.Object)).Returns(true);
			actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			actor.SetupGet(x => x.Body.ExternalItems).Returns(new[] { item.Object });
			IProposal? proposal = null;
			actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
				.Callback<IEffect, TimeSpan>((effect, _) => proposal = ((MudSharp.Effects.Concrete.Accept)effect).Proposal);
			var loan = new LendingContract { Id = 5, BorrowerId = 7, LenderId = 11, LenderType = "Bank", EconomicZoneId = 19,
				CurrencyId = 17, Status = "Active", Principal = 10M };
			LoanService.Store(loan, new LoanTerms { Advance = 10M });
			db.LendingContracts.Add(loan);
			db.SaveChanges();
			var fail = true;
			var injected = false;
			saves.Setup(x => x.Flush()).Callback(() =>
			{
				if (pile.Changed) pile.Save();
				if (fail && db.ChangeTracker.Entries<LendingPosting>().Any(x => x.Entity.Kind == "Payment"))
				{
					injected = true;
					throw new DbUpdateException("Rejected payment posting");
				}
			});

			LoanService.Pay(actor.Object, 5, "all", null);
			Assert.IsNotNull(proposal);
			proposal.Accept();
			Assert.IsTrue(injected);
			fail = false;
			saves.Object.Flush();
			db.SaveChanges();
			Assert.AreEqual(Tuple.Create(coin, count), pile.Coins.Single());
			Assert.AreEqual(originalDefinition, db.GameItemComponents.AsNoTracking().Single().Definition);
			Assert.AreEqual(50M, reserves[currency.Object]);
			Assert.AreEqual(10M, db.LendingContracts.AsNoTracking().Single().Principal);
			Assert.IsFalse(db.LendingPostings.Any());
			LoanService.Pay(actor.Object, 5, "all", null);
			proposal.Accept();
			proposal.Accept();
			Assert.AreEqual(Tuple.Create(unit, 2), pile.Coins.Single());
			var storedCoins = System.Xml.Linq.XElement.Parse(db.GameItemComponents.AsNoTracking().Single().Definition)
				.Descendants("Coin").Single();
			Assert.AreEqual((unit.Id, 2), ((long)storedCoins.Attribute("Id")!, (int)storedCoins.Attribute("Count")!));
			Assert.AreEqual(60M, reserves[currency.Object]);
			Assert.AreEqual("Closed", db.LendingContracts.AsNoTracking().Single().Status);
			Assert.AreEqual(10M, db.LendingPostings.Single().Amount);
		}
		finally { context.SetValue(null, previous); }
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void LendingCommit_LostReplyAndUnavailableDatabaseWaitForReceipt(bool committed)
	{
		var reads = 0;
		var waits = 0;
		var outcome = LoanService.VerifyCommit(() =>
		{
			if (++reads == 1) throw new InvalidOperationException("Transient query failure", new TimeoutException());
			return committed;
		}, () => waits++);
		Assert.AreEqual(committed, outcome);
		Assert.AreEqual((2, 1), (reads, waits));
	}

	[TestMethod]
	public void LendingAuction_ClockTicksPreserveTheListingAndItsBids()
	{
		var clock = LendingStart();
		var timestamp = clock.GetDateTimeString();
		var lot = new AuctionItem { ListingDateTime = clock, FinishingDateTime = clock + MudTimeSpan.FromDays(1) };
		var bid = new AuctionBid { BidDateTime = clock, Bid = 12, BidderId = 7 };
		var bids = new CollectionDictionary<AuctionItem, AuctionBid>();
		bids.Add(lot, bid);
		clock.Time.AddSeconds(60);
		Assert.AreEqual(timestamp, lot.ListingDateTime.GetDateTimeString());
		Assert.AreEqual(timestamp, bid.BidDateTime.GetDateTimeString());
		Assert.AreSame(bid, bids[lot].Single());
	}

	[TestMethod]
	public void LendingSchedule_PartialPaymentTracksArrearsAndReducesFutureInterest()
	{
		var start = LendingStart();
		var terms = new LoanTerms { Rate = 0.01M, Kind = "Mortgage", Minimum = "Principal", Instalment = 100M, Periods = 8 };
		var loan = new LendingContract { Principal = 800, Status = "Active" };
		LoanService.StartSchedule(terms, start);
		var first = new MudDateTime(terms.NextDue, _gameworld);
		LoanService.Accrue(loan, terms, first);
		Assert.AreEqual(108M, terms.Bills.Single().Interest + terms.Bills.Single().Principal);
		Assert.AreEqual((52M, 8M), LoanService.ApplyPayment(loan, terms, 60M));
		Assert.AreEqual(748M, loan.Principal);
		Assert.AreEqual(48M, terms.Bills.Single().Principal);
		terms = LoanService.Terms(loan);
		LoanService.Accrue(loan, terms, first);
		Assert.AreEqual((48M, 0M), LoanService.ApplyPayment(loan, terms, 48M));
		Assert.IsEmpty(terms.Bills);
		LoanService.Accrue(loan, terms, new MudDateTime(terms.NextDue, _gameworld));
		Assert.AreEqual(107M, terms.Bills.Single().Interest + terms.Bills.Single().Principal);
	}

	[TestMethod]
	public void LendingSchedule_BalloonAndGraceAccrueOnceThroughRestartAndStopAtDeath()
	{
		var start = LendingStart();
		var terms = new LoanTerms { Rate = 0.05M, Interval = "every week", Periods = 4, Minimum = "None" };
		var loan = new LendingContract { Principal = 40, Status = "Active" };
		LoanService.StartSchedule(terms, start);
		LoanService.Accrue(loan, terms, start + MudTimeSpan.FromDays(21));
		Assert.IsEmpty(terms.Bills);
		Assert.AreEqual(6M, loan.Interest);
		LoanService.Accrue(loan, terms, start + MudTimeSpan.FromDays(28));
		Assert.AreEqual(48M, terms.Bills.Single().Interest + terms.Bills.Single().Principal);
		terms = LoanService.Terms(loan);
		var endOfGrace = start + MudTimeSpan.FromDays(35);
		LoanService.Accrue(loan, terms, endOfGrace);
		LoanService.Accrue(loan, terms, endOfGrace);
		Assert.AreEqual(50M, terms.Bills.Single().Interest + terms.Bills.Single().Principal);
		loan.EstateId = 4;
		LoanService.Accrue(loan, terms, endOfGrace + MudTimeSpan.FromDays(7));
		Assert.AreEqual(10M, loan.Interest);
	}

	[TestMethod]
	[DataRow(true, 4)]
	[DataRow(false, 2)]
	public void LendingSchedule_Pause_CatchesUpOnceAtAgreedRate(bool chargeInterest, int expected)
	{
		var start = LendingStart();
		var week = MudTimeSpan.FromDays(7);
		var loan = new LendingContract { Principal = 40, Status = "Active" };
		var terms = new LoanTerms { Rate = 0.05M, Interval = "every week", Periods = 4, Minimum = "Interest" };
		LoanService.StartSchedule(terms, start);
		terms.ChargeDuringPause = chargeInterest;
		terms.PauseUntil = (start + week).GetDateTimeString();
		terms.PeriodStart = terms.PauseUntil;
		var end = start + week + week;
		terms.NextDue = end.GetDateTimeString();
		LoanService.Accrue(loan, terms, start + MudTimeSpan.FromDays(3));
		LoanService.Accrue(loan, LoanService.Terms(loan), end);
		Assert.AreEqual((decimal)expected, loan.Interest);
		Assert.AreEqual((decimal)expected, LoanService.Terms(loan).Bills.Single().Interest);
	}

	[TestMethod]
	[DoNotParallelize]
	public void LendingProbate_OnlyRegisteredPawnClaimHasPriorityAndSurplusDistributesOnce()
	{
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), options => options.EnableNullChecks(false)).Options);
		var context = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var original = context.GetValue(null);
		context.SetValue(null, db);
		try
		{
			using var scope = new FMDB();
			var loan = new LendingContract
			{
				Id = 5, BorrowerId = 7, LenderId = 11, LenderType = "Bank", EstateId = 19, CurrencyId = 17,
				Status = "Active", SecurityKey = "Item:23", Principal = 40M
			};
			LoanService.Store(loan, new LoanTerms { Kind = "Pawn", AssetId = 23, Recourse = false, LoanEstateClaimId = 29 });
			var surplus = new LendingContract { Id = 6, BorrowerId = 7, EstateId = 19, CurrencyId = 17, Status = "Closed", Surplus = 25M };
			db.LendingContracts.AddRange(loan, surplus);
			db.SaveChanges();
			var world = new Mock<IFuturemud>();
			var lender = Mock.Of<IBank>(x => x.Id == 11 && x.FrameworkItemType == "Bank");
			world.SetupGet(x => x.Banks).Returns(new All<IBank> { lender });
			var zone = Mock.Of<IEconomicZone>(x => x.Gameworld == world.Object && x.FinancialPeriodReferenceCalendar == _testCalendar &&
				x.Currency == Mock.Of<ICurrency>(c => c.Id == 17));
			var estate = Mock.Of<IEstate>(x => x.Id == 19 && x.Gameworld == world.Object && x.EconomicZone == zone &&
				x.Character == Mock.Of<ICharacter>(c => c.Id == 7));
			var item = Mock.Of<IGameItem>(x => x.Id == 23 && x.FrameworkItemType == "GameItem");
			var claim = new Mock<IEstateClaim>();
			claim.SetupGet(x => x.Id).Returns(31);
			claim.SetupGet(x => x.Estate).Returns(estate);
			claim.SetupGet(x => x.Claimant).Returns(lender);
			claim.SetupGet(x => x.TargetItem).Returns(item);
			claim.SetupGet(x => x.Reason).Returns("LendingContract:5");
			claim.SetupGet(x => x.IsSecured).Returns(true);
			LoanService.LoadPortfolio(world.Object);
			Assert.IsFalse(LoanService.IsSecuredEstateLoanClaim(claim.Object));
			claim.SetupGet(x => x.Id).Returns(29);
			Assert.IsTrue(LoanService.IsSecuredEstateLoanClaim(claim.Object));
			Assert.IsFalse(LoanService.CanSettleEstateClaimFromGeneralPool(claim.Object));
			Assert.IsTrue(LoanService.TryCollectEstateSurplus(estate, out var first));
			Assert.AreEqual(25M, first);
			Assert.IsTrue(LoanService.TryCollectEstateSurplus(estate, out var second));
			Assert.AreEqual(0M, second);
			Assert.AreEqual(25M, db.LendingPostings.Single().Amount);
		}
		finally
		{
			context.SetValue(null, original);
		}
	}

	[TestMethod]
	public void LendingPayment_CapitalisedInterestIsRealisedOnce()
	{
		var loan = new LendingContract { Principal = 110M, Interest = 2M };
		var terms = new LoanTerms { RolledInterest = 10M };
		Assert.AreEqual((3M, 12M), LoanService.ApplyPayment(loan, terms, 15M));
		Assert.AreEqual(97M, loan.Principal);
		Assert.AreEqual((2M, 0M), LoanService.ApplyPayment(loan, LoanService.Terms(loan), 2M));
	}
}
