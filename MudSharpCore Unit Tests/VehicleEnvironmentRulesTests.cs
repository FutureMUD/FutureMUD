#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Vehicles;
using System.Reflection;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleEnvironmentRulesTests
{
	[TestMethod]
	public void IsSurfaceWater_GroundLevelSwimmingCell_ReturnsTrue()
	{
		var cell = new Mock<ICell>();
		cell.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(true);

		Assert.IsTrue(cell.Object.IsSurfaceWater(RoomLayer.GroundLevel));
		Assert.IsFalse(cell.Object.IsSurfaceWater(RoomLayer.Underwater));
	}

	[TestMethod]
	public void OccupantSupport_ProtectedSurfaceWaterVehicle_SupportsAndProtects()
	{
		var harness = CreateHarness(exposesOccupants: false);

		var supported = harness.Character.IsSupportedBySurfaceWaterVehicle(out var supportingVehicle);
		var protectedFromWater = harness.Character.IsProtectedFromSurfaceWater(out _);

		Assert.IsTrue(supported);
		Assert.IsTrue(protectedFromWater);
		Assert.AreSame(harness.Vehicle, supportingVehicle);
		Assert.IsTrue(harness.Vehicle.KeepsExteriorAfloat());
	}

	[TestMethod]
	public void OccupantSupport_ExposedSurfaceWaterVehicle_SupportsWithoutProtecting()
	{
		var harness = CreateHarness(exposesOccupants: true);

		Assert.IsTrue(harness.Character.IsSupportedBySurfaceWaterVehicle(out _));
		Assert.IsFalse(harness.Character.IsProtectedFromSurfaceWater(out _));
	}

	[TestMethod]
	public void OccupantSupport_DestroyedSurfaceWaterVehicle_DoesNotSupportOrFloat()
	{
		var harness = CreateHarness(exposesOccupants: false, destroyed: true);

		Assert.IsFalse(harness.Character.IsSupportedBySurfaceWaterVehicle(out _));
		Assert.IsFalse(harness.Vehicle.KeepsExteriorAfloat());
	}

	[TestMethod]
	public void OccupantSupport_DestroyedExterior_DoesNotSupportOrFloat()
	{
		var harness = CreateHarness(exposesOccupants: false, exteriorDestroyed: true);

		Assert.IsFalse(harness.Character.IsSupportedBySurfaceWaterVehicle(out _));
		Assert.IsFalse(harness.Vehicle.KeepsExteriorAfloat());
	}

	[TestMethod]
	public void MovementProfilePrototype_DatabaseFields_LoadIntoRuntimeContract()
	{
		var profile = new VehicleMovementProfilePrototype(new DB.VehicleMovementProfileProto
		{
			Id = 42,
			Name = "Paddle",
			MovementType = (int)VehicleMovementProfileType.CellExit,
			MovementEnvironment = (int)VehicleMovementEnvironment.SurfaceWater,
			ExposesOccupantsToWater = true,
			RouteSpeedMetresPerSecond = 12.5,
			RoutePropulsionMode = (int)RouteVehiclePropulsionMode.Powered,
			RouteFuelVolumePerMetre = 0.04,
			RoutePowerDrawWatts = 300.0,
			AutomaticOperationCapable = true,
			RequiredInstalledRole = string.Empty
		});

		Assert.AreEqual(VehicleMovementEnvironment.SurfaceWater, profile.MovementEnvironment);
		Assert.IsTrue(profile.ExposesOccupantsToWater);
		Assert.AreEqual(12.5, profile.RouteSpeedMetresPerSecond, 0.000001);
		Assert.AreEqual(RouteVehiclePropulsionMode.Powered, profile.RoutePropulsionMode);
		Assert.AreEqual(0.04, profile.RouteFuelVolumePerMetre, 0.000001);
		Assert.AreEqual(300.0, profile.RoutePowerDrawWatts, 0.000001);
		Assert.IsTrue(profile.AutomaticOperationCapable);
	}

	[TestMethod]
	public void MovementProfilePrototype_NewDatabaseModel_DefaultsToUnrestrictedAndProtected()
	{
		var profile = new DB.VehicleMovementProfileProto();

		Assert.AreEqual((int)VehicleMovementEnvironment.Unrestricted, profile.MovementEnvironment);
		Assert.IsFalse(profile.ExposesOccupantsToWater);
		Assert.AreEqual(0.0, profile.RouteSpeedMetresPerSecond);
		Assert.AreEqual((int)RouteVehiclePropulsionMode.Powered, profile.RoutePropulsionMode);
		Assert.AreEqual(0.0, profile.RouteFuelVolumePerMetre);
		Assert.AreEqual(0.0, profile.RoutePowerDrawWatts);
		Assert.IsFalse(profile.AutomaticOperationCapable);
	}

	[TestMethod]
	public void PropulsionProfilePrototype_NewDatabaseModel_DefaultsToTenSeconds()
	{
		var profile = new DB.VehiclePropulsionProfileProto();

		Assert.AreEqual(10000.0, profile.BaseMoveTimeMilliseconds);
	}

	[TestMethod]
	public void MovementProfile_WhenPersistedSelectionIsMissing_FallsBackToCellExitProfile()
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(42);
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.IsDefault).Returns(true);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		var prototypes = new Mock<IUneditableRevisableAll<IVehiclePrototype>>();
		prototypes.Setup(x => x.Get(10, 0)).Returns(prototype.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.VehiclePrototypes).Returns(prototypes.Object);
		var vehicle = new Vehicle(new DB.Vehicle
		{
			Id = 1,
			Name = "Test Boat",
			VehicleProtoId = 10,
			VehicleProtoRevision = 0,
			CurrentRoomLayer = (int)RoomLayer.GroundLevel
		}, gameworld.Object);

		Assert.AreSame(profile.Object, vehicle.MovementProfile);
	}

	[TestMethod]
	public void ActivePropulsionProfile_WhenPersistedSelectionIsMissing_FallsBackToDefaultMode()
	{
		var propulsion = new Mock<IVehiclePropulsionProfilePrototype>();
		propulsion.SetupGet(x => x.Id).Returns(84);
		propulsion.SetupGet(x => x.IsDefault).Returns(true);
		propulsion.SetupGet(x => x.PropulsionType).Returns(VehiclePropulsionType.Sail);
		var movement = new Mock<IVehicleMovementProfilePrototype>();
		movement.SetupGet(x => x.Id).Returns(42);
		movement.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		movement.SetupGet(x => x.IsDefault).Returns(true);
		movement.SetupGet(x => x.PropulsionProfiles).Returns([propulsion.Object]);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns([movement.Object]);
		var prototypes = new Mock<IUneditableRevisableAll<IVehiclePrototype>>();
		prototypes.Setup(x => x.Get(10, 0)).Returns(prototype.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.VehiclePrototypes).Returns(prototypes.Object);
		var vehicle = new Vehicle(new DB.Vehicle
		{
			Id = 1,
			Name = "Test Boat",
			VehicleProtoId = 10,
			VehicleProtoRevision = 0,
			MovementProfileProtoId = 42,
			ActivePropulsionProfileProtoId = 999,
			CurrentRoomLayer = (int)RoomLayer.GroundLevel
		}, gameworld.Object);

		Assert.AreSame(propulsion.Object, vehicle.ActivePropulsionProfile);
	}

	[TestMethod]
	public void BuildingHelp_ListsSurfaceWaterEnvironmentCommands()
	{
		var help = (string)typeof(VehiclePrototype)
			.GetField("BuildingHelp", BindingFlags.Static | BindingFlags.NonPublic)!
			.GetRawConstantValue()!;

		StringAssert.Contains(help, "movement environment <id> <unrestricted|surfacewater>");
		StringAssert.Contains(help, "movement waterexposure <id> <protected|exposed>");
		StringAssert.Contains(help, "movement propulsion add <movement id>");
		StringAssert.Contains(help, "slot propulsion <id>");
	}

	[TestMethod]
	public void BuilderParsing_RecognisesSurfaceWaterAndExposureValues()
	{
		Assert.AreEqual(VehicleMovementEnvironment.Unrestricted,
			VehiclePrototype.ParseMovementEnvironment("unrestricted"));
		Assert.AreEqual(VehicleMovementEnvironment.SurfaceWater,
			VehiclePrototype.ParseMovementEnvironment("surfacewater"));
		Assert.IsNull(VehiclePrototype.ParseMovementEnvironment("submarine"));
		Assert.AreEqual(false, VehiclePrototype.ParseMovementWaterExposure("protected"));
		Assert.AreEqual(true, VehiclePrototype.ParseMovementWaterExposure("exposed"));
		Assert.IsNull(VehiclePrototype.ParseMovementWaterExposure("damp"));
		Assert.AreEqual(VehiclePropulsionType.SelfPowered,
			VehiclePrototype.ParseVehiclePropulsionType("selfpowered"));
		Assert.AreEqual(VehiclePropulsionType.OutboardMotor,
			VehiclePrototype.ParseVehiclePropulsionType("outboard"));
		Assert.IsNull(VehiclePrototype.ParseVehiclePropulsionType("steam"));
	}

	[TestMethod]
	public void PropulsionFormulaValidation_RejectsNonPositiveSpeedAndNegativeStamina()
	{
		Assert.IsFalse(VehiclePrototype.ValidatePropulsionExpression(VehiclePropulsionType.Sail, "0", true,
			out _));
		Assert.IsFalse(VehiclePrototype.ValidatePropulsionExpression(VehiclePropulsionType.Rowed, "-1", false,
			out _));
		Assert.IsFalse(VehiclePrototype.ValidatePropulsionExpression(VehiclePropulsionType.Rowed, "wind", false,
			out _));
		Assert.IsTrue(VehiclePrototype.ValidatePropulsionExpression(VehiclePropulsionType.SelfPowered,
			"max(0.25, 1.0 + (0.15 * outcome))", true, out _));
	}

	[TestMethod]
	public void BuilderShow_SurfaceWaterProfile_IncludesEnvironmentAndExposure()
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(42);
		profile.SetupGet(x => x.Name).Returns("Surfboard");
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		profile.SetupGet(x => x.ExposesOccupantsToWater).Returns(true);
		var actor = new Mock<ICharacter>();

		var text = VehiclePrototype.DescribeMovementProfileForShow(profile.Object, actor.Object);

		StringAssert.Contains(text, "Surface Water");
		StringAssert.Contains(text, "occupants exposed");
	}

	[TestMethod]
	public void DisembarkingIntoSurfaceWater_SetsSwimmingPostureOnce()
	{
		var location = new Mock<ICell>();
		location.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(true);
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);
		var prototypes = new Mock<IUneditableRevisableAll<IVehiclePrototype>>();
		prototypes.Setup(x => x.Get(10, 0)).Returns(prototype.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.VehiclePrototypes).Returns(prototypes.Object);
		var vehicle = new Vehicle(new DB.Vehicle
		{
			Id = 1,
			Name = "Test Boat",
			VehicleProtoId = 10,
			VehicleProtoRevision = 0,
			CurrentRoomLayer = (int)RoomLayer.GroundLevel
		}, gameworld.Object);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Location).Returns(location.Object);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

		typeof(Vehicle)
			.GetMethod("NormaliseDisembarkedOccupant", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(vehicle, [character.Object]);

		character.Verify(x => x.SetPosition(PositionSwimming.Instance, PositionModifier.None, null, null), Times.Once);
	}

	[TestMethod]
	public void DamageDestructionTransition_ForceDisembarksExactlyOnce()
	{
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		var zonePrototype = new Mock<IVehicleDamageZonePrototype>();
		zonePrototype.SetupGet(x => x.Id).Returns(99);
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.DamageZones).Returns([zonePrototype.Object]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		vehicle.SetupGet(x => x.Destroyed).Returns(true);
		var zone = new VehicleDamageZone(vehicle.Object, new DB.VehicleDamageZone
		{
			Id = 5,
			Name = "Hull",
			VehicleDamageZoneProtoId = 99
		});
		var transition = typeof(VehicleDamageZone)
			.GetMethod("HandleVehicleDestroyedTransition", BindingFlags.Instance | BindingFlags.NonPublic)!;

		transition.Invoke(zone, [false]);
		transition.Invoke(zone, [true]);

		vehicle.Verify(x => x.ForceDisembarkAll(), Times.Once);
	}

	private static EnvironmentHarness CreateHarness(bool exposesOccupants, bool destroyed = false,
		bool exteriorDestroyed = false)
	{
		var location = new Mock<ICell>();
		location.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(true);
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		profile.SetupGet(x => x.ExposesOccupantsToWater).Returns(exposesOccupants);
		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Location).Returns(location.Object);
		exterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		exterior.SetupGet(x => x.Destroyed).Returns(exteriorDestroyed);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Location).Returns(location.Object);
		character.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(1);
		vehicle.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		vehicle.SetupGet(x => x.Location).Returns(location.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Destroyed).Returns(destroyed);
		vehicle.Setup(x => x.IsOccupant(character.Object)).Returns(true);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Vehicles).Returns(new All<IVehicle> { vehicle.Object });
		character.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		return new EnvironmentHarness(character.Object, vehicle.Object);
	}

	private sealed record EnvironmentHarness(ICharacter Character, IVehicle Vehicle);
}
