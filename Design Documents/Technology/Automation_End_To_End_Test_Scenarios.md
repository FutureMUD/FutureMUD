# FutureMUD Automation And Computer End-To-End Test Scenarios

## Scope
This document captures the current manual end-to-end validation passes for the shipped electronics, programming, computer, and telecom-backed network slice.

These are developer test scripts, not player documentation. They are intended for use on a dev game instance while validating:
- component authoring
- item composition
- live `electrical` and `programming` workflows
- mounted microcontrollers
- automation housings
- one-room-at-a-time cable routing
- powered computer hosts, terminals, storage, and host-backed programs
- terminal-session built-in applications
- shared `user@domain` identities, VPN tunnelling, and network service discovery
- `Mail`, `Boards`, and `FTP`

Current behavioural notes:
- administrator characters perform `electrical` and item-targeted `programming` actions instantly without tool requirements, skill checks, or delayed stages
- service housings on door or host items can be accessed through normal `open` / `close` subtarget syntax, for example `open north panel`
- ordinary `look` output focuses on physical state; live signal diagnostics are expected in `electrical <item>`
- live source and component targeting should use normal parent-item keywords, with `item@component` only when the component itself must be named explicitly
- duplicate nearby items should be disambiguated with ordinary numeric item targeting such as `2.sensor`, not raw component ids
- `electrical <item>` should now be the primary debugging surface for automation chains, showing controller inputs, cable mirror routes, nearby routed cable segments, current values, machine state, and whether links are currently resolved or broken
- use an ordinary non-immwalk mover for motion-sensor testing; administrators with `IImmwalkEffect` no longer emit the witnessed movement events that motion sensors consume
- `Directory` should now be the primary debugging surface for reachable hosts, service advertisement, routes, gateways, and active VPN tunnels
- `SysMon` should now be the primary debugging surface for local host power, processes, adapters, hosted VPNs, and active session tunnel state

## Assumptions
- Use the normal `comp edit submit`, review, and approval workflow for components.
- Use the normal `item edit`, `item set add`, and item approval workflow for items.
- Reuse your game's existing ordinary container/openable/lockable support components where this document says to do so.
- Powered machines must still be powered by the game's normal live power infrastructure.
- `MotionSensor`, `TimerSensor`, and `Microcontroller` are powered machines in the current slice.
- `MotionSensor`, `TimerSensor`, and `Microcontroller` can be authored to draw from an automation host's parent-item power source when mounted by setting `comp set mountpower`. That host-side power can come from a producer component on the host item itself or from a compatible attached/connected power item.
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
6. Open the service housing with the normal `open` verb and panel subtarget.
7. Inspect the host and housing:
```text
open north panel
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
electrical "an electronic security door@an airlock controller module"
electrical "an outside motion sensor"
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
- After reboot, a switched-on mounted controller should recover host-derived power without needing a manual off/on cycle.
- `electrical "an outside motion sensor"` should make the routed outside cable visible enough to confirm what it mirrors and through which exit it is routed.

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

## Scenario 3: File-Backed Signal Generator With Local And Remote Editing

### Goal
Validate that a `FileSignalGenerator` can drive automation from a file, be edited locally through `programming item ... file ...`, and optionally be exposed through network file tooling.

### Component Prototypes

#### File-backed signal generator
```text
comp edit new filesignalgenerator
comp set name Signal File Generator
comp set wattage 25
comp set mountpower
comp set file signal.txt
comp set default 0
comp edit submit
```

#### Signal light sink
```text
comp edit new signallight
comp set name File Driven Light
comp set source "Signal File Generator"
comp set threshold 0.5
comp set onemote @ flare|flares into life
comp set offemote @ dim|dims back down
comp edit submit
```

### Item Prototypes

- `a file signal controller`
  - `FileSignalGenerator`
  - `SignalLight`
  - optional `ComputerHost`
  - optional `NetworkAdapter`

### Live Test Script

1. Load `a file signal controller`.
2. Power it through the normal game power workflow.
3. Inspect it:
```text
electrical "a file signal controller"
programming item "a file signal controller" file
```
4. Write zero to the file locally:
```text
programming item "a file signal controller" file write signal.txt 0
electrical "a file signal controller"
```
5. Confirm the light is off.
6. Write a non-zero value locally:
```text
programming item "a file signal controller" file write signal.txt 1
electrical "a file signal controller"
look "a file signal controller"
```
7. Confirm the light turns on and `electrical` reports the current signal value as non-zero.
8. Open the multiline editor path:
```text
programming item "a file signal controller" file edit signal.txt
```
9. In the editor, replace the contents with `0`, submit with `@`, and confirm the light turns back off.
10. If the item also has `ComputerHost` and `NetworkAdapter`, connect to its terminal from another reachable host and publish the signal file:
```text
programming ftp service on
programming ftp file publish signal.txt
```
11. From another reachable host, validate both remote access paths:
```text
programming app filemanager
type list public <host>
type show public <host> signal.txt
type copy public <host> signal.txt
programming app ftp
type open <host>
type list
type show signal.txt
```

### Expected Results
- The `FileSignalGenerator` should emit the parsed numeric value from `signal.txt`.
- The linked `SignalLight` should react as the file changes.
- Local `programming item ... file write` and `file edit` should update the live signal immediately.
- If the file is published, remote public-file inspection should show the same contents without exposing unrelated private files.

## Scenario 4: Powered Computer Host With Terminal Programs And Local Automation Diagnostics

### Goal
Validate a fully powered local computer host with terminal-session authoring, interactive built-in applications, `UserInput()`, and host-local `WaitSignal()`.

### Item Prototypes

- `a computer host`
  - `ComputerHost`
  - optional `NetworkAdapter`
- `a terminal console`
  - `ComputerTerminal`
- `a storage module`
  - `ComputerStorage`

### Live Test Script

1. Load `a computer host`, `a terminal console`, and `a storage module`.
2. Connect storage and terminal to the host through the normal item connectivity workflow.
3. Power the host and terminal.
4. Connect to the terminal:
```text
programming terminal connect "a terminal console"
programming terminal status
```
5. Switch programming ownership to the host and create a simple interactive program:
```text
programming terminal owner host
programming new program prompttest
programming set return text
programming set source
WriteTerminal("Type something:")
return UserInput()
programming compile prompttest
programming execute prompttest
programming processes
```
6. Confirm the process is sleeping on `UserInput`, then resume it:
```text
type hello world
programming processes
```
7. Create a signal wait program on a host that also has a local signal source:
```text
programming new program waittest
programming set return number
programming set source
return WaitSignal("Signal File Generator")
programming compile waittest
programming execute waittest
programming processes
```
8. Trigger the host-local source, for example by editing the `FileSignalGenerator` file from Scenario 3:
```text
programming item "a file signal controller" file write signal.txt 1
programming processes
```
9. Run the built-in apps:
```text
programming apps
programming app sysmon
programming app directory
type summary
type adapters
type routes
type exit
programming app filemanager
type owners
type list
type exit
```

### Expected Results
- `programming terminal status` should show the connected host, mounted storage, and any active tunnel state.
- `UserInput()` programs should suspend and resume from `type`.
- `WaitSignal()` should suspend and resume when the named host-local source emits a non-zero value.
- `SysMon` should report host power, processes, adapters, and local signal state.
- `Directory` and `FileManager` should both run as foreground terminal apps that stay open until `type exit`.

## Scenario 5: Network Identity, VPN, Mail, Boards, And FTP

### Goal
Validate the full 1.0 network-service stack: shared identities, VPN tunnel access, remote discovery, mail, boards, and FTP.

### Required Hosts

- `Public Workstation`
  - powered `ComputerHost`
  - `ComputerTerminal`
  - `NetworkAdapter`
- `Service Host`
  - powered `ComputerHost`
  - `NetworkAdapter`
  - reachable on the same public route
- optional `Private Field Host`
  - powered `ComputerHost`
  - `NetworkAdapter` or `WirelessModem`
  - reachable only through exchange-private or VPN scope

### Live Test Script

1. On `Service Host`, configure shared identity and at least one domain:
```text
programming network domain add example.net
programming network account add alice@example.net secret
programming network account add bob@example.net secret
```
2. If testing private reachability, also configure a hosted VPN:
```text
programming network vpn add fieldops
```
3. On `Service Host`, enable and configure mail:
```text
programming mail service on
programming mail domain add example.net
```
4. On `Service Host`, enable and configure boards:
```text
programming boards service on
programming boards add <board>
```
5. On `Service Host`, enable and configure FTP:
```text
programming ftp service on
programming ftp account add alice secret
programming ftp file list
programming ftp file publish <file>
```
6. From `Public Workstation`, validate discovery:
```text
programming app directory
type hosts
type show <service-host>
type services <service-host>
type gateways
```
7. If testing VPN/private hosts, open a tunnel:
```text
type tunnel connect <service-host> alice@example.net secret fieldops
type routes
type hosts
```
8. Validate mail:
```text
programming app mail
type login alice@example.net secret
type inbox
type send bob@example.net
type subject Test Message
type body
type post
type exit
programming app mail
type login bob@example.net secret
type inbox
type read <id>
type delete <id>
type exit
```
9. Validate boards:
```text
programming app boards
type hosts
type open <service-host>
type login alice@example.net secret
type boards
type use <board>
type list
type post Test Board Post
type list
type read <id>
type delete <id>
type exit
```
10. Validate FTP:
```text
programming app ftp
type open <service-host>
type list
type show <file>
type login alice secret
type owners
type use host
type put <local-file> uploaded.txt
type delete uploaded.txt
type exit
```

### Expected Results
- `Directory` should only show hosts reachable through current public, subnet, and tunnel routes.
- VPN tunnels should change visibility only for the active terminal session that opened them.
- `Mail`, `Boards`, and `FTP` should appear in `Directory services <host>` only when enabled and configured.
- `Mail` should authenticate via shared `user@domain` identities and deliver persisted messages between hosted accounts.
- `Boards` should authenticate via shared `user@domain` identities and allow reading plus creation or deletion of network-authored posts on exposed boards.
- `FTP` should permit anonymous public-file reads and authenticated remote file manipulation separately.

## Regression Notes
When running these scenarios, pay special attention to:
- `comp set` forwarding on component protos that inherit from other protos with their own builder commands
- help output from `comp typehelp` and `comp set help` for the automation component family
- powered-machine behaviour when a sensor or controller is mounted versus loose
- access gating when an `AutomationHousing` is closed, locked, or broken open
- delayed-action cancellation, check failure, and abject failure shock output
