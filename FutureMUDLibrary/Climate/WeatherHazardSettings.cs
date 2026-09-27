#nullable enable

using System;
using System.Xml.Linq;

namespace MudSharp.Climate;

/// <summary>Optional, combinable hazards. Defaults leave legacy weather harmless.</summary>
public sealed record WeatherHazardSettings
{
	public string ForecastDescription { get; init; } = "";
	public long AtmosphereGasId { get; init; }
	public double LightningChance { get; init; }
	public double AtmosphericLightningChance { get; init; }
	public double Damage { get; init; } = 100;
	public double Pain { get; init; } = 100;
	public double Stun { get; init; } = 100;
	public double GroundWeight { get; init; } = 90;
	public double CharacterWeight { get; init; } = 5;
	public double ItemWeight { get; init; } = 5;
	public double GroundDamageFactor { get; init; } = 0.2;
	public string Flash { get; init; } = "A brilliant flash of lightning splits the sky!";
	public string Thunder { get; init; } = "A tremendous crack of thunder shakes the air.";
	public double ThunderDistance { get; init; } = 10;

	public XElement ToXml() => new("Hazards",
		new XElement("Forecast", ForecastDescription), new XElement("Atmosphere", AtmosphereGasId),
		new XElement("Lightning", LightningChance), new XElement("AtmosphericLightning", AtmosphericLightningChance),
		new XElement("Damage", Damage), new XElement("Pain", Pain), new XElement("Stun", Stun),
		new XElement("GroundWeight", GroundWeight), new XElement("CharacterWeight", CharacterWeight), new XElement("ItemWeight", ItemWeight),
		new XElement("GroundDamageFactor", GroundDamageFactor), new XElement("Flash", Flash), new XElement("Thunder", Thunder),
		new XElement("ThunderDistance", ThunderDistance));

	public static WeatherHazardSettings FromXml(XElement? element)
	{
		if (element is null) return new();
		var atmosphere = (long?)element.Element("Atmosphere") ?? 0;
		if (atmosphere < 0) throw new FormatException("Invalid weather atmosphere gas ID.");
		double Number(string name, double fallback, double max = double.MaxValue)
		{
			var value = (double?)element.Element(name) ?? fallback;
			if (!double.IsFinite(value) || value < 0 || value > max) throw new FormatException($"Invalid weather hazard {name}.");
			return value;
		}
		return new()
		{
			ForecastDescription = (string?)element.Element("Forecast") ?? "", AtmosphereGasId = atmosphere,
			LightningChance = Number("Lightning", 0, 1), AtmosphericLightningChance = Number("AtmosphericLightning", 0, 1),
			Damage = Number("Damage", 100), Pain = Number("Pain", 100), Stun = Number("Stun", 100),
			GroundWeight = Number("GroundWeight", 90), CharacterWeight = Number("CharacterWeight", 5), ItemWeight = Number("ItemWeight", 5),
			GroundDamageFactor = Number("GroundDamageFactor", 0.2, 1), ThunderDistance = Number("ThunderDistance", 10, 100),
			Flash = (string?)element.Element("Flash") ?? "A brilliant flash of lightning splits the sky!",
			Thunder = (string?)element.Element("Thunder") ?? "A tremendous crack of thunder shakes the air."
		};
	}
}
