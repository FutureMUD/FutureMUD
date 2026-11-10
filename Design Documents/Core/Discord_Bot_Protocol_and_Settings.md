# Discord Bot Protocol and Settings

## Scope

`DiscordBotCore` connects the engine's TCP notification protocol to Discord and persists the bot's local settings and account links. The bot must tolerate normal TCP fragmentation, reject unauthenticated commands, and preserve account identifiers without culture-dependent formatting.

## TCP framing

Version-two traffic in both directions uses the shared `DiscordBridgeFrameDecoder`: ASCII `FMD2`, a four-byte big-endian payload length, then UTF-8 bytes. The 64 KiB limit applies to the encoded payload, including remote JSON/base64 envelopes. Raw delimiter bytes cannot safely frame arbitrary UTF-16 text.

- a frame split across multiple network reads;
- multiple complete frames in one read;
- empty frames without dispatch;
- a maximum payload of 64 KiB; oversized frames close the connection without a delay or unbounded buffer growth.

The updated bot can detect legacy engine notification framing (UTF-16 terminated by byte 1) for deployment compatibility. Legacy replies retain newline-delimited UTF-16 and escape line breaks. Expanded remote operations require version two; deploy the updated bot before the engine. Do not mix protocols within a connection.

Both endpoints handle complete writes and bounded input; the engine admits at most eight received commands per game-loop pass. Idle peer closure and failed authentication release the connection.

## Authentication and command routing

`login` is the only inbound command accepted before authentication. Its presented secret is compared with the configured `ServerAuth` using an exact, case-sensitive, fixed-time comparison. Empty secrets do not authenticate.

The engine's `DiscordAuthToken`, `DiscordBotIpAddress`, and `DiscordBotPort` static configurations require implementor permission to view or edit in game. Restricting the destination prevents another administrator from redirecting the authentication secret to a different service. Other administrators see setting names with a red `Redacted for Security` placeholder. See [Static Configuration Security](./Static_Configuration_Security.md).

Every recognized command other than `login` is marked as requiring authentication by `DiscordTcpCommandRouter`. Unknown and empty commands are rejected before dispatch. Authentication success and failure retain the existing `authsuccess` and `authfailure` response commands.

Remote requests carry a unique operation ID and Discord invoker/context. The engine resolves a persisted account binding and rechecks current authority, suspension and context. Sensitive compatibility commands also require a verified account at the engine boundary. Public named help uses the public route; the unauthorised where path returns immediately.

See [Discord Remote Operations](./Discord_Remote_Operations.md) for the added actions, public publication settings, identity migration, confirmations, durable outcomes and Discord Hook authoring.

Discord `showaccount` and `showcharacter` requests carry the linked MUD account ID, but the engine remains the authorization boundary: the requester must currently resolve to at least `JuniorAdmin`. Malformed request, requester, or character identifiers are rejected without throwing through the game loop. Account-detail rendering occurs while its `FMDB` scope is alive because the formatter reads account navigation data and related character rows.

## Settings and account links

`settings.json` must deserialize to a non-null settings object with a positive port, non-empty token, server authentication secret, and game name, plus non-null prefix, administrator, and custom-reaction collections. Malformed or incomplete JSON is reported and is not partially applied.

`accountlinks.data` remains a line-oriented comma-separated compatibility format:

```text
<discord-user-id>,<mud-account-name>,<mud-account-id>
```

Numeric identifiers load and save with invariant culture. A malformed account-link line is skipped and counted; it no longer prevents later valid links from loading. Saving retains the established three-field format.

## Verification contract

The normal Discord unit suite covers fragmented and multiple frames, both size-limit paths, Unicode payloads, exact successful and failed authentication, unauthenticated command classification, unknown commands, shutdown modes, valid and malformed main settings, invariant account-link round trips, malformed account-link recovery, and concurrent request-id uniqueness. The tests use callable internal seams and do not require Discord, network delays, or a running engine.
