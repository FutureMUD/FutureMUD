#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LiquidMixtureMassTests
{
	[TestMethod]
	[DataRow(1000.0, 1.0, 0.001, 1.0, 1000.0)]
	[DataRow(990.0, 1.0, 0.001, 1.0, 990.0)]
	[DataRow(8000.0, 0.001, 0.001, 125.0, 1000.0)]
	[DataRow(1000.0, 1.0, 1.0, 1.0, 1.0)]
	public void NativeDensityAndFluidUnits_ProducePhysicalMass(double density, double fluidToLitres,
		double weightToKilograms, double volume, double mass)
	{
		var liquid = new Mock<ILiquid>(); liquid.SetupGet(x => x.Density).Returns(density);
		var units = new Mock<IUnitManager>(); units.SetupGet(x => x.BaseFluidToLitres).Returns(fluidToLitres);
		units.SetupGet(x => x.BaseWeightToKilograms).Returns(weightToKilograms);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.UnitManager).Returns(units.Object);
		var mixture = new LiquidMixture(liquid.Object, volume, world.Object);
		Assert.AreEqual(mass, mixture.TotalWeight, 0.0000001);
		Assert.AreEqual(volume, mixture.TotalVolume);
		mixture.SetLiquidVolume(volume / 2);
		Assert.AreEqual(mass / 2, mixture.TotalWeight, 0.0000001);
	}

	[TestMethod]
	public void PersistedMixtureReload_ConservesConstituentsAndCorrectMass()
	{
		var water = new Mock<ILiquid>(); water.SetupGet(x => x.Id).Returns(1); water.SetupGet(x => x.Density).Returns(1000);
		var wine = new Mock<ILiquid>(); wine.SetupGet(x => x.Id).Returns(2); wine.SetupGet(x => x.Density).Returns(990);
		var units = new Mock<IUnitManager>(); units.SetupGet(x => x.BaseFluidToLitres).Returns(1);
		units.SetupGet(x => x.BaseWeightToKilograms).Returns(0.001);
		var liquids = new Mock<IUneditableAll<ILiquid>>();
		liquids.Setup(x => x.Get(1)).Returns(water.Object);
		liquids.Setup(x => x.Get(2)).Returns(wine.Object);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.UnitManager).Returns(units.Object);
		world.SetupGet(x => x.Liquids).Returns(liquids.Object);
		var original = new LiquidMixture([new LiquidInstance { Liquid = water.Object, Amount = 1 },
			new LiquidInstance { Liquid = wine.Object, Amount = 0.5 }], world.Object);
		var reloaded = new LiquidMixture(original.SaveToXml(), world.Object);
		Assert.AreEqual(1.5, reloaded.TotalVolume);
		Assert.AreEqual(1495.0, reloaded.TotalWeight, 0.000001);
		reloaded.AddLiquidVolume(1.5);
		Assert.AreEqual(2990.0, reloaded.TotalWeight, 0.000001);
	}
}
