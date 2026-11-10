#nullable enable

using MudSharp.Accounts;
using MudSharp.Discord;
using MudSharp.Effects.Concrete;

namespace MudSharp.Commands.Modules;

internal sealed class DiscordModule : Module<ICharacter>
{
	private DiscordModule() : base("Discord")
	{
		IsNecessary = true;
	}

	public static DiscordModule Instance { get; } = new();

	private const string HookHelp = @"Publish character-independent FutureProg actions for linked Discord users.

	#3discordhook list#0 - lists definitions, including disabled/broken actions
	#3discordhook new <key> <prog>#0 - creates a disabled action
	#3discordhook show <key>#0 - shows policy, usage and validation
	#3discordhook edit <key>#0 - selects the hook for set
	#3discordhook set <field> <value>#0 - edits the selected hook
	#3discordhook set <key> <field> <value>#0 - edits a named hook directly

Fields: description, category, usage, prog, authority, enabled, confirm, cooldown (seconds), accounts, guilds, channels.
Use #3none#0 to clear account/guild/channel restrictions. Progs must return Text and take Number accountId, Text discordUserId, Text invocationId, Text arguments.
Only publish trusted bounded progs. Confirmation/timeouts cannot roll back or cancel a running prog.";

	[PlayerCommand("DiscordHook", "discordhook")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("discordhook", HookHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void DiscordHook(ICharacter actor, string input)
	{
		if (actor.Gameworld.DiscordConnection is not DiscordConnection connection)
		{
			actor.Send("The Discord integration is unavailable.".ColourError());
			return;
		}
		var service = connection.Remote;
		var ss = new StringStack(input.RemoveFirstWord());
		try
		{
			var verb = ss.PopForSwitch();
			switch (verb)
			{
				case "list":
					actor.Send(service.HookDefinitions.Select(x =>
						$"{x.Id}: {x.Key} [{x.Category}] enabled={x.Enabled} authority={(PermissionLevel)x.MinimumAuthority}")
						.ListToString(separator: "\n", conjunction: ""));
					return;
				case "new":
					var key = ss.PopSpeech();
					var prog = actor.Gameworld.FutureProgs.GetByIdOrName(ss.PopSpeech()) ??
					           throw new ArgumentException("Supply an existing prog.");
					var created = service.CreateHook(key, prog.Id);
					actor.RemoveAllEffects<BuilderEditingEffect<IDiscordHook>>();
					actor.AddEffect(new BuilderEditingEffect<IDiscordHook>(actor)
						{ EditingItem = new DiscordHookDefinition(created, actor.Gameworld) });
					actor.Send($"Created disabled Discord hook {created.Key.ColourName()} (#{created.Id.ToString("N0", actor)}).");
					return;
				case "edit":
					var editKey = ss.PopSpeech();
					var selected = service.HookDefinitions.FirstOrDefault(x => x.Key.EqualTo(editKey) ||
						x.Id.ToString() == editKey) ?? throw new ArgumentException("No such Discord hook.");
					actor.RemoveAllEffects<BuilderEditingEffect<IDiscordHook>>();
					actor.AddEffect(new BuilderEditingEffect<IDiscordHook>(actor)
						{ EditingItem = new DiscordHookDefinition(selected, actor.Gameworld) });
					actor.Send($"You are editing {selected.Key.ColourName()}.");
					return;
				case "show":
					var showKey = ss.PopSpeech();
					var show = service.HookDefinitions.FirstOrDefault(x => x.Key.EqualTo(showKey) || x.Id.ToString() == showKey)
					           ?? throw new ArgumentException("No such Discord hook.");
					actor.Send($"Discord hook #{show.Id.ToString("N0", actor)}: {show.Key.ColourName()}\n{show.Description}\n" +
						$"Category: {show.Category.ColourName()}\nUsage: {show.Usage.ColourCommand()}\n" +
						$"Prog: {show.ProgId.ToString("N0", actor)} | Enabled: {show.Enabled.ToColouredString()} | " +
						$"Authority: {((PermissionLevel)show.MinimumAuthority).DescribeEnum().ColourName()}\n" +
						$"Confirmation: {show.RequireConfirmation.ToColouredString()} | Cooldown: {show.CooldownSeconds}s\n" +
						$"Accounts: {show.AllowedAccountIds} | Guilds: {show.AllowedGuildIds} | Channels: {show.AllowedChannelIds}");
					return;
				case "set":
					var first = ss.PopSpeech();
					var named = service.HookDefinitions.FirstOrDefault(x => x.Key.EqualTo(first));
					var target = named?.Key ?? actor.EffectsOfType<BuilderEditingEffect<IDiscordHook>>()
						.FirstOrDefault()?.EditingItem.Key ??
						throw new ArgumentException("Select a Discord hook with discordhook edit <key>.");
					var field = named is null ? first : ss.PopSpeech();
					actor.Send(service.EditHook(target, field, ss.SafeRemainingArgument));
					return;
				default:
					actor.Send(HookHelp.SubstituteANSIColour());
					return;
			}
		}
		catch (ArgumentException error)
		{
			actor.Send(error.Message.ColourError());
		}
	}

	private const string ConfigHelp = @"Publish Discord information or restrict integration contexts.
	#3discordconfig show <key>#0
	#3discordconfig set <key> <value>#0
Keys: game.info (public text), game.places (JSON alias:room ID), game.calendars (JSON alias:calendar ID),
game.chargenguide (JSON topic:public helpfile), security.guilds and security.channels (space/comma-separated IDs or none).
Place/calendar/guidance mappings are JSON objects. The game database defines the installation identity.";

	[PlayerCommand("DiscordConfig", "discordconfig")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	[HelpInfo("discordconfig", ConfigHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void DiscordConfig(ICharacter actor, string input)
	{
		if (actor.Gameworld.DiscordConnection is not DiscordConnection connection)
		{
			actor.Send("The Discord integration is unavailable.".ColourError());
			return;
		}
		var ss = new StringStack(input.RemoveFirstWord());
		var verb = ss.PopForSwitch();
		var key = ss.PopSpeech();
		try
		{
			if (verb == "show")
			{
				actor.Send(connection.Remote.PublishedSetting(key) ?? "That value is not configured.");
				return;
			}
			if (verb == "set")
			{
				// JSON quotes are semantically significant.
				connection.Remote.SetPublishedSetting(key, ss.RemainingArgument);
				actor.Send($"Published Discord setting {key.ColourName()}.");
				return;
			}
			actor.Send(ConfigHelp.SubstituteANSIColour());
		}
		catch (Exception error) when (error is ArgumentException or System.Text.Json.JsonException)
		{
			actor.Send(error.Message.ColourError());
		}
	}
}
