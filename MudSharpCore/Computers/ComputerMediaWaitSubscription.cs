#nullable enable

namespace MudSharp.Computers;

/// <summary>
/// A persisted-program wait is represented by its stable input endpoint address. Unlike a numeric signal wait,
/// media delivery is multiplexed by the channel service, so no per-component event handler is required.
/// </summary>
internal sealed class ComputerMediaWaitSubscription
{
	public required long ProcessId { get; init; }
	public required MediaEndpointAddress Endpoint { get; init; }
}
