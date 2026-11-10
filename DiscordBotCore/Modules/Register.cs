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

public class Register : BaseCommandModule
{
    [Command("register")]
    public async Task RegisterAsync(CommandContext context, string account)
    {
        var existingDiscordLink = DiscordBot.Instance.DetailedUserSettings.FirstOrDefault(x => x.DiscordUserId == context.User.Id);
        if (existingDiscordLink is not null && !existingDiscordLink.MudAccountName.Equals(account, StringComparison.InvariantCultureIgnoreCase))
        {
            await context.RespondAsync("Your Discord account is already linked to a different MUD account.");
            return;
        }

        if (DiscordBot.Instance.DetailedUserSettings.Any(x =>
                x.MudAccountName.Equals(account, StringComparison.InvariantCultureIgnoreCase) &&
                x.DiscordUserId != context.User.Id))
        {
            await context.RespondAsync("There is already a registration for an account with that name.");
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
			// The in-game ownership proposal remains valid for two minutes.
			Timeout = TimeSpan.FromMinutes(3),
            Context = context,
            OnResponseAction = HandleMudResponse
        };
        if (!await DiscordBot.Instance.TryCacheDiscordRequestAsync(request)) return;
        await connection.SendTcpCommand($"register {request.RequestId} {context.User.Id} \"{context.User.Username}\" {account}");
    }

    private async Task HandleMudResponse(string text, CommandContext context)
    {
        StringStack ss = new(text);
        switch (ss.Pop())
        {
            case "registeracknowledge":
                string name = ss.PopSpeech();
                long id = long.Parse(ss.PopSpeech());
				var persisted = await DiscordBot.Instance.ReplaceAccountLinkAsync(new DetailedUserSetting(context.User.Id, name, id));
				var message = persisted
					? $"You have successfully linked your discord account to MUD account **{name}**."
					: $"The game approved the link to **{name}**, but the bot could not persist its local mirror. The engine link is authoritative; contact a bot owner before using legacy bot commands.";
				await DiscordBot.SendPrivateCommandResponseAsync(context, message);
                return;
            case "notfound":
                await DiscordBot.SendPrivateCommandResponseAsync(context, "Unable to find any accounts with that name currently logged into the MUD.");
                return;
            case "timeout":
                await DiscordBot.SendPrivateCommandResponseAsync(context, "Your request to link your discord and MUD account timed out.");
                return;
            case "rejected":
                await DiscordBot.SendPrivateCommandResponseAsync(context, "Your request to link your discord and MUD account was rejected by the in-game user.");
                return;
        }
    }
}
