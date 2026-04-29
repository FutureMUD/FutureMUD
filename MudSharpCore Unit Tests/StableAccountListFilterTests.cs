using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Name;
using MudSharp.Commands.Modules;
using MudSharp.Economy;
using MudSharp.Economy.Stables;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DbStableAccount = MudSharp.Models.StableAccount;
using DbStableAccountUser = MudSharp.Models.StableAccountUser;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StableAccountListFilterTests
{
	[TestMethod]
	public void StableAccountListFilters_StatusAndOverLimitFilters_SelectIntersection()
	{
		var accounts = new List<IStableAccount>
		{
			CreateAccount(1L, "settled", "Alice Owner", 0.0M, 100.0M),
			CreateAccount(2L, "suspended debt", "Ben Owner", -25.0M, 100.0M, isSuspended: true),
			CreateAccount(3L, "suspended overdue", "Cara Owner", -125.0M, 100.0M, isSuspended: true),
			CreateAccount(4L, "working overdue", "Dane Owner", -150.0M, 100.0M)
		};
		var filters = ParseFilters("suspended over limit");

		var matches = filters.Apply(accounts).Select(x => x.Id).ToList();

		CollectionAssert.AreEqual(new List<long> { 3L }, matches);
	}

	[TestMethod]
	public void StableAccountListFilters_BalanceFilters_SelectOwingAndPrepaidAccounts()
	{
		var accounts = new List<IStableAccount>
		{
			CreateAccount(1L, "settled", "Alice Owner", 0.0M, 100.0M),
			CreateAccount(2L, "owing", "Ben Owner", -10.0M, 100.0M),
			CreateAccount(3L, "prepaid", "Cara Owner", 25.0M, 100.0M)
		};

		var owingMatches = ParseFilters("owing").Apply(accounts).Select(x => x.Id).ToList();
		var prepaidMatches = ParseFilters("credit").Apply(accounts).Select(x => x.Id).ToList();

		CollectionAssert.AreEqual(new List<long> { 2L }, owingMatches);
		CollectionAssert.AreEqual(new List<long> { 3L }, prepaidMatches);
	}

	[TestMethod]
	public void StableAccountListFilters_OwnerAndAccountNameFilters_MatchCaseInsensitiveText()
	{
		var accounts = new List<IStableAccount>
		{
			CreateAccount(1L, "Courier House", "Alder Stone", 0.0M, 100.0M),
			CreateAccount(2L, "Merchant House", "Alder Stone", 0.0M, 100.0M),
			CreateAccount(3L, "Courier House", "Birch Reed", 0.0M, 100.0M)
		};
		var filters = ParseFilters("owner \"alder stone\" name courier");

		var matches = filters.Apply(accounts).Select(x => x.Id).ToList();

		CollectionAssert.AreEqual(new List<long> { 1L }, matches);
	}

	[TestMethod]
	public void StableAccountListFilters_SearchFilter_MatchesAuthorisedUsers()
	{
		var accounts = new List<IStableAccount>
		{
			CreateAccount(1L, "Courier House", "Alder Stone", 0.0M, 100.0M, false, "Mira Penn"),
			CreateAccount(2L, "Merchant House", "Birch Reed", 0.0M, 100.0M, false, "Toma Grey")
		};
		var filters = ParseFilters("search mira");

		var matches = filters.Apply(accounts).Select(x => x.Id).ToList();

		CollectionAssert.AreEqual(new List<long> { 1L }, matches);
	}

	[TestMethod]
	public void StableAccountListFilters_UnknownFilter_ReturnsHelpfulError()
	{
		var parsed = EconomyModule.TryParseStableAccountListFilters(
			new StringStack("mystery"),
			out _,
			out var error);

		Assert.IsFalse(parsed);
		StringAssert.Contains(error, "not a valid stable account list filter");
		StringAssert.Contains(error, "stable account list");
	}

	private static EconomyModule.StableAccountListFilterSet ParseFilters(string text)
	{
		var parsed = EconomyModule.TryParseStableAccountListFilters(
			new StringStack(text),
			out var filters,
			out var error);

		Assert.IsTrue(parsed, error);
		return filters;
	}

	private static StableAccount CreateAccount(
		long id,
		string accountName,
		string ownerName,
		decimal balance,
		decimal creditLimit,
		bool isSuspended = false,
		params string[] additionalUsers)
	{
		var gameworld = new Mock<IFuturemud>();
		var cultures = new All<INameCulture>();
		var culture = new Mock<INameCulture>();
		culture.SetupGet(x => x.Id).Returns(1L);
		culture.SetupGet(x => x.Name).Returns("test culture");
		culture.Setup(x => x.NamePattern(It.IsAny<NameStyle>()))
			.Returns(Tuple.Create("{0}", new List<NameUsage> { NameUsage.BirthName }));
		cultures.Add(culture.Object);
		gameworld.SetupGet(x => x.NameCultures).Returns(cultures);

		var stable = new Mock<IStable>();
		stable.SetupGet(x => x.Id).Returns(1L);
		stable.SetupGet(x => x.Name).Returns("test stable");
		stable.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var dbAccount = new DbStableAccount
		{
			Id = id,
			StableId = 1L,
			AccountName = accountName,
			AccountOwnerId = id * 10L,
			AccountOwnerName = NameXml(ownerName),
			Balance = balance,
			CreditLimit = creditLimit,
			IsSuspended = isSuspended
		};
		dbAccount.AccountUsers.Add(new DbStableAccountUser
		{
			StableAccountId = dbAccount.Id,
			AccountUserId = dbAccount.AccountOwnerId,
			AccountUserName = NameXml(ownerName)
		});

		var nextUserId = dbAccount.AccountOwnerId + 1L;
		foreach (var user in additionalUsers)
		{
			dbAccount.AccountUsers.Add(new DbStableAccountUser
			{
				StableAccountId = dbAccount.Id,
				AccountUserId = nextUserId++,
				AccountUserName = NameXml(user)
			});
		}

		return new StableAccount(dbAccount, stable.Object);
	}

	private static string NameXml(string name)
	{
		return new XElement("Name",
			new XAttribute("culture", 1L),
			new XElement("Element", new XAttribute("usage", NameUsage.BirthName), new XCData(name))
		).ToString();
	}
}
