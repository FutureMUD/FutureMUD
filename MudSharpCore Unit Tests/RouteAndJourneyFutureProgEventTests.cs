#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RouteAndJourneyFutureProgEventTests
{
	[TestMethod]
	public void RouteMovementEvents_PublishStablePayloadsOncePerHookTarget()
	{
		var cell = new Mock<ICell>();
		var mover = new Mock<IPerceivable>();
		var calls = CaptureEvents(mover);
		var operationId = Guid.Parse("488ec6c6-3244-482e-a7f5-58f21f02ecab");
		var context = new RouteMovementHookContext(
			operationId,
			cell.Object,
			100.0,
			500.0,
			RouteCellDirection.Positive,
			20.0);

		var duplicatedTargets = new[] { mover.Object, mover.Object };
		RouteMovementFutureProgEvents.Begin(duplicatedTargets, context);
		RouteMovementFutureProgEvents.Progress(duplicatedTargets, context, 100.0, 160.0);
		RouteMovementFutureProgEvents.Complete(duplicatedTargets, context);
		RouteMovementFutureProgEvents.Cancelled(duplicatedTargets, context, 160.0, "route blocked");

		CollectionAssert.AreEqual(
			new[]
			{
				EventType.RouteMovementBegin,
				EventType.RoutePositionChanged,
				EventType.RouteMovementProgress,
				EventType.RouteMovementComplete,
				EventType.RouteMovementCancelled
			},
			calls.Select(x => x.EventType).ToArray());
		Assert.AreSame(mover.Object, calls[0].Arguments[0]);
		Assert.AreSame(cell.Object, calls[0].Arguments[1]);
		Assert.AreEqual(100.0, calls[0].Arguments[2]);
		Assert.AreEqual(500.0, calls[0].Arguments[3]);
		Assert.AreEqual("Positive", calls[0].Arguments[4]);
		Assert.AreEqual(20.0, calls[0].Arguments[5]);
		Assert.AreEqual(operationId.ToString("D"), calls[0].Arguments[6]);
		Assert.AreEqual(100.0, calls[1].Arguments[2]);
		Assert.AreEqual(160.0, calls[1].Arguments[3]);
		Assert.AreEqual("route blocked", calls[4].Arguments[6]);
	}

	[TestMethod]
	public void RouteMovementProgress_DoesNotPublishWhenCoordinateDidNotChange()
	{
		var cell = new Mock<ICell>();
		var mover = new Mock<IPerceivable>();
		var calls = CaptureEvents(mover);
		var context = new RouteMovementHookContext(
			Guid.NewGuid(),
			cell.Object,
			100.0,
			500.0,
			RouteCellDirection.Positive,
			20.0);

		RouteMovementFutureProgEvents.Progress([mover.Object], context, 100.0, 100.0001);

		Assert.AreEqual(0, calls.Count);
	}

	[TestMethod]
	public void VehicleJourneyEvents_MapDurableTransitionsAndExposeLookupIds()
	{
		var exterior = new Mock<IGameItem>();
		var calls = CaptureEvents(exterior);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(44L);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		var route = new Mock<IVehicleRoute>();
		route.SetupGet(x => x.Id).Returns(22L);
		var service = new Mock<IVehicleService>();
		service.SetupGet(x => x.Id).Returns(33L);
		var cell = new Mock<ICell>();
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.Location)
			.Returns(new SpatialLocation(cell.Object, RoomLayer.GroundLevel, 7_150.0));
		var journey = new Mock<IVehicleJourney>();
		journey.SetupGet(x => x.Id).Returns(11L);
		journey.SetupGet(x => x.Route).Returns(route.Object);
		journey.SetupGet(x => x.Service).Returns(service.Object);
		journey.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		journey.SetupGet(x => x.CurrentStop).Returns(stop.Object);
		journey.SetupGet(x => x.Delay).Returns(TimeSpan.FromMinutes(4));

		VehicleJourneyFutureProgEvents.Dispatch(journey.Object, VehicleJourneyEventType.Departed, "departed");
		VehicleJourneyFutureProgEvents.Dispatch(journey.Object, VehicleJourneyEventType.StopArrived, "arrived");
		VehicleJourneyFutureProgEvents.Dispatch(journey.Object, VehicleJourneyEventType.DelayChanged, "signal delay");
		VehicleJourneyFutureProgEvents.Dispatch(journey.Object, VehicleJourneyEventType.Cancelled, "cancelled");
		VehicleJourneyFutureProgEvents.Dispatch(journey.Object, VehicleJourneyEventType.Faulted, "faulted");

		CollectionAssert.AreEqual(
			new[]
			{
				EventType.VehicleJourneyDeparted,
				EventType.VehicleJourneyArrived,
				EventType.VehicleJourneyDelayChanged,
				EventType.VehicleJourneyCancelled,
				EventType.VehicleJourneyFaulted
			},
			calls.Select(x => x.EventType).ToArray());
		foreach (var call in calls)
		{
			Assert.AreSame(exterior.Object, call.Arguments[0]);
			Assert.AreEqual(11L, call.Arguments[1]);
			Assert.AreEqual(22L, call.Arguments[2]);
			Assert.AreEqual(33L, call.Arguments[3]);
			Assert.AreEqual(44L, call.Arguments[4]);
		}
		Assert.AreSame(cell.Object, calls[0].Arguments[5]);
		Assert.AreEqual(7_150.0, calls[0].Arguments[6]);
		Assert.AreEqual(TimeSpan.FromMinutes(4), calls[2].Arguments[5]);
		Assert.AreEqual("signal delay", calls[2].Arguments[6]);
		Assert.AreEqual("cancelled", calls[3].Arguments[5]);
	}

	[TestMethod]
	public void VehicleJourneyEvents_MapOnlyPublicLifecycleTransitions()
	{
		var expected = new Dictionary<VehicleJourneyEventType, EventType?>
		{
			[VehicleJourneyEventType.Scheduled] = null,
			[VehicleJourneyEventType.BoardingOpened] = null,
			[VehicleJourneyEventType.Held] = null,
			[VehicleJourneyEventType.Departed] = EventType.VehicleJourneyDeparted,
			[VehicleJourneyEventType.Checkpointed] = null,
			[VehicleJourneyEventType.StopArrived] = EventType.VehicleJourneyArrived,
			[VehicleJourneyEventType.Dwelling] = null,
			[VehicleJourneyEventType.BoardingClosed] = null,
			[VehicleJourneyEventType.Completed] = null,
			[VehicleJourneyEventType.DelayChanged] = EventType.VehicleJourneyDelayChanged,
			[VehicleJourneyEventType.Cancelled] = EventType.VehicleJourneyCancelled,
			[VehicleJourneyEventType.Faulted] = EventType.VehicleJourneyFaulted
		};

		foreach (var pair in expected)
		{
			Assert.AreEqual(pair.Value, VehicleJourneyFutureProgEvents.HookEventFor(pair.Key), pair.Key.ToString());
		}
	}

	private static List<HookCall> CaptureEvents<T>(Mock<T> target) where T : class, IPerceivable
	{
		var calls = new List<HookCall>();
		target.Setup(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()))
			.Callback<EventType, object[]>((eventType, arguments) => calls.Add(new HookCall(eventType, arguments)))
			.Returns(false);
		return calls;
	}

	private sealed record HookCall(EventType EventType, object[] Arguments);
}
