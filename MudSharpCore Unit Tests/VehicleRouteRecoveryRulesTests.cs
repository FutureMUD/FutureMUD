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
		var routeCell = new Mock<ICell>().Object;
		var platformCell = new Mock<ICell>().Object;
		var linear = LinearStep(routeCell, 100.0, 700.0);
		var exit = ExitStep(
			new SpatialLocation(routeCell, RoomLayer.GroundLevel, 700.0),
			new SpatialLocation(platformCell, RoomLayer.GroundLevel));

		var result = VehicleRouteRecoveryRules.TryResolveStepIndex(
			[linear.Object, exit.Object],
			new SpatialLocation(routeCell, RoomLayer.GroundLevel, 425.0),
			out var index,
			out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(0, index);
	}

	[TestMethod]
	public void TryResolveStepIndex_ExactStepBoundary_ContinuesWithFollowingStep()
	{
		var routeCell = new Mock<ICell>().Object;
		var platformCell = new Mock<ICell>().Object;
		var linear = LinearStep(routeCell, 100.0, 700.0);
		var exit = ExitStep(
			new SpatialLocation(routeCell, RoomLayer.GroundLevel, 700.0),
			new SpatialLocation(platformCell, RoomLayer.GroundLevel));

		var result = VehicleRouteRecoveryRules.TryResolveStepIndex(
			[linear.Object, exit.Object],
			new SpatialLocation(routeCell, RoomLayer.GroundLevel, 700.0),
			out var index,
			out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(1, index);
	}

	[TestMethod]
	public void TryResolveStepIndex_AtLegDestination_ReportsEveryStepComplete()
	{
		var routeCell = new Mock<ICell>().Object;
		var platformCell = new Mock<ICell>().Object;
		var linear = LinearStep(routeCell, 100.0, 700.0);
		var exit = ExitStep(
			new SpatialLocation(routeCell, RoomLayer.GroundLevel, 700.0),
			new SpatialLocation(platformCell, RoomLayer.GroundLevel));

		var steps = new IVehicleRouteStep[] { linear.Object, exit.Object };
		var result = VehicleRouteRecoveryRules.TryResolveStepIndex(
			steps,
			new SpatialLocation(platformCell, RoomLayer.GroundLevel),
			out var index,
			out var reason);

		Assert.IsTrue(result, reason);
		Assert.AreEqual(steps.Length, index);
	}

	[TestMethod]
	public void TryResolveStepIndex_RevisitedCoordinate_FailsClosedAsAmbiguous()
	{
		var firstCell = new Mock<ICell>().Object;
		var secondCell = new Mock<ICell>().Object;
		var outbound = ExitStep(
			new SpatialLocation(firstCell, RoomLayer.GroundLevel),
			new SpatialLocation(secondCell, RoomLayer.GroundLevel));
		var returnStep = ExitStep(
			new SpatialLocation(secondCell, RoomLayer.GroundLevel),
			new SpatialLocation(firstCell, RoomLayer.GroundLevel));

		var result = VehicleRouteRecoveryRules.TryResolveStepIndex(
			[outbound.Object, returnStep.Object],
			new SpatialLocation(firstCell, RoomLayer.GroundLevel),
			out _,
			out var reason);

		Assert.IsFalse(result);
		StringAssert.Contains(reason, "more than once");
	}

	private static Mock<IVehicleRouteLinearStep> LinearStep(ICell cell, double origin, double destination)
	{
		var step = new Mock<IVehicleRouteLinearStep>();
		step.SetupGet(x => x.Origin)
			.Returns(new SpatialLocation(cell, RoomLayer.GroundLevel, origin));
		step.SetupGet(x => x.Destination)
			.Returns(new SpatialLocation(cell, RoomLayer.GroundLevel, destination));
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
