#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using System;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SurfaceLiquidSpatialRulesTests
{
	[TestMethod]
	public void PointSpill_IsVisibleOnlyWithinImmediateRouteDistance()
	{
		Assert.IsTrue(SurfaceLiquidSpatialRules.IsVisible(7_150.0, 7_152.0, 3.0));
		Assert.IsFalse(SurfaceLiquidSpatialRules.IsVisible(7_150.0, 9_000.0, 3.0));
		Assert.IsTrue(SurfaceLiquidSpatialRules.IsVisible(null, 9_000.0, 3.0),
			"Uniform environmental surface liquid remains visible throughout the RouteCell.");
	}

	[TestMethod]
	public void Normalise_RouteBoundsAndOrdinaryCompatibility_AreFailClosed()
	{
		var route = new Mock<IRouteCellDefinition>();
		route.SetupGet(x => x.LengthMetres).Returns(10_000.0);

		Assert.AreEqual(7_150.123, SurfaceLiquidSpatialRules.Normalise(route.Object, 7_150.1234));
		Assert.IsNull(SurfaceLiquidSpatialRules.Normalise(route.Object, null));
		Assert.IsNull(SurfaceLiquidSpatialRules.Normalise(null, null));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			SurfaceLiquidSpatialRules.Normalise(route.Object, 10_001.0));
		Assert.ThrowsException<ArgumentException>(() =>
			SurfaceLiquidSpatialRules.Normalise(null, 1.0));
	}
}
