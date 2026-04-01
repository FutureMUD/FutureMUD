# FutureMUD Item System Runtime Model

## Scope
This document explains how the item system is structured in code and at runtime:

- live items versus revisioned definitions
- item composition through components
- persistence and load flows
- revision updates
- morph and destruction behaviour

## Primary Abstractions
### `IGameItem`
`IGameItem` is the live object that exists in the world. It extends perceiver and body-adjacent interfaces, so items are not just data records; they are active world objects with descriptions, location, health, events, and effects.

Important responsibilities include:
- holding runtime component instances through `Components`
- exposing convenience queries like `IsItemType<T>()`, `GetItemType<T>()`, and `GetItemTypes<T>()`
- tracking containment, inventory ownership, and true locations
- handling save/load, deletion, quitting, login, morphing, and update checks
- aggregating behaviour from components rather than owning all behaviour directly

In practice, `GameItem` is the orchestration layer. Components provide most specialised behaviour, while `GameItem` coordinates persistence, description composition, movement, inventory state, wound handling, and world integration.

### `IGameItemProto`
`IGameItemProto` is the revisioned prototype for creating items.

It owns the reusable definition of:
- name and descriptive text
- base quality, weight, material, and size
- health strategy
- default registers
- attached component prototypes
- item group
- morph and destroyed-item setup
- on-load progs
- skin permissions and visibility flags

Prototype methods like `CreateNew(...)` instantiate a `GameItem`, create one runtime component per attached component prototype, apply variable initialisation, and execute on-load progs.

### `IGameItemComponent`
`IGameItemComponent` is the runtime slice of behaviour attached to a live item.

A component may:
- implement gameplay interfaces such as `IContainer`, `IOpenable`, `IWearable`, or `IRangedWeapon`
- participate in saving/loading
- decorate descriptions
- block movement or repositioning
- react to morph/destruction
- manage contained, attached, or connected sub-items
- override material, weight contribution, buoyancy, and purge warnings

Runtime item behaviour is usually discovered by interface lookup on components. Code commonly asks questions like:
- does this item have an `IContainer`?
- does this item have an `IProduceLight`?
- does this item have an `IImplant`?

### `IGameItemComponentProto`
`IGameItemComponentProto` is the revisioned reusable definition of a component type instance.

It owns:
- builder-editable configuration
- XML persistence for that configuration
- the factory methods that create and load runtime components
- type help and builder help text
- flags such as `WarnBeforePurge` and `PreventManualLoad`

## Composition Model
### Components define capabilities
The item system is intentionally interface-first and component-driven.

A live item becomes "a container" because one of its components implements `IContainer`. It becomes "a wearable" because one of its components implements `IWearable`. It becomes "a radio", "a telephone", "a cell tower", "a corpse", "a battery-powered machine", or "a prosthetic" for the same reason.

This has two important consequences:
- most game logic should depend on interfaces from `FutureMUDLibrary`, not concrete component classes
- adding a new capability usually means adding a new component pair, not adding a new item class

Some item capabilities are best thought of as paired systems rather than isolated behaviours. Power, liquid, and telecommunications examples often combine:
- a grid creator that establishes the shared network
- a supply component that contributes to the grid
- one or more connectable consumer or connector components that physically join items together

Those relationships are usually expressed through `IConnectable` plus a domain-specific grid interface such as `ICanConnectToElectricalGrid`, `ICanConnectToLiquidGrid`, or `ICanConnectToTelecommunicationsGrid`.

Some subsystems also need reusable runtime data that is not owned by one concrete item type. Recorded audio is now the reference pattern:
- immutable audio payloads live in `FutureMUDLibrary/Form/Audio`
- a recording is an ordered list of utterance segments plus the elapsed delay before each segment
- each segment snapshots the language id, accent id, raw text, volume, speech outcome, and immutable speaker identity metadata needed to recreate later playback without consulting the speaker's current state
- the shared model owns XML round-tripping so stage-1 item implementations can persist recordings in ordinary component XML rather than new database tables

### Telecommunications and cellular pattern
Telecommunications items are a useful example of how multiple item capabilities compose into one subsystem:
- wired handsets implement `ITelephone`, but the active phone number may belong to a separate `ITelephoneNumberOwner` endpoint such as a telecommunications outlet
- fax machines layer `IFaxMachine` on top of the same telecom-numbering model, but they also own machine runtime state such as paper storage, ink availability, and pending inbound fax memory
- telecommunications grids persist number ownership against the specific endpoint component identity, not just the parent item, so multiple telecom endpoints on one item keep distinct numbers across save/load
- a telecommunications grid is also a specialised power network, so telecom-connected devices can draw power from producers on that grid without exposing themselves as ordinary electrical-service endpoints
- each telecommunications grid owns exchange-level behaviour such as prefix, subscriber length, maximum rings before timeout, and direct exchange links used for long-distance routing
- long-distance routing is direct and prefix-based: a call only forwards to directly linked exchanges, never multi-hop chains, and a local-prefix dial always resolves locally instead of forwarding
- shared numbers are valid at the endpoint layer, which allows multiple outlets or towers to ring for the same number and later join the same live call
- fax routing is a sibling path rather than a speech-call variant: the grid resolves the same addressed number ownership, but voice-to-fax and fax-to-voice mismatches should surface as modem-like noise instead of a normal connected conversation
- cellular phones still implement `ITelephone`, but they own their own number, require a separate local power source, and only function when a powered `ICellPhoneTower` on the same telecommunications grid covers their zone
- physical telephones and cellular phones each have an effective ring volume resolved from a prototype default plus the handset's current player-selected ring setting; wired phones expose `quiet`, `normal`, and `loud`, while cellular phones add `silent`
- silent cellular phones do not emit room audio, but if the handset is tucked into a worn container they can still notify the wearer with a non-audible vibration message; implant telephones remain text-only and silent to the room
- implant telephones follow the same cellular coverage rules as handheld cellular phones, but they are also implants: they draw power through implant power infrastructure and expose control/status through neural-interface implant commands rather than ordinary handheld room commands
- answering machines are daisy-chained endpoints: they can sit between an outlet and downstream handsets, expose both themselves and those downstream phones through `ConnectedTelephones`, and locally answer a ringing line before any future hosted voicemail layer
- telecommunications grids can now also host an exchange voicemail service: the grid owns the service toggle, a reserved access number on the local prefix, and central per-number mailboxes stored in grid XML rather than on a physical item
- hosted voicemail is opt-in twice: the exchange must enable the service, and each `ITelephoneNumberOwner` must enable it for its own number before unanswered calls route into the hosted mailbox
- same-line mailbox retrieval in v1 happens by dialling the exchange voicemail access number from the subscribed line; once connected, keypad digits drive playback and deletion over the existing in-call digit relay
- active calls now relay both speech and explicit keypad digits; `dial <phone> <number>` still starts a call while idle, but the same command becomes in-call digit transmission once the handset is already connected
- items and services that receive keypad digits get `EventType.TelephoneDigitsReceived`, which is the public extension point for future voicemail, IVR, routing, or keypad-driven automation

Grid creator items for power, liquid, and telecommunications are also responsible for owned-grid recovery. If a creator item loads with a missing or zero grid id, it recreates the expected grid immediately and direct-initialises it before the creator item can be saved again.

### `GameItem` aggregates component behaviour
`GameItem` delegates and aggregates a large amount of behaviour:
- movement prevention is aggregated across components
- description decoration is ordered by `DecorationPriority`
- material can be overridden by components
- attached and connected item relationships are exposed through component-provided interfaces
- location resolution checks components such as chairs, doors, belts, connectables, worn items, implants, and prosthetics
- connector-aware components can also persist and restore linked items so that networks survive save/load cycles

This aggregation layer is why item code often looks simple at the call site even when item behaviour is complex.

## Prototypes, Revisions, and Live Items
### Revisioned content model
Item prototypes and component prototypes are editable revisable items. They move through the normal FutureMUD revision flow:
- under design
- pending revision
- current
- obsolete

Items loaded into the world are instances of specific current definitions at the time of creation, but both item prototypes and live items have update paths to move to newer component or prototype revisions.

### Prototype composition
A prototype stores attached component prototypes through the many-to-many revision-aware relationship between item prototypes and component prototypes.

When a prototype creates a new item:
1. a `GameItem` instance is created
2. each attached component prototype creates one runtime component instance
3. variable or stackable setup runs if applicable
4. skin overrides are applied if provided
5. on-load progs execute

## Save and Load Flow
### Loading prototypes
At boot, `FuturemudLoaders` loads:
1. item component prototypes
2. item prototypes
3. special auto-initialised item types
4. item skins

The ordering matters:
- component prototypes must exist before item prototypes
- special auto-initialisers such as `HoldableGameItemComponentProto.InitialiseItemType` run after item prototypes load

### Loading live items
When a live item is loaded:
- the `GameItem` is created from database data
- each stored component row is mapped back to its component proto and runtime component type
- component `FinaliseLoad()` hooks run after broader object availability is established
- item-level late initialisation and effect restoration complete

### Saving live items
`GameItem.Save()` is responsible for item-level persistence such as:
- prototype revision references
- quality, material, ownership, morph progress, and position
- effect and magic data

Each component persists its own XML definition through `GameItemComponent.Save()` and `SaveToXml()`.

## Update Behaviour
### Prototype update checks
`GameItemProto.CheckForComponentPrototypeUpdates()` updates a prototype if any attached component prototype has moved from current to revised or obsolete.

That process:
- swaps old component prototype references to the newest current revision
- updates the persisted mapping rows
- keeps the prototype definition aligned with the latest component prototype revisions

### Live item update checks
`GameItem.CheckPrototypeForUpdate()` updates live items when:
- the parent item prototype has been revised or obsoleted
- attached component membership has changed
- a component prototype revision has changed

It can:
- swap the item to a newer prototype revision
- create runtime components that newly exist on the prototype
- remove runtime components that no longer belong
- ask each runtime component to update itself to a newer prototype revision

The builder-facing `comp update` workflow exists to force these update passes across prototypes and items.

## Morphing, Destruction, and Replacement
### Morphing
Item prototypes can define morph behaviour:
- morph into another item after a timespan
- disappear after a timespan
- emit a morph emote

Morph timing is tracked on live items, and new items may preserve register values depending on prototype settings.

### Destroyed items
Item prototypes can also define a replacement prototype to load when the item is destroyed.

This allows patterns such as:
- intact item -> wreckage
- living body item -> corpse-style remains
- powered machine -> broken shell

### Component participation
Components can influence these transitions through:
- `HandleDieOrMorph`
- `SwapInPlace`
- `Take`
- `AffectsLocationOnDestruction`
- `ComponentDieOrder`

Container-style components are a good example: they often need to decide what happens to contents or locks when the parent item morphs or dies.

## Real Example: Container
`ContainerGameItemComponentProto` is a representative example of a typical editable component proto:
- stores builder-editable values like weight limit, max size, transparency, and preposition
- loads and saves its configuration as XML
- registers itself with `GameItemComponentManager`
- exposes type help and component-specific builder help
- creates and loads the runtime `ContainerGameItemComponent`

`ContainerGameItemComponent` then provides runtime behaviour by implementing `IContainer`, `IOpenable`, and `ILockable`.

It demonstrates several common component patterns:
- internal runtime state layered on top of a proto definition
- description decoration
- handling contained child items
- weight and buoyancy contribution
- destruction and morph transfer logic
- `Copy(...)` support for deep-copy item creation

## Real Example: Tape and Answering Machine
`TapeGameItemComponent` plus `AnsweringMachineGameItemComponent` is the current reference for a mixed media-and-telecom subsystem.

`TapeGameItemComponentProto` authors only storage capacity. The runtime tape component owns:
- write-protect state
- named stored recordings
- used and remaining capacity calculations
- XML persistence for those recordings

`AnsweringMachineGameItemComponentProto` authors:
- power draw while switched on
- default ring emote and ring setting
- the transmit premote used for live speech relays
- default rings before auto-answer
- connector shapes for upstream and downstream telephone-line chaining

The answering-machine runtime component then owns:
- number ownership or delegation to an upstream outlet
- live ringing, call participation, and auto-answer timing
- scheduled greeting playback, beep emission, and caller-message recording
- a one-slot tape container workflow
- `ISelectable` commands for `on`, `off`, `rings`, `greeting ...`, `messages play`, `message <index>`, and `erase ...`
- recursive discovery of downstream handsets so a human pickup on an extension can displace the machine from the live call

## Special Cases
### Read-only or auto-initialised component types
Not every component type behaves like a normal editable component proto.

`HoldableGameItemComponentProto` is a key example:
- it is read-only
- it is auto-initialised through a static `InitialiseItemType`
- it is used as an always-available foundational capability when configured by world settings

When documenting or extending the system, treat these as framework-level special cases rather than normal builder-authored component content.

## Practical Guidance
- Reach for interfaces in `FutureMUDLibrary` first. Runtime systems should usually depend on `IContainer`, `IReadable`, `ITransmit`, and similar interfaces.
- Treat `GameItem` as the composition shell, not the place to add every feature directly.
- Put configuration on the component proto and runtime state on the component.
- Expect update, morph, and destruction flows to matter for any non-trivial item behaviour.
