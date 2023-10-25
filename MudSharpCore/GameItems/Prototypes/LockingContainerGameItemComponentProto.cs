using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes
{
    public class LockingContainerGameItemComponentProto : GameItemComponentProto
    {
        public Difficulty ForceDifficulty { get; private set; } = Difficulty.Normal;
        public Difficulty PickDifficulty { get; private set; } = Difficulty.Normal;
        public string LockEmote { get; private set; } = "@ lock|locks $1$?2| with $2||$";
        public string UnlockEmote { get; private set; } = "@ unlock|unlocks $1$?2| with $2||$";
        public string LockEmoteNoActor { get; private set; } = "@ lock|locks";
        public string UnlockEmoteNoActor { get; private set; } = "@ unlock|unlocks";
        public string LockType { get; private set; } = "Lever Lock";
        /// <summary>
        ///     The total allowable weight that can be contained by this container
        /// </summary>
        public double WeightLimit { get; protected set; } = 1000.0;

        /// <summary>
        ///     The maximum SizeCategory of item that may be contained by this container
        /// </summary>
        public SizeCategory MaximumContentsSize { get; protected set; } = SizeCategory.Small;

        /// <summary>
        ///     Usually either "in" or "on"
        /// </summary>
        public string ContentsPreposition { get; protected set; } = "in";

        public bool Transparent { get; protected set; } = false;

        public override bool WarnBeforePurge => true;
        public override string TypeDescription => "LockingContainer";

        #region Constructors
        protected LockingContainerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "LockingContainer")
        {
        }

        protected LockingContainerGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
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

            element = root.Element("LockType");
            if (element != null)
            {
                LockType = element.Value;
            }

            var attr = root.Attribute("Weight");
            if (attr != null)
            {
                WeightLimit = double.Parse(attr.Value);
            }

            attr = root.Attribute("MaxSize");
            if (attr != null)
            {
                MaximumContentsSize = (SizeCategory)int.Parse(attr.Value);
            }

            attr = root.Attribute("Preposition");
            if (attr != null)
            {
                ContentsPreposition = attr.Value;
            }

            attr = root.Attribute("Transparent");
            if (attr != null)
            {
                Transparent = bool.Parse(attr.Value);
            }
        }
        #endregion

        #region Saving
        protected override string SaveToXml()
        {
            return new XElement("Definition",
                    new XAttribute("Weight", WeightLimit),
                    new XAttribute("MaxSize", (int)MaximumContentsSize),
                    new XAttribute("Preposition", ContentsPreposition),
                    new XAttribute("Transparent", Transparent),
                    new XElement("ForceDifficulty", (int)ForceDifficulty),
                    new XElement("PickDifficulty", (int)PickDifficulty),
                    new XElement("LockEmote", new XCData(LockEmote)),
                    new XElement("UnlockEmote", new XCData(UnlockEmote)),
                    new XElement("LockEmoteNoActor", new XCData(LockEmoteNoActor)),
                    new XElement("UnlockEmoteNoActor", new XCData(UnlockEmoteNoActor)),
                    new XElement("LockType", LockType ?? "")
                ).ToString();
        }
        #endregion

        #region Component Instance Initialising Functions
        public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
        {
            return new LockingContainerGameItemComponent(this, parent, temporary);
        }

        public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
        {
            return new LockingContainerGameItemComponent(component, this, parent);
        }
        #endregion

        #region Initialisation Tasks
        public static void RegisterComponentInitialiser(GameItemComponentManager manager)
        {
            manager.AddBuilderLoader("LockingContainer".ToLowerInvariant(), true, (gameworld, account) => new LockingContainerGameItemComponentProto(gameworld, account));
            manager.AddDatabaseLoader("LockingContainer", (proto, gameworld) => new LockingContainerGameItemComponentProto(proto, gameworld));
            manager.AddTypeHelpInfo(
                "LockingContainer",
                $"Makes an item into a {"[container]".Colour(Telnet.BoldGreen)} with a built-in {"[lock]".Colour(Telnet.Yellow)}",
                BuildingHelpText
            );
        }

        public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
        {
            return CreateNewRevision(initiator, (proto, gameworld) => new LockingContainerGameItemComponentProto(proto, gameworld));
        }
        #endregion

        #region Building Commands

        private const string BuildingHelpText =
        @"You can use the following options with this component:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3size <max size>#0 - sets the maximum size of the objects that can be put in this container
	#3weight#0 - sets the maximum weight of items this container can hold
	#3transparent#0 - toggles whether you can see the contents when closed
	#3preposition <on|in|etc>#0 - sets the preposition used to display contents. Usually on or in.
    #3type <type>#0 - sets the lock type that is used to match keys.
	#3pick <difficulty>#0 - sets the difficulty to pick the lock on this container.
	#3force <difficulty>#0 - sets the difficulty to forcibly open the lock on this container
	#3lock <emote>#0 - sets the emote when locked. $0 is locker, $1 is container, $2 is key.
	#3unlock <emote>#0 - sets the emote when unlocked. $0 is locker, $1 is container, $2 is key.
	#3olock <emote>#0 - sets the emote for the other side of a container when locked. $0 is locker, $1 is container, $2 is key.
	#3ounlock <emote>#0 - sets the emote for the other side of a container when unlocked. $0 is locker, $1 is container, $2 is key.
	#3locknoactor <emote>#0 - sets the emote when locked by prog. $0 is the container.
	#3unlocknoactor <emote>#0 - sets the emote when unlocked by prog. $0 is the container.";
        public override string ShowBuildingHelp => BuildingHelpText;

        public override bool BuildingCommand(ICharacter actor, StringStack command)
        {
            switch (command.PopSpeech().ToLowerInvariant().CollapseString())
            {
                case "capacity":
                case "weight":
                case "weight limit":
                case "weight capacity":
                case "limit":
                    return BuildingCommand_WeightLimit(actor, command);
                case "maximum size":
                case "max size":
                case "maxsize":
                case "size":
                    return BuildingCommand_MaxSize(actor, command);
                case "preposition":
                    return BuildingCommand_Preposition(actor, command);
                case "transparent":
                    return BuildingCommand_Transparent(actor, command);
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
                default:
                    return base.BuildingCommand(actor, command);
            }
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
            actor.Send("You set the unlock emote for this container to {0}", UnlockEmote.Fullstop().Colour(Telnet.Yellow));
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
            actor.Send("You set the lock emote for this container to {0}", LockEmote.Fullstop().Colour(Telnet.Yellow));
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
            actor.Send("It will now be {0} to force this container's lock open.", ForceDifficulty.DescribeColoured());
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
            actor.Send("It will now be {0} to pick this container.", PickDifficulty.DescribeColoured());
            Changed = true;
            return true;
        }

        private bool BuildingCommand_Transparent(ICharacter actor, StringStack command)
        {
            Transparent = !Transparent;
            actor.Send("This container is {0} transparent.", Transparent ? "now" : "no longer");
            Changed = true;
            return true;
        }

        private bool BuildingCommand_WeightLimit(ICharacter actor, StringStack command)
        {
            var weightCmd = command.SafeRemainingArgument;
            var result = actor.Gameworld.UnitManager.GetBaseUnits(weightCmd, UnitType.Mass, out var success);
            if (success)
            {
                WeightLimit = result;
                actor.OutputHandler.Send(
                    $"This container will now hold {actor.Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor).ColourValue()}.");
                return true;
            }

            actor.OutputHandler.Send("That is not a valid weight.");
            return false;
        }

        private bool BuildingCommand_MaxSize(ICharacter actor, StringStack command)
        {
            var cmd = command.PopSpeech().ToLowerInvariant();
            if (cmd.Length == 0)
            {
                actor.OutputHandler.Send("What size do you want to set the limit for this component to?");
                return false;
            }

            var size = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>().ToList();
            SizeCategory target;
            if (size.Any(x => x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal)))
            {
                target = size.FirstOrDefault(x =>
                    x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
            }
            else
            {
                actor.OutputHandler.Send("That is not a valid item size. See SHOW ITEMSIZES for a correct list.");
                return false;
            }

            MaximumContentsSize = target;
            Changed = true;
            actor.OutputHandler.Send("This container will now only take items of up to size \"" + target.Describe() +
                                     "\".");
            return true;
        }

        private bool BuildingCommand_Preposition(ICharacter actor, StringStack command)
        {
            var preposition = command.Pop().ToLowerInvariant();
            if (string.IsNullOrEmpty(preposition))
            {
                actor.OutputHandler.Send("What preposition do you want to use for this container?");
                return false;
            }

            ContentsPreposition = preposition;
            Changed = true;
            actor.OutputHandler.Send("The contents of this container will now be described as \"" + ContentsPreposition +
                                     "\" it.");
            return true;
        }
		#endregion

		public override string ComponentDescriptionOLC(ICharacter actor)
		{
			return $@"{"Locking Container Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

This item can contain {Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor)} and up to {MaximumContentsSize.ToString().Colour(Telnet.Cyan)} size objects. 
It {(Transparent ? "is" : "is not")} transparent
It also contains a built-in lock and is difficulty {PickDifficulty.DescribeColoured()} to pick or {ForceDifficulty.DescribeColoured()} to force open. It takes keys of type {(LockType != null ? LockType.Colour(Telnet.Green) : "None".Colour(Telnet.Red))}.

It uses the following emotes:

Lock: {LockEmote.Colour(Telnet.Yellow)}
Unlock: {UnlockEmote.Colour(Telnet.Yellow)}
Lock (No Actor): {LockEmoteNoActor.Colour(Telnet.Yellow)}
Unlock (No Actor): {UnlockEmoteNoActor.Colour(Telnet.Yellow)}";
		}
	}
}
