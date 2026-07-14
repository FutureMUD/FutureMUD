#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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
	public void CanMove_WhenActorIsInDifferentLocation_Fails()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		var actorLocation = new Mock<ICell>();
		controller.SetupGet(x => x.Location).Returns(actorLocation.Object);
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = strategy.CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("You must be in the same location as the vehicle to move it.", reason);
	}

	[TestMethod]
	public void CanMove_WhenActorIsOnDifferentRoomLayer_Fails()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		controller.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = strategy.CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("You must be on the same room layer as the vehicle to move it.", reason);
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
	public void CanMove_WhenExteriorItemPreventsMovement_Fails()
	{
		var strategy = new CellExitVehicleMovementStrategy();
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		Mock.Get(vehicle.ExteriorItem).Setup(x => x.PreventsMovement()).Returns(true);
		Mock.Get(vehicle.ExteriorItem).Setup(x => x.WhyPreventsMovement(controller.Object))
		    .Returns("the charging lead is still connected");
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = strategy.CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("the charging lead is still connected", reason);
	}

	[TestMethod]
	public void CanMove_WhenExteriorProjectionIsMissing_FailsClosed()
	{
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		Mock.Get(vehicle).SetupGet(x => x.ExteriorItem).Returns((IGameItem)null!);
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = new CellExitVehicleMovementStrategy().CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "intact linked exterior");
	}

	[TestMethod]
	public void CanMove_WhenControllerIsInCombat_Fails()
	{
		var controller = new Mock<ICharacter>();
		controller.SetupGet(x => x.Combat).Returns(new Mock<ICombat>().Object);
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = new CellExitVehicleMovementStrategy().CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "combat");
	}

	[TestMethod]
	public void CanMove_WhenRequiredCrewSlotIsEmpty_Fails()
	{
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		var requiredSlot = new Mock<IVehicleOccupantSlotPrototype>();
		requiredSlot.SetupGet(x => x.Id).Returns(42L);
		requiredSlot.SetupGet(x => x.Name).Returns("brake operator");
		requiredSlot.SetupGet(x => x.RequiredForMovement).Returns(true);
		Mock.Get(vehicle.Prototype).SetupGet(x => x.OccupantSlots).Returns([requiredSlot.Object]);
		Mock.Get(vehicle).SetupGet(x => x.Occupancies).Returns([]);
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);

		var result = new CellExitVehicleMovementStrategy().CanMove(vehicle, controller.Object, exit, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "brake operator");
	}

	[TestMethod]
	public void TryPrepareMove_DelayedPreflightRollsTowCatastropheOnlyAtCommit()
	{
		var controller = new Mock<ICharacter>();
		var vehicle = CreateVehicle(controller.Object, [VehicleMovementProfileType.CellExit], SizeCategory.Large);
		var exit = CreateExit(vehicle.Location, SizeCategory.Huge);
		var plan = new VehicleHitchGraphMovePlan(vehicle,
			[new VehicleHitchGraphTrainMember(vehicle, 0, null)], [], [], 0.0);
		var readiness = new VehicleMovementReadinessResult(true, string.Empty, plan, null, []);
		var readinessService = new Mock<IVehicleOperationalReadinessService>();
		readinessService.Setup(x => x.BuildMovementReadiness(It.IsAny<VehicleMovementReadinessRequest>()))
		                .Returns(readiness);
		readinessService.Setup(x => x.RollTowCatastrophe(plan, controller.Object))
		                .Returns(new VehicleTowCatastropheResult(false, null, string.Empty, [], []));
		var strategy = new CellExitVehicleMovementStrategy(new Mock<IVehicleTowService>().Object,
			new Mock<IVehicleHitchGraphService>().Object, readinessService.Object);

		var initial = strategy.TryPrepareMove(vehicle, controller.Object, exit, false, out _, out _, out _, out _);
		var commit = strategy.TryPrepareMove(vehicle, controller.Object, exit, true, out _, out _, out _, out _);

		Assert.IsTrue(initial);
		Assert.IsTrue(commit);
		readinessService.Verify(x => x.RollTowCatastrophe(plan, controller.Object), Times.Once);
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
	public void CargoSpace_WhenRequiredAccessPointIsMissing_FailsClosed()
	{
		var requiredAccess = new Mock<IVehicleAccessPointPrototype>();
		requiredAccess.SetupGet(x => x.Name).Returns("rear hatch");
		var prototype = new Mock<IVehicleCargoSpacePrototype>();
		prototype.SetupGet(x => x.Id).Returns(51);
		prototype.SetupGet(x => x.RequiredAccessPoint).Returns(requiredAccess.Object);
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.CargoSpaces).Returns([prototype.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.AccessPoints).Returns([]);
		var cargo = new VehicleCargoSpace(vehicle.Object, new DB.VehicleCargoSpace
		{
			Id = 2,
			Name = "Trunk",
			VehicleCargoSpaceProtoId = 51
		});

		var result = cargo.CanAccess(new Mock<ICharacter>().Object, out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "rear hatch");
		StringAssert.Contains(reason, "missing");
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
	public void Installation_WhenRequiredAccessPointIsMissing_BlocksInstallAndRemoval()
	{
		var requiredAccess = new Mock<IVehicleAccessPointPrototype>();
		requiredAccess.SetupGet(x => x.Name).Returns("service hatch");
		var prototype = new Mock<IVehicleInstallationPointPrototype>();
		prototype.SetupGet(x => x.Id).Returns(61);
		prototype.SetupGet(x => x.MountType).Returns("engine");
		prototype.SetupGet(x => x.RequiredAccessPoint).Returns(requiredAccess.Object);
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.InstallationPoints).Returns([prototype.Object]);
		var gameworld = new Mock<IFuturemud>();
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.AccessPoints).Returns([]);
		vehicle.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var installable = new Mock<IVehicleInstallable>();
		installable.SetupGet(x => x.MountType).Returns("engine");
		var item = new Mock<IGameItem>();
		item.Setup(x => x.GetItemType<IVehicleInstallable>()).Returns(installable.Object);
		var emptyInstallation = new VehicleInstallation(vehicle.Object, new DB.VehicleInstallation
		{
			Id = 3,
			VehicleInstallationPointProtoId = 61
		});
		var occupiedInstallation = new VehicleInstallation(vehicle.Object, new DB.VehicleInstallation
		{
			Id = 4,
			VehicleInstallationPointProtoId = 61,
			InstalledItemId = 42
		});
		gameworld.Setup(x => x.TryGetItem(42, true)).Returns(item.Object);

		var canInstall = emptyInstallation.CanInstall(new Mock<ICharacter>().Object, item.Object, out var installReason);
		var canRemove = occupiedInstallation.CanRemove(new Mock<ICharacter>().Object, out var removeReason);

		Assert.IsFalse(canInstall);
		Assert.IsFalse(canRemove);
		StringAssert.Contains(installReason, "service hatch");
		StringAssert.Contains(removeReason, "service hatch");
	}

	[TestMethod]
	public void CanBoard_WhenActorIsAlreadyOccupyingAnotherVehicle_Fails()
	{
		var location = new Mock<ICell>();
		location.SetupGet(x => x.Id).Returns(42L);

		var slot = new Mock<IVehicleOccupantSlotPrototype>();
		slot.SetupGet(x => x.Id).Returns(1L);
		slot.SetupGet(x => x.Capacity).Returns(1);

		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.OccupantSlots).Returns([slot.Object]);

		var prototypes = new Mock<IUneditableRevisableAll<IVehiclePrototype>>();
		prototypes.Setup(x => x.Get(10L, 0)).Returns(prototype.Object);

		var cells = new Mock<IUneditableAll<ICell>>();
		cells.Setup(x => x.Get(42L)).Returns(location.Object);

		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(location.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

		var otherVehicle = new Mock<IVehicle>();
		otherVehicle.Setup(x => x.IsOccupant(actor.Object)).Returns(true);
		var vehicleList = new List<IVehicle> { otherVehicle.Object };
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		vehicles.Setup(x => x.GetEnumerator()).Returns(() => vehicleList.GetEnumerator());

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Cells).Returns(cells.Object);
		gameworld.SetupGet(x => x.VehiclePrototypes).Returns(prototypes.Object);
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var vehicle = new Vehicle(new DB.Vehicle
		{
			Id = 1L,
			Name = "Test Vehicle",
			VehicleProtoId = 10L,
			VehicleProtoRevision = 0,
			LocationType = (int)VehicleLocationType.Cell,
			CurrentCellId = 42L,
			CurrentRoomLayer = (int)RoomLayer.GroundLevel,
			MovementStatus = (int)VehicleMovementStatus.Stationary
		}, gameworld.Object);

		var result = vehicle.CanBoard(actor.Object, slot.Object, out var reason);

		Assert.IsFalse(result);
		Assert.AreEqual("You are already aboard another vehicle.", reason);
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
		item.SetupGet(x => x.Location).Returns(location.Object);
		item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

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

		Mock.Get(controller).SetupGet(x => x.Location).Returns(location.Object);
		Mock.Get(controller).SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Controller).Returns(controller);
		vehicle.SetupGet(x => x.Location).Returns(location.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
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
