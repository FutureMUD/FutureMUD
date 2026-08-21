#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Health.Breathing;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BreathingStrategyTests
{
	[TestMethod]
	public void PartlessBreather_CanBreathe_WhenAtmosphereCountsAsRaceBreathableGas()
	{
		var canonicalAtmosphere = new Mock<IGas>();
		var equivalentAtmosphere = new Mock<IGas>();
		var race = new Mock<IRace>();
		race.SetupGet(x => x.BreathableFluids).Returns(new IFluid[] { canonicalAtmosphere.Object });
		race.Setup(x => x.CanBreatheFluid(equivalentAtmosphere.Object)).Returns((true, 1.0));

		var cell = new Mock<ICell>();
		cell.Setup(x => x.IsUnderwaterLayer(RoomLayer.GroundLevel)).Returns(false);
		cell.SetupGet(x => x.Atmosphere).Returns(equivalentAtmosphere.Object);

		var body = new Mock<IBody>();
		body.SetupGet(x => x.Race).Returns(race.Object);
		body.SetupGet(x => x.Location).Returns(cell.Object);
		body.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

		Assert.IsTrue(new PartlessBreather().CanBreathe(body.Object));
		race.Verify(x => x.CanBreatheFluid(equivalentAtmosphere.Object), Times.Once);
	}
}
