using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ExpressionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.ScatterStrategies;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Form.Shape;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ScatterStrategyUtilitiesTests
{
	[TestMethod]
	public void GetCellInfos_ReturnsDistancesAndDirections()
	{
		var builder = new ScatterTestHelpers.CellNetworkBuilder();
		var origin = builder.AddCell("origin", (0, 0, 0));
		var north = builder.AddCell("north", (0, 1, 0));
		var east = builder.AddCell("east", (1, 0, 0));
		builder.ConnectTwoWay("origin", "north", CardinalDirection.North);
		builder.ConnectTwoWay("origin", "east", CardinalDirection.East);
		builder.Finalise();

		var target = ScatterTestHelpers.CreatePerceiver(origin.Cell.Object);
		var infos = ScatterStrategyUtilities.GetCellInfos(target.Object, 2, true);

		Assert.AreEqual(3, infos.Count);
		var originInfo = infos.Single(x => x.Cell == origin.Cell.Object);
		Assert.AreEqual(0, originInfo.Distance);
		Assert.AreEqual(CardinalDirection.Unknown, originInfo.DirectionFromOrigin);

		var northInfo = infos.Single(x => x.Cell == north.Cell.Object);
		Assert.AreEqual(1, northInfo.Distance);
		Assert.AreEqual(CardinalDirection.North, northInfo.DirectionFromOrigin);

		var eastInfo = infos.Single(x => x.Cell == east.Cell.Object);
		Assert.AreEqual(1, eastInfo.Distance);
		Assert.AreEqual(CardinalDirection.East, eastInfo.DirectionFromOrigin);
	}

	[TestMethod]
	public void GetCellInfos_RespectsClosedDoors()
	{
		var builder = new ScatterTestHelpers.CellNetworkBuilder();
		var origin = builder.AddCell("origin", (0, 0, 0));
		var north = builder.AddCell("north", (0, 1, 0));
		builder.ConnectTwoWay("origin", "north", CardinalDirection.North, true, false);
		builder.Finalise();

		var target = ScatterTestHelpers.CreatePerceiver(origin.Cell.Object);
		var blocked = ScatterStrategyUtilities.GetCellInfos(target.Object, 1, true);
		Assert.IsFalse(blocked.Any(x => x.Cell == north.Cell.Object));

		var unblocked = ScatterStrategyUtilities.GetCellInfos(target.Object, 1, false);
		Assert.IsTrue(unblocked.Any(x => x.Cell == north.Cell.Object));
	}

	[TestMethod]
	public void DescribeFromDirection_ReturnsCorrectSuffixes()
	{
		Assert.AreEqual(string.Empty, ScatterStrategyUtilities.DescribeFromDirection(CardinalDirection.Unknown));
		Assert.AreEqual(" from above", ScatterStrategyUtilities.DescribeFromDirection(CardinalDirection.Up));
		Assert.AreEqual(" from the South", ScatterStrategyUtilities.DescribeFromDirection(CardinalDirection.South));
	}
}

[TestClass]
public class BallisticScatterStrategyTests
{
	[TestMethod]
	public void GetScatterTarget_PrefersForwardDirectionAndReturnsCellWhenNoTarget()
	{
		ScatterTestHelpers.SetWeightExpression(typeof(BallisticScatterStrategy), "_weightExpression", "size + proximity + 1");
		var builder = new ScatterTestHelpers.CellNetworkBuilder();
		var origin = builder.AddCell("origin", (0, 0, 0));
		var north = builder.AddCell("north", (0, 1, 0));
		var south = builder.AddCell("south", (0, -1, 0));
		builder.ConnectTwoWay("origin", "north", CardinalDirection.North);
		builder.ConnectTwoWay("origin", "south", CardinalDirection.South);
		builder.Finalise();

		var shooter = ScatterTestHelpers.CreateCharacter(origin.Cell.Object);
		var target = ScatterTestHelpers.CreatePerceiver(origin.Cell.Object);
		var path = ScatterTestHelpers.CreatePath(CardinalDirection.North);
		var directionSet = ScatterTestHelpers.GetDirectionSet(path);
		var cellInfos = ScatterStrategyUtilities.GetCellInfos(target.Object, 3, true);
		var weights = ScatterTestHelpers.ComputeDirectionalWeights(typeof(BallisticScatterStrategy), cellInfos, directionSet,
		path.Last().OutboundDirection);
		var expectedIndex = ScatterTestHelpers.GetExpectedIndex(weights.Select(x => x.Weight).ToList(), 5);
		ScatterTestHelpers.SeedRandom(5);

		var result = BallisticScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, path);

		Assert.IsNotNull(result);
		Assert.AreEqual(weights[expectedIndex].Info.Cell, result.Cell);
		Assert.AreEqual(weights[expectedIndex].Info.Distance, result.DistanceFromTarget);
		Assert.AreEqual(weights[expectedIndex].Info.DirectionFromOrigin, result.DirectionFromTarget);
		Assert.IsNull(result.Target);
		Assert.AreEqual(target.Object.RoomLayer, result.RoomLayer);
	}

	[TestMethod]
	public void GetScatterTarget_ReturnsNullWhenTargetLocationMissing()
	{
		var shooter = new Mock<ICharacter>();
		var target = new Mock<IPerceiver>();
		target.SetupGet(x => x.Location).Returns((ICell)null);
		Assert.IsNull(BallisticScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, null));
	}
}

[TestClass]
public class ArcingScatterStrategyTests
{
	[TestMethod]
	public void GetScatterTarget_PrefersCloserCellsWhenNoTarget()
	{
		ScatterTestHelpers.SetWeightExpression(typeof(ArcingScatterStrategy), "_weightExpression", "size + proximity + 1");
		var builder = new ScatterTestHelpers.CellNetworkBuilder();
		var origin = builder.AddCell("origin", (0, 0, 0));
		var north = builder.AddCell("north", (0, 1, 0));
		builder.ConnectTwoWay("origin", "north", CardinalDirection.North);
		builder.Finalise();

		var shooter = ScatterTestHelpers.CreateCharacter(origin.Cell.Object);
		var target = ScatterTestHelpers.CreatePerceiver(origin.Cell.Object);
		var cellInfos = ScatterStrategyUtilities.GetCellInfos(target.Object, 1, true);
		var weights = ScatterTestHelpers.ComputeArcingWeights(cellInfos);
		var expectedIndex = ScatterTestHelpers.GetExpectedIndex(weights.Select(x => x.Weight).ToList(), 11);
		ScatterTestHelpers.SeedRandom(11);

		var result = ArcingScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, Array.Empty<ICellExit>());

		Assert.IsNotNull(result);
		Assert.AreEqual(weights[expectedIndex].Info.Cell, result.Cell);
		Assert.AreEqual(weights[expectedIndex].Info.Distance, result.DistanceFromTarget);
		Assert.AreEqual(weights[expectedIndex].Info.DirectionFromOrigin, result.DirectionFromTarget);
		Assert.IsNull(result.Target);
	}

	[TestMethod]
	public void Weight_IncreasesForHigherLayerTargets()
	{
		ScatterTestHelpers.SetWeightExpression(typeof(ArcingScatterStrategy), "_weightExpression", "size + proximity + 1");
		var target = ScatterTestHelpers.CreatePerceiver(null);
		target.Object.RoomLayer = RoomLayer.GroundLevel;
		var lower = ScatterTestHelpers.CreatePerceiver(null);
		lower.Object.RoomLayer = RoomLayer.GroundLevel;
		lower.SetupGet(x => x.Size).Returns(SizeCategory.Small);
		lower.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Distant);
		var higher = ScatterTestHelpers.CreatePerceiver(null);
		higher.Object.RoomLayer = RoomLayer.InTrees;
		higher.SetupGet(x => x.Size).Returns(SizeCategory.Small);
		higher.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Distant);

		var weightMethod = typeof(ArcingScatterStrategy).GetMethod("Weight", BindingFlags.Static | BindingFlags.NonPublic);
		var lowerWeight = (double)weightMethod!.Invoke(null, new object[] { lower.Object, target.Object });
		var higherWeight = (double)weightMethod.Invoke(null, new object[] { higher.Object, target.Object });
		Assert.IsTrue(higherWeight > lowerWeight);
	}

	[TestMethod]
	public void GetScatterTarget_ReturnsNullWhenTargetLocationMissing()
	{
		var shooter = new Mock<ICharacter>();
		var target = new Mock<IPerceiver>();
		target.SetupGet(x => x.Location).Returns((ICell)null);
		Assert.IsNull(ArcingScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, null));
	}
}

[TestClass]
public class LightScatterStrategyTests
{
	[TestMethod]
	public void GetScatterTarget_FavorsContinuationAndReturnsCell()
	{
		ScatterTestHelpers.SetWeightExpression(typeof(LightScatterStrategy), "_weightExpression", "size + proximity + 1");
		var builder = new ScatterTestHelpers.CellNetworkBuilder();
		var origin = builder.AddCell("origin", (0, 0, 0));
		var north = builder.AddCell("north", (0, 1, 0));
		var east = builder.AddCell("east", (1, 0, 0));
		builder.ConnectTwoWay("origin", "north", CardinalDirection.North);
		builder.ConnectTwoWay("origin", "east", CardinalDirection.East);
		builder.Finalise();

		var shooter = ScatterTestHelpers.CreateCharacter(origin.Cell.Object);
		var target = ScatterTestHelpers.CreatePerceiver(origin.Cell.Object);
		var path = ScatterTestHelpers.CreatePath(CardinalDirection.North);
		var directionSet = ScatterTestHelpers.GetDirectionSet(path);
		var cellInfos = ScatterStrategyUtilities.GetCellInfos(target.Object, 5, true);
		var weights = ScatterTestHelpers.ComputeDirectionalWeights(typeof(LightScatterStrategy), cellInfos, directionSet,
		path.Last().OutboundDirection);
		var expectedIndex = ScatterTestHelpers.GetExpectedIndex(weights.Select(x => x.Weight).ToList(), 17);
		ScatterTestHelpers.SeedRandom(17);

		var result = LightScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, path);

		Assert.IsNotNull(result);
		Assert.AreEqual(weights[expectedIndex].Info.Cell, result.Cell);
		Assert.AreEqual(weights[expectedIndex].Info.Distance, result.DistanceFromTarget);
		Assert.AreEqual(weights[expectedIndex].Info.DirectionFromOrigin, result.DirectionFromTarget);
		Assert.IsNull(result.Target);
		Assert.AreEqual(target.Object.RoomLayer, result.RoomLayer);
	}

	[TestMethod]
	public void GetScatterTarget_ReturnsNullWhenTargetLocationMissing()
	{
		var shooter = new Mock<ICharacter>();
		var target = new Mock<IPerceiver>();
		target.SetupGet(x => x.Location).Returns((ICell)null);
		Assert.IsNull(LightScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, null));
	}
}

[TestClass]
public class SpreadScatterStrategyTests
{
	[TestMethod]
	public void GetScatterTarget_DropsIntoOriginWhenNoTargets()
	{
		ScatterTestHelpers.SetWeightExpression(typeof(SpreadScatterStrategy), "_weightExpression", "size + proximity + 1");
		var builder = new ScatterTestHelpers.CellNetworkBuilder();
		var origin = builder.AddCell("origin", (0, 0, 0));
		builder.Finalise();

		var shooter = ScatterTestHelpers.CreateCharacter(origin.Cell.Object);
		var target = ScatterTestHelpers.CreatePerceiver(origin.Cell.Object);
		ScatterTestHelpers.SeedRandom(23);

		var result = SpreadScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, Array.Empty<ICellExit>());

		Assert.IsNotNull(result);
		Assert.AreEqual(origin.Cell.Object, result.Cell);
		Assert.AreEqual(0, result.DistanceFromTarget);
		Assert.AreEqual(CardinalDirection.Unknown, result.DirectionFromTarget);
		Assert.IsNull(result.Target);
	}

	[TestMethod]
	public void GetScatterTarget_StrikesAvailableTarget()
	{
		ScatterTestHelpers.SetWeightExpression(typeof(SpreadScatterStrategy), "_weightExpression", "size + proximity + 1");
		var builder = new ScatterTestHelpers.CellNetworkBuilder();
		var origin = builder.AddCell("origin", (0, 0, 0));
		builder.Finalise();

		var shooter = ScatterTestHelpers.CreateCharacter(origin.Cell.Object);
		var target = ScatterTestHelpers.CreatePerceiver(origin.Cell.Object);
		var victim = ScatterTestHelpers.CreateCharacter(origin.Cell.Object);
		victim.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Immediate);
		origin.Characters.Add(victim.Object);
		ScatterTestHelpers.SeedRandom(29);

		var result = SpreadScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, Array.Empty<ICellExit>());

		Assert.IsNotNull(result);
		Assert.AreEqual(origin.Cell.Object, result.Cell);
		Assert.AreEqual(victim.Object, result.Target);
		Assert.AreEqual(target.Object.RoomLayer, result.RoomLayer);
	}

	[TestMethod]
	public void GetScatterTarget_ReturnsNullWhenTargetLocationMissing()
	{
		var shooter = new Mock<ICharacter>();
		var target = new Mock<IPerceiver>();
		target.SetupGet(x => x.Location).Returns((ICell)null);
		Assert.IsNull(SpreadScatterStrategy.Instance.GetScatterTarget(shooter.Object, target.Object, null));
	}
}

internal static class ScatterTestHelpers
{
	internal sealed class CellNetworkBuilder
	{
		internal sealed class CellData
		{
			internal CellData(Mock<ICell> cell, List<ICharacter> characters, List<IGameItem> gameItems)
			{
				Cell = cell;
				Characters = characters;
				GameItems = gameItems;
			}

			internal Mock<ICell> Cell { get; }
			internal List<ICharacter> Characters { get; }
			internal List<IGameItem> GameItems { get; }
		}

		private readonly Dictionary<string, CellData> _cells = new();
		private readonly Dictionary<Mock<ICell>, List<ICellExit>> _exits = new();

		internal CellData AddCell(string name, (int X, int Y, int Z) coordinates)
		{
			var room = new Mock<IRoom>();
			room.SetupGet(x => x.X).Returns(coordinates.X);
			room.SetupGet(x => x.Y).Returns(coordinates.Y);
			room.SetupGet(x => x.Z).Returns(coordinates.Z);

			var cell = new Mock<ICell>();
			cell.SetupGet(x => x.Name).Returns(name);
			cell.SetupGet(x => x.Id).Returns(0L);
			cell.SetupGet(x => x.Room).Returns(room.Object);
			cell.SetupGet(x => x.EventHandlers).Returns(Array.Empty<IHandleEvents>());
			cell.SetupGet(x => x.Cells).Returns(Array.Empty<ICell>());

			var characters = new List<ICharacter>();
			cell.SetupGet(x => x.Characters).Returns(characters);
			cell.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns((RoomLayer _) => characters);

			var items = new List<IGameItem>();
			cell.SetupGet(x => x.GameItems).Returns(items);
			cell.Setup(x => x.LayerGameItems(It.IsAny<RoomLayer>())).Returns((RoomLayer _) => items);

			var data = new CellData(cell, characters, items);
			_cells[name] = data;
			_exits[cell] = new List<ICellExit>();
			return data;
		}

		internal void ConnectTwoWay(string originName, string destinationName, CardinalDirection direction, bool createClosedDoor = false,
		bool canFireThrough = true)
		{
			var origin = _cells[originName];
			var destination = _cells[destinationName];
			var exit = new Mock<IExit>();
			exit.SetupGet(x => x.Cells).Returns(new[] { origin.Cell.Object, destination.Cell.Object });
			exit.SetupProperty(x => x.Door);

			if (createClosedDoor)
			{
				var door = new Mock<IDoor>();
				door.SetupGet(x => x.IsOpen).Returns(false);
				door.SetupGet(x => x.CanFireThrough).Returns(canFireThrough);
				door.SetupGet(x => x.CanPlayersSmash).Returns(false);
				door.SetupGet(x => x.Locks).Returns(Array.Empty<ILock>());
				exit.Object.Door = door.Object;
			}

			var forward = CreateExit(origin.Cell.Object, destination.Cell.Object, direction, exit.Object);
			var reverse = CreateExit(destination.Cell.Object, origin.Cell.Object, direction.Opposite(), exit.Object);
			forward.SetupGet(x => x.Opposite).Returns(reverse.Object);
			reverse.SetupGet(x => x.Opposite).Returns(forward.Object);

			_exits[origin.Cell].Add(forward.Object);
			_exits[destination.Cell].Add(reverse.Object);
		}

		private static Mock<ICellExit> CreateExit(ICell origin, ICell destination, CardinalDirection direction, IExit sharedExit)
		{
			var exit = new Mock<ICellExit>();
			exit.SetupGet(x => x.Origin).Returns(origin);
			exit.SetupGet(x => x.Destination).Returns(destination);
			exit.SetupGet(x => x.OutboundDirection).Returns(direction);
			exit.SetupGet(x => x.InboundDirection).Returns(direction.Opposite());
			exit.SetupGet(x => x.Exit).Returns(sharedExit);
			return exit;
		}

		internal void Finalise()
		{
			foreach (var (cell, exits) in _exits)
			{
				cell.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>()))
				.Returns((IPerceiver _, bool __) => exits.ToList());
			}
		}

		internal CellData this[string name] => _cells[name];
	}

	internal static void SeedRandom(int seed)
	{
		var newRandom = new Random(seed);
		var fields = typeof(Random).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (var field in fields)
		{
			field.SetValue(Constants.Random, field.GetValue(newRandom));
		}
	}

	internal static void SetWeightExpression(Type strategyType, string fieldName, string expressionText)
	{
		var field = strategyType.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
		field!.SetValue(null, new Expression(expressionText));
	}

	internal static Mock<ICharacter> CreateCharacter(ICell location)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Location).Returns(location);
		character.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		character.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
		character.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Distant);
		character.Setup(x => x.Equals(It.IsAny<object>())).Returns<object>(o => ReferenceEquals(o, character.Object));
		return character;
	}

	internal static Mock<IPerceiver> CreatePerceiver(ICell location)
	{
		var perceiver = new Mock<IPerceiver>();
		perceiver.SetupGet(x => x.Location).Returns(location);
		perceiver.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		perceiver.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
		perceiver.Setup(x => x.GetProximity(It.IsAny<IPerceivable>())).Returns(Proximity.Distant);
		perceiver.Setup(x => x.Equals(It.IsAny<object>())).Returns<object>(o => ReferenceEquals(o, perceiver.Object));
		return perceiver;
	}

	internal static List<ICellExit> CreatePath(params CardinalDirection[] directions)
	{
		var path = new List<ICellExit>();
		foreach (var direction in directions)
		{
			var exit = new Mock<ICellExit>();
			exit.SetupGet(x => x.OutboundDirection).Returns(direction);
			path.Add(exit.Object);
		}
		return path;
	}

	internal static HashSet<CardinalDirection> GetDirectionSet(IReadOnlyList<ICellExit> path)
	{
		var counts = path.CountDirections();
		return new HashSet<CardinalDirection>((counts.Northness, counts.Southness, counts.Westness, counts.Eastness,
		counts.Upness, counts.Downness).ContainedDirections().Where(x => x != CardinalDirection.Unknown));
	}

	internal static List<(CellScatterInfo Info, double Weight)> ComputeDirectionalWeights(Type strategyType,
	IReadOnlyList<CellScatterInfo> infos, HashSet<CardinalDirection> preferredDirections, CardinalDirection lastDirection)
	{
		var method = strategyType.GetMethod("CellWeight", BindingFlags.Static | BindingFlags.NonPublic);
		var results = new List<(CellScatterInfo, double)>();
		foreach (var info in infos)
		{
			var weight = (double)method!.Invoke(null, new object[] { info, preferredDirections, lastDirection });
			if (weight > 0)
			{
				results.Add((info, weight));
			}
		}
		return results;
	}

	internal static List<(CellScatterInfo Info, double Weight)> ComputeArcingWeights(IReadOnlyList<CellScatterInfo> infos)
	{
		var method = typeof(ArcingScatterStrategy).GetMethod("CellWeight", BindingFlags.Static | BindingFlags.NonPublic);
		var results = new List<(CellScatterInfo, double)>();
		foreach (var info in infos)
		{
			var weight = (double)method!.Invoke(null, new object[] { info });
			if (weight > 0)
			{
				results.Add((info, weight));
			}
		}
		return results;
	}

	internal static int GetExpectedIndex(IReadOnlyList<double> weights, int seed)
	{
		var random = new Random(seed);
		var sum = weights.Sum();
		var roll = random.NextDouble() * sum;
		for (var i = 0; i < weights.Count; i++)
		{
			var weight = weights[i];
			if (weight <= 0)
			{
				continue;
			}

			if ((roll -= weight) <= 0.0)
			{
				return i;
			}
		}

		return weights.Count - 1;
	}
}
