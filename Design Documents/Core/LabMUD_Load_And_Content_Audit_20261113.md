# LabMUD startup and loaded-content reuse audit — 13 November 2026

## Scope and conclusions

This follows the [startup implementation investigation](Startup_Performance_Investigation_20261113.md). The requested scope is a LabMUD startup comparison and another review of loaded content for duplicate definitions and common-template opportunities. Armageddon is the main content target, with LabMUD as a comparison. Source worlds remain unchanged; the proposed consolidation retains their identity and provenance requirements.

There is substantial generated-definition repetition. In the Armageddon test world, **18,221 FutureProg rows contain 5,685 distinct exact payloads**. The **12,536 excess rows** are a sharing ceiling, not a deletion list: names, ownership, later installer mutations and clone-time placeholder contracts can require separate identities despite identical code. Definition reuse and accidental repeated event execution are different questions; no duplicate hook bindings were found.

## LabMUD benchmark method

The source is `labmud_dbo` on the existing local MySQL service, port 3307. It was read and exported with `mysqldump --single-transaction --quick --skip-lock-tables --no-tablespaces`; the source server was not stopped, upgraded or written. The 135,000,624-byte snapshot has SHA-256 `266063656c82a5d1b398d15bb01356a45ea413c1528661cc06c24cdda4e537c4`. It is a transactional snapshot of a potentially active world, not a claim that the source was stopped.

The source snapshot had 5,811 character rows, 5,323 NPC rows, 1,397 rooms, 17,355 items, 518 progs, 12 hooks and 146 NPC template revisions. The loader materialized only **94 NPCs**. Persisted NPC-row volume therefore substantially overstates LabMUD's live NPC workload. Armageddon has far more loaded rooms and progs and materializes thousands of NPCs; these worlds are not interchangeable scaling fixtures.

Only the separately owned `codex_labmud_boot_audit` schema on the disposable loopback MySQL instance was restored and run. The source had 294 migrations through `20261110040045_ClanAppointmentRecall`; the clone reached 298 through `20261113003430_AddStagedPerformanceRecords`. Upgrade and normalization were staged separately from the repeat-start comparison. The same frozen original/optimized Windows x64 Release executables as the Armageddon comparison are used; the optimized hash is recorded in the parent investigation.

Clone-only fixture changes disable the external Discord endpoint by targeting loopback port 1, pause AI Storytellers, and replace the test account's credential hash/salt and prompt preference with a known private fixture. No source credentials were changed. Both executable variants receive the same saved checkpoint and these same integration settings. This bounds external side effects and makes their costs comparable; it is not a direct measurement on the running source instance's service integrations.

Initial staging attempts exposed test-harness assumptions: obsolete default credentials, a numbered account menu, and a narrative health prompt instead of an HP prompt. Those incomplete receipts are retained. The harness now accepts the actual configured menu and requires character-entry stamina/health output followed by an accepted administrative shutdown and exit zero. Incomplete attempts are not successful smoke tests.

### Matched repeat-start results

Both runs restored the same normalized checkpoint, SHA-256 `3c3c5eac5ecef8bfea8db3154b97cdd6860fe9ea2094835abdaf5207b96b9126`, before launch. Both reported the database current. Neither performed seeding or migration application. Restore durations of approximately 39 seconds are excluded. The owned server used the source service's 128 MiB InnoDB buffer-pool size. There was no concurrent build, test run or content export during the timed boots.

| External measurement | Original engine | Optimized engine |
| --- | ---: | ---: |
| Process launch to ready message | 41.260 s | 37.937 s |
| Process launch to responsive menu | 41.908 s | 38.598 s |
| SELECTs to ready message | 27,311 | 24,772 |
| SELECTs to responsive menu | 28,781 | 26,258 |
| All statement types to menu | 57,855 | 30,429 |
| Aggregate SQL statement time to menu | 6.376 s | 5.838 s |

Both runs authenticated, entered Japheth and accepted `shutdown stop`, exiting zero. The observed menu-time reduction is **3.310 seconds / 7.9%**. This single desktop pair is not a statistical performance guarantee. LabMUD already falls within the expected 30–90-second range; the large-world Armageddon improvement is much greater because its loaded workload differs.

The optimized LabMUD run measured NPC loading at 3.062 s, economy at 1.651 s, startup persistence at 1.180 s, world-item loading at 0.595 s and FutureProg loading/compilation at approximately 0.18 s. Timestamp gaps identify **9.209 s in post-character finalization**, the largest remaining identified group. [Finalization](../../MudSharpCore/Framework/Futuremud.cs) runs registered game-item health-start and auction finalizers; per-type timing is needed to attribute that group further. There were 4,378 infection-query executions in the complete pre-ready digest. Their relationship to that phase remains unproven without finer attribution; definition deduplication would not itself remove these reads.

Both binaries reported the same existing missing-default-combat-message warning and continued to readiness; no Prog compilation failure or exception appeared in either captured boot. That content warning remains a separate follow-up, not an optimization regression repaired here.

## Exact payload repetition in the Armageddon world

Prog equivalence here requires exact stored function text, return type, accepts-any flag, static mode, public flag and the full ordered parameter signature including names. Comments, category and Prog identity are not executable payload fields. Matching them is still insufficient to prove that every identity can be merged. No whitespace, constants, source IDs or permissions were generalized away.

| Scope/family | Rows | Distinct payloads | Excess rows |
| --- | ---: | ---: | ---: |
| All loaded progs | 18,221 | 5,685 | 12,536 |
| Armageddon category | 14,319 | 3,613 | 10,706 |
| AssistAllies | 2,242 | 34 | 2,208 |
| Aggressive target predicates | 1,931 | 39 | 1,892 |
| ContainerState | 1,384 | 4 | 1,380 |
| PoisonState | 1,782 | 47 | 1,735 |
| CombatAbilities | 836 | 171 | 665 |
| NPC cash | 2,459 | 1,388 | 1,071 |

The Armageddon subset has 604 groups with multiple equal payloads; the full set has 651. Distinct-payload counts also include singleton groups. Family rows are subsets, not quantities to add to the total. For perspective, the original optimized boot still spent about 23 seconds in FutureProg loading/compilation. That gives a measured reason to pursue reuse, but no proportional speedup is promised: repeated short helpers can be cheaper to compile than unique complex programs.

### Best first consolidation candidates

1. **Container and poison initial-state helpers:** these account for 3,115 excess Prog rows together. [Container-state generation](../../ArmageddonSeeder/Seeders/ArmageddonWorldfileSeeder/ArmageddonWorldfileItems.cs) sets initial open/locked state; [poison-state generation](../../ArmageddonSeeder/Seeders/ArmageddonWorldfileSeeder/ArmageddonWorldfilePoisonDelivery.cs) sets the complete kind/type/dose register payload. Identical full bodies/signatures are good leaf-helper candidates. Keep distinct item prototypes, component state and provenance; attach the shared helper separately at each original on-load point.
2. **Ally and aggression predicates:** [ally-support generation](../../ArmageddonSeeder/Seeders/ArmageddonWorldfileSeeder/ArmageddonWorldfileNpcAllySupport.cs) and [generic AI generation](../../ArmageddonSeeder/Seeders/ArmageddonWorldfileSeeder/ArmageddonWorldfileGenericAi.cs) create many equal functions per source vnum. Pool only complete equal payloads. Preserve race/guild/clan/target filters, range, memory and actor arguments.
3. **Cash and combat setup after finalization:** cash programs are subsequently modified by ally-support and other NPC passes, and looked up by vnum-derived function names by [NPC casting](../../ArmageddonSeeder/Seeders/ArmageddonWorldfileSeeder/ArmageddonWorldfileNpcCasting.cs). Interning the initial cash body would let a later mutation change another NPC's setup. Finish per-NPC construction first, then use explicit source-key-to-shared-ID mappings; remove assumptions that a source vnum always owns a uniquely named Prog.

The [Prog writer](../../ArmageddonSeeder/Seeders/ArmageddonWorldfileSeeder/ArmageddonWorldfileProgs.cs) currently allocates unique named rows and rejects name collisions. Reuse needs a dedicated immutable helper pool plus provenance for every source reference. It should not silently change the general builder Prog identity contract. Existing worlds need a reference-aware conversion with an ownership manifest, including XML and name-based references; a blanket DELETE of duplicate bodies is unsafe.

### Cascading hook and AI opportunities

Exact XML grouping uses type plus complete definition; hooks additionally include event type. A second calculation replaces only confirmed Prog-reference IDs with IDs from exact-equivalent Prog groups, retaining the rest of each XML definition. Hooks use `FutureProg`; Aggressor uses `WillAttackProg`; TrackingAggressor uses `WillAttackProg` and `PathingEnabledProg`. Arbitrary numbers were not replaced.

| Definition family | Rows | Exact distinct definitions | Distinct after equivalent Prog references |
| --- | ---: | ---: | ---: |
| Hooks | 4,232 | 3,756 | 445 |
| AI | 2,217 | 2,213 | 320 |
| Aggressor AI subset | 1,967 | 1,967 | 74 |
| TrackingAggressor subset | 123 | 123 | 123 |

These are definition-reuse ceilings. Hook commands, handling-result flags and events remain part of the comparison. AI range, memory, emotes and other settings remain part of the comparison; shared runtime objects must also be checked for cross-NPC mutable state before pooling them. Reusing a Prog while keeping separate AI identities is an independent, safer first step.

All **17,478 hook attachment rows** were checked for repeated hook/target bindings and equivalent definition/target bindings. Both counts were zero, including after the selective Prog canonicalization. Thus this audit found redundant definitions, not evidence that the same target is accidentally running an identical hook twice. It does not authorize skipping arbitrary room hooks or changing invocation counts.

### NPC templates and foundation toolkits

There are 5,035 NPC template rows with distinct IDs in the Armageddon copy. Ten pairs have identical template XML, but the pairs cease to match once their AI attachment sets are included. Ordinary/summoned identities and arena outfit variants have separate behavior/gear contracts. [NPC generation](../../ArmageddonSeeder/Seeders/ArmageddonWorldfileSeeder/ArmageddonWorldfileNpcs.cs) deliberately preserves one source-vnum identity; [arena generation](../../ArmageddonSeeder/Seeders/ArmageddonWorldfileSeeder/ArmageddonWorldfileArena.cs) deliberately adds variants. No fully equivalent NPC-template merge is established by this audit. Shared behavior helpers are the larger demonstrated opportunity.

The remaining 3,902 non-Armageddon progs contain 2,075 exact payloads. Notable examples:

- Chargen has 1,584 rows, 297 distinct payloads and 1,287 excess rows. The largest group contains 1,251 `CultureAccentAvailable` progs. [Culture toolkit generation](../../DatabaseSeeder/Seeders/CultureSeeder/CultureToolkit/CultureToolkitAccents.cs) attaches these to actual accent availability fields and records per-accent managed ownership. They are referenced policy definitions, not demonstrated orphans. Shared defaults would need explicit builder-edit and ownership semantics.
- Clan has 376 rows/11 payloads; Law has 143 rows/11 payloads. Some equal false/zero bodies are deliberate role-specific clone-election placeholders. [Clan seeding](../../DatabaseSeeder/Seeders/ClanSeeder/ClanSeeder.cs) and clone-time rebinding distinguish these by stable names. Preserve those identities unless that workflow is redesigned. A category called DebugAuthority is not sufficient proof that its laws or policies are unused.
- Character-description progs have little exact duplication: Character has 1,590 rows/1,573 payloads. Do not remove distinct descriptive outputs merely to reduce row counts.

The totals across category boundaries differ slightly because a few equal payloads span categories. Toolkit availability, placeholders and historical revisions are not the same as unused data. These measurements establish no general orphan-deletion list. The current loaders load all Prog, hook, AI and NPC-template definitions, including template revisions; pruning or lazy-loading revisions would require checking saved-instance references and builder workflows separately.

### LabMUD content comparison

The read-only export from the upgraded LabMUD copy contains 518 progs/420 exact payloads, with 98 excess rows in 20 duplicate groups. Its largest equal groups are Law policies; preserving independent policy-edit identities remains relevant. All 12 hook definitions and all 23 AI definitions are distinct under exact matching. Its 12 hook bindings contain no duplicates. The 146 NPC template rows belong to **77 template IDs**, so much of the row count represents revisions rather than distinct NPC definitions. These results support prioritizing generated Armageddon helpers rather than a blanket cleanup of both worlds.

## Recommended next implementation tranche

Start with immutable leaf-helper pooling for container/poison state, then exact ally/aggression helpers. Keep source identities and provenance, and establish a mapping before considering dependent hooks/AI or mutable cash setup. Preserve template/clone-placeholder identities. A migration for existing worlds is a separate concern from making fresh imports smaller.

Acceptance should include native compilation, per-source reference/provenance completeness, unchanged on-load event counts, initial item/register state, NPC targeting/memory, cash and population/equipment outcomes, builder override isolation, and save/restart. Repeat the same-checkpoint boot benchmark after that change. These optimizations are proposed from measured content, not yet implemented or benchmarked here.

The subsequent [14 November consolidation](../Seeding/Armageddon_Worldfile_Prog_Consolidation_20261114.md) implements exact post-generation sharing for fresh Worldfile imports and reduces the complete world's progs from 18,221 to 7,703. It records the scalar-reference defect found and fixed during qualification, final test results, independent static equivalence and startup/restart limits. Existing worlds and named foundation placeholders remain unchanged.

## Evidence

Machine-local ignored evidence is under `.artifacts/labmud-boot-audit/`: source inventory/backup receipts, owned clone identity marker, staging and benchmark receipts, timestamped output, redacted telnet transcripts, migration metadata, normalized backup and the content exports. The export data and source-derived bodies are not public repository artifacts. The content review used read-only exports and owning source code. No source database content was consolidated or removed.

The successful matched receipts are `lab-baseline-128-receipt.json` and `lab-optimized-128-receipt.json`. `lab-upgrade-normalise-v4-receipt.json` records the successful staging smoke; its enclosing checkpoint-export command initially failed on an unsupported client flag, then a separate capture succeeded without another Engine run. Earlier failed smoke attempts and export diagnostics remain local. The definition export uses explicit utf8mb4 to preserve legacy LabMUD characters. These are harness corrections, not changes to the measured executables.

No runtime or seeder source changed during this follow-up. Verification consists of the actual MySQL/telnet runs, read-only dataset comparisons, Python syntax checks, documentation-link validation and `git diff --check`; no additional automated code-test suite was required for the audit documentation. The earlier runtime test result remains recorded in the parent investigation. Final cleanup checks confirm the disposable service stopped, the source MySQL service remains available, and the original UAT executable/backup hashes remain unchanged.
