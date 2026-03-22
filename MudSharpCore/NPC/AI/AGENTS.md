# Scope

This document defines the specific rules for `MudSharpCore/NPC/AI`.
It inherits from the project-level [AGENTS.md](../../AGENTS.md) and the solution-level [AGENTS.md](../../../AGENTS.md).

## Purpose
This folder owns the concrete runtime implementation of FutureMUD's NPC AI and group AI systems.

Use this guide when:

- implementing a new AI class
- extending an existing AI class
- adding a new group AI type
- changing AI persistence, registration, or builder workflow

## Choose the Right Level First
Before writing code, decide which of these you are building:

- simple individual AI: local event-driven or periodic behavior on one NPC
- pathing AI: movement-heavy behavior that needs route selection and travel support
- prog-target pathing AI: movement-heavy behavior whose destinations or targets should be builder-configurable
- group AI type: shared multi-NPC behavior with alertness, roles, and shared state

Do not use group AI when the behavior is fundamentally per-NPC.
Do not create a brand-new AI class when a small FutureProg hook or an existing AI plus more progs would solve the problem more cleanly.

## Individual AI Registration Checklist
For a normal new AI type, implement all of the following:

- derive from the appropriate base, usually `ArtificialIntelligenceBase`, `PathingAIBase`, or `PathingAIWithProgTargetsBase`
- add a database-load constructor that accepts the EF AI row and `IFuturemud`
- add a builder/new constructor for creating a new AI definition
- implement `SaveToXml()`
- implement `HandleEvent(...)`
- implement `HandlesEvent(...)`
- implement `Show(...)` when the base output is insufficient
- implement `BuildingCommand(...)` and subtype-specific validation
- expose a public static `RegisterLoader()`
- call `RegisterAIType(...)`
- call `RegisterAIBuilderInformation(...)` unless the type is intentionally load-only

If builder registration is missing, the AI can survive reboot but cannot be created through the normal `ai edit new ...` flow.

## Group AI Type Checklist
For a normal new group AI type:

- derive from `GroupAIType` unless there is a strong reason not to
- implement `HandleMinuteTick(...)`
- implement `LoadData(...)`
- implement `GetInitialData(...)`
- implement `SaveToXml()`
- add a public static `RegisterGroupAIType()`
- register both database and builder loaders with `GroupAITypeFactory`
- ensure the builder loader validates arguments clearly

If the type is not registered, builders cannot select it from `gait set type ...`.

## Event-Handling Checklist
- Keep `HandlesEvent(...)` narrow and accurate.
- Only claim heartbeat events the AI truly needs.
- Remember that NPC heartbeat subscription is driven by `HandlesEvent(...)`.
- Confirm event payload order against `EventTypeEnum.cs` and `show event <event>`.
- Do not assume event argument order from naming alone.
- Prefer early returns for irrelevant state such as dead, paused, immobile, or already-engaged actors.

## Heartbeat Guidance
- `FiveSecondTick`, `TenSecondTick`, `MinuteTick`, and `HourTick` are relatively expensive because they drive recurring work.
- Avoid broad polling when a more specific event would do.
- For movement-heavy or scanning-heavy behaviors, be deliberate about tick frequency.

## Persistence Checklist
- Keep save and load XML in sync.
- Make default values explicit when loading older or incomplete XML.
- Treat `RawXmlDefinition` as a durable schema, not a temporary dump.
- Avoid storing mutable per-NPC state on shared `IArtificialIntelligence` definitions unless that sharing is intentional.
- For group AI, keep mutable state on the `IGroupAI` instance or its `IGroupTypeData`.

## Clone Safety
- `Clone(...)` duplicates the AI definition, not any live NPC state.
- Ensure new config fields are included in saved XML so cloning carries them across.
- If an AI is widely reused, prefer cloning rather than mutating the shared original when the change is content-specific.

## Builder-Surface Checklist
- Provide clear `TypeHelpText`.
- Implement `Show(...)` so builders can see all meaningful configuration at a glance.
- Validate builder input immediately and return actionable error text.
- Keep command names short and consistent with existing AI conventions.
- When emotes or progs are expected, validate them during building rather than failing later at runtime.

## Shared Patterns to Prefer
### FutureProg-driven policy
Prefer progs for game-specific policy such as:

- whether behavior is enabled
- who counts as friend, foe, or target
- which room is acceptable
- what fee or message should be used

Keep long-lived control flow in C# and content policy in progs.

### Emote-driven reactions
Use validated emotes for readable player-facing feedback.
Be explicit about token expectations such as `$0` actor and `$1` target.

### Movement gating
If the AI moves, respect the engine's movement and action blockers.
Reuse the pathing bases and their helpers rather than bypassing them.

### Combat reactions
Combat AI should set `CountsAsAggressive` deliberately when appropriate.
Treat truce, incapacitation, target loss, and post-engagement cleanup as first-class behaviors rather than afterthoughts.

## When to Reuse Existing Bases
### Use `ArtificialIntelligenceBase` when
- the AI is mostly event-driven
- it has limited or no pathfinding needs
- the logic is self-contained

### Use `PathingAIBase` when
- the AI needs deliberate movement
- doors, keys, guards, and path-following matter

### Use `PathingAIWithProgTargetsBase` when
- targets or destinations should be content-configurable
- routing decisions belong in builder-authored progs

### Use `GroupAIType` when
- behavior coordinates multiple NPCs
- roles, alertness, or shared memory matter
- shared state belongs on the group, not on each member individually

## Template vs Shared-State Warning
Most individual AI definitions are shared configuration objects reused by many NPCs.

Because of that:

- do not casually cache per-NPC mutable state on the AI instance
- prefer deriving state from the NPC, effects, or group membership
- if truly per-NPC persisted state is needed, design that explicitly elsewhere

## `WildAnimalHerdAI` Special Case
`WildAnimalHerdAI` is a legacy/advanced implementation and is not registered for normal builder creation.

Do not copy it blindly as the default pattern for new AI work.
Before extending it or using it as a template, decide whether the intended new behavior should instead be:

- a normal builder-creatable AI
- a group AI type
- a modernization/refactor of the existing herd stack

## Group AI Template Guidance
- Template changes are immediate and non-revisable.
- Be careful when editing emotes, avoid/threat progs, or group type on a template that live groups already use.
- When changing a group type on a template, confirm how existing groups' `Data` should be reinitialized.

## Event-System Relationship
This folder implements AI behavior on top of the contracts in `FutureMUDLibrary/Events`.

When adding new event-driven AI:

- verify the event already exists before proposing a new one
- use `show event <event>` semantics as the builder-facing contract
- update the event-side documentation if you change or add event usage patterns

## Practical Rule of Thumb
If the behavior sounds like "an NPC that does X over time", it likely belongs here.
If the behavior sounds like "this particular room/item/character should fire a small reaction when Y happens", prefer the hook/prog system instead.
