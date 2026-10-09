#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Employment;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.Economy.Employment;

public partial class UnifiedEmploymentDispatchTests
{
	[TestMethod]
	[DataRow(EmploymentAuthority.UseStoreAccount)]
	[DataRow(EmploymentAuthority.UseStoreAccount | EmploymentAuthority.SettleHostAccounts)]
	public void EmploymentFinanceSteps_WithoutWithdrawalAuthority_BlocksTransferBeforeMutation(EmploymentAuthority authority)
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var source = BankAccount(currency.Object, 25M, 1000M, id: 901, accountNumber: 123);
		var target = BankAccount(currency.Object, 3M, 1000M, id: 902, accountNumber: 456);
		var (shop, state) = ShopHost(154, "Restricted Transfer Shop", currency.Object, source.Object);
		var world = Gameworld(currency.Object, new Dictionary<long, ICharacter>(), bankAccountsToAdd: [source.Object, target.Object]);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(world.Object);
		var manager = Character(1, "Manager", gameworld: world.Object).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases | EmploymentAuthority.ManageStockRules | authority), null);
		var step = new BankAccountTransferActionStep(target.Object.AccountReference, new MoneyAmount(currency.Object, 10M));
		var context = new EmploymentTaskContext(shop.Object);

		Assert.ThrowsException<InvalidOperationException>(() => state.TaskBoard.CreateActiveTask("unauthorised transfer", new EmploymentActionPlan([step]), manager));
		Assert.IsFalse(step.CanExecute(context, manager, out var reason));
		StringAssert.Contains(reason, "lacks the authority");
		var result = step.Execute(context, manager);
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "withdraw business cash");
		Assert.AreEqual(25M, source.Object.CurrentBalance);
		Assert.AreEqual(3M, target.Object.CurrentBalance);
		Assert.IsFalse(state.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.AccountTransfer));
		source.Verify(x => x.WithdrawFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
		target.Verify(x => x.DepositFromTransfer(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
	}
}
