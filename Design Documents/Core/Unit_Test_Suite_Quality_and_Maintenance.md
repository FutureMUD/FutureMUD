# Unit Test Suite Quality and Maintenance

## Purpose

The FutureMUD test system protects durable engine, seeder, library, converter, bot, database-upgrade, and website behavior. A test belongs in the suite when it expresses a product or compatibility requirement that should remain true after a legitimate refactor.

Tests should normally exercise a public or internal behavioral seam. Direct inspection of production source is a structural-integration fallback, not the preferred definition of behavior.

## Test Gates

Run a single-node restore before either gate:

```powershell
dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
```

The default gate is:

```powershell
& .\scripts\test-unit.ps1
```

`scripts/unit-test-projects.txt` is the canonical default-project manifest consumed by both `test-unit.ps1` and `test-unit.sh`. Both runners build projects sequentially to avoid shared-output locks and then run the already-built, process-isolated test hosts in parallel. Any project failure makes the overall runner fail.

It runs every fast, meaningful test project:

| Project | Responsibility | Tests after the July 2026 review |
| --- | --- | ---: |
| `FutureMUDLibrary Unit Tests` | Shared abstractions, extension methods, utilities, and value objects | 396 |
| `ExpressionEngine Unit Tests` | Expression parsing, evaluation, custom functions, and failure semantics | 21 |
| `DatabaseSeeder Unit Tests` | Seeder behavior, repeatability, content contracts, and maintained catalogues | 539 |
| `MudSharpCore Unit Tests` | Engine runtime and gameplay behavior | 1,672 |
| `MudsharpDatabaseLibrary Unit Tests` | Upgrade coordination, model compatibility, and persistence helpers | 27 |
| `DiscordBotCore Unit Tests` | Discord protocol parsing and bot integration glue | 22 |
| `RPI Engine Worldfile Converter Tests` | Legacy parsing and conversion compatibility | 43 |
| `FutureMUD.Web.Tests` | Website endpoints, publishing, documentation transport, and security | 49 |

The extended climate gate remains separate because it performs deterministic multi-year simulations:

```powershell
& .\scripts\test-unit-climate.ps1
```

It currently contains 34 resolved test cases and takes roughly 42 seconds on the July 2026 Windows development environment.

## Test Design Rules

### Durable requirements

Prefer tests that state observable behavior:

- command input produces the documented state change, validation error, or echo;
- persisted XML or EF state round-trips through the supported compatibility contract;
- a seeder installs the expected content shape, repairs stock-owned content, and remains idempotent;
- a parser accepts supported input forms and rejects invalid forms;
- a calculation preserves its documented bounds and semantics;
- a security boundary rejects unauthorized or unsafe input.

A bug regression is valid when its name and assertions express the general requirement exposed by the bug. The historical bug, PR, private helper name, or exact implementation is not itself the requirement.

### Source-coupled structural tests

Source inspection is acceptable only when the required behavior cannot yet be reached without booting substantial external state and the check protects a high-value integration boundary. Such a test should:

- state the behavioral or compatibility reason in its name and failure message;
- inspect the smallest stable surface possible;
- avoid requiring a private method name, local variable name, statement order, dated completion note, or exact source formatting;
- be replaced by a behavioral test when a testable seam becomes available.

Do not add phase-completion tests whose only assertion is that named methods, dated design-document text, or implementation wiring still exists.

### Random and performance-sensitive tests

Random helpers should be checked with bounded samples and deterministic invariants. Tests must not require a random run to produce more than one distinct value; that is probabilistic and does not define correct behavior.

Database-backed fixtures may be shared only when they are read-only, the class is not parallelized, and mutation/idempotence cases keep isolated contexts. Expensive climate simulations remain in the dedicated opt-in project.

## July 2026 Review Findings

The review began with a clean default baseline of 2,663 passing tests. The four default projects were healthy, but the gate omitted four projects that already contained meaningful tests. The converter test project was also absent from `MudSharp.sln`, so a solution restore did not restore it; a direct restored run revealed two stale assertions.

The completed follow-up default gate passes 2,769 tests with zero failures and zero skips. Three warm Windows runs completed in 68.8, 80.5, and 67.1 seconds; the 68.8-second median is 26.6% below the original 93.7-second post-review result. Together with the unchanged 34-case climate gate, the maintained test system resolves 2,803 passing cases.

The PowerShell scripts relied on `$ErrorActionPreference = 'Stop'`, which does not reliably turn native `dotnet` exit codes into terminating PowerShell errors. An earlier failing suite could therefore be followed by a passing suite and leave a misleading successful script exit. The scripts now check every `dotnet` exit code explicitly, and all test invocations use single-node MSBuild to avoid the known Windows output-lock failure mode.

A static source-read scan found 50 core/seeder test files containing 397 test methods before the review. Not every method in those files is source-coupled, but the concentration confirms material structural-test debt. The clearest obsolete case was `PrimaryProductionSeederSourceTests`: 18 phase-checklist tests asserted private method names, exact wiring statements, and dated documentation. That file was retired. Its durable coverage now lives in public item, craft, project, repeatability, and metadata contracts. After the change, 49 core/seeder source-reading files containing 379 test methods remain; they should be migrated incrementally rather than deleted without replacement.

The suite also contained ignored placeholder tests in the database-library and Discord projects even though both projects now have real coverage. The placeholders were removed and both projects were promoted into the default gate.

ExpressionEngine had only six resolved tests. Its coverage now includes `not`, `drand`, dice limits, invalid custom-function arguments, parser error reporting, missing-function error events, enum parameters, parameter discovery, case-insensitive comparisons, and decimal evaluation. The old random tests performed thousands of evaluations and required result diversity; they now use bounded invariant checks.

The seeder timing profile showed 69.65 seconds of test execution, of which `EconomySeederTests` accounted for 24.91 seconds. Read-only classical, all-era, scale, and currency contexts are now shared within a non-parallel test class, while rerun and mutation tests retain isolated contexts. The focused class fell to 19.94 seconds in the first post-change run. Repeated seeder source-file searches are also cached for the duration of the process.

## July 2026 Follow-up Implementation

A method-level recount corrected the planning estimate of 72 priority source-reading tests to 68. The earlier estimate attributed helper reads to more than one surrounding test. In the named law, command-security, body-form, liquid-contamination, magic, combat, poison, settings, and ranged-weapon files, 67 source-coupled tests were converted or retired. Existing direct suites already covered the durable magic factory round trips, dispel outcomes, combat-setting resolver, weapon-poison configuration, natural-ranged runtime, legal trial state, and surface-liquid state behavior. New callable seams added focused coverage for patrol candidate/custody decisions, body-form deletion and provisioning decisions, surface-liquid routing and washing-machine drying, force target collection selection, and spreadsheet-formula neutralisation.

The sole retained structural exception is `CommandExecutionArchitectureTests.AuthoredCommandPaths_UseTheSecureExecutor`. It is isolated in its own class and marked `SourceCoupled` because the security requirement spans authored command entry points that do not share one runtime fixture. It verifies that those paths delegate to the secure executor; it does not assert private helper names, local variables, or formatting. No other named priority file reads production source.

Future production-source-reading tests require an explicit compatibility or security justification, isolation in a `SourceCoupled` class/category, and an explanation of why a callable seam is not practical. Unrelated seeder catalogue source checks remain for a later content-test audit; they are not precedent for runtime implementation-text tests.

The Discord suite grew from 5 to 22 cases. It now covers framing, size limits without sleeps, authenticated routing, exact fixed-time secret validation, Unicode, shutdown compatibility, main-settings validation, account-link recovery and invariant persistence, and concurrent request ids. The tests exposed two defects: authentication used a case-insensitive comparison, and one malformed account-link row discarded every later link. Both are corrected and documented in `Discord_Bot_Protocol_and_Settings.md`.

The database-library suite grew from 12 to 27 cases. It now covers request validation, settings repair, migration progress and ordering, attempted-state persistence, recovery matching, rollback failures, disabled backups, snapshot exception cleanup, and pruning. It exposed malformed backup-settings JSON as a permanent startup failure; loading now replaces damaged JSON with defaults. The lifecycle is documented in `Database_Upgrade_Coordination.md`.

Normal CI is defined in `.github/workflows/unit-tests.yml` and runs the canonical default gate on .NET 10 for Windows and Linux in parallel matrix jobs. `.github/workflows/climate-tests.yml` keeps the unchanged multi-year climate suite separate on a weekly and manual schedule. Neither workflow collects coverage or runs mutation testing.
## Remaining Risks and Priorities

The suite is broad but not demonstrably complete because this branch intentionally does not add line/branch coverage or mutation tooling. Raw coverage percentage should not become a target by itself.

The next highest-value work is:

1. Audit the remaining legacy source-reading files outside the named priority set, beginning with runtime tests whose requirements can be reached without external services; handle seeder catalogue checks as a separate content-test audit.
2. Add behavior around important pure helpers, parsers, and persistence boundaries when uncovered code is encountered rather than adding snapshots of private implementation text.
3. Add only targeted EF mapping invariants backed by an explicit database compatibility requirement.
4. Keep the climate simulation separate and benchmark-driven; do not weaken its year ranges merely to improve the default-gate time.

This document should be updated whenever a test project moves into or out of the default gate, the test taxonomy changes, or a later measured audit materially changes the priorities above.
