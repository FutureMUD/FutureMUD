using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class LockingDoorGameItemComponentProto : DoorGameItemComponentProtoBase, IHaveSimpleLockType
{
	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3type <type>#0 - sets the lock type that is used to match keys.
	#3pick <difficulty>#0 - sets the difficulty to pick the lock on this door.
	#3force <difficulty>#0 - sets the difficulty to forcibly open the lock on this door
	#3lock <emote>#0 - sets the emote when locked. $0 is locker, $1 is door, $2 is key.
	#3unlock <emote>#0 - sets the emote when unlocked. $0 is locker, $1 is door, $2 is key.
	#3olock <emote>#0 - sets the emote for the other side of a door when locked. $0 is locker, $1 is door, $2 is key.
	#3ounlock <emote>#0 - sets the emote for the other side of a door when unlocked. $0 is locker, $1 is door, $2 is key.
	#3locknoactor <emote>#0 - sets the emote when locked by prog. $0 is the door.
	#3unlocknoactor <emote>#0 - sets the emote when unlocked by prog. $0 is the door.
	#3uninstallable <hinge side difficulty> <other side difficulty> <uninstall trait>#0 - sets the door as uninstallable
	#3uninstallable#0 - sets the door as not uninstallable by players
	#3smashable <difficulty>#0 - sets the door as smashable by players
	#3smashable#0 - sets the door as not smashable
	#3installed <keyword>#0 - sets the keyword for this door as viewed in exits (e.g. iron door)
	#3transparent#0 - sets the door as transparent
	#3opaque#0 - sets the door as opaque
	#3fire#0 - toggles whether the door can be fired through (e.g. gate)";

	public override string TypeDescription => "LockingDoor";

	public Difficulty ForceDifficulty { get; private set; }
	public Difficulty PickDifficulty { get; private set; }
	public string LockEmote { get; private set; } = string.Empty;
	public string UnlockEmote { get; private set; } = string.Empty;
	public string LockEmoteNoActor { get; private set; } = string.Empty;
	public string UnlockEmoteNoActor { get; private set; } = string.Empty;
	public string LockEmoteOtherSide { get; private set; } = string.Empty;
	public string UnlockEmoteOtherSide { get; private set; } = string.Empty;
	public string LockType { get; private set; } = string.Empty;

	protected LockingDoorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "LockingDoor")
	{
		ForceDifficulty = Difficulty.Normal;
		PickDifficulty = Difficulty.Normal;
		LockEmote = "@ lock|locks $1$?2| with $2||$";
		UnlockEmote = "@ unlock|unlocks $1$?2| with $2||$";
		LockEmoteNoActor = "@ lock|locks";
		UnlockEmoteNoActor = "@ unlock|unlocks";
		LockEmoteOtherSide = "$0 is locked from the other side.";
		UnlockEmoteOtherSide = "$0 is unlocked from the other side.";
		LockType = "Lever Lock";
	}

	protected LockingDoorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		LoadDoorPrototypeData(root);
		var element = root.Element("ForceDifficulty");
		if (element is not null)
		{
			ForceDifficulty = (Difficulty)int.Parse(element.Value);
		}

		element = root.Element("PickDifficulty");
		if (element is not null)
		{
			PickDifficulty = (Difficulty)int.Parse(element.Value);
		}

		LockEmote = root.Element("LockEmote")?.Value ?? LockEmote;
		UnlockEmote = root.Element("UnlockEmote")?.Value ?? UnlockEmote;
		LockEmoteNoActor = root.Element("LockEmoteNoActor")?.Value ?? LockEmoteNoActor;
		UnlockEmoteNoActor = root.Element("UnlockEmoteNoActor")?.Value ?? UnlockEmoteNoActor;
		LockEmoteOtherSide = root.Element("LockEmoteOtherSide")?.Value ?? LockEmoteOtherSide;
		UnlockEmoteOtherSide = root.Element("UnlockEmoteOtherSide")?.Value ?? UnlockEmoteOtherSide;
		LockType = root.Element("LockType")?.Value ?? LockType;
	}

	protected override string SaveToXml()
	{
		return SaveDoorPrototypeData(new XElement("Definition",
			new XElement("ForceDifficulty", (int)ForceDifficulty),
			new XElement("PickDifficulty", (int)PickDifficulty),
			new XElement("LockEmote", new XCData(LockEmote)),
			new XElement("UnlockEmote", new XCData(UnlockEmote)),
			new XElement("LockEmoteNoActor", new XCData(LockEmoteNoActor)),
			new XElement("UnlockEmoteNoActor", new XCData(UnlockEmoteNoActor)),
			new XElement("LockEmoteOtherSide", new XCData(LockEmoteOtherSide)),
			new XElement("UnlockEmoteOtherSide", new XCData(UnlockEmoteOtherSide)),
			new XElement("LockType", LockType ?? string.Empty)
		)).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LockingDoorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new LockingDoorGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("lockingdoor", true,
			(gameworld, account) => new LockingDoorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("LockingDoor",
			(proto, gameworld) => new LockingDoorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"LockingDoor",
			$"Turns the item into a {"[door]".Colour(Telnet.Yellow)} that is also a {"[lock]".Colour(Telnet.Yellow)} opened with a {"[key]".Colour(Telnet.Yellow)}",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new LockingDoorGameItemComponentProto(proto, gameworld));
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "pick":
			case "pick difficulty":
			case "pickdifficulty":
				return BuildingCommandPickDifficulty(actor, command);
			case "force":
			case "force difficulty":
			case "forcedifficulty":
				return BuildingCommandForceDifficulty(actor, command);
			case "lock":
			case "lock emote":
				return BuildingCommandLockEmote(actor, command);
			case "locknoactor":
				return BuildingCommandLockNoActorEmote(actor, command);
			case "unlocknoactor":
				return BuildingCommandUnlockNoActorEmote(actor, command);
			case "type":
			case "locktype":
			case "lock type":
				return BuildingCommandLockType(actor, command);
			case "unlock":
			case "unlock emote":
				return BuildingCommandUnlockEmote(actor, command);
			case "lock other emote":
			case "lock other":
			case "olock":
				return BuildingCommandLockOtherEmote(actor, command);
			case "unlock other emote":
			case "unlock other":
			case "ounlock":
				return BuildingCommandUnlockOtherEmote(actor, command);
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
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			@"{0} (#{1}r{2}, {3})

{4}
It also contains a built-in lock and is difficulty {5} to pick or {6} to force open. It takes keys of type {13}.

It uses the following emotes:

Lock: {7}
Unlock: {8}
Lock (other side): {9}
Unlock (other side): {10}
Lock (No Actor): {11}
Unlock (No Actor): {12}",
			"LockingDoor Game Item Component".Colour(Telnet.Cyan),
			Id.ToString("N0", actor),
			RevisionNumber.ToString("N0", actor),
			Name,
			DescribeDoorCharacteristics(actor, false),
			PickDifficulty.DescribeColoured(),
			ForceDifficulty.DescribeColoured(),
			LockEmote.Colour(Telnet.Yellow),
			UnlockEmote.Colour(Telnet.Yellow),
			LockEmoteOtherSide.Colour(Telnet.Yellow),
			UnlockEmoteOtherSide.Colour(Telnet.Yellow),
			LockEmoteNoActor.Colour(Telnet.Yellow),
			UnlockEmoteNoActor.Colour(Telnet.Yellow),
			(LockType ?? "None").Colour(Telnet.Green));
	}

	private bool BuildingCommandLockType(ICharacter actor, StringStack command)
	{
		var types = Gameworld.ItemProtos.SelectNotNull(x => x.GetItemType<IHaveSimpleLockType>())
			.Select(x => x.LockType)
			.Distinct()
			.ToList();

		if (command.IsFinished)
		{
			actor.Send("What type do you want to set this lock too?");
			if (types.Count > 0)
			{
				actor.Send(
					"Other locks and keys have the following lock types: {0}".ColourIncludingReset(Telnet.Yellow),
					types.Select(x => x.ColourValue()).ListToString());
			}

			return false;
		}

		LockType = command.PopSpeech().TitleCase();
		Changed = true;
		actor.Send("This lock now requires keys of type {0} to open.", LockType.ColourValue());
		if (!types.Contains(LockType))
		{
			actor.Send(
				"Warning: There are no other locks or keys with this lock type. Check that you actually intended to create a new locking scheme."
					.Colour(Telnet.Yellow));
		}

		return true;
	}

	private bool BuildingCommandUnlockNoActorEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to set the unlock (no actor) emote to?");
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Trim();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		UnlockEmoteNoActor = emoteText;
		Changed = true;
		actor.Send("You set the unlock (no actor) emote for this lock to {0}",
			UnlockEmoteNoActor.Fullstop().Colour(Telnet.Yellow));
		return true;
	}

	private bool BuildingCommandLockNoActorEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to set the lock (no actor) emote to?");
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Trim();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LockEmoteNoActor = emoteText;
		Changed = true;
		actor.Send("You set the lock (no actor) emote for this lock to {0}",
			LockEmoteNoActor.Fullstop().Colour(Telnet.Yellow));
		return true;
	}

	private bool BuildingCommandLockOtherEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to set the lock (other side) emote to?");
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Trim();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LockEmoteOtherSide = emoteText;
		Changed = true;
		actor.Send("You set the lock (other side) emote for this lock to {0}",
			LockEmoteOtherSide.Fullstop().Colour(Telnet.Yellow));
		return true;
	}

	private bool BuildingCommandUnlockOtherEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to set the unlock (other side) emote to?");
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Trim();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		UnlockEmoteOtherSide = emoteText;
		Changed = true;
		actor.Send("You set the unlock (other side) emote for this lock to {0}",
			UnlockEmoteOtherSide.Fullstop().Colour(Telnet.Yellow));
		return true;
	}

	private bool BuildingCommandUnlockEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to set the unlock emote to?");
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Trim();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		UnlockEmote = emoteText;
		Changed = true;
		actor.Send("You set the unlock emote for this door to {0}", UnlockEmote.Fullstop().Colour(Telnet.Yellow));
		return true;
	}

	private bool BuildingCommandLockEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to set the lock emote to?");
			return false;
		}

		var emoteText = command.SafeRemainingArgument.Trim();
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		LockEmote = emoteText;
		Changed = true;
		actor.Send("You set the lock emote for this door to {0}", LockEmote.Fullstop().Colour(Telnet.Yellow));
		return true;
	}

	private bool BuildingCommandForceDifficulty(ICharacter actor, StringStack command)
	{
		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.Send("That is not a valid difficulty.");
			return false;
		}

		ForceDifficulty = difficulty;
		actor.Send("It will now be {0} to force this door's lock open.", ForceDifficulty.DescribeColoured());
		Changed = true;
		return true;
	}

	private bool BuildingCommandPickDifficulty(ICharacter actor, StringStack command)
	{
		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.Send("That is not a valid difficulty.");
			return false;
		}

		PickDifficulty = difficulty;
		actor.Send("It will now be {0} to pick this door.", PickDifficulty.DescribeColoured());
		Changed = true;
		return true;
	}
}
