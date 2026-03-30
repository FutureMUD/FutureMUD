# FutureMUD Item System Presentation and Integration

## Scope
This document explains how item behaviour is presented to players and how items integrate with adjacent runtime systems.

The focus is on:
- descriptions and decorators
- inventory, containment, and location behaviour
- item groups and skins
- cross-system integration points such as health and magic

## Description Model
### Item-level descriptions
Item prototypes provide the base descriptive layer:
- noun
- short description
- full description
- optional long description override
- extra descriptions gated by progs

Skins can override several of these presentation values without replacing the underlying item behaviour.

### Component-driven description decoration
Components can decorate item descriptions by overriding:
- `DescriptionDecorator(...)`
- `Decorate(...)`
- `DecorationPriority`
- `WrapFullDescription`

This is how components add behavioural presentation without forcing all description logic into `GameItem` itself.

Examples include:
- containers showing fullness, open state, and contents
- locks or connections contributing extra information
- effects such as glow adding descriptive layers through adjacent systems

### Decoration ordering
Decoration ordering matters:
- non-negative priorities are applied before colour processing
- negative priorities are applied after colour processing in descending order

When adding a decorator component, choose a priority deliberately so its output composes cleanly with existing decorators.

## Containment, Inventory, and Location
### Items are not only "in a room"
Items may be:
- directly in a cell
- held or worn by a body
- inside containers
- attached to belts or other items
- installed as doors or furniture-adjacent objects
- implanted or prosthetic
- connected into power or grid systems

Because of this, the item system exposes:
- `TrueLocations`
- `LocationLevelPerceivable`
- `ContainedIn`
- `InInventoryOf`
- deep and shallow item traversal helpers

### Why this matters for component authors
Any component that changes where an item effectively "is" in the world needs to consider:
- location resolution
- movement
- morph and destruction transfer
- inventory change propagation

Common examples:
- `IContainer`
- `IBelt` and `IBeltable`
- `IConnectable`
- `IDoor`
- `IHoldable`
- `IWearable`
- `IImplant`
- `IProsthetic`

## Item Groups
### Purpose
Item groups exist to control room presentation when many similar items are present.

Instead of showing every item individually, a group can provide a form-based grouped description appropriate to the cell.

### Developer relevance
Item groups matter when:
- your new item type is likely to exist in large numbers
- room clutter would otherwise drown out important content
- you need controlled presentation for furniture-heavy or prop-heavy spaces

From the item side, the prototype simply references an `IGameItemGroup`. The grouping logic itself is separate from component behaviour.

## Skins
### Purpose
Skins provide presentation variation without duplicating item behaviour definitions.

An item skin can override:
- item name
- short description
- long description
- full description
- quality

### Developer relevance
Skins are important whenever:
- behaviour should stay constant but appearance should vary
- players or builders need cosmetic variants
- content should be customised without cloning behaviour-heavy item prototypes

When changing item presentation flows, remember that skins may override prototype text.

## Health, Magic, and Other System Integrations
### Health integration
Items participate in the health system more directly than many engines would expect:
- `GameItem` has health strategy integration
- item components can act as corpses, medical tools, implants, prosthetics, breathing support, or treatment systems

This means item features often have consequences for:
- wound handling
- destruction
- surgery
- breathing
- body-state modelling

### Magic integration
Items also integrate with magic through:
- item-owned effects and magic resources
- magic-aware item methods and persistence
- spell effects that can target items

Any new component that changes item descriptions, energy state, or visibility may end up interacting with spell effects and magic systems.

### FutureProg integration
Items and components can be discovered and manipulated through FutureProg-facing systems, while item prototypes can execute on-load progs and hold default register values.

Variable-driven items are a common integration point between item content and scripting.

## Real Example: Container as Presentation + Integration
The container implementation is a strong example because it touches both presentation and system integration.

Presentation:
- adds full-description detail about openness and fullness
- contributes contents lists
- exposes lock information

Integration:
- manages contained items and lock items
- affects weight and buoyancy
- handles morph and destruction transfer
- changes what items are shallowly accessible

It is a good reference whenever a new component needs to both change behaviour and explain that behaviour to players.

## Special-Case Components
Read-only framework components such as `Holdable` still participate in presentation and integration even though they are not ordinary builder-authored component types.

They are important because they establish foundational assumptions:
- whether an item can be picked up
- whether the item should fall
- whether inventory-oriented commands are even meaningful

When troubleshooting strange item behaviour, always verify whether one of these framework-level components is part of the composition.

## Guidance for New Work
- If a feature changes what a player should understand about an item, consider description decoration.
- If a feature changes where the item functionally exists, consider location-level behaviour and movement.
- If a feature is mostly about room clutter and presentation, consider item groups before inventing bespoke logic.
- If a feature is cosmetic-only, consider skins before cloning prototypes.
- If a feature touches health, magic, or scripted behaviour, document the integration explicitly rather than treating it as incidental.
