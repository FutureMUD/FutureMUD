#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArenaSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context)
	{
		context.Currencies.Add(new Currency
		{
			Id = 1,
			Name = "Bits",
			BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
		});

		context.EconomicZones.Add(new EconomicZone
		{
			Id = 1,
			Name = "Arena Zone",
			CurrencyId = 1,
			ReferenceTime = "UTC 0:0:0"
		});

		context.SaveChanges();
	}

	[TestMethod]
	public void SeedData_InstallsAndRerunsWithoutDuplicatingArenasOrEvents()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new ArenaSeeder();
		var prior0 = context.Arenas.Select(x => x.Id).ToArray();
		var prior1 = context.ArenaCombatantClasses.Select(x => x.Id).ToArray();
		var prior2 = context.ArenaEventTypes.Select(x => x.Id).ToArray();
		seeder.SeedData(context, new Dictionary<string, string>());
		var installed0 = context.Arenas.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed0.Except(prior0).Any(), "The first installation must create records beyond its prerequisites.");
		var installed1 = context.ArenaCombatantClasses.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed1.Except(prior1).Any(), "The first installation must create records beyond its prerequisites.");
		var installed2 = context.ArenaEventTypes.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(installed2.Except(prior2).Any(), "The first installation must create records beyond its prerequisites.");
		seeder.SeedData(context, new Dictionary<string, string>());
		CollectionAssert.AreEqual(installed0, context.Arenas.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed1, context.ArenaCombatantClasses.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(installed2, context.ArenaEventTypes.Select(x => x.Id).OrderBy(x => x).ToArray());
	}

	[TestMethod]
	public void ShouldSeedData_MissingStockArenaPiece_ReturnsExtraPackagesAvailable_AndRerunRepairsIt()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ArenaSeeder seeder = new();

		seeder.SeedData(context, new Dictionary<string, string>());
		MudSharp.Models.ArenaEventType squad = context.ArenaEventTypes.Single(x => x.Name == "Squad Skirmish");
		context.ArenaEventTypes.Remove(squad);
		context.SaveChanges();

		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));

		seeder.SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual(1, context.ArenaEventTypes.Count(x => x.Name == "Squad Skirmish"));
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}
}
