# Scope

This document defines the **specific rules for Project FutureMUDLibrary Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains MSTest-based unit tests for shared abstractions, extension methods, utility types, and value objects that belong to `FutureMUDLibrary`.

## Key Architectural Principles
* Keep this suite focused on shared-library behaviour rather than concrete engine runtime workflows.
* Prefer lightweight stubs and mocks over database-backed or bootstrapped engine fixtures.
* Add tests here when the code under test lives in `FutureMUDLibrary`, even if the namespace is also used by `MudSharpCore`.
* This suite is part of the default unit-test pass and should stay fast and deterministic.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
