# Scope

This document defines the **specific rules for Project MudSharpCore Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains the **core runtime** MSTest-based unit tests for `MudSharpCore`. This suite is part of the default unit-test pass, but it is no longer the home for library or seeder coverage.

## Key Architectural Principles
* Use MSTest attributes `[TestClass]` and `[TestMethod]`.
* Employ `Moq` or similar libraries for mocking dependencies.
* Tests must be deterministic and avoid reliance on external state or ordering.
* Mirror the namespace or module of the code under test for organisation.
* Name test methods in the `Method_Scenario_ExpectedResult` format when practical.
* Keep shared-library tests in `FutureMUDLibrary Unit Tests`, expression tests in `ExpressionEngine Unit Tests`, and seeder tests in `DatabaseSeeder Unit Tests`.
* Keep long-running climate seeder regressions out of this project; they belong in the dedicated climate test project.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
