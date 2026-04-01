#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederMaterialTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedMaterials(FuturemudDatabaseContext context)
	{
		var seeder = new CoreDataSeeder();
		typeof(CoreDataSeeder)
			.GetMethod("SeedMaterials", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(seeder, [context]);
	}

	private static Mock<IFuturemud> CreateGameworld(All<ISolid> materials, FuturemudDatabaseContext context)
	{
		var tags = new All<ITag>();
		foreach (var tag in context.Tags)
		{
			var mockTag = new Mock<ITag>();
			mockTag.SetupGet(x => x.Id).Returns(tag.Id);
			mockTag.SetupGet(x => x.Name).Returns(tag.Name);
			mockTag.SetupGet(x => x.FrameworkItemType).Returns("Tag");
			mockTag.SetupGet(x => x.FullName).Returns(tag.Name);
			tags.Add(mockTag.Object);
		}

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Materials).Returns(materials);
		gameworld.SetupGet(x => x.Tags).Returns(tags);
		gameworld.SetupGet(x => x.Liquids).Returns(new All<ILiquid>());
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		gameworld.Setup(x => x.Add(It.IsAny<IMaterial>()));
		return gameworld;
	}

	[TestMethod]
	public void SeedMaterials_SeedsAliasesAndCommonMissingMaterials()
	{
		using var context = BuildContext();
		SeedMaterials(context);

		Assert.IsTrue(context.Materials.Any(x => x.Name == "ABS plastic"));
		Assert.IsTrue(context.Materials.Any(x => x.Name == "polycarbonate"));
		Assert.IsTrue(context.Materials.Any(x => x.Name == "tempered glass"));
		Assert.IsTrue(context.Materials.Any(x => x.Name == "tool steel"));
		Assert.IsTrue(context.Materials.Any(x => x.Name == "spring steel"));

		Assert.IsTrue(context.MaterialAliases.Any(x => x.Material.Name == "mild steel" && x.Alias == "steel"));
		Assert.IsTrue(context.MaterialAliases.Any(x => x.Material.Name == "high-density polyethylene" && x.Alias == "hdpe"));
	}

	[TestMethod]
	public void SeededMaterials_AliasLookupFindsExpectedDefaults()
	{
		using var context = BuildContext();
		SeedMaterials(context);

		var materials = new All<ISolid>();
		var gameworld = CreateGameworld(materials, context);
		foreach (var dbMaterial in context.Materials
			         .Include(x => x.MaterialAliases)
			         .Include(x => x.MaterialsTags))
		{
			materials.Add(new Solid(dbMaterial, gameworld.Object));
		}

		Assert.AreEqual("mild steel", materials.GetByName("steel")?.Name);
		Assert.AreEqual("high-density polyethylene", materials.GetByName("hdpe")?.Name);
	}
}
