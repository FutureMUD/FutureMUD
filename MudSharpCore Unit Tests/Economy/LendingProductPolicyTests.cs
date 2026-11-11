#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Economy.Lending;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
public class LendingProductPolicyTests
{
	[TestMethod]
	[DoNotParallelize]
	public void PawnAppraisal_ExistingOfferAcceptsUnchangedConditionAndRejectsDamage()
	{
		var room = Mock.Of<IRoom>();
		var currency = Mock.Of<ICurrency>(x => x.BaseCurrencyToGlobalBaseCurrencyConversion == 1M);
		var zone = Mock.Of<IEconomicZone>(x => x.Currency == currency);
		var bank = Mock.Of<IBank>(x => x.EconomicZone == zone && x.BranchLocations == new[] { room });
		var world = new Mock<IFuturemud>();
		var borrower = Mock.Of<ICharacter>(x => x.Gameworld == world.Object && x.Location == room);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(23L);
		item.SetupGet(x => x.Owner).Returns(borrower);
		item.SetupGet(x => x.Prototype).Returns(Mock.Of<IGameItemProto>(x => x.CostInBaseCurrency == 100M));
		item.SetupProperty(x => x.Condition, 0.95);
		Mock.Get(borrower).SetupGet(x => x.Body.HeldItems).Returns(new[] { item.Object });
		world.Setup(x => x.TryGetItem(23L, false)).Returns(item.Object);

		var terms = new LoanTerms { Kind = "Pawn" };
		Assert.IsTrue(LoanService.TrySecurity(borrower, bank, terms, "23", out var error), error);
		Assert.AreEqual(95M, terms.Appraisal);

		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		var context = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var original = context.GetValue(null);
		context.SetValue(null, db);
		try
		{
			using var scope = new FMDB();
			Assert.IsTrue(LoanService.CanSecure(borrower, bank, terms, out error), error);
			item.Object.Condition = 0.9499999999;
			Assert.IsFalse(LoanService.CanSecure(borrower, bank, terms, out error));
			StringAssert.Contains(error, "condition");
		}
		finally
		{
			context.SetValue(null, original);
		}
	}

	[TestMethod]
	[DoNotParallelize]
	public void LendingRecovery_RetainedGoodsCarrySettlementValueAndLoseTheirBasisOnDisposal()
	{
		using var db = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), options => options.EnableNullChecks(false)).Options);
		var context = typeof(FMDB).GetProperty(nameof(FMDB.Context), BindingFlags.Public | BindingFlags.Static)!;
		var original = context.GetValue(null);
		context.SetValue(null, db);
		try
		{
			using var scope = new FMDB();
			var world = new Mock<IFuturemud>();
			var shop = Mock.Of<IShop>(x => x.Gameworld == world.Object && x.Id == 11 && x.FrameworkItemType == "Shop");
			var item = Mock.Of<IGameItem>(x => x.Gameworld == world.Object && x.Id == 23 && x.Owner == shop);
			world.SetupGet(x => x.Items).Returns(new All<IGameItem> { item });
			var loan = new LendingContract { Id = 2, LenderId = 11, LenderType = "Shop", Status = "Closed", CurrencyId = 5, EconomicZoneId = 7 };
			LoanService.Store(loan, new LoanTerms { AssetId = 23, CustodyStatus = "Retained", Kind = "Pawn", Appraisal = 100 });
			db.LendingContracts.Add(loan);
			db.LendingPostings.Add(new LendingPosting { ContractId = 2, Kind = "CollateralRecovery", Principal = 70, Interest = 10, Amount = 80 });
			db.SaveChanges();
			LoanService.LoadPortfolio(world.Object);
			Assert.AreEqual(100M, LoanService.RetainedPawns(world.Object, 7, 5, shop).Single().Value);
			Assert.AreEqual(90M, LoanService.RetainedStockCost(shop, item));
			LoanService.EndRetainedPawnOwnership(item);
			LoanService.LoadPortfolio(world.Object);
			Assert.IsEmpty(LoanService.RetainedPawns(world.Object, 7, 5, shop));
			Assert.IsNull(LoanService.RetainedStockCost(shop, item));
		}
		finally
		{
			context.SetValue(null, original);
		}
	}

	[TestMethod]
	public void Underwrite_SecuredProgsSeeActualBorrowerAndAmountAndCannotOverrideHardLimits()
	{
		var world = new Mock<IFuturemud>();
		var borrower = Mock.Of<ICharacter>(x => x.Gameworld == world.Object);
		var lender = Mock.Of<IBank>();
		var progs = new All<IFutureProg>();
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		Mock<IFutureProg> Prog(long id, ProgVariableTypes result)
		{
			var prog = new Mock<IFutureProg>();
			prog.SetupGet(x => x.Id).Returns(id);
			prog.SetupGet(x => x.ReturnType).Returns(result);
			prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			progs.Add(prog.Object);
			return prog;
		}
		var eligibility = Prog(9, ProgVariableTypes.Boolean);
		var cap = Prog(12, ProgVariableTypes.Number);
		var definition = new LendingProductDefinition();
		definition.Terms.Kind = "Pawn";
		definition.Policy.SecuredEligibilityProgId = 9;
		definition.Policy.SecuredMaximumAmountProgId = 12;
		// This missing unsecured filter must have no effect on a pawn loan.
		definition.Policy.UnsecuredEligibilityProgId = 99;
		var product = new LendingProduct { Enabled = true, Definition = JsonSerializer.Serialize(definition) };
		eligibility.Setup(x => x.ExecuteBool(false, It.IsAny<object[]>())).Returns((bool _, object[] a) =>
			a[0] == borrower && (decimal)a[1] <= 85M && (decimal)a[2] == 100M && (decimal)a[3] == 5M);
		cap.Setup(x => x.ExecuteDecimal(It.IsAny<object[]>())).Returns(70M);
		Assert.IsTrue(LoanService.Underwrite(borrower, lender, product, 70, 100, 5, out _));
		Assert.IsFalse(LoanService.Underwrite(borrower, lender, product, 71, 100, 5, out _));
		cap.Setup(x => x.ExecuteDecimal(It.IsAny<object[]>())).Returns(500M);
		Assert.IsFalse(LoanService.Underwrite(borrower, lender, product, 81, 100, 5, out _));
		eligibility.Setup(x => x.ExecuteBool(false, It.IsAny<object[]>())).Throws<InvalidOperationException>();
		Assert.IsFalse(LoanService.Underwrite(borrower, lender, product, 10, 100, 5, out _));
		definition.Terms.Kind = "Unsecured";
		product.Definition = JsonSerializer.Serialize(definition);
		Assert.IsFalse(LoanService.Underwrite(borrower, lender, product, 10, 0, 5, out _));
	}

	[TestMethod]
	public void HardMaximum_Unsecured_UsesUnsecuredPerLoanAndPortfolioLimits()
	{
		var policy = new LendingProductPolicy
		{
			MaximumUnsecuredAmount = 100.0M,
			MaximumUnsecuredExposure = 250.0M,
			MaximumSecuredAmount = 900.0M,
			MaximumSecuredExposure = 1000.0M
		};

		Assert.AreEqual(100.0M, LoanService.HardMaximum(policy, false, 0.0M, 50.0M));
		Assert.AreEqual(40.0M, LoanService.HardMaximum(policy, false, 0.0M, 210.0M));
		Assert.AreEqual(0.0M, LoanService.HardMaximum(policy, false, 0.0M, 250.0M));
	}

	[TestMethod]
	public void HardMaximum_Secured_IsLimitedByAppraisalCoverageAndSeparateExposure()
	{
		var policy = new LendingProductPolicy
		{
			MaximumSecuredAmount = 500.0M,
			MaximumSecuredExposure = 800.0M,
			MinimumCollateralValue = 100.0M,
			MinimumCollateralCoverage = 2.0M
		};

		Assert.AreEqual(150.0M, LoanService.HardMaximum(policy, true, 300.0M, 100.0M));
		Assert.AreEqual(300.0M, LoanService.HardMaximum(policy, true, 1000.0M, 500.0M));
		Assert.AreEqual(0.0M, LoanService.HardMaximum(policy, true, 99.0M, 0.0M));
	}
}
