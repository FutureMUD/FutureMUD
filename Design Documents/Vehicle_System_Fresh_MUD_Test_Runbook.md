# Vehicle System Fresh MUD Test Runbook

## Purpose

This runbook gives a staff-facing, in-game verification path for the current vehicle system on a fresh MUD. It assumes the stock seeded item component prototypes from [Seeded_Item_Components.json](./Seeded_Item_Components.json) are installed, but it does not assume any vehicle-specific item prototypes already exist.

The goal is to verify the supported vehicle shapes end to end:

- `ItemScale` vehicles with an exterior item, one driver slot, one control station, boarding, driving, and disembarking.
- `RoomContainer` vehicles with compartments, access points, cargo projections, install points, damage effects, towing, and operational readiness diagnostics.
- `CellExit` movement through ordinary adjacent cell exits, using either `drive <direction>` or normal movement commands while controlling a vehicle.
- Active character/mount hitches where a person, animal, or mount pulls a vehicle tow point, leads another hitched character, or pulls a vehicle that itself has downstream vehicle tow links.
- Operational readiness checks for access grants, module condition, fuel/power candidates, player repair, and tow-stress catastrophe recovery.

`RoomScale`, route movement, coordinate movement, rich ownership or lease policy, rich parked-harness administration, and broad fuel or power network topology are not fully supported by this runbook.

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
vehicle show <car vehicle id>
unhitch front@trailer
```

Expected result:

- Hard invalid states still fail before any catastrophe roll: missing gear, destroyed gear, missing endpoints, over-capacity links, incompatible tow points, cycles, duplicate incoming links, and blocked exits.
- A stressed but valid link may catastrophically fail before movement and before fuel or power is consumed.
- On catastrophe, the room receives a failure echo, the hitch item or linked exteriors take shearing damage, persistent links are disabled, transient hitch/drag effects are cleared, and reserved hitch items are released.
- `vehicle show` reports the disabled link and recovery hint.
- After physical repair, `vehicle repair <vehicle> hitch all` re-enables only links that pass unified graph validation; otherwise it leaves the link disabled with the validation reason.

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
- `RoomContainer` vehicles represented by one exterior item, with authored compartments, slots, stations, access projections, cargo projections, installation points, damage effects, tow points, operational readiness checks, and cell-exit movement;
- recursive tow trains made from cell-visible vehicles, provided they stay within cell-exit movement and use co-located compatible hitch items;
- active mount/character-pulled carts, wagons, rickshaws, hand carts, and similar vehicle exteriors, including carts with downstream vehicle tow links, while the hitch is a live movement effect or an eligible NPC-only persistent hitch;
- player-facing exterior repair through ordinary repair kits, admin damage/hitch recovery, and tow catastrophe recovery for the current cell-exit movement model.

The current implementation should not yet be considered fully supported for:

- route-based buses, trains, or ferries;
- coordinate-positioned vehicles;
- aircraft, spacecraft, elevators, or ships with large moving interiors;
- vehicles that need rich ownership or lease policy, rich parked-harness administration, or detailed fuel/power topology beyond installed module candidates.
