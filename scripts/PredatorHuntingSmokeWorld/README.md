# Predator hunting native verification

This harness uses the isolated MySQL provisioning and process ownership checks in `AuthoredCelestialSmokeWorld`. It does not use the installed MySQL service or an existing game database. Read the [feature guide](../../Design%20Documents/AI/Predator_Hunting.md) for configuration and the [verification record](Verification.md) for executed evidence and limits.

## Automated builder and persistence scenario

From the repository root on Windows, with Python, the repository .NET SDK and MySQL 8 installed:

```powershell
python -B scripts/AuthoredCelestialSmokeWorld/native.py --smoke-script scripts/PredatorHuntingSmokeWorld/smoke.py
```

The wrapper provisions a unique loopback instance under `.artifacts/authored-celestials`, imports the maintained blank snapshot, migrates it and runs the complete medieval Debug replay. It verifies that a second replay refuses the nonblank database without changing table checksums. Its `--mysql-bin`, `--no-build` and verified `--reuse-owned` options retain their existing meanings; see the [provisioning guide](../AuthoredCelestialSmokeWorld/README.md). Do not build or change source while a test server is running.

`smoke.py` checks linked ambush and giant-venom anatomy, clones a stock AnimalAI, edits hunting and assessment through builder commands, checks the saved XML with an independent MySQL connection, then restarts the server and verifies the AI and ambush definition. It stops on the first failed assertion, an 850-second scenario deadline or a 180-second boot deadline. Both graceful shutdowns must succeed. Receipts and redacted transcripts remain in the owned run directory.

This automated scenario verifies installation, builder editing and persistence. It does not simulate the four complete fights below.

## Native content reconciliation

`Program.cs` runs the production predator content reconciler twice and compares attack identities, definitions, anatomy links, corpse mappings, breathable gases and predator approach settings. It also checks canonical air breathing, prone serpent venom attacks and the complete stock organic corpse material maps.

```powershell
dotnet build scripts/PredatorHuntingSmokeWorld/PredatorHuntingSmokeWorld.csproj -c Debug -m:1
dotnet scripts/PredatorHuntingSmokeWorld/bin/Debug/net10.0/PredatorHuntingSmokeWorld.dll <verified-owned-data-directory> <receipt-path>
```

Run this only while the owned smoke MySQL instance is running and its MUD is stopped. Supply its connection through `PREDATOR_SMOKE_CONNECTION`. The executable accepts only loopback, the disposable `authored_celestial_smoke` database and its passwordless fixture account, and verifies `@@datadir` against the supplied directory before changing content. A nonzero exit or missing PASS receipt is a failed check.

## Live gameplay scenarios

Use one operator and the skill's `mud_session.py` Telnet client against the owned instance. Construct isolated, well-lit cells with the indicated terrain and healthy prey, and attach the stock AI recommendations and matching natural attacks to the NPC fixtures. Resolve IDs from the current world; the numbers in `Verification.md` belong only to that recorded run.

For repeatable observation, the recorded fixtures used active hours, high Brawling/Wrestling/Climbing skills, a one-second engagement delay and, for the leopard, eagle and viper, zero assessment thresholds. These are test settings, not proposed stock balance. Use `impdebug wildlife hungry <id>` to invoke the real needs model. An optional `impdebug combatspeed 0.25` shortens waits; restore `1` before shutdown. Do not move hunters or prey administratively during a physical assertion. The invisible administrator may change its own observation layer.

| Scenario | Setup and action | Required observations |
| --- | --- | --- |
| Tree ambush and extraction | Leopard with the tree hunting profile and Beast Brawler; forest with GroundLevel and InTrees; small healthy prey on the ground. Make the hunter hungry and allow preparation and ordinary combat schedules. | One ambush action changes layer and performs a defended strike. Subsequent opposed grip/control actions precede a haul. Both participants reach InTrees, the hunter remains climbing, and ordinary attacks continue there. |
| Restored web and return home | Giant Spider with Beast Clincher, its web hunting AI and NaturalTrapAI; cave home linked to a retreat. Let it construct and arm its web, save and restart. Move the spider away through the normal exit and allow it to return; then walk healthy prey into the home cell. | The saved web starts armed, the spider returns through pathing, arrival consumes the trap and triggers engagement. Inspect restraint before its duration expires; an expired restraint is not evidence of failure. A successful venom bite can lead to withdrawal and a later helpless opportunity. |
| Aerial pickup and drop | Eagle with Beast Dropper, control/carry attacks and a target within full carrying capacity; terrain with multiple aerial layers. | Approach, clinch/grip and full control precede ascent. Each authored pull moves one higher layer and charges stamina once. At the ceiling the hold is released and ordinary falling occurs. Observe from the relevant layer or use the combat log; a ground observer cannot see every intermediate ascent. |
| Venom withdrawal | Viper with prone normal/clinch venom attacks; lit hunting cell linked to a retreat; healthy edible prey. | A qualifying wound delivers a positive dose. The viper disengages and shadows, then returns after observing incapacitation. Independent dose queries corroborate delivery; AI decisions must use observations rather than those queries. |

Allow up to five minutes per engagement and ten minutes for site construction and a save/restart scenario. Stop and preserve evidence on a server exception, impossible displacement, missed required state or expired deadline; do not call a successful boot or administrative relocation a gameplay pass. Remove old fixture corpses before a new acquisition check, since eating available food correctly takes precedence over hunting.

Use `impdebug wildlife <hunter-id> <prey-id>`, `effect list <target>`, `look` and `impdebug flush` for diagnostics. Query saved character layers or effects independently only after flushing. Shut down the game gracefully, stop only the owned MySQL process, and retain its cleanup receipt and transcripts.

## Automated regression suites

```powershell
.\scripts\test-unit.ps1 -OutputMode Compact -Project 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj','FutureMUDLibrary Unit Tests/FutureMUDLibrary Unit Tests.csproj','DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -ResultsRoot '.artifacts/predator-hunting/tests' -TimeoutSeconds 1800
```

The focused runtime tests cover policy and assessment, ambush permission and defence, venom receipts, pursuit observation, trap ownership and delayed capture, intent persistence, forced movement and carrying, target retention, event dispatch and trap reindexing. Seeder tests cover the 31-species roster, anatomy, stable reconciliation, compatibility repairs and preservation of custom defaults.
