#nullable enable

using System.Net;
using System.Net.Sockets;

namespace MudSharp.Network;

internal static class ProxyProtocolV1
{
	internal const int MaximumHeaderBytes = 108;
	internal static ReadOnlySpan<byte> Prefix => "PROXY "u8;

	internal static bool TryParseHeader(ReadOnlySpan<byte> header, out IPAddress sourceAddress)
	{
		sourceAddress = IPAddress.None;
		if (header.Length < Prefix.Length + 2 || header.Length > MaximumHeaderBytes ||
		    !header.EndsWith("\r\n"u8))
		{
			return false;
		}

		var line = Encoding.ASCII.GetString(header[..^2]);
		var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length != 6 || !string.Equals(parts[0], "PROXY", StringComparison.Ordinal) ||
		    !IPAddress.TryParse(parts[2], out var parsedSourceAddress) ||
		    !IPAddress.TryParse(parts[3], out var destinationAddress) ||
		    !ushort.TryParse(parts[4], out _) ||
		    !ushort.TryParse(parts[5], out _))
		{
			sourceAddress = IPAddress.None;
			return false;
		}

		sourceAddress = parsedSourceAddress;

		var expectedFamily = parts[1] switch
		{
			"TCP4" => AddressFamily.InterNetwork,
			"TCP6" => AddressFamily.InterNetworkV6,
			_ => AddressFamily.Unknown
		};
		if (expectedFamily == AddressFamily.Unknown ||
		    sourceAddress.AddressFamily != expectedFamily ||
		    destinationAddress.AddressFamily != expectedFamily)
		{
			sourceAddress = IPAddress.None;
			return false;
		}

		return true;
	}
}
