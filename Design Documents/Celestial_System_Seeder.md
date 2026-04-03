# FutureMUD Celestial System Seeder

## Purpose
`DatabaseSeeder/Seeders/CelestialSeeder.cs` ships the stock celestial packages used by new installs and test fixtures. Its goal is not to be a generic astronomy authoring system. Its goal is to create coherent, repeatable modern packages that match the runtime loader and linked observer-frame math.

## Stock Packages

| Package | Objects created | Notes |
| --- | --- | --- |
| `EarthSun` | `Sun` | Self-contained Earth-surface root sun |
| `EarthMoonView` | `PlanetaryMoon`, `PlanetFromMoon`, `SunFromPlanetaryMoon` | Requires exactly one matching Earth-facing root sun |
| `GasGiantMoonView` | `Sun`, `PlanetaryMoon`, `PlanetFromMoon`, `SunFromPlanetaryMoon` | Self-contained Jupiter/Ganymede package |

The seeder no longer emits or recognizes `OldSun` as a supported runtime target.

## Repeatability and Detection
Each seeded object is marked with:

- `SeederPackage`
- `SeederRole`

`ShouldSeedData` uses those markers plus legacy-equivalent constant matching where needed. The Earth moon legacy detection now expects the corrected modern lunar constants, including the moon semi-major axis and the parent-body sidereal terms.

## Seeder Prompt Guidance
The seeder now gives calendar-aware epoch guidance instead of assuming Gregorian examples.

- It resolves the chosen calendar and formats examples using that calendar's authored date style
- pressing `Enter` on an epoch question accepts the displayed suggested default
- start-of-year prompts use the first day of the first month in the selected calendar's current year
- the Earth moon prompt uses day 21 of the first month in the selected calendar's current year as the stock full-moon reference
- the Ganymede prompt uses the first day of the first month in the selected calendar's current year as the stock epoch-aligned reference

Typical examples are:

- Gregorian-family calendars: `01/january/year` or `january/01/year`
- Middle-Earth calendars: `01/yestare/year`
- Mission calendar: `01/ignis/year`

For the stock packages, the suggested defaults are:

- Earth-facing sun: first day of the first month
- Earth moon: day 21 of the first month
- Jupiter-facing sun: first day of the first month
- Ganymede: first day of the first month

## Earth Stock Constants

### Earth-facing `Sun`

| Field | Value |
| --- | --- |
| `CelestialDaysPerYear` | `365.24` |
| `MeanAnomalyAngleAtEpoch` | `6.24006` |
| `AnomalyChangeAnglePerDay` | `0.017202` |
| `EclipticLongitude` | `1.796595` |
| `EquatorialObliquity` | `0.409093` |
| `OrbitalEccentricity` | `0.016713` |
| `OrbitalSemiMajorAxis` | `149597870.7` |
| `ApparentAngularRadius` | `0.004654793` |
| `DayNumberAtEpoch` | `2451545.0` |
| `SiderealTimeAtEpoch` | `4.889488` |
| `SiderealTimePerDay` | `6.300388` |
| `PeakIllumination` | `98000.0` |
| `AlphaScatteringConstant` | `0.05` |
| `BetaScatteringConstant` | `0.035` |
| `PlanetaryRadius` | `6378.0` |
| `AtmosphericDensityScalingFactor` | `6.35` |

### Earth moon-view package

| Object | Field | Value |
| --- | --- | --- |
| `PlanetaryMoon` | `CelestialDaysPerYear` | `29.530588` |
| `PlanetaryMoon` | `MeanAnomalyAngleAtEpoch` | `2.355556` |
| `PlanetaryMoon` | `AnomalyChangeAnglePerDay` | `0.228027` |
| `PlanetaryMoon` | `ArgumentOfPeriapsis` | `5.552765` |
| `PlanetaryMoon` | `LongitudeOfAscendingNode` | `2.18244` |
| `PlanetaryMoon` | `OrbitalInclination` | `0.0898` |
| `PlanetaryMoon` | `OrbitalEccentricity` | `0.0549` |
| `PlanetaryMoon` | `OrbitalSemiMajorAxis` | `384400.0` |
| `PlanetaryMoon` | `DayNumberAtEpoch` | `2451545.0` |
| `PlanetaryMoon` | `SiderealTimeAtEpoch` | `4.889488` |
| `PlanetaryMoon` | `SiderealTimePerDay` | `6.300388` |
| `PlanetaryMoon` | `FullMoonReferenceDay` | `0.0` |
| `PlanetFromMoon` | `AngularRadius` | `0.0165` |
| `PlanetFromMoon` | `SunAngularRadius` | `0.004654793` |

The important correction is that the moon-view package now uses the parent body's sidereal rotation terms, not the moon's mean motion, for local sidereal time.

## Gas Giant Stock Constants

### Jupiter-facing `Sun`

| Field | Value |
| --- | --- |
| `CelestialDaysPerYear` | `10475.8818867393` |
| `MeanAnomalyAngleAtEpoch` | `0.343270671018783` |
| `AnomalyChangeAnglePerDay` | `0.000599776264672575` |
| `EclipticLongitude` | `0.257060466847075` |
| `EquatorialObliquity` | `0.0546288055874225` |
| `OrbitalEccentricity` | `0.048775` |
| `OrbitalSemiMajorAxis` | `778547200.0` |
| `ApparentAngularRadius` | `0.000894416` |
| `DayNumberAtEpoch` | `2451545.0` |
| `SiderealTimeAtEpoch` | `4.97331570355784` |
| `SiderealTimePerDay` | `6.28378508344426` |
| `KepplerC1Approximant` | `0.0967569879178372` |
| `KepplerC2Approximant` | `0.0029273119273445` |
| `KepplerC3Approximant` | `0.000122772356038737` |
| `PeakIllumination` | `3619.8` |
| `PlanetaryRadius` | `69911.0` |

### Ganymede moon-view package

| Object | Field | Value |
| --- | --- | --- |
| `PlanetaryMoon` | `CelestialDaysPerYear` | `7.16683557838692` |
| `PlanetaryMoon` | `MeanAnomalyAngleAtEpoch` | `0.82944303060139` |
| `PlanetaryMoon` | `AnomalyChangeAnglePerDay` | `0.878153082764442` |
| `PlanetaryMoon` | `ArgumentOfPeriapsis` | `4.87986552021969` |
| `PlanetaryMoon` | `LongitudeOfAscendingNode` | `5.96372010319795` |
| `PlanetaryMoon` | `OrbitalInclination` | `0.0355727108921961` |
| `PlanetaryMoon` | `OrbitalEccentricity` | `0.00158762974782861` |
| `PlanetaryMoon` | `OrbitalSemiMajorAxis` | `1070400.0` |
| `PlanetaryMoon` | `DayNumberAtEpoch` | `2451545.0` |
| `PlanetaryMoon` | `SiderealTimeAtEpoch` | `4.97331570355784` |
| `PlanetaryMoon` | `SiderealTimePerDay` | `6.28378508344426` |
| `PlanetaryMoon` | `FullMoonReferenceDay` | `0.0` |
| `PlanetFromMoon` | `AngularRadius` | `0.0653594916439514` |
| `PlanetFromMoon` | `SunAngularRadius` | `0.000894416` |

As with the Earth package, the moon uses the parent body's sidereal frame for local sky positioning.

## Linked Object Rules
The seeder stores linked IDs explicitly:

- `PlanetFromMoon` stores the moon ID and root sun ID
- `SunFromPlanetaryMoon` stores the moon ID and root sun ID

`SunFromPlanetaryMoon` copies the root sun illumination block by default so the moon-local solar view stays aligned with the linked root star unless a custom package deliberately overrides it.
