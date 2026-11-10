#nullable enable

using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using MudSharp.Accounts;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp.Discord;

internal sealed partial class DiscordRemoteService
{
	private static readonly ProgVariableTypes[] HookParameters =
		[ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text];

	private DiscordHook FindHook(string key) => _store.Hooks().FirstOrDefault(x =>
		x.Key.Equals(key, StringComparison.OrdinalIgnoreCase) ||
		x.Id.ToString(CultureInfo.InvariantCulture) == key) ??
		throw new ArgumentException("There is no Discord hook by that name.");

	private bool CanSeeHook(DiscordHook hook, IAccount account, DiscordRemoteRequest request)
	{
		if (account.Authority.Level >= PermissionLevel.SeniorAdmin)
		{
			// Maintainers must be able to inspect and disable broken/restricted definitions.
			return true;
		}
		return hook.MinimumAuthority is >= (int)PermissionLevel.Player and <= (int)PermissionLevel.Founder &&
		       (int)account.Authority.Level >= hook.MinimumAuthority &&
		       Allowed(hook.AllowedAccountIds, (ulong)account.Id) &&
		       Allowed(hook.AllowedGuildIds, request.GuildId ?? 0) && Allowed(hook.AllowedChannelIds, request.ChannelId);
	}

	private void ValidateRunnableHook(DiscordHook hook)
	{
		if (!hook.Enabled)
		{
			throw new ArgumentException("That Discord hook is disabled.");
		}
		ValidateSignature(hook);
	}

	private void ValidateSignature(DiscordHook hook)
	{
		var problem = HookProblem(hook);
		if (problem is not null)
		{
			throw new ArgumentException(problem);
		}
	}

	private string? HookProblem(DiscordHook hook)
	{
		var prog = _world.FutureProgs.Get(hook.ProgId);
		if (prog is null)
		{
			return "The referenced prog no longer exists.";
		}
		if (!string.IsNullOrEmpty(prog.CompileError))
		{
			return "The referenced prog has compilation errors.";
		}
		if (prog.AcceptsAnyParameters || prog.ReturnType != ProgVariableTypes.Text ||
		    !prog.Parameters.SequenceEqual(HookParameters))
		{
			return "Discord hooks require Text return and parameters (Number accountId, Text discordUserId, Text invocationId, Text arguments).";
		}
		if (hook.MinimumAuthority is < (int)PermissionLevel.Player or > (int)PermissionLevel.Founder ||
		    hook.CooldownSeconds is < 0 or > 86400 || !ValidIdList(hook.AllowedAccountIds) ||
		    !ValidIdList(hook.AllowedGuildIds) || !ValidIdList(hook.AllowedChannelIds))
		{
			return "The hook has invalid permission, context or cooldown configuration.";
		}
		return null;
	}

	private string HookFingerprint(DiscordHook hook)
	{
		var prog = _world.FutureProgs.Get(hook.ProgId);
		return Hash(JsonSerializer.Serialize(new
		{
			hook.Id, hook.Key, hook.Revision, hook.ProgId, hook.Enabled, hook.MinimumAuthority,
			hook.RequireConfirmation, hook.CooldownSeconds, hook.AllowedAccountIds, hook.AllowedGuildIds,
			hook.AllowedChannelIds, Source = prog?.FunctionText, Return = prog?.ReturnType.ToString(),
			Parameters = prog?.Parameters.Select(x => x.ToString()).ToArray(), prog?.AcceptsAnyParameters
		}));
	}

	internal DiscordHook CreateHook(string key, long progId)
	{
		if (!Regex.IsMatch(key, "^[a-z][a-z0-9_-]{1,63}$", RegexOptions.CultureInvariant) ||
		    _store.Hooks().Any(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
		{
			throw new ArgumentException("Choose a unique 2–64 character lowercase key using letters, digits, - or _.");
		}
		var hook = new DiscordHook { Key = key, ProgId = progId };
		ValidateSignature(hook);
		_store.SaveHook(hook);
		return hook;
	}

	internal string EditHook(string key, string field, string value)
	{
		var hook = FindHook(key);
		field = field.ToLowerInvariant();
		switch (field)
		{
			case "description":
				if (value.Length > 2000) { throw new ArgumentException("Description is limited to 2,000 characters."); }
				hook.Description = value;
				break;
			case "category":
				if (value.Length is < 1 or > 100) { throw new ArgumentException("Category must be 1–100 characters."); }
				hook.Category = value;
				break;
			case "usage":
				if (value.Length > 1000) { throw new ArgumentException("Usage is limited to 1,000 characters."); }
				hook.Usage = value;
				break;
			case "prog":
				var prog = _world.FutureProgs.GetByIdOrName(value) ??
				           throw new ArgumentException("No such prog.");
				hook.ProgId = prog.Id;
				break;
			case "authority":
				if (!Enum.TryParse<PermissionLevel>(value, true, out var authority) ||
				    authority is < PermissionLevel.Player or > PermissionLevel.Founder)
				{
					throw new ArgumentException("Choose Player, Guide, JuniorAdmin, Admin, SeniorAdmin, HighAdmin or Founder.");
				}
				hook.MinimumAuthority = (int)authority;
				break;
			case "enabled":
				hook.Enabled = ParseToggle(value);
				break;
			case "confirm":
				hook.RequireConfirmation = ParseToggle(value);
				break;
			case "cooldown":
				value = value.EndsWith('s') ? value[..^1] : value;
				if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var seconds) ||
				    seconds is < 0 or > 86400)
				{
					throw new ArgumentException("Cooldown must be 0–86400 seconds, optionally followed by s.");
				}
				hook.CooldownSeconds = seconds;
				break;
			case "accounts":
			case "guilds":
			case "channels":
				if (!ValidIdList(value)) { throw new ArgumentException("Supply space/comma-separated IDs, or none."); }
				if (field == "accounts") { hook.AllowedAccountIds = value; }
				else if (field == "guilds") { hook.AllowedGuildIds = value; }
				else { hook.AllowedChannelIds = value; }
				break;
			default:
				throw new ArgumentException("Fields: description, category, usage, prog, authority, enabled, confirm, cooldown, accounts, guilds, channels.");
		}
		// A disabled broken definition remains editable so it can be repaired or shut down.
		if (hook.Enabled)
		{
			ValidateSignature(hook);
		}
		hook.Revision++;
		_store.SaveHook(hook);
		return $"Updated Discord hook {hook.Key}, revision {hook.Revision}. Validation: {HookProblem(hook) ?? "ready"}.";
	}

	private static bool ParseToggle(string value) => value.ToLowerInvariant() switch
	{
		"on" or "true" => true,
		"off" or "false" => false,
		_ => throw new ArgumentException("Specify on or off.")
	};
}
