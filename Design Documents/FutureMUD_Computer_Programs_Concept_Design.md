# FutureMUD Computer Program Design

This is a concept design document to outline a system in FutureMUD for in-world computer programs, to allow builders and players to make custom computer logic that runs on computer and microcomputer systems. This system is not yet implemented.

## Core Concepts

- The Program system will be shared between computers, microcomputers, controllers and other small bits of in-world compute type tasks
- The system will use a cut-down version of the FutureProg system - in fact, these will be an implementation of IFutureProg, just with greatly reduced variable types and function restrictions
- IComputerProgram will be the interface which extends IFutureProg but signals that the type behaves like an IComputerProgram.
- There are ComputerPrograms, which are executed on in-game computers and run some code until they exit or terminate
- Some ComputerPrograms are hard-coded by FutureMUD and they handle complex systems, like mail or chat or whatnot
- Other ComputerPrograms are soft-coded by users and these are CustomComputerPrograms
- There are ComputerFunctions, which can be recycled and called by CustomComputerPrograms as well as power things like Microcontrollers
- These are designed to be stored as text on other data structures like items, computers and the like rather than sitting in a central repository like conventional IFutureProgs
- They are still compiled before being executed
- IComputerPrograms don't always run to completion. They often have wait points like waiting for a user input or a signal input. This will be handled through custom functions and statement types.

### Available Types

Only the following list of types will be permitted in Custom Computer Programs:

- Boolean
- Number
- Text
- MudDateTime
- TimeSpan
- Collections
- Dictionaries
- Collection Dictionaries

### Variable Registers

The traditional variable register would not be available to Custom Computer Programs and ComputerFunctions. 

### File System

The Computer system would have a concept of files. Files are essentially all just text files with text name handles. They are the main way in which data is stored on these systems and are a way to have persistent memory as well.

### Restricted Statements

All statements will be available except for the below statements:

- Console
- Delay
- DelayProg
- Force
- Send

### Additional Statements

There are a few statements that will be only available for Custom Computer Programs and not regular progs. A non-exhaustive list would include:

- Sleep (a way of waiting a period of time and yielding control back)

### Statement Handling

All IStatements will have boolean properties that say whether they are valid in regular progs or computer progs. Using an incorrect statement will produce a compile time error.

### Restricted Functions

By default none of the built-in functions will be available in Custom Computer Programs. Only those that specifically opt-in will be permitted. A tentative list of these would be:

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

## Systems Needed

- Concrete types for ComputerPrograms, CustomComputerPrograms ComputerFunctions and Files
- A way of marking statements and functions as being available for FutureProgs vs ComputerFunctions. FutureProgs would be enabled by default unless disabled. ComputerFunctions is disabled by default unless enabled.
- Some of the core language handling might need some branching, like for example the ability to call FutureProgs with the @ syntax would need to be unavailable in ComputerFunctions or alternately map to those instead
- A desktop computer item to run these programs on
- A microcontroller item type that has input channels and output channels that can be connected to other items, and can run ComputerFunctions on its channels to handle logic
- A couple of signal channel enabled item types to test around with, like a lock that can be controlled by a signal and/or a light and/or a power switch, as well as items to feed signals in like a push button, movement sensor, light sensor etc.
- An internet grid type including the equivalent tie-ins to the grid, cell towers etc. Possibly consider extending the internet grid as a special type of telecommunications grid so the same grid can do both.
- A programming check and command verb that allows players to write programs
- An electrical check and command verb that allows players to install systems, microcontrollers, wire signals together etc. This verb should be able to be used unskilled but carry the risk of electrocution.
- Both of these command verbs should be able to be surpressed so that they don't appear on non-modern MUDs.

## Unsolved Design Questions

- Signals will likely be initiated by events (player input, movement, and actual Events from the Event System. They will also typically have a fixed duration that they're sent and/or pulsed for. We want to make it so that ideally if nothing changes we don't have to constantly reassess the logic and the signals so there needs to be consideration to that in how things are implemented. The pulsing is mostly to give microcontrollers the opportunity to reassess their logic - if their output stays the same pulse to pulse the signal should be considered continuous.
- 

## Conceptual Example - Motion Activated Door

- There is an item with an Electronic Door item component which uses a signal channel to decide whether it's open or closed. 
- Electronic Door is an IAutomationEnabled item which exposes signal channels and can have those channels connected to other items and have microcontrollers installed in those channels as well
- There is an item with a Microcontroller item component that can be programmed by players and installed in the Electronic Door
- There is an item with a MovementSensor item component which sends a signal for a set duration after detecting movement in the vicinity of the automatic door

The signal channel the door exposes is called "DoorOpen". When it receives any value other than zero for this signal it opens the door. When the signal stops it closes the door.

The Microcontroller is programmed with an input parameter of "Signal1" and returns number.

The player programs the Microcontroller with the following function:

```
// Short circuit exit if signal is close
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

The player installs the microcontroller on the door and maps its "Signal1" input to the "DoorOpen" output of the door.

They then install the Movement Sensor on the CellExit that the door is installed in and connect its "MotionDetected" signal to "Signal1" on the microcontroller on the door. They set the motion detection to MinimumSize=Normal and SignalDuration to 45 seconds.

A player approaches the door. The CharacterBeginMovement event fires. The sensor wasn't already sending a signal and they are size Normal so the motion sensor sends MotionDetected:1. The microcontroller receives this as Signal1, runs it through its logic and sees that it is 11am and therefore passes the signal on to the door, and the door opens for 45 seconds.