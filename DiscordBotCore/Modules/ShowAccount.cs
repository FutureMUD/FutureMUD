using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MudSharp.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Discord_Bot.Modules;

public class ShowAccount : BaseCommandModule
{
    [Command("showaccount")]
    public async Task ShowAccountAsync(CommandContext context, string which)
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
        await connection.SendTcpCommand($"showaccount {request.RequestId} {registration.MudAccountId} {which}");
    }

    private async Task HandleMudResponse(string text, CommandContext context)
    {
        StringStack ss = new(text);
        string type = ss.Pop();
        switch (type)
        {
            case "nosuchaccount":
                await context.RespondAsync($"{context.User.Mention} - There is no account with the name or id **{ss.RemainingArgument}**.");
                return;
            case "accountinfo":
                string message = ss.RemainingArgument;
                foreach (string part in message.SplitStringsForDiscord())
                {
                    await context.RespondAsync($"```{part}```");
                }
                return;
        }
    }
}
