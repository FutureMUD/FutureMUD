#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleMovementStrategyTests
{
	[TestMethod]
	public void CanMove_WhenActorIsNotController_Fails()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var actor = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [], SizeCategory.Normal);
		var exit = CreateExit(vehicle.Location, SizeCategory.Normal);

		var result = strategy.CanMove(vehicle, actor.Object, exit, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("You must be in control of the vehicle to move it.", reason);
	}

	[TestMethod]
	public void CanMove_WhenVehicleHasNoCellExitProfile_Fails()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [], SizeCategory.Normal);
		var exit = CreateExit(vehicle.Location, SizeCategory.Normal);

		var result = strategy.CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("That vehicle cannot move through normal cell exits.", reason);
	}

	[TestMethod]
	public void CanMove_WhenExitIsTooSmall_Fails()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Huge);
		var exit = CreateExit(vehicle.Location, SizeCategory.Normal);

		var result = strategy.CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("That exit is too small for the vehicle.", reason);
	}

	[TestMethod]
	public void CanMove_WhenExitIsValid_Succeeds()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = strategy.CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsTrue(result);
		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void CanMove_WhenMovementProfileDisabledByDamage_Fails()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		var profile = vehicle.Prototype.MovementProfiles.Single();
		Mock.Get(vehicle)
		    .Setup(x => x.IsDisabledByDamage(VehicleDamageEffectTargetType.MovementProfile, profile.Id))
		    .Returns(true);
		Mock.Get(vehicle)
		    .Setup(x => x.DamageDisabledReason(VehicleDamageEffectTargetType.MovementProfile, profile.Id))
		    .Returns("engine bay is disabled");
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = strategy.CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("That movement profile is disabled because engine bay is disabled.", reason);
	}

	[TestMethod]
	public void CanMove_WhenTowPointDisabledByDamage_Fails()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		var target = CreateVehicle(new Mock<ICharacter>().Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		Mock.Get(target).SetupGet(x => x.Location).Returns(vehicle.Location);
		Mock.Get(target).SetupGet(x => x.RoomLayer).Returns(vehicle.RoomLayer);
		var sourcePoint = new Mock<IVehicleTowPointPrototype>();
		sourcePoint.SetupGet(x => x.Id).Returns(42);
		sourcePoint.SetupGet(x => x.Name).Returns("rear hitch");
		sourcePoint.SetupGet(x => x.TowType).Returns("pin");
		sourcePoint.SetupGet(x => x.CanTow).Returns(true);
		sourcePoint.SetupGet(x => x.MaximumTowedWeight).Returns(1000.0);
		var targetPoint = new Mock<IVehicleTowPointPrototype>();
		targetPoint.SetupGet(x => x.Id).Returns(43);
		targetPoint.SetupGet(x => x.Name).Returns("front ring");
		targetPoint.SetupGet(x => x.TowType).Returns("pin");
		targetPoint.SetupGet(x => x.CanBeTowed).Returns(true);
		var link = new Mock<IVehicleTowLink>();
		link.SetupGet(x => x.SourceVehicle).Returns(vehicle);
		link.SetupGet(x => x.TargetVehicle).Returns(target);
		link.SetupGet(x => x.SourceTowPoint).Returns(sourcePoint.Object);
		link.SetupGet(x => x.TargetTowPoint).Returns(targetPoint.Object);
		link.SetupGet(x => x.SourceVehicle).Returns(vehicle);
		link.SetupGet(x => x.IsDisabled).Returns(true);
		link.SetupGet(x => x.WhyInvalid).Returns(string.Empty);
		Mock.Get(vehicle).Setup(x => x.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, 42)).Returns(true);
		Mock.Get(vehicle).Setup(x => x.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, 42)).Returns("rear damage is disabled");
		Mock.Get(vehicle).SetupGet(x => x.TowLinks).Returns([link.Object]);
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = strategy.CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("Tow link #0 is invalid: rear hitch is disabled because rear damage is disabled.", reason);
	}

	[TestMethod]
	public void AccessPoint_WhenDisabledByDamage_BlocksUse()
	{
		var prototype = new Mock<IVehicleAccessPointPrototype>();
		prototype.SetupGet(x => x.Id).Returns(42);
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.AccessPoints).Returns([prototype.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.Setup(x => x.IsDisabledByDamage(VehicleDamageEffectTargetType.AccessPoint, 42)).Returns(true);
		vehicle.Setup(x => x.DamageDisabledReason(VehicleDamageEffectTargetType.AccessPoint, 42)).Returns("side door is disabled");
		var access = new VehicleAccessPoint(vehicle.Object, new DB.VehicleAccessPoint
		{
			Id = 1,
			Name = "Side Door",
			VehicleAccessPointProtoId = 42,
			IsOpen = true,
			IsDisabled = false
		});

		var result = access.CanUse(new Mock<ICharacter>().Object, out var reason);

		Assert.IsFalse(result);
		Assert.IsTrue(access.IsDisabled);
		Assert.IsFalse(access.IsManuallyDisabled);
		Assert.AreEqual("That access point is disabled because side door is disabled.", reason);
	}

	[TestMethod]
	public void CargoSpace_WhenDisabledByDamage_BlocksAccess()
	{
		var prototype = new Mock<IVehicleCargoSpacePrototype>();
		prototype.SetupGet(x => x.Id).Returns(51);
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.CargoSpaces).Returns([prototype.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.Setup(x => x.IsDisabledByDamage(VehicleDamageEffectTargetType.CargoSpace, 51)).Returns(true);
		vehicle.Setup(x => x.DamageDisabledReason(VehicleDamageEffectTargetType.CargoSpace, 51)).Returns("trunk is disabled");
		var cargo = new VehicleCargoSpace(vehicle.Object, new DB.VehicleCargoSpace
		{
			Id = 2,
			Name = "Trunk",
			VehicleCargoSpaceProtoId = 51,
			IsDisabled = false
		});

		var result = cargo.CanAccess(new Mock<ICharacter>().Object, out var reason);

		Assert.IsFalse(result);
		Assert.IsTrue(cargo.IsDisabled);
		Assert.IsFalse(cargo.IsManuallyDisabled);
		Assert.AreEqual("That cargo space is disabled because trunk is disabled.", reason);
	}

	[TestMethod]
	public void Installation_WhenDisabledByDamage_BlocksInstallWithoutManualFlag()
	{
		var prototype = new Mock<IVehicleInstallationPointPrototype>();
		prototype.SetupGet(x => x.Id).Returns(61);
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.InstallationPoints).Returns([prototype.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.Setup(x => x.IsDisabledByDamage(VehicleDamageEffectTargetType.InstallationPoint, 61)).Returns(true);
		vehicle.Setup(x => x.DamageDisabledReason(VehicleDamageEffectTargetType.InstallationPoint, 61)).Returns("engine mount is disabled");
		var installation = new VehicleInstallation(vehicle.Object, new DB.VehicleInstallation
		{
			Id = 3,
			VehicleInstallationPointProtoId = 61,
			IsDisabled = false
		});

		var result = installation.CanInstall(new Mock<ICharacter>().Object, new Mock<IGameItem>().Object, out var reason);

		Assert.IsFalse(result);
		Assert.IsTrue(installation.IsDisabled);
		Assert.IsFalse(installation.IsManuallyDisabled);
		Assert.AreEqual("That installation point is disabled because engine mount is disabled.", reason);
	}

	[TestMethod]
	public void RecoverInterruptedMovement_ClearsMovingTransitState()
	{
		var location = new Mock<ICell>();
		location.SetupGet(x => x.Id).Returns(42);

		var cells = new Mock<IUneditableAll<ICell>>();
		cells.Setup(x => x.Get(42)).Returns(location.Object);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Cells).Returns(cells.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);

		var vehicle = new Vehicle(new DB.Vehicle
		{
			Id = 1,
			Name = "Test Vehicle",
			VehicleProtoId = 10,
			VehicleProtoRevision = 0,
			LocationType = (int)VehicleLocationType.CellExitTransit,
			CurrentCellId = 42,
			CurrentRoomLayer = (int)RoomLayer.GroundLevel,
			MovementStatus = (int)VehicleMovementStatus.Moving,
			CurrentExitId = 99,
			DestinationCellId = 100
		}, gameworld.Object);

		vehicle.RecoverInterruptedMovement();

		Assert.AreEqual(VehicleLocationType.Cell, vehicle.MovementState.LocationType);
		Assert.AreEqual(VehicleMovementStatus.Stationary, vehicle.MovementState.MovementStatus);
		Assert.IsNull(vehicle.MovementState.CurrentExitId);
		Assert.IsNull(vehicle.MovementState.DestinationCellId);
		Assert.IsTrue(vehicle.Changed);
	}

	private static IVehicle CreateVehicle(ICharacter controller, IEnumerable<VehicleMovementProfileType> movementTypes, SizeCategory exteriorSize)
	{
		var location = new Mock<ICell>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Size).Returns(exteriorSize);

		var profiles = new List<IVehicleMovementProfilePrototype>();
		foreach (var movementType in movementTypes)
		{
			var profile = new Mock<IVehicleMovementProfilePrototype>();
			profile.SetupGet(x => x.Id).Returns(100 + profiles.Count);
			profile.SetupGet(x => x.MovementType).Returns(movementType);
			profiles.Add(profile.Object);
		}

		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns(profiles);

		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Controller).Returns(controller);
		vehicle.SetupGet(x => x.Location).Returns(location.Object);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(item.Object);
		return vehicle.Object;
	}

	private static ICellExit CreateExit(ICell origin, SizeCategory maximumSize)
	{
		var destination = new Mock<ICell>();
		var exit = new Mock<IExit>();
		exit.SetupGet(x => x.MaximumSizeToEnter).Returns(maximumSize);

		var cellExit = new Mock<ICellExit>();
		cellExit.SetupGet(x => x.Origin).Returns(origin);
		cellExit.SetupGet(x => x.Destination).Returns(destination.Object);
		cellExit.SetupGet(x => x.Exit).Returns(exit.Object);
		cellExit.Setup(x => x.MovementTransition(It.IsAny<ICharacter>()))
		        .Returns((CellMovementTransition.GroundToGround, RoomLayer.GroundLevel));
		return cellExit.Object;
	}
}
