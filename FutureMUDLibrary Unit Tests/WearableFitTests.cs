#nullable enable
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.GameItems.Inventory.Size;
namespace MudSharp_Unit_Tests;
[TestClass]
public class WearableFitTests
{
	[TestMethod]
	public void WearableFit_XmlRoundTripPreservesScalarAndMeasurementValues()
	{
		var scalar = WearableFit.ForScalar(12.5);
		var measurements = WearableFit.ForMeasurements(1.73, 68.25);

		Assert.AreEqual(scalar, WearableFit.FromXml(scalar.SaveToXml()));
		Assert.AreEqual(measurements, WearableFit.FromXml(measurements.SaveToXml()));
		Assert.AreEqual(WearableFitKind.Unfitted, WearableFit.FromXml(null).Kind);
		Assert.AreEqual(WearableFitKind.Universal, WearableFit.FromXml(WearableFit.Universal.SaveToXml()).Kind);
	}

	[TestMethod]
	public void WearableFit_RejectsNonPositiveMeasurementsAndInvalidScalar()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => WearableFit.ForMeasurements(0, 60));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => WearableFit.ForMeasurements(1.7, double.NaN));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => WearableFit.ForScalar(-1));
	}

	[TestMethod]
	public void MeasurementFit_UsesIndependentInclusiveHeightAndWeightToleranceBoundaries()
	{
		var fit = WearableFit.ForMeasurements(2.0, 100.0);
		var exactlyAtBoundary = Body(2.2, 90.0);
		var outsideHeight = Body(2.2001, 100.0);
		var outsideWeight = Body(2.0, 89.99);

		Assert.IsTrue(fit.MatchesMeasurements(exactlyAtBoundary, 0.1, 0.1));
		Assert.IsFalse(fit.MatchesMeasurements(outsideHeight, 0.1, 0.1));
		Assert.IsFalse(fit.MatchesMeasurements(outsideWeight, 0.1, 0.1));
	}

	private static IBody Body(double height, double weight)
	{
		var body = new Mock<IBody>();
		body.Setup(x => x.Height).Returns(height);
		body.Setup(x => x.Weight).Returns(weight);
		return body.Object;
	}
}
