#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

/// <summary>
/// A generic powered recording and/or playback transport. A composite item pairs it with a normal container for
/// ordinary insert/remove behaviour and, when desired, a monitor or speaker sibling for presentation.
/// </summary>
public class MediaDeckGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IMediaDeckPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3format <key>#0 - sets the physical format key accepted by this deck
	#3capabilities <audio|video|av>#0 - sets the media types this deck can record and play
	#3record#0 - toggles recording capability
	#3playback#0 - toggles playback capability
	#3endpoint <key>#0 - sets the stable local media endpoint key
	#3siblings#0 - toggles accepting a media source from a sibling component
	#3ports <count>#0 - sets the number of local media output ports";

	public MediaDeckGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Media Deck")
	{
		FormatKey = "generic";
		Capabilities = MediaCapabilities.Audio;
		CanRecord = true;
		CanPlayback = true;
		EndpointKey = "deck";
		AcceptSiblingSources = false;
		OutputPorts = 1;
		Wattage = 12.0;
		WattageDiscountPerQuality = 0.2;
	}

	protected MediaDeckGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string FormatKey { get; protected set; } = "generic";
	public MediaCapabilities Capabilities { get; protected set; }
	public bool CanRecord { get; protected set; }
	public bool CanPlayback { get; protected set; }
	public string EndpointKey { get; protected set; } = "deck";
	public bool AcceptSiblingSources { get; protected set; }
	public int OutputPorts { get; protected set; }
	public override string TypeDescription => "Media Deck";
	protected override string ComponentDescriptionOLCByline => "This item is a powered media recording and playback deck";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Format: {FormatKey.ColourCommand()}\n" +
		       $"Capabilities: {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}\n" +
		       $"Record: {CanRecord.ToColouredString()}\n" +
		       $"Playback: {CanPlayback.ToColouredString()}\n" +
		       $"Endpoint: {EndpointKey.ColourCommand()}\n" +
		       $"Sibling Binding: {AcceptSiblingSources.ToColouredString()}\n" +
		       $"Output Ports: {OutputPorts.ToString("N0", actor).ColourValue()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		FormatKey = root.Element("FormatKey")?.Value?.Trim() ?? "generic";
		if (string.IsNullOrWhiteSpace(FormatKey))
		{
			FormatKey = "generic";
		}

		Capabilities = Enum.TryParse<MediaCapabilities>(root.Element("Capabilities")?.Value, true,
			out var capabilities) && capabilities != MediaCapabilities.None
			? capabilities
			: MediaCapabilities.Audio;
		CanRecord = !bool.TryParse(root.Element("CanRecord")?.Value, out var canRecord) || canRecord;
		CanPlayback = !bool.TryParse(root.Element("CanPlayback")?.Value, out var canPlayback) || canPlayback;
		EndpointKey = root.Element("EndpointKey")?.Value?.Trim() ?? "deck";
		if (string.IsNullOrWhiteSpace(EndpointKey))
		{
			EndpointKey = "deck";
		}

		AcceptSiblingSources = bool.TryParse(root.Element("AcceptSiblingSources")?.Value, out var siblings) && siblings;
		OutputPorts = int.TryParse(root.Element("OutputPorts")?.Value, out var ports) ? Math.Max(1, ports) : 1;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("FormatKey", FormatKey));
		root.Add(new XElement("Capabilities", Capabilities));
		root.Add(new XElement("CanRecord", CanRecord));
		root.Add(new XElement("CanPlayback", CanPlayback));
		root.Add(new XElement("EndpointKey", EndpointKey));
		root.Add(new XElement("AcceptSiblingSources", AcceptSiblingSources));
		root.Add(new XElement("OutputPorts", OutputPorts));
		return root;
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "format":
				return BuildingCommandFormat(actor, command);
			case "capabilities":
			case "capability":
				return BuildingCommandCapabilities(actor, command);
			case "record":
			case "recording":
				CanRecord = !CanRecord;
				Changed = true;
				actor.Send($"This deck can {(CanRecord ? "now".ColourValue() : "no longer".ColourError())} record media.");
				return true;
			case "playback":
			case "play":
				CanPlayback = !CanPlayback;
				Changed = true;
				actor.Send($"This deck can {(CanPlayback ? "now".ColourValue() : "no longer".ColourError())} play media.");
				return true;
			case "endpoint":
				return BuildingCommandEndpoint(actor, command);
			case "siblings":
			case "sibling":
				AcceptSiblingSources = !AcceptSiblingSources;
				Changed = true;
				actor.Send($"This deck will {(AcceptSiblingSources ? "now".ColourValue() : "no longer".ColourError())} accept a sibling media source.");
				return true;
			case "ports":
			case "outputports":
				return BuildingCommandPorts(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandFormat(ICharacter actor, StringStack command)
	{
		var value = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsWhiteSpace))
		{
			actor.Send("Media format keys must be a single non-empty word.");
			return false;
		}

		FormatKey = value;
		Changed = true;
		actor.Send($"This deck now accepts the {FormatKey.ColourCommand()} format.");
		return true;
	}

	private bool BuildingCommandCapabilities(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !MediaComponentUtilities.TryParseCapabilities(command.PopSpeech(), out var capabilities))
		{
			actor.Send("Choose audio, video or av for this deck.");
			return false;
		}

		Capabilities = capabilities;
		Changed = true;
		actor.Send($"This deck now supports {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()} media.");
		return true;
	}

	private bool BuildingCommandEndpoint(ICharacter actor, StringStack command)
	{
		var endpoint = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(endpoint) || endpoint.Any(char.IsWhiteSpace))
		{
			actor.Send("Endpoint keys must be a single non-empty word.");
			return false;
		}

		EndpointKey = endpoint;
		Changed = true;
		actor.Send($"This deck's endpoint is now {EndpointKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPorts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var ports) || ports < 1)
		{
			actor.Send("How many local media output ports should this deck have?");
			return false;
		}

		OutputPorts = ports;
		Changed = true;
		actor.Send($"This deck now has {OutputPorts.ToString("N0", actor).ColourValue()} local media output ports.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("mediadeck", true,
			(gameworld, account) => new MediaDeckGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("media deck", false,
			(gameworld, account) => new MediaDeckGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("deck", false,
			(gameworld, account) => new MediaDeckGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Media Deck",
			(proto, gameworld) => new MediaDeckGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("Media Deck",
			$"Makes an item a powered {"[media deck]".Colour(Telnet.BoldGreen)} that records to and plays from a compatible physical medium",
			$"{BuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new MediaDeckGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MediaDeckGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new MediaDeckGameItemComponentProto(proto, gameworld));
	}
}
