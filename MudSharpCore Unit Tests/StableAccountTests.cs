using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Economy;
using MudSharp.Economy.Stables;
using MudSharp.Framework;
using System.Xml.Linq;
using DbStableAccount = MudSharp.Models.StableAccount;
using DbStableAccountUser = MudSharp.Models.StableAccountUser;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StableAccountTests
{
	[TestMethod]
	public void IsAuthorisedToUse_PositiveBalanceAndCreditLimit_AllowsPrepaidAndCreditSpend()
	{
		var account = CreateAccount(balance: 50.0M, creditLimit: 100.0M);
		var actor = CreateActor(10L);

		var result = account.IsAuthorisedToUse(actor.Object, 150.0M);

		Assert.AreEqual(StableAccountAuthorisationFailureReason.None, result);
	}

	[TestMethod]
	public void IsAuthorisedToUse_ChargeBeyondCreditLimit_RejectsAccount()
	{
		var account = CreateAccount(balance: 50.0M, creditLimit: 100.0M);
		var actor = CreateActor(10L);

		var result = account.IsAuthorisedToUse(actor.Object, 151.0M);

		Assert.AreEqual(StableAccountAuthorisationFailureReason.AccountOverbalanced, result);
	}

	[TestMethod]
	public void IsAuthorisedToUse_ChargeBeyondUserLimit_RejectsUser()
	{
		var account = CreateAccount(balance: 50.0M, creditLimit: 100.0M, spendingLimit: 25.0M);
		var actor = CreateActor(10L);

		var result = account.IsAuthorisedToUse(actor.Object, 30.0M);

		Assert.AreEqual(StableAccountAuthorisationFailureReason.UserOverbalanced, result);
	}

	[TestMethod]
	public void MaximumAuthorisedToUse_UsesLowerOfSpendingLimitAndCreditAvailable()
	{
		var account = CreateAccount(balance: 40.0M, creditLimit: 100.0M, spendingLimit: 25.0M);
		var actor = CreateActor(10L);

		var result = account.MaximumAuthorisedToUse(actor.Object);

		Assert.AreEqual(25.0M, result);
	}

	private static StableAccount CreateAccount(decimal balance, decimal creditLimit, decimal? spendingLimit = null)
	{
		var gameworld = new Mock<IFuturemud>();
		var cultures = new All<INameCulture>();
		var culture = new Mock<INameCulture>();
		culture.SetupGet(x => x.Id).Returns(1L);
		culture.SetupGet(x => x.Name).Returns("test culture");
		cultures.Add(culture.Object);
		gameworld.SetupGet(x => x.NameCultures).Returns(cultures);

		var stable = new Mock<IStable>();
		stable.SetupGet(x => x.Id).Returns(1L);
		stable.SetupGet(x => x.Name).Returns("test stable");
		stable.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var dbAccount = new DbStableAccount
		{
			Id = 20L,
			StableId = 1L,
			AccountName = "traveller",
			AccountOwnerId = 10L,
			AccountOwnerName = NameXml("Owner"),
			Balance = balance,
			CreditLimit = creditLimit,
			IsSuspended = false
		};
		dbAccount.AccountUsers.Add(new DbStableAccountUser
		{
			StableAccountId = dbAccount.Id,
			AccountUserId = 10L,
			AccountUserName = NameXml("Owner"),
			SpendingLimit = spendingLimit
		});

		return new StableAccount(dbAccount, stable.Object);
	}

	private static Mock<ICharacter> CreateActor(long id)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(id);
		return actor;
	}

	private static string NameXml(string name)
	{
		return new XElement("Name",
			new XAttribute("culture", 1L),
			new XElement("Element", new XAttribute("usage", NameUsage.BirthName), new XCData(name))
		).ToString();
	}
}
