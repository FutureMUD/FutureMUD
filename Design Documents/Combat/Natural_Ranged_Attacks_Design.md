# Natural Ranged Attacks Design

Status: **Ready for Release**
Release review completed: 14 July 2026

## Overview

This document defines the natural ranged attack family and the supporting elemental contact systems added for the natural-ranged combat release. The implementation is data-driven and reuses `WeaponAttack` definitions so the same runtime can support both natural attacks and selected weapon-based attacks where appropriate.

## Attack Families

All new attacks are persisted as `WeaponAttack` definitions with specialised `AdditionalInfo` payloads:

- `RangedNaturalAttack`
  - Single-target ranged natural attack
  - Uses ranged-natural combat checks and ranged defenses
  - Carries `RangeInRooms` and `ScatterType`
- `BreathWeaponAttack`
  - Anchors on a primary target
  - Applies area damage to nearby victims
  - Supports multi-bodypart hits and optional ignition
- `SpitNaturalAttack`
  - Delivers a liquid mixture instead of ordinary damage
  - Uses the liquid contamination system for on-hit and scatter fallout
- `ExplosiveNaturalAttack`
  - Resolves as a ranged hit, then emits explosive damage at the impact point
- `BuffetingNaturalAttack`
  - Uses ranged targeting, then applies forced movement and optional damage

## Swooping Breath

Swooping breath is a combat move, not a stored attack type. It reuses a normal `BreathWeaponAttack` definition and adds:

- ingress from another room or layer
- a breath attack at the point of contact
- immediate egress where possible

The runtime move is `BreathSwoopAttackMove` and the strategy surface is `SwooperStrategy`.

## Shared Combat Surfaces

### Interfaces

New library interfaces:

- `IRangedNaturalAttack`
- `IBreathWeaponAttack`
- `ISpitAttack`
- `IExplosiveRangedAttack`
- `IBuffetingRangedAttack`
- `IRangedAttackMove`
- `IFireProfile`
- `ILiquidSurfaceReaction`

### Weapon Attack Hooks

`IWeaponAttack` now exposes `OnUseAttackProg`. This prog fires after a move is committed and resolved, with parameters:

1. attacker character
2. weapon item or `null`
3. primary target character or `null`

The return value is ignored. This is intended for cooldowns, state-setting, combat telemetry, and similar side effects.

## Persistence

### Weapon Attacks

`WeaponAttacks` now store:

- `OnUseProgId`
- `AdditionalInfo` XML for the new ranged-natural subclasses

### Gases

`Gases` now store:

- `OxidationFactor`

This value controls whether fire profiles that are not self-oxidising can persist in a cell atmosphere.

### Liquids

`Liquids` now store:

- `SurfaceReactionInfo`

This XML payload stores tag-driven surface reactions used by contamination ticking.

## Fire System

Fire is represented by:

- `FireProfile`
  - name
  - damage type
  - damage / pain / stun per tick
  - thermal load per tick
  - spread chance
  - minimum oxidation
  - self-oxidising flag
  - extinguish tags
- `OnFire`
  - scheduled effect that applies ongoing damage
  - pushes thermal imbalance upward
  - stops automatically if the atmosphere cannot sustain the fire
  - can spread to nearby perceivables

The initial implementation is combat-led and profile-driven. It uses atmospheric oxidation and scheduled burning damage, and integrates with `ThermalImbalance`.

## Corrosive / Contact Liquid Reactions

Liquid surface reactions are tag-based:

- each liquid can define one or more `ILiquidSurfaceReaction` entries
- each reaction matches one or more target tags
- matching reactions apply chemical or other damage through the surface-liquid exposure strategy while wetness or drying is resolved

The current implementation hooks into:

- item and body `SurfaceLiquidState` exposure
- drying and residue creation
- virtual room-layer liquid state for puddles and weather accumulation

This keeps corrosion inside the shared surface-contamination runtime. The legacy `LiquidContamination` and `BodyLiquidContamination` saved effects are ignored on load, while `WeaponPoisonCoating` remains a separate combat-delivery effect.

## AI and Strategy

Natural ranged attacks are considered by ranged strategies through `AttemptUseRangedNaturalAttack`.

`SwooperStrategy` prefers breath swoops when the combatant:

- is flying
- has a usable `BreathWeaponAttack`
- has a target in an adjacent room or another reachable layer

Otherwise it falls back to ordinary ranged-natural or standard ranged logic.

## Builder Surface

### Shared Ranged Fields

All ranged-natural attack definitions expose:

- `range <rooms>`
- `scatter <arcing|ballistic|light|spread>`

### Specialised Fields

- `BreathWeaponAttack`
  - `targets <number>`
  - `bodyparts <number>`
  - `ignite <percentage>`
  - `fire <option>`
- `SpitNaturalAttack`
  - `liquid <liquid>`
  - `amount <volume>`
- `ExplosiveNaturalAttack`
  - `explosionsize <size>`
  - `proximity <proximity>`
- `BuffetingNaturalAttack`
  - `push <rooms>`
  - `offadv <amount>`
  - `defadv <amount>`
  - `damage`

All attacks also expose:

- `onuse <prog|none>`

Breath fire profiles expose `name`, `type`, `damage`, `pain`, `stun`, `thermal`, `spread`,
`oxidation`, `selfoxidising`, `interval`, and `extinguish` subcommands. Liquid builders expose
`reaction add`, `reaction <#> delete`, and per-reaction `tag`, `type`, `damage`, `pain`, and `stun`
editing.

## Release Status

The supported release slice includes definitions, persistence, builders, character and item targets,
range/scatter resolution, ranged defenses, adjacent-room breath swoops, area effects, ongoing fire,
liquid-tag extinguishing, and contamination-driven corrosive reactions. Primary area targets are always
included, duplicate proximity entries are collapsed, and launched attacks use ranged penetration without
melee self-damage.

Stock seeding supplies llama and acid spit, dragonfire breath, wing buffet, tail spike, and bombardier
spray examples. The repeat-install path repairs missing catalogue entries and elemental metadata as well as
the fresh-install path. Seeded animal acid reacts with animal skin and seeded dragonfire is extinguished by
water-tagged liquids.

The implementation and its stock content are **Ready for Release**. Further creature profiles, richer
area geometry, and more specialised chemistry are post-release extensions.
