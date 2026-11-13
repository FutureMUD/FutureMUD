#nullable enable

using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Performances;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PerformanceReturnsTests
{
	[TestMethod]
	public void CleanupPolicies_DefaultForOlderWorksAndSurviveRevision()
	{
		var work = JsonSerializer.Deserialize<PerformanceWork>("""{"Roles":[{"Key":"bearer"}],"Props":["lantern"]}""")!;
		Assert.IsTrue(work.Roles[0].ReturnAfterPerformance);
		Assert.IsEmpty(work.LeaveProps);
		work.Roles[0].ReturnAfterPerformance = false;
		work.LeaveProps.Add("lantern");
		var copy = PerformanceAuthoring.CloneForEdit(work, 1, true);
		Assert.IsFalse(copy.Roles[0].ReturnAfterPerformance);
		CollectionAssert.AreEqual(work.LeaveProps, copy.LeaveProps);
		copy.LeaveProps.Clear();
		Assert.HasCount(1, work.LeaveProps);
	}

	[TestMethod]
	public void Restored_RequiresExactContainerHolderAndLayer()
	{
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(7);
		var chest = new Mock<IGameItem>();
		chest.SetupGet(x => x.Id).Returns(8);
		chest.SetupGet(x => x.Location).Returns(room.Object);
		chest.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(other, chest.Object));
		room.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([chest.Object]);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.ContainedIn).Returns(chest.Object);
		var source = new PropReturn("lantern", 9, new(7, RoomLayer.GroundLevel), 8, null);
		Assert.IsTrue(PerformanceService.Restored(source, item.Object));
		Assert.IsFalse(PerformanceService.Restored(source with { Container = 10 }, item.Object));
		Assert.IsFalse(PerformanceService.Restored(source with { Origin = new(7, RoomLayer.InTrees) }, item.Object));
		var body = new Mock<IBody>();
		var holder = new Mock<ICharacter>();
		holder.SetupGet(x => x.Id).Returns(11);
		body.SetupGet(x => x.Actor).Returns(holder.Object);
		chest.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		Assert.IsFalse(PerformanceService.Restored(source, item.Object));
		Assert.IsTrue(PerformanceService.Restored(source with { Lender = new(11, null) }, item.Object));
	}

	[TestMethod]
	public void PendingReturn_RoundTripsWithOriginalDeadlineAndTransfers()
	{
		var lender = new CastMember(15, 7);
		var home = new PerformanceMark(21, RoomLayer.InTrees);
		var plan = new PerformancePlan
		{
			State = PerformanceState.Completed,
			LastOutcome = "Final cue completed.",
			CleanupDueUtc = new System.DateTime(2200, 1, 1, 0, 0, 0, System.DateTimeKind.Utc),
			PerformerReturns = [new(lender, "bearer", home, true)],
			PropReturns = [new("lantern", 42, home, 87, lender) { Custodian = new(19, 9), TransferTo = lender }]
		};
		var loaded = JsonSerializer.Deserialize<PerformancePlan>(JsonSerializer.Serialize(plan))!;
		Assert.IsFalse(loaded.Active);
		Assert.AreEqual(plan.LastOutcome, loaded.LastOutcome);
		Assert.AreEqual(plan.CleanupDueUtc, loaded.CleanupDueUtc);
		CollectionAssert.AreEqual(plan.PerformerReturns, loaded.PerformerReturns);
		CollectionAssert.AreEqual(plan.PropReturns, loaded.PropReturns);
	}

	[TestMethod]
	public void Restore_MovedContainerAndCyclicContainmentAreNeverAValidDestination()
	{
		var chest = new Mock<IGameItem>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.ContainedIn).Returns(chest.Object);
		chest.SetupGet(x => x.Id).Returns(8);
		var source = new PropReturn("book", 9, new(7, RoomLayer.GroundLevel), 8, null);
		Assert.IsFalse(PerformanceService.Restored(source, item.Object));
		chest.SetupGet(x => x.ContainedIn).Returns(item.Object);
		Assert.IsFalse(PerformanceService.Restored(source, item.Object));
		chest.VerifyGet(x => x.InInventoryOf, Times.Once);
	}

	[TestMethod]
	public void ClosedOriginalContainer_WaitsThenExpiresWithoutDroppingOrForcingItOpen()
	{
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(7);
		var carrier = new Mock<ICharacter>();
		carrier.SetupGet(x => x.Location).Returns(room.Object);
		var chest = new Mock<IGameItem>();
		chest.SetupGet(x => x.Id).Returns(8);
		var lid = new Mock<IOpenable>();
		chest.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(other, chest.Object));
		chest.Setup(x => x.GetItemType<IOpenable>()).Returns(lid.Object);
		room.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([chest.Object]);
		var world = new Mock<IFuturemud>();
		var items = new All<IGameItem>();
		items.Add(chest.Object);
		world.SetupGet(x => x.Items).Returns(items);
		var service = new PerformanceService(world.Object);
		var plan = new PerformancePlan();
		var source = new PropReturn("book", 9, new(7, RoomLayer.GroundLevel), 8, null);
		var book = Mock.Of<IGameItem>();
		Assert.IsFalse(service.TryReturnProp(plan, source, book, carrier.Object, false, out var waiting));
		StringAssert.Contains(waiting, "original container");
		Assert.IsTrue(service.TryReturnProp(plan, source, book, carrier.Object, true, out var expired));
		StringAssert.Contains(expired, "in person");
		carrier.VerifyGet(x => x.Body, Times.Never);
		lid.VerifyGet(x => x.IsOpen, Times.Once);
		lid.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void PersonalProp_GivenBackToOriginalNpcOnlyAfterNativeHandoffSucceeds()
	{
		var room = new Mock<IRoom>();
		room.SetupGet(r => r.Id).Returns(4);
		var carrier = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		var lender = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		lender.SetupGet(x => x.Id).Returns(22);
		lender.SetupGet(x => x.Identity.Id).Returns(22);
		lender.SetupGet(x => x.Body.Actor).Returns(lender.Object);
		carrier.SetupGet(x => x.Body.Actor).Returns(carrier.Object);
		carrier.SetupGet(x => x.Location).Returns(room.Object);
		lender.SetupGet(x => x.Location).Returns(room.Object);
		carrier.SetupGet(x => x.Movement).Returns((MudSharp.Movement.IMovement)null!);
		carrier.Setup(x => x.ColocatedWith(lender.Object)).Returns(true);
		carrier.Setup(x => x.CanSee(lender.Object, PerceiveIgnoreFlags.None)).Returns(true);
		var lantern = new Mock<IGameItem>();
		IBody holder = carrier.Object.Body;
		lantern.SetupGet(x => x.InInventoryOf).Returns(() => holder);
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Actors).Returns(new All<ICharacter> { lender.Object });
		var plan = new PerformancePlan();
		var claim = CastMember.From(lender.Object);
		var loan = new PropReturn("lantern", 12, new(5, RoomLayer.GroundLevel), null, claim);
		plan.PerformerReturns.Add(new(claim, "bearer", new(4, RoomLayer.GroundLevel), true));
		var service = new PerformanceService(world.Object);
		Assert.IsFalse(service.TryReturnProp(plan, loan, lantern.Object, carrier.Object, false, out _));
		Assert.AreSame(carrier.Object.Body, holder);
		var hands = Mock.Get(carrier.Object.Body);
		hands.Setup(x => x.CanGive(lantern.Object, lender.Object.Body, 0)).Returns(true);
		hands.Setup(x => x.Give(lantern.Object, lender.Object.Body, 0, null)).Callback(() => holder = lender.Object.Body);
		Assert.IsTrue(service.TryReturnProp(plan, loan, lantern.Object, carrier.Object, false, out var report));
		StringAssert.Contains(report, "returned");
		Assert.AreSame(lender.Object.Body, holder);
	}

	[TestMethod]
	public void SameRoomReturnToHigherLayer_UsesNativeClimbing()
	{
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.Setup(x => x.CanClimbUp()).Returns((true, ""));
		Assert.IsTrue(FollowingPath.PathTowardsLayer(actor.Object, RoomLayer.InTrees));
		actor.Verify(x => x.ClimbUp(null), Times.Once);
	}
}
