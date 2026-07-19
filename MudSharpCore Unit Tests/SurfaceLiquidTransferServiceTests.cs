#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SurfaceLiquidTransferServiceTests
{
	[DataTestMethod]
	[DataRow(false, false, false, 1)]
	[DataRow(false, true, true, 0)]
	[DataRow(false, true, false, 2)]
	public void SelectRoute_HandlesEmptyDisabledSwimmingAndSurfaceCases(bool empty, bool enabled, bool swimming,
		int expected)
	{
		Assert.AreEqual(expected, (int)SurfaceLiquidTransferService.SelectRoute(empty, enabled, swimming));
	}

	[TestMethod]
	public void SelectRoute_EmptyMixture_DoesNotReadPuddleConfiguration()
	{
		var route = SurfaceLiquidTransferService.SelectRoute(
			true,
			() => throw new AssertFailedException("Puddle configuration should not be evaluated."),
			false);

		Assert.AreEqual(SurfaceLiquidRoute.Ignore, route);
	}
	[DataTestMethod]
	[DataRow(0.01, 0.02, 0.02)]
	[DataRow(10.0, 0.02, 3.0)]
	public void DryAfterMachineCycle_UsesMinimumOrThirtyPercent(double currentVolume, double minimum,
		double expected)
	{
		var state = new Mock<ISurfaceLiquidState>();
		state.SetupGet(x => x.LiquidVolume).Returns(currentVolume);

		SurfaceLiquidTransferService.DryAfterMachineCycle(state.Object, minimum);

		state.Verify(x => x.Dry(It.Is<double>(value => Math.Abs(value - expected) < 0.0001), false), Times.Once);
	}
}
