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
