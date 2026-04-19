#nullable enable

namespace MudSharp.GameItems.Interfaces;

public interface IRuntimeConfigurableSignalSinkComponent : ISignalSinkComponent
{
	LocalSignalBinding CurrentBinding { get; }
	double ActivationThreshold { get; }
	bool ActiveWhenAboveThreshold { get; }
	bool ConfigureSignalBinding(ISignalSourceComponent source, string? endpointKey, out string error);
	void ClearSignalBinding();
	bool SetActivationThreshold(double threshold, out string error);
	void SetActiveWhenAboveThreshold(bool activeWhenAboveThreshold);
}
