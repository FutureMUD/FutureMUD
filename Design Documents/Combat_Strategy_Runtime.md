# Combat Strategy Runtime

## Scope

This document describes the current runtime behaviour of the built-in `ICombatStrategy` implementations. It is intended as a practical reference for maintainers and AI agents auditing combat behaviour, especially around cases where a strategy may otherwise choose no move.

The strategy contract lives in `FutureMUDLibrary/Combat/ICombatStrategy.cs`. Concrete strategies live in `MudSharpCore/Combat/Strategies`.

## Shared Contract

Every strategy exposes:

- `Mode`: the `CombatStrategyMode` represented by the strategy.
- `ResponseToMove`: a defensive or reactive move against another combat move.
- `ChooseMove`: the active move to perform on the combat tick.
- `WillAttack` and `WhyWontAttack`: policy gates around attacking helpless, critically injured, or disarmed targets.

`StrategyBase.ChooseMove` runs the shared active-move pipeline:

1. Core blockers: blocking combat effects, missing combat settings, active movement, unconsciousness, and paralysis.
2. Obligatory moves: wake from sleep, execute selected manual combat action, automatic standing, rescue setup, and rescue moves.
3. Inventory moves: retrieve or wield preferred weapons and shields when inventory automation permits.
4. Combat movement: break clinches and perform strategy-specific movement.
5. Attacks: strategy-specific attack selection.

Returning `null` intentionally idles the combatant for a short combat tick. The audit goal is to reserve that for genuine blockers, manual-control decisions, or fully unavailable actions rather than missed fallback logic.

## Shared Defensive Behaviour

`StandardMeleeStrategy` is the broadest defensive implementation. It can respond to:

- melee and natural weapon attacks with block, parry, dodge, desperate versions, or helpless defense;
- ranged weapon attacks with shield block, ranged dodge, exhaustion, or helpless defense;
- clinch attempts, break-clinch attempts, grapples, and magic attack powers;
- movement into melee by allowing the move unless a derived strategy overrides it.

`RangeBaseStrategy` handles ranged defenses, receive-charge opportunities, and ranged-natural defenses. It delegates magic power attack defense to `StandardMeleeStrategy` so ranged-mode combatants still use ordinary ward/block/parry/dodge coverage against magic. Ranged strategies normally rely on the melee-range setter to switch them to their preferred melee strategy once melee range is established.

`WardStrategy` extends melee defense by trying a ward defense before falling back to standard defense. Wards are available against start-clinch, weapon attacks, and magic attack powers when the defender is not ward-beaten, has stamina, the assailant is upright, and the defender has a usable warding weapon or unarmed ward attack.

## Shared Attack Behaviour

Melee-family strategies select active attacks through this order:

1. Fixed combat actions such as coup de grace or fixed move-type effects.
2. Stamina gate via `MinimumStaminaToAttack`.
3. Weighted attack-mode roll: weapon, natural weapon, magic, psychic, then auxiliary.
4. Weapon/natural/magic attack selection constrained by combat settings, allowed classifications, preferred/forbidden intentions, stamina, target type, and current melee or clinch state.

`SwordAndBoardOnly` is the engine's existing handedness term for weapon-and-shield fighting. A one-handed melee weapon can now select `SwordAndBoardOnly` attacks when the attacker also has a separately wielded shield, and ordinary `OneHandedOnly` attacks remain available in the same loadout. This is what lets shield-line spear attacks coexist with the normal spear thrust suite instead of replacing it.

Auxiliary moves are selected by shared strategy code through `AttemptUseAuxilliaryAction`. The move list comes from the attacker's race combat actions and is filtered by position, intention requirements, forbidden intentions, usability prog, target, and stamina. If the selected channel has authored moves but the actor is too exhausted to pay for any of them, the strategy returns `TooExhaustedMove`.

`StandardMeleeStrategy` now includes `AuxiliaryPercentage` in the normal weighted roll while keeping the old melee fallback: if no weighted channel produces a move, it may still try an auxiliary move before idling. This preserves older custom combat settings that left auxiliary probability at zero but relied on auxiliary moves as a last resort. Ranged-family strategies also include `AuxiliaryPercentage` in their weighted roll, but they retain the ranged no-move fallback when no ranged, natural, magic, psychic, or auxiliary channel is selected.

Ranged-family strategies select active attacks through:

1. Weighted attack-mode roll.
2. Ranged weapon firing, aiming, readying, reloading, or loading when ranged automation permits.
3. Ranged natural attacks when configured or when fallback-to-unarmed is enabled.
4. Optional shift to `FullAdvance` when the combat setting says to move to melee if no ranged engagement is possible.

Ranged firing normally reveals a hidden firer when out-of-combat fire engages the target. The exception is explicit weapon capability: `IRangedWeapon.CanFireWhileHidden` defaults false, and only weapons such as blowguns opt in. When that flag is true, `fire` passes `preserveHide` through `Engage` and `JoinCombat`, which skips removing `IHideEffect` while still removing other combat-start effects. This does not make the firer permanently undetectable; blowgun output uses the normal obscured-emote path and observers can still identify the firer if their perception allows it. Blowguns only reach this path if the component can ready/fire normally, which includes having active breath and an uncovered mouth.

## Strategy Rundown

### StandardMelee

The general melee strategy. It handles automatic weapon and shield loadout, normal melee attacks, natural attacks, weighted auxiliary attacks, fixed attacks, coup de grace, legacy auxiliary fallback, and fallback between normal melee and clinch when only one range has viable attacks.

Expected active no-move cases: no target, combat policy refuses the target, stamina below minimum, fully manual inventory or movement, no viable attack and no fallback.

### Ward

A melee strategy that prioritises reach control. It uses standard melee active attacks but attempts `WardDefenseMove` before ordinary defense where possible. Its weapon fitness prefers weapons with ward-free attacks.

Expected active no-move cases mirror `StandardMelee`.

### Clinch

A melee strategy that tries to enter and fight in clinch range. It starts a clinch when upright, not cooling down, and able to clinch the target. Once clinching, it rolls between clinch weapon attacks and unarmed clinch attacks. It suppresses the base clinch-breaking behaviour because clinch is the desired state.

If it discovers it has no viable clinch attacks but has viable normal melee attacks, it switches back to `StandardMelee`.

### GrappleForControl

A clinch strategy that attempts `InitiateGrapple` and then extends the grapple across available limbs. Once multiple limbs are controlled, it may attempt takedowns unless trip intentions are forbidden.

### GrappleForIncapacitation

Extends `GrappleForControl`. As more limbs become controlled, it increasingly prefers wrenching grappled non-head limbs over extending control.

### GrappleForKill

Extends `GrappleForIncapacitation`. As control improves, it prefers lethal grapple follow-ups such as wrenching or strangling when the matching limb state exists.

### MeleeShooter

A melee strategy that can fire melee-capable ranged weapons before falling back to normal melee. It only considers ranged weapons whose weapon type is fireable in melee.

### MeleeMagic

A melee strategy that attempts configured magic or psychic attack powers when those weighted channels are selected. If no legal power exists, it falls back to weapon attacks.

### FullDefense

A melee strategy that keeps standard defensive reactions and obligatory/inventory/movement handling, but performs no active attacks. It currently does not attempt auxiliary defensive actions.

### Skirmish

A melee-range avoidance strategy. It responds to charge and move-to-melee with skirmish, optionally firing while skirmishing. On its own turn, if engaged in melee or threatened by a melee attacker, it flees from melee range unless super-raging.

### FullSkirmish

Currently inherits `Skirmish` behaviour directly. The distinction is mostly exposed to movement resolution, where full skirmish is harder to catch.

### Flee

A strategy that tries to leave combat or break melee contact. It will not flee while rage effects are present, while grappled, or while being dragged. It still uses standard defensive reactions and skirmishes against incoming melee approaches when possible.

### StandardRange

A cover-seeking ranged strategy. It prefers a ranged weapon loadout, seeks usable cover when not in melee, stands only when needed for the selected ranged weapon, and uses the ranged attack pipeline.

### FireNoCover

A ranged strategy that fires without seeking cover. It can stand-and-fire against a charge and otherwise uses the ranged attack pipeline.

### CoveringFire

Currently a plain `RangeBaseStrategy` with no cover-seeking or special covering-fire active behaviour beyond the shared ranged pipeline.

### FullCover

A cover-seeking ranged strategy that seeks cover and otherwise inherits the ranged attack pipeline. Despite the name, it may still attack if its combat settings include attack percentages.

### FireAndAdvance

A ranged-to-melee strategy. It may aim or fire while advancing, moves across cells toward the target, changes layers to reach a target in the same cell, fires-and-advances into melee when possible, then charges or moves into melee.

### CoverAndAdvance

A cover-seeking ranged-to-melee strategy. It requires moving cover, manages body position around cover, moves across cells or layers toward the target, and moves into melee without charging because charging would leave cover.

### FullAdvance

A pure movement strategy for closing distance. It ignores ranged weapon use, changes position as needed, moves across cells or layers toward the target, then charges or moves into melee. It performs no active ranged attacks.

### Swooper

A ranged-family strategy for flying creatures. It can take flight automatically when movement management permits, change layers toward a same-cell target, skirmish against incoming melee approaches while airborne, and prefer breath swoop attacks when flying and offset from the target by room or layer. Otherwise it falls back to the shared ranged attack pipeline.

### Drowner

A melee-family predator strategy for aquatic ambushers. It behaves as `StandardMelee` against targets that innately do not need to breathe or can breathe the relevant local water fluid. Against air-breathers, it looks for authored forced-movement attacks that can pull or shove the target into an underwater layer in the same cell or through an exit into adjacent water. Once the target is underwater, it delegates to `GrappleForControl` so the victim is held there rather than repeatedly repositioned.

Expected active no-move cases: standard melee blockers, no local underwater layer or adjacent water exit, no authored forced-movement attack for the current range and destination type, or hard movement rules rejecting the destination.

### Dropper

A melee-family flying predator strategy. It behaves as `StandardMelee` unless the combatant can fly, can drag the target's weight, and is in the same location. It attempts to establish a controlled grapple, then uses authored forced layer-pull attacks to carry the target to the next higher viable layer. When no higher layer exists, it releases the grapple so the existing fall system handles the drop.

Expected active no-move cases: standard melee blockers, inability to fly or lift the target, no controlled grapple yet and grappling cannot progress, no higher viable layer, or no authored pull-layer attack for the current grapple range.

### PhysicalAvoider

A melee-range avoidance strategy that tries to create distance without leaving the room. When engaged, it prefers pushback attacks, then trip or stagger style physical control, then withdraws only to ranged distance if the opponent cannot meaningfully oppose. If it is already at range and not being pressed by a melee attacker, it does not flee merely because combat is ongoing.

Expected active no-move cases: already safely out of melee, no viable pushback/trip/stagger attack, target can still oppose a withdrawal, or standard melee blockers.

## Movement and Layer Considerations

The ranged advance strategies handle three spatial cases:

- different cells: path to the target and move through the first exit when automatic movement is enabled;
- same cell but different layers: attempt layer movement by swimming, flying, climbing, diving, landing, or descending;
- same cell and same layer but not melee: charge, move, or fire-and-advance into melee depending on strategy.

The shared path filter rejects impossible and fall exits. It rejects swim-only exits for characters who would abjectly fail swimming, while allowing swim-capable characters to path through water. It also rejects fly-only exits unless the character is already flying or can take flight.

Zero-gravity movement is enforced by `CanMove(exit, ...)` and movement helpers. Combat strategies should prefer normal movement APIs rather than bypassing them.

## Forced Positioning Moves

Pushback attacks are authored weapon or natural attacks (`Pushback`, `PushbackUnarmed`, and `PushbackClinch`). They perform the normal attack roll and then an opposed `PushbackCheck` versus `OpposePushbackCheck`. On success they clear melee, clinch, and grapple contact between the attacker and target, remove the target from melee range for all combatants currently pressing them, and apply an outcome-scaled combat delay before the target can charge or otherwise re-engage. On a near miss, the target may still receive a smaller delay.

Forced movement attacks are authored as `ForcedMovement`, `ForcedMovementUnarmed`, or `ForcedMovementClinch` and store their configuration in `WeaponAttack.AdditionalInfo`. Builders choose the supported destination type (`Exit`, `Layer`, or both), supported verb (`Shove`, `Pull`, or both), required range (`Melee`, `Clinch`, or `Grapple`), and resist difficulty. Manual commands and strategies select only attacks that match the current verb, destination type, and range.

Shove moves only the victim. It breaks melee, clinch, and grapple contact and then lets fall logic resolve unsafe air or tree placements. Pull moves both attacker and victim. It preserves melee range, clinch, and existing grapple effects where the underlying movement remains valid, which is what allows crocodile-style dragging and dropper-style carrying to maintain control.

Forced exit movement still enforces hard movement rules: the target-side exit must exist in the target's current location and layer, fall exits are rejected, zero-gravity movement must be possible, size limits apply, fly/climb-only exits require matching capabilities, and `CanCross` must approve the transition. Victim safe-movement warnings are ignored so hostile forced movement can pass through destinations a voluntary mover would sensibly avoid; hard impossibility is not ignored.

Forced layer movement requires the destination layer to exist in the current terrain. Pulling requires the attacker to be able to transition to that layer. Shoving can place the victim in layers such as water, trees, or air even if they could not voluntarily transition there, but the existing floating and fall checks run immediately afterwards.

## Auxiliary Move Effects

Auxiliary combat actions are non-attack combat moves stored as `CombatAction` records with `MoveType = AuxiliaryMove`. Their effect configuration remains in `CombatActions.AdditionalInfo` XML, so adding or changing effect types does not require a database migration.

The current effect types are:

- `attackeradvantage`: applies offensive and defensive advantage or penalties to the attacker from an opposed defense check.
- `defenderadvantage`: applies offensive and defensive advantage or penalties to the defender from an opposed defense check.
- `targetdelay`: delays the target's next combat action. The stock default is 1.5 seconds plus 0.5 seconds per opposed success degree, capped at 6 seconds.
- `facing`: improves or worsens combat facing for the attacker or target. The stock default improves the attacker's position one step toward flank or rear.
- `targetstamina`: drains target stamina. The stock default is 3 stamina plus 1 per opposed success degree, capped at 10.
- `positionchange`: changes combat position. The stock default uses the existing combat knockdown flow and delays the target by 1.5 seconds plus 0.5 seconds per opposed success degree, capped at 5 seconds.
- `disarm`: knocks a wielded item from the target's hands. The stock default chooses the best disarmable wielded weapon and applies the existing `CombatNoGetEffect` / `CombatGetItemEffect` recovery flow for 90 seconds.

The new state-changing effect types share an opposed-resolution configuration: defense trait, defense difficulty, minimum opposed outcome degree, flat amount, per-degree amount, maximum cap, and optional success/failure echoes. Builders can inspect effect syntax with `auxiliary set typehelp <effect>` and edit individual effects from the auxiliary action builder with commands such as `trait`, `difficulty`, `minimum`, `amount`, `perdegree`, `max`, `successecho`, `failureecho`, and `clearecho`. Effect-specific commands add the remaining knobs, for example `facing subject`, `facing direction`, `positionchange knockdown`, `positionchange position`, and `disarm selection`.

State-changing auxiliary effects apply only when the move has a character target. Item or other perceivable targets are safely ignored by these effects rather than trying to mutate character-only combat state.

## Audit Notes

The first audit pass focuses on these robustness concerns:

- inverted stamina or capability predicates that turn viable action into helpless defense;
- ranged strategies losing defensive coverage against attack move families they can plausibly receive;
- path filters that reject viable movement or accept unsafe movement;
- strategy fallbacks that set a better strategy mode but idle for a tick instead of immediately choosing the new strategy's move;
- weighted attack rolls where a selected channel has no legal move but another configured channel does.

Implemented fixes from this pass:

- `StandardMeleeStrategy` now dodges start-clinch attempts when standing and able to afford the stamina, rather than treating that viable state as helpless.
- `StandardMeleeStrategy` now applies the same "cannot dodge while mounted" predicate to magic power attacks that it already applies to ordinary weapon attacks.
- `RangeBaseStrategy` now delegates magic power attack defenses to the standard melee defense implementation instead of returning `null`.
- `RangeBaseStrategy` now immediately asks `FullAdvance` for a move after switching to it because no ranged attack is possible.
- `RangeBaseStrategy.GetPathFunction` now correctly rejects swim-only paths for abject swim failures and rejects fly-only paths for non-flying characters who cannot fly.
- `CombatStrategyMode.Swooper` is now described and classified as both a valid melee-mode and ranged-mode strategy, preserving persisted preferred strategy settings.
- `SwooperStrategy` now uses layer/flight movement and airborne skirmish responses instead of only attacking when it already happens to be in a valid swoop position.
- Pushback and forced-movement authored moves now give strategies and manual commands explicit tools for separating targets from melee, dragging or shoving targets through exits, and moving targets between terrain layers without bypassing zero-g, swimming, flying, exit, size, or fall mechanics.
- `Drowner`, `Dropper`, and `PhysicalAvoider` strategies now cover the principal predator/avoidance behaviours that previously required bespoke NPC scripting or could idle when movement layers became involved.

Remaining follow-up candidates:

- `CoveringFire` is still behaviourally identical to plain ranged fire. If suppression or ally-cover semantics are intended, it needs its own active behaviour.
- `FullCover` still attacks according to normal ranged attack percentages despite its name. That may be desired builder flexibility, but it is worth a naming or setting review.
- Weighted attack selection still may idle when the selected channel has no legal move but another configured channel could act. A broader change here should be made carefully because it changes combat pacing and probability semantics.
