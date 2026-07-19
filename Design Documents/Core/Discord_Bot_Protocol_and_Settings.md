# Discord Bot Protocol and Settings

## Scope

`DiscordBotCore` connects the engine's TCP notification protocol to Discord and persists the bot's local settings and account links. The bot must tolerate normal TCP fragmentation, reject unauthenticated commands, and preserve account identifiers without culture-dependent formatting.

## TCP framing

Inbound engine messages are UTF-16 payloads terminated by byte value `1`. `DiscordTcpFrameDecoder` owns framing and supports:

- a frame split across multiple network reads;
- multiple complete frames in one read;
- empty frames without dispatch;
- a maximum payload of 64 KiB; oversized frames close the connection without a delay or unbounded buffer growth.

Outbound bot-to-engine commands continue to use UTF-16 with the existing newline delimiter. Embedded CRLF, CR, and LF characters are escaped as `\n` before transmission.

## Authentication and command routing

`login` is the only inbound command accepted before authentication. Its presented secret is compared with the configured `ServerAuth` using an exact, case-sensitive, fixed-time comparison. Empty secrets do not authenticate.

Every recognized command other than `login` is marked as requiring authentication by `DiscordTcpCommandRouter`. Unknown and empty commands are rejected before dispatch. Authentication success and failure retain the existing `authsuccess` and `authfailure` response commands.

## Settings and account links

`settings.json` must deserialize to a non-null settings object with a positive port, non-empty token, server authentication secret, and game name, plus non-null prefix, administrator, and custom-reaction collections. Malformed or incomplete JSON is reported and is not partially applied.

`accountlinks.data` remains a line-oriented comma-separated compatibility format:

```text
<discord-user-id>,<mud-account-name>,<mud-account-id>
```

Numeric identifiers load and save with invariant culture. A malformed account-link line is skipped and counted; it no longer prevents later valid links from loading. Saving retains the established three-field format.

## Verification contract

The normal Discord unit suite covers fragmented and multiple frames, both size-limit paths, Unicode payloads, exact successful and failed authentication, unauthenticated command classification, unknown commands, shutdown modes, valid and malformed main settings, invariant account-link round trips, malformed account-link recovery, and concurrent request-id uniqueness. The tests use callable internal seams and do not require Discord, network delays, or a running engine.
