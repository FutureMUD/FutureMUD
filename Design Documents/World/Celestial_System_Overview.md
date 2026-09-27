# FutureMUD Celestial System Overview

## Purpose
The celestial subsystem answers one runtime question: "what is in the sky from this geography, at this calendar date and clock time?"

Celestial objects are not lore-only records. They drive:

- local elevation and azimuth
- zone illumination
- dawn, dusk, rise, and set triggers
- time-of-day classification
- player-facing sky and `time` output
- weather and seasonal astronomy anchors

## Supported Loadable Types
The supported modern celestial types are:

- `Sun`, loaded by `NewSun`
- `PlanetaryMoon`
- `PlanetFromMoon`
- `SunFromPlanetaryMoon`
- `RailSun`, `RailMoon`, `ScriptedSun`, `ScriptedMoon` (authored apparent sky models)

`OldSun` is no longer supported. Persisted `OldSun` records now fail explicitly during load instead of silently falling back to legacy behavior.

## Document Map
- [Authored, geography-independent celestials](./Authored_Celestials.md)
- [Celestial System Seeder](./Celestial_System_Seeder.md)
- [Celestial System Tests](./Celestial_System_Tests.md)
- [Celestial Type: Sun](./Celestial_Type_Sun.md)
- [Celestial Type: PlanetaryMoon](./Celestial_Type_PlanetaryMoon.md)
- [Celestial Type: PlanetFromMoon](./Celestial_Type_PlanetFromMoon.md)
- [Celestial Type: SunFromPlanetaryMoon](./Celestial_Type_SunFromPlanetaryMoon.md)

## Core Runtime Shape
At the interface level, a celestial is anything implementing `ICelestialObject`. The main runtime outputs are:

- `CurrentElevationAngle(GeographicCoordinate geography)`
- `CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle)`
- `CurrentIllumination(GeographicCoordinate geography)`
- `CurrentPosition(GeographicCoordinate geography)`
- `CurrentTimeOfDay(GeographicCoordinate geography)`

`CelestialInformation` is the cached zone-facing result. It stores:

- the source celestial
- the last azimuth angle
- the last local altitude/elevation angle
- the current movement direction

Some celestial types also expose arbitrary-instant ephemeris interfaces for calendar and FutureProg event solving:

- `ICelestialEphemeris`
- `ISolarEphemeris`
- `ILunarEphemeris`

`Sun`/`NewSun` implements the solar ephemeris. `PlanetaryMoon` implements the lunar ephemeris. These interfaces answer right ascension, declination, apparent altitude/azimuth, illumination, ecliptic longitude, and lunar phase angle for any supplied `MudInstant` rather than only the current clock/calendar state.

`AstronomicalEventService` provides deterministic bounded searches for:

- sunrise
- sunset
- solar longitude crossings
- lunar conjunction/new moon
- full moon
- visible crescent approximation
- nth-next occurrence of each supported event

The solver is intentionally deterministic and engine-local. Visible crescent detection uses geometric thresholds at sunset; it does not consult manual observation ledgers or weather-dependent official calendar decisions.

## Observer Frames
The subsystem deliberately models observer frames rather than only physical bodies.

Examples:

- `Sun` is a star as seen from a planetary surface
- `PlanetaryMoon` is a moon as seen from the parent planet
- `PlanetFromMoon` is the parent planet as seen from the moon
- `SunFromPlanetaryMoon` is the same root star transformed into the moon observer frame

That split is intentional. It keeps each type focused and lets weather, zones, and builders bind to the frame that matches the intended world.

## Runtime Flow
The normal runtime flow is:

1. Builders or seeders create celestial definitions.
2. Shards attach one or more celestials.
3. Zones apply geography, timezone, and weather context.
4. Zones cache `CelestialInformation` and illumination per celestial.
5. Minute updates refresh the cache and recalculate zone light.

Physical models use geography and can produce different sky state in different zones at the same moment. Authored models share one intrinsic minute snapshot globally; per-zone wrappers, illumination modifiers and perception remain local. The first eligible celestial and existing elevation/direction thresholds still determine zone time of day.

## Builders and Integration
Builders attach celestials to shards with `shard set <shard> celestials ...`.

Zones then provide the local context that makes the celestial meaningful:

- latitude
- longitude
- elevation
- shard radius
- timezone
- ambient light and weather

Weather controllers and seasons bind to the celestial that defines their astronomy frame. For ordinary planetary worlds this is usually a root `Sun`. Moon-local climates or analysis contexts can instead bind to the linked moon-frame representation.

## FutureProg Integration

FutureProgs can inspect the cached sky state with:

- `celestialelevation(location, celestialId) -> number`
- `celestialelevation(zone, celestialId) -> number`

The function looks up the celestial by ID in the supplied room's zone or the supplied zone and returns the current elevation angle in radians. Positive values are above the horizon and negative values are below it. If the zone or celestial cannot be found, the function returns `0`.

This is intended for environmental gating such as sunlight-sensitive combat moves, outdoor ceremonies, or weather/season logic that needs to know whether a sun-like celestial is high enough above the horizon.

FutureProgs can also ask for deterministic event times as `MudDateTime` values:

- `nextsunrise(location|zone, celestialId, calendar[, occurrence])`
- `nextsunset(location|zone, celestialId, calendar[, occurrence])`
- `nextsolarlongitude(location|zone, celestialId, calendar, longitudeDegrees[, occurrence])`
- `nextnewmoon(location|zone, moonId, calendar[, occurrence])`
- `nextfullmoon(location|zone, moonId, calendar[, occurrence])`
- `nextvisiblecrescent(location|zone, sunId, moonId, calendar[, occurrence])`
- `nextcelestialevent(location|zone, celestialId, calendar, "custom:key"[, occurrence])`

These functions also accept resolved celestial objects. They project the found `MudInstant` through the supplied calendar, clock and local zone timezone. Physical pairs use the existing bounded ephemeris solver. Authored capabilities use independent minute-sampled tracks and direct cycle/rank arithmetic, ignoring observer geography. Unavailable events, invalid arguments and incompatible time contexts return `MudDateTime.Never`. `moonphase(location|zone)` selects the first celestial exposing lunar phase capability. See [Authored Celestials](Authored_Celestials.md) for synthetic longitude and explicitly associated crescent markers.
