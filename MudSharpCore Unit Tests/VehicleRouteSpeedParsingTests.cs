#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleRouteSpeedParsingTests
{
	[TestMethod]
	[DataRow("20m/s", "20m", 20.0, 20.0)]
	[DataRow("20m/sec", "20m", 20.0, 20.0)]
	[DataRow("120m/minute", "120m", 120.0, 2.0)]
	[DataRow("3.6km/hour", "3.6km", 3600.0, 1.0)]
	[DataRow("10m/2s", "10m", 10.0, 5.0)]
	public void TryParseRouteSpeed_NormalDurationForms_ReturnMetresPerSecond(string text,
		string distanceText, double baseDistance, double expectedMetresPerSecond)
	{
		var actor = new Mock<ICharacter>();
		var unitManager = new Mock<IUnitManager>();
		var parsedDistance = baseDistance;
		unitManager
			.Setup(x => x.TryGetBaseUnits(distanceText, UnitType.Length, actor.Object, out parsedDistance))
			.Returns(true);
		unitManager.SetupGet(x => x.BaseHeightToMetres).Returns(1.0);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		var parsed = VehiclePrototype.TryParseRouteSpeed(actor.Object, text, out var metresPerSecond);

		Assert.IsTrue(parsed);
		Assert.AreEqual(expectedMetresPerSecond, metresPerSecond, 0.000001);
	}

	[TestMethod]
	[DataRow("20m/fortnight")]
	[DataRow("20m/0s")]
	[DataRow("20m/")]
	public void TryParseRouteSpeed_InvalidDuration_ReturnsFalse(string text)
	{
		var actor = new Mock<ICharacter>();
		var unitManager = new Mock<IUnitManager>();
		var parsedDistance = 20.0;
		unitManager
			.Setup(x => x.TryGetBaseUnits("20m", UnitType.Length, actor.Object, out parsedDistance))
			.Returns(true);
		unitManager.SetupGet(x => x.BaseHeightToMetres).Returns(1.0);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		Assert.IsFalse(VehiclePrototype.TryParseRouteSpeed(actor.Object, text, out _));
	}
}
