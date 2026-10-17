#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	private static readonly ConcurrentDictionary<(ArmageddonProgressionMode Mode, bool Classes), Lazy<InMemorySeederSnapshot>> Prerequisites = new();

	private static InMemorySeederSnapshot Snapshot(ArmageddonProgressionMode mode, bool classes) =>
		Prerequisites.GetOrAdd((mode, classes), key => new Lazy<InMemorySeederSnapshot>(() =>
		{
			using var db = FreshFixture(key.Mode, key.Classes);
			return InMemorySeederSnapshot.Capture(db);
		})).Value;

	private static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode = ArmageddonProgressionMode.Simple, bool classes = false) =>
		Snapshot(mode, classes).Create();

	[TestMethod]
	public void PrerequisiteSnapshot_PreservesPopulatedSharedTablesAndShadowForeignKeys()
	{
		using var source = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		source.FutureProgs.Add(new FutureProg { Id = 501, FunctionName = "snapshot control" });
		var resource = new ChargenResource { Id = 41, Name = "snapshot resource" };
		source.ChargenResources.Add(resource);
		source.Entry(resource).Property("FK_ChargenResources_FutureProgs").CurrentValue = 501L;
		source.Languages.Add(new Language { Id = 7, Name = "snapshot language" });
		source.Accents.Add(new Accent { Id = 9, Name = "snapshot accent", LanguageId = 7 });
		source.Set<Dictionary<string, object>>("AccentsAssociatedLanguages")
			.Add(new() { ["AccentId"] = 9L, ["LanguageId"] = 7L });
		source.SaveChanges();
		var snapshot = InMemorySeederSnapshot.Capture(source);
		using var clone = snapshot.Create();
		var copied = clone.ChargenResources.Include(x => x.ControlProg).Single();
		Assert.AreEqual(501L, clone.Entry(copied).Property("FK_ChargenResources_FutureProgs").CurrentValue);
		Assert.AreEqual(501L, copied.ControlProg.Id);
		var association = clone.Set<Dictionary<string, object>>("AccentsAssociatedLanguages").Single();
		Assert.AreEqual(9L, association["AccentId"]);
		Assert.AreEqual(7L, association["LanguageId"]);
		Assert.AreEqual(snapshot.Fingerprint, InMemorySeederSnapshot.Capture(clone).Fingerprint);
	}

	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void PrerequisiteSnapshot_PreservesEveryScalarAndIsolatesMutations(ArmageddonProgressionMode mode)
	{
		var snapshot = Snapshot(mode, false);
		using var first = snapshot.Create();
		using var second = snapshot.Create();
		Assert.AreEqual(snapshot.Fingerprint, InMemorySeederSnapshot.Capture(first).Fingerprint);
		Assert.AreEqual(snapshot.Fingerprint, InMemorySeederSnapshot.Capture(second).Fingerprint);
		Assert.IsTrue(first.ChangeTracker.AutoDetectChangesEnabled);
		var account = first.Accounts.First();
		var independentAccount = second.Accounts.Single(x => x.Id == account.Id);
		Assert.AreNotSame(account, independentAccount);
		var originalName = independentAccount.Name;
		account.Name = "mutated in one fixture only";
		var overlay = first.RoomOverlays.Include(x => x.Room).Include(x => x.RoomOverlayPackage).Single(x => x.Id == 74);
		Assert.AreEqual(73L, overlay.Room.Id);
		Assert.AreEqual(11L, overlay.RoomOverlayPackage.Id);
		overlay.RoomOverlayPackage.Name = "mutated navigation";
		first.SeederManagedRecords.Remove(first.SeederManagedRecords.First());
		var largestId = first.Accounts.Max(x => x.Id);
		var added = new Account { Name = "new generated identity" };
		first.Accounts.Add(added);
		first.SaveChanges();
		Assert.IsTrue(added.Id > largestId, "Explicitly restored keys must advance the store's value generator.");
		Assert.AreEqual(originalName, independentAccount.Name);
		second.ChangeTracker.Clear();
		Assert.AreEqual(snapshot.Fingerprint, InMemorySeederSnapshot.Capture(second).Fingerprint);
		using var third = snapshot.Create();
		Assert.AreEqual(snapshot.Fingerprint, InMemorySeederSnapshot.Capture(third).Fingerprint);
		Assert.AreNotEqual(snapshot.Fingerprint, InMemorySeederSnapshot.Capture(first).Fingerprint);
	}
}
