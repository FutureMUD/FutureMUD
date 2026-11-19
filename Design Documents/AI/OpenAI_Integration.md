# OpenAI Integration

## Purpose

FutureMUD uses OpenAI for builder description suggestions, configured GPT threads, FutureProg GPT requests, and AI Storytellers. The API key is read from the `GPT_Secret_Key` static configuration. Only implementors can explicitly view or edit this setting in game; other administrators see a red `Redacted for Security` placeholder. The key must never appear in ordinary request output, Discord alerts, or request logs. See [Static Configuration Security](../Core/Static_Configuration_Security.md).

## API and SDK

The engine uses the official `OpenAI` .NET SDK and the Responses API for text generation. Builder description requests use low reasoning effort for a balance of latency and output quality. `GPT_DescSuggestion_Model` selects their model and defaults to `gpt-5.6-terra`; existing games can override that value. AI Storytellers also use Responses, with their own configured models and reasoning levels.

The old `Microsoft.Extensions.AI` and `Microsoft.Extensions.AI.OpenAI` package references were unused and are intentionally absent. Keeping only the directly used official SDK avoids an unnecessary version constraint between the adapter and `OpenAI` packages.

## Request Lifecycle and Diagnostics

Shared model lookups and text requests run away from the main game loop with a 120-second cancellation timeout. The game loop delivers the result to the command, builder workflow or FutureProg. Every shared-handler request receives a unique `X-Client-Request-Id`. Console diagnostics record that client reference and the provider's `x-request-id` when available.

The shared handler observes HTTP errors, timeouts and transport failures and routes them to its failure path. Incomplete responses and responses with no usable text also fail. Failed requests do not save thread history, set up description suggestions or run a FutureProg success callback. The boolean returned by `GPTRequest(...)` indicates whether the request was queued. A missing key returns `false` immediately.

`gpt create` and `gpt set <thread> model` report that a lookup is in progress. OpenAI must return a model catalogue before the command can open the prompt editor or change the stored model. On failure, `gpt`, `testgpt` and the description suggestion commands report a concise error. When the configured key is missing, the command reports that immediately.

The console records a safe failure summary, exception type and stack trace. Discord alerts also use sanitised diagnostics. Raw exception messages and provider error bodies can contain credential fragments and are omitted. HTTP diagnostics include the status and request references when available.

## Builder Description Suggestions

The following commands share the same OpenAI request and diagnostics path:

- `cell set suggestdesc [<optional extra context>]`
- `item set suggestdesc [<optional extra context>]`
- `itemskin set suggestdesc [<optional extra context>]`

They queue a request immediately and later return either suggestions or a correlated failure. Suggestions are not applied automatically. The builder must use the command's `accept desc <n>` flow to select one.

## Operational Troubleshooting

If a request reports that it could not establish a secure network connection, check the server's network and service identity. On Windows, a process launched in a restricted or sandboxed security context can fail TLS setup with `SEC_E_NO_CREDENTIALS` before OpenAI receives the request. Run the MUD under its normal service/user identity with access to Windows TLS credentials.

For HTTP 401, check `GPT_Secret_Key` and replace the key if it expired or was revoked. For HTTP 403 or 404, check the project's access and configured model. For HTTP 429, check limits and quota. Use the request references when investigating an error with OpenAI support.

## Storyteller runtime

Storytellers use the pinned OpenAI SDK through an injectable protocol adapter. Request preparation and tool execution run on the game loop. Asynchronous network tasks receive immutable JSON only. Scoped model, reasoning and output settings are preserved for the entire run, with a default 120-second cancellation deadline. Completed, refused, incomplete and failed responses are distinguished before tool execution. A provider error finishes the affected run and records safe diagnostics. Pausing or deleting a storyteller cancels active work and discards queued activity.

New stock uses a fast classifier and a separate narrative model. Explicit stable-prefix caching on verified compatible models uses a developer content breakpoint with a 30-minute lifetime; legacy/unknown models retain provider defaults. Raw protocol JSON supports those fields without upgrading the SDK. Full output items, including encrypted reasoning, survive continuations. `ais usage` exposes numerical usage and cache-write/read counters without exposing narrative content. The full builder, migration and opt-in evaluation contract is in [AI Storyteller Design](AI_Storyteller_System.md).
