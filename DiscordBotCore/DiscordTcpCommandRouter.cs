#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Framework;

namespace Discord_Bot;

internal readonly record struct DiscordTcpCommand(string Name, string Payload, bool RequiresAuthentication);

internal static class DiscordTcpCommandRouter
{
	private static readonly HashSet<string> AuthenticatedCommands = new(StringComparer.Ordinal)
	{
		"request",
		"shutdown",
		"crash",
		"chargen",
		"chargen_approved",
		"chargen_rejected",
		"petition",
		"sendmessage",
		"broadcast",
		"badecho",
		"notifyadmins",
		"notifydeath",
		"enforcement",
		"custom",
		"ingamechannel",
		"progerror"
	};

	public static bool TryParse(string input, out DiscordTcpCommand command)
	{
		var stack = new StringStack(input);
		var name = stack.Pop();
		if (name.EqualTo("login"))
		{
			command = new DiscordTcpCommand("login", stack.RemainingArgument, false);
			return true;
		}

		if (!AuthenticatedCommands.Contains(name))
		{
			command = default;
			return false;
		}

		command = new DiscordTcpCommand(name, stack.RemainingArgument, true);
		return true;
	}
}
