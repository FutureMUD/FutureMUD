using System;
using System.Collections.Generic;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Commands;

internal static class StaticConfigurationAccess
{
	private static readonly HashSet<string> _implementorOnlySettings = new(StringComparer.OrdinalIgnoreCase)
	{
		"EmailServer",
		"EngineUpdateBinariesPath",
		"DiscordAuthToken",
		"DiscordBotIpAddress",
		"DiscordBotPort",
		"GPT_Secret_Key",
		"Gemini_Secret_Key",
		"Anthropic_API_Key"
	};

	public static bool CanAccess(ICharacter actor, string settingName)
	{
		return actor.PermissionLevel >= PermissionLevel.Founder || !_implementorOnlySettings.Contains(settingName);
	}

	public static string DisplayValue(ICharacter actor, string settingName)
	{
		return CanAccess(actor, settingName)
			? actor.Gameworld.GetStaticConfiguration(settingName)
			: "Redacted for Security".ColourError();
	}
}
