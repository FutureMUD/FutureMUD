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

Most gameplay-facing item behaviour is discovered by checking whether an item contains a component that implements some public interface such as `IContainer`, `IWearable`, `IMeleeWeapon`, `IProduceLight`, or `IImplant`.

## Recommended Reading Order
1. Start with [Item System Runtime Model](./Item_System_Runtime_Model.md) if you need to understand how items actually work.
2. Continue with [Item System Component Authoring](./Item_System_Component_Authoring.md) if you are adding a new item capability.
3. Use [Item System Content Workflows](./Item_System_Content_Workflows.md) when you need to create, revise, attach, load, or review content.
4. Use [Item System Presentation and Integration](./Item_System_Presentation_and_Integration.md) when working on descriptions, grouping, skins, or cross-subsystem behaviour.

## Important Notes
- The fastest way to add a new item capability is usually to add a new component prototype and component pair, not a new `GameItem` subclass.
- The `Item Templates/GameItem` template is intended to be a starting skeleton, not a complete implementation. The authoring document calls out the manual work the template does not solve.
- Some component types are special cases. For example, `Holdable` is a read-only auto-initialised component type and should be treated differently from ordinary editable component prototypes.
