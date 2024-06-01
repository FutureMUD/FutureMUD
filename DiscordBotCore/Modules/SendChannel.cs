using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Humanizer;
using MudSharp.Framework;

namespace Discord_Bot.Modules;

public class SendChannel : BaseCommandModule
{
	[Command("sendchannel")]
	public async Task SendChannelAsync(CommandContext context, string channel, [RemainingText] string message)
	{
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
		DiscordBot.Instance.TCPConnections.First(x => x.TcpClientAuthenticated).SendTcpCommand($"sendchannel {request.RequestId} \"{registration.MudAccountName}\" \"{channel}\" {message}");
	}

	private async Task HandleMudResponse(string text, CommandContext context)
	{
		var ss = new StringStack(text);
		switch (ss.Pop())
		{
			case "sendfailed":
				var channel = ss.SafeRemainingArgument;
				await context.RespondAsync($"{context.User.Mention} - Failed to send your message, you don't have access to any channel called **{channel}**.");
				return;
			case "sendfailedaccount":
				await context.RespondAsync($"{context.User.Mention} - Failed to send your message, couldn't locate your account.");
				return;
			case "sendacknowledge":
				await context.Message.CreateReactionAsync(DiscordEmoji.FromName(context.Client, ":checkered_flag:"));
				return;
		}
	}
}