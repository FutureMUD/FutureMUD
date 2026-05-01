using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules;

public class Stats : BaseCommandModule
{
    [Command("stats")]
    [Aliases("stat")]
    public async Task StatsAsync(CommandContext context)
    {
        if (!DiscordBot.Instance.TryGetAuthenticatedConnection(out TcpConnection connection))
        {
            await context.RespondAsync($"{context.User.Mention} - I'm not currently connected to the MUD so I cannot do that for you.");
            return;
        }

        await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));
        CachedDiscordRequest request = new()
        {
            Context = context,
            OnResponseAction = HandleMudResponse
        };
        DiscordBot.Instance.CachedDiscordRequests[request.RequestId] = request;
        await connection.SendTcpCommand($"stats {request.RequestId}");
    }

    private async Task HandleMudResponse(string text, CommandContext context)
    {
        await context.RespondAsync($"{context.User.Mention}\n```{text}```");
    }
}
