using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BloodLiquidInstanceTests
{
	[TestMethod]
	public void SaveToXml_AllowsUnknownRaceAndBloodType()
	{
		var liquid = new Mock<ILiquid>();
		liquid.SetupGet(x => x.Id).Returns(42);
		var gameworld = new Mock<IFuturemud>();
		var instance = new BloodLiquidInstance(null, null, null, liquid.Object, gameworld.Object, 10.0);

		var xml = instance.SaveToXml();

		Assert.AreEqual("0", xml.Attribute("source")?.Value);
		Assert.AreEqual("0", xml.Attribute("race")?.Value);
		Assert.AreEqual("0", xml.Attribute("bloodtype")?.Value);
	}
}
