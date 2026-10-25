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

## Non-terrestrial timing and Armageddon weather — 25 October 2026

This follow-up is a working-tree change based on `8c15d749612addfb1c06af3fa25e285206d0ef41`, on Windows with the repository's .NET 10 targets and owned disposable MySQL 8.0.45. Existing weather receives no automatic timing conversion. No persistence model, migration, public climate interface or new runtime weather type was added.

| Check | Result | Evidence under `.artifacts/` |
| --- | --- | --- |
| Full DatabaseSeeder suite | 2,220 passed; two stale six-question assertions failed; three Unix permission cases not executed | `armageddon/weather-final/seeder/seeder.trx` |
| Focused corrections and weather/replay regressions | 37/37 passed, including the corrected seven-question inventories and failed-executor/no-answer-write regression | `armageddon/weather-final-r2/seeder-focused/seeder-focused.trx` |
| Explicit authored-moon time-context eligibility | 1/1 passed | `armageddon/weather-final-r2/authored-moon-eligibility/authored-moon-eligibility.trx` |
| Combined unique seeder cases across full/focused receipts | 2,224 passed; three Unix-only cases not executed on Windows; no unresolved failures | `armageddon/weather-final-r5/seeder-combined-evidence.json` |
| Full climate suite, existing terrestrial targets unchanged | 50/50 passed, including all 12 Armageddon profile/seed cases | `armageddon/weather-final-r2/climate-full/climate-full.trx` |
| Final controller, forecast, hazard, analyzer and CSV regressions | 52/52 passed, including native climate-clock selection before initial weather | `armageddon/weather-final-r5/core-weather/core-weather.trx` |
| Debug harness and Release seeder builds | Passed; Release reported existing warnings with zero errors | `armageddon/weather-final-r5/native-build/` and `armageddon/weather-final-r5/seeder-release-build/` |
| Replay metadata inspection | Present in Debug, absent in Release | Portable PE metadata inspection of both final seeder assemblies |
| Native wrapper cleanup checks | 4/4 passed with writable workspace TEMP/TMP | `scripts/AuthoredCelestialSmokeWorld/test_cleanup.py` |

The full seeder receipt remains a failed attempt; its two inventory assertions were corrected and have passing focused evidence. The combined receipt matches cases by class and test name rather than treating a targeted run as a new full-suite run. The three inconclusive launcher cases require native Unix filesystem permission behavior. The climate run preceded the final controller-creation default correction; profile definitions, timing helper, analyzer and simulation code were unchanged afterward. The final 52 core cases and native integration below cover that correction.

Each Armageddon row sampled five 693-day years after one burn-in year with seeds `123456`, `8675309`, `424242`. All prohibited-weather occupancy was zero. Measured ranges across those three seeds were:

| Profile | Strong wind/Gale | Dust including choking dust | Largest sheltered seasonal/hourly mean error |
| --- | --- | --- | --- |
| Southern Desert | 8.932–9.177% | 5.183–5.738% | 0.100°C |
| Salt Flats | 13.752–14.402% | 9.780–10.346% | 0.105°C |
| Northern Scrubland | 4.344–4.720% | 2.134–2.216% | 0.071°C |
| Northern Forest | 1.373–1.582% | 0.304–0.465% | 0.049°C |

These are authored fictional climates, not recovered historical regional measurements. Targets and authoring inputs are in the [setting guide](../Seeding/Seeders/ArmageddonSettingSeeder.md#regional-weather).

### Native installation and controller integration

Final owned run: `authored-celestials/run-7fabf71fffc7/`. Receipts are `native-replay.json`, `armageddon-weather.json`, `armageddon-time-sky.json`, `cleanup.json`, `smoke-latest.json`, and `operator-receipt.json`.

The maintained Medieval replay completed all 32 seeders; a second attempt was refused with unchanged all-table checksums. A separate new schema restored the maintained blank snapshot through the production importer and applied migrations. It then installed generic weather against Armageddon time, proving nine hourly entries and scaled conventional onsets in MySQL, and installed all four setting profiles. Fresh EF contexts verified persisted templates, preserved edited description/temperature fields, restored a deleted owned season at its original ID and recovered its graph/table rows.

The concrete runtime controller was created with an unrelated 24-hour clock first in the world at hour 18. It selected the climate's Suk-Krath and nine-hour clock before its initial temperature lookup. Its real clock subscriptions drove 1,080 minute updates through every native hour and a year boundary; a cold controller reload preserved season, temperature and forecast anchor. Reinstall refusal and the hazards update left season dates and temperature tables unchanged. Template reruns retained the existing controller's climate. No template was assigned automatically to a zone or controller.

The time/sky qualification also passed fresh-context reloads, answer persistence and preserving partial reruns. Owned MySQL shutdown was verified and data retained. This follow-up used concrete runtime clock/calendar/celestial/climate/controller objects with mocked surrounding world services; it did not start a game server or run Telnet gameplay.

The earlier native attempt `authored-celestials/run-ce13c06e96bb/` passed the replay/refusal gate but failed in the test fixture's generic EF `EnsureCreated()` path with a MySQL foreign-key creation error. Its verified cleanup and failure receipt remain preserved. The fixture was changed to the supported snapshot-and-migration installation path; no schema repair or model change was introduced. Earlier fixture-assertion failures and the initial simulation-duration assertion are likewise retained under `armageddon/weather-package*`, `weather-final*` and `climate-armageddon-r3/`; they are not counted as passing attempts.
