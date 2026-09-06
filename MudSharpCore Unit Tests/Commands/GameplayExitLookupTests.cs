using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class GameplayExitLookupTests
{
	[DataTestMethod]
	[DataRow("n", CardinalDirection.North)]
	[DataRow("north", CardinalDirection.North)]
	[DataRow("nw", CardinalDirection.NorthWest)]
	[DataRow("northwest", CardinalDirection.NorthWest)]
	[DataRow("north-west", CardinalDirection.NorthWest)]
	[DataRow("ne", CardinalDirection.NorthEast)]
	[DataRow("northeast", CardinalDirection.NorthEast)]
	[DataRow("NORTH-EAST", CardinalDirection.NorthEast)]
	public void InstallDoor_OverlappingDirections_ValidatesOnlyTheRequestedExit(string keyword,
		CardinalDirection expected)
	{
		var fixture = new Fixture();
		var northwest = fixture.AddExit(CardinalDirection.NorthWest);
		var northeast = fixture.AddExit(CardinalDirection.NorthEast);
		var north = fixture.AddExit(CardinalDirection.North);
		var exits = new[] { northwest, northeast, north };

		typeof(ManipulationModule).GetMethod("InstallDoor", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [fixture.Actor.Object, new StringStack(keyword), Mock.Of<IGameItem>(), Mock.Of<IDoor>()]);

		foreach (var exit in exits)
		{
			exit.VerifyGet(x => x.AcceptsDoor,
				exit.Object.CellExitFor(fixture.Cell.Object).OutboundDirection == expected ? Times.Once() : Times.Never());
		}
	}

	[TestMethod]
	public void GetExitKeyword_MissingNorth_DoesNotSelectNorthwest()
	{
		var fixture = new Fixture();
		fixture.AddExit(CardinalDirection.NorthWest);

		Assert.IsNull(fixture.Resolve("n"));
		Assert.IsNull(fixture.Resolve("north"));
	}

	[TestMethod]
	public void GetExitKeyword_AllCardinalAliases_ResolveTheirExactDirection()
	{
		var fixture = new Fixture();
		foreach (var direction in CardinalDirectionExtensions.CardinalExitStrings.Values.Distinct())
		{
			fixture.AddExit(direction);
		}

		foreach (var (keyword, direction) in CardinalDirectionExtensions.CardinalExitStrings)
		{
			Assert.AreEqual(direction, fixture.Resolve(keyword)?.OutboundDirection, keyword);
		}
	}

	[TestMethod]
	public void GetExitKeyword_HiddenExit_IsNotTargeted()
	{
		var fixture = new Fixture();
		var exit = fixture.AddExit(CardinalDirection.North);
		fixture.Actor.Setup(x => x.CanSee(exit.Object, PerceiveIgnoreFlags.None)).Returns(false);

		Assert.IsNull(fixture.Resolve("n"));
	}

	[TestMethod]
	public void GetExitKeyword_ExitOnAnotherLayer_IsNotTargeted()
	{
		var fixture = new Fixture();
		fixture.AddExit(CardinalDirection.North);
		fixture.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);

		Assert.IsNull(fixture.Resolve("north"));
	}

	[TestMethod]
	public void GetExitKeyword_NamedExit_PreservesKeywordMatching()
	{
		var fixture = new Fixture();
		var exit = fixture.AddExit(CardinalDirection.Unknown, "street");

		Assert.AreSame(exit.Object.CellExitFor(fixture.Cell.Object), fixture.Resolve("street"));
		Assert.AreSame(exit.Object.CellExitFor(fixture.Cell.Object), fixture.Resolve("str"));
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ICell> Cell { get; } = new();
		private readonly Mock<ICellOverlay> _overlay = new();
		private readonly TestExitManager _manager = new();
		private readonly System.Collections.Generic.List<long> _ids = [];

		public Fixture()
		{
			var terrain = new Mock<ITerrain>();
			terrain.SetupGet(x => x.TerrainLayers).Returns([RoomLayer.GroundLevel]);
			Cell.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain.Object);
			Cell.SetupGet(x => x.Location).Returns(Cell.Object);
			Cell.SetupGet(x => x.CurrentOverlay).Returns(_overlay.Object);
			_overlay.SetupGet(x => x.ExitIDs).Returns(_ids);
			Actor.SetupGet(x => x.Location).Returns(Cell.Object);
			Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), PerceiveIgnoreFlags.None)).Returns(true);
			Cell.Setup(x => x.GetExitKeyword(It.IsAny<string>(), Actor.Object))
				.Returns<string, IPerceiver>((keyword, _) => Resolve(keyword)!);
		}

		public ICellExit? Resolve(string keyword) => _manager.GetExitKeyword(Cell.Object, keyword, Actor.Object);

		public Mock<IExit> AddExit(CardinalDirection direction, string? keyword = null)
		{
			var parent = new Mock<IExit>();
			parent.SetupGet(x => x.Id).Returns(_ids.Count + 1);
			parent.SetupGet(x => x.BlockedLayers).Returns([]);
			ICellExit exit = keyword is null
				? new CellExit(parent.Object, Cell.Object, Cell.Object, direction, direction)
				: new NonCardinalCellExit(parent.Object, Cell.Object, Cell.Object, "leave", keyword,
					[keyword], "towards", keyword, "from", keyword);
			parent.Setup(x => x.CellExitFor(Cell.Object)).Returns(exit);
			parent.Setup(x => x.IsExitKeyword(Cell.Object, It.IsAny<string>()))
				.Returns<ICell, string>((_, text) => exit.IsExitKeyword(text));
			_ids.Add(parent.Object.Id);
			_manager.Add(Cell.Object, _overlay.Object, parent.Object);
			return parent;
		}
	}

	private sealed class TestExitManager() : ExitManager(Mock.Of<IFuturemud>())
	{
		public void Add(ICell cell, ICellOverlay overlay, IExit exit)
		{
			CellExitDictionary.Add((cell, overlay), exit);
		}
	}
}
