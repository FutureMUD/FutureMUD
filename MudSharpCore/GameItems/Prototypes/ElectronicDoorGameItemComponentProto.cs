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

public class ElectronicDoorGameItemComponentProto : DoorGameItemComponentProtoBase
{
	private const string BaseDoorBuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3uninstallable <hinge side difficulty> <other side difficulty> <uninstall trait>#0 - sets the door as uninstallable
	#3uninstallable#0 - sets the door as not uninstallable by players
	#3smashable <difficulty>#0 - sets the door as smashable by players
	#3smashable#0 - sets the door as not smashable
	#3installed <keyword>#0 - sets the keyword for this door as viewed in exits (e.g. iron door)
	#3transparent#0 - sets the door as transparent
	#3opaque#0 - sets the door as opaque
	#3fire#0 - toggles whether the door can be fired through (e.g. gate)
	#3openable#0 - toggles whether players can open this door with the OPEN/CLOSE commands";

	private const string SpecificBuildingHelpText = @"
	#3source <component>#0 - the signal source component prototype name or id whose default signal endpoint drives this door
	#3threshold <number>#0 - the numeric threshold used to determine when the door is commanded open
	#3invert#0 - toggles whether the door opens above or below the threshold
	#3openemote <emote>#0 - the emote shown when the door opens automatically. Use $0 for the item
	#3closeemote <emote>#0 - the emote shown when the door closes automatically. Use $0 for the item";

	private static readonly string CombinedBuildingHelpText =
		$@"{BaseDoorBuildingHelpText}{SpecificBuildingHelpText}";

	protected ElectronicDoorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Electronic Door")
	{
		SourceComponentId = 0L;
		SourceComponentName = string.Empty;
		SourceEndpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
		ActivationThreshold = 0.5;
		OpenWhenAboveThreshold = true;
		OpenEmoteNoActor = "@ slide|slides open";
		CloseEmoteNoActor = "@ slide|slides closed";
		CanBeOpenedByPlayers = false;
	}

	protected ElectronicDoorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public long SourceComponentId { get; protected set; }
	public string SourceComponentName { get; protected set; } = string.Empty;
	public string SourceEndpointKey { get; protected set; } = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public double ActivationThreshold { get; protected set; }
	public bool OpenWhenAboveThreshold { get; protected set; }
	public string OpenEmoteNoActor { get; protected set; } = string.Empty;
	public string CloseEmoteNoActor { get; protected set; } = string.Empty;
	public override string TypeDescription => "Electronic Door";

	protected override void LoadFromXml(XElement root)
	{
		LoadDoorPrototypeData(root);
		SourceComponentId = long.TryParse(root.Element("SourceComponentId")?.Value, out var sourceId) ? sourceId : 0L;
		SourceComponentName = root.Element("SourceComponentName")?.Value ?? string.Empty;
		SourceEndpointKey = SignalComponentUtilities.NormaliseSignalEndpointKey(root.Element("SourceEndpointKey")?.Value);
		ActivationThreshold = double.Parse(root.Element("ActivationThreshold")?.Value ?? "0.5");
		OpenWhenAboveThreshold = bool.Parse(root.Element("OpenWhenAboveThreshold")?.Value ?? "true");
		OpenEmoteNoActor = root.Element("OpenEmoteNoActor")?.Value ?? "@ slide|slides open";
		CloseEmoteNoActor = root.Element("CloseEmoteNoActor")?.Value ?? "@ slide|slides closed";
	}

	protected override string SaveToXml()
	{
		return SaveDoorPrototypeData(new XElement("Definition",
			new XElement("SourceComponentId", SourceComponentId),
			new XElement("SourceComponentName", new XCData(SourceComponentName)),
			new XElement("SourceEndpointKey", new XCData(SourceEndpointKey)),
			new XElement("ActivationThreshold", ActivationThreshold),
			new XElement("OpenWhenAboveThreshold", OpenWhenAboveThreshold),
			new XElement("OpenEmoteNoActor", new XCData(OpenEmoteNoActor)),
			new XElement("CloseEmoteNoActor", new XCData(CloseEmoteNoActor))
		)).ToString();
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

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
			case "openemote":
			case "openecho":
				return BuildingCommandOpenEmote(actor, command);
			case "closeemote":
			case "closeecho":
				return BuildingCommandCloseEmote(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandSource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which signal source component prototype should drive this electronic door?");
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
			$"This electronic door is now driven by the {SourceEndpointKey.ColourCommand()} endpoint on signal source component prototype {SourceComponentName.ColourName()} (#{SourceComponentId.ToString("N0", actor)}).");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric threshold should determine when this door is commanded open?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number.");
			return false;
		}

		ActivationThreshold = value;
		Changed = true;
		actor.Send(
			$"This electronic door now uses a threshold of {ActivationThreshold.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInvert(ICharacter actor)
	{
		OpenWhenAboveThreshold = !OpenWhenAboveThreshold;
		Changed = true;
		actor.Send(
			$"This electronic door is now {(OpenWhenAboveThreshold ? "commanded open above or equal to the threshold".ColourValue() : "commanded open below the threshold".ColourValue())}.");
		return true;
	}

	private bool BuildingCommandOpenEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote should be shown when this door opens automatically? Use @ for the item.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		OpenEmoteNoActor = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This electronic door now echoes {OpenEmoteNoActor.ColourCommand()} when it opens.");
		return true;
	}

	private bool BuildingCommandCloseEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote should be shown when this door closes automatically? Use @ for the item.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		CloseEmoteNoActor = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This electronic door now echoes {CloseEmoteNoActor.ColourCommand()} when it closes.");
		return true;
	}

	public override bool CanSubmit()
	{
		return (SourceComponentId > 0 || !string.IsNullOrWhiteSpace(SourceComponentName)) && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (SourceComponentId <= 0 && string.IsNullOrWhiteSpace(SourceComponentName))
		{
			return "You must specify a signal source component prototype for this electronic door.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Electronic Door Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\n{DescribeDoorCharacteristics(actor, true)}\nSource Endpoint: {SignalComponentUtilities.DescribeSignalComponent(Gameworld, SourceComponentId, SourceComponentName, SourceEndpointKey).ColourName()} (#{SourceComponentId.ToString("N0", actor)})\nThreshold: {ActivationThreshold.ToString("N2", actor).ColourValue()}\nMode: {(OpenWhenAboveThreshold ? "Opens at or above threshold".ColourValue() : "Opens below threshold".ColourValue())}\nOpen Emote: {OpenEmoteNoActor.ColourCommand()}\nClose Emote: {CloseEmoteNoActor.ColourCommand()}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("electronicdoor", true,
			(gameworld, account) => new ElectronicDoorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("electronic door", false,
			(gameworld, account) => new ElectronicDoorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Electronic Door",
			(proto, gameworld) => new ElectronicDoorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ElectronicDoor",
			$"A {"[door]".Colour(Telnet.Yellow)} {SignalComponentUtilities.SignalConsumerTag} with built-in signal-driven opening and closing behaviour",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ElectronicDoorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ElectronicDoorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ElectronicDoorGameItemComponentProto(proto, gameworld));
	}
}
