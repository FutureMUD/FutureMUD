# FutureMUD Celestial System Overview

## Purpose
The celestial system answers a specific world-simulation question: "what is in the sky from here, right now?"

In FutureMUD, celestial objects are not just lore labels. They are runtime objects that:

- convert a shard and zone's geography into apparent sky position
- expose elevation and azimuth for a specific observer latitude and longitude
- contribute illumination to zone light levels
- emit movement and threshold triggers as time advances
- provide player-facing descriptions for commands like `time`
- give weather and seasonal systems a consistent astronomical reference

The core design goal is geography-first sky positioning. A sun, moon, or planet is only meaningful in play once the engine can answer where it appears relative to a particular zone's `GeographicCoordinate`.

## Document Map
- [Celestial System Seeder](./Celestial_System_Seeder.md) explains the supported stock authoring path in `DatabaseSeeder`.
- [Celestial System Tests](./Celestial_System_Tests.md) explains the automated coverage split and current gaps.
- [Celestial Type: OldSun](./Celestial_Type_OldSun.md) documents the legacy solar implementation that is still loadable for existing worlds.
- [Celestial Type: Sun](./Celestial_Type_Sun.md) documents the current `NewSun` implementation and `SunV2` seeder format.
- [Celestial Type: PlanetaryMoon](./Celestial_Type_PlanetaryMoon.md) documents moons viewed from the parent planet.
- [Celestial Type: PlanetFromMoon](./Celestial_Type_PlanetFromMoon.md) documents parent planets viewed from a moon.
- [Celestial Type: SunFromPlanetaryMoon](./Celestial_Type_SunFromPlanetaryMoon.md) documents the moon-local representation of a linked root sun.

## Core Runtime Shape
At the interface level, a celestial is anything implementing `ICelestialObject`. The important outputs are:

- `CurrentElevationAngle(GeographicCoordinate geography)`
- `CurrentAzimuthAngle(GeographicCoordinate geography, double elevationAngle)`
- `CurrentIllumination(GeographicCoordinate geography)`
- `CurrentPosition(GeographicCoordinate geography)`
- `CurrentTimeOfDay(GeographicCoordinate geography)`
- `MinuteUpdateEvent`

`CelestialInformation` is the cached per-location result. It stores:

- the originating celestial
- the last azimuth angle
- the last ascension angle, which is effectively the local elevation/altitude angle in this subsystem's naming
- the movement direction, ascending or descending

That cached object is what rooms, zones, and player-facing commands actually consume most of the time.

## Shards, Zones, Cells, and Viewers
The ownership and application model is:

| Layer | Responsibility |
| --- | --- |
| `Shard` | Chooses which celestial objects exist for that world-space body or plane |
| `Zone` | Applies geography, timezone, and weather context to the shard's celestials |
| `Cell` / room | Exposes zone and shard celestial data to gameplay surfaces |
| Viewer | Sees descriptions and time-of-day consequences derived from the zone cache |

The important relationship boundaries are:

- Shards own the celestial list through `ShardsCelestials`.
- `Zone.Celestials` delegates to `Shard.Celestials`.
- Zones apply `GeographicCoordinate` values such as latitude, longitude, elevation, and the shard radius.
- Cells and locations surface that zone-specific data to players and systems.

This means a shard decides which sky objects exist, while a zone decides how they appear from a particular place.

## Celestial Flow Through the Runtime
The main runtime flow is:

1. A shard is assigned one or more celestial objects.
2. Each zone in that shard calls `InitialiseCelestials()`.
3. The zone subscribes to every celestial's `MinuteUpdateEvent`.
4. The zone computes an initial `CelestialInformation` entry for each celestial at the zone's geography.
5. The zone computes a per-celestial illumination contribution and stores it in `LightLevelDictionary`.
6. The zone recalculates total light level from celestial illumination plus ambient light pollution and other zone context.
7. As clocks advance, celestial objects raise minute updates.
8. The zone refreshes cached `CelestialInformation`, refreshes illumination, and recalculates light again.

Because zones cache this data, sky queries during normal gameplay are cheap. The expensive orbital math happens when time changes or when a system explicitly recomputes.

## Celestial Information and Light
Celestials do more than provide descriptive text. Their illumination feeds directly into zone lighting and therefore into:

- ambient visibility
- `DescribeSky`
- room and outdoor presentation
- weather and season heuristics that care about day versus night
- player perception of dawn, dusk, sunrise, moonrise, and similar transitions

The zone owns the current light level because illumination is geography-dependent. The same shard celestial can therefore produce different apparent conditions in two zones on the same shard at the same moment.

## One Physical Body, Multiple Game Representations
A single physical astronomical object may have multiple game representations.

This is intentional and central to the subsystem.

Examples:

- A root `Sun` represents a star from the point of view of a planetary surface.
- `SunFromPlanetaryMoon` represents that same star from the point of view of an observer standing on a moon.
- `PlanetaryMoon` represents the moon from a planet observer's sky.
- `PlanetFromMoon` represents the parent planet from the moon observer's sky.

These are not duplicate astrophysical bodies. They are alternate observer-frame representations that let the engine answer different local-sky questions without forcing every celestial type into one giant generic model.

This matters when seeding and testing:

- linked representations must stay synchronized
- authoring one representation does not automatically make another visible on a shard
- weather and season systems should bind to the representation that matches the intended world frame

## Link Between Celestials and Shards
Builders attach celestials to shards with:

- `shard set <shard> celestials <celestial1> ... [<celestialn>]`

That command controls which celestial objects a shard exposes. When the shard's celestial list changes:

- existing zones deregister old celestial subscriptions
- the shard's editable celestial list is replaced
- zones reinitialise their celestial caches

This is the point where seeded celestial definitions become live world-space content.

## Link Between Celestials and Zones
Zones do not own independent celestial lists. They inherit from the shard and then provide the context that makes the shard's celestials meaningful:

- latitude
- longitude
- elevation
- shard spherical radius
- timezone per clock
- ambient light pollution
- weather controller assignment

The main builder workflow is:

- create or edit the shard and assign celestials
- create or edit zones in that shard
- set zone latitude, longitude, elevation, and timezones
- attach a weather controller if the zone needs one

Useful commands include:

- `zone set <which> latitude <degrees>`
- `zone set <which> longitude <degrees>`
- `zone set <which> elevation <amount>`
- `zone set <which> timezone <clock> <tz>`
- `zone set <which> weather <wc>`

Without zone geography, a celestial can still exist in the database, but it cannot produce meaningful sky positions for gameplay.

## Interaction With Weather Controllers
Weather controllers use a celestial as their astronomical anchor.

The main builder command is:

- `wc set celestial <which>`

That celestial provides the runtime concept of day versus night for the controller. The controller also keeps a `GeographyForTimeOfDay`, so weather logic can ask the chosen celestial for `CurrentTimeOfDay(...)` using an explicit geography rather than an arbitrary room.

This is why picking the correct representation matters:

- a planetary weather controller should usually bind to a root `Sun`
- a moon-based climate or analysis context may need the moon-local representation instead

## Interaction With Seasons
Seasons also bind to a celestial:

- `season set celestial <id|name>`

That binding tells the seasonal logic which sky object defines the relevant astronomical cycle for the season record. In ordinary Earth-like worlds this is typically the planetary `Sun`, but the system is flexible enough to support other contexts.

## Player-Facing Interaction
The player-facing `time` command pulls the current location's celestial list and cached celestial information. It reports:

- the current time of day
- visible celestial descriptions
- time and date information from visible calendars and clocks

In practice this means celestial objects affect what players are told about the world every day, not just admin tooling.

## Implementor and Analysis Tooling
The celestial system also has explicit implementor/debug surfaces:

- `celestials` runs a long-form celestial analysis/debug pass
- weather analysis tools flatten linked celestial graphs so that derived objects are included in simulation context

This is especially important for linked types like `PlanetFromMoon` and `SunFromPlanetaryMoon`, because they depend on root objects and should be treated as part of one connected astronomical graph during analysis.

## Interaction With Other MUD Systems
The celestial system currently intersects with other systems in the following ways:

| System | Interaction |
| --- | --- |
| Shards | Choose which celestials exist in a world-space context |
| Zones | Apply geography, timezones, and light recalculation |
| Weather controllers | Use one celestial root to determine time-of-day behavior |
| Seasons | Bind to a celestial for seasonal progression |
| Time/date presentation | `time` command reports visible celestial state |
| Perception and room presentation | Sky descriptions and outdoor echoes depend on celestial cache and visibility |
| Ambient light | Zone light levels include celestial illumination |
| Implementor analysis | Debug and weather analysis tools inspect or flatten celestial graphs |

## Builder Workflow Summary
The usual end-to-end builder workflow is:

1. Seed stock celestial definitions or hand-author custom ones.
2. Attach the chosen celestials to a shard with `shard set <shard> celestials ...`.
3. Create or edit zones in that shard.
4. Configure zone latitude, longitude, elevation, and timezones.
5. Attach weather controllers to zones and bind controllers to the appropriate celestial.
6. Bind seasons to the same intended celestial frame.
7. Use `time` and implementor analysis commands to verify the sky behaves as intended.

## Current Supported Loadable Types
The current loadable celestial families are:

- `OldSun`, loaded by the legacy `Sun` class
- `Sun`, loaded by `NewSun`
- `PlanetaryMoon`
- `PlanetFromMoon`
- `SunFromPlanetaryMoon`

The remainder of this documentation suite describes each type in more detail, including property mapping and the exact calculation pipelines used by the runtime.
