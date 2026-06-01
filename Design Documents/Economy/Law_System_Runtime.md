# FutureMUD Law System Runtime

## Purpose

This document records the current runtime behavior of the legal authority, sentencing, and patrol systems. It is intended as a builder-facing and implementer-facing reference for the coded law system rather than a setting-specific legal guide.

## Death Sentences

Death sentences are represented by `PunishmentResult.Execution`. When multiple punishments are combined, the execution flag is preserved alongside fines, custodial sentences, and good-behaviour bonds.

When a legal authority applies a result with `Execution = true`, the condemned character receives an `AwaitingExecution` effect for that authority. The existing sentencing path sets the execution date one in-game day after sentence application. Execution patrols only select loaded, living characters whose execution date has arrived.

## Execution Patrols

The `ExecutionPatrol` patrol strategy is responsible for carrying out due death sentences. It is a normal patrol-route strategy, but it owns its own state machine instead of using the generic enforcement patrol loop. This keeps execution work focused on the condemned character and prevents the patrol from being distracted by ordinary warrant or crime processing.

The first node on the patrol route is the execution location. Builders should add the execution room as the first patrol node, set the route strategy to `execution`, configure the strategy, assign the required enforcement numbers, and mark the route ready.

Execution patrols require:

- at least one due `AwaitingExecution` effect for the route's legal authority
- a configured execution location from the first patrol node
- enough available enforcers for the patrol route
- no other active execution patrol for the same legal authority
- a complete execution-method configuration

The primary executioner is the patrol leader. Other selected patrol members act as supporting guards. Guards help retrieve equipment, subdue resistance, drag the prisoner, and participate in firing squads when that method is used.

## Builder Configuration

Strategy-specific settings are edited through the patrol route `config` command after selecting the execution strategy.

Core options:

- `method cdg|drug|firing`: selects coup de grace with a weapon, administered drug, or firing squad.
- `equipment here|<room>|none`: sets the room used to collect tools. `none` falls back to the legal authority preparation room.
- `drug <id|name>`: selects the drug used by the administered-drug method.
- `dose <grams>`: sets the administered amount per attempt.
- `vector injected|ingested|inhaled|touched`: sets the drug delivery vector.
- `window <seconds>`: sets how long the prisoner has to use `HELPLESS` or surrender before guards begin subduing them.
- `lastwordsdelay <seconds>`: sets the last-words window after the last-words emote.
- `scriptdelay <seconds>`: sets the delay between scripted execution emotes.
- `confirmdelay <seconds>`: sets the delay between execution attempts while confirming death.
- `attempts <number>`: sets how many execution attempts are made before the patrol aborts.

Custom emotes:

- `emote retrieve <emote>`: sent when the prisoner is contacted.
- `emote resist <emote>`: sent before guards subdue resistance.
- `emote arrival <emote>`: sent at the execution room.
- `emote restrain <emote>`: sent after restraints are secured.
- `emote lastwords <emote>`: sent before the last-words window.
- `emote drug <emote>`: sent when administering the configured drug.
- `emote firing <emote>`: sent when ordering the firing squad.
- `emote complete <emote>`: sent when the sentence is completed.

Script steps are ordered emotes run after the last-words window and before the killing method. Builders can use `script add`, `script delete`, `script swap`, and `script clear`.

Execution emotes use the executioner as `$0` and the condemned prisoner as `$1`.

## Runtime Flow

The execution patrol progresses through these stages:

1. Select a due condemned character and apply `ExecutionPatrolNoQuit`.
2. Move patrol members to the equipment room.
3. Collect restraints and method-specific tools.
4. Move the leader to the prisoner and send the retrieval emote.
5. Give the prisoner the configured compliance window to use `HELPLESS` or submit to the guards.
6. If the prisoner resists, guards switch to control-oriented combat settings where available and attempt to subdue them.
7. Drag the helpless or submitted prisoner to the execution location.
8. Restrain the prisoner with locally available restraints.
9. Offer the configured last-words window.
10. Play each configured execution script step.
11. Carry out the configured execution method.
12. Confirm death, retrying up to the configured attempt limit if necessary.
13. Mark execution-punishment crimes as served, remove `AwaitingExecution`, remove the no-quit effect, and complete the patrol.

If required rooms, tools, paths, or targets become unavailable for too long, the patrol aborts and removes only the execution-specific no-quit effect. The `AwaitingExecution` effect remains so a later patrol can try again.

## Trials and PC Judges

Trials are represented by the `OnTrial` effect for the relevant legal authority. NPC judges drive ordinary `OnTrial` effects through the automated plea, argument, verdict, and sentencing phases. PC-held trials use the same effect as a court-session marker, but set the manual-trial flag so NPC judge AI will not advance or finalise the case.

A PC judge is any enforcer whose enforcement authority has `CanConvict` for the jurisdiction and whose authority can judge the defendant's legal class.

Judge-facing trial commands:

- `trial`: shows the active trial in the current room.
- `trial docket [jurisdiction]`: lists defendants awaiting trial for the judge's jurisdictions, including remand/bail/trial status, charge IDs, charge names, and prior local convictions.
- `trial summon <target>`: from the jurisdiction courtroom, calls a remand prisoner or present defendant before the court and begins a PC-held trial.
- `convict <target> <crime> <sentence>`: records a guilty verdict and sentence for one charge.
- `acquit <target> <crime>`: records a not-guilty verdict for one charge.

`trial summon` uses the same court-transfer mechanics and player-facing emotes as the sheriff's automated trial fetch. It clears remand and enforcer-custody state for that jurisdiction, creates a manual `OnTrial`, and moves the defendant to the court if they are being held in a remand cell. Defendants on bail or at large after bail revocation must first return to custody.

While any `OnTrial` exists in the court for a legal authority, sheriff patrols will not fetch a new automatic trial for that authority. NPC judge AI also ignores manual trials, so a PC-held trial will not be taken over by the automated judge. Once the PC judge has finalised all known charges with `convict` or `acquit`, the manual trial state is removed and the defendant is released, sent to prison, or sent to holding for execution according to the recorded sentences.

## Bail and Automatic Trials

Bail follows an arrest-first policy. A defendant on bail is not eligible for automatic court pickup, no-judge automatic sentencing, defendant-initiated `requesttrial`, or PC judge `trial summon`. They must return to remand custody before any trial path can proceed.

When bail is posted, the `OnBail` effect records a return deadline. The deadline uses the legal authority's automatic-conviction timeframe from the current jurisdiction time. The original remand effect remains on the character, but trial systems also require the defendant to be physically held in one of the authority's remand cells.

When the return deadline expires, the bail heartbeat attempts to record a `ViolateBail` crime, revokes the bail effect, and forfeits the recorded bail on the pending crimes. The defendant becomes arrestable again through ordinary enforcement because the underlying crimes are no longer marked as bailed, but the system does not teleport, summon, try, or convict them while they are at large.

The `trial docket` command distinguishes active bail from bail-revoked at-large defendants so PC judges can see why a person is not currently summonable.

## Crime-Driven Patrol Dispatch

Most patrol routes are scheduled from their route readiness, time-of-day, priority, and enforcer-number requirements. Crime-driven patrol strategies use the same route and enforcer configuration, but the patrol controller only dispatches them when a matching reported crime exists. These patrols are not launched as ordinary scheduled patrols.

Crime-driven routes still require:

- a ready patrol route
- at least one patrol node
- required enforcer numbers
- current time of day matching the route
- the reported crime location to be within the strategy coverage radius of at least one patrol node

The patrol remembers the reported crime as its runtime target. This target is transient patrol state, matching existing active-enforcement state; if a patrol is reloaded without that target, the crime-driven strategy concludes rather than enforcing an unknown target.

### Reactive Patrols

The `ReactivePatrol` strategy dispatches to recent reported violent crimes. It is intended to represent increased police presence after violence in an area. The first patrol destination is the crime location, after which the patrol cycles through nearby configured patrol nodes within the route's coverage radius until its configured duration expires. While deployed, it uses the same enforcement scan as ordinary enforcer patrols and can warn, subdue, arrest, or apply lethal-force enforcement according to the laws involved.

Reactive patrol configuration:

- `radius <rooms>` controls how far from a patrol node a violent crime can be and still dispatch the route.
- `window <timespan>` controls how recent the reported violent crime must be.
- `duration <timespan>` controls how long the increased-presence patrol remains active.

Violent-crime classification includes assaults, deadly assaults, battery, murder-family offences, torture, grievous bodily harm, intimidation, resisting arrest, arson, extortion, sexual violence, kidnapping, slavery, animal cruelty, mayhem, and rioting.

### Investigation Patrols

The `InvestigationPatrol` strategy dispatches to reported crimes whose trial evidence is still weak: crimes with an unknown criminal identity or incomplete recorded suspect characteristics. The patrol travels to the crime scene, spends its configured search time there, records investigative evidence, and returns.

Investigation evidence updates existing crime metadata used by trials:

- `CriminalIdentityIsKnown` can be confirmed when the investigator can directly see the suspect at the scene.
- recorded criminal characteristics are filled in according to the strategy's reliability.
- prosecution difficulty now reflects identity certainty, witness count, physical/third-party evidence, crime-scene location, and collected characteristic evidence.

Investigation patrol configuration:

- `radius <rooms>` controls how far from a patrol node a reported crime can be and still dispatch the route.
- `reliability <0.0-1.0>` controls how accurately suspect characteristics are recorded.
- `search <timespan>` controls how long the patrol searches the scene before recording evidence.

## Equipment and Method Notes

For coup de grace, the leader looks for a wielded melee weapon with a usable coup-de-grace fixed-bodypart attack. If no suitable weapon is held, the leader tries to retrieve one from the equipment room.

For administered drugs, the configured drug is dosed directly into the condemned body using the configured vector and grams. The leader also tries to obtain an injector from the equipment room when preparing.

For firing squads, patrol members try to wield, load, ready, and fire ranged weapons. The method succeeds for the tick if at least one available shooter fires.

Restraints are collected from the equipment room and then applied in the execution room. The patrol first prefers guards already holding valid restraints, but can also use restraint items available locally in the execution room.

## Player Constraint

The condemned receives `ExecutionPatrolNoQuit` while the execution is being carried out. Its no-quit reason explains that the character cannot quit while law enforcement is carrying out the death sentence. The effect is removed when the patrol completes or aborts.
