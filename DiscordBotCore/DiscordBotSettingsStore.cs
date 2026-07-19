#nullable enable

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Discord_Bot;

internal sealed record DiscordBotAccountLinkLoadResult(
	IReadOnlyList<DetailedUserSetting> Settings,
	int InvalidLineCount);

internal static class DiscordBotSettingsStore
{
	public static bool TryLoadSettings(string? json, out DiscordBot.DiscordBotSetttings settings)
	{
		settings = null!;
		if (string.IsNullOrWhiteSpace(json))
		{
			return false;
		}

		try
		{
			var loaded = JsonConvert.DeserializeObject<DiscordBot.DiscordBotSetttings>(json);
			if (loaded is null ||
			    loaded.Port <= 0 ||
			    string.IsNullOrWhiteSpace(loaded.Token) ||
			    string.IsNullOrWhiteSpace(loaded.ServerAuth) ||
			    string.IsNullOrWhiteSpace(loaded.GameName) ||
			    loaded.Prefixes is null ||
			    loaded.AdminUsers is null ||
			    loaded.CustomGlobalReactions is null)
			{
				return false;
			}

			settings = loaded;
			return true;
		}
		catch (JsonException)
		{
			return false;
		}
	}

	public static DiscordBotAccountLinkLoadResult LoadAccountLinks(IEnumerable<string> lines)
	{
		var settings = new List<DetailedUserSetting>();
		var invalidLineCount = 0;
		foreach (var line in lines)
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				continue;
			}

			if (DetailedUserSetting.TryParse(line, out var setting))
			{
				settings.Add(setting);
				continue;
			}

			invalidLineCount++;
		}

		return new DiscordBotAccountLinkLoadResult(settings, invalidLineCount);
	}

	public static IEnumerable<string> SaveAccountLinks(IEnumerable<DetailedUserSetting> settings)
	{
		return settings.Select(x => x.SaveToConfig());
	}
}
