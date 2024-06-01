using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord_Bot.Modules;

public class Where : BaseCommandModule
{
	[Command("where")]
	public async Task WhereAsync(CommandContext context)
	{
            if (!DiscordBot.Instance.IsAuthorisedUser(context.User)) {
                await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
            }

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
            DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"where {request.RequestId}");
        }

	private async Task HandleMudResponse(string text, CommandContext context)
	{
            await context.RespondAsync($"{context.User.Mention}\n```{text}```");
        }
}