# Natural Ranged Attacks Implementation Plan

## Completed Structure

### 1. Shared Library Surfaces

Implemented:

- new ranged-natural attack interfaces
- `IRangedAttackMove`
- `IFireProfile`
- `ILiquidSurfaceReaction`
- new `BuiltInCombatMoveType` values
- new `CheckType` values
- new `IWeaponAttack.OnUseAttackProg`

### 2. Persistence

Implemented:

- `WeaponAttacks.OnUseProgId`
- `Gases.OxidationFactor`
- `Liquids.SurfaceReactionInfo`

### 3. Core Combat Definitions

Implemented:

- `RangedNaturalAttackBase`
- `RangedNaturalAttack`
- `BreathWeaponAttack`
- `SpitAttack`
- `ExplosiveRangedAttack`
- `BuffetingRangedAttack`

### 4. Combat Move Runtime

Implemented:

- `RangedNaturalAttackMove`
- `SpitAttackMove`
- `BreathWeaponAttackMove`
- `ExplosiveNaturalAttackMove`
- `BuffetingRangedAttackMove`
- `BreathSwoopAttackMove`

Integrated:

- `CombatMoveFactory`
- ranged-defense selection for ranged-natural attacks
- `SwooperStrategy`
- `CombatStrategyFactory`

### 5. Elemental Runtime

Implemented:

- `FireProfile`
- `OnFire`
- `LiquidSurfaceReaction`
- `LiquidSurfaceReactionHelper`
- liquid contamination ticking hooks for corrosive reactions

### 6. Seeder / Check Defaults

Implemented:

- default check-template routing for the new ranged-natural checks in:
  - `SkillSeeder`
  - `SkillPackageSeeder`

## Remaining Follow-Up

The following work remains desirable to fully round out the release:

1. Add dedicated database migrations and update the EF snapshot.
2. Add richer builders for editing:
   - fire-profile payloads
   - liquid surface reactions
3. Expand the combat seeder, mythical animal seeder, and animal seeder with representative new attacks.
4. Add explicit skill definitions and starter package mappings for worlds that want named skills behind the new checks.
5. Add broader automated test coverage for:
   - ranged-natural scatter
   - breath area effects
   - fire spread/extinguish behavior
   - corrosive liquid reactions

## Recommended Next Sequence

1. Add migration `NaturalRangedAttacksAndElementalContact`.
2. Seed at least one breath creature, one spit creature, and one buffeting creature.
3. Add unit tests around:
   - `OnUseAttackProg`
   - `OnFire`
   - `LiquidSurfaceReactionHelper`
   - `SwooperStrategy`
4. Add combat message defaults for the new move types.
