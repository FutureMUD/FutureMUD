# Scope

This document defines the **specific rules for Project MudSharpDatabaseLibrary**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Houses the Entity Framework Core `DbContext`, models, and migrations for the FutureMUD database.

## Key Architectural Principles
* Model classes map directly to database tables; avoid introducing game logic here.
* Use partial classes to extend generated models without altering scaffolded code.
* `FuturemudDatabaseContext` configures MySQL with lazy-loading proxies.
* Migrations reside in the `Migrations/` folder and should be created via EF Core tooling.
* Connection strings are provided at runtime—do not commit credentials.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
