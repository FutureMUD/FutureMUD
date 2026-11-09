#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework.Units;
using MudSharp.Models;
using System;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederUnitsTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedUnitsOfMeasure(FuturemudDatabaseContext context)
	{
		typeof(CoreDataSeeder)
			.GetMethod("SeedUnitsOfMeasure", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [context]);
		context.SaveChanges();
	}

	[TestMethod]
	public void SeedUnitsOfMeasure_RerunReusesFixedIdentifiersAndRefreshesStockFields()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedUnitsOfMeasure(context);
		int count = context.UnitsOfMeasure.Count();
		context.UnitsOfMeasure.Find(1L)!.PrimaryAbbreviation = "broken";
		context.SaveChanges();

		SeedUnitsOfMeasure(context);

		Assert.AreEqual(count, context.UnitsOfMeasure.Count());
		Assert.AreEqual("g", context.UnitsOfMeasure.Find(1L)!.PrimaryAbbreviation);
	}
}
