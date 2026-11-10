using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules;

public class Where : BaseCommandModule
{
    [Command("where")]
    public async Task WhereAsync(CommandContext context)
    {
        if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
        {
            await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
            return;
        }

        var registration = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
        if (registration is null)
        {
            await context.RespondAsync("You must link your MUD account before using this command.");
            return;
        }

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
        if (!await DiscordBot.Instance.TryCacheDiscordRequestAsync(request)) return;
        await connection.SendTcpCommand($"where {request.RequestId} {registration.MudAccountId}");
    }

    private async Task HandleMudResponse(string text, CommandContext context)
    {
        await DiscordBot.SendPrivateCommandResponseAsync(context, $"```{text}```");
    }
}
