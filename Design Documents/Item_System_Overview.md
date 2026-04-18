# FutureMUD Item System Overview

## Purpose
This document set explains how the FutureMUD item system is structured, how item behaviour is composed through components, and how developers should add or extend item functionality.

The intended audience is engine developers. Builder commands and content workflows are included where they matter for implementing, validating, and shipping item work.

## Document Map
- [Item System Runtime Model](./Item_System_Runtime_Model.md) explains the core runtime abstractions, instance-versus-prototype model, persistence, revisions, and update flows.
- [Item System Component Authoring](./Item_System_Component_Authoring.md) explains how to add a new item capability or "item type" through interfaces, component prototypes, runtime components, registration, and content attachment.
- [Item System Content Workflows](./Item_System_Content_Workflows.md) explains the builder-facing `item` and `comp` workflows developers rely on when testing and shipping content.
- [Item System Presentation and Integration](./Item_System_Presentation_and_Integration.md) explains how items present themselves to players and how they integrate with inventory, grouping, skins, health, magic, and other runtime systems.

## Core Idea
FutureMUD items are composed rather than inherited.

At runtime:
- `IGameItem` is the live perceivable object in the world.
- `IGameItemProto` is the revisioned definition used to create live items.
- `IGameItemComponent` adds a concrete slice of runtime behaviour to a live item.
- `IGameItemComponentProto` is the revisioned, reusable definition for a component.

Most gameplay-facing item behaviour is discovered by checking whether an item contains a component that implements some public interface such as `IContainer`, `IWearable`, `IMeleeWeapon`, `IProduceLight`, `ITelephone`, `ICellPhoneTower`, or `IImplant`.

## Recommended Reading Order
1. Start with [Item System Runtime Model](./Item_System_Runtime_Model.md) if you need to understand how items actually work.
2. Continue with [Item System Component Authoring](./Item_System_Component_Authoring.md) if you are adding a new item capability.
3. Use [Item System Content Workflows](./Item_System_Content_Workflows.md) when you need to create, revise, attach, load, or review content.
4. Use [Item System Presentation and Integration](./Item_System_Presentation_and_Integration.md) when working on descriptions, grouping, skins, or cross-subsystem behaviour.

## Important Notes
- The fastest way to add a new item capability is usually to add a new component prototype and component pair, not a new `GameItem` subclass.
- Telecommunications items follow the same composition model: a wired telephone handset, a telecommunications outlet, a telecommunications feeder, a cell tower, a cellular handset, and an implant telephone are all ordinary item capabilities expressed through components and public interfaces.
- Computer and signal automation work should follow the same pattern. Shared interfaces such as `IComputerHost`, `IComputerFileSystem`, `ISignalSource`, and `ISignalSink` belong in `FutureMUDLibrary`, while concrete behaviour should be delivered through distinct item component families rather than one generic "automation item" component.
- The `Item Templates/GameItem` template is intended to be a starting skeleton, not a complete implementation. The authoring document calls out the manual work the template does not solve.
- Some component types are special cases. For example, `Holdable` is a read-only auto-initialised component type and should be treated differently from ordinary editable component prototypes.

## Computers And Signals
The planned computer-programs subsystem is an item-system feature, not a separate inheritance tree.

The intended component families are:
- host components such as `ComputerHost`, `ComputerTerminal`, `ComputerStorage`, and `NetworkAdapter`
- logic components such as `Microcontroller`
- signal-input components such as `PushButton`, `ToggleSwitch`, `MotionSensor`, `LightSensor`, `RainSensor`, `TemperatureSensor`, `TimerSensor`, and `Keypad`
- signal-output components such as `ElectronicDoor`, `ElectronicLock`, `SignalLight`, `RelaySwitch`, and `AlarmSiren`

Those families should share common computer/signal contracts, but each concrete behaviour should still have its own prototype, runtime component, builder help, and persistence rules.

The first shipped automation slice now includes:
- `ComputerHost`
- `ComputerTerminal`
- `ComputerStorage`
- `NetworkAdapter`
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

That slice now combines three patterns:
- same-item local bindings with stable local source identifiers plus explicit local endpoint keys
- separate mountable modules installed into `AutomationMountHost` bays as real items rather than collapsed components
- adjacent-room one-hop signal cable items that mirror one source endpoint across a specific exit

Builders still author local bindings by component prototype name or id, and the current built-in source families all expose a default local endpoint key of `signal`, but runtime resolution no longer depends on later component renames. `AutomationHousing` is now the dedicated housing or junction component family for concealed modules and cable ends, and is itself the lockable/openable/container access capability on the item. `MotionSensor`, `LightSensor`, `RainSensor`, `TemperatureSensor`, `TimerSensor`, `Keypad`, and `Microcontroller` are now powered-machine or selectable-input implementations, and the powered-machine families can optionally draw power from an automation host's parent-item power source when mounted. This is enough to support authored control panels, ambient and weather-driven automation, timed local automation, keypad entry panels, mounted controllers, relay-controlled power paths, indicator lights, signal-driven doors and locks, motion-triggered alarms, concealed service housings, and one-room-at-a-time wiring while leaving broader graphs, richer sensors, and networked hosts for later phases.

The first live player command surface for that slice is also now present:
- `electrical` for inspecting signal-driven items, installing or removing mountable modules, routing or unrouting cable segments, and configuring sinks
- `programming` as a hybrid surface for both:
  - a private workspace of computer functions and programs
  - inspecting and live-programming real microcontroller items, including mounted ones
  - connecting to a powered `ComputerTerminal` and then targeting the connected `ComputerHost` or a mounted `ComputerStorage` as the current programming owner

The first real computer-host slice now also includes:
- powered `ComputerHost` components that own files, executables, running processes, and built-in application exposure
- `ComputerStorage` items that persist files and executables and can be mounted into a host
- `ComputerTerminal` items that own user sessions into a powered host
- `NetworkAdapter` items that represent the host's telecom-backed network-facing capability
- local computer runtime functions such as `ReadFile`, `WriteFile`, `AppendFile`, `FileExists`, `GetFiles`, `WriteTerminal`, `ClearTerminal`, `LaunchProgram`, `KillProgram`, `UserInput`, and `WaitSignal`
- the terminal-facing `type` verb, which now routes foreground terminal input and resumes programs waiting on `UserInput()`
- shipped built-in host applications `SysMon`, `FileManager`, `Directory`, and `Mail`
- the first shipped network service, `Mail`, including host-scoped service enablement, hosted domains, database-backed accounts and mailboxes, and telecom-backed delivery to reachable mail hosts

Those verbs currently use staged delayed actions, inventory plans for tool handling, configurable static-string echoes, and dedicated checks rather than instant state changes.

## Thermal Sources
Room temperature now includes three layers:
- base weather and climate
- cell and zone environmental temperature effects
- item-driven thermal sources

Thermal source items are authored through dedicated item components rather than special item subclasses. The current supported families are:
- `ElectricHeaterCooler`
- `FuelHeaterCooler`
- `ConsumableHeaterCooler`
- `SolidFuelHeaterCooler`

All four use the shared `IProduceHeat` interface, which now exposes:
- a signed ambient room contribution
- explicit signed proximity-band contributions

This allows both heaters and coolers to use the same item capability, and lets a single item influence the room as a whole while also applying stronger or weaker effects to nearby people and things.
