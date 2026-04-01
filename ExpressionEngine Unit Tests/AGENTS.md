# Scope

This document defines the **specific rules for Project ExpressionEngine Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains MSTest-based unit tests for expression parsing, evaluation, random helpers, and formula semantics implemented by `ExpressionEngine`.

## Key Architectural Principles
* Keep this suite centred on expression evaluation behaviour rather than engine integrations that merely consume expressions.
* Prefer deterministic assertions around parser and function semantics.
* Use seeded or bounded assertions for randomised behaviour rather than brittle exact-value expectations.
* This suite is part of the default unit-test pass and should remain quick to run.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
