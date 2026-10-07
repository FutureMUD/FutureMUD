#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Position;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProximityEventServiceTests
{
	[TestMethod]
	public void MovementIntoRegisteredOrdinaryRoom_EmitsDirectionalChangeOnce()
	{
		var service = new ProximityEventService();
		var origin = CreateRoom(1);
		var destination = CreateRoom(2);
		var receiver = CreatePerceivable(destination.Object);
		var subjectLocation = origin.Object;
		var subject = CreatePerceivable(() => subjectLocation);
		var events = CaptureProximityEvents(receiver);
		receiver.Setup(x => x.GetProximity(subject.Object))
			.Returns(() => ReferenceEquals(subjectLocation, destination.Object)
				? Proximity.Distant
				: Proximity.Unapproximable);

		using var registration = service.Register(receiver.Object, Proximity.Distant);
		using (var change = service.BeginChange(ProximityChangeCause.Movement, subject.Object))
		{
			subjectLocation = destination.Object;
			change.Complete();
		}

		Assert.AreEqual(1, events.Count);
		Assert.AreSame(receiver.Object, events[0].Receiver);
		Assert.AreSame(subject.Object, events[0].Counterpart);
		Assert.AreEqual(Proximity.Unapproximable, events[0].Previous);
		Assert.AreEqual(Proximity.Distant, events[0].Current);
	}

	[TestMethod]
	public void SameSpatialValueUpdate_DoesNotEmitAProximityEvent()
	{
		var service = new ProximityEventService();
		var room = CreateRoom(3);
		var receiver = CreatePerceivable(room.Object);
		var subject = CreatePerceivable(room.Object);
		var events = CaptureProximityEvents(receiver);
		receiver.Setup(x => x.GetProximity(subject.Object)).Returns(Proximity.Distant);

		using var registration = service.Register(receiver.Object, Proximity.Distant);
		using (var change = service.BeginChange(ProximityChangeCause.Movement, subject.Object))
		{
			change.Complete();
		}

		Assert.AreEqual(0, events.Count);
	}

	[TestMethod]
	public void DisposedRegistration_IsNotConsideredByLaterMovement()
	{
		var service = new ProximityEventService();
		var origin = CreateRoom(4);
		var destination = CreateRoom(5);
		var receiver = CreatePerceivable(destination.Object);
		var subjectLocation = origin.Object;
		var subject = CreatePerceivable(() => subjectLocation);
		var events = CaptureProximityEvents(receiver);
		receiver.Setup(x => x.GetProximity(subject.Object)).Returns(Proximity.Distant);

		var registration = service.Register(receiver.Object, Proximity.Distant);
		registration.Dispose();
		using (var change = service.BeginChange(ProximityChangeCause.Movement, subject.Object))
		{
			subjectLocation = destination.Object;
			change.Complete();
		}

		Assert.AreEqual(0, events.Count);
	}

	[TestMethod]
	public void UnregisteredRoomPopulation_IsNotSentTheEvent()
	{
		var service = new ProximityEventService();
		var origin = CreateRoom(6);
		var destination = CreateRoom(7);
		var receiver = CreatePerceivable(destination.Object);
		var subjectLocation = origin.Object;
		var subject = CreatePerceivable(() => subjectLocation);
		var events = CaptureProximityEvents(receiver);
		receiver.Setup(x => x.GetProximity(subject.Object))
			.Returns(() => ReferenceEquals(subjectLocation, destination.Object)
				? Proximity.Distant
				: Proximity.Unapproximable);
		var unrelated = Enumerable.Range(0, 1_000)
			.Select(_ => CreatePerceivable(destination.Object))
			.ToList();
		destination.SetupGet(x => x.Perceivables)
			.Returns([receiver.Object, subject.Object, .. unrelated.Select(x => x.Object)]);

		using var registration = service.Register(receiver.Object, Proximity.Distant);
		using (var change = service.BeginChange(ProximityChangeCause.Movement, subject.Object))
		{
			subjectLocation = destination.Object;
			change.Complete();
		}

		Assert.AreEqual(1, events.Count);
		foreach (var perceivable in unrelated)
		{
			perceivable.Verify(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()), Times.Never);
		}
	}

	[TestMethod]
	public void PersistedPositionHydration_DoesNotPublishALiveProximityTransition()
	{
		var service = new ProximityEventService();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ProximityEventService).Returns(service);
		var subject = new TestPerceivedItem(8, gameworld.Object);
		var target = new Mock<IPerceivable>();
		target.Setup(x => x.CanBePositionedAgainst(It.IsAny<MudSharp.Body.Position.IPositionState>(),
			It.IsAny<PositionModifier>())).Returns(true);
		target.Setup(x => x.GetProximity(subject)).Returns(() =>
			ReferenceEquals(subject.PositionTarget, target.Object) ? Proximity.Immediate : Proximity.Distant);
		gameworld.Setup(x => x.GetPerceivable("Test", 99)).Returns(target.Object);

		using var registration = service.Register(target.Object, Proximity.Immediate);
		subject.LoadPosition(0, 0, string.Empty, 99, "Test");

		target.Verify(x => x.HandleEvent(EventType.PerceivableProximityChanged, It.IsAny<object[]>()), Times.Never);
	}

	private static Mock<IRoom> CreateRoom(long id)
	{
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(id);
		room.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition?)null);
		room.SetupGet(x => x.Perceivables).Returns([]);
		return room;
	}

	private static Mock<IPerceivable> CreatePerceivable(IRoom location)
	{
		return CreatePerceivable(() => location);
	}

	private static Mock<IPerceivable> CreatePerceivable(Func<IRoom> location)
	{
		var perceivable = new Mock<IPerceivable>();
		perceivable.SetupGet(x => x.Location).Returns(location);
		perceivable.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		perceivable.SetupGet(x => x.RoutePositionMetres).Returns((double?)null);
		perceivable.SetupGet(x => x.SpatialLocation)
			.Returns(() => new SpatialLocation(location(), RoomLayer.GroundLevel));
		perceivable.SetupGet(x => x.TargetedBy).Returns([]);
		return perceivable;
	}

	private static List<ObservedChange> CaptureProximityEvents(Mock<IPerceivable> receiver)
	{
		var events = new List<ObservedChange>();
		receiver.Setup(x => x.HandleEvent(EventType.PerceivableProximityChanged, It.IsAny<object[]>()))
			.Callback((EventType _, object[] arguments) => events.Add(new ObservedChange(
				(IPerceivable)arguments[0],
				(IPerceivable)arguments[1],
				(Proximity)(double)arguments[2],
				(Proximity)(double)arguments[3])))
			.Returns(false);
		return events;
	}

	private sealed record ObservedChange(IPerceivable Receiver, IPerceivable Counterpart,
		Proximity Previous, Proximity Current);

	private sealed class TestPerceivedItem : PerceivedItem
	{
		public TestPerceivedItem(long id, IFuturemud gameworld)
			: base(id)
		{
			_name = $"test item {id}";
			_keywords = new Lazy<List<string>>(() => ["test", "item"]);
			Gameworld = gameworld;
		}

		public override string FrameworkItemType => "TestPerceivedItem";
		public override ProgVariableTypes Type => ProgVariableTypes.Perceivable;

		public override void Register(IOutputHandler handler)
		{
		}

		public override object DatabaseInsert()
		{
			return this;
		}

		public override void SetIDFromDatabase(object dbitem)
		{
		}
	}
}
