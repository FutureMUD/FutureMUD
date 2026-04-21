# FutureMUD Crafting System: Runtime and Extensibility

## Scope
This document explains the verified current implementation of the FutureMUD crafting subsystem.

It focuses on the code centred on `MudSharp.Work.Crafts`, but it also includes the adjacent contracts, persistence, commands, item integrations, and AI hooks required to understand how crafts actually work in the live engine.

This document is intentionally current-state focused. Where it gives extension guidance rather than describing existing runtime behaviour, that guidance is explicitly called out as inferred from the present implementation style.

## Layered Shape
Crafting is not implemented as a single isolated module. It is a distributed subsystem with one dominant runtime namespace and a supporting ring of contracts, persistence, commands, item integrations, and seeded content.

| Layer | Current responsibility | Typical locations |
| --- | --- | --- |
| Contracts | Public craft, input, tool, product, phase, and active-state interfaces | `FutureMUDLibrary/Work/Crafts`, `FutureMUDLibrary/GameItems/Interfaces`, `FutureMUDLibrary/Effects/Interfaces` |
| Runtime | Concrete craft execution, editing, phase orchestration, subtype implementations, active craft item behaviour | `MudSharpCore/Work/Crafts`, `MudSharpCore/GameItems/Components`, `MudSharpCore/Effects/Concrete` |
| Persistence | Revisioned craft tables and child records for phases, inputs, products, and tools | `MudsharpDatabaseLibrary/Models/Craft.cs`, `CraftPhase.cs`, `CraftInput.cs`, `CraftTool.cs`, `CraftProduct.cs` |
| Boot and loading | Craft loading and registration timing within world startup | `MudSharpCore/Framework/FuturemudLoaders.cs` |
| Commands and review | Player use, builder editing, listing, filtering, cloning, and review | `MudSharpCore/Commands/Modules/CraftModule.cs`, `MudSharpCore/Commands/Helpers/EditableRevisableItemHelper.cs` |
| Seeder and stock examples | Seeded FutureProgs and stock craft content | `DatabaseSeeder/Seeders/ItemSeederCrafting.cs` |
| Cross-system integration | Item skins, tool items, tags, liquids, NPC templates, AI, inventory plans, economy search | multiple runtime locations described below |

## Main Public Contracts
The crafting subsystem is anchored around the following public interfaces:

- `ICraft`
- `ICraftPhase`
- `ICraftInput`
- `ICraftTool`
- `ICraftProduct`
- `IVariableInput`
- `IActiveCraftEffect`
- `IActiveCraftGameItemComponent`

Important contract consequences:

- `ICraft` is the top-level revisable definition and exposes the player-facing lifecycle methods such as `CanDoCraft`, `BeginCraft`, `PauseCraft`, `CanResumeCraft`, `ResumeCraft`, `HandleCraftPhase`, `DisplayCraft`, and `CalculateCraftIsValid`.
- `ICraftInput`, `ICraftTool`, and `ICraftProduct` are polymorphic extension seams. Most builder-visible craft variety comes from subtype instances rather than from branching inside `Craft` itself.
- `IVariableInput` is the bridge for inputs that can supply characteristic values to variable-aware products.
- `IActiveCraftGameItemComponent` is the persisted runtime state holder for a live craft in progress.

## Boot and Load Order
Crafts are loaded as part of the normal game boot sequence.

Verified current load order details:

- `LoadCrafts()` runs after FutureProgs are loaded, so craft progs can be resolved during craft construction.
- `LoadCrafts()` runs before world items are loaded, which matters because active craft item components refer back to craft definitions by id and revision.
- the craft loader reads `Models.Craft` rows, orders them as needed, and materialises live `MudSharp.Work.Crafts.Craft` instances into `Gameworld.Crafts`

This ordering is important because crafts depend on several already-loaded domains:

- FutureProgs for availability and callback hooks
- traits for outcome checks
- tags, materials, liquids, item prototypes, skins, and NPC templates for subtype definitions

## Persistence Model
The persistence model is fully revision-aware.

### Top-level craft row
Each craft revision is stored in `MudsharpDatabaseLibrary/Models/Craft.cs` with fields for:

- id and revision number
- editable item linkage
- name, blurb, category, action description, and active craft item short description
- quality weighting fields
- free check count
- check trait and difficulty
- fail threshold and fail phase
- quality formula text
- practical versus theoretical flag
- appear/can-use/why-cannot-use/start/complete/cancel prog ids
- interruptable flag

### Child records
Each craft revision owns child rows for:

- `CraftPhase`
- `CraftInput`
- `CraftTool`
- `CraftProduct`

Important child-record details:

- phases are regular structured rows
- inputs, tools, and products persist subtype-specific settings in XML `Definition` blobs
- products also persist `IsFailProduct`
- products optionally persist `MaterialDefiningInputIndex`
- child records store `OriginalAdditionTime`, which the runtime uses to preserve builder order across load and revision clone paths

### Revision cloning behaviour
The craft clone and create-new-revision paths do more than duplicate top-level data:

- phases are copied as full phase records
- inputs and tools are cloned first so the new child ids exist
- products are then cloned, with input and tool id remapping available where needed
- subtype implementations that store child ids in their XML definitions can override revision-save behaviour to rewrite those ids safely

This remapping step is important for products such as:

- `UnusedInputProduct`
- `ScrapInputProduct`
- `DNATestProduct`
- any future subtype that stores input or tool ids inside its persisted definition

## Runtime Registration and Type Discovery
Craft subtypes are loaded through three reflection-backed factories:

- `CraftInputFactory`
- `CraftToolFactory`
- `CraftProductFactory`

Verified current pattern:

- each factory scans the executing assembly for types implementing the relevant interface
- each discovered type may expose a public static registration method with a fixed name:
  - `RegisterCraftInput`
  - `RegisterCraftTool`
  - `RegisterCraftProduct`
- that method registers:
  - a database type name used when loading persisted rows
  - a builder alias used by in-game craft authoring commands

This means subtype registration is data-driven once the concrete class is compiled into `MudSharpCore`; there is no central switch statement in `Craft` itself.

## Runtime Lifecycle
The main craft lifecycle is distributed across `Craft`, `ActiveCraftGameItemComponent`, and `ActiveCraftEffect`.

### 1. Listing and visibility
`ICraft.AppearInCraftsList(actor)` returns true only when all of the following hold:

- the craft is the current approved revision
- the craft is valid
- the appear prog returns true for the actor, unless the actor is an administrator

This is why a craft can exist in the database yet still be invisible to normal players.

### 2. Feasibility check
`ICraft.CanDoCraft(character, component, allowStartOnly, ignoreToolAndMaterialFailure)` checks:

- character state, combat, and movement blockers
- `CanUseProg`, if one is configured
- tool feasibility through inventory plans
- input feasibility through scouting and clash resolution

Important behaviour:

- interruptable crafts can be allowed to start or resume when only the immediate next phase is currently feasible
- failures distinguish between missing tools, missing wielders/hands, and missing materials

### 3. Starting a craft
`BeginCraft` performs the following actions:

1. create a system-generated active craft item via `ActiveCraftGameItemComponentProto.LoadActiveCraft`
2. place it in the current cell and room layer
3. create an `ActiveCraftEffect` on the character
4. subscribe the effect to character and item interruption events
5. schedule the first effect expiry after 5 seconds
6. echo the beginning of the craft
7. execute `OnUseProgStart`, if configured

### 4. Active craft item state
`ActiveCraftGameItemComponent` stores the live craft state:

- `Craft`
- `Phase`
- `HasFailed`
- `CheckOutcome`
- `QualityCheckOutcome`
- `ConsumedInputs`
- `ProducedProducts`
- `UsedToolQualities`

It also serializes:

- craft id and revision
- current phase
- check outcome
- reserved input data

The component decorates the in-progress item's short and full description so the progress item visibly represents the craft.

### 5. Phase execution
Each effect expiry calls `DoNextPhase`, which in turn calls `Craft.HandleCraftPhase`.

`HandleCraftPhase` currently does the following:

1. scout tools and inputs for the requested phase
2. pause or cancel if required tools or materials are no longer available
3. enforce per-phase stamina cost
4. raise character exertion to the phase minimum
5. execute the phase inventory plan
6. apply tool duration usage and phase time multipliers
7. reserve and store newly consumed inputs if their consumption phase has been reached
8. perform the craft outcome check at the configured fail phase
9. calculate the reference quality from check, tool, and input quality sources
10. produce newly available normal or fail products
11. emit the phase or fail echo
12. set the next reschedule duration to the computed phase length

### 6. Success and failure branching
Failure is not a separate mini-craft. The existing craft continues after `HasFailed` is set.

Once the craft has failed:

- fail echoes are used instead of normal echoes
- fail products are produced according to their resolved production phases
- normal products stop being produced unless they were already produced before failure

### 7. Pause, cancel, and resume
`PauseCraft` is called when the active effect is cancelled by interruption.

Current verified behaviour:

- if the craft is not interruptable, pausing immediately becomes cancellation
- cancellation releases products, executes `OnUseProgCancel`, and deletes the active craft item
- interruptable crafts keep the active craft item so they can be resumed later

`CanResumeCraft` and `ResumeCraft` then:

- re-check whether upcoming phases are currently feasible
- recreate and subscribe an `ActiveCraftEffect`
- schedule the effect again after 5 seconds

### 8. Completion and cleanup
When the last phase completes:

- stored products are released into the room
- any releasable input-side items are also released
- `OnUseProgComplete` is executed
- the active craft item is deleted

## Skill and Quality Model
Crafting has two separate but related check concepts.

### Outcome check
The outcome check happens at `FailPhase`.

It uses:

- `CheckTrait`
- `CheckDifficulty`
- `IsPracticalCheck`
- `FailThreshold`

If the rolled outcome is at or below the fail threshold, the craft enters the failure branch.

### Free skill checks
`FreeSkillChecks` are extra improvement rolls using the same trait and difficulty after the main outcome check has happened.

### Quality check
Reference product quality is determined by combining:

- the result of the craft quality check and `QualityFormula`
- net tool quality weighted by `ToolQualityWeighting`
- net input quality weighted by `InputQualityWeighting`
- the outcome-quality contribution weighted by `CheckQualityWeighting`

Current verified notes:

- the quality formula receives the `outcome` variable, and craft checks may also consult the trait context
- if `DisableCraftQualityCalculation` is set globally, item products skip applying the computed reference quality

## Phase Semantics and Echo Tokens
Phases are more than timed messages. The engine parses token references in phase echoes and fail echoes to determine when craft elements come into play.

### Tokens
| Token | Meaning | Current effect |
| --- | --- | --- |
| `$iN` | input reference | first valid success-echo appearance sets the phase when that input is considered consumed |
| `$tN` | tool reference | marks that tool as required in that phase's inventory plan |
| `$pN` | normal product reference | first valid appearance sets the phase when that product becomes produced |
| `$fN` | fail product reference | only valid in fail echoes; first valid appearance sets the fail-product production phase |

### Important defaulting rules
If no token assigns a phase explicitly:

- unreferenced inputs default to consumed phase 1
- unreferenced normal products default to `FailPhase`
- unreferenced fail products default to `FailPhase`
- tools not referenced in any normal echo are treated as required in every normal phase

### Fail-echo rules
Fail echoes have stricter validity requirements:

- they may reference only inputs that were already consumable on the success path
- they may reference only tools that actually exist
- they may reference normal products only if those products are already valid on the normal path
- they may reference fail products with `$fN`
- using `$fN` in a normal echo is invalid

### Duration, exertion, and stamina
Each phase carries:

- `PhaseLengthInSeconds`
- `ExertionLevel`
- `StaminaUsage`

At runtime:

- phase duration starts from the configured phase length
- each participating tool may multiply that duration
- stamina must be available at phase execution time or the craft pauses/cancels
- character exertion is raised to at least the phase exertion level

## Implemented Type Catalogue

### Inputs
| Builder alias | Runtime type | Purpose | Key builder options | Important notes |
| --- | --- | --- | --- | --- |
| `simple` | `SimpleItemInput` | Consume an exact item prototype | `item`, `quantity`, `quality` | Valid only after a target item is set |
| `simplematerial` | `SimpleMaterialInput` | Consume items made of a specific solid or a material tag | `material`, `tag`, `quantity`, `quality` | Accepts either an exact material or a material tag |
| `tag` | `TagInput` | Consume items with a specified tag | `tag`, `quantity`, `quality` | Basic tag-driven input |
| `tagvariable` | `TagVariableInput` | Consume tagged items that also supply characteristic definitions | `tag`, `quantity`, `variable`, `quality` | Implements `IVariableInput`; item must expose each declared definition |
| `commodity` | `CommodityInput` | Consume a commodity pile of a specific solid and mass | `material`, `weight`, optional `tag`, `quality` | Requires both exact material and positive mass |
| `commoditytag` | `CommodityTagInput` | Consume a commodity pile whose material matches a tag | `material`, `weight`, optional `tag`, `quality` | Uses a material tag instead of one exact solid |
| `liquid` | `LiquidUseInput` | Consume a specific liquid amount | `liquid`, `amount`, `quality` | Valid only with a target liquid and positive amount |
| `tagliquid` | `LiquidTagUseInput` | Consume liquid from a tagged source item | `tag`, `amount`, `quality` | Useful for containers or apparatus tagged for the craft |
| `repair` | `ConditionRepairInput` | Target an item to repair by condition percentage | `tag`, `repair`, `quality` | Uses a tagged input item as a repair target rather than raw material |

### Tools
All tool types inherit shared builder options from `BaseTool`:

- desired state: `held`, `worn`, `wield`, `room`
- `quality` / `qualityweight`
- `usetool` to toggle duration consumption on `IToolItem`

| Builder alias | Runtime type | Purpose | Type-specific options | Important notes |
| --- | --- | --- | --- | --- |
| `simple` | `SimpleTool` | Require one exact item prototype | `item` | Best when the specific prototype matters |
| `tag` | `TagTool` | Require any tool item matching a tag | `tag` | Works well with tool families such as hammers, anvils, or sewing needles |

### Products
| Builder alias | Runtime type | Purpose | Key builder options | Important notes |
| --- | --- | --- | --- | --- |
| `simple` | `SimpleProduct` | Load one or more ordinary item prototypes | `item`, `skin`, `quantity`, `material` | Optional craft-aware skin support through `IGameItemSkin.CanUseSkin(..., craft)` |
| `variable` | `SimpleVariableProduct` | Load items whose characteristic values come from `IVariableInput` sources | `item`, `skin`, `quantity`, `variable <definition> <input#>` | Each mapped input must be an `IVariableInput` that supplies the definition |
| `inputvariable` | `InputVariableProduct` | Load items whose variable values depend on which item proto was used for an input | `item`, `skin`, `quantity`, `variable ...` | Supports per-input-index and per-item-to-value mappings |
| `commodity` | `CommodityProduct` | Produce a commodity pile of a material, mass, and optional tag | `commodity`, `weight`, `tag`, `material` | Useful for intermediate materials and scrap |
| `money` | `MoneyProduct` | Produce money in a chosen currency | `currency`, `amount` | Valid only with currency plus positive amount |
| `npc` | `NPCProduct` | Spawn one or more NPCs from an approved NPC template | `template`, `quantity`, optional `prog` | Optional on-load prog must be `void(Character)` |
| `prog` | `ProgProduct` | Let a FutureProg decide which item or items are produced | `prog` | Prog must return item or collection of items and accept item/liquid input collections |
| `progvariable` | `ProgVariableProduct` | Load an item and fill characteristics through per-definition progs | `item`, `quantity`, `variable <definition> <prog>` | Uses progs instead of input indexes for characteristic resolution |
| `unusedinput` | `UnusedInputProduct` | Return copies of an unused portion of a consumed item input | `input`, `percentage` | Target input must consume items or item groups |
| `scrap` | `ScrapInputProduct` | Convert part of a consumed item input into commodity scrap | `input`, `percentage`, `tag`, `material` | Target input must consume items; output is commodity-like salvage |
| `dnatest` | `DNATestProduct` | Compare two liquid-consuming inputs and report whether they match | `input1`, `input2` | Both targeted inputs must consume liquids and must be different |
| `bloodtyping` | `BloodTypingProduct` | Analyse one liquid-consuming input against a blood model and report the result | `input`, `bloodmodel` | Requires a configured `IBloodModel` plus a liquid-consuming input |

## Extension Workflow
The following extension guidance is inferred from the current implementation style and is the safest pattern to follow when adding a new craft subtype.

1. Decide whether a new shared capability is needed.
If the new type needs a reusable public contract, add that interface to `FutureMUDLibrary`. If it is only concrete runtime behaviour, keep the new class in `MudSharpCore`.

2. Place the concrete type in the correct runtime folder.
Use one of:

- `MudSharpCore/Work/Crafts/Inputs`
- `MudSharpCore/Work/Crafts/Tools`
- `MudSharpCore/Work/Crafts/Products`

3. Persist subtype-specific state through XML definitions.
Implement constructor load from `Models.CraftInput`, `Models.CraftTool`, or `Models.CraftProduct` and persist state through `SaveDefinition()`. If the subtype stores input or tool ids, also handle revision remapping.

4. Register both the persisted type name and builder alias.
Expose a public static registration method with the exact expected name and register:

- the persisted database type string
- the builder alias used by `craft set input/tool/product add <type>`

5. Implement builder editing and help.
Every subtype should provide:

- builder help text
- subtype-specific `BuildingCommand(...)`
- `IsValid()` and `WhyNotValid()`
- `HowSeen(...)`

6. Implement search/reference helpers where applicable.
If the subtype refers to item protos, tags, or liquids, implement:

- `RefersToItemProto`
- `RefersToTag`
- `RefersToLiquid`

These power craft searching and filtering outside the craft namespace.

7. Handle runtime data carefully.
Choose the correct runtime data behaviour:

- inputs reserve and possibly own consumed items or liquid mixtures
- products may release items, spawn NPCs, or emit informational results
- variable-aware products may need `IVariableInput` support or FutureProg support

8. Add verification coverage.
Prefer unit coverage for non-trivial runtime behaviour. The current suite already contains craft tool usage tests and DenBuilder AI regression coverage that treats active crafts as real runtime state, not as a purely builder concern.

## Integration Points

### FutureProg integration
Crafts integrate with FutureProg at multiple levels.

Craft-wide progs:

- `AppearInCraftsListProg`: must return boolean and accept one `Character`
- `CanUseProg`: must return boolean and accept one `Character`
- `WhyCannotUseProg`: must return text and accept one `Character`
- `OnUseProgStart`: must accept one `Character`
- `OnUseProgComplete`: must accept one `Character`
- `OnUseProgCancel`: must accept one `Character`

Subtype-specific prog use also exists in products such as:

- `ProgProduct`
- `ProgVariableProduct`
- `NPCProduct`

### Item system integration
Crafting leans heavily on the item system.

Verified touchpoints include:

- item prototypes for simple item inputs, tools, and products
- tags for tag-based inputs and tools
- materials and liquids for commodity and liquid subtypes
- `IToolItem` for optional tool-duration usage
- `IGameItemSkin` for craft-aware skin application
- `IVariable` and characteristic definitions for variable-aware products

### Inventory plans and desired item states
Tool handling uses inventory plans, not ad hoc item grabs.

This is why tool definitions include desired states such as:

- held
- worn
- wielded
- in room

The engine checks feasibility against character anatomy and current context before allowing a craft to proceed.

### Character state, stamina, and exertion
Crafts are blocked or interrupted by:

- inability states
- movement
- melee engagement
- blocking effects
- loss of required tools or materials mid-craft
- insufficient stamina for a phase

### Revision and review framework
Crafts are not special-cased outside the normal revisable-content workflow.

They use:

- `EditableRevisableItemHelper.CraftHelper`
- standard edit/show/list/review/submit flows
- unique name enforcement across approved and in-design craft records

### Seeder content
`DatabaseSeeder/Seeders/ItemSeederCrafting.cs` seeds:

- stock availability and gating progs
- large numbers of example crafts
- common patterns such as room-bound forge tools, material-driven outputs, fail salvage, and multi-phase assembly

These seeded crafts are the best current examples of how the subsystem is expected to be used in real games.

### AI integration
`MudSharpCore/NPC/AI/DenBuilderAI.cs` treats crafts as reusable work definitions.

Verified current behaviour:

- the AI stores one selected `DenCraft`
- it can start or resume that craft
- it deliberately ignores active craft items when looking for den-anchor items

### Economy and search surfaces
Crafts are referenced outside the craft namespace for discovery and debugging.

Examples include:

- `EconomyModule`, which surfaces crafts related to an item
- `CraftModule craft find`, which searches by input, tool, or product relation
- `EditableRevisableItemHelper` custom search filters for tags, liquids, item protos, and category
- `ImplementorModule exportcrafts`, which exports craft metadata and child data for debugging

## Gotchas and Edge Cases
- `AppearInCraftsListProg` is required for craft validity, not merely for optional visibility filtering.
- `FailPhase` must point at a real phase. A craft with no valid fail phase cannot be submitted.
- Invalid echo references make the whole craft invalid. A typo such as `$i9` when only three inputs exist is enough to block submission.
- Fail echoes cannot introduce new unconsumed inputs or undeclared fail products.
- Tools default to every normal phase if they never appear in any normal echo.
- Inputs default to consumed phase 1 if they never appear in any normal echo.
- Normal products and fail products default to `FailPhase` if no earlier token reference sets their phase.
- Interruptable crafts still have to pass per-phase feasibility checks when starting or resuming; interruptability is not a bypass around tool or material scouting.
- Active craft items are system-only. `ActiveCraftGameItemComponentProto` is read-only and prevents manual load.
- Material-defining product inputs only work when the chosen input actually consumes game items or item groups.
- Tool durability or tool-time usage can be applied every phase in which the tool participates.
- Player interruption is event-driven. Movement, death, state change, item removal, and similar events can stop a craft even if no one explicitly typed a cancel command.
- Non-interruptable crafts do not survive interruption as paused craft items; they are cancelled.
- Craft names must remain unique across live and in-design entries, so cloning or renaming into an occupied name fails.
- `craft show` is the authoritative validation view. The error block there is part of the runtime contract builders depend on, not just a cosmetic summary.

## Inferred Guidance
The current system strongly suggests a preferred implementation style for future work:

1. keep craft behaviour polymorphic through subtype classes instead of growing `Craft` with new special-case branches
2. keep shared contracts in `FutureMUDLibrary` and concrete behaviour in `MudSharpCore`
3. preserve in-game authoring by always registering a builder alias and providing builder help
4. design new products and inputs so they participate in existing search, review, and revision flows

That guidance is inferred from the present codebase rather than enforced by one single base class, but it matches how the subsystem already works.
