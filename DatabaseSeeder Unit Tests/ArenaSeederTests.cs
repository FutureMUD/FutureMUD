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
	public void SeedData_CreatesExpandedArenaFormats_AndRemainsIdempotent()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedPrerequisites(context);
		ArenaSeeder seeder = new();

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));

		seeder.SeedData(context, new Dictionary<string, string>());
		seeder.SeedData(context, new Dictionary<string, string>());

		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
		Assert.AreEqual(1, context.Arenas.Count(x => x.Name == "Grand Coliseum"));
		CollectionAssert.AreEquivalent(
			new[] { "Gladiator", "Boxer", "Wrestler", "Arena Animal" },
			context.ArenaCombatantClasses.Select(x => x.Name).ToArray());
		CollectionAssert.AreEquivalent(
			new[]
			{
				"Duel",
				"Team Skirmish",
				"Squad Skirmish",
				"Boxing Match",
				"Wrestling Match",
				"Champion Challenge",
				"Animal Bohort"
			},
			context.ArenaEventTypes.Select(x => x.Name).ToArray());
		CollectionAssert.AreEquivalent(
			new[]
			{
				"ArenaAlwaysEligible",
				"ArenaAnimalEligible",
				"ArenaDefaultOutfit",
				"ArenaStandardIntro",
				"ArenaBoxingScoring",
				"ArenaAnimalNpcLoader"
			},
			context.FutureProgs.Select(x => x.FunctionName).ToArray());

		MudSharp.Models.ArenaEventType boxingEvent = context.ArenaEventTypes.Single(x => x.Name == "Boxing Match");
		Assert.IsFalse(boxingEvent.BringYourOwn);
		Assert.AreEqual((int)MudSharp.Arenas.ArenaEliminationMode.PointsElimination, boxingEvent.EliminationMode);
		Assert.AreEqual("ArenaBoxingScoring",
			context.FutureProgs.Single(x => x.Id == boxingEvent.ScoringProgId).FunctionName);

		MudSharp.Models.ArenaEventType wrestlingEvent = context.ArenaEventTypes.Single(x => x.Name == "Wrestling Match");
		Assert.IsFalse(wrestlingEvent.BringYourOwn);
		Assert.AreEqual((int)MudSharp.Arenas.ArenaEliminationMode.Knockout, wrestlingEvent.EliminationMode);

		MudSharp.Models.ArenaEventType championEvent = context.ArenaEventTypes.Single(x => x.Name == "Champion Challenge");
		ArenaEventTypeSide championSide = context.ArenaEventTypeSides
			.Single(x => x.ArenaEventTypeId == championEvent.Id && x.Index == 1);
		Assert.AreEqual((int)MudSharp.Arenas.ArenaSidePolicy.Closed, championSide.Policy);
		Assert.IsFalse(championSide.AllowNpcSignup);
		Assert.IsTrue(championSide.AutoFillNpc);
		Assert.AreEqual(1800.0m, championSide.MinimumRating);

		long championClassId = context.ArenaEventTypeSideAllowedClasses
			.Single(x => x.ArenaEventTypeSideId == championSide.Id)
			.ArenaCombatantClassId;
		Assert.AreEqual("Gladiator", context.ArenaCombatantClasses.Single(x => x.Id == championClassId).Name);

		MudSharp.Models.ArenaEventType animalEvent = context.ArenaEventTypes.Single(x => x.Name == "Animal Bohort");
		List<ArenaEventTypeSide> animalSides = context.ArenaEventTypeSides
			.Where(x => x.ArenaEventTypeId == animalEvent.Id)
			.OrderBy(x => x.Index)
			.ToList();
		Assert.AreEqual(2, animalSides.Count);
		CollectionAssert.AreEqual(new[] { 2, 2 }, animalSides.Select(x => x.Capacity).ToArray());
		Assert.IsTrue(animalEvent.BringYourOwn);
		Assert.IsTrue(animalSides.All(x => x.AutoFillNpc));
		CollectionAssert.AreEquivalent(
			new[] { "ArenaAnimalNpcLoader" },
			animalSides.Select(x => context.FutureProgs.Single(y => y.Id == x.NpcLoaderProgId).FunctionName).Distinct().ToArray());

		CollectionAssert.AreEqual(
			new[] { 1, 1 },
			context.ArenaEventTypeSides
				.Where(x => x.ArenaEventTypeId == context.ArenaEventTypes.Single(y => y.Name == "Duel").Id)
				.OrderBy(x => x.Index)
				.Select(x => x.Capacity)
				.ToArray());
		CollectionAssert.AreEqual(
			new[] { 2, 2 },
			context.ArenaEventTypeSides
				.Where(x => x.ArenaEventTypeId == context.ArenaEventTypes.Single(y => y.Name == "Team Skirmish").Id)
				.OrderBy(x => x.Index)
				.Select(x => x.Capacity)
				.ToArray());
		CollectionAssert.AreEqual(
			new[] { 3, 3 },
			context.ArenaEventTypeSides
				.Where(x => x.ArenaEventTypeId == context.ArenaEventTypes.Single(y => y.Name == "Squad Skirmish").Id)
				.OrderBy(x => x.Index)
				.Select(x => x.Capacity)
				.ToArray());

		FutureProg animalEligibility = context.FutureProgs.Single(x => x.FunctionName == "ArenaAnimalEligible");
		Assert.AreEqual("return isanimal(@character);", animalEligibility.FunctionText);
		FutureProg boxingScoring = context.FutureProgs.Single(x => x.FunctionName == "ArenaBoxingScoring");
		Assert.AreEqual(
			"return arenaboxingscores(@sideIndices, @scoringAttackerSides, @landedHits, @undefendedHits, @impactLocations);",
			boxingScoring.FunctionText);
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
