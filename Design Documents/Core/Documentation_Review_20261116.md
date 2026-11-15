# Documentation Suite Review, 16 November 2026

## Scope and source baseline

The suite inventory covered 658 tracked Markdown documents (531 under `Design Documents/`) and their navigation/reference roles, against remote `master` commit `874b8f3d9f74b8d13db7cdf320057b84c5c46621`. The earlier credential and MudClient CI changes were separate open draft PRs at this baseline; this documentation branch does not silently merge their runtime/workflow changes.

Reviewers classified the Magic/Armageddon, Seeding/Verification and other runtime documentation groups, with coordinator review of Core/Economy/Health/Items/Crafting and product/operator entry points. Source checks concentrated on deletion/consolidation decisions, contradictory status claims, persisted behavior and moved-source references. Published news, original corpora, catalogue payloads and third-party documentation were identified by role and retained. This is not a fresh line-by-line behavioral certification of every catalogue entry or runtime assertion in every document.

## Consolidation and navigation

- Retired 20 completed plans/task handoffs after transferring their enduring requirements and outstanding qualification limits into the owning guides.
- Moved/renamed 128 documents: setting-specific magic now lives under `Armageddon/Magic/`, vehicles under `World/`, and implemented master guides use descriptive system names. Employment no longer uses space-separated design/roadmap filenames.
- Added a repository entry point, thematic indexes and [documentation roles/naming guidance](../Documentation_Guide.md). Existing catalogue/resource identities remain compatibility exceptions.
- Repaired broken current source/document links, and labelled ignored local evidence paths as unversioned artifacts rather than repository downloads. Historical pinned commit links retain their original layouts.
- Converted the legacy design index and getting-started page to UTF-8. Runtime/research/catalogue data assets remain byte-identical; only two retained Python validators changed their obsolete handoff-document checks.

## Substantive corrections

| Area | Corrected supported contract |
| --- | --- |
| Employment | Task grants/provenance, priority and lifecycle timestamps are runtime-only. Reload uses empty compatibility grants and financial work fails closed. Separate payroll/ledger persistence remains intact. Retrying a restored task does not recreate approval. |
| Arenas | The lifecycle and operational services exist. Fixed odds have a fixed 5% house edge; periodic Economic Zone tax/P&L settlement and automatic class signature-item provisioning are not delivered contracts. Backfill may leave underfilled starts. |
| Time/calendar | All six algorithm families exist, but solar-equinoctial/lunar/lunisolar naming does not imply ephemeris-driven boundaries. Mean-cycle approximations and deferred precision/observation ledgers are explicit. |
| Magic | Classic/full prompts consume magic visibility settings. Land gathering succeeds the earlier organic-yield slice; old verification statements remain historical. Cascade acquisition semantics are retained in the casting master. |
| Armageddon | Fury/Calm's named stock/paid gate is complete; the wider optional catalogue/release programme remains incomplete and disabled. ARM-01 owner decisions, ordered-NPC closure evidence and broader programme requirements are retained. |
| Room building/markup | `room` is canonical and `cell` is its retained alias. Markup references the live Room implementation rather than removed Cell source/class labels. |
| Testing | The maintained manifest and targeted wrappers replace old default-gate counts/whole-solution prerequisites. Historical July counts are kept as history. Git evidence timeout/drain/source-stability contracts move out of the completed repair brief. |

## Completed planning documents and their owners

The source paths below identify deleted planning records for Git-history lookup. Follow the owner link for the retained current contract; the old plan is not required to operate the feature.

| Retired planning record | Maintained owner |
| --- | --- |
| `Design Documents/AI/AI_Storyteller_Implementation_Plan.md` | [AI Storyteller System](../AI/AI_Storyteller_System.md) |
| `Design Documents/AI/Monster_AI_V1_Proposal.md` | [Monster AI](../AI/Monster_AI.md) |
| `Design Documents/Combat/Combat_Arenas_Implementation_Plan.md` | [Combat Arenas System](../Combat/Combat_Arenas_System.md) |
| `Design Documents/Combat/Dual_Wield_Double_Attacks_Plan.md` | [Dual Wield Double Attacks](../Combat/Dual_Wield_Double_Attacks.md) |
| `Design Documents/Combat/Natural_Ranged_Attacks_Implementation_Plan.md` | [Natural Ranged Attacks System](../Combat/Natural_Ranged_Attacks_System.md) |
| `Design Documents/Core/TestReporting_Git_Process_Fix_Brief.md` | [Unit Test Suite Quality and Maintenance](Unit_Test_Suite_Quality_and_Maintenance.md) |
| `Design Documents/Magic/ARM02_C1_Acquisition_Cascade_Fix_Brief.md` | [Configurable Casting](../Magic/Configurable_Casting.md) |
| `Design Documents/Magic/ARM02_Configurable_Casting_Implementation_Brief.md` | [Configurable Casting](../Magic/Configurable_Casting.md) |
| `Design Documents/Magic/Armageddon_EmotionalRuntime_Hook_Contract.md` | [Emotional Combat Hooks](../Armageddon/Magic/Emotional_Combat_Hooks.md) |
| `Design Documents/Magic/Armageddon_Emotional_Casting_Hook_Proposal.md` | [Emotional Combat Hooks](../Armageddon/Magic/Emotional_Combat_Hooks.md) |
| `Design Documents/Magic/Armageddon_Fury_Calm_Final_Integration_Plan.md` | [Fury Calm Runtime](../Armageddon/Magic/Fury_Calm_Runtime.md) |
| `Design Documents/Magic/Armageddon_See_The_Unbodied_Component_Proposal.md` | [See The Unbodied Component Integration](../Armageddon/Magic/See_The_Unbodied_Component_Integration.md) |
| `Design Documents/Magic/Armageddon_Water_Breathing_Adapter_Ownership_Proposal.md` | [Water Breathing Adapter Integration](../Armageddon/Magic/Water_Breathing_Adapter_Integration.md) |
| `Design Documents/Magic/Configurable_Casting_Handover.md` | [Configurable Casting](../Magic/Configurable_Casting.md) |
| `Design Documents/Seeding/CultureSeederRedesignHandoff/02_IMPLEMENTATION_BRIEF.md` | [CultureSeeder](../Seeding/Seeders/CultureSeeder.md) |
| `Design Documents/Seeding/CultureSeederRedesignHandoff/AGENT_TASK.md` | [CultureSeeder](../Seeding/Seeders/CultureSeeder.md) |
| `Design Documents/Seeding/CultureSeederRound2Handoff/02_IMPLEMENTATION_BRIEF.md` | [CultureSeeder](../Seeding/Seeders/CultureSeeder.md) |
| `Design Documents/Seeding/CultureSeederRound2Handoff/AGENT_TASK.md` | [CultureSeeder](../Seeding/Seeders/CultureSeeder.md) |
| `Design Documents/Seeding/ItemSeeder_Unified_Catalogue_Implementation_Proposal.md` | [ItemSeeder](../Seeding/Seeders/ItemSeeder.md) |
| `Design Documents/World/Astronomical_Time_Calendar_Refactor.md` | [Time And Date System](../World/Time_And_Date_System.md) |

## Retained unfinished work and evidence limits

The employment roadmap, ARM-01 proposals and broad Armageddon programme brief remain live design. The large body-instance architecture retains its V1/advanced-policy distinction. Higher-precision calendars, arena finance/provisioning extensions and full storyteller live-loop reliability remain outstanding; deleting phased plans did not implement them.

Seeder handoff directories retain embedded JSON, source catalogues and provenance. ItemSeeder completion/stage receipts and native acceptance records preserve their stated revisions, failed/unexecuted checks and local-artifact limitations. Historical release readiness is not present-day release approval.

Validation covers link/path/index consistency, encoding, preserved data assets, Python syntax and the retained catalogue validators on disposable copies. No engine gameplay, fresh MySQL installation, SMTP, live Discord/model session, browser deployment or release qualification was executed by this documentation consolidation. Final local checks covered 663 current Markdown documents, 535 design documents, 3,652 local links, heading anchors, index reachability and UTF-8 with no unresolved errors. All 473 pre-existing non-Markdown design files were hashed: 471 were unchanged and only the two retained validators were adjusted. The redesign validator passed 4,052 checks; round-two passed 6,949 data checks and 11 retained data/research checksums. A modified research payload was rejected in a negative checksum check. Both validators ran on disposable copies; tracked validation reports/manifests were not rewritten. Independent focused source/document review found no actionable defect, with its whole-suite/runtime verification limits retained.
