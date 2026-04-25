#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ToggleSwitchGameItemComponentProto : GameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3onvalue <number>#0 - signal value emitted when the switch is on
	#3offvalue <number>#0 - signal value emitted when the switch is off
	#3initial#0 - toggles whether the switch starts on or off";

	private const string CombinedBuildingHelpText = @"You can use the following options with this component:
	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3onvalue <number>#0 - signal value emitted when the switch is on
	#3offvalue <number>#0 - signal value emitted when the switch is off
	#3initial#0 - toggles whether the switch starts on or off";

	protected ToggleSwitchGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Toggle Switch")
	{
		OnValue = 1.0;
		OffValue = 0.0;
		InitiallyOn = false;
	}

	protected ToggleSwitchGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public double OnValue { get; protected set; }
	public double OffValue { get; protected set; }
	public bool InitiallyOn { get; protected set; }
	public override string TypeDescription => "Toggle Switch";

	protected override void LoadFromXml(XElement root)
	{
		OnValue = double.Parse(root.Element("OnValue")?.Value ?? "1.0");
		OffValue = double.Parse(root.Element("OffValue")?.Value ?? "0.0");
		InitiallyOn = bool.Parse(root.Element("InitiallyOn")?.Value ?? "false");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("OnValue", OnValue),
			new XElement("OffValue", OffValue),
			new XElement("InitiallyOn", InitiallyOn)
		).ToString();
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "onvalue":
			case "on":
				return BuildingCommandOnValue(actor, command);
			case "offvalue":
			case "off":
				return BuildingCommandOffValue(actor, command);
			case "initial":
			case "initially":
				return BuildingCommandInitial(actor);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandOnValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What signal value should this toggle switch emit when switched on?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number.");
			return false;
		}

		OnValue = value;
		Changed = true;
		actor.Send($"This toggle switch now emits {OnValue.ToString("N2", actor).ColourValue()} when on.");
		return true;
	}

	private bool BuildingCommandOffValue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What signal value should this toggle switch emit when switched off?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number.");
			return false;
		}

		OffValue = value;
		Changed = true;
		actor.Send($"This toggle switch now emits {OffValue.ToString("N2", actor).ColourValue()} when off.");
		return true;
	}

	private bool BuildingCommandInitial(ICharacter actor)
	{
		InitiallyOn = !InitiallyOn;
		Changed = true;
		actor.Send($"This toggle switch will now start {((InitiallyOn ? "on" : "off")).ColourValue()} when new items are created.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Toggle Switch Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis component emits {OnValue.ToString("N2", actor).ColourValue()} when switched on and {OffValue.ToString("N2", actor).ColourValue()} when switched off. New items start {(InitiallyOn ? "on".ColourValue() : "off".ColourName())}.";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("toggleswitch", true,
			(gameworld, account) => new ToggleSwitchGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("toggle switch", false,
			(gameworld, account) => new ToggleSwitchGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Toggle Switch",
			(proto, gameworld) => new ToggleSwitchGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ToggleSwitch",
			$"A {"[switchable]".Colour(Telnet.Yellow)} {SignalComponentUtilities.SignalGeneratorTag} persistent signal input for computer-controlled items",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ToggleSwitchGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ToggleSwitchGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ToggleSwitchGameItemComponentProto(proto, gameworld));
	}
}
