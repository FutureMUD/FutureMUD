#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class PushButtonGameItemComponentProto : GameItemComponentProto, ISelectablePrototype, ISignalSourceComponentPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3keyword <keyword>#0 - the select keyword for the button
	#3value <number>#0 - the signal value emitted while the button is active
	#3duration <seconds>#0 - how long the button stays active after being pressed
	#3emote <emote>#0 - the emote shown when the button is pressed. Use @ for the presser and $1 for the item";

	private const string CombinedBuildingHelpText = @"You can use the following options with this component:
	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3keyword <keyword>#0 - the select keyword for the button
	#3value <number>#0 - the signal value emitted while the button is active
	#3duration <seconds>#0 - how long the button stays active after being pressed
	#3emote <emote>#0 - the emote shown when the button is pressed. Use @ for the presser and $1 for the item";

	protected PushButtonGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Push Button")
	{
		Keyword = "button";
		SignalValue = 1.0;
		SignalDuration = TimeSpan.FromSeconds(1);
		PressEmote = "@ press|presses $1";
	}

	protected PushButtonGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string Keyword { get; protected set; } = string.Empty;
	public double SignalValue { get; protected set; }
	public TimeSpan SignalDuration { get; protected set; }
	public string PressEmote { get; protected set; } = string.Empty;
	public override string TypeDescription => "Push Button";

	protected override void LoadFromXml(XElement root)
	{
		Keyword = root.Element("Keyword")?.Value ?? "button";
		SignalValue = double.Parse(root.Element("SignalValue")?.Value ?? "1.0");
		SignalDuration = TimeSpan.FromSeconds(double.Parse(root.Element("SignalDurationSeconds")?.Value ?? "1.0"));
		PressEmote = root.Element("PressEmote")?.Value ?? "@ press|presses $1";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Keyword", new XCData(Keyword)),
			new XElement("SignalValue", SignalValue),
			new XElement("SignalDurationSeconds", SignalDuration.TotalSeconds),
			new XElement("PressEmote", new XCData(PressEmote))
		).ToString();
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "keyword":
				return BuildingCommandKeyword(actor, command);
			case "value":
			case "signal":
				return BuildingCommandSignalValue(actor, command);
			case "duration":
			case "time":
				return BuildingCommandDuration(actor, command);
			case "emote":
			case "press":
				return BuildingCommandEmote(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandKeyword(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What keyword should builders use with the select command for this push button?");
			return false;
		}

		Keyword = command.PopSpeech().ToLowerInvariant();
		Changed = true;
		actor.Send($"This push button now responds to {"select".ColourCommand()} {Keyword.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandSignalValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric signal value should this button emit when pressed?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number for the signal value.");
			return false;
		}

		SignalValue = value;
		Changed = true;
		actor.Send($"This button now emits a signal value of {SignalValue.ToString("N2", actor).ColourValue()} when pressed.");
		return true;
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many seconds should the button remain active after being pressed?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.Send("You must enter a positive number of seconds.");
			return false;
		}

		SignalDuration = TimeSpan.FromSeconds(value);
		Changed = true;
		actor.Send($"This button will now remain active for {SignalDuration.Describe(actor).ColourValue()} after being pressed.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote should be shown when the button is pressed? Use @ for the presser and $1 for the item.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, actor, actor, new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		PressEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This button now uses the emote {PressEmote.ColourCommand()} when pressed.");
		return true;
	}

	public override bool CanSubmit()
	{
		return !string.IsNullOrWhiteSpace(Keyword) && SignalDuration > TimeSpan.Zero && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (string.IsNullOrWhiteSpace(Keyword))
		{
			return "You must set a select keyword for this push button.";
		}

		if (SignalDuration <= TimeSpan.Zero)
		{
			return "You must set a positive active duration for this push button.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Push Button Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis component emits a signal value of {SignalValue.ToString("N2", actor).ColourValue()} for {SignalDuration.Describe(actor).ColourValue()} whenever someone uses {"select".ColourCommand()} {Keyword.ColourCommand()} on the item.\nPress Emote: {PressEmote.ColourCommand()}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("pushbutton", true,
			(gameworld, account) => new PushButtonGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("push button", false,
			(gameworld, account) => new PushButtonGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Push Button",
			(proto, gameworld) => new PushButtonGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"PushButton",
			$"A {"[selectable]".Colour(Telnet.Yellow)} {SignalComponentUtilities.SignalGeneratorTag} momentary signal input for computer-controlled items",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new PushButtonGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PushButtonGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PushButtonGameItemComponentProto(proto, gameworld));
	}
}
