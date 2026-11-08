#nullable enable

using System;
using System.Globalization;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private long InstallWearableScalarSizeProg()
	{
		var metres = _db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Metre]).BaseMultiplier;
		var kilograms = _db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		if (!double.IsFinite(metres) || metres <= 0 || !double.IsFinite(kilograms) || kilograms <= 0)
			throw new InvalidOperationException("Armageddon wearable sizing requires positive metre and kilogram unit bindings.");

		var heightInches = $"(@character.Height / {metres.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} / 0.0254)";
		var weightTenKilogramUnits = $"(@character.Weight / {kilograms.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} / 10)";
		var prog = Prog("ArmageddonWorldfileScalarWearableSize",
			$"return floor(({heightInches} + {weightTenKilogramUnits}) / 7.4)",
			ProgVariableTypes.Number, ("character", ProgVariableTypes.Character));
		Record("wearable.scalar-size", nameof(FutureProg), prog.Id, null, new
		{
			MetreMultiplier = metres, KilogramMultiplier = kilograms,
			Disposition = "Armageddon-compatible integer scalar: floor((height inches + weight in ten-kilogram units) / 7.4)."
		});
		var setting = _db.StaticConfigurations.Find("DefaultWearableScalarSizeProg");
		if (setting is null) _db.StaticConfigurations.Add(new StaticConfiguration { SettingName = "DefaultWearableScalarSizeProg", Definition = prog.Id.ToString(System.Globalization.CultureInfo.InvariantCulture) });
		else setting.Definition = prog.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
		return prog.Id;
	}

	internal static int SourceWearableSizeFromNativeMeasurements(double height, double weight, double metreMultiplier, double kilogramMultiplier)
	{
		if (!double.IsFinite(height) || height < 0 || !double.IsFinite(weight) || weight < 0 ||
		    !double.IsFinite(metreMultiplier) || metreMultiplier <= 0 ||
		    !double.IsFinite(kilogramMultiplier) || kilogramMultiplier <= 0)
			throw new ArgumentOutOfRangeException(nameof(height), "Measurements and unit multipliers must be finite and nonnegative, with positive unit multipliers.");
		var inches = height / metreMultiplier / 0.0254;
		var tenKilogramUnits = weight / kilogramMultiplier / 10.0;
		return checked((int)Math.Floor((inches + tenKilogramUnits) / 7.4));
	}
}
