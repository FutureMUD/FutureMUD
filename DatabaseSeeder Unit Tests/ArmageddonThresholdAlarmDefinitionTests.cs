#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonThresholdAlarmDefinitionTests
{
	[TestMethod]
	public void ThresholdAlarm_StockDefinitionKeepsEarthRoomCostAndFiniteLifetime()
	{
		var entry = ArmageddonThresholdAlarmSpellContent.Create();
		var definition = entry.Content.BuildDefinition(1, 2, 0);
		Assert.AreEqual("arm.capability.earth", entry.NativeCapabilityKey);
		Assert.AreEqual("120*grade", entry.Content.DurationFormula);
		Assert.AreEqual(33.0, entry.Content.MinimumEnergy);
		Assert.AreEqual("room", (string?)definition.Element("Trigger")?.Attribute("type"));
		Assert.AreEqual("thresholdalarm", (string?)definition.Element("Effects")?.Element("Effect")?.Attribute("type"));
	}
}
