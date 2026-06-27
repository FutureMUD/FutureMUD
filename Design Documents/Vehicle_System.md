# Vehicle System

## Purpose

FutureMUD vehicles use a hybrid model:

- The vehicle domain owns canonical authored and live state.
- The item system owns the visible, targetable exterior projection.

Every vehicle has one `IVehiclePrototype`, one live `IVehicle`, and one exterior `IGameItem` with a `VehicleExteriorGameItemComponent`. Players interact with the exterior item, but movement, occupancy, control stations, compartments, access state, and future route/coordinate behaviour belong to the vehicle domain.

This keeps vehicle logic out of component XML while preserving normal item-world affordances such as seeing, targeting, damaging, locking, towing, examining, and scripting against the exterior shell.

## Current Implementation Status

Phase 1 and the first five Phase 2 vehicle-systems slices are present:

- Vehicle domain interfaces: `IVehiclePrototype`, `IVehicle`, `IVehicleMovementStrategy`, `IVehicleMovementState`, `IVehicleOccupancy`, and `IVehicleAccessState`.
- Vehicle enums: `VehicleScale`, `VehicleLocationType`, `VehicleOccupantSlotType`, `VehicleMovementProfileType`, and `VehicleMovementStatus`.
- EF persistence for the Phase 1 vehicle prototype/live tables plus Phase 2 access points, cargo spaces, installation points, tow points, damage zones, damage-zone effects, tow links, and vehicle-zone wound links.
- Gameworld registries and loaders for vehicle prototypes and live vehicles.
- Vehicle factory creation of canonical vehicle instance plus exterior item projection.
- `VehicleExteriorGameItemComponentProto` and `VehicleExteriorGameItemComponent`.
- Projection components for access points, cargo spaces, and installable vehicle modules.
- `thing@vehicle` targeting for projected access/cargo/module items through the exterior item.
- `ItemScale` and `RoomContainer` authoring through compartments, slots, stations, and cell-visible exterior item projection.
- Player commands: `embark`, `disembark`, `drive`, and ordinary movement commands while controlling a vehicle.
- Admin/builder commands: `vehicleproto` for prototype authoring and creation, `vehicle` for live diagnostics and relinking.
- Cell-exit movement strategy with controller, location, profile, exit-size, transition, disabled/destroyed, closed-access, required installation, required role, fuel, power, and recursive tow-train validation.
- Tow-train service for hitch validation, cycle prevention, tow-point usage checks, recursive train weight checks, required `IHitchGear`/legacy `IDragAid` connector item validity, in-use item reservation, and loaded broken-link diagnostics.
- Active character/mount hitching through the normal drag movement system, including character-to-character chains and character/mount-to-vehicle tow-point hitches with physical connector gear for non-direct tow points.
- Tow point authoring includes a character pull multiplier that scales effective pull capacity for mounts, animals, or people pulling that vehicle point.
- Persistent mixed hitch graph foundation: `VehicleHitchEndpointType`, `IVehicleHitchLink`, `IVehicleHitchService`, `VehicleHitchLinks` EF schema, runtime invalid-link diagnostics, gameworld registry/load order, and reboot projection into active `CharacterHitch`/`Dragging` effects.
- Unified hitch graph traversal through `IVehicleHitchGraphService`, covering legacy `VehicleTowLinks`, persistent mixed `VehicleHitchLinks`, and live transient `CharacterHitch`/`Dragging` effects for validation, train weight, tow-point usage, movement preflight, hitch item relocation, and tow-stress evaluation.
- Operational readiness checks through `IVehicleOperationalReadinessService`, covering board/control/service/repair/hitch access, module condition, movement roles, fuel, power, repair hints, and tow-stress catastrophe mutation.
- `VehicleAccessStates` now provide simple persisted character grants without a new schema: `board`, `control`, `service`, `repair`, `hitch`, and `all` tags with level 1 boarding, level 2 operational actions, and level 3 all-action access.
- Admin `vehicle access <vehicle> list|grant|revoke` commands manage those persisted grants; no access rows remains permissive for compatibility, and existing projection lock/key behaviour is unchanged.
- Vehicle installable modules are condition-aware through `mincondition` and `movementcondition` XML fields with backward-compatible zero-threshold defaults.
- `vehicle show` includes mixed persistent hitch links involving that vehicle and reports invalid causes without requiring endpoint load success.
- Exterior item wound override that routes hull damage into vehicle damage zones and persists vehicle/zone ids on wounds.
- Damage-zone effects that disable linked access points, cargo spaces, installation points, tow points, movement profiles, or whole-vehicle movement at configured damage statuses.
- Admin damage repair for clearing canonical vehicle damage-zone wounds and status without clearing manual disabled flags.
- Occupancy reconciliation when a rider/driver is moved independently from the vehicle by teleport, transfer, arrest-like relocation, combat knockdown, grapple/drag movement, or other hardcoded cell-leave paths.
- Exterior relocation hardening for dragged or force-moved vehicle items, including carrying visible occupants with a still-cell-present exterior and clearing occupancy when the exterior is contained, inventoried, deleted, or destroyed.
- Reboot recovery through persisted vehicle location, movement status, exterior item id, occupancies, and projection resynchronisation on load.

The following areas are deliberately scaffolded rather than fully built:

- Rich physical access-device authoring, ownership, lease, and fleet policy systems beyond explicit persisted access rows and existing projection locks.
- Fuller fuel, power, electrical, and liquid network topology beyond installed candidate modules.
- Route, coordinate, and `RoomScale` movement.
- Dedicated FutureProg vehicle functions.
- Interior cell networks for `RoomScale` vehicles.

## Domain Model

### Vehicle Prototype

`IVehiclePrototype` is the canonical revisioned definition. It is not an item component and is not stored inside an item prototype.

Core fields:

- `Name`
- `Description`
- `Scale`
- `ExteriorItemPrototypeId`
- `ExteriorItemPrototype`
- `Compartments`
- `OccupantSlots`
- `ControlStations`
- `MovementProfiles`
- `AccessPoints`
- `CargoSpaces`
- `InstallationPoints`
- `TowPoints`
- `DamageZones`

Child definitions:

- `VehicleCompartmentPrototype` groups stations and occupant slots.
- `VehicleOccupantSlotPrototype` defines driver, passenger, and crew capacity.
- `VehicleControlStationPrototype` links vehicle control authority to a slot.
- `VehicleMovementProfilePrototype` declares supported movement strategy families.
- `VehicleAccessPointPrototype` defines doors, hatches, ramps, canopies, and service panels projected as targetable items.
- `VehicleCargoSpacePrototype` links a canonical cargo space to a targetable container projection item.
- `VehicleInstallationPointPrototype` defines installable module mount type, optional role, and movement-required slots.
- `VehicleTowPointPrototype` defines tow/towed capability, maximum towed weight, and the character pull multiplier used when a character or mount pulls a vehicle through that tow point.
- `VehicleDamageZonePrototype` defines weighted hit zones, damage thresholds, legacy movement-disable behaviour, and authored damage effects.
- `VehicleDamageZoneEffectPrototype` links a damage zone to a target system family and optional prototype child id.

Submit/create validation currently requires:

- an exterior item prototype
- an exterior item prototype with a `VehicleExteriorGameItemComponentProto` linked to this vehicle prototype
- at least one occupant slot
- at least one driver slot
- at least one `CellExit` movement profile
- valid projection item prototypes for access and cargo definitions
- cargo projection item prototypes that include a normal container component
- valid installation mount types, damage-zone thresholds, and damage-zone effect targets

### Vehicle Instance

`IVehicle` is the canonical live instance. It persists independently from the exterior item.

Core fields:

- vehicle prototype id and revision
- exterior item id
- canonical location state
- room layer
- movement state
- occupancies
- access state
- access points, cargo spaces, installations, tow links, and damage zones

The live vehicle is the source of truth for:

- where the vehicle is
- whether it is moving, stationary, or disabled
- who is aboard
- who controls it
- which slot each occupant occupies
- whether the item projection needs repair or synchronisation
- which projected access/cargo/module items belong to the vehicle
- whether systems are disabled or destroyed

### Item Projection

The exterior item is a normal `IGameItem` instance whose item prototype includes `VehicleExteriorGameItemComponentProto`.

The component exists only as a bridge:

- Prototype XML stores the linked vehicle prototype id.
- Runtime component XML stores the linked live vehicle id and repair notes.
- Canonical vehicle state never lives in the item component.

The component's prototype sets `PreventManualLoad = true`. This prevents builders or item-load commands from creating orphan vehicle shells through normal item loading. The `vehicleproto set exterior <item proto>` command automatically creates and attaches the required internal component when it links the exterior item prototype.

An occupied exterior item cannot be picked up or otherwise normally repositioned through the item inventory flow, and cannot be hauled into a container while occupied. Dragging an occupied exterior through an exit is allowed when the normal drag rules allow it; the item force-move hook then reconciles the canonical vehicle and moves visible occupants with the exterior.

If an exterior item is forcibly moved through the item force-move hook, the vehicle reconciles canonical state from the exterior item: if the item is still in a cell, visible occupants are moved with it and vehicle location state is updated; if the item has been put into inventory, contained, deleted, destroyed, or otherwise removed from cell presence, all occupants are forcibly disembarked and their vehicle occupancy rows are cleared. Exterior item deletion/destruction therefore fails safe by removing occupants from vehicle occupancy rather than leaving them linked to a missing shell.

If a character aboard an item-scale or station-style vehicle is moved independently from the exterior item by teleport, transfer, arrest-like relocation, grapple dragging, ordinary forced movement, or a combat knockdown, the character is forcibly disembarked as they leave the exterior item's cell. This prevents stale occupancy and controller links while letting the character movement continue. Voluntary position changes while aboard a vehicle are blocked; forced position changes disembark first and then continue.

## Persistence

Vehicle persistence is EF-backed.

Prototype tables:

- `VehicleProtos`
- `VehicleCompartmentProtos`
- `VehicleOccupantSlotProtos`
- `VehicleControlStationProtos`
- `VehicleMovementProfileProtos`
- `VehicleAccessPointProtos`
- `VehicleCargoSpaceProtos`
- `VehicleInstallationPointProtos`
- `VehicleTowPointProtos`
- `VehicleDamageZoneProtos`
- `VehicleDamageZoneEffectProtos`

Live tables:

- `Vehicles`
- `VehicleCompartments`
- `VehicleOccupancies`
- `VehicleAccessStates`
- `VehicleAccessPoints`
- `VehicleAccessPointLocks`
- `VehicleCargoSpaces`
- `VehicleInstallations`
- `VehicleTowLinks`
- `VehicleHitchLinks`
- `VehicleDamageZones`

The initial migration is `VehiclesHybridModel`; Phase 2 system tables are added in `VehicleSystemsPhase2`.

Important persistence rules:

- A vehicle prototype references an exterior item prototype by item prototype id and revision.
- A live vehicle references its exterior item by live game item id.
- A live vehicle has a one-to-one external projection by convention and database uniqueness on `Vehicles.ExteriorItemId`.
- Occupants are persisted in `VehicleOccupancies`; they are never stored inside item component XML.
- Movement recovery fields are persisted on `Vehicles`: current cell, room layer, movement status, current exit, destination cell, and movement profile.
- `VehicleExteriorGameItemComponent.VehicleId` is a repairable bridge value, not the source of truth.
- Access and cargo projection components store only repairable bridge ids; their canonical state lives on the vehicle records.
- Cargo contents remain ordinary item/container state on the cargo projection item.
- `VehicleTowLinks` persist vehicle-to-vehicle tow links with vehicle-only endpoints and the optional hitch item id. Non-direct tow point types now require a valid connector item; old non-direct links without one remain diagnosable but cannot move until repaired or unhitched.
- `VehicleHitchLinks` are the canonical persistent graph for mixed character/vehicle hitching when every character endpoint is an NPC. PC-inclusive hitches remain transient runtime links. Persistent NPC hitches recover only when any required hitch item still exists, is compatible, and remains with the chain.
- Vehicle damage-zone wounds are ordinary wound records with optional `VehicleId` and `VehicleDamageZoneId`.
- Damage-zone effects are prototype state. Live system disablement from damage is derived at runtime from canonical zone status, so manual disabled flags remain separate.

## Load Order

Current load order is:

1. Item component prototypes.
2. Item prototypes.
3. Vehicle prototypes.
4. World items.
5. Characters/NPCs.
6. Live vehicles.
7. Persistent vehicle hitch graph finalisation.

This order allows vehicle prototypes to resolve item prototypes, live vehicles to resolve exterior items, and vehicle occupancies to resolve characters.

When vehicles load, they call `SynchroniseExteriorItemToLocation()` so the exterior item projection returns to the vehicle's canonical cell and room layer after reboot.

Persistent hitch links are loaded after vehicles, world items, and NPCs are available. Endpoints resolve lazily and invalid rows remain diagnosable rather than crashing boot. Missing vehicles, NPCs, tow points, required hitch items, incompatible hitch gear, or co-location failures make the link invalid until repaired or unhitched. Valid NPC/vehicle links are projected back into active `CharacterHitch` and `Dragging` effects at boot so ordinary movement continues to move the hitch chain. PC endpoints are not persisted; any PC-inclusive hitch is treated as a transient runtime hitch and blocks voluntary quit or timeout while active.

## Creation Flow

Builder flow:

1. `vehicleproto new <name>`
2. `vehicleproto set scale <itemscale|roomcontainer|roomscale>`
3. `vehicleproto set exterior <item proto>`
4. `vehicleproto set compartment add <name>`
5. `vehicleproto set slot add <compartment id> <driver|passenger|crew> <capacity> <name>`
6. `vehicleproto set station add <slot id> <name>`
7. `vehicleproto set movement cell`
8. `vehicleproto submit <comment>`
9. `vehicleproto approve <id> <comment>`
10. `vehicleproto create <id|name>`

Factory flow:

1. Validate the vehicle prototype.
2. Create the exterior game item from the linked item prototype.
3. Insert the exterior item into the target cell and room layer.
4. Create the `Vehicles` row.
5. Create live `VehicleCompartments` rows.
6. Create live access point, cargo space, installation, and damage zone rows.
7. Load the live `Vehicle`.
8. Link `Vehicle.ExteriorItemId` to the item.
9. Link `VehicleExteriorGameItemComponent.VehicleId` to the vehicle.
10. Create projected access and cargo items from their configured item prototypes.
11. Link projection item components back to their live vehicle system records.
12. Fire normal item-finished-loading behaviour.
13. Flush saves.

## Player Interaction

Players do not normally target `Vehicle` ids. They target the exterior item.

Current player commands:

- `embark <vehicle>`
- `embark <vehicle> <slot id|driver|passenger|crew>`
- `embark <vehicle> [slot] via <access id|name>`
- `disembark`
- `drive <direction>`
- ordinary movement commands such as `north`, `east`, `enter`, or `leave` while controlling a vehicle
- `install <held module> <vehicle> [install point]`
- `uninstall <module@vehicle>`
- `hitch <towpoint>@<vehicle> <towpoint>@<target> [with <item>]`
- `hitch <character> <character|towpoint@vehicle> [with <drag aid>]`
- `unhitch <vehicle>`
- `unhitch <towpoint>@<vehicle>`
- `unhitch <character>`

Projected vehicle system items are exposed through ordinary item targeting. The preferred form is `thing@vehicle`, for example `open hatch@car`, `unlock hatch@car`, `put crate trunk@car`, and `get crate trunk@car`. The older generic attached-item form `vehicle@thing` is also accepted for compatibility, but vehicle documentation and builder examples should use `thing@vehicle`.

Boarding rules currently check:

- the target item has a linked vehicle exterior component
- the actor is not already aboard any vehicle
- the actor is in the vehicle's canonical cell
- the actor is on the vehicle's canonical room layer
- the requested slot exists
- the slot has remaining capacity
- operational readiness access grants allow the actor when access rows exist
- if access points exist, an open, unlocked, enabled access point must reach the selected slot

Driving rules currently check:

- the actor is the vehicle controller
- operational readiness access grants allow control when access rows exist
- the actor is in the same canonical cell and room layer as the vehicle
- the vehicle is at the exit origin
- the vehicle prototype has a `CellExit` movement profile
- the movement profile is not disabled by vehicle damage effects
- the exit is large enough for the exterior item
- the exit has a viable movement transition for the driver
- the vehicle is not disabled or destroyed
- required access points are closed
- required installed modules and roles are present, correctly typed, enabled, not destroyed, and above their movement condition thresholds
- configured fuel and power are available from functional installed candidate modules
- recursive tow-train links are valid, tow points are not damage-disabled, hitch items are co-located, all towed vehicles fit through the exit, and any valid strained link survives the tow-stress catastrophe preflight

When a controller enters an ordinary movement command, character movement redirects it to vehicle movement before walking movement is attempted. This means a bicycle rider can type `north` instead of `drive north`; the explicit `drive` command remains available for clarity. Vehicle movement uses the normal movement pipeline shape: it sets the actor's current `IMovement`, applies a movement delay, supports turn-around cancellation and queued follow-up movement commands, marks the vehicle as moving while in transit, and resolves the movement after the scheduled step.

Visible occupants of a vehicle are presented in the same style as mounted riders. If a character is visibly occupying a vehicle whose exterior item is also visible in the same cell and layer, their long description says they are riding that vehicle, and the exterior item is suppressed from the separate item list to avoid duplicate room lines. If occupants are not visible in the cell, the exterior item remains the room-facing presentation.

### Character And Mount Hitching

Character/mount hitching is implemented as an active movement-state bridge over the normal `Dragging` effect rather than as a persisted vehicle tow link. This supports cases such as a horse pulling a cart, an ox pulling a wagon, a person pulling a rickshaw or tuk-tuk, and short character-to-character hitch chains.

Supported forms:

- `hitch <character> <towpoint@vehicle>`
- `hitch <character> <towpoint@vehicle> with <drag aid>`
- `hitch <character> <character>`
- `hitch <character> <character> with <drag aid>`
- `unhitch <character>`
- `unhitch <vehicle>`
- `unhitch <towpoint@vehicle>`

For vehicle targets, the target tow point must be authored as `towed` or `both`, must not be damage-disabled, must pass hitch access checks, and the vehicle must be in the same cell and layer. Tow point types `hand`, `manual`, `direct`, `none`, and `pull` are treated as direct hitches that do not require a connector item. Other tow point types, such as `hitch`, `yoke`, `harness`, `rope`, or `chain`, require `with <item>` and the named item must expose `IHitchGear` or legacy `IDragAid`. New content should use `IHitchGear` so the item declares a compatible role such as tow bar, yoke, harness, rope, chain, traces, or lead rope.

Pull capacity is:

```text
(source maximum drag weight - source external carried/worn weight)
* optional drag-aid effort multiplier
* vehicle tow-point character pull multiplier
```

The multiplier is represented internally by dividing the vehicle exterior's effective pulled weight by the tow point multiplier. This deliberately reuses character and mount drag capacity so race/body tuning for animals immediately affects vehicle pulling.

Character-to-character links can form chains, so a leader can pull or lead another character/mount that is itself pulling a cart. A target can only have one incoming drag/hitch relationship at a time, and a source can only actively pull one target at a time. The actor can hitch themselves, a source that trusts them as an ally, a helpless source, or a mount they can currently control or mount. Other conscious sources receive an `accept` proposal before the hitch is applied, and the command revalidates location, capacity, hitch item availability, and vehicle tow-point state when the proposal is accepted. Eligible NPC-only character hitch chains are persisted through `VehicleHitchLinks`; PC-inclusive hitches remain active runtime effects only. While a hitch is active, the connector item receives a no-get in-use effect and is released by `unhitch`, `stop`, link invalidation, or effect cleanup. For all character hitches, a hitch item must be in the chain location or on an endpoint, and actor-held hitch items are silently placed with the chain before the persistent link or transient runtime effect is created.

### Persistent Hitch Graph Plan

Status: implemented for NPC/vehicle and NPC/NPC persistent character-root hitches, plus unified runtime traversal across legacy vehicle tow links, persistent mixed hitch links, transient live drag/hitch effects, tow-stress evaluation, and movement completion helpers.

The Phase 2 runtime now has a shared directed hitch graph that covers vehicle-to-vehicle, character-to-vehicle, character-to-character, and eventually vehicle-to-character links with one validation service. The graph preserves the existing live command syntax, keeps the existing persistence tables, and moves eligible NPC/vehicle character-root hitches out of purely transient `CharacterHitch` and `Dragging` effects.

Canonical persistent links use typed endpoints:

- `SourceType`: vehicle or character.
- `SourceVehicleId` / `SourceCharacterId`: one is populated according to source type.
- `SourceTowPointPrototypeId`: required only for vehicle sources.
- `TargetType`: vehicle or character.
- `TargetVehicleId` / `TargetCharacterId`: one is populated according to target type.
- `TargetTowPointPrototypeId`: required only for vehicle targets.
- `HitchItemId`: optional physical hitch reference; required for non-direct vehicle tow point types and validated against `IHitchGear` roles where present.
- `IsDisabled`: manual/admin disabled state.
- `CreatedDateTime`: diagnostics and repair context.

PC endpoints are intentionally not persisted. If either endpoint is a PC, the hitch remains transient and applies a no-quit/no-timeout guard while active. On crash or reboot those PC-inclusive hitches are safely lost rather than attempting to resurrect offline player state. NPC and vehicle endpoints persist because they load into the world at boot and can be resolved after the normal vehicle/NPC/item loaders have run.

The persistent hitch service owns or has implemented the first slice of:

- endpoint resolution, `vehicle show` display, and invalid-link diagnostics
- source and target usage checks
- hitch item validation and co-location checks, including hitch items worn or carried by an endpoint; actor-held hitch items are placed with the chain before persistent links are saved
- conversion between canonical links and runtime projection effects used for presentation, dragging, no-quit, and no-timeout behaviour
- runtime graph wrapping for legacy `VehicleTowLinks`, persistent mixed `VehicleHitchLinks`, and transient `CharacterHitch`/`Dragging` effects
- recursive train discovery, cycle checks, train-weight validation, duplicate tow-point checks, and movement preflight for vehicle-root and character-dragged vehicle movement

Still to complete around the unified service:

- hitch consent through trust-ally, mount-control/mountable logic, helplessness, or `accept` as service-level rules rather than command-local rules
- richer hitch item mechanics, richer catastrophe tuning surfaces, and broader recovery UX

The implementation sequence is:

1. Implemented: add endpoint enums, `IVehicleHitchLink`, EF model, and migration.
2. Implemented: add a central hitch registry/service loaded after vehicles, NPCs, and items.
3. Implemented: refactor vehicle-to-vehicle tow traversal to read through the central graph while preserving existing `VehicleTowLinks`.
4. Implemented: persist NPC/vehicle and NPC/NPC character hitches after consent or direct-authority validation.
5. Implemented: keep PC-inclusive hitches transient and add quit/timeout guards while active.
6. Implemented for cell-exit movement: character-root dragged vehicle exteriors and vehicle-root driving both preflight through the graph and move downstream vehicles/hitch items.
7. Implemented for current scope: `vehicle show` reports mixed hitch links, graph-derived invalid causes, tow stress, and operational readiness; `unhitch` removes persistent mixed links by character, vehicle, or tow point; admin `vehicle repair <vehicle> hitch <link|all>` can re-enable validated persistent links after repair.
8. Partially implemented: missing-endpoint, PC-transient, hitch-item, cycle, recursive train, tow-stress, access, module-condition, and unified movement regression tests are present; reboot recovery tests remain planned.

### Operational Readiness And Repair

Status: implemented for the cell-exit vehicle slice.

`IVehicleOperationalReadinessService` is the shared runtime preflight for vehicle operation. It is intentionally narrow: it does not add new access tables, new repair commands for players, route movement, coordinate movement, or `RoomScale` behaviour. Instead, it normalises existing access rows, projection locks, install points, damage zones, hitch graph links, fuel modules, power modules, and repair-kit flows into one set of actionable diagnostics.

Access readiness uses `VehicleAccessStates` as character grants:

- no access rows means the legacy permissive behaviour remains in place
- administrators bypass operational access checks
- `board` at level 1 permits boarding
- `control`, `service`, `repair`, and `hitch` at level 2 permit driving/control handoff, install/uninstall, repair preflight, and hitch/unhitch operations respectively
- `all` or any level 3 row permits all operational vehicle actions

This access model is checked by boarding, control/driving, install/uninstall, hitch/unhitch, and vehicle exterior repair preflight. It does not replace normal lock/key behaviour on projected access items; locked hatches still behave like locked hatches.

Module readiness extends `IVehicleInstallable` with `MinimumFunctionalCondition`, `MinimumMovementCondition`, `IsFunctional`, and `IsFunctionalForMovement`. Existing component XML remains valid because missing condition fields default to `0%`. New builders can set `mincondition <percent>` and `movementcondition <percent>` on the vehicle installable component prototype. Movement-required modules and required roles count only when the install point is enabled, the module item exists, the installable role matches, the item is not deleted or destroyed, and item condition meets the configured movement threshold.

Fuel and power readiness reports every installed candidate considered by movement. Candidates can fail for disabled install points, missing modules, low condition, wrong fuel, insufficient volume, switched-off/no-power state, or insufficient spike capacity. Fuel and power are consumed only after access, movement, tow, and catastrophe preflight succeeds.

Player-facing repair reuses the existing item repair command: `repair <vehicle exterior> with <kit>`. When vehicle exterior wounds are repaired or removed, all linked damage zones recalculate from their remaining wounds and their status downgrades back toward functional according to the authored damage thresholds. Admin `vehicle repair <vehicle> damage <zone|all>` clears selected vehicle-zone wounds through the same recalculation helpers and still preserves manual disabled flags. Admin `vehicle repair <vehicle> hitch <link|all>` re-enables catastrophe-disabled persistent tow or hitch links only when graph validation passes.

Tow catastrophe is only evaluated for valid but strained graph links. Hard invalid states still fail closed before movement: cycles, missing endpoints, missing/destroyed/incompatible gear, duplicate incoming links, duplicate tow-point use, over maximum towed weight, damage-disabled tow points, and blocked exits. Valid links warn at 90% of effective capacity; failure rolls begin at 95% and scale to a maximum 25% chance at capacity. A catastrophe aborts the movement before fuel or power consumption, echoes the failure, damages the hitch item when available or the linked vehicle exteriors otherwise, disables persistent links, clears transient hitch/drag effects, and releases reserved hitch items. Recovery is deliberately explicit: repair damaged items or vehicle exteriors, then `unhitch` and re-hitch, or use admin `vehicle repair <vehicle> hitch <link|all>` for validated persistent links.

`vehicle show` now includes an operational readiness section with access, modules, fuel, power, damage, repair hints, invalid projection/link reasons, and tow-stress warnings.

## Vehicle Scales

### ItemScale

Implemented for the first slice.

`ItemScale` vehicles are small or normal item-sized vehicles. They have an exterior item and occupancy/control state, but they do not require interior cells.

Examples:

- bicycle
- motorcycle
- small cart
- wheelchair
- ridable drone

### RoomContainer

Implemented for the first slice as compartment/slot/station state without interior cell movement.

`RoomContainer` vehicles are represented externally by one item in a cell, but they can contain multiple authored compartments, passenger slots, and control stations.

Examples:

- car
- wagon
- small boat
- armoured personnel carrier

The current implementation tracks distinct slots and stations, moves all occupants with the exterior vehicle, and keeps the exterior item visible in the canonical cell.

### RoomScale

Planned.

`RoomScale` vehicles will support large moving interiors, docking or attachment behaviour, and eventually coordinate or route movement. They are not implemented in this slice beyond the enum value and prototype scale.

## Cell-Exit Movement

`CellExitVehicleMovementStrategy` is the first completed movement strategy.

It moves a vehicle between adjacent cells through normal exits.

Current validation:

- actor must be the controller and must have control access when explicit access rows exist
- actor must be in the same canonical cell and room layer as the vehicle
- vehicle must be at the exit origin
- vehicle prototype must support `CellExit`
- movement profile must not be damage-disabled
- exterior item must fit through the exit's maximum size
- exterior item components must not block movement
- exit transition must be viable
- vehicle must not be disabled or destroyed
- configured access points that must be closed are closed
- required modules and roles must be installed, enabled, correctly typed, not destroyed, and above their movement condition thresholds
- fuel and power candidates must be functional and have the required resource or spike capacity
- recursive tow-train links must be graph-valid, fit through the exit, remain within hard tow capacities, and survive any tow-stress catastrophe roll

Current movement behaviour:

- create a vehicle `IMovement` for player-driven movement commands
- emit begin/departure echo that names visible riders for item-scale/station vehicles, or the vehicle itself when occupants are not visible
- emit arrival echoes as riding language for visible riders, for example `rides in from the south on <vehicle>`, rather than character-arrival wording glued to vehicle wording
- mark vehicle as `CellExitTransit` and `Moving`
- persist transit state before movement
- schedule the movement delay through the normal movement scheduler
- move the exterior item to the destination cell and target layer
- invoke exterior `IConnectable` force-move cleanup so cables, chargers, and similar independent connections do not remain logically connected across cells
- move co-located occupants to the destination cell and layer as participants in the vehicle movement, clearing any stale occupancy records that are no longer co-located with the vehicle
- consume configured fuel and power only after all access, movement, tow, and catastrophe preflight succeeds
- move all recursively towed vehicles, hitch items, and occupants exactly once through the unified hitch graph
- mark vehicle as `Cell` and `Stationary`
- clear current exit and destination fields
- emit destination echo with the same visible-rider versus vehicle-only distinction

This intentionally keeps route validation and vehicle state changes in the vehicle movement strategy, while player-driven cell-exit driving is represented as an `IMovement` so it cooperates with movement delay, movement blocking, queued movement commands, group movement state, and room movement diagnostics.

## Admin Diagnostics

`vehicle` commands inspect and repair live instances.

Current commands:

- `vehicle list`
- `vehicle show <id|name>`
- `vehicle access <id|name> list`
- `vehicle access <id|name> grant <character> <board|control|service|repair|hitch|all> <1-3>`
- `vehicle access <id|name> revoke <character|row id> [tag]`
- `vehicle repair <id|name>`
- `vehicle repair <id|name> damage <zone|all>`
- `vehicle repair <id|name> hitch <link|all>`
- `vehicle relink <id|name> <item id|local item>`

`vehicle show` distinguishes canonical vehicle state from item projection state:

- prototype and scale
- canonical location, layer, movement status, transit fields
- exterior item id
- loaded exterior item
- component `VehicleId`
- projection synchronisation status
- current occupants and controller
- access point, cargo, installation, tow-link, hitch-link, and damage-zone state
- manual versus damage-derived disable causes for projected systems

`vehicle repair` restores the bidirectional vehicle/item component link for the current exterior item and synchronises the exterior item to the canonical vehicle location.

`vehicle repair <vehicle> damage <zone|all>` clears the selected vehicle damage-zone wounds and recalculates damage status. It does not clear manual disabled flags on access points, cargo spaces, installations, or tow links.

`vehicle repair <vehicle> hitch <link|all>` re-enables persistent tow or hitch links after physical repairs only if unified graph validation passes. Invalid links stay disabled and report the validation reason.

`vehicle access <vehicle>` manages persisted character grants. Use `list` to see current rows, `grant` to add or update an action tag and level, and `revoke` to remove by row id or by character and optional tag.

`vehicle relink` allows an admin to attach a vehicle to a replacement exterior item that already has the vehicle exterior component.

## FutureProg

The exterior item is already accessible to existing item-oriented FutureProg surfaces because it is an ordinary `IGameItem`.

Vehicle-specific FutureProg support should be added after the core runtime model stabilises. The intended shape is:

- functions to get a vehicle from an exterior item
- functions to get the exterior item from a vehicle
- occupancy queries
- controller queries
- movement state queries
- future route/coordinate commands and predicates

The vehicle should remain the canonical argument type for vehicle-specific functions; the exterior item should remain available for ordinary item checks.

## Acceptance Scenarios

Currently covered by implementation:

- Creating a vehicle prototype can link a valid exterior item prototype and automatically attach the internal vehicle exterior component.
- Vehicle prototype creation/submission requires an exterior item component linked to the vehicle prototype.
- Generic item loading is blocked for vehicle exterior shells through `PreventManualLoad`.
- Creating a vehicle instance creates a live `Vehicle` and exterior `GameItem`.
- Vehicle and exterior item have bidirectional links after factory creation.
- Save/load preserves prototype, exterior item id, canonical location, room layer, movement state, occupancies, and access rows.
- ItemScale vehicles can be boarded, controlled, moved through a valid cell exit, and exited.
- Occupied exterior items reject normal item pickup/repositioning.
- Occupied exterior items reject being hauled into containers.
- Forced relocation of the exterior item either moves visible occupants with the exterior to its new cell or clears occupancy if the exterior no longer has a cell location.
- Independent relocation of an occupant by teleport, transfer, arrest-like hardcoded moves, grapple dragging, combat knockdown, or similar cell-leave paths clears that occupant's vehicle occupancy.
- Vehicle cell-exit movement honours exterior item movement blockers and disconnects independent `IConnectable` links that would otherwise span cells.
- RoomContainer vehicles support multiple authored compartments, slots, and stations.
- Invalid cell-exit movement is blocked for controller mismatch, missing movement profile, too-small exit, invalid origin, and non-viable transition.
- Reboot recovery resynchronises the exterior item projection to the vehicle's canonical cell and layer.
- Builder/admin output distinguishes canonical vehicle state from item projection state.
- Factory creation builds and links access/cargo projection items.
- `hatch@vehicle`, `trunk@vehicle`, and installed module targeting resolves through normal item targeting.
- Access point projections implement normal open/close/lockable item behaviour while storing canonical state on `VehicleAccessPoint`.
- Cargo projections gate normal container components through vehicle access rules.
- Installed modules use `VehicleInstallableGameItemComponent` and existing `install` / `uninstall` commands.
- Movement can be blocked by access denial, disabled/destroyed zones, damage-linked movement profiles, open must-close access points, missing or low-condition modules, missing roles, insufficient fuel, insufficient power, invalid recursive tow trains, or tow-stress catastrophe.
- Exterior item damage creates vehicle-zone wounds rather than ordinary exterior item wounds.
- Damage-zone effects disable linked access, cargo, installation, tow-point, movement-profile, or whole-vehicle movement targets.
- Generic `repair <vehicle exterior> with <kit>` and admin `vehicle repair <vehicle> damage <zone|all>` recalculate vehicle damage-zone status from remaining wounds while preserving manually disabled system flags.
- Hitch/unhitch persists tow links, validates recursive tow trains, records required physical hitch items for non-direct tow points, reserves those items while linked, evaluates valid strained links for catastrophe, and moves all linked towed vehicles.
- Active character/mount hitches let characters or mounts pull another character or a vehicle tow point through ordinary cell-exit movement, with `IHitchGear` or legacy `IDragAid` items for non-direct tow points and tow-point pull multipliers.
- Character/mount hitches can pull a vehicle that itself has downstream vehicle tow links; capacity, tow-point limits, hitch gear, exit size, damage, duplicate incoming links, and cycle checks use the whole unified train.
- Dragging a vehicle exterior through an ordinary movement command moves downstream towed vehicles and co-located hitch items exactly once, while the root vehicle still reconciles from the exterior item's movement.

Still to implement:

- Full player-facing vehicle deletion/destruction lifecycle beyond the current exterior-item fail-safe disembark behaviour.
- Rich physical access-device authoring, ownership, lease, and fleet policy tooling beyond simple character grants.
- Terrain/layer restrictions beyond the existing exit transition rules.
- Richer hitch item mechanics, catastrophe tuning/admin surfaces, and fuller fuel/power network topologies.
- Route, coordinate 2D, coordinate 3D, and RoomScale systems.

For a fresh-world manual verification path, including the supporting item and component prototype authoring steps, see [Vehicle System Fresh MUD Test Runbook](./Vehicle_System_Fresh_MUD_Test_Runbook.md).

## Implementation Phases

### Phase 1: Core Hybrid Vehicle Framework

Status: implemented for the first slice.

Scope:

- domain interfaces and enums
- EF models and migration
- gameworld registries and loaders
- vehicle factory
- exterior item component and generic-load guard
- prototype and live admin commands
- ItemScale occupancy
- RoomContainer compartments, slots, stations, and multi-occupant support
- CellExit movement
- focused movement strategy tests

### Phase 2: Vehicle Systems

Status: implemented for cell-exit operational readiness; partially implemented for broader vehicle-system maturity.

Implemented scope:

- access points, cargo spaces, install points, tow points, and damage zones in prototype/live persistence
- projected access and cargo items exposed through `thing@vehicle`
- open/close/lockable access point projection components
- cargo projection component that gates a normal container component through vehicle access rules
- installable module component and existing `install` / `uninstall` integration
- movement profile fuel, power, required role, access-closed, and tow-link requirements
- exterior item wound override into vehicle damage zones
- damage-zone effects that disable access, cargo, install, tow, movement-profile, or whole-vehicle movement targets
- live diagnostics that report manual versus damage-derived disable causes
- admin damage repair that clears zone wounds/status without clearing manual disabled flags
- robust hitch/unhitch commands, loaded broken-link diagnostics, hitch-item validity, and recursive tow-train movement
- active character/mount hitches and character hitch chains for live mount-drawn carts, hand-pulled vehicles, and similar cell-exit movement scenes
- persistent mixed hitch graph for NPC/vehicle character-root hitches with typed endpoints, nullable load-safe references, registry loading, active runtime recovery, and invalid-link diagnostics
- unified hitch graph validation and movement preflight across legacy tow links, persistent mixed hitch links, and transient live drag/hitch effects
- operational readiness service for board/control/service/repair/hitch permissions, module condition, required roles, fuel, power, repair hints, and tow-stress catastrophe mutation
- admin `vehicle access` grant/revoke/list commands backed by existing `VehicleAccessStates`
- condition-aware vehicle installable component XML and builder commands for functional and movement thresholds
- player-facing exterior repair through ordinary repair kits, with vehicle damage-zone recalculation
- admin hitch repair that re-enables persistent links only after unified graph validation passes
- `vehicle show` readiness diagnostics covering access, modules, fuel, power, damage, repair hints, invalid links, and tow stress

Remaining Phase 2 scope:

- rich physical access-device authoring, ownership, lease, and fleet policy tooling beyond simple character grants
- fuller fuel, power, electrical, and liquid network topology support beyond installed candidate modules
- richer hitch item mechanics, catastrophe tuning/admin surfaces, and broader recovery UX
- deletion/destruction lifecycle polish and deeper builder diagnostics for all system records

Recommended next major slice: **Phase 2 Closeout - Route-Ready Operations**. This should still keep the system on cell-exit vehicles, but finish the authoring and diagnostics that route movement would otherwise amplify: access-device presets and fleet policy, richer fuel/power topology, explicit catastrophe tuning, deletion/destruction recovery, staff audit commands, and FutureProg predicates for vehicle readiness. Acceptance should focus on staff being able to author, inspect, script, and support a small vehicle fleet in live play without code changes.

This is recommended before Phase 3 route movement because scheduled transit needs dependable operational predicates. Once route-ready operations are stable, Phase 3 can build route networks and schedules on top of vehicles that already know whether they can board, start, move, consume resources, recover from damage, and report why not.

Cargo and installed systems should reuse existing item/container/component infrastructure, but the vehicle decides which compartment or attachment point exposes each item capability.

### Phase 3: Route Movement

Status: planned.

Scope:

- authored route networks
- stops and schedules
- boarding windows
- delays
- automated transit
- builder route previews and diagnostics

### Phase 4: Coordinate 2D Movement

Status: planned.

Scope:

- vehicles located by map coordinates
- projection into visible cells where appropriate
- speed, heading, and surface constraints
- transition between cell-exit and coordinate movement where needed

### Phase 5: Coordinate 3D And RoomScale

Status: planned.

Scope:

- aircraft, spacecraft, ships, elevators, and other large moving interiors
- multi-cell interiors
- docking, berthing, attachment, and disconnection
- coordinate 3D movement and layered projection

## Design Rules

- Every vehicle has exactly one exterior item projection.
- The live `Vehicle` record is canonical for vehicle state.
- The item component is only a bridge.
- Player interaction routes through the exterior item unless an admin command explicitly targets a vehicle id.
- Occupants are vehicle occupancy records, not component XML.
- Future systems may reuse item/container/component infrastructure, but the vehicle owns the compartment, station, movement, access, and projection decisions.
