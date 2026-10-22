#nullable enable

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class OwnedInMemorySeederContextTests
{
	private static readonly InMemoryDatabaseRoot Stores = new();

	private static DbContextOptions<FuturemudDatabaseContext> Options() =>
		new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), Stores, x => x.EnableNullChecks(false)).Options;

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public async Task OwnerDisposal_ReleasesOnlyItsStore_WhileBorrowersSharePersistedChanges(bool asynchronous)
	{
		var options = Options();
		using var owner = new OwnedInMemorySeederContext(options);
		using var independent = new OwnedInMemorySeederContext(Options());
		owner.Accounts.Add(new Account { Name = "owned" });
		owner.SaveChanges();
		independent.Accounts.Add(new Account { Name = "independent" });
		independent.SaveChanges();
		var factory = ArmageddonInstallationSettingsTests.Factory(owner);
		using (var borrower = factory())
		{
			var account = borrower.Accounts.Single();
			Assert.AreEqual("owned", account.Name);
			account.Name = "changed through a fresh context";
			borrower.SaveChanges();
		}
		using (var reopened = factory())
		{
			Assert.AreEqual("changed through a fresh context", reopened.Accounts.Single().Name,
				"Disposing a borrowed context must not delete the owner's store.");
		}

		if (asynchronous) await owner.DisposeAsync();
		else owner.Dispose();
		// Mixed repeated disposal must neither access a disposed context nor clear another store.
		owner.Dispose();
		await owner.DisposeAsync();
		using var afterDisposal = new FuturemudDatabaseContext(options);
		Assert.AreEqual(0, afterDisposal.Accounts.Count(), "Disposed fixture rows must no longer be rooted by EF's store cache.");
		Assert.AreEqual("independent", independent.Accounts.Single().Name);
	}

	[TestMethod]
	public void Constructor_RejectsRelationalOptionsWithoutOpeningOrDeletingADatabase()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql("Server=127.0.0.1;Database=must_not_connect", new MySqlServerVersion(new Version(8, 0, 45)))
			.Options;
		Assert.ThrowsException<ArgumentException>(() => new OwnedInMemorySeederContext(options));
	}
}
