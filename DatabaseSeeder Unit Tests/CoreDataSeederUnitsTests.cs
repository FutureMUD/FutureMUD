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
	public void SeedUnitsOfMeasure_SeedsImperialUkAndCorrectsLegacyIssues()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedUnitsOfMeasure(context);

		Assert.IsTrue(context.UnitsOfMeasure.Any(x => x.System == "Imperial-UK"));
		Assert.IsTrue(context.UnitsOfMeasure.Any(x => x.System == "Imperial" && x.Name == "short ton"));
		Assert.IsFalse(context.UnitsOfMeasure.Any(x => x.System == "Imperial" && x.Name == "tonne"));
		Assert.AreEqual("ML", context.UnitsOfMeasure.Single(x => x.System == "Metric" && x.Name == "megalitre").PrimaryAbbreviation);
		Assert.AreEqual("ug", context.UnitsOfMeasure.Single(x => x.System == "Metric" && x.Name == "microgram").PrimaryAbbreviation);

		UnitOfMeasure imperialThou = context.UnitsOfMeasure.Single(x => x.System == "Imperial" && x.Name == "thou");
		UnitOfMeasure imperialBmi = context.UnitsOfMeasure.Single(x => x.System == "Imperial" && x.Type == (int)UnitType.BMI);
		UnitOfMeasure usGallon = context.UnitsOfMeasure.Single(x => x.System == "Imperial" && x.Name == "gallon");
		UnitOfMeasure ukGallon = context.UnitsOfMeasure.Single(x => x.System == "Imperial-UK" && x.Name == "gallon");

		Assert.AreEqual((int)UnitType.Length, imperialThou.Type);
		Assert.AreEqual(703.06957964, imperialBmi.BaseMultiplier, 0.0000001);
		Assert.AreEqual(4.54609, ukGallon.BaseMultiplier, 0.0000001);
		Assert.IsTrue(ukGallon.BaseMultiplier > usGallon.BaseMultiplier);
	}

	[TestMethod]
	public void SeedUnitsOfMeasure_SeedsExpectedDisplayDefaultsPerSystem()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedUnitsOfMeasure(context);

		string[] systems = context.UnitsOfMeasure
			.Select(x => x.System)
			.Distinct()
			.OrderBy(x => x)
			.ToArray();

		CollectionAssert.AreEqual(new[] { "Imperial", "Imperial-UK", "Metric" }, systems);
		Assert.IsTrue(context.UnitsOfMeasure.Any(x => x.System == "Imperial-UK" && x.DefaultUnitForSystem && x.Name == "pint"));
		Assert.IsTrue(context.UnitsOfMeasure.Any(x => x.System == "Imperial-UK" && x.DefaultUnitForSystem && x.Name == "celsius"));
		Assert.IsTrue(context.UnitsOfMeasure.Any(x => x.System == "Imperial" && x.DefaultUnitForSystem && x.Name == "fahrenheit"));
		Assert.IsTrue(context.UnitsOfMeasure.Any(x => x.System == "Metric" && x.DefaultUnitForSystem && x.Name == "kg/m2"));
	}
}
