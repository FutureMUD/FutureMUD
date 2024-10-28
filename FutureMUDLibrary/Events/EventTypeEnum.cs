using System.Threading.Tasks;
using MudSharp.FutureProg;

namespace MudSharp.Events {
	/// <summary>
	///     An enumerable representing which class of event is being fired
	/// </summary>
	public enum EventType {

		/// <summary>
		///     Hooks the character who dropped the item. Parameters are character, item
		/// </summary>
		[EventInfo("Hooks a character dropping an item. Fires for the person who dropped the item only.", new[] { "character", "item" }, new []{"dropper", "dropped"}, new []{ ProgVariableTypes.Character, ProgVariableTypes.Item})]
		CharacterDroppedItem = 0,

		/// <summary>
		///     Hooks any IHookable that witnessed an item being dropped. Parameteres are character, item, witness
		/// </summary>
		[EventInfo("Hooks a perceiver witnessing an item being dropped.", new[] { "character", "item", "perceiver" }, new []{"dropper", "dropped", "witness"}, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Perceiver,  })]
		CharacterDroppedItemWitness = 1,

		/// <summary>
		///     Hooks an item that was dropped. Parameters are character, item
		/// </summary>
		[EventInfo("Hooks an item being dropped.", new[] { "character", "item"}, new[] { "dropper", "dropped" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item })]
		ItemDropped = 2,

		/// <summary>
		///     Hooks the character who got the item from the ground. Parameters are character, item
		/// </summary>
		[EventInfo("Hooks a character getting an item from the ground. Fires for the person who got the item only.", new[] { "character", "item" }, new[] { "getter", "gotten" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item })]
		CharacterGotItem = 3,

		/// <summary>
		///     Hooks any IHookable that witnesed an item being gotten from the ground. Parameters are character, item, witness
		/// </summary>
		[EventInfo("Hooks a witnessing an item being gotten from the ground.", new[] { "character", "item", "perceiver" }, new[] { "getter", "gotten", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Perceiver})]
		CharacterGotItemWitness = 4,

		/// <summary>
		///     Hooks an item that was gotten from the ground. Parameters are character, item
		/// </summary>
		[EventInfo("Hooks an item being gotten from the ground.", new[] { "character", "item" }, new[] { "getter", "gotten" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item })]
		ItemGotten = 5,

		/// <summary>
		///     Hooks the character who got the item from a container. Parameters are character, item, container
		/// </summary>
		[EventInfo("Hooks a character getting an item from a container. Fires for the person who got the item only.", new[] { "character", "item", "item" }, new[] {"getter", "gotten", "container"}, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,  })]
		CharacterGotItemContainer = 6,

		/// <summary>
		///     Hooks any IHookable that witnesed an item being gotten from a container. Parameters are character, item, container,
		///     witness
		/// </summary>
		[EventInfo("Hooks witnessing an item being gotten from a container.", new[] { "character", "item", "item", "perceiver" }, new[] { "getter", "gotten", "container", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item, ProgVariableTypes.Perceiver,  })]
		CharacterGotItemContainerWitness = 7,

		/// <summary>
		///     Hooks an item that was gotten from a container. Parameters are character, item, container
		/// </summary>
		[EventInfo("Hooks an item being gotten from a container.", new[] { "character", "item", "item" }, new[] { "getter", "gotten", "container" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,  })]
		ItemGottenContainer = 8,


		/// <summary>
		///     Hooks the character who put the item in a container. Parameters are character, item, container
		/// </summary>
		[EventInfo("Hooks a character putting an item in a container. Fires for the person who put the item only.", new[] { "character", "item", "item" }, new[] { "putter", "put", "container" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,  })]
		CharacterPutItemContainer = 9,

		/// <summary>
		///     Hooks any IHookable that witnesed an item being put in a container. Parameters are character, item, container,
		///     witness
		/// </summary>
		[EventInfo("Hooks witnessing an item being put in a container.", new[] { "character", "item", "item", "perceiver" }, new[] { "putter", "put", "container", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item, ProgVariableTypes.Perceiver,  })]
		CharacterPutItemContainerWitness = 10,

		/// <summary>
		///     Hooks an item that was put in a container. Parameters are character, item, container
		/// </summary>
		[EventInfo("Hooks an item being put in a container.", new[] { "character", "item", "item" }, new[] { "putter", "put", "container" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item,  })]
		ItemPutContainer = 11,


		/// <summary>
		///     Hooks the character who is giving an item to another. Parameters are giver, receiver, item
		/// </summary>
		[EventInfo("Hooks an item being given to someone. Fires for giver only.", new[] { "character", "character", "item" }, new[] { "giver", "receiver", "given" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character,  ProgVariableTypes.Item })]
		CharacterGiveItemGiver = 12,

		/// <summary>
		///     Hooks the character who is receiving an item from another. Parameters are giver, receiver, item
		/// </summary>
		[EventInfo("Hooks an item being given to someone. Fires for receiver only.", new[] { "character", "character", "item" }, new[] { "giver", "receiver", "given" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item })]
		CharacterGiveItemReceiver = 13,

		/// <summary>
		///     Hooks the character who is witnessing an item being given. Parameters are giver, receiver, item, witness
		/// </summary>
		[EventInfo("Hooks witnessing an item being given to someone.", new[] { "character", "character", "item", "perceiver" }, new[] { "giver", "receiver", "given", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Perceiver,  })]
		CharacterGiveItemWitness = 14,

		/// <summary>
		///     Hooks an item that was given to someone. Parameters are giver, receiver, item
		/// </summary>
		[EventInfo("Hooks an item being given to someone.", new[] { "character", "character", "item" }, new[] { "giver", "receiver", "given" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item })]
		ItemGiven = 15,

		/// <summary>
		///     Hooks the character who sheathes an item. Parameters are character, item, sheathe
		/// </summary>
		[EventInfo("Hooks to a character sheathing their own item", new []{"character", "item", "item"}, new[]{"sheather", "item", "sheathe"}, new [] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item})]
		CharacterSheatheItem = 57,

		/// <summary>
		///     Hooks the character who witnesses an item being sheathed. Parameters are character, item, sheath, witness
		/// </summary>
		[EventInfo("Hooks to a perceivable witnessing an item being sheathed", new[] { "character", "item", "item", "perceivable" }, new[] { "sheather", "item", "sheathe", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item, ProgVariableTypes.Perceivable })]
		CharacterSheatheItemWitness = 58,

		/// <summary>
		///     Hooks the item that is sheathed. Parameters are character, item, sheath
		/// </summary>
		[EventInfo("Hooks to the item being put in a sheath", new[] { "character", "item", "item" }, new[] { "sheather", "item", "sheathe" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item })]
		ItemSheathed = 59,

		/// <summary>
		///     Hooks the sheath that is receiving the sheathed item. Parameters are character, item, sheath
		/// </summary>
		[EventInfo("Hooks to a sheath getting an item put in it", new[] { "character", "item", "item" }, new[] { "sheather", "item", "sheathe" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Item })]
		ItemSheathItemSheathed = 60,


		/// <summary>
		///     Hooks the character who moved, as they enter the room. Parameters are mover, cell, exit
		/// </summary>
		[EventInfo("Hooks to a character when they move into a room", new[] { "character", "location", "exit" }, new[] { "mover", "destination", "exit" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit })]
		CharacterEnterCell = 16,

		/// <summary>
		///     Hooks any witness to the character who moved, as they enter the room. Parameters are mover, cell, exit, witness
		/// </summary>
		[EventInfo("Hooks to a perceiver witnessing someone move into a room", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "destination", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterEnterCellWitness = 17,

		/// <summary>
		///     Hooks the character who moved, after movement is complete. Parameters are mover, cell, exit
		/// </summary>
		[EventInfo("Hooks to a character when they have moved into a room and their movement finishes", new[] { "character", "location", "exit" }, new[] { "mover", "destination", "exit" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit })]
		CharacterEnterCellFinish = 18,

		/// <summary>
		///     Hooks any witness to the character who moved, after movement is complete. Parameters are mover, cell, exit, witness
		/// </summary>
		[EventInfo("Hooks to a perceiver witnessing someone finish move into a room", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "destination", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterEnterCellFinishWitness = 19,

		/// <summary>
		///     Hooks the character who begun to move. Parameters are mover, cell, exit
		/// </summary>
		[EventInfo("Hooks to a character when they begin to move", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit })]
		CharacterBeginMovement = 21,

		/// <summary>
		///     Hooks any character who witnessed the character begin to move. Parameters are mover, cell, exit, witness
		/// </summary>
		[EventInfo("Hooks to a perceiver witnessing someone begin to move", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "origin", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterBeginMovementWitness = 22,

		/// <summary>
		///     Hooks the character who moved, as they leave the room. Parameters are mover, cell, exit
		/// </summary>
		[EventInfo("Hooks to a character when they leave the room they started in", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit })]
		CharacterLeaveCell = 23,

		/// <summary>
		///     Hooks any witness to the character who moved, as they leave the room. Parameters are mover, cell, exit, witness
		/// </summary>
		[EventInfo("Hooks to a perceiver witnessing someone move out of a room", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "origin", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterLeaveCellWitness = 24,

		/// <summary>
		///     Hooks a character when they stop moving from the STOP command. Parameters are mover, cell, exit
		/// </summary>
		[EventInfo("Hooks to a character when they stop moving with STOP command", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit })]
		CharacterStopMovement = 25,

		/// <summary>
		///     Hooks any witness to the character when they stop moving from the STOP command. Parameters are mover, cell, exit,
		///     witness
		/// </summary>
		[EventInfo("Hooks to a perceiver witnessing someone stop moving with the STOP command", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "origin", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterStopMovementWitness = 26,

		/// <summary>
		///     Hooks a character when they stop moving from the door being closed. Parameters are mover, cell, exit
		/// </summary>
		[EventInfo("Hooks to a character when they stop moving because of a closed door", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit })]
		CharacterStopMovementClosedDoor = 27,

		/// <summary>
		///     Hooks any witness to the character when they stop moving from the door being closed. Parameters are mover, cell,
		///     exit, witness
		/// </summary>
		[EventInfo("Hooks to a perceiver witnessing someone stop moving because of a closed door", new[] { "character", "location", "exit", "perceivable" }, new[] { "mover", "origin", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterStopMovementClosedDoorWitness = 28,

		/// <summary>
		///     Hooks to trying to move but not being able to. Parameters are mover, cell, exit
		/// </summary>
		[EventInfo("Hooks to a character trying to move but being unable to", new[] { "character", "location", "exit" }, new[] { "mover", "origin", "exit"}, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit})]
		CharacterCannotMove = 45,

		/// <summary>
		///     Hooks to a character entering the game for the first time
		/// </summary>
		[EventInfo("Fires once when a character enters the game for the first time", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		CharacterEntersGame = 20,

		/// <summary>
		///     Hooks to a character entering a command in the presence of items,characters (other than self),rooms BEFORE the
		///     command is evaluated. If handled, does not evaluate the command. Parameters are character, thing, command,
		///     (stringstack)arguments
		/// </summary>
		[EventInfo("Hooks to a character entering a command in the presence of items, character, rooms", new []{ "character", "perceivable", "text", "text collection"}, new []{ "ch", "thing", "command", "args"}, new []{ ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text | ProgVariableTypes.Collection})]
		CommandInput = 34,

		/// <summary>
		///     Hooks to the character themselves BEFORE the command is evaluated. If handled, does not evaluate the command.
		///     Parameters are character, command, (stringstack)arguments
		/// </summary>
		[EventInfo("Hooks to a character entering a command on themselves", new[] { "character", "text", "text collection" }, new[] { "ch", "command", "args" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Text | ProgVariableTypes.Collection })]
		SelfCommandInput = 35,


		/// <summary>
		///     Hooks to any character who witnesses (but is not targeted by) a social. Parameters are socialite, social,
		///     (list)targets, (cellexit)direction, witness
		/// </summary>
		[EventInfo("Hooks to a perceiver witnessing (but not targeted by) a social", new[] { "character", "text", "perceivable collection", "exit", "perceivable" }, new[] { "ch", "social", "targets", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Perceivable | ProgVariableTypes.Collection, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterSocialWitness = 29,

		/// <summary>
		///     Hooks to any character who is targeted by a social. Parameters are Parameters are socialite, social, target,
		///     (cellexit)direction
		/// </summary>
		[EventInfo("Hooks to a perceivable being targeted by a social", new[] { "character", "text", "perceivable", "exit" }, new[] { "socialite", "social", "target", "exit" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Collection, ProgVariableTypes.Exit })]
		CharacterSocialTarget = 30,

		/// <summary>
		///     Hooks to any character who is on the same side of a door when it is knocked on (except the person who knocked).
		///     Parameters are knocker, cell, exit, witness
		/// </summary>
		[EventInfo("Hooks to a character on the same side of a door when it is knocked on", new[] { "character", "location", "exit", "perceivable" }, new[] { "knocker", "origin", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterDoorKnockedSameSide = 31,

		/// <summary>
		///     Hooks to any character who is on the other side of a door when it is knocked on. Parameters are knocker, cell,
		///     exit, witness
		/// </summary>
		[EventInfo("Hooks to a character on the other side of a door when it is knocked on", new[] { "character", "location", "exit", "perceivable" }, new[] { "knocker", "origin", "exit", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Perceivable })]
		CharacterDoorKnockedOtherSide = 32,

		/// <summary>
		///     Hooks to the door itself when knocked upon. Parameters are knocker, cell, exit, door.Parent
		/// </summary>
		[EventInfo("Hooks to a door item when it is knocked on", new[] { "character", "location", "exit", "item" }, new[] { "knocker", "origin", "exit", "door" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Item })]
		DoorKnocked = 33,

		/// <summary>
		///     Hooks to a character speaking. Parameters are character, volume, language, accent, text
		/// </summary>
		[EventInfo("Hooks to a character speaking", new[] { "character", "text", "language", "accent", "text" }, new[] { "character", "volume", "language", "accent", "message" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Language, ProgVariableTypes.Accent, ProgVariableTypes.Text })]
		CharacterSpeaks = 39,

		/// <summary>
		///     Hooks to a character witnessing speaking. Parameters are character, witness, volume, language, accent, text
		/// </summary>
		[EventInfo("Hooks to witnessing a character speaking", new[] { "character", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "witness", "volume", "language", "accent", "message" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Language, ProgVariableTypes.Accent, ProgVariableTypes.Text })]
		CharacterSpeaksWitness = 40,

		/// <summary>
		///     Hooks to a character speaking directly to someone. Parameters are character, target, volume, language, accent, text
		/// </summary>
		[EventInfo("Hooks to a character speaking directly to something", new[] { "character", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "target", "volume", "language", "accent", "message" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Language, ProgVariableTypes.Accent, ProgVariableTypes.Text })]
		CharacterSpeaksDirect = 41,

		/// <summary>
		///     Hooks to a character being spoken to directly by someone. Parameters are character, target, volume, language,
		///     accent, text
		/// </summary>
		[EventInfo("Hooks to a the target of character speaking directly to something", new[] { "character", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "target", "volume", "language", "accent", "message" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Language, ProgVariableTypes.Accent, ProgVariableTypes.Text })]
		CharacterSpeaksDirectTarget = 42,

		/// <summary>
		///     Hooks to a character witnessing someone speaking directly to someone. Parameters are character, target, witness,
		///     volume, language, accent, text
		/// </summary>
		[EventInfo("Hooks to a the witness of a character speaking directly to something", new[] { "character", "perceivable", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "target", "witness", "volume", "language", "accent", "message" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Language, ProgVariableTypes.Accent, ProgVariableTypes.Text })]
		CharacterSpeaksDirectWitness = 43,

		/// <summary>
		///     Hooks to a character witnessing speech from another room (usually from shouting). Parameters are character, target,
		///     witness, volume, language, accent, text
		/// </summary>
		[EventInfo("Hooks to a the witness of a character speaking from another room (i.e. shouting)", new[] { "character", "perceivable", "perceivable", "text", "language", "accent", "text" }, new[] { "character", "target", "witness", "volume", "language", "accent", "message" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Language, ProgVariableTypes.Accent, ProgVariableTypes.Text })]
		CharacterSpeaksNearbyWitness = 44,

		/// <summary>
		///     Hooks to any character who is eating something. Parameters are eater, food, bites, bitesremaining
		/// </summary>
		[EventInfo("Hooks to a character when they eat something", new[] { "character", "item", "number", "number" }, new[] { "character", "food", "bites", "remaining" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number})]
		CharacterEat = 36,

		/// <summary>
		///     Hooks to any character who witnesses someone eating something. Parameters are eater, witness, food, bites,
		///     bitesremaining
		/// </summary>
		[EventInfo("Hooks to someone witnessing a character eat something", new[] { "character", "perceivable", "item", "number", "number" }, new[] { "character", "witness", "food", "bites", "remaining" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number })]
		CharacterEatWitness = 37,

		/// <summary>
		///     Hooks to any item that is being eaten. Parameters are eater, food, bites, bitesremaining
		/// </summary>
		[EventInfo("Hooks to an item when it is being eaten", new[] { "character", "item", "number", "number" }, new[] { "character", "food", "bites", "remaining" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number })]
		ItemEaten = 38,

		/// <summary>
		///     Hooks to a character swallowing something. Parameters are swallower, swallowed
		/// </summary>
		[EventInfo("Hooks to a character when they swallow something", new[] { "character", "item" }, new[] { "character", "swallowed" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item })]
		CharacterSwallow = 61,

		/// <summary>
		///     Hooks to a perceiver witnessing a character swallowing something. Parameters are swallower, swallowed, witness
		/// </summary>
		[EventInfo("Hooks to someone witnessing a character swallow something", new[] { "character", "item", "perceivable" }, new[] { "character", "swallowed", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Perceivable })]
		CharacterSwallowWitness = 62,

		/// <summary>
		///     Hooks to an item being swallowed. Parameters are swallower, swallowed
		/// </summary>
		[EventInfo("Hooks to an item being swallowed", new[] { "character", "item" }, new[] { "character", "swallowed" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item })]
		ItemSwallowed = 63,

		/// <summary>
		///     Hooks to being invited to join a spar. Parameters are inviter, invitee
		/// </summary>
		[EventInfo("Hooks to a character being invited to join a spar", new[] { "character", "character" }, new[] { "inviter", "invitee" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character })]
		SparInvitation = 45,

		/// <summary>
		///     Hooks to engaging an opponent in combat. Parameters are aggressor, target
		/// </summary>
		[EventInfo("Hooks to a character engaging an opponent in combat", new[] { "character", "perceiver" }, new[] { "aggressor", "target" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver })]
		EngageInCombat = 46,

		/// <summary>
		///     Hooks to being engaged by an opponent in combat. Parameters are aggressor, target
		/// </summary>
		[EventInfo("Hooks to a target being engaging in combat", new[] { "character", "perceiver" }, new[] { "aggressor", "target" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver })]
		EngagedInCombat = 47,

		/// <summary>
		///     Hooks to witnessing someone being engaged by an opponent in combat. Parameters are aggressor, target, witness
		/// </summary>
		[EventInfo("Hooks to someone witnessing a character engaging an opponent in combat", new[] { "character", "perceiver", "perceivable" }, new[] { "aggressor", "target", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver, ProgVariableTypes.Perceivable })]
		EngagedInCombatWitness = 48,

		/// <summary>
		///     Hooks to someone starting to target the individual in a melee combat. Parameters are aggressor, target
		/// </summary>
		[EventInfo("Hooks to a character targeting an individual in melee combat", new[] { "character", "perceiver" }, new[] { "aggressor", "target" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver })]
		TargettedInCombat = 49,

		/// <summary>
		///     Hooks to someone who was targeted no longer being targeted. Parameters are aggressor, target, newtarget
		/// </summary>
		[EventInfo("Hooks to a character no longer being targeted in combat", new[] { "character", "perceiver", "perceiver" }, new[] { "aggressor", "target", "newtarget" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver, ProgVariableTypes.Perceiver })]
		NoLongerTargettedInCombat = 50,

		/// <summary>
		///     Hooks to someone leaving combat. Parameters are character.
		/// </summary>
		[EventInfo("Fires when a character leaves combat.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		LeaveCombat = 51,

		/// <summary>
		///     Hooks to someone joining combat. Parameters are character.
		/// </summary>
		[EventInfo("Fires when a character joins combat.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		JoinCombat = 52,

		/// <summary>
		///     Hooks to someone getting to the end of Combatant.AcquireTarget and having no natural target. Parameters are
		///     character.
		/// </summary>
		[EventInfo("Fires when a character has no targets in combat and attempts to do something.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		NoNaturalTargets = 53,

		/// <summary>
		///     Hooks to someone who was previously engaged in melee who is no longer. Parameters are character.
		/// </summary>
		[EventInfo("Fires when a character who was in melee combat is no longer in melee.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		NoLongerEngagedInMelee = 54,


		/// <summary>
		///     Hooks to someone who has themselves bled. Parameters are character, bleeding
		/// </summary>
		[EventInfo("Fires when a character bleeds.", new[] { "character", "number" }, new[] { "person", "litres" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Number,  })]
		BleedTick = 55,

		/// <summary>
		///     Hooks to someone who has witnessed another bleed. Parameters are character, bleeding, witness
		/// </summary>
		[EventInfo("Fires when something witnesses a character bleeding.", new[] { "character", "number", "perceiver" }, new[] { "person", "litres", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Perceiver,  })]
		WitnessBleedTick = 56,


		/// <summary>
		/// Hooks to someone who has fired a gun which turned out to be empty. Parameters are character, target, gun
		/// </summary>
		[EventInfo("Fires when a character fires a gun that is empty.", new[] { "character", "character", "item" }, new[] { "person", "target", "gun" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item,  })]
		FireGunEmpty = 64,

		/// <summary>
		/// Hooks to someone who has tried to ready a gun which turned out to be empty. Parameters are character, gun.
		/// </summary>
		[EventInfo("Fires when a character readies a gun that is empty.", new[] { "character", "item" }, new[] { "person", "gun" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, })]
		ReadyGunEmpty = 65,

		/// <summary>
		/// Fires every 5 seconds. NPCs only. Parameters are character.
		/// </summary>
		[EventInfo("Fires every 5 seconds for characters.", new[] { "character"}, new[] { "person" }, new[] { ProgVariableTypes.Character})]
		FiveSecondTick = 66,

		/// <summary>
		/// Fires every 10 seconds. NPCs only. Parameters are character.
		/// </summary>
		[EventInfo("Fires every 10 seconds for characters.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		TenSecondTick = 67,

		/// <summary>
		/// Fires every hour. NPCs only. Parameters are character.
		/// </summary>
		[EventInfo("Fires every 1 hour for characters.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		HourTick = 69,

		/// <summary>
		/// Fires every minute. NPCs only. Parameters are character.
		/// </summary>
		[EventInfo("Fires every 60 seconds for characters.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		MinuteTick = 68,

		/// <summary>
		/// Fires when a CommandDelay effect expires. Parameters are character, commands (string list)
		/// </summary>
		[EventInfo("Fires when a command delay effect expires.", new[] { "character", "text collection" }, new[] { "person", "commands" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Text | ProgVariableTypes.Collection })]
		CommandDelayExpired = 70,

		/// <summary>
		/// Fires when a combat target is critically injured. Parameters are character, target
		/// </summary>
		[EventInfo("Fires when a target is critically injured in combat.", new[] { "character", "character" }, new[] { "attacker", "target" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character, })]
		TargetIncapacitated = 71,

		/// <summary>
		/// Fires when someone the target is fighting offers a truce. Parameters are character, target
		/// </summary>
		[EventInfo("Fires when a truce is offered in combat.", new[] { "character", "character" }, new[] { "trucer", "target" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character, })]
		TruceOffered = 72,

		/// <summary>
		/// Fires when a combat target is killed in combat. Parameters are character, target
		/// </summary>
		[EventInfo("Fires when a target is killed in combat.", new []{ "character", "character"}, new []{"killer", "victim"}, new []{ ProgVariableTypes.Character, ProgVariableTypes.Character, })]
		TargetSlain = 73,

		/// <summary>
		/// Fires when a target being aimed at becomes invalid. Parameters are character, target, weapon
		/// </summary>
		[EventInfo("Fires when a target being aimed at becomes invalid.", new[] { "character", "perceiver", "item" }, new[] { "shooter", "target", "weapon" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver, ProgVariableTypes.Item, })]
		LostAim = 74,

		/// <summary>
		/// Fires when a character uses the COMMANd command on another character. Parameters are commandee, commander, commandtext
		/// </summary>
		[EventInfo("Fires when a character use the COMMAND command on another character.", new []{ "character", "character", "text" }, new []{ "commandee", "commander", "commandtext"}, new []{ ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Text})]
		CommandIssuedToCharacter = 75,

		/// <summary>
		/// Fires whenever weather changes for all perceivers and locations. Parameters are perceiver, oldweather, newweather
		/// </summary>
		[EventInfo("Fires on all perceivers and locations when weather changes.", new[] { "perceiver", "weatherevent", "weatherevent"}, new[] { "witness", "oldweather", "newweather"}, new[] { ProgVariableTypes.Perceiver, ProgVariableTypes.WeatherEvent, ProgVariableTypes.WeatherEvent})]
		WeatherChanged = 76,
		
		/// <summary>
		/// Fires when a character buys something from a shop. Parameters are buyer, shop, merchandise, item collection
		/// </summary>
		[EventInfo("Fires when a character buys something from a shop.", new[] { "character", "shop", "merchandise", "item collection"}, new[] { "buyer", "shop", "merchandise", "actualitems"}, new[] { ProgVariableTypes.Character, ProgVariableTypes.Shop, ProgVariableTypes.Merchandise, ProgVariableTypes.Item | ProgVariableTypes.Collection})]
		BuyItemInShop = 77,

		/// <summary>
		/// Fires when a perceiver witnesses a character buying something from a shop. Parameters are buyer, witness, shop, merchandise, item collection
		/// </summary>
		[EventInfo("Fires when a character buys something from a shop.", new[] { "character", "perceiver", "shop", "merchandise", "item collection" }, new[] { "buyer", "witness", "shop", "merchandise", "actualitems" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver, ProgVariableTypes.Shop, ProgVariableTypes.Merchandise, ProgVariableTypes.Item | ProgVariableTypes.Collection })]
		WitnessBuyItemInShop = 78,

		/// <summary>
		/// Fires on a character when they have died. Parameters are character.
		/// </summary>
		[EventInfo("Fires on a character when they have died.", new[] { "character"}, new[] { "victim" }, new[] { ProgVariableTypes.Character})]
		CharacterDies = 79,

		/// <summary>
		/// Fires on a perceiver when they witness a character that has died. Parameters are victim, witness.
		/// </summary>
		[EventInfo("Fires on a perceiver when they have witnessed a death.", new[] { "character", "perceiver" }, new[] { "victim", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver })]
		CharacterDiesWitness = 80,

		/// <summary>
		/// Fires on a character when they have become incapacitated. Parameters are character.
		/// </summary>
		[EventInfo("Fires on a character when they have become incapacitated.", new[] { "character" }, new[] { "victim" }, new[] { ProgVariableTypes.Character })]
		CharacterIncapacitated = 81,

		/// <summary>
		/// Fires on a perceiver when they witness a character that has become incapacitated. Parameters are victim, witness.
		/// </summary>
		[EventInfo("Fires on a perceiver when they have witnessed an incapacitation.", new[] { "character", "perceiver" }, new[] { "victim", "witness" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceiver })]
		CharacterIncapacitatedWitness = 82,

		/// <summary>
		/// Fires on all clocked-in employees of a shop when an item requires restocking. Parameters are employee, shop, merchandise, amount
		/// </summary>
		ItemRequiresRestocking = 83,

		/// <summary>
		/// Fires once on each NPC at the end of loading from the database. Parameters are character.
		/// </summary>
		[EventInfo("Fires once on each NPC at the end of loading from the database.", new[] { "character" }, new[] { "person" }, new[] { ProgVariableTypes.Character })]
		NPCOnGameLoadFinished = 84,

		/// <summary>
		/// Fires once on each item at the end of loading into the world for the first time (e.g. craft, manual load, prog etc)
		/// </summary>
		[EventInfo("Fires once on each item at the end of loading into the world for the first time (e.g. craft, manual load, prog etc)", new[] { "item" }, new[] { "item" }, new[] { ProgVariableTypes.Item })]
		ItemFinishedLoading = 85,

		[EventInfo("Fires when a combat at the location ends", new[] { "location" }, new[] { "location" }, new[] { ProgVariableTypes.Location})]
		CombatEndedHere = 86,

		/// <summary>
		/// Fires when the character witnesses a crime committed at their location. (criminal, victim, witness, crime id)
		/// </summary>
		[EventInfo("Fires when the character witnesses a crime committed at their location", new[] { "character", "character", "character", "number" }, new[] { "criminal", "victim", "witness", "crime" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Number})]
		WitnessedCrime = 87,

		VictimOfCrime = 88,

		/// <summary>
		///     Hooks to a character when they leave the room they started in, but fires on their inventory items. Parameters are mover, cell, exit, item
		/// </summary>
		[EventInfo("Hooks to a character when they leave the room they started in, but fires on their inventory items", new[] { "character", "location", "exit", "item" }, new[] { "mover", "origin", "exit", "item" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Item })]
		CharacterLeaveCellItems = 89,

		/// <summary>
		///     Hooks the character who begun to move, but fires on their inventory items. Parameters are mover, cell, exit, item
		/// </summary>
		[EventInfo("Hooks to a character when they begin to move, but fires on their inventory items", new[] { "character", "location", "exit", "item" }, new[] { "mover", "origin", "exit", "item" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Item })]
		CharacterBeginMovementItems = 90,


		/// <summary>
		///     Hooks the character who moved, as they enter the room, but fires on their inventory items. Parameters are mover, cell, exit, item
		/// </summary>
		[EventInfo("Hooks to a character when they move into a room, but fires on their inventory items", new[] { "character", "location", "exit", "item" }, new[] { "mover", "destination", "exit", "item" }, new[] { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Exit, ProgVariableTypes.Item })]
		CharacterEnterCellItems = 91,
	}
}