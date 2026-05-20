# Zero Gravity System

## Scope
This document describes the current zero-gravity runtime model.

Zero gravity is an environmental mechanic, not a continuous physics simulation. The first shipped version uses room-step drift, terrain defaults, room overrides, spells, anchors, tethers, and gas-fed RCS propulsion.

## Current Implementation
### Gravity Resolution
Gravity is resolved for a cell through a single helper:

1. Active `IGravityOverrideEffect` effects on the cell, ordered by priority.
2. The terrain default `ITerrain.GravityModel`.

`OverrideGravity` is the admin override effect. `SpellRoomGravityEffect` is the spell-created override. Terrain gravity persists as `Terrains.GravityModel`; stock terrain defaults are normal unless explicitly set to zero gravity.

Underwater and swimming layers suppress zero-gravity mechanics. Existing swimming, sinking, and underwater behaviour remains dominant even if the terrain or room effect says zero gravity.

### Terrain Defaults
`GravityModel` currently supports:

- `Normal`
- `ZeroGravity`

True space terrains such as orbital, interplanetary, interstellar, and intergalactic space are zero gravity by default. Lunar terrain remains normal gravity in this model; low gravity is future work, not zero gravity.

### Admin Commands
`zerog show|on|off|reset [cell]` controls the admin override:

- `on` forces zero gravity.
- `off` forces normal gravity.
- `reset` removes the admin override and returns to spell or terrain resolution.
- `show` reports terrain default, admin override, and resolved gravity.

`fixitem <item>` and `unfixitem <item>` add or remove `FixedInPlaceEffect`. Fixed items cannot be taken and count as zero-gravity push-off anchors.

## Movement Model
### Floating State
Characters in active non-underwater zero gravity can enter `PositionFloatingInZeroGravity`. This position uses:

- `MovementAbility.ZeroGravity`
- `MovementType.Floating`
- a runtime fallback move speed copied from flying, standing, or the first available body speed

The fallback speed lets existing body prototypes use the floating state even if old data has no explicit zero-g speed row.

### Deliberate Movement
Movement in active zero gravity requires at least one of:

- immwalk
- flight or levitation capability
- a wearable `IZeroGravityPropulsion` item with usable propellant
- an active `IZeroGravityTetherEffect`
- a fixed or component-backed anchor in the same layer
- an indoor or otherwise built surface to push from
- a ground, tree, or rooftop layer surface

Open-air or outdoor drifting without propulsion can therefore leave a character unable to move back down or correct course.

### Drift
`ZeroGravityDrift` represents inertia. It advances a character one room per scheduled tick in the same cardinal direction. It stops when:

- the exit is missing
- normal movement rejects the exit
- a tether blocks the destination
- the character is no longer in zero gravity
- the character is already moving

Version 1 emits stop/bump feedback but does not apply collision damage.

## Tethers
Tethers are effects implementing `IZeroGravityTetherEffect`.

The shared contract supports:

- an anchor perceivable
- optional physical backing item
- maximum room length
- route validation from the anchor to the destination
- taut-line blocking when movement would exceed range

Physical tether items use `ZeroGravityTetherGameItemComponent` plus the `tether` command. Magical tethers use `SpellZeroGravityTetherEffect` and have no physical backing item.

## Propulsion
`RcsThruster` is a wearable/connectable item component implementing `IZeroGravityPropulsion` and `IConnectable`.

It consumes gas from a connected `IGasSupply` item per `thrust` command. Gas containers can be connected through compatible connector types.

The player command is:

- `thrust <direction|up|down>`
- `thrust stop`

## NPC Safeguards
NPC path-following and wandering now check zero-gravity maneuverability before repeatedly attempting movement. NPCs without propulsion or a push-off point idle/fail the movement attempt once rather than retrying in loops. NPCs with flight, immwalk, propulsion, or a valid push-off point can continue using normal movement strategies.

## Future Work
Future versions may add:

- low-gravity terrain distinct from zero gravity
- collision damage
- orbital trajectories
- smarter NPC tactics
- richer tether manipulation
- item drift for loose objects across room boundaries
