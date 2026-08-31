#nullable enable

using MudSharp.Accounts;
using MudSharp.Computers;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class CameraGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IMediaCameraPrototype
{
	private const string SpecificBuildingHelpText = @"
	#3capabilities <audio|video|av>#0 - sets the media carried by this camera
	#3sensitivity <illumination>#0 - sets the minimum illumination required for video capture
	#3interval <seconds>#0 - sets the scene snapshot interval (five seconds or more)
	#3endpoint <key>#0 - sets the stable local output endpoint key
	#3ports <count>#0 - sets the number of local media output ports";

	public CameraGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Camera")
	{
		Capabilities = MediaCapabilities.Video;
		SensorSensitivity = 1.0;
		SnapshotInterval = TimeSpan.FromSeconds(5);
		EndpointKey = "camera";
		OutputPorts = 1;
		Wattage = 8.0;
		WattageDiscountPerQuality = 0.25;
	}

	protected CameraGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public MediaCapabilities Capabilities { get; protected set; }
	public double SensorSensitivity { get; protected set; }
	public TimeSpan SnapshotInterval { get; protected set; }
	public string EndpointKey { get; protected set; } = "camera";
	public int OutputPorts { get; protected set; }
	public override string TypeDescription => "Camera";
	protected override string ComponentDescriptionOLCByline => "This item is a powered camera media source";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Capabilities: {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}\n" +
		       $"Minimum Illumination: {SensorSensitivity.ToString("N2", actor).ColourValue()}\n" +
		       $"Snapshot Interval: {SnapshotInterval.Describe(actor).ColourValue()}\n" +
		       $"Endpoint: {EndpointKey.ColourCommand()}\n" +
		       $"Output Ports: {OutputPorts.ToString("N0", actor).ColourValue()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		Capabilities = Enum.TryParse<MediaCapabilities>(root.Element("Capabilities")?.Value, true, out var capabilities)
			? capabilities
			: MediaCapabilities.Video;
		SensorSensitivity = double.TryParse(root.Element("SensorSensitivity")?.Value, out var sensitivity)
			? Math.Max(0.0, sensitivity)
			: 1.0;
		var intervalMilliseconds = long.TryParse(root.Element("SnapshotIntervalMilliseconds")?.Value,
			out var parsedInterval)
			? parsedInterval
			: 5000L;
		SnapshotInterval = TimeSpan.FromMilliseconds(Math.Max(5000L, intervalMilliseconds));
		EndpointKey = root.Element("EndpointKey")?.Value?.Trim() ?? "camera";
		if (string.IsNullOrWhiteSpace(EndpointKey))
		{
			EndpointKey = "camera";
		}

		OutputPorts = int.TryParse(root.Element("OutputPorts")?.Value, out var ports) ? Math.Max(1, ports) : 1;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("Capabilities", Capabilities));
		root.Add(new XElement("SensorSensitivity", SensorSensitivity));
		root.Add(new XElement("SnapshotIntervalMilliseconds", (long)SnapshotInterval.TotalMilliseconds));
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
				return BuildingCommandCapabilities(actor, command);
			case "sensitivity":
			case "illumination":
				return BuildingCommandSensitivity(actor, command);
			case "interval":
			case "snapshotinterval":
				return BuildingCommandInterval(actor, command);
			case "endpoint":
				return BuildingCommandEndpoint(actor, command);
			case "ports":
			case "outputports":
				return BuildingCommandPorts(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandCapabilities(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !MediaComponentUtilities.TryParseCapabilities(command.PopSpeech(), out var capabilities))
		{
			actor.Send("Choose audio, video or av for this camera.");
			return false;
		}

		Capabilities = capabilities;
		Changed = true;
		actor.Send($"This camera now captures {MediaComponentUtilities.DescribeCapabilities(Capabilities).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandSensitivity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var sensitivity) || sensitivity < 0.0)
		{
			actor.Send("What non-negative illumination threshold should this camera require?");
			return false;
		}

		SensorSensitivity = sensitivity;
		Changed = true;
		actor.Send($"This camera now requires {SensorSensitivity.ToString("N2", actor).ColourValue()} illumination for video capture.");
		return true;
	}

	private bool BuildingCommandInterval(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var seconds) || seconds < 5.0)
		{
			actor.Send("Snapshot intervals must be at least five seconds.");
			return false;
		}

		SnapshotInterval = TimeSpan.FromSeconds(seconds);
		Changed = true;
		actor.Send($"This camera will capture a scene snapshot every {SnapshotInterval.Describe(actor).ColourValue()} while consumed.");
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
		actor.Send($"This camera's media endpoint is now {EndpointKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPorts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var ports) || ports < 1)
		{
			actor.Send("How many local media output ports should this camera have?");
			return false;
		}

		OutputPorts = ports;
		Changed = true;
		actor.Send($"This camera now has {OutputPorts.ToString("N0", actor).ColourValue()} local media output ports.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("camera", true,
			(gameworld, account) => new CameraGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Camera",
			(proto, gameworld) => new CameraGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Camera",
			$"Makes an item a powered {"[media camera]".Colour(Telnet.BoldGreen)} source for local video or A/V routing",
			$"{BuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new CameraGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CameraGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new CameraGameItemComponentProto(proto, gameworld));
	}
}
