using MudWebSocketProxy;

namespace MudClientTests;

public class ProxyStartupValidationTests
{
	[Fact]
	public void ValidateSettingsModeChecksConfigurationWithoutStartingTheProxy()
	{
		var settingsPath = Path.Combine(Path.GetTempPath(), $"mudclient-settings-{Guid.NewGuid():N}.json");
		File.WriteAllText(settingsPath, """
		{
		  "MudServer": { "Address": "127.0.0.1", "Port": 4000 },
		  "WebSocketServer": {
		    "Path": "/ws",
		    "RequireOrigin": true,
		    "AllowedOrigins": [ "https://play.example.com" ]
		  }
		}
		""");

		try
		{
			Program.Main(["--settings", settingsPath, "--validate-settings", "true"]);
		}
		finally
		{
			File.Delete(settingsPath);
		}
	}
}
