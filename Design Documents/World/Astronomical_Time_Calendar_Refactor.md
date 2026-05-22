# Astronomical Time, Calendar, and Celestial Refactor

## Intended Repository Path

`Design Documents/World/Astronomical_Time_Calendar_Refactor.md`

## Purpose

FutureMUD's existing time and date system supports highly configurable fixed calendars: arbitrary month lengths, intercalary days and months, unusual weekday cycles, era text, custom display masks, multiple clocks and time zones. This is sufficient for Gregorian, Julian, French Republican, Tolkien-style, and fantasy calendars.

It is not sufficient for calendars whose date boundaries or month/year structure are derived from astronomical events such as sunset, first crescent visibility, astronomical new moon, winter solstice, solar longitude, or vernal equinox. This document describes a backwards-compatible refactor to add deterministic calculated and astronomical calendar support while preserving all existing MUD data and behaviours.

Manual observation-ledger calendars, staff-confirmed crescent declarations, and weather-dependent official calendar decisions are explicitly out of scope for this phase.

## Non-Negotiable Compatibility Requirements

Existing MUDs must continue to boot and run without rerunning seeders. Existing calendar, clock, timezone, celestial, recurring interval, `MudDate`, `MudTime`, `MudDateTime`, and stored date/time strings must remain loadable. Existing fixed XML calendar definitions without new fields must default to the legacy fixed-month algorithm.

If new persisted data is required, provide both seeded defaults and runtime backfill. Legacy databases must be able to derive missing `MudInstant` anchors from existing clock/calendar state at boot. Prefer additive fields, optional XML elements, and stored-value fallback helpers over destructive migrations.

Existing recurrence semantics must remain compatible. New calendar-day-boundary behaviour must not silently reinterpret legacy recurring intervals.

## Target Architecture

### MudInstant

Introduce `MudInstant` as the absolute in-game time scalar, preferably in `FutureMUDLibrary/TimeAndDate`.

Requirements:
- Stable, comparable, serializable, parseable, and safe to persist.
- Represents absolute in-game seconds/ticks from a deterministic epoch.
- Converts to/from `MudDateTime` when supplied calendar, clock, and timezone context.
- Can be back-calculated from legacy `Calendar.Date` plus current clock time.
- Has round-trip stored text helpers, e.g. `mudinstant:<epoch>:<ticks>` or an equivalent format.
- Handles `Never`/null/sentinel semantics consistently with existing `MudDateTime.Never`.

Suggested tests:
- equality, ordering, round-trip storage
- conversion to/from fixed Gregorian and existing seeded calendars
- back-calculation from legacy data
- interaction with `MudDateTime.Never`

### Calendar Algorithm Separation

Split calendar metadata/presentation from calendar generation logic.

Suggested interfaces:
- `ICalendarAlgorithm`
- `ICalendarAlgorithmFactory` or registry
- `IFixedMonthCalendarAlgorithm`
- `ICalculatedCalendarAlgorithm`
- `IAstronomicalCalendarAlgorithm`

The existing `Calendar.CreateYear`, date parsing, and month/intercalary generation should become or delegate to `FixedMonthCalendarAlgorithm`. XML without an algorithm element must remain fixed-month legacy. Optional new XML may include an algorithm element such as:

```xml
<algorithm type="fixed-months" />
```

Calendar metadata remains on `Calendar`: alias, names, description, display masks, era strings, weekday names, feed clock, current date and validation/display surfaces. The algorithm owns year generation, date-to-instant mapping, instant-to-date mapping, intercalation rules, and algorithm-specific validation.

### Celestial Ephemerides

Add arbitrary-instant celestial query interfaces instead of relying only on current-state calls.

Suggested interfaces:
- `ICelestialEphemeris`
- `ISolarEphemeris`
- `ILunarEphemeris`

Useful members:
- `EclipticLongitudeAt(MudInstant instant)`
- `RightAscensionAt(MudInstant instant)`
- `DeclinationAt(MudInstant instant)`
- `ApparentAltitudeAt(MudInstant instant, GeographicCoordinate observer)`
- `ApparentAzimuthAt(MudInstant instant, GeographicCoordinate observer)`
- `IlluminationAt(MudInstant instant, GeographicCoordinate observer)`
- `PhaseAngleAt(MudInstant instant)` for moons

Refactor existing sun and moon implementations to expose arbitrary-instant calculations while preserving current APIs and seeded celestial compatibility.

### Astronomical Event Solver

Add a deterministic event solver service.

Suggested interface:
`IAstronomicalEventService`

Suggested capabilities:
- next/nth sunrise
- next/nth sunset
- next/nth solar longitude crossing
- next/nth lunar conjunction/new moon
- next/nth full moon
- next/nth deterministic visible crescent approximation

Implementation guidance:
- Use bounded numerical search.
- Coarse sample forward to bracket an event.
- Refine with bisection, secant, or another robust method.
- Enforce maximum search windows and return clear errors.
- Cache derived events where safe.
- Deterministic visible crescent should use geometric thresholds, not weather: sunset condition, moon altitude, moon-sun elongation, and optionally moonset lag.

### Day Boundaries and Authority Locations

Add day-boundary support for calendars whose date does not change at midnight.

Suggested enum:
- `ClockMidnight`
- `FixedClockTime`
- `SunsetAtAuthorityLocation`
- `SunriseAtAuthorityLocation`
- `AstronomicalEvent`

All existing calendars default to `ClockMidnight`.

Astronomical calendars may define an authority location or meridian. Do not make official calendar dates depend on live weather. If a future observation ledger is added, it should layer over deterministic predictions rather than replace this architecture.

Implications:
- Preserve `MudDateTime.Midnight`.
- Add `StartOfCalendarDay` where needed.
- Let recurring intervals explicitly retain legacy clock-day semantics unless configured otherwise.
- Ensure parser/display code does not silently reinterpret legacy dates.

## Calendar Algorithms to Implement

### FixedMonthCalendarAlgorithm

Wraps current behaviour. This is the default for legacy XML.

### TabularLunarCalendarAlgorithm

Alternating 30/29-day lunar months with configurable leap-day cycle. Useful for tabular Hijri and simple lunar calendars.

### CalculatedHebrewCalendarAlgorithm

Modern calculated Hebrew calendar:
- 19-year Metonic cycle
- leap years 3, 6, 8, 11, 14, 17, 19
- molad/dehiyyot/postponement rules
- variable year lengths
- Adar I / Adar II handling
- Latin1-safe romanised names

### SolarEquinoxCalendarAlgorithm

Year start from vernal equinox or another configured solar longitude. Useful for Persian/Solar-Hijri-style deterministic calendars.

### AstronomicalLunarCalendarAlgorithm

Month starts by astronomical new moon or deterministic visible-crescent approximation. Useful for Hijri and Babylonian approximations where historical observation is approximated deterministically.

### EastAsianLunisolarCalendarAlgorithm

Supports traditional Chinese/Korean/Japanese-style lunisolar calendars:
- lunar months begin at deterministic astronomical new moon
- winter-solstice month anchoring
- principal solar terms by solar longitude
- leap month chosen when a month lacks a principal solar term
- unique aliases for leap months
- configurable localisation, epoch, meridian/authority settings, era text, names

## Seeder Requirements

Update `TimeSeeder.cs`:
- prompt text
- validators
- mode switch
- stock alias list
- primary-calendar alias resolution
- setup methods
- tests

Add these modes:
- `islamic-hijri`
- `hebrew`
- `old-persian`
- `babylonian`
- `chinese-minguo`
- `chinese-lunisolar`
- `korean-dangi` or `korean-modern`
- `korean-lunisolar`
- `japanese-koki` or `japanese-modern`
- `japanese-lunisolar`

All stock names, aliases, weekdays, and month names must be Latin1-safe romanisations.

Update `CelestialSeeder.cs` only where the celestial XML/data structure changes. Existing seeded celestials must remain loadable and rerun-idempotent.

If `MudInstant` requires seeded epoch/configuration data, update the appropriate core/foundational seeder and add runtime backfill for existing databases.

## Calendar Modelling Choices

### Islamic Hijri

Use astronomical lunar with deterministic visible-crescent approximation if implemented; tabular lunar is acceptable only as a documented fallback. Prefer sunset day boundary. Month names:
`Muharram`, `Safar`, `Rabi al-Awwal`, `Rabi al-Thani`, `Jumada al-Ula`, `Jumada al-Akhirah`, `Rajab`, `Shaban`, `Ramadan`, `Shawwal`, `Dhu al-Qadah`, `Dhu al-Hijjah`.

### Hebrew

Use calculated Hebrew. Prefer sunset day boundary. Month names:
`Nisan`, `Iyyar`, `Sivan`, `Tammuz`, `Av`, `Elul`, `Tishrei`, `Heshvan`, `Kislev`, `Tevet`, `Shevat`, `Adar`, `Adar I`, `Adar II`.

### Old Persian

Use a documented Old Persian/Zoroastrian-style fixed or solar-equinox model. If using fixed Zoroastrian-style structure, model twelve 30-day months plus epagomenal days. If using equinox, document the inference.

### Babylonian

Use astronomical lunar month starts or regulated lunisolar fallback. Support `Addaru II` and `Ululu II` in the 19-year regulated model. Prefer sunset day boundary. Month names:
`Nisannu`, `Aiaru`, `Simanu`, `Duzu`, `Abu`, `Ululu`, `Tashritu`, `Arahsamnu`, `Kislimu`, `Tebetu`, `Shabatu`, `Addaru`, `Addaru II`, `Ululu II`.

### Chinese, Korean, Japanese

Modern modes are Gregorian-derived civil/era calendars:
- Chinese Minguo
- Korean Dangi or modern civil equivalent
- Japanese Koki or modern civil equivalent

Historical modes use `EastAsianLunisolarCalendarAlgorithm`, with deterministic astronomical new moons, solar terms, winter-solstice anchoring, and leap-month handling. Localise names and eras with Latin1-safe romanisations.

## Builder/Admin Commands

Update clock/calendar/celestial/time command surfaces:
- implementor/admin `time` can show `MudInstant`
- calendar show/preview/validate shows algorithm type
- calendar builder can set/view algorithm where safe
- calendar builder can set/view day boundary and authority location
- celestial show exposes ephemeris capability and relevant parameters
- add event preview command(s), e.g. next sunrise, sunset, new moon, full moon, solar longitude, visible crescent, nth occurrence
- warn before live structural changes that can affect stored dates/schedules

## FutureProg

Expose astronomical event functions.

Suggested functions:
- `nextsunrise(...)`
- `nextsunset(...)`
- `nextsolarlongitude(...)`
- `nextnewmoon(...)`
- `nextfullmoon(...)`
- `nextvisiblecrescent(...)`

Support nth-next via overload or occurrence parameter. Return `MudInstant` if a FutureProg MudInstant type is added; otherwise return `MudDateTime` using supplied calendar/clock/timezone. Invalid celestials, missing ephemeris support, bad occurrence counts, or bounded-search failure must fail cleanly.

## Tests

Required coverage:
- all existing `TimeSeeder` modes still load
- all new modes load and generate valid current dates
- `TimeSeeder` and `CelestialSeeder` reruns remain idempotent
- legacy calendar/celestial XML without new fields loads
- `MudInstant` round-trip/order/conversion/backfill
- fixed-month behaviour unchanged
- Hebrew leap years/months
- Hijri lunar month bounds
- Babylonian leap months
- East Asian leap months and unique parseable aliases
- solar-equinox year boundary
- sunrise/sunset, solar longitude, lunar conjunction, full moon, nth-next event solver
- builder command changes where existing command-test patterns exist
- FutureProg event functions and invalid-argument handling

## Documentation Updates

Update:
- `Design Documents/World/Time_And_Date_System.md`
- celestial design documents
- seeder repeatability docs if behaviour changes
- builder/admin command docs if present
- FutureProg function docs if present

Docs must explain `MudInstant`, compatibility/backfill, calendar algorithm types, deterministic astronomical rules, day boundaries, authority locations, seeded calendar packages, historical approximations, and limitations.

## Suggested Implementation Phases

1. Add `MudInstant` and legacy conversion/backfill support.
2. Add calendar algorithm abstraction and fixed-month wrapper.
3. Add arbitrary-instant celestial ephemeris interfaces.
4. Refactor sun/moon implementations to support arbitrary instants.
5. Add event solver.
6. Add day-boundary and authority-location support.
7. Add calculated/astronomical algorithms.
8. Update seeders and stock calendars.
9. Update builder/admin commands.
10. Add FutureProg functions.
11. Update docs and tests.

## Acceptance Criteria

- Existing databases and stored date/time strings remain compatible.
- Existing fixed calendars behave as before.
- `MudInstant` exists, round-trips, and can be derived from legacy state.
- Calendar algorithms are separated from metadata/presentation.
- Celestials support arbitrary-instant ephemeris queries.
- Event solver deterministically finds required solar/lunar events.
- Required new stock calendars are available and runtime-loadable.
- Builder/admin and FutureProg surfaces expose the new functionality.
- Documentation is updated.
- Relevant targeted builds/tests are run and results are reported.
