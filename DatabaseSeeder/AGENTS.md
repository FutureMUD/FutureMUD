# Scope
This document defines the **specific rules for Project DatabaseSeeder**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance
- Rules here **extend or override** solution-level rules.
- Precedence: **Module > Project > Solution**.

## Purpose of Project
Interactive installer that seeds initial database data and configuration for a new FutureMUD world.

## Key Architectural Principles
* Seed steps reside in the `Seeders/` folder and implement `IDatabaseSeeder`.
* Seeders should be idempotent so they can be safely rerun.
* Use `FuturemudDatabaseContext` for data access and dispose contexts with `using`.
* Interactions occur via the console; keep prompts and output clear for users.
* Register new seeders in `Program` so they are included in the workflow.

## Notes
- All modules inherit both the solution-level and project-level rules unless explicitly overridden.
