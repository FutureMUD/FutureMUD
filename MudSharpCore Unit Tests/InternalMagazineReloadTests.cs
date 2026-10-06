#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Combat;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class InternalMagazineReloadTests
{
	[DataTestMethod]
	[DataRow("chamber")]
	[DataRow("magazine")]
	[DataRow("casing")]
	[DataRow("chamber-claimed")]
	[DataRow("magazine-claimed")]
	[DataRow("casing-claimed")]
	[DataRow("missing")]
	public void NativeInternalMagazineLoad_RestoresExactChildCustodyWithoutGameplayCallbacks(string slot)
	{
		var claimed = slot.EndsWith("-claimed", StringComparison.Ordinal);
		slot = slot.Split('-')[0];
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var parent = new Mock<IGameItem>(); parent.SetupGet(x => x.Gameworld).Returns(world.Object);
		var foreign = claimed ? Mock.Of<IGameItem>() : null;
		var child = new Mock<IGameItem>(); child.SetupProperty(x => x.ContainedIn, foreign);
		child.Setup(x => x.LoadTimeSetContainedIn(parent.Object)).Callback(() => child.Object.ContainedIn = parent.Object);
		var ammo = new Mock<IAmmo>(); ammo.SetupGet(x => x.Parent).Returns(child.Object);
		child.Setup(x => x.GetItemType<IAmmo>()).Returns(ammo.Object);
		world.Setup(x => x.TryGetItem(It.IsAny<long>(), true)).Returns<long, bool>((id, _) => id == 11 ? child.Object : null!);
		var proto = TestObjectFactory.CreateUninitialized<InternalMagazineGunGameItemComponentProto>();
		typeof(FirearmBaseGameItemComponentProto).GetField("<FireModes>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(proto, new List<FirearmFireMode> { new(FirearmFireModeType.Single, 1, 0, 0, 0) });
		proto.CycleType = FirearmCycleType.Manual;
		var xml = new XElement("Definition", new XElement("ChamberedRound", slot == "chamber" ? 11 : 0),
			new XElement("ChamberedCasing", slot == "casing" ? 11 : 0),
			new XElement("RoundsInMagazine", slot is "magazine" or "missing" ? new XElement("Round", slot == "missing" ? 999 : 11) : null));
		var gun = new InternalMagazineGunGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 1, Definition = xml.ToString() }, proto, parent.Object);
		Assert.AreSame(slot == "chamber" && !claimed ? ammo.Object : null, gun.ChamberedRound);
		Assert.AreSame(slot == "casing" && !claimed ? child.Object : null, gun.ChamberedCasing);
		CollectionAssert.AreEqual(slot == "magazine" && !claimed ? new[] { child.Object } : Array.Empty<IGameItem>(), gun.MagazineContents.ToArray());
		Assert.AreSame(claimed ? foreign : slot == "missing" ? null : parent.Object, child.Object.ContainedIn);
		child.Verify(x => x.LoadTimeSetContainedIn(parent.Object), slot == "missing" || claimed ? Times.Never : Times.Once);
		child.VerifySet(x => x.Changed = It.IsAny<bool>(), Times.Never);
		parent.VerifySet(x => x.Changed = It.IsAny<bool>(), Times.Never);
	}
}

public partial class QueuedCommandAuthorityTests
{
	[DataTestMethod]
	[DataRow("unknown-held")]
	[DataRow("held-floor")]
	[DataRow("held-valid")]
	[DataRow("output-rebind")]
	[DataRow("capacity-changed")]
	[DataRow("take-refill")]
	[DataRow("take-throw")]
	public void NativeInternalMagazineLoad_RequiresSuccessfulPlanAndExactHeldCustody(string scenario)
	{
		var f = new Fixture();
		f.Item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var hand = Mock.Of<IGrab>(); f.Body.SetupGet(x => x.HoldLocs).Returns([hand]);
		f.Body.Setup(x => x.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		f.Actor.Setup(x => x.CanManipulateItem(f.Item.Object)).Returns((true, string.Empty));
		f.World.SetupGet(x => x.SaveManager).Returns(Mock.Of<MudSharp.Framework.Save.ISaveManager>());
		Mock.Get(f.Actor.Object.OutputHandler).SetupGet(x => x.Perceiver).Returns(f.Actor.Object);
		Mock.Get(f.Actor.Object.Location).Setup(x => x.LayerCharacters(It.IsAny<MudSharp.Construction.RoomLayer>())).Returns([f.Actor.Object]);
		var round = new Mock<IGameItem>(); round.SetupProperty(x => x.ContainedIn, null);
		var quantity = 1; round.SetupGet(x => x.Quantity).Returns(() => quantity);
		round.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(x => ReferenceEquals(x, round.Object));
		var floor = scenario == "held-floor"; var rebound = false;
		var hold = new Mock<IHoldable>(); hold.SetupProperty(x => x.HeldBy, floor ? null : f.Body.Object);
		round.Setup(x => x.GetItemType<IHoldable>()).Returns(hold.Object);
		round.SetupGet(x => x.InInventoryOf).Returns(() => hold.Object.HeldBy);
		round.SetupGet(x => x.Location).Returns(() => floor || rebound ? f.Actor.Object.Location : null);
		var held = new List<IGameItem>(); if (!floor) held.Add(round.Object);
		var floorItems = floor ? new List<IGameItem> { round.Object } : new List<IGameItem>();
		Mock.Get(f.Actor.Object.Location).SetupGet(x => x.GameItems).Returns(floorItems);
		f.Body.SetupGet(x => x.HeldItems).Returns(held);
		var expectedError = new InvalidOperationException("one-shot detached callback");
		f.Body.Setup(x => x.Take(round.Object)).Callback(() =>
		{
			held.Remove(round.Object); hold.Object.HeldBy = null;
			if (scenario == "take-refill") quantity = 3;
			if (scenario == "take-throw") throw expectedError;
		});
		round.Setup(x => x.Drop(f.Actor.Object.Location)).Callback(() => { rebound = true; floorItems.Add(round.Object); }).Returns(round.Object);
		var ammo = new Mock<IAmmo>(); ammo.SetupGet(x => x.Parent).Returns(round.Object);
		round.Setup(x => x.GetItemType<IAmmo>()).Returns(ammo.Object);
		var plan = new Mock<IInventoryPlan>(); plan.Setup(x => x.PlanIsFeasible()).Returns(InventoryPlanFeasibility.Feasible);
		plan.Setup(x => x.ExecuteWholePlan()).Returns(new[] { new InventoryPlanActionResult
		{ OriginalReference = "loaditem", PrimaryTarget = round.Object,
			ActionState = scenario == "unknown-held" ? DesiredItemState.Unknown : DesiredItemState.Held } });
		var template = new Mock<IInventoryPlanTemplate>(); template.Setup(x => x.CreatePlan(f.Actor.Object)).Returns(plan.Object);
		var proto = TestObjectFactory.CreateUninitialized<InternalMagazineGunGameItemComponentProto>();
		proto.InternalMagazineCapacity = 2; proto.LoadEmote = "@ insert|inserts $2 into $1.";
		proto.LoadTemplate = proto.LoadTemplateIgnoreEmpty = template.Object;
		var gun = new InternalMagazineGunGameItemComponent(proto, f.Item.Object, temporary: true); gun.SetNoSave(false);
		if (scenario == "output-rebind")
			Mock.Get(f.Actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback(() => { rebound = true; floorItems.Add(round.Object); }).Returns(true);
		if (scenario == "capacity-changed") plan.Setup(x => x.ExecuteWholePlan()).Callback(() => proto.InternalMagazineCapacity = 0)
			.Returns(new[] { new InventoryPlanActionResult { OriginalReference = "loaditem", PrimaryTarget = round.Object, ActionState = DesiredItemState.Held } });
		if (scenario == "take-throw") Assert.AreSame(expectedError, Assert.ThrowsException<InvalidOperationException>(() => gun.Load(f.Actor.Object)));
		else gun.Load(f.Actor.Object);
		var accepted = scenario == "held-valid";
		var detached = accepted || scenario is "output-rebind" or "take-refill" or "take-throw";
		f.Body.Verify(x => x.Take(round.Object), detached ? Times.Once : Times.Never);
		CollectionAssert.AreEqual(accepted ? new[] { round.Object } : Array.Empty<IGameItem>(), gun.MagazineContents.ToArray());
		Assert.AreSame(accepted ? f.Item.Object : null, round.Object.ContainedIn);
		Assert.AreSame(detached || floor ? null : f.Body.Object, hold.Object.HeldBy);
		Assert.AreEqual(accepted, gun.Changed);
		Assert.AreEqual(floor || (!accepted && detached), floorItems.Contains(round.Object));
		Assert.AreEqual(scenario == "take-refill" ? 3 : 1, round.Object.Quantity);
		plan.Verify(x => x.ExecuteWholePlan(), Times.Once);
		if (detached) plan.Verify(x => x.FinalisePlanWithExemptions(It.Is<IList<IGameItem>>(items => items.Count == 1 && ReferenceEquals(items[0], round.Object))), Times.Once);
		round.Verify(x => x.Delete(), Times.Never);
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("expire")]
	[DataRow("replace")]
	public void NativeInternalMagazineUnready_OutputCallbackPreservesExactSlotAndCommitsOnlyAdmittedDetach(string change)
	{
		var f = new Fixture();
		f.Item.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var hand = Mock.Of<IGrab>(); f.Body.SetupGet(x => x.HoldLocs).Returns([hand]);
		f.Body.Setup(x => x.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		f.Actor.Setup(x => x.CanManipulateItem(f.Item.Object)).Returns((true, string.Empty));
		var proto = TestObjectFactory.CreateUninitialized<InternalMagazineGunGameItemComponentProto>();
		proto.UnreadyEmote = "@ unready|unreadies $1.";
		var gun = new InternalMagazineGunGameItemComponent(proto, f.Item.Object, temporary: true);
		f.World.SetupGet(x => x.SaveManager).Returns(Mock.Of<MudSharp.Framework.Save.ISaveManager>());
		gun.SetNoSave(false);
		Mock.Get(f.Actor.Object.OutputHandler).SetupGet(x => x.Perceiver).Returns(f.Actor.Object);
		Mock.Get(f.Actor.Object.Location).Setup(x => x.LayerCharacters(It.IsAny<MudSharp.Construction.RoomLayer>())).Returns([f.Actor.Object]);
		var item = new Mock<IGameItem>(); item.SetupProperty(x => x.ContainedIn, f.Item.Object);
		var hold = new Mock<IHoldable>(); hold.SetupProperty(x => x.HeldBy, null);
		item.Setup(x => x.GetItemType<IHoldable>()).Returns(hold.Object);
		item.SetupGet(x => x.InInventoryOf).Returns(() => hold.Object.HeldBy);
		var ammo = new Mock<IAmmo>(); ammo.SetupGet(x => x.Parent).Returns(item.Object); gun.ChamberedRound = ammo.Object;
		var replacement = Mock.Of<IAmmo>();
		f.Body.Setup(x => x.CanGet(item.Object, 0)).Returns(true);
		f.Body.Setup(x => x.Get(item.Object, 0, It.IsAny<MudSharp.PerceptionEngine.IEmote>(), It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()))
			.Callback(() => { Assert.IsNull(gun.ChamberedRound); Assert.IsTrue(gun.Changed); hold.Object.HeldBy = f.Body.Object; });
		Mock.Get(f.Actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<MudSharp.PerceptionEngine.IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback(() => { if (change == "expire") f.Grant = null; if (change == "replace") gun.ChamberedRound = replacement; }).Returns(true);
		using var execution = CommandExecutionScope.EnterDispatch(CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "unready gun", () => true), f.Actor.Object);
		Assert.AreEqual(change == "valid", gun.Unready(f.Actor.Object));
		Assert.AreSame(change == "valid" ? null : change == "replace" ? replacement : ammo.Object, gun.ChamberedRound);
		Assert.AreSame(change == "valid" ? null : f.Item.Object, item.Object.ContainedIn);
		Assert.AreSame(change == "valid" ? f.Body.Object : null, hold.Object.HeldBy);
		f.Body.Verify(x => x.Get(item.Object, 0, It.IsAny<MudSharp.PerceptionEngine.IEmote>(), It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()), change == "valid" ? Times.Once : Times.Never);
	}
}
