#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class DigitalMediaRecorderGameItemComponentProto : ComputerHostGameItemComponentProto,
	IDigitalMediaRecorderPrototype, IComputerMediaInterfacePrototype
{
	private const string RecorderHelp = @"
	#3capabilities <audio|video|av>#0 - sets the recorded media types
	#3endpoint <key>#0 - sets the stable media endpoint key
	#3input <name>#0 - sets the player-facing input name
	#3output <name>#0 - sets the player-facing output name";

	public DigitalMediaRecorderGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: this(gameworld, originator, "Digital Media Recorder")
	{
	}

	protected DigitalMediaRecorderGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type)
		: base(gameworld, originator, type)
	{
		Capabilities = MediaCapabilities.Audio | MediaCapabilities.Video;
		EndpointKey = "recorder";
		InputName = "camera";
		OutputName = "playback";
		StoragePorts = 0;
		TerminalPorts = 0;
		NetworkPorts = 0;
	}

	protected DigitalMediaRecorderGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	public MediaCapabilities Capabilities { get; protected set; }
	public string EndpointKey { get; protected set; } = "recorder";
	public string InputName { get; protected set; } = "camera";
	public string OutputName { get; protected set; } = "playback";
	public override string TypeDescription => "Digital Media Recorder";
	protected override string ComponentDescriptionOLCByline => "This item records digital media to internal storage";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"{base.ComponentDescriptionOLCAddendum(actor)}\nCapabilities: {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}\nInput: {InputName.ColourCommand()}\nOutput: {OutputName.ColourCommand()}\nEndpoint: {EndpointKey.ColourCommand()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		Capabilities = Enum.TryParse<MediaCapabilities>(root.Element("Capabilities")?.Value, true, out var value) &&
		               value != MediaCapabilities.None ? value : MediaCapabilities.Audio | MediaCapabilities.Video;
		EndpointKey = root.Element("EndpointKey")?.Value ?? "recorder";
		InputName = root.Element("InputName")?.Value ?? "camera";
		OutputName = root.Element("OutputName")?.Value ?? "playback";
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		base.SaveSubtypeToXml(root);
		root.Add(new XElement("Capabilities", Capabilities), new XElement("EndpointKey", EndpointKey),
			new XElement("InputName", InputName), new XElement("OutputName", OutputName));
		return root;
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{RecorderHelp}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopForSwitch();
		switch (verb)
		{
			case "capabilities":
			case "capability":
				if (command.IsFinished || !MediaComponentUtilities.TryParseCapabilities(command.PopSpeech(), out var capabilities))
				{
					actor.Send("Choose audio, video or av for this recorder.");
					return false;
				}
				Capabilities = capabilities;
				Changed = true;
				actor.Send($"This recorder now supports {MediaComponentUtilities.DescribeCapabilities(capabilities).ColourValue()} media.");
				return true;
			case "endpoint":
				return SetKey(actor, command, "endpoint", value => EndpointKey = value);
			case "input":
				return SetKey(actor, command, "input", value => InputName = value);
			case "output":
				return SetKey(actor, command, "output", value => OutputName = value);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool SetKey(ICharacter actor, StringStack command, string label, Action<string> setter)
	{
		var value = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsWhiteSpace))
		{
			actor.Send($"The {label} must be a single non-empty word.");
			return false;
		}
		setter(value);
		Changed = true;
		actor.Send($"This recorder's {label} is now {value.ColourCommand()}.");
		return true;
	}

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("digitalmediarecorder", true,
			(gameworld, account) => new DigitalMediaRecorderGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("digital recorder", false,
			(gameworld, account) => new DigitalMediaRecorderGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Digital Media Recorder",
			(proto, gameworld) => new DigitalMediaRecorderGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Digital Media Recorder",
			$"Makes an item a powered {"[digital media recorder]".Colour(Telnet.BoldGreen)} with byte-limited internal storage",
			$"{ShowBuildingHelpStatic}{RecorderHelp}");
	}

	private const string ShowBuildingHelpStatic = "Use the normal powered-computer capacity and power options.";

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false) =>
		new DigitalMediaRecorderGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) =>
		new DigitalMediaRecorderGameItemComponent(component, this, parent);

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) =>
		CreateNewRevision(initiator, (proto, gameworld) => new DigitalMediaRecorderGameItemComponentProto(proto, gameworld));
}
