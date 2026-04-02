# Celestial Type: Sun

## Concept and Real-World Analogy
The current `Sun` celestial type is implemented by `NewSun`.

It represents a star as seen from the surface of a rotating planetary body. The canonical Earth example is "the Sun as seen from Earth." The gas giant stock package also uses the same type to represent "the Sun as seen from Jupiter."

This type is the modern stock solar model and the default choice for new planetary-surface worlds.

## Data Model and Properties
The modern seeder emits the `SunV2` XML shape for this type.

The important groups are:

- calendar and clock feed
- orbital parameters
- illumination parameters
- trigger and description ranges

### Identity and timing properties

| Property | Meaning |
| --- | --- |
| `Name` | Display name of the celestial |
| `Calendar` | Calendar used to determine the current date |
| `Clock` | Feed clock used to determine the fractional time of day |
| `EpochDate` | Calendar date associated with the orbital epoch |

### Orbital properties

| Property | Meaning | Real-world source |
| --- | --- | --- |
| `CelestialDaysPerYear` | Orbital cycle length used by the model | Sidereal or tropical year equivalent chosen for the desired world model |
| `MeanAnomalyAngleAtEpoch` | Mean anomaly at the selected epoch | Almanac or ephemeris mean anomaly at epoch |
| `AnomalyChangeAnglePerDay` | Mean anomaly advance per day | Mean motion in radians per day |
| `EclipticLongitude` | Longitude term added to the true anomaly to reach solar ecliptic longitude in the implementation's convention | Derived from longitude of perihelion or equivalent orbital longitude convention |
| `EquatorialObliquity` | Obliquity between orbital and equatorial planes | Axial tilt / obliquity |
| `DayNumberAtEpoch` | Numeric day count at epoch | Commonly J2000 day number such as `2451545.0` |
| `CurrentDayNumberOffset` | Additional offset added to the calculated current day number | Runtime calibration term; defaults to legacy-compatible `0.5` |
| `SiderealTimeAtEpoch` | Local sidereal time at the epoch day number | Almanac sidereal time at epoch |
| `SiderealTimePerDay` | Sidereal advance per day | Derived from the planet's rotation relative to the stars |
| `KepplerC1Approximant` through `KepplerC6Approximant` | Coefficients used by the true anomaly approximation | Derived from orbital eccentricity or fitted approximation coefficients |

### Illumination properties

| Property | Meaning | Real-world source |
| --- | --- | --- |
| `PeakIllumination` | Maximum direct illumination in lux-like game units | Surface insolation target or chosen gameplay value |
| `AlphaScatteringConstant` | Direct atmospheric attenuation coefficient | Tuned atmospheric parameter |
| `BetaScatteringConstant` | Scattered-light attenuation coefficient | Tuned atmospheric parameter |
| `AtmosphericDensityScalingFactor` | Effective atmospheric depth scale | Tuned atmospheric parameter |
| `PlanetaryRadius` | Planet radius used in the scattering equations | Planet radius in km-like units consistent with the model |

### Presentation properties

| Property | Meaning |
| --- | --- |
| `Triggers` | Elevation-angle thresholds for rise, set, and other sky echoes |
| `ElevationDescriptions` | Human-facing phrases keyed by elevation |
| `AzimuthDescriptions` | Human-facing phrases keyed by azimuth |

## Mapping to Real Astronomical Data
This type is intentionally builder-friendly. Most of its orbital fields correspond to values you can obtain directly from astronomical almanacs, ephemerides, or orbital-element tables.

Practical mapping guidance:

- `MeanAnomalyAngleAtEpoch` is the mean anomaly `M` at the chosen epoch.
- `AnomalyChangeAnglePerDay` is the mean motion `n`.
- `EclipticLongitude` is the implementation's longitude term that, together with `TrueAnomaly + pi`, yields the sun's apparent ecliptic longitude. In practice you usually derive it from the longitude of perihelion or equivalent heliocentric/orbital longitude convention being used by the source data.
- `EquatorialObliquity` is the planet's obliquity.
- `SiderealTimeAtEpoch` and `SiderealTimePerDay` come from the planet's sidereal rotation model.
- `DayNumberAtEpoch` should match the date numbering convention used by the source ephemeris.
- `CurrentDayNumberOffset` is usually left at the default unless you have a specific reason to alter the day-number convention.

For Earth-like content, J2000-derived element tables are usually the easiest source.

## Calculation Pipeline
The runtime pipeline is explicit and mirrors standard astronomy steps.

### 1. Current day number
The type computes:

`CurrentDayNumber = (Calendar.CurrentDate - EpochDate).Days + Clock.CurrentTime.TimeFraction + DayNumberAtEpoch + CurrentDayNumberOffset`

This is the fundamental time input for the orbital calculations.

`CurrentCelestialDay` is also computed from the same date-and-time source but wrapped by `CelestialDaysPerYear`.

### 2. Mean anomaly
The mean anomaly is:

`M = MeanAnomalyAngleAtEpoch + AnomalyChangeAnglePerDay * (dayNumber - DayNumberAtEpoch)`

The result is wrapped into `[0, 2*pi)`.

### 3. True anomaly
The type does not numerically solve Kepler's equation on every call. Instead it applies a configurable trigonometric approximation:

`v = M + C1*sin(M) + C2*sin(2M) + C3*sin(3M) + C4*sin(4M) + C5*sin(5M) + C6*sin(6M)`

The coefficients are the `KepplerC*Approximant` fields.

### 4. Ecliptic longitude of the sun
The runtime computes:

`lambda_sun = v + EclipticLongitude + pi`

and wraps the result into `[0, 2*pi)`.

### 5. Right ascension
The type converts ecliptic longitude to right ascension using the obliquity:

`RA = atan2(sin(lambda_sun) * cos(obliquity), cos(lambda_sun))`

### 6. Declination
Declination is:

`Dec = asin(sin(lambda_sun) * sin(obliquity))`

### 7. Sidereal time
Local sidereal time is:

`LST = SiderealTimeAtEpoch + SiderealTimePerDay * (dayNumber - DayNumberAtEpoch) + longitude`

wrapped into `[0, 2*pi)`.

### 8. Hour angle
The hour angle is:

`HA = LST - RA`

### 9. Altitude
For a zone latitude `phi`:

`Altitude = asin(sin(phi)*sin(Dec) + cos(phi)*cos(Dec)*cos(HA))`

In this subsystem, the cached field name `LastAscensionAngle` stores this local altitude/elevation angle.

### 10. Azimuth
Azimuth is computed as:

`Azimuth = atan2(sin(HA), cos(HA)*sin(phi) - tan(Dec)*cos(phi))`

### 11. Illumination
The modern sun types use a two-part atmosphere/scattering model:

- `U`
- `L`
- `H`
- `RhoH`
- `E1`
- `E2`

The final illumination is:

`CurrentIllumination = E1 + E2`

Conceptually:

- `E1` is the direct sunlight term
- `E2` is the atmospheric scattering term

### 12. Movement direction
Direction is determined by comparing current altitude to the altitude one in-game minute earlier.

The minute fraction is derived from:

`Clock.HoursPerDay * Clock.MinutesPerHour`

with a legacy safety fallback to `1 / 1440` if the clock data is unavailable.

### 13. Time of day
`CurrentTimeOfDay(...)` uses:

- current altitude sign
- small threshold bands near the horizon
- whether the object is ascending or descending

to classify the result as morning, afternoon, dawn, dusk, or night.

## Seeder and Testing Considerations
This is the type used by the stock seeder for:

- the Earth-facing sun package
- the Jupiter-facing sun inside the gas giant moon package

When authoring or testing this type:

- verify the chosen calendar and feed clock make sense together
- verify `EpochDate`, `DayNumberAtEpoch`, and `CurrentDayNumberOffset` are internally consistent
- add numeric regression tests if you change any step of the calculation chain
- add non-24x60 clock tests if you touch motion or time-fraction logic

## Known Caveats and Implementation Notes
- `CurrentDayNumberOffset` exists to preserve legacy-compatible day-number behavior while making the assumption configurable.
- `EclipticLongitude` is the field most likely to confuse builders because source data often publishes related but not identically named longitude terms.
- The type models a star from one planetary-surface frame. If you need the same star from a moon frame, use `SunFromPlanetaryMoon` rather than trying to reuse this object directly.
- Time-of-day classification is a gameplay-oriented interpretation layered on top of the astronomy math.
