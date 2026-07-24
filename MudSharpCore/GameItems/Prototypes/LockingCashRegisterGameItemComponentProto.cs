using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class LockingCashRegisterGameItemComponentProto : CashRegisterGameItemComponentProto,
	IHaveSimpleLockType, ILockablePrototype, ILockPrototype
{
	protected LockingCashRegisterGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "LockingCashRegister")
	{
	}

	protected LockingCashRegisterGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "LockingCashRegister";
	public Difficulty ForceDifficulty { get; private set; } = Difficulty.Normal;
	public Difficulty PickDifficulty { get; private set; } = Difficulty.Normal;
	public string LockEmote { get; private set; } = "@ lock|locks $1$?2| with $2||$";
	public string UnlockEmote { get; private set; } = "@ unlock|unlocks $1$?2| with $2||$";
	public string LockEmoteNoActor { get; private set; } = "$0 click|clicks locked.";
	public string UnlockEmoteNoActor { get; private set; } = "$0 click|clicks unlocked.";
	public string LockType { get; private set; } = "Lever Lock";

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		ForceDifficulty = (Difficulty)int.Parse(root.Element("ForceDifficulty")?.Value ?? "5");
		PickDifficulty = (Difficulty)int.Parse(root.Element("PickDifficulty")?.Value ?? "5");
		LockEmote = root.Element("LockEmote")?.Value ?? LockEmote;
		UnlockEmote = root.Element("UnlockEmote")?.Value ?? UnlockEmote;
		LockEmoteNoActor = root.Element("LockEmoteNoActor")?.Value ?? LockEmoteNoActor;
		UnlockEmoteNoActor = root.Element("UnlockEmoteNoActor")?.Value ?? UnlockEmoteNoActor;
		LockType = root.Element("LockType")?.Value ?? LockType;
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(
			new XElement("ForceDifficulty", (int)ForceDifficulty),
			new XElement("PickDifficulty", (int)PickDifficulty),
			new XElement("LockEmote", new XCData(LockEmote)),
			new XElement("UnlockEmote", new XCData(UnlockEmote)),
			new XElement("LockEmoteNoActor", new XCData(LockEmoteNoActor)),
			new XElement("UnlockEmoteNoActor", new XCData(UnlockEmoteNoActor)),
			new XElement("LockType", LockType)
		);
		return root.ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new LockingCashRegisterGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new LockingCashRegisterGameItemComponent(component, this, parent);
	}

	public static new void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("lockingcashregister", true,
			(gameworld, account) => new LockingCashRegisterGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("LockingCashRegister",
			(proto, gameworld) => new LockingCashRegisterGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("LockingCashRegister",
			$"A shop {"[cash register]".Colour(Telnet.BoldGreen)} with a built-in {"[lock]".Colour(Telnet.Yellow)}",
			BuildingHelpText);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new LockingCashRegisterGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"""
		You can use the following options with this component:

			#3name <name>#0 - sets the name of the component
			#3desc <desc>#0 - sets the description of the component
			#3weight <amount>#0 - sets its capacity
			#3size <size>#0 - sets its maximum contained item size
			#3type <type>#0 - sets the type of key accepted
			#3pick <difficulty>#0 - sets the lock-picking difficulty
			#3force <difficulty>#0 - sets the forcing difficulty
			#3lock <emote>#0 - sets the actor/key locking emote
			#3unlock <emote>#0 - sets the actor/key unlocking emote
			#3locknoactor <emote>#0 - sets the prog-driven locking emote
			#3unlocknoactor <emote>#0 - sets the prog-driven unlocking emote
		""";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "type":
			case "locktype":
				return BuildingCommandLockType(actor, command);
			case "pick":
				return BuildingCommandDifficulty(actor, command, true);
			case "force":
				return BuildingCommandDifficulty(actor, command, false);
			case "lock":
				return BuildingCommandEmote(actor, command, true, false);
			case "unlock":
				return BuildingCommandEmote(actor, command, false, false);
			case "locknoactor":
				return BuildingCommandEmote(actor, command, true, true);
			case "unlocknoactor":
				return BuildingCommandEmote(actor, command, false, true);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandLockType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What lock type should keys for this till use?");
			return false;
		}

		LockType = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"This till now accepts keys of type {LockType.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command, bool pick)
	{
		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send($"That is not a valid difficulty. The valid difficulties are {Enum.GetValues<Difficulty>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (pick)
		{
			PickDifficulty = difficulty;
		}
		else
		{
			ForceDifficulty = difficulty;
		}

		Changed = true;
		actor.OutputHandler.Send($"The {(pick ? "picking" : "forcing")} difficulty is now {difficulty.DescribeEnum().ColourName()}.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command, bool locking, bool noActor)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should be used?");
			return false;
		}

		var text = command.SafeRemainingArgument;
		var emote = noActor
			? new Emote(text, actor, actor)
			: new Emote(text, actor, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		if (locking && noActor)
		{
			LockEmoteNoActor = text;
		}
		else if (locking)
		{
			LockEmote = text;
		}
		else if (noActor)
		{
			UnlockEmoteNoActor = text;
		}
		else
		{
			UnlockEmote = text;
		}

		Changed = true;
		actor.OutputHandler.Send($"The emote is now {text.ColourCommand()}.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $"{base.ComponentDescriptionOLC(actor)}\nIt has a {LockType.ColourName()} lock which is {PickDifficulty.DescribeEnum().ColourName()} to pick and {ForceDifficulty.DescribeEnum().ColourName()} to force.";
	}
}
