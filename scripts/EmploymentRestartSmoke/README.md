# Employment provider restart smoke

This is an opt-in real-provider scenario. Run it only through the existing owned disposable-world harness, after the Debug test binaries have been built:

```powershell
python -X utf8 scripts/AuthoredCelestialSmokeWorld/native.py --no-build --smoke-script scripts/EmploymentRestartSmoke/smoke.py --smoke-timeout-seconds 1800
```

For a focused rerun, the wrapper also accepts `--restart-owned <prior-run-directory>` after that instance has a passing initial replay and verified shutdown. It verifies the preserved server identity before restarting it. Each employment attempt uses a new evidence directory, bank accounts and host identity, so its assertions do not rely on an earlier attempt's task state. This avoids repeating the full stock installation for a recovery-policy regression.

The wrapper imports the initial blank snapshot and seeds its authored celestial smoke world into a newly initialized, loopback-only MySQL instance under `.artifacts/authored-celestials`. The scenario validates the instance's recorded data directory, server UUID, loopback endpoint and port against both `auto.cnf` and live server identity. Before the employment phases, it preserves the current snapshot SQL and manifest under the run directory, then runs the already-built Debug DatabaseSeeder refresh tool against a fresh, unused schema on that same owned instance. It imports the refreshed SQL into a second fresh schema and checks that the imported migration history matches the manifest and that the three employment approval/provenance columns exist. This refresh intentionally rewrites `DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql` and `BlankDatabaseSnapshot.manifest.json`; the pre-refresh copies are retained as chronology evidence.

During snapshot generation only, the scenario sets the verified private instance's server default collation to the maintained snapshot's `utf8mb4_0900_ai_ci`, then restores its prior value in `finally`. The schema assertion checks this default. This preserves existing table defaults rather than carrying the common harness's `unicode_ci` default into the product snapshot. No shared MySQL instance is eligible for this operation.

The scenario then invokes the same filtered native test in two separate `dotnet test` processes, sequentially and against the seeded `authored_celestial_smoke` database. The `write` phase is expected to apply the current EF migrations and exercise a real Down/Up path with legacy employment data; the `read` phase checks persisted state after the first test host has exited. Both phases receive `FUTUREMUD_EMPLOYMENT_NATIVE_ARTIFACTS` pointing to the owned run directory. Each phase must produce a passing test and zero failures, skips, inconclusive or other non-executed results.

The two test-host processes demonstrate a real-provider persistence boundary and process restart. They do not demonstrate server boot behavior, player-facing hiring flows, or live Telnet behavior. A successful native result is not a substitute for a separate live gameplay smoke when that is in scope.

The wrapper owns MySQL startup and shutdown; the script does not start or stop MySQL. It creates only the two verified-absent employment snapshot schemas and imports the refreshed dump into the import schema. Evidence is preserved under a fresh `employment-attempt-<id>/` directory within the owned run: `employment-restart.json` contains status, named assertions, snapshot refresh/import and per-phase timings; `snapshot-before-refresh/` preserves the former SQL and manifest; the two snapshot operation logs and `employment-test-results/` contain diagnostic evidence and TRX files. Failed test hosts also get phase logs. The root `employment-restart.json` and the wrapper-required `smoke-latest.json` are summaries of the latest attempt. The refresh and native connection strings are supplied only in child-process environments and are not written to the receipt or logs by this script.

Keep this as one consolidated opt-in extended case because it requires a real MySQL instance and two test-host startups. The ordinary fast suite should not pay that provider/restart cost, and maintaining a second full-stock installation solely for this boundary would duplicate the wrapper's provisioning and cleanup work. The test should remain opt-in via the existing extended-test setting and focused filter.

The test implementation is owned by `MudSharpCore Unit Tests`; its test name must match `EmploymentRestartNative_` and branch on `FUTUREMUD_EMPLOYMENT_NATIVE_PHASE` (`write` or `read`). The script expects these environment variables:

- `FUTUREMUD_EMPLOYMENT_NATIVE_CONNECTION`
- `FUTUREMUD_EMPLOYMENT_NATIVE_DATADIR`
- `FUTUREMUD_EMPLOYMENT_NATIVE_UUID`
- `FUTUREMUD_EMPLOYMENT_NATIVE_PORT`
- `FUTUREMUD_EMPLOYMENT_NATIVE_PHASE`
- `FUTUREMUD_EMPLOYMENT_NATIVE_ARTIFACTS` (current owned `employment-attempt-<id>/` directory, for migration and fixture metadata)

`FUTUREMUD_EMPLOYMENT_BASELINE_SNAPSHOT` is not set by this scenario. If the test needs the pre-refresh blank snapshot baseline for an upgrade/import assertion, coordinate that setup with the owning test and wrapper rather than deriving or mutating a separate schema here.

## Live Telnet acceptance

`live.py` is a separate, adaptive acceptance operator. It boots the actual Debug MUD, logs in with the disposable world's bootstrap administrator, and drives the shared employment commands over Telnet using the repository's MUD-session helper. It is opt-in and has no place in the default unit gate. A passing receipt certifies only its named completed scenarios and assertions; it does not imply that the entire matrix below ran automatically.

Build current Debug engine binaries before launch. On restricted Windows hosts, disable CLI telemetry to avoid registry access during `dotnet run`:

```powershell
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
python -B -X utf8 scripts/AuthoredCelestialSmokeWorld/native.py --no-build --smoke-script scripts/EmploymentRestartSmoke/live.py --smoke-timeout-seconds 3600
```

Use `--restart-owned <run-directory>` to resume a preserved, verified disposable world. This live operator deliberately retains the same tasks and native bank records across invocations. It does not refresh the product snapshot. The wrapper owns MySQL and requires a passing initial seeder replay and verified previous cleanup before restarting an owned instance.

The run's `employment-live-runtime.txt` points to a new `employment-live-<id>/` directory. Append one JSON object per line to its `requests.jsonl`, then wait for the matching numbered `done` response in `responses.jsonl` before issuing dependent requests. Each request is processed once. Examples:

```json
{"commands":[{"text":"shop tasks approval show Example","expect":"Grant:"}]}
{"sql":"SELECT COUNT(*) FROM BankAccountTransactions","expect":"^[0-9]+$","label":"native transaction evidence"}
{"restart":true}
{"completedScenario":"named scenario and its evidence-backed outcome","finish":true}
```

Commands accept `expect`, `forbidden`, `label` and bounded `seconds` waits. SQL is read-only while live. `stop` performs a flush and graceful shutdown; `start` boots the same database; `crash` kills only the MUD process tree launched by this operator. `fixtureSql` is accepted only while that MUD is stopped, after rechecking the private MySQL identity, and only for narrowly named `QA_EMPLOYMENT_` crash triggers or employment-task compatibility fixtures. The operator has a 55-minute deadline; finish a named phase before that limit and resume the owned world for another phase if necessary. Assertions fail the run, preserve the error, and trigger cleanup. An empty `finish` without a named scenario and live assertions cannot pass.

Acceptance matrix:

1. Author a real shop, linked bank account and hired task-enabled NPC. Use small, distinct transfer amounts per task. Record original balances, task/grant identities, limits/scopes and native transaction counts.
2. Exercise exact-ID/revision validation, audit reasons, expiry parsing, revocation and replacement. Reject a forced ordinary employee's mutation without `AssignTasks`, and verify the stored grant is unchanged. Revoke an NPC issuer's assignment authority and prove its task cannot execute.
3. Stop the MUD to prepare a null-provenance legacy row and unsupported-version row. Restart; legacy work requires explicit bounded replacement, unsupported evidence stays quarantined and retained, and neither executes silently. Expiry must still apply with game time frozen.
4. Interrupt one linked-bank transfer after native commit and before its completed-step checkpoint. A temporary task-specific `BEFORE INSERT` trigger on `EmploymentActiveTaskStepStates` can pause that exact completed-state insert. Poll native balances/transaction records and the persisted `InProgress` step, then crash the owned MUD. Drop the trigger offline. Restart; retry/requeue and approval replacement must not clear the hold. Reconcile `executed` against native evidence and prove there is no second payment.
5. Interrupt another transfer before native payment, after the `InProgress` checkpoint. A temporary correlation-specific trigger on the `Started AccountTransfer action.` register insert can pause this boundary. Poll for zero native payment records and the persisted checkpoint, then crash. Drop the trigger offline and restart. Reconcile `not-executed`; the task must remain blocked with `RequiresExplicitRetry`, through worker assignment auditing and another server restart. Reject an unauthorised or reasonless retry. Only an explicit authorised retry/requeue may resume it, producing exactly one payment.
6. Restart again. Verify completed step states, grant revisions, reconciliation/retry register evidence, original native transaction counts and final balances. Verify all temporary triggers are gone. Finish, then inspect the wrapper's `cleanup.json` and invocation receipt to prove both owned MUD and MySQL stopped.

Keep these pauses confined to the identified task/correlation in a private disposable instance. They model the two crash windows; they are not production migrations. Read-only evidence must establish the actual native outcome before using a reconciliation command. Do not modify runtime source or rebuild binaries while a MUD is running. Boot receipts record the engine binary SHA-256 so changes between phases are visible.

Each phase preserves redacted command transcripts, requests/responses, server output, assertions, boot hashes and shutdown records in `receipt.json`. The wrapper also preserves an invocation receipt and database cleanup evidence. Earlier failed setup/assertion attempts remain available; a later pass does not erase their failures. See the [dated recovery guide](../../Design%20Documents/Economy/Employment_Restart_and_Recovery.md) for the executed scope and corrections found during qualification.
