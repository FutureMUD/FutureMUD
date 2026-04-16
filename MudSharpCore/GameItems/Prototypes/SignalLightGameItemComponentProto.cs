#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class SignalLightGameItemComponentProto : ProgLightGameItemComponentProto
{
	private const string BuildingHelpText = @"You can use the following options with this component:
	All programmable-light options, plus:
	source <componentname> - the sibling signal source component name that drives this light
	threshold <number> - the numeric threshold used to determine when the light is active
	invert - toggles whether the light is active above or below the threshold
	onemote <emote> - the emote shown when the signal lights this component
	offemote <emote> - the emote shown when the signal extinguishes this component";

	protected SignalLightGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Signal Light")
	{
		SourceComponentName = string.Empty;
		ActivationThreshold = 0.5;
		LitWhenAboveThreshold = true;
		LightOnEmote = "@ light|lights up";
		LightOffEmote = "@ go|goes dark";
	}

	protected SignalLightGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string SourceComponentName { get; protected set; } = string.Empty;
	public double ActivationThreshold { get; protected set; }
	public bool LitWhenAboveThreshold { get; protected set; }
	public string LightOnEmote { get; protected set; } = string.Empty;
	public string LightOffEmote { get; protected set; } = string.Empty;
	public override string TypeDescription => "Signal Light";

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		SourceComponentName = root.Element("SourceComponentName")?.Value ?? string.Empty;
		ActivationThreshold = double.Parse(root.Element("ActivationThreshold")?.Value ?? "0.5");
		LitWhenAboveThreshold = bool.Parse(root.Element("LitWhenAboveThreshold")?.Value ?? "true");
		LightOnEmote = root.Element("LightOnEmote")?.Value ?? "@ light|lights up";
		LightOffEmote = root.Element("LightOffEmote")?.Value ?? "@ go|goes dark";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("IlluminationProvided", IlluminationProvided),
			new XElement("SourceComponentName", new XCData(SourceComponentName)),
			new XElement("ActivationThreshold", ActivationThreshold),
			new XElement("LitWhenAboveThreshold", LitWhenAboveThreshold),
			new XElement("LightOnEmote", new XCData(LightOnEmote)),
			new XElement("LightOffEmote", new XCData(LightOffEmote))
		).ToString();
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "source":
				return BuildingCommandSource(actor, command);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
			case "invert":
				return BuildingCommandInvert(actor);
			case "onemote":
				return BuildingCommandOnEmote(actor, command);
			case "offemote":
				return BuildingCommandOffEmote(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandSource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which sibling signal source component name should drive this light?");
			return false;
		}

		SourceComponentName = command.SafeRemainingArgument.Trim();
		Changed = true;
		actor.Send($"This signal light is now driven by the sibling component named {SourceComponentName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric threshold should determine when this light is active?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number.");
			return false;
		}

		ActivationThreshold = value;
		Changed = true;
		actor.Send($"This signal light now uses a threshold of {ActivationThreshold.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInvert(ICharacter actor)
	{
		LitWhenAboveThreshold = !LitWhenAboveThreshold;
		Changed = true;
		actor.Send(
			$"This signal light is now {(LitWhenAboveThreshold ? "lit when the signal is above or equal to the threshold".ColourValue() : "lit when the signal is below the threshold".ColourValue())}.");
		return true;
	}

	private bool BuildingCommandOnEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote should be shown when this light comes on? Use @ for the light item.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		LightOnEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This light now echoes {LightOnEmote.ColourCommand()} when activated.");
		return true;
	}

	private bool BuildingCommandOffEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote should be shown when this light goes off? Use @ for the light item.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		LightOffEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This light now echoes {LightOffEmote.ColourCommand()} when deactivated.");
		return true;
	}

	public override bool CanSubmit()
	{
		return !string.IsNullOrWhiteSpace(SourceComponentName) && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (string.IsNullOrWhiteSpace(SourceComponentName))
		{
			return "You must specify a sibling signal source component name for this light.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Signal Light Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis is a signal-driven light that provides {IlluminationProvided.ToString("N2", actor).ColourValue()} lux when active.\nSource Component: {SourceComponentName.ColourName()}\nThreshold: {ActivationThreshold.ToString("N2", actor).ColourValue()}\nMode: {(LitWhenAboveThreshold ? "Active above/equal threshold".ColourValue() : "Active below threshold".ColourValue())}\nOn Emote: {LightOnEmote.ColourCommand()}\nOff Emote: {LightOffEmote.ColourCommand()}";
	}

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("signallight", true,
			(gameworld, account) => new SignalLightGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("signal light", false,
			(gameworld, account) => new SignalLightGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Signal Light",
			(proto, gameworld) => new SignalLightGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"SignalLight",
			$"A {"[light source]".Colour(Telnet.Pink)} driven by a sibling signal source component",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new SignalLightGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SignalLightGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new SignalLightGameItemComponentProto(proto, gameworld));
	}
}
