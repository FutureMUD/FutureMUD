using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord_Bot.Modules;

public class Map : BaseCommandModule
{
    [Command("map")]
    public async Task MapAsync(CommandContext context, long cellId)
    {
        if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
        {
            await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
            return;
        }

        var registration = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
        if (registration is null)
        {
            await context.RespondAsync($"You have not yet linked your MUD account and discord account, which is a necessary prerequisite of this command.");
            return;
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
        DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated)
            .SendTcpCommand($"map {request.RequestId} {registration.MudAccountId} {cellId}");
    }

    private async Task HandleMudResponse(string text, CommandContext context)
    {
        await context.RespondAsync($"{context.User.Mention}\n```{text}```");
    }
}
