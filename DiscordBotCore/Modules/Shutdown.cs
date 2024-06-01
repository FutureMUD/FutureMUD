using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord_Bot.Modules;

public class Shutdown : BaseCommandModule
{
	[Command("shutdown")]
	public async Task ShutdownAsync(CommandContext context)
	{
            if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
            {
                await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
                return;
            }

            var registration = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
            if (registration == null)
            {
                await context.RespondAsync($"You have not yet tied your discord account to a MUD account, {context.User.Mention}. You must 'REGISTER <mud account>' before you can use the SHUTDOWN command.");
                return;
            }

            if (!DiscordBot.Instance.TCPConnections.Any(x => x.TcpClientAuthenticated))
            {
                await context.RespondAsync($"{context.User.Mention} - I'm not currently connected to the MUD so I cannot do that for you.");
                return;
            }

            await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));
            await DiscordBot.Instance.AskMudToShutdown(registration.MudAccountId, false);
        }

	[Command("shutdownstop")]
	public async Task ShutdownStopAsync(CommandContext context)
	{
              if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
              {
	              await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
	              return;
              }

              var registration = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
              if (registration == null)
              {
	              await context.RespondAsync($"You have not yet tied your discord account to a MUD account, {context.User.Mention}. You must 'REGISTER <mud account>' before you can use the SHUTDOWNSTOP command.");
	              return;
              }

              if (!DiscordBot.Instance.TCPConnections.Any(x => x.TcpClientAuthenticated))
              {
	              await context.RespondAsync($"{context.User.Mention} - I'm not currently connected to the MUD so I cannot do that for you.");
	              return;
              }

              await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));
              await DiscordBot.Instance.AskMudToShutdown(registration.MudAccountId, true);
        }
}