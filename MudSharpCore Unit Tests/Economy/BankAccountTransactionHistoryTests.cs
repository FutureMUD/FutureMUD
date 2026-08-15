#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Economy.Banking;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BankAccountTransactionHistoryTests
{
	[TestMethod]
	public void LoadMostRecent_ReturnsOnlyTheMostRecentBoundedHistoryForTheRequestedAccount()
	{
		using var context = BuildContext();
		context.BankAccountTransactions.AddRange(
			Enumerable.Range(1, 150)
				.Select(id => Transaction(id, 1L))
				.Concat(Enumerable.Range(151, 150)
					.Select(id => Transaction(id, 2L))));
		context.SaveChanges();

		var history = BankAccountTransactionHistory.LoadMostRecent(context.BankAccountTransactions, 1L);

		Assert.AreEqual(BankAccountTransactionHistory.MaximumTransactionHistoryEntries, history.Count);
		CollectionAssert.AreEqual(Enumerable.Range(51, 100).Reverse().ToList(), history.Select(x => (int)x.Id).ToList());
		Assert.IsTrue(history.All(x => x.BankAccountId == 1L));
	}

	[TestMethod]
	public void LoadOlderThan_ReturnsTheNextBoundedHistoryChunkForTheRequestedAccount()
	{
		using var context = BuildContext();
		context.BankAccountTransactions.AddRange(Enumerable.Range(1, 250).Select(id => Transaction(id, 1L)));
		context.SaveChanges();

		var history = BankAccountTransactionHistory.LoadOlderThan(context.BankAccountTransactions, 1L, 201L);

		Assert.AreEqual(BankAccountTransactionHistory.MaximumTransactionHistoryEntries, history.Count);
		CollectionAssert.AreEqual(Enumerable.Range(101, 100).Reverse().ToList(), history.Select(x => (int)x.Id).ToList());
	}

	[TestMethod]
	public void BankAccountTransactionHistoryCommand_QueuesBoundedBackgroundHydration()
	{
		string source = File.ReadAllText(GetSourcePath("MudSharpCore", "Economy", "Banking", "BankAccount.cs"));

		Assert.IsFalse(source.Contains("LoadTransactions", StringComparison.Ordinal));
		StringAssert.Contains(source, "IBankAccount, ILazyLoadDuringIdleTime");
		StringAssert.Contains(source, "RecordTransaction(new BankAccountTransaction");
		StringAssert.Contains(source, "RequestTransactionHistoryLazyLoad();");
		StringAssert.Contains(source, "LoadOlderThan(FMDB.Context.BankAccountTransactions");
		StringAssert.Contains(source, "Gameworld.SaveManager.AddLazyLoad(this);");
		StringAssert.Contains(source, "from transaction in _displayedTransactions");
	}

	private static MudSharp.Models.BankAccountTransaction Transaction(int id, long bankAccountId)
	{
		return new MudSharp.Models.BankAccountTransaction
		{
			Id = id,
			BankAccountId = bankAccountId,
			TransactionType = 0,
			TransactionTime = "2000-01-01 00:00:00",
			TransactionDescription = "Test transaction"
		};
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
