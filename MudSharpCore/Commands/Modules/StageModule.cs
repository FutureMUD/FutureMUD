#nullable enable

using MudSharp.Construction;
using MudSharp.Accounts;
using MudSharp.RPG.Performances;
using System.Globalization;

namespace MudSharp.Commands.Modules;

internal sealed class StageModule : Module<ICharacter>
{
	private StageModule() : base("Stage") { IsNecessary = true; }
	public static StageModule Instance { get; } = new();

	private const string StageHelp = """
		Relay public action from a stage to spectators in a different room. Sight, hearing, language and concealment apply to each spectator. Private cues and whispers stay off the feed.

		#5Managing stage links#0
		  #3stage open <source> <audience> <label>#0 — open a one-way link. The label prefixes what spectators receive.
		  #3stage list [room]#0 — show live handles, both endpoints, labels and ownership. A room filter matches either endpoint.
		  #3stage close <handle>#0 — close the listed handle. Other handles on the same room pair keep working.

		Use #3here#0, a room ID, a room name or #3@1#0 for the most recently built room. Quote multiword room names. For example, #3stage open here "Upper Gallery" Main stage#0.
		Manual links persist until closed. Use Stage and Unstage cues in a work when the production should manage its own links.
		""";

	[PlayerCommand("Stage", "stage")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	[HelpInfo("stage", StageHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Stage(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var action = ss.PopSpeech();
		switch (action.ToLowerInvariant())
		{
			case "open":
			{
				var sourceText = ss.PopSpeech();
				var audienceText = ss.PopSpeech();
				var label = ss.SafeRemainingArgument;
				var source = RoomBuilderModule.LookupRoom(actor, sourceText);
				var audience = RoomBuilderModule.LookupRoom(actor, audienceText);
				string? problem = null;
				if (sourceText.Length == 0 || audienceText.Length == 0 || label.Length == 0)
					problem = $"Supply a source room, audience room and display label. Use {"stage open <source> <audience> <label>".ColourCommand()}.";
				else if (source is null) problem = $"No source room matches {sourceText.ColourCommand()}. Quote a multiword room name.";
				else if (audience is null) problem = $"No audience room matches {audienceText.ColourCommand()}. Quote a multiword room name.";
				else if (source == audience) problem = "Choose separate rooms for the stage and its audience.";
				else if (!StageLinks.IsSupported(source) || !StageLinks.IsSupported(audience))
					problem = "A stage link requires two ordinary rooms. Route rooms and room templates cannot be endpoints.";
				else if (label.Length > 100 || label.Any(char.IsControl))
					problem = "The display label must contain 1 to 100 characters on one line, without control characters.";
				if (problem is not null)
				{
					actor.OutputHandler.Send(problem);
					return;
				}
				var openedHandle = StageLinks.Open(source!, audience!, label);
				actor.OutputHandler.Send(openedHandle == 0
					? "Those rooms cannot be used for stage viewing."
					: $"Stage link {openedHandle.ToString("N0", actor).ColourValue()} opened from {source!.Name.ColourName()} to {audience!.Name.ColourName()}.");
				return;
			}
			case "close":
				var handleText = ss.SafeRemainingArgument;
				if (handleText.Length == 0)
					actor.OutputHandler.Send($"Supply a handle from {"stage list".ColourCommand()}. Use {"stage close <handle>".ColourCommand()}.");
				else if (!long.TryParse(handleText, NumberStyles.Integer | NumberStyles.AllowThousands, actor, out var closeHandle) || closeHandle < 1)
					actor.OutputHandler.Send($"The text {handleText.ColourCommand()} is not a valid stage handle. Enter a whole number of 1 or more from {"stage list".ColourCommand()}.");
				else if (!StageLinks.Close(actor.Gameworld, closeHandle))
					actor.OutputHandler.Send($"Stage handle {handleText.ColourValue()} is no longer active. Use {"stage list".ColourCommand()} to see current handles.");
				else actor.OutputHandler.Send($"Stage link {closeHandle.ToString("N0", actor).ColourValue()} closed.");
				return;
			case "list":
			{
				IRoom? filter = ss.IsFinished ? null : RoomBuilderModule.LookupRoom(actor, ss.SafeRemainingArgument);
				if (!ss.IsFinished && filter is null)
				{
					actor.OutputHandler.Send($"No room matches {ss.SafeRemainingArgument.ColourCommand()}. Use {"here".ColourCommand()}, a room name or ID.");
					return;
				}
				var effects = actor.Gameworld.Rooms
					.SelectMany(x => x.EffectsOfType<Effects.Concrete.StageObservationEffect>()
						.Where(effect => filter is null || x.Id == filter.Id || effect.AudienceRoomId == filter.Id)
						.Select(effect => (Source: x, Effect: effect)));
				var lines = effects.SelectMany(x => x.Effect.Handles.Select(handle =>
					$"{handle.Id.ToString("N0", actor).ColourValue()}: {x.Source.Name.ColourName()} -> {(actor.Gameworld.Rooms.Get(x.Effect.AudienceRoomId)?.Name ?? "<missing room>").ColourName()} [{handle.Label}] owner={handle.RunKey ?? "independent"}")).ToList();
				actor.OutputHandler.Send(lines.Any() ? lines.ListToLines() : "There are no stage links matching that room.");
				return;
			}
			default:
				actor.OutputHandler.Send($"Unknown stage command {action.ColourCommand()}. Use {"stage help".ColourCommand()} to see open, list and close.");
				return;
		}
	}
}
