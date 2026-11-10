#nullable enable

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using MudSharp.Discord;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Discord_Bot;

internal sealed record PendingRemoteDiscordRequest(
	DiscordRemoteRequest Request,
	Func<DiscordRemoteResponse, Task> RespondAsync,
	DateTime ExpiresUtc);

internal sealed record PendingDiscordConfirmation(
	PendingRemoteDiscordRequest Pending,
	Guid ConfirmationId);

public partial class DiscordBot
{
	private readonly object _privateClientGate = new();
	private DiscordRestClient? _privateClient;

	internal Task<DiscordDmChannel> CreatePrivateChannelAsync(ulong userId)
	{
		DiscordRestClient client;
		lock (_privateClientGate)
		{
			client = _privateClient ??= new DiscordRestClient(new DiscordConfiguration
			{
				Token = _botToken, TokenType = TokenType.Bot
			});
		}
		return client.CreateDmAsync(userId);
	}

	internal static async Task SendPrivateCommandResponseAsync(CommandContext context, string text)
	{
		try
		{
			var dm = await Instance.CreatePrivateChannelAsync(context.User.Id);
			foreach (var chunk in DiscordResponseText.Split(DiscordResponseText.NeutraliseMentions(text)))
			{
				await dm.SendMessageAsync(chunk);
			}
		}
		catch (Exception e)
		{
			Log.Warning("Could not deliver private legacy Discord response ({ExceptionType}).", e.GetType().Name);
		}
	}
	private readonly ConcurrentDictionary<Guid, PendingRemoteDiscordRequest> _remoteRequests = new();
	private readonly ConcurrentDictionary<Guid, PendingDiscordConfirmation> _remoteConfirmations = new();
	private readonly object _remoteConfirmationGate = new();
	private static readonly TimeSpan RemoteRequestTimeout = TimeSpan.FromSeconds(45);
	private static readonly TimeSpan RemoteConfirmationLifetime = TimeSpan.FromMinutes(2);
	private const int MaximumPendingRemoteRequests = 256;
	private const int MaximumPendingRemoteConfirmations = 256;

	internal async Task SubmitRemoteCommandAsync(InteractionContext context, string operation, string arguments)
	{
		await context.DeferAsync(true);
		if (DiscordRemoteCommandPolicy.RequiresOwner(operation) && !IsOwner(context.User))
		{
			await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Only a configured Discord bot owner can revoke bot administration."));
			return;
		}
		if (!TryGetAuthenticatedConnection(out var connection) || !connection.SupportsRemoteCommands)
		{
			await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
				$"The game server is unavailable or does not support remote operations yet."));
			return;
		}

		if (_remoteRequests.Count >= MaximumPendingRemoteRequests)
		{
			await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(
				"There are too many outstanding Discord requests. Please try again shortly."));
			return;
		}

		var request = new DiscordRemoteRequest
		{
			DiscordUserId = context.User.Id,
			GuildId = context.Guild?.Id,
			ChannelId = context.Channel.Id,
			Operation = operation,
			Arguments = arguments
		};
		var pending = new PendingRemoteDiscordRequest(request,
			response => RespondToInteractionAsync(context, response, true), DateTime.UtcNow + RemoteRequestTimeout);
		if (!_remoteRequests.TryAdd(request.RequestId, pending))
		{
			await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("The request could not be queued. Please try again."));
			return;
		}

		try
		{
			await connection.SendRemoteCommandAsync(request);
			_ = ExpireRemoteRequestAsync(request.RequestId, RemoteRequestTimeout);
		}
		catch (Exception e)
		{
			_remoteRequests.TryRemove(request.RequestId, out _);
			Log.Warning("Could not send Discord operation {Operation} ({ExceptionType}).", operation, e.GetType().Name);
			await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("The game connection closed before the request could be sent."));
		}
	}

	internal async Task HandleRemoteResponseAsync(DiscordRemoteResponse response)
	{
		if (!_remoteRequests.TryRemove(response.RequestId, out var pending))
		{
			Log.Warning("Received an unknown or expired Discord operation response {RequestId}.", response.RequestId);
			return;
		}

		if (response.Status.Equals("confirmation", StringComparison.OrdinalIgnoreCase) && response.ConfirmationId.HasValue)
		{
			var confirmation = new PendingDiscordConfirmation(pending, response.ConfirmationId.Value);
			bool added;
			lock (_remoteConfirmationGate)
			{
				added = _remoteConfirmations.Count < MaximumPendingRemoteConfirmations &&
				        _remoteConfirmations.TryAdd(response.ConfirmationId.Value, confirmation);
			}

			if (!added)
			{
				await pending.RespondAsync(new DiscordRemoteResponse
				{
					RequestId = pending.Request.RequestId,
					Status = "expired",
					Text = "Too many confirmations are pending or the challenge was duplicated. This confirmation was not retained; inspect operation history before retrying."
				});
				return;
			}
			_ = ExpireRemoteConfirmationAsync(response.ConfirmationId.Value);
			await pending.RespondAsync(response);
			return;
		}

		var persistenceWarning = await HandleCompletedLocalChangesAsync(pending.Request, response);
		if (persistenceWarning is not null)
		{
			response = response with { Text = $"{response.Text}\n\nPARTIAL COMPLETION: {persistenceWarning} Do not retry the engine operation." };
		}
		await pending.RespondAsync(response);
	}

	private async Task<string?> HandleCompletedLocalChangesAsync(DiscordRemoteRequest request, DiscordRemoteResponse response)
	{
		if (!response.Status.Equals("completed", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		try
		{
			if (request.Operation.Equals("account.unlink", StringComparison.OrdinalIgnoreCase))
			{
				return await RemoveAccountLinkAsync(request.DiscordUserId)
					? null
					: "The engine unlink completed, but the bot could not persist its local link mirror. The engine link is authoritative; contact a bot owner before using legacy bot commands.";
			}
			else if (DiscordRemoteCommandPolicy.RequiresOwner(request.Operation))
			{
				if (!DiscordRemoteCommandPolicy.TryGetDeauthorisedDiscordUserId(request.Arguments, out var parsedUserId))
				{
					return "The engine revocation completed, but the bot could not read the target Discord ID to update its local settings.";
				}
				if (parsedUserId == request.DiscordUserId)
				{
					return "The engine revocation completed, but the bot refused to remove the requesting owner from its local settings.";
				}
				return await RemoveAuthorisedUserAsync(parsedUserId)
					? null
					: "The engine revocation completed, but the bot could not persist the local administrator removal. The account may still appear authorised to legacy bot commands until settings are repaired.";
			}
			return null;
		}
		catch (Exception e)
		{
			Log.Warning("Could not update local Discord settings after {Operation} ({ExceptionType}).",
				request.Operation, e.GetType().Name);
			return "The engine action completed, but the bot failed to persist its local mirror. Contact a bot owner; do not retry the engine operation.";
		}
	}

	internal async Task HandleBridgeDisconnectedAsync()
	{
		await CachedDiscordRequests.FailAllAsync("The game connection closed before a response arrived. Check the relevant game state before retrying.");
		foreach (var pair in _remoteRequests.ToArray())
		{
			if (_remoteRequests.TryRemove(pair.Key, out var pending))
			{
				try
				{
					await pending.RespondAsync(new DiscordRemoteResponse
					{
						RequestId = pair.Key,
						Status = "indeterminate",
						InvocationId = pending.Request.Operation.StartsWith("hooks.", StringComparison.OrdinalIgnoreCase) ? pair.Key : null,
						Text = $"The game connection closed before a result arrived. Request ID: {pair.Key:D}. Inspect action history before retrying."
					});
				}
				catch (Exception e)
				{
					Log.Warning("Could not report disconnected Discord operation {RequestId} ({ExceptionType}).", pair.Key, e.GetType().Name);
				}
			}
		}

		foreach (var pair in _remoteConfirmations.ToArray())
		{
			if (_remoteConfirmations.TryRemove(pair.Key, out var confirmation))
			{
				try
				{
					await confirmation.Pending.RespondAsync(new DiscordRemoteResponse
					{
						RequestId = confirmation.Pending.Request.RequestId,
						Status = "expired",
						Text = "The game connection closed. This confirmation was cleared; inspect operation history before retrying."
					});
				}
				catch (Exception e)
				{
					Log.Warning("Could not report cleared Discord confirmation {ConfirmationId} ({ExceptionType}).", pair.Key, e.GetType().Name);
				}
			}
		}
	}

	private static async Task RespondToInteractionAsync(InteractionContext context,
		DiscordRemoteResponse response, bool addConfirmation)
	{
		var text = DiscordResponseText.Format(response);
		if (addConfirmation && response.ConfirmationId.HasValue)
		{
			var builder = new DiscordWebhookBuilder()
				.WithContent(DiscordResponseText.Truncate(text, 1800) + "\n\nConfirm this operation within 2 minutes.")
				.AddComponents(new DiscordButtonComponent(ButtonStyle.Success,
					$"mudconfirm:{response.ConfirmationId.Value:N}", "Confirm"));
			await context.EditResponseAsync(builder);
			return;
		}

		var chunks = DiscordResponseText.Split(text);
		await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(chunks[0]));
		foreach (var chunk in chunks.Skip(1))
		{
			await context.FollowUpAsync(new DiscordFollowupMessageBuilder()
				.WithContent(chunk)
				.AsEphemeral(true));
		}
	}

	private static async Task RespondToInteractionAsync(InteractionContext context, string text)
	{
		var chunks = DiscordResponseText.Split(text);
		await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent(chunks[0]));
		foreach (var chunk in chunks.Skip(1))
		{
			await context.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(chunk).AsEphemeral(true));
		}
	}

	private async Task ExpireRemoteRequestAsync(Guid requestId, TimeSpan timeout)
	{
		await Task.Delay(timeout);
		if (_remoteRequests.TryRemove(requestId, out var pending))
		{
			await pending.RespondAsync(new DiscordRemoteResponse
			{
				RequestId = requestId,
				Status = "indeterminate",
				InvocationId = pending.Request.Operation.StartsWith("hooks.", StringComparison.OrdinalIgnoreCase) ? requestId : null,
				Text = $"The game server did not return a result before the request expired. Request ID: {requestId:D}. Check the action history before retrying."
			});
		}
	}

	private async Task ExpireRemoteConfirmationAsync(Guid confirmationId)
	{
		await Task.Delay(RemoteConfirmationLifetime);
		if (_remoteConfirmations.TryRemove(confirmationId, out var pending))
		{
			await pending.Pending.RespondAsync(new DiscordRemoteResponse
			{
				RequestId = pending.Pending.Request.RequestId,
				Status = "expired",
				Text = "This confirmation expired. Inspect the operation state before starting another one."
			});
		}
	}

	internal async Task ConfirmRemoteOperationAsync(ComponentInteractionCreateEventArgs args, Guid confirmationId)
	{
		if (!_remoteConfirmations.TryRemove(confirmationId, out var pending))
		{
			await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
				new DiscordInteractionResponseBuilder().WithContent("This confirmation has expired or was already used.").AsEphemeral(true));
			return;
		}

		if (args.User.Id != pending.Pending.Request.DiscordUserId)
		{
			_remoteConfirmations.TryAdd(confirmationId, pending);
			await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
				new DiscordInteractionResponseBuilder().WithContent("Only the person who requested this operation can confirm it.").AsEphemeral(true));
			return;
		}

		if (DiscordRemoteCommandPolicy.RequiresOwner(pending.Pending.Request.Operation) && !IsOwner(args.User))
		{
			await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
				new DiscordInteractionResponseBuilder().WithContent("Only a configured Discord bot owner can confirm this operation.").AsEphemeral(true));
			return;
		}

		if (!TryGetAuthenticatedConnection(out var connection) || !connection.SupportsRemoteCommands)
		{
			await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
				new DiscordInteractionResponseBuilder().WithContent("The game server is unavailable. Check the operation history before retrying.").AsEphemeral(true));
			return;
		}

		await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource,
			new DiscordInteractionResponseBuilder().AsEphemeral(true));
		var request = pending.Pending.Request with { ConfirmationId = confirmationId };
		var confirmed = new PendingRemoteDiscordRequest(request,
			async response =>
			{
				var chunks = DiscordResponseText.Split(DiscordResponseText.Format(response));
				await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(chunks[0]));
				foreach (var chunk in chunks.Skip(1))
				{
					await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
						.WithContent(chunk)
						.AsEphemeral(true));
				}
			}, DateTime.UtcNow + RemoteRequestTimeout);
		if (!_remoteRequests.TryAdd(request.RequestId, confirmed))
		{
			await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(
				"This operation is already waiting for a result. Check its history before retrying."));
			return;
		}

		try
		{
			await connection.SendRemoteCommandAsync(request);
			_ = ExpireRemoteRequestAsync(request.RequestId, RemoteRequestTimeout);
		}
		catch (Exception e)
		{
			_remoteRequests.TryRemove(request.RequestId, out _);
			Log.Warning("Could not send confirmation ({ExceptionType}).", e.GetType().Name);
			await args.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(
				"The game connection closed while confirming. Check the operation history before retrying."));
		}
	}

}

internal static class DiscordResponseText
{
	public static string Truncate(string value, int maximumLength)
	{
		value = NeutraliseMentions(value);
		if (value.Length <= maximumLength)
		{
			return value;
		}

		var length = Math.Max(0, maximumLength - 1);
		if (length > 0 && char.IsHighSurrogate(value[length - 1]) && char.IsLowSurrogate(value[length]))
		{
			length--;
		}
		return value[..length].TrimEnd() + "…";
	}

	public static string Format(DiscordRemoteResponse response)
	{
		var text = string.IsNullOrWhiteSpace(response.Text) ? "The game server returned no additional details." : response.Text;
		var status = response.Status.ToLowerInvariant() switch
		{
			"completed" => "Completed",
			"queued" => "Queued",
			"invoking" => "In progress",
			"indeterminate" => "Outcome unknown; inspect history before retrying",
			"denied" => "Not authorised",
			"failed" or "error" => "The operation failed",
			"expired" => "Expired",
			"cancelled" => "Cancelled",
			_ => "Game response"
		};
		return NeutraliseMentions($"**{status}**\n{text}" +
			(response.InvocationId.HasValue ? $"\nInvocation: {response.InvocationId.Value:D}" : ""));
	}

	public static string NeutraliseMentions(string value) => value
		.Replace("@everyone", "@\u200beveryone", StringComparison.OrdinalIgnoreCase)
		.Replace("@here", "@\u200bhere", StringComparison.OrdinalIgnoreCase)
		.Replace("<@", "<@\u200b", StringComparison.Ordinal);

	public static IReadOnlyList<string> Split(string value, int maximumLength = 1850)
	{
		if (value.Length <= maximumLength)
		{
			return [value];
		}

		var parts = new List<string>();
		while (value.Length > maximumLength)
		{
			var length = maximumLength;
			if (char.IsHighSurrogate(value[length - 1]) && char.IsLowSurrogate(value[length]))
			{
				length--;
			}
			var breakAt = value.LastIndexOf('\n', length - 1, length);
			if (breakAt > maximumLength / 2)
			{
				length = breakAt;
			}
			parts.Add(value[..length]);
			value = value[length..].TrimStart('\r', '\n');
		}
		if (value.Length > 0)
		{
			parts.Add(value);
		}
		return parts;
	}
}

internal static class DiscordRemoteCommandPolicy
{
	public static bool IsOwner(ulong userId, IReadOnlyCollection<ulong> ownerUserIds, IReadOnlyCollection<ulong> authorisedUserIds) =>
		ownerUserIds.Contains(userId) ||
		ownerUserIds.Count == 0 && authorisedUserIds.FirstOrDefault() == userId;

	public static bool RequiresOwner(string operation) =>
		operation.Equals("admin.deauthorise", StringComparison.OrdinalIgnoreCase);

	public static bool TryGetDeauthorisedDiscordUserId(string arguments, out ulong userId)
	{
		userId = 0;
		try
		{
			using var document = JsonDocument.Parse(arguments);
			var root = document.RootElement;
			if (!root.TryGetProperty("user", out var value) && !root.TryGetProperty("discordUserId", out value))
			{
				return false;
			}

			var id = value.ValueKind == JsonValueKind.String ? value.GetString() : value.GetRawText();
			return ulong.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out userId);
		}
		catch (Exception e) when (e is System.Text.Json.JsonException or InvalidOperationException)
		{
			return false;
		}
	}
}

internal static class RemoteCommandComponents
{
	public static async Task HandleAsync(DiscordClient sender, ComponentInteractionCreateEventArgs args)
	{
		const string prefix = "mudconfirm:";
		if (!args.Id.StartsWith(prefix, StringComparison.Ordinal) ||
			!Guid.TryParseExact(args.Id[prefix.Length..], "N", out var confirmationId))
		{
			return;
		}

		await DiscordBot.Instance.ConfirmRemoteOperationAsync(args, confirmationId);
	}
}
