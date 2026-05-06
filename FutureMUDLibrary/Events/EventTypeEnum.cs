using MudSharp.FutureProg;
using System.Threading.Tasks;

namespace MudSharp.Events
{
    /// <summary>
    ///     An enumerable representing which class of event is being fired
    /// </summary>
    public enum EventType
    {

        /// <summary>
        ///     Hooks the character who dropped the item. Parameters are character, item
        /// </summary>
        [EventInfo("Hooks a character dropping an item. Fires for the person who dropped the item only.", new[] { "character", "item" }, new[] { "dropper", "dropped" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        CharacterDroppedItem = 0,

        /// <summary>
        ///     Hooks any IHookable that witnessed an item being dropped. Parameteres are character, item, witness
        /// </summary>
        [EventInfo("Hooks a perceivable witnessing an item being dropped.", new[] { "character", "item", "perceivable" }, new[] { "dropper", "dropped", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable, })]
        CharacterDroppedItemWitness = 1,

        /// <summary>
        ///     Hooks an item that was dropped. Parameters are character, item
        /// </summary>
        [EventInfo("Hooks an item being dropped.", new[] { "character", "item" }, new[] { "dropper", "dropped" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        ItemDropped = 2,

        /// <summary>
        ///     Hooks the character who got the item from the ground. Parameters are character, item
        /// </summary>
        [EventInfo("Hooks a character getting an item from the ground. Fires for the person who got the item only.", new[] { "character", "item" }, new[] { "getter", "gotten" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        CharacterGotItem = 3,

        /// <summary>
        ///     Hooks any IHookable that witnesed an item being gotten from the ground. Parameters are character, item, witness
        /// </summary>
        [EventInfo("Hooks a perceivable witnessing an item being gotten from the ground.", new[] { "character", "item", "perceivable" }, new[] { "getter", "gotten", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable })]
        CharacterGotItemWitness = 4,

        /// <summary>
        ///     Hooks an item that was gotten from the ground. Parameters are character, item
        /// </summary>
        [EventInfo("Hooks an item being gotten from the ground.", new[] { "character", "item" }, new[] { "getter", "gotten" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        ItemGotten = 5,

        /// <summary>
        ///     Hooks the character who got the item from a container. Parameters are character, item, container
        /// </summary>
        [EventInfo("Hooks a character getting an item from a container. Fires for the person who got the item only.", new[] { "character", "item", "item" }, new[] { "getter", "gotten", "container" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, })]
        CharacterGotItemContainer = 6,

        /// <summary>
        ///     Hooks any IHookable that witnesed an item being gotten from a container. Parameters are character, item, container,
        ///     witness
        /// </summary>
        [EventInfo("Hooks a perceivable witnessing an item being gotten from a container.", new[] { "character", "item", "item", "perceivable" }, new[] { "getter", "gotten", "container", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable, })]
        CharacterGotItemContainerWitness = 7,

        /// <summary>
        ///     Hooks an item that was gotten from a container. Parameters are character, item, container
        /// </summary>
        [EventInfo("Hooks an item being gotten from a container.", new[] { "character", "item", "item" }, new[] { "getter", "gotten", "container" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, })]
        ItemGottenContainer = 8,


        /// <summary>
        ///     Hooks the character who put the item in a container. Parameters are character, item, container
        /// </summary>
        [EventInfo("Hooks a character putting an item in a container. Fires for the person who put the item only.", new[] { "character", "item", "item" }, new[] { "putter", "put", "container" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, })]
        CharacterPutItemContainer = 9,

        /// <summary>
        ///     Hooks any IHookable that witnesed an item being put in a container. Parameters are character, item, container,
        ///     witness
        /// </summary>
        [EventInfo("Hooks a perceivable witnessing an item being put in a container.", new[] { "character", "item", "item", "perceivable" }, new[] { "putter", "put", "container", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable, })]
        CharacterPutItemContainerWitness = 10,

        /// <summary>
        ///     Hooks an item that was put in a container. Parameters are character, item, container
        /// </summary>
        [EventInfo("Hooks an item being put in a container.", new[] { "character", "item", "item" }, new[] { "putter", "put", "container" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, })]
        ItemPutContainer = 11,


        /// <summary>
        ///     Hooks the character who is giving an item to another. Parameters are giver, receiver, item
        /// </summary>
        [EventInfo("Hooks an item being given to someone. Fires for giver only.", new[] { "character", "character", "item" }, new[] { "giver", "receiver", "given" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        CharacterGiveItemGiver = 12,

        /// <summary>
        ///     Hooks the character who is receiving an item from another. Parameters are giver, receiver, item
        /// </summary>
        [EventInfo("Hooks an item being given to someone. Fires for receiver only.", new[] { "character", "character", "item" }, new[] { "giver", "receiver", "given" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        CharacterGiveItemReceiver = 13,

        /// <summary>
        ///     Hooks the character who is witnessing an item being given. Parameters are giver, receiver, item, witness
        /// </summary>
        [EventInfo("Hooks a perceivable witnessing an item being given to someone.", new[] { "character", "character", "item", "perceivable" }, new[] { "giver", "receiver", "given", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable, })]
        CharacterGiveItemWitness = 14,

        /// <summary>
        ///     Hooks an item that was given to someone. Parameters are giver, receiver, item
        /// </summary>
        [EventInfo("Hooks an item being given to someone.", new[] { "character", "character", "item" }, new[] { "giver", "receiver", "given" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        ItemGiven = 15,

        /// <summary>
        ///     Hooks the character who sheathes an item. Parameters are character, item, sheathe
        /// </summary>
        [EventInfo("Hooks to a character sheathing their own item", new[] { "character", "item", "item" }, new[] { "sheather", "item", "sheathe" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item })]
        CharacterSheatheItem = 57,

        /// <summary>
        ///     Hooks the character who witnesses an item being sheathed. Parameters are character, item, sheath, witness
        /// </summary>
        [EventInfo("Hooks to a perceivable witnessing an item being sheathed", new[] { "character", "item", "item", "perceivable" }, new[] { "sheather", "item", "sheathe", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable })]
        CharacterSheatheItemWitness = 58,

        /// <summary>
        ///     Hooks the item that is sheathed. Parameters are character, item, sheath
        /// </summary>
        [EventInfo("Hooks to the item being put in a sheath", new[] { "character", "item", "item" }, new[] { "sheather", "item", "sheathe" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item })]
        ItemSheathed = 59,

        /// <summary>
        ///     Hooks the sheath that is receiving the sheathed item. Parameters are character, item, sheath
        /// </summary>
        [EventInfo("Hooks to a sheath getting an item put in it", new[] { "character", "item", "item" }, new[] { "sheather", "item", "sheathe" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Item })]
        ItemSheathItemSheathed = 60,


        /// <summary>
        ///     Hooks the character who moved, as they enter the room. Parameters are mover, cell, exit
        /// </summary>
        [EventInfo("Hooks to a character when they move into a room", new[] { "character", "location", "exit" }, new[] { "mover", "destination", "exit" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit })]
        CharacterEnterCell = 16,

        /// <summary>
        ///     Hooks any witness to the character who moved, as they enter the room. Parameters are mover, cell, exit, witness
        /// </summary>
        [EventInfo("Hooks to a perceiver witnessing someone move into a room", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "destination", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterEnterCellWitness = 17,

        /// <summary>
        ///     Hooks the character who moved, after movement is complete. Parameters are mover, cell, exit
        /// </summary>
        [EventInfo("Hooks to a character when they have moved into a room and their movement finishes", new[] { "character", "location", "exit" }, new[] { "mover", "destination", "exit" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit })]
        CharacterEnterCellFinish = 18,

        /// <summary>
        ///     Hooks any witness to the character who moved, after movement is complete. Parameters are mover, cell, exit, witness
        /// </summary>
        [EventInfo("Hooks to a perceiver witnessing someone finish move into a room", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "destination", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterEnterCellFinishWitness = 19,

        /// <summary>
        ///     Hooks the character who begun to move. Parameters are mover, cell, exit
        /// </summary>
        [EventInfo("Hooks to a character when they begin to move", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit })]
        CharacterBeginMovement = 21,

        /// <summary>
        ///     Hooks any character who witnessed the character begin to move. Parameters are mover, cell, exit, witness
        /// </summary>
        [EventInfo("Hooks to a perceiver witnessing someone begin to move", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "origin", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterBeginMovementWitness = 22,

        /// <summary>
        ///     Hooks the character who moved, as they leave the room. Parameters are mover, cell, exit
        /// </summary>
        [EventInfo("Hooks to a character when they leave the room they started in", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit })]
        CharacterLeaveCell = 23,

        /// <summary>
        ///     Hooks any witness to the character who moved, as they leave the room. Parameters are mover, cell, exit, witness
        /// </summary>
        [EventInfo("Hooks to a perceiver witnessing someone move out of a room", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "origin", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterLeaveCellWitness = 24,

        /// <summary>
        ///     Hooks a character when they stop moving from the STOP command. Parameters are mover, cell, exit
        /// </summary>
        [EventInfo("Hooks to a character when they stop moving with STOP command", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit })]
        CharacterStopMovement = 25,

        /// <summary>
        ///     Hooks any witness to the character when they stop moving from the STOP command. Parameters are mover, cell, exit,
        ///     witness
        /// </summary>
        [EventInfo("Hooks to a perceiver witnessing someone stop moving with the STOP command", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "origin", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterStopMovementWitness = 26,

        /// <summary>
        ///     Hooks a character when they stop moving from the door being closed. Parameters are mover, cell, exit
        /// </summary>
        [EventInfo("Hooks to a character when they stop moving because of a closed door", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit })]
        CharacterStopMovementClosedDoor = 27,

        /// <summary>
        ///     Hooks any witness to the character when they stop moving from the door being closed. Parameters are mover, cell,
        ///     exit, witness
        /// </summary>
        [EventInfo("Hooks to a perceiver witnessing someone stop moving because of a closed door", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "origin", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterStopMovementClosedDoorWitness = 28,

        /// <summary>
        ///     Hooks to trying to move but not being able to. Parameters are mover, cell, exit
        /// </summary>
        [EventInfo("Hooks to a character trying to move but being unable to", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit })]
        CharacterCannotMove = 45,

        /// <summary>
        ///     Hooks to a character entering the game for the first time
        /// </summary>
        [EventInfo("Fires once when a character or NPC logs in or loads in", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        CharacterEntersGame = 20,

        /// <summary>
        ///     Hooks to a character entering a command in the presence of items,characters (other than self),rooms BEFORE the
        ///     command is evaluated. If handled, does not evaluate the command. Parameters are character, thing, command,
        ///     (stringstack)arguments
        /// </summary>
        [EventInfo("Hooks to a character entering a command in the presence of items, character, rooms", new[] { "character", "perceivable", "text", "text collection" }, new[] { "ch", "thing", "command", "args" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Text, ProgVariableTypeCode.Text | ProgVariableTypeCode.Collection })]
        CommandInput = 34,

        /// <summary>
        ///     Hooks to the character themselves BEFORE the command is evaluated. If handled, does not evaluate the command.
        ///     Parameters are character, command, (stringstack)arguments
        /// </summary>
        [EventInfo("Hooks to a character entering a command on themselves", new[] { "character", "text", "text collection" }, new[] { "ch", "command", "args" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Text, ProgVariableTypeCode.Text | ProgVariableTypeCode.Collection })]
        SelfCommandInput = 35,


        /// <summary>
        ///     Hooks to any character who witnesses (but is not targeted by) a social. Parameters are socialite, social,
        ///     (list)targets, (cellexit)direction, witness
        /// </summary>
        [EventInfo("Hooks to a perceiver witnessing (but not targeted by) a social", new[] { "character", "text", "perceivable collection", "exit", "perceivable" }, new[] { "ch", "social", "targets", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Text, ProgVariableTypeCode.Perceivable | ProgVariableTypeCode.Collection, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterSocialWitness = 29,

        /// <summary>
        ///     Hooks to any character who is targeted by a social. Parameters are Parameters are socialite, social, target,
        ///     (cellexit)direction
        /// </summary>
        [EventInfo("Hooks to a perceivable being targeted by a social", new[] { "character", "text", "perceivable", "exit" }, new[] { "socialite", "social", "target", "exit" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Text, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Exit })]
        CharacterSocialTarget = 30,

        /// <summary>
        ///     Hooks to any character who is on the same side of a door when it is knocked on (except the person who knocked).
        ///     Parameters are knocker, cell, exit, witness
        /// </summary>
        [EventInfo("Hooks to a character on the same side of a door when it is knocked on", new[] { "character", "location", "exit", "perceivable" }, new[] { "knocker", "origin", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterDoorKnockedSameSide = 31,

        /// <summary>
        ///     Hooks to any character who is on the other side of a door when it is knocked on. Parameters are knocker, cell,
        ///     exit, witness
        /// </summary>
        [EventInfo("Hooks to a character on the other side of a door when it is knocked on", new[] { "character", "location", "exit", "perceivable" }, new[] { "knocker", "origin", "exit", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Perceivable })]
        CharacterDoorKnockedOtherSide = 32,

        /// <summary>
        ///     Hooks to the door itself when knocked upon. Parameters are knocker, cell, exit, door.Parent
        /// </summary>
        [EventInfo("Hooks to a door item when it is knocked on", new[] { "character", "location", "exit", "item" }, new[] { "knocker", "origin", "exit", "door" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Item })]
        DoorKnocked = 33,

        /// <summary>
        ///     Hooks to a character speaking. Parameters are character, volume, language, accent, text
        /// </summary>
        [EventInfo("Hooks to a character speaking", new[] { "character", "text", "language", "accent", "text" }, new[] { "character", "volume", "language", "accent", "message" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Text, ProgVariableTypeCode.Language, ProgVariableTypeCode.Accent, ProgVariableTypeCode.Text })]
        CharacterSpeaks = 39,

        /// <summary>
        ///     Hooks to a character witnessing speaking. Parameters are character, witness, volume, language, accent, text
        /// </summary>
        [EventInfo("Hooks to witnessing a character speaking", new[] { "character", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "witness", "volume", "language", "accent", "message" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Text, ProgVariableTypeCode.Language, ProgVariableTypeCode.Accent, ProgVariableTypeCode.Text })]
        CharacterSpeaksWitness = 40,

        /// <summary>
        ///     Hooks to a character speaking directly to someone. Parameters are character, target, volume, language, accent, text
        /// </summary>
        [EventInfo("Hooks to a character speaking directly to something", new[] { "character", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "target", "volume", "language", "accent", "message" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Text, ProgVariableTypeCode.Language, ProgVariableTypeCode.Accent, ProgVariableTypeCode.Text })]
        CharacterSpeaksDirect = 41,

        /// <summary>
        ///     Hooks to a character being spoken to directly by someone. Parameters are character, target, volume, language,
        ///     accent, text
        /// </summary>
        [EventInfo("Hooks to a the target of character speaking directly to something", new[] { "character", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "target", "volume", "language", "accent", "message" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Text, ProgVariableTypeCode.Language, ProgVariableTypeCode.Accent, ProgVariableTypeCode.Text })]
        CharacterSpeaksDirectTarget = 42,

        /// <summary>
        ///     Hooks to a character witnessing someone speaking directly to someone. Parameters are character, target, witness,
        ///     volume, language, accent, text
        /// </summary>
        [EventInfo("Hooks to a the witness of a character speaking directly to something", new[] { "character", "perceivable", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "target", "witness", "volume", "language", "accent", "message" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Text, ProgVariableTypeCode.Language, ProgVariableTypeCode.Accent, ProgVariableTypeCode.Text })]
        CharacterSpeaksDirectWitness = 43,

        /// <summary>
        ///     Hooks to a character witnessing speech from another room (usually from shouting). Parameters are character, target,
        ///     witness, volume, language, accent, text
        /// </summary>
        [EventInfo("Hooks to a the witness of a character speaking from another room (i.e. shouting)", new[] { "character", "perceivable", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "target", "witness", "volume", "language", "accent", "message" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Text, ProgVariableTypeCode.Language, ProgVariableTypeCode.Accent, ProgVariableTypeCode.Text })]
        CharacterSpeaksNearbyWitness = 44,

        /// <summary>
        ///     Hooks to any character who is eating something. Parameters are eater, food, bites, bitesremaining
        /// </summary>
        [EventInfo("Hooks to a character when they eat something", new[] { "character", "item", "number", "number" }, new[] { "character", "food", "bites", "remaining" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Number, ProgVariableTypeCode.Number })]
        CharacterEat = 36,

        /// <summary>
        ///     Hooks to any character who witnesses someone eating something. Parameters are eater, witness, food, bites,
        ///     bitesremaining
        /// </summary>
        [EventInfo("Hooks to someone witnessing a character eat something", new[] { "character", "perceivable", "item", "number", "number" }, new[] { "character", "witness", "food", "bites", "remaining" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Item, ProgVariableTypeCode.Number, ProgVariableTypeCode.Number })]
        CharacterEatWitness = 37,

        /// <summary>
        ///     Hooks to any item that is being eaten. Parameters are eater, food, bites, bitesremaining
        /// </summary>
        [EventInfo("Hooks to an item when it is being eaten", new[] { "character", "item", "number", "number" }, new[] { "character", "food", "bites", "remaining" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Number, ProgVariableTypeCode.Number })]
        ItemEaten = 38,

        /// <summary>
        ///     Hooks to a character swallowing something. Parameters are swallower, swallowed
        /// </summary>
        [EventInfo("Hooks to a character when they swallow something", new[] { "character", "item" }, new[] { "character", "swallowed" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        CharacterSwallow = 61,

        /// <summary>
        ///     Hooks to a perceiver witnessing a character swallowing something. Parameters are swallower, swallowed, witness
        /// </summary>
        [EventInfo("Hooks to someone witnessing a character swallow something", new[] { "character", "item", "perceivable" }, new[] { "character", "swallowed", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable })]
        CharacterSwallowWitness = 62,

        /// <summary>
        ///     Hooks to an item being swallowed. Parameters are swallower, swallowed
        /// </summary>
        [EventInfo("Hooks to an item being swallowed", new[] { "character", "item" }, new[] { "character", "swallowed" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item })]
        ItemSwallowed = 63,

        /// <summary>
        ///     Hooks to engaging an opponent in combat. Parameters are aggressor, target
        /// </summary>
        [EventInfo("Hooks to a character engaging an opponent in combat", new[] { "character", "perceiver" }, new[] { "aggressor", "target" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver })]
        EngageInCombat = 46,

        /// <summary>
        ///     Hooks to being engaged by an opponent in combat. Parameters are aggressor, target
        /// </summary>
        [EventInfo("Hooks to a target being engaging in combat", new[] { "character", "perceiver" }, new[] { "aggressor", "target" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver })]
        EngagedInCombat = 47,

        /// <summary>
        ///     Hooks to witnessing someone being engaged by an opponent in combat. Parameters are aggressor, target, witness
        /// </summary>
        [EventInfo("Hooks to someone witnessing a character engaging an opponent in combat", new[] { "character", "perceiver", "perceivable" }, new[] { "aggressor", "target", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver, ProgVariableTypeCode.Perceivable })]
        EngagedInCombatWitness = 48,

        /// <summary>
        ///     Hooks to someone starting to target the individual in a melee combat. Parameters are aggressor, target
        /// </summary>
        [EventInfo("Hooks to a character targeting an individual in melee combat", new[] { "character", "perceiver" }, new[] { "aggressor", "target" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver })]
        TargettedInCombat = 49,

        /// <summary>
        ///     Hooks to someone who was targeted no longer being targeted. Parameters are aggressor, target, newtarget
        /// </summary>
        [EventInfo("Hooks to a character no longer being targeted in combat", new[] { "character", "perceiver", "perceiver" }, new[] { "aggressor", "target", "newtarget" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver, ProgVariableTypeCode.Perceiver })]
        NoLongerTargettedInCombat = 50,

        /// <summary>
        ///     Hooks to someone leaving combat. Parameters are character.
        /// </summary>
        [EventInfo("Fires when a character leaves combat.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        LeaveCombat = 51,

        /// <summary>
        ///     Hooks to someone joining combat. Parameters are character.
        /// </summary>
        [EventInfo("Fires when a character joins combat.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        JoinCombat = 52,

        /// <summary>
        ///     Hooks to someone getting to the end of Combatant.AcquireTarget and having no natural target. Parameters are
        ///     character.
        /// </summary>
        [EventInfo("Fires when a character has no targets in combat and attempts to do something.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        NoNaturalTargets = 53,

        /// <summary>
        ///     Hooks to someone who was previously engaged in melee who is no longer. Parameters are character.
        /// </summary>
        [EventInfo("Fires when a character who was in melee combat is no longer in melee.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        NoLongerEngagedInMelee = 54,


        /// <summary>
        ///     Hooks to someone who has themselves bled. Parameters are character, bleeding
        /// </summary>
        [EventInfo("Fires when a character bleeds.", new[] { "character", "number" }, new[] { "person", "litres" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Number, })]
        BleedTick = 55,

        /// <summary>
        ///     Hooks to someone who has witnessed another bleed. Parameters are character, bleeding, witness
        /// </summary>
        [EventInfo("Fires when a perceivable witnesses a character bleeding.", new[] { "character", "number", "perceivable" }, new[] { "person", "litres", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Number, ProgVariableTypeCode.Perceivable, })]
        WitnessBleedTick = 56,


        /// <summary>
        /// Hooks to someone who has fired a gun which turned out to be empty. Parameters are character, target, gun
        /// </summary>
        [EventInfo("Fires when a character fires a gun that is empty.", new[] { "character", "character", "item" }, new[] { "person", "target", "gun" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, })]
        FireGunEmpty = 64,

        /// <summary>
        /// Hooks to someone who has tried to ready a gun which turned out to be empty. Parameters are character, gun.
        /// </summary>
        [EventInfo("Fires when a character readies a gun that is empty.", new[] { "character", "item" }, new[] { "person", "gun" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, })]
        ReadyGunEmpty = 65,

        /// <summary>
        /// Fires every 5 seconds. NPCs only. Parameters are character.
        /// </summary>
        [EventInfo("Fires every 5 seconds for characters.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        FiveSecondTick = 66,

        /// <summary>
        /// Fires every 10 seconds. NPCs only. Parameters are character.
        /// </summary>
        [EventInfo("Fires every 10 seconds for characters.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        TenSecondTick = 67,

        /// <summary>
        /// Fires every hour. NPCs only. Parameters are character.
        /// </summary>
        [EventInfo("Fires every 1 hour for characters.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        HourTick = 69,

        /// <summary>
        /// Fires every minute. NPCs only. Parameters are character.
        /// </summary>
        [EventInfo("Fires every 60 seconds for characters.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        MinuteTick = 68,

        /// <summary>
        /// Fires when a CommandDelay effect expires. Parameters are character, commands (string list)
        /// </summary>
        [EventInfo("Fires when a command delay effect expires.", new[] { "character", "text collection" }, new[] { "person", "commands" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Text | ProgVariableTypeCode.Collection })]
        CommandDelayExpired = 70,

        /// <summary>
        /// Fires when a combat target is critically injured. Parameters are character, target
        /// </summary>
        [EventInfo("Fires when a target is critically injured in combat.", new[] { "character", "character" }, new[] { "attacker", "target" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, })]
        TargetIncapacitated = 71,

        /// <summary>
        /// Fires when someone the target is fighting offers a truce. Parameters are character, target
        /// </summary>
        [EventInfo("Fires when a truce is offered in combat.", new[] { "character", "character" }, new[] { "trucer", "target" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, })]
        TruceOffered = 72,

        /// <summary>
        /// Fires when a combat target is killed in combat. Parameters are character, target
        /// </summary>
        [EventInfo("Fires when a target is killed in combat.", new[] { "character", "character" }, new[] { "killer", "victim" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, })]
        TargetSlain = 73,

        /// <summary>
        /// Fires when a target being aimed at becomes invalid. Parameters are character, target, weapon
        /// </summary>
        [EventInfo("Fires when a target being aimed at becomes invalid.", new[] { "character", "perceiver", "item" }, new[] { "shooter", "target", "weapon" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver, ProgVariableTypeCode.Item, })]
        LostAim = 74,

        /// <summary>
        /// Fires when a character uses the COMMANd command on another character. Parameters are commandee, commander, commandtext
        /// </summary>
        [EventInfo("Fires when a character use the COMMAND command on another character.", new[] { "character", "character", "text" }, new[] { "commandee", "commander", "commandtext" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Text })]
        CommandIssuedToCharacter = 75,

        /// <summary>
        /// Fires whenever weather changes for all perceivers and locations. Parameters are perceiver, oldweather, newweather
        /// </summary>
        [EventInfo("Fires on all perceivers and locations when weather changes.", new[] { "perceivable", "weatherevent", "weatherevent" }, new[] { "witness", "oldweather", "newweather" }, new[] { ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.WeatherEvent, ProgVariableTypeCode.WeatherEvent })]
        WeatherChanged = 76,

        /// <summary>
        /// Fires when a character buys something from a shop. Parameters are buyer, shop, merchandise, item collection
        /// </summary>
        [EventInfo("Fires when a character buys something from a shop.", new[] { "character", "shop", "merchandise", "item collection" }, new[] { "buyer", "shop", "merchandise", "actualitems" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Shop, ProgVariableTypeCode.Merchandise, ProgVariableTypeCode.Item | ProgVariableTypeCode.Collection })]
        BuyItemInShop = 77,

        /// <summary>
        /// Fires when a perceiver witnesses a character buying something from a shop. Parameters are buyer, witness, shop, merchandise, item collection
        /// </summary>
        [EventInfo("Fires when a character buys something from a shop.", new[] { "character", "perceiver", "shop", "merchandise", "item collection" }, new[] { "buyer", "witness", "shop", "merchandise", "actualitems" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver, ProgVariableTypeCode.Shop, ProgVariableTypeCode.Merchandise, ProgVariableTypeCode.Item | ProgVariableTypeCode.Collection })]
        WitnessBuyItemInShop = 78,

        /// <summary>
        /// Fires on a character when they have died. Parameters are character.
        /// </summary>
        [EventInfo("Fires on a character when they have died.", new[] { "character" }, new[] { "victim" }, new[] { ProgVariableTypeCode.Character })]
        CharacterDies = 79,

        /// <summary>
        /// Fires on a perceiver when they witness a character that has died. Parameters are victim, witness.
        /// </summary>
        [EventInfo("Fires on a perceiver when they have witnessed a death.", new[] { "character", "perceiver" }, new[] { "victim", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver })]
        CharacterDiesWitness = 80,

        /// <summary>
        /// Fires on a character when they have become incapacitated. Parameters are character.
        /// </summary>
        [EventInfo("Fires on a character when they have become incapacitated.", new[] { "character" }, new[] { "victim" }, new[] { ProgVariableTypeCode.Character })]
        CharacterIncapacitated = 81,

        /// <summary>
        /// Fires on a perceiver when they witness a character that has become incapacitated. Parameters are victim, witness.
        /// </summary>
        [EventInfo("Fires on a perceiver when they have witnessed an incapacitation.", new[] { "character", "perceiver" }, new[] { "victim", "witness" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver })]
        CharacterIncapacitatedWitness = 82,

        /// <summary>
        /// Fires on all clocked-in employees of a shop when an item requires restocking. Parameters are employee, shop, merchandise, amount
        /// </summary>
        [EventInfo("Fires on clocked-in employees of a shop when merchandise requires restocking.", new[] { "character", "shop", "merchandise", "number" }, new[] { "employee", "shop", "merchandise", "quantity" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Shop, ProgVariableTypeCode.Merchandise, ProgVariableTypeCode.Number })]
        ItemRequiresRestocking = 83,

        /// <summary>
        /// Fires once on each NPC at the end of loading from the database. Parameters are character.
        /// </summary>
        [EventInfo("Fires once on each NPC at the end of loading from the database.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypeCode.Character })]
        NPCOnGameLoadFinished = 84,

        /// <summary>
        /// Fires once on each item at the end of loading into the world for the first time (e.g. craft, manual load, prog etc)
        /// </summary>
        [EventInfo("Fires once on each item at the end of loading into the world for the first time (e.g. craft, manual load, prog etc)", new[] { "item" }, new[] { "item" }, new[] { ProgVariableTypeCode.Item })]
        ItemFinishedLoading = 85,

        [EventInfo("Fires when a combat at the location ends", new[] { "location" }, new[] { "location" }, new[] { ProgVariableTypeCode.Location })]
        CombatEndedHere = 86,

        /// <summary>
        /// Fires when the character witnesses a crime committed at their location. (criminal, victim, witness, crime id)
        /// </summary>
        [EventInfo("Fires when the character witnesses a crime committed at their location", new[] { "character", "character", "character", "crime" }, new[] { "criminal", "victim", "witness", "crime" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Crime })]
        WitnessedCrime = 87,

        /// <summary>
        /// Fires on the victim of a crime. Parameters are criminal, victim, crime.
        /// </summary>
        [EventInfo("Fires on the victim of a crime.", new[] { "character", "character", "crime" }, new[] { "criminal", "victim", "crime" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Crime })]
        VictimOfCrime = 88,

        /// <summary>
        ///     Hooks to a character when they leave the room they started in, but fires on their inventory items. Parameters are mover, cell, exit, item
        /// </summary>
        [EventInfo("Hooks to a character when they leave the room they started in, but fires on their inventory items", new[] { "character", "location", "exit", "item" }, new[] { "mover", "origin", "exit", "item" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Item })]
        CharacterLeaveCellItems = 89,

        /// <summary>
        ///     Hooks the character who begun to move, but fires on their inventory items. Parameters are mover, cell, exit, item
        /// </summary>
        [EventInfo("Hooks to a character when they begin to move, but fires on their inventory items", new[] { "character", "location", "exit", "item" }, new[] { "mover", "origin", "exit", "item" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Item })]
        CharacterBeginMovementItems = 90,


        /// <summary>
        ///     Hooks the character who moved, as they enter the room, but fires on their inventory items. Parameters are mover, cell, exit, item
        /// </summary>
        [EventInfo("Hooks to a character when they move into a room, but fires on their inventory items", new[] { "character", "location", "exit", "item" }, new[] { "mover", "destination", "exit", "item" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Location, ProgVariableTypeCode.Exit, ProgVariableTypeCode.Item })]
        CharacterEnterCellItems = 91,

        /// <summary>
        ///     Hooks to being invited to join a spar. Parameters are inviter, invitee
        /// </summary>
        [EventInfo("Hooks to a character being invited to join a spar", new[] { "character", "character" }, new[] { "inviter", "invitee" }, new[] { ProgVariableTypeCode.Character, ProgVariableTypeCode.Character })]
        SparInvitation = 92,

        /// <summary>
        /// Fires when a CommandDelay effect expires. Parameters are character, commands (string list)
        /// </summary>
        [EventInfo("Fires when a BlockLayerChange effect expires.", new[] { "character" }, new[] { "person", }, new[] { ProgVariableTypeCode.Character })]
        LayerChangeBlockExpired = 93,

        [EventInfo("Fired on an item when the item takes damage. Weapon and Aggressor may be null.", ["item", "item", "character"], ["item", "weapon", "aggressor"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, ProgVariableTypeCode.Character])]
        ItemDamaged = 94,

        [EventInfo("Fired on all perceivables in the location when the item takes damage. Weapon and Aggressor may be null.", ["item", "item", "character", "perceivable"], ["item", "weapon", "aggressor", "witness"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Item, ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable])]
        ItemDamagedWitness = 95,

        [EventInfo("Fired on a character when they take damage. Weapon and Aggressor may be null.", ["character", "item", "character"], ["character", "weapon", "aggressor"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Character])]
        CharacterDamaged = 96,

        [EventInfo("Fired on all perceivables in the location when a character takes damage.  Weapon and Aggressor may be null.", ["character", "item", "character", "perceivable"], ["character", "weapon", "aggressor", "witness"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable])]
        CharacterDamagedWitness = 97,

        [EventInfo("Fired on an item when it is wielded", ["item", "character"], ["item", "wielder"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Character])]
        ItemWielded = 98,

        [EventInfo("Fired on all perceivables in the location when an item is wielded", ["item", "character", "perceivable"], ["item", "wielder", "witness"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable])]
        ItemWieldedWitness = 99,

        [EventInfo("Fired on an item when it is worn", ["item", "character"], ["item", "wearer"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Character])]
        ItemWorn = 100,

        [EventInfo("Fired on all perceivables in the location when an item is worn", ["item", "character", "perceivable"], ["item", "wearer", "witness"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable])]
        ItemWornWitness = 101,

        [EventInfo("Fired on a character when they become hidden", ["character"], ["character"], [ProgVariableTypeCode.Character])]
        CharacterHidden = 102,

        [EventInfo("Fired on all perceivers in the location when they witness someone becoming hidden", ["character", "perceiver"], ["character", "witness"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceiver])]
        CharacterHidesWitness = 103,

        /// <summary>
        /// Fires on an item receiving in-call keypad digits from a telephone source. Parameters are source item, digits.
        /// </summary>
        [EventInfo("Fires on an item receiving in-call keypad digits from a telephone source", ["item", "text"], ["source", "digits"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Text])]
        TelephoneDigitsReceived = 104,

        [EventInfo("Fires on a character when they open an item.", ["character", "item"], ["opener", "item"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item])]
        CharacterOpenedItem = 105,

        [EventInfo("Fires on an item when it is opened by a character.", ["character", "item"], ["opener", "item"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item])]
        ItemOpened = 106,

        [EventInfo("Fires on perceivables witnessing a character opening an item.", ["character", "item", "perceivable"], ["opener", "item", "witness"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable])]
        CharacterOpenedItemWitness = 107,

        [EventInfo("Fires on a character when they close an item.", ["character", "item"], ["closer", "item"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item])]
        CharacterClosedItem = 108,

        [EventInfo("Fires on an item when it is closed by a character.", ["character", "item"], ["closer", "item"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item])]
        ItemClosed = 109,

        [EventInfo("Fires on perceivables witnessing a character closing an item.", ["character", "item", "perceivable"], ["closer", "item", "witness"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable])]
        CharacterClosedItemWitness = 110,

        [EventInfo("Fires on a character when they unwield an item.", ["character", "item"], ["unwielder", "item"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item])]
        CharacterUnwieldedItem = 111,

        [EventInfo("Fires on an item when it is unwielded.", ["character", "item"], ["unwielder", "item"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item])]
        ItemUnwielded = 112,

        [EventInfo("Fires on perceivables witnessing a character unwielding an item.", ["character", "item", "perceivable"], ["unwielder", "item", "witness"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable])]
        CharacterUnwieldedItemWitness = 113,

        [EventInfo("Fires on a character when one of their worn items is removed.", ["character", "character", "item"], ["wearer", "remover", "item"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item])]
        CharacterWornItemRemoved = 114,

        [EventInfo("Fires on an item when it is removed from being worn.", ["character", "character", "item"], ["wearer", "remover", "item"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item])]
        ItemRemovedFromWear = 115,

        [EventInfo("Fires on perceivables witnessing a worn item being removed.", ["character", "character", "item", "perceivable"], ["wearer", "remover", "item", "witness"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable])]
        CharacterWornItemRemovedWitness = 116,

        [EventInfo("Fires on a character when they mount another character.", ["character", "character"], ["rider", "mount"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Character])]
        CharacterMounted = 117,

        [EventInfo("Fires on perceivables witnessing a character mounting another character.", ["character", "character", "perceivable"], ["rider", "mount", "witness"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable])]
        CharacterMountedWitness = 118,

        [EventInfo("Fires on a character when they dismount another character.", ["character", "character"], ["rider", "mount"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Character])]
        CharacterDismounted = 119,

        [EventInfo("Fires on perceivables witnessing a character dismounting another character.", ["character", "character", "perceivable"], ["rider", "mount", "witness"], [ProgVariableTypeCode.Character, ProgVariableTypeCode.Character, ProgVariableTypeCode.Perceivable])]
        CharacterDismountedWitness = 120,

        [EventInfo("Fires on a lock item when it is locked. Actor and key may be null.", ["item", "character", "item", "perceivable"], ["lock", "actor", "key", "target"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable])]
        ItemLocked = 121,

        [EventInfo("Fires on perceivables witnessing a lock item being locked. Actor and key may be null.", ["item", "character", "item", "perceivable", "perceivable"], ["lock", "actor", "key", "target", "witness"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Perceivable])]
        ItemLockedWitness = 122,

        [EventInfo("Fires on a lock item when it is unlocked. Actor and key may be null.", ["item", "character", "item", "perceivable"], ["lock", "actor", "key", "target"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable])]
        ItemUnlocked = 123,

        [EventInfo("Fires on perceivables witnessing a lock item being unlocked. Actor and key may be null.", ["item", "character", "item", "perceivable", "perceivable"], ["lock", "actor", "key", "target", "witness"], [ProgVariableTypeCode.Item, ProgVariableTypeCode.Character, ProgVariableTypeCode.Item, ProgVariableTypeCode.Perceivable, ProgVariableTypeCode.Perceivable])]
        ItemUnlockedWitness = 124
    }
}
