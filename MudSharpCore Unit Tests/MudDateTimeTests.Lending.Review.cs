#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Date;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Lending;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class MudDateTimeTests
{
	[TestMethod]
	[DoNotParallelize]
	public void LendingShopCash_RejectedAdvanceRestoresPhysicalRegisterAndRetriesOnce()
	{
		var now = LendingStart();
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), o => o.EnableNullChecks(false)).Options);
		var context = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var previous = context.GetValue(null);
		context.SetValue(null, db);
		try
		{
			using var handle = new FMDB();
			var world = new Mock<IFuturemud>();
			var saves = new Mock<ISaveManager>();
			world.SetupGet(x => x.SaveManager).Returns(saves.Object);
			var currency = new Mock<ICurrency>();
			currency.SetupGet(x => x.Id).Returns(17);
			world.SetupGet(x => x.Currencies).Returns(new All<ICurrency> { currency.Object });
			var coin = Mock.Of<ICoin>(x => x.Id == 1 && x.Value == 1M);
			currency.SetupGet(x => x.Coins).Returns(new[] { coin });
			var item = Mock.Of<IGameItem>(x => x.Gameworld == world.Object);
			var row = new MudSharp.Models.GameItemComponent { Id = 31, Definition =
				"""<Definition Currency="17"><Coins><Coin Id="1" Count="100"/></Coins></Definition>""" };
			db.GameItemComponents.Add(row);
			var pile = new CurrencyGameItemComponent(row, null!, item);
			var exact = true;
			currency.Setup(x => x.FindCoinsForAmount(0M, out exact)).Returns(new Dictionary<ICoin, int>());
			currency.Setup(x => x.FindCurrency(It.IsAny<IEnumerable<ICurrencyPile>>(), 40M))
				.Returns(new Dictionary<ICurrencyPile, Dictionary<ICoin, int>> { [pile] = new() { [coin] = 40 } });
			var shop = new Mock<IShop>();
			shop.SetupGet(x => x.Currency).Returns(currency.Object);
			shop.SetupProperty(x => x.CashBalance, 0M);
			shop.SetupProperty(x => x.ExpectedCashBalance, 0M);
			shop.Setup(x => x.GetCurrencyPilesForShop()).Returns(new[] { pile });
			var loan = new LendingContract { Id = 5, Principal = 0, Status = "Offered" };
			db.LendingContracts.Add(loan);
			db.SaveChanges();
			var reject = true;
			saves.Setup(x => x.Flush()).Callback(() =>
			{
				if (pile.Changed) pile.Save();
				if (reject && db.ChangeTracker.Entries<LendingPosting>().Any()) throw new DbUpdateException("Rejected Advance");
			});
			void Advance() => LoanService.InTransaction(world.Object, () =>
			{
				LoanService.Credit(shop.Object, -40M, "Loan advance");
				var liveLoan = db.LendingContracts.Find(5L)!;
				liveLoan.Principal = 40M;
				LoanService.Post(liveLoan, "Advance", 40M, 0, 40M, "Test register funding", now);
				return 0;
			});
			Assert.ThrowsExactly<DbUpdateException>(Advance);
			reject = false;
			saves.Object.Flush();
			db.SaveChanges();
			Assert.AreEqual(100M, pile.TotalValue);
			Assert.AreEqual(0M, db.LendingContracts.AsNoTracking().Single().Principal);
			Assert.IsFalse(db.LendingPostings.Any());
			Advance();
			Assert.AreEqual(60M, pile.TotalValue);
			Assert.AreEqual(0M, shop.Object.CashBalance);
			Assert.AreEqual(40M, db.LendingPostings.Single().Amount);
		}
		finally { context.SetValue(null, previous); }
	}
	[TestMethod]
	[DoNotParallelize]
	public void LendingAccept_UnderwritingWithdrawsOffer_DoesNotAdvanceFunds()
	{
		var now = LendingStart();
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), o => o.EnableNullChecks(false)).Options);
		var context = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var previous = context.GetValue(null);
		context.SetValue(null, db);
		try
		{
			using var handle = new FMDB();
			var world = new Mock<IFuturemud>();
			world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			world.Setup(x => x.GetStaticBool("LoansEnabled")).Returns(true);
			var room = Mock.Of<IRoom>();
			var zone = Mock.Of<IEconomicZone>(x => x.Id == 19 && x.Currency == Mock.Of<ICurrency>(c => c.Id == 17) &&
				x.FinancialPeriodReferenceCalendar == Mock.Of<ICalendar>(c => c.CurrentDateTime == now));
			var bank = Mock.Of<IBank>(x => x.Id == 11 && x.EconomicZone == zone && x.BranchLocations == new[] { room } && x.FrameworkItemType == "Bank");
			world.SetupGet(x => x.Banks).Returns(new All<IBank> { bank });
			world.SetupGet(x => x.Shops).Returns(new All<IShop>());
			var actor = Mock.Of<ICharacter>(x => x.Id == 7 && x.Gameworld == world.Object && x.Location == room);
			var definition = new LendingProductDefinition();
			definition.Policy.UnsecuredEligibilityProgId = 9;
			var product = new LendingProduct { Id = 3, Enabled = true, EconomicZoneId = 19,
				Definition = System.Text.Json.JsonSerializer.Serialize(definition) };
			var loan = new LendingContract { Id = 5, ProductId = 3, BorrowerId = 7, LenderId = 11, LenderType = "Bank", Status = "Offered", CurrencyId = 17 };
			LoanService.Store(loan, new LoanTerms { Advance = 40M, OfferExpiryUtc = DateTime.UtcNow.AddHours(1) });
			db.LendingProducts.Add(product);
			db.LendingContracts.Add(loan);
			db.SaveChanges();
			var prog = new Mock<IFutureProg>();
			prog.SetupGet(x => x.Id).Returns(9);
			prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			prog.Setup(x => x.ExecuteBool(false, It.IsAny<object[]>())).Returns(() => { product.Enabled = false; loan.Status = "Withdrawn"; return true; });
			world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { prog.Object });
			StringAssert.Contains(LoanService.Accept(actor, 5), "changed");
			Assert.AreEqual("Withdrawn", db.LendingContracts.AsNoTracking().Single().Status);
			Assert.IsFalse(db.LendingPostings.Any());
		}
		finally { context.SetValue(null, previous); }
	}

	[TestMethod]
	[DoNotParallelize]
	public void LendingProbate_OnlyAssetIsRecoveryAuction_CreatesEstateBeforeDeferringClaim()
	{
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), o => o.EnableNullChecks(false)).Options);
		var context = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var previous = context.GetValue(null);
		context.SetValue(null, db);
		try
		{
			using var handle = new FMDB();
			var world = new Mock<IFuturemud>();
			var actor = Mock.Of<ICharacter>(x => x.Id == 7 && x.Gameworld == world.Object);
			var zone = Mock.Of<IEconomicZone>(x => x.Id == 19 && x.Gameworld == world.Object);
			world.SetupGet(x => x.EconomicZones).Returns(new All<IEconomicZone> { zone });
			var house = Mock.Of<IAuctionHouse>(x => x.Id == 13 && x.ActiveAuctionItems == new[] { new AuctionItem { RecoveryContractId = 5 } });
			world.SetupGet(x => x.AuctionHouses).Returns(new All<IAuctionHouse> { house });
			var loan = new LendingContract { Id = 5, BorrowerId = 7, EconomicZoneId = 19, Status = "Recovery", Principal = 40 };
			LoanService.Store(loan, new LoanTerms { AuctionHouseId = 13 });
			db.LendingContracts.Add(loan);
			db.SaveChanges();
			LoanService.LoadPortfolio(world.Object);
			var estateRequested = false;
			LoanService.CreateEstateClaims(actor, requested => { Assert.AreSame(zone, requested); estateRequested = true; return null!; });
			Assert.IsTrue(estateRequested);
			Assert.AreEqual("Recovery", loan.Status);
			Assert.IsFalse(db.LendingPostings.Any());
		}
		finally { context.SetValue(null, previous); }
	}

}
