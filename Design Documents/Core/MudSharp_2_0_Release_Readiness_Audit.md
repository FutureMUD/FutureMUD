# MudSharp 2.0 Release-Readiness Audit

Audit date: 14 July 2026  
Release baseline: `a68f25871ce238e8e9331c0e33d7b585d174c0e4` (`Engine version update to v1.55.0`, 1 March 2026)  
Audited head: `9babc35e55cf296e324e414ac52c9b883b3238f7` (`master`, 12 July 2026)  
Primary scope: `MudSharpCore` runtime features, commands, builders, and core unit tests

## Executive Verdict

**Overall recommendation: No-Go for an all-inclusive 2.0.0 release at this snapshot.**

The engine has a strong automated baseline: the audited head passed all 1,668 core unit tests with no failures or skips, and the completed employment-closure worktree passed the default repository unit suite with 2,601 tests, including 1,686 core tests. Five large feature families now satisfy the audit's full readiness bar, and no reviewed subsystem needs architectural replacement. After resolving the Vehicle V1 boundary and completing the Unified Employment V1 contract, one advertised feature family still contains reachable unfinished behavior:

1. **Natural ranged weapons**: the shipped blowgun and sling components throw `NotImplementedException` from `GetDamage`, so their primary firing path is incomplete.
Unified employment completed its explicit V1 closure on 14 July 2026: the supported host/action/payment matrix, durable provenance and grants, earnings/payables, consent, goal policy, recovery, capability providers, action/host policy, and acceptance/security coverage are release-ready. Its parent-organisation/accounting and richer native-workflow roadmap is explicitly post-V1.

The release can move from **No-Go** to **Conditional Go** by completing or explicitly excluding the remaining natural-ranged-weapon scope. Vehicle route, coordinate, and moving-interior work is explicitly post-V1, while the employment parent-organisation/accounting and richer native-workflow roadmap is also explicitly post-V1. Documentation sign-off and integration testing should then be completed for the systems listed below.

### Status Totals

| Status | Count | Release meaning |
| --- | ---: | --- |
| Fully Ready | 5 | Suitable for the 2.0 stable feature set |
| Minor Polish Required | 3 | Releasable; low-risk follow-up remains |
| Documentation Required | 7 | Runtime appears releasable, but the documentation does not yet provide a clean v1.0 sign-off |
| Testing Required | 7 | Implementation is credible, but live/integration maturity is not sufficiently demonstrated |
| Further Implementation Required | 1 | Must be completed, narrowed, or excluded from the stable 2.0 promise |
| Major Changes Required | 0 | No reviewed system requires replacement or a fundamental redesign |

## Audit Method

The change catalogue was built from the Git history between the baseline and audited head, current runtime registrations, commands, builders, persistence paths, design documents, and the `MudSharpCore Unit Tests` suite. Feature history includes earlier implementation commits when a feature already existed at 1.55.0, because those commits are relevant evidence of hardening even though the catalogue itself describes the post-1.55 change.

A hardening round means a distinct corrective or refinement commit/PR after initial implementation. Several commits in one implementation PR do not by themselves satisfy the three-round maturity bar.

The audit used a hybrid unfinished-code boundary:

- new systems were reviewed as complete systems;
- established systems were reviewed through the changed end-to-end paths;
- legacy TODOs were counted only when reachable from, or directly adjacent to, the changed feature;
- defensive factory exceptions, abstract support types, and unreferenced legacy classes were not treated as blockers.

Examples deliberately excluded from blocker status include the abstract `MagicalMeleeAttackPower`, defensive unknown-type factory exceptions, and the unreferenced legacy `MedicalExaminationProposal`. By contrast, the blowgun and sling `GetDamage` methods are direct runtime contract implementations and are release blockers.

## Major Change Catalogue

| System | Major post-1.55 change | Primary status | Hardening evidence | Stable-release blocker? |
| --- | --- | --- | --- | --- |
| Health, wounds, infections, and thermal behavior | Health/infection overhaul, editable strategies, configurable temperature consequences | **Fully Ready** | Strong | No |
| Drugs and dependence | Expanded drug effects, dependence/offline behavior, clone metadata fix | **Testing Required** | Limited | No |
| Body forms, simultaneous instances, and possession | Form provisioning, multi-body V1, backups, projection, clone and possession controls | **Fully Ready** | Strong | No |
| Combat settings and strategies | Default combat settings, strategy audit, auxiliary/subdue behavior | **Minor Polish Required** | Strong | No |
| Natural ranged weapons and weapon poison | Sling/blowgun components, poison configuration and delivery | **Further Implementation Required** | Weak for ranged damage | **Yes** |
| Combat arenas | Scoring, surrender, ratings, betting, security and builder refinements | **Documentation Required** | Strong, including pre-1.55 | No |
| Magic and psionics | Multiple spell/power phases, trace, coercion, illusion, wards and information effects | **Documentation Required** | Strong | No |
| Planes, corporeality, portals, and zero gravity | Planar presence, transient/durable portals, zero-gravity movement and anchors | **Testing Required** | Moderate | No |
| Time, calendars, and celestials | `MudInstant`, astronomical event solving, regnal periods, builder hardening | **Testing Required** | Moderate | No |
| Pathfinding and live world topology | Incremental hierarchical index, live-cell mutation, portal-anchor correction | **Fully Ready** | Strong | No |
| Mounts and stables | Stable accounts/tickets, persistence fixes, riding and hitch gear | **Documentation Required** | Strong | No |
| Vehicles, hitches, and towing | Manual cell-exit vehicle movement, readiness, cargo/access, mixed hitch graph | **Testing Required** | Strong unit coverage; fresh-world runbook remains | No |
| Agriculture and primary production | Fields, crops, herds, woodlands, apiaries, project operations, commodity output | **Testing Required** | Moderate | No |
| NPC/group AI and event surface | New individual AI types, animal ecology, group registration and event subscriptions | **Minor Polish Required** | Strong | No |
| AI Storyteller | Builder/help refinement on the existing persisted model/tool-loop subsystem | **Testing Required** | Strong unit coverage, no live model loop | No |
| Economy and commerce | Markets, shop deals, hotel rentals, auctions, clan finance and security hardening | **Documentation Required** | Strong | No |
| Unified employment and task hosts | V1 host/action/payment matrix, durable grants, payroll evidence, consent, recovery and policy | **Fully Ready** | Strong automated closure matrix | No |
| Hospitals | Employment-host treatment dispatch, rooms, supplies, blood workflows and diagnostics | **Documentation Required** | Very strong recent fix history | No |
| Law, crime, patrols, and trials | Automatic crimes, evidence, custody, trials, executions and diagnostics | **Documentation Required** | Strong | No |
| Estates, ownership, and private property | Probate lifecycle, wills, claims, liquidation and durable item/cell ownership | **Documentation Required** | Strong | No |
| Computers, automation, grids, and telecoms | Computer runtime, persisted processes, terminal apps, signals, mail/FTP/boards | **Fully Ready** | Strong | No |
| Item, crafting, writing, and outfit extensions | Writing/book fixes, templates, condition maintenance, contamination and new components | **Testing Required** | Mixed by feature | No |
| FutureProg platform | Extensible type masks, stable persistence, utility/date/magic helpers and security | **Minor Polish Required** | Strong | No |

## Detailed System Assessments

### 1. Health, Wounds, Infections, and Thermal Behavior

**Status: Fully Ready. Release blocker: No.**

- **Change and evidence:** `5930e21b` improved the health system, `6d30315e` reworked infections, `31005b1b` added configurable temperature effects, and later rounds fixed AI wound treatment (`ac49ebc4`), body-switch feedback (`a94d20dc`), wound transfer/load behavior (`65997a6e`), and security (`e9e6e09b`).
- **Implementation:** Runtime strategies, wounds, infections, surgery adjacency, thermal consequences, builders, and persistence are present. The direct incomplete proposal class found by static search is unreferenced and is not part of the changed runtime path.
- **Documentation:** The Health document set covers runtime, medical interactions, items/adjacencies, damage, and builder/seeder state. It explicitly describes the stock medical entry point as release-ready.
- **Recommended work:** No release-blocking implementation work. Add a brief 2.0 compatibility note covering old health-strategy XML and database upgrades.
- **Remaining test plan:** Run multi-day live cases for infection progression, thermal exposure/recovery, surgery interruption, body switch during injury, offline healing, and save/reboot at each wound/infection stage.

### 2. Drugs and Dependence

**Status: Testing Required. Release blocker: No.**

- **Change and evidence:** `77f55e98` added the expanded drug-effect set; `443f8303` fixed null metadata in clone paths. This is only one independent corrective round after the expansion.
- **Implementation:** Drug builders, vectors, metabolism, effect aggregation, dependence, offline elapsed-time handling, and FutureProg exposure exist. No reachable post-1.55 TODO was found in the expanded effect path.
- **Documentation:** `Design Documents/Drugs` has a current implementation map and a substantial builder guide, but no explicit v1.0/release-readiness declaration.
- **Recommended work:** Complete a live pharmacology pass and add a v1.0 supported-effects/known-limitations table. Keep optional future delivery sources out of the stable promise unless tested.
- **Remaining test plan:** Exercise each delivery vector, multi-drug stacking, overdose, withdrawal, offline elapsed time, save/reboot, clone/copy, null metadata, sleep/pass-out interactions, and builder-created drugs with invalid or boundary values.

### 3. Body Forms, Simultaneous Instances, and Possession

**Status: Fully Ready. Release blocker: No.**

- **Change and evidence:** Body-form provisioning landed in `bc811eab`; backups in `6e98e167`; multi-body V1 in `5a64d8e2`; possession/illusion in `8a96b194`; authority and ownership hardening followed in `62568467` and `e1a39a29`.
- **Implementation:** Focus routing, secondary lifecycle, persistence, admin creation, projection, copies/clones, possession, cleanup, diagnostics, and major identity-vs-instance seams are implemented.
- **Documentation:** The simultaneous-body design explicitly states that the V1 foundation is ready and distinguishes post-V1 identity/social policy from architectural prerequisites. A builder guide is present.
- **Recommended work:** Treat remaining disguise, recognition, social consequence, and campaign-specific legal identity behavior as post-2.0 extensions, not hidden V1 promises.
- **Remaining test plan:** Repeat the documented smoke matrix across reboot/reconnect, death, retirement, combat, projects, patrols, stables, vehicles, arenas, possession authority, corpse possession, and two concurrent physical instances of one identity.

### 4. Combat Settings and Strategies

**Status: Minor Polish Required. Release blocker: No.**

- **Change and evidence:** Defaults and validation landed in `c8b1b44e`, `3e259b27`, and `b9547f3d`; auxiliary builder security followed in `cbdcc761`; subdue strategy behavior and tests landed in `321de20b`.
- **Implementation:** Combat-setting resolution, strategy selection, auxiliary actions, and subdue/control behavior are implemented with builder and player surfaces. Natural ranged weapons are rated separately below.
- **Documentation:** The combat strategy runtime document is current but still lists follow-up candidates and does not explicitly sign off a 1.0 strategy baseline.
- **Recommended work:** Add a compact supported-strategy matrix and state which remaining mounted/dual-wield/edge behaviors are post-2.0 polish.
- **Remaining test plan:** Run weapon/no-weapon, mounted, clinch, grapple, flee, helpless, subdue, hostage, truce, and target-loss scenarios for PCs and NPC strategies, including reboot during combat where persistence applies.

### 5. Natural Ranged Weapons and Weapon Poison

**Status: Further Implementation Required. Release blocker: Yes.**

- **Change and evidence:** Sling and blowgun support was added in `83f1602c`; poison configuration and integration have focused tests. The ranged feature has not had a corrective implementation round that closes its primary gap.
- **Implementation gap:** `BlowgunGameItemComponent.GetDamage` and `SlingGameItemComponent.GetDamage` directly throw `NotImplementedException`. Their load-error switches also throw for unexpected feasibility states. Current ranged tests largely inspect source/configuration and do not execute damage resolution.
- **Documentation gap:** `Natural_Ranged_Attacks_Implementation_Plan.md` still contains remaining follow-up and no release recommendation.
- **Recommended work:** Implement and behavior-test damage/ammunition resolution, or unregister/remove the unfinished components from the stable 2.0 feature set. Weapon poison can ship independently if its supported weapon matrix is documented.
- **Remaining test plan:** Cover load, ready, aim, fire, damage, ammunition recovery, poison transfer, hidden fire, breath/mouth restrictions, prone/position restrictions, stamina drain, all inventory-plan failure states, and NPC use.

### 6. Combat Arenas

**Status: Documentation Required. Release blocker: No.**

- **Change and evidence:** Arena runtime predates 1.55 and already had multiple correction rounds (`f82d6cb4`, `c0c2f867`, `e8765767`, `0bd42c97`). Post-1.55 work expanded scoring/runtime formats (`00bf7b2e`) and added security/rating guardrails (`85d8239c`).
- **Implementation:** Lifecycle, scheduling, participant preparation, observation, betting, finance, ratings, NPC management, cleanup, commands, builder helpers, and tests are substantial. No explicit TODO/NotImplemented path was found in `MudSharpCore/Arenas`.
- **Documentation gap:** The design has an extensive current snapshot, but `Combat_Arenas_Implementation_Plan.md` still says `Status: Draft for implementation handoff`. There is no clean v1.0 supported-scope declaration.
- **Recommended work:** Replace the stale plan status with a current v1.0 matrix and explicitly identify stock formats, crash policy, supported finance modes, and known non-blocking extensions.
- **Remaining test plan:** Run complete live events for each elimination/scoring mode, PC/NPC mixtures, spectator leakage, disconnect, reboot cancellation/refund, insolvent payouts, multi-winner pari-mutuel settlement, inventory restoration, corpse cleanup, and schedule recurrence.

### 7. Magic and Psionics

**Status: Documentation Required. Release blocker: No.**

- **Change and evidence:** Multiple independent phases landed and were refined, including persistent sensory/combat effects (`3eed20da`), psionic traces (`7400c745`, fixed by `bfb5d973`), security (`dd16a5b5`), presentation (`7f4e5489`), possession (`8a96b194`), and information effects (`ca61c8c0`).
- **Implementation:** Powers, spells, triggers, resources, generators, wards, illusions, traces, portals, possession, and FutureProg helpers have broad runtime and unit coverage. The remaining `NotImplementedException` in `MagicalMeleeAttackPower` is on an abstract, non-builder support base.
- **Documentation gap:** The document set is extensive and marks individual slices implemented, but it does not provide one builder-facing v1.0 release contract. The gap report mixes completed history with post-V1 ideas.
- **Recommended work:** Publish a concise v1.0 capability/type matrix and builder workflow that separates shipped primitives from campaign-specific future concepts such as fake physical entities and send-shadow policy.
- **Remaining test plan:** Live-test power/spell creation, save/reload, dispel, ward matching, persistent effects, portal reboot, psionic concealment/trace, possession cleanup, invalid progs, and mixed audience/perception behavior.

### 8. Planes, Corporeality, Portals, and Zero Gravity

**Status: Testing Required. Release blocker: No.**

- **Change and evidence:** Planar presence was added in `b9ec507d`; zero-gravity movement in `fe855700`; speed persistence in `4df2423f`; corporeality visibility was corrected in `a87940f0`; durable portal topology followed in `3f4424ce`.
- **Implementation:** Runtime helpers, commands, merits, terrain handling, transient/durable exits, anchors, and tethers are present without explicit unfinished code in the new core paths.
- **Documentation:** Dedicated corporeality/planes and zero-gravity documents exist. Zero gravity identifies its Version 1 boundary, including no collision damage.
- **Recommended work:** Complete a real-world integration round before declaring stable, especially for visibility, combat targeting, rebooted portals, and zero-gravity party/item movement.
- **Remaining test plan:** Test every corporeality pairing; planar speech/look/combat; portal creation, traversal, dispel and reboot; zero-gravity push/stop/bump/tether/anchor; party movement; items and vehicles; and transitions between gravity regimes.

### 9. Time, Calendars, and Celestials

**Status: Testing Required. Release blocker: No.**

- **Change and evidence:** Builder/runtime hardening landed in `322db5e1`, `28660b7d`, and `d098cae1`; the astronomical refactor landed in `ef1676ef`; regnal support and later fixes followed.
- **Implementation:** `MudInstant`, date/time parsing, recurrence, regnal periods, astronomical event searches, builders, and FutureProg functions are implemented and unit-tested.
- **Documentation gap:** Current-state time and celestial documents are strong, but the original astronomical refactor document still reads partly as an implementation proposal and there is no explicit v1.0 release sign-off.
- **Recommended work:** Reconcile the proposal with the current-state documents and run long-horizon/non-Gregorian validation before release.
- **Remaining test plan:** Test multi-calendar conversion, leap/intercalary boundaries, DST invalid/ambiguous times, recurrence over long horizons, regnal edits, `Never`, nth celestial events, opposite hemispheres, rebooted schedules, and extreme/invalid astronomical searches.

### 10. Pathfinding and Live World Topology

**Status: Fully Ready. Release blocker: No.**

- **Change and evidence:** Hierarchical indexing landed in `6fd839cd`; perceived-item routing was fixed in `2a5284e2`; live-cell mutation in `4502d5b4`; duplicate portal anchors in `ab1e33cf`.
- **Implementation:** The opt-in hierarchical service, idle-slice rebuild, immutable snapshot, live validation, automatic/exact modes, diagnostics, and mutation invalidation are implemented.
- **Documentation:** `Pathfinding_System.md` explicitly describes the V1 implementation and its deliberate exact-search/fallback boundaries.
- **Recommended work:** No release-blocking work. Capture representative large-world performance numbers for the release notes.
- **Remaining test plan:** Stress live cell/exit creation and deletion, transient portals, closed/locked doors, route invalidation, multi-slice rebuild, large maps, actor-specific suitability, and exact-vs-hierarchical semantic equivalence where expected.

### 11. Mounts and Stables

**Status: Documentation Required. Release blocker: No.**

- **Change and evidence:** Stable lodging and boot persistence landed and were fixed in `3370b4a0`, `55760a08`, and `e9953652`; account filters followed in `9ae075bb`; mount authorization in `4e61fd49`; riding/hitch gear in `6bc307ed`.
- **Implementation:** Stable accounts, tickets, lodging/redeeming, instance-aware persistence, mount movement authorization, riding gear, and builder/item surfaces exist with focused tests.
- **Documentation gap:** Builder/runtime information is distributed across economy, vehicle, item, and command material. There is no dedicated stable/mount v1.0 guide or release recommendation.
- **Recommended work:** Add one current stable-and-mount workflow document covering creation, fees/accounts, tickets, persistent secondary mounts, equipment, permissions, and known vehicle integration limits.
- **Remaining test plan:** Live-test lodge/redeem across reboot, account arrears, ticket loss/duplication, secondary character instances, rider authorization, gear install/remove, mount death, full stable capacity, and wagon/hitch integration.

### 12. Vehicles, Hitches, and Towing

**Status: Testing Required. Release blocker: No within the declared Vehicle V1 boundary.**

- **Change and evidence:** Vehicle movement and iterative fixes landed across `df464988`, `a6be9707`, `29e3b2ee`, and `935bed0c`; remote occupancy was fixed in `16d7d3cf`; hitch equipment in `6bc307ed`; the unified hitch graph in `113917cf`.
- **Implementation:** Cell-exit movement, explicit control handoff, player status/preflight, access points, cargo, modules, damage/repair, readiness, towing, persistent mixed hitches, builders, commands, and tests comprise the stable V1. The closeout also made required access fail closed, enforced exterior synchronisation and required crew, blocked combat/delayed-action driving, preserved the validated hitch/resource plan through delayed completion, prevented duplicate catastrophe rolls/fail-open train movement, made access/cargo projection markers revision-stable, rejected `RoomScale` and invalid authored values, and hardened primary control-station changes.
- **V1 boundary decision:** Vehicle V1 is manual cell-exit `ItemScale` and `RoomContainer` vehicles. Route movement, coordinate movement, and room-scale moving interiors are explicitly post-V1. Route movement is not moderate work: it needs persisted route/stop/schedule/journey models, scheduler and reboot ownership, boarding/dwell/delay state, builder previews and validation, player timetable UX, automation hooks, and integration tests. Existing readiness/path/hitch services reduce duplication but do not supply that subsystem.
- **Recommended work:** Keep route, coordinate, and moving-interior language out of the 2.0 stable feature promise. Complete the fresh-world runbook and release-candidate soak for the declared cell-exit boundary.
- **Remaining test plan:** Run fresh-MUD builder and player scenarios for create/install/access/start/drive/stop, passengers, cargo, fuel/power failure, damage, towing chains, mixed animal/vehicle hitches, cycles, detach/reboot, retirement/death of endpoints, and failed movement rollback.

### 13. Agriculture and Primary Production

**Status: Testing Required. Release blocker: No.**

- **Change and evidence:** Fields landed in `759e2988`; custom scores in `ce72c152`; seasons/herd driving in `6bc8650f`; apiaries and broader production followed. The implementation is concentrated in a short May delivery window rather than several live hardening cycles.
- **Implementation:** Fields, crop/herd/woodland/apiary definitions, daily ticks, project operations, property gates, commodity outputs, FutureProg, and builder/player `field` workflows are implemented with no explicit unfinished code in the new agriculture runtime.
- **Documentation:** Overview, runtime model, and builder workflow documents are comprehensive, but do not explicitly recommend a stable v1.0 release.
- **Recommended work:** Run at least one accelerated multi-season playtest and document the v1.0 balance/compatibility boundary independently of DatabaseSeeder content.
- **Remaining test plan:** Test create/delete/save/reload, all field uses and operations, seasonal windows, weather stress, ownership/lease permissions, project interruption, crop/herd/woodland/apiary outputs, herd draw/absorb/drive, custom scores, and commodity quality/seed recovery.

### 14. NPC/Group AI and Event Surface

**Status: Minor Polish Required. Release blocker: No.**

- **Change and evidence:** New AI types and examples landed in `698a686a`; the runtime/event authoring document in `e7285789`; template metadata/loading was refined in `80f1c0b1` and `724a7cc3`; later door/pathing fixes provide additional hardening evidence.
- **Implementation:** Individual and group factories, registrations, builders, XML persistence, event subscriptions, animal ecology, pathing, and template additions are present. The abstract predator-group default throw is overridden by the concrete registered predator group and is not a reachable builder gap.
- **Documentation:** The runtime catalogue and local `AGENTS.md` provide unusually strong authoring guidance. Remaining species-specific ecology and richer group behavior are clearly described as extensions.
- **Recommended work:** Add an explicit V1 support statement and reconcile the `WildAnimalHerdAI` special-case warning with the current document that now calls it builder-creatable.
- **Remaining test plan:** Long-run heartbeat/profile each new AI, verify save/clone/reload, event payload order, door/key movement, combat cleanup, shared-definition state isolation, group membership changes, and invalid builder configuration.

### 15. AI Storyteller

**Status: Testing Required. Release blocker: No.**

- **Change and evidence:** The main system predates 1.55 and has extensive unit history; the post-release change `69931d46` refined command help and coverage. The implementation-plan audit itself marks live end-to-end model integration testing as partial.
- **Implementation:** Persistence, builders, surveillance, situations/memories, tool loops, reference documents, pause controls, and bounded error handling are implemented.
- **Documentation:** Design and implementation-audit documents are current and honest, but do not recommend stable v1.0 while end-to-end external-model reliability remains untested.
- **Recommended work:** Perform a controlled provider-backed soak and define supported provider/model behavior, cost/rate-limit handling, privacy expectations, and degradation when credentials or providers are unavailable.
- **Remaining test plan:** Live-test echo/speech/crime/state/heartbeat triggers, malformed/slow/tool-only responses, retries, rate limits, provider outage, pause during a request, rebooted memory/situations, reference restrictions, malicious tool arguments, and prompt/context limits.

### 16. Economy and Commerce

**Status: Documentation Required. Release blocker: No.**

- **Change and evidence:** Shop deals, market pressure, hotel rentals, auction payout/security fixes, and clan finance received numerous independent rounds, including `c592de99`, `0320443a`, `7d2e6597`, `585848bf`, `3f02c7df`, and `39ed62c5`.
- **Implementation:** Core commerce workflows, commands, builders, finance adapters, persistence, and unit tests are broad. Employment, hospitals, and probate are rated separately.
- **Documentation gap:** The economy document set is substantial but distributed and does not provide a concise post-1.55 v1.0 feature matrix for shops, markets, auctions, hotels, and clan finance.
- **Recommended work:** Add an economy 2.0 release matrix stating supported payment/settlement paths and separating stable commerce from employment-preview behavior.
- **Remaining test plan:** Run cash/bank/store-credit purchases, shop deals, market shocks, hotel rent/expiry/lost property, auctions with all payout outcomes, clan finance permissions, cross-calendar taxes, reboot, insolvency, and duplicate settlement protection.

### 17. Unified Employment and Task Hosts

**Status: Fully Ready for the declared V1 contract. Release blocker: No.**

**Closure update, 14 July 2026:** the explicit V1 supported matrix and all ten ordered closure slices in `Employment Hosts Implementation Review and Roadmap.md` are complete. Gates 1-13 and 17-37 are automated and passing; parent-organisation gates 14-16 are explicitly Phase D post-V1 design.

- **Change and evidence:** The unified slice landed around `eabb283d`; craft reservations (`70b420ce`), blocked-task throttling (`cf04ad2c`), manager goals (`6363cad9`), persistence correction (`8f66cad3`), Epic 1 completion (`a21a7e68`), clan/hospital hosts, and scheduler optimisation (`291384d9`) show strong hardening.
- **Implementation:** Durable task provenance and scoped grants, supported wage authoring and disbursement states, schema-backed time/earning evidence, duty-aware assignment, immutable consented applications with atomic acceptance, durable manager-goal policies, bounded recovery, registered host capabilities, and authoring-time host/invocation policy are complete. Unsupported combinations fail before persistence; runtime authority, custody, reservation, and native-subsystem checks remain fail-closed.
- **Documentation:** The roadmap now begins with the stable V1 host/action/payment/finance/logistics matrix, separates every post-V1 limitation, maps the acceptance gates to automated evidence, and records final verification.
- **Verification:** The completed employment-closure worktree passed the full default suite with 2,601 tests: 396 shared-library, 6 expression-engine, 513 seeder, and 1,686 core tests, with 0 failed and 0 skipped. The maintained blank-database snapshot tracks the final employment migration.
- **Post-V1 work:** Parent organisations, consolidated accounting, cross-currency finance, autonomous vehicle/animal movement, and deeper native arena/bank/stable/hotel workflows remain optional future design and are not part of the stable V1 promise.

### 18. Hospitals

**Status: Documentation Required. Release blocker: No, provided the supported employment subset is explicit.**

- **Change and evidence:** Hospital employment dispatch landed in `11dcbed2` and then received many independent live-looking correction rounds: request cancellation (`8540f1a5`), supply routing (`48ecec00`), stabilisation/follow-ups, surgical targeting (`681dba95`), diagnostics (`c5975ae3`), blood recovery (`5439b234`), IV cleanup (`11289fe4`), clothing duplication (`416ca5a1`), and implicit IV staging (`5a48c83c`).
- **Implementation:** Builder/player/manager commands, room roles, staff/opening/application flows, services, requests, task dispatch, theatre/supply/recovery handling, blood donation, debt, cancellation, and failure diagnostics exist with substantial command-service tests.
- **Documentation gap:** Behavior is documented through the economy/employment documents and commit-driven clarifications, but there is no single hospital runtime/builder document with an explicit V1 release recommendation.
- **Recommended work:** Consolidate hospital commands, room setup, staffing, service billing, blood stock, supply custody, cancellation, and failure recovery into one V1 document. State exactly which employment features hospitals rely on.
- **Remaining test plan:** Fresh-build a hospital and run donation, stabilisation, full treatment, surgery, IV, blood compatibility, stock shortage/replenishment, staff absence, patient departure/death, cancellation, debt, reboot at every request phase, concurrent requests, and theatre item recovery.

### 19. Law, Crime, Patrols, and Trials

**Status: Documentation Required. Release blocker: No.**

- **Change and evidence:** Automatic crime and trial work landed around `835fae13`/`f222d823`; execution and custody then received many corrective rounds through `f565be53`, `6dd48221`, `bbfd11af`, `c18ab829`, `dfda5f4b`, `ee219e94`, and `04bf17cb`.
- **Implementation:** Automatic crime attribution, legal classes, patrol strategies, investigation/custody, bail/remand, automated/manual trials, executions, alerts, diagnostics, builders, and tests are present. Remaining TODOs are primarily filters/presentation or old extension ideas.
- **Documentation gap:** `Law_System_Runtime.md` is a strong current-state reference but does not explicitly declare a V1 supported matrix or release recommendation.
- **Recommended work:** Add a V1 status section listing supported patrol/trial strategies and mark witness bias, richer filtering, and other TODO comments as post-release enhancements.
- **Remaining test plan:** Run every automatic crime hook, known/unknown identity, disguise, witness evidence, patrol dispatch/doors, helpless custody, bail, rebooted trials, PC judge flow, sentencing, execution failure/retry, corpse recovery, alerts, and lawful-force crime suppression.

### 20. Estates, Ownership, and Private Property

**Status: Documentation Required. Release blocker: No.**

- **Change and evidence:** Estate inheritance landed in `4ba765cd`; probate/morgue flow in `c4b54aa6`; ownership/property fixes in `96de83e5`, `d4c2d90e`, `4fff3af2`, `bc397b95`, `d28ac0e4`, `49bcbb37`, `bcc7d8a0`, and `cdfecbbb`; durable ownership was expanded again in `6050e33a`.
- **Implementation:** Estates, wills, claims, inheritance, liquidation, auction integration, item ownership, private cells, commands, FutureProg, and durability tests are implemented.
- **Documentation gap:** The dedicated current-state document is strong but has no explicit v1.0 recommendation or compatibility section for pre-existing unowned items/private cells.
- **Recommended work:** Add V1 invariants and an upgrade/repair checklist covering ownership backfill, estate timers, auction dependencies, and partial property shares.
- **Remaining test plan:** Death in multiple zones, repeated death, wills/bequests, secured/targeted claims, partial shares, auction success/failure/unclaimed lots, payout fallback, item containers, private access, reboot at every estate phase, and ownership repair of legacy items.

### 21. Computers, Automation, Grids, and Telecommunications

**Status: Fully Ready. Release blocker: No.**

- **Change and evidence:** Computer contexts and runtime landed through `5450cef1`, `b6639325`, and `30a2b971`; interactive waits, host apps, mail, FTP, signals, and documentation followed through many independent April rounds; lifecycle was fixed in `d17fbb2a`; security in `211872cc` and `c19390b9`.
- **Implementation:** Workspaces, hosts/storage/terminals, persisted processes, files, terminal input, waits, built-in apps, mail/FTP/boards, networks, signals, sensors, controllers, cables, doors/locks, builders, and tests are present.
- **Documentation:** The concept/current-state document explicitly says the subsystem has reached its intended 1.0 design shape and places Messenger and richer topology beyond 1.0.
- **Recommended work:** No release blocker. Ensure release notes do not list Messenger as shipped and call the electric-grid `CanConnect` TODOs permissive V1 policy rather than completed constraint simulation.
- **Remaining test plan:** Reboot suspended processes, terminal ownership changes, storage removal, power cycling, signal waits, cable chains, mounted controller load order, network route/VPN scope, mail/FTP/boards permissions, malformed programs, execution limits, and unauthorized electrical/programming work.

### 22. Item, Crafting, Writing, and Outfit Extensions

**Status: Testing Required. Release blocker: No.**

- **Change and evidence:** This is a collection of smaller slices rather than one new system: writing/book persistence fixes, seal/measurement components, template outfits (`4c13a06c`, later load fixes `2a787a98`/`f1ccbd0f`), condition maintenance (`0bade47c`), contamination V2, commodity/project outputs, and crafting security (`2fe7df9a`).
- **Implementation:** Builders and tests exist for the major additions, but several tests are source-shape checks and each slice has only one or two independent runtime rounds. Deferred length measurement is correctly documented rather than silently stubbed.
- **Documentation:** The item and crafting suites are broad, but there is no concise release matrix identifying which new component families are V1-stable and which remain partial.
- **Recommended work:** Publish a component/change matrix and perform an interactive author-use-save-reboot test for every new item component and project/craft output type.
- **Remaining test plan:** Writing/read/tear/copy/import, outfits and covered placements, seal authorization, measurement/calibration, condition decay/maintenance, contamination mixing/transfer/save, cooked/liquid products, commodity/project output, clone/copy, component exclusivity, and builder validation.

### 23. FutureProg Platform

**Status: Minor Polish Required. Release blocker: No.**

- **Change and evidence:** The extensible type system landed in `aaa29b88`; legal/date/magic helpers followed in `0d0bd298`, `86d1420c`, and `ce683ea3`; documentation, utility expansion, and security were refined in `0b45fe6b`, `354ce7cd`, and `3da77305`.
- **Implementation:** BigInteger-backed type masks, versioned persistence, legacy compatibility, registry parsing, builders, utility/date/magic functions, documentation generation, bounded evaluation, and security tests are present.
- **Documentation:** `FutureProg_Type_System.md` is a clear current implementation reference with a stable `v1:<hex-mask>` persistence contract, but lacks a short release-readiness/compatibility checklist.
- **Recommended work:** Add a 2.0 migration and extension checklist for custom integrations that assumed a `long` enum, and explicitly list the compatibility bridge's intended lifetime.
- **Remaining test plan:** Upgrade legacy stored masks, overflow-era types, collection/dictionary modifiers, attributes using the legacy bridge, builder parsing, variable-register round trips, all newly added built-ins, malformed/overflow inputs, and custom plugin code compiled against the old enum assumptions.

## Cross-Cutting Fix Themes

The following are important 2.0 changes but do not need separate system ratings:

- **Authorization and security:** extensive hardening landed across command execution, FutureProg, economy, arenas, magic, health, automation, mounts, Discord/external surfaces, and builder permissions.
- **Persistence and load order:** repeated fixes addressed item lifecycle, mounted modules, NPCs, ownership, body instances, hospital custody, pathfinding live mutation, and calendar/time fallbacks.
- **Builder and help consistency:** `StringStack` parsing, room/cell targeting, command help, diagnostics, item names, template loading, and show output were repeatedly improved.
- **Automated coverage:** the core suite expanded substantially across arenas, employment, hospitals, law, magic, body instances, vehicles, agriculture, ownership, computers, pathfinding, writing, and FutureProg.
- **Performance:** hierarchical pathfinding, employment host scheduling/coalescing, bounded loops, and lazy empty-work gates are notable engine-level improvements.

## Ordered Pre-Release Work

1. **Resolve natural ranged weapons:** implement and test blowgun/sling damage or remove those components from the stable feature catalogue.
2. **Vehicle V1 boundary — resolved:** ship manual cell-exit `ItemScale` and `RoomContainer` vehicles, access/cargo/modules/readiness/damage, hitching, and towing. Route, coordinate, and moving-interior movement are post-V1.
3. **Employment V1 — complete:** the builder-authorable payment/authority/action boundary, durable grant/payroll semantics, acceptance matrix, documentation, migrations, and full automated verification were closed on 14 July 2026.
4. **Publish V1 sign-off sections:** arenas, magic, mounts/stables, economy, hospitals, law, and ownership need explicit supported-scope/known-limitations sections.
5. **Run integration campaigns:** drugs, planes/zero gravity, time/celestials, agriculture, AI Storyteller, and the grouped item/crafting additions require live or provider-backed testing beyond the green unit suite.
6. **Run a release-candidate soak:** use an upgraded real database, reboot repeatedly, exercise scheduled systems, and monitor logs/performance. This is a core runtime test; it does not require rating DatabaseSeeder.
7. **Complete release packaging:** `MudSharpCore.csproj` still reports `1.55.0`; bump version/file/assembly metadata to `2.0.0` only after the release scope is accepted, then author release notes and rerun verification.

## Verification Evidence

Commands executed against the audited head and the two completed closure worktrees:

```powershell
dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510
& .\scripts\test-unit-core.ps1
& .\scripts\test-unit.ps1
```

Results:

- `MudSharpCore` build: **passed**, 0 warnings, 0 errors.
- `MudSharpCore Unit Tests`: **passed**, 1,668 passed, 0 failed, 0 skipped.
- No live MUD, external AI-provider, or long-duration soak test was claimed by this audit.

Vehicle V1 closeout verification on 14 July 2026:

- targeted `MudSharpCore` build: **passed**, 0 warnings, 0 errors;
- vehicle-focused core tests: **passed**, 85 passed, 0 failed, 0 skipped;
- full `scripts\test-unit-core.ps1` suite: **passed**, 1,674 passed, 0 failed, 0 skipped;
- fresh-MUD runbook and release-candidate soak remain required before final release sign-off.

Employment V1 closeout verification on 14 July 2026:

- targeted `MudSharpCore` build: **passed**, 0 warnings, 0 errors;
- `MudSharpCore Unit Tests`: **passed**, 1,686 passed, 0 failed, 0 skipped;
- full `scripts\test-unit.ps1` default suite: **passed**, 2,601 passed, 0 failed, 0 skipped;
- no live MUD or release-candidate soak is claimed by the employment closeout.

## Scope Boundary

DatabaseSeeder features and content completeness were not catalogued or rated. Supporting library, EF model, migration, and seeder references were used only where necessary to determine whether a MudSharpCore runtime path had persistence or an operational entry point. Existing legacy TODOs outside the changed feature paths did not lower readiness ratings.
