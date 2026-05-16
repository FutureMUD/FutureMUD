# Vehicle System

## Purpose

FutureMUD vehicles use a hybrid model:

- The vehicle domain owns canonical authored and live state.
- The item system owns the visible, targetable exterior projection.

Every vehicle has one `IVehiclePrototype`, one live `IVehicle`, and one exterior `IGameItem` with a `VehicleExteriorGameItemComponent`. Players interact with the exterior item, but movement, occupancy, control stations, compartments, access state, and future route/coordinate behaviour belong to the vehicle domain.

This keeps vehicle logic out of component XML while preserving normal item-world affordances such as seeing, targeting, damaging, locking, towing, examining, and scripting against the exterior shell.

## Current Implementation Status

Phase 1 and the first three Phase 2 vehicle-systems slices are present:

- Vehicle domain interfaces: `IVehiclePrototype`, `IVehicle`, `IVehicleMovementStrategy`, `IVehicleMovementState`, `IVehicleOccupancy`, and `IVehicleAccessState`.
- Vehicle enums: `VehicleScale`, `VehicleLocationType`, `VehicleOccupantSlotType`, `VehicleMovementProfileType`, and `VehicleMovementStatus`.
- EF persistence for the Phase 1 vehicle prototype/live tables plus Phase 2 access points, cargo spaces, installation points, tow points, damage zones, damage-zone effects, tow links, and vehicle-zone wound links.
- Gameworld registries and loaders for vehicle prototypes and live vehicles.
- Vehicle factory creation of canonical vehicle instance plus exterior item projection.
- `VehicleExteriorGameItemComponentProto` and `VehicleExteriorGameItemComponent`.
- Projection components for access points, cargo spaces, and installable vehicle modules.
- `thing@vehicle` targeting for projected access/cargo/module items through the exterior item.
- `ItemScale` and `RoomContainer` authoring through compartments, slots, stations, and cell-visible exterior item projection.
- Player commands: `embark`, `disembark`, and `drive`.
- Admin/builder commands: `vehicleproto` for prototype authoring and creation, `vehicle` for live diagnostics and relinking.
- Cell-exit movement strategy with controller, location, profile, exit-size, transition, disabled/destroyed, closed-access, required installation, required role, fuel, power, and recursive tow-train validation.
- Tow-train service for hitch validation, cycle prevention, tow-point usage checks, recursive train weight checks, hitch-item validity, and loaded broken-link diagnostics.
- Exterior item wound override that routes hull damage into vehicle damage zones and persists vehicle/zone ids on wounds.
- Damage-zone effects that disable linked access points, cargo spaces, installation points, tow points, movement profiles, or whole-vehicle movement at configured damage statuses.
- Admin damage repair for clearing canonical vehicle damage-zone wounds and status without clearing manual disabled flags.
- Reboot recovery through persisted vehicle location, movement status, exterior item id, occupancies, and projection resynchronisation on load.

The following areas are deliberately scaffolded rather than fully built:

- Rich access rules beyond explicit persisted access rows.
- Dynamic tow breakage/catastrophe, rich access-device authoring, player-facing repair workflows, and fuller fuel/power networks.
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
- `VehicleTowPointPrototype` defines tow/towed capability and maximum towed weight.
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

This order allows vehicle prototypes to resolve item prototypes, live vehicles to resolve exterior items, and vehicle occupancies to resolve characters.

When vehicles load, they call `SynchroniseExteriorItemToLocation()` so the exterior item projection returns to the vehicle's canonical cell and room layer after reboot.

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
- `install <held module> <vehicle> [install point]`
- `uninstall <module@vehicle>`
- `hitch <towpoint>@<vehicle> <towpoint>@<target> [with <item>]`
- `unhitch <vehicle>`
- `unhitch <towpoint>@<vehicle>`

Projected vehicle system items are exposed through ordinary item targeting. The preferred form is `thing@vehicle`, for example `open hatch@car`, `unlock hatch@car`, `put crate trunk@car`, and `get crate trunk@car`. The older generic attached-item form `vehicle@thing` is also accepted for compatibility, but vehicle documentation and builder examples should use `thing@vehicle`.

Boarding rules currently check:

- the target item has a linked vehicle exterior component
- the actor is not already aboard
- the actor is in the vehicle's canonical cell
- the actor is on the vehicle's canonical room layer
- the requested slot exists
- the slot has remaining capacity
- explicit access rows allow the actor when access rows exist
- if access points exist, an open, unlocked, enabled access point must reach the selected slot

Driving rules currently check:

- the actor is the vehicle controller
- the vehicle is at the exit origin
- the vehicle prototype has a `CellExit` movement profile
- the movement profile is not disabled by vehicle damage effects
- the exit is large enough for the exterior item
- the exit has a viable movement transition for the driver
- the vehicle is not disabled or destroyed
- required access points are closed
- required installed modules and roles are present
- configured fuel and power are available
- recursive tow-train links are valid, tow points are not damage-disabled, hitch items are co-located, and all towed vehicles fit through the exit

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

- actor must be the controller
- vehicle must be at the exit origin
- vehicle prototype must support `CellExit`
- movement profile must not be damage-disabled
- exterior item must fit through the exit's maximum size
- exit transition must be viable
- vehicle must not be disabled or destroyed
- configured access points that must be closed are closed
- required modules, roles, fuel, power, damage-linked systems, and recursive tow-train links are valid

Current movement behaviour:

- emit origin echo
- mark vehicle as `CellExitTransit` and `Moving`
- persist transit state before movement
- move the exterior item to the destination cell and target layer
- teleport occupants to the destination cell and layer without ordinary character movement queues
- consume configured fuel and power
- move all recursively towed vehicles, hitch items, and occupants
- mark vehicle as `Cell` and `Stationary`
- clear current exit and destination fields
- emit destination echo

This intentionally uses a vehicle movement strategy rather than forcing vehicles through the character-centric `IMovement` pipeline.

## Admin Diagnostics

`vehicle` commands inspect and repair live instances.

Current commands:

- `vehicle list`
- `vehicle show <id|name>`
- `vehicle repair <id|name>`
- `vehicle repair <id|name> damage <zone|all>`
- `vehicle relink <id|name> <item id|local item>`

`vehicle show` distinguishes canonical vehicle state from item projection state:

- prototype and scale
- canonical location, layer, movement status, transit fields
- exterior item id
- loaded exterior item
- component `VehicleId`
- projection synchronisation status
- current occupants and controller
- access point, cargo, installation, tow-link, and damage-zone state
- manual versus damage-derived disable causes for projected systems

`vehicle repair` restores the bidirectional vehicle/item component link for the current exterior item and synchronises the exterior item to the canonical vehicle location.

`vehicle repair <vehicle> damage <zone|all>` clears the selected vehicle damage-zone wounds and damage status. It does not clear manual disabled flags on access points, cargo spaces, installations, or tow links.

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
- RoomContainer vehicles support multiple authored compartments, slots, and stations.
- Invalid cell-exit movement is blocked for controller mismatch, missing movement profile, too-small exit, invalid origin, and non-viable transition.
- Reboot recovery resynchronises the exterior item projection to the vehicle's canonical cell and layer.
- Builder/admin output distinguishes canonical vehicle state from item projection state.
- Factory creation builds and links access/cargo projection items.
- `hatch@vehicle`, `trunk@vehicle`, and installed module targeting resolves through normal item targeting.
- Access point projections implement normal open/close/lockable item behaviour while storing canonical state on `VehicleAccessPoint`.
- Cargo projections gate normal container components through vehicle access rules.
- Installed modules use `VehicleInstallableGameItemComponent` and existing `install` / `uninstall` commands.
- Movement can be blocked by disabled/destroyed zones, damage-linked movement profiles, open must-close access points, missing modules, missing roles, insufficient fuel, insufficient power, and invalid recursive tow trains.
- Exterior item damage creates vehicle-zone wounds rather than ordinary exterior item wounds.
- Damage-zone effects disable linked access, cargo, installation, tow-point, movement-profile, or whole-vehicle movement targets.
- Admin `vehicle repair <vehicle> damage <zone|all>` clears vehicle damage-zone wounds/status while preserving manually disabled system flags.
- Hitch/unhitch persists tow links, validates recursive tow trains, records optional physical hitch items, and moves all linked towed vehicles.

Still to implement:

- Robust destruction/deletion flows for vehicles, exterior items, and occupants.
- Rich access control authoring and enforcement.
- Terrain/layer restrictions beyond the existing exit transition rules.
- Player-facing repair workflows, rich trailer mechanics, and fuller fuel/power topologies.
- Route, coordinate 2D, coordinate 3D, and RoomScale systems.

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

Status: partially implemented.

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

Remaining Phase 2 scope:

- richer access-device authoring and permissions
- module health/function checks beyond installed/disabled state
- fuller fuel/power topology support
- dynamic trailer breakage/catastrophe and richer hitch item mechanics
- player-facing repair workflows and deeper builder diagnostics for all system records

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
