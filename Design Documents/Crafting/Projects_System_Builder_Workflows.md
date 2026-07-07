# FutureMUD Projects System Builder Workflows

## Purpose
This document explains how builders and admins currently create, revise, approve, and operate projects through the in-game command surface.

This is a builder-facing guide. It describes the command workflows that exist today, including current limitations and implementation quirks.

Agriculture uses this project system for field labour. See [Agriculture Builder Workflows](../Agriculture/Agriculture_Builder_Workflows.md) for the field-facing commands and the extra agriculture project context written when `field start` creates a local project.

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
- a qualified player can join a full labour role by displacing an NPC who is currently working that same role
- joining a new project labour automatically leaves whatever `CurrentProject` labour you were already working on

### `project quit <project>`
Use `project quit <project>` to stop working on that active project.

This does not cancel the active project. It only removes your current labour assignment from it.
If your queue has a ready next assignment, the engine immediately tries to join it after you quit.

### `project details <project>`
Use `project details <project>` to inspect the live active project in its current phase.

This is the command to use when you need to see:
- current workers
- whether a full NPC-worked labour is satisfied but joinable by the viewer
- mandatory status
- progress already accrued
- current-phase material requirements
- payment reserve and current-phase labour/material payment rates

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
4. pays any funded per-unit material rate immediately
5. adds the supplied quantity to the active project

### `project fund <project> <amount>`
Use `project fund` to move physical cash from your inventory into an active project's virtual payment reserve.

Current behavior:
- only the owner or an administrator can fund a project
- the command parses the amount in the project's payment currency, falling back to the actor's currency before any payment currency has been set
- funded money is no longer a physical pile; it is held as virtual project reserve cash and written to the shared virtual-cash ledger

### `project withdraw <project> <amount>`
Use `project withdraw` to take unspent virtual reserve cash back as physical currency.

Current behavior:
- the same owner-or-admin project-payment managers who can fund a project can withdraw from it
- withdrawals only draw from the active project's virtual reserve
- outstanding worker payables are not reversed by withdrawing later unspent funds

### `project pay <project>`
Use `project pay` with no extra arguments to review the payment currency, current reserve, and current-phase payment rates.

Use these forms to configure current-phase requirement rates:
- `project pay <project> currency <currency>`
- `project pay <project> labour <requirement> <amount|none>`
- `project pay <project> material <requirement> <amount|none>`

Labour rates are hourly. With the default 15-minute project tick, a worker earns one quarter of the configured hourly rate per tick. Labour pay becomes an owed project payment that must later be claimed. Material rates are per contributed unit and are paid immediately when `project supply` succeeds.

Increasing labour pay is immediate. Lowering or clearing a labour pay rate is also immediate if nobody is currently working that labour, but if active workers are present the command warns that they will be removed and requires `ACCEPT` before applying the lower rate.

Paid work is pay-before-progress. If the reserve cannot cover a paid labour tick, that worker is removed from the active project before progress is recorded. If the reserve cannot cover a paid material contribution, `project supply` fails before consuming the item. Clear the rate with `none` if the requirement should accept unpaid volunteer contributions.

Changing the payment currency requires an empty project reserve so existing virtual balances do not become ambiguous.

### `project claim [account]`
Use `project claim` to collect owed project payments.

Current behavior:
- with no account argument, project payments are claimed as physical cash
- with an account argument, only payments in that account's currency are deposited into the selected active character-owned bank account
- payables remain owed until the claim succeeds

### `project cancel <project>`
Use `project cancel` to destroy an active project.

Current rules:
- a personal project cannot be cancelled while an unfinished active job still depends on that exact active project instance
- administrators bypass builder-authored cancellation progs, but not the active-job invariant
- if no custom cancel progs are configured, both personal and local projects fall back to owner-only cancellation
- unspent project reserve cash is returned to the project owner as a claimable project payment

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
- a player queue entry can become `Ready` when the only occupied slot is held by an NPC who can be displaced
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
- `project set phase <phase> labour <labour> scaled`

When `scaled` is enabled and the supervision labour has a required trait, the best active supervisor's project-labour check target scales the multiplier between `100%` and the configured multiplier. This lets a skilled foreman improve lower-skilled labour without making an unskilled supervisor grant the full bonus.

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
| `commoditytag` | Counts commodity mass where the material has a required material tag, with optional pile tag, characteristics, and quality | `material`, `materialtag`, `tag`, `piletag`, `amount`, `quality`, `characteristic` |

Builder notes:
- `simple` requires a tag before submit
- `commodity` requires a material before submit
- `commoditytag` requires a material tag before submit
- `commodity` `amount` uses the game's mass parser, not a raw item count
- `commoditytag` `amount` also uses the game's mass parser
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
| `agriculture` | Applies a field operation linked through `AgricultureProjectContext` | normally created through `field start` |
| `commodityoutput` | Creates a commodity pile in the active project location or owner fallback location | `material`, `weight`, `tag`, `indirect`, `echo`, `characteristic` |
| `resourcediscovery` | Reveals a configured visible resource marker in the active local-project location, with optional required location tag and duplicate prevention | `locationtag`, `item`, `duplicate`, `echo`, `alreadyecho`, `failureecho` |

Current implementation warning:
- actions execute in ascending `sort` order
- ties fall back to stable id order
- `commodityoutput` requires a solid material and positive mass before submit; optional characteristics are fixed output values, not wildcard input filters
- `resourcediscovery` creates one configured item prototype in the project cell; it is for visible marker props, not for creating rooms or exits

### Optional site-creation completion progs
Stock primary-production projects stop at resource markers and commodity outputs. Builders can still create site-expansion projects, such as opening a mine adit or extending a quarry, by adding a `prog` completion action to a local project.

The completion prog must accept one `project` parameter. It can read:
- `project.location` for the current local project cell
- `project.owner` for the initiating character
- `project.workers` for the characters currently assigned when the phase completes

Relevant room-building FutureProg helpers include:
- `CreateOverlay(builder, name)` to create a new editable overlay package
- `ReviseOverlay(package, builder)` to open a revision of an existing current overlay package
- `CreateCell(package, zone, template)` or `CreateRoom(package, zone, template)` to create the new room
- `NameCell(cell, package, name)` and `DescribeCell(cell, package, description)` to set presentation
- `SetIndoorsNoLight(cell, package)` and `SetTerrain(cell, package, terrain)` to configure mine-like interiors
- `LinkCells(origin, destination, package, direction)` to connect the project location to the new cell
- `ApproveOverlay(package, builder, comment)` to approve the package when the workflow should publish immediately

Recommended workflow:
1. Clone or author a local project that represents the work.
2. Add commodity outputs and byproducts first, so material rewards are independent of world topology.
3. Add a final `prog` action with a higher sort order.
4. In the prog, resolve the builder-chosen overlay package, zone, template room, direction, and approval policy explicitly.
5. Leave the overlay under design if the game requires normal builder review before topology changes enter play.

This is intentionally not seeded as a generic stock project. Overlay package names, template rooms, exit directions, terrain choices, and approval policy are world-specific, and a setting-neutral seeder cannot infer them safely.

## Compact Type Keyword Reference
### Project families
| Family | Keywords |
| --- | --- |
| Project | `personal`, `local` |
| Labour | `simple`, `endless`, `supervision` |
| Material | `simple`, `commodity`, `commoditytag` |
| Action | `prog`, `skilluse`, `agriculture`, `commodityoutput`, `resourcediscovery` |
| Impact | `trait`, `healing`, `job`, `cap` |

## Paid Project Worker AI
Use the `projectworker` AI type for NPCs that should look for paid active project labour rather than ordinary employment host tasks.

Important settings:
- `pay <amount>` sets the minimum hourly project pay the worker will accept
- `currency <currency>` sets the currency used for parsing and comparing that minimum
- `range <cells>` sets the maximum path distance to a project
- `search` toggles automatic paid-project search
- `claim` toggles autonomous claiming of project payables
- `deposit` toggles bank-account deposits instead of cash claims where possible
- `account <account type|none>` optionally lets the AI open a character-owned account type for deposits
- `cadence <timespan>` controls how often an idle worker retries the search after finding no qualifying project

The AI only joins current-phase local project labour it qualifies for, where there is a free worker slot, the project reserve can cover at least one tick of pay, and the path is reachable. NPC workers do not displace other NPCs or players. If several projects qualify, it chooses the highest global-base-currency equivalent hourly rate.

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
- if the labour is full, whether at least one current worker is an NPC; qualified players can displace NPC workers, but NPCs cannot displace each other

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

### Paid project labour is not paying
Check:
- the active project has a positive payment rate for that current-phase requirement
- the active project has a payment currency and a funded reserve in that currency
- the reserve can cover the tick payment or material payment being attempted
- labour payments have been claimed with `project claim`; they are owed first, not paid as cash during the work tick

If the project runs out of reserve cash:
- paid labourers are removed from work before the next unpaid tick can create progress
- paid material supply is rejected before the material is consumed
- the project remains visible as an active project if its completion requirements are still incomplete
- the owner can fund it, lower/clear rates, or cancel it

### A project-worker NPC will not pick up a project
Check:
- the project is a live local project with a reachable location
- the current-phase labour still has a free worker slot
- the NPC satisfies the labour qualification rules
- the configured hourly pay meets the AI's minimum after currency conversion
- the project reserve can cover at least one tick of pay
- the AI's `search` toggle is enabled and its search cooldown has expired

### Multiple requirements are behaving differently than expected
Current completion rules are:
- only mandatory labour requirements block phase completion
- only mandatory material requirements block phase completion
- optional labour and optional material requirements can still exist, but they do not stop the phase from advancing
