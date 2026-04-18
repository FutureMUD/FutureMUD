#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class KeypadGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3code <digits>#0 - the numeric code that activates this keypad
	#3value <number>#0 - the signal value emitted after the correct code is entered
	#3duration <seconds>#0 - how long the keypad remains active after a correct entry
	#3emote <emote>#0 - the emote shown when someone enters a code. Use @ for the actor and $1 for the item

#6Notes:#0

	This powered keypad is used via #3select <item> <digits>#0 and only emits its signal while switched on and powered.";

	private static readonly string CombinedBuildingHelpText =
		$@"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{SpecificBuildingHelpText}";

	protected KeypadGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Keypad")
	{
		UseMountHostPowerSource = true;
		Wattage = 35.0;
		Code = "1234";
		SignalValue = 1.0;
		SignalDuration = TimeSpan.FromSeconds(1);
		EntryEmote = "@ tap|taps digits into $1";
	}

	protected KeypadGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string Code { get; protected set; } = string.Empty;
	public double SignalValue { get; protected set; }
	public TimeSpan SignalDuration { get; protected set; }
	public string EntryEmote { get; protected set; } = string.Empty;
	public override string TypeDescription => "Keypad";

	protected override string ComponentDescriptionOLCByline => "This item is a powered keypad";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return
			$"Code: {Code.ColourCommand()}\nSignal Value: {SignalValue.ToString("N2", actor).ColourValue()}\nActive Duration: {SignalDuration.Describe(actor).ColourValue()}\nEntry Emote: {EntryEmote.ColourCommand()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		Code = root.Element("Code")?.Value ?? "1234";
		SignalValue = double.Parse(root.Element("SignalValue")?.Value ?? "1.0");
		SignalDuration = TimeSpan.FromSeconds(double.Parse(root.Element("SignalDurationSeconds")?.Value ?? "1.0"));
		EntryEmote = root.Element("EntryEmote")?.Value ?? "@ tap|taps digits into $1";
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("Code", new XCData(Code)));
		root.Add(new XElement("SignalValue", SignalValue));
		root.Add(new XElement("SignalDurationSeconds", SignalDuration.TotalSeconds));
		root.Add(new XElement("EntryEmote", new XCData(EntryEmote)));
		return root;
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "code":
				return BuildingCommandCode(actor, command);
			case "value":
			case "signal":
				return BuildingCommandSignalValue(actor, command);
			case "duration":
			case "time":
				return BuildingCommandDuration(actor, command);
			case "emote":
			case "entryemote":
				return BuildingCommandEmote(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandCode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric code should activate this keypad?");
			return false;
		}

		var value = command.PopSpeech().Trim();
		if (string.IsNullOrEmpty(value) || !value.All(char.IsDigit))
		{
			actor.Send("Keypad codes must contain digits only.");
			return false;
		}

		Code = value;
		Changed = true;
		actor.Send($"This keypad now activates on the code {Code.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSignalValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric signal value should this keypad emit after a correct code?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number for the signal value.");
			return false;
		}

		SignalValue = value;
		Changed = true;
		actor.Send(
			$"This keypad now emits a signal value of {SignalValue.ToString("N2", actor).ColourValue()} after a correct code entry.");
		return true;
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many seconds should this keypad remain active after a correct code?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send("You must enter a positive number of seconds.");
			return false;
		}

		SignalDuration = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.Send(
			$"This keypad will now remain active for {SignalDuration.Describe(actor).ColourValue()} after a correct code entry.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote should be shown when someone enters a code? Use @ for the actor and $1 for the keypad item.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		EntryEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This keypad now uses the emote {EntryEmote.ColourCommand()} for code entry.");
		return true;
	}

	public override bool CanSubmit()
	{
		return !string.IsNullOrWhiteSpace(Code) &&
		       Code.All(char.IsDigit) &&
		       SignalDuration > TimeSpan.Zero &&
		       base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (string.IsNullOrWhiteSpace(Code) || !Code.All(char.IsDigit))
		{
			return "You must set a numeric activation code for this keypad.";
		}

		if (SignalDuration <= TimeSpan.Zero)
		{
			return "You must set a positive active duration for this keypad.";
		}

		return base.WhyCannotSubmit();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("keypad", true,
			(gameworld, account) => new KeypadGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Keypad",
			(proto, gameworld) => new KeypadGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Keypad",
			$"A {"[selectable]".Colour(Telnet.Yellow)} {"[powered]".Colour(Telnet.Magenta)} {SignalComponentUtilities.SignalGeneratorTag} that emits a momentary signal after the correct numeric code",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new KeypadGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new KeypadGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new KeypadGameItemComponentProto(proto, gameworld));
	}
}
