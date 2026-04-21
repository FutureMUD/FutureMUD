# FutureMUD Magic System: Powers

## Purpose
This document explains how magic `powers` work in FutureMUD.

Powers are the hard-coded half of the magic subsystem. They are data-backed and builder-editable, but each power type still has a concrete runtime class with its own behavior.

This document is aimed at:

- agents that need the power subsystem map without reading the spell system
- developers adding or debugging a concrete power type
- builders authoring powers for a school
- seeder authors planning how power data should be staged manually

## Quick Map
- Read the runtime flow section if you need to understand how powers reach a player and how they consume resources.
- Read the builder workflow section if you are authoring content with `magic power`.
- Read the developer extension section if you are adding a new power type.
- Read [Magic System: Implemented Types](./Magic_System_Implemented_Types.md#power-types) if you only need the current type catalogue.
- Read [Magic System: Spells](./Magic_System_Spells.md) if the behavior you want is better expressed by editable triggers and spell effects instead of a new hard-coded power.

## What A Power Is
A power is a concrete runtime action that belongs to a `school` and is invoked through that school's player command verb.

In contrast to spells:

- powers are implemented as concrete C# classes
- powers expose one or more verbs for use through the school command surface
- each power type owns its own runtime flow and builder command set
- powers use common support from `MagicPowerBase` but are not assembled from generic trigger and effect pieces

## Runtime Flow
### Contracts and runtime entry points
Important files and contracts are:

- contract: `FutureMUDLibrary/Magic/IMagicPower.cs`
- runtime base: `MudSharpCore/Magic/Powers/MagicPowerBase.cs`
- runtime factory: `MudSharpCore/Magic/Powers/MagicPowerFactory.cs`
- shared interdiction helper: `MudSharpCore/Magic/MagicInterdictionHelper.cs`
- player dispatch: `MudSharpCore/Commands/Modules/MagicModule.cs`
- character aggregation: `MudSharpCore/Character/CharacterMagic.cs`

### How a character gets powers
`CharacterMagic.Powers` aggregates:

- powers explicitly learned through `LearnPower`
- powers granted inherently by active capabilities via `InherentPowers`

Important current-state note:

- the aggregation of learned powers clearly exists in `CharacterMagic`
- this documentation found the builder and runtime surfaces for power definitions, but did not identify a general builder workflow for teaching learned powers to characters
- for most inspected content flows, capability-granted inherent powers appear to be the main access path

### How players invoke powers
`MagicModule` is the player-facing gateway.

At runtime:

1. the engine checks whether the character has a capability whose school verb matches the command word
2. `MagicGeneric` resolves the school or schools behind that verb
3. the player can use:
   - `<schoolverb> powers`
   - `<schoolverb> help <powername>`
   - the concrete power verb itself
4. the chosen power's `UseCommand` method executes the type-specific behavior

### What `MagicPowerBase` handles
`MagicPowerBase` is the shared runtime and builder foundation for most powers.

It currently handles:

- school ownership
- shared name, blurb, and help text fields
- invocation costs per verb
- `CanInvokePowerProg`
- `WhyCantInvokePowerProg`
- common builder commands for shared fields
- common general-use restrictions, such as movement, combat, and blocked state checks
- crime checks when using the power
- `IsPsionic`, which switches the crime category from unlawful magic to unlawful psionics

This means most new power work should build on `MagicPowerBase` rather than reimplementing the whole editing and cost framework.

### Wards and interdiction
Targeted powers now consult the same ward/interdiction layer used by spells.

In current runtime behavior:

- `MagicPowerBase` target acquisition filters out targets blocked by matching room or personal wards
- custom target-gathering powers such as `SensePower` and `ConnectMindPower` also consult the shared interdiction helper
- this means school-based room and personal wards can block magical and psionic powers as well as spells

Unlike spells, powers do not currently use ward reflection retargeting. Their target acquisition path simply treats interdicted targets as invalid.

### How power types are loaded
`MagicPowerFactory` uses reflection to find `IMagicPower` implementers in the executing assembly and call their static `RegisterLoader` method.

That registration provides two maps:

- runtime load from the persisted `PowerModel`
- builder creation from the builder token

The persisted model stores:

- normal columns like `Name`, `Blurb`, `ShowHelp`, `MagicSchoolId`
- a `PowerModel` token
- subtype XML in `Definition`

## Builder Workflow
### Entry point
The builder entry point is `magic power`.

The shared workflow is:

- `magic power list`
- `magic power edit new <type> <school> <name>`
- `magic power edit <which>`
- `magic power show`
- `magic power set ...`

### Shared builder behavior
Shared builder editing comes from `MagicPowerBase`.

All ordinary power types get these common commands:

- `name <name>`
- `school <which>`
- `blurb <blurb>`
- `can <prog>`
- `why <prog>`
- `help`
- `cost <verb> <which> <number>`

Each concrete power type then adds its own subtype commands in its overridden `BuildingCommand`.

### Choosing powers versus spells
Builders should usually choose a `power` when:

- the action is a stable, code-backed ability that deserves its own command flow
- the effect logic is specific enough that generic trigger/effect composition would be awkward
- the school should expose a bespoke verb or multi-step interaction

Builders should usually choose a `spell` when:

- the action can be expressed as a trigger plus one or more spell effects
- the world needs builder-editable composition rather than a new C# runtime type
- the desired behavior is closer to "data-defined magical effect" than to "new command mechanic"

## Seeder and Data Author Workflow
There is no dedicated magic power seeder in `DatabaseSeeder`.

Current seeder implications:

- powers must currently be seeded manually into `MagicPowers`
- the school must already exist
- any referenced resources, progs, and other content must already exist
- the `PowerModel` token and `Definition` XML must match the concrete runtime class

Recommended manual data order:

1. schools
2. resources
3. capabilities and regenerators if the power will be granted inherently
4. powers
5. capabilities or other content that references the power

## Developer Extension Workflow
### When to add a new power type
Add a new power type when the behavior does not fit the spell system well and deserves its own runtime class, commands, and validation rules.

### Recommended implementation steps
1. Create a concrete class under `MudSharpCore/Magic/Powers`.
2. Inherit from `MagicPowerBase` or a more specific existing base if appropriate.
3. Implement:
   - constructor for builder-time creation
   - constructor for load from `Models.MagicPower`
   - `PowerType`
   - `DatabaseType`
   - `Verbs`
   - `UseCommand`
   - `SaveDefinition`
   - subtype builder commands and subtype show output
4. Add static registration in `RegisterLoader`:
   - `MagicPowerFactory.RegisterLoader(<database token>, ...)`
   - `MagicPowerFactory.RegisterBuilderLoader(<builder token>, ...)`
5. Persist subtype-specific data in the `Definition` XML.
6. Keep the builder token and the database token explicit and stable.

### Design guidance
- Use `MagicPowerBase` for shared costs, help text, progs, and school linkage.
- Put subtype-specific arguments in the builder loader rather than trying to overload the generic `magic power edit new` path.
- Keep help text player-facing, not developer-facing.
- If the power can consume resources under different verbs, define those costs per verb through the base `InvocationCosts` support.
- If the power should be treated as psionic rather than magical, make `IsPsionic` explicit and make sure the saved XML includes it through the base helper.

## Current Implemented Power Types
### Builder-registered power types
These are the currently builder-creatable power tokens registered through `MagicPowerFactory`:

| Builder token | Class | Summary |
| --- | --- | --- |
| `armour` | `MagicArmourPower` | Sustained defensive protection effect |
| `anesthesia` | `MindAnesthesiaPower` | Mental anesthesia style effect |
| `choke` | `ChokePower` | Choking or constriction style offensive power |
| `connectmind` | `ConnectMindPower` | Creates or manages mind links |
| `invisibility` | `InvisibilityPower` | Applies invisibility behavior |
| `magicattack` | `MagicAttackPower` | Direct magical attack action |
| `mindaudit` | `MindAuditPower` | Mind-reading or auditing style power |
| `mindbarrier` | `MindBarrierPower` | Mental barrier or protection effect |
| `mindbroadcast` | `MindBroadcastPower` | Broadcast-style mind communication |
| `mindexpel` | `MindExpelPower` | Expels connected minds or effects |
| `mindlook` | `MindLookPower` | Observe or inspect through mind-link mechanics |
| `mindsay` | `MindSayPower` | Directed mind-to-mind speech |
| `sense` | `SensePower` | Sense targets across a configurable distance |
| `telepathy` | `TelepathyPower` | Telepathic communication or related perception |

Important current-state note:

- `MagicArmourPower` also registers a runtime load alias of `armor` in addition to the builder-facing `armour` token. That alias matters to developers and seeder authors working with persisted `PowerModel` values, but it is not exposed as a distinct builder creation type.

### Notable base or runtime-support types
These matter to developers extending the subsystem, but they are not standalone builder-created types.

| Class | Role |
| --- | --- |
| `MagicPowerBase` | Main shared base for most powers |
| `SustainedMagicPower` | Shared support for powers that persist and consume concentration over time |
| `MagicalMeleeAttackPower` | Abstract base for melee-style magic attacks; not a standalone builder type |

## Important Current-State Notes
- School verbs are the player namespace for powers. There is no separate general player command that bypasses the school.
- Capability-granted inherent powers are a first-class runtime path and are the main inspected content linkage for powers.
- Power definitions are polymorphic and XML-backed, so database seeding must match the runtime class exactly.
- If a power type grows mostly into editable composition rather than unique runtime logic, it may be a better fit as a spell instead of as a new power class.
- Room and personal wards are authored as spell effects, but targeted powers and psionic sensing flows still respect them through the shared interdiction helper.

## Related Reading
- [Magic System Overview](./Magic_System_Overview.md)
- [Magic System: Capabilities, Resources, and Generators](./Magic_System_Capabilities_Resources_and_Generators.md)
- [Magic System: Spells](./Magic_System_Spells.md)
- [Magic System: Implemented Types](./Magic_System_Implemented_Types.md#power-types)
