# Dual Wield implementation and verification receipt

Host Percival; branch `codex/dual-wield-double-attacks`; base `890abfd8c443abec70b10e2bc895535928257f5d`; approved plan commit `1d7ce5db3c6fedbff2ecec099e7de9318aec0f16`; implementation commit `6532fba94c603f0fa64e18a9c35279cf9627fe00`. Worktree: `C:\Users\luker\Documents\Codex\2026-10-16\task-3\FutureMUD-dual-wield`.

Luke's later “Start this one” authorises implementation/verification. No PR, merge or deployment was requested. Other worktrees remain untouched. Master was refreshed and unchanged; #792 remains open at `8cd0da25470c5588923b3eefaeb983605ee69c5c`.

## Implementation

Added normalized/frozen pair selection, ordinary single-target children, shared sequence/host completion, captured physical accounting, callback/interruption guards, one recovery, approved coordination defaults, safe upgrade configuration, fresh binding and additive existing-skill support. Existing dual-only profiles retain single-hit semantics. [Runtime and builder guide](./Dual_Wield_Double_Attacks.md).

## Automated evidence

Windows-native SDK 10.0.401 and repository compact runner, single-node targeted builds; documented runner-local restore/warning switches, no committed blanket suppression. Reports are in ignored `.artifacts/test-runs/`.

| Run | Scope | Result |
| --- | --- | --- |
| Initial targeted runtime build | Core Debug | PASS, zero errors; existing warnings |
| `20261016T033208Z-2ad210034cd3` | Initial mechanics | 19 passed, 6 failed; missing fixture damage profile, fixed afterward |
| `20261016T033527Z-9198a4b3ebb9` | Focused runtime | PASS: 28 passed, no failures/skips, source stable |
| `20261016T033937Z-a18973f910ff` | Runtime, seeder, library | 8,584 passed, 5 failed, 3 skipped, source stable |
| `20261016T035030Z-9de7c02196b3` | Final focused runtime (`DualWield`) | PASS: 40 passed, no failures/skips, source stable |
| `20261016T035346Z-572b64d07ffa` | Final coordination seeder and core-static installation tests | PASS: 24 passed, no failures/skips, source stable |

Consolidated core: 6,258 passed. Library: 709 passed. Seeder: 1,617 passed, 5 failed, 3 Windows skips for Unix launcher permissions. One new seeder test's reflection argument mismatch was repaired afterward. Four failures involve unchanged blank-snapshot tests/assets/migrations:

- `CommittedBlankSnapshot_HasDirectRoomOwnershipAndImmutableSpatialProvenance`: missing `rooms`.
- `CommittedBlankSnapshot_RecordsLatestMigrationInHistory`: absent `20261008024603_TargetedLegacyRoomReferences`.
- `CommittedBlankSnapshotManifest_TracksLatestMigration`: records `20261006161646_CellSpatialContraction` instead.
- `CommittedBlankSnapshot_ContainsEnvironmentalStateAndDurableOperationKeys`: missing `roomenvironmentalstates`.

Those inputs have no diff from base. No snapshot refresh/rebuild or unrelated repair was attempted. The full combined gate is not green. The final focused runs cover the repaired reflection fixture, fresh enablement after core setup, explicit setting preservation, captured-body accounting, lethal completion and interruption after attack/defence callbacks. No runtime or seeder source changed after those final receipts.

## Native smoke: representative mechanics and restart passed

Automatic approval review initially rejected startup under the original planning-only boundary. The source task then supplied the authenticated exchange: the assistant said planning was complete and ready for implementation at 03:08:45 UTC; Luke replied directly “Start this one” at 03:20:44 UTC. One retry of the same `prepare.ps1` startup action, with that evidence, was accepted. No approval blocker remains; no denial was bypassed through another route.

MySQL 8.0.45.0 ran under this worktree's `.artifacts/dual-wield-native/mysql/data`, on `127.0.0.1:34317`, database `dual_wield_20261016`, server UUID `bf9f6acb-c915-11f1-a3eb-30560f3c3fde`. Every database operation checked the owned endpoint, UUID and datadir. The MUD used this worktree's ignored Debug runtime and loopback port 4009. Email and Discord were disabled before boot. Repository-local `mud_session.py` supplied launch, telnet, redaction and cleanup; a queued operator reused its functions for inspected builder steps.

The fixture backup was `C:\Users\Luke\source\repos\FutureMUD\DatabaseSeeder\bin\Debug\net10.0\Backups\demo_dbo-DB-Backup-20260828063816.sql`, 108,608,395 bytes, SHA256 `7D865AA4697D02869FACA7330B1A19918FAE22BC350345E9B3CB63E3056274D7`. All three source DROP/CREATE/USE directives were rewritten to the owned database; 550 tables were restored and source database absence was verified. The first import stopped at line 8 because its unconditional DROP named a new, absent schema; the guarded continuation confirmed no target existed and changed only that DROP to IF EXISTS. One manual import completed. No seeder/reseeder ran.

Native startup applied 32 pending migrations, ending at `20261008024603_TargetedLegacyRoomReferences` (272 applied total). It then exposed an unrelated legacy trap failure: `TrapTriggerDefinition.LoadFromXml` rejected `CellEntry` in templates 5, 6, 11 and 12. A later boot automatically recovered its pre-upgrade backup and reapplied migrations, undoing the initial fixture correction. After verifying all migrations applied, the owned failed-boot marker was archived and only those four disposable XML labels were changed to `RoomEntry`. Subsequent boots reached ready state. Both failures and the automatic recovery are retained; the fixture was not repeatedly reset to obtain favourable combat results.

Dedicated native skill 197 was cloned from existing skill 186 with Character ownership, retaining the native cap/improver. Check 227 used `dualwield:197{nobranch}` (expression 490), Standard, ImproveTraits=1, FailIfAnyMissing, CanBranch=0. New NPC templates 133/134 copied 108 with trained/missing coordination skill respectively; original NPC templates were preserved. Actual characters 56/57 were inspected for skills and setting 65, then equipped through ordinary item load/give/wield commands with dagger 5973 and mace 6001. Their inventories proved two wielded weapons before `add character` simulations. Successful outfit-template loading had produced empty inventories, so template-source runs were rejected as mechanic evidence rather than counted as passing tests.

All accepted scenarios used seed 4000, at most two virtual minutes, 2,000 events and 2,000 transcript entries. Output was read from one participant's perspective to avoid counting recipient echoes as extra attacks.

| Native case | Observed evidence |
| --- | --- |
| Disabled control | Ordinary weapon attacks; one attacker strike per timestamp and no coordination use. |
| Enabled at default weight 0.25 | Untrained slot 2 attacked with mace and dagger at `00:00:00.350`, with fresh defences; failure did not gate strike two. |
| Existing Dual Feint | Slot 2 at `00:01:28.950`: one feint attack, with no second strike at that timestamp. |
| Trained diagnostic, temporary weight 100 | Slot 1 at `00:00:00.100`: dagger then mace, masterful then failed parries. Exactly one Normal Dual Wield MinorSuccess log preceded separate Edged and Bludgeoning checks. This is mechanism evidence, not a balance result. |
| Missing skill | No Dual Wield branching log; source character 57 retained zero coordination trait rows. |
| Clean restart | Enabled=true, weight restored to 0.25, recovery=1.5, numeric binding, skill distinction and both wielded weapons survived. Helper expectations and structured assertions passed. |

The verified DLL SHA256 is `82B5893CFC24DEBAB53FDFB86DEA2D34FB430CD01A3D62CBC9A92BB244BA94E5`. Native evidence is retained locally under `.artifacts/dual-wield-native`: `mechanics-verification.json`, `evidence-manifest.json`, redacted `operator`/`restart` transcripts and logs, `skill-audit.txt`, `final-database-checks.txt`, and `cleanup.json`. Generated helper bytecode was restored; no runtime/seeder source changed during native verification.

Owned MUD and MySQL processes were stopped by 04:24:33 UTC. Ports 4009/34317 are closed. Existing MySQL listeners 3306/PID6852 and 3307/PID7020 retained their identities. Only disposable data and logs remain; production databases and other worktrees were not modified.

## Remaining limits

Numerical stamina debits, one recovery action and the full first-hit/counter/callback interruption cases are covered by focused automated mechanics tests, rather than exhaustive native instrumentation. The representative native runs do not establish balance or every world-specific installer combination. The legacy trap/outfit findings are deferred outside this feature; fixture corrections were not runtime fixes. The seeder gate retains four unchanged snapshot failures and three platform skips. No feature EF migration is needed. No unresolved product choices or approval requests block this implementation; assess the existing snapshot gate and #792 enum/wrapper conflicts before publication. No PR, merge or deployment occurred.
