#nullable enable

using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class RelaySwitchGameItemComponentProto : ProgPowerSupplyGameItemComponentProto
{
	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - renames the component
	#3desc <description>#0 - sets the description of the component
	#3wattage <watts>#0 - sets the maximum power the relay can supply when closed
	#3source <component>#0 - the signal source component prototype name or id whose default signal endpoint drives this relay
	#3threshold <number>#0 - the numeric threshold used to determine when the relay closes
	#3invert#0 - toggles whether the relay closes above or below the threshold";

	protected RelaySwitchGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator)
	{
		SourceComponentId = 0L;
		SourceComponentName = string.Empty;
		SourceEndpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
		ActivationThreshold = 0.5;
		ClosedWhenAboveThreshold = true;
	}

	protected RelaySwitchGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public long SourceComponentId { get; protected set; }
	public string SourceComponentName { get; protected set; } = string.Empty;
	public string SourceEndpointKey { get; protected set; } = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public double ActivationThreshold { get; protected set; }
	public bool ClosedWhenAboveThreshold { get; protected set; }
	public override string TypeDescription => "Relay Switch";

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		SourceComponentId = long.TryParse(root.Element("SourceComponentId")?.Value, out var sourceId) ? sourceId : 0L;
		SourceComponentName = root.Element("SourceComponentName")?.Value ?? string.Empty;
		SourceEndpointKey = SignalComponentUtilities.NormaliseSignalEndpointKey(root.Element("SourceEndpointKey")?.Value);
		ActivationThreshold = double.Parse(root.Element("ActivationThreshold")?.Value ?? "0.5");
		ClosedWhenAboveThreshold = bool.Parse(root.Element("ClosedWhenAboveThreshold")?.Value ?? "true");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Wattage", Wattage),
			new XElement("SourceComponentId", SourceComponentId),
			new XElement("SourceComponentName", new XCData(SourceComponentName)),
			new XElement("SourceEndpointKey", new XCData(SourceEndpointKey)),
			new XElement("ActivationThreshold", ActivationThreshold),
			new XElement("ClosedWhenAboveThreshold", ClosedWhenAboveThreshold)
		).ToString();
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "source":
				return BuildingCommandSource(actor, command);
			case "threshold":
				return BuildingCommandThreshold(actor, command);
			case "invert":
				return BuildingCommandInvert(actor);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandSource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which signal source component prototype should drive this relay switch?");
			return false;
		}

		if (!SignalComponentUtilities.TryResolveSignalComponentPrototype(Gameworld, command.SafeRemainingArgument.Trim(),
			    out var sourcePrototype) || sourcePrototype is null)
		{
			actor.Send("There is no such item component prototype.");
			return false;
		}

		SourceComponentId = sourcePrototype.Id;
		SourceComponentName = sourcePrototype.Name;
		SourceEndpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
		Changed = true;
		actor.Send(
			$"This relay switch is now driven by the {SourceEndpointKey.ColourCommand()} endpoint on signal source component prototype {SourceComponentName.ColourName()} (#{SourceComponentId.ToString("N0", actor)}).");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric threshold should determine when this relay closes?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number.");
			return false;
		}

		ActivationThreshold = value;
		Changed = true;
		actor.Send($"This relay switch now uses a threshold of {ActivationThreshold.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInvert(ICharacter actor)
	{
		ClosedWhenAboveThreshold = !ClosedWhenAboveThreshold;
		Changed = true;
		actor.Send(
			$"This relay switch is now {(ClosedWhenAboveThreshold ? "closed above or equal to the threshold".ColourValue() : "closed below the threshold".ColourValue())}.");
		return true;
	}

	public override bool CanSubmit()
	{
		return (SourceComponentId > 0 || !string.IsNullOrWhiteSpace(SourceComponentName)) && Wattage >= 0.0 &&
		       base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (SourceComponentId <= 0 && string.IsNullOrWhiteSpace(SourceComponentName))
		{
			return "You must specify a signal source component prototype for this relay switch.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Relay Switch Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis is a signal-driven relay switch that can provide up to {Wattage.ToString("N2", actor).ColourValue()} watts when closed.\nSource Endpoint: {SignalComponentUtilities.DescribeSignalComponent(Gameworld, SourceComponentId, SourceComponentName, SourceEndpointKey).ColourName()} (#{SourceComponentId.ToString("N0", actor)})\nThreshold: {ActivationThreshold.ToString("N2", actor).ColourValue()}\nMode: {(ClosedWhenAboveThreshold ? "Closed above/equal threshold".ColourValue() : "Closed below threshold".ColourValue())}";
	}

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("relayswitch", true,
			(gameworld, account) => new RelaySwitchGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("relay switch", false,
			(gameworld, account) => new RelaySwitchGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Relay Switch",
			(proto, gameworld) => new RelaySwitchGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"RelaySwitch",
			$"{"[provides power]".Colour(Telnet.BoldMagenta)} {SignalComponentUtilities.SignalConsumerTag} that closes or opens a power feed from a signal source",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new RelaySwitchGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RelaySwitchGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new RelaySwitchGameItemComponentProto(proto, gameworld));
	}
}
