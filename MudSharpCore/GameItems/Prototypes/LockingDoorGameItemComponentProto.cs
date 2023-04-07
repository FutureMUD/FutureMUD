using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class LockingDoorGameItemComponentProto : GameItemComponentProto, IHaveSimpleLockType
{
	public override string TypeDescription => "LockingDoor";

	/// <summary>
	///     A short string, designed to be parsed for characteristics, which appears in brackets after an exit description in
	///     rooms.
	///     e.g. heavy iron door
	/// </summary>
	public string InstalledExitDescription { get; protected set; }

	/// <summary>
	///     Whether this door permits people to see through it
	/// </summary>
	public bool SeeThrough { get; protected set; }

	public bool CanPlayersUninstall { get; protected set; }
	public bool CanPlayersSmash { get; protected set; }
	public Difficulty UninstallDifficultyHingeSide { get; protected set; }
	public Difficulty UninstallDifficultyNotHingeSide { get; protected set; }
	public Difficulty SmashDifficulty { get; protected set; }
	public ITraitDefinition UninstallTrait { get; protected set; }
	public bool CanFireThrough { get; protected set; }
	public Difficulty ForceDifficulty { get; private set; }
	public Difficulty PickDifficulty { get; private set; }
	public string LockEmote { get; private set; }
	public string UnlockEmote { get; private set; }
	public string LockEmoteNoActor { get; private set; }
	public string UnlockEmoteNoActor { get; private set; }
	public string LockEmoteOtherSide { get; private set; }
	public string UnlockEmoteOtherSide { get; private set; }
	public string LockType { get; private set; }

	#region Constructors

	protected LockingDoorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"LockingDoor")
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
		CanPlayersUninstall = false;
		CanPlayersSmash = false;
		UninstallDifficultyHingeSide = Difficulty.Impossible;
		UninstallDifficultyNotHingeSide = Difficulty.Impossible;
		SmashDifficulty = Difficulty.Impossible;
		Changed = true;
	}

	protected LockingDoorGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto,
		gameworld)
	{
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

		element = root.Element("LockType");
		if (element != null)
		{
			LockType = element.Value;
		}

		var attr = root.Attribute("SeeThrough");
		if (attr != null)
		{
			SeeThrough = Convert.ToBoolean(attr.Value);
		}

		attr = root.Attribute("CanFireThrough");
		if (attr != null)
		{
			CanFireThrough = bool.Parse(attr.Value);
		}

		element = root.Element("InstalledExitDescription");
		if (element != null)
		{
			InstalledExitDescription = element.Value;
		}

		element = root.Element("Uninstall");
		if (element == null)
		{
			CanPlayersUninstall = true;
			UninstallDifficultyHingeSide = Difficulty.Normal;
			UninstallDifficultyNotHingeSide = Difficulty.ExtremelyHard;
		}
		else
		{
			attr = element.Attribute("CanPlayersUninstall");
			if (attr != null)
			{
				CanPlayersUninstall = bool.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallDifficultyHingeSide");
			if (attr != null)
			{
				UninstallDifficultyHingeSide = (Difficulty)int.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallDifficultyNotHingeSide");
			if (attr != null)
			{
				UninstallDifficultyNotHingeSide = (Difficulty)int.Parse(attr.Value);
			}

			attr = element.Attribute("UninstallTrait");
			if (attr != null)
			{
				UninstallTrait = Gameworld.Traits.Get(long.Parse(attr.Value));
			}
		}

		element = root.Element("Smash");
		if (element == null)
		{
			CanPlayersSmash = true;
			SmashDifficulty = Difficulty.Normal;
		}
		else
		{
			attr = element.Attribute("CanPlayersSmash");
			if (attr != null)
			{
				CanPlayersSmash = bool.Parse(attr.Value);
			}

			attr = element.Attribute("SmashDifficulty");
			if (attr != null)
			{
				SmashDifficulty = (Difficulty)int.Parse(attr.Value);
			}
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ForceDifficulty", (int)ForceDifficulty),
			new XElement("PickDifficulty", (int)PickDifficulty),
			new XElement("LockEmote", new XCData(LockEmote)),
			new XElement("UnlockEmote", new XCData(UnlockEmote)),
			new XElement("LockEmoteNoActor", new XCData(LockEmoteNoActor)),
			new XElement("UnlockEmoteNoActor", new XCData(UnlockEmoteNoActor)),
			new XElement("LockEmoteOtherSide", new XCData(LockEmoteOtherSide)),
			new XElement("UnlockEmoteOtherSide", new XCData(UnlockEmoteOtherSide)),
			new XElement("LockType", LockType ?? ""),
			new XAttribute("SeeThrough", SeeThrough),
			new XAttribute("CanFireThrough", CanFireThrough),
			new XElement("InstalledExitDescription",
				!string.IsNullOrWhiteSpace(InstalledExitDescription) ? InstalledExitDescription : "door"),
			new XElement("Uninstall", new XAttribute("CanPlayersUninstall", CanPlayersUninstall),
				new XAttribute("UninstallDifficultyHingeSide", (int)UninstallDifficultyHingeSide),
				new XAttribute("UninstallDifficultyNotHingeSide", (int)UninstallDifficultyNotHingeSide),
				new XAttribute("UninstallTrait", UninstallTrait?.Id ?? 0)),
			new XElement("Smash", new XAttribute("CanPlayersSmash", CanPlayersSmash),
				new XAttribute("SmashDifficulty", (int)SmashDifficulty))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LockingDoorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new LockingDoorGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("LockingDoor".ToLowerInvariant(), true,
			(gameworld, account) => new LockingDoorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("LockingDoor",
			(proto, gameworld) => new LockingDoorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"LockingDoor",
			$"Turns the item into a {"[door]".Colour(Telnet.Yellow)} that is also a {"[lock]".Colour(Telnet.Yellow)} opened with a {"[key]".Colour(Telnet.Yellow)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new LockingDoorGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	type <type> - sets the lock type that is used to match keys.
	pick <difficulty> - sets the difficulty to pick the lock on this door.
	force <difficulty> - sets the difficulty to forcibly open the lock on this door
	lock <emote> - sets the emote when locked. $0 is locker, $1 is door, $2 is key.
	unlock <emote> - sets the emote when unlocked. $0 is locker, $1 is door, $2 is key.
	olock <emote> - sets the emote for the other side of a door when locked. $0 is locker, $1 is door, $2 is key.
	ounlock <emote> - sets the emote for the other side of a door when unlocked. $0 is locker, $1 is door, $2 is key.
	locknoactor <emote> - sets the emote when locked by prog. $0 is the door.
	unlocknoactor <emote> - sets the emote when unlocked by prog. $0 is the door.
    uninstallable <hinge side difficulty> <other side difficulty> <uninstall trait> - sets the door as uninstallable
	uninstallable - sets the door as not uninstallable by players
	smashable <difficulty> - sets the door as smashable by players
	smashable - sets the door as not smashable
	installed <keyword> - sets the keyword for this door as viewed in exits (e.g. iron door)
	transparent - sets the door as transparent
	opaque - sets the door as opaque
	fire - toggles whether the door can be fired through (e.g. gate)";

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
				return BuildingCommandLocktype(actor, command);
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
				return BuildingCommand_Uninstallable(actor, command);
			case "smashable":
				return BuildingCommand_Smashable(actor, command);
			case "installed description":
			case "installed":
			case "installed_description":
			case "exit_description":
			case "exit description":
			case "exitdesc":
			case "exit":
				return BuildingCommand_InstalledExitDescription(actor, command);
			case "see through":
			case "seethrough":
			case "transparent":
			case "opaque":
				return BuildingCommand_SeeThrough(actor, command);
			case "fire":
				return BuildingCommand_Fire(actor, command);
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

	private bool BuildingCommand_Uninstallable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (!CanPlayersUninstall)
			{
				actor.Send(
					"This door component is already not removable by players. If you want to make it removable you must specify additional arguments.");
				return false;
			}

			CanPlayersUninstall = false;
			Changed = true;
			actor.Send("This door is no longer removable by players.");
			return true;
		}

		var hingeDifficultyText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.Send(
				"What difficulty do you want players on the non-hinge side of the door to have when removing this door?");
			return false;
		}

		var nonHingeDifficultyText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.Send("What trait will players use to remove this door?");
			return false;
		}

		var traitText = command.PopSpeech();

		if (!CheckExtensions.GetDifficulty(hingeDifficultyText, out var hingeDifficulty))
		{
			actor.OutputHandler.Send($"The text {hingeDifficultyText.ColourCommand()} is not a valid difficulty.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(nonHingeDifficultyText, out var nonHingeDifficulty))
		{
			actor.OutputHandler.Send($"The text {nonHingeDifficultyText.ColourCommand()} is not a valid difficulty.");
			return false;
		}

		var trait = Gameworld.Traits.GetByIdOrName(traitText);
		if (trait == null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		CanPlayersUninstall = true;
		UninstallDifficultyHingeSide = hingeDifficulty;
		UninstallDifficultyNotHingeSide = nonHingeDifficulty;
		UninstallTrait = trait;
		Changed = true;
		actor.Send(
			"This door will now be removeable by players with the {0} trait at difficulties {1} (hinge) and {2} (non-hinge).",
			trait.Name.TitleCase().Colour(Telnet.Green),
			hingeDifficulty.DescribeColoured(),
			nonHingeDifficulty.DescribeColoured()
		);
		return true;
	}

	private bool BuildingCommand_Smashable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (!CanPlayersSmash)
			{
				actor.Send(
					"This door component is already not smashable by players. If you want to make it smashable you must specify additional arguments.");
				return false;
			}

			CanPlayersSmash = false;
			Changed = true;
			actor.Send("This door is no longer smashable by players.");
			return true;
		}

		if (command.IsFinished)
		{
			actor.Send("What difficulty do you want players to have when smashing this door?");
			return false;
		}

		var difficultyText = command.PopSpeech();

		if (!CheckExtensions.GetDifficulty(difficultyText, out var difficulty))
		{
			actor.Send($"The text {difficultyText.ColourCommand()} is not a valid difficulty.");
			return false;
		}

		CanPlayersSmash = true;
		SmashDifficulty = difficulty;
		Changed = true;
		actor.Send("This door will now be smashable by players at a difficulty of {0}.",
			difficulty.DescribeColoured());
		return true;
	}

	private bool BuildingCommand_InstalledExitDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the installed exit description to?");
			return false;
		}

		InstalledExitDescription = command.SafeRemainingArgument.Trim();
		actor.OutputHandler.Send(
			$"You set the Installed Exit Description for this door to {InstalledExitDescription.ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_SeeThrough(ICharacter actor, StringStack command)
	{
		switch (command.Last.ToLowerInvariant())
		{
			case "transparent":
				SeeThrough = true;
				break;
			case "opaque":
				SeeThrough = false;
				break;
			default:
				switch (command.PopSpeech().ToLowerInvariant())
				{
					case "true":
						SeeThrough = true;
						break;
					case "false":
						SeeThrough = false;
						break;
					default:
						actor.OutputHandler.Send("That is not a valid option for the door's see-through property.");
						return false;
				}

				break;
		}

		Changed = true;
		actor.OutputHandler.Send($"The door is now {(SeeThrough ? "transparent" : "opaque")}.");
		return true;
	}

	private bool BuildingCommand_Fire(ICharacter actor, StringStack command)
	{
		CanFireThrough = !CanFireThrough;
		actor.Send(CanFireThrough
			? "You can now fire ranged weapons through this component when it is closed."
			: "You can no longer fire ranged weapons through this component when it is closed.");
		Changed = true;
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is {4} door, and when installed in an exit will show as {5}. {6}. {7}. {8}.\nIt also contains a built-in lock and is difficulty {9} to pick or {10} to force open. It takes keys of type {17}.\n\nIt uses the following emotes:\nLock: {11}\nUnlock: {12}\nLock (other side): {13}\nUnlock (other side): {14}\nLock (No Actor): {15}\nUnlock (No Actor): {16}",
			"LockingDoor Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			SeeThrough ? "a transparent" : "an opaque",
			InstalledExitDescription.Colour(Telnet.Yellow),
			CanPlayersUninstall
				? $"It can be removed by players at a difficulty of {UninstallDifficultyHingeSide.DescribeColoured()} (hinge) / {UninstallDifficultyNotHingeSide.DescribeColoured()} (non-hinge) with the {(UninstallTrait != null ? UninstallTrait.Name.TitleCase().Colour(Telnet.Green) : "None")} skill"
				: "It cannot be removed by players",
			CanPlayersSmash
				? $"It can be smashed by players at a difficulty of {SmashDifficulty.DescribeColoured()}"
				: "It cannot be smashed by players",
			CanFireThrough ? "It can be fired through when closed" : "It cannot be fired through when closed",
			PickDifficulty.DescribeColoured(),
			ForceDifficulty.DescribeColoured(),
			LockEmote.Colour(Telnet.Yellow),
			UnlockEmote.Colour(Telnet.Yellow),
			LockEmoteOtherSide.Colour(Telnet.Yellow),
			UnlockEmoteOtherSide.Colour(Telnet.Yellow),
			LockEmoteNoActor.Colour(Telnet.Yellow),
			UnlockEmoteNoActor.Colour(Telnet.Yellow),
			LockType != null ? LockType.Colour(Telnet.Green) : "None".Colour(Telnet.Red)
		);
	}
}