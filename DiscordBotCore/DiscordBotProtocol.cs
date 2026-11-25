using MudSharp.Framework;
using System.Linq;
using System.Text;

namespace Discord_Bot;

#nullable enable

public static class DiscordBotProtocol
{
	public const byte EngineCommandDelimiter = (byte)'\n';

	internal static string AccountCreatedMessage(string account, string gameName) =>
		DiscordResponseText.Truncate($"A new account called **{DSharpPlus.Formatter.Sanitize(account)}** has been created on **{DSharpPlus.Formatter.Sanitize(gameName)}**", 2000);

	internal static string? EnforcementMessage(string subtype, StringStack ss)
	{
		if (subtype is "trialrequested" or "trialstarted" or "executionpatrol")
		{
			return DSharpPlus.Formatter.Sanitize(ss.RemainingArgument);
		}
		string authority = ss.PopSpeech();
		string criminalid = ss.Pop();
		string criminalname = ss.PopSpeech();
		switch (subtype)
		{
			case "enforcement":
				string patrol = ss.PopSpeech();
				string crime = ss.PopSpeech();
				string location = ss.PopSpeech();
				string action = ss.PopSpeech();
				return $"Enforcement Action in **{authority}** at **{location}**:\n\tThe **{patrol}** patrol is enforcing **{crime}** against **{criminalname}** ({criminalid}) - *{action}*";
			case "conviction":
				crime = ss.PopSpeech();
				string result = ss.PopSpeech();
				return $"Conviction Recorded in **{authority}**:\n\t**{criminalname}** ({criminalid}) was convicted of **{crime}** with a punishment of **{result}**";
			case "returnfrombail":
				return $"**{criminalname}** ({criminalid}) has returned from bail in **{authority}**";
			case "release":
				return $"**{criminalname}** ({criminalid}) has been released from custody in **{authority}**";
			case "imprisonment":
				string length = ss.PopSpeech();
				return $"**{criminalname}** ({criminalid}) has been sentenced to **{length}** imprisonment in **{authority}**";
			case "incarceration":
				return $"**{criminalname}** ({criminalid}) has been incarcerated awaiting trial in **{authority}**";
		}
		return null;
	}

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
