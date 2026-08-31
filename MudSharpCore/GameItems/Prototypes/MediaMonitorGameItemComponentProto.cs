#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class MediaMonitorGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IMediaMonitorPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3capabilities <video|av>#0 - sets whether this monitor accepts video only or audio/video
	#3ambient#0 - toggles ambient relay to everyone in the cell; otherwise viewers must use watch feed
	#3audio#0 - toggles audio presentation when A/V is supported
	#3endpoint <key>#0 - sets the stable local input endpoint key
	#3siblings#0 - toggles accepting a source endpoint on the same composite item";

	public MediaMonitorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Media Monitor")
	{
		Capabilities = MediaCapabilities.Video;
		AmbientPresentation = true;
		AudioEnabled = false;
		EndpointKey = "monitor";
		AcceptSiblingSources = false;
		Wattage = 35.0;
		WattageDiscountPerQuality = 0.75;
	}

	protected MediaMonitorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public MediaCapabilities Capabilities { get; protected set; }
	public bool AmbientPresentation { get; protected set; }
	public bool AudioEnabled { get; protected set; }
	public string EndpointKey { get; protected set; } = "monitor";
	public bool AcceptSiblingSources { get; protected set; }
	public override string TypeDescription => "Media Monitor";
	protected override string ComponentDescriptionOLCByline => "This item is a powered media monitor sink";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Capabilities: {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}\n" +
		       $"Presentation: {(AmbientPresentation ? "ambient".ColourValue() : "opt-in".ColourCommand())}\n" +
		       $"Audio: {AudioEnabled.ToColouredString()}\n" +
		       $"Endpoint: {EndpointKey.ColourCommand()}\n" +
		       $"Sibling Binding: {AcceptSiblingSources.ToColouredString()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		Capabilities = Enum.TryParse<MediaCapabilities>(root.Element("Capabilities")?.Value, true, out var capabilities)
			? capabilities
			: MediaCapabilities.Video;
		if (!Capabilities.HasFlag(MediaCapabilities.Video))
		{
			Capabilities = MediaCapabilities.Video;
		}

		AmbientPresentation = !bool.TryParse(root.Element("AmbientPresentation")?.Value, out var ambient) || ambient;
		AudioEnabled = bool.TryParse(root.Element("AudioEnabled")?.Value, out var audio) && audio;
		EndpointKey = root.Element("EndpointKey")?.Value?.Trim() ?? "monitor";
		AcceptSiblingSources = bool.TryParse(root.Element("AcceptSiblingSources")?.Value, out var siblings) && siblings;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("Capabilities", Capabilities));
		root.Add(new XElement("AmbientPresentation", AmbientPresentation));
		root.Add(new XElement("AudioEnabled", AudioEnabled));
		root.Add(new XElement("EndpointKey", EndpointKey));
		root.Add(new XElement("AcceptSiblingSources", AcceptSiblingSources));
		return root;
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "capabilities":
			case "capability":
				return BuildingCommandCapabilities(actor, command);
			case "ambient":
			case "presentation":
				AmbientPresentation = !AmbientPresentation;
				Changed = true;
				actor.Send($"This monitor now uses {(AmbientPresentation ? "ambient".ColourValue() : "opt-in".ColourCommand())} presentation.");
				return true;
			case "audio":
				AudioEnabled = !AudioEnabled;
				Changed = true;
				actor.Send($"This monitor will {(AudioEnabled ? "now".ColourValue() : "no longer".ColourError())} present audio.");
				return true;
			case "endpoint":
				return BuildingCommandEndpoint(actor, command);
			case "siblings":
			case "sibling":
				AcceptSiblingSources = !AcceptSiblingSources;
				Changed = true;
				actor.Send($"This monitor will {(AcceptSiblingSources ? "now".ColourValue() : "no longer".ColourError())} accept a sibling source on its composite item.");
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandCapabilities(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !MediaComponentUtilities.TryParseCapabilities(command.PopSpeech(), out var capabilities) ||
		    !capabilities.HasFlag(MediaCapabilities.Video))
		{
			actor.Send("A media monitor must support video; choose video or av.");
			return false;
		}

		Capabilities = capabilities;
		if (!Capabilities.HasFlag(MediaCapabilities.Audio))
		{
			AudioEnabled = false;
		}

		Changed = true;
		actor.Send($"This monitor now supports {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}.");
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
		actor.Send($"This monitor's media endpoint is now {EndpointKey.ColourCommand()}.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("mediamonitor", true,
			(gameworld, account) => new MediaMonitorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("media monitor", false,
			(gameworld, account) => new MediaMonitorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("monitor", false,
			(gameworld, account) => new MediaMonitorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Media Monitor",
			(proto, gameworld) => new MediaMonitorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Media Monitor",
			$"Makes an item a powered {"[media display]".Colour(Telnet.BoldGreen)} sink for camera, deck, and network playback",
			$"{BuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new MediaMonitorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MediaMonitorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MediaMonitorGameItemComponentProto(proto, gameworld));
	}
}
