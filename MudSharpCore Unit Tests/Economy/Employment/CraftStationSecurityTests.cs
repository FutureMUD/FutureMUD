#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Work.Crafts;

namespace MudSharp_Unit_Tests.Economy.Employment;

public partial class UnifiedEmploymentDispatchTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void EmploymentCraftReservations_SavedStationMovedOrReplaced_BlocksNativeStart(bool replaced)
	{
		var (actor, station, craft, context, _) = StationCraftFixture("station", 1);
		actor.Setup(x => x.TargetLocalOrHeldItem("forge")).Returns(replaced ? Item(999, "replacement forge").Object : null);

		Assert.IsFalse(context.CanStartCraft("test craft", actor.Object, out var reason));
		StringAssert.Contains(reason, "no longer accessible");
		Assert.IsFalse(context.TryStartCraft(actor.Object, "test craft", out reason, out _));
		StringAssert.Contains(reason, "no longer accessible");
		craft.Verify(x => x.CreateResourceReservation(actor.Object, It.IsAny<IActiveCraftGameItemComponent>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		craft.Verify(x => x.BeginCraft(actor.Object), Times.Never);
		station.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	[DataRow("station", 1, false)]
	[DataRow("input", 1, true)]
	[DataRow("input", 2, false)]
	[DataRow("tool", 1, true)]
	[DataRow("tool", 2, false)]
	public void EmploymentCraftReservations_NativeCraftRefresh_LocksOnlyCurrentResources(string resourceKind, int phase, bool preventsPickup)
	{
		var (actor, station, _, context, effects) = StationCraftFixture(resourceKind, phase);
		if (!preventsPickup)
		{
			SetupCraftReservationEffects(station, [new EmploymentCraftReservationEffect(station.Object,
				Guid.NewGuid(), Guid.NewGuid(), "other task", "shared station", DateTimeOffset.UtcNow.AddMinutes(10))]);
		}

		Assert.IsTrue(context.TryStartCraft(actor.Object, "test craft", out var reason, out _), reason);

		Assert.AreEqual(1, effects.Count);
		Assert.AreEqual(preventsPickup, effects.Single() is INoGetEffect);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void EmploymentCraftReservations_Morph_PreservesStationOrResourcePickupPolicy(bool resource)
	{
		var item = Item(601, "old item").Object;
		var morphed = Item(602, "new item").Object;
		EmploymentCraftReservationEffect effect = resource
			? new EmploymentCraftResourceReservationEffect(item, Guid.NewGuid(), Guid.NewGuid(), "task", "input", DateTimeOffset.UtcNow.AddMinutes(5))
			: new EmploymentCraftReservationEffect(item, Guid.NewGuid(), Guid.NewGuid(), "task", "station", DateTimeOffset.UtcNow.AddMinutes(5));

		var next = (EmploymentCraftReservationEffect)effect.NewEffectOnItemMorph(item, morphed)!;

		Assert.AreSame(morphed, next.Owner);
		Assert.AreEqual(effect.CorrelationId, next.CorrelationId);
		Assert.AreEqual(resource, next is INoGetEffect);
		Assert.AreEqual(resource, next.PreventsItemFromMerging(morphed, item));
	}

	private static (Mock<ICharacter> Actor, Mock<IGameItem> Station, Mock<ICraft> Craft,
		EmploymentTaskContext Context, List<EmploymentCraftReservationEffect> Effects) StationCraftFixture(string resourceKind, int phase)
	{
		var currency = Currency();
		var actor = Character(1, "Crafter");
		var room = Room(10, "workshop").Object;
		var station = Item(301, "forge");
		var craft = new Mock<ICraft>();
		craft.SetupGet(x => x.Id).Returns(42L);
		craft.SetupGet(x => x.Name).Returns("test craft");
		craft.SetupGet(x => x.RevisionNumber).Returns(3);
		craft.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		craft.Setup(x => x.AppearInCraftsList(actor.Object)).Returns(true);
		var crafts = new RevisableAll<ICraft>();
		crafts.Add(craft.Object);
		var world = Gameworld(currency.Object, new Dictionary<long, ICharacter>(), [room], new Dictionary<long, IGameItem> { [301] = station.Object });
		world.SetupGet(x => x.Crafts).Returns(crafts);
		world.Setup(x => x.GetStaticDouble("EmploymentCraftStationReservationCapacity")).Returns(2.0);
		world.Setup(x => x.GetStaticDouble("EmploymentCraftReservationDurationMinutes")).Returns(7.0);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Location).Returns(room);
		actor.Setup(x => x.TargetLocalOrHeldItem("forge")).Returns(station.Object);
		station.SetupGet(x => x.Gameworld).Returns(world.Object);
		SetupCraftReservationEffects(station, []);
		var effects = new List<EmploymentCraftReservationEffect>();
		station.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => effects.Add((EmploymentCraftReservationEffect)effect));
		var component = new Mock<IActiveCraftGameItemComponent>();
		component.SetupGet(x => x.Craft).Returns(craft.Object);
		component.SetupGet(x => x.Parent).Returns(Item(9001, "active craft").Object);
		component.SetupGet(x => x.Phase).Returns(phase);
		var active = Mock.Of<IActiveCraftEffect>(x => x.Component == component.Object);
		actor.Setup(x => x.EffectsOfType<IActiveCraftEffect>(It.IsAny<Predicate<IActiveCraftEffect>>())).Returns([active]);
		craft.Setup(x => x.CreateResourceReservation(actor.Object, component.Object, phase, It.IsAny<int>()))
			.Returns((true, string.Empty, new CraftResourceReservation(42, 3, "test craft", 1, 1,
				resourceKind == "input" ? [new CraftInputReservation(501, "input", "simple", 301, "GameItem", "forge", 1, [301L])] : [],
				resourceKind == "tool" ? [new CraftToolReservation(502, "tool", "simple", 301, "forge", 1)] : [])));
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		host.Hire(actor.Object, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageCraftRules), null);
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("station craft", new EmploymentActionPlan([new CraftStationActionStep("forge"), new CraftTriggerActionStep("test craft")]), actor.Object);
		task.MarkStep(0, EmploymentActionStepStatus.Completed, new EmploymentActionStepOperationalState(CraftJobReference: JsonSerializer.Serialize(new
		{
			Version = "craft-station-v1", Selector = "forge", RoomId = 10L, ItemId = 301L, Description = "forge",
			ReservedAt = DateTimeOffset.UtcNow, ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(7)
		})));
		var context = new EmploymentTaskContext(host);
		context.HydrateTaskState(task, 1);
		return (actor, station, craft, context, effects);
	}
}
