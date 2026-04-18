# FutureMUD Item System Component Authoring

## Scope
This document explains how to add a new item capability through the component system.

It is the primary implementation guide for creating new item "types" in FutureMUD.

## The Real Workflow
Adding a new item capability usually means all of the following:
1. Decide whether there should be a public interface in `FutureMUDLibrary`.
2. Create or update the component prototype class in `MudSharpCore/GameItems/Prototypes`.
3. Create or update the runtime component class in `MudSharpCore/GameItems/Components`.
4. Register the component type with `GameItemComponentManager`.
5. Add builder help and any custom component editing commands.
6. Attach the component prototype to one or more item prototypes with builder commands.
7. Load and test the resulting items in-game.

The template in `Item Templates/GameItem` helps with steps 2 through 4, but it does not solve the design work, interface design, or gameplay integration for you.

## Step 1: Decide on the Public Contract
### Reuse an existing interface when possible
Many item systems already query for public interfaces in `FutureMUDLibrary/GameItems/Interfaces`.

If your new item capability is just another implementation of an existing concept, reuse the existing interface. Examples:
- a new kind of container should still satisfy `IContainer`
- a new kind of light source should satisfy `IProduceLight`
- a new kind of weapon should satisfy the relevant weapon interfaces
- a new power, liquid, or telecom item that physically plugs into something should implement `IConnectable` and usually also a matching `ICanConnectTo...Grid` interface

### Add a new interface when the capability is new
If the capability is genuinely new and other systems need to query for it, add a new interface in `FutureMUDLibrary` first.

That interface should:
- describe the behaviour other systems care about
- avoid binding callers to a concrete component class
- stay focused on the game-facing contract, not internal XML or builder details

The runtime component in `MudSharpCore` should then implement that interface.

If the feature also needs shared immutable value objects rather than only a query surface, place those models in `FutureMUDLibrary` too. The recorded-audio implementation is the reference pattern:
- immutable playback data lives in `MudSharp.Form.Audio`
- gameplay-facing item queries live behind interfaces such as `IAudioStorageTape` and `IAnsweringMachine`
- XML helpers live with the shared models so stage-1 persistence can stay inside normal item/component XML without feature-specific database tables

Computer-program and signal-automation work should follow the same rule:
- shared contracts such as `IComputerHost`, `IComputerFileSystem`, `IComputerExecutable`, `ISignalSource`, and `ISignalSink` belong in `FutureMUDLibrary/Computers`
- item-facing component contracts such as `ISignalSourceComponent`, `ISignalSinkComponent`, and `IMicrocontroller` belong in `FutureMUDLibrary/GameItems/Interfaces`
- concrete behaviours such as `Microcontroller`, `PushButton`, `MotionSensor`, `ElectronicDoor`, or `ComputerTerminal` should still be separate runtime components and component protos in `MudSharpCore`
- avoid collapsing multiple distinct automation behaviours into one generic "sensor" or "actuator" component unless the gameplay contract is genuinely identical

## Step 2: Start from the GameItem Template
The repaired `Item Templates/GameItem` template should now generate a coherent pair:
- `ExampleGameItemComponent`
- `ExampleGameItemComponentProto`

Use it as the first scaffold, then immediately adapt it to the real feature.

What the template gives you:
- a correctly named component class
- a correctly named component proto class
- matching constructors and `Copy(...)`
- registration hooks for builder and database loading
- XML load/save hooks
- default builder help and `ComponentDescriptionOLC(...)`

What the template does not decide for you:
- whether a public interface is required
- what runtime state belongs on the component
- what builder-editable configuration belongs on the proto
- whether the component decorates descriptions
- whether it affects weight, movement, position, destruction, morphing, or purge warnings
- whether it should be primary, read-only, or manually loadable

## Step 3: Build the Component Proto
### Responsibilities of the proto
The component proto is the builder-authored and revisioned definition.

Put the following here:
- editable configuration values
- XML serialisation and deserialisation
- builder commands for changing the configuration
- help text that appears through `comp typehelp` and `comp set help`
- `CreateNew(...)` and `LoadComponent(...)`
- registration through `RegisterComponentInitialiser(...)`

### Registration requirements
Every normal component proto must register itself with `GameItemComponentManager` using a static `RegisterComponentInitialiser(...)`.

Typical registration includes:
- `AddBuilderLoader(...)` for `comp edit new <type>`
- `AddDatabaseLoader(...)` for boot-time loading from the stored type string
- `AddTypeHelpInfo(...)` for type listings and builder help

For automation and signal-capable types, use the type-summary text intentionally:
- signal emitters should usually advertise a coloured `[signal generator]` tag
- signal-driven sinks should usually advertise a coloured `[signal consumer]` tag
- components that both accept and emit signals, such as relays or microcontrollers, should usually advertise both

Use the builder loader names intentionally:
- one primary name should be the main builder-facing type keyword
- optional aliases can be added as non-primary names

### Example: Container proto
`ContainerGameItemComponentProto` is a good reference because it shows the common editable-proto pattern:
- several builder-editable properties
- XML persistence for those properties
- targeted building commands
- readable `ComponentDescriptionOLC(...)`
- standard registration and factory methods

## Step 4: Build the Runtime Component
### Responsibilities of the component
The runtime component should hold live behaviour and live state.

Typical responsibilities include:
- implementing the public gameplay interface
- storing runtime state that changes per live item
- reacting to morph, destruction, or swap flows
- contributing to description decoration
- exposing movement or reposition restrictions
- managing contained, attached, or connected sub-items

### Common overrides to consider
Not every component needs these, but authors should deliberately consider them:
- `Copy(...)`
- `UpdateComponentNewPrototype(...)`
- `SaveToXml()`
- `LoadFromXml(...)`
- `DescriptionDecorator(...)`
- `Decorate(...)`
- `Delete()`
- `Quit()`
- `Login()`
- `Take(...)`
- `SwapInPlace(...)`
- `HandleDieOrMorph(...)`
- `PreventsMovement()`
- `PreventsRepositioning()`
- `ComponentWeight`
- `ComponentWeightMultiplier`
- `ComponentBuoyancy(...)`
- `WarnBeforePurge`

### Runtime state versus proto state
Use this rule consistently:
- if the value is authored once and reused by many items, it belongs on the proto
- if the value changes per live item, it belongs on the runtime component

For example:
- container capacity belongs on the proto
- current contents belong on the runtime component
- connector lists, wattage settings, flow rates, and telecom numbering preferences belong on the proto
- live grid membership, connected peers, battery charge, and flowing liquid state belong on the runtime component
- pending inbound fax jobs, printed-page buffers, and current ink or paper state belong on the runtime component even when the machine's telecom and print capabilities are defined by the proto
- exchange-level telecommunications defaults such as prefix, subscriber digits, and maximum ring count belong to the grid or grid-creator side, not to individual handsets
- default audible behaviour such as telephone and cellular ring volume belongs on the prototype, while the live component owns the current player-selected ring setting that can diverge at runtime

### Connectable item patterns
When a component can join a physical network, treat the connector list as part of the prototype definition and the actual links as runtime state.

That pattern is now used by items such as:
- telephones and telecommunications feeders
- liquid pumps and liquid-grid connectors
- battery-backed devices that can charge from a mains source
- electric feeders and power sockets

The important implementation detail is that the proto should expose the builder-editable connector shapes, while the component should restore connected items through `FinaliseLoad()` and react to `Connect` / `Disconnect` events.

### Pattern: telecom devices versus telecom endpoints
The telephone and cellular implementation is a good reference when a subsystem has to separate "the thing a player uses" from "the thing the network addresses":
- `ITelephone` models the live handset behaviour such as dialling, ringing, pickup, answer, hangup, and speech relay
- `IFaxMachine` is the fax-specific companion pattern when a telecom item also scans readable documents, queues inbound traffic, and prints onto physical paper
- `ITelephoneNumberOwner` models the addressed endpoint that owns the number on the telecommunications grid
- exchange-hosted voicemail is a split responsibility: the grid owns the hosted service configuration and central mailbox storage, while each `ITelephoneNumberOwner` owns the per-line opt-in flag
- when persisting telecom state, save and restore the specific endpoint component identity rather than only the parent item identity; this matters when one item hosts multiple telecom endpoints
- a wired handset may delegate numbering to a connected outlet, so moving the handset between outlets can change its number without changing the handset component itself
- a cellular handset usually implements both roles itself because the number stays with the device rather than a wall outlet
- an implant telephone also usually implements both roles itself, but it should combine the telecom interfaces with implant-facing ones such as `IImplantReportStatus` and `IImplantRespondToCommands` so the neural command surface stays separate from handheld manipulation
- a fax machine usually combines telecom endpoint behaviour with machine-style runtime state such as paper storage, ink usage, and an in-memory queue for inbound faxes that arrived before the printer could physically output them
- if the item participates in telecom wiring or telecom-grid power, also consider `ICanConnectToTelecommunicationsGrid`, `IConnectable`, `IConsumePower`, and `IProducePower`
- an answering machine is the reference for a chained endpoint: it can expose both itself and downstream handsets through `ConnectedTelephones`, while still either owning its own number or delegating numbering to an upstream outlet
- the answering machine also shows how to keep a reusable medium generic: the tape is just an `IAudioStorageTape`, while the machine owns the telecom-specific greeting, message, and `ISelectable` control surface
- the telecommunications grid creator proto is the reference for exchange-authored defaults: prefix, subscriber digits, maximum rings, hosted-voicemail enablement, and the reserved service access number all belong there and flow into the runtime grid instance
- for this kind of split design, keep authored defaults such as wattage, connector shapes, ring volume, ring emote, premote, and default answer-after-rings on the prototype, and keep inserted media, saved recordings, live call state, and armed recording state on the runtime component

When authoring similar systems, decide early whether identity belongs to the device, the connection point, or both.

For creator-style components that own a grid instance, author them so they can recreate that owned grid from prototype settings and current location context during load. That recovery path is part of the runtime contract, not an optional migration step.

## Step 5: Attach the Capability to Item Prototypes
Once the component proto exists and is current, attach it to item prototypes through builder workflows:
- create or edit the component with `comp`
- create or edit an item prototype with `item`
- attach the component to the item with `item set add <id|name>`

The item prototype then becomes the reusable content definition that creates live items carrying the runtime component.

## Step 6: Test with Builder Workflows
At minimum, validate:
- the component can be created with `comp edit new <type>`
- the component can be edited and submitted
- the item prototype can attach and detach the component
- the item can be loaded with `item load`
- the loaded live item actually exposes the expected interface and behaviour

If the component sets `PreventManualLoad`, also validate the expected restrictions around builder loading and prog loading.

## Special Cases
### Read-only framework components
Some components are framework primitives rather than normal builder-authored components.

`HoldableGameItemComponentProto` is the main example:
- read-only
- auto-initialised
- not opened for ordinary editing

Do not use these as the model for a normal component authoring flow.

### When to add custom initialisation
If a component type must always exist or must be created programmatically for engine reasons, provide a static initialiser such as `InitialiseItemType(...)` in addition to normal registration.

Use this sparingly. Most new item capabilities should be ordinary component types authored through `comp`.

## Example Authoring Checklist
Use this checklist when adding a new capability:

1. Add or reuse a public interface in `FutureMUDLibrary`.
2. Generate a starting scaffold from `Item Templates/GameItem`.
3. Rename and align the generated pair with the real gameplay concept.
4. Move reusable definition to the proto and live state to the component.
5. Implement XML save/load on both sides where required.
6. Implement registration through `RegisterComponentInitialiser(...)`.
7. Add builder help and focused building commands.
8. Implement any necessary runtime interfaces and behaviour.
9. Attach the component to an item prototype and load a test item.
10. Validate update, morph, and destruction implications if the feature is stateful.

## Common Mistakes
- Putting gameplay queries against the concrete component class instead of a public interface.
- Storing authored definition on the runtime component instead of the proto.
- Forgetting to register the database loader, which breaks boot-time loading.
- Forgetting to update `ComponentDescriptionOLC(...)` and type help, leaving builders without guidance.
- Ignoring copy or morph behaviour for stateful components.
- Assuming the template is enough on its own. It is only the skeleton.

## Recommended Reference Implementations
- `ContainerGameItemComponentProto` and `ContainerGameItemComponent`
- `WearableGameItemComponentProto` and `WearableGameItemComponent`
- `HoldableGameItemComponentProto` and `HoldableGameItemComponent` for the read-only special-case pattern

## Signal Automation Authoring
The current computer-automation slice is a good reference for "shared contracts, separate concrete behaviours".

Implemented builder-facing component types:
- `pushbutton`
- `toggleswitch`
- `motionsensor`
- `lightsensor`
- `rainsensor`
- `temperaturesensor`
- `timersensor`
- `keypad`
- `microcontroller`
- `automationmounthost`
- `automationhousing`
- `signalcable`
- `signallight`
- `electronicdoor`
- `electroniclock`
- `relayswitch`
- `alarmsiren`

Current authoring pattern:
- sources author their own output behaviour and expose `ISignalSourceComponent`
- sinks author a `source <componentname>` field and resolve that source from sibling components on the same item
- microcontrollers author a list of `input add <variable> <sourcecomponent>` bindings and inline `logic`; the binding command accepts a component prototype name or id and stores a stable local source identifier plus the current default local endpoint key
- automation hosts author one or more named bays plus an optional sibling `automationhousing` component prototype that must be open for service access
- automation housings author which categories of automation items they may conceal and are themselves the dedicated lockable-container service-access capability on the item
- signal cables have no meaningful static routing fields on the proto; they are routed at runtime and persist that live route on the component instance
- `motionsensor` authors powered-machine settings plus signal value, duration, minimum size, and movement mode (`any`, `begin`, `enter`, `stop`)
- `lightsensor` authors powered-machine settings and emits current ambient illumination as a live numeric signal
- `rainsensor` authors powered-machine settings and emits a live numeric rain-intensity signal based on the current weather when climate-exposed
- `temperaturesensor` authors powered-machine settings and emits the current ambient temperature as a live numeric signal in Celsius
- `timersensor` authors powered-machine settings plus active and inactive values, active and inactive durations, and its initial phase
- `keypad` authors powered-machine settings plus numeric code, emitted signal value, signal duration, and keypad entry emote
- `microcontroller` authors powered-machine settings, including optional mount-host power draw via `mountpower`
- `electronicdoor` authors source component prototype, threshold, invert mode, and automatic open and close emotes
- `relayswitch` authors source component prototype, threshold, invert mode, and programmable power-supply behaviour through the relay-controlled power base
- `alarmsiren` authors source component prototype, threshold, invert mode, volume, and repeated alarm emote

Important implementation details from this slice:
- microcontroller inline logic is compiled immediately as a `ComputerFunction` and must return a number
- input variable names are validated and normalised to lower case at compile time
- sinks and microcontrollers detach and reconnect to sibling signal sources during load and teardown using stable local source identifiers plus endpoint keys rather than transient component names
- signal propagation is event-based, so components should avoid re-emitting unchanged values
- live player reconfiguration is a separate runtime concern from builder authoring:
  - configurable sinks that support live rewiring should implement `IRuntimeConfigurableSignalSinkComponent`
  - live-programmable controllers should implement `IRuntimeProgrammableMicrocontroller`
  - those runtime interfaces are what the `electrical` and `programming` command verbs target on loaded items
  - standalone character-owned workspace executables are not item components and are instead managed through the computer-runtime services in `MudSharpCore/Computers`

This is the reference approach for the early phases of computerised items:
1. put the shared signal contract on interfaces first
2. keep authored thresholds, keywords, and sibling-source names on the proto
3. keep live signal state and subscriptions on the runtime component
4. treat broader cross-item graphs as a later concern, but follow the current reference patterns for automation host bays, automation housings, and one-hop cable items when the task explicitly implements them

## Thermal Source Components
The thermal-source family is the reference for "same gameplay concept, multiple activation models".

Shared authoring shape:
- all thermal sources implement `IProduceHeat`
- all thermal source protos author signed output for `ambient`, `intimate`, `immediate`, `proximate`, `distant`, and `verydistant`
- all steady-state variants also author switch-on and switch-off emotes plus active and inactive descriptive addenda

Current builder-facing thermal component types:
- `ElectricHeaterCooler`
  - authors wattage plus the shared thermal profile
  - runtime activation depends on being switched on and receiving power
- `FuelHeaterCooler`
  - authors a single fuel medium (`liquid` or `gas`), a specific fuel type, burn rate, connector shape, and the shared thermal profile
  - runtime activation depends on being switched on, connected, and still supplied with matching fuel
- `ConsumableHeaterCooler`
  - authors burn duration, optional spent-item replacement, spent emote, and the shared thermal profile
  - runtime state is just remaining burn time
- `SolidFuelHeaterCooler`
  - authors valid fuel tag, maximum loaded fuel weight, seconds-per-unit-weight, and the shared thermal profile
  - runtime state includes contained fuel items, the currently burning fuel item, and remaining burn time for that item

When adding similar capabilities in future:
1. Put the shared gameplay contract on a public interface first.
2. Keep the authored cross-family profile on a shared proto base.
3. Keep activation-specific state and persistence on the runtime component.
4. Explicitly decide how morph, destruction, load-time finalisation, and deep-copy flows should treat inserted media or fuel.
