#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System.Collections;
using System.Linq;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TerrainLayerModelTests
{
	[DataTestMethod]
	[DataRow("rooftopsonly")]
	[DataRow("rooftoponly")]
	[DataRow("rooftops-only")]
	[DataRow("rooftop-only")]
	public void RooftopsOnlyAliases_LoadWithRooftopAsLowestLayerAndBothAirLayers(string behaviour)
	{
		var terrain = CreateTerrain(behaviour);

		CollectionAssert.AreEquivalent(
			new[] { RoomLayer.OnRooftops, RoomLayer.InAir, RoomLayer.HighInAir },
			terrain.TerrainLayers.ToArray());
		Assert.AreEqual(RoomLayer.OnRooftops, terrain.TerrainLayers.LowestLayer());
		Assert.AreEqual("rooftopsonly", terrain.TerrainBehaviourString);
		Assert.IsFalse(terrain.TerrainLayers.Contains(RoomLayer.GroundLevel));
	}

	[DataTestMethod]
	[DataRow("rooftops")]
	[DataRow("rooftop")]
	public void RooftopsModelAliases_RetainGroundLevelForExistingTerrain(string behaviour)
	{
		var terrain = CreateTerrain(behaviour);

		CollectionAssert.Contains(terrain.TerrainLayers.ToArray(), RoomLayer.GroundLevel);
		CollectionAssert.Contains(terrain.TerrainLayers.ToArray(), RoomLayer.OnRooftops);
		Assert.AreEqual("rooftops", terrain.TerrainBehaviourString);
	}

	[TestMethod]
	public void RooftopsOnlyToRooftopsExit_AppearsOnRooftopAndAirLayersButNotGround()
	{
		var origin = CreateCell(CreateTerrain("rooftopsonly"));
		var destination = CreateCell(CreateTerrain("rooftops"));
		var parent = new Mock<IExit>();
		parent.SetupGet(x => x.BlockedLayers).Returns([]);
		parent.SetupGet(x => x.ClimbDifficulty).Returns(Difficulty.Normal);
		var exit = new CellExit(
			parent.Object,
			origin.Object,
			destination.Object,
			CardinalDirection.North,
			CardinalDirection.South);

		var layers = exit.WhichLayersExitAppears().ToArray();

		CollectionAssert.AreEquivalent(
			new[] { RoomLayer.OnRooftops, RoomLayer.InAir, RoomLayer.HighInAir },
			layers);
		CollectionAssert.DoesNotContain(layers, RoomLayer.GroundLevel);
	}

	[TestMethod]
	public void LayerExits_CharacterOverload_UsesCharactersCurrentLayer()
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.GetObject).Returns(character.Object);
		AssertLayerExitsPerceiverOverload(
			ProgVariableTypes.Character,
			character.Object);
	}

	[TestMethod]
	public void LayerExits_ItemOverload_UsesItemsCurrentLayer()
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.GetObject).Returns(item.Object);
		AssertLayerExitsPerceiverOverload(
			ProgVariableTypes.Item,
			item.Object);
	}

	private static void AssertLayerExitsPerceiverOverload(
		ProgVariableTypes perceiverType,
		IPerceiver perceiver)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var expectedExitMock = new Mock<ICellExit>();
		expectedExitMock.SetupGet(x => x.GetObject).Returns(expectedExitMock.Object);
		var expectedExit = expectedExitMock.Object;
		var location = new Mock<ICell>();
		location
			.Setup(x => x.ExitsFor(perceiver, false))
			.Returns([expectedExit]);
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x =>
				x.FunctionName.EqualTo("layerexits") &&
				x.Parameters.SequenceEqual(new[] { ProgVariableTypes.Location, perceiverType }));
		var function = compiler.CompilerFunction(
			[
				new ConstantFunction(location.Object, ProgVariableTypes.Location),
				new ConstantFunction((IProgVariable)perceiver, perceiverType)
			],
			FutureProgTestBootstrap.Gameworld);

		var result = function.Execute(new Mock<IVariableSpace>().Object);
		var exits = ((IEnumerable)function.Result)
			.Cast<IProgVariable>()
			.Select(x => x.GetObject)
			.ToList();

		Assert.AreEqual(StatementResult.Normal, result);
		CollectionAssert.AreEqual(new object[] { expectedExit }, exits);
		location.Verify(x => x.ExitsFor(perceiver, false), Times.Once);
	}

	private static Terrain CreateTerrain(string behaviour)
	{
		var model = new DB.Terrain
		{
			Id = 1,
			Name = "Test Terrain",
			TerrainBehaviourMode = behaviour,
			TagInformation = string.Empty
		};

		return new Terrain(model, new Mock<IFuturemud>().Object);
	}

	private static Mock<ICell> CreateCell(ITerrain terrain)
	{
		var cell = new Mock<ICell>();
		cell.Setup(x => x.Terrain(It.IsAny<IPerceiver?>())).Returns(terrain);
		return cell;
	}

	private sealed class ConstantFunction(
		IProgVariable result,
		ProgVariableTypes returnType) : IFunction
	{
		public IProgVariable Result { get; } = result;
		public ProgVariableTypes ReturnType { get; } = returnType;
		public string ErrorMessage => string.Empty;
		public StatementResult ExpectedResult => StatementResult.Normal;

		public StatementResult Execute(IVariableSpace variables)
		{
			return StatementResult.Normal;
		}

		public bool IsReturnOrContainsReturnOnAllBranches()
		{
			return false;
		}
	}
}
