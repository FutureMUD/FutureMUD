# Armageddon generated-program consolidation — 14 November 2026

This implements the program-sharing recommendations from the [loaded-content audit](../Core/LabMUD_Load_And_Content_Audit_20261113.md) for fresh Armageddon Worldfile imports. Existing UAT, walkthrough and source LabMUD worlds are excluded. The package remains question-free and one-shot; this is not an existing-world data migration or a replacement release candidate.

## Implementation

All generation and per-vnum mutations complete before consolidation. Exact equivalence includes function text, return definition, ordered parameter names/types/indices, visibility, static mode and category/subcategory. Programs created by other seeders or builders do not enter the pool. Fully static programs are excluded because their cached-result identities can affect execution.

The first matching protected named target is retained, otherwise the lowest generated ID is canonical. Any generated name mentioned as an identifier in function text or serialized entity definitions is protected, including literals/comments. This is deliberately conservative: function text, recursion, literal strings and name-based lookup are never rewritten. Explicit numeric weapon-poison callbacks retain their targets; a callback expression the linker cannot resolve prevents pooling.

Relinking uses EF foreign-key metadata, typed numeric prog fields and serialization adapters. Liquid draught pointers are included even though the EF model declares no foreign key. Unrelated numeric fields remain untouched. Item on-load composite keys are deleted/recreated when needed; equal callbacks on the same prototype are preserved as separate invocations. Typed XML, trap ExecuteProg parameters, conditional-item policy JSON, and numeric prog settings are covered. Unknown keyed-reference shapes retain their target identities. Closure checks reject surviving typed references to removed IDs before commit; native compilation then validates all retained generated programs. The existing installation transaction rolls back on a save or compilation failure.

Every removed identity is recorded in the stable `progs.shared` provenance record. Historical raw snapshots retain their original IDs/names, with canonical resolution through the alias table; stable source keys survive. Existing builder programs and named foundation policy/clone placeholders remain independent. Shared imported helper edits intentionally affect every binding; clone and rebind before an individual override.

## Verification

Nine focused regressions pass: relational, FK-less scalar and serialized references survive cold reload; signatures and late mutations remain distinct; named target calls compile and execute after alias deletion; literal/dynamic numeric callback handling is conservative; typed JSON/trap/configuration adapters preserve unrelated numbers; and keyed on-load multiplicity is retained. One early native-call fixture failure was corrected by compiling its target before executing the caller. Compilation and intermediate failure evidence remain local.

### Measured reduction and independent comparison

The same private source packet and full installation selections produced **7,703 total progs instead of 18,221**, removing **10,518 identities / 57.7%**. The Worldfile's generated set fell from 14,175 to **3,657**. Source-vnum NPC/item definitions remain independent; foundation toolkit/policy names and protected callback identities account for intentional remaining repetition. No installed source world was modified to obtain this reduction.

The independently exported comparison confirms every alias's exact payload equals its canonical program. Retained Worldfile code and names match the earlier world. All 5,035 NPC templates, 2,217 AI definitions, 4,232 hook definitions, 7,510 template-AI links and 5,259 item on-load links match after expected reference remapping. All 574 import-time hook bindings match. The earlier world's 16,904 additional bindings belong to later population/instance activity, outside this static import comparison.

Independent fresh installs give foundation clone-policy names different generated GUID suffixes. Those names were not consolidated; their IDs/payloads match, and a same-world before/after snapshot verifies that the linker leaves foundation programs unchanged. The original reference export also required explicit utf8mb4 to avoid lossy punctuation in four retained vendor-list progs; the corrected export matches their actual stored code.

### Real-MySQL defect and final post-pass qualification

The full private-corpus installation completed and committed. Its first independent schema audit then found **120 liquid draught references** pointing at removed aliases. This exposed missing FK-less scalar handling; native compilation alone had not detected it. The production linker now processes typed long/int `Prog`/`ProgId` properties and checks their closure, with a new liquid cold-reload regression.

For the final scalar fix, the test restored the exact deleted generated program/signature rows into the owned full-install fixture, loaded 149 relevant model types, and replayed the corrected post-pass in a transaction. It retained the **same 10,518 aliases**, left foundation programs unchanged and committed. A new-context, independent SQL audit checked **121 numeric prog fields with zero unresolved references**; all 120 liquid pointers resolved. This is a transactional post-pass replay following the complete installation, not a second fresh installation of the final revision. The earlier full-install FK/XML/on-load evidence and final scalar replay are retained separately.

The native compiler accepted all **3,657 retained Worldfile programs**. All ten proportional-poison and ten retained draught entry points passed zero-dose/zero-quantity calls. Positive-dose ingestion was not re-exercised live; the unchanged payload comparison and existing poison tests cover that path.

### Native startup and saved-world restart

Both live runs used the unchanged optimized Windows executable and a restarted owned MySQL process with a 128 MiB buffer pool. Both reported the database current, authenticated, entered Admin, accepted `shutdown stop` and exited zero. No generated-program compilation failure or exception appeared in the captured boots.

| Measurement | First population boot | Saved-world restart |
| --- | ---: | ---: |
| Process launch to ready | 342.006 s | 116.256 s |
| Process launch to responsive menu | 344.995 s | 117.013 s |
| FutureProg loading/compilation | 20.970 s | 21.808 s |
| Persisted NPCs loaded | 0 | 2,443 |
| Room boot-finished hooks | 216.736 s | 6.440 s |

The first run creates initial population and is not an ordinary repeat start. The second run is from that saved world. Its population/state differs from the earlier 115-second startup checkpoint, so these timings establish startup/persistence smoke success, not a controlled end-to-end speedup from consolidation. The measured prog phase is only modestly lower than the earlier 23-second observation despite the substantial row reduction; expensive retained programs and fixed work remain. No 90-second startup claim is made.

After both boots, progs remain 7,703, NPC templates 5,035, item prototypes 15,114 and rooms 26,792. The owned copy has 2,491 NPC rows, 29,876 items and 11,545 body-item links. A further new-connection SQL audit again found **zero missing program references across all 121 checked scalar fields**, including draught callbacks. Those live population counts are observations, not fixed roster assertions or a claim of bit-identical random population between independent installations.

### Final automated suite

Release/net11.0 ArmageddonSeeder fast tests with the private corpus: **526 passed, zero failed, one skipped**. The skipped missing-corpus refusal test explicitly applies only when that corpus is absent. Input hashes were stable before/after. The nine focused tests also passed. Tests were selected with the normal Integration/Benchmark/ClimateSeeder/PsychicBenchmark exclusions; the broad extended installer matrix and Linux CI were not run.

The private integration runner initially omitted the normal console's save and answer-memory handling; those failed fixtures are preserved. Its successful run invokes the actual `SeederExecutionService`. An observed redo-checkpoint bottleneck was relieved temporarily on the owned installation process with a 1 GiB buffer/redo capacity, without persistent configuration changes. Startup smoke restarts the server at a 128 MiB buffer pool. No install-speed comparison is claimed.

Machine-local ignored evidence is under `.artifacts/armageddon-prog-consolidation/`. The external private corpus is read through its environment-selected directory and is not embedded in the product or repository.
