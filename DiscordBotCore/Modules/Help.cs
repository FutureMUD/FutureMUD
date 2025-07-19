using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using MudSharp.Framework;

namespace Discord_Bot.Modules;

public class Help : BaseCommandModule
{
	[Command("proghelp")]
	[Aliases("phelp")]
	public async Task ProgHelpAsync(CommandContext context, [RemainingText] string arguments = "")
	{
		    if (!DiscordBot.Instance.TCPConnections.Any(x => x.TcpClientAuthenticated))
		    {
			    await context.RespondAsync($"{context.User.Mention} - I'm not currently connected to the MUD so I cannot do that for you.");
			    return;
		    }

		    await context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👌"));

		    var request = new CachedDiscordRequest
		    {
			    Context = context,
			    OnResponseAction = HandleMudProgHelpResponse
			};
		    DiscordBot.Instance.CachedDiscordRequests[request.RequestId] = request;
		    DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"proghelp {request.RequestId} {arguments}");
		}



	private async Task HandleMudProgHelpResponse(string text, CommandContext context)
	{
		    await context.RespondAsync($"{context.User.Mention} - Prog Help Request");
		    foreach (var part in text.SplitStringsForDiscord())
		    {
			    await context.RespondAsync($"```{part}```");
		    }
	    }

	[Command("help")]
	[Aliases("halp", "hjalp")]
	public async Task HelpAsync(CommandContext context, [RemainingText]string arguments = "")
	{
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await context.RespondAsync(@$"Thanks {context.User.Mention}, here is how to use me:

    **help** - shows this help message
    **help <helpfile>** - shows a helpfile from the MUD
    **playerhelp <helpfile>** - shows the player-specific version of a helpfile, if there are two versions
	**proghelp <args>** - shows help on prog statements, functions, etc. Do PROGHELP on its own to see full syntax.
    **who** - shows the WHO output from the MUD
    **stats** - shows the STATS output from the MUD
    **status** - shows whether the discord bot is connected to the MUD
    **register <account>** - connects your discord and MUD accounts (you need to be logged in to the MUD)

The following commands require you to be registered but not necessarily authorised before using them:

	**sendchannel <which> <message>** - send a message to an in-game channel (e.g. wiznet)

The following commands require you to be registered and authorised before using them:

    **authorise <@mention>** - authorises someone else as a discord bot admin
    **showchargen <id>** - shows a character application
    **approvechargen <id> <message>** - approves a character application
    **rejectchargen <id> <message>** - rejects a character application
    **where** - shows the WHERE output from the MUD
    **map <cell>** - shows the MAP output for a specific cell
    **showaccount <name|id>** - shows an account's details
    **showcharacter <id>** - shows a character's details
    **send <account> <message>** - sends a message to an account in-game, equivalent to SEND command
    **broadcast <message>** - broadcasts a system message (same as using BROADCAST in game)
    **shutdown** - shuts down the MUD
    **shutdownstop** - shuts down the MUD and stops it from rebooting
");
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
                OnResponseAction = HandleMudHelpResponse
            };
            DiscordBot.Instance.CachedDiscordRequests[request.RequestId] = request;
            DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"adminhelp {request.RequestId} {arguments}");
        }

	private async Task HandleMudHelpResponse(string text, CommandContext context)
	{
            await context.RespondAsync($"{context.User.Mention} - Help Request");
            foreach (var part in text.SplitStringsForDiscord())
            {
                await context.RespondAsync($"```{part}```");
            }
        }

	[Command("playerhelp")]
	public async Task PlayerHelpAsync(CommandContext context, [RemainingText]string arguments)
	{
            if (string.IsNullOrWhiteSpace(arguments))
            {
                await context.RespondAsync(@$"{context.User.Mention} - you must specify a helpfile you want me to retrieve for you.");
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
                OnResponseAction = HandleMudHelpResponse
            };
            DiscordBot.Instance.CachedDiscordRequests[request.RequestId] = request;
            DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"help {request.RequestId} {arguments}");
        }
}