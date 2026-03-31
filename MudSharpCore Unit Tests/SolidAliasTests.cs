#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MaterialModel = MudSharp.Models.Material;
using TagModel = MudSharp.Models.Tag;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SolidAliasTests
{
	private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static Solid CreateSolid(long id, string name, IFuturemud gameworld, params string[] aliases)
	{
		var material = new MaterialModel
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
		var tagCollection = new All<ITag>();
		foreach (var tag in tags ?? Enumerable.Empty<TagModel>())
		{
			var mockTag = new Mock<ITag>();
			mockTag.SetupGet(x => x.Id).Returns(tag.Id);
			mockTag.SetupGet(x => x.Name).Returns(tag.Name);
			mockTag.SetupGet(x => x.FrameworkItemType).Returns("Tag");
			mockTag.SetupGet(x => x.FullName).Returns(tag.Name);
			tagCollection.Add(mockTag.Object);
		}

		var gameworld = new Mock<IFuturemud>();
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
		var materials = new All<ISolid>();
		var gameworld = CreateGameworld(materials);
		var solid = CreateSolid(1, "mild steel", gameworld.Object, "steel", "ms");

		CollectionAssert.AreEqual(new[] { "mild steel", "steel", "ms" }, solid.Names.ToList());
	}

	[TestMethod]
	public void TryAddAlias_CollidingNameOrAlias_IsRejected()
	{
		var materials = new All<ISolid>();
		var gameworld = CreateGameworld(materials);
		var mildSteel = CreateSolid(1, "mild steel", gameworld.Object);
		var hdpe = CreateSolid(2, "high-density polyethylene", gameworld.Object, "plastic");
		materials.Add(mildSteel);
		materials.Add(hdpe);

		Assert.IsFalse(mildSteel.TryAddAlias("high-density polyethylene", out _));
		Assert.IsFalse(mildSteel.TryAddAlias("plastic", out _));
		Assert.IsFalse(mildSteel.Aliases.Any());
	}

	[TestMethod]
	public void TryRename_DoesNotPreserveOldNameAsAlias()
	{
		var materials = new All<ISolid>();
		var gameworld = CreateGameworld(materials);
		var solid = CreateSolid(1, "mild steel", gameworld.Object);
		materials.Add(solid);

		var renamed = solid.TryRename("construction steel", out var errorMessage);

		Assert.IsTrue(renamed, errorMessage);
		Assert.AreEqual("construction steel", solid.Name);
		Assert.IsFalse(solid.Names.Contains("mild steel"));
	}

	[TestMethod]
	public void Clone_StartsWithoutCopiedAliases()
	{
		var state = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var materials = new All<ISolid>();
			var gameworld = CreateGameworld(materials);
			var solid = CreateSolid(1, "high-density polyethylene", gameworld.Object, "hdpe");
			materials.Add(solid);

			var clone = solid.Clone("copied polyethylene", "copied polyethylene");

			Assert.IsFalse(clone.Aliases.Any());
			Assert.IsFalse(context.MaterialAliases.Any());
		}
		finally
		{
			RestoreFMDBState(state);
		}
	}
}
