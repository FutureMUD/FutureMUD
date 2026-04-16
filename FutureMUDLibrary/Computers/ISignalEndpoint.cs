#nullable enable

using System;

namespace MudSharp.Computers;

public readonly record struct ComputerSignal(double Value, TimeSpan? Duration, TimeSpan? PulseInterval);

public interface ISignalSource
{
	string Name { get; }
	string EndpointKey { get; }
	double CurrentValue { get; }
	TimeSpan? Duration { get; }
	TimeSpan? PulseInterval { get; }
}

public interface ISignalSink
{
	string Name { get; }
	ISignalSource? UpstreamSource { get; }
	double CurrentValue { get; }
	void ReceiveSignal(ComputerSignal signal, ISignalSource source);
}
