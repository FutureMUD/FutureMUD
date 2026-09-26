using System.Globalization;

#nullable enable

namespace MudSharp.Form.Material;

public sealed record EnvironmentalExposureOptions(EnvironmentalExposureMode Mode, bool Liquids, bool Gases, bool Inhalation,
	bool Heat, bool Characters, bool Items, double Scale, double Interval, double Substep,
	double MaximumInterval, double MinimumVolume, double HeatSlope, double HeatCap, double SplashReferenceVolume, double CloudStrengthCap)
{
	/// <summary>Recheck callback-sensitive gates without reparsing unrelated numerical settings.</summary>
	public static bool AllowsCurrent(IFuturemud world, IPerceivable target, ExposureRoute route)
	{
		if (!Enum.TryParse<EnvironmentalExposureMode>(world.GetStaticConfiguration("EnvironmentalExposureMode"), true, out var mode) ||
			mode != EnvironmentalExposureMode.Enabled) return false;
		bool Flag(string name) => !bool.TryParse(world.GetStaticConfiguration("EnvironmentalExposure" + name), out var value) || value;
		return Flag(target is MudSharp.GameItems.IGameItem ? "Items" : "Characters") && route switch
		{
			ExposureRoute.LiquidContact or ExposureRoute.Ingestion or ExposureRoute.Injection => Flag("Liquids"),
			ExposureRoute.GasContact => Flag("Gases"),
			ExposureRoute.Inhalation => Flag("Inhalation"),
			ExposureRoute.AmbientHeat => Flag("Heat"),
			_ => false
		};
	}

	public static EnvironmentalExposureOptions Read(IFuturemud world)
	{
		string? Text(string name) => world.GetStaticConfiguration("EnvironmentalExposure" + name);
		bool Flag(string name) => !bool.TryParse(Text(name), out var value) || value;
		double Number(string name, double fallback, double min, double max) => double.TryParse(Text(name), NumberStyles.Float,
			CultureInfo.InvariantCulture, out var value) && double.IsFinite(value) ? Math.Clamp(value, min, max) : fallback;
		return new(Enum.TryParse<EnvironmentalExposureMode>(Text("Mode"), true, out var mode) && Enum.IsDefined(mode) ? mode : EnvironmentalExposureMode.Legacy,
			Flag("Liquids"), Flag("Gases"), Flag("Inhalation"), Flag("Heat"), Flag("Characters"), Flag("Items"), Number("Scale", 1, 0, 100),
			Number("Interval", 1, 0.01, 60), Number("Substep", 0.25, 0.001, 1), Number("MaximumInterval", 60, 1, 600),
			Number("MinimumVolume", 1e-9, 1e-12, 0.001), Number("HeatSlope", 0.05, 0, 1000), Number("HeatCap", 20, 0, 1000000),
			Number("SplashReferenceLitres", 0.1, 0.000001, 1000) / (world.UnitManager?.BaseFluidToLitres ?? 1.0), Number("CloudStrengthCap", 1, 0, 100));
	}

	public bool Allows(IPerceivable target, ExposureRoute route) => Mode == EnvironmentalExposureMode.Enabled &&
		(target is MudSharp.GameItems.IGameItem ? Items : Characters) && route switch
		{
			ExposureRoute.LiquidContact or ExposureRoute.Ingestion or ExposureRoute.Injection => Liquids,
			ExposureRoute.GasContact => Gases,
			ExposureRoute.Inhalation => Inhalation,
			ExposureRoute.AmbientHeat => Heat,
			_ => false
		};
}
