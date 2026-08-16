using Microsoft.Extensions.Configuration;
using MudWebSocketProxy.Security;
using System.Net;
using System.Text;

namespace MudClientTests;

public class ProxySecurityTests
{
	[Fact]
	public void ConnectionLimiter_EnforcesPerAddressAndReleasesLease()
	{
		var limits = new ProxyLimits
		{
			MaximumConcurrentConnections = 2,
			MaximumConnectionsPerIp = 1
		};
		var limiter = new ProxyConnectionLimiter(limits);

		using var first = limiter.TryAcquire("192.0.2.1");
		Assert.NotNull(first);
		Assert.Null(limiter.TryAcquire("192.0.2.1"));

		first.Dispose();
		using var replacement = limiter.TryAcquire("192.0.2.1");
		Assert.NotNull(replacement);
	}

	[Fact]
	public void TrafficLimiter_EnforcesMessageAndByteRatesAndResetsWindow()
	{
		var limits = new ProxyLimits
		{
			MaximumClientMessagesPerSecond = 2,
			MaximumClientBytesPerSecond = 10
		};
		var now = DateTimeOffset.UtcNow;
		var limiter = new ClientTrafficLimiter(limits, now);

		Assert.True(limiter.TryConsumeMessage(5, now));
		Assert.True(limiter.TryConsumeMessage(5, now));
		Assert.False(limiter.TryConsumeMessage(1, now));
		Assert.True(limiter.TryConsumeMessage(10, now.AddSeconds(1)));
	}

	[Fact]
	public void ByteTrafficLimiter_EnforcesAndResetsWindow()
	{
		var now = DateTimeOffset.UtcNow;
		var limiter = new ByteTrafficLimiter(10, now);

		Assert.True(limiter.TryConsume(6, now));
		Assert.False(limiter.TryConsume(5, now));
		Assert.True(limiter.TryConsume(10, now.AddSeconds(1)));
	}

	[Fact]
	public void ProxyLimits_ClampsUnsafeConfigurationValues()
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["ProxyLimits:MaximumConcurrentConnections"] = "0",
				["ProxyLimits:MaximumClientMessageBytes"] = "99999999",
				["ProxyLimits:MudConnectionTimeoutSeconds"] = "0"
			})
			.Build();

		var limits = ProxyLimits.FromConfiguration(configuration);

		Assert.Equal(1, limits.MaximumConcurrentConnections);
		Assert.Equal(1_048_576, limits.MaximumClientMessageBytes);
		Assert.Equal(TimeSpan.FromSeconds(1), limits.MudConnectionTimeout);
	}

	[Theory]
	[InlineData("https://play.example.com", true)]
	[InlineData("https://PLAY.example.com/", true)]
	[InlineData("https://play.example.com:444", false)]
	[InlineData("https://evil.example.com", false)]
	public void OriginPolicy_RequiresAnExactConfiguredAuthority(string origin, bool expected)
	{
		Assert.Equal(
			expected,
			WebSocketOriginPolicy.IsAllowed(origin, true, ["https://play.example.com"]));
	}

	[Fact]
	public void OriginPolicy_RejectsMissingOriginByDefault()
	{
		Assert.False(WebSocketOriginPolicy.IsAllowed(null, true, ["https://play.example.com"]));
		Assert.True(WebSocketOriginPolicy.IsAllowed(null, false, ["https://play.example.com"]));
	}

	[Fact]
	public void ProxyConfigurationValidator_RejectsUnsafeProductionOriginConfiguration()
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["MudServer:Address"] = "127.0.0.1",
				["MudServer:Port"] = "4000",
				["MudServer:SendProxyProtocol"] = "true",
				["WebSocketServer:RequireOrigin"] = "false",
				["WebSocketServer:AllowedOrigins:0"] = "https://play.example.com"
			})
			.Build();

		Assert.Throws<InvalidOperationException>(() => ProxyConfigurationValidator.Validate(configuration));
	}

	[Fact]
	public void ProxyProtocolHeader_ContainsTheForwardedClientAddress()
	{
		var header = ProxyProtocolV1Header.Build(IPAddress.Parse("203.0.113.42"), 43210, 4000);

		Assert.Equal(
			"PROXY TCP4 203.0.113.42 0.0.0.0 43210 4000\r\n",
			Encoding.ASCII.GetString(header));
	}

	[Fact]
	public void ProxyConfigurationValidator_RequiresClientIpPropagation()
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>
			{
				["MudServer:Address"] = "127.0.0.1",
				["MudServer:Port"] = "4000",
				["MudServer:SendProxyProtocol"] = "false",
				["WebSocketServer:RequireOrigin"] = "true",
				["WebSocketServer:AllowedOrigins:0"] = "https://play.example.com"
			})
			.Build();

		Assert.Throws<InvalidOperationException>(() => ProxyConfigurationValidator.Validate(configuration));
	}
}
