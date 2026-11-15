# FutureMUD engine maturity audit

Original audit: 14 November 2026; refreshed: 15 November 2026

Source revision: `b32228996a95b745a6ad02298d582347b788ce7e` — fetched remote `master`, 14 November 2026; plus the legacy-tool retirement described in F18

Purpose: divide the engine into thematic features and identify incomplete functionality, weak assurance, documentation gaps and older implementations worth reviewing.

## Overall assessment

FutureMUD is a substantial, actively maintained engine with established implementations across most of its advertised simulation and worldbuilding domains. It is not uniformly complete or equally well qualified. Its most important weaknesses are **credential-policy hardening debt, uneven restart continuity, documentation that mixes historical and current claims, and limited evidence for some cross-system gameplay workflows**.

The best next investment is a focused hardening and documentation programme. The evidence does not support replacing the architecture wholesale. It also does not support calling the entire engine stable merely because many unit tests and design documents exist.

The clearest findings are:

1. **Password-change notifications can contain the new plaintext password.** The runtime supplies it and the stock template includes it. This behavior survives from the 2023 import. See F01.
2. **Passwordless Debug support login is intentional and accepted.** It enables reproducing player account/character problems without collecting their passwords. Debug builds are excluded from public releases. F02 is a documented support property, not a defect or remediation item.
3. **Employment task provenance and grants do not survive database reconstruction.** Financial tasks deliberately fail closed after losing their grants; tests explicitly expect this. The narrower implementation contradicts broader release-readiness wording about durable grants. See F03.
4. **A fatal Release startup/game-loop exception still exits with status zero.** Crash reports exist, but process supervisors cannot distinguish this failure from success using the exit code. See F04.
5. **Several apparent missing features are actually stale documentation.** Land gathering and magic-resource prompts exist. Conversely, some legacy APIs really remain throwing placeholders but have no current callers. See F06–F08.
6. **The strongest “untouched for years” signal is at method/feature level.** Broad cleanup and Room terminology changes make many old implementations look recently maintained. Specific 2023 branches remain in perception contracts, herd AI, time zones, language presentation and legacy tooling. See the historical review queue.

### Scope and limits

This is an engine-wide **source, architecture, documentation, test-inventory and Git-history audit**, with deeper call-chain checks on selected high-value findings. It is not a line-by-line review of every implementation, a penetration test, or a new live-world stability campaign. The census covers all tracked C# files in the main runtime and shared library; thematic assessments also examine persistence, bootstrap/content products, selected tools and their integration surfaces.

No build, automated test suite, MySQL migration, fresh install, live Telnet scenario, external-model call or soak test was run for this audit. Test names below identify existing evidence and useful verification seams; they are not newly passing results. Historical acceptance reports are treated as dated evidence, not current certification.

Remote `master` was fetched and the checkout fast-forwarded from `2d0935518` to `b32228996` on 15 November 2026. The engine now targets `net11.0`. The refresh includes the intervening staged-performance/rites prototype, Armageddon RC1 qualification and startup/prog-consolidation changes. An isolated checkout was used for bounded security/delta review; the full original thematic investigation was not repeated line by line. The source census was recomputed. The pre-existing `MudSharp.sln` edit was preserved byte-for-byte.

## How to read the assessments

**Maturity** describes implementation breadth and integration:

- **Established:** substantial runtime, authoring and persistence paths exist; normal use is supported.
- **Developing:** usable substantial functionality exists, with significant limitations or less qualification.
- **Partial:** an intended capability has explicit missing pieces.
- **Legacy/scaffold:** an older support path or prototype needs a retain/retire decision.

**Completeness is relative to the supported scope**, not every imaginable extension. A linear vehicle system can be complete within its V1 contract while lacking arbitrary 3D navigation. A builder-configurable bank can be complete even when the generic seeder does not create a bank branch in a specific world.

**Stability evidence** is separate:

- **Stronger:** focused behavioral tests and/or dated integration evidence were located for important seams.
- **Mixed:** tests exist, but significant runtime, persistence or cross-system paths still need qualification.
- **Thin:** little focused lifecycle evidence was located for the reviewed feature.

None means “verified stable in this audit.” There are no inferred completion percentages or code-coverage percentages. Directory size, commit count, absence of TODOs and test-file count are not maturity scores.

Findings distinguish **confirmed source behavior**, **documented limitation**, **documentation drift**, and **review risk**. Priorities are recommended work order: P1 important security/reliability or core continuity; P2 targeted qualification/docs; P3 conditional extension or legacy cleanup. No P0 emergency is inferred without deployment evidence. P1 includes reliability and continuity work; it does not automatically mean an exploitable security issue or a reason to hold disclosure. The disclosure assessment below uses the narrower criterion of unauthorized direct access.

## Pre-disclosure security assessment

The 15 November follow-up uses a narrower security threshold than the general maintenance priorities: an attacker-controlled path that can grant unauthorized account, staff or server access in the supported Release deployment. **No such path was confirmed in the bounded source review.** This is not a penetration-test result or a claim that all possible entry points are safe.

| Surface | Follow-up result | Disclosure disposition |
| --- | --- | --- |
| Release account login and recovery | Release requires password verification; recovery proof is bound to the named account and its current salt. Successful password replacement rotates that salt, invalidating the previously issued code hash even if the old recovery-code column is not cleared. In-flight recovery sessions and token lifecycle remain hardening questions. | No demonstrated anonymous login or completed-reset token replay. Do not describe a non-null old hash alone as a usable credential. |
| Debug support instances | Passwordless access is the owner's deliberate support workflow; Debug builds never form part of public releases. | Accepted behavior; original finding withdrawn. |
| Discord remote operations | Bridge authentication, current linked-account status/authority and published-hook permissions are checked. | No direct low-authority bypass established. Trusted published hooks remain an administrator capability. |
| TerrainPlanner API | Password authentication, account/authority floor, endpoint authorization and cookie-principal revalidation are present. | No anonymous/admin bypass established in the inspected paths. |
| Web client/proxy and publishing API | Proxy upstream is configured rather than client-selected; MUD login remains required. Publishing routes require configured bearer authentication before their handlers. | No direct unauthorized access established in the inspected paths. |
| Newly added performances, stage observation and possession-related delta | Native command/venue authority and output visibility checks were traced. Stage-link FutureProg functions remain trusted scripting capabilities rather than ordinary player commands. | No direct player-to-staff escalation established; keep trusted authored-prog policy explicit. |
| Plaintext password notification, legacy password hashing, strength/expiry/throttling policy | Concrete hardening debt, with credential possession, mailbox/storage access or another precondition needed for the identified access consequences. | F01/F05 are P2 work, not confirmed direct-entry vulnerabilities requiring an automatic disclosure hold. |
| Legacy Watcher and Upgrade Service | The owner confirmed that both are obsolete. Their source projects and solution entries are removed by this review. | Resolved by retirement from the maintained repository. External installations are not modified by repository cleanup. |

The employment reload limitation is fail-closed, and the fatal-exit issue concerns reliability/monitoring. Neither was established as a direct-access exploit. The requested legacy service retirement removes their source and build entries; no gameplay/authentication fixes were made. Publishing this report does not extend the source-only security assessment into deployment verification. Real-world deployment exposure remains outside this source-only assessment.

## Thematic feature map

These are logical ownership boundaries for planning and review. They are not a proposal to turn every row into a separate assembly or service. Commands, effects, persistence and seeders cross these boundaries and must remain part of each feature's acceptance criteria.

### 1. Engine platform and developer foundations

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Game loop, clocks, scheduling and heartbeats — `Framework`, `Server` | **Established; bounded.** Serial world execution, stable schedule heaps and bounded dispatch are implemented. Individual callbacks can still block or fail the loop. | **Stronger** for scheduling seams: `RuntimeSchedulerTests`, `SchedulerTests`; mixed for sustained world load. | Good [runtime guide](Game_Loop_and_Scheduling_Runtime.md). **P1:** fatal exit semantics (F04). **P2:** slow/throwing callback qualification (F09). |
| TCP/Telnet, connection admission and output — `Network` | **Established.** Async read/write pumps, command backpressure, output bounds, trusted-proxy handling and controlled shutdown exist. TLS termination is external to this transport. | **Stronger** focused network workflow/state tests; no new slow-client or connection-load campaign. | Good runtime guide and [client deployment guide](../../MudClient/DEPLOYMENT.md). **P2:** exercise limits, malformed frames, reconnects and shutdown under load. |
| Saving, lazy loading, persistence and upgrades — `Framework/Save`, `Database`, persistence library | **Established and recently changing.** Deferred saves, failure recovery, migration coordination and backup/restore mechanisms exist. EF provider upgrade is the audited head. | **Stronger** focused `SaveManagerFailureRecoveryTests` and persistence/upgrade suite; real MySQL behavior remains a separate gate. | Good [upgrade coordination guide](Database_Upgrade_Coordination.md). **P1/P2:** qualify upgrades and rollback on realistic databases, and field-level reload contracts such as F03. |
| Effects and event lifecycle — `Effects`, `Events` | **Established.** Saved effects, scheduling, dispatch and many native integration contracts; completeness varies by concrete effect. | **Mixed/stronger** for selected lifecycle and hook tests. Not every registered effect has been round-tripped here. | Distributed through [event contracts](../AI/Event_System_for_AI_and_Hooks.md), health, magic and world docs. **P2:** use registration/load/save/expiry/reentrancy as a common acceptance matrix. |
| FutureProg language and runtime — `FutureProg` in both projects | **Established and extensive.** Types, compilation, lexical scope, recursion protection, collections, persistence and built-ins exist. Some modes intentionally remain uncached. | **Stronger:** runtime, security, documentation and benchmark suites exist. Synchronous execution still limits isolation. | Good [execution](FutureProg_Execution_Runtime.md), [type-system](FutureProg_Type_System.md) and [language](../../FutureMUDLibrary/FutureProg/LANGUAGE_GUIDE.md) references. **P2:** operational execution budgets and serial/thread assumptions (F09). |
| Expressions and shared framework helpers — `ExpressionEngine`, library `Framework` | **Established.** Small expression adapter and broad reusable formatting/parsing/collection utilities. | **Stronger** focused helper tests and documented benchmarks; two expression test source files do not imply only two cases. | Good [framework contracts](Framework_Extensions.md); expression behavior is mainly code/tests. **P2:** extend behavioral coverage when formula boundaries change; preserve compatibility. |
| Diagnostics, logs, exception reporting and performance monitoring — `Logging`, `Server`, `Framework/Diagnostics` | **Established.** Context-rich reports, queued background notifications and in-memory performance/census tools exist. | **Stronger** formatter/reporting tests; mixed operational evidence. | Good runtime guide. **P1:** F04. **P2:** service-manager behavior, disk-full handling, retention and production monitoring requirements. |
| Builder command platform, revisions, help and documentation export — `Commands`, `Help`, `Documentation` | **Established.** Registrations, permission checks, editable helpers, revision workflows, preflight and export are substantial. Domain commands remain uneven. | **Stronger** command security/help tests; not a substitute for full editor walkthroughs. | Good [generic workflows](../Building/Generic_Editable_Item_Builder_Workflows.md) and [publishing guide](FutureMUD_Website_and_Publishing.md). **P2:** current capability/dependency limitations and documentation discoverability. |

### 2. Accounts, characters and character development

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Accounts, registration, login and recovery — `Accounts`, `Menus` | **Established workflows with credential-policy hardening debt.** Debug support login is intentional; legacy notification/recovery policy merits a separate review. | **Mixed/thin** for full account lifecycle. A hash-compatibility test does not qualify password security. | Security information is fragmented. **P2:** F01/F05 hardening; write one account-security/operations contract. F02 is accepted behavior. |
| Character creation, roles, resources and approval — `CharacterCreation` | **Established; broad.** Configurable stages, skippers, skill groups, approval and role integration exist. | **Stronger** focused reorder, skipper, knowledge and group tests; whole application/reconnect/restart flow unrun. | Good [chargen index](../Characters/Character_Creation_Documentation_Index.md), runtime, builder and seeder guides. **P2:** complete end-to-end templates for supported application paths. |
| Identity, character lifecycle and archives — `Character`, `Menus`, related loaders | **Established.** Login/logout, retirement, archives and physical-instance relationships are implemented. | **Mixed/stronger** for lifecycle guards and cleanup; integrated offline/reboot cases still matter. | Split across character/body-form docs and command help. **P2:** join identity, online state, archives and retirement into an operational lifecycle reference. |
| Anatomy, races, culture and physical bodies — `Body`, `Character/Heritage` | **Established and highly configurable.** Body prototypes, traits, movement capabilities and racial content are extensive; some public API contracts are unfinished. | **Stronger** body/template/perception tests; dormant API holes remain (F07). | Good [body builder guide](../Building/Body_Prototype_Builder_Guide.md) and chargen/seeder material. **P2:** supported API contract census; trace or retire unused members. |
| Multiple body forms, simultaneous instances and possession | **Established within a declared V1 scope.** Provisioning, focus, secondary lifecycle, cloning and authority/cleanup safeguards exist. | **Stronger** focused provisioning/lifecycle/policy tests and historical readiness evidence. | Good [forms](../Characters/Multiple_Body_Form_System.md) and [simultaneous-instance design](../Characters/Multiple_Simultaneous_Body_Instances_Design.md). **P2:** cross-system death, combat, vehicle, project and offline identity campaigns. |
| Skills, attributes, learning and knowledges — `RPG`, body traits | **Established mechanics; documentation incomplete.** Player inspection, skill/knowledge editors, teaching and progression exist. | **Mixed/stronger** progression, NPC learning, knowledge and chargen tests. | Focused [role progression](../Characters/Role_Skill_Progression.md), NPC packages and seeder docs; **no dedicated comprehensive runtime/builder guide located**. **P2:** F12. |
| Checks, decorators, merits and flaws — `RPG/Checks`, `RPG/Merits` | **Established; broad factory/authoring surface.** Completeness should be judged per registered type. | **Mixed/stronger** check/expression and selected merit behavior tests. | [Check editor guide](../Building/Check_and_Decorator_Editors.md) and [racial merits](../Characters/Intrinsic_Racial_Merits.md) exist; central semantics and general merit catalogue are thinner. **P2:** F12. |

### 3. World structure, navigation and environmental simulation

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Rooms, exits, layers, terrain, zones and spatial ownership — `Construction` | **Established, recently refactored.** Builder-facing Room identities, legacy wire compatibility, layers and spatial ownership are real systems. Dynamic flooding remains an explicit omission. | **Stronger** room/layer/spatial tests; migration and large-world topology are major integration boundaries. | Good [room guide](../Building/Room_Building_Builder_Guide.md), [layer primer](../World/Room_Layer_System_Primer.md). **P2:** older/delegated paths after terminology migration; F13 for water semantics. |
| Autobuilding and terrain planning — `Construction/Autobuilder`, TerrainPlanner | **Established supported types; some legacy placeholders.** Current feature creation supports simple/uniform/road groups; the old river loader branch is not a supported builder feature. | **Mixed/stronger** planning/contract and room tests; invalid persisted templates merit checks. | Good [autobuilder how-to](../Building/Autobuilder_Builder_How_To.md) and [planner/API guide](../Building/Terrain_Planner_and_Engine_API.md). **P3:** F07 river loader cleanup/migration diagnostics. |
| Movement, parties, following, positions and stamina — `Movement`, body/character movement | **Established.** Rich native movement and group behavior; Party exposes some unused proxy-contract stubs. | **Mixed/stronger** movement, posture, stamina and security tests. | [Position states](../World/Position_State_System.md), layer and movement-specific docs; not one consolidated movement guide. **P2:** party/follower cancellation, callbacks and inter-system transitions. |
| Pathfinding, topology and route compilation — construction pathing and spatial services | **Established.** Incremental/live topology and route-aware paths are implemented. | **Stronger** live-mutation and path service tests plus dated acceptance evidence. | Good [pathfinding guide](../World/Pathfinding_System.md). **P2:** large topology mutation/portal invalidation campaigns; avoid replacing this with a second path system. |
| Mounts, riding, stables, hitches and towing — body/commands/vehicle integrations | **Established, with integration debt.** Mount equipment, stables, mixed hitch graph and towing exist; service-level consent/recovery design has unfinished refactoring. | **Mixed/stronger** mounted combat, landing, gear, stable and towing tests. | [Mounted combat](../Combat/Mounted_Combat.md) and [vehicle guide](../Vehicle_System.md); mount/stable workflows are spread across sources. **P2:** one builder/player guide and consent/recovery campaign. |
| Vehicles, interiors, routes, services and journeys — `Vehicles` | **Established within V1.** Manual exits, linear RouteCell movement, persistent interiors, services and restart checkpoints exist. Arbitrary 2D/3D navigation is outside V1. | **Stronger** focused tests and [dated live/reboot evidence](../Verification/RouteCell_Vehicle_V1_Acceptance_Evidence.md). Actual persistence adapter has less identified direct recovery coverage than strategy fakes. | Extensive [system guide](../Vehicle_System.md) and [fresh-world runbook](../Vehicle_System_Fresh_MUD_Test_Runbook.md). **P2:** F10; distinguish historical campaign success from current regression protection. |
| Planes, corporeality, portals and zero gravity — `Planes`, related movement/magic | **Established bounded capabilities.** Visibility, physical state, portal topology and zero-gravity anchors/tethers exist. Collision simulation is outside the stated slice. | **Mixed/stronger** topology and selected security tests; many combinations require integration testing. | Good [planes](../World/Corporeality_and_Planes.md), [zero-gravity](../World/Zero_Gravity_System.md). **P2:** perception/combat/party/item/restart matrix. |
| Time, calendars, astronomy and celestials — `TimeAndDate`, `Celestial` | **Established; explicit civil-time omission.** Custom clocks/calendars and astronomical events are substantial; daylight-saving logic remains TODO. | **Stronger** focused calendrical/celestial tests and authored-celestial receipts. | Good [time/date](../World/Time_And_Date_System.md) and [celestial index](../World/Celestial_System_Overview.md). **P3:** decide whether DST is a supported requirement; F08. |
| Weather, climate and environmental exposure — `Climate`, health/environment integrations | **Established with unusually deliberate simulation verification.** Profiles, forecasting, hazards and exposure have native runtime paths. | **Stronger** dedicated slow climate suite, forecast/hazard tests and environmental verification docs; no new simulation here. | Good [weather](../World/Weather_System.md), [verification](../World/Weather_Verification.md), [exposure](../Environment/Environmental_Exposure_Design.md). **P2:** preserve benchmark-driven climate acceptance and qualify world-specific content. |

### 4. Roleplay presentation, communication and authored stories

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Perception, targeting and output audiences — `PerceptionEngine`, body perception | **Established normal paths; partial legacy contracts.** Visibility/hearing and target routing are extensive; unused generic sensing APIs still throw. | **Mixed/stronger** audio/look/targeting tests; generic API completeness differs from player-command completeness. | Distributed through item, health, world and markup docs. **P2:** F07; define what non-language perceivers receive (F08). |
| Emotes, descriptions, characteristics and markup — `PerceptionEngine`, `Form` | **Established and recently reviewed/optimised.** Parser, optional targeting, descriptions and substitution machinery exist. | **Stronger** markup/parser tests and [benchmark evidence](../../MudSharp%20Benchmarks/MarkupBenchmarkResults.md), subject to the exact recorded revision. | Good [emote](../Markup/Emote_System.md), [character](../Markup/Character_Description_System.md), [room](../Markup/Room_Description_Markup.md) references. **P2:** mixed audiences and invalid authored content. |
| Languages, accents, speech, signing and radio — `Communication`, output/component integrations | **Established.** Native language/accents and signed language have real runtime and authoring paths. Some presentation policy remains old/hardcoded. | **Mixed/stronger** native-language/signing/audio tests; radio/device composition is a separate lifecycle boundary. | Good [languages/accents](../Communication/Native_Languages_and_Accents.md), [signing](../Communication/Sign_Language_System.md). **P3:** F08 raw text and word-mask policy. |
| Writing, books, literacy and player boards — communication/item/community integrations | **Established basic workflows; uneven qualification.** Persistent written content and boards exist. | **Mixed/thin** for full board posting/edit/delete/permissions/reload lifecycle; `BoardPresentationSecurityTests` covers a narrower concern. | Writing is covered inside item docs/catalogues and command help; **no dedicated broad board lifecycle guide located**. **P2:** F12 and targeted lifecycle tests. |
| Scripted login events and questionnaire content — `RPG/ScriptedEvents`, menus | **Established narrow feature, not a general quest engine.** It presents authored questions and executes related progs in a login/player workflow. | **Thin:** no dedicated scripted-event lifecycle suite located in the reviewed inventory. | Storyteller command help exists; **no dedicated current runtime/builder guide located**. **P2:** F12. A general mission/quest framework was not found in this snapshot; treat one as a separate product choice. |
| Staged performances, rites and audience stage links — `RPG/Performances` | **Developing persisted prototype.** Reusable works, plans, booking, cast/prop/place binding and typed native cues exist. Broad employment-task integration and richer casting policy remain deferred. | **Mixed:** new authoring/booking/action/returns/observation tests and dated rehearsal notes; no new live restart campaign here. | Good [current-state design](Staged_Performances_and_Rites.md), [builder guide](../Building/Staged_Performance_Builder_Guide.md) and seeder guide. **P2:** qualify ordinary-player authority, venue control, interruption/restart and audience visibility. |

### 5. Items, physical interaction and technology

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Item prototypes, components, revisions, skins, groups and templates — `GameItems` | **Established and very broad.** Composition and persistent item lifecycle are core engine strengths. Component completeness is not uniform. | **Stronger** selected registration/exclusivity/ownership/morph/serialization tests; this audit did not inspect all 630 runtime source files in depth. | Good [item overview](../Items/Item_System_Overview.md), runtime, authoring and content workflow set. **P2:** catalogue supported component contracts and verify component clusters, not raw stub counts. |
| Inventory, containers, clothing, connections and manipulation — body/item/command integrations | **Established and recently reviewed.** Physical custody, reach/access, holding/wearing and connection rules are extensively integrated. | **Stronger** focused inventory/manipulation/security tests and [physical-manipulation audit](../Items/Physical_Manipulation_Audit.md). | Good [item runtime model](../Items/Item_System_Runtime_Model.md). **P2:** callbacks, nested custody, offline ownership and cross-plane interactions. |
| Materials, liquids, gases, contamination, food and cooking | **Established.** Constituent mixtures, surfaces, ingestion and crafting/item integrations exist. Stock catalogue coverage is a separate question. | **Mixed/stronger** liquid/surface/food and craft tests; arbitrary authored combinations remain a large state space. | Good item/health/[food guide](../Crafting/Food_and_Cooking_System.md), generated material inventories. **P2:** serialization, quantity conservation and invalid content. |
| Electrical/liquid grids, power distribution and telecom networks — construction grids/components | **Established bounded/legacy implementation.** Membership persistence and power cutout/cutin logic exist; full network physics/topology is not promised. | **Mixed:** `GridSystemTests` covers meaningful producer/component behavior; direct demand-shedding/priority/reconnection tests were not located. | Scattered item runtime and technology docs. **P2:** F11 allocation and connector-policy matrix. |
| Electronics, sensors, microcontrollers and access control — components/computers | **Developing to established within the shipped slice.** Service access, signals, automation, credential readers and powered hosts exist. | **Mixed** focused tests plus detailed manual end-to-end scenarios; document existence is not execution evidence. | Good [automation scenarios](../Technology/Automation_End_To_End_Test_Scenarios.md). **P2:** power loss, broken routes, credentials, administrator-vs-player actions and restart. |
| Computers, programs, terminals, storage, mail/FTP/boards and media — `Computers`, components | **Developing; substantial but deliberately bounded.** Persisted programs and built-in applications exist; Messenger is intentionally reserved without an executor. | **Mixed:** workspace, media-app and FTP-backoff tests; broad host/network/process lifecycle remains a review target. | Detailed [computer design](../Technology/FutureMUD_Computer_Programs_Concept_Design.md), [media workflows](../Technology/Media_System_Runtime_and_Workflows.md) and scenarios. **P2/P3:** F14, with reserved capabilities clearly labelled. |

### 6. Conflict, health and survival

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Melee, ranged combat, strategies and combat settings — `Combat` | **Established, extensive and actively maintained.** Native strategy/response matrix is broad; defensive throw branches and legacy edge policies deserve targeted review. | **Stronger** simulation, strategy, action-availability, messaging and target-retention tests; not every combination is qualified. | Good [strategy runtime](../Combat/Combat_Strategy_Runtime.md), mounted/ranged/defaults docs. **P2:** F15 response matrix, target changes and custom strategy contracts. |
| Weapons, ammunition, poison, natural attacks and traps — combat/items/`Traps` | **Established scoped features.** Native ranged attacks, generic ammunition damage and trap definitions/activation exist. | **Stronger** natural-ranged, poison and explosive-trap tests plus historical closeout evidence. | Good [natural ranged](../Combat/Natural_Ranged_Attacks_Design.md), [trap system](Trap_System.md), firearms and balance docs. **P2:** authored combinations, activation/recovery/restart and NPC selection. |
| Arenas and organised combat — `Arenas` | **Established bounded system.** Scheduling, lifecycle, preparation, observation, finance, betting, ratings and cleanup exist. | **Stronger** dedicated arena tests; no new full live event campaign. | Substantial design, but [implementation plan](../Combat/Combat_Arenas_Implementation_Plan.md) still opens “Draft for implementation handoff.” **P2:** current support/operations guide and restart/refund/inventory campaign; F06. |
| Wounds, damage, healing, infection, physiology and survival — `Health`, body integrations | **Established and deeply integrated.** Strategies, healing, infection, thermal exposure and survival consequences are implemented. | **Stronger** focused health/wound/body tests and damage reports; long elapsed-time/offline scenarios remain useful. | Good [health runtime](../Health/Health_Core_Runtime.md), [damage design](../Health/Damage_System_Design.md), medical and seeder docs. **P2:** balance and interruption/restart qualification; do not infer missing infection from one TODO (F16). |
| Surgery, medicine, drugs, dependence and disfigurement | **Established scope; uneven end-to-end assurance.** Procedures, treatment items, metabolism/dependence and markings are substantial. A legacy examination-proposal class is unused. | **Mixed/stronger** drug metadata/runtime/dependence and selected treatment tests; full medical campaigns are not established by those alone. | Good [medical interactions](../Health/Health_Medical_Interactions.md), [drugs](../Drugs/README.md), [disfigurement](../Health/Disfigurement_System_Design.md). **P2:** multi-stage treatment, offline time and clone/reload; F07 orphan proposal. |

### 7. Society, law and economy

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Clans, ranks, appointments, elections and external control — `Community` | **Established and actively maintained.** Governance and permissions are native systems; organisation-specific policy is configurable. | **Stronger** dedicated election/appointment/recall/address/persistence/security tests. | Good [command/permissions](../Economy/Clan_Command_Surface_and_Permissions.md), [elections/control](../Economy/Clan_Elections_and_External_Control.md). **P2:** complete real governance transitions and offline/restart scenarios. |
| Crime, law, enforcement, custody, trials and sentencing — `RPG/Law`, NPC/commands | **Established.** Arrest/custody, handover, court progression and sentencing paths align with current documentation. No concrete defect established in this pass. | **Stronger** `LegalTrialFlowTests`, `AutomaticSentencingTests`, patrol/docket/handover tests; live case progression unrun. | Good [law runtime](../Economy/Law_System_Runtime.md) and seeder guidance. **P2:** full seeded authority-to-sentence/release campaign. |
| Currency, markets, taxes, shops and trade — `Economy` | **Established.** Native money, market pressures, merchandise, settlement and tax authoring exist. A turnkey world economy is not seeded automatically. | **Stronger** selected purchase, shop, haggle, market and financial-boundary tests; older tax logic is a review candidate. | Good [economy runtime](../Economy/Economy_System_Runtime.md), workflows and seeder gaps. **P2:** tax periods, cross-calendar settlement, duplicate/failed transfers and F08 aging review. |
| Banking, loans, mortgages, pawn and autopay | **Established banking; newer lending extensions need qualification.** Native accounts and optional secured/unsecured lending are implemented at this head. | **Mixed/stronger** dedicated financial tests, with very recent changes; no full loan lifecycle/restart campaign in this audit. | Economy docs and [lending design](../Economy/Loans_and_Secured_Lending_Design.md) require implementation-vs-proposal status to remain clear. **P2:** default, collateral ownership, interest, precision and restart/retry. |
| Property, rentals/hotels, auctions, estates and probate | **Established, with broad persistence/ownership consequences.** The native features exist and have recent maintenance. | **Mixed/stronger** property/security, auction settlement and estate-focused tests; complete death-to-distribution workflows remain a high-value campaign. | Good economy set and [estate/ownership guide](../Economy/Estate_Probate_and_Item_Ownership.md). **P2:** insolvency, stale owners, duplicated settlement and interrupted transfers. |
| Employment, manager goals, dispatch, worker AI and payroll | **Established runtime with a material persistence limitation.** Host/action/payment matrix, consent, goals, hiring and payroll exist; active-task provenance/grants are runtime-only. | **Mixed/stronger** large functional suite, including an explicit fail-closed grant reload test. It does not prove durable grant continuity. | Extensive [employment roadmap](../Economy/Employment%20Hosts%20Implementation%20Review%20and%20Roadmap.md), but contradictory status claims remain. **P1:** F03 supported restart contract. Autonomous management/QC extensions are separately deferred. |
| Hospitals and institutional services — employment/native health hosts | **Established supported workflows.** Treatment dispatch, rooms, supplies, billing and native medical integration exist. | **Mixed/stronger** hospital billing/room and dispatch tests; player treatment plus worker/equipment/restart continuity needs integration evidence. | Split between health and employment guides. **P2:** a joined setup/operations guide and interruption/custody/billing campaign. |

### 8. Work, production and ecology

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Crafting, recipes, tools, analysis and product routing — `Work/Crafts` | **Established and extensible.** Revisioned crafts, input/tool/product factories and analysis exist. An old quality-formula compatibility fallback persists. | **Stronger** input persistence, product routing, tool and analysis tests. | Good [crafting overview](../Crafting/Crafting_System_Overview.md), runtime and builder guides. **P2:** F08 fallback rationale and production-chain/restart acceptance. |
| Projects, phases, labour, materials and long-running work — `Work/Projects` | **Established.** Personal/local projects, queues, requirements and completion actions exist. Unknown-type factory throws are defensive. | **Mixed/stronger** queue, spatial, labour and worker-AI tests; whole phased lifecycle unrun. | Good [project overview](../Crafting/Projects_System_Overview.md), [builder workflows](../Crafting/Projects_System_Builder_Workflows.md). **P2:** pause, reassignment, invalidated materials/rooms and restart. |
| Agriculture, crops, herds, woodland, apiaries and primary production | **Established scoped runtime; balance/stock qualification varies.** Daily ticks, operations, commodities and organic resource accounting exist. | **Stronger** selected agriculture/native-accounting tests and dated native persistence evidence; no multi-season campaign here. | Good [overview](../Agriculture/Agriculture_System_Overview.md), runtime and builder set. **P2:** seasonal simulation, ownership, ecology and production-chain qualification. |
| Foraging, butchery and renewable-resource harvesting | **Established individual mechanisms.** Integration with fields, native resource stock and authored content varies. | **Mixed** `ForagingRuntimeTests`, butchery and native-organic tests. | [Butchering guide](../Crafting/Butchering_System.md), agriculture/item/seeder notes; **no single comprehensive current foraging guide located**. **P2:** F12, depletion/recovery and authored-stock campaigns. |

### 9. Magic and artificial intelligence

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Magic resources, capabilities, generators and native powers — `Magic` | **Established platform; configurable behavior is broader than stock content.** Some metadata has no generic runtime rule; abstract placeholders are not registered features. | **Stronger** focused magic/factory/persistence tests; no all-power live campaign. | Extensive [magic overview](../Magic/Magic_System_Overview.md) and capability/power docs, with specific stale claims (F06). **P2:** current support matrix and authoring examples. |
| Spells, configurable casting, Vancian magic, scrolls and magic items | **Established/developing by slice.** Significant native lifecycle, payment, access and compatibility work exists; optional setting repertoires and balance are separate. | **Mixed/stronger** configurable/Vancian/scroll/owned-object tests and dated acceptance matrices. | Good [configurable casting](../Magic/Configurable_Casting.md), [Vancian runtime](../Magic/Vancian_Magic_Runtime.md), builder/player guides. **P2:** real installed-world cast/payment/save/reload/expiry campaigns on the intended product revision. |
| Environmental magic, gathering and land ecology | **Established scoped implementations.** Land player selection and committed gathering exist; earlier verification notes describe only a predecessor slice. | **Stronger** gathering/land witness/native-accounting tests and dated native evidence. | Good [gathering](../Magic/Magic_Gathering.md), [regional land](../Magic/Regional_Land_Gathering.md), environmental set. **P2:** reconcile historical boundaries (F06); qualify ecological and legal/NPC policy per configured method. |
| Individual NPC AI, group AI, ecology and event hooks — `NPC` | **Established catalogue with legacy exceptions.** Native registries, builders, persistence, pathing and event subscriptions are substantial. `WildAnimalHerdAI` needs a separate retain/retire review. | **Mixed/stronger** selected commandable/sentry/worker/predator/event tests; not every AI type has comparable evidence. | Good [NPC/group runtime](../AI/NPC_AI_and_Group_AI_Runtime.md), predator/monster guides and local authoring instructions. **P2:** F08 old branches, LOS policy and group long-run behavior. |
| AI Storyteller and external text assistance — `NPC/AIStorytellers`, `OpenAI` | **Developing, operationally bounded.** Persisted storytellers, queues, tool loops, memories, pause/cancellation, output limits and game-loop application exist. Provider-backed quality/reliability remain external dependencies. | **Stronger** focused adapter/tool-loop tests; **mixed** external-provider/long-running evidence. | Good [storyteller design](../AI/AI_Storyteller_Design.md), implementation plan and [integration guide](../AI/OpenAI_Integration.md). **P2:** controlled provider-backed outage/rate-limit/pause/restart/cost campaign; F09 synchronous tools. |

### 10. Installation, content and companion products

| Area and source ownership | Maturity and completeness | Stability evidence | Documentation and next review |
| --- | --- | --- | --- |
| Database bootstrap, shared foundations and seeder workflow — `DatabaseSeeder`, `SeederFoundations`, `SeederFramework` | **Established.** Dependency/question infrastructure, schema/bootstrap and foundational packages exist. Replay guarantees must remain per package. | **Stronger** dedicated tests; no full fresh-install/rerun matrix here. | Substantial seeder docs and instructions; the older system-gap audit has obsolete product/era descriptions (F06). **P2:** verify maintained baseline and ownership preservation. |
| Optional item catalogue and Primary Production — `ItemSeeder` | **Established product; catalogue acceptance is separate from engine maturity.** Compiled catalogue, manifests and authoring are substantial. | **Mixed/stronger** dedicated product tests; large catalogue size does not prove every item/craft playable. | Extensive catalogue/source/design material, separate solution/workflow. **P2:** functional production chains, stock availability and builder-edit preservation. |
| Optional Armageddon setting and Worldfile installation — `ArmageddonSeeder` | **Release-candidate qualified within a recorded slice.** A substantial setting-specific installer, deliberately outside the generic engine/normal solution graph. | **Mixed** dedicated tests and handoffs; corpus-dependent and live installation paths require their own fixtures. | Extensive dated setting docs, including [RC1 qualification](../Seeding/Armageddon_Release_Candidate_1.0_Qualification_20261113.md) and later startup/prog-consolidation records. Those results belong to their stated candidate revisions, not automatically every later commit. **P2:** use the product's exact acceptance revision and source corpus. |
| Legacy RPI Worldfile Converter | **Partial by declared design.** A conversion tool for already-seeded worlds, not an engine replacement or universal migration. | **Mixed** parser/import fixtures and dedicated suite; import order and target-world assumptions matter. | Dedicated project instructions/mapping docs. **P2:** publish supported import matrix; retain explicit exclusions. |
| Discord bridge and standalone bot — runtime `Discord`, `DiscordBotCore` | **Established scoped integration.** Protocol, authentication, linking and remote/admin operations exist. | **Stronger** protocol/auth/routing tests; one test source file can contain many cases. No live Discord campaign here. | Good [protocol/settings](Discord_Bot_Protocol_and_Settings.md), [remote operations](Discord_Remote_Operations.md). **P2:** live authorization, reconnect, malformed payload and service outage. |
| Web client and WebSocket proxy — `MudClient` | **Established companion deployment; assurance gap in ordinary CI.** Proxy, browser client, signed updater and rollback layouts are documented. | **Mixed/stronger** dedicated tests and release gate; client suite is absent from default PR test manifest. | Good [deployment guide](../../MudClient/DEPLOYMENT.md). **P2:** F17, browser/Telnet/TLS/proxy upgrade campaign. |
| Terrain Planner and engine API — `TerrainPlanner` | **Established companion scope.** Planner contracts, server/client and import/export integrations exist. | **Stronger** dedicated tests in default manifest; actual deployed API/browser workflow unrun. | [Planner/API guide](../Building/Terrain_Planner_and_Engine_API.md), [deployment](../../TerrainPlanner/DEPLOYMENT.md). **P2:** authorization, import compatibility and large plans. |
| Website, documentation publishing and product releases — `FutureMUD.Web`, workflows | **Established infrastructure.** Product manifest, release storage, publishing boundaries and documentation export are explicit. | **Stronger** website tests/security workflow and product release gates; no hosted state verified here. | Good publishing/release docs. **P2:** keep release gates distinct from ordinary runtime tests; audit current deployment separately. |
| Legacy WPF configuration tool | **Legacy; outside this retirement.** The configuration tool still uses .NET Framework 4.8/EF6-era dependencies and needs its own retain/retire decision. The Watcher and Upgrade Service have been removed (F18). | **Thin:** no dedicated current lifecycle suite located; not an active release product. | [Retirement record](Retired_Legacy_Tools.md) distinguishes removed tools from this remaining project. **P2/P3:** assess the configuration tool separately; it was not requested for deletion. |

## Confirmed findings and material limitations

### F01 P2 — Password-change email includes the new password

**Classification: confirmed source behavior, high confidence.**

[`LoggedInMenu.CommitPassword`](../../MudSharpCore/Menus/LoggedInMenu.cs#L882) saves the new hash/salt, then calls `SendEmail(AccountPasswordChanged, ..., command)` at lines 903–904, passing the confirmation input as a template argument. The stock [`CoreDataSeeder` template](../../DatabaseSeeder/Seeders/CoreDataSeeder/CoreDataSeeder.cs#L427) contains `{2}` in the sentence giving the new password. [`EmailHelper`](../../MudSharpCore/Email/EmailHelper.cs#L195) formats those arguments into the message body before queueing it.

This exposes a usable password to the configured mail transport and mailbox. Failed-message storage may also retain that body when enabled. Actual exposure depends on the installed template and enabled transport; custom templates may already omit it, and Debug suppresses delivery. The runtime still supplies the sensitive argument.

**Recommended closure:** remove the password from runtime notification inputs and stock templates, with an ownership-safe update path for existing stock templates. Keep a change notification that includes no credential. Verify with a synthetic message and fake transport/failure store; never use a real password. Add a test asserting that notification construction cannot include the replacement password. Both sender and stock template trace to 7 April 2023.

### F02 Accepted — Passwordless Debug support login is intentional

**Classification: accepted product behavior; no fix recommended.**

[`Futuremud.LogIn`](../../MudSharpCore/Framework/Futuremud.cs#L238) checks account existence under `#if DEBUG`, while the Release branch additionally verifies the password. This is deliberately used to reproduce player-reported account or character problems in a Debug instance without asking players to disclose passwords. The owner confirms that Debug builds are never included in public releases.

The original audit incorrectly treated this as a defect and recommended changing it. That finding and recommendation are withdrawn. Public-release authentication remains the relevant security boundary for this review.

### F03 P1 — Employment grants and task provenance are runtime-only

**Classification: confirmed persistence limitation and contradictory documentation, high confidence.**

The trace is complete enough to distinguish intended fail-closed behavior from a permission escalation:

1. [`EmploymentActiveTask`](../../MudSharpCore/Economy/Employment/EmploymentTaskBoard.cs#L4683) exposes source rule/goal, principals, grant, priority and lifecycle timestamps through `Provenance`.
2. [`SaveActiveTask`](../../MudSharpCore/Economy/Employment/EmploymentPersistenceStore.cs#L645) does not write those fields. The [`record model`](../../MudsharpDatabaseLibrary/Models/EmploymentHostState.cs#L311) and active-task mapping in the current snapshot do not provide them.
3. [`ToActiveTask`](../../MudSharpCore/Economy/Employment/EmploymentPersistenceStore.cs#L3028) reconstructs without provenance. The loaded-task constructor supplies compatibility HostSystem provenance, zero priority, new timestamps and an empty grant.
4. [`TaskGrantAuthorisesStep`](../../MudSharpCore/Economy/Employment/EmploymentTaskBoard.cs#L5045) checks authority, payment source, counterparty and amount limits. Empty compatibility grants fail those checks.
5. Existing [`EmploymentPersistence_LoadedTasksWithoutPersistedGrantFailClosedForFinancialAuthority`](../../MudSharpCore%20Unit%20Tests/Economy/Employment/UnifiedEmploymentDispatchTests.cs#L6645) deliberately asserts the lost provenance and blocked financial action. This test was inspected, not executed.

The [employment roadmap](../Economy/Employment%20Hosts%20Implementation%20Review%20and%20Roadmap.md) explicitly describes runtime-only provenance/grants at lines 196–204 and 1600–1611. The [2.0 readiness audit](MudSharp_2_0_Release_Readiness_Audit.md) instead calls durable provenance/grants complete. The narrower code/test contract is authoritative.

**Practical consequence:** a running workflow cannot assume that its task origin, priority, due dates or scoped financial approval survive reload. Financial safety is preserved by refusal, but continuity and audit attribution are reduced. This is not proof that payroll, settled wage ledgers or every employment record is lost; those have separate persistence.

**Recommended closure:** agree whether durable grants/provenance are required for the supported restart contract. If yes, implement versioned persistence and round-trip both permitted and denied grants, exact source/priority/timestamps, revocation and old-row compatibility. If runtime-only remains the intended scope, correct readiness wording and clearly expose post-restart blocked-work recovery to builders. Keep the fail-closed regression.

### F04 P1 — Fatal engine failures report success to the operating system

**Classification: confirmed source behavior, high confidence.**

The Release `Main` catches startup/game-loop exceptions and writes a crash report, then [`finally` calls `Environment.Exit(0)`](../../MudSharpCore/Server/Main.cs#L152). Exit zero is also used after normal completion. A supervisor relying on nonzero exit status, such as a failure-only restart policy, may not restart or alert for these crashes. The exact service policy was not inspected or tested.

**Recommended closure:** preserve a nonzero fatal exit status and a normal success status, with deliberate service restart policy. Test the published Release executable using an invalid startup configuration or isolated injected failure; assert report creation and process exit code. Do not use a production world to simulate this.

### F05 P2 — Account credential and recovery policy needs a joined review

**Classification: confirmed implementation constraints; operational impact needs qualification.**

- [`SecurityUtilities.GetPasswordHash`](../../FutureMUDLibrary/Framework/SecurityUtilities.cs#L19) uses one salted SHA-384 operation and decodes the resulting bytes as UTF-8. This is a legacy compatibility representation, not a versioned, deliberately expensive password-storage scheme. [`SecurityUtilitiesTests`](../../FutureMUDLibrary%20Unit%20Tests/SecurityUtilitiesTests.cs#L13) explicitly preserves it. A replacement needs a safe migration/rehash-on-login plan.
- Registration enforces eight characters at [`MainMenu.CreateAccountPassword`](../../MudSharpCore/Menus/MainMenu.cs#L404); [`EnterNewPassword`](../../MudSharpCore/Menus/LoggedInMenu.cs#L874) hashes the supplied value without the same check. Change/recovery can therefore use a weaker policy than registration.
- [`RecoverPasswordSendEmail` and code validation](../../MudSharpCore/Menus/MainMenu.cs#L240) persist a recovery hash but do not record or check expiry in that path; the account model exposes a recovery code without an expiry field. Clearing the code on successful validation has no explicit `SaveChanges` in that method. Completing a password change rotates the salt, so this is not a claim that the same code remains usable after a successful password replacement.
- Main-menu login/recovery paths do not show a per-account attempt budget or recovery-email cooldown. TCP connection/flood limits are a different protection. Logged-in old-password verification has a three-attempt disconnect and an older TODO about a ban; that is not a complete cross-session/account policy.

**Recommended closure:** define one supported credential/recovery contract, covering secure hashing migration, consistent strength validation, expiring single-use recovery, throttling and generic responses. Verify state transitions with isolated storage and fake email, including disconnect before completion and concurrent attempts. Retain existing-hash compatibility until migration is intentional. This audit does not certify the rest of authentication or claim an external exploit was reproduced.

### F06 P2 — Historical and current documentation are difficult to distinguish

**Classification: confirmed documentation drift.**

| Source text | Current source evidence | Required documentation disposition |
| --- | --- | --- |
| [Native Organic Yields Verification](../Magic/Native_Organic_Yields_Verification.md#L83) says Land command is not implemented “by this PR.” | [`MagicModule.Gathering`](../../MudSharpCore/Commands/Modules/MagicModule.Gathering.cs#L18) offers the player workflow; [`MagicGatheringService.Land`](../../MudSharpCore/Magic/Gathering/MagicGatheringService.Land.cs#L122) implements the commit. | Preserve the historical receipt, but add a prominent current-successor link. Do not report Land gathering as absent. |
| [Capabilities/resources guide](../Magic/Magic_System_Capabilities_Resources_and_Generators.md#L148) cites a prompt-support TODO as current status. | [`Character` prompt construction](../../MudSharpCore/Character/Character.cs#L3104) checks `IncludeMagic` and `ShowMagicResourcesInPrompt`; both classic/full prompt paths exist. | Correct the current guide and remove/update the stale source TODO. |
| Employment roadmap and release-readiness audit disagree about durable grants. | F03 confirms runtime-only grants and an intentional fail-closed reload test. | Publish one supported restart contract and mark historical completion claims precisely. |
| [Arena implementation plan](../Combat/Combat_Arenas_Implementation_Plan.md#L3) says Draft for implementation handoff. | The same document and runtime contain extensive implemented lifecycle/finance/cleanup behavior and tests. | Separate historical plan from current builder/operations contract. |
| [DatabaseSeeder system-gap audit](../Seeding/DatabaseSeeder_System_Gap_Audit.md#L25) lists ItemSeeder under DatabaseSeeder and says later eras are unavailable/no-op. | ItemSeeder is now a separate optional product with a compiled catalogue and later-era source. | Mark the old audit's revision and route current readers to the standalone product docs. Do not use it as the current installer roster. |

Documentation is plentiful: 529 tracked Markdown files were found under `Design Documents` before this report. The problem is often status drift and navigation, rather than complete absence. Historical test totals and one-worktree acceptance receipts should not be promoted into current whole-engine assurances.

### F07 P2/P3 — Genuine throwing placeholders mostly belong to dormant contracts

**Classification: confirmed incomplete code, with limited or no established gameplay reachability.**

| Path | Finding and reachability | Disposition |
| --- | --- | --- |
| [`BodyPerception.CanSense`](../../MudSharpCore/Body/Implementations/BodyPerception.cs#L77), delegated by `Character.CanSense` | Unconditionally throws. Full source search found declarations/implementations/delegation, but no normal caller. | Incomplete shared API, not a demonstrated broken player sensing command. Implement, deprecate or narrow the contract before new consumers use it. |
| [`Body.Listening`](../../MudSharpCore/Body/Implementations/Body.cs#L199), [`ReportCondition`](../../MudSharpCore/Body/Implementations/Body.cs#L478) | Throwing members with no current consumers located. | Legacy contract audit; avoid treating the whole perception/health system as unfinished. |
| [`MedicalExaminationProposal`](../../MudSharpCore/Health/MedicalExaminationProposal.cs#L85) | Reject/Expire throw, but the class has no construction/registration references. Active medical consent uses other proposal paths. | Orphaned code; retire or integrate deliberately. Not an active surgery rejection bug. |
| [`AutobuilderAreaTerrainRectangleRandomFeatures`](../../MudSharpCore/Construction/Autobuilder/Areas/AutobuilderAreaTerrainRectangleRandomFeatures.cs#L64) | XML type `river` throws. Current builder creation supports simple/uniform/road; no river implementation was found. | Improve unsupported/legacy XML diagnostics or remove placeholder support. Not a builder-accessible river generator. |
| `MagicalMeleeAttackPower`, unknown-type project/factory branches | Abstract/nonregistered support base or defensive rejection of invalid persisted types. | Do not count these automatically as missing shipped features. |

### F08 P2/P3 — Older behavior deserves review even when the file was touched recently

**Classification: line-history evidence plus targeted review risks.**

The dated queue below records the actual evidence. Its strongest review concerns are legacy account-policy behavior and legacy AI decision branches; accepted Debug support login is excluded. Time-zone daylight saving, non-language audience presentation, word-mask configuration and the craft quality fallback need product decisions or compatibility review; they are not automatically active defects.

[`WildAnimalHerdAI`](../../MudSharpCore/NPC/AI/WildAnimalHerdAI.cs#L1380) still has TODO/no-op priority handling for Posture/Fight/Flee in the inspected branch. Local AI instructions call this controller advanced/legacy and not normally builder-creatable, while broader historical documentation describes builder support differently. Resolve registration/support status before investing in repairs. Do not infer that all current group/predator AI lacks fighting or fleeing.

### F09 P2 — Synchronous world callbacks and progs remain an operational risk boundary

**Classification: confirmed architecture, workload-dependent risk.**

[`Scheduler.CheckSchedules`](../../MudSharpCore/Framework/Scheduling/Scheduler.cs#L78) bounds callback count, not individual callback time, and rethrows callback exceptions. Main-loop saves, clocks, effects and storyteller tools execute within the world loop. [`ForLoop` compilation](../../MudSharpCore/FutureProg/Statements/ForLoop.cs#L111) leaves standard FutureProg repetition uncapped by its optional iteration limit; other compilation contexts use 10,000. While loops and recursion have their own guards. There is no evidence here of a common elapsed-time budget that preempts arbitrary synchronous work.

This design preserves serial world consistency. It also means a sufficiently costly trusted script/callback can stall the world, and a thrown scheduler callback can reach the process-level failure boundary. Network cancellation cannot interrupt a synchronous storyteller tool already executing.

**Recommended closure:** document supported execution budgets, profile representative authored worlds and add deterministic failure/slow-work scenarios. Consider cooperative budget checks where needed, with an explicit transaction/interruption policy; casually moving world mutations to background threads would introduce different correctness problems.

### F10 P2 — Vehicle recovery has real implementation and historical evidence, but uneven adapter-level regression coverage

**Classification: verification gap, not missing restart support.**

[`VehicleRouteMotionPersistence`](../../MudSharpCore/Vehicles/VehicleRouteMotionPersistence.cs#L315) freezes interrupted motion at durable checkpoints. The [V1 acceptance record](../Verification/RouteCell_Vehicle_V1_Acceptance_Evidence.md) records live/reboot qualification for specific train/platform/wagon scenarios. Several strategy tests substitute `IVehicleRouteMotionPersistence`; no direct automated test of `FreezeInterruptedVehicleMotions` was located in the reviewed core test inventory.

**Recommended closure:** targeted real-provider tests for interruption, checkpoint loading, idempotent freeze/resume, occupied interiors, and route/topology changes. Keep existing historical evidence and state its revision. Arbitrary coordinate travel, collision, fares/reservations and richer dispatch/signalling are declared V1 exclusions, not hidden implementation failures.

### F11 P2 — Grid allocation and connector policy have weaker focused assurance

**Classification: test/contract gap; no specific allocation defect proved.**

[`ElectricalGrid`](../../MudSharpCore/Construction/Grids/ElectricalGrid.cs) persists membership, rebuilds consumers/producers and implements cutout/cutin ordering. [`GridSystemTests`](../../MudSharpCore%20Unit%20Tests/GridSystemTests.cs) exercises producer/component drawdown and other meaningful contracts, but direct grid shedding/reconnection/priority cases were not located. [`PowerSocketGameItemComponent`](../../MudSharpCore/GameItems/Components/PowerSocketGameItemComponent.cs#L301) has generic connection/disconnection approval methods returning true with TODOs; surrounding connector/manipulation checks still apply.

**Recommended closure:** test multiple producers/consumers, equal/unequal draw, failed load, late membership, power loss/reconnection and deletion. State whether generic permission hooks are intentionally permissive or incomplete authoring policy. This is a bounded legacy network model; do not grade it against an unapproved electrical-physics redesign.

### F12 P2 — Several established systems lack an easy current builder/runtime entry point

**Classification: documentation gaps, scoped to what was located.**

| Area | Existing information | Missing or weak entry point |
| --- | --- | --- |
| Skills, knowledges and learning | Player/staff command help, role progression, NPC packages, chargen and seeder docs | One guide joining skill definition, checks, categories, teaching, learning, knowledge, chargen and FutureProg. |
| Checks and general merits/flaws | Check/decorator editor guide, individual merit tests/docs, intrinsic racial merits, stock seeder notes | General check semantics and supported merit authoring/catalogue with practical examples. |
| Scripted login events | Storyteller command help and persisted event/question classes | Builder walkthrough, target/ready/finished lifecycle, answer/prog signatures, reconnect/reload and safe editing. |
| Boards and helpfiles | Inline command help, board presentation security and help-contract tests | Board permissions/post lifecycle/persistence guide; a discoverable helpfile-authoring guide. |
| Foraging | Runtime tests and scattered agriculture/item/seeder information | A joined runtime/builder guide for profiles, yields, depletion, recovery and prerequisites. |
| Mounts/stables and hospitals | Vehicle/mounted-combat or health/employment docs | Joined setup and operations workflows for these cross-system features. |

“Missing” here means no dedicated comprehensive current guide was located in the design-document index/corpus searches. It does not mean there is no command help or relevant documentation anywhere.

### F13 P3 — Flood state is not implemented by authored terrain layers

**Classification: explicit omission/product boundary.**

[`Room`](../../MudSharpCore/Construction/Room.cs#L301) still marks room flooding effects TODO in swimming/underwater-layer calculations. Water traversal currently derives from authored layers in those paths. Weather, surface liquids and immersion-related mechanics elsewhere do not by themselves supply a persistent dynamic flood-state system.

**Recommended closure:** document this limit. Add flooding only if required by the intended simulation scope, with water level, exits, items, occupants, save/reload and recession semantics. Do not list it as already provided by weather support.

### F14 P3 — Reserved computer applications must remain visibly reserved

**Classification: intentional incomplete capability, already documented.**

[`ComputerBuiltInApplications`](../../MudSharpCore/Computers/ComputerBuiltInApplications.cs#L27) contains Messenger. [`ComputerBuiltInApplicationExecutors`](../../MudSharpCore/Computers/ComputerBuiltInApplicationExecutors.cs#L37) returns “not implemented yet” when no executor exists; service discovery filters through `IsImplemented`. The [computer design](../Technology/FutureMUD_Computer_Programs_Concept_Design.md) explicitly marks Messenger reserved at several points.

This is not an undocumented broken shipped service. Review whether local application lists distinguish reserved entries from runnable apps, then either label/hide the reservation or implement it when desired. Do not infer that working Mail, Boards, FTP or media applications are missing.

### F15 P2 — Combat response assertions warrant a defined extension contract

**Classification: latent edge/extension risk, not a reproduced native combat crash.**

[`ChangePositionMove`](../../MudSharpCore/Combat/Moves/ChangePositionMove.cs#L46), [`BreakClinchMove`](../../MudSharpCore/Combat/Moves/BreakClinchMove.cs#L96) and parts of [`ClinchStrategy`](../../MudSharpCore/Combat/Strategies/ClinchStrategy.cs) throw for unsupported responses/targets. Deeper tracing found normal native strategy response paths that avoid the first two fallthroughs: movement responses fall through to null, while break-clinch has a specific supported response. Clinch's non-character target case remains a targeted edge question.

**Recommended closure:** enumerate registered move/response/strategy pairings and define extension behavior for unsupported responses. Add behavioral matrix tests around viable supported combinations, callback target invalidation and non-character targets. Avoid treating every `NotImplementedException` as a reachable combat feature gap.

### F16 P3 — Wound infection TODO is a balance note, not an absent infection system

**Classification: ruled-out functional-gap candidate.**

[`SimpleOrganicWound.HealingTick`](../../MudSharpCore/Health/Wounds/SimpleOrganicWound.cs#L1763) runs infection checks before the later trauma-controlled auto-closure branch with its TODO. Infection calculation already uses configured bleeding/trauma-controlled multipliers, terrain, damage, severity, treatment and resistance. The remaining note around line 2155 concerns an extra penalty or balance decision for that closure path.

Only revisit it with a health-balance requirement and simulation evidence. Do not implement a duplicate infection mechanism based on the comment.

### F17 P2 — Web client tests are missing from ordinary default PR coverage

**Classification: confirmed CI-selection gap at this snapshot.**

[`scripts/unit-test-projects.txt`](../../scripts/unit-test-projects.txt) includes ten projects, including TerrainPlanner and TestReporting, but omits `MudClient/MudClientTests`. The client tests are selected by the [product publishing workflow](../../.github/workflows/publish-products.yml#L159), which runs for product tags/manual publication rather than ordinary PRs. No separate client PR test workflow was located.

**Recommended closure:** add a path-filtered client/proxy CI job or explicitly include the suite in an appropriate PR gate, preserving the separate deployment/package tests. The mere presence of `coverlet.collector` in a project does not establish that coverage is collected by the ordinary workflow.

### F18 Resolved — Obsolete service projects retired

**Classification: resolved by owner-requested source retirement on 15 November 2026.**

`MudSharp Watcher` and `FutureMUD Upgrade Service` are obsolete. Their 23 tracked files, solution project/configuration entries and local generated build output are removed. The FutureProg guide no longer recommends the Watcher. See the [retirement record](Retired_Legacy_Tools.md).

The original evidence remains historical:

- The [Watcher implementation at the audited revision](https://github.com/FutureMUD/FutureMUD/blob/b32228996a95b745a6ad02298d582347b788ce7e/MudSharp%20Watcher/UpdateChecker.cs#L84) downloaded an HTTP archive without authenticity verification in that path.
- The [Upgrade Service implementation at the audited revision](https://github.com/FutureMUD/FutureMUD/blob/b32228996a95b745a6ad02298d582347b788ce7e/FutureMUD%20Upgrade%20Service/Controllers/FileController.cs#L77) contained a placeholder checksum and prototype upload/storage behavior.

Neither project is a maintained release product. Removing them resolves the review's conditional recommendation for their source; it does not uninstall any external copies. The current signed web-client updater and authenticated website publishing implementation are separate and remain in place.

Obsolete terrain-tool build remnants, saved local settings and the archived desktop Terrain Planner Tool migration project are also removed with explicit owner approval. The maintained `TerrainPlanner/` product and DatabaseSeeder terrain seeding remain unchanged. Historical release-backfill mappings continue to target their pinned original source revisions. The WPF configuration tool is a separate legacy project and remains outside this deletion scope.

## Historical review queue

“Last touched” and “last behavior review” answer different questions. The source census found **no runtime C# file whose latest path-level modification was older than two years**, but this is dominated by sweeping work: 1,124 of the 3,580 runtime C# files have latest modification `c3bf6a6a9` (19 July 2026, regenerate seeded data/remove obsolete content). Hundreds more have Room/checkpoint terminology changes as their latest touch. Representative diffs remove usings or change terminology without completing old behavior.

The following line-level evidence is more useful. Dates are the oldest surviving relevant lines/changes in available history, not a claim that no adjacent code has changed. Several lines reach the repository's 7 April 2023 import; development may predate it.

| Feature/branch | History evidence | Why review it |
| --- | --- | --- |
| Debug support login | Passwordless Debug branch originates in `0790ea9cac`, 2023-04-07; Release performs password verification. | Deliberate accepted support behavior, not an aging-code defect (F02). |
| Plaintext password notification | `LoggedInMenu.cs:903–904` and stock template `CoreDataSeeder.cs:427–433` trace to `0790ea9cac`, 2023-04-07. | Active legacy security policy (F01). |
| Generic `Body.CanSense` | `git blame -w -L 77,81 MudSharpCore/Body/Implementations/BodyPerception.cs`: 2023-04-07. | Incomplete unused contract; new consumers can expose it. |
| `MedicalExaminationProposal` Reject/Expire | Throwing methods date to 2023-04-07; July 2026 diff removed usings. | Orphaned code/support-status decision, not active medical failure. |
| Wild-animal herd priority branches | `git blame -w -L 1380,1392 MudSharpCore/NPC/AI/WildAnimalHerdAI.cs`: Posture/Fight/Flee TODO/no-op branch from 2023-04-07. | Real old incomplete branch in a legacy controller; reconcile registration/docs. |
| Combat position response behavior | `git blame -w -L 24,47 MudSharpCore/Combat/Moves/ChangePositionMove.cs`: 2023-04-07; later callback revalidation exists. | Test extension/fallback response contract, without labelling the entire combat subsystem dormant. |
| Autobuilder river XML placeholder | `git log -S 'case "river"' -- ...RandomFeatures.cs` and blame: 2023-04-07. No current river creation path found. | Invalid/legacy persisted-template hazard (F07). |
| Daylight-saving adjustment | `FutureMUDLibrary/TimeAndDate/MudTimeZone.cs:284` TODO dates to 2023-04-07 despite later edits. | Genuine omitted capability if real-world civil DST is required. |
| Non-language audience speech and word-mask configuration | `LanguageOutput.cs:52,130` and `WordMaskScrambler.cs:8` retain 2023 TODO/policy. | Defined fallback exists, but authored policy/configuration remains unresolved. |
| Tax core | Much of `SalesTaxBase.cs:65–90` dates to 2023; targeted tax changes exist in November 2024 (`9bf3db856`). July 2026 touch is not a full tax review. | Cross-calendar periods, rounding, taxable base and settlement tests. Do not describe all economy code as untouched. |
| Craft quality-formula fallback | `Craft.cs` load/clone code is largely 2023; fallback added 2024-08-12 (`09537c2390`) remains marked temporary. | Determine whether old rows still need it and whether diagnostics are adequate. |
| Watcher archive download/extraction | Key `UpdateChecker.cs:84–99` behavior from `f5203d9d6f`, 2023-05-06; latest project changes are library maintenance. | Old trust/compatibility path (F18). |
| Upgrade Service checksum | `FileController.cs:77–82` placeholder from `f5203d9d6f`, 2023-05-06, despite later dependency changes. | Scaffold still incomplete; determine ownership/use. |

The two removed service entries above are retained as historical evidence and are resolved by F18. This queue deliberately does not label recently developed banking/lending, auctions, employment hiring, clan governance, configurable/Vancian magic, Room/layer integration or weather as abandoned. Their older subpaths may still deserve review.

## Documentation and acceptance work to prioritise

### First: correct reliability and security contracts

Close the fatal-exit reliability issue F04, and plan account notification/recovery hardening in F01/F05. Preserve the accepted Debug support behavior F02. For employment, explicitly choose durable restart continuity or documented recovery from runtime-only grants (F03). Before changing behavior, preserve old-world identities/customisation and verify persistence/provider boundaries.

### Second: publish one current support map

Use this thematic map as an index into current builder/runtime guides. Give each major subsystem a short supported-scope and known-limitations section, a minimal setup workflow, and explicit links to historical verification receipts. Mark older plans and PR-boundary reports as historical. Start with magic, employment, arenas and seeder product boundaries, where contradictory status claims were demonstrated.

Fill the dedicated guide gaps in F12. The goal is that a builder can find the supported setup without reconstructing it from source, seeded catalogue prose and multiple dated handovers.

### Third: qualify joined workflows rather than add catalogue snapshots

| Campaign | Observable acceptance boundary |
| --- | --- |
| Account lifecycle | Registration/change/recovery enforce one policy; wrong credentials fail in Release while Debug preserves the accepted support behavior; codes expire/consume correctly; notifications contain no password; failure exits are observable. |
| Employment restart | Source/principal/priority/due state and grant semantics match the chosen contract across independent database reload; financial refusal remains safe and operational recovery is clear. |
| Health and survival | Infection/drugs/thermal state and interrupted medical work survive elapsed/offline time and restart without duplicated treatment, lost resources or incorrect actor/body ownership. |
| Combat matrix | Supported PC/NPC/mounted/natural/ranged/clinch/grapple/helpless and target-loss combinations have defined outcomes; custom/invalid responses fail deliberately. |
| Vehicle/portal/party travel | Occupied interiors, checkpoint/restart, followers, hitches, transient docking and topology invalidation preserve identities and refuse unsafe moves consistently. |
| Society/economy | Case-to-sentence/release, death-to-probate, purchase-to-settlement, loan/default/collateral and elections exercise permissions, idempotency and restart. |
| Production/ecology | Craft/project reservations, seasonal agriculture and gathering conserve quantities, recover state and preserve builder content across replay. |
| Technology/AI | Power loss, network/service discovery, malformed programs/provider replies, cancellation and restart have bounded, observable failure behavior. |

These are recommended acceptance campaigns, not checks executed by this audit. Use disposable owned worlds/databases and ordinary player permissions alongside builder/admin setup. Do not use a successful build or a fake persistence service as proof of real gameplay/reboot behavior.

## Census and assurance appendix

### Source size and unfinished markers

The tracked-source census found **3,580 C# files / 1,035,180 physical lines** under `MudSharpCore` and **1,314 C# files** under `FutureMUDLibrary`. Core contains **114 lexical matches for `throw new NotImplementedException(`** and **295 TODO/FIXME/HACK tokens**. These are search counts, not parsed reachable defects; comments/strings can affect lexical counts. The deeper examples above show why these totals must not become a completion percentage.

Selected runtime directories illustrate breadth and concentration:

| Runtime folder | C# files | Physical lines | Lexical unimplemented throws | TODO/FIXME/HACK tokens |
| --- | ---: | ---: | ---: | ---: |
| GameItems | 630 | 171,191 | 27 | 76 |
| Commands | 148 | 170,659 | 1 | 28 |
| Magic | 417 | 83,280 | 2 | 1 |
| Economy | 140 | 73,379 | 0 | 3 |
| FutureProg | 452 | 58,446 | 0 | 2 |
| NPC | 89 | 44,524 | 0 | 16 |
| Combat | 218 | 42,465 | 31 | 26 |
| Effects | 415 | 42,260 | 0 | 9 |
| Body | 148 | 36,840 | 15 | 32 |
| RPG | 146 | 37,859 | 2 | 16 |
| Work | 87 | 31,367 | 5 | 1 |
| Character | 65 | 31,306 | 1 | 28 |
| Construction | 93 | 29,276 | 7 | 5 |
| Health | 79 | 27,640 | 4 | 25 |
| Framework | 58 | 20,970 | 5 | 7 |
| CharacterCreation | 60 | 20,807 | 2 | 8 |
| Vehicles | 30 | 20,519 | 0 | 0 |
| Computers | 31 | 14,824 | 0 | 0 |

Counts are a workload map. Computers has no lexical TODOs in its own folder but still intentionally reserves an app; Combat has many throws but several are defensive response assertions. Zero markers is not completeness.

The companion [census JSON](Engine_Maturity_Census_20261114.json) records folder counts, raw newest dates, marker definitions and test-source inventory. Generated migration backup/helper projects, local `bin`/`obj`, vendored packages and scratch prototypes are excluded from engine maturity inference. Persistence migration files and catalogue-generated source are not comparable measures of gameplay implementation size.

### Test and workflow evidence

The default manifest contains ten test projects: library, expression, database seeder, runtime, persistence, Discord, converter, website, TerrainPlanner and TestReporting. ItemSeeder and ArmageddonSeeder have separate product workflows; climate has its dedicated weekly/manual workflow; extended integration/benchmark execution is manual. The default gate excludes extended categories. MudClient has release-test selection but lacks ordinary default PR selection (F17).

There are 755 tracked C# source files under `MudSharpCore Unit Tests`, 96 under library tests, 45 under persistence tests, 103 under DatabaseSeeder tests, 46 under ItemSeeder tests and 129 under ArmageddonSeeder tests. These totals include fixtures/helpers/partials and must not be reported as test cases, coverage, passing results or equal subsystem assurance.

The [test-quality document](Unit_Test_Suite_Quality_and_Maintenance.md) explicitly records source-coupled-test debt and dated suite totals. Current [unit workflow](../../.github/workflows/unit-tests.yml) runs Windows/Linux gates; it does not demonstrate current results without inspecting/running them. No new line/branch coverage or mutation analysis was collected here.

### Reproduction method

1. Record `git rev-parse HEAD` and `git status --short` before review.
2. Use `git ls-files` for tracked source/doc/test inventories. Exclude `bin`, `obj`, `MigrationBackup` and `MigrationModels` from implementation comparisons.
3. Count physical lines with `splitlines()` and lexical markers with `\bthrow\s+new\s+NotImplementedException\s*\(` and `\b(?:TODO|FIXME|HACK)\b`.
4. Collect newest path touches from `git log --format=... --name-only --diff-filter=AM --no-renames` at the audited HEAD. Treat renames/merge simplification and missing history cautiously.
5. Inspect representative commits and use `git blame -w`, `git log -S` and whitespace-insensitive diffs to distinguish old behavior from cleanup/terminology churn.
6. Trace candidate findings through command/caller, implementation, registry, storage model/load/save, tests and current-vs-historical documentation. Downgrade orphaned and defensive code appropriately.

The requested fast-forward imported upstream changes; the follow-up retirement removes obsolete service source and build entries without changing supported gameplay. Deliverables include this report, its census, index and retirement documentation, and the requested service/terrain-tool/archive deletions. Solution-load and development-filter checks passed after the service removals; no gameplay test suite was run for this removal-only slice. Unreviewed or unexercised paths remain unverified; they are not assumed broken or stable.
