# Large-world startup investigation — 13 November 2026

## Result and scope

The Armageddon world exposes confirmed scaling problems in character loading, character login cleanup, FutureProg compilation and population/reset processing. The slow startup is not explained simply by reading a large item catalogue or by MySQL rebuilding indexes.

The implementation on `codex/armageddon-startup-profile` reduces process-launch-to-responsive-menu time from **196.0 to 115.3 seconds (41% less time)** in an identical-checkpoint comparison at a 128 MiB buffer pool. SELECT statements to that menu fall from **151,206 to 22,018 (85% fewer)**. Startup persistence completes before readiness. These are local Windows measurements, not a 30–90-second guarantee. The original qualified RC executable and UAT environment remain unchanged; this work is a separate private performance build based on documentation commit `46c0aaec39a8672438c32130f767131e465ffd21`.

The following investigation sections describe the original behaviour and recommendations. The implementation, controlled comparison and verification are recorded at the end of this document.

All three profiling boots used a byte-identical copy of the frozen Windows x64 Release single-file Engine from binary source `7d0c447c086629a000e3cbca6d3b22738d0f5c84`. Executable SHA-256: `72e8fcad15037a7013e43daada2cb484f64b0a1a4e45897e067f44e7aea3c9b7`. There was no seeding, migration application, Debug build, source rebuild or documentation export in those boots. The ordinary migration check reported the database current.

The database was the **disposable restored inspection copy**, `codex_armageddon_rc1_restore`, on the previously owned loopback MySQL instance. The original UAT database, UAT executable/configuration and baseline SQL backup were not modified. The existing walkthrough instance/database were untouched. Normal world activity and saves advanced only the profiling copy; it is no longer the pristine restored checkpoint. The baseline dump remains the reset source.

## Method and measured phase costs

An external observer timestamped each stdout line with a monotonic clock from process launch. MySQL Performance Schema statement digests were sampled every five seconds, with before/ready/login snapshots. These provide normalized statement counts, aggregate server statement time and rows examined/sent without retaining parameter values. CPU/call-stack samples were collected with `dotnet-trace` 10.0.750501. The Engine and database were stopped gracefully after each scenario.

The first measured boot had a 45-second trace attached during NPC loading. The second had no attached trace. A third boot sampled early loading/compilation and the post-timer hooks. The three responsive-menu times were **202.0, 193.7 and 199.0 seconds**. These are desktop observations, not isolated production performance guarantees. NPC/reset state advanced between boots, so they are not an identical-state A/B comparison.

| Phase | Measured boot 1 | Measured boot 2 | Interpretation |
| --- | ---: | ---: | --- |
| NPC query, materialisation and login | 75.77 s | 74.55 s | 2,618 / 2,647 persisted NPCs loaded. Includes constructor and login work, not only SQL. |
| FutureProg construction/materialisation | 12.46 s | 11.86 s | 18,221 progs; includes query materialisation and eager source colourisation. |
| FutureProg compilation | 27.31 s | 27.43 s | CPU dominated; repeated full-name lookup scans confirmed by trace. |
| Boot-finished hooks/reconciliation after internal timer | 22.71 s | 19.78 s | Room/NPC hooks; trace identifies reset execution, NPC creation and save flushes. |
| Room hydration | 5.96 s | 5.89 s | 26,792 loaded rooms including instantiated interiors. |
| Vehicle loading | 4.90 s | 4.82 s | Only 12 vehicles; worth checking graph fanout after larger costs. |
| Shop loading | 3.52 s | 3.44 s | 165 shops. |
| Optional Discord connection attempt | 2.02 s | 2.04 s | Separate from loader's internal boot timer. |
| Ready message to responsive login menu | 7.15 s | 5.59 s | First-loop scheduled interior population contributes. |
| **Process launch to responsive login menu** | **202.00 s** | **193.73 s** | End-to-end availability. Rows above are selected phases, not an exhaustive sum. |

NPC loading, FutureProg loading/compilation and post-timer hooks account for approximately **69%** of boot 2. By comparison, 15,114 item prototypes loaded in approximately 1.3 seconds. Large static catalogue counts alone are not a useful optimisation priority.

## 1. Character graph loading: confirmed N+1 queries

The second boot recorded **149,170 SELECT statements**, plus **149,247 SET statements**, by the responsive login menu. Across statement types the total was 298,911; aggregated server statement time was 27.14 seconds. Observer queries against Performance Schema are excluded from these totals, though a small number of client session setup statements remain. These are SQL statement counts, not a claim that each is a separate TCP connection.

Representative normalized SELECT shapes and counts in that boot:

| Relation/query | Executions | Returned rows in total |
| --- | ---: | ---: |
| Character signed languages by character ID | 5,296 | 0 |
| Character forms, instances, accents, scripts, project queues | approximately 5,296 per shape | Varies |
| Body drug doses by body ID | 2,647 | 0 |
| Body implants by body ID | 2,647 | 0 |
| Allies by character ID | 2,647 | 0 |
| Chargen roles by character ID | 2,647 | 0 |
| Room/overlay exit query | 9,040 | 33,564 |

The root cause is visible in source:

- [LoadNPCs](../../MudSharpCore/Framework/FuturemudLoaders.cs) includes NPC AI links and the Character root, with the former deep include graph commented out. It calls `TryGetCharacter` for each loadable NPC.
- [TryGetCharacter](../../MudSharpCore/Framework/Futuremud.cs) queries the Character again and constructs it using lazy proxies.
- [Character.LoadFromDatabase](../../MudSharpCore/Character/Character.cs), [LoadForms](../../MudSharpCore/Character/CharacterForms.cs), [instance loading](../../MudSharpCore/Character/CharacterInstances.cs) and [Body.LoadFromDatabase](../../MudSharpCore/Body/Implementations/Body.cs) read many unloaded collections. An empty collection still costs its own query.
- [FMDB.CreateSession](../../MudSharpCore/Database/FMDB.cs) opens the mandatory explicit connection, but configures EF with a separate connection string. EF opens/closes its connection around commands; repeated pool reset/session initialisation adds work. This is reuse from a pool, not necessarily a new physical connection for every command.

The NPC trace sampled 37.56 seconds inside `LoadNPCs`: 19.10 seconds included EF `LazyLoader.Load`; 5.14 seconds included MySqlConnector's session reset path. These are **overlapping inclusive sampled stack times**, not additive savings estimates. The character graph is also read during later saves, explaining why several shapes execute approximately twice per NPC.

**Recommended change:** prefetch the graph for the already-selected loadable NPC IDs at the existing allowed character-materialisation stage. Use tracked split queries or explicit batched collection loads, preserving the current boot dependency order. Cover Character forms/sources, primary/secondary instances, character traits, languages, projects/jobs, hooks, NPC/Guest references and all body collections actually consumed. Mark empty collections loaded. Reuse the prefetched Character roots rather than issuing the current per-character root query.

Do not restore the old commented include graph verbatim: it misses newer forms/instances/character traits/jobs paths and would risk a large cartesian join. Preserve stabled/dead/archived exclusions, pending spell-owned activation, offline employment stasis and secondary-instance persistence. Keep on-demand player loading as a separate path. Measure holding EF's connection open within the scoped boot loader; do not globally disable connection reset to conceal the cost.

## 2. Login cleanup: confirmed repeated global scans

[Character.LoginCharacter](../../MudSharpCore/Character/Character.cs) invokes [PossessionControlService.RemovePossessionEffectsForAnchor](../../MudSharpCore/Character/PossessionControlService.cs) for every primary NPC. That enumerates/deduplicates all loaded actors, cached actors and items, then checks each owner's effects. This scales with **NPCs × loaded owners**, including when no possession effects exist. It consumed 7.88 seconds of the 37.56-second NPC trace window.

NPC login emotes also reach [MediaChannelService.GetSources](../../MudSharpCore/Computers/MediaChannelService.cs), which scans all world items for media-source components for each local output. Its item-source predicate appeared for 1.19 seconds in that trace window. This is a structural runtime concern beyond boot, though smaller than possession cleanup in this sample.

**Recommended change:** maintain a lifecycle-aware index of possession effects by anchor identity/instance and a registry of actual media-source components. Enumerate relevant registered sources rather than every game item, and retain current true-location filtering for nested containers, movement and carried sources. An alternative boot-only possession reconciliation must run once after hydration and preserve expiry/removal callbacks and control restoration. Simply skipping cleanup during boot or suppressing NPC events is not a demonstrated equivalent fix.

## 3. FutureProg: measured CPU work and avoidable duplication

The early trace sampled **40.07 seconds** inside `LoadFutureProgs`, including 39.09 seconds marked as CPU time. The SQL query for progs/parameters took 1.41 server seconds in the first measured boot.

- **User-defined call resolution:** [FunctionHelper.CompileFunction](../../MudSharpCore/FutureProg/Functions/FunctionHelper.cs) scans `gameworld.FutureProgs.FirstOrDefault(...)` for every user-defined call. Its name/parameter predicate consumed **8.57 sampled seconds**. Built-in functions already use an index. Build a name-to-overload-list index once for the boot compilation batch. Preserve invariant case-insensitive comparison, enumeration order, first compatible overload and `AcceptsAnyParameters`. A scoped index can fall back to existing lookup outside the batch and avoids stale builder rename/add/delete data.
- **Source colourisation:** [FutureProg constructors](../../MudSharpCore/FutureProg/FutureProg.cs) assign `FunctionText`, whose setter colourises, then explicitly colourise again. Colourisation consumed **9.79 sampled seconds**. Remove the duplicate pass first; lazy presentation formatting can subsequently avoid boot formatting entirely, provided edits and dark-mode changes invalidate the cache. This is presentation work, not compilation or execution.
- **Repeated regex construction:** [IfBlock colourisation](../../MudSharpCore/FutureProg/Statements/IfBlock.cs) constructs the same end-statement Regex per line. Regex construction/parsing appeared for about 2.2 sampled seconds within the broader colourisation work. Reuse the expression; this figure overlaps the 9.79-second colourisation figure.
- `All<T>.Add` has a linear duplicate check and is theoretically quadratic for bulk insertion, but only **0.22 sampled seconds** appeared there during this phase. It is not the first optimisation to pursue. This is why source-pattern suspicion needs measurement.

These figures identify expensive code paths; they are not measured post-fix speedups and should not be added together where call stacks overlap.

## 4. Population/reset work and misleading readiness

The internal `Total Boot Time` stopwatch stops **before** NPC/room boot-finished hooks and Discord connection. Therefore it underreports time to availability. See the tail of [LoadFromDatabase](../../MudSharpCore/Framework/FuturemudLoaders.cs) and the handoff in [Main](../../MudSharpCore/Server/Main.cs).

The post-timer trace sampled 21.10 seconds: about 18.98 seconds were inside FutureProg hooks, 11.39 included `LoadNpcFunction.Execute`, and 11.22 included `SaveManager.Flush`. NPC creation through native controller initialisation can flush existing queued saves. Several save methods then lazy-load character child collections in a new context. These times overlap; they are not separate 11-second penalties.

[InstanceRooms.Execute](../../MudSharpCore/FutureProg/Functions/Location/InstanceRooms.cs) also scans every room to find instances of a template. It appeared for 1.40 seconds in the hook window. [RoomTemplateInstance.InitialiseRooms](../../MudSharpCore/Effects/Concrete/RoomTemplateInstance.cs) schedules additional population work; the first-loop trace confirms this work ran after the ready message and contributed to delayed menu responsiveness.

The original installation's first Engine boot loaded **zero NPCs**, whereas its next boot loaded **2,443 persisted NPCs**. Initial population thus materially changes the workload. That supports first-start population as a contributor to the original 514-second result; it does not establish how much of the remaining difference was cache, I/O or host load. No index-rebuild phase was observed in this investigation.

**Recommended change:** index template-to-instance membership; profile/coalesce equivalent zone reset evaluations without changing spawn limits or reset semantics; remove broad queued-save flushes from individual creation paths only after proving durable-ID/ownership ordering remains correct. Include the callback and first-loop population phases in startup diagnostics, and distinguish a listening socket from responsive application readiness. Moving all population after a ready message would only hide the delay.

## Secondary checks and implementation order

The owned MySQL server used a **128 MiB InnoDB buffer pool**, versus approximately **648.5 MiB of table/index allocation** for the profiling database. A larger measured working-set cache is a useful environment experiment, but will not remove the confirmed CPU scans or per-entity query pattern. Allocation size is not a precise RAM working-set requirement.

Vehicle loading includes many sibling collections without `AsSplitQuery`; 4.8 seconds for 12 vehicles warrants a SQL fanout/query-materialisation comparison. Shop loading also has a multi-collection graph. Room loading already uses split queries and took only about six seconds. Inspect actual row multiplication before adding indexes or globally changing query splitting. The 9,040 per-room exit queries are another batching opportunity after the larger costs.

Recommended implementation sequence:

1. **Bounded compiler improvements and diagnostics:** duplicate colourisation removal, shared regex, scoped user-function index, and accurate phase/readiness metrics. Test overload order, case matching, any-parameter functions, recursive/forward references, batch cleanup and subsequent builder edits.
2. **Largest boot win:** startup-only tracked NPC graph batching and root reuse, plus indexed possession/media lookup. Verify real multi-form characters, empty/populated child collections, secondary-instance recovery, stables, employment stasis, spell activation and possession callbacks. Test query-count scaling at two population sizes; a timing assertion alone is insufficient.
3. **Reset/save tail:** template-instance index, reset evaluation reuse and targeted creation/save batching. Prove final population/equipment/ownership and save/restart results remain equivalent.
4. **Then** benchmark EF connection scope, buffer-pool sizing, vehicle/shop split queries and exit prefetching separately. Repeat from an identical reset checkpoint, one variable at a time.

A 30–90-second repeat start is a useful target to test against, **not a result established here**. Removing the measured traps should be evaluated before changing world content or architecture.

## Original investigation evidence and verification

Machine-local, ignored evidence is under `.artifacts/armageddon-boot-profile/`: `profile.py`, `analyze.py`, `analyze-trace.py`, `analysis.json`, the three `*-receipt.json` files, timestamped `*-output.jsonl`, sampled digest snapshots, `.nettrace` files and derived Speedscope/stack summaries. These files are not public release assets. The scripts explicitly verify MySQL data-directory/server identity, select only the disposable restored database and use the repository's telnet helper with credential redaction.

All three boots reached an authenticated character, accepted `shutdown stop` and exited zero. The owned MySQL process then shut down normally. Final process/port checks are recorded locally. No code tests were run for this investigation-only documentation change; live execution used the unchanged qualified executable. Python syntax and `git diff --check` were checked.

Profiling interpretation follows the primary documentation for [MySQL 8.0 statement summary tables](https://dev.mysql.com/doc/refman/8.0/en/performance-schema-statement-summary-tables.html) and [dotnet-trace](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/dotnet-trace). Statement time is not a direct CPU measurement; sampled inclusive call-stack times overlap.

## Implemented changes

- **Compilation:** a disposable, thread-local boot compilation index preserves case-insensitive name lookup, overload enumeration order, first-compatible selection and any-parameter functions. Ordinary builder compilation uses live lookup outside the scope. Source colourisation is lazy and invalidates on source or dark-mode changes; duplicate construction work and per-line Regex creation are removed.
- **Character hydration:** selected loadable NPC IDs are loaded through tracked split queries in batches of 1,024. The graph includes primary/form/source bodies, empty child collections, character instances, projects, jobs, languages, hooks and NPC references. Temporary roots replace repeated per-character queries and are released after the loader. Existing archive, stable, employment-stasis and spell-activation gates remain in place. EF's connection remains open only within the scoped boot load.
- **Runtime lookups:** possession effects are indexed by anchor identity and physical instance, including legacy wildcard instances. Registration, self-removal, callback ordering, cached actors, archive and simulation release are covered. Media discovery registers actual source components while checking their current true locations; component prototype mutations refresh membership before deletion callbacks. A confirmed pre-existing collection-modified exception in prototype removal was fixed by iterating a snapshot.
- **Rooms and population:** template-instance membership is indexed in stable world order. Existing shared population requests are coalesced and drained, including reentrant requests, before readiness. Arbitrary per-room hooks retain their arguments and execution: sharing a program ID does not prove that their side effects are equivalent. Online scheduled behaviour is retained.
- **Identity and saves:** native NPC creation commits the initialisation queue before reading/publishing identity. During startup, implicit late-ID reads avoid flushing unrelated dirty objects. Explicit Flush remains a full save, other save-manager implementations retain their existing fallback, and the scope restores normal behaviour on exit. Large startup character-save batches prefetch tracked graphs; the final full save is timed before readiness, so savings are not concealed in the first autosave.
- **Other loaders:** exit rows are cached for critical boot topology hydration, preserving duplicate-ID, endpoint, overlay, door, climb and fall semantics and falling back for unknown rows. The cache is released after preload or topology updates. Vehicle includes use split queries. Shops already used split queries and needed no change. No migration, schema index change, world-content reduction or global database setting is required.
- **Diagnostics:** each loader, boot-finished hooks, deferred population, reconciliation, final persistence and the first completed game-loop iteration report durations. Total Boot Time now includes completion callbacks and service connection work.

## Controlled implementation benchmark

Each controlled first boot restored the same stopped-world SQL backup into the separately owned `codex_armageddon_boot_ab` schema. Backup SHA-256: `1a6ffe9dec966d3cddfed4e826c664b0af806efe32e3895c1f89d89bf9fac6f5`. Initial counts were 2,652 characters, 2,651 NPC rows, 26,792 rooms, 30,794 game items, 12,453 body-item links and 42,316 exits. Restore time is excluded. There was no seeding, first-time normalisation or concurrent compilation/test workload during measured boots.

| Measurement | Original RC, 128 MiB | Optimized, 128 MiB |
| --- | ---: | ---: |
| Process launch to ready message | 188.898 s | 114.312 s |
| Process launch to responsive menu | 195.999 s | 115.348 s |
| SELECTs to ready message | 149,777 | 21,480 |
| SELECTs to responsive menu | 151,206 | 22,018 |
| All statement types to responsive menu | 305,045 | 25,656 |
| Aggregate SQL statement time to menu | 29.829 s | 14.018 s |

The final optimized boot measured FutureProg loading/compilation at 23.345 s, NPC loading at 13.129 s, room hooks at 7.958 s, deferred population at 3.481 s and full startup persistence at 9.881 s. Original investigation phase measurements were approximately 39 s for FutureProg and 75 s for NPC loading. The changes were benchmarked as a package; these figures do not establish an isolated speedup for every individual change. Vehicle loading remains approximately 4.8 s overall.

Final measured executable SHA-256: `68ed10b88083263aa9ea587f22290b89b1075908f05ed73bf8a209cd9abb8e28`. The original executable hash is recorded above. The private informational-version suffix is a build label, not a claim of a newly qualified RC or stable release. Final source fingerprints and the patch are retained with the local evidence.

### Buffer experiment and saved-world restart

The same optimized executable and restored checkpoint with a **1 GiB** InnoDB buffer pool reached ready at **104.778 s** and a responsive menu at **105.726 s** (22,020 SELECTs to menu). That is 9.6 s below the optimized 128 MiB observation. Only the owned process's buffer argument changed; no persistent/global configuration was altered. The single pair also contains normal desktop/cache variation, so the entire difference cannot confidently be attributed to buffer size. Import time remained separately excluded; MySQL reported redo-capacity pressure during the import, not an index-rebuild explanation for ordinary boot.

After graceful shutdown, a second Engine process loaded that newly saved database **without restoring it**: ready **99.835 s**, responsive menu **100.603 s**, authenticated character entry and shutdown exit zero. Before and after this restart, counts were identical: 2,681 characters, 2,680 NPC rows, 26,792 rooms, 30,960 items, 12,585 body-item links and 42,316 exits. The increase from the initial checkpoint reflects the first boot's ordinary population. The read-only graph and vehicle-equivalence probes passed again after restart. Counts and successful hydration are persistence smoke evidence, not a byte-for-byte proof of arbitrary gameplay equivalence or another full item-magic UAT campaign.

### Scaling and persistence evidence

The read-only real-MySQL probe loaded 8 and 64 NPC graphs with **44 reader commands at both sizes**, then consumed the graph with **zero additional lazy commands**, including empty navigations. Tracked state remained unchanged. Batches above 1,024 IDs deliberately add another bounded query set. Multi-body/form paths are also covered by in-memory graph tests.

For all 12 persisted vehicles, the original single-query include graph and split graph had identical canonical scalar/child-collection hashes. The probe used 1 versus 12 reader commands and approximately 0.139 versus 0.072 s when warm. This demonstrates graph equivalence and a smaller query-materialisation cost in that probe; it does not establish a multi-second whole-loader gain.

Automated regressions cover overload/forward lookup, colour cache invalidation, graph selection and empty/multiple bodies, effect/source/room-index lifecycle and callback ordering, native prototype mutation, boot and reentrant population queues, cached/direct exits, scoped identity saves, custom-manager compatibility, 32-character tracked parent/child save/reload, and requeueing all items when batch preparation fails. The original broad run's one failing new simulation fixture and the prototype-removal red regression are retained locally alongside their corrections and passing focused reruns.

Final automated run **`20261113T120129Z-4f583b7ad220` passed**, Release/net11.0 on Windows, with stable source fingerprints and the default fast exclusions:

| Suite | Passed | Failed | Skipped |
| --- | ---: | ---: | ---: |
| Core | 9,294 | 0 | 0 |
| Shared library | 871 | 0 | 0 |
| Database library | 244 | 0 | 0 |
| ArmageddonSeeder | 420 | 0 | 98 |
| **Total** | **10,829** | **0** | **98** |

All 98 skipped results explicitly require the absent private Worldfile corpus (`FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY`); they are unverified, not passes. The final persistence-focused run also passed 7 tests. Reports, TRX and native logs are under `.artifacts/test-runs/20261113T120129Z-4f583b7ad220/`. This pass did not run Linux CI, the full extended installer matrix or user acceptance testing. Documentation-only receipt additions followed the frozen-source test run; runtime and test inputs did not change.

### Limits and evidence locations

The target of 30–90 seconds remains unmet in this measured environment. Compilation and other CPU work remain material. The remaining SELECTs include per-character archive/activation checks and smaller existing loaders; this pass does not claim to remove every N+1 query. No cache-size recommendation for production should be inferred from a single desktop run.

Local evidence under `.artifacts/armageddon-boot-profile/` includes `ab-baseline-128-*`, `ab-optimized-128-*` (intermediate), `ab-optimized-128-v2-*` (final production code), `ab-optimized-1024-v2-*` and its `-restart-*` receipt, input/restore receipts, timestamped output, query digests, graph probes, focused TRX, private publish logs and source fingerprints. All successful live runs authenticated, entered a character and shut down gracefully with exit zero. The A/B harness verifies the owned MySQL identity and refuses to reset an unowned schema. Only the disposable copy experiences normal world activity and saves; the original UAT and walkthrough remain excluded.

Final cleanup verified both owned service ports closed and the original UAT executable and SQL backup hashes unchanged. The final runtime-change manifest against the base commit has fingerprint `4bbbfac63eda1b7e96a4c7c3206ead03f2567e79ba617d97734d1fdbe98d8c3d`; `final-runtime-source.json` records its file hashes and executable identity. Changes remain on the local implementation branch; the original RC package has not been replaced or relabelled.

The subsequent [LabMUD comparison and content-reuse audit](LabMUD_Load_And_Content_Audit_20261113.md) records a 41.9-to-38.6-second matched LabMUD boot and quantifies repeated generated Prog/hook/AI definitions. That follow-up leaves the source worlds unchanged and proposes a separate content-consolidation tranche.
