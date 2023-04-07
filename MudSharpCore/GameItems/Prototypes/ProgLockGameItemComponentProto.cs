using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class ProgLockGameItemComponentProto : GameItemComponentProto, IHaveSimpleLockType
{
	public Difficulty ForceDifficulty { get; private set; }
	public Difficulty PickDifficulty { get; private set; }
	public string LockEmoteNoActor { get; private set; }
	public string UnlockEmoteNoActor { get; private set; }
	public string LockEmoteOtherSide { get; private set; }
	public string UnlockEmoteOtherSide { get; private set; }
	public string LockType { get; private set; }
	public override string TypeDescription => "Prog Lock";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("plock", true,
			(gameworld, account) => new ProgLockGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("proglock", false,
			(gameworld, account) => new ProgLockGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Prog Lock",
			(proto, gameworld) => new ProgLockGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ProgLock",
			$"A kind of {"[lock]".Colour(Telnet.Yellow)} that can only be locked/unlocked via progs",
			BuildingHelpText
		);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}\n\nThis item is a programmable lock, and is difficulty {3} to pick or {4} to force open. It takes keys of type {9}.\n\nIt uses the following emotes:\nLock (other side): {5}\nUnlock (other side): {6}\nLock (No Actor): {7}\nUnlock (No Actor): {8}",
			"Programmable Lock Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			PickDifficulty.Describe().Colour(Telnet.Green),
			ForceDifficulty.Describe().Colour(Telnet.Green),
			LockEmoteOtherSide.Colour(Telnet.Yellow),
			UnlockEmoteOtherSide.Colour(Telnet.Yellow),
			LockEmoteNoActor.Colour(Telnet.Yellow),
			UnlockEmoteNoActor.Colour(Telnet.Yellow),
			LockType != null ? LockType.Colour(Telnet.Green) : "None".Colour(Telnet.Red)
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

		element = root.Element("LockType");
		if (element != null)
		{
			LockType = element.Value;
		}
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
			new XElement("LockType", LockType ?? "")
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ProgLockGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ProgLockGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ProgLockGameItemComponentProto(proto, gameworld));
	}

	#region Constructors

	protected ProgLockGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected ProgLockGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Prog Lock")
	{
		ForceDifficulty = Difficulty.Normal;
		PickDifficulty = Difficulty.Normal;
		LockEmoteNoActor = "$0$?1| on $1||$ lock|locks";
		UnlockEmoteNoActor = "$0$?1| on $1||$ unlock|unlocks";
		LockEmoteOtherSide = "$0$?3| on $3||$ is locked from the other side.";
		UnlockEmoteOtherSide = "$0$?3| on $3||$ is unlocked from the other side.";
		LockType = "Programmable";
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
			case "locknoactor":
				return BuildingCommandLockNoActorEmote(actor, command);
			case "unlock":
			case "unlock emote":
			case "unlocknoactor":
				return BuildingCommandUnlockNoActorEmote(actor, command);
			case "type":
			case "locktype":
			case "lock type":
				return BuildingCommandLocktype(actor, command);
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

	private bool BuildingCommandLocktype(ICharacter actor, StringStack command)
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
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), null);
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
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), null);
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
			new DummyPerceivable(), null);
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
			new DummyPerceivable(), null);
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

	private bool BuildingCommandForceDifficulty(ICharacter actor, StringStack command)
	{
		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.Send("That is not a valid difficulty.");
			return false;
		}

		ForceDifficulty = difficulty;
		actor.Send("It will now be {0} to force this lock open.", ForceDifficulty.DescribeColoured());
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
		actor.Send("It will now be {0} to pick this lock.", PickDifficulty.DescribeColoured());
		Changed = true;
		return true;
	}

	#endregion Building Commands
}