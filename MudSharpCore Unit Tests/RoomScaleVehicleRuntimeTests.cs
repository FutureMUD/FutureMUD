#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomScaleVehicleRuntimeTests
{
	[TestMethod]
	[DataRow("indoors", CellOutdoorsType.Indoors)]
	[DataRow("windows", CellOutdoorsType.IndoorsWithWindows)]
	[DataRow("outdoors", CellOutdoorsType.Outdoors)]
	[DataRow("dark", CellOutdoorsType.IndoorsNoLight)]
	[DataRow("climateexposed", CellOutdoorsType.IndoorsClimateExposed)]
	public void InteriorExposureParser_AcceptsRunbookAliases(string text, CellOutdoorsType expected)
	{
		Assert.IsTrue(VehiclePrototype.TryParseInteriorOutdoorsType(text, out var actual));
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void RoomScaleTopology_MissingInteriorTerrain_IsRejected()
	{
		var compartment = Compartment(1, "Control", hasTerrain: false);

		var valid = VehiclePrototype.ValidateRoomScaleTopology([compartment], [], [], out var reason);

		Assert.IsFalse(valid);
		StringAssert.Contains(reason, "does not define an interior terrain");
	}

	[TestMethod]
	public void RoomScaleTopology_DisconnectedCompartments_AreRejected()
	{
		var control = Compartment(1, "Control");
		var deckhouse = Compartment(2, "Deckhouse");

		var valid = VehiclePrototype.ValidateRoomScaleTopology([control, deckhouse], [], [Access(deckhouse)], out var reason);

		Assert.IsFalse(valid);
		StringAssert.Contains(reason, "must be connected");
	}

	[TestMethod]
	public void RoomScaleTopology_NoExternalAccess_IsRejected()
	{
		var control = Compartment(1, "Control");

		var valid = VehiclePrototype.ValidateRoomScaleTopology([control], [], [], out var reason);

		Assert.IsFalse(valid);
		StringAssert.Contains(reason, "at least one external access point");
	}

	[TestMethod]
	public void RoomScaleTopology_RunbookPlatformGraph_IsAccepted()
	{
		var control = Compartment(1, "Control");
		var deckhouse = Compartment(2, "Deckhouse");
		var link = Link(10, control, deckhouse, "aft", "forward");
		var valid = VehiclePrototype.ValidateRoomScaleTopology(
			[control, deckhouse], [link], [Access(deckhouse)], out var reason);

		Assert.IsTrue(valid, reason);
	}

	[TestMethod]
	public void RoomScaleAccess_ConnectedLiveInteriorGraph_ReachesDriverSlot()
	{
		var controlPrototype = Compartment(1, "Control");
		var deckhousePrototype = Compartment(2, "Deckhouse");
		var controlCell = new Mock<ICell>();
		controlCell.SetupGet(x => x.Id).Returns(101L);
		var deckhouseCell = new Mock<ICell>();
		deckhouseCell.SetupGet(x => x.Id).Returns(102L);
		var control = new Mock<IVehicleCompartment>();
		control.SetupGet(x => x.Prototype).Returns(controlPrototype);
		control.SetupGet(x => x.InteriorCell).Returns(controlCell.Object);
		var deckhouse = new Mock<IVehicleCompartment>();
		deckhouse.SetupGet(x => x.Prototype).Returns(deckhousePrototype);
		deckhouse.SetupGet(x => x.InteriorCell).Returns(deckhouseCell.Object);
		var link = new Mock<IVehicleCompartmentLink>();
		link.SetupGet(x => x.SourceCompartment).Returns(control.Object);
		link.SetupGet(x => x.DestinationCompartment).Returns(deckhouse.Object);
		link.SetupGet(x => x.Exit).Returns(new Mock<MudSharp.Construction.Boundary.IExit>().Object);
		control.SetupGet(x => x.Links).Returns([link.Object]);
		deckhouse.SetupGet(x => x.Links).Returns([link.Object]);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Compartments).Returns([control.Object, deckhouse.Object]);
		var accessPrototype = Access(deckhousePrototype);
		var access = new Mock<IVehicleAccessPoint>();
		access.SetupGet(x => x.Prototype).Returns(accessPrototype);
		var slot = new Mock<IVehicleOccupantSlotPrototype>();
		slot.SetupGet(x => x.Compartment).Returns(controlPrototype);

		Assert.IsTrue(vehicle.Object.AccessPointCanReachSlot(access.Object, slot.Object));
	}

	[TestMethod]
	public void RoomScaleAccess_AuthoredLinkWithoutLiveExit_FailsClosed()
	{
		var controlPrototype = Compartment(1, "Control");
		var deckhousePrototype = Compartment(2, "Deckhouse");
		var control = new Mock<IVehicleCompartment>();
		control.SetupGet(x => x.Prototype).Returns(controlPrototype);
		control.SetupGet(x => x.InteriorCell).Returns(new Mock<ICell>().Object);
		var deckhouse = new Mock<IVehicleCompartment>();
		deckhouse.SetupGet(x => x.Prototype).Returns(deckhousePrototype);
		deckhouse.SetupGet(x => x.InteriorCell).Returns(new Mock<ICell>().Object);
		var brokenLink = new Mock<IVehicleCompartmentLink>();
		brokenLink.SetupGet(x => x.SourceCompartment).Returns(control.Object);
		brokenLink.SetupGet(x => x.DestinationCompartment).Returns(deckhouse.Object);
		brokenLink.SetupGet(x => x.Exit).Returns((MudSharp.Construction.Boundary.IExit?)null);
		control.SetupGet(x => x.Links).Returns([brokenLink.Object]);
		deckhouse.SetupGet(x => x.Links).Returns([brokenLink.Object]);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Compartments).Returns([control.Object, deckhouse.Object]);
		var access = new Mock<IVehicleAccessPoint>();
		access.SetupGet(x => x.Prototype).Returns(Access(deckhousePrototype));
		var slot = new Mock<IVehicleOccupantSlotPrototype>();
		slot.SetupGet(x => x.Compartment).Returns(controlPrototype);

		Assert.IsFalse(vehicle.Object.AccessPointCanReachSlot(access.Object, slot.Object));
	}

	[TestMethod]
	public void RoomScaleControlStation_RequiresActorInAssignedHostedCompartment()
	{
		var controlPrototype = Compartment(1, "Control");
		var controlCell = new Mock<ICell>();
		controlCell.SetupGet(x => x.Id).Returns(101L);
		var deckhouseCell = new Mock<ICell>();
		deckhouseCell.SetupGet(x => x.Id).Returns(102L);
		var control = new Mock<IVehicleCompartment>();
		control.SetupGet(x => x.Prototype).Returns(controlPrototype);
		control.SetupGet(x => x.InteriorCell).Returns(controlCell.Object);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Compartments).Returns([control.Object]);
		var slot = new Mock<IVehicleOccupantSlotPrototype>();
		slot.SetupGet(x => x.Compartment).Returns(controlPrototype);
		ICell actorLocation = deckhouseCell.Object;
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(() => actorLocation);

		Assert.IsFalse(vehicle.Object.IsAtOccupantSlotLocation(actor.Object, slot.Object));
		actorLocation = controlCell.Object;
		Assert.IsTrue(vehicle.Object.IsAtOccupantSlotLocation(actor.Object, slot.Object));
	}

	[TestMethod]
	public void RoomScaleOccupantMovement_InternalHostedExit_IsPermitted()
	{
		var controlCell = new Mock<ICell>();
		controlCell.SetupGet(x => x.Id).Returns(101L);
		var deckhouseCell = new Mock<ICell>();
		deckhouseCell.SetupGet(x => x.Id).Returns(102L);
		var control = new Mock<IVehicleCompartment>();
		control.SetupGet(x => x.InteriorCell).Returns(controlCell.Object);
		var deckhouse = new Mock<IVehicleCompartment>();
		deckhouse.SetupGet(x => x.InteriorCell).Returns(deckhouseCell.Object);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Compartments).Returns([control.Object, deckhouse.Object]);
		var exit = new Mock<MudSharp.Construction.Boundary.ICellExit>();
		exit.SetupGet(x => x.Origin).Returns(controlCell.Object);
		exit.SetupGet(x => x.Destination).Returns(deckhouseCell.Object);
		controlCell.Setup(x => x.ExitsFor(null!, false)).Returns([exit.Object]);

		Assert.IsTrue(MudSharp.Character.Character.IsPermittedVehicleInteriorExit(
			vehicle.Object, controlCell.Object, exit.Object));
		Assert.IsTrue(MudSharp.Character.Character.HasPermittedVehicleInteriorExit(
			vehicle.Object, controlCell.Object));
	}

	[TestMethod]
	public void RoomScaleOccupantMovement_DockingExitToExterior_IsRejected()
	{
		var interiorCell = new Mock<ICell>();
		interiorCell.SetupGet(x => x.Id).Returns(101L);
		var exteriorCell = new Mock<ICell>();
		exteriorCell.SetupGet(x => x.Id).Returns(42L);
		var compartment = new Mock<IVehicleCompartment>();
		compartment.SetupGet(x => x.InteriorCell).Returns(interiorCell.Object);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.Compartments).Returns([compartment.Object]);
		var dockingExit = new Mock<MudSharp.Construction.Boundary.ICellExit>();
		dockingExit.SetupGet(x => x.Origin).Returns(interiorCell.Object);
		dockingExit.SetupGet(x => x.Destination).Returns(exteriorCell.Object);

		Assert.IsFalse(MudSharp.Character.Character.IsPermittedVehicleInteriorExit(
			vehicle.Object, interiorCell.Object, dockingExit.Object));
	}

	[TestMethod]
	public void NonRoomScaleOccupantMovement_RemainsBlockedAcrossOrdinaryExit()
	{
		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Id).Returns(101L);
		var destination = new Mock<ICell>();
		destination.SetupGet(x => x.Id).Returns(102L);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomContainer);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		var exit = new Mock<MudSharp.Construction.Boundary.ICellExit>();
		exit.SetupGet(x => x.Origin).Returns(origin.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);

		Assert.IsFalse(MudSharp.Character.Character.IsPermittedVehicleInteriorExit(
			vehicle.Object, origin.Object, exit.Object));
	}

	[TestMethod]
	public void RoomScaleTopology_DuplicateDirectionInOneCompartment_IsRejected()
	{
		var control = Compartment(1, "Control");
		var deckhouse = Compartment(2, "Deckhouse");
		var hold = Compartment(3, "Hold");
		var first = Link(10, control, deckhouse, "aft", "forward");
		var second = Link(11, control, hold, "aft", "forward");

		var valid = VehiclePrototype.ValidateRoomScaleTopology(
			[control, deckhouse, hold], [first, second], [Access(deckhouse)], out var reason);

		Assert.IsFalse(valid);
		StringAssert.Contains(reason, "more than one internal exit named aft");
	}

	[TestMethod]
	public void BuilderHelp_AdvertisesHostedInteriorAndLinkCommands()
	{
		var help = (string)typeof(VehiclePrototype)
			.GetField("BuildingHelp", BindingFlags.NonPublic | BindingFlags.Static)!
			.GetRawConstantValue()!;

		StringAssert.Contains(help, "compartment interior <id>");
		StringAssert.Contains(help, "compartment link add <source id>");
		StringAssert.Contains(help, "compartment link remove <id>");
	}

	[TestMethod]
	public void VehicleLoad_MissingHostedCell_DoesNotSilentlyRegenerateInterior()
	{
		var vehicle = CreateVehicleWithMissingInterior(901);

		var compartment = vehicle.Compartments.Single();
		Assert.AreEqual(901L, compartment.InteriorCellId);
		Assert.IsNull(compartment.InteriorCell);
	}

	[TestMethod]
	public void ResolveBoardingAccess_NoViaSelection_UsesOpenRoomScaleDocking()
	{
		var exteriorCell = new Mock<ICell>();
		exteriorCell.SetupGet(x => x.Id).Returns(42L);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		prototype.SetupGet(x => x.Compartments).Returns([]);
		prototype.SetupGet(x => x.CompartmentLinks).Returns([]);
		var prototypes = new Mock<IUneditableRevisableAll<IVehiclePrototype>>();
		prototypes.Setup(x => x.Get(10L, 0)).Returns(prototype.Object);
		var cells = new Mock<IUneditableAll<ICell>>();
		cells.Setup(x => x.Get(42L)).Returns(exteriorCell.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.VehiclePrototypes).Returns(prototypes.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells.Object);
		var vehicle = new Vehicle(new DB.Vehicle
		{
			Id = 1L,
			Name = "Test Platform",
			VehicleProtoId = 10L,
			VehicleProtoRevision = 0,
			LocationType = (int)VehicleLocationType.Cell,
			CurrentCellId = 42L,
			CurrentRoomLayer = (int)RoomLayer.GroundLevel,
			MovementStatus = (int)VehicleMovementStatus.Stationary
		}, gameworld.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(exteriorCell.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var accessPrototype = new Mock<IVehicleAccessPointPrototype>();
		var access = new Mock<IVehicleAccessPoint>();
		access.SetupGet(x => x.Id).Returns(20L);
		access.SetupGet(x => x.Prototype).Returns(accessPrototype.Object);
		access.Setup(x => x.CanUse(It.IsAny<ICharacter>(), out It.Ref<string>.IsAny))
		      .Returns((ICharacter _, out string reason) =>
		      {
			      reason = string.Empty;
			      return true;
		      });
		((List<IVehicleAccessPoint>)typeof(Vehicle)
			.GetField("_accessPoints", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(vehicle)!).Add(access.Object);
		var docking = (VehicleDocking)RuntimeHelpers.GetUninitializedObject(typeof(VehicleDocking));
		typeof(VehicleDocking).GetField("<AccessPoint>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(docking, access.Object);
		typeof(VehicleDocking).GetField("<ExteriorCell>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(docking, exteriorCell.Object);
		typeof(VehicleDocking).GetField("<ExteriorLayer>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(docking, RoomLayer.GroundLevel);
		typeof(VehicleDocking).GetField("_registered", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(docking, true);
		typeof(VehicleDocking).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(docking, VehicleDockingState.BoardingOpen);
		((List<VehicleDocking>)typeof(Vehicle)
			.GetField("_dockings", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(vehicle)!).Add(docking);
		var slot = new Mock<IVehicleOccupantSlotPrototype>();

		var resolved = vehicle.ResolveBoardingAccess(actor.Object, slot.Object, null!);

		Assert.AreSame(access.Object, resolved);
	}

	[TestMethod]
	public void RebuildRouteDocking_PreservesClosedBoardingWindowEvenWhenAccessPointIsOpen()
	{
		Assert.IsFalse(VehicleDockingService.ShouldReopenRouteDocking(
			VehicleDockingState.DockedClosed,
			accessPointOpen: true,
			accessPointDisabled: false,
			accessPointLocked: false));
		Assert.IsTrue(VehicleDockingService.ShouldReopenRouteDocking(
			VehicleDockingState.BoardingOpen,
			accessPointOpen: true,
			accessPointDisabled: false,
			accessPointLocked: false));
		Assert.IsFalse(VehicleDockingService.ShouldReopenRouteDocking(
			VehicleDockingState.BoardingOpen,
			accessPointOpen: true,
			accessPointDisabled: true,
			accessPointLocked: false));
	}

	[TestMethod]
	public void HostedCellContext_MissingLoadedVehicle_DoesNotInvokeLazyVehicleLoader()
	{
		var vehicles = new Mock<IUneditableAll<IVehicle>>();
		vehicles.Setup(x => x.Get(42L)).Returns((IVehicle?)null);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Vehicles).Returns(vehicles.Object);
		var cell = (Cell)RuntimeHelpers.GetUninitializedObject(typeof(Cell));
		typeof(Cell)
			.GetProperty(nameof(Cell.Gameworld), BindingFlags.Instance | BindingFlags.Public)!
			.SetValue(cell, gameworld.Object);
		typeof(Cell)
			.GetField("_hostedVehicleId", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(cell, 42L);

		var hostedVehicle = typeof(Cell)
			.GetProperty("HostedVehicle", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(cell);

		Assert.IsNull(hostedVehicle);
		vehicles.Verify(x => x.Get(42L), Times.Once);
		gameworld.Verify(x => x.TryGetVehicle(It.IsAny<long>()), Times.Never);
	}

	[TestMethod]
	public void FleetAudit_InteriorMode_ReportsMissingHostedCell()
	{
		var vehicle = CreateVehicleWithMissingInterior(null);
		var readiness = new Mock<IVehicleOperationalReadinessService>();
		readiness
			.Setup(x => x.BuildReport(
				It.IsAny<IVehicle>(),
				It.IsAny<ICharacter>(),
				It.IsAny<VehicleHitchGraphMovePlan?>(),
				It.IsAny<IVehicleMovementProfilePrototype?>()))
			.Returns((IVehicle candidate, ICharacter _, VehicleHitchGraphMovePlan? _,
				IVehicleMovementProfilePrototype? _) =>
				new VehicleOperationalReadinessReport(candidate, [], [], [], [], []));
		var service = new VehicleFleetOperationsService(
			readiness.Object,
			new Mock<IVehicleHitchGraphService>().Object);

		var result = service.Audit(
			[vehicle],
			new Mock<ICharacter>().Object,
			VehicleFleetAuditMode.Interior);

		Assert.AreEqual(1, result.Findings.Count);
		Assert.AreEqual(VehicleOperationalSubsystem.Interior, result.Findings[0].Subsystem);
		StringAssert.Contains(result.Findings[0].Reason, "no hosted interior cell assigned");
		StringAssert.Contains(result.Findings[0].Hint, "vehicle recover <vehicle> interior fix");
	}

	[TestMethod]
	public void InteriorRecovery_PersistedHostedCell_RelinksInsteadOfCreatingReplacement()
	{
		var hostedCell = new Mock<ICell>();
		hostedCell.SetupGet(x => x.Id).Returns(901L);
		var vehicle = CreateVehicleWithMissingInterior(null, loadedCell: hostedCell.Object);
		var compartment = (VehicleCompartment)vehicle.Compartments.Single();
		using var context = CreateContext();
		context.Cells.Add(new DB.Cell
		{
			Id = 901,
			EffectData = "<Effects />",
			HostedVehicleId = vehicle.Id,
			HostedVehicleCompartmentId = compartment.Id
		});
		context.VehicleCompartments.Add(new DB.VehicleCompartment
		{
			Id = compartment.Id,
			VehicleId = vehicle.Id,
			VehicleCompartmentProtoId = compartment.Prototype.Id,
			Name = compartment.Name
		});
		context.SaveChanges();

		var valid = RoomScaleVehicleInteriorService.TryRelinkPersistedInterior(
			vehicle, compartment, context, out var relinked, out var reason);

		Assert.IsTrue(valid, reason);
		Assert.IsTrue(relinked);
		Assert.AreEqual(901L, compartment.InteriorCellId);
		Assert.AreSame(hostedCell.Object, compartment.InteriorCell);
		Assert.AreEqual(901L, context.VehicleCompartments.Find(compartment.Id)!.InteriorCellId);
		Assert.AreEqual(1, context.Cells.Count());
	}

	[TestMethod]
	public void InteriorRecovery_PersistedHostedCellNotLoaded_FailsClosed()
	{
		var vehicle = CreateVehicleWithMissingInterior(null);
		var compartment = (VehicleCompartment)vehicle.Compartments.Single();
		using var context = CreateContext();
		context.Cells.Add(new DB.Cell
		{
			Id = 902,
			EffectData = "<Effects />",
			HostedVehicleId = vehicle.Id,
			HostedVehicleCompartmentId = compartment.Id
		});
		context.VehicleCompartments.Add(new DB.VehicleCompartment
		{
			Id = compartment.Id,
			VehicleId = vehicle.Id,
			VehicleCompartmentProtoId = compartment.Prototype.Id,
			Name = compartment.Name
		});
		context.SaveChanges();

		var valid = RoomScaleVehicleInteriorService.TryRelinkPersistedInterior(
			vehicle, compartment, context, out var relinked, out var reason);

		Assert.IsFalse(valid);
		Assert.IsFalse(relinked);
		StringAssert.Contains(reason, "refused to create a duplicate");
		Assert.IsNull(compartment.InteriorCellId);
		Assert.IsNull(context.VehicleCompartments.Find(compartment.Id)!.InteriorCellId);
		Assert.AreEqual(1, context.Cells.Count());
	}

	[TestMethod]
	public void InteriorRecovery_PersistedHostedCellClaimedElsewhere_DoesNotStealCell()
	{
		var hostedCell = new Mock<ICell>();
		hostedCell.SetupGet(x => x.Id).Returns(903L);
		var vehicle = CreateVehicleWithMissingInterior(null, loadedCell: hostedCell.Object);
		var compartment = (VehicleCompartment)vehicle.Compartments.Single();
		using var context = CreateContext();
		context.Cells.Add(new DB.Cell
		{
			Id = 903,
			EffectData = "<Effects />",
			HostedVehicleId = vehicle.Id,
			HostedVehicleCompartmentId = compartment.Id
		});
		context.VehicleCompartments.AddRange(
			new DB.VehicleCompartment
			{
				Id = compartment.Id,
				VehicleId = vehicle.Id,
				VehicleCompartmentProtoId = compartment.Prototype.Id,
				Name = compartment.Name
			},
			new DB.VehicleCompartment
			{
				Id = 99,
				VehicleId = 2,
				VehicleCompartmentProtoId = 12,
				Name = "Other Compartment",
				InteriorCellId = 903
			});
		context.SaveChanges();

		var valid = RoomScaleVehicleInteriorService.TryRelinkPersistedInterior(
			vehicle, compartment, context, out var relinked, out var reason);

		Assert.IsFalse(valid);
		Assert.IsFalse(relinked);
		StringAssert.Contains(reason, "refused to steal");
		Assert.IsNull(compartment.InteriorCellId);
		Assert.AreEqual(903L, context.VehicleCompartments.Find(99L)!.InteriorCellId);
		Assert.AreEqual(1, context.Cells.Count());
	}

	[TestMethod]
	public void EnsureExteriorProjectionLink_UnlinkedCanonicalItem_RestoresComponentLink()
	{
		var exteriorComponent = new Mock<IVehicleExterior>();
		exteriorComponent.SetupGet(x => x.VehicleId).Returns((long?)null);
		var exteriorItem = new Mock<IGameItem>();
		exteriorItem.Setup(x => x.GetItemType<IVehicleExterior>()).Returns(exteriorComponent.Object);
		var vehicle = CreateVehicleWithMissingInterior(null, exteriorItem.Object);

		var valid = vehicle.EnsureExteriorProjectionLink(out var repaired, out var reason);

		Assert.IsTrue(valid, reason);
		Assert.IsTrue(repaired);
		exteriorComponent.Verify(x => x.LinkVehicle(vehicle), Times.Once);
	}

	[TestMethod]
	public void EnsureExteriorProjectionLink_ConflictingVehicle_DoesNotStealComponentLink()
	{
		var exteriorComponent = new Mock<IVehicleExterior>();
		exteriorComponent.SetupGet(x => x.VehicleId).Returns(77L);
		var exteriorItem = new Mock<IGameItem>();
		exteriorItem.Setup(x => x.GetItemType<IVehicleExterior>()).Returns(exteriorComponent.Object);
		var vehicle = CreateVehicleWithMissingInterior(null, exteriorItem.Object);

		var valid = vehicle.EnsureExteriorProjectionLink(out var repaired, out var reason);

		Assert.IsFalse(valid);
		Assert.IsFalse(repaired);
		StringAssert.Contains(reason, "vehicle #77");
		exteriorComponent.Verify(x => x.LinkVehicle(It.IsAny<IVehicle>()), Times.Never);
	}

	private static Vehicle CreateVehicleWithMissingInterior(long? interiorCellId,
		IGameItem? exteriorItem = null, ICell? loadedCell = null)
	{
		var compartment = Compartment(11, "Cab");
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Scale).Returns(VehicleScale.RoomScale);
		prototype.SetupGet(x => x.Compartments).Returns([compartment]);
		prototype.SetupGet(x => x.CompartmentLinks).Returns([]);
		prototype.SetupGet(x => x.MovementProfiles).Returns([]);

		var prototypes = new Mock<IUneditableRevisableAll<IVehiclePrototype>>();
		prototypes.Setup(x => x.Get(10, 0)).Returns(prototype.Object);
		var cells = new Mock<IUneditableAll<ICell>>();
		cells.Setup(x => x.Get(It.IsAny<long>()))
			.Returns((long id) => loadedCell?.Id == id ? loadedCell : null);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.VehiclePrototypes).Returns(prototypes.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells.Object);
		if (exteriorItem is not null)
		{
			gameworld.Setup(x => x.TryGetItem(42, true)).Returns(exteriorItem);
		}

		var row = new DB.Vehicle
		{
			Id = 1,
			Name = "Test Train",
			VehicleProtoId = 10,
			VehicleProtoRevision = 0,
			ExteriorItemId = exteriorItem is null ? null : 42,
			LocationType = (int)VehicleLocationType.Cell,
			CurrentRoomLayer = (int)RoomLayer.GroundLevel,
			MovementStatus = (int)VehicleMovementStatus.Stationary
		};
		row.Compartments.Add(new DB.VehicleCompartment
		{
			Id = 21,
			VehicleId = 1,
			VehicleCompartmentProtoId = compartment.Id,
			Name = compartment.Name,
			InteriorCellId = interiorCellId
		});
		return new Vehicle(row, gameworld.Object);
	}

	private static FuturemudDatabaseContext CreateContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static IVehicleCompartmentPrototype Compartment(long id, string name, bool hasTerrain = true)
	{
		var compartment = new Mock<IVehicleCompartmentPrototype>();
		compartment.SetupGet(x => x.Id).Returns(id);
		compartment.SetupGet(x => x.Name).Returns(name);
		compartment.SetupGet(x => x.InteriorTerrain).Returns(
			hasTerrain ? new Mock<ITerrain>().Object : null);
		return compartment.Object;
	}

	private static IVehicleCompartmentLinkPrototype Link(long id,
		IVehicleCompartmentPrototype source, IVehicleCompartmentPrototype destination,
		string outboundDirection, string inboundDirection)
	{
		var link = new Mock<IVehicleCompartmentLinkPrototype>();
		link.SetupGet(x => x.Id).Returns(id);
		link.SetupGet(x => x.SourceCompartment).Returns(source);
		link.SetupGet(x => x.DestinationCompartment).Returns(destination);
		link.SetupGet(x => x.OutboundDirection).Returns(outboundDirection);
		link.SetupGet(x => x.InboundDirection).Returns(inboundDirection);
		link.SetupGet(x => x.OutboundDescription).Returns($"the {destination.Name}");
		link.SetupGet(x => x.InboundDescription).Returns($"the {source.Name}");
		return link.Object;
	}

	private static IVehicleAccessPointPrototype Access(IVehicleCompartmentPrototype compartment)
	{
		var access = new Mock<IVehicleAccessPointPrototype>();
		access.SetupGet(x => x.Name).Returns("Loading Ramp");
		access.SetupGet(x => x.Compartment).Returns(compartment);
		return access.Object;
	}
}
