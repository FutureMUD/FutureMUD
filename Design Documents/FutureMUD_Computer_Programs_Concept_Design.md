# FutureMUD Computer Program Design

This is a concept design document to outline a system in FutureMUD for in-world computer programs, to allow builders and players to make custom computer logic that runs on computer and microcomputer systems. This system is not yet implemented.

## Current Implementation Status

The first implementation slice for this design has now landed. The currently implemented foundation is:

- explicit compilation contexts for `StandardFutureProg`, `ComputerFunction`, and `ComputerProgram`
- compiler-time restriction of computer-safe variable types
- compiler-time restriction of computer-safe statements via statement registration metadata
- compiler-time blocking of user-defined `@SomeProg(...)` FutureProg calls inside computer compilation contexts
- shared computer and signal interfaces in `FutureMUDLibrary/Computers`
- item-facing signal component contracts in `FutureMUDLibrary/GameItems/Interfaces`
- core runtime scaffolding for computer executables, programs, files, hosts, processes, and built-in applications in `MudSharpCore/Computers`
- a first usable signal-automation slice in `MudSharpCore/GameItems`:
  - `PushButton`
  - `ToggleSwitch`
  - `MotionSensor`
  - `TimerSensor`
  - `Microcontroller`
  - `SignalLight`
  - `ElectronicDoor`
  - `ElectronicLock`
  - `AlarmSiren`

The current signal-automation slice is intentionally local to a single parent item. Wiring is authored with sibling component prototype names or ids, but runtime bindings are stored and resolved through stable local source identifiers plus explicit local endpoint keys. In the current shipped slice, all built-in local signal sources expose a single default endpoint key of `signal`. Inter-item wiring, reusable wire objects, and persisted signal-graph topologies are still future phases.

The remaining work is still substantial. In particular, persistence tables, resumable runtime execution, terminal sessions, inter-item signal wiring, and data networking are still future phases.

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

The traditional variable register would not be available to Custom Computer Programs and ComputerFunctions. 

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

There are a few statements that will be only available for Custom Computer Programs and not regular progs. A non-exhaustive list would include:

- Sleep (a way of waiting a period of time and yielding control back)

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

There would be some functions that would only be available for Custom Computer Programs and not regular progs. A non-exhaustive list would include:

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
  - Inputs: `PushButton`, `ToggleSwitch`, `MotionSensor`, `LightSensor`, `TimerSensor`, `Keypad`
  - Logic: `Microcontroller`
  - Outputs: `ElectronicDoor`, `ElectronicLock`, `SignalLight`, `RelaySwitch`, `AlarmSiren`
  - Host systems: `ComputerHost`, `ComputerTerminal`, `ComputerStorage`, `NetworkAdapter`
- Implemented in the first slice:
  - `PushButton` is a selectable momentary input that emits a numeric signal for an authored duration
  - `ToggleSwitch` is a persistent on/off numeric input using the normal switchable-item command flow
  - `MotionSensor` is a witnessed-movement input that emits a numeric signal for an authored duration when same-location movement matches its configured mode and minimum size
  - `TimerSensor` is a recurring same-item input that alternates between authored active and inactive numeric phases from a persisted cycle anchor
  - `Microcontroller` is a powered machine component whose inputs are sibling signal sources resolved through stable local source identifiers plus endpoint keys and whose inline logic compiles as a `ComputerFunction`
  - `SignalLight` is a signal-driven light source that wraps the existing programmable-light behaviour
  - `ElectronicDoor` is a standalone signal-driven door component built on the shared internal door runtime base, uses threshold logic, and retries opening while the signal remains active
  - `ElectronicLock` is a signal-driven lock that wraps the existing programmable-lock behaviour
  - `AlarmSiren` is a powered signal-driven audible sink that repeats a configured alarm emote and room audio echo while active
- An internet grid type including the equivalent tie-ins to the grid, cell towers etc. Possibly consider extending the internet grid as a special type of telecommunications grid so the same grid can do both.
- A programming check and command verb that allows players to write programs
- An electrical check and command verb that allows players to install systems, microcontrollers, wire signals together etc. This verb should be able to be used unskilled but carry the risk of electrocution.
- Both of these command verbs should be able to be surpressed so that they don't appear on non-modern MUDs.

## Unsolved Design Questions

- Signals will likely be initiated by events (player input, movement, recurring timers, and actual Events from the Event System). They will also typically have a fixed duration that they're sent and/or pulsed for. We want to make it so that ideally if nothing changes we don't have to constantly reassess the logic and the signals so there needs to be consideration to that in how things are implemented. The pulsing is mostly to give microcontrollers the opportunity to reassess their logic - if their output stays the same pulse to pulse the signal should be considered continuous.
- The current first slice only supports sibling-component wiring on the same item. Those local bindings now target stable component-prototype ids plus explicit local endpoint keys rather than transient component names. Future phases still need to decide how explicit electrical wiring, cross-item routing, install locations, richer multi-port authoring, and builder-facing wiring verbs should persist and present themselves.

## Conceptual Example - Motion Activated Door

This remains a target design example, not current shipped behaviour. The current implementation only supports same-item sibling signal components, so a motion sensor installed elsewhere and connected across items is still future work.

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
