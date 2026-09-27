# Authored, geography-independent celestials

`RailSun`, `RailMoon`, `ScriptedSun` and `ScriptedMoon` are apparent sky models. At the same underlying instant they have the same position, source lux, movement, phase and events everywhere. They appear only on shards to which they are attached. Existing room, weather and perception rules still control experienced light and visibility.

These types do not implement physical ephemerides. `Sun`, `PlanetaryMoon`, `PlanetFromMoon` and `SunFromPlanetaryMoon` retain their orbital models and numerical conventions.

## Time of day

There is no authored time-of-day track. The `TimeOfDay` boolean in source JSON means **eligible to supply time of day**. Suns default eligible; moons default ineligible, including when the boolean is omitted during import. Builders may explicitly opt a moon in.

The first eligible celestial in the existing shard order remains authoritative. Adding another eligible object produces a warning and does not reorder the list. Zone thresholds remain unchanged: elevation above `0.05` radians is Morning/Afternoon, below `-0.20944` is Night, and the interval between is Dawn/Dusk. Ascending selects Morning/Dawn; Descending selects Afternoon/Dusk. Physical suns retain their own existing `CurrentTimeOfDay` thresholds.

## Builder workflow

The `celestial` command requires HighAdmin. `help celestial` includes complete syntax; `celestial types` lists presets, units and configured resource limits.

```text
celestial types
celestial list
celestial new RailSun 1 1 The Golden Sun
celestial set rail 1d 360 1080 90 90 180
celestial validate
celestial preview 360 3 180
celestial event sunrise 360 1000000
celestial save
```

The two IDs after a preset are clock and calendar. Use installed names or IDs; their feed clocks must agree. `new` immediately persists a valid preset and opens its private draft. `edit <object>` opens a private copy; `set` edits only that draft, and `close` discards it. `save` prepares the complete candidate, including its owning calendar and live state, before persisting and silently activating it. A rejected edit leaves the previous definition active. Clones receive new object IDs and clear the seeder ownership marker.

For an oblique southern rail:

```text
celestial set rail 1d 360 1080 90 45 180
```

The optional final argument is set bearing, defaulting opposite the rise bearing. Simple rails require opposite endpoints and a perpendicular tilt bearing. Maximum elevation must be greater than zero and at most 90 degrees. Unequal above/below durations and reversed bearings are supported.

For a three-point small circle with non-opposite endpoints:

```text
celestial set circle 1d 360 1080 30 0 30 330
celestial validate
celestial preview 720
```

The arguments are period, rise/set minutes, rise bearing, via bearing/elevation and set bearing. The via point is not an independently timed key. Validation/preview report the derived transit minute and maximum elevation.

For a sparse script:

```text
celestial new ScriptedSun 1 1 The Intermittent Star
celestial set path sparse 20
celestial set key 0 90 -10 Jump
celestial set key 3 90 10 Hold
celestial set key 4 90 10 Jump
celestial set key 5 90 -10 Hold
celestial set key 20 90 -10 Travel
celestial validate
celestial preview 0 7
celestial save
```

`set key` replaces an existing minute. Sparse source explicitly closes at its period with the initial position. Each real key's transition controls its outgoing segment: Travel uses the shortest spherical arc; Hold retains an identical endpoint; Jump holds the old value until the destination minute. Antipodal Travel needs an intermediate waypoint. The closure key's transition is unused; the last real key controls the seam. Dense mode instead requires every minute from zero to period minus one, without a closure entry.

Attach using `shard set <shard> celestials <ids...>`. This existing command replaces the entire attached list; include all objects you intend to retain, in the intended authority order. `LOOK SKY`, celestial targeting/`LOOK <name>`, and `TIME` show visible authored bodies with position and movement; moon descriptions also show phase.

## Independent channels

All authoritative values are held between feed-clock minute boundaries. Periods/offsets are integer minutes; anchors are integer feed-clock seconds on a minute boundary. A `d` suffix on command periods converts days using that clock's actual hours/day and minutes/hour. JSON periods are always minutes. Clock seconds are game seconds, not wall-clock or Unix seconds. The anchor is expressed in the selected calendar's `MudInstant` epoch basis and primary clock datum.

Path, light, phase, annual metadata and event periods are independent. No combined least-common-multiple timetable is built. Negative epochs use floor division/modulo. Unsupported clocks or unconvertible calendar context fail explicitly; same-clock calendar conversion uses the existing time system. Preview identifies clock, calendar, anchor, dimensions and units.

Lighting is exactly one of a clamped elevation profile or independent timeline:

```text
celestial set profile -90 0
celestial set profile -12 0
celestial set profile 0 10
celestial set profile 90 1000
celestial set lighttrack 60
celestial set lightkey 0 0 Travel
celestial set lightkey 30 50 Travel
celestial set lightkey 60 0 Travel
```

Switching to a profile clears the light timeline, and starting a timeline clears the profile. Profile values clamp to the outer supplied knots. Light values are finite, nonnegative source lux; local modifiers are applied by existing room/zone code. The illustrated values are arithmetic examples, not calibrated sunlight. Moon defaults give zero at/below the horizon.

Moon light mode `Absolute` uses the supplied lux directly; `PhaseScaled` multiplies it by the phase illumination fraction exactly once. `set light <mode>` selects the mode.

```text
celestial new RailMoon 1 1 The Silver Moon
celestial set phase regular 28d 0
celestial set phase fixed 0.5
celestial set phase authored 100
celestial set phasekey 0 0 Travel
celestial set phasekey 20 0.5 Hold
celestial set phasekey 40 0.5 Travel
celestial set phasekey 100 1 Travel
```

Phase turns wrap to `[0,1)`: zero is Full and one half is New, with illumination `(1 + cos(2*pi*q))/2`. The existing named phase bands are preserved. Authored phases interpolate unwrapped turns and support reversals, holds and jumps. Closure may retain an integer winding; each Travel segment permits at most one turn. A fixed phase has no recurring full/new milestone. Entering a landmark hold counts once; jumps count only a landing on the exact landmark, not a skipped phase.

`set year <period>` controls annual day metadata separately from the path. `set longitude <degrees-at-epoch>` enables explicitly synthetic uniform annual longitude; `set longitude off` disables it. It does not infer seasons from the sky path or change weather bindings.

## Events and narrative

```text
celestial set milestone custom:defeated 1d 600
celestial set echo defeated custom:defeated SkyVisible 0 The eastern radiance falters.
celestial set scheduled stir 1d 300 SkyVisible 1 Pale light stirs in the east.
celestial set threshold sunrise 0 Ascending BodyVisible 0 The golden sun rises.
celestial set crescent 51 custom:crescent
celestial set crescent off
```

Create a referenced milestone before validating an echo/crescent association. Threshold echoes apply to rails. Echo IDs are unique; multiple different IDs at the same minute are valid and execute in `Order`, then ordinal ID order. Named milestones use `custom:` keys and have no implicit canonical meaning. `set remove` removes a selected key, profile knot, milestone or echo.

BodyVisible requires a visible body at/above the horizon. SkyVisible permits below-horizon narrative but still respects the recipient's perception, sky-obscuring weather and outdoors/windows state. Window recipients receive the existing outside prefix. Zero source lux alone does not override normal perception rules.

Echo batches are computed once per normal consecutive minute and delivered once per subscribed zone. Each zone receives an independent mutable wrapper. Loading, edits, explicit clock/calendar changes, bulk jumps, historical queries and previews are silent. Rewinding does not replay missed events; a later normal traversal may deliver them again. Detachment unregisters the old zone subscriptions; disposal removes clock subscriptions.

Rise/set events use authoritative samples. Strict opposite signs separated by a zero plateau produce one crossing at the first zero; a zero touch returning to the same side produces none. Jumps cross at the destination minute. Always-above, always-below and whole-horizon objects are valid and return NoFutureOccurrence promptly.

Queries are **strictly after** the reference. Sorted event offsets and checked cycle/rank arithmetic select nth results directly. Sparse compilation uses monotonic subdivisions and bounded root searches, never scanning all empty minutes. `celestial preview` lists sampled state and bounded canonical/named next results; `celestial event` selects a specific event, occurrence, longitude or crescent-associated sun and reports unavailable status. Neither delivers echoes.

An authored moon's visible-crescent events require explicit markers, a matching stable sun ID, valid solar/lunar roles and compatible clocks. This is narrative authoring; no additional sunset, phase, separation, weather or altitude test is imposed. An authored sun with a physical moon has no invented crescent capability. Physical pairs retain the physical solver. Authored geometric conjunction is Unsupported.

## Public consumers

`IAuthoredCelestial` supplies pure apparent state, capabilities and structured event results. `ILunarPhase` is shared by physical and authored moons. `ICelestialTimeContext` supplies canonical calendar/clock context.

`AstronomicalEventService.TryFindNext` retains the physical entry point. The distinct `FindNextForCelestial`/`TryFindNextForCelestial` route dispatches authored capability directly, without fabricated RA/declination or orbital fallback. Diagnostics distinguish Found, NoFutureOccurrence, Unsupported, InvalidRequest, IncompatibleTimeContext, OutOfRange and bounded physical SearchLimitReached.

Existing location/zone and object/ID FutureProg variants work through this route: `nextsunrise`, `nextsunset`, `nextnewmoon`, `nextfullmoon`, `nextsolarlongitude`, `nextvisiblecrescent`. `nextcelestialevent(locationOrZone, celestialOrId, calendar, "custom:key" [, occurrence])` queries named events. `moonphase` uses phase capability; `celestialelevation` retains its existing lookup/missing-object behavior. Invalid IDs/counts, unavailable capabilities or conversion failures return the existing Never value for event functions. IDs and occurrences must be positive integers; counts retain the existing Int32 bound.

Calendar sunrise/sunset day boundaries use the object route. A boundary exactly at midnight is included without changing public strictly-next semantics. Calendar date shifts resynchronise authored bodies silently; a calendar cannot switch to an incompatible clock while it owns authored bodies.

## Persistence, resources and examples

The existing `Celestials` row contains type, feed-clock ID and versioned JSON `Definition`; object/shard IDs remain stable. Version 1 stores source only, with stable enum names and invariant numbers. Compiled coefficients, cursors, zone wrappers and delivered-event state are not persisted. Migration `20260926115932_WidenCelestialDefinition` widens `Definition` from `TEXT` to `LONGTEXT` in place, retaining all row data, required-value constraints, character set and collation. Existing physical XML definitions need no conversion. The blank database snapshot includes this migration.

Dense authoring stores one sample per feed-clock minute: 1,440 entries cover a conventional day and 40,000 cover about 27.8 days. A month of irregular authored motion is a plausible use, although regular motion usually needs only a compact rail or sparse path. Even a single dense day exceeds the former 65,535-byte `TEXT` limit. `LONGTEXT` accommodates the default 16 MiB budget, including its exact upper boundary; MySQL `MEDIUMTEXT` ends one byte below that boundary. See [MySQL string types](https://dev.mysql.com/doc/refman/8.0/en/string-type-syntax.html).

`celestial export` displays source text. `celestial import` opens the normal editor (`@` submits, `*cancel` cancels), parses and validates a private candidate, and leaves activation to `save`. It accepts JSON, not filesystem paths or XML/DTDs. Unknown versions, types and fields, null channels/entries, invalid geometry/closure/phase/light/reference data and checked overflow are rejected explicitly. Load errors identify the celestial and Definition.

| Static configuration | Default | Meaning |
| --- | ---: | --- |
| `AuthoredCelestialSourceBytes` | 16,777,216 (16 MiB) | Maximum serialized UTF-8 source bytes; configurable independently of the `LONGTEXT` capacity |
| `AuthoredCelestialEntries` | 100,000 | Combined supplied entries before compiled arrays are allocated |
| `AuthoredCelestialPreviewSamples` | 10,000 | Maximum sampled rows per preview |
| `AuthoredCelestialPreviewEvents` | 1,000 | Maximum next-event rows per preview |

The compiler and production load/edit/seeding share the 16 MiB default source budget. Explicit builder-configured limits are preserved; increasing the setting must also respect the server's packet and memory limits. These limits constrain source and output, not sparse duration or recurrence rank. Dense data can reach the source-byte limit before the entry limit. Downgrading to the old `TEXT` schema requires every definition to fit 65,535 bytes; export and shorten larger definitions before attempting that rollback.

The optional CelestialSeeder authored package installs RailSun, RailMoon, ScriptedSun, ScriptedMoon, MorningStar and MorningGlow examples. `installauthored` defaults to no interactively; Debug replay profiles explicitly include yes and `authoredcalendar`. All six remain unattached. `SeederPreset` identifies managed examples: reruns add missing members and preserve existing definitions, names and stable IDs. Builder clones clear this marker. MorningStar rises only to two degrees, so it produces Dawn/Dusk without Morning/Afternoon; MorningGlow stays below the horizon and uses explicit glow lighting and sky narrative.

See [verification coverage](Celestial_System_Tests.md), [seeder contract](Celestial_System_Seeder.md), and the [implementation handover](Authored_Celestials_Handover.md) for measured results and native transcripts.
