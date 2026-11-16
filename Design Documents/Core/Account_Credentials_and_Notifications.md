# Account credentials, recovery and implementor workflows

## Passwords and compatible account upgrades

Newly selected passwords use one policy across account creation, normal changes, recovery, direct implementor replacement and first-account installation: 15 to 128 Unicode code points, allowing spaces. A small offline list rejects known common examples. It is not a comprehensive breached-password service. There are no mandatory symbol combinations or periodic password changes. Installed customised prompt text is preserved; operators whose old password-selection text says eight characters should update that wording.

New credentials are Argon2id version 19 with independent random 128-bit salts, 256-bit digests, 19 MiB memory, two iterations and one lane. The self-describing encoded hash retains its parameters. Parsing rejects malformed encodings, unsupported versions and excessive costs before allocating KDF resources. Existing salted SHA-384 credentials retain their exact historical verifier. A successful Release password login replaces the legacy representation with Argon2id; failed login does not migrate it. Old short passwords still work. Dormant accounts remain legacy until a genuine password login or replacement. No offline bulk rehash or blanket forced reset occurs.

Debug support login remains intentionally passwordless, including for already modernised credentials. It never hashes the supplied impersonation input into the account. Public Release login verifies the password. Ordinary password changes still require the actual old password; the explicit implementor tools below provide authorised assistance instead.

Expensive hashing runs in a bounded worker pool (two workers, at most 32 pending operations per world). Workers perform CPU work only. Database access, output, credential commits and controller transitions execute on the game thread. A disconnected, closed or replaced menu cannot apply a queued result. The network pauses command consumption during credential work, preserving subsequent input in its existing bounded queue instead of dropping client macro commands. The synchronous `IFuturemud.LogIn` compatibility API remains for engine callers; network Release logins use the worker route.

## Recovery and concurrent operations

Email recovery and staff-issued recovery use independent cryptographically random, case-sensitive 24-character codes. Only a versioned SHA-256 digest is stored. Codes expire after 30 minutes; requests have a one-minute per-account cooldown. Five failed code attempts exhaust a challenge. Pre-upgrade recovery hashes have no valid new expiry/version and are refused; request a fresh code.

Validating a code opens a restricted password-replacement prompt, not an authenticated account/character menu. Final commit rechecks the digest, credential version and expiry. Only successful password replacement consumes the permission. Abandoning the prompt leaves the challenge usable within its remaining limits. Issuing a newer challenge invalidates an older pending recovery prompt. A normal password change or implementor replacement invalidates all recovery permissions for the account.

`CredentialVersion`, `Password` and `RecoveryCode` participate in EF concurrency checks. Password replacement increments the credential version; routine legacy rehash does not. Concurrent replacements cannot both commit. Pending login checks the original credential snapshot again before admission. A successful replacement closes other live sessions for that account; the session performing an authenticated change may remain connected. A recovery prompt has no authenticated account session to retain.

Login attempts and old-password checks share a five-minute account attempt window. After ten attempts, a further attempt triggers one-minute backoff and starts a fresh window. Account counters and recovery state survive restart. Per-source budgets allow 100 login attempts or 20 recovery operations per five minutes, with bounded in-memory tracking. Successful login/replacement clears login failures. Source budgets reset on process restart; they supplement persistent account limits and existing network controls. Recovery request responses are generic for missing accounts, disabled transport and unavailable addresses.

## Games without email

Email remains optional infrastructure. `DisableAccountRegistrationByEmail` retains its existing meaning: while enabled, accounts satisfy the registration requirement without email verification. New accounts are persisted as registered and skip verification email. The front-end email prompt accepts `none` in this mode. The installer also accepts `none` for its implementor account. No email is invented, and password changes do not silently mark an account registered.

The implementor can use these workflows without configured mail:

| Command or setting | Result |
| --- | --- |
| `editstaticconfig DisableAccountRegistrationByEmail` | Open the existing editor; set `true` to disable the email-registration requirement. Existing configuration permissions are retained. |
| `registeraccount <account>` | Existing SeniorAdmin-or-higher command; explicitly persists registration, including while the global requirement is disabled. |
| `setpassword <account>` | Founder/implementor only. Enter and confirm a chosen password in a protected prompt. No email or old password is required. Normal new-password policy applies. Return to the implementor's character after completion/cancellation. |
| `resetpassword <account> code` | Founder/implementor only. Receive a short-lived code privately, then give it to the verified owner through a trusted channel. The owner chooses their own password using the main-menu recovery option. |
| `resetpassword <account>` or `resetpassword <account> email` | HighAdmin-or-higher command; request delivery to the account's configured email. When transport is unavailable, use the implementor paths instead. No replacement password is emailed. |

`setpassword` never accepts a password on the command line. Its prompt rechecks implementor authority before committing. Direct recovery-code output goes only to the connection and bypasses observation buffers; codes are not included in staff system messages. Account password/code entry and password-management commands are redacted from crash input context. Audit messages identify the actor/account/operation without the password or code. Operators must still establish the user's ownership before using these privileged tools.

The existing registration toggle, staff authority model, account IDs, characters, resources, suspensions and bans are preserved. Turning email verification back on does not undo explicit manual registration. A password replacement does not bypass a suspension or registration policy.

## Password-change notification compatibility

`EmailHelper.SendPasswordChangedEmail` receives only recipient address, account name and source address. MIME composition discards an old caller's third argument for `AccountPasswordChanged`. `{0}` remains the account name, `{1}` the source address, and legacy `{2}` always receives `not included in email`. Existing custom bodies/subjects/return addresses are not overwritten. The compatibility slot is retained indefinitely.

The old `AccountPasswordReset` temporary-password template is no longer called by the staff command. Staff now issue recovery codes or use the protected direct setter. For transport, retries, secret configuration and failed-message storage, see [outbound email](Outbound_Email_Delivery.md).

## Deployment and rollback

Ship generated migration `20261116104103_HardenAccountCredentialRecovery`, its designer/model snapshot, partition registry and the refreshed blank SQL/manifest together. The migration adds eight account-state columns without changing existing IDs, password strings, salts, registration flags or privileges. Legacy recovery codes become unusable under the new verifier. No seeder rerun is needed for existing worlds.

Once a password has migrated to Argon2id, an old engine binary cannot verify it. Do not keep weak duplicate legacy hashes for rollback. Restore a deliberately chosen pre-upgrade backup or use an engine version retaining the new verifier; dropping the eight columns alone is not a safe production binary rollback. Migration down/up tests use a disposable database before credential conversion.

Third-party sign-in, a browser account portal, MFA, passkeys and account-management UI are outside this tranche.

## Verification approach

Default regressions use pure account records, fake clocks/verifiers, parser boundaries and mocked output rather than EF models, real hashing, sleeps or server processes. The native acceptance scenario is opt-in Integration coverage: generated migration up/down/up, refreshed snapshot import, real MySQL concurrency, a real Argon2 round trip, deterministic worker abandonment, Release password verification, legacy migration, manual registration, direct implementor replacement, session revocation, recovery replay and Debug credential preservation. The 16 November acceptance passed 11,224 fast tests and all 55 live assertions. See the [verification record](../Verification/Account_Security_20261116.md) and [acceptance runbook](../../scripts/AccountSecuritySmoke/README.md).

## Verification on 15 November 2026

Release-mode verification passed 881 shared-library tests, 9,305 core runtime tests and 523 seeder tests. Three Unix launcher cases were intentionally skipped on Windows. The library result is from the first stable-source run; after correcting the new email/installed-world fixtures, only the two affected core/seeder suites were repeated. No library implementation changed between those runs.

Three explicit Release subprocess cases also passed: malformed connection configuration and an early startup-file failure each returned `1` and wrote a crash report; successful documentation export returned `0`. All three ran without a live database or email provider. A full world boot and graceful game-loop shutdown were not exercised.

Earlier environment bootstrap failures and the first fixture-failure run were retained in local test evidence. Final core/seeder receipt: `20261115T075150Z-14f7d352ea3a`; unchanged library receipt: `20261115T074521Z-fdf0c64bc9a1`. The subprocess filter was `FullyQualifiedName~EngineProcessExitTests`, explicitly opting into the integration category without rebuilding.
