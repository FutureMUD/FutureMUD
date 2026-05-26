#nullable enable

using MudSharp.Character;
using MudSharp.Framework.Units;

namespace MudSharp.GameItems.Interfaces;

public enum MeasuringInstrumentMode
{
	Weight = 0,
	FluidVolume = 1
}

public sealed record MeasurementResult(
	MeasuringInstrumentMode Mode,
	UnitType UnitType,
	double TrueValue,
	double ReportedValue,
	double Drift,
	double CalibrationBias,
	int UsesSinceCalibration);

public interface IMeasuringInstrument : IGameItemComponent
{
	MeasuringInstrumentMode Mode { get; }
	UnitType UnitType { get; }
	double Precision { get; }
	double Capacity { get; }
	double CalibrationBias { get; }
	bool CalibrationBiasIsPercentage { get; }
	bool HasDeliberateBias { get; }
	int UsesSinceCalibration { get; }
	bool CanMeasure(IGameItem target, out string error);
	MeasurementResult Measure(ICharacter actor, IGameItem target);
	void Calibrate(ICharacter actor);
	bool CalibrateWrong(ICharacter actor, double bias, bool percentageBias, out string error);
	string InspectCalibration(ICharacter actor);
}
