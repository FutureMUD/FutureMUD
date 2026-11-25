---
slug: justice-system
category: Economy
summary: Build a legal authority with laws, patrols, prisons and courts. Configure arrest, bail, advocacy checks, sentencing and execution patrols.
---

# Building the justice system

A **legal authority** is a jurisdiction with its own enforcement zones, currency, laws, custody rooms and court. A **legal class** tells it which group a character belongs to for the purposes of the law. An **enforcement authority** gives qualifying characters powers such as arrest or judgement. A **law** defines an offence and how to respond to it. A **crime** is a case recorded against someone under that law. A witness, a report, an arrest and a conviction are separate events.

Alderbridge needs a Watch House, a bench that can hear cases when player magistrates are away, and a clear rule for who can arrest or judge whom. This walkthrough fits those services around the Market Workshop from the town guide. Use the IDs and names from your own world. The example charges use `cr` for credits. [Clans and town government](Clan_Builder_Guide.md)

## Create the jurisdiction and its legal class

Create a Boolean FutureProg named `AlderEveryone` with one Character parameter named `ch`. Give it the body `return true`. For each new prog in this guide, use `prog edit new <name>`, set its return type with `prog set return <type>`, add parameters with `prog set parameter add <name> <type>`, then enter the body with `prog set text` and submit with `@` on its own line. Check the compile result. Keep progs that inspect live state non-static. [FutureProg authoring](FutureProg_Builder_Guide.md)

As an administrator, create a jurisdiction and its fallback class. Once the class is present, add the world zone containing the town's rooms.

```text
legalauthority edit new "Alderbridge Bench" <currency-id>
legalauthority set class add General AlderEveryone
legalauthority set zone <world-zone-id>
legalauthority classes "Alderbridge Bench"
legalauthority show "Alderbridge Bench"
```

The zone here is the **world zone** that contains the rooms. Alderbridge can use the same currency for taxes and fines, but its economic zone and clan are separate records. Check all authorities that share a room's zone before adding a new one. Each authority evaluates its own laws in its enforcement area.

Start with one class for residents, officers and visitors. If your setting later distinguishes groups such as citizens, visitors or clergy, author Boolean Character membership progs and verify which class the authority resolves for representative characters, including staff, NPCs and outsiders. Keep a catch-all because the engine expects a match. Set intended priorities with `legalauthority set class <class> priority <number>`. When more than one prog matches, the highest priority wins. Equal priorities resolve to the lowest class ID. Give specialised classes a higher priority than your catch-all. The class editor's `detain` switch permits detention for unpaid fines. Alderbridge leaves it off so residents can pay fines at the desk after release.

## Define who can enforce and judge

Create a local `Magistrate` appointment in the town clan and grant it to the character who will hear the trial. As an administrator with that member present, use the following commands.

```text
clan edit alder
clan create appointment Magistrate Mag
clan appoint <magistrate-character> alder Magistrate
```

For the watch, invite the constable to Alderbridge Militia and make sure their membership is active. You can later narrow eligibility to a specific militia office if the militia includes people who should never police the town.

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

The Bench needs both the power to judge and an accusable class covering the defendant. The Watch needs permission to arrest that class and the law itself must permit arrest. Use `legalauthority set enforcement <role> show` to check the flags and class lists. The `forgive` switch enables a separate power to remove active charges the enforcer could accuse. The `also` option includes another enforcement authority's powers. Keep the filters disjoint so members of the Bench exercise judicial powers and the Watch handles arrest.

Use `legalstatus "Alderbridge Bench"` from the constable's and magistrate's characters to confirm they have jurisdiction. Check the same command as an ordinary resident when reviewing who can see enforcement diagnostics.

## Lay out the watch house, prison and court

Put the Watch House desk off the market street, with the remand cells and belongings store behind staff-controlled doors. Connect the cells to court and the release gate to the public street. Issue the Watch keys for the route it will actually use, including the return trip.

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

| Role | How Alderbridge uses it |
| --- | --- |
| `prison` | The public prison location handles reporting, fine payment, bail and custody intake. It also receives sentenced prisoners for authorities with no jail entry configured. |
| `cell` | These are **remand** rooms. Intake selects a registered cell and records that the prisoner is awaiting sentencing. `cell` toggles each room, so inspect the list before repeating it. |
| `belongings` | Intake stores confiscated belongings as a character-specific bundle. Use a secure staff-only room that can return the prisoner's possessions at release. |
| `court` and `release` | The court hosts trials. The release location is the public destination when the authority releases someone. Provide a usable exit from it. |
| `jailentry` and `jail` | Set `jailentry` to the room that receives newly sentenced prisoners. Use `jail` to mark the rooms in that jail, including the entry if it is inside the secure wing. Keep sentenced housing distinct from remand. |

The authority also has `heldprog`, `imprisonedprog` and `releasedprog` hooks for Void progs taking the prisoner Character. Use them for in-game notices or an authored handoff. Use each hook for the transition it represents. Provide staffed access to secure doors, drinking water and a way to ask for a hearing. A room role does not provision locks or guards.

## Write and inspect a law

The Market Workshop is open to its owner, its tenant and people authorised to enter. Alderbridge treats unauthorised entry as trespass. Put the workshop property in the Bench's world zone and enable its criminal-code switch if it is off.

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

Post the **Workshop Watch** at the workshop entrance, where the constable can see anyone coming out. Stand there to add the route's first node. Station the constable there while the workshop is open.

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
| `ReactivePatrol` | Responds to recent reported violent incidents in coverage. Use `config radius <rooms>`, `window <timespan>` and `duration <timespan>` to tune dispatch. A quiet trespass can come to a fixed post through a witness report. |
| `InvestigationPatrol` | Responds to eligible known cases in reach. Use `config radius <rooms>`, `reliability <0.0-1.0>` and `search <timespan>`. Check case evidence before and after the investigation. |
| `Judge`, `Prosecutor`, `Sheriff` | Station the court staff in the courtroom. The sheriff can arrange pickup from remand when a hearing is due. See the NPC hearing setup below. |

A workshop foreman sees a visitor cross into the leased premises without permission. The foreman uses `report` to find the incident ID, then `report <crime-id>` when speaking to the Watch at its post or at the Watch House. A witness who recognises the visitor can use an ordinary character dub to identify them in the report. The constable can review the case with `crimeinfo <crime-id>` and consult the visible person's record with `rapsheet <target>`.

When the Watch sees the wanted visitor, it warns them to surrender. The visitor can use `surrender <constable>` and the patrol escorts them to intake. A player enforcer can instead hold a nearby wanted person with `takecustody <target> [<jurisdiction>]`. Stay close to the prisoner and use `transfercustody <prisoner> <enforcer>` or `endcustody <target>` when handing over or ending personal custody. The runtime reference also documents scripted intake with a handover prog.

Where the workshop has video, a person who actually views the relevant footage can become a witness. Staff can use `witnessmemory <id>` to inspect historical witnesses, current recall and pending virtual reports. [Witness memory and video evidence](../Economy/Law_System_Runtime.md#witness-recall-and-delayed-virtual-reports)

Before opening Alderbridge, confirm that an authorised tenant can enter, an unauthorised visitor can be reported, and a surrendered character reaches remand with their possessions recorded. Use ordinary accounts for these checks. `set lawful` can block the visitor's unlawful movement before entry.


## Set laws for resistance, bail violations and escape

These are separate offence types. Give each its own law and enable `auto` when the jurisdiction should record it. The engine checks the enforcement zone, the applicable legal classes, any policy prog and the repeat rules when the event occurs. A recorded offence may still need a witness report before it is on the docket or can drive a fresh patrol response.

| Crime type | When the engine checks it |
| --- | --- |
| `ResistArrest` | An NPC patrol issues a surrender warning. Completing voluntary movement while warned or letting the 90-real-second warning expire without complying calls the patrol's failure-to-comply routine. The routine skips a helpless target. A newly recorded resistance offence can become that patrol's active enforcement crime, so its strategy also governs the rest of that encounter. A strategy that skips the warning bypasses this warning-based trigger. |
| `ViolateBail` | At or after the return deadline, the authority's bail heartbeat checks for the offence and ends bail. The deposit is forfeited and the original charges lose their bail protection. Those consequences still happen if there is no matching bail-violation law. The defendant remains at large until arrested again. |
| `EscapeCaptivity` | A character in an enforcer's personal custody voluntarily completes a move without the custodian in the same movement. The custodian is the victim for class and prog filtering. This is the custody created by `takecustody`. The hook skips a shared move with the custodian and involuntary movement such as being dragged. The hook belongs to personal custody. Remand-room and sentenced-jail exits still need appropriate locks, guards and access rules. |

`set lawful` can stop voluntary movement before either the warning or custody rule records its offence. Bail expiry runs independently of movement and the lawful setting. `returnbail` at the prison desk handles a timely return. Use the deadline in `score` to schedule it.

### Keep workshop arrests restrained and escalate escape

Alderbridge keeps ordinary trespass on `ArrestAndDetain`. Its Bench fines a bail violator, can put someone who resists on a good behaviour bond, and can jail a fugitive after conviction. Create the three laws in the authority editor.

```text
legalauthority set law add "Refusing the Watch" ResistArrest
legalauthority set law add "Failure to Return" ViolateBail
legalauthority set law add "Breaking Watch Custody" EscapeCaptivity
```

Use `legalauthority set law <law> show` to check each law's offender classes. Add `offender General` wherever General is absent. On **Breaking Watch Custody**, also add `victim General` if needed so the custodian qualifies. These options toggle classes. Then enable automatic application and arrest once for each new law. Bail defaults to off. Decide separately whether to offer it on the new charges.

```text
legalauthority set law "Refusing the Watch" strategy ArrestAndDetain
legalauthority set law "Refusing the Watch" arrest
legalauthority set law "Refusing the Watch" auto
legalauthority set law "Failure to Return" strategy ArrestAndDetain
legalauthority set law "Failure to Return" arrest
legalauthority set law "Failure to Return" auto
legalauthority set law "Breaking Watch Custody" strategy LethalForceArrestAndDetain
legalauthority set law "Breaking Watch Custody" priority 30
legalauthority set law "Breaking Watch Custody" arrest
legalauthority set law "Breaking Watch Custody" auto
```

`LethalForceArrestAndDetain` keeps the surrender warning and allows the patrol to equip for lethal combat if it has to engage. A helpless fugitive remains eligible for custody. Give the pursuing patrol suitable in-game equipment and combat settings, and check which charge it selects when several are open. On a fresh encounter the patrol chooses the strongest eligible enforcement strategy first and uses law priority to break ties. A reported escape can take priority over the original trespass without changing how the Watch handles every trespasser. Use `LethalForceArrestAndDetainNoWarning` only where the jurisdiction intends to skip the surrender warning. `KillOnSight` is a separate policy that does not use the arrest-and-custody path.

Before opening the jurisdiction, have a constable escort someone in personal custody and check that no escape charge appears. On a separate case, have the prisoner leave on their own. Confirm the witness report names the escape offence, the patrol selects the intended charge and surrender still leads to remand. [Patrol response and evidence](../Economy/Law_System_Runtime.md#alerts-and-enforcer-response)

## Set penalties and apply sentences

A law's **enforcement strategy** governs the patrol's encounter with a wanted person. Its **punishment strategy** governs the options on conviction. Use `legalauthority set law <law> punishment` to inspect the current strategy and `punishment type <type>` under that law to replace it. Review the new values before making the law available for sentencing.

| Strategy | Authoring and effect |
| --- | --- |
| `fine` | Set `punishment fine <amount>` for the standard amount and `punishment maxfine <amount>` for the ceiling. Equal amounts give a fixed fine in automated sentencing. With a range, the automatic result rises with the offence's sentencing severity. The defendant pays the recorded debt at the prison desk. |
| `jail` | Set `punishment maximum <duration>` first, then `punishment minimum <duration>`. The editor can adjust the other bound when one changes. Inspect both after editing. A custodial sentence sets or extends the term. Once the case is resolved the authority transfers the prisoner to `jailentry`, with `prison` as a fallback. |
| `bond` | Use `punishment length <duration>` to set a good behaviour bond. The effect tracks an expiry and can be queried by another law or punishment prog. To give repeat offences a different sentence, explicitly make that policy depend on the bond. |
| `execute` | Permits a death sentence. A conviction with that result creates an awaiting-execution status. The execution patrol carries out the sentence when it can dispatch. |
| `multi` | `punishment add <type>` combines several punishments. Each contributes its part of the sentence. An empty list gives no punishment. |
| `hierarchy` | `punishment add <type> <prog>` adds a conditional choice. The first successful Boolean predicate supplies the result and the allowed sentencing options. Put the specific case first and a catch-all last. With no matching branch, there is no punishment. The Character parameter receives the defendant. |

Edit a child by its displayed number, such as `punishment 1 length 7 days` for a bond. In a hierarchy, use `punishment swap 1 2` to change which condition the court considers first. When a combination contains multiple fines or jail terms, the court adds their bounds.

### Give the Bench a sentencing schedule

The Bench uses a fourteen-day bond for **Refusing the Watch**, a fixed `10cr` fine for **Failure to Return**, and one to three days of custody for **Breaking Watch Custody**.

```text
legalauthority set law "Refusing the Watch" punishment type bond
legalauthority set law "Refusing the Watch" punishment length 14 days
legalauthority set law "Failure to Return" punishment type fine
legalauthority set law "Failure to Return" punishment fine 10cr
legalauthority set law "Failure to Return" punishment maxfine 10cr
legalauthority set law "Breaking Watch Custody" punishment type jail
legalauthority set law "Breaking Watch Custody" punishment maximum 3 days
legalauthority set law "Breaking Watch Custody" punishment minimum 1 day
legalauthority set law "Breaking Watch Custody" punishment
```

For a jurisdiction that distinguishes a first refusal from one on an active bond, give **Refusing the Watch** a hierarchy instead. Make a Boolean Character prog called `AlderOnBond` whose body is `return OnGoodBehaviourBond(@ch, ToLegalAuthority(<authority-id>))`. Use the Bench's numeric ID. Put `jail AlderOnBond` first and `bond AlderEveryone` second. A defendant with a live Alderbridge bond then receives the jail branch for the new conviction. Keep the bond duration and jail range visible in the law's punishment view.

### Record the court's decision

In a player-run hearing, use `convict <defendant> <crime-id>` to inspect permitted options, then pass the sentence after that crime ID. Options are for an individual charge. Where the law permits the combination, enter more than one, as in `convict <defendant> <crime-id> fine 10cr jail "2 days"`. Quote multiword amounts and durations. Use `goodbehaviour` to apply the law's configured bond length or `death` where the options permit it. Use `nothing` alone for conviction without punishment, and `acquit <defendant> <crime-id>` for a not-guilty decision. The configured maxima apply. A selected fine or jail term below the displayed minimum prompts the judge to confirm with `accept sentence` within 60 seconds. For example, following restitution a magistrate may choose one day for a charge with a two-day minimum. Use `crimeinfo <crime-id>` to review the confirmed override alongside the sentence.

Use `punishment onpunish <prog>` on a law or child punishment to run a Void prog when that penalty takes effect. The prog receives the defendant and can optionally receive the law name as Text. A compound sentence runs the hooks on the parent and the penalties it applies, once per charge. A hierarchy uses the selected branch. A sentence of `nothing` applies no penalty.

In an automated hearing, the engine chooses the result through the configured strategy after conviction. The judge's sentence display grows as each charge is announced. The final disposition depends on all charges. Fine-only and bond-only cases can go through release, with unpaid-fine detention depending on the applicable class policy. Custodial terms stay on the prisoner until served, and a death sentence sends them through the holding path. [Sentencing and custody flow](../Economy/Law_System_Runtime.md#death-sentences)

## Offer bail and legal counsel

### Bail and returning to remand

Create `AlderBail`, a Number prog taking one Character, with body `return 10`. It returns an amount in the jurisdiction's base currency units. Authorities can also use a Character and Crime signature if the amount varies by charge. Alderbridge gives its defendants two in-game days to return.

```text
legalauthority set bail AlderBail
legalauthority set autoconvicttime 2 days
legalauthority show "Alderbridge Bench"
```

`autoconvicttime` also sets the bail return interval. Keep no-judge `autoconvict` off for the roleplayed hearing. The law must allow bail and the defendant must have gone through arrest first. At the prison desk an advocate can use `bailout list` and `bailout <defendant> [<bank-account>]`. A remand prisoner can use `bailout` to pay with stored belongings cash. Check the return date in `score`, then have the defendant return to the prison location and use `returnbail` before that date. On a timely return, check that the deposit is added to the belongings held for release. The defendant must be back in custody to be tried. A missed deadline can forfeit the deposit and can generate a `ViolateBail` charge where an applicable law provides for it.

### Paid lawyers and court appointments

Give Alderbridge's defence counsel an office near the court with a route through any doors they must pass. Create a Number prog named `AlderCounselFee` taking two Characters in order, `client` then `lawyer`, and give it the body `return 3`. The advocacy check setup below determines what skills their training should include.

```text
ai edit new lawyer AlderCounselAI
ai set fee AlderCounselFee
ai show
ai close
ai add AlderCounselAI <lawyer-npc>
```

The AI's `court` switch controls whether the NPC may also be appointed by an automated court. Fresh Lawyer AIs allow it. Keep a second eligible lawyer available if the town needs public defence while private counsel is occupied. `ai set home <prog>` takes a Location-returning prog with one Character, and `ai set bank <prog>` takes a BankAccount-returning prog with one Character, the lawyer. Bind a bank destination if fees should pay into the lawyer's account. Use `ai set hire <prog>` to screen paid engagements. The Boolean prog takes the defendant and can also take the lawyer. If someone else pays, eligibility is still evaluated for the defendant. Court appointments use the `court` switch and normal availability.

As a remand prisoner, run `engagelawyer me` to see available lawyers and `engagelawyer me <lawyer>` to pay for representation. An advocate can use `engagelawyer list` at the prison desk and `engagelawyer <defendant> <lawyer> [<bank-account>]`. A prisoner hiring themselves from remand can draw cash from their belongings bundle. The lawyer should appear as assigned counsel and be able to reach the defendant at the hearing. The automated judge may appoint an available court-approved lawyer for a defendant without counsel. A player-run hearing can roleplay counsel and witnesses directly.

### Configure the advocacy checks

Both sides roll a check for each contested charge in an automated trial. `DefendLegalCase` uses the defence lawyer's traits, or the defendant's traits when they represent themselves. `ProsecuteLegalCase` uses the prosecutor's traits. The judge compares the opposing outcomes. The case supplies each side's difficulty. Identity certainty, witness evidence and investigation findings can make the prosecution's task easier or harder.

1. Run `show checks` and find **DefendLegalCase** and **ProsecuteLegalCase**. Record the **Expr ID**, formula and check template for each. A seeded setup can use the **Law** skill for both, with a constant `50` fallback when the skill is absent. A constant formula gives no advantage to a trained lawyer.
2. If the setting needs skill-based advocacy, create or choose the Law skill, then edit each check's existing expression with `te edit <expression-id>`. For a single-skill formula, use `te set formula law:<law-skill-id>` with the actual trait ID, and confirm with `te show <expression-id>`. Review other users before editing a shared expression. Use `checktemplate show <template>` to inspect difficulty modifiers and missing-trait behaviour. Tune shared template settings only when the change makes sense for the other checks assigned to it.
3. Give your lawyer and prosecutor NPC templates the skill each expression reads. Have the changed definitions loaded before opening the court. Verify the two advocates in the trial's per-charge argument records. An imported or newly created NPC can have the AI and still lack the skill its check expects.

The fee, bank and home progs control the lawyer service. They are separate from the advocacy checks. [Check and trait-expression editors](Check_and_Decorator_Editors.md)

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

When the Bench imposes jail time for breaking Watch custody, check the term in `score` and send the prisoner through the sentenced wing's intake. The authority uses its configured release room when the term is served.

### An automated hearing

Use separate loaded NPCs for the judge and prosecutor. The judge requires Judge AI, which extends Enforcer AI. Assign the judge to the **Bench** filter you authored above, for example by granting the NPC the Magistrate appointment. Give the prosecutor Enforcer AI and use a separate `Prosecution` authority whose Boolean Character filter matches only the prosecutor. Use the prog's name or ID for `<prosecutor-prog>`. Create a `Prosecutor` appointment in the town clan. Model its eligibility prog on `AlderMagistrateFilter`, looking up `Prosecutor` instead of `Magistrate`. Give the role `accuse General` and reserve adjudication for the Bench. If the prosecutor is already in the militia, exclude them from the Watch filter before using a dedicated route. Put both on duty with their NPC `enforcer` commands.

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

A defendant in remand can use `requesttrial` when the Judge patrol is present and the courtroom is free. Follow the prompts for `plead guilty` or `plead innocent`. A defendant acting for themself uses `argue defense` when prompted. A valid prosecutor patrol member presents the prosecution. Follow charges, arguments, verdict and sentence with `trial`. If it stalls at arguments, check both counsel and prosecutor presence in the same immediate area, especially in a long RouteRoom. The judge waits for the prosecution role.

To offer scheduled retrieval, add a **Sheriff** route, put its node at court and provide another on-duty NPC that matches the route's staffing requirement. It looks for eligible remand prisoners after the configured delay when the court is free. Leave `autoconvict` off if all convictions should go through hearings. Turning it on enables a separate no-judge sentencing path after the delay. A PC trial and an automated trial use the same court capacity. [Trials, bail and patrol diagnostics](../Economy/Law_System_Runtime.md#trials-and-pc-judges)

## Set up an execution patrol

To follow court and patrol activity in Discord, select the jurisdiction and use `legalauthority set discord <channelid>`. The channel receives an alert when a defendant successfully requests a trial, when a judge opens a hearing, and when an execution patrol launches. The patrol may still abort during preparation or retrieval. A linked JuniorAdmin can follow an alert with `/justice status`, select the jurisdiction by ID or exact name, and inspect waiting defendants, judge availability and active patrols. [Discord justice status](../Core/Discord_Remote_Operations.md#justice-status)

Alderbridge reserves capital sentencing for **Murder under the Crown's Peace** and authorises the Watch to bring the accused before the Bench. This example uses a law that the Watch invokes by accusation. It leaves automatic application off.

```text
legalauthority set law add "Murder under the Crown's Peace" Murder
legalauthority set law "Murder under the Crown's Peace" strategy ArrestAndDetain
legalauthority set law "Murder under the Crown's Peace" arrest
legalauthority set law "Murder under the Crown's Peace" punishment type execute
legalauthority set law "Murder under the Crown's Peace" show
```

Check the classes and add missing offender and victim entries for the capital law. A magistrate can use `convict <defendant> <crime-id> death` after hearing the case. The conviction creates the condemned status. Lethal force during arrest follows the law's separate enforcement policy.

### Create the route and choose a method

Make the execution room the **first patrol node**. Provide a walkable route from the equipment store to the remand cells and then to the execution room. Use on-duty NPCs with Enforcer AI and a matching enforcement authority. The patrol leader acts as executioner and the others support retrieval and custody. Assign enough staff to cover both this route and the regular Watch. Place appropriate in-game restraint items and the real keys for the locked route where the patrol can collect them, or issue them to staff.

Stand in Alderbridge's Writ Court and enter the following commands. This example uses two Watch enforcers and a ceremonial game weapon. Replace the Watch staffing requirement with a dedicated enforcement role if your executioners have a separate appointment.

```text
legalauthority set patrol add Crown Writ
legalauthority set patrol "Crown Writ" strategy ExecutionPatrol
legalauthority set patrol "Crown Writ" node
legalauthority set patrol "Crown Writ" numbers Watch 2
legalauthority set patrol "Crown Writ" config equipment <armoury-room-id>
legalauthority set patrol "Crown Writ" config keys on
legalauthority set patrol "Crown Writ" config method cdg
legalauthority set patrol "Crown Writ" config window 20
legalauthority set patrol "Crown Writ" config lastwordsdelay 30
legalauthority set patrol "Crown Writ" ready
```

Run the final `ready` toggle **after** staffing and equipment are in place. `config equipment none` uses the authority's preparation room. `config keys on` requires the leader to obtain matching keys for the prisoner's retrieval path and relevant locked exits before deployment.

| Method | In-game configuration |
| --- | --- |
| `config method cdg` | Provide a wieldable melee item whose weapon type has an applicable fixed-bodypart `CoupDeGrace` attack. The executioner must be able to wield it, and the attack must match the target's body and the weapon's handedness. The patrol cannot substitute a generic melee attack. |
| `config method drug` | Select an authored game drug with `config drug <drug-id>`, choose a compatible in-game route with `config vector <route>` (`injected`, `ingested`, `inhaled` or `touched`) and set a positive simulated amount with `config dose <game-grams>`. The patrol tries to equip an injector. The drug definition supplies the simulated effect directly. Read the drug's game definition and the target's health model when diagnosing the outcome. |
| `config method firing` | Give the patrol members usable in-game ranged items and compatible ammunition in the equipment room or in their loadouts. The route needs at least one member able to ready a weapon. At the execution room, individual shooters must be able to fire under the normal ranged rules. Size the patrol to match the number of shooters intended by the scene. |

Use `config emote lastwords <emote>` and `config script add <emote>` to author the ceremony. The script runs after the last-words window. Use `$0` for the executioner and `$1` for the condemned outside quoted speech, and `%executioner%` or `%condemned%` inside speech. `config scriptdelay <seconds>` controls the pace between steps. `config confirmdelay <seconds>` and `config attempts <count>` govern waiting for a confirmed result and retrying the selected method.

### Know when the route will dispatch

A new sentence sets the execution date one in-game day ahead. The ordinary dispatcher needs a loaded, living prisoner with a due sentence for this authority, a ready route with matching available staff and a complete method configuration. Only one execution patrol can run for the authority at a time. A condemned character in a registered remand cell can use `requestexecution` to ask to bring their date forward when a route can take the job.

The patrol prepares, retrieves the prisoner, offers a chance to surrender, then escorts and secures them before the last words and ceremony. The engine requires actual helplessness, submission or equivalent control. A restraint that leaves the prisoner mobile may be insufficient. The patrol completes the sentence only when the engine confirms death. When attempts are exhausted, the route returns the prisoner to holding, delays the date and aborts. Missing keys or an unusable path can stop the route before the sentence is attempted. Inspect `legalauthority status`, `showenforcers`, `showpatrols` and the prisoner's `score` to distinguish a normal wait from a setup problem. [Execution patrol runtime](../Economy/Law_System_Runtime.md#execution-patrols) · [Building game drugs](Drug_Builder_Guide.md)

## Diagnose the case from each role

| What happened | Where to look |
| --- | --- |
| Entry was refused or no case exists | Check lawful-action protection, property authorisation and its criminal-code switch, world zone, offence type, `auto`, legal class and exception prog. Check the character's actual access and whether the movement completed. |
| The witness saw the incident but cannot report | Check `report` for recall and whether the case is already known. Go to the authority's prison desk or stand near a matching on-duty enforcer. |
| The patrol never starts | Use `legalauthority status`, `showenforcers` and `showpatrols`. Check ready state, route nodes, schedule, strategy requirements, staffing, keys and paths. A crime-triggered route can be correctly waiting. |
| The guard sees the defendant but does not book them | Check whether the crime is known and arrestable, the applicable law's enforcement strategy, the enforcer's arrestable classes and the path to prison. Confirm remand and belongings rooms exist. |
| The docket has a name but will not summon | Check judgement rights for that class, physical remand, bail status and an occupied courtroom. |
| The lawyer is missing or the hearing stalls | Inspect the paid engagement and route to court. Check that the judge, prosecutor, defendant and counsel can reach one another. |
| An execution route is ready but idle | Check the jurisdiction and date in the prisoner's `score`, then use `showpatrols` for the first dispatch blocker. A route without a due prisoner is waiting normally. |
| A sentence seems complete but the defendant is still held | Check all other unresolved charges, outstanding custody and any unpaid-fines detention. Inspect the authority's release room and hooks and confirm the actual location and belongings. |
