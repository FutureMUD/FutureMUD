# Build and deployment harness review — 2026-10-22

Reviewed baseline: `d1c38215ed8957f537e89554910f17e7e9a53760`. Scope: all six Actions workflows, test entry points and reporter, product packaging/deployment dependencies, and the expensive Seeder fixture lifecycle. No product versions, tags, production state, repository rules or test assertions are changed by this task.

## Hosted evidence

The slowdown is observable, although these revisions have different test populations and are not a controlled benchmark:

| Successful PR run | Linux job | Windows job |
| --- | ---: | ---: |
| [38182713325, 11 October](https://github.com/FutureMUD/FutureMUD/actions/runs/38182713325) | 20m34s | 22m19s |
| [39731580431, 22 October](https://github.com/FutureMUD/FutureMUD/actions/runs/39731580431) | 28m16s | 68m15s |

The latter run's structured receipts separate the dominant phases:

| Phase | Linux | Windows |
| --- | ---: | ---: |
| First shared dependency build, including restore | 424.5 s | 1,868.7 s |
| Seeder build, including restore | 45.7 s | 122.8 s |
| Seeder tests | 1,155.8 s | 1,941.2 s |
| Core tests | 217.2 s | 511.2 s |

Test hosts overlap; their durations cannot be summed into job wall time. The first build includes the persistence library and is not a NuGet-only measurement. The migration batching change in baseline commit `d1c38215e` postdates these completed samples; its improvement must not be attributed to this review or estimated from these earlier runs. See [its qualification](../../Design%20Documents/Verification/Migration_Batching_Release_Qualification_20261022.md).

The upstream baseline's [completed master run 39777228715](https://github.com/FutureMUD/FutureMUD/actions/runs/39777228715), observed during this review, passed in **28m57s Linux / 56m49s Windows**. Its first dependency build took 332.0 / 676.9 seconds, while Seeder tests took 1,220.5 / 2,402.2 seconds. This confirms that compilation improved before this patch, but Seeder execution remains a major and variable bottleneck. These are upstream results, not hosted validation of the changes below.

For one identical feature head, [push 39728206387](https://github.com/FutureMUD/FutureMUD/actions/runs/39728206387) consumed 3,840 Windows plus 1,608 Linux job-seconds, while [PR 39728253353](https://github.com/FutureMUD/FutureMUD/actions/runs/39728253353) consumed another 4,400 plus 2,903. The redundant push cost **90.8 runner-minutes**. The previous guard queries for an already-scheduled PR run: a push arriving before PR creation or scheduling naturally misses it. Release tags also started duplicate full Debug gates.

The active default-branch ruleset was read without modification; it prevents deletion and non-fast-forward updates and did not declare required status checks at review time. Existing `default-gate (<OS>)` job names are nevertheless preserved.

## Changes and coverage

| Surface | Decision and reason |
| --- | --- |
| Unit Tests | Run all PRs, `master` pushes, merge groups and manual dispatches. Feature pushes and tags no longer schedule this matrix. This removes the API timing race instead of making a skipped push look like a passing test. Branches without a PR use manual dispatch; conflicted PRs must resolve their conflicts or use manual branch validation. No path filters weaken PR coverage. |
| Required job identity and evidence | Keep both OS jobs, the full ten-project manifest, Debug configuration, timeouts and structured artifacts. Add a readable Actions summary of per-project build/test times, counts and incomplete results. |
| Product release | Keep immutable tag/source validation, all runtime packages, targeted Release tests on Windows and Linux, documentation export, smoke publishing and production gates. Omit the identical x64 test/audit repetition in the x64 runner cross-publishing `linux-arm64`; this job never supplied ARM-native test evidence. Remove preliminary restores already performed by the test/publish commands and packaging scripts. Serialize retries of the same tag without cancelling active publication. |
| Website deployment | Package the managed Release output just tested without a second RID-specific compilation. Enforce NuGet audit errors in the deployment's own prerequisite job; the independent security workflow can otherwise finish after deployment. Include the linked logo in deployment path coverage. |
| Website security | Keep Release tests, NuGet audit, package boundaries, dependency review and CodeQL. Remove the standalone website build immediately repeated by the test-project build. The small independent security test pass is retained: its audit and scheduled/manual coverage are distinct from the general Debug gate and deployment. |
| Climate regression | Keep weekly/manual execution and its existing test command; restore only the climate test project graph, avoiding unrelated solution projects and VSIX prerequisites. |
| Historical backfill | Reviewed but unchanged. Manual-only immutable historical allowlist, SDK compatibility and independent promotion checks are intentional. |
| Reporter | Preserve source fingerprints, process-tree ownership, deadlines, bootstrap isolation, failure precedence and per-project build attribution. Minor repeated Git calls/bootstrap overhead are not the measured bottleneck. Do not trade those evidence guarantees for speculative caching. |
| Local build smoke scripts | The PowerShell entry point previously returned success after a failed native build. Preserve the build exit code explicitly and use single-node builds in both PowerShell and Bash. |

GitHub's documented [event/branch filters](https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-syntax) supply the trigger separation. Package tests are still real checks at the release source revision; no previous successful SHA, downloaded binary cache or untrusted PR artifact substitutes for them.

## Seeder fixture ownership

The settings and prepared-world fixtures create uniquely named databases under static `InMemoryDatabaseRoot` instances. Snapshot clones use EF's shared default store cache. Context disposal alone does not clear those stores: EF 9.0.11's [store cache](https://github.com/dotnet/efcore/blob/v9.0.11/src/EFCore.InMemory/Storage/Internal/InMemoryStoreCache.cs) retains each named store. Creating a separate root per fixture would also create separate cached service providers because the [options cache key includes the root](https://github.com/dotnet/efcore/blob/v9.0.11/src/EFCore.InMemory/Infrastructure/Internal/InMemoryOptionsExtension.cs).

The test-only `OwnedInMemorySeederContext` retains the existing provider/root configuration and clears its own database on synchronous or asynchronous disposal, before disposing the context. Factory-created contexts remain ordinary borrowers: disposing them does not clear data, and tests still exercise fresh-context reads, restart/reconstruction, reruns, defaults and real installer paths. The owner must outlive its borrowers. The helper rejects relational providers. A different context CLR type can add one cached EF model per provider configuration; it does not introduce one provider per fixture.

Only the settings fixture, prepared-world fixture and snapshot clone factory use the new owner. Existing snapshot isolation/fidelity assertions and uncached integration matrices remain intact. New regressions cover child-context persistence, sibling isolation, both disposal paths, mixed repeated disposal and rejection of relational options without connecting to a server.

## Validation and limits

The paired local Seeder runs used the same Debug project scope, 1,800-second timeout and skip policy. They are single observations, not a statistical benchmark:

| Measurement | Before | After |
| --- | ---: | ---: |
| Passed / failed / skipped | 2,096 / 0 / 3 | 2,099 / 0 / 3 |
| Build phase | 13.86 s | 9.36 s |
| Test phase | 860.95 s | 873.31 s |
| Total reporter run | 876.77 s | 884.64 s |
| Observed testhost peak working set | 4.22 GiB | 3.03 GiB |

Every one of the 2,099 original case identities and outcomes was preserved; all three new lifecycle cases passed. The three existing skips exercise Unix-only launcher permissions on Windows. All case durations were present. The after run (`20261022T125910Z-b02b22f63fd7`) is **PASS**, with complete counts, identical start/end source fingerprints and an unchanged test assembly hash during execution. The existing lazy-proxy factory/rerun and snapshot cases are included in this full suite.

The before run (`20261022T124011Z-112f96969964`) has successful native test results but its overall receipt is **INCONCLUSIVE / SOURCE_CHANGED**: workflow, script and documentation edits occurred during execution. Seeder and dependency source stayed fixed, and the test assembly hash was unchanged during that run. This distinction is retained rather than relabelling the receipt as a passing gate. Memory sampling was sparse before (one captured lifetime-peak observation) versus fourteen one-minute samples after; the after maximum was sampled 41 seconds before completion. The observed reduction is approximately 28%, not a guaranteed final-peak measurement. Test time increased by 12.37 seconds (1.4%); no local speedup is claimed. Hosted contention and memory effects require a new CI run to measure.

Additional completed checks:

- Six focused fixture lifecycle/snapshot cases passed before the full run; no assertions, case matrix, timeout or skip rules were relaxed.
- Website deployment's exact audited restore passed outside the sandbox. The sandbox attempt correctly failed with NU1900 because access to vulnerability data was denied; audit enforcement was retained for the successful retry.
- All 54 website Release tests passed with no skips. The new no-build/no-restore publish succeeded, and its website DLL SHA256 matched the tested Release DLL. The published package started locally, returned readiness `ready`, and served both the home page and linked logo with HTTP 200. The owned process was stopped after the probe.
- Actionlint 1.7.12 accepted all six workflows. External ShellCheck/Pyflakes integration was disabled because those tools were unavailable. Changed PowerShell syntax and summary generation were checked with real receipts, missing receipts and synthetic incomplete/failing results. Git Bash syntax checks passed for the smoke, unit, climate and reporter entry points; no native Linux execution is implied.
- A controlled fake build reproduced the original PowerShell success-on-failure defect. Direct `pwsh -File scripts/test.ps1` checks now preserve both failure code 37 and success code 0. `git diff --check` passed.
- Independent focused review found no actionable defects in the workflow changes or owned-fixture implementation.

Raw hosted metadata, downloaded receipts, TRX results and smoke logs are retained locally under `.artifacts/ci-review/`; the earlier 17 October report remains historical evidence. The baseline and after summaries are under `seeder-baseline/20261022T124011Z-112f96969964/` and `seeder-after/20261022T125910Z-b02b22f63fd7/`. Native Linux execution is unavailable on this Windows host (WSL is not installed), so Bash syntax checks and Windows publishing cannot establish hosted Linux acceptance. No workflow has been dispatched, published or merged by this task; production deployment, release packaging across all runtimes and hosted timings for these changes remain unverified.

The next hosted comparison should use the unchanged two-OS full gate and its new timing summary: verify one PR-event run per feature update, preserved counts, first-build time and Seeder test time. The demonstrated saving is removal of redundant runner work (90.8 minutes in the sampled duplicate), not a promise that an individual remaining gate will halve in duration. Avoid additional sharding, per-test parallelism or binary caches until this result is measured: each would change contention, rebuild costs or isolation and needs its own qualification.
