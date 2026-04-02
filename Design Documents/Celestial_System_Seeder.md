# FutureMUD Celestial System Seeder

## Purpose
`DatabaseSeeder/Seeders/CelestialSeeder.cs` is the supported stock authoring path for new celestial content shipped with the engine.

Its job is not to solve every astronomy use case. Its job is to provide coherent, repeatable, linked stock packages that:

- create working celestial definitions
- keep linked moon-view representations synchronized
- remain safe to install on reruns
- give builders a trustworthy starting point before they attach celestials to shards in game

## Package Model
The current seeder supports three stock packages.

| Package | Purpose | Self-contained | Objects created |
| --- | --- | --- | --- |
| `EarthSun` | Earth-facing root sun package | Yes | `Sun` |
| `EarthMoonView` | Earth moon-view package | No, requires Earth-facing sun | `PlanetaryMoon`, `PlanetFromMoon`, `SunFromPlanetaryMoon` |
| `GasGiantMoonView` | Ganymede/Jupiter/Sol package | Yes | Jupiter-facing `Sun`, `PlanetaryMoon`, `PlanetFromMoon`, `SunFromPlanetaryMoon` |

The package model is explicit. The seeder no longer relies on incidental side effects to create linked moon-view objects.

## Earth-Facing Sun Package
The Earth-facing sun package creates the modern `Sun` implementation using the `SunV2` data shape.

The seeder asks for:

- whether to install the Earth-facing sun package
- the calendar that should feed the celestial
- the celestial name
- the epoch date to use for the orbital data

This package is intended for planetary-surface gameplay and is the required root for the Earth moon-view package.

## Earth Moon-View Package
The Earth moon-view package is a linked package representing the Earth-Moon-Sun relationship from two observer frames.

It creates:

- a `PlanetaryMoon` representing Earth's Moon as seen from Earth
- a `PlanetFromMoon` representing Earth as seen from the Moon
- a `SunFromPlanetaryMoon` representing the same physical sun as the root Earth-facing `Sun`, but transformed into the Moon observer's frame

The seeder asks for:

- whether to install the Earth moon-view package
- the calendar for the moon package
- the moon name
- the full moon epoch date

This package is strict about root-sun resolution:

- if the same seeder run just created the Earth-facing `Sun`, that new object is used
- otherwise the seeder searches for exactly one matching Earth-facing sun candidate already in the database
- if no candidate exists, installation is blocked
- if multiple candidates exist, installation is blocked rather than guessed

That strictness exists to keep the linked moon-view objects deterministic.

## Gas Giant Moon Package
The gas giant package is self-contained and does not depend on the Earth-facing sun package.

It creates:

- a Jupiter-facing root `Sun`
- a `PlanetaryMoon` named `Ganymede`
- a `PlanetFromMoon` named `Jupiter`
- a `SunFromPlanetaryMoon` named `Sol`

The seeder asks for:

- whether to install the gas giant package
- the calendar for the package
- the epoch date for the Jupiter-facing sun
- the epoch date for the Ganymede moon phase reference

This package exists because a sun object in FutureMUD is a surface-observer representation, not a universal "one star for the whole solar system" singleton. Jupiter therefore needs its own root `Sun` data.

## How Calendars, Clocks, and Epochs Are Chosen
The seeder resolves the selected calendar first. The celestial then uses the selected calendar's `FeedClockId`.

This means:

- the calendar controls the date progression used by the celestial
- the feed clock controls the time fraction used by orbital math
- the zone timezone determines how that clock is interpreted locally when gameplay asks a zone for time

Epoch dates are author-supplied seeder answers rather than fixed engine constants. The seeder stores:

- the chosen epoch date as the celestial's `EpochDate`
- the orbital constants associated with the package
- the numeric day number at epoch used by the runtime formulae

For the stock packages, the numeric day number at epoch is J2000-based `2451545.0`.

## Package Markers and Repeatability
The seeder is intentionally safe to run more than once.

Each supported stock definition is marked in its XML with:

- `SeederPackage`
- `SeederRole`

Those markers let the seeder distinguish:

- package identity
- object role inside a package
- new-style seeded packages from older content

Repeatability is not based only on object counts. The seeder also includes legacy compatibility checks, especially for the original Earth-facing sun and Earth moon package, so that older worlds can still be recognized as containing equivalent stock content even if they predate the marker system.

## ShouldSeedData Behavior
`ShouldSeedData` currently reports one of the normal seeder states.

For celestial content the important outcomes are:

| Result | Meaning |
| --- | --- |
| `ReadyToInstall` | None of the supported stock packages are present |
| `ExtraPackagesAvailable` | Some, but not all, supported stock packages are present |
| `MayAlreadyBeInstalled` | All supported stock packages are present |

This allows the seeder to behave additively over time as new stock celestial packages are introduced.

## Builder Implications
The seeder only creates celestial definitions in the database. It does not attach them to a shard automatically.

After seeding, builders still need to:

1. attach the created celestial objects to a shard with `shard set <shard> celestials ...`
2. configure zones in that shard with the right geography and timezones
3. bind weather controllers with `wc set celestial <which>`
4. bind seasons with `season set celestial <id|name>`

Important package implications:

- the Earth moon package depends on a matching Earth-facing root sun
- the gas giant package includes its own root sun and is self-contained
- `PlanetFromMoon` and `SunFromPlanetaryMoon` are linked representations and should remain on the same shard as their companion root objects unless a custom world design deliberately wants otherwise

## Stock Data Reference
The following table records the stock constants that ship today.

### Earth-facing Sun

| Field | Value |
| --- | --- |
| `CelestialDaysPerYear` | `365.24` |
| `MeanAnomalyAngleAtEpoch` | `6.24006` |
| `AnomalyChangeAnglePerDay` | `0.017202` |
| `EclipticLongitude` | `1.796595` |
| `EquatorialObliquity` | `0.409093` |
| `DayNumberAtEpoch` | `2451545.0` |
| `SiderealTimeAtEpoch` | `4.889488` |
| `SiderealTimePerDay` | `6.300388` |
| `PeakIllumination` | `98000.0` |
| `AlphaScatteringConstant` | `0.05` |
| `BetaScatteringConstant` | `0.035` |
| `PlanetaryRadius` | `6378.0` |
| `AtmosphericDensityScalingFactor` | `6.35` |

### Earth Moon Package

| Object | Field | Value |
| --- | --- | --- |
| `PlanetaryMoon` | `CelestialDaysPerYear` | `29.530588` |
| `PlanetaryMoon` | `MeanAnomalyAngleAtEpoch` | `2.355556` |
| `PlanetaryMoon` | `AnomalyChangeAnglePerDay` | `0.228027` |
| `PlanetaryMoon` | `ArgumentOfPeriapsis` | `5.552765` |
| `PlanetaryMoon` | `LongitudeOfAscendingNode` | `2.18244` |
| `PlanetaryMoon` | `OrbitalInclination` | `0.0898` |
| `PlanetaryMoon` | `OrbitalEccentricity` | `0.0549` |
| `PlanetaryMoon` | `DayNumberAtEpoch` | `2451545.0` |
| `PlanetaryMoon` | `SiderealTimeAtEpoch` | `4.889488` |
| `PlanetaryMoon` | `SiderealTimePerDay` | `0.228027` |
| `PlanetaryMoon` | `FullMoonReferenceDay` | `0.0` |
| `PlanetFromMoon` | `AngularRadius` | `0.0165` |

The Earth `PlanetFromMoon` is named `Earth`, and its linked `SunFromPlanetaryMoon` uses the linked root sun's actual name rather than a hard-coded label.

### Gas Giant Moon Package

| Object | Field | Value |
| --- | --- | --- |
| Jupiter-facing `Sun` | `CelestialDaysPerYear` | `10475.8818867393` |
| Jupiter-facing `Sun` | `MeanAnomalyAngleAtEpoch` | `0.343270671018783` |
| Jupiter-facing `Sun` | `AnomalyChangeAnglePerDay` | `0.000599776264672575` |
| Jupiter-facing `Sun` | `EclipticLongitude` | `0.257060466847075` |
| Jupiter-facing `Sun` | `EquatorialObliquity` | `0.0546288055874225` |
| Jupiter-facing `Sun` | `DayNumberAtEpoch` | `2451545.0` |
| Jupiter-facing `Sun` | `SiderealTimeAtEpoch` | `4.97331570355784` |
| Jupiter-facing `Sun` | `SiderealTimePerDay` | `6.28378508344426` |
| Jupiter-facing `Sun` | `KepplerC1Approximant` | `0.0967569879178372` |
| Jupiter-facing `Sun` | `KepplerC2Approximant` | `0.0029273119273445` |
| Jupiter-facing `Sun` | `KepplerC3Approximant` | `0.000122772356038737` |
| Jupiter-facing `Sun` | `PeakIllumination` | `3619.8` |
| Jupiter-facing `Sun` | `PlanetaryRadius` | `69911.0` |
| `PlanetaryMoon` (`Ganymede`) | `CelestialDaysPerYear` | `7.16683557838692` |
| `PlanetaryMoon` (`Ganymede`) | `MeanAnomalyAngleAtEpoch` | `0.82944303060139` |
| `PlanetaryMoon` (`Ganymede`) | `AnomalyChangeAnglePerDay` | `0.878153082764442` |
| `PlanetaryMoon` (`Ganymede`) | `ArgumentOfPeriapsis` | `4.87986552021969` |
| `PlanetaryMoon` (`Ganymede`) | `LongitudeOfAscendingNode` | `5.96372010319795` |
| `PlanetaryMoon` (`Ganymede`) | `OrbitalInclination` | `0.0355727108921961` |
| `PlanetaryMoon` (`Ganymede`) | `OrbitalEccentricity` | `0.00158762974782861` |
| `PlanetaryMoon` (`Ganymede`) | `DayNumberAtEpoch` | `2451545.0` |
| `PlanetaryMoon` (`Ganymede`) | `SiderealTimeAtEpoch` | `4.889488` |
| `PlanetaryMoon` (`Ganymede`) | `SiderealTimePerDay` | `0.878153082764442` |
| `PlanetaryMoon` (`Ganymede`) | `FullMoonReferenceDay` | `0.0` |
| `PlanetFromMoon` (`Jupiter`) | `AngularRadius` | `0.0653594916439514` |

The gas giant package names the moon-local solar representation `Sol`.

## Linked Object IDs
The seeder creates linked celestial packages deterministically:

- `PlanetFromMoon` stores the ID of its source `PlanetaryMoon`
- `PlanetFromMoon` also stores the ID of its linked root `Sun`
- `SunFromPlanetaryMoon` stores both the source moon ID and the linked root sun ID

This keeps the package coherent even across reruns or later shard assignment.

## Implementation Notes for Future Seeder Additions
When adding a new stock celestial package, keep the following rules:

1. Decide whether the package is self-contained or depends on an existing root celestial.
2. Mark every seeded object with a package name and a role.
3. Store linked object IDs explicitly in the derived object definitions.
4. Reuse illumination from the root `Sun` when creating `SunFromPlanetaryMoon` unless there is a deliberate reason not to.
5. Add `ShouldSeedData` detection for both new-style markers and any legacy equivalent package you still want to recognize.
6. Add seeder tests for creation, repeatability, and any package dependency failure path.
7. Document the package here when the stock data ships.

## Builder Checklist After Seeding
After running the seeder, verify the package in game:

1. list the created celestial objects and confirm the expected names exist
2. attach them to the target shard with `shard set <shard> celestials ...`
3. configure zone latitude, longitude, elevation, and timezone
4. bind weather controllers and seasons to the correct root celestial
5. use `time` and implementor analysis to confirm the sky descriptions match the intended world frame
