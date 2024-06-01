using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MudSharp.Framework;

namespace Discord_Bot.Modules;

public class Thanks : BaseCommandModule
{
	public string[] Responses = {
            "I need no thanks, {0}, doing my duty is enough for me.",
            "I'm just doing my job, {0}.",
            "You're welcome, {0}.",
            "You're welcome, {0}.",
            "You're welcome, {0}.",
            "You're welcome, {0}.",
            "You're welcome, {0}.",
            "You're very welcome, {0}.",
            "I live only to serve, {0}.",
            "Don't worry about it {0}.",
            "Really, it's no trouble at all {0}.",
            "Bitte, {0}.",
            "♫ What can I say, except, YOU'RE WELCOME! ♫ {0}.",
            "De nada, {0}.",
            "It's nothing, {0}."
        };

	[Command("thanks")]
	public async Task ThanksAsync(CommandContext context) {
            await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));
            await context.RespondAsync(string.Format(Responses.GetRandomElement(), context.User.Mention));
        }
}