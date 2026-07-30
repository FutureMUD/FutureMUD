#nullable enable

using MudSharp.Form.Audio;

namespace MudSharp.GameItems.Interfaces;

/// <summary>
/// A terrestrial vehicle engine. Vehicle movement consumes this interface and is deliberately
/// unaware of the engine's underlying energy source or implementation.
/// </summary>
public interface IVehicleEngine : IGameItemComponent
{
	string FormFactor { get; }
	double MaximumPowerInWatts { get; }
	AudioVolume NoiseLevel { get; }
	bool IsRunning { get; }
	string WhyNotRunning { get; }
	void EmitOperatingNoise();
}
