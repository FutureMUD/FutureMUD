#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Testing.EnvironmentalMagic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalMagicPersistenceBoundaryTests
{
	[TestMethod]
	[TestCategory("E-T09")]
	public void LostAcknowledgement_FreezesDeferredWritesAndExactRetryAdoptsTheDurableResult()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.At(0);
		var resource = world.Resources.Get(1)!;
		room.AddResource(resource, 25.0);
		var before = room.EnvironmentState;
		world.Clock.Advance(TimeSpan.FromMinutes(1));
		var request = Request(damage: 10.0, pressure: 8.0);
		world.Operations.FailAfterCommit = true;

		var uncertain = world.Coordinator.ApplyOperation(room, request);

		Assert.IsFalse(uncertain.Success);
		Assert.AreEqual(request.OperationId, room.PendingEnvironmentalOperationId);
		Assert.AreEqual(before, room.EnvironmentState);
		Assert.AreEqual(25.0, world.Balance());
		Assert.AreEqual(10.0, world.Operations.PersistedStates[room.Id].ScarDamage);
		Assert.AreEqual(26.0, world.Operations.PersistedResourceAmounts[room.Id][resource.Id], 1e-9);
		Assert.IsFalse(room.CanUseResource(resource, 1.0));
		Assert.IsFalse(room.UseResource(resource, 1.0));
		room.AddResource(resource, 10.0);
		Assert.IsFalse(world.Coordinator.TryDebit(room, resource, 1.0, out _));
		Assert.AreEqual(25.0, world.Balance());
		var saved = world.Snapshot(room);
		Assert.AreEqual(10.0, saved.EnvironmentalState!.ScarDamage);
		Assert.AreEqual(26.0, saved.RoomsMagicResources.Single().Amount, 1e-9);

		world.Operations.FailAfterCommit = false;
		var confirmed = world.Coordinator.ApplyOperation(room, request);

		Assert.IsTrue(confirmed.Success, confirmed.Error);
		Assert.IsTrue(confirmed.Replayed);
		Assert.IsNull(room.PendingEnvironmentalOperationId);
		Assert.AreEqual(10.0, confirmed.AppliedDamage);
		Assert.AreEqual(8.0, confirmed.AppliedPressure);
		Assert.AreEqual(10.0, room.EnvironmentState.ScarDamage);
		Assert.AreEqual(8.0, world.Coordinator.InspectState(room).Pressure, 1e-9);
		Assert.AreEqual(26.0, world.Balance(), 1e-9);
		Assert.AreEqual(1, world.Operations.Commits);
		Assert.AreEqual(1, world.Operations.Receipts.Count);
	}

	[TestMethod]
	[TestCategory("E-T09")]
	public void UnavailableConfirmation_RetainsTheFreezeAndRejectsAnotherOperationIdentity()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.At(0);
		var request = Request(damage: 3.0);
		world.Operations.FailAfterCommit = true;
		Assert.IsFalse(world.Coordinator.ApplyOperation(room, request).Success);
		world.Operations.FailRead = true;

		Assert.IsFalse(world.Coordinator.ApplyOperation(room, request).Success);
		Assert.AreEqual(request.OperationId, room.PendingEnvironmentalOperationId);
		var reads = world.Operations.Reads;
		Assert.IsFalse(world.Coordinator.ApplyOperation(room, Request(damage: 4.0)).Success);
		Assert.AreEqual(reads, world.Operations.Reads);
		Assert.ThrowsException<InvalidOperationException>(() =>
			world.Coordinator.SetBinding(room, EnvironmentalMagicBindingMode.Disabled, null));
		Assert.AreEqual(0.0, room.EnvironmentState.ScarDamage);
		Assert.AreEqual(1, world.Operations.Commits);

		world.Operations.FailRead = false;
		world.Operations.FailAfterCommit = false;
		var result = world.Coordinator.ApplyOperation(room, request);
		Assert.IsTrue(result.Success, result.Error);
		Assert.IsTrue(result.Replayed);
		Assert.IsNull(room.PendingEnvironmentalOperationId);
		Assert.AreEqual(3.0, room.EnvironmentState.ScarDamage);
		Assert.AreEqual(1, world.Operations.Commits);
	}

	[TestMethod]
	[TestCategory("E-T09")]
	public void OrdinaryDuplicate_PreservesNewerUncommittedResourceChanges()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.At(0);
		var request = Request(damage: 5.0);
		Assert.IsTrue(world.Coordinator.ApplyOperation(room, request).Success);
		room.AddResource(world.Resources.Get(1)!, 40.0);
		var state = room.EnvironmentState;

		var duplicate = world.Coordinator.ApplyOperation(room, request);

		Assert.IsTrue(duplicate.Success, duplicate.Error);
		Assert.IsTrue(duplicate.Replayed);
		Assert.AreEqual(40.0, world.Balance());
		Assert.AreEqual(state, room.EnvironmentState);
		Assert.IsTrue(room.ResourcesChanged);
		Assert.AreEqual(0.0, world.Operations.PersistedResourceAmounts[room.Id][1]);
		Assert.AreEqual(1, world.Operations.Commits);
	}

	[TestMethod]
	[TestCategory("E-T09")]
	public void FailureBeforeCommit_PreservesLiveStateAndDoesNotApplyProjectedProduction()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.At(0);
		room.AddResource(world.Resources.Get(1)!, 20.0);
		var before = room.EnvironmentState;
		world.Clock.Advance(TimeSpan.FromMinutes(1));
		world.Operations.FailCommit = true;
		var request = Request(damage: 6.0, pressure: 2.0);

		var result = world.Coordinator.ApplyOperation(room, request);

		Assert.IsFalse(result.Success);
		Assert.IsNull(room.PendingEnvironmentalOperationId);
		Assert.AreEqual(before, room.EnvironmentState);
		Assert.AreEqual(20.0, world.Balance());
		Assert.IsTrue(room.ResourcesChanged);
		Assert.AreEqual(0, world.Operations.Commits);
		Assert.AreEqual(0, world.Operations.Receipts.Count);
		world.Operations.FailCommit = false;
		var retry = world.Coordinator.ApplyOperation(room, request);
		Assert.IsTrue(retry.Success, retry.Error);
		Assert.IsFalse(retry.Replayed);
		Assert.AreEqual(6.0, room.EnvironmentState.ScarDamage);
		Assert.AreEqual(1, world.Operations.Commits);
	}

	[TestMethod]
	[TestCategory("E-T09")]
	public void RolledBackClaim_ConfirmedAbsenceReleasesTheFreezeAndRetainsOlderDeferredWork()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.At(0);
		room.AddResource(world.Resources.Get(1)!, 20.0);
		var before = room.EnvironmentState;
		world.Operations.FailAfterClaim = true;
		var request = Request(damage: 6.0);

		Assert.IsFalse(world.Coordinator.ApplyOperation(room, request).Success);
		Assert.AreEqual(request.OperationId, room.PendingEnvironmentalOperationId);
		Assert.AreEqual(before, room.EnvironmentState);
		Assert.AreEqual(20.0, world.Balance());
		Assert.AreEqual(0, world.Operations.Commits);
		Assert.AreEqual(0, world.Operations.Receipts.Count);
		Assert.AreEqual(0.0, world.Snapshot(room).RoomsMagicResources.Single().Amount);
		world.Operations.FailAfterClaim = false;

		var retry = world.Coordinator.ApplyOperation(room, request);

		Assert.IsTrue(retry.Success, retry.Error);
		Assert.IsFalse(retry.Replayed);
		Assert.IsNull(room.PendingEnvironmentalOperationId);
		Assert.AreEqual(6.0, room.EnvironmentState.ScarDamage);
		Assert.AreEqual(20.0, world.Balance());
		Assert.AreEqual(20.0, world.Operations.PersistedResourceAmounts[room.Id][1]);
		Assert.AreEqual(1, world.Operations.Commits);
	}

	[TestMethod]
	[TestCategory("E-T04")]
	public void DeferredEnvironmentalSave_RejectsAnUnexpectedPersistedRevision()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.At(0);
		Assert.IsTrue(world.Coordinator.ApplyOperation(room, Request(damage: 10.0)).Success);
		world.Edit("repair 1");
		room.AddResource(world.Resources.Get(1)!, 0.0);
		world.Tick(60);
		Assert.AreEqual(9.0, room.EnvironmentState.ScarDamage, 1e-9);
		var persisted = world.Models[room.Id].EnvironmentalState!;
		persisted.Revision += 100;
		persisted.ScarDamage = 77.0;

		var error = Assert.ThrowsException<TargetInvocationException>(() => world.Snapshot(room));

		Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
		Assert.AreEqual(77.0, persisted.ScarDamage);
	}

	[TestMethod]
	[TestCategory("E-T11")]
	public void CommittedSettlement_PreservesBalancesAbsentFromThePartialProductionPlan()
	{
		using var world = new EnvironmentalMagicTestWorld(outputs: 2);
		var room = (Room)world.Rooms.At(0);
		world.Edit("output 2 baserate 0");
		room.AddResource(world.Resources.Get(1)!, 10.0);
		room.AddResource(world.Resources.Get(2)!, 31.0);
		world.Clock.Advance(TimeSpan.FromMinutes(1));

		var result = world.Coordinator.ApplyOperation(room, Request(damage: 2.0));

		Assert.IsTrue(result.Success, result.Error);
		Assert.AreEqual(11.0, world.Balance(resourceId: 1), 1e-9);
		Assert.AreEqual(31.0, world.Balance(resourceId: 2), 1e-9);
		Assert.AreEqual(11.0, world.Operations.PersistedResourceAmounts[room.Id][1], 1e-9);
		Assert.AreEqual(31.0, world.Operations.PersistedResourceAmounts[room.Id][2], 1e-9);
	}

	[TestMethod]
	[TestCategory("E-T09")]
	public void RepairWithLostAcknowledgement_ReportsOnlyActualRepairAndRetainsDefileHistory()
	{
		using var world = new EnvironmentalMagicTestWorld(activePercent: 0.0);
		var room = (Room)world.Rooms.At(0);
		world.Edit("repair 5");
		Assert.IsTrue(world.Coordinator.ApplyOperation(room, Request(damage: 10.0, pressure: 8.0)).Success);
		var lastDefile = room.EnvironmentState.LastDefileUtc;
		world.Clock.Advance(TimeSpan.FromMinutes(1));
		var request = Request(repair: 20.0);
		world.Operations.FailAfterCommit = true;

		Assert.IsFalse(world.Coordinator.ApplyOperation(room, request).Success);
		Assert.AreEqual(10.0, room.EnvironmentState.ScarDamage);
		Assert.AreEqual(0.0, world.Operations.PersistedStates[room.Id].ScarDamage);
		world.Operations.FailAfterCommit = false;
		var result = world.Coordinator.ApplyOperation(room, request);

		Assert.IsTrue(result.Success, result.Error);
		Assert.IsTrue(result.Replayed);
		Assert.AreEqual(5.0, result.AppliedRepair, 1e-9);
		Assert.AreEqual(0.0, room.EnvironmentState.ScarDamage);
		Assert.AreEqual(lastDefile, room.EnvironmentState.LastDefileUtc);
		Assert.AreEqual(8.0 * Math.Pow(2.0, -1.0 / 60.0), world.Coordinator.InspectState(room).Pressure, 1e-9);
		Assert.AreEqual(2, world.Operations.Commits);
	}

	[DataTestMethod]
	[DataRow(1e-16, 10)]
	[DataRow(1e-17, 20)]
	[TestCategory("E-P21")]
	public void SubUlpPositiveRates_AccumulateProductionAndRepairAcrossAcceptedMinuteSamples(double rate, int minutes)
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.At(0);
		world.Edit($"output 1 baserate {rate.ToString("G17", CultureInfo.InvariantCulture)}");
		world.Edit($"repair {rate.ToString("G17", CultureInfo.InvariantCulture)}");
		room.AddResource(world.Resources.Get(1)!, 1.0);
		Assert.IsTrue(world.Coordinator.ApplyOperation(room, Request(damage: 1.0)).Success);

		for (var i = 0; i < minutes; i++) world.Tick(60);

		Assert.IsTrue(world.Balance() > 1.0, "Positive online production must survive rounding at each minute boundary.");
		Assert.IsTrue(room.EnvironmentState.ScarDamage < 1.0, "Positive online repair must survive rounding at each minute boundary.");
		Assert.AreEqual(1.0 + rate * minutes, world.Balance(), 2.3e-16);
		Assert.AreEqual(1.0 - rate * minutes, room.EnvironmentState.ScarDamage, 1.2e-16);
	}

	[DataTestMethod]
	[DataRow("full")]
	[DataRow("zero rate")]
	[DataRow("fault")]
	[DataRow("rebind")]
	[TestCategory("E-P21")]
	public void FractionalProduction_IsDiscardedWhenTheEarningIntervalEnds(string boundary)
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.At(0);
		var resource = world.Resources.Get(1)!;
		world.Edit("output 1 baserate 0.0000000000000001");
		room.AddResource(resource, 1.0);
		world.Tick(60);
		Assert.AreEqual(1.0, world.Balance());
		switch (boundary)
		{
			case "full":
				room.AddResource(resource, 99.0);
				Assert.IsTrue(room.UseResource(resource, 99.0));
				break;
			case "zero rate":
				world.Edit("output 1 baserate 0");
				room.AddResource(resource, 0.0);
				world.Edit("output 1 baserate 0.0000000000000001");
				room.AddResource(resource, 0.0);
				break;
			case "fault":
				world.Edit("output 1 maximum scardamage-1");
				room.AddResource(resource, 0.0);
				world.Edit("output 1 maximum basecapacity");
				room.AddResource(resource, 0.0);
				break;
			case "rebind":
				world.Coordinator.SetBinding(room, EnvironmentalMagicBindingMode.Disabled, null);
				world.Coordinator.SetBinding(room, EnvironmentalMagicBindingMode.Explicit, world.Profile.Id);
				break;
		}

		world.Tick(60);

		Assert.AreEqual(1.0, world.Balance(), "Only the new interval's sub-ULP earnings should remain after this boundary.");
	}

	[TestMethod]
	[TestCategory("E-T04")]
	public void MissingResourceDefinition_DeferredSaveRetainsTheRowAndLaterLoadRestoresItsBalance()
	{
		using var world = new EnvironmentalMagicTestWorld(count: 0, start: false);
		var original = world.CreateRoom(balance: 23.5);
		var model = world.Models[original.Id];
		var resource = world.Resources.Get(1)!;
		world.Resources.Remove(resource.Id);
		var loaded = world.LoadRoom(model);

		Assert.AreEqual(0, loaded.MagicResourceAmounts.Count);
		var saved = world.Snapshot(loaded);
		Assert.AreEqual(23.5, saved.RoomsMagicResources.Single().Amount);
		world.Resources.Add(resource);
		typeof(Room).GetMethod("CompleteMagicLoad", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(loaded, null);

		Assert.AreEqual(23.5, loaded.MagicResourceAmounts[resource]);
		Assert.AreEqual(23.5, world.Snapshot(loaded).RoomsMagicResources.Single().Amount);
	}

	[DataTestMethod]
	[DataRow(EnvironmentalMagicBindingMode.Explicit)]
	[DataRow(EnvironmentalMagicBindingMode.Inherit)]
	[TestCategory("E-T02")]
	public void MissingSelectedProfile_DoesNotFallBackToStartingResourcePolicies(EnvironmentalMagicBindingMode binding)
	{
		using var world = new EnvironmentalMagicTestWorld(count: 0, start: false);
		var resource = StartingResource(world);
		world.Profiles.Remove(world.Profile.Id);

		var room = world.CreateRoom(balance: null, binding: binding,
			profileId: binding == EnvironmentalMagicBindingMode.Explicit ? world.Profile.Id : null);

		Assert.AreEqual(0, room.MagicResourceAmounts.Count);
		resource.Verify(x => x.ShouldStartWithResource(It.IsAny<IHaveMagicResource>()), Times.Never);
		resource.Verify(x => x.StartingResourceAmount(It.IsAny<IHaveMagicResource>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(0L)]
	[DataRow(-1L)]
	[TestCategory("E-T02")]
	public void TransientRoom_DoesNotResolveAnEnvironmentalHolder(long id)
	{
		using var world = new EnvironmentalMagicTestWorld(count: 0, start: false);
		var resource = StartingResource(world);

		var room = world.CreateRoom(balance: null, binding: EnvironmentalMagicBindingMode.Explicit,
			profileId: world.Profile.Id, id: id);

		Assert.AreEqual(17.0, room.MagicResourceAmounts[resource.Object]);
		Assert.IsFalse(room.MagicResourceGenerators.Any());
		resource.Verify(x => x.ShouldStartWithResource(room), Times.Once);
	}

	private static Mock<IMagicResource> StartingResource(EnvironmentalMagicTestWorld world)
	{
		var resource = new Mock<IMagicResource>();
		resource.SetupGet(x => x.Id).Returns(1);
		resource.SetupGet(x => x.Name).Returns("Starting resource");
		resource.Setup(x => x.ShouldStartWithResource(It.IsAny<IHaveMagicResource>())).Returns(true);
		resource.Setup(x => x.StartingResourceAmount(It.IsAny<IHaveMagicResource>())).Returns(17.0);
		world.Resources.Add(resource.Object);
		return resource;
	}

	private static EnvironmentalMagicOperationRequest Request(double damage = 0.0, double pressure = 0.0, double repair = 0.0) =>
		new(Guid.NewGuid(), null, "Environmental persistence boundary test", damage, pressure, repair);
}
