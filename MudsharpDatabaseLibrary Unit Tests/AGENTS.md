# Scope

This document defines the **specific rules for Project MudsharpDatabaseLibrary Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains MSTest-based tests that directly target `MudsharpDatabaseLibrary` persistence models, EF configuration, upgrade coordination, and migration-adjacent helper behaviour.

## Key Architectural Principles
* Add direct database-library tests here as coverage is introduced.
* This suite is part of the default unit-test pass and should avoid requiring a live MySQL server.
* Prefer tests that isolate persistence behaviour from higher-level engine runtime concerns.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
