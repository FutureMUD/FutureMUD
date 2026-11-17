# Account security acceptance

Run on Windows with MySQL 8 through the owned-instance runner. No default game connection is used. The wrapper provisions a fresh disposable world, verifies UUID/datadir/port, performs the complete Debug seeder replay and nonblank refusal check, then invokes this scenario. It stops only its own server and preserves redacted evidence under `.artifacts/authored-celestials/`.

Before snapshot refresh, preserve the pre-F05 maintained SQL at `.artifacts/f05-snapshot-baseline.sql` (retrieve it from the parent revision for a later rerun). Build the core unit-test project, Debug engine/seeder/native harness and Release engine/seeder serially. The scenario's native tests are Integration cases excluded from the normal fast gate.

```powershell
$env:F05_SCHEMA_ONLY = '1'
python -X utf8 scripts/AuthoredCelestialSmokeWorld/native.py --smoke-script scripts/AccountSecuritySmoke/smoke.py --smoke-timeout-seconds 1800
Remove-Item Env:F05_SCHEMA_ONLY
```

Inspect `account-schema.json` and `cleanup.json`. This phase imports the old snapshot into `account_upgrade`, copies an owned synthetic account fixture, verifies migration up/down/up without changing its legacy password, refreshes the maintained blank SQL with the original collation, and imports that SQL into `account_snapshot_import`. The latest migration and all eight security columns must be present. These dedicated schemas are never game databases.

Rebuild the embedded snapshot, run the full fast gate, and freeze binaries before resuming the same stopped owned instance:

```powershell
python -X utf8 scripts/AuthoredCelestialSmokeWorld/native.py --restart-owned .artifacts/authored-celestials/run-<id> --no-build --smoke-script scripts/AccountSecuritySmoke/smoke.py --smoke-timeout-seconds 1800
```

The final phase checks two competing MySQL credential commits, a real Argon2 round trip and controlled worker completion after cancellation/disconnect. Three Release boots exercise manual registration, legacy short-password rehash, Founder direct replacement without email, return to the character, target-session closure, wrong-password refusal, manual recovery code delivery, recovery commit/replay, restart, and disabled email-registration requirements. A final Debug boot verifies that intentional support impersonation does not overwrite credentials.

The skill's generic launcher selects Debug, so this scenario explicitly selects the built Release/Debug configurations using the same readiness collector and owned-process cleanup helper. Test passwords/codes are synthetic and redacted from transcripts. Do not print or commit connection strings, challenge values or passwords. Read final receipts and cleanup, not only process exit codes. No outbound mail provider is required or exercised; email MIME compatibility is covered by fake-message tests.
