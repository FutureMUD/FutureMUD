#nullable enable

using System.Globalization;
using System.IO;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Database;

namespace MudSharp.Discord;

public sealed partial class DiscordConnection
{
	internal static bool TryWriteBridgeFrame(Stream stream, byte[] bytes)
	{
		try
		{
			stream.Write(bytes, 0, bytes.Length);
			return true;
		}
		catch (Exception error) when (error is IOException or SocketException or ObjectDisposedException)
		{
			return false;
		}
	}

	private void HandleRemoteCommand(StringStack stack)
	{
		DiscordRemoteRequest? request = null;
		try
		{
			request = DiscordRemoteWire.DeserializeRequest(stack.SafeRemainingArgument);
			SendClientMessage($"remote {DiscordRemoteWire.SerializeResponse(Remote.Process(request))}");
		}
		catch (Exception error) when (error is not SocketException and not IOException)
		{
			// Report a stable error to the caller without leaking SQL/configuration/exception payloads.
			if (request is not null)
			{
				SendClientMessage($"remote {DiscordRemoteWire.SerializeResponse(new DiscordRemoteResponse
				{
					RequestId = request.RequestId, InvocationId = request.RequestId, Status = "error",
					Text = "The Discord operation could not be completed. Inspect its recorded outcome before retrying a mutation."
				})}");
			}
			Console.WriteLine($"Discord remote request failed ({error.GetType().Name}).");
		}
	}

	private IAccount? VerifiedLegacyAccount(long accountId, PermissionLevel authority)
	{
		using (new FMDB())
		{
			if (!FMDB.Context.DiscordAccountLinks.Any(x => x.AccountId == accountId))
			{
				return null;
			}
			var row = FMDB.Context.Accounts.Find(accountId);
			var account = row is null ? null : Gameworld.TryAccount(row);
			return account?.AccountStatus == AccountStatus.Normal && account.Authority.Level >= authority ? account : null;
		}
	}

	private void NotifyLinkedAccount(string accountName, string message)
	{
		if (!_authenticated) { return; }
		try
		{
			using (new FMDB())
			{
				var discordId = FMDB.Context.DiscordAccountLinks.AsNoTracking()
					.Where(x => x.NotificationsEnabled && x.Account.Name == accountName &&
					            x.Account.AccessStatus == (int)AccountStatus.Normal)
					.Select(x => x.DiscordUserId).FirstOrDefault();
				if (discordId is null) { return; }
				var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
				SendClientMessage($"accountnotification {discordId} {payload}");
			}
		}
		catch (Exception error)
		{
			Console.WriteLine($"Optional Discord account notification failed ({error.GetType().Name}).");
		}
	}

	public void NotifyAccountMessage(string account, string message) => NotifyLinkedAccount(account, message);
}
