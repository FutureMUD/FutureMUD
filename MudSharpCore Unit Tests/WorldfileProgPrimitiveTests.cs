#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Characters;
using MudSharp.FutureProg.Functions.GameItem;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WorldfileProgPrimitiveTests
{
	[TestMethod]
	[DataRow(-1.0)]
	[DataRow(1.5)]
	[DataRow(2147483648.0)]
	public void SetLockPattern_InvalidPatternDoesNotMutate(double pattern)
	{
		var key = new Mock<IKey>();
		var item = new Mock<IGameItem>();
		item.Setup(x => x.GetItemType<IKey>()).Returns(key.Object);
		var function = new SetLockPatternFunction([Constant(item.Object, ProgVariableTypes.Item),
			Constant((decimal)pattern, ProgVariableTypes.Number)]);
		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		Assert.AreEqual(false, function.Result.GetObject);
		key.VerifySet(x => x.Pattern = It.IsAny<int>(), Times.Never);
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(13353)]
	public void SetLockPattern_ConfiguresNativeKeyAndLock(int pattern)
	{
		var key = new Mock<IKey>();
		var itemLock = new Mock<ILock>();
		var item = new Mock<IGameItem>();
		item.Setup(x => x.GetItemType<IKey>()).Returns(key.Object);
		item.Setup(x => x.GetItemType<ILock>()).Returns(itemLock.Object);
		var function = new SetLockPatternFunction([Constant(item.Object, ProgVariableTypes.Item),
			Constant((decimal)pattern, ProgVariableTypes.Number)]);
		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		Assert.AreEqual(true, function.Result.GetObject);
		key.VerifySet(x => x.Pattern = pattern, Times.Once);
		itemLock.VerifySet(x => x.Pattern = pattern, Times.Once);
	}

	[TestMethod]
	public void SetLit_UsesNativeLightAndRejectsUnlightableItems()
	{
		var item = new Mock<IGameItem>();
		var light = new Mock<ILightable>();
		FutureProgTestBootstrap.EnsureInitialised();
		var compiler = FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName == "setlit");
		var function = compiler.CompilerFunction([Constant(item.Object, ProgVariableTypes.Item), Constant(true, ProgVariableTypes.Boolean)], FutureProgTestBootstrap.Gameworld);
		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		Assert.AreEqual(false, function.Result.GetObject);
		item.Setup(x => x.GetItemType<ILightable>()).Returns(light.Object);
		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		Assert.AreEqual(true, function.Result.GetObject);
		light.VerifySet(x => x.Lit = true, Times.Once);
	}

	[TestMethod]
	public void LoadedQueries_ReturnSnapshotsWithoutLoadingOfflineEntities()
	{
		var items = new List<IGameItem> { Mock.Of<IGameItem>() };
		var player = Mock.Of<ICharacter>();
		var npc = Mock.Of<MudSharp.NPC.INPC>();
		var actors = new List<ICharacter> { player, npc };
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Items).Returns(Collection(items));
		world.SetupGet(x => x.Characters).Returns(Collection(new List<ICharacter> { player }));
		world.SetupGet(x => x.NPCs).Returns(Collection(new List<ICharacter> { npc }));
		world.SetupGet(x => x.Actors).Returns(Collection(actors));
		var queryItems = new LoadedItemsFunction([], world.Object);
		var queryActors = new LoadedCharactersFunction([], world.Object);
		Assert.AreEqual(StatementResult.Normal, queryItems.Execute(new VariableSpace()));
		Assert.AreEqual(StatementResult.Normal, queryActors.Execute(new VariableSpace()));
		items.Add(Mock.Of<IGameItem>());
		actors.Add(Mock.Of<ICharacter>());
		Assert.AreEqual(1, ((IEnumerable)queryItems.Result.GetObject).Cast<object>().Count());
		Assert.AreEqual(2, ((IEnumerable)queryActors.Result.GetObject).Cast<object>().Count());
	}

	[TestMethod]
	public void PrimitiveContracts_CompileThroughNativeCompiler()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var cases = new[]
		{
			(ProgVariableTypes.Number, "return loadeditems().count(x, @x.proto == 1)"),
			(ProgVariableTypes.Number, "return loadedcharacters().count(x, @x.npctemplateid == 1)"),
			(ProgVariableTypes.Boolean, "return setlockpattern(@item, 13353)"),
			(ProgVariableTypes.Boolean, "return setlit(@item, true)")
		};
		foreach (var (returns, text) in cases)
		{
			var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "worldfile_contract", returns,
				[Tuple.Create(ProgVariableTypes.Item, "item")], text);
			Assert.IsTrue(prog.Compile(), prog.CompileError);
		}
		var pruning = new FutureProg(FutureProgTestBootstrap.Gameworld, "worldfile_pruning", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "npc")],
			"var pending as item collection\npending = collectionconcat(@npc.inventory, @npc.inventory.SelectMany(i, @i.contents))\nvar candidate as item\ncandidate = @pending.First(i, @i.proto == 1)\nif (not(isnull(@candidate)))\npending = @pending.Where(i, @i.id != @candidate.id)\nend if\nreturn true");
		Assert.IsTrue(pruning.Compile(), pruning.CompileError);
		var cash = new FutureProg(FutureProgTestBootstrap.Gameworld, "worldfile_starting_cash", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "npc")],
			"givecurrency(tocurrency(1), random(1, 10), @npc, false)\nreturn true");
		Assert.IsTrue(cash.Compile(), cash.CompileError);
	}

	[TestMethod]
	public void AssignedNullItemCanBeReassignedAcrossNestedScopes()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		item.SetupGet(x => x.GetObject).Returns(item.Object);
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "worldfile_null_reassignment", ProgVariableTypes.Item,
			[Tuple.Create(ProgVariableTypes.Item, "item")],
			"var previous as item\nprevious = null(\"item\")\nif (isnull(@previous))\nif (true)\nprevious = @item\nend if\nend if\nreturn @previous");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.AreSame(item.Object, prog.Execute(item.Object));
	}

	[TestMethod]
	public void AssignedNullItemCanRemainNullAndStillRetainsCompileTimeTypeChecking()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "worldfile_repeat_null", ProgVariableTypes.Boolean, [],
			"var previous as item\nprevious = null(\"item\")\nprevious = null(\"item\")\nreturn isnull(@previous)");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.AreEqual(true, prog.Execute());
		var invalid = new FutureProg(FutureProgTestBootstrap.Gameworld, "worldfile_invalid_type", ProgVariableTypes.Boolean, [],
			"var previous as item\nprevious = null(\"item\")\nprevious = 42\nreturn true");
		Assert.IsFalse(invalid.Compile());
	}

	private static IUneditableAll<T> Collection<T>(List<T> values) where T : class, IFrameworkItem
	{
		var collection = new Mock<IUneditableAll<T>>();
		collection.Setup(x => x.GetEnumerator()).Returns(() => values.GetEnumerator());
		return collection.Object;
	}

	private static IFunction Constant(object value, ProgVariableTypes type)
	{
		if (value is IGameItem item) Mock.Get(item).SetupGet(x => x.GetObject).Returns(item);
		var variable = new Mock<IProgVariable>();
		variable.SetupGet(x => x.GetObject).Returns(value);
		variable.SetupGet(x => x.Type).Returns(type);
		var function = new Mock<IFunction>();
		function.SetupGet(x => x.ReturnType).Returns(type);
		function.SetupGet(x => x.Result).Returns(value as IProgVariable ?? variable.Object);
		function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		return function.Object;
	}
}
