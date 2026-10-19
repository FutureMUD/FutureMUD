#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleExitBarrierTraversalTests
{
	[DataTestMethod]
	[DataRow(DamageType.Burning)]
	[DataRow(DamageType.Piercing)]
	[DataRow(DamageType.Slashing)]
	public void AdmittedTrainContact_DamagesEachCanonicalExteriorOnceAndStops(DamageType type)
	{
		var fixture = new Fixture(type);
		var stopped = VehicleExitBarrierTraversal.Contact(fixture.Plan, fixture.Exit.Object, [],
			fixture.Movement.Object, out var reason);

		Assert.IsTrue(stopped);
		Assert.AreEqual(VehicleExitBarrierTraversal.BarrierReason, reason);
		Assert.AreEqual(2, fixture.Damage.Count);
		CollectionAssert.AreEquivalent(new[] { 101L, 102L }, fixture.Damage.Select(x => x.Item.Id).ToArray());
		Assert.IsTrue(fixture.Damage.All(x => x.Damage.DamageType == type && x.Damage.DamageAmount == 21.0));
		Assert.IsTrue(fixture.Damage.All(x => x.Damage.MutationGuard is not null));
		foreach (var vehicle in fixture.Vehicles)
		{
			vehicle.Verify(x => x.MoveToRoom(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), It.IsAny<IRoomExit>(),
				It.IsAny<IMovement>()), Times.Never);
		}
	}

	[TestMethod]
	public void SameMovementContact_RepeatedCrossingStepDoesNotDamageTrainTwice()
	{
		var fixture = new Fixture(DamageType.Burning);
		Assert.IsTrue(VehicleExitBarrierTraversal.Contact(fixture.Plan, fixture.Exit.Object, [],
			fixture.Movement.Object, out _));
		Assert.IsTrue(VehicleExitBarrierTraversal.Contact(fixture.Plan, fixture.Exit.Object, [],
			fixture.Movement.Object, out _));
		Assert.AreEqual(2, fixture.Damage.Count);
	}

	[TestMethod]
	public void IndependentTrainsInOneMovement_ContactEachExteriorOnce()
	{
		var fixture = new Fixture(DamageType.Burning);
		foreach (var vehicle in fixture.Vehicles)
		{
			var plan = new VehicleHitchGraphMovePlan(vehicle.Object,
				[new VehicleHitchGraphTrainMember(vehicle.Object, 0, null)], [], [], 0.0);
			Assert.IsTrue(VehicleExitBarrierTraversal.Contact(plan, fixture.Exit.Object, [], fixture.Movement.Object, out _));
			Assert.IsTrue(VehicleExitBarrierTraversal.Contact(plan, fixture.Exit.Object, [], fixture.Movement.Object, out _));
		}
		Assert.AreEqual(2, fixture.Damage.Count);
		CollectionAssert.AreEquivalent(new[] { 101L, 102L }, fixture.Damage.Select(x => x.Item.Id).ToArray());
	}

	[TestMethod]
	public void ExposedOccupant_UsesOwnPlaneWhileMaterialHullContactsBarrier()
	{
		var fixture = new Fixture(DamageType.Burning);
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Gameworld).Returns(fixture.Vehicles[0].Object.Gameworld);
		actor.SetupGet(x => x.Location).Returns(fixture.Vehicles[0].Object.Location);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.Setup(x => x.CanCross(fixture.Exit.Object)).Returns((true, null!));
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2L));
		Mock.Get(fixture.Vehicles[0].Object.Prototype).SetupGet(x => x.ExposesOccupantsToExitBarriers).Returns(true);
		fixture.Vehicles[0].SetupGet(x => x.Occupants).Returns([actor.Object]);

		Assert.IsTrue(VehicleExitBarrierTraversal.Contact(fixture.Plan, fixture.Exit.Object, [], fixture.Movement.Object, out _));
		Assert.AreEqual(2, fixture.Damage.Count);
		body.Verify(x => x.SufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void SynchronousContact_WithoutCharacterMovementStillDamagesAndStopsTrain()
	{
		var fixture = new Fixture(DamageType.Piercing);
		Assert.IsTrue(VehicleExitBarrierTraversal.Contact(fixture.Plan, fixture.Exit.Object, [], null, out _));
		Assert.AreEqual(2, fixture.Damage.Count);
	}

	[TestMethod]
	public void DifferentContactPlane_DoesNotDamageOrStopTrain()
	{
		var fixture = new Fixture(DamageType.Slashing, plane: 2L);
		Assert.IsFalse(VehicleExitBarrierTraversal.Contact(fixture.Plan, fixture.Exit.Object, [],
			fixture.Movement.Object, out _));
		Assert.AreEqual(0, fixture.Damage.Count);
	}

	[TestMethod]
	public void ClosedDoorBeforeContact_StopsWithoutNativeDamage()
	{
		var fixture = new Fixture(DamageType.Burning);
		var door = new Mock<IDoor>();
		door.SetupGet(x => x.IsOpen).Returns(false);
		fixture.Boundary.SetupGet(x => x.Door).Returns(door.Object);
		Assert.IsTrue(VehicleExitBarrierTraversal.Contact(fixture.Plan, fixture.Exit.Object, [],
			fixture.Movement.Object, out var reason));
		Assert.AreEqual("The door through that exit is closed.", reason);
		Assert.AreEqual(0, fixture.Damage.Count);
	}

	[TestMethod]
	public void VehicleEligibilityQueries_NeverApplyNativeContact()
	{
		var fixture = new Fixture(DamageType.Burning);
		foreach (var vehicle in fixture.Vehicles)
		{
			Assert.IsTrue(fixture.Barrier.OccupiesLayerAndPlane(vehicle.Object.ExteriorItem));
		}
		Assert.AreEqual(0, fixture.Damage.Count);
	}

	[TestMethod]
	public void ExternalCharacterAdmissionDenied_PreventsAllTrainContactDamage()
	{
		var fixture = new Fixture(DamageType.Burning);
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Gameworld).Returns(fixture.Vehicles[0].Object.Gameworld);
		actor.SetupGet(x => x.Location).Returns(fixture.Vehicles[0].Object.Location);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1L));
		actor.Setup(x => x.CanCross(fixture.Exit.Object)).Returns((false, null!));

		Assert.IsTrue(VehicleExitBarrierTraversal.Contact(fixture.Plan, fixture.Exit.Object, [actor.Object],
			fixture.Movement.Object, out _));
		Assert.AreEqual(0, fixture.Damage.Count);
		actor.Verify(x => x.CanCross(fixture.Exit.Object), Times.Once);
	}

	[TestMethod]
	public void RoomExitStrategy_ReadinessQueryIsPureAndAdmittedMoveStopsBeforeTrainCommit()
	{
		var fixture = new Fixture(DamageType.Burning);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var readiness = new VehicleMovementReadinessResult(true, string.Empty, fixture.Plan, null, []);
		var readinessService = new Mock<IVehicleOperationalReadinessService>();
		readinessService.Setup(x => x.BuildMovementReadiness(It.IsAny<VehicleMovementReadinessRequest>())).Returns(readiness);
		readinessService.Setup(x => x.RollTowCatastrophe(fixture.Plan, actor.Object))
			.Returns(new VehicleTowCatastropheResult(false, null, string.Empty, [], []));
		var graph = new Mock<IVehicleHitchGraphService>();
		var strategy = new RoomExitVehicleMovementStrategy(new Mock<IVehicleTowService>().Object,
			graph.Object, readinessService.Object);

		Assert.IsTrue(strategy.CanMove(fixture.Vehicles[0].Object, actor.Object, fixture.Exit.Object, out _));
		Assert.AreEqual(0, fixture.Damage.Count);
		Assert.IsFalse(strategy.Move(fixture.Vehicles[0].Object, actor.Object, fixture.Exit.Object));
		Assert.AreEqual(2, fixture.Damage.Count);
		graph.Verify(x => x.CompleteVehicleTrainMove(It.IsAny<VehicleHitchGraphMovePlan>(), It.IsAny<IRoom>(),
			It.IsAny<RoomLayer>(), It.IsAny<IRoomExit>(), It.IsAny<IMovement>(), It.IsAny<IVehicle>()), Times.Never);
		readinessService.Verify(x => x.ConsumeMovementResources(It.IsAny<VehicleResourceReadinessPlan>()), Times.Never);
	}

	[TestMethod]
	public void OrdinaryMovement_DraggedTrainContactsBeforeExemptPullerRelocates()
	{
		var f = new Fixture(DamageType.Burning);
		var actor = new Mock<ICharacter>(); var body = new Mock<IBody>();
		actor.SetupGet(x => x.Id).Returns(201); actor.SetupGet(x => x.InstanceId).Returns(201);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		actor.SetupGet(x => x.Gameworld).Returns(f.Vehicles[0].Object.Gameworld);
		actor.SetupGet(x => x.Location).Returns(f.Exit.Object.Origin);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns<ICharacter>(x => ReferenceEquals(x, actor.Object));
		actor.Setup(x => x.CanMove(f.Exit.Object, CanMoveFlags.None)).Returns(CanMoveResponse.True);
		actor.Setup(x => x.CanCross(f.Exit.Object)).Returns((true, null!));
		actor.Setup(x => x.AffectedBy<IImmwalkEffect>()).Returns(true);
		actor.SetupProperty(x => x.Movement);
		var movement = new Movement(actor.Object, null!, [], [], [actor.Object], [], [], [], f.Exit.Object);
		actor.Object.Movement = movement;
		// Admission builds this cache before contact; isolate the commit stage from tow graph setup.
		var plans = (Dictionary<long, VehicleHitchGraphMovePlan>)typeof(Movement)
			.GetField("_vehicleDragMovePlans", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(movement)!;
		plans.Add(f.Plan.RootVehicle.Id, f.Plan);
		movement.IntermediateStep();
		Assert.IsTrue(movement.Cancelled);
		Assert.IsNull(actor.Object.Movement);
		Assert.AreEqual(2, f.Damage.Count, "The puller's exemption does not shelter the train hulls.");
		actor.Verify(x => x.ExecuteMove(It.IsAny<IMovement>()), Times.Never);
		foreach (var vehicle in f.Vehicles)
			Mock.Get(f.Exit.Object.Origin).Verify(x => x.Extract(vehicle.Object.ExteriorItem), Times.Never);
		movement.IntermediateStep();
		Assert.AreEqual(2, f.Damage.Count);
	}

	private sealed class Fixture
	{
		internal Mock<IRoomExit> Exit { get; } = new();
		internal Mock<IExit> Boundary { get; } = new();
		internal Mock<IMovement> Movement { get; } = new();
		internal List<Mock<IVehicle>> Vehicles { get; } = [];
		internal List<(IGameItem Item, IDamage Damage)> Damage { get; } = [];
		internal VehicleHitchGraphMovePlan Plan { get; }
		internal SpellDamagingExitBarrierEffect Barrier { get; }

		internal Fixture(DamageType type, long plane = 1L)
		{
			var world = new Mock<IFuturemud>();
			var origin = new Mock<IRoom>();
			var destination = new Mock<IRoom>();
			origin.SetupGet(x => x.Id).Returns(1L);
			destination.SetupGet(x => x.Id).Returns(2L);
			origin.SetupGet(x => x.Gameworld).Returns(world.Object);
			destination.SetupGet(x => x.Gameworld).Returns(world.Object);
			Boundary.SetupGet(x => x.Gameworld).Returns(world.Object);
			Boundary.SetupGet(x => x.Rooms).Returns([origin.Object, destination.Object]);
			Exit.SetupGet(x => x.Exit).Returns(Boundary.Object);
			Exit.SetupGet(x => x.Origin).Returns(origin.Object);
			Exit.SetupGet(x => x.Destination).Returns(destination.Object);
			var opposite = new Mock<IRoomExit>();
			opposite.SetupGet(x => x.Exit).Returns(Boundary.Object);
			opposite.SetupGet(x => x.Origin).Returns(destination.Object);
			opposite.SetupGet(x => x.Destination).Returns(origin.Object);
			Exit.SetupGet(x => x.Opposite).Returns(opposite.Object);
			Boundary.Setup(x => x.RoomExitFor(origin.Object)).Returns(Exit.Object);
			Movement.SetupGet(x => x.Exit).Returns(Exit.Object);
			Movement.SetupGet(x => x.Phase).Returns(MovementPhase.OriginalRoom);

			var parent = new Mock<IMagicSpellEffectParent>();
			parent.SetupGet(x => x.Owner).Returns(Boundary.Object);
			Barrier = new SpellDamagingExitBarrierEffect(Boundary.Object, parent.Object, type, 21.0,
				RoomLayer.GroundLevel, [plane], DateTime.MaxValue);
			parent.SetupGet(x => x.SpellEffects).Returns([Barrier]);
			Boundary.SetupGet(x => x.Effects).Returns([parent.Object, Barrier]);

			for (var i = 1; i <= 2; i++)
			{
				var item = new Mock<IGameItem>();
				item.SetupGet(x => x.Id).Returns(100L + i);
				item.SetupGet(x => x.Gameworld).Returns(world.Object);
				item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1L));
				item.SetupGet(x => x.Location).Returns(origin.Object);
				item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
				item.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
				item.Setup(x => x.SufferDamage(It.IsAny<IDamage>()))
					.Callback<IDamage>(damage =>
					{
						damage.MutationGuard!.Validate(item.Object, damage);
						Damage.Add((item.Object, damage));
					})
					.Returns(Array.Empty<IWound>());
				var vehicle = new Mock<IVehicle>();
				var prototype = new Mock<IVehiclePrototype>();
				prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
				vehicle.SetupGet(x => x.Id).Returns(i);
				vehicle.SetupGet(x => x.Gameworld).Returns(world.Object);
				vehicle.SetupGet(x => x.Location).Returns(origin.Object);
				vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
				vehicle.SetupGet(x => x.ExteriorItem).Returns(item.Object);
				vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
				vehicle.SetupGet(x => x.Occupants).Returns(Array.Empty<ICharacter>());
				Exit.Setup(x => x.MovementTransition(item.Object))
					.Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
				Vehicles.Add(vehicle);
			}

			Plan = new VehicleHitchGraphMovePlan(Vehicles[0].Object,
				Vehicles.Select((x, i) => new VehicleHitchGraphTrainMember(x.Object, i, null)).ToArray(), [], [], 100.0);
		}
	}
}
