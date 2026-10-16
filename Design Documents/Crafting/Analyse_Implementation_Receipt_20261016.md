# Analyse implementation receipt — 2026-10-16

Branch: `codex/analyse-capability`. Independent worktree: `C:\Users\luker\Documents\Codex\2026-10-16\task-5\FutureMUD-analyse`.

Verified base: fetched `origin/master` at `890abfd8c443abec70b10e2bc895535928257f5d` (PR #791 merge). The initially stale local master was fast-forwarded only on this new branch before implementation. The planning workspace and all other worktrees were preserved. No merge, deployment or PR publication was performed.

## Delivered contract

See [Analyse capability](./Analyse_Capability.md) for commands, conservative product matching, recipe gates and the future Armageddon binding manifest. `AnalyzeEligibilityCheck` uses persisted enum ID **229**; sibling feature branches must retain distinct enum IDs when integrated. Both skill seeders and missing-check upgrade loading default this optional capability to disabled using a zero-modifier Static Check. No Analyse skill or history state is introduced. Role/race/culture/merit expression options use their owning world.

The Armageddon handoff preserves all seven recovered main-guild grants and their provenance, with no recovered subguild grants. Its installation must resolve package-owned class roles to actual target-world IDs. No active Armageddon branch was edited or seeded.

## Automated verification

Native Windows SDK: `10.0.401`. Targeted Debug build and project restores used `-m:1`; local restores disabled NuGet auditing because this was not dependency-audit work. No package versions or committed warning settings changed. Initial missing-import and test-fixture setup failures were repaired; their local logs remain under `.artifacts/analyse-tests/`.

| Run | Scope | Result |
| --- | --- | --- |
| `20261016T051947Z-f274a208d996` | Consolidated runtime filter: `CraftAnalysisTests`, `CraftToolUsageTests`, `CookedFoodProductTests`, `LiquidProductTests`, `SimpleVariableProductTests`, `ProgVariableProductTests`, `ProgCookedFoodProductTests`, `CraftInputPersistenceTests`, `CheckTests` | **PASS: 45 passed, 0 failed, 0 skipped** |
| `20261016T052005Z-cff681b1917a` | Full `DatabaseSeeder Unit Tests` project | **FAIL: 1,602 passed, 4 failed, 3 skipped** |

Both were single consolidated runs through `scripts/test-unit.ps1 -OutputMode Compact`, with `-TimeoutSeconds 900`, explicit projects, and results root `.artifacts/analyse-verification/`. Both completed with stable source fingerprints. All five new runtime tests and `AnalyzeEligibilitySeederTests.SeedChecks_AnalyseIsDisabledStaticWithoutTraitOrProgression` passed. The latter executes real seeding against an in-memory database and verifies the disabled formula, static template, eleven zero modifiers, absent trait parameters and no trait creation.

The four full-suite failures are deterministic checks of the **unchanged committed blank snapshot**:

- `CommittedBlankSnapshot_HasDirectRoomOwnershipAndImmutableSpatialProvenance`: snapshot does not create `rooms`.
- `CommittedBlankSnapshot_RecordsLatestMigrationInHistory`: snapshot omits `20261008024603_TargetedLegacyRoomReferences`.
- `CommittedBlankSnapshotManifest_TracksLatestMigration`: manifest records `20261006161646_CellSpatialContraction` instead of that latest migration.
- `CommittedBlankSnapshot_ContainsEnvironmentalStateAndDurableOperationKeys`: snapshot does not create `roomenvironmentalstates`.

The three skips require Unix launcher permissions on a Unix host. These failures and skips were not suppressed or repaired. `.artifacts/analyse-verification/baseline-snapshot-evidence.json` confirms identical Git blobs at the verified base for the snapshot, manifest, snapshot manager, failing tests and latest migration. The snapshot SQL blob is `716a621cd4518b0e113bcabd0c5bf00dd72ef6f5`; manifest blob is `c10ced214890471ed04fe2cb2d0bc27b801e933a`.

Exact receipts: `.artifacts/analyse-verification/<run-id>/summary.json`, `failures.json`, source fingerprints and invocation TRX/native logs. Runtime fingerprint: `ea1ba5fd9d4da188985970f7e36ffee99e54d7c6dad981d592590aec85b9dde9`; seeder fingerprint: `d36589e217be6aec5eadfdc7eeb4f99fc34bbfddc8779588cb7b497bb1e00a58`. Generated Python bytecode was restored afterward; subsequent tracked changes were documentation only.

Independent read-only defect review found no concrete P1/P2 findings. `git diff --check` passed.

## Native MySQL and command acceptance

Used repository skill `.agents/skills/futuremud-mud-tester/SKILL.md` and its Telnet helper with current branch binaries. Runtime `MudSharp.dll` SHA-256: `4bde7118b216474132244fd75c4e8aeed8f6c3e6054f137f09e774912ba37787`.

Restored once from `demo_dbo-DB-Backup-20260828063816.sql`, length 108,608,395 bytes, SHA-256 `7d865aa4697d02869faca7330b1a19918fae22bc350345e9b3cb63e3056274d7`. The source backup remained unchanged. An identity-checked loopback-only MySQL 8 instance used schema `analyse_20261016`, port **34321**, Telnet **4011**, and an owned data directory beneath `.artifacts/analyse-native/`. Restore confirmed 550 baseline tables and absence of the source schema. Email and Discord integrations were disabled. The legacy backup's trap trigger compatibility correction (`CellEntry` to `RoomEntry`) affected only this disposable world.

An initial sandboxed connection was blocked; the owned instance was resumed outside the sandbox after exact datadir/port/UUID verification. VSTest also required host loopback access. No shared database service was used for the scenario.

The first copied recipe fixtures inherited an invalid fail-product echo from the historical backup. They were corrected only in disposable fixture copies. Crafting skill ownership was placed in current `CharacterTraits`, not the legacy body `Traits` table. The failed fixture transcript and precise repairs remain saved; no runtime repair or second restore was needed.

Native fixture IDs were generated/resolved from that database: class **2**, subclass **3**, denied class **4**, recipes **10145–10151**, eligibility expression **489**, character/body **1**, Pottery trait **44**, carried hearth prototype **37**, item **177**. These are test-world identities, not Armageddon bindings or illustrative runtime constants.

`mechanics-verification.json` records **11 passing assertions**: generated help and aliases; disabled-upgrade denial; generic no-match feedback; rejection of a dropped room item; five positive outputs; both eligible alternatives each time; distinct 7kg/8kg clay requirements and Hot Fire tool state; no hidden/restricted/untrained/dynamic/fail-only recipe leaks; actual class-predicate denial; identical persisted trait/item state across repeat queries; and explicit possible-recipe wording without manufacture-history claims.

`restart-verification.json` records **3 passing assertions**: both command spellings after restart, persisted access to both recipes, and unchanged persisted traits and item. Final database state confirms expression `100`, Static Check, no improvement/branching, 11 modifiers summing to zero, and zero Analyse/Analyze trait definitions.

Evidence root `.artifacts/analyse-native/` contains `ownership.json`, `fixture.json`, `fixture.sql`, `fixture-repair.sql`, `initial.operator.transcript.txt`, `invalid-fixture.operator.transcript.txt`, `positive.operator.transcript.txt`, `restart.operator.transcript.txt`, matching session logs, before/after/restart state files, final check state, verification JSON and `cleanup.json`.

Cleanup confirmed **STOPPED at 05:26:37 UTC**, owned PID 53968 exited, Telnet 4011 and MySQL 34321 closed, and existing MySQL listeners on 3306/3307 retained their original process identities. Disposable data and local evidence were retained. No other worktree was modified.

## Readiness and remaining limitations

Implementation is ready for review. The full seeder gate remains red because of the four baseline snapshot failures above; no claim of a fully green repository is made. Dynamic products without explicit matchers remain unsupported by design. Future Armageddon seeder binding is documented, not installed. No PR was published.

## Haggle-first integration follow-up

Analyse was subsequently published as PR **794**. Its prerequisite, Haggle PR **793**, was independently verified merged at `9429227f14eae012af8d2bea6650fe732e920eae` on 2026-10-16 07:16:51 UTC, from feature head `1d9c235418d0997d9b75181e29f5ae9466c66740`. That current master was merged into this branch before final validation.

The enum conflict retains `HaggleCheck = 227` and `AnalyzeEligibilityCheck = 229`. The loader conflict retains the deterministic disabled Analyse expression and Haggle's variable expression. Both seeder cases remain present. Haggle's snapshot, manifest, schema, migration and reference-correction fixes are incorporated unchanged from master; Analyse adds no duplicate baseline repair or migration. The four failures described above are historical results on the original base, not a claim about this integrated head.

Final integration validation and exact-head Linux/Windows CI results are recorded on PR 794 and in the local `.artifacts/analyse-integration/` evidence directory. The original native acceptance evidence above remains intact. No deployment is part of this operation.
