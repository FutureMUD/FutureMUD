# Discord Remote Operations

## Purpose and deployment

The Discord integration provides public published information, private linked-account self-service, staff operations, economy analytics and world-authored Discord Hooks. Callers do not need to control a character. The [expansion review](./Discord_Bot_Expansion_Review.md) inventories the 22 existing commands and 44 additional actions.

Deploy the updated bot before the updated engine. The bot accepts legacy engine notifications as well as version-two frames; the expanded operations require a version-two engine. A legacy bot cannot read the new engine framing. Existing prefix commands remain available, with sensitive results delivered privately.

Apply the Discord database migration through the normal engine upgrade process. It creates DiscordAccountLinks, DiscordHooks, DiscordIntegrationSettings and DiscordRemoteInvocations. Definitions, identity links and operation outcomes belong to the game database. The bot's accountlinks.data is a compatibility cache, not the authority for new operations.

Configure the Discord application with the bot and application-command installation permissions. The bot registers grouped commands under game, account, admin, economy and hooks. Engine authority remains the permission boundary; a Discord guild role alone grants no game privileges.

Bot settings include explicit installation-owner Discord IDs for bot administration. Protect the bot token and shared bridge secret. Bind the listener to loopback for same-host deployment; explicitly select a private interface/protected transport when deploying across hosts. Existing engine connection/authentication settings remain implementor-restricted.

## Account ownership and privacy

Use the existing prefix register command while an account character is connected. The account holder must accept the in-game proposal. Acceptance persists the authoritative engine binding before the bot acknowledges it. Existing bot-only links must repeat ownership proof to acquire an engine binding; knowing an account name is insufficient.

There is one Discord user per linked account and one linked account per Discord user in a game database. Unlink before changing accounts. Suspended accounts cannot use linked remote operations.

Account slash commands show link status, resource balances, application status, current draft options and notification preferences. They scope data to the verified account. Draft options use read-only stage providers against the actual owned application; they never instantiate a selection screen or advance the draft. Free-text stages and non-persisted intermediate screen progress require continuing in game.

Application decisions and staff messages can be delivered privately when the account opts in with account notifications. Delivery failure never falls back to a public channel. Unlinking revokes pending account confirmations and future account notifications.

Slash results are ephemeral. Sensitive prefix responses use direct messages and fail closed when DMs cannot be delivered. Output is bounded and mentions are neutralised. Do not publish account notes, applications, locations or detailed economy custody to general channels.

## Publishing public information

HighAdmin staff can publish information using the in-game discordconfig command. Mapping values are JSON objects; quote characters in those objects are significant.

```text
discordconfig set game.info Connect to mud.example.org:4000. Rules: https://example.org/rules
discordconfig set game.places {"harbour":123,"capital":456}
discordconfig set game.calendars {"civil":1}
discordconfig set game.chargenguide {"overview":"Getting Started","roles":"Character Roles"}
discordconfig show game.places
```

Place aliases identify explicitly published rooms for neutral clock/weather reports. They do not disclose a caller character's location, body temperature or knowledge-dependent forecast. Calendar aliases publish selected calendars. Chargen guidance maps topics to public helpfiles; it does not publish the administrative race/culture catalogue or substitute for actual draft eligibility.

Optional security.guilds and security.channels settings restrict remote calls to comma/space-separated Discord IDs. The value none clears a restriction. With a restriction set, a DM or other context whose IDs do not match is denied too.

## Staff and economy actions

The additional staff actions provide application queues, bounded account search, staff-note read/add, resource award/deduction, account ban/unban, explicit maintenance settings, connections, performance/census reports, resource audits, action history and bot-access revocation.

Guide can inspect the permitted application queue. Account search/notes/resources start at JuniorAdmin, with resource-specific award rules retained. Ban/unban/maintenance require SeniorAdmin; connection reports require HighAdmin. Bot-access revocation additionally requires installation-owner policy and engine Founder authority. The caller cannot revoke themselves.

Mutating actions use a short-lived confirmation tied to the exact caller, arguments and current target state. Resource changes retain award limits/time checks, balance updates, notes and notifications. Account bans retain self/hierarchy checks, suspension, notes and active-session closure. Maintenance requests set an explicit flag/value rather than blindly toggling it.

Economy commands use IEconomyAnalyticsService directly. Reports/manual snapshots require Admin; configuration changes require HighAdmin. Reports cover summary, aggregate money, detailed holdings, transaction volume, PC income, employment, trends, wealth, risks and snapshot status. Mutations cover manual capture and validated snapshot configuration.

Use real or MUD-time windows with the appropriate economic zone/period. Report coverage begins at deployment; missing historical data is not zero activity. Global totals use configured currency conversion, and shared control is not necessarily an individual's spendable wealth. Detailed live holdings can include offline data and should be requested deliberately.

## Discord Hooks

A Discord Hook is an explicitly published trusted FutureProg action. It is independent of the perceivable/event hook system: no character, room or item attachment is required.

SeniorAdmin staff create and maintain definitions in game:

```text
discordhook new storm-warning <prog>
discordhook edit storm-warning
discordhook set description Issue the configured regional storm warning.
discordhook set category Events
discordhook set usage <region> <message>
discordhook set authority Admin
discordhook set cooldown 60s
discordhook set confirm on
discordhook set guilds 123456789012345678
discordhook set enabled on
discordhook show storm-warning
```

New hooks are disabled, require confirmation, and default to SeniorAdmin with a 60-second cooldown. Keys use lowercase letters, digits, hyphen or underscore. Optional accounts, guilds and channels restrictions accept IDs or none. Maintainers can inspect/disable restricted definitions, but execution still respects their required authority and context. Editing a definition increments its revision and invalidates outstanding confirmations.

The prog must return Text with exactly these parameters, in order:

| Type | Suggested parameter name | Supplied by |
| --- | --- | --- |
| Number | requesterAccountId | Engine's verified account |
| Text | discordUserId | Authenticated Discord invoker |
| Text | invocationId | Durable unique operation ID |
| Text | arguments | User-authored free text |

There is no Account or DiscordContext FutureProg type. Snowflakes remain text; do not convert Discord IDs through floating-point values. The engine validates compilation/signature and uses status-bearing uncached execution, so a fully-static result cannot suppress repeated intentional invocations.

Discord users discover and execute actions through hooks list/show/run. History lists permitted invocations; result retrieves one by its invocation ID. Enabled provides an emergency availability switch with higher administrative permission. Disabled/broken hooks cannot execute.

Suitable authored actions include event control, setting-specific reports, approved variable changes, and existing maintenance/restock progs. Native account/economy actions should use their canonical operations. A read-only description does not sandbox a prog: publish only trusted bounded code.

## Execution, recovery and retention

Each request has a globally unique ID. The engine records invoking before a mutation and records its outcome afterward. Repeated delivery of the same ID returns the saved state/result; conflicting reuse is rejected. Account authority, suspension, link, context and action revision are rechecked when confirming.

Confirmations expire after two minutes. Cooldowns and per-user request limits prevent repeated admission; the engine handles bounded batches on its game loop. Prog execution remains on the owning engine thread.

A network/Discord timeout does not cancel a running prog. A failed prog can leave partial effects, and there is no general rollback. Interrupted invoking records become indeterminate on recovery and are never automatically replayed. Inspect the result and world state before deliberately starting a replacement action.

Detailed request/result text expires after 30 days; minimal IDs/fingerprints/status attribution remain as replay-prevention tombstones. This retention must not delete the deduplication key for a completed mutation. Other callers' hook history/results require explicit audit authority and current definition visibility.

The wire uses FMD2, a big-endian payload length and UTF-8. Frames are limited to 64 KiB in both directions, including encoded remote envelopes. Length limits apply after JSON/base64 encoding. Incoming fragments/coalesced frames and complete writes are handled explicitly.

## Verification boundaries

Automated tests cover protocol framing, Unicode/size handling, remote policies, confirmation/replay/recovery and read-only eligibility alongside bot response handling. Migration/model/snapshot checks are separate from live database verification. A compiled bot or passing unit tests do not establish live Discord application registration, permissions or delivery; those require an explicit deployment acceptance run.
