# FutureMUD Crafting System Overview

## Purpose
This document suite explains the verified current implementation of FutureMUD crafting centred on `MudSharp.Work.Crafts` and the related systems that make crafts usable in play and editable in game.

The intended audience for this head document is:

- engine developers
- Codex agents and other AI assistants working in this repository
- technical builders who need a fast orientation before diving into the deeper runtime or builder workflow documents

## Document Map
- [Crafting System Runtime and Extensibility](./Crafting_System_Runtime_and_Extensibility.md) covers runtime behaviour, persistence, implemented types, extension seams, and cross-system integration.
- [Crafting System Builder Workflows](./Crafting_System_Builder_Workflows.md) covers the in-game authoring and review workflow, command surface, and builder-facing reference material.

## What Crafts Are
In FutureMUD, a craft is a revisioned, builder-authored definition for an on-screen, phase-based activity performed by a character in the world.

A craft combines:

- availability and permission logic
- tools that must be present in particular states
- inputs that are found, reserved, consumed, or transformed during execution
- one or more timed phases with visible echoes
- an outcome check at a designated fail phase
- normal products and fail products
- optional start, cancel, and completion callbacks through FutureProg

At runtime, starting a craft does not instantly create the final result. Instead, the engine creates a system-generated in-progress craft item, applies an active effect to the crafter, and advances the work through timed phases until the craft completes, fails, or is interrupted.

## Scope Boundaries
Crafts are not the only work-oriented subsystem in FutureMUD.

Important distinctions:

- Crafts versus projects: crafts are on-screen, time-phased actions a character is actively performing in the room right now. Projects model longer-lived labour and supply workflows that continue as broader works rather than as a single visible action sequence.
- Crafts versus butchery and salvage: `butcher`, `salvage`, and `skin` are separate command-driven subsystems with their own runtime logic. They are adjacent in purpose, but they are not implemented as `ICraft`.
- Crafts versus one-shot spawning: some craft products load ordinary items, but the craft system itself is about staged execution, not just spawning outputs.

## Subsystem Map
| Layer | Current responsibility | Typical locations |
| --- | --- | --- |
| Contracts | Public interfaces and shared abstractions for crafts, inputs, tools, products, phases, and active craft state | `FutureMUDLibrary/Work/Crafts`, `FutureMUDLibrary/GameItems/Interfaces`, `FutureMUDLibrary/Effects/Interfaces` |
| Runtime logic | Craft execution, validation, builder editing, phase handling, tool and input scouting, quality calculation, active effect behaviour | `MudSharpCore/Work/Crafts`, `MudSharpCore/Commands/Modules/CraftModule.cs` |
| Persistence | Revisioned craft rows plus child phase/input/tool/product rows | `MudsharpDatabaseLibrary/Models/Craft*.cs` |
| Command surface | Player craft use, builder craft editing, review workflow, list and search helpers | `MudSharpCore/Commands/Modules/CraftModule.cs`, `MudSharpCore/Commands/Helpers/EditableRevisableItemHelper.cs` |
| Active in-world tracking | System-only active craft item prototype/component plus active character effect | `MudSharpCore/GameItems/Prototypes/ActiveCraftGameItemComponentProto.cs`, `MudSharpCore/GameItems/Components/ActiveCraftGameItemComponent.cs`, `MudSharpCore/Effects/Concrete/ActiveCraft.cs` |
| Seeder and examples | Stock FutureProgs and seeded craft definitions that demonstrate common patterns | `DatabaseSeeder/Seeders/ItemSeederCrafting.cs` |
| Cross-system integrations | Item skins, tool items, tags, materials, liquids, NPC templates, AI, inventory plans, review framework, economy search surfaces | multiple runtime locations; see the runtime document for the verified list |

## Core Concepts
- Craft definition: a single revisable `ICraft` record with metadata, check settings, phases, inputs, tools, products, and fail products.
- Phases: ordered `ICraftPhase` records with duration, success echo, fail echo, exertion level, and stamina cost.
- Inputs: `ICraftInput` implementations that know how to find, reserve, consume, and describe required materials or target items.
- Tools: `ICraftTool` implementations that know how to find, score, and optionally consume tool durability during the craft.
- Products versus fail products: normal products are produced on the success branch; fail products are produced once the craft has failed and the fail echoes advance.
- Active craft progress item: a system-generated game item carrying `IActiveCraftGameItemComponent` that stores reserved inputs, produced outputs, phase state, and craft revision linkage.
- Availability, can-use, and callback progs: FutureProg hooks control whether a craft appears in lists, whether it can currently be started, why it cannot be started, and what happens on start, cancel, or completion.

## Important Implementation Truths
- Crafts are revisable editable items and participate in the repository's standard build-review-submit workflow rather than being ad hoc runtime objects.
- Craft validity depends on more than field presence. A craft can have all its child records and still be invalid because of bad echo references, a missing appear prog, an invalid fail phase, or invalid subtype configuration.
- Echo tokens drive behaviour. References such as `$i1`, `$t2`, `$p1`, and `$f1` are not only presentation text; they also control when inputs are consumed, when tools are required, and when products become available.
- Active craft items are system-generated, read-only support objects. They are not intended to be manually built, manually loaded, or used as ordinary content prototypes.

## Recommended Reading Order
1. Read [Crafting System Runtime and Extensibility](./Crafting_System_Runtime_and_Extensibility.md) if you need to change code, understand lifecycle and persistence, or add a new craft subtype.
2. Read [Crafting System Builder Workflows](./Crafting_System_Builder_Workflows.md) if you need to create, revise, validate, test, or review craft content in game.
