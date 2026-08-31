#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class PushToTalkMicrophoneGameItemComponentProto : PoweredMachineBaseGameItemComponentProto,
	IPushToTalkMicrophonePrototype
{
	private const string SpecificBuildingHelpText = @"
	#3endpoint <key>#0 - sets the stable local audio output endpoint key
	#3ports <count>#0 - sets the number of local media output ports
	#3premote <emote>#0 - sets the player-facing transmit premote";

	public PushToTalkMicrophoneGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Push To Talk Microphone")
	{
		EndpointKey = "microphone";
		OutputPorts = 1;
		TransmitPremote = "$0 press|presses the transmit control on $1.";
		Wattage = 1.0;
		WattageDiscountPerQuality = 0.05;
	}

	protected PushToTalkMicrophoneGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string EndpointKey { get; protected set; } = "microphone";
	public int OutputPorts { get; protected set; }
	public string TransmitPremote { get; protected set; } = string.Empty;
	public override string TypeDescription => "Push To Talk Microphone";
	protected override string ComponentDescriptionOLCByline => "This item is a powered push-to-talk media microphone";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Endpoint: {EndpointKey.ColourCommand()}\nOutput Ports: {OutputPorts.ToString("N0", actor).ColourValue()}\nTransmit Premote: {TransmitPremote.ColourCommand()}";
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		EndpointKey = root.Element("EndpointKey")?.Value?.Trim() ?? "microphone";
		OutputPorts = int.TryParse(root.Element("OutputPorts")?.Value, out var ports) ? Math.Max(1, ports) : 1;
		TransmitPremote = root.Element("TransmitPremote")?.Value ?? string.Empty;
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("EndpointKey", EndpointKey));
		root.Add(new XElement("OutputPorts", OutputPorts));
		root.Add(new XElement("TransmitPremote", new XCData(TransmitPremote)));
		return root;
	}

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "endpoint":
				return BuildingCommandEndpoint(actor, command);
			case "ports":
			case "outputports":
				return BuildingCommandPorts(actor, command);
			case "premote":
			case "transmitpremote":
				if (command.IsFinished)
				{
					actor.Send("What premote should be used when a character transmits with this microphone?");
					return false;
				}

				TransmitPremote = command.SafeRemainingArgument;
				Changed = true;
				actor.Send($"This microphone's transmit premote is now {TransmitPremote.ColourCommand()}.");
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
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
		actor.Send($"This microphone's media endpoint is now {EndpointKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandPorts(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var ports) || ports < 1)
		{
			actor.Send("How many local media output ports should this microphone have?");
			return false;
		}

		OutputPorts = ports;
		Changed = true;
		actor.Send($"This microphone now has {OutputPorts.ToString("N0", actor).ColourValue()} local media output ports.");
		return true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("pttmicrophone", true,
			(gameworld, account) => new PushToTalkMicrophoneGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("push to talk microphone", false,
			(gameworld, account) => new PushToTalkMicrophoneGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("microphone", false,
			(gameworld, account) => new PushToTalkMicrophoneGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Push To Talk Microphone",
			(proto, gameworld) => new PushToTalkMicrophoneGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Push To Talk Microphone",
			$"Makes an item a powered {"[push-to-talk microphone]".Colour(Telnet.BoldGreen)} compatible with transmit and transmitwith",
			$"{BuildingHelpText}{SpecificBuildingHelpText}");
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new PushToTalkMicrophoneGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PushToTalkMicrophoneGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PushToTalkMicrophoneGameItemComponentProto(proto, gameworld));
	}
}
