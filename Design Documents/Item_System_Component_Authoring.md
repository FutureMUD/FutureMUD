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

### Add a new interface when the capability is new
If the capability is genuinely new and other systems need to query for it, add a new interface in `FutureMUDLibrary` first.

That interface should:
- describe the behaviour other systems care about
- avoid binding callers to a concrete component class
- stay focused on the game-facing contract, not internal XML or builder details

The runtime component in `MudSharpCore` should then implement that interface.

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

### Pattern: telecom devices versus telecom endpoints
The telephone and cellular implementation is a good reference when a subsystem has to separate "the thing a player uses" from "the thing the network addresses":
- `ITelephone` models the live handset behaviour such as dialling, ringing, pickup, answer, hangup, and speech relay
- `ITelephoneNumberOwner` models the addressed endpoint that owns the number on the telecommunications grid
- a wired handset may delegate numbering to a connected outlet, so moving the handset between outlets can change its number without changing the handset component itself
- a cellular handset usually implements both roles itself because the number stays with the device rather than a wall outlet
- if the item participates in telecom wiring or telecom-grid power, also consider `ICanConnectToTelecommunicationsGrid`, `IConnectable`, `IConsumePower`, and `IProducePower`

When authoring similar systems, decide early whether identity belongs to the device, the connection point, or both.

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
