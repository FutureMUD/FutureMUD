# FutureMUD Solution Instructions for AI Agents

## Scope
This document defines **global principles and defaults** that apply across all projects and modules in this solution.

## Inheritance
- All projects and modules inherit these rules by default.
- Lower-level `AGENTS.md` files may **extend** or **override** specific rules where necessary.
- Precedence: **Module > Project > Solution**.

## Purpose

FutureMUD is an engine for creating Roleplay Intensive (RPI) MUD games. It is an engine project, in that it is intended that other people take this engine and builds games with it - it's not necessarily a game but an engine for games.

The objective of this engine is to create a modern, stable, fully featured engine to support the genre and offer customisation either in-engine or using supplied tools so that the end users do not need to know how to program to build and host their game.

### What is a MUD?

A Multi-User Dungeon (MUD) is a persistent, text-based online world where multiple players connect simultaneously to explore environments, interact with each other, and take part in shared narratives. Originating in the late 1970s and 1980s, MUDs combine elements of role-playing games, adventure games, and early social platforms. Players connect through a terminal-like interface, typing commands to move through rooms, perform actions, and communicate with others. Over time, MUDs branched into several subgenres, each with different emphases—combat, socialization, building, or storytelling.

### The Roleplay Intensive (RPI) Genre

Roleplay Intensive (RPI) MUDs are a subgenre that prioritises immersive, character-driven storytelling over mechanical gameplay. Unlike hack-and-slash or purely social MUDs, RPIs emphasise the in-character experience, where players embody fictional personas and interact with the world as those characters. However, unlike MUSHes, RPI MUDs do still prioritise having coded, in-game systems to control things rather than pure imaginative storytelling.

Key traits of RPIs include:

- Enforced Roleplay: Out-of-character chatter is limited and typically segregated into separate channels. The expectation is that nearly all visible actions support the shared fiction.

- Immersive World Simulation: RPIs often feature detailed systems for survival, crafting, or social status, not just combat. These mechanics exist to enrich roleplay, not dominate it.

- Permanent Consequences: Character death is usually permanent (permadeath), raising the stakes for decision-making and interactions.

- Narrative Collaboration: Staff and players alike contribute to ongoing stories, with administrators often guiding world events while players shape outcomes through their roleplay.

- Cultural and Social Realism: RPIs often encourage players to observe setting-specific norms (accents, taboos, hierarchies, etc.) to strengthen immersion.

In short, RPI MUDs aim to provide a deeply collaborative storytelling environment, where the primary reward comes not from “winning” but from contributing to a living, evolving narrative.

## Architecture

The engine is written in C# and typically uses whatever version of .NET / C# is most currently, which at the present time is .NET 9. You should prefer to use C# features that are introduced in newer language versions over older language versions.

The engine uses MySQL for its database through a Pomelo connector and the Entity Framework (EF CORE 9).

There are multiple projects in the solution and many of them are separate "products". An explanation of the projects follows, but if a project is not specified, you can assume it is an experimental or incomplete product that is not part of the core engine package or not yet released:

###FutureMUDLibrary Project

This is a collection of interfaces, helper methods and extensions that are used in multiple products in the solution. Typically all extension methods and interfaces should go in this project. It uses a similar namespace structure to the main game engine.

- [Project FutureMUDLibrary](./FutureMUDLibrary/AGENTS.md)

###FutureMUDDatabaseLibrary Project

This is where the Entity Framework code goes - the DatabaseContext, Models, Migrations and the like. Anything to do with database should go in here. 

Note - there is an exception to this in the MudSharpCore project, the class being *MudSharp.Database.FMDB.cs*. This class is only used in MudSharpCore but is the usual way that the database is invoked, by putting it in a using context like so:

using (new FMDB())
{
   // Code accesses FMDB.Context static method only within this using block. 
}

- [Project MudsharpDatabaseLibrary](./MudsharpDatabaseLibrary/AGENTS.md)

###ExpressionEngine Project

This library is used to supply an IExpression interface and an Expression implementation that provide some functionality over the top of an NCalc library. It is mostly use to take user-defined formulas and efficiently interpret them for a mathematical outcome in the engine, for example damage formulas, hit points and the like.

- [Project ExpressionEngine](./ExpressionEngine/AGENTS.md)

###MudSharpCore Project

This is the main MUD engine - the core FutureMUD product. It is a console application and listens on specified TCP ports for input.

- [Project MudSharpCore](./MudSharpCore/AGENTS.md)

###MudSharpCore Unit Tests Project

This is a unit test project for MudSharpCore. Implement any unit tests in here.

- [Project MudSharpCore Unit Tests](./MudSharpCore Unit Tests/AGENTS.md)

###DatabaseSeeder Project

The Database Seeder is designed to function as the installer for the engine. It is a console application that the user runs before running their game for the first time.

What it does is that it guides the user through a number of choices about how they want their game's data to be and then it programmatically creates the initial database data, saving the end user from having to do it in many cases or at least providing examples. 

It also sets up a few supporting files for the engine and sets up a startup script.

The seeder can also be run again in the future to add additional options and context, or when new content is released in the seeder that the user wants to import.

- [Project DatabaseSeeder](./DatabaseSeeder/AGENTS.md)

###DiscordBotCore Project

The discord bot is a bot designed to run alongside the engine to provide some discord interactions for the game owners, like echoing in game messages or providing admins some control over the engine via discord commands. It uses the DSharpPlus library for discord and is a console application.

- [Project DiscordBotCore](./DiscordBotCore/AGENTS.md)

## Code Style
- Use **tabs** for indentation.
- Use file-scoped namespaces (`namespace MudSharp.Commands;`) for new files.
- Place `using` directives at the top of the file.
- Name classes, methods, and properties in **PascalCase**. Private fields use camelCase with a leading underscore.
- Prefer `var` for local variables when the type is clear and favour early returns for validation.
- Use string interpolation over string format functions were possible. Prefer verbatim strings over normal strings with special characters.
- Enable nullable reference types (`#nullable enable`) in new files and annotate nullable references with `?`.
- Prefer `async`/`await` for I/O-bound work and return `Task` or `Task<T>` rather than `void`.
- Important helper methods are found in the FutureMUDLibrary project under the MudSharp.Framework namespace.
- Use LINQ where possible unless you're being instructed to optimise a particular high-bottleneck section of code. Prefer to put your LINQ statements on separate lines for clarity and don't be afraid to break up logic into multiple steps (e.g. having multiple .Where calls rather than cramming all your logic into one lambda)
- Interface-first design - if you're introducing a new feature get the interface first, implement the logic to work with the interface and then do the implementation last.
- Use design patterns where they make sense. Not everything needs the maximum level of abstraction but we should prefer to follow common C# design patterns where we can.

## Key Internal Concepts
- The interface IFrameworkItem is the base interface for most things in the engine. It provides a 64 bit integer ID, string Name and a string representing the type. Nearly anything that gets saved in the database will implement this interface.
- The interface IPerceivable is the base interface for anything that physically exists somewhere in the game and should interact with the perception system - characters, items, rooms, exits, etc. It extends IFrameworkItem. Broadly speaking, a perceivable is a good way to represent "things that can be seen or interacted with".
- The interface IPerceiver is used as a base for a small number of things that can see echoes and perceive things around them, such as characters and items. It extends IPerceivable.

## Style Preferences and Internal Helper Conventions
- Always prefer to present numbers, times, dates and the like using localised formatting where you know the IPerceiver the message is being presented to. Conventionally an IPerceiver passed to a method for this reason is called a `voyeur`. 
- IPerceivers are IFormatProviders and so can be passed into functions like ToString in the same way as CultureInfo objects.
- When presenting an enum value to a player, prefer to use the extension method `.DescribeEnum()` rather than `.ToString()`. This provides a more user-friendly string.
- When presenting a TimeSpan to a player, prefer to use the extension method `.Describe(voyeur)` rather than `.ToString()`. This provides a more user-friendly string.
- When presenting a boolean to a player, prefer to use the extension method `ToColouredString()` rather than `.ToString()`. This provides a green "true" or red "false".
- Consider colouring certain text using the ANSI colour helpers where appropriate. There is a method `Colour(ANSIColour colour)` on strings that helps with this. Values for ANSIColour are in the MudSharp.Framework.Telnet class as constants.
- Numbers, dates, times, currencies, "values" in general are presented with ANSIColour.Green, often best done through the extension method `ColourValue(this string str)`.
- Errors, warnings and certain negative values are presented with ANSIColour.Red, often best done through the extension method `ColourError(this string str)`.
- Command syntax, user input, template emote text and the like are usually presented with ANSIColour.Yellow, often best done through the extension method `ColourCommand(this string str)`.
- Names of things, locations, people, as well as text representations of enum values are usually presented with ANSIColour.Cyan, often best done through the extension method `ColourName(this string str)`.
- When building up large strings for presentation to players, prefer to use StringBuilder rather than concatenating strings together. If you must concatenate strings, prefer string interpolation over String.Format or concatenation operators.
- When chopping up user input one argument at a time, prefer to use the StringStack class rather than splitting strings manually. It provides a lot of helper methods for this purpose.
- Prefer to use LINQ methods like .Any(), .FirstOrDefault(), .Where() and the like over manual loops where possible for clarity. When stacking multiple LINQ methods, put each method on its own line for clarity.
- Use LINQ query syntax only when it makes the code significantly clearer than method syntax. A common place this is done is when presenting tabular data to players.
- Use the method `StringUtilities.GetTextTable()` to present tabular data to players rather than building tables manually.
- Use the extension method `ListToString()` on IEnumerable collections to present lists of things to players rather than building lists manually. Similarly, you can use `ListToCommaSeparatedValues()` to get a comma-separated string.
- Rooms vs Cells: the legacy distinction is being removed; wherever possible prefer targeting `ICell`/`Cell` rather than `IRoom`/`Room` (and treat "room" mentions in designs as meaning cell) so future merges are simpler.

## Emote Markup
The `Emote` class in `MudSharp.PerceptionEngine` provides a lightweight markup language for
dynamically tailored messages. Emotes substitute placeholders at parse time so each viewer sees
a grammatically correct perspective of the same event. The string passed to an `Emote` **must not**
contain raw `{` characters and targets referenced in the text must be supplied to the constructor in
the same order.

### Source Token
`@` represents the source of the emote.

- `@` – short description of the source.
- `@#` – subject pronoun (he/she/they).
- `@!` – object pronoun (him/her/them).
- `@'s` – possessive form.
If the emote lacks `@`, setting `forceSourceInclusion` prepends the source description automatically.

### Referencing Perceivables (internal `$` tokens)
Internal tokens refer to perceivables passed to the constructor (`$0`, `$1`, …).

- `$0` – description of target 0 / “you”.
- `$0's` – possessive form.
- `!0` – description without article / “you”.
- `&0` – object pronoun (“him/her/them”) / “you”.
- `#0` – subject pronoun (“he/she/they”) / “you”.
- `%0` – reflexive (“himself/herself/itself”) / “yourself”.

### Player Lookup Tokens
When parsing free-form text from players, lookups use `~` for characters and `*` for items.
These forms accept the same modifiers as internal tokens and target strings such as `2.tall.man`.

- `~tall.man` – description / “you”.
- `~!tall.man` – object pronoun.
- `~#tall.man` – subject pronoun.
- `~?tall.man` – reflexive.
- `~tall.man's` or `~!tall.man's` – possessive (“man's” / “your” or “his” / “your”).

### First/Third Person Variants
Use `|` to supply alternative text for the referenced perceiver versus everyone else.

- `verb1|verb2` – first person for the source, third person for others (`@ smile|smiles`).
- `$0|your|his` or `~tall.man|your|his` – “your” for the target, “his” for others.

### Plurality and Pronoun Number
- `&0|is|are` – uses “is” when token 0 is singular, “are” when plural.
- `%0|stop|stops` – conjugates based on the pronoun number of token 0 (“stop” for you/they, “stops” for he/she/it).

### Optional and Conditional Tokens
- `$?2|on $2||$` – includes `on $2` only if perceivable 2 exists.
- `$0=1` – if tokens 0 and 1 refer to the same entity, outputs “yourself”; `$0=1's` gives “your own”.

### Speech and Culture
- Text inside quotes (`"spoken text"`) is parsed as speech and routed through language handling.
- Culture-specific text can be written as `&cultureA,cultureB:text|fallback&`.

### Examples
```csharp
new Emote("@ smile|smiles at $0.", actor, target);
// actor: "You smile at Bob."
// target: "Alice smiles at you."
// others: "Alice smiles at Bob."

new Emote("@ pat|pats $0 on &0 shoulder.", actor, target);
// actor: "You pat Bob on his shoulder."
// target: "Alice pats you on your shoulder."

new Emote("$0 %0|stop|stops here.", actor, group);
// group: "You stop here."
// others: "The guards stop here."
```

## Notes
- When in doubt, defer to this file unless overridden at a lower level.

## Instructions for Codex

To compile the project locally or in automated checks:

1. Run `scripts/setup.sh` once to install the .NET 9 SDK in `~/.dotnet`.
2. Use `scripts/test.sh` to build the main engine project.

`test.sh` sets `DOTNET_EnableWindowsTargeting` so the build succeeds on Linux.
