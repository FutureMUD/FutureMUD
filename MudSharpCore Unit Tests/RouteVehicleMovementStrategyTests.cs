#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteVehicleMovementStrategyTests
{
	[TestMethod]
	public void PoweredMovement_FakeClock_CheckpointsAndArrivesAtExactCoordinate()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var harness = CreatePoweredHarness(clock, (action, delay) => schedules.Add((action, delay)));

		Assert.IsTrue(harness.Strategy.TryBeginManualMove(
			harness.Vehicle.Object,
			harness.Actor.Object,
			50.0,
			out var reason,
			checkpointInterval: TimeSpan.FromSeconds(3.0)), reason);
		Assert.AreEqual(1, harness.Persistence.StartCount);
		Assert.AreEqual(TimeSpan.FromSeconds(3.0), schedules.Single().Delay);
		CollectionAssert.AreEqual(new[] { EventType.RouteMovementBegin }, harness.Events);

		clock.Advance(TimeSpan.FromSeconds(3.0));
		var firstCheckpoint = schedules[0].Action;
		firstCheckpoint();
		Assert.AreEqual(30.0, harness.Position, 0.000001);
		Assert.AreEqual(TimeSpan.FromSeconds(2.0), schedules[1].Delay);
		Assert.AreEqual(30.0, harness.PowerConsumed, 0.000001);
		CollectionAssert.AreEqual(
			new[]
			{
				EventType.RouteMovementBegin,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress
			},
			harness.Events);

		firstCheckpoint();
		Assert.AreEqual(30.0, harness.PowerConsumed, 0.000001,
			"A stale scheduled callback must not commit resources twice.");

		clock.Advance(TimeSpan.FromSeconds(2.0));
		schedules[1].Action();

		Assert.AreEqual(50.0, harness.Position, 0.000001);
		Assert.AreEqual(50.0, harness.ExteriorPosition, 0.000001);
		Assert.AreEqual(50.0, harness.PowerConsumed, 0.000001);
		Assert.AreEqual(1, harness.Persistence.CompleteCount);
		Assert.IsFalse(harness.Strategy.IsMoving(harness.Vehicle.Object));
		CollectionAssert.AreEqual(
			new[]
			{
				EventType.RouteMovementBegin,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress,
				EventType.RouteMovementComplete
			},
			harness.Events);
	}

	[TestMethod]
	public void OccupiedVehicle_CanonicalExteriorSync_KeepsDurableMotionActiveAcrossTwoCheckpoints()
	{
		var clock = new AdvancingTimeProvider(TimeSpan.FromMilliseconds(1.0));
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var gameworld = CreateGameworld();
		gameworld.SetupGet(x => x.DefaultHooks).Returns([]);
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.StaminaCost).Returns(1.0);
		terrain.SetupGet(x => x.CanHaveTracks).Returns(false);
		var perceivables = new List<IPerceivable>();
		var cell = CreateRouteCell(91L, 1_000.0, perceivables, terrain.Object);

		var exteriorComponentPrototype = CreateVehicleExteriorComponentProto(gameworld.Object);
		var coordinateObserver = new Mock<IGameItemComponent>();
		var coordinateObserverPrototype = new Mock<IGameItemComponentProto>();
		coordinateObserverPrototype
			.Setup(x => x.CreateNew(It.IsAny<IGameItem>(), It.IsAny<ICharacter?>(), It.IsAny<bool>()))
			.Returns(coordinateObserver.Object);
		var itemPrototype = new Mock<IGameItemProto>();
		itemPrototype.SetupGet(x => x.Id).Returns(901L);
		itemPrototype.SetupGet(x => x.Name).Returns("occupied wagon exterior");
		itemPrototype.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		itemPrototype.SetupGet(x => x.Components)
			.Returns([exteriorComponentPrototype, coordinateObserverPrototype.Object]);
		itemPrototype.SetupGet(x => x.Morphs).Returns(false);
		itemPrototype.SetupGet(x => x.Keywords).Returns(["occupied", "wagon", "exterior"]);
		var exterior = new GameItem(itemPrototype.Object);
		exterior.MoveTo(cell.Object, RoomLayer.GroundLevel);
		perceivables.Add(exterior);

		var driverPosition = 0.0;
		IMovement? driverMovement = null;
		var driver = new Mock<ICharacter>();
		driver.SetupGet(x => x.Id).Returns(902L);
		driver.SetupGet(x => x.Name).Returns("wagon driver");
		driver.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		driver.SetupGet(x => x.Location).Returns(cell.Object);
		driver.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		driver.SetupGet(x => x.RoutePositionMetres).Returns(() => driverPosition);
		driver.SetupGet(x => x.SpatialLocation)
			.Returns(() => new SpatialLocation(cell.Object, RoomLayer.GroundLevel, driverPosition));
		driver.Setup(x => x.SetRoutePosition(It.IsAny<double?>()))
			.Callback((double? value) => driverPosition = value!.Value);
		driver.SetupGet(x => x.Movement).Returns(() => driverMovement!);
		driver.SetupSet(x => x.Movement = It.IsAny<IMovement>())
			.Callback((IMovement value) => driverMovement = value);
		driver.Setup(x => x.SamePhysicalInstance(It.IsAny<ICharacter>()))
			.Returns((ICharacter other) => ReferenceEquals(other, driver.Object));
		driver.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		driver.SetupGet(x => x.Party).Returns((IParty)null!);
		perceivables.Add(driver.Object);

		var profile = CreateRouteProfile(RouteVehiclePropulsionMode.Powered, 10.0);
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
		vehiclePrototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		var vehiclePosition = 0.0;
		var vehicleOccupants = new List<ICharacter> { driver.Object };
		var forcedMoveStacks = new List<string>();
		var vehicle = CreateVehicle(903L, gameworld.Object, cell.Object, exterior, vehiclePrototype.Object,
			() => vehiclePosition, _ => { }, driver.Object);
		vehicle.SetupGet(x => x.Occupants).Returns(vehicleOccupants);
		vehicles.Setup(x => x.Get(903L)).Returns(vehicle.Object);
		var exteriorComponent = (VehicleExteriorGameItemComponent)exterior.GetItemType<IVehicleExterior>();
		exteriorComponent.LinkVehicle(vehicle.Object);
		vehicle.Setup(x => x.MaterialiseRoutePosition(It.IsAny<double>(), It.IsAny<bool>()))
			.Callback((double value, bool _) =>
			{
				vehiclePosition = value;
				Assert.IsTrue(exteriorComponent.TrySynchroniseRoutePosition(vehicle.Object, value));
			});
		vehicle.Setup(x => x.HandleExteriorItemForceMoved())
			.Callback(() =>
			{
				forcedMoveStacks.Add(Environment.StackTrace);
				driverMovement?.CancelForMoverOnly(driver.Object);
				vehicleOccupants.Clear();
			});

		var movePlan = CreateMovePlan(vehicle.Object);
		var readiness = CreateReadiness(vehicle.Object, profile.Object, movePlan, out _);
		var persistence = new FakeVehicleRouteMotionPersistence();
		var strategy = new RouteVehicleMovementStrategy(
			new RouteSpatialService(RouteSpatialConfiguration.Default, clock),
			readiness.Object,
			new Mock<IVehicleHitchGraphService>().Object,
			persistence,
			schedule: (_, action, delay) => schedules.Add((action, delay)));

		Assert.IsTrue(strategy.TryBeginManualMove(
			vehicle.Object,
			driver.Object,
			100.0,
			out var reason,
			checkpointInterval: TimeSpan.FromSeconds(2.0)), reason);
		Assert.AreEqual(1, vehicleOccupants.Count);

		clock.Advance(TimeSpan.FromSeconds(2.0));
		schedules[0].Action();
		vehicle.Verify(x => x.HandleExteriorItemForceMoved(), Times.Never);
		Assert.IsTrue(persistence.IsActive);
		Assert.AreEqual(1L, persistence.LastCheckpointSequence);
		var firstCheckpointPosition = vehiclePosition;
		Assert.IsTrue(firstCheckpointPosition is > 20.0 and < 21.0);
		Assert.AreEqual(firstCheckpointPosition, exterior.RoutePositionMetres!.Value, 0.000001);
		Assert.AreEqual(firstCheckpointPosition, driverPosition, 0.000001);
		Assert.AreEqual(firstCheckpointPosition, persistence.LastCheckpointPosition, 0.000001);
		Assert.AreEqual(1, vehicleOccupants.Count, string.Join("\n---\n", forcedMoveStacks));
		Assert.AreSame(driver.Object, vehicleOccupants.Single());
		Assert.IsNotNull(driverMovement);

		clock.Advance(TimeSpan.FromSeconds(2.0));
		schedules[1].Action();
		vehicle.Verify(x => x.HandleExteriorItemForceMoved(), Times.Never,
			string.Join("\n---\n", forcedMoveStacks));
		Assert.IsTrue(persistence.IsActive,
			"A canonical exterior checkpoint must not reentrantly complete its own durable motion row.");
		Assert.AreEqual(2L, persistence.LastCheckpointSequence);
		Assert.IsTrue(vehiclePosition > firstCheckpointPosition);
		Assert.AreEqual(vehiclePosition, exterior.RoutePositionMetres!.Value, 0.000001);
		Assert.AreEqual(vehiclePosition, driverPosition, 0.000001);
		Assert.AreEqual(vehiclePosition, persistence.LastCheckpointPosition, 0.000001);
		Assert.AreEqual(1, vehicleOccupants.Count,
			"Canonical exterior synchronization must not disembark the driver.");
		Assert.AreSame(driver.Object, vehicleOccupants.Single());
		Assert.IsNotNull(driverMovement);
		vehicle.Verify(x => x.HandleExteriorItemForceMoved(), Times.Never);
		coordinateObserver.Verify(x => x.ForceMove(), Times.Exactly(2),
			"Non-exterior components must still revalidate at every canonical coordinate change.");

		Assert.IsTrue(strategy.TryStop(vehicle.Object, null, out var stopReason), stopReason);
		Assert.IsFalse(persistence.IsActive);
	}

	[TestMethod]
	public void ExternallyPulledRecursiveCohort_AdvancingClock_CommitsOneSharedPositionWithoutReentrantComplete()
	{
		var clock = new AdvancingTimeProvider(TimeSpan.FromMilliseconds(1.0));
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var gameworld = CreateGameworld();
		gameworld.SetupGet(x => x.DefaultHooks).Returns([]);
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.StaminaCost).Returns(1.0);
		terrain.SetupGet(x => x.CanHaveTracks).Returns(false);
		var perceivables = new List<IPerceivable>();
		var cell = CreateRouteCell(92L, 1_000.0, perceivables, terrain.Object);

		var exteriorPrototype = CreateVehicleExteriorComponentProto(gameworld.Object);
		var rootExterior = CreateRouteGameItem(gameworld.Object, cell.Object, 921L,
			"lead wagon exterior", [exteriorPrototype]);
		var trailerExterior = CreateRouteGameItem(gameworld.Object, cell.Object, 922L,
			"trailing wagon exterior", [exteriorPrototype]);
		var harnessComponent = new Mock<IHitchGear>();
		var harnessPrototype = CreateComponentPrototype(harnessComponent.Object);
		var harness = CreateRouteGameItem(gameworld.Object, cell.Object, 923L,
			"physical horse harness", [harnessPrototype.Object]);
		var towbarComponent = new Mock<IHitchGear>();
		var towbarPrototype = CreateComponentPrototype(towbarComponent.Object);
		var towbar = CreateRouteGameItem(gameworld.Object, cell.Object, 924L,
			"physical wagon towbar", [towbarPrototype.Object]);
		perceivables.AddRange([rootExterior, trailerExterior, harness, towbar]);

		var pullerPosition = 0.0;
		var puller = CreatePuller(925L, cell.Object, gameworld.Object, () => pullerPosition,
			value => pullerPosition = value, new Queue<bool>(Enumerable.Repeat(true, 20)));
		var driverPosition = 0.0;
		var driver = CreatePuller(926L, cell.Object, gameworld.Object, () => driverPosition,
			value => driverPosition = value, new Queue<bool>(Enumerable.Repeat(true, 20)));
		perceivables.Add(puller.Object);
		perceivables.Add(driver.Object);

		var profile = CreateRouteProfile(RouteVehiclePropulsionMode.ExternallyPulled, 0.0);
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
		vehiclePrototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		var rootPosition = 0.0;
		var trailerPosition = 0.0;
		var root = CreateVehicle(927L, gameworld.Object, cell.Object, rootExterior,
			vehiclePrototype.Object, () => rootPosition, _ => { }, driver.Object);
		var trailer = CreateVehicle(928L, gameworld.Object, cell.Object, trailerExterior,
			vehiclePrototype.Object, () => trailerPosition, _ => { }, null!);
		var rootOccupants = new List<ICharacter> { driver.Object };
		root.SetupGet(x => x.Occupants).Returns(rootOccupants);
		vehicles.Setup(x => x.Get(927L)).Returns(root.Object);
		vehicles.Setup(x => x.Get(928L)).Returns(trailer.Object);
		var rootExteriorComponent = (VehicleExteriorGameItemComponent)rootExterior.GetItemType<IVehicleExterior>();
		var trailerExteriorComponent =
			(VehicleExteriorGameItemComponent)trailerExterior.GetItemType<IVehicleExterior>();
		rootExteriorComponent.LinkVehicle(root.Object);
		trailerExteriorComponent.LinkVehicle(trailer.Object);
		root.Setup(x => x.MaterialiseRoutePosition(It.IsAny<double>(), It.IsAny<bool>()))
			.Callback((double value, bool _) =>
			{
				rootPosition = value;
				Assert.IsTrue(rootExteriorComponent.TrySynchroniseRoutePosition(root.Object, value));
			});
		trailer.Setup(x => x.MaterialiseRoutePosition(It.IsAny<double>(), It.IsAny<bool>()))
			.Callback((double value, bool _) =>
			{
				trailerPosition = value;
				Assert.IsTrue(trailerExteriorComponent.TrySynchroniseRoutePosition(trailer.Object, value));
			});
		root.Setup(x => x.HandleExteriorItemForceMoved())
			.Callback(() =>
			{
				driver.Object.Movement?.CancelForMoverOnly(driver.Object);
				rootOccupants.Clear();
			});
		trailer.Setup(x => x.HandleExteriorItemForceMoved())
			.Callback(() => driver.Object.Movement?.CancelForMoverOnly(driver.Object));

		var motiveLink = new VehicleHitchGraphLink(
			"character:925->vehicle:927",
			VehicleHitchGraphLinkKind.PersistentHitch,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Character, null, puller.Object, null),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, root.Object, null, null),
			harness,
			harness.Id,
			false,
			false,
			string.Empty,
			null);
		var recursiveTowLink = new VehicleHitchGraphLink(
			"vehicle:927->vehicle:928",
			VehicleHitchGraphLinkKind.PersistentHitch,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, root.Object, null, null),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, trailer.Object, null, null),
			towbar,
			towbar.Id,
			false,
			false,
			string.Empty,
			null);
		var movePlan = new VehicleHitchGraphMovePlan(
			root.Object,
			[
				new VehicleHitchGraphTrainMember(root.Object, 0, null),
				new VehicleHitchGraphTrainMember(trailer.Object, 1, recursiveTowLink)
			],
			[recursiveTowLink],
			[harness, towbar],
			400_000.0);
		var graph = new Mock<IVehicleHitchGraphService>();
		graph.Setup(x => x.LinksInvolving(gameworld.Object, rootExterior))
			.Returns([motiveLink, recursiveTowLink]);
		var readiness = CreateReadiness(root.Object, profile.Object, movePlan, out _);
		var persistence = new FakeVehicleRouteMotionPersistence();
		var strategy = new RouteVehicleMovementStrategy(
			new RouteSpatialService(RouteSpatialConfiguration.Default, clock),
			readiness.Object,
			graph.Object,
			persistence,
			schedule: (_, action, delay) => schedules.Add((action, delay)));

		Assert.IsTrue(strategy.TryBeginManualMove(
			root.Object,
			driver.Object,
			100.0,
			out var reason,
			checkpointInterval: TimeSpan.FromSeconds(2.0)), reason);

		for (var index = 0; index < 2; index++)
		{
			clock.Advance(TimeSpan.FromSeconds(2.0));
			schedules[index].Action();
			Assert.IsTrue(persistence.IsActive);
			Assert.AreEqual(index + 1L, persistence.LastCheckpointSequence);
			Assert.AreEqual(0, persistence.CompleteCount,
				"No participant may reentrantly complete the operation during an outer checkpoint.");
			var shared = persistence.LastCheckpointPosition;
			Assert.AreEqual(shared, rootPosition, 0.000001);
			Assert.AreEqual(shared, trailerPosition, 0.000001);
			Assert.AreEqual(shared, rootExterior.RoutePositionMetres!.Value, 0.000001);
			Assert.AreEqual(shared, trailerExterior.RoutePositionMetres!.Value, 0.000001);
			Assert.AreEqual(shared, harness.RoutePositionMetres!.Value, 0.000001);
			Assert.AreEqual(shared, towbar.RoutePositionMetres!.Value, 0.000001);
			Assert.AreEqual(shared, pullerPosition, 0.000001);
			Assert.AreEqual(shared, driverPosition, 0.000001);
			Assert.AreEqual(1, rootOccupants.Count);
			Assert.AreSame(driver.Object, rootOccupants.Single());
			Assert.IsNotNull(driver.Object.Movement);
			Assert.IsNotNull(puller.Object.Movement);
		}

		root.Verify(x => x.HandleExteriorItemForceMoved(), Times.Never);
		trailer.Verify(x => x.HandleExteriorItemForceMoved(), Times.Never);
		harnessComponent.Verify(x => x.ForceMove(), Times.Exactly(2));
		towbarComponent.Verify(x => x.ForceMove(), Times.Exactly(2));
		Assert.IsTrue(strategy.TryStop(root.Object, null, out var stopReason), stopReason);
		Assert.IsFalse(persistence.IsActive);
	}

	[TestMethod]
	public void PoweredMovement_StopMaterialisesCurrentLazyPosition()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var harness = CreatePoweredHarness(clock, (action, delay) => schedules.Add((action, delay)));

		Assert.IsTrue(harness.Strategy.TryBeginManualMove(
			harness.Vehicle.Object,
			harness.Actor.Object,
			100.0,
			out var beginReason,
			checkpointInterval: TimeSpan.FromSeconds(30.0)), beginReason);
		clock.Advance(TimeSpan.FromSeconds(2.0));

		Assert.IsTrue(harness.Strategy.TryStop(harness.Vehicle.Object, null, out var stopReason), stopReason);

		Assert.AreEqual(20.0, harness.Position, 0.000001);
		Assert.AreEqual(20.0, harness.ExteriorPosition, 0.000001);
		Assert.AreEqual(20.0, harness.PowerConsumed, 0.000001);
		Assert.AreEqual(1, harness.Persistence.CompleteCount);
		Assert.IsFalse(harness.Strategy.IsMoving(harness.Vehicle.Object));
		CollectionAssert.AreEqual(
			new[]
			{
				EventType.RouteMovementBegin,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress,
				EventType.RouteMovementCancelled
			},
			harness.Events);
	}

	[TestMethod]
	public void PoweredMovement_CheckpointPersistenceFailure_StopsWithoutReapplyingPower()
	{
		var clock = new ManualTimeProvider();
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var harness = CreatePoweredHarness(clock, (action, delay) => schedules.Add((action, delay)));
		harness.Persistence.ThrowAfterApplyingCharges = true;

		Assert.IsTrue(harness.Strategy.TryBeginManualMove(
			harness.Vehicle.Object,
			harness.Actor.Object,
			100.0,
			out var reason,
			checkpointInterval: TimeSpan.FromSeconds(3.0)), reason);
		clock.Advance(TimeSpan.FromSeconds(3.0));
		var checkpoint = schedules.Single().Action;
		checkpoint();

		Assert.AreEqual(0.0, harness.Position, 0.000001);
		Assert.AreEqual(0.0, harness.ExteriorPosition, 0.000001);
		Assert.AreEqual(0.0, harness.PowerConsumed, 0.000001);
		Assert.AreEqual(1, harness.Persistence.CompleteCount);
		Assert.IsFalse(harness.Strategy.IsMoving(harness.Vehicle.Object));
		checkpoint();
		Assert.AreEqual(0.0, harness.PowerConsumed, 0.000001,
			"A failed durable checkpoint must restore uncommitted power consumption.");
	}

	[TestMethod]
	public void PoweredMovement_ResourceApplyFailure_RestoresPositionAndPower()
	{
		var clock = new ManualTimeProvider();
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var harness = CreatePoweredHarness(
			clock,
			(action, delay) => schedules.Add((action, delay)),
			throwAfterPositiveResourceApply: true);

		Assert.IsTrue(harness.Strategy.TryBeginManualMove(
			harness.Vehicle.Object,
			harness.Actor.Object,
			100.0,
			out var reason,
			checkpointInterval: TimeSpan.FromSeconds(3.0)), reason);
		clock.Advance(TimeSpan.FromSeconds(3.0));
		var checkpoint = schedules.Single().Action;
		checkpoint();

		Assert.AreEqual(0.0, harness.Position, 0.000001);
		Assert.AreEqual(0.0, harness.ExteriorPosition, 0.000001);
		Assert.AreEqual(0.0, harness.PowerConsumed, 0.000001);
		Assert.AreEqual(1, harness.Persistence.CompleteCount);
		Assert.IsFalse(harness.Strategy.IsMoving(harness.Vehicle.Object));
		checkpoint();
		Assert.AreEqual(0.0, harness.PowerConsumed, 0.000001,
			"A resource callback that throws after mutation must still be rolled back exactly once.");
	}

	[TestMethod]
	public void PoweredMovement_CompleteFailure_LeavesCommittedArrivalAndDoesNotDoubleCharge()
	{
		var clock = new ManualTimeProvider();
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var harness = CreatePoweredHarness(clock, (action, delay) => schedules.Add((action, delay)));
		harness.Persistence.ThrowOnComplete = true;

		Assert.IsTrue(harness.Strategy.TryBeginManualMove(
			harness.Vehicle.Object,
			harness.Actor.Object,
			25.0,
			out var reason,
			checkpointInterval: TimeSpan.FromSeconds(30.0)), reason);
		clock.Advance(TimeSpan.FromSeconds(2.5));
		var arrival = schedules.Single().Action;
		arrival();

		Assert.AreEqual(25.0, harness.Position, 0.000001);
		Assert.AreEqual(25.0, harness.ExteriorPosition, 0.000001);
		Assert.AreEqual(25.0, harness.PowerConsumed, 0.000001);
		Assert.AreEqual(1, harness.Persistence.CompleteCount);
		Assert.IsFalse(harness.Strategy.IsMoving(harness.Vehicle.Object));
		arrival();
		Assert.AreEqual(25.0, harness.PowerConsumed, 0.000001,
			"A cleanup failure after a durable arrival must not replay its resource charge.");
	}

	[TestMethod]
	public void PoweredMovement_EmitsVehicleOwnedRouteTrack()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var harness = CreatePoweredHarness(
			clock,
			(action, delay) => schedules.Add((action, delay)),
			trackingEnabled: true);

		Assert.IsTrue(harness.Strategy.TryBeginManualMove(
			harness.Vehicle.Object,
			harness.Actor.Object,
			50.0,
			out var reason,
			checkpointInterval: TimeSpan.FromSeconds(3.0)), reason);

		var track = harness.Tracks.Single();
		Assert.AreSame(harness.Vehicle.Object, track.Vehicle);
		Assert.IsNull(track.Character);
		Assert.IsNull(track.BodyProtoType);
		Assert.AreEqual(0.0, track.RoutePositionMetres);
		Assert.AreEqual(RouteCellDirection.Positive, track.RouteDirection);
	}

	[TestMethod]
	public void ExternallyPulledMovement_StaminaFailureFreezesWholeCohortAtCheckpoint()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var gameworld = CreateGameworld();
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.StaminaCost).Returns(1.0);
		var occupants = new List<IPerceivable>();
		var cell = CreateRouteCell(2L, 1_000.0, occupants, terrain.Object);
		var pullerPosition = 0.0;
		var canPay = new Queue<bool>([true, false]);
		var puller = CreatePuller(20L, cell.Object, gameworld.Object, () => pullerPosition,
			value => pullerPosition = value, canPay);
		var exteriorPosition = 0.0;
		var exterior = CreateExterior(201L, cell.Object, () => exteriorPosition,
			value => exteriorPosition = value);
		occupants.Add(puller.Object);
		occupants.Add(exterior.Object);
		var profile = CreateRouteProfile(RouteVehiclePropulsionMode.ExternallyPulled, 0.0);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		var vehiclePosition = 0.0;
		var vehicle = CreateVehicle(2L, gameworld.Object, cell.Object, exterior.Object, prototype.Object,
			() => vehiclePosition, value =>
			{
				vehiclePosition = value;
				exteriorPosition = value;
			}, puller.Object);
		var movePlan = CreateMovePlan(vehicle.Object);
		var readiness = CreateReadiness(vehicle.Object, profile.Object, movePlan, out _);
		var graph = new Mock<IVehicleHitchGraphService>();
		var motiveLink = new VehicleHitchGraphLink(
			"puller",
			VehicleHitchGraphLinkKind.TransientCharacterHitch,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Character, null, puller.Object, null),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, vehicle.Object, null, null),
			null,
			null,
			false,
			false,
			string.Empty,
			null);
		graph.Setup(x => x.LinksInvolving(gameworld.Object, exterior.Object)).Returns([motiveLink]);
		var persistence = new FakeVehicleRouteMotionPersistence();
		var spatial = new RouteSpatialService(RouteSpatialConfiguration.Default, clock);
		var strategy = new RouteVehicleMovementStrategy(
			spatial,
			readiness.Object,
			graph.Object,
			persistence,
			schedule: (_, action, delay) => schedules.Add((action, delay)));

		Assert.IsTrue(strategy.TryBeginManualMove(
			vehicle.Object,
			puller.Object,
			100.0,
			out var reason,
			checkpointInterval: TimeSpan.FromSeconds(2.0)), reason);
		Assert.IsNotNull(puller.Object.Movement,
			"The external motive character must be enrolled in the vehicle's atomic movement cohort.");
		clock.Advance(TimeSpan.FromSeconds(2.0));
		schedules.Single().Action();

		Assert.AreEqual(2.8, vehiclePosition, 0.000001);
		Assert.AreEqual(2.8, exteriorPosition, 0.000001);
		Assert.AreEqual(2.8, pullerPosition, 0.000001);
		Assert.AreEqual(2.8, persistence.LastCheckpointPosition, 0.000001);
		Assert.AreEqual(1, persistence.CompleteCount);
		puller.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
		Assert.IsNull(puller.Object.Movement);
		Assert.IsFalse(strategy.IsMoving(vehicle.Object));
		foreach (var eventType in new[]
		         {
			         EventType.RouteMovementBegin,
			         EventType.RoutePositionChanged,
			         EventType.RouteMovementProgress,
			         EventType.RouteMovementCancelled
		         })
		{
			puller.Verify(x => x.HandleEvent(eventType, It.IsAny<object[]>()), Times.Once);
			exterior.Verify(x => x.HandleEvent(eventType, It.IsAny<object[]>()), Times.Once);
		}
	}

	[TestMethod]
	public void ExternallyPulledCompiledExit_MovesMotiveCharacterAndRecursiveVehicleTrainTogether()
	{
		var harness = CreateExternallyPulledExitHarness();
		VehicleJourneyLegResult? completion = null;

		var started = harness.Strategy.TryBeginManualRoute(
			harness.Root.Object,
			harness.Driver.Object,
			harness.Route.Object,
			result => completion = result,
			out var reason);

		Assert.IsTrue(started, reason);
		Assert.IsNotNull(completion);
		Assert.IsTrue(completion.Succeeded, completion.Reason);
		harness.Readiness.Verify(x => x.BuildMovementReadiness(
			It.Is<VehicleMovementReadinessRequest>(request =>
				request.ExternalPullers != null &&
				request.ExternalPullers.Count == 1 &&
				ReferenceEquals(request.ExternalPullers.Single(), harness.Puller.Object))), Times.Once);
		harness.Puller.Verify(x => x.ExecuteMove(It.Is<IMovement>(movement =>
			movement.Exit == harness.Exit.Object &&
			movement.IsMovementLeader(harness.Puller.Object))), Times.Once);
		harness.Root.Verify(x => x.BeginMoveToCell(harness.Destination.Object, RoomLayer.GroundLevel,
			harness.Exit.Object), Times.Once);
		harness.Trailer.Verify(x => x.BeginMoveToCell(harness.Destination.Object, RoomLayer.GroundLevel,
			harness.Exit.Object), Times.Once);
		harness.Root.Verify(x => x.MaterialiseRoutePosition(7_150.0, true), Times.Once);
		harness.Trailer.Verify(x => x.MaterialiseRoutePosition(7_150.0, true), Times.Once);
		harness.Puller.Verify(x => x.SetRoutePosition(7_150.0), Times.Once);
		foreach (var hitchItem in harness.HitchItems)
		{
			hitchItem.Verify(x => x.SetRoutePosition(7_150.0), Times.Once);
		}
		harness.Graph.Verify(x => x.CompleteVehicleTrainMove(
			harness.MovePlan,
			harness.Destination.Object,
			RoomLayer.GroundLevel,
			harness.Exit.Object,
			It.Is<IMovement>(movement => movement.IsMovementLeader(harness.Puller.Object)),
			null), Times.Once);
	}

	[TestMethod]
	public void ExternallyPulledCompiledExit_PreservesExactReadinessFailureReason()
	{
		const string expected = "The rear wagon towbar is broken.";
		var harness = CreateExternallyPulledExitHarness(expected);

		var started = harness.Strategy.TryBeginManualRoute(
			harness.Root.Object,
			harness.Driver.Object,
			harness.Route.Object,
			_ => Assert.Fail("A rejected portal step must not report route completion."),
			out var reason);

		Assert.IsFalse(started);
		Assert.AreEqual(expected, reason);
		harness.Puller.Verify(x => x.ExecuteMove(It.IsAny<IMovement>()), Times.Never);
			harness.Graph.Verify(x => x.CompleteVehicleTrainMove(
			It.IsAny<VehicleHitchGraphMovePlan>(),
			It.IsAny<ICell>(),
			It.IsAny<RoomLayer>(),
			It.IsAny<ICellExit>(),
			It.IsAny<IMovement>(),
			It.IsAny<IVehicle>()), Times.Never);
	}

	[TestMethod]
	public void AutomaticCompiledExit_OrdinaryToOrdinary_UsesAuthoritativeCellExitCommitPath()
	{
		var harness = CreateAutomaticExitHarness(null);
		VehicleJourneyLegResult? completion = null;

		var started = harness.Strategy.TryBeginLeg(
			harness.Journey.Object,
			harness.Leg.Object,
			result => completion = result,
			out var reason);

		Assert.IsTrue(started, reason);
		Assert.IsNotNull(completion);
		Assert.IsTrue(completion.Succeeded, completion.Reason);
		harness.Readiness.Verify(x => x.BuildMovementReadiness(
			It.Is<VehicleMovementReadinessRequest>(request =>
				request.AutomaticOperation && request.Actor == null &&
				ReferenceEquals(request.MovementProfile, harness.Profile.Object) &&
				ReferenceEquals(request.Exit, harness.Exit.Object))), Times.Once);
		harness.Readiness.Verify(x => x.RollTowCatastrophe(harness.MovePlan, null), Times.Once);
		harness.Readiness.Verify(x => x.ConsumeMovementResources(harness.ResourcePlan), Times.Once);
		harness.Root.Verify(x => x.BeginMoveToCell(harness.Destination.Object, RoomLayer.GroundLevel,
			harness.Exit.Object), Times.Once);
		harness.Trailer.Verify(x => x.BeginMoveToCell(harness.Destination.Object, RoomLayer.GroundLevel,
			harness.Exit.Object), Times.Once);
		harness.Graph.Verify(x => x.CompleteVehicleTrainMove(harness.MovePlan, harness.Destination.Object,
			RoomLayer.GroundLevel, harness.Exit.Object, null, null), Times.Once);
		harness.Root.Verify(x => x.MaterialiseRoutePosition(It.IsAny<double>(), It.IsAny<bool>()), Times.Never);
		harness.Trailer.Verify(x => x.MaterialiseRoutePosition(It.IsAny<double>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void AutomaticCompiledExit_OrdinaryToRouteCell_LandsWholeTrainAtPinnedCoordinate()
	{
		var harness = CreateAutomaticExitHarness(7_150.0);

		var started = harness.Strategy.TryBeginLeg(
			harness.Journey.Object,
			harness.Leg.Object,
			_ => { },
			out var reason);

		Assert.IsTrue(started, reason);
		harness.Root.Verify(x => x.MaterialiseRoutePosition(7_150.0, true), Times.Once);
		harness.Trailer.Verify(x => x.MaterialiseRoutePosition(7_150.0, true), Times.Once);
	}

	[TestMethod]
	public void AutomaticCompiledExit_ClosedDoor_FailsBeforeReadinessOrMutation()
	{
		var harness = CreateAutomaticExitHarness(null, doorClosed: true);

		var started = harness.Strategy.TryBeginLeg(
			harness.Journey.Object,
			harness.Leg.Object,
			_ => Assert.Fail("A closed portal must not complete a route leg."),
			out var reason);

		Assert.IsFalse(started);
		Assert.AreEqual("The door through that exit is closed.", reason);
		harness.Readiness.Verify(x => x.BuildMovementReadiness(It.IsAny<VehicleMovementReadinessRequest>()), Times.Never);
		harness.Readiness.Verify(x => x.ConsumeMovementResources(It.IsAny<VehicleResourceReadinessPlan>()), Times.Never);
		harness.Graph.Verify(x => x.CompleteVehicleTrainMove(It.IsAny<VehicleHitchGraphMovePlan>(),
			It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<ICellExit>(), It.IsAny<IMovement>(),
			It.IsAny<IVehicle>()), Times.Never);
	}

	[TestMethod]
	public void AutomaticCompiledExit_EnvironmentOrSizeFailure_PreservesReadinessReasonWithoutMutation()
	{
		const string expected = "The towed carriage cannot traverse the destination environment.";
		var harness = CreateAutomaticExitHarness(null, readinessFailure: expected);

		var started = harness.Strategy.TryBeginLeg(
			harness.Journey.Object,
			harness.Leg.Object,
			_ => Assert.Fail("A rejected portal must not complete a route leg."),
			out var reason);

		Assert.IsFalse(started);
		Assert.AreEqual(expected, reason);
		harness.Readiness.Verify(x => x.ConsumeMovementResources(It.IsAny<VehicleResourceReadinessPlan>()), Times.Never);
		harness.Root.Verify(x => x.BeginMoveToCell(It.IsAny<ICell>(), It.IsAny<RoomLayer>(),
			It.IsAny<ICellExit>()), Times.Never);
		harness.Trailer.Verify(x => x.BeginMoveToCell(It.IsAny<ICell>(), It.IsAny<RoomLayer>(),
			It.IsAny<ICellExit>()), Times.Never);
	}

	[TestMethod]
	public void AutomaticCompiledExit_TowCatastrophe_FailsBeforeResourcesOrRelocation()
	{
		const string expected = "The rear hitch catastrophically fails under strain.";
		var harness = CreateAutomaticExitHarness(null, catastropheReason: expected);

		var started = harness.Strategy.TryBeginLeg(
			harness.Journey.Object,
			harness.Leg.Object,
			_ => Assert.Fail("A tow catastrophe must not complete a route leg."),
			out var reason);

		Assert.IsFalse(started);
		Assert.AreEqual(expected, reason);
		harness.Readiness.Verify(x => x.RollTowCatastrophe(harness.MovePlan, null), Times.Once);
		harness.Readiness.Verify(x => x.ConsumeMovementResources(It.IsAny<VehicleResourceReadinessPlan>()), Times.Never);
		harness.Root.Verify(x => x.BeginMoveToCell(It.IsAny<ICell>(), It.IsAny<RoomLayer>(),
			It.IsAny<ICellExit>()), Times.Never);
	}

	[TestMethod]
	public void AutomaticCellExitReadiness_WithoutDriver_UsesExitSpecificGraphValidation()
	{
		var harness = CreateAutomaticExitHarness(null);
		harness.Graph.Setup(x => x.CanMoveVehicleTrain(
			It.IsAny<IFuturemud>(),
			harness.Root.Object,
			harness.Exit.Object,
			out It.Ref<VehicleHitchGraphMovePlan>.IsAny,
			out It.Ref<string>.IsAny))
			.Returns((IFuturemud _, IVehicle _, ICellExit _, out VehicleHitchGraphMovePlan plan,
				out string reason) =>
			{
				plan = harness.MovePlan;
				reason = string.Empty;
				return true;
			});
		var service = new VehicleOperationalReadinessService(harness.Graph.Object);

		var result = service.BuildMovementReadiness(new VehicleMovementReadinessRequest(
			harness.Root.Object,
			null,
			harness.Exit.Object,
			harness.Profile.Object,
			AutomaticOperation: true));

		Assert.IsTrue(result.CanMove, result.Reason);
		Assert.AreSame(harness.MovePlan, result.MovePlan);
		Assert.IsNotNull(result.ResourcePlan);
		harness.Graph.Verify(x => x.CanMoveVehicleTrain(It.IsAny<IFuturemud>(), harness.Root.Object,
			harness.Exit.Object, out It.Ref<VehicleHitchGraphMovePlan>.IsAny, out It.Ref<string>.IsAny), Times.Once);
	}

	[TestMethod]
	public void AutomaticCellExitReadiness_GraphEnvironmentFailure_PreservesExactReason()
	{
		const string expected = "The trailer can only move to another surface-water location.";
		var harness = CreateAutomaticExitHarness(null);
		harness.Graph.Setup(x => x.CanMoveVehicleTrain(
			It.IsAny<IFuturemud>(),
			harness.Root.Object,
			harness.Exit.Object,
			out It.Ref<VehicleHitchGraphMovePlan>.IsAny,
			out It.Ref<string>.IsAny))
			.Returns((IFuturemud _, IVehicle _, ICellExit _, out VehicleHitchGraphMovePlan plan,
				out string reason) =>
			{
				plan = harness.MovePlan;
				reason = expected;
				return false;
			});
		var service = new VehicleOperationalReadinessService(harness.Graph.Object);

		var result = service.BuildMovementReadiness(new VehicleMovementReadinessRequest(
			harness.Root.Object,
			null,
			harness.Exit.Object,
			harness.Profile.Object,
			AutomaticOperation: true));

		Assert.IsFalse(result.CanMove);
		Assert.AreEqual(expected, result.Reason);
	}

	[TestMethod]
	public void ManualRoute_AfterMidLegStop_ResumesPinnedLinearContinuation()
	{
		var clock = new ManualTimeProvider(new DateTimeOffset(2026, 7, 22, 0, 0, 0, TimeSpan.Zero));
		var schedules = new List<(Action Action, TimeSpan Delay)>();
		var harness = CreatePoweredHarness(clock, (action, delay) => schedules.Add((action, delay)));

		Assert.IsTrue(harness.Strategy.TryBeginManualMove(
			harness.Vehicle.Object,
			harness.Actor.Object,
			100.0,
			out var beginReason), beginReason);
		clock.Advance(TimeSpan.FromSeconds(2.0));
		Assert.IsTrue(harness.Strategy.TryStop(harness.Vehicle.Object, null, out var stopReason), stopReason);
		Assert.AreEqual(20.0, harness.Position, 0.000001);
		var route = CreateLinearRoute(harness.Vehicle.Object.Location, (101L, 0.0, 100.0));
		VehicleJourneyLegResult? completion = null;

		var resumed = harness.Strategy.TryBeginManualRoute(
			harness.Vehicle.Object,
			harness.Actor.Object,
			route.Object,
			result => completion = result,
			out var resumeReason);

		Assert.IsTrue(resumed, resumeReason);
		Assert.AreEqual(2, harness.Persistence.StartCount,
			"Resuming a stopped pinned leg must create a fresh durable movement operation.");
		Assert.IsTrue(harness.Strategy.IsMoving(harness.Vehicle.Object));
		Assert.IsNull(completion);
		Assert.IsTrue(harness.Strategy.TryStop(harness.Vehicle.Object, null, out var cleanupReason), cleanupReason);
	}

	[TestMethod]
	public void ManualRoute_MidLegResume_FailsClosedWhenContinuationIsAmbiguousOrOffLeg()
	{
		var clock = new ManualTimeProvider();
		var harness = CreatePoweredHarness(clock, (_, _) => { });
		harness.Vehicle.Object.MaterialiseRoutePosition(20.0, true);
		var ambiguous = CreateLinearRoute(
			harness.Vehicle.Object.Location,
			(201L, 0.0, 100.0),
			(202L, 0.0, 80.0));

		var ambiguousResult = harness.Strategy.TryBeginManualRoute(
			harness.Vehicle.Object,
			harness.Actor.Object,
			ambiguous.Object,
			_ => Assert.Fail("An ambiguous route must not start."),
			out var ambiguousReason);

		Assert.IsFalse(ambiguousResult);
		Assert.AreEqual("The vehicle's current position matches more than one pinned route continuation.",
			ambiguousReason);

		var offLeg = CreateLinearRoute(harness.Vehicle.Object.Location, (301L, 100.0, 200.0));
		var offLegResult = harness.Strategy.TryBeginManualRoute(
			harness.Vehicle.Object,
			harness.Actor.Object,
			offLeg.Object,
			_ => Assert.Fail("An off-leg route must not start."),
			out var offLegReason);

		Assert.IsFalse(offLegResult);
		Assert.AreEqual("The vehicle is not at a stop or on a pinned leg of that route.", offLegReason);
	}

	private static AutomaticExitHarness CreateAutomaticExitHarness(
		double? destinationRoutePosition,
		string? readinessFailure = null,
		bool doorClosed = false,
		string? catastropheReason = null)
	{
		var gameworld = CreateGameworld();
		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Id).Returns(70L);
		origin.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		Mock<ICell> destination;
		if (destinationRoutePosition.HasValue)
		{
			var terrain = new Mock<ITerrain>();
			terrain.SetupGet(x => x.StaminaCost).Returns(1.0);
			destination = CreateRouteCell(71L, 10_000.0, [], terrain.Object);
		}
		else
		{
			destination = new Mock<ICell>();
			destination.SetupGet(x => x.Id).Returns(71L);
			destination.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		}

		var exitModel = new Mock<IExit>();
		exitModel.SetupGet(x => x.Id).Returns(950L);
		exitModel.SetupGet(x => x.MaximumSizeToEnter).Returns(SizeCategory.Enormous);
		if (doorClosed)
		{
			var door = new Mock<IDoor>();
			door.SetupGet(x => x.IsOpen).Returns(false);
			exitModel.SetupGet(x => x.Door).Returns(door.Object);
		}

		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Exit).Returns(exitModel.Object);
		exit.SetupGet(x => x.Origin).Returns(origin.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.SetupGet(x => x.OutboundMovementSuffix).Returns("east");
		exit.SetupGet(x => x.InboundMovementSuffix).Returns("from the west");
		exit.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>()))
			.Returns((CellMovementTransition.GroundToGround, RoomLayer.GroundLevel));
		origin.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>())).Returns([exit.Object]);

		var profile = CreateRouteProfile(RouteVehiclePropulsionMode.Powered, 20.0);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		prototype.SetupGet(x => x.OccupantSlots).Returns([]);
		prototype.SetupGet(x => x.ControlStations).Returns([]);
		var rootExterior = new Mock<IGameItem>();
		rootExterior.SetupGet(x => x.Id).Returns(701L);
		rootExterior.SetupGet(x => x.Name).Returns("train exterior");
		rootExterior.SetupGet(x => x.Location).Returns(origin.Object);
		rootExterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		rootExterior.SetupGet(x => x.Size).Returns(SizeCategory.Large);
		rootExterior.SetupGet(x => x.Deleted).Returns(false);
		rootExterior.SetupGet(x => x.Destroyed).Returns(false);
		rootExterior.Setup(x => x.PreventsMovement()).Returns(false);
		rootExterior.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(origin.Object, RoomLayer.GroundLevel, null));
		var trailerExterior = new Mock<IGameItem>();
		trailerExterior.SetupGet(x => x.Id).Returns(702L);
		trailerExterior.SetupGet(x => x.Name).Returns("carriage exterior");
		trailerExterior.SetupGet(x => x.Location).Returns(origin.Object);
		trailerExterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		trailerExterior.SetupGet(x => x.Size).Returns(SizeCategory.Large);
		trailerExterior.SetupGet(x => x.Deleted).Returns(false);
		trailerExterior.SetupGet(x => x.Destroyed).Returns(false);
		trailerExterior.Setup(x => x.PreventsMovement()).Returns(false);
		trailerExterior.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(origin.Object, RoomLayer.GroundLevel, null));

		var root = new Mock<IVehicle>();
		root.SetupGet(x => x.Id).Returns(700L);
		root.SetupGet(x => x.Name).Returns("automatic train");
		root.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		root.SetupGet(x => x.Prototype).Returns(prototype.Object);
		root.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		root.SetupGet(x => x.Location).Returns(origin.Object);
		root.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		root.SetupGet(x => x.RoutePositionMetres).Returns((double?)null);
		root.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(origin.Object, RoomLayer.GroundLevel, null));
		root.SetupGet(x => x.ExteriorItem).Returns(rootExterior.Object);
		root.SetupGet(x => x.Compartments).Returns([]);
		root.SetupGet(x => x.Occupancies).Returns([]);
		root.SetupGet(x => x.AccessPoints).Returns([]);
		root.SetupGet(x => x.Installations).Returns([]);
		root.SetupGet(x => x.Destroyed).Returns(false);
		root.SetupGet(x => x.Disabled).Returns(false);
		root.Setup(x => x.IsDisabledByDamage(It.IsAny<VehicleDamageEffectTargetType>(), It.IsAny<long?>()))
			.Returns(false);
		var trailer = new Mock<IVehicle>();
		trailer.SetupGet(x => x.Id).Returns(701L);
		trailer.SetupGet(x => x.Name).Returns("automatic carriage");
		trailer.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		trailer.SetupGet(x => x.Prototype).Returns(prototype.Object);
		trailer.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		trailer.SetupGet(x => x.Location).Returns(origin.Object);
		trailer.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		trailer.SetupGet(x => x.RoutePositionMetres).Returns((double?)null);
		trailer.SetupGet(x => x.SpatialLocation)
			.Returns(new SpatialLocation(origin.Object, RoomLayer.GroundLevel, null));
		trailer.SetupGet(x => x.ExteriorItem).Returns(trailerExterior.Object);
		trailer.SetupGet(x => x.Compartments).Returns([]);
		trailer.SetupGet(x => x.Destroyed).Returns(false);

		var movePlan = new VehicleHitchGraphMovePlan(
			root.Object,
			[
				new VehicleHitchGraphTrainMember(root.Object, 0, null),
				new VehicleHitchGraphTrainMember(trailer.Object, 1, null)
			],
			[],
			[],
			0.0);
		var resourcePlan = new VehicleResourceReadinessPlan(root.Object, profile.Object, [], [], [], true, true,
			string.Empty);
		var readiness = new Mock<IVehicleOperationalReadinessService>();
		readiness.Setup(x => x.BuildMovementReadiness(It.IsAny<VehicleMovementReadinessRequest>()))
			.Returns(readinessFailure is null
				? new VehicleMovementReadinessResult(true, string.Empty, movePlan, resourcePlan, [])
				: new VehicleMovementReadinessResult(false, readinessFailure, null, null, []));
		readiness.Setup(x => x.RollTowCatastrophe(movePlan, null))
			.Returns(string.IsNullOrWhiteSpace(catastropheReason)
				? new VehicleTowCatastropheResult(false, null, string.Empty, [], [])
				: new VehicleTowCatastropheResult(true, null, catastropheReason, [], []));
		var graph = new Mock<IVehicleHitchGraphService>();
		var cellExitStrategy = new CellExitVehicleMovementStrategy(
			new Mock<IVehicleTowService>().Object,
			graph.Object,
			readiness.Object);
		var strategy = new RouteVehicleMovementStrategy(
			new RouteSpatialService(RouteSpatialConfiguration.Default, new ManualTimeProvider()),
			readiness.Object,
			graph.Object,
			new FakeVehicleRouteMotionPersistence(),
			cellExitStrategy,
			schedule: (_, _, _) => { });

		var route = new Mock<IVehicleRoute>();
		var originStop = new Mock<IVehicleRouteStop>();
		var destinationStop = new Mock<IVehicleRouteStop>();
		var leg = new Mock<IVehicleRouteLeg>();
		var step = new Mock<IVehicleRouteExitStep>();
		var originLocation = new SpatialLocation(origin.Object, RoomLayer.GroundLevel, null);
		var destinationLocation = new SpatialLocation(destination.Object, RoomLayer.GroundLevel,
			destinationRoutePosition);
		originStop.SetupGet(x => x.Id).Returns(710L);
		originStop.SetupGet(x => x.Route).Returns(route.Object);
		originStop.SetupGet(x => x.Sequence).Returns(0);
		originStop.SetupGet(x => x.Location).Returns(originLocation);
		destinationStop.SetupGet(x => x.Id).Returns(711L);
		destinationStop.SetupGet(x => x.Route).Returns(route.Object);
		destinationStop.SetupGet(x => x.Sequence).Returns(1);
		destinationStop.SetupGet(x => x.Location).Returns(destinationLocation);
		leg.SetupGet(x => x.Id).Returns(720L);
		leg.SetupGet(x => x.Route).Returns(route.Object);
		leg.SetupGet(x => x.Sequence).Returns(0);
		leg.SetupGet(x => x.OriginStop).Returns(originStop.Object);
		leg.SetupGet(x => x.DestinationStop).Returns(destinationStop.Object);
		leg.SetupGet(x => x.Steps).Returns([step.Object]);
		step.SetupGet(x => x.Id).Returns(721L);
		step.SetupGet(x => x.Leg).Returns(leg.Object);
		step.SetupGet(x => x.Sequence).Returns(0);
		step.SetupGet(x => x.StepType).Returns(VehicleRouteStepType.CellExit);
		step.SetupGet(x => x.Origin).Returns(originLocation);
		step.SetupGet(x => x.Destination).Returns(destinationLocation);
		step.SetupGet(x => x.Exit).Returns(exit.Object);
		route.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		route.SetupGet(x => x.Stops).Returns([originStop.Object, destinationStop.Object]);
		route.SetupGet(x => x.Legs).Returns([leg.Object]);
		route.SetupGet(x => x.TopologyPins).Returns([]);
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.OperatorMode).Returns(VehicleServiceOperatorMode.Automatic);
		service.SetupGet(x => x.Route).Returns(route.Object);
		service.SetupGet(x => x.Vehicle).Returns(root.Object);
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.Id).Returns(730L);
		journey.SetupGet(x => x.Service).Returns(service.Object);
		journey.SetupGet(x => x.Route).Returns(route.Object);
		journey.SetupGet(x => x.Vehicle).Returns(root.Object);
		journey.SetupGet(x => x.State).Returns(VehicleJourneyState.Boarding);

		return new AutomaticExitHarness(strategy, root, trailer, destination, exit, route, leg, journey,
			profile, readiness, graph, movePlan, resourcePlan);
	}

	private static ExternalExitHarness CreateExternallyPulledExitHarness(string? readinessFailure = null)
	{
		var gameworld = CreateGameworld();
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.StaminaCost).Returns(1.0);
		var perceivables = new List<IPerceivable>();
		var origin = CreateRouteCell(50L, 10_000.0, perceivables, terrain.Object);
		var destination = CreateRouteCell(51L, 10_000.0, [], terrain.Object);
		var exitModel = new Mock<IExit>();
		exitModel.SetupGet(x => x.Id).Returns(900L);
		exitModel.SetupGet(x => x.MaximumSizeToEnter).Returns(SizeCategory.Enormous);
		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Exit).Returns(exitModel.Object);
		exit.SetupGet(x => x.Origin).Returns(origin.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.Setup(x => x.MovementTransition(It.IsAny<IPerceiver>()))
			.Returns((CellMovementTransition.GroundToGround, RoomLayer.GroundLevel));
		origin.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>())).Returns([exit.Object]);

		var driver = new Mock<ICharacter>();
		driver.SetupGet(x => x.Id).Returns(500L);
		driver.SetupGet(x => x.Name).Returns("wagon driver");
		driver.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var pullerPosition = 0.0;
		var puller = CreatePuller(501L, origin.Object, gameworld.Object, () => pullerPosition,
			value => pullerPosition = value, new Queue<bool>([true]));
		puller.Setup(x => x.CanMove(exit.Object, It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);

		var profile = CreateRouteProfile(RouteVehiclePropulsionMode.ExternallyPulled, 0.0);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		var rootPosition = 0.0;
		var rootExteriorPosition = 0.0;
		var rootExterior = CreateExterior(601L, origin.Object, () => rootExteriorPosition,
			value => rootExteriorPosition = value);
		rootExterior.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
		var root = CreateVehicle(601L, gameworld.Object, origin.Object, rootExterior.Object, prototype.Object,
			() => rootPosition, value =>
			{
				rootPosition = value;
				rootExteriorPosition = value;
			}, driver.Object);
		root.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		root.SetupGet(x => x.Occupants).Returns([driver.Object]);

		var trailerPosition = 0.0;
		var trailerExteriorPosition = 0.0;
		var trailerExterior = CreateExterior(602L, origin.Object, () => trailerExteriorPosition,
			value => trailerExteriorPosition = value);
		trailerExterior.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
		var trailer = CreateVehicle(602L, gameworld.Object, origin.Object, trailerExterior.Object, prototype.Object,
			() => trailerPosition, value =>
			{
				trailerPosition = value;
				trailerExteriorPosition = value;
			}, driver.Object);
		trailer.SetupGet(x => x.MovementProfile).Returns(profile.Object);

		var horseHarness = CreateExterior(603L, origin.Object, () => pullerPosition, _ => { });
		var wagonHarness = CreateExterior(604L, origin.Object, () => rootPosition, _ => { });
		perceivables.Add(puller.Object);
		perceivables.Add(rootExterior.Object);
		perceivables.Add(trailerExterior.Object);
		perceivables.Add(horseHarness.Object);
		perceivables.Add(wagonHarness.Object);

		var motiveLink = new VehicleHitchGraphLink(
			"motive",
			VehicleHitchGraphLinkKind.TransientCharacterHitch,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Character, null, puller.Object, null),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, root.Object, null, null),
			horseHarness.Object,
			horseHarness.Object.Id,
			false,
			false,
			string.Empty,
			null);
		var wagonLink = new VehicleHitchGraphLink(
			"wagon",
			VehicleHitchGraphLinkKind.PersistentHitch,
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, root.Object, null, null),
			new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, trailer.Object, null, null),
			wagonHarness.Object,
			wagonHarness.Object.Id,
			false,
			false,
			string.Empty,
			null);
		var movePlan = new VehicleHitchGraphMovePlan(
			root.Object,
			[
				new VehicleHitchGraphTrainMember(root.Object, 0, motiveLink),
				new VehicleHitchGraphTrainMember(trailer.Object, 1, wagonLink)
			],
			[motiveLink, wagonLink],
			[horseHarness.Object, wagonHarness.Object],
			0.0);
		var graph = new Mock<IVehicleHitchGraphService>();
		graph.Setup(x => x.LinksInvolving(gameworld.Object, rootExterior.Object)).Returns([motiveLink]);
		var readiness = new Mock<IVehicleOperationalReadinessService>();
		readiness.Setup(x => x.BuildMovementReadiness(It.IsAny<VehicleMovementReadinessRequest>()))
			.Returns(readinessFailure is null
				? new VehicleMovementReadinessResult(true, string.Empty, movePlan, null, [])
				: new VehicleMovementReadinessResult(false, readinessFailure, null, null, []));
		readiness.Setup(x => x.RollTowCatastrophe(movePlan, driver.Object))
			.Returns(new VehicleTowCatastropheResult(false, null, string.Empty, [], []));
		var cellExitStrategy = new CellExitVehicleMovementStrategy(
			new Mock<IVehicleTowService>().Object,
			graph.Object,
			readiness.Object);
		var strategy = new RouteVehicleMovementStrategy(
			new RouteSpatialService(RouteSpatialConfiguration.Default, new ManualTimeProvider()),
			readiness.Object,
			graph.Object,
			new FakeVehicleRouteMotionPersistence(),
			cellExitStrategy,
			schedule: (_, _, _) => { });
		var route = CreateExitRoute(origin.Object, 0.0, destination.Object, 7_150.0, exit.Object);
		return new ExternalExitHarness(strategy, root, trailer, driver, puller, origin, destination, exit,
			route, readiness, graph, movePlan, [horseHarness, wagonHarness]);
	}

	private static Mock<IVehicleRoute> CreateExitRoute(
		ICell origin,
		double originMetres,
		ICell destination,
		double destinationMetres,
		ICellExit exit)
	{
		var route = new Mock<IVehicleRoute>();
		var originStop = new Mock<IVehicleRouteStop>();
		var destinationStop = new Mock<IVehicleRouteStop>();
		var leg = new Mock<IVehicleRouteLeg>();
		var step = new Mock<IVehicleRouteExitStep>();
		var originLocation = new SpatialLocation(origin, RoomLayer.GroundLevel, originMetres);
		var destinationLocation = new SpatialLocation(destination, RoomLayer.GroundLevel, destinationMetres);
		originStop.SetupGet(x => x.Id).Returns(1L);
		originStop.SetupGet(x => x.Route).Returns(route.Object);
		originStop.SetupGet(x => x.Sequence).Returns(0);
		originStop.SetupGet(x => x.Location).Returns(originLocation);
		destinationStop.SetupGet(x => x.Id).Returns(2L);
		destinationStop.SetupGet(x => x.Route).Returns(route.Object);
		destinationStop.SetupGet(x => x.Sequence).Returns(1);
		destinationStop.SetupGet(x => x.Location).Returns(destinationLocation);
		leg.SetupGet(x => x.Id).Returns(10L);
		leg.SetupGet(x => x.Route).Returns(route.Object);
		leg.SetupGet(x => x.Sequence).Returns(0);
		leg.SetupGet(x => x.OriginStop).Returns(originStop.Object);
		leg.SetupGet(x => x.DestinationStop).Returns(destinationStop.Object);
		leg.SetupGet(x => x.Steps).Returns([step.Object]);
		step.SetupGet(x => x.Id).Returns(11L);
		step.SetupGet(x => x.Leg).Returns(leg.Object);
		step.SetupGet(x => x.Sequence).Returns(0);
		step.SetupGet(x => x.StepType).Returns(VehicleRouteStepType.CellExit);
		step.SetupGet(x => x.Origin).Returns(originLocation);
		step.SetupGet(x => x.Destination).Returns(destinationLocation);
		step.SetupGet(x => x.Exit).Returns(exit);
		route.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		route.SetupGet(x => x.Stops).Returns([originStop.Object, destinationStop.Object]);
		route.SetupGet(x => x.Legs).Returns([leg.Object]);
		return route;
	}

	private static Mock<IVehicleRoute> CreateLinearRoute(
		ICell cell,
		params (long LegId, double Origin, double Destination)[] segments)
	{
		var route = new Mock<IVehicleRoute>();
		var stops = new List<IVehicleRouteStop>();
		var legs = new List<IVehicleRouteLeg>();
		foreach (var segment in segments)
		{
			var originStop = new Mock<IVehicleRouteStop>();
			var destinationStop = new Mock<IVehicleRouteStop>();
			var leg = new Mock<IVehicleRouteLeg>();
			var step = new Mock<IVehicleRouteLinearStep>();
			var origin = new SpatialLocation(cell, RoomLayer.GroundLevel, segment.Origin);
			var destination = new SpatialLocation(cell, RoomLayer.GroundLevel, segment.Destination);
			originStop.SetupGet(x => x.Id).Returns(segment.LegId * 10 + 1);
			originStop.SetupGet(x => x.Route).Returns(route.Object);
			originStop.SetupGet(x => x.Location).Returns(origin);
			destinationStop.SetupGet(x => x.Id).Returns(segment.LegId * 10 + 2);
			destinationStop.SetupGet(x => x.Route).Returns(route.Object);
			destinationStop.SetupGet(x => x.Location).Returns(destination);
			leg.SetupGet(x => x.Id).Returns(segment.LegId);
			leg.SetupGet(x => x.Route).Returns(route.Object);
			leg.SetupGet(x => x.Sequence).Returns(legs.Count);
			leg.SetupGet(x => x.OriginStop).Returns(originStop.Object);
			leg.SetupGet(x => x.DestinationStop).Returns(destinationStop.Object);
			leg.SetupGet(x => x.Steps).Returns([step.Object]);
			step.SetupGet(x => x.Id).Returns(segment.LegId * 10 + 3);
			step.SetupGet(x => x.Leg).Returns(leg.Object);
			step.SetupGet(x => x.Sequence).Returns(0);
			step.SetupGet(x => x.StepType).Returns(VehicleRouteStepType.LinearRoute);
			step.SetupGet(x => x.Origin).Returns(origin);
			step.SetupGet(x => x.Destination).Returns(destination);
			step.SetupGet(x => x.RouteCell).Returns(cell.RouteDefinition!);
			step.SetupGet(x => x.Direction).Returns(segment.Destination >= segment.Origin
				? RouteCellDirection.Positive
				: RouteCellDirection.Negative);
			step.SetupGet(x => x.DistanceMetres).Returns(Math.Abs(segment.Destination - segment.Origin));
			step.SetupGet(x => x.PinnedTopologyVersion).Returns(cell.RouteDefinition!.TopologyVersion);
			stops.Add(originStop.Object);
			stops.Add(destinationStop.Object);
			legs.Add(leg.Object);
		}

		route.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		route.SetupGet(x => x.Stops).Returns(stops);
		route.SetupGet(x => x.Legs).Returns(legs);
		return route;
	}

	private static PoweredHarness CreatePoweredHarness(
		TimeProvider timeProvider,
		Action<Action, TimeSpan> schedule,
		bool trackingEnabled = false,
		bool throwAfterPositiveResourceApply = false)
	{
		var gameworld = CreateGameworld(trackingEnabled);
		var tracks = new List<ITrack>();
		gameworld.Setup(x => x.Add(It.IsAny<ITrack>()))
			.Callback((ITrack track) => tracks.Add(track));
		var terrain = new Mock<ITerrain>();
		terrain.SetupGet(x => x.StaminaCost).Returns(1.0);
		terrain.SetupGet(x => x.CanHaveTracks).Returns(trackingEnabled);
		var occupants = new List<IPerceivable>();
		var cell = CreateRouteCell(1L, 1_000.0, occupants, terrain.Object);
		var exteriorPosition = 0.0;
		var exterior = CreateExterior(101L, cell.Object, () => exteriorPosition,
			value => exteriorPosition = value);
		var events = new List<EventType>();
		exterior.Setup(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()))
			.Callback<EventType, object[]>((eventType, _) => events.Add(eventType))
			.Returns(false);
		occupants.Add(exterior.Object);
		var profile = CreateRouteProfile(RouteVehiclePropulsionMode.Powered, 10.0);
		profile.SetupGet(x => x.RoutePowerDrawWatts).Returns(10.0);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(10L);
		actor.SetupGet(x => x.Name).Returns("driver");
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var vehiclePosition = 0.0;
		var vehicle = CreateVehicle(1L, gameworld.Object, cell.Object, exterior.Object, prototype.Object,
			() => vehiclePosition, value =>
			{
				vehiclePosition = value;
				exteriorPosition = value;
			}, actor.Object);
		var movePlan = CreateMovePlan(vehicle.Object);
		var readiness = CreateReadiness(
			vehicle.Object,
			profile.Object,
			movePlan,
			out var powerConsumed,
			throwAfterPositiveResourceApply);
		var persistence = new FakeVehicleRouteMotionPersistence();
		var spatial = new RouteSpatialService(RouteSpatialConfiguration.Default, timeProvider);
		var strategy = new RouteVehicleMovementStrategy(
			spatial,
			readiness.Object,
			new Mock<IVehicleHitchGraphService>().Object,
			persistence,
			schedule: (_, action, delay) => schedule(action, delay));
		return new PoweredHarness(strategy, vehicle, actor, persistence,
			() => vehiclePosition, () => exteriorPosition, powerConsumed, tracks, events);
	}

	private static Mock<IVehicleOperationalReadinessService> CreateReadiness(
		IVehicle vehicle,
		IVehicleMovementProfilePrototype profile,
		VehicleHitchGraphMovePlan movePlan,
		out Func<double> powerConsumed,
		bool throwAfterPositiveResourceApply = false)
	{
		var consumed = 0.0;
		powerConsumed = () => consumed;
		var readiness = new Mock<IVehicleOperationalReadinessService>();
		var emptyPlan = new VehicleResourceReadinessPlan(vehicle, profile, [], [], [], true, true, string.Empty);
		readiness.Setup(x => x.BuildLongitudinalMovementReadiness(It.IsAny<VehicleLongitudinalReadinessRequest>()))
			.Returns(new VehicleMovementReadinessResult(true, string.Empty, movePlan, emptyPlan, []));
		readiness.Setup(x => x.BuildLongitudinalResourcePlan(
			vehicle,
			profile,
			It.IsAny<double>(),
			It.IsAny<TimeSpan>()))
			.Returns((IVehicle _, IVehicleMovementProfilePrototype _, double _, TimeSpan duration) =>
			{
				if (profile.RoutePowerDrawWatts <= 0.0 || duration <= TimeSpan.Zero)
				{
					return emptyPlan;
				}

				var installation = new Mock<IVehicleInstallation>();
				installation.SetupGet(x => x.Id).Returns(300L);
				var use = new VehicleResourceUse(
					VehicleResourceUseType.Power,
					installation.Object,
					null,
					null,
					null,
					0.0,
					null,
					duration.TotalSeconds * profile.RoutePowerDrawWatts,
					string.Empty);
				return new VehicleResourceReadinessPlan(vehicle, profile, [use], [], [], true, true,
					string.Empty);
			});
		readiness.Setup(x => x.ConsumeMovementResources(It.IsAny<VehicleResourceReadinessPlan>()))
			.Callback((VehicleResourceReadinessPlan plan) =>
			{
				var amount = plan.Uses.Sum(x => x.PowerSpikeInWatts);
				consumed += amount;
				if (throwAfterPositiveResourceApply && amount > 0.0)
				{
					throw new InvalidOperationException("Injected resource apply failure after mutation.");
				}
			});
		readiness.Setup(x => x.RollTowCatastrophe(movePlan, It.IsAny<ICharacter>()))
			.Returns(new VehicleTowCatastropheResult(false, null, string.Empty, [], []));
		return readiness;
	}

	private static Mock<IVehicleMovementProfilePrototype> CreateRouteProfile(
		RouteVehiclePropulsionMode propulsion,
		double speed)
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(1L);
		profile.SetupGet(x => x.Name).Returns("route movement");
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.Route);
		profile.SetupGet(x => x.IsDefault).Returns(true);
		profile.SetupGet(x => x.RoutePropulsionMode).Returns(propulsion);
		profile.SetupGet(x => x.RouteSpeedMetresPerSecond).Returns(speed);
		profile.SetupGet(x => x.AutomaticOperationCapable).Returns(propulsion == RouteVehiclePropulsionMode.Powered);
		return profile;
	}

	private static VehicleHitchGraphMovePlan CreateMovePlan(IVehicle vehicle)
	{
		return new VehicleHitchGraphMovePlan(
			vehicle,
			[new VehicleHitchGraphTrainMember(vehicle, 0, null)],
			[],
			[],
			0.0);
	}

	private static Mock<IVehicle> CreateVehicle(
		long id,
		IFuturemud gameworld,
		ICell cell,
		IGameItem exterior,
		IVehiclePrototype prototype,
		Func<double> position,
		Action<double> setPosition,
		ICharacter controller)
	{
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(id);
		vehicle.SetupGet(x => x.Name).Returns("route vehicle");
		vehicle.SetupGet(x => x.Gameworld).Returns(gameworld);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype);
		vehicle.SetupGet(x => x.Location).Returns(cell);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.RoutePositionMetres).Returns(() => position());
		vehicle.SetupGet(x => x.SpatialLocation)
			.Returns(() => new SpatialLocation(cell, RoomLayer.GroundLevel, position()));
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior);
		vehicle.SetupGet(x => x.Controller).Returns(controller);
		vehicle.SetupGet(x => x.Occupants).Returns([]);
		vehicle.SetupGet(x => x.Installations).Returns([]);
		vehicle.SetupGet(x => x.AccessPoints).Returns([]);
		vehicle.SetupGet(x => x.TowLinks).Returns([]);
		vehicle.SetupGet(x => x.Destroyed).Returns(false);
		vehicle.SetupGet(x => x.Disabled).Returns(false);
		vehicle.Setup(x => x.BeginMoveAlongRoute(It.IsAny<double>()));
		vehicle.Setup(x => x.MaterialiseRoutePosition(It.IsAny<double>(), It.IsAny<bool>()))
			.Callback((double value, bool _) => setPosition(value));
		return vehicle;
	}

	private static Mock<IGameItem> CreateExterior(
		long id,
		ICell cell,
		Func<double> position,
		Action<double> setPosition)
	{
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Id).Returns(id);
		exterior.SetupGet(x => x.Name).Returns("vehicle exterior");
		exterior.SetupGet(x => x.Location).Returns(cell);
		exterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		exterior.SetupGet(x => x.RoutePositionMetres).Returns(() => position());
		exterior.SetupGet(x => x.SpatialLocation)
			.Returns(() => new SpatialLocation(cell, RoomLayer.GroundLevel, position()));
		exterior.Setup(x => x.SetRoutePosition(It.IsAny<double?>()))
			.Callback((double? value) => setPosition(value!.Value));
		return exterior;
	}

	private static Mock<ICharacter> CreatePuller(
		long id,
		ICell cell,
		IFuturemud gameworld,
		Func<double> position,
		Action<double> setPosition,
		Queue<bool> canPay)
	{
		var puller = new Mock<ICharacter>();
		IMovement? movement = null;
		var speed = new Mock<IMoveSpeed>();
		speed.SetupGet(x => x.Multiplier).Returns(1.0);
		speed.SetupGet(x => x.StaminaMultiplier).Returns(1.0);
		var positionState = new Mock<IPositionState>();
		var body = new Mock<IBody>();
		var stamina = 1_000.0;
		body.SetupGet(x => x.CurrentStamina).Returns(() => stamina);
		body.SetupSet(x => x.CurrentStamina = It.IsAny<double>())
			.Callback((double value) => stamina = value);
		positionState.SetupGet(x => x.IgnoreTerrainStaminaCostsForMovement).Returns(false);
		puller.SetupGet(x => x.Id).Returns(id);
		puller.SetupGet(x => x.Name).Returns("draught horse");
		puller.Setup(x => x.SamePhysicalInstance(It.IsAny<ICharacter>()))
			.Returns((ICharacter other) => ReferenceEquals(other, puller.Object));
		puller.SetupGet(x => x.Gameworld).Returns(gameworld);
		puller.SetupGet(x => x.Body).Returns(body.Object);
		puller.SetupGet(x => x.Location).Returns(cell);
		puller.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		puller.SetupGet(x => x.RoutePositionMetres).Returns(() => position());
		puller.SetupGet(x => x.SpatialLocation)
			.Returns(() => new SpatialLocation(cell, RoomLayer.GroundLevel, position()));
		puller.Setup(x => x.SetRoutePosition(It.IsAny<double?>()))
			.Callback((double? value) => setPosition(value!.Value));
		puller.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		puller.SetupGet(x => x.CurrentSpeed).Returns(speed.Object);
		puller.SetupGet(x => x.PositionState).Returns(positionState.Object);
		puller.SetupGet(x => x.EncumbrancePercentage).Returns(0.0);
		puller.SetupGet(x => x.Party).Returns((IParty)null!);
		puller.SetupGet(x => x.RidingMount).Returns((ICharacter?)null);
		puller.SetupGet(x => x.Riders).Returns([]);
		puller.SetupGet(x => x.Following).Returns((IMove)null!);
		puller.SetupGet(x => x.Movement).Returns(() => movement!);
		puller.SetupSet(x => x.Movement = It.IsAny<IMovement>())
			.Callback((IMovement value) => movement = value);
		puller.Setup(x => x.CanMove(It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
		puller.Setup(x => x.CanSpendStamina(It.IsAny<double>()))
			.Returns(() => canPay.Count > 0 && canPay.Dequeue());
		puller.Setup(x => x.EffectsOfType<Dragging>(It.IsAny<Predicate<Dragging>>()))
			.Returns([]);
		puller.Setup(x => x.EffectsOfType<CharacterHitch>(It.IsAny<Predicate<CharacterHitch>>()))
			.Returns([]);
		return puller;
	}

	private static Mock<IFuturemud> CreateGameworld(bool trackingEnabled = false)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<MudSharp.Framework.Save.ISaveManager>().Object);
		gameworld.Setup(x => x.GetStaticBool("TrackingEnabled")).Returns(trackingEnabled);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellMovementCheckpointSeconds")).Returns(30.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellImmediateDistanceMetres")).Returns(3.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellProximateDistanceMetres")).Returns(10.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellDistantDistanceMetres")).Returns(100.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellVeryDistantDistanceMetres")).Returns(500.0);
		gameworld.Setup(x => x.GetStaticDouble("RouteCellDefaultRoomEquivalentMetres")).Returns(100.0);
		gameworld.Setup(x => x.GetStaticDouble("DefaultTerrainStaminaCost")).Returns(1.0);
		gameworld.Setup(x => x.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage")).Returns(1.0);
		return gameworld;
	}

	private static VehicleExteriorGameItemComponentProto CreateVehicleExteriorComponentProto(IFuturemud gameworld)
	{
		var dbPrototype = new MudSharp.Models.GameItemComponentProto
		{
			Id = 1_155L,
			Name = "QA Vehicle Exterior",
			Description = "QA vehicle exterior component",
			Type = "Vehicle Exterior",
			Definition = "<Definition></Definition>",
			RevisionNumber = 0,
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 0,
				BuilderAccountId = 1L,
				BuilderDate = DateTime.UtcNow
			}
		};

		return (VehicleExteriorGameItemComponentProto)Activator.CreateInstance(
			typeof(VehicleExteriorGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[dbPrototype, gameworld],
			null)!;
	}

	private static Mock<IGameItemComponentProto> CreateComponentPrototype(IGameItemComponent component)
	{
		var prototype = new Mock<IGameItemComponentProto>();
		prototype
			.Setup(x => x.CreateNew(It.IsAny<IGameItem>(), It.IsAny<ICharacter?>(), It.IsAny<bool>()))
			.Returns(component);
		return prototype;
	}

	private static GameItem CreateRouteGameItem(
		IFuturemud gameworld,
		ICell cell,
		long id,
		string name,
		IReadOnlyCollection<IGameItemComponentProto> components)
	{
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Id).Returns(id);
		prototype.SetupGet(x => x.Name).Returns(name);
		prototype.SetupGet(x => x.Gameworld).Returns(gameworld);
		prototype.SetupGet(x => x.Components).Returns(components);
		prototype.SetupGet(x => x.Morphs).Returns(false);
		prototype.SetupGet(x => x.Keywords).Returns(name.Split(' '));
		var item = new GameItem(prototype.Object)
		{
			Id = id
		};
		item.MoveTo(cell, RoomLayer.GroundLevel);
		return item;
	}

	private static Mock<ICell> CreateRouteCell(
		long id,
		double length,
		IEnumerable<IPerceivable> perceivables,
		ITerrain terrain)
	{
		var cell = new Mock<ICell>();
		var route = new Mock<IRouteCellDefinition>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.RouteDefinition).Returns(route.Object);
		cell.SetupGet(x => x.Perceivables).Returns(perceivables);
		cell.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain);
		route.SetupGet(x => x.Cell).Returns(cell.Object);
		route.SetupGet(x => x.LengthMetres).Returns(length);
		route.SetupGet(x => x.MetresPerRoomEquivalent).Returns(100.0);
		route.SetupGet(x => x.PositiveDirectionName).Returns("townward");
		route.SetupGet(x => x.NegativeDirectionName).Returns("stationward");
		route.SetupGet(x => x.TopologyVersion).Returns(1L);
		route.SetupGet(x => x.Landmarks).Returns([]);
		route.SetupGet(x => x.ExitAnchors).Returns([]);
		return cell;
	}

	private sealed class PoweredHarness(
		RouteVehicleMovementStrategy strategy,
		Mock<IVehicle> vehicle,
		Mock<ICharacter> actor,
		FakeVehicleRouteMotionPersistence persistence,
		Func<double> position,
		Func<double> exteriorPosition,
		Func<double> powerConsumed,
		IReadOnlyList<ITrack> tracks,
		List<EventType> events)
	{
		public RouteVehicleMovementStrategy Strategy { get; } = strategy;
		public Mock<IVehicle> Vehicle { get; } = vehicle;
		public Mock<ICharacter> Actor { get; } = actor;
		public FakeVehicleRouteMotionPersistence Persistence { get; } = persistence;
		public double Position => position();
		public double ExteriorPosition => exteriorPosition();
		public double PowerConsumed => powerConsumed();
		public IReadOnlyList<ITrack> Tracks { get; } = tracks;
		public List<EventType> Events { get; } = events;
	}

	private sealed class AdvancingTimeProvider(TimeSpan timestampAdvance) : TimeProvider
	{
		private long _timestamp;

		public override long TimestampFrequency => TimeSpan.TicksPerSecond;

		public override long GetTimestamp()
		{
			var current = _timestamp;
			_timestamp = checked(_timestamp + timestampAdvance.Ticks);
			return current;
		}

		public void Advance(TimeSpan elapsed)
		{
			_timestamp = checked(_timestamp + elapsed.Ticks);
		}
	}

	private sealed class ExternalExitHarness(
		RouteVehicleMovementStrategy strategy,
		Mock<IVehicle> root,
		Mock<IVehicle> trailer,
		Mock<ICharacter> driver,
		Mock<ICharacter> puller,
		Mock<ICell> origin,
		Mock<ICell> destination,
		Mock<ICellExit> exit,
		Mock<IVehicleRoute> route,
		Mock<IVehicleOperationalReadinessService> readiness,
		Mock<IVehicleHitchGraphService> graph,
		VehicleHitchGraphMovePlan movePlan,
		IReadOnlyList<Mock<IGameItem>> hitchItems)
	{
		public RouteVehicleMovementStrategy Strategy { get; } = strategy;
		public Mock<IVehicle> Root { get; } = root;
		public Mock<IVehicle> Trailer { get; } = trailer;
		public Mock<ICharacter> Driver { get; } = driver;
		public Mock<ICharacter> Puller { get; } = puller;
		public Mock<ICell> Origin { get; } = origin;
		public Mock<ICell> Destination { get; } = destination;
		public Mock<ICellExit> Exit { get; } = exit;
		public Mock<IVehicleRoute> Route { get; } = route;
		public Mock<IVehicleOperationalReadinessService> Readiness { get; } = readiness;
		public Mock<IVehicleHitchGraphService> Graph { get; } = graph;
		public VehicleHitchGraphMovePlan MovePlan { get; } = movePlan;
		public IReadOnlyList<Mock<IGameItem>> HitchItems { get; } = hitchItems;
	}

	private sealed class AutomaticExitHarness(
		RouteVehicleMovementStrategy strategy,
		Mock<IVehicle> root,
		Mock<IVehicle> trailer,
		Mock<ICell> destination,
		Mock<ICellExit> exit,
		Mock<IVehicleRoute> route,
		Mock<IVehicleRouteLeg> leg,
		Mock<IVehicleJourney> journey,
		Mock<IVehicleMovementProfilePrototype> profile,
		Mock<IVehicleOperationalReadinessService> readiness,
		Mock<IVehicleHitchGraphService> graph,
		VehicleHitchGraphMovePlan movePlan,
		VehicleResourceReadinessPlan resourcePlan)
	{
		public RouteVehicleMovementStrategy Strategy { get; } = strategy;
		public Mock<IVehicle> Root { get; } = root;
		public Mock<IVehicle> Trailer { get; } = trailer;
		public Mock<ICell> Destination { get; } = destination;
		public Mock<ICellExit> Exit { get; } = exit;
		public Mock<IVehicleRoute> Route { get; } = route;
		public Mock<IVehicleRouteLeg> Leg { get; } = leg;
		public Mock<IVehicleJourney> Journey { get; } = journey;
		public Mock<IVehicleMovementProfilePrototype> Profile { get; } = profile;
		public Mock<IVehicleOperationalReadinessService> Readiness { get; } = readiness;
		public Mock<IVehicleHitchGraphService> Graph { get; } = graph;
		public VehicleHitchGraphMovePlan MovePlan { get; } = movePlan;
		public VehicleResourceReadinessPlan ResourcePlan { get; } = resourcePlan;
	}

	private sealed class FakeVehicleRouteMotionPersistence : IVehicleRouteMotionPersistence
	{
		private readonly HashSet<string> _resourceKeys = [];

		public int StartCount { get; private set; }
		public int CompleteCount { get; private set; }
		public double LastCheckpointPosition { get; private set; }
		public long LastCheckpointSequence { get; private set; }
		public bool IsActive { get; private set; }
		public bool ThrowAfterApplyingCharges { get; set; }
		public bool ThrowOnComplete { get; set; }

		public void Start(VehicleRouteMotionStart start)
		{
			StartCount++;
			LastCheckpointPosition = start.Segment.Origin.RoutePositionMetres!.Value;
			LastCheckpointSequence = 0L;
			IsActive = true;
		}

		public void CommitCheckpoint(
			VehicleRouteMotionCheckpoint checkpoint,
			Action<IReadOnlyCollection<VehicleRouteResourceCharge>> applyCharges)
		{
			if (!IsActive)
			{
				throw new InvalidOperationException(
					$"Vehicle route motion operation {checkpoint.OperationId:N} has no active durable row for checkpoint {checkpoint.Sequence:N0}.");
			}

			LastCheckpointPosition = checkpoint.PositionMetres;
			LastCheckpointSequence = checkpoint.Sequence;
			var committed = checkpoint.Charges
				.Where(x => _resourceKeys.Add(
					$"{checkpoint.OperationId:N}:{checkpoint.Sequence}:{x.ResourceKind}:{x.OwnerId}:{x.ResourceKey}"))
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

			IsActive = false;
		}
	}
}
