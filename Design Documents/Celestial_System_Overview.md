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

`OldSun` is no longer supported. Persisted `OldSun` records now fail explicitly during load instead of silently falling back to legacy behavior.

## Document Map
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

This is why geography matters. The same shard celestial can produce different sky state in different zones at the same moment.

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
