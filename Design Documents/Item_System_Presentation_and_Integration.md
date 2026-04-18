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
- connector-driven items showing their current grid membership and physical links
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

Telecommunications components are a special case of connected location behaviour:
- wired handsets may be physically present in a room or inventory while functionally depending on a connected telecom outlet
- a connected outlet can own the number and grid-service state that the handset presents to players
- cell towers project service to zones rather than directly to adjacent inventory or containment relationships
- linked telecommunications exchanges extend that integration one level further: staff link exchanges explicitly, and full-prefix dialling can route to directly linked area codes while still keeping same-prefix calls local

Connector-aware items often need to describe both sides of their integration:
- a component like a telephone may show the telecommunications grid it is attached to and the device it is physically plugged into
- telecommunications presentation should also expose exchange-level state that matters to play and operations, such as maximum rings before timeout, linked exchanges, signal coverage, and whether the current number is locally assigned or delegated through an endpoint
- fax-machine presentation should additionally expose consumable state and deferred work, such as current ink level, current paper reserves, and how many inbound faxes remain queued in memory
- a liquid pump may need to describe the source container, destination container, and power source all at once
- a battery-backed item may need to explain both its stored charge and the connector it uses to recharge

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

Items now also expose ownership metadata to FutureProg. The item `owner` property can be null and, when present, currently surfaces character or clan owners. Item ownership helper functions support direct ownership checks, property-trust checks, clan-aware trust checks, and ownership mutation for individual items or deep item trees.

Deep ownership mutation follows the practical containment graph that builders and players care about rather than only a single container chain. The current scripting helpers walk the root item, nested container contents, sheath contents, and belted attachments so that claiming or clearing ownership can keep carried equipment sets consistent.

Telecommunications items also expose scripting hooks through item and endpoint queries such as current phone number and shared-number policy, which allows crafting or project systems to assign, clear, or reconfigure telephone numbers at runtime. The same runtime scripting surface applies to cellular handsets and implant telephones, even though implant telephones present their user interaction through neural-interface status and command flows instead of direct room manipulation.

Telephone presentation is not only textual state. For room-facing telephones and cellular phones, ringing is an audible output with an effective ring volume. Players adjust that through the ordinary `switch` command rather than a staff-only builder path: wired phones expose `quiet`, `normal`, and `loud`, while cellular phones also expose `silent`. Nearby rooms may hear ringing through ordinary audio-echo rules, but a silent cellular phone can still vibrate for the wearer if it is sitting inside a worn container. Implant telephones do not emit room audio and instead report ringing and connection progress through implant messaging.

The current signal-automation slice has its own presentation and integration rules:
- `PushButton` decorates the full description to show the selectable keyword and whether it is currently active
- `ToggleSwitch` decorates short and full descriptions to show whether it is currently on or off
- `MotionSensor` decorates the full description to show whether it is currently active plus the movement mode and minimum size it watches for
- `LightSensor` decorates the full description to show its current illumination reading, switch state, power state, and emitted signal value
- `RainSensor` decorates the full description to show its current precipitation reading, switch state, power state, and emitted signal value
- `TemperatureSensor` decorates the full description to show its current ambient temperature reading, switch state, power state, and emitted signal value
- `TimerSensor` decorates the full description to show its active and inactive values, its cycle timings, and which phase it is currently in
- `Keypad` decorates the full description to show that it accepts `select <item> <digits>`, along with its switch, power, and active state
- `Microcontroller` decorates the full description with its current on/off state and numeric output value
- `AutomationMountHost` decorates the full description with named bay state and whether the host is presently serviceable through its sibling `AutomationHousing` access path
- `SignalCableSegment` decorates the full description with whether it is routed, which source endpoint it mirrors, and which exit hop it currently spans
- `AutomationHousing` decorates the full description with whether the housing is sealed or open for service, and when open it lists the concealed automation items inside
- `SignalLight` and `ElectronicLock` reuse the existing light and lock presentation behaviour, but now integrate with sibling signal sources resolved through stable local identifiers plus explicit endpoint keys
- `ElectronicDoor` is its own door component family on the shared internal door base, adds control-state detail to the full description, and emits automatic open or close emotes when the commanded state changes
- `RelaySwitch` primarily presents through electrical inspection because its important state is whether its signal-controlled relay is currently closed and therefore producing power
- `AlarmSiren` decorates short and full descriptions to show whether it is sounding, and integrates audible room output with sibling signal sources plus power state
- `component show` output for signal-driven sinks should present the bound local source endpoint, not just the component family, so future multi-port source families stay understandable
- the current slice is not purely same-item any more, but the external graph is still intentionally modest: presentation should explain mounted modules and one-hop cable segments without implying a full arbitrary wiring network
- the live `electrical` and `programming` verbs are now part of the player-facing integration surface for these items, and their staged begin/continue/success/failure output should be authored through configurable static strings rather than embedded per-component prose
- `programming help` should mirror the normal `prog help` structure, but filtered to the computer-safe subset so the player-facing language reference stays aligned with the actual compiler/runtime surface
- abject failure on electrical work is also part of the presentation layer because it must produce a visible shock emote and corresponding damage feedback
- `AutomationHousing` is now the dedicated housing or junction presentation family, and because it is itself the lockable-container capability, it deliberately layers on ordinary container/openable/lockable item presentation rather than replacing it
- those housings should be presented as ordinary openable service panels on the parent item, reached through normal `open` / `close` subtargets rather than bespoke verbs
- ordinary `look` output for signal-driven items should emphasize physical state and serviceability; live control-signal and binding diagnostics should be presented by `electrical` inspection instead
- player-facing automation and programming diagnostics should prefer parent item keywords and `item@component` notation where needed, rather than exposing raw component ids
- `electrical` inspection should now explicitly present controller input bindings, cable mirror routes, nearby routed cable segments where they are relevant to the inspected item, current signal values, switch and power state where relevant, and whether each upstream dependency is currently resolved or broken
- mounted modules should resolve and present nearby signal sources through the spatial context of their mount host rather than behaving like disconnected pocket items
- motion-driven automation presentation should assume that `IImmwalkEffect` administrators do not participate in witnessed movement signalling; that is a runtime rule, not a content bug

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
- If a feature represents a networked device, decide what description state players need to see: power, number, grid membership, signal coverage, off-hook state, ringing state, and connected peers are all presentation concerns rather than purely backend details.
- If a feature owns a subordinate runtime object such as a grid, document what happens when that subordinate object fails to load. Creator-backed grids now recreate and reinitialise themselves instead of leaving the parent item in a broken null-grid state.
- If a feature is part of the current signal-automation slice, document both the physical control surface players interact with and the sibling signal dependency that drives the response.

## Thermal Source Presentation and Integration
Thermal-source items are both presentation features and simulation features.

Presentation:
- active thermal sources decorate their descriptions with an active-state addendum
- inactive steady-state sources decorate with an inactive-state addendum
- switchable steady-state sources also emit on/off emotes when their operating state changes

Integration:
- room temperature now reads active thermal items directly rather than requiring a bespoke room effect
- ambient thermal contribution only matters in indoor contexts, but proximity contribution always matters
- because the aggregation walks deep room items and characters' external items, worn or carried thermal props can still influence nearby targets
- thermal output stacks across multiple active sources
- cross-layer proximity uses the existing `VeryDistant` band rather than inventing a thermal-only distance rule

The strongest reference case is the new heater/cooler family:
- electric items integrate with the power system through `IConsumePower`
- fuel-fed items integrate with connectable fuel infrastructure through `IConnectable`, `ILiquidContainer`, and `IGasSupply`
- consumable items integrate with timed-burn lifecycle and optional replacement-item morph-like behaviour
- solid-fuel items integrate with normal item containment and tagged-content workflows
