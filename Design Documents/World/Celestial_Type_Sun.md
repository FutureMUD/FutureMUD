# Celestial Type: Sun

## Runtime Type
Persisted `Sun` records are loaded by `MudSharp.Celestial.NewSun`.

This is the supported modern solar implementation for planetary-surface observer frames.

## Key Data
The important authored groups are:

- timing: `Calendar`, `EpochDate`, `DayNumberAtEpoch`, `CurrentDayNumberOffset`
- orbital terms: mean anomaly, anomaly rate, longitude, obliquity, eccentricity, semi-major axis
- sidereal terms: `SiderealTimeAtEpoch`, `SiderealTimePerDay`
- illumination terms: peak illumination and atmospheric coefficients

The modern XML also stores:

- `OrbitalEccentricity`
- `OrbitalSemiMajorAxis`
- `ApparentAngularRadius`

These are used by the linked moon-view transforms and seeded stock packages.

## Calculation Pipeline
The runtime flow is:

1. Compute `CurrentDayNumber` from calendar date, clock time fraction, epoch day number, and `CurrentDayNumberOffset`.
2. Compute mean anomaly from the day-number delta.
3. Apply the configured trigonometric approximation to get true anomaly.
4. Convert to solar ecliptic longitude.
5. Convert to right ascension and declination.
6. Compute local sidereal time from the supplied geography and the planet sidereal model.
7. Convert to hour angle, elevation, and azimuth.
8. Compute illumination from the elevation angle with the atmospheric-scattering model.

`PositionVector(dayNumber)` returns a physical observer-frame vector using the configured orbital radius, not just a unit direction. That vector is used by `SunFromPlanetaryMoon`.

## Time of Day
`CurrentTimeOfDay(...)` is gameplay-facing and uses:

- elevation above or below the horizon
- whether the sun is ascending or descending
- a twilight cutoff at 12 degrees below the horizon

The corrected night threshold is `-0.20943951023931953` radians. Values above that threshold but still below the horizon are classified as dawn or dusk depending on direction.

## Direction Sampling
Movement direction is computed by comparing the current elevation with the elevation one in-game minute earlier.

The minute fraction uses the actual configured clock dimensions:

`1 / (HoursPerDay * MinutesPerHour)`

with a legacy fallback only if the clock metadata is unavailable.
