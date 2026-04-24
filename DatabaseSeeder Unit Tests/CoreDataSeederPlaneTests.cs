#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederPlaneTests
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
	public void SeedDefaultPlane_SeedsPrimeMaterialAsDefault()
	{
		using FuturemudDatabaseContext context = BuildContext();

		Plane plane = CoreDataSeeder.SeedDefaultPlane(context);

		Assert.AreEqual("Prime Material", plane.Name);
		Assert.IsTrue(plane.IsDefault);
		Assert.AreEqual(1, context.Planes.Count());
		Assert.IsTrue(context.Planes.Single().Alias.Contains("physical"));
	}

	[TestMethod]
	public void SeedDefaultPlane_DoesNotCreateDuplicatePrimeMaterial()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.Planes.Add(new Plane
		{
			Name = "Prime Material",
			Alias = "prime material",
			Description = "Existing default.",
			DisplayOrder = 0,
			IsDefault = true
		});
		context.SaveChanges();

		CoreDataSeeder.SeedDefaultPlane(context);

		Assert.AreEqual(1, context.Planes.Count());
		Assert.IsTrue(context.Planes.Single().IsDefault);
	}
}
