using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.EventHandling;
using MudSharp.Framework;

namespace Discord_Bot.Modules;

public class Love : BaseCommandModule {
	public string[] Responses = {
            "I love you too, {0}!",
            "I love you more, {0}!",
            "Awww, you're so sweet {0}!",
            "Seriously {0}, I love you more!",
            "Aww shucks...you're just saying that {0}...",
            "I have strong feelings of amiability towards you too, {0}!",
            "I have been programmed to feel love towards you too, {0}!",
            "The feeling is mutual, {0}!",
            "I'm just doing my job, {0}.",
            "I like you too, {0}.",
            "It feels good to be loved, {0}.",
            "Our love is tragically forbidden, {0}.",
            "Let's run away together, {0}.",
        };

	public string[] Reactions = {
            "👌",
            "👌",
            "👌",
            "❤︎",
            "❤︎",
            "❤︎",
            "❤︎",
            "❤︎",
            "🌹",
            "🌹",
            "🌹",
            "🌹",
            "💯",
            "💯",
            "💞",
            "💕",
            "💖",
            "💖",
            "😍",
            "😍",
        };

	[Command("love")]
	public async Task LoveAsync(CommandContext context)
	{
            await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(Reactions.GetRandomElement()));
            await context.RespondAsync(string.Format(Responses.GetRandomElement(), context.User.Mention));
        }
}