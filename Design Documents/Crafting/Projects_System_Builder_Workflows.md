# FutureMUD Projects System Builder Workflows

## Purpose
This document explains how builders and admins currently create, revise, approve, and operate projects through the in-game command surface.

This is a builder-facing guide. It describes the command workflows that exist today, including current limitations and implementation quirks.

## Builder Mental Model
Keep these three layers separate in your head:
- A catalogue project is the revisioned template that players see in `project catalog`.
- An active project is the live runtime instance created when someone starts that template.
- A current labour role is the single task a character is presently assigned to inside one active project.

Important current behavior:
- a character can have active personal projects without currently working on one of them
- a local project can exist in a room with no active workers
- starting a project does not auto-join labour
- a character can only have one selected `CurrentProject` labour role at a time

## Player-Facing Usage
### `projects`
Use `projects` to see:
- your active personal projects
- active local projects in your current cell
- the labour role you are currently working on, if any
- both labour-continuity hours and project-continuity hours for your current assignment
- any labour impacts attached to that role, including how many hours remain before gated impacts kick in

This is the fastest way to confirm the live state after starting, joining, supplying, or cancelling a project.

### `project catalog`
Use `project catalog` to list project templates currently visible to that character.

Catalogue visibility requires the project to be:
- on a `Current` revision
- allowed by its `AppearInProjectListProg`

### `project view <name>`
Use `project view <name>` to inspect a catalogue project before starting it.

This shows the project type, tagline, phase structure, labour requirements, impacts, materials, and completion actions from the approved template revision.

### `project start <name>`
Use `project start <name>` to create a live project instance.

Current behavior:
- only catalogue-visible current revisions can be started
- `CanInitiateProg` must allow it
- personal projects cannot be started twice from the same project definition id
- starting does not auto-join labour

### `project join <project> [labour]`
Use `project join` to start working on a labour requirement in the current phase.

Current behavior:
- if exactly one labour requirement is joinable, it can be inferred
- if multiple labour requirements are joinable, the command requires an explicit labour name
- qualification and worker-cap checks are enforced
- joining a new project labour automatically leaves whatever `CurrentProject` labour you were already working on

### `project quit <project>`
Use `project quit <project>` to stop working on that active project.

This does not cancel the active project. It only removes your current labour assignment from it.
If your queue has a ready next assignment, the engine immediately tries to join it after you quit.

### `project details <project>`
Use `project details <project>` to inspect the live active project in its current phase.

This is the command to use when you need to see:
- current workers
- mandatory status
- progress already accrued
- current-phase material requirements

### `project preview <project> <requirement>`
Use `project preview` to see what item would be supplied if you used `project supply`.

The current implementation uses the requirement's inventory plan and previews the first feasible target it scouts.

### `project supply <project> [requirement]`
Use `project supply` to contribute materials to the current phase.

With no requirement argument, it lists the current phase's material requirements.

With a requirement argument, it:
1. builds the requirement's inventory plan
2. finds a feasible item automatically
3. consumes or splits that item
4. adds the supplied quantity to the active project

### `project cancel <project>`
Use `project cancel` to destroy an active project.

Current rules:
- a personal project cannot be cancelled while an unfinished active job still depends on that exact active project instance
- administrators bypass builder-authored cancellation progs, but not the active-job invariant
- if no custom cancel progs are configured, both personal and local projects fall back to owner-only cancellation

### `project queue`
Use `project queue` to manage what labour role you want to auto-join next.

Current syntax:
- `project queue`
- `project queue <project> [labour]`
- `project queue remove <index>`
- `project queue clear`

Current behavior:
- entries only target the active project's current phase
- only the first queued entry is considered when you become idle
- blocked entries remain queued and show statuses such as `Waiting For Slot`, `Waiting For Qualification`, or `Waiting For Location`
- stale entries are removed automatically when the project disappears or the queued labour is no longer part of the current phase

## Admin And Revision Workflow
### List and inspect definitions
Use:
- `project list`
- `project list all`
- `project show <project>`

`project list` shows current revisions.
`project list all` includes older revisions.

### Create or edit a definition
Use:
- `project edit <project>`
- `project edit new <type>`

Current project type keywords are:
- `personal`
- `local`

After opening a project for editing, use `project set ...` against that current editing session to modify it.

### Submit and review
Use:
- `project edit submit`
- `project review all`
- `project review mine`

For a project to appear in the player catalogue, it must end up on a `Current` revision after the normal submit and review workflow.

## Authoring Workflow
### 1. Create the project shell
Start with:
- `project edit new personal`
- `project edit new local`

Then set the basics:
- `project set name <name>`
- `project set tagline <tagline>`

For personal projects that should be attachable to ongoing jobs, also toggle:
- `project set jobs`

### 2. Configure the gating and hook progs
Projects require three gating progs before they can be submitted:
- `project set appear <prog>`
- `project set can <prog>`
- `project set why <prog>`

Optional hooks are:
- `project set start <prog>`
- `project set finish <prog>`
- `project set cancel <prog>`
- `project set cancancel <prog>`
- `project set cancancel clear`
- `project set whycancel <prog>`
- `project set whycancel clear`

Required signatures:
- `appear`: returns boolean, takes one `character`
- `can`: returns boolean, takes one `character`
- `why`: returns text, takes one `character`
- `start`: takes one `project`
- `finish`: takes one `project`
- `cancel`: takes one `project`
- `cancancel`: returns boolean, takes `character, project`
- `whycancel`: returns text, takes `character, project`

### 3. Build the phase structure
Use:
- `project set phase add`
- `project set phase remove <phase>`
- `project set phase duplicate <phase>`
- `project set phase swap <phase1> <phase2>`
- `project set phase <phase> description <description>`

Use numeric phase numbers, `first`, or `last` where supported by the underlying phase selector.

Every submitted project must have at least one phase, and every phase must have at least one labour or material requirement.

### 4. Add labour requirements
Use:
- `project set phase <phase> labour add <type> [name]`
- `project set phase <phase> labour remove <labour>`
- `project set phase <phase> labour duplicate <labour>`
- `project set phase <phase> labour <labour> show`

Common labour settings:
- `project set phase <phase> labour <labour> name <name>`
- `project set phase <phase> labour <labour> description <description>`
- `project set phase <phase> labour <labour> progress <man-hours>`
- `project set phase <phase> labour <labour> workers <amount>`
- `project set phase <phase> labour <labour> mandatory`
- `project set phase <phase> labour <labour> trait <trait>`
- `project set phase <phase> labour <labour> trait none`
- `project set phase <phase> labour <labour> minskill <amount>`
- `project set phase <phase> labour <labour> difficulty <difficulty>`
- `project set phase <phase> labour <labour> prog <prog>`
- `project set phase <phase> labour <labour> prog none`

Qualification is currently trait-or-prog based:
- builders must set a required trait, or
- builders must set a qualification prog

If both are missing, the labour requirement cannot be submitted.

### Labour type reference
| Keyword | What it does in play | Builder notes |
| --- | --- | --- |
| `simple` | Produces normal progress toward phase completion | Use for standard work measured in man-hours |
| `endless` | Never completes and does not produce real phase progress | Use only for ongoing roles that should not finish a phase by themselves |
| `supervision` | Produces no direct progress and multiplies other workers' output | Always non-mandatory in current behavior |

### Supervision labour note
`supervision` exposes:
- `project set phase <phase> labour <labour> multiplier <percent>`

Current compatibility note:
- the multiplier now persists correctly
- older supervision definitions that were saved before this fix load missing multiplier data as `100%`

### 5. Add labour impacts
Use:
- `project set phase <phase> labour <labour> impact add <type> [name]`
- `project set phase <phase> labour <labour> impact duplicate <impact>`
- `project set phase <phase> labour <labour> impact delete <impact>`
- `project set phase <phase> labour <labour> impact <impact> show`

Shared impact settings:
- `project set phase <phase> labour <labour> impact <impact> name <name>`
- `project set phase <phase> labour <labour> impact <impact> description <description>`
- `project set phase <phase> labour <labour> impact <impact> hours <hours>`
- `project set phase <phase> labour <labour> impact <impact> scope labour|project`

Impact hours control when the effect begins. `scope labour` keys off continuity on the exact current labour role; `scope project` keys off continuity on the current active project even if the worker changes labour roles inside that project.

### Impact type reference
| Keyword | What it does in play | Common extra commands |
| --- | --- | --- |
| `trait` | Modifies one trait, optionally only in one bonus context | `trait`, `bonus`, `context` |
| `healing` | Modifies healing checks, healing rate, and infection chance | `bonus`, `multiplier`, `infection` |
| `job` | Adds ongoing-job performance effort each project tick | `expression` |
| `cap` | Modifies a trait cap | `trait`, `bonus` |

### 6. Add material requirements
Use:
- `project set phase <phase> material add <type>`
- `project set phase <phase> material remove <material>`
- `project set phase <phase> material duplicate <material>`
- `project set phase <phase> material <material> show`

Shared material settings:
- `project set phase <phase> material <material> name <name>`
- `project set phase <phase> material <material> description <description>`

### Material type reference
| Keyword | What it does in play | Common extra commands |
| --- | --- | --- |
| `simple` | Counts discrete items by tag and minimum quality | `tag`, `amount`, `quality` |
| `commodity` | Counts commodity mass by material, optional tag, optional characteristics, and quality | `material`, `tag`, `amount`, `quality`, `characteristic` |

Builder notes:
- `simple` requires a tag before submit
- `commodity` requires a material before submit
- `commodity` `amount` uses the game's mass parser, not a raw item count
- commodity characteristic matching defaults to wildcard, so older requirements still accept characteristic-bearing piles
- use `characteristic none` to require uncharacterised commodity piles
- use `characteristic <definition> any`, `characteristic <definition> <value>`, or `characteristic <definition> remove` to require, pin, or remove a definition filter

### 7. Add completion actions
Use:
- `project set phase <phase> action add <type>`
- `project set phase <phase> action remove <action>`
- `project set phase <phase> action duplicate <action>`
- `project set phase <phase> action <action> name <name>`
- `project set phase <phase> action <action> description <description>`
- `project set phase <phase> action <action> sort <number>`

### Action type reference
| Keyword | What it does in play | Common extra commands |
| --- | --- | --- |
| `prog` | Executes a FutureProg when the phase completes | `prog <prog>` |
| `skilluse` | Grants free checks against a trait when the phase completes | `trait`, `checks`, `difficulty` |

Current implementation warning:
- actions execute in ascending `sort` order
- ties fall back to stable id order

## Compact Type Keyword Reference
### Project families
| Family | Keywords |
| --- | --- |
| Project | `personal`, `local` |
| Labour | `simple`, `endless`, `supervision` |
| Material | `simple`, `commodity` |
| Action | `prog`, `skilluse` |
| Impact | `trait`, `healing`, `job`, `cap` |

## Troubleshooting
### A project will not submit
Check these first:
- at least one phase exists
- every phase has at least one labour or material requirement
- `appear`, `can`, and `why` progs are all set
- each labour, material, action, and impact subtype has its required fields filled in

### A project is approved but not visible in `project catalog`
Check:
- the revision is actually `Current`
- `AppearInProjectListProg` returns true for that character

### `project start` says the character cannot begin it
Check:
- `CanInitiateProg`
- `WhyCannotInitiateProg`
- whether the character already has another personal project from the same project definition id

### Workers cannot join a labour role
Check:
- trait qualification
- minimum trait value
- qualification prog
- worker cap
- whether the named labour is part of the current phase

### Personal-project cancellation fails
Current behavior blocks cancellation when an unfinished active ongoing job depends on that exact active personal-project instance. If you have also configured `cancancel` / `whycancel`, those run only after this hard engine invariant.

### Material supply is choosing the "wrong" item
`project preview` and `project supply` use the requirement's inventory plan and then take the first feasible target it scouts.

Practical implication:
- if multiple eligible items are available, the chosen item is an implementation detail of the current plan and scout order

Use `project preview` before supplying if the exact item choice matters.

### A project exists but nobody is working on it
This is valid current behavior.

Remember:
- `project start` creates the active project
- `project join` assigns a labour role

The same applies after some phase transitions, especially for local projects, which clear active labour and require workers to join again.

### Queueing is not working
Check:
- the queued labour is still part of the active project's current phase
- you are idle; the queue does not activate while you already have a `CurrentProject`
- for local projects, you are in the same cell when the queue entry reaches the front
- the queued labour still has a free worker slot
- you still qualify for that labour

### Multiple requirements are behaving differently than expected
Current completion rules are:
- only mandatory labour requirements block phase completion
- only mandatory material requirements block phase completion
- optional labour and optional material requirements can still exist, but they do not stop the phase from advancing
