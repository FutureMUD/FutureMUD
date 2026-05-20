# AI Storyteller Design Document

## Purpose

The AI Storyteller subsystem provides admin-configurable autonomous narrative agents for FutureMUD.  
It is an engine capability for RPI world management, not a player-facing chat feature.

## Current Scope (Implemented)

- Admin-only creation, configuration, and operation controls.
- OpenAI Responses API integration (`OpenAI.Responses.ResponsesClient`).
- Event-driven triggers from:
	- room echo events (`OnRoomEmoteEcho`) based on surveillance scope
	- character speech events in surveilled rooms (covers SAY/TELL and speech tokens in emotes)
	- crime creation events in surveilled rooms
	- character state transitions (unconscious, pass out, dead) in surveilled rooms
	- fuzzy heartbeats (5m, 10m, 30m, 1h)
	- direct invocation via FutureProg function
- Persistent storyteller state:
	- situations
	- character memories
	- custom tool definitions
	- prompting/subscription configuration
- Built-in and custom tool-calling, with custom tools executing FutureProg.
- Reference document knowledge base with global-or-restricted visibility rules.

## Implemented Components

### Contracts and Runtime Types

- Interfaces in `FutureMUDLibrary/RPG/AIStorytellers`:
	- `IAIStoryteller`
	- `IAIStorytellerSurveillanceStrategy`
	- `IAIStorytellerSituation`
	- `IAIStorytellerCharacterMemory`
	- `IAIStorytellerReferenceDocument`
- Runtime classes in `MudSharpCore/RPG/AIStorytellers`:
	- `AIStoryteller`
	- `AIStorytellerSurveillanceStrategy`
	- `AIStorytellerSituation`
	- `AIStorytellerCharacterMemory`
	- `AIStorytellerReferenceDocument`

### Persistence

- EF models in `MudsharpDatabaseLibrary/Models`:
	- `AIStoryteller`
	- `AIStorytellerSituation`
	- `AIStorytellerCharacterMemory`
	- `AIStorytellerReferenceDocument`
- Migration present: `MudsharpDatabaseLibrary/Migrations/20260211095519_AIStorytellers.cs`.
- `AIStoryteller.Save()` persists:
	- name/description/model
	- system + attention prompts
	- reasoning effort as numeric enum string
	- surveillance strategy XML
	- heartbeat subscription flags
	- event subscription flags (room/speech/crime/state)
	- heartbeat status prog ids
	- paused state
	- custom tool XML (including optional custom player info prog id)
- Situation and memory classes persist lifecycle updates; memory forget deletes db row.
- Reference documents persist metadata/content and storyteller restrictions.

### Boot/Registry Integration

- `IFuturemud` and runtime gameworld include:
	- `AIStorytellers`
	- `AIStorytellerReferenceDocuments`
- `LoadAIStorytellers` is implemented in `MudSharpCore/Framework/FuturemudLoaders.cs` and loads:
	- reference documents
	- storytellers
	- situations
	- character memories
- Loaded situations/memories are linked back to owner storyteller instances.
- Loaded storytellers auto-subscribe to configured heartbeat/room events.

### Admin Command Surface

- Command module exists: `MudSharpCore/Commands/Modules/AIStorytellerModule.cs`.
- Commands:
	- `aistoryteller` / `ais`
	- `aistorytellerreference` / `aisr`
- Permission level: `SeniorAdmin`.
- Generic building helpers implemented in `MudSharpCore/Commands/Helpers/EditableItemHelperAIStorytellers.cs`.
- `AIStoryteller.BuildingCommand` supports editing:
	- identity, model, prompts, reasoning
	- pause/unpause
	- subscriptions (room/speech/crime/state + heartbeat) and heartbeat status progs
	- custom player info prog
	- surveillance strategy
	- custom tools (add/remove/description/parameter/prog/echo-only toggle)
	- reference document search (`refsearch`)
- `AIStorytellerReferenceDocument.BuildingCommand` supports metadata/content/restriction edits.

## Runtime Flow

### Room Echo Path

1. Storyteller receives room echo in surveilled cells.
2. If paused, exit immediately.
3. If `GPT_Secret_Key` missing, exit immediately.
4. Echo text is classified by an attention call using `AttentionAgentPrompt` and minimal reasoning.
5. Attention output is contract-enforced as strict JSON (`Decision` = `interested` or `ignore`, optional `Reason`), and invalid outputs are logged then ignored.
6. Main model call is assembled with:
	- unresolved situation title list
	- location context
	- optional source character context
	- sanitized echo text
	- attention reason
7. Tool loop executes until completion or safety limits are hit.

### Character Speech Path (Surveilled Room)

1. A character speech event fires (`HandleSpeechEvents`), including SAY/TELL and speech embedded in emotes.
2. Storytellers with `SubscribeToSpeechEvents` enabled are filtered by surveillance coverage for the speaker location.
3. If paused or API key is missing, the trigger exits.
4. A direct storyteller prompt is assembled with speaker/target/language/accent/message context.
5. Main model call enters tool loop (echo-only tools available).

### Character Crime Path (Surveilled Room)

1. A crime is created in legal processing (`LegalAuthority.CheckPossibleCrime`).
2. Storytellers with `SubscribeToCrimeEvents` enabled are filtered by surveillance coverage for crime location.
3. If paused or API key is missing, the trigger exits.
4. Crime metadata (type, authority, criminal/victim context, witness count, timestamps) is passed to the storyteller.
5. Main model call enters tool loop (non-echo tool set).

### Character State Path (Surveilled Room)

1. Character state transitions occur in health processing:
	- unconscious
	- pass out
	- dead
2. Storytellers with `SubscribeToStateEvents` enabled are filtered by surveillance coverage for location.
3. If paused or API key is missing, the trigger exits.
4. State-change context is passed directly to the storyteller.
5. Main model call enters tool loop (non-echo tool set).

### Direct FutureProg Invocation Path

1. Builder-authored FutureProg calls `InvokeStorytellerAttention` / `AIStorytellerAttention` (by id or name).
2. Runtime resolves the storyteller and calls `IAIStoryteller.InvokeDirectAttention`.
3. If paused, target not found, or API key is missing, invocation returns false.
4. Otherwise, direct attention text is passed to the storyteller and tool loop executes.

### Heartbeat Path

1. Configured heartbeat event fires.
2. If paused, exit immediately.
3. If `GPT_Secret_Key` missing, exit immediately.
4. Heartbeat-specific context is assembled, including optional status prog output.
5. Unresolved situation title list is appended.
6. Main model call enters tool loop (without echo-only tools).

Pause semantics are a master off-switch for room, speech, crime, state, heartbeat, and direct FutureProg triggers.

## Tool Execution Model

- Response options include built-in tools + valid custom tools.
- Model outputs are processed in a synchronous continuation loop.
- Each function call output is fed back as `FunctionCallOutput` items before next round.
- Loop safety controls:
	- max tool-call depth: 16 rounds
	- max wall-clock duration per trigger: 30 seconds
	- malformed JSON retry budget: 3 retries with corrective feedback message
- Tool outputs are always structured JSON envelopes:
	- success: `{ "ok": true, "result": ... }`
	- error: `{ "ok": false, "error": ... }`

## Built-In Tools (Current Surface)

- `Noop`
- `CreateSituation`
- `UpdateSituation`
- `ResolveSituation`
- `ShowSituation`
- `OnlinePlayers`
- `PlayerInformation`
- `CreateMemory`
- `UpdateMemory`
- `ForgetMemory`
- `Landmarks`
- `ShowLandmark`
- `SearchReferenceDocuments`
- `ShowReferenceDocument`
- `PathBetweenRooms`
- `PathFromCharacterToRoom`
- `PathBetweenCharacters`
- `RecentCharacterPlans`
- `CharacterPlans`

Situation policy:

- unresolved situation titles are injected in prompts
- detailed situation text is retrieved explicitly with `ShowSituation`

## Custom Tool Contract

- Custom tools are persisted as XML with:
	- name
	- description
	- target FutureProg id
	- parameter description metadata
	- echo-only availability flag
- Runtime validates:
	- prog exists
	- prog has no compile error
	- required named parameters are provided
- Dispatch executes via `ExecuteWithRecursionProtection`.
- Argument conversion supports:
	- primitives (`Text`, `Number`, `Boolean`, `Gender`, `DateTime`, `TimeSpan`, `MudDateTime`)
	- collections
	- broad object-id resolution for many `ProgVariableTypes`
	- structured resolution for `Effect`, `Outfit`, and `OutfitItem` values

## Reference Documents

- Reference documents include:
	- name, description, folder, type, keywords, full contents
	- optional restricted storyteller id set
- Visibility rule:
	- empty restriction set => globally visible
	- populated restriction set => only listed storytellers
- Search behavior:
	- multi-term, case-insensitive matching across key metadata and contents
- Legacy XML restriction format is parsed for backward compatibility.

## Prompting and Operational Constraints

- API key source: static configuration `GPT_Secret_Key`.
- Main call reasoning effort: per-storyteller setting.
- Attention call reasoning effort: always minimal.
- Prompt budget guardrail:
	- max prompt length: 24,000 characters (hard truncation marker appended)
- Unresolved situation list guardrail:
	- max 25 titles injected into prompt context
- Errors are logged to debug/console and best-effort relayed to Discord admin notifications.

## Seeder Support

`DatabaseSeeder/Seeders/AIStorytellerSeeder.cs` provides an installable starter pack with:

- one complete sample storyteller
- sample 5m and 1h heartbeat status progs
- one safe sample custom tool prog
- sample prompts
- two sample reference documents (global + restricted)

## Validation Coverage

Current unit tests in `MudSharpCore Unit Tests` cover:

- pause/unpause behavior
- situation and memory lifecycle updates
- surveillance strategy XML roundtrip and effective cell selection
- reference document visibility/search parsing rules
- malformed tool-call JSON handling and retry budget behavior
- built-in tool handlers (including memory and path tools)
- custom tool conversion/error handling (including `Effect`, `Outfit`, and `OutfitItem`)
- attention classifier contract parsing and contract-violation logging behavior
- missing API key behavior for heartbeat/echo trigger entrypoints

## Remaining Gaps

- Test coverage is strong at unit level but does not include live end-to-end model integration tests.
