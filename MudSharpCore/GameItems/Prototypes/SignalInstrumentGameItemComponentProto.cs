#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class SignalInstrumentGameItemComponentProto : InstrumentGameItemComponentProto, ISignalInstrumentPrototype
{
	protected SignalInstrumentGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "SignalInstrument")
	{
		SignalStaminaCost = 5.0;
		SignalCooldown = TimeSpan.FromSeconds(10);
		SignalPatterns.Add(new InstrumentSignalPattern("attention",
			"@ sound|sounds the attention call on $1.",
			"You hear the attention call sounded {0}.",
			"@ attempt|attempts an attention call on $1, but produces only a garbled signal."));
	}

	protected SignalInstrumentGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public override string TypeDescription => "SignalInstrument";
	public List<InstrumentSignalPattern> SignalPatterns { get; } = [];
	public double SignalStaminaCost { get; private set; }
	public TimeSpan SignalCooldown { get; private set; }
	public IFutureProg? CanSignalProg { get; private set; }
	public IFutureProg? WhyCannotSignalProg { get; private set; }
	public IFutureProg? OnSignalProg { get; private set; }

	protected override void LoadAdditionalFromXml(XElement root)
	{
		SignalStaminaCost = double.TryParse(root.Element("SignalStamina")?.Value, out var stamina) ? stamina : 5.0;
		SignalCooldown = TimeSpan.FromSeconds(
			double.TryParse(root.Element("SignalCooldownSeconds")?.Value, out var seconds) ? seconds : 10.0);
		SignalPatterns.Clear();
		foreach (var element in root.Element("Signals")?.Elements("Signal") ?? Enumerable.Empty<XElement>())
		{
			SignalPatterns.Add(new InstrumentSignalPattern(
				element.Attribute("name")?.Value ?? "signal",
				element.Element("Local")?.Value ?? "@ sound|sounds a signal on $1.",
				element.Element("Distant")?.Value ?? "You hear a signal sounded {0}.",
				element.Element("Failure")?.Value ?? "@ produce|produces a garbled signal on $1."));
		}

		CanSignalProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanSignalProg")?.Value ?? "0"));
		WhyCannotSignalProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotSignalProg")?.Value ?? "0"));
		OnSignalProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnSignalProg")?.Value ?? "0"));
	}

	protected override XElement SaveDefinition()
	{
		var root = base.SaveDefinition();
		root.Add(
			new XElement("SignalStamina", SignalStaminaCost),
			new XElement("SignalCooldownSeconds", SignalCooldown.TotalSeconds),
			new XElement("Signals", SignalPatterns.Select(x =>
				new XElement("Signal", new XAttribute("name", x.Name),
					new XElement("Local", new XCData(x.LocalEmote)),
					new XElement("Distant", new XCData(x.DistantEmote)),
					new XElement("Failure", new XCData(x.FailureEmote))))),
			new XElement("CanSignalProg", CanSignalProg?.Id ?? 0),
			new XElement("WhyCannotSignalProg", WhyCannotSignalProg?.Id ?? 0),
			new XElement("OnSignalProg", OnSignalProg?.Id ?? 0));
		return root;
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new SignalInstrumentGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SignalInstrumentGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new SignalInstrumentGameItemComponentProto(proto, gameworld));
	}

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("signalinstrument", true,
			(gameworld, account) => new SignalInstrumentGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("signal instrument", false,
			(gameworld, account) => new SignalInstrumentGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("SignalInstrument",
			(proto, gameworld) => new SignalInstrumentGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("SignalInstrument",
			"A playable instrument with recognised military signal patterns", BuildingHelpText);
	}

	private const string BuildingHelpText =
		@"This component supports all Instrument options plus:

	#3signal <name> <local>|<distant>|<failure>#0 - adds or replaces a signal
	#3signal <name> remove#0 - removes a signal
	#3signalstamina <amount>#0 - sets signal stamina cost
	#3signalcooldown <seconds>#0 - sets signal cooldown
	#3cansignal|whycansignal|onsignal <prog>|clear#0 - sets signal FutureProgs";

	public override string ShowBuildingHelp => base.ShowBuildingHelp + "\n\n" + BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PeekSpeech().ToLowerInvariant();
		switch (verb)
		{
			case "signal":
				command.PopSpeech();
				return BuildingCommandSignal(actor, command);
			case "signalstamina":
				command.PopSpeech();
				if (!double.TryParse(command.SafeRemainingArgument, out var stamina) || stamina < 0.0)
				{
					actor.OutputHandler.Send("Specify a non-negative stamina cost.");
					return false;
				}
				SignalStaminaCost = stamina;
				break;
			case "signalcooldown":
				command.PopSpeech();
				if (!double.TryParse(command.SafeRemainingArgument, out var seconds) || seconds < 0.0)
				{
					actor.OutputHandler.Send("Specify a non-negative cooldown in seconds.");
					return false;
				}
				SignalCooldown = TimeSpan.FromSeconds(seconds);
				break;
			case "cansignal":
				command.PopSpeech();
				return SetProg(actor, command, ProgVariableTypes.Boolean,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text],
					value => CanSignalProg = value, "CanSignal");
			case "whycansignal":
				command.PopSpeech();
				return SetProg(actor, command, ProgVariableTypes.Text,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text],
					value => WhyCannotSignalProg = value, "WhyCannotSignal");
			case "onsignal":
				command.PopSpeech();
				return SetProg(actor, command, ProgVariableTypes.Void,
					[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Number],
					value => OnSignalProg = value, "OnSignal");
			default:
				return base.BuildingCommand(actor, command);
		}

		Changed = true;
		actor.OutputHandler.Send("Signal instrument setting updated.");
		return true;
	}

	private bool BuildingCommandSignal(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		if (string.IsNullOrWhiteSpace(name))
		{
			actor.OutputHandler.Send("Which signal pattern do you want to edit?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("remove"))
		{
			SignalPatterns.RemoveAll(x => x.Name.EqualTo(name));
			Changed = true;
			actor.OutputHandler.Send($"The {name.ColourName()} signal has been removed.");
			return true;
		}

		var parts = command.SafeRemainingArgument.Split('|');
		if (parts.Length != 3)
		{
			actor.OutputHandler.Send("Specify local, distant and failure echoes separated by | characters.");
			return false;
		}

		SignalPatterns.RemoveAll(x => x.Name.EqualTo(name));
		SignalPatterns.Add(new InstrumentSignalPattern(name, parts[0].Trim(), parts[1].Trim(), parts[2].Trim()));
		Changed = true;
		actor.OutputHandler.Send($"The {name.ColourName()} signal has been updated.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return base.ComponentDescriptionOLC(actor) +
		       $"\nSignals: {SignalPatterns.Select(x => x.Name).ListToString()}\nSignal Stamina: {SignalStaminaCost.ToString("N2", actor).ColourValue()}\nSignal Cooldown: {SignalCooldown.Describe(actor).ColourValue()}";
	}
}
