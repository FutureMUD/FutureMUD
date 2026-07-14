# Natural Ranged Attacks Implementation Plan

Status: **Ready for Release**
Release review completed: 14 July 2026

## Completed Structure

### 1. Shared Library and Persistence

Implemented:

- ranged-natural attack, move, fire-profile, and liquid-reaction interfaces
- `BuiltInCombatMoveType` and `CheckType` values for all five attack families and breath swoops
- `IWeaponAttack.OnUseAttackProg`, fired after a committed move resolves
- `WeaponAttacks.OnUseProgId`, `Gases.OxidationFactor`, and `Liquids.SurfaceReactionInfo`
- migration `20260316112529_NaturalRangedAttacksAndElementalContact` and the matching EF snapshot

### 2. Combat Definitions and Moves

Implemented:

- `RangedNaturalAttack`, `BreathWeaponAttack`, `SpitAttack`, `ExplosiveRangedAttack`, and `BuffetingRangedAttack`
- the corresponding combat moves plus `BreathSwoopAttackMove`
- factory, ranged-defense, strategy, and combat-distance integration
- bounded range enforcement and scatter behavior for character and item targets
- distinct area victims with the primary target always included
- ranged penetration checks, correct damage/pain/stun expressions, and no melee-style self-damage on launched attacks
- safe one-cell swoop ingress/egress and the dedicated `BreathWeaponSwoop` check

### 3. Elemental Runtime and Builders

Implemented:

- profile-driven `OnFire` damage, heat, oxidation, spread, and scheduled ticks
- liquid-tag extinguishing on both bodies and items, including already-saturated surfaces
- tag-driven surface reactions through the shared surface-liquid state
- complete breath fire-profile editing for name, damage type, damage/pain/stun, heat, spread, oxidation, self-oxidising behavior, interval, and extinguishing tags
- liquid surface-reaction add/delete/tag/type/damage/pain/stun commands and builder display
- invariant XML number parsing and positive/bounded builder validation

### 4. Seeder Content

Implemented for both fresh and repeat-install paths:

- named `Sling` and `Blowgun` skills and check mappings
- sling, staff sling, blowgun, ammunition, and component-prototype stock definitions
- `Llama Spit`, `Acid Spit`, `Dragonfire Breath`, `Wing Buffet`, `Tail Spike`, and `Bombardier Spray`
- race/template assignments covering spit, breath, buffeting, ballistic, and explosive examples
- corrosive animal-acid surface metadata targeting animal skin
- water-tag extinguishing metadata for seeded dragonfire
- idempotent repair of missing natural-ranged catalogue and elemental payload metadata

### 5. Sling and Blowgun Runtime

Implemented and release-audited:

- load, ready, unready, aim, and fire through the shared ranged-weapon runtime
- damage construction through the generic loaded `Ammunition` component and its `AmmunitionType`; the weapon components do not own a second damage path
- ammunition recovery and poison transfer through the normal ammunition-hit pipeline
- sling readied stamina drain and hand/position restrictions
- blowgun breath, mouth, covering, and hidden-fire restrictions
- non-throwing load failure diagnostics for unexpected feasibility states

## Verification

Focused automated coverage now exercises:

- range and primary-target rules
- breath and explosive victim selection and deduplication
- fire-profile persistence and safe tick intervals
- extinguishing tags and liquid-reaction persistence
- ammunition-driven sling/blowgun damage ownership
- item-target natural ranged attacks
- resolved `OnUseAttackProg` ordering
- fresh/repeat seeder catalogue and acid/dragonfire metadata

The release-candidate verification commands and final counts are recorded in
`Design Documents/Core/MudSharp_2_0_Release_Readiness_Audit.md`.

## Post-Release Extensions

The following are optional extensions rather than release gaps:

- additional creature-specific natural-ranged profiles and combat prose
- richer cone/line geometry beyond the current bounded nearby-victim model
- more specialised liquid chemistry and fire-suppression interactions
- campaign-specific named skills or check remapping beyond the stock definitions
