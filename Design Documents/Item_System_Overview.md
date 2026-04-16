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
- signal-input components such as `PushButton`, `ToggleSwitch`, `MotionSensor`, `LightSensor`, `TimerSensor`, and `Keypad`
- signal-output components such as `ElectronicDoor`, `ElectronicLock`, `SignalLight`, `RelaySwitch`, and `AlarmSiren`

Those families should share common computer/signal contracts, but each concrete behaviour should still have its own prototype, runtime component, builder help, and persistence rules.

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
