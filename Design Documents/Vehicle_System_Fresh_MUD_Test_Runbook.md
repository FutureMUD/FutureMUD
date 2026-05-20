# Vehicle System Fresh MUD Test Runbook

## Purpose

This runbook gives a staff-facing, in-game verification path for the current vehicle system on a fresh MUD. It assumes the stock seeded item component prototypes from [Seeded_Item_Components.json](./Seeded_Item_Components.json) are installed, but it does not assume any vehicle-specific item prototypes already exist.

The goal is to verify the supported vehicle shapes end to end:

- `ItemScale` vehicles with an exterior item, one driver slot, one control station, boarding, driving, and disembarking.
- `RoomContainer` vehicles with compartments, access points, cargo projections, install points, damage effects, and towing.
- `CellExit` movement through ordinary adjacent cell exits, using either `drive <direction>` or normal movement commands while controlling a vehicle.
- Active character/mount hitches where a person, animal, or mount pulls a vehicle tow point or leads another hitched character.

`RoomScale`, route movement, coordinate movement, player-facing vehicle repair gameplay, dynamic trailer breakage, persistent parked mount harness diagnostics, and richer fuel or power networks are not fully supported by this runbook.

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
```

The runbook uses:

- `Holdable` for crate, towbar, and installable module test items.
- `Container_Trunk` for the cargo projection item.
- `DragAid_Harness` or another `DragAid` component for yoke/harness hitch tests.

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
comp edit submit fresh vehicle runbook module component
comp review <component id>
accept edit fresh vehicle runbook module component
```

Record the approved component id as `<engine-installable-component-id>`.

Expected result:

- Both type-help commands show the same vehicle installable help, including `mount <type>` and `role <role>`.
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
| `<towbar-proto>` | physical hitch item | noun `towbar`, sdesc `a QA test towbar`, size `normal`, weight `10 kg` | `Holdable` |
| `<crate-proto>` | cargo contents | noun `crate`, sdesc `a QA test crate`, size `small`, weight `5 kg` | `Holdable` |
| `<cart-hull-proto>` | mount-pulled cart exterior item | noun `cart`, sdesc `a QA test hand cart`, size `normal`, weight `120 kg` | none |
| `<yoke-proto>` | optional mount hitch aid | noun `yoke`, sdesc `a QA test yoke`, size `normal`, weight `8 kg` | `Holdable`, `DragAid_Harness` or another `DragAid` component |

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

Verify access and module gating:

```text
vehicle show <vehicle id>
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

- Boarding is blocked while the required access point is closed.
- Opening `hatch@car` allows boarding.
- Movement is blocked until the required `engine` role is installed.
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

Verify damage linkage and admin repair:

```text
vehicle show <vehicle id>
wound car none slashing 60
vehicle show <vehicle id>
open hatch@car
embark car driver via hatch
vehicle repair <vehicle id> damage all
vehicle show <vehicle id>
open hatch@car
embark car driver via hatch
close hatch@car
drive north
```

Expected result:

- Exterior item damage creates vehicle-zone wounds.
- `vehicle show` reports damage-zone status and effective damage disables.
- Access, cargo, installation, tow, and movement profile effects are blocked while the damage effect is active.
- `vehicle repair <vehicle> damage all` clears the damage-derived disables without clearing any manually disabled flags.

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

- Hitching requires the hitch item to be held.
- The hitch item is dropped into the tow train's location and persisted on the tow link.
- Movement moves the full recursive tow train and occupants together.
- If the hitch item is removed from the train location, destroyed, or deleted, movement should block and `vehicle show` should report an invalid tow-link cause.
- `unhitch <vehicle>` removes all links involving that vehicle, while `unhitch <towpoint>@<vehicle>` removes only links using that point.

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
vehicleproto set tow add none hand towed 300 pull 4 shafts
vehicleproto set tow add none yoke towed 600 pull 6 yoke
vehicleproto set damage add 50 1 30 50 false frame
vehicleproto submit fresh vehicle runbook mount cart
vehicleproto approve <vehicle proto id> fresh vehicle runbook mount cart
vehicleproto create <vehicle proto id>
vehicle show <cart vehicle id>
```

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

- `hitch horse yoke@cart` is rejected because `yoke` tow points require a drag-aid item.
- `hitch horse yoke@cart with yoke` succeeds if the yoke item has an approved `DragAid` component and is visible/available to the actor.
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
- Movement still uses normal character and mount movement validation. Combat, blocking position changes, closed required vehicle access, damage-disabled tow points, or insufficient drag capacity should block the hitch or movement.

Current limitations:

- Active character/mount hitches are not persisted reboot-safe tow-link records and do not appear as durable tow links in `vehicle show`.
- A character/mount hitch cannot currently pull a vehicle that already has persisted vehicle-to-vehicle tow links.
- The hitch item is used as a normal `IDragAid` reference for the active drag effect. It is not consumed or converted into a persistent tow-link record.

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

Expected result:

- Generic item loading of a vehicle exterior shell is blocked.
- Driving while not controlling a vehicle is blocked; ordinary movement commands fall back to normal walking when you are not controlling a vehicle.
- Driving with required access points open is blocked.
- Damage-linked systems are reported as disabled in `vehicle show`; in this runbook's car prototype, the body damage zone disables access, cargo, install, tow, and movement together.
- Admin damage repair restores damage-derived access and movement if the only blocking cause was damage-derived disablement.
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
- `RoomContainer` vehicles represented by one exterior item, with authored compartments, slots, stations, access projections, cargo projections, installation points, damage effects, tow points, and cell-exit movement;
- recursive tow trains made from cell-visible vehicles, provided they stay within cell-exit movement and use simple co-located hitch items;
- active mount/character-pulled carts, wagons, rickshaws, hand carts, and similar vehicle exteriors while the hitch is a live movement effect and not expected to persist through reboot.

The current implementation should not yet be considered fully supported for:

- route-based buses, trains, or ferries;
- coordinate-positioned vehicles;
- aircraft, spacecraft, elevators, or ships with large moving interiors;
- vehicles that need reboot-persistent mount harness records, rich player repair gameplay, dynamic crash/catastrophe handling, or detailed fuel/power topology.
