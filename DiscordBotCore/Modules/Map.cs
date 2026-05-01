using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MudSharp.Framework;
using System.Linq;
using System.Threading.Tasks;

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

        DetailedUserSetting registration = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
        if (registration is null)
        {
            await context.RespondAsync($"You have not yet linked your MUD account and discord account, which is a necessary prerequisite of this command.");
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
        DiscordBot.Instance.CachedDiscordRequests[request.RequestId] = request;
        await connection.SendTcpCommand($"map {request.RequestId} {registration.MudAccountId} {cellId}");
    }

    private async Task HandleMudResponse(string text, CommandContext context)
    {
        StringStack ss = new(text);
        switch (ss.Pop())
        {
            case "notauthorised":
                await context.RespondAsync($"{context.User.Mention} - Your linked MUD account is not authorised to view maps.");
                return;
            case "nosuchcell":
                await context.RespondAsync($"{context.User.Mention} - There is no room with an ID of **{ss.RemainingArgument}**.");
                return;
            case "map":
                await context.RespondAsync($"{context.User.Mention}\n```{ss.RemainingArgument}```");
                return;
            default:
                await context.RespondAsync($"{context.User.Mention}\n```{text}```");
                return;
        }
    }
}
