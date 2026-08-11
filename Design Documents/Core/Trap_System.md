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

Mechanical traps use existing items and cells rather than a Trap game-item component. A tripwire, pressure plate, chest, bear trap, or detonatable charge is represented by its normal item identity plus TrapEffect; IDetonatable and existing automation components are reused where relevant. This keeps crafting free to produce ordinary physical items, then lets a character deploy a current mechanical template through trap lay. There is no special physical item category to maintain or seed.

Magical traps are authored as magical templates, then placed by the createtrap/placetrap magic spell effect. Their payload can resolve another prepared spell. removetrap/dispeltrap is the corresponding magical countermeasure. This keeps casting costs with placement and makes dispelling a clear, configurable interaction rather than an item-only disarm exception.

Natural hazards use the same template/effect model. NaturalTrap AI gives an NPC a current natural template plus enabled/site FutureProgs; on its minute tick it anchors a proximity hazard to that NPC and other hazards to the suitable current cell. FutureProg createtrap is the complementary option for scripted world events. These approaches are preferred to a special spider-web object because they preserve the same detection, persistence, layer, restraint, and payload rules as all other traps.

## Runtime model

ITrapTemplate is a revisable, approved definition stored in TrapTemplates. Its XML definition contains source domain; OR-combined triggers; ordered payloads with independent delay and target selector; charges, cooldown, lifecycle, disarm policy, optional lifespan; and named module parameters.

TrapEffect is a saving effect on an ICell, IGameItem, or character/NPC anchor. It pins template ID and revision, creator ID, state, charges, and a stable instance GUID. A template revision is never retrospectively changed for an installed trap.

Current trap template revision -> trap lay or create -> TrapEffect on cell or item -> matching event or signal -> avoidance, trigger chance, filters -> immediate or scheduled payloads -> cooldown, charges, spent or rearmed.

States are Unarmed, Armed, Resolving, CoolingDown, Spent, Disarmed, and Expired. Indefinite traps have no wall-clock expiry. FixedExpiry traps stop safely after their required lifespan, while Unstable traps resolve once at expiry before becoming expired. Delayed payloads and cooldowns are separate saving effects so they survive reboot. A gradual deterioration policy is deliberately deferred until it has a distinct condition/material model rather than pretending finite charges are degradation.

## Triggers

First-party trigger modules use OR semantics. Common parameters are chance, filterprog, spotdifficulty, avoiddifficulty, and triggerEcho. Proximity triggers additionally accept maximumproximity. When omitted it resolves to Distant in an ordinary cell and Immediate in a RouteCell; RouteCell enum bands use RouteCellImmediateDistanceMetres, RouteCellProximateDistanceMetres, RouteCellDistantDistanceMetres, and RouteCellVeryDistantDistanceMetres. Signal triggers also accept minimumvalue and maximumvalue. Character-origin filters accept character or character+anchor; signal-origin filters accept source perceivable or source+anchor.

| Trigger | Engine event or source | Typical anchors |
|---|---|---|
| ExitTraversal | CharacterBeginMovementWitness | Cell/item tripwire |
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
| DetonateItem | none | Detonates an IDetonatable item anchor |
| CastSpell | spell, optional power | Resolves prepared spell effects using the recorded creator as caster, with duration and target resistance but no second cast action/resource cost; an unavailable creator fails safely |
| EmitSignal | optional targetitem, optional value | Delivers a ComputerSignal to signal sinks on the selected automation item, or the anchor when omitted |
| ExecuteProg | prog | Calls a matching character or character+anchor FutureProg |
| DirectDamage | damage, optional damagetype | Applies a normal damage packet to a random target bodypart |
| LiquidDischarge | liquid, optional amount | Exposes target bodyparts to a LiquidMixture |
| GasCloud | gas, optional duration, dose, cloudecho | Creates temporary local-layer gas cloud without changing bulk room atmosphere |
| Restraint | optional duration, description | Applies a timed movement-blocking TrapRestraintEffect |

A payload can target the triggerer, all same-layer anchor occupants, or a snapshot excluding the triggerer. Delayed payloads retain intended target ID rather than dynamically choosing a later bystander.

## Detection, avoidance, and disarming

A successful spot or search adds TrapKnowledgeEffect keyed by trap instance GUID. trap pointout grants the same knowledge to another character. Known traps use an automatic avoidance check; this reduces activation probability but does not grant creator or ally immunity. Administrators do not trigger traps. Trigger matching respects anchor cell and layer.

New check types are SetTrapCheck, SpotTrapCheck, SearchForTrapCheck, AvoidTrapCheck, DisarmTrapCheck, DispelTrapCheck, and EscapeTrapCheck.

The default seeder setup uses a dedicated Traps skill, while individual templates may use difficulty and FutureProg filters to model specialist domains. Existing Spot, Search, Survival, and magic traits remain valid template/campaign overrides.

Disarm policy is Impossible, Safe, Risky, or Dispellable. Risky failure manually triggers the trap. Magical deployment is supplied by the builder spell effect createtrap (alias placetrap), which accepts a current magical traptemplate and targets an item, cell, or character; a proximity template cannot target a cell. Magical dispelling is supplied by removetrap (alias dispeltrap), which targets a character, item, or cell and uses DispelTrapCheck after the spell's normal casting checks; trap disarm does not silently remove a magical trap.

## Commands

Player surface:

- trap list
- trap inspect item|here
- trap lay template [on item|here]
- trap pointout person item|here
- trap disarm item|here
- trap recover item|here
- trap struggle

arm item recognises an item-anchored trap. disarm item directs players to the safer trap disarm workflow.

Administrators additionally use trap create, trap debug, trap arm, trap trigger, trap reset, trap reveal, and trap delete.

Builders author revisions through traptemplate, with list, show, edit, set, and review lifecycle. set domain, set trigger, set payload, set charges, set cooldown, set disarm, set lifecycle, and set validate are the first-party builder surface.

## FutureProg and automation integration

FutureProg receives trap payload execution through configured prog IDs. Filter progs are invoked as character or character+perceivable-anchor for movement/open triggers, and source perceivable or source+anchor for signal triggers. TrapSignalReceived is reserved for signal-trigger dispatch and diagnostics. PerceivableProximityChanged is available to normal perceivable hooks and exposes receiver, counterpart, old and new numeric Proximity enum values, and change cause. The Trap variable exposes instance UUID, state, template/revision, owner, source, and charges. trapat, createtrap, armtrap, disarmtrap, and triggertrap provide query, creation, and control. triggertrap accepts an optional character target; without one it can still resolve non-character payloads. createtrap uses the same current-revision, validation, and anchor gates as player deployment.

Mechanical signal triggers subscribe to existing ISignalSourceComponent sources. Signal payloads use the existing ISignalSink contract and a trap-owned source identity, so automation consumers receive a normal ComputerSignal.

## Law and balance

Laying a non-administrative trap checks the new BoobyTrapping crime. The stock law seeder creates it as an automatic, arrestable, bailable deployment offence. Direct-damage and restraint payloads also check ordinary Assault law with the recorded creator as actor origin; explosions and spell payloads retain their own established law paths. This makes lawful alarms possible while preserving jurisdiction-specific intent and outcome offences. Natural traps do not create player crime records.

Balance defaults are conservative: one charge, normal deployment difficulty, hard spotting, normal avoidance/disarm, 30-second restraint/cloud duration, and no creator immunity. Authoring must set damage, gas dose, and explosive payload values explicitly. Seeder samples demonstrate alarm, damage, restraint, fluid, gas, and spell hazards rather than hidden one-shot kills.

## Seeder content

The trap seeder supplies a Traps skill; check definitions; and current template examples: tripwire alarm, tripwire explosive, pressure plate, trapped chest liquid splash, trapped chest needle, bear trap, spider web, magical glyph, and gas release.

The item examples are templates and seed content, not new hard-coded item component types. NPCs create natural traps through a dedicated natural-trap AI/Prog action that deploys a current natural template with its NPC as creator.

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
| Automated trap coverage | Complete | 2 trap prog-type tests, 4 core definition/registration tests, and 1 idempotent seeder test are included in the complete fast test matrix. |
| Broad fast test suite | Complete | All 3,493 tests passed: 429 library, 21 expression engine, 687 seeder, 2,207 core, 33 database library, 22 Discord, 43 converter, and 51 web. |
| Live server vertical slice | Complete | Signed in through the existing admin account; created and armed an openable direct-damage trap on a disposable envelope; possessed the existing elderly NPC, triggered it by opening the envelope, and verified that the trap reached Spent with zero charges. Removed the trap and envelope. A final clean server run created then deleted a disposable template, confirmed the empty list, and exercised NPC possess/return without a server fault. |
| Blank-database snapshot refresh | Complete | Refreshed against a dedicated disposable local snapshot database. Manifest, dump history, and migration agree on `20260810130800_AddTrapTemplates`; all six snapshot tests pass. |
