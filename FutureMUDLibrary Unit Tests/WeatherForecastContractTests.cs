#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Climate;

namespace FutureMUDLibrary_Unit_Tests;

[TestClass]
public class WeatherForecastContractTests
{
	[TestMethod]
	public void WeatherRandom_SavedState_ReplaysRemainingSequence()
	{
		var random = new WeatherRandom(12345);
		for (var i = 0; i < 77; i++) random.NextDouble();
		var restored = new WeatherRandom(random.State);
		CollectionAssert.AreEqual(Enumerable.Range(0, 100).Select(_ => random.NextDouble()).ToArray(),
			Enumerable.Range(0, 100).Select(_ => restored.NextDouble()).ToArray());
	}

	[TestMethod]
	public void HazardXml_MissingDefinition_IsHarmlessAndConfiguredDefinitionRoundtrips()
	{
		var legacy = WeatherHazardSettings.FromXml(null);
		Assert.AreEqual(0, legacy.LightningChance);
		Assert.AreEqual(0L, legacy.AtmosphereGasId);
		var configured = legacy with { LightningChance = 0.02, AtmosphericLightningChance = 0.4, Damage = 125,
			AtmosphereGasId = 19, GroundDamageFactor = 0, ForecastDescription = "a choking dust storm", ThunderDistance = 4 };
		Assert.AreEqual(configured, WeatherHazardSettings.FromXml(configured.ToXml()));
	}

	[DataTestMethod]
	[DataRow("Lightning", "1.01")]
	[DataRow("AtmosphericLightning", "-1")]
	[DataRow("GroundDamageFactor", "2")]
	[DataRow("ThunderDistance", "101")]
	[DataRow("Damage", "NaN")]
	[DataRow("Atmosphere", "-1")]
	public void HazardXml_InvalidValues_AreRejected(string field, string value)
	{
		Assert.ThrowsException<FormatException>(() => WeatherHazardSettings.FromXml(new XElement("Hazards", new XElement(field, value))));
	}
}
