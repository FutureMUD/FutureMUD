# AGENTS.md – Commands Module (Project MudSharpCore)

## Scope

This document defines the **local rules for the Commands Module** within Project MudSharpCore.  

It inherits from:

- [Solution-Level AGENTS.md](../../AGENTS.md)
- [Project MudSharpCore AGENTS.md](../AGENTS.md)

## Inheritance

- Rules here take **highest precedence** within this module.
- Only **repeat** rules when overriding.

## Module Purpose
- The purpose of this module is to provide the commands that users enter to interact with the world via the terminal.

## Module-Specific Rules and Concepts
- Commands are methods on module classes that implement Module<ICharacter>
- In order to be recognised as a command it must be a function on a Module<ICharacter> class with the return signature `protected static` and parameters `(ICharacter actor, string input)`, as well as annotated with a [PlayerCommand] attribute.
- In order to create an admin only command, the [CommandPermission] attribute is typically used. This can also be used to make NPC-only or player-only commands in some circumstances
- Building Commands is the term used for commands designed for admin builders to create, edit and interact with game engine content. Many of the building commands use a framework to ensure consistency of syntax and behaviour. These commands typically invoked GenericBuildingCommand (for IEditableItem types) or GenericRevisableBuildingCommand (for IEditableRevisable items). These take an EditableItemHelper or EditableRevisableItemHelper respectively which contains all the logic for working with that type
- There must only be one command with any command name. Duplicates will cause a runtime crash.
- CommandTrees are collections of command modules that apply to users with specific privilege levels.
- Socials are a special type of built-in command that are like abbreviated emotes.
- CommandManager handles the work of translating user input into an executable command
- When you create a new command, you should always include a [HelpInfo] attribute with a default help file that explains what the command is and how to use it. There are ample examples of the way these should be laid out and structured, especially for building commands.
- Within commands, you can use the StringStack class to help you "Pop" individual command arguments off the stack. This is the preferred way to do command interpretation. You will see some examples of Regular Expressions being used, but this is only used in some situations where there is moderately complex branching syntax where REGEX adds clarity.


## Notes

- This file only lists deviations or details specific to Module MudSharpCore.
- For everything else, see higher-level `AGENTS.md` files.

