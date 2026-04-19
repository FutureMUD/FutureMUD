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
- Use StringStack as the default parser for command input.
- Use PopSpeech() for each non-final argument by default. If optional parenthetical emotes are valid at the current position, consume them with PopParentheses() before continuing with PopSpeech().
- Use SafeRemainingArgument for the final argument by default so quoted final arguments behave as players expect.
- Keep RemainingArgument only when quote characters are semantically meaningful to downstream parsing (for example forwarding a command to be re-parsed, regex text, or emote markup that relies on raw quotes).
- Avoid side-effecting pops inside LINQ predicates/selectors. Pop into a temporary variable first, or use Peek/PeekSpeech and then pop once when you intentionally advance the stack.
- Regular expressions are allowed only when whole-input shape branching is substantially clearer than StringStack parsing.

## Text Markup Reference
- [Emote System](../../Design%20Documents/Emote%20System.md)
- [Character Description System](../../Design%20Documents/Character_Description_System.md)
- [Room Description Markup](../../Design%20Documents/Room_Description_Markup.md)


## Notes

- This file only lists deviations or details specific to Module MudSharpCore.
- For everything else, see higher-level `AGENTS.md` files.

