# Event System for AI and Hooks

## Purpose
This document explains the parts of FutureMUD's event system that AI authors and maintainers actually need to understand.

It covers:

- `EventType` and event metadata
- `IHandleEvents` and related contracts
- hook contracts and runtime hook classes
- how AI receives events
- how builders inspect events and create hooks

This is intentionally narrower than a full engine-wide event encyclopedia. The companion document [NPC_AI_and_Group_AI_Runtime.md](./NPC_AI_and_Group_AI_Runtime.md) covers the AI runtime itself.

## Primary Contracts
| Contract | Role |
| --- | --- |
| `EventType` | The canonical enumeration of engine events |
| `EventInfoAttribute` | Metadata describing an event's human-readable purpose and FutureProg-compatible parameter signature |
| `IHandleEvents` | Standard event receiver surface for perceivables and AI |
| `IHandleEventsEffect` | Effect-level event receiver surface |
| `IHook` | Persistent event-to-action binding installed on perceivables |
| `IHookWithProgs` | Hook subtype that exposes linked FutureProg payloads |
| `IDefaultHook` | Rule that auto-installs a hook on matching future perceivables |
| `ICommandHook` | Hook subtype that matches a specific command token |
| `IExecuteProgHook` | Hook subtype that supports adding/removing FutureProg payloads |

## `EventType` and Event Metadata
### `EventType`
`EventType` in `FutureMUDLibrary/Events/EventTypeEnum.cs` is the source of truth for event identifiers.

For AI authors, the important point is that event names are only half of the contract. The full contract is:

- the enum member
- the parameter order
- the semantic meaning of each parameter
- the FutureProg compatibility signature

### `EventInfoAttribute`
Every event entry carries `EventInfoAttribute`, which stores:

- a human-readable description
- parameter type names for display
- parameter variable names for display
- FutureProg-compatible parameter types

This metadata powers:

- `show events`
- `show event <event>`
- hook/prog compatibility checks when building hooks

If you change an event's runtime argument order without updating its metadata, builders and hook validation will drift out of sync.

## `IHandleEvents`
`IHandleEvents` defines the standard event receiver surface:

- `HandleEvent(EventType type, params dynamic[] arguments)`
- `HandlesEvent(params EventType[] types)`
- hook installation/removal methods
- hook collection access

Many engine objects implement this surface, including perceivables and NPCs. AI does not normally install hooks on itself, but it does implement `HandleEvent` and `HandlesEvent` through `IArtificialIntelligence`.

### Why `HandlesEvent` Matters
For AI, `HandlesEvent` is an operational subscription hint, not just a convenience method. `NPC.SetupEventSubscriptions()` inspects attached AIs and only subscribes the NPC to periodic heartbeats if one of its AIs claims that event.

That is why overly broad `HandlesEvent` implementations are undesirable.

## Hook Contracts
### `IHook`
`IHook` is a persistent event binding with:

- `Function`: runtime handler for the event payload
- `Type`: target `EventType`
- `Category`: builder-facing categorization
- `InfoForHooklist`: summary text for `hook list`

Hooks are installed on perceivables such as rooms, characters, and items.

### `IHookWithProgs`
This is the common subtype used by hooks that directly execute one or more FutureProgs.

### `IDefaultHook`
Default hooks auto-install a normal hook on future perceivables of a matching type when an eligibility prog returns true. This is how builders can say "all future rooms/characters/items matching this rule should get this hook".

### `ICommandHook`
Command hooks are special because they only fire when the target event is:

- `CommandInput`
- `SelfCommandInput`

and the intercepted command text matches the configured command token.

### `IExecuteProgHook`
This subtype is used by hook flows that can add or remove FutureProg payloads after creation.

## Runtime Hook Implementations
### `HookBase`
`HookBase` is the common runtime base for hooks. It provides:

- loader registration through `HookLoaders`
- save/load plumbing
- shared identity/category/event-type behavior
- reflection-driven `SetupHooks()`

Hook loading is parallel in concept to AI loading:

- each hook subtype exposes a static `RegisterLoader()`
- `HookBase.SetupHooks()` discovers and registers them
- the persisted `Type` discriminator chooses the concrete loader

### `FutureProgHook`
`FutureProgHook` is the common "run one or more progs when this event fires" implementation.

Its runtime behavior is straightforward:

- ignore events whose `EventType` does not match
- execute each stored prog with the raw event parameter list
- return true once the hook has run

This simplicity is why event signature correctness matters so much. The hook does not reinterpret or reorder the payload for you.

### `HookOnInput`
`HookOnInput` is the command-input specialization for:

- `CommandInput`
- `SelfCommandInput`

It additionally checks the command token and only fires when it matches `TargetCommand`.

This is the major exception to the otherwise simple "event type is enough" rule for hooks. Command-input hooks need both:

- the event family
- the command text

### `DefaultHook`
`DefaultHook` links:

- a perceivable type
- an eligibility prog
- a normal hook

It is not itself an event receiver. It is an installation rule that decides whether future perceivables should receive the referenced hook.

### Other Hook Types
The hook subsystem also includes additional concrete types such as:

- `CommandHook`
- `CommandHookFutureProg`
- `GPTHook`

For most AI and content-authoring work, the primary practical types are still `FutureProgHook`, `HookOnInput`, and `DefaultHook`.

## Save/Load Behavior
Hook persistence is split between:

- the common hook row in `Models.Hooks`
- subtype-specific XML definitions
- separate default-hook rows for `DefaultHook`

The common row stores:

- hook id
- name
- category
- target event type
- concrete hook subtype discriminator
- XML definition

At boot:

1. `HookBase.SetupHooks()` registers known hook loaders.
2. persisted hooks are loaded from the database
3. each row is delegated to the registered concrete loader

## Dispatch Model
### Direct `HandleEvent` Dispatch
Perceivables and related engine objects can receive events directly through `HandleEvent(...)`.

For AI, the main path is:

1. the NPC receives an engine event
2. `NPC.HandleEvent(...)` forwards it to attached AIs
3. each AI may react according to its own `HandleEvent(...)`

AI events do not block ordinary hooks from firing. The NPC still delegates to its base event handling path.

### Hook Dispatch
Hooks are installed on event-capable perceivables. When those perceivables process an event, installed hooks of the matching type are eligible to run.

In practical terms:

- AI is part of the NPC's behavior package
- hooks are installed event responders attached to items, rooms, characters, and similar targets

### Heartbeats
Heartbeat events are normal `EventType` members:

- `FiveSecondTick`
- `TenSecondTick`
- `MinuteTick`
- `HourTick`

For NPC AI:

- these are opt-in through `HandlesEvent`
- the NPC subscribes only when needed

For group AI:

- group instances subscribe directly to the ten-second and minute heartbeat managers
- there is no `IHandleEvents` layer between the group and the heartbeat

### Load/Finish Events
AI authors commonly rely on:

- `CharacterEntersGame`
- `NPCOnGameLoadFinished`
- `ItemFinishedLoading`

These are useful for initialization logic that should run once on load or first activation.

## Builder and Inspection Workflow
### Inspecting Events
Builders use:

- `show events`
- `show event <event>`

`show event <event>` is the safest way to confirm:

- whether the event is the right one
- parameter ordering
- parameter meaning

### Creating Hooks
Builders use:

- `hook create <name> <prog> <event>`
- `hook create <name> <prog> CommandInput|SelfCommandInput <command>`

Normal hooks need only the event type.
Command-input hooks additionally need the intercepted command token.

### Editing Hooks
Important editing commands include:

- `hook prog <hook> <prog>`
- `hook command <hook> <command>`
- `hook rename <hook> <name>`
- `hook category <hook> <category>`

### Installing and Removing Hooks
Builders attach hooks to targets with:

- `hook install <hook> <target>`
- `hook remove <hook> <target>`

Targets are usually:

- a room (`here`)
- a character
- an item

### Default Hooks
Builders manage installation rules with:

- `hook defaults`
- `hook adddefault <perceivable type> <hook> <filter prog>`
- `hook remdefault <perceivable type> <hook> <filter prog>`

Supported default-hook perceivable types are currently:

- room / cell
- character
- item

## Signature Safety and Compatibility
### Event metadata is the source of truth
When creating or editing hooks, the engine validates FutureProg compatibility against the event metadata. That means:

- event metadata must remain accurate
- prog parameter lists must match the documented event shape

For AI authors, the same principle applies even though `HandleEvent` itself is not validated the same way. The metadata remains the authoritative description of what a given event means.

### Command-input events are a special case
`CommandInput` and `SelfCommandInput` are unusual because:

- the hook builder requires an extra command token
- the runtime hook checks both the event family and the command text

Do not treat these as normal "fire on any command" hooks unless that is truly the intent.

### Parameter order discipline
Because event payloads are positional, changing parameter order is a breaking change for:

- hooks
- AI `HandleEvent(...)`
- FutureProgs bound to those events
- `show event` documentation accuracy

If you must change an event:

1. update the `EventType` metadata
2. update all hook/AI consumers
3. confirm builder flows still validate the correct signature

## AI Integration Guidance
### Use a dedicated AI class when

- behavior is continuous or stateful
- timing and repeated event handling matter
- pathing, combat cadence, or multi-step reactions are required
- the logic needs normal builder support and reusable AI configuration

Examples:

- wandering
- aggression
- shopkeeping
- legal authority behavior
- self-care and stealth cadence

### Use hooks and progs when

- the reaction is local to a room/item/character rather than a reusable AI package
- the behavior is small and event-driven
- builders should be able to attach it surgically without introducing a new AI type
- the response is mostly a prog-side effect or echo

Examples:

- room greets on entry
- item-specific triggered logic
- command interception on a specific target

### Use both when appropriate
Some systems benefit from both:

- AI handles the reusable long-lived behavior
- hooks provide localized or content-specific supplements

For example, an NPC can have a normal AI package while a particular room or item still has hooks that add scenario-specific reactions.

## AI-Relevant Event Families
The current AI set leans heavily on a subset of the total event enum.

### Movement
Frequently used movement events include:

- `CharacterEnterCell`
- `CharacterEnterCellWitness`
- `CharacterEnterCellFinish`
- `CharacterEnterCellFinishWitness`
- `CharacterBeginMovement`
- `CharacterBeginMovementWitness`
- `CharacterLeaveCell`
- `CharacterLeaveCellWitness`
- `CharacterStopMovement`
- `CharacterStopMovementClosedDoor`
- `CharacterCannotMove`

These drive wanderers, trackers, herds, doorguards, and greeting/farewell logic.

### Item Interaction
Useful item interaction events include:

- `CharacterOpenedItem`
- `ItemOpened`
- `CharacterOpenedItemWitness`
- `CharacterClosedItem`
- `ItemClosed`
- `CharacterClosedItemWitness`
- `ItemLocked`
- `ItemLockedWitness`
- `ItemUnlocked`
- `ItemUnlockedWitness`

These provide hook/prog surfaces for doors, containers, lockable props, alarms, security scripts, and NPC reactions to people manipulating objects in a room. Lock events fire on the lock item and on witnesses; the actor and key parameters may be null when a lock changes state without a keyed actor action.

### Equipment
Useful equipment events include:

- `ItemWielded`
- `ItemWieldedWitness`
- `ItemUnwielded`
- `CharacterUnwieldedItem`
- `CharacterUnwieldedItemWitness`
- `ItemWorn`
- `ItemWornWitness`
- `CharacterWornItemRemoved`
- `ItemRemovedFromWear`
- `CharacterWornItemRemovedWitness`

These are useful for weapon alarms, cursed or reactive equipment, guards responding to armed characters, and content that cares about people putting on or removing visible gear.

### Mounts
Mounting events include:

- `CharacterMounted`
- `CharacterMountedWitness`
- `CharacterDismounted`
- `CharacterDismountedWitness`

These let mount AI, stable areas, guards, and scenario hooks react to riders mounting or dismounting character mounts.

### Combat
Common combat events include:

- `EngageInCombat`
- `EngagedInCombat`
- `EngagedInCombatWitness`
- `LeaveCombat`
- `NoNaturalTargets`
- `TargetIncapacitated`
- `TargetSlain`
- `TruceOffered`
- `NoLongerEngagedInMelee`

These drive aggressive AI, combat-ending behavior, rescue behavior, and herd threat reactions.

### Crime and Law
Current AI-relevant legal events include:

- `WitnessedCrime`
- `VictimOfCrime`

These are important for law-enforcement style behaviors and related content logic.

### Speech and Command Input
Current speech/input events include:

- `CharacterSpeaks`
- `CommandInput`
- `SelfCommandInput`
- `CommandIssuedToCharacter`
- `TelephoneDigitsReceived`

These are relevant to commandable NPCs, hook-based command interception, and speech-aware content.

`TelephoneDigitsReceived` is the telecom-specific input event:
- it fires on the receiving item or service endpoint, not the sending handset
- the payload is `(source item, digits text)`
- it is the intended hook/prog surface for keypad-driven routing, voicemail navigation, and future IVR-style services

### Shop and Economy
Important shop events include:

- `BuyItemInShop`
- `WitnessBuyItemInShop`
- `ItemRequiresRestocking`

These are primarily used by `ShopkeeperAI`.

### Damage and Health
Important damage/health events include:

- `BleedTick`
- `CharacterDamaged`
- `CharacterDamagedWitness`
- `CharacterDies`
- `CharacterDiesWitness`
- `CharacterIncapacitated`
- `CharacterIncapacitatedWitness`

These are relevant to self-care, reaction, and herd stress behavior.

### Weather
`WeatherChanged` exists and is already used by `ReactAI`.

### Heartbeats
Periodic tick events used by AI include:

- `FiveSecondTick`
- `TenSecondTick`
- `MinuteTick`
- `HourTick`

### Load and initialization
Important startup/load events include:

- `CharacterEntersGame`
- `NPCOnGameLoadFinished`
- `ItemFinishedLoading`

## Adding or Changing Events Safely
When you add or change an event that AI or hooks may consume:

1. add or update the `EventType` enum member
2. add or update `EventInfoAttribute`
3. confirm the parameter order reflects actual runtime behavior
4. verify `show event` displays the intended contract
5. verify hook/prog compatibility logic still behaves correctly
6. search for AI `HandleEvent` consumers that may assume the old payload

Introduce a new event only when an existing event family cannot express the behavior cleanly. The engine already has many narrowly named events; adding more should be deliberate.

## Practical Advice for AI Authors
- Start from `show event <event>` before implementing a new `HandleEvent(...)` branch.
- Keep `HandlesEvent(...)` narrow so NPC heartbeat subscriptions stay intentional.
- Prefer hook/prog solutions for tiny local reactions and dedicated AI classes for ongoing behavior.
- If an event is builder-facing, treat its metadata as part of the public API.
- Be especially careful with command-input hooks because they have an extra command-token contract beyond the enum member itself.

## Related References
- Primary AI runtime document: [NPC_AI_and_Group_AI_Runtime.md](./NPC_AI_and_Group_AI_Runtime.md)
