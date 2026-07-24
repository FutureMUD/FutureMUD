#nullable enable

using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Form.Audio;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class InstrumentGameItemComponentProto : GameItemComponentProto, IInstrumentPrototype
{
	protected InstrumentGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type = "Instrument")
		: base(gameworld, originator, type)
	{
		InstrumentFamily = "instrument";
		PerformanceDifficulty = Difficulty.Normal;
		Volume = AudioVolume.Decent;
		RequiredHands = 1;
		UseModes = InstrumentUseMode.Handheld;
		InitialStaminaCost = 1.0;
		StaminaPerTick = 1.0;
		TickInterval = TimeSpan.FromSeconds(10);
		LocalPlayEmote = "@ begin|begins playing $1.";
		LocalTickEmote = "@ continue|continues playing $1.";
		DistantPlayEmote = "You hear an instrument being played {0}.";
		FailureEmote = "@ attempt|attempts to play $1, but cannot produce a coherent performance.";
		StopEmote = "@ stop|stops playing $1.";
		AllowedPositions.UnionWith(["standing", "sitting", "kneeling"]);
		Styles.Add("general");
		Changed = true;
	}

	protected InstrumentGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Instrument";
	public string InstrumentFamily { get; protected set; } = "instrument";
	public ITraitDefinition? PerformanceTrait { get; protected set; }
	public Difficulty PerformanceDifficulty { get; protected set; }
	public AudioVolume Volume { get; protected set; }
	public int RequiredHands { get; protected set; }
	public InstrumentUseMode UseModes { get; protected set; }
	public double InitialStaminaCost { get; protected set; }
	public double StaminaPerTick { get; protected set; }
	public TimeSpan TickInterval { get; protected set; }
	public HashSet<string> AllowedPositions { get; } = new(StringComparer.OrdinalIgnoreCase);
	public List<string> Styles { get; } = [];
	public string LocalPlayEmote { get; protected set; } = string.Empty;
	public string LocalTickEmote { get; protected set; } = string.Empty;
	public string DistantPlayEmote { get; protected set; } = string.Empty;
	public string FailureEmote { get; protected set; } = string.Empty;
	public string StopEmote { get; protected set; } = string.Empty;
	public IFutureProg? CanPlayProg { get; protected set; }
	public IFutureProg? WhyCannotPlayProg { get; protected set; }
	public IFutureProg? OnPlayProg { get; protected set; }
	public IFutureProg? OnStopProg { get; protected set; }

	protected override void LoadFromXml(XElement root)
	{
		InstrumentFamily = root.Element("Family")?.Value ?? "instrument";
		PerformanceTrait = Gameworld.Traits.Get(long.Parse(root.Element("PerformanceTrait")?.Value ?? "0"));
		PerformanceDifficulty = root.Element("Difficulty")?.Value.TryParseEnum<Difficulty>(out var difficulty) == true
			? difficulty
			: Difficulty.Normal;
		Volume = root.Element("Volume")?.Value.TryParseEnum<AudioVolume>(out var volume) == true
			? volume
			: AudioVolume.Decent;
		RequiredHands = int.TryParse(root.Element("RequiredHands")?.Value, out var hands) ? Math.Clamp(hands, 0, 2) : 1;
		UseModes = Enum.TryParse(root.Element("UseModes")?.Value, true, out InstrumentUseMode modes)
			? modes
			: InstrumentUseMode.Handheld;
		InitialStaminaCost = double.TryParse(root.Element("InitialStamina")?.Value, out var initial) ? initial : 1.0;
		StaminaPerTick = double.TryParse(root.Element("TickStamina")?.Value, out var tick) ? tick : 1.0;
		TickInterval = TimeSpan.FromSeconds(
			double.TryParse(root.Element("TickSeconds")?.Value, out var seconds) ? Math.Max(1.0, seconds) : 10.0);
		AllowedPositions.Clear();
		AllowedPositions.UnionWith(root.Element("Positions")?.Elements("Position").Select(x => x.Value) ??
			Enumerable.Empty<string>());
		Styles.Clear();
		Styles.AddRange(root.Element("Styles")?.Elements("Style").Select(x => x.Value)
			.Distinct(StringComparer.OrdinalIgnoreCase) ?? Enumerable.Empty<string>());
		if (Styles.Count == 0)
		{
			Styles.Add("general");
		}

		LocalPlayEmote = root.Element("LocalPlayEmote")?.Value ?? "@ begin|begins playing $1.";
		LocalTickEmote = root.Element("LocalTickEmote")?.Value ?? "@ continue|continues playing $1.";
		DistantPlayEmote = root.Element("DistantPlayEmote")?.Value ?? "You hear an instrument being played {0}.";
		FailureEmote = root.Element("FailureEmote")?.Value ??
			"@ attempt|attempts to play $1, but cannot produce a coherent performance.";
		StopEmote = root.Element("StopEmote")?.Value ?? "@ stop|stops playing $1.";
		CanPlayProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanPlayProg")?.Value ?? "0"));
		WhyCannotPlayProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotPlayProg")?.Value ?? "0"));
		OnPlayProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnPlayProg")?.Value ?? "0"));
		OnStopProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnStopProg")?.Value ?? "0"));
		LoadAdditionalFromXml(root);
	}

	protected virtual void LoadAdditionalFromXml(XElement root)
	{
	}

	protected virtual XElement SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Family", new XCData(InstrumentFamily)),
			new XElement("PerformanceTrait", PerformanceTrait?.Id ?? 0),
			new XElement("Difficulty", PerformanceDifficulty),
			new XElement("Volume", Volume),
			new XElement("RequiredHands", RequiredHands),
			new XElement("UseModes", UseModes),
			new XElement("InitialStamina", InitialStaminaCost),
			new XElement("TickStamina", StaminaPerTick),
			new XElement("TickSeconds", TickInterval.TotalSeconds),
			new XElement("Positions", AllowedPositions.OrderBy(x => x).Select(x => new XElement("Position", x))),
			new XElement("Styles", Styles.Select(x => new XElement("Style", new XCData(x)))),
			new XElement("LocalPlayEmote", new XCData(LocalPlayEmote)),
			new XElement("LocalTickEmote", new XCData(LocalTickEmote)),
			new XElement("DistantPlayEmote", new XCData(DistantPlayEmote)),
			new XElement("FailureEmote", new XCData(FailureEmote)),
			new XElement("StopEmote", new XCData(StopEmote)),
			new XElement("CanPlayProg", CanPlayProg?.Id ?? 0),
			new XElement("WhyCannotPlayProg", WhyCannotPlayProg?.Id ?? 0),
			new XElement("OnPlayProg", OnPlayProg?.Id ?? 0),
			new XElement("OnStopProg", OnStopProg?.Id ?? 0));
	}

	protected override string SaveToXml()
	{
		return SaveDefinition().ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new InstrumentGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new InstrumentGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new InstrumentGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("instrument", true,
			(gameworld, account) => new InstrumentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Instrument",
			(proto, gameworld) => new InstrumentGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Instrument", "Makes an item a playable audible instrument", BuildingHelpText);
	}

	private const string BuildingHelpText =
		@"You can use the following options with this component:

	#3family <text>#0 - sets the instrument family
	#3trait <skill>|clear#0 - sets the performance skill
	#3difficulty <difficulty>#0 - sets the performance difficulty
	#3volume <volume>#0 - sets loudness and propagation range
	#3hands <0-2>#0 - sets required functioning hands
	#3mode handheld|worn|room#0 - toggles an allowed use mode
	#3position <position>|clear#0 - toggles an allowed position
	#3style <style>|clear#0 - toggles a style
	#3stamina <initial> <per tick>#0 - sets stamina costs
	#3interval <seconds>#0 - sets the repeated performance interval
	#3playecho|tickecho|distantecho|failecho|stopecho <emote>#0 - sets output
	#3canplay|whycanplay|onplay|onstop <prog>|clear#0 - sets FutureProg hooks";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopSpeech().ToLowerInvariant();
		switch (verb)
		{
			case "family":
				return SetText(actor, command, value => InstrumentFamily = value, "instrument family");
			case "trait":
				return BuildingCommandTrait(actor, command);
			case "difficulty":
				if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
				{
					actor.OutputHandler.Send("That is not a valid difficulty.");
					return false;
				}
				PerformanceDifficulty = difficulty;
				break;
			case "volume":
				if (!command.SafeRemainingArgument.TryParseEnum<AudioVolume>(out var volume))
				{
					actor.OutputHandler.Send("That is not a valid audio volume.");
					return false;
				}
				Volume = volume;
				break;
			case "hands":
				if (!int.TryParse(command.SafeRemainingArgument, out var hands) || hands is < 0 or > 2)
				{
					actor.OutputHandler.Send("Required hands must be from zero to two.");
					return false;
				}
				RequiredHands = hands;
				break;
			case "mode":
				return ToggleMode(actor, command);
			case "position":
				return ToggleValue(actor, command, AllowedPositions, "position");
			case "style":
				return ToggleValue(actor, command, Styles, "style");
			case "stamina":
				var initialText = command.PopSpeech();
				var tickText = command.PopSpeech();
				if (!double.TryParse(initialText, out var initial) || !double.TryParse(tickText, out var tick) ||
				    initial < 0.0 || tick < 0.0)
				{
					actor.OutputHandler.Send("Specify non-negative initial and per-tick stamina costs.");
					return false;
				}
				InitialStaminaCost = initial;
				StaminaPerTick = tick;
				break;
			case "interval":
				if (!double.TryParse(command.SafeRemainingArgument, out var seconds) || seconds < 1.0)
				{
					actor.OutputHandler.Send("The interval must be at least one second.");
					return false;
				}
				TickInterval = TimeSpan.FromSeconds(seconds);
				break;
			case "playecho":
				return SetText(actor, command, value => LocalPlayEmote = value, "play echo");
			case "tickecho":
				return SetText(actor, command, value => LocalTickEmote = value, "tick echo");
			case "distantecho":
				return SetText(actor, command, value => DistantPlayEmote = value, "distant echo");
			case "failecho":
				return SetText(actor, command, value => FailureEmote = value, "failure echo");
			case "stopecho":
				return SetText(actor, command, value => StopEmote = value, "stop echo");
			case "canplay":
				return SetProg(actor, command, ProgVariableTypes.Boolean,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text],
					value => CanPlayProg = value, "CanPlay");
			case "whycanplay":
				return SetProg(actor, command, ProgVariableTypes.Text,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text],
					value => WhyCannotPlayProg = value, "WhyCannotPlay");
			case "onplay":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Number],
					value => OnPlayProg = value, "OnPlay");
			case "onstop":
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text],
					value => OnStopProg = value, "OnStop");
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}

		Changed = true;
		actor.OutputHandler.Send("Instrument setting updated.");
		return true;
	}

	protected bool SetText(ICharacter actor, StringStack command, Action<string> setter, string name)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should the {name} be?");
			return false;
		}

		setter(command.SafeRemainingArgument);
		Changed = true;
		actor.OutputHandler.Send($"The {name} has been updated.");
		return true;
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			PerformanceTrait = null;
			Changed = true;
			actor.OutputHandler.Send("This instrument now uses automatic performance outcomes.");
			return true;
		}

		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null || trait.TraitType != TraitType.Skill)
		{
			actor.OutputHandler.Send("There is no such skill.");
			return false;
		}

		PerformanceTrait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This instrument now uses {trait.Name.ColourName()}.");
		return true;
	}

	private bool ToggleMode(ICharacter actor, StringStack command)
	{
		var mode = command.SafeRemainingArgument.ToLowerInvariant() switch
		{
			"hand" or "held" or "handheld" => InstrumentUseMode.Handheld,
			"worn" or "wear" => InstrumentUseMode.Worn,
			"room" or "placed" => InstrumentUseMode.Room,
			_ => InstrumentUseMode.None
		};
		if (mode == InstrumentUseMode.None)
		{
			actor.OutputHandler.Send("Specify handheld, worn or room.");
			return false;
		}

		UseModes = UseModes.HasFlag(mode) ? UseModes & ~mode : UseModes | mode;
		if (UseModes == InstrumentUseMode.None)
		{
			UseModes = mode;
			actor.OutputHandler.Send("An instrument must retain at least one use mode.");
			return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"Allowed use modes are now {UseModes.DescribeEnum().ColourName()}.");
		return true;
	}

	protected bool ToggleValue(ICharacter actor, StringStack command, ICollection<string> values, string name)
	{
		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			values.Clear();
			Changed = true;
			actor.OutputHandler.Send($"The {name} list is now unrestricted.");
			return true;
		}

		var value = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrEmpty(value))
		{
			actor.OutputHandler.Send($"Which {name} should be toggled?");
			return false;
		}

		var existing = values.FirstOrDefault(x => x.EqualTo(value));
		if (existing is null)
		{
			values.Add(value);
		}
		else
		{
			values.Remove(existing);
		}

		Changed = true;
		actor.OutputHandler.Send($"The {name} list has been updated.");
		return true;
	}

	protected bool SetProg(ICharacter actor, StringStack command, ProgVariableTypes returnType,
		ProgVariableTypes[] parameters, Action<IFutureProg?> setter, string name)
	{
		if (command.SafeRemainingArgument.EqualTo("clear"))
		{
			setter(null);
			Changed = true;
			actor.OutputHandler.Send($"The {name} prog has been cleared.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, returnType, parameters).LookupProg();
		if (prog is null)
		{
			return false;
		}

		setter(prog);
		Changed = true;
		actor.OutputHandler.Send($"The {name} prog is now {prog.MXPClickableFunctionName()}.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Instrument Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nFamily: {InstrumentFamily.ColourName()}\nTrait: {PerformanceTrait?.Name.ColourName() ?? "Automatic".ColourValue()}\nDifficulty: {PerformanceDifficulty.DescribeColoured()}\nVolume: {Volume.DescribeEnum().ColourValue()}\nHands: {RequiredHands.ToString("N0", actor).ColourValue()}\nUse Modes: {UseModes.DescribeEnum().ColourName()}\nPositions: {(AllowedPositions.Count == 0 ? "Any" : AllowedPositions.ListToString())}\nStyles: {Styles.ListToString()}\nInitial Stamina: {InitialStaminaCost.ToString("N2", actor).ColourValue()}\nTick: {StaminaPerTick.ToString("N2", actor).ColourValue()} every {TickInterval.Describe(actor).ColourValue()}";
	}
}
