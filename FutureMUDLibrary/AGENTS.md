# Scope

This document defines the **specific rules for Project FutureMUDLibrary**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Provide shared interfaces, extension methods, and utility types used across multiple FutureMUD products. Code here is engine-agnostic so that other projects can implement or reuse these abstractions.

## Key Architectural Principles
* Keep the library free of concrete engine logic—only abstractions and helpers.
* Interface definitions belong here; names start with `I` and live under the appropriate domain folder.
* Extension methods are implemented as static classes alongside their related interfaces.
* Avoid direct database or network access. Depend only on the standard library and other solution libraries.
* Public APIs should remain stable to avoid breaking downstream projects.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
