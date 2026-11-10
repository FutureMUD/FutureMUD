#nullable enable

using DSharpPlus.SlashCommands;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord_Bot.Modules;

internal static class RemoteDiscordCommands
{
	private static readonly JsonSerializerOptions PayloadOptions = new(JsonSerializerDefaults.Web);

	public static Task SubmitAsync(InteractionContext context, string operation,
		params (string Name, object? Value)[] values)
	{
		var arguments = values.Length == 0
			? "{}"
			: JsonSerializer.Serialize(values.Where(x => x.Value is not null)
				.ToDictionary(x => x.Name, x => x.Value), PayloadOptions);
		return DiscordBot.Instance.SubmitRemoteCommandAsync(context, operation, arguments);
	}
}

[SlashCommandGroup("game", "Public information about the game")]
public sealed class GameCommands : ApplicationCommandModule
{
	[SlashCommand("info", "Game description, links, connection information and maintenance notice")]
	public Task InfoAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "game.info");

	[SlashCommand("helpsearch", "Search public helpfiles and command help")]
	public Task HelpSearchAsync(InteractionContext context, [Option("query", "Words to search for")] string query) =>
		RemoteDiscordCommands.SubmitAsync(context, "game.helpsearch", ("query", query));

	[SlashCommand("time", "Show current game time at a published place")]
	public Task TimeAsync(InteractionContext context, [Option("place", "Published clock or place")] string place) =>
		RemoteDiscordCommands.SubmitAsync(context, "game.time", ("place", place));

	[SlashCommand("calendar", "Show a published game calendar")]
	public Task CalendarAsync(InteractionContext context, [Option("calendar", "Calendar name")] string calendar,
		[Option("year", "Optional game year")] long? year = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "game.calendar", ("calendar", calendar), ("year", year));

	[SlashCommand("weather", "Show public conditions for a published weather region")]
	public Task WeatherAsync(InteractionContext context, [Option("region", "Published weather region")] string region) =>
		RemoteDiscordCommands.SubmitAsync(context, "game.weather", ("region", region));

	[SlashCommand("chargenguide", "Show published character creation guidance")]
	public Task ChargenGuideAsync(InteractionContext context,
		[Option("topic", "Optional guidance topic")] string? topic = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "game.chargenguide", ("topic", topic));
}

[SlashCommandGroup("account", "Private account self-service")]
public sealed class AccountCommands : ApplicationCommandModule
{
	[SlashCommand("linkstatus", "Show the MUD account currently linked to you")]
	public Task LinkStatusAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "account.linkstatus");

	[SlashCommand("unlink", "Remove your Discord to MUD account link")]
	public Task UnlinkAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "account.unlink");

	[SlashCommand("resources", "Show your account chargen resources")]
	public Task ResourcesAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "account.resources");

	[SlashCommand("applications", "Show your character applications and their status")]
	public Task ApplicationsAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "account.applications");

	[SlashCommand("options", "Show the current stage of one of your chargen drafts")]
	public Task OptionsAsync(InteractionContext context, [Option("draftid", "Your draft application ID")] long draftId) =>
		RemoteDiscordCommands.SubmitAsync(context, "account.options", ("draftId", draftId));

	[SlashCommand("notifications", "Show or change your private game notification settings")]
	public Task NotificationsAsync(InteractionContext context,
		[Option("enabled", "Whether notifications should be enabled")] bool? enabled = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "account.notifications", ("enabled", enabled));
}

[SlashCommandGroup("admin", "Private staff reports and administration")]
public sealed class AdminCommands : ApplicationCommandModule
{
	[SlashCommand("applications", "List submitted applications awaiting review")]
	public Task ApplicationsAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "admin.applications");

	[SlashCommand("accountsearch", "Find an account by name or ID")]
	public Task AccountSearchAsync(InteractionContext context, [Option("query", "Account name or ID")] string query) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.accountsearch", ("query", query));

	[SlashCommand("accountnotes", "Read staff notes for an account")]
	public Task AccountNotesAsync(InteractionContext context,
		[Option("account", "Exact account name or ID")] string account) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.accountnotes", ("account", account));

	[SlashCommand("addnote", "Add a reasoned staff note to an account")]
	public Task AddNoteAsync(InteractionContext context, [Option("account", "Exact account name or ID")] string account,
		[Option("subject", "Short note subject")] string subject,
		[Option("text", "Note contents")] string text) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.addnote", ("account", account), ("subject", subject), ("text", text));

	[SlashCommand("resources", "Award or deduct chargen resources with an audit reason")]
	public Task ResourcesAsync(InteractionContext context, [Option("account", "Exact account name or ID")] string account,
		[Option("resource", "Resource name")] string resource,
		[Option("amount", "Signed whole amount to change")] long amount,
		[Option("reason", "Reason for the change")] string reason) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.resources", ("account", account), ("resource", resource),
			("amount", amount), ("reason", reason));

	[SlashCommand("ban", "Suspend an account with a required reason")]
	public Task BanAsync(InteractionContext context, [Option("account", "Exact account name or ID")] string account,
		[Option("reason", "Reason for suspension")] string reason) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.ban", ("account", account), ("reason", reason));

	[SlashCommand("unban", "Lift an account suspension with a required reason")]
	public Task UnbanAsync(InteractionContext context, [Option("account", "Exact account name or ID")] string account,
		[Option("reason", "Reason for lifting suspension")] string reason) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.unban", ("account", account), ("reason", reason));

	[SlashCommand("maintenance", "Inspect or set login and chargen maintenance restrictions")]
	public Task MaintenanceAsync(InteractionContext context,
		[Option("setting", "login, accountlogin or chargen")] string? setting = null,
		[Option("enabled", "Set the selected restriction on or off")] bool? enabled = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.maintenance", ("setting", setting),
			("value", enabled?.ToString().ToLowerInvariant()));

	[SlashCommand("connections", "Show connected users and session states")]
	public Task ConnectionsAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "admin.connections");

	[SlashCommand("performance", "Show bounded runtime performance counters")]
	public Task PerformanceAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "admin.performance");

	[SlashCommand("census", "Show bounded runtime actor and schedule counts")]
	public Task CensusAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "admin.census");

	[SlashCommand("actionlog", "Search attributed Discord operation results")]
	public Task ActionLogAsync(InteractionContext context,
		[Option("operation", "Optional operation name")] string? operation = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.actionlog", ("operation", operation));

	[SlashCommand("deauthorise", "Revoke Discord bot administrator access")]
	public Task DeauthoriseAsync(InteractionContext context,
		[Option("userid", "Discord user ID")] string discordUserId,
		[Option("reason", "Reason for revocation")] string reason) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.deauthorise", ("user", discordUserId), ("reason", reason));

	[SlashCommand("resourceaudit", "Audit account chargen-resource balances")]
	public Task ResourceAuditAsync(InteractionContext context,
		[Option("resource", "Optional resource name")] string? resource = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "admin.resourceaudit", ("resource", resource));
}

[SlashCommandGroup("economy", "Private economy reports and controls")]
public sealed class EconomyCommands : ApplicationCommandModule
{
	[SlashCommand("summary", "Summarise money supply, reserves, activity and warnings")]
	public Task SummaryAsync(InteractionContext context, [Option("zoneid", "Optional economic zone ID")] long? zoneId = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.summary", ("zoneId", zoneId));

	[SlashCommand("money", "Show currency totals and control buckets")]
	public Task MoneyAsync(InteractionContext context, [Option("zoneid", "Optional economic zone ID")] long? zoneId = null,
		[Option("currencyid", "Optional currency ID")] long? currencyId = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.money", ("zoneId", zoneId), ("currencyId", currencyId));

	[SlashCommand("holdings", "Show detailed current economic custody")]
	public Task HoldingsAsync(InteractionContext context, [Option("zoneid", "Optional economic zone ID")] long? zoneId = null,
		[Option("currencyid", "Optional currency ID")] long? currencyId = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.holdings", ("zoneId", zoneId), ("currencyId", currencyId));

	[SlashCommand("volume", "Compare exchange and movement across a real or game-time window")]
	public Task VolumeAsync(InteractionContext context,
		[Option("window", "day, week, month, mudday, mudweek, mudmonth or period")] string window,
		[Option("zoneid", "Optional economic zone ID")] long? zoneId = null,
		[Option("currencyid", "Optional currency ID")] long? currencyId = null,
		[Option("periodid", "Optional financial period ID")] long? periodId = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.volume", ("window", window), ("zoneId", zoneId),
			("currencyId", currencyId), ("periodId", periodId));

	[SlashCommand("income", "Show settled ordinary player income")]
	public Task IncomeAsync(InteractionContext context,
		[Option("window", "day, week, month, mudday, mudweek, mudmonth or period")] string window,
		[Option("zoneid", "Optional economic zone ID")] long? zoneId = null,
		[Option("currencyid", "Optional currency ID")] long? currencyId = null,
		[Option("periodid", "Optional financial period ID")] long? periodId = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.income", ("window", window), ("zoneId", zoneId),
			("currencyId", currencyId), ("periodId", periodId));

	[SlashCommand("employment", "Show current jobs, openings and worker capability")]
	public Task EmploymentAsync(InteractionContext context, [Option("zoneid", "Optional economic zone ID")] long? zoneId = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.employment", ("zoneId", zoneId));

	[SlashCommand("trends", "Show a bounded economy trend series")]
	public Task TrendsAsync(InteractionContext context, [Option("metric", "Trend metric")] string metric,
		[Option("classification", "Optional volume classification")] string? classification = null,
		[Option("zoneid", "Optional economic zone ID")] long? zoneId = null,
		[Option("currencyid", "Optional currency ID")] long? currencyId = null,
		[Option("count", "Number of points, 1 to 100")] long? count = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.trends", ("metric", metric), ("classification", classification),
			("zoneId", zoneId), ("currencyId", currencyId), ("count", count));

	[SlashCommand("wealth", "Show a bounded PC-controlled wealth ranking")]
	public Task WealthAsync(InteractionContext context, [Option("zoneid", "Optional economic zone ID")] long? zoneId = null,
		[Option("currencyid", "Optional currency ID")] long? currencyId = null,
		[Option("count", "Number of results, 1 to 100")] long? count = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.wealth", ("zoneId", zoneId), ("currencyId", currencyId), ("count", count));

	[SlashCommand("risks", "Show current economy risk signals")]
	public Task RisksAsync(InteractionContext context, [Option("zoneid", "Optional economic zone ID")] long? zoneId = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.risks", ("zoneId", zoneId));

	[SlashCommand("snapshotstatus", "Show economy snapshot schedule and history coverage")]
	public Task SnapshotStatusAsync(InteractionContext context) => RemoteDiscordCommands.SubmitAsync(context, "economy.snapshotstatus");

	[SlashCommand("snapshot", "Capture current economy state")]
	public Task SnapshotAsync(InteractionContext context, [Option("zoneid", "Optional economic zone ID")] long? zoneId = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.snapshot", ("zoneId", zoneId));

	[SlashCommand("configure", "Configure economy snapshot behavior")]
	public Task ConfigureAsync(InteractionContext context, [Option("setting", "snapshots, interval, rollover or currency")] string setting,
		[Option("value", "New validated setting value")] string value) =>
		RemoteDiscordCommands.SubmitAsync(context, "economy.configure", ("setting", setting), ("value", value));
}

[SlashCommandGroup("hooks", "Discover and execute approved game actions")]
public sealed class DiscordHookCommands : ApplicationCommandModule
{
	[SlashCommand("list", "List hooks available to you")]
	public Task ListAsync(InteractionContext context, [Option("category", "Optional hook category")] string? category = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "hooks.list", ("category", category));

	[SlashCommand("show", "Inspect a hook's purpose, access and usage")]
	public Task ShowAsync(InteractionContext context, [Option("hook", "Hook name or ID")] string hook) =>
		RemoteDiscordCommands.SubmitAsync(context, "hooks.show", ("hook", hook));

	[SlashCommand("run", "Run an approved game action")]
	public Task RunAsync(InteractionContext context, [Option("hook", "Hook name or ID")] string hook,
		[Option("arguments", "Validated action arguments")] string arguments = "") =>
		RemoteDiscordCommands.SubmitAsync(context, "hooks.run", ("hook", hook), ("arguments", arguments));

	[SlashCommand("history", "Show permitted recent hook invocations")]
	public Task HistoryAsync(InteractionContext context, [Option("hook", "Optional hook name or ID")] string? hook = null) =>
		RemoteDiscordCommands.SubmitAsync(context, "hooks.history", ("hook", hook));

	[SlashCommand("result", "Retrieve a durable hook result")]
	public Task ResultAsync(InteractionContext context, [Option("invocation", "Invocation ID")] string invocation) =>
		RemoteDiscordCommands.SubmitAsync(context, "hooks.result", ("invocation", invocation));

	[SlashCommand("enabled", "Enable or disable a hook")]
	public Task EnabledAsync(InteractionContext context, [Option("hook", "Hook name or ID")] string hook,
		[Option("enabled", "Whether the hook should be enabled")] bool enabled) =>
		RemoteDiscordCommands.SubmitAsync(context, "hooks.enabled", ("hook", hook), ("enabled", enabled));
}
