#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Vehicles;

namespace MudSharpCore_Unit_Tests.Vehicles;

[TestClass]
public class VehicleJourneyStateRulesTests
{
	[TestMethod]
	public void TryOpenBoarding_AutomaticService_OpensAccessPointBeforeDocking()
	{
		var gameworld = new Mock<IFuturemud>();
		var stopCell = new Mock<ICell>();
		var platformCell = new Mock<ICell>();
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var accessPrototype = new Mock<IVehicleAccessPointPrototype>();
		accessPrototype.SetupGet(x => x.Id).Returns(31);
		var accessPoint = new Mock<IVehicleAccessPoint>();
		var accessIsOpen = false;
		accessPoint.SetupGet(x => x.Id).Returns(41);
		accessPoint.SetupGet(x => x.Name).Returns("passenger doors");
		accessPoint.SetupGet(x => x.Prototype).Returns(accessPrototype.Object);
		accessPoint.SetupGet(x => x.IsOpen).Returns(() => accessIsOpen);
		accessPoint.SetupGet(x => x.IsDisabled).Returns(false);
		accessPoint.SetupGet(x => x.IsLocked).Returns(false);
		accessPoint.Setup(x => x.SetOpen(It.IsAny<bool>()))
			.Callback<bool>(value => accessIsOpen = value);
		var binding = new Mock<IVehicleRoutePlatformBinding>();
		binding.SetupGet(x => x.Id).Returns(51);
		binding.SetupGet(x => x.PlatformCell).Returns(platformCell.Object);
		binding.SetupGet(x => x.AccessPoint).Returns(accessPrototype.Object);
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.Location)
			.Returns(new SpatialLocation(stopCell.Object, RoomLayer.GroundLevel, null));
		stop.SetupGet(x => x.PlatformBindings).Returns([binding.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.Location).Returns(stopCell.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.AccessPoints).Returns([accessPoint.Object]);
		vehicle.SetupGet(x => x.Dockings).Returns([]);
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.OperatorMode).Returns(VehicleServiceOperatorMode.Automatic);
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		journey.SetupGet(x => x.Service).Returns(service.Object);
		journey.SetupGet(x => x.CurrentStop).Returns(stop.Object);
		var docking = new Mock<IVehicleDocking>();
		var dockingService = new Mock<IVehicleDockingService>();
		dockingService.Setup(x => x.CanDock(vehicle.Object, accessPoint.Object, platformCell.Object,
				RoomLayer.GroundLevel, stop.Object, out It.Ref<string>.IsAny))
			.Returns(true);
		dockingService.Setup(x => x.Dock(vehicle.Object, accessPoint.Object, platformCell.Object,
				RoomLayer.GroundLevel, stop.Object))
			.Returns(docking.Object);
		var coordinator = new VehicleJourneyCoordinator(gameworld.Object, dockingService: dockingService.Object);

		Assert.IsTrue(coordinator.TryOpenBoarding(journey.Object, out var reason), reason);
		Assert.IsTrue(accessIsOpen);
		accessPoint.Verify(x => x.SetOpen(true), Times.Once);
		dockingService.Verify(x => x.SetBoardingOpen(docking.Object, true), Times.Once);
	}

	[TestMethod]
	public void TryOpenBoarding_OnboardServiceWithClosedAccessPoint_FailsWithoutPhantomDocking()
	{
		var gameworld = new Mock<IFuturemud>();
		var stopCell = new Mock<ICell>();
		var platformCell = new Mock<ICell>();
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var accessPrototype = new Mock<IVehicleAccessPointPrototype>();
		accessPrototype.SetupGet(x => x.Id).Returns(31);
		var accessPoint = new Mock<IVehicleAccessPoint>();
		accessPoint.SetupGet(x => x.Name).Returns("passenger doors");
		accessPoint.SetupGet(x => x.Prototype).Returns(accessPrototype.Object);
		accessPoint.SetupGet(x => x.IsOpen).Returns(false);
		accessPoint.SetupGet(x => x.IsDisabled).Returns(false);
		accessPoint.SetupGet(x => x.IsLocked).Returns(false);
		var binding = new Mock<IVehicleRoutePlatformBinding>();
		binding.SetupGet(x => x.Id).Returns(51);
		binding.SetupGet(x => x.PlatformCell).Returns(platformCell.Object);
		binding.SetupGet(x => x.AccessPoint).Returns(accessPrototype.Object);
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.Location)
			.Returns(new SpatialLocation(stopCell.Object, RoomLayer.GroundLevel, null));
		stop.SetupGet(x => x.PlatformBindings).Returns([binding.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.Location).Returns(stopCell.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.AccessPoints).Returns([accessPoint.Object]);
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.OperatorMode).Returns(VehicleServiceOperatorMode.Onboard);
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		journey.SetupGet(x => x.Service).Returns(service.Object);
		journey.SetupGet(x => x.CurrentStop).Returns(stop.Object);
		var dockingService = new Mock<IVehicleDockingService>();
		var coordinator = new VehicleJourneyCoordinator(gameworld.Object, dockingService: dockingService.Object);

		Assert.IsFalse(coordinator.TryOpenBoarding(journey.Object, out var reason));
		StringAssert.Contains(reason, "must be opened by the onboard operator");
		accessPoint.Verify(x => x.SetOpen(It.IsAny<bool>()), Times.Never);
		dockingService.Verify(x => x.Dock(It.IsAny<IVehicle>(), It.IsAny<IVehicleAccessPoint>(),
			It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<IVehicleRouteStop?>()), Times.Never);
		dockingService.Verify(x => x.SetBoardingOpen(It.IsAny<IVehicleDocking>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void TryCloseBoarding_AutomaticService_ClosesDockingBeforeAccessPoint()
	{
		var gameworld = new Mock<IFuturemud>();
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var accessPrototype = new Mock<IVehicleAccessPointPrototype>();
		accessPrototype.SetupGet(x => x.Id).Returns(31);
		var accessPoint = new Mock<IVehicleAccessPoint>();
		var accessIsOpen = true;
		var dockingClosed = false;
		accessPoint.SetupGet(x => x.Name).Returns("passenger doors");
		accessPoint.SetupGet(x => x.Prototype).Returns(accessPrototype.Object);
		accessPoint.SetupGet(x => x.IsOpen).Returns(() => accessIsOpen);
		accessPoint.Setup(x => x.SetOpen(false)).Callback(() =>
		{
			Assert.IsTrue(dockingClosed, "The transient boarding exit must close before the physical access point.");
			accessIsOpen = false;
		});
		var binding = new Mock<IVehicleRoutePlatformBinding>();
		binding.SetupGet(x => x.AccessPoint).Returns(accessPrototype.Object);
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.PlatformBindings).Returns([binding.Object]);
		var docking = new Mock<IVehicleDocking>();
		docking.SetupGet(x => x.State).Returns(VehicleDockingState.BoardingOpen);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.AccessPoints).Returns([accessPoint.Object]);
		vehicle.SetupGet(x => x.Dockings).Returns([docking.Object]);
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.OperatorMode).Returns(VehicleServiceOperatorMode.Automatic);
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		journey.SetupGet(x => x.Service).Returns(service.Object);
		journey.SetupGet(x => x.CurrentStop).Returns(stop.Object);
		var dockingService = new Mock<IVehicleDockingService>();
		dockingService.Setup(x => x.SetBoardingOpen(docking.Object, false))
			.Callback(() => dockingClosed = true);
		var coordinator = new VehicleJourneyCoordinator(gameworld.Object, dockingService: dockingService.Object);

		Assert.IsTrue(coordinator.TryCloseBoarding(journey.Object, out var reason), reason);
		Assert.IsTrue(dockingClosed);
		Assert.IsFalse(accessIsOpen);
		accessPoint.Verify(x => x.SetOpen(false), Times.Once);
	}

	[TestMethod]
	public void TryCloseBoarding_AutomaticServiceCannotCloseAccessPoint_FailsDeparture()
	{
		var gameworld = new Mock<IFuturemud>();
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var accessPrototype = new Mock<IVehicleAccessPointPrototype>();
		accessPrototype.SetupGet(x => x.Id).Returns(31);
		var accessPoint = new Mock<IVehicleAccessPoint>();
		accessPoint.SetupGet(x => x.Name).Returns("passenger doors");
		accessPoint.SetupGet(x => x.Prototype).Returns(accessPrototype.Object);
		accessPoint.SetupGet(x => x.IsOpen).Returns(true);
		var binding = new Mock<IVehicleRoutePlatformBinding>();
		binding.SetupGet(x => x.AccessPoint).Returns(accessPrototype.Object);
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.PlatformBindings).Returns([binding.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.AccessPoints).Returns([accessPoint.Object]);
		vehicle.SetupGet(x => x.Dockings).Returns([]);
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.OperatorMode).Returns(VehicleServiceOperatorMode.Automatic);
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		journey.SetupGet(x => x.Service).Returns(service.Object);
		journey.SetupGet(x => x.CurrentStop).Returns(stop.Object);
		var coordinator = new VehicleJourneyCoordinator(gameworld.Object, dockingService: new Mock<IVehicleDockingService>().Object);

		Assert.IsFalse(coordinator.TryCloseBoarding(journey.Object, out var reason));
		StringAssert.Contains(reason, "could not close");
		accessPoint.Verify(x => x.SetOpen(false), Times.Once);
	}

	[TestMethod]
	public void TryOpenBoarding_SameCellAndCoordinateWrongLayer_FailsClosed()
	{
		var gameworld = new Mock<IFuturemud>();
		var cell = new Mock<ICell>();
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Location).Returns(cell.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.HighInAir);
		vehicle.SetupGet(x => x.RoutePositionMetres).Returns(100.0);
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.Location)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 100.0));
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		journey.SetupGet(x => x.CurrentStop).Returns(stop.Object);
		var coordinator = new VehicleJourneyCoordinator(gameworld.Object);

		Assert.IsFalse(coordinator.TryOpenBoarding(journey.Object, out var reason));
		StringAssert.Contains(reason, "location and layer");
	}

	[TestMethod]
	public void CanTransition_NormalJourneySequence_AllowsEveryStep()
	{
		var sequence = new[]
		{
			VehicleJourneyState.Scheduled,
			VehicleJourneyState.Boarding,
			VehicleJourneyState.Departing,
			VehicleJourneyState.EnRoute,
			VehicleJourneyState.Dwelling,
			VehicleJourneyState.Departing,
			VehicleJourneyState.EnRoute,
			VehicleJourneyState.Arrived
		};

		foreach (var pair in sequence.Zip(sequence.Skip(1)))
		{
			Assert.IsTrue(VehicleJourneyStateRules.CanTransition(pair.First, pair.Second),
				$"Expected {pair.First} -> {pair.Second} to be legal.");
		}
	}

	[TestMethod]
	public void CanTransition_HoldRetryAndCancellation_AreLegal()
	{
		Assert.IsTrue(VehicleJourneyStateRules.CanTransition(VehicleJourneyState.Boarding, VehicleJourneyState.Held));
		Assert.IsTrue(VehicleJourneyStateRules.CanTransition(VehicleJourneyState.Held, VehicleJourneyState.Departing));
		Assert.IsTrue(VehicleJourneyStateRules.CanTransition(VehicleJourneyState.EnRoute, VehicleJourneyState.Cancelled));
		Assert.IsTrue(VehicleJourneyStateRules.CanTransition(VehicleJourneyState.Dwelling, VehicleJourneyState.Faulted));
	}

	[TestMethod]
	public void CanTransition_TerminalState_RejectsResumption()
	{
		foreach (var terminal in new[]
		         {
			         VehicleJourneyState.Arrived,
			         VehicleJourneyState.Cancelled,
			         VehicleJourneyState.Faulted
		         })
		{
			Assert.IsFalse(VehicleJourneyStateRules.CanTransition(terminal, VehicleJourneyState.EnRoute));
		}
	}

	[TestMethod]
	public void DowntimeDelay_AccruesForEveryActiveOperationalState()
	{
		var checkpoint = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
		var now = checkpoint.AddMinutes(7);

		foreach (var state in new[]
		         {
			VehicleJourneyState.Boarding,
			VehicleJourneyState.Held,
			VehicleJourneyState.Departing,
			VehicleJourneyState.EnRoute,
			VehicleJourneyState.Dwelling
		         })
		{
			Assert.AreEqual(TimeSpan.FromMinutes(7),
				VehicleJourneyStateRules.DowntimeDelay(state, checkpoint, now),
				$"State {state} should accrue displayed service delay while offline.");
		}

		foreach (var state in Enum.GetValues<VehicleJourneyState>()
			         .Where(x => !VehicleJourneyStateRules.AccruesDowntimeDelay(x)))
		{
			Assert.AreEqual(TimeSpan.Zero,
				VehicleJourneyStateRules.DowntimeDelay(state, checkpoint, now),
				$"State {state} should not accrue active-service downtime delay.");
		}
	}

	[TestMethod]
	public void ArrivalState_DockingFailureFaultsIntermediateAndTerminalStops()
	{
		Assert.AreEqual(VehicleJourneyState.Faulted,
			VehicleJourneyStateRules.ArrivalState(boardingOpened: false, terminal: false));
		Assert.AreEqual(VehicleJourneyState.Faulted,
			VehicleJourneyStateRules.ArrivalState(boardingOpened: false, terminal: true));
		Assert.AreEqual(VehicleJourneyState.Dwelling,
			VehicleJourneyStateRules.ArrivalState(boardingOpened: true, terminal: false));
		Assert.AreEqual(VehicleJourneyState.Arrived,
			VehicleJourneyStateRules.ArrivalState(boardingOpened: true, terminal: true));
	}

	[TestMethod]
	public void ResolveDurableState_LatestEventWinsCrashBoundaryMismatch()
	{
		Assert.AreEqual(
			VehicleJourneyState.EnRoute,
			VehicleJourneyStateRules.ResolveDurableState(
				VehicleJourneyState.Departing,
				VehicleJourneyState.EnRoute));
		Assert.AreEqual(
			VehicleJourneyState.Boarding,
			VehicleJourneyStateRules.ResolveDurableState(
				VehicleJourneyState.Boarding,
				null));
	}

	[TestMethod]
	public void DowntimeDelay_BackwardsWallClock_DoesNotCreateNegativeDelay()
	{
		var checkpoint = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);

		Assert.AreEqual(TimeSpan.Zero,
			VehicleJourneyStateRules.DowntimeDelay(
				VehicleJourneyState.EnRoute,
				checkpoint,
				checkpoint.AddMinutes(-1)));
	}

	[TestMethod]
	public void HoldHasExpired_UsesInclusiveConfiguredBoundary()
	{
		var started = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
		Assert.IsFalse(VehicleJourneyStateRules.HoldHasExpired(started, started.AddMinutes(14), TimeSpan.FromMinutes(15)));
		Assert.IsTrue(VehicleJourneyStateRules.HoldHasExpired(started, started.AddMinutes(15), TimeSpan.FromMinutes(15)));
		Assert.IsTrue(VehicleJourneyStateRules.HoldHasExpired(started, started, TimeSpan.Zero));
	}

	[TestMethod]
	public void HoldEpisodeStartedAt_RetryTransitionsAndRestartEvents_PreserveOriginalStart()
	{
		var firstHold = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
		var events = new[]
		{
			JourneyEvent(0, VehicleJourneyEventType.Scheduled, firstHold.AddMinutes(-1)),
			JourneyEvent(1, VehicleJourneyEventType.BoardingOpened, firstHold.AddSeconds(-30)),
			JourneyEvent(2, VehicleJourneyEventType.BoardingClosed, firstHold.AddSeconds(-1)),
			JourneyEvent(3, VehicleJourneyEventType.Held, firstHold),
			JourneyEvent(4, VehicleJourneyEventType.DelayChanged, firstHold.AddSeconds(30)),
			JourneyEvent(5, VehicleJourneyEventType.BoardingClosed, firstHold.AddSeconds(30)),
			JourneyEvent(6, VehicleJourneyEventType.Held, firstHold.AddSeconds(31))
		};

		Assert.AreEqual(firstHold, VehicleJourneyStateRules.HoldEpisodeStartedAt(events));
		Assert.IsTrue(VehicleJourneyStateRules.HoldHasExpired(
			VehicleJourneyStateRules.HoldEpisodeStartedAt(events)!.Value,
			firstHold.AddSeconds(30),
			TimeSpan.FromSeconds(30)));
	}

	[TestMethod]
	public void HoldEpisodeStartedAt_SuccessfulDeparture_ResetsNextStopHoldWindow()
	{
		var firstHold = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
		var secondHold = firstHold.AddHours(1);
		var events = new[]
		{
			JourneyEvent(0, VehicleJourneyEventType.Held, firstHold),
			JourneyEvent(1, VehicleJourneyEventType.Departed, firstHold.AddMinutes(1)),
			JourneyEvent(2, VehicleJourneyEventType.StopArrived, secondHold.AddSeconds(-30)),
			JourneyEvent(3, VehicleJourneyEventType.Held, secondHold)
		};

		Assert.AreEqual(secondHold, VehicleJourneyStateRules.HoldEpisodeStartedAt(events));
	}

	private static IVehicleJourneyEvent JourneyEvent(long sequence, VehicleJourneyEventType eventType,
		DateTimeOffset occurredAtUtc)
	{
		var journeyEvent = new Mock<IVehicleJourneyEvent>();
		journeyEvent.SetupGet(x => x.Sequence).Returns(sequence);
		journeyEvent.SetupGet(x => x.EventType).Returns(eventType);
		journeyEvent.SetupGet(x => x.OccurredAtUtc).Returns(occurredAtUtc);
		return journeyEvent.Object;
	}
}
