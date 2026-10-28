# AI Storyteller Design Document

## Purpose

The AI Storyteller subsystem provides admin-configurable autonomous narrative agents for FutureMUD.  
It is an engine capability for RPI world management, not a player-facing chat feature.

## Current Scope (Implemented)

- Admin-only creation, configuration, and operation controls.
- OpenAI Responses API integration (`OpenAI.Responses.ResponsesClient`) through the official `OpenAI` .NET SDK.
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
	- reasoning effort with legacy numeric values retained and new levels stored by name
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
4. Routine room activity is collected into fixed windows when batching is enabled, then classified once using the configured attention model and effort.
5. Attention output is contract-enforced as strict JSON (`Decision` = `interested` or `ignore`, nullable `Reason`), and invalid outputs are logged then ignored.
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
4. A prompt is assembled with speaker/target/language/accent/message context, including signing where appropriate.
5. Routine events enter the room batch and attention classifier before an interested batch enters the tool loop (echo-only tools available).

### Character Crime Path (Surveilled Room)

1. A crime is created in legal processing (`LegalAuthority.CheckPossibleCrime`).
2. Storytellers with `SubscribeToCrimeEvents` enabled are filtered by surveillance coverage for crime location.
3. If paused or API key is missing, the trigger exits.
4. Crime metadata (type, authority, criminal/victim context, witness count, timestamps) enters the attention classifier immediately, without routine-event batching.
5. An interested classification enters the tool loop (non-echo tool set).

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

Pause is a master off-switch: it cancels the active provider request and drops pending work. Deletion does the same. Successful builder configuration commands also discard pending work so old tool definitions cannot execute against changed configuration. Already completed world changes are not rolled back.

## Tool Execution Model

- Response options include built-in tools + valid custom tools.
- `IAIStoryteller.ProcessPendingWork` advances runs on the game loop. An injectable asynchronous adapter sends immutable serialized requests; only the game loop interprets results and reads/mutates world state.
- Each function call output is fed back as `FunctionCallOutput` items before next round.
- Loop safety controls:
	- max tool-call depth: 16 rounds
	- default cancellable deadline per run (classification plus tools): 120 seconds; configurable from 5 to 600
	- maximum local tool calls: 64
	- complete provider status and refusal checks precede all tool execution; incomplete output never becomes an ignore decision
	- duplicate tool call IDs are rejected before executing a batch
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
- Attention, event and time reasoning efforts are independently configurable. Supported levels are `none`, `minimal`, `low`, `medium`, `high`, `xhigh`, and `max`, subject to model compatibility. Legacy numeric values 0–3 retain their meanings.
- Prompt budget guardrail:
	- max prompt length: 24,000 characters (hard truncation marker appended)
- Unresolved situation list guardrail:
	- max 25 titles injected into prompt context
- Errors are logged to debug/console and best-effort relayed to Discord admin notifications. Shared GPT requests also record client and OpenAI request IDs and return correlated failures to interactive callers.

## Seeder Support

`DatabaseSeeder/Seeders/AIStorytellerSeeder/AIStorytellerSeeder.cs` provides an installable starter pack with:

- one complete sample storyteller
- sample 5m and 1h heartbeat status progs
- one safe sample custom tool prog
- sample prompts
- twelve sample reference documents (global/restricted examples plus ten skill guides)

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

- Offline provider fixtures establish protocol and lifecycle behavior. Live model quality, cache-hit rates, account access and monetary savings require the opt-in comparison below.

## Modern request controls

New storytellers and recognized stock upgrades use `gpt-6-luna`/`none` for classification (256 output tokens), `gpt-6.1-terra`/`low` for event work (4,096), and `gpt-6.1-terra`/`medium` for heartbeat work (4,096). These are starting configurations, not measured narrative-quality guarantees. Existing custom storytellers retain their models and authored prompts. Missing budgets default to 4,096 for narrative roles and 2,048 for reasoning classifiers, or 256 when classifier reasoning is `none`.

Builder commands:

- `ais set outputtokens <event|time|attention> <128-128000>` includes reasoning tokens in the output allowance.
- `ais set timeout <5-600>` sets the complete run deadline in seconds.
- `ais set batch <0-60>` sets a fixed routine-event window; zero disables batching.
- `ais set cache <stable|provider>` chooses explicit stable-prefix caching on verified compatible models, or provider-default behavior.
- `ais usage [<id|name>]` shows since-startup requests, batches/events, decisions, token categories, total request latency, retries, incomplete results, cancellations, failures and dropped events.

Options are saved in version 1 `ExecutionOptions` inside the existing custom-tool XML. No database columns are added. Legacy records without this element retain disabled batching. Legacy numeric reasoning values remain readable; newer levels use textual persistence. Arbitrary model IDs remain allowed; known incompatible model/effort combinations are rejected locally without silent fallback.

## Batching and lifecycle

New setups use a five-second window from the first room echo, speech or signing event. Buckets are isolated by storyteller, room and request/tool configuration. Subsequent events do not extend the deadline. Order, source context, timestamp and event kind are retained; shared room context is supplied once. A bucket becomes ready at 20 events or before adding an event would exceed the 24,000-character boundary. A single event too large for that boundary is rejected and counted, not split into misleading fragments.

Crime, state, direct and heartbeat triggers are ready immediately and do not interrupt the active run. At most 25 active/ready/collecting work items are admitted. Waiting work older than 120 seconds is dropped and counted. Direct FutureProg invocation reports unsuccessful queue admission. Pending work is not persisted across restart.

Network cancellation cannot preempt a synchronous FutureProg already executing on the game loop. It prevents subsequent tools and requests. No completed mutation is automatically replayed to recover from a provider failure. Tool definitions, request settings and conversation history remain fixed for each run.

## Context, retrieval and caching

Memory lists and player-information memory fields contain metadata only. `OnlinePlayers`, `ListMemoriesForPlayer`, `PlayerInformation`, `Landmarks`, `SearchReferenceDocuments` and `RecentCharacterPlans` accept nullable `Offset` and `Limit` (default 20, maximum 100). Results include `Total`, `Offset` and nullable `NextOffset`. `ShowMemory` and `ShowReferenceDocument` accept character offsets and a default/maximum 6,000-character chunk. Their results include `TotalCharacters`, `Offset` and nullable `NextOffset`. Authorization/visibility is rechecked by the underlying tool on each request.

Serialized tool output is limited to 8,000 characters. Pages/chunks shrink to fit valid JSON; oversized metadata/custom output returns an explicit error. Custom tools may already have produced world effects before their output is rejected, so they must not be repeated merely to recover a large result. A serialized provider request over 128,000 characters stops the run. These are character-size safeguards, not exact token estimates.

The supported current model profiles use explicit caching with one breakpoint at the end of stable developer instructions and a 30-minute lifetime. Event suffixes and tool results do not acquire breakpoints. Legacy/unknown model IDs use provider defaults. No padding or provider-stored conversations are introduced. All returned response items, including opaque encrypted reasoning, are replayed unchanged through a run. Hosted tool search remains deferred.

Usage reports cache reads and writes separately; both are subsets of input tokens, and reasoning is a subset of output. Narrative payloads remain redacted. Do not interpret absent provider usage as proof of zero billing. Current cache-write charges and retention behavior differ from historical models: see [OpenAI caching](https://developers.openai.com/api/docs/guides/prompt-caching), [reasoning](https://developers.openai.com/api/docs/guides/reasoning), and [conversation state](https://developers.openai.com/api/docs/guides/conversation-state).

## Opt-in live comparison procedure

Offline tests do not make paid requests. For an explicitly authorized live comparison:

1. Use a disposable local world and agree a monetary cap before enabling provider calls. Verify model access and current per-model input, cache-read, cache-write and output rates from official documentation.
2. Prepare the same reviewed event corpus for baseline and modernized configurations: irrelevant chatter, developing conflict, crime, state transitions, heartbeat/no-op, long memory/reference retrieval, and adversarial world text. Use safe recording tools rather than irreversible effects. Have a human label expected attention and acceptable narrative/tool outcomes.
3. Replay isolated events and five-second bursts, measuring attention precision/recall, missed important events, inappropriate/duplicate actions, narrative continuity, latency and queue drops. Keep surveillance, prompts and custom tools equivalent where comparing a single change.
4. Compare cold and warm repeated prefixes inside the 30-minute window; include changed-prefix and expiry cases. Capture provider usage plus `ais usage` deltas. Compare provider-default versus stable caching without prewarming or padding.
5. Calculate cost from uncached input = input minus cache-read minus cache-write, each category at its own verified rate, plus output. Do not add reasoning a second time. Report cost per event and per acceptable action, not cache percentage alone. Stop at the agreed spending cap or a correctness failure.
6. Record configurations, corpus identity, actual requests, provider errors and unexecuted scenarios. Keep existing production settings until the results justify a change. No monetary-saving claim is accepted from mocked fixtures alone.
