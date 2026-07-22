#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework.Revision;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleServiceAuditTests
{
	[DataTestMethod]
	[DataRow(RevisionStatus.Current, true)]
	[DataRow(RevisionStatus.Revised, true)]
	[DataRow(RevisionStatus.UnderDesign, false)]
	[DataRow(RevisionStatus.PendingRevision, false)]
	[DataRow(RevisionStatus.Rejected, false)]
	[DataRow(RevisionStatus.Obsolete, false)]
	public void IsApprovedPinnedRevision_PreservesSupersededApprovedRoutes(
		RevisionStatus status,
		bool expected)
	{
		Assert.AreEqual(expected, VehicleService.IsApprovedPinnedRevision(status));
	}

	[TestMethod]
	public void AuditPlatformBindings_ForeignAccessPointPrototype_ReturnsActionableError()
	{
		var assignedAccess = CreateAccessPrototype(100L, "front doors");
		var foreignAccess = CreateAccessPrototype(200L, "foreign gangway");
		var binding = CreateBinding(300L, foreignAccess.Object);
		var route = CreateRoute(CreateStop(400L, "Central", binding.Object).Object);
		var vehicle = CreateVehicle(500L, assignedAccess.Object, []);

		var errors = VehicleService.AuditPlatformBindings(route.Object, vehicle.Object);

		Assert.AreEqual(1, errors.Count);
		StringAssert.Contains(errors[0], "platform binding #300");
		StringAssert.Contains(errors[0], "access-point prototype #200 foreign gangway");
		StringAssert.Contains(errors[0], "does not belong to assigned vehicle prototype #50 test train");
		StringAssert.Contains(errors[0], "Assign a compatible vehicle or correct the platform binding.");
	}

	[TestMethod]
	public void AuditPlatformBindings_OwnedPrototypeWithoutLiveAccessPoint_ReturnsRecoveryError()
	{
		var assignedAccess = CreateAccessPrototype(100L, "front doors");
		var binding = CreateBinding(301L, assignedAccess.Object);
		var route = CreateRoute(CreateStop(401L, "Harbour", binding.Object).Object);
		var vehicle = CreateVehicle(501L, assignedAccess.Object, []);

		var errors = VehicleService.AuditPlatformBindings(route.Object, vehicle.Object);

		Assert.AreEqual(1, errors.Count);
		StringAssert.Contains(errors[0], "assigned vehicle #501 service vehicle has no matching live access point");
		StringAssert.Contains(errors[0], "repair/recreate its access-point records");
	}

	[TestMethod]
	public void AuditPlatformBindings_OwnedPrototypeWithMatchingLiveAccessPoint_ReturnsNoErrors()
	{
		var assignedAccess = CreateAccessPrototype(100L, "front doors");
		var binding = CreateBinding(302L, assignedAccess.Object);
		var route = CreateRoute(CreateStop(402L, "Terminus", binding.Object).Object);
		var vehicle = CreateVehicle(502L, assignedAccess.Object, out var liveAccess);
		liveAccess.SetupGet(x => x.Vehicle).Returns(vehicle.Object);

		var errors = VehicleService.AuditPlatformBindings(route.Object, vehicle.Object);

		Assert.AreEqual(0, errors.Count);
	}

	[TestMethod]
	public void AuditPlatformBindings_RoomScaleStopWithoutBindingFailsClosed()
	{
		var assignedAccess = CreateAccessPrototype(100L, "front doors");
		var route = CreateRoute(CreateStop(403L, "Unbound Terminus").Object);
		var vehicle = CreateVehicle(503L, assignedAccess.Object, [], VehicleScale.RoomScale);

		var errors = VehicleService.AuditPlatformBindings(route.Object, vehicle.Object);

		Assert.AreEqual(1, errors.Count);
		StringAssert.Contains(errors[0], "Route stop #403 Unbound Terminus has no platform binding");
		StringAssert.Contains(errors[0], "RoomScale vehicle #503 service vehicle");
	}

	[TestMethod]
	public void AuditPlatformBindings_RoomScalePartialRouteReportsEveryUnboundStop()
	{
		var assignedAccess = CreateAccessPrototype(100L, "front doors");
		var binding = CreateBinding(304L, assignedAccess.Object);
		var route = CreateRoute(
			CreateStop(404L, "Bound Origin", binding.Object).Object,
			CreateStop(405L, "Missing Middle").Object,
			CreateStop(406L, "Missing Terminus").Object);
		var vehicle = CreateVehicle(504L, assignedAccess.Object, out var liveAccess,
			VehicleScale.RoomScale);
		liveAccess.SetupGet(x => x.Vehicle).Returns(vehicle.Object);

		var errors = VehicleService.AuditPlatformBindings(route.Object, vehicle.Object);

		Assert.AreEqual(2, errors.Count);
		Assert.IsTrue(errors.Any(x => x.Contains("#405 Missing Middle")));
		Assert.IsTrue(errors.Any(x => x.Contains("#406 Missing Terminus")));
	}

	private static Mock<IVehicle> CreateVehicle(
		long id,
		IVehicleAccessPointPrototype prototypeAccess,
		IEnumerable<IVehicleAccessPoint> liveAccessPoints,
		VehicleScale scale = VehicleScale.ItemScale)
	{
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.Id).Returns(50L);
		prototype.SetupGet(x => x.Name).Returns("test train");
		prototype.SetupGet(x => x.AccessPoints).Returns([prototypeAccess]);
		prototype.SetupGet(x => x.Scale).Returns(scale);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(id);
		vehicle.SetupGet(x => x.Name).Returns("service vehicle");
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.AccessPoints).Returns(liveAccessPoints);
		return vehicle;
	}

	private static Mock<IVehicle> CreateVehicle(
		long id,
		IVehicleAccessPointPrototype prototypeAccess,
		out Mock<IVehicleAccessPoint> liveAccess,
		VehicleScale scale = VehicleScale.ItemScale)
	{
		liveAccess = new Mock<IVehicleAccessPoint>();
		liveAccess.SetupGet(x => x.Prototype).Returns(prototypeAccess);
		return CreateVehicle(id, prototypeAccess, [liveAccess.Object], scale);
	}

	private static Mock<IVehicleRoute> CreateRoute(params IVehicleRouteStop[] stops)
	{
		var route = new Mock<IVehicleRoute>();
		route.SetupGet(x => x.Stops).Returns(stops);
		return route;
	}

	private static Mock<IVehicleRouteStop> CreateStop(
		long id,
		string name,
		params IVehicleRoutePlatformBinding[] bindings)
	{
		var stop = new Mock<IVehicleRouteStop>();
		stop.SetupGet(x => x.Id).Returns(id);
		stop.SetupGet(x => x.Name).Returns(name);
		stop.SetupGet(x => x.Sequence).Returns(0);
		stop.SetupGet(x => x.PlatformBindings).Returns(bindings);
		return stop;
	}

	private static Mock<IVehicleRoutePlatformBinding> CreateBinding(
		long id,
		IVehicleAccessPointPrototype accessPoint)
	{
		var binding = new Mock<IVehicleRoutePlatformBinding>();
		binding.SetupGet(x => x.Id).Returns(id);
		binding.SetupGet(x => x.AccessPoint).Returns(accessPoint);
		return binding;
	}

	private static Mock<IVehicleAccessPointPrototype> CreateAccessPrototype(long id, string name)
	{
		var access = new Mock<IVehicleAccessPointPrototype>();
		access.SetupGet(x => x.Id).Returns(id);
		access.SetupGet(x => x.Name).Returns(name);
		return access;
	}
}
