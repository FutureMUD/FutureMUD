#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic.Environment;

namespace FutureMUDLibrary_UnitTests;

[TestClass]
public class RegionalGatheringTests
{
	private static readonly RegionalGatheringOwner Zone = new(RegionalGatheringOwnerKind.Zone, 1);
	private static RegionalGatheringRegistry World(params RegionalGatheringPool[] pools)
	{
		var registry = new RegionalGatheringRegistry();
		registry.Load([new(1, "Mixed living land", 1, 0, .1, new Dictionary<long, double> { [1] = 3, [2] = 5 })], pools);
		registry.Track(new(10, 1, [7, 8], 1));
		registry.Track(new(11, 1, [7], 2));
		registry.Track(new(12, 1, [], 1));
		registry.CompleteLoading();
		return registry;
	}
	[TestMethod]
	public void NewBinding_SumsMembersAndFillsExactlyOnce()
	{
		var world = World();
		world.SetBinding(Zone, 1, 0);
		Assert.AreEqual(11.0, world.Inspect(10)!.Stock);
		Assert.IsTrue(world.TryPlan(10, 1, out var plan));
		Assert.IsTrue(world.TryApply(plan));
		world.SetBinding(Zone, 1, 0);
		Assert.AreEqual(10.0, world.Inspect(11)!.Stock);
		world.Track(new(13, 1, [], 2));
		Assert.AreEqual(16.0, world.Inspect(13)!.Capacity);
		Assert.AreEqual(10.0, world.Inspect(13)!.Stock, "New terrain capacity must not manufacture life.");
	}
	[TestMethod]
	public void Reload_IndexesAllRoomsBeforeClampingAndDoesNotRefill()
	{
		var world = World(new RegionalGatheringPool(Zone, 1, 0, 40, 7, 11, true));
		Assert.AreEqual(7.0, world.Inspect(11)!.Stock);
		Assert.AreEqual(11.0, world.Inspect(11)!.Capacity);
		Assert.AreEqual(40L, world.Inspect(11)!.Revision);
	}
	[TestMethod]
	public void FinalSharedUnit_TwoRoomsCannotBothSpendIt()
	{
		var world = World(new RegionalGatheringPool(Zone, 1, 0, 4, 1, 11, true));
		Assert.IsTrue(world.TryPlan(10, 1, out var a));
		Assert.IsTrue(world.TryPlan(11, 1, out var b));
		Assert.IsTrue(world.TryApply(a));
		Assert.IsFalse(world.TryApply(b));
		Assert.AreEqual(0.0, world.Inspect(12)!.Stock);
	}
	[TestMethod]
	public void Recovery_IsOncePerOwnerRatherThanPerRoom_AndInspectionIsPure()
	{
		var world = World(new RegionalGatheringPool(Zone, 1, 0, 4, 0, 11, true));
		for (var i = 0; i < 100; i++) Assert.AreEqual(0.0, world.Inspect(10)!.Stock);
		world.RecoverHour();
		Assert.AreEqual(.1, world.Inspect(11)!.Stock, .000001);
		world.RecoverHour();
		Assert.AreEqual(.2, world.Inspect(12)!.Stock, .000001);
	}
	[TestMethod]
	public void FractionalRecovery_TenStockHoursProduceOneUsableLife()
	{
		var world = World(new RegionalGatheringPool(Zone, 1, 0, 4, 0, 11, true));
		for (var i = 0; i < 10; i++) world.RecoverHour();
		Assert.AreEqual(1.0, world.Inspect(10)!.Stock);
		Assert.IsTrue(world.TryPlan(10, 1, out var debit));
		Assert.IsTrue(world.TryApply(debit));
		Assert.AreEqual(0.0, world.Inspect(11)!.Stock);
	}
	[TestMethod]
	public void OverlappingAreas_PriorityThenStableId_SelectOneOwnerWithoutDoubleCounting()
	{
		var world = World();
		world.SetBinding(Zone, 1, 0);
		var a = new RegionalGatheringOwner(RegionalGatheringOwnerKind.Area, 7);
		var b = new RegionalGatheringOwner(RegionalGatheringOwnerKind.Area, 8);
		world.SetBinding(a, 1, 1);
		world.SetBinding(b, 1, 1);
		Assert.AreEqual(a, world.Inspect(10)!.Owner);
		Assert.AreEqual(8.0, world.Inspect(10)!.Capacity);
		Assert.AreEqual(3.0, world.Inspect(12)!.Capacity);
		world.SetBinding(b, 1, 2);
		Assert.AreEqual(b, world.Inspect(10)!.Owner);
		Assert.AreEqual(3.0, world.Inspect(10)!.Capacity);
		Assert.AreEqual(5.0, world.Inspect(11)!.Capacity);
		Assert.AreEqual(11.0, world.Pools.Sum(x => x.Capacity));
	}
	[TestMethod]
	public void ExplicitDisabledArea_BlocksZoneInheritanceButKeepsDormantStock()
	{
		var world = World();
		world.SetBinding(Zone, 1, 0);
		var area = new RegionalGatheringOwner(RegionalGatheringOwnerKind.Area, 7);
		world.SetBinding(area, 1, 0);
		Assert.IsTrue(world.TryPlan(10, 1, out var debit));
		Assert.IsTrue(world.TryApply(debit));
		world.SetBinding(area, null, 0);
		Assert.IsNull(world.Inspect(10));
		Assert.IsNull(world.UnavailableReason(10), "An explicit absent regional pool permits configured room fallback.");
		world.SetBinding(area, 1, 0);
		Assert.AreEqual(7.0, world.Inspect(11)!.Stock);
	}
	[TestMethod]
	public void MembershipAndTerrainChanges_InvalidatePlans_ClampButNeverRefill()
	{
		var world = World(new RegionalGatheringPool(Zone, 1, 0, 4, 10, 11, true));
		Assert.IsTrue(world.TryPlan(10, 1, out var old));
		world.Track(new(11, 2, [], 2));
		Assert.IsFalse(world.TryApply(old));
		Assert.AreEqual(6.0, world.Inspect(10)!.Stock);
		world.Track(new(10, 1, [7, 8], 2));
		Assert.AreEqual(8.0, world.Inspect(10)!.Capacity);
		Assert.AreEqual(6.0, world.Inspect(10)!.Stock);
	}
	[TestMethod]
	public void ExplicitDisabledRoom_NeitherContributesNorUsesTheRegion()
	{
		var world = World();
		world.Track(new(10, 1, [7, 8], 1, true));
		world.SetBinding(Zone, 1, 0);
		Assert.IsNull(world.Inspect(10));
		Assert.IsNotNull(world.UnavailableReason(10));
		Assert.AreEqual(8.0, world.Inspect(11)!.Capacity);
	}
	[TestMethod]
	public void MissingProfile_PreservesEvidenceAndRefusesUntilDefinitionIsRepaired()
	{
		var world = World(new RegionalGatheringPool(Zone, 99, 0, 12, 7, 11, true));
		Assert.AreEqual(7.0, world.Pool(Zone)!.Stock);
		Assert.AreEqual(12L, world.Pool(Zone)!.Revision);
		Assert.IsNull(world.Inspect(10));
		Assert.IsNotNull(world.UnavailableReason(10));
		world.SetProfile(new(99, "Repaired", 1, 3, .1, new Dictionary<long, double>()));
		Assert.AreEqual(7.0, world.Inspect(10)!.Stock);
	}
	[TestMethod]
	public void AreaInheritance_RestoresZoneAndRetainsDormantAreaDepletion()
	{
		var world = World();
		world.SetBinding(Zone, 1, 0);
		var area = new RegionalGatheringOwner(RegionalGatheringOwnerKind.Area, 7);
		world.SetBinding(area, 1, 0);
		Assert.IsTrue(world.TryPlan(10, 1, out var debit));
		Assert.IsTrue(world.TryApply(debit));
		world.SetBinding(area, null, 0, inherit: true);
		Assert.AreEqual(Zone, world.Inspect(10)!.Owner);
		Assert.AreEqual(3.0, world.Inspect(10)!.Stock, "New zone capacity must not refill the clamped parent.");
		world.SetBinding(area, 1, 0);
		Assert.AreEqual(7.0, world.Inspect(10)!.Stock);
	}
	[TestMethod]
	public void RegisteredInputs_CannotMutateCapacityOutsideVersionedChanges()
	{
		var terrain = new Dictionary<long, double> { [1] = 3 };
		var areas = new List<long> { 7 };
		var world = new RegionalGatheringRegistry();
		world.Load([new(1, "Land", 1, 0, .1, terrain)], []);
		world.Track(new(10, 1, areas, 1));
		world.CompleteLoading();
		world.SetBinding(new(RegionalGatheringOwnerKind.Area, 7), 1, 0);
		terrain[1] = 100;
		areas.Clear();
		Assert.AreEqual(3.0, world.Inspect(10)!.Capacity);
		Assert.AreEqual(3.0, world.Profiles.Single().Contribution(1));
	}
	[TestMethod]
	public void ProfileRoundTrip_PreservesWeightsAndRejectsUnversionedDefinitions()
	{
		var original = new RegionalGatheringProfile(1, "Mixed", 4, 0, .1, new Dictionary<long, double> { [5] = 3, [2] = 5 });
		var parsed = RegionalGatheringProfile.Parse(1, original.Name, 4, original.Definition());
		Assert.AreEqual(3.0, parsed.Contribution(5));
		Assert.AreEqual(5.0, parsed.Contribution(2));
		Assert.AreEqual(0.0, parsed.Contribution(3));
		Assert.ThrowsException<FormatException>(() => RegionalGatheringProfile.Parse(1, "Bad", 1, "<RegionalGathering version='2' contribution='3' recovery='.1'/>"));
	}
}
