# AI Storyteller Implementation Plan

## Objective

Complete the AI Storyteller subsystem from partial runtime classes into a fully loadable, configurable, and operational engine feature.

## Phase 1: Implement Reference Documents

1. Add concrete runtime class for `IAIStorytellerReferenceDocument`.
2. Implement all interface members, abstract members and associated Building Commands
3. Create Database Model class and implement save/load routines in the original concrete class
4. Add universal tools to the AI Storyteller to search, browse and retrieve the reference documents

## Phase 2: Runtime Model Completion

1. Ensure fool implementation of `IAIStoryteller` in concrete class.
2. Ensure `AIStoryteller` supports complete read/write persistence for all intended fields.
3. Persist reasoning effort as a numeric enum and align model/runtime conversion.
4. Ensure pause semantics are consistent across echo and heartbeat execution paths.

## Phase 3: Tool Loop Correctness and Safety

1. Finalize the tool-execution loop so function outputs are fed back to the model until complete.
2. Ensure every declared built-in tool has handler coverage, and every handler is declared as a tool.
3. Guarantee all tool outputs are valid JSON payloads.
4. Add safety limits for recursive tool-call depth and execution duration per event trigger.

## Phase 4: Custom Tool Execution

1. Implement runtime dispatch from custom function calls to their mapped FutureProg.
2. Validate argument conversion from JSON into `ProgVariableTypes`, including reference-ID resolution for engine object types.
3. Return structured function results and execution errors back to the model loop.
4. Add guardrails for invalid/compiled-with-error progs and progressively complete full `ProgVariableTypes` coverage.

## Phase 5: Admin Command and OLC Surface

1. Implement admin command entrypoint for storyteller lifecycle management.
2. Complete `AIStoryteller.BuildingCommand` for full configuration editing.
3. Add management commands for custom tools and surveillance strategy editing.
4. Add management commands for builders to search reference documents and search metadata.

## Phase 6: Prompting and Knowledge Context

1. Define and enforce attention-agent output contract (`interested` plus optional reason).
2. Inject unresolved situation titles into each main-agent call; keep detail retrieval tool-driven by situation ID.
3. Implement reference-document visibility rules:
if the restricted storyteller list is empty then globally visible, otherwise restricted to listed storytellers.
4. Define optional custom player information prog behavior and persistence.
5. Add explicit prompt length and context-budget controls.

## Phase 7: Boot and Registry Integration

1. Implement `LoadAIStorytellers` in `MudSharpCore/Framework/FuturemudLoaders.cs`.
2. Load and register reference documents, storytellers, situations, and memories from EF models.
3. Link loaded situations and memories back to their owning storyteller instances.
4. Subscribe loaded storytellers to heartbeats and room events according to configuration.

## Phase 8: Seeder and Example Content

1. Add Useful Seeder templates for at least one complete storyteller setup.
2. Include sample heartbeat status progs and one safe custom tool prog.
3. Include sample reference documents and example attention/system prompts.

## Phase 9: Validation and Hardening

1. Add unit tests for situation/memory lifecycle and surveillance strategy behavior.
2. Add integration tests for echo flow, heartbeat flow, and tool-call side effects.
3. Add tests for pause/unpause, missing API key behavior, and failure logging.
4. Add tests for custom tool type conversion and invalid custom tool definitions.
5. Add retry-and-feedback tests for malformed model tool-call JSON handling.

## Acceptance Criteria

1. Storytellers are loaded at boot and begin configured subscriptions automatically.
2. Admins can fully create and configure storytellers and reference documents in game.
3. Echo and heartbeat triggers reliably execute tool calls with deterministic side effects.
4. Situations and memories persist and reload correctly across restarts.
5. Custom FutureProg tools execute with validated arguments and return structured outputs.
6. Seeder provides a functioning end-to-end example for new installs.

## Settled Product Direction

1. Reference documents default to global scope and become restricted only when a storyteller allowlist is populated.
2. Pause is a master off-switch and suppresses all triggers, including heartbeat and echo paths.
3. Reasoning effort is persisted as a numeric enum.
4. Main-agent calls include situation titles by default; detailed situation text is fetched through tool calls.
5. Custom tools follow `ProgVariableTypes`, with eventual full type coverage and JSON reference-ID resolution for object types.
6. Malformed tool-call JSON handling uses bounded retry with corrective feedback to the model.
