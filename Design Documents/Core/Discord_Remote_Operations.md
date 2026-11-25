# Discord Remote Operations

## Purpose and deployment

The Discord integration provides public published information, private linked-account self-service, staff operations, economy analytics and world-authored Discord Hooks. Callers do not need to control a character. The [expansion review](./Discord_Bot_Expansion_Review.md) records the original proposal and the later staff workflows.

Deploy the updated bot before the updated engine. The bot accepts legacy engine notifications as well as version-two frames; the expanded operations require a version-two engine. A legacy bot cannot read the new engine framing. Existing prefix commands remain available, with sensitive results delivered privately.

Run the normal engine database upgrade. The original Discord migration creates `DiscordAccountLinks`, `DiscordHooks`, `DiscordIntegrationSettings` and `DiscordRemoteInvocations`. Migration `20261125040236_AddDiscordEconomyRiskDigest` adds each recipient’s subscription and last observed incidents to their account link. Existing and new links start opted out. The engine owns these records. The bot keeps a local `accountlinks.data` mirror for legacy commands.

Configure the Discord application with the bot and application-command installation permissions. The bot registers `/game`, `/account`, `/admin`, `/justice`, `/npc`, `/storyteller`, `/economy` and `/hooks`. Set the engine’s `UseDiscordBot` configuration to true to enable the bridge. Each privileged request checks the linked MUD account’s current authority.

Bot settings include explicit installation-owner Discord IDs for bot administration. Protect the bot token and shared bridge secret. Bind the listener to loopback for same-host deployment; explicitly select a private interface/protected transport when deploying across hosts. Existing engine connection/authentication settings remain implementor-restricted.

## Account ownership and privacy

Use the existing prefix register command while an account character is connected. The account holder must accept the in-game proposal. Acceptance persists the authoritative engine binding before the bot acknowledges it. Existing bot-only links must repeat ownership proof to acquire an engine binding; knowing an account name is insufficient.

There is one Discord user per linked account and one linked account per Discord user in a game database. Unlink before changing accounts. Suspended accounts cannot use linked remote operations.

Account slash commands show link status, resource balances, application status, current draft options and notification preferences. They scope data to the verified account. Draft options use read-only stage providers against the actual owned application; they never instantiate a selection screen or advance the draft. Free-text stages and non-persisted intermediate screen progress require continuing in game.

Application decisions and staff messages can be delivered privately when the account opts in with account notifications. Delivery failure never falls back to a public channel. Unlinking revokes pending account confirmations and future account notifications.

Slash results are ephemeral. Sensitive prefix responses use direct messages and fail closed when DMs cannot be delivered. Output is bounded and mentions are neutralised. Do not publish account notes, applications, locations or detailed economy custody to general channels.

## Publishing public information

`/game landmarks` lists up to 20 visible room landmarks in room ID order. Use the last ID with `/game landmarks after:<id>` for the next page. Use `/game landmark id:<id>` for the room description, terrain, meeting-place and safe-quit flags, and any visible landmark text.

An active, verified JuniorAdmin or higher sees every landmark and its extra texts. Everyone else, including unlinked users and Guides, sees a landmark when it has no applicability prog or the prog succeeds with true for the room and a null character. Extra text has its own character prog and must also pass for a null character. Both commands apply the same visibility rule. An unknown ID and a hidden ID produce the same reply. Discord has no character sphere or starting room. Room names and descriptions use the fallback branches of trait, writing and signed-language markup.

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

The bot sends an account creation message to its configured admin announcement channel after the MUD saves a new account, including when email verification is enabled. Set `AdminAnnounceChannelId` and `GameName` in the bot’s `settings.json`. For example, staff in that channel would see “A new account called **Morgan** has been created on **ExampleMUD**”.

Justice announcements use the channel set by `legalauthority set discord <channelid>` for each jurisdiction. An accepted `requesttrial` announces the request. An NPC judge's courtroom introduction or a player judge summoning the defendant announces the start. An execution patrol announces its route when the staffed patrol launches, including when a prisoner successfully brings their execution forward. The bot needs permission to view the selected channel and send messages there. Each announcement is best effort. An unavailable channel drops that announcement and the bridge continues processing later events.

The additional staff actions provide application queues, bounded account search, staff-note read/add, resource award/deduction, account ban/unban, explicit maintenance settings, connections, performance/census reports, resource audits, action history and bot-access revocation.

Guide can inspect the permitted application queue. Account search/notes/resources start at JuniorAdmin, with resource-specific award rules retained. Ban/unban/maintenance require SeniorAdmin; connection reports require HighAdmin. Bot-access revocation additionally requires installation-owner policy and engine Founder authority. The caller cannot revoke themselves.

Mutating actions use a short-lived confirmation tied to the exact caller, arguments and current target state. Resource changes retain award limits/time checks, balance updates, notes and notifications. Account bans retain self/hierarchy checks, suspension, notes and active-session closure. Maintenance requests set an explicit flag/value rather than blindly toggling it.

Economy commands use IEconomyAnalyticsService directly. Reports/manual snapshots require Admin; configuration changes require HighAdmin. Reports cover summary, aggregate money, detailed holdings, transaction volume, PC income, employment, trends, wealth, risks and snapshot status. Mutations cover manual capture and validated snapshot configuration.

### Economy risk digest

A linked Admin or above can use `/economy digest enabled:true` to receive private Discord messages. Omit `enabled` to check the subscription or use `enabled:false` to turn it off. Each link starts opted out. Unlinking removes the subscription. The engine checks current account status and authority before dispatch.

On the next connected economy heartbeat, subscribers receive current reserve shortfalls by economic zone and currency and any overdue scheduled snapshot. A change in the size of a shortfall sends no further alert. Restoring coverage sends a clearance and a later shortfall starts a new incident. A scheduled snapshot more than ten minutes late alerts once until the schedule recovers. See `/economy risks` and `/economy snapshotstatus` to investigate.

The engine persists each recipient’s observed incidents before handing the message to the bot. Reconnecting compares current conditions with that record. A successful scheduled capture re-arms the overdue alert even if Discord was offline during recovery. A reserve shortfall that opens and clears entirely between checks can go unseen. If Discord rejects a DM or delivery is interrupted after handoff, that message is not retried.

Use real or MUD-time windows with the appropriate economic zone/period. Report coverage begins at deployment; missing historical data is not zero activity. Global totals use configured currency conversion, and shared control is not necessarily an individual's spendable wealth. Detailed live holdings can include offline data and should be requested deliberately.

## Justice status

A linked JuniorAdmin or above can run `/justice status` to list jurisdictions, then `/justice status jurisdiction:7` or select one by its exact name. The response is private. The report works even when the jurisdiction has no announcement channel.

The waiting list shows each defendant with known charges and a local remand, bail or awaiting-sentencing record once. It distinguishes remand, active bail and at-large status. A hearing already underway occupies the court and removes that defendant from the waiting list. NPC judge availability follows the same courtroom and Judge patrol checks as `requesttrial`. The separate count of connected judges for player-held trials considers their location, character state and jurisdiction authority. Whether a judge can summon a particular defendant also depends on that case.

Active patrols include each launched route, strategy, phase and member count. For longer lists, use the continuation command at the foot of the response. The same page number advances both the waiting list and the patrol list.

## NPC AI overview

A verified SeniorAdmin or above can run `/npc overview` for a private census of active NPCs. It counts each embodied NPC instance, including secondary bodies running their own AI. It reports total NPCs, the number with no individual AI, and two counts for each AI type. For example, an NPC with two Wanderer routines contributes one NPC and two assignments to the Wanderer row. An NPC with several types appears in each relevant row. Group membership is independent, so a member can also appear in the no-individual-AI count.

The same overview lists live group controllers by ID, name, type and member count, including empty groups. Use `/npc group id:42` to inspect one controller’s roster. Each row gives the member’s name, character and instance IDs when available, and group role. Both commands include a continuation when another page is available.

## AI storyteller operations

A verified SeniorAdmin or higher can use the following private commands. This matches the in-game `ais` permission. Use an exact name or the numeric ID from the overview. If two storytellers share a name, select one by ID.

| Command | What staff see or control |
| --- | --- |
| `/storyteller overview` | Up to ten storytellers in ID order, with their paused state, queued batch count, active role, requests, reported input/output tokens and failures. Use `after` to continue. |
| `/storyteller status target:<id or name>` | Per-role usage, retries, incomplete responses, cancellations and dropped events. Waiting work shows batch count, event count and oldest wait by role. The five latest failures show UTC time, role and a short category. |
| `/storyteller pause target:<id or name>` | A confirmation names the storyteller. Confirming cancels its active request and discards work waiting at that moment. |
| `/storyteller resume target:<id or name>` | A confirmation enables the selected storyteller to accept new work. Discarded events stay discarded. |

Queue and active-role figures describe current work. Usage counters and recent failures cover the current server session. The failure history contains categories such as a provider refusal or an invalid classifier decision. Discord saves the selected storyteller's paused setting before acknowledging the change. [AI Storyteller System](../AI/AI_Storyteller_System.md)

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

## Checking an installation

In a permitted Discord channel, try `/game landmarks` before linking an account. Then link a staff account, use `/account linkstatus` to check the binding, and select a jurisdiction with `/justice status`. Slash replies are private. If the command is missing from Discord’s picker, check application-command installation and bot registration. If it appears but reports that the game is unavailable, check `UseDiscordBot` and the authenticated bridge connection.

If an announcement is missing, check the destination for that event. Account creation uses the bot’s admin announcement channel, justice uses the jurisdiction’s channel, and the economy digest uses the subscriber’s DMs. An unchanged shortfall is silent even when a subscriber reruns `/economy digest enabled:true`. Use `/economy risks` to inspect the current condition.

Automated coverage includes null-reader landmark filters and missing prog references, per-account digest transitions, storyteller confirmations, scoped court reports, physical NPC instance counts, paginated group rosters, and failed justice-channel delivery.

On a disposable MySQL world, Telnet registration produced an account-created event at the local bridge. The same check exercised public and staff landmark visibility, jurisdiction selection, storyteller pause and resume confirmations, and persistence of a staff member’s economy digest preference. Verify Discord’s command picker, channel permissions and DM delivery in the installed guild.
