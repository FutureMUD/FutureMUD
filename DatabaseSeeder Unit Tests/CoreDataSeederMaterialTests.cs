#nullable enable

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederMaterialTests
{
    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static void SeedMaterials(FuturemudDatabaseContext context)
    {
        CoreDataSeeder seeder = new();
        typeof(CoreDataSeeder)
            .GetMethod("SeedMaterials", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(seeder, [context]);
    }

    private static Mock<IFuturemud> CreateGameworld(All<ISolid> materials, FuturemudDatabaseContext context)
    {
        All<ITag> tags = new();
        foreach (MudSharp.Models.Tag tag in context.Tags)
        {
            Mock<ITag> mockTag = new();
            mockTag.SetupGet(x => x.Id).Returns(tag.Id);
            mockTag.SetupGet(x => x.Name).Returns(tag.Name);
            mockTag.SetupGet(x => x.FrameworkItemType).Returns("Tag");
            mockTag.SetupGet(x => x.FullName).Returns(tag.Name);
            tags.Add(mockTag.Object);
        }

        Mock<IFuturemud> gameworld = new();
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
        using FuturemudDatabaseContext context = BuildContext();
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
        using FuturemudDatabaseContext context = BuildContext();
        SeedMaterials(context);

        All<ISolid> materials = new();
        Mock<IFuturemud> gameworld = CreateGameworld(materials, context);
        foreach (MudSharp.Models.Material dbMaterial in context.Materials
                     .Include(x => x.MaterialAliases)
                     .Include(x => x.MaterialsTags))
        {
            materials.Add(new Solid(dbMaterial, gameworld.Object));
        }

        Assert.AreEqual("mild steel", materials.GetByName("steel")?.Name);
        Assert.AreEqual("high-density polyethylene", materials.GetByName("hdpe")?.Name);
    }
}
