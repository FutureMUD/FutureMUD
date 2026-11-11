#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
public class BankWithdrawalFeeTests
{
	[TestMethod]
	public void BuildingCommand_WithdrawalFee_LeavesDepositFeeUnchanged()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var currency = new Mock<ICurrency>();
		decimal amount = 12M;
		currency.Setup(x => x.TryGetBaseCurrency("12", out amount)).Returns(true);
		var bank = Mock.Of<IBank>(x => x.Gameworld == world.Object && x.PrimaryCurrency == currency.Object);
		var accountType = new BankAccountType(new MudSharp.Models.BankAccountType
		{
			Id = 1, Name = "Test", CustomerDescription = "Test", WithdrawalFleeFlat = 2M, DepositFeeFlat = 7M
		}, bank);
		var actor = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());

		Assert.IsTrue(accountType.BuildingCommand(actor, new StringStack("withdrawfee 12")));
		Assert.AreEqual(12M, accountType.WithdrawalFleeFlat);
		Assert.AreEqual(7M, accountType.DepositFeeFlat);
		Assert.IsTrue(accountType.Changed);
	}
}
