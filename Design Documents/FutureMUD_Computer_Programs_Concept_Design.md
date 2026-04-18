# FutureMUD Computer Program Design

This is a concept design document to outline a system in FutureMUD for in-world computer programs, to allow builders and players to make custom computer logic that runs on computer and microcomputer systems. The overall system is still incomplete, but the current shipped slice now includes signal automation, player-facing `electrical` and `programming` verbs, a private character-owned computer workspace, and sleep-capable persisted computer-program execution.

## Current Implementation Status

The first implementation slice for this design has now landed. The currently implemented foundation is:

- explicit compilation contexts for `StandardFutureProg`, `ComputerFunction`, and `ComputerProgram`
- compiler-time restriction of computer-safe variable types
- compiler-time restriction of computer-safe statements via statement registration metadata
- compiler-time blocking of user-defined `@SomeProg(...)` FutureProg calls inside computer compilation contexts
- shared computer and signal interfaces in `FutureMUDLibrary/Computers`
- item-facing signal component contracts in `FutureMUDLibrary/GameItems/Interfaces`
- core runtime scaffolding for computer executables, programs, files, hosts, processes, and built-in applications in `MudSharpCore/Computers`
- dedicated persistence models and EF tables for character-owned computer executables, executable parameters, and computer-program processes
- a private character-owned workspace for computer functions and computer programs, exposed through `ICharacterComputerWorkspace`, `IComputerExecutionService`, and `IComputerHelpService`
- a resumable `sleep`-only computer-program executor with persisted frame/local state and scheduler-driven wake-up
- computer-safe help formatting that mirrors the FutureProg help sources while filtering to the allowed computer subset
- a first usable signal-automation slice in `MudSharpCore/GameItems`:
  - `PushButton`
  - `ToggleSwitch`
  - `MotionSensor`
  - `LightSensor`
  - `RainSensor`
  - `TemperatureSensor`
  - `TimerSensor`
  - `Keypad`
  - `Microcontroller`
  - `AutomationMountHost`
  - `AutomationHousing`
  - `SignalCableSegment`
  - `SignalLight`
  - `ElectronicDoor`
  - `ElectronicLock`
  - `RelaySwitch`
  - `AlarmSiren`

The current signal-automation slice now has two shipped wiring tiers. Local sink and microcontroller bindings are still authored with sibling component prototype names or ids and are stored/resolved through stable local source identifiers plus explicit local endpoint keys. In addition, `Microcontroller` can now exist as a separate mountable item installed into an `AutomationMountHost`, and `SignalCableSegment` is now a reusable one-hop wire item that mirrors a source endpoint across a specific adjacent-room exit. In the current shipped slice, all built-in local signal sources expose a single default endpoint key of `signal`, longer signal runs are built by chaining more cable segments one room at a time, and broader persisted signal-graph topologies are still future phases.

The first player-facing command surface for this slice has also now landed:

- `electrical` inspects and live-configures configurable signal sinks on an item, installs and removes mountable modules, and routes or unroutes one-hop signal cable segments
- `programming` now has two scopes:
  - a private character-owned workspace for creating, editing, compiling, executing, listing, and killing computer functions and computer programs
  - live inspection and programming of real microcontroller items, including mounted ones targeted through `host@module`
- `programming help` mirrors the `prog help` categories but filters to the computer-safe subset of types, statements, functions, and collection helpers
- both verbs currently operate through multistage delayed actions rather than instant changes
- those delayed actions acquire tools through inventory plans, use configurable static-string echoes for begin/continue/success/failure output, and restore tools rather than permanently consuming them
- administrator characters bypass the tool, skill-check, and delayed-action requirements for live `electrical` and item-targeted `programming` work; those actions resolve immediately for them
- `electrical` inspection now surfaces the live automation chain rather than just authored bindings, including mounted-controller input mappings, cable mirror routes, nearby routed cable segments where relevant, current values, switch and power state where relevant, and resolved versus broken links
- `programming` uses `ProgrammingComponentCheck`
- `electrical` uses `InstallElectricalComponentCheck` for install/remove/routing work and `ConfigureElectricalComponentCheck` for rebinding and threshold/mode work
- failed checks still cost time because the delayed action runs to completion before the check resolves
- abject electrical failures trigger electrical shock damage and an electrical-shock emote, but still do not consume tools or materials
- dedicated `AutomationHousing` components now gate concealed automation modules and cable ends by being the actual lockable-container capability on the item, integrating directly with the normal container/openable/lockable and legal/crime handling path rather than relying on arbitrary generic container items
- mounted microcontrollers now restore their mount-host relationship lazily from saved host identity during load and login, so host-derived power and local signal access survive reboot/load ordering
- mounted microcontrollers also refresh their live input-source subscriptions during login and power cut-in, so late-resolved nearby sensors and cable sources seed current values correctly after reboot/load ordering
- mounted powered automation modules and other powered-machine-based automation components now treat power discovery as an ongoing topology-aware process: they retry for a longer post-login window when switched on but initially unpowered, and they also refresh power resolution when relevant parent or host connectivity changes reveal usable power later in the boot sequence
- `ElectronicDoor` likewise retries late signal-source reconnection after load/login so a controller that becomes spatially or structurally discoverable later in the reboot sequence can still drive the door without manual intervention
- witnessed-movement automation ignores movers with `IImmwalkEffect`, so administrator immwalk traversal does not trip motion sensors or movement-driven door logic during testing or live operations

The remaining work is still substantial. In particular, computer file systems, real computer host and terminal items, waits beyond `sleep`, richer multi-port inter-item signal graphs, remote execution, and data networking are still future phases.

## Core Concepts

- The Program system will be shared between computers, microcomputers, controllers and other small bits of in-world compute type tasks
- The system will use FutureProg syntax and most of the existing compiler front-end, but computer code is no longer modelled as ordinary global `IFutureProg` instances
- Shared abstractions are defined in `FutureMUDLibrary/Computers` around `IComputerExecutable`, `IComputerFunction`, `IComputerProgramDefinition`, `IComputerProcess`, `IComputerHost`, `IComputerFileSystem`, `IComputerFile`, `ISignalSource`, and `ISignalSink`
- There are ComputerPrograms, which are executed on in-game computers and run some code until they exit or terminate
- Some ComputerPrograms are hard-coded by FutureMUD and they handle complex systems, like mail or chat or whatnot
- Other ComputerPrograms are soft-coded by users and these are CustomComputerPrograms
- There are ComputerFunctions, which can be recycled and called by CustomComputerPrograms as well as power things like Microcontrollers
- These are designed to be stored as text on other data structures like items, computers and the like rather than sitting in a central repository like conventional IFutureProgs
- They are still compiled before being executed
- IComputerPrograms don't always run to completion. They often have wait points like waiting for a user input or a signal input. This will be handled through custom functions and statement types.
- Variable references continue to use the normal `@variable` syntax when read in expressions
- In the current shipped phase there is not yet a real `ComputerHost` item component, so standalone functions and programs live in a private character-owned workspace rather than on an in-world computer item
- Those workspace artifacts persist in dedicated tables keyed to the owning character, and suspended program processes persist locals, frame state, wake time, result, and last error separately from item revision data

### Available Types

Only the following list of types will be permitted in Custom Computer Programs:

- Boolean
- Number
- Text
- MudDateTime
- TimeSpan
- Typed Collections of the above scalar types
- Typed Dictionaries of the above scalar types
- Typed Collection Dictionaries of the above scalar types

### Variable Registers

The traditional variable register is not available to Custom Computer Programs and ComputerFunctions.

### File System

The Computer system would have a concept of files. Files are essentially all just text files with text name handles. They are the main way in which data is stored on these systems and are a way to have persistent memory as well.

### Restricted Statements

Computer compilation contexts work as an allow-list, not a deny-list. The currently enabled statement families are:

- variable declaration and assignment
- arithmetic assignment
- collection and dictionary mutation helpers
- `if`, `switch`, `while`, `for`, `foreach`
- `break`, `continue`, and `return`

Statements like `console`, `delay`, `delayprog`, `force`, `send`, and `setregister` remain standard-prog-only.

### Additional Statements

There are a few statements that will be only available for Custom Computer Programs and not regular progs. In the currently shipped phase this list contains:

- Sleep (a way of waiting a period of real time, yielding control back, and resuming through a persisted process record)

### Statement Handling

Statement availability is now handled at compiler-registration time rather than by flags on compiled statement instances. Each statement compiler registers the compilation contexts that it supports, and using a statement in the wrong context produces a compile-time error.

The three contexts are:

- `StandardFutureProg`
- `ComputerFunction`
- `ComputerProgram`

### Restricted Functions

Computer compilation contexts use a restricted built-in-function path. User-defined FutureProg calls are always blocked in computer contexts. Built-in functions are additionally filtered so that computer code only sees function signatures compatible with the restricted computer type set and the approved computer-safe function categories.

The intended safe families remain:

- Functions that work with timespans and muddatetimes
- Text manipulation functions, including ToText overrides for the available types
- Math functions for numbers
- Collection and Dictionary manipulation functions
- Universal helpers like IfNull, IsNull, Random etc

### New Functions

There would be some functions that would only be available for Custom Computer Programs and not regular progs. A future non-exhaustive list would include:

- CollectionToFile / DictionaryToFile / CollectionDictionaryToFile: write the contents of a collection, dictionary or collectiondictionary to a file in a way that can be roundtripped
- CollectionFromFile / DictionaryFromFile / CollectionDictionaryFromFile: read the contents of a file back out into a collection type
- ReadFile - reads the contents of a file
- WriteFile - overwrites the contents of a file
- AppendFile - appends to an existing file
- FileExists - tests to see if a file exists
- GetFiles - gets a collection of file names
- WriteTerminal - writes text to the computer terminal
- ClearTerminal - clears the computer terminal
- UserInput - only works in a program not a function, but waits for user input on the computer terminal
- WaitSignal - sleeps the program until a signal channel is triggered
- SendSignal - sends a signal via a signal channel with optional duration, as a once off instruction
- PulseSignal - sends a signal via a signal channel every 5 seconds for a duration (but doesn't count as interrupted in between)
- WriteFileRemote/AppendFileRemote/FileExistsRemote/GetFilesRemote - for interacting over an internet network delivered via an IGrid
- LaunchProgram / KillProgram - to launch or kill other computer programs

### Hard-Coded Applications

The baseline built-in application list for the computer subsystem is now fixed as:

- `Mail` - asynchronous email client plus store-and-forward mail service
- `Boards` - bulletin board and newsreader client plus board service
- `Messenger` - live pager-style messaging client plus relay service
- `FileManager` - local and remote file browser and copy utility
- `Directory` - address book and service discovery utility
- `SysMon` - diagnostics, process manager, storage monitor, signal inspector, and fault log viewer

## Systems Needed

- Concrete runtime persistence and save/load support for ComputerPrograms, ComputerFunctions, Files, SuspendedProcesses, and signal wiring
- A way of marking statements and functions as being available for FutureProgs vs ComputerFunctions. This is now partially implemented via compile contexts and statement registration metadata, but still needs fuller function-by-function rollout
- A dedicated resumable program executor for sleeps, user input waits, and signal waits
- A desktop computer item to run these programs on
- A microcontroller item type that has input channels and output channels that can be connected to other items, and can run ComputerFunctions on its channels to handle logic
- Signal-capable item component families. These should share `ISignalSource` / `ISignalSink` contracts but remain distinct concrete item component types:
  - Inputs: `PushButton`, `ToggleSwitch`, `MotionSensor`, `LightSensor`, `RainSensor`, `TemperatureSensor`, `TimerSensor`, `Keypad`
  - Logic: `Microcontroller`
  - Outputs: `ElectronicDoor`, `ElectronicLock`, `SignalLight`, `RelaySwitch`, `AlarmSiren`
  - Host systems: `ComputerHost`, `ComputerTerminal`, `ComputerStorage`, `NetworkAdapter`
- Implemented in the first slice:
  - `PushButton` is a selectable momentary input that emits a numeric signal for an authored duration
  - `ToggleSwitch` is a persistent on/off numeric input using the normal switchable-item command flow
  - `MotionSensor` is a powered witnessed-movement input that emits a numeric signal for an authored duration when same-location movement matches its configured mode and minimum size
  - `LightSensor` is a powered ambient measurement input that emits the current illumination level as its numeric signal
  - `RainSensor` is a powered weather measurement input that emits a numeric rain-intensity scale while climate-exposed and zero while sheltered
  - `TemperatureSensor` is a powered ambient measurement input that emits the current room temperature in Celsius as its numeric signal
  - `TimerSensor` is a powered recurring same-item input that alternates between authored active and inactive numeric phases from a persisted cycle anchor
  - `Keypad` is a powered selectable input that accepts numeric digit entry and emits a momentary signal only when the entered code matches its authored code
  - `Microcontroller` is a powered machine component whose inputs are local signal sources resolved through stable local source identifiers plus endpoint keys, whose inline logic compiles as a `ComputerFunction`, and which can now also be installed as a separate module item into an automation host bay
- powered machine automation modules and sensors can optionally be authored to draw power from an automation host's parent-item power source when mounted, including compatible attached or connected power-producing items on that host; otherwise they look for a power source on their own parent item
  - `AutomationMountHost` provides named bays for separate automation modules and can require a sibling `AutomationHousing` component before those bays are serviceable
- `AutomationHousing` is a dedicated housing or junction component family that is itself the service-access container capability, reusing the normal lockable/openable/container and legal-system path while exposing concealed automation items only when service access is open
- when an automation housing lives on a door or host item, service access is reached through the ordinary `open` / `close` verbs with a subtarget such as `open north panel` or `close <item> housing`
  - `SignalCableSegment` is a reusable one-hop adjacent-room wire item that persists its source binding, source and destination cells, and routed exit id
  - `SignalLight` is a signal-driven light source that wraps the existing programmable-light behaviour
  - `ElectronicDoor` is a standalone signal-driven door component built on the shared internal door runtime base, uses threshold logic, and retries opening while the signal remains active
  - `ElectronicLock` is a signal-driven lock that wraps the existing programmable-lock behaviour
  - `RelaySwitch` is a signal-driven relay-controlled power switch that wraps the programmable power-supply behaviour and only produces power while its relay is effectively closed
  - `AlarmSiren` is a powered signal-driven audible sink that repeats a configured alarm emote and room audio echo while active
- An internet grid type including the equivalent tie-ins to the grid, cell towers etc. Possibly consider extending the internet grid as a special type of telecommunications grid so the same grid can do both.
- Implemented in the current first player-facing slice:
  - a `programming` command verb that now lets players both:
    - manage a private workspace of computer functions and computer programs (`list`, `new`, `edit`, `set`, `parameter`, `compile`, `execute`, `processes`, `kill`, `help`)
    - inspect microcontrollers, replace logic, and add or remove local input bindings on live items through `programming item <item>` or the existing item-first short form
  - an `electrical` command verb that lets players inspect configurable sinks, rebind them to local signal sources, clear bindings, retune thresholds or activation mode, install or remove separate mountable modules, and route or unroute cable segments one room at a time, including targeting a dedicated automation housing or junction for concealed cable placement
- both verbs currently use multistep delayed actions, inventory plans for tools, configurable static-string echoes, and skill checks without consuming materials
- administrator characters execute those live item actions instantly and do not require tools or checks
  - workspace authoring currently uses immediate ownership-checked edits rather than tool-gated physical actions because there is not yet a real computer terminal/host item path
  - workspace program execution currently supports completion or persisted `sleep` suspension only; terminal IO, `UserInput`, `WaitSignal`, remote file access, and host-control functions are future phases
  - abject failures on electrical work can cause electrical shock damage
- Both of these command verbs should be able to be surpressed so that they don't appear on non-modern MUDs.

## Unsolved Design Questions

- Signals will likely be initiated by events (player input, movement, recurring timers, and actual Events from the Event System). They will also typically have a fixed duration that they're sent and/or pulsed for. We want to make it so that ideally if nothing changes we don't have to constantly reassess the logic and the signals so there needs to be consideration to that in how things are implemented. The pulsing is mostly to give microcontrollers the opportunity to reassess their logic - if their output stays the same pulse to pulse the signal should be considered continuous.
- The current first slice now supports sibling-component local wiring, separate mounted modules, dedicated automation housings or junctions, and one-hop adjacent-room cable segments. Future phases still need to decide how richer multi-port authoring, longer persisted signal graphs, and broader builder-facing wiring verbs should persist and present themselves.
- Mounted automation modules now inherit the host item's spatial context for `TrueLocations`, perception, and local signal targeting while installed in an automation bay.
- Mounted automation modules also now resolve room-local signal sources for live runtime subscriptions through that host spatial context, so mounted controllers behave like installed hardware rather than isolated loose items.
- ordinary item descriptions should present the physical state of housings, doors, locks, and bays; live signal/control diagnostics belong to `electrical` inspection rather than ordinary `look`

## Conceptual Example - Motion Activated Door

This remains a target design example, not exact current shipped behaviour. The current implementation now supports separate-item mounted microcontrollers and one-room cable runs, but richer named door channels and a motion sensor installed on a cell exit rather than an item are still future work.

- There is an item with an Electronic Door item component which uses a signal channel to decide whether it's open or closed. 
- Electronic Door is an IAutomationEnabled item which exposes signal channels and can have those channels connected to other items and have microcontrollers installed in those channels as well
- There is an item with a Microcontroller item component that can be programmed by players and installed in the Electronic Door
- There is an item with a MovementSensor item component which sends a signal for a set duration after detecting movement in the vicinity of the automatic door

The signal channel the door exposes is called "DoorOpen". When it receives any value other than zero for this signal it opens the door. When the signal stops it closes the door.

The Microcontroller is programmed with an input parameter of "Signal1" and returns number.

The player programs the Microcontroller with the following function:

```
// Short circuit exit if signal is low
if (@Signal1 == 0)
  return 0
end if

// Otherwise only open if during business hours
var time = Now()
if (IsBetween(@time, ToTime("07:00:00"), ToTime("17:00:00")))
  return 1
end if
return 0
```

The player installs the microcontroller on the door and maps its `DoorOpen` output to the `DoorOpen` input on the electronic door component.

They then install the Movement Sensor on the CellExit that the door is installed in and connect its "MotionDetected" signal to "Signal1" on the microcontroller on the door. They set the motion detection to MinimumSize=Normal and SignalDuration to 45 seconds.

A player approaches the door. The CharacterBeginMovement event fires. The sensor wasn't already sending a signal and they are size Normal so the motion sensor sends MotionDetected:1. The microcontroller receives this as Signal1, runs it through its logic and sees that it is 11am and therefore passes the signal on to the door, and the door opens for 45 seconds.
