using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot.Modules;

public class Send : BaseCommandModule
{
    [Command("send")]
    public async Task SendAsync(CommandContext context, string to, [RemainingText] string message)
    {
        if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
        {
            await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
            return;
        }

        DetailedUserSetting registration = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
        if (registration == null)
        {
            await context.RespondAsync($"You have not yet tied your discord account to a MUD account, {context.User.Mention}. You must 'REGISTER <mud account>' before you can use the SEND command.");
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
        await connection.SendTcpCommand($"send {request.RequestId} {registration.MudAccountName} {to} {message}");
    }

    private async Task HandleMudResponse(string text, CommandContext context)
    {
        StringStack ss = new(text);
        switch (ss.Pop())
        {
            case "sendfailed":
                string to = ss.Pop();
                await context.RespondAsync($"{context.User.Mention} - Failed to send your message, couldn't find a logged in account with the name **{to}**.");
                return;
            case "sendacknowledge":
                string from = ss.Pop();
                to = ss.Pop();
                string message = ss.RemainingArgument;
                await context.RespondAsync(
                    $"{context.User.Mention}:\n\n```**[Sent to {to} as {from}]** {message}```");
                return;
        }
    }
}
