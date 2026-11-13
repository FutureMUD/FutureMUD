# Armageddon 1.0.0-rc.1 qualification — 13 November 2026

Private Windows x64 candidate built from **`7d0c447c086629a000e3cbca6d3b22738d0f5c84`**, including master `b2836f79c` and schema migration `20261113003430_AddStagedPerformanceRecords`. [Draft PR #866](https://github.com/FutureMUD/FutureMUD/pull/866) remains open for user acceptance. This receipt follows the binary build as documentation; it does not change or relabel its source revision.

The [scope and UAT checklist](Armageddon_Release_Candidate_1.0.md) defines the candidate. No stable release tag, public binary upload, website registration, existing-world activation or walkthrough replacement was performed.

## Exact-source gates

| Gate | Observed result |
| --- | --- |
| Hosted Unit Tests | Windows and Linux passed on the candidate head; run `43379859045`. |
| Hosted Armageddon Seeder Tests | Windows and Linux passed; run `43379858933`. Private-corpus absence in public CI remains an explicit coverage boundary. |
| Hosted Item Seeder Tests | Windows and Linux passed; run `43379858955`. |
| Local six-project Release fast pass | **11,465 passed, zero failed, four expected skips**, stable source; `20261113T065640Z-a50906c07dcb`. The skips are three Unix launcher cases on Windows and the missing-corpus refusal case while the real corpus is configured. That absence contract has its separate passing tranche-5 receipt. |
| Local Release extended integration | **17 passed, zero failed/skipped**, stable source; `20261113T071230Z-27a2cf30791c`. Includes complete-setting, progression/class, rerun and snapshot-isolation cases. |
| Package execution | Matching Engine 2.17.0, Database Seeder 3.9.0 and Armageddon Seeder 1.0.0 Release single-file archives built. Packaged Engine exported all five documentation families with the exact source revision. |

Local post-publication test builds used `DebugType=embedded`, matching the migration-helper symbol format of the packaged build. No assertion, test category or security policy was weakened. Earlier overlapping runs in the handover are dated evidence and are not added to these counts.

## Clean packaged installation and persistence

The actual packaged Release Database Seeder created the schema in a new, empty, isolated MySQL instance. The actual packaged Armageddon Seeder then completed **all 32 foundation/Setting/Worldfile packages through its ordinary interactive console**. The small metadata helper resolved questions and profile answers read-only; it did not execute Debug replay or substitute a direct service call for installation.

The Worldfile step took **2,593.45 seconds**, approximately 43 minutes. Independent verification passed **14 assertions**: complete package order, no Release seeder failure messages, committed `State=Complete` marker, current migration, administrator avatar, full content, both owned item-magic spell definitions and unchanged external packet hashes. At completion the database contained 26,316 rooms, 15,112 item prototypes, 7,037 initial items, 166 spells and one character. These are measured receipts after all packages, not new catalogue-count regression contracts.

The imported world fingerprint is `502f51b5cb06563c2a1095671d7cc8c1314896055c1ab2bd7df734c0157cb285`. Original source/worldfiles and the packet remain external and are excluded from all candidate archives and public CI artifacts.

An actual second packaged Worldfile installation was attempted and refused. All **600 table row digests remained unchanged** (`b5c3135b9b45732eb2202d37d7dc5ab0a3747a1318f87f694c15d1b92a573ed5`).

Two actual extracted packaged Engine boots passed **38 assertions**, including responsive account/character login, actual score output, saved Custodian Glyph and Shadow Passage definitions, queued-save flush and graceful shutdown. Both processes exited zero; captured logs showed no unhandled/fatal/save-failure matches. The optional Discord connection warning remains an environment detail.

| Boot | Early ready | Responsive login | Exit |
| --- | --- | --- | --- |
| First cold world boot | 482.49 s | 514.32 s | 0 |
| Saved-world restart | 215.67 s | 221.80 s | 0 |

The Engine executable SHA-256 is `72e8fcad15037a7013e43daada2cb484f64b0a1a4e45897e067f44e7aea3c9b7`. Early readiness alone was not treated as a successful login. The startup times above are practical limits for UAT, not a general load or performance certificate.

A stopped-world baseline dump was restored into a second disposable database. Independent completion verified all **600 table row digests** against the unchanged original, plus the current migration. Baseline size: 548,445,243 bytes; SHA-256 `1a6ffe9dec966d3cddfed4e826c664b0af806efe32e3895c1f89d89bf9fac6f5`. The restored copy was checked at SQL/schema-history level; a separate restored-copy gameplay campaign was not run.

## UAT handoff

The local artifacts are beneath `.artifacts/armageddon-rc1-smoke/`; the code-only candidate is beneath `.artifacts/armageddon-candidates-qualified/1.0.0-rc.1-win-x64-7d0c447c0866/`. `candidate.json` pins all three archive hashes, runtime, framework and source; `README-UAT.md` uses immutable source-document links.

`Manage-UAT.ps1 -Action Start|Stop` is the local instance launcher. It checks the recorded data-directory/server identity, starts hidden owned processes, waits for a real login greeting, and uses authenticated graceful shutdown before stopping MySQL. **Keep the Start console open; run Stop from a second console.** The supervised launcher round trip passed; independent process checks and MySQL's normal-shutdown log confirmed the owned services stopped. The endpoint and private test-account credentials are in local ignored files; no password is published in this receipt. The baseline and restore copy remain local. The reset snapshot precedes the launcher demonstration; subsequent normal world activity is not represented as a new baseline capture.

## Preserved failures and acceptance boundary

Non-passing setup attempts remain in local receipts: an overlapping publish was stopped and its partial artifacts retained; affected migration symbol outputs and the read-only metadata helper were rebuilt sequentially before successful packaging. The verifier's Python safe-path import and shorthand `Glyph` assumption were corrected: the actual canonical spell is **Custodian Glyph**. The first dump utility invocation used an unsupported client option; a later incomplete CLI identity response interrupted comparison after restore. Independent batched read-only verification subsequently proved the completed restore. An initial detached-launch test lost its child processes when the tool invocation ended; the launcher was changed to retain a foreground supervisor and then passed authenticated graceful stop. These attempts are not counted as passing checks or product defects.

The existing walkthrough instance and database were never used, restarted or changed. This qualification does not claim live ordinary-player paid casting, nonzero live Glyph damage, the full equipment-command matrix, exhaustive balance/fault testing or Linux packaged-world gameplay. Those ordinary-use acceptance cases remain in the user's UAT checklist. The prior native 14-specimen item-magic persistence qualification retains its dated scope. Known `NO_ES`, dynamic host-replacement/callback and exceptional Borrow-failure boundaries remain explicit in the scope document.
