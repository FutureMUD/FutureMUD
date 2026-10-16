# Dual Wield implementation and verification receipt

Host Percival; branch `codex/dual-wield-double-attacks`; base `890abfd8c443abec70b10e2bc895535928257f5d`; approved plan commit `1d7ce5db3c6fedbff2ecec099e7de9318aec0f16`. Worktree: `C:\Users\luker\Documents\Codex\2026-10-16\task-3\FutureMUD-dual-wield`.

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

## Native smoke: blocked before mutation

Read-only preflight found MySQL 8.0.45.0 and the full fixture backup `C:\Users\Luke\source\repos\FutureMUD\DatabaseSeeder\bin\Debug\net10.0\Backups\demo_dbo-DB-Backup-20260828063816.sql`, 108,608,395 bytes, SHA256 `7D865AA4697D02869FACA7330B1A19918FAE22BC350345E9B3CB63E3056274D7`. Fixtures include NPCs 108/109, dagger 5973, mace 6001 and ordinary Dual Feint. DROP/CREATE/USE references to the source database must all be rewritten.

The proposed owned instance uses `.artifacts/dual-wield-native`, loopback 34317, database `dual_wield_20261016`, and a separate game runtime directory. Guarded scripts exist but did not execute. No datadir, instance, restore, MUD launch or live mutation occurred; existing servers/databases are untouched.

Automatic approval review rejected the isolated instance/restore because it treated the original planning-only boundary as active and did not accept the later delegated implementation authorisation as overriding it. Explicit confirmation for this exact disposable-world smoke remains pending. No alternative bypassed the rejection.

After approval, restore once, boot/migrate with external email/Discord effects disabled, and use repository-local `mud_session.py` plus bounded `impdebug combatsim` scenarios for disabled/unbound upgrades, numeric binding, mixed families, trained/untrained pairs, ordinary Dual Feint, independent defence spending and first-hit interruption. Capture actual child/state/check evidence and single recovery; stop owned processes only. No repeated reseeding for a passing random demonstration.

## Remaining limits

Native boot, installed upgrades, Telnet gameplay and persistence/restart assertions are NOT RUN. The seeder gate retains four unchanged snapshot failures and three platform skips. No EF migration is needed. No unresolved product choices remain. Complete native verification after its specific approval blocker clears and assess the existing snapshot gate before publication; #792 enum/wrapper conflicts require integration review. No PR, merge or deployment occurred.
