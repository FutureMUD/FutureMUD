#nullable enable


namespace MudSharp.Computers;

internal sealed class ComputerSignalWaitSubscription
{
	public required long ProcessId { get; init; }
	public required ISignalSourceComponent Source { get; init; }
	public required SignalChangedEvent Handler { get; init; }

	public void Detach()
	{
		Source.SignalChanged -= Handler;
	}
}
