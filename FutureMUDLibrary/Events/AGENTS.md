# Scope

This document defines the specific rules for `FutureMUDLibrary/Events`.
It inherits from the project-level [AGENTS.md](../AGENTS.md) and the solution-level [AGENTS.md](../../AGENTS.md).

## Purpose
This folder owns the shared event contracts used by AI, hooks, perceivables, and effect systems.

Changes here are public API changes. Treat them carefully.

## What Belongs Here
- `EventType`
- `EventInfoAttribute`
- `IHandleEvents`
- `IHandleEventsEffect`
- hook-related shared interfaces such as `IHook`, `IHookWithProgs`, and `IDefaultHook`

## What Does Not Belong Here
- concrete hook implementations
- database persistence
- builder command logic
- AI-specific runtime behavior
- content-specific event handling

Those belong in `MudSharpCore`.

## Adding or Changing an `EventType`
When you add or change an event:

1. add or update the enum member in `EventTypeEnum.cs`
2. add or update `EventInfoAttribute`
3. make sure the description matches real runtime behavior
4. make sure the parameter names and parameter order match real runtime behavior
5. make sure the FutureProg type list matches the actual payload

Do not add an event without metadata unless there is a compelling technical reason.

## `EventInfoAttribute` Requirements
`EventInfoAttribute` is part of the public builder contract because it powers:

- `show events`
- `show event <event>`
- hook/prog compatibility checks

That means inaccurate metadata is a bug, even if runtime dispatch still works.

## Parameter-Order Discipline
Event payloads are positional.

If you change parameter order, you are potentially breaking:

- AI `HandleEvent(...)` implementations
- hook execution
- FutureProg compatibility
- builder expectations from `show event`

Do not reorder parameters casually.

## Hook Compatibility Implications
Hook creation and editing rely on the event metadata to validate prog compatibility.

If the event metadata drifts from reality, builders may:

- attach supposedly compatible progs that fail semantically
- be prevented from attaching actually compatible progs

Treat event metadata as the hook system's source of truth.

## AI Compatibility Implications
AI implementations use the same positional payloads even though they are not validated in the same builder path.

Whenever an event changes, search for:

- `HandleEvent(` consumers
- `HandlesEvent(` consumers
- AI or effect code that assumes a specific argument position

## When to Introduce a New Event
Prefer a new event only when:

- an existing event cannot express the behavior cleanly
- adding more branching to consumers of an old event would make intent less clear
- the new event has a stable, reusable meaning beyond one niche content case

Do not add a new event just to avoid a small amount of consumer-side filtering.

## Naming Guidance
- Prefer names that describe what happened, not who happens to consume it today.
- Keep witness variants and direct-target variants consistent with the existing naming style.
- Follow the established paired naming patterns where possible.

## Minimal Checklist for Event Contract Changes
- Update the enum member and metadata.
- Confirm runtime dispatch uses the same payload order.
- Check whether `show event` output still makes sense.
- Check whether hook creation/edit flows still validate correctly.
- Check whether AI or effect consumers need updates.
- Update the design docs if the change affects AI or hook authoring guidance.

## Notes
- This folder defines the event language of the engine.
- Small contract mistakes here echo into AI, hooks, progs, and builder UX.
- Concrete AI guidance lives in `MudSharpCore/NPC/AI/AGENTS.md`.
