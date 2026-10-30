#nullable enable

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Economy;
using MudSharp.Economy.Banking;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BankDepositPrivilegeTests
{
	[TestMethod]
	public void DepositPrivilege_DoesNotGrantBalanceHistoryWithdrawalOrManagement()
	{
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Id).Returns(42);
		var membership = new Mock<IClanMembership>();
		membership.SetupProperty(x => x.MemberId, 42);
		membership.SetupGet(x => x.NetPrivileges).Returns(ClanPrivilegeType.CanDepositBankAccounts);
		var clan = new Mock<IClan>(); clan.SetupGet(x => x.Memberships).Returns(new List<IClanMembership> { membership.Object });
		var account = (BankAccount)RuntimeHelpers.GetUninitializedObject(typeof(BankAccount));
		typeof(BankAccount).GetField("_accountOwner", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(account, clan.Object);
		Assert.IsTrue(account.IsAuthorisedAccountDepositor(actor.Object));
		Assert.IsFalse(account.IsAuthorisedAccountUser(actor.Object));
		Assert.IsTrue(BankAccountAccessRules.CanPerform(account, actor.Object, "deposit"));
		foreach (var operation in new[] { "show", "transactions", "withdraw", "transfer", "close", "alias", "requestitem", "cancelitems" })
			Assert.IsFalse(BankAccountAccessRules.CanPerform(account, actor.Object, operation), operation);
		var outsider = new Mock<ICharacter>(); outsider.SetupGet(x => x.Id).Returns(99);
		Assert.IsFalse(account.IsAuthorisedAccountDepositor(outsider.Object));
		membership.SetupGet(x => x.IsArchivedMembership).Returns(true);
		Assert.IsFalse(account.IsAuthorisedAccountDepositor(actor.Object));
	}

	[TestMethod]
	public void ExistingManagementPermission_StillPermitsDepositsAndWithdrawals()
	{
		var actor = new Mock<ICharacter>(); var account = new Mock<IBankAccount>();
		account.Setup(x => x.IsAuthorisedAccountUser(actor.Object)).Returns(true);
		Assert.IsTrue(BankAccountAccessRules.CanPerform(account.Object, actor.Object, "deposit"));
		Assert.IsTrue(BankAccountAccessRules.CanPerform(account.Object, actor.Object, "withdraw"));
	}
}
