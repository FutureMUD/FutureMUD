#nullable enable

using System;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitManagedLookupTests
{
	[TestMethod]
	public void LookupTracksNewAndDeletedOwnershipAndDoesNotLeakBetweenRuns()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var record = new SeederManagedRecord { Seeder = "CultureSeeder", EntityType = "Language", StableKey = "test", Module = "renaissance" };
		using (CultureToolkitManagedEntities.BeginLookupScope(context))
		{
			Assert.IsNull(CultureToolkitManagedEntities.Find(context, "Language", "test"));
			context.Add(record);
			Assert.AreSame(record, CultureToolkitManagedEntities.Find(context, "Language", "test"));
			context.SaveChanges();
			Assert.AreSame(record, CultureToolkitManagedEntities.Find(context, "Language", "test"));
			context.Remove(record);
			Assert.IsNull(CultureToolkitManagedEntities.Find(context, "Language", "test"));
			context.SaveChanges();
		}
		context.Add(new SeederManagedRecord { Seeder = "CultureSeeder", EntityType = "Language", StableKey = "later", Module = "renaissance" });
		context.SaveChanges();
		using (CultureToolkitManagedEntities.BeginLookupScope(context))
		{
			Assert.IsNull(CultureToolkitManagedEntities.Find(context, "Language", "test"));
			Assert.IsNotNull(CultureToolkitManagedEntities.Find(context, "Language", "later"));
		}
	}
}
