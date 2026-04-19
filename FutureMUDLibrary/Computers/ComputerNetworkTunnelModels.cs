#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Computers;

public sealed class ComputerNetworkTunnelInfo
{
	public string RouteKey { get; init; } = string.Empty;
	public string RouteDescription { get; init; } = string.Empty;
	public long GatewayHostItemId { get; init; }
	public string GatewayHostName { get; init; } = string.Empty;
	public string AuthenticatedAddress { get; init; } = string.Empty;
	public DateTime ConnectedAtUtc { get; init; }
}

public sealed class ComputerVpnGatewaySummary
{
	public required IComputerHost Host { get; init; }
	public string CanonicalAddress { get; init; } = string.Empty;
	public string DeviceIdentifier { get; init; } = string.Empty;
	public IReadOnlyCollection<string> VpnNetworkIds { get; init; } = Array.Empty<string>();
	public IReadOnlyCollection<string> SharedRouteKeys { get; init; } = Array.Empty<string>();
}

public interface IComputerNetworkTunnelService
{
	IEnumerable<ComputerVpnGatewaySummary> GetReachableVpnGateways(IComputerTerminalSession session);
	bool TryOpenTunnel(IComputerTerminalSession session, string gatewayIdentifier, string address, string password,
		string? requestedVpnNetworkId, out string error);
	bool TryCloseTunnel(IComputerTerminalSession session, string? routeIdentifier, out string error);
	IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host);
}
