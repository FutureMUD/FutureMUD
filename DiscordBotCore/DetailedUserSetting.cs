using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot;

public class DetailedUserSetting
{
	public DetailedUserSetting(string configLine)
	{
            var split = configLine.Split(',');
            DiscordUserId = ulong.Parse(split[0]);
            MudAccountId = long.Parse(split[2]);
            MudAccountName = split[1];
        }

	public DetailedUserSetting(ulong discordid, string mudname, long mudaccountid)
	{
            DiscordUserId = discordid;
            MudAccountName = mudname;
            MudAccountId = mudaccountid;
        }

	public string SaveToConfig()
	{
            return $"{DiscordUserId.ToString("F0")},{MudAccountName},{MudAccountId.ToString("F0")}";
        }

	public ulong DiscordUserId { get; set; }
	public string MudAccountName { get; set; }
	public long MudAccountId { get; set; }
}