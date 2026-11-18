---
slug: automation-systems
category: Technology
summary: Build a motion-operated door, a closed-circuit camera with a viewing station, and street lights controlled by outdoor illumination.
---

# Automation systems

A sensor emits a number. A **sink**, such as an electronic door or signal light, watches a source and compares its value with a threshold. Use a **microcontroller** when a decision needs several inputs or custom logic. A camera carries pictures and optionally sound through **media ports**. Each example below specifies what to build, what to connect, and what a player should observe.

## Prepare components and live items

Start with two adjoining test rooms, a door-capable exit, and power for the sensors and media equipment. In these examples a powered fixture can carry a `GridPowerSupply` alongside its machine component. Assign that live fixture to the electrical grid, give its supply enough watts and switch the machine on. Use unique item keywords such as `sensor` or `camera` when targeting live items. [Electrical and telecommunications grids](Electrical_and_Telecom_Grid_Builder_Guide.md)

Each `comp edit new` block below describes a separate component prototype. Set its description with `comp set desc <description>`, submit it with `comp edit submit`, and have a reviewer run `comp review <id>` and `accept edit <comment>`. On each physical item prototype, `item set add <component-id>` adds a Current component. Submit and review the item, then `item load <item-prototype-id>`. Supply suitable descriptions, sizes and materials. [Item authoring](Item_Authoring_Guidelines.md)

Prototype `source` settings refer to a **component prototype**. Live `electrical` commands target an **item** by keyword. Use `2.sensor` to distinguish two items or `item@component` to select one component on a composite item. Inspect `electrical <item>` for its current value, resolved source, and any broken links. An Admin services items immediately. Ordinary characters can need tools, skill, time and access to an open service housing.

## A door that opens on approach

Build a sensor item with a `MotionSensor` and a power supply, and a door item with an `ElectronicDoor`. Use these settings on the two prototypes, approving the sensor first so the door can refer to it.

```text
comp edit new motionsensor
comp set name LobbyMotion
comp set mode any
comp set size normal
comp set duration 10
comp set value 1
```

```text
comp edit new electronicdoor
comp set name LobbyDoor
comp set installed lobby door
comp set source LobbyMotion
comp set threshold 0.5
```

Install the held door on an exit that accepts its size. The example uses the north exit. Put the powered sensor in the approach room, switch it on, and bind the installed door from that side.

```text
install door north
switch sensor on
electrical north bind LobbyDoor sensor
electrical north mode LobbyDoor above
electrical sensor
electrical north
```

Have an ordinary character of at least normal size walk into the approach room. The movement should produce a pulse of 1, opening the unlocked door. After ten seconds without another trigger, the signal returns to 0 and the door should close. Check locks, the sensor's size and mode, and whether the mover produces witnessed movement events. An administrator using immwalk can pass without triggering the sensor. To respond specifically when a character **opens or closes** a door manually, its item events are `ItemOpened` and `ItemClosed`. Both pass the character and item to a hook. [FutureProg for builders](FutureProg_Builder_Guide.md)

For approaches on both sides, give a powered `Microcontroller` two inputs, `inside` and `outside`, using `comp set input add <name> <source-component>`. In `comp set logic` enter the following program and finish the editor with `@` on its own line.

```text
if (@inside > 0)
    return 1
end if
if (@outside > 0)
    return 1
end if
return 0
```

Place the controller on the inside and bind the door to it. In the outside room, route a `SignalCable` instance using `electrical route cable sensor <exit-to-inside>`. From inside, use `programming item controller input add inside sensor` and `programming item controller input add outside cable`. Inspect both inputs, then walk each approach separately. A longer run takes a new cable at each hop. A mounted controller additionally needs a compatible bay, an open service housing for rewiring, and a working host power source if configured for mount power.

### A contact that indicates manual opening

If the desired event is the door leaf opening, install an event hook on an ordinary openable test door. A Senior Admin can wire this up on the loose door before installation. Use the keyword of that physical door for `door`.

```text
show event ItemOpened
prog edit newevent ItemOpened LobbyContactOpened
prog set text
```

Enter `send(@opener, "The door contact clicks open.")` and finish with `@`. Once the prog compiles, run these commands.

```text
hook create LobbyOpenContact LobbyContactOpened ItemOpened
hook install door LobbyOpenContact
```

For a closing indication, repeat with event `ItemClosed`, prog `LobbyContactClosed`, and body `send(@closer, "The door contact clicks shut.")`. Create and install a second hook for that prog and event. Install the door, then have a character open and close it by hand. Walking past a stationary door should produce neither indication. These hooks report character-operated changes to the operator. The motion circuit above responds to an approach even before anyone touches the door.

## Closed-circuit camera and viewing station

For a local video-only circuit, build one powered camera item and one powered monitor item. Place them together for installation. Give the camera one output port and the monitor opt-in viewing so that a character chooses when to watch.

```text
comp edit new camera
comp set name LobbyCamera
comp set capabilities video
comp set ports 1
comp set interval 5
```

```text
comp edit new mediamonitor
comp set name DeskMonitor
comp set capabilities video
comp set ambient
```

`ambient` toggles the setting. A new Media Monitor starts with ambient presentation enabled. Inspect the prototype and leave it set to opt-in. After loading and powering the two items, connect and test the circuit.

```text
connect camera monitor
switch camera on
switch monitor on
look monitor
watch feed monitor
```

Have a second character perform an ordinary visible action in the lit camera room. Inspect the latest frame on the monitor and observe the action while watching. Use `watch feed none` to stop watching. Disconnect the cable or switch off the camera, repeat the action, and check that **new** activity stops arriving. Frame refresh takes at least five seconds. Darkness below the camera's sensitivity, layer or plane differences, and hidden or OOC output affect what it can capture.

For a control room on another part of the map, connect the camera locally to a computer media interface, and the monitor locally to a second computer's media output. Use powered, reachable computer hosts with network adapters, connected terminals, and the Media network service already enabled on the publishing host. Check the host's advertised services in Directory first. The Media app's publish command registers a feed but does not enable the host service; a fresh host with no advertised Media service can still run the local wired circuit and recorder. In `programming app media`, use `type inputs` and `type outputs` to find the actual endpoint names. On the camera host use `type publish <input> as lobby private` and `type acl lobby add <user@domain>`. At the viewing station log in to Media as that authorised account and use `type subscribe <host-address>/lobby to <output>`. Verify reception, then verify that a viewer without the account grant cannot establish the same private subscription. [Computer and media workflows](../Technology/Media_System_Runtime_and_Workflows.md)

For both local viewing and recording from a single camera port, connect the camera to a `MediaSplitter` and use its outputs for the monitor and the computer media interface. With writable computer storage, run `type recordevent <input> as lobby for 5m` in Media. Produce a visible event, allow the quiet interval to elapse, and inspect `type jobs` and `type files`. Periodic scene refreshes alone do not start event recordings. Use `type stop <job-number>` to disarm the job.

## Street lights that come on after dark

Place a powered `LightSensor` in an outdoor reference room where the street lamps will have little effect on the measured illumination. Put the first lamp one exit east. The sensor reports ambient lux once per second. Read `electrical sensor` in daylight and after dark, then choose a threshold between those readings. The example uses 10 lux.

```text
comp edit new lightsensor
comp set name StreetDaylight
```

Build a cable using `comp edit new signalcable` and name the component `StreetLuxCable`. Approve it before configuring the light. On a fresh signal light:

```text
comp edit new signallight
comp set name StreetLamp
comp set source StreetLuxCable
comp set illumination 30
comp set threshold 10
comp set invert
```

Put those components on separate sensor, cable and lamp items. Load the lamp in the street and the loose cable at the sensor. Power the sensor and run:

```text
switch sensor on
electrical route cable sensor east
east
electrical lamp bind StreetLamp cable
electrical lamp threshold StreetLamp 10
electrical lamp mode StreetLamp below
electrical cable
electrical lamp
```

Below 10 lux the lamp should provide 30 lux. At 10 lux or higher it should extinguish. For a daytime wiring test, temporarily raise the live threshold above the current reading and then lower it below that reading. Restore 10 and check a natural dusk and dawn. Extend the street with one routed cable per additional exit, using the previous cable as the new source. Bind each lamp to its local segment.

Two failure modes matter here. If the lamp illuminates the sensor, it can repeatedly switch itself on and off. Move the sensor and compare its readings with the lamp forced both ways. If the sensor loses power or the binding breaks, a zero reading can also turn this below-threshold light on. `SignalLight` responds to a numeric value and supplies its configured illumination directly. Test the intended outage behaviour independently of the dusk test.
