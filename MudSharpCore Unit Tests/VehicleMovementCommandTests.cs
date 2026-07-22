#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleMovementCommandTests
{
	[TestMethod]
	public void Disembark_DoesNotShowHelpWhenUsedWithoutArguments()
	{
		var method = typeof(VehicleModule).GetMethod("Disembark", BindingFlags.Static | BindingFlags.NonPublic);
		var help = method!.GetCustomAttribute<HelpInfo>();

		Assert.IsNotNull(help);
		Assert.AreEqual(AutoHelp.HelpArg, help!.AutoHelpSetting);
		StringAssert.Contains(help.DefaultHelp, "open docking exit");
	}

	[TestMethod]
	public void EmbarkHelp_AdvertisesOpenPlatformDockings()
	{
		var method = typeof(VehicleModule).GetMethod("Embark", BindingFlags.Static | BindingFlags.NonPublic);
		var help = method!.GetCustomAttribute<HelpInfo>();

		Assert.IsNotNull(help);
		StringAssert.Contains(help!.DefaultHelp, "open platform docking");
	}

	[TestMethod]
	public void DockedVehiclesAt_IncludesVehicleWithOpenDockingAtActorsPlatform()
	{
		var platform = new Mock<ICell>();
		platform.SetupGet(x => x.Id).Returns(42L);
		var elsewhere = new Mock<ICell>();
		elsewhere.SetupGet(x => x.Id).Returns(84L);

		var activeDocking = new Mock<IVehicleDocking>();
		activeDocking.SetupGet(x => x.State).Returns(VehicleDockingState.BoardingOpen);
		activeDocking.SetupGet(x => x.ExteriorCell).Returns(platform.Object);
		activeDocking.SetupGet(x => x.ExteriorLayer).Returns(RoomLayer.GroundLevel);
		var remoteDocking = new Mock<IVehicleDocking>();
		remoteDocking.SetupGet(x => x.State).Returns(VehicleDockingState.BoardingOpen);
		remoteDocking.SetupGet(x => x.ExteriorCell).Returns(elsewhere.Object);
		remoteDocking.SetupGet(x => x.ExteriorLayer).Returns(RoomLayer.GroundLevel);

		var dockedVehicle = new Mock<IVehicle>();
		dockedVehicle.SetupGet(x => x.Id).Returns(1L);
		dockedVehicle.SetupGet(x => x.Dockings).Returns([activeDocking.Object]);
		var remoteVehicle = new Mock<IVehicle>();
		remoteVehicle.SetupGet(x => x.Id).Returns(2L);
		remoteVehicle.SetupGet(x => x.Dockings).Returns([remoteDocking.Object]);
		var vehicleList = new List<IVehicle> { dockedVehicle.Object, remoteVehicle.Object };
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		vehicles.Setup(x => x.GetEnumerator()).Returns(() => vehicleList.GetEnumerator());

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Location).Returns(platform.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

		var method = typeof(VehicleModule).GetMethod("DockedVehiclesAt",
			BindingFlags.Static | BindingFlags.NonPublic);
		var result = (IReadOnlyList<IVehicle>)method!.Invoke(null, [actor.Object])!;

		Assert.AreEqual(1, result.Count);
		Assert.AreSame(dockedVehicle.Object, result[0]);
	}

	[TestMethod]
	public void DockingExitFrom_UsesTransientExitFromInteriorToPlatform()
	{
		var interior = new Mock<ICell>();
		var platform = new Mock<ICell>();
		var cellExit = new Mock<ICellExit>();
		cellExit.SetupGet(x => x.Origin).Returns(interior.Object);
		cellExit.SetupGet(x => x.Destination).Returns(platform.Object);
		var transientExit = new Mock<IExit>();
		transientExit.Setup(x => x.CellExitFor(interior.Object)).Returns(cellExit.Object);
		var docking = new Mock<IVehicleDocking>();
		docking.SetupGet(x => x.State).Returns(VehicleDockingState.BoardingOpen);
		docking.SetupGet(x => x.TransientExit).Returns(transientExit.Object);

		var result = Vehicle.DockingExitFrom(docking.Object, interior.Object);

		Assert.IsNotNull(result);
		Assert.AreSame(platform.Object, result!.Destination);
	}

	[TestMethod]
	public void DockedVehicleLookLines_ShowRemotePlatformDockingWithoutDuplicatingLocalExterior()
	{
		var platform = new Mock<ICell>();
		platform.SetupGet(x => x.Id).Returns(42L);
		var routeCell = new Mock<ICell>();
		routeCell.SetupGet(x => x.Id).Returns(84L);
		var access = new Mock<IVehicleAccessPoint>();
		access.SetupGet(x => x.Name).Returns("Passenger Doors");

		var remoteDocking = new Mock<IVehicleDocking>();
		remoteDocking.SetupGet(x => x.State).Returns(VehicleDockingState.BoardingOpen);
		remoteDocking.SetupGet(x => x.ExteriorCell).Returns(platform.Object);
		remoteDocking.SetupGet(x => x.ExteriorLayer).Returns(RoomLayer.GroundLevel);
		remoteDocking.SetupGet(x => x.AccessPoint).Returns(access.Object);
		var remoteExterior = new Mock<IGameItem>();
		remoteExterior.SetupGet(x => x.Location).Returns(routeCell.Object);
		var train = new Mock<IVehicle>();
		train.SetupGet(x => x.Id).Returns(1L);
		train.SetupGet(x => x.Name).Returns("QA Intertown Train");
		train.SetupGet(x => x.ExteriorItem).Returns(remoteExterior.Object);
		train.SetupGet(x => x.Dockings).Returns([remoteDocking.Object]);

		var localExterior = new Mock<IGameItem>();
		localExterior.SetupGet(x => x.Location).Returns(platform.Object);
		localExterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var localVehicle = new Mock<IVehicle>();
		localVehicle.SetupGet(x => x.Id).Returns(2L);
		localVehicle.SetupGet(x => x.Name).Returns("Local Platform");
		localVehicle.SetupGet(x => x.ExteriorItem).Returns(localExterior.Object);
		localVehicle.SetupGet(x => x.Dockings).Returns([remoteDocking.Object]);

		var vehicleList = new List<IVehicle> { train.Object, localVehicle.Object };
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		vehicles.Setup(x => x.GetEnumerator()).Returns(() => vehicleList.GetEnumerator());
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Location).Returns(platform.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

		var lines = Body.DockedVehicleLookLines(actor.Object).ToList();

		Assert.AreEqual(1, lines.Count);
		StringAssert.Contains(lines[0], "QA Intertown Train");
		StringAssert.Contains(lines[0], "Passenger Doors");
	}

	[TestMethod]
	public void Drive_IsMovementCommandAndCanQueueWhileMoving()
	{
		var method = typeof(VehicleModule).GetMethod("Drive", BindingFlags.Static | BindingFlags.NonPublic);

		Assert.IsNotNull(method);
		Assert.IsNull(method!.GetCustomAttribute<NoMovementCommand>());
		Assert.IsNotNull(method.GetCustomAttribute<DelayBlock>());
	}

	[TestMethod]
	public void VehiclePropulsion_IsRegisteredAsStationaryPlayerCommand()
	{
		var method = typeof(VehicleModule).GetMethod("VehiclePropulsion", BindingFlags.Static | BindingFlags.NonPublic);

		Assert.IsNotNull(method);
		Assert.IsNotNull(method!.GetCustomAttribute<PlayerCommand>());
		Assert.IsNotNull(method.GetCustomAttribute<NoMovementCommand>());
		Assert.IsNotNull(method.GetCustomAttribute<HelpInfo>());
	}

	[TestMethod]
	public void VehicleAccessPoint_IsRegisteredAndDocumentsHostedInteriorOperation()
	{
		var method = typeof(VehicleModule).GetMethod("VehicleAccessPoint",
			BindingFlags.Static | BindingFlags.NonPublic);

		Assert.IsNotNull(method);
		Assert.IsNotNull(method!.GetCustomAttribute<PlayerCommand>());
		Assert.IsNotNull(method.GetCustomAttribute<NoMovementCommand>());
		var help = method.GetCustomAttribute<HelpInfo>();
		Assert.IsNotNull(help);
		StringAssert.Contains(help!.DefaultHelp, "while aboard");
		StringAssert.Contains(help.DefaultHelp, "vehicleaccess close");
	}

	[TestMethod]
	public void VehicleAccessPoint_OccupantWithControlAccess_ClosesCanonicalAccessPoint()
	{
		var context = CreateVehicleAccessCommandContext(accessOpen: true);

		InvokeVehicleAccessPoint(context.Actor.Object, "vehicleaccess close passenger doors");

		context.AccessPoint.Verify(x => x.SetOpen(false), Times.Once);
		context.Output.Verify(x => x.Send(It.Is<string>(text =>
			text.Contains("close") && text.Contains("Passenger Doors")), true, false), Times.Once);
	}

	[TestMethod]
	public void VehicleAccessPoint_WithoutControlAccess_FailsClosed()
	{
		var context = CreateVehicleAccessCommandContext(accessOpen: true);
		var otherCharacter = new Mock<ICharacter>();
		var accessState = new Mock<IVehicleAccessState>();
		accessState.SetupGet(x => x.Character).Returns(otherCharacter.Object);
		accessState.SetupGet(x => x.AccessTag).Returns("control");
		accessState.SetupGet(x => x.AccessLevel).Returns(3);
		context.Vehicle.SetupGet(x => x.AccessStates).Returns([accessState.Object]);

		InvokeVehicleAccessPoint(context.Actor.Object, "vehicleaccess close passenger doors");

		context.AccessPoint.Verify(x => x.SetOpen(It.IsAny<bool>()), Times.Never);
		context.Output.Verify(x => x.Send(It.Is<string>(text => text.Contains("control") &&
		                                                        text.Contains("access")), true, false), Times.Once);
	}

	[TestMethod]
	public void VehicleAccessPoint_LockedOrMoving_FailsBeforeChangingState()
	{
		var locked = CreateVehicleAccessCommandContext(accessOpen: false);
		locked.AccessPoint.SetupGet(x => x.IsLocked).Returns(true);
		InvokeVehicleAccessPoint(locked.Actor.Object, "vehicleaccess open passenger doors");
		locked.AccessPoint.Verify(x => x.SetOpen(It.IsAny<bool>()), Times.Never);
		locked.Output.Verify(x => x.Send(It.Is<string>(text => text.Contains("locked")), true, false), Times.Once);

		var moving = CreateVehicleAccessCommandContext(accessOpen: true);
		moving.MovementState.SetupGet(x => x.MovementStatus).Returns(VehicleMovementStatus.Moving);
		InvokeVehicleAccessPoint(moving.Actor.Object, "vehicleaccess close passenger doors");
		moving.AccessPoint.Verify(x => x.SetOpen(It.IsAny<bool>()), Times.Never);
		moving.Output.Verify(x => x.Send(It.Is<string>(text => text.Contains("stationary")), true, false), Times.Once);
	}

	[TestMethod]
	public void VehicleAccessPoint_DisabledByDamage_ReportsReadinessReasonAndFailsClosed()
	{
		var context = CreateVehicleAccessCommandContext(accessOpen: true);
		context.AccessPoint.SetupGet(x => x.IsDisabled).Returns(true);
		context.Vehicle.Setup(x => x.DamageDisabledReason(VehicleDamageEffectTargetType.AccessPoint, 31))
			.Returns("the boarding mechanism is destroyed");

		InvokeVehicleAccessPoint(context.Actor.Object, "vehicleaccess close passenger doors");

		context.AccessPoint.Verify(x => x.SetOpen(It.IsAny<bool>()), Times.Never);
		context.Output.Verify(x => x.Send(It.Is<string>(text =>
			text.Contains("disabled") && text.Contains("boarding mechanism")), true, false), Times.Once);
	}

	[TestMethod]
	public void VehicleMovement_InitialAction_BeginsTransitAndSchedulesDelayedMove()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Scheduler).Returns(scheduler.Object);
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(10_000.0);

		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupProperty(x => x.Movement);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.SetupGet(x => x.InnerLineFormatLength).Returns(120);
		actor.SetupGet(x => x.QueuedMoveCommands).Returns(new System.Collections.Generic.Queue<string>());
		actor.Setup(x => x.MoveSpeed(It.IsAny<ICellExit>())).Returns(250.0);
		actor.Setup(x => x.CanSee(actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		actor.Setup(x => x.IsSelf(actor.Object)).Returns(true);
		actor.Setup(x => x.HowSeen(actor.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("you");

		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		origin.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns([actor.Object]);
		actor.SetupGet(x => x.Location).Returns(origin.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var destination = new Mock<ICell>();
		destination.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		destination.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns([]);

		var exitModel = new Mock<IExit>();
		exitModel.SetupGet(x => x.MaximumSizeToEnter).Returns(SizeCategory.Huge);
		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Origin).Returns(origin.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.SetupGet(x => x.Exit).Returns(exitModel.Object);
		exit.SetupGet(x => x.OutboundMovementSuffix).Returns("north");
		exit.Setup(x => x.MovementTransition(actor.Object))
		    .Returns((CellMovementTransition.GroundToGround, RoomLayer.GroundLevel));

		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(1L);
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.IsDefault).Returns(true);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);

		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
		exterior.SetupGet(x => x.Location).Returns(origin.Object);
		exterior.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		exterior.Setup(x => x.HowSeen(actor.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("a QA test bicycle");
		actor.Setup(x => x.CanSee(exterior.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(1L);
		vehicle.SetupGet(x => x.Name).Returns("test bicycle");
		vehicle.SetupGet(x => x.Controller).Returns(actor.Object);
		vehicle.SetupGet(x => x.Location).Returns(origin.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Occupants).Returns([actor.Object]);
		vehicle.Setup(x => x.IsOccupant(actor.Object)).Returns(true);
		vehicle.SetupGet(x => x.AccessPoints).Returns([]);
		vehicle.SetupGet(x => x.Installations).Returns([]);
		vehicle.SetupGet(x => x.TowLinks).Returns([]);

		var movement = new VehicleMovement(vehicle.Object, actor.Object, exit.Object);

		movement.InitialAction();

		vehicle.Verify(x => x.BeginMoveToCell(destination.Object, RoomLayer.GroundLevel, exit.Object), Times.Once);
		vehicle.Verify(x => x.MoveToCell(It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<ICellExit>(), It.IsAny<IMovement>()), Times.Never);
		scheduler.Verify(x => x.AddSchedule(It.Is<ISchedule>(schedule => schedule.Type == ScheduleType.Movement)), Times.Once);
		output.Verify(x => x.Send(It.Is<string>(text => text.Contains("begin riding") &&
		                                                text.Contains("QA test bicycle")), true, false), Times.Once);
		Assert.AreSame(movement, actor.Object.Movement);
	}

	[TestMethod]
	public void VehicleMovement_DescribeEnterMove_UsesRidingArrivalGrammar()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(10_000.0);

		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.QueuedMoveCommands).Returns(new Queue<string>());
		actor.Setup(x => x.MoveSpeed(It.IsAny<ICellExit>())).Returns(250.0);

		var voyeur = new Mock<ICharacter>();
		voyeur.Setup(x => x.CanSee(actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		actor.Setup(x => x.IsSelf(voyeur.Object)).Returns(false);
		actor.Setup(x => x.HowSeen(voyeur.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("a wide-eyed gentleman");

		var origin = new Mock<ICell>();
		var destination = new Mock<ICell>();
		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Origin).Returns(origin.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.SetupGet(x => x.InboundMovementSuffix).Returns("in from the South");

		var exterior = new Mock<IGameItem>();
		exterior.Setup(x => x.HowSeen(voyeur.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("a QA test bicycle");
		voyeur.Setup(x => x.CanSee(exterior.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);

		var vehicle = new Mock<IVehicle>();
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
		vehicle.SetupGet(x => x.Name).Returns("test bicycle");
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Occupants).Returns([]);
		vehicle.Setup(x => x.IsOccupant(actor.Object)).Returns(true);

		var movement = new VehicleMovement(vehicle.Object, actor.Object, exit.Object);
		var method = typeof(VehicleMovement).GetMethod("DescribeEnterMove", BindingFlags.Instance | BindingFlags.NonPublic);

		var text = (string)method!.Invoke(movement, [voyeur.Object])!;

		Assert.AreEqual("A wide-eyed gentleman rides in from the South on a QA test bicycle.", text);
		Assert.IsFalse(text.Contains("arrives in"));
	}

	[TestMethod]
	public void VehicleExteriorComponent_BlocksRepositioningWhileOccupied()
	{
		var gameworld = new Mock<IFuturemud>();
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);

		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var actor = new Mock<ICharacter>();
		var vehicle = new Mock<IVehicle>();
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
		vehicle.SetupGet(x => x.Id).Returns(42L);
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicle.SetupGet(x => x.Occupants).Returns([actor.Object]);
		vehicles.Setup(x => x.Get(42L)).Returns(vehicle.Object);

		var prototype = CreateVehicleExteriorComponentProto(gameworld.Object);
		var component = new VehicleExteriorGameItemComponent(prototype, parent.Object, true);
		component.LinkVehicle(vehicle.Object);

		Assert.IsTrue(component.PreventsRepositioning());
		Assert.AreEqual(" is currently occupied.", component.WhyPreventsRepositioning());
	}

	[TestMethod]
	public void VehicleExteriorComponent_ForcedMoveAndDeleteDelegateToVehicle()
	{
		var gameworld = new Mock<IFuturemud>();
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);

		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var vehicle = new Mock<IVehicle>();
		var vehiclePrototype = new Mock<IVehiclePrototype>();
		vehiclePrototype.SetupGet(x => x.Scale).Returns(VehicleScale.ItemScale);
		vehicle.SetupGet(x => x.Id).Returns(42L);
		vehicle.SetupGet(x => x.Prototype).Returns(vehiclePrototype.Object);
		vehicles.Setup(x => x.Get(42L)).Returns(vehicle.Object);

		var prototype = CreateVehicleExteriorComponentProto(gameworld.Object);
		var component = new VehicleExteriorGameItemComponent(prototype, parent.Object, true);
		component.LinkVehicle(vehicle.Object);

		component.ForceMove();
		component.Delete();

		vehicle.Verify(x => x.HandleExteriorItemForceMoved(), Times.Once);
		vehicle.Verify(x => x.ForceDisembarkAll(), Times.Once);
	}

	private static VehicleExteriorGameItemComponentProto CreateVehicleExteriorComponentProto(IFuturemud gameworld)
	{
		var dbproto = new MudSharp.Models.GameItemComponentProto
		{
			Id = 1155L,
			Name = "QA Vehicle Exterior",
			Description = "QA vehicle exterior component",
			Type = "Vehicle Exterior",
			Definition = "<Definition></Definition>",
			RevisionNumber = 0,
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 0,
				BuilderAccountId = 1L,
				BuilderDate = System.DateTime.UtcNow
			}
		};

		return (VehicleExteriorGameItemComponentProto)Activator.CreateInstance(
			typeof(VehicleExteriorGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[dbproto, gameworld],
			null)!;
	}

	private static (Mock<ICharacter> Actor, Mock<IVehicle> Vehicle,
		Mock<IVehicleAccessPoint> AccessPoint, Mock<IVehicleMovementState> MovementState,
		Mock<IOutputHandler> Output) CreateVehicleAccessCommandContext(bool accessOpen)
	{
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);

		var accessPrototype = new Mock<IVehicleAccessPointPrototype>();
		accessPrototype.SetupGet(x => x.Id).Returns(31);
		var accessPoint = new Mock<IVehicleAccessPoint>();
		var open = accessOpen;
		accessPoint.SetupGet(x => x.Id).Returns(41);
		accessPoint.SetupGet(x => x.Name).Returns("Passenger Doors");
		accessPoint.SetupGet(x => x.Prototype).Returns(accessPrototype.Object);
		accessPoint.SetupGet(x => x.IsOpen).Returns(() => open);
		accessPoint.SetupGet(x => x.IsDisabled).Returns(false);
		accessPoint.SetupGet(x => x.IsLocked).Returns(false);
		accessPoint.Setup(x => x.SetOpen(It.IsAny<bool>())).Callback<bool>(value => open = value);

		var movementState = new Mock<IVehicleMovementState>();
		movementState.SetupGet(x => x.MovementStatus).Returns(VehicleMovementStatus.Stationary);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(71);
		vehicle.SetupGet(x => x.Name).Returns("QA Mobile Platform");
		vehicle.SetupGet(x => x.MovementState).Returns(movementState.Object);
		vehicle.SetupGet(x => x.AccessPoints).Returns([accessPoint.Object]);
		vehicle.SetupGet(x => x.AccessStates).Returns([]);
		vehicle.Setup(x => x.IsOccupant(actor.Object)).Returns(true);

		var vehicleList = new List<IVehicle> { vehicle.Object };
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		vehicles.Setup(x => x.GetEnumerator()).Returns(() => vehicleList.GetEnumerator());
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		return (actor, vehicle, accessPoint, movementState, output);
	}

	private static void InvokeVehicleAccessPoint(ICharacter actor, string command)
	{
		var method = typeof(VehicleModule).GetMethod("VehicleAccessPoint",
			BindingFlags.Static | BindingFlags.NonPublic);
		method!.Invoke(null, [actor, command]);
	}
}
