# FutureMUD Magic System: Capabilities, Resources, and Generators

## Purpose
This document explains the setup backbone of the magic subsystem:

- `school`
- `capability`
- `resource`
- `regenerator`

These are the pieces that determine what a school of magic is, who can use it, what magical fuel exists, and how that fuel is recovered over time.

This document is intentionally current-state focused. Where it makes an inference from the inspected code rather than from an explicit comment or command help string, that inference is called out directly.

## Quick Map
- Read the `school` section if you are defining a new family of magic and need its command verb and presentation identity.
- Read the `capability` section if you are deciding who counts as a user of a school and which powers or regenerators they receive.
- Read the `resource` section if you are adding mana-like pools or other magical fuel.
- Read the `regenerator` section if you are deciding how resources recharge over time.
- Read [Magic System: Implemented Types](./Magic_System_Implemented_Types.md) if you only need the current type list.
- Read [Magic System: Powers](./Magic_System_Powers.md) and [Magic System: Spells](./Magic_System_Spells.md) after this document if you are ready to author or extend actual magical actions.

## Backbone Dependency Order
The practical setup order for a new school is:

1. create the `school`
2. create any `resource` records it needs
3. create any `regenerator` records that produce those resources
4. create one or more `capability` records for the school
5. attach regenerators and power-unlock rules to the capabilities
6. then author `power` and `spell` content that depends on the school and the capability backbone

## Schools
### Agent Quick Map
- Runtime class: `MudSharpCore/Magic/MagicSchool.cs`
- Contract: `FutureMUDLibrary/Magic/IMagicSchool.cs`
- Builder surface: `magic school`
- Persistence: `MudsharpDatabaseLibrary/Models/MagicSchool.cs`

### Runtime Behavior
A school is a top-level grouping for one family of magic. It currently stores:

- `Name`
- `ParentSchool`
- `SchoolVerb`
- `SchoolAdjective`
- `PowerListColour`

The school is then referenced by:

- capabilities
- powers
- spells
- player command routing in `MagicModule`

Important current-state notes:

- `SchoolVerb` is the key player-facing command hook. If a character has at least one capability from a school, the module will accept that verb.
- `ParentSchool` exists and is exposed to FutureProg, but no generic inheritance behavior for powers, spells, or capabilities was identified in the inspected runtime.

### Builder Workflow
The builder entry point is `magic school`.

Current authoring flow:

- `magic school edit new <name> <verb> <adjective> [<colour>]`
- `magic school set name <name>`
- `magic school set parent <which>`
- `magic school set parent none`
- `magic school set adjective <which>`
- `magic school set verb <which>`
- `magic school set colour <which>`

Practical builder guidance:

- choose the school verb early, because it becomes the player command namespace for both powers and spells
- choose the adjective for player text, not for developer convenience
- treat parent schools as taxonomy and documentation structure unless you are also adding runtime logic that consumes the parent relationship

### Seeder and Data Author Workflow
There is no dedicated magic school seeder in `DatabaseSeeder`.

Current seeder implications:

- school records must currently be seeded manually
- the seeding work is straightforward because the persisted model is simple and not XML-backed
- the school must exist before any capability, power, or spell can reference it

Recommended manual seed fields:

- `Name`
- `ParentSchoolId` if needed
- `SchoolVerb`
- `SchoolAdjective`
- `PowerListColour`

### Developer Extension Points
There is no type registry for schools. Schools are first-class records rather than a polymorphic family.

Typical developer changes in this area are:

- adding new runtime behavior that consumes `ParentSchool`
- expanding school-level metadata
- extending player command behavior in `MagicModule`

## Capabilities
### Agent Quick Map
- Runtime class and current type: `MudSharpCore/Magic/Capabilities/SkillLevelBasedMagicCapability.cs`
- Contract: `FutureMUDLibrary/Magic/IMagicCapability.cs`
- Character integration: `MudSharpCore/Character/CharacterMagic.cs`
- Merit integration: `MudSharpCore/RPG/Merits/CharacterMerits/MagicCapabilityMerit.cs`
- Effect integration: `FutureMUDLibrary/Effects/Interfaces/IGiveMagicCapabilityEffect.cs`, `MudSharpCore/Effects/Concrete/DrugInducedMagicCapability.cs`
- Builder surface: `magic capability`
- Persistence: `MudsharpDatabaseLibrary/Models/MagicCapability.cs`

### Runtime Behavior
Capabilities are the main access-control and support layer for magic users.

The current `IMagicCapability` contract exposes:

- school membership
- power level
- inherent powers
- concentration ability
- concentration difficulty calculation
- regenerators
- a prompt-visibility flag for resources
- a clone method

The current implemented capability type is `skilllevel`.

Its runtime model includes:

- a `ConcentrationTrait`
- a `ConcentrationCapabilityExpression` that calculates total concentration points
- a `ConcentrationDifficultyExpression` that calculates sustain difficulty
- a list of attached regenerators
- a list of power rules of the form `trait >= minvalue -> grant power`

Character-side behavior is important:

- `CharacterMagic.Capabilities` is derived, not stored as a static list
- admins effectively see all capabilities
- non-admin characters gain capabilities from merits plus active effects
- `CheckResources()` syncs resource generators from the currently active capability set

Important current-state notes:

- `PowerLevel` is persisted and shown in builder output, but no generic runtime rule using it was identified in the inspected files
- `ShowMagicResourcesInPrompt` exists on the capability and is exposed in builder commands, but the implementation file itself marks prompt support as `TODO`

### Builder Workflow
The builder entry point is `magic capability`.

Current creation syntax:

- `magic capability edit new <type> <name> ...`

Current implemented type:

- `skilllevel`

For `skilllevel`, the builder creation flow requires:

- the school
- the concentration trait

Current useful editing commands include:

- `magic capability set name <name>`
- `magic capability set level <##>`
- `magic capability set trait <which>`
- `magic capability set regenerator <which>`
- `magic capability set power <which> <trait> <minvalue>`
- `magic capability set power <which>`
- `magic capability set showpower`
- `magic capability set conpoints <formula>`
- `magic capability set concentration <formula>`

Practical builder guidance:

- attach regenerators before testing powers that consume resources
- treat concentration formulas as part of the capability identity, not as an afterthought
- if a capability grants powers, keep those powers in the same school; the current builder prevents cross-school power grants

### Seeder and Data Author Workflow
There is no dedicated magic capability seeder in `DatabaseSeeder`.

Current seeder implications:

- capabilities must currently be seeded manually into `MagicCapabilities`
- the persisted `Definition` is XML-backed and must be authored carefully
- the school, concentration trait, attached regenerators, and any granted powers must already exist

Recommended manual data order:

1. seed the school
2. seed any resources
3. seed any regenerators
4. seed powers referenced by the capability
5. seed the capability
6. then seed merits, drugs, or other content that grants the capability

### Developer Extension Points
Capabilities use a runtime type registry through `MagicCapabilityFactory`.

To add a new capability type:

1. create a new concrete class under `MudSharpCore/Magic/Capabilities`
2. implement `IMagicCapability`
3. support load-from-database and builder creation
4. provide static registration methods for runtime load and builder load
5. register with `MagicCapabilityFactory` by implementing static `RegisterLoader` and `RegisterBuilderLoader`
6. store the subtype-specific data in the model's `Definition` XML and a `CapabilityModel` token

The current factory token list is:

- `skilllevel`

## Resources
### Agent Quick Map
- Runtime base: `MudSharpCore/Magic/Resources/BaseMagicResource.cs`
- Current implemented type: `MudSharpCore/Magic/Resources/SimpleMagicResource.cs`
- Contract: `FutureMUDLibrary/Magic/IMagicResource.cs`
- Holders: `CharacterMagic`, `GameItemMagic`, `CellMagic`
- Builder surface: `magic resource`
- Persistence: `MudsharpDatabaseLibrary/Models/MagicResource.cs`

### Runtime Behavior
Resources are the consumable or rechargeable magical fuel of the system.

The current `IMagicResource` contract exposes:

- `MagicResourceType`
- holder-specific starting checks
- holder-specific starting amount
- holder-specific cap
- short name
- classic prompt rendering

The current implemented resource type is `simple`.

`SimpleMagicResource` uses FutureProgs to answer:

- whether characters should have the resource at all
- whether items should have the resource at all
- whether cells should have the resource at all
- what starting amount each holder type gets
- what the cap is for any `IHaveMagicResource`

This makes the current resource system more flexible than a fixed mana pool:

- different holder categories can participate independently
- starting amounts and caps are data-driven through progs
- the same resource can appear on a character, item, or location

### Builder Workflow
The builder entry point is `magic resource`.

Current creation syntax:

- `magic resource edit new <type> <name> <shortname>`

Current implemented type:

- `simple`

Useful editing commands include:

- `magic resource set name <name>`
- `magic resource set short <name>`
- `magic resource set bottom <colour>`
- `magic resource set mid <colour>`
- `magic resource set top <colour>`
- `magic resource set cap <prog>`
- `magic resource set characterstart <prog>`
- `magic resource set itemstart <prog>`
- `magic resource set roomstart <prog>`
- `magic resource set characterhas <prog>`
- `magic resource set itemhas <prog>`
- `magic resource set roomhas <prog>`

Practical builder guidance:

- make sure the cap prog is in place before you rely on regeneration
- if a resource should exist only for characters, set the item and room predicates to false
- use the short name for compact cost displays and prompts

### Seeder and Data Author Workflow
There is no dedicated magic resource seeder in `DatabaseSeeder`.

Current seeder implications:

- resources must currently be inserted manually
- the `MagicResource` model stores both ordinary columns and a subtype `Definition` payload
- the default simple-resource implementation depends heavily on valid FutureProg IDs or names

Recommended manual data checks:

- the type token should be `simple`
- all referenced progs should already exist
- the `MagicResourceType` flags should match the holder categories you expect to use

### Developer Extension Points
Resources do not currently use a separate reflection-based factory.

Current type dispatch is handled through explicit switch logic in `BaseMagicResource`:

- builder creation in `CreateResourceFromBuilderInput`
- database loading in `LoadResource`

To add a new resource type:

1. add a new concrete class under `MudSharpCore/Magic/Resources`
2. implement the holder-specific start and cap logic
3. add builder creation support in `CreateResourceFromBuilderInput`
4. add database load support in `LoadResource`
5. choose and persist a new type token in `MagicResource.Type`
6. document the subtype-specific builder commands

The current builder/runtime type token list is:

- `simple`

## Regenerators
### Agent Quick Map
- Runtime base: `MudSharpCore/Magic/Generators/BaseMagicResourceGenerator.cs`
- Current implemented types: `LinearTimeBasedGenerator`, `StateGenerator`
- Contract: `FutureMUDLibrary/Magic/IMagicResourceRegenerator.cs`
- Builder surface: `magic regenerator`
- Persistence: `MudsharpDatabaseLibrary/Models/MagicGenerator.cs`

### Runtime Behavior
Regenerators produce resources over time for any `IHaveMagicResource`.

The runtime shape is:

- a capability grants one or more regenerators
- the holder registers each generator with the heartbeat manager
- the generator returns a minute-heartbeat delegate
- each minute, the generator applies resource gains to the holder

The current implemented generator types are:

- `linear`
- `state`

`linear` behavior:

- tied to one resource
- adds a fixed amount per minute

`state` behavior:

- stores one or more boolean state progs
- each state maps to one or more `(resource, amount)` outputs
- can either stop after the first matching state or evaluate all matching states

This makes the generator layer the main bridge between:

- resource design
- world state or character state
- the holder's long-running resource economy

### Builder Workflow
The builder entry point is `magic regenerator`.

Current creation syntax:

- `magic regenerator edit new <type> <name> <resource>`

Current implemented types:

- `linear`
- `state`

Useful editing commands for all generators include:

- `magic regenerator set name <name>`

Useful subtype commands:

- for `linear`:
  - `magic regenerator set resource <which>`
  - `magic regenerator set amount <##>`
- for `state`:
  - `magic regenerator set state add <prog> <resource> <amount> [<resource> <amount> ...]`
  - `magic regenerator set state delete <index>`
  - `magic regenerator set state swap <index1> <index2>`
  - `magic regenerator set state <index> <resource> <amount>`
  - `magic regenerator set all`

Practical builder guidance:

- use `linear` when the recharge rule is unconditional and simple
- use `state` when recharge depends on holder state, environment, posture, tags, or any other condition you can express in a boolean prog
- make sure the capability that should grant a regenerator actually toggles that regenerator on

### Seeder and Data Author Workflow
There is no dedicated magic regenerator seeder in `DatabaseSeeder`.

Current seeder implications:

- regenerators must currently be inserted manually into `MagicGenerators`
- the subtype is selected by `Type`
- subtype behavior is stored in XML in `Definition`
- all referenced resources and state progs must already exist

Recommended manual data order:

1. seed resources first
2. seed generators next
3. seed capabilities that reference those generators afterwards

### Developer Extension Points
Generators use explicit switch-based dispatch in `BaseMagicResourceGenerator`.

To add a new regenerator type:

1. create a new concrete class under `MudSharpCore/Magic/Generators`
2. implement `InternalGetOnMinuteDelegate`
3. implement subtype builder commands, show output, clone support, and XML save/load
4. add a new case in `LoadFromDatabase`
5. add a new case in `LoadFromBuilderInput`
6. choose and persist a new `MagicGenerator.Type` token

The current builder/runtime type token list is:

- `linear`
- `state`

## Integration Rules To Keep In Mind
- A character's capabilities come from merits plus active effects, not from a dedicated stored capability list.
- Drugs can currently grant temporary capabilities through `DrugInducedMagicCapability`.
- Capabilities grant regenerators and inherent powers, so they are the main bridge between the backbone and actual magical actions.
- Characters, items, and cells can all hold resources through `IHaveMagicResource`.
- Powers and spells both depend on schools, but only powers are granted directly through capability power rules.
- The player command surface is driven by the school's verb, not by a global `power` or `spell` player command.

## Current Implemented Type Summary
- School model: first-class record, no subtype registry
- Capability types: `skilllevel`
- Resource types: `simple`
- Regenerator types: `linear`, `state`

For the full inventory, see [Magic System: Implemented Types](./Magic_System_Implemented_Types.md).
