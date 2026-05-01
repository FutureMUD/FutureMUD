using MudSharp.Framework;
using System.Linq;
using System.Text;

namespace Discord_Bot;

#nullable enable

public static class DiscordBotProtocol
{
	public const byte EngineCommandDelimiter = (byte)'\n';

	public static string PrepareCommandForEngine(string command)
	{
		return command
			.Replace("\r\n", "\\n")
			.Replace("\r", "\\n")
			.Replace("\n", "\\n");
	}

	public static byte[] EncodeCommandForEngine(string command)
	{
		return Encoding.Unicode.GetBytes(PrepareCommandForEngine(command))
			.Concat(new[] { EngineCommandDelimiter })
			.ToArray();
	}

	public static (string ShutdownAccount, bool Reboot) ParseShutdownNotification(string payload)
	{
		var ss = new StringStack(payload);
		var shutdownAccount = ss.PopSpeech();
		var reboot = ss.IsFinished || !bool.TryParse(ss.PopSpeech(), out var parsedReboot) || parsedReboot;
		return (shutdownAccount, reboot);
	}
}
