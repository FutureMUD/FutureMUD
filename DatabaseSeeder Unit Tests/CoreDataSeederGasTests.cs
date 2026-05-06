#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederGasTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void EnsureStockGases_SeedsBreathableVariantsAndEconomicCatalogue()
	{
		using FuturemudDatabaseContext context = BuildContext();

		CoreDataSeeder.EnsureStockGases(context);

		Assert.IsTrue(CoreDataSeeder.StockBreathableAtmosphereGasNamesForTesting.Count >= 10,
			"Expected several stock breathable atmosphere variants.");
		Assert.IsTrue(CoreDataSeeder.StockEconomicGasNamesForTesting.Count >= 30,
			"Expected at least 30 non-atmosphere stock gases.");
		Assert.IsTrue(CoreDataSeeder.StockEconomicGasNamesForTesting.Count <= 50,
			"Expected no more than 50 non-atmosphere stock gases in this first pass.");
		Assert.AreEqual(CoreDataSeeder.StockGasNamesForTesting.Count, context.Gases.Count());

		foreach (string gasName in CoreDataSeeder.StockGasNamesForTesting)
		{
			Assert.AreEqual(1, context.Gases.Count(x => x.Name == gasName),
				$"Expected a single stock gas named {gasName}.");
		}

		Dictionary<string, Gas> gases = context.Gases.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		Gas breathableAtmosphere = gases["Breathable Atmosphere"];
		Assert.AreEqual(1L, breathableAtmosphere.Id);
		Assert.IsNull(breathableAtmosphere.CountAsId);
		Assert.IsNull(breathableAtmosphere.CountsAsQuality);

		foreach (string gasName in CoreDataSeeder.StockBreathableAtmosphereGasNamesForTesting
			         .Where(x => !x.Equals("Breathable Atmosphere", StringComparison.OrdinalIgnoreCase)))
		{
			Assert.AreEqual(breathableAtmosphere.Id, gases[gasName].CountAsId,
				$"Expected {gasName} to count as the canonical breathable atmosphere.");
			Assert.IsNotNull(gases[gasName].CountsAsQuality,
				$"Expected {gasName} to declare a breathable quality multiplier.");
		}

		Assert.AreEqual((int)ItemQuality.Great, gases["High Altitude Breathable Atmosphere"].CountsAsQuality);
		Assert.AreEqual((int)ItemQuality.Standard, gases["Very Thin Breathable Atmosphere"].CountsAsQuality);
		Assert.AreEqual((int)ItemQuality.Legendary, gases["Oxygen-Enriched Breathable Atmosphere"].CountsAsQuality);
		Assert.IsTrue(gases["High-Oxygen Breathable Atmosphere"].OxidationFactor > breathableAtmosphere.OxidationFactor);

		foreach (string gasName in new[]
		         {
			         "Hydrogen", "Chlorine", "Nitrogen", "Natural Gas", "Acetylene", "Carbon Dioxide", "Propane",
			         "Butane", "Neon", "Argon", "Ethylene", "Nitrous Oxide"
		         })
		{
			Assert.IsTrue(gases.ContainsKey(gasName), $"Expected stock economic gas {gasName} to be seeded.");
		}

		Assert.AreEqual(gases["Methane"].Id, gases["Natural Gas"].CountAsId);
		Assert.AreEqual((int)ItemQuality.Good, gases["Natural Gas"].CountsAsQuality);
		Assert.AreEqual(gases["Propane"].Id, gases["Liquefied Petroleum Gas"].CountAsId);
		Assert.AreEqual(gases["Butane"].Id, gases["Isobutane"].CountAsId);

		foreach (string tagName in new[]
		         {
			         "Gases", "Breathable Atmospheres", "Industrial Gases", "Elemental Gases", "Fuel Gases",
			         "Medical Gases", "Noble Gases", "Refrigerant Gases"
		         })
		{
			Assert.AreEqual(1, context.Tags.Count(x => x.Name == tagName),
				$"Expected a single stock gas tag named {tagName}.");
		}
	}

	[TestMethod]
	public void EnsureStockGases_RerunPreservesIdsAndDoesNotDuplicateTags()
	{
		using FuturemudDatabaseContext context = BuildContext();

		CoreDataSeeder.EnsureStockGases(context);
		Dictionary<string, long> originalIds = context.Gases.ToDictionary(x => x.Name, x => x.Id);
		CoreDataSeeder.EnsureStockGases(context);

		Assert.AreEqual(CoreDataSeeder.StockGasNamesForTesting.Count, context.Gases.Count());
		foreach ((string gasName, long gasId) in originalIds)
		{
			Assert.AreEqual(gasId, context.Gases.Single(x => x.Name == gasName).Id,
				$"Expected rerun to preserve the ID for {gasName}.");
		}

		Assert.IsFalse(context.GasesTags
			.GroupBy(x => new { x.GasId, x.TagId })
			.Any(x => x.Count() > 1), "Expected rerun not to duplicate gas tag links.");
	}
}
