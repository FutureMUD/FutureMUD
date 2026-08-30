using System;

namespace MudSharp.GameItems;

public static class PowerBankEnergyMath
{
	public static double ResolveWattHours(double currentWattHours, double capacityWattHours,
		double inputWatts, double chargingEfficiency, double outputWatts, TimeSpan elapsed)
	{
		var inputWattHours = Math.Max(0.0, inputWatts) * Math.Clamp(chargingEfficiency, 0.0, 1.0) *
			elapsed.TotalHours;
		var outputWattHours = Math.Max(0.0, outputWatts) * elapsed.TotalHours;
		return Math.Clamp(currentWattHours + inputWattHours - outputWattHours, 0.0,
			Math.Max(0.0, capacityWattHours));
	}
}
