# Multi-Target Combat Attacks

## Scope

Weapon attacks, natural attacks, ranged-natural attacks, and auxiliary combat actions can be authored to affect more than one hostile character. Multi-targeting is an orthogonal property rather than a separate move type, so it composes with the existing damage and control move families without multiplying the `BuiltInCombatMoveType` catalogue.

The supported combinations include:

- ordinary weapon, natural, and ranged-natural damage attacks;
- knockdown through `UnbalancingBlow` / `UnbalancingBlowUnarmed`;
- stagger through `StaggeringBlow` / `StaggeringBlowUnarmed`;
- stun through the attack's normal `StunExpression` damage profile;
- push through `Pushback` / `PushbackUnarmed`;
- pull into close contact through `PullToMelee` / `PullToMeleeUnarmed`;
- auxiliary actions using their existing `AuxiliaryEffects` inventory.

## Authoring and Persistence

`WeaponAttacks.MaximumTargets` and `CombatActions.MaximumTargets` store the inclusive target cap. Both database columns default to `1`, and runtime loading also clamps legacy or invalid values to at least one.

Weapon attacks use:

- `attack set multitargets <1-100>`
- `attack set targets <1-100>` as an alias, except on `BreathWeaponAttack`, where the older `targets` command continues to control the breath payload's additional-area-target limit.

Auxiliary actions use `auxiliary set targets <1-100>`. Setting the value to `1` preserves single-target behaviour.

`MaximumTargets` is copied by weapon-attack and auxiliary-action cloning and is included in their builder displays. The `MultiTargetCombatActions` EF migration adds the two columns.

## Target Eligibility

The explicitly selected target is always first. Additional targets must be hostile characters in the same combat, must pass the attack or auxiliary usability prog for that victim, and must not be allies of the assailant.

For ordinary melee weapon, natural, and auxiliary moves, an additional victim must:

- be colocated with the assailant;
- currently be targeting the assailant; and
- currently have melee range to the assailant.

This prevents an arc attack from reaching combatants who are merely present in the combat but not actually engaged with the creature.

Ranged-natural attacks may select visible hostile combatants despite melee spacing. Each additional victim must still be within the attack's authored `RangeInRooms`. `ScreechAttack`, `BreathWeaponAttack`, `ExplosiveNaturalAttack`, and `AquaticVehicleAttack` retain their built-in area/payload resolution and are not wrapped again, preventing multiplied area effects. The current conventional ammunition-based `IRangedWeapon` firing pipeline does not use `WeaponAttack` definitions and is therefore outside this field; this release's ranged multi-target support is the ranged-natural family.

Pull-to-melee is the deliberate contact exception. It may select colocated hostile combatants who are targeting the assailant but are not yet at melee range. After the damaging hit succeeds, `ForcedMovementCheck` opposes `OpposeForcedMovementCheck` at the attack's authored resist difficulty. Winning that contest sets melee range in each combat-target direction that exists between attacker and victim; resistance leaves the existing spacing unchanged.

## Resolution

`MultiTargetCombatMove` creates one ordinary child move for each selected target. Consequently:

- each target chooses and pays for an independent defense;
- damage, stun, knockdown, stagger, push, pull, and auxiliary effects resolve independently;
- target-specific combat messages and blood spray remain associated with that victim;
- arena scoring retains each child move's actual defender, defense response, and result;
- the attacker pays the authored stamina cost once and receives one recovery delay;
- the weapon attack's `OnUseAttackProg` fires once with the primary target.

Targets that have left the combat before resolution are skipped. The combined result reports success when any child succeeds and retains all wounds from the individual results.

## Strategy Integration

All strategy and manual-command paths that create weapon, natural, or auxiliary moves route through the multi-target wrapper. Existing strategy percentages and attack weighting are unchanged.

Ranged strategies additionally consider `PullToMelee` weapon attacks and `PullToMeleeUnarmed` natural attacks against colocated targets outside melee. Melee strategies recognise the natural pull type so a creature can use an authored sweeping pull while already engaged.

## Seeded Examples

The combat seeder supplies the following examples. Fresh installs receive the full catalogue; reruns add missing weapon definitions, repair maintained weapon, animal, and auxiliary target caps, and retag legacy Tendril Lash combat messages to the pull move type:

- quarterstaff: three-target sweep, three-target whirling damage strike, and two-target hook-and-pull;
- animals and large creatures: three-target `Massive Claw Sweep` linked only to big-cat and bear loadouts, three-target knockdown `Tusk Sweep`, three-target stagger `Tail Slap`, three-target `Animal Barge Pushback`, four-target `Tendril Lash` pull, and four-target ranged `Wing Buffet`;
- auxiliary actions: multi-target dragon wing shadow, gryphon buffet, hydra many-head feint, and myconid spore cloud.

Existing race and mythical-creature attack links continue to provide these repaired definitions wherever those attacks were already appropriate.
