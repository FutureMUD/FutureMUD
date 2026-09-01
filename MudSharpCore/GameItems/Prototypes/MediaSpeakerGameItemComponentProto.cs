#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Form.Audio;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class MediaSpeakerGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IMediaSpeakerPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3endpoint <key>#0 - sets the stable local audio input endpoint key
	#3volume <volume>#0 - sets the default audio output volume, including silent
	#3siblings#0 - toggles accepting a source endpoint on the same composite item";

	public MediaSpeakerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Media Speaker")
	{
		EndpointKey = "speaker";
		OutputVolume = AudioVolume.Decent;
		AcceptSiblingSources = false;
		Wattage = 15.0;
		WattageDiscountPerQuality = 0.4;
	}

	protected MediaSpeakerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string EndpointKey { get; protected set; } = "speaker";
	public AudioVolume OutputVolume { get; protected set; }
	public bool AcceptSiblingSources { get; protected set; }
	public override string TypeDescription => "Media Speaker";
	protected override string ComponentDescriptionOLCByline => "This item is a powered audio media sink";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Endpoint: {EndpointKey.ColourCommand()}\n" +
		       $"Audio Volume: {OutputVolume.DescribeEnum().ColourValue()}\n" +
		       $"Sibling Binding: {AcceptSiblingSources.ToColouredString()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		EndpointKey = root.Element("EndpointKey")?.Value?.Trim() ?? "speaker";
		OutputVolume = int.TryParse(root.Element("OutputVolume")?.Value, out var rawVolume) &&
		               Enum.IsDefined(typeof(AudioVolume), rawVolume)
			? (AudioVolume)rawVolume
			: AudioVolume.Decent;
		AcceptSiblingSources = bool.TryParse(root.Element("AcceptSiblingSources")?.Value, out var siblings) && siblings;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("EndpointKey", EndpointKey));
		root.Add(new XElement("OutputVolume", (int)OutputVolume));
		root.Add(new XElement("AcceptSiblingSources", AcceptSiblingSources));
		return root;
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "endpoint":
				var endpoint = command.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(endpoint) || endpoint.Any(char.IsWhiteSpace))
				{
					actor.Send("Endpoint keys must be a single non-empty word.");
					return false;
				}

				EndpointKey = endpoint;
				Changed = true;
				actor.Send($"This speaker's media endpoint is now {EndpointKey.ColourCommand()}.");
				return true;
			case "volume":
				if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum<AudioVolume>(out var volume))
				{
					actor.Send($"You must specify a valid audio volume: {Enum.GetValues<AudioVolume>().Select(x => x.DescribeEnum()).ListToString()}.");
					return false;
				}

				OutputVolume = volume;
				Changed = true;
				actor.Send($"This speaker's default audio output is now {OutputVolume.DescribeEnum().ColourValue()}.");
				return true;
			case "siblings":
			case "sibling":
				AcceptSiblingSources = !AcceptSiblingSources;
				Changed = true;
				actor.Send($"This speaker will {(AcceptSiblingSources ? "now".ColourValue() : "no longer".ColourError())} accept a sibling source.");
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("mediaspeaker", true,
			(gameworld, account) => new MediaSpeakerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("media speaker", false,
			(gameworld, account) => new MediaSpeakerGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("speaker", false,
			(gameworld, account) => new MediaSpeakerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Media Speaker",
			(proto, gameworld) => new MediaSpeakerGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("Media Speaker",
			$"Makes an item a powered {"[media speaker]".Colour(Telnet.BoldGreen)} for live and recorded audio",
			$"{BuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new MediaSpeakerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new MediaSpeakerGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new MediaSpeakerGameItemComponentProto(proto, gameworld));
	}
}
