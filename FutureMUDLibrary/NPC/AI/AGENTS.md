# Scope

This document defines the specific rules for `FutureMUDLibrary/NPC/AI`.
It inherits from the project-level [AGENTS.md](../../AGENTS.md) and the solution-level [AGENTS.md](../../../AGENTS.md).

## Purpose
This folder owns the public contracts for NPC AI and group AI.

Code here should define stable, implementation-agnostic abstractions that `MudSharpCore` can implement. It should not contain concrete engine logic, database behavior, or content-specific policy.

## What Belongs Here
- Interface contracts for reusable AI definitions
- Interface contracts for group AI templates, live groups, group types, group emotes, and group data
- Small adjacent abstractions that are genuinely cross-project
- Enum surfaces that are part of the public contract

## What Does Not Belong Here
- Concrete AI implementations
- XML save/load logic
- Reflection-based loader registration
- Builder command parsing
- Heartbeat wiring
- Database access
- Content-specific rules for legal, shop, combat, or movement systems

Those belong in `MudSharpCore`.

## Interface-First Rules
- If a new AI capability must be consumed across project boundaries, add or extend the interface here first.
- Prefer extending an existing interface when the new capability is intrinsic to that abstraction.
- Prefer creating a new narrow interface when the capability is optional or specialized.
- Do not leak `MudSharpCore` implementation details into the contract surface.

## Public API Stability
- Treat these interfaces as stable engine contracts.
- Avoid renaming members casually; downstream implementations and builder/runtime code may already depend on them.
- Additive changes are safer than breaking signature changes.
- If a signature must change, update all dependent runtime implementations and document the impact in the matching design document.

## Naming Guidance
- Keep interface names noun-oriented and capability-oriented.
- Use `I...` naming consistently.
- Keep enums descriptive and builder-friendly.
- Prefer names that describe behavior rather than one specific content pack's usage.

## Designing New AI Contracts
- Keep the contract focused on engine-facing behavior, not builder UX.
- Expose only the minimum information the runtime actually needs.
- Do not add members solely to mirror one current implementation's private convenience state.
- If a concept is specific to only one AI family, prefer a specialized derived interface over bloating `IArtificialIntelligence`.

## Group AI Contract Guidance
- Put reusable group abstractions here: group instance, template, type, emote, type data.
- Keep the split between template and live-instance concepts clear.
- Preserve the distinction between shared configuration and live mutable state.
- If adding new group-wide state, prefer placing it behind `IGroupTypeData` unless it truly belongs on every group instance.

## Minimal Checklist for Contract Changes
- Confirm the change genuinely belongs in the shared library.
- Update or add the relevant interface or enum here.
- Check whether `IFuturemud` needs new registry exposure.
- Check whether `MudSharpCore` implementations and loaders must be updated.
- Check whether the design docs covering AI or events must be updated.
- Check whether the contract change affects builder help, persistence, or event payload expectations downstream.

## Event Relationship
AI contracts here depend on the event abstractions in `FutureMUDLibrary/Events`.

When adding AI members that imply new event behavior:

- prefer reusing existing `EventType` members where possible
- only expand event contracts when the runtime behavior cannot be expressed cleanly otherwise
- keep event-specific implementation details out of these interfaces

## Notes
- This folder defines the shape of the AI subsystem, not its gameplay behavior.
- Concrete authoring guidance for implementing AI belongs in `MudSharpCore/NPC/AI/AGENTS.md`.
- Event-contract guidance belongs in `FutureMUDLibrary/Events/AGENTS.md`.
