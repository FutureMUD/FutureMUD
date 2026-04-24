# FutureMUD NPC AI and Group AI Runtime

## Purpose
This document explains how FutureMUD's NPC AI stack is assembled today:

- contract interfaces in `FutureMUDLibrary`
- concrete runtime implementations in `MudSharpCore`
- persistence and boot/loading behavior
- builder command workflow
- practical guidance for implementing or extending AI features

This is the primary AI reference. The companion document [Event_System_for_AI_and_Hooks.md](./Event_System_for_AI_and_Hooks.md) covers the event and hook machinery that AI authors depend on.

## Scope
This document covers:

- `IArtificialIntelligence` and the concrete AI hierarchy
- `IGroupAI`, `IGroupAITemplate`, `IGroupAIType`, `IGroupEmote`, `IGroupTypeData`
- AI attachment to NPC templates and live NPCs
- group AI templates and live group instances
- builder workflows for `ai`, `npc ... ai`, `gait`, and `group`
- implementation constraints, edge cases, and likely extension paths

This document does not attempt to be a full event-system reference. Use the companion event-system document for hook contracts, event metadata, and `show events` / `hook` workflow.

## Primary Contracts
### Individual AI
| Contract | Role |
| --- | --- |
| `IArtificialIntelligence` | Shared contract for reusable AI definitions that can be attached to NPC templates and live NPCs |
| `IMountableAI` | Narrow extension for mount/rider behavior layered on top of normal AI behavior |
| `IHandleEvents` | Event-handling surface inherited by AI definitions so they can react to engine events and heartbeat ticks |

### Group AI
| Contract | Role |
| --- | --- |
| `IGroupAI` | Live runtime controller for a specific NPC group instance |
| `IGroupAITemplate` | Editable reusable template for creating `IGroupAI` instances |
| `IGroupAIType` | Behavior policy object that drives a group's state machine, role evaluation, and periodic behavior |
| `IGroupEmote` | Conditional group-emote rule with gender/role/target/action/alertness filters |
| `IGroupTypeData` | XML-persisted per-group state owned by a specific `IGroupAIType` implementation |

## Core Contract Behavior
### `IArtificialIntelligence`
`IArtificialIntelligence` is a reusable AI definition, not a per-NPC state object. A single AI record can be attached to:

- one or more NPC templates
- one or more already-spawned NPCs

That means most AI implementations are configuration-driven and must avoid storing per-NPC mutable state in the AI object itself unless that state is truly shared across every NPC using that AI.

The most important members are:

- `AIType`: persisted discriminator and builder-facing type identity
- `CountsAsAggressive`: classification flag used by downstream systems
- `RawXmlDefinition`: saved XML configuration payload
- `Clone(string newName)`: duplicates the AI definition as a new shared AI record
- `IsReadyToBeUsed`: readiness gate for incomplete or partially configured definitions
- `HandleEvent` / `HandlesEvent`: event subscription and event reaction surface

### `IGroupAI`
`IGroupAI` is the live runtime object for one group instance. Unlike `IArtificialIntelligence`, it does own live state:

- current members
- current alertness
- current action priority
- group roles
- type-specific XML-backed `Data`

It points back to a reusable `IGroupAITemplate`, but the actual group state belongs to the instance.

### `IGroupAITemplate`
`IGroupAITemplate` is the editable reusable configuration for group AI. It defines:

- which `IGroupAIType` policy is used
- optional FutureProg-based avoid-cell logic
- optional FutureProg-based threat evaluation logic
- random group emotes

Templates do not use the revisable editing framework. Changes apply immediately to future and existing groups that depend on them.

### `IGroupAIType`
`IGroupAIType` is the real behavior brain for a group. It provides:

- `ConsidersThreat`
- `EvaluateGroupRolesAndMemberships`
- `HandleTenSecondTick`
- `HandleMinuteTick`
- `LoadData`
- `GetInitialData`
- `SaveToXml`

In practice, the type determines how a herd, pack, or family group behaves over time, while the template supplies builder-controlled progs and emotes.

## Runtime Ownership and Lifecycle
### Registries in `IFuturemud`
The gameworld exposes AI registries through `IFuturemud`:

- `AIs`
- `GroupAITemplates`
- `GroupAIs`

Concrete runtime registration happens in `Futuremud`, which supports `Add(...)` and `Destroy(...)` for all three.

### Boot and Load Order
The load order matters:

- `LoadAIs()` runs after future progs and body definitions are available.
- `LoadNPCTemplates()` runs after AIs, so template AI references can be resolved.
- `LoadNPCs()` runs before `LoadGroupAIs()`.
- `LoadGroupAIs()` first loads all group AI templates, then loads all live groups.

The practical consequence is:

- individual AIs can safely reference FutureProgs during load
- NPC templates can safely reference AI ids
- group instances can safely resolve both their template and their current member ids

### Persistence Model
#### Individual AI
Individual AI persistence is split across:

- `MudsharpDatabaseLibrary/Models/ArtificialIntelligence.cs`
- `MudSharpCore/NPC/AI/ArtificialIntelligenceBase.cs`

Each AI row stores:

- `Id`
- `Name`
- `Type`
- `Definition`

`Type` is the runtime loader discriminator. `Definition` is XML owned by the concrete AI class.

#### NPC Attachments
NPC-template and live-NPC attachment are separate:

- template linkage: `NpcTemplatesArtificalIntelligences`
- live NPC linkage: `NpcsArtificialIntelligences`

This is why the same AI definition can be attached at template level and also attached or removed on specific live NPCs.

#### Group AI Templates
Group AI templates persist through `GroupAiTemplate` rows:

- `Id`
- `Name`
- `Definition`

The XML stores template-level avoid/threat progs, group emotes, and serialized `GroupAIType` configuration.

#### Live Group AI Instances
Group instances persist through `GroupAi` rows:

- `Id`
- `Name`
- `GroupAiTemplateId`
- `Data`
- `Definition`

`Definition` stores action, alertness, and member ids. `Data` stores type-specific runtime state owned by the chosen `IGroupAIType`.

### Loader Registration and Discovery
#### Individual AI
`ArtificialIntelligenceBase.SetupAI()` scans the executing assembly for subclasses that expose a public static `RegisterLoader()` method.

Each concrete AI class normally registers:

- a database loader via `RegisterAIType(...)`
- a builder loader via `RegisterAIBuilderInformation(...)`

If a class registers only the database loader, it can load existing rows but cannot be created from the `ai edit new ...` builder workflow.

That was previously true of `WildAnimalHerdAI`, but it is now builder-registered as well. It remains a more advanced legacy AI than most of the catalogue, but it no longer has a creation-path gap.

#### Group AI Types
`GroupAITypeFactory.InitialiseGroupAITypes()` scans for `IGroupAIType` implementers that expose a public static `RegisterGroupAIType()` method.

Each registered group type supplies:

- a database loader from XML
- a builder loader from command arguments

### Heartbeats and Event Subscriptions
#### Individual NPC AI
Live NPC event dispatch is owned by `NPC`:

- `NPC.HandleEvent(...)` forwards events to all attached AIs unless the NPC is dead, in stasis, or paused by `IPauseAIEffect`
- `NPC.HandlesEvent(...)` returns true if either the NPC itself or any attached AI claims the event

Periodic tick wiring is explicit:

- `NPC.SetupEventSubscriptions()` subscribes only to the heartbeat events that at least one attached AI says it handles
- `NPC.ReleaseEventSubscriptions()` detaches them again on quit/unload

That means `HandlesEvent` is not just documentation. It directly controls whether the NPC is subscribed to expensive repeating heartbeats.

#### Group AI
Group AI instances subscribe themselves directly on creation/load:

- `FuzzyTenSecondHeartbeat`
- `FuzzyMinuteHeartbeat`

`GroupAI` then delegates those ticks to its `GroupAIType`.

## How AI Interacts with the Rest of the Engine
### NPC Templates and Spawned NPCs
`NPCTemplateBase` stores a list of `ArtificialIntelligences`. Builders manage these with:

- `npc set ai add <which>`
- `npc set ai remove <which>`

When a new `NPC` is created from a template, the template's AI list is copied into the NPC's `_AIs` collection. The references still point at shared AI definitions.

### Live NPC Overrides
Builders can also attach or remove AIs from an already spawned NPC with the `ai add` / `ai remove` builder flow in `NPCBuilderModule`.

This is useful when:

- a particular spawn should diverge from its template
- testing or live administration needs a quick override

### NPC Spawning
`NPCSpawner` interacts with AI in two notable ways:

- newly created NPCs receive `EventType.NPCOnGameLoadFinished`
- territory-aware open-territory spawning looks for a `TerritorialWanderer` on the template and asks it for candidate cells

That makes `NPCOnGameLoadFinished` a useful initialization hook for AI authors, and it means some AI implementations participate in spawn placement before the NPC ever starts normal behavior.

### FutureProgs
FutureProgs are a core part of the AI architecture. Many AIs use progs for:

- target selection
- gating behavior on or off
- room/path filtering
- legal or social judgment
- fees, permissions, and dialogue outcomes
- combat or threat logic

Group AI templates also use progs for:

- avoid-cell logic
- threat evaluation

The runtime pattern is generally:

- the AI keeps the long-lived reactive behavior in C#
- builder-supplied progs supply game-specific policy decisions

### Pathing and Movement
Movement-centric AI commonly relies on:

- `WandererAI`
- `PathingAIBase`
- `PathingAIWithProgTargetsBase`
- `TerritorialWanderer`
- `FlyingWanderer`
- `TrackingAggressorAI`
- `PathToLocationAI`

The pathing bases centralize door handling, path following, fallback logic, and waypoint/prog-driven destination logic. New movement-heavy AI should usually extend one of these instead of re-implementing path and heartbeat plumbing from scratch.

### Combat
Combat-facing AI currently covers several distinct roles:

- pure aggression and target acquisition
- escalation and selective aggression
- disengagement / truce handling
- rescue/support behavior
- sparring
- herd/group threat response

The dominant combat event families used by current AI are:

- `EngageInCombat`
- `EngagedInCombat`
- `LeaveCombat`
- `TargetIncapacitated`
- `TargetSlain`
- `NoNaturalTargets`
- `TruceOffered`

### Law, Shops, Doors, and Social Systems
AI is already integrated into several content-heavy subsystems:

- `EnforcerAI` and `JudgeAI` interact with legal/court systems
- `LawyerAI` integrates with courtroom and hiring flows
- `ShopkeeperAI` reacts to shop events and restocking
- `DoorguardAI` reacts to door knock and movement-command patterns
- `CommandableAI` and `MountAI` expose player-to-NPC control surfaces
- `ReactAI` handles lightweight social/environmental reactions

### Emotes and Output
Many AI types store builder-authored emotes in XML and emit them during reactions or movement. Emote validation matters because the engine often expects:

- `$0` to be the acting NPC
- `$1` to be a target when one exists

Group emotes are stricter still because they can depend on target-role availability.

### Group Membership
Group AI is a separate runtime system from per-NPC `IArtificialIntelligence`, but they are not mutually exclusive. A group member NPC may still have normal individual AI attached. In practice:

- individual AI controls local reactive behavior
- group AI controls collective priorities, movement, and alertness for the herd/pack/family

Use group AI when the behavior needs to coordinate across multiple NPCs and maintain shared group state.

## Builder Workflow
### `ai`
The `ai` command is the primary builder surface for reusable individual AI definitions.

Key operations:

- `ai list`
- `ai edit new <type> <name>`
- `ai clone <old> <new>`
- `ai edit <which>`
- `ai show <which>`
- `ai typehelp <type>`
- `ai set ...`

The concrete builder options for `ai set` come from each AI's `TypeHelpText` and `BuildingCommand(...)`.

### `npc set ai`
NPC templates attach and remove reusable AI definitions with:

- `npc set ai add <which>`
- `npc set ai remove <which>`

This is the normal way to make future spawns of a template inherit a behavior package.

### `ai add` / `ai remove`
Live NPC instances can be modified directly:

- `ai add <ai> <npc>`
- `ai remove <ai> <npc>`

Use this for live overrides and testing. Remember that the AI definition is still shared. You are changing which NPC uses that AI, not cloning the AI automatically.

### `gait`
`gait` edits reusable group AI templates.

Key operations:

- `gait list`
- `gait edit new`
- `gait edit <id>`
- `gait show`
- `gait clone <which> <newname>`
- `gait set name ...`
- `gait set avoid ...`
- `gait set threat ...`
- `gait set type <newtype> <builder args>`
- `gait set emote ...`

Template changes are immediate and non-revisable.

### `group`
`group` manages live group AI instances.

Key operations:

- `group new <template> <name>`
- `group delete <which>`
- `group show <which>`
- `group addmember <which> <who>`
- `group removemember <which> <who>`
- `group setaction <which> <action>`
- `group setalertness <which> <alertness>`

### Template vs Instance
This distinction is essential:

- `IArtificialIntelligence` is usually a shared definition reused by many NPCs
- `IGroupAITemplate` is a shared definition reused by many group instances
- `IGroupAI` is a live instance with its own mutable state

For individual AI, avoid storing per-NPC mutable state on the AI definition.
For group AI, storing mutable state on the instance is the whole point.

## Base Class Guidance
### `ArtificialIntelligenceBase`
This is the standard base for almost every individual AI implementation.

It provides:

- save/load helpers
- loader registration support
- clone behavior
- builder registration support
- default `HelpText`, `Show`, and `name` editing behavior
- utility method `IsGenerallyAble(...)`

When adding a new AI, this is usually your starting point.

### `PathingAIBase`
Use this when the AI's identity is "an NPC that periodically moves around according to rules".

It centralizes common movement concerns such as:

- doors and keys
- guards and smashing doors
- path following
- movement delays
- location filtering

### `PathingAIWithProgTargetsBase`
Use this when pathing destinations should come from content configuration instead of hard-coded logic.

It adds FutureProg-driven path control such as:

- pathing enabled checks
- target selection
- fallback location selection
- waypoint routing

### `GroupAIType`
This is the base class for most group behavior policies. It contributes:

- active time-of-day handling
- default group data with remembered water/threat locations
- default role maintenance
- leader assignment logic
- generalized movement helpers
- random emote handling

New group behavior should usually derive from this base instead of implementing `IGroupAIType` from scratch.

### `GroupAITypeFactory`
This is the discovery and builder-entry point for group AI types. If a new group type is not registered here through the reflection-based static registration pattern, builders cannot create it from `gait set type ...`.

## Concrete AI Catalogue
The table below covers every current concrete AI file in `MudSharpCore/NPC/AI`.

| AI class | Builder-creatable | Main role | Notes |
| --- | --- | --- | --- |
| `AggressivePatherAI` | Yes | Prog-target pathing AI that hunts for attack targets and engages with delay/emote support | Aggressive classification |
| `AggressorAI` | Yes | Simple immediate aggression against prog-selected targets | Aggressive classification |
| `AnimalAI` | Yes | Compositional animal AI with configurable movement, home, feeding, water, threat, awareness, refuge, and activity strategy slots | Replaces the branch-only solo predator and forager classes; stores configuration in one AI XML payload and per-NPC water/threat memories in effects |
| `ArborealWandererAI` | Yes | Tree-dwelling movement AI that prefers arboreal layers and only descends when explicitly allowed | Built on `PathingAIBase` plus multi-layer pathing |
| `ArenaParticipantAI` | Yes | Arena-aware combat AI that handles preparation, ambush options, and opponent-only targeting inside arena events | Keeps arena rules out of the general aggressor family |
| `CombatEndAI` | Yes | Controls truce acceptance and post-incapacitation combat ending behavior | Reactive combat cleanup layer |
| `CommandableAI` | Yes | Lets players command NPCs subject to prog checks and banned-command rules | Good for guards, servants, followers |
| `DenBuilderAI` | Yes | Craft-backed den or nest builder that claims a home cell and can defend it | Uses persisted per-NPC home/anchor state |
| `DoorguardAI` | Yes | Manages NPCs who respond to knocks, open/close doors, and enforce door access rules | Strongly tied to doors and command timing |
| `EnforcerAI` | Yes | Legal-authority AI that identifies and reacts to wanted criminals | Heavy legal-system integration |
| `FlyingWanderer` | Yes | Movement AI for flying creatures that wander through valid rooms/layers | Movement-focused specialization |
| `IdleEmoterAI` | Yes | Periodic idle-emote generator | Lightweight ambience AI |
| `JudgeAI` | Yes | Courtroom/legal progression AI built on top of `EnforcerAI` | Specialized judicial behavior |
| `LawyerAI` | Yes | Pathing/hiring/court-participation AI for lawyers | Legal-service specialization |
| `LairScavengerAI` | Yes | Full scavenger AI that collects items and returns them to a remembered lair/home | Reuses the same persisted home-base state as `DenBuilderAI` |
| `MountAI` | Yes | Mount/rider permission and control logic | Implements `IMountableAI` |
| `PathToLocationAI` | Yes | Prog-target pathing AI that travels to resolved locations | Generic destination-following behavior |
| `ReactAI` | Yes | Lightweight event-to-prog/emote reaction layer | Supports named reactions such as greet, farewell, weather, gift, damage, hide |
| `RescuerAI` | Yes | Helps or rescues prog-defined allies/friends | Supportive combat/social behavior |
| `ScavengeAI` | Yes | Legacy scavenging AI that evaluates items and executes a scavenging prog | Despite the name, it does not currently implement a full lair-returning scavenger loop |
| `SelfCareAI` | Yes | Performs self-preservation tasks such as responding to bleeding | Reactive health-maintenance helper |
| `SemiAggressiveAI` | Yes | Selective aggression plus movement/pathing | Aggressive classification |
| `ShopkeeperAI` | Yes | Shop staff behavior, buyer reaction, and restocking response | Shop-system integration |
| `SparPartnerAI` | Yes | Accepts or refuses spar invitations and engages accordingly | Training/combat-social niche |
| `StealthAI` | Yes | Hiding/sneaking cadence and stealth posture behavior | Tick-driven stealth utility |
| `TerritorialWanderer` | Yes | Territory-seeking and territory-maintaining wanderer AI | Also used by spawner open-territory placement |
| `TrackingAggressorAI` | Yes | Aggression AI that can track enemies over distance | Aggressive classification and pathing-heavy |
| `WandererAI` | Yes | Core wandering AI for non-flying movers | General-purpose locomotion package |
| `WildAnimalHerdAI` | Yes | Complex herd/animal behavior with its own role, threat, and reaction model | Still a legacy/advanced AI, but now creatable through the normal builder workflow |
| `ArtificialIntelligenceBase` | Not applicable | Shared base class | Not a concrete AI type |
| `PathingAIBase` | Not applicable | Shared movement/pathing base | Not a concrete AI type |
| `PathingAIWithProgTargetsBase` | Not applicable | Shared prog-target pathing base | Not a concrete AI type |

### Reading the Current AI Set
The current AI roster clusters into a few families:

- aggression and combat: `AggressorAI`, `AnimalAI`, `AggressivePatherAI`, `SemiAggressiveAI`, `TrackingAggressorAI`, `ArenaParticipantAI`, `CombatEndAI`, `RescuerAI`, `SparPartnerAI`
- movement and territory: `WandererAI`, `FlyingWanderer`, `PathToLocationAI`, `TerritorialWanderer`, `AnimalAI`, `ArborealWandererAI`
- service/content roles: `DoorguardAI`, `ShopkeeperAI`, `LawyerAI`, `JudgeAI`, `EnforcerAI`, `MountAI`, `CommandableAI`
- ambience and lightweight reactions: `IdleEmoterAI`, `ReactAI`, `StealthAI`, `SelfCareAI`
- animal/specialized behavior: `AnimalAI`, `WildAnimalHerdAI`, `ScavengeAI`, `DenBuilderAI`, `LairScavengerAI`

`AnimalAI` exposes named strategy slots to builders: `movement` (`ground`, `swim`, `fly`, `arboreal`), `home` (`none`, `territorial`, `denning`), `feeding` (`none`, `predator`, `denpredator`, `forager`, `scavenger`, `opportunist`), `water` (`on`, `off`), `threat` (`passive`, `flee`, `defend`, `hungrypredator`), `awareness` (`none`, `wary`, `wimpy`, `skittish`, `guarding`), `refuge` (`none`, `home`, `den`, `trees`, `sky`, `water`, `prog`), and `activity` (`always`, `diurnal`, `nocturnal`, `crepuscular`, `custom`). Slot-specific builder commands configure movement range and layers, territory progs, den or burrow progs and craft, predator attack progs, engagement timing, awareness threat/avoidance progs, refuge layers/cell progs, activity windows, and movement filters. `show` reports the slots together with readiness, and validation rejects impossible combinations such as `feeding denpredator` without `home denning`, `refuge sky` without flying movement, or custom activity with no active times.

The `AnimalAI` water slot uses `NpcKnownWaterLocationsEffect` to persist water memories on each NPC. The awareness slot similarly uses `NpcKnownThreatLocationsEffect` to remember recently seen threat cells with expiry. When animals see a drinkable local liquid source they remember that cell. When thirsty, they try to drink locally before eating, hunting, foraging, returning home, or building; pathing variants first attempt remembered water cells and then search nearby cells for a new drinkable liquid source. If a remembered water cell is dry when visited, the NPC forgets that location. Wary, wimpy, and skittish animals consult current threats, avoid-cell progs, and remembered threat locations before survival needs.

The `AnimalAI` feeding, refuge, activity, and home slots cover the former branch-only solo predator and forager behavior plus basic solitary animal instincts. Predator variants eat local edible corpses before hunting and only choose edible prey when using hungry-predator threat behavior. Den predators claim killed prey, drag the corpse back to the den, and resume fighting if attacked while dragging or eating. Forager variants eat direct edible yields first and use `FORAGE` when eligible forageables exist. Scavengers eat corpses, severed bodyparts, and edible items without hunting; opportunists combine scavenging and foraging without adding predator attacks. Denning animals return to their home cell when fed and watered, and can optionally create a burrow through the configured craft. Refuge and activity settings allow patterns such as wimpy animals retreating to a configured safe place, skittish arboreal animals returning to tree layers, and flyers returning to sky layers once thirst and hunger are satisfied.

## Group AI Catalogue
### Current Group Types
Current builder-registered group AI types live under `MudSharpCore/NPC/AI/Groups/GroupTypes`.

| Group AI type | Main role | Notes |
| --- | --- | --- |
| `FamilyPredatorGroup` | Coordinated family-style predator packs | Uses predator-group behavior with a home/family bias |
| `NeutralHerdGrazers` | Herding grazers that react defensively but are not strongly territorial | Good baseline passive herd |
| `TerritorialHerdGrazer` | Herd grazers with territorial behavior | Combines herd and territory pressure |
| `WimpyHerdGrazers` | Highly evasive grazer herds | Strong flee/avoid posture |

There are also shared bases:

- `GroupAIType`
- `PredatorGroupBase`
- `HerdGrazers`

### Alertness Model
Current alertness states are:

- `NotAlert`
- `Wary`
- `Agitated`
- `VeryAgitated`
- `Aggressive`
- `Broken`

Template threat progs and type-specific threat logic push the group between these states. Alertness then influences:

- emote eligibility
- threat response
- priority selection
- movement and posture decisions

### Action Model
Current `GroupAction` values include:

- `FindWater`
- `FindFood`
- `Graze`
- `Sleep`
- `Rest`
- `Alert`
- `FindShelter`
- `AvoidThreat`
- `Flee`
- `FleeToPreferredTerrain`
- `FleeToDen`
- `Posture`
- `ControlledRetreat`
- `AttackThreats`
- `MoveFood`
- `FindMate`
- `Mate`

In practice, only some are used by the current stock group types. The enum is broader than current implementation coverage.

### Role Assignment
Current roles are:

- `Leader`
- `Pretender`
- `Child`
- `Elder`
- `Outsider`
- `Adult`

`GroupAIType.EvaluateGroupRolesAndMemberships(...)` assigns missing roles, removes stale role entries for departed members, and ensures a leader exists.

### Group Emotes
Group emotes live on the template and are filtered by:

- emoter gender
- emoter role
- target role
- age category
- required action
- min/max alertness

When a target role is required, the emote text must remain valid with a `$1` target. Removing the target requirement without fixing the emote text is explicitly guarded against in the builder flow.

### Template Progs
Current template-level group progs are:

- avoid-cell prog
- threat-evaluation prog

Both support multiple compatible signatures so builders can choose whether alertness is passed as text, number, both, or not at all.

### Group Data Persistence
The group type owns runtime XML data through `IGroupTypeData`. The shared base data already tracks:

- last emote time
- known water locations
- known threat locations

Type-specific implementations extend that data with their own state.

## Event and Hook Relationship
AI depends heavily on the event system but does not use it in exactly the same way as hooks.

At a high level:

- individual AI receives events directly through `NPC.HandleEvent(...)`
- hooks receive events through `IHook.Function`
- heartbeat events for NPC AI are opt-in through `HandlesEvent`
- heartbeat events for group AI are directly subscribed by the group instance

Use [Event_System_for_AI_and_Hooks.md](./Event_System_for_AI_and_Hooks.md) for the event contract and hook workflow details.

## Implementation Guidelines, Edge Cases, and Tips
### 1. Treat XML as a real schema
Each concrete AI owns its own `Definition` XML. If you evolve it:

- preserve load compatibility when possible
- make missing-element defaults explicit
- keep save and load logic in sync
- avoid hidden coupling between XML order and parsing

### 2. `HandlesEvent` is operational, not decorative
For NPC AI, `HandlesEvent` controls heartbeat subscription behavior through `NPC.SetupEventSubscriptions()`. Over-claiming events has real runtime cost.

Claim only the events the AI actually handles.

### 3. Event argument ordering is brittle
`HandleEvent(EventType type, params dynamic[] arguments)` depends on exact argument ordering. Always verify the order against:

- `EventTypeEnum.cs`
- `EventInfoAttribute`
- `show event <event>`

Do not infer argument order from the event name.

### 4. Builder registration is easy to miss
For a normal new AI type, you usually need all of:

- `RegisterAIType(...)`
- `RegisterAIBuilderInformation(...)`
- public static `RegisterLoader()`
- `SaveToXml()`
- load constructor from database row
- builder help text

If any of those are missing, the AI may load but not be creatable, or be creatable but not survivable across reboot.

### 5. Shared AI definitions are not per-NPC state
If you need mutable state that is different for each NPC, do not casually put it on an `IArtificialIntelligence` implementation. Either:

- derive it from events/effects on the NPC
- persist it elsewhere
- redesign the behavior around group or character state

### 6. `CountsAsAggressive` matters
Several AIs explicitly override `CountsAsAggressive`. New combat AI should set this deliberately rather than leaving the default accidentally.

### 7. Reuse the pathing bases
If the AI moves toward places or targets, start from `PathingAIBase` or `PathingAIWithProgTargetsBase` unless there is a strong reason not to. They already encode the engine's expected door, movement, and path-following behavior.

### 8. Prefer prog-driven policy over AI proliferation
If the behavior difference is mostly "who counts as valid", "which room qualifies", or "when should this be on", prefer a configurable prog before creating an entirely new AI class.

### 9. Prefer group AI for truly shared behavior
If the behavior depends on:

- multiple NPCs coordinating
- shared alertness
- leader/member roles
- remembered group resources or threats

then it likely belongs in group AI rather than duplicated per-NPC AI.

### 10. `WildAnimalHerdAI` is still a special case
`WildAnimalHerdAI` is now builder-creatable, but it is still more complex and more stateful than the ordinary AI set. Treat it as a legacy/advanced implementation, not as the default pattern to copy.

### 11. Group AI template edits are immediate
Because `IGroupAITemplate` is non-revisable, changes affect live behavior immediately. That is powerful, but it also means builders need to be careful with edits to progs, emotes, and type changes.

### 12. Template sharing is a feature and a risk
The same AI or group template may be reused widely. Before editing a shared AI definition, confirm whether you actually want to change all consumers or whether a clone would be safer.

## When to Use What
### Use a simple individual AI when

- behavior is local to one NPC
- decisions are mostly reactive
- little or no pathfinding is needed

Examples: `IdleEmoterAI`, `SelfCareAI`, `CombatEndAI`, `ReactAI`.

### Use a pathing AI when

- an NPC needs to move around intentionally
- room filtering and door behavior matter
- destinations come from fixed logic or progs

Examples: `WandererAI`, `FlyingWanderer`, `PathToLocationAI`, `TrackingAggressorAI`.

### Use a group AI when

- multiple NPCs need shared alertness and action priorities
- roles such as leader/outsider/child matter
- behavior should be coordinated at herd or pack scale

Examples: grazer herds and predator families.

## Recently Closed Runtime Gaps
The following individual-AI gaps described in earlier revisions of this document are now implemented:

### Arena-combat specialization
`ArenaParticipantAI` now provides an arena-specific combat family that:

- only activates while the NPC is attached to arena preparation/participation state
- limits targeting to opponents in the same arena event on other sides
- handles pre-fight preparation such as weapon-ready and optional ambush/hide behavior
- uses pathing to pursue arena opponents when they are not currently visible

### Den-building and lair ownership
`DenBuilderAI` now provides a craft-backed den or nest builder that:

- chooses and remembers a home cell per NPC
- starts or resumes a configured den craft at that home cell
- can identify and remember a den anchor item once the build is complete
- can optionally defend the claimed den via a prog

This is supported by a persisted per-NPC home-base effect (`NpcHomeBaseEffect`) that stores the home cell and optional anchor item without leaking that state onto shared AI definitions.

### Arboreal specialization
`ArborealWandererAI` now fills the tree-dwelling movement gap by:

- preferring configured tree-capable layers
- wandering among cells whose terrain supports arboreal layers
- descending only when an explicit descent prog allows it

### True lair-return scavenging
`ScavengeAI` remains the legacy "evaluate and trigger a scavenging prog" AI.

`LairScavengerAI` is now the fuller scavenger implementation that:

- actively evaluates and collects visible items
- reuses the persisted home/lair state from `DenBuilderAI` when available
- falls back to a configured home-location prog otherwise
- returns scavenged items to the lair and deposits or drops them there

### Existing-type fixes landed during this pass
- `WildAnimalHerdAI` now has normal builder registration and safer defaults for newly created instances.
- `AggressivePatherAI` and `TrackingAggressorAI` now subscribe correctly to `CharacterEnterCellWitness`.
- `TerritorialWanderer` now accepts valid percentage input for the `chance` builder command.
- `ScavengeAI` now validates and displays `OnScavengeItemProg` correctly.

### Remaining out-of-scope note
The group-AI runtime still has room to exploit more of the broader `GroupAction` surface, but that is separate from the individual-AI work described here.

## Practical Extension Strategy
When adding new AI, the safest progression is usually:

1. Decide whether the behavior is individual or group-oriented.
2. Reuse the nearest existing base (`ArtificialIntelligenceBase`, `PathingAIBase`, `PathingAIWithProgTargetsBase`, or `GroupAIType`).
3. Put world-specific policy into FutureProgs where possible.
4. Add builder help and validation at the same time as runtime logic.
5. Check the event metadata before wiring any new event-handling code.
6. Clone existing shared AI definitions instead of mutating them blindly when live content already depends on them.

## Related References
- Companion event document: [Event_System_for_AI_and_Hooks.md](./Event_System_for_AI_and_Hooks.md)
- Existing AI storyteller design for contrast with this subsystem: [AI_Storyteller_Design.md](./AI_Storyteller_Design.md)
