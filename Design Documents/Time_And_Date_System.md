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

- `new MudTime(seconds, minutes, hours, timezone, clock, true)` creates the primary feeder time for a clock.
- `new MudTime(seconds, minutes, hours, timezone, clock, daysOffset)` creates a local wall time already expressed in that time zone.

The boolean constructor with `isprimarytime=false` applies time-zone offset logic. It should be used with care because it treats the supplied components as a datum that needs adjustment. Parser and scheduling code that accepts local wall time should use the `daysOffset` constructor or `Clock.GetTime`.

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

The concrete `Calendar` creates generated `Year` instances as needed. Generated years are cached by year number. Calendar day counts, weekday counts, days-between-year counts, and first-weekday calculations are also cached.

The first weekday of a year is computed from:

- the epoch year
- the first weekday at epoch
- the count of weekday-counting days between the target year and epoch
- the actual number of weekdays in the calendar, not an assumed seven-day week

This is what allows calendars with five-day, six-day, eight-day, or other custom week lengths to work correctly.

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
- times such as `3:15pm`, `15:15:00`, and `15:15:00 UTC`
- optional clock hour interval text
- optional time-zone aliases

When a time zone is specified, the parsed time is treated as local wall time in that time zone. Unknown time zones or meridian/period text are rejected cleanly.

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
- specific weekday
- weekly
- yearly

The parser accepts text like:

```text
every 3 days
every 1 month
every weekday 4
```

For `SpecificWeekday`, `Modifier` is the weekday index in the calendar. For other interval types, `Modifier` is usually zero.

Recurring intervals can:

- describe themselves in player-facing text
- find the next date/time after the current game time
- find the last date/time relative to current game time
- find adjacent date/times around current game time
- create listeners through the `IntervalExtensions` helpers

Weekday intervals use the owning calendar's weekday list and support both forward and backward movement.

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

- `clock`: list, create, edit, clone, close, and show clocks
- `timezone`: list, create, and edit time zones for a clock
- `shard set <shard> clocks <clock...>`
- `shard set <shard> calendars <calendar...>`
- `zone set <zone> timezone <clock> <timezone>`
- `show clocks`
- `show calendars`
- `show timezones [clock]`

There is robust clock editing support through `EditableItemHelper.ClockHelper`. Calendar editing is less builder-complete and is currently more dependent on seeded or XML-defined calendar definitions.

## Seeder Support
`TimeSeeder` installs the initial clock, UTC time zone, and selected stock calendar package.

It asks for:

- in-game seconds per real second
- the calendar mode/package
- starting year
- additional package-specific choices, such as Middle-earth age selection

The seeder includes stock calendars such as Gregorian, Julian, Roman, Tranquility, French Republican, Mission, Seasonal 360, and several Tolkien-inspired calendars.

The seeder is repair-capable for canonical stock time data. Other seeders rely on at least one clock and calendar being present.

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
- Temporal listeners consume repeat counts only after successful payload firing.
- Persistent scheduling should store recurrence/reference data and recreate listeners rather than expecting listeners themselves to persist.

## Current Limitations
The system is flexible, but there are areas where current support is incomplete or intentionally narrow:

- calendar authoring is not as builder-friendly as clock and time-zone authoring
- time zones are fixed offsets and do not model daylight saving or historical offset changes
- `MudTime` has constructor overloads with subtly different offset semantics, so call sites must choose carefully
- recurring interval text is compact but limited; it does not yet support richer expressions like "last day of month" or "third Monday"
- listeners are in-memory runtime helpers, so persistence must be implemented by the owning subsystem
- cross-calendar conversion is based on each calendar's current date anchor, which is useful for live worlds but should be treated carefully for historical absolute chronology
- `MudTimeSpan` month and year components are calendar-like approximations until applied to a concrete `MudDateTime`

## Future Work
Useful extensions include:

- a full calendar builder/editor with validation previews for generated years
- builder-facing calendar import/export tooling for XML definitions
- richer recurring interval grammar and validation messages
- daylight saving and date-bounded time-zone transitions
- clearer `MudTime` construction APIs that distinguish datum conversion from local wall time
- design-time tests for every stock seeded calendar, including non-seven-day weeks and intercalaries
- a schedule inspection command that shows active listener state alongside persistent recurrence state
- more FutureProg helper functions for calendar math, such as `daysbetween`, `monthstart`, `monthend`, and `weekdayname`
- stronger validation for calendars linked to shards, zones, celestials, weather controllers, economies, and clans before deletion or major edits
