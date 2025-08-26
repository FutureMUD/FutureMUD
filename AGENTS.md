# FutureMUD Solution Instructions for AI Agents

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

###FutureMUDDatabaseLibrary Project

This is where the Entity Framework code goes - the DatabaseContext, Models, Migrations and the like. Anything to do with database should go in here. 

Note - there is an exception to this in the MudSharpCore project, the class being *MudSharp.Database.FMDB.cs*. This class is only used in MudSharpCore but is the usual way that the database is invoked, by putting it in a using context like so:

using (new FMDB())
{
   // Code accesses FMDB.Context static method only within this using block. 
}

###ExpressionEngine Project

This library is used to supply an IExpression interface and an Expression implementation that provide some functionality over the top of an NCalc library. It is mostly use to take user-defined formulas and efficiently interpret them for a mathematical outcome in the engine, for example damage formulas, hit points and the like.

###MudSharpCore Project

This is the main MUD engine - the core FutureMUD product. It is a console application and listens on specified TCP ports for input.

###MudSharpCore Unit Tests Project

This is a unit test project for MudSharpCore. Implement any unit tests in here.

###DatabaseSeeder Project

The Database Seeder is designed to function as the installer for the engine. It is a console application that the user runs before running their game for the first time.

What it does is that it guides the user through a number of choices about how they want their game's data to be and then it programmatically creates the initial database data, saving the end user from having to do it in many cases or at least providing examples. 

It also sets up a few supporting files for the engine and sets up a startup script.

The seeder can also be run again in the future to add additional options and context, or when new content is released in the seeder that the user wants to import.

###DiscordBotCore Project

The discord bot is a bot designed to run alongside the engine to provide some discord interactions for the game owners, like echoing in game messages or providing admins some control over the engine via discord commands. It uses the DSharpPlus library for discord and is a console application.

## Code Style
- Use **tabs** for indentation.
- Use file-scoped namespaces (`namespace MudSharp.Commands;`) for new files.
- Place `using` directives at the top of the file.
- Name classes, methods, and properties in **PascalCase**. Private fields use camelCase with a leading underscore.
- Prefer `var` for local variables when the type is clear and favour early returns for validation.
- Use string interpolation over string format functions were possible. Prefer verbatim strings over normal strings with special characters.
- Important helper methods are found in the FutureMUDLibrary project under the MudSharp.Framework namespace.
- Use LINQ where possible unless you're being instructed to optimise a particular high-bottleneck section of code. Prefer to put your LINQ statements on separate lines for clarity and don't be afraid to break up logic into multiple steps (e.g. having multiple .Where calls rather than cramming all your logic into one lambda)
- Interface-first design - if you're introducing a new feature get the interface first, implement the logic to work with the interface and then do the implementation last.
- Use design patterns where they make sense. Not everything needs the maximum level of abstraction but we should prefer to follow common C# design patterns where we can.

## Instructions for Codex

To compile the project locally or in automated checks:

1. Run `scripts/setup.sh` once to install the .NET 9 SDK in `~/.dotnet`.
2. Use `scripts/test.sh` to build the main engine project.

`test.sh` sets `DOTNET_EnableWindowsTargeting` so the build succeeds on Linux.
