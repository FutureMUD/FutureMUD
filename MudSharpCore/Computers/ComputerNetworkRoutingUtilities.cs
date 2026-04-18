#nullable enable

using MudSharp.Computers;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Computers;

public static class ComputerNetworkRoutingUtilities
{
	public static string GetDeviceIdentifier(long adapterItemId)
	{
		return $"device-{adapterItemId}";
	}

	public static IReadOnlyCollection<string> GetRouteKeys(INetworkAdapter adapter)
	{
		HashSet<string> routes = new(StringComparer.InvariantCultureIgnoreCase);
		if (adapter.PublicNetworkEnabled)
		{
			routes.Add("public");
		}

		if (adapter.TelecommunicationsGrid is not null &&
		    !string.IsNullOrWhiteSpace(adapter.ExchangeSubnetId))
		{
			routes.Add(GetExchangeSubnetRouteKey(adapter.TelecommunicationsGrid.Id, adapter.ExchangeSubnetId));
		}

		foreach (var vpn in adapter.VpnNetworkIds
			         .Select(NormaliseIdentifier)
			         .Where(x => !string.IsNullOrWhiteSpace(x)))
		{
			routes.Add($"vpn:{vpn}");
		}

		return routes.ToList();
	}

	public static IReadOnlyCollection<string> GetSharedRouteKeys(INetworkAdapter source, INetworkAdapter target)
	{
		HashSet<string> sourceRoutes = new(source.NetworkRouteKeys, StringComparer.InvariantCultureIgnoreCase);
		return target.NetworkRouteKeys
			.Where(sourceRoutes.Contains)
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.OrderBy(x => x)
			.ToList();
	}

	public static bool CanRouteBetween(INetworkAdapter source, INetworkAdapter target)
	{
		if (!source.NetworkReady || !target.NetworkReady)
		{
			return false;
		}

		return GetSharedRouteKeys(source, target).Any();
	}

	public static string DescribeRouteKey(string routeKey)
	{
		if (routeKey.EqualTo("public"))
		{
			return "Public";
		}

		if (routeKey.StartsWith("exchange:", StringComparison.InvariantCultureIgnoreCase))
		{
			var split = routeKey.Split(':', 3);
			if (split.Length == 3)
			{
				return $"Exchange {split[2]}";
			}

			return "Exchange";
		}

		if (routeKey.StartsWith("vpn:", StringComparison.InvariantCultureIgnoreCase))
		{
			var split = routeKey.Split(':', 2);
			return split.Length == 2 ? $"VPN {split[1]}" : "VPN";
		}

		return routeKey;
	}

	public static string DescribeRoutes(IEnumerable<string> routeKeys)
	{
		var routes = routeKeys
			.Select(DescribeRouteKey)
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.OrderBy(x => x)
			.ToList();
		return routes.Any() ? routes.ListToString() : "None";
	}

	public static string GetExchangeSubnetRouteKey(long gridId, string? subnetId)
	{
		return $"exchange:{gridId}:{NormaliseIdentifier(subnetId)}";
	}

	public static string NormaliseIdentifier(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return string.Empty;
		}

		return text.Trim().ToLowerInvariant();
	}
}
