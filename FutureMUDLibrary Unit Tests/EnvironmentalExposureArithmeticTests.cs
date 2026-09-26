#nullable enable
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Form.Material;

namespace FutureMUDLibrary_Unit_Tests;

[TestClass]
public class EnvironmentalExposureArithmeticTests
{
	private static ExposureArithmetic.Result Resolve(double e, double d, double c, double seconds, double? volume) =>
		ExposureArithmetic.Resolve(new[] { new ExposureArithmetic.Demand(e, d, 2 * d, 0, c) }, seconds, volume)[0];

	[TestMethod]
	public void N01_LocalisedReferenceArea_IsPartitionedWithoutMultiplyingWork()
	{
		Assert.AreEqual(10, Resolve(0.1, 20, 0, 5, null).Damage, 1e-12);
		Assert.AreEqual(10, Enumerable.Range(0, 4).Sum(_ => Resolve(0.025, 20, 0, 5, null).Damage), 1e-12);
	}
	[TestMethod]
	public void N02_Exhaustion_ClipsAllOutputsToTheCommonActiveTime()
	{
		var result = Resolve(1, 8, 2, 30, 50);
		Assert.AreEqual(25, result.Work); Assert.AreEqual(50, result.Consumed);
		Assert.AreEqual(200, result.Damage); Assert.AreEqual(400, result.Pain);
		Assert.AreEqual(0, Resolve(1, 8, 2, 5, 0).Damage);
	}
	[TestMethod]
	public void N03_MaterialSpecificConsumption_ProducesDistinctFiniteBounds()
	{
		Assert.AreEqual(50, Resolve(1, 4, 4, 150, 50).Damage);
		Assert.AreEqual(100, Resolve(1, 1, 0.5, 150, 50).Damage);
	}
	[TestMethod]
	public void N04_NonConsumingContact_DoesNotDebitTheSource()
	{
		var result = Resolve(1, 8, 0, 30, 50);
		Assert.AreEqual(240, result.Damage); Assert.AreEqual(0, result.Consumed);
	}
	[TestMethod]
	public void N05_SimultaneousContacts_ShareOneBudgetAndAreOrderIndependent()
	{
		var demands = new[] { new ExposureArithmetic.Demand(0.5, 10, 0, 0, 4), new ExposureArithmetic.Demand(0.5, 10, 0, 0, 4) };
		var result = ExposureArithmetic.Resolve(demands, 10, 20);
		Assert.IsTrue(result.All(x => x.Damage == 25 && x.Consumed == 10));
		CollectionAssert.AreEqual(result.ToArray(), ExposureArithmetic.Resolve(demands.Reverse().ToArray(), 10, 20).Reverse().ToArray());
	}
	[TestMethod]
	public void NonConsumingChannel_SharingAnExhaustedSpecies_StopsAtTheSameTime()
	{
		var result = ExposureArithmetic.Resolve(new[] { new ExposureArithmetic.Demand(1, 10, 0, 0, 2), new ExposureArithmetic.Demand(1, 5, 0, 0, 0) }, 20, 10);
		Assert.AreEqual(25, result[1].Damage); Assert.AreEqual(0, result[1].Consumed);
	}
	[TestMethod]
	public void N07_Dilution_UsesAcidConcentrationAndCoverage()
	{
		var intensity = ExposureArithmetic.ComponentIntensity(10, 100, 100, 1);
		Assert.AreEqual(20, Resolve(intensity, 20, 0, 10, 10).Damage, 1e-12);
		Assert.AreEqual(0.01, ExposureArithmetic.ComponentIntensity(1, 100, 100, 1), 1e-12);
	}
	[TestMethod]
	public void N09_ThermalThresholdUnitsAndCap_AreExplicit()
	{
		var celsius = ExposureArithmetic.ThermalRate(140, 100, 0.5, 60);
		var fahrenheitConverted = ExposureArithmetic.ThermalRate((284 - 32) * 5.0 / 9, (212 - 32) * 5.0 / 9, 0.5, 60);
		var kelvinConverted = ExposureArithmetic.ThermalRate(413.15 - 273.15, 373.15 - 273.15, 0.5, 60);
		Assert.AreEqual(10, celsius * 0.25 * 2, 1e-10);
		Assert.AreEqual(celsius, fahrenheitConverted, 1e-10); Assert.AreEqual(celsius, kelvinConverted, 1e-10);
		Assert.AreEqual(0, ExposureArithmetic.ThermalRate(100, 100, 0.5, 60));
		Assert.AreEqual(0, ExposureArithmetic.ThermalRate(1000, null, 0.5, 60));
		Assert.AreEqual(30, ExposureArithmetic.ThermalRate(1000, 100, 0.5, 60) * 0.25 * 2);
	}
	[TestMethod]
	public void ThermalHookPersistence_KeepsInvalidConfiguredReferenceForFailClosedResolution()
	{
		var material = new MaterialExposureProperties { ThermalIntensityProgId = 71, ThermalSlope = 0.25, ThermalCap = 6 };
		var roundTrip = MaterialExposureProperties.Load(material.Save());
		Assert.AreEqual(71L, roundTrip.ThermalIntensityProgId);
		Assert.AreEqual(0.25, roundTrip.ThermalSlope);
		Assert.AreEqual(-1L, MaterialExposureProperties.Load("<Exposure IntensityProg='invalid'/>").ThermalIntensityProgId);
	}
	[TestMethod]
	public void N10_SubTickContact_UsesActualActiveSeconds()
	{
		Assert.AreEqual(2, Resolve(1, 10, 0, 0.3 - 0.1, null).Damage, 1e-12);
		Assert.AreEqual(10, Enumerable.Range(0, 5).Sum(_ => Resolve(1, 10, 0, 0.2, null).Damage), 1e-12);
	}
	[DataTestMethod]
	[DataRow(double.NaN)] [DataRow(double.PositiveInfinity)] [DataRow(-1.0)]
	public void InvalidRates_AreRejected(double value) => Assert.ThrowsException<ArgumentOutOfRangeException>(() => Resolve(1, value, 1, 1, 1));

	[DataTestMethod]
	[DataRow(0.1, 20.0, 0.0, 5.0, 50.0)]
	[DataRow(1.0, 8.0, 2.0, 30.0, 50.0)]
	[DataRow(1.0, 4.0, 4.0, 150.0, 50.0)]
	[DataRow(1.0, 1.0, 0.5, 150.0, 50.0)]
	[DataRow(1.0, 8.0, 0.0, 30.0, 50.0)]
	[DataRow(0.1, 20.0, 0.0, 10.0, 10.0)]
	public void ConstantFixtures_PartitionAndNumericSaveResumeConserveWorkAndSource(double area, double damage, double consumption, double seconds, double volume)
	{
		var expected = Resolve(area, damage, consumption, seconds, volume);
		var remaining = volume; var actualDamage = 0.0;
		for (var i = 0; i < 20; i++)
		{
			var result = Resolve(area, damage, consumption, seconds / 20, remaining);
			remaining -= result.Consumed; actualDamage += result.Damage;
			if (i == 9) remaining = double.Parse(remaining.ToString("R", System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture);
		}
		Assert.AreEqual(expected.Damage, actualDamage, 1e-8);
		Assert.AreEqual(volume - expected.Consumed, remaining, 1e-8);
	}
}
