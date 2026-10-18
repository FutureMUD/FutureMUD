# Core tag and non-silt foundation verification

Base: `45b4a7780c3545e1d4f9d8f9b22e50d8162f91e9` (`origin/master` at isolated worktree creation). Branch: `codex/core-tag-terrain-foundations-20261019`. The draft PR's head identifies the committed result. This verification note was added after the frozen native run; executable inputs were unchanged.

## Result and boundaries

Core now owns all former Useful tag declarations through the immutable embedded full-path manifest. The corrected inventory is 2,373 declarations / 2,335 paths: the planning audit missed 67 entries emitted by three existing culture/institution loops. All 17 repeated leaf names remain separate identities. The hierarchy export contains all Core paths plus 209 retained optional-content paths; no old export path was removed.

Sewer and Ruins are Core native presets with explicit proportional stamina and forage adaptations. See [terrain details](Stock_Terrain_Catalogue.md) and [identity/ownership rules](Core_Tag_Foundations.md). Optional components/templates remain in Useful. Item/AnimalButchery retain their genuine Useful component dependencies; optional early firearms still need an approved Sheath donor. Existing games, players and activation settings were not used as test targets. No silt or setting/class/culture rollout was implemented.

The reduced native install exposed Hovering Light's hidden generic Animal Head-shape lookup. Fresh installs may bind Human's existing Head Back; retained aliases survive renames and later optional Animal installation. No anatomy or runtime mechanic was added.

## Checks actually run

| Check | Result |
| --- | --- |
| DatabaseSeeder Debug and Release builds, final executable source | Passed, zero errors |
| Full DatabaseSeeder suite, one run | 1,984 passed, 13 failed, three Unix-permission cases skipped on Windows; 2,000 total |
| Corrections and targeted regressions | All 13 failures cleared; latest results across focused reruns: 120 distinct passing cases, zero unresolved |
| Runtime selector bindings, feature masks and spell shelter compatibility | 40 passed |
| Catalogue audit | 2,335 Core paths represented, 17 repeated leaves distinct, no missing/lost export paths, no export duplicates |
| Fresh reduced baseline | Passed: Core, Time, Attribute, SkillPackage, Human, preindustrial Combat, Chargen; no Useful |
| Sealed SQL restore | Passed exact schema/content roundtrip into a new owned database |
| Core rerun | Preserved all 2,335 tag IDs, bootstrap account count and a valid custom ramrod binding |
| Full Armageddon opt-in on restored baseline | Passed; exactly 152 distinct owned canonical spells resolve to native IDs, no Useful packages/components |
| First/cold native boot | Passed Admin login, LOOK, persisted flush, graceful shutdown and retained avatar/cell |
| Diff whitespace check | Passed |

The full suite was not repeated after its failures were fixed: focused reruns cover every failed method, including the renamed template/dependency regressions. Native broad Debug replay, all optional package combinations, exhaustive gameplay/balance matrices and Unix filesystem behavior were not run. Restore used the repository-permitted local NuGet audit-network workaround; this is not a dependency security audit.

The first native attempt captured the clean baseline successfully, then stopped at the hidden Head-shape lookup. It also detected test-only source edits during that run. Its disposable databases/server were cleaned. The successful retry used the explicitly pinned original fixture, with no repeated full base seeding; source-start and source-end fingerprints match.

## Pinned evidence

Evidence is retained locally under `C:/Users/luker/Documents/Codex/2026-10-19/task` and is intentionally excluded from Git.

- Full suite: `evidence/seeder-full.trx`; original input identity: `evidence/seeder-full-input-identity.json`.
- Focused correction receipts: `foundations-final-focused.trx`, `foundations-corrections.trx`, `firearm-readiness-final.trx`, `armageddon-native-head-foundation.trx`; consolidated latest outcomes: `evidence/final-focused-summary.json`.
- Runtime: `evidence/runtime-tag-bindings.trx`.
- Final builds: `build-debug-qualified.log`, `build-release-qualified.log`; harness: `harness-build-qualified.log`.
- Successful native wrapper: `native-foundations-restored/20261018T212523Z_24ddc94c47f944df91e529ca82b32a82`; its `summary.json` has `native_exit=0`, `source_stable=true`.
- Boot: `prepared-replay-boot-native_foundations_726b833ac264493799bd17ed49611bc3/boot-latest.json`, status PASS, both owned process trees stopped.
- Cleanup: `evidence/cleanup-final.json`; owned database disposed, MySQL shut down after exact-instance verification, owned TEMP datadir absent, original sealed files unchanged. The isolated worktree remains for review; unrelated checkouts were not edited.

| Identity | SHA-256 |
| --- | --- |
| Core manifest | `3f03c2e2594d9d79e1657988113ca2ed3be201df7b832a492f649b5a1e7673fe` |
| Sealed reduced-base descriptor | `E4F512CA87843C64A29719680BD9C4537D5BC78635A5173EF4FE1C16E88C0D88` |
| Sealed SQL | `BE994575092D3D4428551BA1A76BE42009B3AA5522E72CD32ECB1BF576FF8817` |
| Native frozen source-start/end | `EEE09121831E4A8969D8C95A255D441D90E98A62199C966EFD7D2B87D979668D` |

## Remaining gaps

Ruins' increased armour-only movement contribution needs a new runtime decision and remains deferred. Its current stamina value does not claim that distinction. Sewer sludge is descriptive; no liquid layer, automatic poison/infection, guaranteed forage or universal Ruins cover was introduced. Legacy ambiguous tag graphs and missing/retired owned template identities are reported or blocked for explicit reconciliation; no existing game migration was attempted. Merge and deployment are outside this draft PR authorization.
