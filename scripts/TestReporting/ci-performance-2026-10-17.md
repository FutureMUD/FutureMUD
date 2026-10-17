# CI performance investigation — 2026-10-17

Baseline source: master `26e1d14e982b0260e5fa8a015cda2008d2ca50ce`. This batch preserves the default project manifest, Debug configuration, Windows/Linux matrix, test assertions, and existing timeouts. Climate remains separate. It does not change production seeding, relational-default regressions, runner plans, token permissions or branch protections.

## Hosted baseline and ranking

[Master run 38886017979](https://github.com/FutureMUD/FutureMUD/actions/runs/38886017979) passed on both operating systems:

| Phase | Linux | Windows |
| --- | ---: | ---: |
| Run creation to job start | 3 s | 4 s |
| Checkout | 16 s | 30 s |
| SDK setup | 12 s | 29 s |
| Default script wall time | 44m19s | 61m52s |
| First shared-library restoring build | 11m47.75s | 13m03.87s |
| Seeder restoring build | 1m05.37s | 1m32.44s |
| Seeder test duration (native console) | 30m22s | 45m30s |
| Core test duration (native console) | 4m30s | 7m54s |
| Artifact upload | absent | absent |

1. Seeder execution dominates the critical path. Both jobs discovered 1,935 cases; Windows passed 1,932 and intentionally skipped three, while Linux passed all 1,935. The snapshot change must not alter those existing platform decisions.
2. Compilation/restore is next. The script builds targets serially with `-m:1`; the first build includes shared dependencies. These are combined build/restore measurements, not restore-only durations. Individual package restore messages are concurrent/overlapping and cannot be summed into a separate phase. The longest reported Windows initial package restore was about 25 seconds, much smaller than the first build, so a NuGet cache is not presented as a measured solution to that build bottleneck.
3. Duplicate feature events waste substantial runner time. At the same head `4bd571a583c6313e58cde0e7888c204b300c37a9`, [push 38886317797](https://github.com/FutureMUD/FutureMUD/actions/runs/38886317797) used 3,068 Linux plus 4,054 Windows job-seconds; [PR 38886325333](https://github.com/FutureMUD/FutureMUD/actions/runs/38886325333) used 3,136 plus 2,376. Total: 210.6 runner-minutes, including 118.7 minutes in the redundant push pair. This is avoidable historical work, not a claim of measured post-change wall-time savings. PR runs use GitHub's merge checkout while push runs use the branch checkout.
4. Queue/setup were small in this sample. No artifact-upload baseline exists because receipts were discarded. The new structured artifact makes phase and case timings available for subsequent investigations without retaining native logs.

[Earlier master run 38875077857](https://github.com/FutureMUD/FutureMUD/actions/runs/38875077857) used source `90a9343ace517b926e11209a3f5347a586a88646` and only 1,630 seeder cases (Linux 9m34s). It is **not** a comparable before/after sample for this change.

## Coverage-preserving duplicate suppression

Each existing OS job performs a bounded, read-only lookup before setup. Only a non-default branch push with an open same-repository PR at the exact head SHA and a corresponding Unit Tests PR run can skip its expensive steps. Cancelled/blocked/missing runs, conflicts that prevent PR scheduling, unavailable API access, malformed responses, and scheduling races retain push coverage. Master, tag, PR, dispatch and branches without a PR continue normally. The probe has a one-minute ceiling and failure is treated as a reason to run tests. No required job names change.

The PowerShell scenario check exercises the actual workflow body. `actionlint` validates the workflow structure. The probe intentionally prefers an occasional duplicate over missing coverage.

A queued/running PR is still contingent on eventual completion: a later manual cancellation can leave that revision without completed test evidence. Check the surviving PR run before accepting a revision; the skipped push is not itself a passing test result.

## Durable evidence

`test-results.json` now includes nullable `duration_seconds` from native TRX. Valid zero and subsecond values remain precise; missing/invalid/negative durations remain unknown without weakening outcome/counter validation. The full reporter regression suite covers parsing, source evidence, timeouts, process ownership and failure precedence.

An `always()` upload retains run/summary/results/failures JSON and summary text for 14 days, separately by OS. Existing native logs are not uploaded as artifacts. A killed runner may not write a final receipt, and missing evidence is never interpreted as success. Build phases still include restore; test hosts still execute concurrently.

## Local measurements and validation

Local SDK: 10.0.401, Windows, Debug, serial targeted builds. Initial clean seeder-target build: 50.17 seconds, zero errors and 203 existing warnings. Other native tests and an S6 core test were active during initial inspection; no other worktree, process, port or database was modified. Local timing comparisons must therefore carry a contention/host caveat and must not be substituted for hosted Linux/Windows measurements.

The full local baseline passed **1,932 cases with three existing Windows skips**, 1,935 total, in **15.1358 minutes**. The skipped DataRows are the Unix-only owner-permission launcher test. Case-level TRX ranking:

| Class | Cases | Summed native seconds |
| --- | ---: | ---: |
| ArmageddonStockSpellInstallerTests | 83 | 563.425 |
| ArmageddonStockSupportInstallerTests | 31 | 51.779 |
| ArmageddonPreparedWorldSeederTests | 98 | 32.964 |
| ArmageddonCatalogueInstallerTests | 6 | 32.308 |
| PreIndustrialFoodCatalogueCraftSeedingTests | 3 | 31.276 |

Only the first class's repeated prerequisites are optimized. Its real progression/catalogue setup creates immutable, process-local snapshots by progression/class configuration. Every narrower spell case receives a unique InMemory database, fresh entities and copied scalar values; the spell installer and reruns still execute. Both four-row full integration matrices deliberately retain fresh real prerequisite setup. No assertion or DataRow is removed. The unmerged Peek/Direction/Guarding PRs are not imported or edited; no native/relational check is replaced with this helper.

Snapshot capture uses tracked reads for shadow values and named shared sets for dictionary join rows; restore copies all mapped scalar/FK values using EF value comparers. Keyless read-only views cannot contain persisted fixture rows. Unsupported owned/inheritance mapping shapes fail explicitly. The helper accepts only the InMemory provider and is not a substitute for database-default or relational regressions.

The first isolation attempt failed two cases on the real model's shared join mapping. Review also identified the risk of losing shadow values with detached reads. The corrected helper passed **3/3** isolation cases, including a populated shared join, a nondefault shadow FK, complete persisted-scalar fingerprints, navigation hydration, generated-ID advancement and mutation/deletion/addition isolation. This failure is retained in local evidence rather than counted as a pass. Independent re-review confirmed both corrections.

Other validation: reporter suite **40/40 passed**, workflow coverage scenarios **16/16 passed**, and `actionlint` 1.7.12 passed. The explicitly requested agent correction is included as a separate cherry-pick: reviewer and QA now use `gpt-6.1-sol`; their reasoning levels and service tiers are unchanged, as are Luna/Astra roles.

## Comparable full-suite result

The after suite ran at `d382c39e6f93433aaf3b1b9431fb12c59ca6568c`, with a clean worktree and identical test-assembly SHA256 before and after execution. Both runs used the same Windows machine (Ryzen 9 9950X3D, 16 cores/32 logical processors, about 61.6 GiB physical RAM), SDK/configuration and command shape:

```powershell
dotnet test 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' --no-build --no-restore -m:1 --logger 'console;verbosity=detailed' --logger 'trx;LogFileName=baseline.trx' --results-directory ..\seeder-baseline
```

The after command changed only the output filename/directory to `after.trx` and `seeder-after`. Native output was redirected to sibling `.log` files. This comparison excludes builds/restores and is one full run per revision, not a statistical benchmark.

| Measurement | Before | After |
| --- | ---: | ---: |
| TRX run start-to-finish | 908.158 s | 672.077 s |
| Passed / skipped / total | 1,932 / 3 / 1,935 | 1,935 / 3 / 1,938 |
| StockSpell summed native durations | 563.425 s (83 cases) | 339.152 s (86 cases) |
| Eight uncached full-integration cases | 134.197 s | 130.024 s |
| Unchanged StockSupport control class | 51.779 s | 51.712 s |

The observed suite span fell **236.081 seconds (26.0%)**; the optimized class fell **39.8%**, including its three added isolation cases. All 1,935 original class/test-name identities remain, with exactly the same outcomes. The three skips remain `EnsureStartScript_UnixGeneratedLauncher_CreatesAndRepairsOwnerOnlyPermissions (0)`, `(1)` and `(2)`. No old test is missing or newly skipped.

TRX spans are the primary comparable timings. Console totals were 15.1358 and 11.2011 minutes. Log creation-to-final-write spans were approximately 908.745 and 672.666 seconds; these are file timestamp proxies, **not independently measured command stopwatch wall times**. Summed case durations are a separate measurement and should not be equated with elapsed time when tests run concurrently. Other native workloads were present, free-memory samples were roughly 40 GiB, and the uncached controls drifted about 3.1%; do not extrapolate the full reduction to hosted hardware without its completed receipts.

The first hosted duplicate check was observed working: [push 39031931616](https://github.com/FutureMUD/FutureMUD/actions/runs/39031931616) completed its Linux/Windows jobs in 9/13 seconds after identifying [matching PR run 39031934806](https://github.com/FutureMUD/FutureMUD/actions/runs/39031934806). That PR's guard scenarios passed on both OSes and its full gates were still running when this report was written. This validates duplicate routing, not hosted test success or artifact upload completion. Subsequent documentation commits trigger fresh hosted checks.

Raw local evidence remains in `C:\Users\luker\Documents\Codex\2026-10-17\task-4`: `seeder-baseline/baseline.trx`, `seeder-after/after.trx`, their sibling logs, `seeder-comparison.json`, `compare-seeder.py`, `after-head.txt`, `after-status-before.txt`, `after-status-end.txt`, `after-binary-before.json`, `after-binary-end.json`, `hardware.json`, `resource-baseline.json`, `resource-after.json`, `reporter-validation/reporter.trx`, `snapshot-isolation/isolation.trx` (initial failure), `snapshot-isolation-fixed/isolation.trx`, `baseline-jobs.json`, `baseline-master.log`, `baseline-pr-jobs.json`, `baseline-push-jobs.json`, `dedup-after-jobs.json` and `dedup-after.log`. Raw logs are intentionally not committed.
