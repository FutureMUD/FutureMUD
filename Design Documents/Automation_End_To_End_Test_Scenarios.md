# FutureMUD Automation End-To-End Test Scenarios

## Scope
This document captures the current manual end-to-end validation passes for the shipped electronics, programming, and signal-automation slice.

These are developer test scripts, not player documentation. They are intended for use on a dev game instance while validating:
- component authoring
- item composition
- live `electrical` and `programming` workflows
- mounted microcontrollers
- automation housings
- one-room-at-a-time cable routing

## Assumptions
- Use the normal `comp edit submit`, review, and approval workflow for components.
- Use the normal `item edit`, `item set add`, and item approval workflow for items.
- Reuse your game's existing ordinary container/openable/lockable support components where this document says to do so.
- Powered machines must still be powered by the game's normal live power infrastructure.
- `MotionSensor`, `TimerSensor`, and `Microcontroller` are powered machines in the current slice.
- `MotionSensor`, `TimerSensor`, and `Microcontroller` can be authored to draw from an automation host's parent-item power source when mounted by setting `comp set mountpower`.
- If `mountpower` is off, powered machines look for an `IProducePower` source on their own parent item.
- `AutomationHousing` is itself the concealment and service-access container component. It should be paired with the normal openable and lockable behaviour on the same item, not treated as a passive marker.

## Scenario 1: Electronic Door With Motion Sensors On Both Sides

### Goal
Validate a door that opens from either adjoining room when a motion sensor on that side fires and a mounted microcontroller passes the signal through.

### Component Prototypes

#### Inside motion sensor
```text
comp edit new motionsensor
comp set name Door Inside Motion Sensor
comp set wattage 25
comp set mountpower
comp set value 1
comp set duration 10
comp set size normal
comp set mode any
comp edit submit
```

#### Outside motion sensor
```text
comp edit new motionsensor
comp set name Door Outside Motion Sensor
comp set wattage 25
comp set mountpower
comp set value 1
comp set duration 10
comp set size normal
comp set mode any
comp edit submit
```

#### Microcontroller
```text
comp edit new microcontroller
comp set name Door Controller
comp set wattage 100
comp set mountpower
comp set input add inside "Door Inside Motion Sensor"
comp set input add outside "Door Outside Motion Sensor"
comp set logic
if (@inside > 0)
	return 1
end if
if (@outside > 0)
	return 1
end if
return 0
comp edit submit
```

#### Automation housing
```text
comp edit new automationhousing
comp set name Door Service Housing
comp set cables true
comp set modules true
comp set signalitems true
comp edit submit
```

#### Automation mount host
```text
comp edit new automationmounthost
comp set name Door Automation Host
comp set bay add controller Microcontroller
comp set access "Door Service Housing"
comp edit submit
```

#### Electronic door
```text
comp edit new electronicdoor
comp set name Security Door
comp set installed security door
comp set source "Door Controller"
comp set threshold 0.5
comp set openemote @ slide|slides open
comp set closeemote @ slide|slides closed
comp edit submit
```

#### Optional cable for the far-side room
```text
comp edit new signalcable
comp set name Outside Sensor Cable
comp edit submit
```

### Item Prototypes

Create the following item prototypes:

- `an electronic security door`
  - `ElectronicDoor`
  - `AutomationMountHost`
  - `AutomationHousing`
  - existing openable component
  - optional existing lockable component
- `an airlock controller module`
  - `Microcontroller`
- `an inside motion sensor`
  - `MotionSensor`
- `an outside motion sensor`
  - `MotionSensor`
- optional `an outside sensor cable`
  - `SignalCableSegment`

### Live Test Script

1. Install the door on an exit between Room A and Room B using the normal existing door workflow.
2. Load `an inside motion sensor` in Room A.
3. Load `an outside motion sensor` in Room B.
4. Load `an electronic security door` on the exit.
5. Load `an airlock controller module` near the door.
6. Open the service housing with the normal `open` verb.
7. Inspect the host and housing:
```text
electrical "an electronic security door"
```
8. Install the controller into the bay:
```text
electrical install "an electronic security door" "an airlock controller module" controller
```
9. Power the door host and mounted controller through the normal game power workflow.
10. Inspect the mounted controller:
```text
programming item "an electronic security door@an airlock controller module"
```
11. Replace the logic live:
```text
programming item "an electronic security door@an airlock controller module" logic
```
Paste:
```text
if (@inside > 0)
	return 1
end if
if (@outside > 0)
	return 1
end if
return 0
```
12. In Room A, bind the inside sensor:
```text
programming item "an electronic security door@an airlock controller module" input add inside "an inside motion sensor"
```
13. Move to Room B and try to bind the outside sensor directly:
```text
programming item "an electronic security door@an airlock controller module" input add outside "an outside motion sensor"
```
14. If step 13 is not valid because of locality, note that result and instead test the cable path:
```text
item load <outside-sensor-cable-proto>
electrical route "an outside sensor cable" "an outside motion sensor" <exit toward Room A>
```
15. Return to the door side and bind the cable instead:
```text
programming item "an electronic security door@an airlock controller module" input add outside "an outside sensor cable"
```
16. Final inspection:
```text
programming item "an electronic security door@an airlock controller module"
electrical "an electronic security door"
```
17. Close the housing:
```text
close "an electronic security door"
```
or the specific housing-bearing item, depending on your content setup.

### Expected Results
- Movement in Room A should trigger the inside sensor and open the door.
- Movement in Room B should trigger the outside sensor or cable-fed input and open the door.
- The door should close again after the active signal ends.
- If the door is locked against opening, it should stay shut rather than bypassing the lock.
- Reopening access after the blocking condition clears should allow the signal-driven open attempt to succeed.
- Closed housing access should block install, remove, rewiring, and microcontroller programming.

## Scenario 2: Multi-Room Automation Housing Chain With Push Button, Light, And Alarm

### Goal
Validate a concealed multi-room signal path where a push button in one room activates a light and a siren in a later room through two one-hop cable runs and automation housings.

### Component Prototypes

#### Push button
```text
comp edit new pushbutton
comp set name Panic Button
comp set keyword panic
comp set value 1
comp set duration 6
comp set emote @ slap|slaps $1
comp edit submit
```

#### Housing
```text
comp edit new automationhousing
comp set name Cable Housing
comp set cables true
comp set modules false
comp set signalitems true
comp edit submit
```

#### Cable
```text
comp edit new signalcable
comp set name Routed Signal Cable
comp edit submit
```

#### Signal light
```text
comp edit new signallight
comp set name Warning Light
comp set source "Panic Button"
comp set threshold 0.5
comp set onemote @ flare|flares into life
comp set offemote @ dim|dims back down
comp edit submit
```

#### Alarm siren
```text
comp edit new alarmsiren
comp set name Panic Siren
comp set wattage 80
comp set source "Panic Button"
comp set threshold 0.5
comp set volume veryloud
comp set emote @ blare|blares an urgent alarm
comp edit submit
```

### Item Prototypes

Create the following item prototypes:

- `a panic button`
  - `PushButton`
- `a junction box`
  - `AutomationHousing`
  - existing openable component
  - optional existing lockable component
- `a service panel`
  - `AutomationHousing`
  - existing openable component
  - optional existing lockable component
- `a corridor cable`
  - `SignalCableSegment`
- `a panel cable`
  - `SignalCableSegment`
- `a warning light`
  - `SignalLight`
- `an alarm siren`
  - `AlarmSiren`

### Live Test Script

Assume three rooms in a line: Room A -> Room B -> Room C.

1. In Room A, load `a panic button`.
2. In Room B, load `a junction box`.
3. In Room C, load `a service panel`, `a warning light`, and `an alarm siren`.
4. Load or carry `a corridor cable` and `a panel cable`.
5. In Room A, route the first hop into the junction box in Room B:
```text
electrical route "a corridor cable" "a panic button" <exit from A to B> "a junction box"
```
6. Move to Room B and open the junction box:
```text
open "a junction box"
electrical "a junction box"
```
7. Route the second hop from Room B into Room C:
```text
electrical route "a panel cable" "a corridor cable" <exit from B to C> "a service panel"
```
8. Move to Room C and open the service panel:
```text
open "a service panel"
electrical "a service panel"
```
9. Bind the light to the chained cable:
```text
electrical "a warning light" bind WarningLight "a panel cable"
```
10. Bind the siren to the chained cable:
```text
electrical "an alarm siren" bind PanicSiren "a panel cable"
```
11. Power the warning light and alarm siren through the normal game power workflow.
12. Close, and optionally lock, both housings:
```text
close "a service panel"
close "a junction box"
```
13. Return to Room A and trigger the button:
```text
select "a panic button" panic
```

### Expected Results
- The button signal should propagate A -> B -> C through the two cable segments.
- The warning light should activate.
- The alarm siren should sound for the authored duration.
- Closing the housings should not stop the already-wired system from working.
- Closed housings should block inspection and rewiring of concealed cable ends.
- Reopening a housing should restore maintenance access.

## Regression Notes
When running either scenario, pay special attention to:
- `comp set` forwarding on component protos that inherit from other protos with their own builder commands
- help output from `comp typehelp` and `comp set help` for the automation component family
- powered-machine behaviour when a sensor or controller is mounted versus loose
- access gating when an `AutomationHousing` is closed, locked, or broken open
- delayed-action cancellation, check failure, and abject failure shock output
