using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MudSharp.Framework;

namespace Discord_Bot.Modules
{
    public class Send : BaseCommandModule
    {
        [Command("send")]
        public async Task SendAsync(CommandContext context, string to, [RemainingText]string message)
        {
            if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
            {
                await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
                return;
            }

            var registration = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
            if (registration == null)
            {
                await context.RespondAsync($"You have not yet tied your discord account to a MUD account, {context.User.Mention}. You must 'REGISTER <mud account>' before you can use the SEND command.");
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
            DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"send {request.RequestId} {registration.MudAccountName} {to} {message}");
        }

        private async Task HandleMudResponse(string text, CommandContext context)
        {
            var ss = new StringStack(text);
            switch (ss.Pop())
            {
                case "sendfailed":
                    var to = ss.Pop();
                    await context.RespondAsync($"{context.User.Mention} - Failed to send your message, couldn't find a logged in account with the name **{to}**.");
                    return;
                case "sendacknowledge":
                    var from = ss.Pop();
                    to = ss.Pop();
                    var message = ss.RemainingArgument;
                    await context.RespondAsync(
                        $"{context.User.Mention}:\n\n```**[Sent to {to} as {from}]** {message}```");
                    return;
            }
        }
    }
}
