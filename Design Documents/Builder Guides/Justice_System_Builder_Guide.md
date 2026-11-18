---
slug: justice-system
category: Economy
summary: Build a legal authority for Alderbridge, author laws and penalties, staff patrols and courts, and take a witnessed case through arrest, bail, representation, trial and release.
---

# Building the justice system

A **legal authority** is a jurisdiction with its own enforcement zones, currency, laws, custody rooms and court. A **legal class** tells it which group a character belongs to for the purposes of the law. An **enforcement authority** gives qualifying characters powers such as arrest or judgement. A **law** defines an offence and how to respond to it. A **crime** is a case recorded against someone under that law. A witness, a report, an arrest and a conviction are separate events.

This walkthrough adds an Alderbridge Bench to the town in the clan guide. The town watch takes a witnessed trespass case into custody. A magistrate hears the first trial. You can then run a second case through a staffed NPC court. Use a test zone, ordinary test characters and a known currency. Replace placeholders with actual IDs or names. Amounts such as `5cr` assume a currency whose parser accepts that format. [Clans and town government](Clan_Builder_Guide.md)

## Create the jurisdiction and its legal class

Create a Boolean FutureProg named `AlderEveryone` with one Character parameter named `ch`. Give it the body `return true`. For each new prog in this guide, use `prog edit new <name>`, set its return type with `prog set return <type>`, add parameters with `prog set parameter add <name> <type>`, then enter the body with `prog set text` and submit with `@` on its own line. Check the compile result. Keep progs that inspect live state non-static. [FutureProg authoring](FutureProg_Builder_Guide.md)

As an administrator, create a jurisdiction and its fallback class. Once the class is present, add the zone containing the test rooms.

```text
legalauthority edit new "Alderbridge Bench" <currency-id>
legalauthority set class add General AlderEveryone
legalauthority set zone <world-zone-id>
legalauthority classes "Alderbridge Bench"
legalauthority show "Alderbridge Bench"
```

The zone here is the **world zone** that contains the rooms. Alderbridge can use the same currency for taxes and fines, but its economic zone and clan are separate records. Check all authorities that share a room's zone before adding a new one. Each authority evaluates its own laws in its enforcement area.

Start with one class for residents, officers and visitors. If your setting later distinguishes groups such as citizens, visitors or clergy, author Boolean Character membership progs and verify which class the authority resolves for representative characters, including staff, NPCs and outsiders. Keep a catch-all because the engine expects a match. Set intended priorities with `legalauthority set class <class> priority <number>` and test overlapping predicates before relying on the order. You can use disjoint predicates to make the result independent of order. The class editor's `detain` switch permits detention for unpaid fines. Leave that off for this first trial.

## Define who can enforce and judge

Create a local `Magistrate` appointment in the town clan and grant it to the character who will hear the trial. As an administrator with that member present, use the following commands.

```text
clan edit alder
clan create appointment Magistrate Mag
clan appoint <magistrate-character> alder Magistrate
```

For the watch, invite the test constable to Alderbridge Militia and make sure their membership is active. You can later narrow eligibility to a specific militia office if the militia includes people who should never police the town.

Make two non-static Boolean progs with one Character parameter `ch`. Replace each angle-bracketed clan ID with the actual number.

| Prog | Body |
| --- | --- |
| `AlderWatchFilter` | `return IsClanMember(@ch, ToClan(<militia-clan-id>))` |
| `AlderMagistrateFilter` | `return IsClanMember(@ch, ToClan(<town-clan-id>), ToAppointment(ToClan(<town-clan-id>), "Magistrate"))` |

```text
legalauthority edit "Alderbridge Bench"
legalauthority set enforcement add Watch
legalauthority set enforcement Watch filter AlderWatchFilter
legalauthority set enforcement Watch accuse General
legalauthority set enforcement Watch arrest General
legalauthority set enforcement add Bench
legalauthority set enforcement Bench filter AlderMagistrateFilter
legalauthority set enforcement Bench accuse General
legalauthority set enforcement Bench convict
legalauthority enforcements "Alderbridge Bench"
```

The Bench needs both the power to judge and an accusable class covering the defendant. The Watch needs permission to arrest that class and the law itself must permit arrest. Use `legalauthority set enforcement <role> show` to check the flags and class lists. The `forgive` switch enables a separate power to remove active charges the enforcer could accuse. The `also` option includes another enforcement authority's powers. Keep the two filters disjoint for the test so the same character cannot match both roles.

Try `legalstatus "Alderbridge Bench"` as the constable, the magistrate and an ordinary resident. Run gameplay tests as those characters without an administrator override.

## Lay out the watch house, prison and court

Build connected rooms with ordinary exits for the first version. Put the public desk outside the locked remand cells. Put stores and holding rooms behind staff-controlled doors, and provide reachable paths to court and the release gate. Check the exits from both sides. Before adding locked doors to a test patrol, establish that it can complete the route, then verify the actual guards can unlock or pass each door.

```text
legalauthority set prepare <armoury-room-id>
legalauthority set marshalling <watch-yard-room-id>
legalauthority set stow <watch-barracks-room-id>
legalauthority set prison <watch-house-room-id>
legalauthority set cell <remand-cell-a-id>
legalauthority set cell <remand-cell-b-id>
legalauthority set belongings <secure-stores-room-id>
legalauthority set court <courtroom-id>
legalauthority set release <public-release-room-id>
legalauthority set jailentry <jail-entry-room-id>
legalauthority set jail <jail-entry-room-id>
legalauthority set jail <custodial-wing-room-id>
legalauthority status "Alderbridge Bench"
```

| Role | What to build and test |
| --- | --- |
| `prison` | The public prison location handles reporting, fine payment, bail and custody intake. The current sentencing transfer also lands here before any authored onward move. |
| `cell` | These are **remand** rooms. Intake selects a registered cell and records that the prisoner is awaiting sentencing. `cell` toggles each room, so inspect the list before repeating it. |
| `belongings` | Intake stores confiscated belongings as a character-specific bundle. Use a secure staff-only room and check the return path with a test item and coins. |
| `court` and `release` | The court hosts trials. The release location is the public destination when the authority releases someone. Provide a usable exit from it. |
| `jailentry` and `jail` | Define the entry and rooms of the sentenced jail. Keep them distinct from remand. To move a sentenced prisoner from the default transfer at `prison` into a separate wing, provide a guarded escort or an `imprisonedprog` appropriate to your layout. Check the resulting location and sentence status on a test prisoner. |

The authority also has `heldprog`, `imprisonedprog` and `releasedprog` hooks for Void progs taking the prisoner Character. Use them for in-game notices or an authored handoff. Test each independently before adding side effects. Provide staffed access to secure doors, drinking water and a way to ask for a hearing. A room role does not provision locks or guards.

## Write and inspect a law

Use trespass at the Market Workshop from the town guide. Put the workshop in the world zone above. Confirm it is part of a property, then enable the property editor's criminal-code switch if required. Use an owner or other authorised character as a control and a visiting character with no property access as the test entrant.

```text
property edit "Market Workshop"
property set lawful
property show "Market Workshop"
legalauthority edit "Alderbridge Bench"
legalauthority set law add "Workshop Trespass" Trespassing
legalauthority set law "Workshop Trespass" show
```

A fresh law starts with no punishment and with automatic application, arrest and bail disabled. The title is the law's displayed name. The crime type is the engine event it matches. This example applies to qualifying trespass in the whole jurisdiction, including other properties with criminal-code enforcement enabled. Use a policy filter if only one property should count. The automatic context for trespass includes the property and room IDs. `law add <name>` without the type lists supported crime types.

Check the law's offender list first. If `General` is absent, use `legalauthority set law "Workshop Trespass" offender General` to add it. This toggles a class. New laws may inherit class lists from the game's defaults. For an offence with a character victim, confirm the victim list too. The entry-based trespass hook has no character victim.

```text
legalauthority set law "Workshop Trespass" punishment type fine
legalauthority set law "Workshop Trespass" punishment fine 5cr
legalauthority set law "Workshop Trespass" punishment maxfine 5cr
legalauthority set law "Workshop Trespass" strategy ArrestAndDetain
legalauthority set law "Workshop Trespass" arrest
legalauthority set law "Workshop Trespass" bail
legalauthority set law "Workshop Trespass" period 7 days
legalauthority set law "Workshop Trespass" priority 20
legalauthority set law "Workshop Trespass" auto
legalauthority set law "Workshop Trespass" show
legalauthority laws "Alderbridge Bench" type Trespassing
```

Leave repeat suppression enabled for this example. `repeats` changes whether repeated automatic incidents in a short window can create new charges. The law's active period governs when a case can go cold. The case record can outlast its usefulness for a patrol. To revise the policy, open the authority and edit the law by ID or quoted name. Check all current class lists, flags and punishment settings before changing them. Review existing charges before deleting a law or changing its crime type.

| For this policy | Configure |
| --- | --- |
| An offence adjudicated by roleplay | Leave `auto` off. An authorised officer uses `accuse <target> <law>`, or `accuse <jurisdiction> <target> <law>` when needed. The accusation is immediately known. |
| An automatically recognised offence with an exception | Bind a Boolean `prog <prog>` to the law. The shortest signature takes offender and victim as Characters. Victim may be null. Extended signatures expose the item or law ID and context. For example, a Boolean prog taking `(character offender, character victim, text crime, text context)` receives the crime name and automatic context. Match the signature exactly when binding it. |
| A warning or a fine without pursuit | Use a policy such as `NoActiveEnforcement`, and inspect the arrest and bail switches separately. Setting the punishment alone does not authorise a patrol to arrest. |
| Prison or a good behaviour bond | Choose `punishment type jail` or `punishment type bond`. On a test jail law, set `punishment maximum 1 day` and `punishment minimum 1 day` for a fixed sentence, and review it with `show`. Use `punishment length <time>` for a bond. |
| Different or combined sentences | A `hierarchy` can choose a penalty by prog. A `multi` strategy can combine penalties. Inspect the chosen strategy's options and test a complete verdict and release for each policy you enable. |

A type alone does not create a coded trigger. Some types have automatic action hooks, others need an accusation or scripted witnessing. For scripting, the runtime reference shows `reportcrime(law, criminal, witness, victim, item, context)`. A distinct present witness must see the accused and the law must apply. [Automatic offences and witnessed-crime progs](../Economy/Law_System_Runtime.md)

## Staff a patrol and report the case

Give a loaded militia NPC **Enforcer** AI and put them on duty. Use an eligible character matching the Watch filter. Run the final command from a staff character who can see the NPC.

```text
ai edit new enforcer AlderWatchAI
ai close
ai add AlderWatchAI <constable-npc>
force <constable-npc> enforcer "Alderbridge Bench"
showenforcers "Alderbridge Bench"
```

The NPC's `enforcer` command is a toggle. A successful filter match gives them their Watch powers. On-duty mode and the AI make them eligible for dispatch. An eligible PC can use law enforcement commands, while the automatic patrol pool takes NPCs. For future spawns, attach the AI to the NPC template and include a suitable on-duty setup in the deployment workflow. [NPC builder guide](NPC_Builder_Guide.md)

Build a **Workshop Watch** post. Stand at the workshop entrance or other room where the patrol should see the test defendant and add that room as its first node. Start the constable there for the first test.

```text
legalauthority set patrol add Workshop Watch
legalauthority set patrol "Workshop Watch" strategy StationEnforcer
legalauthority set patrol "Workshop Watch" node
legalauthority set patrol "Workshop Watch" numbers Watch 1
legalauthority set patrol "Workshop Watch" linger major 15 minutes
legalauthority set patrol "Workshop Watch" ready
showpatrols "Alderbridge Bench"
```

New routes allow all times of day. To limit coverage, use `time <times-of-day>`; inspect `legalauthority patrols "Alderbridge Bench"` before opening the route. Route `priority` controls allocation when routes compete for enforcers. Reserve distinct staff for court jobs if the Watch post must stay staffed. A ready route can still wait for a suitable enforcer or a matching event.

| Strategy | What to configure |
| --- | --- |
| `ArmedPatrol` | Add multiple road or market nodes in the order to visit. Supply weapons that match the patrol equipment plan and accessible keys. Verify the route starts and guards can walk the circuit. |
| `StationEnforcer` | Its first node is a fixed post. It is suitable for proving basic reporting and custody with one NPC. |
| `DoorDuties` | Make the first node the secure door post. Give the NPC Doorguard AI and usable door keys, either already held or collectible from preparation. `config access enforcers` can restrict access to the jurisdiction's enforcers. Walk the route as both a guard and a visitor. |
| `ReactivePatrol` | Responds to recent reported violent incidents in coverage. Use `config radius <rooms>`, `window <timespan>` and `duration <timespan>` to tune dispatch. A quiet trespass is a poor dispatch test for this route. |
| `InvestigationPatrol` | Responds to eligible known cases in reach. Use `config radius <rooms>`, `reliability <0.0-1.0>` and `search <timespan>`. Check case evidence before and after the investigation. |
| `Judge`, `Prosecutor`, `Sheriff` | Station the court staff in the courtroom. The sheriff can arrange pickup from remand when a hearing is due. See the NPC hearing setup below. |

1. Put a witness where they can see the action. Enter the workshop as an authorised character first. Then use an unauthorised test character to walk in voluntarily. `set lawful` toggles that character's prevention of unlawful actions. Check the current setting with `set` and use a separate test account to disable prevention for the character who must complete the entrance.
2. As the witness, use `report` to get the incident ID. At the Watch House or next to the on-duty enforcer, use `report <crime-id>`. As an officer, inspect `crimeinfo <crime-id>` or `rapsheet <visible-target>`. A report brings a case to the authority's attention. Known identity and physical or testimonial evidence are separate from the report itself. For the first test, use a witness who recognises the defendant and has an ordinary character dub for them.
3. Bring the wanted test character within sight and reach of the posted patrol. After a warning, use `surrender <constable>` as the test defendant. Watch the guard take them to the Watch House and process them into remand. Check `score`, their actual room and the belongings bundle. If the report never made the case known, recheck witness perception and where the report was made.
4. If you want a player-controlled escort, an eligible enforcer uses `takecustody <target> [<jurisdiction>]`, stays near the prisoner and can use `transfercustody <prisoner> <enforcer>` or `endcustody <target>`. This is field custody. Use the NPC patrol intake for the remand walkthrough. The runtime also documents a FutureProg handover for scripted intake.

For more evidence, run a separate test with a configured video system. Have someone actually view the incident footage, then check whether that viewer can report. A recording and a delivered report are different facts. Staff can use `witnessmemory <id>` for historical witnesses, current recall and pending virtual reports. Existing virtual witness profiles may report according to their configured class, authority and reporting rules. Start with a direct witness so you can tell which report triggered the case. [Witness memory and video evidence](../Economy/Law_System_Runtime.md#witness-recall-and-delayed-virtual-reports)

## Offer bail and legal counsel

### Bail and returning to remand

Create `AlderBail`, a Number prog taking one Character, with body `return 10`. It returns an amount in the jurisdiction's base currency units. Authorities can also use a Character and Crime signature if the amount varies by charge. Configure a long enough deadline to play the test.

```text
legalauthority set bail AlderBail
legalauthority set autoconvicttime 2 days
legalauthority show "Alderbridge Bench"
```

`autoconvicttime` also sets the bail return interval. Keep no-judge `autoconvict` off for the roleplayed hearing. The law must allow bail and the defendant must have gone through arrest first. At the prison desk an advocate can use `bailout list` and `bailout <defendant> [<bank-account>]`. A remand prisoner can use `bailout` to pay with stored belongings cash. Check the return date in `score`, then have the test defendant return to the prison location and use `returnbail` before that date. On a timely return, check that the deposit is added to the belongings held for release. The defendant must be back in custody to be tried. A missed deadline can forfeit the deposit and can generate a `ViolateBail` charge where an applicable law provides for it.

### Paid lawyers and court appointments

Put a separate lawyer NPC in an office one short, walkable route away from the court and verify they can reach the defendant at the hearing. Create a Number prog named `AlderCounselFee` taking two Characters in order, `client` then `lawyer`, and give it the body `return 3`. Train the NPC in your world's legal argument skill or check setup.

```text
ai edit new lawyer AlderCounselAI
ai set fee AlderCounselFee
ai show
ai close
ai add AlderCounselAI <lawyer-npc>
```

The AI's `court` switch controls whether the NPC may also be appointed by an automated court. Fresh Lawyer AIs allow it. Use a second eligible NPC if you want to test public defence while the private lawyer is occupied. `ai set home <prog>` takes a Location-returning prog with one Character, and `ai set bank <prog>` takes a BankAccount-returning prog with one Character, the lawyer. Bind a bank destination if fees should pay into the lawyer's account. The `hire` setter stores an eligibility prog, but this version's availability check does not consult it. Use the actual offered list to verify who can be hired.

As a remand prisoner, run `engagelawyer me` to see available lawyers and `engagelawyer me <lawyer>` to pay for representation. An advocate can use `engagelawyer list` at the prison desk and `engagelawyer <defendant> <lawyer> [<bank-account>]`. A prisoner hiring themselves from remand can draw cash from their belongings bundle. Verify the fee, the counsel assignment and the NPC's travel to court. The automated judge may appoint an available court-approved lawyer for a defendant without counsel. A player-run hearing can roleplay counsel and witnesses directly.

## Hold a trial and enforce the result

### A hearing run by a player magistrate

Stand in the courtroom as the magistrate. Keep the defendant in a registered remand cell before summoning them. Use the **crime ID** from the docket, which is distinct from the **law ID** used in the editor.

```text
trial docket "Alderbridge Bench"
trial summon <defendant>
trial
crimeinfo <crime-id>
convict <defendant> <crime-id>
```

The last line previews available punishments. Hear the plea, witnesses and arguments in roleplay. Then use `convict <defendant> <crime-id> fine 5cr` or `acquit <defendant> <crime-id>` to record the decision. Decide each charge separately. Finalising the last known charge closes the session and applies release or custody according to the result.

For a fine-only outcome, check release, returned belongings and the outstanding amount. The defendant can use `payfine` at the Watch House and settle with `payfine <crime-id> [<bank-account>]` or `payfine all`. An administrator can link a suitable account with `legalauthority set bankaccount <bankcode>:<account>` or use `none` for virtual revenue. Inspect the destination and `legalauthority set revenue ledger`. This revenue belongs to the authority's collection path. Link the intended account if fines should reach the town treasury.

For a separate custody test, configure a jail punishment on the test law before creating a fresh case. Read the options with `convict <defendant> <crime-id>`, then exercise a suitable sentence with `convict <defendant> <crime-id> jail "1 day"`. Check the convicted character's `score`, the room used for the sentence, and the eventual release location and belongings. Compare with acquittal and the fine-only case. Keep administrative holds and events off during these checks so you can observe what the sentencing path does.

### An automated hearing

Use separate loaded NPCs for the judge and prosecutor. The judge requires Judge AI, which extends Enforcer AI. Assign the judge to the **Bench** filter you authored above, for example by granting the NPC the Magistrate appointment. Give the prosecutor Enforcer AI and use a separate `Prosecution` authority whose Boolean Character filter matches only the prosecutor. Use the prog's name or ID for `<prosecutor-prog>`. For a dedicated role, use an appointment or, in a disposable test, `return @ch.id == <prosecutor-id>`. Give the role `accuse General` and reserve adjudication for the Bench. If the prosecutor is already in the militia, exclude them from the Watch filter before using a dedicated route. Put both on duty with their NPC `enforcer` commands.

Stand in the courtroom when issuing the node commands. Add working AI and staffing before marking each route ready.

```text
ai edit new judge AlderJudgeAI
ai close
ai add AlderJudgeAI <judge-npc>
ai add AlderWatchAI <prosecutor-npc>
legalauthority set enforcement add Prosecution
legalauthority set enforcement Prosecution filter <prosecutor-prog>
legalauthority set enforcement Prosecution accuse General
force <judge-npc> enforcer "Alderbridge Bench"
force <prosecutor-npc> enforcer "Alderbridge Bench"
legalauthority set patrol add Morning Bench
legalauthority set patrol "Morning Bench" strategy Judge
legalauthority set patrol "Morning Bench" numbers Bench 1
legalauthority set patrol "Morning Bench" node
legalauthority set patrol "Morning Bench" linger major 15 minutes
legalauthority set patrol "Morning Bench" ready
legalauthority set patrol add Court Prosecution
legalauthority set patrol "Court Prosecution" strategy Prosecutor
legalauthority set patrol "Court Prosecution" numbers Prosecution 1
legalauthority set patrol "Court Prosecution" node
legalauthority set patrol "Court Prosecution" linger major 15 minutes
legalauthority set patrol "Court Prosecution" ready
showpatrols "Alderbridge Bench"
```

For the first automated test, use a fresh remand case and `requesttrial` with the Judge patrol present in a free court. Follow the prompts for `plead guilty` or `plead innocent`. A defendant acting for themself uses `argue defense` when prompted. A valid prosecutor patrol member presents the prosecution. Follow charges, arguments, verdict and sentence with `trial`. If it stalls at arguments, check both counsel and prosecutor presence in the same immediate area, especially in a long RouteRoom. The judge waits for the prosecution role.

To offer scheduled retrieval, add a **Sheriff** route, put its node at court and provide another on-duty NPC that matches the route's staffing requirement. It looks for eligible remand prisoners after the configured delay when the court is free. Leave `autoconvict` off if all convictions should go through hearings. Turning it on enables a separate no-judge sentencing path after the delay. A PC trial and an automated trial use the same court capacity. [Trials, bail and patrol diagnostics](../Economy/Law_System_Runtime.md#trials-and-pc-judges)

## Diagnose the case from each role

| What happened | Where to look |
| --- | --- |
| Entry was refused or no case exists | Check lawful-action protection, property authorisation and its criminal-code switch, world zone, offence type, `auto`, legal class and exception prog. Test voluntary movement as an ordinary character. |
| The witness saw the incident but cannot report | Check `report` for recall and whether the case is already known. Go to the authority's prison desk or stand near a matching on-duty enforcer. |
| The patrol never starts | Use `legalauthority status`, `showenforcers` and `showpatrols`. Check ready state, route nodes, schedule, strategy requirements, staffing, keys and paths. A crime-triggered route can be correctly waiting. |
| The guard sees the defendant but does not book them | Check whether the crime is known and arrestable, the applicable law's enforcement strategy, the enforcer's arrestable classes and the path to prison. Confirm remand and belongings rooms exist. |
| The docket has a name but will not summon | Check judgement rights for that class, physical remand, bail status and an occupied courtroom. |
| The lawyer is missing or the hearing stalls | Inspect the paid engagement and route to court. Check that the judge, prosecutor, defendant and counsel can reach one another. |
| A sentence seems complete but the defendant is still held | Check all other unresolved charges, outstanding custody and any unpaid-fines detention. Inspect the authority's release room and hooks and confirm the actual location and belongings. |
