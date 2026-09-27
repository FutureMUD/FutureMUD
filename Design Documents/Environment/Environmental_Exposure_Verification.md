# Environmental exposure verification

## Scope and identity

Implementation base: `cdf6f79997cc7b5081e51ceee2e1c45cf6adc39a`. The feature is an uncommitted working-tree change; no published commit or CI result is implied. Repository test receipts record source fingerprints before and after each run. This report separates arithmetic/resolver evidence, runtime integration, native installed data and live gameplay.

The worked N01-N12 figures are synthetic fixtures. They are not stock-world balance values. The production code uses normal material selection, localised wetness, clothing/container routes, shared finite sources and ordinary wounds; a mock damage collector proves dispatch/arithmetic only where explicitly identified as such.

## Commands and reproducibility

Tests use the repository compact reporting wrapper, serial builds and an explicit project/filter. On this Windows worktree, Git's safe-CRLF warning output stalled the source-fingerprint subprocess. Runs therefore used process-local Git configuration; repository and global Git configuration were not changed:

```powershell
$env:GIT_CONFIG_COUNT = '1'
$env:GIT_CONFIG_KEY_0 = 'core.safecrlf'
$env:GIT_CONFIG_VALUE_0 = 'false'
# Run scripts/test-unit.ps1 with the project/filter recorded in each receipt.
```

The fixture import and catalogue capture use:

```powershell
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Debug -m:1 --no-restore
dotnet run --project DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-build --no-restore -- --capture-environmental-exposure --native
```

Live Telnet scenarios use the repository `futuremud-mud-tester` helper with explicit checkout/account and an owned disposable database. Credentials come from local test configuration and are not part of this document or transcript. Only the helper-owned server is stopped. A successful login or dry-run diagnostic is not recorded as a damage assertion.

## Intermediate failures retained as evidence

| Receipt | Observed result | Resolution / significance |
| --- | --- | --- |
| `20260926T130448Z-a985ef2bbf04` | Library: 12 passed | Earlier arithmetic revision only. |
| `20260926T132959Z-2a5974c655bb` | Seeder: 15 passed | Focused seeder/replay/snapshot selection at its recorded fingerprint. |
| `20260926T133101Z-db912edd47d7` | Core: 42 passed | Early exposure and magical-substance selection. |
| `20260926T134811Z-1e2dc3953172` | Core: 72 passed, 11 failed | New fixture lacked a material plane; an existing drying test exposed duplicate change notifications. Fixtures and mutation notification batching were corrected. |
| `20260926T142256Z-fee229c41473` | Core: 82 passed, 17 failed | Recursive mock defaults invented planes/progs, and the saved-effect test lacked factory registration. Corrected fixture registries and registration. |
| `20260926T142825Z-c24eae4db678` | Core: 99 passed, 2 failed | Thermal source combination failed target identity matching. The cache now uses reference identity; subsequent results below govern acceptance. |

Earlier failed receipts are not counted as successful verification. Later tests include the fixes and additional regressions; their counts and fingerprints must be read independently.

## Persistence and installer evidence

The migration and designer were generated with the version-matched EF tool. `has-pending-model-changes` passed after model/context/snapshot alignment. The blank SQL snapshot includes the exact idempotent migration delta and updated manifest. That SQL successfully imported into multiple owned MySQL fixtures at port 3307.

A full blank-snapshot regeneration was attempted but failed: sandbox TLS/SSPI reported `0x8009030E`, and the escalated refresh connection reported unsupported `auth_gssapi_client`. The checked-in SQL was updated from the generated migration delta instead. Successful fixture imports verify that route, but are not described as a successful full refresh.

InMemory catalogue attempts failed on MySQL default/collation differences. An initial native replay then exposed a missing lazy-loading-proxy option in the capture harness. It was corrected to match the installer. The next native replay stopped at the existing Industrial ItemSeeder guard for 464 non-production-ready food concepts. The final capture excludes that guarded item package and records the exclusion; no readiness assertions were weakened.

## Final acceptance evidence

### Automated checks

| Receipt | Executed result | Scope |
| --- | --- | --- |
| `20260926T152023Z-aa4ffd81e7b1` | 5,856 passed, 4 failed, 0 skipped | Earlier full fast gate. Boot-before-service initialisation, the spell compatibility manifest and the old inline animal-acid fixture were repaired. The unrelated title-case apostrophe failure is retained as a baseline issue. |
| `20260926T154512Z-d115ddddf904` | 26 passed, 0 failed/skipped | Exposure seeder, replay, blank snapshot and natural ranged attack selection. |
| `20260926T225043Z-838aab11a906` | 186 passed, 0 failed/skipped | Runtime exposure and dependent surface-liquid, breathing, substance, spell, trap and boot tests after traversal optimisations. |
| `20260926T230522Z-fd28a90b9331` | 190 passed, 0 failed/skipped | Same runtime selection plus body inventory/identity registration and 0.25-second default. Source fingerprint `747f95f922661e6eda279fe4f1c49970119d7b656aa4cd90bff6f9e0f0a1f44f` was stable. |
| `20260926T231807Z-c38736ce193f` | 11 passed, 0 failed/skipped | Seeder rules, migration of combined gas routes, builder customisation and actual seeded human/dog organ armour calibration. |
| `20260926T232743Z-8fc2b02921af` | **5,887 passed, 1 failed, 0 skipped** | Final complete fast gate: 5,888 native TRX results across ten projects. All 3,704 core, 1,392 seeder and 62 persistence-library tests passed. The sole failure is the unchanged `TitleCase_CapitalisesWords` assertion (`Earth's Sky` expected, `Earth'S Sky` actual). Overall gate status is **FAIL**, not a blanket pass. |

Each receipt resides under `.artifacts/test-runs/<run-id>/` with native TRX, invocation logs, `summary.json` and `failures.json`; successful console text alone is not the basis for these counts. All listed runs were complete and reported stable source fingerprints. The final gate's start/end fingerprint was `7310c3561bcfaa1510d41aca500c8c26d47d04efe54edfe05e6233dac0cb736a`. Its remaining projects cover shared library, expressions, bot, converter, website, terrain planner and test-reporting infrastructure. Git comparison confirms that `StringExtensions.cs` and its test file are unchanged from the implementation base. No executable input changed after this gate; final report/ignore-list edits and relocation of initial scratch notes are non-executable housekeeping.

The latest focused commands were:

```powershell
& scripts/test-unit.ps1 -OutputMode Compact -Configuration Debug -TimeoutSeconds 900 `
  -Project 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj' `
  -Filter 'FullyQualifiedName~EnvironmentalExposure|FullyQualifiedName~SimpleLivingDirectHealthCostTests|FullyQualifiedName~SurfaceLiquid|FullyQualifiedName~BreathingStrategyTests|FullyQualifiedName~MagicalSubstanceTests|FullyQualifiedName~VancianEffectTests|FullyQualifiedName~TrapModuleDefinitionTests|FullyQualifiedName~BootLoadingRegressionTests|FullyQualifiedName~VancianSnapshotTests|FullyQualifiedName~BodyInventoryInitialisationTests'
& scripts/test-unit.ps1 -OutputMode Compact -Configuration Debug -TimeoutSeconds 900 `
  -Project 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' `
  -Filter 'FullyQualifiedName~EnvironmentalExposureSeederTests'
& scripts/test-unit.ps1 -OutputMode Compact -Configuration Debug -TimeoutSeconds 1800
```

The wrappers use `-m:1` and no-build/no-restore execution after their targeted build. Local NuGet audit warnings are suppressed through the repository-supported switches; this is not a dependency-security audit. The final Debug seeder build succeeded with zero warnings/errors in `.artifacts/exposure-seeder-calibration-build.log`.

The final serial Release command `dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Release --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510` also succeeded with zero warnings/errors, including its runtime dependencies. `.artifacts/exposure-final-release-build.log` records exit 0. This verifies the Debug-only capture path remains excluded from Release compilation.

### Executable contract coverage

| Behaviour | Executable evidence |
| --- | --- |
| N01–N09 reference arithmetic, units and finite budgets | `EnvironmentalExposureArithmeticTests` and `EnvironmentalExposureTests`; explicit damage and source-amount assertions, shared exhaustion, resistance independent of consumption, dilution, spent carrier and invalid values. |
| N10 timing, disable/re-enable, stationary contamination | `EnvironmentalExposureIntegrationTests` uses the real service with a controlled clock; sub-tick movement, subdivided advances, effect expiry, save-flush deferral and reads without mutation. |
| N11 reagent batching and persistence | `MagicalSubstanceTests` exercises one delivery split over parts, retained enumeration, lot/charge state and effect delivery. |
| N12 layers, containers and physical ownership | Integration tests cover transmission independent of susceptibility, failed layers, gas-tight nesting, owned finite vessel contents, puddles and immersion with harmless pre-wetting/saturation. |
| Breathing and thermal sources | All strategy anatomy selections, clean supplies, failed withdrawal, no airflow/non-breathers, cloud deduplication/cap, route protection, thermal-channel maximum and bounded/failed intensity progs. |
| Ordinary wounds, bodies and corpses | Concrete health-strategy/wound tests cover caps, modifiers, persistence, accumulation and source separation; actual target bodies and corpse anatomy/lifecycle are selected explicitly. |
| Seeder and compatibility | Opt-out, fresh definitions, same-option reruns, missing owned content, customisation conflicts, v1 coefficient retention, route split, actual stock organ armour and blank-snapshot/replay tests. |

These tests vary in fidelity: controlled service fixtures mock world entities, while wound/armour tests use the concrete named implementations. They do not stand in for every native gameplay permutation. The native cases below are separately identified.

### Native catalogue and persistence

The full-install fixture is `futuremud_exposure_269767ee2eb3485db87fad96`. All 31 selected seeders completed in `.artifacts/exposure-native-final.log`. The final calibration was then installed by:

```powershell
dotnet DatabaseSeeder/bin/Debug/net10.0/DatabaseSeeder.dll --capture-environmental-exposure `
  --resume-owned futuremud_exposure_269767ee2eb3485db87fad96
```

`.artifacts/exposure-native-final-calibration.log` records that completed rerun. The final JSON/CSV contain 1,378 solids, 630 liquids and 70 gases, 340 profile/family interactions, five demonstration equipment definitions and no conflicts/deferred entries. All 2,078 catalogue identities have observed producer provenance. The JSON retains the original 31 completed seeders separately from the latest exposure-only execution; it does not call the rerun a new full install. The matrix records distinct inhalation responses. The audit explains 645 unsupported/uncertain entries rather than fabricating compatibility for them.

Migrations `20260926124947_AddEnvironmentalExposureDefinitions` and `20260926145630_ExpandLiquidExposureDefinitions`, their designers, EF snapshot, blank SQL and manifest are aligned. `.artifacts/exposure-model-parity.log` records no pending model changes. The blank snapshot's successful native import is distinct from the blocked full-regeneration attempt described above.

### Live gameplay observations

The live fixture is the separate owned database `futuremud_exposure_dfb3bef821b04a83bcc7f0de4d397442`. Tests used an ordinary human and dog NPC, plus fresh human NPCs for identity and respiratory checks. Stock human natural-armour quality is 2; dog quality is 5. No ordinary armour formulas were weakened. The final server was stopped after restoring air, Void terrain, 25 C and `Disabled` mode.

Transcripts and queried saved rows are in `.artifacts/exposure-live-{fourth,fifth,sixth,seventh,eighth}-*`. Commands and UTC batch timestamps are retained. A requested three- or five-second wait is not claimed to be the exact contact time: editor, command and heartbeat processing add elapsed time.

| Scenario | Observed result and evidence |
| --- | --- |
| Disabled / localised acid | The disabled wetting control caused no injury. After enabling, 0.1 mL hydrochloric acid on the human left hand produced 0.023645 saved damage; the dog's left paw packet was absorbed by its armour. Fourth transcript and saved channels identify the actual parts. |
| Naked acid pool | The short shallow-acid sequence produced 9.4812 total damage on the human and 10.1145 on the dog, with mild pain. Fourth transcript/totals. |
| Lava and retained contact | The shallow-lava sequence produced 29.3024 human and 30.8757 dog damage. A subsequent enabled interval outside the pool raised totals to 43.2314 and 48.6146, proving retained lava continued injuring. Fifth `lava-settled.json` and `retained.json`. |
| Finite compatible/incompatible vessel | Both closed vessels began with 236 mL HF and 31.67 HP. The glass fell to 30.22 HP in the first interval while polymer stayed unchanged. Later the glass was explicitly pre-weakened; continued interior exposure destroyed it and left approximately 220.09 mL in the physical cell. The polymer retained 236 mL. Sixth `vessel-first`, `vessel-destruction` and `spill` artifacts. This is not a healthy-flask destruction-time claim. |
| Direct ambient heat | At 80 C, the short sequence produced 21.5432 human and 16.7815 dog damage; health output still said Normal Temperature. Direct burning therefore remained distinct from physiological temperature imbalance. Sixth `heat-baseline.json`. |
| External preparation and expiry | A non-caster human received 1 mL elemental ward tincture on the abdomen, yielding external-route multiplier 0.25. The next heat interval produced 4.5824 human damage while the unprotected dog took 14.2785; correcting for the control's elapsed interval gives a 0.25 human ratio. The timed preparation later disappeared. Sixth transcript and `heat-protected.json`. |
| External gas | The chlorine overlay caused external chemical wounds on humans and dog, independently of respiratory protection. Sixth and eighth channels/transcripts. |
| Respiratory calibration | The original shared 0.6/s rule produced no respiratory wounds because organ armour absorbed it. An enabled diagnostic proved the cause. After separate 18/s stock inhalation calibration, eighth-run saved lung/trachea damage was 19.6725 each on the new human and 4.3548 each on the dog. The first actual samples were approximately 4.4 seconds. |
| Respiratory preparation and held breath | The human given 10 mL respirant draught had no lung/trachea wounds; diagnostic resistance was 0.25 and predicted post-armour injury zero. It still held its breath in chlorine. The fresh unprotected human's three 19.67 wounds were unchanged across a second explicit `impdebug heartbeat 10second`; diagnostic held breath was 20 seconds. Eighth transcript lines 488–490, 618–620 and following diagnostics. |
| Save/reload | Seventh-run respiratory wounds of 20.1871 per organ reloaded as 20.19 in the eighth health display. The respiratory preparation and substance activation also reloaded. Offline time added no exposure injury. Healing/pain changes remain governed by existing health rules. |
| Performance and fresh NPC | Earlier native runs exposed recursive saving, repeated whole-body work and unset new-body identity. Those were fixed and regression-tested. The final five-body scenario created a new NPC, applied gas, ran diagnostics/health and disabled exposure without the prior save loop. Logged slow heartbeats were 0.13–0.39 seconds and command processing 0.251–0.448 seconds on this machine. This is a bounded smoke observation, not a large-world benchmark. |

The first acid/lava attempts that stalled, the disabled-mode respiratory diagnostic and the pre-fix fresh-NPC stall are retained as failed or inconclusive evidence. They are superseded by the specific successful checks above, not retrospectively counted as passes. The seeder calibration regressions compare actual emitted rules and actual stock organ armour at 10 seconds and forty 0.25-second packets, while confirming ordinary combat remains strike-based.

### Limits

The dedicated long-running climate suite was not required or executed; climate transition behavior was not changed. Native smoke did not exercise every clothing combination, supplied-air component, item-targeted ward, multi-body transformation or trap-cloud arrangement; those paths have the stated automated coverage, not a blanket native acceptance claim. The existing Industrial ItemSeeder readiness guard, blank-snapshot refresh authentication limitation and unrelated title-case failure remain visible. Startup also reported existing missing combat-message defaults and unavailable Discord; the server nevertheless completed boot and the recorded gameplay scenarios.

Normal ingestion/injection reaction damage was considered and left with existing drug, poison, food and reagent systems; new exposure rules resolve external liquid/gas, actual respiratory samples and ambient heat. There is no thermodynamic item-temperature, chemistry, protective-film or ignition simulation. Stock rates are deliberately authored game balance and the strong chlorine example can cause severe respiratory injury on its first sample. Custom games need to calibrate against their own anatomy, armour and health settings.
