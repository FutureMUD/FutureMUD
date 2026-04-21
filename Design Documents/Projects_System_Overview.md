# FutureMUD Projects System Overview

## Purpose
This document describes the current implementation of the projects system that lives under `MudSharp.Work.Projects` and the closely related runtime surfaces that consume it.

The intended audience is engine developers, maintainers, and agents working on the FutureMUD codebase. This is a current-state document, not an aspirational redesign. Where the implementation has TODOs, stubs, or quirks, they are called out explicitly.

## Document Map
- [Projects System Builder Workflows](./Projects_System_Builder_Workflows.md) explains the in-game builder and admin command surface for authoring and operating projects.

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
- `IActiveProject` exposes the project definition, current phase, labour progress, material progress, active workers, join/leave/cancel operations, tick processing, and player-facing display.
- `IPersonalProject` is a marker for active projects owned by a single character.
- `ILocalProject` is a marker for active projects attached to a cell and worked on by multiple people in that location.

### Phase and requirement contracts
- `IProjectPhase` owns its phase number, description, labour requirements, material requirements, completion actions, duplication, deletion, and submit validation.
- `IProjectLabourRequirement` owns qualification logic, hourly progress, worker caps, mandatory status, hours remaining, builder commands, submit validation, and the attached labour impacts.
- `IProjectMaterialRequirement` owns item matching, supply logic, preview logic, quantity description, inventory-plan generation, builder commands, and submit validation.
- `IProjectAction` owns phase-completion side effects plus builder commands, duplication, and submit validation.
- `ILabourImpact` owns builder editing, duplication, apply gating via minimum hours, and presentation for the `projects` command.

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
| `supervision` | `SupervisionProjectLabour` | Labour that contributes no direct progress, multiplies other workers' output, and is always treated as non-mandatory |

### Material requirement types
| Builder keyword | Runtime type | Purpose |
| --- | --- | --- |
| `simple` | `SimpleProjectMaterial` | Counted items matched by tag and minimum quality |
| `commodity` | `CommodityProjectMaterial` | Weighted commodity inputs matched by material, optional tag, and quality |

### Completion action types
| Builder keyword | Runtime type | Purpose |
| --- | --- | --- |
| `prog` | `ProgAction` | Executes a FutureProg when the phase completes |
| `skilluse` | `SkillUseAction` | Grants one or more free skill checks to the project owner when the phase completes |

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

Joining one project labour automatically calls `Leave` on the character's previously selected `CurrentProject.Project` before assigning the new one.

This means the system models:
- many active projects existing at once
- exactly one currently selected labour role per character

### Material supply
Material supply is driven by `IProjectMaterialRequirement.GetPlanForCharacter`.

The current flow is:
1. The material requirement creates an inventory plan.
2. `project preview` or `project supply` scouts the first feasible target with original reference `target`.
3. The requirement decides how much of that item counts.
4. `FulfilMaterial` increments active-project progress for that requirement.

Supply is always against the current phase only.

### Scheduled ticking and labour progress
Active projects advance in the main scheduler loop.

The current tick path is:
1. `FuturemudLoaders` schedules `"Main ActiveProjects Tick"` every `ProjectTickMinutes`.
2. Each active project runs `DoProjectsTick()`.
3. Every active labour entry contributes progress using `HourlyProgress(actor) * ProjectProgressMultiplier`.
4. Supervision-style multipliers are folded in through `ProgressMultiplierForOtherLabourPerPercentageComplete`.
5. After progress, each worker gains `CurrentProjectHours += ProjectProgressMultiplier`.
6. Any `ILabourImpactActionAtTick` impacts execute.

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
- personal projects currently block cancellation if any active job references that same project definition as its job-linked project
- local projects currently allow cancellation only by the project owner or an administrator

When cancellation succeeds:
- `OnCancelProg(project)` executes
- the runtime active project is destroyed

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

Character-side links are stored through:
- `Character.CurrentProjectId`
- `Character.CurrentProjectLabourId`
- `Character.CurrentProjectHours`
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
- cancellation
- material preview and supply
- admin list/show/edit/set/review flows

### Character work state
`CharacterWork` persists and restores:
- `PersonalProjects`
- `CurrentProject`
- `CurrentProjectHours`

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

### Trait, trait-cap, healing, and infection consumers
Project labour impacts currently feed into:
- `BodyTraits` for `ILabourImpactTraits`
- `BodyTraits` for `ILabourImpactTraitCaps`
- `BodyBiology` for healing rate and healing bonus modifiers
- `SimpleOrganicWound` for infection chance

Healing-impact minimum-hours gating is now applied consistently to infection chance in the same way it is already applied to healing-rate and healing-check modifiers.

The free skill-check action uses `CheckType.ProjectSkillUseAction`.

## Extending the System
Start in `FutureMUDLibrary` if the new feature needs a new public contract. Concrete implementations belong in `MudSharpCore`.

### Adding a new project type
1. Add or extend the public interface if the type needs new public surface.
2. Implement the template subclass of `Project`.
3. Implement the active runtime subclass of `ActiveProject`.
4. Implement load, create, `SaveDefinition`, `Show`, `ShowToPlayer`, `InitiateProject`, `CanCancelProject`, and `LoadActiveProject`.
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
- `project queue` is currently stubbed and only replies with `Coming soon.`
- Local-project cancellation policy is currently hardcoded to owner-or-admin in `LocalProject.CanCancelProject`; there is a `TODO - configurable` comment rather than a content-driven rule.
- `CurrentProjectHours` resets whenever `Character.CurrentProject` changes, including normal leave/join transitions between labour roles and projects.
- Older supervision labour definitions that predate multiplier persistence may have no saved multiplier value. Those now load with a safe default of `100%` rather than preserving the prior broken zero-multiplier behavior.
- `JobEffortImpact` is created from the builder keyword `job`, but its concrete type stores and loads using the runtime type string `JobEffort`. Treat the builder keyword list in `ProjectFactory` as the source of truth for authoring.
- Starting a project does not auto-join labour. A project may be active, visible in `projects`, and still have nobody currently working on any labour requirement.
