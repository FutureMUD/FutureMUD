# Scope

This document defines the **specific rules for Project DatabaseSeeder Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains MSTest-based unit and regression tests for `DatabaseSeeder` content generation, template authoring, seeder repeatability, and seed-source invariants.

## Key Architectural Principles
* Put seeder-specific regressions here rather than in the core runtime suite.
* In-memory EF fixtures are acceptable when they model seeder persistence behaviour.
* Prefer validating seeded content shape, idempotence, and builder-facing defaults over incidental implementation details.
* This suite is part of the default unit-test pass and should be run whenever seeder content or seeder workflows change.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
