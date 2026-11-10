#nullable enable

using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using MudSharp.Accounts;
using MudSharp.Database;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp.Discord;

internal sealed partial class DiscordRemoteService
{
	private readonly IFuturemud _world;
	private readonly IDiscordRemoteStore _store;
	private readonly Func<DateTime> _utcNow;
	private readonly Func<long, IAccount?> _resolveAccount;
	private readonly Dictionary<ulong, (DateTime Window, int Count)> _rateLimits = [];
	private bool _recovered;

	internal DiscordRemoteService(IFuturemud world, IDiscordRemoteStore? store = null,
		Func<DateTime>? utcNow = null, Func<long, IAccount?>? resolveAccount = null)
	{
		_world = world;
		_store = store ?? new DiscordRemoteStore();
		_utcNow = utcNow ?? (() => DateTime.UtcNow);
		_resolveAccount = resolveAccount ?? (id =>
		{
			using (new FMDB())
			{
				var row = FMDB.Context.Accounts.Find(id);
				return row is null ? null : world.TryAccount(row);
			}
		});
	}

	private string? GetPublishedSetting(string key) => _store.GetSetting(key);
	internal IReadOnlyList<DiscordHook> HookDefinitions => _store.Hooks();
	internal string? PublishedSetting(string key) => _store.GetSetting(key);

	internal void SetPublishedSetting(string key, string value)
	{
		if (key is not ("game.info" or "game.places" or "game.calendars" or "game.chargenguide" or
		    "security.guilds" or "security.channels"))
		{
			throw new ArgumentException("Unknown published Discord setting.");
		}
		if (value.Length > 12000)
		{
			throw new ArgumentException("Published values cannot exceed 12,000 characters.");
		}
		if (key is "game.places" or "game.calendars")
		{
			var mapping = JsonSerializer.Deserialize<Dictionary<string, long>>(value) ??
			              throw new ArgumentException("Supply an alias-to-ID JSON object.");
			if (mapping.Count > 100 || mapping.Any(x => string.IsNullOrWhiteSpace(x.Key) || x.Value <= 0))
			{
				throw new ArgumentException("Publish at most 100 aliases with positive IDs.");
			}
		}
		if (key == "game.chargenguide")
		{
			_ = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ??
			    throw new ArgumentException("Supply a topic-to-public-helpfile JSON object.");
		}
		if (key.StartsWith("security.", StringComparison.Ordinal) && !ValidIdList(value))
		{
			throw new ArgumentException("Supply space/comma-separated Discord IDs, or none.");
		}
		_store.SetSetting(key, value);
	}

	internal void LinkVerifiedAccount(ulong discordUserId, long accountId)
	{
		if (discordUserId == 0 || accountId <= 0)
		{
			throw new ArgumentException("Invalid account binding.");
		}
		var existing = _store.FindLink(discordUserId.ToString(CultureInfo.InvariantCulture));
		if (existing is not null && existing.AccountId != accountId)
		{
			throw new ArgumentException("Unlink the existing Discord binding before linking another account.");
		}
		_store.SaveLink(existing ?? new DiscordAccountLink
		{
			DiscordUserId = discordUserId.ToString(CultureInfo.InvariantCulture),
			AccountId = accountId, CreatedUtc = _utcNow()
		});
	}

	internal DiscordRemoteResponse Process(DiscordRemoteRequest request)
	{
		if (request.RequestId == Guid.Empty || request.DiscordUserId == 0 || request.ChannelId == 0 ||
		    request.Operation is null || request.Arguments is null || request.Arguments.Length > 8000)
		{
			return Response(request, "error", "Invalid Discord request.");
		}
		if (!_recovered)
		{
			_store.Recover();
			_recovered = true;
		}
		var now = _utcNow();
		if (!Admit(request.DiscordUserId, now))
		{
			return Response(request, "denied", "Too many requests. Please wait a minute.");
		}
		if (!Allowed(_store.GetSetting("security.guilds"), request.GuildId ?? 0) ||
		    !Allowed(_store.GetSetting("security.channels"), request.ChannelId))
		{
			return Response(request, "denied", "This Discord context is not permitted.");
		}
		var link = _store.FindLink(request.DiscordUserId.ToString(CultureInfo.InvariantCulture));
		var account = link is null ? null : _resolveAccount(link.AccountId);
		if (!DiscordRemotePolicy.CanUse(request.Operation, account))
		{
			return Response(request, "denied", "A verified account link with current permission is required.");
		}

		try
		{
			using var arguments = JsonDocument.Parse(string.IsNullOrWhiteSpace(request.Arguments) ? "{}" : request.Arguments);
			if (arguments.RootElement.ValueKind != JsonValueKind.Object)
			{
				throw new ArgumentException("Arguments must be a JSON object.");
			}
			var args = arguments.RootElement;
			DiscordHook? hook = null;
			if (request.Operation is "hooks.show" or "hooks.run" or "hooks.enabled")
			{
				hook = FindHook(TextArgument(args, "hook"));
				if (!CanSeeHook(hook, account!, request))
				{
					return Response(request, "denied", "There is no Discord hook available to you by that name.");
				}
				if (request.Operation == "hooks.run")
				{
					if ((int)account!.Authority.Level < hook.MinimumAuthority ||
					    !Allowed(hook.AllowedAccountIds, (ulong)account.Id) ||
					    !Allowed(hook.AllowedGuildIds, request.GuildId ?? 0) ||
					    !Allowed(hook.AllowedChannelIds, request.ChannelId))
					{
						return Response(request, "denied", "You may inspect this definition, but cannot execute it in this context.");
					}
				}
			}
			var fingerprint = Hash($"{request.DiscordUserId}|{request.GuildId}|{request.ChannelId}|{request.Operation}|{request.Arguments}");
			var existing = _store.FindInvocation(request.RequestId.ToString("D"));
			// A duplicate completion is a result lookup, not another attempt to validate/execute its old target state.
			if (existing is not null && existing.Status != "confirmation")
			{
				return existing.Fingerprint == fingerprint && existing.AccountId == account?.Id
					? FromInvocation(request, existing)
					: Response(request, "denied", "Request ID belongs to a different operation or account.");
			}
			if (request.Operation == "hooks.run") { ValidateRunnableHook(hook!); }
			var definitionFingerprint = hook is not null ? HookFingerprint(hook) :
				DiscordRemotePolicy.RequiresConfirmation(request.Operation, request.Arguments) &&
				request.Operation is not ("account.unlink" or "admin.deauthorise")
					? Hash(ReportConfirmationFingerprint(request, account)) : "";
			if (existing is not null)
			{
				if (existing.Fingerprint != fingerprint || existing.AccountId != account?.Id)
				{
					return Response(request, "denied", "Request ID belongs to a different operation or account.");
				}
				if (existing.Status != "confirmation")
				{
					return FromInvocation(request, existing);
				}
				if (existing.ExpiresUtc <= now)
				{
					existing.Status = "expired";
					existing.ResultText = "Confirmation expired. Start a new request.";
					existing.ConfirmationId = null;
					_store.SaveInvocation(existing);
					return FromInvocation(request, existing);
				}
				if (existing.DefinitionFingerprint != definitionFingerprint)
				{
					return Response(request, "denied", "The action, prog or target state changed. Start a new request.");
				}
				if (request.ConfirmationId?.ToString("D") != existing.ConfirmationId)
				{
					return FromInvocation(request, existing);
				}
			}
			else if (request.ConfirmationId is not null)
			{
				return Response(request, "denied", "Unknown confirmation. Start a new request.");
			}
			var invocation = existing ?? new DiscordRemoteInvocation
			{
				Id = request.RequestId.ToString("D"), DiscordUserId = request.DiscordUserId.ToString(CultureInfo.InvariantCulture),
				AccountId = account?.Id, Operation = request.Operation, Arguments = request.Arguments,
				Fingerprint = fingerprint, DefinitionFingerprint = definitionFingerprint, HookId = hook?.Id,
				ProgId = hook?.ProgId, HookRevision = hook?.Revision,
				CreatedUtc = now, UpdatedUtc = now
			};
			if (existing is null && (DiscordRemotePolicy.RequiresConfirmation(request.Operation, request.Arguments) ||
			                        request.Operation == "hooks.run" && hook!.RequireConfirmation))
			{
				invocation.Status = "confirmation";
				invocation.ConfirmationId = Guid.NewGuid().ToString("D");
				invocation.ExpiresUtc = now.AddMinutes(2);
				invocation.ResultText = $"Confirm {request.Operation}. Arguments: {request.Arguments}. " +
				                        "This may change persistent world state. Confirmation expires in two minutes.";
				_store.SaveInvocation(invocation);
				return FromInvocation(request, invocation);
			}

			if (request.Operation == "hooks.run" &&
			    _store.HookInvokedSince(hook!.Id, now.AddSeconds(-hook.CooldownSeconds)))
			{
				return Response(request, "denied", "That hook is cooling down. Please wait before trying again.");
			}
			invocation.Status = "invoking";
			invocation.ConfirmationId = null;
			invocation.UpdatedUtc = now;
			_store.SaveInvocation(invocation);
			try
			{
				var text = ExecuteOperation(request, args, account, link, hook);
				invocation.Status = "completed";
				invocation.ResultText = text.Length > 12000 ? text[..12000] + "\n[Result truncated]" : text;
			}
			catch (ArgumentException error)
			{
				invocation.Status = "failed";
				invocation.ResultText = error.Message;
			}
			catch (Exception error)
			{
				invocation.Status = "failed";
				invocation.ResultText = $"Operation failed ({error.GetType().Name}). Changes may be partial; inspect state before retrying.";
			}
			invocation.UpdatedUtc = _utcNow();
			_store.SaveInvocation(invocation);
			return FromInvocation(request, invocation);
		}
		catch (Exception error) when (error is ArgumentException or JsonException or FormatException)
		{
			return Response(request, "error", error is JsonException ? "Invalid JSON arguments." : error.Message);
		}
	}

	private string ExecuteOperation(DiscordRemoteRequest request, JsonElement args, IAccount? account,
		DiscordAccountLink? link, DiscordHook? hook)
	{
		switch (request.Operation)
		{
			case "account.linkstatus":
				return $"Linked to {account!.Name} (#{account.Id}). Notifications: {link!.NotificationsEnabled}.";
			case "account.unlink":
				_store.RemoveLink(link!.DiscordUserId);
				return "Your Discord account link and pending confirmations have been revoked.";
			case "account.notifications":
				if (args.TryGetProperty("enabled", out var enabled))
				{
					link!.NotificationsEnabled = enabled.GetBoolean();
					_store.SaveLink(link);
				}
				return $"Private account notifications: {link!.NotificationsEnabled}.";
			case "admin.actionlog":
				return string.Join("\n", _store.RecentInvocations().Take(50).Select(x =>
					$"{x.Id} | {x.CreatedUtc:O} | account {x.AccountId} | Discord {x.DiscordUserId} | {x.Operation} | {x.Status}"));
			case "admin.deauthorise":
				var target = TextArgument(args, "user");
				if (!ulong.TryParse(target, out var targetId) || targetId == 0 || targetId == request.DiscordUserId)
				{
					throw new ArgumentException("Choose another Discord user ID to revoke.");
				}
				_store.RemoveLink(target);
				return $"Revoked engine link for Discord user {target}.";
			case "hooks.list":
				var category = OptionalText(args, "category");
				return string.Join("\n", _store.Hooks().Where(x => CanSeeHook(x, account!, request) &&
					(category.Length == 0 || x.Category.Equals(category, StringComparison.OrdinalIgnoreCase)))
					.Take(100).Select(x => $"{x.Key} | {x.Category} | {(x.Enabled ? "enabled" : "disabled")} | {x.Description}"));
			case "hooks.show":
				return $"{hook!.Key} (#{hook.Id}, revision {hook.Revision})\n{hook.Description}\nUsage: {hook.Usage}\n" +
				       $"Authority: {(PermissionLevel)hook.MinimumAuthority}; enabled: {hook.Enabled}; " +
				       $"confirmation: {hook.RequireConfirmation}; cooldown: {hook.CooldownSeconds}s\n" +
				       $"Prog: {hook.ProgId}; validation: {HookProblem(hook) ?? "ready"}.";
			case "hooks.run":
				var prog = _world.FutureProgs.Get(hook!.ProgId)!;
				if (!prog.ExecuteWithStatus(out var result, account!.Id,
					    request.DiscordUserId.ToString(CultureInfo.InvariantCulture), request.RequestId.ToString("D"),
					    OptionalText(args, "arguments")))
				{
					throw new InvalidOperationException("FutureProg execution failed.");
				}
				return result?.ToString() ?? "The hook completed without a text result.";
			case "hooks.enabled":
				if (!args.TryGetProperty("enabled", out var availability))
				{
					throw new ArgumentException("Supply enabled: true or false.");
				}
				hook!.Enabled = availability.GetBoolean();
				if (hook.Enabled)
				{
					ValidateSignature(hook);
				}
				hook.Revision++;
				_store.SaveHook(hook);
				return $"{hook.Key} is now {(hook.Enabled ? "enabled" : "disabled")}.";
			case "hooks.history":
				var filter = OptionalText(args, "hook");
				var visible = _store.Hooks().Where(x => CanSeeHook(x, account!, request))
					.Where(x => filter.Length == 0 || x.Key.Equals(filter, StringComparison.OrdinalIgnoreCase))
					.Select(x => x.Id).ToHashSet();
				return string.Join("\n", _store.RecentInvocations().Where(x => x.HookId is { } id && visible.Contains(id) &&
					(x.AccountId == account!.Id || account!.Authority.Level >= PermissionLevel.SeniorAdmin))
					.Take(50).Select(x => $"{x.Id} | {x.CreatedUtc:O} | {x.Operation} | account {x.AccountId} | {x.Status}"));
			case "hooks.result":
				var idText = OptionalText(args, "invocation");
				if (!Guid.TryParse(idText, out var resultId))
				{
					throw new ArgumentException("Supply a valid invocation ID.");
				}
				var record = _store.FindInvocation(resultId.ToString("D"));
				if (record?.HookId is not { } hookId || record.AccountId != account!.Id &&
				    account!.Authority.Level < PermissionLevel.SeniorAdmin ||
				    record.HookId is { } targetHookId && !_store.Hooks().Any(x =>
					    x.Id == targetHookId && CanSeeHook(x, account!, request)))
				{
					throw new ArgumentException("No permitted hook result exists by that ID.");
				}
				return $"{record.Status}: {record.ResultText}";
			default:
				return ExecuteReport(request, account);
		}
	}

	private bool Admit(ulong user, DateTime now)
	{
		if (_rateLimits.Count >= 4096)
		{
			foreach (var key in _rateLimits.Where(x => now - x.Value.Window >= TimeSpan.FromMinutes(1))
				         .Select(x => x.Key).ToList())
			{
				_rateLimits.Remove(key);
			}
			if (_rateLimits.Count >= 4096 && !_rateLimits.ContainsKey(user))
			{
				return false;
			}
		}
		var limit = _rateLimits.GetValueOrDefault(user);
		if (now - limit.Window >= TimeSpan.FromMinutes(1))
		{
			limit = (now, 0);
		}
		_rateLimits[user] = (limit.Window, limit.Count + 1);
		return limit.Count < 30;
	}

	private static string TextArgument(JsonElement args, string name)
	{
		var text = OptionalText(args, name);
		return string.IsNullOrWhiteSpace(text) ? throw new ArgumentException($"Supply {name}.") : text;
	}

	private static string OptionalText(JsonElement args, string name) =>
		args.TryGetProperty(name, out var value) ? value.ToString() : "";

	private static DiscordRemoteResponse Response(DiscordRemoteRequest request, string status, string text) =>
		new() { RequestId = request.RequestId, Status = status, Text = text };

	private static DiscordRemoteResponse FromInvocation(DiscordRemoteRequest request, DiscordRemoteInvocation invocation) =>
		new()
		{
			RequestId = request.RequestId, Status = invocation.Status, Text = invocation.ResultText,
			InvocationId = Guid.Parse(invocation.Id),
			ConfirmationId = Guid.TryParse(invocation.ConfirmationId, out var confirmation) ? confirmation : null
		};

	private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
	private static string[] Ids(string? list) => (list ?? "").Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);
	private static bool ValidIdList(string list) => list == "none" || Ids(list).All(x => ulong.TryParse(x, out _));
	private static bool Allowed(string? list, ulong value) => string.IsNullOrWhiteSpace(list) || list == "none" ||
		Ids(list).Contains(value.ToString(CultureInfo.InvariantCulture), StringComparer.Ordinal);
}
