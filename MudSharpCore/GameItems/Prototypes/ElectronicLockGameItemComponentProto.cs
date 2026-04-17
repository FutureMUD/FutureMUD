#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ElectronicLockGameItemComponentProto : ProgLockGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#3source <component>#0 - the signal source component prototype name or id whose default signal endpoint drives this lock
	#3threshold <number>#0 - the numeric threshold used to determine when the lock is active
	#3invert#0 - toggles whether the lock activates above or below the threshold";

	private const string CombinedBuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - renames the component
	#3desc <description>#0 - sets the description of the component
    #3type <type>#0 - sets the lock type that is used to match keys.
	#3pick <difficulty>#0 - sets the difficulty to pick the lock on this container.
	#3force <difficulty>#0 - sets the difficulty to forcibly open the lock on this container
	#3lock <emote>#0 - sets the emote when locked. $0 is locker, $1 is container, $2 is key.
	#3unlock <emote>#0 - sets the emote when unlocked. $0 is locker, $1 is container, $2 is key.
	#3olock <emote>#0 - sets the emote for the other side of a container when locked. $0 is locker, $1 is container, $2 is key.
	#3ounlock <emote>#0 - sets the emote for the other side of a container when unlocked. $0 is locker, $1 is container, $2 is key.
	#3locknoactor <emote>#0 - sets the emote when locked by prog. $0 is the container.
	#3unlocknoactor <emote>#0 - sets the emote when unlocked by prog. $0 is the container.
	#3source <component>#0 - the signal source component prototype name or id whose default signal endpoint drives this lock
	#3threshold <number>#0 - the numeric threshold used to determine when the lock is active
	#3invert#0 - toggles whether the lock activates above or below the threshold";

	protected ElectronicLockGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Electronic Lock")
	{
		SourceComponentId = 0L;
		SourceComponentName = string.Empty;
		SourceEndpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
		ActivationThreshold = 0.5;
		LockWhenAboveThreshold = true;
	}

	protected ElectronicLockGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public long SourceComponentId { get; protected set; }
	public string SourceComponentName { get; protected set; } = string.Empty;
	public string SourceEndpointKey { get; protected set; } = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public double ActivationThreshold { get; protected set; }
	public bool LockWhenAboveThreshold { get; protected set; }
	public override string TypeDescription => "Electronic Lock";

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		SourceComponentId = long.TryParse(root.Element("SourceComponentId")?.Value, out var sourceId) ? sourceId : 0L;
		SourceComponentName = root.Element("SourceComponentName")?.Value ?? string.Empty;
		SourceEndpointKey = SignalComponentUtilities.NormaliseSignalEndpointKey(root.Element("SourceEndpointKey")?.Value);
		ActivationThreshold = double.Parse(root.Element("ActivationThreshold")?.Value ?? "0.5");
		LockWhenAboveThreshold = bool.Parse(root.Element("LockWhenAboveThreshold")?.Value ?? "true");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ForceDifficulty", (int)ForceDifficulty),
			new XElement("PickDifficulty", (int)PickDifficulty),
			new XElement("LockEmoteNoActor", new XCData(LockEmoteNoActor)),
			new XElement("UnlockEmoteNoActor", new XCData(UnlockEmoteNoActor)),
			new XElement("LockEmoteOtherSide", new XCData(LockEmoteOtherSide)),
			new XElement("UnlockEmoteOtherSide", new XCData(UnlockEmoteOtherSide)),
			new XElement("LockType", LockType ?? string.Empty),
			new XElement("SourceComponentId", SourceComponentId),
			new XElement("SourceComponentName", new XCData(SourceComponentName)),
			new XElement("SourceEndpointKey", new XCData(SourceEndpointKey)),
			new XElement("ActivationThreshold", ActivationThreshold),
			new XElement("LockWhenAboveThreshold", LockWhenAboveThreshold)
		).ToString();
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
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandSource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which signal source component prototype should drive this electronic lock?");
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
			$"This electronic lock is now driven by the {SourceEndpointKey.ColourCommand()} endpoint on signal source component prototype {SourceComponentName.ColourName()} (#{SourceComponentId.ToString("N0", actor)}).");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What numeric threshold should determine when this lock is active?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter a valid number.");
			return false;
		}

		ActivationThreshold = value;
		Changed = true;
		actor.Send($"This electronic lock now uses a threshold of {ActivationThreshold.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandInvert(ICharacter actor)
	{
		LockWhenAboveThreshold = !LockWhenAboveThreshold;
		Changed = true;
		actor.Send(
			$"This electronic lock is now {(LockWhenAboveThreshold ? "active above or equal to the threshold".ColourValue() : "active below the threshold".ColourValue())}.");
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
			return "You must specify a signal source component prototype for this lock.";
		}

		return base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Electronic Lock Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis is a signal-driven lock using keys of type {LockType.ColourValue()}.\nSource Endpoint: {SignalComponentUtilities.DescribeSignalComponent(Gameworld, SourceComponentId, SourceComponentName, SourceEndpointKey).ColourName()} (#{SourceComponentId.ToString("N0", actor)})\nThreshold: {ActivationThreshold.ToString("N2", actor).ColourValue()}\nMode: {(LockWhenAboveThreshold ? "Locks at or above threshold".ColourValue() : "Locks below threshold".ColourValue())}\nLock (No Actor): {LockEmoteNoActor.ColourCommand()}\nUnlock (No Actor): {UnlockEmoteNoActor.ColourCommand()}";
	}

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("electroniclock", true,
			(gameworld, account) => new ElectronicLockGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("electronic lock", false,
			(gameworld, account) => new ElectronicLockGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Electronic Lock",
			(proto, gameworld) => new ElectronicLockGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ElectronicLock",
			$"A {"[lock]".Colour(Telnet.Yellow)} that responds automatically to a sibling signal source component",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ElectronicLockGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ElectronicLockGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ElectronicLockGameItemComponentProto(proto, gameworld));
	}
}
