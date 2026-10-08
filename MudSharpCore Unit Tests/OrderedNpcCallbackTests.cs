#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Commands;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Form.Material;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class QueuedCommandAuthorityTests
{
	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("reacquired")]
	[DataRow("relocated")]
	[DataRow("expire")]
	[DataRow("after-claim")]
	public void NativeContainerPut_ExtractedMergeClearsOnlyAcceptedSourceAndPreservesCallbackClaim(string change)
	{
		var f = new Fixture();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Gameworld).Returns(world.Object);
		prototype.SetupGet(x => x.Components).Returns([]);
		var item = new GameItem(prototype.Object);
		var source = new Mock<MudSharp.Construction.IRoom>();
		var sourceItems = new List<IGameItem>();
		source.SetupGet(x => x.GameItems).Returns(sourceItems);
		var locationProperty = typeof(GameItem).GetProperty(nameof(GameItem.Location))!;
		locationProperty.SetValue(item, source.Object);
		var otherRoom = Mock.Of<MudSharp.Construction.IRoom>();
		var foreign = Mock.Of<IGameItem>();
		var destination = new Mock<IGameItem>();
		destination.SetupGet(x => x.Gameworld).Returns(world.Object);
		var survivor = new Mock<IGameItem>();
		survivor.SetupGet(x => x.ContainedIn).Returns(destination.Object);
		survivor.Setup(x => x.CanMerge(item)).Callback(() =>
		{
			if (change == "reacquired") sourceItems.Add(item);
			if (change == "relocated") locationProperty.SetValue(item, otherRoom);
			if (change == "expire") f.Grant = null;
		}).Returns(true);
		survivor.Setup(x => x.Merge(item)).Callback(() =>
		{
			Assert.IsNull(item.DirectLocation, "The accepted merge must publish detached source custody.");
			if (change == "after-claim") item.ContainedIn = foreign;
		});
		var container = TestObjectFactory.CreateUninitialized<ContainerGameItemComponent>();
		var contents = new List<IGameItem> { survivor.Object };
		Set(container, "_contents", contents);
		typeof(ContainerGameItemComponent).GetProperty("Parent")!.SetValue(container, destination.Object);
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "put goods", () => true), f.Actor.Object);
		container.Put(f.Actor.Object, item);
		var accepted = change is "valid" or "after-claim";
		survivor.Verify(x => x.Merge(item), accepted ? Times.Once : Times.Never);
		CollectionAssert.AreEqual(new[] { survivor.Object }, contents.ToArray());
		Assert.AreEqual(change == "valid", item.Deleted);
		Assert.AreSame(change == "after-claim" ? foreign : null, item.ContainedIn);
		Assert.AreSame(accepted ? null : change == "relocated" ? otherRoom : source.Object, item.DirectLocation);
		Assert.AreEqual(change == "reacquired", sourceItems.Contains(item));
	}

	[DataTestMethod]
	[DataRow("valid", false)]
	[DataRow("valid", true)]
	[DataRow("ordinary", false)]
	[DataRow("held-destination", false)]
	[DataRow("still-member", false)]
	[DataRow("reacquired", false)]
	[DataRow("relocated", false)]
	[DataRow("expire", false)]
	[DataRow("holder", false)]
	public void NativeContainerPut_ExtractedRoomPointerIsAdoptedOnlyWhileExactSourceRemainsUnclaimed(string change, bool allowMerge)
	{
		var f = new Fixture();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Gameworld).Returns(world.Object);
		prototype.SetupGet(x => x.Components).Returns([TestObjectFactory.CreateUninitialized<HoldableGameItemComponentProto>()]);
		var item = new GameItem(prototype.Object);
		var source = new Mock<MudSharp.Construction.IRoom>();
		var sourceItems = new List<IGameItem>();
		source.SetupGet(x => x.GameItems).Returns(sourceItems);
		var locationProperty = typeof(GameItem).GetProperty(nameof(GameItem.Location))!;
		locationProperty.SetValue(item, source.Object);
		if (change == "still-member") sourceItems.Add(item);
		var destination = new Mock<IGameItem>();
		destination.SetupGet(x => x.Gameworld).Returns(world.Object);
		if (change == "held-destination") destination.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		var container = TestObjectFactory.CreateUninitialized<ContainerGameItemComponent>();
		var contents = new List<IGameItem>();
		Set(container, "_contents", contents);
		typeof(ContainerGameItemComponent).GetProperty("Parent")!.SetValue(container, destination.Object);
		var otherRoom = Mock.Of<MudSharp.Construction.IRoom>();
		var holder = Mock.Of<IBody>();
		var proximity = new Mock<MudSharp.Construction.IProximityEventService>();
		var batch = new Mock<MudSharp.Construction.IProximityChangeBatch>();
		world.SetupGet(x => x.ProximityEventService).Returns(proximity.Object);
		proximity.Setup(x => x.BeginChange(MudSharp.Construction.ProximityChangeCause.Containment, It.IsAny<IPerceivable[]>()))
			.Callback(() =>
			{
				if (change == "reacquired") sourceItems.Add(item);
				if (change == "relocated") locationProperty.SetValue(item, otherRoom);
				if (change == "expire") f.Grant = null;
				if (change == "holder") item.GetItemType<IHoldable>().HeldBy = holder;
			}).Returns(batch.Object);
		using var execution = change == "ordinary" ? null : CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "put goods", () => true), f.Actor.Object);
		container.Put(change == "ordinary" ? null : f.Actor.Object, item, allowMerge);
		var accepted = change is "valid" or "ordinary" or "held-destination";
		Assert.AreEqual(accepted ? 1 : 0, contents.Count);
		Assert.AreSame(accepted ? destination.Object : null, item.ContainedIn);
		Assert.AreSame(accepted ? null : change == "relocated" ? otherRoom : source.Object, item.DirectLocation);
		Assert.AreSame(change == "holder" ? holder : null, item.GetItemType<IHoldable>().HeldBy);
		Assert.AreSame(change == "held-destination" ? f.Body.Object : change == "holder" ? holder : null, item.InInventoryOf);
		Assert.AreEqual(change is "still-member" or "reacquired", sourceItems.Contains(item));
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("direct-holder")]
	[DataRow("other-container")]
	public void NativeContainedRound_InheritsHeldGunLocationWithoutClaimingItsBody(string change)
	{
		var f = new Fixture();
		var body = TestObjectFactory.CreateUninitialized<Body>();
		body.Actor = f.Actor.Object;
		f.Actor.SetupGet(x => x.Body).Returns(body);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Gameworld).Returns(world.Object);
		prototype.SetupGet(x => x.Components).Returns([TestObjectFactory.CreateUninitialized<HoldableGameItemComponentProto>()]);
		var gun = new GameItem(prototype.Object);
		var round = new GameItem(prototype.Object);
		gun.GetItemType<IHoldable>().HeldBy = body;
		Set(body, "_heldItems", new List<Tuple<IGameItem, IGrab>> { Tuple.Create((IGameItem)gun, Mock.Of<IGrab>()) });
		round.ContainedIn = gun;
		Assert.AreSame(body, round.InInventoryOf);
		Assert.IsNull(round.GetItemType<IHoldable>().HeldBy);
		var foreign = Mock.Of<IGameItem>();
		if (change == "direct-holder") round.GetItemType<IHoldable>().HeldBy = Mock.Of<IBody>();
		if (change == "other-container") round.ContainedIn = foreign;
		Assert.AreEqual(change == "valid", ComponentUnloadCompletion.OwnedBy(round, gun));
		Assert.AreEqual(change == "valid", ComponentItemTransfer.ReleaseFiredItem(round, gun));
		Assert.AreSame(change == "valid" ? null : change == "other-container" ? foreign : gun, round.ContainedIn);
		Assert.AreSame(body, gun.InInventoryOf);
		Assert.AreSame(gun, body.HeldItems.Single());
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("before-expire")]
	[DataRow("before-claim")]
	[DataRow("after-expire")]
	[DataRow("after-claim")]
	public void NativeMusketContain_LoadReferenceAndCustodyCommitTogether(string change)
	{
		var f = new Fixture();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Gameworld).Returns(world.Object);
		prototype.SetupGet(x => x.Components).Returns([]);
		prototype.SetupGet(x => x.Name).Returns("load participant");
		var item = new GameItem(prototype.Object);
		var owner = new Mock<IGameItem>(); owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var foreign = Mock.Of<IGameItem>();
		var component = TestObjectFactory.CreateUninitialized<MusketGameItemComponent>();
		var contents = new List<IGameItem>(); Set(component, "_magazineContents", contents);
		typeof(MusketGameItemComponent).GetProperty("Parent")!.SetValue(component, owner.Object);
		var proximity = new Mock<MudSharp.Construction.IProximityEventService>();
		var batch = new Mock<MudSharp.Construction.IProximityChangeBatch>();
		world.SetupGet(x => x.ProximityEventService).Returns(proximity.Object);
		var calls = 0;
		void Change(string boundary)
		{
			if (!change.StartsWith(boundary, StringComparison.Ordinal)) return;
			if (change.EndsWith("expire", StringComparison.Ordinal)) f.Grant = null;
			else item.ContainedIn = foreign;
		}
		proximity.Setup(x => x.BeginChange(MudSharp.Construction.ProximityChangeCause.Containment, It.IsAny<IPerceivable[]>()))
			.Callback(() => { if (++calls == 1) Change("before"); }).Returns(batch.Object);
		batch.Setup(x => x.Complete()).Callback(() => { if (calls == 1) Change("after"); });
		using var execution = CommandExecutionScope.EnterDispatch(CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get goods", () => true), f.Actor.Object);
		var result = (bool)typeof(MusketGameItemComponent).GetMethod("ContainLoadedItem", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(component, [item])!;
		var adopted = change is "valid" or "after-expire";
		Assert.AreEqual(adopted, result);
		Assert.AreEqual(adopted ? 1 : 0, contents.Count);
		Assert.AreSame(adopted ? owner.Object : change.EndsWith("claim", StringComparison.Ordinal) ? foreign : null, item.ContainedIn);
		Assert.IsTrue(calls > 0);
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("before-expire")]
	[DataRow("before-claim")]
	[DataRow("after-expire")]
	[DataRow("after-claim")]
	public void NativeContainerPut_ContainmentCallbacksCommitExactlyOneParticipant(string change)
	{
		var f = new Fixture();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var prototype = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		prototype.SetupGet(x => x.Gameworld).Returns(world.Object);
		prototype.SetupGet(x => x.Name).Returns("native participant");
		var item = new GameItem(prototype.Object);
		var owner = new Mock<IGameItem>();
		owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var foreign = Mock.Of<IGameItem>();
		var contents = new List<IGameItem>();
		var container = TestObjectFactory.CreateUninitialized<ContainerGameItemComponent>();
		Set(container, "_contents", contents);
		typeof(ContainerGameItemComponent).GetProperty("Parent")!.SetValue(container, owner.Object);
		var proximity = new Mock<MudSharp.Construction.IProximityEventService>();
		var batch = new Mock<MudSharp.Construction.IProximityChangeBatch>();
		world.SetupGet(x => x.ProximityEventService).Returns(proximity.Object);
		var calls = 0;
		void Change(string boundary)
		{
			if (!change.StartsWith(boundary, StringComparison.Ordinal)) return;
			if (change.EndsWith("expire", StringComparison.Ordinal)) f.Grant = null;
			else item.ContainedIn = foreign;
		}
		proximity.Setup(x => x.BeginChange(MudSharp.Construction.ProximityChangeCause.Containment, It.IsAny<IPerceivable[]>()))
			.Callback(() => { if (++calls == 1) Change("before"); }).Returns(batch.Object);
		batch.Setup(x => x.Complete()).Callback(() => { if (calls == 1) Change("after"); });
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "put goods", () => true), f.Actor.Object);
		container.Put(f.Actor.Object, item, false);
		var adopted = change is "valid" or "after-expire";
		Assert.AreEqual(adopted ? 1 : 0, contents.Count);
		Assert.AreSame(adopted ? owner.Object : change.EndsWith("claim", StringComparison.Ordinal) ? foreign : null, item.ContainedIn);
		Assert.IsTrue(calls > 0, "Real native containment did not invoke the callback seam.");
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("expire")]
	[DataRow("claimed")]
	public void NativeContainerPut_MergeEligibilityCannotAdoptAfterRefusalOrClaim(string change)
	{
		var f = new Fixture();
		f.World.SetupGet(x => x.SaveManager).Returns(Mock.Of<MudSharp.Framework.Save.ISaveManager>());
		var destination = new Mock<IGameItem>();
		destination.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var foreign = new Mock<IGameItem>();
		var existing = new Mock<IGameItem>();
		existing.SetupGet(x => x.ContainedIn).Returns(destination.Object);
		var contents = new List<IGameItem> { existing.Object };
		var component = TestObjectFactory.CreateUninitialized<ContainerGameItemComponent>();
		Set(component, "_contents", contents);
		typeof(ContainerGameItemComponent).GetProperty("Parent")!.SetValue(component, destination.Object);
		f.Item.SetupProperty(x => x.ContainedIn);
		existing.Setup(x => x.CanMerge(f.Item.Object)).Callback(() =>
		{
			if (change == "expire") f.Grant = null;
			if (change == "claimed") f.Item.Object.ContainedIn = foreign.Object;
		}).Returns(false);
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "put goods", () => true), f.Actor.Object);
		component.Put(f.Actor.Object, f.Item.Object);
		Assert.AreEqual(change == "valid" ? 2 : 1, contents.Count);
		Assert.AreSame(change == "valid" ? destination.Object : change == "claimed" ? foreign.Object : null, f.Item.Object.ContainedIn);
	}

	[DataTestMethod]
	[DataRow("SimpleWound", "instance", 30L)]
	[DataRow("BoneFracture", "instance", 30L)]
	[DataRow("HealingSimpleWound", "instance", 30L)]
	[DataRow("RobotWound", "instance", 30L)]
	[DataRow("SimpleOrganicWound", "instance", 30L)]
	[DataRow("SimpleWound", "primary", 2L)]
	[DataRow("BoneFracture", "primary", 2L)]
	[DataRow("HealingSimpleWound", "primary", 2L)]
	[DataRow("RobotWound", "primary", 2L)]
	[DataRow("SimpleOrganicWound", "primary", 2L)]
	[DataRow("SimpleWound", "none", 0L)]
	[DataRow("BoneFracture", "none", 0L)]
	[DataRow("HealingSimpleWound", "none", 0L)]
	[DataRow("RobotWound", "none", 0L)]
	[DataRow("SimpleOrganicWound", "none", 0L)]
	public void WoundAttribution_PersistsCanonicalIdentityAndResolvesThatCharacter(string name, string origin, long expected)
	{
		var f = new Fixture();
		var type = typeof(MudSharp.Health.Wounds.SimpleWound).Assembly.GetType("MudSharp.Health.Wounds." + name)!;
		var wound = (MudSharp.Health.IWound)RuntimeHelpers.GetUninitializedObject(type);
		type.GetProperty("Gameworld")!.SetValue(wound, f.World.Object);
		f.World.SetupGet(x => x.SaveManager).Returns(Mock.Of<MudSharp.Framework.Save.ISaveManager>());
		ICharacter? actor = origin == "instance" ? f.Actor.Object : origin == "primary" ? f.Commander.Object : null;
		type.GetProperty(nameof(wound.ActorOrigin))!.SetValue(wound, actor);
		Assert.AreEqual(expected, type.GetField("_actorOriginId", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(wound));
		f.World.Setup(x => x.TryGetCharacter(expected, true)).Returns(expected == 30 ? f.Owner.Object : actor);
		Assert.AreSame(expected == 30 ? f.Owner.Object : actor, wound.ActorOrigin);
		f.World.Verify(x => x.TryGetCharacter(expected, true), Times.Once);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void InventoryPlanGet_ReportsUnknownWhenUnwieldDidNotActuallyProduceHeldCustody(bool completes)
	{
		var f = new Fixture();
		f.Item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(x => ReferenceEquals(x, f.Item.Object));
		var held = new List<IGameItem>();
		f.Body.SetupGet(x => x.HeldItems).Returns(held);
		f.Body.SetupGet(x => x.WieldedItems).Returns([f.Item.Object]);
		f.Item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		f.Body.Setup(x => x.Unwield(f.Item.Object, null, false)).Callback(() =>
		{
			if (completes) held.Add(f.Item.Object);
			else f.Grant = null;
		});
		var action = new InventoryPlanActionHold(f.World.Object, 0, 0, _ => true, _ => true);
		var template = new InventoryPlanTemplate(f.World.Object, action);
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "load weapon", () => true), f.Actor.Object);
		var method = typeof(InventoryPlanTemplate).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
			.Single(x => x.Name == "GetItem" && x.GetParameters().Length == 7);
		var result = (InventoryPlanActionResult)method.Invoke(template, [f.Actor.Object, f.Item.Object, null, 0, false, false, false])!;
		f.Body.Verify(x => x.Unwield(f.Item.Object, null, false), Times.Once);
		Assert.AreEqual(completes ? 1 : 0, held.Count, "The fixture did not perform its configured unwield callback.");
		Assert.AreEqual(completes ? DesiredItemState.Held : DesiredItemState.Unknown, result.ActionState);
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("expire")]
	[DataRow("replacement")]
	public void LiquidConsumption_CallbackCannotConsumeAReplacementMixtureOrALaterConstituent(string change)
	{
		var f = new Fixture();
		var first = Mock.Of<ILiquid>(x => x.Id == 1);
		var second = Mock.Of<ILiquid>(x => x.Id == 2);
		var original = new LiquidMixture(new[] { new LiquidInstance { Liquid = first, Amount = 4 }, new LiquidInstance { Liquid = second, Amount = 4 } }, f.World.Object);
		var replacement = new LiquidMixture(new[] { new LiquidInstance { Liquid = first, Amount = 8 }, new LiquidInstance { Liquid = second, Amount = 8 } }, f.World.Object);
		var requested = new LiquidMixture(new[] { new LiquidInstance { Liquid = first, Amount = 1 }, new LiquidInstance { Liquid = second, Amount = 1 } }, f.World.Object);
		var container = new Mock<ILiquidContainer>();
		container.SetupProperty(x => x.LiquidMixture, original);
		f.Item.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(container.Object);
		var calls = 0;
		original.OnLiquidMixtureChanged += _ =>
		{
			if (++calls != 1) return;
			if (change == "expire") f.Grant = null;
			if (change == "replacement") container.Object.LiquidMixture = replacement;
		};
		var action = new InventoryPlanActionConsumeLiquid(f.World.Object, 0, 0, _ => true, _ => true, _ => true, requested);
		var template = new InventoryPlanTemplate(f.World.Object, action);
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "load weapon", () => true), f.Actor.Object);
		var result = (InventoryPlanActionResult)typeof(InventoryPlanTemplate)
			.GetMethod("ConsumeLiquid", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(template, [f.Actor.Object, f.Item.Object, action, null])!;
		Assert.AreEqual(change == "valid" ? 6.0 : 7.0, original.TotalVolume);
		Assert.AreEqual(16.0, replacement.TotalVolume);
		Assert.AreEqual(change == "valid" ? DesiredItemState.ConsumeLiquid : DesiredItemState.Unknown, result.ActionState);
		Assert.IsTrue(CommandExecutionScope.HasCommitted);
		if (change == "replacement") Assert.AreSame(replacement, container.Object.LiquidMixture);
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("dead")]
	[DataRow("stasis")]
	[DataRow("body-rebound")]
	[DataRow("body-replaced")]
	[DataRow("combat-left")]
	public void OwnedChild_RequiresItsCapturedLivingExecutorBodyAndCombat(string change)
	{
		var f = new Fixture();
		var parent = new TooExhaustedMove { Assailant = f.Actor.Object };
		CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "attack target", () => true).Bind(parent);
		var combat = new Mock<ICombat>();
		combat.SetupGet(x => x.Combatants).Returns([f.Commander.Object]);
		f.Commander.SetupGet(x => x.Combat).Returns(combat.Object);
		var child = new Mock<ICombatMove>();
		child.SetupGet(x => x.Assailant).Returns(f.Commander.Object);
		child.Setup(x => x.ResolveMove(It.IsAny<ICombatMove>())).Returns(new CombatMoveResult { MoveWasSuccessful = true });
		var result = CommandExecutionScope.ResolveOwned(parent, child.Object, () =>
		{
			switch (change)
			{
				case "dead": f.Commander.SetupGet(x => x.State).Returns(CharacterState.Dead); break;
				case "stasis": f.Commander.SetupGet(x => x.State).Returns(CharacterState.Stasis); break;
				case "body-rebound": Mock.Get(f.Commander.Object.Body).SetupGet(x => x.Actor).Returns(f.Owner.Object); break;
				case "body-replaced": f.Commander.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Actor == f.Commander.Object)); break;
				case "combat-left": f.Commander.SetupGet(x => x.Combat).Returns((ICombat)null!); break;
			}
			return null;
		});
		Assert.AreEqual(change == "valid", result.MoveWasSuccessful);
		child.Verify(x => x.ResolveMove(It.IsAny<ICombatMove>()), change == "valid" ? Times.Once : Times.Never);
	}

	[TestMethod]
	public void AcceptedOrder_CannotRebaseOntoAnotherCanonicalBodyForTheSameActor()
	{
		var f = new Fixture();
		var accepted = CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "wear item", () => true);
		f.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>(x => x.Actor == f.Actor.Object));
		Assert.IsFalse(accepted.MayExecute(f.Actor.Object));
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("expire")]
	[DataRow("rebind")]
	public void StockFollow_NativeTargetLookupCannotChangeFollowingAfterAuthorityLoss(string change)
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.Follow(f.Commander.Object)).Callback(() => f.Actor.SetupGet(x => x.Following).Returns(f.Commander.Object));
		f.Actor.Setup(x => x.TargetActor("commander")).Callback(() =>
		{
			if (change == "expire") f.Grant = null;
			if (change == "rebind") f.Body.SetupGet(x => x.Actor).Returns(f.Owner.Object);
		}).Returns(f.Commander.Object);
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "follow commander", () => true), f.Actor.Object);
		typeof(MudSharp.Commands.Modules.MovementModule).GetMethod("Follow", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [f.Actor.Object, "follow commander"]);
		f.Actor.Verify(x => x.Follow(f.Commander.Object), change == "valid" ? Times.Once() : Times.Never());
		if (change != "valid") Assert.IsTrue(CommandExecutionScope.RejectedBeforeCommit);
	}

	[TestMethod]
	public void NativeGetPreparation_SelectsAnUnoccupiedHandEvenWhenEligibilityReportsBothHandsUsable()
	{
		var f = new Fixture();
		var body = TestObjectFactory.CreateUninitialized<Body>();
		body.Actor = f.Actor.Object;
		f.Actor.SetupGet(x => x.Body).Returns(body);
		var occupied = new Mock<IGrab>();
		var free = new Mock<IGrab>();
		occupied.Setup(x => x.CanGrab(f.Item.Object, body)).Returns(WearlocGrabResult.Success);
		free.Setup(x => x.CanGrab(f.Item.Object, body)).Returns(WearlocGrabResult.Success);
		Set(body, "_holdlocs", new List<IGrab> { occupied.Object, free.Object });
		Set(body, "_heldItems", new List<Tuple<IGameItem, IGrab>> { Tuple.Create(Mock.Of<IGameItem>(), occupied.Object) });
		var field = typeof(Body).GetField("_wieldedItems", BindingFlags.Instance | BindingFlags.NonPublic)!;
		field.SetValue(body, Activator.CreateInstance(field.FieldType));
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "get gear", () => true), f.Actor.Object);
		var placement = typeof(Body).GetMethod("PrepareGetPlacement", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(body, [f.Item.Object, false]);
		Assert.IsNotNull(placement);
		Assert.AreSame(free.Object, placement.GetType().GetProperty("Hand")!.GetValue(placement));
		occupied.Verify(x => x.CanGrab(It.IsAny<IGameItem>(), It.IsAny<MudSharp.Body.IInventory>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false, "valid")]
	[DataRow(true, "valid")]
	[DataRow(false, "refused")]
	[DataRow(true, "refused")]
	[DataRow(false, "relocated")]
	[DataRow(true, "relocated")]
	[DataRow(false, "deleted")]
	[DataRow(true, "deleted")]
	public void ComponentLoad_WholeItemRequiresSuccessfulDetachmentAndPreservesCallbackCustody(bool byWeight, string change)
	{
		var f = new Fixture();
		f.Item.SetupProperty(x => x.ContainedIn);
		f.Item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		f.Item.Setup(x => x.DropsWholeByWeight(It.IsAny<double>())).Returns(true);
		f.Body.Setup(x => x.Take(f.Item.Object)).Callback(() =>
		{
			if (change == "refused") return;
			f.Item.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
			if (change == "relocated") f.Item.Object.ContainedIn = Mock.Of<IGameItem>();
			if (change == "deleted") f.Item.SetupGet(x => x.Deleted).Returns(true);
		});
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "load weapon", () => true), f.Actor.Object);
		var detached = byWeight ? ComponentItemTransfer.TakeByWeight(f.Actor.Object, f.Item.Object, 1.0)
			: ComponentItemTransfer.TakeOne(f.Actor.Object, f.Item.Object);
		Assert.AreEqual(change == "valid", detached is not null);
		var destination = Mock.Of<IGameItem>();
		Assert.AreEqual(change == "valid", ComponentItemTransfer.Contain(detached, destination));
		if (change == "valid") Assert.AreSame(destination, f.Item.Object.ContainedIn);
		if (change == "relocated") Assert.AreNotSame(destination, f.Item.Object.ContainedIn);
	}

	[DataTestMethod]
	[DataRow("valid")]
	[DataRow("relocated")]
	[DataRow("deleted")]
	public void ComponentLoad_WeightSplitDoesNotAssignAnUntrackedBodyHolder(string change)
	{
		var f = new Fixture();
		f.Item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		f.Item.Setup(x => x.DropsWholeByWeight(It.IsAny<double>())).Returns(false);
		var split = new Mock<IGameItem>();
		f.Item.Setup(x => x.GetByWeight(null, 1.0)).Returns(() =>
		{
			if (change == "relocated") split.SetupGet(x => x.Location).Returns(f.Actor.Object.Location);
			if (change == "deleted") split.SetupGet(x => x.Deleted).Returns(true);
			return split.Object;
		});
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "load weapon", () => true), f.Actor.Object);
		var detached = ComponentItemTransfer.TakeByWeight(f.Actor.Object, f.Item.Object, 1.0);
		Assert.AreEqual(change == "valid", detached is not null);
		Assert.IsTrue(CommandExecutionScope.HasCommitted);
		f.Item.Verify(x => x.GetByWeight(null, 1.0), Times.Once);
		f.Item.Verify(x => x.GetByWeight(f.Body.Object, It.IsAny<double>()), Times.Never);
		f.Body.Verify(x => x.Take(It.IsAny<IGameItem>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DelayedOwnedContinuation_RetainsOriginalGrantAndCombatBinding(bool departure)
	{
		var f = new Fixture();
		var move = new TooExhaustedMove { Assailant = f.Actor.Object };
		CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "load weapon", () => true).Bind(move);
		CommandExecutionScope.Continuation delayed;
		using (CommandExecutionScope.EnterMove(move)) delayed = CommandExecutionScope.CaptureContinuation(f.Actor.Object);
		if (departure) f.Actor.SetupGet(x => x.Combat).Returns((ICombat)null!);
		else f.Grant = f.Grant! with { Id = Guid.NewGuid() };
		using (delayed.Enter()) Assert.IsFalse(CommandExecutionScope.TryContinue());
		using (CommandExecutionScope.EnterIndependent()) Assert.IsTrue(CommandExecutionScope.TryContinue());
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ReusedChild_AutonomousParentAndIndependentCounterDoNotRetainAnExpiredOrder(bool counter)
	{
		var f = new Fixture();
		var child = new Mock<ICombatMove>();
		child.SetupGet(x => x.Assailant).Returns(f.Actor.Object);
		child.Setup(x => x.ResolveMove(It.IsAny<ICombatMove>())).Returns(new CombatMoveResult { MoveWasSuccessful = true });
		CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "attack target", () => true).Bind(child.Object);
		f.Grant = null;
		var parent = new TooExhaustedMove { Assailant = f.Actor.Object };
		var result = counter ? CommandExecutionScope.ResolveIndependent(child.Object, () => null)
			: CommandExecutionScope.ResolveOwned(parent, child.Object, () => null);
		Assert.IsTrue(result.MoveWasSuccessful);
		child.Verify(x => x.ResolveMove(It.IsAny<ICombatMove>()), Times.Once);
		Assert.IsNull(CommandExecutionAuthority.ForMove(child.Object));
	}
	private static BowGameItemComponent BowFor(Fixture fixture)
	{
		var bow = TestObjectFactory.CreateUninitialized<BowGameItemComponent>();
		typeof(GameItemComponent).GetProperty(nameof(GameItemComponent.Parent))!.SetValue(bow, fixture.Item.Object);
		var hand = Mock.Of<IGrab>();
		fixture.Body.SetupGet(x => x.HoldLocs).Returns([hand]);
		fixture.Body.Setup(x => x.CanUseBodypart(hand)).Returns(CanUseBodypartResult.CanUse);
		fixture.Actor.Setup(x => x.CanManipulateItem(fixture.Item.Object)).Returns((true, string.Empty));
		Mock.Get(fixture.Actor.Object.OutputHandler).SetupGet(x => x.Perceiver).Returns(fixture.Actor.Object);
		Mock.Get(fixture.Actor.Object.Location).Setup(x => x.LayerCharacters(It.IsAny<MudSharp.Construction.RoomLayer>()))
			.Returns([fixture.Actor.Object]);
		return bow;
	}

	[DataTestMethod]
	[DataRow("expire")]
	[DataRow("rebind")]
	public void NativeBowFire_OutputCallbackCannotConsumeTheLoadedProjectileAfterAuthorityLoss(string change)
	{
		var f = new Fixture();
		var bow = BowFor(f);
		var ammo = new Mock<MudSharp.GameItems.Interfaces.IAmmo>();
		ammo.SetupGet(x => x.Parent).Returns(Mock.Of<IGameItem>());
		bow.LoadedAmmo = ammo.Object;
		bow.IsReadied = true;
		Mock.Get(f.Actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<MudSharp.PerceptionEngine.IOutput>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback(() =>
			{
				if (change == "expire") f.Grant = null;
				else f.Body.SetupGet(x => x.Actor).Returns(f.Owner.Object);
			}).Returns(true);
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "fire target", () => true), f.Actor.Object);
		bow.Fire(f.Actor.Object, f.Commander.Object, MudSharp.RPG.Checks.Outcome.Pass, MudSharp.RPG.Checks.Outcome.Pass,
			new OpposedOutcome(MudSharp.RPG.Checks.Outcome.Pass, MudSharp.RPG.Checks.Outcome.Fail), null, null, f.Commander.Object);
		Assert.AreSame(ammo.Object, bow.LoadedAmmo);
		Assert.IsTrue(bow.IsReadied);
		Assert.IsTrue(CommandExecutionScope.RejectedBeforeCommit);
		Assert.AreEqual(0, ammo.Invocations.Count(x => x.Method.Name == "Fire"));
	}

	[TestMethod]
	public void NativeBowReady_HandEligibilityRevocationCannotInstallReadiedStateOrStaminaDrain()
	{
		var f = new Fixture();
		var bow = BowFor(f);
		bow.LoadedAmmo = Mock.Of<MudSharp.GameItems.Interfaces.IAmmo>();
		f.Body.SetupGet(x => x.WieldedItems).Returns([f.Item.Object]);
		f.Item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(x => ReferenceEquals(x, f.Item.Object));
		var first = Mock.Of<IWield>();
		var second = Mock.Of<IWield>();
		f.Body.SetupGet(x => x.WieldLocs).Returns([first, second]);
		f.Body.Setup(x => x.CanUseBodypart(first)).Callback(() => f.Grant = null).Returns(CanUseBodypartResult.CanUse);
		f.Body.Setup(x => x.CanUseBodypart(second)).Returns(CanUseBodypartResult.CanUse);
		f.Body.Setup(x => x.HeldItemsFor(It.IsAny<IBodypart>())).Returns([]);
		f.Body.Setup(x => x.WieldedItemsFor(It.IsAny<IBodypart>())).Returns([]);
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "ready bow", () => true), f.Actor.Object);
		Assert.IsFalse(bow.Ready(f.Actor.Object));
		Assert.IsFalse(bow.IsReadied);
		Assert.IsTrue(CommandExecutionScope.RejectedBeforeCommit);
		Assert.AreEqual(0, Mock.Get(f.Actor.Object.OutputHandler).Invocations.Count);
		Assert.IsFalse(f.Actor.Invocations.Any(x => x.Method.Name == "AddEffect"));
	}
	private sealed class OrderedMultiTargetMove(IReadOnlyList<ICombatMove> moves, IReadOnlyList<ICharacter> targets)
		: MultiTargetCombatMove(moves, targets);
	[TestMethod]
	public void CombatAction_CommittedDamagePaysStaminaAfterCallbackDeparture()
	{
		var f = new Fixture();
		var combat = f.Engage();
		f.Actor.SetupGet(x => x.Race).Returns(Mock.Of<MudSharp.Character.Heritage.IRace>(x => x.RaceUsesStamina));
		var name = new Mock<MudSharp.Character.Name.IPersonalName>();
		name.Setup(x => x.GetName(It.IsAny<MudSharp.Character.Name.NameStyle>())).Returns("ordered attacker");
		f.Actor.SetupGet(x => x.CurrentName).Returns(name.Object);
		var wound = Mock.Of<MudSharp.Health.IWound>();
		var result = new CombatMoveResult { MoveWasSuccessful = true, WoundsCaused = [wound] };
		var move = new Mock<ICombatMove>();
		move.SetupGet(x => x.Assailant).Returns(f.Actor.Object);
		move.SetupGet(x => x.CharacterTargets).Returns([]);
		move.SetupGet(x => x.StaminaCost).Returns(7.0);
		move.Setup(x => x.UsesStaminaWithResult(result)).Returns(true);
		move.Setup(x => x.ResolveMove(It.IsAny<ICombatMove>())).Callback(() =>
		{
			CommandExecutionScope.MarkCommitted();
			f.Actor.Object.Combat = null;
		}).Returns(result);
		CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "manual target", () => true).Bind(move.Object);
		Mock.Get(f.World.Object.Scheduler).Invocations.Clear();
		combat.CombatAction(f.Actor.Object, move.Object);
		move.Verify(x => x.ResolveMove(It.IsAny<ICombatMove>()), Times.Once);
		f.Actor.Verify(x => x.SpendStamina(7.0), Times.Once);
		Assert.AreEqual(0, Mock.Get(f.World.Object.Scheduler).Invocations.Count);
	}

	[TestMethod]
	public void MultiTarget_CompletedChildWoundsSurviveRefusalOfTheNextOwnedChild()
	{
		var f = new Fixture();
		var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Combat).Returns(f.Actor.Object.Combat);
		target.SetupGet(x => x.Race).Returns(Mock.Of<MudSharp.Character.Heritage.IRace>());
		var wound = Mock.Of<MudSharp.Health.IWound>();
		var first = new Mock<ICombatMove>();
		first.SetupGet(x => x.Assailant).Returns(f.Actor.Object);
		first.SetupGet(x => x.CharacterTargets).Returns([target.Object]);
		first.Setup(x => x.ResolveMove(It.IsAny<ICombatMove>())).Callback(() =>
		{
			CommandExecutionScope.MarkCommitted();
			f.Grant = null;
		}).Returns(new CombatMoveResult { MoveWasSuccessful = true, WoundsCaused = [wound] });
		var second = new Mock<ICombatMove>();
		second.SetupGet(x => x.Assailant).Returns(f.Actor.Object);
		second.SetupGet(x => x.CharacterTargets).Returns([target.Object]);
		var parent = new OrderedMultiTargetMove([first.Object, second.Object], [target.Object, target.Object]);
		CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "manual target", () => true).Bind(parent);
		var result = CommandExecutionScope.Resolve(parent, null);
		CollectionAssert.AreEqual(new[] { wound }, result.WoundsCaused.ToArray());
		Assert.IsTrue(result.MoveWasSuccessful);
		Assert.IsTrue(parent.UsesStaminaWithResult(result));
		second.Verify(x => x.ResolveMove(It.IsAny<ICombatMove>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow("stand", "stand", "", true)]
	[DataRow("sit", "stand", "", false)]
	[DataRow("stand", "position", "", true)]
	[DataRow("stand", "stand", "position", false)]
	[DataRow("stand", "position", "stand", false)]
	public void StockPostureAlias_PreservesBuilderAllowlistAndBothBanForms(string requested, string included, string banned, bool allowed)
	{
		var f = new Fixture();
		var executed = false;
		f.Commands.Add(["stand", "sit", "kneel"], new Command<ICharacter>((_, _) => executed = true,
			states: CharacterState.Awake, name: "Position"));
		Set(f.Ai, "_includedCommands", new List<string> { included });
		Set(f.Ai, "_bannedCommands", banned.Length == 0 ? new List<string>() : new List<string> { banned });
		Assert.IsTrue(f.Ai.HandleEvent(EventType.CommandIssuedToCharacter, f.Actor.Object, f.Commander.Object, requested));
		Assert.AreEqual(allowed, executed);
	}

	[TestMethod]
	public void SpecificWearProfile_ExecutesEligibilityBeforeAnExpiredOrderCanWear()
	{
		var f = new Fixture();
		var prototype = TestObjectFactory.CreateUninitialized<WearableGameItemComponentProto>();
		var wearable = TestObjectFactory.CreateUninitialized<WearableGameItemComponent>();
		Set(wearable, "_prototype", prototype);
		var eligibility = new Mock<IFutureProg>();
		eligibility.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() => f.Grant = null).Returns(true);
		typeof(WearableGameItemComponentProto).GetProperty(nameof(prototype.WearableProg))!.SetValue(prototype, eligibility.Object);
		using var execution = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "wear goods profile", () => true), f.Actor.Object);
		Assert.IsFalse(wearable.CanWear(f.Body.Object, Mock.Of<MudSharp.GameItems.Inventory.IWearProfile>()));
		eligibility.Verify(x => x.ExecuteBool(It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void EveryConcreteMove_ExpiredBoundOrderIsRefusedBeforeItsResolverBody()
	{
		var f = new Fixture();
		var authority = CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "manual target", () => true);
		var moves = typeof(CombatMoveBase).Assembly.GetTypes()
			.Where(x => !x.IsAbstract && typeof(CombatMoveBase).IsAssignableFrom(x) &&
				x.Namespace == typeof(CombatMoveBase).Namespace && x != typeof(MagicDefenseMove))
			.Select(type => (Type: type, Move: (CombatMoveBase)RuntimeHelpers.GetUninitializedObject(type))).ToList();
		foreach (var (_, move) in moves)
		{
			typeof(CombatMoveBase).GetField("_assailant", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(move, f.Actor.Object);
			authority.Bind(move);
		}
		f.Grant = null;
		Assert.IsTrue(moves.Count >= 80, "The matrix must enumerate the concrete runtime move families.");
		foreach (var (type, move) in moves)
			Assert.AreSame(CombatMoveResult.Irrelevant, move.ResolveMove(null), type.FullName);
	}

	[DataTestMethod]
	[DataRow("expire")]
	[DataRow("rebind")]
	public void QueuedWear_RealBodyEligibilityCannotMutateAfterAuthorityLoss(string change)
	{
		var f = new Fixture();
		var body = TestObjectFactory.CreateUninitialized<Body>();
		f.Item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(x => ReferenceEquals(x, f.Item.Object));
		body.Actor = f.Actor.Object;
		Set(body, "_heldItems", new List<Tuple<IGameItem, IGrab>> { Tuple.Create(f.Item.Object, Mock.Of<IGrab>()) });
		foreach (var fieldName in new[] { "_wieldedItems", "_wornItems" })
		{
			var field = typeof(Body).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		f.Actor.SetupGet(x => x.Body).Returns(body);
		var prototype = TestObjectFactory.CreateUninitialized<WearableGameItemComponentProto>();
		var wearable = TestObjectFactory.CreateUninitialized<WearableGameItemComponent>();
		Set(wearable, "_prototype", prototype);
		var eligibility = new Mock<IFutureProg>();
		var callbacks = 0;
		eligibility.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() =>
		{
			callbacks++;
			f.Grant = null;
			if (change == "rebind") body.Actor = f.Owner.Object;
		}).Returns(true);
		typeof(WearableGameItemComponentProto).GetProperty(nameof(prototype.WearableProg))!.SetValue(prototype, eligibility.Object);
		f.Item.Setup(x => x.GetItemType<MudSharp.GameItems.Interfaces.IWearable>()).Returns(wearable);
		f.Commands.Add(["wear"], new Command<ICharacter>((actor, _) =>
			actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectWearItem(actor, f.Item.Object, null, null)),
			states: CharacterState.Awake, name: "Wear"));
		Set(f.Ai, "_includedCommands", new List<string> { "wear" });
		Assert.IsTrue(f.Ai.HandleEvent(EventType.CommandIssuedToCharacter, f.Actor.Object, f.Commander.Object, "wear goods"));
		var move = f.Queued!.GetMove(f.Actor.Object)!;
		Assert.IsInstanceOfType(move, typeof(WearItemMove));
		Assert.AreSame(body, move.Assailant.Body, "The move must use the native body rather than the fixture's original mock.");
		Assert.AreSame(wearable, f.Item.Object.GetItemType<MudSharp.GameItems.Interfaces.IWearable>());
		Assert.AreSame(eligibility.Object, prototype.WearableProg);
		Assert.IsTrue(CommandExecutionAuthority.MayExecute(move, f.Actor.Object), "The grant must remain live until wearable eligibility executes.");
		Assert.IsTrue(body.HeldOrWieldedItems.Contains(f.Item.Object));
		var result = CommandExecutionScope.Resolve(move, null);
		Assert.AreEqual(1, callbacks);
		Assert.AreSame(CombatMoveResult.Irrelevant, result);
		Assert.IsFalse(move.UsesStaminaWithResult(result));
		CollectionAssert.AreEqual(new[] { f.Item.Object }, body.HeldItems.ToArray());
		Assert.AreEqual(0, body.WornItemsFullInfo.Count());
		Assert.IsNull(wearable.WornBy);
		Assert.IsFalse(body.InventoryChanged);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void OwnedChild_OriginalPrincipalControlsChildWhileIndependentResponseHasNoGrant(bool independent)
	{
		var f = new Fixture();
		var parent = new TooExhaustedMove { Assailant = f.Actor.Object };
		CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "attack target", () => true).Bind(parent);
		var child = new Mock<ICombatMove>();
		child.SetupGet(x => x.Assailant).Returns(f.Commander.Object);
		child.Setup(x => x.ResolveMove(It.IsAny<ICombatMove>())).Returns(new CombatMoveResult { MoveWasSuccessful = true });
		using var execution = CommandExecutionScope.EnterMove(parent);
		f.Grant = null;
		var result = independent
			? CommandExecutionScope.ResolveIndependent(child.Object, () => null)
			: CommandExecutionScope.ResolveOwned(parent, child.Object, () => null);
		Assert.AreEqual(independent, result.MoveWasSuccessful);
		child.Verify(x => x.ResolveMove(It.IsAny<ICombatMove>()), independent ? Times.Once : Times.Never);
	}

	[TestMethod]
	public void ForeignBodyCommit_DoesNotChargeTheUnexecutedParentOrder()
	{
		var f = new Fixture();
		var parent = new TooExhaustedMove { Assailant = f.Actor.Object };
		CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "wear item", () => true).Bind(parent);
		using (CommandExecutionScope.EnterMove(parent))
		{
			using (CommandExecutionScope.EnterBodyOperation(f.Commander.Object))
			{
				CommandExecutionScope.MarkCommitted(f.Commander.Object);
				Assert.IsNull(CommandExecutionAuthority.Capture(f.Actor.Object));
			}
			f.Grant = null;
			Assert.IsFalse(CommandExecutionScope.TryContinue(f.Actor.Object));
			Assert.IsTrue(CommandExecutionScope.RejectedBeforeCommit);
		}
		Assert.IsFalse(parent.UsesStaminaWithResult(CombatMoveResult.Irrelevant));
	}
}
