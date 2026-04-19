#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Computers;

public sealed class ComputerNetworkTunnelService : IComputerNetworkTunnelService
{
	private readonly IFuturemud _gameworld;

	public ComputerNetworkTunnelService(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public IEnumerable<ComputerVpnGatewaySummary> GetReachableVpnGateways(IComputerTerminalSession session)
	{
		return _gameworld.ComputerExecutionService.GetReachableHosts(session.Host, session)
			.Where(x => x.Host.HostedVpnNetworkIds.Any())
			.OrderBy(x => x.IsLocalGrid ? 0 : 1)
			.ThenBy(x => x.Host.Name)
			.ThenBy(x => x.CanonicalAddress)
			.Select(x => new ComputerVpnGatewaySummary
			{
				Host = x.Host,
				CanonicalAddress = x.CanonicalAddress,
				DeviceIdentifier = x.DeviceIdentifier,
				VpnNetworkIds = x.Host.HostedVpnNetworkIds
					.OrderBy(id => id)
					.ToList(),
				SharedRouteKeys = x.SharedRouteKeys
			})
			.ToList();
	}

	public bool TryOpenTunnel(IComputerTerminalSession session, string gatewayIdentifier, string address, string password,
		string? requestedVpnNetworkId, out string error)
	{
		error = string.Empty;
		if (session is not ComputerTerminalSession runtimeSession)
		{
			error = "That terminal session does not support VPN tunnelling.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(gatewayIdentifier))
		{
			error = "You must specify a reachable VPN gateway host.";
			return false;
		}

		var gateway = _gameworld.ComputerExecutionService.ResolveReachableHost(session.Host, gatewayIdentifier, session);
		if (gateway is null || !gateway.Host.HostedVpnNetworkIds.Any())
		{
			error = $"There is no reachable VPN gateway matching {gatewayIdentifier.ColourCommand()}.";
			return false;
		}

		var authentication = _gameworld.ComputerNetworkIdentityService.Authenticate(session.Host, address, password);
		if (!authentication.Success || authentication.Account is null)
		{
			error = authentication.ErrorMessage;
			return false;
		}

		var gatewayDomains = _gameworld.ComputerNetworkIdentityService.GetHostedDomains(gateway.Host)
			.Where(x => x.Enabled)
			.Select(x => x.DomainName)
			.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
		if (!gatewayDomains.Contains(authentication.Account.DomainName))
		{
			error =
				$"{gateway.Host.Name.ColourName()} does not accept tunnel authentication for {authentication.Account.Address.ColourName()}.";
			return false;
		}

		var availableNetworks = gateway.Host.HostedVpnNetworkIds
			.OrderBy(x => x)
			.ToList();
		var selectedNetworkId = SelectVpnNetworkId(availableNetworks, requestedVpnNetworkId, out error);
		if (string.IsNullOrEmpty(selectedNetworkId))
		{
			return false;
		}

		var routeKey = ComputerNetworkRoutingUtilities.GetVpnRouteKey(selectedNetworkId);
		runtimeSession.AddOrReplaceTunnel(new ComputerNetworkTunnelInfo
		{
			RouteKey = routeKey,
			RouteDescription = ComputerNetworkRoutingUtilities.DescribeRouteKey(routeKey),
			GatewayHostItemId = gateway.Host.OwnerHostItemId ?? 0L,
			GatewayHostName = gateway.Host.Name,
			AuthenticatedAddress = authentication.Account.Address,
			ConnectedAtUtc = DateTime.UtcNow
		});
		return true;
	}

	public bool TryCloseTunnel(IComputerTerminalSession session, string? routeIdentifier, out string error)
	{
		error = string.Empty;
		if (session is not ComputerTerminalSession runtimeSession)
		{
			error = "That terminal session does not support VPN tunnelling.";
			return false;
		}

		if (!runtimeSession.ActiveTunnels.Any())
		{
			error = "There are no active tunnels on that terminal session.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(routeIdentifier) || routeIdentifier.EqualTo("all"))
		{
			runtimeSession.ClearTunnels();
			return true;
		}

		var routeKey = routeIdentifier.StartsWith("vpn:", StringComparison.InvariantCultureIgnoreCase)
			? ComputerNetworkRoutingUtilities.GetVpnRouteKey(
				ComputerNetworkRoutingUtilities.GetVpnNetworkIdFromRouteKey(routeIdentifier))
			: ComputerNetworkRoutingUtilities.GetVpnRouteKey(routeIdentifier);

		if (!runtimeSession.RemoveTunnel(routeKey))
		{
			error = $"There is no active tunnel matching {routeIdentifier.ColourCommand()}.";
			return false;
		}

		return true;
	}

	public IEnumerable<string> GetAdvertisedServiceDetails(IComputerHost host)
	{
		return host.HostedVpnNetworkIds
			.OrderBy(x => x)
			.Select(x => ComputerNetworkRoutingUtilities.DescribeRouteKey(
				ComputerNetworkRoutingUtilities.GetVpnRouteKey(x)))
			.ToList();
	}

	private static string SelectVpnNetworkId(IEnumerable<string> availableNetworks, string? requestedVpnNetworkId,
		out string error)
	{
		error = string.Empty;
		var networks = availableNetworks
			.Select(ComputerNetworkRoutingUtilities.NormaliseIdentifier)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.OrderBy(x => x)
			.ToList();
		if (!networks.Any())
		{
			error = "That gateway is not advertising any VPN networks.";
			return string.Empty;
		}

		if (string.IsNullOrWhiteSpace(requestedVpnNetworkId))
		{
			if (networks.Count == 1)
			{
				return networks.Single();
			}

			error =
				$"That gateway exposes more than one VPN network. Choose one of {networks.Select(x => x.ColourCommand()).ListToString()}.";
			return string.Empty;
		}

		var requested = ComputerNetworkRoutingUtilities.GetVpnNetworkIdFromRouteKey(requestedVpnNetworkId);
		if (!networks.Contains(requested, StringComparer.InvariantCultureIgnoreCase))
		{
			error =
				$"{requestedVpnNetworkId.ColourCommand()} is not one of the VPN networks exposed by that gateway. Choose one of {networks.Select(x => x.ColourCommand()).ListToString()}.";
			return string.Empty;
		}

		return requested;
	}
}
