# Armageddon v1.0 readiness scope, 18 October 2026

The v1.0 priority is a working, installable package with usable ordinary player flows and reliable normal persistence. The user's revised scope removes exceptional database fault injection and exhaustive scenario matrices from the mandatory release gates. PR #801 was merged at `0b40f0ed58e8fd53b0e1c618a75e6eabed29116d`; its reviewed-head CI passed. This decision applies to the Armageddon contribution; it does not certify every FutureMUD product or authorise a release.

A release blocker is a reproduced problem in supported installation/upgrades, ordinary gameplay, permissions, normal persistence/custody, or required build/CI checks. Keep useful existing tests and their real results. Add further testing for a specific uncovered normal-use path or actual regression.

| Essential gate | Current evidence/disposition |
| --- | --- |
| Required content and install | All 152 required spells and 210 blueprints accepted within the named direct/shared/representative contracts; four Classic/Simple x earlier Classes on/off installs passed. |
| Ordinary player flows | Real Classic and Simple Class UI/approval, exact starting grants, earned reserve, paid casting/practice and cold conservation passed within the dated receipts. Simple UI/funding passed 23 assertions; progression-fault samples passed 14 separately. |
| Normal lifetime and restart | Existing family/stock receipts cover normal expiry and custody. Borrow the Dead's paid 60-second expiry, same corpse/body/foreign gear, retired secondary and cold conservation passed 12 assertions on 15 October; its local receipt hash was rechecked, without another live campaign. |
| Supported upgrade/maintenance | Retain the completed migration/snapshot and N24 ownership/player-state conservation results. Repeating these campaigns solely to fill generic matrix labels is unnecessary. |
| Builder/player guidance and initial balance | Use the existing locked values with the documented normal-body measurements and limits. Further tuning is a follow-up unless a normal supported workflow is unusable. |
| Required builds and CI | PR #801 reviewed head passed all four hosted jobs. Merged-master run 39080875832 remains in progress at the packaging-task observation. New packaging-head checks are recorded separately; old pending labels describe their dated receipts. |

The dated [S4 acceptance](Armageddon_S4_Acceptance_Checkpoint_20261015.md), [S6 readiness receipt](Armageddon_S6_Readiness_Qualification_20261017.json), [targeted measurements](Armageddon_S6_Targeted_Measurements_20261018.json), and [machine-readable policy](Armageddon_V1_Readiness_20261018.json) record exact evidence and limits. These results are not relabelled as new executions or added into overlapping totals.

## Deferred known issues

**ARM-V1-FOLLOWUP-001: Borrow retirement after a rejected database DELETE.** The intentional exact-secondary rejection caused an unhandled persistence exception and test-server crash. The captured hold conserved the original corpse/body/gear and paid cast; alternate-use, live/repeated recovery and cold recovery were not run. The test remains FAIL. This robustness repair is explicitly nonblocking for v1.0 under the user's revised priority. Reopen it as a blocker if ordinary expiry/restart fails, field evidence shows it is common, or normal operation loses/corrupts identities, items or payment. Normal expiry and restart already have separate passing evidence.

**ARM-V1-FOLLOWUP-002: Wider exceptional-failure campaigns.** Additional provider/crash permutations, exhaustive grade/configuration combinations, repeated long timers and high-volume campaigns are deferred unless a concrete release-relevant gap justifies them. Existing failure evidence and useful assertions remain available.

**ARM-V1-FOLLOWUP-003: Further balance tuning.** Retain the current values and variant boundaries. Normal Simple characters measured attribute 12/capacity 97; Earth earned 52 at rest in 767.338 seconds and paid 50 for practice. Self50 was refused by the selected body part's damage planner; eligible fertile Land50 succeeded for seven HP plus proportional channels, one pasture and one scar. A 112.5 first-overreach quote exceeds that initial capacity and requires effective attribute 18 under the current formula (arithmetic only). Document these limits; do not invent a full-grind acceptance target or retune values during this scope revision.

## Proceeding to release preparation

The normal gameplay, installation and persistence portions of S6 are qualified within the representative evidence above. Finish final-head CI, address actual required-check regressions, and carry this checklist, the [installer/player guide](Armageddon_Installer_Integration_Guide.md) and deferred known issues into the release handoff. There is no additional injected-database-recovery or subjective balance-approval gate for this v1.0 slice.

The [release-packaging task](Armageddon_V1_Release_Packaging_20261018.md) supersedes PR #801's disabled Release/stock defaults for **new explicit full installs**. Release offers the package; default No is retained. Complete fresh policies, conditional Classes and independent gathering/device permissions are enabled; existing installations and builder edits are preserved. See the [installer guide](Armageddon_Installer_Integration_Guide.md) for activation, upgrade and rollback. This task does not activate an existing game, merge, deploy or publish a release. Historical test outcomes and exceptional-failure limitations remain unchanged.
