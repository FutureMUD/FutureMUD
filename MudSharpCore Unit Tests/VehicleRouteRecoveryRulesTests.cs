#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Vehicles;

namespace MudSharpCore_Unit_Tests.Vehicles;

[TestClass]
public class VehicleRouteRecoveryRulesTests
{
	[TestMethod]
	public void TryResolveStepIndex_MidLinearCheckpoint_ResumesContainingStep()
	{
		var routeRoom = new Mock<IRoom>().Object;
		var platformRoom = new Mock<IRoom>().Object;
		var linear = LinearStep(routeRoom, 100.0, 700.0);
		var exit = ExitStep(
			new SpatialLocation(routeRoom, RoomLayer.GroundLevel, 700.0),
			new SpatialLocation(platformRoom, RoomLayer.GroundLevel));

		var result = VehicleRouteRecoveryRules.TryResolveStepIndex(
			[linear.Object, exit.Object],
			new SpatialLocation(routeRoom, RoomLayer.GroundLevel, 425.0),
			out var index,
			out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(0, index);
	}

	[TestMethod]
	public void TryResolveStepIndex_ExactStepBoundary_ContinuesWithFollowingStep()
	{
		var routeRoom = new Mock<IRoom>().Object;
		var platformRoom = new Mock<IRoom>().Object;
		var linear = LinearStep(routeRoom, 100.0, 700.0);
		var exit = ExitStep(
			new SpatialLocation(routeRoom, RoomLayer.GroundLevel, 700.0),
			new SpatialLocation(platformRoom, RoomLayer.GroundLevel));

		var result = VehicleRouteRecoveryRules.TryResolveStepIndex(
			[linear.Object, exit.Object],
			new SpatialLocation(routeRoom, RoomLayer.GroundLevel, 700.0),
			out var index,
			out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(1, index);
	}

	[TestMethod]
	public void TryResolveStepIndex_AtLegDestination_ReportsEveryStepComplete()
	{
		var routeRoom = new Mock<IRoom>().Object;
		var platformRoom = new Mock<IRoom>().Object;
		var linear = LinearStep(routeRoom, 100.0, 700.0);
		var exit = ExitStep(
			new SpatialLocation(routeRoom, RoomLayer.GroundLevel, 700.0),
			new SpatialLocation(platformRoom, RoomLayer.GroundLevel));

		var steps = new IVehicleRouteStep[] { linear.Object, exit.Object };
		var result = VehicleRouteRecoveryRules.TryResolveStepIndex(
			steps,
			new SpatialLocation(platformRoom, RoomLayer.GroundLevel),
			out var index,
			out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(steps.Length, index);
	}

	[TestMethod]
	public void TryResolveStepIndex_RevisitedCoordinate_FailsClosedAsAmbiguous()
	{
		var firstRoom = new Mock<IRoom>().Object;
		var secondRoom = new Mock<IRoom>().Object;
		var outbound = ExitStep(
			new SpatialLocation(firstRoom, RoomLayer.GroundLevel),
			new SpatialLocation(secondRoom, RoomLayer.GroundLevel));
		var returnStep = ExitStep(
			new SpatialLocation(secondRoom, RoomLayer.GroundLevel),
			new SpatialLocation(firstRoom, RoomLayer.GroundLevel));

		var result = VehicleRouteRecoveryRules.TryResolveStepIndex(
			[outbound.Object, returnStep.Object],
			new SpatialLocation(firstRoom, RoomLayer.GroundLevel),
			out _,
			out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "more than once");
	}

	private static Mock<IVehicleRouteLinearStep> LinearStep(IRoom room, double origin, double destination)
	{
		var step = new Mock<IVehicleRouteLinearStep>();
		step.SetupGet(x => x.Origin)
			.Returns(new SpatialLocation(room, RoomLayer.GroundLevel, origin));
		step.SetupGet(x => x.Destination)
			.Returns(new SpatialLocation(room, RoomLayer.GroundLevel, destination));
		return step;
	}

	private static Mock<IVehicleRouteExitStep> ExitStep(SpatialLocation origin, SpatialLocation destination)
	{
		var step = new Mock<IVehicleRouteExitStep>();
		step.SetupGet(x => x.Origin).Returns(origin);
		step.SetupGet(x => x.Destination).Returns(destination);
		return step;
	}
}
