# FutureMUD Time And Date System

## Scope
This document describes the verified current implementation of FutureMUD's in-game time and date system.

It covers the shared value types in `FutureMUDLibrary/TimeAndDate`, the concrete runtime classes in `MudSharpCore/TimeAndDate`, and the integration points that make time useful to the rest of the engine:

- clocks, time zones, and clock advancement
- calendars, years, months, intercalaries, weekdays, and date display
- `MudDate`, `MudTime`, `MudDateTime`, and `MudTimeSpan`
- temporal listeners and recurring intervals
- FutureProg date/time support
- player, builder, seeder, persistence, and boot integration

The document is intentionally current-state focused. Future work is listed separately near the end.

## Design Goals
The system exists to let worlds define fictional time models without hard-coding Earth Gregorian assumptions into game logic.

The important design goals are:

- support multiple clocks and calendars in the same game
- support non-standard clocks, such as decimal time or non-24-hour days
- support non-standard calendars, including unusual week lengths, leap days, intercalary months, and named special days
- keep builder-facing display and parsing driven by clock/calendar definitions
- provide stable value objects for scheduling, FutureProg, economy, clans, weather, celestial objects, and world presentation
- allow player-facing precision to depend on character knowledge and perception rather than always showing exact machine time

## Layered Shape
The subsystem is split between shared contracts/value objects and concrete runtime implementation.

| Layer | Responsibility | Typical locations |
| --- | --- | --- |
| Contracts and value objects | Interfaces, `MudDate`, `MudTime`, `MudDateTime`, `MudTimeSpan`, calendar parts, intervals | `FutureMUDLibrary/TimeAndDate` |
| Runtime implementations | `Calendar`, `Clock`, `MudTimeZone`, listeners, interval listener adapters, builder editing | `MudSharpCore/TimeAndDate` |
| Persistence | EF models for calendars, clocks, time zones, shard links, zone time zones, and scheduled FutureProgs | `MudsharpDatabaseLibrary/Models` |
| Builder/player commands | `time`, `calendar`, `clock`, `timezone`, shard/zone clock and calendar assignment | `MudSharpCore/Commands/Modules` and `MudSharpCore/Commands/Helpers` |
| Seeding | stock clocks, time zones, and calendar definitions | `DatabaseSeeder/Seeders/TimeSeeder.cs` |
| Automation | temporal listeners, recurring intervals, and FutureProg schedules | `MudSharpCore/TimeAndDate/Listeners`, `MudSharpCore/TimeAndDate/Intervals`, `MudSharpCore/FutureProg/ProgSchedule.cs` |

## Boot And Runtime Advancement
Clocks and calendars are early world infrastructure.

During boot:

- clocks load before calendars, celestials, listeners, world cells, and climate
- calendars load after clocks because each calendar has a feed clock
- celestials, weather, shards, zones, clans, economy, jobs, and player-facing world presentation can then resolve time references safely
- `ClockManager.Initialise()` runs near the end of boot and starts real-time advancement unless the `TimeIsFrozen` static configuration is set

`ClockManager` keeps an in-memory dictionary of each loaded clock and the next real-world UTC instant at which that clock should tick. On each engine update it advances a clock by one in-game second while the stored next-tick time is in the past, then moves the next-tick time forward by:

```text
1000ms / clock.InGameSecondsPerRealSecond
```

The implementor commands can freeze and unfreeze time. Freezing clears the manager's tick dictionary and persists `TimeIsFrozen=true`; unfreezing rebuilds tick state for all loaded clocks and persists `TimeIsFrozen=false`.

## Persistence Model
Time definitions are persisted separately from their current positions.

### Clocks
The `Clocks` table stores:

- `Definition`: XML defining clock display strings, units, hour intervals, crude time intervals, and rate
- `Hours`, `Minutes`, `Seconds`: current position in the primary time zone
- `PrimaryTimezoneId`: the default time zone for the clock

Each clock has one or more `Timezones` rows. A time zone stores:

- `Name`: the alias used in parsing and display
- `Description`: the long display name
- `OffsetHours` and `OffsetMinutes`
- `ClockId`

### Calendars
The `Calendars` table stores:

- `Definition`: XML defining calendar names, display masks, eras, weekdays, months, intercalary rules, and feed clock
- `Date`: current date as parseable calendar text
- `FeedClockId`: the clock normally used for current date/time combinations

### World Links
Shards own available clocks and calendars through shard link tables. Zones choose one time zone per relevant clock, so the same clock can be displayed differently in different areas.

Economic zones, clans, boards, weather controllers, celestials, and FutureProg schedules also persist references to clocks, calendars, time zones, or `MudDateTime` round-trip strings.

### Stored Value Fallbacks
Builder edits to clocks and calendars can make old round-trip strings invalid. Runtime database and XML load paths therefore use explicit stored-value helpers rather than builder/player parsing:

- `MudDateTime.TryParseStored(...)`
- `MudDateTime.FromStoredStringOrFallback(...)`
- `ICalendar.GetStoredDateOrFallback(...)`
- `IClock.GetStoredTimeOrFallback(...)`

These helpers are only for trusted stored engine data. Interactive builder/player input still uses strict parsing and returns explanatory errors.

Fallback policies are chosen by owner semantics:

- active scheduling and cadence state normally falls back to the owning calendar or clock's current date/time
- optional expiry, closed, end, and "not scheduled" values fall back to `MudDateTime.Never` or `null` where the owning code supports null
- historical/audit records fall back to current in-game date/time so displays remain stable
- character birth or join dates fall back to the relevant current calendar date
- celestial epoch values fall back to the first valid date in the epoch year so movement remains deterministic
- saved FutureProg `muddatetime` variables fall back to `MudDateTime.Never`
- calendar current date falls back to the first valid generated date for the current or epoch year

Every fallback attempts to notify admins through `DiscordConnection.NotifyAdmins`. The notification includes the bad stored string, value type, calendar name/id, clock name/id where applicable, owner type/id/name, affected field, fallback value, and parse failure reason. If Discord is unavailable or notification fails, the same payload is written through the server console utility and the load continues.

## Clock Model
`IClock` defines a configurable clock rather than assuming Earth time.

Important properties include:

- `SecondsPerMinute`
- `MinutesPerHour`
- `HoursPerDay`
- `InGameSecondsPerRealSecond`
- fixed digit settings for hours, minutes, and seconds
- `NumberOfHourIntervals`
- short interval names such as `am` and `pm`
- long interval names such as `in the morning`
- crude time ranges such as `night`, `morning`, `afternoon`, and `evening`
- display masks for short, long, and superuser output

`Clock.DisplayTime` turns a `MudTime` into player-facing text using a mask or a display type:

- `Short`
- `Long`
- `Immortal`
- `Vague`
- `Crude`

The player `time` command chooses exact, vague, or crude display depending on checks such as `ExactTimeCheck` and `VagueTimeCheck`, and can also display visible timepiece items.

## Time Zones And MudTime
`MudTime` represents a time-of-day on a specific clock and time zone. It stores:

- seconds
- minutes
- hours
- time zone
- clock
- `DaysOffsetFromDatum`
- whether it is the primary clock time

The primary clock time is the canonical advancing time for a clock. When a primary `MudTime` crosses a day boundary, it raises the clock's day advancement event. Non-primary times instead record day rollover in `DaysOffsetFromDatum`, so callers can apply the rollover to a `MudDate` when building a full `MudDateTime`.

There are two important construction patterns:

- `MudTime.CreatePrimaryTime(seconds, minutes, hours, timezone, clock)` creates the authoritative feeder time for a clock.
- `MudTime.FromPrimaryTime(seconds, minutes, hours, timezone, clock)` converts a primary/datum time into a timezone-local time.
- `MudTime.FromLocalTime(seconds, minutes, hours, timezone, clock, daysOffset)` creates a local wall time already expressed in that time zone.
- `MudTime.ParseLocalTime(text, clock)` and `MudTime.TryParseLocalTime(text, clock, out time, out error)` parse builder/player-entered local wall time.
- `MudTime.CopyOf(time, resetDaysOffsetFromDatum)` copies a time without preserving primary-clock behavior.

The named factories centralize component validation, time-zone ownership checks, and the distinction between datum conversion and local wall time. `Clock.GetTime(string)` remains as a facade over `MudTime.ParseLocalTime`.

`MudTime.GetTimeByTimezone` converts between time zones on the same clock and preserves any day rollover through `DaysOffsetFromDatum`.

## Calendar Model
`ICalendar` defines a named calendar, its feed clock, display masks, eras, weekdays, month definitions, and intercalary months.

A calendar definition includes:

- alias, short name, full name, and description
- display masks for short, long, and wordy date display
- plane text
- feed clock id
- epoch year and first weekday at epoch
- ancient and modern era strings
- ordered weekday names
- base month definitions
- intercalary month definitions
- optional regnal period definitions

The concrete `Calendar` creates generated `Year` instances as needed. Generated years are cached by year number. Calendar day counts, weekday counts, days-between-year counts, and first-weekday calculations are also cached.

Regnal periods are calendar-local metadata layered on top of ordinary dates. They are persisted in optional calendar XML under `<regnalperiods>` with immutable keys, editable short/full display names, a required start date, and an optional end date. Missing XML loads as an empty regnal-period list so legacy calendars keep their existing behavior.

A regnal period does not replace the calendar year. Dates are still stored and compared as ordinary `MudDate` values, and regnal display is recalculated from the base date each time. This lets a future projected regnal input such as `RY20 @charles-iii` convert to an ordinary date first, then later redisplay under a different current regnal period or as an ordinary year if later builder edits close the original period early.

Regnal period XML uses this shape:

```xml
<regnalperiods>
  <regnalperiod key="charles-iii" short="Charles III" full="King Charles III" start="15/march/1200" end="14/march/1208" />
  <regnalperiod key="arthur" short="Arthur" full="King Arthur" start="15/march/1208" />
</regnalperiods>
```

The first weekday of a year is computed from:

- the epoch year
- the first weekday at epoch
- the count of weekday-counting days between the target year and epoch
- the actual number of weekdays in the calendar, not an assumed seven-day week

This is what allows calendars with five-day, six-day, eight-day, or other custom week lengths to work correctly.

## MudInstant And Calendar Algorithms
`MudInstant` is the absolute mud-time value used when a caller needs an ordered, storage-safe instant rather than a calendar-relative string. It stores a versioned epoch plus clock-second ticks from the owning calendar's epoch year and round-trips as:

```text
mudinstant:v1:<ticks>
```

`MudInstant.FromLegacyState(calendar, clock)` back-calculates the current absolute instant from an existing calendar's current date and feed clock time. This is the boot/backfill path for existing worlds: no database migration or seeder rerun is required, and old `MudDate`, `MudTime`, `MudDateTime`, recurring intervals, and stored strings remain authoritative. `MudInstant.ToMudDateTime(calendar, clock, timezone)` projects an instant back through the selected calendar algorithm and clock.

Calendar XML now has optional algorithm and day-boundary metadata:

```xml
<algorithm type="fixed-months" />
<dayboundary type="ClockMidnight" />
```

Missing `algorithm` or `dayboundary` elements preserve the legacy behavior: fixed authored months/intercalaries and clock-midnight day starts. The runtime currently supports these algorithm types:

- `fixed-months`: legacy authored month and intercalary rules
- `tabular-lunar`: alternating 30/29 day lunar months with a deterministic 30-year leap-day cycle
- `calculated-hebrew`: calculated Hebrew month generation with Metonic leap years, postponement-derived year lengths, Heshvan/Kislev variation, and Adar I/II handling
- `solar-equinox`: deterministic solar/equinox approximation, including Old Persian/Zoroastrian-style epagomenal days
- `astronomical-lunar`: deterministic mean-lunation calendars, including Hijri-style and Babylonian regulated variants
- `east-asian-lunisolar`: deterministic East Asian lunisolar approximation with mean new moons and a predictable leap-month placement rule

The algorithm object owns generated-year shape. Calendar metadata still owns display masks, aliases, eras, weekdays, authored month names, and builder presentation.

### Day Boundaries And Authority Locations
Calendar day starts are separately configured through `CalendarDayBoundaryType`.

Supported boundary types are:

- `ClockMidnight`
- `FixedClockTime`
- `SunsetAtAuthorityLocation`
- `SunriseAtAuthorityLocation`
- `AstronomicalEvent`

Legacy calendars default to `ClockMidnight`. Sunrise and sunset boundaries use the calendar's authority location and the first available solar ephemeris when present; otherwise they fall back to clock midnight so old worlds keep booting. Authority locations are stored as `GeographicCoordinate` values in calendar XML and can be inspected or edited by builders.

## Months, Intercalaries, And Weekdays
`MonthDefinition` is the authored month template. `Month` is the generated month for a specific year.

A month definition contains:

- alias
- short name
- full name
- nominal order
- normal day count
- special day names
- non-weekday day numbers
- intercalary day rules

Intercalary days can:

- insert extra days into a month
- add special day names
- remove special day names
- add non-weekday dates
- remove non-weekday dates

Intercalary months are separate month definitions that appear only when their intercalary rule matches the generated year.

`MudDate.SetWeekday` uses the generated year's first weekday, the weekday-counting days in earlier generated months, and the current month's non-weekday list. Dates marked as non-weekdays have an empty weekday string and `WeekdayIndex = -1`.

## MudDate
`MudDate` represents a date in a calendar. It stores:

- calendar
- day
- month
- year number
- generated `Year`
- weekday string and weekday index
- whether it is the primary calendar date

Primary dates update the owning calendar when advanced. Copied or parsed dates are non-primary value objects.

`MudDate` supports:

- day, month, and year advancement
- forward and backward weekday seeking
- conversion to another calendar by day difference from each calendar's current date
- date difference and year difference operations
- round-trip parse text
- lookup of the active regnal period and computed regnal year through the owning calendar
- display through calendar masks

`MudDate.AdvanceToNextWeekday` is exclusive of the current date. A positive occurrence count moves forward; a negative count moves backward; zero is a no-op.

## MudDateTime
`MudDateTime` combines a `MudDate`, `MudTime`, and `IMudTimeZone`.

It is the main value type for in-game scheduling and FutureProg mud date/time operations. It supports:

- parsing from player input
- parsing from round-trip storage text
- display through calendar and clock display modes
- time-zone conversion
- calendar conversion
- comparison across time zones
- addition and subtraction of `MudTimeSpan` or `TimeSpan`
- FutureProg dot references

### Never
`MudDateTime.Never` is a sentinel value represented by a null date, time, and time zone.

Current behavior treats `Never` as less than all real mud datetimes and equal to other `Never` values. Copying, comparison, round-tripping, calendar conversion, object equality, and the FutureProg `midnight` property preserve the sentinel.

Callers should check `Date == null` or use the FutureProg `isnever` dot reference rather than assuming all `MudDateTime` values have live date/time components.

### Parsing
The actor-facing parser accepts:

- `never`
- `now`
- `soon`
- dates in the supported calendar parse forms
- regnal date forms such as `regnal:charles-iii:20:may:3`, `May 3rd RY20 @charles-iii`, and `3 May RY20 @charles-iii`
- times such as `3:15pm`, `15:15:00`, and `15:15:00 UTC`
- optional clock hour interval text
- optional time-zone aliases

When a time zone is specified, the parsed time is treated as local wall time in that time zone. Unknown time zones or meridian/period text are rejected cleanly.

Regnal parsing accepts the canonical `RY<number> @key` form for unambiguous round-tripping. Natural period names may be accepted only when the text uniquely resolves to one configured period. Regnal parsing is allowed to project beyond a period's current end date so future-written text can still be converted to an ordinary date; display only uses a regnal period when the resulting base date falls within the period's actual bounds.

`ToMudDateFunction` in FutureProg uses this parser and returns `MudDateTime.Never` for invalid text, missing calendars, missing clocks, or empty input.

## MudTimeSpan
`MudTimeSpan` is the engine's in-game duration type. It can represent:

- years
- months
- weeks
- days
- hours
- minutes
- seconds
- milliseconds

Years and months are calendar-like duration components. Weeks are interpreted relative to the target calendar's weekday count when applying a span to `MudDateTime`. Smaller components are stored as milliseconds.

Important property conventions:

- `Seconds`, `Minutes`, and `Hours` are total duration values.
- `SecondComponentOnly`, `MinuteComponentOnly`, and `HourComponentOnly` are component remainders.
- `DayComponentOnly` is the day component carried by the millisecond portion.
- `TotalSeconds`, `TotalMinutes`, `TotalHours`, and `TotalDays` expose double totals.

When applying a `MudTimeSpan` to `MudDateTime`, time components adjust the `MudTime`, any day rollover is applied to the date, then week/month/year components are applied to the date.

## Recurring Intervals
`RecurringInterval` is a compact recurrence description used by clans, economy, property, shoppers, jobs, and FutureProg schedules.

It supports interval types:

- minutely
- hourly
- daily
- monthly
- ordinal day of month
- ordinal weekday of month
- specific weekday
- weekly
- yearly

The parser accepts text like:

```text
every 3 days
every 1 month
every weekday 4
every month on day 15
every 2 months on the 15th
every month on last day
every month on the 5th Wednesday
every month on the 5th or last Wednesday
every 3 months on the 12th or last Marketday
```

For `SpecificWeekday`, `Modifier` is the weekday index in the calendar. For other interval types, `Modifier` is usually zero.

For ordinal month intervals:

- `OrdinalDayOfMonth` uses `Modifier` as the target day. `-1` means the last day of the month.
- `OrdinalWeekdayOfMonth` uses `Modifier` as the ordinal occurrence and `SecondaryModifier` as the calendar weekday index.
- `OrdinalFallbackMode.ExactOnly` skips months where the requested weekday occurrence does not exist.
- `OrdinalFallbackMode.OrLast` uses the requested occurrence when present, otherwise the last matching weekday in that month.

Day-of-month recurrences clamp to the last valid day in shorter months. Ordinal weekday recurrences accept any positive ordinal; the search uses generated calendar months and weekday names, so non-seven-day weeks and long fictional months are supported. The search is bounded and throws if no valid occurrence can be found inside the conservative month-search horizon.

Recurring intervals can:

- describe themselves in player-facing text
- find the next date/time after the current game time
- find the last date/time relative to current game time
- find adjacent date/times around current game time
- create listeners through the `IntervalExtensions` helpers

Weekday intervals use the owning calendar's weekday list and support both forward and backward movement.

### Interval Persistence
Persistent recurring interval owners store the primary recurrence type, amount, modifier, secondary modifier, fallback mode, and reference date/time needed to recreate runtime listeners.

Current interval persistence includes:

- `ProgSchedules.IntervalOtherSecondary` and `ProgSchedules.IntervalFallback`
- `EconomicZones.IntervalOther` and `EconomicZones.IntervalFallback`
- `Clans.PayIntervalOtherSecondary` and `Clans.PayIntervalFallback`

The new fields default to `0`, so existing rows continue to load with legacy recurrence semantics. String and XML interval persistence still round-trips through `RecurringInterval.ToString()` and `RecurringInterval.Parse`.

## Temporal Listeners
Temporal listeners are in-memory callback objects that subscribe to clock or calendar events.

The main listener types are:

- `DateListener`
- `TimeListener`
- `WeekdayListener`
- `WeekdayTimeListener`

`ListenerFactory` is the normal construction surface. It creates listeners for absolute date/time targets, offsets, weekday targets, and combined date/time targets.

Listener repeat counts follow the existing one-shot convention:

- `repeatTimes = 0` means fire once and unsubscribe after the successful payload.
- `repeatTimes = 1` also fires once and unsubscribes after the successful payload.
- values above one fire that many successful payloads.

The shared listener trigger path invokes the payload, decrements the repeat count, unsubscribes when exhausted, and removes the listener from the gameworld.

Listeners are not themselves the long-term persistence model. Persistent behaviors such as FutureProg schedules store their recurrence/reference data and recreate listeners on load or after each fire.

## FutureProg Integration
FutureProg exposes three related date/time surfaces:

- real-world UTC `DateTime`
- real-world `TimeSpan`
- in-game `MudDateTime`

`MudDateTime` dot references include:

- `second`
- `minute`
- `hour`
- `day`
- `month`
- `year`
- `isnever`
- `midnight`
- `calendar`
- `clock`
- `timezone`
- `mudinstant`

Important built-in Date/Time functions include:

- `now()` for real-world UTC `DateTime`
- `now(calendar)`, `now(calendar, clock)`, and `now(calendar, clock, timezone)` for in-game `MudDateTime`
- `todate(text, mask)` for real-world `DateTime`
- `todate(calendar, clock, text)` for in-game `MudDateTime`
- `totext(...)` overloads for date/time text output
- `nextweekday(...)` and `lastweekday(...)` for real-world and mud date/time values
- `between(...)` for `TimeSpan`, `DateTime`, and `MudDateTime`
- arithmetic operators for date/time plus or minus spans
- `GameSecondsPerRealSeconds(clock)` for clock speed introspection

Celestial event FutureProg functions return `MudDateTime` values projected from `MudInstant` event searches:

- `nextsunrise(location|zone, celestialId, calendar[, occurrence])`
- `nextsunset(location|zone, celestialId, calendar[, occurrence])`
- `nextsolarlongitude(location|zone, celestialId, calendar, longitudeDegrees[, occurrence])`
- `nextnewmoon(location|zone, moonId, calendar[, occurrence])`
- `nextfullmoon(location|zone, moonId, calendar[, occurrence])`
- `nextvisiblecrescent(location|zone, sunId, moonId, calendar[, occurrence])`

The optional `occurrence` parameter is the nth next matching event. These functions return `MudDateTime.Never` when the zone, calendar, celestial ephemeris, or bounded event search cannot produce a deterministic result.

FutureProg schedules persist a recurrence, a reference `MudDateTime`, and the target FutureProg. On load, a schedule advances the persisted reference to the next future occurrence and creates an in-memory listener. When the listener fires, the schedule executes the FutureProg, computes the next reference time, creates the next listener, and marks itself changed.

## Player And Builder Surfaces
### Player-Facing
Players primarily interact with time through:

- `time`: current perceived time, date, season, celestial information, and visible timepiece items
- `calendar`: calendar definition or generated year view
- object interactions such as timepieces

The `time` command intentionally routes exactness through checks. A character may receive exact time, vague time, or crude time depending on game mechanics.

### Builder/Admin-Facing
Current builder/admin surfaces include:

- `clock`: list, new/create, edit/open, clone, close, show, and set commands
- `timezone`: list, new/create, edit/open, clone, close, show, and set commands
- `calendar`: list, new, edit/open, clone, close, show, preview, validate, and set commands
- `shard set <shard> clocks <clock...>`
- `shard set <shard> calendars <calendar...>`
- `zone set <zone> timezone <clock> <timezone>`
- `show clocks`
- `show calendars`
- `show timezones [clock]`

Clock, calendar, and mud time-zone editing all use `IEditableItem` and `EditableItemHelperTime.cs`, so they share the standard `list/new/edit/show/set/close/clone` workflow used by other builder-managed engine objects. The legacy `timezone list/create/edit` syntax remains as a compatibility alias, but the generic editing workflow is the canonical surface.

Clock editing supports metadata, display masks, clock units, in-game speed, fixed digit settings, zero-hour behavior, hour intervals, primary time-zone selection, current-time setting through local-time parsing, and crude time interval add/remove/clear.

Time-zone editing supports name/alias, description, and offset hours/minutes. Existing time zones cannot be moved between clocks; builders should clone to the target clock instead.

Calendar editing supports metadata, display masks, feed clock, current date, epoch year and first weekday, era display text, weekday and month editing, normal special/non-weekday days, intercalary days/months, generated-year preview, validation, algorithm selection, day-boundary selection, authority-location editing, and regnal-period editing. Structural edits clear generated-year caches, normalize the current date, and mark the calendar changed.

Calendar display masks support ordinary date tokens plus regnal tokens:

- `$rk`: regnal key with `@`, such as `@charles-iii`
- `$rs`: regnal short name
- `$rf`: regnal full name
- `$rn`, `$rN`, `$rt`, `$rT`: numeric, wordy, ordinal, and wordy ordinal regnal year
- `$rq`: canonical regnal reference, such as `RY20 @charles-iii`; outside a regnal period this falls back to ordinary year/era text
- `$rp`: readable regnal phrase, such as `20th year of King Charles III`; outside a regnal period this falls back to ordinary ordinal year/era text

Masks can branch on whether the displayed date is inside a regnal period with `$ir{<regnal text>}{<fallback text>}`. Branch contents may contain ordinary tokens, regnal tokens, and plain text, for example `$ir{$rt year of $rf}{$yo $EE}`. Nested conditionals are not supported; malformed blocks are left literal.

Regnal period builder commands live under `calendar set regnal ...`:

- `regnal list`
- `regnal add <key> "<short>" "<full>" <start> [<end>|open]`
- `regnal close <key> <end>`
- `regnal name <key> short|full <text>`
- `regnal start <key> <date>`
- `regnal end <key> <date|open>`
- `regnal remove <key>`
- `regnal preview <date>`

Period edits use the normal live-calendar structural confirmation path and validate the full period set before applying.

Admin time displays now expose the current `MudInstant`, calendar algorithm type, day-boundary type, authority location when set, and celestial ephemeris support. Admins can preview deterministic astronomical events with:

```text
time instant
time event sunrise <sun> [occurrence]
time event sunset <sun> [occurrence]
time event solarlongitude <sun> <degrees> [occurrence]
time event newmoon <moon> [occurrence]
time event fullmoon <moon> [occurrence]
time event visiblecrescent <sun> <moon> [occurrence]
```

Builders changing live calendars or clocks should preview and validate before closing the edit session, then watch for Discord fallback notifications after reboot/load cycles. Those notifications identify affected saved objects so humans can repair data that was made invalid by the definition change.

## Seeder Support
`TimeSeeder` installs the initial clock, UTC time zone, and selected stock calendar package.

It asks for:

- in-game seconds per real second
- the calendar mode/package
- starting year
- additional package-specific choices, such as Middle-earth age selection

The seeder includes stock calendars such as Gregorian, Julian, Roman, Tranquility, French Republican, Mission, Seasonal 360, and several Tolkien-inspired calendars.

It also includes deterministic astronomical and historical approximation packages:

- Islamic Hijri
- Hebrew
- Old Persian
- Babylonian
- Chinese Minguo
- Chinese lunisolar
- Korean modern/Dangi
- Korean lunisolar
- Japanese modern/Koki
- Japanese lunisolar

Those packages use Latin1-safe romanised aliases and descriptions. The lunisolar and historical packages are deterministic approximations for engine repeatability; they do not implement manual observation ledgers, local weather-dependent official decisions, or historically variable human authority rulings.

The seeder is repair-capable for canonical stock time data. When a later rerun installs another calendar package, it adds the selected clock/calendar links to existing shards without removing older shard calendar assignments; zone time-zone links are reconciled per clock. Other seeders rely on at least one clock and calendar being present.

Seeder regression tests cover all stock `TimeSeeder` modes, including XML shape, runtime loading of generated clock/calendar rows, non-seven-day week packages, decimal clocks, Middle-earth multi-calendar output, idempotent reruns, and additive calendar/clock package reruns against existing shard/zone bindings.

## Integration Points
The time and date system is used broadly across FutureMUD:

- rooms, zones, and shards expose local calendars, clocks, and time zones
- celestial objects use calendars and clocks for sky positions and cycles
- climate and weather controllers use clocks and time zones for weather progression
- economies use calendars, clocks, and time zones for financial periods and recurring policy events
- clans use calendars and recurring intervals for pay cycles and elections
- shoppers, jobs, property leases, hotels, stables, and estate systems use recurring intervals and mud datetimes
- effects can schedule expiration or behavior with date/time listeners
- FutureProg schedules use recurring intervals and persistent `MudDateTime` references
- player presentation uses clocks, calendars, timepieces, and perception checks

## Invariants And Implementation Notes
These are important rules to preserve when changing the system:

- Clocks must load before calendars.
- Calendars should use their feed clock for current `MudDateTime` unless a caller explicitly supplies another clock.
- Non-primary `MudTime` day rollover must be applied to `MudDate` exactly once.
- A parsed time with an explicit time zone is local wall time in that time zone.
- `MudDateTime.Never` must be safe to copy, compare, convert, display, and use in FutureProg dot references.
- Calendar weekday math must use `Weekdays.Count`, not a hard-coded seven-day week.
- Intercalary non-weekday additions and removals must match both generated month behavior and yearly weekday counts.
- `MudDate` copies must preserve generated-year weekday state.
- `MudTime` construction should go through the named factories so datum conversion and local wall-time construction remain explicit.
- Temporal listeners consume repeat counts only after successful payload firing.
- Ordinal recurrence calculations must use generated calendar data rather than Gregorian month/week assumptions.
- Persistent scheduling should store recurrence/reference data and recreate listeners rather than expecting listeners themselves to persist.
- Stored-value fallback helpers must be used for persisted round-trip strings that may have been invalidated by builder edits.
- Fallback reporting must be no-throw; bad telemetry should never prevent the owning object from loading.
- Missing calendar algorithm/day-boundary XML must continue to load as fixed-month, clock-midnight calendars.
- `MudInstant` must remain additive over existing calendar/clock state; existing persisted mud date/time strings stay valid and are not replaced in-place.
- Calculated and astronomical calendar algorithms must produce deterministic generated years from authored metadata without storing generated years or observation ledgers.
- Regnal period keys are immutable calendar-local identifiers. Display names can change without invalidating canonical `RY<number> @key` input.
- Regnal periods may have no gaps or overlaps after the first regnal period starts. At most one period may be open-ended, and an open-ended period must be last.
- Regnal year one begins on the period start date. Later regnal years begin on that same calendar month/day anniversary where the generated year supports it.
- Regnal display must fall back to ordinary year display whenever a base date is outside every configured regnal period.

## Test Coverage
The normal unit-test suites now include focused coverage for:

- `MudTime` factory validation, parsing, copy behavior, and primary-time day rollover
- `MudDate`, `MudDateTime`, `MudTimeSpan`, calendar weekday math, intercalary weekday edits, and `Never` safety
- recurring interval parsing, descriptions, round-trip text, forward/backward search, high ordinal weekdays, exact-month skipping, and "or last" fallback
- runtime listeners, interval extension helpers, and FutureProg date/time helper functions
- clock, time-zone, and calendar builder command paths for high-value edits
- `MudInstant` storage, ordering, conversion, and legacy backfill
- legacy calendar XML loading without an algorithm element
- regnal period XML loading/saving, legacy empty-period loading, display fallback, `$ir{}` branch behavior, canonical parser forms, future projection, and builder continuity validation
- fixed, calculated, and astronomical calendar algorithm generation
- astronomical event solver nth-next behavior and supported solar/lunar/visible-crescent events
- FutureProg celestial event function registration with nth-next overloads
- stored-value fallback behavior and Discord admin notification payloads
- every seeded clock/calendar package produced by `TimeSeeder`, including runtime loading and idempotent reruns for the new astronomical/historical modes

## Current Limitations
The system is flexible, but there are areas where current support is incomplete or intentionally narrow:

- calendar editing is broad but still deliberately command-oriented; there is no graphical calendar designer or bulk import/export workflow
- time zones are fixed offsets and do not model daylight saving or historical offset changes
- recurring interval text is richer than the legacy compact form, but it remains a focused recurrence grammar rather than a general cron system
- listeners are in-memory runtime helpers, so persistence must be implemented by the owning subsystem
- cross-calendar conversion is based on each calendar's current date anchor, which is useful for live worlds but should be treated carefully for historical absolute chronology
- `MudTimeSpan` month and year components are calendar-like approximations until applied to a concrete `MudDateTime`
- major structural edits rely on fallback notification and human repair rather than a bulk migration wizard
- astronomical calendars currently use deterministic approximations rather than observational or jurisdiction-specific historical calendars
- visible crescent detection is geometric and deterministic; it deliberately ignores manual sightings and weather

## Future Work
Useful extensions include:

- builder-facing calendar import/export tooling for XML definitions
- additional recurrence validation previews and builder-facing schedule explanation tools
- daylight saving and date-bounded time-zone transitions
- a schedule inspection command that shows active listener state alongside persistent recurrence state
- more FutureProg helper functions for calendar math, such as `daysbetween`, `monthstart`, `monthend`, and `weekdayname`
- stronger validation for calendars linked to shards, zones, celestials, weather controllers, economies, and clans before deletion or major edits
- an admin repair/report command that enumerates stored time strings that would currently trigger fallbacks
