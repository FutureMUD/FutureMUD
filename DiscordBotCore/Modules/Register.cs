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
    public class Register : BaseCommandModule
    {
        [Command("register")]
        public async Task RegisterAsync(CommandContext context, string account)
        {
            if (DiscordBot.Instance.DetailedUserSettings.Any(x => x.MudAccountName.Equals(account, StringComparison.InvariantCultureIgnoreCase)))
            {
                await context.RespondAsync("There is already a registration for an account with that name.");
                return;
            }

            if (DiscordBot.Instance.DetailedUserSettings.Any(x => x.DiscordUserId == context.User.Id))
            {
                await context.RespondAsync("There is already a registration for your discord account.");
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
            await DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"register {request.RequestId} {context.User.Id} \"{context.User.Username}\" {account}");
        }

        private async Task HandleMudResponse(string text, CommandContext context)
        {
            var ss = new StringStack(text);
            switch (ss.Pop())
            {
                case "registeracknowledge":
                    var name = ss.PopSpeech();
                    var id = long.Parse(ss.PopSpeech());
                    DiscordBot.Instance.DetailedUserSettings.RemoveAll(x => x.MudAccountId == id || x.DiscordUserId == context.User.Id);
                    DiscordBot.Instance.DetailedUserSettings.Add(new DetailedUserSetting(context.User.Id, name, id));
                    await context.RespondAsync($"{context.User.Mention} - You have successfully linked your discord account to MUD account **{name}**.");
                    await DiscordBot.Instance.SaveRegistrationConfig();
                    return;
                case "notfound":
                    await context.RespondAsync($"{context.User.Mention} - Unable to find any accounts with that name currently logged into the MUD.");
                    return;
                case "timeout":
                    await context.RespondAsync($"{context.User.Mention} - Your request to link your discord and MUD account timed out.");
                    return;
                case "rejected":
                    await context.RespondAsync($"{context.User.Mention} - Your request to link your discord and MUD account was rejected by the in-game user.");
                    return;
            }
        }
    }
}
