# Scope
This document defines the **specific rules for Project DatabaseSeeder**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance
- Rules here **extend or override** solution-level rules.
- Precedence: **Module > Project > Solution**.

## Purpose of Project
Interactive installer that seeds initial database data and configuration for a new FutureMUD world.

## Key Architectural Principles
* Seed steps reside in the `Seeders/` folder and implement `IDatabaseSeeder`.
* Seeders should be idempotent so they can be safely rerun.
* Use `FuturemudDatabaseContext` for data access and dispose contexts with `using`.
* Interactions occur via the console; keep prompts and output clear for users.
* Register new seeders in `Program` so they are included in the workflow.

## Seeder Policy
- Treat repeatability as a first-class design concern. New or modified seeders should declare honest repeatability and update semantics through the seeder metadata, not only through prose or warning colors.
- Prefer additive installs and deterministic lookup-and-upsert behavior over one-shot seeders whenever the stock data has stable ownership boundaries.
- Ask as few questions as practical. If multiple stock options can coexist safely, prefer installing multiple options over forcing an early fork in the setup flow.
- Reuse prior answers through shared answer keys and the generic `SeederChoice` answer-memory flow instead of ad hoc per-seeder lookup code.
- Keep prerequisite checks explicit and specific. Prefer describing what is missing over returning a generic blocked state with no user guidance.
- When a seeder cannot yet be safely rerun, document that clearly in both metadata and design docs rather than implying support through `ExtraPackagesAvailable` alone.
- Foundational and high-complexity seeders should not invent bespoke repeatability patterns. Extend the shared framework first, then convert the seeder.
- Editable-item entities that use composite `(Id, RevisionNumber)` keys are not guaranteed to have database-generated entity IDs. Check the EF mapping or migration before assuming `Id` auto-increments; some tables, such as `DisfigurementTemplates`, need the seeder to allocate the next available entity `Id` manually.

## Seeder Strategy Reference
- The full repeatability strategy, audit matrix, and backlog live in [../Design Documents/DatabaseSeeder_Repeatability_Strategy.md](../Design%20Documents/DatabaseSeeder_Repeatability_Strategy.md).

## Economy Seeder Reference
- The economy documentation suite lives in:
  - [../Design Documents/Economy_System_Runtime.md](../Design%20Documents/Economy_System_Runtime.md)
  - [../Design Documents/Economy_System_Workflows_and_Integration.md](../Design%20Documents/Economy_System_Workflows_and_Integration.md)
  - [../Design Documents/Economy_System_Seeder_State_and_Gaps.md](../Design%20Documents/Economy_System_Seeder_State_and_Gaps.md)
- `CurrencySeeder` is currently the only dedicated economy seeder. Treat broader economy seeding as an explicit product decision and ground new work in the seeder opportunity matrix documented above.

## Notes
- All modules inherit both the solution-level and project-level rules unless explicitly overridden.
