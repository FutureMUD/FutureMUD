# Scope

This document defines the **specific rules for Project MudSharpCore**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Implements the core FutureMUD game engine and console server handling gameplay, networking, and runtime systems.

## Key Architectural Principles
* Code is organised by domain (Accounts, Body, Combat, etc.); place new features in the appropriate folder.
* Implement interfaces from `FutureMUDLibrary`; create new interfaces there before adding concrete types here.
* Access the database via the `FMDB` helper and wrap interactions in `using (new FMDB())` blocks.
* Command implementations live under `Commands/` and integrate with the command tree framework.
* For command/building argument parsing, use `StringStack` and avoid ad-hoc regex or split parsing unless whole-input shape branching is materially clearer.
* Prefer `PopSpeech()` for stepwise argument parsing and reserve `Pop()` for explicit raw-token requirements.
* Prefer `SafeRemainingArgument` for final free-text arguments unless raw quotes are semantically required by downstream parsing.
* Do not call side-effecting `Pop*()` methods inside LINQ predicates/selectors; pop once into a temporary value or use `Peek*()` and then pop deliberately.
* Use partial classes to keep large classes manageable and to separate concerns.
* Prefer asynchronous patterns for networking and long-running operations.
* Use the engine's logging abstractions rather than `Console.WriteLine` for output.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
