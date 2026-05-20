# FutureMUD Magic System Overview

## Purpose
This document is the entry point for the FutureMUD magic documentation suite.

It explains how the magic subsystem is shaped at a high level, where the important runtime and persistence surfaces live, and which companion document to read next.

The intended audience is:

- agents that need a fast map of the subsystem before making changes
- engine developers extending or debugging the runtime
- builders authoring schools, capabilities, resources, powers, and spells
- seeder authors planning how to introduce magic content into a world

## Read This If...
- Read this document if you need the subsystem map and dependency order before diving into one slice of the system.
- Read [Magic System: Capabilities, Resources, and Generators](./Magic_System_Capabilities_Resources_and_Generators.md) if you are setting up the backbone of a school or deciding who can use magic and how they recharge it.
- Read [Magic System: Powers](./Magic_System_Powers.md) if you are working on hard-coded power types, school verbs, or power-specific builder workflows.
- Read [Magic System: Spells](./Magic_System_Spells.md) if you are working on editable spell content, triggers, spell effects, or spell runtime flow.
- Read [Magic System: Implemented Types](./Magic_System_Implemented_Types.md) if you only need the currently implemented type catalogue without the narrative explanation.

## High-Level Model
FutureMUD's magic system is a distributed subsystem with one content backbone and two user-facing effect systems built on top of it.

The usual dependency order is:

`MagicSchool -> MagicResource -> MagicGenerator -> MagicCapability -> MagicPower / MagicSpell -> Merit / Drug integration -> Runtime user access`

In practice:

- a `school` defines the identity, colour, adjective, and player command verb for one family of magic
- a `resource` defines what magical fuel exists and which magic-resource holders can have it
- a `regenerator` defines how a holder gains resource over time
- a `capability` determines who counts as a user of a school, what concentration rules apply, which regenerators they receive, and which inherent powers they unlock
- a `power` is a concrete hard-coded runtime behavior with type-specific code
- a `spell` is a more data-driven template made from a trigger plus one or more spell effects
- merits and effects can grant capabilities to characters, which is how chargen and drugs currently plug into the system
- runtime access then flows through the character's capabilities, resources, powers, and known spells

## Where Magic Lives In Code
Magic is spread across contracts, runtime implementations, commands, and persistence models.

| Layer | Current responsibility | Typical locations |
| --- | --- | --- |
| Contracts | Public interfaces and enums shared across the engine | `FutureMUDLibrary/Magic` |
| Runtime | Schools, capabilities, resources, regenerators, powers, spells, triggers, spell effects | `MudSharpCore/Magic` |
| Command surface | Admin and builder editing commands, plus player-facing school verb dispatch | `MudSharpCore/Commands/Helpers/EditableItemHelperMagic.cs`, `MudSharpCore/Commands/Modules/MagicModule.cs` |
| Runtime integration | Characters, drugs, merits, items, cells, effects, combat, FutureProg exposure | `MudSharpCore/Character/CharacterMagic.cs`, `MudSharpCore/Effects/Concrete`, `MudSharpCore/RPG/Merits`, `MudSharpCore/GameItems/GameItemMagic.cs`, `MudSharpCore/Construction/CellMagic.cs` |
| Persistence | EF Core models and migrations for magic entities and stored resource amounts | `MudsharpDatabaseLibrary/Models/*Magic*`, related migrations |

## Core Runtime Relationships
### Schools
Schools are the top-level identity container for a family of magic.

Each school currently owns or anchors:

- a name
- an optional parent school
- a `SchoolVerb` used for the player command surface
- a `SchoolAdjective` used in player text
- a display colour
- all powers, spells, and capabilities assigned to that school

The parent-school relationship is currently a taxonomy reference. In the inspected runtime, it is not a generic inheritance mechanism for powers, spells, or capabilities.

### Capabilities
Capabilities answer "who can use this school?"

They currently provide:

- school membership
- a power level value
- concentration formulas
- resource regenerators granted to the user
- inherent powers unlocked by capability-specific rules
- a flag indicating whether magic resources should appear in prompts

Characters do not hard-code their schools. They derive `Capabilities` dynamically from merits and effects, then derive powers and resource generators from those capabilities.

### Resources and Resource Holders
Magic resources are not character-only.

The `IHaveMagicResource` contract is implemented by:

- characters via `CharacterMagic`
- items via `GameItemMagic`
- cells via `CellMagic`

That means resource amounts can exist on:

- the caster
- a magic item
- a room or cell

The resource itself controls whether a given holder type should start with that resource, how much it starts with, and what its cap is.

### Powers
Powers are concrete runtime classes with type-specific code.

They are:

- grouped by school
- exposed to players through the school's verb in `MagicModule`
- aggregated on characters from inherent capability powers plus any separately learned powers
- implemented as hard-coded classes under `MudSharpCore/Magic/Powers`

### Spells
Spells are the more data-driven half of the subsystem.

Each spell combines:

- a school
- a knowledge prog
- a trigger
- spell effects and optional caster effects
- casting costs
- optional material requirements
- casting and resist settings
- emotes and timing rules

The runtime spell object validates whether it is ready for game use, then handles casting checks, resist checks, duration calculation, effect application, and lockouts.

## Cross-Cutting Integration Points
### Capabilities From Merits
Characters gain permanent or chargen-defined magic access through merits implementing `IMagicCapabilityMerit`.

The current concrete merit is `MagicCapabilityMerit`, which stores a set of capabilities and contributes them to the character's derived `Capabilities` list.

### Capabilities From Effects
Characters can also gain temporary capability access through effects implementing `IGiveMagicCapabilityEffect`.

The main inspected example is `DrugInducedMagicCapability`, which is how temporary drug-driven magic access is currently expressed.

### Drugs
`BodyDrugs` applies `DrugInducedMagicCapability` when a drug produces the `DrugType.MagicAbility` effect and the active drug state matches one or more capabilities.

This means drug-driven magical access is capability-driven rather than being a completely separate magic path.

### Runtime User Access
At runtime, the player-facing flow is:

1. a character has one or more capabilities
2. the capabilities grant resource regenerators and inherent powers
3. the character's school verbs become meaningful player commands
4. the player uses `<schoolverb> powers`, `<schoolverb> spells`, `<schoolverb> cast ...`, or power verbs routed through the same school command

`MagicModule.MagicFilterFunction` and `MagicModule.MagicGeneric` are the main entry points for that player-facing dispatch.

## Builder and Seeder Author Perspective
Builders author magic through the admin `magic` command family, not through a dedicated seeder workflow.

The current builder editing areas are:

- `magic school`
- `magic capability`
- `magic resource`
- `magic regenerator`
- `magic power`
- `magic spell`

Seeder authors should note a current gap:

- there is no dedicated magic seeder in `DatabaseSeeder`
- there is no inspected interactive seeder path for schools, capabilities, resources, regenerators, powers, or spells
- seeder and stock-data authors currently need to seed these records manually, usually by writing the underlying tables and XML-backed definitions in dependency order

The recommended manual dependency order is:

1. schools
2. resources
3. regenerators
4. capabilities
5. powers
6. spells
7. merits, drugs, or other content that references the capabilities

## Companion Documents
- [Magic System: Capabilities, Resources, and Generators](./Magic_System_Capabilities_Resources_and_Generators.md) explains the backbone entities that determine access, concentration, recharge, and resource ownership.
- [Magic System: Powers](./Magic_System_Powers.md) explains how hard-coded powers work, how builders author them, and how developers add a new power type.
- [Magic System: Spells](./Magic_System_Spells.md) explains editable spell runtime, trigger and effect composition, and how to extend spell triggers or spell effects.
- [Magic System: Implemented Types](./Magic_System_Implemented_Types.md) lists the currently implemented capability, resource, regenerator, power, spell trigger, and spell effect types.
