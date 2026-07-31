using Microsoft.Extensions.Configuration;
using MudClientBlazor;

namespace MudClientTests;

public class WebSocketEndpointTests
{
	[Fact]
	public void FromConfiguration_AddsConfiguredProxyPath()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["WebSocketServer:ClientAddress"] = "ws://127.0.0.1",
			["WebSocketServer:ClientPort"] = "5000",
			["WebSocketServer:Path"] = "/ws"
		});

		var endpoint = WebSocketEndpoint.FromConfiguration(configuration);

		Assert.Equal("ws://127.0.0.1:5000/ws", endpoint.ToString());
	}

	[Fact]
	public void FromConfiguration_DefaultsToProxyPath()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["WebSocketServer:ClientAddress"] = "ws://localhost",
			["WebSocketServer:ClientPort"] = "5000"
		});

		var endpoint = WebSocketEndpoint.FromConfiguration(configuration);

		Assert.Equal("ws://localhost:5000/ws", endpoint.ToString());
	}

	[Fact]
	public void FromConfiguration_UsesFullEndpointWhenProvided()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["WebSocketServer:Endpoint"] = "ws://localhost:5000/ws"
		});

		var endpoint = WebSocketEndpoint.FromConfiguration(configuration);

		Assert.Equal("ws://localhost:5000/ws", endpoint.ToString());
	}

	[Fact]
	public void FromConfiguration_ResolvesRelativeEndpointFromHttpAppBase()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["WebSocketServer:Endpoint"] = "/ws"
		});

		var endpoint = WebSocketEndpoint.FromConfiguration(configuration, new Uri("http://play.example.com/"));

		Assert.Equal("ws://play.example.com/ws", endpoint.ToString());
	}

	[Fact]
	public void FromConfiguration_ResolvesRelativeEndpointFromHttpsAppBase()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["WebSocketServer:Endpoint"] = "/ws"
		});

		var endpoint = WebSocketEndpoint.FromConfiguration(configuration, new Uri("https://play.example.com/client/"));

		Assert.Equal("wss://play.example.com/ws", endpoint.ToString());
	}

	[Fact]
	public void FromConfiguration_RejectsRelativeEndpointWithoutAppBase()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["WebSocketServer:Endpoint"] = "/ws"
		});

		var exception = Assert.Throws<InvalidOperationException>(() => WebSocketEndpoint.FromConfiguration(configuration));

		Assert.Equal("WebSocketServer:Endpoint must be an absolute ws:// or wss:// URI.", exception.Message);
	}

	[Fact]
	public void FromConfiguration_RejectsInvalidPort()
	{
		var configuration = BuildConfiguration(new Dictionary<string, string?>
		{
			["WebSocketServer:ClientAddress"] = "ws://localhost",
			["WebSocketServer:ClientPort"] = "not-a-port"
		});

		var exception = Assert.Throws<InvalidOperationException>(() => WebSocketEndpoint.FromConfiguration(configuration));

		Assert.Equal("WebSocketServer:ClientPort must be a valid TCP port.", exception.Message);
	}

	private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
	{
		return new ConfigurationBuilder()
			.AddInMemoryCollection(values)
			.Build();
	}
}
