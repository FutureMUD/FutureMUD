using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class PhysicalManipulationTests
{
	[TestMethod]
	public void ManualAction_NoManipulators_IsRejected()
	{
		var body = new Mock<IBody>();
		Assert.IsFalse(body.Object.CanPerformManualAction(out var reason));
		StringAssert.Contains(reason, "functioning");
	}

	[TestMethod]
	[DataRow(CanUseBodypartResult.CantUseLimbDamage)]
	[DataRow(CanUseBodypartResult.CantUseLimbPain)]
	[DataRow(CanUseBodypartResult.CantUseSevered)]
	[DataRow(CanUseBodypartResult.CantUseLimbGrappled)]
	[DataRow(CanUseBodypartResult.CantUseLimbRestrained)]
	[DataRow(CanUseBodypartResult.CantUseMissingBone)]
	[DataRow(CanUseBodypartResult.CantUseSpinalDamage)]
	[DataRow(CanUseBodypartResult.CantUsePartDamage)]
	[DataRow(CanUseBodypartResult.CantUseNonFunctionalProsthetic)]
	public void ManualAction_UnusableManipulator_IsRejected(CanUseBodypartResult failure)
	{
		var (_, body, _, _) = ActorWithHand();
		body.Setup(x => x.CanUseBodypart(It.IsAny<IBodypart>())).Returns(failure);
		Assert.IsFalse(body.Object.CanPerformManualAction(out _));
	}

	[TestMethod]
	public void ManualAction_OneWorkingNonhumanManipulator_IsEnoughEvenWhenOccupied()
	{
		var (_, body, _, _) = ActorWithHand();
		body.SetupGet(x => x.WielderDescriptionPlural).Returns("tentacles");
		body.Setup(x => x.HeldItemsFor(It.IsAny<IBodypart>())).Returns([Mock.Of<IGameItem>()]);
		Assert.IsTrue(body.Object.CanPerformManualAction(out _));
	}

	[TestMethod]
	public void Body_CanUseBodypart_RejectsASeveredPartEvenWithoutALimbEffect()
	{
		var body = EmptyBody();
		Assert.AreEqual(CanUseBodypartResult.CantUseSevered, body.CanUseBodypart(Mock.Of<IGrab>()));
	}

	[TestMethod]
	public void Body_CanUseBodypart_PropagatesLimbRestraints()
	{
		var body = EmptyBody();
		var hand = Mock.Of<IGrab>();
		var limb = new Mock<ILimb>();
		limb.SetupGet(x => x.Parts).Returns([hand]);
		SetField(body, "_bodyparts", new HashSet<IBodypart> { hand });
		SetField(body, "_limbs", new List<ILimb> { limb.Object });
		var prototype = new Mock<IBodyPrototype>();
		prototype.SetupGet(x => x.Limbs).Returns([limb.Object]);
		typeof(Body).GetProperty(nameof(Body.Prototype))!.SetValue(body, prototype.Object);
		var restraint = new Mock<ILimbIneffectiveEffect>();
		restraint.Setup(x => x.Applies(limb.Object)).Returns(true);
		restraint.Setup(x => x.AppliesToLimb(limb.Object)).Returns(true);
		restraint.SetupGet(x => x.Reason).Returns(LimbIneffectiveReason.Restrained);
		var effects = new Mock<IEffectHandler>();
		effects.Setup(x => x.EffectsOfType<ILimbIneffectiveEffect>(It.IsAny<Predicate<ILimbIneffectiveEffect>>()))
			.Returns([restraint.Object]);
		typeof(Body).GetProperty(nameof(Body.EffectHandler))!.SetValue(body, effects.Object);
		Assert.AreEqual(CanUseBodypartResult.CantUseLimbRestrained, body.CanUseBodypart(hand));
	}

	[TestMethod]
	public void Reach_NestedClosedContainer_IsRejectedBeforeRoomAccess()
	{
		var (actor, _, room, world) = ActorWithHand();
		var outer = Item(world, room);
		var inner = Item(world);
		var item = Item(world);
		inner.SetupGet(x => x.ContainedIn).Returns(outer.Object);
		item.SetupGet(x => x.ContainedIn).Returns(inner.Object);
		var contents = new Mock<IContainer>();
		contents.SetupGet(x => x.Contents).Returns([inner.Object]);
		outer.Setup(x => x.GetItemType<IContainer>()).Returns(contents.Object);
		outer.Setup(x => x.GetItemType<IOpenable>()).Returns(Mock.Of<IOpenable>(x => !x.IsOpen));
		outer.Setup(x => x.GetItemType<IDoor>()).Returns((IDoor)null!);
		outer.Setup(x => x.GetItemType<IContainer>()).Returns(contents.Object);
		var result = actor.Object.CanReachItem(item.Object);
		Assert.IsFalse(result.Truth);
		StringAssert.Contains(result.Message, "open");
	}

	[TestMethod]
	public void Reach_GuardedAncestor_IsRejected()
	{
		var (actor, _, room, world) = ActorWithHand();
		var outer = Item(world, room);
		var item = Item(world);
		item.SetupGet(x => x.ContainedIn).Returns(outer.Object);
		room.Setup(x => x.CanGetAccess(outer.Object, actor.Object)).Returns(false);
		room.Setup(x => x.WhyCannotGetAccess(outer.Object, actor.Object)).Returns("The guard prevents access.");
		Assert.AreEqual((false, "The guard prevents access."), actor.Object.CanReachItem(item.Object));
	}

	[TestMethod]
	public void Reach_ClosedDoorExternalLock_RemainsReachableFromEitherSide()
	{
		var (actor, _, room, world) = ActorWithHand();
		var farSide = new Mock<IRoom>();
		farSide.Setup(x => x.CanGetAccess(It.IsAny<IGameItem>(), actor.Object)).Returns(true);
		var door = Item(world, room);
		var lockItem = Item(world);
		lockItem.SetupGet(x => x.ContainedIn).Returns(door.Object);
		var exit = new Mock<IExit>();
		exit.SetupGet(x => x.Rooms).Returns([room.Object, farSide.Object]);
		var doorComponent = new Mock<IDoor>();
		doorComponent.SetupGet(x => x.InstalledExit).Returns(exit.Object);
		doorComponent.SetupGet(x => x.IsOpen).Returns(false);
		door.Setup(x => x.GetItemType<IDoor>()).Returns(doorComponent.Object);
		door.Setup(x => x.GetItemType<IOpenable>()).Returns(doorComponent.Object);
		Assert.IsTrue(actor.Object.CanReachItem(lockItem.Object).Truth);
		actor.SetupGet(x => x.Location).Returns(farSide.Object);
		Assert.IsTrue(actor.Object.CanReachItem(lockItem.Object).Truth);
	}

	[TestMethod]
	public void Reach_OtherInventory_RequiresPermissionAndProximity()
	{
		var (actor, _, room, world) = ActorWithHand();
		var (owner, ownerBody, _, _) = ActorWithHand();
		owner.SetupGet(x => x.Location).Returns(room.Object);
		var item = Item(world);
		item.SetupGet(x => x.InInventoryOf).Returns(ownerBody.Object);
		Assert.IsFalse(actor.Object.CanReachItem(item.Object).Truth);
		Assert.IsTrue(actor.Object.CanReachItem(item.Object, requireInventoryPermission: false).Truth);
		owner.Setup(x => x.WillingToPermitInventoryManipulation(actor.Object)).Returns(true);
		Assert.IsTrue(actor.Object.CanReachItem(item.Object).Truth);
		owner.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		Assert.IsFalse(actor.Object.CanReachItem(item.Object, requireInventoryPermission: false).Truth);
	}

	[DataTestMethod]
	[DataRow(false, true)]
	[DataRow(true, false)]
	public void SheathEmpty_OtherInventoryWithoutConsentOrProximity_PreservesContents(bool consent, bool colocated)
	{
		var (actor, _, room, world) = ActorWithHand();
		var (owner, ownerBody, _, _) = ActorWithHand();
		owner.SetupGet(x => x.Location).Returns(colocated ? room.Object : Mock.Of<IRoom>());
		owner.Setup(x => x.WillingToPermitInventoryManipulation(actor.Object)).Returns(consent);
		var sheathItem = Item(world);
		sheathItem.SetupGet(x => x.InInventoryOf).Returns(ownerBody.Object);
		actor.Setup(x => x.CanManipulateItem(It.IsAny<IGameItem>()))
			.Returns<IGameItem>(item => actor.Object.CanReachItem(item));
		var weapon = Item(world);
		weapon.SetupGet(x => x.ContainedIn).Returns(sheathItem.Object);
		var wieldable = new Mock<IWieldable>();
		wieldable.SetupGet(x => x.Parent).Returns(weapon.Object);
		var sheath = TestObjectFactory.CreateUninitialized<SheathGameItemComponent>();
		typeof(GameItemComponent).GetProperty(nameof(GameItemComponent.Parent))!.SetValue(sheath, sheathItem.Object);
		typeof(SheathGameItemComponent).GetField("_contents", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(sheath, new List<IWieldable> { wieldable.Object });

		sheath.Empty(actor.Object, null!);

		Assert.AreSame(weapon.Object, sheath.Contents.Single());
		Assert.AreSame(sheathItem.Object, weapon.Object.ContainedIn);
		weapon.VerifySet(x => x.ContainedIn = It.IsAny<IGameItem>(), Times.Never);
		weapon.Verify(x => x.Delete(), Times.Never);
		room.Verify(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()), Times.Never);
		if (colocated)
		{
			owner.Verify(x => x.WillingToPermitInventoryManipulation(actor.Object), Times.Once);
		}
	}

	[TestMethod]
	[Timeout(3000)]
	public void Reach_DifferentLayerOrRoom_AndContainmentCycle_AreRejected()
	{
		var (actor, _, room, world) = ActorWithHand();
		var item = Item(world, room);
		Assert.IsTrue(actor.Object.CanReachItem(item.Object).Truth);
		item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);
		Assert.IsFalse(actor.Object.CanReachItem(item.Object).Truth);
		item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		item.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		Assert.IsFalse(actor.Object.CanReachItem(item.Object).Truth);
		item.SetupGet(x => x.ContainedIn).Returns(item.Object);
		Assert.IsFalse(actor.Object.CanReachItem(item.Object).Truth);
	}

	[TestMethod]
	public void Guard_NullActor_IsReservedForSystemOperations()
	{
		Assert.IsTrue(ItemManipulationGuard.CanManipulate(null, out _, Mock.Of<IGameItem>()));
	}

	[TestMethod]
	public void Guard_NeuralControl_OnlyExemptsInternalImplantsInstalledInTheActor()
	{
		var (actor, body, _, world) = ActorWithHand();
		body.SetupGet(x => x.HoldLocs).Returns([]);
		var item = Item(world);
		var implant = new Mock<IImplantRespondToCommands>();
		implant.SetupGet(x => x.InstalledBody).Returns(body.Object);
		item.Setup(x => x.GetItemType<IImplantRespondToCommands>()).Returns(implant.Object);
		item.Setup(x => x.GetItemType<IImplant>()).Returns(implant.Object);
		Assert.IsTrue(ItemManipulationGuard.CanManipulate(actor.Object, out _, item.Object));
		implant.SetupGet(x => x.External).Returns(true);
		Assert.IsFalse(ItemManipulationGuard.CanManipulate(actor.Object, out _, item.Object));
		implant.SetupGet(x => x.External).Returns(false);
		implant.SetupGet(x => x.InstalledBody).Returns(Mock.Of<IBody>());
		Assert.IsFalse(ItemManipulationGuard.CanManipulate(actor.Object, out _, item.Object));
	}

	[TestMethod]
	public void Reach_MountedModule_RequiresAccessibleHousingAndHostLocation()
	{
		var (actor, _, room, world) = ActorWithHand();
		var hostItem = Item(world, room);
		var moduleItem = Item(world);
		var host = new Mock<IAutomationMountHost>();
		host.SetupGet(x => x.Parent).Returns(hostItem.Object);
		var mountable = new Mock<IAutomationMountable>();
		mountable.SetupGet(x => x.MountHost).Returns(host.Object);
		moduleItem.Setup(x => x.GetItemType<IAutomationMountable>()).Returns(mountable.Object);
		var error = "The housing is closed.";
		host.Setup(x => x.CanAccessMounts(actor.Object, out error)).Returns(false);
		Assert.AreEqual((false, error), actor.Object.CanReachItem(moduleItem.Object));
		host.Setup(x => x.CanAccessMounts(actor.Object, out error)).Returns(true);
		Assert.IsTrue(actor.Object.CanReachItem(moduleItem.Object).Truth);
		hostItem.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		Assert.IsFalse(actor.Object.CanReachItem(moduleItem.Object).Truth);
	}

	[TestMethod]
	public void Inventory_GetWithNoHands_RejectsBeforeConsideringStackMerging()
	{
		var body = EmptyBody();
		SetField(body, "_holdlocs", new List<IGrab>());
		var item = new Mock<IGameItem>();
		Assert.IsFalse(body.CanGet(item.Object, 0));
		item.Verify(x => x.CanMerge(It.IsAny<IGameItem>()), Times.Never);
	}

	[TestMethod]
	public void Inventory_WeightRetrieval_DeniedSourceNeverSplitsOrTakes()
	{
		var (actor, _, room, world) = ActorWithHand();
		var body = EmptyBody();
		body.Actor = actor.Object;
		var source = Item(world, room);
		var item = Item(world);
		var container = new Mock<IContainer>();
		container.SetupGet(x => x.Contents).Returns([item.Object]);
		container.Setup(x => x.CanTake(actor.Object, item.Object, 0)).Returns(false);
		source.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
		Assert.IsFalse(body.CanGetByWeight(item.Object, source.Object, 1.0));
		body.GetByWeight(item.Object, source.Object, 1.0, silent: true);
		item.Verify(x => x.PeekSplitByWeight(It.IsAny<double>()), Times.Never);
		item.Verify(x => x.GetByWeight(It.IsAny<IBody>(), It.IsAny<double>()), Times.Never);
		container.Verify(x => x.Take(It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<int>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Inventory_ContainerRetrieval_RejectsMissingComponentOrUnrelatedSource(bool suppliedContainer)
	{
		var (actor, _, room, world) = ActorWithHand();
		var body = EmptyBody();
		body.Actor = actor.Object;
		var actualSource = Item(world, room);
		var unrelated = Item(world, room);
		var item = Item(world);
		item.SetupGet(x => x.ContainedIn).Returns(actualSource.Object);
		item.SetupGet(x => x.Quantity).Returns(1);
		var container = new Mock<IContainer>();
		container.SetupGet(x => x.Parent).Returns(unrelated.Object);
		container.SetupGet(x => x.Contents).Returns([]);
		if (suppliedContainer)
		{
			unrelated.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
		}

		Assert.IsFalse(body.CanGet(item.Object, unrelated.Object, 0));
		body.Get(item.Object, unrelated.Object, 0, silent: true);

		Assert.AreSame(actualSource.Object, item.Object.ContainedIn);
		container.Verify(x => x.Take(It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<int>()), Times.Never);
		item.Verify(x => x.Get(It.IsAny<IBody>()), Times.Never);
		item.VerifySet(x => x.ContainedIn = It.IsAny<IGameItem>(), Times.Never);
	}

	[TestMethod]
	public void Inventory_RoomWeightRetrieval_GuardedSourceNeverSplits()
	{
		var (actor, _, room, world) = ActorWithHand();
		var body = EmptyBody();
		body.Actor = actor.Object;
		var item = Item(world, room);
		room.Setup(x => x.CanGetAccess(item.Object, actor.Object)).Returns(false);
		Assert.IsFalse(body.CanGetByWeight(item.Object, 1.0));
		body.GetByWeight(item.Object, 1.0, silent: true);
		item.Verify(x => x.PeekSplitByWeight(It.IsAny<double>()), Times.Never);
		item.Verify(x => x.GetByWeight(It.IsAny<IBody>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CurrencyRetrieval_OriginalPilePickupRestriction_PreventsConsumption(bool fromContainer)
	{
		var (actor, _, room, world) = ActorWithHand();
		var body = EmptyBody();
		body.Actor = actor.Object;
		var hand = Mock.Of<IGrab>();
		SetField(body, "_holdlocs", new List<IGrab> { hand });
		SetField(body, "_bodyparts", new HashSet<IBodypart> { hand });
		typeof(Body).GetProperty(nameof(Body.EffectHandler))!.SetValue(body, Mock.Of<IEffectHandler>());
		var item = Item(world, fromContainer ? null : room);
		item.Setup(x => x.CanGet(0, It.IsAny<ItemCanGetIgnore>())).Returns(ItemGetResponse.NoGetEffect);
		var pile = new Mock<ICurrencyPile>();
		pile.SetupGet(x => x.Parent).Returns(item.Object);
		item.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(pile.Object);
		item.Setup(x => x.IsItemType<ICurrencyPile>()).Returns(true);
		room.Setup(x => x.LayerGameItems(It.IsAny<RoomLayer>())).Returns([item.Object]);
		room.Setup(x => x.CanGet(item.Object, actor.Object)).Returns(true);
		var containerItem = Item(world, room);
		var container = new Mock<IContainer>();
		container.SetupGet(x => x.Parent).Returns(containerItem.Object);
		container.SetupGet(x => x.Contents).Returns([item.Object]);
		container.Setup(x => x.CanTake(actor.Object, item.Object, 0)).Returns(true);
		containerItem.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
		if (fromContainer) item.SetupGet(x => x.ContainedIn).Returns(containerItem.Object);
		var coin = Mock.Of<ICoin>(x => x.Value == 1.0M);
		var currency = new Mock<ICurrency>();
		currency.Setup(x => x.FindCurrency(It.IsAny<IEnumerable<ICurrencyPile>>(), 1.0M))
			.Returns<IEnumerable<ICurrencyPile>, decimal>((piles, _) =>
				piles.ToDictionary(x => x, _ => new Dictionary<ICoin, int> { [coin] = 1 }));

		if (fromContainer)
		{
			Assert.IsFalse(body.CanGet(currency.Object, containerItem.Object, 1.0M, true));
			Assert.IsNull(body.Get(currency.Object, containerItem.Object, 1.0M, true, null, true, null!));
		}
		else
		{
			Assert.IsFalse(body.CanGet(currency.Object, 1.0M, true));
			Assert.IsNull(body.Get(currency.Object, 1.0M, true, null, true, null!));
		}

		item.Verify(x => x.CanGet(0, ItemCanGetIgnore.IgnoreWeight), Times.AtLeastOnce);
		pile.Verify(x => x.RemoveCoins(It.IsAny<IEnumerable<Tuple<ICoin, int>>>()), Times.Never);
		item.Verify(x => x.Delete(), Times.Never);
		container.Verify(x => x.Take(It.IsAny<ICharacter>(), item.Object, 0), Times.Never);
	}

	[TestMethod]
	public void Inventory_QuantityRetrieval_PreviewCannotBypassOriginalPickupRestriction()
	{
		var (actor, _, room, world) = ActorWithHand();
		var body = EmptyBody();
		body.Actor = actor.Object;
		var hand = Mock.Of<IGrab>();
		SetField(body, "_holdlocs", new List<IGrab> { hand });
		SetField(body, "_bodyparts", new HashSet<IBodypart> { hand });
		typeof(Body).GetProperty(nameof(Body.EffectHandler))!.SetValue(body, Mock.Of<IEffectHandler>());
		var item = Item(world, room);
		var preview = Item(world);
		item.Setup(x => x.CanGet(1, It.IsAny<ItemCanGetIgnore>())).Returns(ItemGetResponse.NoGetEffect);
		item.Setup(x => x.PeekSplit(1)).Returns(preview.Object);
		preview.Setup(x => x.CanGet(It.IsAny<int>(), It.IsAny<ItemCanGetIgnore>())).Returns(ItemGetResponse.CanGet);
		room.Setup(x => x.CanGet(item.Object, actor.Object)).Returns(true);

		Assert.IsFalse(body.CanGet(item.Object, 1));
		body.Get(item.Object, 1, null, true);

		item.Verify(x => x.CanGet(1, ItemCanGetIgnore.None), Times.AtLeastOnce);
		item.Verify(x => x.Get(It.IsAny<IBody>(), It.IsAny<int>()), Times.Never);
	}

	[TestMethod]
	public void Blowgun_HandlessActorCanFireButAnUnusableMouthCannot()
	{
		var (actor, body, room, world) = ActorWithHand();
		body.SetupGet(x => x.HoldLocs).Returns([]);
		var mouth = TestObjectFactory.CreateUninitialized<MouthProto>();
		body.SetupGet(x => x.Bodyparts).Returns([mouth]);
		body.SetupGet(x => x.IsBreathing).Returns(true);
		var item = Item(world, room);
		var weapon = new BlowgunGameItemComponent(
			TestObjectFactory.CreateUninitialized<BlowgunGameItemComponentProto>(), item.Object, true)
		{
			LoadedAmmo = Mock.Of<IAmmo>(),
			IsReadied = true
		};
		Assert.IsTrue(weapon.CanFire(actor.Object, Mock.Of<IPerceivable>()));
		body.Setup(x => x.CanUseBodypart(mouth)).Returns(CanUseBodypartResult.CantUsePartDamage);
		Assert.IsFalse(weapon.CanFire(actor.Object, Mock.Of<IPerceivable>()));
		StringAssert.Contains(weapon.WhyCannotFire(actor.Object, Mock.Of<IPerceivable>()), "functioning mouth");
	}

	[TestMethod]
	public void Switch_DirectRuntimeCall_RejectsHandLossAndLostAccessWithoutMutation()
	{
		var (actor, body, room, world) = ActorWithHand();
		var item = Item(world, room);
		actor.Setup(x => x.CanManipulateItem(item.Object)).Returns(() => actor.Object.CanReachItem(item.Object));
		var component = new ToggleSwitchGameItemComponent(
			TestObjectFactory.CreateUninitialized<ToggleSwitchGameItemComponentProto>(), item.Object, true);
		Assert.IsTrue(component.CanSwitch(actor.Object, "on"));
		body.SetupGet(x => x.HoldLocs).Returns([]);
		Assert.IsFalse(component.Switch(actor.Object, "on"));
		Assert.IsFalse(component.SwitchedOn);
		body.SetupGet(x => x.HoldLocs).Returns([Mock.Of<IGrab>()]);
		item.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		Assert.IsFalse(component.Switch(actor.Object, "on"));
		Assert.IsFalse(component.SwitchedOn);
		item.SetupGet(x => x.Location).Returns(room.Object);
		Assert.IsTrue(component.Switch(actor.Object, "on"));
		Assert.IsTrue(component.SwitchedOn);
	}

	[TestMethod]
	public void Telekinesis_BypassesHandsOnlyInsideEligibleOperation_AndRechecksBeforeExecution()
	{
		var (actor, body, room, world) = ActorWithHand();
		body.SetupGet(x => x.HoldLocs).Returns([]);
		var item = Item(world, room);
		var component = new ToggleSwitchGameItemComponent(
			TestObjectFactory.CreateUninitialized<ToggleSwitchGameItemComponentProto>(), item.Object, true);
		item.Setup(x => x.GetItemTypes<ISwitchable>()).Returns([component]);
		var eligible = true;
		Assert.IsTrue(TelekineticManipulation.TryPrepare(actor.Object, item.Object, "switch", new("on"),
			_ => eligible, out var execute, out _));
		Assert.IsFalse(component.CanSwitch(actor.Object, "on"));
		eligible = false;
		Assert.IsFalse(execute());
		Assert.IsFalse(component.SwitchedOn);
		eligible = true;
		Assert.IsTrue(execute());
		Assert.IsTrue(component.SwitchedOn);
		Assert.IsFalse(component.CanSwitch(actor.Object, "off"));
	}

	[TestMethod]
	public void MedicalContinuation_HandLossStopsBeforeTreatment()
	{
		var (actor, body, _, _) = ActorWithHand();
		body.SetupGet(x => x.HoldLocs).Returns([]);
		var effect = TestObjectFactory.CreateUninitialized<PerformingCPR>();
		typeof(Effect).GetProperty(nameof(Effect.Owner))!.SetValue(effect, actor.Object);
		typeof(CharacterAction).GetProperty(nameof(CharacterAction.CharacterOwner))!.SetValue(effect, actor.Object);
		effect.ExpireEffect();
		actor.Verify(x => x.RemoveEffect(effect, true), Times.Once);
	}

	private static (Mock<ICharacter> Actor, Mock<IBody> Body, Mock<IRoom> Room, Mock<IFuturemud> World) ActorWithHand()
	{
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		var room = new Mock<IRoom>();
		var world = new Mock<IFuturemud>();
		body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		body.SetupGet(x => x.HoldLocs).Returns([Mock.Of<IGrab>()]);
		body.Setup(x => x.CanUseBodypart(It.IsAny<IBodypart>())).Returns(CanUseBodypartResult.CanUse);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Location).Returns(room.Object);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>()))
			.Returns<IPerceivable>(other => actor.Object.Location == other.Location && actor.Object.RoomLayer == other.RoomLayer);
		room.Setup(x => x.CanGetAccess(It.IsAny<IGameItem>(), actor.Object)).Returns(true);
		return (actor, body, room, world);
	}

	private static Mock<IGameItem> Item(Mock<IFuturemud> world, Mock<IRoom>? room = null)
	{
		var item = new Mock<IGameItem>();
		item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(item.Object, other));
		item.SetupGet(x => x.Gameworld).Returns(world.Object);
		item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
		item.SetupGet(x => x.Location).Returns(room?.Object!);
		return item;
	}

	private static Body EmptyBody()
	{
		var body = TestObjectFactory.CreateUninitialized<Body>();
		typeof(Body).GetProperty(nameof(Body.Prototype))!.SetValue(body, Mock.Of<IBodyPrototype>());
		foreach (var field in new[] { "_bodyparts", "_organs", "_bones", "_limbs", "_prosthetics" })
		{
			var info = typeof(Body).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!;
			info.SetValue(body, Activator.CreateInstance(info.FieldType));
		}
		return body;
	}

	private static void SetField(Body body, string name, object value)
	{
		typeof(Body).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, value);
	}
}
