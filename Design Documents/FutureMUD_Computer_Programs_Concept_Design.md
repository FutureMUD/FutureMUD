# FutureMUD Computer Program Design

This is a concept design document to outline a system in FutureMUD for in-world computer programs, to allow builders and players to make custom computer logic that runs on computer and microcomputer systems. This system is not yet implemented.

## Core Concepts

- The Program system will be shared between computers, microcomputers, controllers and other small bits of in-world compute type tasks
- The system will use a cut-down version of the FutureProg system - in fact, these will be an implementation of IFutureProg, just with greatly reduced variable types and function restrictions
- There are ComputerPrograms, which are executed on in-game computers and run some code until they exit or terminate
- Some ComputerPrograms are hard-coded by FutureMUD and they handle complex systems, like mail or chat or whatnot
- Other ComputerPrograms are soft-coded by users and these are CustomComputerPrograms
- There are ComputerFunctions, which can be recycled and called by CustomComputerPrograms as well as power things like Microcontrollers
- These are designed to be stored as text on other data structures like items, computers and the like rather than sitting in a central repository like conventional IFutureProgs
- They are still compiled before being executed

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

## Systems Needed

- Concrete types for ComputerPrograms, CustomComputerPrograms ComputerFunctions and Files
- A way of marking statements and functions as being available for FutureProgs vs ComputerFunctions. FutureProgs would be enabled by default unless disabled. ComputerFunctions is disabled by default unless enabled.
- Some of the core language handling might need some branching, like for example the ability to call FutureProgs with the @ syntax would need to be unavailable in ComputerFunctions or alternately map to those instead
- A desktop computer item to run these programs on
- A microcontroller item type that has input channels and output channels that can be connected to other items, and can run ComputerFunctions on its channels to handle logic
- A couple of signal channel enabled item types to test around with, like a lock that can be controlled by a signal and/or a light and/or a power switch