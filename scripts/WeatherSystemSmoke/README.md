# Native weather acceptance

Run from the repository on Windows with MySQL 8 installed:

```powershell
python -B -u scripts/WeatherSystemSmoke/native.py
```

The wrapper builds the existing authored-celestial smoke-world harness, provisions a new loopback-only MySQL instance beneath `.artifacts/weather/native-*`, refreshes and imports the repository's blank snapshot, and replays the standard seeded world. It reuses the repository MUD testing skill's Telnet helper. No configured development database, running MySQL service, or personal account is used. The ephemeral administrator password is obtained from the checked-in Debug replay fixture and redacted from evidence.

The scenario exercises:

- Public builder creation and zone assignment of a weather controller without a reboot.
- Meteorology-gated narrative/table forecasts sharing a persisted daily reading.
- Ordinary and choking dust atmosphere overrides and restoration of normal air.
- A deterministic test strike, its flash/thunder, and an electrical wound persisted in MySQL.
- Preservation of an issued forecast after hazard and clock changes.
- Cloning and reloading a rain event with its rain liquid and lightning metadata.
- Indoor recall, graceful shutdown/restart, and preservation of the future queue and private random state.

Each command has a bounded response wait. The gameplay scenario has a 900-second deadline; startup and replay have separate bounded deadlines. The wrapper stops only its owned processes and preserves the database and evidence. `verification.json`, `native-replay.json`, `smoke-latest.json`, the redacted Telnet transcript, logs and `cleanup.json` are stored in the run directory. A failed assertion is a failure, never a pass inferred from successful startup.

After a passing replay and a stopped instance, a smoke failure can be reproduced against the same owned data:

```powershell
python -B -u scripts/WeatherSystemSmoke/native.py --no-build --resume-smoke .artifacts/weather/native-<run-id>
```

Resume starts a new owned process and verifies its data directory before database access. It creates uniquely named builder fixtures and preserves the earlier smoke receipt/transcript in its original subdirectory. Remove `--no-build` when runtime source has changed. The resumed root receipt identifies that snapshot import/replay evidence came from the original run.

The automated unit suites cover protected interiors, underwater targets, loose-item targeting, ground splash, magical atmosphere precedence, low-skill interpretations, daily retry rules and pure future astronomy. The native scenario complements those checks; it is not a statistical calibration of lightning lethality or dust dosage.
