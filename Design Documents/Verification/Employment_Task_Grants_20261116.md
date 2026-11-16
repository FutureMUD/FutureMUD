# Employment task approval persistence verification

Executed 16 November 2026 on Windows, .NET SDK 11.0.100 and disposable MySQL 8.0.45. Base revision: `874b8f3d9f74b8d13db7cdf320057b84c5c46621`, plus the F03 implementation diff. This record qualifies the [durable task approval contract](../Economy/Unified%20Employment%20Dispatch%20Design.md#durable-task-approvals-and-restart-recovery-f03-november-2026), not the entire employment roadmap.

| Check | Result |
| --- | --- |
| EF generation/parity | Migration `20261116022849_PersistEmploymentTaskAuthorisation` generated with EF CLI 10.0.15; no pending model changes. |
| Upgrade/rollback | Imported the pre-F03 maintained snapshot, inserted an old task, migrated up/down/up. The task survived and received no inferred approval. Native test passed. |
| Installer snapshot | Full refresh to the new migration, preserving the baseline collation; separate SQL import verified all three grant tables and latest migration history. Product version remains `3.9.0.0`. |
| Complete fast gate | 10 projects; 11,196 passed, zero failed, three platform-specific skips; 298 seconds including builds. Source fingerprint unchanged throughout the run. Includes 14 passing blank-snapshot cases. |
| Added default tests | 17 pure snapshot, authority, cached-context and financial-fault cases; 26 ms combined reported case time in the complete gate. Standalone 17-case execution took 254 ms. No EF model construction, database, processes or sleeps. |
| Native process reload | Three fresh test-host processes passed write, read and revoked phases using the production persistence store and MySQL. Verified manual/rule/goal attribution, priority, due date, prior-step receipt preservation, cumulative currency limits, expiry, corrupt/legacy/missing-scope refusal, revocation and sticky interrupted-finance quarantine. |
| Normal engine restart | Three ordinary Debug MUD boots. Created a pending task on a real clan, flushed saves, stopped the process, compared the displayed approval/provenance after a cold boot, revoked approval, flushed and verified the same revocation after another cold boot. All 28 scenario assertions passed. |
| Cleanup | Owned server UUID/datadir/port verified before mutation; MySQL stopped successfully; disposable data and redacted receipts/transcript preserved. Separate migration-design server also stopped. |

Fast-gate run: `20261116T030214Z-d8bedb7a54f5`. Source fingerprint: `5b0d4234afeee7f020c53f75aa39520bfc4343de5bebec20930ea85c245193c4`. Runtime SHA-256 used for final engine acceptance: `a102b2f58587b301cef15f2dd9865baf40e95049812d12658483cb69ce816e78`.

Local ignored evidence is under `.artifacts/f03-fastgate-final/` and `.artifacts/authored-celestials/run-557d942bc942/`. Final schema scenario: `employment-7fa22179`; final store/engine scenario: `employment-638712c3`. Reproduction commands and fixture safeguards are in the [runbook](../../scripts/EmploymentRestartSmoke/README.md).

The native store fixtures use a test host and recorded prior-step receipts; they do not execute a live bank transfer. The normal-engine scenario verifies a pending real-clan approval and revocation, without assigning or executing its payment. Native payment atomicity, autonomous reconciliation, manager-policy persistence, host-wide budgets and route-pair policy remain outside F03. Game-clock timestamps are preserved exactly; the roomless clan fixture uses the existing zero game-clock epoch.

An initial native harness build failed because its moved Armageddon installer dependency was missing; the harness reference was repaired before successful runs. The first snapshot refresh exposed default-collation drift from the disposable server; the scoped refresh preserved the original collation and passed import. An interrupted initial fast-gate attempt is retained without a passing claim. Final acceptance above uses the corrected snapshot and stable inputs.
