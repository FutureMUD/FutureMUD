using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord_Bot.Modules;

public class Authorise: BaseCommandModule
{
	[Command("authorise")]
	[Aliases("authorize")]
	public async Task AuthoriseAsync(CommandContext context, [RemainingText]string remainder)
	{
            if (!DiscordBot.Instance.IsAuthorisedUser(context.User))
            {
                await context.RespondAsync($"You are not authorised to do that command, {context.User.Mention}.");
                return;
            }

            var target = context.Message.MentionedUsers.FirstOrDefault();
            if (target == null)
            {
                await context.RespondAsync($"You must mention someone with an @ ping to authorise, {context.User.Mention}.");
                return;
            }

            if (DiscordBot.Instance.IsAuthorisedUser(target))
            {
                await context.RespondAsync($"**{target.Username}** is already an authorised user, {context.User.Mention}.");
                return;
            }

            await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));
            await DiscordBot.Instance.AddAuthorisedUser(target.Id);
            await context.RespondAsync($"{context.User.Mention} - Successfully authorised {target.Mention} as an admin user.");
        }
}