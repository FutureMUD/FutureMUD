#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.Vehicles;
using System.Text;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleStatusRouteReadinessTests
{
	[TestMethod]
	public void AppendMovementReadiness_RouteCellExternallyPulledVehicle_UsesLongitudinalPreflight()
	{
		var activeMovement = new Mock<IMovement>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Movement).Returns(activeMovement.Object);
		var routeDefinition = new Mock<IRouteCellDefinition>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.RouteDefinition).Returns(routeDefinition.Object);
		var routeProfile = new Mock<IVehicleMovementProfilePrototype>();
		routeProfile.SetupGet(x => x.Id).Returns(10L);
		routeProfile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.Route);
		routeProfile.SetupGet(x => x.RoutePropulsionMode).Returns(RouteVehiclePropulsionMode.ExternallyPulled);
		routeProfile.SetupGet(x => x.IsDefault).Returns(true);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns([routeProfile.Object]);
		var movementState = new Mock<IVehicleMovementState>();
		movementState.SetupGet(x => x.MovementStatus).Returns(VehicleMovementStatus.Moving);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Location).Returns(cell.Object);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.MovementState).Returns(movementState.Object);
		var service = new Mock<IVehicleOperationalReadinessService>();
		service.Setup(x => x.BuildLongitudinalMovementReadiness(
				It.Is<VehicleLongitudinalReadinessRequest>(request =>
					ReferenceEquals(request.Vehicle, vehicle.Object) &&
					ReferenceEquals(request.Actor, actor.Object) &&
					ReferenceEquals(request.MovementProfile, routeProfile.Object) &&
					ReferenceEquals(request.ContinuingMovement, activeMovement.Object))))
			.Returns(new VehicleMovementReadinessResult(true, string.Empty, null, null, []));
		var sb = new StringBuilder();

		VehicleModule.AppendMovementReadiness(sb, actor.Object, vehicle.Object, service.Object);

		StringAssert.Contains(sb.ToString(), "RouteCell readiness:");
		StringAssert.Contains(sb.ToString(), "ready");
		Assert.IsFalse(sb.ToString().Contains("Cell-exit readiness:"));
		service.Verify(x => x.BuildLongitudinalMovementReadiness(
			It.IsAny<VehicleLongitudinalReadinessRequest>()), Times.Once);
		service.Verify(x => x.BuildMovementReadiness(It.IsAny<VehicleMovementReadinessRequest>()), Times.Never);
	}

	[TestMethod]
	public void AppendMovementReadiness_OrdinaryCell_PreservesCellExitPreflight()
	{
		var actor = new Mock<ICharacter>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.RouteDefinition).Returns((IRouteCellDefinition?)null);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns([]);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Location).Returns(cell.Object);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		var service = new Mock<IVehicleOperationalReadinessService>();
		service.Setup(x => x.BuildMovementReadiness(It.IsAny<VehicleMovementReadinessRequest>()))
			.Returns(new VehicleMovementReadinessResult(true, string.Empty, null, null, []));
		var sb = new StringBuilder();

		VehicleModule.AppendMovementReadiness(sb, actor.Object, vehicle.Object, service.Object);

		StringAssert.Contains(sb.ToString(), "Cell-exit readiness:");
		StringAssert.Contains(sb.ToString(), "ready");
		Assert.IsFalse(sb.ToString().Contains("RouteCell readiness:"));
		service.Verify(x => x.BuildMovementReadiness(It.IsAny<VehicleMovementReadinessRequest>()), Times.Once);
		service.Verify(x => x.BuildLongitudinalMovementReadiness(
			It.IsAny<VehicleLongitudinalReadinessRequest>()), Times.Never);
	}
}
