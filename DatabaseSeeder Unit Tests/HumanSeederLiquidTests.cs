#nullable enable

using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HumanSeederLiquidTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SeedBloodAndSweat_UsesWaterWithNonDefaultId(bool unrelatedLiquidAtIdOne)
	{
		using var context = BuildContext();
		context.Liquids.AddRange(
			new Liquid { Id = 65, Name = "water", DisplayColour = "blue" },
			new Liquid { Id = 126, Name = "Blood", DisplayColour = "red" },
			new Liquid { Id = 127, Name = "Sweat", DisplayColour = "yellow" });
		if (unrelatedLiquidAtIdOne)
		{
			context.Liquids.Add(new Liquid { Id = 1, Name = "oil", DisplayColour = "yellow" });
		}
		context.SaveChanges();

		var (blood, sweat) = HumanSeeder.SeedBloodAndSweat(context);
		// The gas helper flushes the pending human liquids in the production path.
		CoreDataSeeder.EnsureBreathableAtmosphere(context);
		context.ChangeTracker.Clear();

		foreach (var id in new[] { blood.Id, sweat.Id })
		{
			var liquid = context.Liquids.Include(x => x.DriedResidue).Single(x => x.Id == id);
			Assert.AreEqual(65L, liquid.SolventId);
			Assert.AreEqual(65L, liquid.DriedResidue.SolventId);
			Assert.IsTrue(context.Liquids.Any(x => x.Id == liquid.SolventId));
		}
		Assert.AreEqual(126L, context.Liquids.Find(blood.Id)!.CountAsId);
		Assert.AreEqual(127L, context.Liquids.Find(sweat.Id)!.CountAsId);
	}

	[TestMethod]
	public void SeedBloodAndSweat_MissingWaterReportsPrerequisiteBeforeAddingRecords()
	{
		using var context = BuildContext();
		var exception = Assert.ThrowsException<InvalidOperationException>(() =>
			HumanSeeder.SeedBloodAndSweat(context));
		StringAssert.Contains(exception.Message, "stock water liquid");
		Assert.AreEqual(0, context.ChangeTracker.Entries().Count());
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}
}
