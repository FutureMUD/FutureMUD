# Scope

This document defines the **specific rules for Project DatabaseSeeder Unit Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains MSTest-based functional and regression tests for `DatabaseSeeder` helpers, architecture, installation and repeatability. Apply the [seeder test policy](../Design%20Documents/Seeding/Seeder_Test_Policy.md).

## Key Architectural Principles
* Put seeder-specific regressions here rather than in the core runtime suite.
* In-memory EF fixtures are acceptable when they model seeder persistence behaviour.
* Prefer synthetic inputs and independently expected functional behaviour. Do not freeze catalogue names, counts, prose, balance values or task-specific source inventories. Catalogue-dependent tests require a concrete high-risk failure mode, as described in the test policy.
* Fast checks in this suite are part of the default unit-test pass. Full prerequisite/install/rerun matrices and exhaustive stock-catalogue integration checks use `[TestCategory("Integration")]` and require explicit opt-in.
* Run affected integration checks when their installer, persistence or rerun contracts change: `scripts/test-unit.ps1 -IncludeExtended -Project 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -Filter 'FullyQualifiedName~AffectedInstaller'` (Bash: `--include-extended --project ... --filter ...`). An ordinary name filter alone retains the fast exclusions.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
