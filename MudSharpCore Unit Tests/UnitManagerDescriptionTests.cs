#nullable enable

using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework.Units;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UnitManagerDescriptionTests
{
	[TestMethod]
	public void RecalculateLastUnits_MultipleUnitTypes_RetainsFallbackForEachType()
	{
		var mass = CreateUnit("gram", UnitType.Mass);
		var fluid = CreateUnit("millilitre", UnitType.FluidVolume);
		var manager = new UnitManager([mass.Object, fluid.Object]);

		Assert.IsTrue(mass.Object.LastDescriber);
		Assert.IsTrue(fluid.Object.LastDescriber);
		Assert.AreEqual("0 grams", manager.Describe(0.0, UnitType.Mass, "Metric", CultureInfo.InvariantCulture));
		Assert.AreEqual("0 millilitres",
			manager.Describe(0.0, UnitType.FluidVolume, "Metric", CultureInfo.InvariantCulture));
	}

	[TestMethod]
	public void Describe_NonZeroValue_RemainsUnchanged()
	{
		var fluid = CreateUnit("millilitre", UnitType.FluidVolume);
		var manager = new UnitManager([fluid.Object]);

		Assert.AreEqual("2 millilitres",
			manager.Describe(2.0, UnitType.FluidVolume, "Metric", CultureInfo.InvariantCulture));
	}

	private static Mock<IUnit> CreateUnit(string name, UnitType type)
	{
		var unit = new Mock<IUnit>();
		unit.SetupGet(x => x.Name).Returns(name);
		unit.SetupGet(x => x.Type).Returns(type);
		unit.SetupGet(x => x.System).Returns("Metric");
		unit.SetupGet(x => x.MultiplierFromBase).Returns(1.0);
		unit.SetupGet(x => x.PreMultiplierOffsetFrombase).Returns(0.0);
		unit.SetupGet(x => x.PostMultiplierOffsetFrombase).Returns(0.0);
		unit.SetupGet(x => x.DescriberUnit).Returns(true);
		unit.SetupGet(x => x.SpaceBetween).Returns(true);
		unit.SetupProperty(x => x.LastDescriber);
		return unit;
	}
}
