# Trap System

## Purpose and scope

FutureMUD traps are persistent combinations of one or more triggers and ordered payloads. They are not a new item type. A trap is a saving effect anchored to an existing perceivable, which permits physical traps, prepared spells, and natural hazards to share runtime, persistence, visibility, law, and FutureProg surfaces.

Version 1 uses a gated domain model:

| Domain | Anchor | Intended fiction | Prohibited v1 hybrid |
|---|---|---|---|
| Mechanical | Item or cell | Tripwires, pressure plates, trapped containers, bear traps, automation | Mechanical template plus magical payload |
| Magical | Item or cell | Glyphs, wards, delayed spell effects | Spell template plus mechanical payload |
| Natural | Cell, item, or NPC-created anchor | Spider webs, burrow snares, environmental hazards | Natural template plus foreign payload |

An item may anchor a magical or natural trap, but its template still has exactly one domain. This avoids ambiguous costs, detection rules, law attribution, and builder support. Cross-domain combinations can be introduced later as a deliberate composite feature.

## Domain-composition decision

Fully arbitrary trigger/payload mixing was considered. Its advantage is maximum combinatorial freedom: a magical sensor could detonate a physical charge, and a natural web could emit an automation signal. It would, however, make resource consumption, magical counterplay, ownership, crime attribution, search language, and seeder examples depend on an implicit priority order between domains.

Version 1 therefore permits broad reuse within a domain but gates combinations at template validation. Generic effects such as damage, liquid, gas, restraint, and FutureProg payloads remain usable by all domains; mechanical detonation/signal payloads stay mechanical and prepared-spell payloads stay magical. This gives builders expressive multi-trigger, multi-payload traps without silently creating cross-domain hybrids. A future composite template can make hybrid cost, detection, and legal rules explicit instead of inferring them from its modules.

## Deployment recommendations

Mechanical traps use existing items and cells rather than a Trap game-item component. Every mechanical template declares at least one tagged trigger component and one tagged payload component. A single requirement may be both roles, and one item carrying two applicable tags may satisfy separate trigger and payload requirements; this is the bear-trap case. The requirement also configures spent recovery chance and quality weight. IDetonatable and existing automation components are still required for detonation and owner-routed signal payloads. A character may install parts they are holding or loose parts in the current cell; held/wielded matches are selected before room matches. Every selected part must pass the normal manipulation/access gate, held parts must also be removable from the character's inventory, and both checks are repeated when timed setup completes. Successful placement extracts non-anchor components from their former inventory or cell collection and gives their reservation effect the trap anchor as an effective spatial host. They therefore cease to be loose targetable objects while their effective `Location`, `TrueLocations`, layer, and RouteCell position continue to resolve through the trap for explosions, signals, and other spatial behavior; the component remains its own `LocationLevelPerceivable` so layer-sensitive payload code uses that effective context. The captured install layer and RouteCell coordinate are persisted with the trap binding so this remains stable across reboot. Removal restores extant components at the captured anchor position before releasing their reservations. Contained items, worn items, and items in another character's inventory are not eligible. This keeps crafting free to produce ordinary physical items while making the actual parts, their quality, their authorization, and their fate explicit.

Magical traps are authored as magical templates, then placed by the createtrap/placetrap magic spell effect. Their payload can resolve another prepared spell. removetrap/dispeltrap is the corresponding magical countermeasure. This keeps casting costs with placement and makes dispelling a clear, configurable interaction rather than an item-only disarm exception.

Natural hazards use the same template/effect model. NaturalTrap AI gives an NPC a current natural template plus enabled/site FutureProgs; on its minute tick it anchors a proximity hazard to that NPC and other hazards to the suitable current cell. FutureProg createtrap is the complementary option for scripted world events. These approaches are preferred to a special spider-web object because they preserve the same detection, persistence, layer, restraint, and payload rules as all other traps.

## Runtime model

ITrapTemplate is a revisable, approved definition stored in TrapTemplates. Its XML definition contains source domain; OR-combined triggers; ordered payloads with independent delay and target selector; tagged physical component requirements; charges, cooldown, lifecycle, disarm policy, optional lifespan; setup/disarm/recovery action times; an optional character knowledge FutureProg; and named module parameters. A missing knowledge prog means everyone knows the template. The prog must return boolean and accept one character. Non-mechanical templates must not declare physical requirements.

TrapEffect is a saving effect on an ICell, IGameItem, or character/NPC anchor. It pins template ID and revision, creator ID, state, charges, a stable instance GUID, and the IDs/roles/recovery settings of matched component items. Exit traps are always owned by the origin cell and persist the stable exit ID and origin-side cell ID separately from any door or component. This makes `north` and other exit keywords first-class bindings, permits distinct traps on several exits in one cell, and prevents traversing the opposite side from firing the wrong trap. Installed components receive a non-saving no-get reservation recreated from the trap on load; removal, safe dismantling, or administrative deletion releases it. A template revision is never retrospectively changed for an installed trap.

Compatibility is fail-closed for the new physical contract. Previously deployed component-less mechanical effects retain their pinned runtime data and can resolve or be removed, but a legacy mechanical template cannot be newly deployed or submitted until a builder adds trigger and payload requirements in a new revision. Magical and natural definitions are unaffected. This is an XML-definition evolution and requires no relational migration.

Current trap template revision -> trap lay or create -> TrapEffect on cell or item -> matching event or signal -> avoidance, trigger chance, filters -> immediate or scheduled payloads -> cooldown, charges, spent or rearmed.

States are Unarmed, Armed, Resolving, CoolingDown, Spent, Disarmed, and Expired. Indefinite traps have no wall-clock expiry. FixedExpiry traps stop safely after their required lifespan, while Unstable traps resolve once at expiry before becoming expired. Resolution consumes its charge before payload execution, delayed payloads and cooldowns are separate saving effects, and load-time reconciliation consumes any historical trap left in Resolving; together these rules prevent a reboot or explosive shutdown from repeating a payload. A gradual deterioration policy is deliberately deferred until it has a distinct condition/material model rather than pretending finite charges are degradation.

## Triggers

First-party trigger modules use OR semantics. Common parameters are chance, filterprog, spotdifficulty, avoiddifficulty, and triggerEcho. ExitTraversal also accepts movementtypes (a comma-separated MovementType flag list), minimumsize, and maximumsize. The supported types are Upright, Crawling, Prostrate, Climbing, Swimming, Flying, and Floating; `All` permits all of them. Proximity triggers additionally accept maximumproximity. When omitted it resolves to Distant in an ordinary cell and Immediate in a RouteCell; RouteCell enum bands use RouteCellImmediateDistanceMetres, RouteCellProximateDistanceMetres, RouteCellDistantDistanceMetres, and RouteCellVeryDistantDistanceMetres. Signal triggers also accept minimumvalue and maximumvalue. Character-origin filters accept character or character+anchor; signal-origin filters accept source perceivable or source+anchor.

| Trigger | Engine event or source | Typical anchors |
|---|---|---|
| ExitTraversal | CharacterBeginMovementWitness, including exit and movement context | A specific exit side, cell, or item tripwire |
| Openable | ItemOpened | Container, door, or trapped item |
| Proximity | PerceivableProximityChanged; outside to at-or-within the configured band | Web, wire, mobile natural hazard, anchored glyph |
| CellEntry | CharacterEnterCellWitness with layer match | Room or glyph trap |
| Signal | bound ISignalSourceComponent.SignalChanged | Automation-connected mechanical trap |
| Manual | Explicit trap/admin invocation | Scripted or test trigger |

Proximity delivery is opt-in. A FutureProg hook on PerceivableProximityChanged automatically registers its owner; C# consumers such as TrapEffect register an explicit range. The engine indexes registered receivers by ordinary cell/layer and RouteCell coordinate, so normal movement does not fan out to every item or character. It batches before/after state around movement, route-coordinate changes, layer changes, position targets, containment, and party membership; it emits only when the receiver's calculated proximity actually changes. The event arguments are receiver, counterpart, previous proximity, current proximity, and cause. Both sides of a position-target relationship can receive the directional event when registered. Database position hydration deliberately suppresses these events: a restored position is state reconstruction, not a live transition.

New proximity traps require a real non-cell anchor: an item, door, character, NPC, or another perceivable with an effective spatial host. Use CellEntry for a cell or area hazard. Existing persisted cell-owned proximity effects retain legacy CellEntry compatibility so worlds do not lose hazards; builder and FutureProg deployment reject creating new ones.

## Payloads

Payload parameters are named so template XML remains extensible without schema changes.

| Payload | Required parameters | Behaviour |
|---|---|---|
| DetonateItem | none | Detonates the matched payload component that implements IDetonatable |
| CastSpell | spell, optional power | Resolves prepared spell effects using the recorded creator as caster, with duration and target resistance but no second cast action/resource cost; an unavailable creator fails safely |
| EmitSignal | optional targetitem, optional value | Delivers a ComputerSignal to the explicit item, or to a matched payload component with signal sinks when omitted |
| ExecuteProg | prog | Calls a matching character or character+anchor FutureProg |
| DirectDamage | damage, optional damagetype | Applies a normal damage packet to a random target bodypart |
| LiquidDischarge | liquid, optional amount | Exposes target bodyparts to a LiquidMixture |
| GasCloud | gas, optional duration, dose, cloudecho | Creates temporary local-layer gas cloud without changing bulk room atmosphere |
| Restraint | optional duration, description | Applies a timed movement-blocking TrapRestraintEffect |

A payload can target the triggerer, all same-layer anchor occupants, or a snapshot excluding the triggerer. Delayed payloads retain intended target ID rather than dynamically choosing a later bystander.

## Detection, avoidance, and disarming

A successful spot or search adds TrapKnowledgeEffect keyed by trap instance GUID. trap pointout grants the same knowledge to another character. Once known, an item-anchored trap is called out in both `look` and `evaluate`; administrators always receive that presentation. Known traps use an automatic avoidance check; this reduces activation probability but does not grant creator or ally immunity. Administrators do not trigger traps. Trigger matching respects anchor cell, layer, specific exit side, movement type, and configured size range.

New check types are SetTrapCheck, SpotTrapCheck, SearchForTrapCheck, AvoidTrapCheck, DisarmTrapCheck, DispelTrapCheck, and EscapeTrapCheck.

The default seeder setup uses a dedicated Traps skill, while individual templates may use difficulty and FutureProg filters to model specialist domains. Existing Spot, Search, Survival, and magic traits remain valid template/campaign overrides.

Disarm policy is Impossible, Safe, Risky, or Dispellable. Risky failure manually triggers the trap. Magical deployment is supplied by the builder spell effect createtrap (alias placetrap), which accepts a current magical traptemplate and targets an item, cell, or character; a proximity template cannot target a cell. Magical dispelling is supplied by removetrap (alias dispeltrap), which targets a character, item, or cell and uses DispelTrapCheck after the spell's normal casting checks; trap disarm does not silently remove a magical trap.

Mechanical templates require positive setup, disarm (when disarmable), and recovery times before review. `trap lay`, `trap disarm`, and `trap recover` use cancellable general/movement actions for non-administrators and revalidate the anchor and trap state at completion. For `trap lay`, this includes persistence, reservation, custody/location, removability, and the ordinary `CanManipulateItem` access decision for every selected physical item. Administrative operations remain immediate. Magical and natural deployment continue to use their spell, AI, or FutureProg action timing rather than the mundane setup timer.

Component quality is averaged by configured quality weight relative to Standard. Each trigger-quality stage changes configured trigger chance by 2.5 percentage points; every two stages change spot and avoidance difficulty by one step. Payload quality scales direct damage, discharged liquid, gas dose, and restraint duration by 5% per stage, clamped to 50-150%; specialised item payloads such as explosives retain their own item implementation as well. Safe dismantling returns every extant component. Dismantling a spent trap rolls each distinct item's base recovery chance plus 5 percentage points per weighted quality stage; failures delete the broken item. Spent traps retain their no-get reservations while any component has a non-zero recovery opportunity. If no recoverable component remains, the effect and any zero-chance remnants are removed automatically after the final delayed payload, preventing persistent clutter.

## Commands

Player surface:

- trap list
- trap types (aliases: trap known, trap templates)
- trap inspect item|exit|here
- trap lay template on item|exit|here [using item ...] (held/wielded items are preferred; otherwise each item must be loose and manipulable in the current cell)
- trap pointout person item|exit|here
- trap disarm item|exit|here
- trap recover item|exit|here
- trap struggle

`trap types` includes all templates the character knows and labels whether each is deployed with `trap lay`, a spell/Prog, or an NPC/Prog; magical and natural rows do not misleadingly show a mundane setup time. `proximity [target]` is a related perception command. It groups visible local characters and items by Intimate, Immediate, Proximate, Distant, and Very Distant relationship so builders and players can understand proximity-trigger placement. `select <direction> <option>` resolves a door installed in that exit in the same way as `switch <direction> <option>`.

arm item recognises an item-anchored trap. disarm item directs players to the safer trap disarm workflow.

Administrators additionally use trap create, trap debug, trap arm, trap trigger, trap reset, trap reveal, and trap delete.

Builders author revisions through traptemplate, with list, show, edit, set, and review lifecycle. set domain, set trigger, set payload, set component, set charges, set cooldown, set setuptime, set disarmtime, set recoverytime, set knowprog, set disarm, set lifecycle, and set validate are the first-party builder surface. `component add <trigger|payload|both> <tag> [spent recovery %] [quality weight]` and `component remove <number>` manage physical requirements. `traptemplate set trigger <number>` displays every supported parameter with its current or default value. Invalid trigger editing syntax returns this contextual help rather than a generic parameter error.

## FutureProg and automation integration

FutureProg receives trap payload execution through configured prog IDs. Filter progs are invoked as character or character+perceivable-anchor for movement/open triggers, and source perceivable or source+anchor for signal triggers. TrapSignalReceived is reserved for signal-trigger dispatch and diagnostics. PerceivableProximityChanged is available to normal perceivable hooks and exposes receiver, counterpart, old and new numeric Proximity enum values, and change cause. The Trap variable exposes instance UUID, state, template/revision, owner, source, and charges. trapat, createtrap, armtrap, disarmtrap, and triggertrap provide query, creation, and control. triggertrap accepts an optional character target; without one it can still resolve non-character payloads. createtrap uses the same current-revision, validation, and anchor gates as player deployment; its four-argument overload accepts an item collection and is required when a mechanical anchor item alone does not satisfy every component requirement.

Mechanical signal triggers subscribe to existing ISignalSourceComponent sources. Signal payloads use the existing ISignalSink contract and a trap-owned source identity, so automation consumers receive a normal ComputerSignal.

## Law and balance

Laying a non-administrative trap checks the new BoobyTrapping crime. The stock law seeder creates it as an automatic, arrestable, bailable deployment offence. Direct-damage and restraint payloads also check ordinary Assault law with the recorded creator as actor origin; explosions and spell payloads retain their own established law paths. This makes lawful alarms possible while preserving jurisdiction-specific intent and outcome offences. Natural traps do not create player crime records.

Balance defaults are conservative: one charge, normal deployment difficulty, hard spotting, normal avoidance/disarm, 30-second restraint/cloud duration, and no creator immunity. Authoring must set damage, gas dose, and explosive payload values explicitly. Seeder samples demonstrate alarm, damage, restraint, fluid, gas, and spell hazards rather than hidden one-shot kills.

## Seeder content

The trap seeder supplies a Traps skill; check definitions; a `Functions / Trap Components` tag family; and current template examples: tripwire alarm, tripwire explosive, pressure plate, trapped chest liquid splash, trapped chest needle, bear trap, spider web, magical glyph, and gas release. Every stock mechanical example has tagged trigger and payload requirements, including dual-role pressure, needle, and bear-trap mechanisms. The seeder deliberately provides the reusable tag contract rather than inventing duplicate item prototypes: games apply those tags to their own crafted or seeded wire, mechanisms, explosives, reservoirs, and automation hardware. Signal-trigger parts must additionally implement the existing automation signal-source interface, while signal payload parts without an explicit target must implement a signal sink.

These examples are trap templates and reusable tag contracts, not new hard-coded item component types or duplicate physical item prototypes. NPCs create natural traps through a dedicated natural-trap AI/Prog action that deploys a current natural template with its NPC as creator.

## Persistence and upgrades

TrapTemplates has a composite Id/RevisionNumber key, an EditableItem foreign key, a name, and XML definition. Deployed traps, knowledge, cooldown, delayed payload, gas cloud, and restraint data are stored in existing effect XML. Migration AddTrapTemplates creates the definition table and updates EF snapshot.

## Implementation progress and validation log

| Checkpoint | Status | Evidence |
|---|---|---|
| Core design, gated domains, module model | Complete | This document and ITrap contracts |
| Revisions and EF persistence | Complete | TrapTemplate, context mapping, AddTrapTemplates migration |
| Builder template workflow | Complete | traptemplate and editable helper |
| Runtime physical/natural payload engine | Complete | TrapEffect, gas cloud, restraint, delayed payload/cooldown effects |
| Prepared spell payload and magical deployment | Complete | IMagicSpell.ResolveTriggeredSpell plus createtrap/placetrap spell effect |
| Player/admin command surface | Complete | TrapModule plus arm integration |
| Law/check declarations | Complete | BoobyTrapping, new CheckType values |
| Seeder defaults/content | Complete | TrapSeeder, metadata dependencies, and repeatability test |
| FutureProg trap variable/functions | Complete | Trap variable plus trapat, createtrap, armtrap, disarmtrap, triggertrap |
| Natural NPC AI / Prog creation | Complete | NaturalTrapAI plus createtrap for scripted deployment |
| Proximity-change event and trigger migration | Complete | Indexed IProximityEventService, hook/explicit registration, movement/route/layer/position/containment/party batching, load-hydration suppression, and TrapEffect enter-band handling |
| Proximity-event regression and live verification | Complete | Five focused tests cover changed-only delivery, disposal, hydration suppression, and 1,000 unregistered local objects; LabMUD NPC movement changed a fresh item trap from Armed/1 to Spent/0. See Trap_Live_Test_Logs/93_LabMUD_Proximity_Event_Retest.txt. |
| Exit and player/builder UX follow-up | Complete | Exit-side identity is persisted separately from door items; template knowledge, traversal filters, contextual trigger help, timed mundane actions, item presentation, ordered proximity output, and direction-aware selectable targeting are implemented with focused regression coverage. LabMUD acceptance logs 94-96 verify the builder/player displays, physical explosive binding, movement-triggered signal, and non-admin setup delay. |
| Automated trap coverage | Complete | Core trap definition/registration coverage now includes exit-binding XML identity and ExitTraversal movement-parameter parsing in addition to module serialization, compatibility, event routing, gas dosing, anchor validation, and Prog/magic registration. Seeder repeatability remains covered. |
| Broad fast test suite | Complete | All 3,499 tests passed after the physical-component follow-up: 429 library, 21 expression engine, 687 seeder, 2,213 core, 33 database library, 22 Discord, 43 converter, and 51 web. |
| Live server vertical slice | Complete | Signed in through the existing admin account; created and armed an openable direct-damage trap on a disposable envelope; possessed the existing elderly NPC, triggered it by opening the envelope, and verified that the trap reached Spent with zero charges. Removed the trap and envelope. A final clean server run created then deleted a disposable template, confirmed the empty list, and exercised NPC possess/return without a server fault. |
| Blank-database snapshot refresh | Complete | Refreshed against a dedicated disposable local snapshot database. Manifest, dump history, and migration agree on `20260810130800_AddTrapTemplates`; all six snapshot tests pass. |
| Physical component lifecycle and spent cleanup | Complete | Mechanical templates require tagged trigger/payload parts; deployments persist and reserve matched items; dual-role parts, quality-weighted trigger/payload/recovery behaviour, safe and spent recovery, component-aware detonation/signal routing, FutureProg item collections, and automatic empty-spent cleanup are implemented. Focused core persistence and seeder coverage pass on 12 August 2026. LabMUD logs 98, 101, 102, and 103 prove builder authoring, tripwire binding/reservation/detonation, dual-role bear-trap recovery, interrupted-resolution repair, and same-session spent cleanup. No relational migration was required because both definitions and instances use their existing XML persistence seams. |
