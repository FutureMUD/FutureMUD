---
title: About FutureMUD
summary: A modern, open-source engine for persistent roleplay-intensive text worlds.
---
## An engine, not a single game

FutureMUD is a platform for creating Multi-User Dungeons, with a particular focus on the Roleplay Intensive tradition. It supplies the simulation and authoring systems that game owners need to build a distinct world without turning every design decision into a programming project.

Its systems cover characters and bodies, combat, crafting, economy, law, weather, magic, vehicles, items, languages, time, building, and much more. FutureProg gives builders a strongly typed scripting language for joining those systems together inside a running world.

## Current technology

The engine and its supporting products are written in C# and target **.NET 10**. The game server is a console application that accepts traditional MUD client connections over TCP. Persistent game data uses MySQL through Entity Framework Core and the Pomelo provider. The public website is an ASP.NET Core Razor Pages application and does not connect to a game database.

## Built for world builders

FutureMUD includes an interactive database seeder, in-game builder commands, a Discord integration, terrain tooling, and APIs alongside the core engine. Code-backed command, FutureProg, and item-component help is exported from each stable Engine build, so the reference on this website describes the version you can download.

## Open development

FutureMUD is developed in public. Source, issue tracking, and contribution history are available on [GitHub](https://github.com/FutureMUD/FutureMUD). The engine is under active development; operators should test upgrades against a backup of their own world.
