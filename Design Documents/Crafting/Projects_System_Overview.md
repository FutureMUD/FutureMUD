# FutureMUD Projects System Overview

## Purpose
This document describes the current implementation of the projects system that lives under `MudSharp.Work.Projects` and the closely related runtime surfaces that consume it.

The intended audience is engine developers, maintainers, and agents working on the FutureMUD codebase. This is a current-state document, not an aspirational redesign. Where the implementation has TODOs, stubs, or quirks, they are called out explicitly.

## Document Map
- [Projects System Builder Workflows](./Projects_System_Builder_Workflows.md) explains the in-game builder and admin command surface for authoring and operating projects.
- [Agriculture Runtime Model](../Agriculture/Agriculture_Runtime_Model.md) explains how farming operations use normal local projects plus `AgricultureProjectContext` and the `agriculture` completion action.

## Core Idea
Projects model long-running, mostly off-screen work.

They differ from short-lived craft actions in three important ways:
- project definitions are revisioned builder content, not ad hoc runtime state
- active projects persist over time and advance on a scheduled tick
- a character can have multiple active projects available, but only one currently selected labour role in `CurrentProject`

The system is composed from five layers:
- `IProject` is the revisioned template that builders approve and players browse from the catalogue
- `IActiveProject` is the live runtime instance created from a specific project revision
- `IProjectPhase` partitions the project into sequential stages
- `IProjectLabourRequirement` and `IProjectMaterialRequirement` define what a phase needs
- `IProjectAction` and `ILabourImpact` define side effects on completion and while working

## Public Contracts
### Template and active-project contracts
- `IProject` exposes catalogue visibility, initiation gating, start/cancel/finish hooks, player presentation, cancellation rules, and the ordered phase list.
- `IActiveProject` exposes the project definition, current phase, labour progress, material progress, active workers, payment reserve state, labour and material payment rates, join/leave/cancel operations, tick processing, and player-facing display.
- `IPersonalProject` is a marker for active projects owned by a single character.
- `ILocalProject` is a marker for active projects attached to a cell and worked on by multiple people in that location.

### Phase and requirement contracts
- `IProjectPhase` owns its phase number, description, labour requirements, material requirements, completion actions, duplication, deletion, and submit validation.
- `IProjectLabourRequirement` owns qualification logic, hourly progress, worker caps, mandatory status, hours remaining, builder commands, submit validation, and the attached labour impacts.
- `IProjectMaterialRequirement` owns item matching, supply logic, preview logic, quantity description, inventory-plan generation, builder commands, and submit validation.
- `IProjectAction` owns phase-completion side effects plus builder commands, duplication, and submit validation.
- `ILabourImpact` owns builder editing, duplication, apply gating via minimum hours, minimum-hours scope, and presentation for the `projects` command.
- `IProjectLabourQueueEntry` owns a queued project labour assignment, its queue order, queued timestamp, and readiness state.

### Project FutureProg surface
Active projects register `ProgVariableTypes.Project` and currently expose these dot references:
- `id`
- `name`
- `location`
- `owner`
- `workers`

These are implemented by `ActiveProject.RegisterFutureProgCompiler()` and returned from `ActiveProject.GetProperty`.

## Implemented Types
`ProjectFactory` is the central registry for all currently shipped project-family types.

### Project types
| Builder keyword | Runtime type | Purpose |
| --- | --- | --- |
| `personal` | `PersonalProject` / `ActivePersonalProject` | Private long-running work owned by one character |
| `local` | `LocalProject` / `ActiveLocalProject` | Cell-bound group work visible in the local area |

### Labour requirement types
| Builder keyword | Runtime type | Purpose |
| --- | --- | --- |
| `simple` | `SimpleProjectLabour` | Ordinary progress-producing labour |
| `endless` | `EndlessProjectLabour` | Labour that never completes and contributes no direct phase progress |
| `supervision` | `SupervisionProjectLabour` | Labour that contributes no direct progress, multiplies other workers' output, can optionally scale that multiplier by the supervisor's trait check target, and is always treated as non-mandatory |

### Material requirement types
| Builder keyword | Runtime type | Purpose |
| --- | --- | --- |
| `simple` | `SimpleProjectMaterial` | Counted items matched by tag and minimum quality |
| `commodity` | `CommodityProjectMaterial` | Weighted commodity inputs matched by material, optional tag, and quality |
| `commoditytag` | `CommodityTagProjectMaterial` | Weighted commodity inputs matched by material tag, optional pile tag, characteristics, and quality |

### Completion action types
| Builder keyword | Runtime type | Purpose |
| --- | --- | --- |
| `prog` | `ProgAction` | Executes a FutureProg when the phase completes |
| `skilluse` | `SkillUseAction` | Grants one or more free skill checks to the project owner when the phase completes |
| `agriculture` | `AgricultureOperationAction` | Applies a field operation from `AgricultureProjectContext` when a local agriculture project completes |
| `commodityoutput` | `CommodityOutputProjectAction` | Creates a commodity pile in the active project location or owner fallback location when the phase completes |
| `resourcediscovery` | `ResourceDiscoveryProjectAction` | Creates one configured visible resource marker in the active project location, gated by an optional location tag and duplicate-prevention tag |

### Labour impact types
| Builder keyword | Runtime type | Purpose |
| --- | --- | --- |
| `trait` | `TraitImpact` | Adds or subtracts trait bonus values, optionally gated to a specific bonus context |
| `healing` | `HealingImpact` | Modifies healing checks, healing rate, and infection chance |
| `job` | `JobEffortImpact` | Converts project work into ongoing-job performance effort each project tick |
| `cap` | `TraitCapImpact` | Modifies trait caps while the impact applies |

## Runtime Model
### Revisioned definitions versus live instances
Project templates are revisioned content:
- `Project` derives from `EditableItem`
- projects are created, edited, submitted, and reviewed through `EditableRevisableItemHelper.ProjectHelper`
- active projects store both `ProjectId` and `ProjectRevisionNumber`, so a live instance is pinned to the exact revision it was started from

Live project state is separate:
- `ActiveProject` and its subclasses track the current phase, active workers, and accumulated progress
- live progress is not written back to the project definition
- cancelling or finishing an active project destroys only the runtime instance

### Phases as the unit of progression
Each `IProject` contains an ordered phase list.

Each phase may contain:
- labour requirements
- material requirements
- completion actions

Phase completion moves the active project to the next phase or finishes the project if no further phase exists.

### Labour, materials, actions, and impacts
Labour requirements answer:
- who is qualified
- how fast they progress
- how many workers can contribute at once
- what temporary or ongoing impacts apply while working

Material requirements answer:
- what items count
- how much must be supplied
- how the `project supply` and `project preview` commands locate items automatically

Actions run when the current phase completes.

Impacts apply while a character is working on a labour role and are consumed elsewhere in the codebase through marker interfaces such as:
- `ILabourImpactTraits`
- `ILabourImpactTraitCaps`
- `ILabourImpactHealing`
- `ILabourImpactActionAtTick`

## Lifecycle
### Loading and factory registration
Project data is loaded by `IFuturemudLoader.LoadProjects()` in `FuturemudLoaders`.

The loader:
- loads revisioned project definitions, their phases, and nested actions, labour requirements, impacts, and materials
- reconstructs concrete template types through `ProjectFactory.LoadProject`
- loads active runtime instances through `ProjectFactory.LoadActiveProject`

Factory registration currently lives entirely in `ProjectFactory`, which is the authoritative list of valid builder keywords.

### Catalogue visibility and initiation gating
`Project` stores these gating hooks in its definition XML:
- `AppearInProjectListProg`
- `CanInitiateProg`
- `WhyCannotInitiateProg`
- optional `CanCancelProg`
- optional `WhyCannotCancelProg`
- optional `OnStartProg`
- optional `OnFinishProg`
- optional `OnCancelProg`

Catalogue visibility currently requires:
- the project revision status is `Current`
- `AppearInProjectListProg(character)` returns true

Initiation currently requires:
- the project revision status is `Current`
- `CanInitiateProg(character)` returns true
- the character does not already have another personal project from the same definition id

### Active instance creation
Starting a project creates a concrete active instance:
- `PersonalProject.InitiateProject` creates `ActivePersonalProject`, adds it to the gameworld, and adds it to the character's personal projects
- `LocalProject.InitiateProject` creates `ActiveLocalProject`, adds it to the gameworld, and registers it on the current cell

`OnStartProg(project)` is executed from the active-project constructor.

Starting a project does not automatically join any labour role. It is normal for an active project to exist with no currently active workers.

### Labour joining, switching, and qualification
`project join` works against the current phase only.

Join rules are enforced by:
- `CharacterIsQualified`
- `MaximumSimultaneousWorkers`
- current phase membership
- the player-facing NPC displacement concession, which lets qualified player characters take over a full labour role when at least one current worker in that role is an NPC

Joining one project labour automatically calls `Leave` on the character's previously selected `CurrentProject.Project` before assigning the new one.

This means the system models:
- many active projects existing at once
- exactly one currently selected labour role per character

### Queueing next labour
`project queue` is now implemented as a conservative character-owned FIFO queue.

The shipped behavior is:
- queue entries target an existing active project plus one current-phase labour requirement
- queue entries are evaluated only when the character is idle
- only the first queued entry is considered at a time
- blocked entries stay queued with a status such as `Waiting For Slot`, `Waiting For Qualification`, or `Waiting For Location`
- stale entries are removed automatically if the project disappears or the labour is no longer in the current phase

Queue activation is re-evaluated when it matters, including:
- after `project quit`
- after labour completion clears the worker's active role
- after a project finishes or is cancelled
- after a local-project slot opens
- on login
- when an idle character enters a cell

A queued player assignment is considered ready when the labour has a genuinely free slot or when the slot is full only because an NPC worker can be displaced. NPC queue entries still require a genuinely free slot.

### Material supply
Material supply is driven by `IProjectMaterialRequirement.GetPlanForCharacter`.

The current flow is:
1. The material requirement creates an inventory plan.
2. `project preview` or `project supply` scouts the first feasible target with original reference `target`.
3. The requirement decides how much of that item counts.
4. If that requirement has a per-unit material payment, the project preflights the exact contribution amount against its virtual cash reserve before consuming the item.
5. `FulfilMaterial` increments active-project progress for that requirement.

Supply is always against the current phase only. If a paid material requirement cannot be paid, the supply attempt is rejected and the item is not consumed. Clearing the material rate makes the requirement volunteer-supplied again.

### Scheduled ticking and labour progress
Active projects advance in the main scheduler loop.

The current tick path is:
1. `FuturemudLoaders` schedules `"Main ActiveProjects Tick"` every `ProjectTickMinutes`.
2. Each active project runs `DoProjectsTick()`.
3. Every active labour entry contributes progress using `HourlyProgress(actor) * ProjectProgressMultiplier`.
4. Supervision-style multipliers are folded in through `ProgressMultiplierForOtherLabourPerPercentageComplete`.
5. If that labour requirement has an hourly payment, the project reserves the tick's fraction of hourly pay as a project payable before recording skill use or progress.
6. After progress, each worker gains both `CurrentProjectHours += ProjectProgressMultiplier` and `CurrentProjectProjectHours += ProjectProgressMultiplier`.
7. Any `ILabourImpactActionAtTick` impacts execute.

The default static settings currently include:
- `ProjectTickMinutes = 15`
- `ProjectProgressMultiplier = 0.25`

The standard labour check driving progress is `CheckType.ProjectLabourCheck`.

### Phase transition and finish behavior
When a phase is considered complete:
- each `IProjectAction` in `CurrentPhase.CompletionActions` is executed in ascending `SortOrder` and then id order
- the active project either advances to the next phase or finishes entirely
- labour and material progress dictionaries are cleared for the new phase
- only mandatory labour and material requirements gate phase completion

The two active-project families behave differently:
- personal projects may automatically rejoin the owner to the first qualified labour in the new phase if they were already working when the phase flipped
- local projects clear all active labour on phase transition and require workers to join again manually

When the last phase completes:
- `OnFinishProg(project)` executes
- the runtime active project is destroyed
- the project is removed from the character or cell container

### Cancellation
Cancellation rules are delegated to the template definition:
- hard engine invariants run first
- administrators bypass builder-authored cancel rules, but not hard invariants
- optional `CanCancelProg(character, project)` and `WhyCannotCancelProg(character, project)` can author content-driven policy
- if no cancel progs are set, both personal and local projects fall back to owner-only cancellation

When cancellation succeeds:
- `OnCancelProg(project)` executes
- the runtime active project is destroyed

The current hard invariant is:
- a personal project cannot be cancelled while any unfinished active job still references that exact active project instance

Any unspent payment reserve on a deleted active project is removed from the project's virtual cash reserve and returned to the project owner as a claimable project payable.

### Project payment reserves
Active projects can hold a virtual cash reserve in a specific currency. The reserve uses the shared economy virtual-cash ledger path, while characters still fund and withdraw it through ordinary physical currency items at the command edge.

Project owners and administrators can:
- fund the reserve with `project fund`
- withdraw unspent reserve cash with `project withdraw`
- set the payment currency while the reserve is empty
- set or clear per-hour labour rates by current-phase labour requirement
- set or clear per-unit material rates by current-phase material requirement

Labour payments are measured in hours, so the normal 15-minute project tick pays one quarter of the configured hourly rate when the project uses the default `ProjectProgressMultiplier = 0.25`. Labour payments become persisted project payables that the worker must later claim. Material payments are paid immediately when `project supply` successfully contributes counted material.

Labour payment increases apply immediately. A decrease or removal also applies immediately when nobody is currently assigned to that labour, but if active workers are present the command creates an `Accept` proposal. Accepting recomputes the current workers, removes them from that labour through the normal project leave path, and then applies the lower rate.

Paid contributions are pay-before-progress:
- if a paid labour tick cannot be funded, that worker is removed from the active project before skill tracking, progress, hours, or labour-impact actions are applied
- if a paid material contribution cannot be funded, `project supply` rejects the attempt before consuming the item
- clearing a labour or material rate makes that requirement unpaid/volunteer again

Outstanding labour payables can be claimed as cash or deposited into a character-owned active bank account. Payables persist separately from the active project so workers can still claim earned labour after the project finishes or is cancelled.

## Persistence and Revisioning
### Builder content
Project definitions persist across these EF models:
- `Project`
- `ProjectPhase`
- `ProjectLabourRequirement`
- `ProjectLabourImpact`
- `ProjectMaterialRequirement`
- `ProjectAction`

Common properties such as names, descriptions, sort order, mandatory flags, and counts live in table columns.

Subtype-specific configuration is XML-backed in `Definition` fields:
- project-level FutureProg ids and tagline live in `Project.Definition`
- labour subtype configuration lives in `ProjectLabourRequirement.Definition`
- material subtype configuration lives in `ProjectMaterialRequirement.Definition`
- action subtype configuration lives in `ProjectAction.Definition`
- impact subtype configuration lives in `ProjectLabourImpact.Definition`

### Active runtime state
Active runtime instances persist separately in:
- `ActiveProject`
- `ActiveProjectLabour`
- `ActiveProjectMaterial`
- `ProjectPayable`

The active project row stores the selected payment currency. Labour and material progress rows also store the optional payment rate for their requirement. `ProjectPayable` stores earned but unclaimed labour pay, owner refund payables, the project name and revision at the time the payable was created, the earning character, currency, optional labour requirement, and claim metadata. It deliberately does not require a live active-project foreign key, so payables survive project completion and cancellation.

Character-side links are stored through:
- `Character.CurrentProjectId`
- `Character.CurrentProjectLabourId`
- `Character.CurrentProjectHours`
- `Character.CurrentProjectProjectHours`
- `ProjectLabourQueue`
- the character's `ActiveProjects` collection for owned personal projects

Local projects persist their owning cell id.
Personal projects persist their owning character id.

## Integration Points
### Command surface
The player and admin interaction layer is the `project` command in `CraftModule`, plus the summary `projects` command.

These commands cover:
- catalogue browsing
- initiation
- joining and leaving labour
- queueing next labour
- cancellation
- material preview and supply
- funding, withdrawing, payment-rate setup, and claiming project payments
- admin list/show/edit/set/review flows

### Character work state
`CharacterWork` persists and restores:
- `PersonalProjects`
- `CurrentProject`
- `CurrentProjectHours`
- `CurrentProjectProjectHours`
- `ProjectLabourQueue`

Multiple subsystems consume `CurrentProject` directly, so project work is not isolated inside the work namespace.

### Cell-local project registration
`Cell` maintains `LocalProjects`.

This is how:
- local projects are shown in `projects`
- `project details`, `project join`, `project supply`, and similar commands find local active instances
- local project lifecycle messages are scoped to a physical location

### Employment integration
Ongoing jobs can point at a personal project definition when `AppearInJobsList` is enabled.

Current job integration points are:
- `OngoingJobListing` exposes builder commands to choose a personal project definition for the job
- applying for such a job creates an `ActivePersonalProject`
- `JobEffortImpact` adds effort into `ActiveJob.CurrentPerformance`
- ending the employment attempts to cancel the linked active personal project

Active project names also replace `@job` with the linked active job's name through `ActiveProject.Name`.

### NPC project-worker AI
`ProjectWorkerAI` lets NPCs seek paid active projects without going through the unified employment host model.

The AI evaluates visible live local projects by current-phase labour requirements. It requires:
- a configured minimum hourly payment in its selected currency
- a funded project reserve with enough cash for at least one tick of work
- a reachable project location within its pathing range
- a labour requirement the NPC qualifies for and that still has a free worker slot

When several projects qualify, it chooses the highest global-base-currency equivalent hourly rate, paths to the project location, joins the selected labour, and claims outstanding project payables on its hourly tick. NPC project workers do not displace other NPCs or players; displacement is a player-facing join concession. The AI can optionally deposit claimed project payments into an owned bank account, and can open a configured account type if no suitable account exists.

### Trait, trait-cap, healing, and infection consumers
Project labour impacts currently feed into:
- `BodyTraits` for `ILabourImpactTraits`
- `BodyTraits` for `ILabourImpactTraitCaps`
- `BodyBiology` for healing rate and healing bonus modifiers
- `SimpleOrganicWound` for infection chance

Each impact now chooses whether its minimum-hours gate is measured against:
- labour continuity through `CurrentProjectHours`
- project continuity through `CurrentProjectProjectHours`

Healing-impact minimum-hours gating is now applied consistently to infection chance in the same way it is already applied to healing-rate and healing-check modifiers.

The free skill-check action uses `CheckType.ProjectSkillUseAction`.

The commodity-output action uses `CommodityGameItemComponentProto.CreateNewCommodity(...)` so it works with the system commodity prototype instead of manually loading an item prototype. It persists a solid material, positive mass, optional pile tag, optional indirect quantity display, optional local echo, and fixed commodity characteristics.

## Extending the System
Start in `FutureMUDLibrary` if the new feature needs a new public contract. Concrete implementations belong in `MudSharpCore`.

### Adding a new project type
1. Add or extend the public interface if the type needs new public surface.
2. Implement the template subclass of `Project`.
3. Implement the active runtime subclass of `ActiveProject`.
4. Implement load, create, `SaveDefinition`, `Show`, `ShowToPlayer`, `InitiateProject`, the cancellation fallback or invariant hooks, and `LoadActiveProject`.
5. Register the new builder keyword in `ProjectFactory.CreateProject`, `LoadProject`, and `ValidProjectTypes`.
6. Wire any extra runtime containers similar to personal-project character registration or local-project cell registration.

### Adding a new labour type
1. Prefer inheriting from `ProjectLabourBase`.
2. Implement subtype persistence, duplication, `Show`, `ShowToPlayer`, `HoursRemaining`, and any subtype-specific builder commands.
3. Make sure `CanSubmit` validates all required subtype data.
4. Register create/load keywords in `ProjectFactory`.
5. If the labour changes how other labour behaves, implement `ProgressMultiplierForOtherLabourPerPercentageComplete` or other overrides deliberately.

### Adding a new material type
1. Prefer inheriting from `MaterialRequirementBase`.
2. Implement item matching, supply, preview, quantity description, inventory-plan location, duplication, builder commands, and submit validation.
3. Persist subtype data in `SaveDefinition`.
4. Register create/load keywords in `ProjectFactory`.

### Adding a new action type
1. Prefer inheriting from `BaseAction`.
2. Implement subtype persistence, builder commands, submit validation, and `CompleteAction`.
3. Register create/load keywords in `ProjectFactory`.
4. If action ordering matters, preserve the current ascending `SortOrder` behavior or deliberately document any change.

### Adding a new impact type
1. Prefer inheriting from `BaseImpact`.
2. Implement subtype persistence, builder commands, submit validation, and display methods.
3. Register create/load keywords in `ProjectFactory`.
4. If the impact should affect other systems, add or reuse a marker interface and wire the consuming subsystem to query that interface.
5. If the impact should act each tick, implement `ILabourImpactActionAtTick`.

### Cross-cutting expectations
For every new family member, the expected minimum implementation work is:
- load
- save
- duplicate
- builder editing help and subcommands
- player and builder presentation
- submit validation
- factory registration
- any required integration consumers

## Known Quirks And Edge Cases
- Queue entries are intentionally current-phase-only. If a local project advances phase, old queued labour entries for the previous phase become stale and are removed rather than being remapped.
- For backwards compatibility, characters loaded from worlds that predate `CurrentProjectProjectHours` bootstrap that value from `CurrentProjectHours` if they are already assigned to a current project.
- Older supervision labour definitions that predate multiplier persistence may have no saved multiplier value. Those now load with a safe default of `100%` rather than preserving the prior broken zero-multiplier behavior. Older definitions also default to non-scaled supervision until a builder enables the `scaled` option.
- `JobEffortImpact` is created from the builder keyword `job`, but its concrete type stores and loads using the runtime type string `JobEffort`. Treat the builder keyword list in `ProjectFactory` as the source of truth for authoring.
- Starting a project does not auto-join labour. A project may be active, visible in `projects`, and still have nobody currently working on any labour requirement.
