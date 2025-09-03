# Scope

This document defines the **specific rules for Project DiscordBotCore**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Provides a standalone Discord bot that interacts with the FutureMUD engine via DSharpPlus.

## Key Architectural Principles
* `DiscordBot` is a singleton that manages the connection and any TCP bridges to the game server.
* Commands and features live in the `Modules/` folder and use DSharpPlus CommandNext.
* Operations should be asynchronous; avoid blocking the event loop.
* Configuration is read from `DiscordBotSettings`; keep tokens and secrets out of source control.
* Logging is handled through Serilog and `Microsoft.Extensions.Logging`.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
