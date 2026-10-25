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
* Fast checks in this suite are part of the default unit-test pass. Full prerequisite/install/rerun matrices and exhaustive stock-catalogue integration checks use `[TestCategory("Integration")]` and require explicit opt-in.
* Run affected integration checks when their installer, persistence or rerun contracts change: `scripts/test-unit.ps1 -IncludeExtended -Project 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -Filter 'FullyQualifiedName~AffectedInstaller'` (Bash: `--include-extended --project ... --filter ...`). An ordinary name filter alone retains the fast exclusions.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
