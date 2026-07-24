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

Zero-gravity components are another reference for small gameplay contracts:
- `ZeroGravityAnchor` implements `IZeroGravityAnchorItem` and needs no runtime XML state
- `ZeroGravityTether` implements `IZeroGravityTetherItem` and stores only its builder-authored maximum room length
- `RcsThruster` implements both `IZeroGravityPropulsion` and `IConnectable`, consuming gas from a connected `IGasSupply`

When adding similar movement equipment, keep the public movement contract in `FutureMUDLibrary/GameItems/Interfaces` and keep connector/gas details inside the concrete component pair.

Surface-water vehicle propulsion follows that same interface-first rule. `Vehicle Oar` implements `IVehicleOar` and exposes only a positive efficiency multiplier; the vehicle service discovers it while the item is held or wielded and combines that value with clamped parent-item condition. `Outboard Motor` implements `IOutboardMotor` and exposes a positive output multiplier plus either fuelled or electric energy configuration. It is intentionally compositional rather than a complete engine item by itself:

- every motor parent item must also have `Vehicle Installable` so it can occupy a vehicle installation point;
- a fuelled motor needs `fuel <liquid> <volume>` and a same-item `ILiquidContainer`;
- an electric motor needs `power <watts>` and a same-item `IProducePower`, normally `BatteryPowered` or a battery-fed `PowerSupply`;
- an optional same-item `IOnOff` participates automatically and must be switched on;
- `output <multiplier>` and oar `efficiency <multiplier>` must remain positive.

These component prototypes store their builder values in compatible XML and have no mutable live XML state. Submission rejects non-positive oar/output multipliers, a fuelled motor without a liquid and positive per-move volume, or an electric motor without a positive power spike. Propulsion selection, contributor outcomes, installed-motor identity, and resource charging belong to the vehicle domain rather than component XML. Use `comp typehelp VehicleOar` and `comp typehelp OutboardMotor` for the exact builder surface.

Computer-program and signal-automation work should follow the same rule:
- shared contracts such as `IComputerHost`, `IComputerFileSystem`, `IComputerExecutable`, `ISignalSource`, and `ISignalSink` belong in `FutureMUDLibrary/Computers`
- broader mutable file-owner contracts such as `IComputerFileOwner` belong in `FutureMUDLibrary/Computers` when item components need to expose files without also exposing executable storage
- item-facing component contracts such as `ISignalSourceComponent`, `ISignalSinkComponent`, and `IMicrocontroller` belong in `FutureMUDLibrary/GameItems/Interfaces`
- concrete behaviours such as `ComputerHost`, `ComputerStorage`, `ComputerTerminal`, `NetworkAdapter`, `NetworkSwitch`, `WirelessModem`, `Microcontroller`, `PushButton`, `MotionSensor`, or `ElectronicDoor` should still be separate runtime components and component protos in `MudSharpCore`
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
- capability marker interfaces that mirror the runtime component interfaces

### Prototype capability markers
When a runtime component implements a public `IGameItemComponent` capability interface, the matching component proto should implement the corresponding marker in `FutureMUDLibrary/GameItems/Prototypes`.

For example:
- a runtime component that implements `IContainer` should have a proto that implements `IContainerPrototype`
- a runtime component that implements `IWearable` should have a proto that implements `IWearablePrototype`
- a runtime component that implements `ILiquidContainer` should have a proto that implements `ILiquidContainerPrototype`, which also implies the relevant parent capability markers
- inherited public component capabilities need markers too; for example `ICorpsePrototype` also advertises `IBodyRemainsPrototype` and `IButcherablePrototype`

These markers let item prototypes catch invalid component combinations before review. Most capability markers are exclusive, because runtime code usually calls `GetItemType<T>()` or `IsItemType<T>()` and expects one authoritative component. Aggregate service markers such as `IConnectablePrototype`, `IConsumePowerPrototype`, `IProducePowerPrototype`, `ISignalSourceComponentPrototype`, `IChangeTraitsInInventoryPrototype`, and grid-connection markers remain composable.

If you add a new public `IGameItemComponent` interface, add its matching `I...Prototype` marker at the same time and classify it as exclusive unless the runtime deliberately aggregates multiple sibling components of that interface.

### Opt-in condition maintenance
Use `IConditionDegradingComponent` when a component should optionally consume `IGameItem.Condition` as it is used. The interface extends `IAffectQuality`, and the matching `IConditionDegradingComponentPrototype` marker is aggregate, so a component can contribute a maintenance quality penalty without becoming the item's only quality-affecting component.

Supported component protos expose the shared builder commands:
- `condition on|off`
- `condition use <formula>`
- `condition quality <formula>`
- `condition defaults`

Legacy XML and fresh component protos must load disabled by default. Do not make old weapons, armour, shields, measuring instruments, or firearms begin degrading merely because the code now supports it; builders opt specific component revisions in.

Use the shared `ConditionMaintenanceProfile` helper for XML load/save, builder parsing, formula validation, and clamping. Its formula variables are `condition`, `rawquality`, `basequality`, `usekind`, `outcome`, `degree`, `damage`, `absorbed`, and `passed`. The default quality formula applies no penalty above 20 percent condition and becomes progressively harsher only at very low condition. Hybrid combat profiles should branch their stock use-loss formula on `usekind`, so ranged firing and shield blocks can use the higher default while melee attacks, parries, and warding weapon uses keep the lower melee default.

Place use hooks after the use attempt has resolved so the current action uses the pre-use quality. Combat components should map melee attacks, parries, warding attacks, shield blocks, ranged shots, and armour absorption to the matching `ItemConditionUseKind`. Non-combat components should hook meaningful operation attempts such as a completed measurement or a breathing-filter gas consumption tick.

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

### Ranged weapon components
Ranged weapon component families should reuse `IRangedWeapon` and `IRangedWeaponPrototype` unless they are introducing a genuinely new combat contract. The stock weapon components are authored as ordinary component/proto pairs and then bound to a `RangedWeaponType` definition that supplies the combat skill, range, load model, accuracy formula, damage formula, stamina costs, and timing.

Current builder-facing ranged component loaders include `bow`, `crossbow`, `firearm`, `sling`, and `blowgun`. `sling` and `blowgun` both store only their selected ranged weapon type in component XML, with `sling` also storing a readied stamina drain tick value. Their live components load, ready, unready, and fire through the existing ranged-weapon command surface. Slings use the shared `ReadiedRangedWeaponDrainStamina` path while readied, as bows do; the component decides whether readied use requires a free hand.

Do not add `GetDamage` or another damage-expression path to sling or blowgun components. Their `Fire` methods consume the loaded ammunition and delegate to `IAmmo.Fire`; `AmmunitionGameItemComponent` constructs the actual damage from the selected ammunition and ranged-weapon type. Load failures must return a player-facing feasibility reason rather than throw, including for an unexpected or newly-added inventory-plan state.

Do not create bespoke ammunition components for ordinary projectile variants. Sling bullets and blowgun darts use the generic `Ammunition` component and rely on the `AmmunitionType` row's `RangedWeaponTypes` and `SpecificType` fields to match the weapon's `RangedWeaponType` and specific ammunition grade. Poisoned or drugged darts layer through the existing `WeaponPoisonCoating` and ammunition wound-delivery systems instead of changing the base ammunition contract.

### Writing and inscribing components

Use `PaperSheet` for paper-like loose sheets and scrolls, and `Book` for paged codex-style objects. Both expose the standard `IWriteable` and `IReadable` contracts.

Use `ScribingImplement` when an implement should behave like a configurable writing tool instead of one of the modern fixed components such as `Biro`, `Pencil`, or `Crayon`. The prototype stores `ImplementType`, `Colour` or `ColourCharacteristic`, and `TotalUses`. Set `TotalUses` to `0` for non-consuming stylus-style tools.

Use `InscribableSurface` for wax, clay, wood, ostraca, or other non-paper surfaces. The prototype stores `MaximumCharacterLengthOfText` and an `AllowedImplementTypes` list. Builders should configure the accepted implement types narrowly: wax and clay generally use `Stylus`, wooden blocks may use `Stylus`, `Chisel`, or `Charcoal`, and ostraca may use ink or charcoal implements.

`BlowgunGameItemComponent` also enforces the physical breathing requirement. It cannot be readied or fired by a body that does not breathe, cannot currently breathe, lacks a mouth bodypart, or has anything worn over the mouth. This covers breathing filters and apparatus without a separate item-type check because those components are supplied by mouth-worn items in the breathing system.

`IRangedWeapon.CanFireWhileHidden` is deliberately narrow. Leave the default false for normal ranged weapons. Only components that should preserve hiding through `fire -> Engage/JoinCombat`, such as `BlowgunGameItemComponent`, should return true and use obscured output for ready/load/fire emotes.

### Seal and measuring components

Use `SealStamp` for signets, cylinder seals, office stamps, or similar authority-bearing tools. The prototype stores the design text, issuer, owner/clan/office metadata, material text, forgery difficulty, and optional authority prog. The matching runtime component is stateless; it exposes the stamped metadata through `ISealStamp`.

Use `Sealable` as a separate attachable component rather than folding seal state into containers, books, scrolls, or writing surfaces. This lets the same tamper-evidence behaviour compose with `IContainer`, `IWriteable`, `IReadable`, openable items, and other ordinary item capabilities. The prototype stores allowed media, inspection difficulty, and broken-residue behaviour. The live component stores the active seal snapshot, sealing actor/time evidence where available, medium descriptor, broken state, and residue state.

Use `MeasuringInstrument` for physical measurement tools that should produce falsifiable reported quantities. The current implementation supports `Weight` and `FluidVolume` modes. Length, cubit, and surveying tools should remain prop-only until item dimensions exist. The prototype stores mode, precision, capacity, base drift per use, display unit text, and wrong-calibration limits. The live component stores stable drift direction, use count since calibration, calibration state, and any deliberate base-unit or percentage bias.

Measuring instruments can also opt in to shared condition maintenance. The stock loss formula is `0.0001` per measurement. Effective quality penalties from low condition feed the existing calibration-drift calculation, so a poorly maintained instrument becomes less reliable without a measuring-specific penalty path.

### Incense and offering components

Use `IncenseBurner` for room-scale aromatic burning rather than handheld smoking items. The prototype stores the required fuel tag, maximum contained fuel weight, seconds burned per unit of fuel weight, scent range, lingering multiplier, source and distant scent text, scent tracking difficulty, and optional inhaled drug pulse settings. The runtime component is its own transparent `IContainer` and an `ILightable`; builders load fuel with ordinary `put`, start it with `light`, and stop it with `extinguish`. Scent text appears through ambient description effects and can be discovered by smell checks in `tracks`.

Use `OfferingReceiver` for altars, votive basins, funeral trays, and similar ritual foci that receive item offerings. The prototype stores optional allowed and blocked offering tags, capacity, maximum item size, consumption mode (`ManualBurn`, `BurnOnOffer`, or `RecordOnly`), optional residue item prototype, `CanOfferProg`, `OnOfferProg`, `OnBurnProg`, and accepted/rejected/burn emotes. The runtime component is also its own transparent `IContainer`, so ordinary `put` and `take` still work, while `offer <item> at <focus>` and `burn <item> at <focus>` provide ritual-aware workflows.

`CanOfferProg`, `OnOfferProg`, and `OnBurnProg` all receive `(Character actor, Item focus, Item offering)`. `OfferingReceiver` also raises `OfferingReceived`, `OfferingReceivedWitness`, `OfferingBurned`, and `OfferingBurnedWitness`; witness events append the witnessing perceivable to the payload. V1 supports item and commodity offerings. Direct poured-liquid libations need a later liquid-specific command path rather than pretending that an item receiver can consume free liquid.

### Readable book components
Books remain one component family rather than splitting blank books and published books into separate component types. `BookGameItemComponentProto` owns reusable authored defaults, while `BookGameItemComponent` owns live page state, torn pages, current page, title, and the actual readable rows attached to each loaded item.

Builder-authored book defaults include:
- `title <text|clear>` for the default fresh-item title
- `content list` for direct readable references and collection references
- `content add <page> <language> <script> [provenance]` followed by editor text entry to create a new immutable printed writing
- `content copy <page> <writing id>` to reference an existing immutable writing
- `content drawing <page> <drawing id>` to reference an existing immutable drawing
- `content collection <collection> [append|page <number>]` to expand a writing collection into each fresh book
- `content edit <index>` to replace a direct writing reference with newly entered immutable printed text
- `content remove <index>` and `content clear`

Prototype contents are stored as page-scoped readable references, not mutable embedded text. Legacy embedded printed-content XML still loads for backwards compatibility, but it is converted into a `PrintedWriting` record and saved back as a readable reference. Freshly loaded books share immutable `Writing` and `Drawing` references from the prototype instead of cloning long books into duplicate rows. When a prototype references a writing collection, each new book expands the collection into ordinary page-level readable references; the live book does not remain linked to the collection as a virtual book, so later appends to one book page do not mutate the source collection or other books. Because the live book component serializes readable ids, book and paper readable components initialise after child readable rows have had a database hit; load paths also ignore missing readable ids so legacy corrupted XML cannot leave null readables in the live page list. The builder path validates page range and page capacity, and the runtime constructor repeats those checks defensively when loading prototype XML.

Writing collections are admin-authored virtual books of immutable readables. Use `writingcollection new`, `edit`, `add writing`, `add drawing`, `move`, `remove`, `clear`, `import markdown|json`, and `apply <collection> <book> [append|page <number>]` to keep related writings together, upload long texts, and apply them wholesale to live books. Prototype `content collection` should be preferred when a reusable book definition needs a whole uploaded packet.

Printed book content uses the `PrintedWriting` writing type with `WritingImplementType.Printed`. Printed writing has no character author; use its provenance text for publisher, source, anonymous, or generated-document attribution. V1 printed books remain writable: player handwriting can still be added to any non-torn page if the printed content leaves enough page capacity, and that action adds another immutable readable reference rather than mutating an existing row.

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

### Singleton system item components
Some components exist only so a runtime subsystem can generate trusted items. These should be stricter than ordinary read-only components.

`StableTicketGameItemComponentProto` is the current reference pattern:
- it registers a database loader, but no builder loader
- it is read-only and sets `PreventManualLoad`
- `InitialiseItemType(IFuturemud)` auto-creates the singleton component prototype and item prototype if they are missing
- the generated item prototype is also read-only, so builders cannot create or revise ticket definitions by hand
- live tickets are created through `CreateNewStableTicket(IStableStay)`, not through `item load` or component editing

Use this pattern for system-generated authority tokens where copied or hand-authored items would be a security problem. Runtime validation should bind the item to persistent system state, as stable tickets do with stay id, ticket item id, and a random token.

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
- `RidingGearGameItemComponentProto` and `HitchGearGameItemComponentProto` for semantic capability components where ordinary wearable or holdable items gain domain roles without becoming special item classes

## Riding And Hitch Gear Authoring
Riding and hitch gear are ordinary item components layered onto normal items:

- `RidingGear` declares one or more tack roles: saddle, saddle pad, bridle, reins, bit, stirrups, pack saddle, harness, and bitless control.
- `RidingGear` can add signed control and stability modifiers. Missing tack applies default penalties at runtime, so builders can still author bareback or rough riding scenes.
- `HitchGear` declares connector roles: tow bar, yoke, harness, lead rope, rope, chain, and traces.
- `HitchGear` also exposes the legacy drag-aid multiplier and maximum users because it implements `IDragAid` for compatibility.
- Non-direct vehicle tow point types use these roles to decide whether a physical connector item is valid. Direct/manual pull points such as `hand`, `manual`, `direct`, `none`, and `pull` remain item-free.

Seeded component prototypes include saddle, saddle pad, bridle, reins, bit, stirrups, pack saddle, bitless bridle, riding harness, lead rope, yoke, harness, rope, chain, traces, and tow bar templates.

## Signal Automation Authoring
The current computer-automation slice is a good reference for "shared contracts, separate concrete behaviours".

Implemented builder-facing component types:
- `computerhost`
- `computerterminal`
- `computerstorage`
- `networkadapter`
- `networkswitch`
- `wirelessmodem`
- `pushbutton`
- `toggleswitch`
- `motionsensor`
- `lightsensor`
- `rainsensor`
- `temperaturesensor`
- `timersensor`
- `keypad`
- `filesignalgenerator`
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
- host and file-storage components author executable and file-owner behaviour, while transport-facing components author connectivity, addressing, and access-scope rules
- `networkadapter` authors preferred address, whether it participates in the public network, an optional exchange-private subnet name, and zero or more VPN memberships
- `networkswitch` authors powered-machine settings plus a port count for downstream daisy-chain or endpoint connections; it does not own host executables or file state
- `wirelessmodem` authors the same addressing and access-scope surface as `networkadapter`, but its runtime transport comes from cellular coverage rather than a direct physical telecommunications-grid attachment
- sources author their own output behaviour and expose `ISignalSourceComponent`
- sinks author a `source <componentname>` field and resolve that source from sibling components on the same item
- microcontrollers author a list of `input add <variable> <sourcecomponent>` bindings and inline `logic`; the binding command accepts a component prototype name or id and stores a stable local source identifier plus the current default local endpoint key
- automation hosts author one or more named bays plus an optional sibling `automationhousing` component prototype that must be open for service access
- automation housings author which categories of automation items they may conceal and are themselves the dedicated lockable-container service-access capability on the item
- signal cables have no meaningful static routing fields on the proto; they are routed at runtime and persist that live route on the component instance
- hosted computer mail is not a separate item component family in the current shipped phase; it is runtime configuration on a `ComputerHost` exposed through `programming mail ...`, while domains, accounts, messages, and mailbox entries persist in dedicated database tables
- hosted computer FTP is also not a separate item component family in the current shipped phase; it is runtime configuration on a `ComputerHost` exposed through `programming ftp ...`, while FTP accounts and per-file public visibility flags persist with the owning host or storage runtime data
- `motionsensor` authors powered-machine settings plus signal value, duration, minimum size, and movement mode (`any`, `begin`, `enter`, `stop`)
- `lightsensor` authors powered-machine settings and emits current ambient illumination as a live numeric signal
- `rainsensor` authors powered-machine settings and emits a live numeric rain-intensity signal based on the current weather when climate-exposed
- `temperaturesensor` authors powered-machine settings and emits the current ambient temperature as a live numeric signal in Celsius
- `timersensor` authors powered-machine settings plus active and inactive values, active and inactive durations, and its initial phase
- `keypad` authors powered-machine settings plus numeric code, emitted signal value, signal duration, and keypad entry emote
- `filesignalgenerator` authors powered-machine settings plus the designated signal file name, initial file contents, file-system capacity, and whether that file should start publicly accessible for remote file tools
- `microcontroller` authors powered-machine settings, including optional mount-host power draw via `mountpower`
- `electronicdoor` authors source component prototype, threshold, invert mode, and automatic open and close emotes
- `relayswitch` authors source component prototype, threshold, invert mode, and programmable power-supply behaviour through the relay-controlled power base
- `alarmsiren` authors source component prototype, threshold, invert mode, volume, and repeated alarm emote

Important implementation details from this slice:
- microcontroller inline logic is compiled immediately as a `ComputerFunction` and must return a number
- input variable names are validated and normalised to lower case at compile time
- sinks and microcontrollers detach and reconnect to sibling signal sources during load and teardown using stable local source identifiers plus endpoint keys rather than transient component names
- signal propagation is event-based, so components should avoid re-emitting unchanged values
- file-backed signal generators are the reference pattern for hybrid automation/file components: the runtime component owns a mutable file system, subscribes to its file-change event, reparses the designated text file into a numeric signal, and can participate in `FileManager` / `FTP` as an `IComputerFileOwner`
- transport-facing network components are also now the reference pattern for future security-layer expansion: current builder authoring sets steady-state public, subnet, and VPN memberships, while later tunnelling or hacking features should add temporary or authorised runtime memberships without changing the authored base config
- live player reconfiguration is a separate runtime concern from builder authoring:
  - configurable sinks that support live rewiring should implement `IRuntimeConfigurableSignalSinkComponent`
  - live-programmable controllers should implement `IRuntimeProgrammableMicrocontroller`
  - live-editable file-backed signal sources should expose a file-owner surface plus any focused runtime interface needed by `programming item <item> file ...`
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

## Historical Firearm and Storage Authoring

- `lockingcashregister` authors till capacity, maximum item size, lock type, picking difficulty, and forcing difficulty. Use it instead of combining two container components.
- `container allow <tag>` and `container block <tag>` toggle admission rules; `allow clear` and `block clear` reset the respective list. Blocked matches always win and legacy definitions remain unrestricted.
- `musketcartridge powder <mass>|legacy` authors an explicit charge or restores weapon-defined charge behavior; `wad` toggles included wadding.
- `bayonetattachment style <plug|socket|sword>` and `bore <minimum> <maximum>` author the firearm attachment contract. Add an ordinary melee component to the same item prototype.
- `crossbow spanningtool <tag>|none` authors an optional readying tool, and `readyemote <emote>` authors its use. Tune delay, stamina, damage, and range on distinct ranged weapon type records.

## Instrument and Standard Authoring

- `instrument` authors family, performance trait and difficulty, volume, hands, handheld/worn/room use modes, allowed positions, styles, initial and tick stamina, interval, five emotes, and `CanPlay`, denial, play, and stop progs.
- `signalinstrument` inherits those settings and adds named local/distant/failure signal patterns, signal stamina, cooldown, and `CanSignal`, denial, and success progs. Do not compose it with a second `Instrument` component.
- `militarystandard` authors family, default identity and design, optional unit/ship association, recognition check, named visual patterns, plant/take-up/recognition emotes, bearer and recognition gates, and transition hooks.
- Use `standard set <item> ...` for scenario-specific copy identity, association, custody, or capture count. `standard reset <item> ...` restores prototype identity or clean objective state. Use the existing `ownership` command to establish the standard's lawful character or clan side.
