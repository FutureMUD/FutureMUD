# Weather enhancement verification

Verified on 28 September 2026 (Australia/Sydney), on Windows with .NET SDK 10.0.401, Debug builds and disposable MySQL 8.0.45. Verification was performed on a working-tree change based on `4c5902af53f7b09cabf676a37d7335f0cd691411`.

The [weather guide](Weather_System.md) covers player commands, builder settings, persistence, uncertainty, stock installation and existing-world configuration. The [native acceptance harness](../../scripts/WeatherSystemSmoke/README.md) describes the executable scenario and ownership/cleanup rules.

## Automated evidence

Evidence paths below are relative to the repository. Detailed native output remains under the ignored `.artifacts/weather/` directory.

| Check | Result | Evidence |
| --- | --- | --- |
| Standard fast suite, all 10 projects | 6,157 passed; 2 failed; 0 skipped | `tests/20260927T145606Z-ced9befca4b7/summary.json` beneath `.artifacts/weather/` |
| Database library rerun with writable TEMP/TMP | 63 passed; 0 failed/skipped | `tests/20260927T150207Z-5e198423974d/summary.json` |
| Full climate suite | 38 passed; 0 failed/skipped | `tests/20260927T150241Z-b2b5fc0aed1e/summary.json` |
| Final forecast, hazard and analyzer regressions | 37 passed; 0 failed/skipped | `final-focused/weather-final.trx` and `final-focused.log` |
| EF model/migration parity | No pending model changes | `dotnet ef migrations has-pending-model-changes --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj --no-build` returned success |

The fast suite's two failures were `CreateBlankDatabaseSnapshot_ExportsMigratedScratchDatabaseWithPlaceholderName` and `CreateBlankDatabaseSnapshot_BackupFailure_RemovesScratchDirectory`. Both encountered `UnauthorizedAccessException` beneath the account's system temp directory. All 63 database-library tests then passed using an owned writable TEMP/TMP directory. The original failed receipt remains a failure; all 6,159 tests in that fast-suite selection have passing evidence across the main run and this project rerun.

The full fast run passed 532 library, 36 expression, 1,394 seeder, 3,963 core, 22 bot, 43 converter, 54 website, 34 terrain-planner and 18 test-reporter tests. Its database-library result was 61 passed and 2 failed before the 63/63 rerun.

The managed fast, database and climate receipts each include complete native TRX/process evidence and matching start/end source fingerprints. The full suites preceded the final queue-limit safeguard. The final 37-test run includes that additional regression, and the final native run below used its resulting server build. Only documentation changed after final native acceptance.

Coverage includes:

- Shared hazard XML compatibility and version-stable weather randomness.
- Schedule playback, query-order independence, save/reload, horizon changes, slow processing intervals, mid-minute astronomy, season boundaries, custom clocks and oversized-queue fallback.
- Physical and authored celestial frames, timezone day keys, stable calendar selection and administrative calendar-date changes.
- One saved reading per character/controller/day, imperfect interpretations, both presentation formats and indoor recall.
- Lightning target selection, damage, loose items, ground splash, shelter/interior/underwater exclusions and effective-controller subscriptions.
- Magic/weather/overlay atmosphere precedence and restoration.
- Fresh stock hazards, legacy stock adoption, builder-change preservation, missing-air recovery and repeatability, plus all existing long climate regressions.

## Native MySQL and Telnet evidence

Run root: `.artifacts/weather/native-20c705d93b/`.

The fresh run refreshed the blank snapshot through migrations, imported it into an isolated database, and completed the `medieval-standard` seeder replay. A second full installation was refused without mutation; database digests before/after matched. `fresh-verification.json`, `native-replay.json`, `snapshot-refresh.log` and `snapshot-import.log` preserve this evidence.

The imported schema independently reported:

```text
ForecastHorizonDays  int       7
ForecastState        longtext  NULL
```

The final native gameplay scenario is `smoke-56991c67`. Its `receipt.json` records **71 passing assertions**, and `transcript.txt` contains the redacted Telnet sessions. The final root `verification.json` reports PASS using the previously imported owned database. `final-build-context.json` records the source base, build/test evidence and server assembly SHA-256, verified unchanged after the run:

```text
B2E2BC787532E3B0FD6FE327AC907825AD01DEAD45436A20B38B15FD03EC1AA8
```

The scenario proved public controller creation and zone assignment without reboot; a Meteorology-gated saved forecast shared by narrative/table views; dusty-air and choking-dust overrides followed by normal-air restoration; lightning flash, thunder and a direct strike with electrical wounds independently read from MySQL; unchanged issued readings after weather/clock edits; rain-clone liquid/hazard persistence; indoor recall; and exact future-queue/private-RNG preservation across graceful shutdown and restart.

Both server sessions exited normally. `cleanup.json` confirms the owned MySQL process stopped and its disposable data was retained. The earlier passing fresh-world scenario remains under `smoke-c492e53f/`.

## Reproduction and superseded attempts

On this Windows worktree, large uncommitted diffs can emit enough CRLF warnings to stall the existing test reporter while it reads Git's redirected streams sequentially. Use process-local settings and writable scratch space when reproducing the broad suites:

```powershell
$weatherTemp = Join-Path (Get-Location) '.artifacts/weather/repro-temp'
New-Item -ItemType Directory -Path $weatherTemp -Force | Out-Null
$env:TEMP = $weatherTemp
$env:TMP = $weatherTemp
$env:GIT_CONFIG_COUNT = '1'
$env:GIT_CONFIG_KEY_0 = 'core.safecrlf'
$env:GIT_CONFIG_VALUE_0 = 'false'
./scripts/test-unit.ps1 -OutputMode Compact -ResultsRoot .artifacts/weather/tests -TimeoutSeconds 1800
./scripts/test-unit-climate.ps1 -OutputMode Compact -ResultsRoot .artifacts/weather/tests -TimeoutSeconds 3600
```

Run these in a dedicated PowerShell process. They do not change Git configuration files. The earlier run `20260927T144902Z-413009e068a6` stalled before executing any tests and was cancelled after its owned process tree was verified; it is excluded from passing evidence.

An earlier native attempt (`native-52c9e1dba4`) exposed a stock-gas lookup that recognized legacy `air` but missed the actual `Breathable Atmosphere` name. The lookup and partial-install recovery were corrected, followed by four passing hazard-seeder regressions and the fresh native replay above. Earlier failed timing/calendar fixtures and compilation attempts remain in local evidence and are not counted as passes.

## Verification limits

The native scenario exercises a direct character strike and atmosphere transitions. Loose-item damage, ground splash, protected targets and magic precedence have automated runtime coverage. Long-term dust dosage and lightning lethality are configurable gameplay defaults; this work does not establish a balance calibration for a particular world.

Builds succeeded with compiler warnings, including two nullability warnings in forecast interpretation and the hazard test fixture. No hosted CI or production deployment is part of this verification.
