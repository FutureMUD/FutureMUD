# FutureMUD Law System Runtime

## Purpose

This document records the current runtime behavior of the legal authority, sentencing, and patrol systems. It is intended as a builder-facing and implementer-facing reference for the coded law system rather than a setting-specific legal guide.

## Automatic Crime Application

Automatic enforcement is opt-in per law. A law only applies from coded hooks when `law auto` is enabled, the offender and victim legal-class filters pass, and the optional law prog returns true. `law repeat` controls whether repeated automatic applications against the same law, target, object, and location are suppressed for the short repeat window.

Automatic crime hooks now pass a context text string to the crime record and to law progs that opt into the extended signatures. Existing law prog signatures continue to receive the crime name as their text argument. Builders can use the extended `text text` signatures to receive both the crime name and the automatic context.

Currently supported automatic hooks include:

- `Murder`: applied when a character dies with recent qualifying wounds from another character. Each responsible attacker is checked separately. By default the wound must be at least `Severe`, no more than `7200` real-time seconds old, have a recorded wound time, and not be a friendly-combat wound. Builders can tune this with `AutomaticMurderMinimumWoundSeverity`, `AutomaticMurderWoundAttributionWindowSeconds`, and `AutomaticMurderIncludeFriendlyWounds`. Context includes victim id, wound count, maximum wound severity, damage type, bodypart, wound age, friendly status, and whether the accused was present at the death scene.
- `GreviousBodilyHarm`: applied when a wound caused by another character meets or exceeds `AutomaticGreviousBodilyHarmMinimumSeverity`, defaulting to `Grievous`. Context includes victim id, severity, damage type, and bodypart.
- `Trespassing`: applied when a character enters a property cell without owner, leaseholder, tenant, hotel-room, or `PermitWork` authorisation and the property has criminal-code enforcement enabled. Context includes property and cell ids.
- `Gambling`: applied after an arena wager validates, collects the stake, persists the bet, and credits the arena. Lawful-action protection blocks the wager before money changes hands. Context includes arena id, event id, stake, and betting model.
- `TrafficingContraband`: applied when a character enters a legal-authority enforcement zone from outside that authority while carrying an item that satisfies a `TrafficingContraband` automatic law. The enum spelling is preserved for compatibility. Context includes item, origin cell, destination cell, and actor ids.

Automatic movement hooks evaluate voluntary movers before the movement begins, so a party member with lawful-action protection can block the whole movement before they enter. Dragged movement targets are not charged for automatic entry crimes.

Crime records retain the raw automatic context for administrator diagnostics. `crime info` presents non-admin judges and enforcers with a summarised automatic-evidence block instead of raw context ids, for example an automatic murder record shows the death-investigation source, wound basis, injury details, timing, scene presence, any implement, and a note that this is mechanical evidence rather than an automatic guilty verdict.

Builder customisation remains data-driven: use legal authority enforcement zones for jurisdiction, property `lawful` settings and authorisations for trespass policy, the murder and grievous-wound static settings for injury thresholds, and law progs for venue, route, item, victim, offender, or circumstance-specific exceptions.

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

## Equipment and Method Notes

For coup de grace, the leader looks for a wielded melee weapon with a usable coup-de-grace fixed-bodypart attack. If no suitable weapon is held, the leader tries to retrieve one from the equipment room.

For administered drugs, the configured drug is dosed directly into the condemned body using the configured vector and grams. The leader also tries to obtain an injector from the equipment room when preparing.

For firing squads, patrol members try to wield, load, ready, and fire ranged weapons. The method succeeds for the tick if at least one available shooter fires.

Restraints are collected from the equipment room and then applied in the execution room. The patrol first prefers guards already holding valid restraints, but can also use restraint items available locally in the execution room.

## Player Constraint

The condemned receives `ExecutionPatrolNoQuit` while the execution is being carried out. Its no-quit reason explains that the character cannot quit while law enforcement is carrying out the death sentence. The effect is removed when the patrol completes or aborts.
