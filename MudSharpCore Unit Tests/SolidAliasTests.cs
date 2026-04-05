#nullable enable

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
using MaterialModel = MudSharp.Models.Material;
using TagModel = MudSharp.Models.Tag;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SolidAliasTests
{
    private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static Solid CreateSolid(long id, string name, IFuturemud gameworld, params string[] aliases)
    {
        MaterialModel material = new()
        {
            Id = id,
            Name = name,
            MaterialDescription = name,
            ResidueColour = "white",
            MaterialAliases = aliases.Select(x => new MaterialAlias
            {
                MaterialId = id,
                Alias = x
            }).ToList()
        };

        return new Solid(material, gameworld);
    }

    private static Mock<IFuturemud> CreateGameworld(All<ISolid> materials, IEnumerable<TagModel>? tags = null)
    {
        All<ITag> tagCollection = new();
        foreach (TagModel tag in tags ?? Enumerable.Empty<TagModel>())
        {
            Mock<ITag> mockTag = new();
            mockTag.SetupGet(x => x.Id).Returns(tag.Id);
            mockTag.SetupGet(x => x.Name).Returns(tag.Name);
            mockTag.SetupGet(x => x.FrameworkItemType).Returns("Tag");
            mockTag.SetupGet(x => x.FullName).Returns(tag.Name);
            tagCollection.Add(mockTag.Object);
        }

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.Materials).Returns(materials);
        gameworld.SetupGet(x => x.Tags).Returns(tagCollection);
        gameworld.SetupGet(x => x.Liquids).Returns(new All<ILiquid>());
        gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
        gameworld.Setup(x => x.Add(It.IsAny<IMaterial>()));
        return gameworld;
    }

    private static FMDBState CaptureFMDBState()
    {
        return new FMDBState(
            (FuturemudDatabaseContext?)typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null),
            typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.GetValue(null),
            (uint)typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
                .GetValue(null)!);
    }

    private static void RestoreFMDBState(FMDBState state)
    {
        typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Context);
        typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Connection);
        typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, state.InstanceCount);
    }

    private static void PrimeFMDB(FuturemudDatabaseContext context)
    {
        typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, context);
        typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, null);
        typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, 1u);
    }

    [TestMethod]
    public void Names_PrimaryNameThenAliases_InOrder()
    {
        All<ISolid> materials = new();
        Mock<IFuturemud> gameworld = CreateGameworld(materials);
        Solid solid = CreateSolid(1, "mild steel", gameworld.Object, "steel", "ms");

        CollectionAssert.AreEqual(new[] { "mild steel", "steel", "ms" }, solid.Names.ToList());
    }

    [TestMethod]
    public void TryAddAlias_CollidingNameOrAlias_IsRejected()
    {
        All<ISolid> materials = new();
        Mock<IFuturemud> gameworld = CreateGameworld(materials);
        Solid mildSteel = CreateSolid(1, "mild steel", gameworld.Object);
        Solid hdpe = CreateSolid(2, "high-density polyethylene", gameworld.Object, "plastic");
        materials.Add(mildSteel);
        materials.Add(hdpe);

        Assert.IsFalse(mildSteel.TryAddAlias("high-density polyethylene", out _));
        Assert.IsFalse(mildSteel.TryAddAlias("plastic", out _));
        Assert.IsFalse(mildSteel.Aliases.Any());
    }

    [TestMethod]
    public void TryRename_DoesNotPreserveOldNameAsAlias()
    {
        All<ISolid> materials = new();
        Mock<IFuturemud> gameworld = CreateGameworld(materials);
        Solid solid = CreateSolid(1, "mild steel", gameworld.Object);
        materials.Add(solid);

        bool renamed = solid.TryRename("construction steel", out string? errorMessage);

        Assert.IsTrue(renamed, errorMessage);
        Assert.AreEqual("construction steel", solid.Name);
        Assert.IsFalse(solid.Names.Contains("mild steel"));
    }

    [TestMethod]
    public void Clone_StartsWithoutCopiedAliases()
    {
        FMDBState state = CaptureFMDBState();
        using FuturemudDatabaseContext context = BuildContext();
        try
        {
            PrimeFMDB(context);
            All<ISolid> materials = new();
            Mock<IFuturemud> gameworld = CreateGameworld(materials);
            Solid solid = CreateSolid(1, "high-density polyethylene", gameworld.Object, "hdpe");
            materials.Add(solid);

            ISolid clone = solid.Clone("copied polyethylene", "copied polyethylene");

            Assert.IsFalse(clone.Aliases.Any());
            Assert.IsFalse(context.MaterialAliases.Any());
        }
        finally
        {
            RestoreFMDBState(state);
        }
    }
}
