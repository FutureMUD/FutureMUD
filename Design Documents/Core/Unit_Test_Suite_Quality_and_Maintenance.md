# Unit Test Suite Quality and Maintenance

## Purpose

The FutureMUD test system protects durable engine, seeder, library, converter, bot, database-upgrade, and website behavior. A test belongs in the suite when it expresses a product or compatibility requirement that should remain true after a legitimate refactor.

Tests should normally exercise a public or internal behavioral seam. Direct inspection of production source is a structural-integration fallback, not the preferred definition of behavior.

## Test Gates

Use the paired repository entry points; they restore/build the selected graph before no-build/no-restore test execution. A whole-solution restore is not a prerequisite for a targeted run.

```powershell
& .\scripts\test-unit.ps1 -OutputMode Compact
```

```bash
bash ./scripts/test-unit.sh --output-mode compact
```

`scripts/unit-test-projects.txt` is the canonical default-project manifest. The default suite currently selects library, expression, database seeder, runtime, persistence, Discord, converter, website, TerrainPlanner and TestReporting projects. ItemSeeder and ArmageddonSeeder have separate product gates/solutions. MudClient has release qualification and can be selected explicitly; the separate ordinary PR gate is recorded in [PR 869](https://github.com/FutureMUD/FutureMUD/pull/869), which is not part of this document's base revision.

Select one project with `-Project '<path>.csproj'` / `--project '<path>.csproj'`. Builds are serial; a project failure must make the gate fail. The fast suite excludes Integration, Benchmark, ClimateSeeder and PsychicBenchmark. Extended coverage requires explicit opt-in; the dedicated climate workflow keeps multi-year simulation outside the normal pass. Do not treat historical case counts below as a current inventory or coverage percentage.

### Test execution and source evidence

The [reporter contract](../../scripts/TestReporting/README.md) owns receipt/status semantics, source fingerprints and subprocess handling. Git stdout/stderr are drained concurrently, preserving complete stdout for hashing while bounding surfaced diagnostics. Each Git operation has a 60-second ceiling linked to the original run deadline; cancellation does not begin a fresh fingerprint or extend the deadline. Cleanup targets only the owned process tree, with bounded drain/reap waits.

A missing initial Git prerequisite is BLOCKED; incomplete or unavailable source evidence cannot produce a stable-source PASS. Two complete captures are required to establish stability. Confirmed build/test failure retains precedence, and uncertain cleanup remains visible. The implemented helper and compiled FakeGit regressions supersede the completed Git-process fix brief. Tests of pipe saturation, NUL/tail preservation, operation/caller timeout, inherited pipes and owned descendants remain durable requirements, rather than source-text snapshots.

## Test Design Rules

### Employment approval and restart tests, 17 November 2026

The initial employment restart change added 15 fast cases covering approval identity/plan binding, malformed security fields, live issuer authority, revocation/expiry, cross-task approval isolation, reconciliation and cumulative spending, uncertain cancellation, execute/flush failures, ordinary retry and invalid persisted statuses. These are permanent behavioral/security requirements. They exercise small runtime/serialization seams without constructing the EF provider model. In the final full default run, the added cases took approximately 64 ms combined; the slowest took 35 ms, and none exceeded one second. Retain them in the default suite rather than adding a parallel set of full-EF in-memory mirrors.

The existing `EmploymentPersistence_RoundTripsCoreEmploymentTasksGoalsAndAuditRows` fixture was extended to verify scheduled-rule issuers/issuance authority and manager-goal issuer, expression, currency budgets and risk limits. Its isolated cold run took 3.356 seconds, including EF model initialization; a new model-heavy test would duplicate that cost. Retain these assertions in the existing case because they catch actual save/load omissions that a serializer-only test cannot establish. In the final broad run the same case took 247 ms, below the one-second review threshold. Its additional assertions protect automation approval persistence without adding another EF model initialization.

The single `EmploymentRestartNative_IndependentProcessesPreserveApprovalsAndNeverReplayTransfers` case is classified `Integration` and excluded from the default gate. Its two independent test-host processes use real MySQL migrations, durable approvals and native bank-account transactions. Provider startup, legacy upgrade and process restart are essential to that compatibility requirement and cannot be established by fast mocks. Keep those related assertions consolidated in the existing disposable-world harness; do not add a full-world startup or a separate provider fixture per assertion. See [the reproducible provider scenario](../../scripts/EmploymentRestartSmoke/README.md) for execution and evidence. The final writer/reader cases passed against real MySQL in 8.772 and 6.325 seconds respectively. Those costs are justified only in the opt-in tier; they must not enter the default unit gate. The full default gate passed 11,248 cases with three expected Windows skips; the 14 focused snapshot checks passed separately. Local receipts are in `.artifacts/employment-restart/fast-final/20261117T041429Z-81e17d8c2bb8/`, `snapshot-final/20261117T042654Z-2ad0a20d1011/`, and the owned native attempt `employment-attempt-a32792ee6f72`.

The live Telnet follow-up adds eight permanent fast cases: five task-bound diagnostic blocker previews, two public retry/requeue command cases covering authority/reason/hold requirements, and one manual-assignment case for a reconciled payment followed by nonfinancial work. They protect the actual diagnostic and recovery bypasses found during qualification and focused review. Keep them because they exercise distinct public entry paths that the policy-only cases do not establish. Extend the existing two-outcome reconciliation case to invoke the command service, catching the documented `not-executed` spelling, and the existing JSON round-trip to check the durable explicit-retry flag; do not add duplicate parser or EF fixtures.

In the final affected core/library run, all 10,240 tests passed with no failures/skips and stable source. All 23 approval cases took 250 ms combined (172 ms maximum); the eight new cases took 186 ms combined. None exceeded one second. The existing persistence round-trip took 402 ms. These measurements support retaining the focused cases in the default suite. The live server/crash/restart operator is separate opt-in acceptance, consolidated around one preserved disposable world; it must not become a default unit test or an additional full-provider fixture per assertion. The dated [recovery guide](../Economy/Employment_Restart_and_Recovery.md#live-telnet-follow-up-17-november-2026) records its scope, observed failures/corrections and receipts.

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

An existing class that shares a live EF context must use it read-only and execute serially. Mutation and idempotence cases need an isolated store. Expensive climate simulations remain in the dedicated opt-in project.

### Sharing expensive test setup

Start with the smallest fixture that exercises the contract. Several cheap examples can use the same builder while still constructing fresh state. When many cases pay to install the same prerequisites, use the narrowest fixture scope that covers those consumers. Key variants by the choices that affect persisted data or behaviour. Lazy creation keeps a filtered run from paying for unused variants.

Read-only inputs can be reused directly. For tests that write, a persisted baseline can be captured once and restored into a uniquely owned store per case. A fresh EF context over a writable shared database still sees other tests' writes. Preserve provider options, generated identities, shadow keys, join rows and any tracked-versus-reloaded behaviour the case exercises. Dispose the owner after its borrowers and release the temporary store. Keep clocks, random streams, mock callbacks, event subscribers and temporary directories local when their state can change. Tests that override process-wide registries must restore their changes even on failure.

Use case timings to find candidates, then measure the prerequisite separately. Compare fresh setup with building the baseline, making every copy and releasing it, including the first consumer. Check both execution time and resource cost, such as managed allocations or peak process memory. Run the same assertions and DataRows separately and together, including a pair where one case edits data the other expects to be pristine. The [November fixture review](../../scripts/TestReporting/shared-fixture-review-2026-11-17.md) gives measured examples and an implementation sequence.

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

Normal CI is defined in `.github/workflows/unit-tests.yml` and runs the canonical default gate using the checked-out SDK/toolchain for Windows and Linux in parallel matrix jobs. `.github/workflows/climate-tests.yml` keeps the unchanged multi-year climate suite separate on a weekly and manual schedule. Neither workflow collects coverage or runs mutation testing.
## Remaining Risks and Priorities

The suite is broad but not demonstrably complete because this branch intentionally does not add line/branch coverage or mutation tooling. Raw coverage percentage should not become a target by itself.

The next highest-value work is:

1. Audit the remaining legacy source-reading files outside the named priority set, beginning with runtime tests whose requirements can be reached without external services; handle seeder catalogue checks as a separate content-test audit.
2. Add behavior around important pure helpers, parsers, and persistence boundaries when uncovered code is encountered rather than adding snapshots of private implementation text.
3. Add only targeted EF mapping invariants backed by an explicit database compatibility requirement.
4. Keep the climate simulation separate and benchmark-driven; do not weaken its year ranges merely to improve the default-gate time.

This document should be updated whenever a test project moves into or out of the default gate, the test taxonomy changes, or a later measured audit materially changes the priorities above.
