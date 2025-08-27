# Scope

This document defines the **specific rules for Project MudSharpCore Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains MSTest-based unit tests for MudSharpCore and shared utilities.

## Key Architectural Principles
* Use MSTest attributes `[TestClass]` and `[TestMethod]`.
* Employ `Moq` or similar libraries for mocking dependencies.
* Tests must be deterministic and avoid reliance on external state or ordering.
* Mirror the namespace or module of the code under test for organisation.
* Name test methods in the `Method_Scenario_ExpectedResult` format when practical.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
