#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class MediaCableGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IMediaCablePrototype
{
	private const string SpecificBuildingHelpText = @"
	#3capabilities <audio|video|av>#0 - sets the media carried by this cable
	#3endpoint <key>#0 - sets the stable local media endpoint key";

	public MediaCableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Media Cable")
	{
		Capabilities = MediaCapabilities.Audio | MediaCapabilities.Video;
		EndpointKey = "cable";
		Wattage = 0.0;
		WattageDiscountPerQuality = 0.0;
		Switchable = false;
		PowerOnEmote = string.Empty;
		PowerOffEmote = string.Empty;
	}

	protected MediaCableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public MediaCapabilities Capabilities { get; protected set; }
	public string EndpointKey { get; protected set; } = "cable";
	public override string TypeDescription => "Media Cable";
	protected override string ComponentDescriptionOLCByline => "This item is a passive local media cable";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Capabilities: {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}\n" +
		       $"Endpoint: {EndpointKey.ColourCommand()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		Capabilities = Enum.TryParse<MediaCapabilities>(root.Element("Capabilities")?.Value, true,
			out var capabilities) && capabilities != MediaCapabilities.None
			? capabilities
			: MediaCapabilities.Audio | MediaCapabilities.Video;
		EndpointKey = NormaliseEndpoint(root.Element("EndpointKey")?.Value, "cable");
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("Capabilities", Capabilities));
		root.Add(new XElement("EndpointKey", EndpointKey));
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
					actor.Send("Choose audio, video or av for this cable.");
					return false;
				}

				Capabilities = capabilities;
				Changed = true;
				actor.Send($"This cable now carries {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()} media.");
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
				actor.Send($"This cable's endpoint is now {EndpointKey.ColourCommand()}.");
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("mediacable", true,
			(gameworld, account) => new MediaCableGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("media cable", false,
			(gameworld, account) => new MediaCableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Media Cable",
			(proto, gameworld) => new MediaCableGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Media Cable",
			$"Makes an item a passive {"[local media cable]".Colour(Telnet.BoldGreen)} with one media input and one output",
			$"{BuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new MediaCableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MediaCableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new MediaCableGameItemComponentProto(proto, gameworld));
	}

	private static string NormaliseEndpoint(string? value, string fallback)
	{
		var endpoint = value?.Trim() ?? string.Empty;
		return !string.IsNullOrWhiteSpace(endpoint) && !endpoint.Any(char.IsWhiteSpace) ? endpoint : fallback;
	}
}
