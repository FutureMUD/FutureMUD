#nullable enable

using MudSharp.Accounts;

namespace MudSharp.Discord;

internal static class DiscordRemotePolicy
{
	internal static readonly IReadOnlyDictionary<string, PermissionLevel> Operations =
		new Dictionary<string, PermissionLevel>(StringComparer.Ordinal)
		{
			["game.info"] = PermissionLevel.Guest, ["game.helpsearch"] = PermissionLevel.Guest,
			["game.time"] = PermissionLevel.Guest, ["game.calendar"] = PermissionLevel.Guest,
			["game.weather"] = PermissionLevel.Guest, ["game.chargenguide"] = PermissionLevel.Guest,
			["account.linkstatus"] = PermissionLevel.Player, ["account.unlink"] = PermissionLevel.Player,
			["account.resources"] = PermissionLevel.Player, ["account.applications"] = PermissionLevel.Player,
			["account.options"] = PermissionLevel.Player, ["account.notifications"] = PermissionLevel.Player,
			["admin.applications"] = PermissionLevel.Guide,
			["admin.accountsearch"] = PermissionLevel.JuniorAdmin, ["admin.accountnotes"] = PermissionLevel.JuniorAdmin,
			["admin.addnote"] = PermissionLevel.JuniorAdmin, ["admin.resources"] = PermissionLevel.JuniorAdmin,
			["admin.ban"] = PermissionLevel.SeniorAdmin, ["admin.unban"] = PermissionLevel.SeniorAdmin,
			["admin.maintenance"] = PermissionLevel.SeniorAdmin, ["admin.connections"] = PermissionLevel.HighAdmin,
			["admin.performance"] = PermissionLevel.SeniorAdmin, ["admin.census"] = PermissionLevel.SeniorAdmin,
			["admin.actionlog"] = PermissionLevel.SeniorAdmin, ["admin.deauthorise"] = PermissionLevel.Founder,
			["admin.resourceaudit"] = PermissionLevel.Admin,
			["economy.summary"] = PermissionLevel.Admin, ["economy.money"] = PermissionLevel.Admin,
			["economy.holdings"] = PermissionLevel.Admin, ["economy.volume"] = PermissionLevel.Admin,
			["economy.income"] = PermissionLevel.Admin, ["economy.employment"] = PermissionLevel.Admin,
			["economy.trends"] = PermissionLevel.Admin, ["economy.wealth"] = PermissionLevel.Admin,
			["economy.risks"] = PermissionLevel.Admin, ["economy.snapshotstatus"] = PermissionLevel.Admin,
			["economy.snapshot"] = PermissionLevel.Admin, ["economy.configure"] = PermissionLevel.HighAdmin,
			["hooks.list"] = PermissionLevel.Player, ["hooks.show"] = PermissionLevel.Player,
			["hooks.run"] = PermissionLevel.Player, ["hooks.history"] = PermissionLevel.Player,
			["hooks.result"] = PermissionLevel.Player, ["hooks.enabled"] = PermissionLevel.SeniorAdmin
		};

	internal static bool CanUse(string operation, IAccount? account)
	{
		return Operations.TryGetValue(operation, out var required) &&
		       (required == PermissionLevel.Guest ||
		        account is { AccountStatus: AccountStatus.Normal } && account.Authority?.Level >= required);
	}

	internal static bool RequiresConfirmation(string operation, string arguments) => operation is
		"account.unlink" or "admin.addnote" or "admin.resources" or "admin.ban" or "admin.unban" or
		"admin.deauthorise" or "economy.snapshot" or "economy.configure" or "hooks.enabled" ||
		operation == "admin.maintenance" && arguments != "{}" && !string.IsNullOrWhiteSpace(arguments);
}
