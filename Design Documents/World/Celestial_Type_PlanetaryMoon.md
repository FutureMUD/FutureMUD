# Celestial Type: PlanetaryMoon

## Runtime Role
`PlanetaryMoon` models a moon as seen from the surface of its parent world.

It is the planet-observer half of the linked moon-view system.

## Key Data
The authored fields are:

- orbital cycle length and mean motion
- argument of periapsis
- longitude of ascending node
- orbital inclination
- orbital eccentricity
- orbital semi-major axis
- day number at epoch
- sidereal epoch and sidereal rate
- full-moon reference day
- peak illumination

## Sidereal Model
`PlanetaryMoon` local sidereal time is based on the parent world's sidereal rotation model.

That means the moon package should seed:

- `SiderealTimeAtEpoch` from the parent body
- `SiderealTimePerDay` from the parent body

It should not reuse the moon's orbital mean motion for local sidereal time. The corrected Earth and gas-giant stock packages now use the parent-world sidereal terms.

## Calculation Pipeline
The runtime flow is:

1. Compute current day number and wrapped cycle day from the supplied calendar and clock.
2. Compute mean anomaly and a second-order true-anomaly approximation.
3. Rotate the orbital position into equatorial coordinates using the ascending node, periapsis, and inclination.
4. Compute local sidereal time from the authored sidereal terms plus observer longitude.
5. Convert to hour angle, elevation, and azimuth.
6. Compute illumination from a simple phase-based Lambertian model.

`PositionVector(dayNumber)` returns a physical vector using the authored orbital semi-major axis and eccentricity so linked moon-view transforms can work with relative positions rather than unit directions.

## Phase Naming
The phase cycle is anchored at `FullMoonReferenceDay`.

The runtime phase order for increasing cycle fraction is:

- `Full`
- `WaningGibbous`
- `LastQuarter`
- `WaningCrescent`
- `New`
- `WaxingCrescent`
- `FirstQuarter`
- `WaxingGibbous`

That order matches a cycle measured forward from a known full moon.

## Time of Day
`PlanetaryMoon` never defines a zone's time of day. `CurrentTimeOfDay(...)` always returns `Night`.
