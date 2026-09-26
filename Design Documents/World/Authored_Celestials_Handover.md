# Authored celestials implementation handover

Completed and verified on 26 September 2026 against base `666efc94a2065fb94ef75a1ffe6dd42359940c6c` with the subsequent LONGTEXT migration and full fast-gate verification recorded below. `RailSun`, `RailMoon`, `ScriptedSun` and `ScriptedMoon` now load, persist, support in-game authoring, contribute light, describe movement/phase, deliver scheduled echoes and answer event queries. The feature is prepared for publication on `codex/authored-celestials`; deployment is outside this task.

**No authored time-of-day track was added. Existing first-eligible zone authority and elevation/direction thresholds are preserved.** Existing physical ephemerides and numerical golden values are unchanged.

## Delivery and ownership

| Files/area | Responsibility |
| --- | --- |
| `FutureMUDLibrary/Celestial/AuthoredCelestialContracts.cs`, `CelestialEphemeris.cs` | Apparent state, time/phase capabilities, structured query status and separate object-based event route. |
| `MudSharpCore/Celestial/Authored/AuthoredCelestialDefinition.cs`, `AuthoredMath.cs` | Versioned source, validation/limits, exact signed time/rank arithmetic, phase and compatibility helpers. |
| `CompiledSkyPath.cs`, `CompiledScalarTrack.cs`, `CompiledAuthoredCelestial.cs` in the same folder | Rail/circle/sparse/dense evaluation, independent light/phase/year tracks, bounded event compilation and direct nth lookup. |
| `AuthoredCelestial.cs`, `AuthoredCelestialPresets.cs` | Four concrete persisted types, one live evaluation per minute/version, private zone wrappers, lifecycle/echo delivery and six presets. |
| `AstronomicalEventService.cs`, `NewSun.cs`, `PlanetaryMoon.cs`, FutureProg celestial functions | Capability dispatch preserving the physical path; all existing overloads, new named-event function and shared lunar phase. |
| `TimeModule.AuthoredCelestials.cs`, `TimeModule.cs`, `RoomBuilderModule.cs`, `PerceptionModule.cs`, `CharacterTarget.cs` | Private builder drafts, compile/preview/save/clone/import/export, authority warning, TIME, LOOK SKY and ordinary celestial lookup. |
| `Zone.cs`, `Futuremud.cs`, `FuturemudLoaders.cs` | Attach/detach/disposal, per-zone output opportunity, registration and loader diagnostics. |
| Library `IClock.cs`/`Time.cs`, runtime `Clock.cs`/`Calendar.cs`, `ImplementorModule.cs` | Silent administrative resynchronisation, compatible calendars and nonrecursive sunrise/sunset calendar boundaries. |
| `DatabaseSeeder/Seeders/CelestialSeeder/*`, replay/question/metadata registries | Optional repeat-safe authored package; explicit answers in all five Debug profiles; production compiler validation before seeding. |
| Six `AuthoredCelestial*Tests.cs` core files, fixture JSON, `AuthoredCelestialSeederTests.cs` | Numerical, randomized, failure, integration, event-function, persistence-ownership and performance regressions. |
| `scripts/AuthoredCelestialSmokeWorld/` | Disposable native replay harness, executable Telnet assertions, process cleanup checks and runbook. |
| `MudsharpDatabaseLibrary` mapping, generated migration/designer/model snapshot, blank SQL/manifest and persistence tests | Data-preserving `TEXT` to `LONGTEXT` upgrade, 16 MiB default budget, and snapshot parity. |
| Owning World/Seeding design documents; `.gitignore` | Public contracts, builder guidance, seeder workflow and ignored local evidence. |

The committed [verification summary](Authored_Celestials_Verification.json) retains run identities, counts, fingerprints and limitations for PR reviewers. Detailed raw logs, the SHA-256 changed-file manifest and the handover ZIP live in `.artifacts/authored-celestials` in the implementation worktree; links to that ignored directory below are local evidence references and are not published GitHub files. The bundle includes the referenced test receipts and redacted native transcript.

## Persisted format and commands

The existing `Celestials` type/feed-clock/`Definition` envelope stores version 1 JSON source. Generated migration `20260926115932_WidenCelestialDefinition` widens `Definition` to `LONGTEXT`, preserving existing row values and encoding. The maintained blank snapshot was regenerated and imported successfully. Angles in source/commands are degrees; engine interfaces use radians. Periods are integer feed-clock minutes, anchors are minute-aligned feed-clock seconds, and `d` converts days using the actual clock dimensions. Path/light/phase/year/event periods remain independent. Sparse data explicitly closes at its period; dense data supplies samples `0..P-1`. Compiled arrays and live cursors are not persisted.

HighAdmin uses `celestial new <preset> <clock> <calendar> <name>`, `edit`, `set`, `validate`, `preview`, `event`, `save`, `clone`, `export`, `import` and `close`. `set` supports simple/three-point rails, sparse/dense keys, lux profiles/timelines, regular/fixed/authored phase, synthetic longitude, named milestones, crescent associations and both echo audiences. JSON import uses the normal editor. Invalid edits preserve the active object. Save prepares candidate live state before writing the row.

Attach through `shard set <shard> celestials <ids...>`; this replaces the entire attached list, so retain all wanted IDs in the intended order. The six optional presets remain unattached after seeding. `SeederPreset` markers allow missing members to be restored while preserving builder modifications and IDs.

All location/zone and celestial/ID variants of `nextsunrise`, `nextsunset`, `nextnewmoon`, `nextfullmoon`, `nextsolarlongitude` and `nextvisiblecrescent` dispatch through capability. `nextcelestialevent(..., "custom:key" [, occurrence])` queries named milestones. Invalid or unavailable event queries retain FutureProg `Never`; admin previews distinguish the reason. Synthetic annual longitude and associated authored crescent markers are explicitly conditional capabilities. See the [complete feature guide](Authored_Celestials.md) and [native runbook](../../scripts/AuthoredCelestialSmokeWorld/README.md).

## Verification results

| Check | Passed/executed | Skipped | Evidence |
| --- | ---: | ---: | --- |
| Core physical/authored/calendar/clock regression filter | 167/167 | 0 | [`20260926T105845Z-6db5108eebc6`](../../.artifacts/test-runs/20260926T105845Z-6db5108eebc6/summary.txt) |
| Focused SeederReplayTests (included in full suite) | 9/9 | 0 | [`20260926T105956Z-a876346257e3`](../../.artifacts/test-runs/20260926T105956Z-a876346257e3/summary.txt) |
| Full DatabaseSeeder suite | 1,381/1,381 | 0 | [`20260926T110043Z-1807195e5a5e`](../../.artifacts/test-runs/20260926T110043Z-1807195e5a5e/summary.txt) |
| Shared time/date filter | 23/23 | 0 | [`20260926T110355Z-8b9891b179f7`](../../.artifacts/test-runs/20260926T110355Z-8b9891b179f7/summary.txt) |
| Final authored rerun after nullable cleanup | 66/66 | 0 | [`20260926T110741Z-191c0527c467`](../../.artifacts/test-runs/20260926T110741Z-191c0527c467/summary.txt) |
| Harness process-ownership regressions | 3/3 | 0 | [log](../../.artifacts/authored-celestials/cleanup-tests.log) |
| Original feature DatabaseSeeder Release build | 0 warnings, 0 errors | — | [log](../../.artifacts/authored-celestials/seeder-release-final.log) |
| Native Telnet final scenario | 141/141 assertions | 0 | [receipt](../../.artifacts/authored-celestials/smoke-latest.json) |

Debug builds are recorded in the wrapper receipts. The earlier focused and seeder passes established the original feature behavior; the complete fast gate below additionally covers the final schema, source-budget and snapshot changes. No numerical golden values or assertions were weakened. Build warnings, if present, remain visible in the logs.

The unchanged attachment hash is `f49e8c13bd27c5ddd38e7cf2c26d2cea6b09168bfdfb9271c6254ca59838e0ce`. All supplied fixtures executed against production evaluators:

| Fixture | Supplied case | Result |
| --- | --- | --- |
| R1 | Overhead rail | PASS |
| R2 | South-tilted rail | PASS |
| R3 | Non-opposite bearings, true small circle | PASS |
| S1 | Visible Morning Star, elevation-only dawn | PASS |
| S2 | Below-horizon Morning Star, glow only | PASS |
| S3 | Rapid jumps and multiple events | PASS |
| H1 | Crossing with horizon plateaux | PASS |
| H2 | Horizon touch and retreat | PASS |
| H3 | Permanent horizon | PASS |
| M1 | Daily rail moon, independent 28-day phase | PASS |
| M2 | Authored phase with full and new holds | PASS |
| M3 | Phase jumps skip intermediates but landing is an event | PASS |
| C1 | 20 x 80 x 100 clock | PASS |
| A1 | Independent synthetic annual angle | PASS |

Additional coverage includes independent vector identities for randomized simple/three-point rails, sampled crossing oracles and zero-run rotations, shortest spherical routes, long sparse periods, relatively prime channels, negative/nonstandard-clock epochs, compatible and incompatible calendar contexts, large nth/overflow errors, every registered event-function overload, invalid IDs/counts, first-authority/light/timezone behavior, output visibility contracts, subscription lifetime, atomic edits and independent wrappers. Performance counters require exactly one intrinsic evaluation per normal minute, independent of 1/100/1,000 subscribers.

Reproduction commands (PowerShell, repository root; wrapper builds serially):

```powershell
$env:GIT_CONFIG_COUNT = '1'
$env:GIT_CONFIG_KEY_0 = 'core.safecrlf'
$env:GIT_CONFIG_VALUE_0 = 'false'
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj' -Filter 'FullyQualifiedName~Celestial|FullyQualifiedName~PlanetaryMoon|FullyQualifiedName~PlanetFromMoon|FullyQualifiedName~SunFromPlanetaryMoon|FullyQualifiedName~MudDate|FullyQualifiedName~Calendar|FullyQualifiedName~FutureProgDateTime|FullyQualifiedName~Clock' -TimeoutSeconds 1800
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -Filter 'FullyQualifiedName~SeederReplayTests' -TimeoutSeconds 600
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -TimeoutSeconds 1800
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'FutureMUDLibrary Unit Tests/FutureMUDLibrary Unit Tests.csproj' -Filter 'FullyQualifiedName~MudTimeFactory|FullyQualifiedName~DateUtilities|FullyQualifiedName~ItemTimeRate' -TimeoutSeconds 600
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj' -Filter 'FullyQualifiedName~AuthoredCelestial' -TimeoutSeconds 600
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Release --no-restore -m:1 -p:NuGetAudit=false
python -B -m unittest discover -s scripts/AuthoredCelestialSmokeWorld -p test_cleanup.py -v
python -B scripts/AuthoredCelestialSmokeWorld/native.py
```

The process-local Git setting avoids a reporter pipe deadlock on a large volume of line-ending warnings; it does not change repository Git configuration or source. Wrappers use their documented local NuGet audit/warning settings; this is not a dependency-security audit. Elevated execution was needed for Windows output ACLs.

## Native persistence and gameplay evidence

The complete `medieval-standard` Debug replay executed all 30 enabled profile seeders in a newly initialized, loopback-only MySQL instance. A second replay was refused with zero seeders executed and matching `CHECKSUM TABLE EXTENDED` digest across every table:

`98EB3B62094397FE995F9D81971BF55D5CB8D18E308009B7A38FFAAFB8B93045`

The [replay receipt](../../.artifacts/authored-celestials/native-replay.json) records every completed seeder. No existing MySQL service or game database was used.

The final [redacted transcript](../../.artifacts/authored-celestials/run-9a3180ec1fa0/smoke-eff91c3a/transcript.txt) and [assertion receipt](../../.artifacts/authored-celestials/smoke-latest.json) prove in-game creation/save of all four types; selected R1/M1 previews; millionth-event queries; optional longitude/crescent and all seven public event functions; phase/elevation values; outdoor sky/time/light output; import/export, clone and rejected save; one normal-minute SkyVisible echo; below-horizon BodyVisible suppression; window prefix and shelter suppression; silent queries/admin shifts; another single echo after rewind; and persisted definitions/attachments/event indexes after graceful shutdown and restart. Both MUD runs exited cleanly. The verified disposable MySQL instance is stopped and its data is preserved, as recorded in [cleanup.json](../../.artifacts/authored-celestials/cleanup.json).

## Performance observations

Measured on this Windows .NET 10.0.401 Debug test run, with 100,000 calls per numeric operation; these are observations, not fragile timing assertions. [Raw measurements](../../.artifacts/authored-celestials/performance.json) include ranks 1, 1,000 and 1,000,000. Across cases those event queries averaged 256–311 ns/call, independent of rank in operation count, with zero measured allocated bytes.

| Definition | Stored entries | Compile ms | Evaluate ns/call | Bytes allocated over 100,000 evaluations |
| --- | ---: | ---: | ---: | ---: |
| small rail | 6 | 0.139 | 303.2 | 0 |
| long sparse | 25 | 0.608 | 299.8 | 0 |
| dense 40000 | 40,004 | 152.199 | 157.3 | 0 |

Each fan-out measurement includes 60 clock updates and per-subscriber mutable wrappers:

| Subscribers | Total ms | Allocated bytes | Intrinsic evaluations |
| --- | ---: | ---: | ---: |
| 1 | 0.116 | 83,520 | 60 |
| 100 | 0.338 | 368,640 | 60 |
| 1,000 | 2.582 | 2,960,640 | 60 |

Fan-out allocations include clock/calendar work and wrappers, and therefore grow with recipients; intrinsic numerical evaluation remains once per minute. Sparse definitions compile proportionally to source segments and bounded roots. Dense compilation is linear in supplied samples. Recurrence uses sorted offsets and rank arithmetic, with no orbital scan or combined-period timetable.

## Limits and evidence boundaries

Production load/edit/seeding now use the 16 MiB default source budget backed by `LONGTEXT`. Explicit configured limits are preserved. The 40,000-sample case is both a useful scaling benchmark and a plausible roughly 28-day dense path; it has now been persisted and reloaded from MySQL. Regular motion is usually more compact as a rail or sparse path. Entry and preview guards do not cap sparse period length or direct recurrence duration. Rolling back to `TEXT` requires definitions to fit the old 65,535-byte limit.

Preset lux values are illustrative. Longitude is synthetic; authored crescents do not claim geometric visibility. Unsupported physical conjunctions, mixed pairs without authored markers and invalid contexts return explicit unavailable results. Existing warnings and restrictions on structural clock/calendar edits continue to apply.

Native verification uses one disposable zone and the seeded administrative avatar. Blindness and sky-obscuring weather are tested through the runtime perception/output contract, not a live blinded-player scenario. The 1,000-recipient fan-out is instrumented runtime coverage. The provisioning wrapper is Windows/MySQL 8 specific; native POSIX execution, a full solution/VSIX build, dedicated slow climate tests and production deployment were not run. The complete fast manifest passed; hosted CI is reported separately on the PR. No required check remains blocked.

Earlier failed development checks remain recorded in verification history: azimuth wrap canonicalization, fixture collection setup and the null-calendar-collection regression were repaired; initial compile failures were superseded. One reporter invocation was **NOT_RUN** because its Git fingerprint collection stalled before build; it was stopped and rerun successfully. Initial native setup/assertion attempts, including the replay harness's missing lazy-loading option, were corrected and superseded by successful fresh replay and complete Telnet receipts. They are not counted as passes.

## LONGTEXT follow-up verification

MySQL `TEXT` was too small even for a day of minute-by-minute dense samples. The production serializer measured and the production EF path saved these sources:

| Samples | Conventional 1,440-minute days | UTF-8 bytes | Exact reload and evaluation after MySQL restart |
| ---: | ---: | ---: | --- |
| 1,440 | 1.000 | 205,062 | PASS |
| 40,000 | 27.778 | 5,730,585 | PASS |
| 40,320 | 28.000 | 5,776,680 | PASS |

The owned populated world contained 19 existing celestial rows, including physical XML and authored JSON. A backup was taken before the production migration service applied the generated in-place widening. All scalar fields and server-side SHA-256 hashes of stored definition bytes matched afterward. Source IDs, charset, collation and required-value constraints were preserved. The fixture's MySQL 8.0.45 instance was shut down after verification and its data was retained. No production or shared database was accessed. After current master gained the land-treatment migration, the unpublished celestial migration was regenerated against that combined model and this proof was repeated in a fresh instance from the preserved old-schema backup. Both pending migration IDs are recorded in the receipt.

The generated migration/designer/EF model snapshot match the current model (`has-pending-model-changes`: no changes). A full blank snapshot refresh ran in a separate disposable database; the production snapshot importer then restored it into another empty database, confirming `LONGTEXT`, the latest migration, zero pending migrations and zero celestial fixture rows. SQL SHA-256: `9ECE6704EC16CD4C9874E6F096C7951A4FC8CF87C93FC2ACAFC4FDDE44037757`. Manifest product version: `3.6.1.0`.

Evidence: [upgrade and dense roundtrip](../../.artifacts/authored-celestials/persistence-1e14b217648f/upgrade.json), [snapshot import](../../.artifacts/authored-celestials/persistence-1e14b217648f/snapshot-import.json), [cleanup](../../.artifacts/authored-celestials/persistence-1e14b217648f/cleanup.json). The existing 141-assertion full Telnet scenario preceded this storage follow-up; the follow-up separately verifies upgrade, dense persistence and restart. The focused review of this follow-up found no actionable defects.

Full fast gate: **5,917/5,917 passed, 0 skipped**, source stable, run `20260926T120752Z-3c22adc97175`. This includes the focused authored/model/snapshot regressions. Final Release seeder build passed after the migration and snapshot refresh with 3 existing analyzer release-tracking warnings and zero errors.

| Project | Passed/executed | Skipped |
| --- | ---: | ---: |
| `FutureMUDLibrary Unit Tests/FutureMUDLibrary Unit Tests.csproj` | 505/505 | 0 |
| `ExpressionEngine Unit Tests/ExpressionEngine Unit Tests.csproj` | 36/36 | 0 |
| `DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj` | 1,382/1,382 | 0 |
| `MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj` | 3,761/3,761 | 0 |
| `MudsharpDatabaseLibrary Unit Tests/MudsharpDatabaseLibrary Unit Tests.csproj` | 63/63 | 0 |
| `DiscordBotCore Unit Tests/DiscordBotCore Unit Tests.csproj` | 22/22 | 0 |
| `RPI Engine Worldfile Converter Tests/RPI Engine Worldfile Converter Tests.csproj` | 43/43 | 0 |
| `FutureMUD.Web.Tests/FutureMUD.Web.Tests.csproj` | 54/54 | 0 |
| `TerrainPlanner/TerrainPlanner.Tests/TerrainPlanner.Tests.csproj` | 34/34 | 0 |
| `scripts/TestReporting.Tests/TestReporting.Tests.csproj` | 17/17 | 0 |

Additional reproduction commands:

```powershell
.\scripts\test-unit.ps1 -OutputMode Compact -TimeoutSeconds 1800
dotnet ef migrations has-pending-model-changes --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj --no-build
python -B -u scripts/AuthoredCelestialSmokeWorld/persistence.py --run-dir .artifacts/authored-celestials/<stopped-old-schema-native-run>
```

Use matching EF tooling (9.0.11 for this checkout) and current Debug binaries; never run the snapshot refresher against a game database. The persistence wrapper supplies only its owned disposable databases. The same process-local Git workaround shown above applies to the fast gate. Initial harness return/import-path failures were corrected before the successful native run; these setup failures are retained in local logs, not counted as passes.

The initial full fast gate (`20260926T115024Z-7c1eef252245`) passed 5,851 cases and exposed the existing `TitleCase_CapitalisesWords` failure. The helper now retains internal straight/typographic apostrophes while still capitalising opening quoted words. Its inconsistent sentence-case expectation was corrected to word title casing, preserving the established helper contract. The focused regression and final full gate passed; the initial failure remains in verification history. This small prerequisite repair changes `StringExtensions.cs` and its owning tests.

Final bundle validation caught Git's Windows checkout converting the supplied fixture's LF bytes to CRLF. The parsed JSON values were identical. The original attachment bytes were restored and a `-text` Git attribute now preserves them. All 14 supplied fixtures were rerun successfully from the exact restored file ([TRX](../../.artifacts/authored-celestials/exact-fixture-recheck/fixtures.trx)). This byte-only fixture normalization and documentation/evidence updates followed the full fast gate; production behavior and numerical fixture values were unchanged.


## Hosted CI harness follow-up

The first hosted runs on `b62c114` failed in the shared test-reporting harness while the celestial, seeder and database suites passed. Linux allowed two owners of a `FileShare.Read` lock. Windows parallel process pipes could starve thread-pool continuations, and healthy fake-runner tests had only a three-second total budget. The retained initial logs are failures, not passing evidence: [Linux](../../.artifacts/authored-celestials/ci-linux-failed.log), [Windows](../../.artifacts/authored-celestials/ci-windows-failed.log), [PR run](../../.artifacts/authored-celestials/ci-pr-failed.log).

The harness now uses an exclusive cross-platform lock plus a readable owner marker and dedicated Windows pipe readers. It still reports incomplete drains as inconclusive. Healthy fixture budgets are 30 seconds; deliberate timeout coverage keeps its three-second limit. The new regression checks eight concurrent hosts, complete 200 KB stdout/stderr streams and all expected test records; the lock regression also verifies release and subsequent acquisition. [Harness contract](../../scripts/TestReporting/README.md).

Focused harness suite: **18/18 PASS**, run `20260926T123936Z-ac8deffdcba2`. Full local fast gate: **5,918/5,918 PASS**, zero skipped, stable source, run `20260926T124228Z-3e57de8bdd72`. This changes verification infrastructure only; the earlier native gameplay and migration evidence boundaries remain as recorded above. The initial focused invocation could not start its optional transcript/TEMP setup, but the reporter retained its full structured evidence. Hosted rerun results are reported on PR #766 and in the local publication receipt after they complete.


## Networking test synchronization follow-up

Both hosted PR gates passed all 5,918 tests at `592474df`. The duplicate Windows push run then exposed an existing race in `PlayerConnection_InputBackpressurePreservesEveryCommandInOrder`: the advisory queue count includes producers before their channel write becomes readable, but the test made exactly 24 consume attempts. It now waits for all 24 actual deliveries using the existing bounded poll and retains the exact complete ordered collection assertion. Production networking and celestial code are unchanged.

The focused network workflow suite passed **18/18** tests, run `20260926T131313Z-0406b777a363`; the affected case then passed **20/20 separate repeated executions** ([receipt](../../.artifacts/authored-celestials/network-ci-fix/repeat-summary.json)). The full 5,918-case local gate above precedes this final test-only synchronization change; final hosted gates run the published commit. Historical [successful PR logs](../../.artifacts/authored-celestials/ci-harness-repaired-pr-pass.log) and the [duplicate push failure](../../.artifacts/authored-celestials/ci-push-windows-second-failure.log) are both retained. Final hosted status is recorded on PR #766 and in `publication.json` in the local bundle.
