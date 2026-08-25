using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Combat.Simulation;

internal sealed class CombatSimulationCommandSession(ICell scene)
{
	public List<ICell> Cells { get; } = [scene];
	public ICell Scene
	{
		get => Cells[0];
		set => Cells[0] = value;
	}

	public List<CombatSimulationParticipantRequest> Participants { get; } = [];
	public int Seed { get; set; } = Random.Shared.Next();
	public TimeSpan MaximumVirtualTime { get; set; } = TimeSpan.FromMinutes(30);
	public int MaximumEvents { get; set; } = 100_000;
	public int MaximumTranscriptEntries { get; set; } = 10_000;
	public TimeSpan MaximumWallClockTime { get; set; } = TimeSpan.FromSeconds(60);
	public CombatSimulationResult? LastResult { get; set; }
	public CombatSimulationBatchResult? LastBatch { get; set; }
	public int NextSlot { get; set; } = 1;
}

internal static class CombatSimulationCommand
{
	private const string HelpText = @"The #3impdebug combatsim#0 workflow stages and runs an accelerated combat using transient copies of loaded characters or NPC templates across one or more staged cells. Combat, effects and heartbeats use a private virtual-time scheduler while the normal game loop is blocked. EF SaveChanges calls and ordinary save-queue work are suppressed; generated actors, bodies, items and transient cells are removed afterward. Direct Dapper writes and external effects from hooks or progs cannot be unwound.

Review #3validate#0 before running. Any warning requires #3force#0. An unset or unrecognised #3FUTUREMUD_ENVIRONMENT#0 is treated as production, where every run also requires the exact #3confirm-production#0 argument. Batch runs do not retain transcripts and have a ten-minute aggregate wall-clock guard.

The syntax is:

	#3impdebug combatsim new [<cell>]#0
	#3impdebug combatsim cell add <cell>#0
	#3impdebug combatsim cell remove <number>#0
#3impdebug combatsim add character <loaded character> team <team> [cell <number>] [layer <layer>] [state <position>] [range <melee|ranged>]#0
#3impdebug combatsim add template <NPC template> team <team> [cell <number>] [layer <layer>] [state <position>] [range <melee|ranged>] [count <number>]#0
	#3impdebug combatsim remove <slot>#0
	#3impdebug combatsim set scene <cell>#0
	#3impdebug combatsim set seed <number>#0
	#3impdebug combatsim set maxtime <timespan>#0
	#3impdebug combatsim set maxevents <number>#0
	#3impdebug combatsim set transcript <number>#0
	#3impdebug combatsim show#0
	#3impdebug combatsim validate#0
	#3impdebug combatsim run [force] [confirm-production]#0
	#3impdebug combatsim batch <runs> [seed <start>] [step <increment>] [force] [confirm-production]#0
	#3impdebug combatsim batchreport [runs [<start>] [<count>]]#0
	#3impdebug combatsim batchreport trace <run> <random|state|materialisation> [<start>] [<count>]#0
	#3impdebug combatsim report#0
	#3impdebug combatsim transcript [<start>] [<count>]#0
	#3impdebug combatsim clear#0";

	private static readonly Dictionary<long, CombatSimulationCommandSession> _sessions = [];

	public static void Execute(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().CollapseString().ToLowerInvariant();
		switch (action)
		{
			case "new":
				New(actor, command);
				return;
			case "add":
				Add(actor, command);
				return;
			case "cell":
				Cell(actor, command);
				return;
			case "remove":
				Remove(actor, command);
				return;
			case "set":
				Set(actor, command);
				return;
			case "show":
				Show(actor);
				return;
			case "validate":
				Validate(actor);
				return;
			case "run":
				Run(actor, command);
				return;
			case "batch":
				Batch(actor, command);
				return;
			case "batchreport":
				BatchReport(actor, command);
				return;
			case "report":
				Report(actor);
				return;
			case "transcript":
				Transcript(actor, command);
				return;
			case "clear":
				_sessions.Remove(actor.Id);
				actor.OutputHandler.Send("You clear your staged combat simulation.");
				return;
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return;
		}
	}

	internal static bool IsProductionEnvironment(string? environment)
	{
		return !new[] { "development", "dev", "test", "testing", "local" }
			.Any(x => string.Equals(environment, x, StringComparison.InvariantCultureIgnoreCase));
	}

	private static CombatSimulationCommandSession? Session(ICharacter actor, bool sendError = true)
	{
		if (_sessions.TryGetValue(actor.Id, out var session))
		{
			return session;
		}

		if (sendError)
		{
			actor.OutputHandler.Send(
				$"You have no staged simulation. Begin with {"impdebug combatsim new".ColourCommand()}.");
		}

		return null;
	}

	private static void New(ICharacter actor, StringStack command)
	{
		var scene = command.IsFinished ? actor.Location : actor.Gameworld.Cells.GetByIdOrName(command.SafeRemainingArgument);
		if (scene is null)
		{
			actor.OutputHandler.Send("There is no such cell to use as the combat scene.");
			return;
		}

		_sessions[actor.Id] = new CombatSimulationCommandSession(scene);
		actor.OutputHandler.Send(
			$"You begin staging an accelerated combat simulation in {scene.GetFriendlyReference(actor).ColourName()}. Add at least two opposing teams, then validate it.");
	}

	private static void Add(ICharacter actor, StringStack command)
	{
		var session = Session(actor);
		if (session is null)
		{
			return;
		}

		var sourceType = command.PopSpeech().CollapseString().ToLowerInvariant();
		if (sourceType is not ("character" or "char" or "template" or "npc"))
		{
			actor.OutputHandler.Send(
				$"Do you want to add a {"character".ColourCommand()} or an NPC {"template".ColourCommand()}?");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which loaded character or NPC template do you want to add?");
			return;
		}

		var selector = command.PopSpeech();
		if (!command.PopSpeech().EqualTo("team") || command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify the opposing team with the team keyword.");
			return;
		}

		var team = command.PopSpeech();
		if (!TryParseAddOptions(actor, command, session, out var count, out var startingCell, out var startingLayer,
			    out var startingPosition, out var startsInMelee))
		{
			return;
		}

		if (sourceType is "character" or "char")
		{
			if (count != 1)
			{
				actor.OutputHandler.Send("Loaded character sources can only be added once per command.");
				return;
			}

			var character = actor.Gameworld.Actors.GetByIdOrName(selector);
			if (character is null)
			{
				actor.OutputHandler.Send("There is no such loaded character.");
				return;
			}

			session.Participants.Add(new CombatSimulationParticipantRequest(
				session.NextSlot++, team, CombatSimulationSourceType.Character, character, null,
				StartingCell: startingCell, StartingLayer: startingLayer, StartingPosition: startingPosition,
				StartsInMelee: startsInMelee));
			actor.OutputHandler.Send(
				$"You add {character.PersonalName.GetName(NameStyle.SimpleFull).ColourName()} to team {team.ColourName()} {DescribeStartingLocation(session, startingCell, startingLayer, startingPosition)}.");
			return;
		}

		var template = actor.Gameworld.NpcTemplates.GetByIdOrName(selector);
		if (template is null)
		{
			actor.OutputHandler.Send("There is no such current NPC template.");
			return;
		}

		for (var i = 1; i <= count; i++)
		{
			session.Participants.Add(new CombatSimulationParticipantRequest(
				session.NextSlot++, team, CombatSimulationSourceType.NpcTemplate, null, template, i,
				startingCell, startingLayer, startingPosition, startsInMelee));
		}

		actor.OutputHandler.Send(
			$"You add {count.ToString("N0", actor).ColourValue()} instance{(count == 1 ? string.Empty : "s")} of {template.Name.ColourName()} to team {team.ColourName()} {DescribeStartingLocation(session, startingCell, startingLayer, startingPosition)}.");
	}

	private static bool TryParseAddOptions(
		ICharacter actor,
		StringStack command,
		CombatSimulationCommandSession session,
		out int count,
		out ICell? startingCell,
		out RoomLayer startingLayer,
		out IPositionState? startingPosition,
		out bool startsInMelee)
	{
		count = 1;
		startingCell = null;
		startingLayer = RoomLayer.GroundLevel;
		startingPosition = null;
		startsInMelee = true;
		var seen = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		while (!command.IsFinished)
		{
			var option = command.PopSpeech().CollapseString().ToLowerInvariant();
			if (!seen.Add(option))
			{
				actor.OutputHandler.Send($"You can only specify {option.ColourCommand()} once when adding a combatant.");
				return false;
			}

			switch (option)
			{
				case "count":
					if (command.IsFinished || !int.TryParse(command.PopSpeech(), out count) || count is < 1 or > 100)
					{
						actor.OutputHandler.Send("The count must be a whole number from 1 to 100.");
						return false;
					}

					break;
				case "cell":
					if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var cellNumber) ||
					    cellNumber is < 1 or > int.MaxValue || cellNumber > session.Cells.Count)
					{
						actor.OutputHandler.Send($"The cell must be a staged cell number from 1 to {session.Cells.Count:N0}.");
						return false;
					}

					startingCell = session.Cells[cellNumber - 1];
					break;
				case "layer":
					if (command.IsFinished || !TryParseRoomLayer(command.PopSpeech(), out startingLayer))
					{
						actor.OutputHandler.Send("Specify a valid layer, such as ground, trees, sky or highair.");
						return false;
					}

					break;
				case "state":
				case "position":
					startingPosition = command.IsFinished ? null : PositionState.GetState(command.PopSpeech());
					if (startingPosition is null)
					{
						actor.OutputHandler.Send("Specify a valid starting position, such as standing, sitting, prone or flying.");
						return false;
					}

					break;
				case "range":
					if (command.IsFinished)
					{
						actor.OutputHandler.Send("Specify melee or ranged as the starting combat range.");
						return false;
					}

					var range = command.PopSpeech().CollapseString();
					if (range.EqualTo("melee"))
					{
						startsInMelee = true;
						break;
					}

					if (range.EqualToAny("ranged", "range", "outside"))
					{
						startsInMelee = false;
						break;
					}

					actor.OutputHandler.Send("Specify melee or ranged as the starting combat range.");
					return false;
				default:
					actor.OutputHandler.Send("The add options are cell, layer, state, range and count.");
					return false;
			}
		}

		return true;
	}

	private static void Cell(ICharacter actor, StringStack command)
	{
		var session = Session(actor);
		if (session is null)
		{
			return;
		}

		var action = command.PopSpeech().CollapseString().ToLowerInvariant();
		switch (action)
		{
			case "add":
				var cell = actor.Gameworld.Cells.GetByIdOrName(command.SafeRemainingArgument);
				if (cell is null)
				{
					actor.OutputHandler.Send("There is no such cell.");
					return;
				}

				if (session.Cells.Any(x => ReferenceEquals(x, cell)))
				{
					actor.OutputHandler.Send("That cell is already staged for this combat simulation.");
					return;
				}

				session.Cells.Add(cell);
				actor.OutputHandler.Send($"You add cell #{session.Cells.Count:N0} {cell.GetFriendlyReference(actor).ColourName()} to the staged combat area.");
				return;
			case "remove":
				if (!int.TryParse(command.SafeRemainingArgument, out var cellNumber) ||
				    cellNumber is < 2 or > int.MaxValue || cellNumber > session.Cells.Count)
				{
					actor.OutputHandler.Send($"Specify a non-default staged cell number from 2 to {session.Cells.Count:N0}.");
					return;
				}

				var removedCell = session.Cells[cellNumber - 1];
				if (session.Participants.Any(x => ReferenceEquals(x.StartingCell, removedCell)))
				{
					actor.OutputHandler.Send("Remove or relocate combatants assigned to that cell before removing it.");
					return;
				}

				session.Cells.RemoveAt(cellNumber - 1);
				actor.OutputHandler.Send($"You remove staged cell #{cellNumber:N0}.");
				return;
			case "list":
				ShowCells(actor, session);
				return;
			default:
				actor.OutputHandler.Send("The cell syntax is impdebug combatsim cell add <cell>, cell remove <number>, or cell list.");
				return;
		}
	}

	internal static bool TryParseRoomLayer(string text, out RoomLayer layer)
	{
		if (Utilities.TryParseEnum(text, out layer))
		{
			return true;
		}

		switch (text.ToLowerInvariant().CollapseString())
		{
			case "ground":
			case "groundlevel":
				layer = RoomLayer.GroundLevel;
				return true;
			case "water":
			case "underwater":
				layer = RoomLayer.Underwater;
				return true;
			case "deepwater":
			case "deepunderwater":
				layer = RoomLayer.DeepUnderwater;
				return true;
			case "verydeepwater":
			case "verydeepunderwater":
				layer = RoomLayer.VeryDeepUnderwater;
				return true;
			case "trees":
			case "intrees":
				layer = RoomLayer.InTrees;
				return true;
			case "hightrees":
			case "highintrees":
				layer = RoomLayer.HighInTrees;
				return true;
			case "air":
			case "sky":
			case "inair":
				layer = RoomLayer.InAir;
				return true;
			case "highair":
			case "highsky":
			case "highinair":
				layer = RoomLayer.HighInAir;
				return true;
			case "roof":
			case "rooftop":
			case "rooftops":
			case "onrooftops":
				layer = RoomLayer.OnRooftops;
				return true;
			default:
				layer = RoomLayer.GroundLevel;
				return false;
		}
	}

	private static void Remove(ICharacter actor, StringStack command)
	{
		var session = Session(actor);
		if (session is null)
		{
			return;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var slot))
		{
			actor.OutputHandler.Send("Which numeric combatant slot do you want to remove?");
			return;
		}

		var removed = session.Participants.RemoveAll(x => x.Slot == slot);
		actor.OutputHandler.Send(removed > 0
			? $"You remove combatant slot {slot.ToString("N0", actor).ColourValue()}."
			: "There is no combatant with that slot number.");
	}

	private static void Set(ICharacter actor, StringStack command)
	{
		var session = Session(actor);
		if (session is null)
		{
			return;
		}

		var option = command.PopSpeech().CollapseString().ToLowerInvariant();
		switch (option)
		{
			case "scene":
			case "cell":
				var scene = actor.Gameworld.Cells.GetByIdOrName(command.SafeRemainingArgument);
				if (scene is null)
				{
					actor.OutputHandler.Send("There is no such cell.");
					return;
				}

				if (session.Cells.Skip(1).Any(x => ReferenceEquals(x, scene)))
				{
					actor.OutputHandler.Send("That cell is already staged. Use its staged cell number when adding combatants.");
					return;
				}

				var previousScene = session.Scene;
				session.Scene = scene;
				for (var i = 0; i < session.Participants.Count; i++)
				{
					if (ReferenceEquals(session.Participants[i].StartingCell, previousScene))
					{
						session.Participants[i] = session.Participants[i] with { StartingCell = scene };
					}
				}

				actor.OutputHandler.Send($"The combat scene is now {scene.GetFriendlyReference(actor).ColourName()}.");
				return;
			case "seed":
				if (!int.TryParse(command.SafeRemainingArgument, out var seed))
				{
					actor.OutputHandler.Send("The seed must be a 32-bit whole number.");
					return;
				}

				session.Seed = seed;
				actor.OutputHandler.Send($"The random seed is now {seed.ToString("N0", actor).ColourValue()}.");
				return;
			case "maxtime":
			case "time":
				if (!TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var maximumTime) ||
				    maximumTime <= TimeSpan.Zero || maximumTime > TimeSpan.FromDays(1))
				{
					actor.OutputHandler.Send("Specify a positive timespan no greater than one day.");
					return;
				}

				session.MaximumVirtualTime = maximumTime;
				actor.OutputHandler.Send($"The virtual-time limit is now {maximumTime.Describe(actor).ColourValue()}.");
				return;
			case "maxevents":
			case "events":
				if (!int.TryParse(command.SafeRemainingArgument, out var events) || events is < 1 or > 1_000_000)
				{
					actor.OutputHandler.Send("Specify an event limit from 1 to 1,000,000.");
					return;
				}

				session.MaximumEvents = events;
				actor.OutputHandler.Send($"The event limit is now {events.ToString("N0", actor).ColourValue()}.");
				return;
			case "transcript":
				if (!int.TryParse(command.SafeRemainingArgument, out var entries) || entries is < 0 or > 100_000)
				{
					actor.OutputHandler.Send("Specify a transcript limit from 0 to 100,000 entries.");
					return;
				}

				session.MaximumTranscriptEntries = entries;
				actor.OutputHandler.Send($"The transcript limit is now {entries.ToString("N0", actor).ColourValue()} entries.");
				return;
			default:
				actor.OutputHandler.Send("You can set scene, seed, maxtime, maxevents or transcript. Use combatsim cell add/remove/list to manage the staged area.");
				return;
		}
	}

	private static void Show(ICharacter actor)
	{
		var session = Session(actor);
		if (session is null)
		{
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("Accelerated Combat Simulation");
		sb.AppendLine();
		sb.AppendLine($"Default Cell: {session.Scene.GetFriendlyReference(actor).ColourName()}");
		sb.AppendLine($"Seed: {session.Seed.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Limits: {session.MaximumVirtualTime.Describe(actor).ColourValue()} virtual time, {session.MaximumEvents.ToString("N0", actor).ColourValue()} events, {session.MaximumWallClockTime.Describe(actor).ColourValue()} wall time");
		sb.AppendLine($"Transcript: {session.MaximumTranscriptEntries.ToString("N0", actor).ColourValue()} entries");
		sb.AppendLine();
		sb.AppendLine();
		AppendCells(sb, actor, session);
		sb.AppendLine();
		var rows = session.Participants
			.OrderBy(x => x.Slot)
			.Select(x => new[]
			{
				x.Slot.ToString("N0", actor),
				x.Team,
				x.SourceType.DescribeEnum(true),
				x.SourceDescription,
				CellNumberFor(session, x).ToString("N0", actor),
				x.StartingLayer.DescribeEnum(true),
				(x.StartingPosition ?? PositionStanding.Instance).DefaultDescription()
			});
		sb.AppendLine(session.Participants.Count == 0
			? "No combatants have been added."
			: StringUtilities.GetTextTable(rows, ["Slot", "Team", "Type", "Source", "Cell", "Layer", "State"], actor, Telnet.Green));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void ShowCells(ICharacter actor, CombatSimulationCommandSession session)
	{
		var sb = new StringBuilder();
		AppendCells(sb, actor, session);
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void AppendCells(StringBuilder sb, ICharacter actor, CombatSimulationCommandSession session)
	{
		var rows = session.Cells
			.Select((cell, index) => new[]
			{
				(index + 1).ToString("N0", actor),
				cell.GetFriendlyReference(actor)
			});
		sb.AppendLine(StringUtilities.GetTextTable(rows, ["Cell", "Staged Cell"], actor, Telnet.Green));
	}

	private static int CellNumberFor(CombatSimulationCommandSession session, CombatSimulationParticipantRequest participant)
	{
		if (participant.StartingCell is null)
		{
			return 1;
		}

		var index = session.Cells.FindIndex(x => ReferenceEquals(x, participant.StartingCell));
		return index >= 0 ? index + 1 : 0;
	}

	private static string DescribeStartingLocation(
		CombatSimulationCommandSession session,
		ICell? startingCell,
		RoomLayer startingLayer,
		IPositionState? startingPosition)
	{
		var cell = startingCell is null ? 1 : session.Cells.FindIndex(x => ReferenceEquals(x, startingCell)) + 1;
		return $"in cell #{cell:N0}, {startingLayer.LocativeDescription()} and {(startingPosition ?? PositionStanding.Instance).DefaultDescription()}";
	}

	private static CombatSimulationRequest BuildRequest(ICharacter actor, CombatSimulationCommandSession session, bool force)
	{
		return new CombatSimulationRequest(
			Guid.NewGuid(), actor, session.Scene, session.Participants.ToList(), session.Seed,
			session.MaximumVirtualTime, session.MaximumEvents, session.MaximumTranscriptEntries,
			session.MaximumWallClockTime, force, session.Cells.ToList());
	}

	private static CombatSimulationBatchRequest BuildBatchRequest(
		ICharacter actor,
		CombatSimulationCommandSession session,
		int firstSeed,
		int seedIncrement,
		int runCount,
		bool force)
	{
		return new CombatSimulationBatchRequest(
			Guid.NewGuid(),
			actor,
			session.Scene,
			session.Participants.ToList(),
			firstSeed,
			seedIncrement,
			runCount,
			session.MaximumVirtualTime,
			session.MaximumEvents,
			session.MaximumWallClockTime,
			TimeSpan.FromMinutes(10),
			force,
			session.Cells.ToList());
	}

	private static void Validate(ICharacter actor)
	{
		var session = Session(actor);
		if (session is null)
		{
			return;
		}

		var service = new CombatSimulationService();
		ShowValidation(actor, service.Validate(BuildRequest(actor, session, false)));
	}

	private static void ShowValidation(ICharacter actor, IReadOnlyList<CombatSimulationValidationMessage> messages)
	{
		var sb = new StringBuilder();
		if (messages.Count == 0)
		{
			sb.AppendLine("The staged simulation passed preflight without warnings.".Colour(Telnet.Green));
		}
		else
		{
			foreach (var message in messages)
			{
				sb.AppendLine($"{(message.IsError ? "ERROR".ColourError() : "WARNING".Colour(Telnet.Yellow))}: {message.Message}");
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void Run(ICharacter actor, StringStack command)
	{
		var session = Session(actor);
		if (session is null)
		{
			return;
		}

		var arguments = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		while (!command.IsFinished)
		{
			arguments.Add(command.PopSpeech());
		}

		if (arguments.Any(x => !x.In("force", "confirm-production")))
		{
			actor.OutputHandler.Send("The only run options are force and confirm-production.");
			return;
		}

		var production = IsProductionEnvironment(Environment.GetEnvironmentVariable("FUTUREMUD_ENVIRONMENT"));
		if (production && !arguments.Contains("confirm-production"))
		{
			actor.OutputHandler.Send(
				"This server is treated as production. The simulation blocks the game loop and hooks, progs, direct Dapper writes or external services may have side effects. Re-run with the exact confirm-production argument if you accept that risk."
					.ColourError());
			return;
		}

		actor.OutputHandler.Send(
			$"Starting combat simulation with seed {session.Seed.ToString("N0", actor).ColourValue()}. Player input and ordinary loop work will resume when it finishes.");
		var service = new CombatSimulationService();
		var result = service.Run(BuildRequest(actor, session, arguments.Contains("force")));
		session.LastResult = result;
		actor.OutputHandler.Send(RenderReport(result, actor));
	}

	private static void Batch(ICharacter actor, StringStack command)
	{
		var session = Session(actor);
		if (session is null)
		{
			return;
		}

		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var runCount))
		{
			actor.OutputHandler.Send("Specify how many simulations to run in the batch.");
			return;
		}

		if (runCount is < 1 or > 100)
		{
			actor.OutputHandler.Send("The batch run count must be between 1 and 100.");
			return;
		}

		var firstSeed = session.Seed;
		var seedIncrement = 1;
		var arguments = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		while (!command.IsFinished)
		{
			var option = command.PopSpeech().ToLowerInvariant();
			switch (option)
			{
				case "seed":
					if (command.IsFinished || !int.TryParse(command.PopSpeech(), out firstSeed))
					{
						actor.OutputHandler.Send("The batch starting seed must be a 32-bit whole number.");
						return;
					}

					break;
				case "step":
				case "increment":
					if (command.IsFinished || !int.TryParse(command.PopSpeech(), out seedIncrement))
					{
						actor.OutputHandler.Send("The batch seed increment must be a 32-bit whole number.");
						return;
					}

					break;
				case "force":
				case "confirm-production":
					arguments.Add(option);
					break;
				default:
					actor.OutputHandler.Send("The batch syntax is: impdebug combatsim batch <runs> [seed <start>] [step <increment>] [force] [confirm-production].");
					return;
			}
		}

		if (IsProductionEnvironment(Environment.GetEnvironmentVariable("FUTUREMUD_ENVIRONMENT")) &&
		    !arguments.Contains("confirm-production"))
		{
			actor.OutputHandler.Send(
				"This server is treated as production. A batch blocks the game loop for multiple simulations and may invoke hooks, progs, direct Dapper writes or external services. Re-run with the exact confirm-production argument if you accept that risk."
					.ColourError());
			return;
		}

		actor.OutputHandler.Send(
			$"Starting {runCount.ToString("N0", actor).ColourValue()} combat simulations from seed {firstSeed.ToString("N0", actor).ColourValue()} with an increment of {seedIncrement.ToString("N0", actor).ColourValue()}. Player input and ordinary loop work will resume when the batch finishes.");
		var result = new CombatSimulationService().RunBatch(BuildBatchRequest(
			actor,
			session,
			firstSeed,
			seedIncrement,
			runCount,
			arguments.Contains("force")));
		session.LastBatch = result;
		session.LastResult = result.Runs.LastOrDefault();
		actor.OutputHandler.Send(RenderBatchReport(result, actor));
	}

	private static void BatchReport(ICharacter actor, StringStack command)
	{
		var result = Session(actor)?.LastBatch;
		if (result is null)
		{
			actor.OutputHandler.Send("There is no completed combat-simulation batch report in your session.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(RenderBatchReport(result, actor));
			return;
		}

		var section = command.PopSpeech().ToLowerInvariant();
		if (section == "trace")
		{
			BatchTrace(actor, command, result);
			return;
		}

		if (section != "runs")
		{
			actor.OutputHandler.Send("The batch report syntax is impdebug combatsim batchreport [runs [start] [count]] or impdebug combatsim batchreport trace <run> <random|state|materialisation> [start] [count].");
			return;
		}

		var start = 1;
		var count = 25;
		if (!command.IsFinished && (!int.TryParse(command.PopSpeech(), out start) || start < 1))
		{
			actor.OutputHandler.Send("The run-report start must be a positive run number.");
			return;
		}

		if (!command.IsFinished && (!int.TryParse(command.PopSpeech(), out count) || count is < 1 or > 100))
		{
			actor.OutputHandler.Send("The run-report count must be from 1 to 100.");
			return;
		}

		if (!command.IsFinished)
		{
			actor.OutputHandler.Send("The batch report syntax is impdebug combatsim batchreport [runs [start] [count]] or impdebug combatsim batchreport trace <run> <random|state|materialisation> [start] [count].");
			return;
		}

		actor.OutputHandler.Send(RenderBatchRunPage(result, actor, start, count));
	}

	private static void BatchTrace(ICharacter actor, StringStack command, CombatSimulationBatchResult result)
	{
		if (!int.TryParse(command.PopSpeech(), out var runNumber) || runNumber < 1 || runNumber > result.Runs.Count)
		{
			actor.OutputHandler.Send($"The run number must be between 1 and {result.Runs.Count:N0}.");
			return;
		}

		var traceType = command.PopSpeech().ToLowerInvariant();
		var start = 1;
		var count = 100;
		if (!command.IsFinished && (!int.TryParse(command.PopSpeech(), out start) || start < 1))
		{
			actor.OutputHandler.Send("The trace start must be a positive entry number.");
			return;
		}

		if (!command.IsFinished && (!int.TryParse(command.PopSpeech(), out count) || count is < 1 or > 1_000))
		{
			actor.OutputHandler.Send("The trace count must be from 1 to 1,000.");
			return;
		}

		if (!command.IsFinished)
		{
			actor.OutputHandler.Send("The batch trace syntax is impdebug combatsim batchreport trace <run> <random|state|materialisation> [start] [count].");
			return;
		}

		var trace = result.Runs[runNumber - 1].ExecutionTrace;
		IReadOnlyList<CombatSimulationRandomTraceEntry>? randomEntries = null;
		IReadOnlyList<CombatSimulationStateTraceEntry>? stateEntries = null;
		IReadOnlyList<CombatSimulationMaterialisationTraceEntry>? materialisationEntries = null;
		var traceName = traceType;
		switch (traceType)
		{
			case "random":
				randomEntries = trace.RecentRandomOperations;
				break;
			case "state":
				stateEntries = trace.RecentStateOperations;
				break;
			case "materialisation":
			case "materialization":
				materialisationEntries = trace.MaterialisationEntries;
				traceName = "materialisation";
				break;
			default:
				actor.OutputHandler.Send("The trace type must be random, state or materialisation.");
				return;
		}

		var entries = randomEntries is not null
			? randomEntries.Select(x => (x.OperationIndex, x.Description))
			: stateEntries is not null
				? stateEntries.Select(x => (x.OperationIndex, x.Description))
				: materialisationEntries!.Select(x => (x.OperationIndex, x.Description));
		var page = entries
			.Skip(start - 1)
			.Take(count)
			.ToList();
		if (page.Count == 0)
		{
			actor.OutputHandler.Send($"There are no {traceName} trace entries in that range. Detailed traces are retained only for repeated-seed batch diagnostics.");
			return;
		}

		var end = start + page.Count - 1;
		actor.OutputHandler.Send($"Combat Simulation Batch Run {runNumber:N0} {traceName} trace {start:N0} to {end:N0}:\n" +
			page.Select(x => $"{x.OperationIndex:N0}: {x.Description}").ListToString(separator: "\n", conjunction: string.Empty, twoItemJoiner: "\n"));
	}

	private static void Report(ICharacter actor)
	{
		var result = Session(actor)?.LastResult;
		if (result is null)
		{
			actor.OutputHandler.Send("There is no completed combat simulation report in your session.");
			return;
		}

		actor.OutputHandler.Send(RenderReport(result, actor));
	}

	private static string RenderReport(CombatSimulationResult result, ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Combat Simulation {result.RunId.ToString().ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Status: {result.Status.DescribeEnum(true).ColourName()}");
		sb.AppendLine($"Winner: {(result.WinningTeam?.ColourName() ?? "none".ColourError())}");
		sb.AppendLine($"Seed: {result.Seed.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Duration: {result.VirtualDuration.Describe(actor).ColourValue()} virtual, {result.WallClockDuration.Describe(actor).ColourValue()} wall-clock");
		sb.AppendLine($"Events: {result.EventCount.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Execution fingerprint: {result.ExecutionFingerprint.ColourValue()}");
		sb.AppendLine($"Trace layers: {DescribeTraceSummary(result.ExecutionTrace)}");
		sb.AppendLine($"Transcript: {result.Transcript.Count.ToString("N0", actor).ColourValue()} entries{(result.TranscriptTruncated ? " (truncated)".Colour(Telnet.Yellow) : string.Empty)}");
		if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
		{
			sb.AppendLine($"Error: {result.ErrorMessage.ColourError()}");
		}

		if (result.Participants.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				result.Participants.OrderBy(x => x.Slot).Select(x => new[]
				{
					x.Slot.ToString("N0", actor), x.Team, x.Name, x.Outcome.DescribeEnum(true),
					x.FinalState.Describe(), x.BloodRatio.ToString("P1", actor),
					x.StaminaRatio.ToString("P1", actor), x.WoundCount.ToString("N0", actor)
				}),
				["Slot", "Team", "Name", "Outcome", "State", "Blood", "Stamina", "Wounds"], actor,
				Telnet.Green));
		}

		if (result.Validation.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Preflight and runtime warnings:");
			foreach (var warning in result.Validation)
			{
				sb.AppendLine($"  {(warning.IsError ? "ERROR".ColourError() : "WARNING".Colour(Telnet.Yellow))}: {warning.Message}");
			}
		}

		return sb.ToString();
	}

	private static string RenderBatchReport(CombatSimulationBatchResult result, ICharacter actor)
	{
		var sb = new StringBuilder();
		var lastSeed = result.RequestedRunCount > 0
			? (long)result.FirstSeed + ((long)result.SeedIncrement * (result.RequestedRunCount - 1))
			: result.FirstSeed;
		sb.AppendLine($"Combat Simulation Batch {result.BatchId.ToString().ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Runs: {result.Runs.Count.ToString("N0", actor).ColourValue()} of {result.RequestedRunCount.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Seeds: {result.FirstSeed.ToString("N0", actor).ColourValue()} to {lastSeed.ToString("N0", actor).ColourValue()} (increment {result.SeedIncrement.ToString("N0", actor).ColourValue()})");
		if (result.Runs.Count > 0)
		{
			sb.AppendLine($"Virtual duration: {result.TotalVirtualDuration.Describe(actor).ColourValue()} total, {result.AverageVirtualDuration.Describe(actor).ColourValue()} average, {result.FastestVirtualDuration.Describe(actor).ColourValue()} to {result.SlowestVirtualDuration.Describe(actor).ColourValue()} range");
			sb.AppendLine($"Simulation wall-clock: {result.TotalWallClockDuration.Describe(actor).ColourValue()} total, {result.AverageWallClockDuration.Describe(actor).ColourValue()} average");
		}

		sb.AppendLine($"Batch wall-clock: {result.BatchWallClockDuration.Describe(actor).ColourValue()}");
		sb.AppendLine("Transcripts: omitted for batch runs to keep tournament memory bounded.".Colour(Telnet.Yellow));
		if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
		{
			sb.AppendLine($"Error: {result.ErrorMessage.ColourError()}");
		}

		if (result.Teams.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				result.Teams.Select(x => new[]
				{
					x.Team,
					x.Wins.ToString("N0", actor),
					x.WinRate.ToString("P1", actor)
				}),
				["Team", "Wins", "Win Rate"], actor, Telnet.Green));
		}

		if (result.Statuses.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				result.Statuses.Select(x => new[]
				{
					x.Status.DescribeEnum(true),
					x.Count.ToString("N0", actor)
				}),
				["Run Status", "Count"], actor, Telnet.Green));
		}

		if (result.Outcomes.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				result.Outcomes.Select(x => new[]
				{
					x.Outcome.DescribeEnum(true),
					x.Count.ToString("N0", actor)
				}),
				["Combatant Outcome", "Count"], actor, Telnet.Green));
		}

		var replayGroups = result.Runs
			.GroupBy(x => x.Seed)
			.Where(x => x.Count() > 1)
			.OrderBy(x => x.Key)
			.ToList();
		if (replayGroups.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Repeated-seed replay diagnostics:");
			var replayRows = replayGroups.Select(group =>
			{
				var fingerprints = group.Select(x => x.ExecutionFingerprint).Distinct().ToList();
				return new[]
				{
					group.Key.ToString("N0", actor),
					group.Count().ToString("N0", actor),
					fingerprints.Count == 1 ? "Match".Colour(Telnet.Green) : "MISMATCH".ColourError(),
					fingerprints.Count.ToString("N0", actor),
					fingerprints.Select(ShortFingerprint).ListToString()
				};
			});
			sb.AppendLine(StringUtilities.GetTextTable(replayRows,
				["Seed", "Runs", "Replay", "Traces", "Fingerprint(s)"], actor, Telnet.Green));
			if (replayGroups.Any(x => x.Select(y => y.ExecutionFingerprint).Distinct().Skip(1).Any()))
			{
				sb.AppendLine("WARNING: runs with the same seed produced different execution fingerprints. Use batchreport runs to inspect the affected runs."
					.ColourError());
				foreach (var group in replayGroups.Where(x => x.Select(y => y.ExecutionFingerprint).Distinct().Skip(1).Any()))
				{
					sb.AppendLine($"Seed {group.Key.ToString("N0", actor)} trace comparison: {DescribeTraceComparison(group)}.");
				}
			}
		}

		if (result.Validation.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Batch preflight warnings:");
			foreach (var warning in result.Validation)
			{
				sb.AppendLine($"  {(warning.IsError ? "ERROR".ColourError() : "WARNING".Colour(Telnet.Yellow))}: {warning.Message}");
			}
		}

		return sb.ToString();
	}

	private static string RenderBatchRunPage(CombatSimulationBatchResult result, ICharacter actor, int start, int count)
	{
		var runs = result.Runs
			.Select((run, index) => (Run: run, Number: index + 1))
			.Skip(start - 1)
			.Take(count)
			.ToList();
		if (runs.Count == 0)
		{
			return "There are no completed batch runs in that range.";
		}

		var end = runs[^1].Number;
		var sb = new StringBuilder();
		sb.AppendLine($"Combat Simulation Batch Runs {runs[0].Number.ToString("N0", actor).ColourValue()} to {end.ToString("N0", actor).ColourValue()} of {result.Runs.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(runs.Select(x => new[]
			{
				x.Number.ToString("N0", actor),
				x.Run.Seed.ToString("N0", actor),
				x.Run.Status.DescribeEnum(true),
				x.Run.WinningTeam ?? "none",
				x.Run.VirtualDuration.Describe(actor),
				x.Run.WallClockDuration.Describe(actor),
				x.Run.EventCount.ToString("N0", actor),
				ShortFingerprint(x.Run.ExecutionFingerprint)
			}),
			["Run", "Seed", "Status", "Winner", "Virtual", "Wall", "Events", "Trace"], actor, Telnet.Green));
		return sb.ToString();
	}

	private static string ShortFingerprint(string fingerprint)
	{
		var separator = fingerprint.IndexOf(':');
		var prefix = separator < 0 ? string.Empty : fingerprint[..(separator + 1)];
		var digest = separator < 0 ? fingerprint : fingerprint[(separator + 1)..];
		if (digest.Length <= 12)
		{
			return $"{prefix}{digest}";
		}

		return $"{prefix}{digest[..12]}";
	}

	private static string DescribeTraceSummary(CombatSimulationExecutionTraceSummary trace)
	{
		return $"materialisation {trace.MaterialisationOperations:N0}/{ShortFingerprint(trace.MaterialisationFingerprint)}, " +
		       $"random {trace.RandomOperations:N0}/{ShortFingerprint(trace.RandomFingerprint)}, " +
		       $"scheduler {trace.SchedulerTicks:N0}/{ShortFingerprint(trace.SchedulerFingerprint)}, " +
		       $"output {trace.TranscriptEntries:N0}/{ShortFingerprint(trace.TranscriptFingerprint)}, " +
		       $"terminal {ShortFingerprint(trace.TerminalFingerprint)}, " +
		       $"checkpoints {trace.Checkpoints.Count:N0}{(trace.CheckpointsTruncated ? "+" : string.Empty)}";
	}

	private static string DescribeTraceComparison(IEnumerable<CombatSimulationResult> runs)
	{
		var traces = runs.Select(x => x.ExecutionTrace).ToList();
		var layers = new[]
			{
				DescribeTraceLayer("materialisation", traces.Select(x => x.MaterialisationFingerprint)),
				DescribeTraceLayer("random", traces.Select(x => x.RandomFingerprint)),
				DescribeTraceLayer("scheduler", traces.Select(x => x.SchedulerFingerprint)),
				DescribeTraceLayer("output", traces.Select(x => x.TranscriptFingerprint)),
				DescribeTraceLayer("terminal", traces.Select(x => x.TerminalFingerprint))
			}
			.ListToString(separator: ", ", conjunction: string.Empty, twoItemJoiner: ", ");
		return $"{layers}; {DescribeMaterialisationDivergence(traces)}; {DescribeFirstCheckpointDivergence(traces)}; {DescribeRecentStateDivergence(traces)}; {DescribeRecentRandomDivergence(traces)}";
	}

	private static string DescribeMaterialisationDivergence(
		IReadOnlyList<CombatSimulationExecutionTraceSummary> traces)
	{
		if (!traces.Select(x => x.MaterialisationFingerprint).Distinct().Skip(1).Any())
		{
			return "materialised runtime state match";
		}

		var entriesByRun = traces
			.Select(x => x.MaterialisationEntries.ToDictionary(y => y.OperationIndex, y => y.Description))
			.ToList();
		var comparableIndexes = entriesByRun
			.Select(x => x.Keys.AsEnumerable())
			.Aggregate((left, right) => left.Intersect(right))
			.Order()
			.ToList();
		foreach (var index in comparableIndexes)
		{
			var entries = entriesByRun.Select(x => x[index]).ToList();
			if (entries.Distinct().Skip(1).Any())
			{
				return $"materialisation state entry #{index:N0} differs";
			}
		}

		return "materialisation details differ outside their comparable prefix";
	}

	private static string DescribeRecentStateDivergence(
		IReadOnlyList<CombatSimulationExecutionTraceSummary> traces)
	{
		var operationsByRun = traces
			.Select(x => x.RecentStateOperations.ToDictionary(y => y.OperationIndex, y => y.Description))
			.ToList();
		if (operationsByRun.Count == 0 || operationsByRun.Any(x => x.Count == 0))
		{
			return "no detailed state trace";
		}

		var comparableIndexes = operationsByRun
			.Select(x => x.Keys.AsEnumerable())
			.Aggregate((left, right) => left.Intersect(right))
			.Order()
			.ToList();
		foreach (var index in comparableIndexes)
		{
			var operations = operationsByRun.Select(x => x[index]).ToList();
			if (operations.Distinct().Skip(1).Any())
			{
				return $"state transition #{index:N0} differs ({operations.ListToString(conjunction: string.Empty, separator: " / ", twoItemJoiner: " / ")})";
			}
		}

		return "detailed state trace has no comparable split";
	}

	private static string DescribeTraceLayer(string name, IEnumerable<string> fingerprints)
	{
		return fingerprints.Distinct().Skip(1).Any()
			? $"{name} MISMATCH".ColourError()
			: $"{name} match".Colour(Telnet.Green);
	}

	private static string DescribeFirstCheckpointDivergence(
		IReadOnlyList<CombatSimulationExecutionTraceSummary> traces)
	{
		if (traces.Count == 0 || traces.Any(x => x.Checkpoints.Count == 0))
		{
			return "no comparable checkpoints";
		}

		var commonCount = traces.Min(x => x.Checkpoints.Count);
		for (var index = 0; index < commonCount; index++)
		{
			var checkpoints = traces.Select(x => x.Checkpoints[index]).ToList();
			var differences = new List<string>();
			if (checkpoints.Select(x => x.EventCount).Distinct().Skip(1).Any())
			{
				differences.Add("event total");
			}

			if (checkpoints.Select(x => (x.RandomOperations, x.RandomFingerprint)).Distinct().Skip(1).Any())
			{
				differences.Add("random");
			}

			if (checkpoints.Select(x => (x.SchedulerTicks, x.SchedulerFingerprint)).Distinct().Skip(1).Any())
			{
				differences.Add("scheduler");
			}

			if (checkpoints.Select(x => (x.TranscriptEntries, x.TranscriptFingerprint)).Distinct().Skip(1).Any())
			{
				differences.Add("output");
			}

			if (differences.Count > 0)
			{
				var firstEvent = checkpoints.Min(x => x.EventCount);
				var lastEvent = checkpoints.Max(x => x.EventCount);
				return $"first checkpoint split near {firstEvent:N0}" +
				       (firstEvent == lastEvent ? "" : $" to {lastEvent:N0}") +
				       $" events ({differences.ListToString()})";
			}
		}

		return traces.Select(x => x.Checkpoints.Count).Distinct().Skip(1).Any() ||
		       traces.Select(x => x.CheckpointsTruncated).Distinct().Skip(1).Any()
			? "checkpoint history length differs after the comparable prefix"
			: "checkpoint sequence match";
	}

	private static string DescribeRecentRandomDivergence(
		IReadOnlyList<CombatSimulationExecutionTraceSummary> traces)
	{
		if (!traces.Select(x => x.RandomFingerprint).Distinct().Skip(1).Any())
		{
			return "recent random trace match";
		}

		var operationsByRun = traces
			.Select(x => x.RecentRandomOperations.ToDictionary(y => y.OperationIndex, y => y.Description))
			.ToList();
		var comparableIndexes = operationsByRun
			.Select(x => x.Keys.AsEnumerable())
			.Aggregate((left, right) => left.Intersect(right))
			.Order()
			.ToList();
		foreach (var index in comparableIndexes)
		{
			var operations = operationsByRun.Select(x => x[index]).ToList();
			if (operations.Distinct().Skip(1).Any())
			{
				return $"recent random call #{index:N0} differs ({operations.ListToString(conjunction: string.Empty, separator: " / ", twoItemJoiner: " / ")})";
			}
		}

		return "recent random trace has no comparable split";
	}

	private static void Transcript(ICharacter actor, StringStack command)
	{
		var result = Session(actor)?.LastResult;
		if (result is null)
		{
			actor.OutputHandler.Send("There is no completed combat simulation transcript in your session.");
			return;
		}

		var start = 1;
		var count = 100;
		if (!command.IsFinished && (!int.TryParse(command.PopSpeech(), out start) || start < 1))
		{
			actor.OutputHandler.Send("The transcript start must be a positive entry number.");
			return;
		}

		if (!command.IsFinished && (!int.TryParse(command.PopSpeech(), out count) || count is < 1 or > 1_000))
		{
			actor.OutputHandler.Send("The transcript count must be from 1 to 1,000.");
			return;
		}

		if (!command.IsFinished)
		{
			actor.OutputHandler.Send("The syntax is impdebug combatsim transcript [start] [count].");
			return;
		}

		var entries = result.Transcript.Skip(start - 1).Take(count).ToList();
		actor.OutputHandler.Send(entries.Count == 0
			? "There are no transcript entries in that range."
			: entries.Select((x, i) => $"{(start + i).ToString("N0", actor)}: {x}").ListToString(
				separator: "\n", conjunction: string.Empty, twoItemJoiner: "\n"));
	}
}
