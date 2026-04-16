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
	private const string BuildingHelpText = @"You can use the following options with this component:
	All door options, plus:
	source <component> - the signal source component prototype name or id that drives this door
	threshold <number> - the numeric threshold used to determine when the door is commanded open
	invert - toggles whether the door opens above or below the threshold
	openemote <emote> - the emote shown when the door opens automatically. Use @ for the item
	closeemote <emote> - the emote shown when the door closes automatically. Use @ for the item";

	protected ElectronicDoorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Electronic Door")
	{
		SourceComponentId = 0L;
		SourceComponentName = string.Empty;
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
			new XElement("ActivationThreshold", ActivationThreshold),
			new XElement("OpenWhenAboveThreshold", OpenWhenAboveThreshold),
			new XElement("OpenEmoteNoActor", new XCData(OpenEmoteNoActor)),
			new XElement("CloseEmoteNoActor", new XCData(CloseEmoteNoActor))
		)).ToString();
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
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
			case "removable":
			case "uninstall":
			case "uninstallable":
				return BuildingCommandUninstallable(actor, command);
			case "smashable":
				return BuildingCommandSmashable(actor, command);
			case "installed description":
			case "installed":
			case "installed_description":
			case "exit_description":
			case "exit description":
			case "exitdesc":
			case "exit":
				return BuildingCommandInstalledExitDescription(actor, command);
			case "see through":
			case "seethrough":
			case "transparent":
			case "opaque":
				return BuildingCommandSeeThrough(actor, command);
			case "fire":
				return BuildingCommandFire(actor);
			case "open":
			case "openable":
			case "canbeopened":
			case "canopen":
				return BuildingCommandCanBeOpenedByPlayers(actor);
			default:
				return base.BuildingCommand(actor, command);
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
			    out var sourcePrototype))
		{
			actor.Send("There is no such item component prototype.");
			return false;
		}

		SourceComponentId = sourcePrototype.Id;
		SourceComponentName = sourcePrototype.Name;
		Changed = true;
		actor.Send(
			$"This electronic door is now driven by the signal source component prototype {SourceComponentName.ColourName()} (#{SourceComponentId.ToString("N0", actor)}).");
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
			$"{"Electronic Door Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\n{DescribeDoorCharacteristics(actor, true)}\nSource Component: {SignalComponentUtilities.DescribeSignalComponent(Gameworld, SourceComponentId, SourceComponentName).ColourName()} (#{SourceComponentId.ToString("N0", actor)})\nThreshold: {ActivationThreshold.ToString("N2", actor).ColourValue()}\nMode: {(OpenWhenAboveThreshold ? "Opens at or above threshold".ColourValue() : "Opens below threshold".ColourValue())}\nOpen Emote: {OpenEmoteNoActor.ColourCommand()}\nClose Emote: {CloseEmoteNoActor.ColourCommand()}";
	}

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("electronicdoor", true,
			(gameworld, account) => new ElectronicDoorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("electronic door", false,
			(gameworld, account) => new ElectronicDoorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Electronic Door",
			(proto, gameworld) => new ElectronicDoorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ElectronicDoor",
			$"A {"[door]".Colour(Telnet.Yellow)} with built-in signal-driven opening and closing behaviour",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
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
