#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LinearRouteMovementTests
{
	[TestMethod]
	public void Segment_FakeElapsedTime_InterpolatesAndClampsExactly()
	{
		var (cell, _) = CreateRouteCell(1L, 10_000.0, 100.0);
		var segment = new LinearRouteMovementSegment(
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 1_000.0),
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 1_100.0),
			10.0);

		Assert.AreEqual(RouteCellDirection.Positive, segment.Direction);
		Assert.AreEqual(100.0, segment.DistanceMetres);
		Assert.AreEqual(TimeSpan.FromSeconds(10.0), segment.Duration);
		Assert.AreEqual(1_000.0, segment.PositionAt(TimeSpan.FromSeconds(-1.0)).RoutePositionMetres);
		Assert.AreEqual(1_025.0, segment.PositionAt(TimeSpan.FromSeconds(2.5)).RoutePositionMetres);
		Assert.AreEqual(1_100.0, segment.PositionAt(TimeSpan.FromSeconds(20.0)).RoutePositionMetres);
	}

	[TestMethod]
	public void Segment_BackwardAndInvalidCoordinates_UsesDirectionAndRejectsBounds()
	{
		var (cell, _) = CreateRouteCell(2L, 100.0, 100.0);
		var segment = new LinearRouteMovementSegment(
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 80.0),
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 20.0),
			2.0);

		Assert.AreEqual(RouteCellDirection.Negative, segment.Direction);
		Assert.AreEqual(50.0, segment.PositionAt(TimeSpan.FromSeconds(15.0)).RoutePositionMetres);
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new LinearRouteMovementSegment(
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 0.0),
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 101.0),
			1.0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new LinearRouteMovementSegment(
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 0.0),
			new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 50.0),
			0.0));
	}

	[TestMethod]
	public void Movement_CheckpointAndStop_MaterialisesFakeClockAndChargesIdempotently()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var service = new RouteSpatialService(
			new RouteSpatialConfiguration(3.0, 10.0, 100.0, 500.0, 100.0),
			clock);
		var terrain = CreateTerrain(20.0);
		var gameworld = CreateGameworld();
		var occupants = new List<IPerceivable>();
		var (cell, _) = CreateRouteCell(3L, 1_000.0, 100.0, occupants, terrain.Object);
		var root = CreateCharacter(10L, cell.Object, gameworld.Object, 0.0);
		occupants.Add(root.Mock.Object);
		var store = new FakeRouteMotionPersistence();
		var schedules = new List<(Action Action, TimeSpan Delay)>();

		Assert.IsTrue(LinearRouteMovement.TryCreate(
			root.Mock.Object,
			100.0,
			out var movement,
			out var error,
			speedMetresPerSecond: 10.0,
			spatialService: service,
			persistence: store,
			schedule: (action, delay) => schedules.Add((action, delay)),
			checkpointInterval: TimeSpan.FromSeconds(3.0),
			createTracks: false), error);

		movement!.InitialAction();
		Assert.AreEqual(1, store.StartCount);
		Assert.AreEqual(TimeSpan.FromSeconds(3.0), schedules[0].Delay);
		CollectionAssert.AreEqual(new[] { EventType.RouteMovementBegin }, root.Events);
		clock.Advance(TimeSpan.FromSeconds(3.0));
		var firstCheckpoint = schedules[0].Action;
		firstCheckpoint();

		Assert.AreEqual(30.0, root.Position);
		Assert.AreEqual(6.0, root.StaminaSpent, 0.000001);
		Assert.AreEqual(1, store.CommittedSequences.Count);
		CollectionAssert.AreEqual(
			new[]
			{
				EventType.RouteMovementBegin,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress
			},
			root.Events);
		firstCheckpoint();
		Assert.AreEqual(6.0, root.StaminaSpent, 0.000001,
			"A stale/replayed checkpoint callback must not double charge resources.");

		clock.Advance(TimeSpan.FromSeconds(2.0));
		movement.StopMovement();
		Assert.AreEqual(50.0, root.Position, 0.000001);
		Assert.AreEqual(10.0, root.StaminaSpent, 0.000001);
		Assert.IsTrue(movement.Cancelled);
		Assert.AreEqual(1, store.CompleteCount);
		Assert.IsNull(root.Mock.Object.Movement);
		CollectionAssert.AreEqual(
			new[]
			{
				EventType.RouteMovementBegin,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress,
				EventType.RouteMovementCancelled
			},
			root.Events);
	}

	[TestMethod]
	public void Movement_CheckpointPersistenceFailure_StopsOperationWithoutReapplyingCharge()
	{
		var clock = new ManualTimeProvider();
		var service = new RouteSpatialService(RouteSpatialConfiguration.Default, clock);
		var terrain = CreateTerrain(20.0);
		var gameworld = CreateGameworld();
		var occupants = new List<IPerceivable>();
		var (cell, _) = CreateRouteCell(32L, 1_000.0, 100.0, occupants, terrain.Object);
		var root = CreateCharacter(12L, cell.Object, gameworld.Object, 0.0);
		occupants.Add(root.Mock.Object);
		var store = new FakeRouteMotionPersistence { ThrowAfterApplyingCharges = true };
		var schedules = new List<(Action Action, TimeSpan Delay)>();

		Assert.IsTrue(LinearRouteMovement.TryCreate(
			root.Mock.Object,
			100.0,
			out var movement,
			out var error,
			speedMetresPerSecond: 10.0,
			spatialService: service,
			persistence: store,
			schedule: (action, delay) => schedules.Add((action, delay)),
			checkpointInterval: TimeSpan.FromSeconds(3.0),
			createTracks: false), error);

		movement!.InitialAction();
		clock.Advance(TimeSpan.FromSeconds(3.0));
		var checkpoint = schedules.Single().Action;
		checkpoint();

		Assert.IsTrue(movement.Cancelled);
		Assert.AreEqual(0.0, root.Position, 0.000001);
		Assert.AreEqual(0.0, root.StaminaSpent, 0.000001);
		Assert.AreEqual(1, store.CompleteCount);
		checkpoint();
		Assert.AreEqual(0.0, root.StaminaSpent, 0.000001,
			"A failed durable checkpoint must restore its uncommitted in-memory charge.");
	}

	[TestMethod]
	public void Movement_CompleteFailure_LeavesCommittedArrivalAndDoesNotDoubleCharge()
	{
		var clock = new ManualTimeProvider();
		var service = new RouteSpatialService(RouteSpatialConfiguration.Default, clock);
		var terrain = CreateTerrain(20.0);
		var gameworld = CreateGameworld();
		var occupants = new List<IPerceivable>();
		var (cell, _) = CreateRouteCell(33L, 1_000.0, 100.0, occupants, terrain.Object);
		var root = CreateCharacter(13L, cell.Object, gameworld.Object, 0.0);
		occupants.Add(root.Mock.Object);
		var store = new FakeRouteMotionPersistence { ThrowOnComplete = true };
		var schedules = new List<(Action Action, TimeSpan Delay)>();

		Assert.IsTrue(LinearRouteMovement.TryCreate(
			root.Mock.Object,
			25.0,
			out var movement,
			out var error,
			speedMetresPerSecond: 10.0,
			spatialService: service,
			persistence: store,
			schedule: (action, delay) => schedules.Add((action, delay)),
			checkpointInterval: TimeSpan.FromSeconds(30.0),
			createTracks: false), error);

		movement!.InitialAction();
		clock.Advance(TimeSpan.FromSeconds(2.5));
		var arrival = schedules.Single().Action;
		arrival();

		Assert.AreEqual(25.0, root.Position, 0.000001);
		Assert.AreEqual(5.0, root.StaminaSpent, 0.000001);
		Assert.AreEqual(1, store.CompleteCount);
		Assert.IsNull(root.Mock.Object.Movement);
		arrival();
		Assert.AreEqual(5.0, root.StaminaSpent, 0.000001,
			"A cleanup failure after a durable arrival must not replay its resource charge.");
	}

	[TestMethod]
	public void RouteCheckpointSaveQueue_RestorePreservesPreexistingQueueOnly()
	{
		var saveManager = new SaveManager();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager);
		var preexisting = new QueueTestSaveable(gameworld.Object, 1L);
		var checkpointOnly = new QueueTestSaveable(gameworld.Object, 2L);
		preexisting.Changed = true;
		var states = RouteCheckpointSaveQueue.Capture([preexisting, checkpointOnly]);

		preexisting.Save();
		checkpointOnly.Changed = true;
		checkpointOnly.Save();
		RouteCheckpointSaveQueue.Restore(states);

		Assert.IsTrue(preexisting.Changed);
		Assert.IsTrue(saveManager.IsQueued(preexisting));
		Assert.IsFalse(checkpointOnly.Changed);
		Assert.IsFalse(saveManager.IsQueued(checkpointOnly));
	}

	[TestMethod]
	public void RouteCheckpointSaveQueue_FailedCheckpointKeepsRollbackMutationQueued()
	{
		var saveManager = new SaveManager();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager);
		var checkpointOnly = new QueueTestSaveable(gameworld.Object, 3L);
		var states = RouteCheckpointSaveQueue.Capture([checkpointOnly]);

		checkpointOnly.Changed = true;
		RouteCheckpointSaveQueue.PreserveRollback(states);

		Assert.IsTrue(checkpointOnly.Changed);
		Assert.IsTrue(saveManager.IsQueued(checkpointOnly));
	}

	[TestMethod]
	public void Movement_TargetArrival_CommitsExactDestinationAndCompletes()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var service = new RouteSpatialService(
			new RouteSpatialConfiguration(3.0, 10.0, 100.0, 500.0, 100.0),
			clock);
		var terrain = CreateTerrain(20.0);
		var gameworld = CreateGameworld();
		var occupants = new List<IPerceivable>();
		var (cell, _) = CreateRouteCell(31L, 1_000.0, 100.0, occupants, terrain.Object);
		var root = CreateCharacter(11L, cell.Object, gameworld.Object, 0.0);
		occupants.Add(root.Mock.Object);
		var store = new FakeRouteMotionPersistence();
		var schedules = new List<(Action Action, TimeSpan Delay)>();

		Assert.IsTrue(LinearRouteMovement.TryCreate(
			root.Mock.Object,
			25.0,
			out var movement,
			out var error,
			speedMetresPerSecond: 10.0,
			spatialService: service,
			persistence: store,
			schedule: (action, delay) => schedules.Add((action, delay)),
			checkpointInterval: TimeSpan.FromSeconds(30.0),
			createTracks: false), error);

		movement!.InitialAction();
		Assert.AreEqual(TimeSpan.FromSeconds(2.5), schedules.Single().Delay);
		clock.Advance(TimeSpan.FromSeconds(2.5));
		schedules.Single().Action();

		Assert.AreEqual(25.0, root.Position, 0.000001);
		Assert.AreEqual(5.0, root.StaminaSpent, 0.000001);
		Assert.IsFalse(movement.Cancelled);
		Assert.AreEqual(1, store.CompleteCount);
		Assert.IsNull(root.Mock.Object.Movement);
		CollectionAssert.AreEqual(
			new[]
			{
				EventType.RouteMovementBegin,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress,
				EventType.RouteMovementComplete
			},
			root.Events);
	}

	[TestMethod]
	public void TryCreate_MountRiderAndCharacterHitch_BuildsOneAtomicReferenceCohort()
	{
		var service = new RouteSpatialService(
			new RouteSpatialConfiguration(3.0, 10.0, 100.0, 500.0, 100.0));
		var terrain = CreateTerrain(20.0);
		var gameworld = CreateGameworld();
		var occupants = new List<IPerceivable>();
		var (cell, _) = CreateRouteCell(4L, 1_000.0, 100.0, occupants, terrain.Object);
		var mount = CreateCharacter(20L, cell.Object, gameworld.Object, 100.0);
		var rider = CreateCharacter(21L, cell.Object, gameworld.Object, 100.0);
		rider.Mock.SetupGet(x => x.RidingMount).Returns(mount.Mock.Object);
		mount.Mock.SetupGet(x => x.Riders).Returns([rider.Mock.Object]);
		var wagonPosition = 100.0;
		var wagon = new Mock<IPerceivable>();
		wagon.SetupGet(x => x.Id).Returns(30L);
		wagon.SetupGet(x => x.Name).Returns("wagon");
		wagon.SetupGet(x => x.Location).Returns(cell.Object);
		wagon.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		wagon.SetupGet(x => x.RoutePositionMetres).Returns(() => wagonPosition);
		wagon.SetupGet(x => x.SpatialLocation)
			.Returns(() => new SpatialLocation(cell.Object, RoomLayer.GroundLevel, wagonPosition));
		wagon.Setup(x => x.SetRoutePosition(It.IsAny<double?>()))
			.Callback((double? value) => wagonPosition = value!.Value);
		var hitch = new CharacterHitch(mount.Mock.Object, wagon.Object, 1.0);
		mount.Mock.Setup(x => x.EffectsOfType<CharacterHitch>(It.IsAny<Predicate<CharacterHitch>>()))
			.Returns([hitch]);
		occupants.Add(mount.Mock.Object);
		occupants.Add(rider.Mock.Object);
		occupants.Add(wagon.Object);

		Assert.IsTrue(LinearRouteMovement.TryCreate(
			mount.Mock.Object,
			200.0,
			out var movement,
			out var error,
			speedMetresPerSecond: 5.0,
			spatialService: service,
			persistence: new FakeRouteMotionPersistence(),
			schedule: (_, _) => { },
			checkpointInterval: TimeSpan.FromSeconds(3.0),
			createTracks: false), error);

		Assert.AreEqual(2, movement!.CharacterMovers.Count());
		Assert.AreEqual(1, movement.Mounts.Count());
		Assert.AreEqual(1, movement.NonConsensualMovers.Count());
		Assert.AreEqual(1, movement.Targets.Count());
		Assert.IsTrue(movement.Targets.Contains(wagon.Object));
	}

	private static Mock<IFuturemud> CreateGameworld()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("RouteCellMovementCheckpointSeconds")).Returns(30.0);
		gameworld.Setup(x => x.GetStaticDouble("DefaultTerrainStaminaCost")).Returns(20.0);
		gameworld.Setup(x => x.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage")).Returns(1.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellImmediateDistanceMetres")).Returns(3.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellProximateDistanceMetres")).Returns(10.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellDistantDistanceMetres")).Returns(100.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellVeryDistantDistanceMetres")).Returns(500.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellDefaultRoomEquivalentMetres")).Returns(100.0);
		return gameworld;
	}

	private static Mock<ITerrain> CreateTerrain(double staminaCost)
	{
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.StaminaCost).Returns(staminaCost);
		terrain.SetupGet(x => x.CanHaveTracks).Returns(false);
		return terrain;
	}

	private static CharacterHarness CreateCharacter(
		long id,
		ICell cell,
		IFuturemud gameworld,
		double initialPosition)
	{
		var position = initialPosition;
		const double initialStamina = 1_000.0;
		var currentStamina = initialStamina;
		var movement = default(IMovement);
		var events = new List<EventType>();
		var character = new Mock<ICharacter>();
		var output = new Mock<IOutputHandler>();
		var positionState = new Mock<IPositionState>();
		var speed = new Mock<IMoveSpeed>();
		var body = new Mock<IBody>();
		body.SetupGet(x => x.CurrentStamina).Returns(() => currentStamina);
		body.SetupSet(x => x.CurrentStamina = It.IsAny<double>())
			.Callback((double value) => currentStamina = value);
		positionState.SetupGet(x => x.IgnoreTerrainStaminaCostsForMovement).Returns(false);
		speed.SetupGet(x => x.Multiplier).Returns(1.0);
		speed.SetupGet(x => x.StaminaMultiplier).Returns(1.0);
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns($"character {id}");
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Location).Returns(cell);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		character.SetupGet(x => x.RoutePositionMetres).Returns(() => position);
		character.SetupGet(x => x.SpatialLocation)
			.Returns(() => new SpatialLocation(cell, RoomLayer.GroundLevel, position));
		character.Setup(x => x.SetRoutePosition(It.IsAny<double?>()))
			.Callback((double? value) => position = value!.Value);
		character.SetupGet(x => x.OutputHandler).Returns(output.Object);
		character.SetupGet(x => x.PositionState).Returns(positionState.Object);
		character.SetupGet(x => x.CurrentSpeed).Returns(speed.Object);
		character.SetupGet(x => x.EncumbrancePercentage).Returns(0.0);
		character.SetupGet(x => x.Party).Returns((IParty)null!);
		character.SetupGet(x => x.RidingMount).Returns((ICharacter?)null);
		character.SetupGet(x => x.Riders).Returns(Array.Empty<ICharacter>());
		character.SetupGet(x => x.Following).Returns((IMove)null!);
		character.SetupGet(x => x.Movement).Returns(() => movement!);
		character.SetupSet(x => x.Movement = It.IsAny<IMovement>())
			.Callback((IMovement value) => movement = value);
		character.Setup(x => x.CanMove(It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
		character.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		character.Setup(x => x.SpendStamina(It.IsAny<double>()))
			.Callback((double value) => currentStamina -= value);
		character.Setup(x => x.EffectsOfType<Dragging>(It.IsAny<Predicate<Dragging>>()))
			.Returns(Array.Empty<Dragging>());
		character.Setup(x => x.EffectsOfType<CharacterHitch>(It.IsAny<Predicate<CharacterHitch>>()))
			.Returns(Array.Empty<CharacterHitch>());
		character.Setup(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()))
			.Callback<EventType, object[]>((eventType, _) => events.Add(eventType))
			.Returns(false);
		return new CharacterHarness(character, () => position, () => initialStamina - currentStamina, events);
	}

	private static (Mock<ICell> Cell, Mock<IRouteCellDefinition> Definition) CreateRouteCell(
		long id,
		double length,
		double roomEquivalent,
		IEnumerable<IPerceivable>? perceivables = null,
		ITerrain? terrain = null)
	{
		var cell = new Mock<ICell>();
		var definition = new Mock<IRouteCellDefinition>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.RouteDefinition).Returns(definition.Object);
		cell.SetupGet(x => x.Perceivables).Returns(() => perceivables ?? []);
		cell.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain ?? CreateTerrain(20.0).Object);
		definition.SetupGet(x => x.Cell).Returns(cell.Object);
		definition.SetupGet(x => x.LengthMetres).Returns(length);
		definition.SetupGet(x => x.DefaultPositionMetres).Returns(0.0);
		definition.SetupGet(x => x.MetresPerRoomEquivalent).Returns(roomEquivalent);
		definition.SetupGet(x => x.PositiveDirectionName).Returns("townward");
		definition.SetupGet(x => x.NegativeDirectionName).Returns("stationward");
		definition.SetupGet(x => x.TopologyVersion).Returns(1L);
		definition.SetupGet(x => x.Landmarks).Returns(Array.Empty<IRouteCellLandmark>());
		definition.SetupGet(x => x.ExitAnchors).Returns(Array.Empty<IRouteExitAnchor>());
		return (cell, definition);
	}

	private sealed class CharacterHarness(
		Mock<ICharacter> mock,
		Func<double> position,
		Func<double> staminaSpent,
		List<EventType> events)
	{
		public Mock<ICharacter> Mock { get; } = mock;
		public double Position => position();
		public double StaminaSpent => staminaSpent();
		public List<EventType> Events { get; } = events;
	}

	private sealed class FakeRouteMotionPersistence : IRouteMotionPersistence
	{
		private readonly HashSet<string> _resourceKeys = [];

		public int StartCount { get; private set; }
		public int CompleteCount { get; private set; }
		public HashSet<long> CommittedSequences { get; } = [];
		public bool ThrowAfterApplyingCharges { get; init; }
		public bool ThrowOnComplete { get; init; }

		public void Start(
			ICharacter rootMover,
			LinearRouteMovementSegment segment,
			Guid operationId,
			double targetMinimumMetres,
			double targetMaximumMetres,
			long? selectedExitId,
			IReadOnlyCollection<MudSharp.Form.Shape.ILocateable> cohort)
		{
			StartCount++;
		}

		public void CommitCheckpoint(
			RouteMotionCheckpoint checkpoint,
			Action<IReadOnlyCollection<RouteMotionResourceCharge>> applyCharges)
		{
			CommittedSequences.Add(checkpoint.Sequence);
			var committed = checkpoint.Charges
				.Where(x => _resourceKeys.Add(
					$"{checkpoint.OperationId:N}:{checkpoint.Sequence}:{x.Character.Id}:{x.ResourceKey}"))
				.ToArray();
			applyCharges(committed);
			if (ThrowAfterApplyingCharges)
			{
				throw new InvalidOperationException("Injected checkpoint commit failure.");
			}
		}

		public void Complete(Guid operationId)
		{
			CompleteCount++;
			if (ThrowOnComplete)
			{
				throw new InvalidOperationException("Injected completion cleanup failure.");
			}
		}
	}

	private sealed class QueueTestSaveable : SaveableItem
	{
		public QueueTestSaveable(IFuturemud gameworld, long id)
		{
			Gameworld = gameworld;
			_id = id;
			_name = $"queue test {id:N0}";
		}

		public override string FrameworkItemType => "QueueTestSaveable";

		public override void Save()
		{
			Changed = false;
		}
	}
}
