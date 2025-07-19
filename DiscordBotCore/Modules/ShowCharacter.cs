using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MudSharp.Framework;

namespace Discord_Bot.Modules;

public class ShowCharacter : BaseCommandModule
{
    [Command("showcharacter")]
    public async Task ShowCharacterAsync(CommandContext context, string which)
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
            .SendTcpCommand($"showcharacter {request.RequestId} {registration.MudAccountId} {which}");
    }

    private async Task HandleMudResponse(string text, CommandContext context)
    {
        var ss = new StringStack(text);
        var type = ss.Pop();
        switch (type)
        {
            case "nosuchcharacter":
                await context.RespondAsync($"{context.User.Mention} - There is no character with that ID.");
                return;
            case "characterinfo":
                var message = ss.RemainingArgument;
                foreach (var part in message.SplitStringsForDiscord())
                {
                    await context.RespondAsync($"```{part}```");
                }
                return;
        }
    }
}
