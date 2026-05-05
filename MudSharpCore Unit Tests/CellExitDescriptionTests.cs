#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CellExitDescriptionTests
{
	[TestMethod]
	public void DescribeFor_DoorCapableCardinalExitWithoutDoor_UsesBoldWhite()
	{
		var origin = CreateGroundCell();
		var destination = CreateGroundCell();
		var parent = CreateExit(acceptsDoor: true, door: null);
		var perceiver = CreatePerceiver(origin.Object);
		var exit = new CellExit(parent.Object, origin.Object, destination.Object, CardinalDirection.North,
			CardinalDirection.South);

		var description = exit.DescribeFor(perceiver.Object, colour: true);

		Assert.IsTrue(description.StartsWith(Telnet.BoldWhite.ToString()),
			"Door-capable exits with no installed door should begin with the door-capable colour.");
		Assert.IsTrue(description.Contains("North"));
		Assert.IsTrue(description.EndsWith(Telnet.RESET + Telnet.Green));
	}

	[TestMethod]
	public void DescribeFor_DoorCapableNonCardinalExitWithoutDoor_UsesBoldWhite()
	{
		var origin = CreateGroundCell();
		var destination = CreateGroundCell();
		var parent = CreateExit(acceptsDoor: true, door: null);
		var perceiver = CreatePerceiver(origin.Object);
		var exit = new NonCardinalCellExit(parent.Object, origin.Object, destination.Object, "enter", "gate",
			new[] { "gate" }, "towards", "the gate", "from", "the gate");

		var description = exit.DescribeFor(perceiver.Object, colour: true);

		Assert.IsTrue(description.StartsWith(Telnet.BoldWhite.ToString()),
			"Door-capable non-cardinal exits with no installed door should begin with the door-capable colour.");
		Assert.IsTrue(description.Contains("'Enter Gate'"));
		Assert.IsTrue(description.EndsWith(Telnet.RESET + Telnet.Green));
	}

	[TestMethod]
	public void DescribeFor_ColourFalse_DoesNotApplyDoorCapableColour()
	{
		var origin = CreateGroundCell();
		var destination = CreateGroundCell();
		var parent = CreateExit(acceptsDoor: true, door: null);
		var perceiver = CreatePerceiver(origin.Object);
		var exit = new CellExit(parent.Object, origin.Object, destination.Object, CardinalDirection.North,
			CardinalDirection.South);

		var description = exit.DescribeFor(perceiver.Object, colour: false);

		Assert.AreEqual("North", description);
		Assert.IsFalse(description.Contains("\x1B["));
	}

	[TestMethod]
	public void DescribeFor_DoorCapableClimbExit_UsesClimbColour()
	{
		var origin = CreateGroundCell();
		var destination = CreateGroundCell();
		var parent = CreateExit(acceptsDoor: true, door: null, isClimbExit: true);
		var perceiver = CreatePerceiver(origin.Object);
		var exit = new CellExit(parent.Object, origin.Object, destination.Object, CardinalDirection.Down,
			CardinalDirection.Up);

		var description = exit.DescribeFor(perceiver.Object, colour: true);

		Assert.IsTrue(description.StartsWith(Telnet.Yellow.ToString()),
			"Movement warning colours should take priority over the door-capable colour.");
		Assert.IsFalse(description.StartsWith(Telnet.BoldWhite.ToString()));
	}

	private static Mock<ITerrain> CreateGroundTerrain()
	{
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.TerrainLayers).Returns(new[] { RoomLayer.GroundLevel });
		return terrain;
	}

	private static Mock<ICell> CreateGroundCell()
	{
		var terrain = CreateGroundTerrain();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Location).Returns(cell.Object);
		cell.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain.Object);
		cell.Setup(x => x.IsSwimmingLayer(It.IsAny<RoomLayer>())).Returns(false);
		cell.Setup(x => x.IsUnderwaterLayer(It.IsAny<RoomLayer>())).Returns(false);
		cell.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), true)).Returns(Enumerable.Empty<ICellExit>());
		return cell;
	}

	private static Mock<IExit> CreateExit(bool acceptsDoor, IDoor? door, bool isClimbExit = false)
	{
		var exit = new Mock<IExit>();
		exit.SetupGet(x => x.AcceptsDoor).Returns(acceptsDoor);
		exit.SetupGet(x => x.Door).Returns(door!);
		exit.SetupGet(x => x.BlockedLayers).Returns(Enumerable.Empty<RoomLayer>());
		exit.SetupGet(x => x.IsClimbExit).Returns(isClimbExit);
		exit.SetupGet(x => x.ClimbDifficulty).Returns(Difficulty.Normal);
		return exit;
	}

	private static Mock<IPerceiver> CreatePerceiver(ICell location)
	{
		var perceiver = new Mock<IPerceiver>();
		perceiver.SetupGet(x => x.Location).Returns(location);
		perceiver.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		return perceiver;
	}
}
