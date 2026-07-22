# Vehicle System

## Purpose

FutureMUD vehicles use a hybrid model:

- The vehicle domain owns canonical authored and live state.
- The item system owns the visible, targetable exterior projection.

Every vehicle has one `IVehiclePrototype`, one live `IVehicle`, and one exterior `IGameItem` with a `VehicleExteriorGameItemComponent`. Players interact with the exterior item, but movement, occupancy, control stations, compartments, access state, RouteCell position, and operational route behaviour belong to the vehicle domain.

Authored access-point and cargo-space items are logical target projections, not additional top-level room items. Their effective cell, layer, and RouteCell coordinate are inherited from the canonical exterior item, and ordinary local targeting expands a visible exterior into its live projections. Consequently `open ramp` and `put crate in hold` remain usable after exterior movement without inserting duplicate projection objects into the cell; qualified forms such as `ramp@platform` remain available for disambiguation.

This keeps vehicle logic out of component XML while preserving normal item-world affordances such as seeing, targeting, damaging, locking, towing, examining, and scripting against the exterior shell.

## Current Implementation Status

Phases 1, 2, 3, and 3B are present for the V1.0 vehicle boundary:

- Vehicle domain interfaces: `IVehiclePrototype`, `IVehicle`, `IVehicleMovementStrategy`, `IVehicleMovementState`, `IVehicleOccupancy`, and `IVehicleAccessState`.
- Vehicle enums: `VehicleScale`, `VehicleLocationType`, `VehicleOccupantSlotType`, `VehicleMovementProfileType`, `VehicleMovementEnvironment`, and `VehicleMovementStatus`.
- EF persistence for the Phase 1 vehicle prototype/live tables plus Phase 2 access points, cargo spaces, installation points, tow points, damage zones, damage-zone effects, tow links, and vehicle-zone wound links.
- Gameworld registries and loaders for vehicle prototypes and live vehicles.
- Vehicle factory creation of canonical vehicle instance plus exterior item projection.
- `VehicleExteriorGameItemComponentProto` and `VehicleExteriorGameItemComponent`.
- Projection components for access points, cargo spaces, and installable vehicle modules.
- `thing@vehicle` targeting for projected access/cargo/module items through the exterior item.
- `ItemScale` and `RoomContainer` authoring through compartments, slots, stations, and cell-visible exterior item projection.
- Player commands: `embark`, `disembark`, `vehiclecontrol`, `vehiclestatus`, `drive`, route-aware ordinary movement commands, and `transit` timetable/status queries.
- Admin/builder commands: `vehicleproto` for prototype authoring and creation, `vehicle` for live diagnostics/recovery/retirement, `vehicleroute` for revisioned itineraries, and `vehicleservice` for recurring operations.
- Cell-exit movement strategy with controller, location, profile, exit-size, transition, disabled/destroyed, closed-access, required installation, required role, fuel, power, and recursive tow-train validation.
- Surface-water cell-exit profiles for `ItemScale` and `RoomContainer` craft, including surface-only traversal, hull floating, occupant swim support, configurable occupant water exposure, explicit selectable propulsion, and water-safe disembark/destruction cleanup.
- Tow-train service for hitch validation, cycle prevention, tow-point usage checks, recursive train weight checks, required `IHitchGear`/legacy `IDragAid` connector item validity, in-use item reservation, and loaded broken-link diagnostics.
- Active character/mount hitching through the normal drag movement system, including character-to-character chains and character/mount-to-vehicle tow-point hitches with physical connector gear for non-direct tow points.
- Tow point authoring includes a character pull multiplier that scales effective pull capacity for mounts, animals, or people pulling that vehicle point.
- Persistent mixed hitch graph foundation: `VehicleHitchEndpointType`, `IVehicleHitchLink`, `IVehicleHitchService`, `VehicleHitchLinks` EF schema, runtime invalid-link diagnostics, gameworld registry/load order, and reboot projection into active `CharacterHitch`/`Dragging` effects.
- Unified hitch graph traversal through `IVehicleHitchGraphService`, covering legacy `VehicleTowLinks`, persistent mixed `VehicleHitchLinks`, and live transient `CharacterHitch`/`Dragging` effects for validation, train weight, tow-point usage, movement preflight, hitch item relocation, and tow-stress evaluation.
- Operational readiness checks through `IVehicleOperationalReadinessService`, covering board/control/service/repair/hitch access, module condition, movement roles, fuel, power, repair hints, and tow-stress catastrophe mutation.
- `VehicleAccessStates` now provide simple persisted character grants without a new schema: `board`, `control`, `service`, `repair`, `hitch`, and `all` tags with level 1 boarding, level 2 operational actions, and level 3 all-action access.
- Admin `vehicle access` commands manage persisted grants, reusable access presets, preset application, and grant cloning; no access rows remains permissive for compatibility, and existing projection lock/key behaviour is unchanged.
- Vehicle installable modules are condition-aware through `mincondition` and `movementcondition` XML fields with backward-compatible zero-threshold defaults.
- `vehicle show` includes mixed persistent hitch links involving that vehicle and reports invalid causes without requiring endpoint load success.
- Exterior item wound override that routes hull damage into vehicle damage zones and persists vehicle/zone ids on wounds.
- Damage-zone effects that disable linked access points, cargo spaces, installation points, tow points, movement profiles, or whole-vehicle movement at configured damage statuses.
- Admin damage repair for clearing canonical vehicle damage-zone wounds and status without clearing manual disabled flags.
- Occupancy reconciliation when a rider/driver is moved independently from the vehicle by teleport, transfer, arrest-like relocation, combat knockdown, grapple/drag movement, or other hardcoded cell-leave paths.
- Exterior relocation hardening for dragged or force-moved vehicle items, including carrying visible occupants with a still-cell-present exterior and clearing occupancy when the exterior is contained, inventoried, deleted, or destroyed.
- Reboot recovery through persisted vehicle location, movement status, exterior item id, occupancies, and projection resynchronisation on load.

The following areas remain deliberately narrower than a complete engineering simulation:

- Rich physical access-device authoring, ownership, and lease systems beyond explicit persisted access rows, reusable access presets, fleet helper commands, and existing projection locks.
- Fuller fuel, power, electrical, and liquid network topology beyond installed candidate modules.
- Coordinate 2D/3D movement outside the linear RouteCell model.
- Collision, physical consist length, overtaking, signalling, and dispatch.
- Fares, tickets, reservations, and full electrical/liquid network topology.

### RouteCell and RoomScale Vehicle V1.0 Boundary

Vehicle V1.0 includes `ItemScale`, `RoomContainer`, and `RoomScale` vehicles. The stable contract includes manual cell-exit movement, continuous longitudinal RouteCell movement, persistent RoomScale interior cells, transient docking links, revisioned `VehicleRoute` itineraries, recurring services, restart-safe journeys, occupancy and control, access and locks, cargo, installed modules, damage and repair, fuel and power preflight, mounts and recursive hitch cohorts, builder/player/admin workflows, and scripting diagnostics.

The spatial and operational meanings of “route” are separate:

- [`RouteCell`](./World/Route_Cell_System.md) is a linear cell in which entities have exact metre coordinates.
- `VehicleRoute` is a revisioned operational itinerary containing pinned longitudinal and exit-traversal steps.

An ordinary-cell vehicle route is not a substitute for RouteCell geometry, and entry into a long RouteCell is never treated as one ordinary room of travel.

The following remain post-V1.0: coordinate 2D/3D movement, collision/dispatch/signalling, physical consist length, overtaking, fares, reservations, rich ownership/lease policy, and full electrical/liquid network simulation. V1.0 also does not generate reverse/bidirectional services automatically or dynamically reroute an active journey around changed topology; builders author the required route directions explicitly and topology changes fail closed.

Automated coverage exercises the spatial contracts, longitudinal motion, route compilation, service/journey state rules, persistent RoomScale interiors, docking topology, platform embark/disembark resolution, and route-vehicle command parsing. The scheduled train, ordinary-exit massive platform, and mount-drawn recursive wagon train were also built and driven through the live telnet runtime, including docking, occupied travel, multiple wagon checkpoints, explicit mid-route portal traversal, far-terminal arrival, and reboot reload. The maintained blank-database snapshot includes the final migration. See the [V1.0 acceptance evidence](./Verification/RouteCell_Vehicle_V1_Acceptance_Evidence.md). An upgraded production-world soak remains a release-candidate gate rather than an unimplemented V1 behavior.

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
- `VehicleMovementProfilePrototype` declares the movement strategy family, movement environment, occupant water-exposure policy, and resource/readiness requirements.
- `VehicleAccessPointPrototype` defines doors, hatches, ramps, canopies, and service panels projected as targetable items.
- `VehicleCargoSpacePrototype` links a canonical cargo space to a targetable container projection item.
- `VehicleInstallationPointPrototype` defines installable module mount type, optional role, and movement-required slots.
- `VehicleTowPointPrototype` defines tow/towed capability, maximum towed weight, the character pull multiplier used when a character or mount pulls a vehicle through that tow point, and optional tow-stress policy overrides for warning threshold, failure-start threshold, maximum failure chance, and damage multiplier.
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
- valid movement environments, with occupant water exposure enabled only for surface-water profiles

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
- `VehiclePropulsionProfileProtos`
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

The initial migration is `VehiclesHybridModel`; Phase 2 system tables are added in `VehicleSystemsPhase2`, with later additive migrations for character-pull multipliers, persistent hitch links, operational-readiness fields, nullable tow-stress policy overrides, and surface-water movement profile fields.

Important persistence rules:

- A vehicle prototype references an exterior item prototype by item prototype id and revision.
- A live vehicle references its exterior item by live game item id.
- A live vehicle has a one-to-one external projection by convention and database uniqueness on `Vehicles.ExteriorItemId`.
- Occupants are persisted in `VehicleOccupancies`; they are never stored inside item component XML.
- Movement recovery fields are persisted on `Vehicles`: current cell, room layer, movement status, current exit, destination cell, and movement profile.
- `VehicleMovementProfileProtos.MovementEnvironment` and `ExposesOccupantsToWater` persist the surface-water operating contract. Existing rows default to unrestricted movement and protected exposure, so ordinary vehicles retain their prior behaviour.
- `VehiclePropulsionProfileProtos` stores at most one configuration of each propulsion type for a surface-water movement profile. It persists the default flag, base traversal time, check trait/difficulty, and speed/stamina expressions. `VehicleOccupantSlotProtos.ContributesToPropulsion` marks automatic rower slots, while nullable `Vehicles.ActivePropulsionProfileProtoId` persists the selected live mode. Existing surface-water profiles without child rows retain legacy movement and display a migration warning; new surface-water revisions must author a mode before submission.
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

When vehicles load, they call `SynchroniseExteriorItemToLocation()` so the exterior item projection returns to the vehicle's canonical cell and room layer after reboot. Intact surface-water exteriors are restored to floating posture; destroyed surface-water vehicles clear any stale occupancies and place those characters into swimming posture when the canonical layer is water.

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
8. Optional: `vehicleproto set movement environment <movement profile id> surfacewater`
9. Optional for surface-water craft: `vehicleproto set movement waterexposure <movement profile id> <protected|exposed>`
10. Required for new surface-water craft: `vehicleproto set movement propulsion add <movement profile id> <selfpowered|rowed|sail|outboard|none>`
11. Configure the propulsion row with `time`, `trait`, `difficulty`, `speed`, or `stamina` as applicable; use `vehicleproto set slot propulsion <slot id>` for rowers.
12. `vehicleproto submit <comment>`
13. `vehicleproto approve <id> <comment>`
14. `vehicleproto create <id|name>`

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
- `vehiclecontrol` / `takecontrol`
- `vehiclecontrol release` / `releasecontrol`
- `vehiclestatus [vehicle]`
- `vehiclepropulsion [selfpowered|rowed|sail|outboard|none]`
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
- the exterior item exists, is intact, and is synchronised with the canonical vehicle cell and layer
- the controller is not in combat or blocked by a general/movement delayed action such as crafting
- operational readiness access grants allow control when access rows exist
- the actor is in the same canonical cell and room layer as the vehicle
- the vehicle is at the exit origin
- the vehicle prototype has a `CellExit` movement profile
- the movement profile is not disabled by vehicle damage effects
- every surface-water vehicle in the driven, dragged, or towed train starts and finishes at `GroundLevel` in a cell whose ground layer is a swimming layer
- the exit is large enough for the exterior item
- the exit has a viable movement transition for the driver
- the vehicle is not disabled or destroyed
- required access points are closed
- every occupant slot marked required for movement is staffed
- required installed modules and roles are present, correctly typed, enabled, not destroyed, and above their movement condition thresholds
- configured fuel and power are available from functional installed candidate modules
- the selected explicit propulsion mode is ready; an authored mode never silently falls back to another mode
- recursive tow-train links are valid, tow points are not damage-disabled, hitch items are co-located, all towed vehicles fit through the exit, and any valid strained link survives the tow-stress catastrophe preflight

The first occupant to board an eligible driver slot with a configured control station takes control automatically when they are physically at that station. A controller can release it with `vehiclecontrol release`; another occupant in an eligible driver slot can then use `vehiclecontrol` to take over. For RoomScale vehicles, selecting a driver slot through an access point in another explicitly linked compartment boards successfully into the access point's authored docking compartment, but does not grant control until the driver walks to the hosted control compartment and uses `vehiclecontrol`. `vehiclestatus` gives players a compact view of controller, crew, access, cargo, modules, damage, and—when they control it—the full cell-exit preflight result.

When a controller enters an ordinary movement command, character movement redirects it to vehicle movement before walking movement is attempted. This means a bicycle rider can type `north` instead of `drive north`; the explicit `drive` command remains available for clarity. Vehicle movement uses the normal movement pipeline shape: it sets the actor's current `IMovement`, applies a movement delay, supports turn-around cancellation and queued follow-up movement commands, marks the vehicle as moving while in transit, and resolves the movement after the scheduled step. Delayed movement revalidates at departure commit, rolls tow catastrophe exactly once at that commit, and completes the exact validated hitch/resource plan rather than rebuilding or failing open.

Visible occupants of a vehicle are presented in the same style as mounted riders. If a character is visibly occupying a vehicle whose exterior item is also visible in the same cell and layer, their long description says they are riding that vehicle, and the exterior item is suppressed from the separate item list to avoid duplicate room lines. If occupants are not visible in the cell, the exterior item remains the room-facing presentation.

### Surface-Water Movement Profiles

Surface-water boats use the existing `ItemScale` or `RoomContainer` vehicle scales and the normal `CellExit` strategy. They are not `RoomScale` moving interiors. Builders configure the cell-exit movement profile with:

```text
vehicleproto set movement environment <profile id> surfacewater
vehicleproto set movement waterexposure <profile id> <protected|exposed>
```

`SurfaceWater` requires both the origin and destination to be exactly `RoomLayer.GroundLevel`, with `ICell.IsSwimmingLayer(GroundLevel)` true. The shared hitch-graph preflight applies this rule to every vehicle in driven, character-dragged, and vehicle-towed movement. A surface-water craft can still be created, stored, administratively relocated, or carried as an unoccupied item on land; it simply cannot traverse a cell exit operationally from or into that state.

An intact surface-water exterior is kept in `PositionFloatingInWater` even when its ordinary material buoyancy would be negative. The hull still receives normal liquid exposure. Disabled craft continue to float, while a globally destroyed vehicle loses its floating exemption.

Occupants of an intact, canonically co-located surface-water craft do not run swim heartbeats, spend swimming stamina, or sink. A protected profile also suppresses the terrain-water exposure applied to exposed inventory; an exposed profile leaves that normal immersion in place, which suits surfboards and other wet craft. Protection is continuous while aboard but does not dry existing wetness or block rain, spills, or other liquid sources. Voluntary disembark, forced disembark, exterior destruction, and a damage transition that destroys the overall vehicle clear occupancy and set characters left in water to `PositionSwimming`.

#### Propulsion Profiles

Surface-water movement profiles may author one row for each of `SelfPowered`, `Rowed`, `Sail`, and `OutboardMotor`, or one exclusive `None` row. One authored row is the default for newly created vehicles. A controller aboard a stationary vehicle uses `vehiclepropulsion <mode>` to change the active row; `vehiclepropulsion` and `vehiclestatus` show the current choice. The choice persists, and failure of the chosen mode does not select another mode automatically. `None` prevents the craft from initiating vehicle movement but does not prevent an unoccupied item-scale craft from being carried, or a craft from being dragged or towed when another mover is the root.

Builder commands are:

```text
vehicleproto set movement propulsion add <movement-id> <selfpowered|rowed|sail|outboard|none>
vehicleproto set movement propulsion remove <propulsion-id>
vehicleproto set movement propulsion default <propulsion-id>
vehicleproto set movement propulsion time <propulsion-id> <seconds>
vehicleproto set movement propulsion trait <propulsion-id> <trait-id|name>
vehicleproto set movement propulsion difficulty <propulsion-id> <difficulty>
vehicleproto set movement propulsion speed <propulsion-id> <expression>
vehicleproto set movement propulsion stamina <propulsion-id> <expression>
vehicleproto set slot propulsion <slot-id>
```

Every explicit mode starts with a 10-second base traversal time. Actual time is `base time * max(exit time multiplier, 0.01) / effective propulsion multiplier`, capped by `MaximumMoveTimeMilliseconds` and independent of walking speed. Speed expressions may use only `outcome` for self-powered/rowed, `wind` for sail, or `output` for outboards; self-powered/rowed stamina expressions may use `outcome` and `swimcost`. Departure preflight is stateless. At commit it revalidates the train, freezes wind/contributor/oar/motor/resource identities, rolls each human check once, and charges stamina and motor energy once before movement begins. Cancellation retains those committed costs; intermediate validation uses the frozen plan.

- `SelfPowered` rolls `PaddleVehicleCheck` for the controller with the configured trait and difficulty. Its default speed is `max(0.25, 1.0 + 0.15 * outcome)`.
- `Rowed` automatically selects every able non-combatant occupant in a propulsion slot who holds or wields a usable oar and can afford the worst configured stamina result. The best held oar is chosen by positive efficiency multiplied by clamped item condition. Each rower rolls `RowVehicleCheck`; speed is the square root of the sum of oar effectiveness multiplied by each outcome multiplier, so additional rowers have diminishing returns.
- Self-powered and rowed stamina defaults to `swimcost * max(0.5, 1.0 - 0.10 * outcome)`. Outcomes range from -3 to +3; failure makes propulsion slower and costlier without blocking it. Upgraded databases whose dedicated check is missing or was automatically created with a constant, parameterless expression explicitly use `GenericSkillCheck` with the selected trait.
- `Sail` samples origin wind at departure and requires wind above `Still`. The expression receives ranks 1 through 7 for `OccasionalBreeze` through `MaelstromWind`; the default is `1.0 + 0.15 * (wind - 1.0)`. Direction, tacking, waves, swell, currents, and weather-driven movement are outside this slice.
- `OutboardMotor` uses every installed, functional, switched-on (when an `IOnOff` exists), energy-ready outboard. Outputs sum linearly, while unavailable motors remain visible with their blockers. Each motor that successfully commits consumes its configured same-item fuel or electrical spike exactly once. A motor that fails its commit-time draw is excluded, and duration is recalculated from the motors that actually supplied energy; departure fails without a charge only when none can commit.

`Vehicle Oar` is an item component with a positive efficiency multiplier. `Outboard Motor` is an item component with a positive output multiplier and either a fuelled or electric energy source. A fuelled motor requires a configured liquid/volume and a same-item `ILiquidContainer`; an electric motor requires a configured spike and a same-item `IProducePower`. Motor items must also carry `Vehicle Installable` and be installed on the driven vehicle.

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
- richer hitch item mechanics and broader recovery UX beyond the current stress-policy and staff recovery commands

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

Status: implemented as the shared readiness authority for cell-exit movement, longitudinal RouteCell movement, compiled route steps, and scheduled departure/recovery.

`IVehicleOperationalReadinessService` is the shared runtime preflight for vehicle operation. It intentionally does not add new access tables, coordinate movement, or a second route-specific readiness model. Instead, it normalises existing access rows, projection locks, install points, damage zones, hitch graph links, fuel modules, power modules, and repair-kit flows into one set of actionable diagnostics used by manual cell exits, longitudinal checkpoints, compiled portal steps, scheduled departures, and journey recovery.

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

Tow catastrophe is only evaluated for valid but strained graph links. Hard invalid states still fail closed before movement: cycles, missing endpoints, missing/destroyed/incompatible gear, duplicate incoming links, duplicate tow-point use, over maximum towed weight, damage-disabled tow points, and blocked exits. Defaults warn at 90% of effective capacity, begin failure rolls at 95%, and scale to a maximum 25% chance at capacity; static configuration supplies the global policy and individual tow points can override warning, failure-start, maximum-chance, and damage-multiplier values. A catastrophe aborts the movement before fuel or power consumption, echoes the failure, damages the hitch item when available or the linked vehicle exteriors otherwise, disables persistent links, clears transient hitch/drag effects, and releases reserved hitch items. Recovery is deliberately explicit: repair damaged items or vehicle exteriors, then `unhitch` and re-hitch, or use admin `vehicle repair <vehicle> hitch <link|all>` or `vehicle recover <vehicle> hitch fix` for validated persistent links.

`vehicle show` now includes an operational readiness section with access, modules, fuel, power, damage, repair hints, invalid projection/link reasons, and tow-stress warnings. Staff can also run `vehicle audit <scope>` and `vehicle fleet <scope> audit` to scan a local cell, zone, prototype family, or all loaded vehicles.

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

Implemented for V1.0 as persistent hosted interior cells.

Each live RoomScale compartment owns one stable interior `Cell`. Prototype compartment links become normal internal exits, and access-point prototypes map exterior docking to the appropriate compartment. Exterior movement never relocates or regenerates interior cells or their occupants. Hosted cells retain authored terrain and atmosphere while resolving effective zone, shard, time, and external environmental context through the vehicle exterior.

RoomScale submission requires valid interior terrain/exposure settings, a connected compartment graph, at least one external access point, valid control stations, and at least one CellExit or Route movement profile. Retirement is blocked while an interior is occupied, a journey is active, or a hitch remains.

An access point can serve a slot in another compartment only when the live hosted interiors are connected by usable explicit compartment links. The docking exit itself always terminates in the access point's authored compartment. Occupant-slot assignment does not collapse that interior geography: a RoomScale controller must be physically present in the hosted compartment containing their configured control station, and movement readiness fails closed if the link or station interior is unavailable.

Normal character movement remains blocked for vehicle occupants except for one narrow RoomScale case: an exit may be traversed when its origin and destination are both persistent hosted interiors belonging to that same vehicle. This permits ordinary movement through explicit compartment links. A docking exit has an exterior cell on one side and therefore never satisfies the exception; occupants must use `disembark`, while outside characters use `embark`. `ItemScale` and `RoomContainer` occupants retain the existing blanket walking restriction.

## Cell-Exit Movement

`CellExitVehicleMovementStrategy` is the first completed movement strategy.

It moves a vehicle between adjacent cells through normal exits.

Current validation:

- actor must be the controller and must have control access when explicit access rows exist
- exterior projection must exist, be intact, and match the canonical cell and layer
- controller must not be in combat or blocked by a movement/general delayed action
- actor must be in the same canonical cell and room layer as the vehicle
- vehicle must be at the exit origin
- vehicle prototype must support `CellExit`
- movement profile must not be damage-disabled
- exterior item must fit through the exit's maximum size
- exterior item components must not block movement
- exit transition must be viable
- vehicle must not be disabled or destroyed
- configured access points that must be closed are closed
- occupant slots marked required for movement are staffed
- required modules and roles must be installed, enabled, correctly typed, not destroyed, and above their movement condition thresholds
- fuel and power candidates must be functional and have the required resource or spike capacity
- recursive tow-train links must be graph-valid, fit through the exit, remain within hard tow capacities, and survive any tow-stress catastrophe roll
- every surface-water member of the train must traverse from surface water to surface water at `GroundLevel`

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
- preserve and complete the exact validated resource/hitch plan, with one catastrophe roll at departure commit and no fail-open fallback
- move all recursively towed vehicles, hitch items, and occupants exactly once through the unified hitch graph
- mark vehicle as `Cell` and `Stationary`
- clear current exit and destination fields
- emit destination echo with the same visible-rider versus vehicle-only distinction

This intentionally keeps route validation and vehicle state changes in the vehicle movement strategy, while player-driven cell-exit driving is represented as an `IMovement` so it cooperates with movement delay, movement blocking, queued movement commands, group movement state, and room movement diagnostics.

For RoomScale vehicles the controller may be in the hosted interior cell attached to the primary control station. Exit resolution still uses the exterior projection's cell and layer. Only that exterior and the recursively linked exterior hitch cohort cross the exit; interior occupants remain in stable cells. Docking links are removed before departure and rebuilt only after arrival. External departure/arrival echoes and internal movement announcements use separate output scopes.

## RouteCell Vehicle Movement

`RouteVehicleMovementStrategy` delegates longitudinal travel to the shared RouteCell movement service. Route movement profiles author:

```text
vehicleproto set movement route
vehicleproto set movement route speed <distance>/<time>
vehicleproto set movement route propulsion powered|externallypulled
vehicleproto set movement route fuel <liquid> <volume>/<distance>
vehicleproto set movement route power <watts>
vehicleproto set movement route automatic
```

Powered vehicles use their authored speed. Externally pulled vehicles use the motive hitch-graph root's speed and stamina. The vehicle, its exterior, party/mount/drag participants, hitch gear, and downstream vehicles move atomically at one v1 reference coordinate. The canonical vehicle coordinate drives the exterior item's persisted projection.

A compiled `CellExitStep` uses the same operational-readiness and hitch-graph preflight as longitudinal movement. For an externally pulled train, the incoming character/mount hitch is an explicitly allowed motive link rather than an ordinary “this vehicle is already being towed” blocker. The preflight validates the motive character, every recursive vehicle and connector, the transition, size/environment constraints, stamina, and tow stress before anything crosses. The motive character and its party/mount/drag cohort then traverse with the validated vehicle train and physical hitch gear as one synchronous operation; RoomScale interior occupants remain in their hosted cells. Entry into a RouteCell materialises the step's pinned arrival coordinate for the vehicles and every top-level cohort member. If preparation fails, the exact readiness reason is returned to `drive route` and the cohort remains at the source side.

Every longitudinal begin, durable-progress/checkpoint, arrival, and stop/cancellation phase has two deliberately separate presentation channels. The exterior echo is sourced from the exterior projection and uses RouteCell `Local` scope, so nearby same-layer observers can perceive it while kilometre-distant occupants of the same RouteCell cannot. A RoomScale vehicle additionally sends a plain internal movement announcement to each hosted compartment cell; that message reaches interior occupants without moving them or leaking into the exterior RouteCell. Controller acknowledgements remain personal, and FutureProg movement events remain independent of both presentation channels.

Route checkpoints re-use `IVehicleOperationalReadinessService`. Common readiness is separated from CellExit-only and longitudinal-step checks, but there is one source of truth for access, control, required crew, installations, damage, propulsion, hitches, tow capacity, and tow stress. Coordinate, resource-ledger entries, and newly applied stamina/fuel/power mutations commit in one database transaction, saving only the affected bodies and resource components. Successful commits restore their prior changed/save-queue state; rollback keeps any queue entry required to persist a restored value. Checkpointing never performs a global flush or drains unrelated work. A readiness failure commits the current coordinate, stops the cohort, leaves uncommitted resources untouched, and reports the failed readiness fact. An apply, save, or failed commit instead restores the cohort and every reversible resource snapshot to the preceding durable checkpoint and stops without emitting progress for the failed sequence. An ambiguous commit exception is reconciled by reading back and validating the full durable checkpoint payload, while failure to remove a completed motion row is recoverable and never causes a charge to be replayed.

Player driving adds:

```text
drive forward|backward [<distance>]
drive to <distance|landmark|exit|stop>
drive route <approved vehicle route>
drive stop
```

An exit target reaches the nearest point in its authored portal band and stops. Portal traversal is a separate typed step.

`drive stop` materialises the current coordinate and ends the active manual execution. A later `drive route <approved route>` may resume only when that coordinate lies on exactly one remaining pinned linear step (or at exactly one pending pinned exit-step origin). It starts a fresh durable motion operation at the current coordinate and continues from that step; it never rewinds to the leg's authored origin. No matching continuation, more than one matching continuation, a changed topology version, or a coordinate already at the terminal end fails closed with an actionable message rather than choosing or recomputing a path.

## Persistent RoomScale Interiors

The compartment prototype is the RoomScale interior blueprint. It stores interior terrain, exposure, atmosphere behavior, and the existing compartment description. Explicit compartment-link prototypes generate normal internal exits. Every live compartment stores the id of its persistent hosted `Cell`; the cell stores its vehicle host identity.

The factory creates hosted cells and internal links once. It first durably links the canonical vehicle record to its exterior component, so an interrupted factory run leaves a visible partial vehicle rather than an anonymous exterior item. Reboot loads existing hosted cells and restores a null item-side exterior link from the canonical vehicle record; it reports conflicting exterior links rather than silently stealing them. Missing hosted cells are reported as boot warnings and by `vehicle audit <vehicle> interior`. Normal load never invents a replacement. `vehicle recover <vehicle> interior fix` first relinks the uniquely persisted cell whose hosted-compartment ownership matches the live compartment. If that cell exists but is not loaded or is claimed elsewhere, recovery fails closed instead of creating a duplicate. Only when no persisted hosted cell exists may recovery create a replacement, and only when the affected live compartment has no recorded occupants or contents; an occupied missing interior must be restored from backup. A missing or destroyed exterior stops movement but preserves interiors and their contents.

Hosted cells derive effective zone, shard, clock, and external weather/environment context from the exterior while retaining their own indoor terrain and atmosphere. Access-point prototypes already name a compartment and therefore provide the bridge used for ordinary docking and scheduled platform docking.

Prototype submission validates:

- interior settings on every compartment;
- a connected interior graph;
- at least one usable external access point;
- driver/control station integrity;
- at least one CellExit or Route movement profile.

Vehicle deletion or retirement is blocked while an interior contains characters or top-level items, a journey is active, any vehicle service still references it, or any hitch remains. The database teardown of docking rows, hosted-cell ownership and the vehicle record is one transaction; transient docking and compartment exits are removed only after it commits. Staff recovery supports `vehicle recover <vehicle> interior|docking|all [fix]`.

## VehicleRoutes, Services, And Journeys

`VehicleRoute` is revisioned operational data and is never confused with RouteCell topology. An approved revision owns ordered stops and compiled legs. A stop can be an ordinary cell or an exact RouteCell coordinate; it may bind one or more ordinary platform cells, a dwell time, and a docking tolerance. A leg pins ordered `LinearRouteStep` and `CellExitStep` records plus every referenced RouteCell topology version. Each compiled step records the topology version independently for its origin and destination when that endpoint is a RouteCell, so an exit-only leg and both sides of a RouteCell-to-RouteCell portal become stale when an anchor band or arrival coordinate changes.

Builder workflow:

```text
vehicleroute edit new <name>
vehicleroute edit clone <id|name>
vehicleroute edit <id|name>
vehicleroute close
vehicleroute set stop add <name> [<cell id> [<layer>] [at <route distance>]]
vehicleroute set stop remove <stop>
vehicleroute set stop move <stop> <one-based position>
vehicleroute set stop location <stop> <cell id> [<layer>] [at <route distance>]
vehicleroute set stop dwell <stop> <duration>
vehicleroute set stop platform add <stop> <cell id> <access-point prototype id> [tolerance metres]
vehicleroute set stop platform remove <stop> <binding id>
vehicleroute set leg compile <from-stop> <to-stop>
vehicleroute show <id|name>
vehicleroute preview [id|name]
vehicleroute validate
vehicleroute submit <comment>
vehicleroute approve <id|name> <comment>
```

Submission requires at least two valid stops, a compiled path for every adjacent stop pair, matching topology versions, unique stop ordering, valid platform bindings, and a compatible vehicle route profile. Approved revisions are immutable. Editing or cloning creates a new revision.

A `VehicleService` pins an approved route revision, one live vehicle, operator mode, recurring clock/calendar specification, maximum hold, and enabled state:

```text
vehicleservice new <name> | <route> | <vehicle> | <reference departure> | <recurrence>
vehicleservice edit <id|name>
vehicleservice clone <id|name> <new name>
vehicleservice close
vehicleservice set route <id|name> [revision]
vehicleservice set vehicle <vehicle>
vehicleservice set operator automatic|onboard
vehicleservice set schedule <reference departure> | <recurrence>
vehicleservice set retry <duration>
vehicleservice set maxhold <duration>
vehicleservice set enabled [true|false]
vehicleservice show|audit <id|name>
vehicleservice cancel <id|name> [reason]
vehicleservice resume <id|name>
```

Route or vehicle reassignment is allowed only while the service is disabled and has no active journey. Journey rows reference the stable service id while retaining their own immutable route-revision and vehicle references, so historical journeys remain valid after a later reassignment. When a newer route revision is approved, the formerly current revision becomes `Revised`; an existing service pinned to that still-approved immutable revision remains valid, subject to its topology pins. New manual route selection continues to choose a current revision.

Automatic operation requires an automatic-capable route profile and substitutes only for the primary driver. Other required crew and every physical readiness check remain enforced. Onboard mode requires a valid controller. At an authored RoomScale platform, the journey coordinator opens an unlocked, enabled access point before it exposes the transient boarding link, then removes that link and closes the same access point before automatic departure readiness is evaluated. A disabled or locked access point fails boarding closed; an access point that cannot be closed fails departure into `Held`. Onboard services do not operate physical access points for their controller: the applicable access point must already be open to start boarding and must be closed manually before movement readiness can pass.

Service audit also validates every platform binding against the assigned vehicle. Its access-point prototype must belong to the assigned vehicle prototype, and the live vehicle must contain the corresponding access-point instance. A mismatched binding or missing live access point fails readiness with the affected stop and binding ids rather than waiting for boarding to fail silently.

Journey states are `Scheduled`, `Boarding`, `Held`, `Departing`, `EnRoute`, `Dwelling`, `Arrived`, `Cancelled`, and `Faulted`. A failed departure enters Held, retries every 30 seconds by default, and cancels after the configured maximum hold (default 15 minutes). The maximum-hold clock begins at the first durable `Held` event in the current departure episode. Intermediate retry transitions through `Departing` and later `Held` events do not reset it; only a successfully started leg's durable `Departed` event resets the clock for a later stop. This derivation from ordered journey events makes the same inclusive timeout apply immediately after reboot instead of granting a fresh hold window. Consuming a recurrence, advancing the durable next-departure value, and inserting the uniquely keyed `(service, scheduled departure)` journey plus its first event occur in one transaction; a crash cannot advance the timetable while losing that departure. Each later journey state snapshot and its ordered event append likewise commit in the same database operation. Recovery reconciles legacy rows to the latest durable event and resolves `Departing` or `EnRoute` progress from the vehicle's physical checkpoint; the checkpoint timestamp is carried forward before interrupted motion rows are removed. Journeys record ordered events for state transitions, delays, faults, and checkpoint recovery. Durable motion rejects a same-sequence replay with a different coordinate, duration, or aggregate resource payload as an idempotency conflict and reports it through the terminal movement-fault/system diagnostic path.

Active journeys pin their revision and never silently reroute. A topology change invalidates dependent drafts and makes approved services fail closed until revalidated. Missed departures during downtime are not backfilled; the scheduler creates the next future recurrence. An interrupted active journey resumes at its durable coordinate and adds the offline duration to accumulated delay.

## Docking And Transit UX

Docking is represented by transient exits, not duplicate vehicle items. An ordinary-cell RoomScale vehicle docks between its exterior cell and mapped compartments. A scheduled train within its stop tolerance docks from authored platform cells directly into mapped compartments. Links exist only while the vehicle is stationary, boarding is open, and the access point is usable.

The platform `look` output obtains docked-vehicle and boarding state from the docking service. Opening or retaining a platform docking requires the vehicle to match the stop's authored cell, layer and, for a RouteCell stop, coordinate tolerance. Player commands are:

```text
transit departures [destination]
transit services [destination]
transit status [service]
embark <vehicle> [slot] [via <access id|name>]
disembark
vehicleaccess [open|close <access id|name>]
vehiclestatus
```

Transit output includes scheduled and expected time, platform, destination, boarding state, delay, cancellation or hold reason, and next stop. An occupant with control access can use `vehicleaccess` from a stable RoomScale interior to inspect or operate the canonical live access points without targeting an exterior projection in another cell. The vehicle must be stationary; disabled access points fail closed and locked access points cannot be opened. V1.0 does not add fares, tickets, reservations, or seat allocation.

`transit departures` treats an authored ordinary platform cell as local exactly as ordinary cell topology does. A stop located directly in a RouteCell is local only when the actor's effective lazy-movement coordinate is on the stop's layer and within at least one authored docking tolerance, or within `VehicleRouteDefaultDockingToleranceMetres` when the stop has no binding. Merely sharing a ten-kilometre RouteCell does not make a departure local; when tolerances overlap, the nearest stop is selected.

## Admin Diagnostics

`vehicle` commands inspect and repair live instances.

Current commands:

- `vehicle list`
- `vehicle show <id|name>`
- `vehicle access <id|name> list`
- `vehicle access <id|name> grant <character> <board|control|service|repair|hitch|all> <1-3>`
- `vehicle access <id|name> revoke <character|row id> [tag]`
- `vehicle access <id|name> apply <preset> <character>`
- `vehicle access <id|name> clone <source vehicle>`
- `vehicle access preset list|show|set|remove|delete|reset`
- `vehicle retire <id|name>`
- `vehicle audit <id|name|here|zone|prototype <id|name>|all> [readiness|access|resources|hitch|damage|interior|docking|recovery|all]`
- `vehicle recover <id|name> [projection|install|hitch|interior|docking|all] [fix]`
- `vehicle fleet <scope> access apply|grant|revoke|clone ...`
- `vehicle fleet <scope> recover <projection|install|hitch|interior|docking|all> [fix]`
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

`vehicle access <vehicle>` manages persisted character grants. Use `list` to see current rows, `grant` to add or update an action tag and level, `revoke` to remove by row id or by character and optional tag, `apply` to apply a reusable preset to a character, and `clone` to copy grant rows from another vehicle. `vehicle access preset` manages the semicolon-delimited `VehicleAccessPresets` static configuration without adding schema. `vehicle audit`, `vehicle recover`, and `vehicle fleet` provide staff scan and batch helpers for readiness, access, resource, hitch, damage, projection, and install-state support. Projection recovery distinguishes the canonical exterior from authored secondary projections: a missing exterior requires an explicit `vehicle relink`, while `vehicle recover <vehicle> projection fix` idempotently recreates missing or destroyed access-point and cargo-space items from their pinned authored prototypes. It validates the required component before registering an item, persists both sides of each link, reports per-projection failures, and does nothing when the usable projection already exists.

`vehicle relink` allows an admin to attach a vehicle to a replacement exterior item that already has the vehicle exterior component.

## FutureProg

The exterior item is already accessible to existing item-oriented FutureProg surfaces because it is an ordinary `IGameItem`.

The shared operational FutureProg surface takes vehicle exterior items and is used by both cell-exit and route/service automation:

- `isvehicle(item)`
- `vehiclecanboard(character, item)`
- `vehiclecancontrol(character, item)`
- `vehiclecanservice(character, item)`
- `vehiclecanrepair(character, item)`
- `vehiclecanhitch(character, item)`
- `vehiclecanstart(character, item)`
- `vehiclereadinessreason(character, item, action)` for `board`, `control`, `service`, `repair`, `hitch`, `start`, `move`, or `route`
- `vehicletrainweight(item)`
- `vehicletowstress(item)`

The exterior item remains a supported public FutureProg argument. V1.0 additionally exposes RouteCell identity, exact coordinate/distance, room-equivalent distance, direction, nearest landmark and portal accessibility, plus vehicle route/service identity, journey state, next stop, delay, boarding state, and readiness reason. The [RouteCell movement contract](./World/Route_Cell_System.md#longitudinal-movement) defines the begin, checkpoint position-change/progress, completion, and cancellation hooks fired on vehicle exteriors and external motive characters.

Durable journey transitions fire these hooks on the vehicle exterior:

| Event type | Ordered parameters |
| --- | --- |
| `VehicleJourneyDeparted` | `vehicle`, `journeyid`, `routeid`, `serviceid`, `vehicleid`, `stopcell`, `stoppositionmetres`, `message` |
| `VehicleJourneyArrived` | `vehicle`, `journeyid`, `routeid`, `serviceid`, `vehicleid`, `stopcell`, `stoppositionmetres`, `message` |
| `VehicleJourneyDelayChanged` | `vehicle`, `journeyid`, `routeid`, `serviceid`, `vehicleid`, `delay`, `message` |
| `VehicleJourneyCancelled` | `vehicle`, `journeyid`, `routeid`, `serviceid`, `vehicleid`, `reason` |
| `VehicleJourneyFaulted` | `vehicle`, `journeyid`, `routeid`, `serviceid`, `vehicleid`, `reason` |

Departure and arrival fire once per stop transition; arrival includes intermediate stops as well as the terminal. `delay` is the total accumulated `timespan`. An ordinary-cell stop uses `-1` for `stoppositionmetres`. The numeric journey, route, and service ids are deliberately legacy-compatible and can be passed directly to `vehiclejourney(number)`, `vehicleroute(number)`, and `vehicleservice(number)`.

## Acceptance Scenarios

Currently covered by implementation:

- Creating a vehicle prototype can link a valid exterior item prototype and automatically attach the internal vehicle exterior component.
- Vehicle prototype creation/submission requires an exterior item component linked to the vehicle prototype.
- Vehicle prototype submission validates RoomScale interior settings/connectivity/access, driver control stations, movement support, and finite movement, tow, stress, and damage values.
- Generic item loading is blocked for vehicle exterior shells through `PreventManualLoad`.
- Creating a vehicle instance creates a live `Vehicle` and exterior `GameItem`.
- Vehicle and exterior item have bidirectional links after factory creation.
- Save/load preserves prototype, exterior item id, canonical location, room layer, movement state, occupancies, and access rows.
- ItemScale vehicles can be boarded, controlled, moved through a valid cell exit, and exited.
- Drivers can explicitly release and take control, and players can inspect a compact vehicle status/readiness view.
- Occupied exterior items reject normal item pickup/repositioning.
- Occupied exterior items reject being hauled into containers.
- Forced relocation of the exterior item either moves visible occupants with the exterior to its new cell or clears occupancy if the exterior no longer has a cell location.
- Independent relocation of an occupant by teleport, transfer, arrest-like hardcoded moves, grapple dragging, combat knockdown, or similar cell-leave paths clears that occupant's vehicle occupancy.
- Vehicle cell-exit movement honours exterior item movement blockers and disconnects independent `IConnectable` links that would otherwise span cells.
- RoomContainer vehicles support multiple authored compartments, slots, and stations.
- Invalid cell-exit movement is blocked for controller mismatch, missing movement profile, too-small exit, invalid origin, and non-viable transition.
- Reboot recovery resynchronises the exterior item projection to the vehicle's canonical cell and layer; staff projection recovery recreates missing authored access/cargo projections without duplicating healthy ones.
- Builder/admin output distinguishes canonical vehicle state from item projection state.
- Factory creation builds and links access/cargo projection items.
- `hatch@vehicle`, `trunk@vehicle`, and installed module targeting resolves through normal item targeting.
- Access point projections implement normal open/close/lockable item behaviour while storing canonical state on `VehicleAccessPoint`.
- Cargo projections gate normal container components through vehicle access rules.
- Cargo and installation points with a required access point fail closed if that live access point is missing, disabled, locked, or closed.
- Installed modules use `VehicleInstallableGameItemComponent` and existing `install` / `uninstall` commands.
- Movement can be blocked by access denial, disabled/destroyed zones, damage-linked movement profiles, open must-close access points, missing or low-condition modules, missing roles, insufficient fuel, insufficient power, invalid recursive tow trains, or tow-stress catastrophe.
- Exterior item damage creates vehicle-zone wounds rather than ordinary exterior item wounds.
- Damage-zone effects disable linked access, cargo, installation, tow-point, movement-profile, or whole-vehicle movement targets.
- Generic `repair <vehicle exterior> with <kit>` and admin `vehicle repair <vehicle> damage <zone|all>` recalculate vehicle damage-zone status from remaining wounds while preserving manually disabled system flags.
- Hitch/unhitch persists tow links, validates recursive tow trains, records required physical hitch items for non-direct tow points, reserves those items while linked, evaluates valid strained links for catastrophe, and moves all linked towed vehicles.
- Active character/mount hitches let characters or mounts pull another character or a vehicle tow point through ordinary cell-exit movement, with `IHitchGear` or legacy `IDragAid` items for non-direct tow points and tow-point pull multipliers.
- Character/mount hitches can pull a vehicle that itself has downstream vehicle tow links; capacity, tow-point limits, hitch gear, exit size, damage, duplicate incoming links, and cycle checks use the whole unified train.
- Dragging a vehicle exterior through an ordinary movement command moves downstream towed vehicles and co-located hitch items exactly once, while the root vehicle still reconciles from the exterior item's movement.
- A scheduled multi-compartment RoomScale train boards from an ordinary platform, travels through one or more ten-kilometre RouteCells, reports holds and delay, docks at its destination, and resumes from a durable checkpoint after reboot.
- An occupied massive RoomScale platform moves manually through ordinary cell exits while all hosted interior cell ids and occupants remain stable.
- A mount-driven wagon and recursively hitched wagon train travels through chained RouteCells between distant towns, can stop at an exact mid-route coordinate, uses portal bands explicitly, and charges stamina/tow stress without duplicate checkpoint commits.

Still to implement:

- Optional player-facing vehicle retirement/deletion UX. V1 treats the canonical record as a durable asset: a missing or destroyed exterior fails movement closed and staff use `vehicle recover`/relink diagnostics rather than allowing an invisible vehicle to move.
- Rich physical access-device authoring, ownership, and lease tooling beyond simple character grants, presets, fleet helpers, and existing projection locks.
- Waves, swell, currents, weather-driven vessel motion, and other dynamic water-state effects.
- Damage to boats, capsizing, and boat-targeted weapon damage. The current boat-combat slice affects occupants only.
- Richer hitch item mechanics, broader recovery UX, and fuller fuel/power network topologies.
- Coordinate 2D and coordinate 3D systems.

For a fresh-world manual verification path, including the supporting item and component prototype authoring steps, see [Vehicle System Fresh MUD Test Runbook](./Vehicle_System_Fresh_MUD_Test_Runbook.md).

## Boat Combat And Directional Cover

Each occupant slot can offer a different `IRangedCover` from the same level, above, and below, plus a base boat-stability difficulty:

```text
vehicleproto set slot cover <slot-id> same <ranged-cover-id-or-name>
vehicleproto set slot cover <slot-id> above <ranged-cover-id-or-name|none>
vehicleproto set slot cover <slot-id> below <ranged-cover-id-or-name>
vehicleproto set slot stability <slot-id> <difficulty>
```

`all` may replace the directional token to set or clear all three cover values together. A surface swimmer is below an intact surface-water craft even though both are represented at `GroundLevel`; aerial and elevated attackers are above. Occupants aboard the same vehicle do not receive cover from their own vehicle against one another. Personal and slot cover do not stack: the effective cover is whichever is stronger, with hard cover winning an equal-difficulty soft/hard comparison.

Ordinary swimmer contact attacks cannot cross into the craft. Boarding or a specially authored aquatic vehicle natural attack is required; physical ranged attacks may still target occupants through directional cover. Successful unbalancing, knockdown, push, pull, throw, and aquatic vehicle assault effects call the slot stability check. Failure uses the existing water-safe force-disembark lifecycle and places the occupant swimming. A passed stability check during exit- or layer-based forced movement keeps the occupant aboard and prevents the independent teleport that would otherwise leave a stale occupancy behind. Disabled but intact boats still provide this support; destroyed boats do not.

Using `embark` during combat queues the same two-second, 5-stamina boarding action used by terrestrial-preferring combat AI and revalidates the chosen slot and access point before charging stamina. Outside combat, `embark` remains immediate. Manual combat commands bound to an aquatic vehicle attack may target an occupant; the move resolves against that occupant's vehicle exterior.

The `AquaticVehicleAttack` type attacks the exterior as a single action but does not damage it. One attack roll determines whether the assault rocks the craft, after which each occupant checks stability independently. Boat damage, capsizing, waves, and swell remain outside this slice.

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

Status: complete for the shared operational-readiness and cell-exit foundation used by V1.0.

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
- admin `vehicle access` grant/revoke/list/apply/clone commands and reusable access presets backed by existing `VehicleAccessStates` plus static configuration
- condition-aware vehicle installable component XML and builder commands for functional and movement thresholds
- player-facing exterior repair through ordinary repair kits, with vehicle damage-zone recalculation
- admin hitch repair that re-enables persistent links only after unified graph validation passes
- `vehicle show` readiness diagnostics covering access, modules, fuel, power, damage, repair hints, invalid links, and tow stress
- tow-stress policy configuration through static defaults and per-tow-point builder overrides
- `vehicle audit`, `vehicle recover`, and `vehicle fleet` helpers for staff support of local, zone, prototype, or global vehicle sets
- FutureProg predicates for vehicle identity, action readiness, start readiness, train weight, stress ratio, and readiness reason text
- explicit player control transfer and compact player-facing status/preflight diagnostics
- fail-closed required-access, exterior synchronisation, combat/action-blocker, required-crew, and delayed-movement commit validation
- revision-stable access/cargo projection markers whose live factory links target the exact child system record

Deliberate post-V1 extensions:

- rich physical access-device authoring, ownership, and lease tooling beyond simple character grants, presets, and existing projection locks
- fuller fuel, power, electrical, and liquid network topology support beyond installed candidate modules
- richer hitch item mechanics and broader recovery UX beyond the current stress-policy and staff recovery commands
- deletion/destruction lifecycle polish and deeper builder diagnostics for all system records

Phase 3 builds on this readiness service rather than introducing a second preflight path. A route departure, checkpoint, portal crossing, or resumed journey revalidates the same access, crew, damage, installation, propulsion, hitch, and tow-stress facts used by cell-exit movement.

Cargo and installed systems should reuse existing item/container/component infrastructure, but the vehicle decides which compartment or attachment point exposes each item capability.

### Phase 3: Route Movement

Status: V1.0.

Scope:

- first-class linear RouteCells with exact longitudinal coordinates and anchored exits
- continuous character, mount, hitch-cohort, and vehicle travel
- weighted hybrid spatial paths
- revisioned VehicleRoutes with ordered stops and pinned typed steps
- recurring schedules, boarding windows, holds, delays, cancellation, and restart-safe journeys
- builder route previews, validation, dependency invalidation, and diagnostics
- scheduled-train and horse-drawn wagon-train acceptance scenarios

### Phase 3B: RoomScale Interiors And Docking

Status: V1.0.

Scope:

- persistent hosted cells per compartment
- authored internal links and external access-point mappings
- stable occupants during exterior CellExit and RouteCell movement
- transient docking exits and platform presentation
- interior/docking recovery and deletion guards
- massive mobile-platform acceptance scenario

### Phase 4: Coordinate 2D Movement

Status: planned.

Scope:

- vehicles located by map coordinates
- projection into visible cells where appropriate
- speed, heading, and surface constraints
- transition between cell-exit and coordinate movement where needed

### Phase 5: Coordinate 3D

Status: planned.

Scope:

- aircraft, spacecraft, ships, elevators, and other freely positioned vehicles
- docking, berthing, attachment, and disconnection
- coordinate 3D movement and layered projection

## Design Rules

- Every vehicle has exactly one exterior item projection.
- The live `Vehicle` record is canonical for vehicle state.
- The item component is only a bridge.
- Player interaction routes through the exterior item unless an admin command explicitly targets a vehicle id.
- Occupants are vehicle occupancy records, not component XML.
- Future systems may reuse item/container/component infrastructure, but the vehicle owns the compartment, station, movement, access, and projection decisions.
