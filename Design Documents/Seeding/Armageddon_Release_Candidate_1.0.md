# Armageddon 1.0 release candidate and UAT

Candidate label: **1.0.0-rc.1**, private/local distribution. Prepared from the source revision in `candidate.json`, with matching Release builds of Engine 2.17.0, Database Seeder 3.9.0 and Armageddon Seeder 1.0.0. This is an Armageddon content/package candidate, not a public stable Engine release. Project assembly versions retain their independent three-part product versions; the candidate manifest identifies the combined build.

The [13 November qualification receipt](Armageddon_Release_Candidate_1.0_Qualification_20261113.md) records the qualified binary source, actual native checks, startup times and local UAT handoff. Documentation follow-ups do not change the source pin of the issued archives.

## Scope

The candidate contains the optional Armageddon Setting and one-shot Worldfile import, including the completed item-magic admission/payment, curse cleansing, Glyph, two-stage item Shadow Passage and selective planar inventory work. Existing implemented world services, NPC/combat adaptations, spell components, summons, tools, brewing, traps, travel and mount/stable systems are available for representative UAT. Implementation does not imply that every imported mechanic has passed live acceptance.

The [item-magic handover](Armageddon_Item_Magic_Handover_20261110.md) supplies the dated local qualification. The [gap register](Armageddon_Worldfile_Remaining_Gaps_20261030.md) and its later continuations distinguish completed mappings from historical recommendations. The [V1 readiness policy](../Magic/Armageddon_V1_Readiness_20261018.md) retains representative ordinary gameplay, supported installation and normal persistence as the acceptance boundary.

## Preparation and installation

1. Use Windows x64 with the .NET 11 runtime and a supported MySQL 8 server. RC1's native packaged smoke is Windows-only; hosted Linux tests establish their recorded source coverage, not Linux packaged-world gameplay.
2. Build from a clean committed checkout with `pwsh -File scripts/Prepare-ArmageddonCandidate.ps1`. The script refuses an existing destination, publishes the three matching products, exports Engine documentation and writes archive checksums and `candidate.json`. It creates no release tag, public upload, website registration or announcement. Never mix these components with binaries from another revision.
3. Verify each archive against its `.sha256` file and extract each into its own directory. Original source/worldfiles and the generated private packet are separate inputs; they are never included in these archives or public CI artifacts. Set `FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY` to the approved external packet directory before running Armageddon Seeder.
4. Create a fresh dedicated database and use the packaged Database Seeder to create/migrate its schema, then quit its package menu. Use packaged Armageddon Seeder to install its Armageddon-compatible foundation packages in dependency order, the full Setting and then Worldfile. Do not first install incompatible generic skill/combat foundations. The normal Release console is the supported installation surface; Debug replay is development tooling.
5. The qualification world uses Classic progression, classes enabled, the existing full foundation profile, and the ordinary full Setting opt-in. Defiler admission remains available by the previously approved policy; usable Land bindings remain builder-owned. Independent policies and builder customisations retain their documented ownership rules.
6. Configure a loopback Telnet endpoint for the isolated UAT world and start the matching Engine with that world's connection. Wait for a responsive login prompt, not just the early ready message: interior population can delay the first game-loop response. Never redirect the existing walkthrough to this database or run Worldfile as an in-place refresh.

The local qualification directory contains installation/transcript receipts, external packet hashes, an instance identity, a saved UAT baseline and private test-account credentials. Those are machine-local ignored artifacts, not part of the public repository. The final qualification receipt names their paths and startup/stop procedure. Credentials must not be pasted into issue reports or public logs.

## UAT checklist

Record the candidate source/hash, character/class/grade, starting and ending resources/materials, exact commands and observed result for each case. Use an ordinary player for permission/payment acceptance; an administrator may prepare fixtures but should not substitute for that player. Tests that require a fixture or builder configuration should say so explicitly.

| ID | Representative acceptance |
| --- | --- |
| UAT-01 | Fresh chargen, class admission, starting grants, earned funding, ordinary paid casting/practice and relog/restart conserve progress. |
| UAT-02 | Custodian Glyph (Glyph) charges the shown cost, applies to the selected accessible item, discharges once with nonzero damage/stun at a nonzero grade, aborts that pickup and permits a later pickup. Cleaning/dispel and recasting use the new state. |
| UAT-03 | Both Shadow Passage item stages enforce grade/rank/mass limits and pay once. Missing/invalid components, inaccessible targets and a third stage refuse without payment. The selected target and its containment tree cannot fund their own consumptive payment. |
| UAT-04 | Real held, one/two-handed wielded, worn and nested-container equipment behaves correctly on Shadow entry/return. Ready children and dropped parents preserve custody; removed curse/magic stays removed after restart. |
| UAT-05 | Representative imported NPC aggression, pursuit/memory, melee, ranged, shields, armour, health and recovery work through ordinary encounters without a crash or unusable core loop. |
| UAT-06 | Representative component supply and graded materials, named summon/control/expiry, travel/runes, traps, brewing and specialised tools work within documented mappings. No new source supply placements or recipes are promised. |
| UAT-07 | Representative shops/services, mount purchase/control/logout, stable-ticket redemption, portable interiors and saved-world contents survive normal save/restart without duplication or loss. |
| UAT-08 | A stopped-world backup can restore into another isolated instance with its expected accounts/content; an installed Worldfile package refuses rerun without mutation. |

User UAT begins after the candidate preparation/installation checks; these rows are acceptance work, not claims of newly executed tests. The earlier native item persistence fixtures invoked imported operations directly and did not demonstrate live player-paid casting or nonzero live Glyph damage.

## Known boundaries and deferred work

- `NO_ES` belt-sheathing and one-handed skinning/tool quirks remain outside RC1's implemented mapping. Dynamically created corpse-possession/host-replacement disposal requires the explicit lifecycle prog triggers; source corpse-host replacement itself is not reconstructed. Ordinary death and unrelated body changes intentionally do not run those triggers.
- HOK 1952 and the named Tuluki robe callback rows remain in the typed-placement audit. Existing Hall departure supplies the approved room interaction; absent robe output 59019 remains inactive. No additional source callbacks are promised by this RC.
- Native combat, NPC memory/pursuit and shield rules are documented adaptations, not exact reconstruction of every source timing, probability or transient memory. Historic NPC memory is not fabricated. Residue odour perception and the existing-character staff joke remain excluded by prior decisions.
- The intentional Borrow secondary-DELETE rejection remains a recorded exceptional-failure defect (`ARM-V1-FOLLOWUP-001`). Wider injected-failure campaigns, exhaustive grade/mode matrices and further balance tuning remain deferred under the existing V1 policy. Reopen a release blocker if ordinary use causes identity/item/payment loss or corruption, or a supported core flow is unusable.
- Worldfile import and initial interior population can be slow. Record measured installation and responsive-login times. Windows Debug importer discovery was blocked by local application-control policy in tranche 5; Release importer tests were executed. No security policy workaround is part of this candidate.

## Promotion to V1.0

V1.0 requires the agreed ordinary-use UAT cases to pass, reproduced release-blocking defects to be repaired/retested, deferred deviations to remain explicit, and final source/artifact identity plus required CI/install/restart evidence to match the accepted build. A candidate is not stable merely because it builds or starts. No public publication, release tag, existing-world activation or walkthrough replacement is authorised by this preparation.
