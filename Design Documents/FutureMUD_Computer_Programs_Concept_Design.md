# FutureMUD Computer Program Design

This is a concept design document to outline a system in FutureMUD for in-world computer programs, to allow builders and players to make custom computer logic that runs on computer and microcomputer systems. The overall system is still incomplete, but the current shipped slice now includes signal automation, player-facing `electrical` and `programming` verbs, a private character-owned computer workspace, and persisted computer-program execution with `sleep`, terminal-session `UserInput()`, and execution-host `WaitSignal()` waits.

## Current Implementation Status

The first implementation slice for this design has now landed. The currently implemented foundation is:

- explicit compilation contexts for `StandardFutureProg`, `ComputerFunction`, and `ComputerProgram`
- compiler-time restriction of computer-safe variable types
- compiler-time restriction of computer-safe statements via statement registration metadata
- compiler-time blocking of user-defined `@SomeProg(...)` FutureProg calls inside computer compilation contexts
- shared computer and signal interfaces in `FutureMUDLibrary/Computers`
- item-facing signal component contracts in `FutureMUDLibrary/GameItems/Interfaces`
- core runtime scaffolding for computer executables, programs, files, hosts, processes, and built-in applications in `MudSharpCore/Computers`
- generic computer-artifact ownership support for:
  - private character workspaces
  - `ComputerHost` items
  - `ComputerStorage` items mounted into a host
- dedicated persistence models and EF tables for character-owned computer executables, executable parameters, and computer-program processes
- a private character-owned workspace for computer functions and computer programs, exposed through `ICharacterComputerWorkspace`, `IComputerExecutionService`, and `IComputerHelpService`
- the first real computer item component families in `MudSharpCore/GameItems`:
  - `ComputerHost`
  - `ComputerTerminal`
  - `ComputerStorage`
  - `NetworkAdapter`
- a resumable computer-program executor with persisted frame and local state, supporting scheduler-driven `sleep` wake-up, terminal-session `UserInput()` suspension and resume, and execution-host `WaitSignal()` suspension and resume
- local computer runtime functions for:
  - `ReadFile`
  - `WriteFile`
  - `AppendFile`
  - `FileExists`
  - `GetFiles`
  - `WriteTerminal`
  - `ClearTerminal`
  - `UserInput`
  - `WaitSignal`
  - `LaunchProgram`
  - `KillProgram`
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
  - `FileSignalGenerator`
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
  - live inspection and file editing of file-backed automation signal generators through `programming item <item> file ...`
- `programming` now also has a terminal-session bridge into real in-world computers:
  - `programming terminal connect <terminal>`
  - `programming terminal disconnect`
  - `programming terminal status`
  - `programming terminal owner host`
  - `programming terminal owner <storage>`
- `programming` now also exposes built-in host applications through that same connected terminal session:
  - `programming apps`
  - `programming app <name>`
- administrator characters now also have host-scoped network-service configuration surfaces through an active terminal session:
  - `programming mail`
  - `programming mail service on|off`
  - `programming mail domain add|remove|enable|disable <domain>`
  - `programming mail account add <user@domain> <password>`
  - `programming mail account enable|disable <user@domain>`
  - `programming mail account password <user@domain> <password>`
  - `programming ftp`
  - `programming ftp service on|off`
  - `programming ftp account add <user> <password>`
  - `programming ftp account enable|disable <user>`
  - `programming ftp account password <user> <password>`
  - `programming ftp file list`
  - `programming ftp file publish|unpublish <file>`
- `type` is now the player-facing terminal-input verb:
  - `type <text>` uses the current computer terminal session, or auto-selects a nearby terminal if one can be resolved cleanly
  - `type <terminal> <text>` explicitly targets a nearby terminal
  - when no session exists, `type` auto-connects to the resolved terminal before submitting input
  - when a foreground program on that session is suspended in `UserInput()`, `type` resumes it immediately with the supplied text
- once connected to a powered `ComputerTerminal`, workspace-style authoring commands operate on either the connected `ComputerHost` or one selected mounted `ComputerStorage`
- real host-backed or storage-backed execution now requires a powered execution host, while the private workspace remains power-free
- built-in applications are now exposed by the connected powered `ComputerHost`, not by the private workspace or mounted storage
- `programming apps` lists the built-in applications currently available on that host, even if the selected programming owner is a mounted storage device
- `programming app <name>` executes the named built-in application on that host and uses the current terminal session for its player-facing output
- `ComputerHost` powers local executable runtime, owns built-in application exposure, and autoruns host-backed programs marked `AutorunOnBoot` when power comes online
- `programming help` mirrors the `prog help` categories but filters to the computer-safe subset of types, statements, functions, and collection helpers
- `programming processes` now distinguishes timed `Sleep` waits from terminal-bound `UserInput` waits and signal-bound `WaitSignal` waits so interactive and event-driven programs can be debugged in game
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
- the shared runtime lifecycle is now explicit for powered and signal-capable items: `FinaliseLoad()` restores structural state, while `Login()` is where power drawdown, signal subscriptions, polling, timers, and retry heartbeats begin
- the world boot login pass now logs in world-root items only, while inventory-rooted items remain dormant until their owning character or body logs in; extracted mounted modules still activate because their `AutomationMountHost` forwards the item lifecycle to them
- powered-machine-based automation components no longer begin drawdown merely because they load switched on; they wait for `Login()` before attempting live power use

The remaining work is still substantial. In particular, waits beyond `sleep`, `UserInput()`, and the current v1 `WaitSignal()` implementation, richer multi-port inter-item signal graphs, broader built-in application coverage beyond the currently shipped `SysMon`, `FileManager`, `Directory`, `Mail`, and `FTP`, remote execution semantics beyond local host launch/kill, and broader network services beyond the first shipped `Mail` and `FTP` services are still future phases.

## Core Concepts

- The Program system will be shared between computers, microcomputers, controllers and other small bits of in-world compute type tasks
- The system will use FutureProg syntax and most of the existing compiler front-end, but computer code is no longer modelled as ordinary global `IFutureProg` instances
- Shared abstractions are defined in `FutureMUDLibrary/Computers` around `IComputerExecutable`, `IComputerFunction`, `IComputerProgramDefinition`, `IComputerProcess`, `IComputerHost`, `IComputerFileSystem`, `IComputerFile`, `ISignalSource`, and `ISignalSink`
- The mutable file-owner surface is now broader than executable ownership: `IComputerFileOwner` lets item components expose a file system to `FileManager`, `FTP`, and related tools without also becoming full executable owners
- There are ComputerPrograms, which are executed on in-game computers and run some code until they exit or terminate
- Some ComputerPrograms are hard-coded by FutureMUD and they handle complex systems, like mail or chat or whatnot
- Other ComputerPrograms are soft-coded by users and these are CustomComputerPrograms
- There are ComputerFunctions, which can be recycled and called by CustomComputerPrograms as well as power things like Microcontrollers
- These are designed to be stored as text on other data structures like items, computers and the like rather than sitting in a central repository like conventional IFutureProgs
- They are still compiled before being executed
- IComputerPrograms don't always run to completion. They often have wait points like sleeping, waiting for terminal user input, or waiting for signal input. This is handled through custom computer-only functions and statement types.
- Variable references continue to use the normal `@variable` syntax when read in expressions
- In the current shipped phase there are now real `ComputerHost`, `ComputerTerminal`, `ComputerStorage`, and `NetworkAdapter` item components
- Standalone functions and programs can still live in a private character-owned workspace, but the same runtime now also supports host-owned and storage-owned executables
- Workspace artifacts still persist in dedicated tables keyed to the owning character, and suspended workspace program processes persist locals, frame state, wake time, result, and last error separately from item revision data
- Host-backed and storage-backed executables currently persist in item component XML as part of the owning item runtime, not yet in separate generic database tables
- Network mail is the first shipped database-backed computer service: mail domains, accounts, messages, and mailbox entries now persist in dedicated EF tables rather than in item XML
- Remote file-transfer access is now the first shipped XML-backed network file service: FTP account state and per-file public visibility persist with the owning host or storage runtime, while anonymous public access and authenticated remote file manipulation are resolved at runtime through the telecom-backed computer network layer

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

There are a few statements and wait behaviours that will be only available for Custom Computer Programs and not regular progs. In the currently shipped phase this list contains:

- Sleep (a way of waiting a period of real time, yielding control back, and resuming through a persisted process record)
- `UserInput()` wait behaviour (a way of yielding control until a line of text is typed into the active terminal session and then resuming with that text)
- `WaitSignal()` wait behaviour (a way of yielding control until a named signal source on the real execution host item emits a non-zero signal and then resuming with that numeric value)

The currently shipped `WaitSignal()` implementation is intentionally narrow in scope:

- it is only available in the `ComputerProgram` compilation context
- it currently resolves only named signal source components that exist on the real execution host item
- it resumes when that source emits a non-zero signal and returns the numeric signal value
- broader graph-wide or remote signal waiting is still a future phase

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

There are now some functions that are only available for Custom Computer Programs and not regular progs.

Currently shipped local computer functions include:

- `ReadFile`
- `WriteFile`
- `AppendFile`
- `FileExists`
- `GetFiles`
- `WriteTerminal`
- `ClearTerminal`
- `UserInput`
- `WaitSignal`
- `LaunchProgram`
- `KillProgram`

Future non-exhaustive functions still planned include:

- CollectionToFile / DictionaryToFile / CollectionDictionaryToFile: write the contents of a collection, dictionary or collectiondictionary to a file in a way that can be roundtripped
- CollectionFromFile / DictionaryFromFile / CollectionDictionaryFromFile: read the contents of a file back out into a collection type
- SendSignal - sends a signal via a signal channel with optional duration, as a once off instruction
- PulseSignal - sends a signal via a signal channel every 5 seconds for a duration (but doesn't count as interrupted in between)
- soft-coded remote file helpers such as `WriteFileRemote` / `AppendFileRemote` / `FileExistsRemote` / `GetFilesRemote` - the currently shipped remote file access is exposed through the `FTP` and `FileManager` built-in applications rather than directly to custom computer programs
- LaunchProgram / KillProgram - to launch or kill other computer programs

### Hard-Coded Applications

The baseline built-in application list for the computer subsystem is now fixed as:

- `Mail` - asynchronous email client plus store-and-forward mail service
- `FTP` - remote file-transfer client plus host file-transfer service
- `Boards` - bulletin board and newsreader client plus board service
- `Messenger` - live pager-style messaging client plus relay service
- `FileManager` - local file browser, copy utility, and mounted-storage manager
- `Directory` - address book and service discovery utility
- `SysMon` - diagnostics, process manager, storage monitor, signal inspector, and fault log viewer

In the current shipped phase:

- built-in applications are represented as host-bound built-in program definitions rather than a disconnected catalog
- they execute through the shared computer execution service as real host processes, but use dedicated built-in executors internally
- they are exposed to players through `programming apps` and `programming app <name>` while connected to a powered terminal session
- `SysMon`, `FileManager`, `Directory`, `Mail`, and `FTP` currently have implemented runtime behaviour
- `SysMon` is a terminal-session diagnostics tool that reports host power and storage state, connected storage and terminal devices, network adapters, running processes, and locally accessible automation signal sources and sinks on the execution host item
- `FileManager` is a terminal-session interactive file utility that suspends in `UserInput()` between commands and currently supports listing, reading, editing, writing, appending, deleting, copying, retargeting, and directly inspecting or importing anonymously accessible public files from reachable remote hosts
- `Directory` is a terminal-session interactive discovery utility that suspends in `UserInput()` between commands and now supports both local host inspection and telecom-backed reachable-host discovery through `hosts`, `show <host>`, and `services <host>`
- `Mail` is now the first implemented network-capable built-in application: it runs as an interactive terminal client, authenticates against reachable mail domains, manages inbox and sent mail, and uses the ordinary editor flow to compose message bodies
- `FTP` is now the second implemented network-capable built-in application: it runs as an interactive terminal client, opens a session to a reachable remote host advertising FTP, allows anonymous access to published public files, and allows authenticated full file manipulation across the target host and mounted storage devices
- `NetworkAdapter` is no longer just a local readiness marker; it is now a telecom-grid-backed endpoint that restores its attached `ITelecommunicationsGrid`, joins and leaves that grid through runtime lifecycle, and publishes a canonical network address
- canonical adapter addresses use `PreferredNetworkAddress` when it is unique within the reachable linked-grid cluster; otherwise they fall back to a stable generated address of the form `adapter-<itemid>`
- reachable host discovery is transitive across the linked telecom-grid graph, uses cycle-safe breadth-first traversal, and only includes hosts whose adapters are currently network-ready
- remote service advertisement is intentionally conservative in the current slice: only built-in applications marked as network services and actually implemented are listed
- `Mail` is now the first shipped advertised network service, but it is only advertised when the target host has its mail service enabled and at least one enabled hosted domain
- `FTP` is now the second shipped advertised network service, but it is only advertised when the target host has its FTP service enabled; the advertised details report the count of anonymously readable public files currently exposed by that host and its mounted storage
- `Boards` and `Messenger` remain reserved built-in application identities for later phases

## Systems Needed

- Broader generic runtime persistence and save/load support for ComputerPrograms, ComputerFunctions, Files, SuspendedProcesses, and signal wiring beyond the current split of database-backed workspaces plus XML-backed host and storage owners
- A way of marking statements and functions as being available for FutureProgs vs ComputerFunctions. This is now partially implemented via compile contexts and statement registration metadata, but still needs fuller function-by-function rollout
- Further extension of the dedicated resumable program executor for future wait kinds beyond the currently shipped `Sleep`, `UserInput()`, and the current v1 `WaitSignal()` behaviour
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
- `FileSignalGenerator` is a powered file-backed input that owns a small local file system, parses one designated text file into a numeric signal, and updates its live output whenever that file changes
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
- Computer networking now uses the existing telecommunications grid family as its transport substrate rather than introducing a separate internet-grid runtime. Network adapters attach to `ITelecommunicationsGrid`, and computer-host discovery walks the linked-grid graph for reachable network-ready endpoints.
- Implemented in the current first player-facing slice:
  - a `programming` command verb that now lets players both:
    - manage a private workspace of computer functions and computer programs (`list`, `new`, `edit`, `set`, `parameter`, `compile`, `execute`, `processes`, `kill`, `help`)
    - inspect microcontrollers, replace logic, and add or remove local input bindings on live items through `programming item <item>` or the existing item-first short form
  - an `electrical` command verb that lets players inspect configurable sinks, rebind them to local signal sources, clear bindings, retune thresholds or activation mode, install or remove separate mountable modules, and route or unroute cable segments one room at a time, including targeting a dedicated automation housing or junction for concealed cable placement
  - file-backed signal generators expose a focused live editing path through `programming item <item> file [<component>]`, `file edit`, `file write`, and `file public on|off`
- both verbs currently use multistep delayed actions, inventory plans for tools, configurable static-string echoes, and skill checks without consuming materials
- administrator characters execute those live item actions instantly and do not require tools or checks
  - the same `programming` surface now also has a terminal-first path for real computers, where players connect to a powered `ComputerTerminal` and select either the connected host or a mounted storage device as the current programming owner
  - `programming apps` and `programming app <name>` now expose host-backed built-in applications through that same connected terminal session
  - the `type` verb is the terminal-facing input surface for real computers and now resumes foreground programs waiting on `UserInput()` for the active terminal session
  - workspace authoring still uses immediate ownership-checked edits rather than tool-gated physical actions
- real host-backed or storage-backed program execution currently supports the shipped local file and terminal functions listed above plus completion or persisted `sleep`, `UserInput()`, and v1 `WaitSignal()` suspension; remote file access now exists for players through the `FTP` and `FileManager` built-in applications, while soft-coded remote file functions and broader network execution remain future phases
- host-local file tooling is now file-owner-aware rather than only storage-aware, so component-owned file systems such as `FileSignalGenerator` can participate in `FileManager`, anonymous public-file browsing, and authenticated `FTP` mutation when they live on a reachable host item
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
