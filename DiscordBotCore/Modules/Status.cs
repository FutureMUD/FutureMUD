using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord_Bot.Modules
{
    public class Status : BaseCommandModule
    {
        [Command("status")]
        public async Task StatusAsync(CommandContext context) {
            await context.RespondAsync(await DiscordBot.Instance.GetMudStatusAsync());
        }
    }
}
