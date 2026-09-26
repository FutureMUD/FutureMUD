# Authored celestial native verification

This Windows scenario creates its own MySQL 8 instance on a random loopback port and a unique data directory under `.artifacts/authored-celestials`. It never uses the installed service, existing databases, saved logins or the MUD helper's default connection. It imports the repository blank snapshot, migrates it, runs the complete `medieval-standard` Debug replay, then proves a second replay refuses the nonblank database without changing any table checksum. The existing Debug fixture password is used only for the new disposable Admin account and is redacted from transcripts.

From the repository root, with Python, the repository .NET SDK and MySQL 8 installed:

```powershell
python -B scripts/AuthoredCelestialSmokeWorld/native.py
```

Use `--mysql-bin "C:\path\to\MySQL\bin"` for a different installation. Use `--no-build` only after building the Debug engine and this harness from the intended source. Builds are serial; do not run another build or test wrapper against the same output directories during this scenario.

An interrupted, still-running instance can be resumed explicitly with `--reuse-owned <absolute-run-directory> --no-build`. This requires the matching `@@datadir` and passing first/second replay receipts. A mismatched directory or nonblank first-run target aborts before mutation. The wrapper stops its verified MySQL instance on exit and preserves all data and evidence. It does not reset a failed database. The `smoke.py --run-dir <absolute-run-directory>` entry point runs only the Telnet assertions and stops its MUD processes, leaving the owned MySQL instance available for investigation.

## Assertions and stop conditions

The executable scenario, rather than this prose, is authoritative for command order and dynamic IDs. It stops on the first failed assertion or expired deadline:

1. Boot within 180 seconds and log the freshly seeded Admin avatar in. Freeze the clock. Create, approve and swap an outdoors cell package.
2. Create and save one object of each of the four types through `celestial new`, `set`, `validate` and `save`. Preview R1 and M1 values; query a millionth sunrise, an associated crescent and a fixed moon's explicit no-occurrence result.
3. Attach an existing physical Sun first and observe the first-eligible-authority warning. Attach the new objects, set 09:00 and assert `LOOK SKY`, `TIME`, ordinary named-object lookup, movement, phase and experienced light output.
4. Compile and execute progs through the public in-game editor for `nextsunrise`, `nextsunset`, `nextnewmoon`, `nextfullmoon`, `nextsolarlongitude`, `nextvisiblecrescent`, `nextcelestialevent`, `moonphase` and `celestialelevation`. Require actual returned values; event results must not be `Never`.
5. Export/reimport JSON, reject a bad rail save while preserving the active definition, and clone a saved object.
6. Configure a stationary below-horizon body with independent scheduled SkyVisible/BodyVisible echoes. At an ordinary minute crossing require exactly one sky echo and no body echo. Verify previews and administrative time changes are silent, windows receive `[Outside]`, shelter suppresses output, and an ordinary traversal after rewind emits once again.
7. Flush and gracefully shut down. Check the four persisted types in MySQL, restart, and assert the names, attachment, position/phase output, echo definitions and millionth named-event index survive reload. Shut down gracefully again.

Each command with an expected response has an additional bounded eight-second response wait. The complete Telnet scenario has a 900-second deadline; the provisioning wrapper allows 960 seconds including failure cleanup. Native replay has an 1,800-second limit and each build 600 seconds. Any failure produces a failed receipt, stops subsequent assertions, preserves the transcript and stops the owned MUD. A successful boot is never treated as proof of gameplay assertions.

## Evidence

The run directory contains `mysql-instance.json`, `native-replay.json`, `native-replay.log`, build/import logs, `smoke-latest.json`, a `smoke-<run>/transcript.txt`, per-run receipt, console logs and `cleanup.json`. Receipts distinguish executed assertions from unexecuted work. The full test database is retained for inspection after shutdown.

Unit tests separately cover the supplied 14 numerical fixtures, randomized geometry/crossing oracles, nonstandard clocks, negative epochs, compatible/incompatible calendars, every registered event-function overload, invalid requests, blinded/weather-obscured perception contracts, two-zone echo ordering, detach/reattach and 1,000-zone cache behavior. The native test uses an administrative avatar; it does not claim to simulate a blinded player or changed weather. See [the feature guide](../../Design%20Documents/World/Authored_Celestials.md) and [handover](../../Design%20Documents/World/Authored_Celestials_Handover.md) for verification results and limitations.

Run the three process-ownership cleanup checks without starting a server:

```powershell
python -B -m unittest discover -s scripts/AuthoredCelestialSmokeWorld -p test_cleanup.py -v
```

If an owned child's startup or client identity probe fails, cleanup terminates and waits for that exact child handle. A resumed instance with an unverifiable identity is never terminated by PID or guessed endpoint; the receipt records the failure for investigation.

## Column upgrade and dense persistence

After building this harness and DatabaseSeeder in Debug, an existing **stopped, owned** native-test world with the previous `TEXT` schema can prove the widening migration:

```powershell
python -B -u scripts/AuthoredCelestialSmokeWorld/persistence.py --run-dir .artifacts/authored-celestials/<previous-native-run>
```

The wrapper requires the earlier instance/cleanup receipts and confines the data directory to this worktree's artifact directory. It starts that instance on a fresh loopback port, checks `@@datadir`, takes a backup, and applies `WidenCelestialDefinition` through the production migration service. It also permits the immediately preceding base-branch `LandRejuvenationTreatments` migration if the fixture predates it, recording both applied IDs. It compares every existing celestial field and the stored definition byte hashes across the upgrade. Legacy XML and authored JSON must both be present.

It then saves 1,440-, 40,000- and 40,320-sample dense sources through EF, restarts MySQL, and proves exact source roundtrips and production numerical evaluation at negative, ordinary and distant times. It refreshes the maintained blank snapshot in a separate uniquely named database, and tests the production snapshot importer against another empty database. The snapshot must contain `LONGTEXT`, the complete migration history and no celestial fixture rows. Required steps stop on failure; each subprocess has a 900-second bound. The final `verification.json`, `upgrade.json`, `snapshot-import.json`, logs and cleanup receipt are saved in a new `persistence-<run>` directory. No existing MySQL service is accessed. Repeating this upgrade proof requires an old-schema fixture; it deliberately refuses an already upgraded target. Add `--backup <owned-artifact-backup.sql>` to restore a prior backup into a newly initialized isolated instance, preserving the earlier world and making the proof repeatable after its original fixture has been upgraded. The backup must be within this worktree's authored celestial artifacts.
