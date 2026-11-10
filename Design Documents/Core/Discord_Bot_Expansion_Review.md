# Discord Bot Expansion Review and Discord Hooks Proposal

## Status and recommendation

Review date: 10 November 2026. Source baseline: commit `5048963cb698ca7ef3563569d675ad0163e958a5` and the existing working tree. This document records the approved investigation and implementation scope. The subsequent implementation is described in [Discord Remote Operations](./Discord_Remote_Operations.md); the inventory and foundation findings below describe the pre-implementation baseline. Unrelated working-tree changes were preserved.

The strongest expansion combines character-free administration, account self-service, economy reports, and explicitly published FutureProg actions called **Discord Hooks**. Keep ordinary gameplay and arbitrary command execution outside this interface.

There are **22 current canonical commands**. The backlog proposes **44 additions**, giving **66 total**, or three times today's command count. Aliases, social trigger phrases, parameter variations, buttons and authored hook definitions do not increase this count. If the target was to *add* three times today's count, that would instead mean 66 additions and 88 total; this review targets tripling the final total.

Counts measure useful actions, including discovery, inspection, execution and result retrieval where those are distinct workflows. Several actions can share one slash-command root; these are not 66 separate Discord root commands.

Implement foundation repairs first, then Discord Hooks and economy analytics, followed by account and moderation workflows. An unrestricted remote prog/command console would expose substantially more authority and character-dependent behavior than these use cases require.

## Current interaction inventory

The bot targets .NET 10 and DSharpPlus 4.5.3. It registers CommandsNext commands through assembly discovery, with custom prefix/phrase handling. No application-command or component workflow is registered in the inspected startup. Sources: [project](../../DiscordBotCore/DiscordBotCore.csproj), [registration](../../DiscordBotCore/DiscordBot.cs), [dispatch](../../DiscordBotCore/CustomCommandHandler.cs).

“Bot admin” means the Discord user-ID allowlist in bot settings, distinct from current engine account authority.

| # | Current command | Access and purpose |
| --- | --- | --- |
| 1 | `help` | Public; local command listing without arguments, engine admin-help route with arguments. See F2. |
| 2 | `playerhelp` | Public helpfile/command-help retrieval. |
| 3 | `proghelp` | Public FutureProg language/function/type documentation. |
| 4 | `register` | Link a Discord user to an account through in-game acceptance. Initial linking requires an online character today. |
| 5 | `send` | Bot admin + linked account; message an online account. |
| 6 | `sendchannel` | Linked account; engine channel-access checks. |
| 7 | `broadcast` | Bot admin + linked account; system announcement. |
| 8 | `who` | Public online-count/available-staff report. |
| 9 | `where` | Intended bot-admin report; continues after denying unauthorised users. See F1. |
| 10 | `stats` | Public global activity, uptime and built-content statistics. |
| 11 | `map` | Bot admin + link; engine requires JuniorAdmin. |
| 12 | `showaccount` | Bot admin + link; engine requires JuniorAdmin. |
| 13 | `showcharacter` | Bot admin + link; engine requires JuniorAdmin. Target is a character; caller need not control one. |
| 14 | `showchargen` | Bot admin + link; inspect an application by ID. |
| 15 | `approvechargen` | Bot admin + link; engine checks state, approval authority/approvers and resources. |
| 16 | `rejectchargen` | Bot admin + link; engine checks submitted state but lacks an equivalent explicit authority guard. |
| 17 | `authorise` | Existing bot admin grants another Discord user bot-admin status; no account link required. |
| 18 | `shutdown` | Bot admin + link; engine requires SeniorAdmin; reboot behavior. |
| 19 | `shutdownstop` | Same, suppressing automatic restart. |
| 20 | `status` | Public bot/bridge connection status. |
| 21 | `love` | Public social response. |
| 22 | `thanks` | Public social response. |

Sources: [modules](../../DiscordBotCore/Modules), [engine handlers](../../MudSharpCore/Discord/DiscordConnection.cs). Aliases include halp/hjalp, phelp, stat, authorize and variants of who. The 37 custom social trigger phrases dispatch to love/thanks and are not additional capabilities.

The bridge already pushes application submission/approval/rejection, petitions, broadcasts, shutdown/crash, bad echoes, staff alerts, deaths, enforcement updates, prog errors, custom messages and configured game channels. Reuse these notifications rather than counting them as new features. The existing `senddiscord(channelId, title, message)` prog function provides outbound messages, not inbound remote invocation.

## Foundation findings

These are source findings, not claims from live Discord testing. No production service was contacted.

| ID | Evidence | Consequence and direction |
| --- | --- | --- |
| F1 | [Where.cs:17](../../DiscordBotCore/Modules/Where.cs#L17) sends denial without returning, then requests where. [engine where:1214](../../MudSharpCore/Discord/DiscordConnection.cs#L1214) takes only a request ID and renders player/account/location details. | Ordinary Discord users can reach private location/account output when connected. Fix the return and require current engine account authority. |
| F2 | [Help.cs:101](../../DiscordBotCore/Modules/Help.cs#L101) sends named help as adminhelp without an admin check. [engine:345](../../MudSharpCore/Discord/DiscordConnection.cs#L345) dispatches the admin variant; line 1170 selects AdminHelp. | Built-in admin help can be returned publicly. Separate public/admin help and authorise the latter. Authored help still filters CanView(null)/PublicText; this does not establish that all private helpfiles leak. |
| F3 | [Engine handlers](../../MudSharpCore/Discord/DiscordConnection.cs) have inconsistent caller checks. Map/account/character checks exist; where, broadcast and send lack equivalent current-account authority checks. Rejection resolves an account but has no explicit authority guard like approval. | Bot-admin grants can outlive engine demotion. Verify linked identity, current authority and target rules for every sensitive operation and confirmation. |
| F4 | [CachedDiscordRequest](../../DiscordBotCore/CachedDiscordRequest.cs) retains a CommandContext/callback; [TcpConnection.cs:273](../../DiscordBotCore/TcpConnection.cs#L273) removes it on response. No expiry/disconnect cleanup exists. | Missing replies retain requests indefinitely. Add deadlines, capacity bounds, disconnect completion and explicit outcomes. Never automatically retry a mutation after an ambiguous timeout. |
| F5 | [Decoder:34](../../DiscordBotCore/DiscordTcpFrameDecoder.cs#L34), [encoding:24](../../DiscordBotCore/DiscordBotProtocol.cs#L24) and [engine framing:318](../../MudSharpCore/Discord/DiscordConnection.cs#L318) search individual delimiter bytes inside UTF-16 data. | Unicode can contain delimiters inside code units. A byte-level probe reproduced internal 01 for U+0101 engine-to-bot and 0A for U+010A bot-to-engine. Escaping actual newlines does not fix this. Use versioned length-prefixed frames, encoding and limits on both ends. |
| F6 | [Engine polling](../../MudSharpCore/Framework/Futuremud.cs#L435) dispatches on the game loop. Its receive buffer lacks the bot decoder's 64 KiB cap; many handlers use throwing Parse calls. Socket writes do not loop over possibly partial send counts. | Large/malformed requests and expensive reports can disrupt the loop; long responses increase partial-write risk. Add validated parsing, bounded input/output, complete writes and per-tick admission limits while preserving engine thread ownership. |
| F7 | Responses generally use the invoking channel, with inconsistent splitting and raw Markdown/code fences. Authorise has no matching revoke operation. | Use private defaults for sensitive data, consistent pagination, mention/fence handling and revocation. Do not extend public-channel response patterns to notes or economy custody. |

Existing shared-secret authentication is useful: the bot rejects unknown/unauthenticated engine message types and compares secrets exactly with fixed-time comparison. It does not replace caller authorisation or encrypt TCP. Restrict the listener to the intended loopback/private deployment and protect transport across untrusted networks.

## Proposed backlog: 44 additions

All names below are proposed. Slash notation describes desired UX, not existing commands. Prefix compatibility can call the same operations.

Access: **Public** needs no link and exposes only published information; **Self** is the verified linked account's own data; **Staff** needs live engine authority; **Owner** means installation owner/implementor policy, not a Discord guild role.

Reuse: **Reuse** means a suitable domain contract exists; **Extract** means logic exists inside a character/editor command; **New** requires configuration, operations or persistence. All features still need transport, permissions and presentation. Effort is relative and excludes the shared foundation.

### Public information: 6

| ID | Interaction | Value and scope | Reuse / effort |
| --- | --- | --- | --- |
| P1 | `/game info` | Published connection instructions, rules/site links, description and maintenance notice. No secret settings. | New configuration; small. |
| P2 | `/game helpsearch <query>` | Search permitted help names, keywords and public text with ranked retrieval results. Extends existing prefix/category lookup. | Extract querying; small/medium. |
| P3 | `/game time <place>` | Current date/time for a configured public clock/timezone; explicit place replaces character location. | Extract rendering; medium. |
| P4 | `/game calendar <calendar> [year]` | Published months/weekdays/year layout, distinct from current time. | Extract calendar display; medium. |
| P5 | `/game weather <region>` | Current conditions for an explicitly published region/controller. | Extract neutral rendering; medium. |
| P6 | `/game chargenguide [topic]` | Builder-published creation guidance and approved race/culture/role descriptions linked to public help. | New publication configuration over existing help; medium. |

Do not expose administrative race/culture catalogues wholesale or evaluate eligibility progs with null characters. P6 is curated guidance; S5 uses a real draft. Public weather is a separately configured bulletin, not a character's knowledge-gated forecast or subjective body temperature.

Sources: [time/calendar](../../MudSharpCore/Commands/Modules/TimeModule.cs), [weather](../../MudSharpCore/Commands/Modules/WeatherModule.cs), [heritage builders](../../MudSharpCore/Commands/Modules/HeritageBuilderModule.cs), [role permissions](../../MudSharpCore/Commands/Modules/GuideModule.cs#L338).

### Linked-account self-service: 6

| ID | Interaction | Value and scope | Reuse / effort |
| --- | --- | --- | --- |
| S1 | `/account linkstatus` | Confirm own linked account and whether the link remains valid. | Link data + engine validation; small. |
| S2 | `/account unlink` | Revoke own link, pending confirmations and account notifications. | New revocation; small/medium. |
| S3 | `/account resources` | Own chargen-resource balances and relevant award dates. | Reuse account data; small. |
| S4 | `/account applications` | Own drafts/submissions/decisions and applicant-visible comments; exclude staff-only material. | Extract self-scoped report; medium. |
| S5 | `/account options <draft>` | Eligible creation choices/costs using the actual owned draft/current stage, without modifying it. | Extract picker eligibility; large. |
| S6 | `/account notifications` | Opt in/out of own application-decision/staff-message notifications and private delivery. | New durable preferences + existing events; medium. |

Normal use needs no active character. Initial ownership proof can retain today's in-game acceptance; a challenge through the authenticated account menu would be a later improvement, not an existing feature. Never accept account name alone as proof or request MUD passwords in Discord. Define relinking, duplicate-link policy and suspension behavior.

Sources: [links](../../DiscordBotCore/DetailedUserSetting.cs), [registration](../../MudSharpCore/Discord/DiscordConnection.cs#L603), [resources](../../MudSharpCore/Accounts/Account.cs#L73), [chargen listing](../../MudSharpCore/Commands/Modules/ChargenModule.cs#L436), [pickers](../../MudSharpCore/CharacterCreation).

For the expanded API, make the engine's persisted Discord-user/account binding authoritative. Today's bot-local accountlinks.data is not an engine-side identity registry. Record a binding only after ownership proof; key it by installation/world and Discord user ID. Keep any bot copy as a cache, invalidate it on unlink/relink and never resolve an account solely from a supplied display name. Define a compatibility transition for existing links, re-verifying ownership where provenance cannot be established. Authenticated transport still trusts the bot to report the actual Discord invoker; engine-side link lookup prevents that claim from freely selecting an account ID.

### Staff reports and necessary tasks: 14

| ID | Interaction | Value and permission direction | Reuse / effort |
| --- | --- | --- | --- |
| A1 | `/admin applications` | Pending queue, waiting time, required approvers and reviewer/lock; Guide baseline with per-application visibility. Complements existing decisions. | Extract queue; medium. |
| A2 | `/admin accountsearch <filter>` | Bounded account lookup when exact name/ID is unknown. JuniorAdmin; omit IP/email by default. | Extract query; medium. |
| A3 | `/admin accountnotes <account>` | Search/read permitted staff notes with authors/dates. JuniorAdmin baseline; preserve privacy rules. | Extract notes; medium. |
| A4 | `/admin addnote <account>` | Add an attributed staff account note through a modal. | Extract editor finalisation; medium. |
| A5 | `/admin resources <account> <resource> <signed-amount>` | Award/deduct with reason, preview and confirmation; preserve note/notification/runtime updates. JuniorAdmin baseline. | Extract operation; medium/large. |
| A6 | `/admin ban <account>` | Suspend with reason, self/hierarchy checks and active-session handling. SeniorAdmin. | Extract canonical operation; large. |
| A7 | `/admin unban <account>` | Lift suspension with current-state check, reason and audit. SeniorAdmin. | Extract canonical operation; medium. |
| A8 | `/admin maintenance` | Inspect or explicitly set login/account-login/chargen restrictions. SeniorAdmin; confirm changes rather than blind toggle. | Extract wizlock; medium. |
| A9 | `/admin connections` | Connected users/session states. HighAdmin baseline; redact IPs unless separately authorised. | Extract users report; medium. |
| A10 | `/admin performance` | Bounded loop-phase/heartbeat timings and available counters; read-only initially. SeniorAdmin baseline. | Extract diagnostics; medium. |
| A11 | `/admin census` | Loaded/cached actors, NPCs, bodies and schedules; complements timings. SeniorAdmin baseline. | Extract census; medium. |
| A12 | `/admin actionlog` | Query attributed Discord operations, confirmations and outcomes across reports/moderation/hooks. | New shared audit queries; medium. |
| A13 | `/admin deauthorise <discord-user>` | Revoke bot-admin access and pending confirmations. Owner-controlled with self-lockout protection. | New counterpart to authorise; small/medium. |
| A14 | `/admin resourceaudit [resource]` | All-account resource distribution/award audit. Admin; distinct from own balances. | Extract resource audit; medium. |

Sources: [application queue](../../MudSharpCore/Commands/Modules/GuideModule.cs#L164), [resource audit](../../MudSharpCore/Commands/Modules/StorytellerModule.cs#L811), [award/deduct](../../MudSharpCore/Commands/Modules/StorytellerModule.cs#L1002), [notes](../../MudSharpCore/Commands/Modules/StorytellerModule.cs#L2047), [ban](../../MudSharpCore/Commands/Modules/StaffModule.cs#L1620), [users](../../MudSharpCore/Commands/Modules/StaffModule.cs#L2309), [maintenance](../../MudSharpCore/Commands/Modules/StaffModule.cs#L2367), [performance](../../MudSharpCore/Commands/Modules/StaffModule.cs#L2737), [census](../../MudSharpCore/Commands/Modules/StaffModule.Census.cs).

Consolidate existing Discord application decisions with canonical review-lock/authority/resource/finalisation logic rather than expanding parallel handlers. Never report success before completion. Site bans, raw IP histories, arbitrary log downloads and exception-file uploads are outside this recommended backlog.

### Economy administration: 12

| ID | Interaction | Value | Reuse / effort |
| --- | --- | --- | --- |
| E1 | `/economy summary [zone]` | Supply, PC wealth/liquidity, reserves, activity and warnings. | Reuse analytics + extract composition; medium. |
| E2 | `/economy money [zone] [currency]` | Aggregate custody/control buckets and supply totals. | Reuse holdings; small/medium. |
| E3 | `/economy holdings [filters]` | Detailed custody/controller records with pagination; distinct from totals. | Reuse holdings; medium. |
| E4 | `/economy volume <window>` | Exchange versus movement by zone/currency. | Reuse volume DTO; small/medium. |
| E5 | `/economy income <window>` | Settled ordinary-PC job/project/clan income, excluding unrelated transfers. | Reuse income data; small/medium. |
| E6 | `/economy employment [zone]` | Jobs, openings, vacancies, employed/free workers and capabilities. | Reuse employment DTO; small/medium. |
| E7 | `/economy trends <metric>` | Supply/wealth/reserve/income/employment series; bounded table/export initially. | Reuse trend DTOs; medium. |
| E8 | `/economy wealth [filters]` | Restricted top-wealth/control report, labelled for shared control. | Extract ranking over holdings; medium. |
| E9 | `/economy risks [zone]` | Existing risk signals and why they triggered. | Reuse risk DTOs; small/medium. |
| E10 | `/economy snapshotstatus` | Last/next capture, interval, coverage start, enabled state and display currency. | Reuse properties; small. |
| E11 | `/economy snapshot [zone]` | Take a manual capture; return identifier and completion outcome. | Reuse operation; medium. |
| E12 | `/economy configure` | Snapshots/interval/rollover/display currency, with validated preview and confirmation. HighAdmin. | Reuse validated setters; medium. |

E1–E11 require Admin; configuration mutations require HighAdmin, matching the owning command. Results are private by default.

[IEconomyAnalyticsService.cs:205](../../FutureMUDLibrary/Economy/IEconomyAnalyticsService.cs#L205) already provides structured holdings, volume, employment, trends and risks. Format DTOs instead of scraping ANSI or impersonating a character. Sources: [analytics commands](../../MudSharpCore/Commands/Modules/EconomyModule.Analytics.cs), [employment/income](../../MudSharpCore/Commands/Modules/EconomyModule.Analytics.Employment.cs).

Correctness constraints:

- Real and MUD-time windows differ; MUD windows need economic-zone/calendar context.
- Display coverage start/missing history. New metrics have no backfill; old transactions are not a complete historical ledger.
- Preserve currency units/conversions; never add nominal amounts across currencies.
- Holdings include offline custody and can be expensive. Do not run a live census for every autocomplete/refresh; reuse one capture within a request.
- Shared-PC control is not individual spendable wealth. Label wealth, liquidity and reserve measures accurately.
- Cached reports need capture time and filters; never silently substitute stale results for current state.

### Discord Hooks: 6

| ID | Interaction | Value | Reuse / effort |
| --- | --- | --- | --- |
| H1 | `/hooks list [category]` | Published hooks the caller may see, with purpose/availability. | New registry/query; medium. |
| H2 | `/hooks show <hook>` | Usage, arguments, authority, confirmation, cooldown and effects. | New renderer; small after registry. |
| H3 | `/hooks run <hook> [arguments]` | Validate/execute its prog, with confirmation where required. | New guarded service; large. |
| H4 | `/hooks history [hook]` | Permitted recent invocations, callers, timestamps and outcomes. | Shared audit + hook query; medium. |
| H5 | `/hooks result <invocation>` | Retrieve an outcome after timeout/disconnection; distinguish pending/failed/unknown. | New durable result retrieval; medium. |
| H6 | `/hooks enabled <hook> <on|off>` | Emergency availability control, requiring higher authority than ordinary invocation. | New registry mutation/audit; medium. |

## Discord Hook engine design

### Definition and ownership

A Discord Hook is a **published remote action backed by a trusted FutureProg**, not a public webhook URL. It needs no attached character, room or item.

Use a dedicated shared contract such as IDiscordHook and an engine registry/service. Definitions need stable ID, unique command key, name/category, description/usage, prog reference, enabled state, minimum authority, optional account restrictions, allowed guild/channel contexts, argument contract, cooldown, confirmation requirement, result visibility and definition revision.

Default creation/publication/editing to SeniorAdmin or higher. Execution initially defaults to staff authority; deliberate delegation must remain within engine policy. Discord roles/channel restrictions are additional controls, not engine authority.

Persist definitions/invocations with normal engine save/load ownership and an EF migration. Do not store world actions only in bot settings. Load after progs; revalidate changes to signatures/definitions. Broken actions remain inspectable by permitted maintainers but cannot run. Protect references from unsafe deletion/reassignment.

Existing [IHook](../../FutureMUDLibrary/Events/Hooks/IHook.cs), [HookBase](../../MudSharpCore/Events/Hooks/HookBase.cs), [FutureProgHook](../../MudSharpCore/Events/Hooks/FutureProgHook.cs) and [hook commands](../../MudSharpCore/Commands/Modules/ProgModule.cs#L1970) use EventType and perceivable attachment; persistence includes TargetEventType and subtype XML. They supply useful conventions, but subclassing them would introduce a global-dispatch exception and still need remote policy. A dedicated Discord Hook registry fits better. The trade-off is new persistence/builder surface rather than reusing hook create. No new EventType is needed.

### Builder workflow

Proposed in-game syntax, not implemented commands:

```text
discordhook list
discordhook new storm-warning <prog>
discordhook edit storm-warning
discordhook set description "Issue the configured storm warning."
discordhook set usage "<public-region> <message>"
discordhook set authority admin
discordhook set confirm on
discordhook set cooldown 60s
discordhook set enabled on
discordhook show storm-warning
```

Use existing builder helper conventions and clearable optional fields. Initially keep definition editing in the normal builder environment; Discord gets discovery/execution/emergency availability. Later scalar argument declarations can improve validation and modal forms. Authored definitions do not increase native interaction counts.

### Prog contract

An initial fixed signature avoids adding a new language type:

```text
Return type: Text
Parameters, in order:
  Number requesterAccountId
  Text discordUserId
  Text invocationId
  Text arguments
```

This is a proposed type contract, not literal FutureProg syntax. Engine supplies identity; users supply only arguments. Keep Discord snowflakes as text in wire/prog context rather than converting through floating point. Guild/channel identity remains in engine policy/audit context.

There is no generic Account prog type in [ProgVariableTypes](../../FutureMUDLibrary/FutureProg/ProgVariableTypes.cs). Do not advertise Account/DiscordContext parameters as existing capabilities. Add a context type only when its language/runtime/documentation cost is justified.

Use ExecuteWithStatus: the concrete engine implementation separates errors from legitimate null returns and bypasses cached execution. Validate compilation, exact parameter order/types, standard compilation context and Text return before publication and execution. Blank output can produce a neutral completion message; it does not prove the intended domain change happened. Sources: [interface](../../FutureMUDLibrary/FutureProg/IFutureProg.cs), [implementation:619](../../MudSharpCore/FutureProg/FutureProg.cs#L619), [durable callback precedent](./FutureProg_Execution_Runtime.md).

Optional eligibility progs can follow with their own fixed signature, never granting authority beyond native policy. A read-only label describes author intent; ordinary FutureProg remains powerful and the label does not sandbox side effects.

### Execution and failure semantics

1. Discord acknowledges/defer-responds privately, verifies deployment context/account link, then sends a typed request with stable operation ID.
2. Engine resolves current account, suspension/authority, definition/revision, context, arguments, cooldown and concurrency policy. Apply visibility policy to discovery/autocomplete too.
3. Where required, issue an expiring confirmation bound to caller/account/action revision/exact arguments/target state. Recheck authority/state at confirmation; stale buttons cannot execute changed actions.
4. Persist acceptance and admit bounded work on the engine thread. Persist invoking before calling the prog so restart cannot replay an unknown mutation.
5. Execute on the engine thread; catch operation-boundary exceptions including Debug behavior. Record state, duration, safe response and definition/prog identity.
6. Return privately. Delivery failure does not undo completion; result retrieval uses the operation ID and current authorisation.

States: pending confirmation, queued, invoking, completed, failed, expired/cancelled before execution, and indeterminate after interrupted execution. Duplicate IDs return existing state/results; reject reuse with conflicting caller/operation data.

Use a durable globally unique invocation ID scoped to the world/installation, not the current process-local incrementing request counter. Correlation IDs for socket replies can remain session-local, but reconnect/restart must not collide with old invocation records. Callers can retrieve their own still-permitted results; viewing other callers' history requires explicit audit authority. Recheck current visibility on both history and result retrieval.

This provides duplicate suppression and an auditable at-most-once attempt policy, not exactly-once world effects across crashes. A prog can mutate several objects and fail partway through; general rollback does not exist. Interrupted invocations require inspection and must never be retried automatically. Authors may use invocationId for domain-specific idempotency.

Request/Discord deadlines stop waiting; they do not safely abort a synchronous prog. Current loop/recursion guards do not constitute a universal cancellation budget. Queue limits/cooldowns also cannot bound one arbitrary prog's runtime. Publish trusted bounded staff progs initially, measure durations and disable problematic definitions. Strong instruction/time budgets would be a separate shared FutureProg runtime change. Never move world mutation into Task.Run or claim a timeout cancelled execution.

Use returned text for normal private responses. Existing senddiscord remains an explicit outgoing side effect; argument text must not become an arbitrary destination channel. Its true result means queued, not confirmed delivery.

### Useful authored hooks

Examples of game-authored actions, not claims that every domain already has a suitable built-in function:

- Run a named world maintenance/reconciliation prog already present in that game.
- Start/stop/announce an authored event without a logged-in staff character.
- Update an approved game variable or story-state flag with validated input.
- Produce a setting-specific faction/population/event-readiness report.
- Trigger an approved restock/population/weather policy through canonical domain operations.
- Issue a reasoned in-game warning or staff notification.

Keep native economy/moderation workflows native where stronger canonical services exist. Hooks serve world-specific policy; they do not replace native permission/persistence rules.

## Discord UX and bridge evolution

Use slash commands for discovery, autocomplete for permitted hooks/filters, buttons for confirmation/pagination and modals for reasons. Group under game/account/admin/economy/hooks. Dynamic definitions belong behind hooks run rather than registering a Discord command per prog.

Discord requires an initial response within three seconds; deferred follow-ups currently have a 15-minute token lifetime. Defer promptly, and use durable result lookup beyond that lifetime. Sensitive output defaults to ephemeral. Prefix compatibility needs verified private delivery and must fail closed if unavailable, never fall back to public output.

Official references checked: [receiving/responding](https://docs.discord.com/developers/interactions/receiving-and-responding), [commands/permissions/contexts/autocomplete](https://docs.discord.com/developers/interactions/application-commands).

The project references CommandsNext/Interactivity without an application-command integration. Verify APIs against installed DSharpPlus 4.5.3 before implementation; this proposal does not require an unverified upgrade. An incremental interaction adapter can share operations with prefix commands.

Add a versioned envelope with operation/request ID, caller/link identity, guild/channel context, structured arguments and machine-readable outcomes. Negotiate capabilities so mixed bot/engine versions reject unsupported actions clearly. Coordinate length-prefix framing changes on both ends, including partial reads/writes, limits and deployment compatibility.

Keep secrets out of results/audits. Store Discord user, linked account, operation/version, target, redacted arguments/reason, timestamps, confirmation and outcome. Define retention/access/result-size limits; staff text itself may be sensitive. Preserve local exception-file reporting rather than introducing automatic raw report uploads.

## Delivery order and acceptance

Relative effort excludes common protocol/persistence/UI work. Calendar publication and draft eligibility can grow with game policy. Calendar-time estimates would require an agreed first implementation slice.

1. **Foundation:** fix F1/F2 and centralise sensitive caller checks; bounded/versioned transport, request lifecycle, private rendering and operation audit. Preserve existing notification behavior.
2. **First useful expansion:** H1–H5 with in-game authoring; E1/E4/E6/E9/E10 and A1/S1/S3/S4. Include H6 before broader hook delegation.
3. **Operational workflows:** notes, resources, explicit maintenance, revocation and canonical application decisions; moderation after hierarchy/session/confirmation checks.
4. **Complete the 44:** remaining economy details/config, diagnostics, account preferences/draft eligibility and curated public information.

Implementation acceptance:

- Explicit policies for all existing commands; negative tests for where/admin-help/account/application access, including direct engine requests.
- Demotion/suspension/unlink/revocation/definition edits between preview and confirmation prevent execution.
- Public outputs use only published information; eligibility uses an actual owned draft.
- Definitions reload with stable IDs/signatures/revisions and disabled/broken states. No active character is required to discover/run/retrieve hooks.
- Invalid arguments, compile/runtime errors and blank results have distinct outcomes.
- Duplicate buttons/requests, disconnects, expiry and restart never automatically repeat mutations; interrupted partial execution remains visible.
- Both directions cover Unicode U+0101/U+010A, quotes/newlines, mentions/fences, fragmentation/coalescing, oversized frames and partial writes.
- Private responses, pagination and attachment limits are verified; failed private delivery cannot publish sensitive results.
- Economy reports match service results for identical filters/captures, including offline/shared custody, currencies, real/MUD windows and history gaps.
- Resource/ban/maintenance actions preserve canonical side effects and authority. Attribution survives restart.
- Measure representative expensive reports/bounded hooks on a disposable populated world; queue limits must not starve gameplay/notifications.

The bot unit suite covers framing, authentication, settings, link serialisation and request-ID uniqueness. Core [ExternalIntegrationSecurityTests](../../MudSharpCore%20Unit%20Tests/ExternalIntegrationSecurityTests.cs) cover show-command authority/parsing and alert sanitisation. These do not prove full command permissions or live gameplay/Discord behavior.

Review validation: source/dispatch/persistence/service tracing, complete command inventory, delimiter byte-level probe, document-link and diff checks. No runtime code changed. No unit suites, live MUD, Discord bot or migrations were executed for this investigation. Implementation must run the owning bot/core/persistence suites and an explicit disposable-world Discord acceptance scenario.
