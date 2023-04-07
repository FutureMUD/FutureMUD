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
    public class RejectChargen : BaseCommandModule
    {
        [Command("rejectchargen")]
        public async Task RejectChargenAsync(CommandContext context, string which, [RemainingText]string comment)
        {
            if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
            {
                await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
                return;
            }

            if (!DiscordBot.Instance.DetailedUserSettings.Any(x => x.DiscordUserId == context.User.Id))
            {
                await context.RespondAsync($"You have not yet linked your MUD account and discord account, which is a necessary prerequisite of this command.");
                return;
            }

            if (!long.TryParse(which, out _))
            {
                await context.RespondAsync($"You must supply a numeric ID of the chargen you want to reject, {context.User.Mention}.");
                return;
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                await context.RespondAsync($"You must supply a comment to reject a chargen application, {context.User.Mention}.");
                return;
            }

            if (!DiscordBot.Instance.TCPConnections.Any(x => x.TcpClientAuthenticated))
            {
                await context.RespondAsync($"{context.User.Mention} - I'm not currently connected to the MUD so I cannot do that for you.");
                return;
            }

            await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));
            var accountid = DiscordBot.Instance.DetailedUserSettings.First(x => x.DiscordUserId == context.User.Id).MudAccountId;

            var request = new CachedDiscordRequest
            {
                Context = context,
                OnResponseAction = HandleMudResponse
            };
            DiscordBot.Instance.CachedDiscordRequests[request.RequestId] = request;
            DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"rejectchargen {request.RequestId} {which} {accountid} {comment}");
        }

        private async Task HandleMudResponse(string text, CommandContext context)
        {
            var ss = new StringStack(text);
            switch (ss.Pop())
            {
                case "nosuchchargen":
                    var chargenId = long.Parse(ss.Pop());
                    await context.RespondAsync($"{context.User.Mention} - There is no character application with an ID of {chargenId}.");
                    return;
                case "chargenapprovalerror":
                    chargenId = long.Parse(ss.Pop());
                    await context.RespondAsync($"{context.User.Mention} - Error with rejecting character application {chargenId}: {ss.RemainingArgument}");
                    return;
                default:
                    return;
            }
        }
    }
}
