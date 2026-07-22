# Vehicle System Fresh MUD Test Runbook

## Purpose

This runbook gives a staff-facing, in-game verification path for the current vehicle system on a fresh MUD. It assumes the stock seeded item component prototypes from [Seeded_Item_Components.json](./Seeded_Item_Components.json) are installed, but it does not assume any vehicle-specific item prototypes already exist.

The goal is to verify the supported vehicle shapes end to end:

- `ItemScale` vehicles with an exterior item, one driver slot, one control station, boarding, driving, and disembarking.
- `RoomContainer` vehicles with compartments, access points, cargo projections, install points, damage effects, towing, and operational readiness diagnostics.
- `CellExit` movement through ordinary adjacent cell exits, using either `drive <direction>` or normal movement commands while controlling a vehicle.
- linear RouteCell movement with exact coordinates, named landmarks, and coordinate-banded portals.
- `RoomScale` vehicles with stable hosted interior cells, internal links, docking, boarding, and occupied movement.
- revisioned `VehicleRoute` services with stops, platforms, recurring departures, delay, and reboot recovery.
- Active character/mount hitches where a person, animal, or mount pulls a vehicle tow point, leads another hitched character, or pulls a vehicle that itself has downstream vehicle tow links.
- Operational readiness checks for access grants, module condition, fuel/power candidates, player repair, and tow-stress catastrophe recovery.

Coordinate 2D/3D movement, collision/signalling/dispatch, consist length, overtaking, fares, reservations, rich ownership or lease policy, rich parked-harness administration, and broad fuel or power network topology are outside this runbook.

## Prerequisites

Use an admin or builder account with permission to build item prototypes, component prototypes, vehicle prototypes, and to use admin wound and vehicle diagnostics commands.

Stand in a test cell that has at least one ordinary exit to another cell and a return exit back. The examples below use `north` and `south`; replace them with any valid directions in your test area.

Use item sizes that fit the test exit. On a fresh world, the easiest smoke test is to set the vehicle exterior item sizes to `normal` or another size that your test exit accepts. A too-small exit should block movement, but that is a separate negative test.

Pick one valid solid material from your game, for example with whatever local material listing command your game uses. The examples below use `<solid-material>` as a placeholder.

After each `item edit new`, `comp edit new`, or `vehicleproto new`, the command output gives the new prototype id. Record those ids as you go.

## Component Checklist

Confirm these seeded components exist:

```text
comp list holdable
comp list container
comp list +trunk
comp list dragaid
comp list hitchgear
comp list repairkit
```

The runbook uses:

- `Holdable` for crate, towbar, and installable module test items.
- `Container_Trunk` for the cargo projection item.
- `HitchGear_TowBar`, `HitchGear_Yoke`, `HitchGear_Harness`, `HitchGear_Rope`, or a legacy `DragAid` component for hitch tests. New content should prefer `HitchGear`.
- `Repair_Metal`, `Repair_Wood`, `Repair_Universal`, or another material-compatible `RepairKit` component for player-facing exterior repair tests.

If `Container_Trunk` is not installed, use any current component prototype of type `Container` instead.

Do not manually create or attach these vehicle projection components:

- `Vehicle Exterior`
- `Vehicle Access Point`
- `Vehicle Cargo Space`

The vehicle builder commands create and attach those internal components automatically. They are marked as internal projections and block ordinary manual item loading.

Create this component only if you want to test installed vehicle modules:

```text
comp typehelp VehicleInstallable
comp typehelp vehicle installable
comp edit new vehicle installable
comp set name QA Vehicle Engine Installable
comp set desc Makes a QA test engine installable in vehicle engine mounts.
comp set mount engine
comp set role engine
comp set mincondition 40%
comp set movementcondition 60%
comp edit submit fresh vehicle runbook module component
comp review <component id>
accept edit fresh vehicle runbook module component
```

Record the approved component id as `<engine-installable-component-id>`.

Expected result:

- Both type-help commands show the same vehicle installable help, including `mount <type>`, `role <role>`, `mincondition <percent>`, and `movementcondition <percent>`.
- `comp edit new vehicle installable` opens a `Vehicle Installable` component, not a `Vehicle Exterior` component.
- `comp set` or `comp set ?` while editing the component shows the type-specific help instead of the generic "does not yet have specific help" message.

If an older build has already created an accidental `Vehicle Exterior` component from `comp edit new vehicle installable`, clean it up before continuing:

```text
comp edit <bad component id>
comp edit delete
```

If the bad component has already been approved or attached to an item prototype, obsolete or detach it through the normal item/component builder workflow and create a fresh `Vehicle Installable` component with the commands above.

## Item Prototype Authoring Pattern

Use this pattern for each item prototype in the table below:

```text
item edit new
item set noun <noun>
item set sdesc <short description>
item set ldesc <long description>
item set desc
```

When the editor opens, enter a short full description, save it, and return to the command prompt.

Then finish the prototype:

```text
item set size <size>
item set weight <weight>
item set material <solid-material>
item set add <component name or id>
item edit submit fresh vehicle runbook item
item review <item prototype id>
accept edit fresh vehicle runbook item
```

Skip the `item set add` line when the item should be a plain projection shell. Add multiple components by repeating `item set add`.

### Required Item Prototypes

Build these item prototypes before creating vehicles:

| Placeholder | Purpose | Suggested Settings | Components |
| --- | --- | --- | --- |
| `<bicycle-hull-proto>` | `ItemScale` exterior item | noun `bicycle`, sdesc `a QA test bicycle`, size `normal`, weight `25 kg` | none, or `Holdable` if you want staff to pick it up before it becomes a vehicle exterior |
| `<car-hull-proto>` | `RoomContainer` exterior item | noun `car`, sdesc `a QA test car`, size `normal`, weight `900 kg` | none |
| `<hatch-proto>` | access point projection item | noun `hatch`, sdesc `a QA test car hatch`, size `normal`, weight `25 kg` | none |
| `<trunk-proto>` | cargo projection item | noun `trunk`, sdesc `a QA test car trunk`, size `normal`, weight `30 kg` | `Container_Trunk`, or another seeded `Container` component |
| `<engine-module-proto>` | installable module item | noun `engine`, sdesc `a QA test engine module`, size `normal`, weight `80 kg` | `Holdable`, `<engine-installable-component-id>` |
| `<towbar-proto>` | physical hitch item | noun `towbar`, sdesc `a QA test towbar`, size `normal`, weight `10 kg` | `Holdable`, `HitchGear_TowBar` |
| `<crate-proto>` | cargo contents | noun `crate`, sdesc `a QA test crate`, size `small`, weight `5 kg` | `Holdable` |
| `<cart-hull-proto>` | mount-pulled cart exterior item | noun `cart`, sdesc `a QA test hand cart`, size `normal`, weight `120 kg` | none |
| `<yoke-proto>` | optional mount hitch aid | noun `yoke`, sdesc `a QA test yoke`, size `normal`, weight `8 kg` | `Holdable`, `HitchGear_Yoke` or `HitchGear_Harness` |
| `<repair-kit-proto>` | exterior and hitch-item repair | noun `kit`, sdesc `a QA test repair kit`, size `small`, weight `2 kg` | `Holdable`, `Repair_Metal` for metal hulls or `Repair_Universal` for broad smoke tests |

After `vehicleproto set exterior`, `vehicleproto set access add`, or `vehicleproto set cargo add`, try this negative test:

```text
item load <car-hull-proto>
item load <hatch-proto>
item load <trunk-proto>
```

The exterior and projection items that have received internal vehicle components should refuse ordinary manual loading. Create live vehicle shells through `vehicleproto create`, not `item load`.

## ItemScale Vehicle Test

Create a basic bicycle-style vehicle:

```text
vehicleproto new QA Bicycle
vehicleproto set scale itemscale
vehicleproto set exterior <bicycle-hull-proto>
vehicleproto set compartment add rider
vehicleproto show <vehicle proto id>
vehicleproto set slot add <rider-compartment-id> driver 1 saddle
vehicleproto show <vehicle proto id>
vehicleproto set station add <driver-slot-id> handlebars
vehicleproto set movement cell
vehicleproto set damage add 50 1 30 50 false frame
vehicleproto submit fresh vehicle runbook itemscale
vehicleproto approve <vehicle proto id> fresh vehicle runbook itemscale
vehicleproto create <vehicle proto id>
```

Verify the live vehicle:

```text
vehicle list
vehicle show <vehicle id>
look
embark bicycle driver
vehiclestatus
vehiclecontrol release
north
vehiclecontrol
look
get bicycle
north
vehicle show <vehicle id>
south
disembark
get bicycle
```

Expected result:

- `vehicle show` lists the canonical vehicle state and linked exterior item.
- The exterior item moves with the vehicle.
- The driver moves with the vehicle.
- After boarding, the rider's room long description shows them riding the bicycle, and the bicycle is not repeated as a separate item line while it is already mentioned in that rider line.
- `get bicycle` while someone is riding it is rejected because the exterior item is occupied.
- `north` and `south` work as aliases for `drive north` and `drive south` while the rider controls the bicycle.
- `vehiclestatus` identifies the controller and reports a successful cell-exit preflight while controlled.
- after `vehiclecontrol release`, movement is rejected until an eligible driver uses `vehiclecontrol`; a passenger or driver slot without a configured station cannot take control.
- Vehicle movement has the normal movement rhythm: a begin/departure echo naming the rider and bicycle, movement delay, riding-style arrival echo such as `rides in from the South on a QA test bicycle`, and a refreshed look after arrival.
- `disembark` with no arguments executes the command and returns the character to ordinary cell presence beside the exterior item; it should not show the help file unless you ask for help.
- After disembarking, `get bicycle` follows ordinary item rules for the exterior item prototype. If the hull prototype was made holdable, staff/players with normal permissions can pick it up; if it was not holdable, the usual item-system refusal is expected.

## RoomContainer Vehicle Test

Create a car-style vehicle with access, cargo, an engine installation point, tow points, and a damage zone:

```text
vehicleproto new QA Test Car
vehicleproto set scale roomcontainer
vehicleproto set exterior <car-hull-proto>
vehicleproto set compartment add cabin
vehicleproto show <vehicle proto id>
vehicleproto set slot add <cabin-compartment-id> driver 1 driver seat
vehicleproto set slot add <cabin-compartment-id> passenger 3 passenger seats
vehicleproto show <vehicle proto id>
vehicleproto set station add <driver-slot-id> steering wheel
vehicleproto set movement cell
vehicleproto show <vehicle proto id>
vehicleproto set access add <cabin-compartment-id> door <hatch-proto> side hatch
vehicleproto show <vehicle proto id>
vehicleproto set cargo add <cabin-compartment-id> <access-point-id> <trunk-proto> trunk
vehicleproto set installpoint add <access-point-id> engine engine true engine bay
vehicleproto set tow add none hitch tow 2000 rear hitch
vehicleproto set tow add none hitch towed 2000 front hitch
vehicleproto set tow <rear-tow-point-id> stress warning 90%
vehicleproto set tow <rear-tow-point-id> stress failstart 95%
vehicleproto set tow <rear-tow-point-id> stress maxchance 25%
vehicleproto set tow <rear-tow-point-id> stress damage 2%
vehicleproto set movement role <movement-profile-id> engine
vehicleproto set movement access <movement-profile-id>
vehicleproto set movement tow <movement-profile-id>
vehicleproto set damage add 100 3 50 100 false body
vehicleproto show <vehicle proto id>
vehicleproto set damage <damage-zone-id> effect add access <access-point-id> disabled
vehicleproto set damage <damage-zone-id> effect add cargo <cargo-space-id> disabled
vehicleproto set damage <damage-zone-id> effect add install <installation-point-id> disabled
vehicleproto set damage <damage-zone-id> effect add tow <rear-tow-point-id> disabled
vehicleproto set damage <damage-zone-id> effect add movement <movement-profile-id> disabled
vehicleproto submit fresh vehicle runbook roomcontainer
vehicleproto approve <vehicle proto id> fresh vehicle runbook roomcontainer
vehicleproto create <vehicle proto id>
```

Notes:

- `vehicleproto set access add` automatically attaches the `Vehicle Access Point` projection component to `<hatch-proto>`.
- `vehicleproto set cargo add` automatically attaches the `Vehicle Cargo Space` projection component to `<trunk-proto>`, but the trunk item prototype must already have a normal container component.
- The movement profile role requirement means the car should not drive until the engine module is installed.

Verify access grants, access projections, and module gating:

```text
vehicle show <vehicle id>
vehicle access <vehicle id> list
vehicle access <vehicle id> grant <your character> board 1
vehicle access <vehicle id> grant <your character> control 2
vehicle access <vehicle id> grant <your character> service 2
vehicle access <vehicle id> grant <your character> repair 2
vehicle access <vehicle id> grant <your character> hitch 2
vehicle access <vehicle id> list
vehicle access preset list
vehicle access preset show crew
vehicle access <vehicle id> apply crew <your character>
vehicle audit here access
vehicle audit here readiness
embark car driver
open hatch@car
embark car driver via hatch
close hatch@car
north
open hatch@car
disembark
item load <engine-module-proto>
get engine
open hatch@car
install engine car engine
embark car driver via hatch
close hatch@car
drive north
south
disembark
open hatch@car
uninstall engine@car
```

Expected result:

- `vehicle access list` shows no rows as permissive, then shows each explicit grant after the `grant` commands. Admin characters still bypass operational access checks; use a non-admin test character if you want to verify denial.
- `vehicle access preset list|show` displays reusable presets from static configuration, and `vehicle access <vehicle> apply crew <character>` creates the expected board/control/service/hitch grants without adding a new access schema.
- `vehicle audit here access` reports permissive access when no rows exist and exact access findings once rows exist; `vehicle audit here readiness` reports the same readiness reasons shown by `vehicle show`.
- Once access rows exist, unlisted non-admin characters are blocked from board/control/service/repair/hitch actions with the readiness reason.
- Boarding is blocked while the required access point is closed.
- Opening `hatch@car` allows boarding.
- Movement is blocked until the required `engine` role is installed.
- Installed modules below their `movementcondition` threshold do not count for movement and appear in `vehicle show` readiness diagnostics.
- Closing the hatch allows movement when the movement profile requires access points closed.
- `drive north` and ordinary direction commands both invoke delayed vehicle movement while you control the car.
- `engine@car` resolves as an installed module projection.

Verify cargo projection behaviour:

```text
open hatch@car
item load <crate-proto>
get crate
put crate trunk@car
get crate trunk@car
close hatch@car
put crate trunk@car
```

Expected result:

- `trunk@car` resolves as a normal targetable container projection.
- Cargo contents are normal item/container state.
- If the trunk requires the hatch access point, closed or inaccessible access should block ordinary cargo use.

Verify damage linkage, player repair, and admin repair:

```text
item load <repair-kit-proto>
get kit
vehicle show <vehicle id>
wound car none slashing 60
vehicle show <vehicle id>
open hatch@car
embark car driver via hatch
repair car with kit
vehicle show <vehicle id>
vehicle repair <vehicle id> damage all
vehicle show <vehicle id>
open hatch@car
embark car driver via hatch
close hatch@car
drive north
```

Expected result:

- Exterior item damage creates vehicle-zone wounds.
- `vehicle show` reports damage-zone status, effective damage disables, operational readiness, and repair hints.
- Access, cargo, installation, tow, and movement profile effects are blocked while the damage effect is active.
- `repair car with kit` uses the ordinary item repair flow; after the repair effect completes or removes a wound, linked vehicle damage zones recalculate from remaining wounds.
- If the kit cannot repair the vehicle material, damage type, severity, or remaining repair points, the ordinary repair command explains why.
- `vehicle repair <vehicle> damage all` clears the damage-derived disables through the same recalculation path without clearing any manually disabled flags.

## Surface-Water Boat Tests

These tests require two adjacent cells whose `GroundLevel` is a swimming layer, plus an adjacent land cell. Use the normal room/terrain builder to establish that topology before creating the craft.

Create an exposed `ItemScale` surfboard by following the basic ItemScale workflow, then configure its cell-exit profile before submission:

```text
vehicleproto set movement environment <movement-profile-id> surfacewater
vehicleproto set movement waterexposure <movement-profile-id> exposed
vehicleproto set movement propulsion add <movement-profile-id> selfpowered
vehicleproto show <surfboard vehicle proto id>
vehicleproto set movement propulsion trait <selfpowered-propulsion-id> <paddling-trait-id-or-name>
vehicleproto set damage add 50 1 30 50 true board
vehicleproto show <surfboard vehicle proto id>
```

The new mode starts with a 10-second base time, the default speed formula `max(0.25, 1.0 + (0.15 * outcome))`, and the default stamina formula `swimcost * max(0.5, 1.0 - (0.10 * outcome))`. Use `movement propulsion time|difficulty|speed|stamina` with the propulsion id to test overrides.

Create a protected multi-rower `RoomContainer` rowboat by following the RoomContainer workflow with a hull, driver slot, station, two crew/passenger slots, and a globally movement-disabling hull damage zone, then configure:

```text
vehicleproto set movement environment <movement-profile-id> surfacewater
vehicleproto set movement waterexposure <movement-profile-id> protected
vehicleproto set movement propulsion add <movement-profile-id> rowed
vehicleproto show <rowboat vehicle proto id>
vehicleproto set movement propulsion trait <rowed-propulsion-id> <rowing-trait-id-or-name>
vehicleproto set slot propulsion <port-rower-slot-id>
vehicleproto set slot propulsion <starboard-rower-slot-id>
vehicleproto set damage add 100 1 50 100 true hull
vehicleproto show <boat vehicle proto id>
```

Create and submit a `Vehicle Oar` component prototype, setting `efficiency <multiplier>`, then attach it to a holdable oar item prototype. Give one rower two oars with different efficiency/condition values to verify that only the highest effective oar is selected. Give a second rower another oar to verify multiple contributors and diminishing returns.

Create a protected sailboat with auxiliary outboards. Its movement profile may author both modes, but exactly one must be the default:

```text
vehicleproto set movement environment <movement-profile-id> surfacewater
vehicleproto set movement waterexposure <movement-profile-id> protected
vehicleproto set movement propulsion add <movement-profile-id> sail
vehicleproto set movement propulsion add <movement-profile-id> outboard
vehicleproto show <sailboat vehicle proto id>
vehicleproto set movement propulsion default <sail-propulsion-id>
vehicleproto set installpoint add none outboard auxiliary false port motor mount
vehicleproto set installpoint add none outboard auxiliary false starboard motor mount
```

Create and submit two `Outboard Motor` component prototypes. Both parent items also need `Vehicle Installable` with mount `outboard`. Configure one with `fuel <liquid> <volume>` and a same-item liquid container, and the other with `power <watts>` and a same-item `BatteryPowered` or battery-fed `PowerSupply`. Set each positive `output <multiplier>`. An optional same-item `IOnOff` component must be on. Install both motors on the sailboat.

Finally create a protected non-self-moving barge:

```text
vehicleproto set movement environment <movement-profile-id> surfacewater
vehicleproto set movement waterexposure <movement-profile-id> protected
vehicleproto set movement propulsion add <movement-profile-id> none
vehicleproto show <barge vehicle proto id>
```

Submission should reject `None` combined with another mode, duplicate mode rows, a non-positive speed expression, a negative stamina expression, or a self-powered/rowed mode with no trait. Returning a profile to `unrestricted` requires removing its propulsion rows first and resets water exposure to protected.

Create both vehicles at the surface-water test cell. Verify the movement profile display reports `Surface Water` and the expected occupant exposure policy. Then run:

```text
embark surfboard driver
vehiclepropulsion
drive <water-exit>
vehiclestatus
disembark
embark boat driver
vehiclepropulsion
drive <water-exit>
vehiclestatus
```

Expected result:

- Both craft remain at `GroundLevel`, their exterior items remain floating even if their materials would otherwise have negative buoyancy, and their occupants do not spend swimming stamina or sink.
- The surfboard occupant receives normal continuous terrain-water exposure while aboard.
- The protected boat occupant does not receive terrain-water exposure while aboard. Boarding does not remove water that was already present, and rain or deliberate spills remain ordinary liquid sources.
- Disembarking either craft at the water surface sets the character to swimming immediately.
- The surfboard makes one `PaddleVehicleCheck`, charges the resulting stamina once, and uses its explicit base time rather than walking speed. Cancelling after departure does not refund the charge.
- The rowboat ignores exhausted, incapacitated, in-combat, oarless, or non-designated occupants. Each ready rower checks once; additional effective oars improve speed with square-root diminishing returns.

Test the sailboat in still weather and then in progressively stronger wind:

```text
embark sailboat driver
vehiclepropulsion sail
drive <water-exit>
<set origin weather to OccasionalBreeze>
drive <water-exit>
<set origin weather to GaleWind>
drive <water-exit>
vehiclepropulsion outboard
drive <water-exit>
```

Expected result: sail is blocked at `None` or `Still`, becomes faster as departure wind rank rises, and retains the departure sample if weather changes during the movement delay. Outboard mode reports both motors and their resource candidates, sums ready output linearly, consumes the configured liquid and power spike once, ignores a switched-off/damaged/unfuelled motor with an explicit reason, and blocks only when no motor remains ready. The chosen mode never silently falls back to the other.

Test the barge as both root and tow target. `vehiclepropulsion` reports `None`; direct `drive` is blocked. A valid driven tow vehicle can still tow it water-to-water, and ordinary unoccupied item handling remains available where the hull's item components permit it.

Attempt each operational path toward the land cell:

```text
drive <land-exit>
drag surfboard <land-exit>
hitch <tow-source> <surfboard-tow-point>
drive <land-exit>
```

Expected result: driven, character-dragged, and tow-train movement all fail before resource consumption or relocation because every surface-water vehicle in the move must start and finish at the ground-level water surface. After disembarking, ordinary `get`, inventory, containment, staff relocation, or creation rules can still place the unoccupied surfboard on land; it remains unable to drive or be dragged through exits there.

Finally, return the protected boat to the water surface, board it, and apply enough exterior damage to take its globally movement-disabling hull zone to `Destroyed`:

```text
wound <boat> none slashing 100
vehicle show <boat vehicle id>
```

Expected result: the controller and all passengers are disembarked exactly once into the boat's canonical cell and layer, vehicle control/movement is cleared, and each character is set to swimming. Deleting or destroying the exterior item follows the same water-safe disembark rule. A destroyed hull is no longer exempt from ordinary item sinking.

### Boat Combat And Directional Cover Check

Configure the rowboat's driver and passenger slots with distinct cover and stability values before submission:

```text
vehicleproto set slot cover <driver-slot-id> same <partial-hard-cover>
vehicleproto set slot cover <driver-slot-id> above none
vehicleproto set slot cover <driver-slot-id> below <total-hard-cover>
vehicleproto set slot stability <driver-slot-id> normal
vehicleproto set slot cover <passenger-slot-id> all <partial-soft-cover>
vehicleproto set slot stability <passenger-slot-id> easy
```

Board two test occupants. From another occupant in the same boat, confirm ranged attacks do not use the boat's cover. From the water surface, confirm physical ranged attacks use the slot's `below` cover. Repeat from `InAir` or another elevated layer and confirm the `above` definition is selected. Give the target ordinary personal cover and confirm only the stronger of personal and boat cover applies.

As an unsupported swimmer, attempt an ordinary melee attack, clinch, grapple, and charge against an occupant. Each must explain that boarding is required. Set an NPC combat setting to `combat config terrestrial true`; it should spend a two-second combat action and 5 stamina to board a stationary craft, but no stamina if boarding preflight fails. Set an aquatic predator to a `Beast Aquatic ...` setting; it should use `Aquatic Hull Assault` against the exterior instead of boarding.

Resolve unbalancing, knockdown, push, pull, takedown/throw, and aquatic hull-assault successes against occupants. Each affected occupant makes one independent `BoatStabilityCheck`; a failure force-disembarks exactly once into swimming posture while a success leaves the occupant aboard and preserves the attack's ordinary non-overboard consequences. An aquatic hull assault makes one attack roll for the craft, applies no exterior damage, ignores ranged cover, and can knock any number of occupants overboard through their separate stability results.

## Tow Train Test

For a tow train, create at least two live vehicles with compatible tow points. The `QA Test Car` prototype above can be used for both source and target if it has one `tow` point and one `towed` point, but a dedicated trailer prototype is cleaner.

Create a trailer by reusing the RoomContainer pattern with:

- exterior item prototype: a plain trailer hull item
- one driver slot and station only if you want it independently drivable
- one `towed` front hitch tow point
- optional cargo projection

Then create two or three live vehicles in the same cell and test:

```text
item load <towbar-proto>
get towbar
hitch rear@car front@trailer with towbar
vehicle show <car vehicle id>
embark car driver via hatch
close hatch@car
north
vehicle show <car vehicle id>
vehicle show <trailer vehicle id>
drive south
unhitch front@trailer
```

Expected result:

- Hitching non-direct tow points requires a compatible hitch item to be held and hitch access when explicit vehicle access rows exist.
- The hitch item is dropped into the tow train's location, receives an in-use/no-get effect, and is persisted on the tow link.
- Movement moves the full recursive tow train and occupants together.
- `vehicle show` reports invalid links and warns about valid but stressed links near their effective tow-point capacity.
- Tow-point stress settings on `vehicleproto set tow <id> stress ...` override the global static tow-stress defaults for links that use that point.
- If the hitch item is removed from the train location, destroyed, or deleted, movement should block and `vehicle show` should report an invalid tow-link cause.
- `unhitch <vehicle>` removes all links involving that vehicle, while `unhitch <towpoint>@<vehicle>` removes only links using that point.

### Tow Catastrophe And Recovery Check

Tow catastrophe is probabilistic and only applies to valid but strained links. To smoke-test the recovery path, use a trailer or cargo load that brings a link above 95% of its effective tow-point capacity, then attempt movement a few times in a safe test area:

```text
vehicle show <car vehicle id>
drive north
vehicle show <car vehicle id>
repair towbar with kit
vehicle repair <car vehicle id> hitch all
vehicle recover <car vehicle id> hitch fix
vehicle fleet here recover hitch
vehicle show <car vehicle id>
unhitch front@trailer
```

Expected result:

- Hard invalid states still fail before any catastrophe roll: missing gear, destroyed gear, missing endpoints, over-capacity links, incompatible tow points, cycles, duplicate incoming links, and blocked exits.
- A stressed but valid link may catastrophically fail before movement and before fuel or power is consumed.
- On catastrophe, the room receives a failure echo, the hitch item or linked exteriors take shearing damage, persistent links are disabled, transient hitch/drag effects are cleared, and reserved hitch items are released.
- `vehicle show` reports the disabled link and recovery hint.
- After physical repair, `vehicle repair <vehicle> hitch all` or `vehicle recover <vehicle> hitch fix` re-enables only links that pass unified graph validation; otherwise it leaves the link disabled with the validation reason.
- `vehicle fleet here recover hitch` reports the same hitch recovery findings across all vehicles in the current cell without applying fixes unless `fix` is included.

## Route-Ready Operations Smoke Test

After creating at least one car and one trailer, verify the closeout staff tools:

```text
vehicle audit here all
vehicle audit zone resources
vehicle audit prototype <vehicle proto id> hitch
vehicle fleet here access apply crew <your character>
vehicle fleet here access grant <your character> repair 2
vehicle fleet here access revoke <your character> repair
vehicle fleet here recover all
vehicle fleet here recover hitch fix
```

Expected result:

- Scope selectors cover vehicles in the current cell, current zone, a vehicle prototype family, or all loaded vehicles.
- Audit output groups findings by vehicle and uses the same subsystem and reason vocabulary as `vehicle show`.
- Fleet access operations batch the same grant, revoke, apply, and clone behaviours as single-vehicle `vehicle access` commands.
- Recovery commands report projection, install, and hitch findings; `fix` only mutates validated persistent hitch/tow links and leaves hard-invalid links disabled with their exact validation reason.

For route and automation scripting, verify these built-in FutureProg function names are available in your local FutureProg function help/listing workflow:

```text
isvehicle(item)
vehiclecanboard(character, item)
vehiclecancontrol(character, item)
vehiclecanservice(character, item)
vehiclecanrepair(character, item)
vehiclecanhitch(character, item)
vehiclecanstart(character, item)
vehiclereadinessreason(character, item, "start")
vehicletrainweight(item)
vehicletowstress(item)
```

Expected result:

- The readiness predicates accept vehicle exterior items and return cell-exit operational readiness without requiring route movement to exist.
- `vehiclereadinessreason` returns blank text when ready and the same blocking reason shown by commands when not ready.
- `vehicletrainweight` and `vehicletowstress` expose the unified hitch graph's effective train weight and highest stress ratio for route preflight progs.
## Mount-Hitched Cart Test

This is now an active movement hitch, not a persisted vehicle-to-vehicle tow link. It is intended for live scenes such as a horse pulling a cart, an ox team pulling a wagon, or a person pulling a hand cart. The relationship uses the normal drag/movement system, so it is cleared by `unhitch`, `stop`, drag invalidation, reboot, or ordinary effect cleanup. Use persisted vehicle tow links for parked trailer chains between vehicles.

Prepare one local character, NPC, or mount with enough drag capacity to pull the cart. The examples below call it `horse`; replace that keyword with whatever mount or test character exists in your fresh world. If your fresh world has no mount/NPC creation workflow prepared yet, use a second staff-controlled test character with high enough drag capacity for the smoke test.

Create a simple cart vehicle:

```text
vehicleproto new QA Hand Cart
vehicleproto set scale itemscale
vehicleproto set exterior <cart-hull-proto>
vehicleproto set compartment add cart
vehicleproto show <vehicle proto id>
vehicleproto set slot add <cart-compartment-id> driver 1 cart handle
vehicleproto set station add <cart-driver-slot-id> handles
vehicleproto set movement cell
vehicleproto set tow add none hand towed 300000 pull 4 shafts
vehicleproto set tow add none yoke towed 600000 pull 6 yoke
vehicleproto set damage add 50 1 30 50 false frame
vehicleproto submit fresh vehicle runbook mount cart
vehicleproto approve <vehicle proto id> fresh vehicle runbook mount cart
vehicleproto create <vehicle proto id>
vehicle show <cart vehicle id>
```

The tow point maximum weight argument is currently entered in the engine's base mass units. For the suggested `120 kg` cart item, use `300000` and `600000` rather than `300` and `600`.

Direct hand/shaft hitch test:

```text
look
hitch horse shafts@cart
north
look
vehicle show <cart vehicle id>
south
unhitch horse
```

Expected result:

- `hitch horse shafts@cart` succeeds without a hitch item because `hand` tow points are direct character/mount hitches.
- If `horse` does not trust the actor, is not helpless, and is not a mount the actor can control or mount, the command creates an `accept` proposal on `horse` instead of applying the hitch immediately.
- Moving the horse, rider, or controlled mount through an ordinary direction command pulls the cart exterior through the exit.
- The cart vehicle's canonical location follows the exterior item after movement.
- `unhitch horse` clears the active hitch. `stop` should also clear the ordinary drag effect and its vehicle hitch multiplier.

Item-required yoke/harness test:

```text
hitch horse yoke@cart
item load <yoke-proto>
get yoke
hitch horse yoke@cart with yoke
north
vehicle show <cart vehicle id>
unhitch yoke@cart
```

Expected result:

- `hitch horse yoke@cart` is rejected because `yoke` tow points require a compatible hitch item.
- `hitch horse yoke@cart with yoke` succeeds if the yoke item has an approved `HitchGear` yoke/harness component or a compatible legacy `DragAid` component and is visible/available to the actor.
- The tow point's `pull <multiplier>` setting reduces the cart's effective pulled weight for the horse/mount capacity check.
- `unhitch yoke@cart` removes the active character/mount hitch for that tow point.

Character chain test:

```text
hitch horse ox
hitch ox shafts@cart
north
look
south
unhitch horse
unhitch ox
```

Expected result:

- A hitched character/mount can itself be the source for another hitch, so chains such as `horse -> ox -> cart` can move through normal cell exits.
- Each target can only have one incoming drag/hitch link, and each source can only pull one target at a time.
- Movement still uses normal character and mount movement validation. Combat, blocking position changes, missing hitch access, closed required vehicle access, damage-disabled tow points, invalid downstream trains, tow catastrophe, or insufficient drag capacity should block the hitch or movement.

Current limitations:

- PC-inclusive active character/mount hitches are not persisted reboot-safe tow-link records. NPC-only hitches can persist through `VehicleHitchLinks`, while live PC hitches are recovered only as ordinary transient effects during that session.
- A character/mount hitch can pull a vehicle that already has downstream vehicle-to-vehicle tow links. Movement preflight checks the whole unified train for total weight, exit size, damage-disabled tow points, incompatible or missing hitch gear, duplicate incoming links, and cycles.
- The hitch item is reserved with a no-get effect while the active link exists. NPC-only persistent hitches save the hitch item id; PC-inclusive hitches remain transient and are cleared by reboot.

Optional mixed-train check:

```text
item load <towbar-proto>
get towbar
hitch rear@cart front@trailer with towbar
vehicle show <cart vehicle id>
hitch horse shafts@cart
north
vehicle show <cart vehicle id>
vehicle show <trailer vehicle id>
south
unhitch horse
unhitch front@trailer
```

Expected result:

- The horse/mount pulls the cart and the cart's downstream trailer together through ordinary character movement.
- The cart and trailer canonical locations update to the destination cell.
- The towbar and any loose character-hitch item move with the chain unless they are worn/carried by an endpoint.
- If the combined train is too heavy, a hitch item is missing/destroyed, a linked vehicle cannot fit through the exit, or a valid stressed link catastrophically fails, movement is blocked before any vehicle or hitch item moves.

## RouteCell Topology Fixture

Build this fixture once for the train and wagon tests. Use two ordinary platform/town cells and one or more linear cells between them. Create each RouteCell before linking exits into it.

In the first linear cell:

```text
cell set route create 10km
cell set route direction positive eastbound
cell set route direction negative westbound
cell set route roomequivalent 100m
cell set route landmark add 0m "West Terminus"
cell set route landmark add 7150m "Old Quarry Turn-Off"
cell set route landmark add 10km "East Terminus"
cell set route show
cell set route map
```

Create/link the west platform, east platform, and optional quarry side cell using the normal room builder. Back in the RouteCell, anchor all three route-side exits:

```text
cell set route exit west band 0m 2m arrival 0m
cell set route exit quarry band 7100m 7200m arrival 7150m
cell set route exit east band 9998m 10km arrival 10km
cell set route validate
```

Expected result:

- `travel forward` at 5,000m moves toward the positive endpoint but does not traverse the east exit merely because the movement reaches its band.
- `look forward` and `scan forward` can describe visible landmarks/exits with localised distances.
- `travel to quarry` reaches the nearest point in the 7,100-7,200m band and stops; an ordinary exit command then traverses it.
- Entering from the quarry side arrives at 7,150m.
- `cell set route validate` reports no unanchored topology and the map orders endpoints, landmarks, and portal bands by coordinate.

For chained RouteCells, create the second cell before linking it. Anchor the shared exit independently on both sides, then validate both cells. The shared portal must never make either long cell cost one ordinary room.

## Scheduled RoomScale Train Test

Create a train prototype using the existing exterior/slot/access patterns, then add persistent interior settings, a compartment link, and an automatic-capable Route profile:

```text
vehicleproto new QA Scheduled Train
vehicleproto set scale roomscale
vehicleproto set exterior <train-hull-proto>
vehicleproto set compartment add cab
vehicleproto set compartment add carriage
vehicleproto show <train-proto-id>
vehicleproto set compartment interior <cab-id> <indoor-terrain> indoors
vehicleproto set compartment interior <carriage-id> <indoor-terrain> indoors
vehicleproto set compartment link add <cab-id> <carriage-id> forward backward "the passenger carriage" "the driver's cab"
vehicleproto set slot add <cab-id> driver 1 controls
vehicleproto show <train-proto-id>
vehicleproto set station add <driver-slot-id> controls
vehicleproto set slot add <carriage-id> passenger 20 seats
vehicleproto set access add <carriage-id> door <train-door-proto> passenger doors
vehicleproto set movement route
vehicleproto set movement route speed 20m/s
vehicleproto set movement route propulsion powered
vehicleproto set movement route power 50000
vehicleproto set movement route automatic
vehicleproto submit fresh runbook scheduled train
vehicleproto approve <train-proto-id> fresh runbook scheduled train
```

Stand in the RouteCell at the west terminus and create the train. Record the vehicle id and verify `vehicle show` lists stable interior cell ids, a connected interior graph, exterior coordinate `0m`, and no docking faults.

Author the operational route and service:

```text
vehicleroute edit new QA Intertown Line
vehicleroute set stop add "West Station" <route-cell-id> GroundLevel at 0m
vehicleroute set stop platform add "West Station" <west-platform-cell-id> <train-access-point-id> 2m
vehicleroute set stop dwell "West Station" 2m
vehicleroute set stop add "East Station" <route-cell-id> GroundLevel at 10km
vehicleroute set stop platform add "East Station" <east-platform-cell-id> <train-access-point-id> 2m
vehicleroute set stop dwell "East Station" 2m
vehicleroute set leg compile "West Station" "East Station"
vehicleroute preview <route-id>
vehicleroute validate
vehicleroute submit fresh runbook intertown route
vehicleroute approve <route-id> fresh runbook intertown route
vehicleservice new QA Intertown Service | <approved-route-id> | <train-vehicle-id> | <game-clock departure> | every 15 minutes
vehicleservice set operator automatic
vehicleservice set maxhold 15m
vehicleservice set enabled true
vehicleservice audit "QA Intertown Service"
vehicleservice show "QA Intertown Service"
```

`vehicleroute` stop positions and docking tolerances accept either numeric metres or normal length expressions such as `10km` and `2m`. The service reference departure must use the current location's game clock/calendar syntax. Replace the example recurrence if a 15-minute service is unsuitable for that calendar.

From the west platform, verify:

```text
transit departures east
transit services east
transit status "QA Intertown Service"
look
embark train passenger
vehiclestatus
```

Expected result:

- The platform shows one docked train supplied by the docking service, not a duplicate exterior item.
- `embark train passenger` resolves the open platform docking even though the train exterior item remains in the RouteCell. Embarking crosses that transient docking exit into the persistent carriage cell; `disembark` is available only while the matching compartment docking is open and returns the passenger to the authored platform cell.
- The journey transitions Scheduled -> Boarding -> Departing -> EnRoute -> Dwelling/Arrived and reports scheduled time, expected time, next stop, platform, and delay.
- The exterior coordinate interpolates continuously while carriage and cab cell ids/occupants remain unchanged.
- Disabling a required module before departure enters Held, retries every 30 seconds, and cancels after maximum hold; repairing it within the hold window permits departure.
- A graceful reboot commits the exact coordinate. A crash-style restart resumes the last durable checkpoint without downtime travel and adds downtime to journey delay. Fuel/power for an already committed checkpoint is not charged twice.
- Editing RouteCell topology invalidates later departures until a new route revision is compiled/submitted; an active journey never silently reroutes.
- Submitting a newer route revision does not invalidate a service pinned to the older approved (`Revised`) revision. Disable the service and finish or cancel its active journey before deliberately assigning a different route revision or vehicle.

## Massive Mobile Platform Test

Create a RoomScale platform with at least two linked interior compartments, one external ramp, a driver/control station, and a normal CellExit profile. Create it in the first of two ordinary adjacent cells.

```text
vehicleproto new QA Massive Platform
vehicleproto set scale roomscale
vehicleproto set exterior <platform-hull-proto>
vehicleproto set compartment add control
vehicleproto set compartment add deckhouse
vehicleproto show <platform-proto-id>
vehicleproto set compartment interior <control-id> <indoor-terrain> indoors
vehicleproto set compartment interior <deckhouse-id> <indoor-terrain> indoors
vehicleproto set compartment link add <control-id> <deckhouse-id> aft forward "the deckhouse" "the control room"
vehicleproto set slot add <control-id> driver 1 controls
vehicleproto show <platform-proto-id>
vehicleproto set station add <driver-slot-id> controls
vehicleproto set slot add <deckhouse-id> passenger 20 berths
vehicleproto set access add <deckhouse-id> ramp <platform-ramp-proto> loading ramp
vehicleproto set movement cell
vehicleproto submit fresh runbook massive platform
vehicleproto approve <platform-proto-id> fresh runbook massive platform
vehicleproto create <platform-proto-id>
```

Board the control station and place a second character and a loose item in the deckhouse. Record both interior cell ids, then:

```text
vehicle show <platform-vehicle-id>
look ramp
embark platform driver via ramp
enter forward
vehiclecontrol
drive north
vehicle show <platform-vehicle-id>
vehicle audit <platform-vehicle-id> interior
vehicle audit <platform-vehicle-id> docking
```

Expected result:

- The ramp projection is targetable by the ordinary `ramp` keyword while the projection itself remains out of the cell's top-level item list. It inherits the platform exterior's cell, layer, and RouteCell coordinate after every move; `ramp@platform` remains a valid disambiguating form.
- The Deckhouse ramp is allowed to reach the driver slot through the live explicit Deckhouse-Control link. Embarking lands in Deckhouse, the ramp's authored docking destination, and reserves the driver assignment without granting control there. Moving `enter forward` into Control and using `vehiclecontrol` establishes a physically valid controller.
- Ordinary movement accepts the explicit `enter forward` because both ends of that link are hosted cells of the same RoomScale vehicle. Attempting to walk through the transient docking exit remains blocked; leaving through it requires `disembark`. ItemScale and RoomContainer occupants remain unable to walk while boarded. A current controller should use `enter <internal-exit>` for compartment movement because a bare direction is intentionally interpreted as a request to drive the vehicle.
- A controller in the interior control cell can resolve and traverse the exterior's north exit.
- Only the exterior projection and exterior hitch cohort change ordinary cells.
- Both interior ids, occupants, and loose contents remain stable; internal and external movement messages have separate scopes.
- Docking links disappear before movement and rebuild at arrival.
- Destroying or unlinking the exterior stops further movement without deleting interiors. `vehicle recover <vehicle> interior|docking|all [fix]` reports and safely repairs recoverable faults. To exercise interrupted-creation recovery, clear a compartment's live `InteriorCellId` while retaining its cell's hosted-vehicle/compartment ownership in a disposable database: `interior fix` must relink that same cell id, never create another cell. If the owned cell is not loaded or is claimed by another compartment, recovery must fail closed with an actionable reason.
- If factory creation is interrupted after the live access/cargo rows exist but before their authored item projections are linked, `vehicle recover <vehicle> projection` must list each missing projection and `vehicle recover <vehicle> projection fix` must create and link it. Record the new item ids, run the same fix again, and verify that it reports no projection finding and creates no additional items. A missing exterior is intentionally not recreated by this action; repair it with `vehicle relink`.
- Vehicle deletion/retirement is refused while an interior is occupied, a journey is active, any service still references the vehicle, or a hitch remains.

## Horse-Drawn Route Wagon Train Test

Create two RoomContainer wagons with externally pulled Route profiles and compatible recursive tow points. Use physical yoke/harness/towbar items as required by the authored tow types. Place the lead wagon and mount in the west town/platform cell.

If the fresh world has no suitable mount, this minimal builder sequence creates a functional mount fixture. The maintained blank snapshot contains only the Humanoid, Organic Humanoid, and Human races, so the exact fresh-world fixture clones the admin character and presents it as a draught horse while exercising the real mount AI, stamina, speed, and hitch systems. A game with an authored horse race should select that race and its matching culture/ethnicity instead. Keep every attribute within the selected race's individual cap; `30` is valid for the seeded Human race. `ai edit new` creates a non-revisioned AI and `npc make` creates a current template immediately, so a correct first-pass fixture does not need review. If an already-current NPC template is revised later, submit and approve that new NPC revision normally.

```text
ai edit new mount "QA Wagon Horse Mount"
ai set permit AlwaysTrue
ai set control AlwaysTrue
ai close
npc make 1 QA-Horse
npc set unique qa-draught-horse
npc set gender male
npc set randomise
npc set name Japheth
npc set sdesc a QA draught horse
npc set attribute str 30
npc set ai add "QA Wagon Horse Mount"
npc edit close
npc load qa-draught-horse
mount horse
dismount
force horse speed gallop
```

```text
vehicleproto set movement route
vehicleproto set movement route propulsion externallypulled
vehicleproto set movement route speed 4m/s
vehicleproto set tow add none harness towed 1000000 pull 2 shafts
vehicleproto set tow add none towbar tow 1000000 rear
vehicleproto set tow add none towbar towed 1000000 front
vehicleproto submit fresh runbook route wagon
vehicleproto approve <wagon-proto-id> fresh runbook route wagon
vehicleproto create <wagon-proto-id>
vehicleproto create <wagon-proto-id>
hitch rear@lead-wagon front@rear-wagon with towbar
hitch horse shafts@lead-wagon with harness
embark lead-wagon driver
```

Build the vehicle-to-vehicle chain before attaching the motive character. A character-to-vehicle hitch deliberately marks the lead vehicle as dragged; subsequently trying to add a downstream vehicle from that already-dragged root is rejected. The tow-point builder currently stores maximum weights in engine base mass units, so size the number against the item prototype's displayed base weight and the complete downstream train; the `1000000` fixture values above safely exceed two `200000`-unit QA wagon exteriors. A tow-point character pull multiplier greater than `1` increases effective pull capacity by dividing the train's effective weight; values below `1` are accepted as authored data but clamp to `1` at runtime and provide no benefit. The authored Route-profile speed is used only for powered propulsion; an externally pulled train uses the mount's current movement speed.

Author/approve a two-stop wagon route from the west town cell, through the anchored RouteCell exits, to the east town cell. Compile the leg so its preview contains `CellExitStep`, exact `LinearRouteStep` metres, and the terminal `CellExitStep`.

```text
drive route <approved-wagon-route>
vehiclestatus
drive stop
vehiclestatus
drive route <approved-wagon-route>
drive stop
drive forward 1km
drive to "Old Quarry Turn-Off"
drive route <approved-wagon-route>
```

After each `drive route`, wait until `vehiclestatus` shows that the coordinate has advanced before issuing `drive stop`. A ten-kilometre acceptance trip runs in real time at the mount's speed; for a short checkpoint smoke, use a controlled fast mount, travel at least 300m, and allow more than the default 30-second checkpoint interval before stopping. Do not record that smoke as a completed intertown arrival.

Expected result:

- The mount is the motive root; the driver may remain aboard the wagon.
- Horse, rider/driver, physical hitch gear, lead wagon, downstream wagon, and exterior contents share one reference coordinate during longitudinal movement.
- `drive stop` commits an exact mid-route coordinate. Reissuing `drive route <approved-wagon-route>` starts a new durable operation at that coordinate and resumes the unique remaining pinned step rather than rewinding to an ordinary-room boundary. A coordinate outside the compiled leg, matching multiple continuations, already at the terminal end, or using an invalidated topology fails closed with an actionable reason.
- Portal traversal occurs only as the compiled explicit exit step; passing the quarry band does not take its exit. At a compiled portal the mount, party/riders, physical gear, and recursive vehicle train cross atomically. The exact graph/readiness reason is shown if the portal preflight fails.
- Stamina and tow stress are charged by committed distance. Exhaustion, damaged connectors, over-capacity tow points, missing gear, fuel/power policy, or a catastrophe stops the whole cohort at one committed coordinate with an actionable reason and no duplicate charge.
- Arrival through the terminal anchor places the complete cohort in the east town cell and preserves the recursive hitch graph.

The compiled route is directional. Author and approve a separate reverse route (and, if desired, a separate service) for the east-to-west journey; V1 does not infer or dynamically reverse service legs.

## Negative Tests

Run these as separate checks after the positive path:

```text
item load <car-hull-proto>
```

```text
disembark
drive north
north
```

```text
open hatch@car
embark car driver via hatch
north
close hatch@car
```

```text
wound car none slashing 60
vehicle show <vehicle id>
open hatch@car
embark car driver via hatch
vehicle repair <vehicle id> damage all
```

```text
embark bicycle driver
get bicycle
disembark
```

```text
vehicle access <vehicle id> grant <other-character> board 1
vehicle access <vehicle id> list
```

Expected result:

- Generic item loading of a vehicle exterior shell is blocked.
- Driving while not controlling a vehicle is blocked; ordinary movement commands fall back to normal walking when you are not controlling a vehicle.
- Driving with required access points open is blocked.
- Damage-linked systems are reported as disabled in `vehicle show`; in this runbook's car prototype, the body damage zone disables access, cargo, install, tow, and movement together.
- Admin damage repair restores damage-derived access and movement if the only blocking cause was damage-derived disablement.
- A non-admin character with only `board` access can board when projection access allows it, but cannot control, install/uninstall, repair, hitch, or unhitch until granted the matching level 2 tag or `all`.
- Occupied vehicle exteriors cannot be picked up or normally repositioned.

## Edge Case Checks

These checks focus on hardcoded movement and item-manipulation paths that can bypass ordinary `drive` and `disembark` flows.

Run these against the `QA Bicycle` after the basic `ItemScale` test:

```text
embark bicycle driver
sit
```

Expected result: the position command is rejected with a message telling you to disembark first.

```text
goto <different-test-cell>
vehicle show <vehicle id>
```

Expected result: the character transfers normally, the bicycle does not follow, and `vehicle show` no longer lists the transferred character as an occupant or controller. Return to the bicycle's cell before continuing.

```text
embark bicycle driver
drag bicycle north
look
south
drag bicycle south
```

Expected result: if the bicycle exterior is holdable/draggable and the normal drag rules allow the drag, the bicycle and rider arrive together. The vehicle's canonical location follows the exterior item.

```text
embark bicycle driver
haul bicycle <container>
```

Expected result: hauling the occupied bicycle into a container is rejected. Disembarking first restores ordinary haul/get/container behaviour according to the item prototype's normal rules.

If your test world has a connectable charger/outlet item and a connectable vehicle exterior, add this optional check:

```text
connect <charger> bicycle
embark bicycle driver
north
```

Expected result: if the connection itself blocks movement, driving is rejected with the item-system blocker. If it does not block movement, the independent connection is disconnected when the vehicle exterior changes cells; it should not remain logically connected to an item in the previous room.

## Currently Fully Supported Vehicle Kinds

The current implementation should be considered fully supported for:

- simple `ItemScale` vehicles that board one or more occupants, expose a driver station, and move through cell exits;
- exposed `ItemScale` surface-water craft such as surfboards, with floating support but normal ambient water exposure for occupants;
- `RoomContainer` vehicles represented by one exterior item, with authored compartments, slots, stations, access projections, cargo projections, installation points, damage effects, tow points, operational readiness checks, and cell-exit movement;
- protected `RoomContainer` surface-water boats whose occupants avoid swimming, sinking, and ambient terrain-water exposure while the craft remains intact;
- recursive tow trains made from cell-visible vehicles, provided they stay within cell-exit movement and use co-located compatible hitch items;
- active mount/character-pulled carts, wagons, rickshaws, hand carts, and similar vehicle exteriors, including carts with downstream vehicle tow links, while the hitch is a live movement effect or an eligible NPC-only persistent hitch;
- player-facing exterior repair through ordinary repair kits, admin damage/hitch recovery, and tow catastrophe recovery for the current cell-exit movement model.
- explicit driver control handoff, player-facing readiness/status checks, required-crew enforcement, and fail-closed movement when an exterior projection is missing or out of sync.
- continuous RouteCell driving for powered and externally pulled vehicles, including exact stops, landmark/visible-exit targets, explicit compiled portal steps, durable checkpoints, and recursive hitch cohorts;
- persistent `RoomScale` hosted interiors that remain stable while their exterior moves through ordinary exits or along RouteCells, with transient ordinary-cell and scheduled-platform docking;
- revisioned, topology-pinned `VehicleRoute` definitions, recurring `VehicleService` schedules, restart-safe journeys, holds/cancellation/faults, player timetable/status output, and automatic or onboard operation;
- the scheduled multi-compartment train, ordinary-exit massive mobile platform, and mount-drawn route wagon-train acceptance scenarios in this runbook.

The current implementation should not yet be considered fully supported for coordinate 2D/3D movement, collision/dispatch/signalling, physical consist length, overtaking, automatic reverse-route generation, dynamic rerouting, fares/reservations, rich ownership or lease policy, rich parked-harness administration, or detailed fuel/power network topology beyond installed module candidates. Aircraft, spacecraft, free-moving ships, and coordinate-positioned elevators therefore remain outside this boundary; a large vehicle whose hosted interior moves by the supported cell-exit or RouteCell models is inside it.

This list is the MudSharp 2.0 Vehicle V1 boundary. The train, massive-platform, and recursive wagon-train workflows above were executed through the live telnet runtime on the migrated development world; the wagon evidence includes multiple durable checkpoints, reboot reload, explicit RouteCell portal traversal in both directions, and completion of the terminal route steps into the far-town platform. The maintained blank-database snapshot includes the final Vehicle V1 migration. Exact transcripts, automated totals, and fixture provenance are recorded in the [V1.0 acceptance evidence](./Verification/RouteCell_Vehicle_V1_Acceptance_Evidence.md). An upgraded production-world soak remains a separate release-candidate gate.
