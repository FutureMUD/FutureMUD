# OpenAI Integration

## Purpose

FutureMUD uses OpenAI for builder description suggestions, configured GPT threads, FutureProg GPT requests, and AI Storytellers. The API key is read from the `GPT_Secret_Key` static configuration and must never be written to command output, Discord alerts, or request logs.

## API and SDK

The engine uses the official `OpenAI` .NET SDK and the Responses API for text generation. Builder description requests use low reasoning effort for a balance of latency and output quality. `GPT_DescSuggestion_Model` selects their model and defaults to `gpt-5.6-terra`; existing games can override that value. AI Storytellers also use Responses, with their own configured models and reasoning levels.

The old `Microsoft.Extensions.AI` and `Microsoft.Extensions.AI.OpenAI` package references were unused and are intentionally absent. Keeping only the directly used official SDK avoids an unnecessary version constraint between the adapter and `OpenAI` packages.

## Request Lifecycle and Diagnostics

OpenAI requests run away from the main game loop and have a 120-second cancellation timeout. Every request receives a unique `X-Client-Request-Id`. Console diagnostics record that client reference and, when OpenAI returns a response, its `x-request-id`. These references allow an administrator to correlate a builder-visible failure with the full server log and with OpenAI support without logging the API key.

All exceptions must be caught inside the asynchronous request body. A catch around only the call that schedules background work does not observe later network or API failures. The shared handler therefore routes completion to the success callback and all exceptions, including timeouts and TLS failures, to the failure path.

Builder commands receive a concise failure message and correlation reference. Full exception details go to the server console, while Discord receives a sanitised alert containing only exception type and request references. Network/TLS failures are distinguished from HTTP responses; HTTP failures include the status and OpenAI request ID when one exists.

## Builder Description Suggestions

The following commands share the same OpenAI request and diagnostics path:

- `cell set suggestdesc [<optional extra context>]`
- `item set suggestdesc [<optional extra context>]`
- `itemskin set suggestdesc [<optional extra context>]`

They queue a request immediately and later return either suggestions or a correlated failure. Suggestions are not applied automatically. The builder must use the command's `accept desc <n>` flow to select one.

## Operational Troubleshooting

If a request reports that it could not establish a secure network connection, inspect the full exception in the server console. On Windows, a process launched in a restricted or sandboxed security context can fail TLS setup with `SEC_E_NO_CREDENTIALS` before OpenAI receives the request. Run the MUD under its normal service/user identity with access to Windows TLS credentials; changing the API key or model will not repair that transport failure.

If OpenAI returns an HTTP error, use the client request reference and logged OpenAI request ID to distinguish authentication, model access, rate-limit, and request-validation failures.
