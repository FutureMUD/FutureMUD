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
* Fast source/operation and isolated persistence-helper checks are part of the default unit-test pass. Full production-provider model construction, historical target-model initialisation and generated whole-schema SQL use `[TestCategory("Integration")]` and require `-IncludeExtended` / `--include-extended`. Classify all consumers of a shared expensive model together so its cold startup cost does not simply move to another default case. No test requires a live MySQL server.
* Prefer tests that isolate persistence behaviour from higher-level engine runtime concerns.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
