# Scope

This document defines the **specific rules for Project DiscordBotCore Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Scaffold project for MSTest-based tests that directly target Discord bot command handling, integration glue, and message formatting in `DiscordBotCore`.

## Key Architectural Principles
* Add direct bot tests here as coverage is introduced.
* Keep this suite opt-in until it contains meaningful coverage.
* Prefer testing bot-specific behaviour rather than duplicating engine runtime coverage.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
