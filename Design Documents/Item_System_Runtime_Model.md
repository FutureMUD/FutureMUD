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

Planar presence is resolved at the item level before component-specific physical interaction. A visible but non-interactable item can still appear in descriptions or be speech/observation targeted where commands allow it, while inventory and manipulation checks can reject it through `CanInteractPlanar`.

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
- default `PlanarData` for corporeality and metaphysical visibility

Prototype methods like `CreateNew(...)` instantiate a `GameItem`, create one runtime component per attached component prototype, apply variable initialisation, and execute on-load progs.

If `PlanarData` is null or invalid, the item prototype resolves as ordinary Prime Material corporeal matter. Builders can use the item prototype `planar` command to make special items present on another plane, visible-only to another plane, or fully noncorporeal.

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

Component protos also advertise their runtime capability contracts through marker interfaces such as `IContainerPrototype`, `IWearablePrototype`, or `ILiquidContainerPrototype`. Item prototypes use these markers to reject duplicate exclusive capabilities when builders attach components or submit an item prototype for review.

## Composition Model
### Components define capabilities
The item system is intentionally interface-first and component-driven.

A live item becomes "a container" because one of its components implements `IContainer`. It becomes "a wearable" because one of its components implements `IWearable`. It becomes "a radio", "a telephone", "a cell tower", "a corpse", "a battery-powered machine", or "a prosthetic" for the same reason.

New-style food follows the same rule. A live item becomes prepared food because one component implements `IPreparedFood`, which extends `IEdible` with ingredient ledgers, freshness, serving scope, quality-scaled nutrition, and ingested drug doses. The legacy `Food` component remains available for old content, while `PreparedFood` is the current component for direct loadable foods and recipe-initialised foods.

This has two important consequences:
- most game logic should depend on interfaces from `FutureMUDLibrary`, not concrete component classes
- adding a new capability usually means adding a new component pair, not adding a new item class
- component protos must expose matching prototype markers so the builder flow can distinguish exclusive roles from aggregate service roles

Some item capabilities are best thought of as paired systems rather than isolated behaviours. Power, liquid, and telecommunications examples often combine:
- a grid creator that establishes the shared network
- a supply component that contributes to the grid
- one or more connectable consumer or connector components that physically join items together

Those relationships are usually expressed through `IConnectable` plus a domain-specific grid interface such as `ICanConnectToElectricalGrid`, `ICanConnectToLiquidGrid`, or `ICanConnectToTelecommunicationsGrid`.

Zero-gravity items follow the same interface-first pattern:
- `IZeroGravityAnchorItem` marks a component-backed object as a push-off anchor
- `IZeroGravityTetherItem` supplies a physical tether length for the `tether` command
- `IZeroGravityPropulsion` marks wearable propulsion such as an RCS thruster

Runtime tether constraints are effects rather than hardcoded item state. Physical tether items and magical tethers both create an `IZeroGravityTetherEffect`; physical tethers include a backing item, while spell-created tethers use no backing item. Fixed items can also be anchored through `FixedInPlaceEffect`, which combines no-get behaviour with zero-gravity anchoring.

Some subsystems also need reusable runtime data that is not owned by one concrete item type. Recorded audio is now the reference pattern:
- immutable audio payloads live in `FutureMUDLibrary/Form/Audio`
- a recording is an ordered list of utterance segments plus the elapsed delay before each segment
- each segment snapshots the language id, accent id, raw text, volume, speech outcome, and immutable speaker identity metadata needed to recreate later playback without consulting the speaker's current state
- the shared model owns XML round-tripping so stage-1 item implementations can persist recordings in ordinary component XML rather than new database tables

### Computers and signal automation pattern
The planned computer-programs subsystem follows the same composition rules:
- shared contracts live in `FutureMUDLibrary/Computers`
- item-facing automation contracts such as `ISignalSourceComponent`, `ISignalSinkComponent`, and `IMicrocontroller` live in `FutureMUDLibrary/GameItems/Interfaces`
- live item behaviour should come from item components, not special-case `GameItem` subclasses
- common signal semantics should be expressed through interfaces like `ISignalSource` and `ISignalSink`
- concrete runtime behaviour should still be split into distinct component families such as `ComputerHost`, `ComputerTerminal`, `ComputerStorage`, `NetworkAdapter`, `NetworkSwitch`, `WirelessModem`, `Microcontroller`, `PushButton`, `MotionSensor`, `ElectronicDoor`, and `SignalLight`
- standalone player-owned computer code still exists outside the item runtime in `FutureMUDLibrary/Computers` / `MudSharpCore/Computers` as `ICharacterComputerWorkspace`, `IComputerExecutionService`, and `IComputerHelpService`, but the same runtime now also supports real host-owned and storage-owned executables through `IComputerExecutableOwner`
- host-owned and storage-owned file systems now also carry live network-file state such as per-file public visibility flags, while `ComputerHost` runtime configuration additionally carries FTP account data for the built-in remote file-transfer service
- the broader mutable file-surface contract is now `IComputerFileOwner`, which allows non-executable item components such as automation sources to expose a file system to local or remote file tools without also becoming full executable owners

This means "computerised" items are expected to compose multiple capabilities:
- a host component to own files, executables, and running processes
- one or more terminal or network-facing components for user and remote interaction
- signal source and sink components for electrical-style automation
- ordinary door, lock, light, or switch components when the item also has traditional physical behaviour

The current shipped computer-host slice now provides those first concrete families:
- `ComputerHost` is a powered machine component plus `IConnectable` that owns files, executables, processes, mounted storage, terminal connections, network adapters, and built-in application exposure
- `ComputerStorage` is an `IConnectable` storage owner that persists files and executables and can be mounted into a host
- `ComputerTerminal` is a powered machine component plus `IConnectable` that owns live player sessions into a connected powered host
- `NetworkAdapter` is a powered machine component plus `IConnectable` that exposes the host's telecom-backed network-facing readiness, preferred address, stable device identifier, access route memberships, canonical published address, and attached `ITelecommunicationsGrid`
- `NetworkSwitch` is a powered `INetworkInfrastructure` plus `IConnectable` component that can attach directly to a telecommunications grid or uplink through another infrastructure item, then daisy-chain that transport to downstream adapters and switches
- `WirelessModem` is a powered machine component plus `IConnectable` that exposes the same host-facing network contract as `NetworkAdapter`, but resolves transport by live `ICellPhoneTower` coverage rather than a direct physical uplink

Built-in applications now follow the same host-owned runtime model rather than existing as a disconnected catalog:
- a real `ComputerHost` exposes built-in application definitions as host-bound built-in programs
- those built-ins are not exposed by the private workspace or mounted storage devices
- executing one creates a real host process through the shared computer execution service
- in the current shipped phase `SysMon`, `FileManager`, `Directory`, `Mail`, `FTP`, and `Boards` have implemented built-in behaviour, while `Messenger` remains reserved for a future phase
- shared network identity is now a runtime abstraction above the first mail-backed persistence slice: hosts manage generic hosted domains and `user@domain` accounts through `IComputerNetworkIdentityService`, even though the current persisted backing tables are still the mail domain and mail account tables
- `Mail` is also the first shipped database-backed network service: mail domains, accounts, messages, and mailbox entries persist in dedicated EF-backed tables, while the host only owns the live service enablement and hosted-domain bindings
- `FTP` is the first shipped XML-backed network file service: the host owns live service enablement and account state, each owner file system owns its per-file public visibility flags, and the telecom-backed runtime layer resolves anonymous or authenticated remote access across reachable hosts

As with telecommunications, the runtime goal is interface-first integration. Game logic should ask for capabilities such as `IComputerHost` or `ISignalSink`, while the item itself remains the orchestration shell that aggregates whichever concrete components are attached.

The currently implemented automation runtime slice is intentionally narrower than the full target design:
- `PushButton` is an `ISelectable` same-item signal source with authored keyword, signal value, duration, and press emote
- `ToggleSwitch` is an `ISwitchable` same-item signal source with authored on and off values
- `MotionSensor` is a `PoweredMachineBaseGameItemComponent` same-item signal source that listens for witnessed movement events, filters by detection mode and minimum size, and emits a timed numeric signal
- `LightSensor` is a `PoweredMachineBaseGameItemComponent` same-item signal source that polls the current ambient illumination and emits that lux value as its signal
- `RainSensor` is a `PoweredMachineBaseGameItemComponent` same-item signal source that polls the current weather and emits a numeric rain-intensity scale while its location is climate-exposed
- `TemperatureSensor` is a `PoweredMachineBaseGameItemComponent` same-item signal source that polls the current ambient temperature and emits that value in Celsius
- `TimerSensor` is a `PoweredMachineBaseGameItemComponent` same-item signal source that alternates between authored active and inactive values on a recurring persisted cycle
- `Keypad` is a `PoweredMachineBaseGameItemComponent` plus `ISelectable` that accepts numeric `select <item> <digits>` input and emits a momentary numeric signal only when the entered code matches its authored code
- `FileSignalGenerator` is a `PoweredMachineBaseGameItemComponent` plus `ISignalSourceComponent` and `IComputerFileOwner` that owns a small mutable file system, parses one designated text file as a numeric signal, and emits that parsed value while switched on and powered
- `Microcontroller` is a `PoweredMachineBaseGameItemComponent` plus `IMicrocontroller` that:
  - binds named inputs to local `ISignalSourceComponent` instances
  - keeps live numeric input values
  - compiles authored inline logic in the `ComputerFunction` compilation context
  - emits a single numeric output signal
- `AutomationMountHost` is an `IConnectable` plus `IAutomationMountHost` component that owns named automation bays, persists installed module item ids, and can require a sibling `AutomationHousing` component before those bays are serviceable
- `SignalCableSegment` is an `ISignalSourceComponent` wire item that stores a source binding plus source and destination cells and a routed exit id, then mirrors the source endpoint across that one adjacent-room hop
- `SignalLight` is a signal sink layered on top of programmable-light runtime behaviour
- `ElectronicDoor` is a standalone signal-driven door component built on the shared internal door runtime base and retries until it reaches the currently commanded open or closed state
- `ElectronicLock` is a signal sink layered on top of programmable-lock runtime behaviour
- `RelaySwitch` is a signal-driven power producer layered on top of programmable power-supply behaviour, closing or opening its relay according to its configured threshold logic
- `AlarmSiren` is a `PoweredMachineBaseGameItemComponent` plus `ISignalSinkComponent` that resolves a sibling source, evaluates threshold logic, and emits repeated audible output while active, switched on, and powered

The current live-configuration runtime layer also adds:
- `IRuntimeConfigurableSignalSinkComponent` for sinks whose local binding, threshold, and activation mode can be changed on a live item
- `IRuntimeProgrammableMicrocontroller` for controllers whose inline logic and local input bindings can be changed on a live item
- `LocalSignalBinding` and `MicrocontrollerRuntimeInputBinding` as the stable runtime payloads for live local endpoint bindings

Current runtime connection rules for that slice are:
- sinks and microcontroller inputs resolve their upstream sources by stable local source identifiers plus explicit endpoint keys
- purely local live bindings still resolve on the same parent item
- mounted modules remain separate `IGameItem` instances connected through `AutomationMountHost` bays and move with the host item
- mounted modules inherit spatial context through their host item for `TrueLocations`, perception, and local signal discovery even while removed from ordinary room inventory
- `SignalCableSegment` is the current external wiring layer: one item mirrors one source endpoint across one adjacent-room exit hop, and longer runs require more cable items
- live player rewiring stores the runtime component id plus endpoint key in `LocalSignalBinding`, while builder-authored prototype defaults still use prototype-oriented identifiers
- the currently shipped built-in local source families each expose a single default output endpoint key named `signal`
- builder commands still accept component prototype names or ids, but stored bindings no longer depend on future component renames
- one sink definition points at one source endpoint
- microcontrollers do explicit aggregation by binding multiple input names and recomputing their own single output
- output propagation is event-driven and suppressed when the computed signal value has not actually changed
- motion sensors currently listen only to witnessed movement events on the same item/location path; they do not yet participate in cross-item or inventory-relayed signal graphs
- timer sensors currently generate their own recurring same-item phase changes from a persisted cycle anchor rather than an external event source
- file-backed signal generators treat file changes as their triggering event source: editing the owned file locally or through network file tools recomputes the parsed signal state and re-emits if the live output value changes
- powered machine automation modules can be authored to draw power from their automation host's parent-item power source when mounted, including compatible attached or connected power-producing items on that host; otherwise powered machines still resolve power from their own parent item
- mounted automation modules lazily restore their host linkage from saved host identity during load/login so host-derived power, signal access, and room context continue to work after a reboot
- mounted automation modules now follow the shared item lifecycle contract: load restores structure, while `Login()` is the first point where they begin live power drawdown, signal subscriptions, timers, and similar active behaviour
- world boot now logs in only world-root items that are actually present in cells; items rooted in character inventories stay dormant until their owning body or character logs in and propagates the lifecycle to them
- `AutomationMountHost` forwards `Login()`, `Quit()`, and `Delete()` to mounted bay items so extracted modules still come online and tear down with their host item even though they are not ordinary cell-contained items
- powered machine automation modules and other powered-machine-based automation components now treat power resolution as a topology-aware live process rather than a single login-time lookup: they subscribe to relevant parent and host power-topology changes, retry for a longer post-login window if switched on but initially unpowered, and refresh power resolution when connected or mounted power availability changes after reboot/load ordering
- `ElectronicDoor` now performs the same kind of late reconnect retry for its signal binding after load/login, so a controller or mounted module that becomes discoverable slightly later in the reboot sequence can still subscribe and drive the door without manual rewiring
- witnessed-movement automation ignores movers with `IImmwalkEffect`, and the movement event pipeline suppresses those events at the source as well, so administrator immwalk traversal does not trip motion-driven automation
- `AutomationHousing` is the dedicated housing or junction component family for concealed automation modules and cable ends, and is itself the lockable-container service-access capability on the item rather than a passive sibling marker
- automation hosts now use sibling `AutomationHousing` components for mount-bay service access
- `AutomationMountHost` forwards `Quit()` / `Delete()` teardown to installed mounted items so extracted bay modules do not remain live when their host leaves the game or is destroyed
- there is still not yet a broader persisted multi-hop signal graph or explicit electrical-network runtime object beyond mounted modules and cable segments
- local computer runtime now also exposes:
  - generic executable ownership through `IComputerExecutableOwner`
  - generic mutable file ownership through `IComputerFileOwner`
  - mutable host and storage owners through `IComputerMutableOwner`
  - terminal-scoped execution context via `IComputerTerminalSession`
  - local file, terminal, and signal-wait functions such as `ReadFile`, `WriteFile`, `AppendFile`, `FileExists`, `GetFiles`, `WriteTerminal`, `ClearTerminal`, `UserInput`, `WaitSignal`, `LaunchProgram`, and `KillProgram`

The current player-work runtime flow for that slice is:
- `electrical` and `programming` commands target live item components through the runtime-configurable interfaces above
- `programming` also targets the standalone character-owned workspace when the first token is a reserved workspace verb
- `programming terminal connect <terminal>` creates a terminal session on a powered connected `ComputerTerminal`
- `programming terminal owner host` or `programming terminal owner <storage>` selects which real computer owner the workspace-style `programming` verbs mutate
- when a terminal session is active, workspace-style `programming` verbs operate on that selected real computer owner instead of the private workspace
- `programming apps` lists the built-in applications exposed by the connected powered host, regardless of whether the current selected mutable owner is the host or one mounted storage device
- `programming app <name>` executes the named built-in application on that connected powered host as a real host process
- `programming item <item> file [<component>]`, `file edit`, `file write`, and `file public on|off` now provide the live local editing surface for `FileSignalGenerator` components on ordinary loaded items
- administrator characters can now configure shared host network identity and hosted VPN routes through the active terminal session with `programming network`, `programming network domain ...`, `programming network account ...`, and `programming network vpn ...`
- administrator characters can now configure host mail service state through the active terminal session with `programming mail`, `programming mail service ...`, `programming mail domain ...`, and `programming mail account ...`
- administrator characters can now configure host board-service exposure through the active terminal session with `programming boards`, `programming boards service ...`, and `programming boards add|remove ...`
- `FileManager` is now a shipped built-in application on that surface: it runs as a host process, keeps its current host-or-storage file target in persisted process state, and uses repeated `type <text>` input to drive `list`, `show`, `edit`, `write`, `append`, `delete`, `copy`, `owners`, `use`, `help`, and `exit`
- `FileManager` owner enumeration is now file-owner-aware rather than storage-only, so host-local component owners such as `FileSignalGenerator` appear alongside the host and mounted storage devices as selectable file targets
- `type edit <file>` hands off to the engine's ordinary multiline editor flow, recalls the current file contents, saves on `@`, and leaves the file unchanged on `*cancel`
- `Directory` is now also a shipped built-in application on that surface: it runs as a host process and uses repeated `type <text>` input to browse the local host summary plus its built-in services, mounted storage devices, connected terminals, and local network adapters, and it also supports telecom-backed discovery of reachable hosts through `hosts`, `show <host>`, and `services <host>`, plus route inspection and authenticated VPN tunnelling through `routes`, `gateways`, `tunnel connect ...`, and `tunnel disconnect ...`
- `Directory` and `SysMon` now present network adapters in terms of canonical address, stable device id, base access-route summary, and active session tunnels rather than as a flat all-devices-visible network, so players and administrators can see why a host is reachable without being overwhelmed by unrelated infrastructure
- `Mail` is now also a shipped built-in application on that surface: it runs as a host process, authenticates against reachable shared `user@domain` identities, and uses repeated `type <text>` input to log in, list inbox state, read and delete mailbox entries, compose drafts, and post messages
- `Boards` is now also a shipped built-in application on that surface: it runs as a host process, opens a reachable board-service host, authenticates against reachable shared `user@domain` identities, lists the boards exposed by that host, and uses the normal editor flow to compose posts
- `type` is now the terminal-facing input verb: it submits text to the current terminal session, auto-resolves and auto-connects to a nearby terminal when one can be identified cleanly, and resumes the single foreground program on that session if it is suspended in `UserInput()`
- `programming terminal status` now also surfaces any active session-scoped tunnel routes on the connected terminal session
- computer processes now persist terminal-wait metadata for `UserInput()` waits, including the waiting character and terminal item identity, so those waits can survive save/load and still route correctly from `type`
- host-backed computer processes can now also suspend in `WaitSignal()` with persisted signal-wait metadata that records the awaited local signal binding on the real execution host item
- the current v1 `WaitSignal()` implementation resolves only named signal source components on that real execution host item and resumes when that source emits a non-zero signal value
- `electrical` also handles the physical install/remove and cable routing workflow for separate automation items
- public-file publication and authenticated FTP mutation likewise operate on any reachable `IComputerFileOwner`, so a `FileSignalGenerator` on a host item can expose or accept edits through the same network file infrastructure as the host and its mounted storage
- reachable remote file and service discovery now respects the same route-key accessibility model as `Directory`, so exchange-private and VPN-scoped devices are not unintentionally exposed to unrelated hosts on the broader linked-grid cluster
- real host-backed or storage-backed program execution is now blocked when the execution host is not powered
- built-in application execution is likewise blocked when that connected execution host is not powered
- actions are modelled as targeted delayed effects rather than instant mutation
- required tools are acquired and restored through inventory plans, so failure costs time but does not permanently consume tools or materials
- success, progress, cancel, failure, and shock output are driven by configurable static strings rather than hard-coded prose
- electrical work uses dedicated install/configure checks, and abject electrical failures can apply electrical damage
- administrator characters bypass the live item tool/check/delay layer for `electrical` and item-targeted `programming`, but still go through the same targeting and service-access validation
- service housings on doors and automation hosts are addressed through normal `open` / `close` subtargets such as `open north panel`
- live controller and signal diagnostics are part of `electrical <item>` runtime inspection rather than ordinary item descriptions

### Telecommunications and cellular pattern
Telecommunications items are a useful example of how multiple item capabilities compose into one subsystem:
- wired handsets implement `ITelephone`, but the active phone number may belong to a separate `ITelephoneNumberOwner` endpoint such as a telecommunications outlet
- fax machines layer `IFaxMachine` on top of the same telecom-numbering model, but they also own machine runtime state such as paper storage, ink availability, and pending inbound fax memory
- telecommunications grids persist number ownership against the specific endpoint component identity, not just the parent item, so multiple telecom endpoints on one item keep distinct numbers across save/load
- a telecommunications grid is also a specialised power network, so telecom-connected devices can draw power from producers on that grid without exposing themselves as ordinary electrical-service endpoints
- each telecommunications grid owns exchange-level behaviour such as prefix, subscriber length, maximum rings before timeout, and direct exchange links used for long-distance routing
- the same telecommunications grid runtime now also serves as the transport substrate for computer networking: attached `NetworkAdapter` components join the grid as runtime-derived endpoints, publish canonical addresses, and can discover other reachable network-ready adapters across the linked-grid graph
- computer networking now also has an explicit access-scope model layered on top of that transport. Each reachable endpoint publishes one or more route keys, and host discovery only succeeds when the source and target share at least one route key.
- computer-network reachability is transitive across linked exchanges in this slice, unlike voice-call routing; discovery walks the linked-grid graph breadth-first with cycle protection
- canonical network addresses prefer a configured adapter address when it is unique within the reachable linked-grid cluster, and otherwise fall back to a stable generated address of the form `adapter-<itemid>`
- every network-facing device also now has a stable globally unique device identifier of the form `device-<itemid>`, which is the current player-facing equivalent of a hardware-address or IP-like identifier for diagnostics and future tooling
- current access-scope route keys are:
  - `public` for devices exposed to the broader telecom-backed network
  - exchange-private subnet keys scoped to a specific telecommunications grid and subnet name
  - explicit `vpn:<name>` memberships
- exchange-private subnet keys are the current reference model for isolated field networks at an exchange. Devices on the same exchange-local subnet can all see each other even when they are intentionally invisible from the broader public network.
- `NetworkSwitch` is the infrastructure reference pattern for daisy-chained network topology. It can take one upstream telecom path and fan that out to many downstream adapters or further switches without each endpoint needing a direct grid attachment.
- `WirelessModem` is the infrastructure reference pattern for untethered IoT or field devices. It joins the same telecom-backed network layer through powered cellular coverage and exposes the same public, subnet, and VPN memberships as a wired adapter.
- hosts can now also expose one or more hosted VPN network ids, which are advertised as a lightweight VPN gateway service on reachable hosts and can be granted to a terminal session after authenticating with a hosted `user@domain` identity on that gateway host
- active tunnel memberships are session-scoped rather than hardware-scoped: they extend route visibility only for the authenticated terminal session and do not mutate adapter or host wiring
- future hacking should layer on top of this route-key model by granting or emulating additional temporary route membership rather than replacing the underlying discovery rules
- only implemented built-in applications marked as network services are advertised through the computer discovery layer; `Mail`, `FTP`, and `Boards` are now shipped ones, while `Messenger` stays hidden until it actually ships
- `Mail` advertisement is host-service-aware rather than adapter-owned: a host only advertises `Mail` when that host has the service enabled and at least one enabled hosted domain
- `Boards` advertisement is likewise host-service-aware rather than adapter-owned: a host only advertises `Boards` when that host has the service enabled and at least one hosted board exposed
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
- `FinaliseLoad()` restores structural scaffolding only: references, pending ids, mount relationships, routed cable metadata, and similar non-live state
- after structural restoration completes, the world login pass logs in world-root items that are actually active in the world, while inventory-rooted item trees remain dormant until their owning body or character logs in
- `Login()` is the point where live runtime behaviour begins: power drawdown, heartbeats, signal subscriptions, timers, retries, ringing, and comparable active behaviour
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

Read-only generated remains components such as `Corpse` and `Bodypart` now also pin themselves to the source body that created them.
- they still remember the originating character for identity-facing workflows such as resurrection, morgue ownership, and FutureProg lookups
- they also persist the specific source `Body` id so anatomy, inventory, carried implants, wound routing, and surgery compatibility continue to reflect the form that actually died or was severed
- later character form switches must not cause old corpses or severed parts to silently change shape, wearability, butchery yields, or transplant eligibility

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

`StableTicketGameItemComponentProto` is a stricter singleton example:
- it is auto-created by `InitialiseItemType(IFuturemud)` when missing
- it registers only the database loader, not a builder loader
- it is read-only and prevents manual loading
- its generated item prototype is read-only
- live ticket components persist the stable stay id, the ticket item id, and a random token so copied or stale tickets are invalid without needing to delete the physical item

When documenting or extending the system, treat these as framework-level special cases rather than normal builder-authored component content.

## Practical Guidance
- Reach for interfaces in `FutureMUDLibrary` first. Runtime systems should usually depend on `IContainer`, `IReadable`, `ITransmit`, and similar interfaces.
- Treat `GameItem` as the composition shell, not the place to add every feature directly.
- Put configuration on the component proto and runtime state on the component.
- Expect update, morph, and destruction flows to matter for any non-trivial item behaviour.

## Runtime Integration: Thermal Sources
Thermal-source components now integrate with room temperature through `IProduceHeat`.

`IProduceHeat` exposes:
- `CurrentAmbientHeat` for signed room-wide ambient contribution
- `CurrentHeat(Proximity proximity)` for signed target-specific proximity contribution

`Cell.CurrentTemperature(...)` now layers thermal-source output on top of the pre-existing weather and environmental-effect calculation:
- sheltered indoor cells (`Indoors`, `IndoorsWithWindows`, `IndoorsNoLight`) apply 100% of ambient thermal output
- `IndoorsClimateExposed` applies 50% of ambient thermal output
- outdoor cells apply no ambient thermal output
- target-specific proximity output always applies, regardless of room type

The runtime aggregation walks:
- room items, including deep contained items
- characters' external items, including deep contained items on those externals

This means a carried brazier, a lit crafted fire, or a connected heater can all participate in temperature calculation without needing bespoke cell-side registrations.

The current thermal-source component families use a shared authored thermal profile but different activation models:
- electric sources are active only when switched on and currently powered through `IConsumePower`
- fuel-fed sources are active only when switched on and still connected to a valid fuel source of the authored medium and fuel type
- consumable sources auto-start and remain active until their burn timer expires
- solid-fuel sources stay active while switched on and while they still have queued valid fuel items to burn
