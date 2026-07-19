# Scope

This document defines the **specific rules for Project DiscordBotCore Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains MSTest-based tests that directly target Discord bot command handling, integration glue, protocol parsing, and message formatting in `DiscordBotCore`.

## Key Architectural Principles
* Add direct bot tests here as coverage is introduced.
* This suite is part of the default unit-test pass and should stay independent of a live Discord connection.
* Prefer testing bot-specific behaviour rather than duplicating engine runtime coverage.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
