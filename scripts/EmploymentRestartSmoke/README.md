# Employment approval restart acceptance

This Windows/MySQL 8 scenario provisions an owned disposable world through the existing authored-celestial native runner. It never uses `Connection.config` or a default game database. MySQL identity, UUID, data directory and port are checked before mutation. The runner shuts down only its own server and preserves evidence beneath `.artifacts/authored-celestials/`.

Build the Debug engine, DatabaseSeeder, core test project and `scripts/AuthoredCelestialSmokeWorld/AuthoredCelestialSmokeWorld.csproj` serially first. The integration test is excluded from ordinary unit runs. No sleeps, process launches or native database fixtures were added to the default unit gate.

The schema phase requires the **pre-F03** maintained blank SQL at `.artifacts/f03-snapshot-baseline.sql`. Preserve it before refreshing the source snapshot, or retrieve that exact file from the parent revision of the F03 migration commit. Do not substitute the refreshed snapshot: the test must exercise a real upgrade. This phase intentionally refreshes the maintained SQL and manifest using a separate disposable snapshot schema.

```powershell
$env:F03_SNAPSHOT_ONLY = '1'
python -X utf8 scripts/AuthoredCelestialSmokeWorld/native.py --smoke-script scripts/EmploymentRestartSmoke/smoke.py --smoke-timeout-seconds 1800
Remove-Item Env:F03_SNAPSHOT_ONLY
```

Verify `employment-schema.json` and `cleanup.json` report success in the printed run directory. The schema phase imports the old snapshot, preserves an old task through migration up/down/up, verifies model parity, refreshes the installer snapshot and imports it into another schema.

Rebuild the embedded DatabaseSeeder snapshot and run the fast gate before the final phase. Freeze source and binaries during all native phases. Resume the **same stopped owned instance**:

```powershell
python -X utf8 scripts/AuthoredCelestialSmokeWorld/native.py --restart-owned .artifacts/authored-celestials/run-<id> --no-build --smoke-script scripts/EmploymentRestartSmoke/smoke.py --smoke-timeout-seconds 1800
```

Separate test-host processes write, reload and revoke task approvals through the production persistence store. Cases include source/priority/timestamp fidelity, cumulative currency limits, recorded prior-step receipts, expiry, missing scopes, corrupt/legacy approval refusal and persistent interrupted-finance quarantine. These fixtures use a test host and recorded prior financial effects; they do not perform a native bank transfer.

Three normal engine boots then create and inspect a pending approval on a real clan, verify the complete displayed snapshot after a cold restart, revoke it and verify revocation after another cold restart. The admin Debug login uses the repository's intentional support-login behavior. No live payment is executed by this Telnet scenario. Evidence includes TRX, a redacted transcript, runtime assembly hash and `smoke-latest.json`; inspect both assertions and final cleanup rather than inferring success from a build.
