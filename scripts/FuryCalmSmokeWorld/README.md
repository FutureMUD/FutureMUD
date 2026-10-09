# Fury/Calm disposable-world qualification

This scenario uses the existing `AuthoredCelestialSmokeWorld/native.py` world provisioner and the repository's `futuremud-mud-tester` Telnet helper. The provisioner creates a new loopback MySQL instance, imports the maintained blank snapshot, runs the standard Debug replay, checks refusal of a second nonblank replay and stops the owned MySQL process in `finally`. It retains the database directory and receipts beneath `.artifacts/authored-celestials/run-*`.

On Windows, build serially before running:

```powershell
dotnet build scripts/FuryCalmSmokeWorld/FuryCalmSmokeWorld.csproj -c Debug -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
dotnet build scripts/AuthoredCelestialSmokeWorld/AuthoredCelestialSmokeWorld.csproj -c Debug -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
python -B -u scripts/AuthoredCelestialSmokeWorld/native.py --no-build --smoke-script scripts/FuryCalmSmokeWorld/smoke.py --qualification-marker fury-calm-slice-passed.json
```

Use a short, unique owned `TEMP`/`TMP` directory when the sandbox's default temporary directory has restrictive ACLs. Do not rebuild, modify the tested scripts or operate the database concurrently with the scenario.

`Program.cs` refuses connections outside the orchestrator's owned loopback world and verifies `@@datadir` before mutation. It authors explicit test dependencies and writes their actual IDs to `fury-calm-bindings.json`. These choices are fixture policy, not installer defaults: Constitution, effective capacity `variable*100`, one native attribute unit per endurance point, intensity one, distinct authored terrain proxies, impossible Calm saves, character/character eligibility, Self gathering and selected native provision/light profiles. Public builder commands select Automatic casting, and the stopped fixture explicitly gives its disposable caster controlled grade seven and a 1,000-energy balance. These are deterministic operation fixtures, not mastery or balance qualification. The all-options configuration is expected to own 217 managed records, 12 payloads and 12 admissions per variant. Other configurations have different counts.

The fixture's `restore` mode runs the production `MySqlDatabaseBackupService`, drops/recreates only this disposable database on the verified instance and restores the backup to its original name. Every table checksum must match. The caller must stop its MUD first. Never direct this harness at a shared service or a production world.

The initial scenario covers native installation/reruns, ordinary bodies built and loaded through public NPC commands, explicit enrolment/acquisition, refusal/payment, Fury capacity, reciprocal counters, backup/restore and a cold restart. The receipt deliberately records `milestoneQualified=false`: an initial slice passing does not qualify the remaining combat, missed-attack, lifecycle, callback/failure and full configuration-preservation gates. Transcripts, startup logs, fixture logs and receipts are retained even when a prerequisite fails. Dependent assertions stop at the first failure.

Run the remaining phases sequentially against the same stopped owned directory. Replace `<run-directory>` with the verified directory from the first receipt:

```powershell
python -B -u scripts/AuthoredCelestialSmokeWorld/native.py --no-build --restart-owned <run-directory> --smoke-script scripts/FuryCalmSmokeWorld/installer_failures.py --qualification-marker fury-calm-installer-failures-passed.json
python -B -u scripts/AuthoredCelestialSmokeWorld/native.py --no-build --restart-owned <run-directory> --smoke-script scripts/FuryCalmSmokeWorld/lifecycle.py --qualification-marker fury-calm-lifecycle-passed.json
python -B -u scripts/AuthoredCelestialSmokeWorld/native.py --no-build --restart-owned <run-directory> --smoke-script scripts/FuryCalmSmokeWorld/combat.py --qualification-marker fury-calm-combat-passed.json
```

The failure phase selects the owned Fury record's managed baseline while the MUD is stopped, verifies an actual uncommitted payload change, and kills only that invocation's verified installer connection. It requires unchanged committed table checksums, tests a lost acknowledgement after a real commit, and checks stable retry. A protected `finally` restores the exact earlier builder XML. This explicit fixture preparation is distinct from the installer's preservation of builder edits.

The lifecycle phase restores its explicitly authored ordinary actor's raw Constitution baseline of ten through the native staff command, creates a paid Calm child, stops the MUD for 31 seconds and verifies exact reload and a paused offline lifetime. Public progs probe the effective attribute. Paid casts exercise reciprocal grades, retained Fury state, recasts, both caps, removal, callback refusal and both scheduled expiries. It authors a Fury profile edit before selected/null reruns and requires exact definition and player/NPC table preservation.

The combat phase requires the preceding verified markers. It creates a fifth ordinary NPC and private combat settings, observes target links in a merged four-party fight, and checks selective cessation before administrative cleanup. FullDefense suppresses autonomous attacks. Engagement must preserve Calm before the public stock `kick` command selects an actual native attack. Miss qualification accepts only an observed missed first completed attack while Calm was present; a later miss cannot substitute for an earlier hit. Native FutureProg callbacks create real prepayment and paid replacement combat, followed by durable quarantine, cold restart and staff reconciliation. Temporary save/casting/skill edits, selected combat settings and preferred defenses are restored and verified after success or failure; private fixture settings and progs remain in the disposable world. Spent resources are retained.

Every phase marker is published by the existing runner only after its fresh scenario receipt and owned MySQL cleanup pass. Scenario receipts remain bounded evidence; the coordinator separately decides milestone acceptance and retains unrun programme gates.

The fixture retains the Telnet skill helper's existing continuous stdout collector, with one reader per server process. Receipts include redacted server output, exit codes and collector cleanup for each cold start.

Combat skill assertions follow `TraitDefinition`'s effective owner scope, including its normalization of legacy body-scoped skills to character ownership. They read `CharacterTraits` or `Traits` accordingly and verify exact values or absence after restoration. Earlier body-only cleanup assertions cannot establish skill restoration. When recovering a failed invocation, an owned `fury-calm-fixture-recovery.json` names its retained failed receipt, explicit public-command settings, expected persisted save/threshold/difficulty and controlled skill baselines. Recovery must verify those fields before archiving the instructions and taking a new baseline. Controlled baselines are recorded separately from historically exact restoration. A low fixture reserve may be funded explicitly with the MUD stopped; the receipt records it as preparation, never as a cast refund.

N15 uses the same approved guardian template and builder-installed spell as the qualified N14 receipt. Run it sequentially on that stopped retained world after refreshing the Debug engine and fixture binaries:

```powershell
python -B -u scripts/AuthoredCelestialSmokeWorld/native.py --no-build --restart-owned <run-directory> --smoke-script scripts/FuryCalmSmokeWorld/guardian_lifecycle_modes.py --qualification-marker guardian-lifecycle-modes-passed.json
```

Public builder edits select each lifecycle policy; ordinary paid grade-one casts exercise temporary cleanup, death on expiry and permanent output. Temporary modes compare exact terminal journals/versions, claims, foreign-item destination and native graph identities after cold restart. Death on expiry must create exactly one native corpse, then release it through actual scheduled decay. Permanent output survives beyond the authored temporary lifetime with no deadline and remains as an ordinary durable NPC in the disposable world. The native 45*grade/60-second timings are explicit fixture mappings. Guardian/Fury definitions and corpse policy are restored exactly with the MUD stopped; all owned processes stop. Earlier failed receipts remain retained. Complete stock catalogue parity and high-volume N16 are separate gates.

N16's `guardian_churn.py` runs sequential bounded stages on that same stopped world.
Set `N16_STAGE` to 0 through 15 for two cycles per invocation, then 16 for the continuous
128-output batch. Pass the matching marker `guardian-churn-00-passed.json` through
`guardian-churn-16-passed.json` to the owned wrapper. A stage requires the preceding
source-identical marker; each marker is promoted only after fresh qualification and
MySQL cleanup. Do not skip stages or combine receipts from different source inputs.

The 32 cycles cover both temporary modes, early death and natural expiry, alternating
single and paired paid outputs. Every cycle measures exact heavy identities and
`debug census <creator id>` before casting, while active, after cleanup in the same
process and after its cold restart. The last stage creates 128 outputs in one paid
cast, kills 64 early and lets the other 64 expire without restarting during cleanup.
The batch has an explicit 180-second cast-response budget within the ordinary
900-second scenario deadline. Fixture timing is 30*grade seconds / 30-second corpse
decay, and 240 seconds for the batch; these are not historical balance mappings.

The N15 permanent body's short description and personal name are temporarily changed
through `resdesc` and exact-ID `rename`
to distinguish it from new guardians. A harmless `force qaguardian look` must prove
the actual native keyword resolver has no old recipient before any destructive
command. That exact description, canonical name fields, spell definition and corpse policy are restored
byte-for-byte while stopped. Spent energy, evacuated ordinary goods, canonical
archives and journals/claims remain intentionally retained and explicitly counted.
The approved NPC template remains unchanged. No text scan for IDs is used. The census
reports the detached-body registry and reference-distinct bodies attached to loaded
actors/cache/NPCs separately. Interrupted attempts can be recovered with
`guardian_churn_recover.py` and marker `guardian-churn-recovery-passed.json`; this runs
native expiry and public custody only, preserves payment/history, and never qualifies N16.

Complete lifecycle journal snapshots select the nineteen explicit columns through MySQL
`JSON_ARRAY`, preserving empty diagnostics and nulls when the shared SQL helper strips
stdout boundaries. `guardian_journals.py` rejects missing fields or duplicate journal
identities; comparisons retain every field. Run its transport regression with
`python -B scripts/FuryCalmSmokeWorld/test_guardian_journals.py`.

For a retained TSV transport failure, set `N16_JOURNAL_AUDIT_FAILED_RECEIPT` to its
`smoke-invocation-.../receipt.json` path within the owned world and run
`guardian_journal_transport_audit.py` through the same owned wrapper with marker
`guardian-journal-transport-audit-passed.json`. This read-only audit starts no MUD,
compares the actual rows to the failed receipt and never qualifies gameplay.

Set `N16_DEBUG_CYCLE` to one cycle index (0 through 31) for a bounded diagnostic run
through the owned wrapper with marker `guardian-churn-census-probe-passed.json`.
It captures terminal scheduler entries and effects on the known loaded fixtures.
The receipt explicitly sets `n16DiagnosticOnly=true`; it cannot qualify an N16 stage.
Unset this variable for the normal campaign. Failed census waits retain scheduler and
loaded-effect evidence without changing the assertion or its deadline.

The disposable fixture temporarily uses the supported `improver set nogain 0` on
the exact improvement models linked by `TraitDefinitions.ImproverId` and loaded as
classic, branching or theoretical models. Combat checks and skill gains remain
available. Existing timed effects expire naturally within a bounded 500-second
preflight; no effect is removed and no scheduler entry is excluded from the census.
This isolates incidental ordinary skill cooldowns from the guardian lifecycle gate.
Each stage restores every original improver definition byte-for-byte while stopped.
