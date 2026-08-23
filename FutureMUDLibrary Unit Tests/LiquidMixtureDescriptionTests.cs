using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LiquidMixtureDescriptionTests
{
	[TestMethod]
	public void RenderedDescriptions_HomogeneousInstancesCollapseWithoutMixtureWording()
	{
		ILiquid blood = CreateLiquid("blood", "blood", "fresh blood", Telnet.Red).Object;
		var mixture = new LiquidMixture(
		[
			new LiquidInstance { Liquid = blood, Amount = 4.0 },
			new LiquidInstance { Liquid = blood, Amount = 6.0 }
		], null!);

		Assert.AreEqual("blood", mixture.LiquidDescription);
		Assert.AreEqual("blood".Colour(Telnet.Red), mixture.ColouredLiquidDescription);
		Assert.AreEqual("fresh blood".Colour(Telnet.Red), mixture.ColouredLiquidLongDescription);
	}

	[TestMethod]
	public void RenderedDescriptions_DistinctLiquidsRetainMixtureWording()
	{
		ILiquid blood = CreateLiquid("blood", "blood", "fresh blood", Telnet.Red).Object;
		ILiquid water = CreateLiquid("water", "water", "clear water", Telnet.Blue).Object;
		var mixture = new LiquidMixture(
		[
			new LiquidInstance { Liquid = blood, Amount = 4.0 },
			new LiquidInstance { Liquid = water, Amount = 6.0 }
		], null!);

		Assert.AreEqual("a mixture of blood and water", mixture.LiquidDescription);
		StringAssert.StartsWith(mixture.ColouredLiquidDescription, "a mixture of ");
		StringAssert.StartsWith(mixture.ColouredLiquidLongDescription, "a mixture of ");
	}

	private static Mock<ILiquid> CreateLiquid(string name, string materialDescription, string description,
		ANSIColour colour)
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Id).Returns(name.GetHashCode());
		liquid.SetupGet(x => x.Name).Returns(name);
		liquid.SetupGet(x => x.MaterialDescription).Returns(materialDescription);
		liquid.SetupGet(x => x.Description).Returns(description);
		liquid.SetupGet(x => x.DisplayColour).Returns(colour);
		liquid.SetupGet(x => x.Density).Returns(1.0);
		return liquid;
	}
}
