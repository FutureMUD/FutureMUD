# AI Storyteller Design Document

## Purpose

The AI Storyteller system provides admin-configurable autonomous agents that can observe game events, reason about relevance, and take in-game actions through controlled tool calls.

The design target is an engine-level capability for RPI narrative management, not a player-facing chat feature.

## Scope

- Admin-only creation and configuration.
- OpenAI Responses API as the supported LLM provider.
- Event-driven operation via room echoes and heartbeat timers.
- Persistent memory and situation tracking per storyteller.
- Built-in and custom tools, where custom tools invoke FutureProg.

## Current Implementation Snapshot

### Implemented Foundation

- Interfaces exist in `FutureMUDLibrary/RPG/AIStorytellers`:
	- `IAIStoryteller`
	- `IAIStorytellerSurveillanceStrategy`
	- `IAIStorytellerSituation`
	- `IAIStorytellerCharacterMemory`
	- `IAIStorytellerReferenceDocument`
- Runtime classes exist in `MudSharpCore/RPG/AIStorytellers`:
	- `AIStoryteller`
	- `AIStorytellerSurveillanceStrategy`
	- `AIStorytellerSituation`
	- `AIStorytellerCharacterMemory`
- Database models exist in `MudsharpDatabaseLibrary/Models`:
	- `AIStoryteller`
	- `AIStorytellerSituation`
	- `AIStorytellerCharacterMemory`
	- `AIStorytellerReferenceDocument`
- Gameworld registries and add/destroy paths are present:
	- `IFuturemud.AIStorytellers`
	- `IFuturemud.AIStorytellerReferenceDocuments`

### Incomplete or Missing Integration

- Boot-time loader path is declared but not implemented:
	- `MudSharpCore/Framework/FuturemudLoaders.cs` (`LoadAIStorytellers` contains TODO placeholders).
- No concrete `AIStorytellerReferenceDocument` runtime class is present.
- No admin command module currently exposes create/edit/list/delete for AI Storytellers.
- `AIStoryteller` OLC/building implementation is only stubbed (single `name` branch with `NotImplementedException`).

### Runtime Alignment Notes

- Custom tools are registered in response options, but custom function-name dispatch is not currently implemented in the tool handler switch.
- Tool execution uses a non-blocking async pattern that does not currently enforce a synchronous "tool-call, output, continue" loop.
- `ForgetMemory` has a runtime handler but is not currently declared in built-in tool definitions.
- `Landmarks` and `ShowLandmark` currently assemble payload text but do not return function output items to the model.
- `AIStoryteller.Save()` currently persists prompts/subscriptions/tool definitions, but not all runtime-configurable fields are written back.
- heartbeat path does not currently check `IsPaused`, but pause is intended to suppress all triggers.

## Core Concepts

### AI Storyteller

An `IAIStoryteller` is a `SaveableItem` + `IEditableItem` that owns:

- Identity and description.
- LLM config: model, system prompt, attention prompt, reasoning effort.
- Event subscriptions:
	- room echoes (via surveilled cells)
	- 5m / 10m / 30m / 1h fuzzy heartbeats
- Optional heartbeat status progs per heartbeat interval.
- Collections of situations and character memories.
- Custom tool definitions (always-available and echo-only sets).
- Paused/unpaused state.

### Surveillance Strategy

`AIStorytellerSurveillanceStrategy` selects surveilled cells from:

- Included zones (all cells in each zone).
- Explicitly included cells.
- Explicitly excluded cells.

Definition is persisted as XML and supports OLC-style toggles for zone/include/exclude.

### Situation

A situation is persistent, storyteller-owned state describing an ongoing event.

- Fields: title (`Name`), text, created timestamp, resolved flag.
- Lifecycle:
	- create
	- update
	- resolve

### Character Memory

A character memory is persistent, storyteller-owned state keyed to a character.

- Fields: title, memory text, created timestamp, character reference.
- Lifecycle:
	- create
	- update
	- forget (delete)

### Reference Document

`IAIStorytellerReferenceDocument` is a `SaveableItem` + `IEditableItem` that defines an admin-editable knowledge document with:

- folder name
- keywords
- document contents
- text search predicate (`ReturnForSearch`)
- optional storyteller restriction list

Reference-document scope behavior:

- if the restriction list is empty, the document is globally available to all storytellers
- if the restriction list is populated, the document is available only to listed storytellers

This capability is part of the system contract and data model but currently lacks a runtime implementation path.

## Runtime Flow

### Building Tools

- There should be an AIStorytellerModule in `MudSharpCore.Commands.Modules` that should be added to SeniorAdmin level command trees and up.
- The building commands should be `AIStoryteller` (implemented alternative alias `ais`) and `AIStorytellerReference` (implemented alternative alias `aisr`)
- These building commands should use the generic building module's commands with an EditableItemHelper for each
- Custom Tools, Memories and Situations should be edited via the AIStoryteller building command, operating on the opened AIStoryteller

### Room Echo Path

1. Storyteller subscribes to `OnRoomEmoteEcho` for surveilled cells.
2. Incoming echo is parsed for plain text.
3. Attention agent prompt is used to classify relevance.
4. If attention output begins with `interested`, the main storyteller agent is invoked.
5. Main agent receives structured context:
	- location name
	- optional source character context
	- stripped echo text
	- attention reason
6. Agent acts through tool calls.

### Heartbeat Path

Paused behavior for heartbeat flow:

- if `IsPaused` is true, heartbeat triggers are ignored
- pause is a master off-switch and applies to all trigger sources

1. Storyteller subscribes to selected heartbeat events.
2. On trigger, a heartbeat event message is assembled.
3. Optional status prog output for that heartbeat is appended.
4. Main storyteller agent is invoked.
5. Agent acts through tool calls.

### Tool Execution Model

The intended interaction pattern is:

1. Build response options with system/user messages.
2. Inject unresolved situation titles into main-agent context.
3. Register built-in tools.
4. Register custom tools (optionally including echo-only tools).
5. Execute function calls.
6. Feed function outputs back to model until no further tool action is requested.

The system is explicitly action-oriented; side effects come from tool execution.

Situation-context policy:

- unresolved situation titles are always included in main-agent context
- detailed situation text requires explicit retrieval by tool call

## Built-In Tool Contract

Built-in tool surface currently defined in runtime includes:

- `CreateSituation`
- `UpdateSituation`
- `ResolveSituation`
- `Noop`
- `OnlinePlayers`
- `PlayerInformation`
- `CreateMemory`
- `UpdateMemory`
- `Landmarks`
- `ShowLandmark`

Runtime handler logic also includes `ForgetMemory` behavior and should expose it as a declared tool.

All built-in tools should return structured, valid JSON payloads for reliable follow-up tool reasoning.

Situation-detail retrieval should be available via explicit tool call (ID-based), rather than automatic detail injection in every prompt.

## Custom Tool Contract

Each custom tool definition contains:

- tool name
- description
- FutureProg reference
- per-parameter descriptions
- availability scope:
	- always available
	- echo-triggered only

Custom tool parameter schemas are generated from FutureProg named parameters.

Type coverage policy:

- custom tools use `ProgVariableTypes` mappings for invocation shape
- runtime target is eventual full type coverage, consistent with semantics of ProgExecute function in ProgModule.
- non-primitive values in JSON are represented as references and resolved to engine objects at dispatch time (for example, character ID to `Gameworld.TryGetCharacter`)

For complete behavior, custom tool function calls must execute their mapped progs and return structured outputs to the model loop.

## Persistence Contract

### AIStoryteller

Persistent fields include:

- name, description
- model
- system prompt
- attention prompt
- surveillance strategy definition (XML)
- reasoning effort (numeric enum value)
- custom tool calls definition (XML)
- subscription toggles
- heartbeat status prog IDs
- paused flag

### AIStorytellerSituation

- storyteller ID
- name/title
- situation text
- created UTC timestamp
- resolved flag

### AIStorytellerCharacterMemory

- storyteller ID
- character ID
- memory title
- memory text
- created UTC timestamp

### AIStorytellerReferenceDocument

- name, description
- folder name
- document type
- keywords
- document contents
- optional restricted storyteller IDs collection

## Prompting and Operational Constraints

- OpenAI key is taken from static config `GPT_Secret_Key`.
- If key is absent, storyteller processing exits without action.
- Attention model uses minimal reasoning effort.
- Main model uses per-storyteller configured reasoning effort.
- Runtime errors are logged and admin-notified through Discord integration.
- malformed tool-call JSON should use bounded retry with corrective feedback to the model.

## Admin Configuration Surface

The intended admin surface (OLC command model) must support:

- storyteller lifecycle (create, list, show, delete, pause, unpause)
- model/prompt/reasoning configuration
- heartbeat subscription and status prog configuration
- surveillance strategy editing
- built-in tool visibility and custom tool management
- optional custom player information prog
- reference document management and search behavior

This remains admin-only, with no player-facing command dependency.

## Seeder Expectations

Useful Seeder should provide at least one end-to-end example storyteller pack including:

- prompts
- surveillance scope
- heartbeat configuration
- sample situations/memories behavior
- at least one custom FutureProg tool
- optional reference documents

## Design Outcome

When fully integrated, AI Storytellers become a first-class engine subsystem that can:

- observe live world events at configured scope,
- filter noise through attention gating,
- maintain durable narrative context,
- and take controlled, auditable in-game actions through tool contracts.
