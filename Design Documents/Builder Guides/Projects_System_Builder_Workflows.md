---
slug: projects
category: Crafting and projects
summary: Build a town footbridge and a personal apprenticeship. Design paid work, training, quests and PC job performance with phases, labour and completion actions.
---

# FutureMUD Projects System Builder Workflows

The Wayfarer Inn needs its river crossing back. A smith can forge a batch of nails at the anvil this evening. Rebuilding the footbridge gives the town several days of work, an order for those nails, wages for the crew and a new route when the deck opens. The smith can take part without becoming a bridge carpenter. [Forge nails for the bridge crew](Crafting_System_Builder_Workflows.md#forge-nails-for-the-bridge-crew)

## Choose what the player is committing to

| Design question | Craft | Project |
| --- | --- | --- |
| What does a player do? | One maker works through a sequence in person. Tools, the properties of individual inputs, and interruptions can all matter. | Someone sponsors an undertaking. People can supply materials, take a shift, change roles or leave others to finish. A personal project can also represent one character’s sustained commitment. |
| What does time mean? | The maker sets aside a play session. A job measured in minutes, often ten or fewer, is a good fit. | Work accrues on scheduled ticks while the server is running, including while an assigned player is logged out. A stopped server earns no shifts. |
| What is the result? | The potter gets a mug whose glaze and quality reflect what went into it. | A bank becomes accessible, an apprentice gets skill practice, or a PC earns performance credit in their employment. The result can change the world without producing an item. |

Use the *opportunity cost* to decide close cases. A fighter can give up an ongoing conditioning bonus to take a paid bridge shift. A player who merely delivers nails can keep their conditioning assignment. If the drama is whether someone interrupts the smith at the anvil, put that scene in a craft. Ten minutes is a pacing guideline you choose.

## How a project lives in the world

A **template** describes the work and has an approved revision. Starting it creates an **active project** with its own ID, owner, phase, progress and reserve. Joining chooses a **labour role** on that instance. A character can own an apprenticeship and a bridge commission at once, but can work only one role at a time. Use `project view <template>` for the design, `project details <active-id>` for the actual site and `projects` for the character’s current assignment and impacts.

Each phase ends when its mandatory labour and materials are complete. Deliveries and labour can proceed in either order within a phase. If the deck crew must wait for the foundations, give them separate phases. A worker leaving does not undo completed work. With the default settings each real quarter hour of assigned work earns a quarter of an hour towards continuity. Progress towards a requirement also depends on the labour check and applicable modifiers. Twelve units of required progress do not promise completion twelve clock hours after the tender opens.

Choose **local** for a fixed worksite. Start it where the crew will actually stand. An ordinary room counts as one site. A route uses the immediate neighbourhood of the site’s layer and position. The engine currently clears a worker’s assignment if they are away at the next tick. Whether stepping away should end the assignment is under assessment. [Worksite assignment assessment](https://github.com/FutureMUD/FutureMUD/issues/903)

Choose **personal** when the assignment should follow its owner. Local crews clear at phase changes. A participating personal owner can carry on in the first eligible role with space in the new phase.

## Rebuild the Wayfarer footbridge

### Commission the crossing

Use the near bank as the worksite. Prepare a dedicated room overlay package containing the bridge deck and its connections to both banks. Review and approve it while leaving it unswapped. The work can then open an inspected crossing on the final shift. Keep unrelated room edits in a separate package because swapping activates every room overlay in the chosen package. [Build, review and swap room overlays](Room_Building_Builder_Guide.md)

Give the town a register on the near bank for the approved package ID and another for the commission. An unmarked room has no tender. A commission value of `-1` opens bidding, a positive value identifies the active crew, and `0` closes the tender. Use a fresh, uniquely identified package for this crossing, and keep it Current until the handover. From the **near bank**, enter:

```text
register location BridgePlan number
register location BridgeCommission number
register default location BridgePlan 0
register default location BridgeCommission 0
setregister location here BridgePlan <approved-bridge-package-id>
setregister location here BridgeCommission -1
```

Create the following dynamic FutureProgs with the names and parameter types shown. Compile the reason prog before the start check that calls it. [FutureProg editor and compilation](FutureProg_Builder_Guide.md) The reason for refusal does double duty as the start check, so a player who cannot start gets the same explanation the town uses to make the decision.

**`WhyWayfarerBridge(actor character) → text`**

```futureprog
var site = @actor.location
if (getregister(@site, "BridgePlan") <= 0)
  return "The bridge tender is posted on the near bank."
end if
if (getregister(@site, "BridgeCommission") != -1)
  return "The town is not taking bids for this crossing."
end if
return ""
```

**`CanStartWayfarerBridge(actor character) → boolean`**

```futureprog
return @WhyWayfarerBridge(@actor) == ""
```

**`ClaimWayfarerBridge(job project) → void`**

```futureprog
var site = @job.location
if (getregister(@site, "BridgeCommission") == -1)
  setregister @site "BridgeCommission" @job.id
end if
```

**`ReleaseWayfarerBridge(job project) → void`**

```futureprog
var site = @job.location
if (getregister(@site, "BridgeCommission") == @job.id)
  setregister @site "BridgeCommission" -1
end if
```

**`OpenWayfarerBridge(job project) → void`**

```futureprog
var site = @job.location
if (getregister(@site, "BridgeCommission") == @job.id)
  if (SwapOverlay(getregister(@site, "BridgePlan")))
    setregister @site "BridgeCommission" 0
    send(@job.owner, "The new crossing is open to travellers.")
  else
    send(@job.owner, "The crew has finished. The survey office must release the bridge plans.")
  end if
end if
```

Cancellation before handover reopens this crew’s tender. A completion action cannot veto phase completion. If the planned swap fails, the commission retains the crew ID and prevents a second crew taking the same contract. The survey office can reconcile both ends, use `room package swap <id>` on the correct approved package, and set `BridgeCommission` to `0` at the near bank once the crossing is open.

### Write the two phases

The example uses your world’s Carpentry skill, solid iron, a **Bridge Timber** tag for countable prepared beams and a **Nails** pile tag for wrought fasteners. In the smith’s recipe choose the tagged commodity output if the town orders by weight. If the smith makes 100 individual 20 g nails instead, give those items the Nails tag and use a `simple` requirement for 100 in phase two. An unworked iron pile should never fill a fastener order. [Counted and weighed nail recipes](Crafting_System_Builder_Workflows.md#forge-nails-for-the-bridge-crew)

```text
project edit new local
project set name Rebuild the Wayfarer Footbridge
project set tagline Restore the river crossing by the inn and earn a crew wage.
project set appear AlwaysTrue
project set can CanStartWayfarerBridge
project set why WhyWayfarerBridge
project set start ClaimWayfarerBridge
project set cancel ReleaseWayfarerBridge
project set phase 1 description Set sound timber footings on the near bank.
project set phase 1 labour add simple Groundcrew
project set phase 1 labour Groundcrew prog AlwaysTrue
project set phase 1 labour Groundcrew progress 12
project set phase 1 labour Groundcrew workers 3
project set phase 1 material add simple
project set phase 1 material Material name Timber
project set phase 1 material Timber tag <bridge-timber-tag>
project set phase 1 material Timber amount 6
project set phase add
project set phase 2 description Frame the deck, fasten the boards and open the span.
project set phase 2 labour add simple Decking
project set phase 2 labour Decking trait Carpentry
project set phase 2 labour Decking minskill 20
project set phase 2 labour Decking progress 16
project set phase 2 labour Decking workers 2
project set phase 2 labour add supervision Foreman
project set phase 2 labour Foreman trait Carpentry
project set phase 2 labour Foreman minskill 40
project set phase 2 labour Foreman multiplier 125%
project set phase 2 labour Foreman scaled
project set phase 2 material add commodity
project set phase 2 material Material name Nails
project set phase 2 material Nails material <iron-solid-id>
project set phase 2 material Nails tag <nails-tag>
project set phase 2 material Nails amount 2kg
project set phase 2 action add prog
project set phase 2 action Action name OpenCrossing
project set phase 2 action OpenCrossing prog OpenWayfarerBridge
project edit submit
```

New templates already have phase one. The ground crew has three places and accepts unskilled workers. The decking needs carpenters. The foreman takes a separate place and can increase the carpenters’ output up to the configured multiplier, scaled by the supervisor’s check. A foreman alone cannot lay any decking. A reviewer uses `project review <template-id>` and `accept edit` to make the revision Current. An active commission keeps the revision it started on.

### Run the paid shifts

At the near bank the sponsor starts the Current template. Substitute the ID from `projects` for `<bridge>` below. The money amounts are in the world’s own currency notation. For example, if a ground worker earns four coins an hour, each successful default tick reserves one coin for that worker. Three workers on the same tick cost three coins. Keep money for the six beams and later shifts as well.

```text
project start wayfarer.footbridge
projects
project pay <bridge> currency <town-currency>
project fund <bridge> <cash-budget>
project pay <bridge> labour Groundcrew <hourly-wage>
project pay <bridge> material Timber <price-per-beam>
project details <bridge>
```

A labourer uses `project join <bridge> Groundcrew`. A timber merchant brings beams within reach, uses `project preview <bridge> Timber` and then `project supply <bridge> Timber`. Supplying is independent of holding a crew place. The merchant receives the material payment immediately. Crew members accumulate wages and use `project claim` to collect them. An empty reserve stops paid work before the next unpaid contribution. Funding the site again gives workers a reason to return.

When phase two opens, the reserve carries over but the phase-one rates and assignments clear. Set `project pay <bridge> labour Decking <hourly-wage>` and, if wanted, a separate `Foreman` rate. Carpenters join Decking and the smith supplies Nails in this phase. Use `project queue join <bridge> Decking until completion` to wait for the role when eligible. A queue is consulted when the character is free. If a character is assigned to a standing personal regimen, they must leave it to take the shift.

To recruit a local NPC, create a `projectworker` AI, give it the town currency, an hourly floor at or below the advertised wage, and a path from its usual room to the bank. Use `ai show` to inspect the search toggle before changing it. Attach the AI to a present worker with `ai add WayfarerBridgeHand <nearby-npc>`. An unskilled hand can take a footing shift and still be ineligible for a decking shift. A qualified PC can take a full place held by an NPC. [Paid project worker settings](#paid-project-worker-ai)

```text
ai edit new projectworker WayfarerBridgeHand
ai set currency <town-currency>
ai set pay <minimum-hourly-wage>
ai set range 5
ai show
ai close
```

## A joiner’s apprenticeship

A bridge repair pays for labour and opens a crossing for everyone. The apprenticeship belongs to the joiner. Make it finite so it can lead somewhere. In the example a joiner who already knows basic Carpentry spends time practising mortise joints, then receives three improvement checks. These checks go to the project owner and use the world’s ordinary skill and advancement rules. Three checks need not yield three gains.

Create these two progs for enrolment. Change the skill and threshold together if the joiners’ guild uses a different entry standard.

**`CanBeginJoinery(actor character) → boolean`**

```futureprog
return GetTrait(@actor, ToTrait("Carpentry")) >= 10
```

**`WhyJoinery(actor character) → text`**

```futureprog
return "You need ten points of Carpentry before the joiner can take you on."
```

```text
project edit new personal
project set name Joiner's Apprenticeship
project set tagline Practise fitted joints under the town joiner's instruction.
project set appear AlwaysTrue
project set can CanBeginJoinery
project set why WhyJoinery
project set phase 1 description Lay out, cut and fit a set of practice joints.
project set phase 1 labour add simple Benchwork
project set phase 1 labour Benchwork trait Carpentry
project set phase 1 labour Benchwork prog CanBeginJoinery
project set phase 1 labour Benchwork progress 8
project set phase 1 action add skilluse
project set phase 1 action Action trait Carpentry
project set phase 1 action Action checks 3
project set phase 1 action Action difficulty Normal
project edit submit
```

Once approved, the apprentice uses `project start apprenticeship` followed by `project join <active-id> Benchwork`. They can break off for a bridge shift and resume their accumulated benchwork later. For an enrolment that should award a permanent guild qualification once only, have the finish hook record the qualification and have the start gate check it. Otherwise a completed apprenticeship can be started again.


## Four patterns from the original LabMUD projects

These examples come from the original LabMUD catalogue. The project numbers identify those records in that world. When building your own version, select your world’s traits, tags, roles and permissions.

### Give combatants a reason to keep a personal assignment

**Personal Improvement (#1)** offered six alternative endless regimens. Pump Iron improved Strength, Cardio improved Constitution, Yoga improved Dexterity, Studying improved Intelligence, Affirmation improved Willpower, and Mindfulness improved Perception. Each granted **+2 after twelve hours** in its role. The player chose one. A fighter doing bridge work instead gives up the active bonus. Returning to the regimen starts earning its qualifying hours again.

**Rest and Recuperation (#3)** offered another choice. After **three hours** on Rest, it added 0.5 to healing checks, doubled the healing rate and multiplied infection chance by zero. Those were deliberate, generous settings in this world. For a campaign where infection remains a threat even during rest, choose a multiplier between zero and one. This is a change to the chance of infection while resting. It does not cure an existing infection.

To build the first regimen, start a `personal` template, give it its title, tagline and three catalogue progs as above, then add this labour to its initial phase. Leave it mandatory and endless so the regimen stays available. No finish action is needed.

```text
project set phase 1 labour add endless "Pump Iron"
project set phase 1 labour "Pump Iron" prog AlwaysTrue
project set phase 1 labour "Pump Iron" impact add trait Conditioning
project set phase 1 labour "Pump Iron" impact Conditioning trait Strength
project set phase 1 labour "Pump Iron" impact Conditioning bonus 2
project set phase 1 labour "Pump Iron" impact Conditioning hours 12
project set phase 1 labour "Pump Iron" impact Conditioning scope labour
```

Add other regimens as sibling roles in the same phase. Use `scope labour` when a change from Cardio to Yoga should restart the wait. `scope project` reads the project continuity counter. A manual role change currently passes through a leave and resets that counter too. Keep a regimen on a single role if its waiting period matters. The worker still receives only the impacts on the role they are doing now. For the recovery project, give its endless Rest role one `healing` impact and configure its `hours`, `bonus`, `multiplier` and `infection`. For the original values, use `hours 3`, `bonus 0.5`, `multiplier 200%` and `infection 0%`. Make these effects visible in the role descriptions so combatants can weigh the lost benefit against a crew wage.

### Dig a room that can become the next worksite

LabMUD’s **Create New Room (#2)** built a fresh underground chamber. The footbridge opens plans that staff have already laid out. This project let eligible players decide where the next room would be from inside the game.

| Stage | What happened in LabMUD | What the design achieves |
| --- | --- | --- |
| Commission | The character needed an eligible Rook or King role, a permitted source room and a selected cardinal direction. The start check refused an occupied direction or a site already under construction. The start hook marked the site busy and placed a directional work marker. | No competing projects promising the same room. Someone arriving later can see what the crew is doing. |
| Excavation | Up to ten diggers shared 2,880 units of work. A Bishop, Rook or King could supervise and the project required two shovels. The phase action changed the visible marker for the fit-out. | A material requirement **consumes** the shovels. If a shovel should come back after each session, use a craft with the shovel as a tool for that portion of the work. |
| Fit-out | General labour, structural fit-out and foreman roles shared the phase. The site took 200 fasteners, 30 pieces of shoring timber and two roof beams. | Counts work well when the crew needs particular pieces. General labour had two places and structural fit-out had four. The separately qualified foreman supervised them. |
| Handover | The final action revised an overlay package, made a dark indoor chamber with timber supports, linked it in the selected direction, allowed a door, marked the new room as eligible for later expansion, and approved the package. | The result can itself become a worksite. Set permissions, direction and room presentation as part of the handover. |

Two details matter when adapting the old commission. Its direction command wrote to the character, but its construction progs read from the room. It also had no cancellation hook to clear the marker and busy flag. For a new design, copy the authorised direction to the active project at the start and read that same value in both phase actions. Reserve the site against that project’s ID. On cancellation remove its work marker and release only its reservation. If a draft room already exists, leave it identifiable for the surveyor. Approval and activation are distinct operations in the current engine. For a route to open immediately, check that the link and approval succeeded, then call `SwapOverlay`. For an inspected route, leave the package for the surveyor to review and swap. The north-drift example gives the register declarations, eligibility and refusal progs, completion code and recovery procedure. [Extend the north drift](Primary_Production_and_Agriculture_Builder_Guide.md#extend-the-north-drift)

### Make a personal project one stage of a quest

In **Study Spark (#4)** the character first obtained and ate the Crystal. Its SparkFood component’s OnEat prog set `canstudyspark` if the character had not already completed the study. Both catalogue visibility and permission to start read that flag. The project then took three phases. The numbers below are work requirements, so each phase’s wall-clock duration depends on the participant’s progress.

| Phase | Required work | Cost while assigned |
| --- | ---: | --- |
| Study | 48 | −3 Intelligence |
| Commune | 48 | −4 Willpower |
| Subdue | 24 | −4 Intelligence and −3 Strength |

The last phase action cleared `canstudyspark` and set `studiedspark`. The Technomancy invocation prog read `studiedspark`. Receiving the item, eating it, starting the project and earning the power were four distinct moments. A character who switched to the bridge during Commune gave up their study time and its penalty together. Completing the quest left the unlock in place after the project disappeared.

To build this pattern, declare two boolean character registers, both defaulting to false. Have your edible item’s consumption hook set the first on the **eater**, unless the second is already true. Spawning, picking up or giving away the item must leave those flags alone. Add three phases with one qualified `simple` labour each (a new project already has its first phase, so add two). Set each labour’s `progress` from the table. On each role add a `trait` impact for each affected attribute with a negative `bonus` and zero minimum `hours`.

```text
register character CanStudySpark boolean
register default character CanStudySpark false
register character StudiedSpark boolean
register default character StudiedSpark false
```

**`SparkStudyAvailable(actor character) → boolean`**

```futureprog
return getregister(@actor, "CanStudySpark") and not(getregister(@actor, "StudiedSpark"))
```

**`CompleteSparkStudy(job project) → void`**

```futureprog
var student = @job.owner
setregister @student "StudiedSpark" true
setregister @student "CanStudySpark" false
```

Use `SparkStudyAvailable` for both `project set appear` and `project set can`. Supply a one-character text `why` prog explaining the prerequisite. Give phase three a `prog` completion action and bind `CompleteSparkStudy`. Have the magic eligibility prog read `StudiedSpark`. Cancellation can leave the first flag set if the student should be able to enrol again. Putting the permanent flag in the start hook would let a character unlock the power just by enrolling.

### Tie an ongoing PC job to effort

LabMUD used projects **#5–11** for programming, chemistry, machining, electronics, computers, mechanics and logistics labour. These were personal performance projects supplied through job listings. Each had an endless diligent role giving `variable / 10` effort per tick and an overtime role giving `variable / 5` at the cost of one point of Constitution while working overtime. A programmer could stay employed while doing something else, but their bridge work would not earn programmer effort. The Data Analyst and Robot Programmer listings illustrate the other half of the design: weekly base pay plus a bonus capped at an equal amount when effort reached 1,000.

Here is how to give a PC the same choice as a joiner employed by the inn:

1. Make a `personal` project named **Joinery Shift**. Set `appear` to `AlwaysFalse` so it is obtained through employment. Set `can` to `AlwaysTrue` and give `why` a one-character text prog explaining that the inn assigns this work. Toggle `project set jobs` on. The jobs flag admits it to the employer’s template list independently of public catalogue visibility.
2. Give phase one the two **mandatory endless** roles below. Add a description to each that states the benefit and cost. The `job` impact turns work into performance for the job that owns this exact active project. The expression’s `variable` is the worker’s role trait. Overtime below uses a one-point Constitution penalty, which you can change to fit the setting.
3. Submit and approve the template. At the economic zone’s job market, create the **Inn Joiner** job listing, or edit it before employing anyone. Use `job set projects` to check availability and `job set project Joinery Shift` to attach it. Choose a pay currency and interval with `job set currency <currency>` and `job set interval <interval>`, then use `job set easypay "<base pay>" "<maximum bonus>"` for the capped bonus pattern. Fund the listing with `job deposit <amount>` and use `job ready` when it is ready for applicants.
4. A PC uses `jobs`, `job preview <listing>` and `job apply <listing>` at the job market. Acceptance creates their own active Joinery Shift. In `projects` find that ID and use `project join <active-id> Diligent` or `Overtime`. Payroll uses the job’s effort balance; performance can decay and is reset when pay is calculated. Collect job wages with `job payday`. Reserve `project pay` and `project claim` for a commission paying directly for the shifts.
5. The employee can switch assignments without resigning. The employer cannot change the listing’s project while it has employees. Finish the employment to retire its personal project. Use a distinct template for each independently held job if a character may hold several.

```text
project set phase 1 labour add endless Diligent
project set phase 1 labour Diligent trait Carpentry
project set phase 1 labour Diligent impact add job Performance
project set phase 1 labour Diligent impact Performance expression variable / 10
project set phase 1 labour add endless Overtime
project set phase 1 labour Overtime trait Carpentry
project set phase 1 labour Overtime impact add job Performance
project set phase 1 labour Overtime impact Performance expression variable / 5
project set phase 1 labour Overtime impact add trait Fatigue
project set phase 1 labour Overtime impact Fatigue trait Constitution
project set phase 1 labour Overtime impact Fatigue bonus -1
```

A PC performance project fits a general employment contract with a base salary and effort bonus. A `projectworker` NPC seeks paid places on local commissions. For an NPC who actually orders timber, runs an inn or delivers goods for an employer, use an employment host with the relevant tasks. Farm labour similarly uses local projects created through `field start` so completion can update the right field. [Employment hosts](Employment_Hosts_Builder_Guide.md) · [Agriculture](Agriculture_Builder_Workflows.md)


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
- `project queue <project> [labour]` (compatibility alias for `join ... once`)
- `project queue join <active-project> [labour] [once|for <hours>|until phase|until completion]`
- `project queue start <template> [labour] [once|for <hours>|until phase|until completion]`
- `project queue mode <index> once|for <hours>|until phase|until completion`
- `project queue labour <index> <labour|automatic>`
- `project queue move <index> <new-index>`
- `project queue remove <index>`
- `project queue clear`
- `project queue loop`

Current behavior:
- `join` entries target an existing active project; `start` entries store the project definition id and use the then-current approved revision only when they activate
- adding a start entry requires current catalogue visibility but deliberately does not require initiation eligibility yet
- blocked entries remain queued and show statuses such as `Waiting For Initiation`, `Waiting For Labour Selection`, `Waiting For Slot`, `Waiting For Qualification`, `Waiting For Location`, or `Waiting For Funding`; later ready entries are still considered once per activation pass
- automatic labour selection only proceeds when exactly one qualified role can be joined; otherwise set a labour preference explicitly
- labour preferences are limited to 100 characters by their storage column; adding or editing a longer preference gives an error and leaves the queue unchanged
- duration entries count successfully funded labour time only; phase and completion entries retain their linked active instance across phase changes
- a player queue entry can become `Ready` when the only occupied slot is held by an NPC who can be displaced
- stale join entries are removed automatically when their project disappears
- cancelling an active project satisfies the current queue cycle, including `until completion`

Looping is intentionally opt-in and is rejected while any `once` entry remains. Completed repeatable entries move to the tail; a looping start entry launches a new live project on its next cycle rather than reusing an unrelated project.

### Project labour contribution merit
Builders can create the `Project Labour Contribution` character merit. Its `prog` setting accepts only a numeric FutureProg with the signature `number(character, project)`. The resulting number multiplies project progress after ordinary labour and supervision calculations. More than one applicable merit stacks multiplicatively. This changes neither payment, time worked, impacts nor skill-check cadence.

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
- `project review list mine`

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

When both are set, the character must satisfy both. A trait labour also needs a positive project-labour check target at its configured difficulty. Without either form of qualification, submission is blocked.

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

Set a multiplier above 100% for a foreman who should improve a crew’s output.

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

Impact hours control when the effect begins. `scope labour` keys off continuity on the exact current labour role; `scope project` reads continuity on the active project. A manual `project join` on a different role leaves the old one first and resets both counters. Trait, cap and healing impacts check their hour threshold. The `job` impact currently credits effort on every successful work tick. [Impact hour tracking](https://github.com/FutureMUD/FutureMUD/issues/904)

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
| `skilluse` | Grants the project owner free checks against a trait when the phase completes | `trait`, `checks`, `difficulty` |
| `agriculture` | Applies a field operation linked through `AgricultureProjectContext` | normally created through `field start` |
| `commodityoutput` | Creates a commodity pile in the active project location or owner fallback location | `material`, `weight`, `tag`, `indirect`, `echo`, `characteristic` |
| `resourcediscovery` | Reveals a configured visible resource marker in the active local-project location, with optional required location tag and duplicate prevention | `locationtag`, `item`, `duplicate`, `echo`, `alreadyecho`, `failureecho` |

When combining actions on a phase:
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
- `ApproveOverlay(package, builder, comment)` to mark a package approved; `SwapOverlay(package)` activates its rooms and exits

Recommended workflow:
1. Clone or author a local project that represents the work.
2. Add commodity outputs and byproducts first, so material rewards are independent of world topology.
3. Add a final `prog` action with a higher sort order.
4. In the prog, resolve the builder-chosen overlay package, zone, template room, direction, and approval policy explicitly.
5. For a reviewed expansion, leave the package under design, then approve and swap it after inspection. For immediate opening, check the result of both operations before announcing success. [Extend the north drift](Primary_Production_and_Agriculture_Builder_Guide.md#extend-the-north-drift)

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
Check whether an ongoing job owns that employee’s project. It belongs to the employment until the job ends, even if the project’s usual cancellation prog allows the owner to cancel.

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
- for local projects, you are in the worksite room (within reach of the site on a route)
- the queued labour has a free place or a place the player can take over from an NPC
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
