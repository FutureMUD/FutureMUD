namespace MudWebSocketProxy.Security;

public sealed class ProxyLimits
{
	public int MaximumConcurrentConnections { get; init; } = 200;
	public int MaximumConnectionsPerIp { get; init; } = 20;
	public int MaximumClientMessageBytes { get; init; } = 65_536;
	public int MaximumClientMessagesPerSecond { get; init; } = 30;
	public int MaximumClientBytesPerSecond { get; init; } = 131_072;
	public int MaximumMudBytesPerSecond { get; init; } = 2_097_152;
	public TimeSpan MudConnectionTimeout { get; init; } = TimeSpan.FromSeconds(10);

	public static ProxyLimits FromConfiguration(IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		return new ProxyLimits
		{
			MaximumConcurrentConnections = ReadBounded(configuration, "ProxyLimits:MaximumConcurrentConnections", 200, 1, 10_000),
			MaximumConnectionsPerIp = ReadBounded(configuration, "ProxyLimits:MaximumConnectionsPerIp", 20, 1, 1_000),
			MaximumClientMessageBytes = ReadBounded(configuration, "ProxyLimits:MaximumClientMessageBytes", 65_536, 1_024, 1_048_576),
			MaximumClientMessagesPerSecond = ReadBounded(configuration, "ProxyLimits:MaximumClientMessagesPerSecond", 30, 1, 1_000),
			MaximumClientBytesPerSecond = ReadBounded(configuration, "ProxyLimits:MaximumClientBytesPerSecond", 131_072, 1_024, 16_777_216),
			MaximumMudBytesPerSecond = ReadBounded(configuration, "ProxyLimits:MaximumMudBytesPerSecond", 2_097_152, 4_096, 67_108_864),
			MudConnectionTimeout = TimeSpan.FromSeconds(ReadBounded(configuration, "ProxyLimits:MudConnectionTimeoutSeconds", 10, 1, 120))
		};
	}

	private static int ReadBounded(IConfiguration configuration, string key, int fallback, int minimum, int maximum)
	{
		var value = configuration.GetValue<int?>(key) ?? fallback;
		return Math.Clamp(value, minimum, maximum);
	}
}
