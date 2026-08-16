using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MudWebSocketProxy.Security;

public static class ProxyProtocolV1Header
{
	public static byte[] Build(IPAddress sourceAddress, int sourcePort, int destinationPort)
	{
		sourceAddress = sourceAddress.IsIPv4MappedToIPv6 ? sourceAddress.MapToIPv4() : sourceAddress;
		var (protocol, destinationAddress) = sourceAddress.AddressFamily switch
		{
			AddressFamily.InterNetwork => ("TCP4", IPAddress.Any),
			AddressFamily.InterNetworkV6 => ("TCP6", IPAddress.IPv6Any),
			_ => throw new ArgumentException("Only IPv4 and IPv6 client addresses are supported.", nameof(sourceAddress))
		};
		if (sourcePort is < 0 or > 65_535)
		{
			throw new ArgumentOutOfRangeException(nameof(sourcePort));
		}

		if (destinationPort is < 1 or > 65_535)
		{
			throw new ArgumentOutOfRangeException(nameof(destinationPort));
		}

		return Encoding.ASCII.GetBytes(string.Create(
			CultureInfo.InvariantCulture,
			$"PROXY {protocol} {sourceAddress} {destinationAddress} {sourcePort} {destinationPort}\r\n"));
	}
}
