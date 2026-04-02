# Celestial Type: OldSun

## Status
`OldSun` is a legacy implementation.

It is still loadable and supported for compatibility with existing worlds, but it is not the modern stock default. New seeded content should use the current `Sun` implementation documented in [Celestial Type: Sun](./Celestial_Type_Sun.md).

## Concept and Real-World Analogy
`OldSun` represents a star viewed from the surface of a rotating planet.

The closest Earth analogue is "the Sun as seen from Earth," but the implementation is older and more approximation-driven than the modern `NewSun` class. Rather than building the local sky position from a more explicit ephemeris-style chain of mean anomaly, true anomaly, ecliptic longitude, right ascension, and declination, `OldSun` uses an analemma-oriented approximation and minute-based yearly progression.

This makes it useful for legacy content, but less convenient for builders who want to map fields directly from modern astronomy tables.

## Data Model and Properties
The persisted XML shape is centered on three main sections:

- `Configuration`
- `Illumination`
- `Orbital`

### Configuration properties

| Property | Meaning |
| --- | --- |
| `MinutesPerDay` | Number of clock minutes in one local day |
| `MinutesPerYear` | Number of clock minutes in one local year |
| `MinutesPerYearFraction` | Fractional-minute correction used for year drift handling |
| `OrbitalDaysPerYear` | Number of orbital day units in a year |
| `YearsBetweenFractionBumps` | Interval for applying the fractional-year correction |

### Illumination properties

| Property | Meaning |
| --- | --- |
| `PeakIllumination` | Peak direct illumination when the sun is effectively overhead |
| `AlphaScatteringConstant` | Atmospheric attenuation coefficient for direct light |
| `BetaScatteringConstant` | Atmospheric attenuation coefficient for scattered light |
| `PlanetaryRadius` | Planet radius used by the light-scattering model |
| `AtmosphericDensityScalingFactor` | Effective atmosphere depth term used by the light model |

### Orbital properties

| Property | Meaning |
| --- | --- |
| `OrbitalEccentricity` | Eccentricity of the orbit |
| `OrbitalInclination` | Axial tilt relative to the orbital plane in the old model's terminology |
| `OrbitalRotationPerDay` | Planet rotation per day in radians |
| `AltitudeOfSolarDisc` | Apparent solar disc altitude adjustment used by legacy logic |
| `DayNumberOfVernalEquinox` | Day number reference used by the analemma Y-axis calculation |
| `DayNumberStaticOffsetAxial` | Static phase offset for the axial component |
| `DayNumberStaticOffsetElliptical` | Static phase offset for the elliptical component |

## Mapping to Real Astronomical Data
`OldSun` is harder to author directly from modern almanacs than `NewSun`.

Builders can still approximate the values from real data:

- `OrbitalEccentricity` maps directly to standard orbital eccentricity.
- `OrbitalInclination` is used as the effective obliquity or axial tilt term in the model.
- `OrbitalRotationPerDay` should be the planet's rotation in radians per solar day.
- `MinutesPerDay` and `MinutesPerYear` should match the in-game clock and calendar model, not necessarily a real civil clock.
- `DayNumberOfVernalEquinox`, `DayNumberStaticOffsetAxial`, and `DayNumberStaticOffsetElliptical` are calibration values rather than standard published ephemeris fields.

For Earth-like content, these phase-offset fields were historically tuned to make the apparent yearly solar motion look correct in game rather than copied from a modern astronomical ephemeris.

## Calculation Pipeline
The legacy solar pipeline is built around minute counters and correction terms.

### 1. Convert minutes into a day number
`CalculateDayNumber(minutes)` returns:

`1.0 + minutes / MinutesPerDay`

This day number is then used as the phase input for the yearly approximation functions.

### 2. Compute elliptical correction
The old model computes:

- `lambda = 2*pi / OrbitalDaysPerYear * wrapped(day + DayNumberStaticOffsetElliptical)`
- `sigma = lambda + 2*e*sin(lambda)`

The elliptical noon correction is:

`(lambda - sigma) * (MinutesPerDay / OrbitalRotationPerDay)`

This is the old model's correction for orbital eccentricity.

### 3. Compute axial tilt correction
The model computes an axial phase term `E` and then applies:

`(E - atan(sin(E) * cos(OrbitalInclination) / cos(E))) * (MinutesPerDay / OrbitalRotationPerDay)`

This is the old model's correction for the planet's axial tilt.

### 4. Combine noon error
`CalculateNoonError()` is the sum of the elliptical and axial corrections.

### 5. Compute local hour angle
The model computes a local time fraction from:

- current minute of day
- local longitude

and converts it to a local hour angle.

### 6. Compute analemma Y axis
The model computes:

`asin(sin(sigma(day) - sigma(vernalEquinoxDay)) * sin(OrbitalInclination))`

This is the effective solar declination term in the old model.

### 7. Compute elevation angle
The current elevation uses the local hour angle, the analemma Y-axis term, zone latitude, and an elevation-above-sea-level correction.

### 8. Compute azimuth angle
Azimuth is derived from the same local hour angle and analemma Y-axis value, with the sign branch depending on whether the hour angle is east or west of noon.

### 9. Compute illumination
`OldSun` uses the same general illumination family as the modern solar implementations:

- `U`
- `L`
- `H`
- `RhoH`
- `E1`
- `E2`

The direct and scattered light components are summed to produce current illumination.

### 10. Determine movement direction and time of day
Movement direction is determined by comparing the current elevation to the value one minute earlier.

Time of day is then inferred from:

- elevation sign
- disc altitude threshold
- whether the object is currently ascending or descending

## Seeder and Testing Considerations
`OldSun` is not the modern seeded default.

That means:

- new stock seeder content should not target this type
- custom worlds that still use it should treat it as legacy-supported
- changes to `OldSun` should be conservative because there is no dedicated modern numeric regression suite comparable to `NewSun`

If you must seed or hand-author one for a legacy world, prefer copying from a known-good existing definition and adjusting only a small number of parameters.

## Known Caveats and Implementation Notes
- `OldSun` is minute-driven rather than explicitly calendar-date-and-time-fraction driven in the same way as `NewSun`.
- Several fields are calibration offsets rather than straightforward almanac values.
- The type is still useful for backwards compatibility, but it is not the easiest type for new builder-facing authoring.
- Illumination uses the same broad scattering model family as the newer solar types, so light-level behavior is more modern-looking than the orbital approximation beneath it.
- If you are deciding between `OldSun` and `Sun` for new work, choose `Sun` unless you explicitly need legacy behavior.
