#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Effects;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleTowServiceTests
{
	[TestMethod]
	public void VehicleTowPointPrototype_LoadsCharacterPullMultiplier()
	{
		var dbitem = new DB.VehicleTowPointProto
		{
			Id = 1,
			Name = "shafts",
			Description = "Cart shafts.",
			TowType = "hand",
			CanBeTowed = true,
			CharacterPullMultiplier = 4.5
		};

		var point = new VehicleTowPointPrototype(dbitem, Enumerable.Empty<IVehicleAccessPointPrototype>());

		Assert.AreEqual(4.5, point.CharacterPullMultiplier);

		dbitem.CharacterPullMultiplier = 0.0;
		var fallback = new VehicleTowPointPrototype(dbitem, Enumerable.Empty<IVehicleAccessPointPrototype>());

		Assert.AreEqual(1.0, fallback.CharacterPullMultiplier);
	}

	[TestMethod]
	public void CanHitch_WhenTargetAlreadyTowed_Fails()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location);
		var existingSource = CreateVehicle(2, "ute", location);
		var target = CreateVehicle(3, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		var existingTargetPoint = CreateTowPoint(13, "front ring", canTow: false, canBeTowed: true);
		AddLink(20, existingSource, sourcePoint, target, existingTargetPoint);

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, sourcePoint.Object, target.Vehicle.Object,
			targetPoint.Object, null, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("That target vehicle is already being towed.", reason);
	}

	[TestMethod]
	public void CanHitch_WhenCycleWouldBeCreated_Fails()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		AddLink(20, target, sourcePoint, source, targetPoint);

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, sourcePoint.Object, target.Vehicle.Object,
			targetPoint.Object, null, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("That hitch would create a tow cycle.", reason);
	}

	[TestMethod]
	public void CanHitch_WhenSourceTowPointAlreadyUsed_Fails()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location);
		var existingTarget = CreateVehicle(2, "trailer one", location);
		var target = CreateVehicle(3, "trailer two", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		AddLink(20, source, sourcePoint, existingTarget, targetPoint);

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, sourcePoint.Object, target.Vehicle.Object,
			targetPoint.Object, null, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("That source tow point is already in use.", reason);
	}

	[TestMethod]
	public void CanHitch_WhenRequiredTowAccessUnavailable_Fails()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var requiredAccessPrototype = new Mock<IVehicleAccessPointPrototype>();
		requiredAccessPrototype.SetupGet(x => x.Id).Returns(50);
		var liveAccess = new Mock<IVehicleAccessPoint>();
		liveAccess.SetupGet(x => x.Prototype).Returns(requiredAccessPrototype.Object);
		liveAccess.Setup(x => x.CanUse(actor.Object, out It.Ref<string>.IsAny))
		          .Returns((ICharacter _, out string reason) =>
		          {
			          reason = "That access point is closed.";
			          return false;
		          });
		source.Vehicle.SetupGet(x => x.AccessPoints).Returns([liveAccess.Object]);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false,
			requiredAccess: requiredAccessPrototype.Object);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, sourcePoint.Object, target.Vehicle.Object,
			targetPoint.Object, null, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("hitch is unavailable: That access point is closed.", reason);
	}

	[TestMethod]
	public void CanHitch_UsesRecursiveTargetTrainWeight()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location, weight: 20.0);
		var target = CreateVehicle(2, "trailer", location, weight: 30.0);
		var downstream = CreateVehicle(3, "cart", location, weight: 25.0);
		var limitedSourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false, maxWeight: 40.0);
		var sourcePoint = CreateTowPoint(12, "rear", canTow: true, canBeTowed: false, maxWeight: 100.0);
		var targetPoint = CreateTowPoint(13, "ring", canTow: false, canBeTowed: true);
		AddLink(20, target, sourcePoint, downstream, targetPoint);

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, limitedSourcePoint.Object,
			target.Vehicle.Object, targetPoint.Object, null, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual(55.0, service.TowTrainWeight(target.Vehicle.Object));
		StringAssert.Contains(reason, "target train weighs 55.00");
	}

	[TestMethod]
	public void CanHitch_NonDirectTowPointsWithoutHitchGear_Fails()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "yoke", canTow: true, canBeTowed: false, towType: "yoke");
		var targetPoint = CreateTowPoint(12, "yoke ring", canTow: false, canBeTowed: true, towType: "yoke");

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, sourcePoint.Object, target.Vehicle.Object,
			targetPoint.Object, null, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "require a hitch item");
	}

	[TestMethod]
	public void CanHitch_DirectTowPointsWithoutHitchGear_Succeeds()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hand bar", canTow: true, canBeTowed: false, towType: "hand");
		var targetPoint = CreateTowPoint(12, "hand ring", canTow: false, canBeTowed: true, towType: "hand");

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, sourcePoint.Object, target.Vehicle.Object,
			targetPoint.Object, null, out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void CanHitch_CompatibleHitchGear_Succeeds()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "yoke", canTow: true, canBeTowed: false, towType: "yoke");
		var targetPoint = CreateTowPoint(12, "yoke ring", canTow: false, canBeTowed: true, towType: "yoke");
		var hitchItem = CreateHitchItem(50, location, HitchGearRole.Yoke, actor.Object.Body);

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, sourcePoint.Object, target.Vehicle.Object,
			targetPoint.Object, hitchItem.Object, out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void CanHitch_IncompatibleHitchGear_Fails()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "yoke", canTow: true, canBeTowed: false, towType: "yoke");
		var targetPoint = CreateTowPoint(12, "yoke ring", canTow: false, canBeTowed: true, towType: "yoke");
		var hitchItem = CreateHitchItem(50, location, HitchGearRole.Chain, actor.Object.Body, "a chain");

		var result = service.CanHitch(actor.Object, source.Vehicle.Object, sourcePoint.Object, target.Vehicle.Object,
			targetPoint.Object, hitchItem.Object, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("a chain is not compatible with a yoke tow point.", reason);
	}

	[TestMethod]
	public void TowTrainFrom_ReturnsRecursiveTrain()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var root = CreateVehicle(1, "tractor", location);
		var middle = CreateVehicle(2, "trailer", location);
		var rear = CreateVehicle(3, "cart", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var middleSourcePoint = CreateTowPoint(12, "rear", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(13, "ring", canTow: false, canBeTowed: true);
		AddLink(20, root, sourcePoint, middle, targetPoint);
		AddLink(21, middle, middleSourcePoint, rear, targetPoint);

		var train = service.TowTrainFrom(root.Vehicle.Object);

		CollectionAssert.AreEqual(new[] { 1L, 2L, 3L }, train.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void ValidateLink_WhenNonDirectTowPointHasNoHitchItem_FailsSafely()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		var link = AddLink(20, source, sourcePoint, target, targetPoint);

		var result = service.ValidateLink(link.Object, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("the tow point requires a hitch item", reason);
	}

	[TestMethod]
	public void ValidateLink_WhenTowPointDisabledByDamage_FailsSafely()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		source.Vehicle.Setup(x => x.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, 11)).Returns(true);
		source.Vehicle.Setup(x => x.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, 11))
		      .Returns("rear hitch is destroyed");
		var link = AddLink(20, source, sourcePoint, target, targetPoint);

		var result = service.ValidateLink(link.Object, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("hitch is disabled because rear hitch is destroyed", reason);
	}

	[TestMethod]
	public void ValidateLink_WhenHitchItemMissing_FailsSafely()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		var link = AddLink(20, source, sourcePoint, target, targetPoint, hitchItemId: 99);

		var result = service.ValidateLink(link.Object, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("the hitch item is missing", reason);
	}

	[TestMethod]
	public void ValidateLink_WhenHitchItemDeletionDisabledLink_FailsSafely()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		var link = AddLink(20, source, sourcePoint, target, targetPoint, manuallyDisabled: true);

		var result = service.ValidateLink(link.Object, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("the link is manually disabled", reason);
	}

	[TestMethod]
	public void ValidateLink_WithCompatibleHitchItem_Succeeds()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var source = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		var hitchItem = CreateHitchItem(50, location, HitchGearRole.TowBar);
		var link = AddLink(20, source, sourcePoint, target, targetPoint, hitchItem: hitchItem.Object);

		var result = service.ValidateLink(link.Object, out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void VehicleTowLink_WhenLoadedWithMissingVehicle_IsInvalidWithoutThrowing()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.TryGetVehicle(1)).Returns((IVehicle)null!);

		var link = new VehicleTowLink(gameworld.Object, new DB.VehicleTowLink
		{
			Id = 10,
			SourceVehicleId = 1,
			TargetVehicleId = 2,
			SourceTowPointProtoId = 11,
			TargetTowPointProtoId = 12
		});

		Assert.IsTrue(link.IsBroken);
		Assert.IsTrue(link.IsDisabled);
		Assert.AreEqual("the source vehicle is missing", link.WhyInvalid);
	}

	[TestMethod]
	public void VehicleHitchService_CanPersistCharacterHitch_WithPlayerEndpoint_Fails()
	{
		var service = new VehicleHitchService();
		var source = new Mock<ICharacter>();
		source.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		var target = new Mock<ICharacter>();

		var result = service.CanPersistCharacterHitch(source.Object, target.Object, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("Player-character hitch endpoints remain transient and are not persisted.", reason);
	}

	[TestMethod]
	public void VehicleHitchLink_WhenLoadedWithMissingSourceCharacter_IsInvalidWithoutThrowing()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Characters).Returns(new All<ICharacter>());

		var link = new VehicleHitchLink(gameworld.Object, new DB.VehicleHitchLink
		{
			Id = 10,
			SourceType = (int)VehicleHitchEndpointType.Character,
			SourceCharacterId = 99,
			TargetType = (int)VehicleHitchEndpointType.Character,
			TargetCharacterId = 100
		});

		Assert.IsTrue(link.IsBroken);
		Assert.IsTrue(link.IsDisabled);
		Assert.AreEqual("the source character is missing", link.WhyInvalid);
	}

	[TestMethod]
	public void VehicleHitchLink_WithHitchItemWornByEndpoint_IsValid()
	{
		var location = new Mock<ICell>().Object;
		var hitchItem = new Mock<IGameItem>();
		hitchItem.SetupGet(x => x.Id).Returns(50);
		var sourceBody = new Mock<IBody>();
		sourceBody.SetupGet(x => x.ExternalItems).Returns([hitchItem.Object]);
		var targetBody = new Mock<IBody>();
		targetBody.SetupGet(x => x.ExternalItems).Returns([]);
		var source = new Mock<ICharacter>();
		source.SetupGet(x => x.Id).Returns(1);
		source.SetupGet(x => x.Location).Returns(location);
		source.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		source.SetupGet(x => x.Body).Returns(sourceBody.Object);
		var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Id).Returns(2);
		target.SetupGet(x => x.Location).Returns(location);
		target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		target.SetupGet(x => x.Body).Returns(targetBody.Object);
		var characters = new All<ICharacter>();
		characters.Add(source.Object);
		characters.Add(target.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Characters).Returns(characters);
		gameworld.Setup(x => x.TryGetItem(50, true)).Returns(hitchItem.Object);

		var link = new VehicleHitchLink(gameworld.Object, new DB.VehicleHitchLink
		{
			Id = 10,
			SourceType = (int)VehicleHitchEndpointType.Character,
			SourceCharacterId = 1,
			TargetType = (int)VehicleHitchEndpointType.Character,
			TargetCharacterId = 2,
			HitchItemId = 50
		});

		Assert.IsFalse(link.IsBroken, link.WhyInvalid);
		Assert.AreEqual(string.Empty, link.WhyInvalid);
	}

	[TestMethod]
	public void VehicleHitchLink_WithHitchItemHeldByNonEndpoint_IsInvalid()
	{
		var location = new Mock<ICell>().Object;
		var bystanderBody = new Mock<IBody>().Object;
		var hitchItem = new Mock<IGameItem>();
		hitchItem.SetupGet(x => x.Id).Returns(50);
		hitchItem.SetupGet(x => x.Location).Returns(location);
		hitchItem.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		hitchItem.SetupGet(x => x.InInventoryOf).Returns(bystanderBody);
		var sourceBody = new Mock<IBody>();
		sourceBody.SetupGet(x => x.ExternalItems).Returns([]);
		var targetBody = new Mock<IBody>();
		targetBody.SetupGet(x => x.ExternalItems).Returns([]);
		var source = new Mock<ICharacter>();
		source.SetupGet(x => x.Id).Returns(1);
		source.SetupGet(x => x.Location).Returns(location);
		source.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		source.SetupGet(x => x.Body).Returns(sourceBody.Object);
		var target = new Mock<ICharacter>();
		target.SetupGet(x => x.Id).Returns(2);
		target.SetupGet(x => x.Location).Returns(location);
		target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		target.SetupGet(x => x.Body).Returns(targetBody.Object);
		var characters = new All<ICharacter>();
		characters.Add(source.Object);
		characters.Add(target.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Characters).Returns(characters);
		gameworld.Setup(x => x.TryGetItem(50, true)).Returns(hitchItem.Object);

		var link = new VehicleHitchLink(gameworld.Object, new DB.VehicleHitchLink
		{
			Id = 10,
			SourceType = (int)VehicleHitchEndpointType.Character,
			SourceCharacterId = 1,
			TargetType = (int)VehicleHitchEndpointType.Character,
			TargetCharacterId = 2,
			HitchItemId = 50
		});

		Assert.IsTrue(link.IsBroken);
		Assert.AreEqual("the hitch item is not with the hitch chain", link.WhyInvalid);
	}

	[TestMethod]
	public void CharacterHitch_RemovalEffect_DoesNotDeletePersistentLink()
	{
		var gameworld = new Mock<IFuturemud>();
		var owner = new Mock<ICharacter>();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var target = new Mock<IPerceivable>();
		target.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var effect = new CharacterHitch(owner.Object, target.Object, 1.0, vehicleHitchLinkId: 10);

		effect.RemovalEffect();

		gameworld.Verify(x => x.Destroy(It.IsAny<IVehicleHitchLink>()), Times.Never);
	}

	[TestMethod]
	public void Move_WithThreeVehicleTrain_MovesAllVehicles()
	{
		var service = new VehicleTowService();
		var strategy = new CellExitVehicleMovementStrategy(service);
		var location = new Mock<ICell>().Object;
		var destination = new Mock<ICell>().Object;
		var controller = new Mock<ICharacter>().Object;
		var root = CreateVehicle(1, "tractor", location);
		var middle = CreateVehicle(2, "trailer", location);
		var rear = CreateVehicle(3, "cart", location);
		SetupCellExitProfile(root, controller);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var middleSourcePoint = CreateTowPoint(12, "rear", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(13, "ring", canTow: false, canBeTowed: true);
		AddLink(20, root, sourcePoint, middle, targetPoint,
			hitchItem: CreateHitchItem(50, location, HitchGearRole.TowBar).Object);
		AddLink(21, middle, middleSourcePoint, rear, targetPoint,
			hitchItem: CreateHitchItem(51, location, HitchGearRole.TowBar).Object);
		var exit = CreateExit(location, destination, SizeCategory.Huge);

		var result = strategy.Move(root.Vehicle.Object, controller, exit.Object);

		Assert.IsTrue(result);
		root.Vehicle.Verify(x => x.MoveToCell(destination, RoomLayer.GroundLevel, exit.Object), Times.Once);
		middle.Vehicle.Verify(x => x.MoveToCell(destination, RoomLayer.GroundLevel, exit.Object), Times.Once);
		rear.Vehicle.Verify(x => x.MoveToCell(destination, RoomLayer.GroundLevel, exit.Object), Times.Once);
	}

	[TestMethod]
	public void Move_WithInvalidTowTrain_BlocksBeforePowerConsumption()
	{
		var strategy = new CellExitVehicleMovementStrategy(new VehicleTowService());
		var location = new Mock<ICell>().Object;
		var destination = new Mock<ICell>().Object;
		var controller = new Mock<ICharacter>().Object;
		var root = CreateVehicle(1, "tractor", location);
		var target = CreateVehicle(2, "trailer", location);
		SetupCellExitProfile(root, controller, requiredPower: 100.0);
		var power = new Mock<IProducePower>();
		power.Setup(x => x.CanDrawdownSpike(100.0)).Returns(true);
		var installedItem = new Mock<IGameItem>();
		installedItem.Setup(x => x.GetItemType<IProducePower>()).Returns(power.Object);
		var installation = new Mock<IVehicleInstallation>();
		installation.SetupGet(x => x.InstalledItem).Returns(installedItem.Object);
		root.Vehicle.SetupGet(x => x.Installations).Returns([installation.Object]);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(12, "ring", canTow: false, canBeTowed: true);
		AddLink(20, root, sourcePoint, target, targetPoint, manuallyDisabled: true);
		var exit = CreateExit(location, destination, SizeCategory.Huge);

		var result = strategy.Move(root.Vehicle.Object, controller, exit.Object);

		Assert.IsFalse(result);
		power.Verify(x => x.CanDrawdownSpike(It.IsAny<double>()), Times.Never);
		power.Verify(x => x.DrawdownSpike(It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void CanMoveTowTrain_WhenLinkedTargetHasUnexpectedIncomingLink_Fails()
	{
		var service = new VehicleTowService();
		var location = new Mock<ICell>().Object;
		var destination = new Mock<ICell>().Object;
		var root = CreateVehicle(1, "tractor", location);
		var otherSource = CreateVehicle(2, "ute", location);
		var target = CreateVehicle(3, "trailer", location);
		var sourcePoint = CreateTowPoint(11, "hitch", canTow: true, canBeTowed: false);
		var otherSourcePoint = CreateTowPoint(12, "rear hitch", canTow: true, canBeTowed: false);
		var targetPoint = CreateTowPoint(13, "ring", canTow: false, canBeTowed: true);
		var otherTargetPoint = CreateTowPoint(14, "front ring", canTow: false, canBeTowed: true);
		AddLink(20, root, sourcePoint, target, targetPoint,
			hitchItem: CreateHitchItem(50, location, HitchGearRole.TowBar).Object);
		AddLink(21, otherSource, otherSourcePoint, target, otherTargetPoint,
			hitchItem: CreateHitchItem(51, location, HitchGearRole.TowBar).Object);
		var exit = CreateExit(location, destination, SizeCategory.Huge);

		var result = service.CanMoveTowTrain(root.Vehicle.Object, exit.Object, out _, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("A linked towed vehicle is also attached to another towing vehicle.", reason);
	}

	[TestMethod]
	public void CanAddCharacterVehicleHitch_WithVehicleTowTrain_UsesRecursiveTrainAndSucceeds()
	{
		var graph = new VehicleHitchGraphService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		actor.SetupGet(x => x.Id).Returns(100);
		actor.SetupGet(x => x.Location).Returns(location);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var cart = CreateVehicle(1, "cart", location, weight: 30.0);
		var trailer = CreateVehicle(2, "trailer", location, weight: 25.0);
		var characterTowPoint = CreateTowPoint(11, "shafts", canTow: false, canBeTowed: true,
			maxWeight: 100.0, towType: "hand");
		var cartRearPoint = CreateTowPoint(12, "rear hitch", canTow: true, canBeTowed: false,
			maxWeight: 100.0, towType: "pin");
		var trailerPoint = CreateTowPoint(13, "front ring", canTow: false, canBeTowed: true,
			maxWeight: 100.0, towType: "pin");
		cart.Prototype.SetupGet(x => x.TowPoints).Returns([characterTowPoint.Object, cartRearPoint.Object]);
		trailer.Prototype.SetupGet(x => x.TowPoints).Returns([trailerPoint.Object]);
		AddLink(20, cart, cartRearPoint, trailer, trailerPoint,
			hitchItem: CreateHitchItem(50, location, HitchGearRole.TowBar).Object);

		var result = graph.CanAddCharacterVehicleHitch(actor.Object, actor.Object, cart.Vehicle.Object,
			characterTowPoint.Object, null, null, out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void CanAddCharacterVehicleHitch_WithVehicleTowTrain_UsesTrainWeightForTowPointLimit()
	{
		var graph = new VehicleHitchGraphService();
		var location = new Mock<ICell>().Object;
		var actor = CreateActor();
		actor.SetupGet(x => x.Id).Returns(100);
		actor.SetupGet(x => x.Location).Returns(location);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var cart = CreateVehicle(1, "cart", location, weight: 30.0);
		var trailer = CreateVehicle(2, "trailer", location, weight: 25.0);
		var characterTowPoint = CreateTowPoint(11, "shafts", canTow: false, canBeTowed: true,
			maxWeight: 40.0, towType: "hand");
		var cartRearPoint = CreateTowPoint(12, "rear hitch", canTow: true, canBeTowed: false,
			maxWeight: 100.0, towType: "pin");
		var trailerPoint = CreateTowPoint(13, "front ring", canTow: false, canBeTowed: true,
			maxWeight: 100.0, towType: "pin");
		cart.Prototype.SetupGet(x => x.TowPoints).Returns([characterTowPoint.Object, cartRearPoint.Object]);
		trailer.Prototype.SetupGet(x => x.TowPoints).Returns([trailerPoint.Object]);
		AddLink(20, cart, cartRearPoint, trailer, trailerPoint,
			hitchItem: CreateHitchItem(50, location, HitchGearRole.TowBar).Object);

		var result = graph.CanAddCharacterVehicleHitch(actor.Object, actor.Object, cart.Vehicle.Object,
			characterTowPoint.Object, null, null, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "target train weighs 55.00");
	}

	[TestMethod]
	public void CanDragVehicleTrain_WithIncomingCharacterHitch_MovesDownstreamVehiclesAndHitchItems()
	{
		var graph = new VehicleHitchGraphService();
		var location = new Mock<ICell>().Object;
		var destination = new Mock<ICell>();
		var gameworld = new Mock<IFuturemud>();
		var cart = CreateVehicle(1, "cart", location, weight: 30.0, gameworld: gameworld.Object);
		var trailer = CreateVehicle(2, "trailer", location, weight: 25.0, gameworld: gameworld.Object);
		var characterTowPoint = CreateTowPoint(11, "shafts", canTow: false, canBeTowed: true,
			maxWeight: 100.0, towType: "hand");
		var cartRearPoint = CreateTowPoint(12, "rear hitch", canTow: true, canBeTowed: false,
			maxWeight: 100.0, towType: "pin");
		var trailerPoint = CreateTowPoint(13, "front ring", canTow: false, canBeTowed: true,
			maxWeight: 100.0, towType: "pin");
		cart.Prototype.SetupGet(x => x.TowPoints).Returns([characterTowPoint.Object, cartRearPoint.Object]);
		trailer.Prototype.SetupGet(x => x.TowPoints).Returns([trailerPoint.Object]);
		var legacyHitchItem = CreateHitchItem(51, location, HitchGearRole.TowBar);
		AddLink(20, cart, cartRearPoint, trailer, trailerPoint, hitchItem: legacyHitchItem.Object);
		var incomingHitchItem = CreateHitchItem(50, location, HitchGearRole.Rope);
		var source = CreateActor();
		source.SetupGet(x => x.Id).Returns(100);
		source.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		source.SetupGet(x => x.Location).Returns(location);
		source.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var hitch = new CharacterHitch(source.Object, cart.Vehicle.Object.ExteriorItem, 1.0,
			characterTowPoint.Object.Id, hitchItemId: incomingHitchItem.Object.Id);
		var hitches = new[] { hitch };
		source.Setup(x => x.EffectsOfType<CharacterHitch>(It.IsAny<Predicate<CharacterHitch>>()))
		      .Returns((Predicate<CharacterHitch> predicate) =>
			      predicate is null ? hitches : hitches.Where(x => predicate(x)));
		source.Setup(x => x.EffectsOfType<Dragging>(It.IsAny<Predicate<Dragging>>()))
		      .Returns(Enumerable.Empty<Dragging>());
		var actors = new All<ICharacter> { source.Object };
		var vehicles = new All<IVehicle> { cart.Vehicle.Object, trailer.Vehicle.Object };
		gameworld.SetupGet(x => x.Actors).Returns(actors);
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles);
		gameworld.Setup(x => x.TryGetItem(incomingHitchItem.Object.Id, true)).Returns(incomingHitchItem.Object);
		var exit = CreateExit(location, destination.Object, SizeCategory.Huge);

		var result = graph.CanDragVehicleTrain(gameworld.Object, cart.Vehicle.Object, exit.Object,
			[source.Object], out var plan, out var reason);

		Assert.IsTrue(result, reason);
		CollectionAssert.AreEqual(new[] { 1L, 2L }, plan.Vehicles.Select(x => x.Id).ToArray());
		Assert.IsTrue(plan.Links.Any(x => x.Kind == VehicleHitchGraphLinkKind.TransientCharacterHitch));
		Assert.IsTrue(plan.Links.Any(x => x.Kind == VehicleHitchGraphLinkKind.LegacyVehicleTow));
		CollectionAssert.AreEquivalent(new[] { 50L, 51L }, plan.HitchItems.Select(x => x.Id).ToArray());

		graph.CompleteVehicleTrainMove(plan, destination.Object, RoomLayer.GroundLevel, exit.Object, null,
			cart.Vehicle.Object);

		cart.Vehicle.Verify(x => x.MoveToCell(It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<ICellExit>(),
			It.IsAny<IMovement>()), Times.Never);
		trailer.Vehicle.Verify(x => x.MoveToCell(destination.Object, RoomLayer.GroundLevel, exit.Object, null),
			Times.Once);
		destination.Verify(x => x.Insert(incomingHitchItem.Object, true), Times.Once);
		destination.Verify(x => x.Insert(legacyHitchItem.Object, true), Times.Once);
	}

	[TestMethod]
	public void CanDragVehicleTrain_SurfaceWaterVehicleTowardLand_Fails()
	{
		var graph = new VehicleHitchGraphService();
		var location = new Mock<ICell>();
		location.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(true);
		var destination = new Mock<ICell>();
		var gameworld = new Mock<IFuturemud>();
		var craft = CreateVehicle(1, "surfboard", location.Object, gameworld: gameworld.Object);
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		craft.Vehicle.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		var puller = CreateActor();
		puller.SetupGet(x => x.Id).Returns(100);
		puller.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		puller.SetupGet(x => x.Location).Returns(location.Object);
		puller.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		puller.Setup(x => x.EffectsOfType<CharacterHitch>(It.IsAny<Predicate<CharacterHitch>>()))
		      .Returns([]);
		puller.Setup(x => x.EffectsOfType<Dragging>(It.IsAny<Predicate<Dragging>>()))
		      .Returns([]);
		gameworld.SetupGet(x => x.Actors).Returns(new All<ICharacter> { puller.Object });
		gameworld.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { craft.Vehicle.Object });
		var exit = CreateExit(location.Object, destination.Object, SizeCategory.Huge);

		var result = graph.CanDragVehicleTrain(gameworld.Object, craft.Vehicle.Object, exit.Object,
			[puller.Object], out _, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "only move to another surface-water location");
	}

	[TestMethod]
	public void CanMoveVehicleTrain_SurfaceWaterTowedMemberTowardLand_Fails()
	{
		var graph = new VehicleHitchGraphService();
		var location = new Mock<ICell>();
		location.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(true);
		var destination = new Mock<ICell>();
		var gameworld = new Mock<IFuturemud>();
		var tug = CreateVehicle(1, "tug", location.Object, gameworld: gameworld.Object);
		var boat = CreateVehicle(2, "boat", location.Object, gameworld: gameworld.Object);
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		boat.Vehicle.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		var tugPoint = CreateTowPoint(11, "stern hitch", canTow: true, canBeTowed: false);
		var boatPoint = CreateTowPoint(12, "bow ring", canTow: false, canBeTowed: true);
		AddLink(20, tug, tugPoint, boat, boatPoint);
		gameworld.SetupGet(x => x.Actors).Returns(new All<ICharacter>());
		gameworld.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { tug.Vehicle.Object, boat.Vehicle.Object });
		var exit = CreateExit(location.Object, destination.Object, SizeCategory.Huge);

		var result = graph.CanMoveVehicleTrain(gameworld.Object, tug.Vehicle.Object, exit.Object,
			out _, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "boat can only move to another surface-water location");
	}

	[TestMethod]
	public void ApplyCharacterHitch_TransientActorHeldGear_DropsGearBeforeGraphValidation()
	{
		var graph = new VehicleHitchGraphService();
		var location = new Mock<ICell>().Object;
		var destination = new Mock<ICell>().Object;
		var gameworld = new Mock<IFuturemud>();
		var output = new Mock<IOutputHandler>();
		var actor = CreateActor();
		output.SetupGet(x => x.Perceiver).Returns(actor.Object);
		actor.SetupGet(x => x.Id).Returns(100);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Location).Returns(location);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);

		var source = CreateActor();
		source.SetupGet(x => x.Id).Returns(101);
		source.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		source.SetupGet(x => x.Location).Returns(location);
		source.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		source.SetupGet(x => x.PersistencePolicy).Returns(CharacterInstancePersistencePolicy.DespawnOnReboot);
		var sourceEffects = new List<IEffect>();
		source.Setup(x => x.AddEffect(It.IsAny<IEffect>()))
		      .Callback<IEffect>(sourceEffects.Add);
		source.Setup(x => x.EffectsOfType<CharacterHitch>(It.IsAny<Predicate<CharacterHitch>>()))
		      .Returns((Predicate<CharacterHitch> predicate) =>
			      sourceEffects.OfType<CharacterHitch>().Where(x => predicate?.Invoke(x) ?? true));
		source.Setup(x => x.EffectsOfType<Dragging>(It.IsAny<Predicate<Dragging>>()))
		      .Returns((Predicate<Dragging> predicate) =>
			      sourceEffects.OfType<Dragging>().Where(x => predicate?.Invoke(x) ?? true));

		var cart = CreateVehicle(1, "cart", location, weight: 30.0, gameworld: gameworld.Object);
		var targetPoint = CreateTowPoint(11, "shafts", canTow: false, canBeTowed: true,
			maxWeight: 100.0, towType: "rope");
		cart.Prototype.SetupGet(x => x.TowPoints).Returns([targetPoint.Object]);

		var gear = new Mock<IHitchGear>();
		gear.SetupGet(x => x.Roles).Returns(HitchGearRole.Rope);
		gear.SetupGet(x => x.MaximumTowedWeight).Returns(100.0);
		gear.SetupGet(x => x.EffortMultiplier).Returns(1.0);
		gear.SetupGet(x => x.MaximumUsers).Returns(1);
		IBody? carriedBy = actor.Object.Body;
		var hitchItem = new Mock<IGameItem>();
		hitchItem.SetupGet(x => x.Id).Returns(50);
		hitchItem.SetupGet(x => x.Name).Returns("a rope");
		hitchItem.SetupGet(x => x.Location).Returns(location);
		hitchItem.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		hitchItem.SetupGet(x => x.InInventoryOf).Returns(() => carriedBy!);
		hitchItem.Setup(x => x.GetItemType<IHitchGear>()).Returns(gear.Object);
		hitchItem.Setup(x => x.GetItemType<IDragAid>()).Returns(gear.Object);
		hitchItem.Setup(x => x.AffectedBy<HitchGearInUse>()).Returns(false);
		hitchItem.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
			It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("a rope");
		var actorBody = Mock.Get(actor.Object.Body);
		actorBody.SetupGet(x => x.ExternalItems)
		         .Returns(() => carriedBy == actor.Object.Body
			         ? new[] { hitchItem.Object }
			         : Enumerable.Empty<IGameItem>());
		actorBody.Setup(x => x.Drop(hitchItem.Object, 0, false, null, true))
		         .Callback(() => carriedBy = null);

		var actors = new All<ICharacter> { source.Object };
		var vehicles = new All<IVehicle> { cart.Vehicle.Object };
		gameworld.SetupGet(x => x.Actors).Returns(actors);
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles);
		gameworld.Setup(x => x.TryGetItem(hitchItem.Object.Id, true)).Returns(hitchItem.Object);
		var method = typeof(VehicleModule).GetMethod("ApplyCharacterHitch",
			BindingFlags.Static | BindingFlags.NonPublic)!;

		method.Invoke(null, [actor.Object, source.Object, cart.Vehicle.Object.ExteriorItem, cart.Vehicle.Object,
			targetPoint.Object, hitchItem.Object, gear.Object]);

		actorBody.Verify(x => x.Drop(hitchItem.Object, 0, false, null, true), Times.Once);
		Assert.IsNull(carriedBy);
		var exit = CreateExit(location, destination, SizeCategory.Huge);
		var result = graph.CanDragVehicleTrain(gameworld.Object, cart.Vehicle.Object, exit.Object,
			[source.Object], out _, out var reason);

		Assert.IsTrue(result, reason);
	}

	private static Mock<ICharacter> CreateActor()
	{
		var body = new Mock<IBody>();
		body.Setup(x => x.CanDrop(It.IsAny<IGameItem>(), 0)).Returns(true);
		body.SetupGet(x => x.ExternalItems).Returns([]);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.MaximumDragWeight).Returns(1000.0);
		actor.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
			It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("a test actor");
		return actor;
	}

	private static VehicleHarness CreateVehicle(long id, string name, ICell location, double weight = 10.0,
		SizeCategory size = SizeCategory.Normal, IFuturemud? gameworld = null)
	{
		var links = new List<IVehicleTowLink>();
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Id).Returns(id * 100L);
		exterior.SetupGet(x => x.Weight).Returns(weight);
		exterior.SetupGet(x => x.Size).Returns(size);
		exterior.SetupGet(x => x.Location).Returns(location);
		exterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		exterior.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
			It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(name);

		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns([]);
		prototype.SetupGet(x => x.TowPoints).Returns([]);

		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(id);
		vehicle.SetupGet(x => x.Name).Returns(name);
		vehicle.SetupGet(x => x.Location).Returns(location);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.TowLinks).Returns(links);
		vehicle.SetupGet(x => x.AccessPoints).Returns([]);
		vehicle.SetupGet(x => x.Installations).Returns([]);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		if (gameworld is not null)
		{
			vehicle.SetupGet(x => x.Gameworld).Returns(gameworld);
		}

		var exteriorComponent = new Mock<IVehicleExterior>();
		exteriorComponent.SetupGet(x => x.Vehicle).Returns(vehicle.Object);
		exterior.Setup(x => x.GetItemType<IVehicleExterior>()).Returns(exteriorComponent.Object);
		vehicle.Setup(x => x.DamageDisabledReason(It.IsAny<VehicleDamageEffectTargetType>(), It.IsAny<long?>()))
		       .Returns(string.Empty);
		return new VehicleHarness(vehicle, links, prototype);
	}

	private static Mock<IVehicleTowPointPrototype> CreateTowPoint(long id, string name, bool canTow, bool canBeTowed,
		double maxWeight = 1000.0, string towType = "pin", IVehicleAccessPointPrototype? requiredAccess = null)
	{
		var point = new Mock<IVehicleTowPointPrototype>();
		point.SetupGet(x => x.Id).Returns(id);
		point.SetupGet(x => x.Name).Returns(name);
		point.SetupGet(x => x.TowType).Returns(towType);
		point.SetupGet(x => x.CanTow).Returns(canTow);
		point.SetupGet(x => x.CanBeTowed).Returns(canBeTowed);
		point.SetupGet(x => x.MaximumTowedWeight).Returns(maxWeight);
		point.SetupGet(x => x.CharacterPullMultiplier).Returns(1.0);
		point.SetupGet(x => x.RequiredAccessPoint).Returns(requiredAccess!);
		return point;
	}

	private static Mock<IVehicleTowLink> AddLink(long id, VehicleHarness source,
		Mock<IVehicleTowPointPrototype> sourcePoint, VehicleHarness target,
		Mock<IVehicleTowPointPrototype> targetPoint, bool manuallyDisabled = false, long? hitchItemId = null,
		IGameItem? hitchItem = null)
	{
		var link = new Mock<IVehicleTowLink>();
		link.SetupGet(x => x.Id).Returns(id);
		link.SetupGet(x => x.SourceVehicle).Returns(source.Vehicle.Object);
		link.SetupGet(x => x.TargetVehicle).Returns(target.Vehicle.Object);
		link.SetupGet(x => x.SourceTowPoint).Returns(sourcePoint.Object);
		link.SetupGet(x => x.TargetTowPoint).Returns(targetPoint.Object);
		link.SetupGet(x => x.SourceVehicleId).Returns(source.Vehicle.Object.Id);
		link.SetupGet(x => x.TargetVehicleId).Returns(target.Vehicle.Object.Id);
		link.SetupGet(x => x.SourceTowPointPrototypeId).Returns(sourcePoint.Object.Id);
		link.SetupGet(x => x.TargetTowPointPrototypeId).Returns(targetPoint.Object.Id);
		link.SetupGet(x => x.IsManuallyDisabled).Returns(manuallyDisabled);
		var resolvedHitchItemId = hitchItemId ?? hitchItem?.Id;
		link.SetupGet(x => x.HitchItemId).Returns(resolvedHitchItemId);
		link.SetupGet(x => x.HitchItem).Returns(hitchItem!);
		link.SetupGet(x => x.WhyInvalid).Returns(string.Empty);
		link.SetupGet(x => x.IsDisabled).Returns(manuallyDisabled);
		source.Links.Add(link.Object);
		target.Links.Add(link.Object);
		return link;
	}

	private static Mock<IGameItem> CreateHitchItem(long id, ICell location, HitchGearRole roles,
		IBody? carriedBy = null, string name = "a hitch item", double maximumTowedWeight = 1000.0)
	{
		var gear = new Mock<IHitchGear>();
		gear.SetupGet(x => x.Roles).Returns(roles);
		gear.SetupGet(x => x.MaximumTowedWeight).Returns(maximumTowedWeight);
		gear.SetupGet(x => x.EffortMultiplier).Returns(1.0);
		gear.SetupGet(x => x.MaximumUsers).Returns(1);

		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(name);
		item.SetupGet(x => x.Location).Returns(location);
		item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		item.SetupGet(x => x.InInventoryOf).Returns(carriedBy!);
		item.Setup(x => x.GetItemType<IHitchGear>()).Returns(gear.Object);
		item.Setup(x => x.GetItemType<IDragAid>()).Returns(gear.Object);
		item.Setup(x => x.AffectedBy<HitchGearInUse>()).Returns(false);
		item.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
			It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(name);
		return item;
	}

	private static void SetupCellExitProfile(VehicleHarness vehicle, ICharacter controller, double requiredPower = 0.0)
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(100);
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.RequiredPowerSpikeInWatts).Returns(requiredPower);
		vehicle.Prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		vehicle.Vehicle.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		vehicle.Vehicle.SetupGet(x => x.Controller).Returns(controller);
		Mock.Get(controller).SetupGet(x => x.Location).Returns(vehicle.Vehicle.Object.Location);
		Mock.Get(controller).SetupGet(x => x.RoomLayer).Returns(vehicle.Vehicle.Object.RoomLayer);
	}

	private static Mock<ICellExit> CreateExit(ICell origin, ICell destination, SizeCategory maximumSize)
	{
		var exit = new Mock<IExit>();
		exit.SetupGet(x => x.MaximumSizeToEnter).Returns(maximumSize);
		var cellExit = new Mock<ICellExit>();
		cellExit.SetupGet(x => x.Origin).Returns(origin);
		cellExit.SetupGet(x => x.Destination).Returns(destination);
		cellExit.SetupGet(x => x.Exit).Returns(exit.Object);
		cellExit.SetupGet(x => x.OutboundMovementSuffix).Returns("north");
		cellExit.SetupGet(x => x.InboundMovementSuffix).Returns("from the south");
		cellExit.Setup(x => x.MovementTransition(It.IsAny<ICharacter>()))
		        .Returns((CellMovementTransition.GroundToGround, RoomLayer.GroundLevel));
		return cellExit;
	}

	private sealed record VehicleHarness(Mock<IVehicle> Vehicle, List<IVehicleTowLink> Links,
		Mock<IVehiclePrototype> Prototype);
}
