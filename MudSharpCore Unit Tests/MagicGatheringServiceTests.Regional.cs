#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Gathering;

namespace MudSharp_Unit_Tests;

public partial class MagicGatheringServiceTests
{
	[TestMethod]
	public void RegionalLand_PaysOneSharedUnitBeforeLocalSourcesAndPersistsBeforeCredit()
	{
		using var regional = new RegionalTestService();
		using var fixture = new GatheringFixture(MagicGatheringMethodKind.Land, regionalGathering: regional);
		regional.Attach(fixture.Room);
		fixture.World.Edit("output 1 rate 0");
		fixture.SetSourceBalance(20);
		fixture.UpdateMethod(x => x with { RegionalLifeUnitsPerAction = 1 });
		var start = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5);
		Assert.IsTrue(start.Success, start.Message);
		Assert.IsNotNull(start.Quote!.RegionalLifeDebit);
		Assert.AreEqual(0, start.Quote.LandSources.Count);
		Assert.AreEqual(2.0, regional.Inspect(fixture.Room)!.Stock, "Preview/start cannot spend stock.");
		fixture.Advance(Duration);
		var result = fixture.Service.Complete(fixture.Actor.Object, start.OperationId!.Value);
		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(1.0, regional.Inspect(fixture.Room)!.Stock);
		Assert.AreEqual(20.0, fixture.SourceBalance, .000001, "Regional payment must not debit a configured local source.");
		Assert.AreEqual(5.0, fixture.DestinationBalance);
		Assert.AreEqual(2, fixture.SourceCheckpointCalls, "Shared payment has a durable checkpoint before the ecology checkpoint.");
		Assert.AreEqual(1.0, fixture.Service.LandDetails(start.OperationId.Value)!["paid:region:zone:1"]);
	}
	[TestMethod]
	public void RegionalLand_ExhaustionUsesCapturedLocalFallbackEvenAfterRegionalRecovery()
	{
		using var regional = new RegionalTestService(0);
		using var fixture = new GatheringFixture(MagicGatheringMethodKind.Land, regionalGathering: regional);
		regional.Attach(fixture.Room);
		fixture.World.Edit("output 1 rate 0");
		fixture.SetSourceBalance(20);
		fixture.UpdateMethod(x => x with { RegionalLifeUnitsPerAction = 1 });
		var start = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5);
		Assert.IsTrue(start.Success, start.Message);
		Assert.IsNull(start.Quote!.RegionalLifeDebit);
		for (var i = 0; i < 10; i++) regional.Registry.RecoverHour();
		fixture.Advance(Duration);
		var result = fixture.Service.Complete(fixture.Actor.Object, start.OperationId!.Value);
		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(15.0, fixture.SourceBalance, .000001);
		Assert.AreEqual(1.0, regional.Inspect(fixture.Room)!.Stock, .000001);
	}
	[TestMethod]
	public void RegionalLand_ChangedSharedPoolCancelsBeforePaymentOrCredit()
	{
		using var regional = new RegionalTestService();
		using var fixture = new GatheringFixture(MagicGatheringMethodKind.Land, regionalGathering: regional);
		regional.Attach(fixture.Room);
		fixture.UpdateMethod(x => x with { RegionalLifeUnitsPerAction = 1 });
		var start = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5);
		Assert.IsTrue(start.Success, start.Message);
		Assert.IsTrue(regional.TryApply(fixture.Room, start.Quote!.RegionalLifeDebit!));
		fixture.Advance(Duration);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, start.OperationId!.Value).Success);
		Assert.AreEqual(0.0, fixture.DestinationBalance);
		Assert.AreEqual(0, fixture.World.Operations.Receipts.Count);
	}
	[TestMethod]
	public void RegionalLand_UnresolvedParticipantQuarantinesTheSharedOwnerAcrossRooms()
	{
		using var regional = new RegionalTestService();
		using var fixture = new GatheringFixture(MagicGatheringMethodKind.Land, regionalGathering: regional);
		regional.Attach(fixture.Room);
		fixture.UpdateMethod(x => x with { RegionalLifeUnitsPerAction = 1 });
		var start = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1);
		Assert.IsTrue(start.Success, start.Message);
		fixture.Advance(Duration);
		Assert.IsTrue(fixture.Service.Complete(fixture.Actor.Object, start.OperationId!.Value).Success);
		fixture.Store.Record(fixture.Store.Operation(start.OperationId.Value)! with { Status = "NeedsReview" });
		using var other = new GatheringFixture(fixture, MagicGatheringMethodKind.Land);
		other.MoveActorAway();
		regional.RoomChanged(other.Actor.Object.Location!);
		other.SetLandSources(new MagicLandSourceDefinition(Guid.NewGuid(), "ambient:1", 1));
		other.UpdateMethod(x => x with { RegionalLifeUnitsPerAction = 1 });
		Assert.IsFalse(other.Service.Preview(other.Actor.Object, other.Capability.Object, "draw", 1).Success);
		Assert.AreEqual(1.0, regional.Inspect(other.Actor.Object.Location!)!.Stock);
		Assert.AreEqual(0.0, other.DestinationBalance);
	}
	[TestMethod]
	public void RegionalLand_NoRoomProfileIsRequiredButExplicitDisabledRoomIsRespected()
	{
		using var regional = new RegionalTestService();
		using var fixture = new GatheringFixture(MagicGatheringMethodKind.Land, regionalGathering: regional);
		regional.Attach(fixture.Room);
		fixture.World.Terrain.SetEnvironmentalMagicProfile(null);
		fixture.World.Coordinator.RoomTerrainChanged(fixture.Room);
		fixture.UpdateMethod(x => x with { RegionalLifeUnitsPerAction = 1 });
		var start = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5);
		Assert.IsTrue(start.Success, start.Message);
		fixture.Advance(Duration);
		var result = fixture.Service.Complete(fixture.Actor.Object, start.OperationId!.Value);
		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(5.0, fixture.DestinationBalance);
		Assert.AreEqual(5.0, fixture.World.Coordinator.InspectState(fixture.Room).State.ScarDamage);
		fixture.World.Coordinator.SetBinding(fixture.Room, EnvironmentalMagicBindingMode.Disabled, null);
		Assert.IsFalse(fixture.Service.Preview(fixture.Actor.Object, fixture.Capability.Object, "draw", 1).Success);
	}

	private sealed class RegionalTestService(double stock = 2) : IRegionalGatheringService
	{
		public RegionalGatheringRegistry Registry { get; } = new();
		public IReadOnlyCollection<RegionalGatheringProfile> Profiles => Registry.Profiles;
		public IReadOnlyCollection<RegionalGatheringPool> Pools => Registry.Pools;
		public void Initialise() => Registry.Load([new(1, "Regional test", 1, 3, .1, new Dictionary<long, double>())],
			[new(new(RegionalGatheringOwnerKind.Zone, 1), 1, 0, 1, stock, 3, true)]);
		public void Attach(IRoom room)
		{
			Registry.Track(new(room.Id, 1, [], 1));
			Registry.CompleteLoading();
		}
		public RegionalGatheringSnapshot? Inspect(IRoom room) => Registry.Inspect(room.Id);
		public string? UnavailableReason(IRoom room) => Registry.UnavailableReason(room.Id);
		public bool TryPlan(IRoom room, double units, out RegionalGatheringDebit plan) => Registry.TryPlan(room.Id, units, out plan);
		public bool TryApply(IRoom room, RegionalGatheringDebit plan) => Registry.TryApply(plan);
		public void RoomChanged(IRoom room) { if (room.Id > 0) Registry.Track(new(room.Id, 1, [], 1, room is Room concrete && concrete.EnvironmentBindingMode == EnvironmentalMagicBindingMode.Disabled)); }
		public void Unregister(IRoom room) => Registry.Untrack(room.Id);
		public void SetBinding(RegionalGatheringOwner owner, long? profileId, int priority, bool inherit = false) => Registry.SetBinding(owner, profileId, priority, inherit);
		public void SetProfile(RegionalGatheringProfile profile) => Registry.SetProfile(profile);
		public RegionalGatheringProfile CreateProfile(string name, double contribution, double recovery) => throw new NotSupportedException();
		public void StageSave(RegionalGatheringOwner owner) { }
		public void RecoverSaveFailure(RegionalGatheringOwner owner) { }
		public void Dispose() { }
	}
}
