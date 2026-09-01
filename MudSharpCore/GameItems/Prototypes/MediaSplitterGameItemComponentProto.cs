#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class MediaSplitterGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IMediaSplitterPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3capabilities <audio|video|av>#0 - sets the media carried by this splitter
	#3endpoint <key>#0 - sets the stable local media endpoint key
	#3ports <count>#0 - sets the number of local media output ports";

	public MediaSplitterGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Media Splitter")
	{
		Capabilities = MediaCapabilities.Audio | MediaCapabilities.Video;
		EndpointKey = "splitter";
		OutputPorts = 3;
		Wattage = 0.0;
		WattageDiscountPerQuality = 0.0;
		Switchable = false;
		PowerOnEmote = string.Empty;
		PowerOffEmote = string.Empty;
	}

	protected MediaSplitterGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public MediaCapabilities Capabilities { get; protected set; }
	public string EndpointKey { get; protected set; } = "splitter";
	public int OutputPorts { get; protected set; }
	public override string TypeDescription => "Media Splitter";
	protected override string ComponentDescriptionOLCByline => "This item is a passive local media splitter";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Capabilities: {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}\n" +
		       $"Endpoint: {EndpointKey.ColourCommand()}\n" +
		       $"Output Ports: {OutputPorts.ToString("N0", actor).ColourValue()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		Capabilities = Enum.TryParse<MediaCapabilities>(root.Element("Capabilities")?.Value, true,
			out var capabilities) && capabilities != MediaCapabilities.None
			? capabilities
			: MediaCapabilities.Audio | MediaCapabilities.Video;
		EndpointKey = NormaliseEndpoint(root.Element("EndpointKey")?.Value, "splitter");
		OutputPorts = int.TryParse(root.Element("OutputPorts")?.Value, out var ports) ? Math.Max(2, ports) : 3;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("Capabilities", Capabilities));
		root.Add(new XElement("EndpointKey", EndpointKey));
		root.Add(new XElement("OutputPorts", OutputPorts));
		return root;
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "capabilities":
			case "capability":
				if (command.IsFinished || !MediaComponentUtilities.TryParseCapabilities(command.PopSpeech(), out var capabilities))
				{
					actor.Send("Choose audio, video or av for this splitter.");
					return false;
				}

				Capabilities = capabilities;
				Changed = true;
				actor.Send($"This splitter now carries {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()} media.");
				return true;
			case "endpoint":
				var endpoint = command.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(endpoint) || endpoint.Any(char.IsWhiteSpace))
				{
					actor.Send("Endpoint keys must be a single non-empty word.");
					return false;
				}

				EndpointKey = endpoint;
				Changed = true;
				actor.Send($"This splitter's endpoint is now {EndpointKey.ColourCommand()}.");
				return true;
			case "ports":
			case "outputports":
				if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var ports) || ports < 2)
				{
					actor.Send("A splitter needs at least two output ports.");
					return false;
				}

				OutputPorts = ports;
				Changed = true;
				actor.Send($"This splitter now has {OutputPorts.ToString("N0", actor).ColourValue()} local media output ports.");
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("mediasplitter", true,
			(gameworld, account) => new MediaSplitterGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("media splitter", false,
			(gameworld, account) => new MediaSplitterGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Media Splitter",
			(proto, gameworld) => new MediaSplitterGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("Media Splitter",
			$"Makes an item a passive {"[local media splitter]".Colour(Telnet.BoldGreen)} that fans one source out to multiple sinks",
			$"{BuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new MediaSplitterGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MediaSplitterGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MediaSplitterGameItemComponentProto(proto, gameworld));
	}

	private static string NormaliseEndpoint(string? value, string fallback)
	{
		var endpoint = value?.Trim() ?? string.Empty;
		return !string.IsNullOrWhiteSpace(endpoint) && !endpoint.Any(char.IsWhiteSpace) ? endpoint : fallback;
	}
}
