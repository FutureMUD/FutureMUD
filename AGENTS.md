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

###FutureMUDLibrary Unit Tests Project

This is the unit test project for `FutureMUDLibrary`. Put tests for shared extension methods, helper classes, value objects and other library-first behaviour here rather than in the core engine suite.

- [Project FutureMUDLibrary Unit Tests](./FutureMUDLibrary%20Unit%20Tests/AGENTS.md)

###ExpressionEngine Unit Tests Project

This is the unit test project for `ExpressionEngine`. Put parser, function, and formula-evaluation tests here.

- [Project ExpressionEngine Unit Tests](./ExpressionEngine%20Unit%20Tests/AGENTS.md)

###MudSharpCore Unit Tests Project

This is the unit test project for MudSharpCore runtime behaviour. Implement engine, gameplay, FutureProg and other concrete runtime tests here.

- [Project MudSharpCore Unit Tests](./MudSharpCore Unit Tests/AGENTS.md)

###DatabaseSeeder Project

The Database Seeder is designed to function as the installer for the engine. It is a console application that the user runs before running their game for the first time.

What it does is that it guides the user through a number of choices about how they want their game's data to be and then it programmatically creates the initial database data, saving the end user from having to do it in many cases or at least providing examples. 

It also sets up a few supporting files for the engine and sets up a startup script.

The seeder can also be run again in the future to add additional options and context, or when new content is released in the seeder that the user wants to import.

- [Project DatabaseSeeder](./DatabaseSeeder/AGENTS.md)

###DatabaseSeeder Unit Tests Project

This is the unit test project for `DatabaseSeeder`. Put tests for seeder content, seed-source invariants, repeatability and seeder-specific helpers here.

- [Project DatabaseSeeder Unit Tests](./DatabaseSeeder%20Unit%20Tests/AGENTS.md)

###DiscordBotCore Project

The discord bot is a bot designed to run alongside the engine to provide some discord interactions for the game owners, like echoing in game messages or providing admins some control over the engine via discord commands. It uses the DSharpPlus library for discord and is a console application.

- [Project DiscordBotCore](./DiscordBotCore/AGENTS.md)

###MudsharpDatabaseLibrary Unit Tests Project

This is the scaffold unit test project for `MudsharpDatabaseLibrary`. Use it for direct EF model, mapping and persistence-library tests when those are added.

- [Project MudsharpDatabaseLibrary Unit Tests](./MudsharpDatabaseLibrary%20Unit%20Tests/AGENTS.md)

###DiscordBotCore Unit Tests Project

This is the scaffold unit test project for `DiscordBotCore`. Use it for bot-specific tests when that suite grows beyond a placeholder scaffold.

- [Project DiscordBotCore Unit Tests](./DiscordBotCore%20Unit%20Tests/AGENTS.md)

###MudSharpCore Climate Tests Project

This project contains the long-running climate and weather regression checks used to tune seeded climate models. It is intentionally separate from the normal unit-test pass.

- [Project MudSharpCore Climate Tests](./MudSharpCore%20Climate%20Tests/AGENTS.md)

###

- [Project Temporary Scratch App](./Temporary%20Scratch%20App/Agents.MD)

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
- When creating text for an IEmote, refer to [Emote System](./Design%20Documents/Markup/Emote_System.md)

## Text Markup Reference
- [Emote System](./Design%20Documents/Markup/Emote_System.md)
- [Character Description System](./Design%20Documents/Markup/Character_Description_System.md)
- [Human Seeder Description Patterns](./Design%20Documents/Markup/Human_Seeder_Description_Patterns.md)
- [Room Description Markup](./Design%20Documents/Markup/Room_Description_Markup.md)

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
- Prefer PopSpeech() for argument-by-argument parsing and reserve Pop() for situations that explicitly need raw token behaviour.
- For final free-text arguments, prefer SafeRemainingArgument over RemainingArgument unless quote characters are semantically meaningful to downstream parsing (for example command forwarding, regex patterns, or raw emote text).
- Avoid side-effecting Pop*() calls inside LINQ expressions; pop into a temporary variable first or use Peek*() and then pop once deliberately.
- Prefer to use LINQ methods like .Any(), .FirstOrDefault(), .Where() and the like over manual loops where possible for clarity. When stacking multiple LINQ methods, put each method on its own line for clarity.
- Use LINQ query syntax only when it makes the code significantly clearer than method syntax. A common place this is done is when presenting tabular data to players.
- Use the method `StringUtilities.GetTextTable()` to present tabular data to players rather than building tables manually.
- Use the extension method `ListToString()` on IEnumerable collections to present lists of things to players rather than building lists manually. Similarly, you can use `ListToCommaSeparatedValues()` to get a comma-separated string.
- Rooms vs Cells: the legacy distinction is being removed; wherever possible prefer targeting `ICell`/`Cell` rather than `IRoom`/`Room` (and treat "room" mentions in designs as meaning cell) so future merges are simpler.
- When documenting or implementing room-building workflows, see [Room Building Builder Guide](./Design%20Documents/Building/Room_Building_Builder_Guide.md) for the builder-facing command model.

## Notes
- When in doubt, defer to this file unless overridden at a lower level.

## Test Suite Guide

Use the repository scripts in `scripts/` where practical. They are designed to run inside the repo sandbox without elevation and now have both `.sh` and `.ps1` variants. Run `dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false` once first if package state is not already warm.

| Suite | What it covers | Default? | Run it when | Usually leave it alone when |
| --- | --- | --- | --- | --- |
| `FutureMUDLibrary Unit Tests` | Shared extensions, helper types, string and collection utilities, small value objects | Yes | Changing `FutureMUDLibrary` or a shared helper used across products | You are only changing concrete engine runtime behaviour |
| `ExpressionEngine Unit Tests` | Expression parsing, formula semantics, custom expression helpers | Yes | Changing `ExpressionEngine` or expression semantics consumed by the engine | You did not touch expression parsing or evaluation logic |
| `DatabaseSeeder Unit Tests` | Seeder templates, seeder content, repeatability checks, seeder source invariants | Yes | Changing seeder workflows, seeded content, or authoring helpers | You are only changing runtime systems that consume already-seeded data |
| `MudSharpCore Unit Tests` | Core runtime, gameplay systems, FutureProg, arenas, AI and engine services | Yes | Changing `MudSharpCore` behaviour or integrations | You only touched shared library or seeder code and none of the runtime call sites changed |
| `MudSharpCore Climate Tests` | Slow climate and weather regression and tuning coverage | No | Changing climate, weather seeding, analyzers, or climate tuning | Normal feature work unrelated to weather or climate |
| `MudsharpDatabaseLibrary Unit Tests` | Direct persistence-library coverage | No | You add direct tests for `MudsharpDatabaseLibrary` | The project still only contains its placeholder scaffold |
| `DiscordBotCore Unit Tests` | Direct Discord bot coverage | No | You add direct tests for `DiscordBotCore` | The project still only contains its placeholder scaffold |

## Instructions for Codex when Working in Windows

Use Windows-native `dotnet` commands directly rather than Linux-only shell invocations, or prefer the paired PowerShell scripts in `scripts\`.

1. Run `dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false` when package state needs refreshing.
2. For normal engine verification, prefer targeted project builds instead of `dotnet build MudSharp.sln`.
3. Build the main engine with `dotnet build MudSharpCore/MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510`.
4. Build the seeder with `dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510`.
5. After `dotnet restore MudSharp.sln`, run the default multi-project unit-test pass with `scripts\test-unit.ps1`.
6. After `dotnet restore MudSharp.sln`, run only the core runtime unit tests with `scripts\test-unit-core.ps1`.
7. After `dotnet restore MudSharp.sln`, use the climate and weather regression suite only for climate-specific work or when explicitly requested with `scripts\test-unit-climate.ps1`.
8. After `dotnet restore MudSharp.sln`, use `scripts\test.ps1` for the smoke-build path and `scripts\setup.ps1` only when a repo-local SDK bootstrap is required.

Notes:

- The solution includes `FutureMUD_Analyzers.Vsix`, which depends on Visual Studio SDK targets and may fail under plain `dotnet build` on machines without the Visual Studio extension toolchain installed.
- Because of that VSIX dependency, a full solution build is not the default verification path for Codex on Windows.
- Suppressing `NU1902` and `NU1510` in local verification commands is acceptable unless the task is specifically about package auditing or dependency hygiene.
- Default `dotnet restore` and other implicit restore graph walks can fail silently during the parallel MSBuild project-graph step inside sandboxed Codex runs. Prefer the repo scripts, or force single-node restore/builds with `-m:1` and `-p:RestoreBuildInParallel=false`.
- NuGet vulnerability audit lookups can also be blocked in sandboxed runs, which shows up as `NU1900` warnings against `https://api.nuget.org/v3/index.json`; using `-p:NuGetAudit=false` for local verification is acceptable unless the task is specifically about dependency auditing.
- `MudSharpCore Unit Tests` can fail silently during the default parallel MSBuild project-graph walk inside sandboxed Codex runs. Prefer the repo scripts, or add `-m:1` when invoking that project with direct `dotnet build` or `dotnet test` commands.

## Instructions for Codex when Changing Subsystems with Design Documents

When changing a subsystem that has a design document in `Design Documents/`, start from the [Design Documents index](./Design%20Documents/README.md) to find the owning subsystem folder:

1. Update the relevant design document(s) as part of the same task.
2. Ensure the document reflects the new runtime behavior, command surface, and persistence implications.
3. Call out the documentation update in your task summary.

## Item System Design Document Reference

When changing item-related systems, treat the following documents as the primary design references and keep them in sync with the code:

- [Item System Overview](./Design%20Documents/Items/Item_System_Overview.md)
- [Item System Runtime Model](./Design%20Documents/Items/Item_System_Runtime_Model.md)
- [Item System Component Authoring](./Design%20Documents/Items/Item_System_Component_Authoring.md)
- [Item System Content Workflows](./Design%20Documents/Items/Item_System_Content_Workflows.md)
- [Item System Presentation and Integration](./Design%20Documents/Items/Item_System_Presentation_and_Integration.md)

For the purposes of this instruction, "item-related" includes work in or directly coupled to:

- `MudSharpCore/GameItems`
- `FutureMUDLibrary/GameItems`
- item builder commands and editable-item helpers
- item-related FutureProg functions
- item skins and item groups
- item templates in `Item Templates/`

## Economy Design Document Reference

When changing economy-related systems, treat the following documents as the primary design references and keep them in sync with the code:

- `Design Documents/Economy/Economy_System_Runtime.md`
- `Design Documents/Economy/Economy_System_Workflows_and_Integration.md`
- `Design Documents/Economy/Economy_System_Seeder_State_and_Gaps.md`

For the purposes of this instruction, "economy-related" includes work in or directly coupled to:

- `MudSharp.Economy`
- economy commands and editable-item helpers
- economy-related FutureProg functions
- economy game-item components and effects
- economy persistence models and migrations
- economy seeder content
