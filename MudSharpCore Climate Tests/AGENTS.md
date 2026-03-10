# Scope

This document defines the **specific rules for Project MudSharpCore Climate Tests**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Contains the long-running seeded climate and weather-regression tests that are not part of the default core unit-test pass.

## Key Architectural Principles
* Use MSTest attributes `[TestClass]` and `[TestMethod]`.
* Keep these tests deterministic and benchmark-driven, with broad climate bands rather than brittle exact-value assertions.
* Prefer adding slow climate seeder regressions here rather than back into the core unit-test project.
* Run this project only when climate, weather seeding, weather analysis, or weather-statistics export behaviour changes, or when the user explicitly asks for the extended suite.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
