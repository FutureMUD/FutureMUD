---
name: futuremud-database-seeder
description: "Use when changing FutureMUD DatabaseSeeder workflows, enabled-seeder dependency/order metadata, question contracts, or Debug replay profiles. Do not use for content-only seeder catalogue maintenance without a workflow or replay-profile change."
---

# FutureMUD Database Seeder

Use this skill for DatabaseSeeder behavior and repeatability work, especially the Debug-only replay profiles. It is intentionally narrower than a seeded-catalogue audit: do not use it for content-only item, material, tag, terrain, or stock-data refreshes unless they also change the seeder workflow or replay-profile contract.

## Read first

1. Read `DatabaseSeeder/AGENTS.md` and the relevant seeders before changing the workflow.
2. Read `Design Documents/Seeding/DatabaseSeeder_Repeatability_Strategy.md` for the repeatability model and replay-profile contract.
3. Identify the live `IDatabaseSeeder` metadata, dependency plan, `SeederQuestion` declarations, filters, validators, defaults, and shared-answer behavior that the change affects.

## Debug replay profiles

The Debug-only profiles in `DatabaseSeeder/DebugSeederReplay.cs` are strict, typed inventories. They must:

- use concrete seeder types, never a menu number or display name;
- include every currently enabled seeder exactly once in dependency-plan order;
- intentionally exclude only the mutually exclusive `SkillSeeder` alternative and include `SkillPackageSeeder`;
- contain every declared question ID for every included seeder, including conditionally inactive questions;
- keep the Medieval, Renaissance, and Early Modern profiles cumulative and deterministic.

Whenever an enabled seeder, dependency/order declaration, question ID, filter, validator, default, or recommended answer changes, review every replay profile. Update profile answers deliberately; do not make a drift failure disappear by silently using an interactive default. Keep the Debug credential warning and fresh-local-database restriction intact.

## Implementation rules

- Keep replay types and menu paths inside `#if DEBUG`; Release behavior must retain the normal connection-string prompt and must not expose replay surfaces.
- Both interactive and replay paths must use the shared executor for seeder execution, answer persistence, and exception handling.
- Replay is for a freshly migrated, unseeded development database. Refuse nonblank targets; never reset or overwrite them.
- Re-evaluate question filters and validators against the live context before each seeder. Ignore inventory answers for inactive questions, but fail before execution if an active answer is absent or invalid.
- Preserve commits from completed seeders. Stop at the first blocked prerequisite or exception and report completed, failed, and unstarted steps rather than attempting a misleading cross-seeder rollback.

## Required verification

For any DatabaseSeeder workflow or replay-profile change, run these gates:

1. Focused `SeederReplayTests` while iterating; it is required whenever the replay contract may drift.
2. `dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-restore -m:1` and the corresponding Release build, confirming replay is Debug-only.
3. `dotnet test 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -c Debug --no-restore -m:1`.
4. `git diff --check`.

When a local MySQL development server is available, exercise a profile against a uniquely named disposable database and confirm a second replay attempt is refused without mutation. Do not point a replay profile at a reachable or production database.
