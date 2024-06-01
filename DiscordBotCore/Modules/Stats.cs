using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord_Bot.Modules;

public class Stats : BaseCommandModule
{
	[Command("stats")]
	[Aliases("stat")]
	public async Task StatsAsync(CommandContext context)
	{
            if (!DiscordBot.Instance.TCPConnections.Any(x => x.TcpClientAuthenticated))
            {
                await context.RespondAsync($"{context.User.Mention} - I'm not currently connected to the MUD so I cannot do that for you.");
                return;
            }

            await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));
            var request = new CachedDiscordRequest
            {
                Context = context,
                OnResponseAction = HandleMudResponse
            };
            DiscordBot.Instance.CachedDiscordRequests[request.RequestId] = request;
            DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"stats {request.RequestId}");
        }

	private async Task HandleMudResponse(string text, CommandContext context)
	{
            await context.RespondAsync($"{context.User.Mention}\n```{text}```");
        }
}