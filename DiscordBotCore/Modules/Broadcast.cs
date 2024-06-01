using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord_Bot.Modules;

public class Broadcast : BaseCommandModule
{
	[Command("broadcast")]
	public async Task BroadcastAsync(CommandContext context, [RemainingText]string message)
	{
            if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
            {
                await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
                return;
            }

            var registration = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
            if (registration == null)
            {
                await context.RespondAsync($"You have not yet tied your discord account to a MUD account, {context.User.Mention}. You must 'REGISTER <mud account>' before you can use the BROADCAST command.");
                return;
            }

            await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));
            await DiscordBot.Instance.AskMudToBroadcast(message);
        }
}