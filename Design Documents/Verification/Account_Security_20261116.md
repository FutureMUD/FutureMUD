# Account security verification

Executed 16 November 2026 on Windows with .NET SDK 11.0.100, EF CLI 10.0.15 and disposable MySQL 8.0.45. Baseline: `9cd159efa8effb08c81120d88efcda975e34e969`, plus the account-security upgrade diff. The supported contract and email-free implementor commands are in [account credentials and recovery](../Core/Account_Credentials_and_Notifications.md).

| Check | Executed result |
| --- | --- |
| Builds/model | Debug and Release engine/seeder builds passed; no pending EF model changes. Migration `20261116104103_HardenAccountCredentialRecovery` and matching designer/model snapshot generated with the matched CLI; partition/development-solution checks passed. |
| Package audit | Restored engine dependency graph reported no known vulnerable packages against the current NuGet sources, including the pinned Argon2 dependency. Audit was not suppressed. |
| Installer/replay | Complete fresh native Debug replay passed with the new first-account Argon2 writer; repeat against the nonblank world was refused without mutation. Focused installer question/replay tests: 12 passed. |
| Upgrade/rollback | Imported the pre-upgrade blank snapshot, installed an owned account with a legacy hash/code, migrated up/down/up, preserved the password and refused the obsolete code. Four integration cases passed, including controlled worker completion/abandonment. |
| Blank snapshot | Refreshed using the baseline collation, then imported into another disposable schema. Latest migration history and all eight security columns verified. Product version remains `3.9.0.0`. |
| Complete fast gate | Ten projects: **11,224 passed, zero failed, three Unix-only skips**. Counts complete; source fingerprint unchanged during the run. 350.81 seconds including builds. |
| Added default cost | Nineteen pure security-state/parser cases passed with 5.659 ms combined reported case time. The new installer policy case took 0.828 ms. Email-omission and input-deferral checks reuse lightweight existing fixtures. Real KDF/database/thread-wait campaigns remain opt-in Integration cases. |
| Native concurrency/KDF | Four integration cases passed: competing MySQL credential commits allow only one winner; actual Argon2 correct/wrong-password verification; callbacks on the caller thread; abandoned/closed menus receive no callback. |
| Live runtime | **55/55 assertions passed** across three Release boots and one Debug boot. Verified queued login input, genuine legacy short-password rehash, authenticated self-change retaining its session, Founder direct setting without email, return to the original character, target-session closure, wrong-password refusal, manual registration, private recovery-code delivery, recovery commit/replay refusal, and persistence after restart. |
| Email-free mode | Front-end/installer accept an omitted address under their documented policies. Live manual registration persists both with the email-registration requirement enabled and disabled. A previously unregistered, email-free account can use the account menu when the requirement is disabled. |
| Intentional Debug login | Arbitrary support impersonation input admitted the existing account while its stored password checksum and credential version remained unchanged. |
| Cleanup | Owned MySQL identity verified before mutation; server stopped successfully. Disposable data, redacted transcripts and receipts preserved. |

Fast-gate run: `20261116T111308Z-6c4c5723f9c6`; source fingerprint `bb304b2ffc21506b721da61d3beb5800e760bcd408c6353b1a8e989bec2f5f57`. Local receipt directory: `.artifacts/f05-fastgate-final/`.

Native run: `.artifacts/authored-celestials/run-020d76031392/`; schema scenario `accounts-10af668e`; final runtime scenario `accounts-c8794e1a`. Final runtime SHA-256 values:

- Debug: `2560dcdd27261cf8d49bbd20b4d2ea8ca7f90b7c169ed083c2ee7ebba549edd1`
- Release: `6dca669804daea0c60b6f7ed800803e3be00cfd59ca62c5c2167c48f154b1af3`

The first live attempt successfully performed login/registration but its script compared MySQL BIT output to decimal text. Its failure receipt/transcript (`accounts-9cc83cfd`) was preserved. The query was corrected to read the numeric value, only the two verified synthetic accounts without characters were reset, and the complete live scenario passed. Runtime/default-test code and binaries were unchanged after the successful fast gate; subsequent edits were acceptance-script correction and documentation.

No outbound email provider was exercised. Notification MIME compatibility uses fake-message tests; live recovery used the authorised implementor code path with empty account email addresses. No production world, third-party identity provider, MFA, passkey or account portal was used. The migration down/up check precedes credential conversion: rolling back an old binary after Argon2 conversion requires the documented compatible-verifier or backup strategy.

See [the runbook](../../scripts/AccountSecuritySmoke/README.md) for reproducible commands and owned-fixture safeguards.
