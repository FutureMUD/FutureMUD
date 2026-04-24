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
	public void SeedDefaultPlanes_SeedsPrimeMaterialAndAstralPlane()
	{
		using FuturemudDatabaseContext context = BuildContext();

		var planes = CoreDataSeeder.SeedDefaultPlanes(context);

		Plane prime = planes["Prime Material"];
		Plane astral = planes["Astral Plane"];
		Assert.IsTrue(prime.IsDefault);
		Assert.IsFalse(astral.IsDefault);
		Assert.AreEqual(2, context.Planes.Count());
		Assert.IsTrue(prime.Alias.Contains("physical"));
		Assert.AreEqual("Astral Plane {0}", astral.RoomNameFormat);
		Assert.IsFalse(string.IsNullOrWhiteSpace(astral.RoomDescriptionAddendum));
	}

	[TestMethod]
	public void SeedDefaultPlanes_DoesNotCreateDuplicatePrimeMaterialOrAstralPlane()
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
		context.Planes.Add(new Plane
		{
			Name = "Astral Plane",
			Alias = "astral",
			Description = "Existing astral.",
			DisplayOrder = 10,
			IsDefault = false
		});
		context.SaveChanges();

		CoreDataSeeder.SeedDefaultPlanes(context);

		Assert.AreEqual(2, context.Planes.Count());
		Assert.AreEqual(1, context.Planes.Count(x => x.IsDefault));
		Assert.AreEqual(1, context.Planes.Count(x => x.Name == "Prime Material"));
		Assert.AreEqual(1, context.Planes.Count(x => x.Name == "Astral Plane"));
	}
}
