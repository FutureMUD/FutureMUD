---
name: futuremud-database-seeder
description: "Use when changing FutureMUD DatabaseSeeder workflows, enabled-seeder dependency/order metadata, question contracts, or Debug replay profiles. Do not use for content-only seeder catalogue maintenance without a workflow or replay-profile change."
---

# FutureMUD Database Seeder

Maintain the seeder workflow/replay contract without changing unrelated catalogue content or weakening database protections. Catalogue-only item, material, tag, terrain or stock-data edits do not activate this skill unless they affect that contract.

## Task context

Apply `DatabaseSeeder/AGENTS.md`. Inspect the affected `IDatabaseSeeder` metadata, dependency plan, `SeederQuestion` declarations, filters, validators, defaults and shared-answer behaviour. Read the relevant workflow/replay section of `Design Documents/Seeding/DatabaseSeeder_Repeatability_Strategy.md` when that contract changes or is unclear; a narrow question does not require loading the full audit/backlog.

For a read-only review, inspect the requested contract and report drift or risks without creating a database or running replay. The verification gates below apply to implementation changes.

## Debug replay profiles: preserve the strict inventory

Profiles in `DatabaseSeeder/DebugSeederReplay.cs` must:

- use concrete seeder types, never menu numbers or display names;
- include every enabled seeder exactly once in dependency-plan order;
- intentionally exclude only the mutually exclusive `SkillSeeder` alternative and include `SkillPackageSeeder`;
- contain every declared question ID for every included seeder, including conditionally inactive questions;
- keep Medieval, Renaissance and Early Modern profiles cumulative and deterministic.

Whenever an enabled seeder, dependency/order declaration, question ID, filter, validator, default or recommended answer changes, review every replay profile. Update answers deliberately; never hide drift by silently falling back to an interactive default. Preserve the Debug credential warning and fresh-local-database restriction.

## Execution and failure invariants

- Keep replay types and menu paths within `#if DEBUG`. Release retains the ordinary connection-string prompt and exposes no replay surface.
- Interactive and replay paths use the shared executor for execution, answer persistence and exception handling.
- Replay targets only a freshly migrated, unseeded development database. Refuse nonblank targets without mutation; never reset or overwrite them.
- Reevaluate filters and validators against live context before each seeder. Ignore inventory answers for inactive questions, but fail before execution if an active answer is missing or invalid.
- Preserve commits from completed seeders. Stop at the first blocked prerequisite or exception, and report completed, failed and unstarted steps. Do not imply a cross-seeder rollback occurred.

## Required gates for workflow/replay implementation changes

1. Run focused `SeederReplayTests` while iterating when the replay contract may drift.
2. After the relevant restore, build `DatabaseSeeder/DatabaseSeeder.csproj` in both Debug and Release, confirming replay remains Debug-only.
3. Run the DatabaseSeeder test suite and check the diff:

```text
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-restore -m:1
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Release --no-restore -m:1
dotnet test "DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj" -c Debug --no-restore -m:1
git diff --check
```

When an appropriate local MySQL development server is available, exercise a profile against a uniquely named disposable database, then confirm a second attempt is refused without mutation. Never use a shared, unknown, remote or production database for this replay check. If no safe local target is available, report that this integration check was not run; unit tests do not establish it passed.

Report the changed workflow/question/profile contracts, how drift was handled, checks actually run, and remaining validation gaps. For a review-only task, report findings and evidence rather than inventing an implementation or execution receipt.
