#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleTowServiceTests
{
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
		AddLink(20, root, sourcePoint, middle, targetPoint);
		AddLink(21, middle, middleSourcePoint, rear, targetPoint);
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
		AddLink(20, root, sourcePoint, target, targetPoint);
		AddLink(21, otherSource, otherSourcePoint, target, otherTargetPoint);
		var exit = CreateExit(location, destination, SizeCategory.Huge);

		var result = service.CanMoveTowTrain(root.Vehicle.Object, exit.Object, out _, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("A linked towed vehicle is also attached to another towing vehicle.", reason);
	}

	private static Mock<ICharacter> CreateActor()
	{
		var body = new Mock<IBody>();
		body.Setup(x => x.CanDrop(It.IsAny<IGameItem>(), 0)).Returns(true);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		return actor;
	}

	private static VehicleHarness CreateVehicle(long id, string name, ICell location, double weight = 10.0,
		SizeCategory size = SizeCategory.Normal)
	{
		var links = new List<IVehicleTowLink>();
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Weight).Returns(weight);
		exterior.SetupGet(x => x.Size).Returns(size);
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
		link.SetupGet(x => x.HitchItemId).Returns(hitchItemId);
		link.SetupGet(x => x.HitchItem).Returns(hitchItem!);
		link.SetupGet(x => x.WhyInvalid).Returns(string.Empty);
		link.SetupGet(x => x.IsDisabled).Returns(manuallyDisabled);
		source.Links.Add(link.Object);
		target.Links.Add(link.Object);
		return link;
	}

	private static void SetupCellExitProfile(VehicleHarness vehicle, ICharacter controller, double requiredPower = 0.0)
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(100);
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.RequiredPowerSpikeInWatts).Returns(requiredPower);
		vehicle.Prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		vehicle.Vehicle.SetupGet(x => x.Controller).Returns(controller);
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
