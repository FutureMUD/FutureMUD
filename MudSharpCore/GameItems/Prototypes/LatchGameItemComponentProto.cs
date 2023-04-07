using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class LatchGameItemComponentProto : GameItemComponentProto
{
	public Difficulty ForceDifficulty { get; private set; }
	public Difficulty PickDifficulty { get; private set; }
	public string LockEmote { get; private set; }
	public string UnlockEmote { get; private set; }
	public string LockEmoteNoActor { get; private set; }
	public string UnlockEmoteNoActor { get; private set; }
	public string LockEmoteOtherSide { get; private set; }
	public string UnlockEmoteOtherSide { get; private set; }
	public override string TypeDescription => "Latch";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("latch", true,
			(gameworld, account) => new LatchGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Latch", (proto, gameworld) => new LatchGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Latch",
			$"A kind of {"[lock]".Colour(Telnet.Yellow)} that can only be opened from one side",
			BuildingHelpText
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}\n\nThis item is a latch lock, and is difficulty {3} to pick or {4} to force open.\n\nIt uses the following emotes:\nLock: {5}\nUnlock: {6}\nLock (other side): {7}\nUnlock (other side): {8}\nLock (No Actor): {9}\nUnlock (No Actor): {10}",
			"Latch Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			PickDifficulty.Describe().Colour(Telnet.Green),
			ForceDifficulty.Describe().Colour(Telnet.Green),
			LockEmote.Colour(Telnet.Yellow),
			UnlockEmote.Colour(Telnet.Yellow),
			LockEmoteOtherSide.Colour(Telnet.Yellow),
			UnlockEmoteOtherSide.Colour(Telnet.Yellow),
			LockEmoteNoActor.Colour(Telnet.Yellow),
			UnlockEmoteNoActor.Colour(Telnet.Yellow)
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("ForceDifficulty");
		if (element != null)
		{
			ForceDifficulty = (Difficulty)int.Parse(element.Value);
		}

		element = root.Element("PickDifficulty");
		if (element != null)
		{
			PickDifficulty = (Difficulty)int.Parse(element.Value);
		}

		element = root.Element("LockEmote");
		if (element != null)
		{
			LockEmote = element.Value;
		}

		element = root.Element("UnlockEmote");
		if (element != null)
		{
			UnlockEmote = element.Value;
		}

		element = root.Element("LockEmoteNoActor");
		if (element != null)
		{
			LockEmoteNoActor = element.Value;
		}

		element = root.Element("UnlockEmoteNoActor");
		if (element != null)
		{
			UnlockEmoteNoActor = element.Value;
		}

		element = root.Element("LockEmoteOtherSide");
		if (element != null)
		{
			LockEmoteOtherSide = element.Value;
		}

		element = root.Element("UnlockEmoteOtherSide");
		if (element != null)
		{
			UnlockEmoteOtherSide = element.Value;
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ForceDifficulty", (int)ForceDifficulty),
			new XElement("PickDifficulty", (int)PickDifficulty),
			new XElement("LockEmote", new XCData(LockEmote)),
			new XElement("UnlockEmote", new XCData(UnlockEmote)),
			new XElement("LockEmoteNoActor", new XCData(LockEmote)),
			new XElement("UnlockEmoteNoActor", new XCData(UnlockEmote)),
			new XElement("LockEmoteOtherSide", new XCData(LockEmoteOtherSide)),
			new XElement("UnlockEmoteOtherSide", new XCData(UnlockEmoteOtherSide))
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LatchGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new LatchGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new LatchGameItemComponentProto(proto, gameworld));
	}

	#region Constructors

	protected LatchGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected LatchGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Latch")
	{
		ForceDifficulty = Difficulty.Normal;
		PickDifficulty = Difficulty.Normal;
		LockEmote = "@ latch|latches $1$?2| on $2||$";
		UnlockEmote = "@ unlatch|unlatches $1$?2| on $2||$";
		LockEmoteNoActor = "$0$?1| on $1||$ open|opens";
		UnlockEmoteNoActor = "$0$?1| on $1||$ close|closes";
		LockEmoteOtherSide = "$0$?1| on $1||$ is latched from the other side.";
		UnlockEmoteOtherSide = "$0$?1| on $1||$ is unlatched from the other side.";
		Changed = true;
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tpick <difficulty> - the difficulty to pick this lock\n\tforce <difficulty> - the difficulty to force this lock\n\tlock <emote> - sets the emote when locked. Use @ for locker, $0 for the lock item, and $1 for the item the lock is installed on\n\tunlock <emote> - sets the emote when unlocked. Use @ for unlocker, $0 for the lock item, and $1 for the item the lock is installed on\n\tolock <emote> - sets the other side emote when locked. Use @ for locker, $0 for the lock item, and $1 for the item the lock is installed on\n\tounlock <emote> - sets the other side emote when unlocked. Use @ for unlocker, $0 for the lock item, and $1 for the item the lock is installed on\n\tlocknoactor <emote> - sets the emote when locked without a person (e.g. prog). $0 is the lock\n\tunlocknoactor <emote> - sets the emote when unlocked without a person (e.g. prog). $0 is the lock";

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
			case "unlock":
			case "unlock emote":
				return BuildingCommandUnlockEmote(actor, command);
			case "locknoactor":
				return BuildingCommandLockNoActorEmote(actor, command);
			case "unlocknoactor":
				return BuildingCommandUnlockNoActorEmote(actor, command);
			case "lock other emote":
			case "lock other":
			case "olock":
				return BuildingCommandLockOtherEmote(actor, command);
			case "unlock other emote":
			case "unlock other":
			case "ounlock":
				return BuildingCommandUnlockOtherEmote(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandUnlockNoActorEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to set the unlock (no actor) emote to?");
			return false;
		}

		UnlockEmoteNoActor = command.RemainingArgument.Trim();
		Changed = true;
		actor.Send("You set the unlock (no actor) emote for this latch to {0}",
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

		LockEmoteNoActor = command.RemainingArgument.Trim();
		Changed = true;
		actor.Send("You set the lock (no actor) emote for this latch to {0}",
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

		LockEmoteOtherSide = command.RemainingArgument.Trim();
		Changed = true;
		actor.Send("You set the lock (other side) emote for this latch to {0}",
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

		UnlockEmoteOtherSide = command.RemainingArgument.Trim();
		Changed = true;
		actor.Send("You set the unlock (other side) emote for this latch to {0}",
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

		UnlockEmote = command.RemainingArgument.Trim();
		Changed = true;
		actor.Send("You set the unlock emote for this latch to {0}", UnlockEmote.Fullstop().Colour(Telnet.Yellow));
		return true;
	}

	private bool BuildingCommandLockEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to set the lock emote to?");
			return false;
		}

		LockEmote = command.RemainingArgument.Trim();
		Changed = true;
		actor.Send("You set the lock emote for this latch to {0}", LockEmote.Fullstop().Colour(Telnet.Yellow));
		return true;
	}

	private bool BuildingCommandForceDifficulty(ICharacter actor, StringStack command)
	{
		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.Send("That is not a valid difficulty.");
			return false;
		}

		ForceDifficulty = difficulty;
		actor.Send("It will now be {0} to force this lock open.", ForceDifficulty.Describe().Colour(Telnet.Green));
		Changed = true;
		return true;
	}

	private bool BuildingCommandPickDifficulty(ICharacter actor, StringStack command)
	{
		if (!CheckExtensions.GetDifficulty(command.PopSpeech(), out var difficulty))
		{
			actor.Send("That is not a valid difficulty.");
			return false;
		}

		PickDifficulty = difficulty;
		actor.Send("It will now be {0} to pick this lock.", PickDifficulty.Describe().Colour(Telnet.Green));
		Changed = true;
		return true;
	}

	#endregion Building Commands
}