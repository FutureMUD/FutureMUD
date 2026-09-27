using System;
using System.Globalization;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Form.Material;

/// <summary>Fraction transmitted, independent of damage susceptibility and clothing warmth.</summary>
public sealed class MaterialExposureProperties
{
	public double LiquidTransmission { get; set; } = 1.0;
	public double GasTransmission { get; set; } = 1.0;
	public double ThermalTransmission { get; set; } = 1.0;
	public double SoakPerSecond { get; set; } = 0.02;
	public double? ThermalSlope { get; set; }
	public double? ThermalCap { get; set; }
	public long? ThermalIntensityProgId { get; set; }

	public double Transmission(ExposureRoute route) => route switch
	{
		ExposureRoute.LiquidContact => LiquidTransmission,
		ExposureRoute.GasContact => GasTransmission,
		ExposureRoute.AmbientHeat => ThermalTransmission,
		_ => 1.0
	};

	public static MaterialExposureProperties Load(string? xml)
	{
		var result = new MaterialExposureProperties();
		if (string.IsNullOrWhiteSpace(xml)) return result;
		try
		{
			var root = XElement.Parse(xml);
			double? Read(string key, double maximum) => double.TryParse(root.Attribute(key)?.Value,
				NumberStyles.Float, CultureInfo.InvariantCulture, out var value) && ExposureArithmetic.Valid(value) && value <= maximum ? value : null;
			result.LiquidTransmission = Read("Liquid", 1.0) ?? 1.0;
			result.GasTransmission = Read("Gas", 1.0) ?? 1.0;
			result.ThermalTransmission = Read("Thermal", 1.0) ?? 1.0;
			result.SoakPerSecond = Read("Soak", 1.0) ?? 0.02;
			result.ThermalSlope = Read("Slope", 1000000.0);
			result.ThermalCap = Read("Cap", 1000000.0);
			if (root.Attribute("IntensityProg") is { } prog)
				result.ThermalIntensityProgId = long.TryParse(prog.Value, out var id) && id > 0 ? id : -1;
		}
		catch (System.Xml.XmlException) { /* Historical malformed settings use permeable defaults. */ }
		return result;
	}

	public string Save() => new XElement("Exposure", new XAttribute("Liquid", LiquidTransmission),
		new XAttribute("Gas", GasTransmission), new XAttribute("Thermal", ThermalTransmission),
		new XAttribute("Soak", SoakPerSecond), ThermalSlope is { } slope ? new XAttribute("Slope", slope) : null,
		ThermalCap is { } cap ? new XAttribute("Cap", cap) : null,
		ThermalIntensityProgId is { } prog ? new XAttribute("IntensityProg", prog) : null).ToString();
}
