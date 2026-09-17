---
name: futuremud-mud-tester
description: Launch, connect to, and drive a local FutureMUD server over its telnet interface for live in-game testing. Use when Codex needs to validate FutureMUD behavior by logging into a local development MUD, running player/admin/builder commands, executing scripted command sequences, or capturing transcripts.
---

# FutureMUD MUD Tester

## Purpose

Use this skill when a FutureMUD task needs live in-game verification instead of only unit tests or code inspection. The bundled script can build and launch the local MUD with `dotnet`, connect to `127.0.0.1:4000`, log in, run command sequences, capture a redacted transcript, and stop any MUD process that it launched.

## Default Environment

- Default repo: the current FutureMUD checkout, or pass `--repo <path>` explicitly.
- Default database: `FUTUREMUD_TEST_CONNECTION_STRING` when set; otherwise the local `demo_dbo` development database.
- Default provider: `MySql.Data.MySqlClient`.
- Default account is selected from the database name: `admin` for `demo_dbo`, and `japheth` for `labmud_dbo`.
- Default account password: `password`.
- Default character selector: `1`.
- Default host/port: `127.0.0.1:4000`.

For any other database, pass `--account <name>` explicitly. The helper must not guess an account from a different development database.

Treat credentials as secrets. Do not echo passwords in status updates, logs, or final answers. The helper redacts connection-string passwords and account passwords from its own transcript.

## Launch Workflow

Prefer the script over ad hoc telnet tooling:

```powershell
python C:\Users\luker\.codex\skills\futuremud-mud-tester\scripts\mud_session.py --repo C:\path\to\FutureMUD --command look
```

By default the script:

1. Builds with `dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510`.
2. Launches with `dotnet run --project <repo>\MudSharpCore\MudSharpCore.csproj -c Debug --no-build --no-restore -- <provider> <connection-string>`.
3. Uses the ignored debug output directory as the process working directory so runtime files do not dirty the repo.
4. Starts the process without a Windows console window, disables Windows crash dialogs for that child process, and captures its combined standard output and error stream.
5. Waits for `[SUCCESS] - MUD is now ready to connect`; if boot fails or times out, returns the captured process output plus the console-log tail to Codex.
6. Connects over a raw socket and handles minimal telnet negotiation bytes.
7. Logs in, selects the configured character, runs requested commands, and prints a redacted transcript.
8. Stops only the process tree it launched unless `--keep-running` is supplied.

Use `--use-running` to connect to an already-running local MUD without building or launching. Use `--no-build` only when the current build is already known to be fresh enough for the test.

## Database Refresh

Choose the database state before launching a live test; the session helper deliberately does not refresh it automatically.

- Restore a known-good disposable database backup when the test needs an exact existing world state, repeatable fixture data, or a clean reset after a mutating command sequence. Stop any local MUD first; create a fresh backup if the current state matters; then use MySQL Workbench or the local `mysql` client to import the `.sql` backup into the selected disposable database. Pass that database's connection string with `FUTUREMUD_TEST_CONNECTION_STRING` or `--connection-string`. Never put a database password in a command line or backup script.
- Run the interactive `DatabaseSeeder` against a newly created or otherwise disposable database when the test needs a fresh stock world, a newly added seeder package, or an in-place update explicitly documented as repeatable. Read each package's rerun status: some seeders are additive or repair-capable, but foundational packages such as CoreData, Human, Combat, Item, and Animal are not broadly safe refresh mechanisms for an established database.
- Do not use `--refresh-blank-snapshot` as a test-world reset. It regenerates the bundled installer snapshot and drops/recreates the database named by `FUTUREMUD_SNAPSHOT_CONNECTION_STRING`; use it only while maintaining a DatabaseSeeder release and point it only at a disposable snapshot database.

For a fresh local seeded test world, build and run the seeder normally, then launch the MUD tester against the same database:

```powershell
dotnet build DatabaseSeeder\DatabaseSeeder.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510
dotnet run --project DatabaseSeeder\DatabaseSeeder.csproj -c Debug --no-build --no-restore
$env:FUTUREMUD_TEST_CONNECTION_STRING = 'server=localhost;port=3307;database=your_disposable_test_database;...'
python C:\Users\luker\.codex\skills\futuremud-mud-tester\scripts\mud_session.py --repo C:\path\to\FutureMUD --command look
```

Never place real database passwords in a command file, transcript, or status update. Prefer the environment variable for test connection strings; the helper redacts it from output.

## Running Test Scripts

Pass commands one at a time:

```powershell
python C:\Users\luker\.codex\skills\futuremud-mud-tester\scripts\mud_session.py --repo C:\path\to\FutureMUD --command "look" --command "time"
```

Or use a command file:

```powershell
python C:\Users\luker\.codex\skills\futuremud-mud-tester\scripts\mud_session.py --repo C:\path\to\FutureMUD --commands-file .\mud-test-commands.txt
```

Command files are plain text. Blank lines and lines beginning with `#` are ignored. Use command files for longer subsystem, admin, builder, setup, assertion, and cleanup flows.

Useful options:

- `--account`, `--password`, `--character` override the login target.
- `--skip-login` connects and runs commands without the menu login flow.
- `--expect <text>` and `--expect-regex <pattern>` fail the run if the final transcript does not contain expected output.
- `--transcript <path>` writes the redacted transcript to a file.
- `--read-after-command <seconds>` increases wait time for slow commands.
- `--startup-timeout <seconds>` increases wait time for slow boot sequences.

## Safety Notes

This skill runs against a development database but can still mutate game state. For destructive, builder, economy, admin, or data-changing command sequences, make the command script explicit and include cleanup commands where practical. Do not assume `look` is part of every test; it is just a convenient smoke command when proving connectivity.

If `dotnet run` cannot launch the app for a concrete technical reason, direct `MudSharp.exe` launch can be used as a fallback, but record why the fallback was necessary.
