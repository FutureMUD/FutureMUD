using Discord_Bot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;

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
