#nullable enable

using System;
using System.Globalization;

namespace Discord_Bot;

public class DetailedUserSetting
{
	public DetailedUserSetting(string configLine)
	{
		if (!TryParse(configLine, out var setting))
		{
			throw new FormatException("Account-link settings must contain a Discord user id, account name, and MUD account id.");
		}

		DiscordUserId = setting.DiscordUserId;
		MudAccountName = setting.MudAccountName;
		MudAccountId = setting.MudAccountId;
	}

	public DetailedUserSetting(ulong discordId, string mudName, long mudAccountId)
	{
		DiscordUserId = discordId;
		MudAccountName = mudName;
		MudAccountId = mudAccountId;
	}

	internal static bool TryParse(string? configLine, out DetailedUserSetting setting)
	{
		setting = null!;
		if (string.IsNullOrWhiteSpace(configLine))
		{
			return false;
		}

		var split = configLine.Split(',');
		if (split.Length != 3 || string.IsNullOrWhiteSpace(split[1]) ||
		    !ulong.TryParse(split[0], NumberStyles.None, CultureInfo.InvariantCulture, out var discordId) ||
		    !long.TryParse(split[2], NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var mudAccountId))
		{
			return false;
		}

		setting = new DetailedUserSetting(discordId, split[1], mudAccountId);
		return true;
	}

	public string SaveToConfig()
	{
		return string.Join(",",
			DiscordUserId.ToString(CultureInfo.InvariantCulture),
			MudAccountName,
			MudAccountId.ToString(CultureInfo.InvariantCulture));
	}

	public ulong DiscordUserId { get; set; }
	public string MudAccountName { get; set; }
	public long MudAccountId { get; set; }
}
