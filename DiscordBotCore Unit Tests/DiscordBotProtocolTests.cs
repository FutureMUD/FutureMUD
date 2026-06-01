using Discord_Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiscordBotProtocolTests
{
	[TestMethod]
	public void PrepareCommandForEngine_EscapesLineBreaks()
	{
		Assert.AreEqual(
			@"broadcast first\nsecond\nthird",
			DiscordBotProtocol.PrepareCommandForEngine("broadcast first\r\nsecond\nthird"));
	}

	[TestMethod]
	public void EncodeCommandForEngine_AppendsRawDelimiter()
	{
		byte[] encoded = DiscordBotProtocol.EncodeCommandForEngine("who 1");
		byte[] expectedPayload = Encoding.Unicode.GetBytes("who 1");

		Assert.AreEqual(DiscordBotProtocol.EngineCommandDelimiter, encoded[^1]);
		CollectionAssert.AreEqual(expectedPayload, encoded.Take(encoded.Length - 1).ToArray());
	}

	[TestMethod]
	public async Task CheckIncomingAsync_ClosesConnectionWhenIncompleteFrameExceedsLimit()
	{
		using var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();
		var port = ((IPEndPoint)listener.LocalEndpoint).Port;

		using var client = new TcpClient();
		var acceptTask = listener.AcceptTcpClientAsync();
		await client.ConnectAsync(IPAddress.Loopback, port);
		using var serverClient = await acceptTask;
		var connection = new TcpConnection(serverClient, null!, null!);

		var oversizedPayload = Enumerable
			.Repeat((byte)2, TcpConnection.MaximumIncomingCommandBytes + 1)
			.ToArray();
		await client.GetStream().WriteAsync(oversizedPayload);

		for (var i = 0; i < 32 && !connection.Closing; i++)
		{
			await Task.Delay(10);
			await connection.CheckIncomingAsync();
		}

		Assert.IsTrue(connection.Closing);
		Assert.IsFalse(connection.TcpClientAuthenticated);
	}

	[TestMethod]
	public void ParseShutdownNotification_DefaultsLegacyPayloadToReboot()
	{
		var result = DiscordBotProtocol.ParseShutdownNotification("AdminUser");

		Assert.AreEqual("AdminUser", result.ShutdownAccount);
		Assert.IsTrue(result.Reboot);
	}

	[TestMethod]
	public void ParseShutdownNotification_ReadsExplicitStopFlag()
	{
		var result = DiscordBotProtocol.ParseShutdownNotification("\"Admin User\" false");

		Assert.AreEqual("Admin User", result.ShutdownAccount);
		Assert.IsFalse(result.Reboot);
	}
}
